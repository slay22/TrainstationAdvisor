using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace TrainStationAdvisor
{
    public static class Utils
    {
        public static double EstimatedTimeOfArrival(double ASpeed, double ADistance)
        {
/*
            double _ETASeconds = (( _Distance * 1000 ) / ( _Speed * 0.277777777777777777777777777777777777));

            int _Hours = (int)(_ETASeconds / 3600);
            _ETASeconds = (_ETASeconds - _Hours * 3600 );
            int _Mins = ( int ) ( _ETASeconds * 60 );
            _ETASeconds = ( _ETASeconds - _Mins * 60 );
*/
            return ((ADistance * 1000) / (ASpeed * 0.277777777777777777777777777777777777));
        }

/*

<option value=1>Meters

<option value=0.001>Millimeters

<option value=0.01>Centimeters

<option value=1000>Kilometers

<option value=0.0254>Inches

<option value=0.3048>Feet


<option value=0.9144>Yards

<option value=1.8288>Fathoms

<option value=1609.344>Statute Miles

<option value=1852>Nautical Miles

<option value=9460000000000000>Light Years


<option value=1>Meters per Second

<option value=0.277777777777777777777777777777777777>Kilometers per Hour

<option value=0.3048>Feet per Second

<option value=0.00508>Feet per Minute

<option value=0.01524>Yards per Minute

<option value=0.44704>Statute Miles per Hour

<option value=0.514444444444>Knots



function calculatetime(form) {

//  get conversion factors from selected options

var i = form.distunits.selectedIndex;

var distunitsvalue = form.distunits.options[i].value; 

var j = form.speedunits.selectedIndex;

var speedunitsvalue = form.speedunits.options[j].value;

//  calculate time in seconds    

form.secondvalue.value = (form.distvalue.value * distunitsvalue) / (form.speedvalue.value * speedunitsvalue);

//  convert to hours, minutes, seconds    

form.hourvalue.value = parseInt(form.secondvalue.value / 3600);

form.secondvalue.value = form.secondvalue.value - (form.hourvalue.value * 3600);

form.minutevalue.value = parseInt(form.secondvalue.value / 60);

form.secondvalue.value = parseInt(form.secondvalue.value - (form.minutevalue.value * 60));

return true;

}

*/

        public static string ETAString(string AMessage, double AEtaValue)
        {
            string _ETAType = "h(s)";
            if (AEtaValue < 1)
            {
                _ETAType = "min(s)";
                AEtaValue = (AEtaValue * 100);
            }

            return string.Format(AMessage, AEtaValue, _ETAType);
        }


        public const double EarthRadiusInMiles = 3956.0;
        public const double EarthRadiusInKilometers = 6367.0;

        public static double ToRadian( double val )
        {
            return val * ( Math.PI / 180 );
        }
    
        public static double DiffRadian( double val1, double val2 )
        {
            return ToRadian( val2 ) - ToRadian( val1 );
        }

        /// <summary>
        /// Calculate the distance between two geocodes. Defaults to using Kilometers.
        /// </summary>
        public static double CalcDistance(double ALat1, double ALong1, double ALat2, double ALong2 )
        {
            return CalcDistance( ALat1, ALong1, ALat2, ALong2, GeoCodeCalcMeasurement.Kilometers );
        }
        
        /// <summary>
        /// Calculate the distance between two geocodes.
        /// </summary>
        public static double CalcDistance(double ALat1, double ALong1, double ALat2, double ALong2, GeoCodeCalcMeasurement AMeasurement)
        {                                                                                   
            double radius = (AMeasurement == GeoCodeCalcMeasurement.Kilometers ? Utils.EarthRadiusInKilometers : Utils.EarthRadiusInMiles);

            return (radius * 2 * Math.Asin( Math.Min( 1, Math.Sqrt( ( Math.Pow( Math.Sin( ( DiffRadian( ALat1, ALat2 ) ) / 2.0 ), 2.0 ) + Math.Cos( ToRadian( ALat1 ) ) * Math.Cos( ToRadian( ALat2 ) ) * Math.Pow( Math.Sin( ( DiffRadian( ALong1, ALong2 ) ) / 2.0 ), 2.0 ) ) ) ) ));
        }

        public enum GeoCodeCalcMeasurement : int
        {
           Miles = 0,
           Kilometers = 1
        }

/*
        //This function calculates the direct line distance (km) between two pairs of lat long
        public static double GetLatLongTuppleDistance(double ALat1, double ALong1, double ALat2, double ALong2)
        {
            //Convert Degress to Radians for Calculations
            double _Lat1r = ConvertDegreesToRadians(ALat1);
            double _Lat2r = ConvertDegreesToRadians(ALat2);
            double _Long1r = ConvertDegreesToRadians(ALong1);
            double _Long2r = ConvertDegreesToRadians(ALong2);

            //Spherical law of cosines formula - ignores the effect of hills
            double _Radius = 6371; // Earth’s radius (km)
            double _Distance = Math.Acos(Math.Sin(_Lat1r) * Math.Sin(_Lat2r) +
                               Math.Cos(_Lat1r) * Math.Cos(_Lat2r) *
                               Math.Cos(_Long2r - _Long1r)) * _Radius;

            //Returns distances in km
            return _Distance;
        }

        public static double ConvertDegreesToRadians(double ADegrees)
        {
            double _Radians = (Math.PI / 180) * ADegrees;
            return (_Radians);
        }
*/
    }
}
