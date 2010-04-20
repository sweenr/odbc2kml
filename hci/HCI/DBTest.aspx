<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DBTest.aspx.cs" Inherits="ODBC2KML.DBTest" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Database test</title>
    <style type="text/css" media="all">
        @import "odbcStyle.css";
    </style>
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
        <asp:DropDownList runat="server" ID="connectionSelector" AutoPostBack="true">
        </asp:DropDownList>
        <asp:TextBox runat="server" CssClass="inputBox" Width="300" ID="queryString" />
        <asp:Button runat="server" ID="runQuery" OnClick="executeQuery" Text="Show Results" CssClass="button"/>
        <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="false" OkControlID="hideResults"
             runat="server" PopupControlID="resultsPanel" ID="ModalPopupExtender6"
            TargetControlID="runQuery" />
        <asp:Panel ID="resultsPanel" runat="server" visible="false" Style="display: none;" CssClass="boxPopupStyle">
        </asp:Panel>
        <asp:Panel ID="errorPanel1" runat="server" visible="true" Style="color: White" >
        </asp:Panel>
    </div>
    </form>
</body>
</html>
