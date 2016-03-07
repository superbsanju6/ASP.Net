using System;
using System.Collections;
using System.Web.UI;
using Thinkgate.Base.Classes;
using System.Web.Script.Serialization;

namespace Thinkgate.Controls.E3Criteria.Associations
{
    public partial class Standards : CriteriaBase
    {
        public object JsonDataSource;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cmbStandardSet.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbGrade.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbSubject.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbCourse.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbCourse.OnClientLoad = CriteriaName + "Controller.PopulateControls";
                cmbStandard.OnClientItemsRequesting = CriteriaName + "Controller.OnClientRequesting";
                cmbStandard.OnClientItemChecked = CriteriaName + "Controller.OnChecked";

                cmbStandard.Attributes.Add("CriteriaName", CriteriaName);
                ValueDisplayTemplateName = "StandardsCriteriaValueDisplayTemplate";
                RestrictValueCount = RestrictValueOptions.OnlyOne_Period;
                ScriptManager.RegisterStartupScript(this, typeof(string), "StandardsDependencyData", "var StandardsDependencyData = " + JsonDataSource + ";", true);           
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public class ValueObject
        {
            public string StandardSet { get; set; }
            public string Grade { get; set; }
            public string Subject { get; set; }
            public string Course { get; set; }
            public ArrayList Standards { get; set; }
            public string StandardName { get; set; }
        }      
        
    }
}