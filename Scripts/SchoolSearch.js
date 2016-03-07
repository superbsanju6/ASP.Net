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

function RowSelected(sender, eventArgs) {
    
    var rowIndex = eventArgs.get_itemIndexHierarchical();
    $('.chkRow').each(function () {
        checkboxRowIndex = this.getAttribute("rowIndex");

        if (checkboxRowIndex == rowIndex) {
            this.checked = true;
        }

    });

    var count = countSelectedRows();
    updateSelectedRowCount(count);

    if (count == 0) {
        toggleActionButtons(false);
    }
    else {
        toggleActionButtons(true);
    }

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

    var count = countSelectedRows();
    updateSelectedRowCount(count);

    if (count == 0) {
        toggleActionButtons(false);
    }
    else {
        toggleActionButtons(true);
    }

}

function selectAll(sender, id, totalCount) {

    var radgrid = $find(id);
 
    if (radgrid) {
        var masterTable = radgrid.get_masterTableView();
        if (sender.checked) {
            masterTable.selectAllItems();
            $('.chkRow').attr('checked', 'checked')
            updateSelectedRowCount(totalCount);
            if (totalCount != 0) {
                toggleActionButtons(true);
            }
        }
        else {
            toggleActionButtons(false);

            masterTable.clearSelectedItems();
            $('.chkRow').attr('checked', '')
            updateSelectedRowCount(0);
        }
    }

}

function selectThisRow(sender, id, rowIndex) {
    var radgrid = $find(id);
    
    if (radgrid) {
        var masterTable = radgrid.get_masterTableView();
        if (sender.checked) {
            masterTable.selectItem(rowIndex);
            toggleActionButtons(true);
            
        }
        else {
            if (countSelectedRows() == 0) {
                toggleActionButtons(false);
            }

            masterTable.deselectItem(rowIndex);

        }
        updateSelectedRowCount(countSelectedRows());


    }


}

function countSelectedRows() {
    var count = 0;
    $(".chkRow").each(function () {
        if (this.checked)
            count = count + 1;
    });
    return count;

}

function toggleActionButtons(state) {
    $find('radBtnLock').set_enabled(state);
    $find('radBtnUnlock').set_enabled(state);
    $find('radBtnActivate').set_enabled(state);
    $find('radBtnDeactivate').set_enabled(state);
}

function updateSelectedRowCount(count) {
    var text = "";

    if (count == 0)
        text = "";
    else
        text = 'Records selected: ' + count;

    if ($('#selectedRecordsDiv')) $('#selectedRecordsDiv').text(text);

}

function GridCreated() {
    var RadGrid = this;

} 