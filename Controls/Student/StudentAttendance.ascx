<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentAttendance.ascx.cs"
    Inherits="Thinkgate.Controls.Student.StudentAttendance" %>
<telerik:RadAjaxPanel runat="server" ID="studentAttendancePanel" LoadingPanelID="studentAttendanceLoadingPanel">
    <telerik:RadComboBox ID="cmbYear" runat="server" ToolTip="Select a year" Skin="Web20"
        Width="95" OnSelectedIndexChanged="cmbYear_SelectedIndexChanged" AutoPostBack="true"
        CausesValidation="False" HighlightTemplatedItems="true">
    </telerik:RadComboBox>
    <telerik:RadComboBox ID="cmbTerm" runat="server" ToolTip="Select a term" Skin="Web20"
        Width="69" OnSelectedIndexChanged="cmbTerm_SelectedIndexChanged" AutoPostBack="true"
        CausesValidation="False" HighlightTemplatedItems="true">
    </telerik:RadComboBox>
    <telerik:RadMultiPage runat="server" ID="RadMultiPageStudentAttendance" SelectedIndex="0"
        Height="185px" Width="300px" CssClass="multiPage">
        <telerik:RadPageView runat="server" ID="radPageViewWTD">
            <center>
                <telerik:RadChart runat='server' ID="radChartWTD" DefaultType="Bar" ChartTitle-Visible="false"
                    Width="275" Height="180" Legend-Visible="false">
                    <PlotArea>
                        <YAxis MaxValue="6" Step="1" AutoScale="false" Appearance-MinorGridLines-Visible="false" Appearance-MinorTick-Visible="false">
                        </YAxis>
                        <XAxis Appearance-LabelAppearance-Visible="false" />
                    </PlotArea>
                    <Series>
                        <telerik:ChartSeries Name="Week to Date">
                            <Items>
                                <telerik:ChartSeriesItem YValue="4" Name="Present" Label-TextBlock-Text="Present" Appearance-FillStyle-MainColor="Blue" Appearance-FillStyle-SecondColor="Blue" Appearance-Border-Color="White">
                                </telerik:ChartSeriesItem>
                                <telerik:ChartSeriesItem YValue="1" Name="Absent" Label-TextBlock-Text="Absent" Appearance-FillStyle-MainColor="Red" Appearance-FillStyle-SecondColor="Red" Appearance-Border-Color="White">
                                </telerik:ChartSeriesItem>
                                <telerik:ChartSeriesItem YValue="0" Name="Tardy" Label-TextBlock-Text="Tardy" Appearance-FillStyle-MainColor="Green" Appearance-FillStyle-SecondColor="Green" Appearance-Border-Color="White">
                                </telerik:ChartSeriesItem>
                            </Items>
                        </telerik:ChartSeries>
                    </Series>
                </telerik:RadChart>
            </center>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="radPageViewMTD">
            <center>
                <telerik:RadChart runat='server' ID="radChartMTD" DefaultType="Bar" ChartTitle-Visible="false"
                    Width="275" Height="180" Legend-Visible="false">
                    <PlotArea>
                        <YAxis MaxValue="20" Step="5" AutoScale="false" Appearance-MinorGridLines-Visible="false" Appearance-MinorTick-Visible="false">
                        </YAxis>
                        <XAxis Appearance-LabelAppearance-Visible="false" />
                    </PlotArea>
                    <Series>
                        <telerik:ChartSeries Name="Week to Date">
                            <Items>
                                <telerik:ChartSeriesItem YValue="16" Label-TextBlock-Text="Present" Appearance-FillStyle-MainColor="Blue" Appearance-FillStyle-SecondColor="Blue" Appearance-Border-Color="White">
                                </telerik:ChartSeriesItem>
                                <telerik:ChartSeriesItem YValue="2" Label-TextBlock-Text="Absent" Appearance-FillStyle-MainColor="Red" Appearance-FillStyle-SecondColor="Red" Appearance-Border-Color="White">
                                </telerik:ChartSeriesItem>
                                <telerik:ChartSeriesItem YValue="2" Label-TextBlock-Text="Tardy" Appearance-FillStyle-MainColor="Green" Appearance-FillStyle-SecondColor="Green" Appearance-Border-Color="White">
                                </telerik:ChartSeriesItem>
                            </Items>
                        </telerik:ChartSeries>
                    </Series>
                </telerik:RadChart>
            </center>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="radPageViewYTD">
            <center>
                <telerik:RadChart runat='server' ID="radChartYTD" DefaultType="Bar" ChartTitle-Visible="false"
                    Width="275" Height="180" Legend-Visible="false">
                    <PlotArea>
                        <YAxis MaxValue="100" Step="10" AutoScale="false" Appearance-MinorGridLines-Visible="false" Appearance-MinorTick-Visible="false">
                        </YAxis>
                        <XAxis Appearance-LabelAppearance-Visible="false" />
                    </PlotArea>
                    <Series>
                        <telerik:ChartSeries Name="Week to Date">
                            <Items>
                                <telerik:ChartSeriesItem YValue="81" Label-TextBlock-Text="Present" Appearance-FillStyle-MainColor="Blue" Appearance-FillStyle-SecondColor="Blue" Appearance-Border-Color="White">
                                </telerik:ChartSeriesItem>
                                <telerik:ChartSeriesItem YValue="7" Label-TextBlock-Text="Absent" Appearance-FillStyle-MainColor="Red" Appearance-FillStyle-SecondColor="Red" Appearance-Border-Color="White">
                                </telerik:ChartSeriesItem>
                                <telerik:ChartSeriesItem YValue="4" Label-TextBlock-Text="Tardy" Appearance-FillStyle-MainColor="Green" Appearance-FillStyle-SecondColor="Green" Appearance-Border-Color="White">
                                </telerik:ChartSeriesItem>
                            </Items>
                        </telerik:ChartSeries>
                    </Series>
                </telerik:RadChart>
            </center>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    <telerik:RadTabStrip runat="server" ID="studentAttendanceRadTabStrip" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageStudentAttendance" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left" OnClientTabSelected="toggleView_RadTab_SmallTile">
        <Tabs>
            <telerik:RadTab Text="WTD" Width="90px">
            </telerik:RadTab>
            <telerik:RadTab Text="MTD" Width="90px">
            </telerik:RadTab>
            <telerik:RadTab Text="YTD" Width="90px">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="studentAttendanceLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>
