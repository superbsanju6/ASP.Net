<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchoolResults.ascx.cs"
    Inherits="Thinkgate.Controls.School.SchoolResults" %>
<script type="text/javascript">
    function showAddNewSchoolWindow() {
        customDialog({ url: '../Controls/School/AddSchool.aspx', maximize: true, maxwidth: 550, maxheight: 480 });
    }

    function openExpandedSearch() {
        customDialog({ url: '../Controls/School/SchoolSearch_Expanded.aspx', maximize: true, maxwidth: 950, maxheight: 675 });
    }
</script>
<asp:HiddenField runat="server" ID="schoolsearch_hiddenLevel" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="schoolsearch_hiddenLevelID" ClientIDMode="Static" />
<telerik:RadMultiPage runat="server" ID="RadMultiPageSchoolResults" SelectedIndex="0"
    Height="210px" Width="300px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="RadPageViewSchoolResultsChart">
        <telerik:RadChart ID="schoolCountChart" runat="server" Width="310px" Height="210px"
            DefaultType="Pie" AutoLayout="true" AutoTextWrap="False" CreateImageMap="true">

            <Appearance TextQuality="AntiAlias">
            </Appearance>

            <PlotArea>

                <Appearance Dimensions-Margins="18%, 24%, 22%, 10%">
                </Appearance>

                <XAxis LayoutMode="Inside" AutoScale="false">

                    <Appearance>

                        <LabelAppearance RotationAngle="45" Position-AlignedPosition="Top">
                        </LabelAppearance>

                    </Appearance>

                </XAxis>

                <YAxis IsZeroBased="false">
                </YAxis>

            </PlotArea>

        </telerik:RadChart>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="radPageViewSchoolResults">
        <telerik:RadAjaxPanel runat="server" ID="districtSchoolPanel" LoadingPanelID="districtSchoolLoadingPanel"
            Width="96%">
            <div id="searchHolder">
                <div class="searchTextDiv_smallTile">
                    <input type="text" name="text" id="schoolsSearchText_smallTile" clientidmode="Static"
                        class="searchStyle" runat="server" defaulttext="Search by school name..." onkeypress="return searchSmallTile_SubmitOnEnter(this, event);"
                        onclick="if ($(this).val() == 'Search by school name...') { $(this).val(''); }"
                        value="Search by school name..." />
                </div>
                <div class="searchTileButtonDiv_smallTile">
                    <asp:ImageButton ID="schoolsSearchButton_smallTile" ClientIDMode="Static" runat="server"
                        ImageUrl="~/Images/go.png" OnClick="SearchSchoolsByName_Click" OnClientClick="return isSearchText(this);"></asp:ImageButton>
                </div>
                <div class="advancedSearchDiv_smallTile">
                    <asp:LinkButton runat="server" ID="schoolAdvancedSearchLink_smallTile" ClientIDMode="Static"
                        Text="Advanced Search" OnClientClick="setSearchTileNameCookie('schools'); openExpandedSearch(); return false;" />
                </div>
            </div>
            <div style="width: 100%; height: 170px; padding: 3px; overflow: auto;">
                <telerik:RadGrid runat="server" ID="schoolsSearchTileGrid" AutoGenerateColumns="false"
                    Width="100%" AllowFilteringByColumn="false" ClientIDMode="Static">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
                        ShowHeadersWhenNoRecords="true">
                        <Columns>
                            <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile">
                                <ItemTemplate>
                                    <a href="School.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('School.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;">
                                        <%# Eval("SchoolName") %></a>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                <span id="schoolsSearchMoreLinkSpan" class="searchMoreLinkSpan_smallTile">
                    <asp:LinkButton runat="server" ID="schoolSearchMoreLink" ClientIDMode="Static" Visible="false"
                        Text="more results..." OnClick="expandTile_Click" OnClientClick="setSearchTileNameCookie('schools');" /></span>
                <asp:Label runat="server" ID="schoolNoRecordsMsg" Text="No schools found." Visible="false"
                    CssClass="searchTileNoRecordMsg_smallTile" />
            </div>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ID="districtSchoolLoadingPanel" runat="server" />
    </telerik:RadPageView>
</telerik:RadMultiPage>

    <div style="float: left;">
        <telerik:RadTabStrip runat="server" ID="tabs" Orientation="HorizontalBottom"
            SelectedIndex="0" MultiPageID="RadMultiPageSchoolResults" Skin="Thinkgate_Blue"
            EnableEmbeddedSkins="false" Align="Left">
            <Tabs>
                <telerik:RadTab Text="Summary">
                </telerik:RadTab>
                <telerik:RadTab Text="Search">
                </telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
    </div>
    <div id="addNewSchool" runat="server" class="searchTileBtnDiv_smallTile" title="Add New School"
        onclick="showAddNewSchoolWindow()">
        <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
        <div style="padding: 0;">
            Add New
        </div>
    </div>

<span style="display: none;">
    <telerik:RadXmlHttpPanel runat="server" ID="schoolPieChartXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/PieChartWCF.svc"
        WcfServiceMethod="OpenExpandedWindow" RenderMode="Block" OnClientResponseEnding="openPieChartExpandedWindow">
    </telerik:RadXmlHttpPanel>
</span>