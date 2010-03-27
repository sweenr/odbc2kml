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
function OnColorOpen(sender){
  var textBox = document.getElementById("<%= ColorAddText.ClientID %>");
  sender.setColor(OboutInc.ColorPicker.getStyle(textBox,"background-color"));
}
function OnColorPicked(sender){
  var hidden  =document.getElementById("<%=HiddenValue.ClientID %>");
  hidden.value=sender.getColor();
}
    </script>

</head>
<body>
    <a href="#" style="display: none; visibility: hidden;" onclick="return false" id="dummyLink"
        runat="server">na</a>
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
                                                <asp:Label ID="invalidConnInfo" runat="server" Visible="false" Text="Required fields must be completed!" />&nbsp;&nbsp;
                                                <asp:Label ID="unableToConnect" runat="server" Visible="false" Text="Unable to connect to the selected database!" />&nbsp;&nbsp;
                                                <asp:Label ID="connectionEstablished" runat="server" Visible="false" Text="Successfully connected to the database!" />&nbsp;&nbsp;
                                                <asp:Button runat="server" ID="connectButton" OnClick="updateConnection" Text="Connect" CssClass="button"/>
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
                                            <asp:Panel ID="connectionTables" Visible="true" runat="server" CssClass="dbColumns">
                                            </asp:Panel>
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
                                        ToolTip="Set Table and Field name" />&nbsp;&nbsp;
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
                                            </table>
                                            <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true"
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
                                                                </table>
                                                                <asp:Panel ID="addIconToLibary" Height="300px" ScrollBars="Vertical" runat="server" Visible="true">
                                                                </asp:Panel>
                                                                <div class="right" style="padding-top: 20px;">
                                                                    <!-- <asp:Button ID="btnOk" runat="server" Text="Add Icon" CssClass="button" OnClick="addIconFromLibraryToConn" CommandArgument="none"/>&nbsp;&nbsp; -->
                                                                    <asp:Button ID="btnClose" runat="server" Text="Cancel" CssClass="button" />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>
                                            <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" 
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
                                                                </table>
                                                                <asp:Panel ID="removeIconFromConn"  Height="300px" ScrollBars="Vertical" runat="server" Visible="true">
                                                                </asp:Panel>
                                                                <div class="right">
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
                                                                                                    <asp:Button ID="URLsubmit" runat="server" OnClick="URLsubmitClick" Text="Save" />
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
                                                <asp:Panel ID="OverlayConditionPanel" runat="server" Visible="true"></asp:Panel>
                                            </table>

                                            <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" CancelControlID="closeRemoveOverlay"
                                                runat="server" PopupControlID="RemoveOverlayPanel" ID="RemoveOverlayPopupExtender" TargetControlID="RemoveOverlayButton" />
                                            <asp:Panel ID="RemoveOverlayPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                <div class="mainBoxP">
                                                    <span class="connectionStyle">&nbsp;Remove Overlay Color</span>
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
                                                                </table>
                                                                <asp:Panel ID="removeOverlayInteriorPanel" runat="server" Visible="true">
                                                                </asp:Panel>
                                                                <div class="right" style="padding-top: 20px;">
                                                                    <asp:Button ID="closeRemoveOverlay" runat="server" Text="Cancel" CssClass="button" />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>
                                            <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" CancelControlID="closeAddOverlay"
                                                    runat="server" PopupControlID="AddOverlayPanel" ID="AddOverlayPopupExtender" TargetControlID="AddOverlayButton" />
                                                <asp:Panel ID="AddOverlayPanel" runat="server" CssClass="boxPopupStyle" Style="display: none;">
                                                    <div class="mainBoxP">
                                                        <span class="connectionStyle">&nbsp;Overlay Color Library</span>
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
                                                                    </table>
                                                                    <asp:HiddenField runat="server"  id="HiddenValue" value="" />
                                                                    <asp:label id="addColorMessage" runat="server"  Text="&nbsp;" /> 
                                                                    <table class="omainBox5" cellspacing="0" cellpadding="0">
                                                                        <tr>
                                                                            <td>
                                                                                <!--Color Info Here (Color Box? Color Drop Down?)--->
                                                                                <div class="colorPicker">
                                                                                    &nbsp;&nbsp;Click here:
                                                                                    <obout:ColorPicker ID="ColorPicker1" runat="server" ZIndex="500000" OnClientOpen="OnColorOpen" OnClientPicked="OnColorPicked"
                                                                                        TargetId="ColorAddText" TargetProperty="style.backgroundColor">
                                                                                        <asp:TextBox ReadOnly="true" ID="ColorAddText" Style="cursor: pointer;" runat="server" />
                                                                                    </obout:ColorPicker>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <div class="right" style="padding-top: 20px;">
                                                                        <asp:Label ID="overColorExists" runat="server" Visible="false" Text="Overlay Color Exists! Please Choose Another:" />
                                                                        <asp:Button ID="submitAddOverlay" runat="server" Text="Submit" CssClass="button" OnClick="addOverlayColorToConn" />
                                                                        <asp:Button ID="closeAddOverlay" runat="server" Text="Cancel" CssClass="button" />
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
                                            <div class="right">
                                                <asp:Button ID="RemoveOverlayButton" runat="server" Text="Remove Overlay" CssClass="button" />
                                                &nbsp;&nbsp;
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
                            <input type="submit" name="submit" value="Cancel" class="button" />&nbsp;&nbsp;
                            <asp:Button ID="saveConn" runat="server" Text="Save Connection" OnClick="modifyConnection" CssClass="button"/>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="footer">
            <div class="right">
                <img src="graphics/polyTechW.gif" alt="PolyTech Industries - Mississippi State University" />
            </div>
        </div>
    </div>
    <asp:Panel ID="scriptHandler" runat="server" Visible="true">
    </asp:Panel>
    <asp:Panel ID="errorPanel1" runat="server" Visible="true" Style="color: White">
    </asp:Panel>
    </form>
</body>
</html>
