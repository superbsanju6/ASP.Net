<%@ Page Title="Add New Course" Language="C#" MasterPageFile="~/AddNew.Master" AutoEventWireup="true" CodeBehind="AddCourse.aspx.cs" Inherits="Thinkgate.Controls.LCO.AddCourse" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="tk" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var requiredFields= [['#<%= cmbGrade.ClientID %>', '#<%= LabelGradeErrorMessage.ClientID %>', 'Please enter a value for Course Name.'],
                ['#<%= cmbProgramArea.ClientID %>', '#<%= LabelProgramAreaErrorMessage.ClientID %>', 'Please enter a value for Program Area.'],
                ['#<%= txtCourse.ClientID %>', '#<%= LabelCourseErrorMessage.ClientID %>', 'Please enter a value for Course.'],
                ['#<%= cmbImplementationYear.ClientID %>', '#<%= LabelImplementationYearErrorMessage.ClientID %>', 'Please select a value for Implementation Year.']];
           

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

            function selectTextBoxCourseID() {
                autoSizeWindow();
                $('#txtCourse').focus();
            }

            function editCourse() {

                var inputValue = $.trim($('#<%= txtCourse.ClientID %>').val());
                if (isStringNullOrEmpty(inputValue)) {
                    $('#<%= LabelCourseErrorMessage.ClientID %>'.text('Please enter a value for Course.'));
                    autoSizeWindow();
                }
                else {
                    getCurrentCustomDialog().remove_beforeClose(onClientBeforeClose);
                    __doPostBack('RadButtonEditOk', '');

                }


            }

            function addCourse() {
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

            function openCourse() {
                window.open("../../Record/CourseObject.aspx?xID=" + document.getElementById('<%= TextBoxHiddenEncryptedCourseID.ClientID %>').value);
                parent.location.reload();
            }

            function CloseCourse() {

                window.close();
                parent.location.reload();
            }

            $(function () {
                addWindowBeforeCloseEvent(); //This and other common functions are in AddNew.Master
            });
        </script>
        <style type="text/css">
            .ajax__calendar_next {width: 25px; height: 25px;}
            .ajax__calendar_prev {width: 25px; height: 25px;}
        </style>
    </telerik:RadCodeBlock>
    <div style="width: auto; height: auto; overflow: hidden;">
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" Style="width: auto; height: auto;" runat="server" LoadingPanelID="addCourseLoadingPanel">
            <asp:Panel runat="server" ID="addPanel">
                <table class="fieldValueTable fieldAddModalTable" style="width: 400px; margin-left: auto;
                    margin-right: auto;">
                    <tr>
                        <td></td>
                        </tr>
                    <tr>
                        <td class="fieldLabel">
                            <asp:Label ID="lblGrade" runat="server" Text="Grade:"></asp:Label>
                        </td>
                        <td style="width: 160px;">
                            <asp:Textbox ID="txtGrade" runat="server" Enabled="false" Visible="false" Width="195px" ></asp:Textbox>
                            <telerik:RadComboBox runat="server" ID="cmbGrade" Width="200px" />
                            <asp:Label runat="server" ID="LabelGradeErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                           Program Area:
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="cmbProgramArea" Width="200px" />
                            <asp:Label runat="server" ID="LabelProgramAreaErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Course:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtCourse" MaxLength="25" Width="195px" />
                            <asp:Label runat="server" ID="lblCourseLimit" Text="(Text Limit: 25 Characters)" Font-Italic="true" Font-Size="Smaller"></asp:Label>
                            <asp:Label runat="server" ID="LabelCourseErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            <asp:Label ID="lblImplementationYear" runat="server" Text="Implementation Year:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="cmbImplementationYear" Height="100px" Width="200px" />
                            <asp:Label runat="server" ID="LabelImplementationYearErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            <asp:Label runat="server" ID="lblImplementationSemester" Text="Implementation Semester:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="cmbSemester" Width="200px" />
                            <asp:Label runat="server" ID="lblSemester" Text="" ForeColor="Red"></asp:Label>                       
                        </td>
                    </tr>
                    <tr>
                        <td style="margin-top: 60px;" class="fieldLabel">
                        </td>
                        <td style="text-align: right;">
                            <telerik:RadButton runat="server" ID="RadButtonCancel" Text="Cancel" AutoPostBack="False"
                                OnClientClicked="closeWindow" />
                            &nbsp;
                            <telerik:RadButton runat="server" ID="RadButtonOk" Text="OK" AutoPostBack="False"
                                OnClientClicked="addCourse" />
                            <telerik:RadButton runat="server" ID="RadButtonEditOk" Text="OK" AutoPostBack="False" Visible="false"
                                OnClientClicked="editCourse" />
                            
                            
                           
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel runat="server" ID="resultPanel" Visible="false" ClientIDMode="Static">
                <asp:Label runat="server" ID="lblResultMessage" Text="" />
                <br />
                <telerik:RadButton runat="server" ID="RadButtonOpenCourse" Text="Open Course" AutoPostBack="False"
                    OnClientClicked="openCourse" />
                &nbsp;
                <telerik:RadButton runat="server" ID="RadButtonClose" Text="Close" AutoPostBack="False"
                    OnClientClicked="CloseCourse" />
            </asp:Panel>
            <asp:TextBox ID="TextBoxHiddenEncryptedCourseID" runat="server" Style="visibility: hidden;
                display: none;" />
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="addCourseLoadingPanel" runat="server" />
    </div>
</asp:Content>
