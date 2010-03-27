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
        private String fileName;

        public KMLGenerator(String fileName)
        {
            this.fileName = fileName;
        }

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
            //Needed to generate KML, parameter is desired file name within KML file
            KMLGenerationLibrary kmlGenerator = new KMLGenerationLibrary(this.fileName);

            try
            {
                //Create database
                Database DB = new Database();
                DataTable mapping = DB.executeQueryLocal("SELECT tableName FROM Mapping WHERE connID=\'" + connID + "\'");

                //Create arraylist and add tables to it
                ArrayList tablesToBeSearched = new ArrayList();

                foreach (DataRow row in mapping.Rows)
                {
                    foreach (DataColumn col in mapping.Columns)
                    {
                        tablesToBeSearched.Add(row[col]);
                    }
                }

                DB.setConnInfo(ConnInfo.getConnInfo(connID));
                int dbType = ConnInfo.getConnInfo(connID).getDatabaseType();

                DataTable desc = DB.executeQueryLocal("SELECT description FROM Description WHERE connID=\'" + connID + "\'");

                //Sort through the table and format the description
                String descString = "";

                foreach (DataRow descRow in desc.Rows)
                {
                    foreach (DataColumn descCol in desc.Columns)
                    {
                        if (descRow[descCol] == "description")
                        {
                            descString = descRow[descCol].ToString();
                        }
                    }
                }

                //Create an array to store new description values
                ArrayList descArray = new ArrayList();

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

                        //Declare the lat and long holders
                        Double rowLat = 0, rowLon = 0;

                        //Check to see how many columns there are
                        if (map.getFormat() != Mapping.SEPARATE)
                        {
                            //Select the column value
                            String column = "";
                            foreach (DataColumn remoteColumn in remote.Columns)
                            {
                                if (remoteColumn.ColumnName == map.getLatFieldName())
                                {
                                    column = remoteRow[remoteColumn].ToString();
                                }
                            }

                            //Create the array to hold the coordinates
                            double[] coordinates;

                            //Separate the coordinates
                            //Order == Latitude First
                            if (map.getFormat() == Mapping.LATFIRST)
                            {
                                coordinates = map.separate(column, Mapping.LATFIRST);
                                rowLat = coordinates[0];
                                rowLon = coordinates[1];
                            }
                            else //Order == Longitude first
                            {
                                coordinates = map.separate(column, Mapping.LONGFIRST);
                                rowLon = coordinates[0];
                                rowLat = coordinates[1];
                            }
                        }
                        else//Two separate columns
                        {
                            //Get coordinates
                            foreach (DataColumn remoteColumn in remote.Columns)
                            {
                                if (remoteColumn.ColumnName == map.getLatFieldName())
                                {
                                    rowLat = Double.Parse(remoteRow[remoteColumn].ToString());
                                }
                                else if (remoteColumn.ColumnName == map.getLongFieldName())
                                {
                                    rowLon = Double.Parse(remoteRow[remoteColumn].ToString());
                                }
                            }//End for each
                        }//End else

                        //TODO: DO SOMETHING WITH COORDINATES
                        kmlGenerator.addPlacemark(tableName, rowDesc, rowLat, rowLon, "");
                        counter++;

                    }//End for each
                }//End for each
            }
            catch (ODBC2KMLException e)
            {
                throw e;
            }
            return kmlGenerator.finalizeKML();
        }//End function
    }
}
