<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pacing.ascx.cs" Inherits="Thinkgate.Controls.Plans.Pacing" %>
<telerik:RadAjaxPanel runat="server" ID="pacingPanel" LoadingPanelID="pacingLoadingPanel">
    <script type="text/javascript">
        function refreshPacingTile() {
            document.getElementById('btnPacingTileHidden').click();
        }

        function RadCalendar1_DateSelected(o, args) {
            var get_date = args.get_renderDay().get_date();
            var date = get_date[1] + "/" + get_date[2] + "/" + get_date[0];            
            if (ShowInstructionalPlanCalendar_Day) {
                ShowInstructionalPlanCalendar_Day(date);
            }
        }       
    </script>
    <style type="text/css">
        .pacingWeekView
        {
            background-color: White;
            width: 100%;
            font-size: 8pt;
        }
        
        .pacingWeekView th
        {
            background-color: Navy;
            color: White;
            text-align: center;
            border: 1px solid black;
            width: 20%;
        }
        
        .pacingWeekView td
        {
            text-align: center;
            padding-left: 2px;
            border: 1px solid black;
            height: 30px;
        }
    </style>
    <telerik:RadMultiPage runat="server" ID="RadMultiPageClassSummary" SelectedIndex="0"
        Height="210px" Width="300px" CssClass="multiPage">
        <telerik:RadPageView runat="server" ID="radPageViewDay">
            <asp:Button runat="server" ID="btnPacingTileHidden" ClientIDMode="Static" Style="display: none" />
            <div style="width: 100%; height: 20px; padding: 3px; border-bottom: 1px solid black;
                text-align: center; font-weight: bold;">
                <asp:ImageButton runat="server" ID="previousButton" OnClick="previousButton_Click"
                    AlternateText="Previous Day" ToolTip="Previous Day" ImageUrl="~/Images/arrow_left.png" />
                <asp:Label runat="server" ID="lblCurrentDay" />
                <asp:ImageButton runat="server" ID="nextButton" OnClick="nextButton_Click" AlternateText="Next Day"
                    ToolTip="Next Day" ImageUrl="~/Images/arrow_right.png" />
            </div>
            <div style="width: 100%; height: 183px; overflow: auto; padding: 3px;">
                <table width="100%">
                    <tr>
                        <td style="font-weight: bold">
                            Standards:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblNoStandards" Visible="false" Text="No standards for this day" />
                            <asp:Repeater runat="server" ID="dayViewStandardsRepeater">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="standardNameLink">
                                        <a href="StandardsPage.aspx?xID=<%# Eval("CategoryID_Encrypted") %>" target="_blank"><%# Eval("Subject")%></a>                                    
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold">
                            My Lesson Plans:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblNoLessonPlans" Visible="false" Text="No lesson plans for this day" />
                            <asp:Repeater runat="server" ID="dayViewLessonPlansRepeater" OnItemDataBound="lbxLessonPlans_ItemDataBound">
                                <ItemTemplate>
                                    <asp:HyperLink ID="lnkPlanName" runat="server" NavigateUrl="" Target="_blank">
                                        <%# Eval("Subject")%>
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold">
                            My Resources:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblNoResources" Visible="false" Text="No resources for this day" />
                            <asp:Repeater runat="server" ID="dayViewResourcesRepeater" OnItemDataBound="lbxResources_ItemDataBound">
                                <ItemTemplate>
                                    <asp:HyperLink ID="lnkResourceName" runat="server" NavigateUrl="" Target="_blank">
                                        <%# Eval("Subject")%>
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold">
                            My Assessments:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblNoAssessments" Visible="false" Text="No assessments for this day" />
                            <asp:Repeater runat="server" ID="dayViewAssessmentsRepeater">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="assessmentNameLink">
                                    <a href="AssessmentObjects.aspx?xID=<%# Eval("CategoryID_Encrypted") %>" target="_blank"><%# Eval("Subject")%></a>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                </table>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="radPageViewWeek">
            <div style="width: 100%; height: 20px; padding: 3px; border-bottom: 1px solid black;
                text-align: center; font-weight: bold;">
                <asp:ImageButton runat="server" ID="previousWeekBtn" OnClick="previousWeekBtn_Click"
                    AlternateText="Previous Week" ToolTip="Previous Week" ImageUrl="~/Images/arrow_left.png" />
                <asp:Label runat="server" ID="lblCurrentWeek" />
                <asp:HiddenField runat="server" ID="hiddenCurrentStartOfWeek" />
                <asp:ImageButton runat="server" ID="nextWeekBtn" OnClick="nextWeekBtn_Click" AlternateText="Next Week"
                    ToolTip="Next Week" ImageUrl="~/Images/arrow_right.png" />
            </div>
            <div style="width: 100%; height: 183px; overflow: auto; padding: 3px;">
                <table class="pacingWeekView">
                    <tr>
                        <th>
                        </th>
                        <th>
                            Standards
                        </th>
                        <th>
                            Plans
                        </th>
                        <th>
                            Resources
                        </th>
                        <th>
                            Assessments
                        </th>
                    </tr>
                    <asp:Repeater runat="server" ID="dayOfWeekRepeater">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="assessmentNameLink">
                                        <a href="#" href="javascript:ShowInstructionalPlanCalendar_Day('<%# Eval("DayStringForLink") %>')">
                                            <%# Eval("FormattedDay")%>
                                        </a>
                                    </asp:Label>
                                </td>
                                <td>
                                    <%# Eval("StandardsDisplayHTML")%>
                                </td>
                                <td>
                                    <%# Eval("LessonPlansDisplayHTML")%>
                                </td>
                                <td>
                                    <%# Eval("ResourcesDisplayHTML")%>
                                </td>
                                <td>
                                    <%# Eval("AssessmentsDisplayHTML")%>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="radPageViewMonth">
            <asp:HiddenField runat="server" ID="hiddenCurrentMonthStart" />
            <div style="width: 100%; height: 203px; overflow: auto; padding: 3px;">
                <telerik:RadCalendar ID="RadCalendar1" runat="server" TitleFormat="MMMM yyyy" AutoPostBack="false"
                    OnDayRender="RadCalendar1_DayRender" EnableMultiSelect="false"
                    Width="100%" Height="180px">
                    <ClientEvents OnDateSelected="RadCalendar1_DateSelected" />
                </telerik:RadCalendar>
            </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    <telerik:RadTabStrip runat="server" ID="pacingTileRadTabStrip" Orientation="HorizontalBottom"
        ClientIDMode="Static" SelectedIndex="0" MultiPageID="RadMultiPageClassSummary"
        Skin="Thinkgate_Blue" EnableEmbeddedSkins="false" Align="Left" OnClientTabSelected="toggleView_RadTab_SmallTile">
        <Tabs>
            <telerik:RadTab Text="Day">
            </telerik:RadTab>
            <telerik:RadTab Text="Week">
            </telerik:RadTab>
            <telerik:RadTab Text="Month">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="pacingLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>
