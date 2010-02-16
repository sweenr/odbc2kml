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
    public class Overlay
    {
        private string color;
        private ArrayList conditions;

        //Constructors
        public Overlay()
        {
            conditions = new ArrayList();
        }

        public string getColor()
        {
            return this.color;
        }

        public void setColor(string color)
        {
            this.color = color;
        }

        public ArrayList getConditions()
        {
            return this.conditions;
        }

        public void setConditions(Condition con)
        {
            this.conditions.Add(con);
        }

        public void removeCondition(int arrayPosition)
        {
            this.conditions.RemoveAt(arrayPosition);
        }
    }
}
