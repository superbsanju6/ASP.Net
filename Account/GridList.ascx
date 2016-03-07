<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridList.ascx.cs" Inherits="Thinkgate.Account.GridList" %>
<telerik:RadGrid ID="grdList" runat="server" OnItemDataBound="grdList_ItemDataBound">
    <ClientSettings>
        <Scrolling AllowScroll="true" SaveScrollPosition="true"/>
    </ClientSettings>
</telerik:RadGrid>