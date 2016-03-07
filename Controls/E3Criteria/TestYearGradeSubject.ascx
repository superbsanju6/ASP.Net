<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TestYearGradeSubject.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.TestYearGradeSubject" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>

<e3:DropDownList ID="cmbAssessmentType" CriteriaName="AssessmentType" runat="server" Text="Assessment" EmptyMessage="Select an Assessment"/>
<e3:DropDownList ID="cmbYear" CriteriaName="Year" runat="server" Text="Year" EmptyMessage="Select a Year"/>
<e3:DropDownList ID="cmbGrade" CriteriaName="Grade" runat="server" Text="Grade" EmptyMessage="Select a Grade"/>
<e3:DropDownList ID="cmbSubject" CriteriaName="Subject" runat="server" Text="Subject" EmptyMessage="Select a Subject"/>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        InitialLoad: true,
        AssessmentTypePos: 0,
        YearPos: 1,
        GradePos: 2,
        SubjectPos: 3,

        PopulateControls: function (sourceCriteriaName) {
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;
            if (!data) {
                alert("Error finding TestYearGradeSubject Data.");
                return;
            }
            var assessmentTypes = CriteriaDataHelpers.GetFieldDistinct(data, _this.AssessmentTypePos);
            if (sourceCriteriaName != 'AssessmentType') <%=cmbAssessmentType.CriteriaName%>Controller.PopulateList(assessmentTypes);

            // filter years based on assessment type
            var selectedAssessmentTypes = CriteriaController.GetValuesAsSinglePropertyArray("AssessmentType", "Text");
            var filteredYearPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedAssessmentTypes]);
            var filteredYears = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredYearPos, _this.YearPos);
            if (sourceCriteriaName != 'Year') <%=cmbYear.CriteriaName%>Controller.PopulateList(filteredYears.sort());
            
            // filter grades based on assessment type and years and repopulate list
            var selectedYears = CriteriaController.GetValuesAsSinglePropertyArray("Year", "Text");
            var filteredGradePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedAssessmentTypes, selectedYears]);
            var filteredGrades = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredGradePos, _this.GradePos);
            if (sourceCriteriaName != 'Grade') <%=cmbGrade.CriteriaName%>Controller.PopulateList(filteredGrades.sort());
            
            // filter subjects based on type, year and grade and repopulate
            var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Text");
            var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedAssessmentTypes, selectedYears, selectedGrades]);
            var filteredSubjects = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredSubjectPos, _this.SubjectPos);
            if (sourceCriteriaName != 'Subject') <%=cmbSubject.CriteriaName%>Controller.PopulateList(filteredSubjects.sort());

            if (_this.InitialLoad) {
                _this.InitialLoad = false;
                <%=cmbAssessmentType.CriteriaName%>Controller.CheckDefaultTexts();
                <%=cmbYear.CriteriaName%>Controller.CheckDefaultTexts();
                <%=cmbGrade.CriteriaName%>Controller.CheckDefaultTexts();
                <%=cmbSubject.CriteriaName%>Controller.CheckDefaultTexts();
            }
        },
        
        OnChange: function (sourceCriteriaName) {
            var _this = <%=CriteriaName%>Controller;
            _this.PopulateControls(sourceCriteriaName);
            <%=OnChange%>;
        }
    }
</script>