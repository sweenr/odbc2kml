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

//TODO: Add paths for additional classes

namespace HCI
{
    public class Connection
    {
        //Datatypes
        private Description description;
        private Mapping mapping;
        private ArrayList icons = new ArrayList();
        private ArrayList overlays = new ArrayList();
        private ConnInfo connInfo;

        //Functions

        //Constructor
        Connection()
        {

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
        public Icon getIcons()
        {
            return this.icons;
        }

        //Retrieve overlays
        public Overlay getOverlays()
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

        //Add Comments
        public bool openConn()
        {
            bool open = false;

            return open;
        }

        //TODO: ADD PARAMETERS TO ALL OF THE FOLLOWING

        //Add Comments
        public void closeConn() 
        {
        }

        //Add Comments
        public void saveConn()
        {
        }

        //Add Comments
        public void deleteConn()
        {
        }

        //Add Comments
        public void populateFields()
        {
        }

        //Add Comments
        public void retrieveRows()
        {
        }
    }
}
