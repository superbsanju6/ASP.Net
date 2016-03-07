<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccountMenu.ascx.cs"
	Inherits="Thinkgate.Account.AccountMenu" %>
<div id="dvMenu" runat="server" align="center">
<asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CausesValidation="false"
	onclick="btnChangePassword_Click" />&nbsp;
<asp:Button ID="btnResetPassword" runat="server" Text="Reset Password"  CausesValidation="false"
	onclick="btnResetPassword_Click" />&nbsp;
<asp:Button ID="btnRegistration" runat="server" Text="Create User Account"  CausesValidation="false"
	onclick="btnRegistration_Click" />&nbsp;
<asp:Button ID="btnRoleManagement" runat="server" Text="Manage Roles"  CausesValidation="false"
	onclick="btnRoleManagement_Click" />&nbsp;
<asp:Button ID="btnUserManagemnt" runat="server" Text="Manage Users"  CausesValidation="false"
	onclick="btnUserManagemnt_Click" />&nbsp;
<asp:Button ID="btnHome" runat="server" Text="Home" onclick="btnHome_Click"  CausesValidation="false"/>
</div>