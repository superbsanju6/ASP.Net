<%@ Page Title="" Language="C#" MasterPageFile="~/AddNew.Master" AutoEventWireup="true" CodeBehind="AddUrl.aspx.cs" Inherits="Thinkgate.Controls.Credentials.AddUrl" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function closeUrlSave() {
            $('[id$="btnHiddenBind"]', parent.document).click();
            var oWnd = getCurrentCustomDialog();
            setTimeout(function () { oWnd.close(); }, 100);
        }
        function CloseWindow() {
            var oWnd = getCurrentCustomDialog();
            setTimeout(function () { oWnd.close(); }, 100);
        }
        function urlValidationFailMessage() {
            setTimeout(function () {
                customDialog({ title: "URL", width: 260, height: 140, dialog_style: "alert", content: "Invalid URL Text entered." }, [{ title: "OK" }]);

                var btnSave = $find("RadButtonsave");
                if (btnSave) {
                    btnSave.set_enabled(true);
                }
            }, 100);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table style="margin-left: 8px; margin-top: 12px;">
        <tr style="line-height: 25px; height: 25px;">
            <td colspan="2">Enter URL text below.
            </td>
        </tr>
        <tr style="line-height: 25px; height: 25px;">
            <td colspan="2">
                <telerik:RadTextBox runat="server" ID="radUrl" MaxLength="250" TextMode="MultiLine" Height="50px" ClientIDMode="Static" Width="420px"></telerik:RadTextBox>
                <asp:HiddenField ID="hdCommentId" runat="server" />
            </td>
        </tr>
    </table>
    <div style="margin-top: 45px; margin-right: 10px; float: right;">
        <telerik:RadButton ID="RadButtonsave" Skin="Web20" ClientIDMode="Static" Width="50px" UseSubmitBehavior="false" runat="server"
            Text="OK" OnClick="RadButtonsave_Click" OnClientClicking="checkButton">
        </telerik:RadButton>
        <telerik:RadButton ID="RadButtonCancel" Skin="Web20" runat="server" Text="Cancel" Width="60px" UseSubmitBehavior="false" AutoPostBack="false" OnClientClicked="CloseWindow"></telerik:RadButton>
    </div>

    <script type="text/jscript">
        $(document).ready(function () {
            var btnSave = $("#RadButtonsave")
            btnSave.addClass("rbDisabled");

            $('[id$="radUrl"]').on('input propertychange paste', function () {
                if ($('[id$="radUrl"]').val().length > 0)
                    btnSave.removeClass("rbDisabled");
                else
                    btnSave.addClass("rbDisabled");
            });
        });
        function checkButton(button, args) {
            var btnSave1 = $("#RadButtonsave")
            if (btnSave1.hasClass("rbDisabled"))
                button.set_autoPostBack(false);
            else
                button.set_autoPostBack(true);
        }
    </script>
</asp:Content>
