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
        int conID;
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
                        odbcDBType.Visible = false;
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

                genIconConditionTable(sender, e);
            }
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

        protected void genIconCondition(object sender, EventArgs e)
        {

            
            this.ModalPopupExtender7.Show();
            //ConPanel.Controls.Add(new LiteralControl("BLOB!!!"));
            //ConPanel.Visible = true;
        }


        protected void genIconConditionTable(object sender, EventArgs e)
        {
            Database db = new Database();
            DataTable dt;
            dt = db.executeQueryLocal("SELECT iconID, fieldName, tableName, lowerBound, upperBound, lowerOperator, upperOperator FROM IconCondition WHERE connID = " + conID);
            IconConditionTable1.Controls.Add(new LiteralControl("<tr>\n"));
            IconConditionTable1.Controls.Add(new LiteralControl("<td>\n"));
            foreach (DataRow dr in dt.Rows)
            {
                string iconId = dr.ItemArray.ElementAt(0).ToString();
                Condition condition = new Condition(dr.ItemArray.ElementAt(1).ToString(), dr.ItemArray.ElementAt(2).ToString(),
                    dr.ItemArray.ElementAt(3).ToString(), dr.ItemArray.ElementAt(4).ToString(),
                    dr.ItemArray.ElementAt(5).ToString(), dr.ItemArray.ElementAt(6).ToString());
                if (condition.getLowerOperator() != HCI.Condition.NONE.ToString())
                    IconConditionTable1.Controls.Add(new LiteralControl(condition.getLowerBound() + " " + condition.getLowerOperator() + " "));
                IconConditionTable1.Controls.Add(new LiteralControl(condition.getTableName() + "." + condition.getFieldName() + " "));
                if (condition.getUpperOperator() != HCI.Condition.NONE.ToString())
                    IconConditionTable1.Controls.Add(new LiteralControl(condition.getUpperBound() + " " + condition.getUpperOperator() + " "));
                IconConditionTable1.Controls.Add(new LiteralControl("<br />\n"));
            }
            IconConditionTable1.Controls.Add(new LiteralControl("</td>\n"));
            IconConditionTable1.Controls.Add(new LiteralControl("</tr>\n"));
            
        }
    }
}
