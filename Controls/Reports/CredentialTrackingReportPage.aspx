<%@ Page Title="Credential Tracking Report Portal" Language="C#" MasterPageFile="~/Search.Master" AutoEventWireup="true" CodeBehind="CredentialTrackingReportPage.aspx.cs" Inherits="Thinkgate.Controls.Reports.CredentialTrackingReportPage" %>

<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="Demographics" Src="~/Controls/E3Criteria/Demographics.ascx" %>
<%@ Register TagPrefix="e3" TagName="DateRange" Src="~/Controls/E3Criteria/DateRange.ascx" %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx" %>
<%@ Register TagPrefix="e3" TagName="TreeDropDownList" Src="~/Controls/E3Criteria/TreeDropDownList.ascx" %>

<%@ MasterType VirtualPath="~/Search.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:DropDownList ID="cmbViewBy" CriteriaName="ViewBy" runat="server" Text="View By" EmptyMessage="Select View" OnChange="CmbViewByChanged()" Required="True" />
    <e3:DropDownList ID="cmbSchool" CriteriaName="School" runat="server" Text="School" EmptyMessage="Select School" OnChange="CmbSchoolChanged()" />
    <e3:DropDownList ID="cmbTeacher" CriteriaName="Teacher" runat="server" Text="Teacher" EmptyMessage="Select Teacher" OnChange="CmbTeacherChanged()" />
    <e3:DropDownList ID="cmbClass" CriteriaName="Class" runat="server" Text="Class" EmptyMessage="Select Class" OnChange="CmbClassChanged()" />
    <e3:DropDownList ID="cmbStudent" CriteriaName="Student" runat="server" Text="Student" EmptyMessage="Select Student" />

    <e3:Demographics ID="demos" CriteriaName="Demographics" runat="server" Text="Demographics" />
    <e3:DropDownList ID="cmbGroup" CriteriaName="Group" runat="server" Text="Group" EmptyMessage="Select Group" />

    <e3:TreeDropDownList ID="cmbalignment" CriteriaName="Alignment" runat="server" Text="Alignment" />
    <e3:DropDownList ID="cmbyear" CriteriaName="Year" runat="server" Text="Year"  EmptyMessage="Select Year" Required="True"/>

    
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <div id="divExportOptions" style="text-align: left">
        <asp:ImageButton runat="server" ID="btnExportExcel" OnClick="btnExportExcel_Click"
            ImageUrl="~/Images/Toolbars/excel_button.png" />
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
   <style type="text/css">
       .RadGrid_Transparent .rgHeader {
          color: #fff !important;
          font-weight: normal !important;
            }
       .RadTreeView_Transparent .rtHover .rtIn {
	        background-color: #f99547 !important;
            background-image: none !important;
        }
       .RadGrid_Transparent .rgHeader, .RadGrid_Transparent th.rgResizeCol, .RadGrid_Transparent .rgHeaderWrapper
       {
           /*background:none repeat scroll 0 0 #4066b6 !important;*/
           background:0-2300px repeat-x #7fa5d7 url('/Zeus/WebResource.axd?d=M_gmP0-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418889137483158')
       }
       .rddtSlide{
           margin-top: 10px !important;
       }
   </style>
    <div id="divDefaultMessage" style="height: 100%; text-align: center; clear: both;" runat="server" visible="false">
        <div style="height: 40%"></div>
        <div style="height: 20%">
            <strong>Please select criteria for all required  (indicated by <span style="color: red; font-weight: bold">*</span>)</strong>
            <br />
            <strong>then select Search.</strong>


        </div>
        <div style="height: 40%"></div>
    </div>

    <div id="DivBlackMsg" runat="server"><span style="color: gray; text-align: left; margin-top: 20px; font-weight: 200; float: left; margin-right: 3%">No records to display.</span></div>
    <asp:Panel runat="server" ID="gridResultsPanel">
        <telerik:RadGrid ID="radGridResults" runat="server" AutoGenerateColumns="false" Width="100%" Height="509px" AllowAutomaticUpdates="True" AllowPaging="true"
            OnItemDataBound="radGridResults_ItemDataBound"  OnPreRender="radGridResults_PreRender"           
            OnItemCommand="radGridResults_ItemCommand" PageSize="20" Skin="Web20"
             OnPageIndexChanged="RadGridResults_PageIndexChanged"            
            >
               <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" ></PagerStyle>
            <ClientSettings EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="false" />
                <Scrolling AllowScroll="True" SaveScrollPosition="True"
                    UseStaticHeaders="True" />
                <Resizing AllowColumnResize="True" />
            </ClientSettings>
            <HeaderStyle Font-Bold="true" CssClass="gridcolumnheaderstyle" />
            <MasterTableView>
                <Columns>
                    <telerik:GridBoundColumn DataField="LevelID" Display="false" ReadOnly="True" UniqueName="LevelID" />
                    <telerik:GridBoundColumn DataField="CredentialID" Display="false" ReadOnly="True" />
                    <telerik:GridBoundColumn HeaderText="LevelName"  DataField="LevelName" UniqueName="LevelName" />                    
                    <telerik:GridTemplateColumn HeaderText="Credential Name" ItemStyle-HorizontalAlign="Left" ItemStyle-CssClass="gridcolumnstyle">
                        <ItemTemplate>
                            <%# Eval("CredentialName") %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Student Count" ItemStyle-HorizontalAlign="center" ItemStyle-Wrap="false" HeaderStyle-Wrap="false"
                        ItemStyle-Width="70px" HeaderStyle-Width="100px" ItemStyle-CssClass="gridcolumnstyle"  UniqueName="StudentCount">
                        <ItemTemplate>
                            <asp:LinkButton Visible='<%# _viewBy.Equals("Student")? false:true %>' ID="lnkbStudentCount" runat="server" ForeColor="Blue" Text='<%# Convert.ToInt32(Eval("StudentCount")) > 0?Eval("StudentCount"):"" %>'
                                CommandName="StudentCount" CommandArgument='<%# Eval("LevelID") + ","+ Eval("CredentialID")+","+Eval("CredentialName") %>'>' ></asp:LinkButton>
                            <span runat="server" visible='<%# _viewBy.Equals("Student")? true:false %>' id="spnEarnedDate" >
                                <%# String.Format("{0:MM/dd/yyyy}", Eval("EarnedDate") ) %>
                            </span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        
        <asp:PlaceHolder ID="initialDisplayText" runat="server" Visible="true">
            <div id="lblInitialText" runat="server" style="font-size: 11pt; text-align: center; position: absolute; width: 100%; top: 25%; margin-left: auto; margin-right: auto">
                <b>Please choose criteria and select Search.</b><br />
                (<b>View By</b> is a required field)
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
        if (BrowserDetect.browser == "Explorer") {
            //Sys.Application.add_load(tempSetInitialComboBoxItems);
        }
        //

        $(document).ready(function () {
            resetCriteriaSections();
            resizeContent();

            //This is temporary
            // Non-IE fire here
            if (BrowserDetect.browser != "Explorer") {
                // tempSetInitialComboBoxItems();
            }
            //
        });

        $(window).resize(function () {
            resizeContent();
        });

        function TreeListCreated() {
            resizeContent();
        }



        function resizeContent() {
            $(".rtlDataDiv").height($(window).height() - $("#divExportOptions").height() - $(".rtlHeader").height() - 12);
        }

        function tempSetInitialComboBoxItems() {
            var finalcontrolId = $find("<%= cmbyear.ComboBox.ClientID %>");
            if (finalcontrolId.get_items().get_count() > 0) {
                var totalcount = finalcontrolId.get_items().get_count();
                var cmbyearItemText = finalcontrolId.get_items(totalcount - 1)._array[totalcount - 1]._text;

                var cmbyearItem = finalcontrolId.findItemByText(cmbyearItemText);
                if (cmbyearItem) {
                    cmbyearItem.select();

                }
            }
        }

        function EntryAdded(sender, args) {
            var dropDown = sender;
            if (dropDown) {
                setTimeout(function () {
                    dropDown.closeDropDown();
                    <%= this.ClientScript.GetPostBackEventReference(this.cmbalignment, "OnClientEntryAdded") %>
                }, 100);
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

        function selectTeacherComboBoxItemsCountIsOne() {
            var TeacherfinalcontrolId = $find("<%= cmbTeacher.ComboBox.ClientID %>");

            if (TeacherfinalcontrolId.get_items().get_count() == 1) {
                var TeacherCmbItemText = TeacherfinalcontrolId.get_items(0)._array[0]._text;
                var Teachercomboitem = TeacherfinalcontrolId.findItemByText(TeacherCmbItemText);
                if (Teachercomboitem) {
                    Teachercomboitem.select();
                }

            }
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

        var TopSectionDropDownArray = ["ViewBy", "School", "Teacher", "Class", "Student", "Demographics", "Group"];
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

            //Hide all
            hideSection("topSection");

            return;
        };

        function showDropdown1(theDropdownName) {
            $("#criteriaHeaderDiv_" + theDropdownName).show();
            $("#selectedCritieriaDisplayArea_" + theDropdownName).show();

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

            CriteriaController.RemoveAll("School");
            hideDropdown("School");
            initCriteriaSections();
        }

        function initCriteriaSections() {
            hideSection("topSection");
            showDropdown("ViewBy");

        }

        function openStudentCount(studentCountWindowTitle) {
            customDialog({
                url: '../Reports/CredentialTrackingStudentCount.aspx', maximize: true, maxwidth: 1300, maxheight: 1100, title: studentCountWindowTitle
            });
        }

        function ResetCriteria() {
            CriteriaController.RemoveAll("School");
            CriteriaController.RemoveAll("Teacher");
            CriteriaController.RemoveAll("Class");
            CriteriaController.RemoveAll("Student");
            DemographicsController.Clear();
            CriteriaController.RemoveAll("Group");
            CriteriaController.UpdateCriteriaForSearch();

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

        function CmbSchoolChanged() {
            hideDropdown("Teacher");
            hideDropdown("Class");

            var viewByValue = getCriteriaValue("ViewBy");
            var selectedSchool = getCriteriaValue("School");
            var selectedTeacher = getCriteriaValue("Teacher");

            if (!stringIsEmpty(viewByValue) && !stringIsEmpty(selectedSchool)) {
                if (viewByValue.toLowerCase() == "teacher" || viewByValue.toLowerCase() == "class" || viewByValue.toLowerCase() == "student" || viewByValue.toLowerCase() == "demographics") {
                    if (selectedSchool != null) {
                        getTeacherListForSchool(selectedSchool);
                        showDropdown1("Teacher");

                    }
                }
            }

            if (!stringIsEmpty(viewByValue)
                && !stringIsEmpty(selectedSchool)
                && !stringIsEmpty(selectedTeacher)) {
                showDropdown("Class");
            }
        }

        function CmbTeacherChanged() {
            hideDropdown("Class");

            var viewByValue = getCriteriaValue("ViewBy");
            var selectedSchool = getCriteriaValue("School");
            var selectedTeacher = getCriteriaValue("Teacher");

            if (!stringIsEmpty(viewByValue)
                && !stringIsEmpty(selectedSchool)
                && !stringIsEmpty(selectedTeacher)) {

                if (viewByValue.toLowerCase() == "student") {
                    getStudentListBySchoolAndTeacher(selectedSchool, selectedTeacher);
                    getClassListBySchoolAndTeacher(selectedSchool, selectedTeacher);
                    showDropdown("Class");
                }
                if (viewByValue.toLowerCase() == "class") {
                    getClassListBySchoolAndTeacher(selectedSchool, selectedTeacher);
                    showDropdown("Class");
                }
                if (viewByValue.toLowerCase() == "demographics") {
                    getClassListBySchoolAndTeacher(selectedSchool, selectedTeacher);
                    showDropdown("Class");
                }
            }
        }

        function CmbClassChanged() {
            hideDropdown("Student");

            var viewByValue = getCriteriaValue("ViewBy");
            var selectedSchool = getCriteriaValue("School");
            var selectedTeacher = getCriteriaValue("Teacher");
            var selectedClass = getCriteriaValue("Class");

            if (!stringIsEmpty(viewByValue)
                && !stringIsEmpty(selectedSchool)
                && !stringIsEmpty(selectedTeacher) && !stringIsEmpty(selectedClass)) {

                if (viewByValue.toLowerCase() == "student") {
                    getStudentListBySchoolTeacherAndClass(selectedSchool, selectedTeacher, selectedClass);
                    getClassListBySchoolAndTeacher(selectedSchool, selectedTeacher);
                    showDropdown("Student");
                }
                if (viewByValue.toLowerCase() == "class") {
                    getClassListBySchoolAndTeacher(selectedSchool, selectedTeacher);
                    showDropdown("Class");
                }
            }
        }

        function CmbViewByChanged() {
            
            hideDropdown("School");
            hideDropdown("Teacher");
            hideDropdown("Class");
            hideDropdown("Group");
            ResetCriteria();

            var currentValue = getCriteriaValue("ViewBy");
            var selectedSchool = getCriteriaValue("School");
            var selectedTeacher = getCriteriaValue("Teacher");

            if (!stringIsEmpty(currentValue)) {
                hideSection("topSection");
                showDropdown("ViewBy");
                showDropdown("Demographics");
                switch (currentValue.toLowerCase()) {
                    case "district":

                        break;
                    case "school":

                        showDropdown("School");
                        selectComboBoxItemsCountIsOne();

                        break;
                    case "teacher":

                        showDropdown("School");
                        selectComboBoxItemsCountIsOne();
                        selectedSchool = getCriteriaValue("School");
                        if (!stringIsEmpty(selectedSchool)) {
                            showDropdown("Teacher");
                        }
                        hideDropdown("Class");

                        break;
                    case "class":
                        showDropdown("School");
                        selectComboBoxItemsCountIsOne();
                        selectedSchool = getCriteriaValue("School");
                        if (!stringIsEmpty(selectedSchool)) {
                            showDropdown("Teacher");

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

                        if (!stringIsEmpty(selectedSchool)) {
                            showDropdown("Teacher");

                        }

                        if (!stringIsEmpty(selectedSchool)
                            && !stringIsEmpty(selectedTeacher)) {
                            showDropdown("Class");
                        }

                        break;
                    case "demographics":
                        showDropdown("School");
                        showDropdown("Group");

                        if (!stringIsEmpty(selectedSchool)) {
                            showDropdown("Teacher");
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
            }
        }

        /*This function is used to Add Dynamic Required Field*/


        function DynamicRequiredFieldAddition() {

            $.each(CriteriaController.FindNode("ViewBy").Values, function (index, selectedItem) {

                if (selectedItem.CurrentlySelected) {
                    var currentValue = selectedItem.Value.Text;

                    if (currentValue == "Teacher" || currentValue == "Class" || currentValue == "Student") {
                        $("#criteriaHeaderDiv_School").find("div.left").html('School:');
                        $("#criteriaHeaderDiv_School").find("div.left").append('<spam style="color:red">*</spam>');
                        $("#criteriaHeaderDiv_School").find("div.left").attr('Required', true);

                        if (currentValue === "Class" || currentValue === "Student") {
                            $("#criteriaHeaderDiv_Teacher").find("div.left").html('Teacher:');
                            $("#criteriaHeaderDiv_Teacher").find("div.left").append('<spam style="color:red">*</spam>');
                            $("#criteriaHeaderDiv_Teacher").find("div.left").attr('Required', true);
                        }
                        else {

                            $("#criteriaHeaderDiv_Teacher").find("div.left").html('Teacher:');

                        }
                    }
                    else {
                        $("#criteriaHeaderDiv_School").find("div.left").html('School:');

                    }
                }
            });
        }


        function CmbStandardListChanged() {
            try {
                //var currentValue = CriteriaController.CriteriaNodes[0].Values[0].Value.Value;
                var currentValue = CriteriaController.FindNode("StandardList").Values[0].Value.Value;

                switch (currentValue.toLowerCase()) {
                    case "competencyworksheet":
                    case "competencylist":
                        //TODO: populate list selection dropdown
                        break;
                    case "standardset":
                        break;
                    default:
                }

            } catch (ex) {
                currentValue = "";
            }

            if (currentValue != null && currentValue != "") {
                hideSection("bottomSection");
                //Always show
                showDropdown("StandardList");
                showDropdown("DateRange");
                showDropdown("StandardLevel");
                switch (currentValue.toLowerCase()) {
                    case "competencyworksheet":
                    case "competencylist":
                        showDropdown("ListSelection");
                        break;
                    case "standardset":
                        showDropdown("StandardSet");
                        break;
                    default:
                }
            } else {
                //TODO: ???
            }
        }

        function getTeacherListForSchool(schoolId) {
            $.ajax({
                type: "POST",
                url: "./CredentialTrackingReportPage.aspx/GetTeacherListForSchool",
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
                    TeacherController.PopulateList(data1, 0, 1);
                    selectTeacherComboBoxItemsCountIsOne();
                }
            });
        }

        function getStudentListBySchoolAndTeacher(schoolId, teacherPage) {
            $.ajax({
                type: "POST",
                url: "./CredentialTrackingReportPage.aspx/GetStudentListBySchoolAndTeacher",
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

        function getStudentListBySchoolTeacherAndClass(schoolId, teacherPage, classId) {
            $.ajax({
                type: "POST",
                url: "./CredentialTrackingReportPage.aspx/GetStudentListBySchoolTeacherAndClass",
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


        function getClassListBySchoolAndTeacher(schoolId, teacherId) {
            $.ajax({
                type: "POST",
                url: "./CredentialTrackingReportPage.aspx/GetClassListBySchoolAndTeacher",
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
                            data1.push([data[i].ClassName, data[i].ClassID]);
                        }
                    } else {
                        //No classes found, do something?
                    }
                    ClassController.PopulateList(data1, 0, 1);
                }
            });
        }

    </script>
</asp:Content>
