<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TileContainerDockSix.ascx.cs"
    Inherits="Thinkgate.ContainerControls.TileContainerDockSix" %>
<telerik:RadDockLayout runat="server" ID="radDockLayout" OnSaveDockLayout="RadDockLayout1_SaveDockLayout">
    <telerik:RadDockZone runat="server" ID="RadDockZone1" Orientation="Horizontal" Style="border: 0px;
        float: left; width: 992px;">
        <telerik:RadDock CssClass="radDockContainer padBottom" ID="tileContainerDiv1" runat="server"
            Height="298" Width="325" Title="" EnableAnimation="true" DefaultCommands="None" AutoPostBack="true"
            DockMode="Docked" EnableDrag="false" style="margin-left: 9px;">
        </telerik:RadDock>
        <telerik:RadDock CssClass="radDockContainer padBottom" ID="tileContainerDiv2" runat="server"
            Height="298" Width="325" Title="" EnableAnimation="true" DefaultCommands="None" AutoPostBack="true"
            DockMode="Docked" EnableDrag="false">
        </telerik:RadDock>
        <telerik:RadDock CssClass="radDockContainer padBottom" ID="tileContainerDiv3" runat="server"
            Height="298" Width="325" Title="" EnableAnimation="true" DefaultCommands="None" AutoPostBack="true"
            DockMode="Docked" EnableDrag="false">
        </telerik:RadDock>
        <telerik:RadDock CssClass="radDockContainer" ID="tileContainerDiv4" runat="server"
            Height="298" Width="325" Title="" EnableAnimation="true" DefaultCommands="None" AutoPostBack="true"
            DockMode="Docked" EnableDrag="false" style="margin-left: 9px;">
        </telerik:RadDock>
        <telerik:RadDock CssClass="radDockContainer" ID="tileContainerDiv5" runat="server"
            Height="298" Width="325" Title="" EnableAnimation="true" DefaultCommands="None" AutoPostBack="true"
            DockMode="Docked" EnableDrag="false">
        </telerik:RadDock>
        <telerik:RadDock CssClass="radDockContainer" ID="tileContainerDiv6" runat="server"
            Height="298" Width="325" Title="" EnableAnimation="true" DefaultCommands="None" AutoPostBack="true"
            DockMode="Docked" EnableDrag="false">
        </telerik:RadDock>
    </telerik:RadDockZone>
</telerik:RadDockLayout>
