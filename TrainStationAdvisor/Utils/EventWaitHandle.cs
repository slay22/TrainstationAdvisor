using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace TrainStationAdvisor.ClassLibrary
{
    public class WaitHandleCannotBeOpenedException : ApplicationException
    {
        // Methods
        public WaitHandleCannotBeOpenedException()
            : base("No handle of the given name exists.")
        {
            base.HResult = -2146233044;
        }

        public WaitHandleCannotBeOpenedException(string message)
            : base(message)
        {
            base.HResult = -2146233044;
        }

        public WaitHandleCannotBeOpenedException(string message, Exception innerException)
            : base(message, innerException)
        {
            base.HResult = -2146233044;
        }
    }

    public class EventWaitHandle : WaitHandle
    {
        // Fields
        private const int ERROR_ALREADY_EXISTS = 0xb7;
        private const int EVENT_ALL_ACCESS = 3;
        private const int WAIT_FAILED = -1;
        private const int WAIT_TIMEOUT = 0x102;
        public const int WaitTimeout = 0x102;

        // Methods
        private EventWaitHandle(IntPtr aHandle)
        {
            if (aHandle.Equals(IntPtr.Zero))
            {
                throw new WaitHandleCannotBeOpenedException();
            }
            Handle = aHandle;
        }

        public EventWaitHandle(bool initialState, EventResetMode mode)
            : this(initialState, mode, null)
        {
        }

        public EventWaitHandle(bool initialState, EventResetMode mode, string name)
            : this(CreateEvent(IntPtr.Zero, mode == EventResetMode.ManualReset, initialState, name))
        {
        }

        public EventWaitHandle(bool initialState, EventResetMode mode, string name, out bool createdNew)
        {
            IntPtr ptr = CreateEvent(IntPtr.Zero, mode == EventResetMode.ManualReset, initialState, name);
            if (ptr.Equals(IntPtr.Zero))
            {
                throw new ApplicationException("Cannot create " + name);
            }
            createdNew = Marshal.GetLastWin32Error() != 0xb7;
            Handle = ptr;
        }
        protected EventWaitHandle()
        {
            
        }

        public override void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
        protected override void Dispose(bool explicitDisposing)
        {
            if (Handle != WaitHandle.InvalidHandle)
            {
                CloseHandle(Handle);
                Handle = WaitHandle.InvalidHandle;
            }
            base.Dispose(explicitDisposing);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError = true)]
        private static extern bool EventModify(IntPtr hEvent, EVENT ef);
        ~EventWaitHandle()
        {
            Dispose(false);
        }

        public int GetData()
        {
            return GetEventData(Handle);
        }

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern int GetEventData(IntPtr hEvent);
        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr OpenEvent(int dwDesiredAccess, bool bInheritHandle, string lpName);
        public static EventWaitHandle OpenExisting(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length < 1)
            {
                throw new ArgumentException("name is a zero-length string.");
            }
            if (name.Length > 260)
            {
                throw new ArgumentException("name is longer than 260 characters.");
            }
            IntPtr aHandle = OpenEvent(3, false, name);
            if (aHandle == IntPtr.Zero)
            {
                throw new WaitHandleCannotBeOpenedException();
            }
            return new EventWaitHandle(aHandle);
        }

        public bool Pulse()
        {
            return NativeMethods.EventModify(Handle, NativeMethods.EVENT.PULSE);
        }

        public bool Reset()
        {
            return EventModify(Handle, EVENT.RESET);
        }

        public bool Set()
        {
            return EventModify(Handle, EVENT.SET);
        }

        public bool Set(int data)
        {
            SetEventData(Handle, data);
            return NativeMethods.EventModify(Handle, NativeMethods.EVENT.SET);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll.dll", SetLastError = true)]
        public static extern bool SetEventData(IntPtr hEvent, int dwData);
        public static int WaitAny(IntPtr[] waitHandles)
        {
            return WaitAny(waitHandles, -1, false);
        }

        public static int WaitAny(WaitHandle[] waitHandles)
        {
            return WaitAny(waitHandles, -1, false);
        }

        public static int WaitAny(IntPtr[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            return WaitMultiple(waitHandles, millisecondsTimeout, false);
        }

        public static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            return WaitMultiple(waitHandles, millisecondsTimeout, false);
        }

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern int WaitForMultipleObjects(int nCount, IntPtr[] lpHandles, bool fWaitAll, int dwMilliseconds);
        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);
        private static int WaitMultiple(IntPtr[] handles, int millisecondsTimeout, bool waitAll)
        {
            return WaitForMultipleObjects(handles.Length, handles, waitAll, millisecondsTimeout);
        }

        private static int WaitMultiple(WaitHandle[] waitHandles, int millisecondsTimeout, bool waitAll)
        {
            if (waitHandles == null)
            {
                throw new ArgumentNullException("waitHandles cannot be a null array");
            }
            for (int i = 0; i < waitHandles.Length; i++)
            {
                if (waitHandles[i] == null)
                {
                    throw new ArgumentNullException(String.Format("waitHandle {0} cannot be null", i));
                }
            }
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds must be non-negative");
            }
            if (waitHandles.Length == 0)
            {
                throw new ApplicationException("waitHandles cannot be empty");
            }
            IntPtr[] handles = new IntPtr[waitHandles.Length];
            for (int j = 0; j < handles.Length; j++)
            {
                handles[j] = waitHandles[j].Handle;
            }
            return WaitMultiple(handles, millisecondsTimeout, false);
        }

        public override bool WaitOne()
        {
            return WaitOne(-1, false);
        }

        public override bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            return (WaitForSingleObject(Handle, millisecondsTimeout) != 0x102);
        }

        public bool WaitOne(TimeSpan timeout, bool exitContext)
        {
            return (WaitForSingleObject(Handle, (int)timeout.TotalMilliseconds) != 0x102);
        }

        // Nested Types
        private enum EVENT
        {
            PULSE = 1,
            RESET = 2,
            SET = 3
        }
    }
}
