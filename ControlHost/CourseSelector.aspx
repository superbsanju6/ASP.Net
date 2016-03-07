<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseSelector.aspx.cs"
    Inherits="Thinkgate.ControlHost.CourseSelector" %>

<%@ Register Src="~/Controls/Course/CourseSelector.ascx" TagName="CourseSelector" TagPrefix="th" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Course Selector</title>    
    <link href="~/Styles/Menu.iPhone.css" rel="stylesheet" type="text/css" />    
</head>
<body>
    <form id="mainForm" runat="server" method="post">
         <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <th:CourseSelector runat="server" ID="ctlCourseSelector" />
    </form>
</body>
</html>
