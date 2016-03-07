<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AbsenceRates.ascx.cs"
    Inherits="Thinkgate.Controls.School.AbsenceRates" %>

<telerik:RadMultiPage runat="server" ID="RadMultiPageAbsences" SelectedIndex="0"
    Height="210px" Width="300px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="radPageViewAbsences">
        <telerik:RadChart ID="chartAbsences" runat="server" Width="310px" Height="210px" SkinsOverrideStyles="false"
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
    <telerik:RadPageView runat="server" ID="radPageViewTardies">
        <telerik:RadChart ID="chartTardies" runat="server" Width="310px" Height="210px" SkinsOverrideStyles="false"
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
        SelectedIndex="0" MultiPageID="RadMultiPageAbsences" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left">
        <Tabs>
            <telerik:RadTab Text="Absences" runat="server" ID="tabAbsences">
            </telerik:RadTab>
            <telerik:RadTab Text="Tardies" runat="server" ID="tabTardies">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    
</div>
