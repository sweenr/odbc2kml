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

namespace KMLGenWebSVC
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://polytech-dev/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {

        [WebMethod]
        public XmlDocument getKML(int connID)
        {
            //if this is a test, return the dummy kml file.
            if (connID == -99)
            {
                XmlDocument kmlDoc = new XmlDocument();
                String kml = "<kml xmlns=\"http://earth.google.com/kml/2.0\">\n  <Document>\n  <Placemark>\n   <name>Starkville</name>\n         <Point>\n     <coordinates>-88.790361, 33.457741</coordinates>\n   </Point>\n  </Placemark>\n </Document>\n</kml>";
                kmlDoc.LoadXml(kml);
                return kmlDoc;
            }
            else
            {
                //return KMLGenerator.generate(connID);
                return new XmlDocument();
            }
        }
    }
}
