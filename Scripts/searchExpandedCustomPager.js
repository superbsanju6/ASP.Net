var currentPage = 1;
var maxPage = 0;
var callInProgress = false;
var currentTry = 0;

function goToPage(pageNumber) {
    if (currentPage == pageNumber) {
        return false;
    }



    if (callInProgress != true) {
        currentPage = pageNumber;
        callSearchService(pageNumber, maxPage);
        currentTry = 0;
    }
    else {
        currentTry++;

        if (currentTry > 20) {
            alert("Timeout error: 10 seconds has elapsed and there is no response from the service.")

            currentTry = 0;
            callInProgress = false;
        }
        else {
            setTimeout("goToPage(" + pageNumber + ");", 500);
        }
        return false;
    }

    

    if ($('.rgCurrentPage').length > 0) {
        $('a').removeClass('rgCurrentPage');
    }

    $('#pageTag_' + pageNumber).addClass("rgCurrentPage");

    //implement scrolling for numberWrapper
    var div = $('#pagingScrollWrapper');
    var adjustedIndex = (pageNumber - 1) > 0 ? (pageNumber - 1) : 1;
    /*    div.scrollTo(((pageNumber - 1) * 50), 800); */
    div.scrollTo($('#pageTag_' + adjustedIndex), 800);
}

function goToPrevPage() {
    if (currentPage == 1) {
        return;
    }

    goToPage(currentPage - 1);
}


function goToNextPage() {
    if (currentPage < maxPage) {
        goToPage(currentPage + 1);
    }
}

function gotoLastPage(obj) {
    goToPage(obj.getAttribute("maxPage"));
}

var currentLoadingPanel = null;
var currentUpdatedControl = null;


function ShowSpinner(targetID) {
    currentLoadingPanel = $find("updPanelLoadingPanel");

    var currentUpdatedControl = targetID;
    if (currentLoadingPanel != null) {
        currentLoadingPanel.show(currentUpdatedControl);
    }
}

function HideSpinner(targetID) {
    var currentLoadingPanel = $find("updPanelLoadingPanel");

    var currentUpdatedControl = targetID;
    //hide the loading panel and clean up the global variables 
    if (currentLoadingPanel != null) {
        currentLoadingPanel.hide(currentUpdatedControl);
    }
}