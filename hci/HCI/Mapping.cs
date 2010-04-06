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
    public class Mapping
    {
        //Globals
        public static readonly int NONE = 0;
        public static readonly int SEPARATE = 1;
        public static readonly int LATFIRST = 2;
        public static readonly int LONGFIRST = 3;

        //Datatypes
        internal int connID;
        internal string tableName;
        internal string latFieldName;
        internal string longFieldName;
        internal string placemarkFieldName;
        internal int format;

        //Functions

        //Constructor
        public Mapping()
        {
            connID = -1;
            tableName = "";
            latFieldName = "";
            longFieldName = "";
            placemarkFieldName = "";
            format = -1;
        }

        //Getters

        //Retrieve connID
        public int getConnID()
        {
            return this.connID;
        }

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

        //Retrieve placemarkFieldName
        public string getPlacemarkFieldName()
        {
            return this.placemarkFieldName;
        }

        //Retrieve format
        public int getFormat()
        {
            return this.format;
        }

        //Setters

        //Set connID
        public void setConnID(int connID)
        {
            this.connID = connID;
        }

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

        //Set placemarkFieldName
        public void setPlacemarkFieldName(string placemarkFieldName)
        {
            this.placemarkFieldName = placemarkFieldName;
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

        public Boolean isValid()
        {
            //If lat and long mappings are set
            if (this.getLatFieldName() != "" && this.getLongFieldName() != "")
            {
                if (this.getTableName() == "") //See if there is a table name
                {
                    //No table name
                    return false;
                }

                if (this.getFormat() != 1 || this.getFormat() != 2 || this.getFormat() != 3) //See if there is a format
                {
                    //No format
                    return false;
                }
            }
            else //No lat and long
            {
                if (this.getTableName() != "") //See if there is a table name
                {
                    //Table name
                    return false;
                }

                if (this.getFormat() != -1 || this.getFormat() != Mapping.NONE) //See if format is set to default values
                {
                    //Not default
                    return false;
                }
            }

            return true;
        }

        //Seperating Long from Lat and vise versa
        public double[] separate(string cord, int order)
        {
            int mid;
            if (cord.Contains(','))
                mid = cord.IndexOf(',');
            else if (cord.Contains('|'))
                mid = cord.IndexOf('|');
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
            {}    
            
            p1 = cord.Substring(0,i);

            p2 = cord.Substring(++mid, cord.Length - i - 1);

            Char[] bs = new Char[8] {')', '>', '}', '\'', '\"', '(', '<' ,'{'};

            double q1 = 0;
            double q2 = 0;
            try
            {
                q1 = (double.Parse(p1.Trim(bs)));
                q2 = (double.Parse(p2.Trim(bs)));
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

        /// <summary>
        /// Get all of the mapping information associated with a given connection.
        /// </summary>
        /// <param name="connID">int --> connection ID</param>
        /// <returns>Mapping --> Populated Map Object</returns>
        public static Mapping getMapping(int connID)
        {
            Database localDatabase = new Database();

            //Create mapping query and populate table
            string query = "SELECT * FROM Mapping WHERE connID=" + connID;
            DataTable table = localDatabase.executeQueryLocal(query);

            //Mapping to return
            Mapping map = new Mapping();

            foreach (DataRow row in table.Rows)
            {
                //Set mapping
                map.setTableName(row["tableName"].ToString());
                map.setLatFieldName(row["latFieldName"].ToString());
                map.setLongFieldName(row["longFieldName"].ToString());
                map.setPlacemarkFieldName(row["placemarkFieldName"].ToString());
                map.setFormat((int)row["format"]);
           
            }//End outer loop

            //Return populated map object
            return map;
        }

        /// <summary>
        /// Function to insert this mapping into the local database. Uses the values currently in this mapping object
        /// to populate the database fields.
        /// </summary>
        public static void insertMapping(Mapping m)
        {
            Database localDatabase = new Database();

            string query = "INSERT INTO MAPPING (tableName, latFieldName, longFieldName, placemarkFieldName, format, connID) VALUES ('" + m.tableName + "', '" + m.latFieldName + "', '" + m.longFieldName + "', '" + m.placemarkFieldName + "', '" + m.format + "', '" + m.connID + "')";
            try
            {
                localDatabase.executeQueryLocal(query);
            }
            catch (ODBC2KMLException ex)
            {
                throw new ODBC2KMLException(ex.errorText);
            }
        }

        /// <summary>
        /// Function to update a mapping already in the local database. Updates all columns except the connID columns.
        /// Uses the values currently in this mapping object to update the database fields.
        /// </summary>
        public static void updateMapping(Mapping m)
        {
            Database localDatabase = new Database();

            string query = "UPDATE MAPPING SET latFieldName = '" + m.latFieldName + "', longFieldName = '" + m.longFieldName + "', placemarkFieldName = '" + m.placemarkFieldName + "', format = '" + m.format + "', tableName = '" + m.tableName + "' WHERE connID = '" + m.connID + "'";
            localDatabase.executeQueryLocal(query);
        }

        /// <summary>
        /// Function to delete a mapping from the local database. Deletes the mapping that corresponds to this mapping
        /// object's connID to delete the database row.
        /// </summary>
        public static void deleteMapping(Mapping m)
        {
            Database localDatabase = new Database();

            string query = "DELETE FROM Mapping WHERE connID = '" + m.connID + "'";
            localDatabase.executeQueryLocal(query);
        }

        /// <summary>
        /// DEPRECATED - Use insertMapping() instead.
        /// Function to add to the Mapping database the values passed to it.
        /// </summary>
        /// <param name="connID"> - int --> Connection ID </param>
        /// <param name="tableName">string --> Name of table mapped</param>
        /// <param name="latFieldName">string --> Name of field that contains the latitude</param>
        /// <param name="longFieldName">string --> Name of field that contains the longitude. If the lat and lon are both in the same column, put that same column in both latFieldName and longFieldName.</param>
        /// <param name="format">string --> Format of lat/lon field names, 1 for SEPARATE, 2 for together and LATFIRST, 3 for together and LONGFIRST</param>
        public static void insertMapping(int connID, string tableName, string latFieldName, string longFieldName, int format)
        {
            Mapping mapping = new Mapping();
            Database localDatabase = new Database();

            string query = "INSERT INTO MAPPING ('tableName', 'latFieldName', 'longFieldName', 'format', 'connID') VALUES ('" + tableName + "', '" + latFieldName + "', '" + longFieldName + "', '" + format + "', '" + connID + "')";
            localDatabase.executeQueryLocal(query);
        }

        /// <summary>
        /// DEPRECATED - Use updateMapping() instead.
        /// Function to update the Mapping database with the values passed to it.
        /// </summary>
        /// <param name="connID"> - int --> Connection ID </param>
        /// <param name="tableName">string --> Name of table mapped</param>
        /// <param name="latFieldName">string --> Name of field that contains the latitude</param>
        /// <param name="longFieldName">string --> Name of field that contains the longitude. If the lat and lon are both in the same column, put that same column in both latFieldName and longFieldName.</param>
        /// <param name="format">string --> Format of lat/lon field names, 1 for SEPARATE, 2 for together and LATFIRST, 3 for together and LONGFIRST</param>
        public static void updateMapping(int connID, string tableName, string latFieldName, string longFieldName, int format)
        {
            Mapping mapping = new Mapping();
            Database localDatabase = new Database();

            string query = "UPDATE MAPPING SET latFieldName = '" + latFieldName + "', longFieldName = '" + longFieldName + "', format = '"+ format +"' WHERE connID = '" + connID + "' AND tableName = '" + tableName + "'";
            localDatabase.executeQueryLocal(query);
        }

        /// <summary>
        /// Get a specific mapping for a given tableName. Used to generate KML
        /// </summary>
        /// <param name="connID">int --> connection ID</param>
        /// <param name="tableName">String --> Table name you want to fetch the mapping for</param>
        /// <returns>Mapping --> Mapping of specific table name</returns>
        public static Mapping getMapping(int connID, string tableName)
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
                        case "placemarkFieldName":
                            mapping.setPlacemarkFieldName(row[col].ToString());
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

        public static Mapping getDeepCopyOfMapping(int connID)
        {
            Mapping mapping = new Mapping();

            Database localDatabase = new Database();

            //Create mapping query and populate table
            string query = "SELECT * FROM Mapping WHERE connID='" + connID + "'";
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

    }
}
