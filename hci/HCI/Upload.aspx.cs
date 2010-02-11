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
using HCI;

namespace HCI
{
    public partial class Upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            fetch = false;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (fileUpEx.HasFile)
            {

                string filepath = fileUpEx.PostedFile.FileName;
                string pat = @"\\(?:.+)\\(.+)\.(.+)";
                Regex r = new Regex(pat);
                //run
                Match m = r.Match(filepath);
                string file_ext = m.Groups[2].Captures[0].ToString();
                string filename = m.Groups[1].Captures[0].ToString();
                string file = filename + "." + file_ext;

                //save the file to the server
                fileUpEx.PostedFile.SaveAs(Server.MapPath(".\\") + file);
                lblStatus.Text = "File Saved to: " + Server.MapPath(".\\") + file;

            }
        }
        protected void fetchSubmit_Click(object sender, EventArgs e)
        {
            fetch = !fetch;
            Response.Write("FETCH!");
        }
        protected void URLsubmit_Click(object sender, EventArgs e)
        {
            URL = URLtextBox.Text.Trim();
            URLsubmitLabel.Text = "You entered - " + "<a href=\"" + URL + "\" target=NEW>" + URL + "</a>";
            //DB.executeQueryLocal("");
        }
        private bool fetch;
        private String URL;
        Database DB = new Database();
    }
}
