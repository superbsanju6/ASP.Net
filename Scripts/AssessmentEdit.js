/*
General Documentation:
3/6/12 - WSH - I've altered the assessment page to utilize DIV based RadEditor instead of Iframe one because the the blur events don't inheritly work in some browsers with the Iframe approach. Thus wiring up an event to save the text
when you click off would have required some complicated body/window based focus/click event monitoring to determine when the iframe lost focus. The div based approach introduced a bug in Safari in that clicking an editable area 
showed the editor but wouldn't allow an edit. I tracked this back to some sort of conflict with the draggable on the questions. Thus I altered the code to disable the draggable and used a delayed mouse down activation similar to how
it works on the ipad. There have been other changes here and there made to try and solidify the text editor and how it behaves across all browsers.
*/
var suppress_tool_timer = false;
var activeForm = 1;
var postBackState;
var radtab_Forms;
var radtab_FieldTest;
var radtab_SearchOptions;
var radtab_mainTabStrip;
var $lblForm;
var $rtsNextArrow;
var $rtsPrevArrow;
var currentTestID;
var currentEditor;
var undoList;
var isServiceCalled = false;

$(document).ready(function () {
    $lblForm = $("#lblForm");
    ajaxPanel_Items_Loaded();
    ajaxPanel_FullAssessment_Loaded();
    takeFullHeight();
    prepare_other_items();
    //setTimeout(pcd2, 1);

    // This resizes the containing radWindow to properly contain all of its contents.
    // For some reason the contents are clipped on the right and bottom intially.
    setTimeout(function () { changepush(false); }, 1000);
});


function CkEditorReload() {
    if (CKEDITOR.instances) {
        for (name in CKEDITOR.instances) {
            CKEDITOR.instances[name].destroy();
        }
        contentEditableIdArray.length = 0;
        createInitialCkeditor();
    }
    HideSpinner();
}

function ReloadCkeditorForSingleItem(contentEditableIds) {
    if (contentEditableIds) {
        for (var i = 0; i < contentEditableIds.length; i++) {
            CKEDITOR.inline(contentEditableIds[i].id, { toolbar: 'assessmentEditor' });
        }
    }
}

function mainToolBar_OnClientLoad() {
    undoList = new UndoList();
}

function radtab_SearchOptions_OnClientLoad() {
    radtab_SearchOptions = $find("radtab_SearchOptions");
}

function mainTabStrip_OnClientLoad() {
    radtab_mainTabStrip = $find("mainTabStrip");
}

function radtab_FieldTest_OnClientLoad() {
    radtab_FieldTest = $find("radtab_FieldTest");
}

function itemSearch() {
    var TestCategory = document.getElementById('lbl_TestCategory').value;
    var AssessID = document.getElementById('lbl_TestID').value; 
    /*if (getURLParm('dev') == 'y') {
		itemSearchAddList = null;*/
    customDialog({ url: '../Controls/Items/ItemSearch.aspx?ItemSearchMode=MultiSelect&ItemFilterMode=Unfiltered&TestCurrCourseID=' + TestCurrCourseID + '&TestYear=' + TestYear + '&grade=' + grade + '&subject=' + subject + '&coursename=' + coursename + '&TestCategory=' + TestCategory + '&ShowExpiredItems=No&isSecure=' + isSecureAssessment + '&AssessmentType=' + AssessmentType + '&AssessID=' + AssessID, maximize: true, maxwidth: 1000, maxheight: 665, title: 'Item Search', onClosed: ItemSearch_OnClientClose, destroyOnClose: true });
    /*} else {
		alert('Function is in development.');
	}*/

}

function ItemSearch_OnClientClose() {
    var itemsToAdd = [];

    if (itemSearchAddList && itemSearchAddList.length > 0) {
        var item_sort_orders = $("#testQuestions").sortable('toArray');
        for (var j = 0; j < itemSearchAddList.length; j++) {
            itemsToAdd.push({ id: itemSearchAddList[j][0], standardID: itemSearchAddList[j][1] });
            item_sort_orders.push('B' + itemSearchAddList[j][0]);
        }
        ShowSpinner("QuestionHolder");

        addItemsFromBank(itemsToAdd, item_sort_orders, activeForm, 'addBankItemsSearch');
        itemSearchAddList = null;
    }
}

function showSummaryScreen() {
    var hasSecure = hasSecurePermission.toLowerCase() == "true" ? true : false;
    var isSecureflag = isSecureFlag.toLowerCase() == "true" ? true : false;
    var secureAssessment = isSecureAssessment.toLowerCase() == "true" ? true : false;
    var dialogTitle = 'Assessment Summary';
    if (hasSecure == true && isSecureflag == true) {
        if (secureAssessment == true) {
            dialogTitle=SetSecureAssessmentTitle(dialogTitle);
        }
    }
    customDialog({ url: '../Dialogues/Assessment/AssessmentSummary.aspx?xID=' + getURLParm('xID') + "&yID=" + encryptedUserID, maximize: true, maxwidth: 800, maxheight: 700, title: dialogTitle });
}

function loadAssessmentItems(item_array) {
    $.views.helpers({
        format_dec: function (val, percision) {
            return val.toFixed(percision);
        },
        VerifyIBEditAccess: function (val, SharedContentID) {
            if (CanUserUseItemBank(val, SharedContentID, IBEditArray)) {
                return true;
            }
            return false;
        },
        VerifyIBCopyAccess: function (val, SharedContentID) {
            if (CanUserUseItemBank(val, SharedContentID, IBCopyArray)) {
                return true;
            }
            return false;
        }
    });

    $.views.tags({
        _dok: function () {
            return dok;
        }
    });

    $.templates("questionTmpl", {
        markup: "#questionTemplate",
        allowCode: true
    });

    if (!item_array) item_array = AssessmentItems;
    return $.render.questionTmpl(item_array);

    //$itemRepeater.html($("#questionTemplate").render(AssessmentItems));
}

function expandItem(item) {
    var itemID = $(item).closest(".questionInstance").attr("id");
    for (var j = 0; j < AssessmentItems.length; j++) {
        if (AssessmentItems[j].ID == itemID) {
            window.open('TestQuestionPage.aspx?xID=' + AssessmentItems[j].EncryptedID);
            return;
        }
    }
}

function advancedItemEdit(item) {
    if (typeof currentEditableBody !== "undefined" && typeof currentEditor !== "undefined") {

        assessmentitem_changeField(currentEditableBody, 'Item_Text', currentEditor.getData());
    }

    var itemID = $(item).closest(".questionInstance").attr("id");
    var TestCategory = document.getElementById('lbl_TestCategory').value;
    var TestType = document.getElementById('lbl_TestType').value;
    customDialog({ url: '../Controls/Assessment/ContentEditor/ContentEditor_Item.aspx?AssessmentID=' + encodeURIComponent(getURLParm('xID')) + '&xID=' + itemID + '&qType=TestQuestion' + '&TestCategory=' + TestCategory + '&TestType=' + TestType, maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorITEM', title: 'Content Editor - ITEM', onClosed: ItemEditor_OnClientClose, destroyOnClose: true });
}

function advancedItemEditExternal(item) {
    var itemID = $(item).closest(".questionInstance").attr("id");
    var TestCategory = 'Classroom'; //document.getElementById('lbl_TestCategory').value;
    customDialog({ url: '../Controls/Assessment/ContentEditor/ContentEditor_Item_External.aspx?AssessmentID=' + getURLParm('xID') + '&xID=' + itemID + '&qType=TestQuestion' + '&TestCategory=' + TestCategory, maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorITEM', title: 'Content Editor - EXTERNAL ITEM', onClosed: ItemEditor_OnClientClose, destroyOnClose: true });
}

function itemOnlinePreview(item) {
    var itemID = $(item).closest(".questionInstance").attr("id");
    customDialog({
        name: 'OnlinePreview',
        maximize: true,
        resizable: true,
        movable: true,
        title: 'Online Preview',
        url: document.getElementById('lbl_OTCUrl').value + "?assessmentID=1" + document.getElementById('lbl_TestID').value + "&studentID=0&itemID=" + itemID
        //url: SessionBridge + escape('display.asp?formatoption=search results&key=9120&retrievemode=searchpage&ahaveSkipped=normal&??Question=' + itemID + '&??TestID=-1&qreview=y')
    });
}

function assessmentOnlinePreview() {
    //To put OTC in preview mode, the test ID must have a 1 in front and the student ID must = 0
    customDialog({
        name: 'OnlinePreview',
        maximize: true,
        resizable: true,
        movable: true,
        title: 'Online Preview',
        url: document.getElementById('lbl_OTCUrl').value + "?assessmentID=1" + document.getElementById('lbl_TestID').value + "&studentID=0"
    });
}

function prepare_inline_editor_areas() {
    if (AssessmentIsProofed) return;
    firstEdit = true;
    $currEditableBody = null;
    $editorToolHolder = $('#editorToolHolder');
    $('.reToolbar').css('position', 'static').css('top', '0px');        // I have toolbar appearing above the top of the page to hide it on load. Move it back now
    var $headingArea = $(".headingArea");                               // IE 8 is stupid and doesn't show the top of the headingArea, only the bottom. You can't see the dropdown arrow, etc. Changing the height and back again, 'fixes' it.
    var height = $headingArea.css("height");
    $headingArea.css("height", "26px");
    $headingArea.css("height", height);


    $editorToolHolder.hide();
    $('body').append($editorToolHolder);

    question_instance_list = $('.questionInstance, .questionInstance_highlight');
    question_instance_list.mousedown(highlight_question);
}

var question_instance_list;

function highlight_item(item) {
    questionInstance = item.closest(".questionInstance");
    question_instance_list.removeClass("questionInstance_highlight");
    questionInstance.addClass("questionInstance_highlight");
}

function highlight_question(event) {
    var item = $(this);
    highlight_item(item);
}

var question_sort_in_progress = false;
var $bankSliderImg;
var AssessmentIsProofed = false;

var _drag_delay_toggle = false;
var sortable_is_receiving = false;
function prepare_sortable_lists() {    
    if (AssessmentIsProofed) return;
    $("#testQuestions").sortable({
        cancel: ".SKEditableBodyText",
        scroll: false,
        revert: true,
        placeholder: 'questionPlaceholder',
        receive: function (event, ui) {
            sortable_is_receiving = true;
            bankToTestDrag(event, ui);
            $('body').css('cursor', 'default'); // for some reason, the draggable move cursor ends up getting transfered to the body tag when you release it
        },
        start: function (event, ui) {
            question_sort_in_progress = true;
            top_constraint_for_drag_helper = $('#topPortion').height() + 60;
            bottom_constraint_for_drag_helper = $(window).height() - 40;
            $("body").bind('mousemove', dragHelper);
            $("#testQuestions").disableSelection();     
          
        },
        stop: function (event, ui) {
            $("body").unbind('mousemove', dragHelper);
            question_sort_in_progress = false;
            if (sortable_is_receiving) {
                sortable_is_receiving = false;
            } else {
                onItemMoved();
            }
            _drag_delay_toggle = false;
            $(this).sortable('disable');
            scrollHelper_Cancel();
        },
        cursor: 'move'
    });

    $("#testQuestions").sortable('disable');
    

    $(".questionInstance").bind("touchstart mousedown", function (e) {
        debug_message("mouse down");
        if (_drag_delay_toggle == false) {
            debug_message("step2");
            var _this = $(this);
            var _e = e;
            this.downTimer = setTimeout(function () {
                debug_message("enable sortable");
                $("#testQuestions").sortable('enable');
              
                if (isTouchDevice()) {
                    _this.effect("shake", { times: 3, distance: 8 }, 50);
                } else {
                    _this.addClass("questionInstance_move").removeClass("questionInstance_highlight");
                }
                _drag_delay_toggle = true;
                _e.originalEvent = {}; //fix for ie < 9
                //var ev = new $.Event('stack.overflow');
                //ev.originalEvent = e;
                //$(document).trigger(ev); 
                _this.trigger(_e);


            }, (isTouchDevice() ? 500 : 500));
        }
    }).bind("touchend mouseup", function (e) {
        debug_message("mouseup");

        clearTimeout(this.downTimer);
        _drag_delay_toggle = false;
        $(this).removeClass("questionInstance_move").addClass("questionInstance_highlight");
    }).bind("touchmove mousemove", function (e) {

        if (isTouchDevice()) {
            clearTimeout(this.downTimer);
            _drag_delay_toggle = false;
        }
    });

}

function dragHelper(event) {
    //$('.headingToggleImg').html(event.pageY + ':' + bottom_constraint_for_drag_helper);

    if (event.pageY < top_constraint_for_drag_helper) scollUpHelper_Start();
    else if (event.pageY > bottom_constraint_for_drag_helper) scollDownHelper_Start();
    else scrollHelper_Cancel();
}
/*function QuestionHolder_OnScroll: when dragging from bank on right, tendancy to have it scroll the question area to the right. Thus, while dragging from bank, I reset back to 0 scrollLeft */
/*function QuestionHolder_OnScroll() {
// $(this).scrollLeft(0);
//this.scrollLeft = 0;
}*/

//function makeEditorSafeBeforeReload() {
//	$('body').append($editorToolHolder);
//	$('body').append($('.SKEditorWrapper')); // move the editor so it doesn't get cleared during ajax refresh
//}

function bankToTestDrag(event, ui) {      
        ShowSpinner("form2");

        var jsonObj = [];

        var dragged_item_id = $(ui.item).attr("id");
        var targetBank = $(ui.item).attr('targetBank');
        var simple_items_to_add = [];
        jsonObj.push({ id: dragged_item_id, standardID: $(ui.item).attr("StandardID") });
        simple_items_to_add.push(dragged_item_id);
        //items_to_add.push([dragged_item_id, $(ui.item).attr("StandardID")]);
        $(targetBank + ' .thumbInstance :checked').each(function (index) {
            var $thumb = $(this).closest('.thumbInstance');
            var item_id = $thumb.attr('id');
            if (item_id && item_id != dragged_item_id) {
                jsonObj.push({ id: item_id, standardID: $thumb.attr('StandardID') });
                simple_items_to_add.push(item_id);
                //items_to_add.push([item_id, $thumb.attributes('StandardID')]);
            }

            this.checked = false;
        });

        var item_sort_orders = $("#testQuestions").sortable('toArray');
        //item_sort_orders.splice(ui.item.index(), 1, items_to_add);
        item_sort_orders.splice(item_sort_orders.indexOf(''), 1, simple_items_to_add);

        //document.getElementById('txt_AssessmentForms').value = JSON.stringify(AssessmentForms);
        addItemsFromBank(jsonObj, item_sort_orders, activeForm, 'addBankItemsDrag');   
}

function addItemsFromBank(itemsToAdd, sortOrder, formId, action) {
    
    postBackState = { action: action, scrollTop: $('#QuestionHolder').scrollTop() };
    document.getElementById('txt_QuestionIDsToAdd').value = JSON.stringify(itemsToAdd).replace(/B/gi, ''); //items_to_add.join('],[').replace(/B/gi, '');
    document.getElementById('txt_QuestionSort').value = sortOrder;
    document.getElementById('txt_ActiveForm').value = formId;

    document.getElementById('btn_bankToTestDrag').click();   
}

function assessment_directions_save(e) {
    assessment_changeField("Directions", tel_RadEditorAssessmentDirections.get_html());
}

function previewBankQuestion(id) {
    customDialog({ url: '../ControlHost/PreviewTestQuestion.aspx?type=BankQuestion&xID=' + id, title: 'Item Preview', maximize: true, maxwidth: 400, maxheight: 350 });
}

function AddBlankItems(event) {
    var numItems = parseInt(document.getElementById('txtAddNumItems').value);
    if (numItems > 100) {
        event.stopPropagation();
        customDialog({ title: 'Add Blank Items', maximize: true, maxwidth: 240, maxheight: 75, content: "<br>Please specify 100 questions or less.</br>" });
        document.getElementById('txtAddNumItems').value = "";
    }
    else {
        if (numItems > 0) ShowSpinner("QuestionHolder");

            postBackState = { action: 'addBlankItem' };
            event.stopPropagation();
            document.getElementById('txt_ActiveForm').value = activeForm;
            document.getElementById('btn_AddBlankItem').click();
        
    }
}

function AddNewForm(event) {
    if (parseInt(document.getElementById('formCount').value) >= 99) {
        var winContent = '<br>The maximum number of allowable assessment forms is 99.</br>';
        customDialog({ title: 'Add New Form', maximize: true, maxwidth: 360, maxheight: 75, content: winContent });
        return false;
    }

    ShowSpinner("form2");
    event.stopPropagation();
    //ShowSpinner();
    radtab_Forms = null;
    postBackState = { action: 'addingForm' };

    //document.getElementById('txt_ActiveForm').value = activeForm;
    document.getElementById('btn_AddNewForm').click();
}

/*var newFormID = 0;
function RefreshFormTabs(newFormCount) {
document.getElementById('txt_Subject').value = 'Mathematics';
document.getElementById('txt_FormCount').value = newFormCount;
newFormID = newFormCount;
radtab_Forms = null;
document.getElementById('btn_RefreshFormTab').click();
}*/

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode > 31
      && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function RefreshFormTabs_callback() {
    /*if (newFormID == 0) {
	adjustScrollingTabWidth();
	return;
	}*/
    //$("#radtab_Forms").append($(".rtsNextArrow")).append($(".rtsPrevArrowDisabled"));
    if (!radtab_Forms) radtab_Forms = $find("radtab_Forms");
    //if (!count) count = 0;
    /*if (radtab_Forms.get_tabs().get_count() <= (newFormID - 1)) {
	if (count > 9) return; //give up
	setTimeout("RefreshFormTabs_callback(" + parseInt(count + 1) + ")", 100);
	return;
	}*/
    // move the x marks behind the text in the tabs
    $(".rtsImg").each(function (index, Element) {
        $this = $(this);
        $this.parent().append(this);
        $this.click(DeleteForm);
        $this.attr('formId', index + 1);
        $this.attr('title', 'Remove Form');
    });

    //adjustScrollingTabWidth();

    if (postBackState && postBackState.action == 'addingForm') {
        postBackState = null;
        var tabs = radtab_Forms.get_tabs();
        var tab = tabs.getTab(tabs.get_count() - 1);
        scrollToTab(tabs.get_count() - 1);
        tab.select();
    } else if (postBackState && postBackState.action == 'deletingForm' && postBackState.idx) {
        var idx = postBackState.idx;
        postBackState = null;
        var tabs = radtab_Forms.get_tabs();
        if (tabs.get_count() < idx) idx = tabs.get_count();
        var tab = tabs.getTab(idx - 1);
        scrollToTab(idx);
        tab.select();
        formSelected(idx);
    }
    HideSpinner();
    //ResponseEnd();
}

function DeleteForm(evt) {
    ShowSpinner("form2");
    var formId = $(evt.target).attr('formId');
    document.getElementById('txt_TargetForm').value = formId;
    postBackState = { action: 'deletingForm', idx: formId };
    radtab_Forms = null;
    document.getElementById('btn_DeleteForm').click();
}

function scrollToTab(tab_num) {
    try {
        while (radtab_Forms._scroller.get_currentPosition() > 0) {
            radtab_Forms._scroller._scrollBackward();
        }
        for (var j = 1; j < tab_num; j++) {
            radtab_Forms._scroller._scrollForward();
            if (radtab_Forms._scroller.get_currentPosition() >= radtab_Forms._scroller._maxPosition) return;
        }
    } catch (e) {
    }
}

var currentLoadingPanel = null;
var currentUpdatedControl = null;
function ShowSpinner(target) {

    currentLoadingPanel = $find("updPanelLoadingPanel");

    currentUpdatedControl = target ? target : "form2";
    //show the loading panel over the updated control 
    currentLoadingPanel.show(currentUpdatedControl);
}

function HideSpinner() {
    //hide the loading panel and clean up the global variables 
    if (currentLoadingPanel != null) {
        currentLoadingPanel.hide(currentUpdatedControl);
    }
    currentUpdatedControl = null;
    currentLoadingPanel = null;
}

function ajaxPanel_Bank_Loaded() {
    $("#bankQuestions").html($("#bankItemTemplate").render(Bank_items));

    prepare_bank_drag('#bankQuestions');
    updateBankFlags();
    HideSpinner();
}

function ajaxPanel_Bank_filter_Loaded() {
    $("#bankQuestions_filter").html($("#bankItemTemplate").render(Bank_filter_items));

    prepare_bank_drag('#bankQuestions_filter');
    updateBankFlags();

    HideSpinner();
}

function ajaxPanel_Bank_keyword_Loaded() {
    $("#bankQuestions_keyword").html($("#bankItemTemplate").render(Bank_keyword_items));

    prepare_bank_drag('#bankQuestions_keyword');
    updateBankFlags();

    HideSpinner();
}

function ajaxPanel_FullAssessment_Loaded() {
    $("#imgAddNewItem").click(AddBlankItems);
    $("#imgAddNewForm").click(AddNewForm);
}

function renderItems(item_array) {
    var $testQuestions = $("#testQuestions");
    //makeEditorSafeBeforeReload();
    $testQuestions.html(loadAssessmentItems(item_array));
    RenumberQuestions();

    prepare_sortable_lists();
    post_item_reload();
    prepare_inline_editor_areas();
}

function ajaxPanel_Items_Loaded() {
    //renderItems();
    flag_a_change_to_pdf();
    renderForm(activeForm);
    updateBankFlags();
    HideSpinner();
}

function prepare_other_items() {
    $('.headingToggle').click(headingToggle);
    //setTimeout("undoList = new UndoList();", 400);

    if (AssessmentIsProofed) return;
    $('#bankSlider').click(bankSliderGo);
    $bankSliderImg = $('#bankSliderImg');

    $('#scrollDownHelper').bind('mouseover', scollDownHelper_Start);
    $('#scrollDownHelper').bind('mouseout', scrollHelper_Cancel);
    $('#scrollUpHelper').bind('mouseover', scollUpHelper_Start);
    $('#scrollUpHelper').bind('mouseout', scrollHelper_Cancel);


    $('#rightHandToolbar .rightHandTabButton').click(rightHandToolbar_tabclick);

    /*  $('#OutlineStandardTree').mouseover(function () {
          mouse_is_inside_standardoutline = true;
      }).mouseout(function () {
          mouse_is_inside_standardoutline = false;
      });*/
    $('.reToolbar.Transparent').css('position', 'relative').css('left', '-9px').css('top', '10px');
}

function thumbnailStandardSelect(plusIcon) {
    var $thumb = $(plusIcon).closest('.thumbInstance');
    var item_id = $thumb.attr('id');
    var standard_id = $thumb.attr('StandardID');
    for (var j = 0 ; j < Bank_keyword_items.length; j++) {
        if (Bank_keyword_items[j].ID == item_id.replace("B", "")) {
            var content = "<br/><I>Select the preferred standard to use with the specified item:</I><br/><div class='standard_select' style='height: 100px;'>";
            for (var i = 0; i < Bank_keyword_items[j].Standards.length; i++) {
                content += "<div style='cursor: pointer' title='" + Bank_keyword_items[j].Standards[i].Desc + "' onclick='$(this).children()[0].checked = true;' onmouseover='this.style.backgroundColor = \"#EDD8AF\";' onmouseout='this.style.backgroundColor = \"transparent\";'><input type='radio' " + ((standard_id == Bank_keyword_items[j].Standards[i].ID) ? " checked " : "") + " name='standard_select' value='" + Bank_keyword_items[j].Standards[i].ID + "' text='" + Bank_keyword_items[j].Standards[i].StandardName + "'/>" + Bank_keyword_items[j].Standards[i].StandardName + "</div>";
            }

            content += '</div>';
            customDialog({ title: "Select Standard for Item", maximize: true, maxwidth: 350, maxheight: 200, animation: "none", content: content }, [{ title: "Cancel" }, { title: "Update Item", callback: thumbnailStandardSelect_callback, argArray: [plusIcon, $thumb] }]);
            return;
        }
    }
    alert("Error opening standard list.");
}

function thumbnailStandardSelect_callback(plusIcon, $thumb) {
    for (var j = 0; j < document.forms[0]["standard_select"].length; j++) {
        if (document.forms[0]["standard_select"][j].checked) {
            var newStandardID = document.forms[0]["standard_select"][j].value;
            var newStandardText = document.forms[0]["standard_select"][j].getAttribute('text');
            $(plusIcon.parentNode).children().first().html(newStandardText);
            $thumb.attr('StandardID', newStandardID);
        }
    }
}

function headingToggle(event, ui) {
    var $headingToggle = $(this);
    var current_state = $headingToggle.attr('state');
    if (!current_state) {
        current_state = 'closed';
        $headingToggle.attr('state', current_state);
    }
    var targetHeight;
    if (current_state == 'closed') {
        targetHeight = (AssessmentIsProofed ? 200 : 200);
        current_state = 'opened';
        $('.headingToggleImg').removeClass('headingToggleImgOff');
    } else {
        targetHeight = 26;
        current_state = 'closed';
        $('.headingToggleImg').addClass('headingToggleImgOff');
    }
    $headingToggle.attr('state', current_state);

    height_in_use = $(window).height() - $('#topPortion').height();

    $('.headingArea').animate({ height: targetHeight }, {
        duration: 500,
        step: function (now, fx) {
            $('#QuestionHolder').height(height_in_use - now);
        }
    });

}

var height_in_use;
var scrolling_questions = false;
var scrolling_item;
function scollDownHelper_Start(event) {
    //$('.headingToggle').html(count++);
    scrolling_item = $('#QuestionHolder');
    scrolling_questions = true;
    setTimeout("scrollHelper(10)", 20);
}

function scollUpHelper_Start(event) {
    //$('.headingToggle').html(count++);
    scrolling_item = $('#QuestionHolder');
    scrolling_questions = true;
    setTimeout("scrollHelper(-10)", 20);
}

function scrollHelper(offset) {
    if (!scrolling_questions) return;
    scrolling_item.scrollTop(scrolling_item.scrollTop() + offset);
    setTimeout("scrollHelper(" + offset + ")", 20);
}

function scrollHelper_Cancel(event) {
    scrolling_questions = false;
}

function bankSliderGo(event, ui) {
    var $bank_holder = $('#bankHolder');
    var current_state = $bank_holder.attr('state');
    if (!current_state) {
        current_state = 'opened';
        $bank_holder.attr('state', current_state);
    }
    var original_width = $('#QuestionHolder').width() + $bank_holder.width() + $("#rightHandToolbar").width();
    SuppressResizeEvent = true;

    if (current_state == 'opened') {
        $('.bankScroller').css('overflow', 'hidden');
        $('#rightHandToolbar').stop().animate({ width: 25 }, 500, function () { });
        $bank_holder.stop().animate({ width: 1 }, {
            duration: 500,
            complete: function () { bankSliderDone($(this)); },
            step: function (now, fx) {
                $('#QuestionHolder').width(original_width - now - 25);
            }
        });      // find that going doesn't work properly in Chrome and Safari. It's like the calculation goes haywire and the div jumps back to the full 240px wide instead of disappearing
        current_state = 'closed';
    } else {
        $('#rightHandToolbar').stop().animate({ width: 1 }, 500, function () { });
        $bank_holder.show();
        $bank_holder.stop().animate({ width: 245 }, {
            duration: 500,
            complete: function () { bankSliderDone($(this)); },
            step: function (now, fx) {
                $('#QuestionHolder').width(original_width - now - 1);
            }
        });
        current_state = 'opened';
    }
    $bank_holder.attr('state', current_state);
}

function bankSliderDone($bank_holder) {
    setTimeout(function () { SuppressResizeEvent = false; }, 500);
    var current_state = $bank_holder.attr('state');
    var new_src = $bankSliderImg.attr('src');
    if (current_state == 'closed') {
        $bank_holder.hide();
        new_src = new_src.substring(0, new_src.lastIndexOf('/')) + '/' + $bankSliderImg.attr('closed_src');
        //$('#rightHandToolbar').show();
    } else {
        $bank_holder.show();
        new_src = new_src.substring(0, new_src.lastIndexOf('/')) + '/' + $bankSliderImg.attr('opened_src');
        //$('#rightHandToolbar').hide();
        $('.bankScroller').css('overflow', 'auto');

    }
    $bankSliderImg.attr('src', new_src);
}

function rightHandToolbar_tabclick(event, ui) {
    var tab_name = $(event.target).attr('tab_name');
    selectFilterTab(tab_name);
    $('#bankSlider').trigger(event);
}


function getURLParm(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return results[1];
}

/* Right now this just updates the count of the items and I'm calling it from an add and delete */
function post_item_reload() {
    /*
        $('#lblItemCount').html($("#testQuestions").children().length);
    */
    $('#lblItemCount').html($("#testQuestions").find(".questionInstance").length);

}

function mainToolBar_buttonVisibilityChange(mode) {
    var toolBar = $find("mainToolBar");
    var items = toolBar.get_items();
    var vis_def = eval("mainToolBar_buttonVisibility." + mode);
    for (var i = 0; i < items.get_count() ; i++) {
        if (vis_def.indexOf(items.getItem(i).get_value()) >= 0)
            items.getItem(i).show();
        else
            items.getItem(i).hide();
    }
}

function showEditButtonsOnToolbar() {
    var toolBar = $find("mainToolBar");
    var items = toolBar.get_items();
    for (var i = 0; i < items.get_count() ; i++) {
        items.getItem(i).show();
    }
}

var SuppressResizeEventOnce = false;
var SuppressResizeEvent = false;
$(window).resize(function () {
    //wheight = $(window).height();
    // noscroll();
    if (SuppressResizeEvent) return false;
    if (SuppressResizeEventOnce) {
        SuppressResizeEventOnce = false;
        return false;
    }
    changepush(true);
});

function takeFullHeight() {
    changepush(true);
};

function changepush(suppressResizeEventOnce) {
    SuppressResizeEventOnce = suppressResizeEventOnce;
    var wheight = $(window).height();
    var wwidth = $(window).width();
    //alert(wheight);
    //alert(wheight);
    var contentHeight = wheight - $('#topPortion').height();
    $('#editContentContainer').height(contentHeight);
    $('#QuestionHolder').height(contentHeight - $('.headingArea').height());
    $('#QuestionHolder').width(wwidth - $("#bankColumn").width() - $("#bankSlider").width());  // I hate that I have to do this. setting to 100% and letting browser figure out worked everyone except when opening editor on an answer on ipad. Could not prevent ipad from shrinking the bank area down as this area expanded a bit

    $('#bankScroller').height(contentHeight - $('#bankHeadingArea').height() - $('#radtab_SearchOptions').height());
    $('#bankScroller_filter').height(contentHeight - $('#bankHeadingArea_filter').height() - $('#radtab_SearchOptions').height());
    $('#bankScroller_keyword').height(contentHeight - $('#bankHeadingArea_keyword').height() - $('#radtab_SearchOptions').height());

    $('#mpPreview').height(contentHeight);

}

function getDialogRadWindow(name) {
    if (!name) name = 'DialogRadWindow';
    if ($find(name)) return $find(name);
}

function deleteAssessment_confirmCallback() {
    Service2.DeleteAssessment(getURLParm('xID'), deleteAssessment_onSuccess, deleteAssessment_onFailure);
}

function deleteAssessment_onSuccess(result_raw) {
    var result = jQuery.parseJSON(result_raw);
    if (result.StatusCode > 0) {
        customDialog({ title: "Delete Assessment", maximize: true, maxwidth: 500, maxheight: 100, animation: "none", dialog_style: "alert", content: result.Message }, [{ title: "Ok" }]);
        return;
    }
    if (parent.location.href.indexOf('AssessmentObjects.aspx') > -1) {
        parent.window.close();
    }
    closeCurrentCustomDialog();
    //parent.closeCustomDialog('RadWindow1Url');
    //setTimeout(function() {

    //}, 1);
}

function deleteAssessment_onFailure() {
    alert('Error deleting assessment');
}

function getResponse(dataObj, responseID) {
    for (var j = 0; j < dataObj.Responses.length; j++) {
        if (dataObj.Responses[j].ID == responseID) {
            return dataObj.Responses[j];
        }
    }
    return null;
}

function updateJSONcopy(itemID, field, value, responseID, additional_value) {

    var dataObj = ItemIndex[itemID];
    if (!dataObj) {
        customDialog({ title: "Update Error", maximize: true, maxwidth: 500, maxheight: 100, autoSize: true, animation: "Fade", dialog_style: "alert", content: "Error updating JSON for item " + itemID }, [{ title: "Ok" }]);
        return;
    }
    if (field == "Item_Text") {
        if (!responseID) {
            field = "Question_Text";
        } else {
            for (var j = 0; j < dataObj.Responses.length; j++) {
                if (dataObj.Responses[j].ID == responseID) {
                    dataObj.Responses[j].DistractorText = value;
                    break;
                }
            }
            return;
        }
    } else if (field == "Correct") {
        for (var j = 0; j < dataObj.Responses.length; j++) {
            dataObj.Responses[j].Correct = (dataObj.Responses[j].ID == value);
        }
        return;
    } else if (field == "DifficultyIndex" || field == "ItemWeight" || field == "Rigor") {
        dataObj[field].Text = value;
        return;
    } else if (field == "StandardID") {
        dataObj["StandardName"] = additional_value;
    }

    dataObj[field] = value;
}

function assessment_changeField(field, value, callback_function, callback_arguments) {
    isServiceCalled = false; /*so each time this function is entered, it will be  true only after service call is returned.*/
    Service2.AssessmentUpdateField(getURLParm('xID'), field, value,
		function (result_raw) {
		    isServiceCalled = true; /*once service call is returned set to true so that call to close window  in InlineEditor.js does not hang.*/
		    result = jQuery.parseJSON(result_raw);
		    if (result.StatusCode > 0) {
		        assessment_changeField_event_onFailure(null, result.Message);
		        return;
		    }
		    flag_a_change_to_pdf();
		    if (result.ExecOnReturn) {
		        eval(result.ExecOnReturn);
		    }
		    if (callback_function)
		        if (typeof callback_arguments == 'undefined' || !callback_arguments)
		            callback_function.apply(this);
		        else
		            callback_function.apply(this, callback_arguments);
		}, assessment_changeField_event_onFailure);
}

function assessmentitem_changeField(targetElement, field, value, callback_function, callback_arguments, additional_value)
{
  
    var $targetElement = $(targetElement);
    var $qi = $targetElement.closest(".questionInstance");
    var itemID = $qi.attr("id");
    var upd = $targetElement.attr("update");
    var responseID;
    if (upd) {
        // If this is a rich text edit, it will have an update attribute that will indicate whether it's Question, Answer or Question Directions
        responseID = upd.substring(0, 2) == "A_" ? upd.substring(2) : "";
        if (upd.substring(0, 1) == "D") field = "Directions";
    }
    //PLH 10/15/2013 - If the standardID comes over as negative then it must be coming from a Standard Filter, so get the absolute value to get the real ID. Very hacky
    if (field == 'StandardID') {
        value = Math.abs(value);
    }
    if (field == 'DisplayRubric') {
        value = $targetElement.is(':checked') ? "1" : "0";
    }
    if (itemID != undefined || itemID!=null) {
        updateJSONcopy(itemID, field, value, responseID, additional_value);
    }
    textOnClose = value;
    isServiceCalled = false;  /*so each time this function is entered, it will be  true only after service call is returned.*/
    Service2.AssessmentItemUpdateField(getURLParm('xID'), itemID, responseID, field, value,
		function (result_raw) {
		    isServiceCalled = true; /*once service call is returned set to true so that call to close window  in InlineEditor.js does  not hang.*/
		    result = jQuery.parseJSON(result_raw);
		    if (result.StatusCode > 0) {
		        assessment_changeField_event_onFailure(null, result.Message);
		        return;
		    }
		    flag_a_change_to_pdf();
		    if (result.ExecOnReturn) {
		        eval(result.ExecOnReturn);
		    }
		    if (callback_function)
		        if (typeof callback_arguments == 'undefined' || !callback_arguments)
		            callback_function.apply(this);
		        else
		            callback_function.apply(this, callback_arguments);
		}, assessment_changeField_event_onFailure);
 
    var prevUpdateAction = $qi.attr("PrevUpdateAction");
    var sharedContentID = $qi.attr("SharedContentID");
    var isCopyAccess = $qi.attr("IsCopyAccess");
    var isEditAccess = $qi.attr("IsEditAccess");

    if (field == 'Correct') {
        itemChange_showConfirm(itemID);
    }
    else if (upd != undefined) {
        if (upd.substring(0, 2) == "A_" || upd == "Q")
            itemChange_showConfirm(itemID);
    }
}

function AddStandardToItemFromSearch(itemID, value) {
    Service2.AssessmentItemUpdateField(getURLParm('xID'), itemID, 0, "StandardID", value);
    document.getElementById('btn_RefreshQuestionsPanel').click();
}

var editedItemsList = '';
function itemChange_showConfirm(itemID) {
    var $qi = $("#" + itemID).closest(".questionInstance");
    var sharedContentID = $qi.attr("SharedContentID");
    var isCopyAccess = $qi.attr("IsCopyAccess");
    var isEditAccess = $qi.attr("IsEditAccess");
    var preUpdateAction = '';

    // Get the first action taken on specific item
    if (editedItemsList.indexOf(itemID) >= 0) {
        editedItemsArray = editedItemsList.split(',');

        for (var i = 0; i < editedItemsArray.length - 1; i++) {
            if (editedItemsArray[i].split(':')[0] == itemID)
                preUpdateAction = editedItemsArray[i].split(':')[1];
        }
    }

    if (sharedContentID == 0)
        return;                         // Return, if edited item is not belongs to the Item Bank
    else if (preUpdateAction == '') {   // Check, if item is edited before, in the same edit session
        if (isEditAccess == "true" && (TestCategory == "District" || TestCategory == "Classroom"))
            customDialog({ title: "Item Change", center: true, closeMode: false, maximize: true, maxwidth: 400, maxheight: 200, content: $("#itemChange_dialog").html() }, [{ title: "New", callback: itemChange_confirmCallback, argArray: [itemID, 'New'] }, { title: "Correction", callback: itemChange_confirmCallback, argArray: [itemID, 'CorrectionAsInsert'] }]);
        else if (isCopyAccess == "true")
            itemChange_confirmCallback(itemID, "New");
    }
    else if (preUpdateAction == 'Correction') {
        itemChange_confirmCallback(itemID, "CorrectionAsUpdate");
    }
}

function itemChange_confirmCallback(itemID, updateAction) {
    var $SortImg = $("#sortorder_" + itemID).closest(".SortImg");
    var $qi = $("#" + itemID).closest(".questionInstance");
    var sharedContentID = $qi.attr("SharedContentID");
    
    // Preserve the first action taken on specific item
    if (editedItemsList.indexOf(itemID) < 0) {
        if (updateAction == "New")
            editedItemsList += itemID + ":New,";
        else
            editedItemsList += itemID + ":Correction,";
    }

    // Save changes into database
    Service2.AssessmentItemUpdateAction(itemID, updateAction,
        function (result_raw) {
            result = jQuery.parseJSON(result_raw);
            if (result.StatusCode > 0) {
                assessment_changeField_event_onFailure(null, result.Message);
                return;
            }

            if (updateAction == "New") {
                $SortImg.hide();
            }

            if (updateAction != "CorrectionAsUpdate") {
                // Update Item Bank Assessment items, because SharedContentID is changed in DB
                UpdateIBAssessmentArray(sharedContentID, result.PayLoad.SharedContentID);

                // Update specific item, because SharedContentID is changed in DB
                //UpdateSingleItem(result);
            }

        }, assessment_changeField_event_onFailure);

    // Hide modal popup, control goes to container.
    $(".TelerikModalOverlay").hide();
}

function UpdateIBAssessmentArray(oldSharedContentID, newSharedContentID) {
    if (IBAssessmentArray) {
        var iba = IBAssessmentArray[0];

        for (var i = 0; i < iba.length; i++) {
            if (iba[i].Id == oldSharedContentID) {
                IBAssessmentArray[0][i].Id = newSharedContentID;
                break;
            }
        }
    }
}

function assessment_changeField_event_onFailure(result_raw, errorMessage) {
    if (result_raw) errorMessage = jQuery.parseJSON(result_raw).ErrorMessage;
    customDialog({ title: "Update Error", maximize: true, maxwidth: 500, maxheight: 100, autoSize: true, animation: "Fade", dialog_style: "alert", content: "Error during update: " + errorMessage }, [{ title: "Ok" }]);
    isServiceCalled = true; /*once service call is returned even with fail status - set this to true so that call to close window  in InlineEditor.js does not hang.*/
}

function addendum_display(id) {
    /*var height = $("#addendum_" + id).height();
	var width = $("#addendum_" + id).width();
	alert(width);
	customDialog({ width: width, height: height, title: 'Addendum Text', name: 'RadWindow' + cnt++, autoSize: true, content: $("#addendum_" + id).html(), model: false }, [{ title: "Ok"}]);
	*/
    for (var j = 0; j < AssessmentItems.length; j++) {
        if (AssessmentItems[j].ID == id) {
            var win = customDialog({ maximize: true, maxwidth: 800, maxheight: 400, title: 'Addendum Text', name: 'RadWindow' + cnt++, autoSize: true, content: "<div style='overflow: auto; height: 400px; max-width: 1000px;'>" + AssessmentItems[j].Addendum.Addendum_Text + "</div>", model: false }, [{ title: "Ok" }]);
            if (win) {
                win.set_width(800);
                win.set_height(470);
                win.center();
            }
            return;
        }
    }
    customDialog({ title: "Addendum Text Not Found", maximize: true, maxwidth: 200, maxheight: 100, dialog_style: "alert", content: "The text for this addendum was not loaded properly. Please reload the assessment and try your request again." }, [{ title: "Ok" }]);
    //simple_message('Addendum Text', "<div style='overflow: auto; height: 400px'>" + $("#addendum_" + id).html() + "</div>");
    //simple_message('Addendum Text',  $("#addendum_" + id).html() );
    //var content = "<div style='overflow: scroll; height: 90%'>" + $("#addendum_" + id).html() + "</div>";
    //customDialog({ title: 'Addendum Text', content: content, model: false, maximize: true, moveable: true }, [{ title: "Ok"}]);
    //radalert($("#addendum_" + id).html(), null, null, "Result");
}

function rubric_display(encID, id) {
    for (var j = 0; j < AssessmentItems.length; j++) {
        if (AssessmentItems[j].ID == id) {
            // Corey Creech 2/24/2014 - Rubric text was cut off at the bottom of the screen. 
            //By setting the XPOS and YPOS to 10, it put's the dialog box in the upper left corner.
            customDialog({ title: 'Rubric Text', autoSize: true, name: 'RadWindow' + cnt++, url: "../Controls/Rubrics/RubricText.aspx?yID=" + encID, xPos: 10, yPos: 10 });
            return;
        }
    }
    customDialog({ title: "Rubric Text Not Found", maximize: true, maxwidth: 200, maxheight: 100, dialog_style: "alert", content: "The text for this rubric was not loaded properly. Please reload the assessment and try your request again." }, [{ title: "Ok" }]);
}

var cnt = 1;
function simple_message(title, text) {
    customDialog({ maximize: true, maxwidth: 700, maxheight: 430, title: title, name: 'RadWindow' + cnt++, autoSize: true, content: text, model: false }, [{ title: "Ok" }]);
}

function toggleScoreOnTest(item) {

    var $item = $(item);

    newValue = $item.html().trim() == 'Score' ? 'No' : 'Yes';
    assessmentitem_changeField(item, 'ScoreOnTest', newValue, ScoreOnTest_UpdateCallback, [$item, newValue]);

}

function ScoreOnTest_UpdateCallback($item, newValue) {
    $item.html(newValue == 'Yes' ? 'Score' : 'Do Not Score');
}

function update_weight_pcts(updated_values) {
    $('.WeightPCT').each(function () {
        var weightPCT = $(this);
        var id = weightPCT.closest(".questionInstance").attr("id");
        weightPCT.html("(" + updated_values[id][1] + ")");
    });
    for (var j = 0; j < AssessmentItems.length; j++) {
        AssessmentItems[j].Weight = updated_values[AssessmentItems[j].ID][0];
    }
}

var UNDO_TYPE = {
    QUESTION_TEXT: 0,
    ANSWER_TEXT: 0
};

function UndoList() {
    this.list = new Array();
    var toolBar = $find("mainToolBar");
    this.UndoButton = toolBar.findItemByValue("Undo");
    this.InactiveURL = $("#undo_inactive").attr("src");
    this.ActiveURL = $("#undo_active").attr("src");
}

UndoList.prototype.Add = function (type, id, update, old_value, new_value) {
    this.list.push(new UndoItem(type, id, update, old_value, new_value));
    undoList.UpdateUndoButton();
};
UndoList.prototype.Rollback = function () {


    if (this.list.length == 0) return;
    var target_undo = this.list.pop();
    switch (target_undo.Type) {
        case UNDO_TYPE.QUESTION_TEXT:
        case UNDO_TYPE.ANSWER_TEXT:
            var $qi = $("#" + target_undo.Id); // find questionItem
            var $bodyTextContainer = $qi.find(".SKEditableBody[update*='" + target_undo.Update + "']").find(".SKEditableBodyText");
            var ckeditorId = $qi.find(".SKEditableBody[update*='" + target_undo.Update + "']").find(".SKEditableBodyText").attr("id");
            if ($bodyTextContainer.length == 0) {
                alert('Unable to undo. Item text not found.');
                return;
            }
            if (target_undo.IsItemBeingEdited()) { // if the text editor is currently open and editing the text we are about to rollback, prevent because it will just get confusing and complicated
                this.list.push(target_undo); // requeue
                return;
            }

            if (CKEDITOR) {
                CKEDITOR.instances[ckeditorId].setData(target_undo.Old_value);
            }

            assessmentitem_changeField($bodyTextContainer.parent(), 'Item_Text', target_undo.Old_value);

            break;
    }
    undoList.UpdateUndoButton();
};
UndoList.prototype.UpdateUndoButton = function () {
    if (this.list.length == 0) {
        this.UndoButton.set_imageUrl(this.InactiveURL);
    } else {
        this.UndoButton.set_imageUrl(this.ActiveURL);
    }

};

function UndoItem(type, id, update, old_value, new_value) {
    this.Type = type;
    this.Update = update;
    this.Id = id;
    this.Old_value = old_value;
    this.New_value = new_value;
}

UndoItem.prototype.IsItemBeingEdited = function () {
    if ($currEditableBody) {
        var $editable_qi = $currEditableBody.closest(".questionInstance");
        var editable_itemID = $editable_qi.attr("id");
        if (editable_itemID == this.Id && $currEditableBody.attr("update") == this.Update) {
            alert("You are attempting to undo a change to the text you are currently editing. Complete your current changes before proceeding with an undo.");
            return true;
        }
    }
    return false;
}; // Generic function to load data from a js array into a combobox when a user opens it
function bindComboBoxOnClient(sender, eventArgs) {
    $sender = $(sender);
    if ($sender.attr('loaded')) return;

    var list = eval(sender.get_attributes().getAttribute("DataSourceOnClient"));
    if (!list) {
        alert("Error loading dropdown data");
        return;
    }
    var previous_text = sender.get_text();
    sender.clearItems();
    for (var i = 0; i < list.length; i++) {
        var item = new Telerik.Web.UI.RadComboBoxItem();
        item.set_text(list[i]);
        //items[items.length] = item;
        sender.get_items().add(item);
        if (list[i] == previous_text) {
            sender.get_attributes().setAttribute("suppressNextUpdateEvent", 1);
            item.select();
        }
    }
    $sender.attr('loaded', 'true');
}

// Generic function to set the initial text in a combo box for JS loaded combo boxes
function setInitialTextOnClient(sender, eventArgs) {
    var value = sender.get_attributes().getAttribute("InitialValue");
    //if (value && value != '') {
    var item = new Telerik.Web.UI.RadComboBoxItem();
    item.set_text(value);
    sender.get_items().add(item);
    sender.get_attributes().setAttribute("suppressNextUpdateEvent", 1);
    item.select();
    //}
    $(sender.get_inputDomElement()).css('text-align', 'center');
}

// Specific to Common Item Standard Selection: User has selected a standard in the common item standard combo box
function treeItemStandardCommonClick(sender, args) {
    // Need to get the text that should appear on item based on selection, and call the update service to set standard
    var comboBox = $hidden_dropdown;
    var node = args.get_node();

    var text = node.get_attributes().getAttribute("ButtonText");
    var newValue = node.get_value();

    if (newValue == -1) {
        customDialog({ title: "Invalid Selection", maximize: true, maxwidth: 300, maxheight: 100, dialog_style: "alert", content: "A standard set cannot be selected.<br/><br/>" }, [{ title: "Ok" }]);
        return;
    }
    //var text = node.get_text();
    comboBox.set_text(text);

    //comboBox.trackChanges();
    comboBox.get_items().getItem(0).set_text(text);
    //comboBox.commitChanges();

    //comboBox.hideDropDown();
    assessmentitem_changeField($hidden_dropdown, "StandardID", newValue);
    //comboBox.attachDropDown();
}

function treeItemStandardCommonClick_PlaceholderVersion(sender, args) {
  
    // Need to get the text that should appear on item based on selection, and call the update service to set standard
    var $placeholder = $hidden_dropdown;
    var node = args.get_node();

    var text = node.get_attributes().getAttribute("ButtonText");
    var newValue = node.get_value();


    if (newValue == -1) {
        customDialog({ title: "Invalid Selection", height: 100, width: 300, dialog_style: "alert", content: "A standard set cannot be selected.<br/><br/>" }, [{ title: "Ok" }]);
        return;
    }

    if (text == "Filter") {
        customDialog({ title: "Invalid Selection", height: 100, width: 300, dialog_style: "alert", content: "An entire standard filter cannot be selected.<br/><br/>" }, [{ title: "Ok" }]);
        return;
    }
    //var text = node.get_text();
    $placeholder.attr('text', text);
    $placeholder.attr('value', newValue);
    $placeholder.find('input')[0].value = text;
    $placeholder.find('input')[0].title = text;

    //comboBox.hideDropDown();
    assessmentitem_changeField($hidden_dropdown, "StandardID", newValue, null, null, text);
    //comboBox.attachDropDown();
}

function InsertStandardForItem(standardID, questionID) {
    AddStandardToItemFromSearch(questionID, standardID);
}

// Generic, user has selected value in tree, need to set text in combo
function TreeInComboBoxClick(sender, args) {
    var comboBoxName = sender.get_attributes().getAttribute("ParentComboBox");
    var comboBox = $find(comboBoxName);

    var node = args.get_node();

    comboBox.set_text(node.get_text());

    //comboBox.trackChanges();
    comboBox.get_items().getItem(0).set_text(node.get_text());
    //comboBox.commitChanges();

    comboBox.hideDropDown();

    // Call comboBox.attachDropDown if:
    // 1) The RadComboBox is inside an AJAX panel.
    // 2) The RadTreeView has a server-side event handler for the NodeClick event, i.e. it initiates a postback when clicking on a Node.
    // Otherwise the AJAX postback becomes a normal postback regardless of the outer AJAX panel.

    comboBox.attachDropDown();
}

function StopPropagation(e) {
    if (!e) {
        e = window.event;
    }

    e.cancelBubble = true;
}

var $hidden_dropdown;

function comboStandardCommon_OnOpened(sender, eventArgs) {
    var selected_value = $hidden_dropdown.attr('value');
    var commonTree = $find("CommonStandardTree");
    var nodes = commonTree.get_allNodes();
    for (var j = 0; j < nodes.length; j++) {
        if (nodes[j].get_value() == selected_value) {
            nodes[j].select();
            nodes[j].scrollIntoView();
            break;
        }
    }
}

/*
function cmbStandardCommon_OnOpened(sender, eventArgs) {
var selected_value = hidden_dropdown[hd_radComboBox].get_attributes().getAttribute("StandardID");
var commonTree = $find("RadTreeStandardCommon");
var nodes = commonTree.get_allNodes();
for (var j = 0; j < nodes.length; j++) {
if (nodes[j].get_value() == selected_value) {
nodes[j].select();
nodes[j].scrollIntoView();
break;
}
}
}

// when the common item standard combo box closes
function cmbItemStandardCommon_OnClosed(sender, eventArgs) {
var $combo_StandardCommon = $(sender._element);
$("#standardDropdownHolder").prepend($combo_StandardCommon);
hidden_dropdown[0].show();
}
*/
// Generic for combo with tree: it scrolls tree to selected value
function comboWithNestedTree_OnOpened(sender, eventArgs) {
    var treeName = sender.get_attributes().getAttribute("ChildTreeName");
    var tree = sender.get_items().getItem(0).findControl(treeName);
    var selectedNode = tree.get_selectedNode();
    if (selectedNode) {
        selectedNode.scrollIntoView();
    }
}

var ItemIndex;
function buildItemIndex() {
    ItemIndex = {};
    for (var j = 0; j < AssessmentItems.length; j++) {
        ItemIndex[AssessmentItems[j].ID] = AssessmentItems[j];
    }
}

function getForm(formID) {
    for (var x = 0; x < AssessmentForms.length; x++) {
        if (AssessmentForms[x].FormId == formID) {
            return AssessmentForms[x];
        }
    }
    return null;
}

function renderForm(formID) {
    // first find correct sort orders
    var orders;
    var form = getForm(formID);
    if (form) {
        orders = form.ItemOrders;
    }
    if (!orders) {
        customDialog({ title: "Error", maximize: true, maxwidth: 500, maxheight: 100, autoSize: true, animation: "Fade", dialog_style: "alert", content: "Cannot find form: " + formID }, [{ title: "Ok" }]);
        return;
    }
    activeForm = formID;
    if (!$lblForm) $lblForm = $("#lblForm");
    $lblForm.html(form.FormName);
    var resorted_array = [];
    for (var j = 0; j < orders.length; j++) {
        var item = ItemIndex[orders[j]];
        if (item) {
            resorted_array.push(item);
        }
    }
    renderItems(resorted_array);
    if (formID == FieldTestFormId) {
        $(".visibilityHideFTtrue").removeClass('visibilityHideFTtrue');
    }

    if (postBackState) {
        if (postBackState.action == 'addBlankItem') {
            $('#QuestionHolder').scrollTop($('#testQuestions').height());
            $('.questionInstance').last().effect("highlight", { color: '#AACE43' }, 2300);
        } else if (postBackState.action == 'addBankItemsDrag') {
            $('#QuestionHolder').scrollTop(postBackState.scrollTop);
            $('.questionInstance[NewlyAdded="true"]').effect("highlight", { color: '#AACE43' }, 2300);
        } else if (postBackState.action == 'addBankItemsSearch') {
            $('#QuestionHolder').scrollTop($('#testQuestions').height());
            $('.questionInstance[NewlyAdded="true"]').effect("highlight", { color: '#AACE43' }, 2300);
        }
        postBackState = null;

    }
}

function ShowStandardSearch(questionID) {
    var AssessmentID = getURLParm('xID');
    var TestCategory = document.getElementById('lbl_TestCategory').value;
    customDialog({ url: '../Controls/Standards/StandardsSearch_ExpandedV2.aspx?NewAndReturnType=AssessmentPage&EncID=No&Question=' + questionID + '&MultiSelect=' + ((AssessmentID.length > 2) ? 'Yes' : 'No'), width: 950, height: 675, name: ('Search Standards'), title: ('Search Standards') });
}

var pdf_distincter = Math.random() * Date.parse(new Date());
function previewPDF() {
    //ShowSpinner();
    var $iframe_preview = $('#iframe_preview');
    //if (!$iframe_preview.attr('src'))
    $('#iframe_preview').attr('src', 'RenderAssessmentAsPDF.aspx?rnd=' + pdf_distincter + '&formID=' + activeForm + '&xID=' + getURLParm('xID') + '&#page=1&zoom=120');
    mainToolBar_buttonVisibilityChange('Preview');
    FieldTestTab_showhide(false);
    //setTimeout(HideSpinner, 1000);
}

function formSelected(formId) {
    renderForm(formId);

    var mainTabSelected = radtab_mainTabStrip.get_selectedTab();
    if (mainTabSelected.get_value() == 'Preview') {
        previewPDF();
    }
}

function formTabStrip_OnClientTabSelecting(sender, args) {
    var selected_tab_value = args.get_tab().get_value();
    var tab;
    if (selected_tab_value == FieldTestFormId) {
        tab = radtab_Forms.get_selectedTab();
    } else {
        if (radtab_FieldTest) tab = radtab_FieldTest.get_selectedTab();
    }
    if (tab) tab.set_selected(false);

    formSelected(selected_tab_value);
    CkEditorReload();
}

function loadAltForm(form_name) {
    var $altFormDiv = $("#altForm");
    var data = createJSONfromSortable();
    $altFormDiv.html($("#questionTemplate").render(data));
}

function createJSONfromSortable() {
    var data = [];
    $(".questionInstance").each(function (index) {
        data.push({ QuestionID: this.id });
    });
    return data;
}
function xcomboPlaceholder_OnClick(p) {

}
function comboPlaceholder_OnClick(placeHolder) {
    var $placeHolder = $(placeHolder);
    var comboName = $placeHolder.attr("attachToCombo");
    var comboElement = $("#" + comboName);
    $placeHolder.after(comboElement);
    $placeHolder.hide();
    $hidden_dropdown = $placeHolder;

    var currValue = $placeHolder.attr('value');
    var comboObject = $find(comboName);
    //alert(comboObject._enableScreenBoundaryDetection);
    if (!comboObject) {
        alert("combobox " + comboName + " not found.");
        return false;
    }

    var items = comboObject.get_items();
    var count = items.get_count();
    if (currValue != '') {
        for (var j = 0; j < count; j++) {
            var item = items.getItem(j);
            if (currValue == item.get_value()) {
                comboObject.get_attributes().setAttribute("suppressNext", "true");
                item.select();
            }
        }
    } else {
        comboObject.clearSelection();
    }

    //comboObject.set_text($placeHolder.attr('text'));
    comboObject.showDropDown();
}

function comboShared_OnClosed(sender, eventArgs) {
    var $rcbObject = $(sender._element);
    $("#standardDropdownHolder").prepend($rcbObject);
    $hidden_dropdown.show();
}

function comboShared_OnIndexChanged(sender, eventArgs) {
    var selectedItem = eventArgs.get_item();
    if (selectedItem == null) return;
    var attributes = sender.get_attributes();
    if (attributes.getAttribute("suppressNext") == "true") {
        attributes.setAttribute("suppressNext", "false");
        return false;
    }

    var newText = selectedItem != null ? selectedItem.get_text() : sender.get_text();
    var newValue = selectedItem.get_value();

    $hidden_dropdown.attr('text', newText);
    $hidden_dropdown.attr('value', newValue);
    $hidden_dropdown.find('input')[0].value = newText;

    //comboBox.hideDropDown();
    assessmentitem_changeField($hidden_dropdown, $hidden_dropdown.attr('field'), newValue);
}

function KeywordSearch() {
    if ($find("txtKeyword").get_value() == "") {
        customDialog({ title: "Keyword Required", height: 75, width: 250, autoSize: false, content: 'A keyword is required.', dialog_style: 'alert', animation: 'none' }, [{ title: "Ok" }]);
        return;
    }

    ShowSpinner("bankHolder");
    document.getElementById('btn_KeywordSearch').click();
}

function delete_uploaded_doc(formId, type) {
    hide_upload_frame();
    document.getElementById('FormToDelete').value = formId;
    document.getElementById('TypeToDelete').value = type;
    document.getElementById('btn_DeleteDocument').click();
}

function show_upload_frame(formId, type) {
    //window.frames["uploadFrame"].document.location.href = 'AssessmentUpload.aspx?xID=' + getURLParm('xID') + '&formID=' + formId + '&type=' + type;
    //Commented for FireFox Bug #9680 and Task #
    var frame;
    frame = document.getElementById('uploadFrame').contentWindow.document;
    frame.location.href = 'AssessmentUpload.aspx?xID=' + getURLParm('xID') + '&formID=' + formId + '&type=' + type;
    $("#documentTableHolder").css("max-height", "250px");
    $("#uploadForm").show();
}

function toggle_review_display(obj) {
    var $documentTableHolder = $("#documentTableHolder");
    $documentTableHolder.removeClass("doc_HideReviewer").removeClass("doc_ShowReviewer");
    $documentTableHolder.addClass(obj.checked ? "doc_ShowReviewer" : "doc_HideReviewer");
}

function show_uploaded_doc(fileName) {
    window.open(fileName);
}

function hide_upload_frame() {
    $("#uploadForm").hide();
    $("#documentTableHolder").css("max-height", "410px");
}

function upload_completed() {
    hide_upload_frame();
    document.getElementById('btn_RefreshDocumentsTable').click();
}

function rebuildDocumentsTable() {
    if (!$("#DocumentsTableHolder")) return;
    $("#DocumentsTableHolder").html($("#documentFormTemplate").render(AssessmentForms));
}


function updateBankFlags() {
    var $flag_yellow_template = $($("#flag_yellow_template").get(0));
    $(".thumbInstance").each(function (index, elm) {
        var $elm = $(elm);
        var found = false;
        for (var j = 0; j < AssessmentItems.length; j++) {
            if ($elm.attr('id') == 'B' + AssessmentItems[j].SharedContentID) {
                found = true;
                if ($elm.find(".flag_yellow").length > 0) continue;
                $flag_yellow_template.clone().insertAfter($elm.find(".zoom_in_img"));
                break;
            }

        }
        if (!found) {
            $elm.find(".flag_yellow").remove();
        }

    });

}


var bad_thumb_path = '../Images/thumb_none.png';
var allItem_refresh_needed = false;
var hostWindowName = 'AssessmentEdit';

// These variables will be set by other web pages that are children 
// of this current page (like ContentEditor_Item.aspx) and then 
// referred to once we get back to this page.
var specificItem_refresh_needed,
    specificItem_correction_needed,
	itemSearchAddList,
	arryUpdateWeightPcts;

function setDefaultImage(source) {
    var badImg = new Image();
    badImg.src = bad_thumb_path; // TODO: change this to use ItemThumbnailFolder
    var cpyImg = new Image();
    cpyImg.src = source.src;

    if (!cpyImg.width) {
        source.src = badImg.src;
    }

}

function onImgError(source) {
    source.src = bad_thumb_path; // TODO: change this to use ItemThumbnailFolder
    source.onerror = "";
    return true;
}

function ReorderWindow_OnClientClose() {
    if (allItem_refresh_needed) {
        ShowSpinner("QuestionHolder");

        allItem_refresh_needed = false;
        flag_a_change_to_pdf();
        document.getElementById('btn_RefreshQuestionsPanel').click();
    }
}

function EraseItem(obj) {
    var itemID = ($(obj).closest('.questionInstance')).attr('id');
    Service2.EraseItem(getURLParm('xID'), itemID, function (result_raw) {
        UpdateSingleItem(jQuery.parseJSON(result_raw));
    }, onItemMoved_onFailed);
}

function UpdateSingleItem(result) {
    if (result.PayLoad && result.PayLoad.ID) {
        for (var j = 0; j < AssessmentItems.length; j++) {
            if (AssessmentItems[j].ID == result.PayLoad.ID) {
                AssessmentItems[j] = result.PayLoad;
                ItemIndex[AssessmentItems[j].ID] = AssessmentItems[j];      // update lookup
                var newHTML = loadAssessmentItems(AssessmentItems[j]);
                questionHTML = $('.questionInstance[id=' + AssessmentItems[j].ID + '], .questionInstance_highlight[id=' + AssessmentItems[j].ID + ']');
                if (questionHTML && questionHTML.length && questionHTML.length == 1) {
                    var $item = questionHTML.replaceWith(newHTML);
                    $("#testQuestions").sortable('refresh');
                    highlight_item($('.questionInstance[id=' + AssessmentItems[j].ID + '], .questionInstance_highlight[id=' + AssessmentItems[j].ID + ']'));
                    RenumberQuestions();
                }

                var editorReloadForQueation = $("#" + AssessmentItems[j].ID + " .SKEditableBodyText");

                ReloadCkeditorForSingleItem(editorReloadForQueation);
                break;
            }
        }
        flag_a_change_to_pdf();

        /*  The only place that arryUpdateWeightPcts is set is within ContentEditor_Item.  Calls to AssessmentItemUpdate in Service2 can return an array 
		(via result.PayLoad) and a script (via result.ExecOnReturn) which should be run from this level (AssessmentPage).  But what if the call to 
		service2.AssessmentItemUpdate was not from here, but was from ContentEditor_Item?? Then rather than execute the script, ContentEditor_Item 
		needs to let AssessmentPage (which is its parent) know that it should update weight percents once its done refreshing the assessment items 
		with any changes.  ContentEditor_Item will set arryUpdateWeightPcts with an array of new weights and when we get back here to AssessmentPage, 
		we'll check whether arryUpdateWeightPcts has anything in it.  if so then we'll call Update_Weight_Pcts() essentially doing what 
		RemoveItem_actual does when it evaluates ExecOnReturn. - DHB.
		*/

        if (arryUpdateWeightPcts) {
            update_weight_pcts(arryUpdateWeightPcts);
            arryUpdateWeightPcts = null;
        }
    }
}

function UpdateSingleItem_FromServer(itemID) {
    Service2.GetItemJSON(getURLParm('xID'), itemID, function (result_raw) {
        UpdateSingleItem(jQuery.parseJSON(result_raw));
    }, onItemMoved_onFailed);
}

function ItemEditor_OnClientClose() {
    if (specificItem_refresh_needed) {
        UpdateSingleItem_FromServer(specificItem_refresh_needed);

        if (specificItem_correction_needed) {
            itemChange_showConfirm(specificItem_refresh_needed);
        }

        specificItem_refresh_needed = null;
        specificItem_correction_needed = null;
    }
}

var $manualReplaceTarget;
function ManualReplace(obj) {
    var testID = document.getElementById('lbl_TestID').value;
    $manualReplaceTarget = $(obj).closest('.questionInstance');
    var itemID = ($(obj).closest('.questionInstance')).attr('id');
    var TestCategory = document.getElementById('lbl_TestCategory').value;
    customDialog({ url: '../Controls/Items/ItemSearch.aspx?ItemSearchMode=SingleSelect&ItemFilterMode=Unfiltered&TestCurrCourseID=' + TestCurrCourseID + '&TestYear=' + TestYear + '&grade=' + grade + '&subject=' + subject + '&coursename=' + coursename + '&standardid=' + ItemIndex[itemID].StandardID + '&from=ManualReplace' + '&testID=' + testID + '&ShowExpiredItems=No&TestCategory=' + TestCategory + '&isSecure=' + isSecureAssessment + '&AssessmentType=' + AssessmentType, width: 1000, height: 580, title: 'Item Search', onClosed: ManualReplace_OnClientClose, destroyOnClose: true });
}

function ManualReplace_OnClientClose() {
    if (itemSearchAddList && itemSearchAddList.length > 0) {
        ShowSpinner();
        Service2.ManualReplace(getURLParm('xID'), $manualReplaceTarget.attr('id'), itemSearchAddList[0], function (result_raw) {
            HideSpinner();
            var result = jQuery.parseJSON(result_raw);
            if (result.StatusCode > 0) {
                customDialog({ title: "Manual Replace", maximize: true, maxwidth: 500, maxheight: 100, animation: "none", dialog_style: "alert", content: result.Message }, [{ title: "Ok" }]);
                return;
            }
            UpdateSingleItem(result);
            updateBankFlags();
            flag_a_change_to_pdf();
            itemSearchAddList = null;
            document.getElementById('btn_RefreshQuestionsPanel').click();
        }, onItemMoved_onFailed);
    }
}


function AutoReplace(obj) {
    ShowSpinner();
    var itemID = ($(obj).closest('.questionInstance')).attr('id');
    Service2.AutoReplace(getURLParm('xID'), itemID, function (result_raw) {
        HideSpinner();
        var result = jQuery.parseJSON(result_raw);
        if (result.StatusCode > 0) {
            customDialog({ title: "Auto Replace", hmaximize: true, maxwidth: 500, maxheight: 100, animation: "none", dialog_style: "alert", content: result.Message }, [{ title: "Ok" }]);
            return;
        }
        UpdateSingleItem(result);
        updateBankFlags();
        flag_a_change_to_pdf();
        document.getElementById('btn_RefreshQuestionsPanel').click();
    }, onItemMoved_onFailed);
}

function mainTabStrip_OnClientTabSelecting(sender, args) {
    if (typeof currentEditor != "undefined") {
        currentEditor.focusManager.blur();
    }
    var selected_tab_value = args.get_tab().get_value();
    if (selected_tab_value == 'Preview') {
        previewPDF();
    } else if (selected_tab_value == 'Edit') {
        mainToolBar_buttonVisibilityChange('Edit');
        $('#iframe_preview').attr('src', ''); //Coded for IE10 for TFS Bug Id:9039
        // WSH 1/31/12 Quirk that in at least IE, when switching to Preview tab, loading IFRAME there, then coming back to Edit, text editor for assessment directions is visisible causing toolbar to bleed to front. By hiding editor then reshowing, appears to solve the issue
        var editor = $find('RadEditorAssessmentDirections');
        if (editor) {
            editor.set_visible(false);
            setTimeout("$find('RadEditorAssessmentDirections').set_visible(true);", 50);
        }
        FieldTestTab_showhide(true);
    } else if (selected_tab_value == 'Documents') {
        mainToolBar_buttonVisibilityChange('Documents');
        $('#iframe_preview').attr('src', '');//Coded for IE10 for TFS Bug Id:9039
    } else {
        alert('Function is in development.');
    }
}

function postCloneDialog() {
    setTimeout(postCloneDialog_Actual, 100);
}

function postCloneDialog_Actual(failCount, val) {
    try {
        var msg = "This is a copy of the previously viewed assessment.";
        customDialog({ title: "Assessment Copy Successful", maximize: true, maxwidth: 400, maxheight: 100, autoSize: false, content: msg, dialog_style: 'alert', animation: 'none' }, [{ title: "Close" }]);
    } catch (e) {
        if (failCount > 5) return;
        setTimeout("postCloneDialog_Actual(" + (!failCount ? 1 : ++failCount) + ")", 100);
    }
}

function postCloneRemovedExpiredContentDialog() {
    setTimeout(postCloneRemovedExpiredContentDialog_Actual, 100);
}

function postCloneRemovedExpiredContentDialog_Actual(failCount) {
    try {
        var msg = "This is a copy of the previously viewed assessment. <br/> <br/> This copied assessment had items which have been removed due to copyright expiration of the item or its content.";
        customDialog({ title: "Assessment Copy Successful", maximize: true, maxwidth: 400, maxheight: 100, autoSize: false, content: msg, dialog_style: 'alert', animation: 'none' }, [{ title: "Close" }]);
    } catch (e) {
        if (failCount > 5) return;
        setTimeout("postCloneRemovedExpiredContentDialog_Actual(" + (!failCount ? 1 : ++failCount) + ")", 100);
    }
}

function mainToolBar_OnClientButtonClicked(sender, args) {
    var selected_button_value = args.get_item().get_value();
    var hasSecure = hasSecurePermission.toLowerCase() == "true" ? true : false;
    var isSecureflag = isSecureFlag.toLowerCase() == "true" ? true : false;
    var secureAssessment = isSecureAssessment.toLowerCase() == "true" ? true : false;
    switch (selected_button_value) {
        case "Reorder":
            var dialogTitle = "Reorder Assessment Items";
            if (hasSecure == true && isSecureflag == true) {
                if (secureAssessment == true) {
                    dialogTitle = SetSecureAssessmentTitle(dialogTitle);
                }
            }
            customDialog({ title: dialogTitle, center: true, maximize: true, url: 'AssessmentItemsReorder.aspx?xID=' + getURLParm('xID') + '&activeForm=' + activeForm, onClosed: ReorderWindow_OnClientClose });
            break;
        case "Proof":
            proof_assessment_uploaded_document();
            break;
        case "DeleteAssessment":
            customDialog({ title: "Delete Assessment?", height: 120, content: "You are about to delete this assessment. Do you wish to continue?<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel" }, { title: "Ok", callback: deleteAssessment_confirmCallback }]);
            break;
        case "Undo":
            undoList.Rollback();
            break;
        case "Configure":
            var dialogTitle = "Configure Assessment";
            if (hasSecure == true && isSecureflag == true) {
                if (secureAssessment == true) {
                    dialogTitle = SetSecureAssessmentTitle(dialogTitle);
                }
            }
            customDialog({ title: dialogTitle, url: "../Dialogues/Assessment/AssessmentConfiguration.aspx?xID=" + getURLParm('xID') + "&yID=" + encryptedUserID, maximize: true, maxwidth: 800, maxheight: 600 });
            break;
        case "Copy":
            customDialog({ title: "Assessment Identification", url: "../Dialogues/Assessment/CreateAssessmentIdentification.aspx?xID=" + getURLParm('xID') + "&yID=" + encryptedUserID + "&senderPage=content&copy=yes", maximize: true, maxwidth: 630, maxheight: 600 });
            break;
        case "Print":
            // Modification 12/5/2013
            // Modified this block of code to display an error message if they try to print an external assessment
            // with no uploaded document.
            if (ContentType == 'External') {
                if (!hasReviewerFile && !hasAssessmentFile) {
                    assessmentExternalDocError();
                } else {
                    customDialog({ title: "Print Assessment", url: "../Dialogues/Assessment/AssessmentPrint.aspx?xID=" + getURLParm('xID'), maximize: true, maxwidth: 520, maxheight: 400 });
                }

            } else {
                customDialog({ title: "Print Assessment", url: "../Dialogues/Assessment/AssessmentPrint.aspx?xID=" + getURLParm('xID'), maximize: true, maxwidth: 520, maxheight: 400 });
            }
            //End modification 12/5/2013
            break;
        case "OnlinePreview":
            assessmentOnlinePreview();
            break;
        case "Scheduler":
            window.open("../Controls/Assessment/AssessmentScheduling.aspx?xID=Yjc4VjIvZzhoOU5iZWxvYU5RK01Xdz09&yID=Yjc4VjIvZzhoOU5iZWxvYU5RK01Xdz09");
            //customDialog({ name: 'AssessmentsScheduling', title: "Schedule Assessment", url: "../Controls/Assessment/AssessmentScheduling.aspx?User=District&Category=District", height: 900, width: 700 });
            break;
        case "HTML":
            window.open("../Dialogues/Assessment/AssessmentPrint.aspx?xID=" + getURLParm('xID') + "&PrintHTML=Yes", '_blank');
            break;
        case "Word":
            window.open("../Dialogues/Assessment/AssessmentPrint.aspx?xID=" + getURLParm('xID') + "&PrintWord=Yes", '_blank');
            break;
        case "Tags":
            customDialog({ title: "Tag Associations", url: "../Dialogues/Assessment/AssessmentTags.aspx?xID=" + getURLParm('xID') + "&yID=" + encryptedUserID + "&senderPage=content", width: 940, height: 600 });
            break;
        default:
            alert('Function is in development.');
    }
}

function proof_assessment_uploaded_document() {
    ShowSpinner();
    Service2.ProofAssessmentMissingUploadedDoc(getURLParm('xID'), proof_assessment__uploaded_document_callback, proof_assessment_onFailure);
}

function proof_assessment__uploaded_document_callback(result_raw) {
    HideSpinner();
    var result = jQuery.parseJSON(result_raw);
    if (result.StatusCode > 0) {        
        if (result.StatusCode == Enums.ProofStatus_base + Enums.ProofStatusMultipleChoiceAlignmentProblem + Enums.ProofStatusDistractorCountMismatch) {
            showDistractorCountMismatch(true);
        }
        else if (result.StatusCode == Enums.ProofStatus_base + Enums.ProofStatusDistractorCountMismatch) {
            showDistractorCountMismatch(false);
        }
        else if (result.StatusCode == Enums.ProofStatus_base + Enums.ProofStatusMultipleChoiceAlignmentProblem) {
            showMultipleChoiceAlignmentProblem();
        }
        else if (result.Message == "MissingUploadedDoc") {
            var message = "<div class='rwDialogPopup radalert' style='margin: 0px; padding-bottom: 0px'>";
            message = message + "<div style='color: red; font-weight: bold; font-size: 14pt; text-align: center; position: relative; left: -20px'>Attention!</div><br/>" +
							"<div>Before proofing the assessment an external assessment<br/>document must be uploaded.";
            if (MultiForm == "True")
            { message = message + "<br/><br/>If there are multiple forms, each one requires its matching<br/>external assessment document."; }

            message = message + "</div></div>";

            customDialog({ title: "Proof Assessment", maximize: true, maxwidth: 500, maxheight: 120, animation: "none", content: message }, [{ title: "OK" }]);
        }
        else if (result.Message == "DupXrefs" || result.Message == "OrphanedXrefs" || result.Message == "TQSorts" || result.Message == "XRSorts") {
            var errMessage;
            switch (result.Message) {
                case "DupXrefs":
                    errMessage = "Duplicate XRefs";
                    break;
                case "OrphanedXrefs":
                    errMessage = "Orphaned XRefs";
                    break;
                case "TQSorts":
                    errMessage = "Improper TQ Sorts";
                    break;
                case "XRSorts":
                    errMessage = "Improper Xref Sorts";
                    break;
            }

            var message = "<div class='rwDialogPopup radalert' style='margin: 0px; padding-bottom: 0px'>";
            message = message + "<div style='color: red; font-weight: bold; font-size: 14pt; text-align: center; position: relative; left: -20px'>Attention!</div><br/>" +
							"<div>Unable to proof assessment. Please contact support. <br/><br/> ERROR: " + errMessage + "<br/>TESTID: " + document.getElementById('lbl_TestID').value;
            message = message + "</div></div>";

            customDialog({ title: "Proof Assessment", maximize: true, maxwidth: 500, maxheight: 120, animation: "none", content: message }, [{ title: "OK" }]);
        }
        else if (result.Message == "SecureHasOtherQuestions")
        {
            var message = "<div class='rwDialogPopup radalert' style='margin: 0px; padding-bottom: 0px'>";
            message = message + "<div style='color: red; font-weight: bold; font-size: 14pt; text-align: center; position: relative; left: -20px'>Attention!</div><br/>" +
							"<div>Temporarily Secure Assessments only support multiple choice and true/false items. Please review and revise the assessment content.";
            message = message + "</div></div>";
            customDialog({ title: "Proof Assessment", maximize: true, maxwidth: 500, maxheight: 120, animation: "none", content: message }, [{ title: "OK" }]);
        }
        else {
            customDialog({ title: "Proof Assessment", maximize: true, maxwidth: 500, maxheight: 120, animation: "none", dialog_style: "alert", content: result.Message }, [{ title: "Ok" }]);
        }
        return;
    }
    proof_assessment();
}

function proof_assessment() {
    if (navigator.userAgent.search("Safari") >= 0 || navigator.userAgent.search("Chrome") >= 0) {
        customDialog({ title: "Proof Assessment", maximize: true, maxwidth: 500, maxheight: 120, animation: "none", content: $("#proof_dialog").text() }, [{ title: "Proof Assessment", callback: proof_assessment_confirmed }, { title: "Cancel" }]);
    }
    else {
        customDialog({ title: "Proof Assessment", maximize: true, maxwidth: 500, maxheight: 120, animation: "none", content: $("#proof_dialog").html() }, [{ title: "Proof Assessment", callback: proof_assessment_confirmed }, { title: "Cancel" }]);
        //assessment_changeField('TestProofed', 'Yes', proof_assessment_callback);
        //document.getElementById('btnProofAssessment').click();
    }
}

function assessmentExternalDocError() {
    var message = "<div class='rwDialogPopup radalert' style='margin: 0px; padding-bottom: 0px'>";
    message = message + "<div style='color: red; font-weight: bold; font-size: 14pt; text-align: center; position: relative; left: -20px'>Attention!</div><br/>" +
                    "<div>Before printing an external assessment <br/> a document must be uploaded.";
    message = message + "</div></div>";

    if (navigator.userAgent.search("Safari") >= 0 || navigator.userAgent.search("Chrome") >= 0) {
        customDialog({ title: "External Document Error", maximize: true, maxwidth: 500, maxheight: 120, animation: "none", content: message }, [{ title: "OK" }]);
    }
    else {
        customDialog({ title: "External Document Error", maximize: true, maxwidth: 500, maxheight: 120, animation: "none", content: message }, [{ title: "OK" }]);
    }
}

function proof_assessment_confirmed(bypassLevel2Checks) {
    ShowSpinner();
    if (!bypassLevel2Checks) bypassLevel2Checks = false;
    Service2.ProofAssessment(getURLParm('xID'), bypassLevel2Checks, proof_assessment_callback, proof_assessment_onFailure);
}

function proof_assessment_callback(result_raw) {
    HideSpinner();
    var result = jQuery.parseJSON(result_raw);
    if (result.StatusCode > 0) {
        if (result.StatusCode == Enums.ProofStatus_base + Enums.ProofStatusMultipleChoiceAlignmentProblem + Enums.ProofStatusDistractorCountMismatch) {
            showDistractorCountMismatch(true);
        } else if (result.StatusCode == Enums.ProofStatus_base + Enums.ProofStatusDistractorCountMismatch) {
            showDistractorCountMismatch(false);
        }
        else if (result.StatusCode == Enums.ProofStatus_base + Enums.ProofStatusMultipleChoiceAlignmentProblem) {
            showMultipleChoiceAlignmentProblem();
        } else {
            customDialog({ title: "Proof Assessment", maximize: true, maxwidth: 500, maxheight: 120, animation: "none", dialog_style: "alert", content: result.Message }, [{ title: "Ok" }]);
        }
        return;
    }
    ShowSpinner();
    location.reload();
}

function showDistractorCountMismatch(alsoShowMultipleChoiceAlignmentProblem) {
    var submitButton;
    if (alsoShowMultipleChoiceAlignmentProblem) {
        submitButton = { title: "Proof Assessment", callback: showMultipleChoiceAlignmentProblemDelay };
    } else {
        submitButton = { title: "Proof Assessment", callback: proof_assessment_confirmed, argArray: [true] };
    }
    customDialog({ title: "Item Distractor Notification", maximize: true, maxwidth: 500, maxheight: 150, animation: "none", dialog_style: "alert", content: "Items on this assessment do not have the same number of distractors.<br/><br/>To continue with proofing, select Proof Assessment; to return to the assessment, select Cancel." }
		, [submitButton, { title: "Cancel" }]);
}

function showMultipleChoiceAlignmentProblemDelay() {
    setTimeout(showMultipleChoiceAlignmentProblem, 50);
}

function showMultipleChoiceAlignmentProblem() {
    customDialog({ title: "Proof Assessment Notification", height: 120, width: 500, animation: "none", dialog_style: "alert", content: "<center><B>Please read this important message if administering this assessment with Plain Paper bubble sheets.</B></center><br/><br/>Due to the placement of multiple item types on this assessment, a generic plain paper bubble sheet will be printed." }
		, [{ title: "Proof Assessment", callback: proof_assessment_confirmed, argArray: [true] }, { title: "Cancel" }]);
}

function proof_assessment_onFailure() {
    HideSpinner();
    alert('Error proofing assessment');
}

function RenumberQuestions() {
    var count = 0;
    $(".questionInstance .sort_number").each(function () {
        var div_by_2 = Math.floor(count / 2) + 1;
        var $this = $(this);
        $this.html(div_by_2 + ($this.hasClass('item_number_div') ? '.' : ''));
        count++;
    });
}

function onItemMoved() {
    //Calling the registered web-service using ASP.net AJAX provides intellisense.
    var new_order = $("#testQuestions").sortable('toArray');
    Service2.UpdateItemOrders(getURLParm('xID'), activeForm, new_order.join(','), function (result_raw) {
        result = jQuery.parseJSON(result_raw);
        var form = getForm(activeForm);
        if (form) {
            form.ItemOrders = new_order;
        }
        if (result.StatusCode == Enums.UpdatedDataAttached) {
            AssessmentForms = result.PayLoad;
        }

        RenumberQuestions();
        flag_a_change_to_pdf();
    }, onItemMoved_onFailed);
}

/*function onItemMoved_onSuccess(return_raw) {
			
RenumberQuestions();
flag_a_change_to_pdf();
}*/

function onItemMoved_onFailed(obj) {
    HideSpinner();
    //BUG 21121 : show error message only if _statusCodde is not Zero(0).
    if (obj._statusCode != 0)
    {
        alert(obj._message);
    }
}

function removeItem(removeIcon) {
    if ($telerik.$('.radconfirm').length == 1)
        customDialog({ title: 'Remove Item?', maximize: true, maxwidth: 500, maxheight: 120, content: 'You are about to remove this item from this assessment. Do you wish to continue?<br/>', dialog_style: 'confirm' }, [{ title: 'Cancel' }, { title: 'Ok', callback: removeItem_confirmCallback, argArray: [removeIcon] }]);
    else
        customDialog({ title: 'Remove Item?', maximize: true, maxwidth: 500, maxheight: 120, content: 'You are about to remove this item from this assessment. Do you wish to continue?<br/>', dialog_style: 'confirm' }, [{ title: 'Cancel' }, { title: 'Ok', callback: removeItem_confirmCallback, argArray: [removeIcon] }]);
}

function removeItem_FromEditor(itemID, result_raw) {
    questionHTML = $('.questionInstance[id=' + itemID + '], .questionInstance_highlight[id=' + itemID + ']');
    if (questionHTML && questionHTML.length && questionHTML.length == 1) {
        removeItem_Actual(result_raw, itemID, $($(questionHTML).get(0)));
    }
}

function removeItem_Actual(result_raw, itemID, $itemToDelete) {
    result = jQuery.parseJSON(result_raw);
    if (result.StatusCode > 0) {
        assessment_changeField_event_onFailure(null, result.Message);
        return;
    }
    $itemToDelete.remove();
    $("#testQuestions").sortable('refresh');
    RenumberQuestions();
    flag_a_change_to_pdf();
    post_item_reload();
    for (var j = 0; j < AssessmentItems.length; j++) {
        if (AssessmentItems[j].ID == itemID) {
            AssessmentItems.splice(j, 1);
            break;
        }
    }

    AssessmentForms = jQuery.parseJSON(result.PayLoad[1]);
    updateBankFlags();

    if (result.ExecOnReturn) {
        eval(result.ExecOnReturn);
    }
}

function removeItem_confirmCallback(removeIcon, item_being_deleted) {
    var $question_being_deleted = $(removeIcon).closest('.questionInstance');
    if (!item_being_deleted) item_being_deleted = $question_being_deleted.attr('id');
    //question_sort_array.splice(question_sort_array.indexOf(item_being_deleted), 1);
    Service2.RemoveAssessmentItems(getURLParm('xID'), item_being_deleted, function (result_raw) {
        removeItem_Actual(result_raw, item_being_deleted, $question_being_deleted);
        //removeItem_FromEditor(item_being_deleted, result_raw);
    }, removeItem_onFailed);
}

/* function removeItem_onSuccess() {
$question_being_deleted.remove();
$("#testQuestions").sortable('refresh');
RenumberQuestions();
flag_a_change_to_pdf();
post_item_reload();
}*/

function removeItem_onFailed(result_raw, errorMessage) {
    if (result_raw) errorMessage = jQuery.parseJSON(result_raw).ErrorMessage;
    customDialog({ title: "Update Error", maximize: true, maxwidth: 500, maxheight: 100, autoSize: true, animation: "Fade", dialog_style: "alert", content: "Error during update: " + errorMessage }, [{ title: "Ok" }]);
}

function flag_a_change_to_pdf() {
    pdf_distincter = Math.random() * Date.parse(new Date());
    $('#iframe_preview').attr('src', '');
}

function bankReloadStart() {
    ShowSpinner("bankHolder");
}

function prepare_bank_drag(targetBank, initialLoad) {  
    if (AssessmentIsProofed) return;

    $(targetBank + ' .thumbInstance').draggable({
        scroll: false,
        delay: 300,
        //helper: 'clone',
        containment: '#editContentContainer',
        //opacity: .90,
        revert: 'invalid',
        cursor: 'move',
        appendTo: 'body',
        //cursorAt: { left: 10 },
        connectToSortable: '#testQuestions',
        start: function (event, ui) {
            $("#bankQuestions").disableSelection();
        },
        stop: function (event, ui) {          
            $("body").unbind('mousemove', dragHelper);          // The bind happens on start of drag on #TestQuestions which occurs when you drag from here over to it. However the stop on that object will not fire in all cases so we must unbind here as well
            if (isTouchDevice()) {
                _drag_delay_toggle = false;
                $(this).draggable('disable').removeClass('ui-state-disabled');
            }
            $('#testQuestions').sortable('disable');
            scrollHelper_Cancel();
        },
        helper: function (event, ui) {
            return getThumbnailMovingHelper(event, $(this), targetBank);
        }

    });

    $("#bankQuestions").disableSelection();
    if (isTouchDevice()) {
        $(targetBank + " .thumbInstance").draggable('disable').removeClass('ui-state-disabled');


        $(targetBank + " .thumbInstance").bind("touchstart", function (e) {
            if (_drag_delay_toggle == false) {
                var _this = $(this);
                var _e = e;
                this.downTimer = setTimeout(function () {
                    _this.draggable('enable');
                    $('#testQuestions').sortable('enable');

                    //_this.effect("shake", { times: 5, distance: 8 }, 50);
                    _drag_delay_toggle = true;
                    _this.trigger(_e);
                }, 500);
            }
        }).bind("touchend", function (e) {
            clearTimeout(this.downTimer);
            _drag_delay_toggle = false;
        }).bind("touchmove", function (e) {
            clearTimeout(this.downTimer);
            _drag_delay_toggle = false;
        });
    } else {
        $(targetBank + " .thumbInstance").bind("mousedown", function (e) {
            $('#testQuestions').sortable('enable');

        });
        $(targetBank + " .thumbInstance").bind("mouseup", function (e) {
            $('#testQuestions').sortable('disable');
        });
    }
    if (!initialLoad) takeFullHeight();

}

var top_constraint_for_drag_helper, bottom_constraint_for_drag_helper;


function getThumbnailMovingHelper(event, ui, targetBank) {
    //var ui = $(event.currentTarget);
    //return ui.clone();
    var dragged_item_id = ui.attr("id");
    ui.attr('targetBank', targetBank);
    var checked_count = 0;
    var $multiDragHelper, newItem;
    $(targetBank + ' .thumbInstance :checked').each(function (index) {
        var $div = $(this).closest('.thumbInstance');
        var item_id = $div.attr('id');
        if (item_id != dragged_item_id) {
            if (checked_count < 2) {
                // Start by cloning any items that are checked (but exclude the actually dragged item)
                if (checked_count == 0) {
                    $multiDragHelper = $div.clone();
                    $multiDragHelper.find("span").hide();
                } else {
                    $multiDragHelper.append(newItem = $div.clone().css('position', 'relative').css('left', 10).css('top', 10));
                    newItem.find("span").hide();
                }
            }
            checked_count++;
        }

    });

    // now we need to add the actually dragged item, but it's position is affected by how many other items are getting dragged
    if (checked_count == 1) {
        // we have 1 other item we're dragging
        $multiDragHelper.append(newItem = ui.clone().css('position', 'relative').css('left', 10).css('top', 10));
        newItem.find("span").hide();
        $multiDragHelper.append("<div class=\"adding_helper_text\">Adding 2</div>");
    } else if (checked_count > 1) {
        // we have 2 other items we're dragging (because visibile limit is 3 total)
        $multiDragHelper.append(newItem = ui.clone().css('position', 'relative').css('left', 20).css('top', -180));
        newItem.find("span").hide();
        $multiDragHelper.append("<div class=\"adding_helper_text\">Adding " + (checked_count + 1) + "</div>");
    } else {
        // we are only dragging the one item, straight copy
        $multiDragHelper = ui.clone();
        $multiDragHelper.find("span").hide();
        $multiDragHelper.append("<div class=\"adding_helper_text\">Adding 1</div>");
    }

    return $multiDragHelper;
}


function selectFilterTab(tab_name) {
    var tabStrip = radtab_SearchOptions;
    var tab = tabStrip.findTabByText(tab_name);
    if (!tab) {
        alert("There is no tab with text \"" + text + "\"");
        return false;
    }

    tab.set_selected(true); //The same as tab.select();
    return false;
}

function FieldTestTab_showhide(show_hide) {
    var tabStrip = radtab_Forms;
    var tab = tabStrip.findTabByText("Field Test");
    if (tab) {
        if (show_hide) {
            tab.show();
        } else {
            tab.hide();
        }
    }
}

//This function is used to determine what type of access (edit or copy) a user has for an item.
//Currently in progress, remove first line when we have itembanks for assessmentitems
function CanUserUseItemBank(itemID, SharedContentID, IBarray) {
    //return true;
    if (SharedContentID == 0)
        return true;

    for (var j = 0; j < AssessmentItems.length; j++) {
        if (AssessmentItems[j].ID == itemID) {
            if (IBAssessmentArray) {
                var iba = $.grep(IBAssessmentArray[0], function (a) {
                    return (a.Id == SharedContentID);
                });

                for (var i = 0; i < iba.length; i++) {
                    for (var k = 0; k < IBarray[0].ItemBanks.length; k++) {
                        if (IBarray[0].ItemBanks[k].TargetType == iba[i].TargetType && IBarray[0].ItemBanks[k].Label == iba[i].Label /* Once we figure out how to get schools in here add this: && IBarray[0].ItemBanks[k].Target == iba[i].Target*/) {
                            return true;
                        }
                    }
                }
            }
            return false; // if we didn't return true above, then it's false for this item
        }
    }
    return false;
}

var default_sort_in_progress = false;
function CheckDefaultSortOrder(itemID) {
    if (!default_sort_in_progress) {
        default_sort_in_progress = true;
        Service2.CheckDefaultSortOrder(activeForm, itemID,
            function (result_raw) {
                var result = jQuery.parseJSON(result_raw);

                if (result.StatusCode > 0) {
                    var AddendumResult = result.PayLoad.split("|");
                    var Addendum_Text = AddendumResult[0];
                    var ItemCount = AddendumResult[1];

                    if (ItemCount == "0") {
                        var alertMsg = "<br/>The items for the selected addendum are <br/> already using the default sort order for this <br/>assessment.<br/>";
                        customDialog({ title: "Item Sort Order", center: true, width: 270, height: 75, content: alertMsg }, [{ title: "Ok" }]);
                        default_sort_in_progress = false;
                    }
                    else {
                        var msg = "You have chosen to use the default sort order for items tied to <br /> the selected addendum. There are " + ItemCount + " items tied to this addendum <br/> on the assessment.<br/><p>Select <b>Ok</b> to proceed with using the default sort order. Select<br/> <b>Cancel</b> to keep the items tied to the selected addendum in their <br/> current order.</p>";
                        customDialog({ title: "Item Sort Order - " + Addendum_Text, dialog_style: 'alert', center: true, closeMode: false, width: 445, height: 100, content: msg }, [{ title: "Cancel" }, { title: "Ok", callback: defaultSort_event_confirmCallback, argArray: [itemID] }]);
                        default_sort_in_progress = false;
                    }
                }
         },
            defaultSort_event_onFailure);
    }
}

function defaultSort_event_confirmCallback(itemID) {
    Service2.UseDefaultSortOrder(activeForm, itemID,
        function (result_raw) {
            var result = jQuery.parseJSON(result_raw);

            if (result.StatusCode > 0) {
                var UpdateStatus = result.PayLoad;
                if (UpdateStatus == "True") {
                    document.getElementById('btn_RefreshQuestionsPanel').click();
                }
            }
            default_sort_in_progress = false;
        },
        defaultSort_event_onFailure);
}

function defaultSort_event_onFailure() {
    var alertMsg = "<br/>Error:  Use default sort order failure.";
    customDialog({ title: "Item Sort Order", center: true, width: 270, height: 75, content: alertMsg }, [{ title: "Ok" }]);
    default_sort_in_progress = false;
}
