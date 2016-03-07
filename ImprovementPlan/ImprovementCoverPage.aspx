<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImprovementCoverPage.aspx.cs" Inherits="Thinkgate.ImprovementPlan.ImprovementCoverPage" %>

<%@ Register Src="~/ImprovementPlan/ImprovementPlanCoverPageTemplate.ascx" TagPrefix="impPlanCP" TagName="improvementPlanCPControl" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <title></title>
    
    <link href="../Scripts/jquery-ui/jquery.ui.all.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.9.1.js"></script>
    <script src="../Scripts/jquery-1.9.1.min.js"></script>
    <script src="../Scripts/jquery-ui-1.10.1.js"></script>
    <script src="../Scripts/jquery-ui.min.js"></script>
    <script src="../Scripts/jquery-core.js"></script>

</head>
<body>
    <form id="ImprovementPlanCoverPageForm" runat="server" >
         <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/fullscreenBackground.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
              <asp:ScriptReference Path="https://www.google.com/jsapi" />   
               
            </Scripts>
        </telerik:RadScriptManager>
        
         <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />
        <br />        
        <impPlanCP:improvementPlanCPControl ID="impPlanCtrl" runat="server" />
 

           </form>
</body>
</html>
