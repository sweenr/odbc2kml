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
                editConn.Click += new ImageClickEventHandler(confirmEdit);
                editConn.CommandArgument = dbID;

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

            ConnSMgr.Controls.Add(new LiteralControl("<script type='text/JavaScript'>$('#editConnDBType').change("
              + "function()"
              + "{ if($('#editConnDBType').val() == 'Oracle') { $('#odbcTable1').css('display', 'block'); }"
              + "else { $('#odbcTable1').css('display', 'none');}"
              + "})</script>"));

        }

        protected void confirmEdit(object sender, EventArgs e)
        {
            ImageButton sendBtn = (ImageButton)sender;
            String args = sendBtn.CommandArgument.ToString();

            Database dbCheck = new Database();
            DataTable dtCheck;
            dtCheck = dbCheck.executeQueryLocal("SELECT name,dbName,userName,password,port,address,type,protocol,serviceName,SID FROM Connection WHERE ID=\'" + args + "\'");
            foreach (DataRow dr in dtCheck.Rows)
            {
                editConnName.Text = dr[0].ToString();
                editConnDBName.Text = dr[1].ToString();
                editConnUser.Text = dr[2].ToString();
                editConnPass.Attributes.Add("value", dr[3].ToString());
                editConnDBPort.Text = dr[4].ToString();
                editConnDBAddr.Text = dr[5].ToString();
                if (Convert.ToInt32(dr[6].ToString()) == ConnInfo.MYSQL)
                {
                    editConnDBType.SelectedValue = "MySQL";
                }
                else if (Convert.ToInt32(dr[6].ToString()) == ConnInfo.MSSQL)
                {
                    editConnDBType.SelectedValue = "MSSQL";
                }
                else 
                {
                    editConnDBType.SelectedValue = "Oracle";
                }
                editConnDBType.Text = dr[6].ToString();
                editOracleProtocol.Text = dr[7].ToString();
                editOracleService.Text = dr[8].ToString();
                editOracleSID.Text = dr[9].ToString();
            }
            saveAndEditConn.CommandArgument = args;
            saveEditConn.CommandArgument = args;


            this.editConnModalPopUp.Show();
        }

        protected Boolean editAndSaveData(object sender, EventArgs e)
        {
            Button sendBtn = (Button)sender;
            String args = sendBtn.CommandArgument.ToString();

            ConnInfo tempConnInfo = new ConnInfo();

            tempConnInfo.setConnectionName(editConnName.Text);
            tempConnInfo.setDatabaseName(editConnDBName.Text);
            tempConnInfo.setServerAddress(editConnDBAddr.Text);
            tempConnInfo.setPortNumber(editConnDBPort.Text);
            tempConnInfo.setUserName(editConnUser.Text);
            tempConnInfo.setPassword(editConnPass.Text);
            tempConnInfo.setDatabaseType(Convert.ToInt16(editConnDBType.SelectedItem.ToString()));
            tempConnInfo.setOracleProtocol(editOracleProtocol.Text);
            tempConnInfo.setOracleServiceName(editOracleService.Text);
            tempConnInfo.setOracleSID(editOracleSID.Text);
            
            //If the connection information is bad, report the error and cancel the function
            if(!tempConnInfo.isValid())
            {
                String error = "The entered connection information is invalid. Please make sure all fields are filled and that they are in proper format.";

                if (tempConnInfo.getDatabaseType() == ConnInfo.ORACLE)
                {
                    error = "The entered connection information is invalid. Please verify that all fields have a value and the value is of proper type."
                        + " Also, make sure that Oracle SID or Oracle Service Name and Oracle Protocol have been entered.";
                }

                ErrorHandler eh = new ErrorHandler(error, errorPanel1);
                this.editConnModalPopUp.Hide();
                eh.displayError();
                return false;
            }


           //Create database for update query and update
            Database db = new Database();

            db.executeQueryLocal("UPDATE Connection SET name='" + tempConnInfo.getConnectionName() 
                + "', dbName='" + tempConnInfo.getDatabaseName() + "', userName='" + tempConnInfo.getUserName() 
                + "', password='" + tempConnInfo.getPassword() + "', port='" + tempConnInfo.getPortNumber() 
                + "', address='" + tempConnInfo.getServerAddress() + "', type='" + tempConnInfo.getDatabaseType() 
                + "', protocol='" + tempConnInfo.getOracleProtocol() + "', serviceName='" + tempConnInfo.getOracleServiceName()
                + "', SID='" + tempConnInfo.getOracleSID() + "' WHERE (ID='" + args + "')");

            Connection conn = new Connection(Convert.ToInt16(args));
            conn.populateFields();

            try
            {
                //Force the connection into a safe state, if it is not
                if (conn.safeStateConnection())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(ODBC2KMLException err)
            {
                String error = "Unexpected error when trying to ensure the connection was in a safe state.";
                ErrorHandler eh = new ErrorHandler(error, errorPanel1);
                this.editConnModalPopUp.Hide();
                eh.displayError();
                return false;
            }

            this.editConnModalPopUp.Hide();

            return true;
        }

        protected void editSaveAndLoadData(object sender, EventArgs e)
        {
            //save the connection information and redirect
            if (editAndSaveData(sender, e))
            {
                Response.Redirect("ConnDetails.aspx?ConnID=" + ((Button)sender).CommandArgument.ToString() + "&locked=false");
            }
            else //Edit and save data will present error
            {
                return;
            }
        }

        protected void editCancel(object sender, EventArgs e)
        {
            this.editConnModalPopUp.Hide();
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
            try
            {
                //Generate the KML from the connection
                ImageButton sendBtn = (ImageButton)sender;
                String serverPath = Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"];
                string args = sendBtn.CommandArgument.ToString();
                KMLGenerator kml = new KMLGenerator("testFile", serverPath);


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
            catch (Exception error) 
            {
                ErrorHandler err = new ErrorHandler("There was an exception generating KML.", errorPanel1);
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
            odbcDBType.SelectedIndex = 0;

            if (ConnDBType.Equals("MySQL")){
                DBTypeNum = "0";
            }else if(ConnDBType.Equals("MSSQL")){
                DBTypeNum = "1";
            }else{
                DBTypeNum = "2";
            }

            if (DBTypeNum.Equals("2")){
                if (oracleSName.Equals("") && oracleSID.Equals("")){
                    ErrorHandler eh = new ErrorHandler("Either Service Name or Service ID must be completed!", errorPanel1);
                    this.NewConn1ModalPopUp.Hide();
                    eh.displayError();
                    return;
                }
                if (oracleProtocol.Equals(""))
                {
                    ErrorHandler eh = new ErrorHandler("Oracle protocol must be provided!", errorPanel1);
                    this.NewConn1ModalPopUp.Hide();
                    eh.displayError();
                    return;
                }
            }

            Database dbCheck = new Database();
            DataTable dtCheck;
            dtCheck = dbCheck.executeQueryLocal("SELECT name FROM Connection WHERE name=\'" + ConnName + "\'");
            if (dtCheck.Rows.Count > 0)
            {
                ErrorHandler eh = new ErrorHandler("Connection name already in use!", errorPanel1);
                this.NewConn1ModalPopUp.Hide();
                eh.displayError();
                return;
            }
            if (ConnName.Equals(""))
            {
                ErrorHandler eh = new ErrorHandler("The connection must have a unique name!", errorPanel1);
                this.NewConn1ModalPopUp.Hide();
                eh.displayError();
                return;
            }else if (ConnDBName.Equals(""))
            {
                ErrorHandler eh = new ErrorHandler("The connection must have a database name!", errorPanel1);
                this.NewConn1ModalPopUp.Hide();
                eh.displayError();
                return;
            }else if (ConnDBAddress.Equals(""))
            {
                ErrorHandler eh = new ErrorHandler("The connection must have a database address!", errorPanel1);
                this.NewConn1ModalPopUp.Hide();
                eh.displayError();
                return;
            }else if (ConnPortNum.Equals(""))
            {
                ErrorHandler eh = new ErrorHandler("The connection must have a port number!", errorPanel1);
                this.NewConn1ModalPopUp.Hide();
                eh.displayError();
                return;
            }else if(ConnUser.Equals(""))
            {
                ErrorHandler eh = new ErrorHandler("The connection must have a user name!", errorPanel1);
                this.NewConn1ModalPopUp.Hide();
                eh.displayError();
                return;
            }else if (ConnPWD.Equals(""))
            {
                ErrorHandler eh = new ErrorHandler("The connection must have a password!", errorPanel1);
                this.NewConn1ModalPopUp.Hide();
                eh.displayError();
                return;
            }

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
                    dtTest = dbTest.executeQueryRemote("SELECT TABLE_NAME FROM user_tables");
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
