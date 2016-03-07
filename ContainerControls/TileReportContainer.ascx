<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TileReportContainer.ascx.cs"
    Inherits="Thinkgate.ContainerControls.TileReportContainer" %>
<telerik:RadDockLayout runat="server" ID="radDockLayout">
    <telerik:RadDockZone runat="server" ID="RadDockZoneReports" Orientation="Horizontal" Style="border: 0px;
        float: left; width: 985px;">
        <telerik:RadDock CssClass="radReportCriteriaContainer" ID="tileContainerDiv1" runat="server" EnableDrag="false"
            Height="600" Width="200" Title="" EnableAnimation="true" DefaultCommands="None"
            DockMode="Docked">
        </telerik:RadDock>
        <telerik:RadDock CssClass="radReportResultsContainer" ID="tileContainerDiv2" runat="server" EnableDrag="false"
            Height="600" Width="775" Title="" EnableAnimation="true" DefaultCommands="None"
            DockMode="Docked">
        </telerik:RadDock>        
    </telerik:RadDockZone>
</telerik:RadDockLayout>
