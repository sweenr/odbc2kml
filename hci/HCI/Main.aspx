<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="HCI.Main" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
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
    <div id="wrapIt">
        <div id="header">
            <div id="logo">
            </div>
            <div id="siteInfo">
                MSU Software Engineering Senior Design Project 2010
                <br />
                <div class="right">
                    <div id="home">
                        <table>
                            <tr>
                                <td>
                                    <asp:ImageButton ID="homeIcon" runat="server" CssClass="homeIcon" ImageUrl="graphics/connIcon.gif"
                                        AlternateText="Connections" ToolTip="Connections" PostBackUrl="Main.aspx" />
                                </td>
                                <td>
                                    <div class="newConnA">
                                        <a href="Main.aspx" title="View Connections (Home)">Connections</a></div>
                                </td>
                            </tr>
                        </table>
                    </div>
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
                                        <asp:Label ID="Conn1" runat="server" Text="ODBC Connection 1"></asp:Label>
                                        <a href="#" title="Open Connection"></a>
                                    </td>
                                    <td class="connIcons">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:ImageButton ID="openConn1" runat="server" CssClass="openIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Open Connection" ToolTip="Open Connection" />
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="editConn1" runat="server" CssClass="editIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Edit Connection" ToolTip="Edit Connection"/>
                                                        
                                                    <!-- Sample Extender for 1st Edit Connection Button --->
                                                    <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="editConnUpdate"
                                                        CancelControlID="editConnCancel" runat="server" PopupControlID="editConnPanel"
                                                        ID="ModalPopupExtender1" TargetControlID="editConn1" />
                                                        
                                                    <!-- Sample Panels for Connection Pop-Ups --->
                                                    <asp:Panel ID="editConnPanel" runat="server" cssClass="boxPopupStyle" Style="display: none;">
                                                        <div class="mainBoxP">
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
                                                                                        <asp:TextBox ID="odbcNameE" runat="server" CssClass="inputBox" Width="350"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Database Name: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="odbcDNameE" runat="server" CssClass="inputBox" Width="350"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Database Address: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="odbcDatabaseE" runat="server" CssClass="inputBox" Width="350"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Port Number: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="odbcPNE" runat="server" CssClass="inputBox" Width="100"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Username: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="odbcUserE" runat="server" CssClass="inputBox" Width="200"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Password: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="odbcPWE" runat="server" CssClass="inputBox" Width="200" TextMode="Password"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            
                                                                            <div class="right" style="padding-top:20px;">
                                                                                <asp:Button ID="editConnUpdate" runat="server" Text="Update" CssClass="button" ToolTip="Update" />
                                                                                &nbsp;&nbsp;
                                                                                <asp:Button ID="editConnCancel" runat="server" Text="Cancel" CssClass="button" ToolTip="Cancel" />
                                                                            </div>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </asp:Panel>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="deleteConn1" runat="server" CssClass="deleteIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Delete Connection" ToolTip="Delete Connection" />
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="genKML1" runat="server" CssClass="kmlIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Generate KML File" ToolTip="Generate KML File" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr class="evenConn">
                                    <td>
                                        <asp:Label ID="Conn2" runat="server" Text="ODBC Connection 2"></asp:Label>
                                    </td>
                                    <td class="connIcons">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:ImageButton ID="openConn2" runat="server" CssClass="openIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Open Connection" ToolTip="Open Connection" />
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="editConn2" runat="server" CssClass="editIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Edit Connection" ToolTip="Edit Connection" />
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="deleteConn2" runat="server" CssClass="deleteIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Delete Connection" ToolTip="Delete Connection" />
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="genKML2" runat="server" CssClass="kmlIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Generate KML File" ToolTip="Generate KML File" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr class="oddConn">
                                    <td>
                                        <asp:Label ID="Conn3" runat="server" Text="ODBC Connection 3"></asp:Label>
                                    </td>
                                    <td class="connIcons">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:ImageButton ID="openConn3" runat="server" CssClass="openIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Open Connection" ToolTip="Open Connection" />
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="editConn3" runat="server" CssClass="editIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Edit Connection" ToolTip="Edit Connection" />
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="deleteConn3" runat="server" CssClass="deleteIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Delete Connection" ToolTip="Delete Connection" />
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="genKML3" runat="server" CssClass="kmlIcon" ImageUrl="graphics/connIcon.gif"
                                                        AlternateText="Generate KML File" ToolTip="Generate KML File" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <div class="newConn">
                                <div class="right">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:ImageButton ID="newIcon" runat="server" CssClass="newIcon" ImageUrl="graphics/connIcon.gif"
                                                    AlternateText="Create Connection" ToolTip="Create Connection" />
                                            </td>
                                            <td>
                                                <div class="newConnA">
                                                    <a href="#" title="New Connection">New Connection</a></div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div class="clear">
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="footer">
            <div class="right">
                <img src="graphics/polyTechW.gif" alt="PolyTech Industries - Mississippi State University" /></div>
        </div>
    </div>
    <asp:ScriptManager ID="ConnSMgr" runat="server">
    </asp:ScriptManager>
    </form>
</body>
</html>
