<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentGrowthData.aspx.cs"
    Inherits="Thinkgate.Dialogues.StudentGrowthData" %>

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
        .sgTable
        {
            font-size: 12px;
        }
        
        .sgTableHeadCell
        {
            padding: 5px;
            color: #000099;
            font-weight: bold;   
            text-align: center;  
            border: 1px solid black;       
            font-size: 14px;
        }
        
        .sgTable td
        {
            padding: 5px;
            border: 1px solid black;
            text-align: center;  
        }
        
        .altTD
        {
            background-color: #CCFFFF;
        }
        
        .headerTable 
        {
            font-size: 14px;
        }
        
        .headerTable th
        {
            padding: 5px;
            font-weight: bold;
            border: 1px solid black;
        }
                
        .normalText 
        {
            font-weight: normal;
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
            <table width="100%" class="headerTable">
                <tr>
                    <th colspan="2"><asp:Label runat="server" ID="studentGrowthData_StaffTitle" ClientIDMode="Static" ></asp:Label><asp:Label ID="lblTeacherName" runat="server" CssClass="normalText"/></th>
                    <th rowspan="3" style="text-align: center; vertical-align: middle;">
                        <img src="../Images/Toolbars/print.png" alt="Print" onclick="window.print()" style="cursor: pointer;" />
                    </th>
                </tr>
                <tr>
                    <th colspan="2"><asp:Label runat="server" ID="lblUserNameLabel" />: <asp:Label ID="lblTeacherUserName" runat="server" CssClass="normalText" /></th>
                </tr>
                <tr>
                    <th>Student Growth Concordant: <asp:Label ID="lblStudentGrowthConcordant" runat="server" CssClass="normalText"/></th>
                </tr>
            </table>     
            <asp:Panel runat="server" ID="studentGrowDataTable"></asp:Panel>   
    </telerik:RadAjaxPanel>
    </form>
</body>
</html>
