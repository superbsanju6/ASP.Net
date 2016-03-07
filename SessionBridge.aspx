<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SessionBridge.aspx.cs"
    Inherits="Thinkgate.SessionBridge" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
</head>
<body onload="document.getElementById('frmSessionBridge').submit();">
    Please wait while you are redirected...
    <form runat="server" id="frmSessionBridge" name='frmSessionBridge' method='post'
    action=''>
    <input runat="server" type='hidden' name='UserID' id='UserID' />
    <input runat="server" type='hidden' name='UserPage' id='UserPage' />
    <input runat="server" type='hidden' name='Password' id='Password' />
    <input runat="server" type='hidden' name='ClientID' id='ClientID' />
    <input runat="server" type='hidden' name='ReturnURL' id='ReturnURL' />    
    </form>
</body>
</html>
