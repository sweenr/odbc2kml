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
            //Database db = new Database();
            //DataTable dt;
            //dt = db.executeQueryLocal("SELECT ID,name FROM Connection");

            string odbcName = "";

            //Get the number of DB tables from the database
            int numDBRows = 2;
            int dbID;
            
            for (int i = 0; i < numDBRows; i++)
            {
                odbcName = Convert.ToString(i) ;
                
                //Defines buttons

                ImageButton openConn = new ImageButton();
                openConn.CssClass = "openIcon";
                openConn.ImageUrl = "graphics/connIcon.gif";
                openConn.AlternateText = "Open Connection";
                openConn.ToolTip = "Open Connection";
                openConn.PostBackUrl = "ConnDetails.aspx?ConnID=" + odbcName;

                ImageButton editConn = new ImageButton();
                editConn.CssClass = "editIcon";
                editConn.ImageUrl = "graphics/connIcon.gif";
                editConn.AlternateText = "Edit Connection";
                editConn.ToolTip = "Edit Connection";
                editConn.PostBackUrl = "Modify.aspx?ConnID=" + odbcName;

                ImageButton deleteConn = new ImageButton();
                deleteConn.ID = "dc" + Convert.ToString(i);
                deleteConn.CssClass = "deleteIcon";
                deleteConn.ImageUrl = "graphics/connIcon.gif";
                deleteConn.AlternateText = "Delete Connection";
                deleteConn.ToolTip = "Delete Connection";
                deleteConn.Click += new ImageClickEventHandler(confirmDelete);
                deleteConn.CommandArgument = odbcName;

                ImageButton genKML = new ImageButton();
                genKML.CssClass = "kmlIcon";
                genKML.ImageUrl = "graphics/connIcon.gif";
                genKML.AlternateText = "Generate KML File";
                genKML.ToolTip = "Generate KML File";
                genKML.Click += new ImageClickEventHandler(genKMLFunction);
                genKML.CommandArgument = odbcName;


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
                ConnectionsAvailable.Controls.Add(new LiteralControl("<span id=\"Conn" + odbcName + "\">ODBC Connection " + odbcName + "</span>\n"));
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

            }
        }

        protected void deleteConnFunction(object sender, EventArgs e)
        {
            //Delete the connection
            Button sendBtn = (Button)sender;
            String args = sendBtn.CommandArgument.ToString();
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

            if (ConnName.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                //return;
            }else if (ConnDBName.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                //return;
            }else if (ConnDBAddress.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                //return;
            }else if (ConnPortNum.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                //return;
            }else if(ConnUser.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                //return;
            }else if (ConnPWD.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                //return;
            }

            //Call Create DB with the DB Function
            String connID = "";
            

            //Jump to the Modify page
            //Response.Redirect("Modify.aspx?ConnID=" + connID, true);
        }
    }
}
