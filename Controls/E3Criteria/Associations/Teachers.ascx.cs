using System;

namespace Thinkgate.Controls.E3Criteria.Associations
{
    public partial class Teachers : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtName.ClientEvents.OnBlur = CriteriaName + "Controller.OnChange";
                txtUserId.ClientEvents.OnBlur = CriteriaName + "Controller.OnChange";

                cmbUserType.DataTextField = DataTextField;
                cmbUserType.DataSource = DataSource;
                cmbUserType.DataBind();
                cmbUserType.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";

                ValueDisplayTemplateName = "TeachersCriteriaValueDisplayTemplate";
                RestrictValueCount = RestrictValueOptions.OnlyOne_Period;
                
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public class ValueObject
        {
            public string Name { get; set; }
            public string UserType { get; set; }
            public string UserId { get; set; }
        }  
        
    }
}