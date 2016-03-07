<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Teachers.ascx.cs" Inherits="Thinkgate.Controls.Teacher.Teachers" %>
<div style="width: 300px; height: 230px; overflow: auto; padding: 3px;">
    <div class="listView" style="display: none;">
        <telerik:RadGrid runat="server" ID="teachersGrid" AutoGenerateColumns="false" Width="100%"
            AllowFilteringByColumn="false" OnItemDataBound="SetTeacherIcons">
            <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
                ShowHeadersWhenNoRecords="false">
                <Columns>
                    <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile"
                        ItemStyle-BorderWidth="0">
                        <ItemTemplate>
                            <a href="Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="return false;">
                                <asp:Image ID="SummaryIcon" runat="server" ImageUrl="~/Images/summary.png" AlternateText="Summary"
                                    CssClass="summaryImgButton" /></a> <a href="Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>"
                                        onclick="window.open('Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;">
                                        <%# Eval("FirstName") %>
                                        <%# Eval("LastName") %></a>
                            <asp:Image ID="PrimaryTeacherIcon" runat="server" ImageUrl="~/Images/blank.png" CssClass="inlineImage"
                                Width="15" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridImageColumn UniqueName="DegreeIcon" ImageUrl="~/Images/blank.png" ImageAlign="Bottom"
                        ItemStyle-BorderWidth="0" ImageWidth="15" ItemStyle-Width="30">
                    </telerik:GridImageColumn>
                    <telerik:GridImageColumn UniqueName="CertificationIcon" ImageUrl="~/Images/blank.png"
                        ImageAlign="Bottom" ItemStyle-BorderWidth="0" ImageWidth="15" ItemStyle-Width="30">
                    </telerik:GridImageColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
    <div class="graphicalView">
        <telerik:RadGrid runat="server" ID="teachersGrid_GraphicalView" AutoGenerateColumns="false"
            Width="100%" AllowFilteringByColumn="false" OnItemDataBound="SetTeacherIcons_GraphicView">
            <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
                ShowHeadersWhenNoRecords="false">
                <Columns>
                    <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile"
                        ItemStyle-BorderWidth="0">
                        <ItemTemplate>
                            <a href="Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/new/male_teacher.png" AlternateText="Teacher"
                                    CssClass="smallTile_PhotoButton" /></a>
                            <div class="gridToggleView_Graphical_InfoDiv">
                                <a href="Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;">
                                    <%# Eval("FirstName") %>
                                    <%# Eval("LastName") %></a>
                                <asp:Image ID="GraphicViewPrimaryTeacherIcon" runat="server" ImageUrl="~/Images/blank.png"
                                    CssClass="inlineImage" Width="15" />
                                <br />
                                <a href="Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="return false;">
                                    <asp:Image ID="GraphicViewSummaryIcon" runat="server" ImageUrl="~/Images/summary.png" AlternateText="Summary"
                                        CssClass="summaryImgButton" /></a>
                                <asp:Image ID="GraphicViewDegreeIcon" runat="server" ImageUrl="~/Images/blank.png"
                                    CssClass="inlineImage" Width="15" />
                                <asp:Image ID="GraphicViewCertificationIcon" runat="server" ImageUrl="~/Images/blank.png"
                                    CssClass="inlineImage" Width="15" />
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
</div>
