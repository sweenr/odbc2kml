<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Overlay.aspx.cs" Inherits="HCI._Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Overlay Page</title>
    <style type="text/css">
        .handleImage
        {
            width: 16px;
            height: 16px;
            cursor: se-resize;
        }
        .mainBox
        {
            border: solid 2px rgb(26,49,76);
            background-color: rgb(26,49,76);
            width: 100%;
        }
        .mainBox2
        {
            width: 100%;
            background-color: rgb(60,106,142);
        }
        .mainBox3
        {
            width: 70%;
            background-color: White;
            margin-left: auto;
            margin-right: auto;
            background-color: rgb(26,49,76);
        }
        .mainBox4
        {
	        background-color: rgb(238,238,244);
	        padding: 10px;	        
	        margin: 0px;
        }
        .overlayStyle
        {
            color: white;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <form id="OverlayForm" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="mainBox">
        <span class="overlayStyle">Overlay Page</span>
        <div class="mainBox2">
            <br />
            <br />
            <div class="mainBox3">
                <span class="overlayStyle">Color</span>
                <div class="mainBox4">                    
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
            <div class="mainBox3">
                <span class="overlayStyle">Conditions</span>
                <br />
                <table border="1" width="100%" class="mainBox4">
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
                            <asp:Button ID="AddBtn" runat="server" Text="Add" />
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
                            <asp:Button ID="DelBtn" runat="server" Style="text-align: center" Text="Delete" />
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <asp:Button ID="OkBtn"  Width="50" runat="server" Style="text-align: center; margin-left: 850px" Text="OK" />
            <asp:Button ID="CancelBtn" runat="server" Style="text-align: center;  margin-left: 50px" Text="Cancel" />
        </div>
    </div>
    </form>
</body>
</html>
