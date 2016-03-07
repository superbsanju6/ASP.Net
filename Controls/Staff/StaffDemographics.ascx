<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StaffDemographics.ascx.cs"
    Inherits="Thinkgate.Controls.Staff.StaffDemographics" %>

<table style="text-align: center; background-color: white; width: 100%; margin-left: auto; margin-right: auto">
    <tr><td colspan="4" style="text-decoration: underline; font-weight: bold">Previous Year Staff Demographics</td></tr>
    <tr>
        <td width="50"></td>
        <td align="left">District:
            <asp:Label runat="server" ID="lblDistrictCount"></asp:Label>
        </td>
        <td width="10"></td>
        <td align="left">School: 
            <asp:Label runat="server" ID="lblSchoolCount"></asp:Label>
        </td>
    </tr>
</table>
<telerik:RadMultiPage runat="server" ID="RadMultiPageStaffDemo" SelectedIndex="0"
    Height="176px" Width="300px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="radPageViewExperience">
        <telerik:RadChart ID="chartExperience" runat="server" Width="310px" Height="176px" SkinsOverrideStyles="false"
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
    <telerik:RadPageView runat="server" ID="radPageViewCertifications">
        <telerik:RadChart ID="chartCertifications" runat="server" Width="310px" Height="176px" SkinsOverrideStyles="false"
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
    <telerik:RadPageView runat="server" ID="radPageViewEndorsements">
        <telerik:RadChart ID="chartEndorsements" runat="server" Width="310px" Height="176px" SkinsOverrideStyles="false"
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
</telerik:RadMultiPage>
<div class="tabsAndButtonsWrapper_smallTile">
    <telerik:RadTabStrip runat="server" ID="RadTabStrip2" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageStaffDemo" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left">
        <Tabs>
            <telerik:RadTab Text="Experience" runat="server" ID="tabExperience">
            </telerik:RadTab>
            <telerik:RadTab Text="Certifications" runat="server" ID="tabCertification">
            </telerik:RadTab>
            <telerik:RadTab Text="Endorsements" runat="server" ID="tabEndorsements">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    
</div>
