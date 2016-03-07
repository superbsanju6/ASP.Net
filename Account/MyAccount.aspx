<%@ Page Title="" Language="C#" MasterPageFile="~/AddNew.Master" AutoEventWireup="True"
    CodeBehind="MyAccount.aspx.cs" Inherits="Thinkgate.Account.MyAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <link href="../Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    
    <style type="text/css">
        body {
            font-family: Sans-Serif, Arial;
            background-color: white;
            font-size: 12pt;
        }

        .outerTableForDisplay {
            text-align: left;
            vertical-align: top;
        }

        td.forSeparating_TopPart {
            height: 10px;
            border-bottom: 1px solid gainsboro;
        }

        td.forSeparating_BottomPart {
            height: 10px;
        }

        .fieldLabel {
            font-weight: bold;
            margin-top: 0px;
            margin-bottom: 15px;
        }

        .labelForValidation {
            color: Red;
            display: block;
            font-size: 10pt;
        }
        .ui-dialog-content li {
            font-size: 14px !important;
        }
        .lineHt30px {
            line-height: 30px;

        }
    </style>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
       <script type="text/javascript">

            pageMyAccount = new Object();
            pageMyAccount.ready = false;

            function $$(sID) {
                return document.getElementById(sID);
            }

            function cancelMyAccount() {
                closeWindow();
            }

            function userHasAddedData() {
                if (pageMyAccount.divPhotoEdit.style.display == "block" ||
                    pageMyAccount.divEmailAddrEdit.style.display == "block" ||
                    pageMyAccount.divPasswordEdit.style.display == "block") return true;
            }

            function toggleAnchorInactive(oAnchor, disable) {
                if (!oAnchor) return;
                if (disable) {
                    var href = oAnchor.getAttribute("href");
                    if (href && href != "" && href != null) {
                        oAnchor.setAttribute('href_bak', href);
                    }
                    oAnchor.removeAttribute('href');
                    oAnchor.style.color = "#A0A0A0"; // Grayed out.
                    oAnchor.style.textShadow = "1px 1px #ffffff";
                }
                else {
                    oAnchor.setAttribute('href', oAnchor.attributes['href_bak'].nodeValue);
                    oAnchor.style.color = "blue";
                }
            }

            function UpdateAccountInfo(caller) {

                $('#lblUpdateResults').text('');
                pageMyAccount.divUpdateResults.style.display = "none";


                if (validateFields(caller)) {
                    //autoSizeWindow();
                }
                else {
                    getCurrentCustomDialog().remove_beforeClose(onClientBeforeClose);
                    __doPostBack(caller, '');
                }
            }

            function validateFields(caller) {
                var isError = false;

                pageMyAccount.WhichBtnPosted = caller;
                switch (pageMyAccount.WhichBtnPosted) {

                    /*************************/
                    case "rbEmailAddrSave":
                        /*************************/

                        //Clear out any preexisting error messages.
                        $('#lblEmailAddrErrorMessage').text('');

                        // Test for empty email address text box.

                        if (isStringNullOrEmpty(pageMyAccount.rtbEmailAddr.value)) {
                            $('#lblEmailAddrErrorMessage').text('Please enter your email address.');
                            isError = true;
                        }
                        break;

                        /*************************/
                    case "rbPasswordSave":
                        /*************************/

                        //Clear out any preexisting error messages.
                        $('#lblOrigPwdErrorMessage').text('');
                        $('#lblNewPwdErrorMessage').text('');
                        $('#lblRetypePwdErrorMessage').text('');


                        if (isStringNullOrEmpty(pageMyAccount.rtbOrigPwd.value))
                            $('#lblOrigPwdErrorMessage').text('Please enter your current password.');

                        //Modified to add error for new passwords less than 6 characters long - Ccreech 12/3/2013
                        if (isStringNullOrEmpty(pageMyAccount.rtbNewPwd.value)) {
                            $('#lblNewPwdErrorMessage').text('Please enter a new password.');
                        } else if ($.trim($("#rtbNewPwd").val()) != "" && $("#hdnPasswordConfReq").val() == "Yes") {
                            var passwordReg = new RegExp($("#hdnPasswordFormatReg").val());
                            var password = $("#rtbNewPwd").val();
                            if (password.match(passwordReg) == null) {
                                $('#lblNewPwdErrorMessage').text($("#hdnPasswordValidationMsg").val());
                                isError = true;
                            }
                        }
                        else if (pageMyAccount.rtbNewPwd.value.toString().length <= 6) {
                            $('#lblNewPwdErrorMessage').text('Password length must be greater than or equal to 6');
                            isError = true;
                        }

                        if (isStringNullOrEmpty(pageMyAccount.rtbRetypePwd.value)) {
                            $('#lblRetypePwdErrorMessage').text('Please retype the new password.');
                        }
                        //End Modification - Ccreech 12/3/2013


                        if ($('#lblOrigPwdErrorMessage').text() != '' ||
                            $('#lblNewPwdErrorMessage').text() != '' ||
                            $('#lblRetypePwdErrorMessage').text() != '') {
                            isError = true;
                        } else {
                            if (pageMyAccount.rtbRetypePwd.value != pageMyAccount.rtbNewPwd.value) {
                                $('#lblRetypePwdErrorMessage').text('New and retyped passwords do not match.');
                                isError = true;
                            } else if (pageMyAccount.rtbNewPwd.value == pageMyAccount.rtbOrigPwd.value) {
                                $('#lblRetypePwdErrorMessage').text('Your new password matches your old password.');
                                isError = true;
                            }
                        }
                        break;
                        /*************************/
                    case "rbPhotoSave":
                        /*************************/

                        //Clear out any preexisting error messages.
                        $('#lblPhotoErrorMessage').text('');

                        if ($find("ruUserImage").getFileInputs()[0].value == "") {
                            $('#lblPhotoErrorMessage').text('You have not selected an image file to upload.');
                            isError = true;
                        }

                        if (!$find("ruUserImage").validateExtensions()) {
                            $('#lblPhotoErrorMessage').text('Invalid extension. The only allowable file types are JPG/PNG/GIF/BMP.');
                            isError = true;
                        }
                        break;
                }

                return isError;
            }

            function toggleEditMode(bEditMode, oViewDiv, oEditDiv, oFocusCtrl) {

                if (!bEditMode) {  // reset all controls to "View" mode
                    //Set all divs back to view mode.
                    //Set view divs on
                    pageMyAccount.divEmailAddr.style.display = "block";
                    pageMyAccount.divPassword.style.display = "block";
                    pageMyAccount.divPhoto.style.display = "block";

                    //Set edit divs off
                    pageMyAccount.divEmailAddrEdit.style.display = "none";
                    pageMyAccount.divPasswordEdit.style.display = "none";
                    pageMyAccount.divPhotoEdit.style.display = "none";
                    pageMyAccount.divCopyrightWarning.style.display = "none";

                    //set button divs enabled
                    toggleAnchorInactive(pageMyAccount.hlEditEmailAddr, false);
                    toggleAnchorInactive(pageMyAccount.hlEditPassword, false);
                    toggleAnchorInactive(pageMyAccount.hlEditPhoto, false);

                } else {
                    //Set up div for edit per button that was clicked.
                    //but leave other divs in "view mode and disable the buttons.

                    //disable buttons.
                    toggleAnchorInactive(pageMyAccount.hlEditEmailAddr, true);
                    toggleAnchorInactive(pageMyAccount.hlEditPassword, true);
                    toggleAnchorInactive(pageMyAccount.hlEditPhoto, true);

                    if (oEditDiv.getAttribute("id") == "divPhotoEdit") {
                        pageMyAccount.divCopyrightWarning.style.display = "block";
                    }

                    //Set edit div as visible
                    oEditDiv.style.display = "block";
                    oViewDiv.style.display = "none";

                    //Set focus
                    if (oFocusCtrl != null) oFocusCtrl.focus();
                    //autoSizeWindow();
                }
            };

            function resetToViewMode() {

                pageMyAccount.rtbEmailAddr.value = $('#lblEmailAddr').text();
                $('#lblEmailAddrErrorMessage').text('');

                pageMyAccount.rtbOrigPwd.value = "";
                $('#lblOrigPwdErrorMessage').text('');
                pageMyAccount.rtbNewPwd.value = "";
                $('#lblNewPwdErrorMessage').text('');
                pageMyAccount.rtbRetypePwd.value = "";
                $('#lblRetypePwdErrorMessage').text('');

                pageMyAccount.ruUserImage.value = "";
                $('#lblPhotoErrorMessage').text('');

                $('#lblUpdateResults').text('');
                pageMyAccount.WhichBtnPosted = "";
                pageMyAccount.divUpdateResults.style.display = "none";
                toggleEditMode(false, null, null, null);

            }

            function mapControls(oParent) {
                // Set up object for easy tracking of our HTML controls
                // Carry out any initialization needed.
                //Email Address controls
                oParent.divEmailAddr = $$('divEmailAddr');
                oParent.lblEmailAddr = $$('lblEmailAddr');
                oParent.divEmailAddrEdit = $$('divEmailAddrEdit');
                oParent.rtbEmailAddr = $$('rtbEmailAddr');
                oParent.lblEmailAddrErrorMessage = $$('lblEmailAddrErrorMessage');
                oParent.hlEditEmailAddr = $$('hlEditEmailAddr');
                oParent.rbEmailAddrSave = $$('rbEmailAddrSave');
                oParent.rbEmailAddrCancel = $$('rbEmailAddrCancel');

                //Password controls
                oParent.divPassword = $$('divPassword');
                oParent.divPasswordEdit = $$('divPasswordEdit');
                oParent.rtbOrigPwd = $$('rtbOrigPwd');
                oParent.lblOrigPwdErrorMessage = $$('lblOrigPwdErrorMessage');
                oParent.rtbNewPwd = $$('rtbNewPwd');
                oParent.lblNewPwdErrorMessage = $$('lblNewPwdErrorMessage');
                oParent.rtbRetypePwd = $$('rtbRetypePwd');
                oParent.lblRetypePwdErrorMessage = $$('lblRetypePwdErrorMessage');
                oParent.hlEditPassword = $$('hlEditPassword');
                oParent.rbPasswordSave = $$('rbEmailAddrSave');
                oParent.rbPasswordCancel = $$('rbEmailAddrCancel');

                //Photo controls
                oParent.divPhoto = $$('divPhoto');
                oParent.divPhotoEdit = $$('divPhotoEdit');
                oParent.ruUserImage = $$('ruUserImage');
                oParent.lblPhotoErrorMessage = $$('lblPhotoErrorMessage');
                oParent.hlEditPhoto = $$('hlEditPhoto');
                oParent.rbPhotoSave = $$('rbPhotoSave');
                oParent.rbPhotoCancel = $$('rbPhotoCancel');
                oParent.divCopyrightWarning = $$('divCopyrightWarning');

                //UpdateResults Label
                oParent.divUpdateResults = $$('divUpdateResults');
                oParent.lblUpdateResults = $$('lblUpdateResults');

            }

            function roundTripProcessing() {
                mapControls(pageMyAccount);
                toggleEditMode(false, null, null, null);
                pageMyAccount.divUpdateResults.style.display = "block";
                if ($('#lblUpdateResults').text() != "Update was successful.") {
                    switch (pageMyAccount.WhichBtnPosted) {
                        case "rbEmailAddrSave":
                            toggleEditMode(true, pageMyAccount.divEmailAddr, pageMyAccount.divEmailAddrEdit, pageMyAccount.rtbEmailAddr);
                            break;
                        case "rbPasswordSave":
                            toggleEditMode(true, pageMyAccount.divPassword, pageMyAccount.divPasswordEdit, pageMyAccount.rtbOrigPwd);
                            break;
                        case "rbPhotoSave":
                            toggleEditMode(true, pageMyAccount.divPhoto, pageMyAccount.divPhotoEdit, pageMyAccount.ruUserImage);
                            break;
                        default:
                    }
                    if (pageMyAccount.WhichBtnPosted == "rbEmailAddrSave") {
                        toggleEditMode(true, pageMyAccount.divEmailAddr, pageMyAccount.divEmailAddrEdit, pageMyAccount.rtbEmailAddr);
                        //pageMyAccount.rtbEmailAddr.focus();
                    } else if (pageMyAccount.WhichBtnPosted == "rbPasswordSave") {
                        toggleEditMode(true, pageMyAccount.divPassword, pageMyAccount.divPasswordEdit, pageMyAccount.rtbOrigPwd);
                    }
                }
            }

            function diaplayDialog() {
                window.parent.PasswordRequirement.ShowDialogWithPosition($("#displayMsgChild").html());
                return false;
            }


            /* We will either come through here when the page is first rendered, or when a postback occurs 
            *  due to user uploading a new image.  Therefore, it has to be able to handle both.  We can 
            *  tell the difference by seeing if lblUpdateResults has any text in it.  If so, then it's a 
            *  post back.  Otherwise, it is the initial rendering.
            */
            $(function () {
                getCurrentCustomDialog().set_width(730);
                getCurrentCustomDialog().set_height(570);

                addWindowBeforeCloseEvent();

                mapControls(pageMyAccount);
                if (!isStringNullOrEmpty($('#lblUpdateResults').text())) {
                    pageMyAccount.divUpdateResults.style.display = "block";
                    toggleEditMode(false, null, null, null);
                    if ($('#lblUpdateResults').text() != "Update was successful.") {
                        toggleEditMode(true, pageMyAccount.divPhoto, pageMyAccount.divPhotoEdit, null);
                    }
                }
                else {
                    resetToViewMode();
                }
                pageMyAccount.ready = true;


            });

        </script>
    </telerik:RadCodeBlock>
    <div style="overflow: hidden;">
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="myAccountLoadingPanel">
            <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
                <asp:HiddenField ID="hdnPasswordConfReq" Value="" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdnPasswordFormatReg" Value="" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdnPasswordValidationMsg" Value="" runat="server" ClientIDMode="Static" />
                <div id="displayMsgChild" style="display: none;" runat="server" clientidmode="Static">
                </div>
                <table style="margin-right: 40px; margin-left: 60px;">
                    <tr>
                        <td colspan="3" style="text-align: center;">
                            <div id="divUpdateResults">
                                <asp:Label runat="server" ID="lblUpdateResults" CssClass="labelForValidation" ClientIDMode="Static"></asp:Label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel outerTableForDisplay" style="width: 150px;">Name:
                        </td>
                        <td style="width: 385px;">
                            <label runat="server" id="lblName" clientidmode="Static">
                            </label>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="forSeparating_TopPart" colspan="3"></td>
                    </tr>
                    <tr>
                        <td class="forSeparating_BottomPart" colspan="3"></td>
                    </tr>
                    <tr>
                        <td class="fieldLabel outerTableForDisplay" style="width: 150px;">User ID:
                        </td>
                        <td style="width: 385px;">
                            <label runat="server" id="lblUserID" clientidmode="Static">
                            </label>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="forSeparating_TopPart" colspan="3"></td>
                    </tr>
                    <tr>
                        <td class="forSeparating_BottomPart" colspan="3"></td>
                    </tr>
                    <tr>
                        <td class="fieldLabel outerTableForDisplay" style="width: 150px;">User Role:
                        </td>
                        <td style="width: 385px;">
                            <label runat="server" id="lblUserRole" clientidmode="Static">
                            </label>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="forSeparating_TopPart" colspan="3"></td>
                    </tr>
                    <tr>
                        <td class="forSeparating_BottomPart" colspan="3"></td>
                    </tr>
                    <tr>
                        <td class="fieldLabel outerTableForDisplay" style="width: 150px;">Email Address:
                        </td>
                        <td style="width: 385px;">
                            <div runat="server" id="divEmailAddr" clientidmode="Static">
                                <asp:Label runat="server" ID="lblEmailAddr" ClientIDMode="Static"></asp:Label>
                            </div>
                            <div runat="server" id="divEmailAddrEdit" clientidmode="Static">
                                <table>
                                    <tr>
                                        <td>
                                            <telerik:RadTextBox runat="server" ID="rtbEmailAddr" MaxLength="50" Width="250px" ClientIDMode="Static" />
                                            <asp:Label runat="server" ID="lblEmailAddrErrorMessage" CssClass="labelForValidation" ClientIDMode="Static"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div style="text-align: center;">
                                                <telerik:RadButton runat="server" ID="rbEmailAddrSave" Text="Save Changes" AutoPostBack="False"
                                                    OnClientClicked="function(){UpdateAccountInfo('rbEmailAddrSave');}" ClientIDMode="Static" />
                                                &nbsp;
                                                <telerik:RadButton runat="server" ID="rbEmailAddrCancel" Text="Cancel" AutoPostBack="False"
                                                    OnClientClicked="resetToViewMode" ClientIDMode="Static" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                        <td class="outerTableForDisplay">
                            <a runat="server" id="hlEditEmailAddr" href="javascript:toggleEditMode(true, pageMyAccount.divEmailAddr, pageMyAccount.divEmailAddrEdit, pageMyAccount.rtbEmailAddr);"
                                href_bak="javascript:toggleEditMode(true, pageMyAccount.divEmailAddr, pageMyAccount.divEmailAddrEdit, pageMyAccount.rtbEmailAddr);" clientidmode="Static">Edit</a>
                        </td>
                    </tr>
                    <tr>
                        <td class="forSeparating_TopPart" colspan="3"></td>
                    </tr>
                    <tr>
                        <td class="forSeparating_BottomPart" colspan="3"></td>
                    </tr>
                    <tr style='display: <%= (PermissionToChgPwd) ? "visible" : "none" %>;'>
                        <td class="fieldLabel outerTableForDisplay" style="width: 150px;">Change Password:
                        </td>
                        <td style="width: 385px;">
                            <div runat="server" id="divPassword" style="display: none;" clientidmode="Static">
                            </div>
                            <div runat="server" id="divPasswordEdit" style="display: none;" clientidmode="Static">
                                <table>
                                    <tr>
                                        <td class="fieldLabel" style="width: 110px;">Current:
                                        </td>
                                        <td>
                                            <telerik:RadTextBox runat="server" ID="rtbOrigPwd" MaxLength="128" TextMode="Password" ClientIDMode="Static" />
                                            <asp:Label runat="server" ID="lblOrigPwdErrorMessage" CssClass="labelForValidation" ClientIDMode="Static"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="fieldLabel" style="width: 110px;">New:
                                        </td>
                                        <td>
                                            <telerik:RadTextBox runat="server" ID="rtbNewPwd" MaxLength="128" TextMode="Password" ClientIDMode="Static" />
                                            <asp:Label runat="server" ID="lblNewPwdErrorMessage" CssClass="labelForValidation" ClientIDMode="Static"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="fieldLabel" style="width: 110px;">Re-enter new:
                                        </td>
                                        <td>
                                            <telerik:RadTextBox runat="server" ID="rtbRetypePwd" MaxLength="128" TextMode="Password" ClientIDMode="Static" />
                                            <asp:Label runat="server" ID="lblRetypePwdErrorMessage" CssClass="labelForValidation" ClientIDMode="Static"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <a runat="server" id="h1MinimumPassword" onclick="return diaplayDialog()" class="lineHt30px" clientidmode="Static" style="color:blue;text-decoration:underline;cursor:pointer">Minimum Password Requirements</a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <div style="text-align: center; padding-top: 3px;">
                                                <telerik:RadButton runat="server" ID="rbPasswordSave" Text="Save Changes" AutoPostBack="False"
                                                    OnClientClicked="function(){UpdateAccountInfo('rbPasswordSave');}" ClientIDMode="Static" />
                                                &nbsp;
                                                <telerik:RadButton runat="server" ID="rbPasswordCancel" Text="Cancel" AutoPostBack="False"
                                                    OnClientClicked="resetToViewMode" ClientIDMode="Static" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                        <td class="outerTableForDisplay">
                            <a runat="server" id="hlEditPassword" href="javascript:toggleEditMode(true, pageMyAccount.divPassword, pageMyAccount.divPasswordEdit, pageMyAccount.rtbOrigPwd);"
                                href_bak="javascript:toggleEditMode(true, pageMyAccount.divPassword, pageMyAccount.divPasswordEdit, pageMyAccount.rtbOrigPwd);" clientidmode="Static">Change</a>
                        </td>
                    </tr>
                    <tr style='display: <%= (PermissionToChgPwd) ? "visible" : "none" %>;'>
                        <td class="forSeparating_TopPart" colspan="3"></td>
                    </tr>
                    <tr style='display: <%= (PermissionToChgPwd) ? "visible" : "none" %>;'>
                        <td class="forSeparating_BottomPart" colspan="3"></td>
                    </tr>
                    <tr>
                        <td class="fieldLabel outerTableForDisplay" style="width: 150px;">Photo:
                        </td>
                        <td style="width: 385px;">
                            <div runat="server" id="divPhoto" clientidmode="Static">
                                <img runat="server" id="imgPhoto" src="" alt="Account owner's photo" style="max-width: 150px; max-height: 150px;" />
                            </div>
                            <div runat="server" id="divPhotoEdit" clientidmode="Static">
                                <table>
                                    <tr>
                                        <td class="fieldLabel" style="width: 105px;">File Name:
                                        </td>
                                        <td style="width: 275px;">
                                            <telerik:RadUpload ID="ruUserImage" runat="server" InitialFileInputsCount="1" MaxFileInputsCount="1"
                                                OverwriteExistingFiles="False" ControlObjectsVisibility="None" InputSize="35" Width="300px"
                                                EnableFileInputSkinning="true" Skin="Web20" ClientIDMode="Static">
                                                <Localization Select="Browse" />
                                            </telerik:RadUpload>
                                            <asp:Label runat="server" ID="lblPhotoErrorMessage" CssClass="labelForValidation" ClientIDMode="Static"></asp:Label>
                                            <input runat="server" id="hdnPhotoFilename" clientidmode="Static" type="hidden" value="" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <div style="text-align: center;">
                                                <telerik:RadButton runat="server" ID="rbPhotoSave" Text="Save Changes" AutoPostBack="False"
                                                    OnClientClicked="function(){UpdateAccountInfo('rbPhotoSave');}" />
                                                &nbsp;
                                                <telerik:RadButton runat="server" ID="rbPhotoCancel" Text="Cancel" AutoPostBack="False"
                                                    OnClientClicked="resetToViewMode" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                        <td class="outerTableForDisplay">
                            <a runat="server" id="hlEditPhoto" href="javascript:toggleEditMode(true, pageMyAccount.divPhoto, pageMyAccount.divPhotoEdit, pageMyAccount.ruUserImage);"
                                href_bak="javascript:toggleEditMode(true, pageMyAccount.divPhoto, pageMyAccount.divPhotoEdit, pageMyAccount.ruUserImage);" clientidmode="Static">Edit</a>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="font-size: 10pt;">
                            <div id="divCopyrightWarning" runat="server" style="display: none;" clientidmode="Static">
                                You must honor all copyright protection notifications on all material used within
                                Elements. Only use content (Items, Distractors, Images, Addendums, Assessments)
                                that your school system has purchased the rights to reproduce or that has been marked
                                as public domain. You are responsible for any copyright infringements. If in doubt
                                about the content you wish to use, contact your central office for permission or
                                clarification.
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="myAccountLoadingPanel" runat="server" />
    </div>
</asp:Content>
