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
        internal string tableName;
        internal string latFieldName;
        internal string longFieldName;
        internal string placemarkFieldName;
        internal int format;

        //Functions

        //Constructor
        public Mapping()
        {
            tableName = "";
            latFieldName = "";
            longFieldName = "";
            placemarkFieldName = "";
            format = 0;
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
            catch (Exception)
            {
                valid = false;
                return valid;
            }
            return valid;
        }

        public Boolean isValid(ConnInfo connInfo)
        {
            //If lat and long mappings are set
            if (this.getLatFieldName() != "" && this.getLongFieldName() != "")
            {
                if (this.getTableName() == "") //See if there is a table name
                {
                    //No table name
                    return false;
                }

                if (this.getFormat() != 1 && this.getFormat() != 2 && this.getFormat() != 3) //See if there is a format
                {
                    //No format
                    return false;
                }

                double tempDouble;
                Database db = new Database(connInfo);
                if (this.format == Mapping.SEPARATE)
                {
                    DataTable dt = db.executeQueryRemote("SELECT " + this.latFieldName + ", " + this.longFieldName + " FROM " + this.tableName);
                    foreach (DataRow dr in dt.Rows)
                    {
                        //if we can't parse a double from the column, return false
                        if (!Double.TryParse(dr[this.latFieldName].ToString(), out tempDouble) || !Double.TryParse(dr[this.longFieldName].ToString(), out tempDouble))
                        {
                            return false;
                        }
                    }
                }
                else if (this.format == Mapping.LATFIRST || this.format == Mapping.LONGFIRST)
                {
                    DataTable dt = db.executeQueryRemote("SELECT " + this.latFieldName + " FROM " + this.tableName);
                    foreach (DataRow dr in dt.Rows)
                    {
                        //try to separate the coordinates from the column, if it fails return false
                        try
                        {
                            double[] coords = separate(dr[this.latFieldName].ToString(), this.format);
                        }
                        catch (ODBC2KMLException ex)
                        {
                            return false;
                        }
                    }
                }

            }
            else if (this.getTableName() == "")//No lat and long
            {
                if (this.getTableName() != "") //See if there is a table name
                {
                    //Table name
                    return false;
                }

                if (this.getFormat() != Mapping.NONE) //See if format is set to default values
                {
                    //Not default
                    return false;
                }
            }
            else
            {
                try
                {
                    if (this.getTableName() != null)  // This will throw an exception if this is null
                    {
                        if (this.getLatFieldName() == null)
                        {
                            return false;
                        }

                        if (this.getFormat() == 0)
                        {
                            return false;
                        }
                    }
                }
                catch  // Catch the exception which means this is null, return false because null means not valid
                {
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
            
            p1 = cord.Substring(0,mid);

            p2 = cord.Substring(++mid, cord.Length - (mid-1) - 1);

            Char[] bs = new Char[8] {')', '>', '}', '\'', '\"', '(', '<' ,'{'};

            double q1 = 0;
            double q2 = 0;
            try
            {
                q1 = (double.Parse(p1.Trim(bs)));
                q2 = (double.Parse(p2.Trim(bs)));
      
                double[] coordinates = new double[2];
                coordinates[0] = q1;
                coordinates[1] = q2;

                return coordinates;

            }
            catch (Exception)
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
            DataTable table = null;

            //Create mapping query and populate table
            string query = "SELECT * FROM Mapping WHERE connID=" + connID;

            try
            {
                table = localDatabase.executeQueryLocal(query);
            }
            catch (ODBC2KMLException ex)
            {
                ex.errorText = "Error selecting the mapping from the database";
                throw ex;
            }

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
    }
}
