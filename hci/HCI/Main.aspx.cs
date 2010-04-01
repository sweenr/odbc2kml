using System;
using System.IO;
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
            Database db = new Database();
            DataTable dt;
            dt = db.executeQueryLocal("SELECT id,name FROM CONNECTION");
            int i = 0;

            foreach (DataRow dr in dt.Rows)
            {
                string dbID = dr.ItemArray.ElementAt(0).ToString();
                string odbcName = dr.ItemArray.ElementAt(1).ToString();
                
                //Defines buttons
                ImageButton openConn = new ImageButton();
                openConn.CssClass = "openIcon";
                openConn.ImageUrl = "graphics/connIcon.gif";
                openConn.AlternateText = "Open Connection";
                openConn.ToolTip = "Open Connection";
                openConn.PostBackUrl = "ConnDetails.aspx?ConnID=" + dbID + "&locked=true";

                ImageButton editConn = new ImageButton();
                editConn.CssClass = "editIcon";
                editConn.ImageUrl = "graphics/connIcon.gif";
                editConn.AlternateText = "Edit Connection";
                editConn.ToolTip = "Edit Connection";
                editConn.PostBackUrl = "ConnDetails.aspx?ConnID=" + dbID + "&locked=false";

                ImageButton deleteConn = new ImageButton();
                deleteConn.ID = "dc" + Convert.ToString(i);
                deleteConn.CssClass = "deleteIcon";
                deleteConn.ImageUrl = "graphics/connIcon.gif";
                deleteConn.AlternateText = "Delete Connection";
                deleteConn.ToolTip = "Delete Connection";
                deleteConn.Click += new ImageClickEventHandler(confirmDelete);
                deleteConn.CommandArgument = dbID;
                deleteConn.CommandArgument += "#" + odbcName;

                ImageButton genKML = new ImageButton();
                genKML.CssClass = "kmlIcon";
                genKML.ImageUrl = "graphics/connIcon.gif";
                genKML.AlternateText = "Generate KML File";
                genKML.ToolTip = "Generate KML File";
                genKML.Click += new ImageClickEventHandler(genKMLFunction);
                genKML.CommandArgument = dbID;


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
                ConnectionsAvailable.Controls.Add(new LiteralControl("<span id=\"Conn" + dbID + "\">" + odbcName + "</span>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<a href=\"#\" title=\"Open Connection\"></a>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("</td>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<td class=\"connIcons\">\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<table>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<tr>\n"));
                ConnectionsAvailable.Controls.Add(new LiteralControl("<td>\n"));
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

                i += 1;
            }

            ConnSMgr.Controls.Add(new LiteralControl("<script type='text/JavaScript'>$('#odbcDBType').change("
              + "function()"
              + "{ if($('#odbcDBType').val() == 'Oracle') { $('#oracleTable').css('display', 'block'); }"
              + "else { $('#oracleTable').css('display', 'none');}"
              + "})</script>"));

        }

        protected void deleteConnFunction(object sender, EventArgs e)
        {
            //Delete the connection
            Button sendBtn = (Button)sender;
            String args = sendBtn.CommandArgument.ToString();
            Database db = new Database();
            db.executeQueryLocal("DELETE FROM CONNECTION WHERE ID="+args);
            this.deletePopupExtender.Hide();
            Response.Redirect("Main.aspx");
            
        }

        protected void confirmDelete(object sender, EventArgs e)
        {
            ImageButton sendBtn = (ImageButton)sender;
            String[] args = sendBtn.CommandArgument.ToString().Split(new Char[] {'#'});
            String args1 = args[0].ToString();
            String args2 = args[1].ToString();

            delConnBtn.Click += new EventHandler(deleteConnFunction);
            connToDelete.Text = "Are you sure you want to delete the connection: " + args[1].ToString();
            delConnBtn.CommandArgument = args1.ToString();

            this.deletePopupExtender.Show();

        }

        protected void genKMLFunction(object sender, EventArgs e)
        {
            //Generate the KML from the connection
            ImageButton sendBtn = (ImageButton)sender;
            String serverPath = Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"];
            string args = sendBtn.CommandArgument.ToString();
            KMLGenerator kml = new KMLGenerator("testFile", serverPath);

            try
            {
                //Generate the KML string based on the connection id
                String kmlString = kml.generateKML(int.Parse(args));
                Connection conn = new Connection(int.Parse(args));
                conn.populateFields();

                //Write the KML string to a downloadable file
                Response.ClearHeaders();
                Response.ClearContent();
                Response.ContentType = "application/vnd.google-earth.kml+xml kml";
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + (conn.getConnInfo()).getConnectionName() + ".kml");
                Response.Write(kmlString);
                Response.End();
                return;
            }
            catch (ODBC2KMLException error) 
            {
                ErrorHandler err = new ErrorHandler(error.errorText, errorPanel1);
                err.displayError();
                return;
            }

            //Response.Redirect("Main.aspx", true);
        }

        protected void launchNewConnection(object sender, EventArgs e)
        {
            this.NewConn1ModalPopUp.Show();

            return;
        }

        protected void createConnection(object sender, EventArgs e)
        {
            //this.NewConn1ModalPopUp.Hide();
            validNewConn.Visible = false;

            String ConnName = odbcNameE.Text.ToString();
            String ConnDBName = odbcDNameE.Text.ToString();
            String ConnDBAddress = odbcDatabaseE.Text.ToString();
            String ConnPortNum = odbcPNE.Text.ToString();
            String ConnUser = odbcUserE.Text.ToString();
            String ConnPWD = odbcPWE.Text.ToString();
            String ConnDBType = odbcDBType.SelectedItem.ToString();
            String oracleProtocol = odbcProtocol.Text.ToString();
            String oracleSName = odbcSName.Text.ToString();
            String oracleSID = odbcSID.Text.ToString();
            String DBTypeNum;

            if (ConnDBType.Equals("MySQL")){
                DBTypeNum = "0";
            }else if(ConnDBType.Equals("MSSQL")){
                DBTypeNum = "1";
            }else{
                DBTypeNum = "2";
            }

            if (DBTypeNum.Equals("2")){
                if (oracleSName.Equals("") && oracleSID.Equals("")){
                    this.NewConn1ModalPopUp.Hide();
                    validNewConn.Text = "Either Service Name or Service ID must be completed!";
                    validNewConn.Visible = true;
                    this.NewConn1ModalPopUp.Show();

                    return;
                }
                if (oracleProtocol.Equals(""))
                {
                    this.NewConn1ModalPopUp.Hide();
                    validNewConn.Text = "Oracle protocol must be provided!";
                    validNewConn.Visible = true;
                    this.NewConn1ModalPopUp.Show();

                    return;
                }
            }

            Database dbCheck = new Database();
            DataTable dtCheck;
            dtCheck = dbCheck.executeQueryLocal("SELECT name FROM Connection WHERE name=\'" + ConnName + "\'");
            if (dtCheck.Rows.Count > 0)
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Text = "Connection name already in use!";
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }
            if (ConnName.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Text = "All fields must be completeted!";
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }else if (ConnDBName.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Text = "All fields must be completeted!";
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }else if (ConnDBAddress.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Text = "All fields must be completeted!";
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }else if (ConnPortNum.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Text = "All fields must be completeted!";
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }else if(ConnUser.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Text = "All fields must be completeted!";
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }else if (ConnPWD.Equals(""))
            {
                this.NewConn1ModalPopUp.Hide();
                validNewConn.Text = "All fields must be completeted!";
                validNewConn.Visible = true;
                this.NewConn1ModalPopUp.Show();

                return;
            }

            validNewConn.Visible = false;

            ConnInfo testConn = new ConnInfo();
            testConn.setConnectionName(ConnName);
            testConn.setDatabaseName(ConnDBName);
            testConn.setDatabaseType((int)Convert.ToInt32(DBTypeNum));
            testConn.setPassword(ConnPWD);
            testConn.setPortNumber(ConnPortNum);
            testConn.setServerAddress(ConnDBAddress);
            testConn.setUserName(ConnUser);
            if (DBTypeNum.Equals("2"))
            {
                testConn.setOracleProtocol(oracleProtocol);
                testConn.setOracleServiceName(oracleSName);
                testConn.setOracleSID(oracleSID);
            }

            try
            {
                Database dbTest = new Database(testConn);
                DataTable dtTest;
                if (DBTypeNum.Equals("0"))
                {
                    dtTest = dbTest.executeQueryRemote("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES");
                }
                else if (DBTypeNum.Equals("1"))
                {
                    dtTest = dbTest.executeQueryRemote("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES");
                }
                else
                {
                    dtTest = dbTest.executeQueryRemote("SELECT TABLE_NAME FROM dba_tab_tables");
                }

            }
            catch (ODBC2KMLException ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
                this.NewConn1ModalPopUp.Hide();
                eh.displayError();
                return;
            }

            //Call Create DB with the DB Function
            Database db = new Database();
            DataTable dt;
            if (DBTypeNum.Equals("2"))
            {
                db.executeQueryLocal("INSERT INTO Connection (name, dbName, userName, password, port, address, type, protocol, serviceName, SID) VALUES ('" + ConnName + "', '" + ConnDBName + "', '" + ConnUser + "', '" + ConnPWD + "', '" + ConnPortNum + "', '" + ConnDBAddress + "', '" + DBTypeNum + "', '"+oracleProtocol+"', '"+oracleSName+"', '"+oracleSID+"')");
            }
            else
            {
                db.executeQueryLocal("INSERT INTO Connection (name, dbName, userName, password, port, address, type, protocol, serviceName, SID) VALUES ('" + ConnName + "', '" + ConnDBName + "', '" + ConnUser + "', '" + ConnPWD + "', '" + ConnPortNum + "', '" + ConnDBAddress + "', '" + DBTypeNum + "', '', '', '')");
            }

            this.NewConn1ModalPopUp.Hide();
            //Jump to the Modify page
            
            dt = db.executeQueryLocal("SELECT ID FROM CONNECTION WHERE name='"+ConnName+"' AND dbName='"+ConnDBName+"' AND userName='"+ConnUser+"' AND port='"+ConnPortNum+"' AND address='"+ConnDBAddress+"' AND type='"+DBTypeNum+"'");
            foreach (DataRow dr in dt.Rows)
            {
                string connID = dr.ItemArray.ElementAt(0).ToString();

                Response.Redirect("ConnDetails.aspx?ConnID=" + connID + "&locked=false");
            }
        }
    }
}
