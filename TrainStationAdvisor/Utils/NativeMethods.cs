using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using TrainStationAdvisor.ClassLibrary;

namespace TrainStationAdvisor
{
    internal static class NativeMethods
    {
        // Fields
        internal const int DBGDRIVERSTAT = 0x1802;
        internal const int GETPOWERMANAGEMENT = 0x1804;
        internal const int GETVFRAMELEN = 0x1801;
        internal const int GETVFRAMEPHYSICAL = 0x1800;
        internal const int QUERYESCSUPPORT = 8;
        internal const int SETPOWERMANAGEMENT = 0x1803;

        //internal const uint INFINITE = 4294967295;

        // Methods
        [DllImport("aygshell.dll", SetLastError = true)]
        internal static extern int VibrateGetDeviceCaps(Vibrate.VibrationCapabilities caps);
        [DllImport("aygshell.dll", EntryPoint = "Vibrate", SetLastError = true)]
        internal static extern int VibratePlay(int cvn, IntPtr rgvn, uint fRepeat, uint dwTimeout);
        [DllImport("aygshell.dll", SetLastError = true)]
        internal static extern int VibrateStop();
        [DllImport("aygshell.dll", SetLastError = true)]
        internal static extern int SndPlaySync(string path, uint flags);

        [DllImport("CoreDLL")]
        public static extern int ReleasePowerRequirement(IntPtr hPowerReq);

        [DllImport("CoreDLL", SetLastError = true)]
        public static extern IntPtr SetPowerRequirement
        (
            string pDevice,
            CEDEVICE_POWER_STATE DeviceState,
            DevicePowerFlags DeviceFlags,
            IntPtr pSystemState,
            uint StateFlagsZero
        );

        [DllImport("CoreDLL", SetLastError = true)]
        public static extern IntPtr SetDevicePower
            (
                string pDevice,
                DevicePowerFlags DeviceFlags,
                CEDEVICE_POWER_STATE DevicePowerState
            );

        [DllImport("CoreDLL")]
        public static extern int GetDevicePower(string device, DevicePowerFlags flags, out CEDEVICE_POWER_STATE PowerState);

        [DllImport("CoreDLL")]
        public static extern int SetSystemPowerState(String stateName, PowerState powerState, DevicePowerFlags flags);


        [DllImport("CoreDLL")]
        public static extern int PowerPolicyNotify(
          PPNMessage dwMessage,
            int option
            //    DevicePowerFlags);
        );

        [DllImport("CoreDLL")]
        public static extern int GetSystemPowerStatusEx2(
             SYSTEM_POWER_STATUS_EX2 statusInfo,
            int length,
            int getLatest
                );


        public static SYSTEM_POWER_STATUS_EX2 GetSystemPowerStatus()
        {
            SYSTEM_POWER_STATUS_EX2 retVal = new SYSTEM_POWER_STATUS_EX2();
            int result = GetSystemPowerStatusEx2(retVal, Marshal.SizeOf(retVal), 1);
            return retVal;
        }
        // System\CurrentControlSet\Control\Power\Timeouts
        [DllImport("CoreDLL")]
        public static extern int SystemParametersInfo
        (
            SPI Action,
            uint Param,
            ref int result,
            int updateIni
        );

        [DllImport("CoreDLL")]
        public static extern int SystemIdleTimerReset();

        //[DllImport("CoreDLL")]
        //public static extern int CeRunAppAtTime(string application, SystemTime startTime);
        //[DllImport("CoreDLL")]
        //public static extern int CeRunAppAtEvent(string application, int EventID);

        [DllImport("CoreDLL")]
        public static extern int FileTimeToSystemTime(ref long lpFileTime, SystemTime lpSystemTime);
        [DllImport("CoreDLL")]
        public static extern int FileTimeToLocalFileTime(ref long lpFileTime, ref long lpLocalFileTime);

        // For named events
        //[DllImport("CoreDLL", SetLastError = true)]
        //internal static extern bool EventModify(IntPtr hEvent, EVENT ef);

        //[DllImport("CoreDLL", SetLastError = true)]
        //internal static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        //[DllImport("CoreDLL", SetLastError = true)]
        //internal static extern bool CloseHandle(IntPtr hObject);

        //[DllImport("CoreDLL", SetLastError = true)]
        //internal static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        //[DllImport("CoreDLL", SetLastError = true)]
        //internal static extern int WaitForMultipleObjects(int nCount, IntPtr[] lpHandles, bool fWaitAll, int dwMilliseconds);

        public enum PPNMessage
        {
            PPN_REEVALUATESTATE = 1,
            PPN_POWERCHANGE = 2,
            PPN_UNATTENDEDMODE = 3,
            PPN_SUSPENDKEYPRESSED = 4,
            PPN_POWERBUTTONPRESSED = 4,
            PPN_SUSPENDKEYRELEASED = 5,
            PPN_APPBUTTONPRESSED = 6,

        }

        [StructLayout(LayoutKind.Sequential)]
        public class SYSTEM_POWER_STATUS_EX2
        {
            //AC power status. 
            public ACLineStatus ACLineStatus;
            //Battery charge status
            public BatteryFlag BatteryFlag;
            // Percentage of full battery charge remaining. Must be in 
            // the range 0 to 100, or BATTERY_PERCENTAGE_UNKNOWN if 
            // percentage of battery life remaining is unknown
            public byte BatteryLifePercent;
            byte Reserved1;
            //Percentage of full battery charge remaining. Must be 
            // in the range 0 to 100, or BATTERY_PERCENTAGE_UNKNOWN 
            // if percentage of battery life remaining is unknown. 
            public int BatteryLifeTime;
            // Number of seconds of battery life when at full charge, 
            // or BATTERY_LIFE_UNKNOWN if full lifetime of battery is unknown
            public int BatteryFullLifeTime;
            byte Reserved2;
            // Backup battery charge status.
            public BatteryFlag BackupBatteryFlag;
            // Percentage of full backup battery charge remaining. Must be in 
            // the range 0 to 100, or BATTERY_PERCENTAGE_UNKNOWN if percentage 
            // of backup battery life remaining is unknown. 

            public byte BackupBatteryLifePercent;
            byte Reserved3;
            // Number of seconds of backup battery life when at full charge, or 
            // BATTERY_LIFE_UNKNOWN if number of seconds of backup battery life 
            // remaining is unknown. 
            public int BackupBatteryLifeTime;
            // Number of seconds of backup battery life when at full charge, or 
            // BATTERY_LIFE_UNKNOWN if full lifetime of backup battery is unknown
            public int BackupBatteryFullLifeTime;
            // Number of millivolts (mV) of battery voltage. It can range from 0 
            // to 65535
            public int BatteryVoltage;
            // Number of milliamps (mA) of instantaneous current drain. It can 
            // range from 0 to 32767 for charge and 0 to –32768 for discharge. 
            public int BatteryCurrent;
            //Number of milliseconds (mS) that is the time constant interval 
            // used in reporting BatteryAverageCurrent. 
            public int BatteryAverageCurrent;
            // Number of milliseconds (mS) that is the time constant interval 
            // used in reporting BatteryAverageCurrent. 

            public int BatteryAverageInterval;
            // Average number of milliamp hours (mAh) of long-term cumulative 
            // average discharge. It can range from 0 to –32768. This value is 
            // reset when the batteries are charged or changed. 

            public int BatterymAHourConsumed;
            // Battery temperature reported in 0.1 degree Celsius increments. It 
            // can range from –3276.8 to 3276.7. 
            public int BatteryTemperature;
            // Number of millivolts (mV) of backup battery voltage. It can range 
            // from 0 to 65535.
            public int BackupBatteryVoltage;
            // Type of battery.
            public BatteryChemistry BatteryChemistry;
            //  Add any extra information after the BatteryChemistry member.
        }

        public enum PowerState
        {
            POWER_STATE_ON = (0x00010000),          // on state
            POWER_STATE_OFF = (0x00020000),         // no power, full off
            POWER_STATE_CRITICAL = (0x00040000),    // critical off
            POWER_STATE_BOOT = (0x00080000),        // boot state
            POWER_STATE_IDLE = (0x00100000),        // idle state
            POWER_STATE_SUSPEND = (0x00200000),     // suspend state
            POWER_STATE_UNATTENDED = (0x00400000),  // Unattended state.
            POWER_STATE_RESET = (0x00800000),       // reset state
            POWER_STATE_USERIDLE = (0x01000000),    // user idle state
            POWER_STATE_PASSWORD = (0x10000000)     // This state is password protected.
        };

        public enum CEDEVICE_POWER_STATE : int
        {
            PwrDeviceUnspecified = -1,
            //Full On: full power,  full functionality
            D0 = 0,
            /// <summary>
            /// Low Power On: fully functional at low power/performance
            /// </summary>
            D1 = 1,
            /// <summary>
            /// Standby: partially powered with automatic wake
            /// </summary>
            D2 = 2,
            /// <summary>
            /// Sleep: partially powered with device initiated wake
            /// </summary>
            D3 = 3,
            /// <summary>
            /// Off: unpowered
            /// </summary>
            D4 = 4,
            PwrDeviceMaximum
        }

        [Flags()]
        public enum DevicePowerFlags
        {
            None = 0,
            /// <summary>
            /// Specifies the name of the device whose power should be maintained at or above the DeviceState level.
            /// </summary>
            POWER_NAME = 0x00000001,
            /// <summary>
            /// Indicates that the requirement should be enforced even during a system suspend.
            /// </summary>
            POWER_FORCE = 0x00001000,
            POWER_DUMPDW = 0x00002000
        }

        // Nested Types
        internal enum VideoPowerState
        {
            VideoPowerOn = 1,
            VideoPowerStandBy,
            VideoPowerSuspend,
            VideoPowerOff
        }

        public enum SPI : uint
        {
            /*
            SPI_GETMOUSE = 3,
            SPI_SETMOUSE = 4,
            SPI_SETDESKWALLPAPER = 20,
            SPI_SETDESKPATTERN = 21,

            SPI_SETWORKAREA = 47,
            SPI_GETWORKAREA = 48,
            SPI_GETSHOWSOUNDS = 56,
            SPI_SETSHOWSOUNDS = 57,

            SPI_GETDEFAULTINPUTLANG = 89,

            SPI_SETLANGTOGGLE = 91,

            SPI_GETWHEELSCROLLLINES = 104,
            SPI_SETWHEELSCROLLLINES = 105,

            SPI_GETFONTSMOOTHINGCONTRAST = 0x200C,
            SPI_SETFONTSMOOTHINGCONTRAST = 0x200D,

            SPI_GETFONTSMOOTHING = 0x004A,
            SPI_SETFONTSMOOTHING = 0x004B,
            SPI_GETSCREENSAVETIMEOUT = 14,
            SPI_SETSCREENSAVETIMEOUT = 15,
            */
            SPI_SETBATTERYIDLETIMEOUT = 251,
            SPI_GETBATTERYIDLETIMEOUT = 252,

            SPI_SETEXTERNALIDLETIMEOUT = 253,
            SPI_GETEXTERNALIDLETIMEOUT = 254,

            SPI_SETWAKEUPIDLETIMEOUT = 255,
            SPI_GETWAKEUPIDLETIMEOUT = 256
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        // http://msdn.microsoft.com/en-us/library/aa926903.aspx
        public enum ACLineStatus : byte
        {
            AC_LINE_OFFLINE = 0, // Offline
            AC_LINE_ONLINE = 1, // Online
            AC_LINE_BACKUP_POWER = 2, // Backup Power
            AC_LINE_UNKNOWN = 0xFF, //
            Unknown = 0xFF, //status
        }

        public enum BatteryFlag : byte
        {
            BATTERY_FLAG_HIGH = 0x01,
            BATTERY_FLAG_CRITICAL = 0x04,
            BATTERY_FLAG_CHARGING = 0x08,
            BATTERY_FLAG_NO_BATTERY = 0x80,
            BATTERY_FLAG_UNKNOWN = 0xFF,
            BATTERY_FLAG_LOW = 0x02
        }

        public enum BatteryChemistry : byte
        {
            BATTERY_CHEMISTRY_ALKALINE = 0x01,  // Alkaline battery.
            BATTERY_CHEMISTRY_NICD = 0x02, // Nickel Cadmium battery.
            BATTERY_CHEMISTRY_NIMH = 0x03, // Nickel Metal Hydride battery.
            BATTERY_CHEMISTRY_LION = 0x04, // Lithium Ion battery.
            BATTERY_CHEMISTRY_LIPOLY = 0x05, // Lithium Polymer battery.
            BATTERY_CHEMISTRY_ZINCAIR = 0x06, // Zinc Air battery.
            BATTERY_CHEMISTRY_UNKNOWN = 0xFF // Battery chemistry is unknown.
        }

        // Fields
        public const int ERROR_ALREADY_EXISTS = 0xb7;
        public const int EVENT_ALL_ACCESS = 3;
        public const int INFINITE = -1;
        public const int WAIT_FAILED = -1;
        public const int WAIT_TIMEOUT = 0x102;

        // Methods
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern int CeGetThreadPriority(IntPtr hThread);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern int CeGetThreadQuantum(IntPtr hThread);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool CeSetThreadPriority(IntPtr hThread, int nPriority);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool CeSetThreadPriority(uint hThread, int nPriority);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool CeSetThreadQuantum(IntPtr hThread, int dwTime);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool CeSetThreadQuantum(uint hThread, int dwTime);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool InitialOwner, string MutexName);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern IntPtr CreateSemaphore(IntPtr lpSemaphoreAttributes, int lInitialCount, int lMaximumCount, string lpName);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern void DeleteCriticalSection(IntPtr lpCriticalSection);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern void EnterCriticalSection(IntPtr lpCriticalSection);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool EventModify(IntPtr hEvent, EVENT ef);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern void InitializeCriticalSection(IntPtr lpCriticalSection);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern void LeaveCriticalSection(IntPtr lpCriticalSection);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern int MsgWaitForMultipleObjectsEx(uint nCount, IntPtr[] lpHandles, uint dwMilliseconds, uint dwWakeMask, uint dwFlags);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern IntPtr OpenEvent(int dwDesiredAccess, bool bInheritHandle, string lpName);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern IntPtr OpenSemaphore(int desiredAccess, bool inheritHandle, string name);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool ReleaseMutex(IntPtr hMutex);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool ReleaseSemaphore(IntPtr handle, int lReleaseCount, out int previousCount);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern uint ResumeThread(IntPtr hThread);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern uint ResumeThread(uint hThread);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern uint SuspendThread(IntPtr hThread);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern uint SuspendThread(uint hThread);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool TerminateThread(IntPtr hThread, int dwExitCode);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool TerminateThread(uint hThread, int dwExitCode);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern int WaitForMultipleObjects(int nCount, IntPtr[] lpHandles, bool fWaitAll, int dwMilliseconds);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        // Methods
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern bool CeClearUserNotification(int hNotification);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern bool CeGetUserNotification(int hNotification, uint cBufferSize, ref int pcBytesNeeded, IntPtr pBuffer);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern bool CeGetUserNotificationHandles(int[] rghNotifications, int cHandles, ref int pcHandlesNeeded);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern bool CeGetUserNotificationPreferences(IntPtr hWndParent, UserNotification lpNotification);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern bool CeHandleAppNotifications(string appName);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern bool CeRunAppAtEvent(string pwszAppName, int lWhichEvent);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern bool CeRunAppAtTime(string pwszAppName, ref SYSTEMTIME lpTime);
        [DllImport("coredll.dll", EntryPoint = "CeRunAppAtTime", SetLastError = true)]
        internal static extern bool CeRunAppAtTimeCancel(string pwszAppName, byte[] lpTime);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern int CeSetUserNotification(int hNotification, string pwszAppName, ref SYSTEMTIME lpTime, UserNotification lpUserNotification);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern int CeSetUserNotificationEx(int hNotification, UserNotificationTrigger lpTrigger, UserNotification lpUserNotification);
        [DllImport("coredll.dll", EntryPoint = "NLedGetDeviceInfo", SetLastError = true)]
        internal static extern bool NLedGetDeviceCount(short nID, ref Led.NLED_COUNT_INFO pOutput);
        [DllImport("coredll.dll", EntryPoint = "NLedGetDeviceInfo", SetLastError = true)]
        internal static extern bool NLedGetDeviceSupports(short nID, ref Led.NLED_SUPPORTS_INFO pOutput);
        [DllImport("coredll.dll", SetLastError = true)]
        internal static extern bool NLedSetDevice(short nID, ref Led.NLED_SETTINGS_INFO pOutput);

        // Nested Types
        public enum EVENT
        {
            PULSE = 1,
            RESET = 2,
            SET = 3
        }

        [DllImport("coredll.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("coredll")]
        extern static int ExtEscape(IntPtr hdc, int nEscape, int cbInput, ref VideoPowerManagement vpm, int zero, IntPtr empty);

        [DllImport("coredll")]
        extern static int ExtEscape(IntPtr hdc, int nEscape, int zero, IntPtr empty, int cbOutput, ref VideoPowerManagement outData);

        struct VideoPowerManagement
        {
            public int Length;
            public int DPMSVersion;
            public VideoPowerState PowerState;
        }

        public static VideoPowerState GetVideoPowerState()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            VideoPowerManagement ret = new VideoPowerManagement();
            ExtEscape(hdc, GETPOWERMANAGEMENT, 0, IntPtr.Zero, 12, ref ret);
            return ret.PowerState;
        }
        
        public static void SetVideoPowerState(VideoPowerState value)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);

            VideoPowerManagement vpm = new VideoPowerManagement { 
                Length = 12, 
                DPMSVersion = 1, 
                PowerState = value 
            };

            ExtEscape(hdc, SETPOWERMANAGEMENT, vpm.Length, ref vpm, 0, IntPtr.Zero);
        }

        [DllImport("coredll.dll", EntryPoint = "SystemParametersInfoW", CharSet = CharSet.Unicode)]
        internal static extern int SystemParametersInfo4Strings(uint uiAction, uint uiParam, StringBuilder pvParam, uint fWinIni);

        private const string MicrosoftEmulatorOemValue = "Microsoft DeviceEmulator";

        public enum SystemParametersInfoActions : uint
        {
            SPI_GETPLATFORMTYPE = 257, // this is used elsewhere for Smartphone/PocketPC detection
            SPI_GETOEMINFO = 258,
        }

        private static string GetOemInfo()
        {
            StringBuilder oemInfo = new StringBuilder(50);
            if (SystemParametersInfo4Strings((uint)SystemParametersInfoActions.SPI_GETOEMINFO,
                                             (uint)oemInfo.Capacity, oemInfo, 0) == 0)
            {
                throw new Exception("Error getting OEM info.");
            }
            return oemInfo.ToString();
        }

        internal static bool IsEmulator()
        {
            return GetOemInfo() == MicrosoftEmulatorOemValue;
        }

    }
}
