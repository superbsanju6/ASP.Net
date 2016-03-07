<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewInstructionalPlans.ascx.cs"
	Inherits="Thinkgate.Controls.Plans.ViewInstructionalPlans" %>

<telerik:RadAjaxPanel ID="viewPlansAjaxPanel" runat="server" LoadingPanelID="viewPlansLoadingPanel">
    <style type="text/css">
        .termLink 
        {
            padding-right: 5px;
        }
    </style>

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
		<telerik:RadComboBox ID="cmbCourse" runat="server" ToolTip="Select a course" 
			Skin="Web20" Width="69"
			OnSelectedIndexChanged="cmbCourse_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true">
			<ItemTemplate>
				<span><%# Eval("CourseName")%></span>
			</ItemTemplate>
		</telerik:RadComboBox>
	</div>

	<!-- Pages -->

		<!-- Graphic View -->
		<div class="graphicalView">
				<asp:Panel ID="pnlGraphicNoResults" runat="server" Visible="false" Height="190px">
					<div style="width: 100%; text-align: center;">No instructional plans found.</div>
				</asp:Panel>
			<telerik:RadListBox runat="server" ID="lbxGraphic" Width="100%" Height="190px" OnItemDataBound="lbxGraphic_ItemDataBound">
				<ItemTemplate>
					<!-- Hidden fields to pass parameters to javascript. -->
					<input id="inpGraphicPlanID" type="hidden" value='<%# Eval("ID")%>' runat="server" />
					<input id="inpGraphicPlanName" type="hidden" value='<%# Eval("PlanName")%>' runat="server" />
					<!-- Test icon -->
					<asp:Image ID="testImg" Style="float: left; padding: 2px;" Width="47" Height="56"
						ImageUrl='~/Images/calendar.png' runat="server" />

					<!-- Plan name, Line 1 -->
					<div>						
						<asp:HyperLink ID="lnkGraphicTestName" runat="server" NavigateUrl="" Target="_blank" Visible="True" 
                            Text='<%# Eval("PlanName")%>' />
					</div>
					
                    <!-- Line 2 -->
					<asp:Panel ID="graphicLine2Summary" runat="server" Visible="True">						
					</asp:Panel>
								
				</ItemTemplate>
			</telerik:RadListBox>
		</div>
</telerik:RadAjaxPanel>
<!-- Bottom action button(s) -->

<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="viewPlansLoadingPanel"
	runat="server" />
