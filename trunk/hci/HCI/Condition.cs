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
    public class Condition
    {
        //Globals
        public static readonly int NONE = 0;
        public static readonly int LESSTHAN = 1;
        public static readonly int LESSTHANEQUAL = 2;
        public static readonly int GREATERTHAN = 3;
        public static readonly int GREATERTHANEQUAL = 4;
        public static readonly int EQUAL = 5;
        public static readonly int NOTEQUAL = 5;

        //Data Types
        private string fieldName;
        private string tableName;
        private string lowerBound;
        private string upperBound;
        private int lowerOperator;
        private int upperOperator;

        //Functions
        
        //Constructor
        public Condition()
        {
            fieldName = "";
            tableName = "";
            lowerBound = "";
            upperBound = "";
            lowerOperator = NONE;
            upperOperator = NONE;
        }

        //Getters

        //Retrieve fieldName
        public string getFieldName()
        {
            return this.fieldName;
        }

        //Retrieve tableName
        public string getTableName()
        {
            return this.tableName;
        }

        //Retrieve lowerBound
        public string getLowerBound()
        {
            return this.lowerBound;
        }

        //Retrieve upperBound
        public string getUpperBound()
        {
            return this.upperBound;
        }

        //Retrieve lowerOperator
        public int getLowerOperator()
        {
            return this.lowerOperator;
        }

        //Retrieve upperOperator
        public int getUpperOperator()
        {
            return this.upperOperator;
        }

        //Setters

        //Set fieldName
        public void setFieldName(string fieldName)
        {
            this.fieldName = fieldName;
        }

        //Set tableName
        public void setTableName(string tableName)
        {
            this.tableName = tableName;
        }

        //Set lowerBound
        public void setLowerBound(string lowerBound)
        {
            this.lowerBound = lowerBound;
        }

        //Set upperBound
        public void setUpperBound(string upperBound)
        {
            this.upperBound = upperBound;
        }

        //Set lowerOperator
        public void setLowerOperator(int lowerOperator)
        {
            this.lowerOperator = lowerOperator;
        }

        //Set upperOperator
        public void setUpperOperator(int upperOperator)
        {
            this.upperOperator = upperOperator;
        }

        //Additional

        //Add Comments
        public bool isValid()
        {
            bool valid = false;

            if ((fieldName != "") && (tableName != ""))
            {
                Database db = new Database();
                DataTable testTable = db.executeQueryLocal("SELECT " + fieldName + " FROM " + tableName);
                if (testTable.Columns.Count != 0)  // If the table and column/field names are valid, go on
                {
                    if (!((lowerOperator == NONE) && (upperOperator == NONE)))  // at least 1 operator is selected
                    {
                        if (lowerOperator != NONE)
                        {
                            if (lowerBound != "")
                            {
                                valid = true;
                            }
                        }
                        else if (upperOperator != NONE)
                        {
                            if (upperBound != "")
                            {
                                valid = true;
                            }
                        }
                    }
                }
            }

            return valid;
        }

    }
}
