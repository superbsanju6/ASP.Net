<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TGSecurityXref.ascx.cs"
	Inherits="Thinkgate.Controls.TGSecurityXref" %>
<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
<link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
<asp:Label ID="lblHeaderItemID" runat="server" Visible="False" Text=""></asp:Label>
<asp:Label ID="lblHeaderItemName" runat="server" Visible="True" Text=""></asp:Label>
<br />
<asp:Label ID="lblResultMessage" runat="server"></asp:Label><br />


<!-- Permissions in Roles -->
<telerik:RadTreeList ID="rtlRolePermissionsForEditing" runat="server" OnNeedDataSource="rtlRolePermissionsForEditing_NeedDataSource"
     ParentDataKeyNames="ParentID" DataKeyNames="ID" AutoGenerateColumns="False" Skin="Simple" EditMode="InPlace" OnUpdateCommand="rtlRolePermissionsForEditing_UpdateCommand">
    <SortExpressions>
                              <telerik:TreeListSortExpression FieldName="Name" SortOrder="Ascending"  runat="server"/>
                          </SortExpressions>
        <Columns>
		    <telerik:TreeListTemplateColumn DataField="PermissionName" SortExpression="PermissionName"
			    HeaderText="Permission Name" UniqueName="PermissionName">
			    <ItemTemplate>
				    <asp:Label ID="lblPermissionName" runat="server" Text='<%# Eval("Name") %>'
					    ToolTip='<%# Eval("Description") %>'></asp:Label>
			    </ItemTemplate>
			    <EditItemTemplate>
				    <asp:Label ID="lblPermissionName" runat="server" Text='<%# Bind("Name") %>'
					    ToolTip='<%# Eval("Description") %>'></asp:Label>
			    </EditItemTemplate>
			    <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="120px" />
		    </telerik:TreeListTemplateColumn>
		    <telerik:TreeListTemplateColumn DataField="HasPermission" HeaderText="Has Permission?"
			    UniqueName="HasPermission">
			    <ItemTemplate>
				    <asp:CheckBox ID="cboHasPermission" runat="server" Enabled="false" Checked='<%# Eval("Member") %>'>
				    </asp:CheckBox>
			    </ItemTemplate>
			    <EditItemTemplate>
				    <asp:CheckBox ID="cboHasPermission" runat="server" EnableViewState="true" Checked='<%# Bind("Member") %>'>
				    </asp:CheckBox>
			    </EditItemTemplate>
			    <ItemStyle VerticalAlign="Top" Width="60px" />
		    </telerik:TreeListTemplateColumn>

            <telerik:TreeListEditCommandColumn ShowAddButton="False"/>
	    </Columns>       
</telerik:RadTreeList>
<!-- END: Permissions in Roles -->



<!-- Permissions in Pricing Modules -->
<telerik:RadTreeList ID="rtlPricingModulePermissionsForEditing" runat="server" OnNeedDataSource="rtlPricingModulePermissionsForEditing_NeedDataSource"
     ParentDataKeyNames="ParentID" DataKeyNames="ID" AutoGenerateColumns="False" Skin="Simple" EditMode="InPlace" OnUpdateCommand="rtlPricingModulePermissionsForEditing_UpdateCommand">
    <SortExpressions>
                              <telerik:TreeListSortExpression FieldName="Name" SortOrder="Ascending"  runat="server"/>
                          </SortExpressions>
        <Columns>
		    <telerik:TreeListTemplateColumn DataField="PermissionName" SortExpression="PermissionName"
			    HeaderText="Permission Name" UniqueName="PermissionName">
			    <ItemTemplate>
				    <asp:Label ID="lblPermissionName" runat="server" Text='<%# Eval("Name") %>'
					    ToolTip='<%# Eval("Description") %>'></asp:Label>
			    </ItemTemplate>
			    <EditItemTemplate>
				    <asp:Label ID="lblPermissionName" runat="server" Text='<%# Bind("Name") %>'
					    ToolTip='<%# Eval("Description") %>'></asp:Label>
			    </EditItemTemplate>
			    <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="120px" />
		    </telerik:TreeListTemplateColumn>
		    <telerik:TreeListTemplateColumn DataField="HasPermission" HeaderText="Has Permission?"
			    UniqueName="HasPermission">
			    <ItemTemplate>
				    <asp:CheckBox ID="cboHasPermission" runat="server" Enabled="false" Checked='<%# Eval("Member") %>'>
				    </asp:CheckBox>
			    </ItemTemplate>
			    <EditItemTemplate>
				    <asp:CheckBox ID="cboHasPermission" runat="server" EnableViewState="true" Checked='<%# Bind("Member") %>'>
				    </asp:CheckBox>
			    </EditItemTemplate>
			    <ItemStyle VerticalAlign="Top" Width="60px" />
		    </telerik:TreeListTemplateColumn>
		    
            <telerik:TreeListEditCommandColumn ShowAddButton="False"/>
	    </Columns>       
</telerik:RadTreeList>
<!-- END: Permissions in Pricing Modules -->

<!-- Pricing Modules in Schools -->
<telerik:RadGrid ID="rgSchoolPricingModulesForEditing" runat="server" AllowPaging="True"
	Width="50%" AllowSorting="True" AutoGenerateColumns="False" CellSpacing="0" GridLines="None"
	PageSize="40" OnNeedDataSource="rgSchoolPricingModulesForEditing_NeedDataSource" ShowFooter="True"
	AutoGenerateEditColumn="True" OnUpdateCommand="rgSchoolPricingModulesForEditing_UpdateCommand">
	<SortingSettings EnableSkinSortStyles="False" />
	<MasterTableView DataKeyNames="ID" EditMode="InPlace">
	    <SortExpressions>
                              <telerik:GridSortExpression FieldName="Name" SortOrder="Ascending" />
                          </SortExpressions>
		<CommandItemSettings ExportToPdfText="Export to PDF" />
		<RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
		</RowIndicatorColumn>
		<ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
		</ExpandCollapseColumn>
		<Columns>
			<telerik:GridTemplateColumn DataField="Name" SortExpression="Name"
				HeaderText="Pricing Module Name" HeaderTooltip="Click to sort by name" FilterControlAltText="Filter TemplateColumn column"
				UniqueName="PricingModuleName">
				<ItemTemplate>
					<asp:Label ID="lblPricingModuleName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
				</ItemTemplate>
				<EditItemTemplate>
					<asp:Label ID="lblPricingModuleName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
				</EditItemTemplate>
				<ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="250px" />
			</telerik:GridTemplateColumn>
			<telerik:GridTemplateColumn DataField="Member" HeaderText="Has Pricing Module?"
				FilterControlAltText="Filter TemplateColumn2 column" UniqueName="HasPricingModule">
				<ItemTemplate>
					<asp:CheckBox ID="cboHasPricingModule" runat="server" Enabled="false" Checked='<%# Eval("Member") %>'>
					</asp:CheckBox>
				</ItemTemplate>
				<EditItemTemplate>
					<asp:CheckBox ID="cboHasPricingModule" runat="server" EnableViewState="true" Checked='<%# Bind("Member") %>'>
					</asp:CheckBox>
				</EditItemTemplate>
				<ItemStyle VerticalAlign="Top" Width="60px" />
			</telerik:GridTemplateColumn>
		</Columns>
		<EditFormSettings>
			<EditColumn FilterControlAltText="Filter EditCommandColumn column">
			</EditColumn>
		</EditFormSettings>
	</MasterTableView>
	<FilterMenu EnableImageSprites="False">
		<WebServiceSettings>
			<ODataSettings InitialContainerName="">
			</ODataSettings>
		</WebServiceSettings>
	</FilterMenu>
	<HeaderContextMenu CssClass="GridContextMenu GridContextMenu_Default">
		<WebServiceSettings>
			<ODataSettings InitialContainerName="">
			</ODataSettings>
		</WebServiceSettings>
	</HeaderContextMenu>
</telerik:RadGrid>
<!-- END: Pricing Modules in Schools -->

<asp:ObjectDataSource ID="objdsThinkgatePermissionLevelValues" runat="server" SelectMethod="ConvertPermissionLevelsToTable"
	TypeName="Thinkgate.Base.Classes.ThinkgatePermission"></asp:ObjectDataSource>
