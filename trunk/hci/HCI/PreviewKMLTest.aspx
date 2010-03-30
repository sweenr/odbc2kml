<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewKMLTest.aspx.cs" Inherits="HCI.PreviewKMLTest" %>;

<%
    //int connID = Request.QueryString["connID"];
    //Test code
    int connID = 1;
    HCI.KMLGenerator generator = new HCI.KMLGenerator("Test_FILE");
    String KML = generator.generateKML(connID);
    KML = KML.Replace('\n', ' ');
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html> 
  <head>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8"/>
    <title>Google Maps</title> 
    <script src="http://www.google.com/jsapi?key=ABQIAAAA8npK1YVObaN6uuICGDhh5RT3si1wRED3L-DNqHfJwEViE-IQZxTrkIqSdTIkLgCp5kaNN7uOR1rznQ" type="text/javascript"></script>
  </head>
  <body> 
    <div id="map3d" style="height:800px; width:800px;"></div>
      <script type="text/javascript">

         //var map = new GMap2(document.getElementById("map"));
         
         var ge;
         
          google.load("earth", "1");
          
         function init() {
            google.earth.createInstance('map3d', initCB, failureCB);
         }
                   
         function initCB(instance) {
         ge = instance;
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



         
       
         
        // map.setCenter(new GLatLng(53.763325,-2.579041), 15);
        // map.addControl(new GLargeMapControl());
        // map.addControl(new GMapTypeControl());
        // map.getFeatures().appendChild(kmlObject);

         // ==== Create a KML Overlay ====
         
         //Server path
        // var path = "http://www.campusbookmart.com/";
         //var actualPath = path + "=file";

         
            //map.addOverlay(new GGeoXml(actualPath));

      </script>
  </body>
</html>