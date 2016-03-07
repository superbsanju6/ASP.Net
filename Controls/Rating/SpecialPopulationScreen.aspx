<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SpecialPopulationScreen.aspx.cs" Inherits="Thinkgate.Controls.Rating.SpecialPopulationScreen" %>

<%@ Register TagPrefix="e3" TagName="SpecialPopulation" Src="~/Controls/Rating/SpecialPopulationUC.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link id="lnkWindowStyle" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet"
        type="text/css" runat="server" />
    <link rel="stylesheet" type="text/css" href="~/Styles/Ratings/SpecialPopulations.css" runat="server" />
    <title></title>
    
    <style>
        
        body
        {
            background-color: #DEE2E7;
        }

    </style>

    <base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="radScriptManager" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
                <asp:ScriptReference Path="~/scripts/master.js" />
            </Scripts>
        </telerik:RadScriptManager>

        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false" Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />

        <asp:Panel runat="server" ID="pnlControl" Width="100%">
            <div>
                <e3:SpecialPopulation ClientIDMode="Static" ID="specialPopulationUC" runat="server"/>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
