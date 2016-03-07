<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/AddNew.Master" CodeBehind="AddStudent.aspx.cs"
    Inherits="Thinkgate.Controls.Student.AddStudent" Title="Add New Student" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var requiredFields = [['#<%= TextBoxFirstName.ClientID %>', '#<%= LabelFirstNameErrorMessage.ClientID %>', 'Please enter a value for First Name.'],
                ['#<%= TextBoxLastName.ClientID %>', '#<%= LabelLastNameErrorMessage.ClientID %>', 'Please enter a value for Last Name.'],
                ['#<%= TextBoxStudentID.ClientID %>', '#<%= LabelStudentIDErrorMessage.ClientID %>', 'Please enter a value for Student ID.'],
                ['#<%= RadComboBoxGrade.ClientID %>', '#<%= LabelGradeErrorMessage.ClientID %>', 'Please select a value for Grade.'],
                ['#<%= RadComboBoxSchool.ClientID %>', '#<%= LabelSchoolErrorMessage.ClientID %>', 'Please select a value for School.'],
                ['#<%= cmbGender.ClientID %>', '#<%= lblGenderErrorMessage.ClientID %>', 'Please select a value for Gender.'],
                ['#<%= cmbRace.ClientID %>', '#<%= lblRaceErrorMessage.ClientID %>', 'Please select a value for Race.']];

            function userHasAddedData() {  //This function is used in AddNew.Master
                
                /* Iterate through required fields to see if user has modified these. */
                for (var i = 0; i < requiredFields.length; ++i) {
                    var inputValue = $.trim($(requiredFields[i][0]).val());
                    if (
                            (requiredFields[i][2].indexOf('Please enter') >= 0 && !isStringNullOrEmpty(inputValue))
                            || 
                            (requiredFields[i][2].indexOf('Please select') >= 0 && inputValue != "Select") 
                        ) return true;
                }

                /* Go through the rest of the textboxes to see if user has modified these.*/
                if (!isStringNullOrEmpty($('#<%= TextBoxMiddleName.ClientID %>').val())) return true;

                return false;
            }

            function selectTextBoxStudentID() {
                autoSizeWindow();
                $('#TextBoxStudentID').focus();
            }

            function addStudent() {
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

            function clearErrorMessages() {
                for (var i = 0; i < requiredFields.length; ++i) {
                    $(requiredFields[i][1]).text('');
                }
            }

            function checkRequiredFields() {
                var isError = false;

                for (var i = 0; i < requiredFields.length; ++i) {
                    var inputValue = $.trim($(requiredFields[i][0]).val());

                    if (isStringNullOrEmpty(inputValue) || inputValue == "Select") {
                        $(requiredFields[i][1]).text(requiredFields[i][2]);
                        isError = true;
                    }
                }

                return isError;
            }

            function openStudent() {
                window.open("../../Record/Student.aspx?childPage=yes&xID=" + document.getElementById('<%= TextBoxHiddenEncryptedStudentID.ClientID %>').value);
            }

            $(function () {
                addWindowBeforeCloseEvent(); //This and other common functions are in AddNew.Master
            });

        </script>
    </telerik:RadCodeBlock>
    <div style="overflow: hidden;">
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="addStudentLoadingPanel">
            <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static" >
                <table class="fieldValueTable fieldAddModalTable" style="width: 400px; margin-left: auto;
                    margin-right: auto;">
                    <tr>
                        <td class="fieldLabel">
                            First Name:
                        </td>
                        <td style="width: 200px;">
                            <telerik:RadTextBox Width="100%" runat="server" ID="TextBoxFirstName" />
                            <asp:Label runat="server" ID="LabelFirstNameErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Middle Name:
                        </td>
                        <td>
                            <telerik:RadTextBox Width="100%" runat="server" ID="TextBoxMiddleName" />
                            <div style="font-size: 12px;">
                                (Not Required)</div>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Last Name:
                        </td>
                        <td>
                            <telerik:RadTextBox Width="100%" runat="server" ID="TextBoxLastName" />
                            <asp:Label runat="server" ID="LabelLastNameErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Student ID:
                        </td>
                        <td>
                            <telerik:RadTextBox Width="100%" runat="server" ID="TextBoxStudentID" />
                            <asp:Label runat="server" ID="LabelStudentIDErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Grade:
                        </td>
                        <td>
                            <telerik:RadComboBox Width="100%" runat="server" ID="RadComboBoxGrade" />
                            <asp:Label runat="server" ID="LabelGradeErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            School:
                        </td>
                        <td>
                            <telerik:RadComboBox Width="100%" runat="server" ID="RadComboBoxSchool" />
                            <asp:Label runat="server" ID="LabelSchoolErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Gender:
                        </td>
                        <td>
                            <telerik:RadComboBox Width="100%" runat="server" ID="cmbGender" />
                            <asp:Label runat="server" ID="lblGenderErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Race:
                        </td>
                        <td>
                            <telerik:RadComboBox Width="100%" runat="server" ID="cmbRace" />
                            <asp:Label runat="server" ID="lblRaceErrorMessage" Text="" ForeColor="Red"></asp:Label>
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
                                OnClientClicked="addStudent" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static" >
                <asp:Label runat="server" ID="lblResultMessage" Text="" />
                <br />
                <telerik:RadButton runat="server" ID="RadButtonOpenStudent" Text="Open Student" AutoPostBack="False"
                    OnClientClicked="openStudent" />
                &nbsp;
                <telerik:RadButton runat="server" ID="RadButtonClose" Text="Close" AutoPostBack="False"
                    OnClientClicked="closeWindow" />
            </asp:Panel>
            <asp:TextBox ID="TextBoxHiddenEncryptedStudentID" runat="server" Style="visibility: hidden;
                display: none;" />
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="addStudentLoadingPanel" runat="server" />
    </div>
</asp:Content>
