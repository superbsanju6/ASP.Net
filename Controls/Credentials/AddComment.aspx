<%@ Page Title="Comments" Language="C#" MasterPageFile="~/AddNew.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="AddComment.aspx.cs" Inherits="Thinkgate.Controls.Credentials.AddComment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <telerik:RadCodeBlock ID="jsAddComment" runat="server">
    <script type="text/javascript">
        function closeSaveWindow() {
            window.close();
            parent.location.reload();
        }
            function getCurrentCustomDialog() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function closeWindow() {
                getCurrentCustomDialog().close();
            }

            function onButtonSave(sender, args) {

                var button = sender;
                if (button) {

                    button.set_enabled(false);
        }
                return true;
            }

    </script>
    </telerik:RadCodeBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table style="margin-left: auto; margin-right: auto; margin-top: 12px;">
        <tr>
            <td style="text-align:center">
                <telerik:RadTextBox runat="server" ID="radcomment" MaxLength="250" TextMode="MultiLine" ClientIDMode="Static" Width="400px" Height="115px"></telerik:RadTextBox>
                <asp:HiddenField ID="hdCommentId" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="padding-top: 10px;">
                <div style="float: right; padding-left: 15px;">
                    <telerik:RadButton ID="RadButtonCancel" Skin="Web20" runat="server" Text="Cancel" Width="80px" OnClientClicked="closeWindow" ClientIDMode="Static"></telerik:RadButton>
                </div>
                <div style="float: right;">
                    <telerik:RadButton ID="RadButtonsave" Skin="Web20" Enabled="false" ClientIDMode="Static" Width="80px" runat="server" Text="Save" OnClick="RadButtonsave_Click" OnClientClicked="onButtonSave" UseSubmitBehavior="false"></telerik:RadButton>
                </div>
            </td>
        </tr>
    </table>
    <script type="text/jscript">

        $(document).ready(function () {
            $('#radcomment').bind("keyup", function () {
                var comment = $('#radcomment').val();
                comment = $.trim(comment);
                if (comment != "") {
                    var btnSave = $find("RadButtonsave");
                    btnSave.set_enabled(true);
                }
                else {
                    var btnSave = $find("RadButtonsave");
                    btnSave.set_enabled(false);
                }
            });
        });
    </script>
</asp:Content>
