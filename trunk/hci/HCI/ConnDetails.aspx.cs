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

            genIconConditionTable(sender, e);
            
            BuildTypeList();
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


        protected void genIconConditionTable(object sender, EventArgs e)
        {
            Database db = new Database();
            DataTable dt;
            dt = db.executeQueryLocal("SELECT DISTINCT iconID FROM IconCondition WHERE connID = " + Request.QueryString.Get("ConnID"));
            foreach (DataRow dr in dt.Rows)
            {
                string iconId = dr.ItemArray.ElementAt(0).ToString();
                IconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"iconBox\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<img src=\"icons/cycling.png\" alt=\"\" />\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyle\">\n"));
                IconConditionPanel.Controls.Add(new LiteralControl("<table cellpadding=\"10\">\n"));
                
                Database db2 = new Database();
                DataTable dt2;
                dt2 = db2.executeQueryLocal("SELECT fieldName, tableName, lowerBound, upperBound, lowerOperator, upperOperator FROM IconCondition WHERE connID = " + Request.QueryString.Get("ConnID") + " AND iconID = " + iconId);
                foreach (DataRow dr2 in dt2.Rows)
                {
                    //IconConditionPanel.Controls.Add(new LiteralControl("<tr><td>"));
                    Condition condition = new Condition(dr2.ItemArray.ElementAt(0).ToString(), dr2.ItemArray.ElementAt(1).ToString(),
                        dr2.ItemArray.ElementAt(2).ToString(), dr2.ItemArray.ElementAt(3).ToString(),
                        dr2.ItemArray.ElementAt(4).ToString(), dr2.ItemArray.ElementAt(5).ToString());
                    if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getLowerBound() + " " + condition.getLowerOperator() + " "));
                    IconConditionPanel.Controls.Add(new LiteralControl(condition.getTableName() + "." + condition.getFieldName() + " "));
                    if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                        IconConditionPanel.Controls.Add(new LiteralControl(condition.getUpperBound() + " " + condition.getUpperOperator() + " "));
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
                modifyButton.ID = "modifyIconCondition_" + iconId.ToString();
                IconConditionPanel.Controls.Add(modifyButton);

                Panel modifyIconConditionPopupPanel = new Panel();
                modifyIconConditionPopupPanel.ID = "modifyIconConditionPopupPanel" + iconId.ToString();
                modifyIconConditionPopupPanel.CssClass = "boxPopupStyle";
                Panel modifyIconConditionInsidePopupPanel = new Panel();
                modifyIconConditionInsidePopupPanel.ID = "modifyIconConditionInsidePopupPanel" + iconId.ToString();
                genIconConditionPopup(modifyIconConditionInsidePopupPanel, iconId.ToString());
                modifyIconConditionPopupPanel.Controls.Add(modifyIconConditionInsidePopupPanel);

                Button submitModifyConditionPopup = new Button();
                submitModifyConditionPopup.ID = "submitModifyCondition" + iconId.ToString();
                submitModifyConditionPopup.Text = "Submit";
                modifyIconConditionPopupPanel.Controls.Add(submitModifyConditionPopup);
                Button cancelModifyConditionPopup = new Button();
                cancelModifyConditionPopup.ID = "cancelModifyCondition" + iconId.ToString();
                cancelModifyConditionPopup.Text = "Cancel";
                modifyIconConditionPopupPanel.Controls.Add(cancelModifyConditionPopup);

                AjaxControlToolkit.ModalPopupExtender mpe = new AjaxControlToolkit.ModalPopupExtender();
                mpe.ID = "MPE_" + iconId.ToString();
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

            // Fill in Add Condition TR
            AddIconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("<td class=\"iconBox\">\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("<img src=\"icons/electronics.png\" alt=\"\" />\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("<td class=\"conditionsBox\">\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("<div class=\"conditionsBoxStyleEmpty\">\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("<table cellpadding=\"10\">\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("<tr>\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("<td>\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("</table>\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("</div>\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("<td class=\"buttonClass\">\n"));

            Button addButton = new Button();
            addButton.Text = "Add Condition";
            addButton.CssClass = "button";
            addButton.ID = "addIconCondition";
            addButton.Width = 135;
            AddIconConditionPanel.Controls.Add(addButton);


            AjaxControlToolkit.ModalPopupExtender mpe1 = new AjaxControlToolkit.ModalPopupExtender();
            mpe1.ID = "MPE_AddCondition";
            mpe1.BackgroundCssClass = "modalBackground";
            mpe1.DropShadow = true;
            mpe1.PopupControlID = testPanel1.ID.ToString();
            mpe1.TargetControlID = addButton.ID.ToString();
            mpe1.CancelControlID = testPanelCancel1.ID.ToString();


            AddIconConditionPanel.Controls.Add(mpe1);
            AddIconConditionPanel.Controls.Add(new LiteralControl("</td>\n"));
            AddIconConditionPanel.Controls.Add(new LiteralControl("</tr>\n"));
            
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
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Tool Directions :)\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</tr>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</table>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<p>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</p>\n"));


            Database db = new Database();
            DataTable dt;
            dt = db.executeQueryLocal("SELECT fieldName, tableName, lowerBound, upperBound, lowerOperator, upperOperator FROM IconCondition WHERE connID = " + Request.QueryString.Get("ConnID") + " AND iconID = " + args);
            foreach (DataRow dr in dt.Rows)
            {
                
            }


            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Table\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Field\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Operator\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Value\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("&nbsp;\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</tr>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<tr>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("TheOnlyTable\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("VehicleType\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("==\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("Tank\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"textCenter\">\n"));
            Button deleteConditionButton = new Button();
            deleteConditionButton.ID = "delCondition" + args;
            
            deleteConditionButton.Text = "Remove";
            deleteConditionButton.CssClass = "button";
            deleteConditionButton.ToolTip = "Delete Condition";
            deleteConditionButton.Width = 80;
            modifyIconConditionInsidePopupPanel.Controls.Add(deleteConditionButton);
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</tr>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<tr class=\"tableTR\">\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList list1 = new DropDownList();
            list1.ID = "modIconConditionList1" + args;
            list1.CssClass = "inputDD";
            list1.Width = 100;
            modifyIconConditionInsidePopupPanel.Controls.Add(list1);
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList list2 = new DropDownList();
            list2.ID = "modIconConditionList2" + args;
            list2.CssClass = "inputDD";
            list2.Width = 100;
            modifyIconConditionInsidePopupPanel.Controls.Add(list2);
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));
            DropDownList list3 = new DropDownList();
            list3.ID = "modIconConditionList3" + args;
            list3.CssClass = "inputDD";
            list3.Width = 100;
            list3.Items.Add("==");
            list3.Items.Add(">=");
            list3.Items.Add("<=");
            list3.Items.Add(">");
            list3.Items.Add("<");
            list3.Items.Add("between");
            modifyIconConditionInsidePopupPanel.Controls.Add(list3);

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">\n"));

            TextBox textBox1 = new TextBox();
            textBox1.ID = "modifyIconConditionTextBox1" + args;
            textBox1.MaxLength = 30;
            textBox1.CssClass = "inputBox";
            textBox1.Width = 150;

            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("</td>\n"));
            modifyIconConditionInsidePopupPanel.Controls.Add(new LiteralControl("<td class=\"textCenter\">\n"));
            Button addConditionButton = new Button();
            addConditionButton.ID = "addIconConditionButton" + args;
            addConditionButton.Text = "Add";
            addConditionButton.ToolTip = "Add Condition";
            addConditionButton.Width = 80;
            addConditionButton.CssClass = "button";
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

        protected void btnSubmitClick(object sender, EventArgs e)
        {
            Boolean valid = false;
            if (fileUpEx.HasFile)
            {
                foreach (String type in validTypes)
                {
                    if (fileUpEx.PostedFile.ContentType.Equals(type))
                    {
                        valid = true;
                    }
                }
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
                    //errorPanel1.Text = "Current File type = " + fileUpEx.PostedFile.ContentType + " File type not appropriate (only jpg, gif, tiff, png, bmp accepted)";
                }
                else
                {
                    //errorPanel1.Text = "File dimensions to large (max 128 x 128)";
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
        protected void URLcorrectClick(object sender, EventArgs e)
        {
            String URL = URLtextBox.Text.Trim();
            Database DB = new Database();
            WebClient Client = new WebClient();
            String fileName = System.IO.Path.GetFileNameWithoutExtension(URL);
            String ext = System.IO.Path.GetExtension(URL);
            String suffix = GetRandomString();
            String tempName = tempSaveLoc + fileName + suffix + ext;
            Client.DownloadFile(URL, tempName);
            bool valid = false;
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
                    //errorPanel1.Text = "You linked to an invalid file type";
                }
                else
                {
                    fs.Close();
                    File.Delete(tempName);
                    //errorPanel1.Text = "The file you linked to was to large (max 128 x 128)";
                }
            }
            else
            {
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
                    //errorPanel1.Text = "You linked to an invalid file type";
                }
                else
                {
                    fs.Close();
                    File.Delete(tempName);
                    //errorPanel1.Text = "The file you linked to was to large (max 128 x 128)";
                }
            }
            fetchCheckBox.Checked = false;
            URLtextBox.Text = "";
        }
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }
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
        private bool ValidateFileDimensions(Stream input)
        {
            using (System.Drawing.Image myImage =
              System.Drawing.Image.FromStream(input))
            {
                return (myImage.Height <= height && myImage.Width <= width);
            }
        }
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
    }
}
