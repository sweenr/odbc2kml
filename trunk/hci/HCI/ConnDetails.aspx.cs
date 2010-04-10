using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Text;
using HCI;
using OboutInc.ColorPicker;

namespace HCI
{
    public partial class ConnDetails : System.Web.UI.Page
    {
        
        // Absolute path where icons are stored
        public static String fileSaveLoc = "";
        
        // Relative path to where icons are stored
        public static String relativeFileSaveLoc = @"/icons/";
        
        // Bool set to true after initial page load
        internal bool alreadySetupLists = false;
        
        internal ArrayList iconListAvailableToAdd = new ArrayList();
        
        internal ArrayList iconListAvailableToRemove = new ArrayList();
        
        internal ArrayList overlayListAvailableToRemove = new ArrayList();
        
        internal ArrayList iconList = new ArrayList();
        
        internal ArrayList overlayList = new ArrayList();
        
        internal int tempId = -1;
        
        internal int curOverlayCount = -1;
        
        internal SqlDataSource MSQLTables = new SqlDataSource();
        internal SqlDataSource SQLTables = new SqlDataSource();
        internal SqlDataSource oracleTables = new SqlDataSource();
        internal SqlDataSource colDataSource = new SqlDataSource();

        internal Connection conn = new Connection();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                fileSaveLoc = Server.MapPath("/icons/");
                dFieldPanel.Visible = false;

                if (!IsPostBack)
                {
                    //No ID, redirect to main
                    if (Request.QueryString.Get("ConnID") == null)
                    {
                        Response.Redirect("Main.aspx");
                        return;
                    }
                    else
                    {
                        //Grab and parse connection ID
                        bool isValidConnId = int.TryParse(Request.QueryString.Get("ConnID"),out conn.connID);
                        if (!isValidConnId)
                        {
                            Response.Redirect("Main.aspx");
                        }

                        try
                        {
                            conn.populateFields();
                        }
                        catch (ODBC2KMLException)
                        {
                            ErrorHandler eh = new ErrorHandler("There was an error retreiving connection information", errorPanel1);
                            eh.displayError();
                            return;
                        }

                        ColorAddText.Style["background-color"] = HiddenValue.Value = "#FFFFFF";
                        curOverlayCount = -1;

                        odbcAdd.Text = conn.connInfo.getServerAddress();
                        odbcDName.Text = conn.connInfo.getDatabaseName();
                        odbcName.Text = conn.connInfo.getConnectionName();
                        odbcPass.Attributes.Add("value", conn.connInfo.getPassword());
                        odbcPN.Text = conn.connInfo.getPortNumber();
                        odbcUser.Text = conn.connInfo.getUserName();
                        odbcProtocol.Text = conn.connInfo.getOracleProtocol();
                        odbcSName.Text = conn.connInfo.getOracleServiceName();
                        odbcSID.Text = conn.connInfo.getOracleSID();

                        updateSqlDataSources();

                        if (!alreadySetupLists)
                        {
                            try
                            {
                                fillIconLibraryLists();
                                fillOverlayLibraryLists();
                                fillIconListFromDatabase();
                                alreadySetupLists = true;
                            }
                            catch (ODBC2KMLException)
                            {
                                ErrorHandler eh = new ErrorHandler("There was an error populating the icon and overlay lists", errorPanel1);
                                eh.displayError();
                                return;
                            }
                        }

                        //editor insertion
                        string connectionString_editor = "";
                        string providerName_editor = "";

                        //Set Table Datasources & fill in gridview/boxes
                        if (conn.connInfo.getDatabaseType() == ConnInfo.MSSQL)
                        {
                            connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Initial Catalog=" + conn.connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword();
                            MSQLTables_Mapping.ConnectionString = connectionString_editor;
                            MSQLTables_Mapping.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' AND TABLE_NAME != 'sysdiagrams'";
                        }

                        else if (conn.connInfo.getDatabaseType() == ConnInfo.MYSQL)
                        {
                            connectionString_editor = "server=" + conn.connInfo.getServerAddress() + ";User Id=" + conn.connInfo.getUserName() + ";password=" + conn.connInfo.getPassword() + ";Persist Security Info=True;database=" + conn.connInfo.getDatabaseName();
                            providerName_editor = "MySql.Data.MySqlClient";
                            SQLTables_Mapping.ConnectionString = connectionString_editor;
                            SQLTables_Mapping.ProviderName = providerName_editor;
                            SQLTables_Mapping.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA != 'mysql'";
                        }

                        else if (conn.connInfo.getDatabaseType() == ConnInfo.ORACLE)
                        {
                            connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword() + ";Unicode=True";
                            providerName_editor = "System.Data.OracleClient";
                            oracleTables_Mapping.ConnectionString = connectionString_editor;
                            oracleTables_Mapping.ProviderName = providerName_editor;
                            oracleTables_Mapping.SelectCommand = "select TABLE_NAME from user_tables";
                        }

                        else
                        {
                            throw new ODBC2KMLException("Unknown database type.");
                        }

                        updateTables(conn.connInfo.getDatabaseType());


                        descriptionBox.Text = conn.description.getDesc();

                        //show the current mapping box
                        displayCurrentMapping();

                        sessionSave();
                    }
                }

                sessionLoad();
                ColorPicker1.InitialColor = "-111111";
                fillIconLibraryPopup();
                fillIconLibraryPopupRemove();

                fillOverlayPopupRemove();

                genIconConditionTable(sender, e);
                genOverlayConditionTable(sender, e);

                sessionSave();

                if (Request.QueryString.Get("locked") != "false")
                {
                    LockPage();
                }
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
        }

        internal void sessionLoad()
        {
            if (Session["curOverlayCount"] == null)
            {
                curOverlayCount = -1;
                tempId = -1;
                try
                {
                    fillIconLibraryLists();
                    fillOverlayLibraryLists();
                    fillIconListFromDatabase();
                    alreadySetupLists = true;
                }
                catch (ODBC2KMLException ex)
                {
                    throw ex;
                }
                sessionSave();
                return;
            }
            curOverlayCount = (int)Session["curOverlayCount"];
            conn = (Connection)Session["Connection"];
            alreadySetupLists = (bool)Session["alreadySetupLists"];
            iconList = (ArrayList)Session["iconList"];
            iconListAvailableToAdd = (ArrayList)Session["iconListAvailableToAdd"];
            iconListAvailableToRemove = (ArrayList)Session["iconListAvailableToRemove"];
            overlayListAvailableToRemove = (ArrayList)Session["overlayListAvailableToRemove"];
            overlayList = (ArrayList)Session["overlayList"];
            MSQLTables = (SqlDataSource)Session["MSQLTables"];
            SQLTables = (SqlDataSource)Session["SQLTables"];
            oracleTables = (SqlDataSource)Session["oracleTables"];
            colDataSource = (SqlDataSource)Session["colDataSource"];
            tempId = (int)Session["tempId"];
        }

        internal void sessionSave()
        {
            //Session.Clear();
            Session["curOverlayCount"] = curOverlayCount;
            Session["Connection"] = conn;
            Session["alreadySetupLists"] = alreadySetupLists;
            Session["iconList"] = iconList;
            Session["iconListAvailableToAdd"] = iconListAvailableToAdd;
            Session["iconListAvailableToRemove"] = iconListAvailableToRemove;
            Session["overlayListAvailableToRemove"] = overlayListAvailableToRemove;
            Session["overlayList"] = overlayList;
            Session["MSQLTables"] = MSQLTables;
            Session["SQLTables"] = SQLTables;
            Session["oracleTables"] = oracleTables;
            Session["colDataSource"] = colDataSource;
            Session["tempId"] = tempId;
        }

        /// <summary>
        /// The following code is taken from http://www.codeproject.com/KB/aspnet/Enable_Disable_Controls.aspx
        /// which is distributed under the CPOL license.
        /// Modified for use in ODBC2KML
        /// </summary>
        internal void LockPage()
        {
            addLatLong.Visible = false;
            addPlacemarkField.Visible = false;
            descriptionBox.Enabled = false;
            cancel.Text = "Return";

            foreach (Control c in Page.Controls)
            {
                foreach (Control ctrl in c.Controls)
                {
                    if (ctrl is TextBox)
                    {
                        ((TextBox)ctrl).Enabled = false;
                    }
                    else if (ctrl is Button)
                    {
                        if(ctrl.ClientID != cancel.ClientID)
                            ((Button)ctrl).Visible = false;
                    }
                    else if (ctrl is RadioButton)
                    {
                        ((RadioButton)ctrl).Enabled = false;
                    }
                    else if (ctrl is ImageButton)
                    {
                        if (ctrl.ClientID != homeIcon.ClientID)
                            ((ImageButton)ctrl).Enabled = false;
                    }
                    else if (ctrl is CheckBox)
                    {
                        ((CheckBox)ctrl).Enabled = false;
                    }
                    else if (ctrl is DropDownList)
                    {
                        ((DropDownList)ctrl).Enabled = false;
                    }
                    else if (ctrl is HyperLink)
                    {
                        ((HyperLink)ctrl).Enabled = false;
                    }
                }
            }
        }

        protected void updateConnection(object sender, EventArgs e)
        {
            invalidConnInfo.Visible = false;
            unableToConnect.Visible = false;
            connectionEstablished.Visible = false;

            conn.connInfo.connectionName = odbcName.Text;
            conn.connInfo.serverAddress = odbcAdd.Text;
            conn.connInfo.portNumber = odbcPN.Text;
            conn.connInfo.databaseName = odbcDName.Text;
            conn.connInfo.userName = odbcUser.Text;
            conn.connInfo.password = odbcPass.Text;
            conn.connInfo.databaseType = odbcDBType.SelectedIndex;
            conn.connInfo.oracleProtocol = odbcProtocol.Text;
            conn.connInfo.oracleServiceName = odbcSName.Text;
            conn.connInfo.oracleSID = odbcSID.Text;


            String connectionString = "";

            try
            {
                //If the connection is invalid, return false, else purge invalid conditions and return true
                if (!conn.validateConnnection(descriptionBox))
                {
                    ErrorHandler eh;

                    if (conn.connInfo.databaseType == ConnInfo.ORACLE)
                    {
                        eh = new ErrorHandler("Connection information is invalid. Please verify that all fields are correct and that the oracle specific fields have a value.", errorPanel1);
                    }
                    else
                    {
                        eh = new ErrorHandler("Connection information is invalid. Please verify that all fields are correct.", errorPanel1);
                    }

                    eh.displayError();
                    return;
                }

                if (conn.connInfo.getDatabaseType() == ConnInfo.MSSQL)
                {
                    connectionString = "Data Source=" + conn.connInfo.getServerAddress() + ";Initial Catalog=" + conn.connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword();
                    MSQLTables_Mapping.ConnectionString = connectionString;
                    MSQLTables_Mapping.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' AND TABLE_NAME != 'sysdiagrams'";
                }

                else if (conn.connInfo.getDatabaseType() == ConnInfo.MYSQL)
                {
                    String providerName = "";
                    connectionString = "server=" + conn.connInfo.getServerAddress() + ";User Id=" + conn.connInfo.getUserName() + ";password=" + conn.connInfo.getPassword() + ";Persist Security Info=True;database=" + conn.connInfo.getDatabaseName();
                    providerName = "MySql.Data.MySqlClient";
                    SQLTables_Mapping.ConnectionString = connectionString;
                    SQLTables_Mapping.ProviderName = providerName;
                    SQLTables_Mapping.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA != 'mysql'";
                }

                else if (conn.connInfo.getDatabaseType() == ConnInfo.ORACLE)
                {
                    String providerName = "";
                    connectionString = "Data Source=" + conn.connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword() + ";Unicode=True";
                    providerName = "System.Data.OracleClient";
                    oracleTables_Mapping.ConnectionString = connectionString;
                    oracleTables_Mapping.ProviderName = providerName;
                    oracleTables_Mapping.SelectCommand = "select TABLE_NAME from user_tables";
                }

                else
                {
                    //Display Error Handler, something crashed....
                    ErrorHandler eh = new ErrorHandler("An unexpected error occured, please verify your connection and connection information and try again.", errorPanel1);
                    eh.displayError();
                    return;
                }

                updateTables(conn.connInfo.getDatabaseType());

                updateSqlDataSources();
                genIconConditionTable(sender, e);
                genOverlayConditionTable(sender, e);
                displayCurrentMapping();

                sessionSave();
               // genIconConditionTable(sender, e);
               // genOverlayConditionTable(sender, e);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            catch (Exception)
            {
                ErrorHandler eh = new ErrorHandler("An unexpected error occured, please check your database connection.", errorPanel1);
                eh.displayError();
                return;
            }
              
        }

        protected void updateSqlDataSources()
        {
            string connectionString = "";
            string providerName = "";
            //Set drop down box accordingly
            if (conn.connInfo.databaseType == ConnInfo.MSSQL)
            {
                odbcDBType.SelectedValue = "MSSQL";
                connectionString = "Data Source=" + conn.connInfo.getServerAddress() + ";Initial Catalog=" + conn.connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword();
                MSQLTables.ConnectionString = connectionString;
                MSQLTables.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' AND TABLE_NAME != 'sysdiagrams'";
            }
            else if (conn.connInfo.databaseType == ConnInfo.MYSQL)
            {
                odbcDBType.SelectedValue = "MySQL";
                connectionString = "server=" + conn.connInfo.getServerAddress() + ";User Id=" + conn.connInfo.getUserName() + ";password=" + conn.connInfo.getPassword() + ";Persist Security Info=True;database=" + conn.connInfo.getDatabaseName();
                providerName = "MySql.Data.MySqlClient";
                SQLTables.ConnectionString = connectionString;
                SQLTables.ProviderName = providerName;
                SQLTables.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA != 'mysql'";
            }
            else if (conn.connInfo.databaseType == ConnInfo.ORACLE)
            {
                odbcDBType.SelectedValue = "Oracle";
                connectionString = "Data Source=" + conn.connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword() + ";Unicode=True";
                providerName = "System.Data.OracleClient";
                oracleTables.ConnectionString = connectionString;
                oracleTables.ProviderName = providerName;
                oracleTables.SelectCommand = "SELECT TABLE_NAME FROM user_tables";
            }
            else
            {
                throw new ODBC2KMLException("Unknown database type.");
            }
        }

        protected void fillOverlayLibraryLists()
        {
            overlayList.Clear();
            overlayListAvailableToRemove.Clear();

            foreach (Overlay o in conn.overlays)
            {
                overlayList.Add(new Overlay(o));
                overlayListAvailableToRemove.Add(new Overlay(o));
            }

            sessionSave();
        }

        protected void addSingleIconToLib(String path)
        {
            Database db = new Database();
            DataTable dt;

            try
            {
                dt = db.executeQueryLocal("SELECT ID, location FROM IconLibrary WHERE location=\'" + path + "\'");
            }
            catch (ODBC2KMLException ex)
            {
                throw ex;
            }

            foreach (DataRow dr in dt.Rows)
            {
                string iconId = dr["ID"].ToString();
                string iconLoc = dr["location"].ToString();
                Icon icon = new Icon();
                icon.setId(iconId);
                icon.setLocation(iconLoc);
                iconListAvailableToAdd.Add(icon);
            }
            fillIconLibraryPopup();
            sessionSave();
        }

        protected void fillIconLibraryLists()
        {
            iconListAvailableToAdd.Clear();
            iconListAvailableToRemove.Clear();

            Database db = new Database();
            DataTable dt;

            try
            {
                dt = db.executeQueryLocal("SELECT ID, location FROM IconLibrary AS IL WHERE (NOT EXISTS (SELECT ID, connID FROM Icon AS IC WHERE (connID = " + Request.QueryString.Get("ConnID") + " ) AND (ID = IL.ID)))");
            }
            catch (ODBC2KMLException ex)
            {
                throw ex;
            }
            
            foreach (DataRow dr in dt.Rows)
            {
                string iconId = dr["ID"].ToString();
                string iconLoc = dr["location"].ToString();
                Icon icon = new Icon();
                icon.setId(iconId);
                icon.setLocation(iconLoc);
                iconListAvailableToAdd.Add(icon);
            }

            Database db2 = new Database();
            DataTable dt2;
            try
            {
                dt2 = db2.executeQueryLocal("SELECT IconLibrary.ID, IconLibrary.location FROM IconLibrary,Icon Where IconLibrary.ID=Icon.ID AND Icon.ConnID=" + Request.QueryString.Get("ConnID"));
            }
            catch (ODBC2KMLException ex)
            {
                throw ex;
            }

            foreach (DataRow dr2 in dt2.Rows)
            {
                string iconId = dr2["ID"].ToString();
                string iconLoc = dr2["location"].ToString();
                Icon icon = new Icon();
                icon.setId(iconId);
                icon.setLocation(iconLoc);
                iconListAvailableToRemove.Add(icon);
            }
        }

        protected void fillOverlayPopupRemove()
        {
            int sizeOfBox = 6;
            int currentBoxCount = 0;
            removeOverlayInteriorPanel.Controls.Clear();
            removeOverlayInteriorPanel.Controls.Add(new LiteralControl("<table class=\"boxPopupStyle2\" cellpadding=\"5\">\n"));
            if (overlayListAvailableToRemove.Count == 0)
            {
                removeOverlayInteriorPanel.Controls.Add(new LiteralControl("<tr><td class=\"tableTD\">No overlays currently exist for this connection.</td></tr>\n"));
            }
            else
            {
                foreach (Overlay over in overlayListAvailableToRemove)
                {
                    if (currentBoxCount == sizeOfBox)
                    {
                        removeOverlayInteriorPanel.Controls.Add(new LiteralControl("</tr>\n"));
                        currentBoxCount = 0;
                    }
                    if (currentBoxCount == 0)
                    {
                        removeOverlayInteriorPanel.Controls.Add(new LiteralControl("<tr>\n"));
                    }

                    removeOverlayInteriorPanel.Controls.Add(new LiteralControl("<td class=\"iconBox\">"));
                    ImageButton imgBtn = new ImageButton();
                    imgBtn.ID = "overlayLib_" + over.getId().ToString();
                    System.Drawing.ColorConverter colConvert = new System.Drawing.ColorConverter();
                    imgBtn.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + over.getColor());
                    imgBtn.CssClass = "overlayBox";
                    imgBtn.BorderColor = (System.Drawing.Color)colConvert.ConvertFromString("#000000");
                    imgBtn.BorderWidth = Unit.Pixel(2);
                    imgBtn.Click += new ImageClickEventHandler(removeOverlayColorFromConn);
                    imgBtn.CommandArgument = over.getColor().ToString();
                    imgBtn.ImageUrl = "graphics/overlayBack.gif";
                    
                    imgBtn.AlternateText = " Remove ";

                    removeOverlayInteriorPanel.Controls.Add(imgBtn);
                    removeOverlayInteriorPanel.Controls.Add(new LiteralControl("</td>"));


                    currentBoxCount += 1;
                }
            }
            removeOverlayInteriorPanel.Controls.Add(new LiteralControl("</table>\n"));
        }

        //populates the popup panel for removing an icon from a connection
        protected void fillIconLibraryPopupRemove()
        {
            int sizeOfBox = 6;
            int currentBoxCount = 0;
            removeIconFromConn.Controls.Clear();
            removeIconFromConn.Controls.Add(new LiteralControl("<table class=\"boxPopupStyle2\" cellpadding=\"5\">\n"));
            if (iconListAvailableToRemove.Count == 0)
            {
                removeIconFromConn.Controls.Add(new LiteralControl("<tr><td class=\"tableTD\"><span style=\"color: white;\">No icons currently exist for this connection.</span></td></tr>\n"));
            }
            else
            {
                foreach (Icon icn in iconListAvailableToRemove)
                {
                    if (currentBoxCount == sizeOfBox)
                    {
                        removeIconFromConn.Controls.Add(new LiteralControl("</tr>\n"));
                        currentBoxCount = 0;
                    }
                    if (currentBoxCount == 0)
                    {
                        removeIconFromConn.Controls.Add(new LiteralControl("<tr>\n"));
                    }

                    removeIconFromConn.Controls.Add(new LiteralControl("<td>"));
                    ImageButton imgBtn = new ImageButton();
                    imgBtn.ID = "imgLib_" + icn.getId().ToString();
                    imgBtn.ImageUrl = icn.getLocation().ToString();
                    imgBtn.Click += new ImageClickEventHandler(removeIconFromConnFunct);
                    imgBtn.CommandArgument = icn.getId().ToString();
                    imgBtn.AlternateText = "Icon Cannot be Displayed";
                    imgBtn.ToolTip = icn.getLocation().ToString();

                    removeIconFromConn.Controls.Add(imgBtn);
                    removeIconFromConn.Controls.Add(new LiteralControl("</td>"));


                    currentBoxCount += 1;
                }
            }
            removeIconFromConn.Controls.Add(new LiteralControl("</table>\n"));
        }

        //populates the popup panel for adding an icon to a connection from the icon library
        protected void fillIconLibraryPopup()
        {
            int sizeOfBox = 6;
            int currentBoxCount = 0;

            addIconToLibary.Controls.Clear();
            addIconToLibary.Controls.Add(new LiteralControl("<table class=\"boxPopupStyle2\" cellpadding=\"5\">\n"));
            if (iconListAvailableToAdd.Count == 0)
            {
                addIconToLibary.Controls.Add(new LiteralControl("<tr><td class=\"tableTD\"><span style=\"color: white;\">All icons in the icon library are currently being used in the connection.</span></td></tr>\n"));
            }
            else
            {
                foreach (Icon icn in iconListAvailableToAdd)
                {
                    if (currentBoxCount == sizeOfBox)
                    {
                        addIconToLibary.Controls.Add(new LiteralControl("</tr>\n"));
                        currentBoxCount = 0;
                    }
                    if (currentBoxCount == 0)
                    {
                        addIconToLibary.Controls.Add(new LiteralControl("<tr>\n"));
                    }

                    addIconToLibary.Controls.Add(new LiteralControl("<td>"));
                    ImageButton imgBtn = new ImageButton();
                    imgBtn.ID = "imgLib_" + icn.getId().ToString();
                    imgBtn.ImageUrl = icn.getLocation().ToString();
                    imgBtn.Click += new ImageClickEventHandler(addIconFromLibraryToConn);
                    imgBtn.CommandArgument = icn.getId().ToString();
                    imgBtn.AlternateText = "Icon Cannot be Displayed";
                    imgBtn.ToolTip = icn.getLocation().ToString();

                    addIconToLibary.Controls.Add(imgBtn);
                    addIconToLibary.Controls.Add(new LiteralControl("</td>"));


                    currentBoxCount += 1;
                }
            }
            addIconToLibary.Controls.Add(new LiteralControl("</table>\n"));
        }

        //Adds an icon to a connection from the library
        protected void addIconFromLibraryToConn(object sender, EventArgs e)
        {
            ImageButton sendBtn = (ImageButton)sender;
            String args = sendBtn.CommandArgument.ToString();

            Icon icn = new Icon();
            Icon iconSaved = new Icon();
            icn.setId(args);

            try
            {
                foreach (Icon icon in iconListAvailableToAdd)
                {
                    if (icon.getId().Equals(icn.getId()))
                    {
                        iconSaved.setId(icon.getId());
                        iconSaved.setLocation(icon.getLocation());
                        iconListAvailableToAdd.Remove(icon);
                        iconListAvailableToRemove.Add(new Icon(iconSaved));
                        iconList.Add(new Icon(iconSaved));
                        conn.icons.Add(new Icon(iconSaved));
                        this.fillIconLibraryPopup();
                        this.fillIconLibraryPopupRemove();
                        this.genIconConditionTable(sender, e);
                        break;
                    }
                }
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }

            sessionSave();
        }

        //Removes an icon assocaiated with a connection and all conditions associated with it
        protected void removeIconFromConnFunct(object sender, EventArgs e)
        {
            ImageButton sendBtn = (ImageButton)sender;
            String args = sendBtn.CommandArgument.ToString();

            Icon icn = new Icon();
            Icon iconSaved = new Icon();
            icn.setId(args);

            foreach (Icon icon in iconList)
            {
                if (icon.getId().Equals(icn.getId()))
                {
                    iconList.Remove(icon);
                    break;
                }
            }
            foreach (Icon icon in conn.icons)
            {
                if (icon.getId().Equals(icn.getId()))
                {
                    conn.icons.Remove(icon);
                    break;
                }
            }
            try
            {
                foreach (Icon icon in iconListAvailableToRemove)
                {

                    if (icon.getId().Equals(icn.getId()))
                    {
                        iconSaved.setId(icon.getId());
                        iconSaved.setLocation(icon.getLocation());
                        iconListAvailableToRemove.Remove(icon);
                        int j = 0;
                        foreach (Icon icon2 in iconListAvailableToAdd)
                        {
                            if (System.Convert.ToInt32(iconSaved.getId()) < System.Convert.ToInt32(icon2.getId()))
                            {
                                iconListAvailableToAdd.Insert(j, iconSaved);
                                break;
                            }
                            j += 1;
                            if (j == iconListAvailableToAdd.Count)
                            {
                                iconListAvailableToAdd.Add(iconSaved);
                                break;
                            }
                        }
                        this.fillIconLibraryPopup();
                        this.fillIconLibraryPopupRemove();
                        this.genIconConditionTable(sender, e);
                        break;
                    }
                }
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            sessionSave();
        }

        protected void addOverlayColorToConn(object sender, EventArgs e)
        {
            Button sendBtn = (Button)sender;
            String args = HiddenValue.Value.ToString();
            args = args.Substring(1);

            Overlay ovr = new Overlay();
            ovr.setColor(args);

            bool exists = false;
            foreach (Overlay over in conn.overlays)
            {
                if (over.getColor().Equals(ovr.getColor()))
                {
                    exists = true;
                    break;
                }
            }
            if (ovr.getColor().Equals("-111111"))
            {
                this.AddOverlayPopupExtender.Hide();
                ErrorHandler eh = new ErrorHandler("Overlay color must be selected!", errorPanel1, "AddOverlayPopupExtender");
                eh.displayError();
            }
            else if (exists)
            {
                this.AddOverlayPopupExtender.Hide();
                ErrorHandler eh = new ErrorHandler("Overlay color already exists! Please choose another.", errorPanel1, "AddOverlayPopupExtender");
                eh.displayError();
                
            }
            else
            {
                try
                {
                    ovr.setId(curOverlayCount.ToString());
                    curOverlayCount -= 1;
                    overlayListAvailableToRemove.Add(ovr);
                    //overlayList.Add(new Overlay(ovr));
                    conn.overlays.Add(new Overlay(ovr));
                    this.fillOverlayPopupRemove();
                    this.genOverlayConditionTable(sender, e);
                }
                catch (ODBC2KMLException ex)
                {
                    ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                    eh.displayError();
                    return;
                }
            }
            sessionSave();
        }

        protected void removeOverlayColorFromConn(object sender, EventArgs e)
        {
            ImageButton sendBtn = (ImageButton)sender;
            String args = sendBtn.CommandArgument.ToString();

            Overlay ovr = new Overlay();
            ovr.setColor(args);

            // This should only be for the overlays in conn.overlays, not for overlayList -Micah
            /*foreach (Overlay over in overlayList)
            {
                if (over.getColor().Equals(ovr.getColor()))
                {
                    overlayList.Remove(over);
                    break;
                }
            }*/
            foreach (Overlay over in conn.overlays)
            {
                if (over.getColor().Equals(ovr.getColor()))
                {
                    conn.overlays.Remove(over);
                    break;
                }
            }
            try
            {
                foreach (Overlay over in overlayListAvailableToRemove)
                {

                    if (over.getColor().Equals(ovr.getColor()))
                    {
                        overlayListAvailableToRemove.Remove(over);
                        this.fillOverlayPopupRemove();
                        this.genOverlayConditionTable(sender, e);
                        break;
                    }
                }
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            sessionSave();
        }

        protected void fillIconListFromDatabase()
        {
            iconList.Clear();
            
            foreach (Icon i in conn.icons)
            {
                iconList.Add(new Icon(i));
            }
            sessionSave();
        }

        protected void genIconConditionTable(object sender, EventArgs e)
        {
            IconConditionPanel.Controls.Clear();
            Boolean altRows = true;
            if (conn.icons.Count == 0)  // No images set for the condition. Display a simple table stating such.
            {
                IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyle\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("No images are currently set for this connection.\n"));
                if (Request.QueryString.Get("locked") == "false")
                    IconConditionPanel.Controls.Add(new LiteralControl("<br />Add some using the Add Icons button below.\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</div>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
            }
            else
            {
                foreach (Icon icon in conn.icons)
                {
                    IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"iconBox\">\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("<img src=\"" + icon.getLocation() + "\" alt=\"\" />\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyle\">\n"));

                    if (icon.getConditions().Count != 0)  // conditions exist; display them.
                    {
                        
                            IconConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox7\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("Lower Bound\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("Lower Operator\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("Table\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("Field\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("Upper Operator\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("Upper Bound\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));

                            foreach (Condition condition in icon.getConditions())
                            {
                                if (altRows == true)
                                {
                                    IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                                    if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                                    {
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    }
                                    else
                                    {
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">&nbsp;</td><td class=\"tableTD2\">&nbsp;</td>\n"));
                                    }
                                    IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                    IconConditionPanel.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                                    IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                    IconConditionPanel.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                                    IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                                    {
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    }
                                    else
                                    {
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">&nbsp;</td><td class=\"tableTD2\">&nbsp;</td>\n"));
                                    }

                                    IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));

                                }
                                else
                                {
                                    IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                                    if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                                    {
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    }
                                    else
                                    {
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">&nbsp;</td><td class=\"tableTD3\">&nbsp;</td>\n"));
                                    }
                                    IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                    IconConditionPanel.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                                    IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                    IconConditionPanel.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                                    IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                                    {
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    }
                                    else
                                    {
                                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">&nbsp;</td><td class=\"tableTD3\">&nbsp;</td>\n"));
                                    }

                                    IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));

                                }
                                altRows = !altRows;
                            
                        }
                            IconConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                    }
                    else  // no conditions set. display table stating such.
                    {
                        IconConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("No conditions are currently set.\n"));
                        if (Request.QueryString.Get("locked") == "false")
                            IconConditionPanel.Controls.Add(new LiteralControl("<br />Add some using the button to the right.\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                    }


                    IconConditionPanel.Controls.Add(new LiteralControl("</div>\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"buttonClass\">\n"));
                    Button modifyButton = new Button();
                    if (Request.QueryString.Get("locked") == "false")
                    {
                        modifyButton.Text = "Modify Condition";
                        modifyButton.CssClass = "button";
                        modifyButton.Width = 135;
                        modifyButton.ID = "modifyIconCondition_" + icon.getId();
                        IconConditionPanel.Controls.Add(modifyButton);
                    }
                    Panel modifyIconConditionPopupPanel = new Panel();

                    modifyIconConditionPopupPanel.ScrollBars = ScrollBars.Auto;
                    modifyIconConditionPopupPanel.ID = "modifyIconConditionPopupPanel" + icon.getId();
                    modifyIconConditionPopupPanel.Style["display"] = "none";
                    modifyIconConditionPopupPanel.CssClass = "boxPopupStyle";
                    UpdatePanel modifyIconConditionInsidePopupPanel = new UpdatePanel();
                    modifyIconConditionInsidePopupPanel.ID = "modifyIconConditionInsidePopupPanel" + icon.getId().ToString();
                    modifyIconConditionInsidePopupPanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
                    try
                    {
                        genIconConditionPopup(modifyIconConditionInsidePopupPanel, icon.getId());
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }
                    modifyIconConditionPopupPanel.Controls.Add(modifyIconConditionInsidePopupPanel);
                    modifyIconConditionPopupPanel.Controls.Add(new LiteralControl("<div align=\"right\">"));
                    Button submitModifyConditionPopup = new Button();
                    submitModifyConditionPopup.ID = "submitModifyCondition" + icon.getId();
                    submitModifyConditionPopup.Text = "Submit";
                    submitModifyConditionPopup.CssClass = "button";
                    submitModifyConditionPopup.Click += new EventHandler(genIconConditionTable);
                    modifyIconConditionPopupPanel.Controls.Add(submitModifyConditionPopup);
                    modifyIconConditionPopupPanel.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
                    Button cancelModifyConditionPopup = new Button();
                    cancelModifyConditionPopup.ID = "cancelIconModifyCondition" + icon.getId();
                    cancelModifyConditionPopup.Text = "Cancel";
                    cancelModifyConditionPopup.CssClass = "button";
                    cancelModifyConditionPopup.Click += new EventHandler(cancelModifyIconConditionPopup_Click);
                    cancelModifyConditionPopup.CommandArgument = icon.getId();
                    modifyIconConditionPopupPanel.Controls.Add(cancelModifyConditionPopup);
                    modifyIconConditionPopupPanel.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
                    modifyIconConditionPopupPanel.Controls.Add(new LiteralControl("</div>"));
                    if (Request.QueryString.Get("locked") == "false")
                    {
                        AjaxControlToolkit.ModalPopupExtender mpe = new AjaxControlToolkit.ModalPopupExtender();
                        mpe.ID = "MPE_" + icon.getId();
                        mpe.BackgroundCssClass = "modalBackground";
                        mpe.DropShadow = true;
                        mpe.PopupControlID = modifyIconConditionPopupPanel.ID.ToString();
                        mpe.TargetControlID = modifyButton.ID.ToString();
                        
                        IconConditionPanel.Controls.Add(mpe);

                        IconConditionPanel.Controls.Add(modifyIconConditionPopupPanel);
                    }
                    IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));

                    DropDownList addTableName = (DropDownList)Page.FindControl("addIconTable" + icon.getId());
                    if ((addTableName != null) && (addTableName.Items.Count != 0))
                    {
                        //addTableName.SelectedIndex = 0;
                        addTableName_SelectedIndexChanged(addTableName, new EventArgs());
                    }
                }  // end of foreach icon in tempIconlist
            }
            
        }

        protected void genIconConditionPopup(UpdatePanel modifyIconConditionInsidePopupPanel, String args)
        {
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<span class=\"connectionStyle\">&nbsp;Modify Condition</span>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<div class=\"mainBoxP\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table cellspacing=\"0\" cellpadding=\"5\" class=\"mainBox2\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<div class=\"omainBox4\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table class=\"omainBox6\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Add and remove conditions for this icon using the table below. Click Add after you fill in each new condition, then click Submit. Only conditions that are in the same table as your mapping information will actually be used.\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</table>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<p>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</p>\n"));

            // icon popup table's header
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table class=\"omainBox7\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Lower Bound\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Lower Operator\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Table\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Field\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Upper Operator\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Upper Bound\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("&nbsp;\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
            int i = 0;
            Boolean altRows = true;
            
            foreach (Icon icon in conn.icons)
            {
                if (icon.getId() != args)
                    continue;

                foreach (Condition condition in icon.getConditions())
                {
                    i++;
                    if (altRows == true)
                    {
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
                        if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                        {
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        }
                        else
                        {
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">&nbsp;</td><td class=\"tableTD2\">&nbsp;</td>\n"));
                        }
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                        {
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        }
                        else
                        {
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">&nbsp;</td><td class=\"tableTD2\">&nbsp;</td>\n"));
                        }
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"textCenter\">\n"));
                        Button deleteConditionButton = new Button();
                        deleteConditionButton.ID = "delCondition_" + args + "_" + i.ToString();

                        deleteConditionButton.Text = "Remove";
                        deleteConditionButton.CssClass = "button";
                        deleteConditionButton.ToolTip = "Delete Condition";
                        deleteConditionButton.Width = 80;
                        deleteConditionButton.Click += new EventHandler(deleteIconCondition);
                        deleteConditionButton.CommandArgument = icon.getId() + " " + condition.getId();
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(deleteConditionButton);
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
                    }
                    else
                    {
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
                        if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                        {
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        }
                        else
                        {
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">&nbsp;</td><td class=\"tableTD3\">&nbsp;</td>\n"));
                        }
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                        {
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        }
                        else
                        {
                            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">&nbsp;</td><td class=\"tableTD3\">&nbsp;</td>\n"));
                        }
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3C\">\n"));
                        Button deleteConditionButton = new Button();
                        deleteConditionButton.ID = "delCondition_" + args + "_" + i.ToString();

                        deleteConditionButton.Text = "Remove";
                        deleteConditionButton.CssClass = "button";
                        deleteConditionButton.ToolTip = "Delete Condition";
                        deleteConditionButton.Width = 80;
                        deleteConditionButton.Click += new EventHandler(deleteIconCondition);
                        deleteConditionButton.CommandArgument = icon.getId() + " " + condition.getId();
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(deleteConditionButton);
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));

                    }
                    altRows = !altRows;
                }
            }

           
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr class=\"tableTR\">\n"));
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
                TextBox addLowerBound = new TextBox();
                addLowerBound.ID = "addIconLowerBound" + args;
                addLowerBound.Width = 40;
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addLowerBound);
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
                DropDownList addLowerOperator = new DropDownList();
                addLowerOperator.ID = "addIconLowerOperator" + args;
                addLowerOperator.CssClass = "inputDD";
                addLowerOperator.Width = 40;
                addLowerOperator.Items.Add("");
                addLowerOperator.Items.Add("<");
                addLowerOperator.Items.Add("<=");
                addLowerOperator.Items.Add("==");
                addLowerOperator.Items.Add("!=");
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addLowerOperator);
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
                DropDownList addTableName = new DropDownList();
                addTableName.ID = "addIconTable" + args;
                addTableName.CssClass = "inputDD";
                //addTableName.Width = "auto";
                addTableName.AutoPostBack = true;

                try
                {
                    if (conn.connInfo.getDatabaseType() == ConnInfo.MSSQL)
                    {
                        addTableName.DataSource = MSQLTables;
                        addTableName.DataTextField = "TABLE_NAME";
                        addTableName.DataValueField = "TABLE_NAME";
                        addTableName.DataBind();
                    }
                    else if (conn.connInfo.getDatabaseType() == ConnInfo.MYSQL)
                    {
                        addTableName.DataSource = SQLTables;
                        addTableName.DataTextField = "TABLE_NAME";
                        addTableName.DataValueField = "TABLE_NAME";
                        addTableName.DataBind();
                    }
                    else if (conn.connInfo.getDatabaseType() == ConnInfo.ORACLE)
                    {
                        addTableName.DataSource = oracleTables;
                        addTableName.DataTextField = "TABLE_NAME";
                        addTableName.DataValueField = "TABLE_NAME";
                        addTableName.DataBind();
                    }
                }
                catch
                {
                    throw new ODBC2KMLException("Unable to connect to target database.");
                }

                addTableName.SelectedIndexChanged += new EventHandler(addTableName_SelectedIndexChanged);
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addTableName);
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
                DropDownList addFieldName = new DropDownList();
                addFieldName.ID = "addIconField" + args;
                addFieldName.CssClass = "inputDD";
                //addFieldName.Width = "auto";
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addFieldName);
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
                DropDownList addUpperOperator = new DropDownList();
                addUpperOperator.ID = "addIconUpperOperator" + args;
                addUpperOperator.CssClass = "inputDD";
                addUpperOperator.Width = 40;
                addUpperOperator.Items.Add("");
                addUpperOperator.Items.Add("<");
                addUpperOperator.Items.Add("<=");
                addUpperOperator.Items.Add("==");
                addUpperOperator.Items.Add("!=");
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addUpperOperator);
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
                TextBox addUpperBound = new TextBox();
                addUpperBound.ID = "addIconUpperBound" + args;
                addUpperBound.MaxLength = 30;
                addUpperBound.CssClass = "inputBox";
                addUpperBound.Width = 40;
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addUpperBound);
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAddC\">\n"));
                Button addConditionButton = new Button();
                addConditionButton.ID = "addIconConditionButton" + args;
                addConditionButton.Text = "Add";
                addConditionButton.ToolTip = "Add Condition";
                addConditionButton.Width = 80;
                addConditionButton.CssClass = "button";
                addConditionButton.Click += new EventHandler(addConditionToIcon);
                addConditionButton.CommandArgument = args;
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addConditionButton);
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</table>\n"));
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</div>\n"));
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</table>\n"));
                modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</div>\n"));
           
        }

        protected void deleteIconCondition(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int lastSpace = btn.CommandArgument.LastIndexOf(" ");
            if (lastSpace >= 0)
            {
                string iconId = btn.CommandArgument.Substring(0, lastSpace);
                string conditionId = btn.CommandArgument.Substring(lastSpace + 1);
                foreach (Icon icon in conn.icons)
                {
                    if (icon.getId() == iconId)
                    {
                        icon.removeConditions(conditionId);
                    }
                }
            }
            try
            {
                genIconConditionTable(sender, e);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            sessionSave();
        }

        protected void addConditionToIcon(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string args = btn.CommandArgument.ToString();
            TextBox lowerBound = (TextBox)Page.FindControl("addIconLowerBound" + args);
            DropDownList lowerOperator = (DropDownList)Page.FindControl("addIconLowerOperator" + args);
            DropDownList tableName = (DropDownList)Page.FindControl("addIconTable" + args);
            DropDownList fieldName = (DropDownList)Page.FindControl("addIconField" + args);
            DropDownList upperOperator = (DropDownList)Page.FindControl("addIconUpperOperator" + args);
            TextBox upperBound = (TextBox)Page.FindControl("addIconUpperBound" + args);
            UpdatePanel modifyIconConditionInsidePopupPanel = (UpdatePanel)Page.FindControl("modifyIconConditionInsidePopupPanel" + args);
            string iconId = args;

            Condition condition = new Condition();
            condition.setId(tempId--);
            condition.setLowerBound(lowerBound.Text.ToString());
            condition.setUpperBound(upperBound.Text.ToString());
            condition.setTableName(tableName.Text.ToString());
            condition.setFieldName(fieldName.Text.ToString());
            if (lowerOperator != null)
                condition.setLowerOperator(lowerOperator.SelectedItem.Text.ToString());
            if (upperOperator != null)
                condition.setUpperOperator(upperOperator.SelectedItem.Text.ToString());

            string conditionErrors = condition.getErrorText();
            if (conditionErrors != "")
            {
                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyIconConditionInsidePopupPanel"+iconId), "MPE_"+iconId);
                eh.displayError();
                lowerBound.Text = condition.getLowerBound();
                lowerOperator.SelectedValue = condition.getLowerOperator();
                tableName.SelectedValue = condition.getTableName();
                fieldName.SelectedValue = condition.getFieldName();
                upperOperator.SelectedValue = condition.getUpperOperator();
                upperBound.Text = condition.getUpperBound();
                //errorUpdatePanel.Update();
                return;
            }

            //Verify that the bounds are of the same datatype as the column value
            if (conditionErrors == "")
            {
                //Create database based on the connInfo
                Database testDB = new Database(conn.connInfo);
                DataTable testTable = null;
                try
                {
                    //Create the datatable to parse through
                    testTable = testDB.executeQueryRemote("SELECT " + condition.getFieldName() + " FROM " + condition.getTableName());
                }
                catch (ODBC2KMLException)
                {
                    ErrorHandler eh = new ErrorHandler("There was an error connecting to the remote database to verify icon condition information", errorPanel1);
                    eh.displayError();
                    return;
                }

                //Check the bounds against the database values
                foreach (DataRow row in testTable.Rows)
                {
                    //Counter to break out of the loop, this way only a subset of the data is tested
                    int counter = 0;

                    //If both conditions....
                    if (condition.getLowerBound() != "" && condition.getUpperBound() != "")
                    {
                        //Need variables for Try parse
                        Double lower;
                        Double upper;

                        //See if both conditions evaluate to doubles
                        if (Double.TryParse(condition.getLowerBound(), out lower) && Double.TryParse(condition.getUpperBound(), out upper))
                        {
                            Double value;
                            //If value is not a double, throw an error
                            if (!Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "")
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyIconConditionInsidePopupPanel" + iconId), "MPE_" + iconId);
                                eh.displayError();
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                        else //They evaluate to strings
                        {
                            Double value;
                            //If value is a double, throw an error
                            if (Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "") 
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyIconConditionInsidePopupPanel" + iconId), "MPE_" + iconId);
                                eh.displayError();
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                    }
                    else if (condition.getLowerBound() != "")
                    {
                        //Need variables for Try parse
                        Double lower;

                        //See if the lower conditions evaluate to doubles
                        if (Double.TryParse(condition.getLowerBound(), out lower))
                        {
                            Double value;
                            //If value is not a double, throw an error
                            if (!Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "") 
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyIconConditionInsidePopupPanel" + iconId), "MPE_" + iconId);
                                eh.displayError();
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                        else //They evaluate to strings
                        {
                            Double value;
                            //If value is a double, throw an error
                            if (Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "") 
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyIconConditionInsidePopupPanel" + iconId), "MPE_" + iconId);
                                eh.displayError();
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                    }
                    else
                    {
                        //Need variables for Try parse
                        Double upper;

                        //See if the lower conditions evaluate to doubles
                        if (Double.TryParse(condition.getUpperBound(), out upper) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "") 
                        {
                            Double value;
                            //If value is not a double, throw an error
                            if (!Double.TryParse(row[condition.getFieldName()].ToString(), out value))
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyIconConditionInsidePopupPanel" + iconId), "MPE_" + iconId);
                                eh.displayError();
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                        else //They evaluate to strings
                        {
                            Double value;
                            //If value is a double, throw an error
                            if (Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "") 
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";                                
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyIconConditionInsidePopupPanel" + iconId), "MPE_" + iconId);
                                eh.displayError();
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                    }
                }
            }

            foreach (Icon icon in conn.icons)
            {
                if (icon.getId() == iconId)
                {
                    icon.setConditions(condition);
                }
            }
            try
            {
                genIconConditionTable(sender, e);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            sessionSave();
        }

        protected void cancelModifyIconConditionPopup_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string iconId = btn.CommandArgument;
            Icon replaceWithThisIcon = new Icon();
            foreach (Icon icon in iconList)
            {
                if (icon.getId() == iconId)
                {
                    replaceWithThisIcon = icon;
                    break;
                }
            }
            foreach (Icon icon in conn.icons)
            {
                if (icon.getId() == iconId)
                    icon.setConditions(replaceWithThisIcon.getDeepCopyOfConditions());
            }
            try
            {
                genIconConditionTable(sender, e);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            sessionSave();
        }

        /// <summary>
        /// PostBack function for the overlay colorPicker
        /// </summary>
        /// <param name="sender">Object that called postback</param>
        /// <param name="e">Color and Previous color for that colorPicker</param>
        private void overlayColorChanged(Object sender, ColorPostBackEventArgs e)
        {
            ColorPicker colorPicker = (ColorPicker)sender;
            string overlayID = colorPicker.TargetId;
            overlayID = overlayID.Replace("overlay", "");
            overlayID = overlayID.Replace("color", "");

            // Check to see if the overlay color already exists in the connection
            foreach (Overlay over in conn.overlays)
            {
                if (e.Color == ("#" + over.color) && over.id != int.Parse(overlayID))
                {
                    ErrorHandler eh = new ErrorHandler("Overlay color already exists! Please choose another.", errorPanel1);
                    eh.displayError();
                    return;
                }
            }

            // if it is a unique color, save the change
            foreach (Overlay over in conn.overlays)
            {

                if(over.id == int.Parse(overlayID))
                {
                    over.color = e.Color.Replace("#","");
                    break;
                }
            }

            //Regenerate the overlay condition table
            genOverlayConditionTable(null, null);

            sessionSave();
        }

        protected void genOverlayConditionTable(object sender, EventArgs e)
        {
            OverlayConditionPanel.Controls.Clear();
            if (conn.overlays.Count == 0)  // No images set for the condition. Display a simple table stating such.
            {
                OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyle\">\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("No overlays are currently set for this connection.</span>\n"));
                if (Request.QueryString.Get("locked") == "false")
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<br />Add some using the Add Overlay button below.\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</div>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
            }
            else
            {
                foreach (Overlay overlay in conn.overlays)
                {
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"iconBox\">\n"));
                    
                    // Generate Text Box to hold the color
                    TextBox tx = new TextBox();
                    tx.ReadOnly = true;
                    tx.CssClass = "overlayBox";
                    tx.ID = "overlay" + overlay.getId() + "color";
                    tx.Style["background-color"] = "#" + overlay.getColor();
                    tx.Style["border"] = "2px solid black";
                    tx.Style["cursor"] = "pointer";
                    
                    // Generate the ColorPicker for each overlay
                    ColorPicker colorPicker = new ColorPicker();
                    colorPicker.InitialColor = "#"+overlay.getColor();
                    colorPicker.ColorPostBack += overlayColorChanged;
                    colorPicker.TargetId = tx.ClientID;
                    colorPicker.ID = "overlayColorPicker" + overlay.getId();
                    colorPicker.PickButton = false;
                    colorPicker.TargetProperty = "style.backgroundColor";
                    colorPicker.AutoPostBack = true;
                    colorPicker.Controls.Add(tx);
                    OverlayConditionPanel.Controls.Add(colorPicker);

                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyle\">\n"));

                    if (overlay.getConditions().Count != 0)  // conditions exist; display them.
                    {
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox7\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Lower Bound\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Lower Operator\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Table\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Field\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Upper Operator\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Upper Bound\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));

                        Boolean altRows = true;
                        foreach (Condition condition in overlay.getConditions())
                        {
                            if (altRows == true)
                            {
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                                if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                                {
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                }
                                else
                                {
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">&nbsp;</td><td class=\"tableTD2\">&nbsp;</td>\n"));
                                }
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                                {
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                }
                                else
                                {
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD2\">&nbsp;</td><td class=\"tableTD2\">&nbsp;</td>\n"));
                                }

                                OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                            }
                            else
                            {
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                                if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                                {
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                }
                                else
                                {
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">&nbsp;</td><td class=\"tableTD3\">&nbsp;</td>\n"));
                                }
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                                {
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                }
                                else
                                {
                                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD3\">&nbsp;</td><td class=\"tableTD3\">&nbsp;</td>\n"));
                                }

                                OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                            }
                            altRows = !altRows;
                        }
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                    }
                    else  // no conditions set. display table stating such.
                    {
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("No conditions are currently set.\n"));
                        if (Request.QueryString.Get("locked") == "false")
                            OverlayConditionPanel.Controls.Add(new LiteralControl("<br />Add some using the button to the right.\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                    }


                    OverlayConditionPanel.Controls.Add(new LiteralControl("</div>\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"buttonClass\">\n"));
                    Button modifyButton = new Button();
                    if (Request.QueryString.Get("locked") == "false")
                    {
                        modifyButton.Text = "Modify Condition";
                        modifyButton.CssClass = "button";
                        modifyButton.Width = 135;
                        modifyButton.ID = "modifyOverlayCondition_" + overlay.getId();
                        OverlayConditionPanel.Controls.Add(modifyButton);
                    }
                    Panel modifyOverlayConditionPopupPanel = new Panel();
                    modifyOverlayConditionPopupPanel.ScrollBars = ScrollBars.Auto;
                    modifyOverlayConditionPopupPanel.ID = "modifyOverlayConditionPopupPanel" + overlay.getId();
                    modifyOverlayConditionPopupPanel.Style["display"] = "none";
                    modifyOverlayConditionPopupPanel.CssClass = "boxPopupStyle";
                    UpdatePanel modifyOverlayConditionInsidePopupPanel = new UpdatePanel();
                    modifyOverlayConditionInsidePopupPanel.ID = "modifyOverlayConditionInsidePopupPanel" + overlay.getId();
                    modifyOverlayConditionInsidePopupPanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
                    try
                    {
                        genOverlayConditionPopup(modifyOverlayConditionInsidePopupPanel, overlay.getId());
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }
                    modifyOverlayConditionPopupPanel.Controls.Add(modifyOverlayConditionInsidePopupPanel);


                    modifyOverlayConditionPopupPanel.Controls.Add(new LiteralControl("<div align=\"right\">"));
                    Button submitModifyConditionPopup = new Button();
                    submitModifyConditionPopup.ID = "submitOverlayModifyCondition" + overlay.getId();
                    submitModifyConditionPopup.Text = "Submit";
                    submitModifyConditionPopup.CssClass = "button";
                    submitModifyConditionPopup.Click += new EventHandler(genOverlayConditionTable);
                    modifyOverlayConditionPopupPanel.Controls.Add(submitModifyConditionPopup);
                    modifyOverlayConditionPopupPanel.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
                    Button cancelModifyConditionPopup = new Button();
                    cancelModifyConditionPopup.ID = "cancelOverlayModifyCondition" + overlay.getId();
                    cancelModifyConditionPopup.Text = "Cancel";
                    cancelModifyConditionPopup.CssClass = "button";
                    cancelModifyConditionPopup.Click += new EventHandler(cancelModifyOverlayConditionPopup_Click);
                    cancelModifyConditionPopup.CommandArgument = overlay.getId();
                    modifyOverlayConditionPopupPanel.Controls.Add(cancelModifyConditionPopup);
                    modifyOverlayConditionPopupPanel.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
                    modifyOverlayConditionPopupPanel.Controls.Add(new LiteralControl("</div>"));
                    if (Request.QueryString.Get("locked") == "false")
                    {
                        AjaxControlToolkit.ModalPopupExtender mpe = new AjaxControlToolkit.ModalPopupExtender();
                        mpe.ID = "MPE_OVERLAY_" + overlay.getId();
                        mpe.BackgroundCssClass = "modalBackground";
                        mpe.DropShadow = true;
                        mpe.PopupControlID = modifyOverlayConditionPopupPanel.ID.ToString();
                        mpe.TargetControlID = modifyButton.ID.ToString();
                        OverlayConditionPanel.Controls.Add(mpe);

                        OverlayConditionPanel.Controls.Add(modifyOverlayConditionPopupPanel);
                    }
                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));

                    DropDownList addTableName = (DropDownList)Page.FindControl("addOverlayTable" + overlay.getId());
                    if ((addTableName != null) && (addTableName.Items.Count != 0))
                    {
                        //addTableName.SelectedIndex = 0;
                        addTableName_SelectedIndexChanged(addTableName, new EventArgs());
                    }
                }  // end of foreach overlay in tempOverlaylist
            }

        }

        protected void genOverlayConditionPopup(UpdatePanel modifyOverlayConditionInsidePopupPanel, String args)
        {
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<span class=\"connectionStyle\">&nbsp;Modify Condition</span>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<div class=\"mainBoxP\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table cellspacing=\"0\" cellpadding=\"5\" class=\"mainBox2\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<div class=\"omainBox4\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table class=\"omainBox6\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Add and remove conditions for this overlay using the table below. Click Add after you fill in each new condition, then click Submit. Only conditions that are in the same table as your mapping information will actually be used.\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</table>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<p>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</p>\n"));

            // overlay popup table's header
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table class=\"omainBox7\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Lower Bound\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Lower Operator\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Table\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Field\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Upper Operator\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Upper Bound\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("&nbsp;\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
            int i = 0;
            Boolean altRows = true;

            foreach (Overlay overlay in conn.overlays)
            {
                if (overlay.getId() != args)
                    continue;

                foreach (Condition condition in overlay.getConditions())
                {
                    i++;
                    if (altRows == true)
                    {
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
                        if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                        {
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        }
                        else
                        {
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">&nbsp;</td><td class=\"tableTD2\">&nbsp;</td>\n"));
                        }
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                        {
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        }
                        else
                        {
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD2\">&nbsp;</td><td class=\"tableTD2\">&nbsp;</td>\n"));
                        }
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"textCenter\">\n"));
                        Button deleteConditionButton = new Button();
                        deleteConditionButton.ID = "delOverlayCondition_" + args + "_" + i.ToString();

                        deleteConditionButton.Text = "Remove";
                        deleteConditionButton.CssClass = "button";
                        deleteConditionButton.ToolTip = "Delete Condition";
                        deleteConditionButton.Width = 80;
                        deleteConditionButton.Click += new EventHandler(deleteOverlayCondition);
                        deleteConditionButton.CommandArgument = overlay.getId() + " " + condition.getId();
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(deleteConditionButton);
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
                    }
                    else
                    {
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
                        if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                        {
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        }
                        else
                        {
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">&nbsp;</td><td class=\"tableTD3\">&nbsp;</td>\n"));
                        }
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                        {
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        }
                        else
                        {
                            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3\">&nbsp;</td><td class=\"tableTD2\">&nbsp;</td>\n"));
                        }
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD3C\">\n"));
                        Button deleteConditionButton = new Button();
                        deleteConditionButton.ID = "delOverlayCondition_" + args + "_" + i.ToString();

                        deleteConditionButton.Text = "Remove";
                        deleteConditionButton.CssClass = "button";
                        deleteConditionButton.ToolTip = "Delete Condition";
                        deleteConditionButton.Width = 80;
                        deleteConditionButton.Click += new EventHandler(deleteOverlayCondition);
                        deleteConditionButton.CommandArgument = overlay.getId() + " " + condition.getId();
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(deleteConditionButton);
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
                    }

                    altRows = !altRows;
                }
            }

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr class=\"tableTR\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
            TextBox addLowerBound = new TextBox();
            addLowerBound.ID = "addOverlayLowerBound" + args;
            addLowerBound.Width = 40;
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addLowerBound);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
            DropDownList addLowerOperator = new DropDownList();
            addLowerOperator.ID = "addOverlayLowerOperator" + args;
            addLowerOperator.CssClass = "inputDD";
            addLowerOperator.Width = 40;
            addLowerOperator.Items.Add("");
            addLowerOperator.Items.Add("<");
            addLowerOperator.Items.Add("<=");
            addLowerOperator.Items.Add("==");
            addLowerOperator.Items.Add("!=");
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addLowerOperator);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
            DropDownList addTableName = new DropDownList();
            addTableName.ID = "addOverlayTable" + args;
            addTableName.CssClass = "inputDD";
            //addTableName.Width = "auto";
            addTableName.AutoPostBack = true;
            
            try
            {
                if (conn.connInfo.getDatabaseType() == ConnInfo.MSSQL)
                {
                    addTableName.DataSource = MSQLTables;
                    addTableName.DataTextField = "TABLE_NAME";
                    addTableName.DataValueField = "TABLE_NAME";
                    addTableName.DataBind();
                }
                else if (conn.connInfo.getDatabaseType() == ConnInfo.MYSQL)
                {
                    addTableName.DataSource = SQLTables;
                    addTableName.DataTextField = "TABLE_NAME";
                    addTableName.DataValueField = "TABLE_NAME";
                    addTableName.DataBind();
                }
                else if (conn.connInfo.getDatabaseType() == ConnInfo.ORACLE)
                {
                    addTableName.DataSource = oracleTables;
                    addTableName.DataTextField = "TABLE_NAME";
                    addTableName.DataValueField = "TABLE_NAME";
                    addTableName.DataBind();
                }
            }
            catch
            {
                throw new ODBC2KMLException("Unable to connect to target database.");
            }
            addTableName.SelectedIndexChanged += new EventHandler(addTableName_SelectedIndexChanged);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addTableName);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
            DropDownList addFieldName = new DropDownList();
            addFieldName.ID = "addOverlayField" + args;
            addFieldName.CssClass = "inputDD";
            //addFieldName.Width = "auto";
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addFieldName);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
            DropDownList addUpperOperator = new DropDownList();
            addUpperOperator.ID = "addOverlayUpperOperator" + args;
            addUpperOperator.CssClass = "inputDD";
            addUpperOperator.Width = 40;
            addUpperOperator.Items.Add("");
            addUpperOperator.Items.Add("<");
            addUpperOperator.Items.Add("<=");
            addUpperOperator.Items.Add("==");
            addUpperOperator.Items.Add("!=");
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addUpperOperator);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAdd\">\n"));
            TextBox addUpperBound = new TextBox();
            addUpperBound.ID = "addOverlayUpperBound" + args;
            addUpperBound.MaxLength = 30;
            addUpperBound.CssClass = "inputBox";
            addUpperBound.Width = 40;
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addUpperBound);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTDAddC\">\n"));
            Button addConditionButton = new Button();
            addConditionButton.ID = "addOverlayConditionButton" + args;
            addConditionButton.Text = "Add";
            addConditionButton.ToolTip = "Add Condition";
            addConditionButton.Width = 80;
            addConditionButton.CssClass = "button";
            addConditionButton.Click += new EventHandler(addConditionToOverlay);
            addConditionButton.CommandArgument = args;
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addConditionButton);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</table>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</div>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</table>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</div>\n"));

        }

        protected void deleteOverlayCondition(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int lastSpace = btn.CommandArgument.LastIndexOf(" ");
            if (lastSpace >= 0)
            {
                string overlayId = btn.CommandArgument.Substring(0, lastSpace);
                string conditionId = btn.CommandArgument.Substring(lastSpace + 1);
                foreach (Overlay overlay in conn.overlays)
                {
                    if (overlay.getId() == overlayId)
                    {
                        overlay.removeConditions(conditionId);
                    }
                }
            }
            try
            {
                genOverlayConditionTable(sender, e);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            sessionSave();
        }

        protected void addConditionToOverlay(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string args = btn.CommandArgument.ToString();
            TextBox lowerBound = (TextBox)Page.FindControl("addOverlayLowerBound" + args);
            DropDownList lowerOperator = (DropDownList)Page.FindControl("addOverlayLowerOperator" + args);
            DropDownList tableName = (DropDownList)Page.FindControl("addOverlayTable" + args);
            DropDownList fieldName = (DropDownList)Page.FindControl("addOverlayField" + args);
            DropDownList upperOperator = (DropDownList)Page.FindControl("addOverlayUpperOperator" + args);
            TextBox upperBound = (TextBox)Page.FindControl("addOverlayUpperBound" + args);
            UpdatePanel modifyIconConditionInsidePopupPanel = (UpdatePanel)Page.FindControl("modifyOverlayConditionInsidePopupPanel" + args);
            string overlayId = args;

            Condition condition = new Condition();
            condition.setId(tempId--);
            condition.setLowerBound(lowerBound.Text.ToString());
            condition.setUpperBound(upperBound.Text.ToString());
            condition.setTableName(tableName.Text.ToString());
            condition.setFieldName(fieldName.Text.ToString());
            if (lowerOperator != null)
                condition.setLowerOperator(lowerOperator.SelectedItem.Text.ToString());
            if (upperOperator != null)
                condition.setUpperOperator(upperOperator.SelectedItem.Text.ToString());

            string conditionErrors = condition.getErrorText();
            if (conditionErrors != "")
            {
                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyOverlayConditionInsidePopupPanel" + overlayId), "MPE_OVERLAY_" + overlayId);
                eh.displayError();
                lowerBound.Text = condition.getLowerBound();
                lowerOperator.SelectedValue = condition.getLowerOperator();
                tableName.SelectedValue = condition.getTableName();
                fieldName.SelectedValue = condition.getFieldName();
                upperOperator.SelectedValue = condition.getUpperOperator();
                upperBound.Text = condition.getUpperBound();
                return;
            }

            //Verify that the bounds are of the same datatype as the column value
            if (conditionErrors == "")
            {
                //Create database based on the connInfo
                Database testDB = new Database(conn.connInfo);
                DataTable testTable = null;

                try
                {
                    //Create the datatable to parse through
                    testTable = testDB.executeQueryRemote("SELECT " + condition.getFieldName() + " FROM " + condition.getTableName());
                }
                catch (ODBC2KMLException)
                {
                    ErrorHandler eh = new ErrorHandler("There was an error connecting to the remote database to verify overlay condition information", errorPanel1);
                    eh.displayError();
                    return;
                }

                //Check the bounds against the database values
                foreach (DataRow row in testTable.Rows)
                {
                    //Counter to break out of the loop, this way only a subset of the data is tested
                    int counter = 0;

                    //If both conditions....
                    if (condition.getLowerBound() != "" && condition.getUpperBound() != "")
                    {
                        //Need variables for Try parse
                        Double lower;
                        Double upper;

                        //See if both conditions evaluate to doubles
                        if (Double.TryParse(condition.getLowerBound(), out lower) && Double.TryParse(condition.getUpperBound(), out upper))
                        {
                            Double value;
                            //If value is not a double, throw an error
                            if (!Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "") 
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyOverlayConditionInsidePopupPanel" + overlayId), "MPE_OVERLAY_" + overlayId);
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                eh.displayError();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                        else //They evaluate to strings
                        {
                            Double value;
                            //If value is a double, throw an error
                            if (Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "")
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyOverlayConditionInsidePopupPanel" + overlayId), "MPE_OVERLAY_" + overlayId);
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound(); 
                                eh.displayError();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                    }
                    else if (condition.getLowerBound() != "")
                    {
                        //Need variables for Try parse
                        Double lower;

                        //See if the lower conditions evaluate to doubles
                        if (Double.TryParse(condition.getLowerBound(), out lower))
                        {
                            Double value;
                            //If value is not a double, throw an error
                            if (!Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "")
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyOverlayConditionInsidePopupPanel" + overlayId), "MPE_OVERLAY_" + overlayId);
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                eh.displayError();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                        else //They evaluate to strings
                        {
                            Double value;
                            //If value is a double, throw an error
                            if (Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "") 
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyOverlayConditionInsidePopupPanel" + overlayId), "MPE_OVERLAY_" + overlayId);
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                eh.displayError();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                    }
                    else
                    {
                        //Need variables for Try parse
                        Double upper;

                        //See if the lower conditions evaluate to doubles
                        if (Double.TryParse(condition.getUpperBound(), out upper))
                        {
                            Double value;
                            //If value is not a double, throw an error
                            if (!Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "") 
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyOverlayConditionInsidePopupPanel" + overlayId), "MPE_OVERLAY_" + overlayId);
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                eh.displayError();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                        else //They evaluate to strings
                        {
                            Double value;
                            //If value is a double, throw an error
                            if (Double.TryParse(row[condition.getFieldName()].ToString(), out value) && row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "") 
                            {
                                conditionErrors = "The datatype of the entered bounds do not match the datatype of the database values.";
                                ErrorHandler eh = new ErrorHandler(conditionErrors, (UpdatePanel)Page.FindControl("modifyOverlayConditionInsidePopupPanel" + overlayId), "MPE_OVERLAY_" + overlayId);
                                lowerBound.Text = condition.getLowerBound();
                                lowerOperator.SelectedValue = condition.getLowerOperator();
                                tableName.SelectedValue = condition.getTableName();
                                fieldName.SelectedValue = condition.getFieldName();
                                upperOperator.SelectedValue = condition.getUpperOperator();
                                upperBound.Text = condition.getUpperBound();
                                eh.displayError();
                                return;
                            }
                            else //Decrement counter, this row wasn't a valid row and should be ignored
                            {
                                counter--;
                            }

                            //test five elements
                            if (++counter > 5)
                                break;
                        }
                    }
                }
            }

            foreach (Overlay overlay in conn.overlays)
            {
                if (overlay.getId() == overlayId)
                {
                    overlay.setConditions(condition);
                }
            }
            try
            {
                genOverlayConditionTable(sender, e);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            sessionSave();
        }

        protected void cancelModifyOverlayConditionPopup_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string overlayId = btn.CommandArgument;
            Overlay replaceWithThisOverlay = new Overlay();
            foreach (Overlay overlay in overlayList)
            {
                if (overlay.getId() == overlayId)
                {
                    replaceWithThisOverlay = overlay;
                    break;
                }
            }
            foreach (Overlay overlay in conn.overlays)
            {
                if (overlay.getId() == overlayId)
                    overlay.setConditions(replaceWithThisOverlay.getDeepCopyOfConditions());
            }
            try
            {
                genOverlayConditionTable(sender, e);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            sessionSave();
        }

        protected void addTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connectionString = "";
            string providerName = "";

            DropDownList tableList = (DropDownList)sender;
            string selectedTable = tableList.SelectedItem.ToString();
            string fieldListId;
            if (tableList.ID.ToString().LastIndexOf("addIconTable") != -1)
                fieldListId= "addIconField" + tableList.ID.Substring(tableList.ID.LastIndexOf("e") + 1);
            else
                fieldListId = "addOverlayField" + tableList.ID.Substring(tableList.ID.LastIndexOf("e") + 1);
            DropDownList fieldList = (DropDownList)Page.FindControl(fieldListId);
            if (selectedTable == "")
            {
                return;
            }

            try
            {
                if (conn.connInfo.databaseType == ConnInfo.MSSQL)
                {
                    connectionString = "Data Source=" + conn.connInfo.serverAddress + ";Initial Catalog=" + conn.connInfo.databaseName + ";Persist Security Info=True;User Id=" + conn.connInfo.userName + ";Password=" + conn.connInfo.password;
                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                    fieldList.DataSource = temp;
                    fieldList.DataValueField = "COLUMN_NAME";
                    fieldList.DataTextField = "COLUMN_NAME";
                    fieldList.DataBind();
                }
                else if (conn.connInfo.databaseType == ConnInfo.MYSQL)
                {
                    connectionString = "server=" + conn.connInfo.serverAddress + ";User Id=" + conn.connInfo.userName + ";password=" + conn.connInfo.password + ";Persist Security Info=True;database=" + conn.connInfo.databaseName;
                    providerName = "MySql.Data.MySqlClient";
                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString;
                    temp.ProviderName = providerName;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                    fieldList.DataSource = temp;
                    fieldList.DataValueField = "COLUMN_NAME";
                    fieldList.DataTextField = "COLUMN_NAME";
                    fieldList.DataBind();
                }
                else if (conn.connInfo.databaseType == ConnInfo.ORACLE)
                {
                    connectionString = "Data Source=" + conn.connInfo.serverAddress + ";Persist Security Info=True;User ID=" + conn.connInfo.userName + ";Password=" + conn.connInfo.password + ";Unicode=True";
                    providerName = "System.Data.OracleClient";
                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString;
                    temp.ProviderName = providerName;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedTable + "')";
                    fieldList.DataSource = temp;
                    fieldList.DataValueField = "COLUMN_NAME";
                    fieldList.DataTextField = "COLUMN_NAME";
                    fieldList.DataBind();
                }
                else
                {
                    throw new ODBC2KMLException("Unknown database type.");
                }
            }
            catch
            {
                throw new ODBC2KMLException("Unable to connect to target database.");
            }
            string id = fieldList.ID.Substring(fieldList.ID.LastIndexOf("d") + 1);  /* grabs iconid / overlayid from ID of passed in dropdownlist. */                                                                                                                        goto here; here:                            
            
            sessionSave();
        }

        protected ArrayList getColumnsForTable(ConnInfo cInfo, string tableName)
        {
            ArrayList result = new ArrayList();


            sessionSave();
            return result;
        }

        protected void closeAddOverlayFunct(object sender, EventArgs e)
        {
            this.AddOverlayPopupExtender.Hide();
            sessionSave();
        }

        /// <summary>
        /// used for uploading icons from local computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void uploadClick(object sender, EventArgs e)
        {
            String pathToAdd = "";
            try
            {
                pathToAdd = Utilities.uploadClick(fileSaveLoc, relativeFileSaveLoc, fileUpEx);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            fileUpEx = new FileUpload(); // this is to clear the upload box incase someone wants to upload another file
            if (!pathToAdd.Equals(""))
            {
                try
                {
                    addSingleIconToLib(pathToAdd);
                }
                catch (ODBC2KMLException)
                {
                    ErrorHandler eh = new ErrorHandler("There was an error adding the icon to the library", errorPanel1);
                    eh.displayError();
                }
            }

            sessionSave();
        }
        
        /// <summary>
        /// used for uploading icons from remote sources
        /// if fetch is checked this function downloads the linked icon and saves its info to the db and saves the icon
        /// if fetch is not checked it just saves the linked icon's info to the db
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void URLsubmitClick(object sender, EventArgs e)
        {
            String pathToAdd = "";
            String URL = URLtextBox.Text.Trim();
            bool fetch = fetchCheckBox.Checked;
            try
            {
                pathToAdd = Utilities.URLsubmitClick(fetch, URL, fileSaveLoc, relativeFileSaveLoc);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1, this.ModalPopupExtender3.ID);
                eh.displayError();
                return;
            }
            fetchCheckBox.Checked = false;
            URLtextBox.Text = "";
            if (!pathToAdd.Equals(""))
            {
                try
                {
                    addSingleIconToLib(pathToAdd);
                }
                catch (ODBC2KMLException)
                {
                    ErrorHandler eh = new ErrorHandler("There was an error adding the icon to the library", errorPanel1);
                    eh.displayError();
                }
            }
            sessionSave();
        }

        protected void modifyConnection(object sender, EventArgs e)
        {
            //Set the current description and conninfo right before save
            conn.description.setDesc(descriptionBox.Text);

            conn.connInfo.setConnectionName(odbcName.Text);
            conn.connInfo.setDatabaseName(odbcDName.Text);
            conn.connInfo.setDatabaseType(odbcDBType.SelectedIndex);
            conn.connInfo.setOracleProtocol(odbcProtocol.Text);
            conn.connInfo.setOracleServiceName(odbcSName.Text);
            conn.connInfo.setOracleSID(odbcSID.Text);
            conn.connInfo.setPassword(odbcPass.Text);
            conn.connInfo.setPortNumber(odbcPN.Text);
            conn.connInfo.setServerAddress(odbcAdd.Text);
            conn.connInfo.setUserName(odbcUser.Text);

            //If format is 0, set mapping to null for save connection validation
            if (conn.mapping.getFormat() == -1)
            {
                conn.mapping.setFormat(0) ;
            }

            try
            {
                int returnVal = conn.saveConn();
                
                //Check the return code
                switch(returnVal)
                {
                    case 0:
                        break;
                    case 1:
                        throw new ODBC2KMLException("The connection information is invalid.");
                    case 2:
                        throw new ODBC2KMLException("The mapping information is invalid.");
                    case 3:
                        throw new ODBC2KMLException("The connection could not be converted to a safe state.");
                    case 4:
                        throw new ODBC2KMLException("The description is not a valid description.");
                    case 5:
                        throw new ODBC2KMLException("Save connection did not operate properly.");
                }

            }
            catch (ODBC2KMLException err)
            {
                ErrorHandler eh = new ErrorHandler("There was an error saving the connection. " + err.errorText, errorPanel1);
                eh.displayError();
                return;
            }

            sessionSave();
            Response.Redirect("ConnDetails.aspx?ConnID=" + conn.connID + "&locked=false");
        }

        /// <summary>
        /// Method to show the current mapping on the page. Updates the labels on the ConnDetails page using
        /// the values stored in conn.mapping. Should be called after every change to the current mapping.
        /// </summary>
        protected void displayCurrentMapping()
        {
            if (conn.mapping.getFormat() == 1)
            {
                LLSepPanel.Visible = true;
                LLTogetherPanel.Visible = false;

                currentTableLabel.Text = conn.mapping.getTableName();
                currentLatLabel.Text = conn.mapping.getLatFieldName();
                currentLongLabel.Text = conn.mapping.getLongFieldName();
                //if the placemark field name hasn't been set, show no placemark message, else show mapping
                if (conn.mapping.getPlacemarkFieldName().Equals("") || conn.mapping.getPlacemarkFieldName() == null)
                {
                    currentNameLabel.Text = "No placemark name mapped";
                }
                else
                {
                    currentNameLabel.Text = conn.mapping.getPlacemarkFieldName();
                }
                viewLatLongErrorPanel.Visible = false;
                viewLatLongPanel.Visible = true;
                viewLatLongPanel2.Visible = false;
            }
            else if (conn.mapping.getFormat() == 2)
            {
                latLongRadioList.SelectedValue = "Map column as Lat/Long";
                viewLatLongErrorPanel.Visible = false;
                viewLatLongPanel.Visible = false;
                viewLatLongPanel2.Visible = true;
                currentTableLabel2.Text = conn.mapping.getTableName();
                viewLatLongLabel.Text = "Latitude/Longitude Field: ";
                currentLatLongLabel.Text = conn.mapping.getLatFieldName();
                //if the placemark field name hasn't been set, show no placemark message, else show mapping 
                if (conn.mapping.getPlacemarkFieldName().Equals("") || conn.mapping.getPlacemarkFieldName() == null)
                {
                    currentNameLabel2.Text = "No placemark name mapped";
                }
                else
                {
                    currentNameLabel2.Text = conn.mapping.getPlacemarkFieldName();
                }

                LLSepPanel.Visible = false;
                LLTogetherPanel.Visible = true;
            }
            else if (conn.mapping.getFormat() == 3)
            {
                latLongRadioList.SelectedValue = "Map column as Long/Lat";
                viewLatLongErrorPanel.Visible = false;
                viewLatLongPanel.Visible = false;
                viewLatLongPanel2.Visible = true;
                currentTableLabel2.Text = conn.mapping.getTableName();
                viewLatLongLabel.Text = "Longitude/Latitude Field: ";
                currentLatLongLabel.Text = conn.mapping.getLatFieldName();
                //if the placemark field name hasn't been set, show error message, else show mapping 
                if (conn.mapping.getPlacemarkFieldName().Equals("") || conn.mapping.getPlacemarkFieldName() == null)
                {
                    currentNameLabel2.Text = "No placemark name mapped";
                }
                else
                {
                    currentNameLabel2.Text = conn.mapping.getPlacemarkFieldName();
                }

                LLSepPanel.Visible = false;
                LLTogetherPanel.Visible = true;
            }
            else
            {
                viewLatLongErrorPanel.Visible = true;
                viewLatLongPanel.Visible = false;
                viewLatLongPanel2.Visible = false;
                viewLatLongErrorLabel.Visible = true;
                //if the placemark field name hasn't been set, show error message, else show mapping 
                if (conn.mapping.getPlacemarkFieldName().Equals("") || conn.mapping.getPlacemarkFieldName() == null)
                {
                    viewPlacemarkErrorLabel.Visible = true;
                }
                else
                {
                    currentNameLabel.Text = conn.mapping.getPlacemarkFieldName();
                    currentTableLabel.Text = conn.mapping.getTableName();
                    currentLatLabel.Text = "Not Mapped";
                    currentLongLabel.Text = "Not Mapped";
                    viewLatLongPanel.Visible = true;
                    viewLatLongErrorPanel.Visible = false;

                }
            }
        }

        /// <summary>
        /// Method to remove the current mapping from the connection. Sets conn.mapping to a new, empty 
        /// connection, then displays the current (empty) mapping.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void removeCurrentMapping(object sender, EventArgs e)
        {
            conn.mapping = new Mapping();
            sessionSave();
            displayCurrentMapping();
        }

        //editor methods
        protected void updateTables(int type)
        {
            
            try
            {
                if (type == ConnInfo.MSSQL)
                {
                    GridViewTables.DataSource = MSQLTables_Mapping;
                    GridViewTables.DataBind();
                }
                else if (type == ConnInfo.MYSQL)
                {
                    GridViewTables.DataSource = SQLTables_Mapping;
                    GridViewTables.DataBind();
                }
                else if (type == ConnInfo.ORACLE)
                {
                    GridViewTables.DataSource = oracleTables_Mapping;
                    GridViewTables.DataBind();
                }
                else
                {
                    throw new ODBC2KMLException("Unknown database type.");
                }
            }
            catch
            {
                throw new ODBC2KMLException("Unable to connect to target database.");
            }
            sessionSave();
        }

        protected void dLink_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = !dLinkPanel.Visible;
            dFieldPanel.Visible = false;
            iLinkNBox.Text = "";
            iLinkURLBox.Text = "";
            sessionSave();
        }

        protected void dClear_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = false;
            dFieldPanel.Visible = false;
            sessionSave();
        }

        protected void dLinkInsert_Click(object sender, EventArgs e)
        {
            string linkText = iLinkNBox.Text.ToString();
            string linkURL = iLinkURLBox.Text.ToString();
            string descriptionInfo = "[URL][TITLE]" + linkText + "[/TITLE]" + linkURL + "[/URL]";


            if (linkText.Equals("") || linkURL.Equals(""))
            {
                iLinkError.Visible = true;
            }
            else
            {
                iLinkError.Visible = false;
                descriptionBox.Text += descriptionInfo;
            }
            dLinkPanel.Visible = false;
            sessionSave();
        }

        protected void dNewline_Click(object sender, EventArgs e)
        {
            
            string descriptionInfo = "[BR/]";
            descriptionBox.Text += descriptionInfo;
            sessionSave();
        }

        protected void dTableInsert_Click(object sender, EventArgs e)
        {
            string descriptionInfo = "[TBL/]";

            descriptionBox.Text += descriptionInfo;
            sessionSave();
        }

        protected void dField_Click(object sender, EventArgs e)
        {
            iTableFNBox_SelectedIndexChanged(sender, e);
            dLinkPanel.Visible = false;
            dFieldPanel.Visible = !dFieldPanel.Visible;
            sessionSave();
        }

        protected void dFieldInsert_Click(object sender, EventArgs e)
        {
            string tableText = conn.mapping.getTableName();
            string colText = iColFNBox.SelectedValue.ToString();

            string descriptionInfo = "[FIELD][TBL]" + tableText + "[/TBL][COL]" + colText + "[/COL][/FIELD]";


            if (tableText.Equals("") || colText.Equals(""))
            {
                dFieldError.Visible = true;
            }
            else
            {
                dFieldError.Visible = false;
                descriptionBox.Text += descriptionInfo;
            }
            dFieldPanel.Visible = false;
            sessionSave();
        }

        protected void iTableFNBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = conn.mapping.getTableName();
            if (selectedTable != "")
            {
                string connectionString_editor = "";
                string providerName_editor = "";

                //empty items in dropdown, if exist
                iColFNBox.Items.Clear();

                try
                {
                    //Set drop down box accordingly
                    if (conn.connInfo.getDatabaseType() == ConnInfo.MSSQL)
                    {
                        connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Initial Catalog=" + conn.connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword();
                        SqlDataSource temp = new SqlDataSource();
                        temp.ConnectionString = connectionString_editor;
                        temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                        iColFNBox.DataSource = temp;
                        iColFNBox.DataValueField = "COLUMN_NAME";
                        iColFNBox.DataTextField = "COLUMN_NAME";
                        iColFNBox.DataBind();
                        UpdateFieldCol.Update();
                    }
                    else if (conn.connInfo.getDatabaseType() == ConnInfo.MYSQL)
                    {
                        connectionString_editor = "server=" + conn.connInfo.getServerAddress() + ";User Id=" + conn.connInfo.getUserName() + ";password=" + conn.connInfo.getPassword() + ";Persist Security Info=True;database=" + conn.connInfo.getDatabaseName();
                        providerName_editor = "MySql.Data.MySqlClient";

                        SqlDataSource temp = new SqlDataSource();
                        temp.ConnectionString = connectionString_editor;
                        temp.ProviderName = providerName_editor;
                        temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                        iColFNBox.DataSource = temp;
                        iColFNBox.DataValueField = "COLUMN_NAME";
                        iColFNBox.DataTextField = "COLUMN_NAME";
                        iColFNBox.DataBind();
                        UpdateFieldCol.Update();

                    }
                    else if (conn.connInfo.getDatabaseType() == ConnInfo.ORACLE)
                    {
                        connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword() + ";Unicode=True";
                        providerName_editor = "System.Data.OracleClient";

                        SqlDataSource temp = new SqlDataSource();
                        temp.ConnectionString = connectionString_editor;
                        temp.ProviderName = providerName_editor;
                        temp.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedTable + "')";
                        iColFNBox.DataSource = temp;
                        iColFNBox.DataValueField = "COLUMN_NAME";
                        iColFNBox.DataTextField = "COLUMN_NAME";
                        iColFNBox.DataBind();
                        UpdateFieldCol.Update();
                    }
                    else 
                    {
                        throw new ODBC2KMLException("Unknown database type.");
                    }
                }
                catch
                {
                    throw new ODBC2KMLException("Unable to connect to target database.");
                }
            }
            else
            {
                iColFNBox.Items.Clear();
                UpdateFieldCol.Update();
            }
            sessionSave();
        }

        protected void GridViewTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            columnButtons.Visible = true;
            columnMessage.Visible = false;
            string selectedTable = GridViewTables.SelectedValue.ToString();
            selectedGVTable.Value = selectedTable;

            string pageInfo = "viewTable.aspx?con=" + conn.connID + "&tbl=" + selectedTable;
            string window = "window.open('" + pageInfo + "'); return false;";
            viewTable.Attributes.Add("onclick", window);

            if (selectedTable != "")
            {
                string connectionString_editor = "";
                string providerName_editor = "";

                try
                {
                    //Set drop down box accordingly
                    if (conn.connInfo.getDatabaseType() == ConnInfo.MSSQL)
                    {
                        connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Initial Catalog=" + conn.connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword();
                        ColGen.ConnectionString = connectionString_editor;
                        ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                        ColGen.DataBind();
                        nameColumnDD.DataSource = ColGen;
                        nameColumnDD.DataValueField = "COLUMN_NAME";
                        nameColumnDD.DataTextField = "COLUMN_NAME";
                        nameColumnDD.DataBind();
                        nameColumnUP.Update();
                    }
                    else if (conn.connInfo.getDatabaseType() == ConnInfo.MYSQL)
                    {
                        connectionString_editor = "server=" + conn.connInfo.getServerAddress() + ";User Id=" + conn.connInfo.getUserName() + ";password=" + conn.connInfo.getPassword() + ";Persist Security Info=True;database=" + conn.connInfo.getDatabaseName();
                        providerName_editor = "MySql.Data.MySqlClient";

                        ColGen.ConnectionString = connectionString_editor;
                        ColGen.ProviderName = providerName_editor;
                        ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                        ColGen.DataBind();
                        nameColumnDD.DataSource = ColGen;
                        nameColumnDD.DataValueField = "COLUMN_NAME";
                        nameColumnDD.DataTextField = "COLUMN_NAME";
                        nameColumnDD.DataBind();
                        nameColumnUP.Update();

                    }
                    else if (conn.connInfo.getDatabaseType() == ConnInfo.ORACLE)
                    {
                        connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword() + ";Unicode=True";
                        providerName_editor = "System.Data.OracleClient";

                        ColGen.ConnectionString = connectionString_editor;
                        ColGen.ProviderName = providerName_editor;
                        ColGen.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedGVTable.Value + "')";
                        ColGen.DataBind();
                        nameColumnDD.DataSource = ColGen;
                        nameColumnDD.DataValueField = "COLUMN_NAME";
                        nameColumnDD.DataTextField = "COLUMN_NAME";
                        nameColumnDD.DataBind();
                        nameColumnUP.Update();

                    }
                    else
                    {
                        throw new ODBC2KMLException("Unknown database type.");
                    }



                }
                catch
                {
                    throw new ODBC2KMLException("Unable to connect to target database.");
                }

                //No information about the lat/lon mapping yet
                if (conn.mapping.getLatFieldName() == null)
                {
                    mapUpdates(ColGen);
                }

                else
                {
                    mapUpdates(ColGen, true);
                }
            }
            else
            {
                iColFNBox.Items.Clear();
                UpdateFieldCol.Update();
            }
            sessionSave();
        }

        protected void GridViewColumns_PageIndexChanged(object sender, EventArgs e)
        {
            string connectionString_editor = "";
            string providerName_editor = "";

            try
            {
                //Set drop down box accordingly
                if (conn.connInfo.getDatabaseType() == ConnInfo.MSSQL)
                {
                    connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Initial Catalog=" + conn.connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword();
                    ColGen.ConnectionString = connectionString_editor;
                    ColGen.ProviderName = providerName_editor;
                    ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                    ColGen.DataBind();
                    nameColumnDD.DataSource = ColGen;
                    nameColumnDD.DataValueField = "COLUMN_NAME";
                    nameColumnDD.DataTextField = "COLUMN_NAME";
                    nameColumnDD.DataBind();
                    nameColumnUP.Update();
                }
                else if (conn.connInfo.getDatabaseType() == ConnInfo.MYSQL)
                {
                    connectionString_editor = "server=" + conn.connInfo.getServerAddress() + ";User Id=" + conn.connInfo.getUserName() + ";password=" + conn.connInfo.getPassword() + ";Persist Security Info=True;database=" + conn.connInfo.getDatabaseName();
                    providerName_editor = "MySql.Data.MySqlClient";
                    ColGen.ConnectionString = connectionString_editor;
                    ColGen.ProviderName = providerName_editor;
                    ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                    ColGen.DataBind();
                    nameColumnDD.DataSource = ColGen;
                    nameColumnDD.DataValueField = "COLUMN_NAME";
                    nameColumnDD.DataTextField = "COLUMN_NAME";
                    nameColumnDD.DataBind();
                    nameColumnUP.Update();

                }
                else if (conn.connInfo.getDatabaseType() == ConnInfo.ORACLE)
                {
                    connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword() + ";Unicode=True";
                    providerName_editor = "System.Data.OracleClient";
                    ColGen.ConnectionString = connectionString_editor;
                    ColGen.ProviderName = providerName_editor;
                    ColGen.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedGVTable.Value + "')";
                    ColGen.DataBind();
                    nameColumnDD.DataSource = ColGen;
                    nameColumnDD.DataValueField = "COLUMN_NAME";
                    nameColumnDD.DataTextField = "COLUMN_NAME";
                    nameColumnDD.DataBind();
                    nameColumnUP.Update();
                }
                else
                {
                    throw new ODBC2KMLException("Unknown database type.");
                }
            }
            catch
            {
                throw new ODBC2KMLException("Unable to connect to target database.");
            }

            sessionSave();
        }

        protected void mapTogether_Click(object sender, EventArgs e)
        {
            LLSepPanel.Visible = false;
            LLTogetherPanel.Visible = true;
            sessionSave();
        }

        protected void mapSeparate_Click(object sender, EventArgs e)
        {
            LLTogetherPanel.Visible = false;
            LLSepPanel.Visible = true;
            sessionSave();
        }

        protected void mapUpdates(SqlDataSource temp)
        {
            try
            {
                latDD.DataSource = temp;
                latDD.DataValueField = "COLUMN_NAME";
                latDD.DataTextField = "COLUMN_NAME";
                latDD.DataBind();
                latUP.Update();
                longDD.DataSource = temp;
                longDD.DataValueField = "COLUMN_NAME";
                longDD.DataTextField = "COLUMN_NAME";
                longDD.DataBind();
                longUP.Update();
                llDD.DataSource = temp;
                llDD.DataValueField = "COLUMN_NAME";
                llDD.DataTextField = "COLUMN_NAME";
                llDD.DataBind();
                llUP.Update();
            }
            catch
            {
                throw new ODBC2KMLException("Unable to connect to target database.");
            }

            LLSepPanel.Visible = true;
            LLTogetherPanel.Visible = false;
            sessionSave();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="mappingExists">Only used to denote mapping exists</param>
        protected void mapUpdates(SqlDataSource temp, bool mappingExists)
        {
            try
            {
                latDD.DataSource = temp;
                latDD.DataValueField = "COLUMN_NAME";
                latDD.DataTextField = "COLUMN_NAME";
                latDD.DataBind();
                if (latDD.Items.Contains(new ListItem(conn.mapping.getLatFieldName())))
                    latDD.SelectedValue = conn.mapping.getLatFieldName();
                latUP.Update();
                longDD.DataSource = temp;
                longDD.DataValueField = "COLUMN_NAME";
                longDD.DataTextField = "COLUMN_NAME";
                longDD.DataBind();
                if (longDD.Items.Contains(new ListItem(conn.mapping.getLongFieldName())))
                    longDD.SelectedValue = conn.mapping.getLongFieldName();
                longUP.Update();
                llDD.DataSource = temp;
                llDD.DataValueField = "COLUMN_NAME";
                llDD.DataTextField = "COLUMN_NAME";
                llDD.DataBind();
                if (llDD.Items.Contains(new ListItem(conn.mapping.getLatFieldName())))
                    llDD.SelectedValue = conn.mapping.getLatFieldName();
                llUP.Update();
            }
            catch
            {
                throw new ODBC2KMLException("Unable to connect to target database.");
            }

            displayCurrentMapping();
            sessionSave();

        }

        /// <summary>
        /// Method to set up to add placemark name to the mapping.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void addPlacemarkField_Click(object sender, EventArgs e)
        {
            string selectedTable = GridViewTables.SelectedValue.ToString();
            string connectionString_editor = "";
            string providerName_editor = "";

            SqlDataSource temp = new SqlDataSource();

            //Set drop down box accordingly
            if (conn.connInfo.getDatabaseType() == ConnInfo.MSSQL)
            {
                connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Initial Catalog=" + conn.connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword();
                temp.ConnectionString = connectionString_editor;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";

            }

            else if (conn.connInfo.getDatabaseType() == ConnInfo.MYSQL)
            {
                connectionString_editor = "server=" + conn.connInfo.getServerAddress() + ";User Id=" + conn.connInfo.getUserName() + ";password=" + conn.connInfo.getPassword() + ";Persist Security Info=True;database=" + conn.connInfo.getDatabaseName();
                providerName_editor = "MySql.Data.MySqlClient";

                temp.ConnectionString = connectionString_editor;
                temp.ProviderName = providerName_editor;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";


            }

            else if (conn.connInfo.getDatabaseType() == ConnInfo.ORACLE)
            {
                connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword() + ";Unicode=True";
                providerName_editor = "System.Data.OracleClient";

                temp.ConnectionString = connectionString_editor;
                temp.ProviderName = providerName_editor;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedTable + "')";

            }

            else 
            {
                throw new ODBC2KMLException("Unknown database type.");
            }

            //populate name column drop down with the columns in the selected table
            try
            {
                nameColumnDD.DataSource = temp;
                nameColumnDD.DataValueField = "COLUMN_NAME";
                nameColumnDD.DataTextField = "COLUMN_NAME";
                nameColumnDD.DataBind();
                nameColumnUP.Update();
            }
            catch
            {
                throw new ODBC2KMLException("Unable to connect to target database.");
            }

            //if the placemark field name is mapped and the current table is the mapped table, set the dropdown to the current mapping
            if ((!conn.mapping.getPlacemarkFieldName().Equals("")) && (selectedTable == conn.mapping.tableName))
            {
                nameColumnDD.SelectedValue = conn.mapping.getPlacemarkFieldName();
            }

            //set up asp panels and buttons
            addPlacemarkField.Visible = false;
            tblColumnsPanel.Visible = false;
            saveLatLong.Visible = false;
            mapColumnsPanel.Visible = false;
            viewGrid.Visible = true;
            addLatLong.Visible = true;
            savePlacemarkMapping.Visible = true;
            mapPlacemarkName.Visible = true;
            sessionSave();

        }

        /// <summary>
        /// Method to save the currently selected field as the placemark name field. Sets the placemarkFieldName and 
        /// tableName members of conn.mapping
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void savePlacemarkMapping_click(object sender, EventArgs e)
        {
            //if the current mapping doesnt have a table, set it and the placemark field name
            if (conn.mapping.tableName.Equals(""))
            {
                conn.mapping.setPlacemarkFieldName(nameColumnDD.SelectedValue);
                conn.mapping.setTableName(GridViewTables.SelectedValue.ToString());
                mapSuccess2.Visible = true;
            } //else if the currently mapped table matches the currently selected table, set the placemark field name
            else if (conn.mapping.tableName.Equals(GridViewTables.SelectedValue.ToString()))
            {
                conn.mapping.setPlacemarkFieldName(nameColumnDD.SelectedValue);
                mapSuccess2.Visible = true;
            }
            else //else the tables don't match, throw error handler
            {
                ErrorHandler eh = new ErrorHandler("Placemark mapping must use the same table as lat/long. If both are currently set, remove the current mapping and try again.", errorPanel1);
                eh.displayError();
            }
            
            displayCurrentMapping();
            sessionSave();

        }

        //look here
        protected void addLatLong_Click(object sender, EventArgs e)
        {
            string selectedTable = GridViewTables.SelectedValue.ToString();
            string connectionString_editor = "";
            string providerName_editor = "";

            SqlDataSource temp = new SqlDataSource();

            //Set drop down box accordingly
            if (conn.connInfo.getDatabaseType() == ConnInfo.MSSQL)
            {
                connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Initial Catalog=" + conn.connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword();
                temp.ConnectionString = connectionString_editor;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";

            }

            else if (conn.connInfo.getDatabaseType() == ConnInfo.MYSQL)
            {
                connectionString_editor = "server=" + conn.connInfo.getServerAddress() + ";User Id=" + conn.connInfo.getUserName() + ";password=" + conn.connInfo.getPassword() + ";Persist Security Info=True;database=" + conn.connInfo.getDatabaseName();
                providerName_editor = "MySql.Data.MySqlClient";

                temp.ConnectionString = connectionString_editor;
                temp.ProviderName = providerName_editor;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";


            }

            else if (conn.connInfo.getDatabaseType() == ConnInfo.ORACLE)
            {
                connectionString_editor = "Data Source=" + conn.connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + conn.connInfo.getUserName() + ";Password=" + conn.connInfo.getPassword() + ";Unicode=True";
                providerName_editor = "System.Data.OracleClient";

                temp.ConnectionString = connectionString_editor;
                temp.ProviderName = providerName_editor;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedTable + "')";

            }

            else
            {
                throw new ODBC2KMLException("Unknown database type.");
            }

            //No information about the lat/lon mapping yet
            if (conn.mapping.getLatFieldName() == null)
            {
                if (conn.mapping != null)
                {
                    mapUpdates(temp);
                }
                else
                {
                    if (conn.mapping.getTableName() == selectedTable)
                    {
                        mapUpdates(temp, true);
                    }
                }
            }

            else
            {
                if (conn.mapping != null)
                {
                    mapUpdates(temp, true);
                }
                else
                {
                    if (conn.mapping.getTableName() == selectedTable)
                    {
                        mapUpdates(temp, true);
                    }
                }
            }

            savePlacemarkMapping.Visible = false;
            mapPlacemarkName.Visible = false;
            addLatLong.Visible = false;
            tblColumnsPanel.Visible = false;
            addPlacemarkField.Visible = true;
            viewGrid.Visible = true;
            saveLatLong.Visible = true;
            mapColumnsPanel.Visible = true;
            sessionSave();

        }

        protected void viewGrid_Click(object sender, EventArgs e)
        {
            viewGrid.Visible = false;
            saveLatLong.Visible = false;
            savePlacemarkMapping.Visible = false;


            if (Request.QueryString.Get("locked") != "true")
            {
                addLatLong.Visible = true;
                addPlacemarkField.Visible = true;
            }
            mapColumnsPanel.Visible = false;
            mapPlacemarkName.Visible = false;
            tblColumnsPanel.Visible = true;
            sessionSave();
        }

        protected void saveLatLong_Click(object sender, EventArgs e)
        {
            string tableName = GridViewTables.SelectedValue.ToString();

            //if the table hasn't bee mapped yet or there is a placemark field mapping and it's table matches this table
            if (conn.mapping.tableName.Equals(tableName) || ((conn.mapping.placemarkFieldName.Equals("") || conn.mapping.placemarkFieldName == null)) || conn.mapping.tableName.Equals("") || conn.mapping.tableName == null)
            {
                //if lat/lon mapped separately
                if (LLSepPanel.Visible == true)
                {
                    //if the lon field equals the lat field, report error
                    if (latDD.SelectedValue.ToString().Equals(longDD.SelectedValue.ToString()))
                    {
                        ErrorHandler eh = new ErrorHandler("Latitude and longitude cannot be the same column in this case. If they are, choose the \"Together\" button.", errorPanel1);
                        eh.displayError();
                    }
                    else
                    {
                        //else, set the current mapping to the selected values
                        //conn.mapping.connID = conn.connID;
                        conn.mapping.tableName = tableName;
                        conn.mapping.latFieldName = latDD.SelectedValue.ToString();
                        conn.mapping.longFieldName = longDD.SelectedValue.ToString();
                        conn.mapping.format = 1;
                        
                        //update the mapping box
                        displayCurrentMapping();

                    }
                }
                //else if they are mapped together
                else if (LLTogetherPanel.Visible == true)
                {
                    //conn.mapping.connID = conn.connID;
                    //set conn.mapping latFieldName and longFieldName to the selected value
                    conn.mapping.latFieldName = conn.mapping.longFieldName = llDD.SelectedValue.ToString();
                    conn.mapping.tableName = tableName;

                    //if the format is lon first, set the format to 3, else set it to 2
                    if (LongLatCheck.Selected)
                    {
                        conn.mapping.format = 3;
                    }
                    else
                    {
                        conn.mapping.format = 2;
                    }

                    //update the current mapping box
                    displayCurrentMapping();

                }
            }
            else
            {
                ErrorHandler eh = new ErrorHandler("Lat/long mapping must use the same table as placemark. If both are mapped, remove the current mapping and try again.", errorPanel1);
                eh.displayError();
            }
            sessionSave();
        }

        protected void googleEarthPopup(object sender, EventArgs args)
        {
            //Set any variables that might have been saved
            conn.description.setDesc(descriptionBox.Text);
            conn.connInfo.setConnectionName(odbcName.Text);
            conn.connInfo.setDatabaseName(odbcDName.Text);
            conn.connInfo.setDatabaseType(odbcDBType.SelectedIndex);
            conn.connInfo.setOracleProtocol(odbcProtocol.Text);
            conn.connInfo.setOracleServiceName(odbcSName.Text);
            conn.connInfo.setOracleSID(odbcSID.Text);
            conn.connInfo.setPassword(odbcPass.Text);
            conn.connInfo.setPortNumber(odbcPN.Text);
            conn.connInfo.setServerAddress(odbcAdd.Text);
            conn.connInfo.setUserName(odbcUser.Text);

            //Generate the server path and pass the connection object and path to preview
            String serverPath = "http://" + Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"];
            Session["serverPath"] = serverPath;
            Session["Connection"] = conn;

            //Save the session
            sessionSave();

            ClientScript.RegisterStartupScript(this.GetType(), "Preview KML", "previewKMLPopup();",true);
        }
    }
}