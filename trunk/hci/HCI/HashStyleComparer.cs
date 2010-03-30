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
using System.Collections.Generic;

namespace HCI
{
    public class HashStyleComparer : IEqualityComparer <Style>
    {
        public HashStyleComparer() { }
        public bool Equals(Style x, Style y)
        {
            if (x.getStyleName().Equals(y.getStyleName()))
            {
                return true;
            }

            return false;
    
   }
      //Not so relevant in this scenario. So just returning 0
        public int GetHashCode(Style x)
        {
            return x.getStyleName().GetHashCode();
        }
    } 
    
}
