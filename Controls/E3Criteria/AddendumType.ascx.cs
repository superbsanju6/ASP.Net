using System;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class AddendumType : CriteriaBase
    {
        public object ParentDataSource;
        public string ParentDataTextField;
        public string ParentDataValueField;
        public object ChildDataSource;
        public string ChildDataTextField;
        public string ChildDataValueField;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RadListBox1.OnClientItemChecked = CriteriaName + "Controller.OnCheck";
                RadListBox2.OnClientItemChecked = CriteriaName + "Controller.OnCheck";
                RadListBox2.OnClientLoad = CriteriaName + "Controller.InitialGenreHide";
                if (!Width.IsEmpty) RadToolTip1.Width = Width;
                DataBind();
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public override void DataBind()
        {
            RadListBox1.DataSource = ParentDataSource;
            RadListBox1.DataTextField = ParentDataTextField;
            RadListBox1.DataValueField = ParentDataValueField;
            RadListBox1.DataBind();
            RadListBox2.DataSource = ChildDataSource;
            RadListBox2.DataTextField = ChildDataTextField;
            RadListBox2.DataValueField = ChildDataValueField;
            RadListBox2.DataBind();
            base.DataBind();

        }

        public class ValueObject
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public string Genre { get; set; }
        }

    }
}