<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConnDetails.aspx.cs" Inherits="HCI.ConnDetails" %>

<%@ Register TagPrefix="ed" Namespace="OboutInc.Editor" Assembly="obout_Editor" %>
<%@ Register TagPrefix="obout" Namespace="OboutInc.ColorPicker" Assembly="obout_ColorPicker" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>Connection Details</title>
    <style type="text/css" media="all">
        body
        {
            font: 100% Verdana, Arial, Helvetica, sans-serif;
            margin: 0; /* it's good practice to zero the margin and padding of the body element to account for differing browser defaults */
            padding: 0;
            text-align: center; /* this centers the container in IE 5* browsers. The text is then set to the left aligned default in the #container selector */
            color: #000000;
            background-color: #999;
        }
        #container
        {
            width: 468px;
            margin: 0 auto; /* the auto margins (in conjunction with a width) center the page */
            text-align: left; /* this overrides the text-align: center on the body element. */
            background-color: #999999;
            background-repeat: repeat;
        }
    </style>

    <script src="jquery/jquery-1.4.1.js" type="text/javascript"></script>

    <script src="jquery/jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>

    <link href="jquery/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
    
      $(document).ready(function() {
		   $("#uploadTabs").tabs();
		       
           if($('#odbcDBType').val() == 'Oracle') 
           { 
                $('#oracleTable').css('display', 'block'); 
           }
           else 
           { 
                $('#oracleTable').css('display', 'none');
           }
           $("#odbcDBType").change(function () {
               if($('#odbcDBType').val() == 'Oracle') 
               { 
                    $('#oracleTable').css('display', 'block'); 
               }
               else 
               { 
                    $('#oracleTable').css('display', 'none');
               }
           });
	   });

    
    </script>

    <style type="text/css" media="all">
        @import "odbcStyle.css";
    </style>

    <script type="text/JavaScript">
        function OnColorOpen(sender)
        {
          var elVPosition = "BOTTOM";
          var elHPosition = "RIGHT";
          var elTOffset = 0;
          var elLOffset = 0;
          var textBox = document.getElementById("<%= color.ClientID %>");
          sender.setColor(OboutInc.ColorPicker.getStyle(textBox,"background-color"));
          sender.setVerticalPosition  (elVPosition);
          sender.setHorizontalPosition(elHPosition);
          elTOffset.value = sender.setOffsetTop (elTOffset);
          elLOffset.value = sender.setOffsetLeft(elLOffset);
        }
    </script>

</head>
<body>
    <form id="connDetailsForm" runat="server">
    <asp:ScriptManager ID="ConnSMgr2" runat="server" />
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
                                        <a href="Main.aspx" title="View Connections (Home)">Connections</a>
                                    </div>
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
                                                        <asp:TextBox runat="server" ID="odbcName" Width="300" CssClass="inputBox" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Database Address: </span>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox runat="server" ID="odbcAdd" Width="300" CssClass="inputBox" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Port Number: </span>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox runat="server" ID="odbcPN" Width="75" CssClass="inputBox" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Database Name: </span>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox runat="server" ID="odbcDName" Width="300" CssClass="inputBox" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Username: </span>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox runat="server" ID="odbcUser" Width="300" CssClass="inputBox" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Password: </span>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox runat="server" TextMode="Password" ID="odbcPass" Width="300" CssClass="inputBox" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="connectionTitle">Database Type:</span>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="odbcDBType" runat="server">
                                                            <asp:ListItem Text="SQL"></asp:ListItem>
                                                            <asp:ListItem Text="MySQL"></asp:ListItem>
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
                                            <div class="right">
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
                            <span class="connectionStyle">&nbsp;Connection Tables/Columns</span>
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
                                    <asp:Button ID="bText" runat="server" Text=" B " CssClass="descButton" Font-Bold="True"
                                        ToolTip="Bold" />&nbsp;&nbsp;
                                    <asp:Button ID="iText" runat="server" Text=" I  " CssClass="descButton" Font-Italic="True"
                                        ToolTip="Italics" />&nbsp;&nbsp;
                                    <asp:Button ID="uText" runat="server" Text=" U " CssClass="descButton" Font-Underline="True"
                                        ToolTip="Underline" />&nbsp;&nbsp;
                                    <asp:Button ID="dLink" runat="server" Text="Add Link" CssClass="descButton" ToolTip="Add Link" />&nbsp;&nbsp;
                                    <asp:Button ID="dField" runat="server" Text="Add Field/Image" CssClass="descButton"
                                        ToolTip="Add Field/Image" />&nbsp;&nbsp;
                                    <asp:Button ID="DescPopup" runat="server" Text="Table and Field" CssClass="descbutton"
                                        ToolTipe="Set Table and Field name" />&nbsp;&nbsp;
                                    <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="DescPopupOk"
                                        CancelControlID="DescPopupCan" runat="server" PopupControlID="ConPanel" ID="ModalPopupExtender6"
                                        TargetControlID="DescPopup" />
                                    <asp:Panel ID="Panel2" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                        <span class="connectionStyle">&nbsp;Table and Field</span>
                                        <div class="mainBoxP">
                                            <div class="right" style="padding-top: 20px;">
                                                <asp:Button ID="DescPopupOk" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                <asp:Button ID="DescPopupCan" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <br />
                                    <br />
                                    <asp:TextBox ID="descriptionBox" runat="server" Width="99%" Height="250" BorderColor="#766640"
                                        TextMode="MultiLine"></asp:TextBox>
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
                                                <asp:Panel ID="IconConditionPanel" runat="server" Visible="true">
                                                </asp:Panel>
                                                <asp:Panel ID="AddIconConditionPanel" runat="server" Visible="true">
                                                </asp:Panel>
                                                <tr>
                                                    <td>
                                                        <asp:Panel ID="testPanel1" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                            <span class="connectionStyle">&nbsp;Modify Condition</span>
                                                            <div class="mainBoxP">
                                                                <table cellspacing="0" cellpadding="5" class="mainBox2">
                                                                    <tr>
                                                                        <td>
                                                                            <div class="omainBox4">
                                                                                <table class="omainBox6" cellspacing="0" cellpadding="0">
                                                                                    <tr>
                                                                                        <td>
                                                                                            Tool Directions :)
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <p>
                                                                                </p>
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
                                                                                            <asp:Button ID="Button5" runat="server" Style="text-align: center" Text="Remove"
                                                                                                CssClass="button" ToolTip="Delete Condition" Width="80" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr class="tableTR">
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList4" runat="server" CssClass="inputDD" Width="100">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList5" runat="server" CssClass="inputDD" Width="100">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList6" runat="server" CssClass="inputDD" Width="100">
                                                                                                <asp:ListItem>==</asp:ListItem>
                                                                                                <asp:ListItem>&gt;=</asp:ListItem>
                                                                                                <asp:ListItem>&lt;=</asp:ListItem>
                                                                                                <asp:ListItem>&gt;</asp:ListItem>
                                                                                                <asp:ListItem>&lt;</asp:ListItem>
                                                                                                <asp:ListItem>between</asp:ListItem>
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:TextBox ID="TextBox3" runat="server" MaxLength="30" CssClass="inputBox" Width="150"></asp:TextBox>
                                                                                        </td>
                                                                                        <td class="textCenter">
                                                                                            <asp:Button ID="Button6" runat="server" Style="text-align: center" Text="Add" CssClass="button"
                                                                                                ToolTip="Add Condition" Width="80" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <div class="right" style="padding-top: 20px;">
                                                                                    <asp:Button ID="testPanelOK1" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                                                    <asp:Button ID="testPanelCancel1" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                            <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="btnOk"
                                                CancelControlID="btnClose" runat="server" PopupControlID="AddIconsPanel" ID="ModalPopupExtender1"
                                                TargetControlID="addIcon" />
                                            <asp:Panel ID="AddIconsPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                <div class="mainBoxP">
                                                    <span class="connectionStyle">&nbsp;Icon Library</span>
                                                    <table cellspacing="0" cellpadding="5" class="mainBox2">
                                                        <tr>
                                                            <td>
                                                                <table class="boxPopupStyle2" cellpadding="5">
                                                                    <tr>
                                                                        <td colspan="8">
                                                                            <table class="omainBox6" cellspacing="0" cellpadding="0">
                                                                                <tr>
                                                                                    <td>
                                                                                        Tool Directions Go Here! Yay User Friendliness! :)
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <p>
                                                                            </p>
                                                                        </td>
                                                                    </tr>
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
                                                ID="ModalPopupExtender2" TargetControlID="removeIcon" />
                                            <asp:Panel ID="RemoveIconsPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                <div class="mainBoxP">
                                                    <span class="connectionStyle">&nbsp;Remove Icons</span>
                                                    <table cellspacing="0" cellpadding="5" class="mainBox2">
                                                        <tr>
                                                            <td>
                                                                <table class="boxPopupStyle2" cellpadding="5">
                                                                    <tr>
                                                                        <td colspan="8">
                                                                            <table class="omainBox6" cellspacing="0" cellpadding="0">
                                                                                <tr>
                                                                                    <td>
                                                                                        Tool Directions Go Here! Yay User Friendliness! :)
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <p>
                                                                            </p>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
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
                                                ID="ModalPopupExtender3" TargetControlID="uploadIcon" />
                                            <asp:Panel ID="UploadIconsPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                <div class="mainBoxP">
                                                    <span class="connectionStyle">&nbsp;Upload Icons</span>
                                                    <table cellspacing="0" cellpadding="5" class="mainBox2">
                                                        <tr>
                                                            <td>
                                                                <table class="boxPopupStyle2" cellpadding="5">
                                                                    <tr>
                                                                        <td>
                                                                            <table class="omainBox6" cellspacing="0" cellpadding="0">
                                                                                <tr>
                                                                                    <td>
                                                                                        Tool Directions
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <p>
                                                                            </p>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <div id="container">
                                                                                <% //Maybe %>
                                                                                <div id="uploadTabs" class="ui-widget">
                                                                                    <ul class="ui-tabs-nav">
                                                                                        <li><a href="#remote">Upload Icon From Link</a></li>
                                                                                        <li><a href="#local">Upload Icon Locally</a></li>
                                                                                    </ul>
                                                                                    <div id="remote" class="ui-tabs-panel">
                                                                                        <ul>
                                                                                            <li>
                                                                                                <p>
                                                                                                    Please check fetch if you would like to have the file saved on the server</p>
                                                                                                <p>
                                                                                                    <asp:CheckBox ID="fetchCheckBox" runat="server" Text="Fetch" />
                                                                                                </p>
                                                                                            </li>
                                                                                            <li>
                                                                                                <p>
                                                                                                    Please enter the URL of the icon you would like to use</p>
                                                                                                <p>
                                                                                                    <asp:TextBox ID="URLtextBox" runat="server" Width="270" />
                                                                                                    <asp:Button ID="URLcorrect" runat="server" OnClick="URLcorrectClick" Text="Save" />
                                                                                                </p>
                                                                                            </li>
                                                                                        </ul>
                                                                                    </div>
                                                                                    <div id="local" class="ui-tabs-panel">
                                                                                        <ul>
                                                                                            <li>
                                                                                                <p>
                                                                                                    Please select the icon file you wish to upload</p>
                                                                                                <p>
                                                                                                    <asp:FileUpload ID="fileUpEx" runat="server" /><br />
                                                                                                    <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmitClick" Text="Submit" />
                                                                                                </p>
                                                                                            </li>
                                                                                        </ul>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
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
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="newConnA">
                                            <asp:Button ID="Button4" runat="server" Text="Modify Overlay" CssClass="button"
                                                            Width="135" style="display: none; visibility: hidden;" />
                                                <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" 
                                                    CancelControlID="modCondCancel1" runat="server" PopupControlID="ConPanel" ID="ModalPopupExtender7"
                                                    TargetControlID="Button4" />
                                                <asp:Panel ID="ConPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                    <span class="connectionStyle">&nbsp;Modify Condition</span>
                                                    <div class="mainBoxP">
                                                        <table cellspacing="0" cellpadding="5" class="mainBox2">
                                                            <tr>
                                                                <td>
                                                                    <div class="omainBox4">
                                                                        <table class="omainBox6" cellspacing="0" cellpadding="0">
                                                                            <tr>
                                                                                <td>
                                                                                    Tool Directions Go Here! Yay User Friendliness! :)
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <p>
                                                                        </p>
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
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="right">
                                                <asp:Button ID="uploadIcon" runat="server" Text="Upload Icons" CssClass="button" />&nbsp;&nbsp;
                                                <asp:Button ID="removeIcon" runat="server" Text="Remove Icons" CssClass="button" />&nbsp;&nbsp;
                                                <asp:Button ID="addIcon" runat="server" Text="Add Icons" CssClass="button" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
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
                                                        <div class="overlayBox" style="background-color: Red;" />
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
                                                        <asp:Button ID="addOverlay1" runat="server" Text="Modify Overlay" CssClass="button"
                                                            Width="135" />&nbsp;&nbsp;
                                                        <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="modOOK1"
                                                            CancelControlID="modOCancel1" runat="server" PopupControlID="OverlayPanel" ID="ModalPopupExtender4"
                                                            TargetControlID="addOverlay1" />
                                                        <asp:Panel ID="OverlayPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                            <span class="connectionStyle">&nbsp;Modify Overlay Conditions</span>
                                                            <div class="mainBoxP">
                                                                <table cellspacing="0" cellpadding="5" class="mainBox2">
                                                                    <tr>
                                                                        <td>
                                                                            <div class="omainBox4">
                                                                                <table class="omainBox6" cellspacing="0" cellpadding="0">
                                                                                    <tr>
                                                                                        <td>
                                                                                            Tool Directions Go Here! Yay User Friendliness! :)
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <table class="omainBox5" cellspacing="0" cellpadding="0">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <!--Color Info Here (Color Box? Color Drop Down?)--->
                                                                                            <div class="colorPicker">
                                                                                                <asp:TextBox ReadOnly="true" ID="color" Style="vertical-align: middle;" runat="server" />
                                                                                                &nbsp;&nbsp;Click here:
                                                                                                <obout:ColorPicker ID="ColorPicker1" EnableViewState="true" ZIndex="500000" runat="server"
                                                                                                    OnClientOpen="OnColorOpen" TargetId="color" TargetProperty="style.backgroundColor" />
                                                                                            </div>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
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
                                                                                            TableData
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            >=
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            5.0
                                                                                        </td>
                                                                                        <td class="textCenter">
                                                                                            <asp:Button ID="Button2" runat="server" Style="text-align: center" Text="Remove"
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
                                                                                            <asp:Button ID="Button3" runat="server" Style="text-align: center" Text="Add" CssClass="button"
                                                                                                ToolTip="Add Condition" Width="80" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <div class="right" style="padding-top: 20px;">
                                                                                    <asp:Button ID="modOOK1" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                                                    <asp:Button ID="modOCancel1" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
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
                                                        <div class="overlayBox" style="background-color: Yellow">
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
                                                        <asp:Button ID="addOverlay2" runat="server" Text="Modify Overlay" CssClass="button"
                                                            Width="135" />&nbsp;&nbsp;
                                                        <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="modOOK2"
                                                            CancelControlID="modOCancel2" runat="server" PopupControlID="OverlayPanel2" ID="ModalPopupExtender5"
                                                            TargetControlID="addOverlay2" />
                                                        <asp:Panel ID="OverlayPanel2" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                            <span class="connectionStyle">&nbsp;Modify Overlay Conditions</span>
                                                            <div class="mainBoxP">
                                                                <table cellspacing="0" cellpadding="5" class="mainBox2">
                                                                    <tr>
                                                                        <td>
                                                                            <div class="omainBox4">
                                                                                <table class="omainBox6" cellspacing="0" cellpadding="0">
                                                                                    <tr>
                                                                                        <td>
                                                                                            Tool Directions Go Here! Yay User Friendliness! :)
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <p>
                                                                                </p>
                                                                                <table class="omainBox4" cellspacing="0" cellpadding="0">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <!--Color Info Here (Color Box? Color Drop Down?)--->
                                                                                            <div class="colorPicker">
                                                                                                <asp:TextBox ReadOnly="true" ID="TextBox2" Style="vertical-align: middle;" runat="server" />
                                                                                                &nbsp;&nbsp;Click here:
                                                                                                <obout:ColorPicker ID="ColorPicker2" EnableViewState="true" ZIndex="500000" runat="server"
                                                                                                    OnClientOpen="OnColorOpen" TargetId="color" TargetProperty="style.backgroundColor" />
                                                                                            </div>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <p>
                                                                                </p>
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
                                                                                            TableData
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            >=
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            5.0
                                                                                        </td>
                                                                                        <td class="textCenter">
                                                                                            <asp:Button ID="Button1" runat="server" Style="text-align: center" Text="Remove"
                                                                                                CssClass="button" ToolTip="Delete Condition" Width="80" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr class="tableTR">
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList10" runat="server" CssClass="inputDD" Width="100">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList11" runat="server" CssClass="inputDD" Width="100">
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:DropDownList ID="DropDownList12" runat="server" CssClass="inputDD" Width="100">
                                                                                                <asp:ListItem>==</asp:ListItem>
                                                                                                <asp:ListItem>&gt;=</asp:ListItem>
                                                                                                <asp:ListItem>&lt;=</asp:ListItem>
                                                                                                <asp:ListItem>&gt;</asp:ListItem>
                                                                                                <asp:ListItem>&lt;</asp:ListItem>
                                                                                                <asp:ListItem>between</asp:ListItem>
                                                                                            </asp:DropDownList>
                                                                                        </td>
                                                                                        <td class="tableTD">
                                                                                            <asp:TextBox ID="TextBox4" runat="server" MaxLength="30" CssClass="inputBox" Width="150"></asp:TextBox>
                                                                                        </td>
                                                                                        <td class="textCenter">
                                                                                            <asp:Button ID="Button7" runat="server" Style="text-align: center" Text="Add" CssClass="button"
                                                                                                ToolTip="Add Condition" Width="80" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <div class="right" style="padding-top: 20px;">
                                                                                    <asp:Button ID="Button8" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                                                                    <asp:Button ID="Button9" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="right">
                                                <asp:Button ID="AddOverlayButton" runat="server" Text="Add Overlay" CssClass="button" />
                                                &nbsp;&nbsp;
                                            </div>
                                        </td>
                                    </tr>
                                </table>
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
    <asp:Panel ID="scriptHandler" runat="server" Visible="true">
    </asp:Panel>
    <asp:Panel ID="errorPanel1" runat="server" Visible="true" Style="color: White">
    </asp:Panel>
    </form>
</body>
</html>
