using System;
using System.Text;
using Thinkgate.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.Addendums
{
    public partial class AddendumIdentification : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var addendum = (Thinkgate.Base.Classes.Addendum)Tile.TileParms.GetParm("addendum");
            if (addendum == null) return;

            lblGrade.Text = addendum.Addendum_Grade;
            lblSubject.Text = addendum.Addendum_Subject;
            lblName.Text = addendum.Addendum_Name;
            lblType.Text = addendum.Addendum_Type;
            lblGenre.Text = addendum.Addendum_Genre;
            lblKeywords.Text = addendum.Addendum_Keywords;
            lblCopyright.Text = addendum.Addendum_Copyright;
            lblSource.Text = addendum.Addendum_Source;
            lblCredit.Text = addendum.Addendum_Credit;
            lblItemBanks.Text = ItemBankMasterList.Filtered_ItemBank_Labels(addendum.Addendum_ItemBankList,SessionObject.LoggedInUser);
        }
    }
}