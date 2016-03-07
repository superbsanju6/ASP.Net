<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentVersionPrompt.aspx.cs"
		Inherits="Thinkgate.Dialogues.Assessment.AssessmentVersionPrompt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
		<meta http-equiv="X-UA-Compatible" content="IE=8" />
		<meta http-equiv="PRAGMA" content="NO-CACHE" />
		<meta http-equiv="Expires" content="-1" />
		<meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
		<link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
		<title>Select Version</title>
		<base target="_self" />
		<style type="text/css">
			.textStyle
			{
				font-size: 12pt;
				font-weight: bold;
				text-align: center;
				width: 100%;
			}
		</style>
</head>
<body style="background-color: LightSteelBlue; font-family: Arial, Sans-Serif; font-size: 10pt;">
		<form runat="server" id="mainForm" method="post">
		<telerik:RadScriptManager ID="RadScriptManager1" runat="server">
				<Scripts>
						<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
						<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
						<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
						<asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
						<asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
						<asp:ScriptReference Path="~/scripts/master.js" />
				</Scripts>
		</telerik:RadScriptManager>
		<telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
				Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />
		<input id="inpAssessmentID" type="hidden" value="0" runat="server" />
		<input id="inpAssessmentName" type="hidden" value="true" runat="server" />
		<asp:Panel ID="Panel1" runat="server" Width="400" Font-Size="11pt" Font-Bold="True">
				<div style="margin-top: 30px; width: 100%;">
					<span class="textStyle">Please select the version of this assessment you would like to print.</span>
					<br />
					<br />
					<a class="textStyle" style="float:right" href="#" onclick="printAssessmentContent(); return false;">Assessment Content</a>
					<br />
					<br />
					<a class="textStyle" style="float:right" href="#" onclick="printUploadedContent(); return false;">Uploaded Document</a>
				</div>
		</asp:Panel>
		<script type="text/javascript">
				window.onload = function () {
				};

				function printAssessmentContent() {
					var assessmentIDEle = document.getElementById("inpAssessmentID");
					var assessmentID = assessmentIDEle.value;
					var assessmentNameEle = document.getElementById("inpAssessmentName");
					var assessmentName = assessmentNameEle.value;
				    //parent.customDialog({ url: ('AssessmentPrint.aspx?xID=' + assessmentID.toString() + '&yID=' + escape(assessmentName)) + '&zID=Assmt', autoSize: true, name: 'AssessmentPrint' });
					parent.customDialog({ url: ('<%=Request.ApplicationPath%>/Dialogues/Assessment/AssessmentPrint.aspx?xID=' + assessmentID.toString() + '&yID=' + escape(assessmentName)) + '&zID=Assmt', maximize: true, maxwidth: 520, maxheight: 400, name: 'AssessmentPrint' });
				}

				function printUploadedContent() {
					var assessmentIDEle = document.getElementById("inpAssessmentID");
					var assessmentID = assessmentIDEle.value;
					var assessmentNameEle = document.getElementById("inpAssessmentName");
					var assessmentName = assessmentNameEle.value;
					parent.customDialog({ url: ('<%=Request.ApplicationPath%>/Dialogues/Assessment/AssessmentPrint.aspx?xID=' + assessmentID.toString() + '&yID=' + escape(assessmentName)) + '&zID=Upload', maximize: true, maxwidth: 520, maxheight: 400, name: 'AssessmentPrint' });
				}

		</script>
		</form>
</body>
</html>
