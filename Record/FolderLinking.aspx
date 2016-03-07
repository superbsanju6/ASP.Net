<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FolderLinking.aspx.cs" Inherits="Thinkgate.Record.FolderLinking" %>
<%@ Register TagPrefix="th" TagName="DoublePanel" Src="~/Controls/DoubleScrollPanel.ascx" %>
<%@ Register TagPrefix="th" Namespace="Thinkgate.Controls" Assembly="Thinkgate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderImageContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FoldersContent" runat="server">
    <th:Folders runat="server" ID="ctlFolders" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadAjaxPanel ID="doubleRotatorPanel" runat="server" LoadingPanelID="folderLinkingTilesLoadingPanel"
        Width="100%" Height="100%">
        <th:DoublePanel runat="server" ID="ctlDoublePanel" />
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="folderLinkingTilesLoadingPanel"
        runat="server" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageFooter" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ExcelButtonContent" runat="server">
</asp:Content>
