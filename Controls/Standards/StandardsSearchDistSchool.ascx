<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardsSearchDistSchool.ascx.cs"
	Inherits="Thinkgate.Controls.Standards.StandardsSearchDistSchool" %>
<telerik:RadAjaxPanel ID="standardsSearchDistSchoolAjaxPanel" runat="server" LoadingPanelID="standardsSearchDistSchoolLoadingPanel">
	<div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
		<telerik:RadComboBox ID="cmbStandardSet" runat="server" ToolTip="Select a standard set" 
			Skin="Web20" Width="95"
			OnSelectedIndexChanged="cmbStandardSet_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true">
			<ItemTemplate>
				<span><%# Eval("Standard_Set")%></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbGrade" runat="server" ToolTip="Select a grade" 
			Skin="Web20" Width="95"
			OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true">
			<ItemTemplate>
				<span><%# Eval("Grade") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbSubject" runat="server" ToolTip="Select a subject" 
			Skin="Web20" Width="95"
			OnSelectedIndexChanged="cmbSubject_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
			<ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
	</div>

	<asp:Panel ID="pnlNoResults" runat="server" Visible="false" Height="100%">
		<div style="width: 100%; text-align: center;">
			No standards found for selected criteria.</div>
	</asp:Panel>

	<telerik:RadListBox runat="server" ID="lbxStandards" Width="100%" Height="205px" OnItemDataBound="lbxStandards_ItemDataBound">
		<ItemTemplate>
			<asp:HyperLink ID="lnkStandard" runat="server" NavigateUrl=""><%# Eval("ShortenedDescription")%></asp:HyperLink>
            <asp:Label ID="lblStandard" runat="server"><%# Eval("ShortenedDescription") %></asp:Label>
		</ItemTemplate>
	</telerik:RadListBox>

</telerik:RadAjaxPanel>

<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="standardsSearchDistSchoolLoadingPanel"
	runat="server" />
