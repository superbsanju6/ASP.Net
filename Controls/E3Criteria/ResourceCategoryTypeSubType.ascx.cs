using System;
using System.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class ResourceCategoryTypeSubType : CriteriaBase
    {
        public object JsonDataSource;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                chkCategory.OnChange = CriteriaName + "Controller.OnChange();";
                chkCategory.Width = 240;
                chkType.OnChange = CriteriaName + "Controller.OnChange();";
                
                chkSubtype.OnClientLoad = CriteriaName + "Controller.PopulateControls";


                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);      
            }
        }
    }
}