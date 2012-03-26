using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Net.Configuration;
using System.Configuration;
namespace IBC.Database
{
    public class db : IDisposable
    {
        private readonly SqlConnection _sqlConn = new SqlConnection();
        public SqlDataReader Reader;
        private readonly SqlCommand _cmd = new SqlCommand();
        private bool _disposed = false;
        private int _rowsUpdated;

        /// <summary>
        /// Database Constructor
        /// </summary>
        /// <param name="conn">Takes in the database configuration name from web.config</param>
        public db(string conn)
        {
            _sqlConn.ConnectionString = ConfigurationManager.ConnectionStrings[conn].ConnectionString;
            _sqlConn.Open();
            _cmd.Connection = _sqlConn;
        }

        ~db()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed) return;
            if (disposing)
            {
                // Dispose managed resources.
                _sqlConn.Close();
                _sqlConn.Dispose();
                _cmd.Dispose();
            }
            _disposed = true;
        }

        public void SetSqlStoredProcedure()
        {
            _cmd.CommandType = System.Data.CommandType.StoredProcedure;
        }

        public void SetSqlText()
        {
            _cmd.CommandType = System.Data.CommandType.Text;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public SqlException ExecuteSql(string sqlStr)
        {
            try
            {
                _cmd.CommandText = sqlStr;
                _rowsUpdated = _cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                CloseConnection();
                return ex;
            }
            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public SqlException ExecuteSqlReader(string sqlStr)
        {
            try
            {
                _cmd.CommandText = sqlStr;
                Reader = _cmd.ExecuteReader();
            }
            catch (SqlException ex)
            {
                CloseConnection();
                return ex;
            }
            return null;
        }

        public void ParameterIdentity(string parameter)
        {
            var param = new SqlParameter
                            {
                                ParameterName = parameter,
                                Direction = ParameterDirection.Output,
                                SqlDbType = SqlDbType.Int
                            };
            _cmd.Parameters.Add(param);
        }

        public object ParameterValue(string parameter)
        {
            return _cmd.Parameters[parameter].Value;
        }

        public void ParameterAdd(string parameter, object value)
        {
            _cmd.Parameters.AddWithValue(parameter, value);
        }

        public void ParameterEdit(string parameter, object value)
        {
            _cmd.Parameters[parameter].Value = value;
        }

        public void ParameterClear(string parameter)
        {
            _cmd.Parameters.RemoveAt(parameter);
        }

        public void ParameterClearAll()
        {
            _cmd.Parameters.Clear();
        }

        public string ConnectionStatus()
        {
            return _sqlConn.State.ToString();
        }

        public void CloseConnection()
        {
            _sqlConn.Close();
        }

        public int getRowsUpdated()
        {
            return _rowsUpdated;
        }

    }
}
