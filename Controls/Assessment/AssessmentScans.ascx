<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentScans.ascx.cs"
	Inherits="Thinkgate.Controls.Assessment.AssessmentScans" %>
<script type="text/javascript">
    function openScansSearchExpanded() {
        var assessmentID = $('#scansearch_hiddenAssessmentID').attr('value');

        var url = '../Dialogues/Assessment/AssessmentScans.aspx?xID=' + assessmentID + '&level=assessment';
        customDialog({ url: url, maxwidth: 950, maxheight: 675, maximize:true });
    }
</script>
<style>
    .RadGrid .rgHeader
    {
        background-color: rgb(127, 165, 215) !important;
    }
</style>
<asp:HiddenField runat="server" ID="scansearch_hiddenAssessmentID" ClientIDMode="Static" />
<telerik:RadAjaxPanel ID="assessmentsResultsAjaxPanel" runat="server" LoadingPanelID="assessmentResultsLoadingPanel">
    <telerik:RadGrid ID="grdScans" runat="server" AllowSorting="true" OnNeedDataSource="grdScans_NeedDataSource" Skin="Web20" AutoGenerateColumns="false" Height="225px" OnItemDataBound="GrdScans_ItemDataBound">
		<ClientSettings Selecting-AllowRowSelect="false" EnableRowHoverStyle="True">
			<Selecting AllowRowSelect="False" />
			<Scrolling AllowScroll="True" UseStaticHeaders="true" />
		</ClientSettings>
		<SortingSettings SortedBackColor="Bisque" EnableSkinSortStyles="false" />

		<MasterTableView Width="97%">
			<Columns>
				<telerik:GridTemplateColumn HeaderText="Job ID" ShowSortIcon="true" SortExpression="JobID" SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
					<ItemTemplate>
						<span><%# Eval("JobID") %></span>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Reject/Repair" ShowSortIcon="false" >
					<ItemTemplate>
						<asp:Image ID="btnRepair" ToolTip="Reject/Repair Scores" ImageUrl="~/Images/tools.png" runat="server" />
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Date/Time" ShowSortIcon="true" SortExpression="fileDate" SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
					<ItemTemplate>
						<span><%# Eval("fileDate")%></span>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="View" ShowSortIcon="false" >
					<ItemTemplate>
						<asp:Image ID="btnViewFile" ToolTip="View File" ImageUrl="~/Images/ViewPage.png" runat="server"
								style="cursor: pointer; visibility: visible; width: 18px; height: 24px;" />
					</ItemTemplate>
				</telerik:GridTemplateColumn>
			</Columns>
		</MasterTableView>
	</telerik:RadGrid>
    <span id="scansSearchMoreLinkSpan" class="searchMoreLinkSpan_smallTile"><a runat="server"
    id="scansSearchMoreLink" visible="false" href="#" onclick="openScansSearchExpanded(); return false;">
    more results... </a></span>

	<script type="text/javascript">
	    // Cause the old systems' Reject-Repair window to open with the passed job id.
	    function openRejectRepair(jobid) {
	        open('../SessionBridge.aspx?ReturnURL=' + escape('display.asp?formatoption=search%20results&key=7109&retrievemode=searchpage&??KF=jobid&??KeyID=') + jobid + escape('&??Days=0'), 'rejectrepair', 'height=560, width=900, location=no, menubar=no, resizable=yes, scrollbars=yes, status=no, toolbar=no');
	    }

	    // View the csv file for a scan.
	    function viewFile(filename) {
	        //			alert(filename);
	        open('../SessionBridge.aspx?ReturnURL=' + escape('DBACCESS/') + filename, 'fileview', 'height=560, width=770, location=no, menubar=no, resizable=yes, scrollbars=yes, status=no, toolbar=no');
	    }
	</script>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="assessmentResultsLoadingPanel"
	runat="server" />
