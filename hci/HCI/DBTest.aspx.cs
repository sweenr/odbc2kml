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
        /// <summary>
        /// The following code is taken from http://www.codeproject.com/KB/aspnet/Enable_Disable_Controls.aspx
        /// which is distributed under the CPOL license
        /// </summary>
        /// <param name="status"></param>
        private void ChangeControlStatus(bool status)
        {

            foreach (Control c in Page.Controls)
                foreach (Control ctrl in c.Controls)

                    if (ctrl is TextBox)

                        ((TextBox)ctrl).Enabled = status;

                    else if (ctrl is Button)

                        ((Button)ctrl).Enabled = status;

                    else if (ctrl is RadioButton)

                        ((RadioButton)ctrl).Enabled = status;

                    else if (ctrl is ImageButton)

                        ((ImageButton)ctrl).Enabled = status;

                    else if (ctrl is CheckBox)

                        ((CheckBox)ctrl).Enabled = status;

                    else if (ctrl is DropDownList)

                        ((DropDownList)ctrl).Enabled = status;

                    else if (ctrl is HyperLink)

                        ((HyperLink)ctrl).Enabled = status; 

        }

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
            if (Request.QueryString.Get("locked") == "1")
            {
                ChangeControlStatus(false);
            }

        }

        protected void executeQuery(object sender, EventArgs e)
        {
            //try
            //{
                Database db;
                DataTable dt;
                Label title = new Label();

                if (connectionSelector.SelectedItem.Text == "local")
                {
                    try
                    {
                        db = new Database();

                        dt = db.executeQueryLocal(queryString.Text);
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }
                }
                else
                {
                    ConnInfo info = new ConnInfo();

                    try
                    {
                        db = new Database();

                        string query = "SELECT * FROM Connection WHERE ID=" + connectionSelector.SelectedItem.Value;
                        dt = db.executeQueryLocal(query);
                        if (dt.HasErrors)
                        {
                            throw new ODBC2KMLException("Unknown Database error");
                        }
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }

                    info = ConnInfo.getConnInfo(int.Parse(connectionSelector.SelectedItem.Value));

                    db.setConnInfo(info);
                    try
                    {
                        dt = db.executeQueryRemote(queryString.Text);
                    }
                    catch (ODBC2KMLException ex)
                    {
                        ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                        eh.displayError();
                        return;
                    }
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
            //}
            /*catch(Exception exception)
            {
                errorPanel1.Visible = true;
                errorPanel1.Controls.Add(new LiteralControl("<div style=\"color: black\"><p>"+exception.Message+"</p></div>"));
                errorPanel1.Controls.Add(new LiteralControl("<script type=\"text/javascript\">$(\"#errorPanel1\").dialog('open')</script>"));
            }*/
        }
    }
}
