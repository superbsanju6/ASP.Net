<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SuspensionRates.ascx.cs"
    Inherits="Thinkgate.Controls.School.SuspensionRates" %>

<telerik:RadMultiPage runat="server" ID="RadMultiPageSuspensions" SelectedIndex="0"
    Height="210px" Width="300px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="radPageViewInSchool">
        <telerik:RadChart ID="chartInSchool" runat="server" Width="310px" Height="210px" SkinsOverrideStyles="false"
            DefaultType="Bar" AutoLayout="True" AutoTextWrap="True">
            <Appearance>
                <Border Visible="False"></Border>
            </Appearance>
            <PlotArea>
                <XAxis DataLabelsColumn="SchoolYear">
                </XAxis>
                <YAxis AxisMode="Extended" AutoScale="True">
                    <Appearance>
                        <LabelAppearance Visible="False"></LabelAppearance>
                    </Appearance>
                </YAxis>
            </PlotArea>
        </telerik:RadChart>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="radPageViewOutOfSchool">
        <telerik:RadChart ID="chartOutOfSchool" runat="server" Width="310px" Height="210px" SkinsOverrideStyles="false"
            DefaultType="Bar" AutoLayout="True" AutoTextWrap="True" IntelligentLabelsEnabled="True">
            <Appearance>
                <Border Visible="False"></Border>
            </Appearance>
            <PlotArea>
                <XAxis DataLabelsColumn="SchoolYear">
                </XAxis>
                <YAxis AxisMode="Extended" AutoScale="True">
                    <Appearance>
                        <LabelAppearance Visible="False"></LabelAppearance>
                    </Appearance>
                </YAxis>
            </PlotArea>
        </telerik:RadChart>
    </telerik:RadPageView>
</telerik:RadMultiPage>
<div class="tabsAndButtonsWrapper_smallTile">
    <telerik:RadTabStrip runat="server" ID="RadTabStrip2" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageSuspensions" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left">
        <Tabs>
            <telerik:RadTab Text="In School" runat="server" ID="tabInSchool">
            </telerik:RadTab>
            <telerik:RadTab Text="Out of School" runat="server" ID="tabOutOfSchool">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    
</div>
