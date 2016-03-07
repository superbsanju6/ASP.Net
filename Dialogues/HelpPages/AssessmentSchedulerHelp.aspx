<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentSchedulerHelp.aspx.cs"
	Inherits="Thinkgate.Dialogues.HelpPages.AssessmentSchedulerHelp" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=8" />
	<meta http-equiv="PRAGMA" content="NO-CACHE" />
	<meta http-equiv="Expires" content="-1" />
	<meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
	<link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
	<link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4"
		rel="stylesheet" type="text/css" runat="server" />
	<title>Help</title>
	<base target="_self" />
	<style type="text/css">
		.roundBottom
		{
			-moz-border-radius-bottomright: 5px;
			-moz-border-radius-bottomleft: 5px;
			-webkit-border-bottom-right-radius: 5px;
			-webkit-border-bottom-left-radius: 5px;
			border-bottom-left-radius: 5px;
			border-bottom-right-radius: 5px;
		}
		
		.borders
		{
			border-bottom: 1px solid Black;
			border-left: 1px solid Black;
			border-right: 1px solid Black;
			height: 130px;
			width: 100%;
		}
		
		.areaTitle
		{
			font-size: 12pt;
			font-weight: bold;
			text-align: center;
		}
		
		.statusText
		{
			font-size: 10pt;
			margin-left: 20px;
			margin-right: 20px;
		}
	</style>
</head>
<body style="background-color: white; font-family: Arial, Sans-Serif; font-size: 10pt;">
	<div style="width: 400px;">
		<div class="borders" style="background-color: rgb(243, 247, 252);">
			<br />
			<p class="areaTitle">Administration</p>
			<br />
			<p class="statusText"><b><%= Thinkgate.Base.Enums.AssessmentScheduling.AssessmentScheduleStatus.Disabled.ToString() %></b> - Assessment is closed for administration. Assessment administration screens are disabled.</p>
			<br />
			<p class="statusText"><b><%= Thinkgate.Base.Enums.AssessmentScheduling.AssessmentScheduleStatus.Enabled.ToString() %></b> - Assessment is open for administration.</p>
		</div>
		<div class="borders" style="background-color: rgb(252, 245, 230);">
			<br />
			<p class="areaTitle">Content</p>
			<br />
			<p class="statusText"><b><%= Thinkgate.Base.Enums.AssessmentScheduling.AssessmentScheduleStatus.Disabled.ToString() %></b> - Content links are disabled on reports.</p>
			<br />
			<p class="statusText"><b><%= Thinkgate.Base.Enums.AssessmentScheduling.AssessmentScheduleStatus.Enabled.ToString() %></b> - Content links are enabled on reports.</p>
		</div>
		<div class="borders" style="background-color: rgb(255, 255, 229);">
			<br />
			<p class="areaTitle">Print</p>
			<br />
			<p class="statusText"><b><%= Thinkgate.Base.Enums.AssessmentScheduling.AssessmentScheduleStatus.Disabled.ToString() %></b> - Printing of assessments is not available.</p>
			<br />
			<p class="statusText"><b><%= Thinkgate.Base.Enums.AssessmentScheduling.AssessmentScheduleStatus.Enabled.ToString() %></b> - Printing of assessments is available.</p>
		</div>
	</div>
</body>
</html>
