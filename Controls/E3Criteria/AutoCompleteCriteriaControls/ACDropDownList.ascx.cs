using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Web.UI;

namespace Thinkgate.Controls.E3Criteria.AutoCompleteCriteriaControls
{
    public partial class ACDropDownList : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        public string DataValueField;
        public string OnClientLoad;
        public string EmptyMessage;
        public List<String> DefaultTexts;
        public object JsonDataSource;
        public bool LoadDefaults = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ACComboBox.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // dropdown list can only have 1 value
                ACComboBox.EmptyMessage = EmptyMessage;
                if (!String.IsNullOrEmpty(OnClientLoad))
                {
                    ACComboBox.OnClientLoad = OnClientLoad;
                }
                else
                {
                    if (DefaultTexts != null && DefaultTexts.Count > 0 && LoadDefaults)
                    {
                        ACComboBox.OnClientLoad = CriteriaName + "Controller.CheckDefaultTexts";
                    }
                }
                if (JsonDataSource != null) ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);
                DataBind();
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);

            ACComboBox.Filter = RadComboBoxFilter.Contains;
        }

        public override void DataBind()
        {
            ACComboBox.DataSource = DataSource;
            ACComboBox.DataTextField = DataTextField;
            ACComboBox.DataValueField = DataValueField;
            ACComboBox.DataBind();
            base.DataBind();
        }

        public string DefaultTextAsJs()
        {
            return (new JavaScriptSerializer()).Serialize(DefaultTexts);
        }

        public class ValueObject
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }

        public RadComboBox ComboBox
        {
            get { return ACComboBox; }
        }

        public RadToolTip ToolTip
        {
            get { return RadToolTip1; }
        }
    }
}