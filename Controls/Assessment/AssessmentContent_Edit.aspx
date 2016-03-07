<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentContent_Edit.aspx.cs"
    Inherits="Thinkgate.Controls.Assessment.AssessmentContent_Edit" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<%--This height of 100% helps elements fill the page--%>
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <title></title>
    <base target="_self" />
    <telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
    </telerik:RadStyleSheetManager>
    <script type='text/javascript' src='../../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../../Scripts/jquery-migrate-1.1.0.min.js'></script>
    <script type='text/javascript' src='../../Scripts/jquery.scrollTo.js'></script>
    <script type="text/javascript">        var $j = jQuery.noConflict();</script>
</head>
<body>
    <form id="form2" runat="server" style="height: 100%;">
    <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    </telerik:RadAjaxManager>
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
    </telerik:RadSkinManager>
    <telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel"
        Width="100%">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/under-construction-sign.png" ImageAlign="Middle" CssClass="expandedConstructionMargins" />
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    </form>
</body>
</html>
