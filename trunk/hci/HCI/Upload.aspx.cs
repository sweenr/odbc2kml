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
    public partial class Upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void btnSubmitClick(object sender, EventArgs e)
        {
            if (fileUpEx.HasFile)
            {

                String filepath = fileUpEx.PostedFile.FileName;
                //string pat = @"\\(?:.+)\\(.+)\.(.+)";
                //Regex r = new Regex(pat);
                ////run
                //Match m = r.Match(filepath);
                String file_ext = System.IO.Path.GetExtension(filepath);
                String filename = System.IO.Path.GetFileNameWithoutExtension(filepath);
                String suffix = GetRandomString();
                String file = filename + suffix + file_ext;

                //save the file to the server
                fileUpEx.PostedFile.SaveAs(fileSaveLoc + file);
                lblStatus.Text = "File Saved to: " + fileSaveLoc + file;

            }
        }
        //protected void fetchSubmitClick(object sender, EventArgs e)
        //{
        //    fetch = !fetch;
        //    Response.Write("FETCH!");
        //}
        protected void URLsubmitClick(object sender, EventArgs e)
        {
            String URL = URLtextBox.Text.Trim();
            URLpanel.Visible = true;
            URLsubmitLabel.Text = "You entered - " + "<a href=\"" + URL + "\" target=NEW>" + URL + "</a>";
            //DB.executeQueryLocal("");
        }
        protected void URLcorrectClick(object sender, EventArgs e)
        {
            String URL = URLtextBox.Text.Trim();
            Database DB = new Database();
            if (fetchCheckBox.Checked)
            {
                WebClient Client = new WebClient();
                String fileName = System.IO.Path.GetFileNameWithoutExtension(URL);
                String ext = System.IO.Path.GetExtension(URL);
                String suffix = GetRandomString();
                String Name = fileSaveLoc + fileName + suffix + ext;
                Client.DownloadFile(URL, Name);
                DB.executeQueryLocal("INSERT INTO IconLibrary (location, isLocal) VALUES (\'" + Name + "\', 1)");
            }
            else
            {
                DB.executeQueryLocal("INSERT INTO IconLibrary (location, isLocal) VALUES (\'" + URL + "\', 0)");
            }
        }
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }
        //private bool fetch;
        //private String URL;
        //private Database DB;
        public static String fileSaveLoc = @"C:\odbc2kml\uploads\";
    }
}
