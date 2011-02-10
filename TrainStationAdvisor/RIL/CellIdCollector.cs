using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Data;
using System.Diagnostics;
using OpenNETCF.WindowsCE;

namespace TrainStationAdvisor.ClassLibrary
{
    public class CellIdCollector : IDisposable
    {
        private const int DUE_TIME = 100;
        private const int PERIOD = 15000;
        private const int PERIOD_THREAD = 2000;

        #region Properties
        //private System.Threading.Timer m_Timer;    
        private Thread m_Thread;
        //private string m_CurrentCellid;
        private object m_Lock;
        private volatile bool m_StopThread;
        private bool m_Disposed;
        private bool m_AlwaysOn;
        private int m_DueTime;
        private int m_Period;
        private Dictionary<string, GeoLocation> m_CellCollection;
        private Interfaces.ICellStorage m_Storage;
        private LargeIntervalTimer m_Timer;

        private GeoLocation m_GeoLocation;
        private GeoLocation GeoLocation
        {
            get
            {
                return m_GeoLocation;
            }
            set
            {
                m_GeoLocation = value;
                OnCellIdFound();
            }
        }

        private const string FILE_NAME = @"LogCellIDs.txt";
        private StreamWriter m_StreamWriter;
        private StreamWriter StreamWriter
        {
            get
            {
                if (null == m_StreamWriter)
                {
                    string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

                    m_StreamWriter = new StreamWriter(string.Format("{0}\\{1}", filePath, FILE_NAME), true, Encoding.ASCII);
                }
                return m_StreamWriter;
            }
        }


        public bool Started { get; private set; }
        public bool WorkUnattended { get; set; }
        
        private Logger m_Logger;
        public Logger Logger
        {
            get
            {
                if (null == m_Logger)
                {
                    m_Logger = new Logger();
                }

                return m_Logger;
            }
            set
            {
                m_Logger = value;
            }
        }
        #endregion

        #region Events
        public event EventHandler<EventArgs.CellIdCollectorEventArgs> CellIdFound;
        
        /// <summary>
        /// Triggers the CellIdFound event.
        /// </summary>
        protected virtual void OnCellIdFound()
        {
            if (CellIdFound != null)
            {
                CellIdFound(this, new EventArgs.CellIdCollectorEventArgs(m_GeoLocation));
            }
        }
        #endregion

        #region Constructor/Destructor
        public CellIdCollector(Interfaces.ICellStorage storage, int dueTime, int period, bool alwaysOn)
        {
            //m_StopThread = false;
            //m_CurrentCellid = string.Empty;
            m_CellCollection = new Dictionary<string, GeoLocation>();
            m_Lock = new object();
            m_DueTime = dueTime;
            m_Period = period;
            m_Storage = storage;
            m_AlwaysOn = alwaysOn;

            if (m_AlwaysOn)
            {
                //System.Threading.TimerCallback tcb = CellCollector;
                //m_Timer = new System.Threading.Timer(tcb, new object(), System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                m_Thread = new Thread(CellCollectorThread);
                m_Thread.Name = "CellCollectorThread";

                //m_Thread.IsBackground = true;
            }
            else
            {
                m_Timer = new LargeIntervalTimer();
                m_Timer.OneShot = false;                                            //run forever
                m_Timer.FirstEventTime = DateTime.Now.AddMilliseconds(m_DueTime);   //start in x seconds
                m_Timer.Interval = TimeSpan.FromMilliseconds(m_Period);
                m_Timer.Tick += Timer_Tick;
            }
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            ProcessGeolocation();
        }

        //Default values : DueTime = 10, Period = 2000
        public CellIdCollector(Interfaces.ICellStorage storage)
            : this(storage, DUE_TIME, PERIOD, false)
        {
        }

        public CellIdCollector() : this(null, DUE_TIME, PERIOD, false)
        {
        }

        public CellIdCollector(Interfaces.ICellStorage storage, bool alwaysOn)
            : this(storage, DUE_TIME, PERIOD, alwaysOn)
        {
        }

        public CellIdCollector(bool alwaysOn)
            : this(null, DUE_TIME, PERIOD, alwaysOn)
        {
        }


        ~CellIdCollector()
        {
            Dispose(false);
        }
        #endregion

        #region Methods
        public void Start()
        {
            Started = true;
            if (m_AlwaysOn)
            {
                Thread.Sleep(m_DueTime);

                m_Thread.Start();
            }
            else
            {
                //m_Timer.Change(m_DueTime, m_Period);
                m_Timer.Enabled = true;
            }
        }

        public void Stop()
        {
            Started = false;
            if (m_AlwaysOn)
            {
                m_StopThread = true;
                //m_Timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
            else
            {
                m_Timer.Enabled = false;
            }
        }
        
        private void CellCollectorThread()
        {
            while (!m_StopThread && !m_Disposed)
            {
                ProcessGeolocation();

                Console.WriteLine(string.Format("CellCollectorThread() {0}:{1}", m_Disposed, m_StopThread));
                Logger.Log("CellCollectorThread() {0}:{1}", m_Disposed, m_StopThread);

                //Overrides Global period and the device should be always on and "powered"
                Thread.Sleep(PERIOD_THREAD);
            }

            Logger.Log("CellCollectorThread Finished - {0}:{1}", m_Disposed, m_StopThread);
        }

        private void ProcessGeolocation()
        {
            Logger.Log("ProcessGeolocation - Disposed {0}", m_Disposed);

            if (WorkUnattended)
            {
                NativeMethods.PowerPolicyNotify(NativeMethods.PPNMessage.PPN_UNATTENDEDMODE, -1);
                Thread.Sleep(100);
            }

            if (!m_Disposed)
            {
                lock (m_Lock)
                {
                    string cellid = RIL.GetCellTowerInfo();

                    Logger.Log("GetCellTowerInfo() - cellid {0}", cellid);

                    if (!cellid.Equals("Failed to initialize RIL"))
                    {
                        string txt = String.Format("{0}-{1}", cellid, DateTime.Now);

                        if (!m_CellCollection.ContainsKey(cellid))
                        {
                            GetLocation(cellid, txt);

                            Logger.Log("GetLocation - cellid, txt {0} found by service", cellid, txt);

                            if (GeoLocation.IsValid)
                            {
                                Logger.Log("GeoLocation - IsValid {0}", GeoLocation.IsValid);

                                m_CellCollection.Add(cellid, GeoLocation);
                            }
                        }
                        else
                        {
                            Logger.Log("GetLocation - cellid, txt {0} cached in memory", cellid, txt);

                            GeoLocation = m_CellCollection[cellid];
                        }

                        WriteToFile(cellid, txt);
                    }
                    else
                    {
                        GeoLocation = GeoLocation.Empty;
                    }
                }
            }

            Logger.Log("ProcessGeolocation Finished - Disposed {0}", m_Disposed);
        }


        private void GetLocation(string cellId, string txt)
        {
            GeoLocation geoLocation = null;

            if (null != m_Storage)
            {
                DataRow row = m_Storage.GetCellByID(cellId);
                if (null != row)
                {
                    geoLocation = new GeoLocation { 
                                Latitude = Convert.ToDouble(row["Lat"].ToString().Replace('.', ',')), 
                                Longitude = Convert.ToDouble(row["Lon"].ToString().Replace('.', ','))
                    };
                }
            }

            if (null == geoLocation)
            {
                // [0] - CID 
                // [1] - LAC
                // [2] – MCC
                // [3] - Time

                /*---Arguments for GetLatLng(MCC MNC LAC CID)
                string[] args = { 
                    cellidFields[2], // MCC 
                    "0",             // MNC – don’t need it here
                    cellidFields[1], // LAC
                    cellidFields[0]  // CID
                };
                */

                string[] cellidFields = txt.Split('-');

                CellTower cellTower = null;
                bool loadLocation = true;

                try
                {
                    cellTower = new CellTower
                    {
                        LocationAreaCode = int.Parse(cellidFields[1]),
                        MobileCountryCode = int.Parse(cellidFields[2]),
                        MobileNetworkCode = 0,
                        TowerId = int.Parse(cellidFields[0])
                    };
                }
                catch (Exception ex)
                {
                    loadLocation = false;
                }

                if (loadLocation)
                {
                    geoLocation = GoogleMapsCellService.GetLocation(cellTower);

                    if (null != m_Storage && geoLocation.IsValid)
                    {
                        m_Storage.InsertCellId(cellId, geoLocation.Latitude.ToString().Replace(',', '.'), geoLocation.Longitude.ToString().Replace(',', '.'));
                    }

                    Logger.Log("GetLocation - celltower id:{0} retreived from Service", cellTower.TowerId);
                }
                else
                {
                    geoLocation = GeoLocation.Empty;
                }

            }

            GeoLocation = geoLocation; 
        }

        private void WriteToFile(string cellid, string txt)
        {
            if (!m_CellCollection.ContainsKey(cellid))
            {
                string _txt = txt;

                if (null != GeoLocation)
                {
                    _txt = string.Format("{0};Lat:{1};Lon:{2}", txt, GeoLocation.Latitude, GeoLocation.Longitude);
                }

                //---write to file---           
                StreamWriter.WriteLine(_txt);
                StreamWriter.Flush();
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool ADisposing)
        {
            if (!m_Disposed)
            {
                // wenn true, alle managed und unmanaged resources mussen aufgelegt werden.
                if (ADisposing)
                {
                    //nix zu machen in moment
                }

                //Console.WriteLine("{0}: GPS-Thread beendet", DateTime.Now);

                lock (m_Lock)
                {
                    if (null != m_Thread)
                    {
                        m_StopThread = true;
                        m_Thread.Join();
                        
                        Logger.Log("Thread {0} Stopped", m_Thread.ManagedThreadId);

                        m_Thread = null;
                    }

                    if (null != m_Timer)                    
                    {
                        m_Timer.Enabled = false;
                        Thread.Sleep(1000);
                        m_Timer.Dispose();
                        m_Timer = null;

                        Logger.Log("Timer Disposed");
                    }
                    
                    m_CellCollection.Clear();
                    m_CellCollection = null;

                    Logger.Log("Cell Collection cleared");

                    if (null != m_StreamWriter)
                    {
                        m_StreamWriter.Flush();
                        m_StreamWriter.Close();
                        m_StreamWriter.Dispose();

                        Logger.Log("StreamWriter Closed and Disposed");
                    }

                    m_StreamWriter = null;
                }

                m_Lock = null;
            }

            m_Disposed = true;
        }
        #endregion

    }
}
