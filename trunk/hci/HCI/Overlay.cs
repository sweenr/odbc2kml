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
    public class Overlay
    {
        private string color;
        private Condition conditions;

        //Constructors
        public Overlay()
        {

        }

        public string getColor()
        {
            return this.color;
        }

        public void setColor(string color)
        {
            this.color = color;
        }

        public Condition getConditions()
        {
            return this.conditions;
        }

        public void setConditions(Condition con)
        {
            this.conditions = con;
        }

        public void removeCondition()
        {

        }
    }
}
