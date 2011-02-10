using System;

namespace TrainStationAdvisor.ClassLibrary
{
    public enum DistanceTypes
    {
        Meters,
        Km
    }

    public class Station
    {
        public int StationID { get; set; }
        public int StationIndex { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool PerformAlert { get; set; }
        public double AlertDistance { get; set; }
        public double DistanceToDestination { get; private set; }

        public DistanceTypes DistanceType
        {
            get
            {
                return (((DistanceToDestination * 1000) < 1000) ? DistanceTypes.Meters : DistanceTypes.Km);
            }
        }

        public void RefreshDistanceTo(double latitude, double longitude)
        {
            DistanceToDestination = Utils.CalcDistance(latitude, longitude, Latitude, Longitude);
        }

    }
}
