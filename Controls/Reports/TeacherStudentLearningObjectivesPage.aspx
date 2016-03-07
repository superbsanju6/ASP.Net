<%@ Page Title="Teacher Student Learning Objectives" Language="C#" MasterPageFile="~/Search.Master" AutoEventWireup="true" CodeBehind="TeacherStudentLearningObjectives.aspx.cs" Inherits="Thinkgate.Controls.Reports.TeacherStudentLearningObjectives" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ MasterType virtualpath="~/Search.Master" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        #ctl00_RightColumnReportViewerContentPlaceHolder_rptViewer_ctl09 
        {
            height: 488px !important;
            background-color: white !important;
        }
        #ctl00_RightColumnReportViewerContentPlaceHolder_rptViewer 
        {
            width : 100% !important;
        }
        #leftColumn 
        {
            height : 535px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results"/>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:DropDownList ID="cmbDistrictID" CriteriaName="DistrictID" runat="server" Text="District ID" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbDistrictName" CriteriaName="DistrictName" runat="server" Text="District Name" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbRegion" CriteriaName="Region" runat="server" Text="Region" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbSchoolID" CriteriaName="SchoolID" runat="server" Text="School ID" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbSchoolName" CriteriaName="SchoolName" runat="server" Text="School Name" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbTeacherCertID" CriteriaName="TeacherCertID" runat="server" Text="Teacher Cert ID" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbTeacherFirstName" CriteriaName="TeacherFirstName" runat="server" Text="Teacher First Name" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbTeacherLastName" CriteriaName="TeacherLastName" runat="server" Text="Teacher Last Name" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbCourseID" CriteriaName="CourseID" runat="server" Text="Course ID" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbCourseName" CriteriaName="CourseName" runat="server" Text="Course Name" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbSectionID" CriteriaName="SectionID" runat="server" Text="Section ID" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbGTID" CriteriaName="GTID" runat="server" Text="GTID" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbStudentFirstName" CriteriaName="StudentFirstName" runat="server" Text="Student First Name" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbStudentLastName" CriteriaName="StudentLastName" runat="server" Text="Student Last Name" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbPreAssessment" CriteriaName="PreAssessment" runat="server" Text="Pre Assessment" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbMeetsTarget" CriteriaName="MeetsTarget" runat="server" Text="Meets Target" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbExceedsTarget" CriteriaName="ExceedsTarget" runat="server" Text="Exceeds Target" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbPostAssessment" CriteriaName="PostAssessment" runat="server" Text="Post Assessment" EmptyMessage="None"/>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="RightColumnReportViewerContentPlaceHolder" runat="server">
    <div><rsweb:ReportViewer ID="rptViewer" ClientIDMode="Static" runat="server" maxwidth="800px" maxheight="800px"></rsweb:ReportViewer></div>
    <input id="reportServerUrl" type="hidden" runat="server"/>
    <input id="reportPath" type="hidden" runat="server"/>
    <input id="reportUsername" type="hidden" runat="server"/>
    <input id="reportPassword" type="hidden" runat="server"/>
    <input id="reportDomain" type="hidden" runat="server"/>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>


<asp:Content ID="Content7" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        function InitialLoadOfDistrictIDList() {
            DistrictIDController.PopulateList(DistrictIDDependencyData, 0, 1);
        }
        function InitialLoadOfDistrictNameList() {
            DistrictNameController.PopulateList(DistrictNameDependencyData, 0, 1);
        }
        function InitialLoadOfRegionList() {
            RegionController.PopulateList(RegionDependencyData, 0, 1);
        }
        function InitialLoadOfSchoolIDList() {
            SchoolIDController.PopulateList(SchoolIDDependencyData, 0, 1);
        }
        function InitialLoadOfSchoolNameList() {
            SchoolNameController.PopulateList(SchoolNameDependencyData, 0, 1);
        }
        //function InitialLoadOfTeacherCertIDList() {
        //    TeacherCertIDController.PopulateList(TeacherCertIDDependencyData, 0, 1);
        //}
        function InitialLoadOfTeacherFirstNameList() {
            TeacherFirstNameController.PopulateList(TeacherFirstNameDependencyData, 0, 1);
        }
        function InitialLoadOfTeacherLastNameList() {
            TeacherLastNameController.PopulateList(TeacherLastNameDependencyData, 0, 1);
        }
        function InitialLoadOfCourseIDList() {
            CourseIDController.PopulateList(CourseIDDependencyData, 0, 1);
        }
        function InitialLoadOfCourseNameList() {
            CourseNameController.PopulateList(CourseNameDependencyData, 0, 1);
        }
        function InitialLoadOfSectionIDList() {
            SectionIDController.PopulateList(SectionIDDependencyData, 0, 1);
        }
        function InitialLoadOfGTIDList() {
            GTIDController.PopulateList(GTIDDependencyData, 0, 1);
        }
        function InitialLoadOfStudentFirstNameList() {
            StudentFirstNameController.PopulateList(StudentFirstNameDependencyData, 0, 1);
        }
        function InitialLoadOfStudentLastNameList() {
            StudentLastNameController.PopulateList(StudentLastNameDependencyData, 0, 1);
        }
        function InitialLoadOfPreAssessmentList() {
            PreAssessmentController.PopulateList(PreAssessmentDependencyData, 0, 1);
        }
        function InitialLoadOfMeetsTargetList() {
            MeetsTargetController.PopulateList(MeetsTargetDependencyData, 0, 1);
        }
        function InitialLoadOfExceedsTargetList() {
            ExceedsTargetController.PopulateList(ExceedsTargetDependencyData, 0, 1);
        }
        function InitialLoadOfPostAssessmentList() {
            PostAssessmentController.PopulateList(PostAssessmentDependencyData, 0, 1);
        }
    </script>
</asp:Content>