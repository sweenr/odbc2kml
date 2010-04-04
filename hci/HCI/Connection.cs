using System;
using System.Data;
using System.Configuration;
using System.Collections;
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
    public class Connection
    {
        //Datatypes
        internal Description description;
        internal Mapping mapping;
        internal ArrayList icons;
        internal ArrayList overlays;
        internal ConnInfo connInfo;
        internal int connID;

        //Functions

        //Constructor
        public Connection()
        {
            icons = new ArrayList();
            overlays = new ArrayList();
            description = new Description();
            mapping = new Mapping();
            connInfo = new ConnInfo();
        }

        public Connection(int connID)
        {
            this.connID = connID;
        }

        //Getters

        //Retrieve description
        public Description getDescription()
        {
            return this.description;
        }

        //Retrieve mapping
        public Mapping getMapping()
        {
            return this.mapping;
        }

        //Retrieve icons
        public ArrayList getIcons()
        {
            return this.icons;
        }

        //Retrieve overlays
        public ArrayList getOverlays()
        {
            return this.overlays;
        }

        //Retrieve connInfo
        public ConnInfo getConnInfo()
        {
            return this.connInfo;
        }

        //Setters

        //Set description
        public void setDescription(Description description)
        {
            this.description = description;
        }

        //Set mapping
        public void setMapping(Mapping mapping)
        {
            this.mapping = mapping;
        }

        //Set icons
        public void setIcons(Icon icon)
        {
            this.icons.Add(icon);
        }

        //Set overlays
        public void setOverlays(Overlay overlay)
        {
            this.overlays.Add(overlay);
        }

        //Retrieve connInfo
        public void setConnInfo(ConnInfo connInfo)
        {
            this.connInfo = connInfo;
        }

        //Add Comments
        public bool isValid()
        {
            bool valid = false;

            return valid;
        }

        //Additional

        //TODO: ADD PARAMETERS TO ALL OF THE FOLLOWING

        //Add Comments
        public void saveConn()
        {
        }

        //Add Comments
        public void deleteConn()
        {
        }

        /*
         * Populate fields uses the connID passed into the constructor
         * and retrieves all of the information about that specific connection
         * from the database. It will then set all of the classes variables
         * based on this information. 
         */
        public void populateFields()
        {
            try
            {
                this.connInfo = ConnInfo.getConnInfo(this.connID);
                this.overlays = Overlay.getOverlays(this.connID);
                this.description = Description.getDescription(this.connID);
                this.mapping = Mapping.getMapping(this.connID);                
                this.icons = Icon.getIcons(this.connID);
            }
            catch(ODBC2KMLException e) //Add whatever exceptions are needed and error handling code
            {
                throw e;
            }
        }
    }
}
