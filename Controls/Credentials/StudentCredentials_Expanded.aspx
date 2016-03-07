<%@ Page Title="Student Credentials" Language="C#" MasterPageFile="~/AddNew.Master" AutoEventWireup="true" CodeBehind="StudentCredentials_Expanded.aspx.cs" Inherits="Thinkgate.Controls.Credentials.StudentCredentials_Expanded" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        .commentLink {
            cursor: pointer;
        }

        .btnSelect {
            height: 22px;
            line-height: 14px;
            font-size: 11px;
            width: 47px;
            border-radius: 3px;
        }

        .btnUnselect {
            height: 22px;
            line-height: 14px;
            font-size: 11px;
            width: 62px;
            border-radius: 3px;
        }
    </style>

    <telerik:RadCodeBlock ID="JsCodeStudentCredential" runat="server">
        <script type="text/javascript">
            var currentStudentID = '<%= this.CurrentStudentID %>';
            var currentButton = null;

            function EntryAdded(sender, args) {
                var dropDown = sender;
                if (dropDown) {
                    setTimeout(function () {
                        dropDown.closeDropDown();
                    <%= this.ClientScript.GetPostBackEventReference(this.ddlAlignments, "OnClientEntryAdded") %>
                    }, 100);
                }
            }

            function closeStudentCredentialExWindow(refreshParent) {

                var oWnd = getCurrentCustomDialog();
                setTimeout(function () {
                    oWnd.close();
                }, 100);

                if (refreshParent) {
                    if (window.top.reloadStudentCredentialsTile) {
                        window.top.reloadStudentCredentialsTile();
                    }
                }
            }

            function closeSaveWindow() {
                closeStudentCredentialExWindow(true);
            }

            /*This is used for Print Document Download*/
            function RequestStart(sender, args) {
                if (args.get_eventTarget().indexOf("btnPrintBtn") != -1) {
                    args.set_enableAjax(false);
                }
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div>
        <table width="100%;">
            <tr>
                <td class="fieldLabel1" style="padding-top: 10px;">
                    <span style="font-weight: bold; padding-right: 10px;">Student Name:</span>
                    <asp:Label runat="server" ID="lblStudentName"></asp:Label>
                    <br />
                    <span style="font-weight: bold; padding-right: 5px;">Credentials Earned:</span>
                    <asp:Label runat="server" ID="lblCredentialsEarned" Text=""></asp:Label>
                </td>
                <td>
                    <asp:ImageButton ID="btnPrintBtn" runat="server" OnClick="btnPrintBtn_Click" ImageUrl="~/Images/Toolbars/print.png" Style="padding-right: 20px; float: right;" />
                </td>
            </tr>
        </table>
        <div style="padding-bottom: 30px;"></div>
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="expandStudentCredentialLoadingPanel" ClientEvents-OnRequestStart="RequestStart">
            <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
                <table class="fieldValueTable fieldAddModalTable" style="width: 98%; margin-left: auto; margin-right: auto;">
                    <tr>
                        <td style="padding-left: 10px; padding-bottom: 5px;">
                            <span style="font-weight: bold; margin-left: 7px; margin-right: 5px;">List:</span>
                            <telerik:RadComboBox runat="server" ID="ddlEarned"
                                OnSelectedIndexChanged="ddlEarned_SelectedIndexChanged" ClientIDMode="AutoID" Skin="Web20"
                                Text="" CssClass="radDropdownBtn" Width="100" AutoPostBack="true">
                                <Items>
                                    <telerik:RadComboBoxItem Text="All" Value="All" />
                                    <telerik:RadComboBoxItem Text="Earned" Value="Earned" />
                                </Items>
                            </telerik:RadComboBox>
                            <span runat="server" id="spnAlignment">
                            <span style="font-weight: bold; margin-left: 35px; margin-right: 7px;">Alignment:</span>
                            <telerik:RadDropDownTree runat="server" ID="ddlAlignments" DataTextField="CredentialAlignment" DataFieldParentID="ParentID"
                                DataFieldID="ID" OnDataBound="ddlAlignments_DataBound" OnNodeDataBound="ddlAlignments_NodeDataBound"
                                DataValueField="ID" Width="300" OnClientEntryAdded="EntryAdded" OnEntryAdded="ddlAlignments_EntryAdded">
                                <DropDownSettings OpenDropDownOnLoad="false" Height="280" />
                            </telerik:RadDropDownTree>
                                </span>
                        </td>
                    </tr>

                    <tr>
                        <td class="fieldLabel1" style="text-align: center;">
                            <telerik:RadGrid ID="gridStudentCredentials" runat="server" AutoGenerateColumns="false" Width="100%" Height="445px" AllowAutomaticUpdates="True"
                                OnItemDataBound="gridStudentCredentials_ItemDataBound" MasterTableView-DataKeyNames="CredentialID">
                                <ClientSettings EnableRowHoverStyle="False">
                                    <Selecting AllowRowSelect="False" />
                                    <Scrolling AllowScroll="True" SaveScrollPosition="True"
                                        UseStaticHeaders="True" />
                                    <Resizing AllowColumnResize="True" />
                                </ClientSettings>
                                <HeaderStyle Font-Bold="true" CssClass="gridcolumnheaderstyle" />
                                <MasterTableView>
                                    <Columns>
                                        <telerik:GridTemplateColumn HeaderText="Earned" DataField="Value" ItemStyle-Width="75px" HeaderStyle-Width="75px"
                                            ItemStyle-CssClass="gridcolumnstyle" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkEarned" runat="server" Checked='<%# Convert.ToBoolean(Eval("Earned")) %>' Style="display: none;" />
                                                <input type="button" id="btnEarned" onclick="selectUnselectCredential(this);" class='<%# Convert.ToBoolean(Eval("Earned")) == true ? "btnUnselect" : "btnSelect" %>' value='<%# Convert.ToBoolean(Eval("Earned")) == true ? "Unselect" : "Select" %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="ID" Display="false" ReadOnly="True" />
                                        <telerik:GridTemplateColumn HeaderText="Credential Name" ItemStyle-HorizontalAlign="Left"
                                            ItemStyle-Width="230px" HeaderStyle-Width="230px" ItemStyle-CssClass="gridcolumnstyle">
                                            <ItemTemplate>
                                                <span runat="server" id="spanCredentialName">
                                                    <%# Eval("CredentialName") %>
                                                </span>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>

                                        <telerik:GridTemplateColumn HeaderText="Date Earned (Required)"
                                            ItemStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-CssClass="gridcolumnstyle"                                            
                                            >
                                            <ItemTemplate>
                                                <span id="divrdpEarnedDate">
                                                    <telerik:RadDatePicker ID="rdpEarnedDate" DateInput-ReadOnly="true" runat="server" Width="100px" DateInput-DateFormat="MM/dd/yyyy"
                                                        MinDate="01/01/1000" MaxDate='<%# DateTime.Now.Date %>' Calendar-FastNavigationStep="12" Calendar-FastNavigationNextText="" Calendar-FastNavigationPrevText="" Calendar-NavigationNextText="" Calendar-NavigationPrevText="">
                                                    </telerik:RadDatePicker>
                                                </span>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="School Year Earned (Required)"
                                            ItemStyle-Width="120px" HeaderStyle-Width="120px" ItemStyle-CssClass="gridcolumnstyle" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="cmbYear"></asp:DropDownList>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Expiration Date"
                                            ItemStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-CssClass="gridcolumnstyle">
                                            <ItemTemplate>
                                                <span id="divrdpExpirationDate">
                                                    <telerik:RadDatePicker ID="rdpExpirationDate" DateInput-ReadOnly="true" runat="server" Width="100px" DateInput-DateFormat="MM/dd/yyyy"
                                                        MinDate="01/01/1000" MaxDate="01/01/3000"  Calendar-FastNavigationStep="12"
                                                        Calendar-FastNavigationNextText="" Calendar-FastNavigationPrevText="" Calendar-NavigationNextText="" Calendar-NavigationPrevText=""
                                                        >
                                                    </telerik:RadDatePicker>
                                                </span>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>

                                        <telerik:GridTemplateColumn HeaderText="Recorded By" ItemStyle-HorizontalAlign="Left" ItemStyle-CssClass="gridcolumnstyle"
                                            ItemStyle-Width="130px" HeaderStyle-Width="130px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRecordedBy" runat="server" Text='<%# Eval("RecordedBy") %>'></asp:Label>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>

                                        <telerik:GridTemplateColumn HeaderText="Comments" ItemStyle-CssClass="gridcolumnstyle" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px" HeaderStyle-Width="100px">
                                            <ItemTemplate>
                                                <a href="#" id="lnkComment" onclick='<%# string.Format("openComments(this, {0}, {1}, {2})", HttpUtility.UrlEncode(Eval("ID").ToString()), 
                                                HttpUtility.UrlEncode(Eval("StudentID").ToString()),
                                                HttpUtility.UrlEncode(Eval("CredentialID").ToString())
                                                ) %>'                                                    
                                                    runat="server">View</a>
                                                <img runat="server" id="imgComment" src="~/Images/comment.png" alt="No comments" style="height: 17px; width: 17px;"
                                                    onclick='<%# string.Format("openAddComments(this, {0}, {1})", HttpUtility.UrlEncode(Eval("StudentID").ToString()),
                                                HttpUtility.UrlEncode(Eval("CredentialID").ToString())
                                                ) %>' />

                                                <input type="hidden" id="hdnCommentCount" value='<%# Eval("CommentsCount") %>'>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="CredentialID" Display="false" ReadOnly="True" />
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; padding-top: 30px;">
                            <telerik:RadButton runat="server" ID="RadButtonUpdate" Text="Update" OnClick="RadButtonUpdate_Click" OnClientClicking="saveCredentials" ClientIDMode="Static" />
                            &nbsp;
                            <telerik:RadButton runat="server" ID="RadButtonCancel" Text="Cancel" AutoPostBack="False" OnClientClicked="closeCustomDialog" ClientIDMode="Static" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static">
                <asp:Label runat="server" ID="lblResultMessage" Text="" CssClass="resultPanel" />
                <br />
                <telerik:RadButton runat="server" ID="RadButton1" Text="Close" AutoPostBack="False" CssClass="resultPanel" OnClientClicked="refreshParentWindow" />
            </asp:Panel>
            <input type="button" id="btnRefreshCredential" runat="server" onserverclick="btnRefreshCredential_ServerClick" style="display: none;" />
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="expandStudentCredentialLoadingPanel" runat="server" />
    </div>

    <script type="text/javascript">

        function openComments(sender, studCrdId, studId, crdId) {
            customDialog({
                url: '../Credentials/StudentCredentialComment.aspx?studCrdId=' + studCrdId + '&studId=' + studId + '&crdId=' + crdId + '&lnkId=' + sender.id, maximize: true, maxwidth: 1300, maxheight: 1100, title: 'Comments'
            });
        }
        function openAddComments(sender, studId, crdId) {
            customDialog({
                url: '../Credentials/StudentCredentialComment.aspx?studId=' + studId + '&crdId=' + crdId + '&lnkId=' + sender.id, maximize: true, maxwidth: 1300, maxheight: 1100, title: 'Comments'
            });
        }


        function ShowInvalidDateMessage() {
            customDialog({ title: "Alert", autoSize: true, dialog_style: "alert", content: "Please confirm that the date is enter in a valid mm/dd/yyyy format." }, [{ title: "OK" }]);
            return false;
        }

        function validateearneddate() {
            var flag = true;
            $('[id$="chkEarned"]').each(function () {
                if ($(this).is(':checked')) {
                    var earnedDataCtrl = $(this).closest("tr").find('[id$="rdpEarnedDate_dateInput"]');
                    var expireDataCtrl = $(this).closest("tr").find('[id$="rdpExpirationDate_dateInput"]');

                    if ($(earnedDataCtrl).length > 0) {
                        if ($(earnedDataCtrl).val() == "") {
                            $(earnedDataCtrl).css("border-color", "red");
                            customDialog({ title: "Alert", autoSize: true, dialog_style: "alert", content: "Please be sure all required fields have been entered for credentials that have been earned." }, [{ title: "OK" }]);
                            flag = false;
                            return false;
                        }
                        else if (!Date.parse($(earnedDataCtrl).val())) {
                            ShowInvalidDateMessage();
                            flag = false;
                            return false;
                        }
                        else if ((new Date($(earnedDataCtrl).val())) > (new Date())) {
                            $(earnedDataCtrl).css("border-color", "red");
                            customDialog({ title: "Alert", autoSize: true, dialog_style: "alert", content: "Please be sure there are no future dates entered.<br/>Only Current and past dates can be entered." }, [{ title: "OK" }]);
                            flag = false;
                            return false;
                        }
                        else if ($(expireDataCtrl).length > 0 && $(expireDataCtrl).val() != "") {
                            var dtEarned = new Date($(earnedDataCtrl).val());
                            var dtExpire = new Date($(expireDataCtrl).val());

                            if (!Date.parse($(expireDataCtrl).val())) {
                                ShowInvalidDateMessage();
                                flag = false;
                                return false;
                            }

                            if (dtEarned > dtExpire) {
                                customDialog({ title: "Alert", autoSize: true, dialog_style: "alert", content: "Please be sure the <b>Expiration Dates</b> are not  earlier than the <b>Date Earned</b> for credentials that have been earned." }, [{ title: "OK" }]);
                                $(expireDataCtrl).css("border-color", "red");
                                flag = false;
                                return false;
                            }
                        }

                    }

                    if ($(this).closest("tr").find("[id$='cmbYear'] :selected").text() == "")
                    {
                        customDialog({ title: "Alert", autoSize: true, dialog_style: "alert", content: "Please be sure a year has been entered in the <b>School Year Earned</b> column for all credentials that have been earned." }, [{ title: "OK" }]);
                        //$(this).closest("tr").find("[id$='cmbYear']").css("border-color", "red");                        
                        flag = false;
                        return false;
                    }

                }
                if (!flag) return false;
            });
            return flag;
        }

        function closeCustomDialog(button, args) { 
            //var result = confirm('Are you sure you wish to cancel? Any changes made will be lost.');
            //if (result)            
            // removeTemporaryAssociation();           
            radconfirm('Are you sure you wish to cancel? Any changes made will be lost.', confirmCallback, 500, 120, null, 'Confirm');            
        }

        function removeTemporaryAssociation()
        {
            var studentId = currentStudentID;
            $.ajax({
                type: "POST",
                url: "CredentialWebServices.asmx/RemoveTemporaryCredentialAssociation",
                data: "{ 'studentId': " + studentId + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Exception in removing temporary credentials");
                },
                success: function (resultData) {
                    var result = JSON.parse(resultData.d);
                    if (result.SaveStatus) {
            closeStudentCredentialExWindow(false);
        }
                    else
                    {
                        alert("Exception in removing temporary credentials");
                    }
                }
            });
        }

        function saveCredentials(button, args) {            
            var flag = validateearneddate();
            button.set_autoPostBack(flag);
            args.set_cancel(!flag);
            if (flag) {
                var win = getCurrentCustomDialog();
                win.remove_beforeClose(parent.onClientBeforeClose);
            }
        }

        function selectUnselectCredential(button) {
            var buttonText = $(button).val();
            $(button).attr("disabled", "true");
            $("#RadButtonUpdate").addClass("rbDisabled");

            if (buttonText == "Select") {
                $(button).parent("td").siblings().each(function () {
                    if ($(this).find('[id$="chkEarned"]').length > 0)
                        $(this).find('[id$="chkEarned"]').prop('checked', true);

                    if ($(this).find('[id$="divrdpEarnedDate"]').length > 0)
                        $(this).find('[id$="divrdpEarnedDate"]').css({ 'display': 'block' });

                    if ($(this).find('[id$="cmbYear"]').length > 0)
                        $(this).find('[id$="cmbYear"]').css({ 'display': 'block' });

                    if ($(this).find('[id$="divrdpExpirationDate"]').length > 0)
                        $(this).find('[id$="divrdpExpirationDate"]').css({ 'display': 'block' });

                    if ($(this).find('[id$="lblRecordedBy"]').length > 0)
                        $(this).find('[id$="lblRecordedBy"]').css({ 'display': 'block' });

                    if ($(this).find('[id$="lnkComment"]').length > 0)
                        if ($(this).find('[id$="hdnCommentCount"]').length > 0)
                            if ($(this).find('[id$="hdnCommentCount"]').val() > 0)
                                $(this).find('[id$="lnkComment"]').css({ 'display': 'block', 'text-align': 'center' });
                            else
                                $(this).find('[id$="lnkComment"]').css({ 'display': 'none', 'text-align': 'center' });

                    if ($(this).find('[id$="imgComment"]').length > 0)
                        if ($(this).find('[id$="hdnCommentCount"]').length > 0)
                            if ($(this).find('[id$="hdnCommentCount"]').val() == 0)
                                $(this).find('[id$="imgComment"]').css({ 'display': 'block', 'cursor': 'pointer' });
                            else
                                $(this).find('[id$="imgComment"]').css({ 'display': 'none', 'cursor': 'pointer' });

                });
                onChangeSelectUnselect(button);
            }
            else if (buttonText == "Unselect") {
                showConfirmationForUnselect(button);
            }
        }

        function showConfirmationForUnselect(button) {
            currentButton = button;
            customDialog({ title: 'Alert', maxwidth: 300, maxheight: 120, content: "Are you sure you wish to remove this entry? Once removed, any date entered for the credential will be lost.", dialog_style: 'alert', onClosed: confirmAndCancel }, [{ title: 'Cancel', callback: confirmAndCancel }, { title: 'OK', callback: confirmAndUnselect }]);
        }

        function confirmAndUnselect() {
            var button = currentButton;
            $("#RadButtonUpdate").addClass("rbDisabled");

            $(button).parent("td").siblings().each(function () {
                if ($(this).find('[id$="chkEarned"]').length > 0)
                    $(this).find('[id$="chkEarned"]').prop('checked', false);
                if ($(this).find('[id$="divrdpEarnedDate"]').length > 0)
                    $(this).find('[id$="divrdpEarnedDate"]').css({ 'display': 'none' });

                if ($(this).find('[id$="cmbYear"]').length > 0)
                    $(this).find('[id$="cmbYear"]').css({ 'display': 'none' });

                if ($(this).find('[id$="divrdpExpirationDate"]').length > 0)
                    $(this).find('[id$="divrdpExpirationDate"]').css({ 'display': 'none' });

                if ($(this).find('[id$="lblRecordedBy"]').length > 0)
                    $(this).find('[id$="lblRecordedBy"]').css({ 'display': 'none' });

                if ($(this).find('[id$="lnkComment"]').length > 0)
                    $(this).find('[id$="lnkComment"]').css({ 'display': 'none', 'text-align': 'center' });

                if ($(this).find('[id$="imgComment"]').length > 0)
                    $(this).find('[id$="imgComment"]').css({ 'display': 'none', 'cursor': 'pointer' });
            });
            onChangeSelectUnselect(button);
        }

        function confirmAndCancel() {
            var button = currentButton;
            $(button).removeAttr("disabled");
            var btnUPdate = $("#RadButtonUpdate");
            if (btnUPdate.hasClass("rbDisabled")) {
                btnUPdate.removeClass("rbDisabled");
            }
        }

        function commentLinkEnable() {
            
            $('input[type=button]').each(function (index, button) {
                var buttonText = $(button).val();
                if (buttonText == "Unselect") {
                    $(this).parent("td").siblings().each(function (i, td) {
                        if ($(this).find('[id$="divrdpEarnedDate"]').length > 0)
                            $(this).find('[id$="divrdpEarnedDate"]').css({ 'display': 'block' });

                        if ($(this).find('[id$="cmbYear"]').length > 0)
                            $(this).find('[id$="cmbYear"]').css({ 'display': 'block' });

                        if ($(this).find('[id$="divrdpExpirationDate"]').length > 0)
                            $(this).find('[id$="divrdpExpirationDate"]').css({ 'display': 'block' });

                        if ($(this).find('[id$="lblRecordedBy"]').length > 0)
                            $(this).find('[id$="lblRecordedBy"]').css({ 'display': 'block' });

                        if ($(this).find('[id$="lnkComment"]').length > 0)
                            if ($(this).find('[id$="hdnCommentCount"]').length > 0)
                                if ($(this).find('[id$="hdnCommentCount"]').val() > 0)
                                    $(this).find('[id$="lnkComment"]').css({ 'display': 'block', 'text-align': 'center' });
                                else
                                    $(this).find('[id$="lnkComment"]').css({ 'display': 'none', 'text-align': 'center' });

                        if ($(this).find('[id$="imgComment"]').length > 0)
                            if ($(this).find('[id$="hdnCommentCount"]').length > 0)
                                if ($(this).find('[id$="hdnCommentCount"]').val() == 0)
                                    $(this).find('[id$="imgComment"]').css({ 'display': 'block', 'cursor': 'pointer' });
                                else
                                    $(this).find('[id$="imgComment"]').css({ 'display': 'none', 'cursor': 'pointer' });

                        highlightGridRow(button);
                    });
                }
                else {
                    $(this).parent("td").siblings().each(function () {
                        if ($(this).find('[id$="divrdpEarnedDate"]').length > 0)
                            $(this).find('[id$="divrdpEarnedDate"]').css({ 'display': 'none' });

                        if ($(this).find('[id$="cmbYear"]').length > 0)
                            $(this).find('[id$="cmbYear"]').css({ 'display': 'none' });

                        if ($(this).find('[id$="divrdpExpirationDate"]').length > 0)
                            $(this).find('[id$="divrdpExpirationDate"]').css({ 'display': 'none' });

                        if ($(this).find('[id$="lblRecordedBy"]').length > 0)
                            $(this).find('[id$="lblRecordedBy"]').css({ 'display': 'none' });

                        if ($(this).find('[id$="lnkComment"]').length > 0)                            
                                $(this).find('[id$="lnkComment"]').css({ 'display': 'none', 'text-align': 'center' });

                        if ($(this).find('[id$="imgComment"]').length > 0)                            
                            $(this).find('[id$="imgComment"]').css({ 'display': 'none', 'cursor': 'pointer' });

                        highlightGridRow(button);
                    });
                }
            });
            $('[id$="gridStudentCredentials_GridData"]').css({ 'height': '410px' });

            $('.gridcolumnstyle').css({ 'vertical-align': 'middle' });
            $('.gridcolumnheaderstyle').css({ 'vertical-align': 'middle', 'height': '5px', 'padding-top': '0px', 'padding-bottom': '0px' });

        }

        function onChangeSelectUnselect(button) {

            var studentId = currentStudentID;
            var currentStatus = $(button).val();
            var studentCredentialId = $(button).parent("td").siblings("td").first().text();
            var credentialId = $(button).parent("td").siblings("td").last().text();


            $.ajax({
                type: "POST",
                url: "CredentialWebServices.asmx/CreateCredentialAssociation",
                data: "{  'currentStatus': '" + currentStatus + "','studentCredentialId':" + studentCredentialId + ",  'credentialId': " + credentialId + ", 'studentId': " + studentId + "}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $(button).removeAttr("disabled");
                    var btnUPdate = $("#RadButtonUpdate");
                    if (btnUPdate.hasClass("rbDisabled")) {
                        btnUPdate.removeClass("rbDisabled");
                    }
                },
                success: function (resultData) {
                    var result = JSON.parse(resultData.d);
                    if (result.SaveStatus) {
                    if (result.PreviousStatus == "Select") {
                        $(button).val(result.NewStatus);
                        $(button).removeClass("btnSelect").addClass("btnUnselect");
                            $(button).parent("td").parent('tr').find('[id$="chkEarned"]').prop('checked', true)
                    }
                    else {
                        $(button).val(result.NewStatus);
                        $(button).removeClass("btnUnselect").addClass("btnSelect");
                            $(button).parent("td").parent('tr').find('[id$="chkEarned"]').prop('checked', false)
                        }
                    }
                    highlightGridRow(button);
                    $(button).removeAttr("disabled");
                    var btnUPdate = $("#RadButtonUpdate");
                    if (btnUPdate.hasClass("rbDisabled")) {
                        btnUPdate.removeClass("rbDisabled");
                    }
                }
            });
        }

        function highlightGridRow(button) {
            if ($(button).val() == "Unselect") {
                $(button).parent("td").parent("tr").css({ "background-color": "#CDFF75" });
            }
            else {
                $(button).parent("td").parent("tr").css({ "background-color": "#ffffff" });
            }
        }

        $(function () {

            commentLinkEnable();
            //removeErrorMessage();
          
        });

        if (parent) {
            var win = getCurrentCustomDialog();
            if (parent.onClientBeforeClose) {
                win.remove_beforeClose(parent.onClientBeforeClose);
            }
            parent.onClientBeforeClose = localClientBeforeClose;
            win.add_beforeClose(parent.onClientBeforeClose);

        }
        function localClientBeforeClose(sender, eventArgs) {            
            arg = eventArgs.get_argument();
            if (arg) {
                
            }
            else {
                //call radconfirm
                radconfirm('Are you sure you wish to cancel? Any changes made will be lost.', confirmCallback, 500, 120, null, 'Confirm');
                //cancel event so that the user response is handled.
                eventArgs.set_cancel(true);
            }
        }

        function confirmCallback(arg) {
            if (arg) //the user clicked OK
            {
         
                removeTemporaryAssociation();
                var win = getCurrentCustomDialog();
                win.remove_beforeClose(parent.onClientBeforeClose);                
            }            
        }

        //function removeErrorMessage()
        //{
        //    $("[id$='cmbYear']").each(function () {
        //        $(this).focus(function () {                    
        //            $(this).css({ 'border-color': '#ffffff'});
        //        });
        //    });
        //}

    </script>
</asp:Content>

