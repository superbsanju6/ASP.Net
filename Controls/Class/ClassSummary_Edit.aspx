<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClassSummary_Edit.aspx.cs"
	Inherits="Thinkgate.Controls.Class.ClassSummary_Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<%--This height of 100% helps elements fill the page--%>
<head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=8" />
	<meta http-equiv="PRAGMA" content="NO-CACHE" />
	<meta http-equiv="Expires" content="-1" />
	<meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
	<link href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet" type="text/css" runat="server" />
	<title></title>
	<base target="_self" />
	<telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
		<StyleSheets>
			<telerik:StyleSheetReference Path="~/Styles/reset.css" />
			<telerik:StyleSheetReference Path="~/Styles/Site.css" />
		</StyleSheets>
	</telerik:RadStyleSheetManager>
	<script type='text/javascript' src='../../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../../Scripts/jquery-migrate-1.1.0.min.js'></script>
	<script type='text/javascript' src='../../Scripts/jquery.scrollTo.js'></script>
	<script type="text/javascript">        var $j = jQuery.noConflict();</script>
	<script type="text/javascript" src="../../Scripts/master.js"></script>
	<script type="text/javascript">
		var expandedWindow = getCurrentCustomDialog();
		//expandedWindow.add_beforeClose(cancelChanges);

		function primarySelect(checkbox) {
			var teachersGrid = $('#teachersGrid');
			if (!teachersGrid) return;
			if (checkbox.checked) $('input[id*="primaryCheckbox"]', teachersGrid).attr('checked', false);
			checkbox.checked = true;
		}

		function cancelChanges(sender, args) {
            /*
		    var classChanged = false;
			var teachersGrid = $('#teachersGrid');
			var rosterGrid = $('#rosterGrid_GridData[class^="rgMasterTable"]')[0];
			var primaryTeacherID = $('#primaryTeacherID').attr('value');
			var isInitialPrimaryTeacher = $('input[id*="primaryCheckbox"][teacherID="' + primaryTeacherID + '"]', teachersGrid)[0].checked;
			var isTeacherRemoveChecked = false;
			var isRosterRemoveChecked = false;

			$('input[id*="removeTeacherCheckbox"]', teachersGrid).each(function () {
				if ($(this).attr('checked')) {
					isTeacherRemoveChecked = true;
				}
			});

			$('input[id*="removeStudentCheckbox"]', rosterGrid).each(function () {
				if ($(this).attr('checked')) {
					isRosterRemoveChecked = true;
				}
			});

			if ($find('gradeDropdown')._value != $find('gradeDropdown')._attributes.getAttribute('initialValue')) classChanged = true;
			else if ($find('subjectDropdown')._value != $find('subjectDropdown')._attributes.getAttribute('initialValue')) classChanged = true;
			else if ($find('courseDropdown')._value != $find('courseDropdown')._attributes.getAttribute('initialValue')) classChanged = true;
			else if ($('#sectionTextBox').attr('value') != $('#sectionTextBox').attr('initialValue')) classChanged = true;
			else if ($find('periodDropdown')._value != $find('periodDropdown')._attributes.getAttribute('initialValue')) classChanged = true;
			else if ($find('semesterDropdown')._value != $find('semesterDropdown')._attributes.getAttribute('initialValue')) classChanged = true;
			else if ($('#blockTextBox').attr('value') != $('#blockTextBox').attr('initialValue')) classChanged = true;
			else if ($find('schoolDropdown')._value != $find('schoolDropdown')._attributes.getAttribute('initialValue')) classChanged = true;
			else if (!isInitialPrimaryTeacher) classChanged = true;
			else if (isTeacherRemoveChecked) classChanged = true;
			else if (isRosterRemoveChecked) classChanged = true;
            
			if (classChanged) {
				if (args.set_cancel) args.set_cancel(true);
            */
				var confirmDialogText = 'Are you sure you want to cancel? Any changes that have been made will not be saved.';
				customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'Cancel Changes', content: confirmDialogText, dialog_style: 'confirm' },
					[{ title: 'OK', callback: cancelCallbackFunction }, { title: 'Cancel'}]);
		/*	
		}
			else {
				if (expandedWindow) {
					expandedWindow.remove_beforeClose(cancelChanges);
					setTimeout(function() { expandedWindow.close(); }, 0);
				}
			}
        */
		}

		function cancelCallbackFunction() {
			if (expandedWindow) {
				expandedWindow.remove_beforeClose(cancelChanges);
				setTimeout(function() { expandedWindow.close(); }, 0);
			}
		}

		function deleteClass(sender, args) {
			var confirmDialogText = 'Are you sure you want to delete this class? Once deleted the class cannot be recovered.';
			customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'Delete Class', content: confirmDialogText, dialog_style: 'confirm' },
				[{ title: 'OK', callback: deleteCallbackFunction }, { title: 'Cancel'}]);
		}

		function AddTeacher() {
		    customDialog({ name: 'Add Teacher', maximize: true, maxwidth: 850, maxheight: 600, title: 'Add Teacher', url: '../Teacher/TeacherSearch_Expanded.aspx?AddTeacher=Yes' });
		}

		function AddTeachers(TeacherList) {
		    document.getElementById('addTeacherList').value = TeacherList;
		    __doPostBack();
		}

		function deleteCallbackFunction() {
			var deleteButton = $find('deleteButton');
			var classID = deleteButton.get_element().getAttribute('classID');
			var panel = $find('deleteClassXmlHttpPanel');

			panel.set_value('{"ClassID":' + classID + '}');
		}

		function refreshParentPage(sender, args) {
			var result = args.get_content();

			if (result.indexOf('Error') > -1) {
				setTimeout(function () { parent.radalert(result, 300, 100); }, 0);
			}
			else {
				parent.window.reload();
			}
		}

		function saveCallbackFunction(sender, args) {
		    var saveButtonTrigger = $find('saveButton');

		    if (saveButtonTrigger) __doPostBack(saveButtonTrigger._uniqueID, '');
		}


		function SaveClass(sender, args) {
		    var saveButton = $find('saveButton');
		    var classID = saveButton.get_element().getAttribute('classID');
		    var grade = $find('gradeDropdown')._value;
		    var subject = $find('subjectDropdown')._value;
		    var course = $find('courseDropdown')._value;
		    var section = document.getElementById('sectionTextBox').value;
		    var period = $find('periodDropdown')._value;
		    var semester = $find('semesterDropdown')._value;
		    var block = document.getElementById('blockTextBox').value;
		    var school = $find('schoolDropdown')._value;
		    var retentionFlag = $find('rbRetainOnResyncYes')._checked;
		    var panel = $find('updateClassXmlHttpPanel');

		    panel.set_value('{"ClassID":"' + classID + '","Course":"' + course + '","Section":"' + section + '","Block":"' + block + '","Period":"' + period + '","Semester":"' + semester + '","RetentionFlag":"' + retentionFlag + '","School":"' + school + '","UserID":"0"}');
		}

		function SavePrimaryTeacher(sender, args) {
		    document.getElementById('primaryTeacherID') = sender.getAttribute('teacherID');
		    __doPostBack();
		}

		//function saveClass(sender, args) {
		//	var teachersGrid = $('#teachersGrid');
		//	var rosterGrid = $('#rosterGrid_GridData');
		//	var allTeachersSelected = true;
		//	var isRemovingPrimaryTeacher = false;
		//	var teacherRemovalCount = 0;
		//	var studentRemovalCount = 0;

		//	$('input[id*="removeTeacherCheckbox"]', teachersGrid).each(function () {
		//		if ($(this).attr('checked')) {
		//			teacherRemovalCount++;

		//			if ($('input[id*="primaryCheckbox"][teacherID="' + $(this).attr('teacherID') + '"]', teachersGrid)[0].checked) {
		//				isRemovingPrimaryTeacher = true;
		//			}
		//		}
		//		else {
		//			allTeachersSelected = false;
		//		}
		//	});

		//	$('input[studentID]', rosterGrid).each(function () {
		//		if ($(this).attr('checked')) {
		//			studentRemovalCount++;
		//		}
		//	});

		//	if (allTeachersSelected) {
		//		setTimeout(function () { radalert('At least one teacher must be assigned to this class. Please uncheck a minimum of one teacher from the remove column.', 350, 100); }, 0);
		//		return false;
		//	}
		//	else if (isRemovingPrimaryTeacher) {
		//		setTimeout(function () { radalert('At least one teacher must be assigned as the primary teacher. Please select a new primary teacher.', 350, 100); }, 0);
		//		return false;
		//	}

		//	if (teacherRemovalCount > 0 || studentRemovalCount > 0) {
		//		var confirmDialogText = '';
		//		if (teacherRemovalCount > 0 && studentRemovalCount > 0) {
		//			confirmDialogText = 'You have removed ' + teacherRemovalCount + ' teacher' + (teacherRemovalCount > 1 ? 's' : '')
		//				+ ' and ' + studentRemovalCount + ' student' + (studentRemovalCount > 0 ? 's' : '') + ' from this class. Do you wish to continue?';
		//		}
		//		else if (teacherRemovalCount > 0) {
		//			confirmDialogText = 'You have removed ' + teacherRemovalCount + ' teacher' + (teacherRemovalCount > 1 ? 's' : '') + ' from this class. Do you wish to continue?';
		//		}
		//		else if (studentRemovalCount > 0) {
		//			confirmDialogText = 'You have removed ' + studentRemovalCount + ' student' + (studentRemovalCount > 1 ? 's' : '') + ' from this class. Do you wish to continue?';
		//		}

		//		parent.customDialog({ width: 300, height: 100, resizable: false, title: 'Confirm removal', content: confirmDialogText, dialog_style: 'confirm' },
		//			[{ title: 'OK', callback: saveCallbackFunction }, { title: 'Cancel'}]);
		//	}
		//	else {
		//		__doPostBack(sender._uniqueID, '');
		//		if (expandedWindow) {
		//			expandedWindow.remove_beforeClose(cancelChanges);
		//			setTimeout(function() { expandedWindow.close(); }, 0);
		//		}
		//	}
		//}

		
	</script>
	<style type="text/css">
		.floatRight
		{
			float: right;
			margin-right: 35%;
		}
		
		.floatRight_aLittleMore
		{
			float: right;
			margin-right: 1px;
		}
		
		.floatRight_bottom
		{
			float: right;
			margin-right: 5px;
		}
		
		.floatLeft_bottom
		{
			float: left;
			margin-left: 5px;
		}
		
		.hiddenSaveButton
		{
			display: none;
		}
	</style>
</head>
<body>
	<form id="form2" runat="server" style="height: 100%;">
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
	<telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
		Skin="Thinkgate_Window" EnableEmbeddedSkins="False" Modal="True" InitialBehaviors="Close"
		Animation="None">
	</telerik:RadWindowManager>
	<telerik:RadAjaxPanel ID="radPanelClassSummary_Edit" runat="server" LoadingPanelID="loadingPanelClassSummary_Edit">
		<input type="hidden" id="primaryTeacherID" runat="server" />
        <input type="hidden" id="addTeacherList" runat="server" />
        <input type="hidden" id="isSaveClass" runat="server" />
		<table style="width: 100%; height: 595px;">
			<tr>
				<td width="40%" style="vertical-align: top; padding: 5px;">
					<div runat="server" id="divIdentificationPermission" clientidmode="Static">
						<h1 class="dashboardSection">
							Identification</h1>
						<table width="100%" class="fieldValueTable">
							<tr>
								<td class="fieldLabel">
									Grade:
								</td>
								<td>
									<telerik:RadComboBox ID="gradeDropdown" runat="server" Width="200" AutoPostBack="false" Skin="Web20" xmlHttpPanelID="classFilterSubjectXmlHttpPanel" 
									OnClientSelectedIndexChanged="requestFilter" />
								</td>
							</tr>
							<tr>
								<td class="fieldLabel">
									Subject:
								</td>
								<td>
									<telerik:RadComboBox ID="subjectDropdown" runat="server" Width="200" AutoPostBack="false" Skin="Web20" xmlHttpPanelID="classFilterCourseXmlHttpPanel" 
									OnClientSelectedIndexChanged="requestFilter" />
								</td>
							</tr>
							<tr>
								<td class="fieldLabel">
									Course:
								</td>
								<td>
									<telerik:RadComboBox ID="courseDropdown" runat="server" Width="200" AutoPostBack="false"
										Skin="Web20" />
								</td>
							</tr>
							<!--
							<tr>
								<td colspan="2" style="text-align: center">
									<a href='javascript:OpenCourseSelectorWindow($find("CourseSelectorWindow").show());'>
										Change Course</a>
								</td>
							</tr>
							-->
							<tr>
								<td class="fieldLabel">
									Section:
								</td>
								<td>
									<asp:TextBox ID="sectionTextBox" ClientIDMode="Static" runat="server" Width="197" AutoPostBack="false" />
								</td>
							</tr>
							<tr>
								<td class="fieldLabel">
									Period:
								</td>
								<td>
									<telerik:RadComboBox ID="periodDropdown" runat="server" Width="200" AutoPostBack="false"
										Skin="Web20" />
								</td>
							</tr>
							<tr>
								<td class="fieldLabel">
									Semester:
								</td>
								<td>
									<telerik:RadComboBox ID="semesterDropdown" runat="server" Width="200" AutoPostBack="false"
										Skin="Web20" />
								</td>
							</tr>
							<tr>
								<td class="fieldLabel">
									Year:
								</td>
								<td>
									<asp:Label runat="server" ID="lblYear" />
								</td>
							</tr>
							<tr>
								<td class="fieldLabel">
									Block:
								</td>
								<td>
									<asp:TextBox ID="blockTextBox" ClientIDMode="Static" runat="server" Width="197" AutoPostBack="false"
										Skin="Web20" />
								</td>
							</tr>
							<tr>
								<td class="fieldLabel">
									School:
								</td>
								<td>
									<telerik:RadComboBox ID="schoolDropdown" runat="server" Width="200" AutoPostBack="false"
										Skin="Web20" />
								</td>
							</tr>
							<tr>
								<td class="fieldLabel">
									Retain on resync:
								</td>
								<td>
									<telerik:RadButton ID="rbRetainOnResyncYes" runat="server" ButtonType="ToggleButton" ToggleType="Radio" GroupName="RetainOnResync" Text="Yes"  AutoPostBack="false">
									</telerik:RadButton>
									&nbsp;&nbsp;&nbsp;
									<telerik:RadButton ID="rbRetainOnResyncNo" runat="server" ButtonType="ToggleButton" ToggleType="Radio" GroupName="RetainOnResync" Text="No" AutoPostBack="false">
									</telerik:RadButton>
								</td>
							</tr>
						</table>
					</div>
				</td>
				<td width="60%" style="vertical-align: top; padding: 5px;">
					<div runat="server" id="divTeacherPermission" clientidmode="Static">
						<h1 class="dashboardSection">
							Teacher
						</h1>
                        <div style="width: 95%; margin: 5px;">
                            <telerik:RadButton runat="server" ID="btnTeacherAdd" Text="Add Teacher"	Skin="Web20" AutoPostBack="false" OnClientClicked="AddTeacher" />
                            <telerik:RadButton style="margin-left: 5px;" runat="server" ID="btnTeacherRemove" Text="Remove Teacher" Skin="Web20" AutoPostBack="true" OnClick="btnTeacherRemove_Click" />
                            <asp:Label runat="server" ID="lblTeacherMessage" ForeColor="Red"></asp:Label>
                        </div>
						<div style="height: 100px; overflow: auto;">
							<telerik:RadGrid runat="server" ID="teachersGrid" AutoGenerateColumns="false" Width="65%"
								OnItemDataBound="Teacher_ItemDataBound">
								<MasterTableView>
									<Columns>
										<telerik:GridTemplateColumn InitializeTemplatesFirst="false" UniqueName="TeacherGridRemove" HeaderText="Remove"
											ItemStyle-Width="1" ItemStyle-HorizontalAlign="Center">
											<ItemTemplate>
												<asp:CheckBox runat="server" ID="removeTeacherCheckbox"></asp:CheckBox>
											</ItemTemplate>
										</telerik:GridTemplateColumn>
										<telerik:GridTemplateColumn InitializeTemplatesFirst="false" HeaderText="Name">
											<ItemTemplate>
												<asp:HyperLink runat="server" ID="teacherLink"></asp:HyperLink><br />
											</ItemTemplate>
										</telerik:GridTemplateColumn>
										<telerik:GridTemplateColumn InitializeTemplatesFirst="false" HeaderText="Primary"
											ItemStyle-Width="1" ItemStyle-HorizontalAlign="Center">
											<ItemTemplate>
												<asp:CheckBox runat="server" ID="primaryCheckbox" AutoPostBack="true" OnCheckedChanged="primaryCheckbox_CheckedChanged"></asp:CheckBox>
											</ItemTemplate>
										</telerik:GridTemplateColumn>
									</Columns>
								</MasterTableView>
							</telerik:RadGrid>
						</div>
					</div>
					<div runat="server" id="divRosterPermission" clientidmode="Static">
						<h1 class="dashboardSection" style="height:24px;">
							Roster: <span runat="server" id="studentCountSpan" style="font-style: italic; font-size: 11pt;
								color: #000;"></span>
							<telerik:RadButton runat="server" ID="addStudentButton" Text="Add Student" Skin="Web20"
								CssClass="floatRight_aLittleMore" AutoPostBack="false" Visible="false" OnClientClicked="functionIsInDevelopment" />
						</h1>
						<telerik:RadGrid runat="server" ID="rosterGrid" AutoGenerateColumns="false" Width="575"
							Height="400" OnItemDataBound="Roster_ItemDataBound">
							<MasterTableView>
								<Columns>
									<telerik:GridTemplateColumn InitializeTemplatesFirst="false" HeaderText="Remove"
										HeaderStyle-Width="65" ItemStyle-HorizontalAlign="Center" Visible="false">
										<ItemTemplate>
											<asp:CheckBox runat="server" ID="removeStudentCheckbox"></asp:CheckBox><br />
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridTemplateColumn InitializeTemplatesFirst="false" HeaderText="Name" HeaderStyle-Width="350">
										<ItemTemplate>
											<asp:HyperLink runat="server" ID="studentLink"></asp:HyperLink><br />
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridBoundColumn DataField="Grade.DisplayText" HeaderText="Grade" HeaderStyle-Width="75">
									</telerik:GridBoundColumn>
									<telerik:GridBoundColumn DataField="StudentID" HeaderText="Student&nbsp;ID">
									</telerik:GridBoundColumn>
								</Columns>
							</MasterTableView>
							<ClientSettings>
								<Scrolling AllowScroll="true" UseStaticHeaders="True" SaveScrollPosition="true" />
							</ClientSettings>
						</telerik:RadGrid>
					</div>
				</td>
			</tr>
		</table>
		<telerik:RadButton runat="server" Visible="false" ID="deleteButton" ClientIDMode="Static" Text="Delete Class"
			OnClientClicked="deleteClass" AutoPostBack="false" Skin="Web20" CssClass="floatLeft_bottom" />
		<telerik:RadButton runat="server" Visible="true" ID="saveButton" Text="Save Changes" OnClientClicked="SaveClass"
			Skin="Web20" CssClass="floatRight_bottom" />
		<telerik:RadButton runat="server" Visible="true" ID="cancelButton" Text="Cancel" OnClientClicked="cancelChanges"
			AutoPostBack="false" Skin="Web20" CssClass="floatRight_bottom" />
	</telerik:RadAjaxPanel>
	<telerik:RadAjaxLoadingPanel runat="server" ID="loadingPanelClassSummary_Edit">
	</telerik:RadAjaxLoadingPanel>
	<telerik:RadXmlHttpPanel runat="server" ID="deleteClassXmlHttpPanel" ClientIDMode="Static"
		Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/ClassWCF.svc"
		WcfServiceMethod="DeleteClass" RenderMode="Block" OnClientResponseEnding="refreshParentPage">
	</telerik:RadXmlHttpPanel>
    <telerik:RadXmlHttpPanel runat="server" ID="updateClassXmlHttpPanel" ClientIDMode="Static"
		Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/ClassWCF.svc"
		WcfServiceMethod="UpdateClass" RenderMode="Block" OnClientResponseEnded="saveCallbackFunction">
	</telerik:RadXmlHttpPanel>
	<telerik:RadXmlHttpPanel runat="server" ID="classFilterSubjectXmlHttpPanel" ClientIDMode="Static"
		Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/ClassWCF.svc"
		WcfServiceMethod="LoadClassSubjectList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
		objectToLoadID="subjectDropdown">
	</telerik:RadXmlHttpPanel>
	<telerik:RadXmlHttpPanel runat="server" ID="classFilterCourseXmlHttpPanel" ClientIDMode="Static"
		Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/ClassWCF.svc"
		WcfServiceMethod="LoadClassCourseList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
		objectToLoadID="courseDropdown">
	</telerik:RadXmlHttpPanel>
	</form>

	<script type="text/javascript">
		function requestFilter(sender, args) {
			var senderElement = sender.get_element();
			var itemText = args.get_item().get_text();
			var panelID = senderElement.getAttribute('xmlHttpPanelID');
			var panel = $find(panelID);

			switch (panelID) {
				case 'classFilterSubjectXmlHttpPanel':
					var courseDropdown = $find('courseDropdown');
					clearAllDropdownItems(courseDropdown);
					panel.set_value('{"Grade":"' + itemText + '"}');
					break;
				case 'classFilterCourseXmlHttpPanel':
					var gradeDropdown = $find('gradeDropdown');
					var grade = gradeDropdown.get_selectedItem().get_value();
					panel.set_value('{"Grade":"' + grade + '","Subject":"' + itemText + '"}');
					break;
			}

		}

		function loadChildFilter(sender, args) {
			//load panel
			var senderElement = sender.get_element();
			var results = args.get_content();
			var dropdownObjectID = senderElement.getAttribute('objectToLoadID');
			var dropdownObject = $find(dropdownObjectID);

			//Clear all context menu items
			clearAllDropdownItems(dropdownObject);

			/*Add each new context menu item
			results[i].Key(value) = ID
			results[i].Value(text) = display text
			*/
			for (var i = 0; i < results.length; i++) {
				addDropdownItem(dropdownObject, results[i][1], results[i][0]);
			}


			dropdownObject.get_items().getItem(0).select();


		}

		function addDropdownItem(dropdownObject, itemTextandValue, value) {
			if (!dropdownObject || !itemTextandValue) {
				return false;
			}

			/*indicates that client-side changes are going to be made and 
			these changes are supposed to be persisted after postback.*/
			dropdownObject.trackChanges();

			//Instantiate a new client item
			var item = new Telerik.Web.UI.RadComboBoxItem();

			//Set its text and add the item
			item.set_text(itemTextandValue);
			item.set_value(value);

			dropdownObject.get_items().add(item);

			//submit the changes to the server.
			dropdownObject.commitChanges();
		}

		function clearAllDropdownItems(dropdownObject) {
			var allItems = dropdownObject.get_items().get_count();
			if (allItems < 1) {
				return false;
			}

			/*indicates that client-side changes are going to be made and 
			these changes are supposed to be persisted after postback.*/
			dropdownObject.trackChanges();

			//clear all items
			dropdownObject.get_items().clear();

			//submit the changes to the server.
			dropdownObject.commitChanges();
			

			return false;
		}
	</script>
</body>
</html>
