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
            while (desc.IndexOf("[FIELD]", startIndex, StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                //if end of field tag is found, set startindex, else return false
                if (desc.IndexOf("[/FIELD]", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    //get the index of end of tag and calculate lengthoftag
                    endIndex = desc.IndexOf("[/FIELD]", startIndex, StringComparison.InvariantCultureIgnoreCase);
                    lengthOfTag = endIndex - startIndex;
                }
                else 
                { 
                    return false; 
                }
                
                //if start of table tag found look for end of field tag else return false
                if (desc.IndexOf("[TBL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    //if we find a close tbl tag before the open tbl tag return false
                    if ((desc.IndexOf("[/TBL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase)) < (desc.IndexOf("[TBL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase)))
                        return false;
                    //if we find another tbl tag, return false
                    else if (desc.IndexOf("[TBL]", desc.IndexOf("[TBL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, endIndex - desc.IndexOf("[TBL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, StringComparison.InvariantCultureIgnoreCase) != -1)
                        return false;
                    //else if we don't find a close tbl tag, return false
                    else if (desc.IndexOf("[/TBL]", desc.IndexOf("[TBL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, endIndex - desc.IndexOf("[TBL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, StringComparison.InvariantCultureIgnoreCase) == -1)
                        return false;
                    //else if the length of the table tag is 0, return false
                    else if (desc.IndexOf("[TBL]", startIndex, lengthOfTag) + 5 - desc.IndexOf("[/TBL]", desc.IndexOf("[TBL]") + 4, endIndex - desc.IndexOf("[TBL]", startIndex, lengthOfTag) + 4, StringComparison.InvariantCultureIgnoreCase) == 0)
                        return false;

                    //validate table name
                    int openTbl = desc.IndexOf("[TBL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase);
                    int closeTbl = desc.IndexOf("[/TBL]", desc.IndexOf("[TBL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, endIndex - desc.IndexOf("[TBL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, StringComparison.InvariantCultureIgnoreCase);
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
                if (desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    //if we find a close col tag before the open col tag return false
                    if ((desc.IndexOf("[/COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase)) < (desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase)))
                        return false;
                    //if we find another col tag, return false
                    if (desc.IndexOf("[COL]", desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, endIndex - desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, StringComparison.InvariantCultureIgnoreCase) != -1)
                        return false;
                    //else if we don't find a close col tag, return false
                    else if (desc.IndexOf("[/COL]", desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, endIndex - desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, StringComparison.InvariantCultureIgnoreCase) == -1)
                        return false;
                    //else if the length of the column tag is 0, return false
                    else if (desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 5 - desc.IndexOf("[/COL]", desc.IndexOf("[COL]", StringComparison.InvariantCultureIgnoreCase) + 4, endIndex - desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, StringComparison.InvariantCultureIgnoreCase) == 0)
                        return false;

                    //validate column name
                    int openCol = desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase);
                    int closeCol = desc.IndexOf("[/COL]", desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, endIndex - desc.IndexOf("[COL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 4, StringComparison.InvariantCultureIgnoreCase);
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
            while (desc.IndexOf("[URL]", startIndex, StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                //if end of url tag is found, set startindex, else return false
                if (desc.IndexOf("[/URL]", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    //get the index of end of tag and calculate lengthoftag
                    endIndex = desc.IndexOf("[/URL]", startIndex, StringComparison.InvariantCultureIgnoreCase);
                    lengthOfTag = endIndex - startIndex;
                }
                else
                {
                    return false;
                }
                //if start of title tag found look for end of title tag else return false
                if (desc.IndexOf("[TITLE]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    //if we find another title tag, return false
                    if (desc.IndexOf("[TITLE]", desc.IndexOf("[TITLE]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 7, endIndex - desc.IndexOf("[TITLE]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) - 7, StringComparison.InvariantCultureIgnoreCase) != -1)
                        return false;
                    //else if we don't find a close title tag, return false
                    else if (desc.IndexOf("[/TITLE]", desc.IndexOf("[TITLE]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 7, endIndex - desc.IndexOf("[TITLE]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) - 7, StringComparison.InvariantCultureIgnoreCase) == -1)
                        return false;
                    //else if the length of the title tag is 0, return false
                    else if (desc.IndexOf("[TITLE]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 8 - desc.IndexOf("[/TITLE]", desc.IndexOf("[TITLE]", StringComparison.InvariantCultureIgnoreCase) + 7, endIndex - desc.IndexOf("[TITLE]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) - 7, StringComparison.InvariantCultureIgnoreCase) == 0)
                        return false;
                }
                else
                {
                    return false;
                }
                //if length of url is 0 return false
                if (desc.IndexOf("[/TITLE]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 8 - desc.IndexOf("[/URL]", desc.IndexOf("[URL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) + 5, endIndex - desc.IndexOf("[URL]", startIndex, lengthOfTag, StringComparison.InvariantCultureIgnoreCase) - 5, StringComparison.InvariantCultureIgnoreCase) == 0)
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
            //get a copy of the orig string for resetting the descString after each iteration
            String descStringOrig = descString;
            //create arraylist for holding description for each row
            ArrayList descArray = new ArrayList();

            //for each row in the datatable
            foreach (DataRow row in inTable.Rows)
            {

                while (descString.IndexOf("[URL]", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    //get index of open and close url tag and calculate length of url 
                    int URLindex = descString.IndexOf("[URL]", StringComparison.InvariantCultureIgnoreCase);
                    int URLendIndex = descString.IndexOf("[/URL]", StringComparison.InvariantCultureIgnoreCase);
                    int length = URLendIndex - URLindex;
                    
                    //cut desc string into pre and post url tags. also removes the url tags from the desc string
                    String descString1 = descString.Substring(0,URLindex);
                    String descString2 = "";
                    if(URLendIndex + 6 != descString.Length)
                        descString2 = descString.Substring(URLendIndex + 6);
                    String URLstring = descString.Substring(URLindex + 5, length - 5);

                    //get index of open and close title tags and calculate length of title
                    int titleIndex = URLstring.IndexOf("[TITLE]");
                    int titleEndIndex = URLstring.IndexOf("[/TITLE]");
                    int titleLength = titleEndIndex - titleIndex;

                    //cut url string into text before and after title, and just the title text
                    //also removes the title tags from the url string
                    String URLsubString1 = URLstring.Substring(0, titleIndex);
                    String URLsubString2 = URLstring.Substring(titleEndIndex + 8);
                    String titleString = URLstring.Substring(titleIndex + 7, titleLength - 7);
                    
                    //create the final url from the url substrings and title string
                    String finalURL = "<a href=\"" + URLsubString1 + URLsubString2 + "\">" + titleString + "</a>";

                    //set the descString to the pre, url, and post strings
                    descString = descString1 + finalURL + descString2;
                }

                while (descString.IndexOf("[TBL/]", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    //get index of tbl tag and cut descString into pre and post tbl tags
                    int tblStart = descString.IndexOf("[TBL/]", StringComparison.InvariantCultureIgnoreCase);
                    string startToTbl = descString.Substring(0, tblStart);
                    string tblToEnd = "";
                    if(tblStart + 6 == descString.Length)
                        tblToEnd = descString.Substring(tblStart + 6);

                    //set the desc string to the pre, tableName, post tbl tag
                    descString = startToTbl + tableName + tblToEnd;
                }

                while (descString.IndexOf("[BR/]", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    //get index of br tag and cut descString into pre and post br tags
                    int brStart = descString.IndexOf("[BR/]", StringComparison.InvariantCultureIgnoreCase);
                    string startToBr = descString.Substring(0, brStart);
                    string brToEnd = "";
                    if(brStart + 5 == descString.Length)
                        brToEnd = descString.Substring(brStart + 5);

                    //set the descString to the pre, br tag, and post br tag
                    descString = startToBr + "<br />" + brToEnd;
                }

                while (descString.IndexOf("[FIELD]", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    //get index of open and close field tags and calculate length of field tag
                    int fieldIndex = descString.IndexOf("[FIELD]", StringComparison.InvariantCultureIgnoreCase);
                    int fieldEndIndex = descString.IndexOf("[/FIELD]", StringComparison.InvariantCultureIgnoreCase) + 8;
                    int fieldLength = fieldEndIndex - fieldIndex;

                    //cut descString into pre and post field tags. also removes field tags
                    String descString1 = descString.Substring(0, fieldIndex);
                    String descString2 = "";
                    if (fieldIndex != descString.Length)
                        descString2 = descString.Substring(fieldEndIndex);
                    String fieldString = descString.Substring(fieldIndex+7, fieldLength-7);

                    //get index of open and close tbl tags and calculate length of table
                    int tblIndex = fieldString.IndexOf("[TBL]", StringComparison.InvariantCultureIgnoreCase);
                    int tblEndIndex = fieldString.IndexOf("[/TBL]", StringComparison.InvariantCultureIgnoreCase);
                    int tblLength = tblEndIndex - tblIndex;

                    //get the table name and remove the tbl tags
                    String tblString = fieldString.Substring(tblIndex+5, tblLength-5);

                    //get index of open and close col tags and calculate length of column
                    int colIndex = fieldString.IndexOf("[COL]", StringComparison.InvariantCultureIgnoreCase);
                    int colEndIndex = fieldString.IndexOf("[/COL]", StringComparison.InvariantCultureIgnoreCase);
                    int colLength = colEndIndex - colIndex;

                    //get the column name and remove the col tags
                    String colString = fieldString.Substring(colIndex+5, colLength-5);
                    
                    //get the field value from the DataRow
                    fieldString = (String)row[colString.Trim()];

                    //set the descString to be the pre, field value, and post field string
                    descString = descString1 + fieldString + descString2;

                }

                //add the description to the arraylist
                descArray.Add(descString);

                //reset the descString to the original descString
                descString = descStringOrig;
            }
            return descArray;
        }
    }
}
