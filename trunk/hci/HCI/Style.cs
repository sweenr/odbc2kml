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
    public class Style
    {
        //Datatypes
        internal Icon icon;
        internal UInt64 color;
        internal String styleName;

        /// <summary>
        /// Basic constructor.
        /// </summary>
        public Style()
        {
            icon = new Icon();
            color = 0;
            styleName = "";
        }

        /// <summary>
        /// Constructor. Sets the icon, color, and style name. 
        /// </summary>
        /// <param name="icon">Icon --> Used for icon location when creating styles in KML</param>
        /// <param name="color">int --> Overlay color for KML</param>
        /// <param name="styleName">String --> unique style name</param>
        public Style(Icon icon, UInt64 color, String styleName)
        {
            this.icon = new Icon();
            this.icon.setId(icon.getId());
            this.icon.setLocality(icon.getLocality());
            this.icon.setLocation(icon.getLocation());

            foreach (Condition c in icon.getConditions())
            {
                this.icon.setConditions(c);
            }

            this.color = color;
            this.styleName = styleName;
        }

        ~Style()
        {
            icon = null;
            color = 0;
            styleName = null;
        }

        //return style icon
        public Icon getStyleIcon()
        {
            return this.icon;
        }

        //return style color
        public UInt64 getStyleColor()
        {
            return this.color;
        }

        //set style name
        public void setStyleName(String styleName)
        {
            this.styleName = styleName;
        }

        //set style icon
        public void setStyleIcon(Icon icon)
        {
            this.icon = icon;
        }

        //set style color
        public void setStyleColor(UInt64 color)
        {
            this.color = color;
        }

        //Return style name
        public String getStyleName()
        {
            return this.styleName;
        }


        //Overloaded operator used for comparisons in the set
        public override bool Equals(Object a)
        {
            Style aS = (Style)this;
            Style bS = (Style)a;

            if(aS.getStyleName().Equals(bS.getStyleName())) 
            {
                return true;
            }

            return false;
        }

        //Override of == for Styles
        public static bool operator ==(Style a, Style b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;

            if (a.getStyleName() == b.getStyleName())
            {
                return true;
            }

            return false;
        }

        //Override of != 
        public static bool operator !=(Style a, Style b)
        {
            return !(a == b);
        }
    }
}
