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
        public bool isValid()
        {
            bool valid = false;

            return valid;
        }
    }
}
