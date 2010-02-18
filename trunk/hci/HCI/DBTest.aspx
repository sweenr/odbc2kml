<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DBTest.aspx.cs" Inherits="HCI.DBTest" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Database test</title>
    <style type="text/css" media="all">
        @import "odbcStyle.css";
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="dbtestManager" runat="server">
    </asp:ScriptManager>
    <div>
        <asp:DropDownList runat="server" ID="connectionSelector">
        </asp:DropDownList>
        <asp:TextBox runat="server" CssClass="inputBox" Width="300" ID="queryString" />
        <asp:Button runat="server" ID="runQuery" OnClick="executeQuery" Text="Show Results" CssClass="button"/>
        <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="hideResults"
             runat="server" PopupControlID="resultsPanel" ID="ModalPopupExtender6"
            TargetControlID="runQuery" />
        <asp:Panel ID="resultsPanel" runat="server" visible="false" Style="display: none;" CssClass="boxPopupStyle">
        </asp:Panel>
        <asp:Panel ID="errorPanel1" runat="server" visible="false" Style="display: none;">
        </asp:Panel>
    </div>
    </form>
</body>
</html>
