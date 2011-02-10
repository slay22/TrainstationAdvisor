using System;

namespace TrainStationAdvisor.ClassLibrary
{
    public class CellTower
    {
        public int TowerId { get; set; }
        /// <summary>
        /// Gets or sets the location area code, a.k.a. LAC.
        /// </summary>
        /// <value>The location area code.</value>
        public int LocationAreaCode { get; set; }
        /// <summary>
        /// Gets or sets the mobile country code, a.k.a. MCC.
        /// </summary>
        /// <value>The mobile country code.</value>
        public int MobileCountryCode { get; set; }
        /// <summary>
        /// Gets or sets the mobile network code, a.k.a. MNC.
        /// </summary>
        /// <value>The mobile network code.</value>
        public int MobileNetworkCode { get; set; }

    }
}
