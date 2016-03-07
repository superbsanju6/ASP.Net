<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StarReport.aspx.cs" MasterPageFile="~/Dialogues/Assessment/AssessmentDialog.Master" Inherits="Thinkgate.Controls.Reports.StarReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        body {
            font-family: Sans-Serif, Arial;
        }
        
        .containerDiv {
            width: 500px;
            height: 100px;
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
    <div class="containerDiv" align="center">
        <div class="formContainer">
            <div class="labels">
                School:<span style="color: red;">*</span>
            </div>
             <telerik:RadComboBox ID="schoolDropDown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement"
                OnClientSelectedIndexChanged="requestSchoolFilter" teacherID="" level="" xmlHttpPanelID="schoolXmlHttpPanel" />
            <input type="hidden" runat="server" id="initSchool" clientidmode="Static" />
            <div class="labels">
                Grade:<span style="color: red;">*</span>
            </div>
            <telerik:RadComboBox ID="gradeDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement"
                OnClientSelectedIndexChanged="requestSubjectFilter" teacherID="" level="" xmlHttpPanelID="subjectXmlHttpPanel" />
            <input type="hidden" runat="server" id="initGrade" clientidmode="Static" />
            <div class="labels">
                Subject:<span style="color: red;">*</span>
            </div>
            <telerik:RadComboBox ID="subjectDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement"
                OnClientSelectedIndexChanged="requestCourseFilter" teacherID="" level="" xmlHttpPanelID="courseXmlHttpPanel" />
            <input type="hidden" runat="server" id="initSubject" clientidmode="Static" />
            <div class="labels">
                Course:<span style="color: red;">*</span>
            </div>
            <telerik:RadComboBox ID="courseDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement"/>
            <input type="hidden" runat="server" id="initCourse" clientidmode="Static" />
            <asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="setTimeout(function() { modalWin.close(); }, 0); return false;" />
            <asp:Button runat="server" ID="nextButton" ClientIDMode="Static" CssClass="roundButtons" CountAddendums="0"
                Text="&nbsp;&nbsp;Generate&nbsp;Report&nbsp;&nbsp;" OnClientClick="GenerateReport(); return false;" />
            </div>
        </div>
    <telerik:RadXmlHttpPanel runat="server" ID="schoolXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
        WcfServiceMethod="RequestGradeList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
        objectToLoadID="gradeDropdown">
    </telerik:RadXmlHttpPanel>
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
    <%--<span style="display: none;">
        <telerik:RadXmlHttpPanel runat="server" ID="clearStandardsXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="ClearStandardsList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
            objectToLoadID="subjectDropdown">
        </telerik:RadXmlHttpPanel>
    </span>--%>
    <input type="hidden" runat="server" id="isProofed" value="No" />
    <input type="hidden" runat="server" id="nextButtonInitialText" clientidmode="Static" />
    <script type="text/javascript">
        var dropdownObjectBlurred = false;
        var modalWin = getCurrentCustomDialog();
        var prepWindow = true;
        
        function GenerateReport() {
            var school = $find('schoolDropDown').get_value();
            var grade = $find('gradeDropdown').get_value();
            var subject = $find('subjectDropdown').get_value();
            var course = $find('courseDropdown').get_text();

            if (school == '' || grade == '' || subject == '' || course == '' || course == '<Select One>') {
                alert("Please select all required criteria!");
                return false;
            }
            window.open("../Reports/StarReportGeneratePDF.aspx?Grade=" + grade + "&Subject=" + subject + "&Course=" + course + "&SchoolId=" + school, "_blank");
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

        function requestSchoolFilter(sender, args) {
            function callbackFunction(arg) {
                var senderElement = sender.get_element();
                if (arg) {
                    var itemText = args.get_item().get_value();
                    var panelID = senderElement.getAttribute('xmlHttpPanelID');
                    var panel = $find(panelID);
                    panel.set_value('{"SchoolID":"' + itemText + '"}');
                }
                else {
                    sender.get_items().getItem(document.getElementById('initSchool').value).select();
                }
            }

            var senderElement = sender.get_element();
            var itemText = args.get_item().get_value();
            var panelID = senderElement.getAttribute('xmlHttpPanelID');
            var panel = $find(panelID);
            panel.set_value('{"SchoolID":"' + itemText + '"}');
        }

        function requestSubjectFilter(sender, args) {
            //var standardsCleared = eval(document.getElementById('standardsCleared').value);
            //var assessmentItemsCount = document.getElementById('assessmentItemsCount').value;

            function callbackFunction(arg) {
                var senderElement = sender.get_element();
                if (arg) {
                    var itemText = args.get_item().get_text();
                    //var teacherID = senderElement.getAttribute('teacherID');
                    //var level = senderElement.getAttribute('level');
                    var panelID = senderElement.getAttribute('xmlHttpPanelID');
                    var panel = $find(panelID);

                    panel.set_value('{"Grade":"' + itemText + '"}');

                    //document.getElementById('standardsCleared').value = "true";
                }
                else {
                    sender.get_items().getItem(document.getElementById('initGrade').value).select();
                }

                dropdownObjectBlurred = false;
            }

            //if (standardsCleared && assessmentItemsCount.length == 0) {
                var senderElement = sender.get_element();
                var itemText = args.get_item().get_text();
                //var teacherID = senderElement.getAttribute('teacherID');
                //var level = senderElement.getAttribute('level');
                var panelID = senderElement.getAttribute('xmlHttpPanelID');
                var panel = $find(panelID);

                panel.set_value('{"Grade":"' + itemText + '"}');
            //}
            //else if (!dropdownObjectBlurred && assessmentItemsCount.length > 0) {
            //    var confirmDialogText = 'The <b>grade, subject or course</b> has been changed for this assessment. Please verify if the existing items are still valid based on the updated Assessment Identification.';
            //    dropdownObjectBlurred = true;
            //    setTimeout(function () { window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
            //}
            //else if (!dropdownObjectBlurred) {
            //    var confirmDialogText = 'Updating either <b>grade, subject or course</b> discards any previous Standards selections made. Select OK to continue with the update action or Cancel to return to the previous window.';
            //    dropdownObjectBlurred = true;
            //    setTimeout(function () { window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
            //}
        }

        function requestCourseFilter(sender, args) {
            //var standardsCleared = eval(document.getElementById('standardsCleared').value);
            //var assessmentItemsCount = document.getElementById('assessmentItemsCount').value;
            function callbackFunction(arg) {
                var senderElement = sender.get_element();
                if (arg) {
                    var gradeDropdown = $find('gradeDropdown');
                    var itemText = args.get_item().get_text();
                    //var teacherID = senderElement.getAttribute('teacherID');
                    //var level = senderElement.getAttribute('level');
                    var grade = gradeDropdown.get_selectedItem().get_value();
                    var panelID = senderElement.getAttribute('xmlHttpPanelID');
                    var panel = $find(panelID);

                    panel.set_value('{"Grade":"' + grade + '", "Subject":"' + itemText + '"}');

                    //document.getElementById('standardsCleared').value = "true";
                }
                else {
                    sender.get_items().getItem(document.getElementById('initSubject').value).select();
                }

                dropdownObjectBlurred = false;
            }

            //if (standardsCleared && assessmentItemsCount.length == 0) {
                var senderElement = sender.get_element();
                var gradeDropdown = $find('gradeDropdown');
                var itemText = args.get_item().get_text();
                //var teacherID = senderElement.getAttribute('teacherID');
                //var level = senderElement.getAttribute('level');
                var grade = gradeDropdown.get_selectedItem().get_value();
                var panelID = senderElement.getAttribute('xmlHttpPanelID');
                var panel = $find(panelID);

                panel.set_value('{"Grade":"' + grade + '", "Subject":"' + itemText + '"}');
            //}
            //else if (!dropdownObjectBlurred && assessmentItemsCount.length > 0) {
            //    var confirmDialogText = 'The <b>grade, subject or course</b> has been changed for this assessment. Please verify if the existing items are still valid based on the updated Assessment Identification.';
            //    dropdownObjectBlurred = true;
            //    setTimeout(function () { window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
            //}
            //else if (!dropdownObjectBlurred) {
            //    var confirmDialogText = 'Updating either <b>grade, subject or course</b> discards any previous Standards selections made. Select OK to continue with the update action or Cancel to return to the previous window.';
            //    dropdownObjectBlurred = true;
            //    setTimeout(function () { window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Alert'); }, 0);
            //}
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
    </script>
</asp:Content>
