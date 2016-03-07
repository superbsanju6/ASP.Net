<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RubricItemResources.aspx.cs"
    Inherits="Thinkgate.Dialogues.RubricItemResources" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4"
        rel="stylesheet" type="text/css" runat="server" />
    <title></title>
    <base target="_self" />
    <style type="text/css">
        .resourceTable
        {
            font-size: 14px;
        }
        
        .resourceTable th
        {
            padding: 5px;
        }
        
        .resourceTable td
        {
            padding: 5px;
        }
    </style>
    <script type="text/javascript">
        
    </script>
</head>
<body style="font-family: Arial, Sans-Serif; font-size: 10pt;">
    <form runat="server" id="mainForm" method="post">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
            <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
            <asp:ScriptReference Path="~/scripts/master.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
        Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
        <asp:Label runat="server" ID="lblNoneFound">No resources found for the selected item.</asp:Label>
        <asp:Repeater runat="server" ID="repeaterResources">
            <HeaderTemplate>
                <table width="100%" class="resourceTable">
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# Eval("ResourceHTML") %>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr>
                    <td style='background-color: #dadada'>
                        <%# Eval("ResourceHTML") %>
                    </td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </telerik:RadAjaxPanel>
    </form>
</body>
</html>
