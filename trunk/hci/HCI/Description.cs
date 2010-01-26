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
    public class Description
    {
        private string desc;

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
            string fieldValue;

            return fieldValue;
        }

        public string generateFieldName()
        {
            string fieldName;

            return fieldName;
        }

        public string generateImageLink()
        {
            string imageLink;

            return imageLink;
        }

        public string generateHref()
        {
            string href;

            return href;
        }
    }
}
