<%@ Page Title="Usage Statistic Report" Language="C#" AutoEventWireup="True" CodeBehind="UsageStatisticReport.aspx.cs" Inherits="Thinkgate.Controls.Reports.UsageStatisticReport" MasterPageFile="~/Search.Master" %>

<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextWithDropdown" Src="~/Controls/E3Criteria/TextWithDropdown.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextControl" Src="~/Controls/E3Criteria/Text.ascx" %>
<%@ Register TagPrefix="e3" TagName="MonthYearCal" Src="~/Controls/E3Criteria/MonthYearCalender.ascx" %>
<%@ Register TagPrefix="e3" TagName="RegionSchoolTypeSchool" Src="~/Controls/E3Criteria/RegionSchoolTypeSchool.ascx" %>
<%@ Register TagPrefix="e3" TagName="UsageReportGradeSubjectCourse" Src="~/Controls/E3Criteria/UsageReportGradeSubjectCourse.ascx" %>

<%@ MasterType VirtualPath="~/Search.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <div style="width: 100%; clear: both; padding-top: 5px; margin-bottom: 20px;">
        <span runat="server" id="sortSpan">
            <span style="margin-right: 5px; font-weight: bold; vertical-align: middle;">View:</span>
            <telerik:RadComboBox ID="ddlView" runat="server" OnClientSelectedIndexChanged="ddlViewIndexChanged" EmptyMessage="View By" Skin="Web20" Width="100" AutoPostBack="false" DataTextField="Name" DataValueField="Value">
            </telerik:RadComboBox>
            <span id="spanResultAsOf" style="font-weight: bold; padding-left: 10px;">Results as of: 
            </span>
            <span id="spanResultDate" style="display: none; font-weight: normal;"></span>
        </span>
        <asp:ImageButton ID="exportGridImgBtn" OnClientClick="return ValidateData();" ToolTip="Export to Excel." UseSubmitBehavior="false" OnClick="exportGridImgBtn_Click" ClientIDMode="Static" Style="float: right; margin-right: 1%;" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
            Width="20px" />
    </div>

    <div id="hidden_content" style="display: none">
        <asp:Image CssClass="flag_img flag_yellow" ID="flag_yellow_template" runat="server" ImageUrl="~/Images/commands/flag_yellow.png" ToolTip="Item already added to this assessment" ClientIDMode="Static" />
    </div>
    <div style="width: 100%; margin: 2px; margin-top: 5px;">
        <div class="listWrapper" style="width: 49%; text-align: left; float: left;">
        </div>
        <div style="width: 49%; text-align: right; float: right;">
            <%--<asp:ImageButton ID="exportGridImgBtn"  OnClick="exportGridImgBtn_Click" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
                Width="20px" />--%>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:DropDownList ID="cmbLevel" CriteriaName="LevelFilter" runat="server" EmptyMessage="Select a Level" Text="Level" DataTextField="Name" DataValueField="Value" Required="True" onchange="ddlLevelIndexChanged()" />
    <e3:MonthYearCal runat="server" ID="CalStartMonthYear" CriteriaName="StartMonthYear" EmptyMessage="Start..." Text="Start Date" Required="True" />
    <e3:MonthYearCal runat="server" ID="CalEndMonthYear" CriteriaName="EndMonthYear" EmptyMessage="End..." Text="End Date" />
    <e3:CheckBoxList ID="cblComponentType" CriteriaName="ComponentType" runat="server" Text="Component Type" DataTextField="Name" DataValueField="Value" />
    <e3:RegionSchoolTypeSchool runat="server" ID="ctrlRegionSchoolTypeSchool" CriteriaName="RegionSchoolTypeSchool" />
    <e3:UsageReportGradeSubjectCourse runat="server" ID="ctrlUsageGradeSubjectCourse" CriteriaName="UsageGradeSubjectCourseSet" />
    <script type="text/javascript">

    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <div id="divDefaultMessage" style="height: 100%; text-align: center; clear: both;">
        <div style="height: 40%"></div>
        <div style="height: 20%">
            <strong>Please select criteria for all required fields (indicated by <span style="color: red; font-weight: bold">*</span>)</strong>
            <br />
            <strong>then Update Results.</strong>
        </div>
        <div style="height: 40%"></div>
    </div>
    <div id="DivBlackMsg"><span style="color: gray; text-align: center; margin-top: 20px; font-weight: 200; float: right; margin-right: 3%">No records to display.</span></div>
    <div id="TableView" style="overflow: auto; float: left;" class="ddlview">
        <table id="tblUsageStatistics" border="0" class="cell-border" style="width: 100%; border-collapse: collapse;" cellpadding="0" cellspancing="0">
        </table>
    </div>
    <div id="GraphView" style="overflow: auto; height: inherit; width: 100%;" class="ddlview">
        <div>
            <div id="GraphicalViewDiv"></div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <div style=""></div>
    <link rel="stylesheet" type="text/css" href="../../Scripts/usagestatistic/css/jquery.dataTables.css"></link>
    <link rel="stylesheet" type="text/css" href="../../Scripts/usagestatistic/css/StyleUsageStatistic.css"></link>
    <script src="../../Scripts/usagestatistic/js/jquery.dataTables.js" type="text/javascript"></script>
    <script src="../../Scripts/usagestatistic/js/JSUsageStatistic.js" type="text/javascript"></script>

    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">
        var reportData = {};
        var Resultdata = [];

        $(document).ready(function () {
            initializeControls();
        });


        $(document).live("keypress", function (e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            if (code == 13) {
                e.preventDefault();
                e.stopPropagation();
                $(this).closest('form').submit();
            }
        });

        ///Initialize the control.
        function initializeControls() {
            $("#criteriaHeaderDiv_Grade").css('display', 'none');
            $("#criteriaHeaderDiv_Subject").css('display', 'none');
            $("#criteriaHeaderDiv_Course").css('display', 'none');
        }

        //$(window).resize(function () {
        //    // This will execute whenever the window is resized
        //  var height=  $(window).height(); // New height
        //  var width = $(window).width(); // New width
        //  alert("height: " + height + " , " + "width: " + width);
        //});

        $("a[href^=#]").on("click", function (e) {
            e.preventDefault();
            history.pushState({}, "", this.href);
        });
        /* This function is called by Ajax framework automatically when page load completes */
        function pageLoad() {
            $("#exportGridImgBtn").hide();
            $("#DivBlackMsg").hide();
            $("#divDefaultMessage").show();
            $("#AjaxPanelResults").css({ 'background-color': 'lightgrey' });

            if (reportData && reportData.lstUsageData) {
                if (reportData.lstUsageData && reportData.lstUsageData.length > 0) {
                    $("#spanResultDate").show();
                    $("#spanResultDate").text(reportData.lstUsageData[0].InsertedOn);

                    $("#exportGridImgBtn").show();
                    $("#divDefaultMessage").hide();
                    var height = $('#AjaxPanelResults').height();
                    var width = $('#AjaxPanelResults').width();
                    $("#TableView").css({ 'height': height - 45, 'width': width - 30 });
                    bindUsageGrid(reportData);
                    addColumnGroupTitle(reportData.lstGridColumGroup);
                    Resultdata = reportData;
                    generateCharts();
                    $("#DivBlackMsg").hide();
                }
                else {
                    $("#DivBlackMsg").show();
                    $("#divDefaultMessage").hide();
                    $("#AjaxPanelResults").css({ 'background-color': 'white' });
                    //$('#GraphicalViewDiv').html("<span style='background-color:white;color:black;text-align: 'center', margin-top: 20px, color: '#ffffff', font-weight: 'bold'>No records to display.</span>");
                    $("#exportGridImgBtn").hide();
                    $("#spanResultDate").hide();
                }
                var combo = $find('<%=ddlView.ClientID%>'); var item = combo.findItemByValue("Graphical"); item.select();
            }
            else {
            }
        }



        /*******Graphical view code start **********/
        function generateCharts() {
            if (Resultdata.graphicalViewData == null) return true;
            var counter = Resultdata.graphicalViewData.length;
            for (var i = 0; i < counter ; i++) {
                var graphdata = eval(eval(Resultdata.graphicalViewData[i]));
                var divId = (eval(eval(Resultdata.graphicalViewData[i]))[0][0]).replace(/ +/g, "");
                var chartTypeValue = (eval(eval(Resultdata.graphicalViewData[i]))[0][1]).toLowerCase();
                var chartTitle = eval(eval(Resultdata.graphicalViewData[i]))[0][0];
                drawChart(graphdata, { Title: chartTitle, chartDivID: divId, vAxisLabel: 'Percent', hAxisLabel: 'Gender', chartType: chartTypeValue });
            }
        }
        // Load the Visualization API and the piechart package.
        google.load('visualization', '1.0', { 'packages': ['corechart'] });

        function drawChart(graphdata, options) {
            options = options || new Object();

            var vAxisLabel = options.vAxisLabel || '';
            var hAxisLabel = options.hAxisLabel || '';
            var dd = $('#AjaxPanelResults').width();
            var size_width = eval((dd / 4) - 10);

            ///Checks for the count if its only 
            if (graphdata.length > 1) {

                switch (options.chartType) {
                    case 'column':

                        var data2 = google.visualization.arrayToDataTable(graphdata);
                        $("#GraphicalViewDiv").append($("<div id=" + options.chartDivID + " class='graph'></div>"));

                        columnChart = new google.visualization.ColumnChart(document.getElementById(options.chartDivID));
                        var columnOptions = {
                            title: options.Title,
                            width: size_width, //Set container width
                            height: 200, //Set container height
                            fontSize: 9,
                            titleTextStyle: { bold: true, fontSize: 13 },
                            is3D: true,
                            legend: 'none'

                        };

                        columnChart.draw(data2, columnOptions);
                        $("text:contains(" + columnOptions.title + ")").attr({ 'x': '65', 'y': '20' });


                        break;

                    case 'pie':

                        var data2 = google.visualization.arrayToDataTable(graphdata);
                        $("#GraphicalViewDiv").append($("<div id=" + options.chartDivID + " class='graph'></div>"));
                        columnChart2 = new google.visualization.PieChart(document.getElementById(options.chartDivID));
                        var pieOptions =
                            {
                                title: options.Title,
                                width: size_width, //Set container width
                                height: 200, //Set container height                        
                                titleTextStyle: { bold: true, fontSize: 13 },
                                is3D: true,
                                pieSliceTextStyle: { color: 'black', fontSize: '9', bold: true },
                                legend: { alignment: 'center', textStyle: { 'word-wrap': 'break-word' } },
                                chartArea: { left: '8%', width: '90%' },
                                pieSliceText: 'percentage'
                            };

                        var isNoSlices = false;

                        for (var count = 0; count < graphdata.length; count++) {
                            var countSecond = graphdata[count].length;
                            for (var sCount = 0; sCount < countSecond; sCount++) {
                                if (graphdata[count][sCount] == 0) {
                                    isNoSlices = true;
                                }
                            }
                        }

                        if (!isNoSlices) {
                            pieOptions.slices = {
                                1: { offset: 0.3 }
                            };
                        }

                        columnChart2.draw(data2, pieOptions);
                        $("text:contains(" + pieOptions.title + ")").each(function () {

                            if (this.textContent != undefined && this.textContent == pieOptions.title) {
                                $(this).attr({ 'x': '85', 'y': '20' });
                                return;
                            }
                        });

                        break;
                }
            }
            else {
                var data2 = google.visualization.arrayToDataTable(graphdata);
                $("#GraphicalViewDiv").append($("<div id=" + options.chartDivID + " class='graph'></div>"));
                columnChart2 = new google.visualization.PieChart(document.getElementById(options.chartDivID));
                var pieOptions =
                    {
                        title: options.Title,
                        width: size_width, //Set container width
                        height: 200, //Set container height                        
                        titleTextStyle: { bold: true, fontSize: 13 },
                        is3D: true,
                        pieSliceTextStyle: { color: 'black', fontSize: '9', bold: true },
                        slices: {
                            1: { offset: 0.3 }
                        },
                        legend: { alignment: 'center', textStyle: { 'word-wrap': 'break-word' } },
                        chartArea: { left: '8%', width: '90%' },
                        pieSliceText: 'percentage'
                    };


                columnChart2.draw(data2, pieOptions);
                $("text:contains(" + pieOptions.title + ")").each(function () {

                    if (this.textContent != undefined && this.textContent == pieOptions.title) {
                        $(this).attr({ 'x': '85', 'y': '20' });
                        return;
                    }
                });

                $("text:contains('No data')").each(function () {

                    this.textContent = "No Results for this Component";
                    $(this).css('color', 'grey');

                });
            }

        }

        function ValidateData() {
            if (reportData && reportData.lstUsageData && reportData.lstUsageData.length > 0)
                return true;
            return false;
        }

        function ddlViewIndexChanged(sender, args) {
            var option = args.get_item()._text;
            if (option === "Table") {
                $("#TableView").show();
                $("#GraphView").hide();
                if ((reportData && reportData.lstUsageData) && ($("#AjaxPanelResults").css("background-color") != "rgb(255, 255, 255)"))
                    $("#exportGridImgBtn").show();
                else
                    $("#exportGridImgBtn").hide();
            }
            else if (option === "Graphical") {
                $("#TableView").hide();
                $("#GraphView").show();
                $("#GraphView").css("display", "inline");
                $("#exportGridImgBtn").hide();

            }
            if ($("#AjaxPanelResults").css("background-color") == "rgb(255, 255, 255)") {
                $("#divDefaultMessage").hide();
                $("#DivBlackMsg").show();

            }
        }

        /******* Graphical view end code**********/
        /*End of usage statistic grid display*/

        function ddlLevelIndexChanged() {
            $.each(CriteriaController.FindNode("LevelFilter").Values, function (index, selectedItem) {
                if (selectedItem.CurrentlySelected)
                    var currentValue = selectedItem.Value.Text;
                if (currentValue === "School" || currentValue === "User") {
                    $("#criteriaHeaderDiv_School").find("div.left").html('School:');
                    $("#criteriaHeaderDiv_School").find("div.left").append('<spam style="color:red">*</spam>');
                    $("#criteriaHeaderDiv_School").find("div.left").attr('Required', true);
                    $("#criteriaHeaderDiv_UserName").css('display', '');
                    $("#selectedCritieriaDisplayArea_UserName").css('display', '');
                }
                else {
                    $("#criteriaHeaderDiv_School").find("div.left").html('School:');
                    $("#criteriaHeaderDiv_UserName").css('display', 'none');
                    $("#selectedCritieriaDisplayArea_UserName").css('display', 'none');

                }
            });


        }
    </script>
    <div style="overflow: auto">
    </div>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        div.RadToolTip {
            position: fixed !important;
        }

        .unorderedList {
            font-weight: normal;
            list-style-type: square;
            margin: 0px 0px 0px 15px;
            padding: 0px 0px 0px 0px;
        }

        #rightColumn {
            background-color: lightgray;
        }

        .questionInformation td {
            font-weight: bold;
            vertical-align: top;
        }

        .questionInformation span {
            display: block;
            margin: 0px 0px 10px 10px;
            font-weight: normal;
            font-size: small;
        }

        .itemFieldName {
            font-family: arial;
            font-weight: bold;
            font-size: 8pt;
            padding-left: 10px;
            padding-right: 3px;
            padding-top: 2px;
            float: left;
        }

        .itemActions {
            height: 16px;
            background-repeat: no-repeat;
            background-image: url('../../Images/AssessmentItemIcons.png');
            float: right;
            margin-right: 5px;
            cursor: pointer;
        }

        .itemCopyActions {
            height: 16px;
            background-repeat: no-repeat;
            background-image: url('../../Images/copy.gif');
            float: right;
            margin-right: 5px;
            cursor: pointer;
        }

        .flag_img {
            position: relative;
            left: -5px;
            margin-top: 5px;
        }

        .graph {
            width: 24.3%;
            margin: 4px;
            float: left;
            display: block;
        }

        #AjaxPanelResults {
            background-color: lightgray;
        }
    </style>
</asp:Content>
