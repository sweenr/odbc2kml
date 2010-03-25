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
                    
                    //Set drop down box accordingly
                    if (connInfo.getDatabaseType() == ConnInfo.MSSQL) 
                    {
                        odbcDBType.SelectedValue = "SQL";
                    }
                    else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)                    
                    {
                        odbcDBType.SelectedValue = "MySQL";
                    }
                    else if (connInfo.getDatabaseType() == ConnInfo.ORACLE)
                    {
                        odbcDBType.SelectedValue = "Oracle";
                    }
                    else //Default set to SQL
                    {
                        odbcDBType.SelectedValue = "SQL";
                    }

                    //Garbage collection
                    connInfo = null;

                    fillIconLibraryLists();
                    fillIconListFromDatabase();
                }
            }

            fillIconLibraryPopup();
            fillIconLibraryPopupRemove();

            genIconConditionTable(sender, e);
            
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


            //Database db = new Database();
            //db.executeQueryLocal("DELETE FROM Icon WHERE ID=" + args + " AND connID=" + Request.QueryString.Get("ConnID"));

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
                IconConditionPanel.Controls.Add(new LiteralControl("<img src=\""+icon.getLocation()+"\" alt=\"\" />\n"));  // TODO: change this to pic from icon.location
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
                Panel modifyIconConditionInsidePopupPanel = new Panel();
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

        protected void genIconConditionPopup(Panel modifyIconConditionInsidePopupPanel, String args)
        {
            //modifyIconConditionInsidePopupPanel.Controls.Clear();
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<span class=\"connectionStyle\">&nbsp;Modify Condition</span>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<div class=\"mainBoxP\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<table cellspacing=\"0\" cellpadding=\"5\" class=\"mainBox2\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<tr>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<div class=\"omainBox4\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<table class=\"omainBox6\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<tr>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Add and remove conditions for this icon using the table below.\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</tr>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</table>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<p>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</p>\n"));

            // icon popup table's header
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Lower Bound\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Lower Operator\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Table\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Field\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Upper Operator\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Upper Bound\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("&nbsp;\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</tr>\n"));
            int i = 0;
            
            foreach (Icon icon in iconList)
            {
                if (icon.getId() != args)
                    continue;

                foreach (Condition condition in icon.getConditions())
                {
                    i++;
                    modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<tr>\n"));
                    if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                    {
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl(condition.getLowerBound() + "\n"));
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl(condition.getLowerOperator() + "\n"));
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
                    }
                    else
                    {
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">&nbsp;</td><td class=\"tableTD\">&nbsp;</td>\n"));
                    }
                    modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                    modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl(condition.getTableName() + "\n"));
                    modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
                    modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                    modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl(condition.getFieldName() + "\n"));
                    modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
                    if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                    {
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl(condition.getUpperOperator() + "\n"));
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl(condition.getUpperBound() + "\n"));
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
                    }
                    else
                    {
                        modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">&nbsp;</td><td class=\"tableTD\">&nbsp;</td>\n"));
                    }
                    modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"textCenter\">\n"));
                    Button deleteConditionButton = new Button();
                    deleteConditionButton.ID = "delCondition_" + args + "_" + i.ToString();

                    deleteConditionButton.Text = "Remove";
                    deleteConditionButton.CssClass = "button";
                    deleteConditionButton.ToolTip = "Delete Condition";
                    deleteConditionButton.Width = 80;
                    deleteConditionButton.Click += new EventHandler(deleteIconCondition);
                    deleteConditionButton.CommandArgument = icon.getId() + " " + condition.getId();
                    modifyIconConditionInsidePopupPanel.Controls.Add(deleteConditionButton);
                    modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
                    modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</tr>\n"));

                }
            }

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<tr class=\"tableTR\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            TextBox addLowerBound = new TextBox();
            addLowerBound.ID = "addLowerBound" + args;
            addLowerBound.Width = 50;
            modifyIconConditionInsidePopupPanel.Controls.Add(addLowerBound);
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addLowerOperator = new DropDownList();
            addLowerOperator.ID = "addLowerOperator" + args;
            addLowerOperator.CssClass = "inputDD";
            addLowerOperator.Width = 50;
            addLowerOperator.Items.Add("");
            addLowerOperator.Items.Add("<");
            addLowerOperator.Items.Add("<=");
            addLowerOperator.Items.Add("==");
            modifyIconConditionInsidePopupPanel.Controls.Add(addLowerOperator);
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            TextBox addTableName = new TextBox();
            addTableName.ID = "addTableName" + args;
            addTableName.Width = 50;
            modifyIconConditionInsidePopupPanel.Controls.Add(addTableName);
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            TextBox addFieldName = new TextBox();
            addFieldName.ID = "addFieldName" + args;
            addFieldName.Width = 50;
            modifyIconConditionInsidePopupPanel.Controls.Add(addFieldName);
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList addUpperOperator = new DropDownList();
            addUpperOperator.ID = "modIconConditionList2" + args;
            addUpperOperator.CssClass = "inputDD";
            addUpperOperator.Width = 50;
            addUpperOperator.Items.Add("");
            addUpperOperator.Items.Add("<");
            addUpperOperator.Items.Add("<=");
            addUpperOperator.Items.Add("==");
            modifyIconConditionInsidePopupPanel.Controls.Add(addUpperOperator);
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            TextBox addUpperBound = new TextBox();
            addUpperBound.ID = "addUpperBound" + args;
            addUpperBound.MaxLength = 30;
            addUpperBound.CssClass = "inputBox";
            addUpperBound.Width = 50;
            modifyIconConditionInsidePopupPanel.Controls.Add(addUpperBound);
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"textCenter\">\n"));
            Button addConditionButton = new Button();
            addConditionButton.ID = "addIconConditionButton" + args;
            addConditionButton.Text = "Add";
            addConditionButton.ToolTip = "Add Condition";
            addConditionButton.Width = 80;
            addConditionButton.CssClass = "button";
            addConditionButton.Click += new EventHandler(addConditionToIcon);
            addConditionButton.CommandArgument = args;
            //addConditionButton.CommandArgument = addLowerBound.Text.ToString() + "|" + addLowerOperator.SelectedItem.Text.ToString() + "|" + addTableName.Text.ToString() + "|" + addFieldName.Text.ToString() + "|" + addUpperOperator.SelectedItem.Text.ToString() + "|" + addUpperBound.Text.ToString() + "|" + args;
            modifyIconConditionInsidePopupPanel.Controls.Add(addConditionButton);
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</tr>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</table>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</div>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</tr>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</table>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</div>\n"));
            
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
                condition.setLowerOperator(Condition.operatorStringToInt(lowerOperator.SelectedItem.Text.ToString()));
            if (upperOperator != null)
                condition.setUpperOperator(Condition.operatorStringToInt(upperOperator.SelectedItem.Text.ToString()));

            foreach (Icon icon in iconList)
            {
                if (icon.getId() == iconId)
                {
                    icon.setConditions(condition);
                }
            }
            genIconConditionTable(sender, e);
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
        public static String tempSaveLoc = @"C:\odbc2kml\temp\";
        public static String fileSaveLoc = @"C:\odbc2kml\uploads\";
        public ArrayList validTypes = new ArrayList();
        private static ArrayList iconList = new ArrayList();
        private static ArrayList iconListAvailableToAdd = new ArrayList();
        private static ArrayList iconListAvailableToRemove = new ArrayList();
        private ArrayList overlayList = new ArrayList();
    }
}
