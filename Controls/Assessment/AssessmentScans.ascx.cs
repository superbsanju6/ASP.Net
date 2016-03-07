using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.Assessment
{
		public partial class AssessmentScans : TileControlBase
		{
		    private Thinkgate.Base.Enums.EntityTypes _level;
		    private Int32 _levelID;            
            private DataTable _dtScans;
            private Int32 _userID;
            private Int32 _assessmentID;
            private SessionObject _sessionObject;

		    protected new void Page_Init(object sender, EventArgs e)
		    {
			    base.Page_Init(sender, e);

			    if (Tile == null) return;

			    _assessmentID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("assessmentID"));
                _level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
			    _levelID = (Int32)Tile.TileParms.GetParm("levelID");
		    }

		    protected void Page_Load(object sender, EventArgs e)
		    {
			    if (Tile == null) return;

                _sessionObject = (SessionObject)Session["SessionObject"];
                _userID = _sessionObject.LoggedInUser.Page;

                if (!IsPostBack)
                {
                    LoadTableData();
                    scansearch_hiddenAssessmentID.Value = _assessmentID.ToString();
                }
		    }

            protected void LoadTableData()
            {
                _dtScans = Thinkgate.Base.Classes.Assessment.LoadAssessmentScansByTestID(_assessmentID);

                // We must save the data table in view state to allow sorting later.
                ViewState["_dtScans"] = _dtScans;

                grdScans.DataSource = _dtScans;
                grdScans.DataBind();

                if (_dtScans.Rows.Count > 0 && DataIntegrity.ConvertToInt(_dtScans.Rows[0]["JobCount"]) > 100)
                {
                    scansSearchMoreLink.Visible = true;

                    var js = "searchSmallTileAppendMoreLinkOnLoad('" + grdScans.ClientID + "', 'scansSearchMoreLinkSpan');";
                    ScriptManager.RegisterStartupScript(Page, typeof(TileControlBase), "AssessmentScansMoreLinkAppend", js, true);
                }
            }

            public void GrdScans_ItemDataBound(object sender, GridItemEventArgs e)
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    var dataItem = (DataRowView)item.DataItem;
                    var img = (Image)item.FindControl("btnRepair");
                    var img2 = (Image) item.FindControl("btnViewFile");

                    if(img != null)
                    {
                        var repairVisibility = DataIntegrity.ConvertToInt(dataItem["#Error"]) > 0 ? "visible" : "hidden";
                        img.Attributes["style"] = "cursor: pointer; visibility: " + repairVisibility + "; width: 24px; height: 24px;";
                        img.Attributes["onclick"] = "openRejectRepair(" + dataItem["JobID"] + ");";
                    }

                    if(img2 != null)
                    {
                        img2.Attributes["onclick"] = "viewFile('" + dataItem["FileName"] + "')";
                    }
                }
            }

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

                    if (_dtScans.Rows.Count > 0 && DataIntegrity.ConvertToInt(_dtScans.Rows[0]["JobCount"]) > 100)
                    {
                        scansSearchMoreLink.Visible = true;

                        var js = "searchSmallTileAppendMoreLinkOnLoad('" + grdScans.ClientID + "', 'scansSearchMoreLinkSpan');";
                        ScriptManager.RegisterStartupScript(Page, typeof(TileControlBase), "AssessmentScansMoreLinkAppend", js, true);
                    }
                }
            }
		}
}