<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DoubleScrollPanel.ascx.cs"
    Inherits="Thinkgate.Controls.DoubleScrollPanel" %>
<link href="../Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
<script src="<%: ResolveUrl("~/Scripts/jquery-1.9.1.min.js") %>"></script>
<script src="<%: ResolveUrl("~/Scripts/jquery-migrate-1.1.0.min.js") %>"></script>
<script src="<%: ResolveUrl("~/Scripts/jquery.scrollTo.js") %>"></script>
<script src="<%: ResolveUrl("~/Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js") %>"></script>

  <style type="text/css">
      .ui-front {
          z-index:100105;
      }
  </style>      

<script type="text/javascript">

    $('tileScrollDiv1').scrollTo(0);
    $('tileScrollDiv2').scrollTo(0);

    var currentPageNumber1 = 1;
    var currentPageNumber2 = 1;
    var objCurrentPage1;
    var objCurrentPage2;

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(HeaderBar_EndRequestHandler);

    function HeaderBar_EndRequestHandler(sender, args) {
        // ******* 2012-08-29 DHB - Begin code changes.
        // QA176 Page Indicators get messed up when you click a button that causes post back.
        //highlightDefaultPage();
        //if (args.get_error() == undefined && sender._postBackSettings.sourceElement != undefined) {
        if (args.get_error() == undefined && sender._postBackSettings.sourceElement != undefined) {
            //highlightDefaultPage();
            currentPageNumber2 = 1;
            // ******* 2012-08-29 DHB - Finish code changes.
            if (document.getElementById("ReturnToPostBackPage") && document.getElementById("ReturnToPostBackPage").value.length > 0) {
                DoubleScrollPanel_ReturnToPostBackPage(document.getElementById("ReturnToPostBackPage").value);
                document.getElementById("ReturnToPostBackPage").value = "";
            }
        }
    }

    function DoubleScrollPanel_ReturnToPostBackPage(divIndex) {        
        var pageNumber = document.getElementById("tileScrollDiv" + divIndex + "_currentPage").value;
        if (pageNumber.length == 0) return;

        if (divIndex == 1) currentPageNumber1 = pageNumber;
        if (divIndex == 2) currentPageNumber2 = pageNumber;

        scrollToPageNumber($('#tileScrollDiv' + divIndex), pageNumber);
    }

    function DoubleScrollPanel_JumpToPage(pageNumber, divIndex) {
        if (divIndex == 1) {
            currentPageNumber1 = pageNumber;
            scrollToPageNumber($('#tileScrollDiv1'), currentPageNumber1)            
        }
        else {
            currentPageNumber2 = pageNumber;
            scrollToPageNumber($('#tileScrollDiv2'), currentPageNumber2)
        }
    }

    function rightArrowClick(divIndex) {
        if (divIndex == 1) {            
            if (!document.getElementById('Button1_' + (currentPageNumber1 + 1))) return; //Page does not exist, on last page
            currentPageNumber1++;
            scrollToPageNumber($('#tileScrollDiv1'), currentPageNumber1)
        }
        else {       
            if (!document.getElementById('Button2_' + (currentPageNumber2 + 1))) return; //Page does not exist, on last page
            currentPageNumber2++;
            scrollToPageNumber($('#tileScrollDiv2'), currentPageNumber2)
        }
    }

    function scrollToPageNumber(div, pageNumber) {
        savePageNumbersToHidden();
        div.scrollTo(((pageNumber-1) * 994), 800);
    }

    function leftArrowClick(divIndex) {
        if (divIndex == 1) {
            if (currentPageNumber1 <= 1) return;
            currentPageNumber1--;
            scrollToPageNumber($('#tileScrollDiv1'), currentPageNumber1)
        }
        else {
            if (currentPageNumber2 <= 1) return;
            currentPageNumber2--;
            scrollToPageNumber($('#tileScrollDiv2'), currentPageNumber2)
        }
    }

    function highlightDefaultPage() 
    {
        objCurrentPage1 = document.getElementById('Button1_1');
        objCurrentPage2 = document.getElementById('Button2_1');

        if (objCurrentPage1) objCurrentPage1.className = "rotatorPageButtonHighlight";
        if (objCurrentPage2) objCurrentPage2.className = "rotatorPageButtonHighlight";    
    }

    function savePageNumbersToHidden() {
        document.getElementById("tileScrollDiv1_currentPage").value = currentPageNumber1;
        document.getElementById("tileScrollDiv2_currentPage").value = currentPageNumber2;

        //if (objCurrentPage1) objCurrentPage1.removeClass("btnHighlight");
        //if (objCurrentPage2) objCurrentPage2.removeClass("btnHighlight"); 

        if (objCurrentPage1) objCurrentPage1.className = "rotatorPageButton";
        if (objCurrentPage2) objCurrentPage2.className = "rotatorPageButton";

        objCurrentPage1 = document.getElementById('Button1_' + currentPageNumber1);
        objCurrentPage2 = document.getElementById('Button2_' + currentPageNumber2);

        if (objCurrentPage1) objCurrentPage1.className = "rotatorPageButtonHighlight";
        if (objCurrentPage2) objCurrentPage2.className = "rotatorPageButtonHighlight";
        //if (objCurrentPage1) objCurrentPage1.addClass("btnHighlight");
        //if (objCurrentPage2) objCurrentPage2.addClass("btnHighlight"); 
    }

    // ******* 2012-08-29 DHB - Begin code changes.
    // QA176 Page Indicators get messed up when you click a button that causes post back.
    $(function () {
        highlightDefaultPage();
    });
    // ******* 2012-08-29 DHB - Finish code changes.

</script>
<asp:Label ID="container1Label" CssClass="carouselLabel" runat="server"></asp:Label>
<asp:Panel ID="buttonsContainer1" runat="server" ClientIDMode="Static" CssClass="pagingDiv">
</asp:Panel>
<div id="tileDivWrapper1" runat="server" class="tileWrapper">
    <div id="tileScrollDiv1" class="tileScroll">
        <div id="tileDiv1" runat="server" class="tileDiv">
        </div>
    </div>
</div>
<asp:Label ID="container2Label" CssClass="carouselLabel" runat="server"></asp:Label>
<asp:Panel ID="buttonsContainer2" runat="server" ClientIDMode="Static" CssClass="pagingDivTall">
</asp:Panel>
<div id="tileDivWrapper2" runat="server" class="tileWrapper">
    <div id="tileScrollDiv2" class="tileScroll">
        <div id="tileDiv2" runat="server" class="tileDiv">
        </div>
    </div>
</div>

<input clientidmode="Static" type="hidden" runat="server" id="ReturnToPostBackPage"/>
<input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv1_currentPage" value="1" />
<input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv2_currentPage" value="1" />
<input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv1_postBackPage"/>
<input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv2_postBackPage"/>
