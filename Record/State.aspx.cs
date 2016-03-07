using System;
using System.Collections.Generic;
using Thinkgate.Utilities;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;

namespace Thinkgate.Record
{
    public partial class State : RecordPage
    {
        #region Variables

        private int _districtId;
        private Base.Classes.District _selectedDistrict;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.District + "_"; }
        }

        #endregion

        protected new void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            SessionObject.CurrentPortal = EntityTypes.State;
            SetupFolders();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            LoadDistrict();
            if (_selectedDistrict == null) return;
            if (!IsPostBack)
            {
                SessionObject.ClickedInformationControl = "Profile";
            }

            LoadDefaultFolderTiles();
            ctlFolders.Visible = false;
        }

        private void LoadDistrict()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _districtId = GetDecryptedEntityId(X_ID);

                SessionObject.DistrictTileParms.AddParm("userID", _districtId);

                if (RecordExistsInCache(Key))
                {
                    _selectedDistrict = ((Base.Classes.District) Base.Classes.Cache.Get(Key));
                    districtName.Text = _selectedDistrict.DistrictName;
                }
            }
        }

        private void SetupFolders()
        {
            Folders = new List<Folder>();
            if (UserHasPermission(Permission.Folder_Profile)) Folders.Add(new Folder("Profile", "~/Images/new/folder_profile.png", LoadProfileTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));

            ctlFolders.BindFolderList(Folders);
        }

        private void LoadProfileTiles()
        {
            if (_selectedDistrict == null) return;

            // create params object for tiles
            TileParms stateAssessmentParms = new TileParms();
            stateAssessmentParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.State);
            stateAssessmentParms.AddParm("levelID", _selectedDistrict.ID);
            stateAssessmentParms.AddParm("category", "State");
            stateAssessmentParms.AddParm("folder", "Information");

            SessionObject.DistrictTileParms = stateAssessmentParms;

            // State Assessments tile
            Rotator1Tiles.Add(
                new Tile(Permission.Tile_StateAssessments,
                    "State Assessments",
                    "~/Controls/Assessment/ViewAssessmentsV2.ascx",
                    false,
                    stateAssessmentParms,
                    null,
                    "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=State&level=State",
                    null
            ));

            // Assessment Results tile
            Rotator1Tiles.Add(
                new Tile("Assessment Results",
                    "~/Controls/Assessment/AssessmentResults.ascx",
                    false,
                    stateAssessmentParms,
                    null,
                    "../Controls/Assessment/AssessmentResults_ExpandedV2.aspx",
                    null,
                    false,
                    null,
                    "expandAssessmentResults")
            );

            // Advanced Reporting tile
            Rotator1Tiles.Add(
                new Tile(Permission.Tile_AdvancedReporting,
                    "Advanced Reporting",
                    "~/Controls/Reports/AdvancedReporting.ascx",
                    false,
                    stateAssessmentParms,
                    null,
                    null,
                    null,
                    false,
                    null,
                    null)
            );

            // Standards tile
            Rotator1Tiles.Add(
                new Tile("Standards",
                    "~/Controls/Standards/StandardsSearchDistSchool.ascx",
                    false,
                    stateAssessmentParms,
                    null,
                    "../Controls/Standards/StandardsSearch_ExpandedV2.aspx")
            );

            // Items tile
            Rotator1Tiles.Add(
                new Tile(Thinkgate.Base.Enums.Permission.Tile_Items,
                    "Items",
                    "~/Controls/Items/Items.ascx",
                    false,
                    stateAssessmentParms,
                    null,
                    UserHasPermission(Thinkgate.Base.Enums.Permission.Search_Item) ? "../Controls/Items/ItemSearch.aspx" : null,
                    null,
                    false,
                    null,
                    null,
                    null,
                    null,
                    "assessmentItemsExportToExcel")
            );

            // Addendums tile
            Rotator1Tiles.Add(
                new Tile(Thinkgate.Base.Enums.Permission.Tile_Addendum,
                    "Addendums",
                    "~/Controls/Assessment/AssessmentItems.ascx",
                    false,
                    stateAssessmentParms,
                    null,
                    UserHasPermission(Thinkgate.Base.Enums.Permission.Search_Addendum) ? "../Controls/Addendums/AddendumSearch_ExpandedV2.aspx" : null)
            );

            // Rubrics tile
            Rotator1Tiles.Add(
                new Tile(Thinkgate.Base.Enums.Permission.Tile_Rubrics,
                    "Rubrics",
                    "~/Controls/Assessment/AssessmentItems.ascx",
                    false,
                    stateAssessmentParms,
                    null,
                    UserHasPermission(Thinkgate.Base.Enums.Permission.Search_Rubric) ? "../Controls/Rubrics/RubricSearch_ExpandedV2.aspx" : null)
            );

            // Images tile
            Rotator1Tiles.Add(
                new Tile(Thinkgate.Base.Enums.Permission.Tile_Images,
                    "Images",
                    "~/Controls/Assessment/AssessmentItems.ascx",
                    false,
                    stateAssessmentParms,
                    null,
                    UserHasPermission(Thinkgate.Base.Enums.Permission.Search_Image) ? "../Controls/Images/ImageSearch_ExpandedV2.aspx" : null)
            );

            // Standard Filter tile
            Rotator1Tiles.Add(
                new Tile(Base.Enums.Permission.Tile_Standards,
                    "Standard Filter",
                    "~/Controls/Standards/StandardsFilter.ascx",
                    false,
                    null,
                    null,
                    null)
            );

            // Staff tile
            Rotator1Tiles.Add(
                new Tile(Base.Enums.Permission.Tile_Staff,
                    "Staff",
                    "~/Controls/LCO/StaffSearch.ascx",
                    false,
                    stateAssessmentParms,
                    null,
                    (UserHasPermission(Permission.Icon_ExpandedSearch_Staff)) ? "../Controls/Teacher/TeacherSearch_Expanded.aspx" : "")
            );

        }

        protected override object LoadRecord(int xId)
        {
            return Base.Classes.District.GetDistrictByID(xId);
        }
    }
}
