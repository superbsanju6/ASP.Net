<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegionalCoordinator.aspx.cs" Inherits="Thinkgate.Record.RegionalCoordinator" %>
<%@ Register Src="~/Controls/DoubleScrollPanel.ascx" TagName="DoublePanel" TagPrefix="th" %>
<%@ Register Src="~/Controls/Folders.ascx" TagName="Folders" TagPrefix="th" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeaderImageContent" runat="server">
    <asp:Image runat='server' ID="lcoImage" ImageUrl="~/Images/new/female_teacher.png" Visible="false" Height="50px"  />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">
    <asp:Label runat="server" ID="lcoName" class="pageTitle" Text="Local Courses" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="FoldersContent" Visible=false runat="server">
    <th:Folders runat="server" Visible=false ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="lcoTilesLoadingPanel"
        Width="100%" Height="100%">
        <th:DoublePanel runat="server" ID="ctlDoublePanel" />       
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="lcoTilesLoadingPanel"
        runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
</asp:Content>
