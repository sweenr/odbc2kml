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
        private ArrayList icons = new ArrayList();
        private ArrayList overlays = new ArrayList();
        private ConnInfo connInfo;
        private int connID;

        //Functions

        //Constructor
        Connection()
        {
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
                        switch(col.ColumnName) 
                        {
                            case name:
                                this.connInfo.setConnectionName(row[col]);
                                break;
                            case dbName:
                                this.connInfo.setDatabaseName(row[col]);
                                break;
                            case userName:
                                this.connInfo.setUserName(row[col]);
                                break;
                            case password:
                                this.connInfo.setPassword(row[col]);
                                break;
                            case port:
                                this.connInfo.setPortNumber(row[col]);
                                break;
                            case address:
                                this.connInfo.setServerAddress(row[col]);
                                break;
                            case type:
                                this.connInfo.setDatabaseType(row[col]);
                                break;
                            case protocol:
                                this.connInfo.setOracleProtocol(row[col]);
                                break;
                            case serviceName:
                                this.connInfo.setOracleServiceName(row[col]);
                                break;
                            case SID:
                                this.connInfo.setOracleSID(row[col]);
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
                    foreach(DataColumn col in table.Columns)
                    {
                        switch(col.ColumnName)
                        {

                        }
                    }
                }//End outer loop

                //Clear table
                table.Clear();

                //Create description query and populate table
                query = "SELECT * FROM Description WHERE connID=" + this.connID;
                table = localDatabase.executeQueryLocal(query);

                //Clear table
                table.Clear();

                //Create mapping query and populate table
                query = "SELECT * FROM Mapping WHERE connID=" + this.connID;
                table = localDatabase.executeQueryLocal(query);

                //Clear table
                table.Clear();

                //Create icon query and populate table
                query = "SELECT * FROM Icon WHERE connID=" + this.connID;
                table = localDatabase.executeQueryLocal(query);

                //Clear table
                table.Clear();
            }
            catch(Exception e) 
            {

            }


        }

        //Add Comments
        public void retrieveRows()
        {
        }
    }
}
