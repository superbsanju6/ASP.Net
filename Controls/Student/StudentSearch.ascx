<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentSearch.ascx.cs"
    Inherits="Thinkgate.Controls.Student.StudentSearch" %>
<script type="text/javascript">
    function showAddNewStudentWindow() {
        customDialog({ url: '../Controls/Student/AddStudent.aspx', autosize: true, maxheight: 600, maximize_height:true });
    }

    function openStudentSearchExpanded(objLevel, objLevelID) {
        var level = objLevel.value;
        var levelID = objLevelID.value;
        //setSearchTileNameCookie('students');

        var url = '../Controls/Student/StudentSearch_Expanded.aspx?level=' + level + '&levelID=' + levelID;
        customDialog({ url: url, maximize: true, maxwidth: 950, maxheight: 675 });
    }
    
    function enrollStudent() {
        window.open('<%= ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>' + escape('fastsql_v2_direct.asp?ID=EnrollUtility|Testgate_SearchStudents&??x=find'));
    }
</script>
<asp:HiddenField runat="server" ID="studentsearch_hiddenLevel" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="studentsearch_hiddenLevelID" ClientIDMode="Static" />
<telerik:RadMultiPage runat="server" ID="RadMultiPageStudentSummary" SelectedIndex="0"
    Height="210px" Width="300px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="RadPageView1">
        <telerik:RadChart ID="studentCountChart" runat="server" Width="310px" Height="210px"
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
    <telerik:RadPageView runat="server" ID="RadPageView2">
        <telerik:RadAjaxPanel runat="server" ID="schoolStudentsPanel" LoadingPanelID="schoolStudentLoadingPanel"
            Width="96%">
            <div id="searchHolder">
                <div class="searchTextDiv_smallTile">
                    <input type="text" name="text" id="studentsSearchText_smallTile" clientidmode="Static"
                        class="searchStyle" runat="server" defaulttext="Search by last name..." onkeypress="return searchSmallTile_SubmitOnEnter(this, event);"
                        onclick="if( $(this).val() == 'Search by last name...' ) {$(this).val('');}"
                        value="Search by last name..." />
                </div>
                <div class="searchTileButtonDiv_smallTile">
                    <asp:ImageButton ID="studentsSearchButton_smallTile" ClientIDMode="Static" runat="server"
                        ImageUrl="~/Images/go.png" OnClick="SearchStudentsByLastName_Click" OnClientClick="return isSearchText(this);">
                    </asp:ImageButton>
                </div>
                <div class="advancedSearchDiv_smallTile" runat="server" id="StudentSearch_DivAdvancedSearch" ClientIDMode="Static" >
                    <a href="#" onclick="openStudentSearchExpanded(document.getElementById('studentsearch_hiddenLevel'), document.getElementById('studentsearch_hiddenLevelID')); return false;">
                        Advanced Search </a>
                </div>
            </div>
            <div style="width: 100%; height: 170px; padding: 3px; overflow: auto;">
                <telerik:RadGrid runat="server" ID="studentsSearchTileGrid" AutoGenerateColumns="false"
                    Width="100%" AllowFilteringByColumn="false" ClientIDMode="Static" OnItemDataBound="studentsSearchTileGrid_ItemDataBound">
                    <GroupingSettings CaseSensitive="false" />
                    <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
                        ShowHeadersWhenNoRecords="true">
                        <Columns>
                            <telerik:GridTemplateColumn InitializeTemplatesFirst="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="studentNameLink"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                <span id="studentsSearchMoreLinkSpan" class="searchMoreLinkSpan_smallTile"><a runat="server"
                    id="studentSearchMoreLink" visible="false" href="#" onclick="openStudentSearchExpanded(document.getElementById('studentsearch_hiddenLevel'), document.getElementById('studentsearch_hiddenLevelID')); return false;">
                    more results... </a></span>
                <asp:Label runat="server" ID="studentNoRecordsMsg" Text="No students found." Visible="false"
                    CssClass="searchTileNoRecordMsg_smallTile" />
            </div>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ID="schoolStudentLoadingPanel" runat="server" />
    </telerik:RadPageView>
</telerik:RadMultiPage>

    <div style="float: left;">
    <telerik:RadTabStrip runat="server" ID="RadTabStrip2" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageStudentSummary" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left">
        <Tabs>
            <telerik:RadTab Text="Summary">
            </telerik:RadTab>
            <telerik:RadTab Text="Search">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
       </div>
    <div id="addNewStudent" runat="server" class="searchTileBtnDiv_smallTile" title="Add New Student"
        onclick="showAddNewStudentWindow()">
        <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon">
        </span>
        <div style="padding: 0;">
            Add New</div>
    </div>
    <div id="enrollStudent" runat="server" class="searchTileBtnDiv_smallTile" title="Enroll Student" onclick="enrollStudent();">
        <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_enrollIcon">
        </span>
        <div style="padding: 0;">
            Enroll</div>
    </div>
    <%--<div class="searchTileBtnDiv_smallTile" title="Add New Student" style="background-image: url(../Images/add.gif);">
        Add New</div>
    <div class="searchTileBtnDiv_smallTile" title="Enroll Student" style="background-image: url(../Images/clipboard_student.gif);">
        Enroll</div>--%>

<span style="display: none;">
    <telerik:RadXmlHttpPanel runat="server" ID="studentPieChartXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/PieChartWCF.svc"
        WcfServiceMethod="OpenExpandedWindow" RenderMode="Block" OnClientResponseEnding="openPieChartExpandedWindow">
    </telerik:RadXmlHttpPanel>
</span>