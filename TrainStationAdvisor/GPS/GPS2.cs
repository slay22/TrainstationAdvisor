using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using TrainStationAdvisor.ClassLibrary;

namespace TrainStationAdvisor.ClassLibrary
{
    public delegate void LocationChangedEventHandler(object sender, EventArgs.GPSLocationChangedEventArgs args);
    //public delegate void ErrorReceivedEventHandler(object sender, EventArgs.GPSErrosEventArgs args);
    public delegate void SignalStrengthChangedEventHandler(object sender, EventArgs.GPSSignalStrengthChangedEventArgs args);
    public delegate void SpeedChangedEventHandler(object sender, EventArgs.GPSSpeedChangedEventArgs args);

	public class GPS : IDisposable
	{
        public enum FixLevel
        {
            noFix,
            twoD,
            threeD
        }

        private bool m_Disposed;
		private Thread m_ReadThread;
        private bool m_StopThread;
        
        private int m_LastSignalStrength;
        private const int _Radius = 6378137;
        private const double m_Wgs84_a = 6378137;
        private const double m_NumerischeExzentrizitaet = 0.00669437999013;
        private const double m_Wgs84_b = m_Wgs84_a * (1.0 - (1.0 / 298.257223563));

        private bool m_UseSirfBinary;

        private FixLevel m_CurrentFixLevel = FixLevel.noFix;

        private Position m_CurrentPos;
		public Position CurrentPosition
		{
			get
			{
                return m_CurrentPos;
			}
		}

        private bool m_IsPortOpen;
        public bool Opened
        {
            get
            {
                return m_IsPortOpen;
            }
        }

        //public event EventHandler<EventArgs.GPSLocationChangedEventArgs> LocationChanged;
        //public event EventHandler<EventArgs.GPSErrosEventArgs> ErrorReceived;
        //public event EventHandler<EventArgs.GPSSignalStrengthChangedEventArgs> SignalStrengthChanged;
        //public event EventHandler<EventArgs.GPSSpeedChangedEventArgs> SpeedChanged;

        public event LocationChangedEventHandler LocationChanged;
        //public event ErrorReceivedEventHandler ErrorReceived;
        public event SignalStrengthChangedEventHandler SignalStrengthChanged;
        public event SpeedChangedEventHandler SpeedChanged;

		public GPS()
		{
            m_Disposed = false;
            m_IsPortOpen = false;

            m_ReadThread = new Thread(ReadGPS);
            m_ReadThread.Priority = ThreadPriority.BelowNormal;
            m_ReadThread.IsBackground = true;
		}

        ~GPS()
        {
            Dispose(false);
        }

        protected void OnLocationChanged(EventArgs.GPSLocationChangedEventArgs e)
        {
            if (null != LocationChanged)
            {
                LocationChanged(this, e);
            }
        }

        //protected void OnErrorReceived(EventArgs.GPSErrosEventArgs e)
        //{
        //    if (null != ErrorReceived)
        //    {
        //        ErrorReceived(this, e);
        //    }
        //}

        protected void OnSignalStrengthChanged(EventArgs.GPSSignalStrengthChangedEventArgs e)
        {
            if (null != SignalStrengthChanged)
            {
                SignalStrengthChanged(this, e);
            }
        }

        protected void OnSpeedChanged(EventArgs.GPSSpeedChangedEventArgs e)
        {
            if (null != SpeedChanged)
            {
                SpeedChanged(this, e);
            }
        }

        public void Open()
        {
            Console.WriteLine("{0}: GPS-Thread-ID: {1}", DateTime.Now, m_ReadThread.ManagedThreadId);
            m_ReadThread.Start();
            Thread.Sleep(10);
        }

        public void Close()
        {
            m_StopThread = true;
            m_ReadThread.Join();
        }

		private void ReadGPS()
		{
            SerialPort _Port = new SerialPort("COM7") { BaudRate = 38400, DataBits = 8, Handshake = Handshake.None, StopBits = StopBits.One, ReadTimeout = 5000 };

			while (true && !m_StopThread)
			{
				try
				{
					Console.Write("{0}: Opening GPS-Port...", DateTime.Now);
                    _Port.Open();
					Console.WriteLine("succeeded.");
                    InitModule(_Port);
                    m_IsPortOpen = true;
					break;
				}
				catch (Exception exc)
				{
                    m_IsPortOpen = false;
					Console.WriteLine("failed: "+ exc.Message);
				}

                Thread.Sleep(2500);
			}

			string _LastLine = string.Empty;

			while (!m_StopThread)
			{
				try
                {
                    #region SIRFBinary
                    if (m_UseSirfBinary)
                    {
                        throw new NotImplementedException();
                        //byte[] buffer;
                        //int read = -1;
                        //while (read!= 0 && !stopThread)
                        //{
                        //    buffer = new byte[2000];
                        //    read = port.Read(buffer, 0, 2000);

                        //    // Puffer auswerten
                        //    // Startsequenz: A0A2
                        //    for (int i = 0; i < read; i++)
                        //    {
                        //        /// AUFPASSEN AM ENDE DES BUFFERS!!!!
                        //        if (buffer[i]==0xA0 && buffer[i+1]==0xA2)
                        //        {
                        //            Debug.WriteLine(string.Format("Anfang gefunden bei {0}", i));
                        //            int payloadLength = buffer[i + 2] * 256 + buffer[i + 3];
                        //            byte[] subBuffer = new byte[payloadLength+2];
                        //            subBuffer.Initialize();
                        //            for (int j = 0; j < payloadLength; j++)
                        //            {
                        //                subBuffer[j] = buffer[i + j + 4];
                        //            }

                        //            string bufferAsString = ConvertToBinaryCodedDecimal(false, subBuffer);

                        //            #region 02 - Measure Navigation Data Out
                        //            if (subBuffer[0] == 0x02)
                        //            {
                        //                Position currentPos = new Position();
                        //                // Measure Navigation Data Out
                        //                int xPosition = Int32.Parse(bufferAsString.Substring(2,8), NumberStyles.AllowHexSpecifier);
                        //                int yPosition = Int32.Parse(bufferAsString.Substring(10, 8), NumberStyles.AllowHexSpecifier);
                        //                int zPosition = Int32.Parse(bufferAsString.Substring(18, 8), NumberStyles.AllowHexSpecifier);

                        //                currentPos.Quality = SignalQuality.SPS;
                                        
                        //                int mode1 = subBuffer[24];

                        //                if ((128 & mode1)>0)
                        //                    currentPos.Quality = SignalQuality.DGPS;

                        //                currentPos.Latitude = (float)GetLatitude(xPosition, yPosition, zPosition);
                        //                currentPos.Longitude = (float)GetLongitude(xPosition, yPosition);

                        //                currentPos.Timestamp = DateTime.Now;
                        //                currentPos.NumberOfSatellites = subBuffer[33];

                        //                if (currentPos.NumberOfSatellites != lastSignalStrength)
                        //                {
                        //                    lastSignalStrength = currentPos.NumberOfSatellites;
                        //                    if (SignalStrengthChanged != null && currentFixLevel!= FixLevel.noFix)
                        //                        SignalStrengthChanged((currentPos.NumberOfSatellites * 100) / 12);
                        //                }

                        //                if (PositionInformationReceived != null)
                        //                    PositionInformationReceived(currentPos);

                        //            }
                        //            #endregion
                        //            #region 1B - DGPS Status
                        //            if (subBuffer[0] == 0x1B)
                        //            {
                        //                Console.WriteLine();
                        //            }
                        //            #endregion

                        //        }
                        //    }
                        //}
                    }
                    #endregion
                    else
                    {
                        Application.DoEvents();
                        _LastLine = "-";
                        while (_LastLine != string.Empty && !m_StopThread)
                        {
                            _LastLine = _Port.ReadLine();
                            //Thread.Sleep(100);
                            if (CheckChecksum(_LastLine))
                            {
                                string[] _Token = _LastLine.Substring(1, _LastLine.Length - 5).Split(',');
                                System.Diagnostics.Debug.WriteLine(_LastLine);
                                
                                // Welcher Datensatz kam?
                                switch (_Token[0])
                                {
                                    case "GPGSV":
                                        InterpretGSV(_Token);
                                        break;
                                    case "GPGGA":
                                        InterpretGGA(_Token);
                                        break;
                                    //case "GPVTG":
                                    //    InterpretVTG(token);
                                    //    break;
                                    case "GPGSA":
                                        InterpretGSA(_Token);
                                        break;
                                    case "GPRMC":
                                        InterpretRMC(_Token);
                                        break;
                                }
                            }
                        }
                    }

					Thread.Sleep(1000);

				}
				catch (TimeoutException exc)
				{
                    Console.WriteLine("{0}: TimeoutException: {1}", DateTime.Now, exc.Message);
					
                    m_CurrentPos = new Position();
                    m_CurrentPos.SignalStrength = 0;

                    OnSignalStrengthChanged(new EventArgs.GPSSignalStrengthChangedEventArgs(m_CurrentPos.SignalStrength));

					// Try to reopen port.
					while (true && !m_StopThread)
					{
						try
						{
                            _Port.Close();
							Console.Write("{0}: Opening GPS-Port...", DateTime.Now);
                            _Port.Open();
							Console.WriteLine("succeeded.");
                            InitModule(_Port);
                            m_IsPortOpen = true;
							break;
						}
						catch (Exception exc2)
						{
                            m_IsPortOpen = false;
							Console.WriteLine("failed (2): " + exc2.Message);
						}
						Thread.Sleep(2500);
					}
				}
				catch (IOException exc)
				{
                    Console.WriteLine("{0}: Ein Ein-Ausgabefehler ist aufgetreten beim Lesen vom Com-Port", DateTime.Now);
					m_LastSignalStrength = 0;

                    m_CurrentPos = new Position();
                    m_CurrentPos.SignalStrength = 0;

                    m_CurrentPos.Timestamp = DateTime.Now;

                    OnSignalStrengthChanged(new EventArgs.GPSSignalStrengthChangedEventArgs(m_CurrentPos.SignalStrength));

                    m_ReadThread = new Thread(ReadGPS);
                    m_ReadThread.IsBackground = true;
                    m_ReadThread.Start();

					break;
				}
            }

            m_IsPortOpen = false;

			_Port.Close();
			_Port.Dispose();
		}
 
        private void InterpretGSA(string[] token)
        {
            if (token[2] == "1")
            {
                m_LastSignalStrength = 0;
                m_CurrentPos.SignalStrength = 0;

                OnSignalStrengthChanged(new EventArgs.GPSSignalStrengthChangedEventArgs(m_CurrentPos.SignalStrength));

                m_CurrentFixLevel = FixLevel.noFix;
            }
            else if (token[2] == "2")
            {
                m_CurrentFixLevel = FixLevel.twoD;
            }
            else
            {
                m_CurrentFixLevel = FixLevel.threeD;
            }
        }

        private double GetLongitude(double X, double Y)
        {           
            double _Lon =  Math.Atan(Y / X)/(Math.PI/180.0);
            if (_Lon < 0)
            {
                _Lon += 360.0;
            }
            
            return _Lon;
        }

        private double GetLatitude(double X, double Y, double Z)
        {
            double p = Math.Sqrt(X * X + Y * Y);
            double rp = Math.Sqrt(X*X+Y*Y+Z*Z);
            double dtr = Math.PI / 180.0;
            double asq = m_Wgs84_a * m_Wgs84_a;
            double bsq = m_Wgs84_b * m_Wgs84_b;
            double eccsq = 1 - bsq / asq;
            double ecc = Math.Sqrt(eccsq);
            double esq = ecc * ecc;

            double flatgc = Math.Asin(Z/rp)/dtr;

            double rnow = rearth(flatgc);
            double altkm = rp - rnow;
            double flat = gc2gd(flatgc, altkm);
            double rn = radcur(flat)[1];

            double slat, tangd, flatn, dlat, clat;
            for (int i = 0; i < 5; i++)
            {
                slat = Math.Sin(dtr * flat);
                tangd = (Z + rn * esq * slat) / p;
                flatn = Math.Atan(tangd) / dtr;
                dlat = flatn - flat;
                flat = flatn;
                clat = Math.Cos(dtr * flat);
                rn = radcur(flat)[1];
                altkm = (p / clat) - rn;
                if (Math.Abs(dlat) < 0.000000000001)
                    break;

            }

            return flat;
        }

        private static double gc2gd(double flatgc, double altkm)
        {
            double dtr = Math.PI / 180.0;
            double rtd = 1/dtr;
            double asq = m_Wgs84_a * m_Wgs84_a;
            double bsq = m_Wgs84_b * m_Wgs84_b;
            double eccsq = 1 - bsq / asq;
            double ecc = Math.Sqrt(eccsq);
            double esq = ecc * ecc;
            double altnow = altkm;
            double rn = radcur(flatgc)[1];
            double ratio = 1 - esq * rn/(rn + altnow);
            double tlat = Math.Tan(dtr * flatgc) * ratio;
            double flatgd = rtd * Math.Atan(tlat);
            rn = radcur(flatgd)[1];
            ratio = 1 - esq * rn/(rn + altnow);
            tlat = Math.Tan(dtr * flatgc) / ratio;
            flatgd = rtd * Math.Atan(tlat);

            return flatgd;

        }

        static double rearth(double lati)
        {            
            return radcur(lati)[0];
        }

        private static double[] radcur(double lati)
        {
            double dtr = Math.PI / 180.0;
            double[] result = new double[3];
            double asq = m_Wgs84_a * m_Wgs84_a;
            double bsq = m_Wgs84_b * m_Wgs84_b;
            double eccsq = 1 - bsq / asq;
            double ecc = Math.Sqrt(eccsq);

            double clat = Math.Cos(dtr * lati);
            double slat = Math.Sin(dtr * lati);
            double dsq = 1.0 - eccsq * slat * slat;
            double d = Math.Sqrt(dsq);
            double rn = m_Wgs84_a / d;
            double rm = rn*(1.0-eccsq)/dsq;
            double rho = rn * clat;
            double z = (1.0 - eccsq) * rn * slat;
            double rsq = rho * rho + z * z;
            double r = Math.Sqrt(rsq);
            result[0] = r;
            result[1] = rn;
            result[2] = rm;

            return result;
        }

        public static string ConvertToBinaryCodedDecimal(bool isLittleEndian, params byte[] bytes)
        {
            StringBuilder bcd = new StringBuilder(bytes.Length * 2);
            if (isLittleEndian)
            {
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    byte bcdByte = bytes[i];
                    int idHigh = bcdByte >> 4;
                    int idLow = bcdByte & 0x0F;                    
                    bcd.Append(string.Format("{0:X}{1:X}", idHigh, idLow));
                }
            }
            else
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    byte bcdByte = bytes[i];
                    int idHigh = bcdByte >> 4;
                    int idLow = bcdByte & 0x0F;
                    
                    bcd.Append(string.Format("{0:X}{1:X}", idHigh, idLow));
                }
            }
            return bcd.ToString();
        }

		private static void InitModule(SerialPort port)
		{
			//port.WriteLine("$PSRF105,1*3E");
			//port.WriteLine("$PSRF109,120*30");
		}

		private void InterpretRMC(string[] AToken)
		{
			 //0	RMC          Recommended Minimum sentence C
			 //1	123519       Fix taken at 12:35:19 UTC
			 //2	A            Status A=active or V=Void.
			 //3	4807.038,N   Latitude 48 deg 07.038' N
			 //4	01131.000,E  Longitude 11 deg 31.000' E
			 //5	022.4        Speed over the ground in knots
			 //6	084.4        Track angle in degrees True
			 //7	230394       Date - 23rd of March 1994
			 //8	003.1,W      Magnetic Variation
			 //9	*6A          The checksum data, always begins with *

			// Aktuell wird aus dem RMC lediglich die Geschwindigkeit ausgelesen.
            if (AToken.Length < 8 || AToken[7] == string.Empty)
				return;

            double _Speed = double.Parse(AToken[7]) * 1.852;

            OnSpeedChanged(new EventArgs.GPSSpeedChangedEventArgs(_Speed));
		}

		private void InterpretVTG(string[] AToken)
		{
			//0		VTG          Track made good and ground speed
			//1		054.7,T      True track made good (degrees)
			//2		034.4,M      Magnetic track made good
			//3		005.5,N      Ground speed, knots
			//4		010.2,K      Ground speed, Kilometers per hour
			//*48          Checksum
            if (AToken.Length < 6)
				return;

            double _Speed = double.Parse(AToken[5].Substring(0, AToken[5].Length - 2));

            OnSpeedChanged(new EventArgs.GPSSpeedChangedEventArgs(_Speed));
		}

		private void InterpretGGA(string[] AToken)
		{
			//0		GGA          Global Positioning System Fix Data
			//1		Fix taken at 12:35:19 UTC
			//2		Latitude 48 deg 07.038' N
			//3		North/South			 
			//4		Longitude 11 deg 31.000' E
			//5		East/West
			//6		Fix quality:	0 = invalid
			//						1 = GPS fix (SPS)
			//						2 = DGPS fix
			//						3 = PPS fix
			//						4 = Real Time Kinematic
			//						5 = Float RTK
			//						6 = estimated (dead reckoning) (2.3 feature)
			//						7 = Manual input mode
			//						8 = Simulation mode
			//7		Number of satellites being tracked
			//8		Horizontal dilution of position
			//9		Altitude, Meters, above mean sea level
			//10	Unit
			//11	Height of geoid (mean sea level) above WGS84 ellipsoid
			//12	Unit
			//13	(empty field) time in seconds since last DGPS update
			//14	(empty field) DGPS station ID number
			
			// Nur bei gültigem Signal weiterverarbeiten

            if (int.Parse(AToken[6]) != 0)
			{
				m_CurrentPos = new Position();

                DateTime _Value = DateTime.Parse(string.Format("{0}:{1}:{2}", AToken[1].Substring(0, 2), AToken[1].Substring(2, 2), AToken[1].Substring(4, 2)));
                DateTime _ConvertedDate = DateTime.SpecifyKind(_Value, DateTimeKind.Utc);

                m_CurrentPos.Timestamp = _ConvertedDate.ToLocalTime();

				CultureInfo _English = new CultureInfo("en-US");

                m_CurrentPos.Latitude = float.Parse(AToken[2].Substring(0, 2), _English.NumberFormat) + float.Parse(AToken[2].Substring(2), _English.NumberFormat) / 60.0f;
                m_CurrentPos.LatitudeHemisphere = AToken[3].Equals("N") ? LatHemisphere.North : LatHemisphere.South;
                m_CurrentPos.Longitude = float.Parse(AToken[4].Substring(0, 3), _English.NumberFormat) + float.Parse(AToken[4].Substring(3), _English.NumberFormat) / 60.0f;
                m_CurrentPos.LongitudeHemisphere = AToken[5].Equals("W") ? LonHemisphere.West : LonHemisphere.East;

                switch (int.Parse(AToken[6]))
				{
					case 1:
                        m_CurrentPos.Quality = SignalQuality.SPS;						
						break;
					case 2:
                        m_CurrentPos.Quality = SignalQuality.DGPS;
						break;
					case 3:
                        m_CurrentPos.Quality = SignalQuality.PPS;
						break;
					case 4:
                        m_CurrentPos.Quality = SignalQuality.RTK;
						break;
					case 5:
                        m_CurrentPos.Quality = SignalQuality.FRTK;
						break;
					case 6:
                        m_CurrentPos.Quality = SignalQuality.Estimated;
						break;
					case 7:
                        m_CurrentPos.Quality = SignalQuality.Manual;
						break;
					case 8:
                        m_CurrentPos.Quality = SignalQuality.Simulated;
						break;
				}

                m_CurrentPos.NumberOfSatellites = int.Parse(AToken[7]);
                m_CurrentPos.HorizontalDilution = AToken[8] == string.Empty ? 0 : decimal.Parse(AToken[8], _English.NumberFormat);

                m_CurrentPos.Altitude = decimal.Parse(AToken[9], _English.NumberFormat);
			}

            if (m_CurrentPos.NumberOfSatellites != m_LastSignalStrength)
			{
                m_LastSignalStrength = m_CurrentPos.NumberOfSatellites;
                m_CurrentPos.SignalStrength = ((m_CurrentPos.NumberOfSatellites * 100) / 12);

				if (m_CurrentFixLevel != FixLevel.noFix)
                {
                    OnSignalStrengthChanged(new EventArgs.GPSSignalStrengthChangedEventArgs(m_CurrentPos.SignalStrength));
                }
			}

            OnLocationChanged(new EventArgs.GPSLocationChangedEventArgs(m_CurrentPos));
		}

		public class Position
		{
			public DateTime Timestamp { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public LatHemisphere LatitudeHemisphere { get; set; }
            public LonHemisphere LongitudeHemisphere { get; set; }
            public SignalQuality Quality { get; set; }
            public decimal Altitude { get; set; }
            public int NumberOfSatellites { get; set; }
            public decimal HorizontalDilution { get; set; }
            public double Speed { get; set; }
            public int SignalStrength { get; set; }

            public DateTime Time
            {
                get
                {
                    DateTime _Result = DateTime.Now;

                    if (TimeValid)
                    {
                        _Result = Timestamp;
                    }
                
                    return _Result;
                }
            }

            public static bool LatitudeValid
            {
                get
                {
                    return true;
                }
            }

            public static bool LongitudeValid
            {
                get
                {
                    return true;
                }
            }

            public bool TimeValid
            {
                get
                {
                    return (null != Timestamp);
                }
            }

            public bool SpeedValid
            {
                get
                {
                    return (null != Speed);
                }
            }

            public bool SatellitesInSolutionValid
            {
                get
                {
                    return (NumberOfSatellites > 0);
                }
            }

            public bool SatellitesInViewValid
            {
                get
                {
                    return (NumberOfSatellites > 0);
                }
            }


            public bool SatelliteCountValid
            {
                get
                {
                    return (NumberOfSatellites > 0);
                }
            }
		}

		public enum SignalQuality
		{
			SPS,
			DGPS,
			PPS,
			RTK,
			FRTK,
			Estimated,
			Manual,
			Simulated
		}

		public enum LatHemisphere
		{
			North,
			South
		}

		public enum LonHemisphere
		{
			East,
			West
		}
		
        private List<string[]> GSVTokens;

		private void InterpretGSV(string[] AToken)
		{
			//0		GSV          Satellites in view
			//1		Number of sentences for full data
			//2		sentence 1 of 2
			//3		Number of satellites in view		
			//4		Satellite PRN number
			//5		Elevation, degrees
			//6		Azimuth, degrees
			//7		SNR - higher is better
			
			// Neuer Satz?
            if (int.Parse(AToken[2]) == 1)
                GSVTokens = new List<string[]>(int.Parse(AToken[1]));

			// Satz hinzufügen
			if (GSVTokens != null)
			{
                GSVTokens.Add(AToken);
			}

			// Vollständig?
            if (GSVTokens != null && GSVTokens.Count == int.Parse(AToken[1]))
			{
				//Console.WriteLine();
			}

		}

		private static bool CheckChecksum(string ALastLine)
		{
            bool _Result = false;

            //if (ALastLine.Length == 0 || ALastLine.Length < 5)
            //    return false;

            if (ALastLine.Length > 0 && ALastLine.Length >= 5)
            {
                string _ToCheck = ALastLine.Substring(1, ALastLine.Length - 5);
                string _CheckSum = ALastLine.Substring(ALastLine.Length - 3, 2);

		        int _Check = -1;
                foreach (char currentChar in _ToCheck)
		        {
                    if (_Check == -1)
                        _Check = currentChar;
			        else
                        _Check ^= currentChar;
		        }

		        try
		        {
                    _Result = (int.Parse(_CheckSum, NumberStyles.AllowHexSpecifier) == _Check);
		        }
		        catch 
		        {				
		        }
            }

            return _Result;
		}

		#region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool ADisposing)
		{
            if (!m_Disposed)
            {
                // wenn true, alle managed und unmanaged resources mussen aufgelegt werden.
                if (ADisposing)
                {
                    //nix zu machen in moment
                }

                m_StopThread = true;
                m_ReadThread.Join();
                Console.WriteLine("{0}: GPS-Thread beendet", DateTime.Now);
            }

            m_Disposed = true;
		}
		#endregion
	}
}
