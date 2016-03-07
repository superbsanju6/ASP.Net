using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Services.Contracts.ImprovementPlanService;
using Thinkgate.Utilities;

namespace Thinkgate.Record
{
	public partial class School : RecordPage
	{
        #region Variables

        private int _schoolId;
		private Base.Classes.School _selectedSchool;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.School + "_"; }
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
            SessionObject.CurrentPortal = EntityTypes.School;
			SetupFolders();
			InitPage(ctlFolders, ctlDoublePanel, sender, e);
			LoadSchool();
			if (_selectedSchool == null) return;
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


		#endregion

        private void SetHeaderImageUrl()
        {
            schoolImage.ImageUrl = AppSettings.IsIllinois ? "~/Images/ISLE_logo1.png" : "~/Images/new/school.png";
            }

		private void LoadSchool()
		{
			if (Request.QueryString["xID"] == null)
			{
				SessionObject.RedirectMessage = "No entity ID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else
			{
                _schoolId = GetDecryptedEntityId(X_ID);
                SessionObject.LoggedInUser.School = _schoolId;
              
				var key = "School_" + _schoolId;
				if (!RecordExistsInCache(key)) return;
				_selectedSchool = ((Base.Classes.School)Base.Classes.Cache.Get(key));
				schoolName.Text = _selectedSchool.Name;
			}
		}

		#region Folder Methods

		private void SetupFolders()
		{
			Folders = new List<Folder>();

			if (UserHasPermission(Permission.Folder_Profile)) Folders.Add(new Folder("Profile", "~/Images/new/folder_profile.png", LoadProfileTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));

			if (UserHasPermission(Permission.Folder_Curriculum))
			{
				Folders.Add(new Folder("Planning", "~/Images/new/folder_curriculum.png", LoadPlanningTiles,
															 "~/ContainerControls/TileContainerDockSix.ascx", 6));
			}

			if (UserHasPermission(Permission.Folder_Instruction)) Folders.Add(new Folder("Instruction", "~/Images/new/folder_instruction.png", LoadInstructionTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));
			if (UserHasPermission(Permission.Folder_Assessment)) Folders.Add(new Folder("Assessment", "~/Images/new/folder_assessment.png", LoadAssessmentTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));

			if (SessionObject.LoggedInUser.HasPermission(Permission.Folder_Reporting))
				Folders.Add(new Folder("Reporting", "~/Images/new/folder_data_analysis.png", LoadReportingTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));

			if (UserHasPermission(Permission.Folder_Professional_Development_School))
				Folders.Add(new Folder("PD", "~/Images/new/folder_professional_dev.png", LoadPDTiles, "~/ContainerControls/TileContainer_3_1.ascx", 3, "~/ContainerControls/TileContainer_3_1.ascx", 3));

			if (UserHasPermission(Permission.Folder_Staff_Evaluations))
				Folders.Add(new Folder("Evaluations", "~/Images/new/folder_professional_dev.png", LoadEvalTiles, "~/ContainerControls/TileContainer_3_1.ascx", 3, "~/ContainerControls/TileContainer_3_1.ascx", 3));

			if (SessionObject.LoggedInUser.HasPermission(Permission.Folder_Linking))
				Folders.Add(new Folder("Linking", "~/Images/blank.png", LoadFolderLinking, "~/ContainerControls/TileContainer_1.ascx", 1));

            //Verify Kentico is Enabled
            bool KenticoEnabled = (Session["KenticoEnabled"] != null) ? (bool)Session["KenticoEnabled"] : false;
            if (UserHasPermission(Permission.Folder_RTI_Forms) && KenticoEnabled)
                Folders.Add(new Folder("RTI", "~/Images/new/folder_professional_dev.png", LoadMtssTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));

			ctlFolders.BindFolderList(Folders);
		}

		#endregion

		#region Tile Methods
		private void LoadFolderLinking()
		{
			Rotator1Tiles.Add(new Tile(Permission.Folder_Linking, "Groups", "~/Controls/Groups/GroupSingleUser.ascx"));
		}

		private void LoadProfileTiles()
		{
			var schoolTileParms = new TileParms();
			schoolTileParms.AddParm("school", _selectedSchool);

			var searchTileParms = new TileParms();
			searchTileParms.AddParm("level", EntityTypes.School);
			searchTileParms.AddParm("levelID", _selectedSchool.ID);
			searchTileParms.AddParm("levelType", _selectedSchool.Type);

			var editSchoolUrl = UserHasPermission(Permission.Icon_Edit_School_Identification)
															? "../Controls/School/SchoolIdentification_Edit.aspx?xID=" +
																Standpoint.Core.Classes.Encryption.EncryptInt(_selectedSchool.ID)
															: null;
			Rotator1Tiles.Add(new Tile(Permission.Tile_Identification_School, "Identification", "~/Controls/School/SchoolIdentification.ascx", false,
																	schoolTileParms, null, null, editSchoolUrl));

			Rotator1Tiles.Add(new Tile(Permission.Tile_Staff,
																	"Staff",
																	"~/Controls/Teacher/TeacherSearch.ascx",
																	false,
																	searchTileParms,
																	null,
																	UserHasPermission(Permission.Icon_ExpandedSearch_Staff) ? "../Controls/Teacher/TeacherSearch_Expanded.aspx" : ""));

			Rotator1Tiles.Add(new Tile(Permission.Tile_Classes,
																	"Classes",
																	"~/Controls/Class/ClassSearch.ascx",
																	false,
																	searchTileParms,
																	null,
																	UserHasPermission(Permission.Icon_ExpandedSearch_Classes) ? "../Controls/Class/ClassSearch_Expanded.aspx" : ""));

			var studentSearchUrl = "../Controls/Student/StudentSearch_Expanded.aspx?level=School&levelID=" + Standpoint.Core.Classes.Encryption.EncryptInt(_selectedSchool.ID);
			Rotator1Tiles.Add(new Tile(Permission.Tile_Students,
																	"Students",
																	"~/Controls/Student/StudentSearch.ascx",
																	false,
																	searchTileParms,
																	null,
																	UserHasPermission(Permission.Icon_ExpandedSearch_Students) ? studentSearchUrl : "",
																	string.Empty,
																	false,
																	"list"));

			Rotator1Tiles.Add(new Tile(Permission.Tile_StaffDemographics_School,
																   "Staff Demographics",
																   "~/Controls/Staff/StaffDemographics.ascx",
																   false,
																   searchTileParms,
																   null, null));

			Rotator1Tiles.Add(new Tile(Permission.Tile_Absences_School,
																 "Absences/Tardies (10+)",
																 "~/Controls/School/AbsenceRates.ascx",
																 false,
																 searchTileParms,
																 null, null));

			Rotator1Tiles.Add(new Tile(Permission.Tile_SuspensionRates_School,
																	 "Suspension Rates",
																	 "~/Controls/School/SuspensionRates.ascx",
																	 false,
																	 searchTileParms,
																	 null, null));

			if (_selectedSchool.HasHighSchoolClasses)
			{
				Rotator1Tiles.Add(new Tile(Permission.Tile_GraduationRates_School,
											"Graduation Rates",
											"~/Controls/School/SchoolGraduationRates.ascx",
											false,
											searchTileParms,
											null,
											""));
				Rotator1Tiles.Add(new Tile(Permission.Tile_DropoutPrevention_School,
											"Dropout Rates",
											"~/Controls/School/SchoolDropoutPrevention.ascx",
											false,
											searchTileParms,
											null,
											""));
			}

			Rotator1Tiles.Add(new Tile(Permission.Tile_Attendance_School,
																	"Attendance Rates",
																	"~/Controls/School/SchoolAttendanceRates.ascx",
																	false,
																	searchTileParms,
																	null,
																	""));
			//TODO: Add real permission flags and develop functionality for two tiles.
			//_rotator1Tiles.Add(new Tile("UDFs", "~/Controls/School/SchoolUserDefinedFields.ascx", false, schoolTileParms));
			//_rotator1Tiles.Add(new Tile("Demographics", "~/Controls/PlaceholderTile.ascx", false, schoolTileParms));
		}

		private void LoadPlanningTiles()
		{
			var schoolTileParms = new TileParms();
			schoolTileParms.AddParm("level", EntityTypes.School);
			schoolTileParms.AddParm("levelID", _selectedSchool.ID);
			schoolTileParms.AddParm("school", _selectedSchool);

			if (UserHasPermission(Permission.Tile_Standards))
			{
                Rotator1Tiles.Add(new Tile("Standards", "~/Controls/Standards/StandardsSearchDistSchool.ascx", false, schoolTileParms, null, "../Controls/Standards/StandardsSearch_ExpandedV2.aspx"));
                Rotator1Tiles.Add(new Tile("Standard Filter", "~/Controls/Standards/StandardsFilter.ascx", false, null, null, null));

			}

            if ((bool)Session["KenticoEnabled"])
				{
                if (UserHasPermission(Permission.Tile_Competencylists))
                {
                    var resourceTileParms2 = new TileParms();
                    var title = "Competency Lists";
                    resourceTileParms2.AddParm("resourceToShow", "Thinkgate.CompetencyList");
                    resourceTileParms2.AddParm("title", title);
                    resourceTileParms2.AddParm("height", "500");
                    resourceTileParms2.AddParm("width", "900");
                    resourceTileParms2.AddParm("type", "CompetencyLists");
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, resourceTileParms2, null, null));
				}
			}
            if (UserHasPermission(Permission.Tile_DistrictImprovementPlan))
            {
                var improvementPlanTileParms = new TileParms();
                improvementPlanTileParms.AddParm("improvementPlanType", ImprovementPlanType.District);
                Rotator1Tiles.Add(new Tile("District Improvement Plan", "~/Controls/ImprovementPlan/ImprovementPlanTile.ascx",
                    false, improvementPlanTileParms));
		}

            if (UserHasPermission(Permission.Tile_SchoolImprovementPlan))
            {
                var improvementPlanTileParms = new TileParms();
                improvementPlanTileParms.AddParm("improvementPlanType", ImprovementPlanType.School);
                improvementPlanTileParms.AddParm("schoolID", _schoolId);
                Rotator1Tiles.Add(new Tile("School Improvement Plan", "~/Controls/ImprovementPlan/ImprovementPlanTile.ascx",
                    false, improvementPlanTileParms));
            }            
		}

		private void LoadInstructionTiles()
		{
			var schoolTileParms = new TileParms();
			schoolTileParms.AddParm("school", _selectedSchool);

			if ((bool)Session["KenticoEnabled"] == true)
			{
                if (UserHasPermission(Permission.Tile_InstructionalPlans))
                {
				var InstructionalPlanTileParms = new TileParms();
                var title = GetInstructionalPlanTileTitle();
				InstructionalPlanTileParms.AddParm("resourceToShow", "thinkgate.InstructionalPlan");
                InstructionalPlanTileParms.AddParm("title", title);
				InstructionalPlanTileParms.AddParm("height", "500");
				InstructionalPlanTileParms.AddParm("width", "900");
				InstructionalPlanTileParms.AddParm("type", "InstructionalPlans");
                InstructionalPlanTileParms.AddParm("typeKey", ((int)LookupDetail.CurriculumMap).ToString());
                Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, InstructionalPlanTileParms, null, UserHasPermission(Permission.Icon_Expand_CurriculumMap) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)LookupDetail.CurriculumMap).ToString() : null));
                }

                if (UserHasPermission(Permission.Tile_UnitPlans))
                {
				var UnitPlanTileParms = new TileParms();
                var title = GetUnitPlanTileTitle();
                UnitPlanTileParms.AddParm("resourceToShow", "thinkgate.UnitPlan");
                UnitPlanTileParms.AddParm("title", title);
				UnitPlanTileParms.AddParm("height", "500");
				UnitPlanTileParms.AddParm("width", "900");
				UnitPlanTileParms.AddParm("type", "UnitPlans");
                UnitPlanTileParms.AddParm("typeKey", ((int)LookupDetail.UnitPlan).ToString());
                Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, UnitPlanTileParms, null, UserHasPermission(Permission.Icon_Expand_Unitplan) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)LookupDetail.UnitPlan).ToString() : null));
                }

                if (UserHasPermission(Permission.Tile_LessonPlansResources))
                {
				var LessonPlanTileParms = new TileParms();
                var title = GetLessonPlanTileTitle();
                LessonPlanTileParms.AddParm("resourceToShow", "thinkgate.LessonPlan");
                LessonPlanTileParms.AddParm("title", title);
				LessonPlanTileParms.AddParm("height", "500");
				LessonPlanTileParms.AddParm("width", "900");
				LessonPlanTileParms.AddParm("type", "LessonPlans");
                LessonPlanTileParms.AddParm("typeKey", ((int)LookupDetail.LessonPlan).ToString());
                Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, LessonPlanTileParms, null, UserHasPermission(Permission.Icon_Expand_Lessonplan) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)LookupDetail.LessonPlan).ToString() : null));
                }

              
                if (UserHasPermission(Permission.Tile_ModelCurriculumUnit))
                {
                    string clientState = DistrictParms.State;
                    TileParms modelCurriculumParms = new TileParms();
                    string title;
                    switch (clientState)
                    {
                        case "OH": //OHIO
                            title = GetModelCurriculumTileTitle();
                            modelCurriculumParms.AddParm("resourceToShow", "thinkgate.curriculumUnit");
                            modelCurriculumParms.AddParm("title", title);
                            modelCurriculumParms.AddParm("height", "500");
                            modelCurriculumParms.AddParm("width", "900");
                            modelCurriculumParms.AddParm("type", "Curriculum Unit");
                            modelCurriculumParms.AddParm("showStateOnly", true);
                            modelCurriculumParms.AddParm("typeKey", ((int)LookupDetail.CurriculumUnitOH).ToString());
                            Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, modelCurriculumParms, null, UserHasPermission(Permission.Icon_Expand_Curriculumunit) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)LookupDetail.CurriculumUnitOH) : null));
                            break;
                        case "MA": //MASS
                            title = GetModelCurriculumTileTitle();
                            modelCurriculumParms.AddParm("resourceToShow", "thinkgate.UnitPlan");
                            modelCurriculumParms.AddParm("title", title);
                            modelCurriculumParms.AddParm("height", "500");
                            modelCurriculumParms.AddParm("width", "900");
                            modelCurriculumParms.AddParm("type", "UnitPlans");
                            modelCurriculumParms.AddParm("showStateOnly", true);
                            modelCurriculumParms.AddParm("typeKey", ((int)LookupDetail.CurriculumUnitMA).ToString());
                            Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, modelCurriculumParms, null, UserHasPermission(Permission.Icon_Expand_Curriculumunit) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)LookupDetail.UnitPlan) : null));
                            break;
                    }
                }
               

                if (DistrictParms.Tile_Kentico_Resources == "Yes")
                {
                   
                    var resourceTileParms3 = new TileParms();
                    var title = GetResourceTileTitle();
                    resourceTileParms3.AddParm("resourceToShow", GetKenticoResourceTileResourceTypeName());
                    resourceTileParms3.AddParm("title", title);
                    resourceTileParms3.AddParm("height", "500");
                    resourceTileParms3.AddParm("width", "900");
                    resourceTileParms3.AddParm("type", "Resources");
                    resourceTileParms3.AddParm("typeKey", ((int)LookupDetail.Resource).ToString(CultureInfo.CurrentCulture));
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, resourceTileParms3, null, UserHasPermission(Permission.Icon_Expand_Resources) ? "../Controls/Resources/ResourceSearchKentico.aspx" : null));
                }

			}
            else
            {
                TileParms unitPlanParms = new TileParms();
                unitPlanParms.AddParm("level", EntityTypes.School);
                unitPlanParms.AddParm("levelID", _selectedSchool.ID);
                unitPlanParms.AddParm("type", "Unit Plans");
                Rotator1Tiles.Add(new Tile(Permission.Tile_UnitPlans, "Unit Plans", "~/Controls/Documents/Resources.ascx", false, unitPlanParms, null, "../Controls/Resources/ResourceSearch.aspx"));

                TileParms lessonPlanParms = new TileParms();
                lessonPlanParms.AddParm("level", EntityTypes.School);
                lessonPlanParms.AddParm("levelID", _selectedSchool.ID);
                lessonPlanParms.AddParm("type", "Lesson Plans");
                Rotator1Tiles.Add(new Tile(Permission.Tile_LessonPlansResources, "Lesson Plans", "~/Controls/Documents/Resources.ascx", false, lessonPlanParms, null, "../Controls/Resources/ResourceSearch.aspx"));

                TileParms pacingDocumentsParms = new TileParms();
                pacingDocumentsParms.AddParm("level", EntityTypes.School);
                pacingDocumentsParms.AddParm("levelID", _selectedSchool.ID);
                pacingDocumentsParms.AddParm("type", "Pacing Documents");
                Rotator1Tiles.Add(new Tile(Permission.Tile_PacingDocuments, "Pacing Documents", "~/Controls/Documents/Resources.ascx", false, pacingDocumentsParms, null, "../Controls/Resources/ResourceSearch.aspx"));
			}

            
            if (DistrictParms.Tile_E3_Resources == "Yes")
            {
            TileParms plansParms = new TileParms();
            plansParms.AddParm("level", EntityTypes.School);
            plansParms.AddParm("levelID", _selectedSchool.ID);
            plansParms.AddParm("type", "Resources");
            var resourcesTileFilterToDisplay = DistrictParms.LoadDistrictParms().ResourcesTileFilterToDisplay;
            if (!string.IsNullOrEmpty(resourcesTileFilterToDisplay)) plansParms.AddParm("UseResourcesTileFilterToDisplay", resourcesTileFilterToDisplay);

            var resourceTileExpand = UserHasPermission(Permission.Icon_Expand_Resources) ? "../Controls/Resources/ResourceSearch.aspx" : null;
            Rotator1Tiles.Add(new Tile(Permission.Tile_School_Resources, "Resources", "~/Controls/Documents/Resources.ascx", false, plansParms, null, resourceTileExpand));
            }

            if (UserHasPermission(Permission.Tile_Playbook))
            {
                Rotator1Tiles.Add(new Tile("Playbooks", "~/Controls/Plans/Playbook.ascx", false, null));
            }

			Rotator1Tiles.Add(new Tile(Permission.Tile_School_Attendance, "Attendance", "~/Controls/School/SchoolAttendance.ascx", false, schoolTileParms));
			Rotator1Tiles.Add(new Tile(Permission.Tile_School_Discipline, "Discipline", "~/Controls/School/SchoolDiscipline.ascx", false, schoolTileParms));
			Rotator1Tiles.Add(new Tile(Permission.Tile_School_Grades, "Grades", "~/Controls/School/SchoolGrades.ascx", false, schoolTileParms));            

		}

		private void LoadReportingTiles()
		{
			var schoolTileParms = new TileParms();
			schoolTileParms.AddParm("level", EntityTypes.School);
			schoolTileParms.AddParm("levelID", _selectedSchool.ID);
			schoolTileParms.AddParm("selectID", _selectedSchool.ID);
			schoolTileParms.AddParm("school", _selectedSchool);
			schoolTileParms.AddParm("folder", "Reporting");

			Rotator1Tiles.Add(new Tile(Permission.Tile_AdvancedReporting, "Advanced Reporting", "~/Controls/Reports/AdvancedReporting.ascx", false, schoolTileParms));

			var archivedReportingTileParms = new TileParms();
			archivedReportingTileParms.AddParm("archives", "yes");
			Rotator1Tiles.Add(new Tile(Permission.Tile_ArchivedReporting, "Archived Reporting", "~/Controls/Reports/AdvancedReporting.ascx", false, archivedReportingTileParms));

			Rotator1Tiles.Add(new Tile(Permission.Tile_Growth_Analysis, "Growth Analysis", "~/Controls/School/SchoolGrowthAnalysis.ascx", false, schoolTileParms));
			Rotator1Tiles.Add(new Tile(Permission.Tile_School_ProficiencyAnalysis, "Proficiency Analysis", "~/Controls/School/SchoolProficiencyAnalysis.ascx", false, schoolTileParms));
#if false
						_rotator1Tiles.Add(new Tile("Results", "~/Controls/School/SchoolResults.ascxc", false, schoolTileParms));
#endif
			Rotator1Tiles.Add(new Tile(Permission.Tile_AssessmentResults, "Assessment Results", "~/Controls/Assessment/AssessmentResults.ascx", false, schoolTileParms, null, "../Controls/Assessment/AssessmentResults_ExpandedV2.aspx", null, false, null, "expandAssessmentResults"));
			Rotator1Tiles.Add(new Tile(Permission.Tile_School_EarlyWarning, "Early Warning", "~/Controls/School/SchoolEarlyWarning.ascx", false, schoolTileParms));
			Rotator1Tiles.Add(new Tile(Permission.Tile_School_Readiness, "Readiness", "~/Controls/School/SchoolReadiness.ascx", false, schoolTileParms));
		}

		private void LoadPDTiles()
		{
			if (_selectedSchool == null)
				return;
		}

		private void LoadAssessmentTiles()
		{
			if (_selectedSchool == null)
				return;

			var schoolTileParms = new TileParms();
			schoolTileParms.AddParm("school", _selectedSchool);

			TileParms classroomAssessmentParms = new TileParms();
			classroomAssessmentParms.AddParm("level", EntityTypes.School);
			classroomAssessmentParms.AddParm("levelID", _selectedSchool.ID);
			classroomAssessmentParms.AddParm("category", "Classroom");

			TileParms districtAssessmentParms = new TileParms();
			districtAssessmentParms.AddParm("level", EntityTypes.School);
			districtAssessmentParms.AddParm("levelID", _selectedSchool.ID);
			districtAssessmentParms.AddParm("category", "District");

            TileParms blueprintAssessmentParms = new TileParms();
            blueprintAssessmentParms.AddParm("level", EntityTypes.School);
            blueprintAssessmentParms.AddParm("levelID", _selectedSchool.ID);
            blueprintAssessmentParms.AddParm("category", "Blueprint");

            Rotator1Tiles.Add(new Tile(Permission.Tile_DistrictAssessment, "District Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, districtAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=District&level=School", null));
            Rotator1Tiles.Add(new Tile(Permission.Tile_ClassroomAssessment, "Classroom Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, classroomAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=Classroom&level=School", null));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Items, "Items", "~/Controls/Items/Items.ascx", false, schoolTileParms, null, UserHasPermission(Permission.Search_Item) ? "../Controls/Items/ItemSearch.aspx" : null, null, false, null, null, null, null, "assessmentItemsExportToExcel"));
			Rotator1Tiles.Add(new Tile(Permission.Tile_Images, "Images", "~/Controls/Assessment/AssessmentItems.ascx", false, schoolTileParms, null, UserHasPermission(Permission.Search_Image) ? "../Controls/Images/ImageSearch_ExpandedV2.aspx" : null));
			Rotator1Tiles.Add(new Tile(Permission.Tile_Addendum, "Addendums", "~/Controls/Assessment/AssessmentItems.ascx", false, schoolTileParms, null, UserHasPermission(Permission.Search_Addendum) ? "../Controls/Addendums/AddendumSearch_ExpandedV2.aspx" : null));
			Rotator1Tiles.Add(new Tile(Permission.Tile_Rubrics, "Rubrics", "~/Controls/Assessment/AssessmentItems.ascx", false, schoolTileParms, null, UserHasPermission(Permission.Search_Rubric) ? "../Controls/Rubrics/RubricSearch_ExpandedV2.aspx" : null));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Blueprint, "Blueprints", "~/Controls/Assessment/ViewBlueprints.ascx", false, blueprintAssessmentParms, null, null, string.Empty, false, "graphical"));
		}

		private void LoadEvalTiles()
		{
			TileParms schoolTileParmsCi = new TileParms();
			schoolTileParmsCi.AddParm("level", EntityTypes.School);
			schoolTileParmsCi.AddParm("levelID", _selectedSchool.ID);
			schoolTileParmsCi.AddParm("evalType", EvaluationTypes.TeacherClassroom);

			TileParms schoolTileParmsNci = new TileParms();
			schoolTileParmsNci.AddParm("level", EntityTypes.School);
			schoolTileParmsNci.AddParm("levelID", _selectedSchool.ID);
			schoolTileParmsNci.AddParm("evalType", EvaluationTypes.TeacherNonClassroom);

			TileParms schoolTileParmsSa = new TileParms();
			schoolTileParmsSa.AddParm("level", EntityTypes.School);
			schoolTileParmsSa.AddParm("levelID", _selectedSchool.ID);
			schoolTileParmsSa.AddParm("evalType", EvaluationTypes.Administrator);


			if (UserHasPermission(Permission.Tile_ClassroomInstructionalStaff))
				Rotator1Tiles.Add(new Tile("Classroom Instructional Staff", "~/Controls/Staff/StaffEvaluationSearch.ascx", false, schoolTileParmsCi));
			if (UserHasPermission(Permission.Tile_NonClassroomInstructionalStaff))
				Rotator1Tiles.Add(new Tile("Non-Classroom Instructional Staff", "~/Controls/Staff/StaffEvaluationSearch.ascx", false, schoolTileParmsNci));
			if (UserHasPermission(Permission.Tile_SchoolBasedAdministrativeStaff))
				Rotator1Tiles.Add(new Tile("School-Based Administrative Staff", "~/Controls/Staff/StaffEvaluationSearch.ascx", false, schoolTileParmsSa));
		}

        private void LoadMtssTiles()
        {
            DistrictParms districtParms = DistrictParms.LoadDistrictParms();

            var resourceTileParms1 = new TileParms();
            var title1 = GetRtiFormTileTitle();
            resourceTileParms1.AddParm("level", EntityTypes.School);
            resourceTileParms1.AddParm("resourceToShow", "thinkgate.MTSSForms");
            resourceTileParms1.AddParm("title", title1);
            resourceTileParms1.AddParm("height", "500");
            resourceTileParms1.AddParm("width", "900");
            resourceTileParms1.AddParm("type", "MTSSForms");
            Rotator1Tiles.Add(new Tile(Permission.Tile_RTIForms_SchoolPortal, title1, "~/Controls/Documents/MTSSFormsDocumentTile.ascx", false, resourceTileParms1));

            var resourceTileParms2 = new TileParms();
            var title2 = GetRtiAnalysisTileTitle();
            resourceTileParms2.AddParm("level", EntityTypes.School);
            resourceTileParms2.AddParm("resourceToShow", "thinkgate.MTSSAnalysis");
            resourceTileParms2.AddParm("title", title2);
            resourceTileParms2.AddParm("height", "500");
            resourceTileParms2.AddParm("width", "900");
            resourceTileParms2.AddParm("type", "MTSSAnalysis");
            resourceTileParms2.AddParm("isFormColumnEnabled", UserHasPermission(Permission.Tile_RTIForms_DistrictPortal));
            var link = ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + System.Web.HttpUtility.UrlEncode("fastsql_v2_direct.asp?ID=7274|Testgate_ViewRoster&??x=find");
            Rotator1Tiles.Add(new Tile(Permission.Tile_RTIAnalysis_SchoolPortal, title2, "~/Controls/Documents/MTSSAnalysisDocumentTile.ascx", false, resourceTileParms2, null, link, null, false, null, "OpenLegacyRTISearch"));

            var resourceTileParms3 = new TileParms();
            var title3 = GetRtiInterventionsTileTitle();
            resourceTileParms3.AddParm("level", EntityTypes.School);
            resourceTileParms3.AddParm("resourceToShow", "thinkgate.MTSSInterventions");
            resourceTileParms3.AddParm("title", title3);
            resourceTileParms3.AddParm("height", "500");
            resourceTileParms3.AddParm("width", "900");
            resourceTileParms3.AddParm("type", "MTSSInterventions");
            resourceTileParms3.AddParm("isFormColumnEnabled", UserHasPermission(Permission.Tile_RTIForms_DistrictPortal));
            Rotator1Tiles.Add(new Tile(Permission.Tile_RTIInterventions_SchoolPortal, title3, "~/Controls/Documents/MTSSInterventionsDocumentTile.ascx", false, resourceTileParms3, null, link, null, false, null, "OpenLegacyRTISearch"));
        }

		#endregion

        protected override object LoadRecord(int xId)
        {
            return Base.Classes.School.GetSchoolByID(xId);
        }
	}
}
