<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentConfiguration.ascx.cs"
	Inherits="Thinkgate.Controls.Assessment.AssessmentConfiguration" %>

<style type="text/css">
	.limitedWidthFieldLabel
	{
		width: 55%;
	}
</style>

<telerik:RadAjaxPanel ID="assessmentConfigurationAjaxPanel" runat="server" LoadingPanelID="assessmentConfigurationLoadingPanel" style="margin-top: 0px; overflow-y:scroll;height:90% !important;">
	<table width="100%" class="fieldValueTable">
		<tr>
				<td class="fieldLabel limitedWidthFieldLabel">
						Content Type:
				</td>
				<td>
						<asp:Label runat="server" ID="lblContentType" />
				</td>
		</tr>
		<tr id="trNumForms" runat="server">
				<td class="fieldLabel limitedWidthFieldLabel">
						Number of Forms:
				</td>
				<td>
						<asp:Label runat="server" ID="lblNumForms" />
				</td>
		</tr>
		<tr id="trOnlineContent" runat="server">
				<td class="fieldLabel limitedWidthFieldLabel">
						Online Content Format:
				</td>
				<td>
						<asp:Label runat="server" ID="lblOnlineContent" />
				</td>
		</tr>
		<tr id="trInclFieldTest" runat="server">
				<td class="fieldLabel limitedWidthFieldLabel">
						Include Field Test:
				</td>
				<td>
						<asp:Label runat="server" ID="lblIncFieldTest" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel limitedWidthFieldLabel">
						Number of Distractors:
				</td>
				<td>
						<asp:Label runat="server" ID="lblNumDist" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel limitedWidthFieldLabel">
						Distractor Labels:
				</td>
				<td>
						<asp:Label runat="server" ID="lblDistLabels" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel limitedWidthFieldLabel">
						Score Type:
				</td>
				<td>
						<asp:Label runat="server" ID="lblScoreType" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel limitedWidthFieldLabel">
						Performance Levels:
				</td>
				<td>
						<asp:Label runat="server" ID="lblPerfLevels" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel limitedWidthFieldLabel">
						Print Columns:
				</td>
				<td>
						<asp:Label runat="server" ID="lblPrintCols" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel limitedWidthFieldLabel">
						Print Short Answer<br />Section on Bubble Sheet:
				</td>
				<td>
						<asp:Label runat="server" ID="lblPrintSA" />
				</td>
		</tr>
		<tr id="trSource" runat="server">
				<td class="fieldLabel limitedWidthFieldLabel">
						Source:
				</td>
				<td>
						<asp:Label runat="server" ID="lblSource" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel limitedWidthFieldLabel">
						Author:
				</td>
				<td>
						<asp:Label runat="server" ID="lblAuthor" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel limitedWidthFieldLabel">
						Last Edited:
				</td>
				<td>
						<asp:Label runat="server" ID="lblLastEdit" />
				</td>
		</tr>
	</table>
</telerik:RadAjaxPanel>

<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="assessmentConfigurationLoadingPanel"	runat="server" />
