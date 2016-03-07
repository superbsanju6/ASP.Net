using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Web.UI;



namespace Thinkgate.Controls.E3Criteria.AutoCompleteCriteriaControls
{
    public partial class ACSchoolTeacher : CriteriaBase
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

                ddlSchool.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                ddlTeacher.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChangeTeacher";
                RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // dropdown list can only have 1 value
                ddlSchool.EmptyMessage = EmptyMessage;
                if (!String.IsNullOrEmpty(OnClientLoad))
                {
                    ddlSchool.OnClientLoad = OnClientLoad;
                }
                else
                {
                    if (DefaultTexts != null && DefaultTexts.Count > 0 && LoadDefaults)
                    {
                        ddlSchool.OnClientLoad = CriteriaName + "Controller.CheckDefaultTexts";
                    }
                }
                if (JsonDataSource != null) ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);
                DataBind();
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);

            ddlSchool.Filter = RadComboBoxFilter.Contains;
            ddlTeacher.Filter = RadComboBoxFilter.Contains;
        }

        public override void DataBind()
        {
            ddlSchool.DataSource = DataSource;
            ddlSchool.DataTextField = DataTextField;
            ddlSchool.DataValueField = DataValueField;
            ddlSchool.DataBind();
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
            get { return ddlSchool; }
        }

        public RadToolTip ToolTip
        {
            get { return RadToolTip1; }
        }
    }
}