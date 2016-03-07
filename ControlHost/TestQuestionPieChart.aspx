<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestQuestionPieChart.aspx.cs" Inherits="Thinkgate.ControlHost.TestQuestionPieChart" %>

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
</head>
<body style="background-image: none !important;">
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
        
        <telerik:RadChart ID="testQuestionPieChart" runat="server" Width="480px" Height="240px" DefaultType="Pie" Skin="Marble"
            AutoLayout="true" AutoTextWrap="False">                    
        </telerik:RadChart>
        <br />

        <table width="100%">
            <tr runat="server" id="RationaleA">
                <td style="font-weight: bold">A</td>
                <td>(<asp:Label runat="server" ID="lblAPercent" />)</td>
                <td><asp:Label runat="server" ID="lblADescription" /></td>
            </tr>
            <tr runat="server" id="RationaleB">
                <td style="font-weight: bold">B</td>
                <td>(<asp:Label runat="server" ID="lblBPercent" />)</td>
                <td><asp:Label runat="server" ID="lblBDescription" /></td>
            </tr>
            <tr runat="server" id="RationaleC">
                <td style="font-weight: bold">C</td>
                <td>(<asp:Label runat="server" ID="lblCPercent" />)</td>
                <td><asp:Label runat="server" ID="lblCDescription" /></td>
            </tr>
            <tr runat="server" id="RationaleD">
                <td style="font-weight: bold">D</td>
                <td>(<asp:Label runat="server" ID="lblDPercent" />)</td>
                <td><asp:Label runat="server" ID="lblDDescription" /></td>
            </tr>
            <tr runat="server" id="RationaleE">
                <td style="font-weight: bold">E</td>
                <td>(<asp:Label runat="server" ID="lblEPercent" />)</td>
                <td><asp:Label runat="server" ID="lblEDescription" /></td>
            </tr>
        </table>

    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    </form>
</body>
</html>
