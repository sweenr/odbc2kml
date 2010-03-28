<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainPage.aspx.cs" Inherits="HCI.MainPage" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
                Site Logo Goes Here
            </div>
            <div id="home">
                Link Home
            </div>
        </div>
        <div id="page">
            <table cellpadding="10" width="50%">
                <tr>
                    <td>
                        <div class="mainBox">
                            <span class="connectionStyle">&nbsp;Connection Information</span>
                            <table cellspacing="0" cellpadding="10" class="mainBox2">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="ODBC Connection 1"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="CheckBox1" runat="server" Text="Edit" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="ODBC Connection 2"></asp:Label>                                        
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="CheckBox2" runat="server" Text="Edit" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>           
            <div align="right">
<!-- ------------------------------------------------------------------------------------------------------------->
                <asp:Button ID="Button1" runat="server" Text="New" Width="68px" CssClass="button" /> 
                &nbsp;&nbsp;
                <cc1:ModalPopupExtender BackgroundCssClass="modalBackground" DropShadow="true" OkControlID="modCondOK1"
                    CancelControlID="modCondCancel1" runat="server" PopupControlID="DavidsPopup"
                    ID="ModalPopupExtender7" TargetControlID="Button1" /> 
                <asp:Panel ID="DavidsPopup" runat="server" class="boxPopupStyle" Style="display: none;">
                    <span class="connectionStyle">&nbsp;Create Connection</span>
                        <div class="boxPopupStyle1">
                                <asp:Button ID="ModCondOK1" runat="server" Text="Submit" CssClass="button" />&nbsp;&nbsp;
                                <asp:Button ID="ModCondCancel1" runat="server" Text="Cancel" CssClass="button" />&nbsp;&nbsp;    
                        </div>
                </asp:Panel>
                
<!-- -------------------------------------------------------------------------------------------------------------> 
                <asp:Button ID="Button2" runat="server" Text="Modify" Width="70px" CssClass="button" />
                &nbsp;&nbsp;
                <asp:Button ID="Button3" runat="server" Text="View" Width="71px" CssClass="button" />
                &nbsp;&nbsp;
                <asp:Button ID="Button4" runat="server" Text="Delete" Width="71px" CssClass="button" />
                &nbsp;&nbsp;
                <asp:Button ID="Button5" runat="server" Text="Create KML" Width="90px" CssClass="button" Style="margin-right:20px" />         
            </div>
        </div>
    </div>  
    <asp:ScriptManager ID="ScriptManager1" runat="server"> 
    </asp:ScriptManager> 
    </form>
</body>
</html>
