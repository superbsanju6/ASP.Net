<%@ Page Title="" Language="C#" MasterPageFile="~/Security.Master" AutoEventWireup="true"
	CodeBehind="RoleManagement.aspx.cs" Inherits="Thinkgate.Account.RoleManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	
	<b>Create a New Role: </b>
	<asp:TextBox ID="RoleName" runat="server"></asp:TextBox>
	<br />
	<asp:Button ID="CreateRoleButton" runat="server" Text="Create Role" OnClick="CreateRoleButton_Click" />
	<br />
	<br />
	<asp:GridView ID="RoleList" runat="server" AutoGenerateColumns="False" 
	onrowdeleting="RoleList_RowDeleting">
		<Columns>
			<asp:CommandField DeleteText="Delete Role" ShowDeleteButton="True" />
			<asp:TemplateField HeaderText="Role">
				<ItemTemplate>
					<asp:Label runat="server" ID="RoleNameLabel" Text='<%# Container.DataItem %>' />
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>
	Add/Remove Users To Role Role Selector Flag Role Active/Inactive User CheckBox List
</asp:Content>
