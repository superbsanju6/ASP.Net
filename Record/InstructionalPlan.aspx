<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="InstructionalPlan.aspx.cs" Inherits="Thinkgate.Record.InstructionalPlan" %>

<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function InstructionalPlan_ClosePage() {
            closeWindow();
        }

        function expandPacingTile(sender, args) {
            var view = $find('pacingTileRadTabStrip').get_selectedTab().get_text();
            ShowInstructionalPlanCalendar("", "", view);
        }

        function editPacingTile(sender, args) {
            customDialog({ url: '<% =ResolveUrl("../Dialogues/Pacing.aspx?xID=" + _planID) %>',
                autoSize: true, name: 'EditPacing', onClosed: checkAndRefreshPacingTile
            });
        }

        function ShowInstructionalPlanCalendar(type, typeID, view) {
            var sUrl = '<% =ResolveUrl("../Dialogues/InstructionalPlanCalendar.aspx?xID=" + _planID) %>' + "&" + type + "=" + typeID;

            if (view)
                sUrl += "&view=" + view;

            customDialog({ url: sUrl,
                autoSize: false, maximize: true, maxwidth: 825, maxheight: 680, name: 'InstructionalPlanCalendar', onClosed: checkAndRefreshPacingTile
            });
        }

        function ShowInstructionalPlanCalendar_Day(date) {
            customDialog({ url: '<% =ResolveUrl("../Dialogues/InstructionalPlanCalendar.aspx?xID=" + _planID) %>' + "&day=" + date,
                autoSize: false, maximize: true, maxwidth: 825, maxheight: 680, name: 'InstructionalPlanCalendar' //, onClosed: checkAndRefreshPacingTile
            });
        }

        function checkAndRefreshPacingTile(sender, args) {
            if (refreshPacingTile)
                refreshPacingTile();
        }

        function actionsMenuItemClicked(sender, args) {
            var text = args.get_item().get_text();

            /*switch (text) {
                case "Close":
                    
                    break;
            }*/
        }

    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
    <asp:Image runat='server' ID="teacherImage" ImageUrl="~/Images/calendar.png" Height="50px" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
    <asp:Label runat="server" ID="planName" class="pageTitle" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" runat="server">
    <th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="teacherTilesLoadingPanel"
        Width="100%" Height="100%">
        <th:DoublePanel runat="server" ID="ctlDoublePanel" />
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="teacherTilesLoadingPanel"
        runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
</asp:Content>
