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
//using HCI;

namespace HCI
{
    public class Description
    {
        internal string desc;

        //Constructors
        public Description()
        {
            this.desc = "";
        }

        public string getDesc()
        {
            return this.desc;
        }

        public void setDesc(string desc)
        {
            this.desc = desc;
        }

        /// <summary>
        /// Function to validate a description string. Checks URLs and Field tags. 
        /// URLs - verifies that there is one and only one TITLE element and that the title and URL are not empty, and that there is an open and closing tag
        /// Field tag - verifies that there one and only one set of TBL and COL tags, that the tags are not empty, and that there is an open and closing tag
        /// </summary>
        /// <param name="currentConnInfo">ConnInfo object containing the current connection info for the description being tested</param>
        /// <param name="currentMapping">Mapping object containing the current mapping for the description being tested</param>
        /// <returns>true if a description is valid and false if it is not</returns>
        public bool isValid(ConnInfo currentConnInfo, Mapping currentMapping)
        {
            //validate field tags
            int startIndex = 0;
            int endIndex = 0;
            int lengthOfTag = 0;
            //if start of field tag is found
            while (desc.IndexOf("[FIELD]", startIndex) != -1)
            {
                //if end of field tag is found, set startindex, else return false
                if (desc.IndexOf("[/FIELD]") != -1)
                {
                    //get the index of end of tag and calculate lengthoftag
                    endIndex = desc.IndexOf("[/FIELD]", startIndex);
                    lengthOfTag = endIndex - startIndex;
                }
                else 
                { 
                    return false; 
                }

                //if start of table tag found look for end of field tag else return false
                if (desc.IndexOf("[TBL]", startIndex, lengthOfTag) != -1)
                {
                    //if we find a close tbl tag before the open tbl tag return false
                    if ((desc.IndexOf("[/TBL]", startIndex, lengthOfTag)) < (desc.IndexOf("[TBL]", startIndex, lengthOfTag)))
                        return false;
                    //if we find another tbl tag, return false
                    else if (desc.IndexOf("[TBL]", desc.IndexOf("[TBL]", startIndex, lengthOfTag) + 4, endIndex - desc.IndexOf("[TBL]", startIndex, lengthOfTag)+4) != -1)
                        return false;
                    //else if we don't find a close tbl tag, return false
                    else if (desc.IndexOf("[/TBL]", desc.IndexOf("[TBL]", startIndex, lengthOfTag) + 4, endIndex - desc.IndexOf("[TBL]", startIndex, lengthOfTag)+4) == -1)
                        return false;
                    //else if the length of the table tag is 0, return false
                    else if(desc.IndexOf("[TBL]", startIndex, lengthOfTag)+5 - desc.IndexOf("[/TBL]", desc.IndexOf("[TBL]") + 4, endIndex - desc.IndexOf("[TBL]", startIndex, lengthOfTag)+4) == 0)
                        return false;

                    //validate table name
                    int openTbl = desc.IndexOf("[TBL]", startIndex, lengthOfTag);
                    int closeTbl = desc.IndexOf("[/TBL]", desc.IndexOf("[TBL]", startIndex, lengthOfTag) + 4, endIndex - desc.IndexOf("[TBL]", startIndex, lengthOfTag) + 4);
                    string table = desc.Substring(openTbl+5, closeTbl - openTbl - 5);
                    //if the table isn't the currently mapped table, return false
                    if (!table.Trim().Equals(currentMapping.tableName))
                        return false;
                }
                else
                {
                    return false;
                }
                //if start of column tag is found, look for end of field tag else return false
                if (desc.IndexOf("[COL]", startIndex, lengthOfTag) != -1)
                {
                    //if we find a close col tag before the open col tag return false
                    if ((desc.IndexOf("[/COL]", startIndex, lengthOfTag)) < (desc.IndexOf("[COL]", startIndex, lengthOfTag)))
                        return false;
                    //if we find another col tag, return false
                    if (desc.IndexOf("[COL]", desc.IndexOf("[COL]", startIndex, lengthOfTag) + 4, endIndex - desc.IndexOf("[COL]", startIndex, lengthOfTag)+4) != -1)
                        return false;
                    //else if we don't find a close col tag, return false
                    else if (desc.IndexOf("[/COL]", desc.IndexOf("[COL]", startIndex, lengthOfTag) + 4, endIndex - desc.IndexOf("[COL]", startIndex, lengthOfTag)+4) == -1)
                        return false;
                    //else if the length of the column tag is 0, return false
                    else if (desc.IndexOf("[COL]", startIndex, lengthOfTag) + 5 - desc.IndexOf("[/COL]", desc.IndexOf("[COL]") + 4, endIndex - desc.IndexOf("[COL]", startIndex, lengthOfTag) + 4) == 0)
                        return false;

                    //validate column name
                    int openCol = desc.IndexOf("[COL]", startIndex, lengthOfTag);
                    int closeCol = desc.IndexOf("[/COL]", desc.IndexOf("[COL]", startIndex, lengthOfTag) + 4, endIndex - desc.IndexOf("[COL]", startIndex, lengthOfTag) + 4);
                    string column = desc.Substring(openCol+5, closeCol - openCol - 5);
                    Database db = new Database(currentConnInfo);
                    string query = "SELECT " + column.Trim() + " FROM " + currentMapping.tableName;
                    try
                    {
                        db.executeQueryRemote(query);
                    }
                    catch (ODBC2KMLException)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                startIndex = endIndex + 8;
            }


            //validate url tags
            startIndex = 0;
            endIndex = 0;
            lengthOfTag = 0;
            //if start of url tag is found
            while (desc.IndexOf("[URL]", startIndex) != -1)
            {
                //if end of url tag is found, set startindex, else return false
                if (desc.IndexOf("[/URL]") != -1)
                {
                    //get the index of end of tag and calculate lengthoftag
                    endIndex = desc.IndexOf("[/URL]", startIndex);
                    lengthOfTag = endIndex - startIndex;
                }
                else
                {
                    return false;
                }
                //if start of title tag found look for end of title tag else return false
                if (desc.IndexOf("[TITLE]", startIndex, lengthOfTag) != -1)
                {
                    //if we find another title tag, return false
                    if (desc.IndexOf("[TITLE]", desc.IndexOf("[TITLE]", startIndex, lengthOfTag) + 7, endIndex - desc.IndexOf("[TITLE]", startIndex, lengthOfTag) + 7) != -1)
                        return false;
                    //else if we don't find a close title tag, return false
                    else if (desc.IndexOf("[/TITLE]", desc.IndexOf("[TITLE]", startIndex, lengthOfTag) + 7, endIndex - desc.IndexOf("[TITLE]", startIndex, lengthOfTag) + 7) == -1)
                        return false;
                    //else if the length of the title tag is 0, return false
                    else if (desc.IndexOf("[TITLE]", startIndex, lengthOfTag) + 8 - desc.IndexOf("[/TITLE]", desc.IndexOf("[TITLE]") + 7, endIndex - desc.IndexOf("[TITLE]", startIndex, lengthOfTag) + 7) == 0)
                        return false;
                }
                else
                {
                    return false;
                }
                //if length of url is 0 return false
                if(desc.IndexOf("[/TITLE]", startIndex, lengthOfTag) + 8 - desc.IndexOf("[/URL]", desc.IndexOf("[URL]", startIndex, lengthOfTag) + 5, endIndex-desc.IndexOf("[URL]", startIndex, lengthOfTag) + 5) == 0)
                    return false;

                startIndex = endIndex + 6;
            }


            return true;
        }

        public static Description getDescription(int connID)
        {
            //Basic Constructs
            Description description = new Description();
            Database localDatabase = new Database();
            DataTable table = null;
            description.setDesc("");
            //Create description query and populate table
            string query = "SELECT * FROM Description WHERE connID=" + connID;

            try
            {
                table = localDatabase.executeQueryLocal(query);
            }
            catch (ODBC2KMLException ex)
            {
                ex.errorText = "Error retreiving description from the local database";
                throw ex;
            }

            foreach (DataRow row in table.Rows)
            {
                description.setDesc(row["description"].ToString());
            }//End outer loop

            return description;
        }

        /// <summary>
        /// parses the description
        /// </summary>
        /// <param name="inTable"></param>
        /// <param name="descString"></param>
        /// <returns>ArrayList of parsed descriptions</returns>
        public static ArrayList parseDesc(DataTable inTable, String descString, String tableName)
        {
            String descStringOrig = descString;
            ArrayList descArray = new ArrayList();
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
                while(descString.Contains("[TBL/]"))
                {
                    //look at URL for explanation
                    descString = descString.Replace("[TBL/]", tableName);
                }
                while (descString.Contains("[BR/]"))
                {
                    descString = descString.Replace("[BR/]", @"<br />");
                }
     
                while (descString.Contains("[FIELD]"))
                {
                    //look at URL for explanation
                    int fieldIndex = descString.IndexOf("[FIELD]");
                    int fieldEndIndex = descString.IndexOf("[/FIELD]")+8;
                    int fieldLength = fieldEndIndex - fieldIndex;
                    String descString1 = descString.Substring(0, fieldIndex);
                    String descString2 = descString.Substring(fieldEndIndex);
                    String fieldString = descString.Substring(fieldIndex, fieldLength);
                    fieldString = fieldString.Replace("[FIELD]", "");
                    fieldString = fieldString.Replace("[/FIELD]", "");
                    while (fieldString.Contains("[BR/]"))
                    {
                        fieldString = fieldString.Replace("[BR/]", "");
                    }
                    if (fieldString.Contains("[TBL]") && fieldString.Contains("[COL]"))
                    {
                        int tblIndex = fieldString.IndexOf("[TBL]");
                        int tblEndIndex = fieldString.IndexOf("[/TBL]");
                        int tblLength = tblEndIndex - tblIndex;
                        
                        String tblString = fieldString.Substring(tblIndex, tblLength);
                        tblString = tblString.Replace("[TBL]", "");
                        tblString = tblString.Replace("[/TBL]", "");
                        int colIndex = fieldString.IndexOf("[COL]");
                        int colEndIndex = fieldString.IndexOf("[/COL]");
                        int colLength = colEndIndex - colIndex;
                        
                        String colString = fieldString.Substring(colIndex, colLength);
                        colString = colString.Replace("[COL]", "");
                        colString = colString.Replace("[/COL]", "");
                        fieldString = (String)row[colString];
                        descString = descString1 + fieldString + descString2;
                    }
                }
                descArray.Add(descString);
                descString = descStringOrig;
            }
            return descArray;
        }
    }
}
