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
    /// <summary>
    /// This class is designed to handle all manipulation and generation for KML associated tasks.
    /// This class is used when the users generates KML from the main page and from the web service.
    /// </summary>
    public class KMLGenerator
    {
        //Desired filename to appear within the KML file
        private String fileName;
        private String serverPath;

        /// <summary>
        /// Default constructor. Takes the desired filename for the kml file.
        /// </summary>
        /// <param name="fileName">String --> KML file name</param>
        public KMLGenerator(String fileName, String serverPath)
        {
            this.fileName = fileName;
            this.serverPath = serverPath;
        }

        /// <summary>
        /// This function takes a connection ID and generates all the associated KML for that connection.
        /// It works hand-in-hand with KMLGenerationLibrary, Styles, and Placemarks.
        /// It also calls generateKMLFromConnection.
        /// 
        /// There is also a helper class used to prevent duplicate styles (HashStyleComparer).
        /// </summary>
        /// <param name="connID">int --> connection ID for the connection that you wish to generate KML for</param>
        /// <returns>String --> A string that is the KML</returns>
        public string generateKML(int connID)
        {
            Connection connection = new Connection(connID);
            connection.populateFields();

            return generateKMLFromConnection(connection);
        }

        /// <summary>
        /// This function takes a connection object and generates all the associated KML for that connection.
        /// It works hand-in-hand with KMLGenerationLibrary, Styles, and Placemarks.
        /// 
        /// There is also a helper class used to prevent duplicate styles (HashStyleComparer).
        /// </summary>
        /// <param name="connection">Connection --> connection object for the connection that you wish to generate KML for</param>
        /// <returns>String --> A string that is the KML</returns>
        public string generateKMLFromConnection(Connection connection)
        {
            //Needed to generate KML, parameter is desired file name within KML file
            KMLGenerationLibrary kmlGenerator = new KMLGenerationLibrary(this.fileName);

            try
            {
                //Get mappings fromc onnection object
                ArrayList mappings = connection.getMapping();

                //Create array list to hold places
                ArrayList placemarks = new ArrayList();
                //Create hashset to hold unique styles, takes HashStyleComparer which is a helper class
                HashSet<Style> styles = new HashSet<Style>(new HashStyleComparer());

                //Create the Icon array and grabs the icons for the connection
                ArrayList icons = new ArrayList();
                icons = connection.getIcons();

                //Create the overlay array and grab the overlays for the connection
                ArrayList overlays = new ArrayList();
                overlays = connection.getOverlays();

                //Retrieve description string
                String descString = connection.getDescription().getDesc();

                //Create an array to store new description values
                ArrayList descArray = new ArrayList();

                //Create data table to pass to parser
                DataTable remote = null;

                //Create database
                Database DB = new Database(connection.getConnInfo());

                //originally took string table name, changed for test
                foreach (Mapping map in mappings)
                {
                    //Grab the tablename out of mapping
                    String tableName = map.getTableName();

                    if (connection.getConnInfo().getDatabaseType() == ConnInfo.MSSQL)
                    {
                        remote = DB.executeQueryRemote("SELECT * FROM " + tableName);
                    }
                    else if (connection.getConnInfo().getDatabaseType() == ConnInfo.MYSQL)
                    {
                        remote = DB.executeQueryRemote("SELECT * FROM " + tableName + ";");
                    }
                    else if (connection.getConnInfo().getDatabaseType() == ConnInfo.ORACLE)
                    {
                        remote = DB.executeQueryRemote("SELECT * FROM \"" + tableName + "\"");
                    }

                    //Parsed descriptions for rows
                    descArray = Description.parseDesc(remote, descString, tableName);

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
                                if (c.evaluateCondition(remoteRow, c, tableName))
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

                        //Long unsigned int, needed to properly interpret colors
                        UInt64 color = 0;
                        foreach (Overlay o in overlays)
                        {
                            foreach (Condition c in o.getConditions())
                            {
                                //See if the condition applies
                                if (c.evaluateCondition(remoteRow, c, tableName))
                                {
                                    if (color == 0)
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
                        Style rowStyle = new Style();
                        Placemark rowPlacemark;

                        //if there is an icon, create the name of the style based on the icon name and color
                        if (rowIcon.getLocation() != "")
                        {
                            if (rowIcon.getLocality() == false)
                            {
                                rowStyle = new Style(rowIcon, color, (rowIcon.getLocation() + "_" + color.ToString("X")));
                            }
                            else //If the icon is local, append server data
                            {
                                rowIcon.setLocation(this.serverPath + rowIcon.getLocation());
                                rowStyle = new Style(rowIcon, color, (rowIcon.getLocation() + "_" + color.ToString("X")));
                            }
                        }
                        else if (rowIcon.getLocation() == "" && color != 0) //Create the style name based on the color
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
                            rowPlacemark.setPlacemarkStyleName("#" + rowStyle.getStyleName());
                            styles.Add(rowStyle);
                        }
                        else
                        {
                            //Default value which won't add a style to this placemark in KML
                            rowPlacemark.setPlacemarkStyleName("");
                        }

                        //Increment counter for next row (associated with getting the row description)
                        counter++;

                        rowPlacemark = null;
                        rowIcon = null;



                    }//End for each
                }//End for each

                //Add each style to the KML
                foreach (Style s in styles)
                {
                    kmlGenerator.addStyle(s);
                }

                //Used to check if a look at has been added
                Boolean addLookAt = false;

                //Add each placemark to the KML
                foreach (Placemark p in placemarks)
                {
                    kmlGenerator.addPlacemark(p);
                    if (!addLookAt) //Add the first placemark as default lookat
                    {
                        kmlGenerator.addLookAt(p);
                        addLookAt = true;
                    }
                }
            }
            catch (ODBC2KMLException e) //If bad things happen pass it up to connection details
            {
                throw e;
            }

            //Return KML string
            return kmlGenerator.finalizeKML();
        }//End function
    }
}
