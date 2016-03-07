using System;
using System.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class SchoolTypeSchool : CriteriaBase
    {
        public object JsonDataSource;
        public DropDownList CmbSchoolType
        {
            get { return cmbSchoolType; }
        }

        public DropDownList CmbSchool
        {
            get { return cmbSchool; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cmbSchoolType.OnChange = CriteriaName + "Controller.OnChange();";
                cmbSchool.OnChange = CriteriaName + "Controller.OnChange();";
                cmbSchool.OnClientLoad = CriteriaName + "Controller.PopulateControls";

                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);      
            }
        }
    }
}