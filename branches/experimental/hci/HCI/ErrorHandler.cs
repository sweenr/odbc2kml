using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace HCI
{
    public class ErrorHandler
    {
        string errorText;
        Panel errorPanel;

        public ErrorHandler(String error, Panel panel)
        {
            errorText = error;
            errorPanel = panel;
        }

        public void displayError()
        {
            errorPanel.Visible = true;
            errorPanel.Controls.Add(new LiteralControl("<div style=\"color: black; Z-index:5000000000\"><p>" + errorText + "</p></div>"));
            errorPanel.Controls.Add(new LiteralControl("<script type=\"text/javascript\">$(\"#errorPanel1\").dialog('open')</script>"));
        }
    }
}
