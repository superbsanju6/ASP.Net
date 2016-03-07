using System;
using System.Web.UI;

namespace Thinkgate.Controls.E3Criteria.Associations
{
    public partial class Schools : CriteriaBase
    {
        public object JsonDataSource;
        public Boolean IncludeSchoolIdControl = false; // When set to True, the "School ID" control will added to the display.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cmbSchoolType.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                cmbSchoolType.OnClientBlur = CriteriaName + "Controller.OnChange";
                cmbSchool.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                cmbSchool.OnClientBlur = CriteriaName + "Controller.OnChange";

                if (IncludeSchoolIdControl)
                {
                    txtSchoolId.ClientEvents.OnBlur = CriteriaName + "Controller.OnTextChanged";
                }
                
                ValueDisplayTemplateName = "SchoolsCriteriaValueDisplayTemplate";
                RestrictValueCount = RestrictValueOptions.OnlyOne_Period;
                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);

                if (!IncludeSchoolIdControl)
                {
                    // Controller.Populate controls must be called on last item in the criteria list.  In this case, cmbSchool is the last item.
                    cmbSchool.OnClientLoad = CriteriaName + "Controller.PopulateControls";
                }
                
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public class ValueObject
        {
            public string SchoolType { get; set; }
            public string School { get; set; }
            public string SchoolId { get; set; }
        }  
        
    }
}