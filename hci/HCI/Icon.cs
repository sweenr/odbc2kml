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
    public class Icon
    {
        private string location;
        private ArrayList conditions;

        public Icon()
        {
            conditions = new ArrayList();
        }
       
        public string getLocation()
        {
            return this.location;
        }

        public void setLocation(string loc)
        {
            this.location = loc;
        }

        public ArrayList getConditions()
        {
            return this.conditions;
        }

        public void setConditions(Condition con)
        {
            this.conditions.Add(con);
        }

        public void removeConditions(int arrayPosition)
        {
            this.conditions.RemoveAt(arrayPosition);
        }
    }
}
