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
        internal Description description;
        internal Mapping mapping;
        internal ArrayList icons;
        internal ArrayList overlays;
        internal ConnInfo connInfo;
        internal int connID;

        //Functions

        //Constructor
        public Connection()
        {
            icons = new ArrayList();
            overlays = new ArrayList();
            description = new Description();
            mapping = new Mapping();
            connInfo = new ConnInfo();
        }

        public Connection(int connID)
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

        /// <summary>
        /// Remove all icon and overlay conditions that are no longer valid. If any conditions are removed,
        /// purge the description. Returns true if the connection has been validated and false if it failed.
        /// This is only a temporary purge. If the update is canceled, the connection will be restored to its
        /// previous state. If something unexpected happen, this function will throw an ODBC2KMLException.
        /// </summary>
        /// <param name="descriptionBox">TextBox --> The TextBox that holds the description</param>
        /// <returns>Boolean --> false if the purge failed, true if it succeeded</returns>
        public bool validateConnnection(TextBox descriptionBox)
        {
            //Return false if the conn information is bad
            if (!this.connInfo.isValid())
            {
                return false;
            }

            //Database needed to see if values need to be purged
            Database purgeDB = new Database(this.connInfo);

            //DataTable to hold all table names and 
            DataTable purgeDT;

            //dataset to hold all column names for each table in the datatable
            DataSet newTableColumnRelation = new DataSet();

            if (this.connInfo.getDatabaseType() == ConnInfo.MSSQL)
            {
                //MSSQL specific call
                purgeDT = purgeDB.executeQueryRemote("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' AND TABLE_NAME != 'sysdiagrams'");

                foreach (DataRow row in purgeDT.Rows)
                {
                    //Retrieve each column name for each table in the purge data table
                    DataTable purgeDC = purgeDB.executeQueryRemote("SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + row["TABLE_NAME"] + "')");

                    //Add the retrieved table to the Dataset
                    purgeDC.TableName = row["TABLE_NAME"].ToString();
                    newTableColumnRelation.Tables.Add(purgeDC);
                }
            }
            else if (this.connInfo.getDatabaseType() == ConnInfo.MYSQL)
            {
                //MySQL specific call
                purgeDT = purgeDB.executeQueryRemote("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA != 'mysql'");

                foreach (DataRow row in purgeDT.Rows)
                {
                    //Retrieve each column name for each table in the purge data table
                    DataTable purgeDC = purgeDB.executeQueryRemote("SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + row["TABLE_NAME"] + "')");

                    //Add the retrieved table to the Dataset
                    purgeDC.TableName = row["TABLE_NAME"].ToString();
                    newTableColumnRelation.Tables.Add(purgeDC);
                }
            }
            else if (this.connInfo.getDatabaseType() == ConnInfo.ORACLE)
            {
                //Oracle specific call
                purgeDT = purgeDB.executeQueryRemote("select TABLE_NAME from user_tables");

                foreach (DataRow row in purgeDT.Rows)
                {
                    //Retrieve each column name for each table in the purge data table
                    DataTable purgeDC = purgeDB.executeQueryRemote("SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + row["TABLE_NAME"] + "')");

                    //Add the retrieved table to the Dataset
                    purgeDC.TableName = row["TABLE_NAME"].ToString();
                    newTableColumnRelation.Tables.Add(purgeDC);
                }
            }
            else //Just in case....Bad error
            {
                throw new ODBC2KMLException("The update function failed to perform properly, please try again.");
            }

            foreach (Icon i in this.getIcons())
            {
                if(i.purgeInvalidIconConditions(purgeDT, newTableColumnRelation))
                {
                    descriptionBox.Text = "";
                }
            }

            foreach (Overlay o in this.getOverlays())
            {
                if (o.purgeInvalidOverlayConditions(purgeDT, newTableColumnRelation))
                {
                    descriptionBox.Text = "";
                }
            }

            return true;
        }

        /// <summary>
        /// This function is called when the information is editted from main. This connection forces all
        /// invalid connection information to be purged from the database. This is a permanent purge.
        /// If anything unexpected happens, a ODBC2KMLException will be thrown.
        /// </summary>
        /// <returns>Boolean --> False if connection cannot be guaranteed to be in a safe state
        /// true, if the connection is in a safe state</returns>
        public Boolean safeStateConnection()
        {
            //Return false if the conn information is bad
            if (!this.connInfo.isValid())
            {
                return false;
            }

            //Database needed to see if values need to be purged
            Database purgeDB = new Database(this.connInfo);

            //DataTable to hold all table names and 
            DataTable purgeDT;

            //dataset to hold all column names for each table in the datatable
            DataSet newTableColumnRelation = new DataSet();

            if (this.connInfo.getDatabaseType() == ConnInfo.MSSQL)
            {
                //MSSQL specific call
                purgeDT = purgeDB.executeQueryRemote("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' AND TABLE_NAME != 'sysdiagrams'");

                foreach (DataRow row in purgeDT.Rows)
                {
                    //Retrieve each column name for each table in the purge data table
                    DataTable purgeDC = purgeDB.executeQueryRemote("SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + row["TABLE_NAME"] + "')");

                    //Add the retrieved table to the Dataset
                    purgeDC.TableName = row["TABLE_NAME"].ToString();
                    newTableColumnRelation.Tables.Add(purgeDC);
                }
            }
            else if (this.connInfo.getDatabaseType() == ConnInfo.MYSQL)
            {
                //MySQL specific call
                purgeDT = purgeDB.executeQueryRemote("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA != 'mysql'");

                foreach (DataRow row in purgeDT.Rows)
                {
                    //Retrieve each column name for each table in the purge data table
                    DataTable purgeDC = purgeDB.executeQueryRemote("SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + row["TABLE_NAME"] + "')");

                    //Add the retrieved table to the Dataset
                    purgeDC.TableName = row["TABLE_NAME"].ToString();
                    newTableColumnRelation.Tables.Add(purgeDC);
                }
            }
            else if (this.connInfo.getDatabaseType() == ConnInfo.ORACLE)
            {
                //Oracle specific call
                purgeDT = purgeDB.executeQueryRemote("select TABLE_NAME from user_tables");

                foreach (DataRow row in purgeDT.Rows)
                {
                    //Retrieve each column name for each table in the purge data table
                    DataTable purgeDC = purgeDB.executeQueryRemote("SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + row["TABLE_NAME"] + "')");

                    //Add the retrieved table to the Dataset
                    purgeDC.TableName = row["TABLE_NAME"].ToString();
                    newTableColumnRelation.Tables.Add(purgeDC);
                }
            }
            else //Just in case....Bad error
            {
                throw new ODBC2KMLException("The update function failed to perform properly, please try again.");
            }

            //Flag used to see if the description should be removed
            Boolean removeDescription = false;

            //For any invalid icon conditions, remove them from the database
            foreach (Icon i in this.getIcons())
            {
                if (i.purgeInvalidIconConditionsFromDatabase(purgeDT, newTableColumnRelation, purgeDB))
                {
                    removeDescription = true;
                }
            }

            //For any invalid overlay conditions, remove them from the database
            foreach (Overlay o in this.getOverlays())
            {
                if (o.purgeInvalidOverlayConditionsFromDatabase(purgeDT, newTableColumnRelation, purgeDB))
                {
                    removeDescription = true;
                }
            }

            //Remove the description from the database
            if (removeDescription)
            {
                String query = "DELETE FROM Description WHERE connID=" + this.connID;
                purgeDB.executeQueryLocal(query);
            }

            return true;
        }

        //Additional

        //TODO: ADD PARAMETERS TO ALL OF THE FOLLOWING

        //Add Comments
        public void saveConn()
        {
        }

        //Add Comments
        public void deleteConn()
        {
        }


        /// <summary>
        /// Populate fields uses the connID passed into the constructor
        /// and retrieves all of the information about that specific connection
        /// from the database. It will then set all of the classes variables
        /// based on this information. 
        /// </summary>
        public void populateFields()
        {
            try
            {
                this.connInfo = ConnInfo.getConnInfo(this.connID);
                this.overlays = Overlay.getOverlays(this.connID);
                this.description = Description.getDescription(this.connID);
                this.mapping = Mapping.getMapping(this.connID);                
                this.icons = Icon.getIcons(this.connID);
            }
            catch(ODBC2KMLException e) //Add whatever exceptions are needed and error handling code
            {
                throw e;
            }
        }
    }
}
