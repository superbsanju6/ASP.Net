using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Reflection;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Telerik.Web.UI;

namespace Thinkgate.Record
{
    public partial class TestQuestionPage : RecordPage
    {
        #region Variables

        private int _itemId;
        private string _itemIdEncrypted;
        private Base.Classes.TestQuestion _selectedItem;
        private string _itemTitle;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.TestQuestion + "_"; }
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
                    //miCopy = new RadMenuItem("Copy");
                    //miDelete = new RadMenuItem("Delete");
                    //siteMaster.Banner.AddMenuItem(Banner.ContextMenu.Actions, miCopy);
                    //siteMaster.Banner.AddMenuItem(Banner.ContextMenu.Actions, miDelete);
                    siteMaster.Banner.AddOnClientItemClicked(Banner.ContextMenu.Actions, "actionsMenuItemClicked");
                }

                SessionObject = (SessionObject)Session["SessionObject"];
                SetupFolders();
                InitPage(ctlFolders, ctlDoublePanel, sender, e);
                LoadItem();
                if (_selectedItem == null) return;
                if (!IsPostBack)
                {
                    SessionObject.ClickedInformationControl = "Identification";
                }
                LoadDefaultFolderTiles();

            }
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
                var pageScriptMgr = Page.Master.FindControl("RadScriptManager1") as RadScriptManager;
                var newSvcRef = new ServiceReference("~/Services/Service2.Svc");
                pageScriptMgr.Services.Add(newSvcRef);
            }
        }

        #endregion

        private void LoadItem()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {

                _itemId = GetDecryptedEntityId(X_ID);
                _itemIdEncrypted = EntityIdEncrypted;
                var key = "TestQuestion_" + _itemId;

                if (!RecordExistsInCache(key))
                {
                    _selectedItem = Base.Classes.TestQuestion.GetTestQuestionByID(_itemId);
                    if (_selectedItem != null)
                        Thinkgate.Base.Classes.Cache.Insert(key, _selectedItem);
                    else
                    {
                        SessionObject.RedirectMessage = "Could not find the item.";
                        Response.Redirect("~/PortalSelection.aspx", true);
                    }
                }
                else _selectedItem = (Base.Classes.TestQuestion)Cache[key];

                // Set the page title text.
                _itemTitle = ("Item ID: " + _selectedItem.ID.ToString()) ?? string.Empty;
            }
        }

        #region Folder Methods

        private void SetupFolders()
        {
            Folders = new List<Folder>();
            Folders.Add(new Folder("Identification", "~/Images/new/Information.png", LoadInformationTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));
            //Folders.Add(new Folder("Identification", "~/Images/new/Information.png", LoadInformationTiles, "~/ContainerControls/TileContainer_3_1.ascx", 3));

            ctlFolders.BindFolderList(Folders);
        }

        #endregion

        #region Tile Methods

        private void LoadInformationTiles()
        {
            if (_selectedItem == null) return;

            var ItemTileParms = new TileParms();
            ItemTileParms.AddParm("item", _selectedItem);
            ItemTileParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.TestQuestion);
            ItemTileParms.AddParm("levelID", _selectedItem.ID);

            ItemTileParms.AddParm("folder", ""); //This parameter isn't really needed other than the StandardsSearch 
            //tile expects there to be something other than null in it.
            Rotator1Tiles.Add(new Tile(Permission.Tile_Identification_ItemObjectScreen, "Item Identification", "~/Controls/Items/ItemIdentification.ascx", false, ItemTileParms));
            Rotator1Tiles.Add(new Tile(Permission.Tile_Content_ItemObjectScreen, "Item Content", "~/Controls/Items/ItemContent.ascx", false, ItemTileParms));
            Rotator1Tiles.Add(new Tile(Permission.Tile_Standards_ItemObjectScreen, "Item Standards", "~/Controls/Standards/StandardsSearch.ascx", false, ItemTileParms));
            Rotator1Tiles.Add(new Tile("Item Advanced", "~/Controls/Items/ItemAdvanced.ascx", false, ItemTileParms));
            Rotator1Tiles.Add(new Tile(Permission.Tile_ItemStatistics, "Item Statistics", "~/Controls/Items/ItemStatistics.ascx", false, ItemTileParms));
        }

        protected void ScheduleClassTileClick(TileParms tileParms)
        {
            SessionObject.clickedClass = (Thinkgate.Base.Classes.Class)tileParms.GetParm("class");
            ReloadTilesControl("Classes");
        }

        #endregion

        protected override object LoadRecord(int xId)
        {
            return TestQuestion.GetTestQuestionByID(xId);
        }
    }
}