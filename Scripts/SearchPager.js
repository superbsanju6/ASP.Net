var currentPage = 1;
var maxPage = 0;
var callInProgress = false;
var currentTry = 0;

function goToPage(pageNumber) {
    if (currentPage == pageNumber) return false;

    currentPage = pageNumber;
    callSearchService(pageNumber);
    goToComplete(pageNumber);
    return true;
}

function goToComplete(pageNumber) {
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

function callSearchService(pagenumber, count) {
    //removeAllResults();  
    if (!count) count = 0;  
    if (ItemArray[pagenumber - 1] == null) {
        if (callInProgress) {
            if (count == 0) ShowSpinner("AjaxPanelResults");
            if (count > 100) {
                HideSpinner();
                alert("Timeout error: 50 seconds has elapsed and there is no response from the service.");
                callInProgress = false;
            }
            setTimeout("callSearchService(" + pagenumber + ", " + (count + 1) + ");", 500);
            return;
        }
        if (count == 0) ShowSpinner("AjaxPanelResults");
        pageSpecificSearch(false, pagenumber);
    }
    else {
        callInProgress = false;
        renderData(ItemArray[pagenumber - 1]);
        HideSpinner();
        
        if (pagenumber < maxPage)
            preFetchNext(pagenumber + 1);
    }
}

function onSearchSuccess(prefetch, results, pagenumber) {
    var parsedResults = jQuery.parseJSON(results);
    ItemArray[pagenumber - 1] = parsedResults;
    callInProgress = false;
    
    if (!prefetch) {
        renderData(parsedResults);
        HideSpinner();
        if (pagenumber < maxPage)
            preFetchNext(pagenumber + 1);

        goToComplete(pagenumber);
    } 
}

function preFetchNext(pagenumber, count) {  
    if (ItemArray[pagenumber - 1] == null) {
        if (callInProgress) {
            if (!count) count = 0;
            if (count > 100) {
                HideSpinner();
                alert("Timeout error: 50 seconds has elapsed and there is no response from the service.");
                callInProgress = false;
            }
            setTimeout("preFetchNext(" + pagenumber + ", " + (count++) + ");", 500);
            return;
        }
        callInProgress = true;
        pageSpecificSearch(true, pagenumber);
    }
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

function goToLastPage(obj) {
    goToPage(obj.getAttribute("maxPage"));
}

var currentLoadingPanel = null;
var currentUpdatedControl = null;


