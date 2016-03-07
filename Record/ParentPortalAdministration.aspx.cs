using System;
using System.Collections.Generic;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;

namespace Thinkgate.Record
{
    public partial class ParentPortalAdministration : RecordPage
    {
    

        #region Variables

        private ParentPortal _selectedParentPortal;
        private int _parentPortalId;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.ParentPortal + "_"; }
        }

        #endregion

        #region Page Events

        protected new void Page_Init(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["childPage"]))
            {
                var siteMaster = Master as SiteMaster;
                if (siteMaster != null)
                {
                    siteMaster.BannerType = BannerType.ObjectScreen;
                }
            }
            SessionObject = (SessionObject)Session["SessionObject"];

            SessionObject.CurrentPortal = EntityTypes.ParentPortal;
            SetupFolders();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            LoadParentPortal();
            if (_selectedParentPortal == null) return;
            if (!IsPostBack)
            {
                SessionObject.ClickedInformationControl = "Parent Portal";
            }

            LoadDefaultFolderTiles();
            SetHeaderImageUrl();
        }

        protected void Page_LoadComplete()
        {
            var siteMaster = Master as SiteMaster;
            if (siteMaster != null)
            {
                siteMaster.BannerControl.HideContextMenu(Banner.ContextMenu.Actions);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region Private Methods

        private void SetHeaderImageUrl()
        {
            ParentPortalImage.ImageUrl = AppSettings.IsIllinois
                ? "~/Images/ISLE_logo1.png"
                : "~/Images/new/district_alt.png";
        }

        private void LoadParentPortal()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _parentPortalId = GetDecryptedEntityId(X_ID);

                SessionObject.SystemAdministratorParms.AddParm("userID", _parentPortalId);

                if (RecordExistsInCache(Key))
                {
                    _selectedParentPortal = (ParentPortal)Cache.Get(Key);
                    ParentPortalName.Text = _selectedParentPortal.ClientName;
                }
            }
        }

        #endregion

        #region Folder Methods

        private void SetupFolders()
        {
            Folders = new List<Folder>();
            if (UserHasPermission(Permission.Folder_SysAdmin))
            {
                Folders.Add(new Folder("Parent Portal", "~/Images/new/folder_profile.png", LoadParentPortalTile, "~/ContainerControls/TileContainerDockSix.ascx", 6));
            }

            ctlFolders.BindFolderList(Folders);
        }

        #endregion

        #region Tile Methods

        private void LoadParentPortalTile()
        {
            if (_selectedParentPortal == null) return;

            if (_selectedParentPortal == null) return;

            var parentPortalParms = new TileParms();
            parentPortalParms.AddParm("level", EntityTypes.ParentPortal);
            //parentPortalParms.AddParm("levelID", _selectedDistrict.ID);
            //parentPortalParms.AddParm("selectID", _selectedDistrict.ID);
            //parentPortalParms.AddParm("district", _selectedDistrict);
            //parentPortalParms.AddParm("folder", "Administation");

            Rotator1Tiles.Add(new Tile(Permission.Tile_ParentStudentPortal, "Parent / Student Portal", "~/Controls/ParentPortalAdministration/ParentStudentPortalAdministration.ascx", false, parentPortalParms, null, null, null, false, null, null));

         }

        #endregion

        #region Overridden Methods

        protected override object LoadRecord(int xId)
        {
            return ParentPortal.GetParentPortalByID(xId);
        }

        #endregion
    }

}