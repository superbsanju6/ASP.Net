using System;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.Rubrics
{
    public partial class RubricIdentification : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var rubric = (Thinkgate.Base.Classes.Rubric)Tile.TileParms.GetParm("rubric");
            if (rubric == null) return;

            lblGrade.Text = rubric.Grade;
            lblSubject.Text = rubric.Subject;
            lblName.Text = rubric.Name;
            lblType.Text = rubric.TypeDesc;
            lblScoring.Text = rubric.Scoring;
            lblKeywords.Text = rubric.Keywords;
            lblCopyright.Text = rubric.Copyright;
            lblSource.Text = rubric.Source;
            lblCredit.Text = rubric.Credit;
            lblItemBanks.Text = ItemBankMasterList.Filtered_ItemBank_Labels(rubric.ItemBankList, SessionObject.LoggedInUser);
        }
    }
}