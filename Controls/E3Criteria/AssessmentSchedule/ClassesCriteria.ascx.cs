using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.E3Criteria.AssessmentSchedule
{
    public partial class ClassesCriteria : CriteriaBase
    {
        public object JsonDataSource;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //
                //Setup Criteria
                //
                RestrictValueCount = RestrictValueOptions.OnlyOne_Period;

                txtTeacher.Attributes.Add("CriteriaName", CriteriaName);
                txtTeacher.ClientEvents.OnBlur = CriteriaName + "Controller.OnTextChanged";
                txtSection.ClientEvents.OnBlur = CriteriaName + "Controller.OnTextChanged";
                txtBlock.ClientEvents.OnBlur = CriteriaName + "Controller.OnTextChanged";

                //
                // Setup cmbSemester
                //
                cmbSemester.Attributes.Add("CriteriaName", CriteriaName);
                cmbSemester.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbSemester.OnClientBlur = CriteriaName + "Controller.OnComboChanged";

                //
                // Setup cmbPeriod
                //
                cmbPeriod.Attributes.Add("CriteriaName", CriteriaName);
                cmbPeriod.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbPeriod.OnClientBlur = CriteriaName + "Controller.OnComboChanged";

                //cmbPeriod.OnClientLoad = CriteriaName + "Controller.PopulateControls";  // did this on .aspx for this control
                ValueDisplayTemplateName = "ClassesCriteriaValueDisplayTemplate";

                //
                // Create Json array strings for populating our dropdownlist controls, and register them as javascript;
                //
                var serializer = new JavaScriptSerializer();
                var arry = new ArrayList();
               
                // data for cmbSemester
                arry.Clear();                
                arry = Semester.BuildJsonArray();
                ScriptManager.RegisterStartupScript(this, typeof(string), "ClassesCrit_SemesterDependencyData", "var ClassesCrit_SemesterDependencyData = " + serializer.Serialize(arry) + ";", true);

                // data for cmbPeriod
                arry.Clear();                
                arry = Period.BuildJsonArray();
                ScriptManager.RegisterStartupScript(this, typeof(string), "ClassesCrit_PeriodDependencyData", "var ClassesCrit_PeriodDependencyData = " + serializer.Serialize(arry) + ";", true);

            }

            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);

        }

        public class ValueObject
        {
            public string Teacher { get; set; }
            public string Semester { get; set; }
            public string Period { get; set; }
            public string Section { get; set; }
            public string Block { get; set; }
        }

    }
}