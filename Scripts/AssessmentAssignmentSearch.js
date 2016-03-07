// Bug #7044: Show "Enable/ Disable Online" icons for school portal regardless of the security settings
var checked = [];   //To store checked item ids
var unchecked = []; //To store unchecked item ids
var isCheckEvent = false;

function RowClicked(sender, args) {
    if (isCheckEvent == false)
        args.set_cancel(true);
}

function RowSelected(sender, eventArgs) {

    var rowIndex = eventArgs.get_itemIndexHierarchical();
    $('.chkRow').each(function () {
        checkboxRowIndex = this.getAttribute("rowIndex");

        if (checkboxRowIndex == rowIndex) {
            this.checked = true;
        }

    });

}

function RowDeselected(sender, eventArgs) {

    var rowIndex = eventArgs.get_itemIndexHierarchical();
    var checkBoxElement = eventArgs.get_item();
    var gridID = sender.get_id();

    $('.chkRow').each(function () {
        checkboxRowIndex = this.getAttribute("rowIndex");

        if (checkboxRowIndex == rowIndex) {
            this.checked = false;
        }

    });

}

function selectAll(sender, id, totalCount) {
    isCheckEvent = true;
    var radgrid = $find(id);
    var $radGrid = $("#" + id);
    if ($radGrid) {
        if (!$radGrid.data("selectedTotalCount")) {
            $radGrid.data("selectedTotalCount", 0);
        }
    }

    if (radgrid) {
        var masterTable = radgrid.get_masterTableView();
        if (sender.checked) {
            masterTable.selectAllItems();

            $('#hiddenChkAll').val('1');
            $('#hiddenSelectedCount').val(totalCount);
            $('#hidSelectedTestEventIDs').val($('#hiddenListOfTestEventIDs').val());
            $radGrid.data("selectedTotalCount", totalCount);
        }
        else {
            masterTable.clearSelectedItems();

            $('#hiddenChkAll').val('0');
            $('#hiddenSelectedCount').val('0');
            $('#hidSelectedTestEventIDs').val('');
            $radGrid.data("selectedTotalCount", 0);
        }

        checked.splice(0, checked.length);
        unchecked.splice(0, unchecked.length);
        $('#hiddenSelected').val('');
        $('#hiddenDeSelected').val('');
        $('#hiddenTotalCount').val(totalCount);

        updateSelectedRowCount();
    }
    isCheckEvent = false;

}

function selectThisRow(sender, id, rowIndex, totalCount, testID, classID, testeventid) {
    isCheckEvent = true;
    var radgrid = $find(id);
    var $radGrid = $("#" + id);
    if ($radGrid) {
        if (!$radGrid.data("selectedTotalCount")) {
            $radGrid.data("selectedTotalCount", $('#hiddenSelectedCount').val());
        }
    }
    if (radgrid) {
        if ($('#hiddenDeSelected').val() != "") {
            unchecked = $('#hiddenDeSelected').val().split(',');
        }

        if ($('#hiddenSelected').val() != "") {
            checked = $('#hiddenSelected').val().split(',');
        }

        var checkedFound = jQuery.inArray(testID.toString() + '-' + classID.toString(), checked);
        var unCheckedFound = jQuery.inArray(testID.toString() + '-' + classID.toString(), unchecked);

        var masterTable = radgrid.get_masterTableView();
        if (sender.checked) {
            masterTable.selectItem(rowIndex);

            // PUSH item into CheckedList
            checked.push(testID.toString() + '-' + classID.toString());

            AddRemoveFromTestList(testeventid.toString(), true);

            // POP item from UnCheckedList
            if (unCheckedFound >= 0)
                unchecked.splice(unCheckedFound, 1);
            var selectedRecords = masterTable.get_selectedItems();
            if ($radGrid.data("selectedTotalCount") != "") {
            $radGrid.data("selectedTotalCount", parseInt($radGrid.data("selectedTotalCount")) + 1);
            }
            else
            {
                $radGrid.data("selectedTotalCount", 1);
            }
            //var selectedRecords = $('#hiddenSelectedCount').val() == "" ? 0 : parseInt($('#hiddenSelectedCount').val());
            //$('#hiddenSelectedCount').val(selectedRecords.length);
            $('#hiddenSelectedCount').val($radGrid.data("selectedTotalCount"));
        }
        else {
            masterTable.deselectItem(rowIndex);

            // POP item from CheckedList
            if (checkedFound >= 0)
                checked.splice(checkedFound, 1);

            AddRemoveFromTestList(testeventid.toString(), false);

            // PUSH item into UnCheckedList
            unchecked.push(testID.toString() + '-' + classID.toString());
            var selectedRecords = masterTable.get_selectedItems();
            $radGrid.data("selectedTotalCount", parseInt($radGrid.data("selectedTotalCount")) - 1);
            //var selectedRecords = $('#hiddenSelectedCount').val() == "" ? 0 : parseInt($('#hiddenSelectedCount').val());
            //$('#hiddenSelectedCount').val(selectedRecords.length);
            $('#hiddenSelectedCount').val($radGrid.data("selectedTotalCount"));
        }

        $('#hiddenSelected').val(checked);
        $('#hiddenDeSelected').val(unchecked);
        $('#hiddenTotalCount').val(totalCount);

        updateSelectedRowCount();
    }
    isCheckEvent = false;

}

function AddRemoveFromTestList(testeventid, isAdd) {
    if (isAdd == true) {
        if ($('#hidSelectedTestEventIDs').val().indexOf(testeventid) < 0) {
            if ($('#hidSelectedTestEventIDs').val().length > 0) {
                $('#hidSelectedTestEventIDs').val($('#hidSelectedTestEventIDs').val() + "," + testeventid);
            }
            else {
                $('#hidSelectedTestEventIDs').val($('#hidSelectedTestEventIDs').val() + testeventid);
            }
        }
    }
    else {
        if ($('#hidSelectedTestEventIDs').val().indexOf(testeventid) >= 0) {
            var newList = $('#hidSelectedTestEventIDs').val();
            newList = newList.replace("," + testeventid + ",", ",").replace(testeventid + ",", "").replace("," + testeventid, "");
            $('#hidSelectedTestEventIDs').val(newList);
        }
    }
}

function toggleActionButtons(state) {
    if ($find('imgPrintBubble') != null) {
    $find('imgPrintBubble').set_enabled(state);
    }
    if ($find('radBtnEnable') != null) {
    $find('radBtnEnable').set_enabled(state);
    }
    if ($find('radBtnDisable') != null) {
    $find('radBtnDisable').set_enabled(state);
    }

    $('.chkRow').each(function () {
        if ($(this).attr('checked') && $(this).attr('assessmentSecurity') == 'Inactive' && $(this).attr('currentPortal') == 'Teacher') {
            if ($find('radBtnEnable') != null) {
            $find('radBtnEnable').set_enabled(false);
            }
            if ($find('radBtnDisable') != null) {
            $find('radBtnDisable').set_enabled(false);
        }
        }
    });
}

function updateSelectedRowCount() {
    var selectedRecords = parseInt($('#hiddenSelectedCount').val());
    var TotalRecords = parseInt($('#hiddenTotalCount').val());

    // Checked 'Select All' checkbox, if only, all records are selected
    if (TotalRecords != 0)
        $('#chkAll').attr('checked', selectedRecords == TotalRecords);

    // Enable buttons, if only, atleast one record is selected
    toggleActionButtons(selectedRecords != 0);

    // Display selected records count
    if ($('#selectedRecordsDiv'))
        $('#lblSelectedCount').text('Records selected: ' + selectedRecords.toString());
    else
        $('#lblSelectedCount').text('');

}


function printBubbleSheets() {
    //    var cteIdEle = document.getElementById("inpCteID");
    //    var cteId = cteIdEle.value;

    //    var studentIdArray = getSelectedStudentIds();
    //    var studentIds = "";
    //    for (var i = 0; i < studentIdArray.length; i++) {
    //        studentIds += (i > 0) ? "," : "";
    //        studentIds += studentIdArray[i];
    //    }
    //    //			alert('print bubble ' + assessmentID + " " + classID + " " + studentIds);
    //    customDialog({ url: ('AssessmentPrintBubbleSheets.aspx?xID=' + cteId + '&yID=' + escape(studentIds)), autoSize: true });
}
