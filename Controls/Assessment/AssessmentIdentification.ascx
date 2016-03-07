<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentIdentification.ascx.cs"
	Inherits="Thinkgate.Controls.Assessment.AssessmentIdentification" %>

<style type="text/css">
</style>

<telerik:RadAjaxPanel ID="assessmentIdentificationAjaxPanel" runat="server" LoadingPanelID="assessmentIdentificationLoadingPanel" style="margin-top: 15px;">
	<table width="100%" class="fieldValueTable">
		<tr>
				<td class="fieldLabel">
						Grade:
				</td>
				<td>
						<asp:Label runat="server" ID="lblGrade" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						Subject:
				</td>
				<td>
						<asp:Label runat="server" ID="lblSubject" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						Course:
				</td>
				<td>
						<asp:Label runat="server" ID="lblCourse" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						Type:
				</td>
				<td>
						<asp:Label runat="server" ID="lblType" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						Term:
				</td>
				<td>
						<asp:Label runat="server" ID="lblTerm" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						Description:
				</td>
				<td>
						<asp:Label runat="server" ID="lblDescription" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						Author:
				</td>
				<td>
						<asp:Label runat="server" ID="lblAuthor" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						Date Created:
				</td>
				<td>
						<asp:Label runat="server" ID="lblCreated" />
				</td>
		</tr>
	</table>
    <span style="display:none;"><telerik:RadButton runat="server" ID="assessmentIdentificationRefreshTrigger" ClientIDMode="Static" Text="Refresh" /></span>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="assessmentIdentificationLoadingPanel"	runat="server" />
