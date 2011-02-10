using System;
using System.Threading;

namespace TrainStationAdvisor.ClassLibrary
{
    public class LargeIntervalTimerLeo : IDisposable
    {
        // Fields
        private bool? m_cachedEnabled = null;
        private bool m_disposing;
        private bool m_enabled;
        private DateTime m_firstTime;
        private object m_interlock;
        private TimeSpan m_interval;
        private bool m_oneShot;
        private EventWaitHandle m_quitHandle;
        private bool m_useFirstTime;
        private int TestCondition = -1;
        private int ThreadCount;
        private int m_duetime;

        // Events
        public event EventHandler Tick;

        // Methods
        public void Dispose()
        {
            lock (m_interlock)
            {
                m_disposing = true;
                if (Enabled)
                {
                    Enabled = false;
                }
                if (m_quitHandle != null)
                {
                    m_quitHandle.Set();
                    m_quitHandle.Close();
                    m_quitHandle = null;
                }
            }
        }

        public LargeIntervalTimerLeo()
        {
            m_interlock = new object();
            m_firstTime = DateTime.MinValue;
            m_interval = new TimeSpan(0, 0, 60);
            m_quitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            TestCondition = -1;
        }

        ~LargeIntervalTimerLeo()
        {
            Dispose();
        }

        private void InternalThreadProc(object state)
        {
            WaitCallback callBack = null;
            ThreadCount++;
            string name = Guid.NewGuid().ToString();
            
            EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.AutoReset, name);

            //Thread.Sleep(1000);

            try
            {
                while (m_enabled)
                {
                    //Thread.Sleep(1000);

                    if (m_disposing)
                    {
                        return;
                    }

                    //Thread.Sleep(1000);
                    
                    if (m_useFirstTime)
                    {
                        Notify.RunAppAtTime(string.Format(@"\\.\Notifications\NamedEvents\{0}", name), m_firstTime);
                        m_useFirstTime = false;
                    }
                    else
                    {
                        Notify.RunAppAtTime(string.Format(@"\\.\Notifications\NamedEvents\{0}", name), DateTime.Now.Add(m_interval));
                        m_firstTime = DateTime.MinValue;
                    }

                    //Thread.Sleep(1000);                    

                    if (m_disposing)
                    {
                        return;
                    }

                    int num = EventWaitHandle.WaitAny(new WaitHandle[] { handle, m_quitHandle });

                    //Thread.Sleep(1000);

                    if (num == 0)
                    {
                        m_cachedEnabled = null;

                        if (Tick != null)
                        {
                            if (callBack == null)
                            {
                                callBack = delegate { Tick(this, null); };
                            }
                            ThreadPool.QueueUserWorkItem(callBack);
                        }

                        //Thread.Sleep(1000);

                        if (OneShot)
                        {
                            //Thread.Sleep(1000);

                            if (m_cachedEnabled.HasValue)
                            {
                                //Thread.Sleep(1000);

                                m_enabled = m_cachedEnabled == true;

                                //Thread.Sleep(1000);
                            }
                            else
                            {
                                m_enabled = false;
                            }
                        }
                    }
                    else
                    {
                        m_enabled = false;
                    }
                }
            }
            finally
            {
                handle.Close();
                ThreadCount--;
                if (ThreadCount == 0)
                {
                    m_quitHandle.Reset();
                }
            }
        }

        // Properties
        public bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                lock (m_interlock)
                {
                    m_cachedEnabled = new bool?(value);
                    if ((!m_enabled || !value) && (m_enabled || value))
                    {
                        m_enabled = value;
                        if (ThreadCount > 0)
                        {
                            m_quitHandle.Set();
                            Thread.Sleep(1);
                        }
                        if (m_enabled)
                        {
                            ThreadPool.QueueUserWorkItem(InternalThreadProc);
                        }
                    }
                }
            }
        }

        public DateTime FirstEventTime
        {
            get
            {
                return m_firstTime;
            }
            set
            {
                lock (m_interlock)
                {
                    if (value.CompareTo(DateTime.Now) <= 0)
                    {
                        m_firstTime = DateTime.MinValue;
                        m_useFirstTime = false;
                    }
                    m_firstTime = value;
                    m_useFirstTime = true;
                }
            }
        }

        public TimeSpan Interval
        {
            get
            {
                return m_interval;
            }
            set
            {
                lock (m_interlock)
                {
                    if (value.TotalSeconds < 15.0)
                    {
                        throw new ArgumentException("Interval cannot be less than 15 seconds");
                    }
                    m_interval = new TimeSpan(value.Days, value.Hours, value.Minutes, value.Seconds);
                }
            }
        }

        public bool OneShot
        {
            get
            {
                lock (m_interlock)
                {
                    return m_oneShot;
                }
            }
            set
            {
                lock (m_interlock)
                {
                    m_oneShot = value;
                }
            }
        }
    }
}
