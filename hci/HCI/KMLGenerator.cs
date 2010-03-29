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
using System.Collections.Generic;

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
        //May not need the following data type, remove it if so
        //private DataTable kmlInformation;
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

                //Create array list to hold places and styles
                ArrayList placemarks = new ArrayList();
                HashSet<Style> styles = new HashSet<Style>();

                foreach (DataRow row in mapping.Rows)
                {
                    foreach (DataColumn col in mapping.Columns)
                    {
                        tablesToBeSearched.Add(row[col]);
                    }
                }

                DB.setConnInfo(ConnInfo.getConnInfo(connID));
                int dbType = ConnInfo.getConnInfo(connID).getDatabaseType();

                DataTable desc = DB.executeQueryLocal("SELECT description FROM Description WHERE connID=" + connID);
                
                //Create the Icon array and grabs the icons for the connection
                ArrayList icons = new ArrayList();
                icons = Icon.getIcons(connID);

                //Create the overlay array and grab the overlays for the connection
                ArrayList overlays = new ArrayList();
                overlays = Overlay.getOverlays(connID);

                //Sort through the table and format the description
                String descString = "";

                foreach (DataRow descRow in desc.Rows)
                {
                    descString = descRow["description"].ToString();
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
                    //For each row in the table!!!
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

                        //Row's icon
                        Icon rowIcon = new Icon();
                        rowIcon.setLocation("");

                        //For each icon until the first one found, compare the icons
                        //conditions against the given row
                        Boolean breakLoop = false;
                        foreach (Icon i in icons)
                        {
                            foreach (Condition c in i.getConditions())
                            {
                                //See if the condition applies to the given row
                                if(c.evaluateCondition(remoteRow, c, tableName))
                                {
                                    //Set temp icon to row icon and tell it to break out
                                    rowIcon = i;
                                    breakLoop = true;
                                }

                                //Grabbed the first icon, break out
                                if (breakLoop)
                                    break;
                            }//End inner for each

                            //Grabbed the first icon, break out
                            if (breakLoop)
                                break;
                        }//End outer for each

                        UInt64 color = 0;
                        foreach (Overlay o in overlays)
                        {
                            foreach (Condition c in o.getConditions())
                            {
                                //See if the condition applies
                                if(c.evaluateCondition(remoteRow, c, tableName))
                                {
                                    if(color == 0)
                                    {
                                        //Set the color to hex value
                                        color = 0xFF000000;
                                    }
                                    //Mix the colors, if multiple colors work
                                    color = color | (Convert.ToUInt64(o.getColor(), 16));
                                }
                            }//End inner for each
                        }//End outer for each

                        //Create Style and placemark for this coordinate set
                        Style rowStyle;
                        Placemark rowPlacemark;

                        if(rowIcon.getLocation() != "")
                        {
                            rowStyle = new Style(rowIcon, color, (rowIcon.getId() + "_" + color.ToString("X")));
                        }
                        else if (rowIcon.getLocation() == "" && color != 0)
                        {
                            rowStyle = new Style(rowIcon, color, color.ToString("X"));
                        }
                        else //If rowstyle is null, ignore it
                        {
                            rowStyle = null;
                        }

                        //Create placemark and add it to array list
                        rowPlacemark = new Placemark(rowLat, rowLon, rowDesc, "test");
                        placemarks.Add(rowPlacemark);

                        //If there is a row style, add it to the placemark and the array list
                        if (rowStyle != null)
                        {
                            rowPlacemark.setPlacemarkStyleName(rowStyle.getStyleName());
                            styles.Add(rowStyle);
                        }
                        else
                        {
                            //Default value which won't add a style to this placemark in KML
                            rowPlacemark.setPlacemarkStyleName("");
                        }

                        //Increment counter for next row (associated with getting the row description)
                        counter++;

                    }//End for each
                }//End for each

                //Add each style to the KML
                foreach (Style s in styles)
                {
                    kmlGenerator.addStyle(s);
                }

                //Add each placemark to the KML
                foreach (Placemark p in placemarks)
                {
                    kmlGenerator.addPlacemark(p);
                }
            }
            catch (ODBC2KMLException e)
            {
                throw e;
            }

            //Return KML string
            return kmlGenerator.finalizeKML();
        }//End function
    }
}
