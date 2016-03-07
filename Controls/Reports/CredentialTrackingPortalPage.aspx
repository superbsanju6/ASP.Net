<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CredentialTrackingPortalPage.aspx.cs" Inherits="Thinkgate.Controls.Reports.CredentialTrackingPortalPage" %>
<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        function setDocumentTitle(reportTitle) {
            if (reportTitle != null)
                document.title = reportTitle;
        }

        function criteriaSliderGo() {
            var $criteria_holder = $('#criteriaHolder');
            var current_state = $criteria_holder.attr('state');

            if (!$criteria_holder) return false;

            if (!current_state) {
                current_state = 'opened';
                $criteria_holder.attr('state', current_state);
            }
            if (current_state == 'opened') {
                $criteria_holder.stop().animate({ width: 1 }, 500, function () { $criteria_holder.hide(); });     // find that going doesn't work properly in Chrome and Safari. It's like the calculation goes haywire and the div jumps back to the full 240px wide instead of disappearing
                current_state = 'closed';
                $('#columnExpanderHandleImage').attr('src', '../../Images/arrow_gray_right.gif');

            } else {
                //$('#criteriaScroller').css('overflow', 'hidden');
                $criteria_holder.stop().animate({ width: 200 }, 500, function () { $criteria_holder.show(); $('#criteriaScroller').css('overflow', 'hidden'); });
                current_state = 'opened';
                $('#columnExpanderHandleImage').attr('src', '../../Images/arrow_gray_left.gif');
            }

            $criteria_holder.attr('state', current_state);
        }
		</script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
		<asp:Image runat='server' ID="dataAnalysisImage" ImageUrl="~/Images/new/folder_data_analysis.png" Height="50px" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
		<asp:Label runat="server" ID="pageTitleLabel" class="pageTitle" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" runat="server">
		<th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
		<telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="CompetencyTrackingPortalPageTilesLoadingPanel"
				Width="100%" Height="100%">
				<th:DoublePanel runat="server" ID="ctlDoublePanel" />
		</telerik:RadAjaxPanel>
		<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="CompetencyTrackingPortalPageTilesLoadingPanel"
				runat="server" />
				<asp:TextBox ID="hiddenTxtBox" runat="server" Style="visibility: hidden;" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
		
</asp:Content>
