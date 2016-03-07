using System;
using System.Collections.Generic;
using Thinkgate.Classes;
using Thinkgate.Base.Enums;

namespace Thinkgate.Record
{
    public partial class InstructionalPlan : RecordPage
    {
        #region Variables

        private int _planId;
        private Base.Classes.InstructionalPlan _selectedPlan;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.InstructionalPlan + "_"; }
        }

        #endregion

        #region Page Events

        protected new void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var siteMaster = this.Master as SiteMaster;
                if (siteMaster != null)
                {
                    siteMaster.BannerType = BannerType.ObjectScreen;                    
                    siteMaster.Banner.AddOnClientItemClicked(Banner.ContextMenu.Actions, "actionsMenuItemClicked");
                }               
            }

            SessionObject = (SessionObject)Session["SessionObject"];
            SetupFolders();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            LoadInstructionalPlan();
            if (_selectedPlan == null) return;
            LoadDefaultFolderTiles();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_selectedPlan == null) return;

            planName.Text = _selectedPlan.Title;
        }

        #endregion

        private void LoadInstructionalPlan()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _planId = GetDecryptedEntityId(X_ID);
                if (RecordExistsInCache(Key))
                {
                    _selectedPlan = ((Base.Classes.InstructionalPlan)Base.Classes.Cache.Get(Key));
                }
            }
        }

        #region Folder Methods

        private void SetupFolders()
        {
            Folders = new List<Folder>();

            //TODO: Should Terms be loaded dynamically
            Folders.Add(new Folder("Term 1", "", LoadTerm1, "~/ContainerControls/TileContainerDockSix.ascx", 6));
            Folders.Add(new Folder("Term 2", "", LoadTerm4, "~/ContainerControls/TileContainerDockSix.ascx", 6));
            Folders.Add(new Folder("Term 3", "", LoadTerm3, "~/ContainerControls/TileContainerDockSix.ascx", 6));
            Folders.Add(new Folder("Term 4", "", LoadTerm2, "~/ContainerControls/TileContainerDockSix.ascx", 6));                      
            
            ctlFolders.BindFolderList(Folders);
        }

        #endregion

        #region Tile Methods

        private void LoadTerm1()
        {
            LoadTiles(1);
        }

        private void LoadTerm2()
        {
            LoadTiles(2);
        }

        private void LoadTerm3()
        {
            LoadTiles(3);
        }

        private void LoadTerm4()
        {
            LoadTiles(4);
        }

        private void LoadTiles(int term)
        {
            if (_selectedPlan == null) return;

            var pacingParms = new TileParms();
            pacingParms.AddParm("planID", _planId);
            Rotator1Tiles.Add(new Tile(Permission.Tile_Pacing, "Pacing", "~/Controls/Plans/Pacing.ascx", false, pacingParms, null,
                                "~/Dialogues/InstructionalPlanCalendar.aspx", "~/Dialogues/Pacing.aspx", false, "list", "expandPacingTile", "editPacingTile"));

            var classTileParms = new TileParms();
            classTileParms.AddParm("class", _selectedPlan.Class);
            classTileParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
            classTileParms.AddParm("levelID", _selectedPlan.TeacherPage);
            classTileParms.AddParm("selectID", _selectedPlan.Class.ID);
            classTileParms.AddParm("category", "Classroom");
            classTileParms.AddParm("folder", "Classes");
            classTileParms.AddParm("planID", _planId);
            classTileParms.AddParm("term", term);
            classTileParms.AddParm("showCalendarIcon", true);

            if (UserHasPermission(Base.Enums.Permission.Tile_Standards))
            {
                Rotator1Tiles.Add(new Tile("Standards", "~/Controls/Standards/StandardsSearch.ascx",
                                            false, classTileParms, null,
                                            "../Controls/Standards/StandardsSearch_ExpandedV2.aspx"));
            }

            var lessonPlanParms = new TileParms();
            lessonPlanParms.AddParm("showCalendarIcon", true);
            lessonPlanParms.AddParm("selectID", _selectedPlan.Class.ID);
            lessonPlanParms.AddParm("planID", _planId);
            lessonPlanParms.AddParm("term", term);
            Rotator1Tiles.Add(new Tile(Permission.Tile_LessonPlans, "Lesson Plans", "~/Controls/Plans/LessonPlans.ascx", false,  lessonPlanParms));

            TileParms districtAssessmentParms = new TileParms();
            districtAssessmentParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Class);
            districtAssessmentParms.AddParm("category", "District");
            districtAssessmentParms.AddParm("userID", _selectedPlan.TeacherPage);
            districtAssessmentParms.AddParm("levelID", _selectedPlan.Class.ID);
            districtAssessmentParms.AddParm("showCalendarIcon", true);
            Rotator1Tiles.Add(new Tile("District Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, districtAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=District", null));

            TileParms classroomAssessmentParms = new TileParms();
            classroomAssessmentParms.AddParm("class", _selectedPlan.Class);
            classroomAssessmentParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Class);
            classroomAssessmentParms.AddParm("category", "Classroom");
            classroomAssessmentParms.AddParm("userID", _selectedPlan.TeacherPage);
            classroomAssessmentParms.AddParm("levelID", _selectedPlan.Class.ID);
            classroomAssessmentParms.AddParm("folder", "Classes");
            classroomAssessmentParms.AddParm("showCalendarIcon", true);
            Rotator1Tiles.Add(new Tile("Classroom Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, classroomAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=Classroom", null));
            
            var resourceParms = new TileParms();
            resourceParms.AddParm("showCalendarIcon", true);
            resourceParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.InstructionalPlan);
            resourceParms.AddParm("levelID", _selectedPlan.ID);
            resourceParms.AddParm("selectID", _selectedPlan.Class.ID);
            resourceParms.AddParm("planID", _planId);
            resourceParms.AddParm("term", term);
            Rotator1Tiles.Add(new Tile(Permission.Tile_Resources, "Resources", "~/Controls/Documents/Resources.ascx", false, resourceParms));

        }

        #endregion

        protected override object LoadRecord(int xId)
        {
            return Base.Classes.InstructionalPlan.GetPlanByID(xId);
        }

        protected override void UpdateCache(string key, object record)
        {
            if (!RecordExistsInCache(Key))
            {
                base.UpdateCache(Key, record);
            }
        }
    }
}
