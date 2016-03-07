<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestCriteria.aspx.cs" Inherits="Thinkgate.Record.TestCriteria" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />

</head>
<body>
    <form id="form1" runat="server">
      <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <%--Needed for JavaScript IntelliSense in VS2010--%>
                <%--For VS2008 replace RadScriptManager with ScriptManager--%>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/fullscreenBackground.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
                <asp:ScriptReference Path="https://www.google.com/jsapi" />
                    
            </Scripts>
            </telerik:RadScriptManager>
    <div>
        <asp:PlaceHolder ID="phTestCriteria" runat="server">

        </asp:PlaceHolder>
        <asp:TextBox ID="hiddenTxtBox" runat="server" Style="visibility: hidden;" />
    </div>
    </form>

</body>
</html>
