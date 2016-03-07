<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardsFilter.ascx.cs" Inherits="Thinkgate.Controls.Standards.StandardsFilter" %>
<script type="text/javascript">
    function ClientNodeCollapsing(sender, args) {
        args.set_cancel(true);
    }
</script>
<telerik:RadAjaxPanel runat="server" ID="standardsFilterPanel" LoadingPanelID="standardsFilterLoadingPanel" >
<div style="width:310px; height: 215px; overflow:auto;">
    <div id="standardsFilterDefaultTextSpan" runat="server" style="margin-top:25%; text-align:center;">&lt;&lt; Select Add New to create a filter &gt;&gt;</div>

    <telerik:RadTreeView ID="standardsFilterRadTree" runat="server" OnNodeDataBound="StandardsFilterRadTree_NodeDataBound" AutoGenerateColumns="false" 
    EnableEmbeddedSkins="false" Skin="Thinkgate_TreeView" OnClientNodeCollapsing="ClientNodeCollapsing">
        <DataBindings>
            <telerik:RadTreeNodeBinding Expanded="true" />
        </DataBindings>
        <NodeTemplate>
            <span><%# Eval("Name") %></span> 
            <asp:ImageButton ID="editLink" ClientIDMode="Static" runat="server" AlternateText="Edit Standard Filter" ImageUrl="../../Images/Edit.png" Visible="false" />
        </NodeTemplate>
    </telerik:RadTreeView>
</div>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="standardsFilterLoadingPanel" runat="server" />

<span style="display:none;"><telerik:RadButton runat="server" ID="standardFilterRefreshTrigger" ClientIDMode="Static" Text="Refresh" OnClick="StandardFilterRefresh_Click" AutoPostBack="true" /></span>

<div runat="server" id="BtnAdd" class="searchTileBtnDiv_smallTile" title="Add Standards Filter" style="margin-top: -1px;" 
    onclick="customDialog({ name: 'RadWindowAddAssessment', url: '../Dialogues/Assessment/CreateAssessmentOptions.aspx', title: 'Options to Create Assessment', maximize: true, maxwidth: 550, maxheight: 450});">
    <span runat="server" id="BtnAddSpan" class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
    <div style="padding: 0;" runat="server" id="BtnAddDiv">Add New</div>
</div>
