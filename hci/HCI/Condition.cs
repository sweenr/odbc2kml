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

namespace HCI
{
    public class Condition
    {
        //Globals
        static public const int NONE = 0;
        static public const int LESSTHAN = 1;
        static public const int LESSTHANEQUAL = 2;
        static public const int GREATERTHAN = 3;
        static public const int GREATERTHANEQUAL = 4;
        static public const int EQUAL = 5;
        static public const int NOTEQUAL = 5;

        //Data Types
        private string fieldName;
        private string tableName;
        private string lowerBound;
        private string upperBound;
        private int lowerOperator;
        private int upperOperator;

        //Functions
        
        //Constructor
        Condition()
        {

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

            return valid;
        }

    }
}
