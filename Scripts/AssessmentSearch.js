var checked = [];   //To store checked item ids
var unchecked = []; //To store unchecked item ids
var isCheckEvent = false;

function findSelectedAssessments(id) {

    var items = $find(id).get_selectedItems();

    var assessmentList = "";
    var i = 0;

    $(items).each(function () {
        i++;
        assessmentList += this.findControl("hiddenEncryptedID").get_value();
        if (i < items.length) {
            assessmentList += ",";
        }

    });
    return assessmentList;
}

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
    var radgrid = window.$find(id);
    
    if (radgrid) {
        var masterTable = radgrid.get_masterTableView();
        var i;
        var chks;
        if (sender.checked) {
            chks = masterTable.get_dataItems();
            for (i = 0; i < chks.length; i++)
            {
                var chkBox = masterTable.get_dataItems()[i].findElement("chkRowInput");
                if (!chkBox.disabled)
                    chkBox.checked = true;
            }

            
            $('#hiddenChkAll').val('1');
            $('#hiddenSelectedCount').val(totalCount);
        }
        else {
            chks = masterTable.get_dataItems();
            for (i = 0; i < chks.length; i++) {
                masterTable.get_dataItems()[i].findElement("chkRowInput").checked = false;
            }
            
            $('#hiddenChkAll').val('0');
            $('#hiddenSelectedCount').val('0');
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

function selectThisRow(sender, id, rowIndex, totalCount, testID) {
    isCheckEvent = true;
    var radgrid = $find(id);

    if (radgrid) {
        if ($('#hiddenDeSelected').val() != "") {
            unchecked = $('#hiddenDeSelected').val().split(',');
        }

        if ($('#hiddenSelected').val() != "") {
            checked = $('#hiddenSelected').val().split(',');
        }

        var checkedFound = jQuery.inArray(testID.toString(), checked);
        var unCheckedFound = jQuery.inArray(testID.toString(), unchecked);

        var masterTable = radgrid.get_masterTableView();
        if (sender.checked) {
            masterTable.selectItem(rowIndex);

            // PUSH item into CheckedList
            checked.push(testID.toString());

            // POP item from UnCheckedList
            if (unCheckedFound >= 0)
                unchecked.splice(unCheckedFound, 1);

            var selectedRecords = $('#hiddenSelectedCount').val() == "" ? 0 : parseInt($('#hiddenSelectedCount').val());
            $('#hiddenSelectedCount').val(selectedRecords + 1);
        }
        else {
            masterTable.deselectItem(rowIndex);

            // POP item from CheckedList
            if (checkedFound >= 0)
                checked.splice(checkedFound, 1);

            // PUSH item into UnCheckedList
            unchecked.push(testID.toString());

            var selectedRecords = $('#hiddenSelectedCount').val() == "" ? 0 : parseInt($('#hiddenSelectedCount').val());
            $('#hiddenSelectedCount').val(selectedRecords - 1);
        }

        $('#hiddenSelected').val(checked);
        $('#hiddenDeSelected').val(unchecked);
        $('#hiddenTotalCount').val(totalCount);

        updateSelectedRowCount();
    }
    isCheckEvent = false;

}

function toggleActionButtons(state) {
    if ($find('radBtnLock') != null) {
        $find('radBtnLock').set_enabled(state);
    }
    if ($find('radBtnUnlock') != null) {
        $find('radBtnUnlock').set_enabled(state);
    }
    if ($find('radBtnActivate') != null) {
        $find('radBtnActivate').set_enabled(state);
    }
    if ($find('radBtnDeactivate') != null) {
        $find('radBtnDeactivate').set_enabled(state);
    }
    if ($find('radBtnViewTestEvents') != null) {
        $find('radBtnViewTestEvents').set_enabled(state);
    }
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

function GridCreated() {
    var RadGrid = this;

}