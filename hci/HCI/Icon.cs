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
        private string location;
        private ArrayList conditions;
        private string iconId;
        private Boolean isLocal;

        public Icon()
        {
            conditions = new ArrayList();
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

        public void setConditions(Condition con)
        {
            this.conditions.Add(con);
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

        public void removeConditions(Condition condition)
        {
            this.conditions.Remove(condition);
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

        public static Icon getIcon(int connID, int iconID)
        {
            Database localDatabase = new Database();
            Icon icon = new Icon();

            //Create icon query and populate table
            string query = "SELECT * FROM Icon WHERE connID=" + connID
                + " AND ID=" + iconID;
            DataTable table = localDatabase.executeQueryLocal(query);

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {

                    //Create a new table to perform subqueries on
                    DataTable newTable = new DataTable();

                    //IconLibrary query
                    string locQuery = "SELECT * FROM IconLibrary WHERE ID=" + iconID;
                    newTable = localDatabase.executeQueryLocal(locQuery);

                    foreach (DataRow nRow in newTable.Rows)
                    {
                        //Set the location of the icon
                        icon.setLocation(nRow["location"].ToString());
                        
                        if((Boolean)nRow["isLocal"] == false)
                        {
                            icon.setLocality(false);
                        }
                        else
                        {
                            icon.setLocality(true);
                        }
                    }//End outer loop

                    newTable.Clear();

                    //IconCondition query
                    string conQuery = "SELECT * FROM IconCondition WHERE iconID="
                        + iconID + " AND connID=" + connID;
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
                        icon.setConditions(condition);
                        //Free up condition memory
                        condition = null;
                    }//End outer loop
                    //Free up table memory
                    newTable = null;
                }
            }

            return icon;
        }

        public static ArrayList getIcons(int connID)
        {
            ArrayList icons = new ArrayList();
            Database localDatabase = new Database();

            //Create icon query and populate table
            string query = "SELECT * FROM Icon WHERE connID=" + connID;
            DataTable table = localDatabase.executeQueryLocal(query);

            foreach (DataRow row in table.Rows)
            {
                //Create a new icon
                Icon newIcon = new Icon();

                foreach (DataColumn col in table.Columns)
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
                            //Set the location of the icon
                            newIcon.setLocation(nRow["location"].ToString());

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
                            + ((int)row[col]) + " AND connID=" + connID;
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
                                        if (nRow[nCol] != null)
                                        {
                                            condition.setLowerBound(nRow[nCol].ToString());
                                        }
                                        else
                                        {
                                            condition.setLowerBound("");
                                        }
                                        break;
                                    case "upperBound":
                                        if (nRow[nCol] != null)
                                        {
                                            condition.setUpperBound(nRow[nCol].ToString());
                                        }
                                        else
                                        {
                                            condition.setUpperBound("");
                                        }
                                        break;
                                    case "lowerOperator":
                                        if (nRow[nCol] != null)
                                        {
                                            condition.setLowerOperator((int)nRow[nCol]);
                                        }
                                        else
                                        {
                                            condition.setLowerOperator(0);
                                        }
                                        break;
                                    case "upperOperator":
                                        if (nRow[nCol] != null)
                                        {
                                            condition.setUpperOperator((int)nRow[nCol]);
                                        }
                                        else
                                        {
                                            condition.setUpperOperator(0);
                                        }
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
                icons.Add(newIcon);
                //Free up icon memory
                newIcon = null;

            }//End outer loop

            return icons;
        }
    }
}
