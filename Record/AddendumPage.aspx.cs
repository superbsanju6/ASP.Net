using System;
using System.Web.UI;
using System.Collections.Generic;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Telerik.Web.UI;
using Thinkgate.Base.Enums;

namespace Thinkgate.Record
{
    public partial class AddendumPage : RecordPage
    {
        #region Variables

        private int _addendumId;
        private string _addendumIdEncrypted;
        private Addendum _selectedAddendum;
        private string _addendumTitle;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.Addendum + "_"; }
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
                var siteMaster = Master as SiteMaster;
                if (siteMaster != null)
                {
                    siteMaster.BannerType = BannerType.ObjectScreen;
                    var miDelete = new RadMenuItem("Delete");
                    siteMaster.Banner.AddMenuItem(Banner.ContextMenu.Actions, miDelete);
                    siteMaster.Banner.AddOnClientItemClicked(Banner.ContextMenu.Actions, "actionsMenuItemClicked");
                }
                
                SessionObject = (SessionObject)Session["SessionObject"];
                SetupFolders();
                InitPage(ctlFolders, ctlDoublePanel, sender, e);
                LoadAddendum();
                if (_selectedAddendum == null) return;
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
                AddendumName.Text = _addendumTitle;

                // Hide the single folder for now.
                FoldersControl.Visible = false;
                //load up hidden input box with encrypted item ID and item type.

                AddendumPage_hdnEncryptedData.Value = Request.QueryString["xID"];

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
                AddendumImage.ImageUrl = "~/Images/ISLE_logo1.png";
            }
            else
            {
                AddendumImage.ImageUrl = "~/Images/new/Addendum.png";
            }
        }

        private void LoadAddendum()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _addendumId = GetDecryptedEntityId(X_ID);
                _addendumIdEncrypted = EntityIdEncrypted;
                var key = "Addendum_" + _addendumId;

                if (!RecordExistsInCache(key))
                {
   				    _selectedAddendum = Addendum.GetAddendumByID(_addendumId);
				    if(_selectedAddendum != null)
					    Base.Classes.Cache.Insert(key, _selectedAddendum);
				    else
				    {
					    SessionObject.RedirectMessage = "Could not find the addendum.";
					    Response.Redirect("~/PortalSelection.aspx", true);
				    }
				}
				else _selectedAddendum = (Addendum)Cache[key];

                // Set the page title text.
                _addendumTitle = _selectedAddendum.Addendum_Name ?? string.Empty;
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
            if (_selectedAddendum == null) return;

            var addendumTileParms = new TileParms();
            addendumTileParms.AddParm("addendum", _selectedAddendum);

            var editUrl = "../Controls/Assessment/ContentEditor/ContentEditor_Addendum.aspx?xID=" + _addendumIdEncrypted;
            Rotator1Tiles.Add(new Tile(Permission.Tile_Identification_AddendumObjectScreen,
                                        "Addendum Identification", 
                                        "~/Controls/Addendums/AddendumIdentification.ascx", 
                                        false,
                                        addendumTileParms, 
                                        null, 
                                        null, 
                                        editUrl, 
                                        false, 
                                        null, 
                                        null, 
                                        "AddendumPage_openExpandEditRadWindow"));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Content_AddendumObjectScreen,
                                        "Addendum Content",
                                        "~/Controls/Addendums/AddendumContent.ascx", 
                                        false, 
                                        addendumTileParms, 
                                        null,
                                        null, 
                                        editUrl, 
                                        false,
                                        null, 
                                        null, 
                                        "AddendumPage_openExpandEditRadWindow"));
        }

        protected void ScheduleClassTileClick(TileParms tileParms)
        {
            SessionObject.clickedClass = (Base.Classes.Class)tileParms.GetParm("class");
            ReloadTilesControl("Classes");
        }

        #endregion


        protected override object LoadRecord(int xId)
        {
            return Addendum.GetAddendumByID(xId);
        }
    }
}
