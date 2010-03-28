<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="HCI.Main" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Main Page</title>
    <script src="jquery/jquery-1.4.1.js" type="text/javascript"></script>

    <script src="jquery/jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>

    <link href="jquery/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
    
    <style type="text/css" media="all">
        @import "odbcStyle.css";
    </style>
</head>
<body>
    <a href="#" style="display: none; visibility: hidden;" onclick="return false" id="dummyLink"
        runat="server">na</a>
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
                                        AlternateText="View Connections (Home)" ToolTip="View Connections (Home)" PostBackUrl="Main.aspx" />
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
                                <asp:Panel ID="ConnectionsAvailable" runat="server" Visible="true">
                                </asp:Panel>
                            </table>
                            <div class="newConn">
                                <div class="right">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:ImageButton ID="newConnection" runat="server" CssClass="newIcon" ImageUrl="graphics/connIcon.gif"
                                                    AlternateText="Create Connection" ToolTip="New Connection" />
                                            </td>
                                            <td>
                                                <div class="newConnA">
                                                    <asp:HyperLink ID="newConnectionA" runat="server" ToolTip="New Connection">New Connection</asp:HyperLink>
                                                    <!-- Sample Extender for New Connection Button --->
                                                    <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" CancelControlID="newConnCancel"
                                                        runat="server" PopupControlID="newConnPanel" ID="NewConn1ModalPopUp" TargetControlID="newConnection" />
                                                    <!-- Sample Panels for Connection Pop-Ups --->
                                                    <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" CancelControlID="cancelDelConn"
                                                        runat="server" PopupControlID="deleteConnPanel" ID="deletePopupExtender" TargetControlID="dummyLink" />
                                                    <asp:Panel ID="deleteConnPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                        <div class="mainBoxP">
                                                            <span id="DelSpan" visible="true" class="connectionStyle">&nbsp;Delete Connection</span>
                                                            <table cellspacing="0" cellpadding="10" class="mainBox2">
                                                                <tr>
                                                                    <td>
                                                                    <div style="background-color: white; padding: 5px;">
                                                                            <table cellspacing="5">
                                                                            <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="connToDelete" runat="server" visible="true"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        <div class="right" style="padding-top: 20px;">
                                                                            <asp:Button ID="delConnBtn" runat="server" Text="Delete" CssClass="button" ToolTip="Delete"
                                                                                OnClick="deleteConnFunction" CommandArgument="none" />
                                                                            &nbsp;&nbsp;
                                                                            <asp:Button ID="cancelDelConn" runat="server" Text="Cancel" CssClass="button" ToolTip="Cancel" />
                                                                        </div>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </asp:Panel>
                                                    <asp:Panel ID="newConnPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                        <div class="mainBoxP">
                                                            <span id="validConn" visible="true" class="connectionStyle">&nbsp;Connection Information</span>
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
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Database Type:</span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:DropDownList ID="odbcDBType" runat="server">
                                                                                            <asp:ListItem Text="MySQL"></asp:ListItem>
                                                                                            <asp:ListItem Text="MSSQL"></asp:ListItem>
                                                                                            <asp:ListItem Text="Oracle"></asp:ListItem>
                                                                                        </asp:DropDownList>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <table cellspacing="5" id="oracleTable" style="display: none">
                                                                                <tr id="odbcProtocolRow">
                                                                                    <td>
                                                                                        <span class="connectionTitle">Protocol:</span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox runat="server" ID="odbcProtocol" Width="300" CssClass="inputBox" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="odbcSNameRow">
                                                                                    <td>
                                                                                        <span class="connectionTitle">Service Name:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox runat="server" ID="odbcSName" Width="300" CssClass="inputBox" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="odbcSIDRow">
                                                                                    <td>
                                                                                        <span class="connectionTitle">Service ID:</span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox runat="server" ID="odbcSID" Width="300" CssClass="inputBox" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <div class="right" style="padding-top: 20px;">
                                                                                <asp:Literal ID="validNewConn" runat="server" Visible="false" Text="All fields must be completeted!"></asp:Literal>
                                                                                &nbsp;&nbsp;
                                                                                <asp:Button ID="newConnUpdate" runat="server" Text="Create" CssClass="button" ToolTip="Update"
                                                                                    OnClick="createConnection" CommandArgument="none" />
                                                                                &nbsp;&nbsp;
                                                                                <asp:Button ID="newConnCancel" runat="server" Text="Cancel" CssClass="button" ToolTip="Cancel" />
                                                                            </div>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </asp:Panel>
                                                </div>
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
    <asp:Panel ID="errorPanel1" runat="server" Visible="true" Style="color: White">
    </asp:Panel>
    <asp:ScriptManager ID="ConnSMgr" runat="server">
    </asp:ScriptManager>
    </form>
</body>
</html>
