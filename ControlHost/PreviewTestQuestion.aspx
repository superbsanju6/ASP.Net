<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewTestQuestion.aspx.cs" Inherits="Thinkgate.ControlHost.PreviewTestQuestion" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/ItemBanks/Inspect.css" rel="stylesheet" type="text/css"/>
    <link href="~/Styles/ItemBanks/MCAS.css" rel="stylesheet" type="text/css"/>
    <link href="~/Styles/ItemBanks/NWEA.css" rel="stylesheet" type="text/css"/>
    <title></title>
    <base target="_self" />
    <telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
    </telerik:RadStyleSheetManager>
    <script type='text/javascript' src='../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../Scripts/jquery-migrate-1.1.0.min.js'></script>
    <script type='text/javascript' src='../Scripts/jquery.scrollTo.js'></script>
    <script type="text/javascript">var $j = jQuery.noConflict();</script>
    <style>
        p {
          margin: 0;  
          line-height: 1.1;
        }
    </style>
</head>
<body style="background-image: none !important; background-color: white">
    <form id="form3" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
        </Scripts>
    </telerik:RadScriptManager>
        <telerik:RadScriptBlock runat="server" ID="mathjaxLoad" >
            
            <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$appSettings:MathJaxUrl%>' />/MathJax.js?config=TeX-AMS-MML_HTMLorMML,<asp:Literal runat='server' Text='<%$appSettings:MathJaxUrl%>' />/config/local/thinkgate-e3.js"></script>

        </telerik:RadScriptBlock>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    </telerik:RadAjaxManager>
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
    </telerik:RadSkinManager>
    <telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel"
        Width="100%" Height="100%">
        <asp:PlaceHolder runat="server" ID="testQuestionPlaceHolder"></asp:PlaceHolder><br/>
        <asp:PlaceHolder runat="server" ID="standardPlaceHolder"></asp:PlaceHolder><br/>
        <asp:PlaceHolder runat="server" ID="addendumPlaceHolder"></asp:PlaceHolder><br/>
        <asp:PlaceHolder runat="server" ID="rubricPlaceHolder"></asp:PlaceHolder><br/>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    <script type="text/javascript">
        function toggleCollapsedText(obj, containerID) {
            if(obj.src.indexOf('plusik') > -1) {
                obj.src = obj.src.replace('plusik', 'minus');
                $('#' + containerID).css('display', '');
            }
            else {
                obj.src = obj.src.replace('minus', 'plusik');
                $('#' + containerID).css('display', 'none');
            }
        }
        
        MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
    </script>
    </form>
</body>
</html>
