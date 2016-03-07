using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.E3Criteria.AssessmentSchedule
{
    public partial class DistrictCriteria : CriteriaBase
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

                //
                // Setup cmbDistrict
                //
                cmbDistrict.Attributes.Add("CriteriaName", CriteriaName);
                cmbDistrict.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnComboChanged";

                cmbDistrict.OnClientLoad = CriteriaName + "Controller.PopulateControls";

                ValueDisplayTemplateName = "DistrictCriteriaValueDisplayTemplate";

                //
                // Create Json array strings for populating our dropdownlist controls, and register them as javascript;
                //
                var serializer = new JavaScriptSerializer();
                var arry = new ArrayList();

                // data for cmbDistrict
                arry.Clear();
                // todo: mpf - populate district data. //arry = Semester.BuildJsonArray();
                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + serializer.Serialize(arry) + ";", true);
            }

            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public class ValueObject
        {
            public string District { get; set; }
        }
    }
}