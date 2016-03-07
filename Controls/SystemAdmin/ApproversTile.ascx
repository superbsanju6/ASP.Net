<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApproversTile.ascx.cs" Inherits="Thinkgate.Controls.SystemAdmin.ApproversTile" %>
<telerik:RadAjaxPanel ID="approversAjaxPanel" runat="server" LoadingPanelID="approversLoadingPanel">
    <div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
        <telerik:RadComboBox ID="cmbLevel" runat="server" ToolTip="Select a level" Skin="Web20" Width="69" AutoPostBack="true" CausesValidation="False" HighlightTemplatedItems="true">
            <ItemTemplate>
                <span><%# Eval("Level") %></span>
            </ItemTemplate>
        </telerik:RadComboBox>
        <telerik:RadComboBox ID="cmbElement" runat="server" ToolTip="Select an element type" Skin="Web20" Width="69" AutoPostBack="true" CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
            <ItemTemplate>
                <span><%# Eval("ElementType") %></span>
            </ItemTemplate>
        </telerik:RadComboBox>
    </div>
    <div class="graphicalView" id="divGraphicalView" runat="server">
        <asp:Panel ID="pnlNoResults" runat="server" Visible="false" Height="190px">
            <div style="width: 100%; text-align: center;">No approvers found for selected criteria.</div>
        </asp:Panel>
        <telerik:RadListBox runat="server" ID="lbx" Width="100%" Height="190px" OnItemDataBound="lbxList_ItemDataBound">
            <ItemTemplate>
                <asp:Image ID="personImg" Style="float: left; padding: 2px;" Width="47" Height="56" ImageUrl='~/Images/blankperson.png' runat="server" />
                <div>
                    <asp:HyperLink ID="lnkApprover" runat="server" Target="_blank" Visible="True"></asp:HyperLink>
                    <asp:Label ID="lblDesc" runat="server"></asp:Label>
                </div>
            </ItemTemplate>
        </telerik:RadListBox>
    </div>
</telerik:RadAjaxPanel>
<div runat="server" id="BtnAdd" class="searchTileBtnDiv_smallTile" title="Manage Approvers" style="margin-top: 1px;"
    onclick="tbd">
    <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
    <div style="padding: 0;">
        Manage Approvers
    </div>
</div>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="approversLoadingPanel" runat="server" />
