using System;

namespace Thinkgate.Controls.E3Criteria.Associations
{
    public partial class Students : CriteriaBase
    {
        public object DataSource;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtName.ClientEvents.OnBlur = CriteriaName + "Controller.OnChange";
                txtId.ClientEvents.OnBlur = CriteriaName + "Controller.OnChange";

                cmbGrade.DataSource = DataSource;
                cmbGrade.DataBind();
                cmbGrade.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                
                ValueDisplayTemplateName = "StudentsCriteriaValueDisplayTemplate";
                RestrictValueCount = RestrictValueOptions.OnlyOne_Period;
                
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public class ValueObject
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string Grade { get; set; }
        }  
        
    }
}