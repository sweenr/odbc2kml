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

        public static readonly int INVALIDCONNINFO = 1;
        public static readonly int INVALIDMAPPING = 2;
        public static readonly int INVALIDSTATE = 3;
        public static readonly int INVALIDDESCRIPTION = 4;
        public static readonly int INVALIDFLAGS = 5;
        public static readonly int CONNECTIONSAVED = 0;

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
            try
            {
                //Return false if the conn information is bad
                if (!this.connInfo.isValid(this.connID))
                {
                    return false;
                }
            }
            catch (ODBC2KMLException ex)
            {
                throw ex;
            }

            //Database needed to see if values need to be purged
            Database purgeDB = new Database(this.connInfo);

            //DataTable to hold all table names and 
            DataTable purgeDT = null;

            //dataset to hold all column names for each table in the datatable
            DataSet newTableColumnRelation = new DataSet();

            //Verify mapping
            try
            {
                //If there is a mapping format
                if (this.mapping.getFormat() != 0)
                {
                    String queryables = "";

                    //Add columns needed
                    if (this.mapping.getLatFieldName() != "")
                    {
                        queryables += this.mapping.getLatFieldName();
                    }

                    if (this.mapping.getLongFieldName() != "")
                    {
                        if (queryables == "")
                        {
                            queryables += this.mapping.getLongFieldName();
                        }
                        else
                        {
                            queryables += ", " + this.mapping.getLongFieldName();
                        }
                    }

                    if (this.mapping.getPlacemarkFieldName() != "")
                    {
                        if (queryables == "")
                        {
                            queryables += this.mapping.getPlacemarkFieldName();
                        }
                        else
                        {
                            queryables += ", " + this.mapping.getPlacemarkFieldName();
                        }
                    }

                    //Test the query
                    String query = "SELECT " + queryables + " FROM " + this.mapping.getTableName();
                    purgeDB.executeQueryRemote(query);
                }
                else //reset the mapping, just to be safe if format = 0
                {
                    this.mapping = new Mapping();
                }

            }
            catch //Clear mapping
            {
                this.mapping = new Mapping();
            }

            try
            {
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
            }
            catch (ODBC2KMLException ex)
            {
                ex.errorText = "There was a problem retreiving column names from the remote database";
                throw ex;
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
            try
            {
                //Return false if the conn information is bad
                if (!this.connInfo.isValid(this.connID))
                {
                    return false;
                }
            }
            catch (ODBC2KMLException ex)
            {
                throw ex;
            }

            //Database needed to see if values need to be purged
            Database purgeDB = new Database(this.connInfo);

            //DataTable to hold all table names and 
            DataTable purgeDT;

            //dataset to hold all column names for each table in the datatable
            DataSet newTableColumnRelation = new DataSet();

            //Flag to determine if the mapping should be removed
            Boolean removeMapping = false;

            //Verify mapping
            try
            {
                //If there is a mapping format
                if (this.mapping.getFormat() != 0)
                {
                    String queryables = "";

                    //Add columns needed
                    if (this.mapping.getLatFieldName() != "")
                    {
                        queryables += this.mapping.getLatFieldName();
                    }

                    if (this.mapping.getLongFieldName() != "")
                    {
                        if (queryables == "")
                        {
                            queryables += this.mapping.getLongFieldName();
                        }
                        else
                        {
                            queryables += ", " + this.mapping.getLongFieldName();
                        }
                    }

                    if (this.mapping.getPlacemarkFieldName() != "")
                    {
                        if (queryables == "")
                        {
                            queryables += this.mapping.getPlacemarkFieldName();
                        }
                        else
                        {
                            queryables += ", " + this.mapping.getPlacemarkFieldName();
                        }
                    }

                    //Test the query
                    String query = "SELECT " + queryables + " FROM " + this.mapping.getTableName();
                    purgeDB.executeQueryRemote(query);
                }
                else //reset the mapping, just to be safe if format = 0
                {
                    removeMapping = true;
                    this.mapping = new Mapping();
                }

            }
            catch //Clear mapping
            {
                removeMapping = true;
                this.mapping = new Mapping();
            }

            try
            {
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
            }
            catch (ODBC2KMLException ex)
            {
                ex.errorText = "There was a problem retreiving column names from the remote database";
                throw ex;
            }

            //Flag used to see if the description should be removed
            Boolean removeDescription = false;

            try
            {
                //For any invalid icon conditions, remove them from the database
                foreach (Icon i in this.getIcons())
                {
                    if (i.purgeInvalidIconConditionsFromDatabase(purgeDT, newTableColumnRelation, purgeDB))
                    {
                        removeDescription = true;
                    }
                }
            }
            catch (ODBC2KMLException ex)
            {
                throw ex;
            }

            try
            {
                //For any invalid overlay conditions, remove them from the database
                foreach (Overlay o in this.getOverlays())
                {
                    if (o.purgeInvalidOverlayConditionsFromDatabase(purgeDT, newTableColumnRelation, purgeDB))
                    {
                        removeDescription = true;
                    }
                }
            }
            catch (ODBC2KMLException ex)
            {
                throw ex;
            }

            //Remove the description from the database
            if (removeDescription)
            {
                String query = "DELETE FROM Description WHERE connID=" + this.connID;
       
                try
                {
                    purgeDB.executeQueryLocal(query);
                }
                catch (ODBC2KMLException ex)
                {
                    ex.errorText = "There was a problem deleting the decription from the database.";
                    throw ex;
                }
            }

            //Remove the mapping if flag is set
            if (removeMapping)
            {
                String query = "DELETE FROM Mapping WHERE connID=" + this.connID;

                try
                {
                    purgeDB.executeQueryLocal(query);
                }
                catch (ODBC2KMLException ex)
                {
                    ex.errorText = "There was a problem deleting the mapping from the database.";
                    throw ex;
                }
            }

            return true;
        }

        //Additional

        /// <summary>
        /// Verifies all connection information and proceeds to attempt to save the connection.
        /// Returns 1 for INVALIDCONNINFO.
        /// Returns 2 for INVALIDMAPPING.
        /// Returns 3 for INVALIDSTATE.
        /// Returns 4 for INVALIDDESCRIPTION.
        /// Returns 5 for INVALIDFLAGS.
        /// Returns 0 for CONNECTIONSAVED.
        /// </summary>
        /// <returns>Integer --> Value based on what happens</returns>
        public int saveConn()
        {
            Boolean validConnInfo = false;
            Boolean validMapping = false;
            Boolean validDescription = false;
            Boolean validIcons = false;
            Boolean validOverlays = false;

            try
            {

                //Validate the Connection information
                if (!this.connInfo.isValid(this.connID))
                {
                    return 1;
                }
                else
                {
                    validConnInfo = true;
                }

                //Validate the mapping
                if (!this.mapping.isValid()) //TODO: resolve this
                {
                    return 2;
                }
                else
                {
                    validMapping = true;
                }

                //Remove bad icon and overlay conditions
                if (!this.safeStateConnection())
                {
                    return 3;
                }
                else
                {
                    validOverlays = true;
                    validIcons = true;
                }

                //Validate description
                if (!this.description.isValid(this.connInfo, this.mapping))
                {
                    return 4;
                }
                else
                {
                    validDescription = true;
                }

                //ALL THINGS ARE VALID, ATTEMPT A SAVE
                if (validConnInfo && validDescription && validIcons && validMapping && validOverlays)
                {
                    //Database needed to make all of the queries
                    Database database = new Database();

                    String query = "UPDATE Connection SET name=\'" + this.connInfo.connectionName
                        + "\', dbName=\'" + this.connInfo.databaseName
                        + "\', userName=\'" + this.connInfo.userName
                        + "\', password=\'" + this.connInfo.password
                        + "\', port=\'" + this.connInfo.portNumber
                        + "\', address=\'" + this.connInfo.serverAddress
                        + "\', protocol=\'" + this.connInfo.oracleProtocol
                        + "\', SID=\'" + this.connInfo.oracleSID
                        + "\', serviceName=\'" + this.connInfo.oracleServiceName
                        + "\', type=\'" + this.connInfo.databaseType + "\' WHERE ID=" + this.connID;

                    //Update the connection information
                    database.executeQueryLocal(query);

                    //If the latfieldname is empty, and it passed the validation function, then remove the mapping
                    if (this.mapping == null)
                    {
                        //Remove the mapping
                        query = "DELETE FROM Mapping WHERE connID=" + this.connID;
                        database.executeQueryLocal(query);
                    }
                    else
                    {
                        //See if there is a mapping
                        query = "SELECT * FROM Mapping WHERE connID=" + this.connID;
                        DataTable checkTable = database.executeQueryLocal(query);

                        //Is there currently a mapping?
                        if (checkTable.Rows.Count > 0)
                        {
                            query = "UPDATE Mapping SET format=" + this.mapping.getFormat()
                                + ", latFieldName='" + this.mapping.getLatFieldName() + "'"
                                + ", longFieldName='" + this.mapping.getLongFieldName() + "'"
                                + ", tableName='" + this.mapping.getTableName() + "'"
                                + ", placemarkFieldName='" + this.mapping.getPlacemarkFieldName() + "'"
                                + " WHERE connID=" + this.connID;
                        }
                        else //Insert mapping
                        {
                            query = "INSERT INTO Mapping (format, latFieldName, longFieldName, tableName, placemarkFieldName, connID) "
                                + "VALUES(" + this.mapping.getFormat() + ", '" + this.mapping.getLatFieldName() + "', '"
                                + this.mapping.getLongFieldName() + "', '" + this.mapping.getTableName() + "', '"
                                + this.mapping.getPlacemarkFieldName() + "', " + this.connID + ")";
                        }

                        //Update mapping
                        database.executeQueryLocal(query);
                    }

                    if (this.description.getDesc() == "") //No description
                    {
                        //Remove the description
                        query = "DELETE FROM Description WHERE connID=" + this.connID;
                        database.executeQueryLocal(query);
                    }
                    else //Description
                    {
                        //See if there is a description
                        query = "SELECT * FROM Description WHERE connID=" + this.connID;
                        DataTable checkTable = database.executeQueryLocal(query);

                        //Update the description
                        if (checkTable.Rows.Count > 0)
                        {
                            query = "UPDATE Description SET description='" + this.description.getDesc()
                                + "' WHERE connID=" + this.connID;
                        }
                        else //Insert the description
                        {
                            query = "INSERT INTO Description (connID, description) VALUES(" + this.connID
                                + ", '" + this.description.getDesc() + "')";
                        }

                        database.executeQueryLocal(query);
                    }

                    //Delete all icons
                    query = "DELETE FROM Icon WHERE connID=" + this.connID;
                    database.executeQueryLocal(query);

                    //There are icons
                    if (this.icons.Count > 0)
                    {
                        //Add all current icons to the database
                        foreach (Icon i in this.icons)
                        {
                            query = "INSERT INTO Icon (connID, iconID) VALUES(" + this.connID + ", " + i.getId() + ")";
                            database.executeQueryLocal(query);

                            //Add all conditions
                            foreach (Condition c in i.getConditions())
                            {
                                query = "INSERT INTO IconCondition (iconID, connID, lowerBound, upperBound, "
                                    + "lowerOperator, upperOperator, fieldName, tableName) VALUES(" + i.getId()
                                    + ", " + this.connID + ", '" + c.getLowerBound() + "', '" + c.getUpperBound() + "', "
                                    + Condition.operatorStringToInt(c.getLowerOperator()) + ", " + Condition.operatorStringToInt(c.getUpperOperator()) + ", '" + c.getFieldName()
                                    + "', '" + c.getTableName() + "')";
                                database.executeQueryLocal(query);
                            }
                        }
                    }

                    //Delete all icons
                    query = "DELETE FROM Overlay WHERE connID=" + this.connID;
                    database.executeQueryLocal(query);

                    //There are overlays
                    if (this.overlays.Count > 0)
                    {
                        //Add all current icons to the database
                        foreach (Overlay o in this.overlays)
                        {
                            query = "INSERT INTO Overlay (connID, color) VALUES(" + this.connID + ", '"  
                                + o.getColor() + "')";
                            database.executeQueryLocal(query);

                            query = "SELECT ID FROM Overlay WHERE connID=" + this.connID + " AND "
                                + "color='" + o.getColor() + "'";
                            DataTable tempTable = database.executeQueryLocal(query);

                            //Set ID
                            o.setId(tempTable.Rows[0]["ID"].ToString());

                            //Add all conditions
                            foreach (Condition c in o.getConditions())
                            {
                                query = "INSERT INTO OverlayCondition (overlayID, connID, lowerBound, upperBound, "
                                    + "lowerOperator, upperOperator, fieldName, tableName) VALUES(" + o.getId()
                                    + ", " + this.connID + ", '" + c.getLowerBound() + "', '" + c.getUpperBound() + "', "
                                    + Condition.operatorStringToInt(c.getLowerOperator()) + ", " + Condition.operatorStringToInt(c.getUpperOperator()) + ", '" + c.getFieldName()
                                    + "', '" + c.getTableName() + "')";
                                database.executeQueryLocal(query);
                            }
                        }
                    }
                }
                else //All flags are not true
                {
                    return 5;
                }
            }
            catch (ODBC2KMLException err)
            {
                throw err;
            }

            //The connection was saved and properly updated
            return 0;
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
