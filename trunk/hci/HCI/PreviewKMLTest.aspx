<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewKMLTest.aspx.cs" Inherits="HCI.PreviewKMLTest" %>;

<%
    string file = Request.QueryString["name"];
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html> 
  <head>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8"/>
    <title>Google Maps</title> 
    <script src="http://maps.google.com/maps?file=api&amp;v=2&amp;sensor=false&amp;key=ABQIAAAA2xKK7kwByh7EgVL6iAp38hT2yXp_ZAY8_ufC3CFXhHIE1NvwkxSxVtYegdPh_IerJZ-CiNXor1V9iQ" type="text/javascript"></script> 
  </head>
  <body onunload="GUnload()"> 
    <div id="map" style="width: 1000px; height: 700px"></div>
      <script type="text/javascript">
        if (GBrowserIsCompatible()) {

         var map = new GMap2(document.getElementById("map"));
         map.setCenter(new GLatLng(53.763325,-2.579041), 15);
         map.addControl(new GLargeMapControl());
         map.addControl(new GMapTypeControl());

         // ==== Create a KML Overlay ====
         
         //Server path
         var path = "http://www.campusbookmart.com/";
         var actualPath = path + "<%=file%>";

         
         map.addOverlay(new GGeoXml(actualPath));

         }

         // display a warning if the browser was not compatible
         else {
         alert("Sorry, the Google Maps API is not compatible with this browser");
         }

      </script>
  </body>
</html>