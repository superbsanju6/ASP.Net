<%@ Page Title="Assessment Scheduling" Language="C#" AutoEventWireup="True" CodeBehind="AssessmentScheduling.aspx.cs" Inherits="Thinkgate.Controls.Assessment.AssessmentScheduling" MasterPageFile="~/Search.Master" %>

<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="ScheduleCriteria" Src="~/Controls/E3Criteria/AssessmentSchedule/ScheduleCriteria.ascx" %>
<%@ Register TagPrefix="e3" TagName="Curriculum" Src="~/Controls/E3Criteria/Associations/Curriculum.ascx" %>
<%@ Register TagPrefix="e3" TagName="Classes" Src="~/Controls/E3Criteria/AssessmentSchedule/ClassesCriteria.ascx" %>
<%@ Register TagPrefix="e3" TagName="Schools" Src="~/Controls/E3Criteria/Associations/Schools.ascx" %>
<%@ Register TagPrefix="e3" TagName="Districts" Src="~/Controls/E3Criteria/AssessmentSchedule/DistrictCriteria.ascx" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="Schedules" Src="~/Controls/Schedules.ascx" %>
<%@ Register TagPrefix="e3" TagName="Schedules_Edit" Src="~/Controls/Schedules_Edit.ascx" %>

<%@ MasterType VirtualPath="~/Search.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <div style="bottom: 0px; left: 0px; height: 14.72px; width: 300px;">
        <div style="font-weight: bold; float: left;" id="selectedRecordsDiv">
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" OnClientClear="AssessmentSchedulingClearAllCriteria" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:ScheduleCriteria ID="ctrlScheduleCriteria" CriteriaName="ScheduleCriteria" runat="server" Text="Schedule Criteria" Required="true" />
    <div id="divCtrlDistrictContainer" runat="server">
        <e3:DropDownList ID="ctrlDistricts2" CriteriaName="Districts2" Text="Districts2" runat="server" DataTextField="Key" DataValueField="Value" EmptyMessage="Select District" />
    </div>
    <e3:Curriculum ID="ctrlCurriculum" CriteriaName="Curriculum" Text="Curriculum" runat="server" IncludeTypeAndTermControls="true" />
    <e3:Classes ID="ctrlClasses" CriteriaName="Classes" Text="Classes" runat="server" />
    <e3:Schools ID="ctrlSchools" CriteriaName="Schools" Text="Schools" runat="server" />
    <e3:DropDownList ID="ctrlStatus" CriteriaName="Status" Text="Status" runat="server" DataTextField="Key" DataValueField="Value" EmptyMessage="Select Status" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
   <style type="text/css">
        
        #AssessmentScheduling_gridResultsPanel {
            /*height: 350px;*/
            overflow-y: auto;
            overflow-x: hidden;
        }
        .Scroll {
            overflow: auto !important;
        }
            #AssessmentScheduling_gridResultsPanel .rgDataDiv {
                /*Added this so that when the schedule area shows at the top of the screen you can still get to the last item in the grid*/
                /*max-height: 65%;
            overflow-y:auto;*/
            }
    </style>
    <div style="width: 100%; min-height: 90px; display: none;" runat="server" id="divEditCtrl" clientidmode="Static">
        <e3:Schedules_Edit ID="ctrlSchedEdit" ClientIDMode="Static" runat="server" SaveHandler="saveSchedChanges" CancelHandler="cancelSchedChanges"></e3:Schedules_Edit>
    </div>
       <asp:Panel runat="server" ID="AssessmentScheduling_gridResultsPanel" ClientIDMode="Static" CssClass="Scroll">
        <div id="gridContainer" style="overflow: auto;">
            <telerik:RadGrid
                runat="server"
                ID="radGridResults"
                AutoGenerateColumns="False"
                Width="100%"
                AllowFilteringByColumn="False"
                PageSize="20"
                AllowPaging="True"
                AllowSorting="True"
                OnPageIndexChanged="RadGridResults_PageIndexChanged"
                AllowMultiRowSelection="true"
                OnItemDataBound="radGridResults_ItemDataBound"
                OnSortCommand="OnSortCommand"
                Skin="Web20"
                OnNeedDataSource="RadGrid_NeedDataSource">
                <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
                <ClientSettings EnableRowHoverStyle="true">
                    <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />
                    <ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeselected" OnGridCreated="setGridHeight" />
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True" ScrollHeight="640px"></Scrolling>
                </ClientSettings>
                <MasterTableView TableLayout="Auto" PageSize="100">
                    <HeaderStyle />
                    <Columns>
                        <telerik:GridTemplateColumn HeaderText="" UniqueName="colSelected" HeaderStyle-Width="30px">
                            <HeaderTemplate>
                                <input id="chkAll" runat="server" type="checkbox" name="headerSelect" value="all" class="assessSchedChkRow" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <input id="chkRowInput" class="assessSchedChkRow" runat="server" type="checkbox" name="rowSelect" value='<%# Eval("ID")%>' />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <%--<telerik:GridHyperLinkColumn UniqueName="colAssessmentLink" HeaderText="Assessment" DataTextField="TestName" Target="_blank" NavigateUrl="javascript:OpenTest();" HeaderStyle-Width="200px" />--%>
                        <telerik:GridTemplateColumn DataField="TestName" UniqueName="colAssessmentLink" HeaderText="Assessment" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkListTestName" runat="server" NavigateUrl="javascript:OpenTest();" Target="_blank"
                                    Visible="true" Style="color: Blue;"><%# String.Format("{0}", Eval("TestName")) %></asp:HyperLink>
                                <asp:Image runat="server" ToolTip="Secure" ImageUrl="~/Images/IconSecure-lock.png" ID="imgIconSecure" Width="22px" Height="15px" Visible="false" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn UniqueName="colDistrict" HeaderText="District" DataField="ClientName" HeaderStyle-Width="100px" />
                        <telerik:GridBoundColumn UniqueName="colSchool" HeaderText="School" DataField="SchoolName" HeaderStyle-Width="150px" />
                        <telerik:GridBoundColumn UniqueName="colClass" HeaderText="Class" DataField="ClassName" HeaderStyle-Width="150px" />
                        <telerik:GridBoundColumn UniqueName="colTeacher" HeaderText="Teacher" DataField="TeacherName" HeaderStyle-Width="150px" />
                        <telerik:GridTemplateColumn UniqueName="colSchedules" HeaderText="Scheduling" HeaderStyle-Width="430px" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <div>
                                    <e3:Schedules ID="ctrlSchedules" runat="server" />
                                </div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
        <asp:PlaceHolder ID="initialDisplayText" runat="server"></asp:PlaceHolder>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">

    <script type="text/javascript">
        /*******************************************************************************************************************************
        ShowEditCtrl

        This function will either display or hide the Schedules Edit control.  When showing, however, it needs to determine what to 
        display as follows:  Of the rows selected if the "Locked" status is the same across all rows, then the "Locked" control in the 
        Schedules Edit Control should display that value.  If they differ, then the "Locked" control should display a 3rd state which 
        means "heterogeneous" state.

        For the beginning and ending dates, if the dates across the rows are the same, then that date should appear in the Schedules 
        Edit Control, otherwise, leave that date field blank.
        *******************************************************************************************************************************/
        function ShowEditCtrl(show) {

            if (show) {
                // Get reference to the rows in the radgrid.
                var ctrlRadGridResults = get_radGridResults_Control();
                var masterTable = ctrlRadGridResults.get_masterTableView();
                var selectedRows = masterTable.get_selectedItems();

                // Get the different schedule types in the Schedule Edit Control and 
                // iterate through these to determine how to populate the fields in 
                // the control.

                Schedules_Edit_Control.LoadControlsArray();
                Schedules_Edit_Control.LoadControlValuesArray();
                //if (Schedules_Edit_Control.ControlValuesArray.length == 0) Schedules_Edit_Control.LoadControlValuesArray();
                var aryCtrlValues = Schedules_Edit_Control.ControlValuesArray;
                var aryCtrls = Schedules_Edit_Control.ControlsArray;

                for (var i = 0; i < aryCtrls.length; i++) {

                    // We are assuming that the two arrays aryCtrlValues and aryCtrls are sorted the same (because of the way function LoadControlValuesArray() works).


                    //var commonBeginDate = (aryCtrlValues[i].Lock_Begin == null || isNaN(Date.parse(aryCtrlValues[i].Lock_Begin))) ? "01/01/100" : aryCtrlValues[i].Lock_Begin;
                    //var commonEndDate = (aryCtrlValues[i].Lock_End == null || isNaN(Date.parse(aryCtrlValues[i].Lock_End))) ? "01/01/100" : aryCtrlValues[i].Lock_End;
                    //var commonLocked = (aryCtrlValues[i].Locked == 2) ? "Not set yet." : aryCtrlValues[i].Locked;
                    var commonBeginDate = "01/01/100";
                    var commonEndDate = "01/01/100";
                    var commonLocked = "Not set yet.";

                    // itterate through each selected row in grid to determine how to set the controls in Schedules Edit Control
                    for (var j = 0; j < selectedRows.length; j++) {

                        // if all of our common variables have been nulled out then there is no use to iterating through any more rows.
                        if (!(commonBeginDate || commonEndDate || commonLocked)) break;
                        else {
                            //fetch out of the current row, all elements for this scheduletype
                            var jqArrayOfSchedElementsInRow = $(".Schedules", "#" + selectedRows[j].get_id()).filter("." + aryCtrls[i].ScheduleTypeName);

                            if (jqArrayOfSchedElementsInRow.length > 0) {
                                if (commonBeginDate != null) {

                                    // Get begindate value out of the row.
                                    var beginDate = jqArrayOfSchedElementsInRow.filter(".Begin")[0].innerHTML;

                                    //If date fetched from row is legit, then compare it commendBeginDate.
                                    if (!isNaN(Date.parse(beginDate)))
                                        if (commonBeginDate == "01/01/100") commonBeginDate = beginDate;
                                        else
                                            if (commonBeginDate != beginDate) commonBeginDate = null; //heterogeneous across selected rows.
                                }

                                if (commonEndDate != null) {

                                    // Get endDate value out of the row.
                                    var endDate = jqArrayOfSchedElementsInRow.filter(".End")[0].innerHTML;

                                    //If date fetched from row is legit, then compare it commendEndDate.
                                    if (!isNaN(Date.parse(endDate)))
                                        if (commonEndDate == "01/01/100") commonEndDate = endDate;
                                        else
                                            if (commonEndDate != endDate) commonEndDate = null; //heterogeneous across selected rows.
                                }

                                if (commonLocked != null) {

                                    // Get Locked value out of the row.
                                    var sValue = jqArrayOfSchedElementsInRow.filter(".Locked").attr("value");
                                    var locked = (sValue == "True") ? 1 : 0;

                                    if (commonLocked == "Not set yet.") commonLocked = locked;
                                    else
                                        if (commonLocked != locked) commonLocked = null; //heterogeneous across selected rows.
                                }

                            }
                        }
                    }

                    // Having determined whether dates and lock are heterogeneous or not, populate the controls in Schedules Edit Control accordingly.
                    if (commonBeginDate != null && commonBeginDate != "01/01/100") aryCtrls[i].BeginDateCtrl.set_selectedDate(new Date(commonBeginDate));
                    else aryCtrls[i].BeginDateCtrl.clear();

                    if (commonEndDate != null && commonEndDate != "01/01/100") aryCtrls[i].EndDateCtrl.set_selectedDate(new Date(commonEndDate));
                    else aryCtrls[i].EndDateCtrl.clear();

                    if (commonLocked != null) aryCtrls[i].ToggleCtrl.set_selectedToggleStateIndex(commonLocked);
                    else aryCtrls[i].ToggleCtrl.set_selectedToggleStateIndex(2); //heterogeneous state.
                }

                divEditCtrl.style.display = "block";
            }

            else {
                divEditCtrl.style.display = "none";
            }

        }


        function selectAll(sender, id, totalCount) {

            var radgrid = $find(id);

            if (radgrid) {
                var masterTable = radgrid.get_masterTableView();
                if (sender.checked) {
                    masterTable.selectAllItems();
                    $('.assessSchedchkRow').attr('checked', 'checked')
                    updateSelectedRowCount(totalCount);
                    ShowEditCtrl(true);
                }
                else {
                    ShowEditCtrl(false);
                    masterTable.clearSelectedItems();
                    $('.assessSchedchkRow').attr('checked', '')
                    updateSelectedRowCount(0);
                }
            }

        }

        function selectThisRow(sender, id, rowIndex) {
            var radgrid = $find(id);

            if (radgrid) {
                var masterTable = radgrid.get_masterTableView();
                if (sender.checked) {
                    masterTable.selectItem(rowIndex);
                }
                else {
                    masterTable.deselectItem(rowIndex);
                }

                var count = countSelectedRows();
                updateSelectedRowCount(count);

                if (count == 0) {
                    ShowEditCtrl(false);
                    setGridHeight();
                }
                else {
                    ShowEditCtrl(true);
                    setGridHeight();

                }
            }
        }

        function setGridHeight() {
            var controlHeight = $("#divEditCtrl").height();
            var margin = 0;
            var diff = 0;
            var gridId = ".rgDataDiv";
            var space = $(window).height();
            margin = .06 * space;
            if ($("#divEditCtrl").is(":visible"))
                diff = space - margin - controlHeight;
            else
                diff = space - margin;
            $(gridId).height(diff);

        }

        var isheightfixed = false;
        // Test for any checked checkboxes in grid then show/hide the edit control accordingly.
        function RowSelected(sender, eventArgs) {
            if (isheightfixed == false) {
                setGridHeight();
                isheightfixed = true;
            }
            var rowIndex = eventArgs.get_itemIndexHierarchical();
            $('.assessSchedChkRow').each(function () {
                checkboxRowIndex = this.getAttribute("rowIndex");

                if (checkboxRowIndex == rowIndex) {
                    this.checked = true;
                }

            });

            var count = countSelectedRows();
            updateSelectedRowCount(count);

            if (count == 0) {
                ShowEditCtrl(false);
                setGridHeight();
            }
            else {
                ShowEditCtrl(true);
                setGridHeight();
            }
        }

        function RowDeselected(sender, eventArgs) {

            var rowIndex = eventArgs.get_itemIndexHierarchical();
            var checkBoxElement = eventArgs.get_item();
            var gridID = sender.get_id();

            $('.assessSchedChkRow').each(function () {
                checkboxRowIndex = this.getAttribute("rowIndex");

                if (checkboxRowIndex == rowIndex) {
                    this.checked = false;
                }

            });

            var count = countSelectedRows();
            updateSelectedRowCount(count);

            if (count == 0) {
                ShowEditCtrl(false);
                setGridHeight();
            }
            else {
                ShowEditCtrl(true);
            }

        }


        function updateSelectedRowCount(count) {
            var text = "";

            if (count == 0)
                text = "";
            else
                text = 'Records selected: ' + count;

            if ($('#selectedRecordsDiv')) $('#selectedRecordsDiv').text(text);
        }

        function countSelectedRows() {
            var count = 0;
            $(".assessSchedChkRow").each(function () {
                if (this.checked && $(this).attr('id') != 'ctl00_RightColumnContentPlaceHolder_radGridResults_ctl00_ctl02_ctl01_chkAll')
                    count = count + 1;
            });
            return count;
        }

        function saveSchedChanges(EditControlData) {
            //Build a Json object of the values needed in order for the codebehind to be able to save off scheduling changes.
            //$("#hdnCriteriaController")[0].value = CriteriaController.ToJSON()

            // Get values from Edit Window to be used to set all selected schedules.
            //$find("ajxpEditCtrl").ajaxRequest(JSON.stringify(EditControlData));

            //    $find("AjaxPanelResults").ajaxRequest(CriteriaController.ToJSON())
            //$("#hdnEditCtrlValues")[0].value = JSON.stringify(EditControlData);
            //$("#AssessmentScheduling_hdnBtnSave")[0].click();

            //__doPostBack("EditControlSave", JSON.stringify(EditControlData));        
            //$find("AjaxPanelUpdateSchedules").ajaxRequest('{"David":"Ballantyne","Pet":"Detective"}');

            $("#hdnAjxpEditCtrlValues")[0].value = JSON.stringify(EditControlData);
            CriteriaController.UpdateCriteriaForSearch();
            $find("AjaxPanelResults").ajaxRequest(CriteriaController.ToJSON());
        }

        function cancelSchedChanges() {
            // set values of date controls back to their defaults.
            Schedules_Edit_Control.SetDateControlsToDefaults();

            // Uncheck all selected rows in the grid.
            // Get reference to the rows in the radgrid.
            var ctrlRadGridResults = get_radGridResults_Control();
            var masterTable = ctrlRadGridResults.get_masterTableView();
            masterTable.clearSelectedItems();

            // call ShowEditCtrl()
            ShowEditCtrl(false);
        }


    </script>
    <asp:TextBox ID="hiddenGuidBox" runat="server" Style="visibility: hidden; display: none;" />
    <input runat="server" clientidmode="Static" id="hdnAjxpEditCtrlValues" type="hidden" value="" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        function autoHeight() {
            var gridId = ".rgDataDiv";
            var hh = $("#AssessmentScheduling_gridResultsPanel").height();
            $(gridId).height(hh);
        }

        function alterPageComponents(state) {

            $('.rgHeaderDiv').each(function () {
                this.style.width = '';
            });

        }

        window.onload = function () {
            alterPageComponents('moot');
        };

        function openSchool(id) {
            window.open('<%=ResolveUrl("~/record/school.aspx") %>' + '?xID=' + id + 'childpage=yes');
        }

        //
        // In order to make the Curriculum ascx control work correctly,
        // it expects there to be an "AssociationsRemoveAll()" method
        // in the hosting web page.  So I have added it here and 
        // changed its functionality accordingly
        //
        function AssociationsRemoveAll() {
            //CriteriaController.RemoveAll("Curriculum");
            //CriteriaController.RemoveAll("Classes");
            //CriteriaController.RemoveAll("Schools");
        }

        function AssessmentSchedulingClearAllCriteria() {
            AssociationsRemoveAll();
            CriteriaController.RemoveAll("ScheduleCriteria");
            ScheduleCriteriaController.Clear();
            CurriculumController.Clear();
            ClassesController.Clear();
            SchoolsController.Clear();
            Districts2Controller.Clear();
            StatusController.Clear();
            
        }

        function get_radGridResults_Control() { return $find("<%= radGridResults.ClientID %>"); }

    </script>
</asp:Content>



<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        #rightColumn {
            background-color: white;
        }
    </style>
</asp:Content>
