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

namespace HCI
{
    public class Database
    {
        //Datatypes
        //private OdbcConnection connection; 

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
            //query = "SELECT * FROM Connection";

            //This is the database connection:
            string connectionString = "Initial Catalog=odbc2kml;Data Source=codylaptop\\sqlexpress;Integrated Security=SSPI;";
            SqlConnection connection = new SqlConnection(connectionString);

            // This is your data adapter that understands SQL databases:
            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
            
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

                
                  /*  foreach (DataRow dataRow in dataTable.Rows)

                    {

                        System.Console.WriteLine(dataRow[0]);

                    }*/
            }
            catch (SqlException e) 
            {
                //Add Error Handler Code

                /*string msg = "";
                for (int i=0; i < e.Errors.Count; i++)
                {
                    msg += "Error #" + i + " Message: " + e.Errors[i].Message + "\n";
                }
                System.Console.WriteLine(msg);*/
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
        public DataTable executeQueryRemote(ConnInfo connInfo, string query)
        {
            DataTable dataTable = new DataTable();

            return dataTable;
        }
    }
}
