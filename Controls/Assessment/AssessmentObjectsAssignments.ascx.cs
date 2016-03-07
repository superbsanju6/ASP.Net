using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentObjectsAssignments : TileControlBase
    {
        protected Int32 _assessmentID;
        private Base.Classes.Assessment _assessment;
        // True if this is a postback.
        protected Boolean _isPostBack;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            if (Tile == null) return;
            _assessmentID = (Int32)Tile.TileParms.GetParm("assessmentID");
            _assessment = (Base.Classes.Assessment)Tile.TileParms.GetParm("assessment");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            String controlId = GetControlThatCausedPostBack(Parent.Page);
            // Simulate IsPostBack.
            _isPostBack = (controlId != null);

            // Initial load.
            if (!_isPostBack)
            {
                hiddenAssessmentID.Value = _assessmentID.ToString();
                BuildUi();
            }

        }

        protected void BuildUi()
        {
            // Limit results to 500 rows.
            const Int32 maxRows = 500;
            DataTable dtAssignments = Thinkgate.Base.Classes.Assessment.GetAssignments(_assessmentID);
            while (dtAssignments.Rows.Count > maxRows)
                dtAssignments.Rows.RemoveAt(maxRows - 1);
            DataColumn percentageCol = dtAssignments.Columns.Add("Percentage", typeof(Int32));
            foreach (DataRow row in dtAssignments.Rows)
                row[percentageCol] = (Int32)Math.Round(DataIntegrity.ConvertToDouble(row["Score"]) * 100);
            dtAssignments = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dtAssignments, "ClassID", "ClassID_Encrypted");


            // Bind to the grid.
            grdAssignments.DataSource = dtAssignments;
            grdAssignments.DataBind();

            // Show 'More Results' if we have the maximum number of results (500).
            if (dtAssignments.Rows.Count >= maxRows)
            {
                assignmentsSearchMoreLink.Visible = true;
                var js = "searchSmallTileAppendMoreLinkOnLoad('" + grdAssignments.ClientID + "', 'assignmentsSearchMoreLinkSpan');";
                ScriptManager.RegisterStartupScript(Page, typeof(TileControlBase), "AssessmentAssignmentsMoreLinkAppend", js, true);
            }
        }

        protected void grdAssignments_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
        }

        protected void grdAssignments_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem gridItem = (GridDataItem)e.Item;
                HyperLink lnk = (HyperLink)gridItem["LnkLevelName"].Controls[0];
                DataRowView row = (DataRowView)(gridItem).DataItem;
                lnk.NavigateUrl = "~/Record/Class.aspx?xID=" + row["ClassID_Encrypted"];
            }
        }

    }
}