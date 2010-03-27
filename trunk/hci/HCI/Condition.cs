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
        public static readonly int NOTEQUAL = 6;

        //Data Types
        private string fieldName;
        private string tableName;
        private string lowerBound;
        private string upperBound;
        private int lowerOperator;
        private int upperOperator;
        private int id;

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
            id = 0;
        }

        public Condition(string FieldName, string TableName, string LowerBound, string UpperBound, string LowerOperator, string UpperOperator)
        {
            fieldName = FieldName;
            tableName = TableName;
            lowerBound = LowerBound;
            upperBound = UpperBound;
            if (LowerOperator != "")
                lowerOperator = Convert.ToInt32(LowerOperator);
            else
                lowerOperator = NONE;
            if (UpperOperator != "")
                upperOperator = Convert.ToInt32(UpperOperator);
            else
                upperOperator = NONE;
            id = 0;
        }

        public static string operatorIntToString(int op)
        {
            if (op == 1)
                return "<";
            else if (op == 2)
                return "<=";
            else if (op == 3)
                return ">";
            else if (op == 4)
                return ">=";
            else if (op == 5)
                return "==";
            else if (op == 6)
                return "!=";
            else  // NONE or invalid
                return "";
        }

        public static int operatorStringToInt(string op)
        {
            if (op == "<")
                return 1;
            else if (op == "<=")
                return 2;
            else if (op == ">")
                return 3;
            else if (op == ">=")
                return 4;
            else if (op == "==")
                return 5;
            else if (op == "!=")
                return 6;
            else
                return 0;
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
        public string getLowerOperator()
        {
            return operatorIntToString(lowerOperator);
        }

        //Retrieve upperOperator
        public string getUpperOperator()
        {
            return operatorIntToString(upperOperator);
        }

        public string getId()
        {
            return this.id.ToString();
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

        public void setLowerOperator(string lowerOperator)
        {
            this.lowerOperator = operatorStringToInt(lowerOperator);
        }

        //Set upperOperator
        public void setUpperOperator(int upperOperator)
        {
            this.upperOperator = upperOperator;
        }

        public void setUpperOperator(string upperOperator)
        {
            this.upperOperator = operatorStringToInt(upperOperator);
        }

        public void setId(int ID)
        {
            this.id = ID;
        }
        /// <summary>
        /// below class will be used when editing IconConditions already in the database
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="iconID"></param>
        public void setIDfromDB(int connID, int iconID)
        {
            Database DB = new Database();
            DataTable id = new DataTable();
            try
            {
                 id = DB.executeQueryLocal("SELECT ID FROM IconCondition WHERE connID=" 
                     + connID + " and iconID=" + iconID + " "
                     + "and lowerBound=\'" + this.lowerBound + "\' "
                     + "and upperBound=\'" + this.upperBound + "\' " 
                     + "and lowerOperator=\'" + this.lowerOperator + "\' " 
                     + "and upperOperator=\'" + this.upperOperator + "\' " 
                     + "and fieldName=\'" + this.fieldName + "\' " 
                     + "and tableName=\'" + this.tableName + "\'");
            }
            catch (ODBC2KMLException ex)
            {
                throw new ODBC2KMLException(ex.errorText);
            }
            foreach (DataRow row in id.Rows)
            {
                this.id = (int)row[0];
            }
        }

        //Additional

        //Add Comments
        public string getErrorText()
        {
            string errorString = "";

            if (tableName == "")
                errorString = "Must enter table name.";
            else if (fieldName == "")
                errorString = "Must enter field name.";
            else if ((lowerBound == "") && (upperBound == ""))
                errorString = "Must enter a lower bound or an upper bound.";
            else if ((lowerOperator != 0) && (lowerBound == ""))
                errorString = "You entered a lower operator, so you must also enter a lower bound.";
            else if ((lowerBound != "") && (lowerOperator == 0))
                errorString = "You entered a lower bound, so you must also enter a lower operator.";
            else if ((upperOperator != 0) && (upperBound == ""))
                errorString = "You entered a upper operator, so you must also enter an upper bound.";
            else if ((upperBound != "") && (upperOperator == 0))
                errorString = "You entered a upper bound, so you must also enter an upper operator.";
            else if (((upperOperator == 5) && (lowerOperator != 0)) || ((lowerOperator == 5) && (upperOperator != 0)))
                errorString = "You cannot enter one \"==\" operator and also use another operator.";
            else if (((upperOperator == 6) && (lowerOperator != 0)) || ((lowerOperator == 6) && (upperOperator != 0)))
                errorString = "You cannot enter one \"!=\" operator and also use another operator.";
            
            return errorString;
        }

    }
}
