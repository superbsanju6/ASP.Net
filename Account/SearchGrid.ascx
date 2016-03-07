<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchGrid.ascx.cs" Inherits="Thinkgate.Account.SearchGrid" %>
<asp:Panel ID="pnlSearchGrid" runat="server" GroupingText="Search Criteria" Style="width: 400px; margin: 5px;" DefaultButton="btnSearch">
    <div style="margin: 5px;">
        <div style="margin: 5px;">
            <asp:Label runat="server" Text="Search:"></asp:Label>
            <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
            <telerik:RadButton OnClick="btnSearch_OnClick" Style="margin-left: 10px;" ID="btnSearch" Text="Search" Skin="Black" runat="server"></telerik:RadButton>
            <asp:Label ID="lblRequirement" Text="Min. 3 Chars" Font-Size="X-Small" ForeColor="Red" runat="server" Visible="false"></asp:Label>
        </div>
        <telerik:RadGrid ID="grdSearch" Visible="false" OnItemCommand="grdSearch_ItemCommand" OnColumnCreated="gridSearch_ColumnCreated" runat="server">
            <MasterTableView>
                <Columns>
                    <telerik:GridButtonColumn HeaderStyle-Width="30" Text="Select" ButtonType="PushButton" CommandName="Select"></telerik:GridButtonColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
</asp:Panel>
