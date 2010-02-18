<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="HCI.Upload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Icon Upload</title>
    <!--<style type="text/css" media="all">
        @import "odbcStyle.css";
    </style>-->
    
    <script type="text/javascript">
    
      $(document).ready(function() {
		    $("#uploadTabs").tabs();
	    });

    
    </script>
    <script src="jquery/jquery-1.4.1.js" type="text/javascript"></script> 
    <script src="jquery/jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>
    <link href="jquery/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
    <!--<script src="jquery/jquery-test.js" type="text/javascript"></script>-->
    <!--<link rel="stylesheet" type="text/css" href="jquery/Stylesheet1.css" />-->
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div id="uploadTabs" class="ui-widget">
            <ul class="ui-tabs-nav">
                <li><a href="#local">Upload Icon Locally</a></li>
                <li><a href="#remote">Upload Icon From Link</a></li>
            </ul>
            <div id="local" class="ui-tabs-panel">
                <ul>
                    <li>Please select the icon file you wish to upload</li>
                    <li>
                        <asp:FileUpload ID="fileUpEx" runat="server" /><br />
                        <asp:button ID="btnSubmit" runat="server" OnClick="btnSubmitClick" Text="Submit" />
                        <asp:label ID="lblStatus" runat="server"></asp:label>
                    </li>
                </ul>
            </div>
            <div id="remote" class="ui-tabs-panel">
                <ul>
                    <li>Please check fetch if you would like to have the file saved on the server</li>
                    <li>
                        <asp:CheckBox ID="fetchCheckBox" runat="server" Text="Fetch"/>
                    </li>
                    <li>Please enter the URL of the icon you would like to use</li>
                    <li>
                        <asp:TextBox ID="URLtextBox" runat="server" />
                        <asp:Button ID="URLsubmit" runat="server" OnClick="URLsubmitClick" Text="Save" />
                        <asp:Panel ID="URLpanel" runat="server" Visible="false">
                            <br />
                            <asp:label ID="URLsubmitLabel" runat="server"></asp:label>
                            <br /><p>Is the above address correct?</p>
                            <asp:Button ID="URLcorrect" runat="server" OnClick="URLcorrectClick" Text="Correct" />
                            <asp:label ID="test" runat="server"></asp:label>
                        </asp:Panel>
                    </li>
                </ul>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
