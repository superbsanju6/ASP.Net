<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditAssessmentIdentification.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.EditAssessmentIdentification" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=8" />
	<meta http-equiv="PRAGMA" content="NO-CACHE" />
	<meta http-equiv="Expires" content="-1" />
	<meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
	<link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />    
	<title>Edit Assessment Identification</title>
	<base target="_self" />
		<style type="text/css">
				body 
				{
						font-family: Sans-Serif, Arial;
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
<body oncontextmenu="return false;">
		<div class="contextMenuDiv" oncontextmenu="return false;"></div>
		<form runat="server" id="mainForm" method="post">
				<telerik:RadScriptManager ID="RadScriptManager1" runat="server">
				<Scripts>
					<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
					<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
				</Scripts>
			</telerik:RadScriptManager>
<!--				<img runat="server" id="headerImg" src="../../Images/repairtool.png" alt="Create assessment icon" class="headerImg" /> -->
				<div class="containerDiv" align="center">
<span>Under Construction</span>
						<div class="formContainer" style="visibility: hidden;">
								<div class="labels">Grade:</div>
								<telerik:RadButton ID="gradeButton" Skin="Web20" ClientIDMode="Static" runat="server" Text="&lt;Select One&gt;" AutoPostBack="false"
										OnClientClicked="dropdownButtonClick" Width="200px" dropdownListID="gradeList" CssClass="formElement" />
								<telerik:RadContextMenu ID="gradeList" ClientIDMode="Static" runat="server" 
										dropdownButtonID="gradeButton" OnClientItemBlur="requestSubjectFilter" teacherID="" 
										defaultChildButtonText="&lt;Select One&gt;" xmlHttpPanelID="subjectXmlHttpPanel" Skin="Web20">
								</telerik:RadContextMenu>

								<div class="labels">Subject:</div>
								<telerik:RadButton ID="subjectButton" Skin="Web20" ClientIDMode="Static" runat="server" Text="&lt;Select One&gt;" AutoPostBack="false"
										OnClientClicked="dropdownButtonClick" Width="200px" dropdownListID="subjectList" CssClass="formElement" />
								<telerik:RadContextMenu ID="subjectList" ClientIDMode="Static" runat="server"
										dropdownButtonID="subjectButton" OnClientItemBlur="requestCourseFilter" teacherID="" 
										defaultChildButtonText="&lt;Select One&gt;" xmlHttpPanelID="courseXmlHttpPanel" Width="200" Skin="Web20">
								</telerik:RadContextMenu>

								<div class="labels">Course:</div>
								<telerik:RadButton ID="courseButton" Skin="Web20" ClientIDMode="Static" runat="server" Text="&lt;Select One&gt;" AutoPostBack="false"
										OnClientClicked="dropdownButtonClick" Width="200px" dropdownListID="courseList" CssClass="formElement" />
								<telerik:RadContextMenu ID="courseList" ClientIDMode="Static" runat="server" OnClientItemClicked="dropdownItemsClick" 
										dropdownButtonID="courseButton" OnClientItemBlur="confirmClearStandards" defaultChildButtonText="&lt;Select One&gt;" Width="200" Skin="Web20">
								</telerik:RadContextMenu>

								<div class="labels">Type:</div>
								<telerik:RadButton ID="typeButton" Skin="Web20" ClientIDMode="Static" runat="server" Text="Test" AutoPostBack="false"
										OnClientClicked="dropdownButtonClick" Width="200px" dropdownListID="typeList" CssClass="formElement" />
								<telerik:RadContextMenu ID="typeList" ClientIDMode="Static" runat="server" OnClientItemClicked="dropdownItemsClick" 
										dropdownButtonID="typeButton" defaultChildButtonText="&lt;Select One&gt;" Width="200" Skin="Web20">
								</telerik:RadContextMenu>

								<div class="labels">Term:</div>
								<telerik:RadButton ID="termButton" Skin="Web20" ClientIDMode="Static" runat="server" Text="&lt;Select One&gt;" AutoPostBack="false"
										OnClientClicked="dropdownButtonClick" Width="200px" dropdownListID="termList" CssClass="formElement" />
								<telerik:RadContextMenu ID="termList" ClientIDMode="Static" runat="server" OnClientItemClicked="dropdownItemsClick" 
										dropdownButtonID="termButton" defaultChildButtonText="&lt;Select One&gt;" Width="200" Skin="Web20">
										<Items>
												<telerik:RadMenuItem Text="1" Width="200" />
												<telerik:RadMenuItem Text="2" Width="200" />
												<telerik:RadMenuItem Text="3" Width="200" />
												<telerik:RadMenuItem Text="4" Width="200" />
										</Items>
								</telerik:RadContextMenu>

								<div class="labels">Content:</div>
								<telerik:RadButton ID="contentButton" Skin="Web20" ClientIDMode="Static" runat="server" Text="Item Bank" AutoPostBack="false"
										OnClientClicked="dropdownButtonClick" Width="200px" dropdownListID="contentList" CssClass="formElement" />
								<telerik:RadContextMenu ID="contentList" ClientIDMode="Static" runat="server" OnClientItemClicked="dropdownItemsClick" 
										dropdownButtonID="contentButton" defaultChildButtonText="Item Bank" Width="200" Skin="Web20">
										<Items>
												<telerik:RadMenuItem Text="Item Bank" Width="200" Selected="true" />
												<telerik:RadMenuItem Text="External" Width="200" />
										</Items>
								</telerik:RadContextMenu>

								<div class="labels">Description:</div>
								<input runat="server" type="text" id="descriptionInput" clientidmode="Static" name="descriptionInput" class="formElement" />


								<asp:Button runat="server" ID="nextButton" ClientIDMode="Static" CssClass="roundButtons" Text="&nbsp;&nbsp;Continue&nbsp;&nbsp;" 
								OnClientClick="createAssessment('submitXmlHttpPanel'); return false;" />
								<asp:Button runat="server" ID="backButton" ClientIDMode="Static" CssClass="roundButtons" Text="&nbsp;&nbsp;Back&nbsp;&nbsp;"
								OnClientClick="parent.openDialogRadWindowUrl('../Dialogues/Assessment/CreateAssessmentOptions.aspx', {title: 'Options to Create Assessment', width: '550', height: '450'}, null, parent.assessmentDialogCloseCallback); return false;" />
								<asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons" Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" 
								OnClientClick="parent.dialogRadWindow_customButtonClose(); return false;" />

								<input runat="server" type="hidden" id="assessmentID" name="assessmentID" value="" />
								<input runat="server" type="hidden" id="standardsCleared" name="standardsCleared" value="true"/>
						</div>
				</div>

				<telerik:RadXmlHttpPanel runat="server" ID="subjectXmlHttpPanel" ClientIDMode="Static" Value="" WcfRequestMethod="POST" 
				WcfServicePath="~/Services/TeacherWCF.svc" WcfServiceMethod="RequestSubjectList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
				objectToLoadID="subjectList">
				</telerik:RadXmlHttpPanel>
				<telerik:RadXmlHttpPanel runat="server" ID="courseXmlHttpPanel" ClientIDMode="Static" Value="" WcfRequestMethod="POST" 
				WcfServicePath="~/Services/TeacherWCF.svc" WcfServiceMethod="RequestCourseList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
				objectToLoadID="courseList">
				</telerik:RadXmlHttpPanel>
				<span style="display:none;">
				<telerik:RadXmlHttpPanel runat="server" ID="clearStandardsXmlHttpPanel" ClientIDMode="Static" Value="" WcfRequestMethod="POST" 
				WcfServicePath="~/Services/TeacherWCF.svc" WcfServiceMethod="ClearStandardsList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
				objectToLoadID="subjectList">
				</telerik:RadXmlHttpPanel>
				<telerik:RadXmlHttpPanel runat="server" ID="submitXmlHttpPanel" ClientIDMode="Static" Value="" WcfRequestMethod="POST" 
				WcfServicePath="~/Services/AssessmentWCF.svc" WcfServiceMethod="RequestNewAssessmentID" RenderMode="Block" OnClientResponseEnding="goToNewAssessment"
				objectToLoadID="assessmentID">
				</telerik:RadXmlHttpPanel>
				<telerik:RadXmlHttpPanel runat="server" ID="submitQuickBuildXmlHttpPanel" ClientIDMode="Static" Value="" WcfRequestMethod="POST" 
				WcfServicePath="~/Services/AssessmentWCF.svc" WcfServiceMethod="StoreAssessmentIdentification" RenderMode="Block" OnClientResponseEnding="goToStandards"
				objectToLoadID="assessmentID">
				</telerik:RadXmlHttpPanel>
				</span>
		</form>

		<script type="text/javascript">
				var contextMenuBlurred = false;
				function requestSubjectFilter(sender, args) {
						var standardsCleared = eval(document.getElementById('standardsCleared').value);
						
						function callbackFunction(arg) {
								if (arg) {
										var senderElement = sender.get_element();
										var dropdownButton = $find(senderElement.getAttribute('dropdownButtonID'));
										var itemText = args.get_item().get_text();
										var teacherID = senderElement.getAttribute('teacherID');
										var panelID = senderElement.getAttribute('xmlHttpPanelID');
										var panel = $find(panelID);
										var childButton = $find(senderElement.getAttribute('childButtonID'));
										var defaultChildButtonText = senderElement.getAttribute('defaultChildButtonText');

										panel.set_value('{"TeacherID":"' + teacherID + '", "Grade":"' + itemText + '"}');

										dropdownButton.set_text(itemText);
										if (childButton && defaultChildButtonText) childButton.set_text(defaultChildButtonText);

										document.getElementById('standardsCleared').value = "true";
								}

								contextMenuBlurred = false;
						}

						if(standardsCleared) {
								var senderElement = sender.get_element();
								var dropdownButton = $find(senderElement.getAttribute('dropdownButtonID'));
								var itemText = args.get_item().get_text();
								var teacherID = senderElement.getAttribute('teacherID');
								var panelID = senderElement.getAttribute('xmlHttpPanelID');
								var panel = $find(panelID);
								var childButton = $find(senderElement.getAttribute('childButtonID'));
								var defaultChildButtonText = senderElement.getAttribute('defaultChildButtonText');

								panel.set_value('{"TeacherID":"' + teacherID + '", "Grade":"' + itemText + '"}');

								dropdownButton.set_text(itemText);
								if (childButton && defaultChildButtonText) childButton.set_text(defaultChildButtonText);
						}
						else if(!contextMenuBlurred) {
								var confirmDialogText = 'Updating either <b>grade, subject or course</b> discards any previous Standards selections made. Select OK to continue with the update action or CLOSE to return to the Wizard.';
								contextMenuBlurred = true;
								setTimeout(function () { parent.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
						}
				}

				function requestCourseFilter(sender, args) {
						var standardsCleared = eval(document.getElementById('standardsCleared').value);

						function callbackFunction(arg) {
								if (arg) {
										var senderElement = sender.get_element();
										var dropdownButton = $find(senderElement.getAttribute('dropdownButtonID'));
										var gradeList = $find('gradeList');
										var itemText = args.get_item().get_text();
										var teacherID = senderElement.getAttribute('teacherID');
										var grade = gradeList._selectedValue;
										var panelID = senderElement.getAttribute('xmlHttpPanelID');
										var panel = $find(panelID);
										var childButton = $find(senderElement.getAttribute('childButtonID'));
										var defaultChildButtonText = senderElement.getAttribute('defaultChildButtonText');

										panel.set_value('{"TeacherID":"' + teacherID + '", "Grade":"' + grade + '", "Subject":"' + itemText + '"}');

										dropdownButton.set_text(itemText);
										if (childButton && defaultChildButtonText) childButton.set_text(defaultChildButtonText);

										document.getElementById('standardsCleared').value = "true";
								}

								contextMenuBlurred = false;
						}

						if (standardsCleared) {
								var senderElement = sender.get_element();
								var dropdownButton = $find(senderElement.getAttribute('dropdownButtonID'));
								var gradeList = $find('gradeList');
								var itemText = args.get_item().get_text();
								var teacherID = senderElement.getAttribute('teacherID');
								var grade = gradeList._selectedValue;
								var panelID = senderElement.getAttribute('xmlHttpPanelID');
								var panel = $find(panelID);
								var childButton = $find(senderElement.getAttribute('childButtonID'));
								var defaultChildButtonText = senderElement.getAttribute('defaultChildButtonText');

								panel.set_value('{"TeacherID":"' + teacherID + '", "Grade":"' + grade + '", "Subject":"' + itemText + '"}');

								dropdownButton.set_text(itemText);
								if (childButton && defaultChildButtonText) childButton.set_text(defaultChildButtonText);
						}
						else if (!contextMenuBlurred) {
								var confirmDialogText = 'Updating either <b>grade, subject or course</b> discards any previous Standards selections made. Select OK to continue with the update action or CLOSE to return to the Wizard.';
								contextMenuBlurred = true;
								setTimeout(function () { parent.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
						}
				}
				
				function confirmClearStandards(sender, args) {
						var standardsCleared = eval(document.getElementById('standardsCleared').value);
						if (standardsCleared) return;
						function callbackFunction(arg) {
								if (arg) {
										var panel = $find('clearStandardsXmlHttpPanel');
										panel.set_value('{"TeacherID":"0"}');

										document.getElementById('standardsCleared').value = "true";
								}

								contextMenuBlurred = false;
						}

						if (!contextMenuBlurred) {
								var confirmDialogText = 'Updating either <b>grade, subject or course</b> discards any previous Standards selections made. Select OK to continue with the update action or CLOSE to return to the Wizard.';
								contextMenuBlurred = true;
								setTimeout(function () { parent.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
						}
				}

				function loadChildFilter(sender, args) {
						//load panel
						var senderElement = sender.get_element();
						var results = args.get_content();
						var contextMenuObjectID = senderElement.getAttribute('objectToLoadID');
						var contextMenuObject = $find(contextMenuObjectID);
						var dropdownButtonID = contextMenuObject._attributes.getAttribute('dropdownButtonID');
						var dropdownButton = $find(dropdownButtonID);
						var defaultChildButtonText = contextMenuObject._attributes.getAttribute('defaultChildButtonText');
						var gradeButton = $find('gradeButton');
						var grade = gradeButton.get_text();

						//Clear all context menu items
						clearAllContextMenuItems(contextMenuObject);

						/*Add each new context menu item
						results[i].Key(value) = full text
						results[i].Value(button display) = short text...
						*/
						for (var i = 0; i < results.length; i++) {
								addContextMenuItem(contextMenuObject, results[i].Key, results[i].Value);
						}

						if (results.length == 1) {
								contextMenuObject._selectedIndex = 0;
								dropdownButton.set_text(results[0].Value);
						}
						else if (dropdownButton && defaultChildButtonText) dropdownButton.set_text(defaultChildButtonText);
				}

				function addContextMenuItem(contextMenuObject, itemTextandValue, buttonText) {
						if (!contextMenuObject || !itemTextandValue || !buttonText) {
								return false;
						}

						/*indicates that client-side changes are going to be made and 
						these changes are supposed to be persisted after postback.*/
						contextMenuObject.trackChanges();

						//Instantiate a new client item
						var item = new Telerik.Web.UI.RadMenuItem();

						//Set its text and add the item
						item.set_text(itemTextandValue);
						item.get_attributes().setAttribute('buttonText', buttonText);
						contextMenuObject.get_items().add(item);

						//Set width
						var itemIndex = contextMenuObject.get_items().get_count() - 1;
						contextMenuObject.get_items().getItem(itemIndex).get_linkElement().style.width = '200px';

						//submit the changes to the server.
						contextMenuObject.commitChanges();
				}

				function clearAllContextMenuItems(contextMenuObject) {
						var allItems = contextMenuObject.get_allItems();
						if (allItems.length < 1) {
								return false;
						}

						/*indicates that client-side changes are going to be made and 
						these changes are supposed to be persisted after postback.*/
						contextMenuObject.trackChanges();

						//clear all items
						contextMenuObject.get_items().clear();

						//submit the changes to the server.
						contextMenuObject.commitChanges();

						return false;
				}

				function dropdownButtonClick(sender, args) {
						if (!sender.get_commandName()) {
								var senderElement = sender.get_element();
								var currentLocation = $telerik.getLocation(senderElement);
								var contextMenu = $find(senderElement.getAttribute('dropdownListID'));
								contextMenu.showAt(currentLocation.x, currentLocation.y + 22);
						}
				}

				function dropdownItemsClick(sender, args) {
						var senderElement = sender.get_element();
						var itemText;
						if (args.get_item()._attributes && args.get_item()._attributes.getAttribute('buttonText')) itemText = args.get_item()._attributes.getAttribute('buttonText');
						else itemText = args.get_item().get_text();
						var dropdownButton = $find(senderElement.getAttribute('dropdownButtonID'));
						var childButton = $find(senderElement.getAttribute('childButtonID'));
						var defaultChildButtonText = senderElement.getAttribute('defaultChildButtonText');

						dropdownButton.set_text(itemText);
						if (childButton && defaultChildButtonText) childButton.set_text(defaultChildButtonText);
				}

				function createAssessment(xmlHttpPanelID) {
						var gradeButtonText = $find('gradeButton').get_text(); if (gradeButtonText == '<Select One>') gradeButtonText = '';
						var subjectButtonText = $find('subjectButton').get_text(); if (subjectButtonText == '<Select One>') subjectButtonText = '';
						var courseButtonText = $find('courseButton').get_text(); if (courseButtonText == '<Select One>') courseButtonText = '';
						var typeButtonText = $find('typeButton').get_text(); if (typeButtonText == '<Select One>') typeButtonText = '';
						var termButtonText = $find('termButton').get_text(); if (termButtonText == '<Select One>') termButtonText = '';
						var contentButtonText = $find('contentButton').get_text();
						var descriptionText = document.getElementById('descriptionInput').value;
						var panel = $find(xmlHttpPanelID);
						var panelValue;

						if (gradeButtonText.length > 0 && subjectButtonText.length > 0 && courseButtonText.length > 0
						&& typeButtonText.length > 0 && termButtonText.length > 0 && contentButtonText.length > 0 && descriptionText.length > 0) {
								panelValue = '{"Grade":"' + gradeButtonText + '", "Subject":"' + subjectButtonText + '", "Course":"' + courseButtonText + '", ' +
								'"Type":"' + typeButtonText + '", "Term":"' + termButtonText + '", "Content":"' + contentButtonText + '", "Description":"' + descriptionText + '"}';

								panel.set_value(panelValue);
						}
						else {
								setTimeout(function () { var wnd = parent.radalert('Some selections were not made. Please check your criteria and try again.', null, null, 'Alert'); wnd.setSize(400, 150); }, 0);
						}
				}

				function goToNewAssessment(sender, args) {
						var result = args.get_content();
						var gradeList = $find('gradeList');
						var gradeList_gradeOrdinal = gradeList.get_items().getItem(gradeList._selectedItemIndex)._attributes.getAttribute('gradeOrdinal');
						var subjectButtonText = $find('subjectButton').get_text();
						var courseButtonText = $find('courseButton').get_text();
						var typeButtonText = $find('typeButton').get_text();
						var termButtonText = $find('termButton').get_text();
						var courseText = (courseButtonText == subjectButtonText ? '' : courseButtonText);
						var modalTitle = 'Term ' + termButtonText + ' ' + typeButtonText + ' - ' + gradeList_gradeOrdinal + ' Grade ' + subjectButtonText + courseText;

						parent.openDialogRadWindowUrl('../Record/AssessmentPage.aspx?xID=' + result, { title: modalTitle, maximize: true }, null, parent.assessmentDialogCloseCallback);
				}

				function goToStandards(sender, args) {
						parent.openDialogRadWindowUrl('../Dialogues/Assessment/AssessmentStandards.aspx?headerImg=' + document.getElementById('headerImg').getAttribute('headerImgName'), { title: 'Assessment Standards Selection', width: 800, height: 500, confirmClose: true, confirmCloseTitle: 'Cancel Assessment', confirmCloseText: 'Proceeding with <b>Cancel</b> discards any entries made and does not create an assessment. Select OK to continue with the Cancel action or CLOSE to return to the Wizard.', confirmCloseWidth: 350, confirmCloseHeight: 100 }, parent.assessmentDialogCloseCallback, parent.assessmentDialogCloseCallback);
				}
		</script>
</body>
</html>