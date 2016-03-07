<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="StaffSearch.ascx.cs"
    Inherits="Thinkgate.Controls.LCO.StaffSearch" %>
<script type="text/javascript">
    function showAddNewStaffWindow() {
        customDialog({ url: '../Controls/LCO/AddLCOStaff.aspx', maximize: true, maxwidth: 550, maxheight: 480 });
        // functionIsInDevelopment();
    }

    function openStaffSearchExpanded(objLevel, objLevelID) {
        var level = objLevel.value;
        var levelID = objLevelID.value;
        //setSearchTileNameCookie('students');

        var url = '../Controls/LCO/StaffSearch_Expanded.aspx?level=' + level + '&levelID=' + levelID;
        customDialog({ url: url, maximize: true, maxwidth: 950, maxheight: 675 });
    }
    

</script>
<asp:HiddenField runat="server" ID="staffSearch_HiddenLevel" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="staffSearch_HiddenLevelID" ClientIDMode="Static" />
<telerik:RadMultiPage runat="server" ID="RadMultiPageStaffSearch" SelectedIndex="0"
    Height="210px" Width="300px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="RadPageViewStaffSearchChart">
        <telerik:RadChart ID="staffCountChart" runat="server" Width="310px" Height="210px"
            DefaultType="Pie" AutoLayout="true" AutoTextWrap="False" CreateImageMap="true">
        </telerik:RadChart>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="radPageViewStaffSearch">
        <telerik:RadAjaxPanel runat="server" ID="staffSearchPanel" LoadingPanelID="staffSearchLoadingPanel"
            Width="96%">
            <div id="searchHolder">
                <div class="searchTextDiv_smallTile">
                    <input type="text" name="text" id="staffSearchText_smallTile" clientidmode="Static"
                        class="searchStyle" runat="server" defaulttext="Search by last name..." onkeypress="return searchSmallTile_SubmitOnEnter(this, event);"
                        onclick="if( $(this).val() == 'Search by last name...' ) {$(this).val('');}"
                        value="Search by last name..." />
                </div>
                <div class="searchTileButtonDiv_smallTile">
                    <asp:ImageButton ID="staffSearchButton_smallTile" ClientIDMode="Static" runat="server"
                        ImageUrl="~/Images/go.png" OnClick="SearchStaffByLastName_Click" OnClientClick="return isSearchText(this);">
                    </asp:ImageButton>
                </div>
                <div class="advancedSearchDiv_smallTile" runat="server" id="StaffSearch_DivAdvancedSearch" ClientIDMode="Static">
                    <a href="#" onclick="openStaffSearchExpanded($('#staffSearch_HiddenLevel')[0], $('#staffSearch_HiddenLevelID')[0]); return false;">Advanced Search</a>
                </div>
            </div>
            <div style="width: 100%; height: 170px; padding: 3px; overflow: auto;">
                <telerik:RadGrid runat="server" ID="staffSearchTileGrid" AutoGenerateColumns="false"
                    Width="100%" AllowFilteringByColumn="false" ClientIDMode="Static" OnItemDataBound="staffSearchTileGrid_ItemDataBound">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
                        ShowHeadersWhenNoRecords="true">
                        <Columns>
                            <telerik:GridTemplateColumn InitializeTemplatesFirst="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="staffNameLink"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                <span id="staffSearchMoreLinkSpan" class="searchMoreLinkSpan_smallTile">
                    <a runat="server" id="staffSearchMoreLink" ClientIDMode="Static" visible="false" href="#" onclick="openStaffSearchExpanded($('#staffSearch_HiddenLevel')[0], $('#staffSearch_HiddenLevelID')[0]); return false;">more results...</a>
                </span>
                <asp:Label runat="server" ID="staffNoRecordsMsg" Text="No staff found." Visible="false"
                    CssClass="searchTileNoRecordMsg_smallTile" />
            </div>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ID="staffSearchLoadingPanel" runat="server" />
    </telerik:RadPageView>
</telerik:RadMultiPage>
<div class="tabsAndButtonsWrapper_smallTile">
    <telerik:RadTabStrip runat="server" ID="RadTabStrip2" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageStaffSearch" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left">
        <Tabs>
            <telerik:RadTab Text="Summary">
            </telerik:RadTab>
            <telerik:RadTab Text="Search">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <div id="addNewStaff" runat="server" class="searchTileBtnDiv_smallTile" title="Add New Staff" onclick="showAddNewStaffWindow()">
        <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon">
        </span>
        <div style="padding: 0;">
            Add New</div>
    </div>
<span style="display: none;">
    <telerik:RadXmlHttpPanel runat="server" ID="staffPieChartXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/PieChartWCF.svc"
        WcfServiceMethod="OpenExpandedWindow" RenderMode="Block">
    </telerik:RadXmlHttpPanel>
</span>