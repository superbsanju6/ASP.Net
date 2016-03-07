<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
		CodeBehind="Reports.aspx.cs" Inherits="Thinkgate.Record.Reports" %>

<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
		<script type="text/javascript">
				function openItemAnalysisDetail(chartItem) {
						document.getElementById("itemAnalysis_selectedChartItem").value = chartItem;
						document.getElementById("itemAnalysis_btnSelectChartItem").click();
				}

				function criteriaSliderGo() {
						var $criteria_holder = $('#criteriaHolder');
						var current_state = $criteria_holder.attr('state');

						if (!$criteria_holder) return false;

						if (!current_state) {
								current_state = 'opened';
								$criteria_holder.attr('state', current_state);
						}
						 if (current_state == 'opened') {
							 $criteria_holder.stop().animate({ width: 1 }, 500, function () { $criteria_holder.hide(); });     // find that going doesn't work properly in Chrome and Safari. It's like the calculation goes haywire and the div jumps back to the full 240px wide instead of disappearing
								current_state = 'closed';
								$('#columnExpanderHandleImage').attr('src', '../Images/arrow_gray_right.gif');
								$('.DistractorAnalysisRadGrid').each(function () {
								    $(this).attr('style', 'width: 929px;');
										$(this).find('.rgHeaderDiv').attr('style', 'width: 929px;');
							});
						} else {
								//$('#criteriaScroller').css('overflow', 'hidden');
								$criteria_holder.stop().animate({ width: 200 }, 500, function () { $criteria_holder.show(); $('#criteriaScroller').css('overflow', 'hidden'); });
								current_state = 'opened';
								$('#columnExpanderHandleImage').attr('src', '../Images/arrow_gray_left.gif');
								$('.DistractorAnalysisRadGrid').each(function () {
								    $(this).attr('style', 'width: 735px;');
										$(this).find('.rgHeaderDiv').attr('style', 'width: 735px;');
								});

						}

						$criteria_holder.attr('state', current_state);
				}

				function printPDFView(analysis, extraParms) {
				    if (analysis) {
				        var url = '<%= string.IsNullOrEmpty(Thinkgate.Base.Classes.AppSettings.ClientID) || Thinkgate.Base.Classes.AppSettings.ClientID == "/" ? ".." : Thinkgate.Base.Classes.AppSettings.ClientID %>/ControlHost/AnalysisPDFView.aspx?' + extraParms;
				        window.open(url, 'Report_PDFView');
				    }
				    else {
				        var formDropdown = $find('rcbDistractorAnalysisForms');
				        var formValue = formDropdown ? formDropdown.get_selectedItem().get_value() : '';
				        var highlightCorrectInput = document.getElementById('highlightCorrect_checkbox');
				        var highlightCorrectValue = highlightCorrectInput ? highlightCorrectInput.checked : true;
				        var highlightIncorrectInput = document.getElementById('highlightIncorrect_checkbox');
				        var highlightIncorrectValue = highlightCorrectInput ? highlightIncorrectInput.checked : true;
				        var hideStudentName = document.getElementById('chkHideStudentName');
				        var hideStudentNameValue = hideStudentName ? hideStudentName.checked : true;
				        var hideStudentID = document.getElementById('chkHideStudentID');
				        var hideStudentIDValue = hideStudentID ? hideStudentID.checked : true;
				        var showRigorInput = document.getElementById('showRigor_checkbox');
				        var showRigorValue = showRigorInput ? showRigorInput.checked : true;
				        var url = window.location.href + '&printPDFView=yes&formID=' + formValue + '&highlightCorrect=' + (highlightCorrectValue ? "yes" : "no") +
    				        '&highlightIncorrect=' + (highlightIncorrectValue ? "yes" : "no") + '&showRigor=' + (showRigorValue ? "yes" : "no") + '&hdStdName=' + (hideStudentNameValue ? "yes" : "no") + '&hdStdID=' + (hideStudentIDValue ? "yes" : "no");
				        window.open(url, 'Report_PDFView');
				    }
				}
		</script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
		<asp:Image runat='server' ID="studentImage" ImageUrl="~/Images/new/folder_data_analysis.png" Height="50px" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
		
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" runat="server">
		<th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ExcelButtonContent" runat="server">
    <asp:ImageButton ID="exportGridImgBtn" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
                OnClick="ExportGridImgBtn_Click" Width="20px" Style="display:none;" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
		<telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="studentTilesLoadingPanel"
				Width="100%" Height="100%">
				<th:DoublePanel runat="server" ID="ctlDoublePanel" />
		</telerik:RadAjaxPanel>
		<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="studentTilesLoadingPanel"
				runat="server" />
				<asp:TextBox ID="hiddenTxtBox" runat="server" Style="visibility: hidden;" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
		
</asp:Content>
