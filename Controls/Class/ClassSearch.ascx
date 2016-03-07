<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassSearch.ascx.cs"
    Inherits="Thinkgate.Controls.Class.ClassSearch" %>
<script type="text/javascript">
    function showAddNewClassWindow() {
        customDialog({ url: '../Controls/Class/AddClass.aspx', maximize: true, maxwidth: 550, maxheight: 530 });
    }

    function openClassSearch() {
        customDialog({ url: '../Controls/Class/ClassSearch_Expanded.aspx', maximize: true, maxwidth: 950, maxheight: 675 });
    }
</script>
<telerik:RadMultiPage runat="server" ID="RadMultiPageClassSearch" SelectedIndex="0"
    Height="210px" Width="300px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="RadPageViewClassSearchChart">
        <telerik:RadChart ID="classCountChart" runat="server" Width="310px" Height="210px"
            DefaultType="Pie" AutoLayout="true" AutoTextWrap="False" CreateImageMap="true">
            <Appearance TextQuality="AntiAlias">
            </Appearance>  
        </telerik:RadChart>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="radPageViewClassSearch">
        <div style="width: 100%; height: 200px; overflow: auto; padding: 3px;">
            <div style="margin-top: 25%; text-align: center;">
                <asp:HyperLink runat="server" Font-Underline="true" Font-Size="14" ForeColor="Blue" Style="cursor: pointer;" OnClick="openClassSearch();" Text="Click here to search for classes"></asp:HyperLink>
            </div>
        </div>
    </telerik:RadPageView>
</telerik:RadMultiPage>

      <div style="float: left;">
    <telerik:RadTabStrip runat="server" ID="RadTabStrip2" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageClassSearch" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false">
        <Tabs>
            <telerik:RadTab Text="Summary">
            </telerik:RadTab>
            <telerik:RadTab Text="Search">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
          </div>
    <div id="addNewClass" runat="server" class="searchTileBtnDiv_smallTile" title="Add New Class" onclick="showAddNewClassWindow()">
        <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon">
        </span>
        <div style="padding: 0;">
            Add New</div>
    </div>

<span style="display: none;">
    <telerik:RadXmlHttpPanel runat="server" ID="classPieChartXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/PieChartWCF.svc"
        WcfServiceMethod="OpenExpandedWindow" RenderMode="Block" OnClientResponseEnding="openPieChartExpandedWindow">
    </telerik:RadXmlHttpPanel>
</span>