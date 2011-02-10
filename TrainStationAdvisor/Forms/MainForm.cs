using System;
using System.Linq;
using System.IO;
using System.Text;
//using System.Media;
using System.Windows.Forms;
using TrainStationAdvisor.ClassLibrary;
//using Microsoft.WindowsMobile.Status;
//using Microsoft.WindowsMobile.Samples.Location;
using System.Reflection;
using Microsoft.Win32;
using WindowlessControls;
using WindowlessControls.CommonControls;
using Microsoft.WindowsMobile.PocketOutlook;
using OpenNETCF.WindowsCE;
using StedySoft.SenseSDK.DrawingCE;
using StedySoft.SenseSDK;
using StedySoft.SenseSDK.Localization;
using System.Globalization;

namespace TrainStationAdvisor
{
    public partial class MainForm : Form
    {
        private SMSSender m_SMSSender;
        //private GpsDeviceState m_GpsDeviceState = null;
        //private GpsPosition m_GpsPosition = null;
        //private Gps m_Gps = null;
        private GPS.Position m_GpsPosition;
        private GPS m_Gps;
        private CellIdCollector m_CellIdCollector;
        private GeoLocation m_GeoLocation;

        //private Vibrate m_Vibrate;
        private ItemsControl m_ItemsControl;
        private Options m_Options;


        private static EventHandler m_UpdateDataHandler;

        private double m_Latitude;
        private double m_Longitude;

        private bool m_LocationServiceStarted;
        private bool m_SMSSent;
        //private bool m_RingerOff;

        //private Timer m_Timer;
        private IntPtr m_PowerRequirements;
        private bool m_DevicePowered;
        //private SystemState m_SystemState;

        private static Logger m_Logger;
        public static Logger Logger
        {
            get
            {
                return m_Logger;
            }
        }

        public MainForm()
        {
            m_Logger = new Logger();
            m_Logger.LogFilePath = Path.Combine(EnvironmentEx.CallingAssemblyDirectory, "TSA.txt");

            m_DevicePowered = false;
            DeviceManagement.ACPowerApplied += DeviceManagement_ACPowerApplied;
            DeviceManagement.ACPowerRemoved += DeviceManagement_ACPowerRemoved;

            ResourceManager.Instance.Culture = new CultureInfo("en_US");

            InitializeComponent();

            //m_SystemState = new SystemState(SystemProperty.PhoneRingerOff);
            //m_SystemState.Changed += SystemState_Changed;
            m_ItemsControl = new ItemsControl();

            VerticalStackPanelHost scrollHost = new VerticalStackPanelHost();
            StackPanel stack = scrollHost.Control;
            stack.HorizontalAlignment = WindowlessControls.HorizontalAlignment.Stretch;
            WindowlessHost.Control.Controls.Add(scrollHost);
            WindowlessHost.AutoScroll = true;

            scrollHost.Control.Controls.Add(m_ItemsControl);

            m_ItemsControl.ContentPresenter = typeof(StationPresenter);
            m_ItemsControl.Control = new StackPanel();

            m_Options = new Options();
            m_Options.UseGps += chkUseGps_Click;
            m_Options.RouteChanged += Options_RouteChanged;
            m_Options.DebugChanged += Options_DebugChanged;

            m_PowerRequirements = IntPtr.Zero;
            m_SMSSent = false;
            m_LocationServiceStarted = false;

            bool _Error = false;

            m_UpdateDataHandler = UpdateData;
            //m_Vibrate = new Vibrate();

            //m_Timer = new Timer();
            //m_Timer.Interval = GetInterval();
            //m_Timer.Tick += Timer_Tick;

            try
            {
                m_SMSSender = new SMSSender();
                m_SMSSender.SMSCompleted += SMSSender_SMSCompleted;
            }
            catch (Exception e)
            {
                _Error = true;
                MessageBox.Show(e.Message);
            }

            if (!_Error)
            {
                //m_Timer.Enabled = chkAlwaysOn.Checked;

                LoadStationList(false);

                if (Options.AutoStartLocationService && !m_LocationServiceStarted)
                {
                    StartLocationService();
                    m_LocationServiceStarted = true;
                }

                menuItem1.Text = (m_LocationServiceStarted ? "Stop" : "Start");

                txtStatus.Text = string.Format("{0} Running on Emulator", !NativeMethods.IsEmulator() ? "Not" : string.Empty).Trim();

                txtStatus.Visible = Options.DebugOn;
            }
            else
            {
                Close();
                Application.Exit();
            }
        }

        private void DeviceManagement_ACPowerRemoved()
        {
            m_DevicePowered = false;
        }

        private void DeviceManagement_ACPowerApplied()
        {
            m_DevicePowered = true;
        }

        private void Options_DebugChanged(object sender, System.EventArgs e)
        {
            txtStatus.Visible = Options.DebugOn;
        }

        private void LoadStationList(bool reset)
        {
            Logger.Log("Loadstation - reset {0}", reset);

            if (reset)
            {
                m_ItemsControl.Items.Clear();
            }

            foreach (Station _Station in m_Options.GetStations())
            {
                m_ItemsControl.Items.Add(_Station);
            }

            Logger.Log("Loadstation - finished");
        }

        //private static int GetInterval()
        //{
        //    int retVal = 1000;
        //    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"\SYSTEM\CurrentControlSet\Control\Power");
        //    object oBatteryTimeout = key.GetValue("BattPowerOff");
        //    object oACTimeOut = key.GetValue("ExtPowerOff");
        //    object oScreenPowerOff = key.GetValue("ScreenPowerOff");

        //    if (oBatteryTimeout is int)
        //    {
        //        int v = (int)oBatteryTimeout;
        //        if (v > 0)
        //            retVal = Math.Min(retVal, v);
        //    }
        //    if (oACTimeOut is int)
        //    {
        //        int v = (int)oACTimeOut;
        //        if (v > 0)
        //            retVal = Math.Min(retVal, v);
        //    }
        //    if (oScreenPowerOff is int)
        //    {
        //        int v = (int)oScreenPowerOff;
        //        if (v > 0)
        //            retVal = Math.Min(retVal, v);
        //    }

        //    return retVal * 9 / 10; 
        //}

        private static void Timer_Tick(object sender, System.EventArgs e)
        {
            NativeMethods.SystemIdleTimerReset();
        }

        //private void SystemState_Changed(object sender, ChangeEventArgs args)
        //{
        //    m_RingerOff = SystemState.PhoneRingerOff;
        //    //m_DeviceLocked = SystemState.DeviceLocked;
        //}

        private void StartLocationService()
        {
            Logger.Log("StartLocationService - GpsActive {0}", Options.GpsActive);

            if (Options.GpsActive)
            {
                StopCellIDCollector();
                StartGps();
            }
            else
            {
                StopGps();
                StartCellIDCollector();
            }

            Logger.Log("StartLocationService - Finished");
        }

        private void StartCellIDCollector()
        {
            if (null == m_CellIdCollector)
            {
                bool _AlwaysOn = (NativeMethods.IsEmulator() || (!NativeMethods.IsEmulator() && m_DevicePowered));

                m_CellIdCollector = new CellIdCollector(_AlwaysOn);
                m_CellIdCollector.WorkUnattended = Options.Unattended;
                m_CellIdCollector.CellIdFound += CellIdCollector_CellIdFound;
                m_CellIdCollector.Logger = Logger;

                Logger.Log("New CellIdCollector - AlwaysOn : {0}", _AlwaysOn);
            }
            m_CellIdCollector.Start();

            Logger.Log("CellIDCollector Started");
        }

        private void CellIdCollector_CellIdFound(object sender, TrainStationAdvisor.ClassLibrary.EventArgs.CellIdCollectorEventArgs e)
        {
            Logger.Log("Cell and Location Found");

            m_GeoLocation = e.GeoLocation;
            BeginInvoke(m_UpdateDataHandler);
            //Invoke(m_UpdateDataHandler);
        }

        private void StopCellIDCollector()
        {
            if (null != m_CellIdCollector && m_CellIdCollector.Started)
            {
                m_CellIdCollector.Stop();

                //I need to create always new CellCollector 
                //in order to use the switch AlwaysOn.

                m_CellIdCollector.CellIdFound -= CellIdCollector_CellIdFound;
                m_CellIdCollector.Dispose();
                m_CellIdCollector = null;
           }
        }

        private void StopGps()
        {
            if (null != m_Gps && m_Gps.Opened)
            {
                m_Gps.Close();
            }
        }

        private void StartGps()
        {
            if (null == m_Gps)
            {
                m_Gps = new GPS();
                //m_Gps.DeviceStateChanged += new DeviceStateChangedEventHandler(Gps_DeviceStateChanged);
                //m_Gps.LocationChanged += new LocationChangedEventHandler(Gps_LocationChanged);
                m_Gps.LocationChanged += Gps_LocationChanged;
            }
            m_Gps.Open();
        }

/*
        private static void Gps_DeviceStateChanged(object sender, EventArgs.DeviceStateChangedEventArgs args)
        {
            //m_GpsDeviceState = args.DeviceState;

            //Invoke(m_UpdateDataHandler);
        }

        //private void Gps_LocationChanged(object sender, LocationChangedEventArgs args)
        //{
        //    m_GpsPosition = args.Position;

        //    Invoke(m_UpdateDataHandler);
        //}
*/
        private void Gps_LocationChanged(object sender, TrainStationAdvisor.ClassLibrary.EventArgs.GPSLocationChangedEventArgs args)
        {
            m_GpsPosition = args.Position;

            //Invoke(m_UpdateDataHandler);
            BeginInvoke(m_UpdateDataHandler);
        }


        private void UpdateData(object sender, System.EventArgs args)
        {
            Logger.Log("Updating data onscreen");

            bool _CheckStation = true;
            StringBuilder _Str = new StringBuilder();

            if (m_Gps != null && m_Gps.Opened)
            {
                //if (m_GpsDeviceState != null)
                //{
                //    _Str.AppendLine(string.Format("{0} {1}, {2}", m_GpsDeviceState.FriendlyName, m_GpsDeviceState.ServiceState,m_GpsDeviceState.DeviceState));
                //}

                if (m_GpsPosition != null)
                {
                    if (GPS.Position.LatitudeValid)
                    {
                        _Str.AppendLine(string.Format("Latitude: {0}", m_GpsPosition.Latitude));
                        m_Latitude = m_GpsPosition.Latitude;
                    }

                    if (GPS.Position.LongitudeValid)
                    {
                        _Str.AppendLine(string.Format("Longitude: {0}", m_GpsPosition.Longitude));
                        m_Longitude = m_GpsPosition.Longitude;
                    }

                    if (m_GpsPosition.SatellitesInSolutionValid &&
                        m_GpsPosition.SatellitesInViewValid &&
                        m_GpsPosition.SatelliteCountValid)
                    {
                        //_Str.AppendLine(string.Format("Satellite Count: {0}/{1} ({2})", m_GpsPosition.GetSatellitesInSolution().Length,
                        //    m_GpsPosition.GetSatellitesInView().Length, m_GpsPosition.SatelliteCount));

                        _Str.AppendLine(string.Format("Satellite Count: {0}", m_GpsPosition.NumberOfSatellites));
                    }

                    if (m_GpsPosition.TimeValid)
                    {
                        _Str.AppendLine(string.Format("Time: {0}", m_GpsPosition.Time.ToString()));
                    }

                    if (m_GpsPosition.SpeedValid)
                    {
                        _Str.AppendLine(string.Format("Speed: {0} km/h", m_GpsPosition.Speed.ToString()));
                    }
                }
            }
            else if (null != m_CellIdCollector && null != m_GeoLocation)
            {
                m_Latitude = m_GeoLocation.Latitude;
                m_Longitude = m_GeoLocation.Longitude;

                if (!m_GeoLocation.IsValid)
                {
                    _CheckStation = false;
                }
                else
                {
                    _Str.AppendLine(string.Format("Latitude: {0}", m_Latitude));
                    _Str.AppendLine(string.Format("Longitude: {0}", m_Longitude));
                }
            }
            else
            {
                _CheckStation = false;
            }

            if (_CheckStation)
            {
                CheckStation();
            }

            txtStatus.Text = _Str.ToString();
        }

        private void CheckStation()
        {
            Logger.Log("CheckStation - Start");

            double _AlertDistance = m_Options.AlertDistance;

            foreach (Station _Station in m_ItemsControl.Items)
            {
                //m_Latitude = 48.323644;
                //m_Longitude = 10.050087;

                //Have to check dinamically of any changes made on the options dialog
                var _StationChanges = from _StationChange in m_Options.GetStations()
                                      where _StationChange.StationID == _Station.StationID
                                      select _StationChange;

                if (null != _StationChanges.SingleOrDefault())
                {
                    _Station.PerformAlert = _StationChanges.SingleOrDefault().PerformAlert;
                }
                else
                {
                    _Station.PerformAlert = false;
                }

                _Station.RefreshDistanceTo(m_Latitude, m_Longitude);
                _Station.AlertDistance = _AlertDistance;

                m_ItemsControl.Items.GetContentPresenter(_Station).RefreshContent();
                
                Application.DoEvents();

                if (_Station.PerformAlert && _AlertDistance > 0 && _Station.DistanceToDestination < _AlertDistance)
                {
                    if (m_Options.SmsActive && !m_SMSSent)
                    {
                        //TODO : Calculate speed without GPS information, only with distance.
                        //if (m_GpsPosition.SpeedValid)
                        //{
                        //    _EstimatedTime = Utils.ETAString(", estaria llegando en {0} {1}", Utils.EstimatedTimeOfArrival(m_GpsPosition.Speed, _Distance));
                        //}

                        //TODO : Multilanguage
                        DistanceTypes _DistanceType = _Station.DistanceType;
                        string _DistanceTypeFormat = "I'm {0:0.00} km from {1}";
                        if (_DistanceType == DistanceTypes.Meters)
                            _DistanceTypeFormat = "I'm {0:0.00} meters from {1}";

                        string _SMSMessage = string.Format(_DistanceTypeFormat, 
                            (_DistanceType == DistanceTypes.Meters ? Convert.ToInt32(_Station.DistanceToDestination) : _Station.DistanceToDestination), 
                            _Station.Name);

                        SendSMS(_SMSMessage);
                    }

                    Logger.Log("Video Power on");

                    //Power on the device if stand by
                    if (NativeMethods.GetVideoPowerState() != NativeMethods.VideoPowerState.VideoPowerOn)
                        NativeMethods.SetVideoPowerState(NativeMethods.VideoPowerState.VideoPowerOn);

                    //Doesnt matter if the Ringer is on/off, 
                    //when off it will vibrates when on will do both.
                    Vibrate.PlaySync(150, 150, 150, 150);
                    Vibrate.PlaySnd();

                    Logger.Log("Vibrate and Sound played");
                }
            }

            NativeMethods.PowerPolicyNotify(NativeMethods.PPNMessage.PPN_UNATTENDEDMODE, 0);

            Logger.Log("CheckStation - Finished");

            //string _Message = string.Empty;
            //string _EstimatedTime = string.Empty;
            //Color _Color = Color.Green;
            //double _Distance = 0;
            //double _AlertInstance = m_Options.GetAlert();
            //bool _Alert = false;

            //try
            //{
            //    Station _Station = m_Options.GetStation();
            //    if (null != _Station)
            //    {
            //        //TEST
            //        //_Station.Latitude = 48.323644;
            //        //_Station.Longitude = 10.050087;
                    
            //        txtStatus.Text = string.Format("{0} Lat:{1}-Long:{2} <-> Current Lat:{3}-Lon:{4}", _Station.Name, _Station.Latitude, _Station.Longitude, m_Latitude, m_Longitude);

            //        _Distance = _Station.DistanceTo(m_Latitude, m_Longitude);

            //        if (_AlertInstance > 0 && _Distance < _AlertInstance)
            //        {
            //            _Color = Color.Red;
            //            _Alert = true;
            //        }

            //        //if (m_GpsPosition.SpeedValid)
            //        //{
            //        //    _EstimatedTime = Utils.ETAString("ETA: {0} {1}", Utils.EstimatedTimeOfArrival(m_GpsPosition.Speed, _Distance));
            //        //}

            //        //"{0} in {1:0.00} km. {2}"
            //        _Message = string.Format("{0} in {1:0.00} km", _Station.Name, _Distance/*, _EstimatedTime*/);
            //    }
            //    else
            //    {
            //        txtStatus.Text = "Station cannot be found";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _Color = Color.Red;
            //    _Message = ex.Message;
            //}

            //lblAlert.ForeColor = _Color;
            //lblAlert.Text = _Message;

            //if (_Alert)
            //{
            //    //TODO : Recheck this option.
            //    //if (chkSendSMS.Checked && !m_SMSSent)
            //    //{
            //    //    if (m_GpsPosition.SpeedValid)
            //    //    {
            //    //        _EstimatedTime = Utils.ETAString(", estaria llegando en {0} {1}", Utils.EstimatedTimeOfArrival(m_GpsPosition.Speed, _Distance));
            //    //    }

            //    //    //TODO : Multilanguage
            //    //    string _DistanceType = (((_Distance * 1000) < 1000) ? "metros" : "km");
            //    //    string _SMSMessage = string.Format("Estoy a {0} {1} de {2}", _Distance, _DistanceType, _Station, _EstimatedTime);
            //    //    SendSMS(_SMSMessage);
            //    //}

            //    //Doesnt matter if the Ringer is on/off, 
            //    //when off it will vibrates when on will do both.
            //    //m_Vibrate.Play();
            //    //Application.DoEvents();
            //    //m_Vibrate.Play();
            //    //Application.DoEvents();
            //    //m_Vibrate.Play();

            //    Vibrate.PlaySync(300, 300, 300, 300);
            //    Vibrate.PlaySnd();
            //}
        }

        private void SendSMS(string AMessage)
        {
            if (null != m_SMSSender)
            {
                Contact _Contact = m_Options.GetContact();

                if (null != _Contact)
                {
                    m_SMSSender.SendMessage(AMessage, _Contact.MobileTelephoneNumber);
                }
            }
        }

        private void SMSSender_SMSCompleted(object sender, SMSSender.SMSEventArgs e)
        {
            if (e.Error != string.Empty)
            {
                ShowMessage(e.Error);
            }
            else
            {
                m_SMSSent = true;
            }
        }

        private void ShowMessage(string AMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowMessage(AMessage)));
            }
            else
            {
                //MessageBox.Show(AMessage);
                SenseAPIs.SenseMessageBox.Show(AMessage, Text, SenseMessageBoxButtons.OK);
            }
        }

        private void menuItem1_Click(object sender, System.EventArgs e)
        {
            if (m_LocationServiceStarted)
            {
                StopCellIDCollector();
                StopGps();
            }
            else
            {
                StartLocationService();
            }

            m_LocationServiceStarted = !m_LocationServiceStarted;

            menuItem1.Text = (m_LocationServiceStarted ? "Stop" : "Start");
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            Size = Screen.PrimaryScreen.WorkingArea.Size;
/*
            CEDEVICE_POWER_STATE currentPowerState;
            NativeMethods.GetDevicePower(GpsDeviceName, NativeMethods.DevicePowerFlags.POWER_NAME, out currentPowerState);
            m_PowerRequirements = NativeMethods.SetPowerRequirement(GpsDeviceName, NativeMethods.CEDEVICE_POWER_STATE.D0, NativeMethods.DevicePowerFlags.POWER_NAME, IntPtr.Zero, 0);
            if (currentPowerState == NativeMethods.CEDEVICE_POWER_STATE.D4)
                System.Threading.Thread.Sleep(500);
 */ 

        }

        private void chkUseGps_Click(object sender, System.EventArgs e)
        {
            StartLocationService();
        }

        private void Options_RouteChanged(object sender, System.EventArgs e)
        {
            LoadStationList(true);
        }

        private void mnuAbout_Click(object sender, System.EventArgs e)
        {
            using (About _About = new About
            {
                ApplicationName = "TrainStation Advisor",
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                ApplicationDescription = "Alerts when a defined Station its near.",
                Author = "Leonardo Gutierrez",
                ContactInfo = "leonardomgutierrez@googlemail.com"
            })
            {
                _About.ShowDialog();
            }
        }

        private void mnuOptions_Click(object sender, System.EventArgs e)
        {
            m_Options.ShowDialog();
        }

        private void menuItem3_Click(object sender, System.EventArgs e)
        {
            Close();
            //Application.Exit();
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //    //NativeMethods.PowerPolicyNotify(NativeMethods.PPNMessage.PPN_UNATTENDEDMODE, 0);
            e.Cancel = (SenseAPIs.SenseMessageBox.Show("Would you like to close the Application?", Text, SenseMessageBoxButtons.YesNo) == DialogResult.No);
        }
    }
}