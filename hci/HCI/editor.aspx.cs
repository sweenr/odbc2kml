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
            dTablePanel.Visible = false;
            dFieldPanel.Visible = false;
            dImagePanel.Visible = false;
        }
        protected void dLinkInsert_Click(object sender, EventArgs e)
        {

            string linkText = iLinkNBox.Text.ToString();
            string linkURL = iLinkURLBox.Text.ToString();
            string descriptionInfo = "[START URL][TITLE]" + linkText + "[/TITLE]" + linkURL + "[END URL]";
        

            if (linkText.Equals("") || linkURL.Equals(""))
            {
                iLinkError.Visible = true;
            }
            else
            {
                iLinkError.Visible = false;
                descriptionBox.Text += descriptionInfo;
            }
           
        }


        protected void dTable_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = false;
            dTablePanel.Visible = true;
            dFieldPanel.Visible = false;
            dImagePanel.Visible = false;
        }
        protected void dTableInsert_Click(object sender, EventArgs e)
        {

        }


        protected void dField_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = false;
            dTablePanel.Visible = false;
            dFieldPanel.Visible = true;
            dImagePanel.Visible = false;
        }
        protected void dFieldInsert_Click(object sender, EventArgs e)
        {

        }

        protected void dImage_Click(object sender, EventArgs e)
        {
            dLinkPanel.Visible = false;
            dTablePanel.Visible = false;
            dFieldPanel.Visible = false;
            dImagePanel.Visible = true;
        }
        protected void dImageInsert_Click(object sender, EventArgs e)
        {

        }

    }
}
