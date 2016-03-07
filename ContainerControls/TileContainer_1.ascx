<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TileContainer_1.ascx.cs" Inherits="Thinkgate.ContainerControls.TileContainer_1" %>
<telerik:RadDockLayout runat="server" ID="radDockLayout" >
    <telerik:RadDockZone runat="server" ID="RadDockZone1" Orientation="Horizontal" Style="border: 0px;
        float: left; width: 985px;">
        <telerik:RadDock CssClass="radDockContainer" ID="tileContainerDiv1" runat="server"
            AutoPostBack="true" Height="307" Width="325" Title="" EnableAnimation="true"
            DefaultCommands="None" AllowedZones="RadDockZone1" DockMode="Docked" EnableDrag="false">
        </telerik:RadDock>
    </telerik:RadDockZone>
</telerik:RadDockLayout>