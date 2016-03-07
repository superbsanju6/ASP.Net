
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DistractorAnalysis.ascx.cs"
		Inherits="Thinkgate.Controls.Reports.DistractorAnalysis" %>
<table class="reportTable">
		<tr>
				<td>
						# Schools:
						<asp:Label runat="server" ID="lblSchoolCount" />
				</td>
				<td>
						# Teachers:
						<asp:Label runat="server" ID="lblTeacherCount" />
				</td>
				<td>
						# Classes:
						<asp:Label runat="server" ID="lblClassCount" />
				</td>
				<td>
						# Students:
						<asp:Label runat="server" ID="lblStudentCount" />
				</td>
		</tr>
		<tr>
				<td>
						High:
						<asp:Label runat="server" ID="lblHigh" />
				</td>
				<td>
						Low:
						<asp:Label runat="server" ID="lblLow" />
				</td>
				<td>
						Median:
						<asp:Label runat="server" ID="lblMedian" />
				</td>
				<td>
						Mean:
						<asp:Label runat="server" ID="lblMean" />
				</td>
		</tr>
</table>
<br />

<style type="text/css">
    .imgButtons 
        {
            padding: 2px;
            display: inline;
            float: right;
            margin-left: 10px;
            cursor: pointer;
        }
</style>
<script type="text/javascript">
    function printExcel() {
        var excelButton = document.getElementById('exportGridImgBtn');
        if (excelButton) {
            excelButton.click();
        }
    }

    function printPDFViewDistractorAnalysis() {
        var highlightCorrect = document.getElementById('highlightCorrect_checkbox').checked;
        var highlightIncorrect = document.getElementById('highlightIncorrect_checkbox').checked;
        var hideStudentID = document.getElementById('chkHideStudentID_checkbox').checked;
        var hideStudentName = document.getElementById('chkHideStudentName_checkbox').checked;
        var showRigor = document.getElementById('showRigor_checkbox').checked;

        var url = window.location.href + '&printPDFView=yes&highlightCorrect=' + (highlightCorrect ? 'yes' : 'no') + '&highlightIncorrect=' + (highlightIncorrect ? 'yes' : 'no')
            + '&hdStdID=' + (hideStudentID ? 'yes' : 'no') + '&hdStdName=' + (hideStudentName ? 'yes' : 'no')
            + '&showRigor=' + (showRigor ? 'yes' : 'no')
            + '&guid=' + '<%= Guid %>';
        window.open(url, 'Distractor_Analysis_PDFView');
    }
</script>
<telerik:RadComboBox ID="rcbDistractorAnalysisForms" ClientIDMode="Static"  runat="server" Skin="Web20" OnSelectedIndexChanged="ctxForm_ItemClick" AutoPostBack="true" classID="" levelID=""></telerik:RadComboBox>
<asp:ImageButton runat="server" ID="printButton" ClientIDMode="Static" CssClass="imgButtons" ImageUrl="~/Images/Printer.png"
                OnClientClick="printPDFViewDistractorAnalysis();/*setTimeout(function() {window.radalert('Functionality is under construction.');}, 0);*/ return false;" />
<asp:ImageButton ID="excelImgBtn" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
                 Width="20px" CssClass="imgButtons" OnClientClick="printExcel(); return false;" />

<div class="tblContainer" style="width: 100%; height: 445px;">
		<div class="tblRow" style="height: 445px;">
			<div id="criteriaHolder" class="tblLeft" style="background-color: #d1c9ca; padding-top: 3px; vertical-align:top;">
					<asp:PlaceHolder ID="criteriaPlaceholder" runat="server"></asp:PlaceHolder>
			</div>

			<div id="criteriaScroller" class="tblMiddle" style="width: 10px; height: 100%; vertical-align: top; background-color: #CCCCCC;">
				<div id="columnExpanderHandle" onclick="criteriaSliderGo();" style="cursor: pointer;
                                                                                                                                                                                                                                                                                            height: 100px; background-color: #0F3789; position: relative; top: 42%;">
					<asp:Image runat="server" ID="columnExpanderHandleImage" ClientIDMode="Static" Style="position: relative;
                                                                                                                                                                                                                                                                                                                                                                            left: 1px; top: 40px; width: 8px" ImageUrl="~/Images/arrow_gray_left.gif" />
				</div>
			</div>

			<div class="tblRight" style="width: 100%;  height: 445px; vertical-align: top;">
				<div style="width: 100%; height:465px; overflow: hidden;">
								
			        <div style="text-align:center;">
                        <!--<asp:CheckBox ID="chkHideStudentName" ClientIDMode="Static" Text="Hide Student Name" style="font-size: 10pt; margin-right:5px;margin-bottom:3px;" runat="server" OnCheckedChanged="chk_CheckedChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="chkHideStudentID" ClientIDMode="Static" Text="Hide Student ID" style="font-size: 10pt; margin-right:5px;margin-bottom:3px;" runat="server" OnCheckedChanged="chk_CheckedChanged" AutoPostBack="true" />-->
			            <input runat="server" id="chkHideStudentName_checkbox" clientidmode="Static" type="checkbox" value="studentname" onclick="adjustCells(this);" />
			            <span runat="server" id="chkHideStudentName_span" style="margin-right:5px;margin-bottom:3px;font-size:10pt;">Hide Student Name</span>
                        <input runat="server" id="chkHideStudentID_checkbox" clientidmode="Static" type="checkbox" value="studentid" onclick="adjustCells(this);" />
			            <span runat="server" id="chkHideStudentID_span" style="margin-right:5px;margin-bottom:3px;font-size:10pt;">Hide Student ID</span>
                        <input runat="server" id="highlightCorrect_checkbox" clientidmode="Static" type="checkbox" value="correct" onclick="adjustCells(this);" />
			            <span runat="server" id="highlightCorrect_span" style="margin-right:5px;margin-bottom:3px;font-size:10pt;">Highlight Correct</span>
			            <input runat="server" id="highlightIncorrect_checkbox" clientidmode="Static" type="checkbox" value="incorrect" onclick="adjustCells(this);" />
			            <span runat="server" id="highlightIncorrect_span" style="margin-right:5px;margin-bottom:3px;font-size:10pt;">Highlight Incorrect</span>
			            <input runat="server" id="showStandards_checkbox" clientidmode="Static" type="checkbox" value="standards" onclick="adjustCells(this);" />
			            <span runat="server" id="showStandards_span" style="margin-right:5px;margin-bottom:3px;font-size:10pt;">Show Standards</span>
			            <input runat="server" id="showRigor_checkbox" clientidmode="Static" type="checkbox" value="rigor" onclick="adjustCells(this);" />
			            <span runat="server" id="showRigor_span" style="margin-right:5px;margin-bottom:3px;font-size:10pt;"></span>
			        </div>
                    <div style="text-align:center;">
                        <span>* Indicates that this question was not included in scoring.</span>
                    </div>
                    <asp:Label ID="lblNoRecords" Font-Size="20" runat="server" Width="100%" Visible="false" style="text-align: center; align-content: center; margin-top: 10px;" Text="No Data Available for This Form"></asp:Label>
				    <telerik:RadGrid runat="server" CssClass="DistractorAnalysisRadGrid" ID="reportGrid" AutoGenerateColumns="true" OnItemDataBound="reportGrid_ItemDataBound" 
                    OnSortCommand="ReportGrid_SortCommand" Width="735" Height="430" AllowFilteringByColumn="false" AllowSorting="true">
						<MasterTableView AllowMultiColumnSorting="true" />
						<ClientSettings>
							<Scrolling AllowScroll="true" FrozenColumnsCount="3" UseStaticHeaders="True" SaveScrollPosition="true" />
						</ClientSettings>
				    </telerik:RadGrid>
                    <span runat="server" id="distractorContent">
                    </span>
				</div>
			</div>
		</div>
</div>