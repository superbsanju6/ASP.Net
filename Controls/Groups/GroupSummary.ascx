<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupSummary.ascx.cs" Inherits="Thinkgate.Controls.Groups.GroupSummary" %>
<telerik:RadAjaxPanel runat="server" ID="classSummaryPanel" LoadingPanelID="classSummaryLoadingPanel">
    <div style="width: 97%; height: 203px; overflow: auto; padding: 3px;">
        <div class="listView" style="display: none;">
            <telerik:RadGrid runat="server" ID="rosterGrid" AutoGenerateColumns="false" Style="background-image: url(../../Images/transparent_bkgd.png);"
                Width="100%" AllowFilteringByColumn="false" OnItemDataBound="SetRTIImage">
                <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false" ShowHeadersWhenNoRecords="false">
                    <Columns>
                        <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile" ItemStyle-BorderWidth="0">
                            <ItemTemplate>
                                <a href="Student.aspx?childPage=yes&xID=<%# Eval("ID") %>" onclick="return false;">
                                    <asp:Image runat="server" ID="listViewSummaryIcon" ImageUrl="~/Images/summary.png" AlternateText="Summary" CssClass="summaryImgButton" /></a>
                                <asp:HyperLink runat="server" ID="studentNameLinkListView" Target="_blank" />
                                <asp:Label runat="server" ID="studentNameLabelListView" Style="color: #3B3B3B; text-decoration: none;" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridImageColumn UniqueName="RTIImage" ImageUrl="~/Images/blank.png" ImageAlign="Bottom" ItemStyle-BorderWidth="0" ImageWidth="15" AlternateText="RTI Tier">
                        </telerik:GridImageColumn>
                        <telerik:GridImageColumn UniqueName="AccomodationsImage" ImageUrl="~/Images/blank.png" ImageAlign="Bottom" ItemStyle-BorderWidth="0" ImageWidth="15">
                        </telerik:GridImageColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>

        <div class="graphicalView">
            <telerik:RadGrid runat="server" ID="rosterGrid_GraphicView" AutoGenerateColumns="false" Style="background-image: url(../../Images/transparent_bkgd.png);"
                Width="100%" AllowFilteringByColumn="false" OnItemDataBound="SetRTIImage_GraphicView">
                <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false" ShowHeadersWhenNoRecords="false">
                    <Columns>
                        <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile" ItemStyle-BorderWidth="0">
                            <ItemTemplate>
                                <asp:HyperLink runat="server" ID="StudentPhoto" ImageUrl="~/Images/new/male_student.png" ToolTip="Student" CssClass="smallTile_PhotoButton" Target="_blank" />
                                <div class="gridToggleView_Graphical_InfoDiv">
                                    <asp:HyperLink runat="server" ID="studentNameLinkGraphicView" Target="_blank" />
                                    <asp:Label runat="server" ID="studentNameLabelGraphicView" />
                                    <br />
                                    <a href="Student.aspx?childPage=yes&xID=<%# Eval("ID") %>" onclick="return false;">
                                        <asp:Image runat="server" ID="graphicalViewSummaryIcon" ImageUrl="~/Images/summary.png" AlternateText="Summary" CssClass="summaryImgButton" /></a>
                                    <asp:Image ID="GraphicViewRTIImage" runat="server" ImageUrl="~/Images/blank.png" AlternateText="RTI Tier" CssClass="inlineImage" Width="15" />
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/blank.png" AlternateText="Accomodations" CssClass="inlineImage" Width="15" />
                                </div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
    </div>  
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="classSummaryLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>
