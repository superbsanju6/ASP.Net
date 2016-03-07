<%@ Page Title="Assessment Item Usage Report" Language="C#" MasterPageFile="~/Search.Master" AutoEventWireup="true" CodeBehind="AssessmentItemUsageReport.aspx.cs" Inherits="Thinkgate.Controls.Reports.AssessmentItemUsageReport" %>

<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextWithDropdown" Src="~/Controls/E3Criteria/TextWithDropdown.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextControl" Src="~/Controls/E3Criteria/Text.ascx" %>
<%@ Register TagPrefix="e3" TagName="DateRange" Src="~/Controls/E3Criteria/DateRange.ascx" %>

<%@ MasterType VirtualPath="~/Search.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        #RightColumnContentPlaceHolder_gridResultsPanel{
            height:100%;
        }
        #rightColumn{
             padding-left:18px;
        }
        .RadCalendar_Vista a:link, a:visited {
   
              text-align: center !important;
        }
      
        .RadCalendar_Vista
        {
            margin-top:8px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        function submit_print() {
            window.open('../../Record/RenderAssessmentItemUsageReportAsPDF.aspx?ItemBank=' + $("#RightColumnContentPlaceHolder_hdnItemBank").val() + '&Category=' + $("#RightColumnContentPlaceHolder_hdnCategory").val() + '&School=' + $("#RightColumnContentPlaceHolder_hdnSchool").val() + '&Grade=' + $("#RightColumnContentPlaceHolder_hdnGrade").val() + '&Subject=' + $("#RightColumnContentPlaceHolder_hdnSubject").val() + '&StartDate=' + $("#RightColumnContentPlaceHolder_hdnStartDate").val() + '&EndDate=' + $("#RightColumnContentPlaceHolder_hdnEndDate").val() + '&CurrDate=' + $("#lblDate").text());
        }
       
    </script>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <div style="width: 100%; clear: both; padding-top: 5px;margin-bottom:20px;">
        <div style="margin-right:25px;font-weight:bold;vertical-align:middle;font-size:16px;float:left;">Assessment Item Usage Report
        </div>
        <span>
                    <asp:ImageButton OnClick="exportGridImgBtn_Click" ID="exportGridImgBtn" ToolTip="Export displayed results to Excel" UseSubmitBehavior="false" ClientIDMode="Static" Style="" runat="server" ImageUrl="~/Images/Reports/ExcelIcon.png" Width="20px" Height="21"/>
                    <asp:ImageButton OnClick="printGridImgBtn_Click" ID="printGridImgBtn" ToolTip="Print displayed results" UseSubmitBehavior="false" ClientIDMode="Static" Style="" runat="server" ImageUrl="~/Images/Reports/PrintIcon.png" Width="20px" />
                </span>
        <div id="divResultAsOf" style="font-weight: bold; padding-left: 20px; padding-top:10px">
            <div>
                <span>Results as of: <asp:Label ID="lblDate" ClientIDMode="Static" runat="server"></asp:Label></span>
                
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="gridResultsPanel">
        <telerik:RadGrid runat="server" ID="radGridResults" Visible="false" AutoGenerateColumns="False" Width="594px"
            AllowFilteringByColumn="False" PageSize="20" AllowPaging="True" AllowSorting="True"
            OnPageIndexChanged="radGridResults_PageIndexChanged" AllowMultiRowSelection="true"
            OnItemDataBound="radGridResults_ItemDataBound" OnSortCommand="radGridResults_SortCommand"  CssClass="assessmentSearchHeader" Skin="Web20" OnNeedDataSource="radGridResults_NeedDataSource">
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" ></PagerStyle>
            <ClientSettings EnableRowHoverStyle="true">
                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True" ScrollHeight="200px">
                </Scrolling>
            </ClientSettings>
            <MasterTableView TableLayout="Auto"  PageSize="100" >
                <Columns>
                    <telerik:GridTemplateColumn InitializeTemplatesFirst="false" HeaderText="Item ID" HeaderStyle-Width="50%" ItemStyle-Font-Size="Small" DataField="ItemID" UniqueName="ItemID">
						<ItemTemplate>
							<asp:HyperLink runat="server" ID="itemIDLink"></asp:HyperLink><br />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="Frequency" HeaderText="Frequency" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" >
                        <ItemTemplate>
							<span runat="server" id="spanbankTotal"></span>
						</ItemTemplate>
                        </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid><br/>
        <telerik:RadGrid runat="server" ID="radGridItemBank" Visible="false" AutoGenerateColumns="False" Width="900px"
            AllowFilteringByColumn="False" PageSize="20" AllowSorting="True"
            AllowMultiRowSelection="true"
            OnItemDataBound="radGridItemBank_ItemDataBound" OnSortCommand="radGridItemBank_SortCommand"  CssClass="assessmentSearchHeader" Skin="Web20" OnNeedDataSource="radGridItemBank_NeedDataSource">
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" ></PagerStyle>
            <ClientSettings EnableRowHoverStyle="true">
                <%--<Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True" ScrollHeight="200px">
                </Scrolling>--%>
            </ClientSettings>
            <MasterTableView TableLayout="Auto"  PageSize="100" >
                <Columns>
                    <telerik:GridBoundColumn DataField="ItemBank" HeaderText="Item Bank" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="33%" />
                    <telerik:GridBoundColumn DataField="ItemTotal" HeaderText="Item Total" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="33%" />
                    <telerik:GridBoundColumn DataField="Frequency" HeaderText="Frequency" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <asp:PlaceHolder ID="initialDisplayText" runat="server">
            <div id="divDefaultMessage" style="height: 100%; text-align: center; clear:both;">
                <div style="height: 40%"></div>
                <div style="height: 20%">
                <strong>Please select criteria for all required fields (indicated by <span style="color: red; font-weight: bold">*</span>)</strong>
                <br />
                <strong>then Update Results.</strong>
        </div>
        <div style="height: 40%"></div>
    </div>
        </asp:PlaceHolder>
        <asp:HiddenField ID="hdnItemBank" runat="server" />
        <asp:HiddenField ID="hdnCategory" runat="server" />
        <asp:HiddenField ID="hdnSchool" runat="server" />
        <asp:HiddenField ID="hdnGrade" runat="server" />
        <asp:HiddenField ID="hdnSubject" runat="server" />
        <asp:HiddenField ID="hdnStartDate" runat="server" />
        <asp:HiddenField ID="hdnEndDate" runat="server" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnReportViewerContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" />
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:CheckBoxList ID="chkItemBank" CriteriaName="ItemBank" runat="server" Text="Item Bank" DataTextField="Label" DataValueField="Label" />
    <e3:DropDownList ID="ddlCategory" CriteriaName="Category" runat="server" Text="Category" Required="True" EmptyMessage="Select Category" />
    <e3:DropDownList ID="ddlSchool" CriteriaName="School" runat="server" Text="School" DataTextField="Name" DataValueField="Name" EmptyMessage="Select School" OnChange="BindSchoolGrad(this)"/>
    <e3:DropDownList ID="ddlGrade" CriteriaName="Grade" runat="server" Text="Grade" DataTextField="Grade" DataValueField="Grade" EmptyMessage="Select Grade"/>
    <asp:HiddenField ID="hdnSchoolClickedFromAssReprot" Value="NO" />
    <e3:CheckBoxList ID="chkSubject" CriteriaName="Subject" runat="server" Text="Subject" DataTextField="Subject" DataValueField="Subject" />
    <e3:DateRange ID="drDateRange" CriteriaName="DateRange" runat="server" Text="Date Range" />
    <script type="text/javascript">
       
        Sys.Application.add_load(function () {
            var startDatePicker = $find($("#<%=drDateRange.ClientID%>" + "_CriteriaHeader_expand_bubble").attr("startDatePickerID"));
            var endDatePicker = $find($("#<%=drDateRange.ClientID%>" + "_CriteriaHeader_expand_bubble").attr("endDatePickerID"));
            startDatePicker.set_minDate(new Date('<%=StartDate%>'));
            startDatePicker.set_maxDate(new Date('<%=EndDate%>'));
            endDatePicker.set_minDate(new Date('<%=StartDate%>'));
            endDatePicker.set_maxDate(new Date('<%=EndDate%>'));

            startDatePicker._calendar.set_fastNavigationStep(12);
            endDatePicker._calendar.set_fastNavigationStep(12);

        });

        $(document).ready(function ()
        {
            var startPickerId = "#ctl00_LeftColumnContentPlaceHolder_drDateRange_rdPickerStart_dateInput";
            var endPickerId = "#ctl00_LeftColumnContentPlaceHolder_drDateRange_rdPickerEnd_dateInput";
            $(startPickerId).attr("readonly", "true")
            $(endPickerId).attr("readonly", "true")
          
        });

        $(function () {
            $("body").delegate("#printGridImgBtn", "click", function () {
                if ($("#ctl00_RightColumnContentPlaceHolder_radGridResults").length > 0) {
                    submit_print();
                }
                return false;
            }).delegate("#exportGridImgBtn", "click", function () {
                if ($("#ctl00_RightColumnContentPlaceHolder_radGridResults").length > 0) {
                    return true;
                }
                return false;
            });
        });


        
        function stringIsEmpty(str) {
            if (str && str !== null && str !== "") {
                return false;
            } else {
                return true;
            }
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

        function BindSchoolGrad()
        {
            CriteriaController.RemoveAll("Grade");
            $find("<%= ddlGrade.ComboBox.ClientID %>").clearItems();

            var selectedSchoolID = getCriteriaValue("School");

            if (selectedSchoolID>0) {
                GetGradeListOnSchoolSelection(selectedSchoolID);
            }
            $("#hdnSchoolClickedFromAssReprot").val("YES");
        }


        function GetGradeListOnSchoolSelection(selectedSchoolID) {
            $.ajax({
                type: "POST",
                url: "./AssessmentItemUsageReport.aspx/GetGradeListOnSchoolSelection",
                data: "{'schoolID':" + selectedSchoolID + "}",
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
                            data1.push([data[i].Grade]);
                        }
                    } else {
                        //No classes found, do something?
                    }
                    GradeController.PopulateList(data1, 0, 1);
                }
            });
        }
     
    </script>
</asp:Content>
<asp:Content ID="Content8" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>
