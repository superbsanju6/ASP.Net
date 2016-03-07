<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/AddNew.Master" CodeBehind="AddLCOStaff.aspx.cs"
    Inherits="Thinkgate.Controls.LCO.AddLCOStaff" Title="Add New Staff" %>

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
                var dropdownObj = $find("<%= cmbSection.ClientID %>");
                var itemChecked = item.checked;
                var itemValue = $get(item.id.replace(dropdownType + 'Checkbox', dropdownType + 'Label')).innerHTML;
                var allItemsChecked = true;
                var itemsCheckedCount = 0;
                var checkbox;
                var labelValue;
                var items = dropdownObj.get_items();

                if (itemChecked && itemValue == 'All') {
                    for (var i = 1; i < items.get_count(); i++) {
                        checkbox = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Checkbox');
                        labelValue = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Label').innerHTML;
                        if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;
                        checkbox.checked = true;
                    }

                    dropdownObj.set_text('Multiple');
                }
                else if ((itemValue == 'All' || itemValue == '') && !itemChecked) {
                    for (var i = 1; i < items.get_count(); i++) {
                        checkbox = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Checkbox');
                        labelValue = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Label').innerHTML;
                        if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;
                        checkbox.checked = false;
                    }

                    dropdownObj.set_text('');
                }
                else {
                    for (var i = 1; i < items.get_count(); i++) {
                        checkbox = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Checkbox');
                        labelValue = $get(dropdownObj.get_id() + '_i' + i + '_' + dropdownType + 'Label').innerHTML;
                        if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                        if (!checkbox.checked) {
                            allItemsChecked = false;
                        }
                        else {
                            itemsCheckedCount++;
                        }
                    }

                    if (allItemsChecked) {
                        checkbox = $get(dropdownObj.get_id() + '_i0_' + dropdownType + 'Checkbox');
                        checkbox.checked = true;
                        dropdownObj.set_text('Multiple');
                    }
                    else {
                        checkbox = $get(dropdownObj.get_id() + '_i0_' + dropdownType + 'Checkbox');
                        checkbox.checked = false;

                        switch (itemsCheckedCount) {
                            case 0:
                                dropdownObj.set_text('');
                                break;
                            case 1:
                                for (var i = 1; i < items.get_count(); i++) {
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
                if ($('#userTypeDropdown')[0].value != 'Select...') return true;
                if ($('#cmbRegion')[0].value != 'Select...') return true;
                if ($('#cmbLEA')[0].value != 'Select...') return true;
                if ($('#cmbSection')[0].value != 'Select...') return true;
                return false;
            }

            function checkSectionDropDown() {
                var cmbDropdown = $find("<%= cmbSection.ClientID %>");
                var userTypeItems = cmbDropdown.get_items();
                for (var i = 1; i < userTypeItems.get_count(); i++) {
                    checkbox = $get(cmbDropdown.get_id() + '_i' + i + '_chkPA');
                    if (checkbox.checked) {
                        return true;
                    }
                }
                return false;
            }
            

            function checkRequiredFields() {
                if ($('#firstName').attr('value').replace(/ /g, '').length > 0 && 
                    $('#lastName').attr('value').replace(/ /g, '').length > 0 &&
                    $('#loginID').attr('value').replace(/ /g, '').length > 0 &&
                    $('#loginID').attr('value').replace(/ /g, '').length > 0 &&
                    $('#restrictionsDropdown')[0].value != '' &&
                    (($('#userTypeDropdown')[0].value == 'IMC' && $('#cmbLEA')[0].value != '') ||
                       ($('#userTypeDropdown')[0].value == 'RegionalCoordinator' && $('#cmbRegion')[0].value != '') ||
                       ($('#userTypeDropdown')[0].value == 'LCOAdministrator') ||
                       checkSectionDropDown())) {
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

            function CloseCourse() {
                window.close();
                parent.location.reload();
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
                    <div id="divUserType" class="row">
                        <div class="cellLabel">
                            UserRole:</div>
                        <div class="cellValue">
                            <telerik:RadComboBox ID="userTypeDropdown" runat="server" AutoPostBack="true" 
                                ClientIDMode="Static" OnSelectedIndexChanged="userTypeDropdown_SelectedIndexChanged" CssClass="radDropdownBtn" EmptyMessage="Select..." 
                                HighlightTemplatedItems="true" Skin="Web20" Text="" Width="250">
                            </telerik:RadComboBox>
                        </div>
                    </div>
                    <div runat=server id=divLEA class="row" visible=false>
                        <div class="cellLabel">
                            LEA:</div>
                        <div class="cellValue">
                            <telerik:RadComboBox ID="cmbLEA" runat="server" AutoPostBack="false" 
                                ClientIDMode="Static" CssClass="radDropdownBtn" EmptyMessage="Select..." 
                                HighlightTemplatedItems="true" Skin="Web20" Text="" Width="250">
                            </telerik:RadComboBox>
                        </div>
                    </div>
                    <div runat=server id=divRegion class="row" visible=false>
                        <div class="cellLabel">
                            Region:</div>
                        <div class="cellValue">
                            <telerik:RadComboBox ID="cmbRegion" runat="server" AutoPostBack="false" 
                                ClientIDMode="Static" CssClass="radDropdownBtn" EmptyMessage="Select..." 
                                HighlightTemplatedItems="true" Skin="Web20" Text="" Width="250">
                            </telerik:RadComboBox>
                        </div>
                    </div>
                    <div runat=server id=divSection class="row" visible=false>
                        <div class="cellLabel">
                            Program Area:</div>
                        <div class="cellValue">
                            <telerik:RadComboBox runat="server" ID="cmbSection" ClientIDMode="AutoID"
                                Skin="Web20" Text="" CssClass="radDropdownBtn" Width="250" AutoPostBack="false" HighlightTemplatedItems=true EmptyMessage="Select...">
                                  <ItemTemplate>
                                    <div>
                                        <asp:CheckBox runat="server" ID="chkPA" ClientIDMode="AutoID" />
                                        <asp:Label runat="server" ID="lblPA" ClientIDMode="AutoID" AssociatedControlID="chkPA">
                                        </asp:Label>
                                    </div>
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
                <telerik:RadButton runat="server" ID="RadButtonClose" Text="Close" AutoPostBack="False" OnClientClicked="CloseCourse"/>
            </asp:Panel>
            <input runat="server" id="hdnNewStaffIDEncrypted" type="hidden" value="" clientidmode="Static"/>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="addStaffLoadingPanel" runat="server" />
    </div>
</asp:Content>
