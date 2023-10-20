using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using ApplicationSentinel.Controllers;

namespace ApplicationSentinel
{
    public class SqlHelper : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlHelper"></see> class.
        /// </summary>
        public SqlHelper() {
        }
        #endregion

        #region Add Parameter TO Query
        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="command">The command.
        /// <param name="parameterName">Name of the parameter.
        /// <param name="dbType">Type of the db.
        /// <param name="size">The size.
        /// <param name="direction">The direction.
        /// <param name="precision">The precision.
        /// <param name="scale">The scale.
        /// <param name="sourceColumn">The source column.
        /// <param name="sourceVersion">The source version.
        /// <param name="value">The value.
        private void AddParameter(SqlCommand command, string parameterName, SqlDbType dbType, int size, ParameterDirection direction, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            SqlParameter p = new SqlParameter(parameterName, dbType, size, direction, true, precision, scale, sourceColumn,
                sourceVersion, value);
            command.Parameters.Add(p);
        }

        public static string GetDBConnectionString()
        {

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(ApplicationSecretsController._configuration.GetConnectionString("DefaultConnection"));

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("XMOPZRBLN5VC1NU3");
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }


            //}

        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="command">The command.
        /// <param name="parameterName">Name of the parameter.
        /// <param name="dbType">Type of the db.
        /// <param name="size">The size.
        /// <param name="direction">The direction.
        /// <param name="value">The value.
        public void AddParameter(SqlCommand command, string parameterName, SqlDbType dbType, int size, ParameterDirection direction, object value)
        {
            AddParameter(command, parameterName, dbType, size, direction, 0, 0, null, DataRowVersion.Current, value);
        }

        /// <summary>
        /// Adds the in parameter.
        /// </summary>
        /// <param name="command">The command.
        /// <param name="parameterName">Name of the parameter.
        /// <param name="dbType">Type of the db.
        /// <param name="value">The value.
        public void AddInParameter(SqlCommand command, string parameterName, SqlDbType dbType, object value)
        {
            AddParameter(command, parameterName, dbType, 0, ParameterDirection.Input, value);
        }

        /// <summary>
        /// Adds the out parameter.
        /// </summary>
        /// <param name="command">The command.
        /// <param name="parameterName">Name of the parameter.
        /// <param name="dbType">Type of the db.
        /// <param name="size">The size.
        public void AddOutParameter(SqlCommand command, string parameterName, SqlDbType dbType, int size)
        {
            AddParameter(command, parameterName, dbType, size, ParameterDirection.Output, null);
        }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="command">The command.
        /// <param name="parameterName">Name of the parameter.
        /// <returns></returns>
        public object GetParameterValue(SqlCommand command, string parameterName)
        {
            return command.Parameters[parameterName].Value;
        }
        #endregion


        #region Database Related Command

        public int ExecuteNonQuery(string sqlquery)
        {
            int returnCode = 0;

            using (SqlConnection connection = new SqlConnection(GlobalData.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlquery, connection);
                command.CommandType = CommandType.Text;
                returnCode = command.ExecuteNonQuery();
            }

            return returnCode;
        }

        public object ExecuteScalar(string sqlquery)
        {
            object returnCode = new object();

            using (SqlConnection connection = new SqlConnection(GlobalData.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlquery, connection);
                command.CommandType = CommandType.Text;
                returnCode = command.ExecuteScalar();
            }

            return returnCode;
        }

        public DataTable LoadDataTable(string sqlquery)
        {
            DataTable dt = new DataTable();



            using (SqlConnection connection = new SqlConnection(GlobalData.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlquery, connection);
                command.CommandType = CommandType.Text;

                using (SqlDataAdapter da = new SqlDataAdapter(command))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SqlHelper"></see> is reclaimed by garbage collection.
        /// </summary>
        ~SqlHelper()
        {
            Dispose();
        }
        #endregion

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}
