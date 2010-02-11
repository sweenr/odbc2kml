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

//using HCI;

namespace HCI
{
    public class Database
    {
        //Datatypes
        //private OdbcConnection connection; 
        private ConnInfo connInfo;

        //Constructors
        
        //Default
        Database()
        {
            connInfo = null;
        }

        //Accepts a connection info class
        Database(ConnInfo connInfo)
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
            catch (OdbcException e) 
            {
                //Add Error Handler Code
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

            //Check the database type to determine the connection string

            //Database type = My SQL
            if (ConnInfo.MYSQL == connInfo.getDatabaseType())
            {
                //My SQL connection string
                string connectionString = "Driver={MySQL ODBC 5.1 Driver};Server="
                    + this.connInfo.getServerAddress() + ";Database="
                    + this.connInfo.getDatabaseName() + ";User=" + this.connInfo.getUserName()
                    + "; Password=" + this.connInfo.getPassword() + ";Option=3;";
            }//Database type = MS SQL
            else if (ConnInfo.MSSQL == connInfo.getDatabaseType())
            {
                //MS SQL connection string
                string connectionString = "Driver={SQL Native Client};Server="
                    + this.connInfo.getServerAddress() + ";Database="
                    + this.connInfo.getDatabaseName() + ";Uid=" + this.connInfo.getUserName()
                    + ";Pwd=" + this.connInfo.getPassword() + ";";
            }//Database type = oracle
            else if (ConnInfo.ORACLE == connInfo.getDatabaseType())
            {
                //My SQL connection string
                string connectionString = "Driver={Microsoft ODBC for Oracle};Server="
                    + this.connInfo.getServerAddress()
                    + ";Uid=" + this.connInfo.getUserName()
                    + ";Pwd=" + this.connInfo.getPassword() + ";";
            }
            else
            {
                //Pass an error to error handler signalling an improper database type
            }

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
            catch (OdbcException e)
            {
                //Add Error Handler Code
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
    }
}
