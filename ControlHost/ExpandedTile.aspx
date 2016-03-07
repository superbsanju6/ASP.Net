<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpandedTile.aspx.cs" Inherits="Thinkgate.ControlHost.ExpandedTile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<%--This height of 100% helps elements fill the page--%>
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <title></title>
    <base target="_self" />
    <telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
    </telerik:RadStyleSheetManager>
    <script type='text/javascript' src='../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../Scripts/jquery-migrate-1.1.0.min.js'></script>
    <script type='text/javascript' src='../Scripts/jquery.scrollTo.js'></script>
    <script type="text/javascript">        var $j = jQuery.noConflict();</script>

    <script type="text/javascript">
        $j('tileScrollDiv1').scrollTo(0);
        $j('tileScrollDiv2').scrollTo(0);

        var currentPageNumber1 = 1;
        var currentPageNumber2 = 1;
        var objCurrentPage1;
        var objCurrentPage2;

        setTimeout("highlightDefaultPage()", 100);

        function DoubleScrollPanel_JumpToPage(pageNumber, divIndex) {
            if (divIndex == 1) {
                currentPageNumber1 = pageNumber;
                scrollToPageNumber($j('#tileScrollDiv1'), currentPageNumber1)
            }
            else {
                currentPageNumber2 = pageNumber;
                scrollToPageNumber($j('#tileScrollDiv2'), currentPageNumber2)
            }
        }

        function rightArrowClick(divIndex) {
            if (divIndex == 1) {
                if (!document.getElementById('Button1_' + (currentPageNumber1 + 1))) return; //Page does not exist, on last page
                currentPageNumber1++;
                scrollToPageNumber($j('#tileScrollDiv1'), currentPageNumber1)
            }
            else {
                if (!document.getElementById('Button2_' + (currentPageNumber2 + 1))) return; //Page does not exist, on last page
                currentPageNumber2++;
                scrollToPageNumber($j('#tileScrollDiv2'), currentPageNumber2)
            }
        }

        function scrollToPageNumber(div, pageNumber) {
            savePageNumbersToHidden();
            div.scrollTo(((pageNumber - 1) * 708), 800);
        }

        function leftArrowClick(divIndex) {
            if (divIndex == 1) {
                if (currentPageNumber1 <= 1) return;
                currentPageNumber1--;
                scrollToPageNumber($j('#tileScrollDiv1'), currentPageNumber1)
            }
            else {
                if (currentPageNumber2 <= 1) return;
                currentPageNumber2--;
                scrollToPageNumber($j('#tileScrollDiv2'), currentPageNumber2)
            }
        }

        function highlightDefaultPage() {
            objCurrentPage1 = document.getElementById('Button1_1');            
            if (objCurrentPage1) objCurrentPage1.className = "rotatorPageButtonHighlight";

        }

        function savePageNumbersToHidden() {
            document.getElementById("tileScrollDiv1_currentPage").value = currentPageNumber1;
            document.getElementById("tileScrollDiv2_currentPage").value = currentPageNumber2;

            if (objCurrentPage1) objCurrentPage1.className = "rotatorPageButton";
            if (objCurrentPage2) objCurrentPage2.className = "rotatorPageButton";

            objCurrentPage1 = document.getElementById('Button1_' + currentPageNumber1);
            objCurrentPage2 = document.getElementById('Button2_' + currentPageNumber2);

            if (objCurrentPage1) objCurrentPage1.className = "rotatorPageButtonHighlight";
            if (objCurrentPage2) objCurrentPage2.className = "rotatorPageButtonHighlight";
        }

        function openStudent(studentID) {
            var selectedStudentInput = document.getElementById('selectedStudentInput');

            var gotoStudentBtn = document.getElementById('gotoStudentBtn');

            if (selectedStudentInput == null || gotoStudentBtn == null)
                return false;

            selectedStudentInput.value = studentID;

            //gotoStudentBtn.click();

            window.open('/Record/Student.aspx?xID=' + selectedStudentInput.value + '&childPage=yes');
        }

        function openConfigurationWindow() {
            radopen(null, "criteriaConfigWindow");
            var oManager = GetRadWindowManager();
            var oWnd = oManager.GetWindowByName("DialogWindow");
            oWnd.Show();


        }

        function clickSearch() {
            var searchBtn = document.getElementById('imgBtnNameSearch');

            if (searchBtn == null)
                return false;

            searchBtn.click();
        }

    </script>
</head>
<body>    
    <form id="form2" runat="server" style="height: 100%;">
    <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    </telerik:RadAjaxManager>
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
    </telerik:RadSkinManager>
    <telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel"
        Width="100%">
        <asp:PlaceHolder runat="server" ID="TilePlaceHolder"></asp:PlaceHolder>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    <input clientidmode="Static" type="hidden" runat="server" id="ReturnToPostBackPage" />
    <input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv1_currentPage"
        value="1" />
    <input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv2_currentPage"
        value="1" />
    <input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv1_postBackPage" />
    <input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv2_postBackPage" />
    </form>
</body>
</html>
