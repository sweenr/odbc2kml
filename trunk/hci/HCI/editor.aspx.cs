using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HCI
{
    public partial class editor : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void dLink_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = true;
        }
        protected void dLinkInsert_Click(object sender, EventArgs e)
        {

            string linkText = iLinkNBox.Text.ToString();
            string linkURL = iLinkURLBox.Text.ToString();
            string currentText = descriptionBox.Text.ToString();

            if (linkText == "" || linkText == null || linkURL == "" || linkURL == null)
            {
                iLinkError.Visible = true;
            }
            else
            {
                iLinkError.Visible = false;
                descriptionBox.Text += "<a href = \"" + linkURL + "\">" + linkText + "</a>";
            }

           
        }
      
    }
}
