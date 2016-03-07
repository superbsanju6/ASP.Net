<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AddendumPage.aspx.cs" Inherits="Thinkgate.Record.AddendumPage" %>

<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
    <asp:Image runat='server' ID="AddendumImage" ImageUrl="~/Images/new/Addendum.png" Height="50px"  />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
    <asp:Label runat="server" ID="AddendumName" class="pageTitle" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" runat="server">
    <th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
        <script type="text/javascript">
            function AddendumPage_ClosePage() {
                closeWindow();
            }

            function AddendumPage_openExpandEditRadWindow(sender, args) {
                customDialog({ url: args.command.get_name(), title: "Content Editor - ADDENDUM", maximize: true, maxwidth: 950, maxheight: 675, autoSize: true, name: 'ContentEditorADDENDUM', onClosed: AddendumEditor_OnClientClose, destroyOnClose: true });
            }

            function notifyUser(title, content) {
                customDialog({ title: title,
                    maximize: true, maxwidth: 500, maxheight: 100,
                    autoSize: false,
                    content: '<div style="width:100%; height:100%; text-align:center; vertical-align:middle;">' + content + '</div>'
                },
                [{ title: "Ok"}]);
            }

            function alertUser(title, content) {
                customDialog({ title: title,
                    maximize: true, maxwidth: 1500, maxheight: 100,
                    autoSize: false,
                    dialog_style: "alert",
                    content: '<div style="width:100%; height:100%; text-align:center; vertical-align:middle;">' + content + '</div>'
                },
                [{ title: "Ok"}]);
            }

            function actionsMenuItemClicked(sender, args) {
                var text = args.get_item().get_text();

                switch (text) {
                    case "Delete":
                        customDialog({ title: "Delete Addendum?",
                            maximize: true, maxwidth: 500, maxheight: 100,
                            autoSize: false,
                            content: "<div>You are about to delete this addendum. Do you wish to continue?</div>",
                            dialog_style: 'confirm'
                        },
                    [{ title: "Cancel" },
                        { title: "Continue",
                            callback: deleteAddendum_Confirmed
                        }]);
                        break;
                }
            }

            function deleteAddendum_Confirmed() {
                var AddendumPage_hdnEncryptedData = $('#AddendumPage_hdnEncryptedData')[0];
                Service2.DeleteAddendumFromDatabase(AddendumPage_hdnEncryptedData.value, deleteAddendum_CallBack_Success, deleteAddendum_CallBack_Failure);
            }

            function deleteAddendum_CallBack_Success(callBack_Results) {
                var title, msg;

                if (callBack_Results == "") {
                    customDialog({
                        title: "Delete successful",
                        maximize: true, maxwidth: 500, maxheight: 100,
                        autoSize: false,
                        content: '<div style="width:100%; height:100%; text-align:left; vertical-align:middle;">Addendum deleted.  This browser window will be closed.</div>',
                        onClosed: closeWindow
                    },
                        [{ title: "Ok"}]);
                } else {
                    {
                        customDialog({
                            title: "Unable to delete Addendum",
                            maximize: true, maxwidth: 500, maxheight: 100,
                            autoSize: false,
                            content: '<div style="width:100%; height:100%; text-align:left; vertical-align:middle;">Unable to delete Addendum. It is still being used in the following questions: ' + callBack_Results + '.</div>'
                        },
                        [{ title: "Ok"}]);

                    }
                }
            }

            function deleteAddendum_CallBack_Failure(callBack_Results) {
                if (callBack_Results) {
                    var errorMsg = callBack_Results;
                }
                else {
                    var AddendumPage_hdnEncryptedData = $('#AddendumPage_hdnEncryptedData')[0];
                    var errorMsg = "<div>Attempt to delete Addendum failed for ID = '" + AddendumPage_hdnEncryptedData.value + "'.</div>";
                }
                alertUser("Delete failed!", errorMsg);
            }

            /* Need a variable to serve as a dirty flag. If user goes 
            into content editor and changes the item's values, 
            then we will need to refresh the page.                 */
            var hostWindowName = 'AddendumPage';
            var refreshNeeded;

            function AddendumEditor_OnClientClose() {
                if (refreshNeeded) {
                    location.reload(true);
                }
                refreshNeeded = null;
            }

        </script>
    <telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="AddendumTilesLoadingPanel"
        Width="100%" Height="100%">
        <th:DoublePanel runat="server" ID="ctlDoublePanel" />
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="AddendumTilesLoadingPanel" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
    <input type="hidden" id="AddendumPage_hdnEncryptedData" value="" runat="server" ClientIDMode="Static"/>
</asp:Content>
