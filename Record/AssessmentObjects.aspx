<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
		CodeBehind="AssessmentObjects.aspx.cs" Inherits="Thinkgate.Record.AssessmentObjects" %>

<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
	    <input id="lbl_TestID" type="hidden" clientidmode="Static" runat="server" />
	    <input id="lbl_OTCUrl" type="hidden" clientidmode="Static" runat="server" />
    <style type="text/css">
        .rwPopupButton {float:right !important}
    </style>
	<script type="text/javascript">
		
		function AssessmentObjectsPage_openSchedulerExpandEditWindow(sender, args) {
			var idEle = document.getElementById("MainContent_inpAssessmentID");
			var xID = idEle.value.toString();
			var proofedEle = document.getElementById("MainContent_inpAssessmentProofed");
			var proofed = (proofedEle.value == "True") ? "Yes" : "No";
		    var assessmentYear = document.getElementById('MainContent_inpAssessmentYear').value;

		    customDialog({ url: ('<%= ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>' + escape("fastsql_v2_direct.asp?ID=7098/E3Scheduler&??SCHEDULE=Assessment&??TESTID=") + xID + escape("&??PROOFED=") + proofed + escape("&??YEAR=") + assessmentYear),
		        autoSize: false, maximize: true, maxwidth: 1000, maxheight: 600, name: this.name
		    });
		}

		function AssessmentObjectsPage_SchedulerHelp() {
			customDialog({ url: ('../Dialogues/HelpPages/AssessmentSchedulerHelp.aspx'), autoSize: true, title: 'Help' });
		}

		function AssessmentObjectsPage_openExpandedWindow(sender, args) {
		    var idEle = document.getElementById("MainContent_inpAssessmentID");
//		    var assessmentYear = document.getElementById('MainContent_inpAssessmentYear').value;
//		    customDialog({ name: "AssessmentAssignmentSearch_ExpandedWindow", url: ('../Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?encrypted=false&assessmentID=' + idEle.value.toString() + escape("&??YEAR=") + assessmentYear), width: 950, height: 675 });
		    customDialog({ name: "AssessmentAssignmentSearch_ExpandedWindow", url: ('../Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?encrypted=false&assessmentID=' + idEle.value.toString()), maximize: true, maxwidth: 1000, maxheight: 675 });
		   }

	</script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
		<asp:Image runat='server' ID="assessmentImage" ImageUrl="~/Images/new/folder_assessment.png"
				Height="50px"  />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
		<asp:Label runat="server" ID="assessmentName" class="pageTitle" />
</asp:Content>
<asp:Content ID="foldersContent" ContentPlaceHolderID="FoldersContent" runat="server">
		<th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<input id="inpAssessmentID" type="hidden" value="" runat="server" />
	<input id="inpAssessmentName" type="hidden" value="" runat="server" />
	<input id="inpShowSelectPrompt" type="hidden" value="false" runat="server" />
	<input id="inpAssessmentCategory" type="hidden" value="" runat="server" />
	<input id="inpAssessmentProofed" type="hidden" value="" runat="server" />
    <input id="inpAssessmentYear" type="hidden" value="" runat="server" />

    <input runat="server" type="hidden" id="hiddenAccessSecureTesting" clientidmode="Static" name="hiddenAccessSecureTesting" value="" />
    <input runat="server" type="hidden" id="hiddenIsSecuredFlag" clientidmode="Static" name="hiddenIsSecuredFlag" value="" />
    <input runat="server" type="hidden" id="hiddenSecureType" clientidmode="Static" name="hiddenSecureType" value="" />

    <script type="text/javascript" src="../scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery.ui.touch-punch.js"></script>
    <script type="text/javascript" src="../Scripts/AssessmentEdit.js?j=2"></script>
    <script type="text/javascript" src="../Scripts/jsrender.js"></script>
    <script type="text/javascript" src="../Scripts/master.js"></script>
	<script type="text/javascript">
		var assessmentID = "";
		function actionsMenuItemClicked(sender, args) {
			var text = args.get_item().get_text();

			if (text == "Print")
				printAssessment();
		}

		// Show the print dialog.
		function printAssessment() {
			var assessmentIDEle = document.getElementById("MainContent_inpAssessmentID");
			assessmentID = assessmentIDEle.value;
			var assessmentNameEle = document.getElementById("MainContent_inpAssessmentName");
			var assessmentName = assessmentNameEle.value;

			// Either show the select upload prompt or just go to the print screen.
			if (document.getElementById("MainContent_inpShowSelectPrompt").value == 'True')
			    customDialog({ url: ('../Dialogues/Assessment/AssessmentVersionPrompt.aspx?xID=' + assessmentID.toString() + '&yID=' + escape(assessmentName)), autoSize: true, name: 'AssessmentPrint' });
			else 
			    customDialog({ url: ('../Dialogues/Assessment/AssessmentPrint.aspx?xID=' + assessmentID.toString() + '&yID=' + escape(assessmentName)), maximize: true, maxwidth: 520, maxheight: 400, name: 'AssessmentPrint' });
		}

	</script>
		<telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="assessmentTilesLoadingPanel"
				Width="100%" Height="100%">
				<th:DoublePanel runat="server" ID="ctlDoublePanel" />
		</telerik:RadAjaxPanel>
		<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="assessmentTilesLoadingPanel"
				runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
</asp:Content>
