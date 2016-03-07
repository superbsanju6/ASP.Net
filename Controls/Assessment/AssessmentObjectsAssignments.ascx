<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentObjectsAssignments.ascx.cs"
	Inherits="Thinkgate.Controls.Assessment.AssessmentObjectsAssignments" %>
<style type="text/css">
	.docTable
	{
		width: auto;
		font-size: 9pt;
		color: Black;
		margin-left: auto;
		margin-right: auto;
		margin-top: 8px;
	}
	.rptTable
	{
	}
	.cellCommon
	{
		padding-top: 2px;
		padding-bottom: 2px;
		padding-left: 4px;
		padding-right: 4px;
		font-size: 9pt;
		border-width: 1px;
		border-style: solid;
	}
	.cellContent
	{
		background-color: White;
		border-color: Black;
		text-align: center;
		vertical-align: middle;
	}
	
	.cellHeader
	{
		/*		background-image: url(../Images/HeaderBkgnd1.png); */
		background-color: rgb(219, 219, 218);
		color: Black;
		border-color: rgb(164, 171, 178);
		vertical-align: middle;
		text-align: left;
	}
	.headerBtn
	{
		width: 12;
		height: 12;
		vertical-align: middle;
	}
	.contentBtn
	{
		vertical-align: middle;
	}
	.visible
	{
		visibility: visible;
	}
	.notVisible
	{
		visibility: hidden;
	}

</style>

<script type="text/javascript">
	function btnAssessmentAdmin_ClientClicked(sender, args) {
		var v = sender.valueOf();
		var classID = v.get_value().toString();
		var assessmentID = document.getElementById("hiddenAssessmentID").value;
		parent.customDialog({ url: ('../Dialogues/Assessment/AssessmentAdministration.aspx?xID=' + assessmentID.toString() + '&yID=' + classID.toString()), autoSize: true });
	}

	function openAssignmentsSearchExpanded() {
		var assessmentID = document.getElementById("hiddenAssessmentID").value;
		parent.customDialog({ name: "AssessmentAssignmentSearch_ExpandedWindow", url: ('../Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?encrypted=false&assessmentID=' + assessmentID), maxwidth: 950, maxheight: 675, maximize:true });
	}


</script>

<asp:HiddenField ID="hiddenAssessmentID" runat="server" ClientIDMode="Static"/>

<telerik:RadAjaxPanel ID="rapAssessmentObjectsAssignments" runat="server" LoadingPanelID="assessmentObjectsAssignmentsLoadingPanel">
		<telerik:RadGrid ID="grdAssignments" runat="server" AllowSorting="true" Style="width: auto;"
			Skin="Web20" Font-Size="9pt" AllowMultiRowSelection="true"  Height="225px"
			AutoGenerateColumns="false" onneeddatasource="grdAssignments_NeedDataSource" OnItemDataBound="grdAssignments_ItemDataBound">
			<ClientSettings Selecting-AllowRowSelect="true" EnableRowHoverStyle="True">
				<Selecting AllowRowSelect="True" />
				<Scrolling AllowScroll="True" ScrollHeight="225px" UseStaticHeaders="false" />
			</ClientSettings>
			<SortingSettings SortedBackColor="Bisque" EnableSkinSortStyles="false" />
			<MasterTableView  Width="97%" DataKeyNames="ClassID" ClientDataKeyNames="ClassID">
				<Columns>
					<telerik:GridTemplateColumn ItemStyle-Wrap="false" ShowSortIcon="false">
						<ItemTemplate>
							<telerik:RadButton ID="btnAssessmentAdmin" runat="server" Text="Assessment Administration" Image-EnableImageButton="true" Image-ImageUrl="~/Images/dashboard_small.png" Width="21" Height="13"
								Value='<%# Eval("ClassID") %>' AutoPostBack="false" OnClientClicked="btnAssessmentAdmin_ClientClicked">
							</telerik:RadButton>
						</ItemTemplate>
						<ItemStyle Wrap="False"></ItemStyle>
					</telerik:GridTemplateColumn>
                    <telerik:GridHyperLinkColumn UniqueName="LnkLevelName" DataTextField="ClassName" NavigateUrl="" Target="_blank"
						HeaderText="Class" SortExpression="ClassName" HeaderStyle-HorizontalAlign="Center" ShowSortIcon="true"
                        SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif"/>
					<telerik:GridTemplateColumn HeaderText="Total Scored" ShowSortIcon="true" SortExpression="NumScored"
						SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
						<ItemTemplate>
							<span class='<%# ((Int32)Eval("HasResults")) == 0 ? "notVisible" : "visible"%>'><%# Eval("NumScored")%>/<%# Eval("NumSeated")%></span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Avg. Score" ShowSortIcon="true" SortExpression="Percentage"
						SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
						<ItemTemplate>
							<span class='<%# ((Int32)Eval("HasResults")) == 0 ? "notVisible" : "visible"%>'><%# Eval("Percentage")%>%</span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
		<span id="assignmentsSearchMoreLinkSpan" class="searchMoreLinkSpan_smallTile"><a runat="server"
		id="assignmentsSearchMoreLink" visible="false" href="#" onclick="openAssignmentsSearchExpanded(); return false;">
		more results... </a></span>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="assessmentObjectsAssignmentsLoadingPanel"
	runat="server" />
