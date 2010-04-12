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
    public class Utilities
    {
        // Max height for an icon
        public static readonly int HEIGHT = 128;

        // Max width for an icon
        public static readonly int WIDTH = 128;

        /// <summary>
        /// used for uploading icons from local computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static String uploadClick(String fileSaveLoc, String relativeFileSaveLoc, FileUpload fileUpEx)
        {
            ArrayList validTypes = BuildTypeList();
            Boolean valid = false;
            //checks to make sure there is an uploaded file
            if ((fileUpEx.HasFile) && (!fileUpEx.FileName.Equals("")))
            {
                //checks for valid filetype
                foreach (String type in validTypes)
                {
                    if (fileUpEx.PostedFile.ContentType.Equals(type))
                    {
                        valid = true;
                    }
                }
                //checks for valid dimensions
                if (valid && ValidateFileDimensions(fileUpEx.PostedFile.InputStream))
                {
                    String filepath = fileUpEx.PostedFile.FileName;
                    String file_ext = System.IO.Path.GetExtension(filepath);
                    String filename = System.IO.Path.GetFileNameWithoutExtension(filepath);
                    String suffix = GetRandomString();
                    String file = filename + suffix + file_ext;
                    String relativeName = relativeFileSaveLoc + file;
                    //save the file to the server
                    try
                    {
                        fileUpEx.PostedFile.SaveAs(fileSaveLoc + file);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        throw new ODBC2KMLException("Error saving file, please ensure " + fileSaveLoc + " exists on this machine");
                    }

                    Database DB = new Database();
                    try
                    {
                        DB.executeQueryLocal("INSERT INTO IconLibrary (location, isLocal) VALUES (\'" + relativeName + "\', 1)");
                    }
                    catch (ODBC2KMLException ex)
                    {
                        throw new ODBC2KMLException(ex.errorText);
                    }
                    return relativeName;
                }
                else if (!valid)
                {
                    String errorText = "Current File type = " + fileUpEx.PostedFile.ContentType + " File type not appropriate (only jpg, gif, tiff, png, bmp accepted)";
                    throw new ODBC2KMLException(errorText);
                }
                else
                {
                    throw new ODBC2KMLException("File dimensions to large (max 128 x 128)");
                }
            }
            else
            {
                throw new ODBC2KMLException("Please select a file to upload.");
            }
        }

        /// <summary>
        /// used for uploading icons from remote sources
        /// if fetch is checked this function downloads the linked icon and saves its info to the db and saves the icon
        /// if fetch is not checked it just saves the linked icon's info to the db
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static String URLsubmitClick(bool fetch, String URL, String fileSaveLoc, String relativeFileSaveLoc)
        {
            ArrayList validTypes = BuildTypeList2();

            Database DB = new Database();
            WebClient Client = new WebClient();

            //Create a request for the URL.
            if (URL.Equals(""))
                throw new ODBC2KMLException("Please enter a URL.");

            DataTable dt = DB.executeQueryLocal("SELECT * FROM IconLibrary WHERE location='" + URL + "'");
            if (dt.Rows.Count > 0)
                throw new ODBC2KMLException("URL Path already exists in Icon Library.");

            WebRequest request = WebRequest.Create(URL);
            request.Proxy = null;
            try
            {
                //Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                throw new ODBC2KMLException("Cannot connect to the URL you entered");
            }

            //below lines get information to check validity of icon and saves the icon temporarily
            String fileName = System.IO.Path.GetFileNameWithoutExtension(URL);
            String ext = System.IO.Path.GetExtension(URL);
            String suffix = GetRandomString();
            String name = fileSaveLoc + fileName + suffix + ext;

            //checks to see if fileType of icon is valid
            bool valid = false;
            foreach (String type in validTypes)
            {
                if (ext.Equals(type))
                {
                    valid = true;
                    break;
                }
            }

            if (valid)
            {
                try
                {
                    Client.DownloadFile(URL, name);
                }
                catch (WebException)
                {
                    throw new ODBC2KMLException("Error with temporary download, please ensure " + fileSaveLoc + " exists on this machine");
                }
                /*catch (ArgumentException)
                {
                    throw new ODBC2KMLException("No URL entered");
                }*/

                FileStream fs = File.OpenRead(name);
                bool validDim = ValidateFileDimensions(fs);
                fs.Close();
                if (fetch)
                {
                    String relativeName = relativeFileSaveLoc + fileName + suffix + ext;
                    //checks if icon has valid dimensions
                    if (validDim)
                    {
                        try
                        {
                            DB.executeQueryLocal("INSERT INTO IconLibrary (location, isLocal) VALUES (\'" + relativeName + "\', 1)");
                        }
                        catch (ODBC2KMLException ex)
                        {
                            throw new ODBC2KMLException(ex.errorText);
                        }
                    }
                    else
                    {
                        File.Delete(name);
                        throw new ODBC2KMLException("The file you linked to was to large (max 128 x 128)");
                    }
                    return relativeName;
                }
                else
                {
                    //checks if icon has valid dimensions
                    File.Delete(name);
                    if (validDim)
                    {
                        try
                        {
                            DB.executeQueryLocal("INSERT INTO IconLibrary (location, isLocal) VALUES (\'" + URL + "\', 0)");
                        }
                        catch (ODBC2KMLException ex)
                        {
                            throw new ODBC2KMLException(ex.errorText);
                        }
                    }
                    else
                    {
                        throw new ODBC2KMLException("The file you linked to was to large (max 128 x 128)");
                    }
                    return URL;
                }
            }
            else if (!valid)
            {
                throw new ODBC2KMLException("You linked to an invalid file type");
            }
            return "";
        }

        /// <summary>
        /// helper function used by saving icons without name overlap
        /// </summary>
        /// <returns></returns>
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }

        /// <summary>
        /// type list used to check for valid file types of uploaded icon
        /// </summary>
        public static ArrayList BuildTypeList()
        {
            ArrayList validTypes = new ArrayList();

            validTypes.Add("image/bmp");
            validTypes.Add("image/gif");
            validTypes.Add("image/jpeg");
            validTypes.Add("image/pjpeg");
            validTypes.Add("image/png");
            validTypes.Add("image/tiff");
            validTypes.Add("image/x-tiff");
            validTypes.Add("image/x-windows-bmp");

            return validTypes;
        }

        /// <summary>
        /// type list used to check for valid file types of icons when using URL
        /// </summary>
        public static ArrayList BuildTypeList2()
        {
            ArrayList validTypes = new ArrayList();

            validTypes.Add(".bmp");
            validTypes.Add(".gif");
            validTypes.Add(".jpeg");
            validTypes.Add(".pjpeg");
            validTypes.Add(".png");
            validTypes.Add(".tiff");
            validTypes.Add(".tif");
            validTypes.Add(".jpg");

            return validTypes;
        }

        /// <summary>
        /// helper function to check dimensions of uploaded icons
        /// </summary>
        /// <param name="input"></param>
        /// <returns>bool, true if valid dimensions</returns>
        public static bool ValidateFileDimensions(Stream input)
        {
            using (System.Drawing.Image myImage =
              System.Drawing.Image.FromStream(input))
            {
                return (myImage.Height <= HEIGHT && myImage.Width <= WIDTH);
            }
        }
    }
}
