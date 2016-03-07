using System;
using System.Web.UI;
using System.Collections.Generic;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Telerik.Web.UI;
using Thinkgate.Base.Enums;

namespace Thinkgate.Record
{
    public partial class RubricPage : RecordPage
    {
        #region Variables

        private int _rubricId;
        private string _rubricIdEncrypted;
        private Rubric _selectedRubric;
        private string _rubricTitle;
        private RadMenuItem _miDelete;

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
                LoadRubric();
                if (_selectedRubric == null) return;
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
                RubricName.Text = _rubricTitle;

                // Hide the single folder for now.
                FoldersControl.Visible = false;

                //load up hidden input box with encrypted item ID and item type.
                RubricPage_hdnEncryptedData.Value = Request.QueryString["xID"];

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
                RubricImage.ImageUrl = "~/Images/ISLE_logo1.png";
            }
            else
            {
                RubricImage.ImageUrl = "~/Images/new/Rubric.png";
            }
        }

        private void LoadRubric()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _rubricId = GetDecryptedEntityId(X_ID);
                _rubricIdEncrypted = EntityIdEncrypted;

                if (!RecordExistsInCache(Key))
                {
                    _selectedRubric = Rubric.GetRubricByID(_rubricId);
                    if (_selectedRubric != null)
                        Base.Classes.Cache.Insert(Key, _selectedRubric);
                    else
                    {
                        SessionObject.RedirectMessage = "Could not find the Rubric.";
                        Response.Redirect("~/PortalSelection.aspx", true);
                    }
                }
                else _selectedRubric = (Rubric)Cache[Key];

                // Set the page title text.
                _rubricTitle = _selectedRubric.Name ?? string.Empty;

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
            if (_selectedRubric == null) return;

            var rubricTileParms = new TileParms();
            rubricTileParms.AddParm("rubric", _selectedRubric);

            var editURL = "../Controls/Assessment/ContentEditor/ContentEditor_Rubric.aspx?xID=" + _rubricIdEncrypted;
            Rotator1Tiles.Add(new Tile(Permission.Tile_Identification_RubricObjectScreen, 
                                        "Rubric Identification", 
                                        "~/Controls/Rubrics/RubricIdentification.ascx", 
                                        false, 
                                        rubricTileParms,
                                        null, 
                                        null, 
                                        editURL, 
                                        false, 
                                        null, 
                                        null, 
                                        "RubricPage_openExpandEditRadWindow"));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Content_RubricObjectScreen,
                                        "Rubric Content", 
                                        "~/Controls/Rubrics/RubricContent.ascx", 
                                        false, 
                                        rubricTileParms, 
                                        null, 
                                        null, 
                                        editURL, 
                                        false, 
                                        null, 
                                        null, 
                                        "RubricPage_openExpandEditRadWindow"));
        }

        protected void ScheduleClassTileClick(TileParms tileParms)
        {
            SessionObject.clickedClass = (Thinkgate.Base.Classes.Class)tileParms.GetParm("class");
            ReloadTilesControl("Classes");
        }

        #endregion

        protected override object LoadRecord(int xId)
        {
            return Rubric.GetRubricByID(xId);
        }
    }

}