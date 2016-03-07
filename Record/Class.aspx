<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false"
    CodeBehind="Class.aspx.cs" Inherits="Thinkgate.Record.Class" %>

<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
    <asp:Image runat='server' ID="classImage" ImageUrl="~/Images/new/class.png" Height="50px" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
    <asp:Label runat="server" ID="className" class="pageTitle" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" runat="server">
    <th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
        <script type="text/javascript">

            function Class_ClosePage() {
                closeWindow();
            }

//            function Class_openExpandEditRadWindow(sender, args) {
//                customDialog({ url: args.command.get_name(), title: "Content Editor - ITEM", width: 950, height: 675, autoSize: true, name: 'ContentEditorITEM', onClosed: ItemEditor_OnClientClose });
//            }


            function actionsMenuItemClicked(sender, args) {
                var text = args.get_item().get_text();

                switch (text) {
                    case "Delete":
                        customDialog({ title: "Delete Class?",
                            maximize: true, maxwidth: 500, maxheight: 100,
                            autoSize: false,
                            content: "<div>Are you sure you want to delete this class? Once deleted the class cannot be recovered.</div>",
                            dialog_style: 'confirm'
                        },
                    [{ title: "Cancel" },
                        { title: "Continue",
                            callback: deleteClass_Confirmed
                        }]);
                        break;

                    case "Copy":
                        customDialog({ title: "Copy class?",
                            maximize: true, maxwidth: 500, maxheight: 100,
                            autoSize: false,
                            content: "<div>You are about to copy this class. Do you wish to continue?</div>",
                            dialog_style: 'confirm'
                        },
                    [{ title: "Cancel" },
                        { title: "Continue",
                            callback: copyClass_Confirmed
                        }]);
                        break;
                }
            }

            function deleteClass_Confirmed() {
                var Class_hdnEncryptedData = $('#Class_hdnEncryptedData')[0];
                var panel = $find('deleteClassXmlHttpPanel');

                panel.set_value('{"ClassID":' + Class_hdnEncryptedData.value + '}');
            }

            function deleteClass_CallBack_Success(sender, args) {
                var results = args.get_content();

                if (results.indexOf("Error") > -1) {
                    customDialog({ title: "Class was not deleted.",
                        maximize: true, maxwidth: 500, maxheight: 100,
                        autoSize: false,
                        content: '<div style="width:100%; height:100%; text-align:left; vertical-align:middle;">' + results + '</div>'
                    },
                                [{ title: "Ok"}]);

                } else {
                    customDialog({ title: "Delete successful",
                        maximize: true, maxwidth: 500, maxheight: 100,
                                    autoSize: false,
                                    content: '<div style="width:100%; height:100%; text-align:left; vertical-align:middle;">' + results + '  This browser window will be closed.</div>',
                                    onClosed: closeWindow
                                },
                                [{ title: "Ok"}]);
                }

  

            }

            function copyClass_Confirmed() {
                var Class_hdnEncryptedData = $('#Class_hdnEncryptedData')[0];
                var panel = $find('copyClassXmlHttpPanel');

                panel.set_value('{"ClassID":' + Class_hdnEncryptedData.value + '}');
            }

            function copyClass_CallBack_Success(sender, args) {
                var results = args.get_content();
                if (results.indexOf('CLONE:') < 0) 
                {
                    var Class_hdnEncryptedData = $('#Class_hdnEncryptedData')[0];
                    var errorMsg = "Attempt to copy class failed for ID = '" + Class_hdnEncryptedData.value + "'. Please contact your system administrator.";
                    customDialog({ title: "Copy failed!",
                        maximize: true, maxwidth: 500, maxheight: 100,
                        autoSize: false,
                        dialog_style: "alert",
                        content: '<div style="width:100%; height:100%; text-align:center; vertical-align:middle;">' + errorMsg + '</div>'
                    },
                [{ title: "Ok"}]);


                }
                else 
                {
                    customDialog({ title: "Copy successful",
                        maximize: true, maxwidth: 500, maxheight: 100,
                        autoSize: false,
                        content: '<div style="width:100%; height:100%; text-align:center; vertical-align:middle;">This class was successfully copied.  It will now be loaded into the current page.</div>',
                        onClosed: function () { loadClassCopy(results); }} 
                    ,
                        [{ title: "Ok"}]);
                }
            }

            function loadClassCopy(results) {
                var parentURL = parent.window.location.href;
                var redirectURL = parentURL.substring(0, parentURL.indexOf('xID=') + 4) + results.substr(results.indexOf('CLONE:')+6);
                parent.window.location.href = redirectURL;

            }

            /* Need a variable to serve as a dirty flag. If user goes 
            into content editor and changes the item's values, 
            then we will need to refresh the page.                 */
            var hostWindowName = 'Class';
            var refreshNeeded;

            function ClassEditor_OnClientClose() {
                if (refreshNeeded) {
                    location.reload(true);
                }
                refreshNeeded = null;
            }
        </script>
    <telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="classTilesLoadingPanel"
        Width="100%" Height="100%">
        <th:DoublePanel runat="server" ID="ctlDoublePanel" />
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="classTilesLoadingPanel" runat="server" />
    <telerik:RadXmlHttpPanel runat="server" ID="deleteClassXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/ClassWCF.svc"
        WcfServiceMethod="DeleteClass" RenderMode="Block" OnClientResponseEnding="deleteClass_CallBack_Success">
    </telerik:RadXmlHttpPanel>
    <telerik:RadXmlHttpPanel runat="server" ID="copyClassXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/ClassWCF.svc"
        WcfServiceMethod="CopyClass" RenderMode="Block" OnClientResponseEnding="copyClass_CallBack_Success">
    </telerik:RadXmlHttpPanel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
    <input type="hidden" id="Class_hdnEncryptedData" value="" runat="server" ClientIDMode="Static"/>
</asp:Content>
