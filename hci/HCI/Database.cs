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

namespace HCI
{
    public class Database
    {
        //Datatypes
        //private OdbcConnection connection; 
        private ConnInfo connInfo;

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

        /*********************************************************
         * executeQueryLocal is the database function that
         * communicates with the local database. 
         * 
         * Parameters: string query (query to be executed)
         * 
         * Return: DataTable(Result set)
         ********************************************************/
        public DataTable executeQueryLocal(string query)
        {
            //Database connection string
            string connectionString = "Driver={SQL Native Client};Database=odbc2kml;Server=" 
                + Environment.MachineName + "\\sqlexpress;Trusted_Connection=yes;";
            //Create the Odbc Connection
            OdbcConnection connection = new OdbcConnection(connectionString);

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
                throw new ODBC2KMLException("Database error");
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


        /*******************************************************
         * executeQueryRemote is used for retrieving info
         * from the databases connected to by ODBC2KML.
         * 
         * Parameters: ConnInfo connInfo (All info pertaining to the connection)
         *             string query (query to be executed)
         *             
         * Return:  (Result set)
         *******************************************************/
        public DataTable executeQueryRemote(string query)
        {
            DataTable dataTable = new DataTable();
            string connectionString = "";

            //Check the database type to determine the connection string

            if (connInfo.getDatabaseType() == ConnInfo.MSSQL || connInfo.getDatabaseType() == ConnInfo.MYSQL)
            {
                //Database type = My SQL
                if (ConnInfo.MYSQL == connInfo.getDatabaseType())
                {
                    //My SQL connection string
                    connectionString = "Driver={MySQL ODBC 5.1 Driver};Server="
                        + this.connInfo.getServerAddress() + ";Database="
                        + this.connInfo.getDatabaseName() + ";User=" + this.connInfo.getUserName()
                        + "; Password=" + this.connInfo.getPassword() + ";Option=3;";
                }//Database type = MS SQL
                else if (ConnInfo.MSSQL == connInfo.getDatabaseType())
                {
                    //MS SQL connection string
                    connectionString = "Driver={SQL Server Native Client 10.0};Server="
                        + this.connInfo.getServerAddress() + ";Database="
                        + this.connInfo.getDatabaseName() + ";Uid=" + this.connInfo.getUserName()
                        + ";Pwd=" + this.connInfo.getPassword() + ";";
                }

                //Create the Odbc Connection
                OdbcConnection connection = new OdbcConnection(connectionString);

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
                    throw new ODBC2KMLException("Database error");
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
                //My SQL connection string
                connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS="
                    + "(PROTOCOL=" + connInfo.getOracleProtocol() + ")(HOST=" + connInfo.getServerAddress()
                    + ")(PORT=" + connInfo.getPortNumber() + ")))(CONNECT_DATA=(SERVER=DEDICATED)";
                
                if(connInfo.getOracleServiceName().Length != 0)
                {
                    connectionString += "(SERVICE_NAME="
                    + connInfo.getOracleServiceName() + ")";
                }
                else if(connInfo.getOracleSID().Length != 0)
                {
                    connectionString += "(SID="
                    + connInfo.getOracleSID() + ")";
                }
                
                connectionString += "));User Id=" + connInfo.getUserName() + ";Password="
                    + connInfo.getPassword() + ";";

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    OracleCommand command = new OracleCommand(query);
                    command.Connection = connection;
                    connection.Open();

                    OracleDataReader reader = command.ExecuteReader();
                    try
                    {
                        dataTable.Load(reader);
                    }
                    catch (Exception ex)
                    {
                        throw new ODBC2KMLException("Database error");
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

        public void setConnInfo(ConnInfo info)
        {
            connInfo = info;
        }

        public DataTable getPrimaryKeys(string tableName)
        {
            int dbType = this.connInfo.getDatabaseType();
            DataTable remote = null;
            if(dbType == ConnInfo.MSSQL)
            {
                remote = this.executeQueryRemote("SELECT [name] "
                                                  + "FROM syscolumns "
                                                  + "WHERE [id] IN (SELECT [id] "
                                                                  + "FROM sysobjects "
                                                                  + "WHERE [name] = '" + tableName + "') "
                                                  + "AND colid IN (SELECT SIK.colid "
                                                                   + "FROM sysindexkeys SIK "
                                                                   + "JOIN sysobjects SO ON SIK.[id] = SO.[id] "
                                                                   + "WHERE SIK.indid = 1 "
                                                                   + "AND SO.[name] = '" + tableName + "')");
            }
            else if (dbType == ConnInfo.MYSQL)
            {
                remote = this.executeQueryRemote("SELECT k.column_name "
                                                + "FROM information_schema.table_constraints t "
                                                + "JOIN information_schema.key_column_usage k "
                                                + "USING(constraint_name,table_schema,table_name) "
                                                + "WHERE t.constraint_type='PRIMARY KEY' "
                                                  + "AND t.table_schema='" + this.connInfo.getDatabaseName() + "' "
                                                  + "AND t.table_name='" + tableName + "';");
            }
            else if (dbType == ConnInfo.ORACLE)
            {
                //remote = this.executeQueryRemote("SELECT COLUMN_NAME FROM ALL_CONS_COLUMNS A JOIN ALL_CONSTRAINTS C  ON A.CONSTRAINT_NAME = C.CONSTRAINT_NAME WHERE C.TABLE_NAME = 'LOCKLOCATIONS' AND C.CONSTRAINT_TYPE = 'P');
            }
            else
            {
                remote = null;
            }
            return remote;
        }
    }
}
