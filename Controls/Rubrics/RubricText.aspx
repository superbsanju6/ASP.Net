<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RubricText.aspx.cs" Inherits="Thinkgate.Controls.Rubrics.RubricText" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title id="PageTitle" runat="server"></title>
    <style>
        .divcenter
        {
            height:300px;
            width:400px;
            margin:0px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="divcenter">
            <asp:Label runat="server" ID="lblRubricText"></asp:Label>
        </div>
    </form>
</body>
</html>
