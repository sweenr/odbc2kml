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
                    
                }
            }

            fillIconLibraryPopup();

            fillIconLibraryPopupRemove();

            if (!IsPostBack)
            {
                fillIconListFromDatabase();
            }

            genIconConditionTable(sender, e);
            
            BuildTypeList();
        }

        //populates the popup panel for removing an icon from a connection
        protected void fillIconLibraryPopupRemove()
        {
            Database db = new Database();
            DataTable dt;
            dt = db.executeQueryLocal("SELECT IconLibrary.ID, IconLibrary.location FROM IconLibrary,Icon Where IconLibrary.ID=Icon.ID AND Icon.ConnID="+Request.QueryString.Get("ConnID"));

            int sizeOfBox = 8;
            int currentBoxCount = 0;

            removeIconFromConn.Controls.Add(new LiteralControl("<table class=\"boxPopupStyle2\" cellpadding=\"5\">\n"));
            foreach (DataRow dr in dt.Rows)
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
                imgBtn.ID = "imgLib_" + dr.ItemArray.ElementAt(0);
                imgBtn.ImageUrl = dr.ItemArray.ElementAt(1).ToString();
                imgBtn.Click += new ImageClickEventHandler(removeIconFromConnFunct);
                imgBtn.CommandArgument = dr.ItemArray.ElementAt(0).ToString();

                removeIconFromConn.Controls.Add(imgBtn);
                removeIconFromConn.Controls.Add(new LiteralControl("</td>"));


                currentBoxCount += 1;
            }
            removeIconFromConn.Controls.Add(new LiteralControl("</table>\n"));
        }

        //populates the popup panel for adding an icon to a connection from the icon library
        protected void fillIconLibraryPopup()
        {
            Database db = new Database();
            DataTable dt;
            dt = db.executeQueryLocal("SELECT ID, location FROM IconLibrary AS IL WHERE (NOT EXISTS (SELECT ID, connID FROM Icon AS IC WHERE (connID = "+Request.QueryString.Get("ConnID")+" ) AND (ID = IL.ID)))");

            int sizeOfBox = 8;
            int currentBoxCount = 0;

            addIconToLibary.Controls.Add(new LiteralControl("<table class=\"boxPopupStyle2\" cellpadding=\"5\">\n"));
            foreach (DataRow dr in dt.Rows)
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
                imgBtn.ID = "imgLib_" + dr.ItemArray.ElementAt(0);
                imgBtn.ImageUrl = dr.ItemArray.ElementAt(1).ToString();
                imgBtn.Click += new ImageClickEventHandler(addIconFromLibraryToConn);
                imgBtn.CommandArgument = dr.ItemArray.ElementAt(0).ToString();

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

            Database db = new Database();
            db.executeQueryLocal("INSERT INTO ICON (ID, connID) VALUES ('" + args + "', '" + Request.QueryString.Get("ConnID") + "')");

        }

        //Removes an icon assocaiated with a connection and all conditions associated with it
        protected void removeIconFromConnFunct(object sender, EventArgs e)
        {
            ImageButton sendBtn = (ImageButton)sender;
            String args = sendBtn.CommandArgument.ToString();

            Database db = new Database();
            db.executeQueryLocal("DELETE FROM Icon WHERE ID=" + args + " AND connID=" + Request.QueryString.Get("ConnID"));

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

//Create Panel for latLong button................................
//            Panel LatLongDetailsPopupPanel = new Panel();
//            LatLongDetailsPopupPanel.ID = "LatLongDetailsPopupPanel";
//            LatLongDetailsPopupPanel.CssClass = "boxPopupStyle";
//
//            AjaxControlToolkit.ModalPopupExtender lld = new AjaxControlToolkit.ModalPopupExtender();
//            lld.PopupControlID = LatLongDetailsPopupPanel.ID.ToString();
//            lld.TargetControlID = latLong.ID.ToString();
//.......................................................
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
                dt2 = db2.executeQueryLocal("SELECT ID, fieldName, tableName, lowerBound, upperBound, lowerOperator, upperOperator FROM IconCondition WHERE connID = " + Request.QueryString.Get("ConnID") + " AND iconID = " + iconId);
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
                    icon.setConditions(condition);
                }
                iconList.Add(icon);
            }
            Database db3 = new Database();
            DataTable dt3;
            try
            {
                dt3 = db3.executeQueryLocal("SELECT ID, location FROM IconLibrary WHERE (ID = (SELECT ID FROM Icon AS IX WHERE (NOT EXISTS (SELECT DISTINCT iconID FROM IconCondition AS IC WHERE (connID = " + Request.QueryString.Get("ConnID") + ") AND (iconID = IX.ID))) AND (connID = " + Request.QueryString.Get("ConnID") + ")))");
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                eh.displayError();
                return;
            }
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
            foreach (Icon icon in iconList)
            {
                IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"iconBox\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<img src=\""+icon.getLocation()+"\" alt=\"\" />\n"));  // TODO: change this to pic from icon.location
                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyle\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<table cellpadding=\"10\">\n"));
                
                foreach (Condition condition in icon.getConditions())
                {
                    //IconConditionPanel.Controls.Add(new LiteralControl("<tr><td>"));
                    if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getLowerBound() + " " + condition.getLowerOperator() + " "));
                    IconConditionPanel.Controls.Add(new LiteralControl(condition.getTableName() + "." + condition.getFieldName() + " "));
                    if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getUpperOperator() + " " + condition.getUpperBound() + " "));
                    IconConditionPanel.Controls.Add(new LiteralControl("<br />\n"));
                    //IconConditionPanel.Controls.Add(new LiteralControl("</td></tr>"));
                }
                
                IconConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</div>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"buttonClass\">\n"));

                Button modifyButton = new Button();
                modifyButton.Text = "Modify Condition";
                modifyButton.CssClass = "button";
                modifyButton.Width = 135;
                modifyButton.ID = "modifyIconCondition_" + icon.getId();
                IconConditionPanel.Controls.Add(modifyButton);

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

                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
            }           
            
            
        }

        protected void genIconConditionPopup(Panel modifyIconConditionInsidePopupPanel, String args)
        {
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
                    deleteConditionButton.ID = "delCondition" + condition.getId();

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
            addLowerBound.ID = "addLowerBound" + args + "_" + i;
            addLowerBound.Width = 50;
            modifyIconConditionInsidePopupPanel.Controls.Add(addLowerBound);

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));

            DropDownList list1 = new DropDownList();

            DropDownList addLowerOperator = new DropDownList();
            addLowerOperator.ID = "addLowerOperator" + args + "_" + i;
            addLowerOperator.CssClass = "inputDD";
            addLowerOperator.Width = 50;
            addLowerOperator.Items.Add("<");
            addLowerOperator.Items.Add("<=");
            addLowerOperator.Items.Add("==");
            modifyIconConditionInsidePopupPanel.Controls.Add(addLowerOperator);

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));

            TextBox addTableName = new TextBox();
            addTableName.ID = "addTableName" + args + "_" + i;
            addTableName.Width = 50;
            modifyIconConditionInsidePopupPanel.Controls.Add(addTableName);

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));

            TextBox addFieldName = new TextBox();
            addFieldName.ID = "addFieldName" + args + "_" + i;
            addFieldName.Width = 50;
            modifyIconConditionInsidePopupPanel.Controls.Add(addFieldName);

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            
            DropDownList addUpperOperator = new DropDownList();
            addUpperOperator.ID = "modIconConditionList2" + args + "_" + i;
            addUpperOperator.CssClass = "inputDD";
            addUpperOperator.Width = 50;
            addUpperOperator.Items.Add("<");
            addUpperOperator.Items.Add("<=");
            addUpperOperator.Items.Add("==");
            modifyIconConditionInsidePopupPanel.Controls.Add(addUpperOperator);

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));

            TextBox addUpperBound = new TextBox();
            addUpperBound.ID = "addUpperBound" + args + "_" + i;
            addUpperBound.MaxLength = 30;
            addUpperBound.CssClass = "inputBox";
            addUpperBound.Width = 50;
            modifyIconConditionInsidePopupPanel.Controls.Add(addUpperBound);

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"textCenter\">\n"));

            Button addConditionButton = new Button();
            addConditionButton.ID = "addIconConditionButton" + args + "_" + i;
            addConditionButton.Text = "Add";
            addConditionButton.ToolTip = "Add Condition";
            addConditionButton.Width = 80;
            addConditionButton.CssClass = "button";
            addConditionButton.Click += new EventHandler(addConditionToIcon);
            addConditionButton.CommandArgument = addLowerBound.Text + "|" + addLowerOperator.Text + "|" + addTableName.Text + "|" + addFieldName.Text + "|" + addUpperOperator.Text + "|" + addUpperBound.Text + "|" + args;
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

        protected void genAddIconConditionPopup(Panel addIconConditionPopupPanel)
        {
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<span class=\"connectionStyle\">&nbsp;Modify Condition</span>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<div class=\"mainBoxP\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<table cellspacing=\"0\" cellpadding=\"5\" class=\"mainBox2\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<tr>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<div class=\"omainBox4\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<table class=\"omainBox6\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<tr>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("Enter new conditions for this icon using the table below.\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</tr>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</table>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<p>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</p>\n"));

            // icon popup table's header
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("Lower Bound\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("Lower Operator\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("Table\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("Field\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("Upper Operator\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("Upper Bound\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</tr>\n"));

            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<tr class=\"tableTR\">\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            TextBox addLowerBound = new TextBox();
            addLowerBound.ID = "addIconConditionLowerBound";
            addLowerBound.Width = 50;
            addIconConditionPopupPanel.Controls.Add(addLowerBound);

            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));

            DropDownList list1 = new DropDownList();

            DropDownList addLowerOperator = new DropDownList();
            addLowerOperator.ID = "addIconConditionLowerOperator";
            addLowerOperator.CssClass = "inputDD";
            addLowerOperator.Width = 50;
            addLowerOperator.Items.Add("<");
            addLowerOperator.Items.Add("<=");
            addLowerOperator.Items.Add("==");
            addIconConditionPopupPanel.Controls.Add(addLowerOperator);

            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));

            TextBox addTableName = new TextBox();
            addTableName.ID = "addIconConditionTableName";
            addTableName.Width = 50;
            addIconConditionPopupPanel.Controls.Add(addTableName);

            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));

            TextBox addFieldName = new TextBox();
            addFieldName.ID = "addIconConditionFieldName";
            addFieldName.Width = 50;
            addIconConditionPopupPanel.Controls.Add(addFieldName);

            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));

            DropDownList addUpperOperator = new DropDownList();
            addUpperOperator.ID = "addIconConditionUpperOperator";
            addUpperOperator.CssClass = "inputDD";
            addUpperOperator.Width = 50;
            addUpperOperator.Items.Add("<");
            addUpperOperator.Items.Add("<=");
            addUpperOperator.Items.Add("==");
            addIconConditionPopupPanel.Controls.Add(addUpperOperator);

            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));

            TextBox addUpperBound = new TextBox();
            addUpperBound.ID = "addIconConditionUpperBound";
            addUpperBound.MaxLength = 30;
            addUpperBound.CssClass = "inputBox";
            addUpperBound.Width = 50;
            addIconConditionPopupPanel.Controls.Add(addUpperBound);

            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</tr>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</table>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</div>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</tr>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</table>\n"));
            addIconConditionPopupPanel.Controls.Add(new LiteralControl("</div>\n"));
        }
        /// <summary>
        /// used for uploading icons from local computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

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
        }

        protected void addConditionToIcon(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int[] breaks;
            breaks = new int[7];
            int i = 0;
            for (int j = 0; j < btn.CommandArgument.Length; j++)
            {
                if (i >= 7)
                    continue;
                if (btn.CommandArgument[j] == '|')
                {
                    breaks[i] = j;
                    i++;
                }
            }
            string lowerBound = btn.CommandArgument.Substring(0,breaks[0]);
            string lowerOperator = btn.CommandArgument.Substring(breaks[0] + 1, breaks[1] - breaks[0] - 1);
            string tableName = btn.CommandArgument.Substring(breaks[1] + 1, breaks[2] - breaks[1] - 1);
            string fieldName = btn.CommandArgument.Substring(breaks[2] + 1, breaks[3] - breaks[2] - 1);
            string upperOperator = btn.CommandArgument.Substring(breaks[3] + 1, breaks[4] - breaks[3] - 1);
            string upperBound = btn.CommandArgument.Substring(breaks[4] + 1, breaks[5] - breaks[4] - 1);
            string iconId = btn.CommandArgument.Substring(breaks[5] + 1);

            Condition condition = new Condition();
            condition.setLowerBound(lowerBound);
            condition.setUpperBound(upperBound);
            condition.setTableName(tableName);
            condition.setFieldName(fieldName);
            switch (lowerOperator)
            {
                case "<":
                    condition.setLowerOperator(1);
                    break;
                case "<=":
                    condition.setLowerOperator(2);
                    break;
                case "==":
                    condition.setLowerOperator(5);
                    break;
                case "!=":
                    condition.setLowerOperator(6);
                    break;
                default:
                    condition.setLowerOperator(0);
                    break;
            }
            switch (upperOperator)
            {
                case "<":
                    condition.setUpperOperator(1);
                    break;
                case "<=":
                    condition.setUpperOperator(2);
                    break;
                case "==":
                    condition.setUpperOperator(5);
                    break;
                case "!=":
                    condition.setUpperOperator(6);
                    break;
                default:
                    condition.setUpperOperator(0);
                    break;
            }

            foreach (Icon icon in iconList)
            {
                if (icon.getId() == iconId)
                {
                    icon.setConditions(condition);
                }
            }
        }

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
                    String relativeName = relativeFileSaveLoc + file;
                    //save the file to the server
                    fileUpEx.PostedFile.SaveAs(fileSaveLoc + file);
                    //errorPanel1.Text = "File Saved to: " + fileSaveLoc + file;
                    Database DB = new Database();
                    try
                    {
                        DB.executeQueryLocal("INSERT INTO IconLibrary (location, isLocal) VALUES (\'" + relativeName + "\', 1)");
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }
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
                String relativeName = relativeFileSaveLoc + fileName + suffix + ext;
                //checks if icon has valid dimensions
                if (valid && ValidateFileDimensions(fs))
                {
                    fs.Close();
                    File.Move(tempName, Name);
                    File.Delete(tempName);
                    try
                    {
                        DB.executeQueryLocal("INSERT INTO IconLibrary (location, isLocal) VALUES (\'" + relativeName + "\', 1)");
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }
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
            else
            {
                //checks if icon has valid dimensions
                if (valid && ValidateFileDimensions(fs))
                {
                    fs.Close();
                    File.Delete(tempName);
                    try
                    {
                        DB.executeQueryLocal("INSERT INTO IconLibrary (location, isLocal) VALUES (\'" + URL + "\', 0)");
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }
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
        public static String fileSaveLoc = @"C:\Users\JP\Documents\Senior Design\hci\HCI\icons\";
        public static String relativeFileSaveLoc = @"icons/";
        public ArrayList validTypes = new ArrayList();
        private static ArrayList iconList = new ArrayList();
        private static ArrayList overlayList = new ArrayList();
    }
}
