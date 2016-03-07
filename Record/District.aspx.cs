using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Globalization;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Services.Contracts.ImprovementPlanService;
using Thinkgate.Utilities;

namespace Thinkgate.Record
{
	public partial class District : RecordPage
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
            // TFS Bug# 22436 Code fix -- start
            if (SessionObject.CurrentPortal == EntityTypes.Teacher)
            {
                try
                {
                    string teacherID = Request.QueryString[X_ID].ToString();
                    Response.Redirect("~/Record/Teacher.aspx?xID=" + teacherID,false);
                }
                catch
                {
                    Services.Service2.KillSession();
                }
            } // TFS Bug# 22436  Code fix -- end
			SessionObject.CurrentPortal = EntityTypes.District;
			SetupFolders();
			InitPage(ctlFolders, ctlDoublePanel, sender, e);
			LoadDistrict();
			if (_selectedDistrict == null) return;
			if (!IsPostBack)
			{
				SessionObject.ClickedInformationControl = "Profile";
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

		#endregion

        private void SetHeaderImageUrl()
        {
            if (AppSettings.IsIllinois)
            {
                districtImage.ImageUrl = "~/Images/ISLE_logo1.png";
            }
            else
            {
                districtImage.ImageUrl = "~/Images/new/district_alt.png";
            }
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

	    #region Folder Methods

		private void SetupFolders()
		{
			Folders = new List<Folder>();
		    if (UserHasPermission(Permission.Folder_Profile))
		    {
		        Folders.Add(new Folder("Profile", "~/Images/new/folder_profile.png", LoadProfileTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));
		    }

			if (UserHasPermission(Permission.Folder_Curriculum))
			{
				Folders.Add(new Folder("Planning", "~/Images/new/folder_curriculum.png", LoadPlanningTiles,
                                                             "~/ContainerControls/TileContainerDockSix.ascx", 6));
			}

		    if (UserHasPermission(Permission.Folder_Instruction))
		    {
		        Folders.Add(new Folder("Instruction", "~/Images/new/folder_instruction.png", LoadInstructionTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));
		    }

		    if (UserHasPermission(Permission.Folder_Assessment))
		    {
		        Folders.Add(new Folder("Assessment", "~/Images/new/folder_assessment.png", LoadAssessmentTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));
		    }

		    if (SessionObject.LoggedInUser.HasPermission(Permission.Folder_Reporting))
		    {
				Folders.Add(new Folder("Reporting", "~/Images/new/folder_data_analysis.png", LoadReportingTiles, "~/ContainerControls/TileContainer_3_1.ascx", 3, "~/ContainerControls/TileContainer_3_1.ascx", 3));
		    }

			if (UserHasPermission(Permission.Folder_Professional_Development_District))
		    {
		        Folders.Add(new Folder("PD", "~/Images/new/folder_professional_dev.png", LoadPdTiles,
		            "~/ContainerControls/TileContainer_3_1.ascx", 3, "~/ContainerControls/TileContainer_3_1.ascx", 3));
		    }

			if (UserHasPermission(Permission.Folder_Staff_Evaluations))
		    {
		        Folders.Add(new Folder("Evaluations", "~/Images/new/folder_professional_dev.png", LoadEvalTiles,
		            "~/ContainerControls/TileContainer_3_1.ascx", 3, "~/ContainerControls/TileContainer_3_1.ascx", 3));
		    }

			if (SessionObject.LoggedInUser.HasPermission(Permission.Folder_Linking))
		    {
		        Folders.Add(new Folder("Linking", "~/Images/blank.png", LoadFolderLinking,
		            "~/ContainerControls/TileContainer_1.ascx", 1));
		    }

            //if (UserHasPermission(Permission.Folder_SysAdmin))
            //{
            //    Folders.Add(new Folder("Sys Admin", "~/Images/new/folder_profile.png", LoadAdministationTiles, "~/ContainerControls/TileContainer_1.ascx", 1));
            //}

            //if (true)
            //{
            //    Folders.Add(new Folder("System Admin", "~/Images/blank.png", LoadSystemAdminTiles,
            //        "~/ContainerControls/TileContainerDockSix.ascx", 6));
            //}

            //Verify Kentico is Enabled
            bool kenticoEnabled = (Session["KenticoEnabled"] != null) ? (bool)Session["KenticoEnabled"] : false;
            if (UserHasPermission(Permission.Folder_RTI_Forms) && kenticoEnabled)
                Folders.Add(new Folder("RTI", "~/Images/new/folder_professional_dev.png", LoadMtssTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));

			ctlFolders.BindFolderList(Folders);
		}

		#endregion

		#region Tile Methods

		private void LoadProfileTiles()
		{
			if (_selectedDistrict == null) return;

			var districtTileParms = new TileParms();
			districtTileParms.AddParm("district", _selectedDistrict);
            districtTileParms.AddParm("level", EntityTypes.District);
            districtTileParms.AddParm("levelID", _selectedDistrict.ID);

			Rotator1Tiles.Add(new Tile(Permission.Tile_Schools, "Schools", "~/Controls/School/SchoolResults.ascx", false, districtTileParms, null,
                (UserHasPermission(Permission.Icon_ExpandedSearch_Schools)) ? "../Controls/School/SchoolSearch_Expanded.aspx" : ""));

			Rotator1Tiles.Add(new Tile(Permission.Tile_Staff, "Staff", "~/Controls/Teacher/TeacherSearch.ascx", false, districtTileParms, null,
										 (UserHasPermission(Permission.Icon_ExpandedSearch_Staff)) ? "../Controls/Teacher/TeacherSearch_Expanded.aspx" : ""));
			
            Rotator1Tiles.Add(new Tile(Permission.Tile_Classes, "Classes", "~/Controls/Class/ClassSearch.ascx", false, districtTileParms, null,
										 (UserHasPermission(Permission.Icon_ExpandedSearch_Classes)) ? "../Controls/Class/ClassSearch_Expanded.aspx" : ""));

			var studentSearchUrl = "../Controls/Student/StudentSearch_Expanded.aspx?level=District&levelID=" + Standpoint.Core.Classes.Encryption.EncryptInt(_selectedDistrict.ID);
			Rotator1Tiles.Add(new Tile(Permission.Tile_Students, "Students", "~/Controls/Student/StudentSearch.ascx", false, districtTileParms, null,
										(UserHasPermission(Permission.Icon_ExpandedSearch_Students)) ? studentSearchUrl : "", "", false, "list"));


			Rotator1Tiles.Add(new Tile(Permission.Tile_Demographics_District, "Demographics", "~/Controls/PlaceholderTile.ascx"));
		}

		private void LoadPlanningTiles()
		{
			if (_selectedDistrict == null) return;

			var districtTileParms = new TileParms();
			districtTileParms.AddParm("level", EntityTypes.District);
			districtTileParms.AddParm("levelID", _selectedDistrict.ID);

		    Rotator1Tiles.Add(new Tile("Standards", "~/Controls/Standards/StandardsSearchDistSchool.ascx", false,
		        districtTileParms, null, "../Controls/Standards/StandardsSearch_ExpandedV2.aspx"));

		    Rotator1Tiles.Add(new Tile(Permission.Tile_Standards, "Standard Filter",
		        "~/Controls/Standards/StandardsFilter.ascx", false, null, null, null));

		    if ((bool)Session["KenticoEnabled"])
            {
                if (UserHasPermission(Permission.Tile_Competencylists)) // PBI 11435 
                {
                    var competencyListTileParms = new TileParms();
                    var title = "Competency Lists";
                    competencyListTileParms.AddParm("resourceToShow", "Thinkgate.CompetencyList");
                    competencyListTileParms.AddParm("title", title);
                    competencyListTileParms.AddParm("height", "500");
                    competencyListTileParms.AddParm("width", "900");
                    competencyListTileParms.AddParm("type", "CompetencyLists");
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, competencyListTileParms, null, null));
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
                Rotator1Tiles.Add(new Tile("School Improvement Plan", "~/Controls/ImprovementPlan/ImprovementPlanTile.ascx",
                    false, improvementPlanTileParms));
            }

            /*Student credential tile*/
            var StudentCredentialsTileParms = new TileParms();
             Rotator1Tiles.Add(new Tile(Permission.Tile_Credentials_Planning,
                                       "Credentials",
                                       "~/Controls/Credentials/CredentialsList.ascx",
                                       false,
                                       StudentCredentialsTileParms,
                                       null,
                                       null,
                                       (UserHasPermission(Permission.Icon_Edit_Credential) ? ResolveUrl("~/Controls/Credentials/") + "EditCredentials.aspx" : null),
                                       false,
                                       null,
                                       null));


		}

		private void LoadInstructionTiles()
		{
			if (_selectedDistrict == null) return;

			var districtTileParms = new TileParms();
			districtTileParms.AddParm("district", _selectedDistrict);

			if ((bool)Session["KenticoEnabled"])
			{
				if (UserHasPermission(Permission.Tile_InstructionalPlans))
				{
					var resourceTileParms2 = new TileParms();
					var title = GetInstructionalPlanTileTitle();
					resourceTileParms2.AddParm("resourceToShow", "thinkgate.InstructionalPlan");
					resourceTileParms2.AddParm("title", title);
					resourceTileParms2.AddParm("height", "500");
					resourceTileParms2.AddParm("width", "900");
					resourceTileParms2.AddParm("type", "InstructionalPlans");
                    resourceTileParms2.AddParm("typeKey", ((int)LookupDetail.CurriculumMap).ToString(CultureInfo.CurrentCulture));
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, resourceTileParms2, null, UserHasPermission(Permission.Icon_Expand_CurriculumMap) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)LookupDetail.CurriculumMap).ToString(CultureInfo.CurrentCulture) : null));
				}

				if (UserHasPermission(Permission.Tile_UnitPlans))
				{
					var resourceTileParms2 = new TileParms();
                    var title = GetUnitPlanTileTitle();
                    resourceTileParms2.AddParm("resourceToShow", "thinkgate.UnitPlan");
                    resourceTileParms2.AddParm("title", title);
					resourceTileParms2.AddParm("height", "500");
					resourceTileParms2.AddParm("width", "900");
					resourceTileParms2.AddParm("type", "UnitPlans");
                    resourceTileParms2.AddParm("typeKey", ((int)LookupDetail.UnitPlan).ToString(CultureInfo.CurrentCulture));
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, resourceTileParms2, null, UserHasPermission(Permission.Icon_Expand_Unitplan) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)LookupDetail.UnitPlan).ToString(CultureInfo.CurrentCulture) : null));
				}

				if (UserHasPermission(Permission.Tile_LessonPlansResources))
				{
					var resourceTileParms2 = new TileParms();
                    var title = GetLessonPlanTileTitle();
                    resourceTileParms2.AddParm("resourceToShow", "thinkgate.LessonPlan");
                    resourceTileParms2.AddParm("title", title);
					resourceTileParms2.AddParm("height", "500");
					resourceTileParms2.AddParm("width", "900");
					resourceTileParms2.AddParm("type", "LessonPlans");
                    resourceTileParms2.AddParm("typeKey", ((int)LookupDetail.LessonPlan).ToString(CultureInfo.CurrentCulture));
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, resourceTileParms2, null, UserHasPermission(Permission.Icon_Expand_Lessonplan) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)LookupDetail.LessonPlan).ToString(CultureInfo.CurrentCulture) : null));
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
                            modelCurriculumParms.AddParm("typeKey", ((int)LookupDetail.CurriculumUnitOH).ToString(CultureInfo.CurrentCulture));
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
                            modelCurriculumParms.AddParm("typeKey", ((int)LookupDetail.CurriculumUnitMA).ToString(CultureInfo.CurrentCulture));
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
                    resourceTileParms3.AddParm("typeKey", ((int)LookupDetail.Resource));
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, resourceTileParms3, null, UserHasPermission(Permission.Icon_Expand_Resources) ? "../Controls/Resources/ResourceSearchKentico.aspx" : null));
				}

                }
			else
			{
				TileParms unitPlanParms = new TileParms();
				unitPlanParms.AddParm("level", EntityTypes.District);
				unitPlanParms.AddParm("levelID", _selectedDistrict.ID);
				unitPlanParms.AddParm("type", "Unit Plans");
				Rotator1Tiles.Add(new Tile(Permission.Tile_UnitPlans, "Unit Plans", "~/Controls/Documents/Resources.ascx", false, unitPlanParms, null, "../Controls/Resources/ResourceSearch.aspx"));

				TileParms lessonPlanParms = new TileParms();
				lessonPlanParms.AddParm("level", EntityTypes.District);
				lessonPlanParms.AddParm("levelID", _selectedDistrict.ID);
				lessonPlanParms.AddParm("type", "Lesson Plans");
				Rotator1Tiles.Add(new Tile(Permission.Tile_LessonPlansResources, "Lesson Plans", "~/Controls/Documents/Resources.ascx", false, lessonPlanParms, null, "../Controls/Resources/ResourceSearch.aspx"));

				TileParms pacingDocumentsParms = new TileParms();
				pacingDocumentsParms.AddParm("level", EntityTypes.District);
				pacingDocumentsParms.AddParm("levelID", _selectedDistrict.ID);
				pacingDocumentsParms.AddParm("type", "Pacing Documents");
				Rotator1Tiles.Add(new Tile(Permission.Tile_PacingDocuments, "Pacing Documents", "~/Controls/Documents/Resources.ascx", false, pacingDocumentsParms, null, "../Controls/Resources/ResourceSearch.aspx"));

			}

			if (UserHasPermission(Permission.Tile_Playbook))
			{
				Rotator1Tiles.Add(new Tile("Playbooks", "~/Controls/Plans/Playbook.ascx", false, null));
			}
			if (DistrictParms.Tile_E3_Resources == "Yes")
			{
				TileParms plansParms = new TileParms();
				plansParms.AddParm("level", EntityTypes.District);
				plansParms.AddParm("levelID", _selectedDistrict.ID);
				plansParms.AddParm("type", "Resources");
				var resourcesTileFilterToDisplay = DistrictParms.LoadDistrictParms().ResourcesTileFilterToDisplay;
				if (!string.IsNullOrEmpty(resourcesTileFilterToDisplay)) plansParms.AddParm("UseResourcesTileFilterToDisplay", resourcesTileFilterToDisplay);
				var resourceTileExpand = UserHasPermission(Permission.Icon_Expand_Resources) ? "../Controls/Resources/ResourceSearch.aspx" : null;
				Rotator1Tiles.Add(new Tile(Permission.Tile_Resources, "Resources", "~/Controls/Documents/Resources.ascx", false, plansParms, null, resourceTileExpand));
			}

		}

		private void LoadAssessmentTiles()
		{
			if (_selectedDistrict == null) return;

			var districtTileParms = new TileParms();
			districtTileParms.AddParm("district", _selectedDistrict);

			TileParms stateAssessmentParms = new TileParms();
			TileParms districtAssessmentParms = new TileParms();
			TileParms classroomAssessmentParms = new TileParms();
			TileParms blueprintAssessmentParms = new TileParms();

			stateAssessmentParms.AddParm("level", EntityTypes.District);
			stateAssessmentParms.AddParm("levelID", _selectedDistrict.ID);
			stateAssessmentParms.AddParm("category", "State");

			districtAssessmentParms.AddParm("level", EntityTypes.District);
			districtAssessmentParms.AddParm("levelID", _selectedDistrict.ID);
			districtAssessmentParms.AddParm("category", "District");

			SessionObject.DistrictTileParms = districtAssessmentParms;

			classroomAssessmentParms.AddParm("level", EntityTypes.District);
			classroomAssessmentParms.AddParm("levelID", _selectedDistrict.ID);
			classroomAssessmentParms.AddParm("category", "Classroom");

			blueprintAssessmentParms.AddParm("level", EntityTypes.District);
			blueprintAssessmentParms.AddParm("levelID", _selectedDistrict.ID);
			blueprintAssessmentParms.AddParm("category", "Blueprint");

			Rotator1Tiles.Add(new Tile(Permission.Tile_StateAssessments, "State Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, stateAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=State&level=District", null));
        
            Rotator1Tiles.Add(new Tile(Permission.Tile_DistrictAssessment, "District Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, districtAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=District&level=District", null,false,null,null,null,null,null,null,null, "../Controls/Assessment/SetPerformanceLevelList.aspx"));

			Rotator1Tiles.Add(new Tile(Permission.Tile_ClassroomAssessment, "Classroom Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, classroomAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=Classroom&level=District", null));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Items, "Items", "~/Controls/Items/Items.ascx", false, districtTileParms, null, UserHasPermission(Permission.Search_Item) ? "../Controls/Items/ItemSearch.aspx" : null, null, false, null, null,  "assessmentItemsExportToExcel"));


			Rotator1Tiles.Add(new Tile(Permission.Tile_Images, "Images", "~/Controls/Assessment/AssessmentItems.ascx", false, districtTileParms, null, UserHasPermission(Permission.Search_Image) ? "../Controls/Images/ImageSearch_ExpandedV2.aspx" : null));
			Rotator1Tiles.Add(new Tile(Permission.Tile_Addendum, "Addendums", "~/Controls/Assessment/AssessmentItems.ascx", false, districtTileParms, null, UserHasPermission(Permission.Search_Addendum) ? "../Controls/Addendums/AddendumSearch_ExpandedV2.aspx" : null));
			Rotator1Tiles.Add(new Tile(Permission.Tile_Rubrics, "Rubrics", "~/Controls/Assessment/AssessmentItems.ascx", false, districtTileParms, null, UserHasPermission(Permission.Search_Rubric) ? "../Controls/Rubrics/RubricSearch_ExpandedV2.aspx" : null));

			Rotator1Tiles.Add(new Tile(Permission.Tile_Blueprint, "Blueprints", "~/Controls/Assessment/ViewBlueprints.ascx", false, blueprintAssessmentParms, null,null, string.Empty, false, "graphical"));
		}

		private void LoadReportingTiles()
		{
			if (_selectedDistrict == null) return;

			var districtTileParms = new TileParms();
			districtTileParms.AddParm("level", EntityTypes.District);
			districtTileParms.AddParm("levelID", _selectedDistrict.ID);
			districtTileParms.AddParm("selectID", _selectedDistrict.ID);
			districtTileParms.AddParm("district", _selectedDistrict);
			districtTileParms.AddParm("folder", "Reporting");

			Rotator1Tiles.Add(new Tile(Permission.Tile_AssessmentResults, "Assessment Results", "~/Controls/Assessment/AssessmentResults.ascx", false, districtTileParms, null, "../Controls/Assessment/AssessmentResults_ExpandedV2.aspx", null, false, null, "expandAssessmentResults"));
			Rotator1Tiles.Add(new Tile(Permission.Tile_AdvancedReporting, "Advanced Reporting", "~/Controls/Reports/AdvancedReporting.ascx", false, districtTileParms, null, null, null, false, null, null));

            var edwinAnalyticsReports = new TileParms();
            edwinAnalyticsReports.AddParm("folder", "Reporting");
		    Rotator1Tiles.Add(new Tile(Permission.Tile_ExternalLinks_EdwinAnalytics, "Select Edwin Analytics Reports",
		        "~/Controls/Reports/EdwinAnalyticsReport.ascx", false, edwinAnalyticsReports, null, null, null, false, null,
		        null));

		    var archivedReportingTileParms = new TileParms();
			archivedReportingTileParms.AddParm("archives", "yes");
			Rotator1Tiles.Add(new Tile(Permission.Tile_ArchivedReporting, "Archived Reporting", "~/Controls/Reports/AdvancedReporting.ascx", false, archivedReportingTileParms));
#if false
						_rotator1Tiles.Add(new Tile("Proficiency Analysis", "~/Controls/District/DistrictProficiencyAnalysis.ascx", false, districtTileParms));
						_rotator1Tiles.Add(new Tile("Report Builder", "~/Controls/District/DistrictReportBuilder.ascx", false, districtTileParms));
						_rotator1Tiles.Add(new Tile("Report Engine", "~/Controls/District/DistrictReportEngine.ascx", false, districtTileParms));
						_rotator1Tiles.Add(new Tile("State Analysis", "~/Controls/District/DistrictStateAnalysis.ascx", false, districtTileParms));
						_rotator1Tiles.Add(new Tile("Results", "~/Controls/District/DistrictResults.ascx", false, districtTileParms));
						_rotator1Tiles.Add(new Tile("Early Warning", "~/Controls/District/DistrictEarlyWarning.ascx", false, districtTileParms));
						_rotator1Tiles.Add(new Tile("Readiness", "~/Controls/District/DistrictReadiness.ascx", false, districtTileParms));
#endif
		}

		private void LoadPdTiles()
		{
		    if (_selectedDistrict != null)
		    {
		        
		    }
		}

		private void LoadEvalTiles()
		{
			if (_selectedDistrict == null)
				return;

			TileParms districtTileParmsCi = new TileParms();
			districtTileParmsCi.AddParm("level", EntityTypes.District);
			districtTileParmsCi.AddParm("levelID", _selectedDistrict.ID);
			districtTileParmsCi.AddParm("evalType", EvaluationTypes.TeacherClassroom);

			TileParms districtTileParmsNci = new TileParms();
			districtTileParmsNci.AddParm("level", EntityTypes.District);
			districtTileParmsNci.AddParm("levelID", _selectedDistrict.ID);
			districtTileParmsNci.AddParm("evalType", EvaluationTypes.TeacherNonClassroom);

			TileParms districtTileParmsSa = new TileParms();
			districtTileParmsSa.AddParm("level", EntityTypes.District);
			districtTileParmsSa.AddParm("levelID", _selectedDistrict.ID);
			districtTileParmsSa.AddParm("evalType", EvaluationTypes.Administrator);


			if (UserHasPermission(Permission.Tile_ClassroomInstructionalStaff))
				Rotator1Tiles.Add(new Tile("Classroom Instructional Staff", "~/Controls/Staff/StaffEvaluationSearch.ascx", false, districtTileParmsCi));
			if (UserHasPermission(Permission.Tile_NonClassroomInstructionalStaff))
				Rotator1Tiles.Add(new Tile("Non-Classroom Instructional Staff", "~/Controls/Staff/StaffEvaluationSearch.ascx", false, districtTileParmsNci));
			if (UserHasPermission(Permission.Tile_SchoolBasedAdministrativeStaff))
				Rotator1Tiles.Add(new Tile("School-Based Administrative Staff", "~/Controls/Staff/StaffEvaluationSearch.ascx", false, districtTileParmsSa));
		}

        private void LoadMtssTiles()
        {
            if (_selectedDistrict == null) return;
            var districtTileParms = new TileParms();
            districtTileParms.AddParm("district", _selectedDistrict);

            var resourceTileParms1 = new TileParms();
            var title1 = GetRtiFormTileTitle();
            resourceTileParms1.AddParm("resourceToShow", "thinkgate.MTSSForms");
            resourceTileParms1.AddParm("title", title1);
            resourceTileParms1.AddParm("height", "500");
            resourceTileParms1.AddParm("width", "900");
            resourceTileParms1.AddParm("type", "MTSSForms");
            Rotator1Tiles.Add(new Tile(Permission.Tile_RTIForms_DistrictPortal, title1, "~/Controls/Documents/MTSSFormsDocumentTile.ascx", false, resourceTileParms1));

            var resourceTileParms2 = new TileParms();
            var title2 = GetRtiAnalysisTileTitle();
            resourceTileParms2.AddParm("resourceToShow", "thinkgate.MTSSAnalysis");
            resourceTileParms2.AddParm("title", title2);
            resourceTileParms2.AddParm("height", "500");
            resourceTileParms2.AddParm("width", "900");
            resourceTileParms2.AddParm("type", "MTSSAnalysis");
            resourceTileParms2.AddParm("isFormColumnEnabled", UserHasPermission(Permission.Tile_RTIForms_DistrictPortal));
            var link = ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + System.Web.HttpUtility.UrlEncode("fastsql_v2_direct.asp?ID=7274|Testgate_ViewRoster&??x=find");
            Rotator1Tiles.Add(new Tile(Permission.Tile_RTIAnalysis_DistrictPortal, title2, "~/Controls/Documents/MTSSAnalysisDocumentTile.ascx", false, resourceTileParms2, null, link, null, false, null, "OpenLegacyRTISearch"));

            var resourceTileParms3 = new TileParms();
            var title3 = GetRtiInterventionsTileTitle();
            resourceTileParms3.AddParm("resourceToShow", "thinkgate.MTSSInterventions");
            resourceTileParms3.AddParm("title", title3);
            resourceTileParms3.AddParm("height", "500");
            resourceTileParms3.AddParm("width", "900");
            resourceTileParms3.AddParm("type", "MTSSInterventions");
            resourceTileParms3.AddParm("isFormColumnEnabled", UserHasPermission(Permission.Tile_RTIForms_DistrictPortal));
            Rotator1Tiles.Add(new Tile(Permission.Tile_RTIInterventions_DistrictPortal, title3, "~/Controls/Documents/MTSSInterventionsDocumentTile.ascx", false, resourceTileParms3, null, link, null, false, null, "OpenLegacyRTISearch"));
        }

        private void LoadFolderLinking()
        {
            Rotator1Tiles.Add(new Tile(Permission.Folder_Linking, "Groups", "~/Controls/Groups/GroupSingleUser.ascx"));
        }

	    private void LoadSystemAdminTiles()
	    {
            Rotator1Tiles.Add(new Tile("Approvers", "~/Controls/SystemAdmin/ApproversTile.ascx"));

            Rotator1Tiles.Add(new Tile("Graders Queue Management", "~/Controls/PlaceholderTile.ascx"));

            Rotator1Tiles.Add(new Tile("Approvers Queue Management", "~/Controls/PlaceholderTile.ascx"));

            Rotator1Tiles.Add(new Tile("SLA", "~/Controls/PlaceholderTile.ascx"));

            Rotator1Tiles.Add(new Tile("System Admin Reports", "~/Controls/PlaceholderTile.ascx"));
	    }

        private void LoadAdministationTiles()
        {
            //if (_selectedDistrict == null) return;

            //var districtTileParms = new TileParms();
            //districtTileParms.AddParm("level", EntityTypes.District);
            //districtTileParms.AddParm("levelID", _selectedDistrict.ID);
            //districtTileParms.AddParm("selectID", _selectedDistrict.ID);
            //districtTileParms.AddParm("district", _selectedDistrict);
            //districtTileParms.AddParm("folder", "Administation");

            //Rotator1Tiles.Add(new Tile(Permission.Tile_ParentStudentPortal, "Parent / Student Portal", "~/Controls/ParentPortalAdministration/ParentStudentPortalAdministration.ascx", false, districtTileParms, null, null, null, false, null, null));

        }

		#endregion

        #region Overridden Methods

        protected override object LoadRecord(int xId)
	    {
            return Base.Classes.District.GetDistrictByID(xId);
        }

        #endregion
    }
}
