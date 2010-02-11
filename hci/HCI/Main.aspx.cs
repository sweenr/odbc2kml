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
            //htmlContent.Text = "";
            string odbcName = "";

            //Get the number of DB tables from the database
            int numDBRows = 2;
            
            for (int i = 0; i < numDBRows; i++)
            {
                odbcName = Convert.ToString(i) ;
                
                //Defines buttons

                ImageButton openConn = new ImageButton();
                openConn.CssClass = "openIcon";
                openConn.ImageUrl = "graphics/connIcon.gif";
                openConn.AlternateText = "Open Connection";
                openConn.ToolTip = "Open Connection";
                openConn.PostBackUrl = "ConnDetails.aspx?ConnID=" + Convert.ToString(i);

                ImageButton editConn = new ImageButton();
                editConn.CssClass = "editIcon";
                editConn.ImageUrl = "graphics/connIcon.gif";
                editConn.AlternateText = "Edit Connection";
                editConn.ToolTip = "Edit Connection";
                editConn.PostBackUrl = "Modify.aspx?ConnID=" + Convert.ToString(i);

                ImageButton deleteConn = new ImageButton();
                deleteConn.CssClass = "deleteIcon";
                deleteConn.ImageUrl = "graphics/connIcon.gif";
                deleteConn.AlternateText = "Delete Connection";
                deleteConn.ToolTip = "Delete Connection";
                deleteConn.OnClientClick = "deleteConnFunction";
                deleteConn.CommandArgument = odbcName;

                ImageButton genKML = new ImageButton();
                genKML.CssClass = "kmlIcon";
                genKML.ImageUrl = "graphics/connIcon.gif";
                genKML.AlternateText = "Generate KML File";
                genKML.ToolTip = "Generate KML File";
                genKML.OnClientClick = "genKMLFunction";
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
                //ConnectionsAvailable.Controls.Add(new ControlBuilder("<asp:ImageButton ID=\"deleteConn1\" runat=\"server\" CssClass=\"deleteIcon\" ImageUrl=\"graphics/connIcon.gif\" AlternateText=\"Delete Connection\" ToolTip=\"Delete Connection\" OnClick=\"deleteConnFunction\" CommandArgument=\"none\" />"));
                ConnectionsAvailable.Controls.Add(deleteConn);
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
            Response.Redirect("www.google.com", true);
            //Delete the connection
            ImageButton sendBtn = (ImageButton)sender;
            string args = sendBtn.CommandArgument.ToString();
            Conn1.Visible = true;

            Response.Redirect("~/Modify.aspx", true);
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

            String ConnName = odbcNameE.ToString();
            String ConnDBName = odbcDNameE.ToString();
            String ConnDBAddress = odbcDatabaseE.ToString();
            String ConnPortNum = odbcPNE.ToString();
            String ConnUser = odbcUserE.ToString();
            String ConnPWD = odbcPWE.ToString();
            String ConnDBType = DropDownList4.SelectedItem.ToString();

            if (ConnName.Equals(""))
            {
                
                return;
            }
            if (ConnDBName.Equals(""))
            {
                
                return;
            }
            if (ConnDBAddress.Equals(""))
            {
                
                return;
            }
            if (ConnPortNum.Equals(""))
            {
                
                return;
            }
            if (ConnUser.Equals(""))
            {
                
                return;
            }
            if (ConnPWD.Equals(""))
            {
                
                return;
            }

            //Call Create DB with the DB Function
            String connID = "";

            //Jump to the Modify page
            Response.Redirect("Modify.aspx?ConnID=" + connID, true);
        }
    }
}
