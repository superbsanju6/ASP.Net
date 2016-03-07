<%@ Page Title="Summative Report" Language="C#" MasterPageFile="~/Search.Master"
    AutoEventWireup="true" CodeBehind="SummativeReportPageV2.aspx.cs" Inherits="Thinkgate.Controls.Reports.SummativeReportPageV2" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" tagName="TestYearGradeSubject" Src="~/Controls/E3Criteria/TestYearGradeSubject.ascx"%>
<%@ Register TagPrefix="e3" tagName="Demographics" Src="~/Controls/E3Criteria/Demographics.ascx"%>
<%@ MasterType virtualpath="~/Search.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results"/>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:TestYearGradeSubject ID="ctrlTestYearGradeSubject" CriteriaName="TestYearGradeSubject" runat="server"/>
    <e3:DropDownList ID="cmbSchool" CriteriaName="School" runat="server" Text="School" EmptyMessage="Select a School"/>
    <e3:Demographics ID="demos" CriteriaName="Demographics" runat="server" Text="Demographics"/>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <div id="divExportOptions" style="text-align: left">
        <asp:ImageButton runat="server" ID="btnExportExcel" OnClick="btnExportExcel_Click"
            ImageUrl="~/Images/Toolbars/excel_button.png" />
        <asp:ImageButton runat="server" ID="btnFLDOE" OnClientClick="window.open('http://www.fldoe.org/asp/k12memo/pdf/tngcbtf.pdf'); return false;"
            ImageUrl="~/Images/Toolbars/info.png" Visible="False" ToolTip="Go to www.fldoe.org"/>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <div id="lblInitialText" runat="server" style="font-size: 11pt; text-align:center; position: absolute; width: 100%; top: 25%; margin-left: auto; margin-right: auto">Please select criteria for all required fields (Indicated by <span style="color: rgb(255, 0, 0); font-weight: bold;">*</span>)<br/> then Update Results.</div>
    <telerik:RadTreeList ID="radTreeResults" runat="server" ParentDataKeyNames="ParentConcatKey"
        OnChildItemsDataBind="TreeListChild_DataSourceNeeded" OnNeedDataSource="TreeListDataSourceNeeded"
        OnItemDataBound="radTreeResults_ItemDataBound" AllowLoadOnDemand="true" Skin="Office2010Silver"
        DataKeyNames="ConcatKey" AutoGenerateColumns="false" Height="100%" Width="100%" 
        HeaderStyle-Width="100px" Visible="False">
        <ClientSettings Scrolling-AllowScroll="true" Scrolling-UseStaticHeaders="true">
            <Resizing ResizeMode="AllowScroll" AllowColumnResize="true" EnableRealTimeResize="true" />
            <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true" ScrollHeight="560px" />
            <ClientEvents OnTreeListCreated="TreeListCreated" />
        </ClientSettings>
    </telerik:RadTreeList>
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

        function ChangeSchoolBasedOnGrade(grade) {
            var selectedGrades = CriteriaController.GetValuesAsSinglePropertyArray("Grade", "Text", true);
            var filteredSchoolPos = CriteriaDataHelpers.GetFilteredDataPositions(SchoolDependencyData, [null, null, selectedGrades]);
            var filteredSchools = [];
            for (var j = 0; j < filteredSchoolPos.length; j++) {
                filteredSchools.push(SchoolDependencyData[filteredSchoolPos[j]]);
            }

            SchoolController.PopulateList(filteredSchools, 0, 1);
        }

        function InitialLoadOfSchoolList() {
            SchoolController.PopulateList(SchoolDependencyData, 0, 1);  
  
        }
    </script>
</asp:Content>