using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Thinkgate.Classes;
using System.Text;

namespace Thinkgate.Controls.Standards
{
    public partial class StandardsContent : TileControlBase
    {
        private StringBuilder sb;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadData();
        }

        private void LoadData()
        {
            /* Get our standard from TileParms collection. */
            var oStd = (Thinkgate.Base.Classes.Standards)Tile.TileParms.GetParm("standards");

            /* populate controls from our standard. */
            StdsPage_StdsContent_DivStdTitle.InnerText = BuildStdTitle(oStd);
            StdsPage_StdsContent_DivStdText.InnerHtml = oStd.Desc;

            /* populate parent information if our standard has a parent */

            if (oStd.Parent != null)
            {
                StdsPage_StdsContent_DivStdParentTitle.InnerText = BuildStdTitle(oStd.Parent);
                StdsPage_StdsContent_DivStdParentText.InnerHtml = oStd.Parent.Desc;
            }
            else
            {
                StdsPage_StdsContent_DivStdParentTitle.InnerText = "";
                StdsPage_StdsContent_DivStdParentText.InnerText = "";
            }

            /* populate children informationif our standard has children */
            if (oStd.Children.Count >0)
            {
                sb = new StringBuilder("");

                HtmlTable oTbl = new HtmlTable();
                HtmlTableRow oRow;
                HtmlTableCell oCell;
                System.Web.UI.HtmlControls.HtmlGenericControl oDiv;

                foreach (Thinkgate.Base.Classes.Standards oChildStd in oStd.Children)
                {
                    oRow = new HtmlTableRow();
                    oCell = new HtmlTableCell();

                    oDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    oDiv.Attributes.Add("class", "stdTitle");
                    oDiv.Style.Add("display","block");
                    oDiv.InnerText = BuildStdTitle(oChildStd);

                    oCell.Controls.Add(oDiv);

                    oDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    oDiv.Attributes.Add("class", "stdText");
                    oDiv.Style.Add("display", "block");
                    oDiv.InnerHtml = oChildStd.Desc;

                    oCell.Controls.Add(oDiv);

                    oRow.Cells.Add(oCell);
                    oTbl.Rows.Add(oRow);
                }

                StdsPage_StdsContent_divStdChildren.Controls.Add(oTbl);
            }
        }

        private string BuildStdTitle(Thinkgate.Base.Classes.Standards oStd)
        {
            sb = new StringBuilder("");

            if (!string.IsNullOrEmpty(oStd.Level)) sb.Append(oStd.Level + " ");
            if (!string.IsNullOrEmpty(oStd.StateNbr)) sb.Append("(" + oStd.StateNbr + ") ");
            //if (!string.IsNullOrEmpty(oStd.StandardName)) sb.Append(oStd.StandardName + " ");

            return sb.ToString();
        }
    }
}