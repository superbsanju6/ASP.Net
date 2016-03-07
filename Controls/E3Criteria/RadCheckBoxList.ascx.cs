using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class RadCheckBoxList : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        public string DataValueField;
        public string OnClientLoad;
        public List<String> DefaultTexts;
        public bool LoadDefaults = true;
        public bool AutoPostBack = false;
        public string SelectedText = string.Empty;
        public string SelectedValue = string.Empty;
        public bool EnableLoadOnDemand = false;
        public event RadComboBoxSelectedIndexChangedEventHandler SelectedIndexChanged;
        public event RadComboBoxItemsRequestedEventHandler ItemsRequested;
        public string OnClientItemsRequesting { get; set; }
        public string OnClientDropDownOpening { get; set; }
        public string EmptyMessage { get; set; }
        public string OnClientDropDownOpened { get; set; }
        public string RadClientId = string.Empty;
        public bool IsNotAllowToEnterText { get; set; }
        public bool EnableCheckAllItemsCheckBox { get; set; }
        public string OnRemoveByKeyHandler { get; set; }

        public bool CheckBoxes = true;


        protected void Page_Load(object sender, EventArgs e)
        {

            RadListBox1.OnClientItemChecked = CriteriaName + "Controller.OnCheck";
            RadListBox1.EmptyMessage = EmptyMessage;
            if (!String.IsNullOrEmpty(OnClientLoad))
            {
                RadListBox1.OnClientLoad = OnClientLoad;
            }
            else
            {
                if (DefaultTexts != null && DefaultTexts.Count > 0 && LoadDefaults)
                {
                    RadListBox1.OnClientLoad = CriteriaName + "Controller.CheckDefaultTexts";
                }
            }
            if (!Width.IsEmpty) RadToolTip1.Width = Width;
            if (!IsPostBack)
            {
                DataBind();
            }


            if (!string.IsNullOrWhiteSpace(this.OnClientDropDownOpened))
                this.RadListBox1.OnClientDropDownOpened = this.OnClientDropDownOpened;


            this.RestrictValueCount = this.CheckBoxes ? CriteriaBase.RestrictValueOptions.Unlimited : CriteriaBase.RestrictValueOptions.OnlyOneAppliedAtATime;

            this.RadListBox1.CheckBoxes = this.CheckBoxes;
            if (!this.CheckBoxes)
                RadListBox1.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
            this.RadListBox1.AutoPostBack = this.AutoPostBack;
            this.SelectedValue = this.RadListBox1.SelectedValue;
            this.SelectedText = this.RadListBox1.SelectedItem != null ? this.RadListBox1.SelectedItem.Text : "";
            if (this.AutoPostBack)
                this.RadListBox1.SelectedIndexChanged += SelectedIndexChanged;

            this.RadListBox1.EnableLoadOnDemand = this.EnableLoadOnDemand;
            if (this.EnableLoadOnDemand)
                this.RadListBox1.ItemsRequested += ItemsRequested;

            if (!string.IsNullOrWhiteSpace(OnClientItemsRequesting))
                this.RadListBox1.OnClientItemsRequesting = this.OnClientItemsRequesting;

            if (!string.IsNullOrWhiteSpace(OnClientDropDownOpening))
                this.RadListBox1.OnClientDropDownOpening = this.OnClientDropDownOpening;


            this.RadListBox1.EnableCheckAllItemsCheckBox = this.EnableCheckAllItemsCheckBox;

            if (this.EnableCheckAllItemsCheckBox)
            {
                var _script = string.Format(@"function CheckAll(sender, args)
                                {{                                    
                                   $('#' + sender._dropDownElement.id).find('.rcbCheckAllItemsCheckBox').each(function(){{                                        
                                        $(this).off('change').on('change',function(){{     
                                             $(this).parent('label').click(function(){{                                                  
//                                                       setTimeout(function(){{ 
//                                                                            if(!$('#' + sender._dropDownElement.id).find('.rcbCheckAllItemsCheckBox').is(':checked') && $('#' + sender._dropDownElement.id).find('.rcbList input[type=checkbox]:checked').length != $('#' + sender._dropDownElement.id).find('.rcbList input[type=checkbox]').length && $('#' + sender._dropDownElement.id).find('.rcbList input[type=checkbox]:checked').length > 0)
//                                                                                $('#' + sender._dropDownElement.id).find('.rcbCheckAllItemsCheckBox').trigger('click')                                                       
//                                                                            }}                                                             
//                                                                    , 1000);
                                            }});
                                            var i = 0;
                                            setInterval(function(){{ 
                                                                        {0}Controller.AddByCheckAll(sender, args);                                                                          
                                                                 }} 
                                            , 1000);
                                        }})                                                                              
                                    }})
                                    var i = 0;
                                    setInterval(function(){{
                                                            if($('#' + sender._dropDownElement.id).find('.rcbList').find('li').length > 1 && i < 1 )
                                                            {{
                                                               $('#' + sender._dropDownElement.id).find('.rcbCheckAllItemsCheckBox').trigger('click') 
                                                               i = i + 1;
                                                            }}
                                                        }}, 1000);
                                }}", CriteriaName);
                ScriptManager.RegisterStartupScript(this, typeof(string), "CheckAll", _script, true);
                this.RadListBox1.OnClientItemsRequested = "CheckAll";
            }

            this.RadClientId = this.RadListBox1.ClientID;

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

            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public override void DataBind()
        {
            RadListBox1.DataSource = DataSource;
            RadListBox1.DataTextField = DataTextField;
            RadListBox1.DataValueField = DataValueField;
            RadListBox1.DataBind();
            base.DataBind();
            if (DefaultTexts != null)
            {
                foreach (var txt in DefaultTexts)
                {
                    var item = RadListBox1.FindItemByText(txt);
                    if (item != null) item.Checked = true;
                }

            }
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



        /* functions that could be useful, but ultimately I phased out */
        #region useful but unsed methods
        /*
        public IEnumerable<object> GetSelectedValues()
        {
            //var criteria = selectedCriteria.Find(x => x.CriteriaName.Equals(criteriaName));
            //if (criteria.Count == 0) return null;
            return selectedCriteria.CriteriaItems.Select(x => x.Value);
        }

        public static IEnumerable<object> GetSelectedValues(SelectedCriteria selectedCriteria)
        {
            //var criteria = selectedCriteria.Find(x => x.CriteriaName.Equals(criteriaName));
            //if (criteria.Count == 0) return null;
            return selectedCriteria.CriteriaItems.Select(x => x.Value);            
        }*/

        /*
        public IList<RadListBoxItem> CheckedItems
        {
            get { return RadListBox1.CheckedItems; }
        }

        public IEnumerable<String> SelectedTexts()
        {
            return RadListBox1.CheckedItems.Select(item => item.Text);
        }

        public IEnumerable<String> SelectedValues()
        {
            return RadListBox1.CheckedItems.Select(item => item.Value);
        }*/
        #endregion

        public string SelectedValues { get; set; }


    }
}