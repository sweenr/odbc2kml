<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DBTest.aspx.cs" Inherits="HCI.DBTest" %>

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
    <div>
        <asp:TextBox runat="server" CssClass="inputBox" Width="150" ID="queryString" />
        <asp:Button runat="server" ID="runQuery" OnClick="executeQuery" Text="Submit" />
        <asp:Panel ID="resultsPanel" runat="server" Visible="false" CssClass="mainBox3Panel">
        </asp:Panel>
    </div>
    </form>
</body>
</html>
