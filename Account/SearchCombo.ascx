<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCombo.ascx.cs" Inherits="Thinkgate.Account.SearchCombo" %>
<asp:Panel ID="pnlSearchCombo" runat="server" GroupingText="Search Criteria" Style="width: 400px; margin: 5px;">
    <telerik:RadComboBox ID="cmb" BackColor="White" Filter="Contains" EmptyMessage="Select..." AutoPostBack="true"
        OnSelectedIndexChanged="cmb_SelectedIndexChanged" runat="server" Style="width: auto;">
    </telerik:RadComboBox>
</asp:Panel>