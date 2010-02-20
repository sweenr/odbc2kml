<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editor.aspx.cs" Inherits="HCI.editor" %>

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
                            <asp:GridView ID="GridViewTables" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                CellPadding="4" DataSourceID="SQLTables" ForeColor="#333333" GridLines="None"
                                PageSize="10" ShowHeader="False" Width="100%" 
                                onselectedindexchanged="GridViewTables_SelectedIndexChanged" 
                                DataKeyNames="TABLE_NAME">
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
                        <td class="mainBox3" valign="top">  
                        <asp:GridView ID="GridViewColumns" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                CellPadding="4" DataSourceID="SQLColGen" ForeColor="#333333" GridLines="None"
                                PageSize="10" ShowHeader="False" Width="100%" 
                                onpageindexchanged="GridViewColumns_PageIndexChanged">
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
                    <asp:Label ID="dLabel" runat="server" Text="Insert: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                    <asp:Button ID="dLink" runat="server" Text="Link" CssClass="descButton" ToolTip="Insert Link"
                        OnClick="dLink_Click" />&nbsp;&nbsp;
                    <asp:Button ID="dTable" runat="server" Text="Table" CssClass="descButton" ToolTip="Insert Table"
                        OnClick="dTable_Click" />&nbsp;&nbsp;
                    <asp:Button ID="dField" runat="server" Text="Field" CssClass="descButton" ToolTip="Insert Field"
                        OnClick="dField_Click" />&nbsp;&nbsp;
                    <asp:Button ID="dImage" runat="server" Text="Image" CssClass="descButton" ToolTip="Insert Image"
                        OnClick="dImage_Click" />&nbsp;&nbsp;
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
                                                                <asp:DropDownList ID="iTableNBox" runat="server" DataSourceID="SQLTables" DataTextField="TABLE_NAME"
                                                                    DataValueField="TABLE_NAME">
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
                                                                <asp:DropDownList ID="iTableFNBox" runat="server" DataSourceID="SQLTables" DataTextField="TABLE_NAME"
                                                                    DataValueField="TABLE_NAME" OnSelectedIndexChanged="iTableFNBox_SelectedIndexChanged"
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
                            <asp:Panel ID="dImagePanel" runat="server" Visible="false" CssClass="descPanel">
                                <table class="descPanelTable">
                                    <tr>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="iTableIN" runat="server" Text="Table Name: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:UpdatePanel ID="UpdateImageTable" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <asp:DropDownList ID="iTableINBox" runat="server" DataSourceID="SQLTables" DataTextField="TABLE_NAME"
                                                                    DataValueField="TABLE_NAME" OnSelectedIndexChanged="iTableINBox_SelectedIndexChanged"
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
                                                        <asp:Label ID="iColIN" runat="server" Text="Column Name: " CssClass="descLabel"></asp:Label>&nbsp;&nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:UpdatePanel ID="UpdateImageCol" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <asp:DropDownList ID="iColINBox" runat="server">
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
                                                        <asp:Button ID="dImageInsert" runat="server" CssClass="descButton" OnClick="dImageInsert_Click"
                                                            Text="Insert Image" ToolTip="Insert Image" />
                                                        &nbsp;&nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <asp:Label ID="dImageError" runat="server" CssClass="descLabelError" Text="Please select a valid database table and column."
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
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="dLinkInsert" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <asp:SqlDataSource ID="SQLTables" runat="server" ConnectionString="<%$ ConnectionStrings:testConnectionString %>"
        ProviderName="<%$ ConnectionStrings:testConnectionString.ProviderName %>" SelectCommand="SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'test'">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SQLColGen" runat="server" ConnectionString="<%$ ConnectionStrings:testConnectionString %>"
        ProviderName="<%$ ConnectionStrings:testConnectionString.ProviderName %>"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SQLColumns" runat="server" ConnectionString="<%$ ConnectionStrings:testConnectionString %>"
        ProviderName="<%$ ConnectionStrings:testConnectionString.ProviderName %>"></asp:SqlDataSource>
    </form>
</body>
</html>
