using System;
using SamsungMobileSdk;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TrainStationAdvisor.ClassLibrary;
using System.Threading;

namespace TrainStationAdvisor
{
    public class Vibrate
    {
#if SAMSUNG
        private Haptics.HapticsNote[] m_hapticsNotes;
        private int m_hapticsHandle;
        private Haptics.Notification m_defaultHapticsNotification;
#endif
        // Methods
        public Vibrate()
        {
#if SAMSUNG
            if (!InitHaptics())
            {
                Debug.WriteLine("Error: Failed to obtain Haptics handle.");
            }
#endif
        }

        ~Vibrate()
        {
#if SAMSUMG
            Haptics.Stop(m_hapticsHandle);
            Haptics.Close(m_hapticsHandle);
#endif
        }

#if SAMSUNG

        private bool InitHaptics()
        {
            bool result = true;

            if (Haptics.Open(ref m_hapticsHandle) != SmiResultCode.Success)
            {
                result = false;
            }
            else
            {
                m_hapticsNotes = new Haptics.HapticsNote[1];
                m_hapticsNotes[0].magnitude = 255;
                m_hapticsNotes[0].startingMagnitude = 0;
                m_hapticsNotes[0].endingMagnitude = 0;
                m_hapticsNotes[0].duration = 50;
                m_hapticsNotes[0].endTimeDuration = 0;
                m_hapticsNotes[0].startTimeDuration = 0;
                m_hapticsNotes[0].style = Haptics.NoteStyle.Sharp;
                m_hapticsNotes[0].period = 0;

                m_defaultHapticsNotification = DefaultNotification;

            }
            return result;
        }
#endif

        private static void DefaultNotification(int handle, Haptics.HapticsNote[] note)
        {

        }

        private static int GetDeviceCaps(VibrationCapabilities caps)
        {
            return NativeMethods.VibrateGetDeviceCaps(caps);
        }

        private static void SetVibrate(bool state)
        {
            Led.NLED_SETTINGS_INFO info = new Led.NLED_SETTINGS_INFO { LedNum = 1, OffOnBlink = state ? 1 : 0 };
            NativeMethods.NLedSetDevice(1, ref info);
        }

        public static void PlaySync(params int[] millisecondPattern)
        {
            try
            {
                for (int i = 0; i < millisecondPattern.Length; i++)
                {
                    SetVibrate(i % 2 == 0);
                    Thread.Sleep(millisecondPattern[i]);
                }
                SetVibrate(false);

            }
            catch (Exception)
            {
            }
        }

        public bool Play()
        {
            bool result = true;
            
#if SAMSUNG
            m_hapticsNotes[0].magnitude = 255;
            SmiResultCode r = Haptics.PlayNotes(m_hapticsHandle, 1, m_hapticsNotes, false, m_defaultHapticsNotification);

            return (r == SmiResultCode.Success);
#endif
            return result;

            ////if (NativeMethods.VibratePlay(0, IntPtr.Zero, uint.MaxValue, uint.MaxValue) != 0)
            //if (NativeMethods.VibratePlay(0, IntPtr.Zero, 1, NativeMethods.INFINITE) != 0)
            //{
            //    return false;
            //}
            //return true;
        }

        //public bool Stop()
        //{
        //    if (NativeMethods.VibrateStop() != 0)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public static void PlaySnd()
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            NativeMethods.SndPlaySync(string.Format("{0}\\Alarm.mp3", filePath), 0);         
        }

        // Nested Types
        public enum VibrationCapabilities
        {
            Amplitude,
            Frequency
        }

    }
}
