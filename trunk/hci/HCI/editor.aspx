<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editor.aspx.cs" Inherits="HCI.editor"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css" media="all">
        @import "odbcStyle.css";
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
        <div class="mainBox">
            <span class="connectionStyle">&nbsp;Connection Description</span>
            <br />
            <div class="mainBox4">
                <div style="background-color: white; padding: 5px;">
                    <asp:Label ID="dLabel" runat="server" Text="Insert: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                    <asp:Button ID="dLink" runat="server" Text="Link" CssClass="descButton" ToolTip="Insert Link"
                        OnClick="dLink_Click" />&nbsp;&nbsp;
                    <asp:Button ID="dField" runat="server" Text="Field" CssClass="descButton" ToolTip="Insert Field" />&nbsp;&nbsp;
                    <asp:Button ID="dImage" runat="server" Text="Image" CssClass="descButton" ToolTip="Insert Image" />&nbsp;&nbsp;
                    <br />
                    <br />
                    <asp:UpdatePanel runat="server" ID="dUpdatePanel">
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
                                                <tr><td colspan="3"><asp:Label ID="iLinkError" runat="server" CssClass="descLabelError" Text="Please insert a valid Site Name and URL." Visible="false"></asp:Label>
                                                        &nbsp;&nbsp;</td></tr>
                                            </table>
                                </td></tr></table>
                                <p>
                                </p>
                            </asp:Panel>
                            <asp:TextBox ID="descriptionBox" runat="server" Width="99%" Height="250" BorderColor="#766640"
                                TextMode="MultiLine"></asp:TextBox>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="dLinkInsert" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
