using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TrainStationAdvisor.ClassLibrary
{
    [Flags]
    public enum NotificationAction
    {
        Dialog = 4,
        Led = 1,
        Private = 0x20,
        Repeat = 0x10,
        Sound = 8,
        Vibrate = 2
    }

    public enum NotificationEvent
    {
        None,
        TimeChange,
        SyncEnd,
        OnACPower,
        OffACPower,
        NetConnect,
        NetDisconnect,
        DeviceChange,
        IRDiscovered,
        RS232Detected,
        RestoreEnd,
        Wakeup,
        TimeZoneChange,
        MachineNameChange,
        RndisFNDetected,
        InternetProxyChange
    }

    public enum NotificationType
    {
        ClassicTime = 4,
        Event = 1,
        Period = 3,
        Time = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
        public short wYear;
        public short wMonth;
        public short wDayOfWeek;
        public short wDay;
        public short wHour;
        public short wMinute;
        public short wSecond;
        public short wMillisecond;

        public static SYSTEMTIME FromDateTime(DateTime dt)
        {
            return new SYSTEMTIME { wYear = (short)dt.Year, wMonth = (short)dt.Month, wDayOfWeek = (short)dt.DayOfWeek, wDay = (short)dt.Day, wHour = (short)dt.Hour, wMinute = (short)dt.Minute, wSecond = (short)dt.Second, wMillisecond = (short)dt.Millisecond };
        }

        public DateTime ToDateTime()
        {
            if ((((this.wYear == 0) && (this.wMonth == 0)) && ((this.wDay == 0) && (this.wHour == 0))) && ((this.wMinute == 0) && (this.wSecond == 0)))
            {
                return DateTime.MinValue;
            }
            return new DateTime(this.wYear, this.wMonth, this.wDay, this.wHour, this.wMinute, this.wSecond, this.wMillisecond);
        }
    }

    public enum NotificationStatus
    {
        Inactive,
        Signalled
    }


    [StructLayout(LayoutKind.Sequential)]
    public class UserNotificationTrigger
    {
        internal int dwSize = 0x34;
        private int dwType;
        private int dwEvent;
        [MarshalAs(UnmanagedType.LPWStr)]
        private string lpszApplication = "";
        [MarshalAs(UnmanagedType.LPWStr)]
        private string lpszArguments;
        internal SYSTEMTIME stStartTime = new SYSTEMTIME();
        internal SYSTEMTIME stEndTime = new SYSTEMTIME();
        public NotificationType Type
        {
            get
            {
                return (NotificationType)this.dwType;
            }
            set
            {
                this.dwType = (int)value;
            }
        }
        public NotificationEvent Event
        {
            get
            {
                return (NotificationEvent)this.dwEvent;
            }
            set
            {
                this.dwEvent = (int)value;
            }
        }
        public string Application
        {
            get
            {
                return this.lpszApplication;
            }
            set
            {
                this.lpszApplication = value;
            }
        }
        public string Arguments
        {
            get
            {
                return this.lpszArguments;
            }
            set
            {
                this.lpszArguments = value;
            }
        }
        public DateTime StartTime
        {
            get
            {
                return this.stStartTime.ToDateTime();
            }
            set
            {
                this.stStartTime = SYSTEMTIME.FromDateTime(value);
            }
        }
        public DateTime EndTime
        {
            get
            {
                return this.stEndTime.ToDateTime();
            }
            set
            {
                this.stEndTime = SYSTEMTIME.FromDateTime(value);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class UserNotification
    {
        private int ActionFlags;
        [MarshalAs(UnmanagedType.LPWStr)]
        private string pwszDialogTitle;
        [MarshalAs(UnmanagedType.LPWStr)]
        private string pwszDialogText;
        [MarshalAs(UnmanagedType.LPWStr)]
        private string pwszSound;
        private int nMaxSound;
        private int dwReserved;
        public NotificationAction Action
        {
            get
            {
                return (NotificationAction)this.ActionFlags;
            }
            set
            {
                this.ActionFlags = (int)value;
            }
        }
        public string Title
        {
            get
            {
                return this.pwszDialogTitle;
            }
            set
            {
                this.pwszDialogTitle = value;
            }
        }
        public string Text
        {
            get
            {
                return this.pwszDialogText;
            }
            set
            {
                this.pwszDialogText = value;
            }
        }
        public string Sound
        {
            get
            {
                return this.pwszSound;
            }
            set
            {
                this.pwszSound = value;
            }
        }
        internal int MaxSound
        {
            get
            {
                return this.nMaxSound;
            }
            set
            {
                this.nMaxSound = value;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class UserNotificationInfoHeader
    {
        public int hNotification;
        public int dwStatus;
        public UserNotificationTrigger pcent;
        public UserNotification pceun;
        public int Handle
        {
            get
            {
                return this.hNotification;
            }
            set
            {
                this.hNotification = value;
            }
        }
        public NotificationStatus Status
        {
            get
            {
                return (NotificationStatus)this.dwStatus;
            }
            set
            {
                this.dwStatus = (int)value;
            }
        }
        public UserNotificationTrigger UserNotificationTrigger
        {
            get
            {
                return this.pcent;
            }
            set
            {
                this.pcent = value;
            }
        }
        public UserNotification UserNotification
        {
            get
            {
                return this.pceun;
            }
            set
            {
                this.pceun = value;
            }
        }
    }

    public static class Notify
    {
        // Methods
        public static bool ClearUserNotification(int handle)
        {
            return NativeMethods.CeClearUserNotification(handle);
        }

        public static UserNotificationInfoHeader GetUserNotification(int handle)
        {
            int pcBytesNeeded = 0;
            NativeMethods.CeGetUserNotification(handle, 0, ref pcBytesNeeded, IntPtr.Zero);
            IntPtr pBuffer = Marshal.AllocHGlobal(pcBytesNeeded);
            if (!NativeMethods.CeGetUserNotification(handle, (uint)pcBytesNeeded, ref pcBytesNeeded, pBuffer))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error getting UserNotification");
            }
            UserNotificationInfoHeader header = new UserNotificationInfoHeader
            {
                hNotification = Marshal.ReadInt32(pBuffer, 0),
                dwStatus = Marshal.ReadInt32(pBuffer, 4)
            };
            IntPtr ptr = (IntPtr)Marshal.ReadInt32(pBuffer, 8);
            IntPtr ptr3 = (IntPtr)Marshal.ReadInt32(pBuffer, 12);
            header.pcent = new UserNotificationTrigger();
            if (ptr != IntPtr.Zero)
            {
                header.pcent.dwSize = Marshal.ReadInt32(ptr);
                header.pcent.Type = (NotificationType)Marshal.ReadInt32(ptr, 4);
                header.pcent.Event = (NotificationEvent)Marshal.ReadInt32(ptr, 8);
                header.pcent.Application = Marshal.PtrToStringUni((IntPtr)Marshal.ReadInt32(ptr, 12));
                header.pcent.Arguments = Marshal.PtrToStringUni((IntPtr)Marshal.ReadInt32(ptr, 0x10));
                header.pcent.stStartTime = (SYSTEMTIME)Marshal.PtrToStructure((IntPtr)(ptr.ToInt32() + 20), typeof(SYSTEMTIME));
                header.pcent.stEndTime = (SYSTEMTIME)Marshal.PtrToStructure((IntPtr)(ptr.ToInt32() + 0x24), typeof(SYSTEMTIME));
            }
            header.pceun = new UserNotification();
            if (ptr3 != IntPtr.Zero)
            {
                header.pceun.Action = (NotificationAction)Marshal.ReadInt32(ptr3, 0);
                header.pceun.Title = Marshal.PtrToStringUni((IntPtr)Marshal.ReadInt32(ptr3, 4));
                header.pceun.Text = Marshal.PtrToStringUni((IntPtr)Marshal.ReadInt32(ptr3, 8));
                header.pceun.Sound = Marshal.PtrToStringUni((IntPtr)Marshal.ReadInt32(ptr3, 12));
                header.pceun.MaxSound = Marshal.ReadInt32(ptr3, 0x10);
            }
            Marshal.FreeHGlobal(pBuffer);
            return header;
        }

        public static int[] GetUserNotificationHandles()
        {
            int pcHandlesNeeded = 0;
            if (!NativeMethods.CeGetUserNotificationHandles(null, 0, ref pcHandlesNeeded))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error retrieving handles");
            }
            int[] rghNotifications = new int[pcHandlesNeeded];
            if (!NativeMethods.CeGetUserNotificationHandles(rghNotifications, pcHandlesNeeded, ref pcHandlesNeeded))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error retrieving handles");
            }
            return rghNotifications;
        }

        public static UserNotification GetUserNotificationPreferences(IntPtr hWnd)
        {
            UserNotification template = new UserNotification();
            return GetUserNotificationPreferences(hWnd, template);
        }

        public static UserNotification GetUserNotificationPreferences(IntPtr hWnd, UserNotification template)
        {
            template.MaxSound = 260;
            template.Sound = new string('\0', 260);
            if (!NativeMethods.CeGetUserNotificationPreferences(hWnd, template))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not get user preferences");
            }
            return template;
        }

        public static void HandleAppNotifications(string application)
        {
            if (!NativeMethods.CeHandleAppNotifications(application))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error clearing Application Notifications");
            }
        }

        public static void RunAppAtEvent(string appName, NotificationEvent whichEvent)
        {
            if (!NativeMethods.CeRunAppAtEvent(appName, (int)whichEvent))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot Set Notification Handler");
            }
        }

        public static void RunAppAtTime(string appName, DateTime time)
        {
            SYSTEMTIME lpTime = new SYSTEMTIME();
            if (time != DateTime.MinValue)
            {
                lpTime = new SYSTEMTIME();
                lpTime = SYSTEMTIME.FromDateTime(time);
                if (!NativeMethods.CeRunAppAtTime(appName, ref lpTime))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot Set Notification Handler");
                }
            }
            else if (!NativeMethods.CeRunAppAtTimeCancel(appName, null))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot Cancel Notification Handler");
            }
        }

        public static void SetNamedEventAtTime(string eventName, DateTime eventTime)
        {
            RunAppAtTime(string.Format(@"\\.\Notifications\NamedEvents\{0}", eventName), eventTime);
        }

        public static void SetNamedEventAtTime(string eventName, TimeSpan timeFromNow)
        {
            RunAppAtTime(string.Format(@"\\.\Notifications\NamedEvents\{0}", eventName), DateTime.Now.Add(timeFromNow));
        }

        public static int SetUserNotification(UserNotificationTrigger trigger, UserNotification notification)
        {
            int num = NativeMethods.CeSetUserNotificationEx(0, trigger, notification);
            if (num == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error setting UserNotification");
            }
            return num;
        }

        public static int SetUserNotification(int handle, UserNotificationTrigger trigger, UserNotification notification)
        {
            int num = NativeMethods.CeSetUserNotificationEx(handle, trigger, notification);
            if (num == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error setting UserNotification");
            }
            return num;
        }

        public static int SetUserNotification(string application, DateTime time, UserNotification notify)
        {
            return SetUserNotification(0, application, time, notify);
        }

        public static int SetUserNotification(int handle, string application, DateTime time, UserNotification notify)
        {
            SYSTEMTIME lpTime = SYSTEMTIME.FromDateTime(time);
            int num = NativeMethods.CeSetUserNotification(handle, application, ref lpTime, notify);
            if (num == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error setting UserNotification");
            }
            return num;
        }
    }

    public class Led
    {
        // Fields
        private int m_count;
        private const int NLED_COUNT_INFO_ID = 0;
        private const int NLED_SETTINGS_INFO_ID = 2;
        private const int NLED_SUPPORTS_INFO_ID = 1;

        // Methods
        public Led()
        {
            NLED_COUNT_INFO pOutput = new NLED_COUNT_INFO();
            if (!NativeMethods.NLedGetDeviceCount(0, ref pOutput))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error Initialising LED's");
            }
            this.m_count = (int)pOutput.cLeds;
        }

        public void SetLedStatus(int led, LedState newState)
        {
            NLED_SETTINGS_INFO pOutput = new NLED_SETTINGS_INFO
            {
                LedNum = led,
                OffOnBlink = (int)newState
            };
            NativeMethods.NLedSetDevice(2, ref pOutput);
        }

        // Properties
        public int Count
        {
            get
            {
                return this.m_count;
            }
        }

        // Nested Types
        public enum LedState
        {
            Off,
            On,
            Blink
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NLED_COUNT_INFO
        {
            public uint cLeds;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NLED_SETTINGS_INFO
        {
            public int LedNum;
            public int OffOnBlink;
            public int TotalCycleTime;
            public int OnTime;
            public int OffTime;
            public int MetaCycleOn;
            public int MetaCycleOff;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NLED_SUPPORTS_INFO
        {
            public uint LedNum;
            public int lCycleAdjust;
            public bool fAdjustTotalCycleTime;
            public bool fAdjustOnTime;
            public bool fAdjustOffTime;
            public bool fMetaCycleOn;
            public bool fMetaCycleOff;
        }
    }
}
