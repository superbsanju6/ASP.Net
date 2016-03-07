<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentTargets.ascx.cs"
    Inherits="Thinkgate.Controls.Assessment.AssessmentTargets" %>
<telerik:RadAjaxPanel ID="TargetsAjaxPanel" runat="server" LoadingPanelID="TargetsLoadingPanel">
</telerik:RadAjaxPanel>
<telerik:RadGrid ID="grdTargets" runat="server" Skin="Web20" AutoGenerateColumns="false" OnItemDataBound="grdTargets_ItemDataBound">
    <MasterTableView>
        <Columns>
            <telerik:GridBoundColumn Visible="false" DataField="ID" UniqueName="TestID"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Type" DataField="type"></telerik:GridBoundColumn>
            <telerik:GridHyperLinkColumn HeaderText="Name" DataTextField="name" UniqueName="link" Target="_blank"></telerik:GridHyperLinkColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="TargetsLoadingPanel" runat="server" />
