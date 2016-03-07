using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Telerik.Web.UI;
using System.Linq;
using Thinkgate.Base.Classes;


namespace Thinkgate.Controls.Reports
{
    public partial class AdvancedReporting : TileControlBase
    {
        private bool _isArchives;
        protected Thinkgate.Base.Enums.EntityTypes _level;
        protected int _schoolID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            _isArchives = Tile != null && Tile.TileParms != null && Tile.TileParms.GetParm("archives") != null;
            divAssessmentItemUsageReport.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_AssessmentItemUsageReport); ;
			divDAReportFL.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_DAReport_Florida);
			divDAReportGen.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_DAReport_General);
			divExportFiles.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_ExportFiles);
			divExtractEngine.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_ExtractEngine);
			divProficiencyPortal.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Report_ProficiencyPortal);
			divProficiency.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Report_ProficiencyReportState);
			divReportBuilder.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_ReportBuilder);
			divReportEngine.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_ReportEngine);
			divStateAnalysis.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_StateAnalysis);
			divStateAssessments.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Report_SummativeReport);
			divTeacherStudentLearningObjectives.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Hyperlink_TeacherStudentLearningObjectives);
			divBalancedScorecard.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Hyperlink_BalancedScorecard);
			divCompetencyTrackingPortal.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Hyperlink_CompetencyTrackingPortal);
            divCredentialTrackingPortal.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_CredentialTrackingReport);
            divUsageReport.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_UsageReport);
            dvChecklistReport.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_AdvisementChecklist);
            dvSecureAssessmentResultsReport.Visible = !_isArchives && UserHasPermission(Base.Enums.Permission.Access_SecureTesting_SecureAssessmentResultsReport);

            if ((!_isArchives && !UserHasPermission(Base.Enums.Permission.Tile_ArchivedReporting)) || _isArchives)
            {
                divStateAssessmentsArchive.Visible = UserHasPermission(Base.Enums.Permission.Report_SummativeReportState_Archivedreporting);
                divProficiencyArchive.Visible = UserHasPermission(Base.Enums.Permission.Report_ProficiencyReportState_Archivedreporting);
                divTeacherStudentLearningObjectivesArchive.Visible = UserHasPermission(Base.Enums.Permission.Hyperlink_TeacherStudentLearningObjectives);
            }
            else
            {
                divStateAssessmentsArchive.Visible = false;
                divProficiencyArchive.Visible = false;
                divTeacherStudentLearningObjectivesArchive.Visible = false;
            }
           
            dvStarReport.Visible = UserHasPermission(Base.Enums.Permission.Access_Report_StarReport);

            //this is quick coding to get grouping into OH/MA. Needs to be changed at some point -PLH
            if (Tile.TileParms.Parms.ContainsKey("level"))
            {
                _level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            }

            if (Tile.TileParms.Parms.ContainsKey("schoolID"))
            {
                _schoolID = (int)Tile.TileParms.GetParm("level");
            }

            string _passedGroupParm = string.Empty;
            switch (_level)
            {
                case Base.Enums.EntityTypes.District:
                case Base.Enums.EntityTypes.School:
                case Base.Enums.EntityTypes.Teacher:
                    _passedGroupParm = _level.ToString().ToLower();
                    break;
                default:
                    _passedGroupParm = "none";
                    break;
            }

            if (Tile.TileParms.Parms.ContainsKey("level"))
            {
                lnkReportEngine.HRef = ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "fastsql_v2_direct.asp?ID=6740|reportengine%26engineType=ReportEngine&??userID=" + SessionObject.LoggedInUser.UserId.ToString() + "&??rolePortal=" + _passedGroupParm + "&??schoolID=" + _schoolID.ToString() + "&selectedReport=Report Engine";
            }
        }
    }
}