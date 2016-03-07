using System;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Statistics;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentStatistics : TileControlBase
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (Tile.TileParms.GetParm("assessment") == null) return;
            Base.Classes.Assessment assessment = (Base.Classes.Assessment)Tile.TileParms.GetParm("assessment");

            int assessmentId = assessment.AssessmentID;
            DataTable dtOrig = ThinkgateStatistics.GetAssessmentStatisticsById(assessmentId);
            lbxOriginalStatistics.DataSource = dtOrig;
            lbxOriginalStatistics.DataBind();
        }

        protected void lbxList_ItemDataBound(Object sender, RadListBoxItemEventArgs e)
        {
            RadListBoxItem listBoxItem = e.Item;
            DataRowView row = (DataRowView)(listBoxItem).DataItem;

            Label lblDesc = (Label) listBoxItem.FindControl("lblDesc");
            lblDesc.Text = row["StatisticName"].ToString();

            Label lblValue = (Label) listBoxItem.FindControl("lblValue");
            lblValue.Text = row["Value"].ToString();

            if (lblValue.Text == string.Empty)
            {
                listBoxItem.ForeColor = System.Drawing.Color.Gray;
                lblDesc.ForeColor = System.Drawing.Color.Gray;
                lblValue.ForeColor = System.Drawing.Color.Gray;
            }
            else
            {
                listBoxItem.ForeColor = System.Drawing.Color.Black;
                lblDesc.ForeColor = System.Drawing.Color.Black;
                lblValue.ForeColor = System.Drawing.Color.Black;
            }
        }
    }


}