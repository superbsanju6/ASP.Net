﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Approved.ascx.cs" Inherits="Thinkgate.Controls.LCO.Approved" %>
<telerik:RadAjaxPanel ID="approvedAjaxPanel" runat="server" LoadingPanelID="approvedLoadingPanel">
	<!-- Filter combo boxes -->
	<div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
		<telerik:RadComboBox ID="cmbSubject" runat="server" ToolTip="Select a grade" 
			Skin="Web20" Width="69"
			OnSelectedIndexChanged="cmb_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
            <ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbLEA" runat="server" ToolTip="Select a status" 
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
				
					<div ID="divListNoResults" runat="server" Visible="false" style="width: 100%; text-align: center;">No Approvals found for selected criteria.</div>
			
				<telerik:RadListBox runat="server" ID="lbxList" Width="100%" Height="185px">
				<ItemTemplate>
					<!-- First line -->
					<div>
						<asp:HyperLink ID="lnkListCourseName" runat="server" Target="_blank" Visible="True"><%# Eval("TestName")%></asp:HyperLink>
						<asp:Label ID="lblDesc" runat="server" ToolTip='<%# Eval("Description")%>'><%# Eval("ListDescription")%></asp:Label>

                        <asp:Image ID="testImg" Style="float: left; padding: 2px;" Width="47" Height="56" runat="server" ImageUrl='~/Images/proofed.png' />
					</div>
					
					<div>
                        <asp:Label ID="lblLEA" runat="server" ToolTip='<%# Eval("Description")%>'><%# Eval("ListDescription")%></asp:Label>
                    </div>
										
                    <div>
                        <asp:Label ID="lblApprovedDate" runat="server" ToolTip='<%# Eval("Description")%>'><%# Eval("ListDescription")%></asp:Label>
                    </div>
				</ItemTemplate>
			</telerik:RadListBox>
		</div>
	
</telerik:RadAjaxPanel>

<!-- Bottom action button(s) -->
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="approvedLoadingPanel"
	runat="server" />