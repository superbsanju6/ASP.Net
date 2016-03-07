<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentScans.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.AssessmentScans" %>
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
		<title>Import Jobs</title>
		<base target="_self" />
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
            <asp:ScriptReference Path="~/Scripts/master.js" />
		</Scripts>
	</telerik:RadScriptManager>

    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false" Skin="Thinkgate_Window" EnableEmbeddedSkins="false">
			
	</telerik:RadWindowManager>

	<asp:Label ID="lblNumRecs" runat="server" Text="" Width="800px" Font-Bold="True" style="margin-top: 10px; margin-bottom: 10px; text-align: center;"></asp:Label>

    <telerik:RadButton ID="btnUpload" runat="server" CssClass="radDropdownBtn" Skin="Web20" ClientIDMode="Static" AutoPostBack="false" Text="Upload Results" style="vertical-align: baseline" OnClientClicked="ImportTestResponseCard"></telerik:RadButton>

    <input id="inpxID" type="hidden" value="0" runat="server" />
       
	<telerik:RadGrid ID="grdScans" runat="server" AllowSorting="true"
			 OnNeedDataSource="grdScans_NeedDataSource"
			 Skin="Web20" AutoGenerateColumns="false" style="margin-left: 4px; margin-right: 4px; height: 500">
			<ClientSettings Selecting-AllowRowSelect="false" EnableRowHoverStyle="True">
				<Selecting AllowRowSelect="False" />
				<Scrolling AllowScroll="True" ScrollHeight="500px" UseStaticHeaders="false" />
			</ClientSettings>
			<SortingSettings SortedBackColor="Bisque" EnableSkinSortStyles="false" />

			<MasterTableView Width="100%">
				<Columns>
					<telerik:GridTemplateColumn HeaderText="Job ID" ShowSortIcon="true" SortExpression="JobID" SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
						<ItemTemplate>
							<span><%# Eval("JobID") %></span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="#Cards" ShowSortIcon="true" SortExpression="#Cards" SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
						<ItemTemplate>
							<span><%# Eval("#Cards")%></span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="#Errors" ShowSortIcon="true" SortExpression="#Error" SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
						<ItemTemplate>
							<span><%# Eval("#Error")%></span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Reject/Repair" ShowSortIcon="false" >
						<ItemTemplate>
							<img id="btnRepair" alt="Reject/Repair Scores" title="Reject/Repair Scores" src="../../Images/tools.png"
								 style='cursor: pointer; visibility: <%# DataBinder.Eval(Container.DataItem, "RepairVisibility")%>; width: 24px; height: 24px;'
								 onclick='openRejectRepair(<%# Eval("JobID") %>)' />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Teacher" ItemStyle-Wrap="false" ShowSortIcon="true" SortExpression="TeacherName" SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
						<ItemTemplate>
							<span><%# Eval("TeacherName")%></span>
						</ItemTemplate>
						<ItemStyle Wrap="False"></ItemStyle>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Period" ShowSortIcon="true" SortExpression="Period" SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
						<ItemTemplate>
							<span><%# Eval("Period")%></span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Date/Time" ShowSortIcon="true" SortExpression="fileDate" SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
						<ItemTemplate>
							<span><%# Eval("fileDate")%></span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="User" ItemStyle-Wrap="false" ShowSortIcon="true" SortExpression="UserName" SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
						<ItemTemplate>
							<span><%# Eval("UserName")%></span>
						</ItemTemplate>
						<ItemStyle Wrap="False"></ItemStyle>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="View File" ShowSortIcon="false" >
						<ItemTemplate>
							<img id="btnViewFile" alt="View File" title="View File" src="../../Images/ViewPage.png"
								 style="cursor: pointer; visibility: visible; width: 18px; height: 24px;" onclick='viewFile(&quot;<%# Eval("FileName") %>&quot;)' />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
			</MasterTableView>
	</telerik:RadGrid>

	<script type="text/javascript">
		// Cause the old systems' Reject-Repair window to open with the passed job id.
		function openRejectRepair(jobid) {
			open('../../SessionBridge.aspx?ReturnURL=' + escape('display.asp?formatoption=search%20results&key=7109&retrievemode=searchpage&??KF=jobid&??KeyID=') + jobid + escape('&??Days=0'), 'rejectrepair', 'height=560, width=900, location=no, menubar=no, resizable=yes, scrollbars=yes, status=no, toolbar=no');
		}

		// View the csv file for a scan.
		function viewFile(filename) {
//			alert(filename);
			open('../../SessionBridge.aspx?ReturnURL=' + escape('DBACCESS/') + filename, 'fileview', 'height=560, width=770, location=no, menubar=no, resizable=yes, scrollbars=yes, status=no, toolbar=no');
		}

		function ImportTestResponseCard() {
		    var xID = document.getElementById("inpxID").value;

		    parent.customDialog({
		        url: '../Dialogues/Assessment/ImportTestResponseCard.aspx?xID=' + xID,
		        title: "Test Respond Card Import",
		        maximize: true, maxwidth: 700, maxheight: 390,
		        autoSize: false
		    });
		}
	</script>
</form>
</body>
</html>