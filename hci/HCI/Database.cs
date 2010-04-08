﻿using System;
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
                connectionString = Database.getConnectionString(connInfo);
                
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

        public void setConnInfo(ConnInfo info)
        {
            connInfo = info;
        }

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
                return "Driver={SQL Native Client};Address="
                    + info.getServerAddress() + ";Port=" + info.portNumber + ";Database="
                    + info.getDatabaseName() + ";Uid=" + info.getUserName()
                    + ";Pwd=" + info.getPassword() + ";";
            }
            else if (ConnInfo.ORACLE == info.getDatabaseType())
            {
                //Oracle connection string
                return "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS="
                    + "(PROTOCOL=" + info.getOracleProtocol() + ")(HOST=" + info.getServerAddress()
                    + ")(PORT=" + info.getPortNumber() + ")))(CONNECT_DATA=(SERVER=DEDICATED)";
            }

            return "";
        }
    }
}
