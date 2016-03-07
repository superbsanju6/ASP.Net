<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ImagePage.aspx.cs" Inherits="Thinkgate.Record.ImagePage" %>

<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
    <asp:Image runat='server' ID="imgItemImage" ImageUrl="~/Images/new/Image.png" Height="50px" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
    <asp:Label runat="server" ID="lblImageName" class="pageTitle" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" runat="server">
    <th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
        <script type="text/javascript">
            function ImagePage_ClosePage() {
                closeWindow();
            }

            function ImagePage_openExpandEditRadWindow(sender, args) {
                customDialog({ url: args.command.get_name(), title: "Content Editor - IMAGE", maximize: true, maxwidth: 950, maxheight: 675, autoSize: true, name: 'ContentEditorIMAGE', onClosed: ImageEditor_OnClientClose, destroyOnClose: true });
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
                    maximize: true, maxwidth: 500, maxheight: 100,
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
                        customDialog({ title: "Delete image?",
                            maximize: true, maxwidth: 500, maxheight: 100,
                            autoSize: false,
                            content: "<div>You are about to delete this image. Do you wish to continue?</div>",
                            dialog_style: 'confirm'
                        },
                    [{ title: "Cancel" },
                        { title: "Continue",
                            callback: deleteImage_Confirmed
                        }]);
                        break;
                }
            }

            function deleteImage_Confirmed() {
                var ImagePage_hdnEncryptedData = $('#ImagePage_hdnEncryptedData')[0];
                Service2.DeleteImageFromDatabase(ImagePage_hdnEncryptedData.value, deleteImage_CallBack_Success, deleteImage_CallBack_Failure);
            }

            function deleteImage_CallBack_Success(callBack_Results) {
                customDialog({ title: "Delete successful",
                    maximize: true, maxwidth: 500, maxheight: 100,
                    autoSize: false,
                    content: '<div style="width:100%; height:100%; text-align:left; vertical-align:middle;">Image deleted.  This browser window will be closed.</div>',
                    onClosed: closeWindow
                },
                [{ title: "Ok"}]);

            }

            function deleteImage_CallBack_Failure(callBack_Results) {
                if (callBack_Results) {
                    var errorMsg = callBack_Results;
                }
                else {
                    var errorMsg = "<div>Attempt to delete image failed for ID = '" + $$(hdnEncryptedData).nodeValue + "'.</div>";
                }
                alertUser("Delete failed!", errorMsg);
            }

            /* Need a variable to serve as a dirty flag. If user goes 
            into content editor and changes the item's values, 
            then we will need to refresh the page.                 */
            var hostWindowName = 'ImagePage';
            var refreshNeeded;

            function ImageEditor_OnClientClose() {
                if (refreshNeeded) {
                    location.reload(true);
                }
                refreshNeeded = null;
            }

        </script>
    <telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="ImageTilesLoadingPanel"
        Width="100%" Height="100%">
        <th:DoublePanel runat="server" ID="ctlDoublePanel" />
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="ImageTilesLoadingPanel" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
    <input type="hidden" id="ImagePage_hdnEncryptedData" value="" runat="server" ClientIDMode="Static"/>
</asp:Content>
