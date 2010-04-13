<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewKML.aspx.cs" Inherits="HCI.PreviewKML" %>

<%
    if (Session["Connection"] == "" || Session["Connection"] == null
        || Session["serverPath"] == "" || Session["serverPath"] == null)
    { %>
    <script type="text/javascript">
        alert("This page failed to load properly. This page should only be accessed through the preview KML button.");
    </script>
      <%  Response.Redirect("Main.aspx");
    }
    
    HCI.Connection connection = (HCI.Connection)Session["Connection"];
    //Test code
    String serverPath = Session["serverPath"].ToString(); //Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"];
    HCI.KMLGenerator generator = new HCI.KMLGenerator(connection.getConnInfo().getConnectionName(), serverPath);

    String KML = "";
    
    //Generate KML
    try
    {
        KML = generator.generateKMLFromConnection(connection);
        KML = KML.Replace('\n', ' ');
        KML = KML.Replace("\r", "<br/>");
        KML = KML.Replace("\"", "\\\"");
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
    <title>Preview KML</title> 
    <script src="http://www.google.com/jsapi?key=ABQIAAAA8npK1YVObaN6uuICGDhh5RT3si1wRED3L-DNqHfJwEViE-IQZxTrkIqSdTIkLgCp5kaNN7uOR1rznQ" type="text/javascript"></script>

  </head>
    <body>
    <div id="map3d" style="height: 800px; width: 800px;">
    </div>

    <script type="text/javascript">

         //JavaScript needed for google earth
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