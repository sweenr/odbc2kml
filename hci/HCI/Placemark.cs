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
    public class Placemark
    {
        //Datatypes
        private String name;
        private String styleName;
        private String description;
        private Double latitude;
        private Double longitude;

        /// <summary>
        /// Constructor. Requires a latitude and longitude. Also requires a
        /// description which can be blank and a name that does not have to be
        /// unique.
        /// </summary>
        /// <param name="latitude">double --> Latitude coordinate for placemark</param>
        /// <param name="longitude">double --> Longitude coordinate for placemark</param>
        /// <param name="description">String --> Description for this table/placemark</param>
        /// <param name="name">String --> Name of the placemark(what appears in the KML)</param>
        public Placemark(Double latitude, Double longitude, String description, String name)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.description = description;
            this.name = name;
        }

        ~Placemark()
        {
            latitude = 500;
            longitude = 500;
            description = null;
            name = null;
        }

        //Return placemark name
        public String getPlacemarkName()
        {
            return this.name;
        }

        //Return style name
        //Could be an empty string
        public String getPlacemarkStyleName()
        {
            return this.styleName;
        }

        //Return coordinate description
        public String getPlacemarkDescription()
        {
            return this.description;
        }

        //Return latitude coordinate
        public Double getPlacemarkLatitude()
        {
            return this.latitude;
        }

        //Return longitude coordinate
        public Double getPlacemarkLongitude()
        {
            return this.longitude;
        }

        //Sets style name
        public void setPlacemarkStyleName(String styleName)
        {
            this.styleName = styleName;
        }
    }
}
