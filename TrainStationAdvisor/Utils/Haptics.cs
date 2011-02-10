using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace SamsungMobileSdk
{
    public class Haptics
    {
        // callback notification function 
        private delegate void c_Notification(int handle, IntPtr note);

        // state
        public enum HapticsState
        {
            Idle = 0,
            Playing = 1,
            Paused = 2,
        } 

        public enum NoteStyle
        {
            Strong = 0,
            Smooth = 1,
            Sharp = 2
        } 

        // note structure
        public struct HapticsNote
        {
            public byte magnitude;
            public byte startingMagnitude;
            public byte endingMagnitude;
            public uint duration;
            public uint startTimeDuration;
            public uint endTimeDuration;
            public NoteStyle style;
            public uint period;
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        public struct c_Capabilities
        {
            public uint maxPeriod;
            public uint minPeriod;

            //  feature supported 
            public uint startEndMag;  // support start/end mangitude if is 1,otherwise is 0
            public uint noteStyle;    // support note style if is 1,otherwise is 0 
            public uint periodicPlay; // support periodic play if is 1,otherwise is 0 
            public uint pauseResume;  // support pause and resume if is 1,otherwise is 0 
        }       

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiHapticsGetCapabilities")]
        private static extern SmiResultCode c_GetCapabilities(ref c_Capabilities capability);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiHapticsOpen")]
        public static extern SmiResultCode Open(ref int handle);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiHapticsClose")]
        public static extern void Close(int handle);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiHapticsPlayNotes")]
        private static extern SmiResultCode c_PlayNotes(int handle,
                                           uint noteCount,
                                           HapticsNote[] notes,
                                           bool repeat,
                                           c_Notification callback);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiHapticsPause")]
        public static extern SmiResultCode Pause(int handle);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiHapticsResume")]
        public static extern SmiResultCode Resume(int handle);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiHapticsStop")]
        public static extern SmiResultCode Stop(int handle);

        [DllImport(Shared.SamsungMobileSDKDllName, EntryPoint = "SmiHapticsGetState")]
        public static extern SmiResultCode GetState(int handle, ref HapticsState state);


        public struct Capabilities
        {
            public uint maxPeriod;
            public uint minPeriod;

            //  feature supported 
            public uint startEndMag;  // support start/end mangitude if is 1,otherwise is 0
            public uint noteStyle;    // support note style if is 1,otherwise is 0 
            public uint periodicPlay; // support periodic play if is 1,otherwise is 0 
            public uint pauseResume;  // support pause and resume if is 1,otherwise is 0 
        }

        public static SmiResultCode GetCapabilities(ref Capabilities capability)
        {
            c_Capabilities c = new c_Capabilities();
            SmiResultCode result = c_GetCapabilities(ref c);
                       
            if (result == SmiResultCode.Success)
            {
                capability.maxPeriod = c.maxPeriod;
                capability.minPeriod = c.minPeriod;
                capability.noteStyle = c.noteStyle;
                capability.pauseResume = c.pauseResume;
                capability.periodicPlay = c.periodicPlay;
                capability.startEndMag = c.startEndMag;
            }
             
            return result;

        }
        
        private struct CallbackItem
        {
            public HapticsNote [] notesArray;
            public Notification callback;
            public c_Notification c_callback; 
            public int handle;
        }


        static void NotificationCallbakHandler(int handle, IntPtr note)
        {           
            //System.Console.WriteLine("@NotificationCallbakHandler handle  = " + handle);
            //System.Console.WriteLine("@NotificationCallbakHandler note    = " + note);

            HapticsNote[] notesArray = null;
            Notification callback=null;

            lock (_locker)
            {
                // protect the _list inside of critical section
                foreach( CallbackItem item in _list)
                {
                    if (item.handle == handle)
                    {
                        callback   = item.callback;
                        notesArray = item.notesArray;
                        _list.Remove(item);
                        break;
                    }
                }
            }

            if (callback != null)
            {
                callback(handle,notesArray);
            }             
        }

        public delegate void Notification(int handle, HapticsNote[] note);

        static public SmiResultCode PlayNotes(int handle,
                             uint noteCount,
                             HapticsNote[] notes,
                             bool repeat,
                             Notification callback)
        {                        
            CallbackItem item= new CallbackItem();
            c_Notification cN = new c_Notification(Haptics.NotificationCallbakHandler);

            item.callback   = callback;
            item.handle     = handle;
            item.notesArray = notes;
            item.c_callback = cN; // reserve callback until callback is done

            lock (_locker)
            {
                // protect the _list inside of critical section
                _list.Add(item);
            }

            SmiResultCode result = c_PlayNotes(handle,
                               noteCount,
                               notes,
                               repeat,
                               cN);

            //System.Console.WriteLine("@PlayNotes handle     = " + handle);
            //System.Console.WriteLine("@PlayNotes noteCount  = " + noteCount);
            //System.Console.WriteLine("@PlayNotes repeat     = " + repeat);
            //System.Console.WriteLine("@PlayNotes result  = " + result);

            return result;
        }

        
        static private List<CallbackItem> _list=new List<CallbackItem>();
        static object _locker = new object();
    }
}
