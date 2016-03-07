using System;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class AddendumSearch : CriteriaBase
    {
        public string TestCategory { get; set; }
        public int AssessmentID { get; set; }
                
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!Width.IsEmpty) RadToolTip1.Width = Width;
                DataBind();
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public override void DataBind()
        {
            base.DataBind();

        }
        
        public class ValueObject
        {
            public string Value { get; set; }
            public string Text { get; set; }
            public string ID { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Genre { get; set; }
        }

    }
}