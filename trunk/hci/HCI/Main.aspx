<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="HCI.Main" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Main Page</title>
    
    <style type="text/css" media="all">
        body
        {
            font: 100% Verdana, Arial, Helvetica, sans-serif;
            margin: 0; /* it's good practice to zero the margin and padding of the body element to account for differing browser defaults */
            padding: 0;
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
	$(function() {
		$("#errorPanel1").dialog({
			bgiframe: true,
			modal: true,
			autoOpen: false,
			title: 'Error!',
			resizable: false,
			dialogClass: 'alert',
			buttons: {
				Ok: function() {
					$(this).dialog('close');
				}
			}
		});
	});
    </script>
    
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

    <script type="text/javascript">
    $(document).ready(function(){
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
        
        if($('#editConnDBType').val() == 'Oracle') 
        { 
            $('#odbcTable1').css('display', 'block'); 
        }
        else 
        { 
            $('#odbcTable1').css('display', 'none');
        }
        $("#editConnDBType").change(function () {
           if($('#editConnDBType').val() == 'Oracle') 
           { 
                $('#odbcTable1').css('display', 'block'); 
           }
           else 
           { 
                $('#odbcTable1').css('display', 'none');
           }
        });
    });
    </script>

    <link href="jquery/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
    
    <style type="text/css" media="all">
        @import "odbcStyle.css";
    </style>
</head>
<body>
    <a href="#" style="display: none; visibility: hidden;" onclick="return false" id="dummyLink"
        runat="server">na</a>
        <a href="#" style="display: none; visibility: hidden;" onclick="return false" id="dummyLink2"
        runat="server">na</a>
         <a href="#" style="display: none; visibility: hidden;" onclick="return false" id="dummyLink3"
        runat="server">na</a>
        <a href="#" style="display: none; visibility: hidden;" onclick="return false" id="dummyLink4"
        runat="server">na</a>
    <form id="mainForm" runat="server">
    <asp:ScriptManager ID="ConnSMgr" runat="server">
    </asp:ScriptManager>
    <asp:Panel ID="errorPanel1" runat="server" Visible="true" Style="color: White">
    </asp:Panel>
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
            <table id="mainTable" cellspacing="2">
                <tr>
                    <td style="width:900px;">
                        <div class="mainBoxMP">
                            <span class="connectionStyle">&nbsp;Connection Information</span>
                                <asp:Panel ID="ConnectionsAvailable" runat="server" Visible="true">
                                </asp:Panel>
                            <div class="newConn">
                                <div class="right">
                                    <table style="text-align:right;">
                                        <tr>
                                            <td>
                                                <asp:Button ID="uploadIcon" runat="server" Text="Upload Icons" CssClass="buttonMain" />
                                                <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnClose3"
                                                            runat="server" PopupControlID="UploadIconsPanel" ID="ModalPopupExtender3" TargetControlID="uploadIcon" />
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
                                                                                                <td style="text-align:center">
                                                                                                    Icon types are limited to bmp, png, jpg, png, tif and a max size of 128 x 128 pixels
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
                                                                                                                <asp:Button ID="URLsubmit" runat="server" OnClick="URLsubmitClick" Text="Save" CssClass="button" />
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
                                                                                                            </p>
                                                                                                            <br />
                                                                                                            <br />
                                                                                                            <div align="right">
                                                                                                                <p>
                                                                                                                    <asp:Button ID="uploadSubmit" runat="server" OnClick="uploadClick" Text="Submit"
                                                                                                                        CssClass="button" />
                                                                                                                </p>
                                                                                                            </div>
                                                                                                        </li>
                                                                                                    </ul>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <div class="right">
                                                                                &nbsp;&nbsp;
                                                                                <asp:Button ID="btnClose3" runat="server" Text="Cancel" CssClass="button" />
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </asp:Panel>
                                            </td>
                                           
                                            <td>
                                                <asp:Button ID="viewIconLib" runat="server" CssClass="buttonMain3" ToolTip="View Icon Library"
                                                    Text="View Icon Library" OnClick="viewIconLibFunc"></asp:Button>
                                                <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="viewLibClose"
                                                    runat="server" PopupControlID="iconLibMainPanel" ID="IconLibModalPopup" TargetControlID="dummyLink3" />
                                                <asp:Panel ID="iconLibMainPanel" runat="server" Style="display: none;" CssClass="boxPopupStyle">
                                                    <div class="mainBoxP">
                                                        <span class="connectionStyle">&nbsp;Icon Library</span>
                                                        <table cellspacing="0" cellpadding="5" class="mainBox2">
                                                            <tr>
                                                                <td>
                                                                    <asp:Panel ID="iconLibPanel" Height="300px" ScrollBars="Both" runat="server" Visible="true">
                                                                    </asp:Panel>
                                                                    <div class="right" style="padding-top: 20px;">
                                                                        <asp:Button ID="viewLibClose" runat="server" Text="Close" CssClass="button" />
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </asp:Panel>
                                            </td>
                                            <td>
                                                <div class="newConnA">
                                                    <asp:Button ID="newConnectionA" runat="server" CssClass="buttonMain2" ToolTip="New Connection" Text="New Connection" OnClick="launchNewConnection"></asp:Button>
                                                    <asp:ImageButton ID="newConnection" runat="server" CssClass="newIcon" ImageUrl="graphics/connIcon.gif"
                                                    AlternateText="Create Connection" ToolTip="New Connection" OnClick="launchNewConnection" Visible="false"/>
                                           
                                                    <!-- Sample Extender for New Connection Button --->
                                                    <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="newConnCancel"
                                                        runat="server" PopupControlID="newConnPanel" ID="NewConn1ModalPopUp" TargetControlID="dummyLink2" />
                                                    <!-- Sample Panels for Connection Pop-Ups --->
                                                    <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="cancelDelConn"
                                                        runat="server" PopupControlID="deleteConnPanel" ID="deletePopupExtender" TargetControlID="dummyLink" />
                                                    <asp:Panel ID="newConnPanel" runat="server" Style="display: none;" CssClass="boxPopupStyle">
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
                                                                                        <span class="connectionTitle">Database Name: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="odbcDNameE" runat="server" CssClass="inputBox" Width="350"></asp:TextBox>
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
                                                                                <tr id="odbcInfoRow">
                                                                                    <td colspan="2">
                                                                                        <span class="connectionTitle">&nbsp; &nbsp; Only Service Name or Service ID is required, not both</span>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <div class="right" style="padding-top: 20px;">
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
                                                    <ajax:ModalPopupExtender runat="server" PopupControlID="connUpdateWarning"
                                                        TargetControlID="dummyLink4" ID="warningModal" BackgroundCssClass="modalBackground"
                                                        DropShadow="false">
                                                    </ajax:ModalPopupExtender>
                                                    <asp:Panel ID="connUpdateWarning" runat="server" Visible="true" Style="display: none">
                                                        <div class='mainBoxP'>
                                                            <table class="omainBox6" cellspacing="0" cellpadding="0">
                                                                <tr>
                                                                    <td>
                                                                        If there are any differences between the original database and the one being updated,
                                                                        description and any conditions associated with tables no longer present will be
                                                                        removed from the connection. Are you sure you would like to update the connection
                                                                        information?
                                                                        <br />
                                                                        <br />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <asp:Button ID="continueUpdate" runat="server" Text="Yes" OnCommand="updateConnection"
                                                                CssClass="button" />
                                                            <asp:Button ID="cancelUpdateWarning" runat="server" Text="No" OnClick="cancelUpdateConnection" CssClass="button" />
                                                        </div>
                                                    </asp:Panel>
                                                    <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="false" 
                                                        runat="server" PopupControlID="editConnPanel" ID="editConnModalPopUp" TargetControlID="dummyLink3" CancelControlID="cancelEditConn" />
                                                    <asp:Panel ID="editConnPanel" runat="server" Style="display: none;" CssClass="boxPopupStyle">
                                                        <div class="mainBoxP">
                                                            <span id="Span1" visible="true" class="connectionStyle">&nbsp;Connection Information</span>
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
                                                                                        <asp:TextBox ID="editConnName" runat="server" CssClass="inputBox" Width="350"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Database Address: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="editConnDBAddr" runat="server" CssClass="inputBox" Width="350"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Port Number: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="editConnDBPort" runat="server" CssClass="inputBox" Width="100"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Database Name: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="editConnDBName" runat="server" CssClass="inputBox" Width="350"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Username: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="editConnUser" runat="server" CssClass="inputBox" Width="200"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Password: </span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="editConnPass" runat="server" CssClass="inputBox" Width="200" TextMode="Password"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <span class="connectionTitle">Database Type:</span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:DropDownList ID="editConnDBType" runat="server">
                                                                                            <asp:ListItem Text="MySQL"></asp:ListItem>
                                                                                            <asp:ListItem Text="MSSQL"></asp:ListItem>
                                                                                            <asp:ListItem Text="Oracle"></asp:ListItem>
                                                                                        </asp:DropDownList>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <table cellspacing="5" id="odbcTable1" style="display: none">
                                                                                <tr id="odbcProtocolRow1">
                                                                                    <td>
                                                                                        <span class="connectionTitle">Protocol:</span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox runat="server" ID="editOracleProtocol" Width="300" CssClass="inputBox" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="odbcServiceRow1">
                                                                                    <td>
                                                                                        <span class="connectionTitle">Service Name:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox runat="server" ID="editOracleService" Width="300" CssClass="inputBox" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="odbcSIDRow1">
                                                                                    <td>
                                                                                        <span class="connectionTitle">Service ID:</span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox runat="server" ID="editOracleSID" Width="300" CssClass="inputBox" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="Tr4">
                                                                                    <td colspan="2">
                                                                                        <span class="connectionTitle">&nbsp; &nbsp; Only Service Name or Service ID is required, not both</span>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <div class="right" style="padding-top: 20px;">
                                                                                <asp:Button ID="saveEditConn" runat="server" Text="Save" CssClass="button" ToolTip="Update"
                                                                                    CommandName="saveConn" OnCommand="editAndSaveConnectionInformation" CommandArgument="0" />
                                                                                &nbsp;&nbsp;
                                                                                <asp:Button ID="saveAndEditConn" runat="server" Text="Save and Edit" CssClass="button" ToolTip="Update"
                                                                                    OnCommand="editAndSaveConnectionInformation" CommandArgument="2" CommandName="saveAndEditConn" />
                                                                                &nbsp;&nbsp;
                                                                                <asp:Button ID="cancelEditConn" runat="server" Text="Cancel" CssClass="button" ToolTip="Cancel"/>
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
    </form>
</body>
</html>
