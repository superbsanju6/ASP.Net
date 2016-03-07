using System;
using System.Web.UI;

namespace Thinkgate.Controls.E3Criteria.Associations
{
    public partial class Documents : CriteriaBase
    {
        public object JsonDataSource;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cmbTemplateName.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                cmbTemplateType.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                txtDocName.ClientEvents.OnLoad = CriteriaName + "Controller.PopulateControls";
                
                txtDocName.Attributes.Add("CriteriaName", CriteriaName);
                txtDocName.ClientEvents.OnBlur = CriteriaName + "Controller.OnChange";
                
                ValueDisplayTemplateName = "DocumentsCriteriaValueDisplayTemplate";
                RestrictValueCount = RestrictValueOptions.OnlyOne_Period;
                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);   
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public class ValueObject
        {
            public string TemplateType { get; set; }
            public string TemplateName { get; set; }
            public string DocumentName { get; set; }
        }  
        
    }
}