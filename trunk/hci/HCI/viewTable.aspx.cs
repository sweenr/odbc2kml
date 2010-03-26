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
    public partial class viewTable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Database db = new Database();
            DataTable dt;
            Label title = new Label();
            string connID = Request["con"];
            string table = Request["tbl"];
            string tblQuery = "SELECT * FROM " + table;


            ConnInfo info = new ConnInfo();


            try
            {
                db = new Database();

                string query = "SELECT * FROM Connection WHERE ID=" + connID;
                dt = db.executeQueryLocal(query);
                if (dt.HasErrors)
                {
                    throw new ODBC2KMLException("Unknown Database error");
                }
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.conErrorText, errorPanel1);
                eh.displayError();
                return;
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
            try
            {
                dt = db.executeQueryRemote(tblQuery);
            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.tblErrorText, errorPanel1);
                eh.displayError();
                return;
            }



            //resultsPanel.Visible = true;


            Page.Controls.Add(new LiteralControl("<span>&nbsp;Database Query Results</span>"));
            Page.Controls.Add(new LiteralControl("<div>"));

            Page.Controls.Add(new LiteralControl("<table cellpadding=\"5\" cellspacing=\"2\" border=\"1\">"));

            Page.Controls.Add(new LiteralControl("<tr><td>"));
            Page.Controls.Add(new LiteralControl("<div>"));
            Page.Controls.Add(new LiteralControl("<table cellspacing=\"2\" cellpadding=\"2\" style=\"border:0px\">"));

            Page.Controls.Add(new LiteralControl("<tr>"));
            foreach (DataColumn dc in dt.Columns)
            {
                Page.Controls.Add(new LiteralControl("<td><b>" + dc.ColumnName + "<br/></b></td>"));
            }
            Page.Controls.Add(new LiteralControl("</tr><tr><td><br/></td></tr>"));
            foreach (DataRow dr in dt.Rows)
            {

                Page.Controls.Add(new LiteralControl("<tr>"));
                foreach (Object data in dr.ItemArray)
                {
                    Page.Controls.Add(new LiteralControl("<td>" + data.ToString() + "</td>"));
                }
                Page.Controls.Add(new LiteralControl("</tr>"));

            }

            Page.Controls.Add(new LiteralControl("</table>"));
            Page.Controls.Add(new LiteralControl("<div align=\"right\" style=\"padding-top: 20px;\">"));
            Page.Controls.Add(new LiteralControl("<input type=\"submit\" ID=\"hideResults\" value=\"Hide Results\" />"));
            Page.Controls.Add(new LiteralControl("</div>"));
            Page.Controls.Add(new LiteralControl("</td></tr>"));
            Page.Controls.Add(new LiteralControl("</div>"));
            Page.Controls.Add(new LiteralControl("</span>"));


        }

        protected void Page_Error(object sender, EventArgs e)
        {
            Session["Error"] = "Unknown Error";
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void executeQuery(object sender, EventArgs e)
        {

        }
    }
}

