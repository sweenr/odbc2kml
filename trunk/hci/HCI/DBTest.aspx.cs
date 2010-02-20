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
            
            
            if (!IsPostBack)
            {
                Database db = new Database();
                DataTable dt;

                dt = db.executeQueryLocal("SELECT id,name FROM CONNECTION");
                connectionSelector.DataSource = dt;
                connectionSelector.DataTextField = "name";
                connectionSelector.DataValueField = "id";
                connectionSelector.DataBind();
                connectionSelector.Items.Add("local");
            }

        }

        protected void executeQuery(object sender, EventArgs e)
        {
            try
            {
                Exception exception = new Exception();
                //throw exception;

                Database db;
                DataTable dt;
                Label title = new Label();


                if (connectionSelector.SelectedItem.Text == "local")
                {
                    db = new Database();

                    dt = db.executeQueryLocal(queryString.Text);
                }
                else
                {
                    ConnInfo info = new ConnInfo();

                    db = new Database();

                    string query = "SELECT * FROM Connection WHERE ID=" + connectionSelector.SelectedItem.Value;
                    dt = db.executeQueryLocal(query);

                    if (dt.HasErrors)
                    {
                        throw exception;
                    }

                    //Cycle through each row and column
                    foreach (DataRow row in dt.Rows)
                    {

                        foreach (DataColumn col in dt.Columns)
                        {
                            //Set all connInfo
                            switch (col.ColumnName)
                            {
                                case "name":
                                    info.setConnectionName(row[col].ToString());
                                    break;
                                case "dbName":
                                    info.setDatabaseName(row[col].ToString());
                                    break;
                                case "userName":
                                    info.setUserName(row[col].ToString());
                                    break;
                                case "password":
                                    info.setPassword(row[col].ToString());
                                    break;
                                case "port":
                                    info.setPortNumber(row[col].ToString());
                                    break;
                                case "address":
                                    info.setServerAddress(row[col].ToString());
                                    break;
                                case "type":
                                    info.setDatabaseType(int.Parse(row[col].ToString()));
                                    break;
                                case "protocol":
                                    info.setOracleProtocol(row[col].ToString());
                                    break;
                                case "serviceName":
                                    info.setOracleServiceName(row[col].ToString());
                                    break;
                                case "SID":
                                    info.setOracleSID(row[col].ToString());
                                    break;
                                default:
                                    break;
                            }
                        }
                    }//End outer loop

                    db.setConnInfo(info);
                    dt = db.executeQueryRemote(queryString.Text);
                }


                resultsPanel.Visible = true;

                resultsPanel.Controls.Add(new LiteralControl("<span class=\"connectionStyle\">&nbsp;Database Query Results</span>"));
                resultsPanel.Controls.Add(new LiteralControl("<div class=\"mainBoxP\">"));
                
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
                    foreach (Object data in dr.ItemArray)
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

                ModalPopupExtender6.Show();
            }
            catch(Exception exception)
            {
                errorPanel1.Visible = true;
                errorPanel1.Controls.Add(new LiteralControl("<div style=\"color: black\"><p>"+exception.Message+"</p></div>"));
                errorPanel1.Controls.Add(new LiteralControl("<script type=\"text/javascript\">$(\"#errorPanel1\").dialog('open')</script>"));
            }
        }
    }
}
