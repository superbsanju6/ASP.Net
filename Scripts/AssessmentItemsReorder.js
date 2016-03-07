var activeForm = 1;
var bad_thumb_path = '../Images/thumb_none.png';

$(document).ready(function () {
    $sortableGrid = $('#sortableGrid');
    $("#sortable").sortable({
        start: function (event, ui) {
            top_constraint_for_drag_helper = $('#topPortion').height() + 60;
            bottom_constraint_for_drag_helper = $(window).height() - 40;
            $("body").bind('mousemove', dragHelper);
        },
        stop: function (event, ui) {
            $("body").unbind('mousemove', dragHelper);
            scrollHelper_Cancel();
            onItemMoved(event, ui);
        },
        placeholder: 'questionPlaceholder',
        helper: function (event, ui) {
            return getThumbnailMovingHelper(event, ui);
        }
    });
    $("#sortable").disableSelection();
    takeFullHeight();

    setTimeout(function () {
        var rtsScroll = $(".rtsScroll");
        rtsScroll.css('width', (parseInt($(".rtsScroll").css('width')) + 20) + "px");

    }, 500);
    RenumberQuestions();

});


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

function previewBankQuestion(id) {
    customDialog({ url: '../ControlHost/PreviewTestQuestion.aspx?type=TestQuestion&xID=' + id, title: 'Item Preview', maximize: true, maxwidth: 400, maxheight: 350 });
}

function getDialogRadWindow(name) {
    if (!name) name = 'DialogRadWindow';
    if ($find(name)) return $find(name);
}

function selectedFieldTestItems(checkboxes) {
    var ftCount = 0;
    for (var j=0; j < checkboxes.length; j++) {
        if ($(checkboxes[j]).attr("isFieldTest") == "True") {
            checkboxes[j].checked = false;
            ftCount++;
        }
    }
    return ftCount;
}

function removeItems() {
    var text;
    var selected_checkboxes = $('.thumbInstance :checked');
    var ftCount = selectedFieldTestItems(selected_checkboxes);
    if (selected_checkboxes.length == 0) {
        customDialog({ title: "Remove Items", content: 'No items selected to remove', dialog_style: 'alert', animation: 'none' }, [{ title: "Ok"}]);
        return;
    } else if (activeForm != FieldTestFormId && ftCount > 0) {
        if (selected_checkboxes.length > ftCount) {
            if (selected_checkboxes.length == ftCount + 1) {
                text = 'One or more of the items you selected are field test items which can only be removed from the Field Test tab. If you continue, you will remove the 1 other item you selected from this assessment. Do you wish to continue?';
            } else if (selected_checkboxes.length > ftCount) {
                text = 'One or more of the items you selected are field test items which can only be removed from the Field Test tab. If you continue, you will remove the ' + (selected_checkboxes.length - ftCount) + ' other items you selected from this assessment. Do you wish to continue?';
            }
            var selected_checkboxes = $('.thumbInstance :checked'); // reload list now that FT items have been de-checked
            customDialog({ title: "Remove Items?", maximize: true, maxwidth: 500, maxheight: 120, autoSize: false, content: text, dialog_style: 'confirm', animation: 'none' }, [{ title: "Cancel" }, { title: "Continue", callback: removeItem_confirmCallback, argArray: [selected_checkboxes]}]);
        } else {
            text = 'All of the items you selected are test items which can only be removed from the Field Test tab.';
            customDialog({ title: "Cannot Remove Items", maximize: true, maxwidth: 500, maxheight:120, autoSize: false, content: text, dialog_style: 'alert', animation: 'none' }, [{ title: "Ok"}]);
        }

    } else {
        if (selected_checkboxes.length == 1) {
            text = 'You are about to remove 1 item from this assessment. Do you wish to continue?';
        } else {
            text = 'You are about to remove ' + selected_checkboxes.length + ' items from this assessment. Do you wish to continue?';
        }
        customDialog({ title: "Remove Items?", maximize: true, maxwidth: 500, maxheight:120, autoSize: false, content: text, dialog_style: 'confirm', animation: 'none' }, [{ title: "Cancel" }, { title: "Continue", callback: removeItem_confirmCallback, argArray: [selected_checkboxes]}]);
    }


}

function removeItem_confirmCallback(selected_checkboxes) {
    //$question_being_deleted = $(removeIcon).closest('.questionInstance');
    var question_sort_array = $("#sortable").sortable('toArray');
    var selected_ids = new Array(selected_checkboxes.length);
    var selected_items = new Array(selected_checkboxes.length);
    for (var j = 0; j < selected_checkboxes.length; j++) {
        selected_items[j] = $(selected_checkboxes[j]).closest('.thumbInstance');
        var this_id = $(selected_items[j]).attr('id');
        selected_ids[j] = this_id;
        question_sort_array.splice(question_sort_array.indexOf(this_id), 1);
    }
    //    selected_items.each(function (index) { selected_ids[index] = $(this).closest('.thumbInstance').attr('id'); });
    //question_sort_array.splice(question_sort_array.indexOf($question_being_deleted.attr('id')), 1);
    Service2.RemoveAssessmentItems(getURLParm('xID'), selected_ids.join(',')/*, question_sort_array.join(',')*/, function (result_raw) {
        result = jQuery.parseJSON(result_raw);
        for (var j = 0; j < selected_items.length; j++) {
            selected_items[j].remove();
        }
        $("#sortable").sortable('refresh');
        AlertAssessmentWindowOfChanges();
        RenumberQuestions();
        AssessmentForms = jQuery.parseJSON(result.PayLoad[1]);
    }, removeItem_onFailed);
}


function removeItem_onFailed(obj) {
    AlertAssessmentWindowOfChanges();   // just in case data gets fouled up
    if (obj)
        alert(obj);
}

function renderForm(formID) {
    activeForm = formID;
    var orders;
    for (var x = 0; x < AssessmentForms.length; x++) {
        if (AssessmentForms[x].FormId == formID) {
            orders = AssessmentForms[x].ItemOrders;
            break;
        }
    }

    var $hiddenFieldTestHoldingArea = $("#hiddenFieldTestHoldingArea");
    var $sortable = $("#sortable");
    item_array = $(".thumbInstance");                               // First get the items

    // move all of the items to hidden storage
    for (var j = 0; j < item_array.length; j++ ) {
        $hiddenFieldTestHoldingArea.append(item_array[j]);
    }
    // then move all of the items that are in the current form into view, in order
    for (var j = 0; j < orders.length; j++) {
        for (var i = 0; i < item_array.length; i++) {
            if (item_array[i].getAttribute('id') == orders[j]) {
                $sortable.append(item_array[i]);
                break;
            }
        }
    }
    //$("#sortable").sortable("refresh");             // update the sortable
    RenumberQuestions();
}

var shuffle_in_progress = false;
function Shuffle() {
    if (shuffle_in_progress)
        return;
    shuffle_in_progress = true;
    slide_queue = [];
    var addemIDCount = 0;
    var order_array = [];
    var addendum_lookup = {};
    item_array = $(".ui-sortable .thumbInstance");                               // First get the items
    for (var j = 0; j < item_array.length; j++) {
        item_array[j] = $(item_array[j]);                           // Go ahead now and update the array so the elements are JQuery items
        var addendum_id = item_array[j].attr("AddendumID");
        if (addendum_id != "") {                                   // Item has addendum
            addemIDCount = addemIDCount + 1;
            if (!addendum_lookup[addendum_id]) {                    // Have not seen this addendum yet, create new associative array entry for it, append to order output array                
                addendum_lookup[addendum_id] = [];
                order_array.push(addendum_lookup[addendum_id]);
            }
            //else {
            //    addendum_lookup[addendum_id] = [];
            //    order_array.push(addendum_lookup[addendum_id]);
            //}
            addendum_lookup[addendum_id].push(j);                   // Add this item to the specific addendum entry in the associative array
        } else {
            order_array.push(j);                                    // No Addendum for item, just add to order output array
        }
    }

    if (addemIDCount == item_array.length) //Special case when all items are associated with same addendum.
    {
        order_array = [];
        for (var j = 0; j < item_array.length; j++) {
            item_array[j] = $(item_array[j]);                           // Go ahead now and update the array so the elements are JQuery items
            var addendum_id = item_array[j].attr("AddendumID");
            if (addendum_id != "")                                    // Item has addendum
                order_array.push(j);            
        }
    }

    resorted_order_array = order_array.slice();                     // clone the order array for before vs after
    shuffle_array(resorted_order_array);                            // shuffle the after array
    order_array = flatten(order_array);                             // flatten both arrays so addendum items are now lined up together
    resorted_order_array = flatten(resorted_order_array);
    for (var j = 0; j < order_array.length; j++) {                    // loop through the before array
        for (var i = 0; i < resorted_order_array.length; i++) {
            if (order_array[j] == resorted_order_array[i]) {        // find matching element in after array
                // queue up a slide for the selected before item (j) to the position on the screen occupied by after item (i)
                slide_queue.push([item_array[order_array[j]], item_array[i].position().left, item_array[i].position().top]);
                break;
            }
        }
    }
    active_slides = 0;
    SlideThumbnails();                                              // to the left! to the left! now slide baby, slide baby!
}

var slide_queue, resorted_order_array, item_array;
var active_slides;
var max_active_slides = 100;         // number of simultaneous slides allowed, may need to be tweaked once we see performance on slower platforms
//var delays = 0;
function SlideThumbnails() {
    if (active_slides >= max_active_slides) {
        // if we've reached max simultaneous slide max, don't pop off queue, delay
        setTimeout("SlideThumbnails()", 250);
        //delays++;
        return;
    }
    var elm = slide_queue.pop();                // get next item to slide
    if (!elm) {
        return;                                 // should never happen... but just in case
    }
    $element1 = elm[0];                         // load the jquery item to slide, and positions to slide to.
    var position_top = elm[2] - $element1.position().top;
    var position_left = elm[1] - $element1.position().left;
    active_slides++;
    if (slide_queue.length == 0) {
        $element1.animate({ left: position_left, top: position_top }, 750, function () {
            var $sortable = $("#sortable");
            for (var j = 0; j < resorted_order_array.length; j++) {
                // sliding the items is just for show, now we need to reorder the items in the DOM correctly and restore their positioning to auto float
                $sortable.append(item_array[resorted_order_array[j]]);
                item_array[resorted_order_array[j]].css('left', 'auto').css('top', 'auto');
            }
            //alert(delays);
            $("#sortable").sortable("refresh");             // update the sortable
            active_slides--;
            RenumberQuestions();                            // renumber the questions
            var new_order = $("#sortable").sortable('toArray');
            Service2.UpdateItemOrders(getURLParm('xID'), activeForm, new_order.join(','), function (result_raw) {
                orderUpdated(new_order);
                result = jQuery.parseJSON(result_raw);
                if (result.Status == 'Updated Orders Attached') {
                    AssessmentForms = result.PayLoad;
                }
            }, onFailed);      // send the updated orders to the server
        }
                );
    } else {
        $element1.animate({ left: position_left, top: position_top }, 750, function () { active_slides--; });
        setTimeout("SlideThumbnails()", 50);
    }
}

function flatten(oArray) {
    var retVal = [];
    for (var i = 0; i < oArray.length; i++) {
        if (!isArray(oArray[i])) {
            retVal.push(oArray[i]);
        } else {
            var tempFlatt = flatten(oArray[i]);
            for (var j = 0; j < tempFlatt.length; j++) {
                retVal.push(tempFlatt[j]);
            }
        }
    }
    return retVal;
}

function isArray(anElement) {
    return (typeof anElement == "object" && anElement.constructor == Array);
}

function shuffle_array(array) {
    var tmp, current, top = array.length;

    if (top) while (--top) {
        current = Math.floor(Math.random() * (top + 1));
        tmp = array[current];
        array[current] = array[top];
        array[top] = tmp;
    }

    return array;
}

function formTabStrip_OnClientTabSelecting(sender, args) {
    var selected_tab_value = args.get_tab().get_value();
    renderForm(selected_tab_value);
}

var $sortableGrid;

var top_constraint_for_drag_helper, bottom_constraint_for_drag_helper;
function dragHelper(event) {
    //$('.headingToggleImg').html(event.pageY + ':' + bottom_constraint_for_drag_helper);

    if (event.pageY < top_constraint_for_drag_helper) scollUpHelper_Start();
    else if (event.pageY > bottom_constraint_for_drag_helper) scollDownHelper_Start();
    else scrollHelper_Cancel();
}
var scrolling_questions = false;
var scrolling_item;
function scollDownHelper_Start(event) {
    //$('.headingToggle').html(count++);
    scrolling_questions = true;
    setTimeout("scrollHelper(10)", 20);
}
function scollUpHelper_Start(event) {
    //$('.headingToggle').html(count++);
    scrolling_questions = true;
    setTimeout("scrollHelper(-10)", 20);
}
function scrollHelper(offset) {
    if (!scrolling_questions) return;
    $sortableGrid.scrollTop($sortableGrid.scrollTop() + offset);
    setTimeout("scrollHelper(" + offset + ")", 20);
}
function scrollHelper_Cancel(event) {
    scrolling_questions = false;
}


function getThumbnailMovingHelper(event, ui) {
    var dragged_item_id = $(ui[0]).attr("id");
    var checked_count = 0;
    var $multiDragHelper, newItem;
    $('.thumbInstance :checked').each(function (index) {
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
        $multiDragHelper.append("<div class=\"moving_helper_text\">Moving 2</div>");
    } else if (checked_count > 1) {
        // we have 2 other items we're dragging (because visibile limit is 3 total)
        $multiDragHelper.append(newItem = ui.clone().css('position', 'relative').css('left', 20).css('top', -170));
        newItem.find("span").hide();
        $multiDragHelper.append("<div class=\"moving_helper_text\" style=\"left: 50px\">Moving " + (checked_count + 1) + "</div>");
    } else {
        // we are only dragging the one item, straight copy
        $multiDragHelper = ui.clone(true);
        $multiDragHelper.find("span").hide();
        $multiDragHelper.append("<div class=\"moving_helper_text\">Moving 1</div>");
    }


    return $multiDragHelper;
}

function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow) oWindow = window.radWindow;
    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
    return oWindow;
}

function AlertAssessmentWindowOfChanges() {
    var parentPage = GetRadWindow().BrowserWindow;
    parentPage.allItem_refresh_needed = true;
}

function orderUpdated(new_order) {
    shuffle_in_progress = false;
    for (var x = 0; x < AssessmentForms.length; x++) {
        if (AssessmentForms[x].FormId == activeForm) {
            AssessmentForms[x].ItemOrders = new_order;
            break;
        }
    }
    AlertAssessmentWindowOfChanges();
    RenumberQuestions();
}



function onFailed(obj) {
    shuffle_in_progress = false;
    if (obj._statusCode != 0)
        alert(obj._message);
}

function onItemMoved(event, ui) {
    var dragged_item_id = $(ui.item).attr("id");
    var items_to_add = new Array();
    items_to_add.push(dragged_item_id);

    //var item_sort_orders = $("#sortable").sortable('toArray');
    $('.thumbInstance :checked').each(function (index) {
        var $li = $(this).closest('.thumbInstance');
        var item_id = $li.attr('id');
        if (item_id != dragged_item_id) {
            $li.insertAfter($(ui.item));
            //item_sort_orders.splice(item_sort_orders.indexOf(item_id), 1);
            //items_to_add.push(item_id);
        }

        this.checked = false;
    });
    //$("#sortable").sortable('refresh');
    //var item_sort_orders = $("#sortable").sortable('toArray');
    //item_sort_orders.splice(item_sort_orders.indexOf(dragged_item_id), 1, items_to_add);
    var new_order = $("#sortable").sortable('toArray');
    Service2.UpdateItemOrders(getURLParm('xID'), activeForm, new_order.join(','), function (result_raw) {
        orderUpdated(new_order);
        result = jQuery.parseJSON(result_raw);
        if (result.StatusCode == UpdatedDataAttached) {
            AssessmentForms = result.PayLoad;
        }
    }, onFailed);
}

function getURLParm(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

function RenumberQuestions() {
    var count = 1;
    $(".ui-sortable .thumbInstance .sort_number").each(function () {
        /*var div_by_2 = Math.floor(count / 2) + 1;
        $(this).html(div_by_2);*/
        $(this).html(count);
        count++;
    });
}

function mainToolBar_OnClientButtonClicked(sender, args) {
    var selected_button_value = args.get_item().get_value();
    switch (selected_button_value) {
        case "RemoveItems":
            removeItems();
            break;
        case "Print":
            customDialog({ title: "Print Assessment", url: "../Dialogues/Assessment/AssessmentPrint.aspx?xID=" + getURLParm('xID'), maximize: true, maxwidth: 520, maxheight:400 });
            break;
        default:
            alert('Function is in development.');
    }
}

function mainTabStrip_OnClientTabSelecting() {

}

SuppressResizeEvent = false;

$(window).resize(function () {
    if (SuppressResizeEvent) {
        SuppressResizeEvent = false;
        return false;
    }
    changepush(true);
});

function takeFullHeight() {
    changepush(true);

};

function changepush(suppressResizeEvent) {
    SuppressResizeEvent = suppressResizeEvent;
    var wheight = $telerik.$(window).height() + (isTouchDevice() ? -20 : 0); // ipad fix... don't ask me
    //var wheight = $telerik.$(window).height(); // ipad fix... don't ask me
    //alert(wheight);
    var contentHeight = wheight - $('#topPortion').height();
    $('#sortableGrid').height(contentHeight);
    //alert(contentHeight);
    //alert(wheight);
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
                    AlertAssessmentWindowOfChanges();
                    location.reload();
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