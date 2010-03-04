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
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using HCI;

namespace HCI
{
    public partial class editor : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                //Grab and parse connection ID
                int conID = 1;

                //Create ConnInfo object and populate elements
                ConnInfo connInfo = ConnInfo.getConnInfo(conID);

                string connectionString = "";
                string providerName = "";
                //Set drop down box accordingly
                if (connInfo.getDatabaseType() == ConnInfo.MSSQL)
                {
                    odbcDBType.SelectedValue = "SQL";
                    odbcDBType.Visible = false;
                }
                else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)
                {
                    odbcDBType.SelectedValue = "MySQL";
                    connectionString = "server=" + connInfo.getServerAddress() + ";User Id=" + connInfo.getUserName() + ";password=" + connInfo.getPassword() + ";Persist Security Info=True;database=" + connInfo.getDatabaseName();
                    providerName = "MySql.Data.MySqlClient";
                    SQLTables.ConnectionString = connectionString;
                    SQLTables.ProviderName = providerName;
                    SQLTables.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA != 'mysql'";
                    iTableNBox.DataSource = SQLTables;
                    iTableNBox.DataTextField = "TABLE_NAME";
                    iTableNBox.DataValueField = "TABLE_NAME";
                    iTableNBox.DataBind();
                    iTableFNBox.DataSource = SQLTables;
                    iTableFNBox.DataTextField = "TABLE_NAME";
                    iTableFNBox.DataValueField = "TABLE_NAME";
                    iTableFNBox.DataBind();
                    iTableINBox.DataSource = SQLTables;
                    iTableINBox.DataTextField = "TABLE_NAME";
                    iTableINBox.DataValueField = "TABLE_NAME";
                    iTableINBox.DataBind();
                        
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
        }

        

        protected void dLink_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = true;
            dTablePanel.Visible = false;
            dFieldPanel.Visible = false;
            dImagePanel.Visible = false;
        }
        protected void dLinkInsert_Click(object sender, EventArgs e)
        {
            string linkText = iLinkNBox.Text.ToString();
            string linkURL = iLinkURLBox.Text.ToString();
            string descriptionInfo = "[URL][TITLE]" + linkText + "[/TITLE]" + linkURL + "[/URL]";
        

            if (linkText.Equals("") || linkURL.Equals(""))
            {
                iLinkError.Visible = true;
            }
            else
            {
                iLinkError.Visible = false;
                descriptionBox.Text += descriptionInfo;
            }
        }
        protected void dTable_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = false;
            dTablePanel.Visible = true;
            dFieldPanel.Visible = false;
            dImagePanel.Visible = false;
        }

        protected void dTableInsert_Click(object sender, EventArgs e)
        {
            string tableText = iTableNBox.SelectedValue.ToString();
            string descriptionInfo = "[TABLE]" + tableText + "[/TABLE]";

            descriptionBox.Text += descriptionInfo;
            
        }

        protected void dField_Click(object sender, EventArgs e)
        {
           
            dLinkPanel.Visible = false;
            dTablePanel.Visible = false;
            dFieldPanel.Visible = true;
            dImagePanel.Visible = false;

        }

        protected void dFieldInsert_Click(object sender, EventArgs e)
        {
            string tableText = iTableFNBox.SelectedValue.ToString();
            string colText = iColFNBox.SelectedValue.ToString();
          
            string descriptionInfo = "[FIELD][TBL]" + tableText + "[/TBL][COL]" + colText + "[/COL][/FIELD]";


            if (tableText.Equals("") || colText.Equals(""))
            {
                dFieldError.Visible = true;
            }
            else
            {
                dFieldError.Visible = false;
                descriptionBox.Text += descriptionInfo;
            }
        }

        protected void dImage_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = false;
            dTablePanel.Visible = false;
            dFieldPanel.Visible = false;
            dImagePanel.Visible = true;
        }

        protected void dImageInsert_Click(object sender, EventArgs e)
        {
            string tableText = iTableINBox.SelectedValue.ToString();
            string colText = iColINBox.SelectedValue.ToString();

            string descriptionInfo = "[IMG][TBL]" + tableText + "[/TBL][COL]" + colText + "[/COL][/IMG]";


            if (tableText.Equals("") || colText.Equals(""))
            {
                dImageError.Visible = true;
            }
            else
            {
                dImageError.Visible = false;
                descriptionBox.Text += descriptionInfo;
            }
        }

        protected void iTableFNBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = iTableFNBox.SelectedValue.ToString();
            if (selectedTable != "")
            {
                SqlDataSource temp = new SqlDataSource();
                temp.ConnectionString = SQLTables.ConnectionString;
                temp.ProviderName = SQLTables.ProviderName;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                iColFNBox.DataSource = temp;
                iColFNBox.DataValueField = "COLUMN_NAME";
                iColFNBox.DataTextField = "COLUMN_NAME";
                iColFNBox.DataBind();
                UpdateFieldCol.Update();
            }
            else
            {
                iColFNBox.Items.Clear();
                UpdateFieldCol.Update();
            }
        }

        protected void iTableINBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = iTableINBox.SelectedValue.ToString();
            if (selectedTable != "")
            {
                SqlDataSource temp = new SqlDataSource();
                temp.ConnectionString = SQLTables.ConnectionString;
                temp.ProviderName = SQLTables.ProviderName;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                iColFNBox.DataSource = temp;
                iColINBox.DataValueField = "COLUMN_NAME";
                iColINBox.DataTextField = "COLUMN_NAME";
                iColINBox.DataBind();
                UpdateImageCol.Update();

            }
            else
            {
                iColINBox.Items.Clear();
                UpdateImageCol.Update();
            }
        }

        protected void GridViewTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            columnButtons.Visible = true;
            columnMessage.Visible = false;
            string selectedTable = GridViewTables.SelectedValue.ToString();
            selectedGVTable.Value = selectedTable;
            SQLColGen.ConnectionString = SQLTables.ConnectionString;
            SQLColGen.ProviderName = SQLTables.ProviderName;
            SQLColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
            
        }

        protected void GridViewColumns_PageIndexChanged(object sender, EventArgs e)
        {
            SQLColGen.ConnectionString = SQLTables.ConnectionString;
            SQLColGen.ProviderName = SQLTables.ProviderName;
            SQLColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
        }

        protected void viewTable_Click(object sender, EventArgs e)
        {
            
        }
        
    }
}


