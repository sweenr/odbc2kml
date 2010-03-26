<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="viewTable.aspx.cs" Inherits="HCI.viewTable" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Database test</title>
   
    <script src="jquery/jquery-1.4.1.js" type="text/javascript"></script> 
    <script src="jquery/jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>
    <link href="jquery/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
	$(function() {
		$("#errorPanel1").dialog({
			bgiframe: true,
			modal: true,
			autoOpen: false,
			title: 'Error!',
			resizable: false,
			dialogClass: 'alert',
			buttons: {
				Ok: function() {
					$(this).dialog('close');
				}
			}
		});
	});
	</script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="dbtestManager" runat="server">
    </asp:ScriptManager>
    <div>
      
    
        <asp:Panel ID="errorPanel1" runat="server" visible="true" Style="color: White" >
        </asp:Panel>
        
    </div>
    </form>
</body>
</html>
