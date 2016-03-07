var aSelected = [];
var selectedNodeId;
var unSelectedNodeId;
var selectedNodeDesc;
var selectedNodeName;

function tgInitDataTable(tableID) {
    if (objExists(tableID)) {
        /* Init the table */
        if (tableID != "tblAddExistingCompetencyList") {
        var oTable = $('#' + tableID).dataTable({
            "oLanguage": { "sSearch": "Search:" },
            "iDisplayLength": 10,
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",
            "sScrollY": 350,
            "aaSorting": [[0, "asc"]]
        });
        }
        else {            
                var oTable = $('#' + tableID).dataTable({                                                                                
                    "iDisplayLength": 10,
                    "sPaginationType": "full_numbers",
                    "sScrollY": 300,
                    "aaSorting": [[0, "asc"]],
                    "bFilter": false,
                    "bLengthChange": false                    
                });
    }
}
}


function tgMakeTableSelectable(tableID) {
    if (objExists(tableID)) {
        /* Add a click handler to the table rows - this could be used as a callback */
        $('#' + tableID + ' tr').click(function () {
                if (tableID != "tblAddExistingCompetencyList") {
            if ($(this).hasClass('row_selected')) {
                unSelectedNodeId = $(this)[0].cells[1].textContent;
                $(this).removeClass('row_selected');
            } else {
                selectedNodeId = $(this)[0].cells[1].textContent;
                $(this).addClass('row_selected');
            }
                }
                 else {
                    j$('#' + tableID + ' tr').removeClass("row_selected");
                    selectedNodeId = j$(this)[0].cells[0].textContent;
                    selectedNodeName = j$(this)[0].cells[2].textContent;
                    selectedNodeDesc = j$(this)[0].cells[3].textContent;
                    j$('input[name=selectedNodeDesc]').val(selectedNodeDesc);
                    j$('input[name=selectedNodeName]').val(selectedNodeName);                    
                    j$(this).addClass('row_selected');
                  }
            fnGetSelected(tableID);
        });
    }
}

function fnGetSelected(tableID) {
    var aReturn = new Array();
        if (tableID != "tblAddExistingCompetencyList") {
    var aTrs = $(".row_selected").children();
        } else {
            var aTrs = j$(".row_selected").children();
        }

        
    if (objExists(aTrs) && aTrs.length > 0) {
        if (tableID == "currentStandardsDataTable") {
            aReturn = getSelectedColumnValues_currentStandardsDataTable(aTrs);
        } else {
            if (tableID == "availableStandardsDataTable") {
                aReturn = getSelectedColumnValues_availableStandardsDataTable(aTrs);
            } else {
                if (tableID == "currentCurriculaDataTable") {
                    aReturn = getSelectedColumnValues_currentCurriculaDataTable(aTrs);
                } else {
                    if (tableID == "availableCurriculaDataTable") {
                        aReturn = getSelectedColumnValues_availableCurriculaDataTable(aTrs);
                    } else {
                            if (tableID == "tblAddExistingPlans" || tableID == "tblAddExistingCompetencyList") {
                            aReturn = getSelectedColumnValues_availableModelCurriculum(aTrs);
                        }
                    }
                }
            }
        }
    }

    var selectedNodeIdIndex = jQuery.inArray(selectedNodeId, aSelected);
    if (selectedNodeIdIndex === -1) {
        aSelected.push(selectedNodeId);
    } 
         
    var unSelectedNodeIdIndex = jQuery.inArray(unSelectedNodeId, aSelected);
    if (unSelectedNodeIdIndex !== -1) {
        aSelected.splice(unSelectedNodeIdIndex, 1);
    }
        if (tableID == "tblAddExistingCompetencyList") { aSelected = []; aSelected.push(selectedNodeId);}
    $('input[name=SelectedItems]').val(aSelected + "");
    return aSelected;
}

function getSelectedColumnValues_availableStandardsDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var planNode = aTrs[i].textContent;
        var documentName = aTrs[i + 1].textContent

        aReturn.push(planNode);
        i = i + nbrcols;
    }
    return aReturn;
}

function getSelectedColumnValues_currentStandardsDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var tblid = aTrs[i].textContent;

        aReturn.push(tblid);
        i = i + nbrcols;
    }
    return aReturn;
}


function getSelectedColumnValues_currentCurriculaDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var tblid = aTrs[i].textContent;

        aReturn.push(tblid);
        i = i + nbrcols;
    }
    return aReturn;
}


function getSelectedColumnValues_availableCurriculaDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var tblid = aTrs[i].textContent;

        aReturn.push(tblid);
        i = i + nbrcols;
    }
    return aReturn;
}

function getSelectedColumnValues_availableModelCurriculum(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var parid = aTrs[i].textContent;
        var tblid = aTrs[i + 1].textContent;

        aReturn.push(tblid);
        i = i + nbrcols;
    }
    return aReturn;
}

function getSelectedColumnValues_tblYourTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var parentNode = aTrs[i].textContent;
        var planNode = aTrs[i + 1].textContent;
        var documentName = aTrs[i + 2].textContent

        aReturn.push(planNode + "|" + documentName);
        i = i + nbrcols;
    }
    return aReturn;
}



function showAddNewDocumentModal(doctype, doctypelbl, parentid, classid, ctrlid) {

    var rbgrp = ctrlid + "-" + parentid + "_tgAddNewDoc";
    var dialogid = ctrlid + "-" + parentid + "-dialog-confirm";

    $("#" + dialogid).dialog({
        resizable: false,
        height: 170,
        modal: true,
        title: "Add New " + doctypelbl,
        buttons: {
            OK: function () {
                var rbval = "";
                rbval = $('input[name=' + rbgrp + ']:checked').val();
                if (rbval == "new") {
                    showAddNewPlan(doctypelbl, parentid, classid);
                } else if (rbval == "existing") {
                    showAddExistingPlan(doctype, doctypelbl, parentid);
                } else {
                    // do nothing
                }

                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

}

function showAddNewPlan(doctypelbl, parentid, classid) {
    var url = '/' + kenticoVirtualPath + '/CMSModules/Content/CMSDesk/Edit/Edit.aspx?action=new&classid=' + classid + '&parentnodeid=' + parentid + '&culture=en-US';
    $('#addNew-dialog-content')[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=600 frameborder=0 >";

    $("#addNew-dialog").dialog({
        resizable: false,
        height: 680,
        width: 1020,
        title: "Add New " + doctypelbl,
        resizable: true,
        bgiframe: true,
        modal: true
    });

}

function showAddExistingPlan(doctype, doctypelbl, parentid) {
    var url = '/' + kenticoVirtualPath + '/CMSWebParts/ThinkgateWebParts/AddExistingSearch.aspx?doctype=' + doctype + '&parentnodeid=' + parentid;
    $('#addExisting-dialog-content')[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=600 frameborder=0 >";

    $("#addExisting-dialog").dialog({
        height: 680,
        width: 1020,
        title: "Copy Existing " + doctypelbl,
        resizable: true,
        bgiframe: true,
        modal: true,
        dialogClass: "tgAddExistingDialog"
    });


}



function closeAddExistingDialog() {
    try {
        //window.parent.jQuery('.tgAddExistingDialog').children(".ui-dialog-titlebar").children("button").click();
        closeCurrentCustomDialog();
    } catch (ex) { alert(ex) }
    return false;
}