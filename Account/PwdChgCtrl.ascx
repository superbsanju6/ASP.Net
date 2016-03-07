<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="PwdChgCtrl.ascx.cs" Inherits="Thinkgate.Account.PwdChgCtrl" %>

<style type="text/css">
    .textBox {
        border: solid 1px #000;
        padding: 2px;
        width: 250px;
    }
    
    .ui-dialog-content li {
        font-size: 14px !important;
    }
    
    .labelForValidation {
        color: Red;
        display: block;
        font-size: 10pt;
        height: 45px;
    }

    .fieldLabel {
        font-weight: bold;
        font-size: 10pt;
        white-space: nowrap;
        text-align: left;
        vertical-align: top;
        width: 125px;
        height: 45px;
    }

    .fieldEntry {
        vertical-align: top;
        text-align: left;
        margin-top: 20px;
        height: 45px;
    }

    .tableDiv {
        display: table;
    }

    .row {
        display: table-row;
    }

    .cellLabel {
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

    .radDropdownBtn {
        font-weight: bold;
        font-size: 11pt;
    }

    .labelForValidation {
        color: Red;
        display: block;
        font-size: 10pt;
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

    .resultsDiv {
        width: 385;
        height: 270;
        text-align: center;
        vert-align: middle;
        margin: 30px;
    }
    html, body {
    height:100%
}

    #overlay { 
    position:absolute;
    z-index:10;
    width:100%;
    height:100%;
    top:0;
    left:0;
    background-color:#f00;
    filter:alpha(opacity=10);
    -moz-opacity:0.1;
    opacity:0.1;
    cursor:pointer;

} 
.height10px {
    height: 10px !important;
}

.dialog {
    position:absolute;
    border:2px solid #3366CC;
    width:250px;
    height:120px;
    background-color:#ffffff;
    z-index:12;
}
    .padBottom5px {
        padding-bottom: 8px !important;
    }

</style>

<div runat="server" id="divPasswordChangeCtrl" clientidmode="Static">
    <asp:HiddenField ID="pwdChgCtrl_hdnPasswordConfReq" Value="" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="pwdChgCtrl_hdnPasswordFormatReg" Value="" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="pwdChgCtrl_hdnValidationMsg" Value="" runat="server" ClientIDMode="Static" />
    <asp:Panel runat="server" ID="pwdChgCtrl_EditPanel" ClientIDMode="Static" Visible="True">
        <div id="displayMsgChild" style="display: none;" runat="server" ClientIDMode="Static">
        </div>
        <table>
            <tr>
                <td colspan="2">

                    <asp:Label runat="server" ID="pwdChgCtrl_LblValidation" CssClass="labelForValidation" ClientIDMode="Static" Text=""></asp:Label>
                    <br />
                </td>
            </tr>
            <tr>
                <td class="fieldLabel">New Password:
                </td>
                <td class="fieldEntry">
                    <telerik:RadTextBox runat="server" ID="pwdChgCtrl_TextBoxNewPwd" MaxLength="128" TextMode="Password"
                        Skin="Office2010Black" ClientIDMode="Static" Style="top: 0px; left: 0px" />
                    <asp:Label runat="server" ID="pwdChgCtrl_LblStatusNewPwd" CssClass="labelForValidation" ClientIDMode="Static"></asp:Label>

                </td>
            </tr>
            <tr>
                <td class="fieldLabel">Re-enter:
                </td>
                <td class="fieldEntry">
                    <telerik:RadTextBox runat="server" ID="pwdChgCtrl_TextBoxRetypePwd" MaxLength="128" TextMode="Password"
                        Skin="Office2010Black" ClientIDMode="Static" />
                    <asp:Label runat="server" ID="pwdChgCtrl_LblStatusRetypePwd" CssClass="labelForValidation height10px padBottom5px" ClientIDMode="Static"></asp:Label>
                    <a runat="server" id="h1MinimumPassword" clientidmode="Static" style="color:blue;text-decoration:underline;cursor:pointer">Minimum Password Requirements</a>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="text-align: center; padding-top: 8px;">
                        <telerik:RadButton runat="server" ID="pwdChgCtrl_CancelBtn" Text="Cancel" AutoPostBack="False"
                            OnClientClicked="plsWireUp_CancelBtnClickedEvent" Skin="Web20" ClientIDMode="Static" />
                        &nbsp;
                        <telerik:RadButton runat="server" ID="pwdChgCtrl_SubmitBtn" Text="Save New Password" AutoPostBack="False"
                            OnClientClicked="ChangePassword" Skin="Web20" ClientIDMode="Static" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static">
        <div class="resultsDiv" style="margin: 0px !important; padding-top: 30px !important;">
            <asp:Label runat="server" ID="pwdChgCtrl_LblResultMessage" Text="" />
            <br />
            <telerik:RadButton runat="server" ID="pwdChgCtrl_CloseBtn" Text="Close" Skin="Web20" AutoPostBack="False"
                OnClientClicked="plsWireUp_CloseBtnClickedEvent" />
        </div>
    </asp:Panel>

</div>

<telerik:RadCodeBlock runat="server" ID="pwdChgCtrl_RadCodeBlock" ClientIDMode="Static">
    
     
    <script type="text/javascript">

        /******************************************************************************************
        requiredFields is an array of field elements that must be filled out in order for the ctrl 
        to be submitted. It consists of three columns:
            1st     ID of element
            2nd     label near element to post validation/error messages about that elemet
            3rd     error message to post in the lable when not filled out.
        ******************************************************************************************/
        var requiredFields =
        [
        ['#<%= pwdChgCtrl_TextBoxNewPwd.ClientID %>', '#<%= pwdChgCtrl_LblStatusNewPwd.ClientID %>', 'Please enter new password.'],
        ['#<%= pwdChgCtrl_TextBoxRetypePwd.ClientID %>', '#<%= pwdChgCtrl_LblStatusRetypePwd.ClientID %>', 'Please retype new password.']
        ];

        /******************************************************************************************
        IsStringNullOrEmpty() - Returns true if value of str is null or empty; false if otherwise.
        ******************************************************************************************/
        function isStringNullOrEmpty(str) {
            return (str == null || str == "");
        }


        /******************************************************************************************
        UserHasAddedData() - Returns true/false after determining whether the values in any of the
                             fields in the edit panel have changed.
        ******************************************************************************************/
        function userHasAddedData() {

            /* Iterate through required fields to see if user has modified these. */
            for (var i = 0; i < requiredFields.length; ++i) {
                var inputValue = $.trim($(requiredFields[i][0]).val());
                if (!isStringNullOrEmpty(inputValue)) return true;
            }

            // if we've gotten this far, then user has not added any data to the fields in the 
            // edit panel.
            return false;
        }

        /******************************************************************************************
        SelectTextBoxNewPwd() - sets focus to new password text box.
        ******************************************************************************************/
        function SelectTextBoxNewPwd() {
            //if(autoSizeWindow) autoSizeWindow();
            $('#pwdChgCtrl_TextBoxNewPwd').focus();
        }

        /******************************************************************************************
        ValidateFields() - Returns true/false after determining whether what has been entered into 
                           the fields in the edit panel is valid for changing the password.  If
                           one or more fields are invalid, then true is returned.  Display 
                           validation message in appropriate label if issue is found.  If no 
                           validation issues are found, then false is returned.
        ******************************************************************************************/
        function ValidateFields() {



            for (var i = 0; i < requiredFields.length; ++i) {
                var inputValue = $.trim($(requiredFields[i][0]).val());

                if (isStringNullOrEmpty(inputValue)) {
                    $(requiredFields[i][1]).text(requiredFields[i][2]);
                    return true;
                }
            }
            if ($.trim($("#pwdChgCtrl_TextBoxNewPwd").val()) != "" && $("#pwdChgCtrl_hdnPasswordConfReq").val() == "Yes") {
                var passwordReg = new RegExp($("#pwdChgCtrl_hdnPasswordFormatReg").val());
                var password = $("#pwdChgCtrl_TextBoxNewPwd").val();
                if (password.match(passwordReg) == null) {
                    $('#pwdChgCtrl_LblValidation').text($("#pwdChgCtrl_hdnValidationMsg").val());
                    return true;
                }
                else {
                    $('#pwdChgCtrl_LblValidation').text("");
                }
            }
            else {
                if ($("#pwdChgCtrl_TextBoxNewPwd")[0].value.length < 6) {
                    /** REMOVED THIS LINE OF CODE BECAUSE .innerText is not compatible with FireFox
                    $('#pwdChgCtrl_LblValidation')[0].innerText = "Password length must be greater than or equal to 6 characters.";
                    **/
                    $('#pwdChgCtrl_LblValidation').text("Password length must be greater than or equal to 6 characters.");
                    return true;
                }
            }

            //Check to see whether "New Pwd" textbox and "Retype Pwd" textbox match.
            if ($("#pwdChgCtrl_TextBoxNewPwd")[0].value != $('#pwdChgCtrl_TextBoxRetypePwd')[0].value) {
                /** REMOVED THIS LINE OF CODE BECAUSE .innerText is not compatible with FireFox
                $('#pwdChgCtrl_LblValidation')[0].innerText = "New and retyped password values do not match.";
                **/
                $('#pwdChgCtrl_LblValidation').text("New and retyped password values do not match.");
                return true;
            }







            return false;
        }

        /******************************************************************************************
        ClearErrorMessages() - Sets all status labels in edit panel to "".
        ******************************************************************************************/
        function ClearErrorMessages() {
            for (var i = 0; i < requiredFields.length; ++i) {
                $(requiredFields[i][1]).text('');
            }

            $('#pwdChgCtrl_LblValidation')[0].innerText = '';
        }

        /******************************************************************************************
        ChangePassword() - If contents of fields in edit panel are valid, then performs postback where
                           password change will occur.
        ******************************************************************************************/
        function ChangePassword() {
            ClearErrorMessages();

            if (ValidateFields()) {
                //if (autoSizeWindow) autoSizeWindow();
            }
            else {

                /***********************************************************************
                Confirmation before close was only needed if the user cancelled or 
                closed (x in upper right).  We are now posting back so we can add. 
                Remove the confirm action from the customDialog's close process.
                ***********************************************************************/
                if (addWindowBeforeCloseEvent) getCurrentCustomDialog().remove_beforeClose(onClientBeforeClose);

                __doPostBack('pwdChgCtrl_SubmitBtn', '');
            }
        }

        function plsWireUp_CancelBtnClickedEvent() {
            //TODO: DHB Tried to rewire these two events from outside this control so it would be more generic, but Chrome wouldn't work.
            closeWindow();
            //alert('Please wire up this Button\'s event.');
        }

        function plsWireUp_CloseBtnClickedEvent() {
            //TODO: DHB Tried to rewire these two events from outside this control so it would be more generic, but Chrome wouldn't work.
            closeWindow();
            //alert('Please wire up this Button\'s event.');
        }

        //function checkPasswordStrength() {
        //    if ($.trim($("#pwdChgCtrl_TextBoxNewPwd").val()) != "" && $("#pwdChgCtrl_hdnPasswordConfReq").val() == "Yes") {
        //        var passwordReg = $("#pwdChgCtrl_hdnPasswordFormatReg").val();
        //        passwordReg = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$/;
        //        if (!passwordReg.test($("#pwdChgCtrl_TextBoxNewPwd").val())) {
        //            $('#pwdChgCtrl_LblValidation').text("Password must follow password policy.");
        //            return false;
        //        }
        //    }
        //    else
        //        ClearErrorMessages();

        //}

        /**********************************************************************
        Initialization script
        **********************************************************************/
        $(function () {
            $("#h1MinimumPassword").on("click", function () {
                window.parent.PasswordRequirement.ShowDialog($("#displayMsgChild").html());
            });
            SelectTextBoxNewPwd();
        });


    </script>
</telerik:RadCodeBlock>
