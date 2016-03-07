
using System;
using System.Web.UI.WebControls;
using Thinkgate.Classes.Search;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class CheckboxList : Criterion
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
                this.chk.DataSource = this.DataSource;
                this.chk.DataTextField = this.DataTextField;
                this.chk.DataValueField = this.DataValueField;
                this.chk.DataBind();

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