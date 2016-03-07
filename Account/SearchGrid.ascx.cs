using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Thinkgate.Account
{
    public partial class SearchGrid : System.Web.UI.UserControl
    {
        public event EventHandler SearchSelect;
        public event EventHandler SearchSearch;
        private Base.Classes.Administration adm = new Base.Classes.Administration();
        private const string searchType = "searchType";
        public string userName = "userName";

        public enum SearchModes
        {
            None,
            User
        }

        public SearchModes SearchMode
        {
            get { return ViewState[searchType] != null ? (SearchModes)Enum.Parse(typeof(SearchModes), ViewState[searchType].ToString()) : SearchModes.None; }
            set { ViewState[searchType] = value.ToString(); }
        }

        protected void btnSearch_OnClick(object o, EventArgs e)
        {
            if (txtSearch.Text.Length < 3)
            {
                grdSearch.DataSource = null;
                grdSearch.Visible = false;
                lblRequirement.Visible = true;
                return;
            }
            else
            {

                lblRequirement.Visible = false;
            }
            
            switch (SearchMode)
            {
                case SearchModes.User:
                    grdSearch.DataSource = adm.SearchForUsers(txtSearch.Text.ToLower());
                    break;
                default:
                    break;
            }

            grdSearch.DataBind();
            grdSearch.Visible = true;

            if (SearchSearch != null)
            {
                SearchSearch(this, e);
            }
        }

        public void ReloadGUI(SearchModes mode)
        {
            SearchMode = mode;

            grdSearch.DataSource = null;
            grdSearch.DataBind();
            grdSearch.Visible = false;

            txtSearch.Text = string.Empty;
            txtSearch.Focus();

            lblRequirement.Visible = false;
        }

        protected void grdSearch_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (SearchSelect != null)
            {
                switch(SearchMode)
                {
                    case SearchModes.User:
                        this.Attributes.Add(this.ID.ToString(), ((GridDataItem)e.Item)["UserID"].Text);
                        this.Attributes.Add(userName, ((GridDataItem)e.Item)["UserName"].Text);
                        break;
                    default:
                        break;
                }

                foreach (GridDataItem gdi in grdSearch.Items)
                {
                    gdi.Display = (GridDataItem)e.Item == gdi ? true : false;
                }

                SearchSelect(this, e);              
            }
        }

        protected void gridSearch_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
        {
            if (sender is RadGrid && SearchMode != SearchModes.None)
            {
                switch (SearchMode)
                {
                    case SearchModes.User:
                        if ((e.Column.UniqueName.ToLower() != "username") && (e.Column.UniqueName.ToLower() != "firstname") && e.Column.UniqueName.ToLower() != "lastname")
                        {
                            e.Column.Display = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}