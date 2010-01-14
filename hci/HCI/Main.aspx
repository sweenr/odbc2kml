<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="HCI.Main" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Main Page</title>
    <style type="text/css" media="all">
        @import "odbcStyle.css";
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="wrapIt">
        <div id="header">
            <div id="logo">
                Site Logo Goes Here
            </div>
            <div id="home">
                Link Home
            </div>
        </div>
        <div id="page">
            <table cellpadding="10" width="50%">
                <tr>
                    <td>
                        <div class="mainBox">
                            <span class="connectionStyle">&nbsp;Connection Information</span>
                            <table cellspacing="0" cellpadding="10" class="connectionBox">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="ODBC Connection 1"></asp:Label>
                                    </td>
                                    <td>
                                    <img src = "/graphics/new.gif" alt = "New"/>&nbsp;&nbsp;
                                    <img src = "/graphics/edit.gif" alt = "Edit"/>&nbsp;&nbsp;
                                    <img src = "/graphics/delete.gif" alt = "Delete"/>&nbsp;&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="ODBC Connection 2"></asp:Label>                                        
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="CheckBox2" runat="server" Text="Edit" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
            <div align="right">
                <asp:Button ID="Button1" runat="server" Text="New" Width="68px" CssClass="button" />
                &nbsp;&nbsp;
                <asp:Button ID="Button2" runat="server" Text="Modify" Width="70px" CssClass="button" />
                &nbsp;&nbsp;
                <asp:Button ID="Button3" runat="server" Text="View" Width="71px" CssClass="button" />
                &nbsp;&nbsp;
                <asp:Button ID="Button4" runat="server" Text="Delete" Width="71px" CssClass="button" />
                &nbsp;&nbsp;
                <asp:Button ID="Button5" runat="server" Text="Create KML" Width="90px" CssClass="button" Style="margin-right:20px" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
