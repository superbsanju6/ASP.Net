<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Staff.aspx.cs" Inherits="Thinkgate.Record.Staff" %>

<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">    
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
    <asp:Image runat='server' ID="staffImage" ImageUrl="~/Images/blankperson.png" Height="50px"  />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
    <asp:Label runat="server" ID="staffName" class="pageTitle" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" runat="server">
    <th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../Scripts/MinimumPasswordRequirement/MinimumPasswordRequirements.js" type="text/javascript"></script>
    <script type="text/javascript">
        function actionsMenuItemClicked(sender, args) {
            var text = args.get_item().get_text();

            switch (text) {
                case "Delete":
                    customDialog({ title: "Delete Staff?",
                        maximize: true, maxwidth: 500, maxheight: 100,
                        autoSize: false,
                        content: "<div>Are you sure you want to delete this user?</div>",
                        dialog_style: 'confirm'
                    },
                    [{ title: "Cancel" },
                        { title: "OK",
                            callback: deleteStaff
                        }]);
                    break;
            }
        }

        function deleteStaff() {
            var staffGuid = $('#StaffGuid').attr('value');
            var panel = $find('deleteStaffXmlHttpPanel');
            panel.set_value(staffGuid);

//            var staffPageEncryptedID = $('#StaffPageEncryptedID').attr('value');
//            var panel = $find('deleteStaffXmlHttpPanel');
//            panel.set_value('{"StaffID":"' + staffPageEncryptedID + '"}');
        }

        function deleteStaffConfirm(sender, args) {
            var sTitle = "Delete successful";
            var sMsg = "Staff deleted.  This browser window will be closed.";

            customDialog({ title: sTitle,
                maximize: true, maxwidth: 500, maxheight: 100,
                autoSize: false,
                content: '<div style="width:100%; height:100%; text-align:left; vertical-align:middle;">' + sMsg + '</div>',
                onClosed: closeWindow
            },
                [{ title: "OK"}]);

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
    </script>
    <telerik:RadXmlHttpPanel runat="server" ID="deleteStaffXmlHttpPanel" ClientIDMode="Static" EnableClientScriptEvaluation="true" OnServiceRequest="deleteStaffXmlHttpPanel_ServiceRequest" RenderMode="Block" OnClientResponseEnding="deleteStaffConfirm">
    </telerik:RadXmlHttpPanel>
    <telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="staffTilesLoadingPanel"
        Width="100%" Height="100%">
        <th:DoublePanel runat="server" ID="ctlDoublePanel" />
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="staffTilesLoadingPanel"
        runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
    <input type="hidden" id="StaffPageEncryptedID" value="" runat="server" ClientIDMode="Static"/>
    <input type="hidden" id="StaffGuid" value="" runat="server" ClientIDMode="Static"/>
</asp:Content>
