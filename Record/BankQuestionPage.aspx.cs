using System;
using System.Collections.Generic;
using System.Reflection;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Utilities;
using System.Web.UI;

namespace Thinkgate.Record
{
    public partial class BankQuestionPage : RecordPage
    {
        #region Variables

        public int ItemId;
        public string ItemIdEncrypted;
        public BankQuestion SelectedItem;
        private string _itemTitle;
        private RadMenuItem _miCopy, _miDelete;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.BankQuestion + "_"; }
        }

        #endregion

        #region Page Events

        protected new void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack) return;
                /********************************************************************
                 * Set up banner for Item Page to display the following actions
                 * as menu options:
                 *          Copy    - Copies current item into User's personal Bank
                 *          Delete  - Deletes the current Item from the system.
                 * *****************************************************************/
            var siteMaster = Master as SiteMaster;
                if (siteMaster != null)
                {
                    siteMaster.BannerType = BannerType.ObjectScreen;
                    //TODO: Some day a permission will be invented to determine if user can copy/delete items.
                    //      Obviously, Icon_Edit_Standard isn't it...
                    //if (UserHasPermission(Permission.Icon_Edit_Standard))
                    //{
                _miCopy = new RadMenuItem("Copy");
                _miDelete = new RadMenuItem("Delete");
                siteMaster.Banner.AddMenuItem(Banner.ContextMenu.Actions, _miCopy);
                siteMaster.Banner.AddMenuItem(Banner.ContextMenu.Actions, _miDelete);
                    //}
                    siteMaster.Banner.AddOnClientItemClicked(Banner.ContextMenu.Actions, "actionsMenuItemClicked");
                }

                SessionObject = (SessionObject)Session["SessionObject"];
                SetupFolders();
                InitPage(ctlFolders, ctlDoublePanel, sender, e);
                LoadItem();
            if (SelectedItem == null) return;
                if (!IsPostBack)
                {
                    SessionObject.ClickedInformationControl = "Identification";
                }
                LoadDefaultFolderTiles();
                SetHeaderImageUrl();
            }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ItemName.Text = _itemTitle;

                // Hide the single folder for now.
                FoldersControl.Visible = false;

                //load up hidden input box with encrypted item ID and item type.
                ItemPage_hdnEncryptedData.Value = Request.QueryString["xID"];

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
                if (Page.Master == null) return;
                var pageScriptMgr = Page.Master.FindControl("RadScriptManager1") as RadScriptManager;
                var newSvcRef = new ServiceReference("~/Services/Service2.Svc");
                if (pageScriptMgr != null) pageScriptMgr.Services.Add(newSvcRef);
            }
        }

        protected void Page_LoadComplete()
        {
            var siteMaster = Master as SiteMaster;
            if (siteMaster == null) return;
            if ((_miDelete != null && !_miDelete.Visible) && (_miCopy != null &&!_miCopy.Visible))
                    siteMaster.BannerControl.HideContextMenu(Banner.ContextMenu.Actions);
            }

        #endregion

        private void SetHeaderImageUrl()
        {
            if (AppSettings.IsIllinois)
            {
                ItemImage.ImageUrl = "~/Images/ISLE_logo1.png";
            }
            else
            {
                ItemImage.ImageUrl = "~/Images/Star_Icon.png";
            }
        }

        private void LoadItem()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                ItemId = GetDecryptedEntityId(X_ID);
                ItemIdEncrypted = EntityIdEncrypted;

                if (!RecordExistsInCache(Key))
                {
                    SelectedItem = BankQuestion.GetQuestionByID(ItemId);
                    if (SelectedItem != null)
                        Base.Classes.Cache.Insert(Key, SelectedItem);
                    else
                    {
                        SessionObject.RedirectMessage = "Could not find the item.";
                        Response.Redirect("~/PortalSelection.aspx", true);
                    }
                }
                else SelectedItem = (BankQuestion)Cache[Key];

                // Set the page title text.
                if (SelectedItem != null) _itemTitle = ("Item ID: " + SelectedItem.ID);
            }
        }

        #region Folder Methods
            
        private void SetupFolders()
        {
            Folders = new List<Folder>
            {
                new Folder("Identification", "~/Images/new/Information.png", LoadInformationTiles,
                    "~/ContainerControls/TileContainerDockSix.ascx", 6)
            };
            //Folders.Add(new Folder("Identification", "~/Images/new/Information.png", LoadInformationTiles, "~/ContainerControls/TileContainer_3_1.ascx", 3));

            ctlFolders.BindFolderList(Folders);
        }

        #endregion

        #region Tile Methods

        private void LoadInformationTiles()
        {
            if (SelectedItem == null) return;

            var itemTileParms = new TileParms();
            itemTileParms.AddParm("item", SelectedItem);
            itemTileParms.AddParm("level", EntityTypes.BankQuestion);
            itemTileParms.AddParm("levelID", SelectedItem.ID);

            itemTileParms.AddParm("folder",""); //This parameter isn't really needed other than the StandardsSearch 
                                                //tile expects there to be something other than null in it.
            string editUrl = "";
            string editJavascriptFunction = "";

            bool restrictedByCopyRight = AssessmentUtil.CheckIfCopyRightRestricted(SelectedItem.Copyright, SelectedItem.CopyRightExpiryDate);

            if (ItemBankMasterList.DetermineIfItemBankListHasAccess(SelectedItem.ItemBankList, SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit,"Search"))
            {
                editUrl = "../Controls/Assessment/ContentEditor/ContentEditor_Item.aspx?xID=" + ItemIdEncrypted;
                editJavascriptFunction = "ItemPage_openExpandEditRadWindow";
            } else
            {
                _miDelete.Visible = false;
            }
            if ((!ItemBankMasterList.DetermineIfItemBankListHasAccess(SelectedItem.ItemBankList, SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankCopy, "Search")) || restrictedByCopyRight)
            {
                _miCopy.Visible = false;
            }
            


            Rotator1Tiles.Add(new Tile(Permission.Tile_Identification_ItemObjectScreen,
                                        "Item Identification", 
                                        "~/Controls/Items/ItemIdentification.ascx", 
                                        false, 
                                        itemTileParms, 
                                        null, 
                                        null, 
                                        editUrl, 
                                        false, 
                                        null, 
                                        null, 
                                        //TODO:DHB - Uncomment as soon as permission is setup.
                                        //(UserHasPermission(Permission.Icon_Edit_ItemAdvanced) ? "ItemPage_openExpandEditRadWindow" : null)
                                        editJavascriptFunction)
                                        );
            
            Rotator1Tiles.Add(new Tile(Permission.Tile_Content_ItemObjectScreen, 
                                        "Item Content", 
                                        "~/Controls/Items/ItemContent.ascx", 
                                        false, 
                                        itemTileParms, 
                                        null, 
                                        null, 
                                        editUrl, 
                                        false, 
                                        null, 
                                        null,
                                        editJavascriptFunction));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Standards_ItemObjectScreen,
                                        "Item Standards", 
                                        "~/Controls/Standards/StandardsSearch.ascx", 
                                        false, 
                                        itemTileParms, 
                                        null, 
                                        null,
                                        editUrl, 
                                        false, 
                                        null, 
                                        null,
                                        editJavascriptFunction)
                                        );
            
            Rotator1Tiles.Add(new Tile(Permission.Tile_Advanced_ItemObjectScreen, 
                                        "Item Advanced", 
                                        "~/Controls/Items/ItemAdvanced.ascx", 
                                        false, 
                                        itemTileParms, 
                                        null, 
                                        null, 
                                        editUrl, 
                                        false, 
                                        null, 
                                        null,
                                        editJavascriptFunction));

            Rotator1Tiles.Add(new Tile(Permission.Tile_ItemStatistics,
                                        "Item Statistics", 
                                        "~/Controls/Items/ItemStatistics.ascx",
                                        false,
                                        itemTileParms,
                                        null,
                                        null,
                                        null,
                                        false,
                                        null,
                                        null,
                                        null,
                                        null
                                        ));
        }

        protected void ScheduleClassTileClick(TileParms tileParms)
        {
            SessionObject.clickedClass = (Base.Classes.Class)tileParms.GetParm("class");
            ReloadTilesControl("Classes");
        }

        #endregion

        protected override object LoadRecord(int xId)
        {
            return BankQuestion.GetQuestionByID(xId);
        }
    }
}
