using System;
using MySql.Data.MySqlClient;
using Microsoft.Data.SqlClient;
using System.Data.Odbc;
using System.Data;

namespace LSSyncApp
{
    public class Connections
    {
        /***************** SqlServer ********************/
        private SqlConnection dbConn;
        private SqlCommand dbQuery;

        /******************* Sybase *********************/
        private OdbcConnection srcDbConn;
        private OdbcCommand command;

        /******************* MySql **********************/
        private MySqlConnection mySqlDbConn;
        private MySqlCommand mySqlCommand;

        /****************** SAP Hana ********************/
        private ADODB.Connection SAPConn = new ADODB.Connection();
        private ADODB.Recordset SAPCommand = new ADODB.Recordset();
        //private ADODB.Command SAPCommand = new ADODB.Command();
        private int ExecuteTimeOut = 7200;


        /////////////////////////////////////////////////////////////////////////
        //DB Connection Configuration
        /////////////////////////////////////////////////////////////////////////
        /***************** SqlServer Connectivity Functions ********************/
        public string destConnSetup(string serverName, string dbName, string uid, string pwd, int aiAuthMode)
        {
            try
            
            {
                if(aiAuthMode == 0)             //Service Principal
                {
                    SqlAuthenticationProvider.SetProvider(SqlAuthenticationMethod.ActiveDirectoryServicePrincipal,new ActiveDirectoryAuthenticationProvider());
                    dbConn = new SqlConnection($"Server={serverName}; Authentication=Active Directory Service Principal; Encrypt=True; Database={dbName}; User Id={uid}; Password={pwd};TrustServerCertificate=False;");
                }
                else if(aiAuthMode == 1)        //User Id & Password
                {
                    dbConn = new SqlConnection($"Server={serverName};Database={dbName};user id={uid};password={pwd};Trusted_Connection=False;TrustServerCertificate=True;");
                }
                else if(aiAuthMode == 2)        //Windows Authentication
                {
                    dbConn = new SqlConnection($"Server={serverName}; Database={dbName}; Integrated Security=True;");
                }
                dbQuery = new SqlCommand
                {
                    Connection = dbConn
                };
                dbConn.Open();
                destDBClose();

                return "Success";
            }
            catch(Exception Ex)
            {
                return "Failed - " + Ex.Message;
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Execute the Query in Database
        //////////////////////////////////////////////////////////////////////////////
        public string destExecQuery(string sqlQuery, int aiBulkDel = 0)
        {
            try
            {
                string lsQuery = sqlQuery;
                if (dbConn.State == ConnectionState.Open)
                {
                    destDBClose();
                }

                if (aiBulkDel == 1) lsQuery = $"EXECUTE sp_executesql N'{sqlQuery.Replace("'","''")}'";

                dbQuery.CommandText = lsQuery;
                dbQuery.CommandTimeout = ExecuteTimeOut;
                
                dbConn.Open();
                dbQuery.ExecuteNonQuery();
                destDBClose();
                return "";
            }
            catch (Exception e)
            {
                destDBClose();
                return e.Message + " -- |Failed to Execute Query";
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Change DataBase
        //////////////////////////////////////////////////////////////////////////////
        public string changeDB(string dbName)
        {
            try
            {
                if (dbConn.State == ConnectionState.Open)
                {
                    destDBClose();
                }
                dbConn.Open();
                dbConn.ChangeDatabase(dbName);
                destDBClose();
                return "Success";
            }
            catch(Exception)
            {
                return "Failed";
            }
        }
        //////////////////////////////////////////////////////////////////////////////
        //Executes the Scalar Query and and fetch single value from Database.
        //////////////////////////////////////////////////////////////////////////////
        public string destDBExecRetOne(string sqlQuery)
        {
            try
            {
                if (dbConn.State == ConnectionState.Open)
                {
                    destDBClose();
                }
                dbQuery.CommandText = sqlQuery;
                dbQuery.CommandTimeout = ExecuteTimeOut;
                dbConn.Open();
                object retVal = dbQuery.ExecuteScalar();
                destDBClose();
                if (retVal != null)
                {
                    return retVal.ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                destDBClose();
                return e.Message + " -- |Failed to Execute Query One Return";
            }
        }
        //////////////////////////////////////////////////////////////////////////////
        //Close DB.
        //////////////////////////////////////////////////////////////////////////////
        public void destDBClose()
        {
            if (dbConn.State == ConnectionState.Open)
            {
                dbConn.Close();
            }
        }


        //////////////////////////////////////////////////////////////////////////////
        //Executes the Query and and fetch multiple values from Database.
        //////////////////////////////////////////////////////////////////////////////
        public SqlDataReader destDBExecRetMultiple(string sqlQuery, out string asStatus)
        {
            try
            {
                
                if (dbConn.State == ConnectionState.Open)
                {
                    destDBClose();
                }
                dbQuery.CommandText = sqlQuery;
                dbQuery.CommandTimeout = ExecuteTimeOut;
                dbConn.Open();
                SqlDataReader retVal = dbQuery.ExecuteReader();
                asStatus = (retVal != null ? "Success" : "No Rows");
                if (retVal != null)
                {
                    return retVal;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception Ex)
            {
                destDBClose();
                asStatus = $"Failed - {Ex.Message}";
                return null;
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Insert the data to Database from DataTable.
        //////////////////////////////////////////////////////////////////////////////
        public string destDBInsertBulk(string tableName, DataTable dt)
        {
            try
            {

                SqlBulkCopy bulkCopy = new SqlBulkCopy(dbConn)
                {
                    DestinationTableName = tableName,
                    BulkCopyTimeout = ExecuteTimeOut

                };

                if (dbConn.State == ConnectionState.Open)
                {
                    destDBClose();
                }
                dbConn.Open();
                bulkCopy.WriteToServer(dt);
                destDBClose();
                return "";
            }
            catch (Exception Ex)
            {
                return "Failed - " + Ex.Message;
            }
        }

        public string destDBInsertBulkCred(string tableName, DataTable dt, string serverName, string dbName, string uid, string pwd, int aiAuthMode)
        {
            try
            {
                destConnSetup(serverName, dbName, uid, pwd, aiAuthMode);
                string lsConnStr = "";
                if (aiAuthMode == 0)             //Service Principal
                {
                    lsConnStr = $"Server={serverName}; Authentication=Active Directory Service Principal; Encrypt=True; Database={dbName}; User Id={uid}; Password={pwd}";
                }
                else if (aiAuthMode == 1)        //User Id & Password
                {
                    lsConnStr = $"Server={serverName};Database={dbName};user id={uid};password={pwd};Trusted_Connection=False;TrustServerCertificate=True;";
                }
                else if (aiAuthMode == 2)        //Windows Authentication
                {
                    lsConnStr = $"Server={serverName}; Database={dbName}; Integrated Security=True;";
                }
                SqlBulkCopy bulkCopy = new SqlBulkCopy(lsConnStr, SqlBulkCopyOptions.KeepNulls & SqlBulkCopyOptions.KeepIdentity)
                {
                    DestinationTableName = tableName
                };
                if (dbConn.State == ConnectionState.Open)
                {
                    destDBClose();
                }
                dbConn.Open();
                bulkCopy.WriteToServer(dt);
                destDBClose();
                return "";
            }
            catch (Exception Ex)
            {
                return "Failed - " + Ex.Message;
            }
        }
        /**************** SqlServer Connectivity Functions End *****************/
        /*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*/

        /**************** Sybase ODBC Connectivity Functions *******************/
        public string srcDBConn(string dsn, string uid, string pwd)
        {
            try
            {
                srcDbConn = new OdbcConnection($"dsn={dsn};uid={uid};pwd={pwd}");
                command = new OdbcCommand
                {
                    Connection = srcDbConn
                };
                srcDbConn.Open();
                srcDbConn.Close();
            }
            catch (Exception E)
            {
                return "Failed | " + E.Message;
            }
            return "Success";
        }


        public string srcDBExecQueryRetOne(string sqlQuery, string dsn, string uid, string pwd)
        {
            try
            {
                srcDbConn = new OdbcConnection($"dsn={dsn};uid={uid};pwd={pwd}");
                command = new OdbcCommand(sqlQuery, srcDbConn)
                {
                    CommandTimeout = ExecuteTimeOut
                };

                srcDbConn.Open();
                string str = command.ExecuteScalar().ToString();
                srcDbConn.Close();
                return str;
            }
            catch (Exception E)
            {
                return "Failed | " + E.Message;
            }
        }
        public OdbcDataReader srcDBExecRetMultiple(string sqlQuery, ODBCSyncParam od = null)
        {
            try
            {

                command.CommandText = sqlQuery;
                command.CommandTimeout = ExecuteTimeOut;
                srcDbConn.Open();
                OdbcDataReader retVal = command.ExecuteReader();
                if (retVal != null)
                {
                    return retVal;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception Ex)
            {
                if (od != null)
                    od.setStatusLog("status", Ex.Message, 1);
                srcDbConn.Close();
                return null;
            }
        }

        public void SrcDBClose()
        {
            try
            {
                srcDbConn.Close();
            }
            catch
            {

            }
        }

        /************** Sybase ODBC Connectivity Functions End *****************/
        /*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*/
        /***************** MySQL ODBC Connectivity Functions *******************/
        public string mySqlDBConn(string host, string port, string db, string uid, string pwd)
        {
            try
            {
                mySqlDbConn = new MySqlConnection($"Server={host};Port={port};Database={db};Uid={uid};Pwd={pwd};");
                mySqlCommand = new MySqlCommand
                {
                    Connection = mySqlDbConn
                };

                mySqlDbConn.Open();
                mySqlDbConn.Close();
            }
            catch (Exception E)
            {
                return "Failed | " + E.Message;
            }
            return "Success";
        }

        public string mySqlExecQueryRetOne(string sqlQuery, string host, string port, string db, string uid, string pwd)
        {
            try
            {
                mySqlDbConn = new MySqlConnection($"Server={host};Port={port};Database={db};Uid={uid};Pwd={pwd};");
                mySqlCommand = new MySqlCommand(sqlQuery, mySqlDbConn)
                {
                    CommandTimeout = ExecuteTimeOut
                };

                mySqlDbConn.Open();
                string str = mySqlCommand.ExecuteScalar().ToString();
                mySqlDbConn.Close();
                return str;
            }
            catch (Exception E)
            {
                return "Failed | " + E.Message;
            }
        }

        public MySqlDataReader mySqlExecRetMultiple(string sqlQuery)
        {
            try
            {
                mySqlCommand.CommandText = sqlQuery;
                mySqlCommand.CommandTimeout = ExecuteTimeOut;
                mySqlDbConn.Open();
                MySqlDataReader retVal = mySqlCommand.ExecuteReader();
                if (retVal != null)
                {
                    return retVal;
                }
                else
                {
                    return null;
                }
            }
            catch 
            {
                mySqlDbConn.Close();
                return null;
            }
        }

        public void mySqlDBClose()
        {
            try
            {
                mySqlDbConn.Close();
            }
            catch
            {

            }
        }
        /*************** MySQL ODBC Connectivity Functions End *****************/


        /**************** SAP Hana ODBC Connectivity Functions *****************/
        public string sapHanaDBConn(string dsn, string host, string port, string uid, string pwd, string dbName, string currentSchema)
        {
            try
            {
                string connectionString;
                //connectionString = "DSN=HANADBPRO; SERVERNODE=192.168.2.120:30013; UID=leapsurge; PWD=Leap@1999; DATABASENAME=HDP;CURRENTSCHEMA=SAPHANADB";
                connectionString = $"DSN={dsn}; SERVERNODE={host}:{port}; UID={uid}; PWD={pwd}; DATABASENAME={dbName};CURRENTSCHEMA={currentSchema}";
                SAPConn.ConnectionString = connectionString;
                SAPConn.Open();
                SAPConn.Close();
            }
            catch (Exception E)
            {
                return "Failed | " + E.Message;
            }
            return "Success";
        }


        public ADODB.Recordset sapHanaExecRetMultiple(string sqlQuery, string dsn, string host, string port, string uid, string pwd,
            string dbName, string currentSchema, out string asReturn)
        {
            asReturn = "Failed";
            try
            {
                string connectionString;
                ADODB.Recordset Recs = new ADODB.Recordset();
                //connectionString = "DSN=HANADBPRO; SERVERNODE=192.168.2.120:30013; UID=leapsurge; PWD=Leap@1999; DATABASENAME=HDP;CURRENTSCHEMA=SAPHANADB";
                connectionString = $"DSN={dsn}; SERVERNODE={host}:{port}; UID={uid}; PWD={pwd}; DATABASENAME={dbName};CURRENTSCHEMA={currentSchema}";

                SAPConn.ConnectionString = connectionString;
                SAPConn.CommandTimeout = ExecuteTimeOut;
                SAPConn.Open();
                
                Recs = SAPConn.Execute(sqlQuery, out _);
                //SAPConn.Close();
                asReturn = "Success";
                return Recs;

            }
            catch (Exception E)
            {
                SAPConn.Close();
                asReturn = E.Message;
                return null;
            }
        }

        public void sapHanaDBClose()
        {
            try
            {
                SAPConn.Close();
            }
            catch
            {

            }
        }
        /************** SAP Hana ODBC Connectivity Functions End ***************/
    }
}
