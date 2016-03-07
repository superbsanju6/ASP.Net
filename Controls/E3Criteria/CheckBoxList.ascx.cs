using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class CheckBoxList : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        public string DataValueField;
        public string OnClientLoad;
        public List<String> DefaultTexts;
        public bool LoadDefaults = true;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RadListBox1.OnClientItemChecked = CriteriaName + "Controller.OnCheck";
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
                
                DataBind();
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
            if (DefaultTexts != null )
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
    }
}