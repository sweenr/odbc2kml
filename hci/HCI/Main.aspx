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
            </div>
            <div id="siteInfo">
            MSU Software Engineering Senior Design Project 2010
            <br />
            <div class="right">
            <div id = "home">
             <table><tr><td>
                            <div class="homeIcon"><a href = "#" title="View Connections (Home)"></a></div></td>
                            <td><div class="newConnA"><a href = "#" title="View Connections (Home)">Connections</a></div></td></tr></table></div>
                            <div class="clear">
            </div>
            </div>
            </div>
           
        </div>
        <div id="page">
            <table id="mainTable">
                <tr>
                    <td>
                        <div class="mainBox">
                            <span class="connectionStyle">&nbsp;Connection Information</span>
                            <table cellspacing="0" cellpadding="10" class="connectionBox">
                                <tr class="oddConn">
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="ODBC Connection 1"></asp:Label>
                                    </td>
                                    <td class="connIcons">
                                    <table><tr>
                                    <td><div class="openIcon"><a href = "#" title="Open Connection"></a></div></td>
                                    <td><div class="editIcon"><a href = "#" title="Edit Connection"></a></div></td>
                                    <td><div class="deleteIcon"><a href = "#" title="Delete Connection"></a></div></td>
                                    <td><div class="kmlIcon"><a href = "#" title="Generate KML File"></a></div></td>
                                    </tr></table>
                                    </td>
                                </tr>
                                <tr class="evenConn">
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="ODBC Connection 2"></asp:Label>
                                    </td>
                                    <td class="connIcons">
                                    <table><tr>
                                    <td><div class="openIcon"><a href = "#" title="Open Connection"></a></div></td>
                                    <td><div class="editIcon"><a href = "#" title="Edit Connection"></a></div></td>
                                    <td><div class="deleteIcon"><a href = "#" title="Delete Connection"></a></div></td>
                                    <td><div class="kmlIcon"><a href = "#" title="Generate KML File"></a></div></td>
                                    </tr></table>
                                    </td>
                                </tr>
                                <tr class="oddConn">
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text="ODBC Connection 3"></asp:Label>
                                    </td>
                                    <td class="connIcons">
                                    <table><tr>
                                    <td><div class="openIcon"><a href = "#" title="Open Connection"></a></div></td>
                                    <td><div class="editIcon"><a href = "#" title="Edit Connection"></a></div></td>
                                    <td><div class="deleteIcon"><a href = "#" title="Delete Connection"></a></div></td>
                                    <td><div class="kmlIcon"><a href = "#" title="Generate KML File"></a></div></td>
                                    </tr></table>
                                    </td>
                                </tr>
                                </table>
                                
                                 <div class="newConn">
                                 <div class="right">
                            <table><tr><td>
                            <div class="newIcon"><a href = "#" title="New Connection"></a></div></td>
                            <td><div class="newConnA"><a href = "#" title="New Connection">New Connection</a></div></td></tr></table></div>
                            <div class="clear"></div></div>
                   </div>
         </td></tr></table>  
        </div>
        <div id = "footer">
        <div class="right"><img src = "graphics/polyTechW.gif" alt = "PolyTech Industries - Mississippi State University"/></div>
        </div>
</div>
    </form>
</body>
</html>
