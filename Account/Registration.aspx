<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="Thinkgate.Account.Registration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="~/Account/Registration.ascx" TagName="Registration" TagPrefix="reg" %>
<%@ Register Src="~/Account/AccountMenu.ascx" TagName="AdminMenu" TagPrefix="adm" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
	<adm:AdminMenu ID="admMenu" runat="server"></adm:AdminMenu>
    <div>
    <reg:Registration ID="reg" runat="server"></reg:Registration>
    </div>
    </form>
</body>
</html>

