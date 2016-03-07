<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Items.ascx.cs" Inherits="Thinkgate.Controls.Items.Items" %>
<%@ Register Src="~/Charts/BarChart.ascx" TagName="BarChart" TagPrefix="bc" %>

<style type="text/css">
    /* commenting css classes for removing 'Item/standard Dist' tab */
    /*.class1_text {
        margin-left: -7px;
        margin-top: -2px;  
      }

    .class1 {
        width: 55px !important;
    }     

    .class2 {
        width: 106px !important;
    }

    .class3 {
        width: 60px !important;
    }

    .class2_list {
        margin-left: -1px !important;
    }

    .rtsLevel ul li li {
       width: 106px !important;
        border: 1px solid red !important;
        padding-left: 2px !important;
    }

    .rtsLevel ul li li li {
            width: 60px !important;
            border: 1px solid red !important;
            padding-left: 2px !important;
        }

    .rtsFirst {
        width: 59px !important;
        border-right: 1px solid #000000 !important;
    }

    .rtsLast {
        width: 60px !important;
        margin-left: -6px !important;
      
    }*/

    .rtsSelected {
        background-color: #0F3789;
        padding: 0 1px !important;
    }

    /* Adding css classes to adjust UI after removing 'Item/standard Dist' tab */
    .rtsLevel ul {
        margin-top: -1px !important;
        border: none !important;
        margin-left: -2px !important;
    }

    .rtsSelected .rtsOut {
        margin-left: -2px !important;
        margin-top: -1px !important;
    }
    /* Adding css classes ends here*/

    /* commenting css classes for removing 'Item/standard Dist' tab */
    /*.rtsLevel ul li:nth-child(2) {
        position: relative !important;
        width: 114px !important;
    }

    .rtsLevel ul li:nth-child(3) {
        position: relative !important;
        width: 64px !important;
    }*/

    .pageView {
        border-bottom: 1px solid rgba(0,0,0,0.5);
    }

    .addnewClass {
        background-image: url(../Images/add.gif);
        margin-top: -20px !important;
        margin-left: 5px !important;
        padding-right: 5px !important;
    }

    .CountasofDiv {
        float: right;
        padding-right: 20px;
        padding-top: 5px;
    }

    .Countlbl {
        padding: 0 0 0 5px;
    }

    .combowrapperDiv {
        z-index: 1;
        height: 24px;
        margin-left: 4px;
        margin-top: 1px;
    }

    .multipageclass {
        overflow-y: auto !important;
        overflow-x: hidden !important;
        border: 1px solid #808080;
        clear: both !important;
    }

    .radtabmarginclass {
        margin-top: 0px;
    }

    .rcbEmptyMessage {
        font-style: normal !important;
    }

    .RadGrid .rgHeader {
        background-color: rgb(127,165,215)!important;
    }

    .cellCommon {
        padding-top: 2px;
        padding-bottom: 2px;
        padding-left: 4px;
        padding-right: 4px;
        text-align: center;
    }

    .cellHeader {
        background-color: Gray;
        color: White;
    }

    .cellContent {
        background-color: White;
    }

    .cellContentLeft {
        background-color: White;
        text-align: left;
    }

    .leftCell {
        width: 48px;
    }

    .rightCell {
        width: 181px;
    }

    .tooltip {
        background-color: white;
        position: absolute;
        z-index: 2;
    }
</style>

<script type="text/javascript">

    $(document).ready(function () {
        document.getElementById('ComboOuterDiv').style.display = 'none';
    });

    function addNewItem(itemType) {
        var dTitle = "Identification";
        var dHeight = 405;
        var dWidth = 480;

        customDialog({ url: ('../Dialogues/AddNewItem.aspx?xID=' + itemType), title: dTitle, height: dHeight, width: dWidth, autoSize: true, name: 'NewItem' });
    }

    function OnClientTabSelected(sender, eventArgs) {
        var tab = eventArgs.get_tab();
        //if (tab.get_text() == "Item/Standard Dist." || tab.get_text() == "Rigor Dist.") {
        if (tab.get_text() == "Rigor Dist.") {
            document.getElementById('ComboOuterDiv').style.display = '';
        }
        else { document.getElementById('ComboOuterDiv').style.display = 'none'; }
    }

    var showTooltip = function () {
        $('div.tooltip').remove();
        var tableData = document.getElementById("tootTipTable").innerHTML;
        $('<div class="tooltip">' + tableData + '</div>')
          .appendTo('body');
        changeTooltipPosition();
    };

    var changeTooltipPosition = function () {
        var position = $("#rigorDistributionView").offset();
        var tooltipX = parseInt(position.left) + 30;
        var tooltipY = parseInt(position.top) + 60;
        $('div.tooltip').css({ top: tooltipY, left: tooltipX });
    };

    var hideTooltip = function () {
        $('div.tooltip').remove();
    };

    function bindToolTip() {
        $(".cellHover").bind({
            mouseenter: showTooltip,
            mouseleave: hideTooltip
        });
    }

</script>

<div style="display: none;">
    <asp:Button ID="exportToExcelIcon" runat="server" OnClick="exportToExcelIcon_Click" ClientIDMode="Static" />
</div>
<asp:UpdatePanel ID="updatepanel" runat="server">
    <Triggers>
        <asp:PostBackTrigger ControlID="exportToExcelIcon" />
    </Triggers>
    <ContentTemplate></ContentTemplate>
</asp:UpdatePanel>

<telerik:RadAjaxPanel ID="ItemsAjaxPanel" runat="server" LoadingPanelID="ItemsLoadingPanel">
    <div class="combowrapperDiv">
        <div style="width: 135px; float: left;" id="ComboOuterDiv" runat="server" clientidmode="Static">
            <telerik:RadComboBox runat="server" ID="cmbItembank" ToolTip="Select Item Bank" CausesValidation="false" AutoPostBack="true" OnSelectedIndexChanged="cmbItembank_SelectedIndexChanged"
                EmptyMessage="Item Bank" Skin="Web20" Width="130" HighlightTemplatedItems="true">
                <ItemTemplate>
                    <span><%# Eval("ItemBank") %></span>
                </ItemTemplate>
            </telerik:RadComboBox>
        </div>
        <div class="CountasofDiv">Counts as of:<asp:Label runat="server" ID="lblCountAsof" CssClass="Countlbl"></asp:Label></div>
    </div>

    <telerik:RadMultiPage runat="server" ID="ItemsRadMultiPage" SelectedIndex="0"
        Height="185px" Width="100%" CssClass="multiPage multipageclass">
        <telerik:RadPageView runat="server" ID="radPageViewSummary" Height="184px" Width="100%" CssClass="pageView">
            <asp:PlaceHolder ID="summaryNotFound" runat="server"></asp:PlaceHolder>
            <bc:BarChart ID="barChartLocal" runat="server" ClientIDMode="static" Width="311" Height="92"
                GridLines="7" BarHexColor="#007fff" />
            <bc:BarChart ID="barChartThirdParty" runat="server" ClientIDMode="static" Width="311" Height="92"
                GridLines="7" BarHexColor="#007fff" />
        </telerik:RadPageView>

        <%--   <telerik:RadPageView runat="server" ID="radPageViewStandardDist" Height="184px" Width="100%" CssClass="pageView">
            <label id="standardInitialText" runat="server" style="width: 100%; text-align: center; top: 44%; position: absolute;">Please select an Item Bank</label>
            <asp:Panel ID="standardDistributionView" runat="server" ScrollBars="None" Height="100%" Width="100%" Visible="false" ClientIDMode="Static">
                <telerik:RadGrid ID="radGridStandard" runat="server" AllowPaging="False" AllowSorting="False" AutoGenerateColumns="False"
                    CellSpacing="0" GridLines="None" Width="100%" OnItemDataBound="Items_ItemDataBound">
                    <SortingSettings EnableSkinSortStyles="False" />
                    <MasterTableView>
                        <Columns>
                            <telerik:GridTemplateColumn DataField="StandardName" HeaderText="Standard">
                                <ItemTemplate>
                                    <asp:Label ID="lblStandard" runat="server" Text='<%# Eval("StandardName") %>' ToolTip='<%# Eval("StandardName")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="40%" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="PctItemBank" HeaderText="% of Item Bank">
                                <ItemTemplate>
                                    <asp:Label ID="lblPercent" runat="server" Text='<%# Eval("PctItemBank") + "%" %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="25%" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="ItemCount" HeaderText="Items">
                                <ItemTemplate>
                                    <asp:Label ID="lblItems" runat="server" Text='<%# Eval("StandardItems") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="25%" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </asp:Panel>
        </telerik:RadPageView>--%>

        <telerik:RadPageView runat="server" ID="radPageViewRigorDist" Height="184px" Width="100%" CssClass="pageView">
            <label id="rigorInitialText" runat="server" style="width: 100%; text-align: center; top: 44%; position: absolute;">Please select an Item Bank</label>
            <asp:Panel ID="rigorDistributionView" runat="server" ScrollBars="Auto" Height="100%" Width="100%" Visible="false" ClientIDMode="Static">
                <asp:Literal ID="tableRigor" runat="server"></asp:Literal>
            </asp:Panel>
            <div id="tootTipTable" style="display: none;">
                <div style="width: 233px; border: 1px solid black; padding: 2px;">
                    <div style="width: 227px; border: 1px solid black; padding: 2px;">
                        <table>
                            <tr>
                                <td class="leftCell">MC3</td>
                                <td class="rightCell">= Multiple choice, 3 distractors</td>
                            </tr>
                            <tr>
                                <td class="leftCell">MC4</td>
                                <td class="rightCell">= Multiple choice, 4 distractors</td>
                            </tr>
                            <tr>
                                <td class="leftCell">MC5</td>
                                <td class="rightCell">= Multiple choice, 5 distractors</td>
                            </tr>
                            <tr>
                                <td class="leftCell">S/A</td>
                                <td class="rightCell">= Short Answer</td>
                            </tr>
                            <tr>
                                <td class="leftCell">Essay</td>
                                <td class="rightCell">= Essay</td>
                            </tr>
                            <tr>
                                <td class="leftCell">T/F</td>
                                <td class="rightCell">= True False</td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>

    <telerik:RadTabStrip runat="server" ID="ItemsRadTabStrip" SelectedIndex="0" Orientation="HorizontalBottom"
        MultiPageID="ItemsRadMultiPage" Skin="Thinkgate_Blue" EnableEmbeddedSkins="false" Align="Left"
        OnClientTabSelected="OnClientTabSelected" CssClass="radtabmarginclass">
        <Tabs>
            <telerik:RadTab Text="Summary" CssClass="class1_text">
            </telerik:RadTab>
            <%--<telerik:RadTab Text="Item/Standard Dist." CssClass="class1_text">
            </telerik:RadTab>--%>
            <telerik:RadTab Text="Rigor Dist." CssClass="class1_text">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <div id="AddDiv">
        <div id="btnAdd" runat="server" class="bottomTextButton addnewClass" title="Add New">
            Add 
        </div>
    </div>
</telerik:RadAjaxPanel>

<telerik:RadAjaxLoadingPanel runat="server" ID="ItemsLoadingPanel" ClientIDMode="Static" />
