<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TileContainer_3_1.ascx.cs"
    Inherits="Thinkgate.ContainerControls.TileContainer_3_1" %>
<telerik:RadDockLayout runat="server" ID="radDockLayout" OnSaveDockLayout="RadDockLayout1_SaveDockLayout">
    <telerik:RadDockZone runat="server" ID="RadDockZone1" Orientation="Horizontal" Style="border: 0px;
        float: left; width: 985px;">
        <telerik:RadDock CssClass="radDockContainer" ID="tileContainerDiv1" runat="server"
            AutoPostBack="true" Height="307" Width="325" Title="" EnableAnimation="true"
            DefaultCommands="None" AllowedZones="RadDockZone1" DockMode="Docked" EnableDrag="false">
        </telerik:RadDock>
        <telerik:RadDock CssClass="radDockContainer" ID="tileContainerDiv2" runat="server"
            AutoPostBack="true" Height="307" Width="325" Title="" EnableAnimation="true"
            DefaultCommands="None" AllowedZones="RadDockZone1" DockMode="Docked" EnableDrag="false">
        </telerik:RadDock>
        <telerik:RadDock CssClass="radDockContainer" ID="tileContainerDiv3" runat="server"
            AutoPostBack="true" Height="307" Width="325" Title="" EnableAnimation="true"
            DefaultCommands="None" AllowedZones="RadDockZone1" DockMode="Docked" EnableDrag="false">
        </telerik:RadDock>
    </telerik:RadDockZone>
</telerik:RadDockLayout>
