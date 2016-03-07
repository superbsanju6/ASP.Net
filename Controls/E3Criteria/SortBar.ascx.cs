using System;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class SortBar : System.Web.UI.UserControl
    {
        //public event EventHandler<SortChangeEvent> Sort;

        public event EventHandler ExcelClick;
        public object DataSource; 
        public bool ShowSelectAndReturnButton;
        public NameValue SelectedSort;
        public bool ShowExcelButton;
        public bool ShowSortDropdown = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                imgExcel.Visible = ShowExcelButton;
                sortSpan.Visible = ShowSortDropdown;
                DataBind();
            }
        }

        public string SelectedSortText()
        {
            return sortByDropdown.Text;
        }

        protected void ExcelButtonClick(object sender, EventArgs e)
        {
            if (ExcelClick != null)
                ExcelClick(this, e);
        }

        /*protected void SortChange(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (Sort != null)
                Sort(this, new SortChangeEvent(e.Text, e.Value));
        }*/

        public override void DataBind()
        {
            sortByDropdown.DataSource = DataSource; 
            sortByDropdown.DataBind();
            base.DataBind();

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            btnSelectReturn.Visible = ShowSelectAndReturnButton;
        }

        public void HideExcelButtonClientSide()
        {
            imgExcel.Visible = true;
            imgExcel.Style.Add("display", "none");
        }
    }
    /*
    public class SortChangeEvent : EventArgs
    {
        public string Value;
        public string Text;
        
        public SortChangeEvent(string text, string value)
        {
            Text = text;
            Value = value;
        }
    }*/
}