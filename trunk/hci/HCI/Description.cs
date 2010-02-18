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
    public class Description
    {
        private string desc;

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

    }
}
