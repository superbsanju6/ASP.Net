<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemStatistics.ascx.cs"
    Inherits="Thinkgate.Controls.Items.ItemStatistics" %>

<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />

<script src="http://code.jquery.com/jquery-latest.js"></script>

<script>
    $(".btnOrig").click(function () {
        $(this).text("Show Original");
    });
    $(".btnCalc").click(function () {
        $(this).text("Show Calculated");
    });
</script>

<telerik:RadMultiPage runat="server" ID="RadMultiPageStatistics" SelectedIndex="0"
    Height="210px" Width="310px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="RadPageOriginalStatistics">
        <telerik:RadListBox runat="server" ID="lbxOriginalStatistics" Width="100%" Height="210px" OnItemDataBound="lbxList_ItemDataBound" BackColor="White">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblDesc" Width="170px" Style="text-align: left;" BackColor="White"></asp:Label>
                <asp:Label runat="server" ID="lblDistractor" Width="50px" Style="text-align: right;" BackColor="White"></asp:Label>
                <asp:Label runat="server" ID="lblValue" Width="50px" Style="text-align: right;" BackColor="White"></asp:Label>
            </ItemTemplate>
        </telerik:RadListBox>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="RadPageCalculatedStatistics">
        <telerik:RadListBox runat="server" ID="lbxCalculatedStatistics" Width="100%" Height="210px" OnItemDataBound="lbxList_ItemDataBound" BackColor="White">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblDesc" Width="170px" Style="text-align: left;" BackColor="White"></asp:Label>
                <asp:Label runat="server" ID="lblDistractor" Width="50px" Style="text-align: right;" BackColor="White"></asp:Label>
                <asp:Label runat="server" ID="lblValue" Width="50px" Style="text-align: right;" BackColor="White"></asp:Label>
            </ItemTemplate>
        </telerik:RadListBox>
    </telerik:RadPageView>
</telerik:RadMultiPage>

<div style="float: left;">
    <telerik:RadTabStrip runat="server" ID="RadTabStrip2" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageStatistics" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left">
        <Tabs>
            <telerik:RadTab Text="Original">
            </telerik:RadTab>
            <telerik:RadTab Text="Calculated">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
</div>
