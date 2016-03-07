<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Dialogues/Assessment/AssessmentDialog.Master" CodeBehind="CreateAssessmentIdentification.aspx.cs"
    Inherits="Thinkgate.Dialogues.Assessment.CreateAssessmentIdentification" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        body {
            font-family: Sans-Serif, Arial;
        }
        
        .containerDiv {
            width: 600px;
            height: 450px;
            margin: 10px;
        }
        
        .labels {
            font-size: 11pt;
            width: 100px;
            text-align: left;
            margin-right: 10px;
            position: relative;
            float: left;
        }
        
        .formElement {
            position: relative;
            float: left;
            margin-bottom: 30px !important;
            top: 0px;
            left: 0px;
        }
        
        input.formElement {
            width: 200px;
            padding: 3px;
            border: solid 1px #000;
        }
        
        .formContainer {
            width: 380px;
            text-align: center;
            margin-top: 60px;
        }
        
        .headerImg {
            position: absolute;
            left: 0;
            top: 0;
        }
        
        .roundButtons {
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            position: relative;
            border: solid 1px #000;
            border-radius: 50px;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
        }
        
        .contextMenuDiv {
            height: 490px;
            position: absolute;
            top: 0px;
            left: 0px;
        }

     
        .RadComboBox_Web20 .rcbInputCellLeft, .RadComboBox_Web20 .rcbInputCellRight, .RadComboBox_Web20 .rcbArrowCellLeft, .RadComboBox_Web20 .rcbArrowCellRight {
            /*background-image: url('../../Images/rcbSprite.png') !important; */           
        }
        
        .RadComboBox_Web20 .rcbHovered .rcbInputCell .rcbInput, .RadComboBox_Web20 .rcbFocused .rcbInputCell .rcbInput {
            color: #fff !important;            
        }
        
        .RadComboBox_Web20 .rcbMoreResults a {
            /*background-image: url('../../Images/rcbSprite.png') !important;*/
        }
        
        .hiddenUpdateButton {
            display: none;
        }

        span .rbText {
            margin-right: 47px;
        }
        .labels img{margin-top: -5px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div class="contextMenuDiv" oncontextmenu="return false;">
    </div>
    <img runat="server" id="headerImg" src="../../Images/repairtool.png" alt="Create assessment icon"
        class="headerImg" clientidmode="Static" />
    <div class="containerDiv" align="center">
        <div class="formContainer">
            <div id="divSecure" runat="server" visible="false">
            <div class="labels">
                    <img src="../../Images/IconSecure.png" height="26px" class="" alt="SecureIcon" />
                </div>
                <telerik:RadCodeBlock runat="server">
                    <telerik:RadAjaxPanel runat="server">

                        <telerik:RadButton ID="rbtnNO" runat="server" GroupName="PostingType" Skin="Web20" Width="" CssClass="formElement" ForeColor="Black" ClientIDMode="Static"
                            ToggleType="Radio" ButtonType="ToggleButton" Checked="true" Text="No" AutoPostBack="true" OnClick="rbtnNO_OnCheckedChanged">
                        </telerik:RadButton>
                        <telerik:RadButton ID="rbtnYES" runat="server" GroupName="PostingType" Skin="Web20" Width="" CssClass="formElement" ClientIDMode="Static"
                            ToggleType="Radio" ButtonType="ToggleButton" Text="Yes" AutoPostBack="true" OnClick="rbtnYES_OnCheckedChanged">
                        </telerik:RadButton>
                    </telerik:RadAjaxPanel>
                </telerik:RadCodeBlock>
            </div>
            <div class="labels">
                Grade:
            </div>
            <telerik:RadComboBox ID="gradeDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement"
                OnClientSelectedIndexChanged="requestSubjectFilter" teacherID="" level="" xmlHttpPanelID="subjectXmlHttpPanel" />
            <input type="hidden" runat="server" id="initGrade" clientidmode="Static" />
            <div class="labels">
                Subject:
            </div>
            <telerik:RadComboBox ID="subjectDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement"
                OnClientSelectedIndexChanged="requestCourseFilter" teacherID="" level="" xmlHttpPanelID="courseXmlHttpPanel" />
            <input type="hidden" runat="server" id="initSubject" clientidmode="Static" />
            <div class="labels">
                Course:
            </div>
            <telerik:RadComboBox ID="courseDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement"
                OnClientSelectedIndexChanged="confirmClearStandards" />
            <input type="hidden" runat="server" id="initCourse" clientidmode="Static" />
            <div class="labels">
                Type:
            </div>
            <telerik:RadCodeBlock runat="server">
                <telerik:RadAjaxPanel runat="server">

            <telerik:RadComboBox ID="typeDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement" />
                </telerik:RadAjaxPanel>
            </telerik:RadCodeBlock>
            <div class="labels">
                Term:
            </div>
            <telerik:RadComboBox ID="termDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement">
            </telerik:RadComboBox>
            <div class="labels" id="contentLabel" runat="server">
                Content:
            </div>
            <telerik:RadComboBox ID="contentDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement" OnClientSelectedIndexChanged="displayExtraFilters">
                <Items>
                    <telerik:RadComboBoxItem Text="Item Bank" Value="Item Bank" Selected="true" />
                    <telerik:RadComboBoxItem Text="External" Value="External" />
                    <telerik:RadComboBoxItem Text="No Items/Content" Value="No Items/Content" />
                </Items>
            </telerik:RadComboBox>
            <span id="noItemsContentFormSpan" style="display: none;">
                <div class="labels">
                    Score Type:
                </div>
                <telerik:RadComboBox ID="scoreTypeDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                    EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement">
                    <Items>
                        <telerik:RadComboBoxItem Text="Percent" Value="P" />
                        <telerik:RadComboBoxItem Text="Pass/Fail" Value="F" />
                        <telerik:RadComboBoxItem Text="Yes/No" Value="Y" />
                    </Items>
                </telerik:RadComboBox>
                <div class="labels">
                    Performance Level Set:
                </div>
                <telerik:RadComboBox ID="performanceLevelSetsDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                    EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement">
                </telerik:RadComboBox>
                <div class="labels">Keywords:</div>
                <input runat="server" type="text" id="keywordsInput" clientidmode="Static" name="keywordsInput"
                    class="formElement" />
            </span>
            <span id="assessmentSelectionFormSpan" runat="server" clientidmode="Static" style="display: none;">
                <div class="labels">Item Selection Method:</div>
                <telerik:RadComboBox ID="assessmentSelectionDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                    EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement">
                    <Items>
                        <telerik:RadComboBoxItem Text="Blank Item" Value="Blank"/>
                        <telerik:RadComboBoxItem Text="Item Type and Rigor" Value="Rigor"/>
                        <telerik:RadComboBoxItem Text="Addendums" Value="Addendum" />
                    </Items>
                </telerik:RadComboBox>
            </span>
            <div class="labels">Description:</div>
            <input runat="server" type="text" id="descriptionInput" clientidmode="Static" name="descriptionInput"
                class="formElement" />

             <%--<div class="labels" id="divYear">--%>
            <div class="labels">
                Year:
             <%--<label id="lblYear" runat="server"> Year:</label>--%>
            </div>
            <telerik:RadComboBox ID="yearDropDown" Skin="Web20" ClientIDMode="Static" runat="server"
                AutoPostBack="false" Width="200" CssClass="formElement" />
            <div style="float: left">
            <asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="setTimeout(function() { modalWin.close(); }, 0); return false;" />
            <asp:Button runat="server" ID="backButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Back&nbsp;&nbsp;" OnClientClick="openAddAssessmentWindow('../Dialogues/Assessment/CreateAssessmentOptions.aspx'); return false;" />
            <asp:Button runat="server" ID="nextButton" ClientIDMode="Static" CssClass="roundButtons" CountAddendums="0"
                Text="&nbsp;&nbsp;Continue&nbsp;&nbsp;" OnClientClick="openStandardSelectionCMO(this); return false;" />
            </div>
            <div class="hiddenUpdateButton">
                <telerik:RadButton runat="server" ID="updateButtonTrigger" ClientIDMode="Static" Text="" OnClick="UpdateIdentification_Click" />
            </div>
            <input runat="server" type="hidden" id="assessmentID" name="assessmentID" value="" clientidmode="Static" />
            <input runat="server" type="hidden" id="assessmentID_encrypted" name="assessmentID_encrypted" value="" clientidmode="Static" />
            <input runat="server" type="hidden" id="standardsCleared" name="standardsCleared"
                value="true" clientidmode="Static" />
            <input runat="server" type="hidden" id="assessmentItemsCount" name="assessmentItemsCount"
                value="" clientidmode="Static" />
            <input runat="server" clientidmode="Static" id="senderPage" name="senderPage" type="hidden" value="" />
            <input runat="server" clientidmode="Static" id="copy" name="copy" type="hidden" value="" />
            <input runat="server" clientidmode="Static" id="createAssessmentClassCount" name="createAssessmentClassCount" type="hidden" value="0" />
            <input runat="server" clientidmode="Static" id="isTeacher" name="isTeacher" type="hidden" value="no" />
            <input runat="server" clientidmode="Static" id="classID" name="classID" type="hidden" value="0" />
            <input runat="server" clientidmode="Static" id="selectedTestCategory" name="selectedTestCategory" type="hidden" value="0" />
            <input runat="server" clientidmode="Static" id="OTCNavigationRestricted" name="OTCNavigationRestricted" type="hidden" value="" />
        </div>
    </div>
    <telerik:RadXmlHttpPanel runat="server" ID="subjectXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
        WcfServiceMethod="RequestSubjectList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
        objectToLoadID="subjectDropdown">
    </telerik:RadXmlHttpPanel>
    <telerik:RadXmlHttpPanel runat="server" ID="courseXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
        WcfServiceMethod="RequestCourseList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
        objectToLoadID="courseDropdown">
    </telerik:RadXmlHttpPanel>
    <span style="display: none;">
        <telerik:RadXmlHttpPanel runat="server" ID="clearStandardsXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="ClearStandardsList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
            objectToLoadID="subjectDropdown">
        </telerik:RadXmlHttpPanel>
        <telerik:RadXmlHttpPanel runat="server" ID="submitXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="RequestNewAssessmentID" RenderMode="Block" OnClientResponseEnding="goToNewAssessment"
            objectToLoadID="assessmentID">
            </telerik:RadXmlHttpPanel>
        <telerik:RadXmlHttpPanel runat="server" ID="submitQuickBuildXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="StoreAssessmentIdentification" RenderMode="Block" OnClientResponseEnding="goToStandards"
            objectToLoadID="assessmentID">
        </telerik:RadXmlHttpPanel>
        <telerik:RadXmlHttpPanel runat="server" ID="updateXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="UpdateIdentification" RenderMode="Block" OnClientResponseEnding="closeAndRefresh"
            objectToLoadID="assessmentID">
        </telerik:RadXmlHttpPanel>
        <telerik:RadXmlHttpPanel runat="server" ID="copyXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="CopyAssessment" RenderMode="Block" OnClientResponseEnding="goToNewAssessment"
            objectToLoadID="assessmentID">
        </telerik:RadXmlHttpPanel>
    </span>
    <input type="hidden" runat="server" id="isProofed" value="No" />
    <input type="hidden" runat="server" id="nextButtonInitialText" clientidmode="Static" />
    <script type="text/javascript">
        var dropdownObjectBlurred = false;
        var modalWin = getCurrentCustomDialog();
        var prepWindow = true;

        prepModalWindow();
        function prepModalWindow() {
            if (modalWin && modalWin.isVisible()) {
                //if (modalWin && modalWin.get_height() != 600) modalWin.set_height(600);

                modalWin.remove_beforeClose(modalWin.confirmBeforeClose);
                if ($('#<%=isProofed.ClientID %>').attr('value') == 'Yes') return;
                
                switch ($('#senderPage').attr('value')) {
                    case 'config':
                        modalWin.addConfirmDialog({
                            title: 'Cancel Identification Update',
                            text: 'Proceeding with <b>Cancel</b> discards any entries made and does not update the assessment. Select OK to continue with the Cancel action or Cancel to return to the current window.',
                            callback: function () {
                                var win = parent.$find('RadWindow1Url');
                                var adjustedHeight = Math.ceil($telerik.$(parent.window).height() * 96 / 100);
                                var winHeight = adjustedHeight < 675 ? adjustedHeight : 675; //to compensate for clients with very low resolution
                                win.SetUrl('../Dialogues/Assessment/AssessmentConfiguration.aspx?xID=' + $('#assessmentID_encrypted').attr('value'));
                                win.SetSize(800, winHeight);
                                win.Center();
                            }
                        }, window);
                        break;
                    case 'identification':
                        modalWin.addConfirmDialog({
                            title: 'Cancel Identification Update',
                            text: 'Proceeding with <b>Cancel</b> discards any entries made and does not update the assessment. Select OK to continue with the Cancel action or Cancel to return to the current window.'
                        }, window);
                        break;
                    case 'new':
                        modalWin.addConfirmDialog({
                            title: 'Cancel Identification Update',
                            text: 'Proceeding with <b>Cancel</b> discards any entries made and does not create an assessment. Select OK to continue with the Cancel action or Cancel to return to the current window.',
                            callback: parent.createAssessmentOptionsCallback
                        }, window);
                        break;
                    default:
                        break;
                }
            }
        }

        $(document).ready(function () {
            var headerImg = '<%= Request.QueryString["headerImg"] %>';
            if (headerImg == 'repairtool') {
                $('#assessmentSelectionFormSpan').css('display', '');
            }
            else {
                $('#assessmentSelectionFormSpan').css('display', 'none');
            }
        });

        function openAddAssessmentWindow(url) {
            parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: url,
                title: 'Options to Create Assessment',
                maximize: true, maxwidth: 550, maxheight: 450
            });
        }

        function updateIdentification() {
            var assessmentID = document.getElementById('assessmentID').value;
            var assessmentID_encrypted = document.getElementById('assessmentID_encrypted').value;
            var gradeDropdownText = $find('gradeDropdown').get_value(); if (gradeDropdownText == '') gradeDropdownText = '';
            var subjectDropdownText = $find('subjectDropdown').get_value(); if (subjectDropdownText == '') subjectDropdownText = '';
            var courseDropdownText = $find('courseDropdown').get_value(); if (courseDropdownText == '') courseDropdownText = '';
            var typeDropdownText = $find('typeDropdown').get_text(); if (typeDropdownText == '<Select One>') typeDropdownText = '';
            var termDropdownText = $find('termDropdown').get_text(); if (termDropdownText == '<Select One>') termDropdownText = '';
            var descriptionText = document.getElementById('descriptionInput').value;
            var yearText = $find('yearDropDown').get_value();
            var AllowedOTCNavigation = document.getElementById('OTCNavigationRestricted').value;
            var panel = $find('updateXmlHttpPanel');
            
            if (courseDropdownText == "")
                courseDropdownText = "\"";

            if (gradeDropdownText.length > 0 && subjectDropdownText.length > 0 && !isNaN(courseDropdownText)
            && typeDropdownText.length > 0 && termDropdownText.length > 0 && descriptionText.replace(/ /g, '').length > 0) {
                //$find('updateButtonTrigger').click();
                //setTimeout(function () { parent.getCurrentCustomDialogByName('RadWindowAddAssessment').close(); }, 0);
                panel.set_value('{"AssessmentID":"' + assessmentID + '", "EncryptedAssessmentID":"' + assessmentID_encrypted + '", "Grade":"' + gradeDropdownText + '", "Subject":"' + subjectDropdownText + '", "CourseID":"' + courseDropdownText + '",' +
                ' "Type":"' + typeDropdownText + '", "Term":' + termDropdownText + ', "Description":"' + descriptionText + '","Year":"' + yearText + '","AllowedOTCNavigation":"' + AllowedOTCNavigation + '"}');
            }
            else {
                var wnd = window.radalert('Some selections were not made. Please check your criteria and try again.', 400, 150, 'Alert');
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            }
        }

        function closeAndRefresh(sender, args) {
            if (parent.$find('assessmentIdentificationRefreshTrigger')) parent.$find('assessmentIdentificationRefreshTrigger').click();
            //modalWin.remove_beforeClose(modalWin.confirmBeforeClose);
            //setTimeout(function() { modalWin.close(); }, 0);
            if (window.location.search.indexOf('fromConfig') > -1) {
                var adjustedHeight = Math.ceil($telerik.$(parent.window).height() * 96 / 100);
                var winHeight = adjustedHeight < 675 ? adjustedHeight : 675; //to compensate for clients with very low resolution
                modalWin.SetUrl('../Dialogues/Assessment/AssessmentConfiguration.aspx?xID=' + $('#assessmentID_encrypted').attr('value'));
                modalWin.SetSize(800, winHeight);
                modalWin.Center();
            }
            else {
                modalWin.remove_beforeClose(modalWin.confirmBeforeClose);
                setTimeout(function () { modalWin.close(); }, 0);
            }
        }

        function requestSubjectFilter(sender, args) {
            var standardsCleared = eval(document.getElementById('standardsCleared').value);
            var assessmentItemsCount = document.getElementById('assessmentItemsCount').value;

            function callbackFunction(arg) {
                var senderElement = sender.get_element();
                if (arg) {
                    var itemText = args.get_item().get_text();
                    var teacherID = senderElement.getAttribute('teacherID');
                    var level = senderElement.getAttribute('level');
                    var panelID = senderElement.getAttribute('xmlHttpPanelID');
                    var panel = $find(panelID);

                    panel.set_value('{"TeacherID":"' + teacherID + '", "Level":"' + level + '", "Grade":"' + itemText + '"}');

                    document.getElementById('standardsCleared').value = "true";
                }
                else {
                    sender.get_items().getItem(document.getElementById('initGrade').value).select();
                }

                dropdownObjectBlurred = false;
            }

            if (standardsCleared && assessmentItemsCount.length == 0) {
                var senderElement = sender.get_element();
                var itemText = args.get_item().get_text();
                var teacherID = senderElement.getAttribute('teacherID');
                var level = senderElement.getAttribute('level');
                var panelID = senderElement.getAttribute('xmlHttpPanelID');
                var panel = $find(panelID);

                panel.set_value('{"TeacherID":"' + teacherID + '", "Level":"' + level + '", "Grade":"' + itemText + '"}');
            }
            else if (!dropdownObjectBlurred && assessmentItemsCount.length > 0) {
                var confirmDialogText = 'The <b>grade, subject or course</b> has been changed for this assessment. Please verify if the existing items are still valid based on the updated Assessment Identification.';
                dropdownObjectBlurred = true;
                setTimeout(function () { window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
            }
            else if (!dropdownObjectBlurred) {
                var confirmDialogText = 'Updating either <b>grade, subject or course</b> discards any previous Standards selections made. Select OK to continue with the update action or Cancel to return to the previous window.';
                dropdownObjectBlurred = true;
                setTimeout(function () { window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
            }
        }

        function requestCourseFilter(sender, args) {
            var standardsCleared = eval(document.getElementById('standardsCleared').value);
            var assessmentItemsCount = document.getElementById('assessmentItemsCount').value;

            function callbackFunction(arg) {
                var senderElement = sender.get_element();
                if (arg) {
                    var gradeDropdown = $find('gradeDropdown');
                    var itemText = args.get_item().get_text();
                    var teacherID = senderElement.getAttribute('teacherID');
                    var level = senderElement.getAttribute('level');
                    var grade = gradeDropdown.get_selectedItem().get_value();
                    var panelID = senderElement.getAttribute('xmlHttpPanelID');
                    var panel = $find(panelID);

                    panel.set_value('{"TeacherID":"' + teacherID + '", "Level":"' + level + '", "Grade":"' + grade + '", "Subject":"' + itemText + '"}');

                    document.getElementById('standardsCleared').value = "true";
                }
                else {
                    sender.get_items().getItem(document.getElementById('initSubject').value).select();
                }

                dropdownObjectBlurred = false;
            }

            if (standardsCleared && assessmentItemsCount.length == 0) {
                var senderElement = sender.get_element();
                var gradeDropdown = $find('gradeDropdown');
                var itemText = args.get_item().get_text();
                var teacherID = senderElement.getAttribute('teacherID');
                var level = senderElement.getAttribute('level');
                var grade = gradeDropdown.get_selectedItem().get_value();
                var panelID = senderElement.getAttribute('xmlHttpPanelID');
                var panel = $find(panelID);

                panel.set_value('{"TeacherID":"' + teacherID + '", "Level":"' + level + '", "Grade":"' + grade + '", "Subject":"' + itemText + '"}');
            }
            else if (!dropdownObjectBlurred && assessmentItemsCount.length > 0) {
                var confirmDialogText = 'The <b>grade, subject or course</b> has been changed for this assessment. Please verify if the existing items are still valid based on the updated Assessment Identification.';
                dropdownObjectBlurred = true;
                setTimeout(function () { window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
            }
            else if (!dropdownObjectBlurred) {
                var confirmDialogText = 'Updating either <b>grade, subject or course</b> discards any previous Standards selections made. Select OK to continue with the update action or Cancel to return to the previous window.';
                dropdownObjectBlurred = true;
                setTimeout(function () { window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
            }
        }

        function confirmClearStandards(sender, args) {
            var standardsCleared = eval(document.getElementById('standardsCleared').value);
            var assessmentItemsCount = document.getElementById('assessmentItemsCount').value;

            if (standardsCleared) return;
            function callbackFunction(arg) {
                var senderElement = sender.get_element();
                if (arg) {
                    var panel = $find('clearStandardsXmlHttpPanel');
                    panel.set_value('{"TeacherID":"0"}');

                    document.getElementById('standardsCleared').value = "true";
                }
                else {
                    sender.get_items().getItem(document.getElementById('initCourse').value).select();
                }

                dropdownObjectBlurred = false;
            }

            if (!dropdownObjectBlurred && assessmentItemsCount.length > 0) {
                var confirmDialogText = 'The <b>grade, subject or course</b> has been changed for this assessment. Please verify if the existing items are still valid based on the updated Assessment Identification.';
                dropdownObjectBlurred = true;
                setTimeout(function () { window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
            }
            if (!dropdownObjectBlurred) {
                var confirmDialogText = 'Updating either <b>grade, subject or course</b> discards any previous Standards selections made. Select OK to continue with the update action or Cancel to return to the previous window.';
                dropdownObjectBlurred = true;
                setTimeout(function () { window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
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
            results[i].Key(value) = full text
            results[i].Value = ellipsed text
            */
            for (var i = 0; i < results.length; i++) {
                addDropdownItem(dropdownObject, results[i].Key, results[i].Value);
            }

            if (results.length == 1) {
                dropdownObject.get_items().getItem(0).select();
            }
            else if (dropdownObject._emptyMessage) {
                dropdownObject.clearSelection();
            }
        }

        function addDropdownItem(dropdownObject, itemValue, itemText) {
            if (!dropdownObject || !itemText || !itemValue) {
                return false;
            }

            /*indicates that client-side changes are going to be made and 
            these changes are supposed to be persisted after postback.*/
            dropdownObject.trackChanges();

            //Instantiate a new client item
            var item = new Telerik.Web.UI.RadComboBoxItem();

            //Set its text and add the item
            item.set_text(itemText);
            item.set_value(itemValue);
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

        function createAssessment(xmlHttpPanelID) {
            var gradeDropdownText = $find('gradeDropdown').get_value();
            var subjectDropdownText = $find('subjectDropdown').get_value(); 
            var courseDropdownText = $find('courseDropdown').get_value();
            var typeDropdownText = $find('typeDropdown').get_text(); if (typeDropdownText == '<Select One>') typeDropdownText = '';
            var termDropdownText = $find('termDropdown').get_text(); if (termDropdownText == '<Select One>') termDropdownText = '';
            var contentDropdownText = $find('contentDropdown').get_text();
            var assessmentSelectionText = $find('assessmentSelectionDropdown').get_value(); if (assessmentSelectionText == '<Select One>') assessmentSelectionText = '';
            var descriptionText = document.getElementById('descriptionInput').value;
            var scoreTypeDropdownText = $find('scoreTypeDropdown').get_value(); if (scoreTypeDropdownText == '<Select One>') scoreTypeDropdownText = '';
            var performanceLevelSetsDropdownText = $find('performanceLevelSetsDropdown').get_text(); if (performanceLevelSetsDropdownText == '<Select One>') performanceLevelSetsDropdownText = '';
            var keywordsText = document.getElementById('keywordsInput').value;
            var panel = $find(contentDropdownText == 'No Items/Content' ? 'submitXmlHttpPanel' : xmlHttpPanelID);
            var yearText = $find('yearDropDown').get_text();
            var panelValue;
            var headerImg = '<%= Request.QueryString["headerImg"] %>';
            //If the required selections for an online or offline test were made, then create assessment, otherwise alert user to complete form.
            if (gradeDropdownText.length > 0 && subjectDropdownText.length > 0 && !isNaN(courseDropdownText) && courseDropdownText > 0
                && typeDropdownText.length > 0 && termDropdownText.length > 0 && contentDropdownText.length > 0
                && descriptionText.replace(/ /g, '').length > 0
                && ((contentDropdownText == 'No Items/Content' && scoreTypeDropdownText.length > 0 && performanceLevelSetsDropdownText.length > 0)
                    || (contentDropdownText != 'No Items/Content')) && ((contentDropdownText == 'Item Bank' && assessmentSelectionText.length > 0)
                    || (contentDropdownText != 'Item Bank') || (headerImg != 'repairtool'))) {

                panelValue = '{"Grade":"' + gradeDropdownText + '", "Subject":"' + subjectDropdownText + '", "CourseID":"' + courseDropdownText + '", ' +
                '"Type":"' + typeDropdownText + '", "Term":"' + termDropdownText + '", "Content":"' + contentDropdownText + '", "Description":"' + descriptionText + '","Year":"' + yearText +
                (contentDropdownText == 'No Items/Content' ? '", "ScoreType":"' + (scoreTypeDropdownText == '' ? 'P' : scoreTypeDropdownText) + '", "PerformanceLevelSet":"' + performanceLevelSetsDropdownText + '", "Keywords":"' + keywordsText : '') +
                (headerImg == 'repairtool' ? (contentDropdownText == 'Item Bank' ? '", "AssessmentSelection":"' + assessmentSelectionText : '') : '') +
                '"}';

                    panel.set_value(panelValue);
            }
            else {
                var wnd = window.radalert('Some selections were not made. Please check your criteria and try again.', 400, 150, 'Alert');
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            }
        }

        function goToNewAssessmentCMO(assessmentID, modalTitle) {


            parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: '../Record/AssessmentPage.aspx?xID=' + assessmentID + "&encrypted=true",
                title: modalTitle,
                maximize: true
            });
        }

        function goToNewAssessment(sender, args) {
            var result = args.get_content();
            var contentDropdownText = $find('contentDropdown').get_text();

            if (contentDropdownText == 'No Items/Content') {
                $('#assessmentID').attr('value', result);                                
                if ($('#isTeacher').attr('value') == 'yes' || ($('#isTeacher').attr('value') == 'no' && $('#selectedTestCategory').attr('value') == 'District')) {
                    customDialog({ title: "Confirmation", maximize: true, maxwidth: 300, maxheight: 75, autoSize: false, content: "The assessment has been created.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Close", callback: noItemsCloseCallback }, { title: "Input Scores", callback: inputScoresCallback }]);
                }
                else {
                    customDialog({ title: "Confirmation", maximize: true, maxwidth: 300, maxheight: 75, autoSize: false, content: "The assessment has been created.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Close", callback: noItemsCloseCallback }]);
                }
            }
            else {

                switch ($('#senderPage').attr('value')) {
                    case 'content':
                    case 'portal':
                        var parentURL = parent.window.location.href;
                        var assessmentURL = parentURL.substring(0, parentURL.indexOf('xID=') + 4) + result;
                        parent.window.location.href = assessmentURL;
                        break;
                    default:
                        var gradeDropdown = $find('gradeDropdown');
                        var gradeDropdown_gradeOrdinal = gradeDropdown.get_items().getItem(gradeDropdown._selectedIndex)._attributes.getAttribute('gradeOrdinal');
                        var subjectDropdownText = $find('subjectDropdown').get_text();
                        var courseDropdownText = $find('courseDropdown').get_text();
                        var typeDropdownText = $find('typeDropdown').get_text();
                        var termDropdownText = $find('termDropdown').get_text();
                        var courseText = (courseDropdownText == subjectDropdownText ? '' : courseDropdownText);
                        var modalTitle = 'Term ' + termDropdownText + ' ' + typeDropdownText + ' - ' + gradeDropdown_gradeOrdinal + ' Grade ' + subjectDropdownText + courseText;

                        parent.customDialog({
                            name: 'RadWindowAddAssessment',
                            url: '../Record/AssessmentPage.aspx?xID=' + result + '&encrypted=true',
                            title: modalTitle,
                            maximize: true
                        });
                        break;
                }
            }
        }

        function inputScoresCallback() {            
            var scoreTypeDropdownText = $find('scoreTypeDropdown')._value; if (scoreTypeDropdownText == '<Select One>') scoreTypeDropdownText = '';
            if ($('#isTeacher').attr('value') == 'no' ||
                (parseInt($('#classID').attr('value')) == 0)) {
                var gradeDropdown = $find('gradeDropdown');
                var gradeDropdown_gradeOrdinal = gradeDropdown.get_items().getItem(gradeDropdown._selectedIndex)._attributes.getAttribute('gradeOrdinal');
                var subjectDropdownText = $find('subjectDropdown').get_text();
                var courseDropdownText = $find('courseDropdown').get_text();
                var typeDropdownText = $find('typeDropdown').get_text();
                var termDropdownText = $find('termDropdown').get_text();
                var courseText = (courseDropdownText == subjectDropdownText ? '' : courseDropdownText);
                var modalTitle = 'Assessment Assignments: ' + gradeDropdown_gradeOrdinal + subjectDropdownText + courseText + termDropdownText + '-' + typeDropdownText + ' - External Test';
                var testCategory = $('#selectedTestCategory').attr('value');

                var win = parent.customDialog({
                    name: 'RadWindowAssessmentAssignmentSearch',
                    //url: '../Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?assessmentID=' + $('#assessmentID').attr('value') + '&encrypted=false&contentType=noItemContent&scoreType=' + scoreTypeDropdownText + '&testcategory=' + testCategory,
                    url: '../Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?assessmentID=' + $('#assessmentID').attr('value') + '&encrypted=false&contentType=noItemContent&scoreType=' + scoreTypeDropdownText + '&testcategory=' + testCategory,
                    title: modalTitle,
                    maximize: true, maxwidth: 950, maxheight: 675
                });

                modalWin.remove_beforeClose(modalWin.confirmBeforeClose);
                setTimeout(function () { modalWin.close(); }, 0);
            }
            else {
                var win = parent.customDialog({
                    name: 'RadWindowAssessmentInputScores',
                    url: '../Dialogues/Assessment/AssessmentOfflineScores.aspx?assessmentID=' + $('#assessmentID').attr('value') + '&classID=' + $('#classID').attr('value') + '&scoreType=' + scoreTypeDropdownText,
                title: 'Offline Test - 3rd Party Assessment on Fractions',
                maximize: true, maxwidth: 600, maxheight: 520
                });

                modalWin.remove_beforeClose(modalWin.confirmBeforeClose);
                setTimeout(function () { modalWin.close(); }, 0);
            }
        }

        function noItemsCloseCallback() {
            modalWin.remove_beforeClose(modalWin.confirmBeforeClose);
            setTimeout(function () { modalWin.close(); }, 0);
        }

        function goToStandards(sender, args) {
            var isSecure = false;
            var allMax = true;
            if (navigator.appVersion.indexOf("Win") == -1) {
                allMax = false;
            }
            var button = $find("<%= rbtnYES.ClientID%>");

            if (button != null) {
                isSecure = button.get_checked();
            }
            var title = "Assessment Standards Selection";

            if (isSecure == true) {
                //title = "[SECURE] Assessment Standards Selection";
                title = SetSecureAssessmentTitle(title);
            }
            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: '../Dialogues/Assessment/AssessmentStandards.aspx?headerImg=<%= Request.QueryString["headerImg"] %>' + '&isSecure=' + isSecure,
                title: title,
                width: 1200,
                height: 550,
                maximizable: allMax,
                resizable: false,
                movable: true,
                maximize_height: !allMax,
                maximize_width: !allMax,
                center: true
            });

            win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
        }

        function copyAssessment() {
            var assessmentID = document.getElementById('assessmentID').value;
            var gradeDropdownText = $find('gradeDropdown').get_value(); if (gradeDropdownText == '') gradeDropdownText = '';
            var subjectDropdownText = $find('subjectDropdown').get_value(); if (subjectDropdownText == '') subjectDropdownText = '';

            // the course dropdown is "special", leave it as get_text()
            var courseDropdownText = $find('courseDropdown').get_text(); if (courseDropdownText == '<Select One>') courseDropdownText = '';

            var typeDropdownText = $find('typeDropdown').get_text(); if (typeDropdownText == '<Select One>') typeDropdownText = '';
            var termDropdownText = $find('termDropdown').get_text(); if (termDropdownText == '<Select One>') termDropdownText = '';
            var descriptionText = document.getElementById('descriptionInput').value;
            var AllowedOTCNavigation = document.getElementById('OTCNavigationRestricted').value;
            var yearText = $find('yearDropDown').get_text();
            
            var panel = $find('copyXmlHttpPanel');

            if (gradeDropdownText.length > 0 && subjectDropdownText.length > 0 && courseDropdownText.length > 0
								&& typeDropdownText.length > 0 && termDropdownText.length > 0 && descriptionText.replace(/ /g, '').length > 0 && yearText.length > 0) {
           		panel.set_value('{"AssessmentID":"' + assessmentID + '", "Grade":"' + gradeDropdownText + '", "Subject":"' + subjectDropdownText + '", "Course":"' + courseDropdownText + '",' +
                ' "Type":"' + typeDropdownText + '", "Term":' + termDropdownText + ', "Description":"' + descriptionText + '","Year":"' + yearText + '","AllowedOTCNavigation":"' + AllowedOTCNavigation + '"}');
            }
            else {
                var wnd = window.radalert('Some selections were not made. Please check your criteria and try again.', 400, 150, 'Alert');
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            }
        }

        function displayExtraFilters(sender, args) {
            var headerImg = '<%= Request.QueryString["headerImg"] %>';
            if (sender._value == 'Item Bank' && headerImg == 'repairtool') {
                $('#assessmentSelectionFormSpan').css('display', '');
            }
            else {
                $('#assessmentSelectionFormSpan').css('display', 'none');
            }

            if (sender._value == 'No Items/Content') {
                $('#noItemsContentFormSpan').css('display', '');
                $('#nextButton').attr('value', '  Done  ');
                return;
            }
            else {
                $('#noItemsContentFormSpan').css('display', 'none');
                $('#nextButton').attr('value', $('#nextButtonInitialText').attr('value'));
            }
        }

        var defaultTestType;
        $(document).ready(function () {
            var radButton = $find('typeDropdown');
            defaultTestType = radButton.get_value();
        });
        function updateSecureType() {
           <%-- var button = $find("<%= rbtnYES.ClientID%>");
            var isSecure = button.get_checked();
            if (isSecure == true) {
                var radButton = $find('typeDropdown');
                //radButton.set_text('Secure');
                //radButton.disabled();
                
            } else {

                var radButton = $find('typeDropdown');
                radButton.enable();
                radButton.clearSelection();
                //var emptyText = radButton.get_emptyMessage();
                radButton.set_text(defaultTestType);
                LoadTypeButtonFilter();
            }--%>
        }

        function openStandardSelectionCMO(button) {
            var headerImg = '<%= Request.QueryString["headerImg"] %>';
            var contentDropdownText = $find('contentDropdown').get_text();
            var assessmentSelectionText = $find('assessmentSelectionDropdown').get_value(); if (assessmentSelectionText == '<Select One>') assessmentSelectionText = '';

            if (headerImg == "repairtool" && contentDropdownText == "Item Bank" && (assessmentSelectionText == "Rigor" || assessmentSelectionText == "Addendum")) {
                createAssessment('submitQuickBuildXmlHttpPanel');
            }
            else {
                createAssessment('submitXmlHttpPanel');
            }
        }
    </script>
</asp:Content>
