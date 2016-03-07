<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TGSecurityXref.aspx.cs" Inherits="Thinkgate.ControlHost.TGSecurityXref" %>

<%@ Register Src="~/Controls/TGSecurityXref.ascx" TagName="Xref" TagPrefix="xref" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>    
</head>
<body>
    <form id="mainForm" runat="server" method="post">
         <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>

		<xref:Xref ID="xref" runat="server"></xref:Xref>
    </form>
</body>
</html>
