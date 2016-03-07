using System;
using System.Data;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentAddendums : TileControlBase
    {        
        protected new void Page_Init(object sender, EventArgs e)
        {
            DataTable dt = Thinkgate.Base.Classes.Assessment.LoadUserAddendumCountsByBank(SessionObject.ApplicationUser);
            var thirdPartyData = new DataTable();
            thirdPartyData.Columns.Add("ItemBank");
            thirdPartyData.Columns.Add("ItemCount");

            var localData = new DataTable();
            localData.Columns.Add("ItemBank");
            localData.Columns.Add("ItemCount");

            foreach (DataRow r in dt.Rows)
            {
                if (r["Category"].ToString() == "3rd Party")
                {
                    thirdPartyData.Rows.Add(r["ItemBank"], r["ItemCount"]);
                }
                else
                {
                    localData.Rows.Add(r["ItemBank"], r["ItemCount"]);
                }
            }

            if (localData.Rows.Count > 0)
            {
                barchart1.Visible = true;
                barchart1.Data = localData;
                //barchart1.Height = 200;
                barchart1.Height = thirdPartyData.Rows.Count > 0 ? 105 : 211;
                barchart1.VerticalHeader = "ItemBank";
                barchart1.HorizontalHeader = "ItemCount";
                barchart1.ShowLegend = false; 
            }


            if (thirdPartyData.Rows.Count > 0)
            {
                barchart1.Visible = true;
                barchart1.Data = localData;
                //barchart1.Height = 200;
                barchart1.Height = localData.Rows.Count > 0 ? 105 : 211;
                barchart1.VerticalHeader = "ItemBank";
                barchart1.HorizontalHeader = "ItemCount";
                barchart1.ShowLegend = false;
            }
        }
    }
}