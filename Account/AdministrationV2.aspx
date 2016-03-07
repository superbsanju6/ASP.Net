<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdministrationV2.aspx.cs" Inherits="Thinkgate.Account.AdministrationV2" %>

<%@ Register TagPrefix="uc" TagName="SearchGrid" Src="~/Account/SearchGrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchCombo" Src="~/Account/SearchCombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="GridList" Src="~/Account/GridList.ascx" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .ui-dialog-content li {
            font-size: 14px !important;
        }
    </style>
</head>
<body>
    <form runat="server">
        <div>
            <telerik:RadCodeBlock runat="server">
                <script src="../Scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
                <link href="../Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
                <script src="../Scripts/jquery-migrate-1.1.0.min.js" type="text/javascript"></script>
                <script src="../Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" type="text/javascript"></script>
                <script src="../Scripts/MinimumPasswordRequirement/MinimumPasswordRequirements.js" type="text/javascript"></script>
                <script type="text/javascript">
                    function onClientItemChecked(sender, eventArgs) {
                        var item = eventArgs.get_item();
                        var dropDown = sender._uniqueId == 'cmbCreateUserRoles' ? $find("cmbCreateUserPrimaryRole") : $find("cmbCreateUserPrimarySchool");
                        var comboItem = new Telerik.Web.UI.RadComboBoxItem();

                        if (item.get_checked()) {
                            comboItem.set_text(item.get_text());
                            dropDown.trackChanges();
                            comboItem.set_value(item.get_value());
                            dropDown.get_items().add(comboItem);
                            comboItem.select();
                            dropDown.commitChanges();
                        }
                        else {
                            comboItem.set_text(item.get_text());
                            dropDown.trackChanges();
                            comboItem.set_value(item.get_value());
                            dropDown.get_items().remove(dropDown.findItemByValue(item.get_value()));
                            dropDown.commitChanges();
                        }
                    }

                    function StandardConfirm(sender, args) {
                        args.set_cancel(!window.confirm("Are you sure you want to continue?"));
                    }
                    $(document).ready(function () {
                        $("#h1MinimumPassword").on("click", function () {
                            PasswordRequirement.ShowDialog($("#displayMsgChild").html());
                        });
                        $("#txtResetNew").val("");
                        $("#txtResetConfirm").val("");
                        $('#btnSpecial').click(function () {
                            if ($("#hdnPasswordConfReq").val() == "Yes") {
                                var newPassword = $.trim($("#txtResetNew").val());
                                var confirmPassword = $.trim($("#txtResetConfirm").val());
                                var passwordReg = new RegExp($("#hdnPasswordFormatReg").val());
                                if (newPassword == "") {
                                    $("#lblSpecial").text('Please enter a new password.');
                                    return false;
                                }
                                else if (newPassword.match(passwordReg) == null) {
                                    $("#lblSpecial").text($("#hdnPasswordValidationMsg").val());
                                    return false;
                                }
                                else if (confirmPassword == "") {
                                    $("#lblSpecial").text('Please enter a confirm password.');
                                    return false;
                                }
                                else if (newPassword != confirmPassword) {
                                    $("#lblSpecial").text("New and confirm passwords do not match.");
                                    return false;
                                }
                            }
                            return true;
                        });
                    });
                </script>
            </telerik:RadCodeBlock>
            <asp:HiddenField ID="hdnPasswordConfReq" Value="" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="hdnPasswordFormatReg" Value="" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="hdnPasswordValidationMsg" Value="" runat="server" ClientIDMode="Static" />
            <div id="displayMsgChild" style="display: none;" runat="server" clientidmode="Static">
            </div>
            <telerik:RadScriptManager runat="server"></telerik:RadScriptManager>
            <telerik:RadTabStrip runat="server" UnSelectChildren="true" ID="rtsMain" OnTabClick="rtsMain_TabClick" MultiPageID="rmpMain" Skin="Black">
                <Tabs>
                    <telerik:RadTab runat="server" Text="Permissions" PageViewID="rpvBlank">
                        <Tabs>
                            <telerik:RadTab ID="tabPermissionList" runat="server" Text="Permission List" PageViewID="rpvList" />
                            <telerik:RadTab ID="tabPermissionCreate" runat="server" Text="Create Permission" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab ID="tabPermissionEdit" runat="server" Text="Edit Permission" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab ID="tabPermissionRoles" runat="server" Text="Roles in Permission" PageViewID="rpvEnableGrid" />
                            <telerik:RadTab ID="tabPermissionUsers" runat="server" Text="Users in Permission" PageViewID="rpvEnableGrid" />
                        </Tabs>
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="Roles" PageViewID="rpvBlank">
                        <Tabs>
                            <telerik:RadTab ID="tabRoleList" runat="server" Text="Role List" PageViewID="rpvList" />
                            <telerik:RadTab ID="tabRoleCreate" runat="server" Text="Create Role" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab ID="tabRoleEdit" runat="server" Text="Edit Role" PageViewID="rpvEdit" />
                            <telerik:RadTab ID="tabRolePermissions" runat="server" Text="Role Permissions" PageViewID="rpvEnableGrid" />
                            <telerik:RadTab ID="tabRoleHiearchy" runat="server" Text="Role Hierarchy" PageViewID="rpvSortBox" />
                        </Tabs>
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="Users" PageViewID="rpvBlank">
                        <Tabs>
                            <telerik:RadTab ID="tabUserList" runat="server" Text="User List" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab ID="tabUserCreate" runat="server" Text="Create User" PageViewID="rpvCreate" />
                            <telerik:RadTab ID="tabUserEdit" runat="server" Text="Edit User" PageViewID="rpvEdit" />
                            <telerik:RadTab ID="tabUserResetPassword" runat="server" Text="Reset Password" PageViewID="rpvSpecial" />
                            <telerik:RadTab ID="tabUserMultiEdit" runat="server" Text="Multi-User Edit" PageViewID="rpvDevInProgress">
                                <Tabs>
                                    <telerik:RadTab ID="tabUserMultiPasswords" runat="server" Text="Reset Passwords" PageViewID="rpvDevInProgress" />
                                    <telerik:RadTab ID="tabUserMultiRoles" runat="server" Text="Roles" PageViewID="rpvDevInProgress" />
                                    <telerik:RadTab ID="tabUserMultiSchools" runat="server" Text="Schools" PageViewID="rpvDevInProgress" />
                                </Tabs>
                            </telerik:RadTab>
                            <telerik:RadTab ID="tabUserRoles" runat="server" Text="User Roles" PageViewID="rpvEnableGrid" />
                            <telerik:RadTab ID="tabUserPermissions" runat="server" Text="User Permissions" PageViewID="rpvEnableGrid" />
                            <telerik:RadTab ID="tabUserSchools" runat="server" Text="User Schools" PageViewID="rpvEnableGrid" />
                        </Tabs>
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="Schools" PageViewID="rpvBlank">
                        <Tabs>
                            <telerik:RadTab ID="tabSchoolList" runat="server" Text="School List" PageViewID="rpvList" />
                            <telerik:RadTab ID="tabSchoolCreate" runat="server" Text="Create School" PageViewID="rpvCreate" />
                            <telerik:RadTab ID="tabSchoolEdit" runat="server" Text="Edit School" PageViewID="rpvEdit" />
                        </Tabs>
                    </telerik:RadTab>
                    <telerik:RadTab ID="tabPricingModules" runat="server" Text="Pricing Modules" PageViewID="rpvBlank">
                        <Tabs>
                            <telerik:RadTab ID="tabPricingModuleList" runat="server" Text="Pricing Module List" PageViewID="rpvList" />
                            <telerik:RadTab ID="tabPricingModuleStatus" runat="server" Text="Pricing Module Status" PageViewID="rpvEnableGrid" />
                        </Tabs>
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="District Setup" PageViewID="rpvBlank">
                        <Tabs>
                            <telerik:RadTab runat="server" Text="Parms" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab runat="server" Text="Grades" PageViewID="rpvBlank">
                                <Tabs>
                                    <telerik:RadTab ID="tabGradeList" Text="Grade List" PageViewID="rpvList" />
                                    <telerik:RadTab ID="tabGradeCreate" Text="Create Grade" PageViewID="rpvDevInProgress" />
                                    <telerik:RadTab ID="tabGradeEdit" Text="Edit Grade" PageViewID="rpvDevInProgress" />
                                </Tabs>
                            </telerik:RadTab>
                            <telerik:RadTab runat="server" Text="Subjects" PageViewID="rpvBlank">
                                <Tabs>
                                    <telerik:RadTab ID="tabSubjectList" runat="server" Text="Subject List" PageViewID="rpvList" />
                                    <telerik:RadTab ID="tabSubjectCreate" runat="server" Text="Create Subject" PageViewID="rpvCreate" />
                                    <telerik:RadTab ID="tabSubjectEdit" runat="server" Text="Edit Subject" PageViewID="rpvEdit" />
                                </Tabs>
                            </telerik:RadTab>
                            <telerik:RadTab runat="server" Text="Periods" PageViewID="rpvDevInProgress">
                                <Tabs>
                                    <telerik:RadTab ID="tabPeriodList" runat="server" Text="Period List" PageViewID="rpvDevInProgress" />
                                    <telerik:RadTab ID="tabPeriodCreate" runat="server" Text="Create Period" PageViewID="rpvDevInProgress" />
                                    <telerik:RadTab ID="tabPeriodEdit" runat="server" Text="Edit Period" PageViewID="rpvDevInProgress" />
                                </Tabs>
                            </telerik:RadTab>
                            <telerik:RadTab runat="server" Text="Test Terms" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab runat="server" Text="Test Types" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab runat="server" Text="Semesters" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab runat="server" Text="Years" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab runat="server" Text="Courses" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab runat="server" Text="Curriculum" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab runat="server" Text="Standards" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab runat="server" Text="Curriculum Directory" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab runat="server" Text="Standards Directory" PageViewID="rpvDevInProgress" />
                            <telerik:RadTab runat="server" Text="Performance Levels" PageViewID="rpvDevInProgress" />
                        </Tabs>
                    </telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="rmpMain" runat="server">
                <telerik:RadPageView ID="rpvBlank" Style="margin-top: 20px;" runat="server" />
                <telerik:RadPageView ID="rpvDevInProgress" Style="margin-top: 20px;" runat="server">
                    <asp:Label runat="server" Text="In Development..." Style="vertical-align: middle;"></asp:Label>
                </telerik:RadPageView>
                <telerik:RadPageView ID="rpvSlowWarning" Style="margin-top: 20px;" runat="server">
                    <asp:Label runat="server" Text="Warning! Running any of the following methods will take a long time when first clicked. Please be patient!" ForeColor="Red" Width="100%" Height="100%"></asp:Label>
                </telerik:RadPageView>
                <telerik:RadPageView ID="rpvList" Style="margin-top: 20px;" runat="server">
                    <uc:GridList ID="gdlList" runat="server" Mode="None"></uc:GridList>
                </telerik:RadPageView>
                <telerik:RadPageView ID="rpvSortBox" Style="margin-top: 20px;" runat="server">
                    <asp:Label ID="lblSortInstructions" Width="100%" runat="server" Text="The highest is the most privileged. The lowest is the least privileged."></asp:Label>
                    <telerik:RadListBox ID="lbxSort" Style="width: 500px; margin-top: 10px;" runat="server" AllowReorder="True" Skin="WebBlue"></telerik:RadListBox>
                    <div style="width: 475px;">
                        <telerik:RadButton ID="btnUpdate" Style="float: right; margin-right: 5px; margin-top: 5px;" runat="server" Text="Update" OnClick="btnUpdate_Click" Skin="Black" />
                    </div>
                    <asp:Label ID="lblSort" runat="server" ForeColor="Red"></asp:Label>
                </telerik:RadPageView>
                <telerik:RadPageView ID="rpvCreate" Style="margin-top: 20px;" runat="server">
                    <asp:Panel ID="pnlUserCreate" Visible="false" Style="margin-top: 10px; width: 500px;" runat="server" GroupingText="Create User">
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="First Name:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtCreateUserFirstName" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Middle Name:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtCreateUserMiddleName" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Last Name:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtCreateUserLastName" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="LoginID:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtCreateUserUserName" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Email:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtCreateUserEmail" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Teacher ID:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtCreateUserTeacherID" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Roles:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadComboBox ID="cmbCreateUserRoles" ClientIDMode="AutoID" OnClientItemChecked="onClientItemChecked" AutoPostBack="false" CheckBoxes="true" EmptyMessage="Multiple" Style="margin-left: 10px;" runat="server" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Primary Role:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadComboBox ID="cmbCreateUserPrimaryRole" EmptyMessage="Select..." Style="margin-left: 10px;" runat="server" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Schools:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadComboBox ID="cmbCreateUserSchools" OnClientItemChecked="onClientItemChecked" AutoPostBack="false" CheckBoxes="true" EmptyMessage="Multiple" Style="margin-left: 10px;" runat="server" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Primary School:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadComboBox ID="cmbCreateUserPrimarySchool" EmptyMessage="Select..." Style="margin-left: 10px;" runat="server" />
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlSchoolCreate" Style="margin-top: 10px; width: 500px;" runat="server" Visible="false" GroupingText="Create School">
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="School Name:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSchoolName" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Abbreviation:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSchoolAbbreviation" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Phone:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSchoolPhone" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Cluster:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSchoolCluster" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="School Type:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadComboBox ID="cmbSchoolType" EmptyMessage="Select..." Style="margin-left: 10px;" runat="server" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="School ID:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSchoolID" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlSubjectCreate" Style="margin-top: 10px; width: 500px;" runat="server" Visible="false" GroupingText="Create Subject">
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Name:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSubjectName" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Abbreviation:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSubjectAbbreviation" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                    </asp:Panel>
                    <div style="width: 500px;">
                        <telerik:RadButton ID="btnCreate" Style="float: right; margin-right: 5px; margin-top: 5px;" runat="server" Text="Create" OnClick="btnCreate_OnClick" Skin="Black" />
                    </div>
                    <asp:Label ID="lblCreate" runat="server" ForeColor="Red" Width="100%" Style="text-align: left;"></asp:Label>
                </telerik:RadPageView>
                <telerik:RadPageView ID="rpvEdit" Style="margin-top: 20px;" runat="server">
                    <uc:SearchCombo ID="ComboEdit" runat="server"></uc:SearchCombo>
                    <uc:SearchGrid ID="SearchEdit" runat="server"></uc:SearchGrid>
                    <asp:Panel ID="pnlEditRole" runat="server" BorderStyle="Solid" BorderWidth="1" Visible="false" Style="float: left; width: 500px;">
                        <div style="width: 40%; float: left; margin-bottom: 5px;">
                            <asp:Label Style="float: right;" runat="server" Text="Role Name:"></asp:Label>
                        </div>
                        <div style="width: 59%; float: right; margin-bottom: 5px;">
                            <asp:Label ID="lblRoleName" runat="server"></asp:Label>
                        </div>
                        <div style="width: 40%; float: left; margin-bottom: 5px;">
                            <asp:Label Style="float: right;" runat="server" Text="Description:"></asp:Label>
                        </div>
                        <div style="width: 59%; float: right; margin-bottom: 5px;">
                            <asp:TextBox ID="txtRoleDescription" runat="server"></asp:TextBox>
                        </div>
                        <div style="width: 40%; float: left; margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label Style="float: right;" runat="server" Text="Portal:"></asp:Label>
                        </div>
                        <div style="width: 59%; float: right; margin-bottom: 5px;">
                            <telerik:RadComboBox ID="cmbEditRolePortal" runat="server" />
                        </div>
                        <div style="width: 40%; float: left; margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label Style="float: right;" runat="server" Text="Active:"></asp:Label>
                        </div>
                        <div style="width: 59%; float: right; margin-bottom: 5px;">
                            <asp:CheckBox ID="chkEditRoleActive" runat="server"></asp:CheckBox>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlEditUser" Style="margin-top: 10px; width: 500px;" runat="server" BorderWidth="1" Visible="false">
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="First Name:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtUserEditFirstName" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Middle Name:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtUserEditMiddleName" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Last Name:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtUserEditLastName" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="LoginID:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtUserEditLoginID" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Email:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtUserEditEmail" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Teacher ID:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtUserEditTeacherID" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Is Approved:" Width="45%" Style="text-align: right;"></asp:Label>
                            <asp:CheckBox ID="chkUserEditIsApproved" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Is Locked:" Width="45%" Style="text-align: right;"></asp:Label>
                            <asp:CheckBox ID="chkUserEditIsLocked" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Created Date:" Width="45%" Style="text-align: right;"></asp:Label>
                            <asp:Label ID="lblUserEditCreatedDate" runat="server" Width="50%" Style="margin-left: 10px;"></asp:Label>
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Last Login:" Width="45%" Style="text-align: right;"></asp:Label>
                            <asp:Label ID="lblUserEditLastLogin" runat="server" Width="50%" Style="margin-left: 10px;"></asp:Label>
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Last Password Change:" Width="45%" Style="text-align: right;"></asp:Label>
                            <asp:Label ID="lblUserEditLastPasswordChange" runat="server" Width="50%" Style="margin-left: 10px;"></asp:Label>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlEditSchool" Style="margin-top: 10px; width: 500px;" runat="server" Visible="false" GroupingText="Edit School">
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="School Name:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSchoolEditName" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Abbreviation:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSchoolEditAbbr" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Phone:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSchoolEditPhone" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Cluster:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSchoolEditCluster" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="School Type:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadComboBox ID="cmbSchoolEditType" EmptyMessage="Select..." Style="margin-left: 10px;" runat="server" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="School ID:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtSchoolEditID" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlEditSubject" Style="margin-top: 10px; width: 500px;" runat="server" Visible="false" GroupingText="Edit Subject">
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Name:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtEditSubjectName" MaxLength="50" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Abbreviation:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtEditSubjectAbbreviation" MaxLength="10" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                    </asp:Panel>
                    <div style="width: 500px;">
                        <telerik:RadButton ID="btnEdit" Style="float: right; margin-right: 5px; margin-top: 5px;" Visible="false" runat="server" Text="Save" OnClick="btnEdit_OnClick" Skin="Black" />
                        <asp:Label ID="lblEdit" ForeColor="Red" runat="server" Width="100%" Style="margin: 5px; text-align: left;"></asp:Label>
                    </div>
                </telerik:RadPageView>
                <telerik:RadPageView ID="rpvMulti" Style="margin-top: 20px;" runat="server" />
                <telerik:RadPageView ID="rpvEnableGrid" Style="margin-top: 20px;" runat="server">
                    <uc:SearchCombo ID="ComboEnable" runat="server"></uc:SearchCombo>
                    <uc:SearchGrid ID="SearchEnable" runat="server"></uc:SearchGrid>
                    <telerik:RadGrid ID="grdEnable" OnPdfExporting="grid_PdfExporting" Visible="false" runat="server" OnColumnCreated="gridEnable_ColumnCreated" Style="width: 500px; margin-top: 10px;" OnItemDataBound="gridEnable_ItemDataBound">
                        <ClientSettings>
                            <Scrolling AllowScroll="true" SaveScrollPosition="true" ScrollHeight="600px" />
                        </ClientSettings>
                    </telerik:RadGrid>
                    <div style="width: 500px;">
                        <telerik:RadButton ID="btnEnable" Style="float: right; margin-right: 5px; margin-top: 5px;" Visible="false" runat="server" Text="Save" OnClick="btnEnable_OnClick" Skin="Black" />
                        <telerik:RadButton ID="btnPrint" Style="float: right; margin-right: 5px; margin-top: 5px;" Visible="false" runat="server" Text="Print" OnClick="btnPrint_OnClick" Skin="Black" />
                    </div>
                    <asp:Label ID="lblEnable" runat="server" ForeColor="Red"></asp:Label>
                </telerik:RadPageView>
                <telerik:RadPageView ID="rpvSpecial" Style="margin-top: 20px;" runat="server">
                    <uc:SearchCombo ID="ComboSpecial" runat="server"></uc:SearchCombo>
                    <uc:SearchGrid ID="SearchSpecial" runat="server"></uc:SearchGrid>
                    <asp:Panel ID="pnlResetPassword" Style="margin-top: 10px; width: 500px;" runat="server" Visible="false" GroupingText="Reset Password">
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Selected User:" Width="45%" Style="text-align: right;"></asp:Label>
                            <asp:Label ID="lblResetUser" runat="server" Width="50%" Style="margin-left: 10px;"></asp:Label>
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="New Password:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtResetNew" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <asp:Label runat="server" Text="Confirm Password:" Width="45%" Style="text-align: right;"></asp:Label>
                            <telerik:RadTextBox ID="txtResetConfirm" runat="server" Width="50%" Style="margin-left: 10px;" />
                        </div>
                        <div style="margin-bottom: 5px; margin-top: 5px;">
                            <a runat="server" id="h1MinimumPassword" clientidmode="Static" style="color:blue;text-decoration:underline;cursor:pointer">Minimum Password Requirements</a>
                        </div>
                    </asp:Panel>
                    <div style="width: 500px;">
                        <telerik:RadButton ID="btnSpecial" Style="float: right; margin-right: 5px; margin-top: 5px;" Visible="false" runat="server" Text="Save" OnClick="btnSpecial_OnClick" Skin="Black" />
                    </div>
                    <asp:Label ID="lblSpecial" runat="server" ForeColor="Red"></asp:Label>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
            <div style="float: right; display: none;">
                <asp:HyperLink ID="hlkGemBox" runat="server" Visible="false" NavigateUrl="http://www.gemboxsoftware.com/" Text="Gembox Software"></asp:HyperLink>
            </div>
        </div>
    </form>
</body>
</html>
