<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewAssessments.ascx.cs"
	Inherits="Thinkgate.Controls.Assessment.ViewAssessments" %>
<telerik:RadAjaxPanel ID="viewAssessmentsAjaxPanel" runat="server" LoadingPanelID="viewAssessmentsLoadingPanel">
	<!-- Filter combo boxes -->
	<div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
		<telerik:RadComboBox ID="cmbGrade" runat="server" ToolTip="Select a grade" 
			Skin="Web20" Width="69"
			OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true">
			<ItemTemplate>
				<span><%# Eval("Grade") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbSubject" runat="server" ToolTip="Select a subject" 
			Skin="Web20" Width="69"
			OnSelectedIndexChanged="cmbSubject_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
			<ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbTerm" runat="server" ToolTip="Select a term" 
			Skin="Web20" Width="69"
			OnSelectedIndexChanged="cmbTerm_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true">
			<ItemTemplate>
				<span><%# Eval("Term") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbTestType" runat="server" ToolTip="Select a test type" 
			Skin="Web20" Width="69"
			OnSelectedIndexChanged="cmbTestType_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
			<ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbStatus" runat="server" ToolTip="Select a test status" 
			Skin="Web20" Width="75"
			OnSelectedIndexChanged="cmbStatus_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true">
			<ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
	</div>

	<!-- Pages -->
		<asp:HiddenField ID="currView" runat="server" Value="graphicalView" ClientIDMode="Static"/>

		<!-- List View -->
		<div class="listView" id="divListView" runat="server">
				<asp:Panel ID="pnlListNoResults" runat="server" Visible="false" Height="190px">
					<div style="width: 100%; text-align: center;">No assessments found for selected criteria.</div>
				</asp:Panel>
				<telerik:RadListBox runat="server" ID="lbxList" Width="100%" Height="190px" OnItemDataBound="lbxList_ItemDataBound">
				<ItemTemplate>
					<!-- First line -->
					<div>
						<asp:Label ID="lblListTestName" runat="server" Visible="False"><%# Eval("TestName")%></asp:Label>
						<asp:HyperLink ID="lnkListTestName" runat="server" NavigateUrl="" Target="_blank" Visible="True"><%# Eval("TestName")%></asp:HyperLink>
						<asp:Label ID="lblDesc" runat="server" ToolTip='<%# Eval("Description")%>'><%# Eval("ListDescription")%></asp:Label>
					</div>
					<!-- More Results line -->
					<asp:Panel ID="listMore" runat="server" Visible="False">
						<a href="#" id="lnkListMore" runat="server" onclick="">More Results...</a>
					</asp:Panel>
					<!-- Second line, non edit mode -->
					<asp:Panel ID="listLine2Summary" runat="server" Visible="True">
                        <asp:HyperLink ID="btnListUpdate" runat="server" ImageUrl="~/Images/Edit.png"
                            ToolTip="Update" Style="margin-left: 30px;" />
                        <asp:HyperLink ID="imgListPrint1" runat="server" ImageUrl="~/Images/Printer.png"
                            ToolTip="Print" NavigateUrl="javascript:alert('Missing event target');" />
                        <asp:HyperLink ID="imgListAdmin" runat="server" ImageUrl="~/Images/dashboard_small.png"
                            ToolTip="Administration" NavigateUrl="javascript:alert('Missing event target');" />
						<%# GetSummaryElement(Container.DataItem) %>
						<span style="margin-left: 4px;">Administered:</span>
						<asp:Label ID="lblListTestScored" runat="server" Visible="False"><%# Eval("Scored")%>/<%# Eval("Seated")%> (<%# Eval("Percentage")%>%)</asp:Label>
						<asp:Label ID="lblListTestScoredPct" runat="server"  Visible="False"><%# Eval("Percentage")%>%</asp:Label>
						<asp:HyperLink ID="lnkListTestScored" runat="server"  NavigateUrl="#" Target="_blank" onclick='alert("Function is in development"); return false;'><%# Eval("Scored")%>/<%# Eval("Seated")%> (<%# Eval("Percentage")%>%)</asp:HyperLink>
						<asp:HyperLink ID="lnkListTestScoredPct" runat="server"  NavigateUrl="#" Target="_blank" onclick='alert("Function is in development"); return false;'><%# Eval("Percentage")%>%</asp:HyperLink>
					</asp:Panel>
					<!-- Second line, edit mode -->
					<asp:Panel ID="listLine2Edit" runat="server" Visible="False">
						<!-- <span><%# Eval("Seated")%> Items &nbsp</span> -->
                        <asp:HyperLink ID="btnListEdit" runat="server" ImageUrl="~/Images/Edit.png"
                            ToolTip="Edit" Style="margin-left: 30px;" />
                        <asp:HyperLink ID="imgListPrint2" runat="server" ImageUrl="~/Images/Printer.png"
                            ToolTip="Print" NavigateUrl="javascript:alert('Missing event target');" />
						<span>&nbsp Last Edited:&nbsp<%# Eval("DateEdited")%></span></asp:Panel>
				</ItemTemplate>
			</telerik:RadListBox>
		</div>

		<!-- Graphic View -->
		<div class="graphicalView" id="divGraphicalView" runat="server">
				<asp:Panel ID="pnlGraphicNoResults" runat="server" Visible="false" Height="190px">
					<div style="width: 100%; text-align: center;">No assessments found for selected criteria.</div>
				</asp:Panel>
			<telerik:RadListBox runat="server" ID="lbxGraphic" Width="100%" Height="190px" OnItemDataBound="lbxList_ItemDataBound">
				<ItemTemplate>
					<!-- Hidden fields to pass parameters to javascript. -->
					<input id="inpGraphicAssessmentID" type="hidden" value='<%# Eval("TestID")%>' runat="server" />
					<input id="inpGraphicTestName" type="hidden" value='<%# Eval("TestName")%>' runat="server" />
					<!-- Test icon -->
					<asp:Image ID="testImg" Style="float: left; padding: 2px;" Width="47" Height="56"
						ImageUrl='~/Images/editable.png' runat="server" />

					<!-- Test name, Line 1 -->
					<div>
						<asp:Label ID="lblGraphicTestName" runat="server" Visible="False"><%# Eval("TestName")%></asp:Label>
						<asp:HyperLink ID="lnkGraphicTestName" runat="server" NavigateUrl="" Target="_blank" Visible="True"><%# Eval("TestName")%></asp:HyperLink>
						<asp:Label ID="lblDesc" runat="server" ToolTip='<%# Eval("Description")%>'><%# Eval("GraphicDescription")%></asp:Label>
					</div>
					<!-- More Results line -->
					<asp:Panel ID="graphicMore" runat="server" Visible="False">
						<a href="#" id="lnkGraphicMore" runat="server" onclick="">More Results...</a>
					</asp:Panel>

					<!-- Line 2 -->
					<asp:Panel ID="graphicLine2Summary" runat="server" Visible="True">
						<span style="margin-left: 4px;">Administered:</span>
						<asp:Label ID="lblGraphicTestScored" runat="server" Visible="False"><%# Eval("Scored")%>/<%# Eval("Seated")%> (<%# Eval("Percentage")%>%)</asp:Label>
						<asp:Label ID="lblGraphicTestScoredPct" runat="server"  Visible="False"><%# Eval("Percentage")%>%</asp:Label>
						<asp:HyperLink ID="lnkGraphicTestScored" runat="server"  NavigateUrl="#" Target="_blank" onclick='alert("Function is in development"); return false;'><%# Eval("Scored")%>/<%# Eval("Seated")%> (<%# Eval("Percentage")%>%)</asp:HyperLink>
						<asp:HyperLink ID="lnkGraphicTestScoredPct" runat="server"  NavigateUrl="#" Target="_blank" onclick='alert("Function is in development"); return false;'><%# Eval("Percentage")%>%</asp:HyperLink>
					</asp:Panel>
					<asp:Panel ID="graphicLine2Edit" runat="server" Visible="False">
						<span><%# Eval("NumItems")%> Items &nbsp</span>
                        <asp:HyperLink ID="btnGraphicEdit" runat="server" ImageUrl="~/Images/Edit.png"
                            ToolTip="Edit" Style="margin-left: 30px;" />
                        <asp:HyperLink ID="imgGraphicPrint1" runat="server" ImageUrl="~/Images/Printer.png"
                            ToolTip="Print" NavigateUrl="javascript:alert('Missing event target');" />
					</asp:Panel>

					<!-- Line 3 -->
					<asp:Panel ID="graphicLine3Summary" runat="server" Visible="True">
                        <asp:HyperLink ID="btnGraphicUpdate" runat="server" ImageUrl="~/Images/Edit.png"
                            ToolTip="Update" Style="margin-left: 30px;" />
                        <asp:HyperLink ID="imgGraphicPrint2" runat="server" ImageUrl="~/Images/Printer.png"
                            ToolTip="Print" NavigateUrl="javascript:alert('Missing event target');" />
                        <asp:HyperLink ID="imgGraphicAdmin" runat="server" ImageUrl="~/Images/dashboard_small.png"
                            ToolTip="Administration" NavigateUrl="javascript:alert('Missing event target');" />
						<%# GetSummaryElement(Container.DataItem) %>
												<img id="imgAddToCalendar" runat="server" src="~/Images/commands/calendar_add_small.png" alt="Add to Calendar" testid='<%# Eval("TestID")%>'
														 title="Add to Calendar" style="cursor: pointer;" onclick="if(ShowInstructionalPlanCalendar) ShowInstructionalPlanCalendar('assessmentID', this.getAttribute('testid'));" visible="false"/>
					</asp:Panel>
					<asp:Panel ID="graphicLine3Edit" runat="server" Visible="False">
						<span>Last Edited:&nbsp<%# Eval("DateEdited")%></span></asp:Panel>
				</ItemTemplate>
			</telerik:RadListBox>
		</div>
</telerik:RadAjaxPanel>
<!-- Bottom action button(s) -->
<div runat="server" id="BtnAdd" class="searchTileBtnDiv_smallTile" title="Add Assessment" style="margin-top: 1px;" 
		onclick="customDialog({ name: 'RadWindowAddAssessment', url: '../Dialogues/Assessment/CreateAssessmentOptions.aspx?level=' + this.getAttribute('level') + '&amp;testCategory=' + this.getAttribute('testCategory'), title: 'Options to Create Assessment', maximize: true, maxwidth: 550, maxheight: 450 });">
		<span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon">
		</span>
		<div style="padding: 0;">
				Add New</div>
</div>
<div runat="server" id="btnScheduler" class="searchTileBtnDiv_smallTile" title="Scheduler" style="margin-top: 1px;" 
		onclick="window.open('../Controls/Assessment/AssessmentScheduling.aspx?xID=' + this.getAttribute('xID') + '&amp;yID=' + this.getAttribute('yID'));" >
		<span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_SchedulerIcon">
		</span>
		<div style="padding: 0;">
				Scheduler</div>
</div>
<div runat="server" id="BtnTestEvents" class="searchTileBtnDiv_smallTile" title="Test Events" style="margin-top: 1px;"
    onclick="customDialog({name: 'RadWindowTestEvents', url: '../Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?navigateFrom=' + 'TestEvents', title: 'Assessment Search', maximize: true, maxwidth: 950, maxheight: 675});">
    <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_hourglassIcon">
    </span>

    <div style="padding: 0;">Test Events</div>

</div>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="viewAssessmentsLoadingPanel"
	runat="server" />


