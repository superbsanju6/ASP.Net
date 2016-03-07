<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StaffIdentification_Edit.aspx.cs"
    Inherits="Thinkgate.Controls.Staff.StaffIdentification_Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<%--This height of 100% helps elements fill the page--%>
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <title>Staff Identification</title>
    <base target="_self" />
    <telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
    </telerik:RadStyleSheetManager>
    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css" rel="stylesheet"
        type="text/css" runat="server" />
    <script type="text/javascript">
        pageMyAccount = new Object();
        pageMyAccount.ready = false;

        function $$(sID) {
            return document.getElementById(sID);
        }
    </script>
    <style>
        .tableDiv {
            display: table;
        }

        .row {
            display: table-row;
            width:100%;
        }
        .lineHt30px {
            line-height: 30px;
        }
        .cellLabel {
            display: table-cell;
            width: 200px;
            height: 40px;
            text-align: left;
            font-size: 13pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
        }
        .ui-dialog-content li {
            font-size: 14px !important;
        }
        .cellValue {
            display: table-cell;
            height: 40px;
            text-align: left;
            font-size: 12pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
        }

        .textBox {
            border: solid 1px #000;
            padding: 2px;
            width: 250px;
        }    

        .smallText {
            font-size: 9pt;
        }
    #RadWindowWrapper_RadWindow1Url{width:630px !important;}

        .radDropdownBtn {
            font-weight: bold;
            font-size: 11pt;
        }

        .labelForValidation {
            color: Red;
            display: block;
            font-size: 10pt;
            width:250px;
        }

        .fieldLabel {
            font-weight: bold;
            font-size: 10pt;
            white-space: nowrap;
            text-align: left;
            width: 110px;
            height: 30px;
            vertical-align: top;
        }

        .fieldEntry {
            vertical-align: top;
            text-align: left;
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
            float: right;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
        }
    </style>
</head>
<body style="background-color: #FFF;">
    <form id="form2" runat="server" style="height: 100%;">
        <asp:HiddenField ID="hdnPasswordConfReq" Value="" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdnPasswordFormatReg" Value="" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdnPasswordValidationMsg" Value="" runat="server" ClientIDMode="Static" />
        <div id="displayMsgChild" style="display: none;" runat="server" clientidmode="Static">
        </div>
        <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-migrate-1.1.0.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery.scrollTo.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="False" Modal="True" InitialBehaviors="Close"
            Animation="None">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
        </telerik:RadSkinManager>
        <div id="divUpdateResults">
            <asp:Label runat="server" ID="lblUpdateResults" CssClass="labelForValidation"></asp:Label>
        </div>
        <div class="tableDiv" style="height:600px !important;">
            <div class="row">
                <div class="cellLabel">First Name:</div>
                <div class="cellValue">
                    <asp:TextBox ID="firstName" runat="server" ClientIDMode="Static" CssClass="textBox" />
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">Middle Name:</div>
                <div class="cellValue">
                    <asp:TextBox ID="middleName" runat="server" ClientIDMode="Static" CssClass="textBox" /><span class="smallText">(Not Required)</span>
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">Last Name:</div>
                <div class="cellValue">
                    <asp:TextBox ID="lastName" runat="server" ClientIDMode="Static" CssClass="textBox" />
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">Email:</div>
                <div class="cellValue">
                    <asp:TextBox ID="email" runat="server" ClientIDMode="Static" CssClass="textBox" /><span class="smallText">(Not Required)</span>
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">Login ID:</div>
                <div class="cellValue">
                    <asp:TextBox ID="loginID" runat="server" ClientIDMode="Static" CssClass="textBox" />
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">School:</div>
                <div class="cellValue">
                    <telerik:RadComboBox runat="server" ID="schoolDropdown" ClientIDMode="AutoID" Skin="Web20" Text=""
                        CssClass="radDropdownBtn" Width="250" AutoPostBack="false" EmptyMessage="Multiple" HighlightTemplatedItems="true">
                        <ItemTemplate>
                            <div onclick="StopPropagation(event)" style="white-space: nowrap;">
                                <asp:CheckBox runat="server" ID="schoolCheckbox" ClientIDMode="AutoID" onclick="evaluateCheckedItem(this, 'school')" />
                                <asp:Label runat="server" ID="schoolLabel" ClientIDMode="AutoID" AssociatedControlID="schoolCheckbox">
                                    <img src="../../Images/ok.png" onclick="$('#' + this.parentNode.id.replace('schoolLabel', 'schoolCheckbox')).click();" style="cursor:pointer;">
                                </asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadComboBox>
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">
                    Primary School:
                </div>
                <div class="cellValue">
                    <telerik:RadComboBox runat="server" ID="cmbPrimarySchool" ClientIDMode="AutoID" Skin="Web20"
                        Text="" CssClass="radDropdownBtn" Width="250" AutoPostBack="false" EmptyMessage="Select..."
                        HighlightTemplatedItems="true">
                    </telerik:RadComboBox>
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">User Type:</div>
                <div class="cellValue">
                    <telerik:RadComboBox runat="server" ID="userTypeDropdown" ClientIDMode="AutoID" Skin="Web20" Text=""
                        CssClass="radDropdownBtn" Width="250" AutoPostBack="false" EmptyMessage="Multiple" HighlightTemplatedItems="true">
                        <ItemTemplate>
                            <div onclick="StopPropagation(event)" style="white-space: nowrap;">
                                <asp:CheckBox runat="server" ID="userTypeCheckbox" ClientIDMode="AutoID" onclick="evaluateCheckedItem(this, 'userType')" />
                                <asp:Label runat="server" ID="userTypeLabel" ClientIDMode="AutoID" AssociatedControlID="userTypeCheckbox">
                                    <img src="../../Images/ok.png" onclick="$('#' + this.parentNode.id.replace('userTypeLabel', 'userTypeCheckbox')).click();" style="cursor:pointer;">
                                </asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadComboBox>
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">Primary Role:</div>
                <div class="cellValue">
                    <telerik:RadComboBox runat="server" ID="cmbPrimaryUser" ClientIDMode="AutoID" Skin="Web20"
                        Text="" CssClass="radDropdownBtn" Width="250" AutoPostBack="false" EmptyMessage="Select..."
                        HighlightTemplatedItems="true">
                    </telerik:RadComboBox>
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">Restrictions:</div>
                <div class="cellValue">
                    <telerik:RadComboBox runat="server" ID="restrictionsDropdown" ClientIDMode="AutoID" Skin="Web20" Text="" CssClass="radDropdownBtn" Width="250" AutoPostBack="false">
                        <Items>
                            <telerik:RadComboBoxItem Text="None" Value="None" />
                            <telerik:RadComboBoxItem Text="Access Revoked" Value="Access Revoked" />
                            <telerik:RadComboBoxItem Text="Access Revoked-password attempts" Value="Access Revoked-password attempts" />
                            <telerik:RadComboBoxItem Text="Change password on next login" Value="Change password on next login" />
                        </Items>
                    </telerik:RadComboBox>
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">Password:</div>
                <div class="cellValue" style="text-align: right; padding-right: 30px;">
                    <div runat="server" id="divPassword" style="display: none;">
                    </div>
                    <div runat="server" id="divPasswordEdit" style="display: none;">
                        <table>
                            <tr>
                                <td class="fieldLabel">New:
                                </td>
                                <td class="fieldEntry">
                                    <telerik:RadTextBox runat="server" ID="rtbNewPwd" MaxLength="128" TextMode="Password" Skin="Office2010Black" />
                                    <asp:Label runat="server" ID="lblNewPwdErrorMessage" CssClass="labelForValidation"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="fieldLabel">Re-enter new:
                                </td>
                                <td class="fieldEntry">
                                    <telerik:RadTextBox runat="server" ID="rtbRetypePwd" MaxLength="128" TextMode="Password" Skin="Office2010Black" />
                                    <asp:Label runat="server" ID="lblRetypePwdErrorMessage" CssClass="labelForValidation"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="fieldLabel">

                                </td>
                                <td class="fieldEntry">
                                    <a runat="server" id="h1MinimumPassword" class="lineHt30px" clientidmode="Static" style="color:blue;text-decoration:underline;cursor:pointer">Minimum Password Requirements</a>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div style="text-align: center; padding-top: 3px;">
                                        <telerik:RadButton runat="server" ID="rbPasswordCancel" Text="Cancel" AutoPostBack="False"
                                            OnClientClicked="resetToViewMode" Skin="Web20" />
                                        &nbsp;
                                        <telerik:RadButton runat="server" ID="rbPasswordSave" Text="Save Changes" AutoPostBack="False"
                                            Skin="Web20" OnClick="UpdatePassword" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Label runat="server" ID="lblResultMessage" Text="" />
                    <a runat="server" id="hlEditPassword" href="javascript:toggleEditMode(true, pageMyAccount.divPassword, pageMyAccount.divPasswordEdit);"
                        href_bak="javascript:toggleEditMode(true, pageMyAccount.divPassword, pageMyAccount.divPasswordEdit);" style="float:right;padding-right:60px">Change</a>
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">Photo:</div>
                <div class="cellValue" style="padding-right: 30px;">
                    <div runat="server" id="divPhoto" style="float: left;">
                        <img runat="server" id="imgPhoto" src="" alt="Account owner's photo" style="max-width: 150px; max-height: 150px;" />
                    </div>
                    <a runat="server" id="hlEditPhoto" style="float: right;padding-right:60px" href="javascript:toggleEditMode(true, pageMyAccount.divPhoto, pageMyAccount.divPhotoEdit, pageMyAccount.ruUserImage);"
                        href_bak="javascript:toggleEditMode(true, pageMyAccount.divPhoto, pageMyAccount.divPhotoEdit, pageMyAccount.ruUserImage);">Edit</a>
                
        <div runat="server" id="divPhotoEdit">
                <table>
                    <tr>
                        <td class="fieldLabel">File Name:</td>
                        <td class="fieldEntry">
                            <telerik:RadAsyncUpload ID="ruUserImage" runat="server" InitialFileInputsCount="1"
                                MaxFileSize="1500000" MaxFileInputsCount="1" OverwriteExistingFiles="False" ControlObjectsVisibility="None"
                                AllowedFileExtensions=".jpg,.gif,.png" Width="270px" EnableFileInputSkinning="true"
                                Skin="Web20" ClientIDMode="Static">
                                <Localization Select="Browse" />
                            </telerik:RadAsyncUpload>
                            <asp:Label runat="server" ID="lblPhotoErrorMessage" CssClass="labelForValidation"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div style="text-align: center;">
                                <telerik:RadButton runat="server" ID="rbPhotoSave" ClientIDMode="Static" Text="Upload" AutoPostBack="False"
                                    OnClientClicked="UpdateAccountInfo" OnClick="UpdatePhoto" Skin="Web20" />
                                &nbsp;
                                <telerik:RadButton runat="server" ID="rbPhotoCancel" Text="Cancel" AutoPostBack="False"
                                    OnClientClicked="resetToViewMode" Skin="Web20" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                             <br />
            <div id="divCopyrightWarning" runat="server" style="display: none; font-size: 10pt; text-align: left;">
                You must honor all copyright protection notifications on all material used within
                Elements. Only use content (Items, Distractors, Images, Addendums, Assessments) that your
                school system has purchased the rights to reproduce or that has been marked as public
                domain. You are responsible for any copyright infringements. If in doubt about the
                content you wish to use, contact your central office for permission or clarification.
            </div>
                        </td>
                    </tr>
                </table>
            </div>         

                </div>
            </div>
       
       
        <div class="row">
            <div class="cellLabel">&nbsp;</div>
                <div class="cellValue" style="padding-right:80px">
            <asp:Button runat="server" ID="updateStaffButton" ClientIDMode="Static" CssClass="roundButtons" Text="  OK  " OnClientClick="updateStaffIdentification(); return false;" />
            <span style="display: none;">
                <asp:Button runat="server" ID="updateStaffButtonTrigger" ClientIDMode="Static" OnClick="UpdateStaff" /></span>
            <asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons" Text="  Cancel  " OnClientClick="cancelUpdate(); return false;" />
        </div>

        </div>
         </div>
        <script type="text/javascript">
            var modalWin = parent.$find('RadWindow1Url');

            function StopPropagation(e) {
                //cancel bubbling
                e.cancelBubble = true;
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
            }



            function evaluateCheckedItem(item, dropdownType) {
                var dropdownObj = dropdownType == 'school' ? $find("<%= schoolDropdown.ClientID %>") : $find("<%= userTypeDropdown.ClientID %>");
                var ddPrimary = dropdownType == 'school' ? $find("<%= cmbPrimarySchool.ClientID %>") : $find("<%= cmbPrimaryUser.ClientID %>");
                var itemChecked = item.checked;
                var itemValue = $get(item.id.replace(dropdownType + 'Checkbox', dropdownType + 'Label')).innerHTML;
                var allItemsChecked = true;
                var itemsCheckedCount = 0;
                var checkbox;
                var labelValue;
                var items = dropdownObj.get_items();
                ddPrimary.clearItems();
                ddPrimary.clearSelection();

                if (itemChecked && itemValue == 'All') {
                    for (var i = 1; i < items.get_count() ; i++) {
                        checkbox = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Checkbox');
                        labelValue = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Label').innerHTML;
                        if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;
                        checkbox.checked = true;
                        var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                        comboItem.set_text(labelValue);
                        ddPrimary.trackChanges();
                        comboItem.set_value(dropdownObj.findItemByText(labelValue).get_value());
                        ddPrimary.get_items().add(comboItem);
                        comboItem.select();
                        ddPrimary.commitChanges();
                    }

                    dropdownObj.set_text('Multiple');
                }
                else if ((itemValue == 'All' || itemValue == '') && !itemChecked) {
                    for (var i = 1; i < items.get_count() ; i++) {
                        checkbox = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Checkbox');
                        labelValue = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Label').innerHTML;
                        if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;
                        checkbox.checked = false;
                        ddPrimary.clearItems();
                        ddPrimary.clearSelection();
                    }

                    dropdownObj.set_text('');
                }
                else {
                    ddPrimary.clearItems();
                    ddPrimary.clearSelection();
                    for (var i = 0; i < items.get_count() ; i++) {
                        checkbox = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Checkbox');
                        labelValue = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Label').innerHTML;
                        if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                        if (!checkbox.checked) {
                            allItemsChecked = false;
                        }
                        else {
                            itemsCheckedCount++;
                            var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                            comboItem.set_text(labelValue);
                            ddPrimary.trackChanges();
                            comboItem.set_value(dropdownObj.findItemByText(labelValue).get_value());
                            ddPrimary.get_items().add(comboItem);
                            comboItem.select();
                            ddPrimary.commitChanges();

                        }
                    }

                    if (allItemsChecked) {
                        //checkbox = $get(dropdownObj.get_id() + '_i0_' + dropdownType + 'Checkbox');
                        //checkbox.checked = true;
                        dropdownObj.set_text('Multiple');
                    }
                    else {
                        //checkbox = $get(dropdownObj.get_id() + '_i0_' + dropdownType + 'Checkbox');
                        //checkbox.checked = false;

                        switch (itemsCheckedCount) {
                            case 0:
                                dropdownObj.set_text('');
                                break;
                            case 1:
                                for (var i = 0; i < items.get_count() ; i++) {
                                    checkbox = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Checkbox');
                                    labelValue = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Label').innerHTML;
                                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                                    if (checkbox.checked) {
                                        var labelValue = $get(checkbox.id.replace(dropdownType + 'Checkbox', dropdownType + 'Label')).innerHTML;
                                        dropdownObj.set_text(labelValue);
                                    }
                                }
                                break;
                            default:
                                dropdownObj.set_text('Multiple');
                                break;
                        }
                    }
                }

                if (itemValue.toLowerCase().toLowerCase().indexOf('<img') > -1) {
                    if (allItemsChecked || itemsCheckedCount > 0) {
                        dropdownObj.hideDropDown();
                    }
                    //else {
                    //staffEditAlert('Please select a ' + (dropdownType == 'school' ? 'school' : 'user type') + '.', 300, 100);
                    //}
                }
            }

            function staffEditAlert(message, width, height) {
                var wnd = window.radalert(message, width, height);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            }

            function updateStaffIdentification() {
                var schoolDropdown = $find("<%= schoolDropdown.ClientID %>");
                var userTypeDropdown = $find("<%= userTypeDropdown.ClientID %>");
                var primarySchool = $find("<%= cmbPrimarySchool.ClientID %>");
                var primaryUser = $find("<%= cmbPrimaryUser.ClientID %>");
                var anySchoolItemsChecked = false;
                var anyUserTypeItemsChecked = false;
                var checkbox;
                var labelValue;
                var schoolItems = schoolDropdown.get_items();
                var userTypeItems = userTypeDropdown.get_items();

                for (var i = 0; i < schoolItems.get_count() ; i++) {
                    checkbox = $get(schoolDropdown.get_id() + '_i' + i + '_schoolCheckbox');
                    labelValue = $get(schoolDropdown.get_id() + '_i' + i + '_schoolLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                    if (checkbox.checked) {
                        anySchoolItemsChecked = true;
                        break;
                    }
                }

                for (var i = 0; i < userTypeItems.get_count() ; i++) {
                    checkbox = $get(userTypeDropdown.get_id() + '_i' + i + '_userTypeCheckbox');
                    labelValue = $get(userTypeDropdown.get_id() + '_i' + i + '_userTypeLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                    if (checkbox.checked) {
                        anyUserTypeItemsChecked = true;
                        break;
                    }
                }

                if ($('#firstName').attr('value').replace(/ /g, '').length > 0 && $('#lastName').attr('value').replace(/ /g, '').length > 0 &&
                    $('#loginID').attr('value').replace(/ /g, '').length > 0 && anySchoolItemsChecked && anyUserTypeItemsChecked && primarySchool.get_selectedItem().get_text() != '' &&
                    primaryUser.get_selectedItem().get_text() != '') {
                    document.getElementById('updateStaffButtonTrigger').click();
                }
                else {
                    staffEditAlert('Please enter a value for all required fields.', 300, 100);
                }
            }

            function cancelUpdate() {
                if (modalWin) setTimeout(function () { modalWin.close(); }, 0);
            }

            /*************************************************************
            PASSWORD AND PHOTO CHANGE CODE BELOW
        *************************************************************/
            //Initialize view/edit divs to view mode
            pageMyAccount.ready = true;
            if (pageMyAccount.ready) {
                mapControls(pageMyAccount);
                toggleEditMode(false, null, null, null);
                pageMyAccount.divUpdateResults.style.display = "block";
                if (pageMyAccount.lblUpdateResults.innerText != "Update was successful.") {
                    if (pageMyAccount.WhichBtnPosted == "rbPasswordSave") {
                        toggleEditMode(true, pageMyAccount.divPassword, pageMyAccount.divPasswordEdit);
                    }
                }
            }

            function mapControls(oParent) {
                // Set up object for easy tracking of our HTML controls
                // Carry out any initialization needed.

                //Password controls
                oParent.divPassword = $$('divPassword');
                oParent.divPasswordEdit = $$('divPasswordEdit');
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

            function toggleEditMode(bEditMode, oViewDiv, oEditDiv, oFocusCtrl) {

                if (!bEditMode) {  // reset all controls to "View" mode
                    //Set all divs back to view mode.
                    //Set view divs on
                    pageMyAccount.divPassword.style.display = "block";
                    pageMyAccount.divPhoto.style.display = "block";

                    //Set edit divs off
                    pageMyAccount.divPasswordEdit.style.display = "none";
                    pageMyAccount.divPhotoEdit.style.display = "none";
                    pageMyAccount.divCopyrightWarning.style.display = "none";

                    //set button divs enabled
                    toggleAnchorInactive(pageMyAccount.hlEditPassword, false);
                    toggleAnchorInactive(pageMyAccount.hlEditPhoto, false);

                } else {
                    //Set up div for edit per button that was clicked.
                    //but leave other divs in "view mode and disable the buttons.

                    //disable buttons.
                    toggleAnchorInactive(pageMyAccount.hlEditPassword, true);
                    toggleAnchorInactive(pageMyAccount.hlEditPhoto, true);

                    if (oEditDiv.getAttribute("id") == "divPhotoEdit") {
                        pageMyAccount.divCopyrightWarning.style.display = "block";
                    }

                    //Set edit div as visible
                    oEditDiv.style.display = "block";
                    oViewDiv.style.display = "none";
                    lblResultMessage.innerText = "";

                    //Set focus

                    //if (oFocusCtrl != null) oFocusCtrl.focus();
                }
            }

            function toggleAnchorInactive(oAnchor, disable) {
                if (disable) {
                    oAnchor.style.display = 'none';
                }
                else {
                    oAnchor.style.display = '';
                }
            }

            function validateFields() {
                var isError = false;
                var e = event.srcElement || event.target;
                pageMyAccount.WhichBtnPosted = e.name;
                switch (pageMyAccount.WhichBtnPosted) {
                    /*************************/
                    case "rbPasswordSave":
                        /*************************/

                        //Clear out any preexisting error messages.
                        pageMyAccount.lblNewPwdErrorMessage.textContent = "";
                        pageMyAccount.lblRetypePwdErrorMessage.textContent = "";

                        if (isStringNullOrEmpty(pageMyAccount.rtbNewPwd.value)) {
                            pageMyAccount.lblNewPwdErrorMessage.textContent = "Please enter a new password.";
                        }
                        else if ($.trim($("#rtbNewPwd").val()) != "" && $("#hdnPasswordConfReq").val() == "Yes") {
                            var passwordReg = new RegExp($("#hdnPasswordFormatReg").val());
                            var password = $("#rtbNewPwd").val();
                            if (password.match(passwordReg) == null) {
                                $('#lblNewPwdErrorMessage').text($("#hdnPasswordValidationMsg").val());
                                isError = true;
                            }
                        }

                        if (isStringNullOrEmpty(pageMyAccount.rtbRetypePwd.value)) pageMyAccount.lblRetypePwdErrorMessage.innerText = "Please retype the new password.";

                        if (pageMyAccount.lblNewPwdErrorMessage.textContent != "" || pageMyAccount.lblRetypePwdErrorMessage.textContent != "") {
                            isError = true;
                        } else {
                            if (pageMyAccount.rtbRetypePwd.value != pageMyAccount.rtbNewPwd.value) {
                                pageMyAccount.lblRetypePwdErrorMessage.textContent = "New and retyped passwords do not match.";
                                isError = true;
                            }
                        }

                        break;

                        /*************************/
                    case "rbPhotoSave":
                        /*************************/

                        //Clear out any preexisting error messages.
                        pageMyAccount.lblPhotoErrorMessage.innerText = "";

                        //Test for file type. if not in list of file types expected then display message
                        //                        if (!$find('ruUserImage').validateExtensions()) {  //Could not find the validateExtensions function using pageMyAccount.ruUseImage.
                        //                            pageMyAccount.lblPhotoErrorMessage.innerText = "The file being uploaded must have an extension of .jpg, .png, or .gif";
                        //                            isError = true;
                        //                        }
                        break;

                }

                return isError;
            }

            function isStringNullOrEmpty(str) {
                if (str == null || str == "") {
                    return true;
                }

                return false;
            }



            function UpdateAccountInfo(sender, args) {
                pageMyAccount.lblUpdateResults.textContent = "";
                pageMyAccount.divUpdateResults.style.display = "none";
                //if (!validateFields()) {
                __doPostBack("rbPhotoSave", '');
                //}
            }



            $(document).ready(function () {

                $("#h1MinimumPassword").on("click", function () {
                    window.parent.PasswordRequirement.ShowDialog($("#displayMsgChild").html());
                });
                jQuery('#<%= rbPasswordSave.ClientID %>').click(function () {
                    var isError = false;
                    //Clear out any preexisting error messages.
                    pageMyAccount.lblNewPwdErrorMessage.textContent = "";
                    pageMyAccount.lblRetypePwdErrorMessage.textContent = "";

                    if (isStringNullOrEmpty(pageMyAccount.rtbNewPwd.value)) pageMyAccount.lblNewPwdErrorMessage.textContent = "Please enter a new password.";
                    else if ($.trim($("#rtbNewPwd").val()) != "" && $("#hdnPasswordConfReq").val() == "Yes") {
                        var passwordReg = new RegExp($("#hdnPasswordFormatReg").val());
                        var password = $("#rtbNewPwd").val();
                        if (password.match(passwordReg) == null) {
                            $('#lblNewPwdErrorMessage').text($("#hdnPasswordValidationMsg").val());
                            isError = true;
                        }
                    }

                    if (isStringNullOrEmpty(pageMyAccount.rtbRetypePwd.value)) pageMyAccount.lblRetypePwdErrorMessage.textContent = "Please retype the new password.";

                    if (pageMyAccount.lblNewPwdErrorMessage.textContent != "" || pageMyAccount.lblRetypePwdErrorMessage.textContent != "") {
                        isError = true;
                    } else {
                        if (pageMyAccount.rtbRetypePwd.value != pageMyAccount.rtbNewPwd.value) {
                            pageMyAccount.lblRetypePwdErrorMessage.textContent = "New and retyped passwords do not match.";
                            isError = true;
                        }
                    }

                    if (!isError) {
                        __doPostBack('<%= rbPasswordSave.ClientID %>', '');
                    }

});

            });


            function resetToViewMode() {
                pageMyAccount.rtbNewPwd.value = "";
                pageMyAccount.lblNewPwdErrorMessage.innerText = "";
                pageMyAccount.rtbRetypePwd.value = "";
                pageMyAccount.lblRetypePwdErrorMessage.innerText = "";

                pageMyAccount.ruUserImage.value = "";
                pageMyAccount.lblPhotoErrorMessage.innerText = "";

                pageMyAccount.lblUpdateResults.innerText = "";
                pageMyAccount.WhichBtnPosted = "";
                pageMyAccount.divUpdateResults.style.display = "none";
                toggleEditMode(false, null, null, null);

            }

            /* We will either come through here when the page is first rendered, or when a postback occurs 
            *  due to user uploading a new image.  Therefore, it has to be able to handle both.  We can 
            *  tell the difference by seeing if lblUpdateResults has any text in it.  If so, then it's a 
            *  post back.  Otherwise, it is the initial rendering.
            */

            mapControls(pageMyAccount);
            if (!isStringNullOrEmpty(pageMyAccount.lblUpdateResults.innerText)) {
                pageMyAccount.divUpdateResults.style.display = "block";
                toggleEditMode(false, null, null, null);
                if (pageMyAccount.lblUpdateResults.innerText != "Update was successful.") {
                    toggleEditMode(true, pageMyAccount.divPhoto, pageMyAccount.divPhotoEdit, null);
                }
            }
            else {
                resetToViewMode();
            }

            /************************************************************
                END OF PASSWORD AND PHOTO CHANGE CODE
            ************************************************************/
        </script>
    </form>
</body>
</html>
