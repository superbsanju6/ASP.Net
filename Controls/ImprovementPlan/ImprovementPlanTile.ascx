<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImprovementPlanTile.ascx.cs" Inherits="Thinkgate.Controls.ImprovementPlan.ImprovementPlanTile" %>

<telerik:RadAjaxPanel ID="ImprovementPlanTileAjaxPanel" runat="server" LoadingPanelID="ImprovementPlanTileLoadingPanel" >
    <div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
        <telerik:RadComboBox ID="cmbYearDistrict" runat="server" ClientIDMode="Static" ToolTip="Select a year" Skin="Web20" Width="69" Height="200px" AutoPostBack="true" CausesValidation="False" HighlightTemplatedItems="true" OnSelectedIndexChanged="cmbYear_OnSelectedIndexChanged"/>
        <telerik:RadComboBox ID="cmbYearSchool" runat="server" ClientIDMode="Static" ToolTip="Select a year" Skin="Web20" Width="69" Height="200px" AutoPostBack="true" CausesValidation="False" HighlightTemplatedItems="true" OnSelectedIndexChanged="cmbYear_OnSelectedIndexChanged"/>
        <telerik:RadComboBox ID="cmbSchool" runat="server" ClientIDMode = "Static" ToolTip="Select a school" Skin="Web20" Width="69" Height="200px" AutoPostBack="true" CausesValidation="False" HighlightTemplatedItems="true" OnSelectedIndexChanged="cmbSchool_OnSelectedIndexChanged" DropDownWidth="200px"  />
	</div>

    <div id="divTile" runat="server">
				<asp:Panel ID="pnlNoResults" runat="server" Visible="false" Height="190px">
					<div style="width: 100%; text-align: center;">No Improvement Plan found for selected criteria.</div>
				</asp:Panel>
			<telerik:RadListBox runat="server" ID="lbxImprovementPlanList" Width="100%" Height="190px" OnItemDataBound="lbxList_ItemDataBound" >
				<ItemTemplate>
					<asp:Image ID="improvementPlanImg" Style="float: left; padding: 2px;" Width="47" Height="56" ImageUrl='~/Images/resources.png' runat="server" />
					<div>
						<asp:HyperLink ID="lnkImpPlanName" runat="server" Target="_blank" Visible="True"></asp:HyperLink>
						<asp:Label ID="lblDesc" runat="server"></asp:Label>
					</div>
					<asp:Panel ID="graphicLine2Edit" runat="server">
					    <asp:HyperLink ID="hlkEdit" runat="server" Target="_blank" ImageUrl="~/Images/Edit.png" ToolTip="Edit" />
                        </asp:Panel>
				</ItemTemplate>
			</telerik:RadListBox>
		</div>
    <div runat="server" id="BtnAdd" class="searchTileBtnDiv_smallTile" title="Add New" style="margin-top: 1px;">
		  <span runat="server" id="BtnAddSpan" class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
          <div style="padding: 0;" runat="server" id="BtnAddDiv">Add New</div>

</div>
    <span style="display:none;" ClientidMode="Static" id="refreshTile"><asp:Button runat="server" ClientIDMode="Static" ID="btnrefreshTile" OnClick="btnrefreshTile_Click" /></span>
</telerik:RadAjaxPanel>

  

<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="ImprovementPlanTileLoadingPanel" runat="server" />

<script type="text/javascript">
    function inProgressTileRefresh() {
        var btnSpan = this.document.getElementById('refreshTile');
        var btnTrigger = btnSpan && typeof (btnSpan) != 'undefined' && btnSpan.childNodes.length > 0 ? btnSpan.childNodes[0] : null;
        if (btnTrigger)
            btnTrigger.click();
    }
</script>
 