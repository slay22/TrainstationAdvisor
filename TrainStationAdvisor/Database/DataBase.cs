using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.IO;
using System.Data;
using System.Reflection;

namespace TrainStationAdvisor
{
    public class Database : IDisposable
    {
        private string m_FileName = "TrainStationAdvisor.sdf";
        private string m_FilePath = "SD-MMC Card";
        private string m_Password = "metal22";
        private SqlCeEngine m_Engine;
        private SqlCeConnection m_Conn;
        private SqlCeCommand m_Cmd;
        private SqlCeTransaction m_Transaction;
        private string m_ConectionString;
        private bool m_Disposed;

        private bool IntegrityCheck { get; set; }

        public enum ParamType
        {
            BigInt = SqlDbType.BigInt,
            Binary = SqlDbType.Binary,
            Bit = SqlDbType.Bit,
            Char = SqlDbType.Char,
            DateTime = SqlDbType.DateTime,
            Decimal = SqlDbType.Decimal,
            Float = SqlDbType.Float,
            Image = SqlDbType.Image,
            Int = SqlDbType.Int,
            Money = SqlDbType.Money,
            NChar = SqlDbType.NChar,
            NText = SqlDbType.NText,
            NVarChar = SqlDbType.NVarChar,
            Real = SqlDbType.Real,
            UniqueIdentifier = SqlDbType.UniqueIdentifier,
            SmallDateTime = SqlDbType.SmallDateTime,
            SmallInt = SqlDbType.SmallInt,
            SmallMoney = SqlDbType.SmallMoney,
            Text = SqlDbType.Text,
            Timestamp = SqlDbType.Timestamp,
            TinyInt = SqlDbType.TinyInt,
            VarBinary = SqlDbType.VarBinary,
            VarChar = SqlDbType.VarChar,
            Variant = SqlDbType.Variant,
            Xml = SqlDbType.Xml,
            Udt = SqlDbType.Udt,
            Structured = SqlDbType.Structured,
            Date = SqlDbType.Date,
            Time = SqlDbType.Time,
            DateTime2 = SqlDbType.DateTime2,
            DateTimeOffset = SqlDbType.DateTimeOffset
        }

        public class ParamDefinition
        {
            public string ID { get; set; }
            public ParamType Type { get; set; }
            public object Value { get; set; }

            public ParamDefinition(string AID)
            {
                ID = AID;
            }

            public ParamDefinition(string AID, ParamType AType)
                : this(AID)
            {
                Type = AType;
            }

            public ParamDefinition(string AID, ParamType AType, object AValue)
                : this(AID, AType)
            {
                Value = AValue;
            }
        }

        public SqlCeConnection Connection
        {
            get
            {
                return m_Conn;
            }
        }

        private bool m_InmediateCommit = true;
        public bool InmediateCommit
        {
            get
            {
                return m_InmediateCommit;
            }
            set
            {
                m_InmediateCommit = value;
            }
        }

        public Database()
        {
            //Data Source=Mobile Device\SD-MMC Card\EnviDATA.sdf;Persist Security Info=True

            // Create new database
            try
            {
                m_FilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            }
            catch (Exception ex)
            {
                //Silent catch
                //ClassLibrary.Global.HandleError(null, ex);
                MessageBox.Show(ex.Message);
            }

            string _Path = string.Format("{0}\\{1}", m_FilePath, m_FileName);
            if (!File.Exists(_Path))
            {
                //check
            }

            m_ConectionString = string.Format("Data Source={0};Persist Security Info=True;Password ='{1}'", _Path, m_Password);

            m_Engine = new SqlCeEngine(m_ConectionString);

            Database.CheckIntegrity();
        }

        public Database(string connectionString)
            : this()
        {
            if (connectionString != string.Empty)
            {
                m_ConectionString = connectionString;
            }
        }

        public void Open()
        {
            // create connection
            if (null == m_Conn)
            {
                m_Conn = new SqlCeConnection(m_ConectionString);
            }
            m_Conn.Open();
        }

        public void OpenConnection()
        {
            Open();
        }

        public void BeginTransation()
        {
            m_Transaction = m_Conn.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void CommitTransaction()
        {
            CommitMode m_Mode = (InmediateCommit ? CommitMode.Immediate : CommitMode.Deferred);
            m_Transaction.Commit(m_Mode);
            DisposeTransaction();
        }

        public void Shrink()
        {
            m_Engine.Shrink();
        }

        public void RollbackTransaction()
        {
            m_Transaction.Rollback();
            DisposeTransaction();
        }

        private void DisposeTransaction()
        {
            m_Transaction.Dispose();
            m_Transaction = null;
        }

        ~Database()
        {
            Dispose(false);
        }

        public void Close()
        {
            Close(true);
        }

        public void Close(bool APermanent)
        {
            if (!m_Conn.State.Equals(ConnectionState.Closed))
            {
                if (null != m_Cmd)
                {
                    m_Cmd.Dispose();
                    m_Cmd = null;
                }

                m_Conn.Close();
            }

            if (APermanent)
            {
                m_Conn.Dispose();
                m_Conn = null;
            }
        }

        public bool IsOpen()
        {
            bool _Result = false;
            try
            {
                if (null != m_Conn)
                {
                    if (m_Conn.State.Equals(ConnectionState.Open))
                    {
                        _Result = true;
                    }
                }
            }
            catch
            {
                _Result = false;
            }

            return _Result;
        }

        public int ExecuteScalarInt(string sql)
        {
            return ExecuteScalarInt(sql, null);
        }

        public int ExecuteScalarInt(string sql, Dictionary<string, object> AParams)
        {
            int _Result;
            object _ScalarResult = ExecuteScalar(sql, AParams);

            if (_ScalarResult != DBNull.Value)
                _Result = Convert.ToInt32(_ScalarResult);
            else
                _Result = 0;

            return _Result;
        }

        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> AParams)
        {

            //            ClassLibrary.WaitCursor.Show(true); 

            object _Result = null;
            try
            {
                SqlCeCommand _Cmd = GetCommand();
                _Cmd.CommandText = sql;

                if (null != AParams)
                {
                    AddCommandParameters(_Cmd, AParams);
                }

                _Result = _Cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //if (!this.IntegrityCheck)
                //{
                //    ClassLibrary.Global.HandleError(null, ex);
                //}
                throw ex;
            }

            //            ClassLibrary.WaitCursor.Show(false); 

            return _Result;
        }
        /*
                public DataSet Execute(string tableName, string sql)
                {
        //            ClassLibrary.WaitCursor.Show(true);

                    DataSet _Result = new DataSet();
                    try
                    {
                        SqlCeDataAdapter _Adapter = GetAdapter(sql);
                        _Adapter.Fill(_Result, tableName);
                    }
                    catch (Exception ex)
                    {
                        ClassLibrary.Global.HandleError(null, ex);
                        throw ex;
                    }

        //            ClassLibrary.WaitCursor.Show(false);
            
                    return _Result;
                }
        */
        public DataSet Execute(string tableName, string sql)
        {
            return Execute(tableName, sql, null);
        }

        public DataSet Execute(string tableName, string sql, Dictionary<string, object> AParams)
        {
            //            ClassLibrary.WaitCursor.Show(true);
            DataSet _Result = new DataSet();
            try
            {
                SqlCeDataAdapter _Adapter = null;
                if (null == AParams)
                {
                    _Adapter = GetAdapter(sql);
                }
                else
                {
                    SqlCeCommand _Cmd = GetCommand();
                    _Cmd.CommandText = sql;

                    AddCommandParameters(_Cmd, AParams);

                    _Adapter = GetAdapter(_Cmd);
                }

                /*------------------------------------------------------------------------------------------
                 * TODO : Check here if next statement can be used, somehow without indexes the fill method 
                 * consumes too much time.
                 * SqlCeResultSet _ResultSet = _Cmd.ExecuteResultSet(ResultSetOptions.Scrollable);
                ------------------------------------------------------------------------------------------*/ 

                _Adapter.Fill(_Result, tableName);

                _Adapter.Dispose();
                _Adapter = null;
            }
            catch (Exception ex)
            {
                //if (!this.IntegrityCheck)
                //{
                //    ClassLibrary.Global.HandleError(null, ex);
                //}
                throw ex;
            }

            //            ClassLibrary.WaitCursor.Show(false);

            return _Result;
        }

        private static void AddCommandParameters(SqlCeCommand ACommand, Dictionary<string, object> AParams)
        {
            foreach (KeyValuePair<string, object> _Param in AParams)
            {
                SqlCeParameter _SqlParam = null;

                if (_Param.Value is Database.ParamDefinition)
                {
                    Database.ParamDefinition _ParamDef = (_Param.Value as Database.ParamDefinition);
                    _SqlParam = new SqlCeParameter(_ParamDef.ID, (SqlDbType)_ParamDef.Type);

                    if (_ParamDef.Type == ParamType.UniqueIdentifier)
                    {
                        _SqlParam.Value = new Guid(_ParamDef.Value.ToString());
                    }
                    else
                    {
                        _SqlParam.Value = _ParamDef.Value;
                    }
                }
                else
                {
                    _SqlParam = new SqlCeParameter(_Param.Key, _Param.Value);
                }

                ACommand.Parameters.Add(_SqlParam);
            }
        }

        public int ExecuteNonQuery(string sql)
        {
            int _Result = 0;
            try
            {
                SqlCeCommand _Cmd = GetCommand();
                _Cmd.CommandText = sql;
                _Result = _Cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                //if (!this.IntegrityCheck)
                //{
                //    ClassLibrary.Global.HandleError(null, ex);
                //}
                throw ex;
            }

            return _Result;
        }
        /*
                        SqlCeDataAdapter _Adapter = null;
                        if (null == AParams)
                        {
                            _Adapter = GetAdapter(sql);
                        }
                        else
                        {
                            SqlCeCommand _Cmd = this.GetCommand();
                            _Cmd.CommandText = sql;

                            this.AddCommandParameters(_Cmd, AParams);

                            _Adapter = GetAdapter(_Cmd);
                        }

          */

        public int ExecuteNonQuery(string sql, Dictionary<string, object> AParams)
        {
            int _Result = 0;
            try
            {
                if (null == AParams)
                {
                    _Result = ExecuteNonQuery(sql);
                }
                else
                {
                    SqlCeCommand _Cmd = GetCommand();
                    _Cmd.CommandText = sql;

                    AddCommandParameters(_Cmd, AParams);

                    _Result = _Cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //if (!this.IntegrityCheck)
                //{
                //    ClassLibrary.Global.HandleError(null, ex);
                //}
                throw ex;
            }

            return _Result;
        }

        // return data adapter
        private SqlCeDataAdapter GetAdapter(string sql)
        {
            if (m_Conn.State.Equals(ConnectionState.Closed))
            {
                m_Conn.Open();
            }

            return new SqlCeDataAdapter(sql, m_Conn);
        }

        private SqlCeDataAdapter GetAdapter(SqlCeCommand cmd)
        {
            if (m_Conn.State.Equals(ConnectionState.Closed))
            {
                m_Conn.Open();
            }

            return new SqlCeDataAdapter(cmd);
        }

        private SqlCeCommand GetCommand()
        {
            if (m_Conn.State.Equals(ConnectionState.Closed))
            {
                m_Conn.Open();
            }

            // create command object
            if (m_Cmd == null)
            {
                m_Cmd = m_Conn.CreateCommand();
                m_Cmd.CommandType = CommandType.Text;
            }

            if (null != m_Transaction)
            {
                m_Cmd.Transaction = m_Transaction;
            }

            m_Cmd.CommandText = String.Empty;
            m_Cmd.Parameters.Clear();

            return m_Cmd;
        }

        private static void CheckIntegrity()
        {
            //this.Open();

            //this.IntegrityCheck = true;

            //string sql = "SELECT joblist FROM mandanten";

            //try
            //{
            //    this.ExecuteScalarInt(sql);
            //}
            //catch (SqlCeException ex1)
            //{
            //    sql = "ALTER TABLE mandanten ADD joblist smallint NULL";
            //    this.ExecuteNonQuery(sql);
            //}

            //sql = "SELECT current_district FROM mandanten";
            //try
            //{
            //    this.ExecuteScalarInt(sql);
            //}
            //catch (SqlCeException ex2)
            //{
            //    sql = "ALTER TABLE mandanten ADD current_district nvarchar(35) NULL";
            //    this.ExecuteNonQuery(sql);
            //}

            //sql = "SELECT BIN_ADDRESS_RELATION_KEY FROM penalties";
            //try
            //{
            //    this.ExecuteScalarInt(sql);
            //}
            //catch (SqlCeException ex3)
            //{
            //    sql = "ALTER TABLE penalties ADD BIN_ADDRESS_RELATION_KEY numeric(10) NULL";
            //    this.ExecuteNonQuery(sql);
            //}

            //sql = "SELECT COMMENT FROM acknowledgements";
            //try
            //{
            //    this.ExecuteScalar(sql);
            //}
            //catch (SqlCeException ex3)
            //{
            //    sql = "ALTER TABLE acknowledgements ADD COMMENT nvarchar(1000) NULL";
            //    this.ExecuteNonQuery(sql);
            //}

            //this.IntegrityCheck = false;

            //this.Close();
        }
        /*
                public int ExecuteNonQuery(string query, bool keepConnOpen)
                {
                    int result = 0;
                    try
                    {
                        if (!conn.State.Equals(ConnectionState.Open))
                            conn.Open();

                        SqlCeCommand cmd = new SqlCeCommand(query, conn);
                        result = cmd.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {	// throw 1
                        throw ex;
                    }
                    finally
                    {
                        if (!keepConnOpen && !conn.State.Equals(ConnectionState.Closed))
                            conn.Close();
                    }

                    return result;
                }

                public int ExecuteNonQuery(SqlCeCommand cmd, bool keepConnOpen)
                {
                    int result = 0;
                    try
                    {
                        if (!conn.State.Equals(ConnectionState.Open))
                            conn.Open();

                        cmd.Connection = conn;
                        result = cmd.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {	// throw 2
                        throw ex;
                    }
                    finally
                    {
                        if (!keepConnOpen && !conn.State.Equals(ConnectionState.Closed))
                            conn.Close();
                    }

                    return result;

                }

                public int InsertEmptyRecord(string table)
                {
                    string query = "INSERT INTO " + table + " DEFAULT VALUES";
                    int result = 0;
                    try
                    {
                        if (!conn.State.Equals(ConnectionState.Open))
                            conn.Open();

                        SqlCeCommand cmd = new SqlCeCommand(query, conn);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT TOP 1 @@identity FROM " + table;
                        result = int.Parse(cmd.ExecuteScalar().ToString());
                    }
                    catch (Exception ex)
                    {	// throw 5
                        throw ex;

                    }
                    finally
                    {
                        if (!conn.State.Equals(ConnectionState.Closed))
                            conn.Close();
                    }

                    return result;

                }

                public SqlCeDataReader ExecuteReader(string query)
                {
                    try
                    {
                        if (!conn.State.Equals(ConnectionState.Open))
                            conn.Open();

                        SqlCeCommand cmd = new SqlCeCommand(query, conn);

                        return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    }
                    catch (Exception ex)
                    {
                        //throw ex;
                        return null;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                public DataSet GetDataSet(string query)
                {
                    DataSet ds = new DataSet();

                    try
                    {
                        if (!conn.State.Equals(ConnectionState.Open))
                            conn.Open();

                        SqlCeDataAdapter adapter = new SqlCeDataAdapter(query, conn);
                        adapter.Fill(ds);
                        adapter.Dispose();
                        adapter = null;

                    }
                    catch (Exception ex)
                    {// throw 7
                        throw ex;
                    }
                    finally
                    {
                        if (!conn.State.Equals(ConnectionState.Closed))
                            conn.Close();
                    }
                    return ds;
                }

                public DataSet GetDataSet(string query, string srcTable)
                {
                    DataSet ds = new DataSet();

                    try
                    {
                        if (!conn.State.Equals(ConnectionState.Open))
                            conn.Open();

                        SqlCeDataAdapter adapter = new SqlCeDataAdapter(query, conn);
                        adapter.Fill(ds, srcTable);
                        adapter.Dispose();
                        adapter = null;

                    }
                    catch (Exception ex)
                    {// throw 6
                        throw ex;
                    }
                    finally
                    {
                        if (!conn.State.Equals(ConnectionState.Closed))
                            conn.Close();
                    }
                    return ds;
                }

                public DataTable GetDataTable(string query, string srcTable)
                {
                    DataSet ds = new DataSet();

                    try
                    {
                        if (!conn.State.Equals(ConnectionState.Open))
                            conn.Open();

                        SqlCeDataAdapter adapter = new SqlCeDataAdapter(query, conn);
                        adapter.Fill(ds, srcTable);
                        adapter.Dispose();
                        adapter = null;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (!conn.State.Equals(ConnectionState.Closed))
                            conn.Close();
                    }
                    return ds.Tables[srcTable];
                }

                public DataSet GetDataSet(string query, string srcTable, DataSet ds)
                {
                    try
                    {
                        if (!conn.State.Equals(ConnectionState.Open))
                            conn.Open();

                        SqlCeDataAdapter adapter = new SqlCeDataAdapter(query, conn);
                        adapter.Fill(ds, srcTable);
                        adapter.Dispose();
                        adapter = null;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (!conn.State.Equals(ConnectionState.Closed))
                            conn.Close();
                    }
                    return ds;
                }


                public DataSet GetDataSet(SqlCeCommand cmd)
                {
                    DataSet ds = new DataSet();

                    try
                    {
                        if (!conn.State.Equals(ConnectionState.Open))
                            conn.Open();
                        cmd.Connection = conn;
                        SqlCeDataAdapter adapter = new SqlCeDataAdapter(cmd);
                        adapter.Fill(ds);
                        adapter.Dispose();
                        adapter = null;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (!conn.State.Equals(ConnectionState.Closed))
                            conn.Close();
                    }
                    return ds;
                }

                public DataSet GetDataSet(SqlCeCommand cmd, string srcTable)
                {
                    DataSet ds = new DataSet();

                    try
                    {
                        if (!conn.State.Equals(ConnectionState.Open))
                            conn.Open();
                        cmd.Connection = conn;
                        SqlCeDataAdapter adapter = new SqlCeDataAdapter(cmd);
                        adapter.Fill(ds, srcTable);
                        adapter.Dispose();
                        adapter = null;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (!conn.State.Equals(ConnectionState.Closed))
                            conn.Close();
                    }
                    return ds;
                }
        */

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

                try
                {
                    if (null != m_Cmd)
                    {
                        m_Cmd.Dispose();
                    }
                    m_Cmd = null;

                    if (null != m_Transaction)
                    {
                        m_Transaction.Dispose();
                    }

                    m_Transaction = null;

                    if (null != m_Conn)
                    {
                        m_Conn.Close();
                        m_Conn.Dispose();
                    }

                    m_Conn = null;

                    if (null != m_Engine)
                    {
                        //m_Engine.Shrink();

                        m_Engine.Dispose();
                        m_Engine = null;
                    }
                }
                catch
                {
                }
            }

            m_Disposed = true;
        }

        #endregion
    }
}
