<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="HCI.Upload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Icon Upload</title>
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
        #message 
        {
        	color: Red;
        	width: 465px;
        	margin: 0 auto;
        	background-color: #F2F5F7;
        }
    </style>
    <script src="jquery/jquery-1.4.1.js" type="text/javascript"></script> 
    <script src="jquery/jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>
    <link href="jquery/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
    
      $(document).ready(function() {
		    $("#uploadTabs").tabs();
	    });

    
    </script>
    <!--<script src="jquery/jquery-test.js" type="text/javascript"></script>-->
    <!--<link rel="stylesheet" type="text/css" href="jquery/Stylesheet1.css" />-->
</head>
<body>
    <form id="form1" runat="server">
    <div id="container">
        <div id="message">
            <center><asp:label ID="messages" runat="server"></asp:label></center>
        </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div id="uploadTabs" class="ui-widget">
            <ul class="ui-tabs-nav">
                <li><a href="#remote">Upload Icon From Link</a></li>
                <li><a href="#local">Upload Icon Locally</a></li>
            </ul>
            <div id="remote" class="ui-tabs-panel">
                <ul>
                    <li>
                        <p>Please check fetch if you would like to have the file saved on the server</p>
                        <p>
                            <asp:CheckBox ID="fetchCheckBox" runat="server" Text="Fetch"/>
                        </p>
                    </li>
                    <li>
                        <p>Please enter the URL of the icon you would like to use</p>
                        <p>
                            <asp:TextBox ID="URLtextBox" runat="server" Width="270" />
                            <asp:Button ID="URLsubmit" runat="server" OnClick="URLsubmitClick" Text="Save" />
                            <asp:Panel ID="URLpanel" runat="server" Visible="false">
                                <br />
                                <asp:label ID="URLsubmitLabel" runat="server"></asp:label>
                                <br /><p>Is the above address correct?</p>
                                <asp:Button ID="URLcorrect" runat="server" OnClick="URLcorrectClick" Text="Correct" />
                                <asp:label ID="test" runat="server"></asp:label>
                            </asp:Panel>
                        </p>
                    </li>
                </ul>
            </div>
            <div id="local" class="ui-tabs-panel">
                <ul>
                    <li>
                        <p>Please select the icon file you wish to upload</p>
                        <p>
                            <asp:FileUpload ID="fileUpEx" runat="server" /><br />
                            <asp:button ID="btnSubmit" runat="server" OnClick="btnSubmitClick" Text="Submit" />
                            <asp:label ID="lblStatus" runat="server"></asp:label>
                        </p>
                    </li>
                </ul>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
