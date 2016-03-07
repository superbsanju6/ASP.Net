
using System;
using System.Web.UI.WebControls;
using Thinkgate.Classes.Search;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class GradeSubjectCurriculum : Criterion
    {
        #region " Properties "

        #endregion

        #region Page Events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.RES_GSC_CriteriaGrades.DataSource = this.GradesDataSource;
                this.RES_GSC_CriteriaGrades.DataTextField = "Value";
                this.RES_GSC_CriteriaGrades.DataValueField = "Key";
                this.RES_GSC_CriteriaGrades.DataBind();

                this.RES_GSC_CriteriaSubjects.DataSource = this.SubjectsDataSource;
                this.RES_GSC_CriteriaSubjects.DataTextField = "Value";
                this.RES_GSC_CriteriaSubjects.DataValueField = "Key";
                this.RES_GSC_CriteriaSubjects.DataBind();

                this.RES_GSC_CriteriaCurricula.DataSource = this.CurriculaDataSource;
                this.RES_GSC_CriteriaCurricula.DataTextField = "Value";
                this.RES_GSC_CriteriaCurricula.DataValueField = "Key";
                this.RES_GSC_CriteriaCurricula.DataBind();

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