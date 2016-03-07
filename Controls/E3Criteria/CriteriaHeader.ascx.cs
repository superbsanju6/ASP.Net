using System;
using Telerik.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    /// <summary>
    /// This generates the header node (visible item)
    /// </summary>
    public partial class CriteriaHeader : System.Web.UI.UserControl
    {
        #region Local Accessor Properties From Parent Control
        public string Text { get; set; }
        public string CriteriaName { get; set; }
        public string ValueDisplayTemplateName { get; set; }
        public bool Required { get; set; }
        public RadToolTip ToolTip { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ToolTip != null) ToolTip.TargetControlID = expand_bubble.UniqueID;          // this is what connects the header control to the tooltip
            RequiredSpan.Visible = Required;
        }

       
    }
}