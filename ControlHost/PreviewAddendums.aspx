<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewAddendums.aspx.cs" Inherits="Thinkgate.ControlHost.PreviewAddendums" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
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
    <script type='text/javascript' src='../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../Scripts/jquery-migrate-1.1.0.min.js'></script>
    <script type='text/javascript' src='../Scripts/jquery.scrollTo.js'></script>
    <script type="text/javascript">var $j = jQuery.noConflict();</script>
    <style>
        .fullText 
        {
            display: none;
            position: absolute;
            z-index: 9999;
            background-color: #FFF;
            border-top: solid 1px #D0D0D0;
            border-left: solid 1px #D0D0D0;
            border-right: solid 2px #A0A0A0;
            border-bottom: solid 2px #A0A0A0;
            filter: progid:DXImageTransform.Microsoft.Shadow(color='#969696',  Direction=135, Strength=3);
            width: 97%;
            top: 0px;
            left: 0px;
            padding: 2px;
        }
    </style>
</head>
<body style="background-image: none !important;">

<script type="text/javascript">
    function displayFullDescription(obj) {
        var fullTextSpan = $('.fullText', obj.parentNode);
        fullTextSpan.css('display', 'inline');
    }
</script>
    <form id="form3" runat="server">
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
        Width="100%" Height="100%">
        <div runat="server" id="addendumPreviewDiv" clientidmode="Static"></div>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    </form>
</body>
</html>
