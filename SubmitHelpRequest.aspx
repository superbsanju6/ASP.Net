<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmitHelpRequest.aspx.cs" Inherits="Thinkgate.SubmitHelpRequest" EnableEventValidation="false"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">        
        .frmlbl
        {
            color: #000;
            font-family: arial,helvetica,sans-serif;
            font-size: 13px;
            line-height: 18px;
            text-align: left;
            vertical-align: middle;
            text-indent: 10px;
            margin-bottom: 0.5em;
            margin-left: 0;
            margin-right: 0;
            margin-top: 0.5em;
            padding-bottom: 0.5em;
            font-weight: bold;
        }

        .lblWidth {
            width: 86px;
        }

        .bordered {
            border: solid 2px #000;
        }

    </style>

	<script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
	<script type='text/javascript' src='Scripts/jquery-migrate-1.1.0.min.js'></script>
    <script type="text/javascript" src="../../Scripts/master.js"></script>

    <script type="text/javascript" src="Scripts/jquery.maskedinput-1.2.2.js"></script>

    <script language="javascript" type="text/javascript">
        var flag = 0;
        var OkStatus;

        $(window).load(function () {
            HidePopup();
            document.getElementById('txtEmail').focus();
            $('#txtPhone').mask("(999) - 999 - 9999");
        });

        function SubjectValueChange() {
            if ($("input[name=cmbComp]").val() == " ") {              
                $("input[name=cmbComp]").val("<Select One>");             
            }
        }

        function SubmitClick() {
            if ($("input[name=cmbComp]").val() == "<Select One>") {               
                $("input[name=cmbComp]").val(" ");               
                return true;
            }
            return true;
        }

        function ExpandCollapse(obj, row) {           
            var div = document.getElementById(obj);
            if (flag == 0) {
                $(div).addClass('bordered');
                ShowPopup();
                $("#divShowHide").hide();
                $("#divAnchor").hide();
                $("#divAttachmentDisabled").show();
                flag = 1;
            }
            else {
                $(div).removeClass('bordered');
                HidePopup();
                $("#divShowHide").show();
                $("#divAnchor").show();
                $("#divAttachmentDisabled").hide();
                flag = 0;
            }            
        }

        function HidePopup() {
            $("#divAttach").hide();              
            $("#divDisable").hide();
            $("#divSubmit").show();
        }

        function ShowPopup() {
            $("#divAttach").show();
            $("#divDisable").show();
            $("#divSubmit").hide();            
        }

        function ConfirmAndClose() {           
            document.forms[0].submit();           
            var confirmDialogText = "Your help request has been submitted.";
            parent.customDialog({ title: 'Confirmation Message', maximize: true, maxwidth: 500, maxheight: 100, animation: 'None', dialog_style: 'alert', content: confirmDialogText }, [{ title: 'Close' }]);;
        }

        function MessageAlert(message) {            
            parent.customDialog({ title: 'Add Attachment', maximize: true, maxwidth: 500, maxheight: 100, animation: 'None', dialog_style: 'alert', content: message }, [{ title: 'OK' }]);;

            $("#divBorder").attr('bordered');
            ShowPopup();
            $("#divAnchor").show();
            $("#divAttachmentDisabled").hide();
        }

        function ErrorMessageAlert(message) {
            parent.customDialog({ title: 'Add Attachment', maximize: true, maxwidth: 500, maxheight: 100, animation: 'None', dialog_style: 'alert', content: message }, [{ title: 'OK' }]);;
        }

        function FileUploadClose() {
            $('#divAttach').hide();
            SubjectValueChange();
        }

        var newWin = getCurrentCustomDialog();

        function CloseConfirmation(strText) {          
            parent.customDialog({ maximize: true, maxwidth: 500, maxheight: 100, resizable: false, title: 'Cancel Submit Help Request', content: "Are you sure you want to cancel? Any information entered will be lost?", dialog_style: 'confirm' },
                        [{ title: 'OK', callback: cancelCallbackFunction }, { title: 'Cancel' }]);            
        }

        function cancelCallbackFunction() {
            if (newWin) {
                setTimeout(function () { newWin.close(); }, 0);
            }
        }       
       
        function AllowNumericOnly(e) {          
            var keycode;           

            if (window.event)
            { keycode = window.event.keyCode; }
            else if (event)
            { keycode = event.keyCode; }
            else if (e)
            { keycode = e.which; }
            else { return true; }

            if (keycode >= 48 && keycode <= 57)
            { return true; }
            else { return false; }

            return true;
        }

        function StopKeypress(e) {           
            var keycode;

            if (window.event)
            { keycode = window.event.keyCode; }
            else if (event)
            { keycode = event.keyCode; }
            else if (e)
            { keycode = e.which; }
            else { return true; }

            if (keycode != 13)
            { return true; }
            else { return false; }

            return true;
        }

        function StopEnterKey(e) {
            var keycode;
            var flagBackSpace;

            if (window.event)
            { keycode = window.event.keyCode; }
            else if (event)
            { keycode = event.keyCode; }
            else if (e)
            { keycode = e.which; }
            else { return true; }

            if (keycode == 13)
            { return false; }
            else { return true; }
        }       
              
        function StopSpecialChars(e) {
            var keycode;

            if (window.event)
            { keycode = window.event.keyCode; }
            else if (event)
            { keycode = event.keyCode; }
            else if (e)
            { keycode = e.which; }
            else { return true; }

            if (keycode == 60 || keycode == 62)
            { return false; }
            else { return true; }

            return true;
        }
    </script>
</head>
<body>
 <form id="form1" runat="server" onkeydown ="return StopEnterKey(this);" onsubmit="return SubmitClick()" > 
    <telerik:RadScriptManager ID="ScriptManager2" runat="server" />
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
    </telerik:RadSkinManager>      
    <table width="100%">
        <tr>
            <td align="center">
                <div style="width: 550px; text-align: center; border-collapse: collapse;border: 1px solid black;">
                    <table  style="text-align: center; border-collapse: collapse;">
                    <tr>
                        <td id="label_email" style="padding: 1px 1px 1px 4px;" align="left" class="lblWidth">
                            <label for="email">
                               <b> Email:</b></label>
                        </td>
                        <td style="width:550px;" align="left">                           
                            <asp:TextBox ID="txtEmail" ClientIDMode="Static" runat ="server" Width ="150" type="text" OnKeyPress=" return StopKeypress(this);" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqEmail" runat="server" Display="Dynamic" ControlToValidate="txtEmail" Text ="*" ForeColor ="#ff0000" ErrorMessage ="*"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rgeContEmail" Display="Dynamic" runat="server" ForeColor ="#ff0000"
                                  ControlToValidate="txtEmail" ErrorMessage="*"
                                  ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td id="label_phone" style="padding: 1px 1px 1px 4px;" align="left" class="lblWidth">
                            <label for="phone">
                                <b>Phone:</b></label>
                        </td> 
                        <td align="left"> 
                            <table width="58%">
                                <tr>
                                <td>
                                    <asp:TextBox ID="txtPhone" runat="server" maxlength="40" Width ="150" onpaste="return false" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqPhone" runat="server" ControlToValidate="txtPhone" SetFocusOnError ="true" Text ="*" ForeColor ="#ff0000" ErrorMessage ="*"/>
                                </td>
                                <td align="left">
                                    <asp:Label ID ="lblExtn" runat ="server" Text="Extn : " ToolTip ="If you have extension,Please enter."></asp:Label>
                                    <asp:TextBox ID="txtExtn" runat="server" MaxLength="4" Width="40" onpaste="return false" OnKeyPress=" return AllowNumericOnly(this);"></asp:TextBox>
                                </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td id="label_component" style="padding: 1px 1px 1px 4px;" align="left" class="lblWidth">
                            <label for="component">
                                <b>Component:</b></label>                                    
                        </td>
                        <td style="width: 100px; padding: 0px 0px 0px 2px;" align="left" >
                           <div>                              
                                <telerik:RadComboBox ID="cmbComp" Skin="Web20" runat="server" DataTextField="ContentType"
                                         CausesValidation ="false"   EmptyMessage="<Select One>" Width="155" CssClass="formElement" />
                               <asp:RequiredFieldValidator ID ="reqComponent" runat ="server" ControlToValidate="cmbComp"  ErrorMessage="*" Text ="*" ForeColor ="#ff0000"></asp:RequiredFieldValidator>
                            </div>
                        </td>
                    </tr>         
                    <tr>
                        <td id="label_subject" style="padding: 1px 1px 1px 4px;" align="left" class="lblWidth">
                            <label for="subject">
                               <b> Subject:</b></label>
                        </td>
                        <td align="left">                            
                            <asp:TextBox ID="txtSubject" runat="server" maxlength="40" Width ="150" onkeypress ="return StopSpecialChars(this);" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td id="label_details" style="vertical-align: top; padding: 1px 1px 1px 4px;" align="left" class="lblWidth">
                            <label for="details">
                                <b>Details:</b>
                            </label>
                        </td>
                        <td style="vertical-align: middle; font-weight: normal; text-align: left;" align="left">
                            <asp:TextBox ID ="txtDetails" runat="server" TextMode ="MultiLine" style="width: 98%; height: 100px;" onkeypress ="return StopSpecialChars(this);" ></asp:TextBox>                            
                            <br />
                           <font size="2"> Please include additional information that will allow us to help you more quickly.</font><br />
                            <span style="color: red;"><font size="2">Please be sure to accurately complete each field above.</font></span><br />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div id="divBorder" style="width: 99%;">
                            <table style="width: 99%;">
                                <tr>
                                    <td width="103" align="left" id="Td1" style="vertical-align: top;
                                        padding: 0px 0px 0px 2px;">
                                       <u><b> Attachment:</b></u>
                                    </td>
                                    <td style="vertical-align: middle; font-weight: normal; text-align: left;" align="left"
                                        class="frmlbl">
                                        <div id="divAnchor" align="right" style="padding: 1px 1px 1px 4px;">                                         
                                            <%--<a href="javascript:expandcollapse('divBorder', 'one');"  style="text-decoration:none;">   --%>
                                            <a href="javascript:ExpandCollapse('divBorder', 'one');">   
                                                Add Attachment                                         
                                            </a>
                                        </div>
                                        <div id="divAttachmentDisabled" align="right" style="padding: 2px 2px 2px 4px;display:none;">
                                            <asp:label ID="lblFaded" runat="server" Font-Underline="true" Text ="Add Attachment" ForeColor ="#666699"></asp:label>
                                        </div>
                                    </td>
                                </tr>
                                <tr> 
                                     <td align="center" style="padding-left:0px" colspan="2">
                                         <div id="divAttach">
                                             <table>
                                                 <tr>
                                                     <td style="padding-left:100px">
                                                        <telerik:RadUpload ID="ruFileUpload" 
                                                            runat="server" InitialFileInputsCount="1" AllowedFileExtensions=".pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.jpg,.jpeg,.png,.gif,.bmp"
                                                            MaxFileInputsCount="3" OverwriteExistingFiles="False" ControlObjectsVisibility="None" 
                                                            InputSize="35" Width="327px" EnableFileInputSkinning="true" Skin="Web20"
                                                            ClientIDMode="Static" CssClass="formElement" Height="26px" >
                                                            <Localization Select="Browse" />                                                         
                                                        </telerik:RadUpload>                                                    
                                                     </td>
                                                 </tr>
                                                 <tr>
                                                  <td style="text-align: center;" class="editorLabel" colspan="2">
                                                    <telerik:RadButton runat="server" ID="rbClose" ClientIDMode="Static" Skin="Web20"
                                                        Text="Cancel" Width="50px"  CausesValidation="False" OnClick="rbClose_Click" 
                                                        />
                                                    &nbsp;
                                                    <telerik:RadButton runat="server" ID="rbOk" ClientIDMode="Static" Skin="Web20"
                                                        Text="OK" Width="50px" CausesValidation="false"  OnClick="rbOk_Click"  />
                                                 </td>
                                               </tr>
                                            </table>
                                        </div>                                        
                                      </td>
                                </tr> 
                                <tr>
                                    <td>
                                        <div id="divShowHide" >  
                                            <table width ="100%">
                                                <tr>
                                                    <td align="left"> 
                                                        <div style="overflow:hidden;width:400px;word-wrap:break-word;">
                                                         <asp:GridView ID ="gvFiles" runat="server" ShowHeader="false"  AutoGenerateColumns="false" BorderStyle ="None" GridLines="None" OnRowDataBound="gvFiles_RowDataBound" OnRowCommand="gvFiles_RowCommand" OnRowDeleting="gvFiles_RowDeleting" >
                                                             <Columns>
                                                                 <asp:TemplateField HeaderStyle-Height="0">
                                                                     <ItemTemplate> 
                                                                         <asp:HiddenField ID="hdnFileID" Value='<%# Eval("FileID") %>' runat="server" />
                                                                     </ItemTemplate>
                                                                 </asp:TemplateField>
                                                                 <asp:TemplateField>
                                                                     <ItemTemplate>
                                                                         <asp:ImageButton ID="imgDelete" runat="server" ToolTip="Click to delete file" CommandName="delete" ImageUrl="~/Images/cross.gif"
                                                                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FileID") %>' CausesValidation="false" Width="15px" />
                                                                     </ItemTemplate>
                                                                 </asp:TemplateField>
                                                                 <asp:TemplateField ItemStyle-Wrap = "true">
                                                                     <ItemTemplate>
                                                                         <a target="_blank" href='<%# "FTP/SubmitHelpRequest/" + Eval("FileName") %>'><%# Eval("ClientFileName") %></a>
                                                                     </ItemTemplate>
                                                                 </asp:TemplateField>
                                                                 <asp:TemplateField>
                                                                     <ItemTemplate>
                                                                         <asp:Label ID="lblFileDesc" runat="server" Text='<%# Eval("Description") %>' />
                                                                     </ItemTemplate>
                                                                 </asp:TemplateField>
                                                            </Columns>
                                                         </asp:GridView>
                                                     </div></td>
                                                </tr>
                                            </table>         
                                         </div>
                                    </td>
                                </tr>
                            </table>
                            </div>
                        </td>
                    </tr>
                    <tr style="height: 100px;">
                        <td colspan="2" style="text-align: center;" valign="bottom">
                            <div id="divSubmit">
                                <table width="160" border="0" align="right" cellpadding="2" cellspacing="2" >
                                    <tr>
                                        <td style="text-align: right;width:70%;" class="editorLabel" align="right" >
                                             <telerik:RadButton runat="server" ID="rbCancel" ClientIDMode="Static" Skin="Web20" TabIndex="1"
                                                Text="Cancel" CausesValidation="false"  OnClick="rbCancel_Click" 
                                          AutoPostBack ="false" OnClientClicked ="CloseConfirmation" ValidateRequestMode="Disabled"/>
                                        </td>
                                        <td align="left">
                                            <telerik:RadButton runat="server" ID="rbSubmit" Text="Submit" TabIndex ="0"
                                            Skin="Web20"  ClientIDMode="Static" OnClick="rbSubmit_Click" />
                                        </td>
                                    </tr>
                                    <tr><td colspan="2"></td></tr>
                                </table>
                           </div>
                            <div id="divDisable">
                                <table width="160" border="0" align="right" cellpadding="2" cellspacing="2">
                                    <tr>
                                        <td style="text-align: right;width:70%;" class="editorLabel" align="right">
                                            <telerik:RadButton runat="server" ID="RadButton1" Enabled ="false" ClientIDMode="Static" Skin="Web20"
                                                Text="Cancel" Width="80px" />
                                        </td>
                                        <td>
                                            <telerik:RadButton runat="server" ID="RadButton2" Text="Submit" Enabled ="false"
                                            Skin="Web20" ClientIDMode="Static" OnClick="rbSubmit_Click" />
                                        </td>
                                    </tr>
                                </table>
                           </div>
                          
                        </td>
                    </tr>
                </table>
             </div>
             </td>
        </tr>
    </table> 
</form>   
</body>
</html>
