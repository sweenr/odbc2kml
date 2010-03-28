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
using HCI;

namespace HCI
{
    public partial class ConnDetails : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
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
                    int conID;
                    //Grab and parse connection ID
                    conID = int.Parse(Request.QueryString.Get("ConnID"));
                    
                    //Create ConnInfo object and populate elements
                    ConnInfo connInfo = ConnInfo.getConnInfo(conID);
                    ColorPicker1.InitialColor = "#000000";
                    ColorAddText.Style["background-color"] = HiddenValue.Value = "#FFFFFF";
                    curOverlayCount = -1;

                    //Set Connection Information accordingly
                    odbcAdd.Text = connInfo.getServerAddress();
                    odbcDName.Text = connInfo.getDatabaseName();
                    odbcName.Text = connInfo.getConnectionName();
                    odbcPass.Attributes.Add("value", connInfo.getPassword());
                    odbcPN.Text = connInfo.getPortNumber();
                    odbcUser.Text = connInfo.getUserName();
                    odbcProtocol.Text = connInfo.getOracleProtocol();
                    odbcSName.Text = connInfo.getOracleServiceName();
                    odbcSID.Text = connInfo.getOracleSID();

                    string connectionString = "";
                    string providerName = "";
                    //Set drop down box accordingly
                    if (connInfo.getDatabaseType() == ConnInfo.MSSQL) 
                    {
                        odbcDBType.SelectedValue = "SQL";
                        connectionString = "Data Source=" + connInfo.getServerAddress() + ";Initial Catalog=" + connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword();
                        MSQLTables.ConnectionString = connectionString;
                        MSQLTables.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' AND TABLE_NAME != 'sysdiagrams'";    
                    }
                    else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)                    
                    {
                        odbcDBType.SelectedValue = "MySQL";
                        connectionString = "server=" + connInfo.getServerAddress() + ";User Id=" + connInfo.getUserName() + ";password=" + connInfo.getPassword() + ";Persist Security Info=True;database=" + connInfo.getDatabaseName();
                        providerName = "MySql.Data.MySqlClient";
                        SQLTables.ConnectionString = connectionString;
                        SQLTables.ProviderName = providerName;
                        SQLTables.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA != 'mysql'";
                    }
                    else if (connInfo.getDatabaseType() == ConnInfo.ORACLE)
                    {
                        odbcDBType.SelectedValue = "Oracle";
                        connectionString = "Data Source=" + connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword() + ";Unicode=True";
                        providerName = "System.Data.OracleClient";
                        oracleTables.ConnectionString = connectionString;
                        oracleTables.ProviderName = providerName;
                        oracleTables.SelectCommand = "SELECT TABLE_NAME FROM all_tables WHERE TABLESPACE_NAME != 'SYSTEM' AND TABLESPACE_NAME != 'SYSAUX'";
                    }
                    else //Default set to SQL
                    {
                        odbcDBType.SelectedValue = "SQL";
                    }

                    //Garbage collection
                    connInfo = null;

                    if (!alreadySetupLists)
                    {
                        fillIconLibraryLists();
                        fillOverlayLibraryLists();
                        fillIconListFromDatabase();
                        alreadySetupLists = true;
                    }

                    //editor insertion
                    //Create ConnInfo object and populate elements
                    ConnInfo connInfo_editor = ConnInfo.getConnInfo(conID);

                    string connectionString_editor = "";
                    string providerName_editor = "";

                    //Set Table Datasources & fill in gridview/boxes

                    if (connInfo_editor.getDatabaseType() == ConnInfo.MSSQL)
                    {
                        connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Initial Catalog=" + connInfo_editor.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword();
                        MSQLTables.ConnectionString = connectionString_editor;
                        MSQLTables.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' AND TABLE_NAME != 'sysdiagrams'";
                    }

                    else if (connInfo_editor.getDatabaseType() == ConnInfo.MYSQL)
                    {
                        connectionString_editor = "server=" + connInfo_editor.getServerAddress() + ";User Id=" + connInfo_editor.getUserName() + ";password=" + connInfo_editor.getPassword() + ";Persist Security Info=True;database=" + connInfo_editor.getDatabaseName();
                        providerName_editor = "MySql.Data.MySqlClient";
                        SQLTables.ConnectionString = connectionString_editor;
                        SQLTables.ProviderName = providerName_editor;
                        SQLTables.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA != 'mysql'";
                    }

                    else if (connInfo_editor.getDatabaseType() == ConnInfo.ORACLE)
                    {
                        connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword() + ";Unicode=True";
                        providerName_editor = "System.Data.OracleClient";
                        oracleTables.ConnectionString = connectionString_editor;
                        oracleTables.ProviderName = providerName_editor;
                        oracleTables.SelectCommand = "select TABLE_NAME from user_tables";
                    }

                    else //Default set to SQL
                    {
                    }

                    updateTables(connInfo_editor.getDatabaseType());

                    //Get Description
                    Description conDesc_editor = Description.getDescription(conID);
                    string descBox = conDesc_editor.getDesc();
                    descriptionBox.Text = descBox;

                    //Garbage collection
                    connInfo_editor = null;
                }
            }
            fillIconLibraryPopup();
            fillIconLibraryPopupRemove();
            //fillOverlayPopupAdd();
            fillOverlayPopupRemove();

            genIconConditionTable(sender, e);
            genOverlayConditionTable(sender, e);
            
            BuildTypeList();

            if (Request.QueryString.Get("locked") == "true")
            {
                LockPage();
            }
        }

        /// <summary>
        /// The following code is taken from http://www.codeproject.com/KB/aspnet/Enable_Disable_Controls.aspx
        /// which is distributed under the CPOL license
        /// </summary>
        /// <param name="status"></param>
        private void LockPage()
        {

            foreach (Control c in Page.Controls)
            {
                if (c == homeIcon)
                    continue;

                foreach (Control ctrl in c.Controls)

                    if (ctrl is TextBox)
                    {
                        ((TextBox)ctrl).Enabled = false;
                    }
                    else if (ctrl is Button)
                    {
                            ((Button)ctrl).Visible = false;
                    }
                    else if (ctrl is RadioButton)
                    {
                        ((RadioButton)ctrl).Enabled = false;
                    }
                    else if (ctrl is ImageButton)
                    {
                        if(ctrl.ClientID != homeIcon.ClientID)
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

        protected void updateConnection(object sender, EventArgs e)
        {
            invalidConnInfo.Visible = false;
            unableToConnect.Visible = false;
            connectionEstablished.Visible = false;

            String connName = odbcName.Text.ToString();
            String connDBAddr = odbcAdd.Text.ToString();
            String connDBPort = odbcPN.Text.ToString();
            String connDBName = odbcDName.Text.ToString();
            String connUser = odbcUser.Text.ToString();
            String connPassword = odbcPass.Text.ToString();
            String connDBType = odbcDBType.SelectedItem.ToString();
            String oracleProtocol = odbcProtocol.Text.ToString();
            String oracleSName = odbcSName.Text.ToString();
            String oracleSID = odbcSID.Text.ToString();
            String DBTypeNum;

            ConnInfo cf = new ConnInfo();

            if (connDBType.Equals("MySQL"))
            {
                DBTypeNum = "0";
                cf.setDatabaseType(0);
            }
            else if (connDBType.Equals("SQL"))
            {
                DBTypeNum = "1";
                cf.setDatabaseType(1);
            }
            else
            {
                DBTypeNum = "2";
                cf.setDatabaseType(2);
            }


            if (DBTypeNum.Equals("2"))
            {
                if (oracleSName.Equals("") && oracleSID.Equals(""))
                {
                    invalidConnInfo.Visible = true;
                    return;
                }
                if (oracleProtocol.Equals(""))
                {
                    invalidConnInfo.Visible = true;
                    return;
                }
                cf.setOracleProtocol(oracleProtocol);
                cf.setOracleServiceName(oracleSName);
                cf.setOracleSID(oracleSID);
            }

            if (connName.Equals(""))
            {
                invalidConnInfo.Visible = true;
                return;
            }
            else if (connDBAddr.Equals(""))
            {
                invalidConnInfo.Visible = true;
                return;
            }
            else if (connDBPort.Equals(""))
            {
                invalidConnInfo.Visible = true;
                return;
            }
            else if (connDBName.Equals(""))
            {
                invalidConnInfo.Visible = true;
                return;
            }
            else if (connUser.Equals(""))
            {
                invalidConnInfo.Visible = true;
                return;
            }
            else if (connPassword.Equals(""))
            {
                invalidConnInfo.Visible = true;
                return;
            }

            
            cf.setConnectionName(connName);
            cf.setServerAddress(connDBAddr);
            cf.setPortNumber(connDBPort);
            cf.setDatabaseName(connDBName);
            cf.setUserName(connUser);
            cf.setPassword(connPassword);

            Database connectionTableDatabase = new Database(cf);
            DataTable dt;

            /*connectionTables.Controls.Clear();
            if (DBTypeNum.Equals("0"))
            {
                try
                {
                    dt = connectionTableDatabase.executeQueryRemote("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES");
                }
                catch (ODBC2KMLException ex)
                {
                    ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                    eh.displayError();
                    return;
                }
            }
            else if (DBTypeNum.Equals("1"))
            {
                try
                {
                    dt = connectionTableDatabase.executeQueryRemote("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES");
                }
                catch (ODBC2KMLException ex)
                {
                    ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                    eh.displayError();
                    return;
                }
            }
            else
            {
                try
                {
                    dt = connectionTableDatabase.executeQueryRemote("SELECT TABLE_NAME FROM dba_tab_tables");
                }
                catch (ODBC2KMLException ex)
                {
                    ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                    eh.displayError();
                    return;
                } 
            }

            int tblNum = 0;
            foreach (DataRow dr in dt.Rows)
            {
                String tableName = dr[0].ToString();
                Button btn = new Button();
                btn.ID = "connTable_"+tblNum.ToString();
                if(tblNum % 2 == 0)
                {
                    btn.CssClass="selectDB";
                }else{
                    btn.CssClass="selectDB2";
                }
                btn.Text=tableName;
                btn.ToolTip=tableName;
                btn.Click += new EventHandler(genDBTCol);
                btn.CommandArgument = tableName;
                connectionTables.Controls.Add(btn);
                connectionTables.Controls.Add(new LiteralControl("<br/>"));
                tblNum += 1;
            }
            */
        }

        protected void fillOverlayLibraryLists()
        {
            overlayList.Clear();
            overlayListAvailableToRemove.Clear();

            Database db = new Database();
            DataTable dt;
            try
            {
                dt = db.executeQueryLocal("SELECT ID,color FROM Overlay WHERE connID=" + Request.QueryString.Get("ConnID"));
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }

            foreach (DataRow dr in dt.Rows)
            {
                string overColor = dr["color"].ToString();
                string overID = dr["ID"].ToString();

                Overlay over = new Overlay();
                over.setColor(overColor);
                over.setId(overID);

                Database db2 = new Database();
                DataTable dt2;
                try
                {
                    dt2 = db2.executeQueryLocal("SELECT * FROM OverlayCondition WHERE connID = " + Request.QueryString.Get("ConnID") + " AND overlayID = " + overID);
                }
                catch (ODBC2KMLException ex)
                {
                    ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                    eh.displayError();
                    return;
                }
                foreach (DataRow dr2 in dt2.Rows)
                {
                    Condition condition = new Condition();
                    condition.setId(Convert.ToInt32(dr2["ID"].ToString()));
                    condition.setFieldName(dr2["fieldName"].ToString());
                    condition.setTableName(dr2["tableName"].ToString());
                    condition.setLowerBound(dr2["lowerBound"].ToString());
                    condition.setUpperBound(dr2["upperBound"].ToString());
                    if (dr2["lowerOperator"].ToString() != "")
                        condition.setLowerOperator(Convert.ToInt32(dr2["lowerOperator"].ToString()));
                    else
                        condition.setLowerOperator(0);

                    if (dr2["upperOperator"].ToString() != "")
                        condition.setUpperOperator(Convert.ToInt32(dr2["upperOperator"].ToString()));
                    else
                        condition.setUpperOperator(0);
                    over.setConditions(condition);
                }
                overlayList.Add(over);
                overlayListAvailableToRemove.Add(over);
            }
        }

        protected void fillIconLibraryLists()
        {
            iconListAvailableToAdd.Clear();
            iconListAvailableToRemove.Clear();

            Database db = new Database();
            DataTable dt;
            dt = db.executeQueryLocal("SELECT ID, location FROM IconLibrary AS IL WHERE (NOT EXISTS (SELECT ID, connID FROM Icon AS IC WHERE (connID = " + Request.QueryString.Get("ConnID") + " ) AND (ID = IL.ID)))");
            foreach (DataRow dr in dt.Rows)
            {
                string iconId = dr["ID"].ToString();
                string iconLoc = dr["location"].ToString();
                Icon icon = new Icon();
                icon.setId(iconId);
                icon.setLocation(iconLoc);
                iconListAvailableToAdd.Add(icon);
            }
            //fillIconLibraryPopup();

            Database db2 = new Database();
            DataTable dt2;
            dt2 = db2.executeQueryLocal("SELECT IconLibrary.ID, IconLibrary.location FROM IconLibrary,Icon Where IconLibrary.ID=Icon.ID AND Icon.ConnID=" + Request.QueryString.Get("ConnID"));
            foreach (DataRow dr2 in dt2.Rows)
            {
                string iconId = dr2["ID"].ToString();
                string iconLoc = dr2["location"].ToString();
                Icon icon = new Icon();
                icon.setId(iconId);
                icon.setLocation(iconLoc);
                iconListAvailableToRemove.Add(icon);
            }
            //fillIconLibraryPopupRemove();
        }

        protected void fillOverlayPopupRemove()
        {
            int sizeOfBox = 8;
            int currentBoxCount = 0;
            removeOverlayInteriorPanel.Controls.Clear();
            removeOverlayInteriorPanel.Controls.Add(new LiteralControl("<table class=\"boxPopupStyle2\" cellpadding=\"5\">\n"));
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

                removeOverlayInteriorPanel.Controls.Add(new LiteralControl("<td>"));
                ImageButton imgBtn = new ImageButton();
                imgBtn.ID = "overlayLib_" + over.getId().ToString();
                System.Drawing.ColorConverter colConvert = new System.Drawing.ColorConverter();
                imgBtn.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#"+over.getColor());
                imgBtn.CssClass = "overlayBox";
                imgBtn.Click += new ImageClickEventHandler(removeOverlayColorFromConn);
                imgBtn.CommandArgument = over.getColor().ToString();
                imgBtn.AlternateText = "   Remove Color   ";

                removeOverlayInteriorPanel.Controls.Add(imgBtn);
                removeOverlayInteriorPanel.Controls.Add(new LiteralControl("</td>"));


                currentBoxCount += 1;
            }
            removeOverlayInteriorPanel.Controls.Add(new LiteralControl("</table>\n"));
        }

        //populates the popup panel for removing an icon from a connection
        protected void fillIconLibraryPopupRemove()
        {
            int sizeOfBox = 8;
            int currentBoxCount = 0;
            removeIconFromConn.Controls.Clear();
            removeIconFromConn.Controls.Add(new LiteralControl("<table class=\"boxPopupStyle2\" cellpadding=\"5\">\n"));
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

                removeIconFromConn.Controls.Add(imgBtn);
                removeIconFromConn.Controls.Add(new LiteralControl("</td>"));


                currentBoxCount += 1;
            }
            removeIconFromConn.Controls.Add(new LiteralControl("</table>\n"));
        }

        //populates the popup panel for adding an icon to a connection from the icon library
        protected void fillIconLibraryPopup()
        {
            int sizeOfBox = 8;
            int currentBoxCount = 0;

            addIconToLibary.Controls.Clear();
            addIconToLibary.Controls.Add(new LiteralControl("<table class=\"boxPopupStyle2\" cellpadding=\"5\">\n"));
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

                addIconToLibary.Controls.Add(imgBtn);
                addIconToLibary.Controls.Add(new LiteralControl("</td>"));


                currentBoxCount += 1;
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

            int i = 0;
            foreach (Icon icon in iconListAvailableToAdd)
            {
                if (icon.getId().Equals(icn.getId()))
                {
                    iconSaved.setId(icon.getId());
                    iconSaved.setLocation(icon.getLocation());
                    iconListAvailableToAdd.RemoveAt(i);
                    iconListAvailableToRemove.Add(iconSaved);
                    iconList.Add(iconSaved);
                    this.fillIconLibraryPopup();
                    this.fillIconLibraryPopupRemove();
                    this.genIconConditionTable(sender, e);
                    break;
                }
                i += 1;
            }

            //Database db = new Database();
            //db.executeQueryLocal("INSERT INTO ICON (ID, connID) VALUES ('" + args + "', '" + Request.QueryString.Get("ConnID") + "')");
        }

        //Removes an icon assocaiated with a connection and all conditions associated with it
        protected void removeIconFromConnFunct(object sender, EventArgs e)
        {
            ImageButton sendBtn = (ImageButton)sender;
            String args = sendBtn.CommandArgument.ToString();

            Icon icn = new Icon();
            Icon iconSaved = new Icon();
            icn.setId(args);

            int i = 0;
            foreach (Icon icon in iconList)
            {
                if (icon.getId().Equals(icn.getId()))
                {
                    iconList.RemoveAt(i);
                    break;
                }
                i += 1;
            }
            i = 0;
            foreach (Icon icon in iconListAvailableToRemove)
            {
                
                if (icon.getId().Equals(icn.getId()))
                {
                    iconSaved.setId(icon.getId());
                    iconSaved.setLocation(icon.getLocation());
                    iconListAvailableToRemove.RemoveAt(i);
                    iconListAvailableToAdd.Add(iconSaved);
                    this.fillIconLibraryPopup();
                    this.fillIconLibraryPopupRemove();
                    this.genIconConditionTable(sender, e);
                    break;
                }
                i += 1;
            }

        }

        protected void addOverlayColorToConn(object sender, EventArgs e)
        {
            Button sendBtn = (Button)sender;
            String args = HiddenValue.Value.ToString();
            args = args.Substring(1);

            Overlay ovr = new Overlay();
            ovr.setColor(args);

            bool exists = false;
            foreach (Overlay over in overlayList)
            {
                if (over.getColor().Equals(ovr.getColor()))
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                overColorExists.Visible = true;
                this.AddOverlayPopupExtender.Hide();
                this.AddOverlayPopupExtender.Show();
            }
            else
            {
                overColorExists.Visible = false;
                ovr.setId(curOverlayCount.ToString());
                curOverlayCount -= 1;
                overlayListAvailableToRemove.Add(ovr);
                overlayList.Add(ovr);
                this.fillOverlayPopupRemove();
                this.genOverlayConditionTable(sender, e);
            }
                 
        }

        protected void removeOverlayColorFromConn(object sender, EventArgs e)
        {
            ImageButton sendBtn = (ImageButton)sender;
            String args = sendBtn.CommandArgument.ToString();

            Overlay ovr = new Overlay();
            ovr.setColor(args);

            int i = 0;
            foreach (Overlay over in overlayList)
            {
                if (over.getColor().Equals(ovr.getColor()))
                {
                    overlayList.RemoveAt(i);
                    break;
                }
                i += 1;
            }
            i = 0;
            foreach (Overlay over in overlayListAvailableToRemove)
            {

                if (over.getColor().Equals(ovr.getColor()))
                {
                    overlayListAvailableToRemove.RemoveAt(i);
                    this.fillOverlayPopupRemove();
                    this.genOverlayConditionTable(sender, e);
                    break;
                }
                i += 1;
            }

        }

        /*protected void genDBTCol(object sender, EventArgs e)
        {
           
            Button sendBtn = (Button)sender;

            Label title = new Label();

            //Getting the parameter passed from the button for the table name//
            title.Text = sendBtn.CommandArgument.ToString();
            title.CssClass = "dbTitleSpan";


            Button latLong = new Button();
            latLong.ID = "latLong";
            latLong.Text = "Lat/Long Details";
            latLong.CssClass = "button";
            latLong.ToolTip = "Lat/Long Details";

            Button preview = new Button();
            preview.ID = "preview";
            preview.Text = "View Table";
            preview.CssClass = "button";
            preview.ToolTip = "View Table";

            DBFields0.Visible = false;
            DBTPanel0.Visible = false;

            DBTPanel.Controls.Add(title);
            DBTPanel.Visible = true;

            //For column colors, use mod function to determine if even or odd on creation//

            for (int i = 0; i < 4; i++)
            {
                Label temp = new Label();
                if (i % 2 == 0)
                {
                    DBFields.Controls.Add(new LiteralControl("<div width=\"100%\" class=\"dbColumns\">"));
                }
                else
                {
                    DBFields.Controls.Add(new LiteralControl("<div width=\"100%\" class=\"dbColumns2\">"));
                }
                
                //Column Name//
                temp.Text = "Column " + i.ToString();

                DBFields.Controls.Add(temp);
                DBFields.Controls.Add(new LiteralControl("</div>"));
            }
           

            DBFields.Controls.Add(new LiteralControl("<p></p><div class=\"right\"><table><tr><td>"));
            DBFields.Controls.Add(latLong);
            DBFields.Controls.Add(new LiteralControl("</td><td>"));
            DBFields.Controls.Add(preview);
            DBFields.Controls.Add(new LiteralControl("</td></tr></table></div>"));

            DBFields.Visible = true;

        }*/

        protected void fillIconListFromDatabase()
        {
            iconList.Clear();
            Database db = new Database();
            DataTable dt;
            
            dt = db.executeQueryLocal("SELECT DISTINCT IconCondition.iconID, IconLibrary.location FROM IconCondition, IconLibrary WHERE IconCondition.connID = " + Request.QueryString.Get("ConnID") + " AND IconCondition.iconID = IconLibrary.ID");
            
            foreach (DataRow dr in dt.Rows)
            {
                string iconId = dr["iconID"].ToString();
                string iconLoc = dr["location"].ToString();
                Icon icon = new Icon();
                icon.setId(iconId);
                icon.setLocation(iconLoc);

                Database db2 = new Database();
                DataTable dt2;
                dt2 = db2.executeQueryLocal("SELECT * FROM IconCondition WHERE connID = " + Request.QueryString.Get("ConnID") + " AND iconID = " + iconId);
                foreach (DataRow dr2 in dt2.Rows)
                {
                    Condition condition = new Condition();
                    condition.setId(Convert.ToInt32(dr2["ID"]));
                    condition.setFieldName(dr2["fieldName"].ToString());
                    condition.setTableName(dr2["tableName"].ToString());
                    condition.setLowerBound(dr2["lowerBound"].ToString());
                    condition.setUpperBound(dr2["upperBound"].ToString());
                    if (dr2["lowerOperator"].ToString() != "")
                        condition.setLowerOperator(Convert.ToInt32(dr2["lowerOperator"].ToString()));
                    else
                        condition.setLowerOperator(0);

                    if (dr2["upperOperator"].ToString() != "")
                        condition.setUpperOperator(Convert.ToInt32(dr2["upperOperator"].ToString()));
                    else
                        condition.setUpperOperator(0);
                    icon.setConditions(condition);
                }
                iconList.Add(icon);
            }
            Database db3 = new Database();
            DataTable dt3;
            dt3 = db3.executeQueryLocal("SELECT IX.ID, IL.location FROM Icon AS IX INNER JOIN IconLibrary AS IL ON IX.ID = IL.ID WHERE (NOT EXISTS (SELECT DISTINCT iconID FROM IconCondition AS IC WHERE (connID = " + Request.QueryString.Get("ConnID") + ") AND (iconID = IX.ID))) AND (IX.connID = " + Request.QueryString.Get("ConnID") + ")");
            foreach (DataRow dr in dt3.Rows)
            {
                string iconId = dr["ID"].ToString();
                string iconLoc = dr["location"].ToString();
                Icon icon = new Icon();
                icon.setId(iconId);
                icon.setLocation(iconLoc);
                iconList.Add(icon);
            }
        }

        protected void genIconConditionTable(object sender, EventArgs e)
        {
            IconConditionPanel.Controls.Clear();
            if (iconList.Count == 0)  // No images set for the condition. Display a simple table stating such.
            {
                IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyle\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("No images are currently set for this connection.<br />Add some using the Add Icons button below.\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</div>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
            }
            else
            {
                foreach (Icon icon in iconList)
                {
                    IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"iconBox\">\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("<img src=\"" + icon.getLocation() + "\" alt=\"\" />\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyle\">\n"));

                    if (icon.getConditions().Count != 0)  // conditions exist; display them.
                    {
                        IconConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("Lower Bound\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("Lower Operator\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("Table\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("Field\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("Upper Operator\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("Upper Bound\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));

                        foreach (Condition condition in icon.getConditions())
                        {
                            IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                            if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                            {
                                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                                IconConditionPanel.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                                IconConditionPanel.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            }
                            else
                            {
                                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">&nbsp;</td><td class=\"tableTD\">&nbsp;</td>\n"));
                            }
                            IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                            IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                            {
                                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                                IconConditionPanel.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                                IconConditionPanel.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            }
                            else
                            {
                                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">&nbsp;</td><td class=\"tableTD\">&nbsp;</td>\n"));
                            }

                            IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                        }
                        IconConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                    }
                    else  // no conditions set. display table stating such.
                    {
                        IconConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        IconConditionPanel.Controls.Add(new LiteralControl("No conditions are currently set.<br />Add some using the button to the right.\n"));
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
                    modifyIconConditionPopupPanel.ID = "modifyIconConditionPopupPanel" + icon.getId();
                    modifyIconConditionPopupPanel.CssClass = "boxPopupStyle";
                    UpdatePanel modifyIconConditionInsidePopupPanel = new UpdatePanel();
                    modifyIconConditionInsidePopupPanel.ID = "modifyIconConditionInsidePopupPanel" + icon.getId().ToString();
                    modifyIconConditionInsidePopupPanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
                    genIconConditionPopup(modifyIconConditionInsidePopupPanel, icon.getId());
                    modifyIconConditionPopupPanel.Controls.Add(modifyIconConditionInsidePopupPanel);

                    Button submitModifyConditionPopup = new Button();
                    submitModifyConditionPopup.ID = "submitModifyCondition" + icon.getId();
                    submitModifyConditionPopup.Text = "Submit";
                    submitModifyConditionPopup.Click += new EventHandler(genIconConditionTable);
                    modifyIconConditionPopupPanel.Controls.Add(submitModifyConditionPopup);
                    /*Button cancelModifyConditionPopup = new Button();
                    cancelModifyConditionPopup.ID = "cancelModifyCondition" + icon.getId();
                    cancelModifyConditionPopup.Text = "Cancel";
                    modifyIconConditionPopupPanel.Controls.Add(cancelModifyConditionPopup);*/

                    if (Request.QueryString.Get("locked") == "false")
                    {
                        AjaxControlToolkit.ModalPopupExtender mpe = new AjaxControlToolkit.ModalPopupExtender();
                        mpe.ID = "MPE_" + icon.getId();
                        mpe.BackgroundCssClass = "modalBackground";
                        mpe.DropShadow = true;
                        mpe.PopupControlID = modifyIconConditionPopupPanel.ID.ToString();
                        mpe.TargetControlID = modifyButton.ID.ToString();
                        //mpe.OkControlID = submitModifyConditionPopup.ID.ToString();
                        //mpe.CancelControlID = cancelModifyConditionPopup.ID.ToString();
                        IconConditionPanel.Controls.Add(mpe);

                        IconConditionPanel.Controls.Add(modifyIconConditionPopupPanel);
                    }
                    IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                    IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                }
            }
            
        }

        protected void genIconConditionPopup(UpdatePanel modifyIconConditionInsidePopupPanel, String args)
        {
            //modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Clear();
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<span class=\"connectionStyle\">&nbsp;Modify Condition</span>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<div class=\"mainBoxP\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table cellspacing=\"0\" cellpadding=\"5\" class=\"mainBox2\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<div class=\"omainBox4\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table class=\"omainBox6\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Add and remove conditions for this icon using the table below.\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</table>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<p>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</p>\n"));

            // icon popup table's header
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Lower Bound\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Lower Operator\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Table\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Field\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Upper Operator\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Upper Bound\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("&nbsp;\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
            int i = 0;
            
            foreach (Icon icon in iconList)
            {
                if (icon.getId() != args)
                    continue;

                foreach (Condition condition in icon.getConditions())
                {
                    i++;
                    modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
                    if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                    {
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                    }
                    else
                    {
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">&nbsp;</td><td class=\"tableTD\">&nbsp;</td>\n"));
                    }
                    modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                    modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                    modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                    modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                    modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                    modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                    if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                    {
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                    }
                    else
                    {
                        modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">&nbsp;</td><td class=\"tableTD\">&nbsp;</td>\n"));
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
            }

            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr class=\"tableTR\">\n"));
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            TextBox addLowerBound = new TextBox();
            addLowerBound.ID = "addIconLowerBound" + args;
            addLowerBound.Width = 50;
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addLowerBound);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addLowerOperator = new DropDownList();
            addLowerOperator.ID = "addIconLowerOperator" + args;
            addLowerOperator.CssClass = "inputDD";
            addLowerOperator.Width = 50;
            addLowerOperator.Items.Add("");
            addLowerOperator.Items.Add("<");
            addLowerOperator.Items.Add("<=");
            addLowerOperator.Items.Add("==");
            addLowerOperator.Items.Add("!=");
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addLowerOperator);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addTableName = new DropDownList();
            addTableName.ID = "addIconTable" + args;
            addTableName.CssClass = "inputDD";
            addTableName.Width = 50;
            addTableName.AutoPostBack = true;
            ConnInfo connInfo = ConnInfo.getConnInfo(Convert.ToInt32(Request.QueryString.Get("ConnID")));
            if (connInfo.getDatabaseType() == ConnInfo.MSSQL)
            {
                addTableName.DataSource = MSQLTables;
                addTableName.DataTextField = "TABLE_NAME";
                addTableName.DataValueField = "TABLE_NAME";
                addTableName.DataBind();
            }
            else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)
            {
                addTableName.DataSource = SQLTables;
                addTableName.DataTextField = "TABLE_NAME";
                addTableName.DataValueField = "TABLE_NAME";
                addTableName.DataBind();
            }
            else if (connInfo.getDatabaseType() == ConnInfo.ORACLE)
            {
                addTableName.DataSource = oracleTables;
                addTableName.DataTextField = "TABLE_NAME";
                addTableName.DataValueField = "TABLE_NAME";
                addTableName.DataBind();
            }
            addTableName.Items.Insert(0, "");
            addTableName.SelectedIndexChanged += new EventHandler(addTableName_SelectedIndexChanged);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addTableName);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addFieldName = new DropDownList();
            addFieldName.ID = "addIconField" + args;
            addFieldName.CssClass = "inputDD";
            addFieldName.Width = 50;
            addFieldName.AutoPostBack = true;
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addFieldName);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addUpperOperator = new DropDownList();
            addUpperOperator.ID = "addIconUpperOperator" + args;
            addUpperOperator.CssClass = "inputDD";
            addUpperOperator.Width = 50;
            addUpperOperator.Items.Add("");
            addUpperOperator.Items.Add("<");
            addUpperOperator.Items.Add("<=");
            addUpperOperator.Items.Add("==");
            addUpperOperator.Items.Add("!=");
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addUpperOperator);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            TextBox addUpperBound = new TextBox();
            addUpperBound.ID = "addIconUpperBound" + args;
            addUpperBound.MaxLength = 30;
            addUpperBound.CssClass = "inputBox";
            addUpperBound.Width = 50;
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addUpperBound);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"textCenter\">\n"));
            Button addConditionButton = new Button();
            addConditionButton.ID = "addIconConditionButton" + args;
            addConditionButton.Text = "Add";
            addConditionButton.ToolTip = "Add Condition";
            addConditionButton.Width = 80;
            addConditionButton.CssClass = "button";
            addConditionButton.Click += new EventHandler(addConditionToIcon);
            addConditionButton.CommandArgument = args;
            //addConditionButton.CommandArgument = addLowerBound.Text.ToString() + "|" + addLowerOperator.SelectedItem.Text.ToString() + "|" + addTableName.Text.ToString() + "|" + addFieldName.Text.ToString() + "|" + addUpperOperator.SelectedItem.Text.ToString() + "|" + addUpperBound.Text.ToString() + "|" + args;
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
                foreach (Icon icon in iconList)
                {
                    if (icon.getId() == iconId)
                    {
                        icon.removeConditions(conditionId);
                    }
                }
            }
            genIconConditionTable(sender, e);
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
                genIconConditionTable(sender, e);
                genOverlayConditionTable(sender, e);
                ErrorHandler eh = new ErrorHandler(conditionErrors, errorPanel1);
                eh.displayError();
                return;
            }

            foreach (Icon icon in iconList)
            {
                if (icon.getId() == iconId)
                {
                    icon.setConditions(condition);
                }
            }
            genIconConditionTable(sender, e);
        }

        protected void genOverlayConditionTable(object sender, EventArgs e)
        {
            OverlayConditionPanel.Controls.Clear();
            if (iconList.Count == 0)  // No images set for the condition. Display a simple table stating such.
            {
                OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyle\">\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("No overlays are currently set for this connection.<br />Add some using the Add Icons button below.\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</div>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
            }
            else
            {
                foreach (Overlay overlay in overlayList)
                {
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"iconBox\">\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<div class=\"overlayBox\" style=\"background-color: #" + overlay.getColor() + ";\" />\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyle\">\n"));

                    if (overlay.getConditions().Count != 0)  // conditions exist; display them.
                    {
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Lower Bound\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Lower Operator\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Table\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Field\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Upper Operator\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("Upper Bound\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));

                        foreach (Condition condition in overlay.getConditions())
                        {
                            OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                            if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                            {
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            }
                            else
                            {
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">&nbsp;</td><td class=\"tableTD\">&nbsp;</td>\n"));
                            }
                            OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                            OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                            OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                            OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                            OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                            {
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                            }
                            else
                            {
                                OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">&nbsp;</td><td class=\"tableTD\">&nbsp;</td>\n"));
                            }

                            OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                        }
                        OverlayConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                    }
                    else  // no conditions set. display table stating such.
                    {
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        OverlayConditionPanel.Controls.Add(new LiteralControl("No conditions are currently set.<br />Add some using the button to the right.\n"));
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
                    modifyOverlayConditionPopupPanel.ID = "modifyOverlayConditionPopupPanel" + overlay.getId();
                    modifyOverlayConditionPopupPanel.CssClass = "boxPopupStyle";
                    UpdatePanel modifyOverlayConditionInsidePopupPanel = new UpdatePanel();
                    modifyOverlayConditionInsidePopupPanel.ID = "modifyOverlayConditionInsidePopupPanel" + overlay.getId();
                    modifyOverlayConditionInsidePopupPanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
                    genOverlayConditionPopup(modifyOverlayConditionInsidePopupPanel, overlay.getId());
                    modifyOverlayConditionPopupPanel.Controls.Add(modifyOverlayConditionInsidePopupPanel);

                    Button submitModifyConditionPopup = new Button();
                    submitModifyConditionPopup.ID = "submitOverlayModifyCondition" + overlay.getId();
                    submitModifyConditionPopup.Text = "Submit";
                    submitModifyConditionPopup.Click += new EventHandler(genIconConditionTable);
                    modifyOverlayConditionPopupPanel.Controls.Add(submitModifyConditionPopup);
                    /*Button cancelModifyConditionPopup = new Button();
                    cancelModifyConditionPopup.ID = "cancelOverlayModifyCondition" + overlay.getId();
                    cancelModifyConditionPopup.Text = "Cancel";
                    modifyOverlayConditionPopupPanel.Controls.Add(cancelModifyConditionPopup);*/

                    if (Request.QueryString.Get("locked") == "false")
                    {
                        AjaxControlToolkit.ModalPopupExtender mpe = new AjaxControlToolkit.ModalPopupExtender();
                        mpe.ID = "MPE_OVERLAY_" + overlay.getId();
                        mpe.BackgroundCssClass = "modalBackground";
                        mpe.DropShadow = true;
                        mpe.PopupControlID = modifyOverlayConditionPopupPanel.ID.ToString();
                        mpe.TargetControlID = modifyButton.ID.ToString();
                        //mpe.OkControlID = submitModifyConditionPopup.ID.ToString();
                        //mpe.CancelControlID = cancelModifyConditionPopup.ID.ToString();
                        OverlayConditionPanel.Controls.Add(mpe);

                        OverlayConditionPanel.Controls.Add(modifyOverlayConditionPopupPanel);
                    }
                    OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                    OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
                }
            }

        }

        protected void genOverlayConditionPopup(UpdatePanel modifyOverlayConditionInsidePopupPanel, String args)
        {
            //modifyOverlayConditionInsidePopupPanel.Controls.Clear();
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<span class=\"connectionStyle\">&nbsp;Modify Condition</span>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<div class=\"mainBoxP\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table cellspacing=\"0\" cellpadding=\"5\" class=\"mainBox2\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<div class=\"omainBox4\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table class=\"omainBox6\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Add and remove conditions for this overlay using the table below.\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</table>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<p>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</p>\n"));

            // overlay popup table's header
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Lower Bound\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Lower Operator\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Table\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Field\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Upper Operator\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("Upper Bound\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("&nbsp;\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</tr>\n"));
            int i = 0;

            foreach (Overlay overlay in overlayList)
            {
                if (overlay.getId() != args)
                    continue;

                foreach (Condition condition in overlay.getConditions())
                {
                    i++;
                    modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr>\n"));
                    if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                    {
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                    }
                    else
                    {
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">&nbsp;</td><td class=\"tableTD\">&nbsp;</td>\n"));
                    }
                    modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                    modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                    modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                    modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                    modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                    modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                    if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                    {
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));
                    }
                    else
                    {
                        modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">&nbsp;</td><td class=\"tableTD\">&nbsp;</td>\n"));
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
            }

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<tr class=\"tableTR\">\n"));
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            TextBox addLowerBound = new TextBox();
            addLowerBound.ID = "addOverlayLowerBound" + args;
            addLowerBound.Width = 50;
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addLowerBound);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addLowerOperator = new DropDownList();
            addLowerOperator.ID = "addOverlayLowerOperator" + args;
            addLowerOperator.CssClass = "inputDD";
            addLowerOperator.Width = 50;
            addLowerOperator.Items.Add("");
            addLowerOperator.Items.Add("<");
            addLowerOperator.Items.Add("<=");
            addLowerOperator.Items.Add("==");
            addLowerOperator.Items.Add("!=");
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addLowerOperator);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addTableName = new DropDownList();
            addTableName.ID = "addOverlayTable" + args;
            addTableName.CssClass = "inputDD";
            addTableName.Width = 50;
            addTableName.AutoPostBack = true;
            ConnInfo connInfo = ConnInfo.getConnInfo(Convert.ToInt32(Request.QueryString.Get("ConnID")));
            if (connInfo.getDatabaseType() == ConnInfo.MSSQL)
            {
                addTableName.DataSource = MSQLTables;
                addTableName.DataTextField = "TABLE_NAME";
                addTableName.DataValueField = "TABLE_NAME";
                addTableName.DataBind();
            }
            else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)
            {
                addTableName.DataSource = SQLTables;
                addTableName.DataTextField = "TABLE_NAME";
                addTableName.DataValueField = "TABLE_NAME";
                addTableName.DataBind();
            }
            else if (connInfo.getDatabaseType() == ConnInfo.ORACLE)
            {
                addTableName.DataSource = oracleTables;
                addTableName.DataTextField = "TABLE_NAME";
                addTableName.DataValueField = "TABLE_NAME";
                addTableName.DataBind();
            }
            addTableName.Items.Insert(0, "");
            addTableName.SelectedIndexChanged += new EventHandler(addTableName_SelectedIndexChanged);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addTableName);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addFieldName = new DropDownList();
            addFieldName.ID = "addOverlayField" + args;
            addFieldName.CssClass = "inputDD";
            addFieldName.Width = 50;
            addFieldName.AutoPostBack = true;
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addFieldName);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addUpperOperator = new DropDownList();
            addUpperOperator.ID = "addOverlayUpperOperator" + args;
            addUpperOperator.CssClass = "inputDD";
            addUpperOperator.Width = 50;
            addUpperOperator.Items.Add("");
            addUpperOperator.Items.Add("<");
            addUpperOperator.Items.Add("<=");
            addUpperOperator.Items.Add("==");
            addUpperOperator.Items.Add("!=");
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addUpperOperator);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            TextBox addUpperBound = new TextBox();
            addUpperBound.ID = "addOverlayUpperBound" + args;
            addUpperBound.MaxLength = 30;
            addUpperBound.CssClass = "inputBox";
            addUpperBound.Width = 50;
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addUpperBound);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"textCenter\">\n"));
            Button addConditionButton = new Button();
            addConditionButton.ID = "addOverlayConditionButton" + args;
            addConditionButton.Text = "Add";
            addConditionButton.ToolTip = "Add Condition";
            addConditionButton.Width = 80;
            addConditionButton.CssClass = "button";
            addConditionButton.Click += new EventHandler(addConditionToOverlay);
            addConditionButton.CommandArgument = args;
            //addConditionButton.CommandArgument = addLowerBound.Text.ToString() + "|" + addLowerOperator.SelectedItem.Text.ToString() + "|" + addTableName.Text.ToString() + "|" + addFieldName.Text.ToString() + "|" + addUpperOperator.SelectedItem.Text.ToString() + "|" + addUpperBound.Text.ToString() + "|" + args;
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
                foreach (Overlay overlay in overlayList)
                {
                    if (overlay.getId() == overlayId)
                    {
                        overlay.removeConditions(conditionId);
                    }
                }
            }
            genOverlayConditionTable(sender, e);
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
                genIconConditionTable(sender, e);
                genOverlayConditionTable(sender, e);
                ErrorHandler eh = new ErrorHandler(conditionErrors, errorPanel1);
                eh.displayError();
                return;
            }

            foreach (Overlay overlay in overlayList)
            {
                if (overlay.getId() == overlayId)
                {
                    overlay.setConditions(condition);
                }
            }
            genOverlayConditionTable(sender, e);
        }

        protected void addTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            Database db = new Database();
            DataTable dt;
            try
            {
                dt = db.executeQueryLocal("SELECT * FROM Connection WHERE ID=" + Request.QueryString.Get("ConnID"));
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            string connectionString = "";
            string providerName = "";
            int type = 0;
            string address = "";
            string dbName = "";
            string username = "";
            string password = "";
            foreach (DataRow dr in dt.Rows)  // it should just be the one row, but I don't know how to just grab 1 row.
            {
                type = Convert.ToInt32(dr["type"]);
                address = dr["address"].ToString();
                dbName = dr["dbName"].ToString();
                username = dr["userName"].ToString();
                password = dr["password"].ToString();
            }

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

            if (type == ConnInfo.MSSQL)
            {
                connectionString = "Data Source=" + address + ";Initial Catalog=" + dbName + ";Persist Security Info=True;User Id=" + username + ";Password=" + password;
                SqlDataSource temp = new SqlDataSource();
                temp.ConnectionString = connectionString;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                fieldList.DataSource = temp;
                fieldList.DataValueField = "COLUMN_NAME";
                fieldList.DataTextField = "COLUMN_NAME";
                fieldList.DataBind();
            }
            else if (type == ConnInfo.MYSQL)
            {
                connectionString = "server=" + address + ";User Id=" + username + ";password=" + password + ";Persist Security Info=True;database=" + dbName;
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
            else if (type == ConnInfo.ORACLE)
            {
                connectionString = "Data Source=" + address + ";Persist Security Info=True;User ID=" + username + ";Password=" + password + ";Unicode=True";
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
                //Default case goes here
            }
            string id = fieldList.ID.Substring(fieldList.ID.LastIndexOf("d") + 1);  /* grabs iconid / overlayid from ID of passed in dropdownlist. */                                                                                                                        goto here; here:                            
            UpdatePanel up;
            if (tableList.ID.ToString().LastIndexOf("addIconTable") != -1)
                up = (UpdatePanel)Page.FindControl("modifyIconConditionInsidePopupPanel" + id);
            else
                up = (UpdatePanel)Page.FindControl("modifyOverlayConditionInsidePopupPanel" + id);
            
            //up.Update();
        }

        protected ArrayList getColumnsForTable(ConnInfo cInfo, string tableName)
        {
            ArrayList result = new ArrayList();



            return result;
        }

        /// <summary>
        /// used for uploading icons from local computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmitClick(object sender, EventArgs e)
        {
            Boolean valid = false;
            //checks to make sure there is an uploaded file
            if (fileUpEx.HasFile)
            {
                //checks for valid filetype
                foreach (String type in validTypes)
                {
                    if (fileUpEx.PostedFile.ContentType.Equals(type))
                    {
                        valid = true;
                    }
                }
                //checks for valid dimensions
                if (valid && ValidateFileDimensions(fileUpEx.PostedFile.InputStream))
                {
                    String filepath = fileUpEx.PostedFile.FileName;
                    String file_ext = System.IO.Path.GetExtension(filepath);
                    String filename = System.IO.Path.GetFileNameWithoutExtension(filepath);
                    String suffix = GetRandomString();
                    String file = filename + suffix + file_ext;

                    //save the file to the server
                    fileUpEx.PostedFile.SaveAs(fileSaveLoc + file);
                    //errorPanel1.Text = "File Saved to: " + fileSaveLoc + file;
                }
                else if (!valid)
                {
                    String errorText = "Current File type = " + fileUpEx.PostedFile.ContentType + " File type not appropriate (only jpg, gif, tiff, png, bmp accepted)";
                    ErrorHandler eh = new ErrorHandler(errorText, errorPanel1);
                    eh.displayError();
                    return;
                }
                else
                {
                    ErrorHandler eh = new ErrorHandler("File dimensions to large (max 128 x 128)", errorPanel1);
                    eh.displayError();
                    return;
                }
            }
            fileUpEx = new FileUpload();
        }
        //protected void URLsubmitClick(object sender, EventArgs e)
        //{
        //    String URL = URLtextBox.Text.Trim();
        //    URLpanel.Visible = true;
        //    URLsubmitLabel.Text = "You entered - " + "<a href=\"" + URL + "\" target=NEW>" + URL + "</a>";
        //    //DB.executeQueryLocal("");
        //}
        /// <summary>
        /// used for uploadinc icons from remote sources
        /// if fetch is checked this function downloads the linked icon and saves its info to the db and saves the icon
        /// if fetch is not checked it just saves the linked icon's info to the db
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void URLsubmitClick(object sender, EventArgs e)
        {
            String URL = URLtextBox.Text.Trim();
            Database DB = new Database();
            WebClient Client = new WebClient();
            //below lines get information to check validity of icon and saves the icon temporarily
            String fileName = System.IO.Path.GetFileNameWithoutExtension(URL);
            String ext = System.IO.Path.GetExtension(URL);
            String suffix = GetRandomString();
            String tempName = tempSaveLoc + fileName + suffix + ext;
            Client.DownloadFile(URL, tempName);
            bool valid = false;
            //checks to see if fileType of icon is valid
            foreach (String type in validTypes)
            {
                String localFileType = GetContentType(tempName);
                if (localFileType.Equals(type))
                {
                    valid = true;
                    break;
                }
            }
            FileStream fs = File.OpenRead(tempName);
            if (fetchCheckBox.Checked)
            {
                String Name = fileSaveLoc + fileName + suffix + ext;
                //checks if icon has valid dimensions
                if (valid && ValidateFileDimensions(fs))
                {
                    fs.Close();
                    File.Move(tempName, Name);
                    File.Delete(tempName);
                    DB.executeQueryLocal("INSERT INTO IconLibrary (location, isLocal) VALUES (\'" + Name + "\', 1)");
                }
                else if (!valid)
                {
                    fs.Close();
                    File.Delete(tempName);
                    ErrorHandler eh = new ErrorHandler("URL contains to many TitlesYou linked to an invalid file type", errorPanel1);
                    eh.displayError();
                    return;
                }
                else
                {
                    //ModalPopupExtender3.Hide();
                    fs.Close();
                    File.Delete(tempName);
                    ErrorHandler eh = new ErrorHandler("The file you linked to was to large (max 128 x 128)", errorPanel1);
                    eh.displayError();
                    return;
                }
            }
            else
            {
                //checks if icon has valid dimensions
                if (valid && ValidateFileDimensions(fs))
                {
                    fs.Close();
                    File.Delete(tempName);
                    DB.executeQueryLocal("INSERT INTO IconLibrary (location, isLocal) VALUES (\'" + URL + "\', 0)");
                }
                else if (!valid)
                {
                    fs.Close();
                    File.Delete(tempName);
                    ErrorHandler eh = new ErrorHandler("You linked to an invalid file type", errorPanel1);
                    eh.displayError();
                    return;
                }
                else
                {
                    fs.Close();
                    File.Delete(tempName);
                    ErrorHandler eh = new ErrorHandler("The file you linked to was to large (max 128 x 128)", errorPanel1);
                    eh.displayError();
                    return;
                }
            }
            fetchCheckBox.Checked = false;
            URLtextBox.Text = "";
        }
        /// <summary>
        /// helper function used by saving icons without name overlap
        /// </summary>
        /// <returns></returns>
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }
        /// <summary>
        /// type list used to check for valid file types of icons
        /// </summary>
        public void BuildTypeList()
        {
            this.validTypes.Add("image/bmp");
            this.validTypes.Add("image/gif");
            this.validTypes.Add("image/jpeg");
            this.validTypes.Add("image/pjpeg");
            this.validTypes.Add("image/png");
            this.validTypes.Add("image/tiff");
            this.validTypes.Add("image/x-tiff");
            this.validTypes.Add("image/x-windows-bmp");
        }
        /// <summary>
        /// helper function to check dimensions of uploaded icons
        /// </summary>
        /// <param name="input"></param>
        /// <returns>bool, true if valid dimensions</returns>
        private bool ValidateFileDimensions(Stream input)
        {
            using (System.Drawing.Image myImage =
              System.Drawing.Image.FromStream(input))
            {
                return (myImage.Height <= height && myImage.Width <= width);
            }
        }
        /// <summary>
        /// helper function to get type of uploaded icons
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>contentType</returns>
        private string GetContentType(string fileName)
        {
            string contentType = "application/octetstream";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (registryKey != null && registryKey.GetValue("Content Type") != null)
                contentType = registryKey.GetValue("Content Type").ToString();
            return contentType;
        }
        protected void modifyConnection(object sender, EventArgs e)
        {
            saveIconList();
        }
        private void saveIconList()
        {
            //int connID = Convert.ToInt32(Request.QueryString.Get("ConnID"));
            int connID = int.Parse(Request.QueryString.Get("ConnID"));
            Database DB = new Database();
            DataTable iconTable = new DataTable();
            try
            {
                iconTable = DB.executeQueryLocal("SELECT * FROM Icon WHERE connID=" + connID);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
            ArrayList addedIcons = new ArrayList();
            ArrayList deletedIcons = new ArrayList();
            ArrayList modifiedIcons = new ArrayList();
            foreach (DataRow row in iconTable.Rows)
            {
                bool deleted = true;
                foreach (Icon icon in iconList)
                {
                    int iconID = Convert.ToInt32(icon.getId());
                    if(iconID == (int)row[0])
                    {
                        deleted = false;
                        modifiedIcons.Add(icon);
                    }
                }
                if(deleted)
                {
                    deletedIcons.Add((int)row[0]);
                }
            }
            foreach (Icon icon in iconList)
            {
                bool newIcon = true;
                foreach (DataRow row in iconTable.Rows)
                {
                    int iconID = Convert.ToInt32(icon.getId());
                    if(iconID == (int)row[0])
                    {
                        newIcon = false;
                    }
                }
                if(newIcon)
                {
                    addedIcons.Add(icon);
                }
            }
            foreach (Icon icon in addedIcons)
            {
                int iconID = Convert.ToInt32(icon.getId());
                //int iconID = int.Parse(icon.getId());
                try
                {
                    DB.executeQueryLocal("INSERT INTO Icon (ID, connID) VALUES (" + iconID + ", " + connID + ")");
                }
                catch (ODBC2KMLException ex)
                {
                    ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                    eh.displayError();
                    return;
                }
                ArrayList condArray = icon.getConditions();
                foreach (Condition condition in condArray)
                {
                    int ID, lowerOperator, upperOperator = 0;
                    String lowerBound, upperBound, fieldName, tableName = "";
                    ID = Convert.ToInt32(condition.getId());
                    if ( ID != 0)
                    {
                        ErrorHandler eh = new ErrorHandler("Error saving condition (already used)", errorPanel1);
                        eh.displayError();
                        return; 
                    }
                    //ID = int.Parse(condition.getId());
                    //lowerOperator = Convert.ToInt32(condition.getLowerOperator());
                    //upperOperator = Convert.ToInt32(condition.getUpperOperator());
                    //lowerOperator = int.Parse(condition.getLowerOperator());
                    //upperOperator = int.Parse(condition.getUpperOperator());
                    lowerOperator = Condition.operatorStringToInt(condition.getLowerOperator());
                    upperOperator = Condition.operatorStringToInt(condition.getUpperOperator());
                    lowerBound = condition.getLowerBound();
                    upperBound = condition.getUpperBound();
                    fieldName = condition.getFieldName();
                    tableName = condition.getTableName();
                    try
                    {
                        DB.executeQueryLocal("INSERT INTO IconCondition (connID, iconID, lowerBound, upperBound, lowerOperator, upperOperator, fieldName, tableName) VALUES ("
                            + connID + ", "
                            + iconID + ", \'" + lowerBound + "\', \'"
                            + upperBound + "\', \'" + lowerOperator + "\', \'"
                            + upperOperator + "\', \'" + fieldName + "\', \'"
                            + tableName + "\')");
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }
                }
            }
            foreach (Icon icon in modifiedIcons)
            {
                int iconID = Convert.ToInt32(icon.getId());
                DataTable conditions = new DataTable();
                try
                {
                    conditions = DB.executeQueryLocal("SELECT * FROM IconCondition WHERE connID="
                               + connID + " and iconID=" + iconID);
                }
                catch (ODBC2KMLException ex)
                {
                    ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                    eh.displayError();
                    return;
                }
                //int iconID = Convert.ToInt32(icon.getId());
                //int iconID = int.Parse(icon.getId());
                ArrayList condArray = icon.getConditions();
                ArrayList deletedCond = new ArrayList();
                foreach (DataRow row in conditions.Rows)
                {
                    bool deleted = true;
                    foreach (Condition condition in condArray)
                    {
                        try
                        {
                            condition.setIDfromDB(connID, iconID);
                        }
                        catch (ODBC2KMLException ex)
                        {
                            ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                            eh.displayError();
                            return;
                        }
                        if ((int)row[0] == Convert.ToInt32(condition.getId()))
                        {
                            deleted = false;
                        }
                    }
                    if(deleted)
                    {
                        deletedCond.Add((int)row[0]);
                    }
                }
                ArrayList newArray = new ArrayList();
                foreach (Condition condition in condArray)
                {
                    //try
                    //{
                    //    condition.setIDfromDB(connID, iconID);
                    //}
                    //catch (ODBC2KMLException ex)
                    //{
                    //    ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                    //    eh.displayError();
                    //    return;
                    //}
                    bool alreadyExists = false;
                    foreach (DataRow row in conditions.Rows)
                    {
                        if ((int)row[0] == Convert.ToInt32(condition.getId()))
                        {
                            alreadyExists = true;
                        }
                    }
                    if (!alreadyExists)
                    {
                        newArray.Add(condition);
                    }
                }
                foreach (Condition condition in newArray)
                {
                    //condition.setIDfromDB(connID, iconID);
                    int lowerOperator, upperOperator = 0;
                    String lowerBound, upperBound, fieldName, tableName = "";
                    //ID = Convert.ToInt32(condition.getId());
                    //ID = int.Parse(condition.getId());
                    lowerOperator = Condition.operatorStringToInt(condition.getLowerOperator());
                    upperOperator = Condition.operatorStringToInt(condition.getUpperOperator());
                    //lowerOperator = int.Parse(condition.getLowerOperator());
                    //upperOperator = int.Parse(condition.getUpperOperator());
                    lowerBound = condition.getLowerBound();
                    upperBound = condition.getUpperBound();
                    fieldName = condition.getFieldName();
                    tableName = condition.getTableName();
                    try
                    {
                        //DB.executeQueryLocal("UPDATE IconCondition SET lowerBound=\'" + lowerBound
                        //    + "\', upperBound=\'" + upperBound
                        //    + "\', lowerOperator=" + lowerOperator
                        //    + ", upperOperator=" + upperOperator
                        //    + ", fieldName=\'" + fieldName
                        //    + "\', tableName=\'" + tableName + "\' "
                        //    + "WHERE ID=" + ID + " and connID=" + connID + " and iconID=" + iconID);
                        DB.executeQueryLocal("INSERT INTO IconCondition (connID, iconID, lowerBound, upperBound, lowerOperator, upperOperator, fieldName, tableName) VALUES ("
                            + connID + ", "
                            + iconID + ", \'" + lowerBound + "\', \'"
                            + upperBound + "\', \'" + lowerOperator + "\', \'"
                            + upperOperator + "\', \'" + fieldName + "\', \'"
                            + tableName + "\')");
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }
                }
                foreach (int int1 in deletedCond)
                {
                    try
                    {
                        DB.executeQueryLocal("DELETE FROM IconCondition WHERE ID=" + int1);
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }
                }
            }
            foreach (int iconID in deletedIcons)
            {
                try
                {
                    DB.executeQueryLocal("DELETE FROM Icon WHERE ID=" + iconID + " and connID=" + connID);
                }
                catch (ODBC2KMLException ex)
                {
                    ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                    eh.displayError();
                    return;
                }
            }
        }

        //private bool fetch;
        //private String URL;
        //private Database DB;
        public const int height = 128;
        public const int width = 128;
        private static int curOverlayCount = -1;
        public static String tempSaveLoc = @"C:\odbc2kml\temp\";
        public static String fileSaveLoc = @"C:\odbc2kml\uploads\";
        public ArrayList validTypes = new ArrayList();
        private static bool alreadySetupLists = false;
        private static ArrayList iconList = new ArrayList();
        private static ArrayList iconListAvailableToAdd = new ArrayList();
        private static ArrayList iconListAvailableToRemove = new ArrayList();
        private static ArrayList overlayListAvailableToRemove = new ArrayList();
        private static ArrayList overlayList = new ArrayList();
        private static ArrayList tableNameList = new ArrayList();
        private static SqlDataSource MSQLTables = new SqlDataSource();
        private static SqlDataSource SQLTables = new SqlDataSource();
        private static SqlDataSource oracleTables = new SqlDataSource();
        private static SqlDataSource colDataSource = new SqlDataSource();
        private static int tempId = -1;


        //editor methods
        protected void updateTables(int type)
        {
            if (type == ConnInfo.MSSQL)
            {
                iTableNBox.DataSource = MSQLTables;
                iTableNBox.DataTextField = "TABLE_NAME";
                iTableNBox.DataValueField = "TABLE_NAME";
                iTableNBox.DataBind();
                iTableFNBox.DataSource = MSQLTables;
                iTableFNBox.DataTextField = "TABLE_NAME";
                iTableFNBox.DataValueField = "TABLE_NAME";
                iTableFNBox.DataBind();
                iTableINBox.DataSource = MSQLTables;
                iTableINBox.DataTextField = "TABLE_NAME";
                iTableINBox.DataValueField = "TABLE_NAME";
                iTableINBox.DataBind();
                GridViewTables.DataSource = MSQLTables;
                GridViewTables.DataBind();
            }
            else if (type == ConnInfo.MYSQL)
            {
                iTableNBox.DataSource = SQLTables;
                iTableNBox.DataTextField = "TABLE_NAME";
                iTableNBox.DataValueField = "TABLE_NAME";
                iTableNBox.DataBind();
                iTableFNBox.DataSource = SQLTables;
                iTableFNBox.DataTextField = "TABLE_NAME";
                iTableFNBox.DataValueField = "TABLE_NAME";
                iTableFNBox.DataBind();
                iTableINBox.DataSource = SQLTables;
                iTableINBox.DataTextField = "TABLE_NAME";
                iTableINBox.DataValueField = "TABLE_NAME";
                iTableINBox.DataBind();
                GridViewTables.DataSource = SQLTables;
                GridViewTables.DataBind();
            }
            else if (type == ConnInfo.ORACLE)
            {
                iTableNBox.DataSource = oracleTables;
                iTableNBox.DataTextField = "TABLE_NAME";
                iTableNBox.DataValueField = "TABLE_NAME";
                iTableNBox.DataBind();
                iTableFNBox.DataSource = oracleTables;
                iTableFNBox.DataTextField = "TABLE_NAME";
                iTableFNBox.DataValueField = "TABLE_NAME";
                iTableFNBox.DataBind();
                iTableINBox.DataSource = oracleTables;
                iTableINBox.DataTextField = "TABLE_NAME";
                iTableINBox.DataValueField = "TABLE_NAME";
                iTableINBox.DataBind();
                GridViewTables.DataSource = oracleTables;
                GridViewTables.DataBind();
            }
            else
            {
                //Default case goes here
            }
        }

        protected void dLink_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = true;
            dTablePanel.Visible = false;
            dFieldPanel.Visible = false;
            dImagePanel.Visible = false;
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
        }

        protected void dTable_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = false;
            dTablePanel.Visible = true;
            dFieldPanel.Visible = false;
            dImagePanel.Visible = false;
        }

        protected void dTableInsert_Click(object sender, EventArgs e)
        {
            string tableText = iTableNBox.SelectedValue.ToString();
            string descriptionInfo = "[TABLE]" + tableText + "[/TABLE]";

            descriptionBox.Text += descriptionInfo;

        }

        protected void dField_Click(object sender, EventArgs e)
        {

            dLinkPanel.Visible = false;
            dTablePanel.Visible = false;
            dFieldPanel.Visible = true;
            dImagePanel.Visible = false;

        }

        protected void dFieldInsert_Click(object sender, EventArgs e)
        {
            string tableText = iTableFNBox.SelectedValue.ToString();
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
        }

        protected void dImage_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = false;
            dTablePanel.Visible = false;
            dFieldPanel.Visible = false;
            dImagePanel.Visible = true;
        }

        protected void dImageInsert_Click(object sender, EventArgs e)
        {
            string tableText = iTableINBox.SelectedValue.ToString();
            string colText = iColINBox.SelectedValue.ToString();

            string descriptionInfo = "[IMG][TBL]" + tableText + "[/TBL][COL]" + colText + "[/COL][/IMG]";


            if (tableText.Equals("") || colText.Equals(""))
            {
                dImageError.Visible = true;
            }
            else
            {
                dImageError.Visible = false;
                descriptionBox.Text += descriptionInfo;
            }
        }

        protected void iTableFNBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = iTableFNBox.SelectedValue.ToString();
            if (selectedTable != "")
            {
                int conID = Convert.ToInt32(Request.QueryString.Get("ConnID"));
                ConnInfo connInfo_editor = ConnInfo.getConnInfo(conID);

                string connectionString_editor = "";
                string providerName_editor = "";

                //Set drop down box accordingly
                if (connInfo_editor.getDatabaseType() == ConnInfo.MSSQL)
                {
                    connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Initial Catalog=" + connInfo_editor.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword();
                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString_editor;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                    iColFNBox.DataSource = temp;
                    iColFNBox.DataValueField = "COLUMN_NAME";
                    iColFNBox.DataTextField = "COLUMN_NAME";
                    iColFNBox.DataBind();
                    UpdateFieldCol.Update();
                }

                else if (connInfo_editor.getDatabaseType() == ConnInfo.MYSQL)
                {
                    connectionString_editor = "server=" + connInfo_editor.getServerAddress() + ";User Id=" + connInfo_editor.getUserName() + ";password=" + connInfo_editor.getPassword() + ";Persist Security Info=True;database=" + connInfo_editor.getDatabaseName();
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

                else if (connInfo_editor.getDatabaseType() == ConnInfo.ORACLE)
                {
                    connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword() + ";Unicode=True";
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

                else //Default set to SQL
                {
                }

                //Garbage collection
                connInfo_editor = null;
            }
            else
            {
                iColFNBox.Items.Clear();
                UpdateFieldCol.Update();
            }
        }

        protected void iTableINBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = iTableINBox.SelectedValue.ToString();
            if (selectedTable != "")
            {
                int conID = Convert.ToInt32(Request.QueryString.Get("ConnID"));
                ConnInfo connInfo_editor = ConnInfo.getConnInfo(conID);

                string connectionString_editor = "";
                string providerName_editor = "";

                //Set drop down box accordingly
                if (connInfo_editor.getDatabaseType() == ConnInfo.MSSQL)
                {
                    connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Initial Catalog=" + connInfo_editor.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword();
                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString_editor;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                    iColINBox.DataSource = temp;
                    iColINBox.DataValueField = "COLUMN_NAME";
                    iColINBox.DataTextField = "COLUMN_NAME";
                    iColINBox.DataBind();
                    UpdateFieldCol.Update();
                }

                else if (connInfo_editor.getDatabaseType() == ConnInfo.MYSQL)
                {
                    connectionString_editor = "server=" + connInfo_editor.getServerAddress() + ";User Id=" + connInfo_editor.getUserName() + ";password=" + connInfo_editor.getPassword() + ";Persist Security Info=True;database=" + connInfo_editor.getDatabaseName();
                    providerName_editor = "MySql.Data.MySqlClient";

                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString_editor;
                    temp.ProviderName = providerName_editor;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                    iColINBox.DataSource = temp;
                    iColINBox.DataValueField = "COLUMN_NAME";
                    iColINBox.DataTextField = "COLUMN_NAME";
                    iColINBox.DataBind();
                    UpdateFieldCol.Update();

                }

                else if (connInfo_editor.getDatabaseType() == ConnInfo.ORACLE)
                {
                    connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword() + ";Unicode=True";
                    providerName_editor = "System.Data.OracleClient";

                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString_editor;
                    temp.ProviderName = providerName_editor;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedTable + "')";
                    iColINBox.DataSource = temp;
                    iColINBox.DataValueField = "COLUMN_NAME";
                    iColINBox.DataTextField = "COLUMN_NAME";
                    iColINBox.DataBind();
                    UpdateFieldCol.Update();
                }

                else //Default set to SQL
                {
                }

                //Garbage collection
                connInfo_editor = null;
            }
            else
            {
                iColINBox.Items.Clear();
                UpdateFieldCol.Update();
            }
        }

        protected void GridViewTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            int conID = Convert.ToInt32(Request.QueryString.Get("ConnID"));
            columnButtons.Visible = true;
            columnMessage.Visible = false;
            string selectedTable = GridViewTables.SelectedValue.ToString();
            selectedGVTable.Value = selectedTable;

            string pageInfo = "viewTable.aspx?con=" + conID + "&tbl=" + selectedTable;
            string window = "window.open('" + pageInfo + "'); return false;";
            viewTable.Attributes.Add("onclick", window);

            if (selectedTable != "")
            {
                ConnInfo connInfo_editor = ConnInfo.getConnInfo(conID);

                string connectionString_editor = "";
                string providerName_editor = "";

                //Set drop down box accordingly
                if (connInfo_editor.getDatabaseType() == ConnInfo.MSSQL)
                {
                    connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Initial Catalog=" + connInfo_editor.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword();
                    ColGen.ConnectionString = connectionString_editor;
                    ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                    ColGen.DataBind();
                }

                else if (connInfo_editor.getDatabaseType() == ConnInfo.MYSQL)
                {
                    connectionString_editor = "server=" + connInfo_editor.getServerAddress() + ";User Id=" + connInfo_editor.getUserName() + ";password=" + connInfo_editor.getPassword() + ";Persist Security Info=True;database=" + connInfo_editor.getDatabaseName();
                    providerName_editor = "MySql.Data.MySqlClient";

                    ColGen.ConnectionString = connectionString_editor;
                    ColGen.ProviderName = providerName_editor;
                    ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                    ColGen.DataBind();

                }

                else if (connInfo_editor.getDatabaseType() == ConnInfo.ORACLE)
                {
                    connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword() + ";Unicode=True";
                    providerName_editor = "System.Data.OracleClient";

                    ColGen.ConnectionString = connectionString_editor;
                    ColGen.ProviderName = providerName_editor;
                    ColGen.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedGVTable.Value + "')";
                    ColGen.DataBind();

                }

                else //Default set to SQL
                {
                }


                Mapping conMapping = Mapping.getMapping2(conID, selectedTable);

                string dbTable = conMapping.getTableName();

                int dbFormat = conMapping.getFormat();

                string dbLat = conMapping.getLatFieldName();
                string dbLong = conMapping.getLongFieldName();

                //No information about the table yet
                if (dbTable == null)
                {
                    mapUpdates(ColGen);
                }

                else
                {
                    mapUpdates(ColGen, dbLat, dbLong, dbFormat);
                }

                //Garbage collection
                connInfo_editor = null;
            }
            else
            {
                iColFNBox.Items.Clear();
                UpdateFieldCol.Update();
            }

        }

        protected void GridViewColumns_PageIndexChanged(object sender, EventArgs e)
        {
            int conID = Convert.ToInt32(Request.QueryString.Get("ConnID"));
            ConnInfo connInfo_editor = ConnInfo.getConnInfo(conID);

            string connectionString_editor = "";
            string providerName_editor = "";

            //Set drop down box accordingly
            if (connInfo_editor.getDatabaseType() == ConnInfo.MSSQL)
            {
                connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Initial Catalog=" + connInfo_editor.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword();
                ColGen.ConnectionString = connectionString_editor;
                ColGen.ProviderName = providerName_editor;
                ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                ColGen.DataBind();
            }

            else if (connInfo_editor.getDatabaseType() == ConnInfo.MYSQL)
            {
                connectionString_editor = "server=" + connInfo_editor.getServerAddress() + ";User Id=" + connInfo_editor.getUserName() + ";password=" + connInfo_editor.getPassword() + ";Persist Security Info=True;database=" + connInfo_editor.getDatabaseName();
                providerName_editor = "MySql.Data.MySqlClient";
                ColGen.ConnectionString = connectionString_editor;
                ColGen.ProviderName = providerName_editor;
                ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                ColGen.DataBind();

            }

            else if (connInfo_editor.getDatabaseType() == ConnInfo.ORACLE)
            {
                connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword() + ";Unicode=True";
                providerName_editor = "System.Data.OracleClient";
                ColGen.ConnectionString = connectionString_editor;
                ColGen.ProviderName = providerName_editor;
                ColGen.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedGVTable.Value + "')";
                ColGen.DataBind();
            }

            else //Default set to SQL
            {
            }

            //Garbage collection
            connInfo_editor = null;
        }

        protected void updateDescription(object sender, EventArgs e)
        {
            int conID = Convert.ToInt32(Request.QueryString.Get("ConnID"));
            Description conDesc_editor = Description.getDescription(conID);
            string descBox = conDesc_editor.getDesc();

            string descText = descriptionBox.Text.ToString();

            //No description entry exists
            if (descBox == null)
            {
                Description.insertDescription(conID, descText);
                descSuccess.Text = "Description inserted successfully!";
            }

            //Update existing description entry
            else
            {
                Description.updateDescription(conID, descText);
                descSuccess.Text = "Description updated successfully!";
            }

        }

        protected void mapTogether_Click(object sender, EventArgs e)
        {
            LLSepPanel.Visible = false;
            LLTogetherPanel.Visible = true;
        }

        protected void mapSeparate_Click(object sender, EventArgs e)
        {
            LLTogetherPanel.Visible = false;
            LLSepPanel.Visible = true;
        }

        protected void mapUpdates(SqlDataSource temp)
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

            LLSepPanel.Visible = true;
            LLTogetherPanel.Visible = false;

            mapError1.Visible = false;
            mapError2.Visible = false;
            mapSuccess.Visible = false;

        }

        protected void mapUpdates(SqlDataSource temp, string latitude, string longitude, int format)
        {
            latDD.DataSource = temp;
            latDD.DataValueField = "COLUMN_NAME";
            latDD.DataTextField = "COLUMN_NAME";
            latDD.DataBind();
            latDD.SelectedValue = latitude;
            latUP.Update();
            longDD.DataSource = temp;
            longDD.DataValueField = "COLUMN_NAME";
            longDD.DataTextField = "COLUMN_NAME";
            longDD.DataBind();
            longDD.SelectedValue = longitude;
            longUP.Update();
            llDD.DataSource = temp;
            llDD.DataValueField = "COLUMN_NAME";
            llDD.DataTextField = "COLUMN_NAME";
            llDD.DataBind();
            llDD.SelectedValue = latitude;
            llUP.Update();

            if (format == 1)
            {
                LLSepPanel.Visible = true;
                LLTogetherPanel.Visible = false;
            }
            else
            {
                if (format == 2)
                {
                    LatLongCheck.Checked = true;
                    LongLatCheck.Checked = false;
                }
                else
                {
                    LatLongCheck.Checked = false;
                    LongLatCheck.Checked = true;
                }

                LLSepPanel.Visible = false;
                LLTogetherPanel.Visible = true;
            }

            mapError1.Visible = false;
            mapError2.Visible = false;
            mapSuccess.Visible = false;

        }

        protected void addLatLong_Click(object sender, EventArgs e)
        {
            int conID = Convert.ToInt32(Request.QueryString.Get("ConnID"));
            ConnInfo connInfo_editor = ConnInfo.getConnInfo(conID);

            string selectedTable = GridViewTables.SelectedValue.ToString();
            string connectionString_editor = "";
            string providerName_editor = "";

            SqlDataSource temp = new SqlDataSource();

            //Set drop down box accordingly
            if (connInfo_editor.getDatabaseType() == ConnInfo.MSSQL)
            {
                connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Initial Catalog=" + connInfo_editor.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword();
                temp.ConnectionString = connectionString_editor;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";

            }

            else if (connInfo_editor.getDatabaseType() == ConnInfo.MYSQL)
            {
                connectionString_editor = "server=" + connInfo_editor.getServerAddress() + ";User Id=" + connInfo_editor.getUserName() + ";password=" + connInfo_editor.getPassword() + ";Persist Security Info=True;database=" + connInfo_editor.getDatabaseName();
                providerName_editor = "MySql.Data.MySqlClient";

                temp.ConnectionString = connectionString_editor;
                temp.ProviderName = providerName_editor;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";


            }

            else if (connInfo_editor.getDatabaseType() == ConnInfo.ORACLE)
            {
                connectionString_editor = "Data Source=" + connInfo_editor.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo_editor.getUserName() + ";Password=" + connInfo_editor.getPassword() + ";Unicode=True";
                providerName_editor = "System.Data.OracleClient";

                temp.ConnectionString = connectionString_editor;
                temp.ProviderName = providerName_editor;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedTable + "')";

            }

            else //Default set to SQL
            {
            }


            Mapping conMapping = Mapping.getMapping2(conID, selectedTable);

            string dbTable = conMapping.getTableName();

            int dbFormat = conMapping.getFormat();

            string dbLat = conMapping.getLatFieldName();
            string dbLong = conMapping.getLongFieldName();

            //No information about the table yet
            if (dbTable == null)
            {
                mapUpdates(temp);
            }

            else
            {
                mapUpdates(temp, dbLat, dbLong, dbFormat);
            }


            //Garbage collection
            connInfo_editor = null;

            viewGrid.Visible = true;
            saveLatLong.Visible = true;
            addLatLong.Visible = false;
            tblColumnsPanel.Visible = false;
            mapColumnsPanel.Visible = true;

        }

        protected void viewGrid_Click(object sender, EventArgs e)
        {
            viewGrid.Visible = false;
            saveLatLong.Visible = false;
            addLatLong.Visible = true;
            mapColumnsPanel.Visible = false;
            tblColumnsPanel.Visible = true;
        }

        protected void saveLatLong_Click(object sender, EventArgs e)
        {
            string tableName = GridViewTables.SelectedValue.ToString();
            string latFieldName = "";
            string longFieldName = "";
            int format = 1;

            int conID = Convert.ToInt32(Request.QueryString.Get("ConnID"));
            Mapping saveMapping = Mapping.getMapping2(conID, tableName);
            string mapTblName = saveMapping.getTableName();

            if (LLSepPanel.Visible == true)
            {
                latFieldName = latDD.SelectedValue.ToString();
                longFieldName = longDD.SelectedValue.ToString();
                if (latFieldName.Equals(longFieldName))
                {
                    mapError1.Visible = true;
                    mapSuccess.Visible = false;
                }
                else
                {
                    mapError1.Visible = false;
                    mapError2.Visible = false;

                    if (mapTblName == null)
                    {
                        Mapping.insertMapping(conID, tableName, latFieldName, longFieldName, format);
                        mapSuccess.Text = "Lat/Long Mapping inserted successfully!";
                        mapSuccess.Visible = true;
                    }
                    else
                    {
                        Mapping.updateMapping(conID, tableName, latFieldName, longFieldName, format);
                        mapSuccess.Text = "Lat/Long Mapping updated successfully!";
                        mapSuccess.Visible = true;
                    }

                }
            }
            else if (LLTogetherPanel.Visible == true)
            {
                latFieldName = llDD.SelectedValue.ToString();
                longFieldName = latFieldName;
                if ((LatLongCheck.Checked && LongLatCheck.Checked) || (!LatLongCheck.Checked && !LongLatCheck.Checked))
                {
                    mapSuccess.Visible = false;
                    mapError2.Visible = true;
                }
                else
                {
                    mapError1.Visible = false;
                    mapError2.Visible = false;

                    if (LongLatCheck.Checked)
                    {
                        format = 3;
                    }
                    else
                    {
                        format = 2;
                    }
                    if (mapTblName == null)
                    {
                        Mapping.insertMapping(conID, tableName, latFieldName, longFieldName, format);
                        mapSuccess.Text = "Lat/Long Mapping inserted successfully!";
                        mapSuccess.Visible = true;
                    }
                    else
                    {
                        Mapping.updateMapping(conID, tableName, latFieldName, longFieldName, format);
                        mapSuccess.Text = "Lat/Long Mapping updated successfully!";
                        mapSuccess.Visible = true;
                    }

                }
            }
        }
    }
}
