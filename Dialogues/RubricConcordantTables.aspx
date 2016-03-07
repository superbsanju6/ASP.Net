<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RubricConcordantTables.aspx.cs"
    Inherits="Thinkgate.Dialogues.RubricConcordantTables" %>

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
        .concordantRangesTable td
        {
            text-align: center;
            padding: 3px;
            border: 1px solid black;
            font-size: 12px;
        }
        
        .concordantRangesTable th
        {
            text-align: center;
            border: 1px solid black;
            font-weight: bold;
            padding: 3px;
        }
        
        div.tableDivWrapper
        {
            width: 200px;
            float: left;
            padding-left: 30px;
            padding-bottom: 50px;
            text-align: center;
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
        <asp:Label runat="server" ID="lblNoTablesFound" Text="No tables found" />
        <asp:Repeater runat="server" ID="tablesRepeater" OnItemDataBound="tablesRepeater_ItemDataBound">
            <ItemTemplate>
                <div class="tableDivWrapper">                    
                    <asp:Repeater runat="server" ID="concordantRangesTable" OnItemDataBound="concordantRangesTable_ItemDataBound">
                        <HeaderTemplate>
                            <table width="100%" class="concordantRangesTable">
                                <tr>
                                    <th colspan="3">
                                        <asp:Label runat="server" ID="lblTableHeader" />
                                    </th>
                                </tr>
                                <tr>
                                    <th>
                                        Min
                                    </th>
                                    <th>
                                        Max
                                    </th>
                                    <th>
                                        Concordant
                                    </th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr style='background-color: <%# Eval("Color") %>'>
                                <td>
                                    <%# Eval("minValue")%>
                                </td>
                                <td>
                                    <%# Eval("maxValue")%>
                                </td>
                                <td>
                                    <%# Eval("concordantValue")%>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table></FooterTemplate>
                    </asp:Repeater>
                    <asp:Label runat="server" ID="lblNoDataFound" Text="No data found" style="font-weight: bold" />
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Panel runat="server" ID="panelGroup5Data" Visible="false">
        </asp:Panel>

    </telerik:RadAjaxPanel>
    </form>
</body>
</html>
