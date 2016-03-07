using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes.Search;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class SchoolGradeName : Criterion
    {

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                this.ddlCriteriaSchool.DataSource = this.SchoolDataSource;
                this.ddlCriteriaSchool.DataTextField = "Key";
                this.ddlCriteriaSchool.DataValueField = "Value";
                this.ddlCriteriaSchool.DataBind();

                this.ddlCriteriaGrades.DataSource = this.GradesDataSource;
                this.ddlCriteriaGrades.DataTextField = "Value";
                this.ddlCriteriaGrades.DataValueField = "Key";
                this.ddlCriteriaGrades.DataBind();

                this.ddlCriteriaNames.DataSource = this.TeacherNameDataSource;
                this.ddlCriteriaNames.DataTextField = "Value";
                this.ddlCriteriaNames.DataValueField = "Key";
                this.ddlCriteriaNames.DataBind();

                CriteriaHeaderText.Text = this.Header;
                if (this.IsRequired)
                {
                    RequiredCriteriaIndicator.Text = "*";
                    RequiredCriteriaIndicator.Style.Add("color", "red");
                    RequiredCriteriaIndicator.Style.Add("font-weight", "bold");
                }
            }
        }




    }
}
