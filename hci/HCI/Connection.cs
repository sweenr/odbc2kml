using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using HCI;

namespace HCI
{
    public class Connection
    {
        //Datatypes
        private Description description;
        private Mapping mapping;
        private ArrayList icons;
        private ArrayList overlays;
        private ConnInfo connInfo;
        private int connID;

        //Functions

        //Constructor
        Connection()
        {
            icons = new ArrayList();
            overlays = new ArrayList();
            description = new Description();
            mapping = new Mapping();
            connInfo = new ConnInfo();
        }

        Connection(int connID)
        {
            this.connID = connID;
        }

        //Getters

        //Retrieve description
        public Description getDescription()
        {
            return this.description;
        }

        //Retrieve mapping
        public Mapping getMapping()
        {
            return this.mapping;
        }

        //Retrieve icons
        public ArrayList getIcons()
        {
            return this.icons;
        }

        //Retrieve overlays
        public ArrayList getOverlays()
        {
            return this.overlays;
        }

        //Retrieve connInfo
        public ConnInfo getConnInfo()
        {
            return this.connInfo;
        }

        //Setters

        //Set description
        public void setDescription(Description description)
        {
            this.description = description;
        }

        //Set mapping
        public void setMapping(Mapping mapping)
        {
            this.mapping = mapping;
        }

        //Set icons
        public void setIcons(Icon icon)
        {
            this.icons.Add(icon);
        }

        //Set overlays
        public void setOverlays(Overlay overlay)
        {
            this.overlays.Add(overlay);
        }

        //Retrieve connInfo
        public void setConnInfo(ConnInfo connInfo)
        {
            this.connInfo = connInfo;
        }

        //Add Comments
        public bool isValid()
        {
            bool valid = false;

            return valid;
        }

        //Additional

        //Add Comments
        public bool openConn()
        {
            bool open = false;

            return open;
        }

        //TODO: ADD PARAMETERS TO ALL OF THE FOLLOWING

        //Add Comments
        public void closeConn() 
        {
        }

        //Add Comments
        public void saveConn()
        {
        }

        //Add Comments
        public void deleteConn()
        {
        }

        /*
         * Populate fields uses the connID passed into the constructor
         * and retrieves all of the information about that specific connection
         * from the database. It will then set all of the classes variables
         * based on this information. 
         */
        public void populateFields()
        {
            //Create an instance of Database to handle local queries
            Database localDatabase = new Database();

            //Create the DataTable to capture all of the information
            DataTable table = new DataTable();

            try
            {
                //Construct the connInfo query and retrieve the DataTable
                string query = "SELECT * FROM Connection WHERE ID=" + this.connID;
                table = localDatabase.executeQueryLocal(query);

                //Cycle through each row and column
                foreach(DataRow row in table.Rows)
                {
                   
                    foreach(DataColumn col in table.Columns)
                    {
                        //Set all connInfo
                        switch(col.ColumnName) 
                        {
                            case "name":
                                this.connInfo.setConnectionName(row[col].ToString());
                                break;
                            case "dbName":
                                this.connInfo.setDatabaseName(row[col].ToString());
                                break;
                            case "userName":
                                this.connInfo.setUserName(row[col].ToString());
                                break;
                            case "password":
                                this.connInfo.setPassword(row[col].ToString());
                                break;
                            case "port":
                                this.connInfo.setPortNumber(row[col].ToString());
                                break;
                            case "address":
                                this.connInfo.setServerAddress(row[col].ToString());
                                break;
                            case "type":
                                this.connInfo.setDatabaseType((int)row[col]);
                                break;
                            case "protocol":
                                this.connInfo.setOracleProtocol(row[col].ToString());
                                break;
                            case "serviceName":
                                this.connInfo.setOracleServiceName(row[col].ToString());
                                break;
                            case "SID":
                                this.connInfo.setOracleSID(row[col].ToString());
                                break;
                            default:
                                break;
                        }
                    }
                }//End outer loop

                //Clear the table
                table.Clear();

                //Create overlay query and populate table
                query = "SELECT * FROM Overlay WHERE connID=" + this.connID;
                table = localDatabase.executeQueryLocal(query);

                foreach(DataRow row in table.Rows)
                {
                    Overlay newOverlay = new Overlay();
                    foreach(DataColumn col in table.Columns)
                    {
                        if (col.ColumnName == "ID") //Branch off and get the conditions
                        {
                            //Create the new table for another query
                            DataTable newTable = new DataTable();

                            //Query string and query
                            string conQuery = "SELECT * FROM OverlayCondition WHERE overlayID="
                                + ((int)row[col]) + " AND connID=" + this.connID;
                            newTable = localDatabase.executeQueryLocal(conQuery);

                            //Cycle through each condition
                            foreach (DataRow nRow in newTable.Rows)
                            {
                                //Create the condition and add its values
                                Condition condition = new Condition();
                                
                                foreach (DataColumn nCol in newTable.Columns)
                                { 
                                    switch (nCol.ColumnName)
                                    {
                                        case "lowerBound":
                                            condition.setLowerBound(nRow[nCol].ToString());
                                            break;
                                        case "upperBound":
                                            condition.setUpperBound(nRow[nCol].ToString());
                                            break;
                                        case "lowerOperator":
                                            condition.setLowerOperator((int)nRow[nCol]);
                                            break;
                                        case "upperOperator":
                                            condition.setUpperOperator((int)nRow[nCol]);
                                            break;
                                        case "fieldName":
                                            condition.setFieldName(nRow[nCol].ToString());
                                            break;
                                        case "tableName":
                                            condition.setTableName(nRow[nCol].ToString());
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                //Add the condition to the overlay array
                                newOverlay.setConditions(condition);
                                //Free up condition memory
                                condition = null;
                            }//End outer loop
                            //Free up table memory
                            newTable = null;
                        }
                        else if(col.ColumnName == "color") //Set the color
                        {
                            newOverlay.setColor(row[col].ToString());
                        }
                    }
                    //Add the overlay to the list of overlays
                    this.setOverlays(newOverlay);
                    newOverlay = null;
                }//End outer loop

                //Clear table
                table.Clear();

                //Create description query and populate table
                query = "SELECT * FROM Description WHERE connID=" + this.connID;
                table = localDatabase.executeQueryLocal(query);

                foreach(DataRow row in table.Rows)
                {
                    foreach(DataColumn col in table.Columns)
                    {
                        //Set Description
                        if (col.ColumnName == "description")
                        {
                            description.setDesc(row[col].ToString());
                        }
                    }
                }//End outer loop

                //Clear table
                table.Clear();

                //Create mapping query and populate table
                query = "SELECT * FROM Mapping WHERE connID=" + this.connID;
                table = localDatabase.executeQueryLocal(query);

                foreach(DataRow row in table.Rows)
                {
                    foreach(DataColumn col in table.Columns)
                    {
                        //Set mapping
                        switch(col.ColumnName)
                        {
                            case "tableName":
                                mapping.setTableName(row[col].ToString());
                                break;
                            case "latFieldName":
                                mapping.setLatFieldName(row[col].ToString());
                                break;
                            case "longFieldName":
                                mapping.setLongFieldName(row[col].ToString());
                                break;
                            case "format":
                                mapping.setFormat((int)row[col]);
                                break;
                        }
                    }
                }//End outer loop

                //Clear table
                table.Clear();

                //Create icon query and populate table
                query = "SELECT * FROM Icon WHERE connID=" + this.connID;
                table = localDatabase.executeQueryLocal(query);

                foreach(DataRow row in table.Rows)
                {
                    //Create a new icon
                    Icon newIcon = new Icon();

                    foreach(DataColumn col in table.Columns)
                    {
                        if (col.ColumnName == "ID")
                        {
                            //Create a new table to perform subqueries on
                            DataTable newTable = new DataTable();
                            
                            //IconLibrary query
                            string locQuery = "SELECT * FROM IconLibrary WHERE ID=" + ((int)row[col]);
                            newTable = localDatabase.executeQueryLocal(locQuery);

                            foreach (DataRow nRow in newTable.Rows)
                            {
                                foreach (DataColumn nCol in newTable.Columns)
                                {
                                    //Set the location of the icon
                                    if (nCol.ColumnName == "location")
                                    {
                                        newIcon.setLocation(nRow[nCol].ToString());
                                    }
                                }
                            }//End outer loop

                            newTable.Clear();

                            //IconCondition query
                            string conQuery = "SELECT * FROM IconCondition WHERE iconID="
                                + ((int)row[col]) + " AND connID=" + this.connID;
                            newTable = localDatabase.executeQueryLocal(conQuery);

                            //Cycle through each condition
                            foreach (DataRow nRow in newTable.Rows)
                            {
                                //Create the condition and add its values
                                Condition condition = new Condition();

                                foreach (DataColumn nCol in newTable.Columns)
                                {
                                    switch (nCol.ColumnName)
                                    {
                                        case "lowerBound":
                                            condition.setLowerBound(nRow[nCol].ToString());
                                            break;
                                        case "upperBound":
                                            condition.setUpperBound(nRow[nCol].ToString());
                                            break;
                                        case "lowerOperator":
                                            condition.setLowerOperator((int)nRow[nCol]);
                                            break;
                                        case "upperOperator":
                                            condition.setUpperOperator((int)nRow[nCol]);
                                            break;
                                        case "fieldName":
                                            condition.setFieldName(nRow[nCol].ToString());
                                            break;
                                        case "tableName":
                                            condition.setTableName(nRow[nCol].ToString());
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                //Add the condition to the icon array
                                newIcon.setConditions(condition);
                                //Free up condition memory
                                condition = null;
                            }//End outer loop
                            //Free up table memory
                            newTable = null;
                        }
                    }
                    this.setIcons(newIcon);
                    //Free up icon memory
                    newIcon = null;

                }//End outer loop

                //Clear table
                table.Clear();
            }
            catch(Exception e) //Add whatever exceptions are needed and error handling code
            {

            }
            finally
            {
                //Memory free
                table = null;
                localDatabase = null;
            }
        }

        //Add Comments
        public void retrieveRows()
        {
        }
    }
}
