<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Modify.aspx.cs" Inherits="_Modify" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ODBC2KML</title>
    <style type="text/css" media="all">
        @import "odbcStyle.css";
    </style>
</head>
<body>
    <form id="form2" runat="server">
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
            <table cellpadding="10" width="100%">
                <tr>
                    <td>
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
                        <br />
                        <br />
                        <div class="mainBox">
                            <span class="connectionStyle">&nbsp;Database Tables</span>
                            <div style="width: 100%;">
                                <table cellspacing="10" cellpadding="20" class="mainBox2">
                                    <tr>
                                        <td class="mainBox3" valign="top">
                                            <span class="selectedDatabase">Database Table 1</span><br />
                                            <br />
                                            <span class="selectDatabase">Database Table 2</span><br />
                                            <br />
                                            <span class="selectDatabase">Database Table 3</span><br />
                                            <br />
                                            <span class="selectDatabase">Database Table 4</span><br />
                                            <br />
                                        </td>
                                        <td style="width: 10%;">
                                        </td>
                                        <td class="mainBox3" valign="top">
                                            <table class="tableClass">
                                                <tr>
                                                    <td>
                                                        Table 1 Column 1
                                                        <br />
                                                        <br />
                                                        Table 1 Column 2
                                                        <br />
                                                        <br />
                                                        Table 1 Column 3
                                                        <br />
                                                        <br />
                                                        Table 1 Column 4
                                                        <br />
                                                        <br />
                                                        Table 1 Column 5
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <div align="right">
                                                <input type="submit" name="submit" value="Preview Data" class="button" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="mainBox">
                            <span style="color: white; font-weight: bold;">&nbsp;Connection Description</span>
                            <br />
                            <div class="mainBox4">
                                <div style="background-color: white; padding: 5px;">
                                    This description is for this database connection. This description will also contain
                                    a URL link to view sample data in the selected table.
                                </div>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="mainBox">
                            <span class="connectionStyle">&nbsp;Icons</span>
                            <br />
                            <div class="mainBox2">
                                <table width="100%">
                                    <tr class="mainBox5" align="center">
                                        <td>
                                            <table width="100%">
                                                <tr>
                                                    <td class="iconBox">
                                                        <img src="icons/cycling.png" alt="" />
                                                    </td>
                                                    <td class="conditionsBox">
                                                        <div class="conditionsBoxStyle">
                                                            <table cellpadding="10">
                                                                <tr>
                                                                    <td>
                                                                        Condition 1<br />
                                                                        Condition 2<br />
                                                                        Condition 3<br />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                    <td class="buttonClass">
                                                        <asp:Button ID="DavidsButton" runat="server" Text="Modify Condition" CssClass="button" />&nbsp;&nbsp;
                                                        <cc1:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="modCondOK1"
                                                            CancelControlID="modCondCancel1" runat="server" PopupControlID="DavidsPanel"
                                                            ID="ModalPopupExtender7" TargetControlID="DavidsButton" /> 
                                                        <asp:Panel ID="DavidsPanel" runat="server" class="boxPopupStyle" Style="display: none;">
                                                            <span class="connectionStyle">&nbsp;Modify Condition</span>
                                                                <div class="boxPopupStyle1">
                                                                    <table class="boxPopupStyle2" cellpadding="5">
                                                                        <tr>
                                                                            <td>            
                                                                            <br />
                                                                            <br />
                                                                            <div class="omainBox3">
                                                                                <span class="overlayStyle">Conditions</span>
                                                                                <br />
                                                                                <table border="1" width="100%" class="omainBox4">
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            Table
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Field
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Operator
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Value
                                                                                        </td>
                                                                                        <td>
                                                                                            &nbsp;
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            TheOnlyTable
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            VehicleType
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            ==
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Tank
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:Button ID="Button4" runat="server" Style="text-align: center" Text="Delete" class="button" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList7" runat="server">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList8" runat="server">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList9" runat="server">
                                                                                                <asp:ListItem>==</asp:ListItem>
                                                                                                <asp:ListItem>&gt;=</asp:ListItem>
                                                                                                <asp:ListItem>&lt;=</asp:ListItem>
                                                                                                <asp:ListItem>&gt;</asp:ListItem>
                                                                                                <asp:ListItem>&lt;</asp:ListItem>
                                                                                                <asp:ListItem>between</asp:ListItem>
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center; margin-left: 80px">
                                                                                            <asp:TextBox ID="TextBox7" runat="server" MaxLength="30"></asp:TextBox>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:Button ID="Button5" runat="server" Text="Add" class="button" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <div align="right">
                                                                    <asp:Button ID="ModCondOK1" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                                    <asp:Button ID="ModCondCancel1" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                                                </div>
                                                            </div>
                                          <!-- ----------------------------------------------------------->                                  
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="iconBox">
                                                        <img src="icons/bus.png" alt="" />
                                                    </td>
                                                    <td class="conditionsBox">
                                                        <div class="conditionsBoxStyle">
                                                            <table cellpadding="10">
                                                                <tr>
                                                                    <td>
                                                                        Condition 1<br />
                                                                        Condition 2<br />
                                                                        Condition 3<br />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                    <td class="buttonClass">
                                                        <asp:Button ID="DavidsButton2" runat="server" Text="Modify Condition" CssClass="button" />&nbsp;&nbsp;
                                                        <cc1:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="modCondOK2"
                                                            CancelControlID="modCondCancel2" runat="server" PopupControlID="DavidsPanel2"
                                                            ID="ModalPopupExtender8" TargetControlID="DavidsButton2" /> 
                                                        <asp:Panel ID="DavidsPanel2" runat="server" class="boxPopupStyle" Style="display: none;">
                                                            <span class="connectionStyle">&nbsp;Modify Condition</span>
                                                                <div class="boxPopupStyle1">
                                                                    <table class="boxPopupStyle2" cellpadding="5">
                                                                        <tr>
                                                                            <td>            
                                                                            <br />
                                                                            <br />
                                                                            <div class="omainBox3">
                                                                                <span class="overlayStyle">Conditions</span>
                                                                                <br />
                                                                                <table border="1" width="100%" class="omainBox4">
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            Table
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Field
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Operator
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Value
                                                                                        </td>
                                                                                        <td>
                                                                                            &nbsp;
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            TheOnlyTable
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            VehicleType
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            ==
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Jeep
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:Button ID="Button9" runat="server" Style="text-align: center" Text="Delete" class="button" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList10" runat="server">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList11" runat="server">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList12" runat="server">
                                                                                                <asp:ListItem>==</asp:ListItem>
                                                                                                <asp:ListItem>&gt;=</asp:ListItem>
                                                                                                <asp:ListItem>&lt;=</asp:ListItem>
                                                                                                <asp:ListItem>&gt;</asp:ListItem>
                                                                                                <asp:ListItem>&lt;</asp:ListItem>
                                                                                                <asp:ListItem>between</asp:ListItem>
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center; margin-left: 80px">
                                                                                            <asp:TextBox ID="TextBox8" runat="server" MaxLength="30"></asp:TextBox>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:Button ID="Button10" runat="server" Text="Add" class="button" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <div align="right">
                                                                    <asp:Button ID="ModCondOK2" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                                    <asp:Button ID="ModCondCancel2" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="iconBox">
                                                        <img src="icons/electronics.png" alt="" />
                                                    </td>
                                                    <td class="conditionsBox">
                                                        <div class="conditionsBoxStyleEmpty">
                                                            <table cellpadding="10">
                                                                <tr>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                    <td class="buttonClass">
                                                        <asp:Button ID="DavidsButton3" runat="server" Text="Modify Condition" CssClass="button" />&nbsp;&nbsp;
                                                        <cc1:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="modCondOK3"
                                                            CancelControlID="modCondCancel3" runat="server" PopupControlID="DavidsPanel3"
                                                            ID="ModalPopupExtender9" TargetControlID="DavidsButton3" /> 
                                                        <asp:Panel ID="DavidsPanel3" runat="server" class="boxPopupStyle" Style="display: none;">
                                                            <span class="connectionStyle">&nbsp;Modify Condition</span>
                                                                <div class="boxPopupStyle1">
                                                                    <table class="boxPopupStyle2" cellpadding="5">
                                                                        <tr>
                                                                            <td>            
                                                                            <br />
                                                                            <br />
                                                                            <div class="omainBox3">
                                                                                <span class="overlayStyle">Conditions</span>
                                                                                <br />
                                                                                <table border="1" width="100%" class="omainBox4">
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            Table
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Field
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Operator
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Value
                                                                                        </td>
                                                                                        <td>
                                                                                            &nbsp;
                                                                                        </td>
                                                                                    </tr>

                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList13" runat="server">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList14" runat="server">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList15" runat="server">
                                                                                                <asp:ListItem>==</asp:ListItem>
                                                                                                <asp:ListItem>&gt;=</asp:ListItem>
                                                                                                <asp:ListItem>&lt;=</asp:ListItem>
                                                                                                <asp:ListItem>&gt;</asp:ListItem>
                                                                                                <asp:ListItem>&lt;</asp:ListItem>
                                                                                                <asp:ListItem>between</asp:ListItem>
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center; margin-left: 80px">
                                                                                            <asp:TextBox ID="TextBox9" runat="server" MaxLength="30"></asp:TextBox>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:Button ID="Button14" runat="server" Text="Add" class="button" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <div align="right">
                                                                    <asp:Button ID="ModCondOK3" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                                    <asp:Button ID="ModCondCancel3" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                                                </div>
                                                            </div>                                                                            
                                                        </asp:Panel>
                                                   <!-- ----------------------------------------------------------->
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div align="right" class="mainBox2">
                                <asp:Button ID="uploadIcon" runat="server" Text="Upload Icons" CssClass="button" />&nbsp;&nbsp;
                                <asp:Button ID="removeIcon" runat="server" Text="Remove Icons" CssClass="button" />&nbsp;&nbsp;
                                <asp:Button ID="addIcon" runat="server" Text="Add Icons" CssClass="button" />&nbsp;&nbsp;
                                <cc1:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="btnOk"
                                    CancelControlID="btnClose" runat="server" PopupControlID="AddIconsPanel" ID="ModalPopupExtender1"
                                    TargetControlID="AddIcon" />
                                <asp:Panel ID="AddIconsPanel" runat="server" class="boxPopupStyle" Style="display: none;">
                                    <span class="connectionStyle">&nbsp;Icon Library</span>
                                    <div class="boxPopupStyle1">
                                        <table class="boxPopupStyle2" cellpadding="5">
                                            <tr>
                                                <td>
                                                    <img src="icons/1.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/2.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/3.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/4.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/5.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/6.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/7.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/8.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/9.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/10.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/A.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/airports.png" alt="" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <img src="icons/arrow-reverse.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/arrow.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/arts.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/B.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/bars.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/blu-blank.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/blu-circle.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/blu-diamond.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/blu-square.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/blu-stars.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/blue-pushpin.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/bus.png" alt="" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <img src="icons/C.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/cabs.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/camera.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/campfire.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/campground.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/caution.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/coffee.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/convenience.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/cycling.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/D.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/dining.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/dollar.png" alt="" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <img src="icons/donut.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/E.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/earthquake.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/electronics.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/euro.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/F.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/falling_rocks.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/ferry.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/firedept.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/fishing.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/flag.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/forbidden.png" alt="" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div align="right">
                                            <asp:Button ID="btnOk" runat="server" Text="Add Icon" CssClass="button" />&nbsp;&nbsp;
                                            <asp:Button ID="btnClose" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                        </div>
                                    </div>
                                </asp:Panel>
                                <cc1:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="btnOk2"
                                    CancelControlID="btnClose2" runat="server" PopupControlID="RemoveIconsPanel"
                                    ID="ModalPopupExtender2" TargetControlID="RemoveIcon" />
                                <asp:Panel ID="RemoveIconsPanel" runat="server" class="boxPopupStyle" Style="display: none;">
                                    <span class="connectionStyle">&nbsp;Remove Icons</span>
                                    <div class="boxPopupStyle1">
                                        <table class="boxPopupStyle2" cellpadding="5">
                                            <tr>
                                                <td width="64">
                                                </td>
                                                <td width="64">
                                                </td>
                                                <td width="64">
                                                </td>
                                                <td width="64">
                                                </td>
                                                <td>
                                                    <img src="icons/cycling.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/bus.png" alt="" />
                                                </td>
                                                <td>
                                                    <img src="icons/electronics.png" alt="" />
                                                </td>
                                                <td width="64">
                                                </td>
                                                <td width="64">
                                                </td>
                                                <td width="64">
                                                </td>
                                                <td width="64">
                                                </td>
                                                <td width="64">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                        <div align="right">
                                            <asp:Button ID="btnOk2" runat="server" Text="Remove Icon" CssClass="button" />&nbsp;&nbsp;
                                            <asp:Button ID="btnClose2" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                        </div>
                                    </div>
                                </asp:Panel>
                                <cc1:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="btnOk3"
                                    CancelControlID="btnClose3" runat="server" PopupControlID="UploadIconsPanel"
                                    ID="ModalPopupExtender3" TargetControlID="UploadIcon" />
                                <asp:Panel ID="UploadIconsPanel" runat="server" class="boxPopupStyle" Style="display: none;">
                                    <span class="connectionStyle">&nbsp;Upload Icons</span>
                                    <div class="boxPopupStyle1">
                                        <table class="boxPopupStyle2" cellpadding="5">
                                            <tr>
                                                <td>
                                                    <cc1:AsyncFileUpload ID="AsyncFileUpload1" runat="server" Text="Upload Icon" BackColor="White" />
                                                    <br />
                                                    <cc1:AsyncFileUpload ID="AsyncFileUpload2" runat="server" Text="Upload Icon" BackColor="White" />
                                                    <br />
                                                    <cc1:AsyncFileUpload ID="AsyncFileUpload3" runat="server" Text="Upload Icon" BackColor="White" />
                                                    <br />
                                                    <cc1:AsyncFileUpload ID="AsyncFileUpload4" runat="server" Text="Upload Icon" BackColor="White" />
                                                    <br />
                                                </td>
                                            </tr>
                                        </table>
                                        <div align="right">
                                            <asp:Button ID="btnOk3" runat="server" Text="Upload" CssClass="button" />&nbsp;&nbsp;
                                            <asp:Button ID="btnClose3" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="mainBox">
                            <span class="connectionStyle">&nbsp;Overlay Colors</span>
                            <br />
                            <div class="mainBox2">
                                <table width="100%">
                                    <tr class="mainBox5" align="center">
                                        <td>
                                            <table width="100%">
                                                <tr>
                                                    <td class="iconBox">
                                                        <div class="overlayBox" style="background-color: Red;">
                                                        </div>
                                                    </td>
                                                    <td class="conditionsBox">
                                                        <div class="conditionsBoxStyle">
                                                            <table cellpadding="10">
                                                                <tr>
                                                                    <td>
                                                                        Condition 1<br />
                                                                        Condition 2<br />
                                                                        Condition 3<br />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                    <td class="buttonClass">
                                                        <cc1:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="modOverlayOk"
                                                            CancelControlID="modOverlayCancel" runat="server" PopupControlID="ModOverlayPanel"
                                                            ID="ModalPopupExtender5" TargetControlID="ModOverlay" />
                                                        <asp:Panel ID="ModOverlayPanel" runat="server" class="boxPopupStyle" Style="display: none;">
                                                            <span class="connectionStyle">&nbsp;Modify Overlay</span>
                                                            <div class="boxPopupStyle1">
                                                                <table class="boxPopupStyle2" cellpadding="5">
                                                                    <tr>
                                                                        <td>
                                                                            <div class="omainBox3" style="">
                                                                                <span class="overlayStyle">Color</span>
                                                                                <div class="omainBox4">
                                                                                    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                                                                                    <cc1:ColorPickerExtender ID="ColorPickerExtender2" runat="server" TargetControlID="ColorPickTxtBox"
                                                                                        PopupButtonID="Image1" SampleControlID="colorTxtBox">
                                                                                    </cc1:ColorPickerExtender>
                                                                                    &nbsp;<asp:TextBox ID="TextBox2" runat="server" Height="20px" Width="20px"></asp:TextBox>
                                                                                    &nbsp;<asp:Image ID="Image2" runat="server" ImageUrl="~/cp_button.png" ImageAlign="AbsMiddle" />
                                                                                </div>
                                                                            </div>
                                                                            <br />
                                                                            <br />
                                                                            <div class="omainBox3">
                                                                                <span class="overlayStyle">Conditions</span>
                                                                                <br />
                                                                                <table border="1" width="100%" class="omainBox4">
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            Table
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Field
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Operator
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Value
                                                                                        </td>
                                                                                        <td>
                                                                                            &nbsp;
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            TheOnlyTable
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            VehicleType
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            ==
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Tank
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:Button ID="DelBtn" runat="server" Style="text-align: center" Text="Delete" class="button" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList1" runat="server">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList2" runat="server">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList3" runat="server">
                                                                                                <asp:ListItem>==</asp:ListItem>
                                                                                                <asp:ListItem>&gt;=</asp:ListItem>
                                                                                                <asp:ListItem>&lt;=</asp:ListItem>
                                                                                                <asp:ListItem>&gt;</asp:ListItem>
                                                                                                <asp:ListItem>&lt;</asp:ListItem>
                                                                                                <asp:ListItem>between</asp:ListItem>
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center; margin-left: 80px">
                                                                                            <asp:TextBox ID="TextBox3" runat="server" MaxLength="30"></asp:TextBox>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:Button ID="Button2" runat="server" Text="Add" class="button" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <div align="right">
                                                                    <asp:Button ID="modOverlayOK" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                                    <asp:Button ID="modOverlayCancel" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
                                                        <asp:Button ID="ModOverlay" runat="server" Text="Modify Overlay" CssClass="button" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="iconBox">
                                                        <div class="overlayBox" style="background-color: Yellow;">
                                                        </div>
                                                    </td>
                                                    <td class="conditionsBox">
                                                        <div class="conditionsBoxStyle">
                                                            <table cellpadding="10">
                                                                <tr>
                                                                    <td>
                                                                        Condition 1<br />
                                                                        Condition 2<br />
                                                                        Condition 3<br />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                    <td class="buttonClass">
                                                        <cc1:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="modOverlayOk2"
                                                            CancelControlID="modOverlayCancel2" runat="server" PopupControlID="ModOverlayPanel2"
                                                            ID="ModalPopupExtender6" TargetControlID="modOverlay2" />
                                                        <asp:Panel ID="modOverlayPanel2" runat="server" class="boxPopupStyle" Style="display: none;">
                                                            <span class="connectionStyle">&nbsp;Modify Overlay</span>
                                                            <div class="boxPopupStyle1">
                                                                <table class="boxPopupStyle2" cellpadding="5">
                                                                    <tr>
                                                                        <td>
                                                                            <div class="omainBox3" style="">
                                                                                <span class="overlayStyle">Color</span>
                                                                                <div class="omainBox4">
                                                                                    <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox>
                                                                                    <cc1:ColorPickerExtender ID="ColorPickerExtender3" runat="server" TargetControlID="ColorPickTxtBox"
                                                                                        PopupButtonID="Image1" SampleControlID="colorTxtBox">
                                                                                    </cc1:ColorPickerExtender>
                                                                                    &nbsp;<asp:TextBox ID="TextBox5" runat="server" Height="20px" Width="20px"></asp:TextBox>
                                                                                    &nbsp;<asp:Image ID="Image3" runat="server" ImageUrl="~/cp_button.png" ImageAlign="AbsMiddle" />
                                                                                </div>
                                                                            </div>
                                                                            <br />
                                                                            <br />
                                                                            <div class="omainBox3">
                                                                                <span class="overlayStyle">Conditions</span>
                                                                                <br />
                                                                                <table border="1" width="100%" class="omainBox4">
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            Table
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Field
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Operator
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Value
                                                                                        </td>
                                                                                        <td>
                                                                                            &nbsp;
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            AnotherTable
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            VehicleType
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            ==
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            Jeep
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:Button ID="Button3" runat="server" Style="text-align: center" Text="Delete"
                                                                                                class="button" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList4" runat="server">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList5" runat="server">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:DropDownList ID="DropDownList6" runat="server">
                                                                                                <asp:ListItem>==</asp:ListItem>
                                                                                                <asp:ListItem>&gt;=</asp:ListItem>
                                                                                                <asp:ListItem>&lt;=</asp:ListItem>
                                                                                                <asp:ListItem>&gt;</asp:ListItem>
                                                                                                <asp:ListItem>&lt;</asp:ListItem>
                                                                                                <asp:ListItem>between</asp:ListItem>
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td style="text-align: center; margin-left: 80px">
                                                                                            <asp:TextBox ID="TextBox6" runat="server" MaxLength="30"></asp:TextBox>
                                                                                        </td>
                                                                                        <td style="text-align: center">
                                                                                            <asp:Button ID="Button1" runat="server" Text="Add" class="button" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <div align="right">
                                                                    <asp:Button ID="modOverlayOk2" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                                    <asp:Button ID="modOverlayCancel2" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
                                                        <asp:Button ID="modOverlay2" runat="server" Text="Modify Overlay" CssClass="button" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div align="right" class="mainBox2">
                                <asp:Button ID="overlayButton" runat="server" Text="Add Overlay Color" CssClass="button" />&nbsp;&nbsp;
                                <cc1:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="btnOkO"
                                    CancelControlID="btnCloseO" runat="server" PopupControlID="OverlayPanel" ID="ModalPopupExtender4"
                                    TargetControlID="overlayButton" />
                                <asp:Panel ID="OverlayPanel" runat="server" class="boxPopupStyle" Style="display: none;">
                                    <span class="connectionStyle">&nbsp;Add Overlay Color</span>
                                    <div class="boxPopupStyle1">
                                        <table class="boxPopupStyle2" cellpadding="5">
                                            <tr>
                                                <td>
                                                    <div class="omainBox3" style="">
                                                        <span class="overlayStyle">Color</span>
                                                        <div class="omainBox4">
                                                            <asp:TextBox ID="ColorPickTxtBox" runat="server"></asp:TextBox>
                                                            <cc1:ColorPickerExtender ID="ColorPickerExtender1" runat="server" TargetControlID="ColorPickTxtBox"
                                                                PopupButtonID="Image1" SampleControlID="colorTxtBox">
                                                            </cc1:ColorPickerExtender>
                                                            &nbsp;<asp:TextBox ID="colorTxtBox" runat="server" Height="20px" Width="20px"></asp:TextBox>
                                                            &nbsp;<asp:Image ID="Image1" runat="server" ImageUrl="~/cp_button.png" ImageAlign="AbsMiddle" />
                                                        </div>
                                                    </div>
                                                    <br />
                                                    <br />
                                                    <div class="omainBox3">
                                                        <span class="overlayStyle">Conditions</span>
                                                        <br />
                                                        <table border="1" width="100%" class="omainBox4">
                                                            <tr>
                                                                <td style="text-align: center">
                                                                    Table
                                                                </td>
                                                                <td style="text-align: center">
                                                                    Field
                                                                </td>
                                                                <td style="text-align: center">
                                                                    Operator
                                                                </td>
                                                                <td style="text-align: center">
                                                                    Value
                                                                </td>
                                                                <td>
                                                                    &nbsp;
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="text-align: center">
                                                                    <asp:DropDownList ID="TableDrpDwn" runat="server">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td style="text-align: center">
                                                                    <asp:DropDownList ID="FieldDrpDwn" runat="server">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td style="text-align: center">
                                                                    <asp:DropDownList ID="OperatorDrpDown" runat="server">
                                                                        <asp:ListItem>==</asp:ListItem>
                                                                        <asp:ListItem>&gt;=</asp:ListItem>
                                                                        <asp:ListItem>&lt;=</asp:ListItem>
                                                                        <asp:ListItem>&gt;</asp:ListItem>
                                                                        <asp:ListItem>&lt;</asp:ListItem>
                                                                        <asp:ListItem>between</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td style="text-align: center; margin-left: 80px">
                                                                    <asp:TextBox ID="ValueTxtBox" runat="server" MaxLength="30"></asp:TextBox>
                                                                </td>
                                                                <td style="text-align: center">
                                                                    <asp:Button ID="AddBtn" runat="server" Text="Add" class="button" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <div align="right">
                                            <asp:Button ID="btnOkO" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                            <asp:Button ID="btnCloseO" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div align="right">
                            <input type="submit" name="submit" value="Cancel" class="button" />&nbsp;&nbsp;<input
                                type="submit" name="submit" value="Modify Connection" class="button" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    </form>
</body>
</html>
