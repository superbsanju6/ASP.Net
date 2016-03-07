using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class RadDropDownList : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        public string DataValueField;
        public string OnClientLoad;
        public string EmptyMessage;
        public List<String> DefaultTexts;
        public object JsonDataSource;
        public bool LoadDefaults = true;
        public bool AutoPostBack = false;
        public string SelectedText = string.Empty;
        public string SelectedValue = string.Empty;
        public bool EnableLoadOnDemand = false;
        public event RadComboBoxSelectedIndexChangedEventHandler SelectedIndexChanged;
        public event RadComboBoxItemsRequestedEventHandler ItemsRequested;
        public string OnClientItemsRequesting { get; set; }
        public string OnClientDropDownOpening { get; set; }
        public string RadClientId = string.Empty;
        public bool EnableVirtualScrolling = false;
        public Unit Height = 55;
        public string OnClientDropDownOpened { get; set; }
        public bool IsNotAllowToEnterText { get; set; }
        public string OnRemoveByKeyHandler { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            RadComboBox1.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
            RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // dropdown list can only have 1 value
            RadComboBox1.EmptyMessage = EmptyMessage;
            if (!String.IsNullOrEmpty(OnClientLoad))
            {
                RadComboBox1.OnClientLoad = OnClientLoad;
            }
            else
            {
                if (DefaultTexts != null && DefaultTexts.Count > 0 && LoadDefaults)
                {
                    RadComboBox1.OnClientLoad = CriteriaName + "Controller.CheckDefaultTexts";
                }
            }
            if (JsonDataSource != null) ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);
            if (!IsPostBack)
            {
                DataBind();
            }


            this.RadComboBox1.AutoPostBack = this.AutoPostBack;
            this.SelectedValue = this.RadComboBox1.SelectedItem != null ? this.RadComboBox1.SelectedItem.Value : "";
            this.SelectedText = this.RadComboBox1.SelectedItem != null ? this.RadComboBox1.SelectedItem.Text : "";
            if (this.AutoPostBack)
                this.RadComboBox1.SelectedIndexChanged += SelectedIndexChanged;

            if (!string.IsNullOrWhiteSpace(this.OnClientDropDownOpened))
                this.RadComboBox1.OnClientDropDownOpened = this.OnClientDropDownOpened;

            this.RadComboBox1.EnableLoadOnDemand = this.EnableLoadOnDemand;
            if (this.EnableLoadOnDemand)
                this.RadComboBox1.ItemsRequested += ItemsRequested;

            if (!string.IsNullOrWhiteSpace(OnClientItemsRequesting))
                this.RadComboBox1.OnClientItemsRequesting = this.OnClientItemsRequesting;

            if (!string.IsNullOrWhiteSpace(OnClientDropDownOpening))
                this.RadComboBox1.OnClientDropDownOpening = this.OnClientDropDownOpening;

            if (IsNotAllowToEnterText)
            {
                var _script = @"function TextChanging(sender, args)
                                {                                    
                                   $('#' + sender._popupElement.id).find('.rcbInputCell').each(function(){                                        
                                        $(this).find('input[type=text]').off('focus').on('focus',function(){
                                            $(this).blur();
                                        })
                                        $(this).find('input[type=text]').off('keypress').on('keypress',function(event){                                                                                                          
                                            event.preventDefault();
                                        }).css('cursor','default').css('text-shadow', '0px 0px 0px #000');
                                    })
                                }";
                ScriptManager.RegisterStartupScript(this, typeof(string), "DisableTextEnter", _script, true);
                this.RadToolTip1.OnClientShow = "TextChanging";
            }


            this.RadComboBox1.EnableVirtualScrolling = this.EnableVirtualScrolling;

            this.RadComboBox1.Height = this.Height;

            this.RadClientId = this.RadComboBox1.ClientID;

            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
            RadComboBox1.Filter = RadComboBoxFilter.Contains;
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