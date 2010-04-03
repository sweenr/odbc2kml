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

        function stopRKey(evt) { 
          var evt = (evt) ? evt : ((event) ? event : null); 
          var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null); 
          if ((evt.keyCode == 13) && ((node.type=="text") || (node.type=="password")))  {return false;} 
        } 

        document.onkeypress = stopRKey; 

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
    <a href="#" style="display: none; visibility: hidden;" onclick="return false" id="dummyLink"
        runat="server">na</a>
    <form id="connDetailsForm" runat="server">
    <asp:ScriptManager ID="ConnSMgr2" runat="server" EnablePartialRendering="true" />
    <asp:UpdatePanel runat="server" ID="errorUpdatePanel" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="errorPanel1" runat="server" Visible="true" Style="color: White">
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:SqlDataSource ID="SQLTables_Mapping" runat="server"></asp:SqlDataSource>
    <asp:SqlDataSource ID="ColGen" runat="server"></asp:SqlDataSource>
    <asp:SqlDataSource ID="oracleTables_Mapping" runat="server"></asp:SqlDataSource>
    <asp:SqlDataSource ID="MSQLTables_Mapping" runat="server"></asp:SqlDataSource>
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
                                                <tr id="odbcInfoRow">
                                                    <td colspan="2">
                                                        <span class="connectionTitle">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Only Service Name
                                                            or Service ID is required, not both</span>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div align="right">
                                                <asp:Label ID="invalidConnInfo" runat="server" Visible="false" Text="Required fields must be completed!" />&nbsp;&nbsp;
                                                <asp:Label ID="unableToConnect" runat="server" Visible="false" Text="Unable to connect to the selected database!" />&nbsp;&nbsp;
                                                <asp:Label ID="connectionEstablished" runat="server" Visible="false" Text="Successfully connected to the database!" />&nbsp;&nbsp;
                                                <asp:Button runat="server" ID="connectButton" Text="Update" CssClass="button" />
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
                                <asp:Panel ID="viewLatLongErrorPanel" runat="server" Visible="false" BackColor="#D1DDF1" HorizontalAlign="Center">
                                    <table style="text-align:left;" width="100%">
                                        <tr>
                                            <td width="30%" rowspan="3" style="text-align:center;">
                                                <asp:Label ID="curMappingLabel3" runat="server" Text="Current Mapping" CssClass="descLabel" Visible="true" />
                                            </td>
                                            <td>
                                                <asp:Label ID="viewLatLongErrorLabel" runat="server" CssClass="descLabelError"
                                                        Text="There are not lat/long mappings for this connection." HorizontalAlign="Center" />
                                            </td>
                                            <td width="30%">&nbsp;</td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:Panel ID="viewLatLongPanel" runat="server" Visible="false" BackColor="#D1DDF1" HorizontalAlign="Center">
                                    <table style="text-align:left;" width="100%">
                                        <tr>
                                            <td width="30%" rowspan="4" style="text-align:center;">
                                                <asp:Label ID="curMappingLabel1" runat="server" Text="Current Mapping" CssClass="descLabel" Visible="true" />
                                            </td>
                                            <td>
                                                <asp:Label ID="viewTableLabel" runat="server" Text="Table: " CssClass="descLabel" />
                                            </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;<asp:Label ID="currentTableLabel" runat="server" Text="" CssClass="descLabel2" />
                                            </td>
                                            <td width="30%" rowspan="4">
                                                <% if (Request.QueryString.Get("locked") != "true")
                                                   { %>
                                                    <asp:Button ID="removeCurMappingButton1" runat="server" Text="Remove" CssClass="button" 
                                                            OnClick="removeCurrentMapping" ToolTip="Remove" />
                                                <% } else { %>
                                                    $nbsp;
                                                <% } %>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="viewLatLabel" runat="server" Text="Latitude Field: " CssClass="descLabel" />
                                            </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;<asp:Label ID="currentLatLabel" runat="server" Text="" CssClass="descLabel2" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="viewLongLabel" runat="server" Text="Longitude Field: " CssClass="descLabel" />
                                            </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;<asp:Label ID="currentLongLabel" runat="server" Text="" CssClass="descLabel2" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="viewNameLabel" runat="server" Text="Placemark Field: " CssClass="descLabel" />
                                            </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;<asp:Label ID="currentNameLabel" runat="server" Text="" CssClass="descLabel2" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:Panel ID="viewLatLongPanel2" runat="server" Visible="false" BackColor="#D1DDF1" HorizontalAlign="Center">
                                    <table style="text-align:left;" width="100%">
                                        <tr>
                                            <td width="30%" rowspan="3" style="text-align:center;">
                                                <asp:Label ID="curMappingLabel2" runat="server" Text="Current Mapping" CssClass="descLabel" Visible="true" />
                                            </td>
                                            <td>
                                                <asp:Label ID="viewTableLabel2" runat="server" Text="Table: " CssClass="descLabel" />
                                            </td>
                                            <td>
                                                <asp:Label ID="currentTableLabel2" runat="server" Text="" CssClass="descLabel2" />
                                            </td>
                                            <td width="30%" rowspan="2">
                                                <% if (Request.QueryString.Get("locked") != "true")
                                                { %>
                                                    <asp:Button ID="removeCurMappingButton2" runat="server" Text="Remove" CssClass="button" 
                                                            OnClick="removeCurrentMapping" ToolTip="Remove" />
                                                <% } else { %>
                                                    $nbsp;
                                                <% } %>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="viewLatLongLabel" runat="server" Text="" CssClass="descLabel" />
                                            </td>
                                            <td>
                                                <asp:Label ID="currentLatLongLabel" runat="server" Text="" CssClass="descLabel2" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
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
                                        <td class="mainBox3" valign="top" align="left">
                                            <asp:GridView ID="GridViewTables" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                                CellPadding="4" ForeColor="#333333" GridLines="None" PageSize="10" ShowHeader="False"
                                                Width="100%" OnSelectedIndexChanged="GridViewTables_SelectedIndexChanged" DataKeyNames="TABLE_NAME">
                                                <RowStyle BackColor="#EFF3FB" />
                                                <Columns>
                                                    <asp:BoundField DataField="TABLE_NAME" HeaderText="TABLE_NAME" ShowHeader="False"
                                                        SortExpression="TABLE_NAME" />
                                                    <asp:CommandField ShowSelectButton="True">
                                                        <ItemStyle Width="50px" />
                                                    </asp:CommandField>
                                                </Columns>
                                                <FooterStyle BackColor="#3C6A8E" Font-Bold="True" ForeColor="White" />
                                                <PagerStyle BackColor="#3C6A8E" ForeColor="White" HorizontalAlign="Center" />
                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <EditRowStyle BackColor="#2461BF" />
                                                <AlternatingRowStyle BackColor="White" />
                                            </asp:GridView>
                                            <asp:HiddenField ID="selectedGVTable" runat="server" />
                                        </td>
                                        <td class="tdSpace3">
                                        </td>
                                        <td class="mainBox3" valign="top" align="left">
                                            <asp:Panel ID="mapPlacemarkName" runat="server" Visible="false">
                                                <br />
                                                <table class="omainBox4" cellspacing="0" cellpadding="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="mapPlacemarkLabel" runat="server" Text="Map Placemark Name: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table class="descPanelTable" cellspacing="0" cellpadding="0">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Panel ID="Panel2" runat="server" Visible="true">
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="fieldLabelPM" runat="server" Text="Field: " CssClass="descLabel"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:UpdatePanel ID="nameColumnUP" runat="server" UpdateMode="Conditional">
                                                                                            <ContentTemplate>
                                                                                                <asp:DropDownList ID="nameColumnDD" runat="server">
                                                                                                </asp:DropDownList>
                                                                                            </ContentTemplate>
                                                                                        </asp:UpdatePanel>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:Button ID="savePlacemarkMapping" runat="server" Text="Submit" CssClass="descButton" ToolTip="Submit" OnClick="savePlacemarkMapping_click" /> 
                                                                                    </td>
                                                                                </tr>                                                                                
                                                                            </table>
                                                                            <p>
                                                                            </p>
                                                                            <asp:Label ID="mapSuccess2" runat="server" CssClass="descLabel" Text="Placemark name mapped successfully." Visible="false"></asp:Label>
                                                                        </asp:Panel>
                                                                        
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <asp:Panel ID="mapColumnsPanel" runat="server" Visible="false">
                                                <br />
                                                <table class="omainBox4" cellspacing="0" cellpadding="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="mapLabel" runat="server" Text="Map Lat/Long: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                                                            <asp:Button ID="mapSeparate" runat="server" Text="Separately" CssClass="descButton"
                                                                ToolTip="Separately" OnClick="mapSeparate_Click" />&nbsp;&nbsp;
                                                            <asp:Button ID="mapTogether" runat="server" Text="Together" CssClass="descButton"
                                                                ToolTip="Together" OnClick="mapTogether_Click" />&nbsp;&nbsp;<p>
                                                                </p>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table class="descPanelTable" cellspacing="0" cellpadding="0">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Panel ID="LLSepPanel" runat="server" Visible="true">
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="latLabel" runat="server" Text="Latitude: " CssClass="descLabel"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:UpdatePanel ID="latUP" runat="server" UpdateMode="Conditional">
                                                                                            <ContentTemplate>
                                                                                                <asp:DropDownList ID="latDD" runat="server">
                                                                                                </asp:DropDownList>
                                                                                            </ContentTemplate>
                                                                                        </asp:UpdatePanel>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="longLabel" runat="server" Text="Longitude: " CssClass="descLabel"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:UpdatePanel ID="longUP" runat="server" UpdateMode="Conditional">
                                                                                            <ContentTemplate>
                                                                                                <asp:DropDownList ID="longDD" runat="server">
                                                                                                </asp:DropDownList>
                                                                                            </ContentTemplate>
                                                                                        </asp:UpdatePanel>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <p>
                                                                            </p>
                                                                            <asp:Label ID="mapError1" runat="server" CssClass="descLabelError" Text="Please selected different columns for latitude and longitude."
                                                                                Visible="false"></asp:Label>
                                                                        </asp:Panel>
                                                                        <asp:Panel ID="LLTogetherPanel" runat="server" Visible="false">
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="latLongLabel" runat="server" Text="Latitude/Longitude: " CssClass="descLabel"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:UpdatePanel ID="llUP" runat="server" UpdateMode="Conditional">
                                                                                            <ContentTemplate>
                                                                                                <asp:DropDownList ID="llDD" runat="server">
                                                                                                </asp:DropDownList>
                                                                                            </ContentTemplate>
                                                                                        </asp:UpdatePanel>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan="2">
                                                                                        <asp:RadioButtonList ID="latLongRadioList" runat="server">
                                                                                            <asp:ListItem ID="LatLongCheck" runat="server" Value="Map column as Lat/Long" />
                                                                                            <asp:ListItem ID="LongLatCheck" runat="server" Value="Map column as Long/Lat" />
                                                                                        </asp:RadioButtonList>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <p>
                                                                            </p>
                                                                            <asp:Label ID="mapError2" runat="server" CssClass="descLabelError" Text="Please select either 'Map column as Lat/Long' or 'Map column as Long/Lat'."
                                                                                Visible="false"></asp:Label>
                                                                        </asp:Panel>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <asp:Panel ID="tblColumnsPanel" runat="server">
                                                <br />
                                                <asp:GridView ID="GridViewColumns" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                                    CellPadding="4" DataSourceID="ColGen" ForeColor="#333333" GridLines="None" PageSize="10"
                                                    ShowHeader="False" Width="100%" OnPageIndexChanged="GridViewColumns_PageIndexChanged">
                                                    <RowStyle BackColor="#EFF3FB" />
                                                    <Columns>
                                                        <asp:BoundField DataField="COLUMN_NAME" HeaderText="COLUMN_NAME" ShowHeader="False"
                                                            SortExpression="COLUMN_NAME" />
                                                    </Columns>
                                                    <FooterStyle BackColor="#3C6A8E" Font-Bold="True" ForeColor="White" />
                                                    <PagerStyle BackColor="#3C6A8E" ForeColor="White" HorizontalAlign="Center" />
                                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                    <EditRowStyle BackColor="#2461BF" />
                                                    <AlternatingRowStyle BackColor="White" />
                                                </asp:GridView>
                                            </asp:Panel>
                                            <asp:Panel ID="columnMessage" runat="server" Visible="true">
                                                <asp:Label ID="selectTableMessage" runat="server" Text="Select a database table to view the table's columns and latitude/longitude information."
                                                    CssClass="descLabel"></asp:Label>
                                                <p>
                                                </p>
                                            </asp:Panel>
                                            <asp:Panel ID="columnButtons" runat="server" Visible="false">
                                                <p>
                                                </p>
                                                &nbsp;&nbsp;<asp:Label ID="mapSuccess" runat="server" CssClass="descLabel"></asp:Label>
                                                <p>
                                                </p>
                                                <div class="right">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Button ID="saveLatLong" runat="server" Text="Save" ToolTip="Save" CssClass="button"
                                                                    OnClick="saveLatLong_Click" Visible="false" />
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="addPlacemarkField" runat="server" Text="Map Name" ToolTip="Map Placemark Name"
                                                                    CssClass="button" OnClick="addPlacemarkField_Click" />
                                                                <asp:Button ID="addLatLong" runat="server" Text="Map Lat/Long" ToolTip="Map Lat/Long"
                                                                    CssClass="button" OnClick="addLatLong_Click" />
                                                                <asp:Button ID="viewGrid" runat="server" Text="Return" ToolTip="Return" Visible="false"
                                                                    CssClass="button" OnClick="viewGrid_Click" />
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="viewTable" runat="server" Text="View Table" ToolTip="View Table"
                                                                    CssClass="button" CausesValidation="False" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
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
                                <% if (Request.QueryString.Get("locked") != "true")
                                   { %>
                                <div style="background-color: white; padding: 5px; text-align: left;">
                                    <asp:Label ID="dLabel" runat="server" Text="Insert: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                                    <asp:Button ID="dLink" runat="server" Text="Link" CssClass="descButton" ToolTip="Insert Link"
                                        OnClick="dLink_Click" />&nbsp;&nbsp;
                                    <asp:Button ID="dTable" runat="server" Text="Table" CssClass="descButton" ToolTip="Insert Table"
                                        OnClick="dTable_Click" />&nbsp;&nbsp;
                                    <asp:Button ID="dField" runat="server" Text="Field" CssClass="descButton" ToolTip="Insert Field"
                                        OnClick="dField_Click" />&nbsp;&nbsp;
                                    <asp:Button ID="dBr" runat="server" Text="Newline" CssClass="descButton" ToolTip="Insert Newline"
                                        OnClick="dNewline_Click" />&nbsp;&nbsp;
                                    <% } %>
                                    <br />
                                    <br />
                                    <asp:UpdatePanel runat="server" ID="dUpdatePanel" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Panel ID="dLinkPanel" runat="server" Visible="false" CssClass="descPanel">
                                                <table class="descPanelTable">
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="iLinkN" runat="server" Text="Site Name: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="iLinkNBox" runat="server" CssClass="inputBox" Width="200px"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="iLinkURL" runat="server" CssClass="descLabel" Text="Site URL: "></asp:Label>
                                                                        &nbsp;&nbsp;
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="iLinkURLBox" runat="server" CssClass="inputBox" Width="200px"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Button ID="dLinkInsert" runat="server" CssClass="descButton" OnClick="dLinkInsert_Click"
                                                                            Text="Insert Link" ToolTip="Insert Link" />
                                                                        &nbsp;&nbsp;
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="3">
                                                                        <asp:Label ID="iLinkError" runat="server" CssClass="descLabelError" Text="Please insert a valid Site Name and URL."
                                                                            Visible="false"></asp:Label>
                                                                        &nbsp;&nbsp;
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <p>
                                                </p>
                                            </asp:Panel>
                                            <asp:Panel ID="dTablePanel" runat="server" Visible="false" CssClass="descPanel">
                                                <table class="descPanelTable">
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="iTableN" runat="server" Text="Table Name: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                                                                    </td>
                                                                    <td>
                                                                        <asp:UpdatePanel ID="UpdateTables" runat="server" UpdateMode="Conditional">
                                                                            <ContentTemplate>
                                                                                <asp:DropDownList ID="iTableNBox" runat="server">
                                                                                </asp:DropDownList>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Button ID="dTableInsert" runat="server" CssClass="descButton" OnClick="dTableInsert_Click"
                                                                            Text="Insert Table" ToolTip="Insert Table" />
                                                                        &nbsp;&nbsp;
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="3">
                                                                        <asp:Label ID="iTableError" runat="server" CssClass="descLabelError" Text="Please select a valid database table."
                                                                            Visible="false"></asp:Label>
                                                                        &nbsp;&nbsp;
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <p>
                                                </p>
                                            </asp:Panel>
                                            <asp:Panel ID="dFieldPanel" runat="server" Visible="false" CssClass="descPanel">
                                                <table class="descPanelTable">
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="iTableFN" runat="server" Text="Table Name: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                                                                    </td>
                                                                    <td>
                                                                        <asp:UpdatePanel ID="UpdateFieldTable" runat="server" UpdateMode="Conditional">
                                                                            <ContentTemplate>
                                                                                <asp:DropDownList ID="iTableFNBox" runat="server" OnSelectedIndexChanged="iTableFNBox_SelectedIndexChanged"
                                                                                    AutoPostBack="true" AppendDataBoundItems="true">
                                                                                    <asp:ListItem Value="">--Select Table--</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="iColFN" runat="server" Text="Column Name: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                                                                    </td>
                                                                    <td>
                                                                        <asp:UpdatePanel ID="UpdateFieldCol" runat="server" UpdateMode="Conditional">
                                                                            <ContentTemplate>
                                                                                <asp:DropDownList ID="iColFNBox" runat="server">
                                                                                </asp:DropDownList>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Button ID="dFieldInsert" runat="server" CssClass="descButton" OnClick="dFieldInsert_Click"
                                                                            Text="Insert Field" ToolTip="Insert Field" />
                                                                        &nbsp;&nbsp;
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="3">
                                                                        <asp:Label ID="dFieldError" runat="server" CssClass="descLabelError" Text="Please select a valid database table and column."
                                                                            Visible="false"></asp:Label>
                                                                        &nbsp;&nbsp;
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <p>
                                                </p>
                                            </asp:Panel>
                                            <asp:TextBox ID="descriptionBox" runat="server" Width="99%" Height="250" BorderColor="#766640"
                                                TextMode="MultiLine"></asp:TextBox>
                                            <br />
                                            <br />
                                            <asp:Label ID="descSuccess" runat="server" Text="" CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                                            <br />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="dLink" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="dTable" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="dField" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="dBr" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
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
                                            <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" CancelControlID="btnClose"
                                                runat="server" PopupControlID="AddIconsPanel" ID="ModalPopupExtender1" TargetControlID="addIcon" />
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
                                                                                        Select an icon from the icon library to add it to the connection.
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <p>
                                                                            </p>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <asp:Panel ID="addIconToLibary" Height="300px" ScrollBars="Both" runat="server" Visible="true">
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
                                            <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" CancelControlID="btnClose2"
                                                runat="server" PopupControlID="RemoveIconsPanel" ID="ModalPopupExtender2" TargetControlID="removeIcon" />
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
                                                                                        Select an icon to remove it from the connection.
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <p>
                                                                            </p>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <asp:Panel ID="removeIconFromConn" Height="300px" ScrollBars="Both" runat="server"
                                                                    Visible="true">
                                                                </asp:Panel>
                                                                <div class="right">
                                                                    <asp:Button ID="btnClose2" runat="server" Text="Cancel" CssClass="button" />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>
                                            <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" CancelControlID="btnClose3"
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
                                                                                                        <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmitClick" Text="Submit"
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
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="newConnA">
                                                <asp:Button ID="Button4" runat="server" Text="Modify Overlay" CssClass="button" Width="135"
                                                    Style="display: none; visibility: hidden;" />
                                                <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" CancelControlID="modCondCancel1"
                                                    runat="server" PopupControlID="ConPanel" ID="ModalPopupExtender7" TargetControlID="Button4" />
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
                                                <asp:Panel ID="OverlayConditionPanel" runat="server" Visible="true">
                                                </asp:Panel>
                                            </table>
                                            <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" CancelControlID="closeRemoveOverlay"
                                                runat="server" PopupControlID="RemoveOverlayPanel" ID="RemoveOverlayPopupExtender"
                                                TargetControlID="RemoveOverlayButton" />
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
                                                                                        Select the overlay color you want to remove.
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <p>
                                                                            </p>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <asp:Panel ID="removeOverlayInteriorPanel" runat="server" ScrollBars="Both" Visible="true">
                                                                </asp:Panel>
                                                                <div class="right" style="padding-top: 20px;">
                                                                    <asp:Button ID="closeRemoveOverlay" runat="server" Text="Cancel" CssClass="button" />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>
                                            <ajax:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" runat="server"
                                                PopupControlID="AddOverlayPanel" ID="AddOverlayPopupExtender" TargetControlID="AddOverlayButton" />
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
                                                                                        Use the Color Picker to select an overlay color. Choose a color and click Save.
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <p>
                                                                            </p>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <asp:HiddenField runat="server" ID="HiddenValue" Value="" />
                                                                <asp:Label ID="addColorMessage" runat="server" Text="&nbsp;" />
                                                                <table class="omainBox5" cellspacing="0" cellpadding="0">
                                                                    <tr>
                                                                        <td>
                                                                            <!--Color Info Here (Color Box? Color Drop Down?)--->
                                                                            <div class="colorPicker">
                                                                                &nbsp;&nbsp;Click here:
                                                                                <obout:ColorPicker ID="ColorPicker1" runat="server" ZIndex="500000" OnClientOpen="OnColorOpen"
                                                                                    OnClientPicked="OnColorPicked" TargetId="ColorAddText" TargetProperty="style.backgroundColor">
                                                                                    <asp:TextBox ReadOnly="true" ID="ColorAddText" Style="cursor: pointer;" runat="server" />
                                                                                </obout:ColorPicker>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <div class="right" style="padding-top: 20px;">
                                                                    <!-- <asp:Label ID="overColorExists" runat="server" Visible="false" Text="Overlay Color Exists! Please Choose Another:" /> -->
                                                                    <asp:Button ID="submitAddOverlay" runat="server" Text="Submit" CssClass="button"
                                                                        OnClick="addOverlayColorToConn" />
                                                                    <asp:Button ID="closeAddOverlay" runat="server" Text="Cancel" CssClass="button" OnClick="closeAddOverlayFunct" />
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
                            <% if (Request.QueryString.Get("locked") == "true")
                               { %>
                            <asp:Button ID="cancel" runat="server" Text="Cancel" CssClass="button" PostBackUrl="Main.aspx" />&nbsp;&nbsp;
                            <% }
                               else
                               {%>
                            <asp:Button ID="Button1" runat="server" Text="Cancel Changes" CssClass="button" PostBackUrl="Main.aspx" />&nbsp;&nbsp;
                            <% } %>
                            <asp:Button ID="saveConn" runat="server" Text="Save Connection" OnClick="modifyConnection"
                                CssClass="button" />
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
    <ajax:ModalPopupExtender OkControlID="cancelUpdate" runat="server" PopupControlID="connUpdateWarning"
        TargetControlID="connectButton" ID="warningModal" BackgroundCssClass="modalBackground"
        DropShadow="true">
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
            <asp:Button ID="continueUpdate" runat="server" Text="Yes" OnClick="updateConnection"
                CssClass="button" />
            <asp:Button ID="cancelUpdate" runat="server" Text="No" CssClass="button" />
        </div>
    </asp:Panel>
    <asp:Panel ID="scriptHandler" runat="server" Visible="true">
    </asp:Panel>
    </form>
</body>
</html>
