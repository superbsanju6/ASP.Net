<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentObjectsResults.ascx.cs"
	Inherits="Thinkgate.Controls.Assessment.AssessmentObjectsResults" %>
<style type="text/css">
	.rgrid
	{
		font-size: 8pt;
	}


</style>

<script type="text/javascript">
	function pageLoad() {
		fixGridHeaders();
	}

	function fixGridHeaders()	{
		$('.rgHeaderDiv').each(function (index, hdrElement) {
			var theGrid = $(hdrElement).parent();
			var masterTbl = theGrid.children('.rgDataDiv').first().children('.rgMasterTable').first();
			var masterTblWd = masterTbl.width();
			if (masterTblWd != null && masterTblWd > 0)
				hdrElement.style.width = masterTblWd.toString() + 'px';
		});
	}

</script>


<telerik:RadAjaxPanel ID="rapAssessmentObjectsResults" runat="server" LoadingPanelID="assessmentObjectsResultsLoadingPanel">
	<!-- Pages -->
	<telerik:RadMultiPage runat="server" ID="rmpObjectsResults" SelectedIndex="0" Height="208px"
		Width="100%" CssClass="multiPage" Style="overflow-y: auto ! important; overflow-x: hidden ! important;
		border-bottom: 1px solid Black;">
		<!-- First page -->
		<telerik:RadPageView runat="server" ID="rpv0">
			<telerik:RadGrid runat="server" ID="rg0" AutoGenerateColumns="False" Width="100%"
				AllowFilteringByColumn="False" AllowPaging="False" AllowSorting="True"
				AllowMultiRowSelection="false"
				OnItemDataBound="rg_ItemDataBound" OnSortCommand="rg_OnSortCommand" Height="206px"
				CssClass="rgrid" Skin="Web20">
				<SortingSettings EnableSkinSortStyles="false" />
				<ClientSettings EnableRowHoverStyle="true">
					<Selecting AllowRowSelect="True" />
					<Scrolling AllowScroll="True" SaveScrollPosition="True" 
						UseStaticHeaders="True" />
					<Resizing AllowColumnResize="True" />
				</ClientSettings>
				<MasterTableView TableLayout="Fixed" Font-Size="8pt">
					<CommandItemSettings ExportToPdfText="Export to PDF" />
					<RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
					</RowIndicatorColumn>
					<ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
					</ExpandCollapseColumn>
					<Columns>
						<telerik:GridHyperLinkColumn UniqueName="LnkLevelName" DataTextField="LevelName" NavigateUrl="" Target="_blank"
							HeaderText="Level" SortExpression="LevelName" HeaderStyle-HorizontalAlign="Center"/>
						<telerik:GridBoundColumn DataField="StudentCount" HeaderText="Students"
							ShowSortIcon="true" HeaderStyle-Width="22%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
						<telerik:GridBoundColumn DataField="Score" HeaderText="Score"
							ShowSortIcon="true" DataFormatString="{0:F0}%" HeaderStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
						<telerik:GridBoundColumn DataField="%Proficient" HeaderText="Proficient"
							ShowSortIcon="true" DataFormatString="{0:F0}%" HeaderStyle-Width="22%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
					</Columns>
					<EditFormSettings>
						<EditColumn FilterControlAltText="Filter EditCommandColumn column">
						</EditColumn>
					</EditFormSettings>
					<HeaderStyle Font-Size="8pt" />
				</MasterTableView>
				<HeaderStyle Font-Size="8pt" />
				<ItemStyle Font-Size="8pt" />
				<FilterMenu EnableImageSprites="False">
				</FilterMenu>
			</telerik:RadGrid>
		</telerik:RadPageView>
		<!-- Second page -->
		<telerik:RadPageView runat="server" ID="rpv1">
			<telerik:RadGrid runat="server" ID="rg1" AutoGenerateColumns="False" Width="100%"
				AllowFilteringByColumn="False" AllowPaging="False" AllowSorting="True"
				AllowMultiRowSelection="false"
				OnItemDataBound="rg_ItemDataBound" OnSortCommand="rg_OnSortCommand" Height="206px"
				CssClass="rgrid" Skin="Web20">
				<SortingSettings EnableSkinSortStyles="false" />
				<ClientSettings EnableRowHoverStyle="true">
					<Selecting AllowRowSelect="True" />
					<Scrolling AllowScroll="True" SaveScrollPosition="True" 
						UseStaticHeaders="True" />
					<Resizing AllowColumnResize="True" />
				</ClientSettings>
				<MasterTableView TableLayout="Fixed" Font-Size="8pt">
					<CommandItemSettings ExportToPdfText="Export to PDF" />
					<RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
					</RowIndicatorColumn>
					<ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
					</ExpandCollapseColumn>
					<Columns>
						<telerik:GridHyperLinkColumn UniqueName="LnkLevelName" DataTextField="LevelName" NavigateUrl="" Target="_blank"
							HeaderText="Level" SortExpression="LevelName" HeaderStyle-HorizontalAlign="Center"/>
						<telerik:GridBoundColumn DataField="StudentCount" HeaderText="Students"
							ShowSortIcon="true" HeaderStyle-Width="22%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
						<telerik:GridBoundColumn DataField="Score" HeaderText="Score"
							ShowSortIcon="true" DataFormatString="{0:F0}%" HeaderStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
						<telerik:GridBoundColumn DataField="%Proficient" HeaderText="Proficient"
							ShowSortIcon="true" DataFormatString="{0:F0}%" HeaderStyle-Width="22%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
					</Columns>
					<EditFormSettings>
						<EditColumn FilterControlAltText="Filter EditCommandColumn column">
						</EditColumn>
					</EditFormSettings>
					<HeaderStyle Font-Size="8pt" />
				</MasterTableView>
				<HeaderStyle Font-Size="8pt" />
				<ItemStyle Font-Size="8pt" />
				<FilterMenu EnableImageSprites="False">
				</FilterMenu>
			</telerik:RadGrid>
		</telerik:RadPageView>
		<!-- Third page -->
		<telerik:RadPageView runat="server" ID="rpv2">
			<telerik:RadGrid runat="server" ID="rg2" AutoGenerateColumns="False" Width="100%"
				AllowFilteringByColumn="False" AllowPaging="False" AllowSorting="True"
				AllowMultiRowSelection="false"
				OnItemDataBound="rg_ItemDataBound" OnSortCommand="rg_OnSortCommand" Height="206px"
				CssClass="rgrid" Skin="Web20">
				<SortingSettings EnableSkinSortStyles="false" />
				<ClientSettings EnableRowHoverStyle="true">
					<Selecting AllowRowSelect="True" />
					<Scrolling AllowScroll="True" SaveScrollPosition="True" 
						UseStaticHeaders="True" />
					<Resizing AllowColumnResize="True" />
				</ClientSettings>
				<MasterTableView TableLayout="Fixed" Font-Size="8pt">
					<CommandItemSettings ExportToPdfText="Export to PDF" />
					<RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
					</RowIndicatorColumn>
					<ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
					</ExpandCollapseColumn>
					<Columns>
						<telerik:GridHyperLinkColumn UniqueName="LnkLevelName" DataTextField="LevelName" NavigateUrl="" Target="_blank"
							HeaderText="Level" SortExpression="LevelName" HeaderStyle-HorizontalAlign="Center"/>
						<telerik:GridBoundColumn DataField="StudentCount" HeaderText="Students"
							ShowSortIcon="true" HeaderStyle-Width="22%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
						<telerik:GridBoundColumn DataField="Score" HeaderText="Score"
							ShowSortIcon="true" DataFormatString="{0:F0}%" HeaderStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
						<telerik:GridBoundColumn DataField="%Proficient" HeaderText="Proficient"
							ShowSortIcon="true" DataFormatString="{0:F0}%" HeaderStyle-Width="22%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
					</Columns>
					<EditFormSettings>
						<EditColumn FilterControlAltText="Filter EditCommandColumn column">
						</EditColumn>
					</EditFormSettings>
					<HeaderStyle Font-Size="8pt" />
				</MasterTableView>
				<HeaderStyle Font-Size="8pt" />
				<ItemStyle Font-Size="8pt" />
				<FilterMenu EnableImageSprites="False">
				</FilterMenu>
			</telerik:RadGrid>
		</telerik:RadPageView>
		<!-- Fourth page -->
		<telerik:RadPageView runat="server" ID="rpv3">
			<telerik:RadGrid runat="server" ID="rg3" AutoGenerateColumns="False" Width="100%"
				AllowFilteringByColumn="False" AllowPaging="False" AllowSorting="True"
				AllowMultiRowSelection="false"
				OnItemDataBound="rg_ItemDataBound" OnSortCommand="rg_OnSortCommand" Height="206px"
				CssClass="rgrid" Skin="Web20">
				<SortingSettings EnableSkinSortStyles="false" />
				<ClientSettings EnableRowHoverStyle="true">
					<Selecting AllowRowSelect="True" />
					<Scrolling AllowScroll="True" SaveScrollPosition="True" 
						UseStaticHeaders="True" />
					<Resizing AllowColumnResize="True" />
				</ClientSettings>
				<MasterTableView TableLayout="Fixed" Font-Size="8pt">
					<CommandItemSettings ExportToPdfText="Export to PDF" />
					<RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
					</RowIndicatorColumn>
					<ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
					</ExpandCollapseColumn>
					<Columns>
						<telerik:GridHyperLinkColumn UniqueName="LnkLevelName" DataTextField="LevelName" NavigateUrl="" Target="_blank"
							HeaderText="Level" SortExpression="LevelName" HeaderStyle-HorizontalAlign="Center"/>
						<telerik:GridBoundColumn DataField="StudentCount" HeaderText="Students"
							ShowSortIcon="true" HeaderStyle-Width="22%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
						<telerik:GridBoundColumn DataField="Score" HeaderText="Score"
							ShowSortIcon="true" DataFormatString="{0:F0}%" HeaderStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
						<telerik:GridBoundColumn DataField="%Proficient" HeaderText="Proficient"
							ShowSortIcon="true" DataFormatString="{0:F0}%" HeaderStyle-Width="22%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
						</telerik:GridBoundColumn>
					</Columns>
					<EditFormSettings>
						<EditColumn FilterControlAltText="Filter EditCommandColumn column">
						</EditColumn>
					</EditFormSettings>
					<HeaderStyle Font-Size="8pt" />
				</MasterTableView>
				<HeaderStyle Font-Size="8pt" />
				<ItemStyle Font-Size="8pt" />
				<FilterMenu EnableImageSprites="False">
				</FilterMenu>
			</telerik:RadGrid>
		</telerik:RadPageView>
	</telerik:RadMultiPage>
	<div>
		<telerik:RadTabStrip runat="server" ID="rtsAssessmentObjectsResults" Orientation="HorizontalBottom"
			ClientIDMode="Static" SelectedIndex="0" Skin="Thinkgate_Blue" MultiPageID="rmpObjectsResults" style="margin-left: 3px;"
			EnableEmbeddedSkins="False">
			<Tabs>
				<telerik:RadTab Text="0" Selected="True">
				</telerik:RadTab>
				<telerik:RadTab Text="1">
				</telerik:RadTab>
				<telerik:RadTab Text="2">
				</telerik:RadTab>
				<telerik:RadTab Text="3">
				</telerik:RadTab>
			</Tabs>
		</telerik:RadTabStrip>
	</div>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="assessmentObjectsResultsLoadingPanel"
	runat="server" />

