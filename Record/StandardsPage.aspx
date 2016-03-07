<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="StandardsPage.aspx.cs" Inherits="Thinkgate.Record.Standards" %>

<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
    <asp:Image runat='server' ID="StandardImage" ImageUrl="../Images/Standards.png"
        Height="50px"  Visible="true"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
    <asp:Label runat="server" ID="StandardName" class="pageTitle" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" runat="server">
    <th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function StandardPage_ClosePage() {
            closeWindow();
        }

        function StandardPage_openExpandEditRadWindow(sender, args) {
            customDialog({ url: args.command.get_name(), title: "Content Editor - Standard", width: 950, height: 675, autoSize: true, onClosed: StandardEditor_OnClientClose });
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
                case "add":
                    		//alert("Add New standard - Under Construction");
                           // customDialog({ url: < % =ResolveUrl("../Dialogues/AddNewItem.aspx?xID=Item") % >, autoSize: true, name: 'NewItem' });
                    break;
            }
        }

    </script>
    <telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="districtTilesLoadingPanel"
        Width="100%" Height="100%">
        <th:DoublePanel runat="server" ID="ctlDoublePanel" />
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="districtTilesLoadingPanel"
        runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
</asp:Content>
