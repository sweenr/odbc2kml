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
using System.Text;

namespace HCI
{
    public partial class Main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Get the DB stuff from here
            Database db = new Database();
            DataTable dt;
            dt = db.executeQueryLocal("SELECT id,name FROM CONNECTION");
            int i = 0;

            foreach (DataRow dr in dt.Rows)
            {
                string dbID = dr.ItemArray.ElementAt(0).ToString();
                string odbcName = dr.ItemArray.ElementAt(1).ToString();
                
                //Defines buttons
                ImageButton openConn = new ImageButton();
                openConn.CssClass = "openIcon";
                openConn.ImageUrl = "graphics/connIcon.gif";
                openConn.AlternateText = "Open Connection";
                openConn.ToolTip = "Open Connection";
                openConn.PostBackUrl = "ConnDetails.aspx?ConnID=" + dbID;

                ImageButton editConn = new ImageButton();
                editConn.CssClass = "editIcon";
                editConn.ImageUrl = "graphics/connIcon.gif";
                editConn.AlternateText = "Edit Connection";
                editConn.ToolTip = "Edit Connection";
                editConn.PostBackUrl = "ConnDetails.aspx?ConnID=" + dbID;

                ImageButton deleteConn = new ImageButton();
                deleteConn.ID = "dc" + Convert.ToString(i);
                deleteConn.CssClass = "deleteIcon";
                deleteConn.ImageUrl = "graphics/connIcon.gif";
                deleteConn.AlternateText = "Delete Connection";
                deleteConn.ToolTip = "Delete Connection";
                deleteConn.Click += new ImageClickEventHandler(confirmDelete);
                deleteConn.CommandArgument = dbID;

                ImageButton genKML = new ImageButton();
                genKML.CssClass = "kmlIcon";
                genKML.ImageUrl = "graphics/connIcon.gif";
                genKML.AlternateText = "Generate KML File";
                genKML.ToolTip = "Generate KML File";
                genKML.Click += new ImageClickEventHandler(genKMLFunction);
                genKML.CommandArgument = dbID;


                //End button definition
                if (i % 2.00 == 0)
                {
                    ConnectionsAvailable.Controls.Add(new LiteralControl("<tr class=\"oddConn\">\n"));
                }
                else
                {
                    ConnectionsAvailable.Controls.Add(new LiteralControl("<tr class=\"evenConn\">\n"));
                }

                ConnectionsAvailable.Controls.Add(new LiteralControl("<td>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<span id=\"Conn" + dbID + "\">" + odbcName + "</span>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<a href=\"#\" title=\"Open Connection\"></a>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("</td>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<td class=\"connIcons\">\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<table>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<tr>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<td>\n"));
                ConnectionsAvailable.Controls.Add(openConn);
                ConnectionsAvailable.Controls.Add(new LiteralControl("</td>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<td>\n"));
                ConnectionsAvailable.Controls.Add(editConn);
                ConnectionsAvailable.Controls.Add(new LiteralControl("</td>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<td>\n"));
                ConnectionsAvailable.Controls.Add(deleteConn);
                ConnectionsAvailable.Controls.Add(new LiteralControl("</td>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<td>\n"));
                ConnectionsAvailable.Controls.Add(genKML);
                ConnectionsAvailable.Controls.Add(new LiteralControl("</td>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("</tr>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("</table>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("</td>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("</tr>\n"));

                i += 1;
            }
        }

        protected void deleteConnFunction(object sender, EventArgs e)
        {
            //Delete the connection
            Button sendBtn = (Button)sender;
            String args = sendBtn.CommandArgument.ToString();
            Database db = new Database();
            db.executeQueryLocal("DELETE FROM CONNECTION WHERE ID="+args);
            this.deletePopupExtender.Hide();
            Response.Redirect("Main.aspx");
            
        }

        protected void confirmDelete(object sender, EventArgs e)
        {
            ImageButton sendBtn = (ImageButton)sender;
            String args = sendBtn.CommandArgument.ToString();
            delConnBtn.Click += new EventHandler(deleteConnFunction);
            delConnBtn.CommandArgument = args[0].ToString();

            this.deletePopupExtender.Show();

        }

        protected void genKMLFunction(object sender, EventArgs e)
        {
            //Generate the KML from the connection
            ImageButton sendBtn = (ImageButton)sender;
            string args = sendBtn.CommandArgument.ToString();

            Response.Redirect("Main.aspx", true);
        }

        protected void createConnection(object sender, EventArgs e)
        {
            //this.NewConn1ModalPopUp.Hide();

            String ConnName = odbcNameE.Text.ToString();
            String ConnDBName = odbcDNameE.Text.ToString();
            String ConnDBAddress = odbcDatabaseE.Text.ToString();
            String ConnPortNum = odbcPNE.Text.ToString();
            String ConnUser = odbcUserE.Text.ToString();
            String ConnPWD = odbcPWE.Text.ToString();
            String ConnDBType = DropDownList4.SelectedItem.ToString();
            String DBTypeNum;

            if (ConnDBType.Equals("MySQL")){
                DBTypeNum = "0";
            }else if(ConnDBType.Equals("MSSQL")){
                DBTypeNum = "1";
            }else{
                DBTypeNum = "2";
            }

            if (ConnName.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }else if (ConnDBName.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }else if (ConnDBAddress.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }else if (ConnPortNum.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }else if(ConnUser.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }else if (ConnPWD.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }

            //Call Create DB with the DB Function
            Database db = new Database();
            DataTable dt;
            db.executeQueryLocal("INSERT INTO Connection (name, dbName, userName, password, port, address, type, protocol, serviceName, SID) VALUES ('" + ConnName + "', '" + ConnDBName + "', '" + ConnUser + "', '" + ConnPWD + "', '" + ConnPortNum + "', '" + ConnDBAddress + "', '" + DBTypeNum + "', '', '', '')");

            this.NewConn1ModalPopUp.Hide();
            //Jump to the Modify page
            
            dt = db.executeQueryLocal("SELECT ID FROM CONNECTION WHERE name='"+ConnName+"' AND dbName='"+ConnDBName+"' AND userName='"+ConnUser+"' AND port='"+ConnPortNum+"' AND address='"+ConnDBAddress+"' AND type='"+DBTypeNum+"'");
            foreach (DataRow dr in dt.Rows)
            {
                string connID = dr.ItemArray.ElementAt(0).ToString();

                Response.Redirect("ConnDetails.aspx?ConnID=" + connID);
            }
            //Response.Redirect("Modify.aspx?ConnID=" + connID, true);
        }
    }
}
