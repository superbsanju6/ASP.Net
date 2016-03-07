
$('tileScrollDiv1').scrollTo(0);
$('tileScrollDiv2').scrollTo(0);

var currentPageNumber1 = 1;
var currentPageNumber2 = 1;
var objCurrentPage1;
var objCurrentPage2;

setTimeout("highlightDefaultPage()", 100);

function DoubleScrollPanel_JumpToPage(pageNumber, divIndex) {
    if (divIndex == 1) {
        currentPageNumber1 = pageNumber;
        scrollToPageNumber($('#tileScrollDiv1'), currentPageNumber1);
    }
    else {
        currentPageNumber2 = pageNumber;
        scrollToPageNumber($('#tileScrollDiv2'), currentPageNumber2);
    }
}

function setListBoxMaxHeight(sender, args) {
    var listBoxArray = $("[ListBoxIdentifier='" + sender.get_element().getAttribute("lstBoxID") + "']:first");
    var listBox = $find(listBoxArray[0].id);

    var windowHeight = $(window).height();
    var headerToolBarHeight = 50;
    var listBoxHeight = 0;

    var items = listBox.get_items();
    for (var i = 0; i < items.get_count() ; i++) {
        listBoxHeight = listBoxHeight + $telerik.getSize(listBox.getItem(i).get_element()).height;
    }

    if (listBoxHeight + headerToolBarHeight > windowHeight) {
        //subtract 50 for headers and toolbars       
        listBox._groupElement.style.height = (windowHeight - headerToolBarHeight) + "px";
    }
}

function rightArrowClick(divIndex) {
    if (divIndex == 1) {
        if (!document.getElementById('Button1_' + (currentPageNumber1 + 1))) return; //Page does not exist, on last page
        currentPageNumber1++;
        scrollToPageNumber($('#tileScrollDiv1'), currentPageNumber1);
    }
    else {
        if (!document.getElementById('Button2_' + (currentPageNumber2 + 1))) return; //Page does not exist, on last page
        currentPageNumber2++;
        scrollToPageNumber($('#tileScrollDiv2'), currentPageNumber2);
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
        scrollToPageNumber($('#tileScrollDiv1'), currentPageNumber1);
    }
    else {
        if (currentPageNumber2 <= 1) return;
        currentPageNumber2--;
        scrollToPageNumber($('#tileScrollDiv2'), currentPageNumber2);
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

    window.open('/Record/Student.aspx?childPage=yes&xID=' + selectedStudentInput.value);
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

function changeGridHeaderWidth() {
    $('.rgHeaderDiv').each(function () {

        this.style.marginRight = '17px';

    });
}