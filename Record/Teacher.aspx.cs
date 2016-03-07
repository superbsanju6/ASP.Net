using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using Thinkgate.Services.Contracts.ImprovementPlanService;

namespace Thinkgate.Record
{
	public partial class Teacher : RecordPage
	{
        #region Variables

        private int _teacherId;
		private Base.Classes.Teacher _selectedTeacher;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.Teacher + "_"; }
        }

        protected bool IsCalledbyUpperCarousel
        {
            get;
            set;
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
			SessionObject.CurrentPortal = EntityTypes.Teacher;
			SetupFolders();
			InitPage(ctlFolders, ctlDoublePanel, sender, e);
			LoadTeacher();
			if (_selectedTeacher == null) return;
			LoadDefaultFolderTiles();
            SetHeaderImageUrl();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
            System.Web.UI.ScriptManager _scriptMan = System.Web.UI.ScriptManager.GetCurrent(this.Page);
            _scriptMan.AsyncPostBackTimeout = 36000;
            if (IsPostBack && Request["__EVENTTARGET"].Contains(doubleRotatorPanel.ClientID))
            {
                var eventArg = Request["__EVENTARGUMENT"];
                if (eventArg.Contains("~") && eventArg.Contains(":"))
                {
                    IsCalledbyUpperCarousel = true;
                    var dockID = eventArg.Substring(0, eventArg.IndexOf("~"));
                    SessionObject.TileClicked = dockID;
                    SessionObject.LastElementsFolder_TileClicked = SessionObject.Elements_ActiveFolder;

                    Group objGroup = new Group();
                    objGroup.ID = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(eventArg.Substring(eventArg.IndexOf("~") + 1, ((eventArg.IndexOf(":")-8)-eventArg.IndexOf("~"))-1 ));
                    objGroup.GroupName = eventArg.Substring(eventArg.IndexOf(":") + 1);
                    SessionObject.clickedGroup = objGroup;
                    ReloadTilesControl("Groups");
                }
                else
                {
                    var dockID = eventArg.Substring(0, eventArg.IndexOf("~"));
                    SessionObject.TileClicked = dockID;
                    SessionObject.LastElementsFolder_TileClicked = SessionObject.Elements_ActiveFolder;

                    var classID =
                            Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(
                                    eventArg.Substring(eventArg.IndexOf("~") + 1));
                    SessionObject.clickedClass = _selectedTeacher.Classes.FindLast(c => c.ID == classID); //Find By ID and set
                    ReloadTilesControl("Classes");
                }
            }
            else
            {
                try
                {
                    if (!String.IsNullOrEmpty(SessionObject.CheckNewWorksheetCreated))
                    {                        
                        if (SessionObject.CheckNewWorksheetCreated == "1")
                        {
                            var dockID = "rotator1_container2_tileContainerDiv1";
                            SessionObject.TileClicked = dockID;
                            SessionObject.LastElementsFolder_TileClicked = SessionObject.Elements_ActiveFolder;

                            var classID = SessionObject.clickedClass.ID;
                            SessionObject.clickedClass = _selectedTeacher.Classes.FindLast(c => c.ID == classID); //Find By ID and set
                            ReloadTilesControl("Classes");
                        }
                    }
                }
                catch { }
            }
		}

        protected void Page_LoadComplete()
        {
            var siteMaster = this.Master as SiteMaster;
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
                teacherImage.ImageUrl = "~/Images/ISLE_logo1.png";
            }
            else
            {
                teacherImage.ImageUrl = "~/Images/new/female_teacher.png";
            }
        }

		private void LoadTeacher()
		{
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
			else
			{
                _teacherId = GetDecryptedEntityId(X_ID);
                SessionObject.TeacherTileParms.AddParm("userID", _teacherId);
                
				if (!RecordExistsInCache(Key))
				{
					return;
				}

                _selectedTeacher = (Base.Classes.Teacher)Base.Classes.Cache.Get(Key);
				teacherName.Text = _selectedTeacher.FormattedFirstName + " " + _selectedTeacher.FormattedLastName;

				if (!string.IsNullOrEmpty(_selectedTeacher.Picture))
				{
					teacherImage.ImageUrl = AppSettings.ProfileImageUserWebPath + '/' + _selectedTeacher.Picture;
				}
			}
		}

		#region Folder Methods

		private void SetupFolders()
		{
			Folders = new List<Folder>();

			if (UserHasPermission(Base.Enums.Permission.Folder_Classes))
			{
                SessionObject.DefaultEmptyMessage=string.Empty;                
				Folders.Add(new Folder("Classes", "~/Images/new/folder_classes.png", LoadClassesTiles,
															 "~/ContainerControls/TileContainer_3_1.ascx", 3,
															 "~/ContainerControls/TileContainer_3_1.ascx", 3, "Classes",
															 "Class Information"));
			}

			if (UserHasPermission(Permission.Folder_Profile)) Folders.Add(new Folder("Profile", "~/Images/new/folder_profile.png", LoadProfileTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));
			//Folders.Add(new Folder("History", "~/Images/new/folder_history.png", LoadHistoryTiles, "~/ContainerControls/TileContainer_3_1.ascx", 3, "~/ContainerControls/TileContainer_3_1.ascx", 3));

			if (UserHasPermission(Base.Enums.Permission.Folder_Curriculum))
			{
				Folders.Add(new Folder("Planning", "~/Images/new/folder_curriculum.png", LoadPlanningTiles,
															 "~/ContainerControls/TileContainerDockSix.ascx", 6));
			}

			if (UserHasPermission(Permission.Folder_Instruction)) Folders.Add(new Folder("Instruction", "~/Images/new/folder_instruction.png", LoadInstructionTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));
			if (UserHasPermission(Permission.Folder_Assessment)) Folders.Add(new Folder("Assessment", "~/Images/new/folder_assessment.png", LoadAssessmentTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));

			if (SessionObject.LoggedInUser.HasPermission(Thinkgate.Base.Enums.Permission.Folder_Reporting))
				Folders.Add(new Folder("Reporting", "~/Images/new/folder_data_analysis.png", LoadDataAnalysisTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));

			if (SessionObject.LoggedInUser.HasPermission(Permission.Folder_Linking))
                Folders.Add(new Folder("Groups", "~/Images/new/folder_classes.png", LoadFolderLinking,
                                                             "~/ContainerControls/TileContainer_3_1.ascx", 3,
                                                             "~/ContainerControls/TileContainer_3_1.ascx", 3, "Groups",
                                                             "Group Information"));

            //Verify Kentico is Enabled
            bool KenticoEnabled = (Session["KenticoEnabled"] != null) ? (bool)Session["KenticoEnabled"] : false;
            if (UserHasPermission(Permission.Folder_RTI_Forms) && KenticoEnabled)
                Folders.Add(new Folder("RTI", "~/Images/new/folder_professional_dev.png", LoadMTSSTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));

			ctlFolders.BindFolderList(Folders);
		}

		#endregion

		#region Tile Methods

		private void LoadFolderLinking()
		{
            SessionObject.clickedClass = null;
             Thinkgate.Services.Contracts.Groups.GroupsProxy _groupProxy = new Thinkgate.Services.Contracts.Groups.GroupsProxy();
             List<Thinkgate.Services.Contracts.Groups.GroupDataContract> _groupsWithStudents = _groupProxy.GetGroupsForUser(SessionObject.LoggedInUser.Page,
                     DistrictParms.LoadDistrictParms().ClientID);

             int counter = 1;
             int totalCount = _groupsWithStudents.Count();
            Rotator1Tiles.Add(new Tile(Permission.Folder_Linking, "Groups", "~/Controls/Groups/GroupSingleUser.ascx"));
            foreach (var g in _groupsWithStudents.OrderBy(x => x.ID))
            {
                if (true /*UserHasPermission(Base.Enums.Permission.Tile_ClassSummary)*/)                {
                   
                    var tileParms = new TileParms();
                    tileParms.AddParm("group", g);
                    const string controlPath = "~/Controls/Groups/GroupSummary.ascx";
                    var encryptedClassID = Standpoint.Core.Classes.Encryption.EncryptInt(g.ID);
                    string name = g.Name.Replace("'", "&#39;");
                    var title = "<div class='selectableClassTile' title='" + name + "' onclick='javascript: __doPostBack(\"" + doubleRotatorPanel.ClientID + "\",\"" + "@@dockID~" + g.ID + "@@folder:" + name + "\")'>" + name + "</div>";
                    var tile = new Tile(title, controlPath, true, tileParms, null, null, null, true, "graphical");                    
                    Rotator1Tiles.Add(tile);
                    if ((counter % 2) == 0 && (counter + 1) <= totalCount)
                    {
			Rotator1Tiles.Add(new Tile(Permission.Folder_Linking, "Groups", "~/Controls/Groups/GroupSingleUser.ascx"));
		}
                }
                counter += 1;
            }

            (ctlDoublePanel.GetButtonsContainer2()).CssClass = "pagingDivTall";
            ctlDoublePanel.ResetPageOnPostBack("1");
            Thinkgate.Base.Classes.Group selectedGroup = SessionObject.clickedGroup;

            string urlReferrer = "";
            if (Request != null && Request.UrlReferrer != null)
            {
                urlReferrer = string.IsNullOrEmpty(Request.UrlReferrer.ToString()) ? "" : Request.UrlReferrer.ToString();
            }
            try
            {
                if (!urlReferrer.Contains("Teacher.aspx") || selectedGroup == null)
                {
                    if (UserHasPermission(Base.Enums.Permission.Tile_ClassSummary))
                    {
                        Panel buttonDiv2 = ctlDoublePanel.GetButtonsContainer2();

                        SessionObject.DefaultEmptyMessage = "Select the group link above to display details here.";
                        LoadContainer(ctlDoublePanel, 2, "~/ContainerControls/TileContainer_3_1_Empty.ascx", null, 0, 0);                        
                        buttonDiv2.CssClass = "pagingDivTallHidden";
                        Session.Remove("tileClicked");
                        Session.Remove("selectedRDTitleBarClass");
                    }
                }
                else
                {
                    (ctlDoublePanel.GetButtonsContainer2()).CssClass = "pagingDivTall";                                                 

                    if ((bool)Session["KenticoEnabled"] == true)
                    {
                        if (UserHasPermission(Permission.Tile_InstructionAssignments) && totalCount>0)
                        {
                            #region To be replaced with Instruction material code. 
                            var tileParms = new TileParms();
                            tileParms.AddParm("groupID", selectedGroup.ID);
                            //tileParms.AddParm("selectedTeacher", _selectedTeacher);
                            const string controlPath = "~/Controls/InstructionMaterial/InstructionAssignment.ascx";                           
                            var title = "Instruction Assignments";
                            var tile = new Tile(title, controlPath, false, tileParms, null, null, null, false);
                            Rotator2Tiles.Add(tile); 
                            #endregion
                            
                        }
		}

 
                }
            }
            catch
            {
            }



		}

		private void LoadClassesTiles()
		{
            var roleportalID = (RolePortal)(SessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection));
            if (roleportalID == RolePortal.District)
            {
                var districtUserAssessmentParms = new TileParms();
                districtUserAssessmentParms.AddParm("level", EntityTypes.District);
                districtUserAssessmentParms.AddParm("levelID", SessionObject.LoggedInUser.District);
                districtUserAssessmentParms.AddParm("category", "District");

                SessionObject.DistrictTileParms = districtUserAssessmentParms;
            }

            SessionObject.clickedGroup = null;
			if (_selectedTeacher == null) return;
			foreach (var c in _selectedTeacher.Classes.OrderBy(x => x.Period).ThenBy(n => n.Grade, new GradeComparer()))
			{
				if (UserHasPermission(Base.Enums.Permission.Tile_ClassSummary))
				{
					var tileParms = new TileParms();
					tileParms.AddParm("class", c);
					const string controlPath = "~/Controls/Class/ClassSummary.ascx";
					var encryptedClassID = Standpoint.Core.Classes.Encryption.EncryptInt(c.ID);
					var expandUrl = UserHasPermission(Base.Enums.Permission.Icon_Expand_Class) ? "../Controls/Class/ClassSummary_Expanded.aspx?xID=" + encryptedClassID : string.Empty;
					var editURL = UserHasPermission(Base.Enums.Permission.Edit_Class) ? "../Controls/Class/ClassSummary_Edit.aspx?xID=" + encryptedClassID : string.Empty;
					var title = "<div class='selectableClassTile' title='" + c.GetClassToolTip() + "' onclick='javascript: __doPostBack(\"" + doubleRotatorPanel.ClientID + "\",\"" + "@@dockID~" + c.ID + "\")'>" + c.GetFriendlyName() + "</div>";
					var tile = new Tile(title, controlPath, true, tileParms, null, expandUrl, editURL, true, "graphical");

					Rotator1Tiles.Add(tile);
				}
			}

			Thinkgate.Base.Classes.Class selectedClass = SessionObject.clickedClass;

			string urlReferrer = "";
			if (Request != null && Request.UrlReferrer != null)
			{
				urlReferrer = string.IsNullOrEmpty(Request.UrlReferrer.ToString()) ? "" : Request.UrlReferrer.ToString();
			}
            try
            {
                if ((!urlReferrer.Contains("Teacher.aspx") || selectedClass == null) && SessionObject.CheckNewWorksheetCreated != "1")
                {
                    if (UserHasPermission(Base.Enums.Permission.Tile_ClassSummary))
                    {
                        Panel buttonDiv2 = ctlDoublePanel.GetButtonsContainer2();

                        LoadContainer(ctlDoublePanel, 2, "~/ContainerControls/TileContainer_3_1_Empty.ascx", null, 0, 0);

                        //div.InnerHtml = "<div class='rotator2InitTxt'>Use <div class='lowerCarouselMsgIcon'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div> to select a class above and display details here.</div>";
                        buttonDiv2.CssClass = "pagingDivTallHidden";

                        Session.Remove("tileClicked");
                        Session.Remove("selectedRDTitleBarClass");
                    }
                }
                else
                {
                    SessionObject.CheckNewWorksheetCreated = "0";

                    (ctlDoublePanel.GetButtonsContainer2()).CssClass = "pagingDivTall";
                    ctlDoublePanel.ResetPageOnPostBack("1");

                    TileParms districtAssessmentParms = new TileParms();
                    districtAssessmentParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Class);
                    districtAssessmentParms.AddParm("category", "District");
                    districtAssessmentParms.AddParm("userID", _selectedTeacher.PersonID);
                    districtAssessmentParms.AddParm("levelID", selectedClass.ID);
                    Rotator2Tiles.Add(new Tile(Permission.Tile_DistrictAssessment, "District Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, districtAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=District&level=Teacher", null));

                    TileParms classroomAssessmentParms = new TileParms();
                    classroomAssessmentParms.AddParm("class", selectedClass);
                    classroomAssessmentParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Class);
                    classroomAssessmentParms.AddParm("category", "Classroom");
                    classroomAssessmentParms.AddParm("userID", _selectedTeacher.PersonID);
                    classroomAssessmentParms.AddParm("levelID", selectedClass.ID);
                    classroomAssessmentParms.AddParm("folder", "Classes");

                    SessionObject.TeacherTileParms = classroomAssessmentParms;

                    Rotator2Tiles.Add(new Tile(Permission.Tile_ClassroomAssessment, "Classroom Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, classroomAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=Classroom&level=Teacher", null));

                    var classTileParms = new TileParms();
                    classTileParms.AddParm("class", selectedClass);
                    classTileParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
                    classTileParms.AddParm("levelID", _selectedTeacher.PersonID);
                    classTileParms.AddParm("selectID", selectedClass.ID);
                    classTileParms.AddParm("category", "Classroom");
                    classTileParms.AddParm("folder", "Classes");

                    Rotator2Tiles.Add(new Tile(Permission.Tile_AssessmentResults, "Assessment Results", "~/Controls/Assessment/AssessmentResults.ascx", false, classroomAssessmentParms, null, "../Controls/Assessment/AssessmentResults_ExpandedV2.aspx", null, false, null, "expandAssessmentResults"));

                    if (UserHasPermission(Base.Enums.Permission.Tile_Competencyworksheet)) // PBI 12283 
                    {
                        var CompetencyWSheetTileParms = new TileParms();
                        //var resourceTileParms2 = new TileParms();
                        string title = "Competency Worksheets";
                        CompetencyWSheetTileParms.AddParm("resourceToShow", "Competency Worksheets");
                        CompetencyWSheetTileParms.AddParm("title", title);
                        CompetencyWSheetTileParms.AddParm("height", "500");
                        CompetencyWSheetTileParms.AddParm("width", "900");
                        CompetencyWSheetTileParms.AddParm("class", selectedClass);
                        CompetencyWSheetTileParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Class);
                        CompetencyWSheetTileParms.AddParm("category", "Classroom");
                        CompetencyWSheetTileParms.AddParm("userID", _selectedTeacher.PersonID);
                        CompetencyWSheetTileParms.AddParm("levelID", selectedClass.ID);
                        CompetencyWSheetTileParms.AddParm("folder", "Classes");
                        Rotator2Tiles.Add(new Tile(title, "~/Controls/CompetencyWorksheet/Competencyworksheetsearch.ascx", false, CompetencyWSheetTileParms, null, null));
                    }

                    if (UserHasPermission(Base.Enums.Permission.Tile_Standards))
                    {
                        Rotator2Tiles.Add(new Tile("Standards", "~/Controls/Standards/StandardsSearch.ascx",
                                                                                false, classTileParms, null,
                                                                                "../Controls/Standards/StandardsSearch_ExpandedV2.aspx"));
                    }

                    //_rotator2Tiles.Add(new Tile(Thinkgate.Base.Enums.Permission.Tile_ClassResources, "Class Resources", "~/Controls/Documents/Resources.ascx", false, classroomAssessmentParms, null, "../Controls/Resources/ResourceSearch.aspx"));


                    DistrictParms districtParms = DistrictParms.LoadDistrictParms();
                    var resourceTileParms3 = new TileParms();
                    resourceTileParms3.AddParm("resourceForFolder", "Teacher");
                    resourceTileParms3.AddParm("resourceToShow", GetKenticoResourceTileResourceTypeName());
                    resourceTileParms3.AddParm("title", "Resources");
                    resourceTileParms3.AddParm("height", "500");
                    resourceTileParms3.AddParm("width", "900");
                    resourceTileParms3.AddParm("type", "Resources");
                    resourceTileParms3.AddParm("CallingByUperCarousel", IsCalledbyUpperCarousel);
                    Rotator2Tiles.Add(new Tile(Thinkgate.Base.Enums.Permission.Tile_ClassResources, "Class Resources", "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, resourceTileParms3, null, "../Controls/Resources/ResourceSearchKentico.aspx"));

                    if(UserHasPermission(Permission.Tile_InstructionAssignments))
                    { 
                        var instructionAssignmentTile = new TileParms();
                        instructionAssignmentTile.AddParm("resourceForFolder", "Teacher");
                        instructionAssignmentTile.AddParm("classID", selectedClass.ID);
                        instructionAssignmentTile.AddParm("resourceToShow", "ALL");
                        instructionAssignmentTile.AddParm("title", "Instruction Assignments");
                        instructionAssignmentTile.AddParm("height", "500");
                        instructionAssignmentTile.AddParm("width", "900");
                        instructionAssignmentTile.AddParm("type", "All");
                        instructionAssignmentTile.AddParm("selectedTeacher", _selectedTeacher);
                        Rotator2Tiles.Add(new Tile("Instruction Assignments", "~/Controls/InstructionMaterial/InstructionAssignment.ascx", false, instructionAssignmentTile, null, null));
                    }

                    Rotator2Tiles.Add(new Tile(Permission.Tile_ClassGrades, "Class Grades", "~/Controls/Class/ClassGrades.ascx", false, classroomAssessmentParms));
                    Rotator2Tiles.Add(new Tile(Permission.Tile_ClassEarlyWarning, "Class Early Warning", "~/Controls/Class/ClassEarlyWarning.ascx"));
                    Rotator2Tiles.Add(new Tile(Permission.Tile_Class_Readiness, "Class Readiness", "~/Controls/Class/ClassReadiness.ascx", false, classroomAssessmentParms));
                }
            }
            catch
            {
            }

		}

		private void LoadProfileTiles()
		{
			var teacherTileParms = new TileParms();
			var searchTileParms = new TileParms();
			teacherTileParms.AddParm("teacher", _selectedTeacher);
			searchTileParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
			searchTileParms.AddParm("levelID", _selectedTeacher.PersonID);
			searchTileParms.AddParm("ctlDoublePanel", ctlDoublePanel);

			var editTeacherURL = "../Controls/Staff/StaffIdentification_Edit.aspx?xID=" +
								 Standpoint.Core.Classes.Encryption.EncryptInt(_selectedTeacher.PersonID) +
								 "&type=teacher";
			Rotator1Tiles.Add(new Tile(Permission.Tile_Identification_User,
											"Identification",
											"~/Controls/Teacher/TeacherIdentification.ascx",
											false,
											teacherTileParms,
											null,
											null,
											(UserHasPermission(Permission.Icon_Edit_Teacher_Identification)) ? editTeacherURL : null,
                                            editJSFunctionOverride : "openTeacherStaffIdentificationEditRadWindow"
										)
							   );
			Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_Certifications, "Certifications", "~/Controls/PlaceholderTile.ascx"));

			//TODO:DHB 2012-08-30 The IsClassroomTeacher_Evaluation is currently hardcoded to true.  However, it is anticipated that down the road, it will be used to determine when a non classroom instructor (NCI) is still to be considered a teacher.  See email from HCB on 8/30/2012.
			if (
					(
					(SessionObject.LoggedInUser.IsClassroomTeacher_Evaluation || SessionObject.LoggedInUser.IsSchoolAdministrator_Evaluation) &&
					  UserHasPermission(Permission.Tile_Evaluations_SchoolTeacherCI)

					 ||

					 (SessionObject.LoggedInUser.IsNonClassroomTeacher_Evaluation || SessionObject.LoggedInUser.IsSchoolAdministrator_Evaluation) &&
					  UserHasPermission(Permission.Tile_Evaluations_SchoolTeacherNCI))

					//******* DHB 2012-08-30 - Per Patti, there are situations where an administrator should be able to see a teacher's evaluation so the following logic is to be commented out.
				//&& 
				//SessionObject.LoggedInUser.Page == _teacherID
				)
			{
				TileParms evalTileParms = new TileParms();
				if (SessionObject.LoggedInUser.IsClassroomTeacher_Evaluation) evalTileParms.AddParm("staffType", "CI");
				else if (SessionObject.LoggedInUser.IsNonClassroomTeacher_Evaluation) evalTileParms.AddParm("staffType", "NCI");
				else if (SessionObject.LoggedInUser.IsSchoolAdministrator_Evaluation) evalTileParms.AddParm("staffType", "SA");

				Rotator1Tiles.Add(new Tile("Personal Evaluations", "~/Controls/Staff/StaffEvaluation.ascx", false, evalTileParms));
			}

			Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_PerformanceHistory, "Performance History", "~/Controls/PlaceholderTile.ascx"));

			var studentSearchURL = "../Controls/Student/StudentSearch_Expanded.aspx?level=Teacher&levelID=" + Standpoint.Core.Classes.Encryption.EncryptInt(_selectedTeacher.PersonID);
			Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_Students, "Students", "~/Controls/Student/StudentSearch.ascx", false, searchTileParms, null,
						(UserHasPermission(Permission.Icon_ExpandedSearch_Students)) ? studentSearchURL : ""));
			Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_Portfolio_Teacher, "Portfolio", "~/Controls/PlaceholderTile.ascx"));

			var udfEditURL = UserHasPermission(Base.Enums.Permission.Icon_Edit_AdditionalInformation) ? "../Controls/ExpandedPlaceholder.aspx" : null;
			Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_AdditionalInformation,
										"Additional Information", "~/Controls/UDFs/UDFInformation.ascx", false,
										searchTileParms, null, null, udfEditURL));

			Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_ThinkgateUniversity, "Thinkgate University", "~/Controls/Teacher/ThinkgateUniversity.ascx", false, teacherTileParms));

		}

		private void LoadHistoryTiles()
		{
			Rotator1Tiles.Add(new Tile("10-11", "~/Controls/PlaceholderTile.ascx"));
			Rotator1Tiles.Add(new Tile("09-10", "~/Controls/PlaceholderTile.ascx"));
			Rotator1Tiles.Add(new Tile("08-09", "~/Controls/PlaceholderTile.ascx"));
			Rotator1Tiles.Add(new Tile("Year Group", "~/Controls/PlaceholderTile.ascx"));

			Rotator2Tiles.Add(new Tile("Attendance", "~/Controls/PlaceholderTile.ascx"));
			Rotator2Tiles.Add(new Tile("Discipline", "~/Controls/PlaceholderTile.ascx"));
			Rotator2Tiles.Add(new Tile("Results", "~/Controls/PlaceholderTile.ascx"));
		}

		private void LoadPlanningTiles()
		{
			if (UserHasPermission(Base.Enums.Permission.Tile_Standards))
			{
				var searchTileParms = new TileParms();
				searchTileParms.AddParm("level", EntityTypes.Teacher);
				searchTileParms.AddParm("levelID", _selectedTeacher.PersonID);
				searchTileParms.AddParm("selectID", 0);
				searchTileParms.AddParm("folder", "Curriculum");

				Rotator1Tiles.Add(new Tile("Standards", "~/Controls/Standards/StandardsSearch.ascx", false,
																		searchTileParms, null,
																		"../Controls/Standards/StandardsSearch_ExpandedV2.aspx"));
                Rotator1Tiles.Add(new Tile("Standard Filter",
                    "~/Controls/Standards/StandardsFilter.ascx", false, null, null, null));

			}

            if ((bool)Session["KenticoEnabled"] == true)
            {
                if (UserHasPermission(Base.Enums.Permission.Tile_Competencylists))
                {
                    var CompetencyListTileParms = new TileParms();
                    // var resourceTileParms2 = new TileParms();
                    var title = "Competency Lists";
                    CompetencyListTileParms.AddParm("resourceToShow", "Thinkgate.CompetencyList");
                    CompetencyListTileParms.AddParm("title", title);
                    CompetencyListTileParms.AddParm("height", "500");
                    CompetencyListTileParms.AddParm("width", "900");
                    CompetencyListTileParms.AddParm("type", "CompetencyLists");
                    // resourceTileParms2.AddParm("typeKey", ((int)Base.Enums.LookupDetail.LessonPlan).ToString());
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, CompetencyListTileParms, null, null));
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
		}

		private void LoadInstructionTiles()
		{

			DistrictParms districtParms = DistrictParms.LoadDistrictParms();

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
                    InstructionalPlanTileParms.AddParm("typeKey", ((int)Base.Enums.LookupDetail.CurriculumMap).ToString());
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, InstructionalPlanTileParms, null, UserHasPermission(Permission.Icon_Expand_CurriculumMap) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)Base.Enums.LookupDetail.CurriculumMap).ToString() : null));
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
                    UnitPlanTileParms.AddParm("typeKey", ((int)Base.Enums.LookupDetail.UnitPlan).ToString());
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, UnitPlanTileParms, null, UserHasPermission(Permission.Icon_Expand_Unitplan) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)Base.Enums.LookupDetail.UnitPlan).ToString() : null));
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
                    LessonPlanTileParms.AddParm("typeKey", ((int)Base.Enums.LookupDetail.LessonPlan).ToString());
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, LessonPlanTileParms, null, UserHasPermission(Permission.Icon_Expand_Lessonplan) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)Base.Enums.LookupDetail.LessonPlan).ToString() : null));
				}

				if (UserHasPermission(Base.Enums.Permission.Tile_ModelCurriculumUnit))
				{
					string clientState = districtParms.State;
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
                            modelCurriculumParms.AddParm("typeKey", ((int)Base.Enums.LookupDetail.CurriculumUnitOH).ToString());
                            Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, modelCurriculumParms, null, UserHasPermission(Permission.Icon_Expand_Curriculumunit) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)Base.Enums.LookupDetail.CurriculumUnitOH) : null));
							break;
						case "MA": //MASS
							title = GetModelCurriculumTileTitle();
							modelCurriculumParms.AddParm("resourceToShow", "thinkgate.UnitPlan");
							modelCurriculumParms.AddParm("title", title);
							modelCurriculumParms.AddParm("height", "500");
							modelCurriculumParms.AddParm("width", "900");
							modelCurriculumParms.AddParm("type", "UnitPlans");
							modelCurriculumParms.AddParm("showStateOnly", true);
                            modelCurriculumParms.AddParm("typeKey", ((int)Base.Enums.LookupDetail.CurriculumUnitMA).ToString());
                            Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, modelCurriculumParms, null, UserHasPermission(Permission.Icon_Expand_Curriculumunit) ? "../Controls/Resources/ResourceSearchKentico.aspx?type=" + ((int)Base.Enums.LookupDetail.UnitPlan) : null));
							break;

						default:
							break;
					}
				}

				if (districtParms.Tile_Kentico_Resources == "Yes")
				{
					var resourceTileParms3 = new TileParms();
                    var title = GetResourceTileTitle();
                    resourceTileParms3.AddParm("resourceToShow", GetKenticoResourceTileResourceTypeName());
                    resourceTileParms3.AddParm("title", title);
					resourceTileParms3.AddParm("height", "500");
					resourceTileParms3.AddParm("width", "900");
					resourceTileParms3.AddParm("type", "Resources");
                    resourceTileParms3.AddParm("typeKey", ((int)Base.Enums.LookupDetail.Resource).ToString());
                    Rotator1Tiles.Add(new Tile(title, "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, resourceTileParms3, null, UserHasPermission(Permission.Icon_Expand_Resources) ? "../Controls/Resources/ResourceSearchKentico.aspx" : null));
				}
                
			}
			else
			{
				TileParms unitPlanParms = new TileParms();
				unitPlanParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
				unitPlanParms.AddParm("levelID", _selectedTeacher.PersonID);
				unitPlanParms.AddParm("type", "Unit Plans");
				Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_UnitPlans, "Unit Plans", "~/Controls/Documents/Resources.ascx", false, unitPlanParms, null, "../Controls/Resources/ResourceSearch.aspx"));

				TileParms lessonPlanParms = new TileParms();
				lessonPlanParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
				lessonPlanParms.AddParm("levelID", _selectedTeacher.PersonID);
				lessonPlanParms.AddParm("type", "Lesson Plans");
				Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_LessonPlansResources, "Lesson Plans", "~/Controls/Documents/Resources.ascx", false, lessonPlanParms, null, "../Controls/Resources/ResourceSearch.aspx"));

				TileParms pacingDocumentsParms = new TileParms();
				pacingDocumentsParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
				pacingDocumentsParms.AddParm("levelID", _selectedTeacher.PersonID);
				pacingDocumentsParms.AddParm("type", "Pacing Documents");
				Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_PacingDocuments, "Pacing Documents", "~/Controls/Documents/Resources.ascx", false, pacingDocumentsParms, null, "../Controls/Resources/ResourceSearch.aspx"));
			}
			if (districtParms.Tile_E3_Resources == "Yes")
			{
				TileParms plansParms = new TileParms();
				plansParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
				plansParms.AddParm("levelID", _selectedTeacher.PersonID);
				plansParms.AddParm("type", "Resources");
				var resourcesTileFilterToDisplay = DistrictParms.LoadDistrictParms().ResourcesTileFilterToDisplay;
				if (!string.IsNullOrEmpty(resourcesTileFilterToDisplay)) plansParms.AddParm("UseResourcesTileFilterToDisplay", resourcesTileFilterToDisplay);
				var resourceTileExpand = UserHasPermission(Permission.Icon_Expand_Resources) ? "../Controls/Resources/ResourceSearch.aspx" : null;
				Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_Resources, "Resources", "~/Controls/Documents/Resources.ascx", false, plansParms, null, resourceTileExpand));
			}
            

           

           

		}

		private void LoadAssessmentTiles()
		{

			TileParms districtAssessmentParms = new TileParms();
			districtAssessmentParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
			districtAssessmentParms.AddParm("levelID", _selectedTeacher.PersonID);
			districtAssessmentParms.AddParm("category", "District");
			districtAssessmentParms.AddParm("userID", _selectedTeacher.PersonID);
			Rotator1Tiles.Add(new Tile(Permission.Tile_DistrictAssessment, "District Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, districtAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=District&level=Teacher", null));

			TileParms classroomAssessmentParms = new TileParms();
			classroomAssessmentParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
			classroomAssessmentParms.AddParm("levelID", _selectedTeacher.PersonID);
			classroomAssessmentParms.AddParm("category", "Classroom");
			classroomAssessmentParms.AddParm("userID", _selectedTeacher.PersonID);
			SessionObject.TeacherTileParms = classroomAssessmentParms;
			Rotator1Tiles.Add(new Tile(Permission.Tile_ClassroomAssessment, "Classroom Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, classroomAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=Classroom&level=Teacher", null));

            Rotator1Tiles.Add(new Tile(Thinkgate.Base.Enums.Permission.Tile_Items, "Items", "~/Controls/Items/Items.ascx", false, classroomAssessmentParms, null, UserHasPermission(Thinkgate.Base.Enums.Permission.Search_Item) ? "../Controls/Items/ItemSearch.aspx" : null, null, false, null, null, null, null, "assessmentItemsExportToExcel"));
			Rotator1Tiles.Add(new Tile(Thinkgate.Base.Enums.Permission.Tile_Images, "Images", "~/Controls/Assessment/AssessmentItems.ascx", false, classroomAssessmentParms, null, UserHasPermission(Thinkgate.Base.Enums.Permission.Search_Image) ? "../Controls/Images/ImageSearch_ExpandedV2.aspx" : null));
			Rotator1Tiles.Add(new Tile(Thinkgate.Base.Enums.Permission.Tile_Addendum, "Addendums", "~/Controls/Assessment/AssessmentItems.ascx", false, classroomAssessmentParms, null, UserHasPermission(Thinkgate.Base.Enums.Permission.Search_Addendum) ? "../Controls/Addendums/AddendumSearch_ExpandedV2.aspx" : null));
			Rotator1Tiles.Add(new Tile(Thinkgate.Base.Enums.Permission.Tile_Rubrics, "Rubrics", "~/Controls/Assessment/AssessmentItems.ascx", false, classroomAssessmentParms, null, UserHasPermission(Thinkgate.Base.Enums.Permission.Search_Rubric) ? "../Controls/Rubrics/RubricSearch_ExpandedV2.aspx" : null));

			TileParms blueprintAssessmentParms = new TileParms();
			blueprintAssessmentParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
			blueprintAssessmentParms.AddParm("levelID", _selectedTeacher.PersonID);
			districtAssessmentParms.AddParm("userID", _selectedTeacher.PersonID);
			blueprintAssessmentParms.AddParm("category", "Blueprint");
			Rotator1Tiles.Add(new Tile(Permission.Tile_Blueprint, "Blueprints", "~/Controls/Assessment/ViewBlueprints.ascx", false, blueprintAssessmentParms, null, null, string.Empty, false, "graphical"));
		}

		private void LoadDataAnalysisTiles()
		{
			var reportingTileParms = new TileParms();
			reportingTileParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
			reportingTileParms.AddParm("levelID", _selectedTeacher.PersonID);
			reportingTileParms.AddParm("folder", "Reporting");

			Rotator1Tiles.Add(new Tile(Permission.Tile_AssessmentResults, "Assessment Results", "~/Controls/Assessment/AssessmentResults.ascx", false, reportingTileParms, null, "../Controls/Assessment/AssessmentResults_ExpandedV2.aspx", null, false, null, "expandAssessmentResults"));

			//_rotator1Tiles.Add(new Tile(Permission.Tile_AdvancedReporting, "Advanced Reporting", "~/Controls/Reports/AdvancedReporting.ascx"));
			//Above tile calling has been changed because we are getting object reference error in Tile.Params on AdvancedReporting.aspx page. Need to review this call once.
			//This call is same as AssessmentResult page call. need to provide editControlPath parameter, now it is blank.
			var advReportingTileParms = new TileParms();
			advReportingTileParms.AddParm("level", Thinkgate.Base.Enums.EntityTypes.Teacher);
			advReportingTileParms.AddParm("levelID", _selectedTeacher.PersonID);
			advReportingTileParms.AddParm("folder", "Reporting");
            Rotator1Tiles.Add(new Tile(Permission.Tile_AdvancedReporting, "Advanced Reporting", "~/Controls/Reports/AdvancedReporting.ascx", false, advReportingTileParms, null, "", null, false, null, "advanceReportingResults"));

		    var edwinAnalyticsReports = new TileParms();
            edwinAnalyticsReports.AddParm("folder", "Reporting");
		    Rotator1Tiles.Add(new Tile(Permission.Tile_ExternalLinks_EdwinAnalytics, "Select Edwin Analytics Reports",
		        "~/Controls/Reports/EdwinAnalyticsReport.ascx", false, edwinAnalyticsReports, null, null, null, false, null,
		        null));

			var archivedReportingTileParms = new TileParms();
			archivedReportingTileParms.AddParm("archives", "yes");
            Rotator1Tiles.Add(new Tile(Permission.Tile_ArchivedReporting, "Archived Reporting", "~/Controls/Reports/AdvancedReporting.ascx", false, archivedReportingTileParms));

			/*
			_rotator1Tiles.Add(new Tile("Proficiency Analysis", "~/Controls/PlaceholderTile.ascx"));
			_rotator1Tiles.Add(new Tile("Growth Analysis", "~/Controls/PlaceholderTile.ascx"));
			_rotator1Tiles.Add(new Tile("State Analysis", "~/Controls/PlaceholderTile.ascx"));
			_rotator1Tiles.Add(new Tile("Report Engine", "~/Controls/PlaceholderTile.ascx"));
			_rotator1Tiles.Add(new Tile("Report Builder", "~/Controls/PlaceholderTile.ascx"));
			 */
		}

		protected void ScheduleClassTileClick(TileParms tileParms)
		{
			SessionObject.clickedClass = (Thinkgate.Base.Classes.Class)tileParms.GetParm("class");
			ReloadTilesControl("Classes");
		}

		protected void ScheduleClassGroupClick(TileParms tileParms)
		{
			ReloadTilesControl("Classes");
		}

        private void LoadMTSSTiles()
        {
            DistrictParms districtParms = DistrictParms.LoadDistrictParms();

            var resourceTileParms1 = new TileParms();
            var title1 = GetRtiFormTileTitle();
            resourceTileParms1.AddParm("level", EntityTypes.Teacher);
            resourceTileParms1.AddParm("resourceToShow", "thinkgate.MTSSForms");
            resourceTileParms1.AddParm("title", title1);
            resourceTileParms1.AddParm("height", "500");
            resourceTileParms1.AddParm("width", "900");
            resourceTileParms1.AddParm("type", "MTSSForms");
            Rotator1Tiles.Add(new Tile(Permission.Tile_RTIForms_SchoolPortal, title1, "~/Controls/Documents/MTSSFormsDocumentTile.ascx", false, resourceTileParms1));

            var resourceTileParms2 = new TileParms();
            var title2 = GetRtiAnalysisTileTitle();
            resourceTileParms2.AddParm("level", EntityTypes.Teacher);
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
            resourceTileParms3.AddParm("level", EntityTypes.Teacher);
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
            return Base.Classes.Data.TeacherDB.GetTeacherByPage(xId);
        }
	}
}
