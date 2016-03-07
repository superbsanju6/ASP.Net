<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardsSearch.ascx.cs"
    Inherits="Thinkgate.Controls.Standards.StandardsSearch" %>
<telerik:RadAjaxPanel runat="server" ID="standardsSearchPanel" LoadingPanelID="standardsSearchLoadingPanel">
    <style type="text/css">
        .searchResuls_smallTile_Items
        {
            text-align: left;
            vertical-align: top;
        }
        
        .RadComboBox .rcbInputCell .rcbEmptyMessage 
        {
            font-style: normal;
        }
        /*.rgMasterTable td {border:none !important; outline:none !important;
        }
        .RadGrid_Transparent table tr td {border:none !important; outline:none !important;
        }*/
        .RadGrid table tr td {border:none !important; outline:none !important;
        }
    </style>
    <div style="width: 300px; height: 238px; overflow: hidden;">
        <div id="searchHolder" runat="server">
            <div class="buttonAreaFloatLeft">
                <telerik:RadComboBox ID="standardsSetSearchDropdown" Skin="Web20" ClientIDMode="Static"
                    runat="server" EmptyMessage="Standard Set" AutoPostBack="false" OnClientSelectedIndexChanged="requestCourseFilter_StandardsSearchSmallTile"
                    Width="125" xmlHttpPanelID="StandardsCoursesXmlHttpPanel" />
                <telerik:RadComboBox ID="standardsCourseSearchDropdown" Skin="Web20" ClientIDMode="Static"
                    runat="server" EmptyMessage="Standard Course" AutoPostBack="false" OnClientSelectedIndexChanged="standardsSearchTrigger"
                    OnSelectedIndexChanged="SearchStandardsBySetAndCourse_Click" Width="125" HighlightTemplatedItems="true">
                    <ItemTemplate>
                        <span>
                            <%# Eval("CourseValue") %></span>
                    </ItemTemplate>
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="standardsSubjectSearchDropdown" Skin="Web20" ClientIDMode="Static"
                    runat="server" EmptyMessage="Subject" AutoPostBack="false" OnClientSelectedIndexChanged="standardsSearchTrigger"
                    OnSelectedIndexChanged="SearchStandardsBySetAndCourse_Click" Width="125" HighlightTemplatedItems="true">
                    <ItemTemplate>
                        <span>
                            <%# Eval("SubjectValue") %></span>
                    </ItemTemplate>
                </telerik:RadComboBox>
            </div>
        </div>
        <div style="width: 98%; height: 190px; padding: 3px; overflow: auto;" runat="server" id="radGridContainerDiv">
            <telerik:RadGrid runat="server" ID="standardsSearchTileGrid" AutoGenerateColumns="false"
                Width="100%" AllowFilteringByColumn="false" Visible="false" OnItemDataBound="SetStandardNameLinkAccess">
                <GroupingSettings CaseSensitive="false" />
                <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
                    ShowHeadersWhenNoRecords="true">
                    <Columns>
                        <telerik:GridTemplateColumn UniqueName="AddToCalendar" Visible="false">
                            <ItemTemplate>
                                <img id="imgAddToCalendar" runat="server" src="~/Images/commands/calendar_add_small.png"
                                    alt="Add to Calendar" standardid='<%# Eval("ID")%>' title="Add to Calendar" style="cursor: pointer;"
                                    onclick="if(ShowInstructionalPlanCalendar) ShowInstructionalPlanCalendar('standardid', this.getAttribute('standardid'));"/>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="standardNameLink">
                                    <a href="StandardsPage.aspx?xID=<%# Eval("ID_Encrypted") %>" target="_blank"><%# Eval("StandardName") %></a>
                                </asp:Label>
                                <asp:Label runat="server" ID="standardNameLabel"><%# Eval("StandardName") %></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="Desc" AllowFiltering="false">
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>

            <telerik:RadGrid runat="server" ID="standardsCurriculumSearchTileGrid" AutoGenerateColumns="false"
                Width="100%" AllowFilteringByColumn="false" Visible="false" OnItemDataBound="SetStandardSubjectLinkAccess">
                <GroupingSettings CaseSensitive="false" />
                <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
                    ShowHeadersWhenNoRecords="true" BackColor="Gray">
                    <Columns>
                        <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile">
                            <ItemTemplate>
                                <asp:LinkButton ID="standardSubjectLink" runat="server" ToolTip='<%# Eval("FullDesc") %>'><%# Eval("Desc") %></asp:LinkButton>
                                <asp:Label ID="standardSubjectLabel" runat="server"><%# Eval("Desc") %></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>

            <telerik:RadGrid runat="server" ID="standardsAssignedToItemTileGrid" AutoGenerateColumns="false"
                Width="100%" AllowFilteringByColumn="false" Visible="false">
                <GroupingSettings CaseSensitive="false" />
                <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
                    ShowHeadersWhenNoRecords="true">
                    <Columns>
                        <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile">
                            <ItemTemplate>
                                <table style="border: none;">
                                    <tr>
                                        <td style="text-align: left; vertical-align: top;">
                                            <a href="StandardsPage.aspx?xID=<%# Eval("ID_Encrypted") %>" target="_blank">
                                                <%# Eval("StandardName") %></a>
                                        </td>
                                        <td style="text-align: left; vertical-align: top;">
                                            <%# Eval("StandardText") %>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <span id="standardsSearchMoreLinkSpan" class="searchMoreLinkSpan_smallTile">
                <asp:LinkButton runat="server" ID="standardSearchMoreLink" ClientIDMode="Static"
                    Visible="false" Text="more results..." OnClick="expandTile_Click" OnClientClick="setSearchTileNameCookie('standards');" /></span>
            <asp:Label runat="server" ID="standardNoRecordsMsg" Text="No standards found." Visible="false"
                CssClass="searchTileNoRecordMsg_smallTile" />
        </div>
    </div>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="standardsSearchLoadingPanel" runat="server" />
<telerik:RadXmlHttpPanel runat="server" ID="StandardsCoursesXmlHttpPanel" ClientIDMode="Static"
    Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/StandardsWCF.svc"
    WcfServiceMethod="RequestStandardsCourseList" RenderMode="Block" OnClientResponseEnding="loadFilter_StandardsSearchSmallTile"
    objectToLoadID="standardsCourseSearchDropdown">
</telerik:RadXmlHttpPanel>
<telerik:RadXmlHttpPanel runat="server" ID="StandardsSubjectsXmlHttpPanel" ClientIDMode="Static"
    Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/StandardsWCF.svc"
    WcfServiceMethod="RequestStandardsSubjectList" RenderMode="Block" OnClientResponseEnding="loadFilter_StandardsSearchSmallTile"
    objectToLoadID="standardsSubjectSearchDropdown">
</telerik:RadXmlHttpPanel>
