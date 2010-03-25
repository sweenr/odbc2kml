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
        private int id;
        private string color;
        private ArrayList conditions;

        //Constructors
        public Overlay()
        {
            conditions = new ArrayList();
            id = 0;
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

        public void setConditions(Condition con)
        {
            this.conditions.Add(con);
        }

        public void removeCondition(int arrayPosition)
        {
            this.conditions.RemoveAt(arrayPosition);
        }

        public static Overlay getOverlay(int connID, int overlayID)
        {
            Database localDatabase = new Database();
            Overlay overlay = new Overlay();

            //Create icon query and populate table
            string query = "SELECT * FROM Icon WHERE connID=" + connID
                + " AND ID=" + overlayID;
            DataTable table = localDatabase.executeQueryLocal(query);

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    //Create a new table to perform subqueries on
                    DataTable newTable = new DataTable();

                    //IconCondition query
                    string conQuery = "SELECT * FROM OverlayCondition WHERE overlayID="
                        + overlayID + " AND connID=" + connID;
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
                        overlay.setConditions(condition);
                        //Free up condition memory
                        condition = null;
                    }//End outer loop
                    //Free up table memory
                    newTable = null;
                }
            }

            return overlay;
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
                Overlay newOverlay = new Overlay();
                foreach (DataColumn col in table.Columns)
                {
                    if (col.ColumnName == "ID") //Branch off and get the conditions
                    {
                        //Create the new table for another query
                        DataTable newTable = new DataTable();

                        //Query string and query
                        string conQuery = "SELECT * FROM OverlayCondition WHERE overlayID="
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
                    else if (col.ColumnName == "color") //Set the color
                    {
                        newOverlay.setColor(row[col].ToString());
                    }
                }
                //Add the overlay to the list of overlays
                overlays.Add(newOverlay);
                newOverlay = null;
            }//End outer loop

            return overlays;
        }
    }
}
