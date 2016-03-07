using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using Thinkgate.Base.Classes;
using System.Web.Script.Serialization;

namespace Thinkgate.Controls.E3Criteria.AssessmentSchedule
{
    public partial class ScheduleCriteria : CriteriaBase
    {
        public CriteriaDataSources DataSources = new CriteriaDataSources();

		// The following properties are used to prepopulate and lock down 
		// the controls.  However, the logic for this is on the .ASCX side.
		public string DefaultSchedule;
		public string DefaultCategory;
		public bool ScheduleReadOnly;
		public bool CategoryReadOnly;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //
                //Setup Criteria
                //
                RestrictValueCount = RestrictValueOptions.OnlyOne_Period;

                //
                // Setup cmbCategory
                //
                cmbCategory.Attributes.Add("CriteriaName", CriteriaName);
                cmbCategory.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";

                cmbSchedule.Attributes.Add("CriteriaName", CriteriaName);
                cmbSchedule.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbSchedule.OnClientBlur = CriteriaName + "Controller.OnComboChanged";

                cmbSchedule.OnClientLoad = CriteriaName + "Controller.PopulateControls";

                ValueDisplayTemplateName = "ScheduleCriteriaValueDisplayTemplate";

                //
                // Create Json array strings for populating our dropdownlist controls, and register them as javascript;
                //
                var serializer = new JavaScriptSerializer();

                var JsonDataSource = serializer.Serialize(DataSources);
                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);

            }

            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);

        }

        public class ValueObject
        {
            public string Category { get; set; }
            public string ScheduleLevel { get; set; }
        }

        public class CriteriaDataSources
        {
            public object CategoryData { get; set; }
            public object ScheduleLevelData { get; set; }
        }

	}
}