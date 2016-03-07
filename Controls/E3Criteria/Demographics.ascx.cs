using System;
using Thinkgate.Base.Classes;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Linq;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class Demographics : CriteriaBase
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadControl();
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        /// <summary>
        /// This method loads and databinds the controls within the Demographics UserControl
        /// </summary>
        protected void LoadControl()
        {
            bReset.OnClientClicked = CriteriaName + "Controller.Clear";
            rptDropdown.DataSource = DemographicMasterList.TypesByValueType(DemographicType.DemoValueType.List);
            rptDropdown.DataBind();
            rptRadio.DataSource = DemographicMasterList.TypesByValueType(DemographicType.DemoValueType.Boolean);
            rptRadio.DataBind();
            ValueDisplayTemplateName = "DemographicsCriteriaValueDisplayTemplate";      
        }

        /// <summary>
        /// This method clears and resets the current Demographics control
        /// </summary>
        public void Clear()
        {
            LoadControl();
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        protected void BindComboValues(object sender, RepeaterItemEventArgs e)
        {
            var combo = (RadComboBox) e.Item.FindControl("demoCombo");
            var type = (DemographicType) e.Item.DataItem;
            combo.Items.Add(new RadComboBoxItem("Select a " + type.Label, "-1"));
            combo.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
            combo.Attributes.Add("DemoField", type.DemoField.ToString());
            combo.Attributes.Add("DemoLabel", type.Label);
            combo.CssClass = CriteriaName + "Finder";
            combo.DataSource = DemographicMasterList.Values.Where(x => x.Label == type.Label);
            combo.DataBind();
        }

        protected void BindRadios(object sender, RepeaterItemEventArgs e)
        {
            var type = (DemographicType)e.Item.DataItem;
            ConfigureRadio((RadButton) e.Item.FindControl("bAll"), type);
            ConfigureRadio((RadButton) e.Item.FindControl("bYes"), type);
            ConfigureRadio((RadButton) e.Item.FindControl("bNo"), type);
        }

        private void ConfigureRadio(RadButton button, DemographicType type)
        {
            button.OnClientClicked = CriteriaName + "Controller.OnChange";
            button.GroupName = type.DemoField.ToString();
            button.Attributes.Add("DemoField", type.DemoField.ToString());
            button.Attributes.Add("DemoLabel", type.Label);
            button.CssClass = CriteriaName + "Finder";
        }

        [Serializable]
		public class ValueObject
        {
            public string DemoLabel { get; set; }
            public string DemoField { get; set; }
            public string DemoValue { get; set; }
            public string DemoValueText { get; set; }
        }  
        
    }
}