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
        private string errorText;
        private Panel errorPanel;
        private UpdatePanel errorUpdatePanel;
        private string mpeID;

        /// <summary>
        /// Setup ErrorHandler
        /// </summary>
        /// <param name="error">Error Text</param>
        /// <param name="panel">Panel to display error in</param>
        public ErrorHandler(String error, Panel panel)
        {
            errorText = error;
            errorPanel = panel;
            mpeID = new String('\0',0);
        }

        /// <summary>
        /// Setup ErrorHandler inside ModalPopupExtender
        /// </summary>
        /// <param name="error">Error Text</param>
        /// <param name="panel">Panel to display error in</param>
        /// <param name="mpeString">ID of ModalPopupExtender</param>
        public ErrorHandler(String error, Panel panel, String mpeString)
        {
            errorText = error;
            errorPanel = panel;
            mpeID = mpeString;
        }

        /// <summary>
        /// Setup ErrorHandler
        /// </summary>
        /// <param name="error">Error Text</param>
        /// <param name="panel">UpdatePanel to display error in</param>
        public ErrorHandler(String error, UpdatePanel panel)
        {
            errorText = error;
            errorUpdatePanel = panel;
            mpeID = new String('\0', 0);
        }

        /// <summary>
        /// Setup ErrorHandler inside ModalPopupExtender
        /// </summary>
        /// <param name="error">Error Text</param>
        /// <param name="panel">UpdatePanel to display error in</param>
        /// <param name="mpeString">ID of ModalPopupExtender</param>
        public ErrorHandler(String error, UpdatePanel panel, String mpeString)
        {
            errorText = error;
            errorUpdatePanel = panel;
            mpeID = mpeString;
        }

        /// <summary>
        /// Displays the error message
        /// </summary>
        public void displayError()
        {
            // Use obj instead of the errorPanel or errorUpdate panel so we don't have to have two sets of jsError strings
            Control obj;
            if (errorUpdatePanel == null)
            {
                errorPanel.Controls.Clear();
                obj = errorPanel;
            }
            else
            {
                obj = errorUpdatePanel;
            }

            // Javascript code that will setup/run the JQueryUI Dialog box
            string jsError = "<script type=\"text/javascript\">$(function() { $(\"#" + obj.ClientID + "errorDiv\").dialog({ bgiframe: true, modal: true, autoOpen: false, title: 'Error!', resizable: false, dialogClass: 'alert', buttons: { Ok: function() { $(this).dialog('close');";
            
            // Add code to show MPE after "Ok" clicked if we are inside an MPE
            if(mpeID.Length != 0)
            {
                jsError += "$find('" + mpeID + "').show(); ";
            }
            
            jsError += " } } }); }); ";

            // Add code to hide MPE when the ErrorHandler is shown if we are inside an MPE
            if(mpeID.Length != 0)
            {
                jsError += "$find('" + mpeID + "').hide(); ";
            }
            
            jsError += "$(\"#" + obj.ClientID + "errorDiv\").dialog('open');$(\"#" + obj.ClientID + "errorDiv\").focus();</script>";


            // not in an UpdatePanel, use regular Panel code
            if (errorUpdatePanel == null)
            {
                errorPanel.Visible = true;
                errorPanel.Controls.Add(new LiteralControl("<div style=\"color: black; Z-index:5000000000\" id=\"" + errorPanel.ClientID + "errorDiv\"><p>" + errorText + "</p></div>"));
                errorPanel.Controls.Add(new LiteralControl(jsError));
            }
            else
            {
                errorUpdatePanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<div style=\"color: black; Z-index:5000000000\" id=\"" + errorUpdatePanel.ClientID + "errorDiv\"><p>" + errorText + "</p></div>"));
                ScriptManager.RegisterClientScriptBlock(errorUpdatePanel, typeof(UpdatePanel), errorUpdatePanel.ClientID, jsError, false);
            }
                //errorUpdatePanel.ContentTemplateContainer.Controls.Add(new LiteralControl("<script type=\"text/javascript\">$(\"#errorPanel1\").dialog('open')</script>"));
        }
    }
}
