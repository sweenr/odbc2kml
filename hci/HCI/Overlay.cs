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
    public class Overlay
    {
        internal int id;
        internal string color;
        internal ArrayList conditions;

        //Constructors
        public Overlay()
        {
            conditions = new ArrayList();
            id = 0;
        }

        public Overlay(Overlay i)
        {
            id = Convert.ToInt32(i.getId());
            conditions = i.getDeepCopyOfConditions();
            color = i.getColor();
        }

        public string getId()
        {
            return this.id.ToString();
        }

        public void setId(string id)
        {
            this.id = Convert.ToInt32(id);
        }

        public string getColor()
        {
            return this.color;
        }

        public void setColor(string color)
        {
            this.color = color;
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

        public void removeConditions()
        {
            this.conditions.Clear();
        }

        public void removeConditions(Condition condition)
        {
            this.conditions.Remove(condition);
        }

        public static ArrayList getOverlays(int connID)
        {
            Database localDatabase = new Database();
            ArrayList overlays = new ArrayList();

            //Create overlay query and populate table
            string query = "SELECT * FROM Overlay WHERE connID=" + connID;
            DataTable table = localDatabase.executeQueryLocal(query);

            foreach (DataRow row in table.Rows)
            {
                //Create overlay and set basic information
                Overlay newOverlay = new Overlay();
                newOverlay.setColor(row["color"].ToString());
                newOverlay.setId(row["id"].ToString());

                //Create the new table for another query
                DataTable newTable = new DataTable();

                //Query string and query
                string conQuery = "SELECT * FROM OverlayCondition WHERE overlayID="
                    + (Convert.ToInt16(newOverlay.getId())) + " AND connID=" + connID;
                newTable = localDatabase.executeQueryLocal(conQuery);

                //Cycle through each condition
                foreach (DataRow nRow in newTable.Rows)
                {
                    //Create the condition and add its values
                    Condition condition = new Condition();
                    condition.setFieldName(nRow["fieldName"].ToString());
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
                    condition.setId(Convert.ToInt16(nRow["ID"].ToString()));

                    //Add the condition to the overlay array
                    newOverlay.setConditions(condition);
                    //Free up condition memory
                    condition = null;
                }//End loop           

                //Add the overlay to the list of overlays
                overlays.Add(newOverlay);
                newOverlay = null;
                newTable = null;
            }
            
            return overlays;
        }

        /// <summary>
        /// This function purges all of the invalid conditions for the given Database information.
        /// </summary>
        /// <param name="purgeDT">DataTable --> List of tablename</param>
        /// <param name="columnToTableRelation">DataSet --> List of columns for each table name</param>
        /// <returns>Boolean --> True if purge, false if no purge</returns>
        public Boolean purgeInvalidOverlayConditions(DataTable purgeDT, DataSet columnToTableRelation)
        {
            Boolean didPurge = false;

            //Get the overlay's conditions
            for (int count = 0; count < this.getConditions().Count; count++)
            {
                if (!(((Condition)this.getConditions()[count]).isValid(purgeDT, columnToTableRelation)))
                {
                    this.removeCondition(count);
                    didPurge = true;
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
        public Boolean purgeInvalidOverlayConditionsFromDatabase(DataTable purgeDT, DataSet columnToTableRelation, Database temp)
        {
            Boolean didPurge = false;

            //Get the icon's conditions
            for (int count = 0; count < this.getConditions().Count; count++)
            {
                //If the condition is invalid, remove it from the database and connection object
                if (!(((Condition)this.getConditions()[count]).isValid(purgeDT, columnToTableRelation)))
                {
                    String query = "DELETE FROM OverlayCondition WHERE ID=" + ((Condition)this.getConditions()[count]).getId();
                    temp.executeQueryLocal(query);
                    this.removeCondition(count);
                    didPurge = true;
                }
            }

            return didPurge;
        }
    }
}
