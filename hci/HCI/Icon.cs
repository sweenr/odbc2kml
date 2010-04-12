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
using HCI;

namespace HCI
{
    public class Icon
    {
        internal string location;
        internal ArrayList conditions;
        internal string iconId;
        internal Boolean isLocal;

        public Icon()
        {
            conditions = new ArrayList();
        }

        public Icon(Icon i)
        {
            location = i.getLocation();
            conditions = i.getDeepCopyOfConditions();
            iconId = i.getId();
            isLocal = i.getLocality();
        }

        ~Icon()
        {
            location = null;
            conditions = null;
            iconId = null;
            isLocal = false;
        }
       
        public string getLocation()
        {
            return this.location;
        }

        public void setLocation(string loc)
        {
            this.location = loc;
        }

        public ArrayList getConditions()
        {
            return this.conditions;
        }

        public ArrayList getDeepCopyOfConditions()
        {
            return new ArrayList(this.conditions);
        }

        public void setConditions(Condition con)
        {
            this.conditions.Add(con);
        }

        public void setConditions(ArrayList conList)
        {
            this.conditions = conList;
        }

        public void removeConditions(string conditionId)
        {
            foreach (Condition condition in this.conditions)
            {
                if (condition.getId() == conditionId)
                {
                    this.conditions.Remove(condition);
                    return;
                }
            }
        }

        /// <summary>
        /// Remove condition removes a condition at a specified index.
        /// </summary>
        /// <param name="index">int --> index of condition</param>
        public void removeCondition(int index)
        {
            this.conditions.RemoveAt(index);
        }

        public string getId()
        {
            return this.iconId;
        }

        public void setId(string id)
        {
            this.iconId = id;
        }

        public Boolean getLocality()
        {
            return this.isLocal;
        }

        public void setLocality(Boolean a)
        {
            this.isLocal = a;
        }

        public static ArrayList getIcons(int connID)
        {
            ArrayList icons = new ArrayList();
            Database localDatabase = new Database();

            //Create icon query and populate table
            string query = "SELECT * FROM Icon WHERE connID=" + connID + " ORDER BY ID";
            DataTable table = null;

            try
            {
                table = localDatabase.executeQueryLocal(query);
            }
            catch (ODBC2KMLException ex)
            {
                ex.errorText = "There was an error getting icons for the connection";
                throw ex;
            }

            foreach (DataRow row in table.Rows)
            {
                //Create a new icon
                Icon newIcon = new Icon();

                //Create a new table to perform subqueries on
                DataTable newTable = new DataTable();

                //IconLibrary query
                string locQuery = "SELECT * FROM IconLibrary WHERE ID=" + ((int)row["iconLibraryID"]) + " ORDER BY ID";

                try
                {
                    newTable = localDatabase.executeQueryLocal(locQuery);
                }
                catch (ODBC2KMLException ex)
                {
                    ex.errorText = "There was an error populating the Icon Library";
                    throw ex;
                }

                foreach (DataRow nRow in newTable.Rows)
                {
                    //Set the location of the icon
                    newIcon.setLocation(nRow["location"].ToString());
                    newIcon.setId(nRow["ID"].ToString());
                    if ((Boolean)nRow["isLocal"] == false)
                    {
                        newIcon.setLocality(false);
                    }
                    else
                    {
                        newIcon.setLocality(true);
                    }
                }//End outer loop

                newTable.Clear();

                //IconCondition query
                string conQuery = "SELECT * FROM IconCondition WHERE iconID="
                    + ((int)row["ID"]) + " AND connID=" + connID;

                try
                {
                    newTable = localDatabase.executeQueryLocal(conQuery);
                }
                catch (ODBC2KMLException ex)
                {
                    ex.errorText = "There was a problem selecting icon conditions for icon " + (int)row["iconLibraryID"];
                    throw ex;
                }

                //Cycle through each condition
                foreach (DataRow nRow in newTable.Rows)
                {
                    //Create the condition and add its values
                    Condition condition = new Condition();

                    if (nRow["lowerBound"] != null)
                    {
                        condition.setLowerBound(nRow["lowerBound"].ToString());
                    }
                    else
                    {
                        condition.setLowerBound("");
                    }

                    if (nRow["upperBound"] != null)
                    {
                        condition.setUpperBound(nRow["upperBound"].ToString());
                    }
                    else
                    {
                        condition.setUpperBound("");
                    }

                    condition.setLowerOperator((int)nRow["lowerOperator"]);
                    condition.setUpperOperator((int)nRow["upperOperator"]);
                    condition.setTableName(nRow["tableName"].ToString());
                    condition.setFieldName(nRow["fieldName"].ToString());
                    condition.setId(Convert.ToInt16(nRow["iconID"].ToString()));


                    //Add the condition to the icon array
                    newIcon.setConditions(condition);
                    //Free up condition memory
                    condition = null;
                }//End outer loop
                //Free up table memory
                newTable = null;
                
                icons.Add(newIcon);
                //Free up icon memory
                newIcon = null;

            }//End outer loop

            return icons;
        }

        /// <summary>
        /// This function purges all of the invalid conditions for the given Database information.
        /// </summary>
        /// <param name="purgeDT">DataTable --> List of tablename</param>
        /// <param name="columnToTableRelation">DataSet --> List of columns for each table name</param>
        /// <returns>Boolean --> True if purge, false if no purge</returns>
        public Boolean purgeInvalidIconConditions(DataTable purgeDT, DataSet columnToTableRelation)
        {
            Boolean didPurge = false;

            //Get the icon's conditions
            for (int count = 0; count < this.getConditions().Count; count++)
            {
                if (!(((Condition)this.getConditions()[count]).isValid(purgeDT, columnToTableRelation)))
                {
                    this.removeCondition(count);
                    didPurge = true;
                    count--;
                }
            }

            return didPurge;
        }

        /// <summary>
        /// This function purges all of the invalid conditions for the given Database information from the local database.
        /// </summary>
        /// <param name="purgeDT">DataTable --> List of tablename</param>
        /// <param name="columnToTableRelation">DataSet --> List of columns for each table name</param>
        /// <param name="temp">Database --> A temp database used to remove the icon conditions from local database</param>
        /// <returns>Boolean --> True if purge, false if no purge</returns>
        public Boolean purgeInvalidIconConditionsFromDatabase(DataTable purgeDT, DataSet columnToTableRelation, Database temp)
        {
            Boolean didPurge = false;

            //Get the icon's conditions
            for (int count = 0; count < this.getConditions().Count; count++)
            {
                //If the condition is invalid, remove it from the database and connection object
                if (!(((Condition)this.getConditions()[count]).isValid(purgeDT, columnToTableRelation)))
                {
                    String query = "DELETE FROM IconCondition WHERE ID=" + ((Condition)this.getConditions()[count]).getId();

                    try
                    {
                        temp.executeQueryLocal(query);
                        this.removeCondition(count);
                        count--;
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ex.errorText = "There was an error deleting an icon condition";
                        throw ex;
                    }
                    didPurge = true;
                }
            }

            return didPurge;
        }
    }
}
