using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Base.Enums;
using System.Linq;

namespace Thinkgate.Record
{
    public partial class IMC : RecordPage
    {
        #region Variables

        private Int32 _userId;
        private readonly List<LCO> _lcos = new List<LCO>();
        private readonly List<LCO> _alllcos = new List<LCO>();

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get
            {
                return EntityTypes.IMC + "_"; }
        }

        #endregion

        protected new void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            GetLcoRole();
            LoadLcoFolders();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            LoadLcoTiles();
            if (SessionObject.LoggedInUser == null) return;
            LoadDefaultFolderTiles();  
        }

        private void GetLcoRole()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {   _userId = SessionObject.LoggedInUser.Page;
                SessionObject.LCOrole = EntityTypes.IMC;
                foreach (var lco in LCO.GetLCOCourseListByUserID(_userId))
                {
                    _lcos.Add(lco);
                }
                foreach (var alllco in LCO.GetAllLCOCourses())
                {
                    _alllcos.Add(alllco);
                }
                
            }
        }

        private void LoadLcoFolders()
        {
            Folders = new List<Folder>();

            Folders.Add(new Folder("Local Courses", "~/Images/new/folder_classes.png", LoadLcoTiles,
                             "~/ContainerControls/TileContainer_3_1.ascx", 3,
                             "~/ContainerControls/TileContainer_3_1.ascx", 3, string.Empty,
                             string.Empty));

            ctlFolders.BindFolderList(Folders);
        }

        private void LoadLcoTiles()
        {
            if (UserHasPermission(Permission.Tile_PendingSubmission))
            {
                TileParms pendingSubmissionParms = new TileParms();
                pendingSubmissionParms.AddParm("lcos", _lcos.Where(lco => lco.IsRegionRequested == false).ToList());
                Rotator1Tiles.Add(new Tile(Permission.Tile_PendingSubmission, "Pending Submissions", "~/Controls/LCO/PendingSubmissions.ascx", false, pendingSubmissionParms, null, null));
            }
            
            if (UserHasPermission(Permission.Tile_PendingApproval))
            {
                TileParms pendingApprovalParms = new TileParms();
                pendingApprovalParms.AddParm("lcos", _lcos.Where(lco => ((lco.IsRegionRequested == true) && (lco.IsApproved == false) && (lco.IsInfoRequested != true))).ToList());
                Rotator1Tiles.Add(new Tile(Permission.Tile_PendingApproval, "Pending Approvals", "~/Controls/LCO/PendingApprovals.ascx", false, pendingApprovalParms, null, null));
            }

            if (UserHasPermission(Permission.Tile_LcoInformationRequested))
            {
                TileParms infoReqParms = new TileParms();
                infoReqParms.AddParm("lcos", _lcos.Where(lco => ((lco.IsRegionRequested == true) && (lco.IsApproved == false) && (lco.IsInfoRequested==true))).ToList());
                Rotator1Tiles.Add(new Tile(Permission.Tile_LcoInformationRequested, "Information Requested", "~/Controls/LCO/LcoInformationRequested.ascx", false, infoReqParms, null, null));
            }
            if (UserHasPermission(Permission.Tile_Approved))
            {
                TileParms ApprovedParms = new TileParms();
                ApprovedParms.AddParm("lcos", _lcos.Where(lco => lco.IsApproved == true).ToList());
                Rotator2Tiles.Add(new Tile(Permission.Tile_Approved, "Approved", "~/Controls/LCO/Approved.ascx", false, ApprovedParms, null, null));
            }
            if (UserHasPermission(Permission.Tile_LCO_CourseCatalog))
            {
                TileParms CatalogParms = new TileParms();
                CatalogParms.AddParm("alllcos", _alllcos.Where(alllco => ((alllco.IsApproved == true) && (alllco.IsOriginal==true))).ToList());
                Rotator2Tiles.Add(new Tile(Permission.Tile_LCO_CourseCatalog, "Course Catalog", "~/Controls/LCO/CourseCatalog.ascx", false, CatalogParms, null, null));
            }

        }

        protected override object LoadRecord(int xId)
        {
            return LCO.GetLCOCourseListByUserID(xId);
        }
    }
}