using System;
using System.Collections.Generic;
using System.Globalization;
using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Classes;
using System.Data;
using System.Linq;
using Thinkgate.Base.Enums;
using System.IO;
using ClosedXML.Excel;
using System.Web.UI;
using Thinkgate.Controls.Reports;
using Thinkgate.Domain.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Record
{
    public partial class Reports : RecordPage
    {
        #region Variables

        private string _level;
        private int _levelId;
        private int _testId;
        private string _multiTestIDs;
        private string _term;
        private string _year;
        private string _type;
        private string _class;
        private string _groups;
        private string _parent;
        private int _parentId;
        private string _selectedReport;
        public SearchParms SearchParms;
        public Criteria ReportCriteria;


        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.Report + "_"; }
        }

        #endregion

        #region Page Events

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            var siteMaster = Master as SiteMaster;
            if (siteMaster != null)
            {
                siteMaster.BannerType = BannerType.ObjectScreen;
            }
            _selectedReport = !string.IsNullOrEmpty(Request.QueryString["selectedReport"])
                ? Request.QueryString["selectedReport"]
                : "";
            SetupFolders();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            _level = Request.QueryString["level"];
            _levelId = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "levelID");
            _testId = DataIntegrity.ConvertToInt(Request.QueryString["testID"]);
            _multiTestIDs = Request.QueryString["multiTestIDs"];
            _parentId = DataIntegrity.ConvertToInt(Request.QueryString["parentID"]);
            _term = !string.IsNullOrEmpty(Request.QueryString["term"])
                ? Request.QueryString["term"]
                : "All";
            _year = !string.IsNullOrEmpty(Request.QueryString["year"])
                ? Request.QueryString["year"]
                : DistrictParms.LoadDistrictParms().Year;
            _type = !string.IsNullOrEmpty(Request.QueryString["type"])
                ? Request.QueryString["type"]
                : "All";
            _class = !string.IsNullOrEmpty(Request.QueryString["cid"])
                ? Request.QueryString["cid"]
                : "0";
            _parent = !string.IsNullOrEmpty(Request.QueryString["parent"])
                ? Request.QueryString["parent"]
                : string.Empty;
            _groups = !string.IsNullOrEmpty(Request.QueryString["groups"])
                ? Request.QueryString["groups"]
                : string.Empty;

            if (string.IsNullOrEmpty(Request.QueryString["levelID"])
                || (string.IsNullOrEmpty(Request.QueryString["testID"])
                    && string.IsNullOrEmpty(Request.QueryString["multiTestIDs"]))
                || string.IsNullOrEmpty(Request.QueryString["parent"])
                || string.IsNullOrEmpty(Request.QueryString["parentID"])
                || _levelId == 0)
            {
                SessionObject.RedirectMessage = "There was a missing required parameter in the URL.";
                Response.Redirect("~/PortalSelection.aspx");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadSearchCriteria();

            if (!IsPostBack)
            {
                Session["ItemAnalysisData_ItemAnalysisData"] = null;
                Session["ItemAnalysisData_StandardAnalysisData"] = null;
                Session["DistractorAnalysisData"] = null;
                Session["DistractorAnalysisContentData"] = null;
                Session["AtRiskStudent_TreeData"] = null;
                Session["AtRiskStandard_TreeData"] = null;
                Session["ReportCardStandard_" + Encryption.DecryptString(Request.QueryString["xID"])] = null;
            }

            LoadDefaultFolderTiles();

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
            Folders = new List<Folder>();
            if (SessionObject.LoggedInUser.HasPermission(Permission.Report_ItemAnalysis) && (String.IsNullOrEmpty(_selectedReport) || _selectedReport == "Item Analysis"))
                Folders.Add(new Folder("Item Analysis", "~/Images/new/folder_data_analysis.png", LoadItemAnalysis, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
            if (Request.QueryString["level"] != "District" && Request.QueryString["level"] != "School" && SessionObject.LoggedInUser.HasPermission(Permission.Report_DistractorAnalysis)
                 && (String.IsNullOrEmpty(_selectedReport) || _selectedReport == "Distractor Analysis"))
                Folders.Add(new Folder("Distractor Analysis", "~/Images/new/folder_data_analysis.png", LoadDistractorAnaylsis, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
            if (SessionObject.LoggedInUser.HasPermission(Permission.Report_ReportCardByStandardForStudents) && (String.IsNullOrEmpty(_selectedReport) || _selectedReport == "Report Card By Standards"))
                Folders.Add(new Folder("Report Card By Standards", "~/Images/new/folder_data_analysis.png", LoadReportCard, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
            if (SessionObject.LoggedInUser.HasPermission(Permission.Report_AtRiskStandardByStudent) && (String.IsNullOrEmpty(_selectedReport) || _selectedReport == "At Risk Standards by Student"))
                Folders.Add(new Folder("At Risk Standards by Student", "~/Images/new/folder_data_analysis.png", LoadAtRiskStudent, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
            if (SessionObject.LoggedInUser.HasPermission(Permission.Report_AtRiskStudentsByStandard) && (String.IsNullOrEmpty(_selectedReport) || _selectedReport == "At Risk Students by Standard"))
                Folders.Add(new Folder("At Risk Students by Standard", "~/Images/new/folder_data_analysis.png", LoadAtRiskStandard, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
            if (SessionObject.LoggedInUser.HasPermission(Permission.Report_StandardAnalysis) && (String.IsNullOrEmpty(_selectedReport) || _selectedReport == "Standard Analysis"))
                Folders.Add(new Folder("Standard Analysis", "~/Images/new/folder_data_analysis.png", LoadStandardAnaylsis, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));
            if (SessionObject.LoggedInUser.HasPermission(Permission.Report_GradeConversion) && (String.IsNullOrEmpty(_selectedReport) || _selectedReport == "Grade Conversion"))
                Folders.Add(new Folder("Grade Conversion", "~/Images/new/folder_data_analysis.png", LoadGradeConversion, "~/ContainerControls/TileReportContainer_1_1.ascx", 1));

            ctlFolders.BindFolderList(Folders);
        }

        #endregion

        #region Tile Methods

        private void LoadSearchCriteria()
        {

            var yearList = new List<string> { _year };
            var testList = new List<int> { _testId };

            var searchCriteria = new Criteria();

            var demographicParms = new SearchParms();
            demographicParms.AddParm(string.Empty, string.Empty);

            switch (_parent)
            {
                case "Class":
                    searchCriteria.Add(new Criterion
                    {
                        Header = "Class\\Group",
                        Locked = true,
                        Key = "Class",
                        Object = Base.Classes.Class.GetClassByID(DataIntegrity.ConvertToInt(_class)).ClassName,
                        ReportStringKey = "CID",
                        ReportStringVal = _class,
                        Type = "String",
                        Description = "Classes"
                    });

                    break;

                case "School":
                    var school = Base.Classes.School.GetSchoolByID(_parentId);

                    searchCriteria.Add(new Criterion
                    {
                        Header = "School",
                        Locked = true,
                        Key = "School",
                        Object = school.Name,
                        Type = "String"
                    });


                    break;

                case "Teacher":
                    searchCriteria.Add(new Criterion
                    {
                        Header = "Teacher",
                        Locked = true,
                        Key = "Teacher",
                        Object = TeacherDB.GetTeacherByPage(_parentId).TeacherName,
                        Type = "String"
                    });
                    break;
            }

            switch (_level)
            {
                case "Class":
                    searchCriteria.Add(new Criterion
                    {
                        Header = "Class\\Group",
                        Key = "Class",
                        Object = Base.Classes.Class.GetClassByID(DataIntegrity.ConvertToInt(_class)).ClassName,
                        ReportStringKey = "CID",
                        ReportStringVal = _levelId.ToString(CultureInfo.CurrentCulture),
                        Type = "String",
                        Description = "Classes",
                        Locked = false,
                        UIType = UIType.DropDownList,
                        DataSource = Base.Classes.Reports.GetReportingLevels(_level, _levelId, DataIntegrity.ConvertToInt(_parentId), testList, 0),
                        DataTextField = "Name",
                        DataValueField = "ID"
                    });

                    break;

                case "Student":
                    searchCriteria.Add(new Criterion
                    {
                        Header = "Student",
                        Locked = false,
                        Key = "Student",
                        Object = StudentDB.GetStudentByID(_levelId).StudentName,
                        Type = "String",
                        Description = "Classes",
                        UIType = UIType.DropDownList,
                        ReportStringKey = "StudentRecID",
                        ReportStringVal = _levelId.ToString(CultureInfo.CurrentCulture),
                        DataSource = Base.Classes.Reports.GetReportingLevels(_level, _levelId, DataIntegrity.ConvertToInt(_parentId), testList, 0),
                        DataTextField = "Name",
                        DataValueField = "ID"
                    });
                    break;

                case "Teacher":
                    searchCriteria.Add(new Criterion
                    {
                        Header = "Teacher",
                        Key = "Teacher",
                        Object = TeacherDB.GetTeacherByPage(_levelId).TeacherName,
                        ReportStringKey = "TEAID",
                        ReportStringVal = _levelId.ToString(CultureInfo.CurrentCulture),
                        Type = "String",
                        Locked = false,
                        UIType = UIType.DropDownList
                    });
                    break;
            }

            DataTable assessmentListDataTable = Base.Classes.Reports.GetTestList(_level, _levelId,
                DataIntegrity.ConvertToInt(_class), yearList, 0);

            searchCriteria.Add(new Criterion
            {
                Header = "Assessment",
                Locked = assessmentListDataTable.Rows.Count < 2,
                Key = "Assessment",
                Object = _testId == 0 ? "" : Assessment.GetAssessmentByID(_testId).TestName,
                Type = "String",
                ReportStringKey = "TID",
                ReportStringVal = _testId == 0 ? "" : _testId.ToString(CultureInfo.CurrentCulture),
                Description = "2010-2011",
                UIType = UIType.DropDownList,
                DataSource = assessmentListDataTable,
                DataTextField = "Name",
                DataValueField = "ID"
            });

            searchCriteria.Add(new Criterion
            {
                Header = "Type",
                Key = "Type",
                Object = _type,
                ReportStringKey = "TTYPES",
                ReportStringVal = _type,
                Type = "String",
                Description = "type",
                Locked = true
            });

            searchCriteria.Add(new Criterion
            {
                Header = "Year(s)",
                Key = "Year(s)",
                Object = _year,
                ReportStringKey = "TYRS",
                ReportStringVal = _year,
                Type = "String",
                Description = "2010-2011",
                Locked = true
            });

            searchCriteria.Add(new Criterion
            {
                Header = "Terms",
                Key = "Terms",
                Type = "String",
                ReportStringKey = "TTERMS",
                ReportStringVal = _term,
                Description = "List of terms",
                Object = _term,
                Locked = true
            });

            searchCriteria.Add(new Criterion
            {
                Header = "Demographics",
                Key = "Demographics",
                Locked = true,
                Empty = true
            });

            searchCriteria.Add(new Criterion
            {
                Header = "Groups",
                Key = "group",
                Object = _groups,
                ReportStringKey = "GRP",
                ReportStringVal = _groups,
                Type = "String",
                Description = "Groups Filter",
                Locked = true,
                UIType = UIType.DropDownList,
                DataSource = null,
                DataTextField = "DisplayName",
                DataValueField = "RoleId",
                Visible = false
            });

            SearchParms = new SearchParms();
            SearchParms.AddParm("reportCriteria", searchCriteria);

            var guid = Guid.NewGuid().ToString();
            hiddenTxtBox.Text = guid;

            Session["Criteria_" + guid] = searchCriteria;
        }

        private void LoadReportTiles(string tilePath, TileParms tileParms = null, string reportName = "Results")
        {
            if (tileParms == null) tileParms = new TileParms();

            tileParms.AddParm("CriteriaGUID", hiddenTxtBox.Text);
            tileParms.AddParm("multiTestIDs", _multiTestIDs);

            //_rotator1Tiles.Add(new Tile("Criteria", "~/Controls/Reports/ReportCriteria.ascx", false, tileParms));
            Rotator1Tiles.Add(new Tile(reportName, tilePath, false, tileParms));
        }

        private void LoadItemAnalysis()
        {
            var tileParms = new TileParms();

            tileParms.AddParm("AnalysisType", AnalysisType.ItemAnalysis);
            LoadReportTiles("~/Controls/Reports/ItemAnalysis.ascx", tileParms, "Item Analysis");
        }

        private void LoadDistractorAnaylsis()
        {
            LoadReportTiles("~/Controls/Reports/DistractorAnalysis.ascx", null, "Distractor Analysis");
        }

        private void LoadReportCard()
        {
            var tileParms = new TileParms();
            tileParms.AddParm("type", _type);
            LoadReportTiles("~/Controls/Reports/ReportCardStandard.ascx", tileParms, "Report Card by Standard");
        }

        private void LoadGradeConversion()
        {
            var tileParms = new TileParms();
            tileParms.AddParm("testID", _testId);
            tileParms.AddParm("level", _level);
            tileParms.AddParm("levelID", _levelId);
            tileParms.AddParm("year", _year);
            tileParms.AddParm("testType", _type);
            LoadReportTiles("~/Controls/Reports/GradeConversion.ascx", tileParms, "Grade Conversion");
        }

        private void LoadAtRiskStandard()
        {
            LoadReportTiles("~/Controls/Reports/AtRiskStandard.ascx", null, "At Risk Students by Standard");
        }

        private void LoadAtRiskStudent()
        {
            LoadReportTiles("~/Controls/Reports/AtRiskStudent.ascx", null, "At Risk Standards by Students");
        }

        private void LoadStandardAnaylsis()
        {
            var tileParms = new TileParms();
            tileParms.AddParm("AnalysisType", AnalysisType.StandardAnalysis);
            LoadReportTiles("~/Controls/Reports/ItemAnalysis.ascx", tileParms, "Standard Analysis");
        }

        protected void ExportGridImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            // Create the workbook
            /*XLWorkbook workbook = new XLWorkbook();
            ExportToExcel(workbook);
             */
            //TODO : WSH - This should loop through all rotaters, and loop through all tiles to find the specific tile we are wanting to excelify. Will need to pass in from button click which tile we are targeting. However, for now, and since this is report page with only a single tile, this is ok for expeidency
            ((TileControlBase)Rotator1Tiles[0].ParentDock.ContentContainer.Controls[0]).
                ExportToExcel();
        }

        public void ExportToExcel(XLWorkbook workbook)
        {
            //Prepare the response
            workbook.Worksheets.Add("Sheet1");
            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=ExportData.xlsx");

                // Flush the workbook to the Response.OutputStream

                Response.BinaryWrite(memoryStream.ToArray());

                Response.End();
            }
        }

        #endregion

        protected override void UpdateCache(string key, object record)
        {
            //This page doesn't use Cache so overrode base implementation to do nothing.
        }
    }
}
