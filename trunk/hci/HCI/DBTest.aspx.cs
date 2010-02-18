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
    public partial class DBTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void executeQuery(object sender, EventArgs e)
        {
            Database db = new Database();
            DataTable dt;
            Label title = new Label();

            dt = db.executeQueryLocal(queryString.Text);

            

            resultsPanel.Visible = true;

            resultsPanel.Controls.Add(new LiteralControl("<span class=\"connectionStyle\">&nbsp;Database Query Results</span>"));
            resultsPanel.Controls.Add(new LiteralControl("<div class=\"mainBoxP\">"));  
            //resultsPanel.Controls.Add(new LiteralControl("<div width=\"100%\" class=\"dbColumns\">"));
            resultsPanel.Controls.Add(new LiteralControl("<table cellpadding=\"5\" cellspacing=\"0\" class=\"mainBox2\">"));
            
            resultsPanel.Controls.Add(new LiteralControl("<tr><td>"));
            resultsPanel.Controls.Add(new LiteralControl("<div class=\"omainBox4\">"));
            resultsPanel.Controls.Add(new LiteralControl("<table class=\"omainBox5\" cellspacing=\"0\" cellpadding=\"0\">"));
            
            resultsPanel.Controls.Add(new LiteralControl("<tr>"));
            foreach (DataColumn dc in dt.Columns)
            {
                resultsPanel.Controls.Add(new LiteralControl("<td><b>" + dc.ColumnName + "<br/></b></td>"));
            }
            resultsPanel.Controls.Add(new LiteralControl("</tr><tr><td><br/></td></tr>"));

            foreach (DataRow dr in dt.Rows)
            {
                resultsPanel.Controls.Add(new LiteralControl("<tr>"));
                foreach(Object data in dr.ItemArray)
                {
                    resultsPanel.Controls.Add(new LiteralControl("<td>" + data.ToString() + "</td>")); 
                }
                resultsPanel.Controls.Add(new LiteralControl("</tr>"));
            }
            resultsPanel.Controls.Add(new LiteralControl("</table>"));
            resultsPanel.Controls.Add(new LiteralControl("<div align=\"right\" style=\"padding-top: 20px;\">"));
            resultsPanel.Controls.Add(new LiteralControl("<input type=\"submit\" ID=\"hideResults\" value=\"Hide Results\" class=\"button\" />"));
            resultsPanel.Controls.Add(new LiteralControl("</div>"));
            resultsPanel.Controls.Add(new LiteralControl("</td></tr>"));
            resultsPanel.Controls.Add(new LiteralControl("</div>"));
            resultsPanel.Controls.Add(new LiteralControl("</span>"));
            
        }
    }
}
