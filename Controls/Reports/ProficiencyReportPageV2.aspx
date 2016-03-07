<%@ Page Title="Proficiency Report" Language="C#" MasterPageFile="~/Search.Master"
    AutoEventWireup="true" CodeBehind="ProficiencyReportPageV2.aspx.cs" Inherits="Thinkgate.Controls.Reports.ProficiencyReportPageV2" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" tagName="TestYearGradeSubject" Src="~/Controls/E3Criteria/TestYearGradeSubject.ascx"%>
<%@ MasterType virtualpath="~/Search.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results"/>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:TestYearGradeSubject ID="ctrlTestYearGradeSubject" CriteriaName="TestYearGradeSubject" runat="server"/>
    <e3:DropDownList ID="cmbSchool" CriteriaName="School" runat="server" Text="School" EmptyMessage="None"/>
    <e3:DropDownList ID="cmbTeacher" CriteriaName="Teacher" runat="server" Text="Teacher" EmptyMessage="None"/>
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <div id="divExportOptions" style="text-align: left">
        <asp:ImageButton runat="server" ID="btnExportExcel" OnClick="btnExportExcel_Click"
            ImageUrl="~/Images/Toolbars/excel_button.png" />
        <asp:ImageButton runat="server" ID="btnFLDOE" OnClientClick="window.open('http://www.fldoe.org/asp/k12memo/pdf/tngcbtf.pdf'); return false;"
                         ImageUrl="~/Images/Toolbars/info.png" Visible="False" ToolTip="Go to www.fldoe.org" />
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <div id="divProficiencyLevels" class="reportSection">
        <asp:Panel runat="server" ID="pnlProficiencyLevels" />
    </div>
    <div id="divDropdownMenus" class="reportSection">
        <telerik:RadComboBox ID="cmbGridDisplay" runat="server" ToolTip="Select a display option"
            Skin="Web20" OnSelectedIndexChanged="cmbGridDisplay_SelectedIndexChanged" AutoPostBack="true"
            CausesValidation="False" HighlightTemplatedItems="true" Visible="False">
            <Items>
                <telerik:RadComboBoxItem Value="Level" Text="Show Level Distribution" />
                <telerik:RadComboBoxItem Value="Score" Text="Show Average Scores" />
                <telerik:RadComboBoxItem Value="All" Text="Show All Statistics" />
            </Items>
        </telerik:RadComboBox>
        <telerik:RadComboBox ID="cmbDomain" runat="server" ToolTip="Select a domain" Skin="Web20" Visible="false"
            OnSelectedIndexChanged="cmbDomain_SelectedIndexChanged" AutoPostBack="true" CausesValidation="False"
            HighlightTemplatedItems="true">
            <ItemTemplate>
                <span>
                    <%# Eval("Domain") %></span>
            </ItemTemplate>
        </telerik:RadComboBox>
    </div>
    <div id="lblInitialText" runat="server" style="font-size: 11pt; text-align:center; position: absolute; width: 100%; top: 25%; margin-left: auto; margin-right: auto">Please select criteria for all required fields (Indicated by <span style="color: rgb(255, 0, 0); font-weight: bold;">*</span>)<br/> then Update Results.</div>
    <div id="divResults" class="reportSection" style="background-color: white">
        <telerik:RadGrid runat="server" ID="gridResults" AutoGenerateColumns="false" OnItemDataBound="gridResults_DataBound" Visible="False">
        </telerik:RadGrid>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            resizeContent();
        });
         
        $(window).resize(function () {
            resizeContent();
        });

        function TreeListCreated() {
            resizeContent();
        }

        function resizeContent() {
            $(".rtlDataDiv").height($(window).height() - $("#divExportOptions").height() - $(".rtlHeader").height()-12);
        }

        function ChangeSchoolBasedOnGrade() {
            var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Text", true);
            var filteredSchoolPos = CriteriaDataHelpers.GetFilteredDataPositions(SchoolDependencyData, [null, null, selectedGrades]);
            var filteredSchools = [];
            for (var j = 0; j < filteredSchoolPos.length; j++) {
                filteredSchools.push(SchoolDependencyData[filteredSchoolPos[j]]);
            }

            SchoolController.PopulateList(filteredSchools, 0, 1);
            var selectedYear = '';
            var selectedGrade = '';
            var selectedYears = CriteriaController.GetValuesAsSinglePropertyArray("Year", "Text", true);
            if (selectedYears && selectedYears.length > 0) {
                selectedYear = selectedYears[0];
            }
            if (selectedGrades && selectedGrades.length > 0) {
                selectedGrade = selectedGrades[0];
            }
            if (TeacherController.year != selectedYear || TeacherController.grade != selectedGrade) {       // if year or grade actually change, and isn't just getting fired because list was repopulated
                TeacherController.requestItems('', false);
            }
        }

        function InitialLoadOfSchoolList() {
            SchoolController.PopulateList(SchoolDependencyData, 0, 1);
        }

        function TeacherTooltipHide(sender, eventArgs) {
            // when the server adds additional teachers to the list via 'more', it visually messes up due to tooltip closing
            if (requestingTeachers) {
                eventArgs.set_cancel(true);
            }
        }

        function OnSchoolChange(comboBox) {
            // if we change the school, need to update the teachers
            TeacherController.requestItems('', false);
        }

        var requestingTeachers = false;
        function OnClientItemsRequesting_Teachers(sender, args) {
            // grab the currently selected school and pass that in the context to the service
            var selectedSchools = CriteriaController.GetValuesAsSinglePropertyArray("School", "Value", true);
            if (selectedSchools && selectedSchools.length > 0) {
                var context = args.get_context();
                context["SchoolId"] = selectedSchools[0];
            }
            var selectedYears = CriteriaController.GetValuesAsSinglePropertyArray("Year", "Text", true);
            if (selectedYears && selectedYears.length > 0) {
                var context = args.get_context();
                context["Year"] = selectedYears[0];
                TeacherController.year = selectedYears[0];
            }
            var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Text", true);
            if (selectedGrades && selectedGrades.length > 0) {
                var context = args.get_context();
                context["Grade"] = selectedGrades[0];
                TeacherController.grade = selectedGrades[0];
            }
            
            requestingTeachers = true;          // flag that we are in middle of a load to prevent tooltip closure
        }

        function OnClientItemsRequested_Teachers(sender, args) {
            // we've completed the item request, flag this so we can close the tooltip
            requestingTeachers = false;
            TeacherController.CheckComboForSelectedValueAfterDependencyChange();
        }
        
        function OnClientDropDownClosed_Teachers(sender, args) {
            // if we cancel the tool tip close due to a data load, it doesn't appear to ever want to close. Thus we force it to close when the dropdown closes
            TeacherController.CloseTooltip();
        }
        
    </script>
    <style type="text/css">
    .profLevel
    {
        width: 150px;
        border: 0px;
        margin: 2px;
        padding: 2px;
        float: left;
    }
    
    .reportSection
    {
        text-align: center;
        width: 100%;
    }
</style>
</asp:Content>