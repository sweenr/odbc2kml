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

namespace ODBC2KML
{
    /// <summary>
    /// This is a helper class used with hash sets and styles.
    /// It ensures proper comparisons of styles when they are being placed
    /// in the hash set.
    /// </summary>
    public class HashStyleComparer : IEqualityComparer <Style>
    {
        //Default constructor
        public HashStyleComparer() { }

        //Comparison used for hash set
        public bool Equals(Style x, Style y)
        {
            if (x.getStyleName().Equals(y.getStyleName()))
            {
                return true;
            }

            return false;
    
   }
        //Not important, just set something
        public int GetHashCode(Style x)
        {
            return x.getStyleName().GetHashCode();
        }
    } 
    
}
