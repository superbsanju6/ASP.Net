using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class GradeSubjectStandardsV2 : Criterion
    {

        //protected override void OnInit(EventArgs e)
        //{
        //    base.OnInit(e);
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            

            this.ddlCriteriaStandardSet.DataSource = this.StandardSetDataSource;
            this.ddlCriteriaStandardSet.DataTextField = "Value";
            this.ddlCriteriaStandardSet.DataValueField = "Key";
            this.ddlCriteriaStandardSet.DataBind();

            this.ddlCriteriaGrades.DataSource = this.GradesDataSource;
            this.ddlCriteriaGrades.DataTextField = "Value";
            this.ddlCriteriaGrades.DataValueField = "Key";
            this.ddlCriteriaGrades.DataBind();

            this.ddlCriteriaSubjects.DataSource = this.SubjectsDataSource;
            this.ddlCriteriaSubjects.DataTextField = "Value";
            this.ddlCriteriaSubjects.DataValueField = "Key";
            this.ddlCriteriaSubjects.DataBind();

            this.ddlCriteriaCourse.DataSource = this.CourseDataSource;
            this.ddlCriteriaCourse.DataTextField = "Value";
            this.ddlCriteriaCourse.DataValueField = "Key";
            this.ddlCriteriaCourse.DataBind();

            this.ddlCriteriaStandards.DataSource = this.StandardsDataSource;
            this.ddlCriteriaStandards.DataTextField = "Value";
            this.ddlCriteriaStandards.DataValueField = "Key";
            this.ddlCriteriaStandards.DataBind();

            //CriteriaHeaderText.Text = this.Header;
            //if (this.IsRequired)
            //{
            //    RequiredCriteriaIndicator.Text = "*";
            //    RequiredCriteriaIndicator.Style.Add("color", "red");
            //    RequiredCriteriaIndicator.Style.Add("font-weight", "bold");
            //}
            //if ( IsPostBack)
            //{
            //    ddlCriteriaStandardSet.SelectedValue = StandardSelection.StandardSet;
            //    ddlCriteriaGrades.SelectedValue = StandardSelection.Grade;
            //    ddlCriteriaSubjects.SelectedValue = StandardSelection.Subject;
            //    ddlCriteriaCourse.SelectedValue = StandardSelection.Course;
            //    ddlCriteriaStandards.SelectedValue = StandardSelection.Key;
            //}
        }
    }
}