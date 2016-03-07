using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.Reporting.WebForms;

using Telerik.Web.UI;

using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria;

namespace Thinkgate.Controls.Reports
{
    public partial class TeacherStudentLearningObjectives : BasePage
    {

        int loggedSchoolID = 0;
        string loggedTeacherFirstName = null;
        string loggedTeacherLastName = null;

        #region Events

        protected new void Page_Init(object sender, EventArgs e)
        {
            Master.Search += SearchHandler;
            base.Page_Init(sender, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (SessionObject.LoggedInUser.Roles[0].RoleName.ToLower())
            {
                case "teacher":
                    loggedTeacherFirstName = SessionObject.LoggedInUser.FirstName;
                    loggedTeacherLastName = SessionObject.LoggedInUser.LastName;
                    break;
                case "school administrator":
                    loggedSchoolID = SessionObject.LoggedInUser.School;
                    break;
            }

            if (!IsPostBack)
            {
                LoadCriteria();
                SetReportConfiguration();
            }
        }

        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            GetReportData();
        }

        #endregion

        #region Private Methods

        private void LoadCriteria()
        {
            var serializer = new JavaScriptSerializer();

            Thinkgate.Base.Classes.Reports.GetAPSReportCriteriaData(loggedSchoolID, loggedTeacherFirstName, loggedTeacherLastName);

            cmbDistrictID.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(0));
            cmbDistrictID.OnClientLoad = "InitialLoadOfDistrictIDList";

            cmbDistrictName.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(1));
            cmbDistrictName.OnClientLoad = "InitialLoadOfDistrictNameList";

            cmbRegion.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(2));
            cmbRegion.OnClientLoad = "InitialLoadOfRegionList";

            cmbSchoolID.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(3));
            cmbSchoolID.OnClientLoad = "InitialLoadOfSchoolIDList";

            cmbSchoolName.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(4));
            cmbSchoolName.OnClientLoad = "InitialLoadOfSchoolNameList";

            //cmbTeacherCertID.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(5));
            //cmbTeacherCertID.OnClientLoad = "InitialLoadOfTeacherCertIDList";

            List<string> list = new List<string>();
            list.Add("N/A");
            cmbTeacherCertID.DataSource = list;

            cmbTeacherFirstName.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(6));
            cmbTeacherFirstName.OnClientLoad = "InitialLoadOfTeacherFirstNameList";

            cmbTeacherLastName.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(7));
            cmbTeacherLastName.OnClientLoad = "InitialLoadOfTeacherLastNameList";

            cmbCourseID.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(8));
            cmbCourseID.OnClientLoad = "InitialLoadOfCourseIDList";

            cmbCourseName.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(9));
            cmbCourseName.OnClientLoad = "InitialLoadOfCourseNameList";

            cmbSectionID.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(10));
            cmbSectionID.OnClientLoad = "InitialLoadOfSectionIDList";

            cmbGTID.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(11));
            cmbGTID.OnClientLoad = "InitialLoadOfGTIDList";

            cmbStudentFirstName.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(12));
            cmbStudentFirstName.OnClientLoad = "InitialLoadOfStudentFirstNameList";

            cmbStudentLastName.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(13));
            cmbStudentLastName.OnClientLoad = "InitialLoadOfStudentLastNameList";

            cmbPreAssessment.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(14));
            cmbPreAssessment.OnClientLoad = "InitialLoadOfPreAssessmentList";

            cmbMeetsTarget.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(15));
            cmbMeetsTarget.OnClientLoad = "InitialLoadOfMeetsTargetList";

            cmbExceedsTarget.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(16));
            cmbExceedsTarget.OnClientLoad = "InitialLoadOfExceedsTargetList";

            cmbPostAssessment.JsonDataSource = serializer.Serialize(Thinkgate.Base.Classes.Reports.BuildJsonArray(17));
            cmbPostAssessment.OnClientLoad = "InitialLoadOfPostAssessmentList";
        }

        private void GetReportData()
        {
            if (reportServerUrl.Value.Length != 0 && reportPath.Value.Length != 0)
            {
                var criteriaController = Master.CurrentCriteria();

                int selectedDitsrictID = DataIntegrity.ConvertToInt(criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("DistrictID").Select(x => x.Text).FirstOrDefault());
                string selectedDistrictName = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("DistrictName").Select(x => x.Text).FirstOrDefault();
                string selectedRegion = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Region").Select(x => x.Text).FirstOrDefault();
                int selectedSchoolID = loggedSchoolID == 0 ? DataIntegrity.ConvertToInt(criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("SchoolID").Select(x => x.Text).FirstOrDefault()) : loggedSchoolID;
                string selectedSchoolName = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("SchoolName").Select(x => x.Text).FirstOrDefault();
                string selectedTeacherCertID = null;
                string selectedTeacherFirstName = loggedTeacherFirstName == null ? criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("TeacherFirstName").Select(x => x.Text).FirstOrDefault() : loggedTeacherFirstName;
                string selectedTeacherLastName = loggedTeacherLastName == null ? criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("TeacherLastName").Select(x => x.Text).FirstOrDefault() : loggedTeacherLastName;
                string selectedCourseID = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("CourseID").Select(x => x.Text).FirstOrDefault();
                string selectedCourseName = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("CourseName").Select(x => x.Text).FirstOrDefault();
                int selectedSectionID = DataIntegrity.ConvertToInt(criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("SectionID").Select(x => x.Text).FirstOrDefault());
                int selectedGTID = DataIntegrity.ConvertToInt(criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("GTID").Select(x => x.Text).FirstOrDefault());
                string selectedStudentFirstName = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("StudentFirstName").Select(x => x.Text).FirstOrDefault();
                string selectedStudentLastName = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("StudentLastName").Select(x => x.Text).FirstOrDefault();
                string selectedPreAssessment = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("PreAssessment").Select(x => x.Text).FirstOrDefault();
                string selectedMeetsTarget = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("MeetsTarget").Select(x => x.Text).FirstOrDefault();
                string selectedExceedsTarget = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("ExceedsTarget").Select(x => x.Text).FirstOrDefault();
                string selectedPostAssessment = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("PostAssessment").Select(x => x.Text).FirstOrDefault();

                var reportParameter = new ReportParameterCollection();
                reportParameter.Add(new ReportParameter("DistrictID", selectedDitsrictID == 0 ? null : selectedDitsrictID.ToString()));
                reportParameter.Add(new ReportParameter("DistrictName", selectedDistrictName));
                reportParameter.Add(new ReportParameter("Cluster", selectedRegion));
                reportParameter.Add(new ReportParameter("SchoolID", selectedSchoolID == 0 ? null : selectedSchoolID.ToString()));
                reportParameter.Add(new ReportParameter("SchoolName", selectedSchoolName));
                reportParameter.Add(new ReportParameter("TeacherCertID", selectedTeacherCertID));
                reportParameter.Add(new ReportParameter("TeacherFirstName", selectedTeacherFirstName));
                reportParameter.Add(new ReportParameter("TeacherLastName", selectedTeacherLastName));
                reportParameter.Add(new ReportParameter("CourseID", selectedCourseID));
                reportParameter.Add(new ReportParameter("CourseName", selectedCourseName));
                reportParameter.Add(new ReportParameter("SectionID", selectedSectionID == 0 ? null : selectedSectionID.ToString()));
                reportParameter.Add(new ReportParameter("GTID", selectedGTID == 0 ? null : selectedGTID.ToString()));
                reportParameter.Add(new ReportParameter("StudentFirstName", selectedStudentFirstName));
                reportParameter.Add(new ReportParameter("StudentLastName", selectedStudentLastName));
                reportParameter.Add(new ReportParameter("PreAssessment", selectedPreAssessment));
                reportParameter.Add(new ReportParameter("MeetsTarget", selectedMeetsTarget));
                reportParameter.Add(new ReportParameter("ExceedsTarget", selectedExceedsTarget));
                reportParameter.Add(new ReportParameter("PostAssessment", selectedPostAssessment));

                rptViewer.ProcessingMode = ProcessingMode.Remote;
                rptViewer.ServerReport.ReportServerUrl = new Uri(reportServerUrl.Value);
                rptViewer.ServerReport.ReportPath = reportPath.Value;
                rptViewer.ServerReport.ReportServerCredentials = new CustomReportCredentials(reportUsername.Value, reportPassword.Value, reportDomain.Value);
                rptViewer.ShowParameterPrompts = false;
                rptViewer.ShowPrintButton = false;
                rptViewer.ShowRefreshButton = false;
                rptViewer.ShowBackButton = false;
                rptViewer.ServerReport.SetParameters(reportParameter);
                rptViewer.ServerReport.Refresh();

                var ajaxPnl = (RadAjaxPanel)Master.FindControl("AjaxPanelResults");
                ajaxPnl.Height = System.Web.UI.WebControls.Unit.Pixel(20);
            }
        }

        private void SetReportConfiguration()
        {
            var sSRSServerInfo = Thinkgate.Base.Classes.Reports.GetSSRSServerInfo(SessionObject.GlobalInputs);
            var sSRSReportPaths = Thinkgate.Base.Classes.Reports.GetSSRSReportPaths("TeacherStudentLearningObjectivesReport");

            if (sSRSServerInfo.Rows.Count == 0 || sSRSReportPaths.Rows.Count == 0)
            { return; }

            reportServerUrl.Value = sSRSServerInfo.Rows[0]["URL"].ToString();
            reportPath.Value = sSRSReportPaths.Rows[0]["Path"].ToString();
            reportUsername.Value = sSRSServerInfo.Rows[0]["Username"].ToString();
            reportPassword.Value = sSRSServerInfo.Rows[0]["Password"].ToString();
            reportDomain.Value = sSRSServerInfo.Rows[0]["Domain"].ToString();
        }

        #endregion
    }
}


