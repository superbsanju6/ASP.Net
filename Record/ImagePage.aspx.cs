using System;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Base.Enums;
using System.Collections.Generic;

namespace Thinkgate.Record
{
    public partial class ImagePage : RecordPage
    {
        #region Variables

        private int _imageId;
        private string _imageIdEncrypted;
        private ItemImage _selectedImage;
        private string _imageTitle;
        private RadMenuItem _miDelete;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.Image + "_"; }
        }

        #endregion

        #region Page Events

        protected new void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                /********************************************************************
                 * Set up banner for Item Page to display the following actions
                 * as menu options:
                 *          Copy    - Copies current item into User's personal Bank
                 *          Delete  - Deletes the current Item from the system.
                 * *****************************************************************/
                var siteMaster = this.Master as SiteMaster;
                if (siteMaster != null)
                {
                    siteMaster.BannerType = BannerType.ObjectScreen;
                    _miDelete = new RadMenuItem("Delete");
                    siteMaster.Banner.AddMenuItem(Banner.ContextMenu.Actions, _miDelete);
                    siteMaster.Banner.AddOnClientItemClicked(Banner.ContextMenu.Actions, "actionsMenuItemClicked");
                }
                SessionObject = (SessionObject)Session["SessionObject"];
                SetupFolders();
                InitPage(ctlFolders, ctlDoublePanel, sender, e);
                LoadItemImage();
                if (_selectedImage == null) return;
                if (!IsPostBack)
                {
                    SessionObject.ClickedInformationControl = "Identification";
                }
                LoadDefaultFolderTiles();
                SetHeaderImageUrl();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblImageName.Text = _imageTitle;

                // Hide the single folder for now.
                FoldersControl.Visible = false;

                //load up hidden input box with encrypted item ID and item type.
                ImagePage_hdnEncryptedData.Value = Request.QueryString["xID"];

                /****************************************************************
                 * Find the RadScriptManager control and insert service into it.
                 * 
                 * The RadScriptManager is in the Site.Master masterpage. 
                 * Because that master page is used by so many pages, we don't 
                 * want to add services to Site.Master's script manager.  The
                 * pages that don't need the service don't need the extra overhead
                 * that the service causes.  So we must find the scriptManager and
                 * add the service to it from here.
                 * **************************************************************/
                var pageScriptMgr = Page.Master.FindControl("RadScriptManager1") as RadScriptManager;
                var newSvcRef = new ServiceReference("~/Services/Service2.Svc");
                pageScriptMgr.Services.Add(newSvcRef);
            }
        }

        #endregion

        private void SetHeaderImageUrl()
        {
            if (AppSettings.IsIllinois)
            {
                imgItemImage.ImageUrl = "~/Images/ISLE_logo1.png";
            }
            else
            {
                imgItemImage.ImageUrl = "~/Images/new/Image.png";
            }
        }

        private void LoadItemImage()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _imageId = GetDecryptedEntityId(X_ID);
                _imageIdEncrypted = EntityIdEncrypted;
                var key = "Image_" + _imageId;

                if (!RecordExistsInCache(key))
                {
                    _selectedImage = ItemImage.GetImageByID(_imageId);
                    if (_selectedImage != null)
                        Base.Classes.Cache.Insert(key, _selectedImage);
                    else
                    {
                        SessionObject.RedirectMessage = "Could not find the image.";
                        Response.Redirect("~/PortalSelection.aspx", true);
                    }
                }
                else _selectedImage = (ItemImage)Cache[key];

                // Set the page title text.
                _imageTitle = _selectedImage.Name ?? string.Empty;

            }
        }

        #region Folder Methods

        private void SetupFolders()
        {
            Folders = new List<Folder>();
            Folders.Add(new Folder("Identification", "~/Images/new/Information.png", LoadInformationTiles, "~/ContainerControls/TileContainer_3_1.ascx", 3));

            ctlFolders.BindFolderList(Folders);
        }

        #endregion

        #region Tile Methods

        private void LoadInformationTiles()
        {
            if (_selectedImage == null) return;

            var imageTileParms = new TileParms();
            imageTileParms.AddParm("Image", _selectedImage);

            var editURL = "../Controls/Assessment/ContentEditor/ContentEditor_Image.aspx?xID=" + _imageIdEncrypted;
            Rotator1Tiles.Add(new Tile(Permission.Tile_Identification_ImageObjectScreen, 
                                        "Image Identification", 
                                        "~/Controls/Images/ImageIdentification.ascx", 
                                        false, 
                                        imageTileParms, 
                                        null, 
                                        null, 
                                        editURL, 
                                        false, 
                                        null,
                                        null, 
                                        "ImagePage_openExpandEditRadWindow"));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Content_ImageObjectScreen, 
                                        "Image Content", 
                                        "~/Controls/Images/ImageContent.ascx", 
                                        false, 
                                        imageTileParms,
                                        null, 
                                        null, 
                                        editURL, 
                                        false, 
                                        null,
                                        null, 
                                        "ImagePage_openExpandEditRadWindow"));
        }

        protected void ScheduleClassTileClick(TileParms tileParms)
        {
            SessionObject.clickedClass = (Thinkgate.Base.Classes.Class)tileParms.GetParm("class");
            ReloadTilesControl("Classes");
        }

        #endregion

        protected override object LoadRecord(int xId)
        {
            return ItemImage.GetImageByID(xId);
        }
    }
}
