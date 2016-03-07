<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/AddNew.Master" CodeBehind="StudentIdentification_Edit.aspx.cs" Inherits="Thinkgate.Controls.Student.StudentIdentification_Edit" %>
   
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <style>
            td {
                /* border: solid blue 1px; */
            }
            td.fieldLabel {
                width: 120px;
                margin-top: 0px;
                padding-bottom: 20px;
                vertical-align: top;
            }
                
            td.fieldEdit {
                vertical-align: top;
            }

            .resultPanel 
            {
                text-align: center;
            }
            tr {
                /* border: solid greenyellow 1px; */
            }
        </style>
        <script type="text/javascript">
            var requiredFields =    [["<%= TextBoxFirstName.ClientID %>", "#<%= LabelFirstNameErrorMessage.ClientID %>", "Please enter a value for First Name.", "<%= _selectedStudent.FirstName%>"],
                                    ["<%= TextBoxLastName.ClientID %>", "#<%= LabelLastNameErrorMessage.ClientID %>", "Please enter a value for Last Name.", "<%= _selectedStudent.LastName%>"],
                                    ["<%= TextBoxStudentID.ClientID %>", "#<%= LabelStudentIDErrorMessage.ClientID %>", "Please enter a value for Student ID.", "<%= _selectedStudent.StudentID%>"],
                                    ["<%= RadComboBoxGrade.ClientID %>", "#<%= LabelGradeErrorMessage.ClientID %>", "Please select a value for Grade.", "<%= _selectedStudent.Grade%>"],
                                    ["<%= RadComboBoxSchool.ClientID %>", "#<%= LabelSchoolErrorMessage.ClientID %>", "Please select a value for School.", "<%= _selectedStudent.SchoolID%>"]];

            function userHasAddedData() {  //This function is used in AddNew.Master

                /* Iterate through required fields to see if user has modified these. */
                for (var i = 0; i < requiredFields.length; ++i) {
                    //var inputValue = $.trim($(requiredFields[i][0]).val());
                    var inputValue = $find(requiredFields[i][0]).get_value();
                    if (inputValue != requiredFields[i][3]) return true;
                }

                /* Go through the rest of the adsftextboxes to see if user has modified these.*/
                if ($find("<%= TextBoxMiddleName.ClientID %>").get_value() != "<%= _selectedStudent.MiddleName%>") return true;
                if ($find("<%= TextBoxEmail.ClientID %>").get_value() != "<%= _selectedStudent.Email%>") return true;
                if ($("#divPhotoEdit")[0].style.display != "none") return true;
                if ($find("RadUpload1").getFileInputs()[0].value != "") return true;
                

                return false;
            }

            function selectTextBoxStudentID() {
                autoSizeWindow();
                $('#TextBoxStudentID').focus();
            }

            function selectTextBoxEmail() {
                autoSizeWindow();
                $('#TextBoxEmail').focus();
            }

            function saveStudent() {
                clearErrorMessages();

                if (checkRequiredFields()) {
                    autoSizeWindow();
                }
                else {

                    /***********************************************************************
                    Confirmation before close was only needed if the user cancelled or 
                    closed (x in upper right).  We are now posting back so we can add. 
                    Remove the confirm action from the customDialog's close process.
                    ***********************************************************************/
                    getCurrentCustomDialog().remove_beforeClose(onClientBeforeClose);

                    __doPostBack('RadButtonOk', '');
                }
            }

            function uploadPhoto() {

                /***********************************************************************
                Confirmation before close was only needed if the user cancelled or 
                closed (x in upper right).  We are now posting back so we can add. 
                Remove the confirm action from the customDialog's close process.
                ***********************************************************************/
                //getCurrentCustomDialog().remove_beforeClose(onClientBeforeClose);
                
                var labelPhotoErrorMessage
                if ($find("RadUpload1").getFileInputs()[0].value == "") {
                    labelPhotoErrorMessage = $("#labelPhotoErrorMessage");
                    labelPhotoErrorMessage.text("You have not selected an image file to upload.");
                    return;
                } 

                if (!$find("RadUpload1").validateExtensions()) {
                    labelPhotoErrorMessage = $("#labelPhotoErrorMessage");
                    labelPhotoErrorMessage.text("Invalid extension. The only allowable file types are JPG/PNG/GIF/BMP.");
                    return;
                }
                    getCurrentCustomDialog().remove_beforeClose(onClientBeforeClose);
                __doPostBack('rbPhotoSave', '');
            }

            function clearErrorMessages() {
                for (var i = 0; i < requiredFields.length; ++i) {
                    $(requiredFields[i][1]).text('');
                }
            }

            function checkRequiredFields() {
                var isError = false;

                for (var i = 0; i < requiredFields.length; ++i) {
                    var inputValue = $find(requiredFields[i][0]).get_value();

                    if (requiredFields[i][2].indexOf('Please enter') >= 0 && isStringNullOrEmpty(inputValue)) {
                        $(requiredFields[i][1]).text(requiredFields[i][2]);
                        isError = true;
                    }
                }
                return isError;
            }

            function isStringNullOrEmpty(str) {
                if (str == null || str == "") {
                    return true;
                }

                return false;
            }

            function toggleEditMode(bEditMode) {
                var imgPhoto = $('#imgPhoto')[0];
                var divPhotoEdit = $('#divPhotoEdit')[0];
                var divCopyrightWarning = $('#divCopyrightWarning')[0];
                var hlEditPhoto = $('#hlEditPhoto')[0];
                var RadUpload1 = $('#RadUpload1')[0];
                var parentCustomDialog = getCurrentCustomDialog();
                
                if (!bEditMode) {  // reset control to "View" mode
                    //Set view div on
                    imgPhoto.style.display = "inline";

                    //hide edit div
                   divPhotoEdit.style.display = "none";
                   divCopyrightWarning.style.display = "none";

                   //set button div enabled
                   hlEditPhoto.setAttribute('href', hlEditPhoto.attributes['href_bak'].nodeValue);
                   hlEditPhoto.style.color = "blue";

                   parentCustomDialog.set_width(450);
                   //ParentCustomDialog.set_autoSizeBehaviors(Telerik.Web.UI.WindowAutoSizeBehaviors.Width);
                   //ParentCustomDialog.autoSize(false);


                } else { //set control to "Edit" mode
                    //disable buttons.
                   var href = hlEditPhoto.getAttribute("href");
                   if (href && href != "" && href != null) {
                       hlEditPhoto.setAttribute('href_bak', href);
                   }
                   hlEditPhoto.removeAttribute('href');
                   hlEditPhoto.style.color = "#A0A0A0"; // Grayed out.
                   hlEditPhoto.style.textShadow = "1px 1px #ffffff";

                    divCopyrightWarning.style.display = "block";

                    //Set edit div as visible
                    divPhotoEdit.style.display = "block";
                    imgPhoto.style.display = "none";

                    parentCustomDialog.set_width(500);

                    //Set focus
                    RadUpload1.focus();
                }

                function loadClassCopy(results) {
                    var parentURL = parent.window.location.href;
                    var redirectURL = parentURL.substring(0, parentURL.indexOf('xID=') + 4) + results.substr(results.indexOf('CLONE:') + 6);
                    parent.window.location.href = redirectURL;

                }

                autoSizeWindow();
            };

            /****************************** Initialization script **********************************************/
            $(function () {
                var labelPhotoErrorMessage = $('#labelPhotoErrorMessage')[0];
                addWindowBeforeCloseEvent(); //This and other common functions are in AddNew.Master

                /*************************************************************************************
                *  This section is called when the page is created or upon returning from a postback.
                *  Below are some conditions which help to determine what situation we are in when we
                *  hit this code:
                *       $('#addPanel')[0] - This panel holds all the controls to allow the user to 
                *                           edit the student's info. We display at start and will let
                *                           the user post back when he/she is done entering the 
                *                           desired changes.  After saving these changes, the server 
                *                           will hide this panel and in its place display 
                *                           $('#resultPanel')[0] instead, which holds a "changes 
                *                           successfully saved" type message.  So if addPanel doesn't 
                *                           exist, then we are returning from saving off the changes.
                *
                *      isStringNullOrEmpty(labelPhotoErrorMessage.value) - if this control is not 
                *                           empty, then the server did not like something about the 
                *                           photo file the user tried to upload (maybe file size was 
                *                           too big). In this case, if we are returning from the 
                *                           postback with a message indication the upload of a 
                *                           photo failed, then we don't want to hide the edit section.
                *************************************************************************************/
                if ($('#addPanel')[0]) {
                    if (isStringNullOrEmpty(labelPhotoErrorMessage.innerText)) {
                        toggleEditMode(false);
                    }
                }
                else {
                    var parentCustomDialog = getCurrentCustomDialog();
                    parentCustomDialog.add_beforeClose(refreshParentWindow);
                    parentCustomDialog.set_minWidth(200);
                    autoSizeWindow();
                }
            });

        </script>
    </telerik:RadCodeBlock>
    <div style="overflow: hidden;">
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="saveStudentLoadingPanel">
            <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static" >
                <table class="fieldValueTable fieldAddModalTable" style="width: 400px; margin-left: auto; margin-right: auto;">
                    <tr>
                        <td class="fieldLabel">
                            First Name:
                        </td>
                        <td class="fieldEdit">
                            <telerik:RadTextBox runat="server" ID="TextBoxFirstName" />
                            <asp:Label runat="server" ID="LabelFirstNameErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Middle Name:
                        </td>
                        <td class="fieldEdit">
                            <telerik:RadTextBox runat="server" ID="TextBoxMiddleName" />
                            <div style="font-size: 12px;">
                                (Not Required)</div>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Last Name:
                        </td>
                        <td class="fieldEdit">
                            <telerik:RadTextBox runat="server" ID="TextBoxLastName" />
                            <asp:Label runat="server" ID="LabelLastNameErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Email:
                        </td>
                        <td class="fieldEdit">
                            <telerik:RadTextBox runat="server" ID="TextBoxEmail" />
                            <div style="font-size: 12px;">(Not Required)</div>
                            <asp:Label runat="server" ID="LabelEmailErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Student ID:
                        </td>
                        <td class="fieldEdit">
                            <telerik:RadTextBox runat="server" ID="TextBoxStudentID" />
                            <asp:Label runat="server" ID="LabelStudentIDErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Grade:
                        </td>
                        <td class="fieldEdit">
                            <telerik:RadComboBox runat="server" ID="RadComboBoxGrade" />
                            <asp:Label runat="server" ID="LabelGradeErrorMessage" Text="" ForeColor="Red" ></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            School:
                        </td>
                        <td class="fieldEdit">
                            <telerik:RadComboBox runat="server" ID="RadComboBoxSchool" />
                            <asp:Label runat="server" ID="LabelSchoolErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel" >
                            Photo:
                        </td>
                        <td class="fieldEdit">
                            <img runat="server" id="imgPhoto" ClientIDMode="Static" src="" alt="Student photo" style="max-width: 150px; max-height: 150px; display: inline; float:left;" />
                            <a runat="server" id="hlEditPhoto" ClientIDMode="Static" href="javascript:toggleEditMode(true);" href_bak="javascript:toggleEditMode(true);" style="display:inline; float: right;">Edit</a>
                        </td>
                    </tr> 
                    <tr>
                        <td colspan="2">
                            <div runat="server" id="divPhotoEdit" clientidmode="Static" style="display: block;">
                                <table>
                                    <tr>
                                        <td>File Name:</td>
                                        <td>
                                            <telerik:RadUpload ID="RadUpload1" runat="server" InitialFileInputsCount="1" 
                                                MaxFileInputsCount="1" OverwriteExistingFiles="False" ControlObjectsVisibility="None" 
                                                Width="400px" EnableFileInputSkinning="true" Skin="Web20" ClientIDMode="Static">
                                                <Localization Select="Browse" />
                                            </telerik:RadUpload>
                                            <asp:Label runat="server" ID="labelPhotoErrorMessage" ClientIDMode="Static" Text="" ForeColor="Red"></asp:Label>
                                            <input runat="server" id="hdnPhotoFilename" clientidmode="Static" type="hidden" value=""/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <div style="text-align: center;">
                                                <telerik:RadButton runat="server" ID="rbPhotoSave" Text="&nbsp;&nbsp;Upload&nbsp;&nbsp;" OnClientClicked="uploadPhoto" AutoPostBack="False"/>
                                                &nbsp;
                                                <telerik:RadButton runat="server" ID="rbPhotoCancel" Text="Cancel" AutoPostBack="False" OnClientClicked="function() {toggleEditMode(false); return false;}" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <div class="LegalMessage" id="divCopyrightWarning">
                                                <br />
                                                You must honor all copyright protection notifications on all material used within Elements&trade;. Only use content (Items, Distractors, Images, Addendums, and Assessments) that your school system has purchased the rights to reproduce or that has been marked as public domain. You are responsible for any copyright infringements. If in doubt about the content you wish to use, contact your central office for permission clarification.
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel" style="margin-top: 60px;">
                        </td>
                        <td style="text-align: right;">
                            <telerik:RadButton runat="server" ID="RadButtonCancel" Text="Cancel" AutoPostBack="False"
                                OnClientClicked="closeWindow" />
                            &nbsp;
                            <telerik:RadButton runat="server" ID="RadButtonOk" Text="OK" AutoPostBack="False"
                                OnClientClicked="saveStudent" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static" >
                <asp:Label runat="server" ID="lblResultMessage" Text="" CssClass="resultPanel"/>
                <br />
                <telerik:RadButton runat="server" ID="RadButtonClose" Text="Close" AutoPostBack="False" CssClass="resultPanel" OnClientClicked="refreshParentWindow" />
            </asp:Panel>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="saveStudentLoadingPanel" runat="server" />
    </div>
</asp:Content>
