<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Administration.aspx.cs"
    Inherits="Thinkgate.Administration" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <%@ register src="~/Account/Registration.ascx" tagname="Registration" tagprefix="reg" %>
    <title></title>
</head>
<body>
    <script type="text/javascript">
        function OpenEditRolePermissionsWindow() {
            var combo = $find("<%= rcbRoles.ClientID %>");
            var val = combo.get_value();
            if (val == '-1') return;

            var combo1 = $find("<%= rcbPricingModules.ClientID %>");
            var itm1 = combo1.findItemByValue("-1");
            itm1.select();

            var combo2 = $find("<%= rcbSchools.ClientID %>");
            var itm2 = combo2.findItemByValue("-1");
            itm2.select();

            var wnd = $find("EditWindow");
            var url = "../ControlHost/TGSecurityXref.aspx?Method=LoadRolePermissions&ID=" + val;
            wnd.SetUrl(url);
            wnd.show();
        }
        function OpenEditPricingModulePermissionsWindow() {
            var combo = $find("<%= rcbPricingModules.ClientID %>");
            var val = combo.get_value();
            if (val == '-1') return;

            var combo1 = $find("<%= rcbRoles.ClientID %>");
            var itm1 = combo1.findItemByValue("-1");
            itm1.select();

            var combo2 = $find("<%= rcbSchools.ClientID %>");
            var itm2 = combo2.findItemByValue("-1");
            itm2.select();

            var wnd = $find("EditWindow");
            var url = "../ControlHost/TGSecurityXref.aspx?Method=LoadPricingModulePermissions&ID=" + val;
            wnd.SetUrl(url);
            wnd.show();
        }
        function OpenEditSchoolsPricingModuleWindow() {
            var combo = $find("<%= rcbSchools.ClientID %>");
            var val = combo.get_value();
            if (val == '-1') return;

            var combo1 = $find("<%= rcbPricingModules.ClientID %>");
            var itm1 = combo1.findItemByValue("-1");
            itm1.select();

            var combo2 = $find("<%= rcbRoles.ClientID %>");
            var itm2 = combo2.findItemByValue("-1");
            itm2.select();

            var wnd = $find("EditWindow");
            var url = "../ControlHost/TGSecurityXref.aspx?Method=LoadSchoolPricingModules&ID=" + val;
            wnd.SetUrl(url);
            wnd.show();
        }
        function CloseEditRolePermissionsWindow() {
            var wnd = $find("EditWindow");
            wnd.close();
        }
        function CloseEditPricingModulePermissionsWindow() {
            var wnd = $find("EditWindow");
            wnd.close();
        }
        function CloseEditPricingModulePermissionsWindow() {
            var wnd = $find("EditWindow");
            wnd.close();
        }
        function requestNewPassword() {
            if (document.getElementById("cboResetPassword").checked)
            {
                 document.getElementById("newPassword").value = prompt("Enter new password","");
            }
        }
    </script>
    <form id="form1" runat="server">
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgUserRolesForEditing" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:Label ID="lblResultMessage" runat="server"></asp:Label>
    <asp:TabContainer ID="tcAdministration" runat="server" ActiveTabIndex="0" TabStripPlacement="Top">
        <asp:TabPanel ID="tbpUsers" runat="server" HeaderText="Users">
            <ContentTemplate>
                <table>
                    <tr valign="top">
                        <td style="padding-right: 10px">
                            <h3>
                                Manage A User</h3>
                            <p>
                                <table>
                                    <tr>
                                        <td align="right">
                                            Select a User Account &nbsp;&nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="Username" runat="server"></asp:TextBox>
                                        </td>
                                        <td style="padding-left: 10px">
                                            <asp:Button ID="btnSelectUser" OnClick="btnSelectUser_Click" runat="server" Text="Search" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Email Address &nbsp;&nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtEmailAddress" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Reset Password? &nbsp;&nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="cboResetPassword" runat="server" ClientIDMode="Static" />
                                            <asp:HiddenField runat="server" ID="newPassword" ClientIDMode="Static"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Is Approved? &nbsp;&nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="cboIsApproved" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Is Locked Out? &nbsp;&nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="cboLockedOut" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                        </td>
                                        <td align="left">
                                            <asp:Button ID="btnSubmitAccountChanges" runat="server" Text="Submit Changes" OnClick="btnSubmitAccountChanges_Click"
                                                Width="120px" CausesValidation="False" OnClientClick="requestNewPassword()" />
                                        </td>
                                    </tr>
                                </table>
                            </p>
                        </td>
                        <td style="padding-left: 10px; padding-right: 10px">
                            Additional Info:<br />
                            <asp:TextBox ID="txtUserInfo" runat="server" Height="100px" Width="400px" ReadOnly="True"
                                TextMode="MultiLine"></asp:TextBox>
                        </td>
                        <td style="border-left: 1,solid,black; border-left-style: solid; border-left-width: thin;
                            border-left-color: #000000; padding-left: 10px">
                            <h3>
                                Create a User Account:</h3>
                            <reg:Registration ID="reg" runat="server"></reg:Registration>
                        </td>
                    </tr>
                </table>
                <asp:TabContainer ID="tbcUserPermissionEditing" runat="server" ActiveTabIndex="0"
                    CssClass="">
                    <asp:TabPanel ID="tbpUserRoles" runat="server" HeaderText="User Roles">
                        <ContentTemplate>
                            <telerik:RadAjaxPanel ID="RadAjaxPanel3" runat="server">
                                <telerik:RadGrid ID="rgUserRolesForEditing" runat="server" AllowPaging="True" AllowSorting="True"
                                    ClientIDMode="Static" Width="25%" AutoGenerateColumns="False" CellSpacing="0"
                                    GridLines="None" ShowFooter="True" PageSize="20" OnNeedDataSource="rgUserRolesForEditing_NeedDataSource"
                                    AutoGenerateEditColumn="True" OnUpdateCommand="rgUserRolesForEditing_UpdateCommand">
                                    <SortingSettings EnableSkinSortStyles="False" />
                                    <MasterTableView DataKeyNames="RoleID">
                                        <CommandItemSettings ExportToPdfText="Export to PDF" />
                                        <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                                        </RowIndicatorColumn>
                                        <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                                        </ExpandCollapseColumn>
                                        <Columns>
                                            <telerik:GridTemplateColumn DataField="RoleName" SortExpression="RoleName" HeaderText="Role Name"
                                                HeaderTooltip="Click to sort by name" FilterControlAltText="Filter TemplateColumn column"
                                                UniqueName="TemplateColumn">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRoleName" runat="server" Text='<%# Eval("RoleName") %>' ToolTip='<%# Eval("Description") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Label ID="lblRoleName" runat="server" Text='<%# Bind("RoleName") %>' ToolTip='<%# Eval("Description") %>'></asp:Label>
                                                </EditItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="120px" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn DataField="HasRole" HeaderText="Has Role?" FilterControlAltText="Filter TemplateColumn3 column"
                                                UniqueName="TemplateColumn3">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="cboHasRole" runat="server" Enabled="false" Checked='<%# Eval("HasRole") %>'>
                                                    </asp:CheckBox>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:CheckBox ID="cboHasRole" runat="server" Checked='<%# Bind("HasRole") %>'></asp:CheckBox>
                                                </EditItemTemplate>
                                                <ItemStyle VerticalAlign="Top" Width="50px" />
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
                            </telerik:RadAjaxPanel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="tbpUserSchools" runat="server" HeaderText="User Schools">
                        <ContentTemplate>
                            <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
                                <telerik:RadGrid ID="rgUserSchoolsForEditing" runat="server" AllowPaging="True" AllowSorting="True"
                                    Width="50%" AutoGenerateColumns="False" CellSpacing="0" GridLines="None" ShowFooter="True"
                                    PageSize="20" OnNeedDataSource="rgUserSchoolsForEditing_NeedDataSource" AutoGenerateEditColumn="True"
                                    OnUpdateCommand="rgUserSchoolsForEditing_UpdateCommand">
                                    <SortingSettings EnableSkinSortStyles="False" />
                                    <MasterTableView DataKeyNames="ID">
                                        <CommandItemSettings ExportToPdfText="Export to PDF" />
                                        <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                                        </RowIndicatorColumn>
                                        <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                                        </ExpandCollapseColumn>
                                        <Columns>
                                            <telerik:GridTemplateColumn DataField="Page" SortExpression="Page" HeaderText="Page"
                                                HeaderTooltip="Click to sort by page" FilterControlAltText="Filter TemplateColumn column"
                                                UniqueName="TemplateColumn">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPage" runat="server" Text='<%# Eval("Page") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Label ID="lblPage" runat="server" Text='<%# Bind("Page") %>'></asp:Label>
                                                </EditItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="50px" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn DataField="Name" SortExpression="Name" HeaderText="School Name"
                                                HeaderTooltip="Click to sort by name" FilterControlAltText="Filter TemplateColumn1 column"
                                                UniqueName="TemplateColumn1">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSchoolName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Label ID="lblSchoolName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                                </EditItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="220px" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn DataField="SchoolType" SortExpression="SchoolType" HeaderText="School Type"
                                                HeaderTooltip="Click to sort by type" FilterControlAltText="Filter TemplateColumn6 column"
                                                UniqueName="TemplateColumn6">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSchoolType" runat="server" Text='<%# Eval("SchoolType") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Label ID="lblSchoolType" runat="server" Text='<%# Eval("SchoolType") %>'></asp:Label>
                                                </EditItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn DataField="SchoolID" SortExpression="SchoolID" HeaderText="School ID"
                                                HeaderTooltip="Click to sort by school ID" FilterControlAltText="Filter TemplateColumn7 column"
                                                UniqueName="TemplateColumn7">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSchoolID" runat="server" Text='<%# Eval("SchoolID") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Label ID="lblSchoolID" runat="server" Text='<%# Eval("SchoolID") %>'></asp:Label>
                                                </EditItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn DataField="HasSchool" HeaderText="Has School?" FilterControlAltText="Filter TemplateColumn9 column"
                                                UniqueName="TemplateColumn9">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="cboHasSchool" runat="server" Enabled="false" Checked='<%# Eval("HasSchool") %>'>
                                                    </asp:CheckBox>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:CheckBox ID="cboHasSchool" runat="server" Checked='<%# Bind("HasSchool") %>'>
                                                    </asp:CheckBox>
                                                </EditItemTemplate>
                                                <ItemStyle VerticalAlign="Top" Width="80px" />
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
                            </telerik:RadAjaxPanel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="tbpUserPermissions" runat="server" HeaderText="User Permissions (Overrides)">
                        <ContentTemplate>
                            <telerik:RadAjaxPanel ID="RadAjaxPanel2" runat="server" >
                                <telerik:RadTreeList ID="rtlUserPermissionsForEditing" runat="server" OnNeedDataSource="rtlUserPermissionsForEditing_NeedDataSource"
                                    ParentDataKeyNames="ParentPermissionId" DataKeyNames="PermissionId" AutoGenerateColumns="False"
                                    Skin="Simple" EditMode="InPlace" OnUpdateCommand="rtlUserPermissionsForEditing_UpdateCommand">
                                    <SortExpressions>
                              <telerik:TreeListSortExpression FieldName="PermissionName" SortOrder="Ascending"  runat="server"/>
                          </SortExpressions>
                                    <Columns>
                                        <telerik:TreeListTemplateColumn DataField="PermissionName" SortExpression="PermissionName"
                                            HeaderText="Permission Name" UniqueName="PermissionName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPermissionName" runat="server" Text='<%# Eval("PermissionName") %>'
                                                    ToolTip='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblPermissionName" runat="server" Text='<%# Bind("PermissionName") %>'
                                                    ToolTip='<%# Eval("Description") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="120px" />
                                        </telerik:TreeListTemplateColumn>
                                        <telerik:TreeListTemplateColumn DataField="HasPermission" HeaderText="Has Permission?"
                                            UniqueName="HasPermission">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cboHasPermission" runat="server" Enabled="false" Checked='<%# Eval("HasPermission") %>'>
                                                </asp:CheckBox>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="cboHasPermission" runat="server" EnableViewState="true" Checked='<%# Bind("HasPermission") %>'>
                                                </asp:CheckBox>
                                            </EditItemTemplate>
                                            <ItemStyle VerticalAlign="Top" Width="60px" />
                                        </telerik:TreeListTemplateColumn>
                                        <telerik:TreeListEditCommandColumn ShowAddButton="False" />
                                    </Columns>
                                </telerik:RadTreeList>
                            </telerik:RadAjaxPanel>
                        </ContentTemplate>
                    </asp:TabPanel>
                </asp:TabContainer>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="tbpRoles" runat="server" HeaderText="Roles">
            <ContentTemplate>
                <div align="center">
                    <h1>
                        <b>Manage Roles</b></h1>
                    <telerik:RadGrid ID="rgRoles" runat="server" AllowPaging="True" AllowSorting="True"
                        PageSize="20" AutoGenerateColumns="False" CellSpacing="0" GridLines="None" ShowFooter="True"
                        Width="50%" AutoGenerateEditColumn="true" ShowGroupPanel="False" AllowFilteringByColumn="False"
                        OnNeedDataSource="rgRoles_NeedDataSource" OnUpdateCommand="rgRoles_UpdateCommand">
                        <SortingSettings EnableSkinSortStyles="false" />
                        <MasterTableView DataKeyNames="RoleID" TableLayout="Auto">
                            <Columns>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-Width="120px"
                                    ItemStyle-HorizontalAlign="Left" DataField="PermissionName" SortExpression="RoleName"
                                    HeaderText="Role Name" HeaderTooltip="Click to sort by name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRoleName" runat="server" Text='<%# Eval("RoleName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtRoleName" runat="server" Text='<%# Bind("RoleName") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" DataField="Description"
                                    HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtDescription" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridDropDownColumn DataField="RolePortalSelection" 
					                DataSourceID="objdsThinkgateRolePortalSelectionList" ItemStyle-Width="5pc" HeaderText="Portal" ListTextField="PortalName"                        
					                ListValueField="ID" UniqueName="RolePortalSelection"      ItemStyle-VerticalAlign="Top"                  
					             />   
                                 <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" DataField="Active" HeaderText="Active?">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cboActive" runat="server" Enabled="false" Checked='<%# Eval("Active") %>'>
                                        </asp:CheckBox>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="cboActive" runat="server" Checked='<%# Bind("Active") %>'></asp:CheckBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="tbpSchools" runat="server" HeaderText="Schools">
            <ContentTemplate>
                <div align="left">
                    <h1>
                        <b>Manage Schools</b></h1>
                    <telerik:RadGrid ID="rgSchools" runat="server" AllowPaging="True" AllowSorting="True"
                        AutoGenerateColumns="False" CellSpacing="0" GridLines="None" ShowFooter="True"
                        PageSize="25" AutoGenerateEditColumn="true" ShowGroupPanel="False" AllowFilteringByColumn="False"
                        OnNeedDataSource="rgSchools_NeedDataSource" OnUpdateCommand="rgSchools_UpdateCommand">
                        <SortingSettings EnableSkinSortStyles="false" />
                        <MasterTableView DataKeyNames="ID" TableLayout="Auto">
                            <Columns>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-Width="50px"
                                    ItemStyle-HorizontalAlign="Left" DataField="Page" SortExpression="Page" HeaderText="Page"
                                    HeaderTooltip="Click to sort by page">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPage" runat="server" Text='<%# Eval("Page") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:Label ID="lblPage" runat="server" Text='<%# Bind("Page") %>'></asp:Label>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-Width="220px"
                                    ItemStyle-HorizontalAlign="Left" DataField="Name" SortExpression="Name" HeaderText="School Name"
                                    HeaderTooltip="Click to sort by name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSchoolName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtSchoolName" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left"
                                    DataField="District" SortExpression="District" HeaderText="District" HeaderTooltip="Click to sort by district">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDistrict" runat="server" Text='<%# Eval("District") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtDistrict" runat="server" Text='<%# Bind("District") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left"
                                    DataField="Abbreviation" SortExpression="Abbreviation" HeaderText="Abbreviation"
                                    HeaderTooltip="Click to sort by abbreviation">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAbbreviation" runat="server" Text='<%# Eval("Abbreviation") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtAbbreviation" runat="server" Text='<%# Bind("Abbreviation") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left"
                                    DataField="Phone" SortExpression="Phone" HeaderText="Phone" HeaderTooltip="Click to sort by phone">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPhone" runat="server" Text='<%# Eval("Phone") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtPhone" runat="server" Text='<%# Bind("Phone") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left"
                                    DataField="Cluster" SortExpression="Cluster" HeaderText="Cluster" HeaderTooltip="Click to sort by cluster">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCluster" runat="server" Text='<%# Eval("Cluster") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtCluster" runat="server" Text='<%# Bind("Cluster") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left"
                                    DataField="SchoolType" SortExpression="SchoolType" HeaderText="School Type" HeaderTooltip="Click to sort by type">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSchoolType" runat="server" Text='<%# Eval("SchoolType") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadComboBox ID="RadComboBox1" runat="server" DataSourceID="objdsThinkgateSchoolTypes"
                                            DataTextField="SchoolType" DataValueField="SchoolType" SelectedValue='<%# Bind("SchoolType") %>'>
                                        </telerik:RadComboBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left"
                                    DataField="SchoolID" SortExpression="SchoolID" HeaderText="School ID" HeaderTooltip="Click to sort by school ID">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSchoolID" runat="server" Text='<%# Eval("SchoolID") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtSchoolID" runat="server" Text='<%# Bind("SchoolID") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left"
                                    DataField="PortalFlag" SortExpression="PortalFlag" HeaderText="PortalFlag" HeaderTooltip="Click to sort by portal flag">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPortalFlag" runat="server" Text='<%# Eval("PortalFlag") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtPortalFlag" runat="server" Text='<%# Bind("PortalFlag") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="tbpPricingModules" runat="server" HeaderText="PricingModules">
            <ContentTemplate>
                <div align="center">
                    <h1>
                        <b>Manage Pricing Modules</b></h1>
                    <telerik:RadGrid ID="rgPricingModules" runat="server" AllowPaging="True" AllowSorting="True"
                        Width="25%" AutoGenerateColumns="False" CellSpacing="0" GridLines="None" ShowFooter="True"
                        PageSize="20" AutoGenerateEditColumn="true" ShowGroupPanel="False" AllowFilteringByColumn="False"
                        OnNeedDataSource="rgPricingModules_NeedDataSource" OnUpdateCommand="rgPricingModules_UpdateCommand">
                        <SortingSettings EnableSkinSortStyles="false" />
                        <MasterTableView DataKeyNames="ID" TableLayout="Auto">
                            <Columns>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-Width="120px"
                                    ItemStyle-HorizontalAlign="Left" DataField="PermissionName" SortExpression="PricingModuleName"
                                    HeaderText="PricingModule Name" HeaderTooltip="Click to sort by name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPricingModuleName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:Label ID="lblPricingModuleName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" DataField="Active" HeaderText="Active?">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cboActive" runat="server" Enabled="false" Checked='<%# Eval("Active") %>'>
                                        </asp:CheckBox>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="cboActive" runat="server" Checked='<%# Bind("Active") %>'></asp:CheckBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="tbpPermissions" runat="server" HeaderText="Permissions">
            <ContentTemplate>
                <div align="center">
                    <h1>
                        <b>Manage Permissions</b></h1>
                    <telerik:RadGrid ID="rgPermissions" runat="server" AllowPaging="True" AllowSorting="True"
                        Width="50%" AutoGenerateColumns="False" CellSpacing="0" GridLines="None" ShowFooter="True"
                        AutoGenerateEditColumn="true" ShowGroupPanel="False" AllowFilteringByColumn="False"
                        OnNeedDataSource="rgPermissions_NeedDataSource" OnUpdateCommand="rgPermissions_UpdateCommand">
                        <SortingSettings EnableSkinSortStyles="false" />
                        <MasterTableView DataKeyNames="PermissionID" TableLayout="Auto">
                            <SortExpressions>
                              <telerik:GridSortExpression FieldName="PermissionName" SortOrder="Ascending" />
                          </SortExpressions>
                            <Columns>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" ItemStyle-Width="120px"
                                    ItemStyle-HorizontalAlign="Left" DataField="PermissionName" SortExpression="PermissionName"
                                    HeaderText="Permission Name" HeaderTooltip="Click to sort by name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPermissionName" runat="server" Text='<%# Eval("PermissionName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtPermissionName" runat="server" Text='<%# Bind("PermissionName") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn ItemStyle-VerticalAlign="Top" DataField="Description"
                                    HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtDescription" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="TabPanel1" runat="server" HeaderText="Cross Refs">
            <ContentTemplate>
                <div align="center">
                    <h1>
                        <b>Manage Cross References</b></h1>
                    <table>
                        <tr style="padding-top: 10px">
                            <td>
                                Permissions in Roles:
                            </td>
                            <td>
                                Pricing Modules in Schools:
                            </td>
                            <td>
                                Permissions in Pricing Modules:
                            </td>
                        </tr>
                        <tr style="padding-top: 10px">
                            <td style="padding-left: 5px; padding-right: 5px">
                                <telerik:RadComboBox ID="rcbRoles" runat="server" ClientIDMode="Static" AppendDataBoundItems="True"
                                    DataTextField="Name" DataValueField="ID" DataSourceID="objdsThinkgateRoleList"
                                    OnClientSelectedIndexChanged="OpenEditRolePermissionsWindow">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="[Select]" Value="-1" Selected="true" />
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                            <td style="padding-left: 5px; padding-right: 5px">
                                <telerik:RadComboBox ID="rcbSchools" runat="server" AppendDataBoundItems="true" Width="250px"
                                    DataTextField="Name" DataValueField="ID" DataSourceID="objdsThinkgateSchoolList"
                                    OnClientSelectedIndexChanged="OpenEditSchoolsPricingModuleWindow">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="[Select]" Value="-1" Selected="true" />
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                            <td style="padding-left: 5px; padding-right: 5px">
                                <telerik:RadComboBox ID="rcbPricingModules" runat="server" AppendDataBoundItems="true"
                                    DataTextField="Name" DataValueField="ID" DataSourceID="objdsThinkgatePricingModuleList"
                                    OnClientSelectedIndexChanged="OpenEditPricingModulePermissionsWindow">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="[Select]" Value="-1" Selected="true" />
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
    </asp:TabContainer>
    <telerik:RadWindow runat="server" Width="800px" Height="545px" VisibleStatusbar="false"
        ShowContentDuringLoad="false" ID="EditWindow" ClientIDMode="Static" Modal="true"
        Skin="Simple" Behaviors="Close, Move">
    </telerik:RadWindow>
    </form>
</body>
</html>
<asp:objectdatasource id="objdsThinkgateSchoolTypes" runat="server" selectmethod="GetSchoolTypes"
    typename="Thinkgate.Base.Classes.ThinkgateSchool">
</asp:objectdatasource>
<asp:objectdatasource id="objdsThinkgatePermissionLevelValues" runat="server" selectmethod="ConvertPermissionLevelsToTable"
    typename="Thinkgate.Base.Classes.ThinkgatePermission">
</asp:objectdatasource>
<asp:objectdatasource id="objdsThinkgateRoleList" runat="server" selectmethod="GetAllRoleIdsAndNames"
    typename="Thinkgate.Base.Classes.ThinkgateRole">
</asp:objectdatasource>
<asp:objectdatasource id="objdsThinkgateSchoolList" runat="server" selectmethod="GetAllSchoolIdsAndNames"
    typename="Thinkgate.Base.Classes.ThinkgateSchool">
</asp:objectdatasource>
<asp:objectdatasource id="objdsThinkgatePricingModuleList" runat="server" selectmethod="GetAllPricingModuleIdsAndNames"
    typename="Thinkgate.Base.Classes.ThinkgatePricingModule">
</asp:objectdatasource>
<asp:objectdatasource id="objdsThinkgateRolePortalSelectionList" runat="server" selectmethod="GetAllRolePortalSelectionIdsAndNames"
    typename="Thinkgate.Base.Classes.ThinkgateRole">
</asp:objectdatasource>
<p>
    &nbsp;</p>
