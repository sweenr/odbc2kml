<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConnDetails.aspx.cs" Inherits="HCI.ConnDetails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>Connection Details</title>
    <style type="text/css" media="all">
        @import "odbcStyle.css";
    </style>
</head>
<body>
    <form id="connDetailsForm" runat="server">
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
                            <span class="connectionStyle">&nbsp;ODBC Connection 1</span>
                            <div class="full">
                                <table cellspacing="10" cellpadding="10" class="mainBox2">
                                    <tr>
                                        <td class="dbTitle">
                                            <asp:Label ID="DBLabel" runat="server" Text="Database Tables" CssClass="dbTitleSpan"></asp:Label>
                                        </td>
                                        <td class="tdSpace3">
                                        </td>
                                        <td class="dbTitle">
                                            <asp:Panel ID="DBTPanel0" runat="server" Visible="true" CssClass="dbTitle">
                                                <asp:Label ID="DBTLabel0" runat="server" Text="Table Columns" CssClass="dbTitleSpan"></asp:Label>
                                            </asp:Panel>
                                            <asp:Panel ID="DBTPanel" runat="server" Visible="false" CssClass="dbTitle">
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="mainBox3" valign="top">
                                            <asp:Button ID="DBTable1" runat="server" CssClass="selectDB" Text="Table 1" ToolTip="Table 1"
                                                OnClick="genDBTCol" CommandArgument="Table 1 Name" />
                                            <br />
                                            <asp:Button ID="DBTable2" runat="server" CssClass="selectDB2" Text="Table 2" ToolTip="Table 2"
                                                OnClick="genDBTCol" CommandArgument="Table 2 Name" />
                                            <br />
                                            <asp:Button ID="DBTable3" runat="server" CssClass="selectDB" Text="Table 3" ToolTip="Table 3"
                                                OnClick="genDBTCol" CommandArgument="Table 3 Name" />
                                            <br />
                                            <asp:Button ID="DBTable4" runat="server" CssClass="selectDB2" Text="Table 4" ToolTip="Table 4"
                                                OnClick="genDBTCol" CommandArgument="Table 4 Name" />
                                            <br />
                                        </td>
                                        <td class="tdSpace3">
                                        </td>
                                        <td class="mainBox3" valign="top">
                                            <asp:Panel ID="DBFields0" runat="server" Visible="true" CssClass="mainBox3Panel">
                                                Click on the appropriate database table to view table columns
                                            </asp:Panel>
                                            <asp:Panel ID="DBFields" runat="server" Visible="false" CssClass="mainBox3Panel">
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="mainBox">
                            <span class="connectionStyle">&nbsp;Connection Description</span>
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
                                                        <asp:Button ID="ConButton" runat="server" Text="Modify Condition" CssClass="button"
                                                            Width="135" />&nbsp;&nbsp;
                                                        <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="modCondOK1"
                                                            CancelControlID="modCondCancel1" runat="server" PopupControlID="ConPanel" ID="ModalPopupExtender7"
                                                            TargetControlID="ConButton" />
                                                        <asp:Panel ID="ConPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                            <span class="connectionStyle">&nbsp;Modify Condition</span>
                                                            <div class="mainBoxP">
                                                                <table cellspacing="0" cellpadding="5" class="mainBox2">
                                                                    <tr>
                                                                        <td>
                                                                            <div class="omainBox4">
                                                                                <table class="omainBox5" cellspacing="0" cellpadding="0">
                                                                                    <tr class="tableTRTitle">
                                                                                        <td class="tableTD">
                                                                                            Table
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            Field
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            Operator
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            Value
                                                                                        </td>
                                                                                        <td>
                                                                                            &nbsp;
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td class="tableTD">
                                                                                            TheOnlyTable
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            VehicleType
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            ==
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            Tank
                                                                                        </td>
                                                                                        <td class="textCenter">
                                                                                            <asp:Button ID="DeleteCon1" runat="server" Style="text-align: center" Text="Remove"
                                                                                                CssClass="button" ToolTip="Delete Condition" Width="80" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr class="tableTR">
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList7" runat="server" CssClass="inputDD" Width="100">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList8" runat="server" CssClass="inputDD" Width="100">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList9" runat="server" CssClass="inputDD" Width="100">
                                                                                                <asp:ListItem>==</asp:ListItem>
                                                                                                <asp:ListItem>&gt;=</asp:ListItem>
                                                                                                <asp:ListItem>&lt;=</asp:ListItem>
                                                                                                <asp:ListItem>&gt;</asp:ListItem>
                                                                                                <asp:ListItem>&lt;</asp:ListItem>
                                                                                                <asp:ListItem>between</asp:ListItem>
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:TextBox ID="TextBox7" runat="server" MaxLength="30" CssClass="inputBox" Width="150"></asp:TextBox>
                                                                                        </td>
                                                                                        <td class="textCenter">
                                                                                            <asp:Button ID="AddCond1" runat="server" Style="text-align: center" Text="Add" CssClass="button"
                                                                                                ToolTip="Add Condition" Width="80" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <div class="right" style="padding-top: 20px;">
                                                                                    <asp:Button ID="modCondOK1" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                                                    <asp:Button ID="modCondCancel1" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
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
                                                        <asp:Button ID="ConButton2" runat="server" Text="Modify Condition" CssClass="button"
                                                            Width="135" ToolTip="Modify Condition" />&nbsp;&nbsp;
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
                                                        <asp:Button ID="ConButton3" runat="server" Text="Add Condition" CssClass="button"
                                                            Width="135" ToolTip="Add Condition" />&nbsp;&nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="mainBox2">
                                <div align="right">
                                    <asp:Button ID="uploadIcon" runat="server" Text="Upload Icons" CssClass="button" />&nbsp;&nbsp;
                                    <asp:Button ID="removeIcon" runat="server" Text="Remove Icons" CssClass="button" />&nbsp;&nbsp;
                                    <asp:Button ID="addIcon" runat="server" Text="Add Icons" CssClass="button" /></div>
                                <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="btnOk"
                                    CancelControlID="btnClose" runat="server" PopupControlID="AddIconsPanel" ID="ModalPopupExtender1"
                                    TargetControlID="addIcon" />
                                <asp:Panel ID="AddIconsPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                    <div class="mainBoxP">
                                        <span class="connectionStyle">&nbsp;Icon Library</span>
                                        <table cellspacing="0" cellpadding="10" class="mainBox2">
                                            <tr>
                                                <td>
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
                                                        </tr>
                                                    </table>
                                                    <div class="right" style="padding-top: 20px;">
                                                        <asp:Button ID="btnOk" runat="server" Text="Add Icon" CssClass="button" />&nbsp;&nbsp;
                                                        <asp:Button ID="btnClose" runat="server" Text="Cancel" CssClass="button" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </asp:Panel>
                                <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="btnOk2"
                                    CancelControlID="btnClose2" runat="server" PopupControlID="RemoveIconsPanel"
                                    ID="ModalPopupExtender2" TargetControlID="RemoveIcon" />
                                <asp:Panel ID="RemoveIconsPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                    <div class="mainBoxP">
                                        <span class="connectionStyle">&nbsp;Remove Icons</span>
                                        <table cellspacing="0" cellpadding="10" class="mainBox2">
                                            <tr>
                                                <td>
                                                    <table class="boxPopupStyle2" cellpadding="5">
                                                        <tr>
                                                            <td style="width: 64px;">
                                                            </td>
                                                            <td style="width: 64px;">
                                                            </td>
                                                            <td style="width: 64px;">
                                                            </td>
                                                            <td style="width: 64px;">
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
                                                            <td style="width: 64px;">
                                                            </td>
                                                            <td style="width: 64px;">
                                                            </td>
                                                            <td style="width: 64px;">
                                                            </td>
                                                            <td style="width: 64px;">
                                                            </td>
                                                            <td style="width: 64px;">
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
                                                    <div class="right">
                                                        <asp:Button ID="btnOk2" runat="server" Text="Remove Icon" CssClass="button" />
                                                        &nbsp;&nbsp;
                                                        <asp:Button ID="btnClose2" runat="server" Text="Cancel" CssClass="button" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </asp:Panel>
                                <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="btnOk3"
                                    CancelControlID="btnClose3" runat="server" PopupControlID="UploadIconsPanel"
                                    ID="ModalPopupExtender3" TargetControlID="UploadIcon" />
                                <asp:Panel ID="UploadIconsPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                    <div class="mainBoxP">
                                        <span class="connectionStyle">&nbsp;Upload Icons</span>
                                        <table cellspacing="0" cellpadding="10" class="mainBox2">
                                            <tr>
                                                <td>
                                                    <table class="boxPopupStyle2" cellpadding="5">
                                                        <tr>
                                                            <td>
                                                                <ajax:AsyncFileUpload ID="AsyncFileUpload1" runat="server" Text="Upload Icon" BackColor="White" />
                                                                <br />
                                                                <ajax:AsyncFileUpload ID="AsyncFileUpload2" runat="server" Text="Upload Icon" BackColor="White" />
                                                                <br />
                                                                <ajax:AsyncFileUpload ID="AsyncFileUpload3" runat="server" Text="Upload Icon" BackColor="White" />
                                                                <br />
                                                                <ajax:AsyncFileUpload ID="AsyncFileUpload4" runat="server" Text="Upload Icon" BackColor="White" />
                                                                <br />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <div class="right">
                                                        <asp:Button ID="btnOk3" runat="server" Text="Upload" CssClass="button" />
                                                        &nbsp;&nbsp;
                                                        <asp:Button ID="btnClose3" runat="server" Text="Cancel" CssClass="button" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
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
                                                        <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="modOverlayOk"
                                                            CancelControlID="modOverlayCancel" runat="server" PopupControlID="ModOverlayPanel"
                                                            ID="ModalPopupExtender5" TargetControlID="ModOverlay" />
                                                        <asp:Panel ID="ModOverlayPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                            <span class="connectionStyle">&nbsp;Modify Condition</span>
                                                            <div class="mainBoxP">
                                                                <table cellspacing="0" cellpadding="5" class="mainBox2">
                                                                    <tr>
                                                                        <td>
                                                                            <div class="omainBox4">
                                                                                <table class="omainBox5" cellspacing="0" cellpadding="0">
                                                                                    <tr class="tableTRTitle">
                                                                                        <td class="tableTD">
                                                                                            Table
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            Field
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            Operator
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            Value
                                                                                        </td>
                                                                                        <td>
                                                                                            &nbsp;
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td class="tableTD">
                                                                                            TheOnlyTable
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            VehicleType
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            ==
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            Tank
                                                                                        </td>
                                                                                        <td class="textCenter">
                                                                                            <asp:Button ID="Button1" runat="server" Style="text-align: center" Text="Remove"
                                                                                                CssClass="button" ToolTip="Delete Condition" Width="80" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr class="tableTR">
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList1" runat="server" CssClass="inputDD" Width="100">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList2" runat="server" CssClass="inputDD" Width="100">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList3" runat="server" CssClass="inputDD" Width="100">
                                                                                                <asp:ListItem>==</asp:ListItem>
                                                                                                <asp:ListItem>&gt;=</asp:ListItem>
                                                                                                <asp:ListItem>&lt;=</asp:ListItem>
                                                                                                <asp:ListItem>&gt;</asp:ListItem>
                                                                                                <asp:ListItem>&lt;</asp:ListItem>
                                                                                                <asp:ListItem>between</asp:ListItem>
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:TextBox ID="TextBox1" runat="server" MaxLength="30" CssClass="inputBox" Width="150"></asp:TextBox>
                                                                                        </td>
                                                                                        <td class="textCenter">
                                                                                            <asp:Button ID="Button2" runat="server" Style="text-align: center" Text="Add" CssClass="button"
                                                                                                ToolTip="Add Condition" Width="80" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <div class="right" style="padding-top: 20px;">
                                                                                    <asp:Button ID="modOverlayOk" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                                                    <asp:Button ID="modOverlayCancel" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
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
                                                        <asp:Button ID="modOverlay2" runat="server" Text="Modify Overlay" CssClass="button" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="mainBox2">
                                <div class="right">
                                    <asp:Button ID="overlayButton" runat="server" Text="Add Overlay Color" CssClass="button" /></div>
                                <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="btnOkO"
                                    CancelControlID="btnCloseO" runat="server" PopupControlID="OverlayPanel" ID="ModalPopupExtender4"
                                    TargetControlID="overlayButton" />
                                <asp:Panel ID="OverlayPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                    <div class="mainBoxP">
                                        <span class="connectionStyle">&nbsp;Add Overlay Color</span>
                                        <table cellspacing="0" cellpadding="10" class="mainBox2">
                                            <tr>
                                                <td>
                                                    <table class="boxPopupStyle2" cellpadding="5">
                                                        <tr>
                                                            <td>
                                                                <div class="omainBox3" style="">
                                                                    <span class="overlayStyle">Color</span>
                                                                    <div class="omainBox4">
                                                                        <asp:TextBox ID="ColorPickTxtBox" runat="server"></asp:TextBox>
                                                                        <ajax:ColorPickerExtender ID="ColorPickerExtender1" runat="server" TargetControlID="ColorPickTxtBox"
                                                                            PopupButtonID="Image1" SampleControlID="colorTxtBox">
                                                                        </ajax:ColorPickerExtender>
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
                                                                                <asp:Button ID="AddBtn" runat="server" Text="Add" CssClass="button" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <div class="right">
                                                        <asp:Button ID="btnOkO" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                        <asp:Button ID="btnCloseO" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="right">
                            <input type="submit" name="submit" value="Cancel" class="button" />&nbsp;&nbsp;<input
                                type="submit" name="submit" value="Modify Connection" class="button" />
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
    <asp:ScriptManager ID="ConnSMgr2" runat="server">
    </asp:ScriptManager>
    </form>
</body>
</html>
