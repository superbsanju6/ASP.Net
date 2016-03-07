using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Telerik.Web;
using Telerik.Charting;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Collections.Generic;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentScans : BasePage
	{
		DataSet _dsAssessmentAdmin;
		DataTable _dtScans;
		Int32 _userID, _assessmentID, _classID;
        bool _isGroup;
		SessionObject sessionObject;
	    private string _level;

		protected new void Page_Init(object sender, EventArgs e)
		{
		    _level = Request.QueryString["level"];
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			sessionObject = (SessionObject)Session["SessionObject"];
			_userID = sessionObject.LoggedInUser.Page;

			if (Request.QueryString["xID"] == null || !Int32.TryParse(Request.QueryString["xID"], out _assessmentID))
			{
				sessionObject.RedirectMessage = "No assessment ID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else if (_level != "assessment" && (Request.QueryString["yID"] == null ||  !Int32.TryParse(Request.QueryString["yID"], out _classID)))
			{
				sessionObject.RedirectMessage = "No class ID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
            else if (_level != "assessment" && (Request.QueryString["isGroup"] == null || !Boolean.TryParse(Request.QueryString["isGroup"], out _isGroup)))
            {
                sessionObject.RedirectMessage = "Unable to get Group Status.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
			else
			{
                inpxID.Value = Request.QueryString["xID"].ToString();
				if(!IsPostBack)
				{
					LoadTableData();
					lblNumRecs.Text = "Records Found: " + _dtScans.Rows.Count.ToString();

                    // TFS #3549 : Show 'Upload Results' button, if only, assessment is proofed.
                    btnUpload.Visible = IsAssessmentProofed();
				}
			}
		}

		protected void LoadTableData()
		{
            if (_level == "assessment")
            {
                _dtScans = Thinkgate.Base.Classes.Assessment.LoadAssessmentScansByTestID(_assessmentID, 1);
                grdScans.AllowPaging = true;
                grdScans.PageSize = 100;
            }
            else
            {
                _dsAssessmentAdmin = Base.Classes.Assessment.LoadAssessmentAdmin(_userID, _assessmentID, _classID, _isGroup);
                _dtScans = _dsAssessmentAdmin.Tables[2];
            }
		    // Add a data column that will be evaluated in the table data binding to show the Reject/Repair icon if error.
			DataColumn col = _dtScans.Columns.Add("RepairVisibility", typeof(String));
			foreach(DataRow row in _dtScans.Rows)
				row[col] = ((Int32)row["#Error"] > 0) ? "visible" : "hidden";

			// We must save the data table in view state to allow sorting later.
			ViewState["_dtScans"] = _dtScans;

			grdScans.DataSource = _dtScans;
			grdScans.DataBind();
		}

        private bool IsAssessmentProofed()
        {
            Thinkgate.Base.Classes.Assessment assessment = Thinkgate.Base.Classes.Assessment.GetAssessmentByID(_assessmentID);
            return assessment.IsProofed;
        }

		/// <summary>
		/// Gotta have it for sort to work ??? wierd.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void grdScans_SortCommand(object source, Telerik.Web.UI.GridSortCommandEventArgs e)
		{
		}

		protected void grdScans_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
		{
			String gridSortString = grdScans.MasterTableView.SortExpressions.GetSortString();
			_dtScans = (DataTable)ViewState["_dtScans"];

			if (_dtScans != null)
			{
				DataView view = _dtScans.AsDataView();
				view.Sort = gridSortString;
				grdScans.DataSource = _dtScans;
			}
		}

	}
}