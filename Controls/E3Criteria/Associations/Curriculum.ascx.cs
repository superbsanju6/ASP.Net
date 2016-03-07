using System;
using System.Web.UI;
using Thinkgate.Base.Classes;
using System.Web.Script.Serialization;

namespace Thinkgate.Controls.E3Criteria.Associations
{
    public partial class Curriculum : CriteriaBase
    {
        public Boolean IncludeTypeAndTermControls = false;  //When set to True, then the Type and Term controls will added to the display.
		public CriteriaDataSources DataSources = new CriteriaDataSources();
        public Object JsonDataSource { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                trType.Visible = false;
                trTerm.Visible = false;
                if (IncludeTypeAndTermControls)
                {
                    trType.Visible = true;
                    trTerm.Visible = true;
                    cmbType.Attributes.Add("CriteriaName", CriteriaName);
                    cmbType.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                    cmbType.OnClientBlur = CriteriaName + "Controller.OnComboChanged";

                    cmbTerm.Attributes.Add("CriteriaName", CriteriaName);
                    cmbTerm.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                    cmbTerm.OnClientBlur = CriteriaName + "Controller.OnComboChanged";
                }

                cmbGrade.Attributes.Add("CriteriaName", CriteriaName);
                cmbGrade.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbGrade.OnClientBlur = CriteriaName + "Controller.OnComboChanged";

                cmbSubject.Attributes.Add("CriteriaName", CriteriaName);
                cmbSubject.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbSubject.OnClientBlur = CriteriaName + "Controller.OnComboChanged";

                cmbCurriculum.Attributes.Add("CriteriaName", CriteriaName);
                cmbCurriculum.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbCurriculum.OnClientBlur = CriteriaName + "Controller.OnComboChanged";

                var lastCritControl = cmbCurriculum;

                //
                // After controls are loaded, one of the controls needs to fire the CurriculumController.PopulateControls() 
                // method over on the JavaScript side.
                //
                //cmbCurriculum.OnClientLoad = CriteriaName + "Controller.PopulateControls";

                ValueDisplayTemplateName = "CurriculumCriteriaValueDisplayTemplate";
                RestrictValueCount = RestrictValueOptions.OnlyOne_Period;


				if (JsonDataSource == null && IncludeTypeAndTermControls)
				{
					var serializer = new JavaScriptSerializer();
					JsonDataSource = serializer.Serialize(DataSources);

                    lastCritControl = cmbTerm;
				}
                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);

                lastCritControl.OnClientLoad = CriteriaName + "Controller.PopulateControls";

            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public class ValueObject
        {
            public string Grade { get; set; }
            public string Subject { get; set; }
            public string Curriculum { get; set; }
            public string Type { get; set; }
            public string Term { get; set; }
        }

		public class CriteriaDataSources
		{
			public object GradeSubjectCurriculumData { get; set; }
			public object TypeData { get; set; }
			public object TermData { get; set; }
		}
    }
}