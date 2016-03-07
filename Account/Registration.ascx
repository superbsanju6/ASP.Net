<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Registration.ascx.cs"
	Inherits="Thinkgate.Account.RegistrationAscx" %>
<p>
	<asp:Label ID="CreateAccountResults" runat="server"></asp:Label>
</p>
<p>
	<table>
		<tr >
			<td align="right" style="padding-bottom: 5px">
				Enter a username: &nbsp;&nbsp;
			</td>
			<td align="left">
				<asp:TextBox ID="Username" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td align="right" style="padding-bottom: 5px">
				Choose a password: &nbsp;&nbsp;
			</td>
			<td align="left">
				<asp:TextBox ID="Password" TextMode="Password" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td align="right"style="padding-bottom: 5px">
				Confirm password: &nbsp;&nbsp;
			</td>
			<td align="left">
				<asp:TextBox ID="ConfirmPassword" TextMode="Password" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td align="right"style="padding-bottom: 5px">
				Enter your email address: &nbsp;&nbsp;
			</td>
			<td align="left">
				<asp:TextBox ID="Email" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td align="right"style="padding-bottom: 5px">
			</td>
			<td align="left">
				<asp:Button ID="CreateAccountButton" runat="server" Text="Create Account" OnClick="CreateAccountButton_Click" Width="120px" ValidationGroup="CreateAccountValidation" />
			</td>
		</tr>
	</table>
	<br />
	<br />
	<asp:RequiredFieldValidator runat="server" ID="rfvUsername" ControlToValidate="Username" ValidationGroup="CreateAccountValidation"
		ErrorMessage="Please enter a user name"></asp:RequiredFieldValidator>
	<br />
	<asp:RequiredFieldValidator runat="server" ID="rfvPassword" ControlToValidate="Password" ValidationGroup="CreateAccountValidation"
		ErrorMessage="Please enter a password"></asp:RequiredFieldValidator>
	<br />
	<asp:RequiredFieldValidator runat="server" ID="rfvPasswordConfirm" ControlToValidate="ConfirmPassword" ValidationGroup="CreateAccountValidation"
		ErrorMessage="Please confirm your password"></asp:RequiredFieldValidator>
	<br />
	<asp:RequiredFieldValidator runat="server" ID="rfvEmail" ControlToValidate="Email" ValidationGroup="CreateAccountValidation"
		ErrorMessage="Please enter an email address"></asp:RequiredFieldValidator>
	<br />
	<asp:CompareValidator ID="cvPasswordConfirm" runat="server" ErrorMessage="Passwords do not match" ValidationGroup="CreateAccountValidation"
		ControlToCompare="Password" ControlToValidate="ConfirmPassword"></asp:CompareValidator>
	<br />
</p>
