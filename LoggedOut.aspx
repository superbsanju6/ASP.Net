<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoggedOut.aspx.cs" Inherits="Thinkgate.LoggedOut" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Logged Out</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>
        Logged Out</h1>
    <p>
        You have been logged out of the application.  Please close your browser.</p>
    </div>
    </form>
    <script type="text/javascript">
        window.open('', '_self', '');
        window.close();
    </script>
</body>
</html>
