using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using System.Text;
using Standpoint.Core.Utilities;
using Standpoint.Core.Classes;
using Telerik.Web.UI;

namespace Thinkgate.Controls.Items
{
    public partial class ItemContent : TileControlBase
	{
        private Type _questionType;
        private QuestionBase _oQuestionBase;
        private BankQuestion _oBankQuestion;
        private TestQuestion _oTestQuestion;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
           
            if (Tile.TileParms.GetParm("item") == null) return;

            _questionType = Tile.TileParms.GetParm("item").GetType();
            if (_questionType.Name == "BankQuestion")
            {
                _oBankQuestion = (BankQuestion)Tile.TileParms.GetParm("item");
                _oQuestionBase = (QuestionBase)_oBankQuestion;
            }
            else
            {
                _oTestQuestion = (TestQuestion)Tile.TileParms.GetParm("item");
                _oQuestionBase = (QuestionBase)_oTestQuestion;
            }

            var dparms = DistrictParms.LoadDistrictParms();
            ItemContentTile_lbl_OTCUrl.Value = AppSettings.OTCUrl + dparms.ClientID;
            ItemContentTile_itemID.Value = _oQuestionBase.ID.ToString();
            ItemContentTile_itemType.Value = _questionType.Name.ToString();

            /* set up our image element to point
             * to the item's thumbnail image */
            var imgFilePath = AppSettings.ItemThumbnailWebPath_Content + _oQuestionBase.ThumbnailName;
            
            if (!System.IO.File.Exists(Server.MapPath(imgFilePath))) imgFilePath = "~/Images/thumb_none.png";

            ItemContentTile_imgThumbnail.Src = imgFilePath;

            /* determine size of image.  If it is smaller 
             * than the space we allow, then increase its 
             * height or width.  If it is larger than the 
             * space we allow, then use max-width and max 
             * height to shrink it. */
            System.Drawing.Image imgThumbNail = System.Drawing.Image.FromFile(Server.MapPath(imgFilePath));
            var fImgHeight = imgThumbNail.PhysicalDimension.Height;
            var fImgWidth = imgThumbNail.PhysicalDimension.Width;
            var lImgTagMaxHeight = DataIntegrity.ConvertToNullableFloat(ItemContentTile_imgThumbnail.Style["max-height"].ToString().Replace("px","").Replace("%",""));
            var lImgTagMaxWidth = DataIntegrity.ConvertToNullableFloat(ItemContentTile_imgThumbnail.Style["max-width"].ToString().Replace("px", "").Replace("%", ""));

            if (fImgHeight < lImgTagMaxHeight && fImgWidth < lImgTagMaxWidth)
            {
                if (fImgHeight > fImgWidth)
                    ItemContentTile_imgThumbnail.Height = DataIntegrity.ConvertToInt(lImgTagMaxHeight);
                else
                    ItemContentTile_imgThumbnail.Width = DataIntegrity.ConvertToInt(lImgTagMaxWidth);
            }



            /* Set up our online preview link's onClick
             * event with the item's ID. */
            ItemContentTile_hdnThumbnailURL.Value = @"display.asp?formatoption=search results&key=9120&retrievemode=searchpage&ahaveSkipped=normal&??Question=" + _oQuestionBase.ID.ToString() + @"&??TestID=0&qreview=y";
            imgPreviewIcon.Src = ResolveUrl("../../Images/preview_small.png");
            
            /* Set up our Print Preview hidden text box
             * to hold the encrypted ID of the item */
            ItemContentTile_hdnPrintPreviewInfo.Value = ResolveUrl("../../Record/RenderItemAsPDF.aspx?xID=") + 
                                                        Encryption.EncryptString(_oQuestionBase.ID.ToString()) + 
                                                        "&TestQuestion=" + Encryption.EncryptString((_questionType.Name=="BankQuestion") ? "false" : "true");
            imgPrintPreviewIcon.Src = ResolveUrl("../../Images/mag_glass.png");

            /* See if the item has an addendum. If so then
             * set up and display link to pop up Addendum.
             * Otherwise make sure it doesn't display.*/
            if (_oQuestionBase.AddendumID == 0)
            {
                divDisplayAddendum.Style["visibility"] = "hidden"; 
            }
            else
            {
                if (_questionType.Name == "BankQuestion") _oBankQuestion.LoadAddendum();
                //_oQuestionBase.LoadAddendum();
                hlLaunchAddendumPage.InnerText = _oQuestionBase.Addendum.Addendum_Name;
                hlLaunchAddendumPage.Attributes["onClick"] = "window.open('" + ResolveUrl("../../Record/AddendumPage.aspx?xID=") + Encryption.EncryptString(_oQuestionBase.AddendumID.ToString()) + "'); return false;";

                divAddendumContent.InnerHtml = _oQuestionBase.Addendum.Addendum_Text;

                hlDisplayAddendum.Attributes["onClick"] = "displayAddendumClick();";

                HtmlImage imgAddendumIcon = new HtmlImage();
                imgAddendumIcon.ID = "imgAddendumIcon";
                imgAddendumIcon.Src = ResolveUrl("../../Images/Addendum_small.png");
                imgAddendumIcon.Height = 20;
                hlDisplayAddendum.Controls.Add(imgAddendumIcon);

                Label lblAddendum_Name = new Label();
                lblAddendum_Name.Text = _oQuestionBase.Addendum.Addendum_Name;
                hlDisplayAddendum.Controls.Add(lblAddendum_Name);
                //hlDisplayAddendum.InnerText = oItem.Addendum.Addendum_Name;
                divDisplayAddendum.Style["visibility"] = "visible";
            }


        }
    }
}