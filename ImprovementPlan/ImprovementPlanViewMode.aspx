<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImprovementPlanViewMode.aspx.cs" Inherits="Thinkgate.ImprovementPlan.ImprovementPlanViewMode" EnableEventValidation="false" ValidateRequest="false" %>

<%@ Register Src="~/ImprovementPlan/ImprovementPlanStrategyTemplate.ascx" TagPrefix="impPlan" TagName="improvementPlanControl" %>
<%@ Register Src="~/ImprovementPlan/ImprovementPlanCoverPageTemplate.ascx" TagPrefix="impPlanCP" TagName="improvementPlanCPControl" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />

    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet"
        type="text/css" runat="server" />

    <title></title>
    <link href="../Scripts/jquery-ui/jquery.ui.all.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.9.1.js"></script>
    <script src="../Scripts/jquery-1.9.1.min.js"></script>
    <script src="../Scripts/jquery-ui-1.10.1.js"></script>
    <script src="../Scripts/jquery-ui.min.js"></script>
    <script src="../Scripts/jquery-core.js"></script>

    <style type="text/css">
      
    </style>

    <script>
        function deleteImprovementPlan(object) {
            var height = $telerik.$(window).height();
            var width = $telerik.$(window).width();

            var height_Cal = (height * 0.1);
            var width_Cal = (width * 0.35);


            customDialog({
                title: "Alert",
                height: height_Cal,
                width: width_Cal,
                autoSize: false,
                content: "Are you certain you would like to delete this entire Improvement Plan?. <br/> Select OK to proceed or CANCEL to return to the current form.",
                dialog_style: "alert",
            }, [{ title: "Cancel" }, { title: "Ok", callback: deletePlan }]);
        }

        function deletePlan() {
            raiseEvent('btnDelete', '');
           
        }

        function refreshOpenerPage() {

            if (window.opener != null && window.opener.location != null)
                window.opener.location.reload();
            window.close();
        }

        function exportToExcel() {
            raiseEvent('btnExcel', '');
        }

        function exportToPDF() {
            raiseEvent('btnPDF', '');
        }


        function raiseEvent(objectID) {
            __doPostBack(objectID, '');
        }

    </script>
</head>
<body>
    <form id="ImprovementPlanCoverPageForm" runat="server" style="background: lavender">
        <telerik:RadScriptManager ID="radScriptManager" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/fullscreenBackground.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
                <asp:ScriptReference Path="https://www.google.com/jsapi" />

            </Scripts>
        </telerik:RadScriptManager>

        <telerik:RadWindowManager ID="radWindowManager" runat="server" EnableShadow="false"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />

        <div id="dvViewContainer">
            <telerik:RadAjaxPanel runat="server" LoadingPanelID="improvementPlanView" Width="100%">
                <div id="dvCoverPage">
                    <ul>
                        <li style="float:right; list-style:none; padding:3px">
                            <asp:ImageButton ID="imgDelete" runat="server" ToolTip="Delete" ClientIDMode="Static" ImageUrl="~/Images/icons/delete.png" Width="40px" AutoPostBack="false" OnClientClick="deleteImprovementPlan(this); return false;" />
                        </li>
                        <li style="float:right; list-style:none; padding:3px">
                            <asp:ImageButton ID="imgPDF" runat="server" ToolTip="Save to PDF" ClientIDMode="Static" ImageUrl="~/Images/icons/PDF.png" Width="40px" AutoPostBack="false" OnClientClick="exportToPDF(); return false;" />
                        </li>
                        <li style="float:right; list-style:none; padding:3px">
                            <asp:ImageButton ID="imgExcel" runat="server" ToolTip="Save to Excel" ClientIDMode="Static" ImageUrl="~/Images/icons/excel.png" Width="40px" AutoPostBack="false" OnClientClick="exportToExcel(this); return false;" />
                        </li>
                    </ul>
                </div>

                <br />
                <br />

                <div id="dvCoverDetails">
                    <impPlanCP:improvementPlanCPControl ID="impPlanCPCtrl" runat="server" isPDF="false"/>
                </div>

                <br />
                <br />

                <div id="dvStrategyDetails">
                    <asp:Repeater ID="rptAllStrategy" runat="server" OnItemDataBound="rptAllStrategy_ItemDataBound">
                        <ItemTemplate>
                            <br/>
                           <div style="page-break-before:always">
                               <br/>
                             <impPlan:improvementPlanControl ID="impPlanCtrl" runat="server" />
                               </div>
                          </ItemTemplate>
                    </asp:Repeater>
                </div>

            </telerik:RadAjaxPanel>
            <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="improvementPlanView" runat="server" Width="100%" />
        </div>





    </form>
</body>
</html>

