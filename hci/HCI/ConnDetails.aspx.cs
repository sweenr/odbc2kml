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
                }
                else
                {
                    //Grab and parse connection ID
                    int conID = int.Parse(Request.QueryString.Get("ConnID"));
                    
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

                ConnSMgr2.Controls.Add(new LiteralControl("<script type='text/JavaScript'>$('#odbcDBType').change("
               + "function()"
               + "{ if($('#odbcDBType').val() == 'Oracle') { $('#oracleTable').css('display', 'block'); }"
               + "else { $('#oracleTable').css('display', 'none');}"
               + "})</script>"));
            }
        }

       /* protected void oracleFields(object sender, EventArgs e)
        {      
        
            if(odbcDBType.SelectedValue == "Oracle")
            {
                ConnSMgr2.Controls.Add(new LiteralControl("<script type='text/JavaScript'>$('#odbcDBType').change("
                + "function()"
                + "{ if($('#odbcDBType').val() == 'Oracle') { $('#oracleTable').css('display', 'block'); }"
                + "else { $('#oracleTable').css('display', 'none');}" 
                + "})</script>"));
            }
            else 
            {
                //$("#oracleTable").css("display", "none");
            }
     
        }*/

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

        protected void genIconCondition(object sender, EventArgs e)
        {

            /*
            ConPanel.Controls.Add(new LiteralControl("<span class=\"connectionStyle\">&nbsp;Modify Condition</span>"));
            ConPanel.Controls.Add(new LiteralControl("<div class=\"mainBoxP\">"));
            ConPanel.Controls.Add(new LiteralControl("<table cellspacing=\"0\" cellpadding=\"5\" class=\"mainBox2\">"));
            ConPanel.Controls.Add(new LiteralControl("<tr>"));
            ConPanel.Controls.Add(new LiteralControl("<td>"));
            ConPanel.Controls.Add(new LiteralControl("<div class=\"omainBox4\">"));
            ConPanel.Controls.Add(new LiteralControl("<table class=\"omainBox6\" cellspacing=\"0\" cellpadding=\"0\">"));
            ConPanel.Controls.Add(new LiteralControl("<tr>"));
            ConPanel.Controls.Add(new LiteralControl("<td>"));
            ConPanel.Controls.Add(new LiteralControl("Tool Directions Go Here! Yay User Friendliness! :)"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("</tr>"));
            ConPanel.Controls.Add(new LiteralControl("</table>"));
            ConPanel.Controls.Add(new LiteralControl("<p>"));
            ConPanel.Controls.Add(new LiteralControl("</p>"));
            ConPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">"));
            ConPanel.Controls.Add(new LiteralControl("<tr class=\"tableTRTitle\">"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("Table"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("Field"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("Operator"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("Value"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td>"));
            ConPanel.Controls.Add(new LiteralControl("&nbsp;"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("</tr>"));
            ConPanel.Controls.Add(new LiteralControl("<tr>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("TheOnlyTable"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("VehicleType"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("=="));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("Tank"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"textCenter\">"));
            ConPanel.Controls.Add(new LiteralControl("<asp:Button ID=\"DeleteCon1\" runat=\"server\" Style=\"text-align: center\" Text=\"Remove\" "));
            ConPanel.Controls.Add(new LiteralControl("CssClass=\"button\" ToolTip=\"Delete Condition\" Width=\"80\" />"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("</tr>"));
            ConPanel.Controls.Add(new LiteralControl("<tr class=\"tableTR\">"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("<asp:DropDownList ID=\"DropDownList7\" runat=\"server\" CssClass=\"inputDD\" Width=\"100\">"));
            ConPanel.Controls.Add(new LiteralControl("</asp:DropDownList>"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("<asp:DropDownList ID=\"DropDownList8\" runat=\"server\" CssClass=\"inputDD\" Width=\"100\">"));
            ConPanel.Controls.Add(new LiteralControl("</asp:DropDownList>"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("<asp:DropDownList ID=\"DropDownList9\" runat=\"server\" CssClass=\"inputDD\" Width=\"100\">"));
            ConPanel.Controls.Add(new LiteralControl("<asp:ListItem>==</asp:ListItem>"));
            ConPanel.Controls.Add(new LiteralControl("<asp:ListItem>&gt;=</asp:ListItem>"));
            ConPanel.Controls.Add(new LiteralControl("<asp:ListItem>&lt;=</asp:ListItem>"));
            ConPanel.Controls.Add(new LiteralControl("<asp:ListItem>&gt;</asp:ListItem>"));
            ConPanel.Controls.Add(new LiteralControl("<asp:ListItem>&lt;</asp:ListItem>"));
            ConPanel.Controls.Add(new LiteralControl("<asp:ListItem>between</asp:ListItem>"));
            ConPanel.Controls.Add(new LiteralControl("</asp:DropDownList>"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"tableTD\">"));
            ConPanel.Controls.Add(new LiteralControl("<asp:TextBox ID=\"TextBox7\" runat=\"server\" MaxLength=\"30\" CssClass=\"inputBox\" Width=\"150\"></asp:TextBox>"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("<td class=\"textCenter\">"));
            ConPanel.Controls.Add(new LiteralControl("<asp:Button ID=\"AddCond1\" runat=\"server\" Style=\"text-align: center\" Text=\"Add\" CssClass=\"button\""));
            ConPanel.Controls.Add(new LiteralControl("ToolTip=\"Add Condition\" Width=\"80\" />"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("</tr>"));
            ConPanel.Controls.Add(new LiteralControl("</table>"));
            ConPanel.Controls.Add(new LiteralControl("<div class=\"right\" style=\"padding-top: 20px;\">"));
            ConPanel.Controls.Add(new LiteralControl("<asp:Button ID=\"modCondOK1\" runat=\"server\" Text=\"Submit\" CssClass=\"button\" />&nbsp;&nbsp;"));
            ConPanel.Controls.Add(new LiteralControl("<asp:Button ID=\"modCondCancel1\" runat=\"server\" Text=\"Cancel\" CssClass=\"button\" />&nbsp;&nbsp;"));
            ConPanel.Controls.Add(new LiteralControl("</div>"));
            ConPanel.Controls.Add(new LiteralControl("</div>"));
            ConPanel.Controls.Add(new LiteralControl("</td>"));
            ConPanel.Controls.Add(new LiteralControl("</tr>"));
            ConPanel.Controls.Add(new LiteralControl("</table>"));
            ConPanel.Controls.Add(new LiteralControl("</div>"));
            */
            
            
            ConPanel.Visible = true;
        }
    }
}
