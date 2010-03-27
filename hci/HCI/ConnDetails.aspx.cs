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
                    
                    fillIconLibraryLists();
                    fillOverlayLibraryLists();
                    fillIconListFromDatabase();
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

        protected void updateConnection(object sender, EventArgs e)
        {

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
                    dt2 = db2.executeQueryLocal("SELECT ID, fieldName, tableName, lowerBound, upperBound, lowerOperator, upperOperator FROM OverlayCondition WHERE connID = " + Request.QueryString.Get("ConnID") + " AND overlayID = " + overID);
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

        protected void genDBTCol(object sender, EventArgs e)
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

        }
        
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
                dt2 = db2.executeQueryLocal("SELECT fieldName, tableName, lowerBound, upperBound, lowerOperator, upperOperator FROM IconCondition WHERE connID = " + Request.QueryString.Get("ConnID") + " AND iconID = " + iconId);
                foreach (DataRow dr2 in dt2.Rows)
                {
                    Condition condition = new Condition();
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
            foreach (Icon icon in iconList)
            {
                IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"iconBox\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<img src=\""+icon.getLocation()+"\" alt=\"\" />\n"));
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
                modifyIconConditionInsidePopupPanel.ID = "modifyIconConditionInsidePopupPanel" + iconList.IndexOf(icon).ToString();
                genIconConditionPopup(modifyIconConditionInsidePopupPanel, icon.getId());
                modifyIconConditionPopupPanel.Controls.Add(modifyIconConditionInsidePopupPanel);

                Button submitModifyConditionPopup = new Button();
                submitModifyConditionPopup.ID = "submitModifyCondition" + icon.getId();
                submitModifyConditionPopup.Text = "Submit";
                modifyIconConditionPopupPanel.Controls.Add(submitModifyConditionPopup);
                Button cancelModifyConditionPopup = new Button();
                cancelModifyConditionPopup.ID = "cancelModifyCondition" + icon.getId();
                cancelModifyConditionPopup.Text = "Cancel";
                modifyIconConditionPopupPanel.Controls.Add(cancelModifyConditionPopup);

                if (Request.QueryString.Get("locked") == "false")
                {
                    AjaxControlToolkit.ModalPopupExtender mpe = new AjaxControlToolkit.ModalPopupExtender();
                    mpe.ID = "MPE_" + icon.getId();
                    mpe.BackgroundCssClass = "modalBackground";
                    mpe.DropShadow = true;
                    mpe.PopupControlID = modifyIconConditionPopupPanel.ID.ToString();
                    mpe.TargetControlID = modifyButton.ID.ToString();
                    mpe.OkControlID = submitModifyConditionPopup.ID.ToString();
                    mpe.CancelControlID = cancelModifyConditionPopup.ID.ToString();
                    IconConditionPanel.Controls.Add(mpe);

                    IconConditionPanel.Controls.Add(modifyIconConditionPopupPanel);
                }
                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
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
            addLowerBound.ID = "addLowerBound" + args;
            addLowerBound.Width = 50;
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addLowerBound);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addLowerOperator = new DropDownList();
            addLowerOperator.ID = "addLowerOperator" + args;
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
            addTableName.ID = "addIconConditionTable" + args;
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
            
            addTableName.SelectedIndexChanged += new EventHandler(addTableName_SelectedIndexChanged);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addTableName);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addFieldName = new DropDownList();
            addFieldName.ID = "addIconConditionField" + args;
            addFieldName.CssClass = "inputDD";
            addFieldName.Width = 50;
            addFieldName.AutoPostBack = true;
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addFieldName);
            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addUpperOperator = new DropDownList();
            addUpperOperator.ID = "addUpperOperator" + args;
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
            addUpperBound.ID = "addUpperBound" + args;
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
            TextBox lowerBound = (TextBox)Page.FindControl("addLowerBound" + args);
            DropDownList lowerOperator = (DropDownList)Page.FindControl("addLowerOperator" + args);
            TextBox tableName = (TextBox)Page.FindControl("addTableName" + args);
            TextBox fieldName = (TextBox)Page.FindControl("addFieldName" + args);
            DropDownList upperOperator = (DropDownList)Page.FindControl("addUpperOperator" + args);
            TextBox upperBound = (TextBox)Page.FindControl("addUpperBound" + args);
            string iconId = args;

            Condition condition = new Condition();
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
                modifyOverlayConditionInsidePopupPanel.ID = "modifyOverlayConditionInsidePopupPanel" + overlayList.IndexOf(overlay).ToString();
                genOverlayConditionPopup(modifyOverlayConditionInsidePopupPanel, overlay.getId());
                modifyOverlayConditionPopupPanel.Controls.Add(modifyOverlayConditionInsidePopupPanel);

                Button submitModifyConditionPopup = new Button();
                submitModifyConditionPopup.ID = "submitOverlayModifyCondition" + overlay.getId();
                submitModifyConditionPopup.Text = "Submit";
                modifyOverlayConditionPopupPanel.Controls.Add(submitModifyConditionPopup);
                Button cancelModifyConditionPopup = new Button();
                cancelModifyConditionPopup.ID = "cancelOverlayModifyCondition" + overlay.getId();
                cancelModifyConditionPopup.Text = "Cancel";
                modifyOverlayConditionPopupPanel.Controls.Add(cancelModifyConditionPopup);

                if (Request.QueryString.Get("locked") == "false")
                {
                    AjaxControlToolkit.ModalPopupExtender mpe = new AjaxControlToolkit.ModalPopupExtender();
                    mpe.ID = "MPE_OVERLAY_" + overlay.getId();
                    mpe.BackgroundCssClass = "modalBackground";
                    mpe.DropShadow = true;
                    mpe.PopupControlID = modifyOverlayConditionPopupPanel.ID.ToString();
                    mpe.TargetControlID = modifyButton.ID.ToString();
                    mpe.OkControlID = submitModifyConditionPopup.ID.ToString();
                    mpe.CancelControlID = cancelModifyConditionPopup.ID.ToString();
                    OverlayConditionPanel.Controls.Add(mpe);

                    OverlayConditionPanel.Controls.Add(modifyOverlayConditionPopupPanel);
                }
                OverlayConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                OverlayConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
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
            TextBox addTableName = new TextBox();
            addTableName.ID = "addOverlayTableName" + args;
            addTableName.Width = 50;
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(addTableName);
            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("</td>\n"));

            modifyOverlayConditionInsidePopupPanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            TextBox addFieldName = new TextBox();
            addFieldName.ID = "addOverlayFieldName" + args;
            addFieldName.Width = 50;
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
            TextBox tableName = (TextBox)Page.FindControl("addOverlayTableName" + args);
            TextBox fieldName = (TextBox)Page.FindControl("addOverlayFieldName" + args);
            DropDownList upperOperator = (DropDownList)Page.FindControl("addOverlayUpperOperator" + args);
            TextBox upperBound = (TextBox)Page.FindControl("addOverlayUpperBound" + args);
            string overlayId = args;

            Condition condition = new Condition();
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
            string fieldListId = "addIconConditionField" + tableList.ID.Substring(tableList.ID.LastIndexOf("addIconConditionTable") + 1);
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

        //private bool fetch;
        //private String URL;
        //private Database DB;
        public const int height = 128;
        public const int width = 128;
        private static int curOverlayCount = -1;
        public static String tempSaveLoc = @"C:\odbc2kml\temp\";
        public static String fileSaveLoc = @"C:\odbc2kml\uploads\";
        public ArrayList validTypes = new ArrayList();
        private static ArrayList iconList = new ArrayList();
        private static ArrayList iconListAvailableToAdd = new ArrayList();
        private static ArrayList iconListAvailableToRemove = new ArrayList();
        private static ArrayList overlayListAvailableToRemove = new ArrayList();
        private static ArrayList overlayList = new ArrayList();
        private static ArrayList tableNameList = new ArrayList();
        private static SqlDataSource MSQLTables = new SqlDataSource();
        private static SqlDataSource SQLTables = new SqlDataSource();
        private static SqlDataSource oracleTables = new SqlDataSource();
    }
}
