using System;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Rubrics
{
    public partial class RubricContent : TileControlBase
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var oRubric = (Thinkgate.Base.Classes.Rubric)Tile.TileParms.GetParm("rubric");
            if (oRubric == null) return;
            RubricContentTile_HdnRubricName.Value = oRubric.Name;
            RubricContentTile_HdnRubricContent.Value = oRubric.Content;
            RubricContentTile_hdnPrintPreviewUrl.Value = ResolveUrl("../../Record/RenderRubricAsPDF.aspx?xID=") + oRubric.ID_Encrypted;
            hlRubricContent.Attributes.Add("onclick", "hlRubricContentClick()");
            //anchorRubricContent.Attributes.Add("onclick",
            //                         "customDialog({ title: 'Rubric Preview', width: 600, height: 100, content: \"" +
            //                         "<table><tr><td><div class='contentTile-PreviewLabel'>" + oRubric.Name +
            //                         "</div></td></tr><tr><td>" + oRubric.Content +
            //                         "</td></tr></table>\", model: false, maximize_width: false }, [{ title: 'Ok' }]);");

        }
    }
}