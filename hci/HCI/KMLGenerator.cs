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
using System.Collections;

namespace HCI
{
    /****************************************************************************************************
     * 
     * 
     * 
     * 
     ****************************************************************************************************/

    public class KMLGenerator
    {
        private DataTable kmlInformation;

        /*
         * This functions pulls all of the required information about a connection from the local
         * database and transforms it into a KML file. It then stores that file on the server in a 
         * temporary location. 
         * 
         * return kml (returns a string associated with the file name and location)
         * 
         */ 
        public string generateKML(int connID)
        {
            string kml = "test";

            return kml;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connID"></param>
        /// <returns></returns>
        /// Grab table names and descriptions
        /// lattitude, longitude, description, icons, overlays
        public DataTable generateInfoForDesc(int connID)
        {
            //Create database
            Database DB = new Database();
            DataTable mapping = DB.executeQueryLocal("SELECT 'tableName' FROM Mapping WHERE connID=\'" + connID + "\'");
            
            //Create arraylist and add tables to it
            ArrayList tablesToBeSearched = null;
            
            foreach(DataRow row in mapping.Rows)
            {
                foreach(DataColumn col in mapping.Columns)
                {
                    tablesToBeSearched.Add(row[col].ToString());
                }
            }
            
            DB.setConnInfo(ConnInfo.getConnInfo(connID));
            int dbType = ConnInfo.getConnInfo(connID).getDatabaseType();
            
            DataTable desc = DB.executeQueryLocal("SELECT 'description' FROM Description WHERE connID=\'" + connID + "\'");
            
            //Format the description
            String descString = desc.ToString();
            
            //Create an array to store new description values
            ArrayList descArray = null;
           
            //Create data table to pass to parser
            DataTable remote = null;

            foreach (String tableName in tablesToBeSearched)
            { 
                if (dbType == ConnInfo.MSSQL)
                {
                    remote = DB.executeQueryRemote("SELECT * FROM " + tableName);
                }
                else if (dbType == ConnInfo.MYSQL)
                {
                    remote = DB.executeQueryRemote("SELECT * FROM " + tableName + ";");
                }
                else if (dbType == ConnInfo.ORACLE)
                {
                    remote = DB.executeQueryRemote("SELECT * FROM \"" + tableName + "\"");
                }

                //Parsed descriptions for rows
                descArray = Description.parseDesc(remote, descString, tableName);

                //Create mapping and populate it.
                Mapping map = Mapping.getMapping(connID, tableName); ;

                int counter = 0;
                foreach (DataRow remoteRow in remote.Rows)
                {
                    //Foreach row set the description for each row
                    String rowDesc = descArray[counter].ToString();
                    /*foreach (DataColumn remoteCol in remote.Columns)
                    {
                        if(remoteCol.col) 
                        {
                        }
                    }*/
                }


            }
        }
    }
}
