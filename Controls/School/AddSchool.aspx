<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/AddNew.Master" CodeBehind="AddSchool.aspx.cs"
    Inherits="Thinkgate.Controls.School.AddSchool" Title="Add New School" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var requiredFields = [['#<%= TextBoxSchoolName.ClientID %>', '#<%= LabelSchoolNameErrorMessage.ClientID %>', 'Please enter a value for School Name.'],
                ['#<%= TextBoxAbbreviation.ClientID %>', '#<%= LabelAbbreviationErrorMessage.ClientID %>', 'Please enter a value for Abbreviation.'],
                ['#<%= TextBoxSchoolID.ClientID %>', '#<%= LabelSchoolIDErrorMessage.ClientID %>', 'Please enter a value for School ID.'],
                ['#<%= RadComboBoxSchoolType.ClientID %>', '#<%= LabelSchoolTypeErrorMessage.ClientID %>', 'Please select a value for School Type.'],
                ['#<%= RadComboBoxCluster.ClientID %>', '#<%= LabelClusterErrorMessage.ClientID %>', 'Please select a value for Cluster.']];

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
                if (!isStringNullOrEmpty($('#<%= TextBoxPhone.ClientID %>').val())) return true;

                return false;
            }
            
            function selectTextBoxSchoolID() {
                autoSizeWindow();
                $('#TextBoxSchoolID').focus();
            }

            function addSchool() {
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

            function openSchool() {
                window.open("../../Record/School.aspx?childPage=yes&xID=" + document.getElementById('<%= TextBoxHiddenEncryptedSchoolID.ClientID %>').value);
            }

            $(function () {
                addWindowBeforeCloseEvent(); //This and other common functions are in AddNew.Master
            });
        </script>
    </telerik:RadCodeBlock>
    <div style="overflow: hidden;">
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="addSchoolLoadingPanel">
            <asp:Panel runat="server" ID="addPanel">
                <table class="fieldValueTable fieldAddModalTable" style="width: 400px; margin-left: auto;
                    margin-right: auto;">
                    <tr>
                        <td class="fieldLabel">
                            School Name:
                        </td>
                        <td style="width: 160px;">
                            <asp:TextBox runat="server" ID="TextBoxSchoolName" />
                            <asp:Label runat="server" ID="LabelSchoolNameErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Abbreviation:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="TextBoxAbbreviation" />
                            <asp:Label runat="server" ID="LabelAbbreviationErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            School ID:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="TextBoxSchoolID" />
                            <asp:Label runat="server" ID="LabelSchoolIDErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            School Type:
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="RadComboBoxSchoolType" />
                            <asp:Label runat="server" ID="LabelSchoolTypeErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Cluster:
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="RadComboBoxCluster" />
                            <asp:Label runat="server" ID="LabelClusterErrorMessage" Text="" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Phone:
                        </td>
                        <td>
                            <telerik:RadMaskedTextBox runat="server" ID="TextBoxPhone" Mask="(###) ###-####" />
                            <div style="font-size: 12px;">
                                (Not Required)</div>
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
                                OnClientClicked="addSchool" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel runat="server" ID="resultPanel" Visible="false" ClientIDMode="Static">
                <asp:Label runat="server" ID="lblResultMessage" Text="" />
                <br />
                <telerik:RadButton runat="server" ID="RadButtonOpenSchool" Text="Open School" AutoPostBack="False"
                    OnClientClicked="openSchool" />
                &nbsp;
                <telerik:RadButton runat="server" ID="RadButtonClose" Text="Close" AutoPostBack="False"
                    OnClientClicked="closeWindow" />
            </asp:Panel>
            <asp:TextBox ID="TextBoxHiddenEncryptedSchoolID" runat="server" Style="visibility: hidden;
                display: none;" />
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="addSchoolLoadingPanel" runat="server" />
    </div>
</asp:Content>
