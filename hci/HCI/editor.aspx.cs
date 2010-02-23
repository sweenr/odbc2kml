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
            string descriptionInfo = "[URL][TITLE]" + linkText + "[/TITLE]" + linkURL + "[/URL]";
        

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
            string tableText = iTableNBox.SelectedValue.ToString();
            string descriptionInfo = "[TABLE]" + tableText + "[/TABLE]";

            descriptionBox.Text += descriptionInfo;
            
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
            string tableText = iTableFNBox.SelectedValue.ToString();
            string colText = iColFNBox.SelectedValue.ToString();
          
            string descriptionInfo = "[FIELD][TBL]" + tableText + "[/TBL][COL]" + colText + "[/COL][/FIELD]";


            if (tableText.Equals("") || colText.Equals(""))
            {
                dFieldError.Visible = true;
            }
            else
            {
                dFieldError.Visible = false;
                descriptionBox.Text += descriptionInfo;
            }
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
            string tableText = iTableINBox.SelectedValue.ToString();
            string colText = iColINBox.SelectedValue.ToString();

            string descriptionInfo = "[IMG][TBL]" + tableText + "[/TBL][COL]" + colText + "[/COL][/IMG]";


            if (tableText.Equals("") || colText.Equals(""))
            {
                dImageError.Visible = true;
            }
            else
            {
                dImageError.Visible = false;
                descriptionBox.Text += descriptionInfo;
            }
        }

        protected void iTableFNBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = iTableFNBox.SelectedValue.ToString();
            if (selectedTable != "")
            {
                SQLColumns.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                iColFNBox.DataSource = SQLColumns;
                iColFNBox.DataValueField = "COLUMN_NAME";
                iColFNBox.DataTextField = "COLUMN_NAME";
                iColFNBox.DataBind();
                UpdateFieldCol.Update();
            }
            else
            {
                iColFNBox.Items.Clear();
                UpdateFieldCol.Update();
            }
        }

        protected void iTableINBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = iTableINBox.SelectedValue.ToString();
            if (selectedTable != "")
            {
                SQLColumns.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedTable + "')";
                iColINBox.DataSource = SQLColumns;
                iColINBox.DataValueField = "COLUMN_NAME";
                iColINBox.DataTextField = "COLUMN_NAME";
                iColINBox.DataBind();
                UpdateImageCol.Update();

            }
            else
            {
                iColINBox.Items.Clear();
                UpdateImageCol.Update();
            }
        }

        protected void GridViewTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            columnButtons.Visible = true;
            columnMessage.Visible = false;
            string selectedTable = GridViewTables.SelectedValue.ToString();
            selectedGVTable.Value = selectedTable;
            SQLColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
            
        }

        protected void GridViewColumns_PageIndexChanged(object sender, EventArgs e)
        {
            SQLColGen.SelectCommand = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE (TABLE_NAME = '" + selectedGVTable.Value + "')";
        }

        protected void viewTable_Click(object sender, EventArgs e)
        {
            
        }

    }
}


