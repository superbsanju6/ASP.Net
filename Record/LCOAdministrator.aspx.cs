using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Base.Enums;
using System.Linq;

namespace Thinkgate.Record
{
    public partial class LCOAdministrator : RecordPage
    {
        private Int32 _userID;
        private List<LCO> _lcos = new List<LCO>();

        protected new void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            GetLCORole();
            LoadLCOFolders();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            LoadLCOTiles();
            if (SessionObject.LoggedInUser == null) return;
            LoadDefaultFolderTiles();  
        }

        private void GetLCORole()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {   SessionObject.LCOrole = EntityTypes.LCOAdministrator;
                _userID = SessionObject.LoggedInUser.Page;
                foreach (var lco in LCO.GetLCOCourseListByUserID(_userID))
                {
                    _lcos.Add(lco);
                }
                
            }
        }

        private void LoadLCOFolders()
        {
            Folders = new List<Folder>();

            Folders.Add(new Folder("Local Courses", "~/Images/new/folder_classes.png", LoadLCOTiles,
                             "~/ContainerControls/TileContainer_3_1.ascx", 3,
                             "~/ContainerControls/TileContainer_3_1.ascx", 0, string.Empty,
                             string.Empty));

            ctlFolders.BindFolderList(Folders);
        }

        private void LoadLCOTiles()
        {
            if (UserHasPermission(Permission.Tile_Staff))
            {
                TileParms lcoStaffParms = new TileParms();
                lcoStaffParms.AddParm("levelID", _userID);
                Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_Staff, "Staff", "~/Controls/LCO/StaffSearch.ascx", false, lcoStaffParms, null,
                             (UserHasPermission(Permission.Icon_ExpandedSearch_Staff)) ? "" : ""));
            }
        }
    }
}