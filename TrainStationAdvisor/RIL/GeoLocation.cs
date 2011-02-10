using System;

namespace TrainStationAdvisor.ClassLibrary
{
    public class GeoLocation
    {
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>The latitude.</value>
        public double Latitude { get; set; }
        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>The longitude.</value>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets the longitude/latitude are valid.
        /// </summary>
        /// <value>The longitude.</value>
        public bool IsValid 
        {
            get
            {
                return (Latitude != 0 && Longitude != 0);
            }
        }

        /// <summary>
        /// Gets an empty GeoLocation instance.
        /// </summary>
        /// <value>Latitude and Longitude are set to double.NaN.</value>
        public static GeoLocation Empty
        {
            get { return new GeoLocation() { Latitude = 0, Longitude = 0 }; }
        }
        /// <summary>
        /// Compares GeoLocation instances for equality
        /// </summary>
        /// <param name="toCompare">To compare.</param>
        /// <returns>True if equal; otherwise false</returns>
        public override bool Equals(object toCompare)
        {
            if (!(toCompare is GeoLocation))
            {
                return false;
            }

            return Equals((GeoLocation)toCompare);
        }
        /// <summary>
        /// Compares GeoLocation instances for equality
        /// </summary>
        /// <param name="toCompare">To compare.</param>
        /// <returns>True if equal; otherwise false</returns>
        public bool Equals(GeoLocation toCompare)
        {
            // Avoid null reference exceptions
            if (toCompare == null)
            {
                return false;
            }

            // Check comparison types for equality before object comparison
            if (GetType() != toCompare.GetType())
            {
                return false;
            }

            GeoLocation location = (GeoLocation)toCompare;

            // Check for equality of latitude
            if (!this.Latitude.Equals(location.Latitude))
            {
                return false;
            }

            // Check for equality of longitude
            return this.Longitude.Equals(toCompare.Longitude);
        }
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode();
        }
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="location1">The left hand location.</param>
        /// <param name="location2">The right hand location.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(GeoLocation location1, GeoLocation location2)
        {
            // Call ReferenceEquals to prevent recursion
            if (object.ReferenceEquals(location1, null))
            {
                return object.ReferenceEquals(location2, null);
            }

            if (object.ReferenceEquals(location2, null))
            {
                return false;
            }

            if (location1.GetType() != location2.GetType())
            {
                return false;
            }

            return location1.Equals(location2);
        }
        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="location1">The left hand location.</param>
        /// <param name="location2">The right hand location.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(GeoLocation location1, GeoLocation location2)
        {
            return !(location1 == location2);
        }
    }
}
