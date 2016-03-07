<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TestQuestionPage.aspx.cs" Inherits="Thinkgate.Record.TestQuestionPage" %>
<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>
    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    </asp:Content>
    <asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
        <asp:Image runat='server' ID="ItemImage" ImageUrl="~/Images/Star_Icon.png" Height="50px" />
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
        <asp:Label runat="server" ID="ItemName" class="pageTitle" />
    </asp:Content>
    <asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" runat="server">
        <th:Folders runat="server" ID="ctlFolders" />
    </asp:Content>
    <asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
        <script type="text/javascript">

            function ItemPage_ClosePage() {
                closeWindow();
            }

//            function ItemPage_openExpandEditRadWindow(sender, args) {
//                customDialog({ url: args.command.get_name(), title: "Content Editor - ITEM", width: 950, height: 675, autoSize: true, name: 'ContentEditorITEM', onClosed: ItemEditor_OnClientClose });
//            }

//            function notifyUser(title, content) {
//                customDialog({ title: title,
//                    height: 75,
//                    width: 450,
//                    autoSize: false,
//                    content: '<div style="width:100%; height:100%; text-align:center; vertical-align:middle;">' + content + '</div>'
//                },
//                [{ title: "Ok"}]);
//            }

//            function alertUser(title, content) {
//                customDialog({ title: title,
//                    height: 75,
//                    width: 450,
//                    autoSize: false,
//                    dialog_style: "alert",
//                    content: '<div style="width:100%; height:100%; text-align:center; vertical-align:middle;">' + content + '</div>'
//                },
//                [{ title: "Ok"}]);
//            }

            function actionsMenuItemClicked(sender, args) {
                var text = args.get_item().get_text();

                switch (text) {
//                    case "Delete":
//                        customDialog({ title: "Delete Item?",
//                            height: 75,
//                            width: 450,
//                            autoSize: false,
//                            content: "<div>You are about to delete this item. Do you wish to continue?</div>",
//                            dialog_style: 'confirm'
//                        },
//                    [{ title: "Cancel" },
//                        { title: "Continue",
//                            callback: deleteItem_Confirmed
//                        }]);
//                        break;

//                    case "Copy":
//                        customDialog({ title: "Copy Item?",
//                            height: 75,
//                            width: 450,
//                            autoSize: false,
//                            content: "<div>You are about to copy this item to your personal bank. Do you wish to continue?</div>",
//                            dialog_style: 'confirm'
//                        },
//                    [{ title: "Cancel" },
//                        { title: "Continue",
//                            callback: copyItem_Confirmed
//                        }]);
//                        break;
                }
            }

//            function deleteItem_Confirmed() {
//                var ItemPage_hdnEncryptedData = $('#ItemPage_hdnEncryptedData')[0];
//                Service2.DeleteBankQuestionFromDatabase(ItemPage_hdnEncryptedData.value, deleteItem_CallBack_Success, deleteItem_CallBack_Failure);
//            }

//            function deleteItem_CallBack_Success(callBack_Results) {
//                customDialog({ title: "Delete successful",
//                    height: 75,
//                    width: 450,
//                    autoSize: false,
//                    content: '<div style="width:100%; height:100%; text-align:left; vertical-align:middle;">Item deleted.  This browser window will be closed.</div>',
//                    onClosed: closeWindow
//                },
//                [{ title: "Ok"}]);

//            }

//            function deleteItem_CallBack_Failure(callBack_Results) {
//                if (callBack_Results) {
//                    var errorMsg = callBack_Results;
//                }
//                else {
//                    var errorMsg = "<div>Attempt to delete Item failed for ID = '" + $$(hdnEncryptedData).nodeValue + "'.</div>";
//                }
//                alertUser("Delete failed!", errorMsg);
//            }


//            function copyItem_Confirmed() {
//                var ItemPage_hdnEncryptedData = $('#ItemPage_hdnEncryptedData')[0];
//                Service2.CopyToUserPersonalBank(ItemPage_hdnEncryptedData.value, copyItem_CallBack_Success, copyItem_CallBack_Failure);
//            }

//            function copyItem_CallBack_Success(callBack_Results) {
//                notifyUser("Copy successful", "This item has been copied to your personal bank.");
//            }

//            function copyItem_CallBack_Failure(callBack_Results) {
//                if (callBack_Results) {
//                    var errorMsg = callBack_Results;
//                }
//                else {
//                    var ItemPage_hdnEncryptedData = $('#ItemPage_hdnEncryptedData')[0];
//                    var errorMsg = "<div>Attempt to copy Item failed for ID = '" + ItemPage_hdnEncryptedData.value + "'.<div>";
//                }
//                alertUser("Copy failed!", errorMsg);
//            }

//            /* Need a variable to serve as a dirty flag. If user goes 
//            into content editor and changes the item's values, 
//            then we will need to refresh the page.                 */
//            var hostWindowName = 'ItemPage';
//            var refreshNeeded;

//            function ItemEditor_OnClientClose() {
//                if (refreshNeeded) {
//                    location.reload(true);
//                }
//                refreshNeeded = null;
//            }
        </script>
        <telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="ItemTilesLoadingPanel"
            Width="100%" Height="100%">
            <th:DoublePanel runat="server" ID="ctlDoublePanel" />
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="ItemTilesLoadingPanel" runat="server" />
    </asp:Content>
    <asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
        <input type="hidden" id="ItemPage_hdnEncryptedData" value="" runat="server" ClientIDMode="Static"/>
    </asp:Content>
