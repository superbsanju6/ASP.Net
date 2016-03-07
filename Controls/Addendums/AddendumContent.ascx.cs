using System;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Addendums
{
    public partial class AddendumContent : TileControlBase
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var oAddendum = (Thinkgate.Base.Classes.Addendum)Tile.TileParms.GetParm("addendum");
            if (oAddendum == null) return;
            AddendumContentTile_HdnAddendumName.Value = oAddendum.Addendum_Name;
            AddendumContentTile_HdnAddendumContent.Value = oAddendum.Addendum_Text;
            AddendumContentTile_hdnPrintPreviewUrl.Value = ResolveUrl("../../Record/RenderAddendumAsPDF.aspx?xID=") + oAddendum.ID_Encrypted;
            hlAddendumContent.Attributes.Add("onclick", "hlAddendumContentClick()");
            //hlAddendumContent.Attributes.Add("onclick",
            //                                     "customDialog({ title: 'Addendum Preview', width: 400, height: 100, content: '" +
            //                                     "<table><tr><td><div class=\"contentTile-PreviewLabel\">" + oAddendum.Addendum_Name +
            //                                     "</div></td></tr><tr><td><div class=\"contentTile-PreviewContent\">" + oAddendum.Addendum_Text + 
            //                                     "</div></td></tr></table>', maximize_width: false }, [{ title: 'Ok' }]);");
        }
    }
}