<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="GradeSubjectCourseStandardSet.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.GradeSubjectCourseStandardSet" %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx"  %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>

<e3:CheckBoxList ID="chkStandardSet" CriteriaName="StandardSet" runat="server" Text="Standard Set"/>
<e3:CheckBoxList ID="chkGrade" CriteriaName="Grade" runat="server" Text="Grade"/>
<e3:CheckBoxList ID="chkSubject" CriteriaName="Subject" runat="server" Text="Subject"/>
<e3:DropDownList ID="cmbCourse" CriteriaName="Course" runat="server" Text="Course" EmptyMessage="Select a Course"/>
<e3:DropdownList ID="cmbLevel" CriteriaName="Level" runat="server" Text="Level" EmptyMessage="Select a level"/>



<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        InitialLoad: true,
        GradePos: 0,
        SubjectPos: 1,
        CoursePos: 2,
        StandardSetPos: 3,
        StdLevels_SetPos: 0,
        StdLevels_LevelPos: 1,        /* 2nd column of StandardLevelsData array */
        ShowStandardLevels: <%=ShowStandardLevels.ToString().ToLower()%>,

        PopulateControls: function () {
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;
            var StandardLevelsData;
            var GradeSubjectCourseStandardSetData;

            if (!data) {
                alert("Error finding GradeSubjectCourseStandardSet Data.");
                return;
            }
            else {
                // If hosting aspx wants to show the Standard Levels criteria, then it must provide 
                // two data arrays instead of one.  So either data will be an object holding two
                // arrays, or it will simply be the one array.
                if (_this.ShowStandardLevels) {
                    StandardLevelsData = data.StandardLevelsData;
                    GradeSubjectCourseStandardSetData = data.GradeSubjectCourseStandardSetData;
                }
                else
                    GradeSubjectCourseStandardSetData = data;
            }

            var selectedStandardSets = null;
            
            // if we are using the standard set criteria, the deal with it. Otherwise, ignore it
            if (typeof <%=chkStandardSet.CriteriaName%>Controller != 'undefined') {
                var standardSets = CriteriaDataHelpers.GetFieldDistinct(GradeSubjectCourseStandardSetData, _this.StandardSetPos);
                <%=chkStandardSet.CriteriaName%>Controller.PopulateList(standardSets);
                var selectedStandardSets = CriteriaController.GetValuesAsSinglePropertyArray("StandardSet", "Value");
            }

            // filter grades based on standardset and repopulate list          

            var filteredGradePos = CriteriaDataHelpers.GetFilteredDataPositions(GradeSubjectCourseStandardSetData, [null, null, null, selectedStandardSets]);
            var filteredGrades = CriteriaDataHelpers.GetFilteredFieldDistinct(GradeSubjectCourseStandardSetData, filteredGradePos, _this.GradePos);
            <%=chkGrade.CriteriaName%>Controller.PopulateList(filteredGrades.sort(function(a, b) {
                var charA = a.replace('_', '');
                var charB = b.replace('_', '');
                if((!isNaN(charA) && !isNaN(charB)) || (isNaN(charA) && isNaN(charB))) {
                    return charA - charB;
                }
                else {
                    return isNaN(charA) ? -1 : 1;
                }
            }));
            
            // filter subjects based on grade, standardset and repopulate list            
            var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Value");
            var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(GradeSubjectCourseStandardSetData, [selectedGrades, null, null, selectedStandardSets]);
            var filteredSubjects = CriteriaDataHelpers.GetFilteredFieldDistinct(GradeSubjectCourseStandardSetData, filteredSubjectPos, _this.SubjectPos);
            <%=chkSubject.CriteriaName%>Controller.PopulateList(filteredSubjects.sort());
            
            
            // filter curriculums based on grade, subject, standardset and repopulate
            var selectedSubjects = CriteriaController.GetValuesAsSinglePropertyArray("Subject", "Value");
            var filteredCoursePos = CriteriaDataHelpers.GetFilteredDataPositions(GradeSubjectCourseStandardSetData, [selectedGrades, selectedSubjects, null, selectedStandardSets]);
            var filteredCourses = CriteriaDataHelpers.GetFilteredFieldDistinct(GradeSubjectCourseStandardSetData, filteredCoursePos, _this.CoursePos);
            <%=cmbCourse.CriteriaName%>Controller.PopulateList(filteredCourses.sort());

            if (_this.ShowStandardLevels) {
                // filter standard levels by selected standard sets
                var filteredStandardSetsPos = CriteriaDataHelpers.GetFilteredDataPositions(StandardLevelsData, [selectedStandardSets, null]);
                var filteredStandardLevels = CriteriaDataHelpers.GetFilteredFieldDistinct(StandardLevelsData, filteredStandardSetsPos, _this.StdLevels_LevelPos);
                <%=cmbLevel.CriteriaName%>Controller.PopulateList(filteredStandardLevels.sort());
            }

            if (_this.InitialLoad) {
                _this.InitialLoad = false;
                if (typeof <%=chkStandardSet.CriteriaName%>Controller != 'undefined') <%=chkStandardSet.CriteriaName%>Controller.CheckDefaultTexts();
                <%=chkGrade.CriteriaName%>Controller.CheckDefaultTexts();
                <%=chkSubject.CriteriaName%>Controller.CheckDefaultTexts();
                <%=cmbCourse.CriteriaName%>Controller.CheckDefaultTexts();

                // Fixed issue 25063 "The Items Search window's Course values are not in sync with the Subject value"
                //Start
                var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Value");
                var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(GradeSubjectCourseStandardSetData, [selectedGrades, null, null, selectedStandardSets]);
                var filteredSubjects = CriteriaDataHelpers.GetFilteredFieldDistinct(GradeSubjectCourseStandardSetData, filteredSubjectPos, _this.SubjectPos);
                <%=chkSubject.CriteriaName%>Controller.PopulateList(filteredSubjects.sort());
            
            
                // filter curriculums based on grade, subject, standardset and repopulate
                var selectedSubjects = CriteriaController.GetValuesAsSinglePropertyArray("Subject", "Value");
                var filteredCoursePos = CriteriaDataHelpers.GetFilteredDataPositions(GradeSubjectCourseStandardSetData, [selectedGrades, selectedSubjects, null, selectedStandardSets]);
                var filteredCourses = CriteriaDataHelpers.GetFilteredFieldDistinct(GradeSubjectCourseStandardSetData, filteredCoursePos, _this.CoursePos);
                <%=cmbCourse.CriteriaName%>Controller.PopulateList(filteredCourses.sort());
                //End
            }
        },
        
        OnChange: function () {
            var _this = <%=CriteriaName%>Controller;
            _this.PopulateControls();
        }
    }
</script>