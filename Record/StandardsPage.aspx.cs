using System;
using System.Collections.Generic;
using Standpoint.Core.Utilities;
using Thinkgate.Classes;
using Telerik.Web.UI;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Record
{
    public partial class Standards : RecordPage
    {
        #region Variables

        private RadMenuItem _miAdd;
        private int _standardId;
        private string _standardIdEncrypted;
        private Base.Classes.Standards _selectedStandard;
        private string _standardTitle;
        private string _from;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.Standards + "_"; }
        }

        #endregion

        #region Page Events

        protected new void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            SetupFolders();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            LoadStandard();
            if (_selectedStandard == null) return;
            LoadDefaultFolderTiles();
            StandardImage.ImageUrl = Page.ResolveUrl(AppSettings.ImagesFolderWebPath + "/New" + "rubric.png");

            SessionObject.Standards_SelectedStandardID = Request.QueryString["xID"] == null ? 100 : DataIntegrity.ConvertToInt(Request.QueryString["xID"]);
            
            if (!IsPostBack)
            {
                /********************************************************************
                 * Set up banner for StandardsPage to display the following actions
                 * as menu options:
                 *          Copy    - Copies current item into User's personal Bank
                 *          Delete  - Deletes the current Item from the system.
                 * *****************************************************************/
                var siteMaster = Master as SiteMaster;
                if (siteMaster != null)
                {
                    siteMaster.BannerType = BannerType.ObjectScreen;
                    _miAdd = new RadMenuItem("Add Item");
                    siteMaster.Banner.AddMenuItem(Banner.ContextMenu.Actions, _miAdd);
                    siteMaster.Banner.AddOnClientItemClicked(Banner.ContextMenu.Actions, "actionsMenuItemClicked");
                    if (!UserHasPermission(Permission.Menu_Actions_AddItem_Standard)) _miAdd.Visible = false;
                }

                SessionObject.ClickedInformationControl = "Identification";

                //LoadStandard(SessionObject.Standards_SelectedStandardID);
            }

            SetHeaderImageUrl();

            if (SessionObject.Standards_SelectedStandard == null) return;

            StandardName.Text = SessionObject.Standards_SelectedStandard.StandardName;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                StandardName.Text = _standardTitle;

                // Hide the single folder for now.
                FoldersControl.Visible = false;

            }
        }

        protected void Page_LoadComplete()
        {
            var siteMaster = Master as SiteMaster;
            if (siteMaster != null && _miAdd != null)
            {
                if (!_miAdd.Visible || !UserHasPermission(Permission.Menu_MyProfile_Actions))
                    siteMaster.BannerControl.HideContextMenu(Banner.ContextMenu.Actions);
            }
        }


        #endregion

        private void SetHeaderImageUrl()
        {
            if (AppSettings.IsIllinois)
            {
                StandardImage.ImageUrl = "~/Images/ISLE_logo1.png";
            }
            else
            {
                StandardImage.ImageUrl = "~/Images/Standards.png";
            }
        }

        private void LoadStandard()
        {
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);

            }
            else
            {
                _standardId = GetDecryptedEntityId(X_ID);
                var key = "Standard_" + _standardId;

                if (!RecordExistsInCache(key))
                {
                    _selectedStandard = Base.Classes.Standards.GetStandardByID(_standardId);
                    if (_selectedStandard != null)
                        Base.Classes.Cache.Insert(key, _selectedStandard);
                    else
                    {
                        SessionObject.RedirectMessage = "Could not find the Standard.";
                        Response.Redirect("~/PortalSelection.aspx", true);
                    }
                }
                else _selectedStandard = (Base.Classes.Standards)Cache[key];

                // Set the page title text.
                if (_selectedStandard != null)
                {
                    _standardTitle = _selectedStandard.StandardName ?? string.Empty;
                }
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
            DistrictParms districtParms = DistrictParms.LoadDistrictParms();
            if (_selectedStandard == null) return;
            _from = Request.QueryString["from"] ?? string.Empty;

            var standardsTileParms = new TileParms();
            standardsTileParms.AddParm("standards", _selectedStandard);

            Rotator1Tiles.Add(new Tile(Permission.Icon_Edit_Standard, "Standard Identification", "~/Controls/Standards/StandardsIdentification.ascx", false, standardsTileParms, null, null, "../Controls/ExpandedPlaceholder.aspx", false, null, null));
            Rotator1Tiles.Add(new Tile("Standard Contents", "~/Controls/Standards/StandardsContent.ascx", false, standardsTileParms));

            if (districtParms.Tile_E3_Resources_StandardsPage == "Yes")
            {
                TileParms plansParms = new TileParms();
                plansParms.AddParm("level", EntityTypes.Standard);
                plansParms.AddParm("levelID", _selectedStandard.ID);
                Rotator1Tiles.Add(new Tile(Permission.Tile_Resources_StandardObjectScreen, "Resources", "~/Controls/Documents/Resources.ascx", false, plansParms, null, "../Controls/Resources/ResourceSearch.aspx"));
            }
            if (districtParms.Tile_Kentico_Resources == "Yes")
            {
               
                var resourceTileParms3 = new TileParms();
                resourceTileParms3.AddParm("resourceToShow", GetKenticoResourceTileResourceTypeName());
                resourceTileParms3.AddParm("title", "Resources");
                resourceTileParms3.AddParm("height", "500");
                resourceTileParms3.AddParm("width", "900");
                resourceTileParms3.AddParm("type", "Resources");                
                resourceTileParms3.AddParm("standards", _selectedStandard);
                Rotator1Tiles.Add(new Tile("Resources", "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, resourceTileParms3, null, "../Controls/Resources/ResourceSearchKentico.aspx?" + string.Format("from={0}&standardID={1}", _from, _selectedStandard.ID)));
            }

        }
        #endregion

        protected override object LoadRecord(int xId)
        {
            return Base.Classes.Standards.GetStandardByID(xId);
        }
    }
}
