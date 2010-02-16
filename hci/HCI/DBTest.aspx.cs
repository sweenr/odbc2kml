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

            title.Text = "Query Results";
            title.CssClass = "dbTitleSpan";

            dt = db.executeQueryLocal(queryString.Text);

            resultsPanel.Controls.Add(title);
            resultsPanel.Visible = true;

            resultsPanel.Controls.Add(new LiteralControl("<div width=\"100%\" class=\"dbColumns\"><table border=1>"));
            resultsPanel.Controls.Add(new LiteralControl("<tr>"));
            foreach (DataColumn dc in dt.Columns)
            {
                resultsPanel.Controls.Add(new LiteralControl("<td><b>" + dc.ColumnName + "</b></td>"));
            }
            resultsPanel.Controls.Add(new LiteralControl("</tr>"));

            foreach (DataRow dr in dt.Rows)
            {
                resultsPanel.Controls.Add(new LiteralControl("<tr>"));
                foreach(Object data in dr.ItemArray)
                {
                    resultsPanel.Controls.Add(new LiteralControl("<td>" + data.ToString() + "</td>")); 
                }
                resultsPanel.Controls.Add(new LiteralControl("</tr>"));
            }
            resultsPanel.Controls.Add(new LiteralControl("</table></div>"));
            
        }
    }
}
