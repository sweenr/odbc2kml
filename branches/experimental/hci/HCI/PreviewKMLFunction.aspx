<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewKMLFunction.aspx.cs" Inherits="HCI.PreviewKMLFunction" %>;

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8"/>
    <title>Preview KML</title> 
    <script src="http://maps.google.com/maps?file=api&amp;v=2&amp;sensor=false&amp;key=ABQIAAAA2xKK7kwByh7EgVL6iAp38hT2yXp_ZAY8_ufC3CFXhHIE1NvwkxSxVtYegdPh_IerJZ-CiNXor1V9iQ" type="text/javascript"></script> 
  </head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type='button' value='Preview KML' onclick="generateAndDisplayKML()"/>
    </div>
    </form>
    <script type="text/javascript">        
    
        function generateAndDisplayKML(filename) 
        {
            filename = "lancashire.kml";
        
            //Generate KML and GO!
            window.open("http://localhost:4882/PreviewKMLTest.aspx?name=" + filename);
        }
    
    </script>
</body>
</html>