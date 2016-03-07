using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Domain.Classes;
using System.Linq;
using System.IO;
using Thinkgate.Base.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Record
{
    public partial class Student : RecordPage
    {
        #region Variables

        private int _studentId;
        public Base.Classes.Student SelectedStudent;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.Student + "_"; }
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
            SessionObject.CurrentPortal = EntityTypes.Student;
            SetupFolders();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            LoadStudent();
            if (SelectedStudent == null) return;
            if (!IsPostBack)
            {
                SessionObject.ClickedInformationControl = "Identification";
            }

            LoadDefaultFolderTiles();
            SetHeaderImageUrl();

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack && Request["__EVENTTARGET"].Contains(doubleRotatorPanel.ClientID))
            {
                var eventArg = Request["__EVENTARGUMENT"];
                if (eventArg.Contains("~"))
                {
                    var dockId = eventArg.Substring(0, eventArg.IndexOf("~", StringComparison.Ordinal));
                    SessionObject.TileClicked = dockId;
                    SessionObject.LastElementsFolder_TileClicked = SessionObject.Elements_ActiveFolder;

                    var classId =
                            Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(
                                    eventArg.Substring(eventArg.IndexOf("~", StringComparison.Ordinal) + 1));
                    SessionObject.clickedClass = SelectedStudent.Classes.FindLast(c => c.ID == classId); //Find By ID and set
                    ReloadTilesControl("Classes");

                }
            }
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

        #region Folder Methods

        private void SetupFolders()
        {
            Folders = new List<Folder>
                          {
                              new Folder("Profile", "~/Images/new/folder_schedule.png", LoadProfileTiles,
                                         "~/ContainerControls/TileContainerDockSix.ascx", 6),
                              new Folder("Classes", "~/Images/new/folder_classes.png", LoadScheduleTiles,
                                         "~/ContainerControls/TileContainer_3_1.ascx", 3,
                                         "~/ContainerControls/TileContainer_3_1.ascx", 3, "Classes", "Class Information")
                          };

            var sessionObject = SessionObject;

            var rolePortal = (RolePortal)sessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection);
            if (SessionObject.LoggedInUser.HasPermission(Permission.Folder_Linking) && rolePortal == RolePortal.Teacher)
                Folders.Add(new Folder("Linking", "~/Images/blank.png", LoadFolderLinking, "~/ContainerControls/TileContainer_1.ascx", 1));

            ctlFolders.BindFolderList(Folders);
        }

        #endregion

        #region Tile Methods

        private void LoadFolderLinking()
        {
            var tileParms = new TileParms();
            tileParms.AddParm("studentId", _studentId);
            Rotator1Tiles.Add(new Tile(Permission.Folder_Linking, "Groups", "~/Controls/Groups/GroupStudent.ascx", tileParms: tileParms));
        }

        private void LoadProfileTiles()
        {
            if (SelectedStudent == null) return;

            var studentTileParms = new TileParms();
            studentTileParms.AddParm("student", SelectedStudent);
            studentTileParms.AddParm("level", EntityTypes.Student);
            studentTileParms.AddParm("levelID", SelectedStudent.ID);
            studentTileParms.AddParm("folder", "Profile");

            Rotator1Tiles.Add(new Tile(Permission.Tile_Identification_Student,
                                        "Identification",
                                        "~/Controls/Student/StudentInformation.ascx",
                                        false,
                                        studentTileParms,
                                        null,
                                        null,
                                        (UserHasPermission(Permission.Icon_Edit_StudentIdentification) ? ResolveUrl("~/Controls/Student/") + "StudentIdentification_Edit.aspx?xID=" + EntityIdEncrypted : null),
                                        false,
                                        null,
                                        null));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Demographics_Student,
                                        "Demographics",
                                        "~/Controls/Student/StudentDemographics.ascx",
                                        false,
                                        studentTileParms,
                                        null,
                                        null,
                                        (UserHasPermission(Permission.Icon_Edit_StudentDemographics) ? "../Controls/Student/StudentDemographics_Edit.aspx?xID=" + EntityIdEncrypted : null),
                                        false,
                                        null,
                                        null));

            System.Data.DataTable studentchecklist = Base.Classes.Student.GetStudentGradeInformation(Convert.ToInt32(SelectedStudent.ID));

            if (Convert.ToInt32(studentchecklist.Rows[0][0]) == 0)
                Rotator1Tiles.Add(new Tile(Permission.Tile_AdvisementChecklist, "Advisement Checklist", "~/Controls/Student/StudentChecklistTile.ascx", false, studentTileParms));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Portfolio_Student, "Portfolio", "~/Controls/Student/StudentPortfolio.ascx", false, studentTileParms));
            Rotator1Tiles.Add(new Tile(Permission.Tile_StudentAttendance, "Attendance", "~/Controls/Student/StudentAttendance.ascx", false, studentTileParms));
            Rotator1Tiles.Add(new Tile(Permission.Tile_StudentGrades, "Grades", "~/Controls/Student/StudentGrades.ascx", false, studentTileParms));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Discipline, "Discipline", "~/Controls/Student/StudentDiscipline.ascx", false, studentTileParms));
            Rotator1Tiles.Add(new Tile(Permission.Tile_StandardProficiencyReport, "Standards Proficiency", "~/Record/StandardsProficiency.ascx", false, studentTileParms));

            var udfEditUrl = UserHasPermission(Permission.Icon_Edit_AdditionalInformation) ? "../Controls/ExpandedPlaceholder.aspx" : null;
            Rotator1Tiles.Add(new Tile(Permission.Tile_AdditionalInformation,
                                        "Additional Information", "~/Controls/UDFs/UDFInformation.ascx", false,
                                        studentTileParms, null, null, udfEditUrl));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Accommodations_Student,
                                        "Accommodations",
                                        "~/Controls/Student/StudentAccommodation.ascx",
                                        false,
                                        studentTileParms,
                                        null,
                                        null,
                                        (UserHasPermission(Permission.Icon_Edit_StudentAccommodations) ? ResolveUrl("~/Controls/Student/") + "StudentAccommodation_Edit.aspx?xID=" + EntityIdEncrypted : null),
                                        false,
                                        null,
                                        null));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Assessment_Results_StudentPortal, "Assessment Results", "~/Controls/Assessment/AssessmentResults.ascx", false, studentTileParms));

            Rotator1Tiles.Add(new Tile(Permission.Tile_Credentials_Student,
                                       "Credentials",
                                       "~/Controls/Credentials/StudentCredentials.ascx",
                                       false,
                                       studentTileParms,
                                       null,
                                       (UserHasPermission(Permission.Icon_Expand_StudentCredential) ? ResolveUrl("~/Controls/Credentials/") + "StudentCredentials_Expanded.aspx?xID=" + EntityIdEncrypted : null),
                                       null,
                                       false,
                                       null,
                                       null));            
        }

        private void LoadScheduleTiles()
        {
            if (SelectedStudent == null) return;

            foreach (var c in SelectedStudent.Classes)
            {
                var tileParms = new TileParms();
                tileParms.AddParm("class", c);
                const string controlPath = "~/Controls/Class/ClassIdentification.ascx";
                var encryptedClassId = Standpoint.Core.Classes.Encryption.EncryptInt(c.ID);
                var expandUrl = UserHasPermission(Permission.Icon_Expand_ClassIdentification) ? "../Controls/Class/ClassSummary_Edit.aspx?xID=" + encryptedClassId : null;
                var editUrl = UserHasPermission(Permission.Icon_Edit_ClassIdentification) ? "../Controls/Class/ClassSummary_Edit.aspx?xID=" + encryptedClassId : null;

                var title = "<div class='selectableClassTile' title='" + c.GetClassToolTip() + "' onclick='javascript: __doPostBack(\"" + doubleRotatorPanel.ClientID + "\",\"" + "@@dockID~" + c.ID + "\")'>" + c.GetFriendlyName() + "</div>";
                var tile = new Tile(title, controlPath, true, tileParms, null, expandUrl, editUrl, false);

                Rotator1Tiles.Add(tile);
                //_rotator1Tiles.Add(new Tile(c.GetFriendlyName(), controlPath, true, tileParms, ScheduleClassTileClick, expandUrl, editURL, true));
            }

            var selectedClass = SessionObject.clickedClass;

            if (selectedClass == null)
            {
                Panel buttonDiv2 = ctlDoublePanel.GetButtonsContainer2();
                LoadContainer(ctlDoublePanel, 2, "~/ContainerControls/TileContainer_3_1_Empty.ascx", null, 0, 0);

                //div.InnerHtml = "<div class='rotator2InitTxt'>Use <div class='lowerCarouselMsgIcon'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div> to select a class above and display details here.</div>";
                buttonDiv2.CssClass = "pagingDivTallHidden";

                Session.Remove("tileClicked");
                Session.Remove("selectedRDTitleBarClass");
            }
            else
            {
                (ctlDoublePanel.GetButtonsContainer2()).CssClass = "pagingDivTall";
                ctlDoublePanel.ResetPageOnPostBack("1");

                var classTileParms = new TileParms();
                classTileParms.AddParm("class", selectedClass);
                classTileParms.AddParm("level", EntityTypes.Student);
                classTileParms.AddParm("levelID", SelectedStudent.ID);
                classTileParms.AddParm("selectID", selectedClass.ID);
                classTileParms.AddParm("category", "Classroom");
                classTileParms.AddParm("folder", "Classes");
                classTileParms.AddParm("assignmentSharingTypeID", AssignShareMode.Student);

                Rotator2Tiles.Add(new Tile(Permission.Tile_Assessment_Results_StudentPortal, "Assessment Results", "~/Controls/Assessment/AssessmentResults.ascx", false, classTileParms)); //issue
                Rotator2Tiles.Add(new Tile(Permission.Tile_Class_Standards, "Standards", "~/Controls/Standards/StandardsSearch.ascx", false, classTileParms, null, "../Controls/Standards/StandardsSearch_ExpandedV2.aspx"));

                // TODOMPF: Do not delete.  -mpf: This is for assignments and sharing (capped off for now)
                //_rotator2Tiles.Add(new Tile(Permission.Tile_Assignments_Student, "Assignments", "~/Controls/Student/StudentAssignments.ascx", false, classTileParms, null, null, "../Controls/AssignmentShare/Assignment.aspx?EntityTypeID=2&mode=2&contentid=" + SelectedStudent.ID));                                
            }

        }

        protected void ScheduleClassTileClick(TileParms tileParms)
        {
            SessionObject.clickedClass = (Base.Classes.Class)tileParms.GetParm("class");
            ReloadTilesControl("Classes");
        }

        #endregion

        private void SetHeaderImageUrl()
        {
            if (AppSettings.IsIllinois)
            {
                studentImage.ImageUrl = "~/Images/ISLE_logo1.png";
            }
            else
            {
                studentImage.ImageUrl = "~/Images/new/male_student.png";
            }
        }

        private void LoadStudent()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _studentId = GetDecryptedEntityId(X_ID);
                if (!RecordExistsInCache(Key))
                {
                    return;
                }

                SelectedStudent = (Base.Classes.Student)Base.Classes.Cache.Get(Key);
                SetStudentImage();
                DisplayStudentName();
            }
        }

        private void DisplayStudentName()
        {
            String[] nameParts = SelectedStudent.StudentName.Split(' ');
            studentName.Text = String.Empty;
            for (var i = 1; i < nameParts.Length; i++)
            {
                studentName.Text += string.Format("{0} ", nameParts[i]);
            }

            studentName.Text += nameParts[0].TrimEnd(',');
        }

        private void SetStudentImage()
        {
            //Use Profile picture if available
            string imgName = SelectedStudent.ProfilePhoto.ToString(CultureInfo.CurrentCulture);
            if (String.IsNullOrEmpty(imgName) ||
                !File.Exists(Server.MapPath(AppSettings.ProfileImageStudentWebPath + "/" + imgName)))
            {
                studentImage.ImageUrl = ResolveUrl("~/Images/new/male_student.png");
            }
            else
            {
                studentImage.ImageUrl = AppSettings.ProfileImageStudentWebPath + "/" + imgName;
            }
        }

        protected override object LoadRecord(int xId)
        {
            return StudentDB.GetStudentByID(xId);
        }
    }
}
