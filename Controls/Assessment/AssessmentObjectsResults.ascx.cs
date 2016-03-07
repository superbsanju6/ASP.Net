using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentObjectsResults : TileControlBase
    {
        protected Int32 _assessmentID;
        // True if this is a postback.
        protected Boolean _isPostBack;
        // _levelData is a collection of tables whose first table is the specification of levels to show
        // and then up to 4 tables, one for each level.
        protected DataSet _levelData;
        protected RadGrid[] _grids;

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

            _grids = new RadGrid[] { rg0, rg1, rg2, rg3 };

            // Initial load.
            if (!_isPostBack)
            {
                BuildUi();
            }
        }

        protected DataSet LevelData
        {
            get
            {
                if (_levelData == null)
                    LoadLevelData();
                return _levelData;
            }
            set { _levelData = value; }
        }

        protected void LoadLevelData()
        {
            _levelData = new DataSet();
            DataTable dtLevels = Thinkgate.Base.Classes.Assessment.GetAssessmentCategoryLevels(_assessmentID);
            dtLevels.TableName = "Levels";
            _levelData.Tables.Add(dtLevels.Copy());

            // Now load a table for each level.
            foreach (DataRow row in dtLevels.Rows)
            {
                String level = row["Level"].ToString();
                DataTable dt = Thinkgate.Base.Classes.Assessment.GetAssessmentObjectsResults(_assessmentID, level);
                dt.TableName = level;
                // Convert %Proficient column to actual percentage by multiplying by 100.
                DataColumn profCol = dt.Columns["%Proficient"];
                // Convert the average score column to actual percentage by multiplying by 100.
                DataColumn scoreCol = dt.Columns["Score"];
                foreach (DataRow pRow in dt.Rows)
                {
                    if (profCol != null && !(pRow[profCol] is System.DBNull))
                        pRow[profCol] = (Double)pRow[profCol] * 100.0;
                    if (scoreCol != null && !(pRow[scoreCol] is System.DBNull))
                        pRow[scoreCol] = (Double)pRow[scoreCol] * 100.0;
                }
                _levelData.Tables.Add(dt.Copy());
            }
        }

        protected void BuildUi()
        {
            Int32 i;
            DataSet ds = LevelData;
            // Set tab name and visibility.
            for (i = 0; i < rtsAssessmentObjectsResults.Tabs.Count; i++)
            {
                if (i >= ds.Tables["Levels"].Rows.Count)
                    rtsAssessmentObjectsResults.Tabs[i].Visible = false;
                else
                {
                    rtsAssessmentObjectsResults.Tabs[i].Text = ds.Tables["Levels"].Rows[i]["DisplayLevel"].ToString();
                    // Set column names and visibility for this tab/grid.
                    SetColumns(ds.Tables["Levels"].Rows[i]["Level"].ToString(), ds.Tables[i + 1], _grids[i]);
                    // Bind a grid for this tab.
                    BindDataToGrid(ds.Tables[i + 1], _grids[i]);
                }
            }
        }

        protected void SetColumns(String level, DataTable dt, RadGrid rg)
        {
            // Make unused columns invisible.
            if (!dt.Columns.Contains("Score"))
                rg.Columns.FindByDataField("Score").Visible = false;
            if (!dt.Columns.Contains("StudentCount"))
                rg.Columns.FindByDataField("StudentCount").Visible = false;
            if (!dt.Columns.Contains("%Proficient"))
                rg.Columns.FindByDataField("%Proficient").Visible = false;

            rg.Columns[0].HeaderText = level;
            if (dt.Rows.Count == 1 && String.Compare(dt.Rows[0]["LevelName"].ToString(), level, true) == 0)
                rg.Columns[0].HeaderText = "";
        }

        protected void BindDataToGrid(DataTable dt, RadGrid rg)
        {
            rg.DataSource = dt;
            rg.DataBind();
        }

        protected void rg_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem gridItem = (GridDataItem)e.Item;
                HyperLink lnk = (HyperLink)gridItem["LnkLevelName"].Controls[0];
                DataRowView row = (DataRowView)(gridItem).DataItem;
                String level = row["Level"].ToString();
                if (String.Compare(level, "Class", true) == 0)
                    lnk.NavigateUrl = "~/Record/Class.aspx?xID=" + Standpoint.Core.Classes.Encryption.EncryptString(row["LevelID"].ToString());
                else if (String.Compare(level, "Student", true) == 0)
                    lnk.NavigateUrl = "~/Record/Student.aspx?xID=" + Standpoint.Core.Classes.Encryption.EncryptString(row["LevelID"].ToString() + "&childPage=yes");
                else if (String.Compare(level, "School", true) == 0)
                    lnk.NavigateUrl = "~/Record/School.aspx?xID=" + Standpoint.Core.Classes.Encryption.EncryptString(row["LevelID"].ToString() + "&childPage=yes");
            }   
        }

        protected void rg_OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            RadGrid rg = sender as RadGrid;
            if (rg != null)
            {
                Int32 i;
                for (i = 0; i < _grids.Length; i++)
                {
                    if (rg.ID == _grids[i].ID)
                    {
                        BindDataToGrid(LevelData.Tables[i + 1], _grids[i]);
                        break;
                    }
                }
            }
        }
    }
}