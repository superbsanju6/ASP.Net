<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentAssessmentResultsChart.ascx.cs"
    Inherits="Thinkgate.Controls.Student.StudentAssessmentResultsChart" %>

<a href="javascript: expandResultsChart();">Expand Me</a>
<br />

<telerik:RadChart ID="ResultsChart" runat="server" Width="290px" Height="220px" DefaultType="Line"
    AutoLayout="true" AutoTextWrap="true" CreateImageMap="false" ChartTitle-TextBlock-Visible="false">
    <Appearance Border-Visible="false" />
    <Series>
        <telerik:ChartSeries DataYColumn="RawScore" Name="RawScore" Type="Line" DataLabelsColumn="TestName">
            <Appearance LegendDisplayMode="Nothing">
            </Appearance>
        </telerik:ChartSeries>
    </Series>
</telerik:RadChart>

<telerik:raddock id="RadDock3" title="RadDock3" runat="server" CssClass="onTop"  width="200px" dockmode="Floating" Top="200px" Left="600px"
                            text="DockMode is Floating. This object cannot be docked ">

                        </telerik:raddock>

