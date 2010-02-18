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
            BuildTypeList();
        }
        protected void btnSubmitClick(object sender, EventArgs e)
        {
            Boolean valid = false;
            if (fileUpEx.HasFile)
            {
                foreach (String type in validTypes)
                {
                    if (fileUpEx.PostedFile.ContentType.Equals(type))
                    {
                        valid = true;
                    }
                }
                if (valid && ValidateFileDimensions())
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
                else if (!valid)
                {
                    lblStatus.Text = "Current File type = " + fileUpEx.PostedFile.ContentType + " File type not appropriate (only jpg, gif, tiff, png, bmp accepted)";
                }
                else
                {
                    lblStatus.Text = "File dimensions to large (max 128 x 128)";
                }
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
                String tempName = tempSaveLoc + fileName + suffix + ext;
                String Name = fileSaveLoc + fileName + suffix + ext;
                Client.DownloadFile(URL, tempName);
                //add conditions
                File.Move(tempName, Name);
                File.Delete(tempName);
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
        public void BuildTypeList()
        {
            this.validTypes.Add("image/bmp");
            this.validTypes.Add("image/gif");
            this.validTypes.Add("image/jpeg");
            this.validTypes.Add("image/pjpeg");
            this.validTypes.Add("image/png");
            this.validTypes.Add("image/tiff");
            this.validTypes.Add("image/x-tiff");
            this.validTypes.Add("image/x-windows-bmp");
        }
        public bool ValidateFileDimensions()
        {
            using (System.Drawing.Image myImage =
              System.Drawing.Image.FromStream(fileUpEx.PostedFile.InputStream))
            {
                return (myImage.Height <= height && myImage.Width <= width);
            }
        } 
        //private bool fetch;
        //private String URL;
        //private Database DB;
        public const int height = 128;
        public const int width = 128;
        public static String tempSaveLoc = @"C:\odbc2kml\temp\";
        public static String fileSaveLoc = @"C:\odbc2kml\uploads\";
        public ArrayList validTypes = new ArrayList();
    }
}
