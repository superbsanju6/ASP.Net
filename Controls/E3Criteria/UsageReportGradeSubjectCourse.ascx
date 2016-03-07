<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UsageReportGradeSubjectCourse.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.UsageReportGradeSubjectCourse" %>

<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="Text" Src="~/Controls/E3Criteria/Text.ascx" %>

<e3:text ID="txtUserName" CriteriaName="UserName" runat="server" Text="User Name" Header = "Name" Key = "UserName" DataTextField="UserName" DataValueField="UserName" Type = "String" />
<e3:DropDownList ID="cmbGrade" CriteriaName="Grade" runat="server" Text="Grade" EmptyMessage="Select a Grade" />
<e3:DropDownList ID="cmbSubject" CriteriaName="Subject" runat="server" Text="Subject" EmptyMessage="Select a Subject" />
<e3:DropDownList ID="cmbCourse" CriteriaName="Course" runat="server" Text="Course" EmptyMessage="Select a Course" />

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        InitialLoad: true,
        GradePos: 0,
        SubjectPos: 1,
        CoursePos: 2,
        UserPos: 7,
        UserValuePos: 8,
        UserID: 0,

        PopulateControls: function (sourceCriteriaName) {
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;

            if (!data) {
                alert("Error finding GradeSubjectCourse Data.");
                return;
            }
            else {
                GradeSubjectCourseData = data;
            }

            // if we are using the grade set criteria, the deal with it. Otherwise, ignore it
            if (typeof <%=cmbGrade.CriteriaName%>Controller != 'undefined') {
                var GradeSets = CriteriaDataHelpers.GetFieldDistinct(data, _this.GradePos);
                if (sourceCriteriaName != 'Grade') <%=cmbGrade.CriteriaName%>Controller.PopulateList(GradeSets.sort());
            }

            // filter subjects based on grade list
            var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Value");
            var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedGrades]);
            var filteredSubjects = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredSubjectPos, _this.SubjectPos);
            <%=cmbSubject.CriteriaName%>Controller.PopulateList(filteredSubjects.sort());

            // filter Courses based on grade, subject and repopulate
            var selectedSubjects = CriteriaController.GetValuesAsSinglePropertyArray("Subject", "Value");
            var filteredCoursePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedGrades, selectedSubjects]);
            var filteredCourses = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredCoursePos, _this.CoursePos);
            <%=cmbCourse.CriteriaName%>Controller.PopulateList(filteredCourses.sort());

            if (_this.InitialLoad) {
                _this.InitialLoad = false;

               

                if (typeof <%=cmbGrade.CriteriaName%>Controller != 'undefined') <%=cmbGrade.CriteriaName%>Controller.CheckDefaultTexts();
                <%=cmbSubject.CriteriaName%>Controller.CheckDefaultTexts();
                <%=cmbCourse.CriteriaName%>Controller.CheckDefaultTexts();
            }
        },

        PopulateSubjects: function () {
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;

            if (!data) {
                alert("Error finding GradeSubjectCourse Data.");
                return;
            }
            else {
                GradeSubjectCourseData = data;
            }

            /* Find if SchoolType and/or School is also selected. We need to filter on that too. */
            var selectedSchoolType = null;
            var selectedSchool = null;
            var schoolTypeNode = CriteriaController.FindNode("SchoolType");
            var schoolNode = CriteriaController.FindNode("School");
            if (schoolTypeNode && schoolTypeNode.Values.length > 0) {
                selectedSchoolType = [schoolTypeNode.Values[0].Value.Text];
            }
            if (schoolNode && schoolNode.Values.length > 0) {
                selectedSchool = [schoolNode.Values[0].Value.Value];
            }

            // filter subjects based on grade list
            var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Value");
            var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedGrades, null, null, null, selectedSchoolType, selectedSchool]);
            var filteredSubjects = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredSubjectPos, _this.SubjectPos);
            <%=cmbSubject.CriteriaName%>Controller.PopulateList(filteredSubjects.sort());
        },

        PopulateCourses: function () {
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;

            if (!data) {
                alert("Error finding GradeSubjectCourse Data.");
                return;
            }
            else {
                GradeSubjectCourseData = data;
            }

            /* Find if SchoolType and/or School is also selected. We need to filter on that too. */
            var selectedSchoolType = null;
            var selectedSchool = null;
            var schoolTypeNode = CriteriaController.FindNode("SchoolType");
            var schoolNode = CriteriaController.FindNode("School");
            if (schoolTypeNode && schoolTypeNode.Values.length > 0) {
                selectedSchoolType = [schoolTypeNode.Values[0].Value.Text];
            }
            if (schoolNode && schoolNode.Values.length > 0) {
                selectedSchool = [schoolNode.Values[0].Value.Value];
            }

            // filter subjects based on grade list
            var filteredGradeePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [null, null, null, null, selectedSchoolType, selectedSchool]);
            var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Value");
            if (!selectedGrades) {
                selectedGrades = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredGradeePos, _this.GradePos);
            }
            if (selectedGrades && selectedGrades.length == 0) {
                selectedGrades = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredGradeePos, _this.GradePos);
            }

            // filter Courses based on grade, subject and repopulate
            var selectedSubjects = CriteriaController.GetValuesAsSinglePropertyArray("Subject", "Value");
            var filteredCoursePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedGrades, selectedSubjects, null, null, selectedSchoolType, selectedSchool]);
            var filteredCourses = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredCoursePos, _this.CoursePos);
            <%=cmbCourse.CriteriaName%>Controller.PopulateList(filteredCourses.sort());
        },

        OnChange: function (sourceCriteriaName) {
            var _this = <%=CriteriaName%>Controller;
            _this.PopulateControls(sourceCriteriaName);
            <%=OnChange%>;
        },

        OnGradeChange: function (sourceCriteriaName) {
            var _this = <%=CriteriaName%>Controller;
            _this.PopulateSubjects();
            _this.PopulateCourses();
        },

        OnSubjectChange: function (sourceCriteriaName) {
            var _this = <%=CriteriaName%>Controller;
            _this.PopulateCourses();
        }
    }


        function PopulateUsageReportGrade(Cluster, SchoolType, School) {
            var _this = <%=CriteriaName%>Controller;
        var data = <%=CriteriaName%>DependencyData;

        if (!data) {
            alert("Error finding GradeSubjectCourse Data.");
            return;
        }
        else {
            GradeSubjectCourseData = data;
        }


        // if we are using the grade set criteria, the deal with it. Otherwise, ignore it
        if (typeof <%=cmbGrade.CriteriaName%>Controller != 'undefined') {
            var filteredGradeePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [null, null, null, Cluster, SchoolType, School]);
            var GradeSets = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredGradeePos, _this.GradePos);
            <%=cmbGrade.CriteriaName%>Controller.PopulateList(GradeSets.sort());
        }

        // filter subjects based on grade list
        var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Value");
        var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedGrades]);

        if (!selectedGrades) {
            selectedGrades = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredGradeePos, _this.GradePos);
            var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [null, null, null, Cluster, SchoolType, School]);
        }
        if (selectedGrades && selectedGrades.length == 0) {
            selectedGrades = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredGradeePos, _this.GradePos);
            var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [null, null, null, Cluster, SchoolType, School]);
        }

        var filteredSubjects = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredSubjectPos, _this.SubjectPos);
        <%=cmbSubject.CriteriaName%>Controller.PopulateList(filteredSubjects.sort());


        // filter Courses based on grade, subject and repopulate
        var selectedSubjects = CriteriaController.GetValuesAsSinglePropertyArray("Subject", "Value");
        var filteredCoursePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedGrades, selectedSubjects]);
        if (!selectedSubjects) {
            selectedSubjects = filteredSubjects;
            var filteredCoursePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [null, null, null, Cluster, SchoolType, School]);
        }
        if (selectedSubjects && selectedSubjects.length == 0) {
            selectedSubjects = filteredSubjects;
            var filteredCoursePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [null, null, null, Cluster, SchoolType, School]);
        }

        var filteredCourses = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredCoursePos, _this.CoursePos);
        <%=cmbCourse.CriteriaName%>Controller.PopulateList(filteredCourses.sort());

        if (_this.InitialLoad) {
            _this.InitialLoad = false;
            if (typeof <%=cmbGrade.CriteriaName%>Controller != 'undefined') <%=cmbGrade.CriteriaName%>Controller.CheckDefaultTexts();
            <%=cmbSubject.CriteriaName%>Controller.CheckDefaultTexts();
            <%=cmbCourse.CriteriaName%>Controller.CheckDefaultTexts();
        }
    }

</script>
