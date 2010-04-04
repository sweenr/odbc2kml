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

        //Add Comments
        public bool isValid()
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

            //Check variable to allow for easy breaks out of loops
            //and to verify that a table/field name combination matched
            Boolean breakOut = false;

            //Check to know if the description should be purged
            Boolean purgeDescription = false;

            //For each icon in temp icon list
            foreach (Icon i in this.icons)
            {
                //Get the icon's conditions
                for (int count = 0; count < i.getConditions().Count; count++)
                {
                    //For each table name, see if the conditions table name matches
                    foreach (DataRow row in purgeDT.Rows)
                    {
                        String rowName = row["TABLE_NAME"].ToString().ToLower();

                        //Do table names match?
                        if (rowName.Equals(((Condition)i.getConditions()[count]).getTableName().ToLower()))
                        {
                            //Ok, check the column names
                            foreach (DataRow row1 in newTableColumnRelation.Tables[row["TABLE_NAME"].ToString()].Rows)
                            {
                                String columnName = row1["COLUMN_NAME"].ToString().ToLower();

                                if (columnName.Equals(((Condition)i.getConditions()[count]).getFieldName().ToLower()))
                                {
                                    breakOut = true;
                                    break;
                                }
                            } //End for each

                            if (!breakOut) //Didn't break out, purge condition
                            {
                                i.removeCondition(count);
                                purgeDescription = true;
                                count--;
                            }
                        }

                        if (breakOut) //If a column and row has been matched, break out and proceed with next condition
                            break;
                    } //End for each

                    //Never found a match table, purge it
                    if (!breakOut)
                    {
                        i.removeCondition(count);
                        purgeDescription = true;
                        count--;
                    }

                    //Reset Variable
                    breakOut = false;

                } //End for each
            } //End for each

            //For each overlay in temp overlay list
            foreach (Overlay o in this.overlays)
            {
                //Get the icon's conditions
                for (int count = 0; count < o.getConditions().Count; count++)
                {
                    //For each table name, see if the conditions table name matches
                    foreach (DataRow row in purgeDT.Rows)
                    {
                        String rowName = row["TABLE_NAME"].ToString().ToLower();

                        //Do table names match?
                        if (rowName.Equals(((Condition)o.getConditions()[count]).getTableName().ToLower()))
                        {
                            //Ok, check the column names
                            foreach (DataRow row1 in newTableColumnRelation.Tables[row["TABLE_NAME"].ToString()].Rows)
                            {
                                String columnName = row1["COLUMN_NAME"].ToString().ToLower();

                                if (columnName.Equals(((Condition)o.getConditions()[count]).getFieldName().ToLower()))
                                {
                                    breakOut = true;
                                    break;
                                }
                            } //End for each

                            if (!breakOut) //Didn't break out, purge condition
                            {
                                o.removeCondition(count);
                                purgeDescription = true;
                                count--;
                            }
                        }

                        if (breakOut) //If a column and row has been matched, break out and proceed with next condition
                            break;
                    } //End for each

                    //Never found a match table, purge it
                    if (!breakOut)
                    {
                        o.removeCondition(count);
                        purgeDescription = true;
                        count--;
                    }

                    breakOut = false;

                } //End for each
            } //End for each

            //Purge description if needed
           /* if (purgeDescription)
            {
                descriptionBox.Text = "";
            }*/

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

        /*
         * Populate fields uses the connID passed into the constructor
         * and retrieves all of the information about that specific connection
         * from the database. It will then set all of the classes variables
         * based on this information. 
         */
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
