using System;
using System.Web.UI;
using Thinkgate.Base.Classes;
using System.Web.Script.Serialization;

namespace Thinkgate.Controls.E3Criteria.Associations
{
    public partial class Classes : CriteriaBase
    {
        public object JsonDataSource;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cmbGrade.Attributes.Add("CriteriaName", CriteriaName);
                cmbGrade.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbSubject.Attributes.Add("CriteriaName", CriteriaName);
                cmbSubject.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbCourse.Attributes.Add("CriteriaName", CriteriaName);
                cmbCourse.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";
                cmbCourse.OnClientLoad = CriteriaName + "Controller.PopulateControls";
                ValueDisplayTemplateName = "ClassesCriteriaValueDisplayTemplate";
                RestrictValueCount = RestrictValueOptions.OnlyOne_Period;
                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);   
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public class ValueObject
        {
            public string Grade { get; set; }
            public string Subject { get; set; }
            public string Course { get; set; }
        }  
        
    }
}