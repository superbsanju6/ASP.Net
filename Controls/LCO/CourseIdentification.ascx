<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourseIdentification.ascx.cs" Inherits="Thinkgate.Controls.LCO.CourseIdentification" %>
<style type="text/css">
</style>

<telerik:RadAjaxPanel ID="LCOCourseIdentificationAjaxPanel" runat="server" LoadingPanelID="LCOCourseIdentificationLoadingPanel" style="margin-top: 15px;">
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
						Program Area:
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
						Course Number:
				</td>
				<td>
						<asp:Label runat="server" ID="lblCourseNumber" />
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
						LEA:
				</td>
				<td>
						<asp:Label runat="server" ID="lblLEA" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						Date Created:
				</td>
				<td>
						<asp:Label runat="server" ID="lblDateCreated" />
				</td>
		</tr>
        		<tr>
				<td class="fieldLabel">
						Date Approved:
				</td>
				<td>
						<asp:Label runat="server" ID="lblDateApproved" />
				</td>
		<%--</tr>
        		<tr>
				<td class="fieldLabel">
						Implementation Date:
				</td>
				<td>
						<asp:Label runat="server" ID="lblImplementationDate" />
				</td>
		</tr>--%>
	</table>
    <span style="display:none;"><telerik:RadButton runat="server" ID="LCOCourseIdentificationRefreshTrigger" ClientIDMode="Static" Text="Refresh" /></span>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="LCOCourseIdentificationLoadingPanel"	runat="server" />
