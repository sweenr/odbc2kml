<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewKMLTest.aspx.cs" Inherits="HCI.PreviewKMLTest" %>

<%
    //int connID = Request.QueryString["connID"];
    //Test code
    String serverPath = Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"];
    int connID = 1;
    HCI.KMLGenerator generator = new HCI.KMLGenerator("Test_FILE", serverPath);

    String KML = "";
    
    //Generate KML
    try
    {
        KML = generator.generateKML(connID);
        KML = KML.Replace('\n', ' ');
    }
    catch (HCI.ODBC2KMLException e)
    {
        Response.Write("ERROR: Could not generate KML. Check database connection. Actual error: " + e);
    }
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html> 
  <head>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8"/>
    <title>Google Maps</title> 
    <script src="http://www.google.com/jsapi?key=ABQIAAAA8npK1YVObaN6uuICGDhh5RT3si1wRED3L-DNqHfJwEViE-IQZxTrkIqSdTIkLgCp5kaNN7uOR1rznQ" type="text/javascript"></script>

  </head>
<body>
    <div id="map3d" style="height: 800px; width: 800px;">
    </div>

    <script type="text/javascript">

         
         var ge;
         
          google.load("earth", "1");
          
         function init() {
            google.earth.createInstance('map3d', initCB, failureCB);
         }
                   
         function initCB(instance) {
         ge = instance;
         ge.getNavigationControl().setVisibility(ge.VISIBILITY_AUTO); 
         ge.getWindow().setVisibility(true);
         var kmlString = "<%=KML%>";
         var kmlObject = ge.parseKml(kmlString);
         ge.getFeatures().appendChild(kmlObject);
         ge.getView().setAbstractView(kmlObject.getAbstractView());
      }

      function failureCB(errorCode) {
        alert("Google Earth did not load properly.");
      }

      google.setOnLoadCallback(init);
    </script>
  </body>
</html>