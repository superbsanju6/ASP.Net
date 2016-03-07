<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentPrintBubbleSheets.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.AssessmentPrintBubbleSheets" %>
<%@ Import Namespace="System.Drawing" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
		<meta http-equiv="X-UA-Compatible" content="IE=8" />
		<meta http-equiv="PRAGMA" content="NO-CACHE" />
		<meta http-equiv="Expires" content="-1" />
		<meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
		<link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />    
		<title>Print Assessment Bubble Sheets</title>
		<base target="_self" />
 
		<style type="text/css">
			.radioItem
			{
				margin: 4px;
				font-weight: bold;
				font-size: 11pt;
			}

			.radioText
			{
				font-weight: bold;
				font-size: 11pt;
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
				cursor: pointer;
				background-color: #FFF;
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

	<input id="inpUserID" type="hidden" value="0" runat="server"/>
	<input id="inpCteID" type="hidden" value="0" runat="server"/>
	<input id="inpStudentIdsCsv" type="hidden" value="" runat="server"/>

<asp:Panel ID="Panel1" runat="server" Width="400px">
	<br />
	<telerik:RadButton  ID="btnStudentForm" runat="server" GroupName="PrintOption" Skin="Web20" 
		Text="Student Form"  ButtonType="ToggleButton" ToggleType="Radio" CssClass="radioItem" OnClientCheckedChanged="checkedChanged" Enabled="false" />
	<br />
	<telerik:RadButton  ID="btnRosterForm" runat="server" GroupName="PrintOption" Skin="Web20" 
		Text="Roster Form" ButtonType="ToggleButton" ToggleType="Radio" CssClass="radioItem" OnClientCheckedChanged="checkedChanged" Enabled="false" />
	<br />
	<telerik:RadButton ID="btnBlankForm" runat="server" GroupName="PrintOption" Skin="Web20" 
		Text="Blank Form" ButtonType="ToggleButton" ToggleType="Radio" CssClass="radioItem" OnClientCheckedChanged="checkedChanged" Enabled="false" />
	<br />
	<telerik:RadButton ID="btnHaloNonCal" runat="server" GroupName="PrintOption" Skin="Web20" 
		Text="HALO Non-Calibrated" ButtonType="ToggleButton" ToggleType="Radio" CssClass="radioItem" OnClientCheckedChanged="checkedChanged" Enabled="false" />
	<br />
	<telerik:RadButton ID="btnHaloCal" runat="server"  GroupName="PrintOption" Skin="Web20" 
		Text="HALO Calibrated" ButtonType="ToggleButton" ToggleType="Radio" CssClass="radioItem" OnClientCheckedChanged="checkedChanged" Enabled="false" />

	<br />
	<br />
	<asp:Panel ID="Panel2" runat="server" Width="200px" style="float: right; margin-right: 4px;">
		<asp:Button runat="server" ID="btnPrint" ClientIDMode="Static" CssClass="roundButtons" Text="&nbsp;&nbsp;&nbsp;&nbsp;Print&nbsp;&nbsp;&nbsp;&nbsp;" 
			OnClientClick="doPrint(); return false;" />
		<asp:Button runat="server" ID="btnCancel" ClientIDMode="Static" CssClass="roundButtons" Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" 
			OnClientClick="closeCurrentCustomDialog(); return false;" />
	</asp:Panel>
</asp:Panel>
	<script type="text/javascript">
		window.onload = function () {
			checkedChanged(null, null);
		}

		// A radio button changed.
		function checkedChanged(sender, args) {
			var disabled = true;
			var btn = $find("<%=btnStudentForm.ClientID %>");
			if (btn.get_checked())
				disabled = false;
			btn = $find("<%=btnRosterForm.ClientID %>");
			if (btn.get_checked())
				disabled = false;
			btn = $find("<%=btnBlankForm.ClientID %>");
			if (btn.get_checked())
				disabled = false;
			btn = $find("<%=btnHaloNonCal.ClientID %>");
			if (btn.get_checked())
				disabled = false;
			btn = $find("<%=btnHaloCal.ClientID %>");
			if (btn.get_checked())
				disabled = false;

			var printBtn = document.getElementById("btnPrint");
			printBtn.disabled = disabled ;
		}

		function doPrint() {
			// We will need the class test event.
			var cteID = document.getElementById("inpCteID").value;
			// And the csv list of selected students.
			var studentsCsv = document.getElementById("inpStudentIdsCsv").value;
			// And the current user id.
			var userID = document.getElementById("inpUserID").value;

//			var cteList = cteID.split(",");
//			if (cteList.length > 220) {
//			    alert("MAXC");
//			    return false;
//			}

			// Do student form.
			var btn = $find("<%=btnStudentForm.ClientID %>");
			if (btn.get_checked()) {
			    if (cteID.indexOf(",") > -1)
			        open('../../SessionBridge.aspx?ReturnURL=' + escape('pageactions.asp?action=route&formatoption=refresh&retrievemode=page&buttonid=8918/PPSStudent_multi&passthroughparms=yes&??CurUser=') + userID.toString() + escape('&??FormType=Student&CurrURL=??ClassTestEventIDList=') + ',' + cteID.toString() + ',');
                else
                    open('../../SessionBridge.aspx?ReturnURL=' + escape('pageactions.asp?action=route&formatoption=refresh&retrievemode=page&buttonid=8917/PPSStudent&passthroughparms=yes&??CurUser=') + userID.toString() + escape('&??FormType=Student&??StudentIDList=') + studentsCsv + escape('&CurrURL=??ClassTestEventID=') + cteID.toString());
			}

			// Do roster form.
			btn = $find("<%=btnRosterForm.ClientID %>");
			if (btn.get_checked()) {
			    if (cteID.indexOf(",") > -1)
			        open('../../SessionBridge.aspx?ReturnURL=' + escape('pageactions.asp?action=route&formatoption=refresh&retrievemode=page&buttonid=8918/PPSRoster_multi&passthroughparms=yes&??CurUser=') + userID.toString() + escape('&??FormType=StudentList&CurrURL=??ClassTestEventIDList=') + ',' + cteID.toString() + ',');
			    else
				    open('../../SessionBridge.aspx?ReturnURL=' + escape('pageactions.asp?action=route&formatoption=refresh&retrievemode=page&buttonid=8917/PPSRoster&passthroughparms=yes&??CurUser=') + userID.toString() + escape('&??FormType=StudentList&??StudentIDList=&CurrURL=??ClassTestEventID=') + cteID.toString());
			}

			// Do blank form.
			btn = $find("<%=btnBlankForm.ClientID %>");
			if (btn.get_checked()) {
			    if (cteID.indexOf(",") > -1)
			        open('../../SessionBridge.aspx?ReturnURL=' + escape('pageactions.asp?action=route&formatoption=refresh&retrievemode=page&buttonid=8918/PPSBlank_multi&passthroughparms=yes&??CurUser=') + userID.toString() + escape('&??FormType=Blank&CurrURL=??ClassTestEventIDList=') + ',' + cteID.toString() + ',');
			    else
				    open('../../SessionBridge.aspx?ReturnURL=' + escape('pageactions.asp?action=route&formatoption=refresh&retrievemode=page&buttonid=8917/PPSBlank&passthroughparms=yes&??CurUser=') + userID.toString() + escape('&??FormType=Blank&??StudentIDList=&CurrURL=??ClassTestEventID=') + cteID.toString());
			}

			// Do halo non calibrated.
			btn = $find("<%=btnHaloNonCal.ClientID %>");
			if (btn.get_checked()) {
			    if (cteID.indexOf(",") > -1)
			        open('../../SessionBridge.aspx?ReturnURL=' + escape('pageactions.asp?action=route&formatoption=refresh&retrievemode=page&buttonid=8918/HALO Preslug_multi&passthroughparms=yes&??CurUser=') + userID.toString() + escape('&??FormType=HALO&CurrURL=??ClassTestEventIDList=') + ',' + cteID.toString() + ',');
			    else
			        open('../../SessionBridge.aspx?ReturnURL=' + escape('pageactions.asp?action=route&formatoption=refresh&retrievemode=page&buttonid=8914/HALO Preslug_V2&passthroughparms=yes&??calid=0&CurrURL=??ClassTestEventID=') + cteID.toString() + escape('&??slist=') + "'" + studentsCsv + "'");
			}

			// Do halo calibrated print.
			btn = $find("<%=btnHaloCal.ClientID %>");
			if (btn.get_checked()) {
			    if (cteID.indexOf(",") > -1)
			        open('../../SessionBridge.aspx?ReturnURL=' + escape('preslug_multi.asp?cteid=') + cteID + ',');
			    else
			        open('../../SessionBridge.aspx?ReturnURL=' + escape('preslug.asp?cteid=') + cteID + escape('&slist=')  + studentsCsv );
			}
		}

		</script>

</form>
</body>
</html>