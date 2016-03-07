<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImprovementPlanStrategyPage.aspx.cs"
    Inherits="Thinkgate.ImprovementPlan.ImprovementPlanStrategyPage" EnableEventValidation="false" ValidateRequest="false" %>

<%@ Register Src="~/ImprovementPlan/ImprovementPlanStrategyTemplate.ascx" TagPrefix="impPlan" TagName="improvementPlanControl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />

    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet"
        type="text/css" runat="server" />

    <title></title>
    <link href="../Scripts/jquery-ui/jquery.ui.all.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.9.1.js"></script>
    <script src="../Scripts/jquery-1.9.1.min.js"></script>
    <script src="../Scripts/jquery-ui-1.10.1.js"></script>
    <script src="../Scripts/jquery-ui.min.js"></script>
    <script src="../Scripts/jquery-core.js"></script>
    <script src="../Scripts/jquery.elastic.js"></script>

    <script type="text/javascript">
        //jQuery(document).ready(function () {

        //    jQuery('input[type=text], textarea').elastic();

        //});
    </script>
</head>
<body>
    <form id="ImprovementPlanStrategyPageForm" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/fullscreenBackground.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
                <asp:ScriptReference Path="https://www.google.com/jsapi" />

            </Scripts>
        </telerik:RadScriptManager>

        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />
        <br />        
        <impPlan:improvementPlanControl ID="impPlanCtrl" runat="server" />
    </form>
</body>
</html>
