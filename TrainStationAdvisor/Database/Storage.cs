using System;
using System.Collections.Generic;
using System.Data;

namespace TrainStationAdvisor
{
    public class Storage : ClassLibrary.Interfaces.ICellStorage, IDisposable
    {
        private bool m_Disposed;
        private Database m_Database;

        public Storage()
        {
            m_Database = new Database();
            m_Database.Open();
        }

        ~Storage()
        {
            Dispose(false);
        }

        public DataSet GetRoutes()
        {
            return m_Database.Execute("Routes", "SELECT ID, Name FROM Routes");
        }

        public DataSet GetStationRoutes()
        {
            string _Sql = @"SELECT rd.RouteID, rd.StationID, rd.[Order], s.Name, s.Lat, s.Lon" +
                            " FROM RouteDetail AS rd, Stations AS s" +
                           " WHERE s.ID = rd.StationID";

            return  m_Database.Execute("Stations", _Sql);
        }

        public DataRow GetStationByID(string AStationID)
        {
            string _Sql = string.Format("SELECT s.ID, s.Name, s.Lat, s.Lon" +
                                        " FROM Stations AS s" +
                                       " WHERE s.ID = {0}", AStationID);

            return GetDataRow("Station", _Sql);
        }

        public DataRow GetLastStationForRoute(string ARouteID)
        {
            string _Sql = string.Format("SELECT s.ID, s.Name, s.Lat, s.Lon" +
                                  " FROM RouteDetail AS rd, Stations AS s " +
                                 " WHERE rd.StationID = s.ID" +
                                   " AND rd.RouteID = {0} " +
                                   " AND (rd.[Order] IN " +
                                   " (SELECT MAX(rd2.[Order]) AS MaxOrder " +
                                      " FROM RouteDetail AS rd2 " +
                                     " WHERE rd2.RouteID = rd.RouteID))", ARouteID);

            return GetDataRow("Station", _Sql);
        }

        public DataRow GetCellByID(string ACellId)
        {
            string _Sql = string.Format("SELECT c.CellId, c.Lat, c.Lon" +
                                        " FROM CellIds AS c" +
                                       " WHERE c.CellId = '{0}'", ACellId);

            return GetDataRow("CellId", _Sql);
        }

        public void InsertCellId(string ACellId, string ALatitude, string ALongitude)
        {
            string _Sql = string.Format("INSERT INTO CellIds VALUES ('{0}', '{1}', '{2}')", ACellId, ALatitude, ALongitude);
            m_Database.ExecuteNonQuery(_Sql);
        }


        private DataRow GetDataRow(string ATable, string ASql)
        {
            return GetDataRow(ATable, ASql, null);
        }

        private DataRow GetDataRow(string ATable, string ASql, Dictionary<string, object> AParams)
        {
            DataSet _ds = m_Database.Execute(ATable, ASql, AParams);
            DataRow _Result = null;

            try
            {
                _Result = _ds.Tables[0].Rows[0];
            }
            catch
            {

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

                if (null != m_Database)
                {
                    m_Database.Close();
                    m_Database.Dispose();
                }
                
                m_Database = null;
            }

            m_Disposed = true;
        }
        #endregion
    }
}
