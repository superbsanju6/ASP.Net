<%@ Page Title="" Language="C#" MasterPageFile="~/Security.Master" AutoEventWireup="true"
	CodeBehind="UserManagement.aspx.cs" Inherits="Thinkgate.Account.UserManagement" %>

<%@ Register Src="~/Account/Registration.ascx" TagName="Registration" TagPrefix="reg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<h3>
		Create a User Account:</h3>
	<br />
	<reg:Registration ID="reg" runat="server">
	</reg:Registration>
	<h3>
		Manage A User</h3>
	<asp:Label ID="lblResultMessage" runat="server"></asp:Label>
	<p>
		<table>
			<tr>
				<td align="right">
					Select a User Account &nbsp;&nbsp;
				</td>
				<td align="left">
					<asp:TextBox ID="Username" runat="server"></asp:TextBox>
				</td>
				<td>
					<asp:Button ID="btnSelectUser" OnClick="btnSelectUser_Click" CausesValidation="false"
						runat="server" Text="Search" />
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
					<asp:CheckBox ID="cboResetPassword" runat="server" Checked="false" />
				</td>
			</tr>
			<tr>
				<td align="right">
					Is Approved? &nbsp;&nbsp;
				</td>
				<td align="left">
					<asp:CheckBox ID="cboIsApproved" runat="server" Checked="false" />
				</td>
			</tr>
			<tr>
				<td align="right">
					Is Locked Out? &nbsp;&nbsp;
				</td>
				<td align="left">
					<asp:CheckBox ID="cboLockedOut" runat="server" Checked="false" />
				</td>
			</tr>
			<tr>
				<td align="right">
				</td>
				<td align="left">
					<asp:Button ID="btnSubmitAccountChanges" runat="server" Text="Submit Changes" OnClick="btnSubmitAccountChanges_Click"
						CausesValidation="false" />
				</td>
			</tr>
			<tr>
				<td colspan="3">
					<br />Additional Info:<br />
					<asp:TextBox ID="txtUserInfo" runat="server" Height="100" Width="400" ReadOnly="true"
						TextMode="MultiLine"></asp:TextBox>
				</td>
			</tr>
		</table>
	</p>
	<br />
	<br />
	<div id="dvUserRoles" runat="server" visible="false">
		<h2>
			Manage User Roles</h2>
		<br />
		<p align="center">
			<asp:Label ID="ActionStatus" runat="server" CssClass="Important"></asp:Label>
		</p>
		<p>
			<asp:Repeater ID="UsersRoleList" runat="server">
				<ItemTemplate>
					<asp:CheckBox runat="server" ID="RoleCheckBox" AutoPostBack="true" Text='<%# Container.DataItem %>'
						OnCheckedChanged="RoleCheckBox_CheckChanged" />
					<br />
				</ItemTemplate>
			</asp:Repeater>
			<br />
			<asp:Button ID="btnSubmitUserRoleChanges" runat="server" OnClick="btnSubmitUserRoleChanges_Click"
				Text="Submit Role Changes" />
		</p>
	</div>
</asp:Content>
