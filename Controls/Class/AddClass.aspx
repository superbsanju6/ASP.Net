<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/AddNew.Master" CodeBehind="AddClass.aspx.cs" Inherits="Thinkgate.Controls.Class.AddClass" Title="Add New Class" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var requiredFields = [['#<%= RadComboBoxSchool.ClientID %>', '#<%= LabelSchoolErrorMessage.ClientID %>', 'Please select a value for School.'],
                ['#<%= RadComboBoxGrade.ClientID %>', '#<%= LabelGradeErrorMessage.ClientID %>', 'Please select a value for Grade.'],
                ['#<%= RadComboBoxSubject.ClientID %>', '#<%= LabelSubjectErrorMessage.ClientID %>', 'Please select a value for Subject.'],
                ['#<%= RadComboBoxCourse.ClientID %>', '#<%= LabelCourseErrorMessage.ClientID %>', 'Please select a value for Course.'],
                ['#<%= RadComboBoxYear.ClientID %>', '#<%= LabelYearErrorMessage.ClientID %>', 'Please select a value for Year.'],
                ['#<%= RadComboBoxSemester.ClientID %>', '#<%= LabelSemesterErrorMessage.ClientID %>', 'Please select a value for Semester.'],
                ['#<%= RadComboBoxPeriod.ClientID %>', '#<%= LabelPeriodErrorMessage.ClientID %>', 'Please select a value for Period.']];

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

                return false;
            }

            function addClass() {
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

            function openClass() {
                window.open("../../Record/Class.aspx?xID=" + document.getElementById('<%= TextBoxHiddenEncryptedClassID.ClientID %>').value);
            }

            $(function () {
                addWindowBeforeCloseEvent();  //This and other common functions are in AddNew.Master
            });
        </script>
    </telerik:RadCodeBlock>
    <div style="overflow: hidden;">
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="addClassLoadingPanel">
            <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
                <asp:Label runat="server" ID="LabelGenericErrorMessage" Text="" ForeColor="Red"></asp:Label>
                <table class="fieldValueTable fieldAddModalTable" style="width: 400px; margin-left: auto;
                    margin-right: auto;">
                    <tr>
                        <td class="fieldLabel">
                            School:
                        </td>
                        <td style="width: 160px;">
                            <telerik:RadComboBox runat="server" ID="RadComboBoxSchool" />
                            <asp:Label runat="server" ID="LabelSchoolErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Grade:
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="RadComboBoxGrade" OnSelectedIndexChanged="RadComboBoxGradeSelectedIndexChanged"
                                AutoPostBack="true" />
                                <asp:Label runat="server" ID="LabelGradeErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Subject:
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="RadComboBoxSubject" OnSelectedIndexChanged="RadComboBoxSubjectSelectedIndexChanged"
                                AutoPostBack="true" />
                                <asp:Label runat="server" ID="LabelSubjectErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Course:
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="RadComboBoxCourse" />
                            <asp:Label runat="server" ID="LabelCourseErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Year:
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="RadComboBoxYear" />
                            <asp:Label runat="server" ID="LabelYearErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Semester:
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="RadComboBoxSemester" />
                            <asp:Label runat="server" ID="LabelSemesterErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Period:
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="RadComboBoxPeriod" />
                            <asp:Label runat="server" ID="LabelPeriodErrorMessage" Text="" ForeColor="Red"></asp:Label>
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
                                OnClientClicked="addClass" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel runat="server" ID="resultPanel" Visible="False" ClientIDMode="Static">
                <asp:Label runat="server" ID="lblResultMessage" Text="" />
                <br />
                <telerik:RadButton runat="server" ID="RadButtonOpenClass" Text="Open Class" AutoPostBack="False"
                    OnClientClicked="openClass" />
                &nbsp;
                <telerik:RadButton runat="server" ID="RadButtonClose" Text="Close" AutoPostBack="False"
                    OnClientClicked="closeWindow" />
            </asp:Panel>
            <asp:TextBox ID="TextBoxHiddenEncryptedClassID" runat="server" Style="visibility: hidden;
                display: none;" />
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="addClassLoadingPanel" runat="server" />
    </div>
</asp:Content>
