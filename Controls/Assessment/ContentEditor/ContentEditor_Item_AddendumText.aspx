<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentEditor_Item_AddendumText.aspx.cs" Inherits="Thinkgate.Controls.Assessment.ContentEditor.ContentEditor_Item_AddendumText" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title id="PageTitle" runat="server"></title>
    <link href="~/Styles/ItemBanks/MCAS.css" rel="stylesheet" type="text/css"/>
    <link href="~/Styles/ItemBanks/Inspect.css" rel="stylesheet" type="text/css"/>
    <link href="~/Styles/ItemBanks/NWEA.css" rel="stylesheet" type="text/css"/>
    <style>
        .divcenter
        {
            height:300px;
            width:800px;
            margin:0px;
            overflow:auto;
            padding:5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="divcenter">
        <asp:Label runat="server" ID="lblAddendumText"></asp:Label>
    </div>
    </form>
</body>
</html>
