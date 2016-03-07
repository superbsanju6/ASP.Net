<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClassSummary_Expanded.aspx.cs"
	Inherits="Thinkgate.Controls.Class.ClassSummary_Expanded" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<%--This height of 100% helps elements fill the page--%>
<head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=8" />
	<meta http-equiv="PRAGMA" content="NO-CACHE" />
	<meta http-equiv="Expires" content="-1" />
	<meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
	<link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
	<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
	<link href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet"
		type="text/css" runat="server" />
	<title></title>
	<base target="_self" />
	<telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
	</telerik:RadStyleSheetManager>
	<script type='text/javascript' src='../../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../../Scripts/jquery-migrate-1.1.0.min.js'></script>
	<script type='text/javascript' src='../../Scripts/jquery.scrollTo.js'></script>
	<script type="text/javascript">        var $j = jQuery.noConflict();</script>
	<script type="text/javascript" src="https://www.google.com/jsapi"></script>
	<script type="text/javascript">
		//Initiate chart object variables
		var columnChart;
		var barChart;
		var pieChart;

		function onRequestStart(sender, args) {
		    if (args.get_eventTarget().indexOf("btnPrintStudentRoster") >= 0)
		        args.set_enableAjax(false);
		}

		// Load the Visualization API and the piechart package.
		google.load('visualization', '1.0', { 'packages': ['corechart'] });

		// Set a callback to run when the Google Visualization API is loaded.
		google.setOnLoadCallback(generateCharts);

		// Callback that creates and populates a data table,
		// instantiates the pie chart, passes in the data and
		// draws it.
		function generateCharts() {
			drawChart({ rawDataInputID: 'genderRawData', labelsInputID: 'genderLabels', chartDivID: 'genderChartDiv', vAxisLabel: 'Percent', hAxisLabel: 'Gender', chartType: 'column' });
			drawChart({ rawDataInputID: 'raceRawData', labelsInputID: 'raceLabels', chartDivID: 'raceChartDiv', vAxisLabel: 'Percent', hAxisLabel: 'Race', chartType: 'pie' });
			drawChart({ rawDataInputID: 'subgroupRawData', labelsInputID: 'subgroupLabels', chartDivID: 'subgroupChartDiv', vAxisLabel: 'Subgroups', hAxisLabel: 'Percent', chartType: 'bar' });
		}

		function drawChart(options) {
			/*  OPTION VALUES

			- rawDataInputID: raw data hidden input ID
			- labelsInputID: labels hidden input ID
			- chartDivID: chart DIV container ID
			- vAxisLabel: vertical or y-axis chart label
			- hAxisLabel: horizontal or x-axis chart label
			- chartType: type of chart (column, bar, or pie)
			*/

			options = options || new Object();

			var vAxisLabel = options.vAxisLabel || '';
			var hAxisLabel = options.hAxisLabel || '';

			// Create the data table.
			var data = new google.visualization.DataTable();
			var rawData = eval($('#' + options.rawDataInputID).attr('value'));
			var labels = eval($('#' + options.labelsInputID).attr('value'));

			// Instantiate and draw chart
			switch (options.chartType) {
				case 'column':
					if (rawData.length == 0) {
						$('#' + options.chartDivID).html('<span style="font-style:italic;">No students have been added to this class.</span>');
						return;
					}

					//Add data column for labels
					data.addColumn('string', 'Labels');

					//Add data columns for each value, which also sets the legends
					for (var i = 0; i < rawData.length; i++) {
						data.addColumn('number', rawData[i][0] + ': ' + rawData[i][2], rawData[i][0]);
					}

					//Add blank data rows for each label
					data.addRows(labels.length);

					//Set label values
					for (var x = 0; x < labels.length; x++) {
						data.setValue(x, 0, labels[x]);
					}

					//Set graph values
					for (var y = 0; y < rawData.length; y++) {
						data.setValue(y, y + 1, rawData[y][1]);
					}
					columnChart = new google.visualization.ColumnChart(document.getElementById(options.chartDivID));
					columnChart.draw(data, {
						width: 200, //Set container width
						height: 160, //Set container height
						fontSize: 11,
						backgroundColor: '#b4b4b4',
						chartArea: { width: '30%', height: '80%', left: 50, top: 10 }, //Set graph dimensions inside container
						vAxis: { minValue: 0, maxValue: 1, gridlines: { count: 11 }, title: vAxisLabel, format: '###%' }, //Set number of gridlines going vertically, min/max vertical values, number format, and y-axis label
						hAxis: { title: hAxisLabel }, //Set x-axis label
						tooltip: { trigger: 'none' }, //Set hover text off
						colors: ['blue', '#09F']
					});

					function columnSelectHandler() {
						//Clear other chart selections
						if (pieChart) pieChart.setSelection();
						if (barChart) barChart.setSelection();

						var selection = columnChart.getSelection();
						var rosterGridContainer = $j('#rosterGrid_GridData');

						if (selection.length > 0) {
							var selectedColumnValue = data.getColumnId(selection[0].column);

							$j('table[class^="rgMasterTable"]', rosterGridContainer).each(function () {
								$j('tr', $j(this)).each(function (index) {
									if ($j(this).attr('gender') == selectedColumnValue) {
										$j('td', $j(this)).each(function () {
											$j(this).css('backgroundColor', '#FFC');
										});
									}
									else {
										$j('td', $j(this)).each(function () {
											$j(this).css('backgroundColor', '');
										});
									}
								});
							});
						}
						else {
							$j('table[class^="rgMasterTable"] tr td', rosterGridContainer).css('backgroundColor', '');
						}
					}
					//Add "select" event listener to highlight students belonging to group
					google.visualization.events.addListener(columnChart, 'select', columnSelectHandler);
					break;
				case 'bar':
					var barContainerHeight;
					var barChartHeight;

					if ($('#genderRawData').attr('value') == '' && $('#raceRawData').attr('value') == '' && rawData.length == 0) {
						$('#' + options.chartDivID).html('<span style="font-style:italic;">No students have been added to this class.</span>');
						return;
					}
					else if (rawData.length == 0) {
						$('#' + options.chartDivID).html('<span style="font-style:italic;">No students have been assigned to a subgroup for this class.</span>');
						return;
					}

					if (rawData.length == 1) {
						barContainerHeight = 144;
						barChartHeight = 50;
						$('#demoSpacer').css('height', '110px');
					}
					else {
						barContainerHeight = 244;
						barChartHeight = 200;
					}
					//Add data column for labels
					data.addColumn('string', 'Labels');

					//Add data columns for each value, which also sets the legends
					for (var i = 0; i < rawData.length; i++) {
						data.addColumn('number', rawData[i][0] + ': ' + rawData[i][2], rawData[i][3]);
					}

					//Add blank data rows for each label
					data.addRows(labels.length);

					//No label values for this bar graph as per ESD---------------

					//Set graph values
					for (var y = 0; y < rawData.length; y++) {
						data.setValue(y, y + 1, rawData[y][1]);
					}
					barChart = new google.visualization.BarChart(document.getElementById(options.chartDivID));
					barChart.draw(data, {
						width: 508, //Set container width
						height: barContainerHeight, //Set container height
						fontSize: 11,
						backgroundColor: '#b4b4b4',
						isStacked: true,
						chartArea: { width: 280, height: barChartHeight, left: 10, top: 10 }, //Set graph dimensions inside container
						vAxis: { title: vAxisLabel }, //Set y-axis label
						hAxis: { minValue: 0, maxValue: 1, title: hAxisLabel, format: '###%' }, //Set number of gridlines going horizontally, min/max horizontal values, number format, and x-axis label
						tooltip: { trigger: 'none'} //Set hover text off
					});

					function barSelectHandler() {
						//Clear other chart selections
						if (pieChart) pieChart.setSelection();
						if (columnChart) columnChart.setSelection();

						var selection = barChart.getSelection();
						var rosterGridContainer = $j('#rosterGrid_GridData');

						if (selection.length > 0) {
							var selectedColumnValue = data.getColumnId(selection[0].column);

							$j('table[class^="rgMasterTable"]', rosterGridContainer).each(function () {
								$j('tr', $j(this)).each(function (index) {
									if ($j(this).attr('demoField' + selectedColumnValue) == 'Yes') {
										$j('td', $j(this)).each(function () {
											$j(this).css('backgroundColor', '#FFC');
										});
									}
									else {
										$j('td', $j(this)).each(function () {
											$j(this).css('backgroundColor', '');
										});
									}
								});
							});
						}
						else {
							$j('table[class^="rgMasterTable"] tr td', rosterGridContainer).css('backgroundColor', '');
						}
					}
					//Add "select" event listener to highlight students belonging to group
					google.visualization.events.addListener(barChart, 'select', barSelectHandler);
					break;
				case 'pie':
					if (rawData.length == 0) {
						$('#' + options.chartDivID).html('<span style="font-style:italic;">No students have been added to this class.</span>');
						return;
					}

					//Add data column for labels
					data.addColumn('string', 'Labels');
					//Add data column for counts
					data.addColumn('number', 'Counts');

					//Add data rows
					for (var i = 0; i < rawData.length; i++) {
						data.addRow([rawData[i][0] + ': ' + rawData[i][2], { v: (rawData[i][1] * 100), f: rawData[i][0]}]);
					}

					pieChart = new google.visualization.PieChart(document.getElementById(options.chartDivID));
					pieChart.draw(data, {
						width: 280, //Set container width
						height: 160, //Set container height
						fontSize: 11,
						backgroundColor: '#b4b4b4',
						chartArea: { width: '90%', height: '90%', top: 5, left: 0 }, //Set graph dimensions inside container
						tooltip: { trigger: 'none' }, //Set hover text off
						is3D: true
					});

					function pieSelectHandler() {
						//Clear other chart selections
						if (columnChart) columnChart.setSelection();
						if (barChart) barChart.setSelection();

						var selection = pieChart.getSelection();
						var rosterGridContainer = $j('#rosterGrid_GridData');
						if (!rosterGridContainer) return;

						if (selection.length > 0) {
							var selectedColumnValue = data.getFormattedValue(selection[0].row, 1);

							$j('table[class^="rgMasterTable"]', rosterGridContainer).each(function () {
								$j('tr', $j(this)).each(function (index) {
									if ($j(this).attr('race') == selectedColumnValue) {
										$j('td', $j(this)).each(function () {
											$j(this).css('backgroundColor', '#FFC');
										});
									}
									else {
										$j('td', $j(this)).each(function () {
											$j(this).css('backgroundColor', '');
										});
									}
								});
							});
						}
						else {
							$j('table[class^="rgMasterTable"] tr td', rosterGridContainer).css('backgroundColor', '');
						}
					}
					//Add "select" event listener to highlight students belonging to group
					google.visualization.events.addListener(pieChart, 'select', pieSelectHandler);
					break;
			}
		}

		function clearSelections(sender, args) {
			//Clear chart selections
			if (pieChart) pieChart.setSelection();
			if (columnChart) columnChart.setSelection();
			if (barChart) barChart.setSelection();
			
			//Start: BugID#:129 , 17 Dec 2012: Yogesh Raj, To unselect pie chart after clearing highlights
			generateCharts();
			//End: BugID#:129 , 17 Dec 2012: Yogesh Raj, To unselect pie chart after clearing highlights

			//Clear highlights
			var rosterGridContainer = $j('#rosterGrid_GridData');
			if (!rosterGridContainer) return;

			$j('table[class^="rgMasterTable"] tr td', rosterGridContainer).css('backgroundColor', '');
		}
	</script>
	<style>
		.floatRight
		{
			float: right;
			margin-right: 2px;
		}
		
		a.blueLink
		{
			color: #00F;
		}
		
		a.blueLink:active
		{
			color: #00F;
		}
		
		a.blueLink:hover
		{
			color: #00F;
		}
		
		a.blueLink:visited
		{
			color: #00F;
		}
	</style>
</head>
<body>
	<form id="form2" runat="server" style="height: 100%;">
	<telerik:RadScriptManager ID="RadScriptManager2" runat="server">
		<Scripts>
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
			<asp:ScriptReference Path="~/Scripts/master.js" />
		</Scripts>
	</telerik:RadScriptManager>
	<telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
		Skin="Thinkgate_Window" EnableEmbeddedSkins="False" Modal="True" InitialBehaviors="Close"
		Animation="None">
	</telerik:RadWindowManager>
	<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" ClientEvents-OnRequestStart="onRequestStart">
	</telerik:RadAjaxManager>
	<telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
	</telerik:RadSkinManager>
	<telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel" ClientEvents-OnRequestStart="onRequestStart"
		Width="100%">
		<table style="width: 100%; height: 620px;">
			<tr>
				<td style="width: 508px; vertical-align: top; padding: 5px;">
					<h1 class="dashboardSection">
						Identification</h1>
					<table class="fieldValueTableSmall" style="width: 100%;">
						<tr>
							<td class="fieldLabel" style="width: 15%;">
								Grade:
							</td>
							<td style="width: 25%;">
								<asp:Label runat="server" ID="lblGrade" />
							</td>
							<td style="width: 15%;" class="fieldLabel">
								Semester:
							</td>
							<td>
								<asp:Label runat="server" ID="lblSemester" />
							</td>
						</tr>
						<tr>
							<td class="fieldLabel">
								Subject:
							</td>
							<td>
								<asp:Label runat="server" ID="lblSubject" />
							</td>
							<td class="fieldLabel">
								Year:
							</td>
							<td>
								<asp:Label runat="server" ID="lblYear" />
							</td>
						</tr>
						<tr>
							<td class="fieldLabel">
								Course:
							</td>
							<td>
								<asp:Label runat="server" ID="lblCourse" />
							</td>
							<td class="fieldLabel">
								Block:
							</td>
							<td>
								<asp:Label runat="server" ID="lblBlock" />
							</td>
						</tr>
						<tr>
							<td class="fieldLabel">
								Section:
							</td>
							<td>
								<asp:Label runat="server" ID="lblSection" />
							</td>
							<td class="fieldLabel">
								School:
							</td>
							<td>
								<asp:Label runat="server" ID="lblSchoolName" />
							</td>
						</tr>
						<tr>
							<td class="fieldLabel">
								Period:
							</td>
							<td>
								<asp:Label runat="server" ID="lblPeriod" />
							</td>
							<td>
							</td>
							<td>
							</td>
						</tr>
					</table>
					<div id="demoSpacer" style="height: 20px;">
					</div>
					<h1 class="dashboardSection">
						Demographics</h1>
					<!--Div that will hold the pie chart-->
					<input type="hidden" id="genderRawData" runat="server" />
					<input type="hidden" id="genderLabels" runat="server" />
					<input type="hidden" id="raceRawData" runat="server" />
					<input type="hidden" id="raceLabels" runat="server" />
					<input type="hidden" id="subgroupRawData" runat="server" />
					<input type="hidden" id="subgroupLabels" runat="server" />
					<table width="100%" class="fieldValueTableSmall">
						<tr>
							<td class="fieldLabel">
								Gender
							</td>
							<td class="fieldLabel">
								Race
							</td>
						</tr>
						<tr>
							<td>
								<div id="genderChartDiv">
								</div>
							</td>
							<td>
								<div id="raceChartDiv">
								</div>
							</td>
						</tr>
						<tr>
							<td class="fieldLabel" colspan="2">
								Subgroups
							</td>
						</tr>
						<tr>
							<td colspan="2">
								<div id="subgroupChartDiv">
								</div>
							</td>
						</tr>
					</table>
				</td>
				<td style="vertical-align: top; background-color: #D0D0D0; padding: 5px;">
					<div style="height: 108px; overflow: auto;" id="curriculumContainer">
						<table border="0" width="100%" id="curriculumTable" runat="server">
							<tr>
								<td>
									<h1 class="dashboardSection">
										Curriculum</h1>
								</td>
							</tr>
						</table>
					</div>
					<div style="height: 100px; overflow: auto;" id="teachersContainer" >
						<table border="0" width="100%" id="teachersTable" runat="server">
							<tr>
								<td>
									<h1 class="dashboardSection">
										Teacher</h1>
								</td>
							</tr>
						</table>
					</div>
					<div style="height:24px;">
						<h1 class="dashboardSection">
							Roster: <span runat="server" id="studentCountSpan" style="font-style: italic; font-size: 11pt;
								color: #000;"></span>
                            <asp:ImageButton ID="btnPrintStudentRoster" runat="server" ImageUrl="~/Images/Printer.png" OnClick="btnPrintStudentRoster_Click" />
							<telerik:RadButton runat="server" ID="clearSelectionsButton" Text="Clear Highlights"
								Skin="Web20" OnClientClicked="clearSelections" AutoPostBack="false" CssClass="floatRight">
							</telerik:RadButton>
						</h1>
					</div>
					<div style="height: 345px;">
						<telerik:RadGrid runat="server" ID="rosterGrid" AutoGenerateColumns="false" Width="385"
							Height="375" CellPadding="0" OnItemDataBound="Roster_ItemDataBound">
							<MasterTableView HeaderStyle-Font-Size="8" HeaderStyle-HorizontalAlign="Left">
								<Columns>
                                    <telerik:GridHyperLinkColumn DataTextField="StudentName" UniqueName="name" ItemStyle-ForeColor="Blue" HeaderText="Name" ItemStyle-Font-Size="8" Target="_blank"></telerik:GridHyperLinkColumn>
									<telerik:GridBoundColumn DataField="Grade.DisplayText" HeaderText="Grade" ItemStyle-Font-Size="8"></telerik:GridBoundColumn>
									<telerik:GridBoundColumn UniqueName="studentid" DataField="StudentID" HeaderText="Student&nbsp;ID" ItemStyle-Font-Size="8"></telerik:GridBoundColumn>
									<telerik:GridBoundColumn DataField="Cohort" HeaderText="Cohort" ItemStyle-Font-Size="8"></telerik:GridBoundColumn>
								</Columns>
							</MasterTableView>
							<ClientSettings>
								<Scrolling AllowScroll="true" UseStaticHeaders="false" SaveScrollPosition="true" />
							</ClientSettings>
						</telerik:RadGrid>
					</div>
				</td>
			</tr>
		</table>
	</telerik:RadAjaxPanel>
	<telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
	</form>
</body>
</html>
