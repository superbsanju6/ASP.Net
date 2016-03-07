<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/AddNew.Master" CodeBehind="AddStaff.aspx.cs"
    Inherits="Thinkgate.Controls.Staff.AddStaff" Title="Add New Staff" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <style>
        .tableDiv
        {
            display: table;
        }
        
        .row
        {
            display: table-row;
        }
        
        .cellLabel
        {
            display: table-cell;
            width: 40%;
            height: 40px;
            text-align: left;
            font-size: 13pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
        }
        
        .cellValue
        {
            display: table-cell;
            height: 40px;
            text-align: left;
            font-size: 12pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
        }
        
        .textBox
        {
            border: solid 1px #000;
            padding: 2px;
            width: 250px;
        }
        
        .smallText
        {
            font-size: 9pt;
        }
        
        .radDropdownBtn
        {
            font-weight: bold;
            font-size: 11pt;
        }
        
        .labelForValidation
        {
            color: Red;
            display: block;
            font-size: 10pt;
        }
        
        .fieldLabel
        {
            font-weight: bold;
            font-size: 10pt;
            white-space: nowrap;
            text-align: left;
            width: 110px;
            height: 30px;
            vertical-align: top;
        }
        
        .fieldEntry
        {
            vertical-align: top;
            text-align: left;
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
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function StopPropagation(e) {
                //cancel bubbling
                e.cancelBubble = true;
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
            }

            function isStringNullOrEmpty(str) {
                if (str == null || str == "") {
                    return true;
                }

                return false;
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
                    for (var i = 1; i < items.get_count(); i++) {
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
                    for (var i = 1; i < items.get_count(); i++) {
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
                    for (var i = 0; i < items.get_count(); i++) {
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
//                        checkbox = $get(dropdownObj.get_id() + '_i0_' + dropdownType + 'Checkbox');
//                        checkbox.checked = true;
                        dropdownObj.set_text('Multiple');
                    }
                    else {
//                        checkbox = $get(dropdownObj.get_id() + '_i0_' + dropdownType + 'Checkbox');
//                        checkbox.checked = false;

                        switch (itemsCheckedCount) {
                            case 0:
                                dropdownObj.set_text('');  
                                break;
                            case 1:
                                for (var i = 0; i < items.get_count(); i++) {
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

            function userHasAddedData() {
                // This function returns a true if we detect that any of our fields have
                // had data added to them.
                
                if ($('#firstName').attr('value').replace(/ /g, '').length > 0) return true;
                if ($('#middleName').attr('value').replace(/ /g, '').length > 0) return true;
                if ($('#lastName').attr('value').replace(/ /g, '').length > 0) return true;
                if ($('#email').attr('value').replace(/ /g, '').length > 0) return true;
                if ($('#loginID').attr('value').replace(/ /g, '').length > 0) return true;
                if ($('#restrictionsDropdown')[0].value != 'None') return true;

                var schoolDropdown = $find("<%= schoolDropdown.ClientID %>");
                var userTypeDropdown = $find("<%= userTypeDropdown.ClientID %>");
                var checkbox;
                var labelValue;
                var schoolItems = schoolDropdown.get_items();
                var userTypeItems = userTypeDropdown.get_items();

                // Check for whether user has checked any schools.
                for (var i = 0; i < schoolItems.get_count(); i++) {
                    checkbox = $get(schoolDropdown.get_id() + '_i' + i + '_schoolCheckbox');
                    labelValue = $get(schoolDropdown.get_id() + '_i' + i + '_schoolLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                    if (checkbox.checked) {
                        return true;
                    }
                }

                // Check for whether user has checked any roles.
                for (var i = 0; i < userTypeItems.get_count(); i++) {
                    checkbox = $get(userTypeDropdown.get_id() + '_i' + i + '_userTypeCheckbox');
                    labelValue = $get(userTypeDropdown.get_id() + '_i' + i + '_userTypeLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                    if (checkbox.checked) {
                        return true;
                    }
                }

                return false;
            }

            function checkRequiredFields() {
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

                for (var i = 0; i < schoolItems.get_count(); i++) {
                    checkbox = $get(schoolDropdown.get_id() + '_i' + i + '_schoolCheckbox');
                    labelValue = $get(schoolDropdown.get_id() + '_i' + i + '_schoolLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                    if (checkbox.checked) {
                        anySchoolItemsChecked = true;
                        break;
                    }
                }

                for (var i = 0; i < userTypeItems.get_count(); i++) {
                    checkbox = $get(userTypeDropdown.get_id() + '_i' + i + '_userTypeCheckbox');
                    labelValue = $get(userTypeDropdown.get_id() + '_i' + i + '_userTypeLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                    if (checkbox.checked) {
                        anyUserTypeItemsChecked = true;
                        break;
                    }
                }

                if ($('#firstName').attr('value').replace(/ /g, '').length > 0 && 
                    $('#lastName').attr('value').replace(/ /g, '').length > 0 &&
                    $('#loginID').attr('value').replace(/ /g, '').length > 0 && 
                    primarySchool.get_selectedItem().get_text() != '' &&
                    primaryUser.get_selectedItem().get_text() != '' &&
                    anySchoolItemsChecked && 
                    anyUserTypeItemsChecked) {
                    return false;
                }
                else {
                    staffEditAlert('Please enter a value for all required fields.', 300, 100);
                    return true;
                }
            }

            function addStaff() {
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

            function openStaff() {
                var hdnNewStaffIDEncrypted = $('#hdnNewStaffIDEncrypted')[0];
                window.open("../../Record/Staff.aspx?xID=" + hdnNewStaffIDEncrypted.value);
            }


            /*****************************************************************************************
            * After Page has loaded, add the "onClientBeforeClose" delegate to our customDialog window
            *****************************************************************************************/
            $(function () {
                addWindowBeforeCloseEvent();
            });

        </script>
    </telerik:RadCodeBlock>

    <div style="overflow: hidden;">
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="addStaffLoadingPanel">
            <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
                <div id="divUpdateResults">
                    <asp:Label runat="server" ID="lblUpdateResults" CssClass="labelForValidation"></asp:Label></div>
                <div class="tableDiv">
                    <div class="row">
                        <div class="cellLabel">
                            First Name:</div>
                        <div class="cellValue">
                            <asp:TextBox ID="firstName" runat="server" ClientIDMode="Static" CssClass="textBox" /></div>
                    </div>
                    <div class="row">
                        <div class="cellLabel">
                            Middle Name:</div>
                        <div class="cellValue">
                            <asp:TextBox ID="middleName" runat="server" ClientIDMode="Static" CssClass="textBox" /><span
                                class="smallText"><br/>(Not Required)</span></div>
                    </div>
                    <div class="row">
                        <div class="cellLabel">
                            Last Name:</div>
                        <div class="cellValue">
                            <asp:TextBox ID="lastName" runat="server" ClientIDMode="Static" CssClass="textBox" /></div>
                    </div>
                    <div class="row">
                        <div class="cellLabel">
                            Email:</div>
                        <div class="cellValue">
                            <asp:TextBox ID="email" runat="server" ClientIDMode="Static" CssClass="textBox" /><span
                                class="smallText"><br/>(Not Required)</span></div>
                    </div>
                    <div class="row">
                        <div class="cellLabel">
                            Login ID:</div>
                        <div class="cellValue">
                            <asp:TextBox ID="loginID" runat="server" ClientIDMode="Static" CssClass="textBox" /></div>
                    </div>
                    <div class="row">
                        <div class="cellLabel">
                            School:</div>
                        <div class="cellValue">
                            <telerik:RadComboBox runat="server" ID="schoolDropdown" ClientIDMode="AutoID" Skin="Web20"
                                Text="" CssClass="radDropdownBtn" Width="250" AutoPostBack="false" EmptyMessage="Multiple"
                                HighlightTemplatedItems="true">
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
                            Primary School:</div>
                        <div class="cellValue">
                            <telerik:RadComboBox runat="server" ID="cmbPrimarySchool" ClientIDMode="AutoID" Skin="Web20"
                                Text="" CssClass="radDropdownBtn" Width="250" AutoPostBack="false" EmptyMessage="Select..."
                                HighlightTemplatedItems="true">
                                <ItemTemplate>
                                        <asp:Label runat="server" ID="lblPrimarySchool" ClientIDMode="AutoID"></asp:Label>
                                </ItemTemplate>
                            </telerik:RadComboBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cellLabel">
                            User Type:</div>
                        <div class="cellValue">
                            <telerik:RadComboBox runat="server" ID="userTypeDropdown" ClientIDMode="AutoID" Skin="Web20"
                                Text="" CssClass="radDropdownBtn" Width="250" AutoPostBack="false" EmptyMessage="Multiple"
                                HighlightTemplatedItems="true">
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
                        <div class="cellLabel">
                            Primary User Type:</div>
                        <div class="cellValue">
                            <telerik:RadComboBox runat="server" ID="cmbPrimaryUser" ClientIDMode="AutoID" Skin="Web20"
                                Text="" CssClass="radDropdownBtn" Width="250" AutoPostBack="false" EmptyMessage="Select..."
                                HighlightTemplatedItems="true">
                                <ItemTemplate>
                                        <asp:Label runat="server" ID="lblPrimaryUser" ClientIDMode="AutoID"></asp:Label>
                                </ItemTemplate>
                            </telerik:RadComboBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cellLabel">
                            Restrictions:</div>
                        <div class="cellValue">
                            <telerik:RadComboBox runat="server" ID="restrictionsDropdown" ClientIDMode="Static"
                                Skin="Web20" Text="" CssClass="radDropdownBtn" Width="250" AutoPostBack="false">
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
                        <div class="cellLabel"></div>
                        <div class="cellValue">
                            <asp:Button runat="server" ID="btnCancel" ClientIDMode="Static" CssClass="roundButtons" Text="Cancel" OnClientClick="closeWindow(); return false;"/>
                            &nbsp;
                            <asp:Button runat="server" ID="btnAddStaff" ClientIDMode="Static" CssClass="roundButtons" Text="   OK   " OnClientClick="addStaff(); return false;"/>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static">
                <asp:Label runat="server" ID="lblResultMessage" Text="" />
                <br />
                <telerik:RadButton runat="server" ID="RadButtonOpenStaff" Text="Open Staff" AutoPostBack="False" OnClientClicked="openStaff"  />
                &nbsp;
                <telerik:RadButton runat="server" ID="RadButtonClose" Text="Close" AutoPostBack="False" OnClientClicked="closeWindow"/>
            </asp:Panel>
            <input runat="server" id="hdnNewStaffIDEncrypted" type="hidden" value="" ClientIDMode="Static"/>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="addStaffLoadingPanel" runat="server" />
    </div>
</asp:Content>
