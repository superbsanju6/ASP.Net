
var $ = $telerik.$;
var uploadsInProgress = 0;
var cursorPosition = null;
var suppress_tool_timer = true;
var oItemKeyValueColl;
var questionType = '';
function mainToolBar_OnClientLoad() {
    undoList = new UndoList();
}

var UNDO_TYPE = {
    QUESTION_TEXT: 0,
    ANSWER_TEXT: 0
};

function ReloadCkeditorForSingleItem(contentEditableIds) {
	if (contentEditableIds) {

		if (CKEDITOR.instances) {
			for (name in CKEDITOR.instances) {
				CKEDITOR.instances[name].destroy();
			}
		}

		for (var i = 0; i < contentEditableIds.length; i++) {
			CKEDITOR.inline(contentEditableIds[i].id);
		}
	}
}

/********* ItemKeyValueCollection Class ****************************************************************/

function ItemKeyValueCollection() {
    this.ItemID = "";
    this.EncAssessmentID = "";
    this.Attributes = new Array();
}

ItemKeyValueCollection.prototype.ClearAttributes = function () {
    this.Attributes = null;
    this.Attributes = new Array();
};

ItemKeyValueCollection.prototype.Add = function (oKeyValue) {
    this.Attributes.push(oKeyValue);
};

ItemKeyValueCollection.prototype.ToJSON = function () {
    return JSON.stringify(this);
};

/***************************************************************************************************/

function GetKeyValueFromCtrl(oCtrl) {
    return { key: oCtrl.attributes["field"].value, val: oCtrl.control._value };
}

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
            if ($bodyTextContainer.length == 0) {
                alert('Unable to undo. Item text not found.');
                return;
            }
            if (target_undo.IsItemBeingEdited()) { // if the text editor is currently open and editing the text we are about to rollback, prevent because it will just get confusing and complicated
                this.list.push(target_undo); // requeue
                return;
            }

            $bodyTextContainer.html(target_undo.Old_value); // set item text back to old text

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
};

function onFileSelected(sender, args) {
    if (!uploadsInProgress)
        $("#SaveButton").attr("disabled", "disabled");

    uploadsInProgress++;
}

function onFileUploaded(sender, args) {
    decrementUploadsInProgress();
}

function onUploadFailed(sender, args) {
    decrementUploadsInProgress();
}

function decrementUploadsInProgress() {
    uploadsInProgress--;

    if (!uploadsInProgress)
        $("#SaveButton").removeAttr("disabled");
}

//<![CDATA[

function validationFailed(sender, eventArgs) {
    $(".ErrorHolder").append("<p>Validation failed for '" + eventArgs.get_fileName() + "'.</p>").fadeIn("slow");
}

//]]>

function OpenPrintWindow() {
    var a = document.getElementById('divTestQ').innerHTML;
    var divEncItemID = document.getElementById('divEncItemID').innerHTML;
    var xt = a.length > 1 ? '&TestQuestion=' + a : '';
    window.open(appClient + 'Record/RenderItemAsPDF.aspx?xID=' + divEncItemID + xt, 'print');
}

function CopyItem() {
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    if (AssessmentID.length > 5) {
        //var divItemID = document.getElementById('divItemID').innerHTML;
        //removeItem_confirmCallback(divItemID);
    } else {
        var divEncItemID = document.getElementById('divEncItemID').innerHTML;
        Service2.CopyItemToUserPersonalBank(divEncItemID, function (result_raw) {
            document.location.href = 'ContentEditor_Item.aspx?xID=' + result_raw + '&isCopy=true';
        }, copyItemBank_CallBack_Failure);
    }
    return false;
}


function copyItemBank_CallBack_Failure() {

}

function AlertForCopiedItem() {
    var x = setTimeout(AlertForCopiedItemDialog, 700);
}

function AlertForCopiedItemDialog() {
    try {
        customDialog({ title: "Copy Item", maximize: true, maxwidth: 500, maxheight:100, autoSize: false, content: "This is a copy of the previously viewed item.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Ok" }]);
    } catch (e) {

    }
}
var isItemDeleted = false;
function mainToolBar_OnClientButtonClicked(sender, args) {
    var selected_button_value = args.get_item().get_value();
    switch (selected_button_value) {
        case "DeleteItem":
            customDialog({ title: "Delete Item?", maxwidth: 500, maxheight: 125, maximize: true, autoSize: false, content: "You are about to delete this Item. Do you wish to continue?<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel" }, { title: "Ok", callback: deleteItem_confirmCallback }]);
            break;
        case "PrintItem":
            OpenPrintWindow();
            break;
        case "AddendumInsert":
            //InsertAddendum(6, 'The Donkey and the Load of Salt', 'passage', 'Folktale');
            break;
        case "OnlinePreview":
            onLinePreviewClick();
            break;
        case "CopyItem":
            CopyItem();
            break;
        case "Undo":
            undoList.Rollback();
            break;
        case "Tags":
            customDialog({ title: "Tag Associations", url: "../../../Dialogues/Assessment/ItemTags.aspx?xID=" + getURLParm('xID') + "&senderPage=content&qType=" + questionType, maximize: true, width: 1000, height: 640 });
            break;
        default:
            alert('Function is in development.');
    }
}

function setQuestionType(qtr) {
    questionType = qtr;
}

String.prototype.trimString = function () {
    return this.replace(/^\s+|\s+$/g, "");
}

function deleteItem_confirmCallback() {
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    var divItemID = document.getElementById('divItemID').innerHTML;
    if (AssessmentID.length > 5) {
        removeItem_confirmCallback(divItemID);
        isItemDeleted = true;
    } else {
        var divEncItemID = document.getElementById('divEncItemID').innerHTML;
        Service2.DeleteBankQuestionFromDatabase(divEncItemID, deleteItemBank_CallBack_Success, deleteItemBank_CallBack_Failure);
        isItemDeleted = true;
    }
    
    return false;
}

function deleteItemBank_CallBack_Success() {
    if (parent) {
        if (parent.ItemPage_ClosePage) {
            parent.ItemPage_ClosePage();
        }
    }
    closeCurrentCustomDialog();
}

function deleteItemBank_CallBack_Failure() {

}

function removeItem_confirmCallback(item_being_deleted) {
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    Service2.RemoveAssessmentItems(AssessmentID, item_being_deleted, function (result_raw) {
        result = jQuery.parseJSON(result_raw);
        if (result.StatusCode > 0) {
            //assessment_changeField_event_onFailure(null, result.ErrorMessage);
            return;
        }
        try {
            parent.removeItem_FromEditor(item_being_deleted, result_raw);
        } catch (e) {

        }
        closeCurrentCustomDialog();

    }, removeItem_onFailed);
}

function removeItem_onFailed() {

}

function deleteAddendum_onSuccess() {

}

function deleteAddendum_onFailure() {

}


function updateItemBanks(obj) {

    var checks = $j("[type='checkbox'][checked='true'][targetType='5'].itemBankUpdate")
    var inBank = $j("[type='checkbox'][inBank='true'][targetType='5'].itemBankUpdate")
    var itemBanks = '';

    if (obj.getAttribute('targetType') == '5') {
        if (obj.checked) {
            if (obj.getAttribute('multiBanks') == 'false') {
                for (var i = 0; i < itemBankList.length; i++) {
                    document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = (itemBankList[i][0] == '5' && obj.getAttribute('target') == itemBankList[i][1]) ? true : false;
                    document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = (itemBankList[i][0] == '5') ? false : true;
                }
            } else {
                for (var i = 0; i < itemBankList.length; i++) {
                    if (itemBankList[i][0] != '5') {
                        document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = false;
                        document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = true;
                    } else {
                        if (itemBankList[i][3] == 'false') {
                            document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = false;
                        } else {
                            if (itemBankList[i][2] == 'true' || obj.getAttribute('target') == itemBankList[i][1]) {
                                document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = true;
                            }
                        }
                    }
                }
            }
        } else {
            for (var i = 0; i < itemBankList.length; i++) {
                if (checks.length != 0 || inBank.length != 0) {
                    if (itemBankList[i][0] < '5') {
                        document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = false;
                        document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = true;
                    } else {
                        if (checks.length < 1) {
                            document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = (itemBankList[i][2] == 'true') ? true : false;
                        }
                    }
                } else {
                    document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = (itemBankList[i][2] == 'true') ? true : false;
                    document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = (itemBankList[i][0] == 2) ? true : false;
                }
            }
        }
    }

    for (var i = 0; i < itemBankList.length; i++) {
        itemBanks += (document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked) ? ',' + itemBankList[i][0] + ':' + itemBankList[i][1] : '';
    }
    if (itemBanks != '') {
        //alert(itemBanks.substring(1));
        Item_changeItemBank();
    }
}


function Item_changeItemBank() {
    var updatestring = '';
    var iBank = $j("[type='checkbox'][cbtype='ItemBank']");
    for (var x = 0; x < iBank.length; x++) {
        if (iBank[x].checked) {
            updatestring += '' + iBank[x].getAttribute('TargetType') + '@';
            updatestring += '' + iBank[x].getAttribute('Target') + '|';
        }

    }
    updatestring += '';

    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    if (AssessmentID.length > 2) {
        alert('Assessment items do not have item banks, nothing to update.');
        return false;
    }
    if (updatestring.length > 5) {
        //alert(updatestring);
        var ID = document.getElementById('divEncItemID').innerHTML;
        Service2.UpdateItem_ItemBank(ID, 1, updatestring, UpdateAddendum_ItemBank_onSuccess, UpdateAddendum_ItemBank_onFailure);
    }
}

function UpdateAddendum_ItemBank_onSuccess() {

}

function UpdateAddendum_ItemBank_onFailure() {

}

function RemoveRubric() {
    customDialog({ title: "Remove Rubric?", maximize: true, maxwidth: 500,maxheight: 125, autoSize: false, content: "You are about to remove this rubric. Do you wish to continue?<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel" }, { title: "Ok", callback: removeRubric_confirmCallback }]);
}

function removeRubric_confirmCallback() {

    var keyVal = { key: "RubricID", val: 0 };
    oItemKeyValueColl.Add(keyVal);

    AssessmentItem_Change_Key_Value_Info();

    AssessmentItem_Change_ItemWeight("Normal");

    document.getElementById('trRubric_Yes').style.display = 'none';
    document.getElementById('trRubric_No').style.display = '';
}

function RemoveAddendum() {
    customDialog({
        title: "Remove Addendum?",
        maxwidth: 500,
        maxheight: 125,
        maximize: true,
        autoSize: false, content: "You are about to remove this addendum. Do you wish to continue?<br/><br/>", dialog_style: 'confirm'
    }, [{ title: "Cancel" }, { title: "Ok", callback: deleteAssessment_confirmCallback }]);
}

function deleteAssessment_confirmCallback() {
    assessment_changeField('AddendumID', '0');
    document.getElementById('trAddendum_Yes').style.display = 'none';
    document.getElementById('trAddendum_No').style.display = '';
}

function Assessment_ChangeField_ItemType(sender, args) {
        AlertHostWindowOfChanges(oItemKeyValueColl.ItemID);

        __doPostBack('cmbItemType', '');
        return true;
    }

function msieversion() {
    var ua = window.navigator.userAgent
    var msie = ua.indexOf("MSIE ")

    if (msie > 0)      // If Internet Explorer, return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf(".", msie)))
    else                 // If another browser, return 0
        return 0

}

function sleep(seconds) {
    var e = new Date().getTime() + (seconds * 1000);
    while (new Date().getTime() <= e) { }
}

function Assessment_ChangeField_ScoreType(sender, args) {

    oItemKeyValueColl.ClearAttributes();
    oItemKeyValueColl.Add(GetKeyValueFromCtrl($("#cmbScoreType")[0]));

    var value = args.get_item().get_value();

    //if R then display rubric type, rubric points rubric panel,
    if (value == 'R') {


        //Set rubric info to default 2-point rubric
        keyVal = GetKeyValueFromCtrl($("#cmbRubricType")[0]);
        keyVal.val = "B";
        oItemKeyValueColl.Add(keyVal);

        keyVal = GetKeyValueFromCtrl($("#cmbRubricScoring")[0]);
        keyVal.val = "2";
        oItemKeyValueColl.Add(keyVal);

        keyVal = { key: 'RubricID', val: "0" };
        oItemKeyValueColl.Add(keyVal);

        //Set controls to default values also.
        var cmbCtrl, cmbItem;

        cmbCtrl = $find("cmbRubricType");
        if (cmbCtrl._selectedItem) cmbCtrl._selectedItem.set_selected(false);
        cmbItem = cmbCtrl.findItemByValue('B');
        if (cmbItem) {
            cmbItem.set_selected(true);
            cmbCtrl.set_selectedItem(cmbItem);
        }

        cmbCtrl = $find("cmbRubricScoring");
        if (cmbCtrl._selectedItem) cmbCtrl._selectedItem.set_selected(false);
        cmbItem = cmbCtrl.findItemByValue('2');
        if (cmbItem) {
            cmbItem.set_selected(true);
            cmbCtrl.set_selectedItem(cmbItem);
        }

        //Show and hide controls
        $("#divRubricTypeLabel")[0].style.display = "inline";
        $("#divRubricScoringLabel")[0].style.display = "inline";
        $("#pnlItem_Rubric")[0].style.display = "block";
        $("#trRubric_Yes")[0].style.display = "block";
        $("#trRubric_No")[0].style.display = 'none';
        $("#ContentEditor_Item_imgRemoveRubric")[0].style.display = "none";

        //set control values
        $("#ContentEditor_Item_hdnRubricID")[0].value = "-2";
        $("#lblRubricName")[0].innerHTML = "Default Rubric 2-point";
        $("#lblRubricType")[0].innerHTML = "Holistic";
        $("#ContentEditor_Item_divRubricPanelRubricScoring")[0].style.display = "inline";
        $("#lblRubricScoring")[0].innerHTML = "2 Point";

        AssessmentItem_Change_ItemWeight("2x");
    }
    else {
        //clear out rubric info
        keyVal = GetKeyValueFromCtrl($("#cmbRubricType")[0]);
        keyVal.val = "";
        oItemKeyValueColl.Add(keyVal);

        keyVal = GetKeyValueFromCtrl($("#cmbRubricScoring")[0]);
        keyVal.val = "0";
        oItemKeyValueColl.Add(keyVal);

        keyVal = { key: 'RubricID', val: "0" };
        oItemKeyValueColl.Add(keyVal);

        //Show and hide controls
        $("#divRubricTypeLabel")[0].style.display = "none";
        $("#divRubricScoringLabel")[0].style.display = "none";
        $("#pnlItem_Rubric")[0].style.display = "none";
        $("#trRubric_Yes")[0].style.display = "block";
        $("#trRubric_No")[0].style.display = 'none';

        AssessmentItem_Change_ItemWeight("Normal");
    }

    //Submit item changes to database.
    AssessmentItem_Change_Key_Value_Info();

}

function Assessment_ChangeField_RubricType(sender, args) {
    oItemKeyValueColl.ClearAttributes();

    //Load up changes to Item to be sent to database.
    var keyVal = { key: "RubricID", val: "0" };
    oItemKeyValueColl.Add(keyVal);

    oItemKeyValueColl.Add(GetKeyValueFromCtrl($("#cmbRubricType")[0]));

    var value = args.get_item().get_value();

    if (value == 'A') {
        //set up to clear out scoring value in database 
        keyVal = GetKeyValueFromCtrl($("#cmbRubricScoring")[0]);
        keyVal.val = "0";
        oItemKeyValueColl.Add(keyVal);

        AssessmentItem_Change_ItemWeight("Normal");

        //Show and hide controls
        $("#divRubricScoringLabel")[0].style.display = "none";
        $("#trRubric_Yes")[0].style.display = "none";
        $("#trRubric_No")[0].style.display = "block";
    }
    else {
        //set up to change scoring value to 2 points in databse.
        keyVal = GetKeyValueFromCtrl($("#cmbRubricScoring")[0]);
        keyVal.val = "2";
        oItemKeyValueColl.Add(keyVal);

        //Set controls to default 2-pt rubric
        var cmbCtrl, cmbItem;

        cmbCtrl = $find("cmbRubricScoring");
        if (cmbCtrl._selectedItem) cmbCtrl._selectedItem.set_selected(false);

        cmbItem = cmbCtrl.findItemByValue('2');
        if (cmbItem) {
            cmbItem.set_selected(true);
            cmbCtrl.set_selectedItem(cmbItem);
        }


        //Show and hide controls.
        $("#divRubricScoringLabel")[0].style.display = "inline";
        $("#trRubric_Yes")[0].style.display = "block";
        $("#trRubric_No")[0].style.display = 'none';
        $("#ContentEditor_Item_imgRemoveRubric")[0].style.display = "none";

        //Set control values.
        $("#ContentEditor_Item_hdnRubricID")[0].value = "-2";
        $("#lblRubricName")[0].innerHTML = "Default Rubric 2-point";
        $("#lblRubricType")[0].innerHTML = "Holistic";
        $("#ContentEditor_Item_divRubricPanelRubricScoring")[0].style.display = "inline";
        $("#lblRubricScoring")[0].innerHTML = "2 Point";

        AssessmentItem_Change_ItemWeight("2x");
    }

    //Submit changes to database.
    AssessmentItem_Change_Key_Value_Info();
}
//Assessment_ChangeField_RubricScoring

function Assessment_ChangeField_RubricScoring(sender, args) {

    oItemKeyValueColl.ClearAttributes();

    //Load up new rubric Points value for call to database
    var keyVal = GetKeyValueFromCtrl($("#cmbRubricScoring")[0]);
    oItemKeyValueColl.Add(keyVal);

    var value = args.get_item().get_value();

    if (value != 0) {
        //Defaut Rubric

        //Set item's weight to match rubric.
        AssessmentItem_Change_ItemWeight(value + "x");

        //Zero out RubricID in database if it was set to a custom rubric.
        if ($("#ContentEditor_Item_hdnRubricID")[0].value > 0) {

            keyVal = { key: "RubricID", val: "0" };
            oItemKeyValueColl.Add(keyVal);
        }

        $("#ContentEditor_Item_imgRemoveRubric")[0].style.display = "none";
        $("#trRubric_Yes")[0].style.display = "block";
        $("#trRubric_No")[0].style.display = 'none';

        $("#ContentEditor_Item_hdnRubricID")[0].value = -value;
        $("#lblRubricName")[0].innerHTML = "Default Rubric " + value.toString() + "-point";
        $("#lblRubricType")[0].innerHTML = "Holistic";
        $("#lblRubricScoring")[0].innerHTML = value.toString() + " Point";

    }
    else {
        //Holistic Custom Rubric

        //Set item's weight to match rubric.
        AssessmentItem_Change_ItemWeight("Normal");

        $("#trRubric_Yes")[0].style.display = "none";
        $("#trRubric_No")[0].style.display = "block";
    }

    //Submit changes to database.
    AssessmentItem_Change_Key_Value_Info();
}

function Assessment_ChangeField_copyright(sender, args) {
    var value = sender._element.attributes['cbvalue'].value;
    if (value == 'Yes') {
        //Show Source and credit
        document.getElementById('trSource').style.display = '';
        document.getElementById('trCredit').style.display = '';
        document.getElementById('trExpiryDate').style.display = '';
    } else {
        //Hide Source and credit
        document.getElementById('trSource').style.display = 'none';
        document.getElementById('trCredit').style.display = 'none';
        document.getElementById('trExpiryDate').style.display = 'none';
        

    }
    Assessment_ChangeField(sender, args);
}


function Assessment_ChangeField(sender, args) {
    var field = sender._element.attributes['field'].value;
    var value = '';
    if (sender._element.nodeName == "INPUT")
        value = args.get_newValue();
    else if (field == 'Copyright' || field == 'AnchorItem' || field == 'DocumentUpload' || field == 'AllowComments')
        value = sender._element.attributes['cbvalue'].value;
    else
        value = args.get_item().get_value();

    assessment_changeField(field, value);
}

function Assessment_ChangeField_CopyRightExpiryDate(sender, args) {
    var field = "CopyRightExpiryDate";
    var value = args._newValue;
    assessment_changeField(field, value);
}


function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow) oWindow = window.radWindow;
    else if (window.frameElement && window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
    return oWindow;
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

function AlertHostWindowOfChanges(itemID, field) {
    var parentPage = (GetRadWindow() == null ? null : GetRadWindow().BrowserWindow);
    if (parentPage && parentPage.hostWindowName) {
        switch (parentPage.hostWindowName) {
            case 'AssessmentEdit':
                parentPage.specificItem_refresh_needed = itemID;

                // Task #2481: Correction in original item in item bank or add as new item
                // Not showing Correct/ New popup for Addendum or Item Type change only
                if ((field == 'AddendumID' || field == undefined) && parentPage.specificItem_correction_needed != true)
                    parentPage.specificItem_correction_needed = false;
                else
                    parentPage.specificItem_correction_needed = true;

                break;
            case 'ItemPage':
                parentPage.refreshNeeded = true;
                break;
            default:
                break;
        }
    } else if (getURLParm('evokingWindowName') != null) {
        var windowName = getURLParm('evokingWindowName');
        switch (windowName) {
            case 'ItemSearch_Expanded':
                var DialogueToSendResultsTo;
                DialogueToSendResultsTo = parent.getCurrentCustomDialogByName(windowName);
                try {
                    DialogueToSendResultsTo.get_contentFrame().contentWindow.specificItem_refresh_needed = itemID;
                } catch (e) {

                }
                break;
            default:
                break;
        }

    }
}

function update_weight_pcts(newWeights) {
    var parentPage = (GetRadWindow() == null ? null : GetRadWindow().BrowserWindow);
    if (parentPage && parentPage.hostWindowName) {
        if (parentPage.hostWindowName == 'AssessmentEdit')
            parentPage.arryUpdateWeightPcts = newWeights;
    }
}


function assessment_changeField(field, value) {
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    var itemID = document.getElementById('divItemID').innerHTML;
    AlertHostWindowOfChanges(itemID, field);
    var responseID;
    if (AssessmentID.length > 2) {
        Service2.AssessmentItemUpdateField(AssessmentID, itemID, responseID, field, value,
            function (result_raw) {
                var result = jQuery.parseJSON(result_raw);
                if (result.StatusCode > 0) {
                    assessment_changeField_event_onFailure(null, result.Message);
                    return;
                }
                //flag_a_change_to_pdf();
                if (result.ExecOnReturn) {
                    eval(result.ExecOnReturn);
                }
            }, assessment_changeField_event_onFailure);
    } else {
        itemID = document.getElementById('divEncItemID').innerHTML;
        Service2.BankItemUpdateField(itemID, responseID, field, value,
            function (result_raw) {
                var result = jQuery.parseJSON(result_raw);
                if (result.StatusCode > 0) {
                    assessment_changeField_event_onFailure(null, result.Message);
                    return;
                }
                //flag_a_change_to_pdf();
                if (result.ExecOnReturn) {
                    eval(result.ExecOnReturn);
                }
            }, assessment_changeField_event_onFailure);
    }
}

function AssessmentItem_Change_Key_Value_Info() {

    AlertHostWindowOfChanges(oItemKeyValueColl.ItemID);

    if (oItemKeyValueColl.EncAssessmentID.length > 2) {
        Service2.AssessmentItemUpdateKeyValueFields(oItemKeyValueColl.ToJSON(),
            function (result_raw) {
                var result = jQuery.parseJSON(result_raw);
                if (result.StatusCode > 0) {
                    assessment_changeField_event_onFailure(null, result.Message);
                    return;
                }
                //flag_a_change_to_pdf();
                if (result.ExecOnReturn)
                    eval(result.ExecOnReturn);
            }, assessment_changeField_event_onFailure);
    } else {
        Service2.BankItemUpdateKeyValueFields(oItemKeyValueColl.ToJSON(),
            function (result_raw) {
                var result = jQuery.parseJSON(result_raw);
                if (result.StatusCode > 0) {
                    assessment_changeField_event_onFailure(null, result.Message);
                    return;
                }
                //flag_a_change_to_pdf();
                if (result.ExecOnReturn) {
                    eval(result.ExecOnReturn);
                }
            }, assessment_changeField_event_onFailure);
    }
}

function AssessmentItem_Change_ItemWeight(newValue) {
    var encAssessmentID = $('#divAssessmentID')[0].innerHTML.trimString();
    var itemID = $('#divItemID')[0].innerHTML.trimString();
    AlertHostWindowOfChanges(itemID);

    if (encAssessmentID.length > 2) {
        Service2.AssessmentItemUpdateField(encAssessmentID,
                                      itemID,
                                      0,
                                      'ItemWeight',
                                      newValue,
                                      function (result_raw) {
                                          var result = jQuery.parseJSON(result_raw);
                                          if (result.StatusCode > 0) {
                                              assessment_changeField_event_onFailure(null, result.Message);
                                              return;
                                          }
                                          if (result.ExecOnReturn) eval(result.ExecOnReturn);
                                      },
                                      assessment_changeField_event_onFailure);
    } else {
        itemID = $('#divEncItemID')[0].innerHTML.trimString();
        Service2.BankItemUpdateField(itemID,
                                    0,
                                    'ItemWeight',
                                    newValue,
                                    function (result_raw) {
                                        var result = jQuery.parseJSON(result_raw);
                                        if (result.StatusCode > 0) {
                                            assessment_changeField_event_onFailure(null, result.Message);
                                            return;
                                        }
                                        if (result.ExecOnReturn) eval(result.ExecOnReturn);
                                    },
                                    assessment_changeField_event_onFailure);
    }
}


function assessment_changeField_event_onFailure(result_raw, errorMessage) {

    if (result_raw) errorMessage = jQuery.parseJSON(result_raw).ErrorMessage;
    customDialog({ title: "Update Error", maximize: true, maxwidth: 500, maxheight: 10, autoSize: false, animation: "Fade", dialog_style: "alert", content: "Error during update: " + errorMessage }, [{ title: "Ok" }]);
}

function assessmentitem_changeField(targetElement, field, value, callback_function, callback_arguments, additional_value) {
    var $targetElement = $(targetElement);
    var $qi = $targetElement.closest(".questionInstance");
    var itemID = $qi.attr("id");
    AlertHostWindowOfChanges(itemID, field);
    var upd = $targetElement.attr("update");
    var responseID;
    if (upd) {
        // If this is a rich text edit, it will have an update attribute that will indicate whether it's Question, Answer or Question Directions
        responseID = upd.substring(0, 2) == "A_" ? upd.substring(2) : "";
        //if (upd.substring(0, 1) == "D") field = "Directions";
        if (upd.substring(0, 3) == "DR_") field = "DistractorRationale" + upd.substring(3);
    }
    //updateJSONcopy(itemID, field, value, responseID, additional_value);
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    if (AssessmentID.length > 2) {
        Service2.AssessmentItemUpdateField(AssessmentID, itemID, responseID, field, value,
            function (result_raw) {
                var result = jQuery.parseJSON(result_raw);
                if (result.StatusCode > 0) {
                    assessment_changeField_event_onFailure(null, result.Message);
                    return;
                }
                //flag_a_change_to_pdf();
                if (result.ExecOnReturn) eval(result.ExecOnReturn);
                if (callback_function)
                    if (typeof callback_arguments == 'undefined' || !callback_arguments)
                        callback_function.apply(this);
                    else
                        callback_function.apply(this, callback_arguments);
            }, assessment_changeField_event_onFailure);

    } else {
        itemID = document.getElementById('divEncItemID').innerHTML;
        Service2.BankItemUpdateField(itemID, responseID, field, value,
            function (result_raw) {
                var result = jQuery.parseJSON(result_raw);
                if (result.StatusCode > 0) {
                    assessment_changeField_event_onFailure(null, result.Message);
                    return;
                }
                //flag_a_change_to_pdf();
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
}


function InsertStandard(StandardList) {
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    var divIID = document.getElementById('divEncItemID').innerHTML;
    Service2.ItemInsertStandard(AssessmentID, divIID, StandardList, ItemInsertStandard_CallBack_Success(StandardList), ItemInsertStandard_CallBack_Failure);
    return false;
}

function ItemInsertStandard_CallBack_Success(StandardList) {
    AddStandardListToArray(StandardList);
    var itemID = document.getElementById('divItemID').innerHTML;
    AlertHostWindowOfChanges(itemID);
}

function ItemInsertStandard_CallBack_Failure() {

}

function AddStandardListToArray(StandardList) {
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    if (AssessmentID.length > 0) {
        ItemStandardsArray = [];
    }
    var StandardListSplit = StandardList.split("|");
    for (var x = 0; x < StandardListSplit.length; x++) {
        var StandardListSubSplit = StandardListSplit[x].split(",");
        if (!IsAddedStandardAlreadyOnItem(StandardListSubSplit[1])) {
            ItemStandardsArray.push({ EncID: StandardListSubSplit[1], StandardName: StandardListSubSplit[0] });
        }
    }
    BuildStandardHTML();
}

function IsAddedStandardAlreadyOnItem(EncID) {
    for (var j = 0; j < ItemStandardsArray.length; j++) {
        if (ItemStandardsArray[j].EncID == EncID) {
            return true;
        }
    }
    return false;
}

function BuildStandardHTML() {
    var stringStandards = '';
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    var lblStandardNamePanelLink  = document.getElementById('lblStandardNamePanelLink');
    if (lblStandardNamePanelLink != null) {
        lblStandardNamePanelLink.onclick = "";
        lblStandardNamePanelLink.style.display = "none";
    }
    var imgRemoveStandard = document.getElementById('imgRemoveStandard');
    if (imgRemoveStandard != null) {
        imgRemoveStandard.style.display = "none";
    }
    if (ItemStandardsArray.length > 1) {
        document.getElementById('lblStandardNamePanel').innerHTML = 'Standards listed below...';

        stringStandards = '<ul>';
        for (var j = 0; j < ItemStandardsArray.length; j++) {
            stringStandards += '<li><a href="javascript:" onclick="OpenStandardText(\'' + ItemStandardsArray[j].EncID + '\');">' + ItemStandardsArray[j].StandardName + '</a>&nbsp;<img src="../../../Images/cross.gif" title="Remove Standard" onclick="RemoveStandardfromItem(\'' + ItemStandardsArray[j].EncID + '\');" /></li>';
        }
        stringStandards += '</ul>';
        document.getElementById('divStandards').innerHTML = stringStandards;
    }
    if (ItemStandardsArray.length == 1) {
        stringStandards += '<a href="javascript:" onclick="OpenStandardText(\'' + ItemStandardsArray[0].EncID + '\');return false;">' + ItemStandardsArray[0].StandardName + '</a>';
        if (AssessmentID.length == 0) {
            stringStandards += '&nbsp;<img src="../../../Images/cross.gif" title="Remove Standard" onclick="RemoveStandardfromItem(\'' + ItemStandardsArray[0].EncID + '\');" />';
        }
        document.getElementById('lblStandardNamePanel').innerHTML = stringStandards;
        document.getElementById('divStandards').innerHTML = '';
    }
    if (ItemStandardsArray.length == 0) {
        document.getElementById('divStandards').innerHTML = '';
        document.getElementById('lblStandardNamePanel').innerHTML = '';
    }
}


function RemoveStandardfromItem(encStandard) {
    //alert('RemoveStandardfromItem: ' + encStandard);
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    var divIID = document.getElementById('divEncItemID').innerHTML;

    if (AssessmentID.length == 0) {
        Service2.ItemRemoveStandard(AssessmentID, divIID, encStandard, ItemRemoveStandard_CallBack_Success, ItemRemoveStandard_CallBack_Failure);

        for (var j = 0; j < ItemStandardsArray.length; j++) {
            if (ItemStandardsArray[j].EncID == encStandard) {
                ItemStandardsArray.splice(j, 1);
                break;
            }
        }

        BuildStandardHTML();
    }
    return false;
}

function ItemRemoveStandard_CallBack_Success() {

}

function ItemRemoveStandard_CallBack_Failure() {

}

function OpenRubricText() {
    var paramID;
    if (useTQ) {
        paramID = "?yID=" + $("#divEncItemID")[0].innerHTML.trimString();
    }
    else {
        paramID = "?xID=" + $("#ContentEditor_Item_hdnRubricID")[0].value;
    }
    setTimeout(function () { customDialog({ url: (rubricTextSrc + paramID), autoSize: true, name: "RubricText" }); }, 0);

    //customDialog({ url: ('../../Rubrics/RubricText.aspx?xID=' + id), autoSize: true, name: 'RubricText' });
    //setTimeout(function () { parent.customDialog({ url: ("/Controls/Rubrics/RubricText.aspx?yID=" + ItemID), autoSize: true, name: "RubricText" }); }, 0);
    //setTimeout(function () { parent.customDialog({ title: 'RubricContent', modal:true, content: $("#ContentEditor_Item_hdnRubricContent")[0].innerhtml, autoSize: true, name: "RubricContent" }); }, 0);
}

function OpenStandardText(encStandard) {
    //alert('OpenStandardText: ' + encStandard);
    stopPropagation($.event);
    customDialog({ url: ('ContentEditor_Item_StandardText.aspx?xID=' + encStandard), autoSize: true, name: 'ContentEditorItemStandardText' });
    return false;
}


function upload_completed() {
    window.location.reload();
}

$(document).ready(function () {

    oItemKeyValueColl = new ItemKeyValueCollection();
        oItemKeyValueColl.EncAssessmentID = $('#divAssessmentID')[0].innerHTML.trimString();
        oItemKeyValueColl.ItemID = $('#divItemID')[0].innerHTML.trimString();
   
    if (($("#cmbItemType_Input")[0].value == "Multiple Choice ") || ($("#cmbItemType_Input")[0].value == "Multiple Choice (4 Distractors)") || ($("#cmbItemType_Input")[0].value == "Multiple Choice (3 Distractors)") || ($("#cmbItemType_Input")[0].value == "Multiple Choice (5 Distractors)"))  {
        $DistractorHolder = $('#DistractorHolder');
        $("#sortable").sortable({
            cancel: ".SKEditableBodyText",
            start: function (event, ui) {
                top_constraint_for_drag_helper = $('#topPortion').height() + 60;
                bottom_constraint_for_drag_helper = $(window).height() - 40;
                //$("body").bind("mousedown", main_editor_lost_focusV2);
                $("body").bind('mousemove', dragHelper);
            },
            stop: function (event, ui) {
                $("body").unbind('mousemove', dragHelper);
                scrollHelper_Cancel();
                onItemMoved(event, ui);
            },
            placeholder: 'questionPlaceholder'
        });

        setTimeout(function () {
            var rtsScroll = $(".rtsScroll");
            rtsScroll.css('width', (parseInt($(".rtsScroll").css('width')) + 20) + "px");
        }, 500);
    }
    
});

var firstEdit;
var question_sort_in_progress = false;

function main_editor_gain_focus(e) {
    clearTimeout(blur_timeout);
    return true;
}

function main_editor_mousedown(e) {
    StopPropagation(e);
}

function StopPropagation(e) {
    e.cancelBubble = true;
    if (e.stopPropagation) {
        e.stopPropagation();
    }
}

var panelBar;
var panelBarProductsTab;
var multiPage;
var tel_RadEditorAddendumEdit;

function onLoad(sender) {
    panelBar = sender;
    panelBarProductsTab = panelBar.get_items().getItem(1);
    //            multiPage = panelBar.get_items().getItem(1).findControl("RadMultiPage1");
}

function onItemClicked(sender, eventArgs) {
    if (!panelBarProductsTab.get_selected()) {
        panelBarProductsTab.expand();
        panelBarProductsTab.select();
    }

    var pageView = multiPage.get_pageViews().getPageView(
        eventArgs.get_item().get_index());

    pageView.set_selected(true);
}


function stopPropagation(e) {
    e.cancelBubble = true;

    if (e.stopPropagation)
        e.stopPropagation();
}


function addNewItem(itemType) {
    var TestCategory = document.getElementById('lbl_TestCategory').value;
    //parent.customDialog({ url: ('../../../Controls/Assessment/ContentEditor/ContentEditor_Image.aspx?xID=a0pFaDN2aHh0RVNINHRCVkxpY3J2QT09&NewAndReturnID=a3o5YVh2TGtiZnJtQkVSQkhQV2VsQT09&NewAndReturnType=Item'), autoSize: true, name: 'ContentEditorITEM' });
    if (itemType == "Rubric") {
        itemType = ($("#cmbRubricType")[0].control._text == "Holistic" ? "RubricHolistic" : "RubricAnalytical");
    }
    if (itemType == "Image") {
        parent.customDialog({ url: (appClient + 'Dialogues/AddNewItem.aspx?xID=' + itemType + '&NewAndReturnID=' + document.getElementById('divItemID').innerHTML + '&NewAndReturnType=ContentEditorITEM' + '&TestCategory=' + TestCategory), maximize: true, maxwidth: 450, maxheight: 250, autoSize: true, name: 'NewItem', title: ('Add New ' + itemType), destroyOnClose: true });
    }
    else {
        customDialog({ url: (appClient + 'Dialogues/AddNewItem.aspx?xID=' + itemType + '&NewAndReturnID=' + document.getElementById('divItemID').innerHTML + '&NewAndReturnType=ContentEditorITEM' + '&TestCategory=' + TestCategory), autoSize: true, name: 'NewItem', title: ('Add New ' + itemType) });
    }
}

function SearchItem(itemType) {
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    var TestCategory = document.getElementById('lbl_TestCategory').value;
    //parent.customDialog({ url: ('../../../Controls/Assessment/ContentEditor/ContentEditor_Image.aspx?xID=a0pFaDN2aHh0RVNINHRCVkxpY3J2QT09&NewAndReturnID=a3o5YVh2TGtiZnJtQkVSQkhQV2VsQT09&NewAndReturnType=Item'), autoSize: true, name: 'ContentEditorITEM' });
    if (itemType == 'Image')
        customDialog({ url: (appClient + 'Controls/Images/ImageSearch_ExpandedV2.aspx?NewAndReturnType=ContentEditorITEM&ShowExpiredItems=No&xID=' + AssessmentID), maximize: true, maxwidth: 950, maxheight: 675, name: ('Search' + itemType), title: ('Search ' + itemType), destroyOnClose: true });
    if (itemType == 'Addendum')
        customDialog({ url: (appClient + 'Controls/Addendums/AddendumSearch_ExpandedV2.aspx?NewAndReturnType=ContentEditorITEM' + '&TestCategory=' + TestCategory + '&ShowExpiredItems=No'), maximize: true, maxwidth: 950, maxheight: 675, name: ('Search' + itemType), title: ('Search ' + itemType), destroyOnClose: true });
    if (itemType == 'Standards')
        customDialog({ url: (appClient + 'Controls/Standards/StandardsSearch_ExpandedV2.aspx?NewAndReturnType=ContentEditorITEM&MultiSelect=' + ((AssessmentID.length > 2) ? 'Yes' : 'No')), maximize: true, maxwidth: 950, maxheight: 675, name: ('Search' + itemType), title: ('Search ' + itemType), destroyOnClose: true });
    if (itemType == 'Rubric') {
        var selectedText = $("#cmbRubricType")[0].control._text;
        customDialog({ url: (appClient + 'Controls/Rubrics/RubricSearch_ExpandedV2.aspx?NewAndReturnType=ContentEditorITEM&rubrictype=' + selectedText + '&TestCategory=' + TestCategory + '&ShowExpiredItems=No'), maximize: true, maxwidth: 950, maxheight: 675, name: ('Search' + itemType), title: ('Search ' + itemType), destroyOnClose: true });
    }
}

function setCursorPos() {
    try {
        cursorPosition = tel_RadEditorAddendumEdit.getSelection().getRange();
    } catch (e) {

    }
}

function setup_assessment_directions(editor, args) {
    //cursorPosition = editor.getSelection().getRange();
    tel_RadEditorAddendumEdit = editor;
    editor.attachEventHandler("onblur", Addendum_Edit_save);
    editor.attachEventHandler("onclick", setCursorPos);
    editor.attachEventHandler("onkeyup", setCursorPos);
}

function Addendum_Edit_save(e) {
    //cursorPosition = tel_RadEditorAddendumEdit.getSelection().getRange();
    Addendum_changeFieldValue("Content", tel_RadEditorAddendumEdit.get_html());
    document.getElementById('previewAddendum').innerHTML = tel_RadEditorAddendumEdit.get_html();
}

function xInsertImage(img) {
    //cursorPosition = tel_RadEditorAddendumEdit.getSelection().getRange();
    if (cursorPosition != null) {
        tel_RadEditorAddendumEdit.getSelection().selectRange(cursorPosition); //restore cursor position
        tel_RadEditorAddendumEdit.pasteHtml(String.format("<img src='{0}' border='0' align='' alt='' /> ", img));
    }
}

function InsertAddendum(AddendumID, AddendumName, AddendumType, AddendumGenre) {
    assessment_changeField('AddendumID', AddendumID);
    document.getElementById('trAddendum_No').style.display = 'none';
    document.getElementById('lblNameAddendum').innerHTML = AddendumName;
    document.getElementById('lblTypeAddendum').innerHTML = AddendumType;
    document.getElementById('lblGenreAddendum').innerHTML = AddendumGenre;
    document.getElementById('trAddendum_Yes').style.display = '';
}

function InsertRubric(encRubricID, RubricName, RubricType, RubricScoring, CriteriaCount, RubricID) {
    oItemKeyValueColl.ClearAttributes();

    var keyVal = { key: "RubricID", val: RubricID };
    oItemKeyValueColl.Add(keyVal);

    if (RubricType == 'A')
        AssessmentItem_Change_ItemWeight("Normal");
    else
        AssessmentItem_Change_ItemWeight(RubricScoring + "x");

    oItemKeyValueColl.Add(keyVal);

    AssessmentItem_Change_Key_Value_Info();

    document.getElementById('trRubric_No').style.display = 'none';
    document.getElementById('lblRubricName').innerHTML = RubricName;
    if (RubricType == "A") {
        $('#lblRubricType')[0].innerHTML = RubricType == 'A' ? 'Analytical ' + CriteriaCount + ' x ' + RubricScoring : 'Holistic';
        $("#ContentEditor_Item_divRubricPanelRubricScoring")[0].style.display = "none";
    }
    else {
        $('#lblRubricType')[0].innerHTML = 'Holistic';
        $("#ContentEditor_Item_divRubricPanelRubricScoring")[0].style.display = "inline";
        document.getElementById('lblRubricScoring').innerHTML = RubricScoring == '0' ? 'Custom' : RubricScoring + ' Point';
    }
    $("#ContentEditor_Item_hdnRubricID")[0].value = RubricID.toString();

    document.getElementById('trRubric_Yes').style.display = 'block';
    $("#ContentEditor_Item_imgRemoveRubric")[0].style.display = "inline";

}

var modalWin = parent.$find('RadWindow1Url');

function displayFullDescription(obj) {
    var spanContainer = obj.parentNode;
    var fullTextSpan = $('.fullText', obj.parentNode);
    fullTextSpan.css('display', 'inline');
}

function requestFilter(sender, args) {
    var senderElement = sender.get_element();
    var itemText = args.get_item().get_text();
    var panelID = senderElement.getAttribute('xmlHttpPanelID');
    var panel = $find(panelID);

    switch (panelID) {
        case 'standardsFilterSubjectXmlHttpPanel':
            var courseDropdown = $find('courseDropdown');
            clearAllDropdownItems(courseDropdown);
            panel.set_value('{"Grade":"' + itemText + '"}');
            Assessment_ChangeField(sender, args);
            break;
        case 'standardsFilterCourseXmlHttpPanel':
            var gradeDropdown = $find('gradeDropdown');
            var grade = gradeDropdown.get_selectedItem().get_value();
            panel.set_value('{"Grade":"' + grade + '","Subject":"' + itemText + '"}');
            Assessment_ChangeField(sender, args);
            break;
    }

}

function loadChildFilter(sender, args) {
    //load panel
    var senderElement = sender.get_element();
    var results = args.get_content();
    var dropdownObjectID = senderElement.getAttribute('objectToLoadID');
    var dropdownObject = $find(dropdownObjectID);

    if (!dropdownObject) return false;

    //Clear all context menu items
    clearAllDropdownItems(dropdownObject);

    /*Add each new context menu item
    results[i].Key(value) = ID
    results[i].Value(text) = display text
    */
    for (var i = 0; i < results.length; i++) {
        addDropdownItem(dropdownObject, results[i].Key ? results[i].Key : results[i], results[i].Value);
    }

    if (results.length == 1) {
        dropdownObject.get_items().getItem(0).select();
    } else if (dropdownObject._emptyMessage) {
        dropdownObject.clearSelection();
    }
}

function addDropdownItem(dropdownObject, itemTextandValue, value) {
    if (!dropdownObject || !itemTextandValue) {
        return false;
    }

    /*indicates that client-side changes are going to be made and 
    these changes are supposed to be persisted after postback.*/
    dropdownObject.trackChanges();

    //Instantiate a new client item
    var item = new Telerik.Web.UI.RadComboBoxItem();

    //Set its text and add the item
    if (typeof (value) == 'undefined') {
        item.set_text(itemTextandValue);
        item.set_value(itemTextandValue);
    } else {
        item.set_text(value);
        item.set_value(itemTextandValue);
    }
    dropdownObject.get_items().add(item);

    //submit the changes to the server.
    dropdownObject.commitChanges();
}

function clearAllDropdownItems(dropdownObject) {
    if (dropdownObject == null)
        return false;
    if (dropdownObject.get_items() == null)
        return false;
    var allItems = dropdownObject.get_items().get_count();
    if (allItems < 1) {
        return false;
    }

    /*indicates that client-side changes are going to be made and 
    these changes are supposed to be persisted after postback.*/
    dropdownObject.trackChanges();

    //clear all items
    dropdownObject.get_items().clear();

    //submit the changes to the server.
    dropdownObject.commitChanges();

    return false;
}

function editFilterName(obj) {
    standardFilterName = document.getElementById('standardFilterName');
    switch (obj.innerText) {
        case 'Edit':
            standardFilterName.className = 'standardFilterNameHeader_edit';
            standardFilterName.disabled = false;
            obj.innerText = 'Done';
            break;
        default:
            standardFilterName.className = 'standardFilterNameHeader';
            standardFilterName.disabled = true;
            obj.innerText = 'Edit';
            break;
    }
}


function RationalShowHide(e) {

    for (var x = 0; x < 10; x++) {

        try {

            if (document.getElementById('DRHeader_' + x).style.display == 'inline-block')
                document.getElementById('DRHeader_' + x).style.display = 'none';
            else
                document.getElementById('DRHeader_' + x).style.display = 'inline-block';

            if (document.getElementById('DRbody_' + x).style.display == 'inline-block')
                document.getElementById('DRbody_' + x).style.display = 'none';
            else {
                document.getElementById('DRbody_' + x).style.display = 'inline-block';
                var editor = CKEDITOR.inline("DistractorRationale_Text_"+ x);
                editor.on('focus', function () {
                    editor.setReadOnly(false);
                });
            }
        } catch (e) {

        }

    }   
    $('div[id^="DistractorRationale_Text_"]').attr('contenteditable', 'true');  
}


function previewPDF() {
    //ShowSpinner();
    var $iframe_preview = $('#iframe_preview');
    //if (!$iframe_preview.attr('src'))
    //$('#iframe_preview').attr('src', 'RenderItemAsPDF.aspx?formID=' + activeForm + '&xID=' + getURLParm('xID') + '&#page=1&view=Fit');

    var a = document.getElementById('divTestQ').innerHTML;
    var divIID = document.getElementById('divEncItemID').innerHTML;
    var xt = a.length > 1 ? '&TestQuestion=' + a : '';
    $('#iframe_preview').attr('height', '600px');
    $('#iframe_preview').attr('src', appClient + 'Record/RenderItemAsPDF.aspx?xID=' + divIID + xt, 'print');
}

function mainTabStrip_OnClientTabSelecting(sender, args) {
    var selected_tab_value = args.get_tab().get_value();
    if (selected_tab_value == 'Preview') {
        previewPDF();
    }
    else {
        $('#iframe_preview').attr('src', '');
    }
}

function RubricTypeMessage() {

    customDialog({ title: "Rubric Type", maximize: true, maxwidth: 500, maxheight: 100, autoSize: false, content: "An Analytical rubric identifies and assesses components of a finished product.<br/><br/> A Holistic rubric assesses student work as a whole.", dialog_style: 'confirm' }, [{ title: "Ok" }]);

}

function OpenAddendumText() {
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    var divIID = document.getElementById('divEncItemID').innerHTML.trimString();
    Service2.GetAddedendumText(divIID, AssessmentID, function (jsonResult) {
        var addendumTextContent = $.parseJSON(jsonResult);
        setTimeout(function () {
            if (parent && parent.location.pathname.indexOf("AssessmentPage.aspx") <= -1) {
                customDialog({ title: '', maximize: true, maxwidth: 800, maxheight: 400, content: addendumTextContent });
            }
            else {
                customDialog({ url: ('ContentEditor_Item_AddendumText.aspx?xID=' + divIID + '&AssessmentID=' + AssessmentID), autoSize: true, name: 'ContentEditorItemAddendumText' });
            }
        }, 0);
    });
}

function OpenAdvancedItemInfo() {
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    var divIID = document.getElementById('divEncItemID').innerHTML.trimString();
    customDialog({ url: (appClient + 'Controls/Assessment/ContentEditor/ContentEditor_Item_AdvInfo.aspx?xID=' + divIID + '&AssessmentID=' + AssessmentID + (AssessmentID.length > 2 ? '&qType=TestQuestion' : '')), maximize: true, maxwidth: 700, maxheight: 300, name: 'ContentEditorItemAddendumText' });
}


function onFailed(obj) {
    if (obj)
        alert(obj);
}

function onItemMoved(event, ui) {
	var ItemID = $(".questionInstance").attr("id");

    var Field = "Distractor";
    var CorrectAnswer;
    AlertHostWindowOfChanges(ItemID, Field);
    RenumberDistractors();
    var newOrder = new Array();
    var count = 1;
    var answerCount = 1;
    var distidx = 0;
    var distnode = null;
	var wrkhtml = "";
    var distractorNodes = $(".tblRow .SKEditableBodyText");
    for (distidx = 0; distidx < distractorNodes.length; distidx++) {
    	distnode = distractorNodes[distidx];
    	if (distnode.id && (distnode.id.substr(0, 15) == "Distractor_Text")) {
    		var mathnodes = $('#' + distnode.id).find('.math-tex');
			if (mathnodes) {
				for (var i = 0; i < mathnodes.length; i++) {
					if (mathnodes[i].dataset && mathnodes[i].dataset.ckeWidgetData) {
						var mathdata = mathnodes[i].dataset.ckeWidgetData;
						if (mathdata) {
							mathdata = mathdata.replace(/\{\"math\"\:\"/i, '');
							mathdata = mathdata.replace(/\"}/i, '');
							mathdata = mathdata.replace(/\\\\/g, '\\');

							var parentNode = mathnodes[i].parentNode;

							$(parentNode).replaceWith(function () {
								return $('<span class="math-tex">').append(mathdata).append("</span>");
							});
						}
					}
				}
			}
			
    		wrkhtml = distnode.innerHTML;

  			wrkhtml = wrkhtml.trimStart().replace(/&nbsp;/gi, ' ');
			
			newOrder.push(wrkhtml);
			count++;
		}
	}
    if (count > 4) {
        newOrder.push('');
        count++;
    }
    for (distidx = 0; distidx < distractorNodes.length; distidx++) {
    	distnode = distractorNodes[distidx];
    	if (distnode.id && distnode.id.substr(0, 24) == ("DistractorRationale_Text")) {
    		wrkhtml = distnode.innerHTML;
    		wrkhtml = wrkhtml.trimStart().replace(/&nbsp;/gi, ' ');
    		newOrder.push(wrkhtml);
    		count++;
    	}
    }
    if (count > 9) {
        newOrder.push('');
        count++;
    }
    $(".tblRow #CorrectAnswer").each(function () {
        var response = $(this)[0].checked;
        if (response)
            CorrectAnswer = answerCount;
        answerCount++;
    });
    var AssessmentID = document.getElementById('divAssessmentID').innerHTML.trimString();
    if (AssessmentID.length > 2) {
        Service2.AssessmentItemDistractorOrderUpdateField(AssessmentID, ItemID, CorrectAnswer, newOrder.join('@@'),
            function (result_raw) {
                var result = jQuery.parseJSON(result_raw);
                if (result.StatusCode > 0) {
                    assessment_changeField_event_onFailure(null, result.Message);
                    return;
                }
                if (result.ExecOnReturn) eval(result.ExecOnReturn);
            }, assessment_changeField_event_onFailure);

    } else {
        //itemID = document.getElementById('divEncItemID').innerHTML;
        Service2.BankItemDistractorOrderUpdateField(ItemID, CorrectAnswer, newOrder.join('@@'),
            function (result_raw) {
                var result = jQuery.parseJSON(result_raw);
                if (result.StatusCode > 0) {
                    assessment_changeField_event_onFailure(null, result.Message);
                    return;
                }
                if (result.ExecOnReturn) {
                    eval(result.ExecOnReturn);
                }
            }, assessment_changeField_event_onFailure);
    }

	ReloadCkeditorForSingleItem(distractorNodes);
	
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


var $DistractorHolder;
var top_constraint_for_drag_helper, bottom_constraint_for_drag_helper;

function dragHelper(event) {
    if (event.pageY < top_constraint_for_drag_helper) scollUpHelper_Start();
    else if (event.pageY > bottom_constraint_for_drag_helper) scollDownHelper_Start();
    else scrollHelper_Cancel();
}

var scrolling_distractors = false;
var scrolling_item;

function scollDownHelper_Start(event) {
    scrolling_distractors = true;
    setTimeout("scrollHelper(10)", 20);
}

function scollUpHelper_Start(event) {
    scrolling_distractors = true;
    setTimeout("scrollHelper(-10)", 20);
}

function scrollHelper(offset) {
    if (!scrolling_distractors) return;
    $DistractorHolder.scrollTop($DistractorHolder.scrollTop() + offset);
    setTimeout("scrollHelper(" + offset + ")", 20);
}

function scrollHelper_Cancel(event) {
    scrolling_distractors = false;
}

function RenumberDistractors() {
    var count = 65;
    $(".tblRow .sort_Index").each(function () {
        $(this).html(String.fromCharCode(count) + '.')
        count++;
    });

    var distractor_Text_Count = 1;
    var distractorRationale_Text_Count = 1;
    $(".tblRow .answer_text").each(function () {
        if ($(this).attr("update").substring(0, 2) == "A_") {
            $(this).attr("update", "A_" + distractor_Text_Count);
            distractor_Text_Count++;
        }
        if ($(this).attr("update").substring(0, 3) == "DR_") {
            $(this).attr("update", "DR_" + distractorRationale_Text_Count);
            distractorRationale_Text_Count++;
        }
    });


}
