using System;
using System.Collections.Generic;
using System.Data;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using System.Collections;
using Thinkgate.Controls.Reports;

namespace Thinkgate.Classes
{
    [Serializable]

    public class SessionObject
    {        
        public string BackgroundClass { get; set; }
        public string BackgroundColor { get; set; }
        public string TileClicked { get; set; }
        public int LogKey { get; set; }
        public SequentialGuid LogGuid { get; set; }

        //Entity IDs
        public int District_SelectedDistrictID { get; set; }
        public int Class_SelectedClassID { get; set; }
        public int Standards_SelectedStandardID { get; set; }
        public int Staff_SelectedStaffID { get; set; }
        public int Student_SelectedStudentID { get; set; }
        public int Teacher_SelectedTeacherID { get; set; }
        public int School_SelectedSchoolID { get; set; }

        //Search Parms
        public SearchParms StudentSearchParms = new SearchParms();
        public SearchParms SchoolSearchParms = new SearchParms();
        public SearchParms TeacherSearchParms = new SearchParms();
        public SearchParms StandardsSearchParms = new SearchParms();
        public SearchParms CourseSearchParms = new SearchParms();

        //Tile Parms
        public TileParms TeacherTileParms = new TileParms();
        public TileParms DistrictTileParms = new TileParms();

        //Assessment Build Parms
        public Dictionary<string, string> AssessmentBuildParms = new Dictionary<string, string>();
        public StandardRigorLevels Standards_RigorLevels_ItemCounts = new StandardRigorLevels();

        public StandardAddendumLevels Standards_Addendumevels_ItemCounts = new StandardAddendumLevels();
        public ArrayList ItemBanks = new ArrayList();

        //Assessment Parms
        public Dictionary<int, ArrayList> ItemBankLabels = new Dictionary<int, ArrayList>();

        //Preview Standard Dialog Parms
        public Dictionary<int, Standards> PreviewStandardsDialogParms = new Dictionary<int, Standards>();

        //Reports
        public string Reports_GridSortOrder { get; set; }

        //LCO
        public Thinkgate.Base.Enums.EntityTypes LCOrole { get; set; }

        //Objects to hold loaded entities   
        public Thinkgate.Base.Classes.Standards Standards_SelectedStandard { get; set; }
        public Thinkgate.Base.Classes.Staff Staff_SelectedStaff { get; set; }
		public ThinkgateUser ApplicationUser { get; set; }
		public ThinkgateUser LoggedInUser { get; set; }
		public ThinkgateUser SelectedUser { get; set; }
		public ThinkgateUser ImpersonatingUser { get; set; }
        public drGeneric_String_String GlobalInputs { get; set; }
        public EntityTypes CurrentPortal { get; set; }
        
        //Others
        public Thinkgate.Base.Classes.Class clickedClass { get; set; }
        public String ClickedInformationControl { get; set; }        
        public string Elements_ActiveFolder { get; set; }       
        public string Students_SearchResultSize { get; set; }                
        public string LastElementsFolder_TileClicked { get; set; }             
        public string selectedRDTitleBarClass { get; set; }
        public string RedirectMessage { get; set; }
        public string RecoverableRedirectMessage { get; set; }
        public string StudentSearchTxtPostBack_smallTile { get; set; }
        public string SchoolSearchTxtPostBack_smallTile { get; set; }
        public string TeacherSearchTxtPostBack_smallTile { get; set; }
        public string GenericPassThruParm { get; set; }
        public string CheckNewWorksheetCreated { get; set; }

        //public string ExpandingTile_ControlPath { get; set; }   
        public Tile ExpandingTile_ControlTile { get; set; }
        public string ExpandingTileMode { get; set; }

        //For assessment portal - should go away!
        public DataTable TeacherPortal_assessmentData2 { get; set; }
        public DataTable TeacherPortal_assessmentRollup { get; set; }
        public DataTable TeacherPortal_chartData { get; set; }
        public DataTable TeacherPortal_heirarchyData { get; set; }
        public DataTable TeacherPortal_resultsData { get; set; }
        public DataTable TeacherPortal_reportCardByStandard { get; set; }

        public DataTable SummativeReport_DataTable { get; set; }
		public ReportData CompetencyTracking_ReportData { get; set; }
        public ReportDataCredential CredentialTracking_ReportData { get; set; }

        public string EncryptedProvidedPwd { get; set; }

        // SingleSignOn (SSO)
        public bool IsSsoAuthTimeoutSpecified { get; set; }
        public DateTime SsoAuthTimeoutDateTime { get; set; }

        //For StudentCheckList 

        public string StudentCheckList { get; set; }

        //Tile Parms for Parent Portal
        public TileParms SystemAdministratorParms = new TileParms();


        /*Credentials Alignment Available*/
        public bool? IsAlignmentDataAvailable { get; set; }
        //Groups
        public string DefaultEmptyMessage { get; set; }
        public Thinkgate.Base.Classes.Group clickedGroup { get; set; }
        public SessionObject()
        {
            BackgroundClass = "radial";
            LogKey = 1;
            LogGuid = new SequentialGuid();
        }

		/// <summary>
		/// This is in case a user is updating his/her own information, so that the information
		/// will persist throughout the user experience without logging out and back in.
		/// 
		/// </summary>
		/// <param name="user"></param>
		public void UpdateUserObject(ThinkgateUser user)
		{
			if (ApplicationUser != null && ApplicationUser.UserId == user.UserId) ApplicationUser = user;
			if (ImpersonatingUser != null && ImpersonatingUser.UserId == user.UserId) ImpersonatingUser = user;
			if (LoggedInUser != null && LoggedInUser.UserId == user.UserId) LoggedInUser = user;
			if (SelectedUser != null && SelectedUser.UserId == user.UserId) SelectedUser = user;

		}
        public void CleanUpSession(string pagePath)
        {
            if (!pagePath.Contains("StandardsPage.aspx")) Standards_SelectedStandard = null;
            
        }
    }
}
