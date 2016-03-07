<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchoolDropoutPrevention.ascx.cs" Inherits="Thinkgate.Controls.School.SchoolDropoutPrevention" %>

<telerik:RadMultiPage runat="server" ID="RadMultiPageTeacherSearch" SelectedIndex="0"
    Height="210px" Width="300px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="RadPageViewTeacherSearchChart">
        <telerik:RadChart ID="teacherCountChart" runat="server" Width="310px" Height="210px" SkinsOverrideStyles="false"
            DefaultType="Bar" AutoLayout="true" AutoTextWrap="False">
            
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
