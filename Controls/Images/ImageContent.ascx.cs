using System;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.Images
{
    public partial class ImageContent : TileControlBase
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string ImageWebPath = AppSettings.UploadFolderWebPath + @"/";

            var oImage = (Thinkgate.Base.Classes.ItemImage)Tile.TileParms.GetParm("Image");

            if (oImage == null) return;

            ImageContentTile_HdnImageName.Value = oImage.Name;
            ImageContentTile_HdnImageContent.Value = ResolveUrl(ImageWebPath) + oImage.FileName;
            ImageContentTile_hdnPrintPreviewUrl.Value = ResolveUrl("../../Record/RenderImageAsPDF.aspx?xID=") + oImage.ID_Encrypted;
            hlImagePreview.Attributes.Add("onclick", "hlImagePreviewClick()");



            /* set up our image element to point
             * to the item's thumbnail image */
            var imgFilePath = AppSettings.ItemThumbnailWebPath_Content + oImage.FileName;

            if (!System.IO.File.Exists(Server.MapPath(imgFilePath))) imgFilePath = "~/Images/image.png";

            imgThumbNail.Src = imgFilePath;

            /* determine size of image.  If it is smaller 
             * than the space we allow, then increase its 
             * height or width.  If it is larger than the 
             * space we allow, then use max-width and max 
             * height to shrink it. */
            System.Drawing.Image ThumbNailFilePath = System.Drawing.Image.FromFile(Server.MapPath(imgFilePath));
            var fImgHeight = ThumbNailFilePath.PhysicalDimension.Height;
            var fImgWidth = ThumbNailFilePath.PhysicalDimension.Width;
            var lImgTagMaxHeight = DataIntegrity.ConvertToNullableFloat(imgThumbNail.Style["max-height"].ToString().Replace("px", "").Replace("%", ""));
            var lImgTagMaxWidth = DataIntegrity.ConvertToNullableFloat(imgThumbNail.Style["max-width"].ToString().Replace("px", "").Replace("%", ""));

            if (fImgHeight < lImgTagMaxHeight && fImgWidth < lImgTagMaxWidth)
            {
                if (fImgHeight > fImgWidth)
                    imgThumbNail.Height = DataIntegrity.ConvertToInt(lImgTagMaxHeight);
                else
                    imgThumbNail.Width = DataIntegrity.ConvertToInt(lImgTagMaxWidth);
            }
        }
    }
}