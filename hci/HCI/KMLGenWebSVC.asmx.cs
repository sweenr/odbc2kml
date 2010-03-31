using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;
using HCI;

namespace KMLGenWebSVC
{
    /// <summary>
    /// Web Service that, given a connID, return the KML generated for that connection
    /// </summary>
    [WebService(Namespace = "http://polytech-dev/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        /// <summary>
        /// Method to return a KML file in an XMLDocument  
        /// </summary>
        /// <param name="connID">int --> the connection ID used to generate KML</param>
        /// <returns>XMLDocument --> contains the KML formatted in an XMLDocument</returns>
        [WebMethod]
        public XmlDocument getKML(int connID)
        {
            String serverPath = "http://" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + ":"
                + HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            //create new connection a populate fields to get the connection name for KMLGenerator
            Connection conn = new Connection(connID);
            conn.populateFields();
            string name = conn.getConnInfo().getConnectionName();
            //create a new kml genereator with the connection name as the placemark name
            KMLGenerator kmlGen = new KMLGenerator(name, serverPath);
            //generate the kml for the given connID
            string kml = kmlGen.generateKML(connID);
            //add the kml to an XMLDoc and return
            XmlDocument kmlDoc = new XmlDocument();
            kmlDoc.LoadXml(kml);
            return kmlDoc;
        }
        
    }
}
