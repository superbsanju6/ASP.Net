using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class DropDownList : CriteriaBase
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
                RadComboBox1.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // dropdown list can only have 1 value
                RadComboBox1.EmptyMessage = EmptyMessage;
                if (!String.IsNullOrEmpty(OnClientLoad))
                {
                    RadComboBox1.OnClientLoad = OnClientLoad;
                } else
                {
                    if (DefaultTexts != null && DefaultTexts.Count > 0 && LoadDefaults)
                    {
                        RadComboBox1.OnClientLoad = CriteriaName + "Controller.CheckDefaultTexts";
                    }
                }
                if (JsonDataSource != null) ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);      
                DataBind();
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public override void DataBind()
        {
            RadComboBox1.DataSource = DataSource;
            RadComboBox1.DataTextField = DataTextField;
            RadComboBox1.DataValueField = DataValueField;
            RadComboBox1.DataBind();
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
            get { return RadComboBox1; }
        }
   
        public RadToolTip ToolTip
        {
            get { return RadToolTip1; }
        }
    }
}