<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourseCatalog.ascx.cs" Inherits="Thinkgate.Controls.LCO.CourseCatalog" %>
<telerik:RadAjaxPanel ID="pendingApprovalsAjaxPanel" runat="server" LoadingPanelID="pendingApprovalsLoadingPanel">
	<!-- Filter combo boxes -->
	<div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
		<telerik:RadComboBox ID="cmbGrade" runat="server" ToolTip="Select a Grade" 
			Skin="Web20" Width="69"
			OnSelectedIndexChanged="cmb_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px"> 
            <ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
        
        <telerik:RadComboBox ID="cmbSubject" runat="server" ToolTip="Select a Subject" 
			Skin="Web20" Width="69"
			OnSelectedIndexChanged="cmb_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px"> 
            <ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		
        <telerik:RadComboBox ID="cmbLEA" runat="server" ToolTip="Select a LEA" 
			Skin="Web20" Width="69"
			OnSelectedIndexChanged="cmb_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
            <ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
	</div>

	<!-- Pages -->
		<asp:HiddenField ID="currView" runat="server" Value="listView" ClientIDMode="Static"/>

		<!-- List View -->
		<div class="listView" id="divListView" runat="server">
					<div ID="divListNoResults"  runat="server" Visible="false" style="width: 100%; text-align: center;">No courses found.</div>
            <telerik:RadListBox runat="server" ID="lbxList" Width="100%" Height="185px">
				<ItemTemplate>
					<div>
						<asp:HyperLink ID="lnkListCourseName" runat="server"  Target="_blank" Visible="True"><%# Eval("TestName")%></asp:HyperLink>
					</div>
				</ItemTemplate>
			</telerik:RadListBox>
		</div>
</telerik:RadAjaxPanel>

<!-- Bottom action button(s) -->
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="pendingApprovalsLoadingPanel"
	runat="server" />