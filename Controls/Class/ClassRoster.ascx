<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassRoster.ascx.cs"
    Inherits="Thinkgate.Controls.Class.ClassRoster" %>
<div style="width: 300px; height: 230px; overflow: auto; padding: 3px;">
                <div class="listView" style="display:none;">
                    <telerik:RadGrid runat="server" ID="rosterGrid" AutoGenerateColumns="false" 
                        Width="100%" AllowFilteringByColumn="false" OnItemDataBound="SetRTIImage">
                        <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false" ShowHeadersWhenNoRecords="false">
                            <Columns>
                                <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile" ItemStyle-BorderWidth="0">
                                    <ItemTemplate>
                                        <a href="Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="return false;"><asp:Image ID="SummaryIcon" runat="server" ImageUrl="~/Images/summary.png" AlternateText="Summary" CssClass="summaryImgButton" /></a>
                                        <a href="Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;"><%# Eval("Student_Name") %></a>
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
                    <telerik:RadGrid runat="server" ID="rosterGrid_GraphicView" AutoGenerateColumns="false"
                    Width="100%" AllowFilteringByColumn="false" OnItemDataBound="SetRTIImage_GraphicView">
                        <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false" ShowHeadersWhenNoRecords="false">
                            <Columns>
                                <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile" ItemStyle-BorderWidth="0">
                                    <ItemTemplate>
                                        <a href="Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;"><asp:Image runat="server" ID="StudentPhoto" ImageUrl="~/Images/new/male_student.png" AlternateText="Student" CssClass="smallTile_PhotoButton" Height="50" /></a>
                                        <div class="gridToggleView_Graphical_InfoDiv">
                                            <a href="Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;"><%# Eval("Student_Name") %></a>
                                            <br />
                                            <a href="Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="return false;"><asp:Image ID="GraphicViewSummaryIcon" runat="server" ImageUrl="~/Images/summary.png" AlternateText="Summary" CssClass="summaryImgButton" /></a>
                                            <asp:Image ID="GraphicViewRTIImage" runat="server" ImageUrl="~/Images/blank.png" AlternateText="RTI Tier" CssClass="inlineImage" Width="15" />
                                            <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/blank.png" AlternateText="Accomodations" CssClass="inlineImage" Width="15" />
                                        </div>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>