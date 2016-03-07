<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentConfigurationn_Edit.aspx.cs"
		Inherits="Thinkgate.Controls.Assessment.AssessmentConfiguration_Edit" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
		<meta http-equiv="X-UA-Compatible" content="IE=8" />
		<meta http-equiv="PRAGMA" content="NO-CACHE" />
		<meta http-equiv="Expires" content="-1" />
		<meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
		<link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
		<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
		<title>Edit Assessment Configuration</title>
		<base target="_self" />
		<telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
		</telerik:RadStyleSheetManager>
		<script type='text/javascript' src='../../Scripts/jquery-1.9.1.min.js'></script>
		<script type='text/javascript' src='../../Scripts/jquery-migrate-1.1.0.min.js'></script>
		<script type='text/javascript' src='../../Scripts/jquery.scrollTo.js'></script>
		<script type="text/javascript">        var $j = jQuery.noConflict();</script>
		<script type='text/javascript' src='../../Scripts/master.js'></script>
		<style type="text/css">
			.editorTable
			{
				text-align: left;
				font-size: 12pt;
			}

			.editorLabel
			{
				font-weight: bold;
				padding-top: 25px;
			}

			.editorControl
			{
				padding-left: 40px;
				padding-top: 25px;
			}

			.containerDiv 
			{
				width: 500px;
				height: 400px;
				margin: 10px;
			}
				
			.labels 
			{
				font-size: 12pt;
				width: 100px;
				text-align: left;
				margin-right: 10px;
				position: relative;
				float: left;
			}
				
			.formElement 
			{
				position: relative;
				float: left;
				margin-bottom: 30px;
			}
				
			input.formElement
			{
				width: 200px;
				padding: 3px;
				border: solid 1px #000;
			}
			
			.formContainer 
			{
				width: 340px;
				text-align: center;
				margin-top: 60px;
			}
				
			.headerImg 
			{
				position: absolute;
				left: 0;
				top: 0;
			}
				
			.roundButtons 
			{
				color: #00F;
				font-weight: bold;
				font-size: 12pt;
				padding: 2px;
				display: inline;
				position: relative;
				border: solid 1px #000;
				border-radius: 50px;
				float: right;
				margin-left: 10px;
				margin-top: 30px;
				cursor: pointer;
				background-color: #FFF;
			}
				
			.contextMenuDiv 
			{
				height: 490px; 
				width: 565px; 
				position: absolute; 
				top: 0px; 
				left: 0px;
			}
		</style>
</head>
<body>
		<form id="form2" runat="server" style="height: 100%; background-color: White;">
		<telerik:RadScriptManager ID="RadScriptManager2" runat="server">
				<Scripts>
						<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
						<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
						<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
				</Scripts>
		</telerik:RadScriptManager>
		<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
		</telerik:RadAjaxManager>
		<telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
		</telerik:RadSkinManager>
		<telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel" Width="100%" Height="100%" BackColor="White">
			<!-- What a hack. This seems to be the only way to have a white background -->
			<div id="pageDiv" style="width: 100%; height: 620px; background-color: White;">

				<table class="editorTable" style="margin-left: 260px;">
					<tr>
						<td class="editorLabel">
							Content Type:
						</td>
						<td class="editorControl">
							<telerik:RadComboBox ID="cmbContentType" runat="server" ToolTip="Select a content type" 
								Skin="Web20" Width="200"
							 EmptyMessage="&lt;Select One&gt;" 
								OnSelectedIndexChanged="cmbContentType_SelectedIndexChanged" AutoPostBack="true" 
								CausesValidation="False" DataTextField="ContentType">
							</telerik:RadComboBox>
						</td>
					</tr>
					<tr id="trNumForms" runat="server">
						<td class="editorLabel">
							Number of Forms:
						</td>
						<td class="editorControl">
							<telerik:RadComboBox ID="cmbNumForms" runat="server" ToolTip="Select the number of forms" 
								Skin="Web20" Width="200"
							 EmptyMessage="&lt;Select One&gt;" 
								OnSelectedIndexChanged="cmbNumForms_SelectedIndexChanged" AutoPostBack="true" 
								CausesValidation="False" DataTextField="NumForms">
							</telerik:RadComboBox>
						</td>
					</tr>
					<tr id="trOnlineContent" runat="server">
						<td class="editorLabel">
							Online Content Format:
						</td>
						<td class="editorControl">
							<telerik:RadComboBox ID="cmbOnlineContFmt" runat="server" ToolTip="Select the number of columns to print" 
								Skin="Web20" Width="200"
							 EmptyMessage="&lt;Select One&gt;" 
								OnSelectedIndexChanged="cmbOnlineContFmt_SelectedIndexChanged" AutoPostBack="true" 
								CausesValidation="False" DataTextField="DisplayName" DataValueField="OnlineContentFormat">
							</telerik:RadComboBox>
						</td>
					</tr>
					<tr id="trInclFieldTest" runat="server">
						<td class="editorLabel">
							Include Field Test:
						</td>
						<td class="editorControl">
							<telerik:RadComboBox ID="cmbInclFieldTest" runat="server" ToolTip="Select field test option" 
								Skin="Web20" Width="200"
							 EmptyMessage="&lt;Select One&gt;" 
								OnSelectedIndexChanged="cmbInclFieldTest_SelectedIndexChanged" AutoPostBack="true" 
								CausesValidation="False" DataTextField="InclFieldTest">
							</telerik:RadComboBox>
						</td>
					</tr>
					<tr>
						<td class="editorLabel">
							Number of Distractors:
						</td>
						<td class="editorControl">
							<telerik:RadComboBox ID="cmbNumDistractors" runat="server" ToolTip="Select the number of distractors" 
								Skin="Web20" Width="200"
							 EmptyMessage="&lt;Select One&gt;" 
								OnSelectedIndexChanged="cmbNumDistractors_SelectedIndexChanged" AutoPostBack="true" 
								CausesValidation="False" DataTextField="NumDistractors">
							</telerik:RadComboBox>
						</td>
					</tr>
					<tr>
						<td class="editorLabel">
							Distractor Labels:
						</td>
						<td class="editorControl">
							<telerik:RadComboBox ID="cmbDistractorLabels" runat="server" ToolTip="Select the number of distractors" 
								Skin="Web20" Width="200"
							 EmptyMessage="&lt;Select One&gt;" 
								OnSelectedIndexChanged="cmbDistractorLabels_SelectedIndexChanged" AutoPostBack="true" 
								CausesValidation="False" DataTextField="DistractorLabel" DataValueField="Value">
							</telerik:RadComboBox>
						</td>
					</tr>
					<tr>
						<td class="editorLabel">
							Score Type:
						</td>
						<td class="editorControl">
							<asp:Label runat="server" ID="lblScoreType" >Percent</asp:Label>
						</td>
					</tr>
					<tr>
						<td class="editorLabel">
							Performance Levels:
						</td>
						<td class="editorControl">
							<telerik:RadComboBox ID="cmbPerfLevels" runat="server" ToolTip="Select the performance level" 
								Skin="Web20" Width="200"
							 EmptyMessage="&lt;Select One&gt;" 
								OnSelectedIndexChanged="cmbPerfLevels_SelectedIndexChanged" AutoPostBack="true" 
								CausesValidation="False" DataTextField="PerformanceLevel">
							</telerik:RadComboBox>
						</td>
					</tr>
					<tr>
						<td class="editorLabel">
							Print Columns:
						</td>
						<td class="editorControl">
							<telerik:RadComboBox ID="cmbPrintCols" runat="server" ToolTip="Select the number of columns to print" 
								Skin="Web20" Width="200"
							 EmptyMessage="&lt;Select One&gt;" 
								OnSelectedIndexChanged="cmbPrintCols_SelectedIndexChanged" AutoPostBack="true" 
								CausesValidation="False" DataTextField="PrintColumn">
							</telerik:RadComboBox>
						</td>
					</tr>
					<tr>
						<td class="editorLabel">
							Print Short Answer Section<br />on Bubble Sheet:
						</td>
						<td class="editorControl">
							<telerik:RadComboBox ID="cmbPrintShortAns" runat="server" ToolTip="Select the number of columns to print" 
								Skin="Web20" Width="200"
							 EmptyMessage="&lt;Select One&gt;" 
								OnSelectedIndexChanged="cmbPrintShortAns_SelectedIndexChanged" AutoPostBack="true" 
								CausesValidation="False" DataTextField="PrintShortAnswer">
							</telerik:RadComboBox>
						</td>
					</tr>
					<tr id="trSource" runat="server">
						<td  class="editorLabel">
							Source:
						</td>
						<td class="editorControl">
							<telerik:RadTextBox ID="tbxSource" runat="server"  Skin="Web20" 
								Width="200" ontextchanged="tbxSource_TextChanged">
							</telerik:RadTextBox>
						</td>
					</tr>
					<tr>
						<td class="editorLabel">
							Author:
						</td>
						<td class="editorControl">
							<asp:Label runat="server" ID="lblAuthor" />
						</td>
					</tr>
					<tr>
						<td class="editorLabel">
							Date Created:
						</td>
						<td class="editorControl">
							<asp:Label runat="server" ID="lblCreated" />
						</td>
					</tr>
				</table>

				<div style="width: 610px;">
					<asp:Button runat="server" Width="100px" ID="okButton" ClientIDMode="Static" 
						CssClass="roundButtons" Text="Ok" onclick="okButton_Click" />
					<asp:Button runat="server" Width="100px" ID="cancelButton" 
						ClientIDMode="Static" CssClass="roundButtons" Text="Cancel" 
						onclick="cancelButton_Click"/>
				</div>		
			</div>
		</telerik:RadAjaxPanel>
		<telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
		</form>

		<script type="text/javascript">
			var pgRegMgr = Sys.WebForms.PageRequestManager.getInstance();
			pgRegMgr.add_endRequest(EndHandler);

			function GetRadWindow() {
				var oWindow = null;
				if (window.radWindow) oWindow = window.radWindow;
				else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
				return oWindow;
			}

			function EndHandler(sender, args) {
				if (args.get_error() == undefined && sender._postBackSettings.sourceElement != undefined) {
					var postbackTrigger = sender._postBackSettings.sourceElement.id;
					// Just close the editor window if cancel.
					if (postbackTrigger == "cancelButton")
						GetRadWindow().close();
					// Refresh the top-level window if changes were made.
					else if (postbackTrigger == "okButton")
						window.parent.location.reload(true);
				}
			}
		</script>

</body>
</html>
