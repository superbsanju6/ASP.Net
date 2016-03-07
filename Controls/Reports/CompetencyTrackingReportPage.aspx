<%@ Page Title="Competency Tracking Report" Language="C#" MasterPageFile="~/Search.Master"
    AutoEventWireup="true" CodeBehind="CompetencyTrackingReportPage.aspx.cs" Inherits="Thinkgate.Controls.Reports.CompetencyTrackingReportPage" %>

<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="Demographics" Src="~/Controls/E3Criteria/Demographics.ascx" %>
<%@ Register TagPrefix="e3" TagName="DateRange" Src="~/Controls/E3Criteria/DateRange.ascx" %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx" %>
<%@ Register Src="~/Controls/E3Criteria/AutoCompleteCriteriaControls/ACDropDownList.ascx" TagPrefix="e3" TagName="ACDropDownList" %>
<%@ Register Src="~/Controls/E3Criteria/AutoCompleteCriteriaControls/ACSchoolTeacher.ascx" TagPrefix="e3" TagName="ACSchoolTeacher" %>
<%@ Register Src="~/Controls/E3Criteria/AutoCompleteCriteriaControls/GetGradeSubjectCourseStandard.ascx" TagPrefix="e3" TagName="GetGradeSubjectCourseStandard" %>
<%@ Register TagPrefix="e3" TagName="RadDropDownList" Src="~/Controls/E3Criteria/RadDropDownList.ascx" %>


<%@ MasterType VirtualPath="~/Search.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:ACDropDownList ID="cmbViewBy" CriteriaName="ViewBy" runat="server" Text="View By" EmptyMessage="Select View" OnChange="CmbViewByChanged()" Required="True" />

    <%--    <e3:DropDownList    ID="cmbViewBy"      CriteriaName="ViewBy"       runat="server" Text="View By"       EmptyMessage="Select View"      OnChange="CmbViewByChanged()"  Required="True" />--%>
    <e3:ACDropDownList ID="cmbSchool" CriteriaName="School" runat="server" Text="School" EmptyMessage="Select School" OnChange="CmbSchoolChanged()" />

    <e3:ACDropDownList ID="cmbTeacherName" CriteriaName="TeacherName" ClientIDMode="Static" runat="server" Text="Teacher" EmptyMessage="Select Teacher" OnChange="CmbTeacherNameChanged()" />
    <e3:DropDownList ID="cmbClass" CriteriaName="Class" runat="server" Text="Class" EmptyMessage="Select Class" OnChange="CmbClassChanged()" />
    <e3:ACDropDownList ID="cmbStudent" CriteriaName="Student" runat="server" Text="Student" EmptyMessage="Select Student" />

    <%--<e3:Demographics ID="demos" CriteriaName="Demographic" runat="server" Text="Demographic"/>--%>
    <e3:ACDropDownList ID="cmbDemos" CriteriaName="Demographic" runat="server" Text="Demographic" EmptyMessage="Select Demographic" />
    <e3:ACDropDownList ID="cmbGroup" CriteriaName="Group" runat="server" Text="Group" EmptyMessage="Select Group" />
    <p runat="server">
        <span style="font-weight: bold; text-decoration: underline; color: black; position: relative; padding-top:5px; float: left; left: 10px;">Competency Selection</span>
        <e3:RadDropDownList ID="cmbStandardList" CriteriaName="StandardList" runat="server" Text="Standard List" EmptyMessage="Select Standards" OnChange="CmbStandardListChanged()" Required="True" />
      
     
       <e3:ACSchoolTeacher ID="cmbTeacher" CriteriaName="Teacher" runat="server" Text="Teacher" EmptyMessage="Select School" OnChange="CmbTeacherChanged()" />
       
    
       
        <e3:GetGradeSubjectCourseStandard ID="cmbStandardSet" CriteriaName="StandardSet" runat="server" Text="Standard Set"  OnChange="cmbStandardSetChanged()"></e3:GetGradeSubjectCourseStandard>
<%--        <e3:RadDropDownList ID="cmbListSelection" CriteriaName="ListSelection" runat="server"  Text="List Selection" EmptyMessage="Select List Selection" EnableItemCaching="false" DataTextField="level" OnClientItemsRequesting="BindDataOnDemand" DataValueField="level" EnableLoadOnDemand="True" OnClientDropDownOpening="SetValue" />--%>
        <e3:RadDropDownList ID="cmbCompentencyList" CriteriaName="ListSelection" runat="server"  Text="List Selection" EmptyMessage="Select List Selection" EnableItemCaching="false" DataTextField="level" OnClientItemsRequesting="BindDataOnDemand" DataValueField="level" EnableLoadOnDemand="True" OnClientDropDownOpening="SetValue"  OnChange="ListWorksheetChanged()"  />
        <e3:RadDropDownList ID="cmbCompentencyWorksheet" CriteriaName="WorksheetSelection" runat="server"  Text="List Selection" EmptyMessage="Select List Selection" EnableItemCaching="false" DataTextField="level" OnClientItemsRequesting="BindDataOnDemand" DataValueField="level" EnableLoadOnDemand="True" OnClientDropDownOpening="SetValue"  OnChange="ListWorksheetChanged()"   />

        <e3:RadDropDownList ID="cmbStandardLevel" IsNotAllowToEnterText="true" CriteriaName="StandardLevel" runat="server" Text="Standard Level" DataTextField="Level" DataValueField="Level" EnableLoadOnDemand="True" OnClientItemsRequesting="BindDataOnDemand" EmptyMessage="Select Standard Level" />
        <e3:DateRange ID="drDateRange" CriteriaName="DateRange" runat="server" Text="Date Range" />
    </p>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <div id="divExportOptions" style="text-align: left">
        <asp:ImageButton runat="server" ID="btnExportExcel" OnClick="btnExportExcel_Click"
            ImageUrl="~/Images/Toolbars/excel_button.png" />
<div id="divMessage1" style="display:none;float:right;font-style:italic;font-weight:bold;word-wrap: break-word;position:justify;width: 83%;">*Results represent the most recent date a student was scored for each competency and a count of the number of times marked per rubric column.</div>
        <div id="divMessage2" style="display:none;float:right;font-style:italic;font-weight:bold;word-wrap: break-word;position:justify;width: 76%;margin-left:7px;margin-top:4px;">*Percentages represent the % of competencies in column 3 marked for each rubric column.</div>
        <div id="divMessage3" style="display:none;float:right;font-style:italic;font-weight:bold;word-wrap: break-word;position:justify;width: 88%;margin-left:7px;margin-top:4px;">*Percentages represent the % of students from column 1 most recently  marked for each rubric column.</div>
        <div id="divMessage4" style="display:none;float:right;font-style:italic;font-weight:bold;word-wrap: break-word;position:justify;width: 88%;">*Percentages represent the % of total results for students in column 1 and competencies in column 3 most recently marked for each rubric column.</div></div>    
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="gridResultsPanel">
        <telerik:RadGrid runat="server" ID="radGridResults" AutoGenerateColumns="False" Width="100%" Height="509px" AllowFilteringByColumn="False"
            PageSize="18" AllowPaging="false" AllowSorting="False" CssClass="" Skin="Web20"
            AllowMultiRowSelection="False"
            OnItemDataBound="radGridResults_ItemDataBound"
            OnSortCommand="OnSortCommand">
            <PagerStyle Mode="NextPrevAndNumeric" />
            <ClientSettings EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="false" UseClientSelectColumnOnly="false" />
                <Scrolling AllowScroll="True" UseStaticHeaders="False" SaveScrollPosition="True" />
            </ClientSettings>

        </telerik:RadGrid>
        <asp:PlaceHolder ID="initialDisplayText" runat="server">
            <div id="lblInitialText" runat="server" style="font-size: 11pt; text-align: center; position: absolute; width: 100%; top: 25%; margin-left: auto; margin-right: auto">
                <b>Please choose criteria and select Search.</b><br />
                (<b>View By</b> and <b>Standard List</b> are required)
            </div>
        </asp:PlaceHolder>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript" src="../../Scripts/thinkgate.util.js"></script>
    <script type="text/javascript">

        //This is temporary
        // IF IE load tempSetInitialComboBoxItems() here, else it will fire on DOMReady
        //if (BrowserDetect.browser == "Explorer") {
        //    Sys.Application.add_load(tempSetInitialComboBoxItems);
        //}
        //
        function CompetencyexpandedStandardsSearchAjaxResponseHandler(sender, eventArgs) {
            try {
                //Hide all messages first
                $("#divMessage1").hide();
                $("#divMessage2").hide();
                $("#divMessage3").hide();
                $("#divMessage4").hide();
                if ($("#ctl00_RightColumnContentPlaceHolder_radGridResults_ctl00") != undefined) {
                    var msg = $("#ctl00_RightColumnContentPlaceHolder_radGridResults_ctl00")[0].innerText;
                    if (msg == undefined) {
                        msg = $("#ctl00_RightColumnContentPlaceHolder_radGridResults")[0].innerHTML;
                    }
                    if (msg.indexOf("No records to display.") == -1) {
                        var viewByValue = getCriteriaText("ViewBy");
                        var standardLevelValue = getCriteriaText("StandardLevel");
                        if (viewByValue.toLowerCase() == "student" && standardLevelValue == null) {
                            $("#divMessage1").show();
                            $("#divMessage2").hide();
                            $("#divMessage3").hide();
                            $("#divMessage4").hide();
                        }
                        else if (viewByValue.toLowerCase() == "student" && standardLevelValue != null) {
                            $("#divMessage2").show();
                            $("#divMessage1").hide();
                            $("#divMessage3").hide();
                            $("#divMessage4").hide();
                        }
                        else if (viewByValue.toLowerCase() != "student" && standardLevelValue == null) {
                            $("#divMessage3").show();
                            $("#divMessage1").hide();
                            $("#divMessage2").hide();
                            $("#divMessage4").hide();
                        }
                        else if (viewByValue.toLowerCase() != "student" && standardLevelValue != null) {
                            $("#divMessage4").show();
                            $("#divMessage3").hide();
                            $("#divMessage2").hide();
                            $("#divMessage1").hide();
                        }
                    }
                }
            }
            catch (e) { }
        }

        function StudentsCTLView(selectedObjectID, standartID, workSheetID, robricsortNumber, viewbySelectedValue, demographicID, groupID) {
            var url = 'CompetencyTrackingReportStudentList.aspx?selectedObjectID=' + selectedObjectID + '&standartID=' + standartID + '&workSheetID=' + workSheetID + '&robricsortNumber=' + robricsortNumber + '&viewbySelectedValue=' + viewbySelectedValue + '&demographicID=' + demographicID + '&groupID=' + groupID;

            customDialog({ url: url, maximize: true, maxwidth: 950, height: 950, title: 'Competency Tracking - Details' });
        }

        function StanderdCTLView(selectedObjectID, standartID, workSheetID, robricsortNumber, viewbySelectedValue, studentID) {
            var url = 'CompetencyTrackingDetailsStandardRollup.aspx?selectedObjectID=' + selectedObjectID + '&standartID=' + standartID + '&workSheetID=' + workSheetID + '&robricsortNumber=' + robricsortNumber + '&viewbySelectedValue=' + viewbySelectedValue + '&studentID=' + studentID;

            customDialog({ url: url, maximize: true, maxwidth: 950, height: 950, title: 'Competency Tracking - Details' });
        }


        $(document).ready(function () {
            resetCriteriaSections();
            resizeContent();
            //This is temporary
            // Non-IE fire here
            //if (BrowserDetect.browser != "Explorer") {
            //    tempSetInitialComboBoxItems();
            //}
            //
            $("#ctl00_LeftColumnHeaderContentPlaceHolder_SearchAndClear_btnClear_input").click(function()
            {
                hideDropdown("StandardLevel");
            })

        });

        $(window).resize(function () {
            resizeContent();
        });

        function GetSelectedLevelText() {
            $find('<%=cmbStandardLevel.RadClientId %>').clearSelection();
            $find('<%=cmbStandardLevel.RadClientId %>').clearItems();
            CriteriaController.RemoveAllDependency('<%= cmbStandardLevel.CriteriaName %>')
        }

        function TreeListCreated() {
            resizeContent();
        }

        function resizeContent() {
            $(".rtlDataDiv").height($(window).height() - $("#divExportOptions").height() - $(".rtlHeader").height() - 12);
        }

        //function ChangeSchoolBasedOnGrade(grade) {
        //    var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Text", true);
        //    var filteredSchoolPos = CriteriaDataHelpers.GetFilteredDataPositions(SchoolDependencyData, [null, null, selectedGrades]);
        //    var filteredSchools = [];
        //    for (var j = 0; j < filteredSchoolPos.length; j++) {
        //        filteredSchools.push(SchoolDependencyData[filteredSchoolPos[j]]);
        //    }

        //    SchoolController.PopulateList(filteredSchools, 0, 1);
        //}

        //  var TopSectionDropDownArray = ["ViewBy", "School", "Teacher", "Class", "Student", "Demographic", "Group"];
        var TopSectionDropDownArray = ["ViewBy", "School", "Teacher", "Class", "Student", "TeacherName", "Group"];
        var BottomSectionDropDownArray = ["StandardList", "ListSelection", "StandardSet", "StandardLevel", "DateRange"];

        function hideSection(sectionToHide) {
            if (sectionToHide === "topSection") {
                TopSectionDropDownArray.forEach(function (entry) {
                    //console.log(entry);
                    $("#criteriaHeaderDiv_" + entry).hide();
                    //$("#selectedCritieriaDisplayArea_" + entry).hide();
                });
                return;
            }
            if (sectionToHide === "bottomSection") {
                BottomSectionDropDownArray.forEach(function (entry) {
                    //console.log(entry);
                    $("#criteriaHeaderDiv_" + entry).hide();
                    //$("#selectedCritieriaDisplayArea_" + entry).hide();
                });
                return;
            }
            //Hide all
            hideSection("topSection");
            hideSection("bottomSection");
            return;
        };

        function showDropdown(theDropdownName) {
            $("#criteriaHeaderDiv_" + theDropdownName).show();
            $("#selectedCritieriaDisplayArea_" + theDropdownName).show();
        };

        function hideDropdown(theDropdownName) {
            $("#criteriaHeaderDiv_" + theDropdownName).hide();
            $("#selectedCritieriaDisplayArea_" + theDropdownName).hide();
        };

        function reloadPage() {
            window.location.reload(false);
        }

        function resetCriteriaSections() {
            CriteriaController.Clear();
            initCriteriaSections();
        }

        function initCriteriaSections() {
            hideSection("topSection");
            showDropdown("ViewBy");

            hideSection("bottomSection");
            showDropdown("StandardList");
            // showDropdown("StandardLevel");
            showDropdown("DateRange");
          
        }

        function ResetCriteria() {

            CriteriaController.RemoveAll("School");
            CriteriaController.RemoveAll("Teacher");
            $find("<%= cmbTeacherName.ComboBox.ClientID %>").clearItems();

            CriteriaController.RemoveAll("Class");
            $find("<%= cmbClass.ComboBox.ClientID %>").clearItems();

            CriteriaController.RemoveAll("Student");
            $find("<%= cmbStudent.ComboBox.ClientID %>").clearItems();

            CriteriaController.RemoveAll("Demographic");
            CriteriaController.RemoveAll("Group");

            CriteriaController.RemoveAll("StandardList");
            CriteriaController.RemoveAll("ListSelection");
            CriteriaController.RemoveAll("StandardSet");
            CriteriaController.RemoveAll("StandardLevel");
            CriteriaController.RemoveAll("DateRange");
            CriteriaController.RemoveAll("TeacherName");
            CriteriaController.RemoveAll("Teacher");
            CriteriaController.UpdateCriteriaForSearch();
            TeacherController.Clear();
            $("#criteriaHeaderDiv_Demographic").find("div.left").html('Demographic:');
        }



        function getCriteriaValue(criteria) {

            var criteriaValue = null;

            if (CriteriaController.FindNode(criteria) && CriteriaController.FindNode(criteria).Values[0]) {
                for (var i = 0; i < CriteriaController.FindNode(criteria).Values.length; i++) {
                    if (CriteriaController.FindNode(criteria).Values[i].CurrentlySelected)
                        criteriaValue = CriteriaController.FindNode(criteria).Values[i].Value.Value;
                }
            }

            return criteriaValue;
        }
        function getCriteriaText(criteria) {

            var criteriaValue = null;

            if (CriteriaController.FindNode(criteria) && CriteriaController.FindNode(criteria).Values[0]) {
                for (var i = 0; i < CriteriaController.FindNode(criteria).Values.length; i++) {
                    if (CriteriaController.FindNode(criteria).Values[i].CurrentlySelected)
                        criteriaValue = CriteriaController.FindNode(criteria).Values[i].Value.Text;
                }
            }

            return criteriaValue;
        }

        function cmbStandardSetChanged() {
            CriteriaController.RemoveAll("StandardLevel");
            $find("<%= cmbStandardLevel.ComboBox.ClientID %>").clearItems();
            showDropdown("StandardLevel");
        }



        function CmbSchoolChanged() {
            //CriteriaController.RemoveAll("Teacher");
            TeacherController.Clear();
            CriteriaController.RemoveAllDependency('<%= cmbTeacher.CriteriaName %>')
            getteacherEnabled();
            //CriteriaController.UpdateCriteriaForSearch();
            // hideDropdown("Teacher");
            hideDropdown("Class");

            var viewByValue = getCriteriaText("ViewBy");
            var selectedSchool = getCriteriaText("School");
            var selectedTeacherName = getCriteriaText("TeacherName");
            var selectedTeacherValue = getCriteriaValue("TeacherName");
            var selectedSchoolValue = getCriteriaValue("School");
            if (!stringIsEmpty(viewByValue) && !stringIsEmpty(selectedSchool)) {
                if (viewByValue.toLowerCase() == "teacher" || viewByValue.toLowerCase() == "teachername") {
                    if (selectedSchool != null) {
                        var selectedSchoolVal = getCriteriaValue("School");

                        getTeacherListForSchool(selectedSchoolVal);

                        showDropdown("TeacherName");

                        var SelectedSchoolControl = $find("<%= cmbTeacher.ComboBox.ClientID %>");

                        var SelectedSchoolitem = SelectedSchoolControl.findItemByText(selectedSchool);

                        if (SelectedSchoolitem) {
                            SelectedSchoolitem.select();
                        }

                    }
                }
                else if (viewByValue.toLowerCase() == "school") {
                    var SelectedSchoolControl = $find("<%= cmbTeacher.ComboBox.ClientID %>");

                    var SelectedSchoolitem = SelectedSchoolControl.findItemByText(selectedSchool);

                    if (SelectedSchoolitem) {
                        SelectedSchoolitem.select();
                    }

                }
                else if (viewByValue.toLowerCase() == "demographics") {
                    if (selectedSchool != null) {
                        var selectedSchoolVal = getCriteriaValue("School");
                        getTeacherListForSchool(selectedSchoolVal);
                        showDropdown("TeacherName");
                        showDropdown("Class");

                        var SelectedSchoolControl = $find("<%= cmbTeacher.ComboBox.ClientID %>");

                        var SelectedSchoolitem = SelectedSchoolControl.findItemByText(selectedSchool);

                        if (SelectedSchoolitem) {
                            SelectedSchoolitem.select();
                        }

                    }
                }

                else if (viewByValue.toLowerCase() == "class") {
                    if (selectedSchool != null) {
                        showDropdown("TeacherName");
                        showDropdown("Class");
                        var selectedSchoolVal = getCriteriaValue("School");
                        getTeacherListForSchool(selectedSchoolVal);

                        var SelectedSchoolControl = $find("<%= cmbTeacher.ComboBox.ClientID %>");

                        var SelectedSchoolitem = SelectedSchoolControl.findItemByText(selectedSchool);

                        if (SelectedSchoolitem) {
                            SelectedSchoolitem.select();
                        }

                        if (selectedTeacherValue != null) {
                            getClassListBySchoolAndTeacher(selectedSchoolValue, selectedTeacherValue);
                        }

                    }
                }
                else if (viewByValue.toLowerCase() == "student") {
                    if (selectedSchool != null) {
                        showDropdown("TeacherName");
                        showDropdown("Class");
                        showDropdown("Student");
                        var selectedSchoolVal = getCriteriaValue("School");
                        getTeacherListForSchool(selectedSchoolVal);

                        var SelectedSchoolControl = $find("<%= cmbTeacher.ComboBox.ClientID %>");

                       var SelectedSchoolitem = SelectedSchoolControl.findItemByText(selectedSchool);

                       if (SelectedSchoolitem) {
                           SelectedSchoolitem.select();
                       }
                       if (selectedTeacherValue != null) {
                           getClassListBySchoolAndTeacher(selectedSchoolValue, selectedTeacherValue);
                       }

                   }
               }
}

    getteacherDisabled();
}


function CmbTeacherNameChanged() {
    hideDropdown("Class");
    var viewByValue = getCriteriaText("ViewBy");
    var selectedSchool = getCriteriaValue("School");
    var selectedTeacherName = getCriteriaValue("TeacherName");



    if (!stringIsEmpty(viewByValue)
        && !stringIsEmpty(selectedSchool)
       && !stringIsEmpty(selectedTeacherName)) {


        if (viewByValue.toLowerCase() == "student") {
            getStudentListBySchoolAndTeacher(selectedSchool, selectedTeacherName);
            getClassListBySchoolAndTeacher(selectedSchool, selectedTeacherName);
            showDropdown("Class");
            showDropdown("Student");
        }
        if (viewByValue.toLowerCase() == "class") {
            getClassListBySchoolAndTeacher(selectedSchool, selectedTeacherName);
            showDropdown("Class");
        }
        if (viewByValue.toLowerCase() == "demographics") {
            getClassListBySchoolAndTeacher(selectedSchool, selectedTeacherName);
            showDropdown("Class");
        }
        var currentValue = getCriteriaText("StandardList");
        UpdateTeacherSchoolvalue();
        if (currentValue && currentValue.toLowerCase() == "competency worksheet") {
            showDropdown("Teacher");
        }
        else {
            hideDropdown("Teacher");
        }

    }
}

function CmbTeacherChanged() {

    var viewByValue = getCriteriaText("ViewBy");
    var selectedSchool = getCriteriaValue("School");
    var selectedTeacher = getCriteriaValue("Teacher");
    var standardListValue = getCriteriaText("StandardList");
    if (!stringIsEmpty(standardListValue)
       && !stringIsEmpty(selectedTeacher)) {
        var teacherName = getCriteriaText("Teacher");

    }

}

function CmbClassChanged() {
    //hideDropdown("Student");
    CriteriaController.RemoveAll("Student");
    $find("<%= cmbStudent.ComboBox.ClientID %>").clearItems();

    var viewByValue = getCriteriaText("ViewBy");
    var selectedSchool = getCriteriaValue("School");
    var selectedTeacherName = getCriteriaValue("TeacherName");
    var selectedClass = getCriteriaValue("Class");

    if (!stringIsEmpty(viewByValue)
        && !stringIsEmpty(selectedSchool)
        && !stringIsEmpty(selectedTeacherName) && !stringIsEmpty(selectedClass)) {

        if (viewByValue.toLowerCase() == "student") {
            getStudentListBySchoolTeacherAndClass(selectedSchool, selectedTeacherName, selectedClass);
            //  getClassListBySchoolAndTeacher(selectedSchool, selectedTeacher);
            showDropdown("Student");
            // DynamicRequiredFieldAddition();
        }
        if (viewByValue.toLowerCase() == "class") {
            getClassListBySchoolAndTeacher(selectedSchool, selectedTeacherName);
            showDropdown("Class");
        }
    }
}

function CmbViewByChanged() {

    hideDropdown("School");
    hideDropdown("Teacher");
    hideDropdown("Class");
    hideDropdown("TeacherName");
    hideDropdown("Student");
    ResetCriteria();

    var currentValue = getCriteriaText("ViewBy");
    var selectedSchool = getCriteriaValue("School");
    var selectedTeacher = getCriteriaValue("Teacher");
    var selectedClass = getCriteriaValue("Class");


    if (!stringIsEmpty(currentValue)) {
        hideSection("topSection");
        showDropdown("ViewBy");
        switch (currentValue.toLowerCase()) {
            case "district":
                var SelectedSchool = $find("<%= cmbTeacher.ComboBox.ClientID %>");
                SelectedSchool.clearSelection();
                var SelectedTeacher = $find("ctl00_LeftColumnContentPlaceHolder_cmbTeacher_ddlTeacher");
                SelectedTeacher.clearSelection();
                break;
            case "school":

                showDropdown("School");
                selectComboBoxItemsCountIsOne();

                break;
            case "teacher":

                showDropdown("School");
                selectComboBoxItemsCountIsOne();
                showDropdown("TeacherName");
                selectedSchool = getCriteriaValue("School");
                //if (!stringIsEmpty(selectedSchool)) {
                //    showDropdown("Teacher");
                //}
                hideDropdown("Class");

                break;
            case "class":
                showDropdown("School");
                selectComboBoxItemsCountIsOne();
                showDropdown("TeacherName");
                showDropdown("Class");

                selectedSchool = getCriteriaValue("School");
                if (!stringIsEmpty(selectedSchool)) {
                    //showDropdown("Teacher");
                    showDropdown("Class");
                }
                selectedTeacher = getCriteriaValue("Teacher");
                if (!stringIsEmpty(selectedSchool)
                    && !stringIsEmpty(selectedTeacher)) {
                    showDropdown("Class");
                }

                break;
            case "student":
                showDropdown("School");
                selectComboBoxItemsCountIsOne();
                showDropdown("TeacherName");
                showDropdown("Class");
                showDropdown("Student");

                if (!stringIsEmpty(selectedSchool)) {
                    showDropdown("TeacherName");
                    showDropdown("Class");
                    showDropdown("Student");
                }

                if (!stringIsEmpty(selectedSchool)
                    && !stringIsEmpty(selectedTeacher)) {
                    showDropdown("Class");
                }
                if (!stringIsEmpty(selectedSchool)
                    && !stringIsEmpty(selectedTeacher) && !stringIsEmpty(selectedClass)) {
                    showDropdown("Student");
                }

                break;
            case "demographics":
                showDropdown("School");
                selectComboBoxItemsCountIsOne();
                showDropdown("TeacherName");
                showDropdown("Class");
                showDropdown("Group");

                $("#criteriaHeaderDiv_TeacherName").find("div.left").html('Teacher:');

                if (!stringIsEmpty(selectedSchool)) {
                    showDropdown("TeacherName");
                }

                if (!stringIsEmpty(selectedSchool)
                    && !stringIsEmpty(selectedTeacher)) {
                    showDropdown("Class");
                }

                break;
            case "group":
                showDropdown("Group");

                break;
            default:
        }
        DynamicRequiredFieldAddition();
        getteacherEnabled();

    }


}
function selectComboBoxItemsCountIsOne() {

    var finalcontrolId = $find("<%= cmbSchool.ComboBox.ClientID %>");

    if (finalcontrolId.get_items().get_count() == 1) {
        var schoolCmbItemText = finalcontrolId.get_items(0)._array[0]._text;

        var schoolcomboitem = finalcontrolId.findItemByText(schoolCmbItemText);
        if (schoolcomboitem) {
            schoolcomboitem.select();

        }
    }

}

function DynamicRequiredFieldAddition() {

    $.each(CriteriaController.FindNode("ViewBy").Values, function (index, selectedItem) {

        if (selectedItem.CurrentlySelected) {
            var currentValue = selectedItem.Value.Text;

            if (currentValue == "Teacher" || currentValue == "Class" || currentValue == "Student" || currentValue == "School") {
                $("#criteriaHeaderDiv_School").find("div.left").html('School:');
                $("#criteriaHeaderDiv_School").find("div.left").append('<spam style="color:red">*</spam>');
                $("#criteriaHeaderDiv_School").find("div.left").attr('Required', true);

                if (currentValue === "Class" || currentValue === "Student" || currentValue === "Teacher") {
                    $("#criteriaHeaderDiv_Teacher").find("div.left").html('Teacher:');
                    $("#criteriaHeaderDiv_Teacher").find("div.left").append('<spam style="color:red">*</spam>');
                    $("#criteriaHeaderDiv_Teacher").find("div.left").attr('Required', true);

                    $("#criteriaHeaderDiv_TeacherName").find("div.left").html('Teacher:');
                    $("#criteriaHeaderDiv_TeacherName").find("div.left").append('<spam style="color:red">*</spam>');
                    $("#criteriaHeaderDiv_TeacherName").find("div.left").attr('Required', true);
                }
                else {

                    $("#criteriaHeaderDiv_Teacher").find("div.left").html('Teacher:');
                    $("#criteriaHeaderDiv_TeacherName").find("div.left").html('Teacher:');

                }
            }
            else {
                $("#criteriaHeaderDiv_School").find("div.left").html('School:');

            }
            if (currentValue === "Group") {
                $("#criteriaHeaderDiv_Group").find("div.left").html('Group:');
                $("#criteriaHeaderDiv_Group").find("div.left").append('<spam style="color:red">*</spam>');
                $("#criteriaHeaderDiv_Group").find("div.left").attr('Required', true);
            }
            else {
                $("#criteriaHeaderDiv_Group").find("div.left").html('Group:');
            }
            if (currentValue === "Demographics") {
                $("#criteriaHeaderDiv_Demographic").find("div.left").html('Demographic:');
                $("#criteriaHeaderDiv_Demographic").find("div.left").append('<spam style="color:red">*</spam>');
                $("#criteriaHeaderDiv_Demographic").find("div.left").attr('Required', true);
            }
            else {
                $("#criteriaHeaderDiv_Demographic").find("div.left").html('Demographic:');
            }
            if (currentValue === "Student") {
                $("#criteriaHeaderDiv_Student").find("div.left").html('Student:');
                $("#criteriaHeaderDiv_Student").find("div.left").append('<spam style="color:red">*</spam>');
                $("#criteriaHeaderDiv_Student").find("div.left").attr('Required', true);
            }
            else {
                $("#criteriaHeaderDiv_Student").find("div.left").html('Student:');
            }
            if (currentValue === "Class") {
                $("#criteriaHeaderDiv_Class").find("div.left").html('Class:');
                $("#criteriaHeaderDiv_Class").find("div.left").append('<spam style="color:red">*</spam>');
                $("#criteriaHeaderDiv_Class").find("div.left").attr('Required', true);
            }
            else {
                $("#criteriaHeaderDiv_Class").find("div.left").html('Class:');
            }
            //if (currentValue === "Teacher") {
            //    $("#criteriaHeaderDiv_Teacher").find("div.left").html('Teacher:');
            //    $("#criteriaHeaderDiv_Teacher").find("div.left").append('<spam style="color:red">*</spam>');
            //    $("#criteriaHeaderDiv_Teacher").find("div.left").attr('Required', true);
            //}
            //else {

            //    $("#criteriaHeaderDiv_Teacher").find("div.left").html('Teacher:');

            //}

        }
    });
}

function DynamicRequiredFieldForStandardList() {
    $.each(CriteriaController.FindNode("StandardList").Values, function (index, selectedItem) {

        if (selectedItem.CurrentlySelected) {
            var currentValue = selectedItem.Value.Text;


            if (currentValue === "Competency Worksheet") {
                $("#criteriaHeaderDiv_WorksheetSelection").find("div.left").html('List Selection:');
                $("#criteriaHeaderDiv_WorksheetSelection").find("div.left").append('<spam style="color:red">*</spam>');
                $("#criteriaHeaderDiv_WorksheetSelection").find("div.left").attr('Required', true);

                $("#criteriaHeaderDiv_Teacher").find("div.left").html('Teacher:');
                $("#criteriaHeaderDiv_Teacher").find("div.left").append('<spam style="color:red">*</spam>');
                $("#criteriaHeaderDiv_Teacher").find("div.left").attr('Required', true);
            }
            else {
                $("#criteriaHeaderDiv_WorksheetSelection").find("div.left").html('List Selection:');
                $("#criteriaHeaderDiv_Teacher").find("div.left").html('Teacher:');
            }

            if (currentValue === "Competency List") {
                $("#criteriaHeaderDiv_ListSelection").find("div.left").html('List Selection:');
                $("#criteriaHeaderDiv_ListSelection").find("div.left").append('<spam style="color:red">*</spam>');
                $("#criteriaHeaderDiv_ListSelection").find("div.left").attr('Required', true);


            }
            else {
                $("#criteriaHeaderDiv_ListSelection").find("div.left").html('List Selection:');
            }

            if (currentValue === "Standard Set") {
                $("#criteriaHeaderDiv_StandardSet").find("div.left").html('Standard Set:');
                $("#criteriaHeaderDiv_StandardSet").find("div.left").append('<spam style="color:red">*</spam>');
                $("#criteriaHeaderDiv_StandardSet").find("div.left").attr('Required', true);


            }
            else {
                $("#criteriaHeaderDiv_StandardSet").find("div.left").html('Standard Set:');


            }



        }
    });
}





function UpdateTeacherSchoolvalue() {

    getteacherEnabled();
    var ViewBycombo = $find("ctl00_LeftColumnContentPlaceHolder_cmbTeacher_ddlTeacher");
    var selectedTeacher = getCriteriaText("TeacherName");

    var StandardListcomboitem = ViewBycombo.findItemByText(selectedTeacher);
    if (StandardListcomboitem) {
        StandardListcomboitem.select();
    }

    var ViewBycombo = $find("ctl00_LeftColumnContentPlaceHolder_cmbTeacher_ddlSchool");
    var selectedSchool = getCriteriaText("SchoolName");

    var Schoolcomboitem = ViewBycombo.findItemByText(selectedSchool);
    if (Schoolcomboitem) {
        Schoolcomboitem.select();
    }

    if (selectedTeacher) {

        getteacherDisabled();
    }

}
function ResetStandardListCriteria() {


    //  CriteriaController.RemoveAll("Teacher");
    CriteriaController.RemoveAll("StandardLevel");
    $find("<%= cmbStandardLevel.ComboBox.ClientID %>").clearItems();
    CriteriaController.RemoveAll("DateRange");

}

        function CmbStandardListChanged() {
            //debugger;

            hideDropdown("ListSelection");
            hideDropdown("WorksheetSelection");
            hideDropdown("Teacher");
            hideDropdown("StandardSet");
            hideDropdown("StandardLevel");
            showDropdown("DateRange");
            ResetStandardListCriteria();
            try {

                var currentValue = getCriteriaText("StandardList");
                switch (currentValue.toLowerCase()) {
                    case "competency worksheet":

                        $("#hdnSelectedStanderd").val("Competency WorkSheet");
                        hideDropdown("ListSelection");
                        showDropdown("WorksheetSelection");
                        showDropdown("Teacher");

                        UpdateTeacherSchoolvalue();
                        CriteriaController.RemoveAll("ListSelection");
                        $find("<%= cmbCompentencyList.ComboBox.ClientID %>").clearItems();
                CriteriaController.RemoveAll("StandardSet");
                break;
            case "competency list":

                showDropdown("ListSelection");
                hideDropdown("WorksheetSelection");
                CriteriaController.RemoveAll("StandardSet");
                CriteriaController.RemoveAll("WorksheetSelection");
                $find("<%= cmbCompentencyWorksheet.ComboBox.ClientID %>").clearItems();
                break;
            case "standard set":
                showDropdown("StandardSet");
                CriteriaController.RemoveAll("ListSelection");
                CriteriaController.RemoveAll("WorksheetSelection");
                $find("<%= cmbCompentencyList.ComboBox.ClientID %>").clearItems();
                $find("<%= cmbCompentencyWorksheet.ComboBox.ClientID %>").clearItems();
                StandardSetController.Clear();
                break;
            default:
        }
        DynamicRequiredFieldForStandardList();

    }
    catch (ex) {
        currentValue = "";
    }




}

function getCompetencyList(resourceToShow) {
    $.ajax({
        type: "POST",
        url: "./CompetencyTrackingReportPage.aspx/GetCompetencyList",
        //data: "{'resourceToShow':" + resourceToShow + "}",

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus + "\n" + errorThrown);
        },
        success: function (result) {
            var data = [];
            var data1 = [];
            data = JSON.parse(result.d);
            //data = result.d;

            for (var i = 0; i < data.length; i++) {
                data1.push([data[i].DocumentId, data[i].FriendlyName]);
            }
            ListSelectionController.PopulateList(data1, 1, 0);
        }
    });
}

function getTeacherListForSchool(schoolId) {
    $.ajax({
        type: "POST",
        url: "./CompetencyTrackingReportPage.aspx/GetTeacherListForSchool",
        data: "{'schoolID':" + schoolId + "}",

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(textStatus + "\n" + errorThrown);
        },
        success: function (result) {
            var data = [];
            var data1 = [];
            data = JSON.parse(result.d);

            for (var i = 0; i < data.length; i++) {
                data1.push([data[i].TeacherName, data[i].TeacherPage]);
            }
            TeacherNameController.PopulateList(data1, 0, 1);
        }
    });
}

function getStudentListBySchoolAndTeacher(schoolId, teacherPage) {
    $.ajax({
        type: "POST",
        url: "./CompetencyTrackingReportPage.aspx/GetStudentListBySchoolAndTeacher",
        data: "{'schoolID':" + schoolId + ", 'teacherPage':" + teacherPage + "}",

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(textStatus + "\n" + errorThrown);
        },
        success: function (result) {
            var data = [];
            var data1 = [];
            data = JSON.parse(result.d);

            for (var i = 0; i < data.length; i++) {
                data1.push([data[i].StudentName, data[i].StudentID]);
            }
            StudentController.PopulateList(data1, 0, 1);
        }
    });
}

function getClassListBySchoolAndTeacher(schoolId, teacherId) {
    $.ajax({
        type: "POST",
        url: "./CompetencyTrackingReportPage.aspx/GetClassListBySchoolAndTeacher",
        data: "{'schoolId':" + schoolId + ", 'teacherId':" + teacherId + "}",

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(textStatus + "\n" + errorThrown);
        },
        success: function (result) {
            var data = [];
            var data1 = [];
            data = JSON.parse(result.d);

            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    data1.push([data[i].FriendlyName, data[i].ID]);
                }
            } else {
                //No classes found, do something?
            }
            ClassController.PopulateList(data1, 0, 1);
        }
    });
}

function getStudentListBySchoolTeacherAndClass(schoolId, teacherPage, classId) {
    $.ajax({
        type: "POST",
        url: "./CompetencyTrackingReportPage.aspx/GetStudentListBySchoolTeacherAndClass",
        data: "{'schoolID':" + schoolId + ", 'teacherPage':" + teacherPage + ", 'classId':" + classId + "}",

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(textStatus + "\n" + errorThrown);
        },
        success: function (result) {
            var data = [];
            var data1 = [];
            data = JSON.parse(result.d);

            for (var i = 0; i < data.length; i++) {
                data1.push([data[i].StudentName, data[i].StudentID]);
            }
            StudentController.PopulateList(data1, 0, 1);
        }
    });
}

function getListSelectionForTeacher(teacherName) {
    $.ajax({
        type: "POST",
        url: "./CompetencyTrackingReportPage.aspx/PopulateListSelectionDropdownByTeacher",
        data: "{'teacherName':'" + teacherName + "'}",

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(textStatus + "\n" + errorThrown);
        },
        success: function (result) {
            var data = [];
            var data1 = [];
            data = JSON.parse(result.d);

            for (var i = 0; i < data.length; i++) {
                data1.push([data[i].Name, data[i].ID]);
            }
            ListSelectionController.PopulateList(data1, 0, 1);
        }
    });
}
function BindDataOnDemand(sender, eventArgs) {
    var context = eventArgs.get_context();
    context["StandardSet"] = $find('ddlCriteriaStandardSet').get_value();
    context["StandardList"] = getCriteriaValue("StandardList");
    var listSelected = "";
    if (context["StandardList"] == "9") {
        listSelected = getCriteriaValue("ListSelection");
    }
    else if (context["StandardList"] == "8") {
        listSelected = getCriteriaValue("WorksheetSelection");
    }

    context["ListSelection"] = listSelected;
    if (getCriteriaValue("Teacher") != null) context["TeacherID"] = getCriteriaValue("Teacher"); else context["TeacherID"] = 0;

    $find("<%= cmbStandardLevel.ComboBox.ClientID %>").clearItems();
    showDropdown("StandardLevel");

}

function ListWorksheetChanged()
{
    CriteriaController.RemoveAll("StandardLevel");
    $find("<%= cmbStandardLevel.ComboBox.ClientID %>").clearItems();
}

function SetValue(sender, args) {
    sender.clearItems();
}


    </script>

    <style type="text/css">
.criteriaText
{
    color:black !important;
    font-style:italic;
    padding-bottom:5px;
}
#wrapperColumnRight
{
    position:static !important;
}
.criteriaText {
    float:none !important;
  word-wrap: break-word;
  padding-bottom:5px;
 
}
.RadComboBoxDropDown .rcbScroll
{
   min-height:40px !important;
}
    </style>
</asp:Content>
