using System;
using Standpoint.Core.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;

namespace Thinkgate.Dialogues
{
    public partial class RubricConcordantTables : BasePage
    {
        private int _concordantGroupID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _concordantGroupID = DataIntegrity.ConvertToInt(Request.QueryString["xID"]);
            }

            var ds = Thinkgate.Base.Classes.Rubric.GetConcordantTables(_concordantGroupID);

            lblNoTablesFound.Visible = true;

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return; //ERROR

            var pageTitle = ds.Tables[0].Rows[0]["concordantName"].ToString();
            var firstTableCount = DataIntegrity.ConvertToInt(ds.Tables[0].Rows[0]["tableCount"]);
            var overrideKey = ds.Tables[0].Rows[0]["overrideKey"].ToString();

            Page.Title = pageTitle;

            if (!String.IsNullOrEmpty(overrideKey))
            {
                ShowAlternateData(overrideKey, pageTitle, ds.Tables[0]);
            }
            else
            {
                lblNoTablesFound.Visible = (firstTableCount == 0);

                if (firstTableCount > 0)
                {
                    tablesRepeater.DataSource = ds.Tables;
                    tablesRepeater.DataBind();
                }
            }
        }

        private void ShowAlternateData(string panelName, string pageTitle, DataTable dt)
        {
            var panel = FindControl(panelName);
            var table = new HtmlTable();
            if (panel == null) return;
            ((Panel)panel).Visible = true;
            lblNoTablesFound.Visible = false;
            tablesRepeater.Visible = false;
            dt.Columns.Remove("concordantName");
            dt.Columns.Remove("tableCount");
            dt.Columns.Remove("overrideKey");

            table.Attributes["class"] = "concordantRangesTable";
            table.Attributes["style"] = "width:100%;";
            /*
            var captionRow = new HtmlTableRow();
            var captionCell = new HtmlTableCell();
            captionCell.ColSpan = dt.Columns.Count;
            captionCell.InnerHtml = pageTitle;
            captionCell.Attributes["style"] = "font-weight:bold;";
            captionRow.Cells.Add(captionCell);
            table.Rows.Add(captionRow);
            */
            var headerRow = new HtmlTableRow();
            foreach(DataColumn column in dt.Columns)
            {
                var headerCell = new HtmlTableCell();
                headerCell.InnerHtml = column.ColumnName;
                headerCell.Attributes["style"] = "font-weight:bold;";
                headerRow.Cells.Add(headerCell);
            }
            table.Rows.Add(headerRow);

            foreach(DataRow row in dt.Rows)
            {
                var newRow = new HtmlTableRow();
                var bgColor = table.Rows.Count % 2 == 0 ? "D0D0D0" : "";
                foreach(DataColumn column in dt.Columns)
                {
                    var newCell = new HtmlTableCell();
                    newCell.InnerHtml = row[column].ToString();
                    newCell.BgColor = bgColor;
                    newRow.Cells.Add(newCell);
                }
                table.Rows.Add(newRow);
            }

            panel.Controls.Add(table);
        }

        protected void tablesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var table = (DataTable)e.Item.DataItem;

                ((Label)e.Item.FindControl("lblNoDataFound")).Visible = (table == null || table.Rows.Count <= 1);

                ((Repeater)e.Item.FindControl("concordantRangesTable")).DataSource = table;
                ((Repeater)e.Item.FindControl("concordantRangesTable")).DataBind();

            }
        }

        protected void concordantRangesTable_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var dt = ((DataTable)((Repeater)e.Item.Parent).DataSource);
                if (dt == null || dt.Rows.Count == 0) return;
                ((Label)e.Item.FindControl("lblTableHeader")).Text = dt.Rows[0]["tableHeader"].ToString();
            }
        }

        protected void districtConcordantRangesTable_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var dt = ((DataTable)((Repeater)e.Item.Parent).DataSource);
                if (dt == null || dt.Rows.Count == 0) return;
                ((Label)e.Item.FindControl("lblTableHeader")).Text = dt.Rows[0]["tableHeader"].ToString();
            }
        }
    }
}