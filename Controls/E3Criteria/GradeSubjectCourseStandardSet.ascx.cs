using System;
using System.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    /// <summary>
    /// Custom dependent control for Grade, Subject, Course & Standard Set
    /// </summary>
    public partial class GradeSubjectCourseStandardSet : CriteriaBase
    {
        public object JsonDataSource;
        public CheckBoxList ChkStandardSet
        {
            get { return chkStandardSet; }
        }
        public CheckBoxList ChkGrade
        {
            get { return chkGrade;}
        }

        public CheckBoxList ChkSubject
        {
            get { return chkSubject;}
        }

        public DropDownList CmbCourse
        {
            get { return cmbCourse; }
        }

		public DropDownList CmbLevel
		{
			get { return cmbLevel; }
		}

		public bool ShowStandardLevels = false;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                chkStandardSet.OnChange = CriteriaName + "Controller.OnChange();";
                chkGrade.OnChange = CriteriaName + "Controller.OnChange();";
                chkSubject.OnChange = CriteriaName + "Controller.OnChange();";

                if (ShowStandardLevels)
                {
                    cmbLevel.Visible = true;
                    cmbLevel.OnClientLoad = CriteriaName + "Controller.PopulateControls";             
                }
                else
                {
                    cmbLevel.Visible = false;
                    cmbCourse.OnClientLoad = CriteriaName + "Controller.PopulateControls";
                }

                chkStandardSet.LoadDefaults = false;
                


                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);      
            }
        }

		public class GradeSubjectCourseStandardSetWithStandardLevels
		{
			public object GradeSubjectCourseStandardSetData;
			public object StandardLevelsData;
		}
   
    }
}