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
<form id="mainForm" runat="server">
    <!-- #include file ="layout\top.inc" -->
           
                        <div class="mainBox">
                            <span class="connectionStyle">&nbsp;Connection Information</span>
                            <table cellspacing="0" cellpadding="10" class="connectionBox">
                                <tr class="oddConn">
                                    <td>
                                        <asp:Label ID="Conn1" runat="server" Text="ODBC Connection 1"></asp:Label>
                                        <a href = "#" title="Open Connection"></a>
                                    </td>
                                    <td class="connIcons">
                                    <table><tr>
                                    <td>
                                        <asp:ImageButton ID="openConn1" runat="server" CssClass="openIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Open Connection" ToolTip="Open Connection" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="editConn1" runat="server" CssClass="editIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Edit Connection" ToolTip="Edit Connection" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="deleteConn1" runat="server" CssClass="deleteIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Delete Connection" ToolTip="Delete Connection" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="genKML1" runat="server" CssClass="kmlIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Generate KML File" ToolTip="Generate KML File" />
                                    </td>
                                    </tr></table>
                                    </td>
                                </tr>
                                <tr class="evenConn">
                                    <td>
                                        <asp:Label ID="Conn2" runat="server" Text="ODBC Connection 2"></asp:Label>
                                    </td>
                                    <td class="connIcons">
                                    <table><tr>
                                    <td>
                                        <asp:ImageButton ID="openConn2" runat="server" CssClass="openIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Open Connection" ToolTip="Open Connection" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="editConn2" runat="server" CssClass="editIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Edit Connection" ToolTip="Edit Connection" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="deleteConn2" runat="server" CssClass="deleteIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Delete Connection" ToolTip="Delete Connection" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="genKML2" runat="server" CssClass="kmlIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Generate KML File" ToolTip="Generate KML File" />
                                    </td>
                                    </tr></table>
                                    </td>
                                </tr>
                                <tr class="oddConn">
                                    <td>
                                        <asp:Label ID="Conn3" runat="server" Text="ODBC Connection 3"></asp:Label>
                                    </td>
                                    <td class="connIcons">
                                    <table><tr>
                                    <td>
                                        <asp:ImageButton ID="openConn3" runat="server" CssClass="openIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Open Connection" ToolTip="Open Connection" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="editConn3" runat="server" CssClass="editIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Edit Connection" ToolTip="Edit Connection" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="deleteConn3" runat="server" CssClass="deleteIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Delete Connection" ToolTip="Delete Connection" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="genKML3" runat="server" CssClass="kmlIcon" 
                                            ImageUrl="graphics/connIcon.gif" AlternateText="Generate KML File" ToolTip="Generate KML File" />
                                    </td>
                                    </tr></table>
                                    </td>
                                </tr>
                                </table>
                                 <div class="newConn">
                                 <div class="right">
                            <table><tr>
                            <td>
                            <asp:ImageButton ID="newIcon" runat="server" CssClass="newIcon"
                            ImageUrl="graphics/connIcon.gif" AlternateText="Create Connection" ToolTip="Create Connection"/>
                            </td>
                            <td><div class="newConnA"><a href = "#" title="New Connection">New Connection</a></div></td></tr></table></div>
                            <div class="clear"></div></div>
                   </div>
        <!-- #include file ="layout\bottom.inc" -->
        
                            <asp:Panel ID="EditConn" runat="server" class="boxPopupStyle" Style="display: none;">        
                                <div class="mainBox">
                            <span class="connectionStyle">&nbsp;Connection Information</span>
                            <table cellspacing="0" cellpadding="10" class="mainBox2">
                                <tr>
                                    <td>
                                        <div style="background-color: white; padding: 5px;">
                                            <table cellspacing="5">
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Connection Name: </span>
                                                    </td>
                                                    <td>
                                                        <input type="text" name="odbcName" id="odbcName" size="50" class="inputBox" title=""
                                                            value="ODBC2KML Connection 1" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Database Address: </span>
                                                    </td>
                                                    <td>
                                                        <input type="text" name="odbcLoc" id="odbcLoc" size="50" class="inputBox" title=""
                                                            value="Database Address" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Port Number: </span>
                                                    </td>
                                                    <td>
                                                        <input type="text" name="odbcLoc" id="odbcPN" size="20" class="inputBox" title=""
                                                            value="Port #" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Database Name: </span>
                                                    </td>
                                                    <td>
                                                        <input type="text" name="odbcLoc" id="odbcDName" size="50" class="inputBox" title=""
                                                            value="Database Name" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Username: </span>
                                                    </td>
                                                    <td>
                                                        <input type="text" name="odbcLoc" id="odbcUser" size="50" class="inputBox" title=""
                                                            value="Username" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Password: </span>
                                                    </td>
                                                    <td>
                                                        <input type="password" name="odbcLoc" id="odbcPassword" size="50" class="inputBox"
                                                            title="" value="Password" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <div align="right">
                                                <input type="submit" name="submit" value="Connect" class="button" />
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            </div>
                            </asp:Panel>
                        
    </form>
</body>
</html>
