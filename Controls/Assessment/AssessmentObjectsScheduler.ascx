<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentObjectsScheduler.ascx.cs"
	Inherits="Thinkgate.Controls.Assessment.AssessmentObjectsScheduler" %>
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
	}
	
	.areaTitle
	{
		font-size: 10pt;
		font-weight: bold;
		margin-left: 4px;
	}
	
	.statusText
	{
		font-size: 10pt;
		height: 100%;
	}

	.dateText
	{
		font-size: 10pt;
	}

</style>

<div class="borders" style="height: 78px; line-height: 78px; width:309px; background-color: rgb(243, 247, 252);">
	<span class="areaTitle">Administration:&nbsp;</span>
	<span class="statusText" runat="server" id="lblAdminStatus"></span>
</div>

<div class="borders" style="height: 78px; line-height: 78px; width:309px; background-color: rgb(252, 245, 230);">
	<span class="areaTitle">Content:&nbsp;</span>
	<span class="statusText" runat="server" id="lblContStatus"></span>
</div>

<div class="borders roundBottom" style="height: 78px; line-height: 78px; width:309px; background-color: rgb(255, 255, 229);">
	<span class="areaTitle">Print:&nbsp;</span>
	<span class="statusText" runat="server" id="lblPrintStatus"></span>
</div>
