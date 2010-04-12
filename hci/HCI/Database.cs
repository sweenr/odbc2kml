using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Collections;

namespace HCI
{
    public class Database
    {
        //Datatypes
        //internal OdbcConnection connection; 
        internal ConnInfo connInfo;

        //Constructors
        
        //Default
        public Database()
        {
            connInfo = null;
        }

        //Accepts a connection info class
        public Database(ConnInfo connInfo)
        {
            this.connInfo = connInfo;
        }

        //Functions

        /// <summary>
        /// Database function that communicates with the local database. Does not need a connInfo
        /// object for this communication.
        /// </summary>
        /// <param name="query">String --> query to be executed</param>
        /// <returns>DataTable --> Return result set</returns>
        public DataTable executeQueryLocal(string query)
        {
            //Database connection string
            string connectionString = "Driver={SQL Native Client};Database=odbc2kml;Server=" 
                + Environment.MachineName + "\\sqlexpress;Trusted_Connection=yes;"
                + "Connection Timeout=5;";
            //Create the Odbc Connection
            OdbcConnection connection = new OdbcConnection(connectionString);
            connection.ConnectionTimeout = 5;

            // This is your data adapter that understands SQL databases:
            OdbcDataAdapter dataAdapter = new OdbcDataAdapter(query, connection);
            
            // This is your table to hold the result set:
            DataTable dataTable = new DataTable();

            try
            {
                //Open the connection to the database
                connection.Open();

                // Fill the data table with select statement's query results:
                int recordsAffected = dataAdapter.Fill(dataTable);

                //Close connection
                connection.Close();
            }
            catch (Exception ex)
            {
                throw new ODBC2KMLException(ex.Message);
            }
            finally
            {
                //Close the connection
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            //Return the data table to be operated on
            return dataTable;
        }

        /// <summary>
        /// Database function that communicates with the local database. Does not need a connInfo
        /// object for this communication.
        /// </summary>
        /// <param name="querys">ArrayList --> queries to be executed</param>
        /// <returns>DataTable --> Return result set</returns>
        public DataTable executeQueryLocal(ArrayList querys)
        {
            //Database connection string
            string connectionString = "Driver={SQL Native Client};Database=odbc2kml;Server="
                + Environment.MachineName + "\\sqlexpress;Trusted_Connection=yes;"
                + "Connection Timeout=5;";
            //Create the Odbc Connection
            OdbcConnection connection = new OdbcConnection(connectionString);
            connection.ConnectionTimeout = 5;

            // This is your table to hold the result set:
            DataTable dataTable = new DataTable();

            OdbcCommand command = new OdbcCommand();
            OdbcTransaction transaction = null;

            // Set the Connection to the new OdbcConnection.
            command.Connection = connection;

            try
            {
                //Open the connection to the database
                connection.Open();

                // Start a local transaction
                transaction = connection.BeginTransaction();

                // Assign transaction object for a pending local transaction.
                command.Connection = connection;
                command.Transaction = transaction;

                foreach (String query in querys)
                {
                    // Execute the commands.
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();

                //Close connection
                connection.Close();
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {

                }

                throw new ODBC2KMLException(ex.Message);
            }
            finally
            {
                //Close the connection
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            //Return the data table to be operated on
            return dataTable;
        }

        /// <summary>
        /// This database function communicates with Oracle, MySQL, and SQL remote servers.
        /// This functions requires the Database object to hold a ConnInfo object with information
        /// about the remote server.
        /// </summary>
        /// <param name="query">String --> query to be executed</param>
        /// <returns>DataTable --> Result set</returns>
        public DataTable executeQueryRemote(string query)
        {
            DataTable dataTable = new DataTable();
            string connectionString = "";

            //Check the database type to determine the connection string

            if (connInfo.getDatabaseType() == ConnInfo.MSSQL || connInfo.getDatabaseType() == ConnInfo.MYSQL)
            {
                connectionString = Database.getConnectionString(connInfo);
                
                //Create the Odbc Connection
                OdbcConnection connection = new OdbcConnection(connectionString);
                connection.ConnectionTimeout = 5;

                // This is your data adapter that understands SQL databases:
                OdbcDataAdapter dataAdapter = new OdbcDataAdapter(query, connection);

                // This is your table to hold the result set:

                try
                {
                    //Open the connection to the database
                    connection.Open();

                    // Fill the data table with select statement's query results:
                    int recordsAffected = dataAdapter.Fill(dataTable);

                    //Close connection
                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw new ODBC2KMLException(ex.Message);
                }
                finally
                {
                    //Close the connection
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }

            }
            //Database type = oracle
            else if (ConnInfo.ORACLE == connInfo.getDatabaseType())
            {
                connectionString = Database.getConnectionString(connInfo);
                
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    OracleCommand command = new OracleCommand(query);
                    command.Connection = connection;
                    try
                    {

                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        throw new ODBC2KMLException(ex.Message);
                    }

                    OracleDataReader reader = null;
                    try
                    {
                        reader = command.ExecuteReader();
                        dataTable.Load(reader);
                    }
                    catch (Exception ex)
                    {
                        throw new ODBC2KMLException(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                    }

                }
            }
            else
            {
                //Pass an error to error handler signalling an improper database type
            }

            

            //Return the data table to be operated on
            return dataTable;
        }

        /// <summary>
        /// Set the databases conninfo object.
        /// </summary>
        /// <param name="info">ConnInfo --> Remote database information</param>
        public void setConnInfo(ConnInfo info)
        {
            connInfo = info;
        }

        /// <summary>
        /// Returns a connection string based on the current ConnInfo object.
        /// </summary>
        /// <param name="info">ConnInfo --> current database information</param>
        /// <returns>String --> connection string</returns>
        public static String getConnectionString(ConnInfo info)
        {
            if (ConnInfo.MYSQL == info.getDatabaseType())
            {
                //My SQL connection string
                return "Driver={MySQL ODBC 5.1 Driver};Server="
                    + info.getServerAddress() + ";Port=" + info.portNumber + ";Database="
                    + info.getDatabaseName() + ";User=" + info.getUserName()
                    + "; Password=" + info.getPassword() + ";Option=3;";
            }//Database type = MS SQL
            else if (ConnInfo.MSSQL == info.getDatabaseType())
            {
                //MS SQL connection string
                return "Driver={SQL Native Client};Server="
                    + info.getServerAddress() + ";Port=" + info.portNumber + ";Database="
                    + info.getDatabaseName() + ";Uid=" + info.getUserName()
                    + ";Pwd=" + info.getPassword() + ";";
            }
            else if (ConnInfo.ORACLE == info.getDatabaseType())
            {
                string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS="
                    + "(PROTOCOL=" + info.getOracleProtocol() + ")(HOST=" + info.getServerAddress()
                    + ")(PORT=" + info.getPortNumber() + ")))(CONNECT_DATA=(SERVER=DEDICATED)";

                if (info.getOracleServiceName().Length != 0)
                {
                    connectionString += "(SERVICE_NAME="
                    + info.getOracleServiceName() + ")";
                }
                else if (info.getOracleSID().Length != 0)
                {
                    connectionString += "(SID="
                    + info.getOracleSID() + ")";
                }

                connectionString += "));User Id=" + info.getUserName() + ";Password="
                    + info.getPassword() + ";";

                //Oracle connection string
                return connectionString;
            }

            return "";
        }
    }
}
