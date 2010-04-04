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
    public class Description
    {
        internal string desc;

        //Constructors
        public Description()
        {

        }

        public string getDesc()
        {
            return this.desc;
        }

        public void setDesc(string desc)
        {
            this.desc = desc;
        }

        public bool isValid()
        {
            bool valid = false;

            return valid;
        }

        public string generateFieldValue()
        {
            string fieldValue = "test";

            return fieldValue;
        }

        public string generateFieldName()
        {
            string fieldName = "test";

            return fieldName;
        }

        public string generateImageLink()
        {
            string imageLink = "test";

            return imageLink;
        }

        public string generateHref()
        {
            string href = "test";

            return href;
        }

        public static void insertDescription(int connID, string desc)
        {
            Description description = new Description();
            Database localDatabase = new Database();

            string query = "INSERT INTO DESCRIPTION VALUES ('" + connID + "', '" + desc + "')";
            localDatabase.executeQueryLocal(query);
        } 

        public static void updateDescription(int connID, string desc)
        {
            Description description = new Description();
            Database localDatabase = new Database();

            string query = "UPDATE DESCRIPTION SET description = '" + desc + "' WHERE connID = '" + connID + "'";
            localDatabase.executeQueryLocal(query);
        } 

        public static Description getDescription(int connID)
        {
            Description description = new Description();
            Database localDatabase = new Database();

            //Create description query and populate table
            string query = "SELECT * FROM Description WHERE connID=" + connID;
            DataTable table = localDatabase.executeQueryLocal(query);

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    //Set Description
                    if (col.ColumnName == "description")
                    {
                        description.setDesc(row[col].ToString());
                    }
                }
            }//End outer loop

            return description;
        }

        //public static ArrayList parseDesc(int connID)
        /// <summary>
        /// parses the description
        /// </summary>
        /// <param name="inTable"></param>
        /// <param name="descString"></param>
        /// <returns>ArrayList of parsed descriptions</returns>
        public static ArrayList parseDesc(DataTable inTable, String descString, String tableName)
        {
            //Database DB = new Database();
            //DataTable mapping = DB.executeQueryLocal("SELECT 'tableName' FROM Mapping WHERE connID=\'" + connID + "\'");
            //ArrayList tablesToBeSearched = null;
            //foreach(DataRow row in mapping.Rows)
            //{
            //    foreach(DataColumn col in mapping.Columns)
            //    {
            //        tablesToBeSearched.Add(row[col].ToString());
            //    }
            //}
            //DB.setConnInfo(ConnInfo.getConnInfo(connID));
            //int dbType = ConnInfo.getConnInfo(connID).getDatabaseType();
            //DataTable desc = DB.executeQueryLocal("SELECT 'description' FROM Description WHERE connID=\'" + connID + "\'");
            //String descString = desc.ToString();
            ArrayList descArray = new ArrayList();
            //foreach (String tableName in tablesToBeSearched)
            //{
            //    DataTable remote = null;
            //    if(dbType==ConnInfo.MSSQL)
            //    {
            //        remote = DB.executeQueryRemote("SELECT * FROM " + tableName);
            //    }
            //    else if(dbType==ConnInfo.MYSQL)
            //    {
            //        remote = DB.executeQueryRemote("SELECT * FROM " + tableName + ";");
            //    }
            //    else if(dbType==ConnInfo.ORACLE)
            //    {
            //        remote = DB.executeQueryRemote("SELECT * FROM \"" + tableName + "\"");
            //    }
            //    int rowCount=0;
                //foreach(DataRow row in remote.Rows)
            foreach (DataRow row in inTable.Rows)
            {
                while (descString.Contains("[URL]"))
                {
                    //explanation for all steps below, get index of open and close brackets
                    //length is the distance from the first index to second
                    //descStrings are substrings before the open bracket and after the close bracket
                    //URL string is the information
                    //URL is parsed for TITLE using the above algorithm
                    //URL is changed to a correct URL output string
                    //descString1 and 2 are concatenated to the beginning and end of URL respectively
                    int URLindex = descString.IndexOf("[URL]");
                    int URLendIndex = descString.IndexOf("[/URL]");
                    int length = URLendIndex - URLindex;
                    String descString1 = descString.Substring(0,URLindex);
                    String descString2 = descString.Substring(URLendIndex+6);
                    String URLstring = descString.Substring(URLindex+5, length-5);
                    while (URLstring.Contains("[BR/]"))
                    {
                        URLstring = URLstring.Replace("[BR/]", "");
                    }
                    URLstring = URLstring.Replace("[URL]", "");
                    URLstring = URLstring.Replace("[/URL]", "");
                    String finalURL = "";
                    if (URLstring.Contains("[TITLE]"))
                    {
                        int titleIndex = URLstring.IndexOf("[TITLE]");
                        int titleEndIndex = URLstring.IndexOf("[/TITLE]");
                        int titleLength = titleEndIndex - titleIndex;
                        String URLsubString1 = URLstring.Substring(0, titleIndex);
                        String URLsubString2 = URLstring.Substring(titleEndIndex+8);
                        String titleString = URLstring.Substring(titleIndex+7, titleLength-7);
                        titleString = titleString.Replace("[TITLE]", "");
                        titleString = titleString.Replace("[/TITLE]", "");
                        finalURL = "<a href=\"" + URLsubString1 + URLsubString2 + "\">"
                            + titleString + "</a>";
                        if (finalURL.Contains("[TITLE]"))
                        {
                            throw new ODBC2KMLException("URL contains to many Titles\n" + finalURL);
                        }
                    }
                    else
                    {
                        finalURL = "<a href\"" + URLstring + "\">" + URLstring + "</a>";
                    }
                    descString = descString1 + finalURL + descString2;
                }
                while(descString.Contains("[TBL /]"))
                {
                    //look at URL for explanation
        //below has been changed and can be safely deleted if not used in the future
                    //int tableIndex = descString.IndexOf("[TABLE]");
                    //int tableEndIndex = descString.IndexOf("[/TABLE]");
                    //String descString1 = descString.Substring(0,tableIndex);
                    //String descString2 = descString.Substring(tableEndIndex);
                    //descString = descString1 + tableName + descString2;
                    descString = descString.Replace("[TBL /]", tableName);
                }
                while (descString.Contains("[BR/]"))
                {
                    descString = descString.Replace("[BR/]", @"<br />");
                }
     //field parsing is incomplete and commented out to prevent infinite looping until
     //the correct procedure for parsing a FIELD value is identified.
                //while(descString.Contains("[FIELD]"))
                //{
                //    //look at URL for explanation
                //    int fieldIndex = descString.IndexOf("[FIELD]");
                //    int fieldEndIndex = descString.IndexOf("[/FIELD]");
                //    int fieldLength = fieldEndIndex - fieldIndex;
                //    String descString1 = descString.Substring(0,fieldIndex);
                //    String descString2 = descString.Substring(fieldEndIndex);
                //    String fieldString = descString.Substring(fieldIndex, fieldLength);
                //    if(fieldString.Contains("[TBL]") && fieldString.Contains("[COL]"))
                //    {
                //        int tblIndex = fieldString.IndexOf("[TBL]");
                //        int tblEndIndex = fieldString.IndexOf("[/TBL]");
                //        int tblLength = tblEndIndex - tblIndex;
                //        //String fieldString1 = fieldString.Substring(0,tblIndex);
                //        //String fieldString2 = fieldString.Substring(tblEndIndex);
                //        String tblString = fieldString.Substring(tblIndex, tblLength);
                //        tblString = tblString.Replace("[TBL]", "");
                //        tblString = tblString.Replace("[/TBL]", "");
                //        int colIndex = fieldString.IndexOf("[COL]");
                //        int colEndIndex = fieldString.IndexOf("[/COL]");
                //        int colLength = colEndIndex - colIndex;
                //        //String fieldString1 = fieldString.Substring(0,tblIndex);
                //        //String fieldString2 = fieldString.Substring(tblEndIndex);
                //        String colString = fieldString.Substring(colIndex, colLength);
                //        colString = colString.Replace("[COL]", "");
                //        colString = colString.Replace("[/COL]", "");
                //        //DataTable remote2;
                //        //below isn't finished
                //        //if(dbType == ConnInfo.MSSQL)
                //        //{
                //        //        remote2 = DB.executeQueryRemote("SELECT * FROM " + tblString + "WHERE" );
                //        //}
                //        //else if(dbType == ConnInfo.MYSQL)
                //        //{
                //        //        remote2 = DB.executeQueryRemote("SELECT * FROM " + tableName + ";");
                //        //}
                //        //else if(dbType == ConnInfo.ORACLE)
                //        //{
                //        //        remote2 = DB.executeQueryRemote("SELECT \"" + colString +"\" FROM \"" + tableName + "\" WHERE rownum=" + rowCount);
                //        //}
                //    }
                //    if(fieldString.Contains("[TBL]") && !fieldString.Contains("[COL]"))
                //    {
                //        throw new ODBC2KMLException("Description doesn't contain field Column information");
                //    }
                //    if(!fieldString.Contains("[TBL]") && fieldString.Contains("[COL]"))
                //    {
                //        throw new ODBC2KMLException("Description doesn't contain field Table information");
                //    }
                //}
  //below is commented out because imageGenWebService has been assigned to low priority
                //while (descString.Contains("[IMAGE]"))
                //{
                //    //look at URL for explanation
                //    int imageIndex = descString.IndexOf("[IMAGE]");
                //    int imageEndIndex = descString.IndexOf("[/IMAGE]");
                //    int imageLength = imageEndIndex - imageIndex;
                //    String descString1 = descString.Substring(0, imageIndex);
                //    String descString2 = descString.Substring(imageEndIndex);
                //    String imageString = descString.Substring(imageIndex, imageLength);
                //    imageString = imageString.Replace("[IMAGE]", "");
                //    imageString = imageString.Replace("[/IMAGE]", "");
                //    if (imageString.Contains("[TBL]") && imageString.Contains("[COL]"))
                //    {
                //        int tblIndex = imageString.IndexOf("[TBL]");
                //        int tblEndIndex = imageString.IndexOf("[/TBL]");
                //        int tblLength = tblEndIndex - tblIndex;
                //        String tblString = imageString.Substring(tblIndex, tblLength);
                //        tblString = tblString.Replace("[TBL]", "");
                //        tblString = tblString.Replace("[/TBL]", "");
                //        int colIndex = imageString.IndexOf("[COL]");
                //        int colEndIndex = imageString.IndexOf("[/COL]");
                //        int colLength = colEndIndex - colIndex;
                //        String colString = imageString.Substring(colIndex, colLength);
                //        colString = colString.Replace("[COL]", "");
                //        colString = colString.Replace("[/COL]", "");
                //        //imageString = "<img src=\"./ImageWebSVC.asmx/getImage?connID="
                //        //    + connID + "&table="
                //        //    + tblString + "&field="
                //        //    + colString + "&row="
                //        //    + rowCount + "\" />";
                //        imageString = "this function doesn't work right now";
                //    }
                //    if (imageString.Contains("[TBL]") && !imageString.Contains("[COL]"))
                //    {
                //        throw new ODBC2KMLException("Description doesn't contain Image Column information");
                //    }
                //    if (!imageString.Contains("[TBL]") && imageString.Contains("[COL]"))
                //    {
                //        throw new ODBC2KMLException("Description doesn't contain Image Table information");
                //    }
                //    descString = descString1 + imageString + descString2;
                //}
                //rowCount++;
                descArray.Add(descString);
            }
                //String URLstringFinal = @"<a href='" +
                //finish above line
                //String[] tempDescString = descString.Split("[URL]");
                //String[] URLstring = tempDescString[1].Split("[/URL]");
                ////tempDescString[1] = URLstring[1];
                //if (descString.Contains("[TITLE]"))
                //{
                //    String[] tempTitle = URLstring[0].Split("[TITLE]");
                //    String[] tempTitle2 = tempTitle[1].Split("[/TITLE]");
                //    String title = tempTitle2[0];
                //}
            //the above is where the string will be split to begin parsing the description
            //descArray.Add(descString);
            return descArray;
        }
    }
}
