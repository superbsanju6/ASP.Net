<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="TeacherSearch.ascx.cs"
    Inherits="Thinkgate.Controls.Teacher.TeacherSearch" %>
<script type="text/javascript">
    function showAddNewStaffWindow() {
        //customDialog({ url: '../Controls/Staff/AddStaff.aspx', width: 550, height: 570 });
        customDialog({ url: '../Controls/Teacher/AddStaff.aspx', maximize: true, maxwidth: 550, maxheight: 620 });
        // functionIsInDevelopment();
    }

    function openTeacherSearchExpanded(objLevel, objLevelID) {
        var level = objLevel.value;
        var levelID = objLevelID.value;
        //setSearchTileNameCookie('students');

        var url = '../Controls/Teacher/TeacherSearch_Expanded.aspx?level=' + level + '&levelID=' + levelID;
        customDialog({ url: url, maximize: true, maxwidth: 950, maxheight: 675 });
    }
     

</script>
<asp:HiddenField runat="server" ID="teacherSearch_HiddenLevel" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="teacherSearch_HiddenLevelID" ClientIDMode="Static" />
<telerik:RadMultiPage runat="server" ID="RadMultiPageTeacherSearch" SelectedIndex="0"
    Height="210px" Width="300px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="RadPageViewTeacherSearchChart">
        <telerik:RadChart ID="teacherCountChart" runat="server" Width="310px" Height="210px"
            DefaultType="Pie"  AutoLayout="true" AutoTextWrap="False" CreateImageMap="true">
            <ChartTitle>
                <Appearance Position-AlignedPosition="TopRight">
                </Appearance>
                <TextBlock Text="Teacher Count">
                    <Appearance TextProperties-Color="102, 102, 102"
                        TextProperties-Font="Arial, 8pt">
                    </Appearance>
                </TextBlock>
            </ChartTitle>
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
    <telerik:RadPageView runat="server" ID="radPageViewTeacherSearch">
        <telerik:RadAjaxPanel runat="server" ID="teacherSearchPanel" LoadingPanelID="teacherSearchLoadingPanel"
            Width="96%">
            <div id="searchHolder">
                <div class="searchTextDiv_smallTile">
                    <input type="text" name="text" id="teachersSearchText_smallTile" clientidmode="Static"
                        class="searchStyle" runat="server" defaulttext="Search by last name..." onkeypress="return searchSmallTile_SubmitOnEnter(this, event);"
                        onclick="if( $(this).val() == 'Search by last name...' ) {$(this).val('');}"
                        value="Search by last name..." />
                </div>
                <div class="searchTileButtonDiv_smallTile">
                    <asp:ImageButton ID="teachersSearchButton_smallTile" ClientIDMode="Static" runat="server"
                        ImageUrl="~/Images/go.png" OnClick="SearchTeachersByLastName_Click" OnClientClick="return isSearchText(this);">
                    </asp:ImageButton>
                </div>
                <div class="advancedSearchDiv_smallTile" runat="server" id="TeacherSearch_DivAdvancedSearch" ClientIDMode="Static">
                    <a href="#" onclick="openTeacherSearchExpanded($('#teacherSearch_HiddenLevel')[0], $('#teacherSearch_HiddenLevelID')[0]); return false;">Advanced Search</a>
                </div>
            </div>
            <div style="width: 100%; height: 170px; padding: 3px; overflow: auto;">
                <telerik:RadGrid runat="server" ID="teachersSearchTileGrid" AutoGenerateColumns="false"
                    Width="100%" AllowFilteringByColumn="false" ClientIDMode="Static" OnItemDataBound="teachersSearchTileGrid_ItemDataBound">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
                        ShowHeadersWhenNoRecords="true">
                        <Columns>
                            <telerik:GridTemplateColumn InitializeTemplatesFirst="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="teacherNameLink"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                <span id="teachersSearchMoreLinkSpan" class="searchMoreLinkSpan_smallTile">
                    <a runat="server" id="teacherSearchMoreLink" ClientIDMode="Static" visible="false" href="#" onclick="openTeacherSearchExpanded($('#teacherSearch_HiddenLevel')[0], $('#teacherSearch_HiddenLevelID')[0]); return false;">more results...</a>
                </span>
                <asp:Label runat="server" ID="teacherNoRecordsMsg" Text="No staff found." Visible="false"
                    CssClass="searchTileNoRecordMsg_smallTile" />
            </div>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ID="teacherSearchLoadingPanel" runat="server" />
    </telerik:RadPageView>
</telerik:RadMultiPage>

     <div style="float: left;">
    <telerik:RadTabStrip runat="server" ID="RadTabStrip2" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageTeacherSearch" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left">
        <Tabs>
            <telerik:RadTab Text="Summary">
            </telerik:RadTab>
            <telerik:RadTab Text="Search">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
       </div>
    <div id="addNewStaff" runat="server" class="searchTileBtnDiv_smallTile" title="Add New Staff" onclick="showAddNewStaffWindow()">
        <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon">
        </span>
        <div style="padding: 0;">
            Add New</div>
    </div>

<span style="display: none;">
    <telerik:RadXmlHttpPanel runat="server" ID="teacherPieChartXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/PieChartWCF.svc"
        WcfServiceMethod="OpenExpandedWindow" RenderMode="Block" OnClientResponseEnding="openPieChartExpandedWindow">
    </telerik:RadXmlHttpPanel>
</span>