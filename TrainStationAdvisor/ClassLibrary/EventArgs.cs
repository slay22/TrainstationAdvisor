using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace TrainStationAdvisor.ClassLibrary
{
    public class EventArgs
    {
        public class GPSLocationChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the new position when the GPS reports a new one.
            /// </summary>
            public GPS.Position Position { get; private set; }

            public GPSLocationChangedEventArgs(GPS.Position APosition)
            {
                Position = APosition;
            }

        }

        public class GPSErrosEventArgs : EventArgs
        {
            /// <summary>
            /// Gets an Error when the GPS reports a new one.
            /// </summary>
            public string EventType { get; private set; }

            public GPSErrosEventArgs(string AEventType)
            {
                EventType = AEventType;
            }
        }

        public class GPSSignalStrengthChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the new SignalStrength when the GPS reports a new one.
            /// </summary>
            public int SignalStrength { get; private set; }

            public GPSSignalStrengthChangedEventArgs(int ASignalStrength)
            {
                SignalStrength = ASignalStrength;
            }
        }

        public class GPSSpeedChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the new Speed change when the GPS reports a new one.
            /// </summary>
            public double Speed { get; private set; }

            public GPSSpeedChangedEventArgs(double ASpeed)
            {
                Speed = ASpeed;
            }
        }

        public class CellIdCollectorEventArgs : System.EventArgs
        {
            public GeoLocation GeoLocation { get; private set; }

            public CellIdCollectorEventArgs(GeoLocation geoLocation)
            {
                GeoLocation = geoLocation;
            }
        }
    }
}
