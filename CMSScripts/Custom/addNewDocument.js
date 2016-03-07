function tgInitDataTable(tableID) {
	if (objExists(tableID)) {
		/* Init the table */
		var oTable = $j('#' + tableID).dataTable({
			"oLanguage": { "sSearch": "Search:" },
			"iDisplayLength": 10,
			"bJQueryUI": true,
			"sPaginationType": "full_numbers",
			"sScrollY": 350,
			"aaSorting": [[0, "asc"]]
		});
	}
}


function tgMakeTableSelectable(tableID) {
	if (objExists(tableID)) {
		/* Add a click handler to the table rows - this could be used as a callback */
		$j('#' + tableID + ' tr').click(function () {
			if ($j(this).hasClass('row_selected')) {
				$j(this).removeClass('row_selected');
			} else {
				$j(this).addClass('row_selected');
			}
			fnGetSelected(tableID);
		});
	}
}

function fnGetSelected(tableID) {
	var aReturn = new Array();
	var aTrs = $j(".row_selected").children();
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
					} else
					    if (tableID == "currentResourcesDataTable") {
					        aReturn = getSelectedColumnValues_currentResourceDataTable(aTrs);
					    } else {
					        if (tableID == "availableResourcesDataTable") {
					            aReturn = getSelectedColumnValues_availableResourceDataTable(aTrs);
					        } else
					            if (tableID == "tblAddExistingPlans") {
							        aReturn = getSelectedColumnValues_availableModelCurriculum(aTrs);
						        }
					}
				}
			}
		}
	}
	
	$j('input[name=SelectedItems]').val(aReturn + "");
	return aReturn;
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

function getSelectedColumnValues_currentResourceDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var tblid = aTrs[i].textContent;

        aReturn.push(tblid);
        i = i + nbrcols;
    }
    return aReturn;
}

function getSelectedColumnValues_availableResourceDataTable(aTrs) {
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

	$j("#" + dialogid).dialog({
		resizable: false,
		height: 170,
		modal: true,
		title: "Add New " + doctypelbl,
		buttons: {
			OK: function () {
				var rbval = "";
				rbval = $j('input[name=' + rbgrp + ']:checked').val();
				if (rbval == "new") {
					showAddNewPlan(doctypelbl, parentid, classid);
				} else if (rbval == "existing") {
					showAddExistingPlan(doctype, doctypelbl, parentid);
				} else {
					// do nothing
				}

				$j(this).dialog("close");
			},
			Cancel: function () {
				$j(this).dialog("close");
			}
		}
	});

}

function showAddNewPlan(doctypelbl, parentid, classid) {
    var url = kenticoVirtualPath + '/CMSModules/Content/CMSDesk/Edit/Edit.aspx?action=new&classid=' + classid + '&parentnodeid=' + parentid + '&culture=en-US';
	$j('#addNew-dialog-content')[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=600 frameborder=0 >";

	$j("#addNew-dialog").dialog({
		resizable: false,
		//maxheight: 680,
		//maxwidth: 1020,
        maximize: true,
		title: "Add New " + doctypelbl,
		resizable: true,
		bgiframe: true,
		modal: true,
		close: function (event) {
		        window.location.reload(true);
		    }
	});

}

function showAddExistingPlan(doctype, doctypelbl, parentid) {
    var url = kenticoVirtualPath + '/CMSWebParts/ThinkgateWebParts/AddExistingSearch.aspx?doctype=' + doctype + '&parentnodeid=' + parentid;
	$j('#addExisting-dialog-content')[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=600 frameborder=0 >";

	$j("#addExisting-dialog").dialog({
		//maxheight: 680,
		//maxwidth: 1020,
        maximize: true,
		title: "Copy Existing " + doctypelbl,
		resizable: true,
		bgiframe: true,
		modal: true,
		dialogClass: "tgAddExistingDialog",
		close: function (event) {
		    window.location.reload(true);
		    }
	});

}



function closeAddExistingDialog() {
	try {
		window.parent.jQuery('.tgAddExistingDialog').children(".ui-dialog-titlebar").children("button").click();
	} catch (ex) { alert(ex) }
	return false;
}

function tgSelectAll(tableID) {
    if (objExists(tableID)) {
        if ($j('#chkSelectAll').is(':checked')) {
            $j('#' + tableID + ' tr').each(function () {
                $j(this).addClass('row_selected');
            });
        }
        else {
            $j('#' + tableID + ' tr').each(function () {
                $j(this).removeClass('row_selected');
            });
        }
        fnGetSelected(tableID);


    }
}