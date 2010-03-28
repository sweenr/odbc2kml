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
using HCI;

namespace HCI
{
    public class Mapping
    {
        //Globals
        public static readonly int SEPARATE = 1;
        public static readonly int LATFIRST = 2;
        public static readonly int LONGFIRST = 3;

        //Datatypes
        private string tableName;
        private string latFieldName;
        private string longFieldName;
        private int format;

        //Functions

        //Constructor
        public Mapping()
        {

        }

        //Getters

        //Retrieve tableName
        public string getTableName()
        {
            return this.tableName;
        }

        //Retrieve latFieldName
        public string getLatFieldName()
        {
            return this.latFieldName;
        }

        //Retrieve longFieldName
        public string getLongFieldName()
        {
            return this.longFieldName;
        }

        //Retrieve format
        public int getFormat()
        {
            return this.format;
        }

        //Setters

        //Set tableName
        public void setTableName(string tableName)
        {
            this.tableName = tableName;
        }

        //Set latFieldName
        public void setLatFieldName(string latFieldName)
        {
            this.latFieldName = latFieldName;
        }

        //Set longFieldName
        public void setLongFieldName(string longFieldName)
        {
            this.longFieldName = longFieldName;
        }

        //Set format
        public void setFormat(int format)
        {
            this.format = format;
        }

        //Additional
        public bool isValid(DataTable LLTable)
        {
            bool valid = true;
            
            try
            {             
                foreach (DataRow dr in LLTable.Rows)
                {
                    foreach (Object data in dr.ItemArray)
                    {
                        double test = double.Parse(data.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                valid = false;
                return valid;
            }
            return valid;
        }

        //Seperating Long from Lat and vise versa
        public double[] separate(string cord, int order)
        {
            int mid;
            if (cord.Contains(','))
                mid = cord.IndexOf(',');
            else if (cord.Contains('|'))
                mid = cord.IndexOf('|');
            else if (cord.Contains('-'))
                mid = cord.IndexOf('-');
            else if (cord.Contains('/'))
                mid = cord.IndexOf('/');
            else if (cord.Contains('\\'))
                mid = cord.IndexOf('\\');
            else if (cord.Contains(' '))
                mid = cord.IndexOf(' ');
            else
            {
                throw new ODBC2KMLException("Not a correct format of Latitude and Longitude");
            }


            String p1 = "", p2 = "";
            int i;
            for (i = 0; i < mid; i++)
                p1 += cord[i];
            for (++i; i <= cord.Length; i++)
                p2 += cord[i];

            double q1 = 0;
            double q2 = 0;
            try
            {
                q1.Equals(double.Parse(p1));
                q2.Equals(double.Parse(p2));
                if (order.Equals(LATFIRST))
                {
                    double[] coordinates = new double[2];
                    coordinates[0] = q1;
                    coordinates[1] = q2;

                    return coordinates;
                    //latFieldName = p1;
                    //longFieldName = p2;
                }
                else
                {
                    double[] coordinates = new double[2];
                    coordinates[0] = q2;
                    coordinates[1] = q1;

                    return coordinates;

                    //latFieldName = p2;
                    //longFieldName = p1;
                }

            }
            catch (Exception e)
            {
                throw new ODBC2KMLException("This is not a Longitude and Latitude Coordinate.");
            }
        }

        public static Mapping getMapping(int connID, string tableName)
        {
            Mapping mapping = new Mapping();
            Database localDatabase = new Database();

            //Create mapping query and populate table
            string query = "SELECT * FROM Mapping WHERE connID=" + connID + " AND tableName='" + tableName + "'";
            DataTable table = localDatabase.executeQueryLocal(query);

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    //Set mapping
                    switch (col.ColumnName)
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
                        default:
                            break;                  
                    }
                }
            }//End outer loop

            return mapping;
        }

        public static void insertMapping(int connID, string tableName, string latFieldName, string longFieldName, int format)
        {
            Mapping mapping = new Mapping();
            Database localDatabase = new Database();

            string query = "INSERT INTO MAPPING VALUES ('" + tableName+ "', '" + latFieldName + "', '" + longFieldName + "', '" + format + "', '" + connID + "')";
            localDatabase.executeQueryLocal(query);
        }

        public static void updateMapping(int connID, string tableName, string latFieldName, string longFieldName, int format)
        {
            Mapping mapping = new Mapping();
            Database localDatabase = new Database();

            string query = "UPDATE MAPPING SET latFieldName = '" + latFieldName + "', longFieldName = '" + longFieldName + "', format = '"+ format +"' WHERE connID = '" + connID + "' AND tableName = '" + tableName + "'";
            localDatabase.executeQueryLocal(query);
        }

        public static Mapping getMapping2(int connID, string tableName)
        {
            Mapping mapping = new Mapping();
            Database localDatabase = new Database();

            //Create mapping query and populate table
            string query = "SELECT * FROM Mapping WHERE connID='" + connID + "' AND tableName = '" + tableName + "'";
            DataTable table = localDatabase.executeQueryLocal(query);

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    //Set mapping
                    switch (col.ColumnName)
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
                        case "latlongFieldName":
                            mapping.separate(row[col].ToString(), LATFIRST);
                            break;
                        case "longlatFieldName":
                            mapping.separate(row[col].ToString(), LONGFIRST);
                            break;

                    }
                }
            }//End outer loop

            return mapping;
        }

    }
}
