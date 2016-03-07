<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdvancedReporting.ascx.cs"
    Inherits="Thinkgate.Controls.Reports.AdvancedReporting" %>
<%@ Import Namespace="Thinkgate.Utilities" %>
<style type="text/css">
    .reportLink > div {
        padding: 3px;
    }
    
        .reportLink > div > a {
        font-size: 14px;
    }
</style>
<div style="text-align: center; padding-top: 15px;height:85%;overflow-y:scroll;" class="reportLink">
    <div id="divAssessmentItemUsageReport" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Controls/Reports/AssessmentItemUsageReport.aspx") %>'>Assessment Item Usage Report</a>
    </div>
    <div id="divDAReportFL" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>fastsql_v2_direct.asp?ID=DAReporting|Criteria&selectedReport=DA Report (FL)'>DA Report (FL)</a>
    </div>
    <div id="divDAReportGen" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>fastsql_v2_direct.asp?ID=DAReportingV2|Criteria&selectedReport=DA Report'>DA Report</a>
    </div>
    <div id="divExportFiles" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>fastsql_v2_direct.asp?ID=6740|reportengine%26engineType=ExportFiles&selectedReport=Export Files'>Export Files</a>
    </div>
    <div id="divExtractEngine" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>fastsql_v2_direct.asp?ID=6740|extractengine&selectedReport=Extract Engine'>Extract Engine</a>
    </div>
    <div id="divProficiencyPortal" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>fastsql_v2_direct.asp?ID=ProficiencyPortal|Criteria&selectedReport=Proficiency Portal'>Proficiency Portal</a>
    </div>
    <div id="divProficiency" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Record/StateAnalysis.aspx") %>?xID=<%= Request.QueryString["xID"].ToString() %>&folder=Proficiency'>Proficiency</a>
    </div>
    <div id="divProficiencyArchive" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Record/StateAnalysis.aspx") %>?xID=<%= Request.QueryString["xID"].ToString() %>&folder=Proficiency%20Archive'>Proficiency Archive</a>
    </div>
    <div id="divStateAssessments" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Record/StateAnalysis.aspx") %>?xID=<%= Request.QueryString["xID"].ToString() %>&folder=Multiple%20FCAT%20Indicators'>Multiple FCAT Indicators</a>
    </div>
    <div id="divStateAssessmentsArchive" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Record/StateAnalysis.aspx") %>?xID=<%= Request.QueryString["xID"].ToString() %>&folder=Multiple%20FCAT%20Indicators%20Archive'>Multiple FCAT Indicators Archive</a>
    </div>
    <div id="divReportBuilder" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>fastsql_v2_direct.asp?ID=6740|reportengine&selectedReport=Report Builder'>Report Builder</a>
    </div>
    <div id="divReportEngine" runat="server">
        <a target="_blank" id="lnkReportEngine" runat="server" href='<%= ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>fastsql_v2_direct.asp?ID=6740|reportengine%26engineType=ReportEngine&selectedReport=Report Engine'>Report Engine</a>
    </div>
    <div id="divStateAnalysis" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>fastsql_v2_direct.asp?id=6760|Testgate_StateAnalysis%26sparse=yes%26??mode=SSR%26??districts=-1%26??schools=-1%26??demographic=1%26??year=2010%26??subject=Math%26??grade=3%26??similar=0%26??type=A&selectedReport=State Analysis'>State Analysis</a>
    </div>
    <div id="divTeacherStudentLearningObjectives" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Record/StateAnalysis.aspx")%>?xID=<%= Request.QueryString["xID"].ToString() %>&folder=Teacher%20Student%20Learning%20Objectives'>Teacher Student Learning Objectives</a>
    </div>
    <div id="divTeacherStudentLearningObjectivesArchive" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Record/StateAnalysis.aspx")%>?xID=<%= Request.QueryString["xID"].ToString() %>&folder=Teacher%20Student%20Learning%20Objectives%20Archive'>Teacher Student Learning Objectives</a>
    </div>
    <div id="divBalancedScorecard" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Controls/Reports/BalancedScorecardPage.aspx")%>'>Balanced Scorecard</a>
    </div>
    <div id="divCompetencyTrackingPortal" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Controls/Reports/CompetencyTrackingPortalPage.aspx") %>?xID=<%= HttpUtility.UrlEncode(Request.QueryString["xID"].ToString()) %>&folder=Competency%20Tracking%20Report'>Competency Tracking Portal</a>
    </div>    
    <div id="divUsageReport" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Controls/Reports/UsageStatisticReport.aspx") %>'>Usage Report</a>
    </div> 
    
    <div id="divCredentialTrackingPortal" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Controls/Reports/CredentialTrackingPortalPage.aspx") %>?xID=<%= HttpUtility.UrlEncode(Request.QueryString["xID"].ToString()) %>&folder=Credential%20Tracking%20Report'>
            Credential Tracking Report</a>
    </div>     
    <div id="dvChecklistReport" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Controls/Reports/StudentChecklistReport.aspx") %>'>Advisement Checklist</a>
    </div>
    <div id="dvStarReport" runat="server">
        <a href="#" onclick="customDialog({ name: 'RadWindowStarReport', url: '../Controls/Reports/StarReport.aspx', title: 'STAR Report', maximize: true, maxwidth: 550, maxheight: 450});" >STAR Report</a>
    </div>
    <div id="dvSecureAssessmentResultsReport" runat="server">
         <a target="_blank" href='<%= ResolveUrl("~/Controls/Reports/SecureAssessmentResultsReport.aspx") %>?level=<%= _level  %>'> Secure Assessment Results Report </a>
    </div>
</div>
