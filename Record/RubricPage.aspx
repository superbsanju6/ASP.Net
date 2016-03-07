<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RubricPage.aspx.cs" Inherits="Thinkgate.Record.RubricPage" %>

<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
    <asp:Image runat='server' ID="RubricImage" Height="50px"  />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
    <asp:Label runat="server" ID="RubricName" class="pageTitle" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" runat="server">
    <th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
        <script type="text/javascript">
            function RubricPage_ClosePage() {
                closeWindow();
            }

            function RubricPage_openExpandEditRadWindow(sender, args) {
                customDialog({ url: args.command.get_name(), title: "Content Editor - RUBRIC", width: 950, height: 675, autoSize: true, name: 'ContentEditorRUBRIC', onClosed: RubricEditor_OnClientClose, destroyOnClose: true });
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
                        customDialog({ title: "Delete Rubric?",
                            maximize: true, maxwidth: 500, maxheight: 100,
                            autoSize: false,
                            content: "<div>You are about to delete this rubric. Do you wish to continue?</div>",
                            dialog_style: 'confirm'
                        },
                    [{ title: "Cancel" },
                        { title: "Continue",
                            callback: deleteRubric_Confirmed
                        }]);
                        break;
                }
            }

            function deleteRubric_Confirmed() {
                var RubricPage_hdnEncryptedData = $('#RubricPage_hdnEncryptedData')[0];
                Service2.DeleteRubricFromDatabase(RubricPage_hdnEncryptedData.value, deleteRubric_CallBack_Success, deleteRubric_CallBack_Failure);
            }

            function deleteRubric_CallBack_Success(callBack_Results) {
                var title, msg;

                if (callBack_Results == "") {
                    customDialog({
                            title: "Delete successful",
                            maximize: true, maxwidth: 500, maxheight: 100,
                            autoSize: false,
                            content: '<div style="width:100%; height:100%; text-align:left; vertical-align:middle;">Rubric deleted.  This browser window will be closed.</div>',
                            onClosed: closeWindow
                        },
                        [{ title: "Ok"}]);
                    } else {
                    {
                        customDialog({
                            title: "Unable to delete Rubric",
                            maximize: true, maxwidth: 500, maxheight: 100,
                            autoSize: false,
                            content: '<div style="width:100%; height:100%; text-align:left; vertical-align:middle;">Unable to delete rubric. It is still being used in the following questions: ' + callBack_Results + '.</div>'
                        },
                        [{ title: "Ok"}]);
                        
                    }
                }
            }

            function deleteRubric_CallBack_Failure(callBack_Results) {
                if (callBack_Results) {
                    var errorMsg = callBack_Results;
                }
                else {
                    var RubricPage_hdnEncryptedData = $('#RubricPage_hdnEncryptedData')[0];
                    var errorMsg = "<div>Attempt to delete rubric failed for ID = '" + RubricPage_hdnEncryptedData.value + "'.</div>";
                }
                alertUser("Delete failed!", errorMsg);
            }


            /* Need a variable to serve as a dirty flag. If user goes 
            into content editor and changes the item's values, 
            then we will need to refresh the page.                 */
            var hostWindowName = 'RubricPage';
            var refreshNeeded;

            function RubricEditor_OnClientClose() {
                if (refreshNeeded) {
                    location.reload(true);
                }
                refreshNeeded = null;
            }

        </script>
    <telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="RubricTilesLoadingPanel"
        Width="100%" Height="100%">
        <th:DoublePanel runat="server" ID="ctlDoublePanel" />
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="RubricTilesLoadingPanel" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
    <input type="hidden" id="RubricPage_hdnEncryptedData" value="" runat="server" ClientIDMode="Static"/>
</asp:Content>
