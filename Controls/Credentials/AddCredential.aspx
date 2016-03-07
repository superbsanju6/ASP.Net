<%@ Page Title="" Language="C#" MasterPageFile="~/AddNew.Master" AutoEventWireup="true" CodeBehind="AddCredential.aspx.cs" ValidateRequest="false"
    Inherits="Thinkgate.Controls.Credentials.AddCredential" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        a {
            cursor: pointer;
            text-decoration: underline;
            color: mediumblue;
        }

        .fieldLabel {
            width: 35%;
            font-size: 15px;
            float: none;
            text-align: left;
            margin-top: auto;
            vertical-align: top;
        }

        .fieldContent {
            width: 65%;
            float: none;
            text-align: left;
            margin-top: auto;
            vertical-align: top;
        }

        .tableCrd {
        }

        .attachButton {
            border: 1px solid #e3e3e3;
            border-radius: 40px;
            color: #000;
            font-size: 14px;
            width: 110px;
            float: right;
            line-height: 25px;
            cursor: pointer;
            background-repeat: no-repeat;
            background-image: url('../../Images/commands/add_new.png');
            background-position: 7px, 45px;
        }

        .TelerikModalOverlay {
            width: 486px;
        }
    </style>

    <script type="text/javascript">
        var documentChanged = false;
        var isActive = '<%= isActive %>';
        var alignments = '<%= alignments%>';
        var currentUrlButton = "";
        var deactivateButton = "";
        var addingCredential = '<%= credentialID <= 0 ? 1 : 0 %>';
       
        function closeCredentialsWindow() {
           
            var oWnd = getCurrentCustomDialog();
            setTimeout(function () {
                oWnd.close();
            }, 100)
            if (addingCredential == "1") {
                if (window.top.reloadPlanningCredentialsTile) {
                    window.top.reloadPlanningCredentialsTile();
                }
            }
        }

        var msgCredNameRequired = "Credential Name is a required field.";
        var msgCredMaxLength = "Maximum length of Credential Name is 200 characters.";
        var msgConfirmActivation = "Are you sure you want to <strong>activate</strong> this credential? Once <strong>activated</strong>, it will be added to the active list of credentials and can be recorded for students.";
        var msgConfirmDeactivation = "Are you sure you want to <strong> deactivate </strong> this credential? Once <strong>deactivated</strong>, it will be removed from the active list of credentials and can no longer be recorded for students.";
        var msgCredCannotDelete = "This credential cannot be deleted because it has been earned by students. If you do not want this credential to be available to be earned, you may deactivate the credential.";
        var msgConfirmDeleteCred = 'Are you sure you wish to delete this credential?';
        var msgCancelCredential = 'Are you sure you want to cancel?';
        var msgConfirmUrlRemove = 'Are you sure you wish to remove this entry? Once removed it cannot be retrieved.';
        var msgExisitingCredential = 'A credential with this name already exists. Please modify the name of this credential before saving.';

        function saveCredential(button, args) {
            var flag = false;
            var btnRadUPdate = $("#RadButtonUpdate")
            if (btnRadUPdate.hasClass("rbDisabled")) {
                button.set_autoPostBack(false);
            }
            else {
                flag = validateFormData(button);
                button.set_autoPostBack(flag);
                args.set_cancel(!flag);
            }
        }

        function existingCredentialAlert() {
            setTimeout(function () {
                customDialog({ title: 'Confirm?', maximize: true, maxwidth: 300, maxheight: 120, content: msgExisitingCredential, dialog_style: 'confirm' }, [{ title: 'OK' }]);
            }, 100);
        }

        function validateFormData() {        
            if ($('[id$="txtCredentialName"]').val().length == 0) {
                customDialog({ title: "Alert", maximize: true, maxwidth: 300, maxheight: 120, dialog_style: "alert", content: msgCredNameRequired }, [{ title: "OK" }]);
                return false;
            }
            else if ($('[id$="txtCredentialName"]').val().length > 200) {
                customDialog({ title: "Alert", maximize: true, maxwidth: 300, maxheight: 120, dialog_style: "alert", content: msgCredMaxLength }, [{ title: "OK" }]);
                return false;
            }

            else {
                if ($('[id$="rdActivateDeactivate"]').length > 0)
                    if (($('[id$="rdActivateDeactivate"]').is(":checked") && isActive == 1) )
                    {
                        customDialog({ title: 'Deactivate Credential?', maximize: true, maxwidth: 300, maxheight: 120, content: msgConfirmDeactivation, dialog_style: 'confirm' }, [{ title: 'Cancel' }, { title: 'OK', callback: deactivate }]);
                        return false;
                    }
                    else
                    {
                        customDialog({ title: 'Activate Credential?', maximize: true, maxwidth: 300, maxheight: 120, content: msgConfirmActivation, dialog_style: 'confirm' }, [{ title: 'Cancel' }, { title: 'OK', callback: deactivate }]);
                        return false;
                    }
                  
                }
            return true;
            
        }

        function deactivate() { 
            $('[id$="btnUpdateCredential"]').click();
        }

        function deleteCredential(button, args) {
            var AssignedToCount = '<%= assignedCount %>';
            if (AssignedToCount > 0) {
                customDialog({ title: "Alert", maximize: true, maxwidth: 400, maxheight: 160, dialog_style: "alert", content: msgCredCannotDelete }, [{ title: "OK" }]);
            }
            else {
                customDialog({ title: 'Confirm?', maximize: true, maxwidth: 300, maxheight: 120, content: msgConfirmDeleteCred, dialog_style: 'confirm' }, [{ title: 'Cancel' }, { title: 'OK', callback: deleteCredentialConfirm }]);
            }
        }

        function cancelCredential() {
            if (isCredentialHasChanges())
                customDialog({ title: 'Confirm?', maximize: true, maxwidth: 300, maxheight: 120, content: msgCancelCredential, dialog_style: 'confirm' }, [{ title: 'Cancel' }, { title: 'OK', callback: closeCredentialsWindow }]);
            else {
                var oWnd = getCurrentCustomDialog();
                oWnd.close();
            }
        }

        function deleteCredentialConfirm() {
            $('[id$="btnHiddenDelete"]').click();
        }

        function showAddNewCredentialURLWindow() {
            customDialog({ url: '<%= this.ResolveUrl("~/Controls/Credentials/AddUrl.aspx") %>', title: "URL Text", maximize: true, maxwidth: 461, maxheight: 200 });
        }

        function openAlignment() {
            var selectedAlignments = $("input[id*=hdnAlignments]").val();
            parent.customDialog({ name: "RadWindowManager2", url: '<%= this.ResolveUrl("~/Controls/Credentials/CredentialAlignments.aspx?crdAlignments=") %>' + encodeURIComponent(selectedAlignments), title: "Credential Alignment", maximize: true, maxwidth: 430, maxheight: 510 });
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="expandStudentCredentialLoadingPanel">
        <asp:HiddenField ID="hdnAlignments" runat="server" />   
         <asp:HiddenField ID="hdcredentialName" runat="server" />           
        <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
            <table class="tableCrd" style="width: 95%; table-layout: fixed; margin-left: 20px; margin-top: 25px;">
                <tr style="height: 45px;">
                    <td class="fieldLabel" align="left">Name:<span style="color: red;">*</span>
                    </td>
                    <td class="fieldContent" style="width: 200px;" align="left">
                        <telerik:RadTextBox Width="100%" runat="server" ID="txtCredentialName" MaxLength="200" BorderColor="Black" />
                        <asp:Label runat="server" ID="LabelNameErrorMessage" Text="" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr style="height: 45px;">
                    <td class="fieldLabel" align="left">Credential URLs:
                    </td>
                    <td class="fieldContent" align="left">
                        <div style="height: 200px; overflow: auto; border: 1px solid black;">
                            <asp:DataList ID="dtlURLs" runat="server" Width="99%">
                                <ItemTemplate>
                                    <table>
                                        <tr>
                                            <td style="padding-left: 5px">
                                                <asp:ImageButton ID="imgRemoveUrl" OnClientClick="return confirmUrlRemove(this);" argument='<%# Eval("ID") %>' ImageUrl="../../Images/deleteIcon.png" runat="server"
                                                    CommandArgument='<%# Eval("ID") %>' />
                                                <button id="btnDelete" class="btndelete" style="display: none" argument='<%# Eval("ID") %>' runat="server" onserverclick="hypUrlDelete_Click"></button>
                                            </td>
                                             <td style="padding-left: 5px; overflow: auto" >
                                                <a href='<%#Eval("URL") %>' target="_blank"><%#Eval("URL") %></a>
                                            </td>
                                        </tr>

                                    </table>
                                </ItemTemplate>
                            </asp:DataList>
                        </div>
                        <div style="margin-top: 10px; margin-bottom: 5px; float: left;">
                            <div class="attachButton" runat="server" id="btnAddURL" onclick="$find('RadButtonUpdate').set_enabled(true); showAddNewCredentialURLWindow();">
                                <div style="margin-left: 30px; line-height: 25px;">
                                    <span style="font-size: 14px;">Add URL</span>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr style="height: 45px;" id="trAlignment" runat="server">
                    <td class="fieldLabel" style="padding-top: 10px;" align="left">Alignment: </td>
                    <td class="fieldContent" style="padding-top: 10px;" align="left">
                        <div>
                            <a id="btnAlignment" runat="server" onclick="openAlignment();">View/Edit</a>
                        </div>
                    </td>
                </tr>
                <tr style="height: 45px;" id="trActivate" runat="server">
                    <td colspan="2">
                        <asp:CheckBox ID="rdActivateDeactivate" runat="server" Style="font-size: 14px;" />
                        <asp:Label ID="lblDeactivate" runat="server" Text="Deactivate Credential" AssociatedControlID="rdActivateDeactivate"></asp:Label>
                    </td>
                </tr>
                <tr style="height: 50px;" id="trSpacer" runat="server">
                    <td colspan="2"></td>
                </tr>


                <tr style="height: 45px;">
                    <td class="fieldLabel" style="margin-top: 60px;"></td>
                    <td class="fieldContent" style="text-align: right;">
                        <telerik:RadButton runat="server" ID="RadButtonDelete" Text="Delete" ClientIDMode="Static" AutoPostBack="false"
                            OnClientClicked="deleteCredential" Visible="false" UseSubmitBehavior="false" />
                        &nbsp;
                    <telerik:RadButton runat="server" ID="RadButtonCancel" Text="Cancel" AutoPostBack="False"
                        OnClientClicked="cancelCredential" UseSubmitBehavior="false" />
                        &nbsp;
                    <telerik:RadButton runat="server" ID="RadButtonUpdate" Text="Save" OnClick="RadButtonUpdate_Click" OnClientClicking="saveCredential" ClientIDMode="Static" UseSubmitBehavior="false" />
                    

                         <asp:Label runat="server" ID="lblResultMessage" Text="" />
                        <button id="btnHiddenBind" style="display: none" runat="server" onserverclick="btnHiddenBind_ServerClick"></button>
                        <button id="btnHiddenDelete" style="display: none" runat="server" onserverclick="RadButtonDelete_Click"></button>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="expandStudentCredentialLoadingPanel" runat="server" />
      <button id="btnUpdateCredential"  style="display: none"  runat="server" onserverclick="RadButtonUpdate_Click"></button>
    <script type="text/javascript">
        $(function () {
           
            var btnUpdate = $("#RadButtonUpdate");
            btnUpdate.addClass("rbDisabled");
            $('[id$="txtCredentialName"]').on('input propertychange paste', function () {
                enableDisableUpdateButton();
            });

            $('[id$="rdActivateDeactivate"]').on('change', function () {
                enableDisableUpdateButton();
            });
        });
        

        function removeUrlEntry()
        {
            var button = $(currentUrlButton).next();
            $(button).click();
            documentChanged = true;
            enableDisableUpdateButton();
        }

        function enableDisableUpdateButton() {            
            if (isCredentialHasChanges())
                var btnUpdate = $("#RadButtonUpdate")
            btnUpdate.removeClass("rbDisabled");
        }

        function confirmUrlRemove(button) {
            currentUrlButton = button;
            customDialog({ title: 'Confirm?', maximize: true, maxwidth: 300, maxheight: 120, content: msgConfirmUrlRemove, dialog_style: 'confirm' }, [{ title: 'Cancel', callback: urlRemoveCancelled }, { title: 'OK', callback: removeUrlEntry, argArray: [button] }]);
            return false;
        }

        function urlRemoveCancelled(button) {
            return false;
        }

        function setAlignmentsModified() {
            documentChanged = true;
            $("#RadButtonUpdate").removeClass("rbDisabled");
            $find("RadButtonUpdate").set_enabled(true);
        }
        function isCredentialHasChanges() {
            var credentialName = $("input[id*=hdcredentialName]").val();
              if ($('[id$="txtCredentialName"]').val() != credentialName)
                  documentChanged = true;
              if ($('[id$="rdActivateDeactivate"]').length > 0)
                  if (($('[id$="rdActivateDeactivate"]').is(":checked") && isActive == 1) || (!$('[id$="rdActivateDeactivate"]').is(":checked") && isActive == 0))
                      documentChanged = true;
              if ($('[id$="hdnAlignments"]').val() != alignments)
                  documentChanged = true;
              return documentChanged;
          }
       
    </script>
</asp:Content>
