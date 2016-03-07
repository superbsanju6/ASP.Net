<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassSummary.ascx.cs"
    Inherits="Thinkgate.Controls.Class.ClassSummary" %>
<telerik:RadAjaxPanel runat="server" ID="classSummaryPanel" LoadingPanelID="classSummaryLoadingPanel">
    <telerik:RadMultiPage runat="server" ID="RadMultiPageClassSummary" SelectedIndex="0"
        Height="210px" Width="300px" CssClass="multiPage">
        <telerik:RadPageView runat="server" ID="radPageViewRoster">
            <div style="width: 100%; height: 203px; overflow: auto; padding: 3px;">
                <div class="listView" style="display:none;">
                    <telerik:RadGrid runat="server" ID="rosterGrid" AutoGenerateColumns="false" Style="background-image: url(../../Images/transparent_bkgd.png);"
                        Width="100%" AllowFilteringByColumn="false" OnItemDataBound="SetRTIImage">
                        <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false" ShowHeadersWhenNoRecords="false">
                            <Columns>
                                <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile" ItemStyle-BorderWidth="0">
                                    <ItemTemplate>
                                        <a href="Student.aspx?childPage=yes&xID=<%# Eval("IDEncrypted") %>" onclick="return false;"><asp:Image runat="server" ID="listViewSummaryIcon" ImageUrl="~/Images/summary.png" AlternateText="Summary" CssClass="summaryImgButton" /></a>
                                        <asp:HyperLink runat="server" ID="studentNameLinkListView" Target="_blank" />
                                        <asp:Label runat="server" ID="studentNameLabelListView" style="color:#3B3B3B; text-decoration:none;" />
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
                    Width="100%" AllowFilteringByColumn="false" OnItemDataBound="SetRTIImage_GraphicView" Style="background-image: url(../../Images/transparent_bkgd.png);">
                        <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false" ShowHeadersWhenNoRecords="false">
                            <Columns>
                                <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile" ItemStyle-BorderWidth="0">
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server" ID="StudentPhoto" ImageUrl="~/Images/new/male_student.png" ToolTip="Student" CssClass="smallTile_PhotoButton" Target="_blank" />
                                        <div class="gridToggleView_Graphical_InfoDiv">
                                            <asp:HyperLink runat="server" ID="studentNameLinkGraphicView" Target="_blank" />
                                            <asp:Label runat="server" ID="studentNameLabelGraphicView" />
                                            <br />
                                            <a href="Student.aspx?childPage=yes&xID=<%# Eval("IDEncrypted") %>" onclick="return false;"><asp:Image runat="server" ID="graphicalViewSummaryIcon" ImageUrl="~/Images/summary.png" AlternateText="Summary" CssClass="summaryImgButton" /></a>
                                            <asp:Image ID="GraphicViewRTIImage" runat="server" ImageUrl="~/Images/blank.png" AlternateText="RTI Tier" CssClass="inlineImage" Width="15" />
                                            <asp:Image runat="server" ImageUrl="~/Images/blank.png" AlternateText="Accomodations" CssClass="inlineImage" Width="15" />
                                        </div>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="radPageViewTeachers">
            <div style="width: 100%; height: 203px; overflow: auto; padding: 3px;">
                <div class="listView" style="display:none;">
                    <telerik:RadGrid runat="server" ID="teachersGrid" AutoGenerateColumns="false" Style="background-image: url(../../Images/transparent_bkgd.png);"
                        Width="100%" AllowFilteringByColumn="false" OnItemDataBound="SetTeacherIcons">
                        <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false" ShowHeadersWhenNoRecords="false">
                            <Columns>
                                <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile" ItemStyle-BorderWidth="0">
                                    <ItemTemplate>
                                        <a href="Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="return false;"><asp:Image runat="server" ID="listViewSummaryIconTeacher" ImageUrl="~/Images/summary.png" AlternateText="Summary" CssClass="summaryImgButton" /></a>
                                        <a href="Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;"><%# Eval("FirstName") %> <%# Eval("LastName") %></a>
                                        <asp:Image ID="PrimaryTeacherIcon" runat="server" ImageUrl="~/Images/blank.png" CssClass="inlineImage" Width="15" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridImageColumn UniqueName="DegreeIcon" ImageUrl="~/Images/blank.png" ImageAlign="Bottom" ItemStyle-BorderWidth="0" ImageWidth="15" ItemStyle-Width="30">
                                </telerik:GridImageColumn>
                                <telerik:GridImageColumn UniqueName="CertificationIcon" ImageUrl="~/Images/blank.png" ImageAlign="Bottom" ItemStyle-BorderWidth="0" ImageWidth="15" ItemStyle-Width="30">
                                </telerik:GridImageColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>

                <div class="graphicalView">
                    <telerik:RadGrid runat="server" ID="teachersGrid_GraphicalView" AutoGenerateColumns="false" Style="background-image: url(../../Images/transparent_bkgd.png);"
                    Width="100%" AllowFilteringByColumn="false" OnItemDataBound="SetTeacherIcons_GraphicView">
                        <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false" ShowHeadersWhenNoRecords="false">
                            <Columns>
                                <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile" ItemStyle-BorderWidth="0">
                                    <ItemTemplate>
                                        <a href="Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;"><asp:Image runat="server" ImageUrl="~/Images/new/male_teacher.png" AlternateText="Teacher" CssClass="smallTile_PhotoButton" /></a>
                                        <div class="gridToggleView_Graphical_InfoDiv">
                                            <a href="Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;"><%# Eval("FirstName") %> <%# Eval("LastName") %></a>
                                            <asp:Image ID="GraphicViewPrimaryTeacherIcon" runat="server" ImageUrl="~/Images/blank.png" CssClass="inlineImage" Width="15" />
                                            <br />
                                            <a href="Teacher.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="return false;"><asp:Image runat="server" ID="graphicalViewSummaryIconTeacher" ImageUrl="~/Images/summary.png" AlternateText="Summary" CssClass="summaryImgButton" /></a>
                                            <asp:Image ID="GraphicViewDegreeIcon" runat="server" ImageUrl="~/Images/blank.png" CssClass="inlineImage" Width="15" />
                                            <asp:Image ID="GraphicViewCertificationIcon" runat="server" ImageUrl="~/Images/blank.png" CssClass="inlineImage" Width="15" />
                                        </div>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="RadPageView1">
            <div style="height:205px; width:308px; overflow-y:auto;">
                <table class="fieldValueTableSmall RadGrid_Transparent" style="margin: 2px; background-image: url(../../Images/transparent_bkgd.png);">
                    <tr>
                        <td class="fieldLabel" style="width: 120px;">
                            School:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblSchoolName" />
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Primary Teacher:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblPrimaryTeacher" />
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Grade:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblGrade" />
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Subject:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblSubject" />
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Course:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblCourse" />
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Section:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblSection" />
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Period:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblPeriod" />
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Semester:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblSemester" />
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Year:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblYear" />
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Block:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblBlock" />
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel">
                            Retain on Resync:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblRetainOnResync" />
                        </td>
                    </tr>
                </table>
            </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    <telerik:RadTabStrip runat="server" ID="classSummaryRadTabStrip" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageClassSummary" Skin="Thinkgate_Blue" EnableEmbeddedSkins="false" Align="Left" OnClientTabSelected="toggleView_RadTab_SmallTile">
        <Tabs>
        <telerik:RadTab runat="server" ID="rosterRadTab" Text="Roster">
        </telerik:RadTab>
        <telerik:RadTab runat="server" ID="teachersRadTab" Text="Teachers">
        </telerik:RadTab>
        <telerik:RadTab runat="server" ID="identificationRadTab" Text="Identification">
        </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadToolTipManager runat="server" ID="teacherToolTipManager" Position="MiddleRight"
        RelativeTo="Element" Width="100px" Height="50px" Animation="Resize" HideEvent="LeaveTargetAndToolTip"
        Skin="Default" OnAjaxUpdate="OnAjaxUpdate" EnableShadow="true"
        RenderInPageRoot="true" AnimationDuration="200">
    </telerik:RadToolTipManager>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="classSummaryLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>
