 <%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeacherIdentification.ascx.cs"
		Inherits="Thinkgate.Controls.Teacher.TeacherIdentification" %>

<style type="text/css">
	.bottomTextButton
	{
		position: relative;
		float: right;
		top: 0px;  
		left: 0px;
		height: 16px;
		cursor: pointer;
		background-repeat: no-repeat;
		background-position: left center;
		text-align: right;
		padding-left: 20px;
		padding-right: 10px;
	}
</style>

<div style="height:90%">
	<table width="100%" class="fieldValueTable">
		<tr>
				<td class="fieldLabel">
						Name:
				</td>
				<td>
						<asp:Label runat="server" ID="lblName" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						User ID:
				</td>
				<td>
						<asp:Label runat="server" ID="lblUserID" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						School:
				</td>
				<td>
						<asp:Label runat="server" ID="lblSchool" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						User Type:
				</td>
				<td>
						<asp:Label runat="server" ID="lblUserType" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						Email:
				</td>
				<td>
						<a href="#" id="anchorEmail" runat="server"><asp:Label runat="server" ID="lblEmail" /></a>
				</td>
		</tr>
	</table>

<div style="position: absolute; bottom: 0px; right: 0px;">
	<div runat="server" id="tileTeacherIdentification_btnResetPassword" clientidmode="Static" class="bottomTextButton" style="background-image: url(../Images/Gears.png);" title="Reset Password" >Reset Password</div>
	<div runat="server" id="tileTeacherIdentification_btnUploadPicture" clientidmode="Static" class="bottomTextButton" style="background-image: url(../Images/Upload.png);" title="Upload Picture">Upload Picture</div>
</div>
</div>