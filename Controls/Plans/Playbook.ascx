<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Playbook.ascx.cs" Inherits="Thinkgate.Controls.Plans.Playbook" %>

<telerik:RadAjaxPanel ID="PlaybookAjaxPanel" runat="server" LoadingPanelID="PlaybookLoadingPanel">
    <telerik:RadGrid ID="grdPlaybooks" Height="210px" runat="server" Skin="Web20" AutoGenerateColumns="false" ClientSettings-Scrolling-AllowScroll="true" OnItemDataBound="grdPlaybooks_ItemDataBound">
        <MasterTableView>
            <Columns>
                <telerik:GridBoundColumn HeaderText="ID" DataField="ID" UniqueName="ID" Visible="false"></telerik:GridBoundColumn>
                <telerik:GridHyperLinkColumn HeaderText="Monitoring Interval" DataTextField="MonitoringInterval" UniqueName="link" Target="_blank"></telerik:GridHyperLinkColumn>
                <telerik:GridBoundColumn HeaderText="Status" DataField="DocStatus" UniqueName="DocStatus"></telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</telerik:RadAjaxPanel>
<div runat="server" id="BtnAdd" class="searchTileBtnDiv_smallTile" title="Add Playbook" style="margin-top: 1px;">
<span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon">
		</span>
		<div style="padding: 0;">
				Add New</div>
</div>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="PlaybookLoadingPanel" runat="server" />
