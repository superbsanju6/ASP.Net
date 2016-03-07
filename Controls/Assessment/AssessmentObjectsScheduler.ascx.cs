using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentObjectsScheduler : TileControlBase
    {
        protected Int32 _assessmentID;
        // True if this is a postback.
        protected Boolean _isPostBack;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            if (Tile == null) return;
            _assessmentID = (Int32)Tile.TileParms.GetParm("assessmentID");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            // Simulate IsPostBack.
            Control postBackControl = GetControlObjThatCausedPostBack(Parent.Page);
            _isPostBack = (postBackControl != null);

            // Initial load.
            if (!_isPostBack)
            {
                BuildUI();
            }
        }

        protected void BuildUI()
        {
            DataRow row = Thinkgate.Base.Classes.Assessment.GetAssessmentSchedule(_assessmentID);
            if (row != null)
            {
                lblAdminStatus.InnerText = row["Administration"].ToString();
                lblContStatus.InnerText = row["CONTENT"].ToString();
                lblPrintStatus.InnerText = row["Print"].ToString();
            }
        }        
    }
}