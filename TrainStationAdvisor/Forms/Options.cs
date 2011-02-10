using System;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using Microsoft.WindowsMobile.PocketOutlook;
using TrainStationAdvisor.ClassLibrary;
using StedySoft.SenseSDK;
using StedySoft.SenseSDK.DrawingCE;
using StedySoft.SenseSDK.Localization;
using System.Reflection;
using System.IO;

namespace TrainStationAdvisor
{
    public partial class Options : Form
    {
        private Storage m_Storage;
        private DataSet m_RoutesDataSet;
        private DataSet m_StationsDataSet;
        private OutlookSession m_OutlookSession;

        public static bool GpsActive
        {
            get
            {
                return Convert.ToBoolean(Settings.GetProperty("UseGps"));
            }
        }

        public bool SmsActive
        {
            get
            {
                return chkSendSMS.Checked;
            }
        }

        public static bool AutoStartLocationService
        {
            get
            {
                return Convert.ToBoolean(Settings.GetProperty("AutomaticStartLocationService"));
            }
        }

        public static bool DebugOn
        {
            get
            {
                return Convert.ToBoolean(Settings.GetProperty("DebugOn"));
            }
        }

        public static bool Unattended
        {
            get
            {
                return Convert.ToBoolean(Settings.GetProperty("Unattended"));
            }
        }

        public double AlertDistance
        {
            get
            {
                string _AlertDistance = Settings.GetProperty("AlertDistance");
                return (_AlertDistance.Length > 0 ? double.Parse(_AlertDistance) : 0);
            }
        }

        public event EventHandler UseGps;
        public event EventHandler RouteChanged;
        public event EventHandler DebugChanged;

        private static bool UseLastStation 
        {
          get
          {
                return Convert.ToBoolean(Settings.GetProperty("LastStationFromRoute"));
          }
        }
        
        public Options()
        {
            InitializeComponent();

            m_OutlookSession = new OutlookSession();
            m_Storage = new Storage();

            SetDefaultForControls();

            LoadRoutes();
            LoadStationsRoutes();
            LoadPhoneNumbers();
        }

        private void SetDefaultForControls()
        {
            AlertControls();

            OptionControls();

            //Controls for the additional features not available in version 1
            chkSendSMS.Checked = Convert.ToBoolean(Settings.GetProperty("SendSMS"));
            chkAlwaysOn.Checked = Convert.ToBoolean(Settings.GetProperty("AlwaysOn"));
            chkAlwaysOn.Visible = false;
        }

        private void OptionControls()
        {
            senseListCtrlOptions.BeginUpdate();

            senseListCtrlOptions.AddItem(SenseFactory.CreateCheckboxItem(new SenseFactory.SensePropertyBag("LastStationFromRoute", "Use Last Station", "Use Last Station from the selected route"), 
                                                                         OnCheckboxStatusChanged));
            
            senseListCtrlOptions.AddItem(SenseFactory.CreateCheckboxItem(new SenseFactory.SensePropertyBag("AutomaticStartLocationService", "Automatic Start", "Automatic Start of Location Service"),
                                         OnCheckboxStatusChanged));

            senseListCtrlOptions.AddItem(SenseFactory.CreateOnOffItem(new SenseFactory.SensePropertyBag("UseGps", "GPS", "Permits the use of the GPS Antenna"), 
                                         OnOnOffItemStatusChanged));

            senseListCtrlOptions.AddItem(new SensePanelDividerItem("DividerItem1", "Internal Debug Options"));

            senseListCtrlOptions.AddItem(SenseFactory.CreateOnOffItem(new SenseFactory.SensePropertyBag("DebugOn", "Debug Window", "Shows the Debug Window"), 
                                         OnOnOffItemStatusChanged));

            senseListCtrlOptions.AddItem(SenseFactory.CreateOnOffItem(new SenseFactory.SensePropertyBag("Unattended", "Work Unattended", "Internal property"),
                                         OnOnOffItemStatusChanged));

            senseListCtrlOptions.EndUpdate();
        }

        private void AlertControls()
        {
            senseListCtrlAlert.BeginUpdate();

            //senseListCtrlAlert.AddItem(SenseFactory.CreateTextboxItem("AlertDistance", "Alert Distance"));
            //senseListCtrlAlert.AddItem(SenseFactory.CreateTrackbarItem(new SenseFactory.SensePropertyBag("AlertDistance", "Alert Distance"), 
            //                           OnTrackbarItemValueChanged));

            senseListCtrlAlert.AddItem(SenseFactory.CreateNumericItem(new SenseFactory.SensePropertyBag("AlertDistance", "Alert Distance", "Tap on the property to open a dialog"),
                                       OnNumericItemValueChanged));

            senseListCtrlAlert.AddItem(SenseFactory.CreateComboItem(new SenseFactory.SensePropertyBag("Routes", "Route", true), 
                                        OnComboSelectedIndexChanged));
            senseListCtrlAlert.AddItem(SenseFactory.CreateComboItem(new SenseFactory.SensePropertyBag("Stations", "Station", false), 
                                        OnComboSelectedIndexChanged));

            senseListCtrlAlert.EndUpdate();
        }

        #region Delegates

        private void OnNumericItemValueChanged(object sender, int value)
        {
            SensePanelNumericItem _Item = (sender as SensePanelNumericItem);

            if (null != _Item)
            {
                switch (_Item.Name)
                {
                    case "AlertDistance":
                        Settings.SetProperty(_Item.Name, value.ToString());
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnOnOffItemStatusChanged(object sender, ItemStatus status)
        {
            SensePanelOnOffItem _Itm = (sender as SensePanelOnOffItem);
            //itm.SecondaryText = string.Format("{0} and {1}", itm.Enabled ? "Enabled" : "Disabled", itm.Status.Equals(ItemStatus.On) ? "On" : "Off");
            //SensePanelOnOffItem _Itm = (senseListCtrl["OnOffItem1"] as SensePanelOnOffItem);
            if (_Itm != null)
            {
                UpdateSetting(_Itm.Name, SenseFactory.StatusToBool(status).ToString());

                switch (_Itm.Name)
                {
                    case "DebugOn":
                        if (null != DebugChanged)
                        {
                            DebugChanged(sender, new System.EventArgs());
                        }
                        break;
                    case "UseGps":
                        if (null != UseGps)
                        {
                            UseGps(sender, new System.EventArgs());
                        }
                        break;
                    default:
                        break;
                }

                //_Itm.Enabled = Status.Equals(ItemStatus.On);
                //_Itm.SecondaryText = string.Format("{0} and {1}", _Itm.Enabled ? "Enabled" : "Disabled", _Itm.Status.Equals(ItemStatus.On) ? "On" : "Off");
            }
        }

        //private void OnTextBoxItemTextChanged(object sender, System.EventArgs e)
        //{
        //    SensePanelTextboxItem textboxItem = (sender as SensePanelTextboxItem);

        //    if (null != textboxItem)
        //    {
        //        switch (textboxItem.Name)
        //        {
        //            case "AlertDistance":
        //                Settings.SetProperty(textboxItem.Name, textboxItem.Text);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        private void OnCheckboxStatusChanged(object sender, ItemStatus status)
        {
            SensePanelCheckboxItem _Itm = (sender as SensePanelCheckboxItem);
            if (_Itm != null)
            {
                UpdateSetting(_Itm.Name, SenseFactory.StatusToBool(status).ToString());

                switch (_Itm.Name)
                {
                    case "LastStationFromRoute":
                        SensePanelComboItem _Cbo = (senseListCtrlAlert["Stations"] as SensePanelComboItem);
                        if (null != _Cbo)
                        {
                            _Cbo.Enabled = (status == ItemStatus.Off);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnComboSelectedIndexChanged(object sender, int index)
        {
            SensePanelComboItem _Cbo = (sender as SensePanelComboItem);

            if (null != _Cbo)
            {
                if (_Cbo.Name.Contains("Routes"))
                {
                    string _ID = string.Empty;
                    try
                    {
                        _ID = (_Cbo.Items[index] as SensePanelComboItem.Item).Value.ToString();
                    }
                    catch
                    {
                    }

                    if (_ID != string.Empty)
                    {
                        if (null != m_StationsDataSet)
                        {
                            m_StationsDataSet.Tables[0].DefaultView.RowFilter = string.Format("RouteID = {0}", _ID);
                        }
                    }

                    LoadComboBoxValues("Stations", true);

                    if (null != RouteChanged)
                    {
                        RouteChanged(sender, new System.EventArgs());
                    }
                }
                else
                {
                    //
                }
            }
        }

        //private void OnTrackbarItemValueChanged(object sender, int value)
        //{
        //    SensePanelTrackbarItem _Item = (sender as SensePanelTrackbarItem);

        //    if (null != _Item)
        //    {
        //        switch (_Item.Name)
        //        {
        //            case "AlertDistance":
        //                Settings.SetProperty(_Item.Name, value.ToString());
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    //(this.senseListCtrl["TrackItem02"] as SensePanelTrackbarItem).AnimateValueTo((Sender as SensePanelTrackbarItem).Value);
        //}
        #endregion

        private static void UpdateSetting(string propertyName, string value)
        {
            Settings.SetProperty(propertyName, value);
        }

        private void chkAlwaysOn_Click(object sender, System.EventArgs e)
        {
            UpdateSettings();

            //m_Timer.Enabled = chkAlwaysOn.Checked;

            //StopCellIDCollector();
            //StartCellIDCollector();
        }

        private void menuItem1_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void LoadRoutes()
        {
            if (null != m_Storage)
            {
                m_RoutesDataSet = m_Storage.GetRoutes();

                if (null != m_RoutesDataSet)
                {
                    m_RoutesDataSet.Tables[0].DefaultView.Sort = "ID";

                    LoadComboBoxValues("Routes", false);
                    //cboRoutes.DataSource = m_RoutesDataSet.Tables[0].DefaultView;
                    //cboRoutes.DisplayMember = m_RoutesDataSet.Tables[0].Columns["Name"].ColumnName;
                }
            }
        }

        private void LoadStationsRoutes()
        {
            if (null != m_Storage)
            {
                m_StationsDataSet = m_Storage.GetStationRoutes();

                if (null != m_StationsDataSet)
                {
                    m_StationsDataSet.Tables[0].DefaultView.Sort = "StationID";

                    string _ID = m_RoutesDataSet.Tables[0].DefaultView[0].Row["ID"].ToString();
                    m_StationsDataSet.Tables[0].DefaultView.RowFilter = string.Format("RouteID = {0}", _ID);

                    LoadComboBoxValues("Stations", false);

                    //cboStations.DataSource = m_StationsDataSet.Tables[0].DefaultView;
                    //cboStations.DisplayMember = m_StationsDataSet.Tables[0].Columns["Name"].ColumnName;
                }
            }
        }

        private void LoadPhoneNumbers()
        {
            try
            {
                if (null != m_OutlookSession && m_OutlookSession.Contacts.Items.Count > 0)
                {
                    var _Phones = from Contact _Contact in m_OutlookSession.Contacts.Items
                                  where _Contact.MobileTelephoneNumber.Length > 0
                                  select _Contact;

                    if (null != _Phones && _Phones.Count() > 0)
                    {
                        cboPhoneNr.DataSource = _Phones.ToList();
                        cboPhoneNr.DisplayMember = "MobileTelephoneNumber";
                    }
                }
            }
            catch (Exception e)
            {
                cboPhoneNr.Enabled = false;
                //ShowMessage(string.Format("LoadPhoneNumbers {0}",e.Message));
            }
        }

        private void LoadComboBoxValues(string cboName, bool reset)
        {
            DataSet _DataSet = (cboName.Contains("Routes") ? m_RoutesDataSet : m_StationsDataSet);
            string _Value = (cboName.Contains("Routes") ? "ID" : "StationID");

            SensePanelComboItem _Cbo = (senseListCtrlAlert[cboName] as SensePanelComboItem);

            if (null != _Cbo)
            {
                senseListCtrlAlert.BeginUpdate();

                if (reset)
                {
                    _Cbo.Items.Clear();
                }

                _Cbo.Tag = _DataSet.Tables[0].DefaultView;
                foreach (DataRowView _Row in (_Cbo.Tag as DataView))
                {
                    SensePanelComboItem.Item _Item = new SensePanelComboItem.Item
                    {
                        Tag = _Row,
                        Text = _Row["Name"].ToString(),
                        Value = _Row[_Value].ToString()
                    };
                    _Cbo.Items.Add(_Item);
                }

                if (_Cbo.Items.Count > 0)
                    _Cbo.SelectedIndex = 0;

                senseListCtrlAlert.EndUpdate();
            }
        }

        public Station GetStation()
        {
            Station _Station = null;
            DataRow _DataRow = null;
            SensePanelComboItem _Cbo = null;

            try
            {
                _Cbo = (senseListCtrlAlert[(UseLastStation ? "Routes" : "Stations")] as SensePanelComboItem);
            }
            catch
            {
            }

            if (null != _Cbo)
            {
                SensePanelComboItem.Item _Item = (_Cbo.Items[_Cbo.SelectedIndex] as SensePanelComboItem.Item);
                
                if (null != _Item)
                {
                    string _ID = _Item.Value.ToString();

                    _DataRow = (UseLastStation ? m_Storage.GetLastStationForRoute(_ID) : m_Storage.GetStationByID(_ID));

                    if (null != _DataRow)
                    {
                        _Station = new Station { 
                            Name = _DataRow["Name"].ToString(), 
                            Latitude = Convert.ToDouble(_DataRow["Lat"].ToString().Replace('.', ',')), 
                            Longitude = Convert.ToDouble(_DataRow["Lon"].ToString().Replace('.', ',')) 
                        };
                    }
                }
            }

            return _Station;
        }

        public Station[] GetStations()
        {
            DataView _DataView = m_StationsDataSet.Tables[0].DefaultView;
            _DataView.Sort = "Order";

            Station[] result = new Station[_DataView.Count];
            int index = 0;

            foreach (DataRowView _Row in _DataView)
            {
                result[index] = new Station
                {
                    StationID = int.Parse(_Row["StationID"].ToString()),
                    StationIndex = int.Parse(_Row["Order"].ToString()),
                    Name = _Row["Name"].ToString(),
                    Latitude = Convert.ToDouble(_Row["Lat"].ToString().Replace('.', ',')),
                    Longitude = Convert.ToDouble(_Row["Lon"].ToString().Replace('.', ','))
                };
                index++;
            }

            if (UseLastStation)
            {
                result[result.Length - 1].PerformAlert = true;
            }
            else
            {
                SensePanelComboItem _Cbo = (senseListCtrlAlert["Stations"] as SensePanelComboItem);
                SensePanelComboItem.Item _Item = (_Cbo.Items[_Cbo.SelectedIndex] as SensePanelComboItem.Item);
                string _StationID = _Item.Value.ToString();

                //DataRowView _DataRowView = (cboStations.SelectedItem as DataRowView);
                //string _StationID = _DataRowView["StationID"].ToString();

                var StationSelected = from _Station in result
                                      where _Station.StationID.ToString() == _StationID
                                      select _Station;

                if (StationSelected.SingleOrDefault() != null) 
                {
                    StationSelected.SingleOrDefault().PerformAlert = true;
                }
            }

            _DataView.Sort = "StationID";

            return result;
        }

        public Contact GetContact()
        {
            return (cboPhoneNr.SelectedItem as Contact);
        }

        private void UpdateSettings()
        {
            Settings.SetProperty("SendSMS", chkSendSMS.Checked.ToString());
        }

        private void chkSendSMS_Click(object sender, System.EventArgs e)
        {
            UpdateSettings();
        }
    }
}
