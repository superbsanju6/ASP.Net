
using System;
using System.Web.UI.WebControls;
using Thinkgate.Classes.Search;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class DropdownList : Criterion
    {
        #region Page Events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.ddl.DataSource = this.DataSource;
                this.ddl.DataTextField = this.DataTextField;
                this.ddl.DataValueField = this.DataValueField;
                this.ddl.DataBind();

                CriteriaHeaderText.Text = this.Header;
                if (this.IsRequired)
                {
                    RequiredCriteriaIndicator.Text = "*";
                    RequiredCriteriaIndicator.Style.Add("color", "red");
                    RequiredCriteriaIndicator.Style.Add("font-weight", "bold");
                }
            }
        }

        #endregion
    }
}