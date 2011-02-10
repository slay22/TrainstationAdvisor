using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;

namespace TrainStationAdvisor.ClassLibrary
{
    public class GPSTest
    {
        //public class GPSLocationChangedEventArgs: EventArgs
        //{
        //    /// <summary>
        //    /// Gets the new position when the GPS reports a new position.
        //    /// </summary>
        //    public GpsPosition Pos { get; private set; }

        //    public GPSLocationChangedEventArgs(GpsPosition APosition)
        //    {
        //        this.Pos = APosition;
        //    }

        //}

        //public class GPSErrosEventArgs: EventArgs
        //{
        //    /// <summary>
        //    /// Gets the new position when the GPS reports a new position.
        //    /// </summary>
        //    public string EvtType { get; private set; }

        //    public GPSErrosEventArgs(string AEventType)
        //    {
        //        this.EvtType = AEventType;
        //    }
        //}

        //public EventHandler<GPSLocationChangedEventArgs> LocationChanged;
        //public EventHandler<GPSErrosEventArgs> ErrorReceived; 

        //private SerialPort m_ComPort;

        //public bool IsOpen
        //{
        //    get
        //    {
        //        bool _Result = false;

        //        if (null != m_ComPort)
        //        {
        //            _Result = m_ComPort.IsOpen;
        //        }

        //        return _Result;
        //    }
        //}

        //public GPS()
        //{
        //    m_ComPort = new SerialPort();
        //    m_ComPort.DataReceived += new SerialDataReceivedEventHandler(ComPort_DataReceived);
        //    m_ComPort.ErrorReceived += new SerialErrorReceivedEventHandler(ComPort_ErrorReceived);
        //}

        //~GPS()
        //{
        //    m_ComPort.DataReceived -= new SerialDataReceivedEventHandler(ComPort_DataReceived);
        //    m_ComPort.ErrorReceived -= new SerialErrorReceivedEventHandler(ComPort_ErrorReceived);

        //    if (m_ComPort.IsOpen)
        //    {
        //        m_ComPort.Close();
        //    }
        //}

        //public void Open(string APortName)
        //{
        //    this.Open(APortName, 4800);
        //}

        //public void Open(string APortName, int ABaudRate)
        //{
        //    try
        //    {
        //        m_ComPort.PortName = APortName;
        //        m_ComPort.BaudRate = ABaudRate;
        //        m_ComPort.ReadTimeout = 5000;
        //        m_ComPort.Open();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        ///*
        //$GPGGA,123519,4807.038,N,01131.324,E,1,08,0.9,545.4,M,46.9,M,,*42

        //GGA  = Global Positioning System Fix Data

        //1    = UTC of Position
        //2    = Latitude
        //3    = N or S
        //4    = Longitude
        //5    = E or W
        //6    = GPS quality indicator (0=invalid; 1=GPS fix; 2=Diff. GPS fix)
        //7    = Number of satellites in use [not those in view]
        //8    = Horizontal dilution of position
        //9    = Antenna altitude above/below mean sea level (geoid)
        //10   = Meters  (Antenna height unit)
        //11   = Geoidal separation (Diff. between WGS-84 earth ellipsoid and
        //       mean sea level.  -=geoid is below WGS-84 ellipsoid)
        //12   = Meters  (Units of geoidal separation)
        //13   = Age in seconds since last update from diff. reference station
        //14   = Diff. reference station ID#
        //15   = Checksum
        // * 
        // * 
        // * 
        // *    public double Latitude    //Latitude, Breitengrad
        //        {    
        //            set
        //            {
        //                DataReceived     = true;
        //                gpsdata[2]        = value.ToString();
        //            }
            
        //            get
        //            {
        //                if(!DataReceived || gpsdata[2] == "")
        //                    return 0; 
                
        //                   return double.Parse(gpsdata[2].Replace(".",","), System.Globalization.NumberStyles.Currency);
        //            }
        //        }     
        // * 
        // * 
        //*/


        //private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    string _Tmp = m_ComPort.ReadLine();
        //    if (_Tmp.Contains("$GPGGA"))
        //    {

        //        string[] _Buffer = _Tmp.Split(',');

        //        GpsPosition _GpsPosition = new GpsPosition();

        //        if (_Buffer[3].Contains("N"))
        //        {
        //            _GpsPosition.Latitude = Replace(FormatNumber((Mid(buff(2), 1, 2)) + (Mid(buff(2), 3, 2) / 60) + ((Mid(buff(2), 6, 4) * 0.006) / 3600), 6), ",", ".")
        //        }
        //        {
        //            _GpsPosition.Latitude = Replace(FormatNumber((Mid(buff(2), 1, 2)) + (Mid(buff(2), 3, 2) / 60) + ((Mid(buff(2), 6, 4) * 0.006) / 3600), 6) * -1, ",", ".")
        //        }
        //        else
        //        {
        //            _GpsPosition.Latitude = 0;
        //        }


        //        _GpsPosition.dwSatelliteCount = Convert.ToInt32(_Buffer[7]);
        //        _GpsPosition.

        //    }
        //}

        //private void ComPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        //{
        //    OnErrorReceived(new GPSErrosEventArgs(e.EventType.ToString()));
        //}

        //protected virtual void OnLocationChanged(GPSLocationChangedEventArgs e)
        //{
        //    if (null != this.LocationChanged)
        //    {
        //        this.LocationChanged(this, e);
        //    }
        //}

        //protected virtual void OnErrorReceived(GPSErrosEventArgs e)
        //{
        //    if (null != ErrorReceived)
        //    {
        //        this.ErrorReceived(this, e);
        //    }
        //}

    }
}
