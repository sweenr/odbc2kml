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
        //Get ConID from value passed
        //Right now just testing with numbers to make sure works
        int conID = 2;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Create ConnInfo object and populate elements
                ConnInfo connInfo = ConnInfo.getConnInfo(conID);

                string connectionString = "";
                string providerName = "";

                //Set Table Datasources & fill in gridview/boxes

                if (connInfo.getDatabaseType() == ConnInfo.MSSQL)
                {
                    connectionString = "Data Source=" + connInfo.getServerAddress() + ";Initial Catalog=" + connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword();
                    MSQLTables.ConnectionString = connectionString;
                    MSQLTables.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' AND TABLE_NAME != 'sysdiagrams'";    
                }

                else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)
                {
                    connectionString = "server=" + connInfo.getServerAddress() + ";User Id=" + connInfo.getUserName() + ";password=" + connInfo.getPassword() + ";Persist Security Info=True;database=" + connInfo.getDatabaseName();
                    providerName = "MySql.Data.MySqlClient";
                    SQLTables.ConnectionString = connectionString;
                    SQLTables.ProviderName = providerName;
                    SQLTables.SelectCommand = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA != 'information_schema' && TABLE_SCHEMA != 'mysql'";
                }

                else if (connInfo.getDatabaseType() == ConnInfo.ORACLE)
                {
                    connectionString = "Data Source=" + connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword() + ";Unicode=True";
                    providerName = "System.Data.OracleClient";
                    oracleTables.ConnectionString = connectionString;
                    oracleTables.ProviderName = providerName;
                    oracleTables.SelectCommand = "SELECT TABLE_NAME FROM all_tables WHERE TABLESPACE_NAME != 'SYSTEM' AND TABLESPACE_NAME != 'SYSAUX'";
                }

                else //Default set to SQL
                {
                }

                updateTables(connInfo.getDatabaseType());

                //Get Description
                Description conDesc = Description.getDescription(conID);
                string descBox = conDesc.getDesc();
                descriptionBox.Text = descBox;

                //Garbage collection
                connInfo = null;

            }
        }

        protected void updateTables(int type)
        {
            if (type == ConnInfo.MSSQL)
            {
                iTableNBox.DataSource = MSQLTables;
                iTableNBox.DataTextField = "TABLE_NAME";
                iTableNBox.DataValueField = "TABLE_NAME";
                iTableNBox.DataBind();
                iTableFNBox.DataSource = MSQLTables;
                iTableFNBox.DataTextField = "TABLE_NAME";
                iTableFNBox.DataValueField = "TABLE_NAME";
                iTableFNBox.DataBind();
                iTableINBox.DataSource = MSQLTables;
                iTableINBox.DataTextField = "TABLE_NAME";
                iTableINBox.DataValueField = "TABLE_NAME";
                iTableINBox.DataBind();
                GridViewTables.DataSource = MSQLTables;
                GridViewTables.DataBind();
            }
            else if (type == ConnInfo.MYSQL)
            {
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
                GridViewTables.DataSource = SQLTables;
                GridViewTables.DataBind();
            }
            else if (type == ConnInfo.ORACLE)
            {
                iTableNBox.DataSource = oracleTables;
                iTableNBox.DataTextField = "TABLE_NAME";
                iTableNBox.DataValueField = "TABLE_NAME";
                iTableNBox.DataBind();
                iTableFNBox.DataSource = oracleTables;
                iTableFNBox.DataTextField = "TABLE_NAME";
                iTableFNBox.DataValueField = "TABLE_NAME";
                iTableFNBox.DataBind();
                iTableINBox.DataSource = oracleTables;
                iTableINBox.DataTextField = "TABLE_NAME";
                iTableINBox.DataValueField = "TABLE_NAME";
                iTableINBox.DataBind();
                GridViewTables.DataSource = oracleTables;
                GridViewTables.DataBind();
            }
            else
            {
                //Default case goes here
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
                ConnInfo connInfo = ConnInfo.getConnInfo(conID);

                string connectionString = "";
                string providerName = "";

                //Set drop down box accordingly
                if (connInfo.getDatabaseType() == ConnInfo.MSSQL)
                {
                    connectionString = "Data Source=" + connInfo.getServerAddress() + ";Initial Catalog=" + connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword();
                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                    iColFNBox.DataSource = temp;
                    iColFNBox.DataValueField = "COLUMN_NAME";
                    iColFNBox.DataTextField = "COLUMN_NAME";
                    iColFNBox.DataBind();
                    UpdateFieldCol.Update();
                }

                else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)
                {
                    connectionString = "server=" + connInfo.getServerAddress() + ";User Id=" + connInfo.getUserName() + ";password=" + connInfo.getPassword() + ";Persist Security Info=True;database=" + connInfo.getDatabaseName();
                    providerName = "MySql.Data.MySqlClient";
                   
                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString;
                    temp.ProviderName = providerName;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                    iColFNBox.DataSource = temp;
                    iColFNBox.DataValueField = "COLUMN_NAME";
                    iColFNBox.DataTextField = "COLUMN_NAME";
                    iColFNBox.DataBind();
                    UpdateFieldCol.Update();

                }

                else if (connInfo.getDatabaseType() == ConnInfo.ORACLE)
                {
                    connectionString = "Data Source=" + connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword() + ";Unicode=True";
                    providerName = "System.Data.OracleClient";

                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString;
                    temp.ProviderName = providerName;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedTable + "')";
                    iColFNBox.DataSource = temp;
                    iColFNBox.DataValueField = "COLUMN_NAME";
                    iColFNBox.DataTextField = "COLUMN_NAME";
                    iColFNBox.DataBind();
                    UpdateFieldCol.Update();
                }

                else //Default set to SQL
                {
                }

                //Garbage collection
                connInfo = null;
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
                ConnInfo connInfo = ConnInfo.getConnInfo(conID);

                string connectionString = "";
                string providerName = "";

                //Set drop down box accordingly
                if (connInfo.getDatabaseType() == ConnInfo.MSSQL)
                {
                    connectionString = "Data Source=" + connInfo.getServerAddress() + ";Initial Catalog=" + connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword();
                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                    iColINBox.DataSource = temp;
                    iColINBox.DataValueField = "COLUMN_NAME";
                    iColINBox.DataTextField = "COLUMN_NAME";
                    iColINBox.DataBind();
                    UpdateFieldCol.Update();
                }

                else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)
                {
                    connectionString = "server=" + connInfo.getServerAddress() + ";User Id=" + connInfo.getUserName() + ";password=" + connInfo.getPassword() + ";Persist Security Info=True;database=" + connInfo.getDatabaseName();
                    providerName = "MySql.Data.MySqlClient";

                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString;
                    temp.ProviderName = providerName;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                    iColINBox.DataSource = temp;
                    iColINBox.DataValueField = "COLUMN_NAME";
                    iColINBox.DataTextField = "COLUMN_NAME";
                    iColINBox.DataBind();
                    UpdateFieldCol.Update();

                }

                else if (connInfo.getDatabaseType() == ConnInfo.ORACLE)
                {
                    connectionString = "Data Source=" + connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword() + ";Unicode=True";
                    providerName = "System.Data.OracleClient";

                    SqlDataSource temp = new SqlDataSource();
                    temp.ConnectionString = connectionString;
                    temp.ProviderName = providerName;
                    temp.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedTable + "')";
                    iColINBox.DataSource = temp;
                    iColINBox.DataValueField = "COLUMN_NAME";
                    iColINBox.DataTextField = "COLUMN_NAME";
                    iColINBox.DataBind();
                    UpdateFieldCol.Update();
                }

                else //Default set to SQL
                {
                }

                //Garbage collection
                connInfo = null;
            }
            else
            {
                iColINBox.Items.Clear();
                UpdateFieldCol.Update();
            }
        }

        protected void GridViewTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            columnButtons.Visible = true;
            columnMessage.Visible = false;
            string selectedTable = GridViewTables.SelectedValue.ToString();
            selectedGVTable.Value = selectedTable;

            string pageInfo = "viewTable.aspx?con=" + conID + "&tbl=" + selectedTable;
            string window = "window.open('" + pageInfo  +"'); return false;";
            viewTable.Attributes.Add("onclick", window);

            if (selectedTable != "")
            {
                ConnInfo connInfo = ConnInfo.getConnInfo(conID);

                string connectionString = "";
                string providerName = "";

                //Set drop down box accordingly
                if (connInfo.getDatabaseType() == ConnInfo.MSSQL)
                {
                    connectionString = "Data Source=" + connInfo.getServerAddress() + ";Initial Catalog=" + connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword();
                    ColGen.ConnectionString = connectionString;
                    ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                    ColGen.DataBind();
                }

                else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)
                {
                    connectionString = "server=" + connInfo.getServerAddress() + ";User Id=" + connInfo.getUserName() + ";password=" + connInfo.getPassword() + ";Persist Security Info=True;database=" + connInfo.getDatabaseName();
                    providerName = "MySql.Data.MySqlClient";

                    ColGen.ConnectionString = connectionString;
                    ColGen.ProviderName = providerName;
                    ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                    ColGen.DataBind();

                }

                else if (connInfo.getDatabaseType() == ConnInfo.ORACLE)
                {
                    connectionString = "Data Source=" + connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword() + ";Unicode=True";
                    providerName = "System.Data.OracleClient";

                    ColGen.ConnectionString = connectionString;
                    ColGen.ProviderName = providerName;
                    ColGen.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedGVTable.Value + "')";
                    ColGen.DataBind();
                   
                }

                else //Default set to SQL
                {
                }


                Mapping conMapping = Mapping.getMapping2(conID, selectedTable);

                string dbTable = conMapping.getTableName();

                int dbFormat = conMapping.getFormat();

                string dbLat = conMapping.getLatFieldName();
                string dbLong = conMapping.getLongFieldName();

                //No information about the table yet
                if (dbTable == null)
                {
                    mapUpdates(ColGen);
                }

                else
                {
                    mapUpdates(ColGen, dbLat, dbLong, dbFormat);
                }

                //Garbage collection
                connInfo = null;
            }
            else
            {
                iColFNBox.Items.Clear();
                UpdateFieldCol.Update();
            }
            
        }

        protected void GridViewColumns_PageIndexChanged(object sender, EventArgs e)
        {
            ConnInfo connInfo = ConnInfo.getConnInfo(conID);

            string connectionString = "";
            string providerName = "";

            //Set drop down box accordingly
            if (connInfo.getDatabaseType() == ConnInfo.MSSQL)
            {
                connectionString = "Data Source=" + connInfo.getServerAddress() + ";Initial Catalog=" + connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword();
                ColGen.ConnectionString = connectionString;
                ColGen.ProviderName = providerName;
                ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                ColGen.DataBind();
            }

            else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)
            {
                connectionString = "server=" + connInfo.getServerAddress() + ";User Id=" + connInfo.getUserName() + ";password=" + connInfo.getPassword() + ";Persist Security Info=True;database=" + connInfo.getDatabaseName();
                providerName = "MySql.Data.MySqlClient";
                ColGen.ConnectionString = connectionString;
                ColGen.ProviderName = providerName;
                ColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
                ColGen.DataBind();

            }

            else if (connInfo.getDatabaseType() == ConnInfo.ORACLE)
            {
                connectionString = "Data Source=" + connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword() + ";Unicode=True";
                providerName = "System.Data.OracleClient";
                ColGen.ConnectionString = connectionString;
                ColGen.ProviderName = providerName;
                ColGen.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedGVTable.Value + "')";
                ColGen.DataBind();
            }

            else //Default set to SQL
            {
            }

            //Garbage collection
            connInfo = null;
        }

        protected void updateDescription(object sender, EventArgs e)
        {
            Description conDesc = Description.getDescription(conID);
            string descBox = conDesc.getDesc();

            string descText = descriptionBox.Text.ToString();

            //No description entry exists
            if (descBox == null)
            {
                Description.insertDescription(conID, descText);
                descSuccess.Text = "Description inserted successfully!";
            }

            //Update existing description entry
            else
            {
                Description.updateDescription(conID, descText);
                descSuccess.Text = "Description updated successfully!";
            }

        }

        protected void mapTogether_Click(object sender, EventArgs e)
        {
            LLSepPanel.Visible = false;
            LLTogetherPanel.Visible = true;
        }

        protected void mapSeparate_Click(object sender, EventArgs e)
        {
            LLTogetherPanel.Visible = false;
            LLSepPanel.Visible = true;
        }

        protected void mapUpdates(SqlDataSource temp)
        {
            latDD.DataSource = temp;
            latDD.DataValueField = "COLUMN_NAME";
            latDD.DataTextField = "COLUMN_NAME";
            latDD.DataBind();
            latUP.Update();
            longDD.DataSource = temp;
            longDD.DataValueField = "COLUMN_NAME";
            longDD.DataTextField = "COLUMN_NAME";
            longDD.DataBind();
            longUP.Update();
            llDD.DataSource = temp;
            llDD.DataValueField = "COLUMN_NAME";
            llDD.DataTextField = "COLUMN_NAME";
            llDD.DataBind();
            llUP.Update();

            LLSepPanel.Visible = true;
            LLTogetherPanel.Visible = false;

            mapError1.Visible = false;
            mapError2.Visible = false;
            mapSuccess.Visible = false;

        }

        protected void mapUpdates(SqlDataSource temp, string latitude, string longitude, int format)
        {
            latDD.DataSource = temp;
            latDD.DataValueField = "COLUMN_NAME";
            latDD.DataTextField = "COLUMN_NAME";
            latDD.DataBind();
            latDD.SelectedValue = latitude;
            latUP.Update();
            longDD.DataSource = temp;
            longDD.DataValueField = "COLUMN_NAME";
            longDD.DataTextField = "COLUMN_NAME";
            longDD.DataBind();
            longDD.SelectedValue = longitude;
            longUP.Update();
            llDD.DataSource = temp;
            llDD.DataValueField = "COLUMN_NAME";
            llDD.DataTextField = "COLUMN_NAME";
            llDD.DataBind();
            llDD.SelectedValue = latitude;
            llUP.Update();

            if (format == 1)
            {
                LLSepPanel.Visible = true;
                LLTogetherPanel.Visible = false;
            }
            else
            {
                if (format == 2)
                {
                    LatLongCheck.Checked = true;
                    LongLatCheck.Checked = false;
                }
                else
                {
                    LatLongCheck.Checked = false;
                    LongLatCheck.Checked = true;
                }

                LLSepPanel.Visible = false;
                LLTogetherPanel.Visible = true;
            }

            mapError1.Visible = false;
            mapError2.Visible = false;
            mapSuccess.Visible = false;

        }

        protected void addLatLong_Click(object sender, EventArgs e)
        {
            ConnInfo connInfo = ConnInfo.getConnInfo(conID);

            string selectedTable = GridViewTables.SelectedValue.ToString();
            string connectionString = "";
            string providerName = "";

            SqlDataSource temp = new SqlDataSource();

            //Set drop down box accordingly
            if (connInfo.getDatabaseType() == ConnInfo.MSSQL)
            {
                connectionString = "Data Source=" + connInfo.getServerAddress() + ";Initial Catalog=" + connInfo.getDatabaseName() + ";Persist Security Info=True;User Id=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword();
                temp.ConnectionString = connectionString;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                
            }

            else if (connInfo.getDatabaseType() == ConnInfo.MYSQL)
            {
                connectionString = "server=" + connInfo.getServerAddress() + ";User Id=" + connInfo.getUserName() + ";password=" + connInfo.getPassword() + ";Persist Security Info=True;database=" + connInfo.getDatabaseName();
                providerName = "MySql.Data.MySqlClient";

                temp.ConnectionString = connectionString;
                temp.ProviderName = providerName;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
               

            }

            else if (connInfo.getDatabaseType() == ConnInfo.ORACLE)
            {
                connectionString = "Data Source=" + connInfo.getServerAddress() + ";Persist Security Info=True;User ID=" + connInfo.getUserName() + ";Password=" + connInfo.getPassword() + ";Unicode=True";
                providerName = "System.Data.OracleClient";

                temp.ConnectionString = connectionString;
                temp.ProviderName = providerName;
                temp.SelectCommand = "SELECT COLUMN_NAME FROM dba_tab_columns WHERE (OWNER IS NOT NULL AND TABLE_NAME = '" + selectedTable + "')";
               
            }

            else //Default set to SQL
            {
            }


            Mapping conMapping = Mapping.getMapping2(conID, selectedTable);
            
            string dbTable = conMapping.getTableName();
            
            int dbFormat = conMapping.getFormat();
            
            string dbLat = conMapping.getLatFieldName();
            string dbLong = conMapping.getLongFieldName();

            //No information about the table yet
            if (dbTable == null)
            {
                mapUpdates(temp);
            }

            else
            {
                mapUpdates(temp, dbLat, dbLong, dbFormat);    
            }


            //Garbage collection
            connInfo = null;

            viewGrid.Visible = true;
            saveLatLong.Visible = true;
            addLatLong.Visible = false;
            tblColumnsPanel.Visible = false;
            mapColumnsPanel.Visible = true;
           
        }

        protected void viewGrid_Click(object sender, EventArgs e)
        {
            viewGrid.Visible = false;
            saveLatLong.Visible = false;
            addLatLong.Visible = true;
            mapColumnsPanel.Visible = false;
            tblColumnsPanel.Visible = true;
        }

        protected void saveLatLong_Click(object sender, EventArgs e)
        {
            string tableName = GridViewTables.SelectedValue.ToString();
            string latFieldName = "";
            string longFieldName = "";
            int format = 1;

            Mapping saveMapping = Mapping.getMapping2(conID, tableName);
            string mapTblName = saveMapping.getTableName();

            if (LLSepPanel.Visible == true)
            {
                latFieldName = latDD.SelectedValue.ToString();
                longFieldName = longDD.SelectedValue.ToString();
                if (latFieldName.Equals(longFieldName))
                {
                    mapError1.Visible = true;
                    mapSuccess.Visible = false;
                }
                else
                {
                    mapError1.Visible = false;
                    mapError2.Visible = false;

                    if (mapTblName == null)
                    {
                        Mapping.insertMapping(conID, tableName, latFieldName, longFieldName, format);
                        mapSuccess.Text = "Lat/Long Mapping inserted successfully!";
                        mapSuccess.Visible = true;
                    }
                    else
                    {
                        Mapping.updateMapping(conID, tableName, latFieldName, longFieldName, format);
                        mapSuccess.Text = "Lat/Long Mapping updated successfully!";
                        mapSuccess.Visible = true;
                    }

                }
            }
            else if (LLTogetherPanel.Visible == true)
            {
                latFieldName = llDD.SelectedValue.ToString();
                longFieldName = latFieldName;
                if ((LatLongCheck.Checked && LongLatCheck.Checked) || (!LatLongCheck.Checked && !LongLatCheck.Checked))
                {
                    mapSuccess.Visible = false;
                    mapError2.Visible = true;
                }
                else
                {
                    mapError1.Visible = false;
                    mapError2.Visible = false;

                    if (LongLatCheck.Checked)
                    {
                        format = 3;
                    }
                    else
                    {
                        format = 2;
                    }
                    if (mapTblName == null)
                    {
                        Mapping.insertMapping(conID, tableName, latFieldName, longFieldName, format);
                        mapSuccess.Text = "Lat/Long Mapping inserted successfully!";
                        mapSuccess.Visible = true;
                    }
                    else
                    {
                        Mapping.updateMapping(conID, tableName, latFieldName, longFieldName, format);
                        mapSuccess.Text = "Lat/Long Mapping updated successfully!";
                        mapSuccess.Visible = true;
                    }

                }
            }
        }
    }
}


