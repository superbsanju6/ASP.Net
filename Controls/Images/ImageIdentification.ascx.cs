using System;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.Images
{
    public partial class ImageIdentification : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var image = (Thinkgate.Base.Classes.ItemImage)Tile.TileParms.GetParm("Image");
            if (image == null) return;

            lblGrade.Text = image.Grade;
            lblSubject.Text = image.Subject;
            lblDescription.Text = image.Description;
            lblName.Text = image.Name;
            lblFileName.Text = image.FileName;
            lblKeywords.Text = image.Keywords;
            lblCopyright.Text = image.Copyright;
            lblSource.Text = image.Source;
            lblCredit.Text = image.Credit;
            lblItemBanks.Text = ItemBankMasterList.Filtered_ItemBank_Labels(image.ItemBankList, SessionObject.LoggedInUser);
       }
    }
}