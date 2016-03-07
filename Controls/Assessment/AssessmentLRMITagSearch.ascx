<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentLRMITagSearch.ascx.cs" Inherits="Thinkgate.Controls.Assessment.AssessmentLRMITagSearch" %>
<asp:HiddenField ID="hdnSearchControlSchema" runat="server" ClientIDMode="Static" />
<style type="text/css">
    .ui-widget-content {
        background: #ccc!important;
        padding: 1px 0;
        border-radius: 4px;
    }

</style>
<script type="text/javascript">
    var keepOpen = false;
    var delayHide = 5000; //default
    var isOpen = false;
    var currentCriteria = '';

    var criterionAction = {
        none: 0,
        add: 1,
        remove: 2
    };

    var criteria = {
        actions: {
            list: {},
            containsUniqueKey: function(uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function(key) {
                for (var criterion in this.list) {
                    if (this.list[criterion].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function(key, value) {
            this.addAction(criterionAction.add, key + "_" + value, key, value);
        },
        addAction: function(action, uniqueKey, key, value) {
            if (this.actions.containsUniqueKey(uniqueKey)) {
                this.actions.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.actions.list[uniqueKey] = criterion;
            }

            //BJC - 6/11/2012: If this is a required selection
            if (action == 1) { //Add key to requiredFields hidden value.
                if ($('#requiredFields').attr('value').indexOf(key) > -1 && $('#requiredFieldsSelected').attr('value').indexOf(key) == -1) {
                    var requiredFieldsSelectedValue = $('#requiredFieldsSelected').attr('value') + ',' + key;
                    $('#requiredFieldsSelected').attr('value', requiredFieldsSelectedValue);
                }
            } else if (action == 2) { //Remove key from requiredFields hidden value.
                if ($('#requiredFields').attr('value').indexOf(key) > -1 && $('#requiredFieldsSelected').attr('value').indexOf(key) > -1) {
                    var requiredFieldsSelectedArr = $('#requiredFieldsSelected').attr('value').split(',');
                    for (var i = 0; i < requiredFieldsSelectedArr.length; i++) {
                        if (requiredFieldsSelectedArr[i] == key) {
                            requiredFieldsSelectedArr.splice(i, 1);
                            $('#requiredFieldsSelected').attr('value', requiredFieldsSelectedArr.join());
                            break;
                        }
                    }

                    $('#requiredFieldsSelected').attr('value', requiredFieldsSelectedValue);
                }
            }
        },
        remove: function(key) {
            this.addAction(criterionAction.remove, key, key, 0);
        },
        removeAction: function(uniqueKey) {
            if (this.actions.containsUniqueKey(uniqueKey)) {
                delete this.actions.list[uniqueKey];

                //BJC - 6/11/2012: If this is a selection required, remove key from requiredFields hidden value.
                var requiredKey = uniqueKey.substring(0, uniqueKey.indexOf('_'));
                if ($('#requiredFields').attr('value').indexOf(requiredKey) > -1 && $('#requiredFieldsSelected').attr('value').indexOf(requiredKey) > -1) {
                    var requiredFieldsSelectedArr = $('#requiredFieldsSelected').attr('value').split(',');
                    for (var i = 0; i < requiredFieldsSelectedArr.length; i++) {
                        if (requiredFieldsSelectedArr[i] == requiredKey) {
                            requiredFieldsSelectedArr.splice(i, 1);
                            $('#requiredFieldsSelected').attr('value', requiredFieldsSelectedArr.join());
                            break;
                        }
                    }

                    $('#requiredFieldsSelected').attr('value', requiredFieldsSelectedValue);
                }
            }
        },
        ignore: function(key) {
            if (!this.actions.containsKey(key)) {
                return;
            }

            this.actions.list[key].action = criterionAction.none;
        },
        clearActions: function() {
            this.actions.list = {};
        },
        buildActionStrings: function() {
            var deletionString = "";
            var updateString = "";

            for (var key in this.actions.list) {
                switch (this.actions.list[key].action) {
                case criterionAction.add:
                    updateString += this.actions.list[key].key + "~" + this.actions.list[key].value + ";";
                    break;

                case criterionAction.remove:
                    deletionString += this.actions.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    function showTooltip(whichButton, dialogTarget) {

        var key = $("#" + dialogTarget).attr("key");
        var criteria = getCriteriaByKey(key);
        currentCriteria = criteria.Key;

        closeCurrentDialogImmediately();

        //retrieve dependencies
        var dependencies = getDependencyParameters(key);
        var values = getDependencyParameterValues(key, dependencies);
        var tooltipHeight = '';
        var tooltipWidth = '';
        //get control id to pass as argument to callback.
        var controlId = '';
        if (criteria.UIType == '1') {
            var controlId = $("#" + dialogTarget).find("table")[0].id;
            tooltipHeight = '150';
            tooltipWidth = '220';
        }
        else if (criteria.UIType == '2') {
            var controlId = $("#" + dialogTarget).find("select")[0].id;
            tooltipHeight = '62';
            tooltipWidth = '250';
        }
        else if (criteria.UIType == '3') {
            tooltipHeight = '47';
            tooltipWidth = '290';
        }
        else if (criteria.UIType == '5') {
            var controlId = $("#" + dialogTarget).find("select")[0].id;
            tooltipHeight = '490';
            tooltipWidth = '700';
        }
        else if (criteria.UIType == '8') {
            var controlId = $("#" + dialogTarget).find("select")[2].id;
            tooltipHeight = '118';
            tooltipWidth = '260';
        }

        else if (criteria.UIType == '9') {
            var controlId = $("#" + dialogTarget).find("select")[2].id;
            tooltipHeight = '165';
            tooltipWidth = '260';
        }
        else if (criteria.UIType == '11') {
            tooltipHeight = '47';
            tooltipWidth = '180';
        }
        else if (criteria.UIType == '12') {
            tooltipHeight = '47';
            tooltipWidth = '340';
        }
        else if (criteria.UIType == '13') {
            tooltipHeight = '47';
            tooltipWidth = '540';
        }

        if (criteria.IsUpdatedByUser == false) {
            preSelectDefaultValues(criteria, controlId);
        }

        // and invoke dynamic callback to let caller handle cascading
        if (values && values.length > 0) {
            if (criteria && criteria.HandlerName) {
                var handler = window[criteria.HandlerName];
                if (typeof handler != "undefined") {
                    values.push({ "ControlID": controlId });
                    values.push({ "Criteria": criteria });
                    var arguments = values;
                    handler.apply(this, arguments);
                }
            }
        }

        dialogTarget = $("#" + dialogTarget);
        if (criteria.UIType == '5') {
            $(dialogTarget).dialog({
                open: function () {
                    isOpen = true;
                    reloadCriteriaIntoPopup();
                },
                close: function () {
                    isOpen = false;
                },
                resizable: false, height: tooltipHeight, width: tooltipWidth, position: { my: "left top", at: "right top", of: $(whichButton) }
            });
        }
        else {
            $(dialogTarget).dialog({ open: function () { isOpen = true; }, close: function () { isOpen = false; }, closeOnEscape: true, resizable: false, height: tooltipHeight, width: tooltipWidth, position: { my: "left top", at: "right top", of: $(whichButton) } });
        }
        $(dialogTarget).dialog("open");

    }
    function removeCriterion(div, key) {
        //criteriaDeletionString += key + ";";
        criteria.remove(key);

        criteria.actions.list[key].onclick = $(div).attr('onclick');

        var textDiv = "#" + key + "_Text";
        var imageDiv = "#" + key + "_Image";
        var updateDiv = "#" + key + "_updateMessage";

        $(textDiv).css('text-decoration', 'line-through');
        $(imageDiv).attr('src', '../../Images/commands/exclamation_red.png');
        $(div).attr('onclick', 'javascript:void(0);');

        animateItem(updateDiv);
    }

    function animateItem(divID) {
        $(divID).animate({
            left: '-50%'
        }, 300, function () {
            $(this).css('left', '150%');
            $(this).html("<b>Update required...</b>");
            $(this).animate({ left: '0%' }, 300);
        });
    }
    function closeDialog(dialogTarget) {
        keepOpen = false;
        //try { window.clearTimeout(timer); } catch (ex) { }

        var key = $("#" + dialogTarget).attr("key");
        var criteria = getCriteriaByKey(key);
        // if (criteria && criteria.AutoHide == true) {
        //     enableCloseTimer(dialogTarget);
        //}
    }
    function finalizeCriteriaDeletionStringLrmi(sender, args) {
        var actionStrings = criteria.buildActionStrings();
        criteria.clearActions();

        var jsonDemographics = buildDemographicsUpdateData();
        if (jsonDemographics != null) {
            actionStrings.updateString += "Demographics~" + jsonDemographics + ";";
        }

        var jsonRTI = buildRTIUpdateData();
        if (jsonRTI != null) {
            actionStrings.updateString += "RTI~" + jsonRTI + ";";
        }

        //BJC - 6/11/2012: Loops through an array of required fields and checks it against the updateString and deletionString. 
        //If a required field was not selected, the update action is canceled and the user is alerted.
        //Else the update action is executed.
        var requiredFieldsArr = $('#requiredFields').attr('value').split(',');
        var requiredFieldsSelected = $('#requiredFieldsSelected').attr('value');
        var missedRequired = false;
        for (var i = 0; i < requiredFieldsArr.length; i++) {
            if (requiredFieldsSelected.indexOf(requiredFieldsArr[i]) == -1 &&
            (actionStrings.updateString.indexOf(requiredFieldsArr[i]) == -1 ||
            (actionStrings.deletionString.indexOf(requiredFieldsArr[i]) > -1 && actionStrings.updateString.indexOf(requiredFieldsArr[i]) == -1))) {
                missedRequired = true;
                break;
            }
        }

        if (missedRequired) {
            alert('Please make sure all required criteria is selected before updating search.');
        }
        else {
            if (document.getElementById('hiddenLrmiTextField')) {
                document.getElementById('hiddenLrmiTextField').value = actionStrings.deletionString;
            }

            if (document.getElementById('changedLrmiSelections')) {
                document.getElementById('changedLrmiSelections').value = actionStrings.updateString;
            }

            if (document.getElementById('lblSelectedCount')) {
                $('#lblSelectedCount').text('');
            }
            if (document.getElementById('hiddenChkAll')) {
                $('#hiddenChkAll').val('');
            }
            if (document.getElementById('hiddenSelected')) {
                $('#hiddenSelected').val('');
            }
            if (document.getElementById('hiddenDeSelected')) {
                $('#hiddenDeSelected').val('');
            }
            if (document.getElementById('hiddenSelectedCount')) {
                $('#hiddenSelectedCount').val('');
            }
            if (document.getElementById('hiddenTotalCount')) {
                $('#hiddenTotalCount').val('');
            }

            __doPostBack(sender._uniqueID, '');
        }
    }

    function OnClientToggleStateChanged(sender, eventArgs) {
        logEvent("ToggleStateChanged event: <strong>" + sender.get_text() + "</strong> SelectedToggleStateIndex: " + sender.get_selectedToggleStateIndex());
    }

    function OnClientSelectedIndexChanging(sender, eventArgs) {
        sender.get_attributes().setAttribute("prevValue", eventArgs.get_item().get_value());
    }

    function onSelectedIndexChanged(sender, eventArgs) {
        //**DETERMINE IF SPECIAL CONTROL**//


        var textboxDivID = sender.get_attributes().getAttribute("textBoxDivID");
        var appendVal = "";
        if (textboxDivID != null && textboxDivID != "") {
            var textBoxLists = $("input[id*='RadTextBoxAssessmentTextSearch']");
            var textBoxList = null;
            for (var i = 0; i < textBoxLists.length; ++i) {

                textBoxList = $find(textBoxLists[i].id);

                if (textBoxList != null)
                    break;
            }

            if (textBoxList.get_value() == "") {
                return;
            }

            if (textBoxList != null) {
                appendVal = textBoxList.get_value();
                appendVal = "||" + appendVal;
            }
        }

        //**GET ARGS FROM SENDER**//
        var selectedItem = eventArgs.get_item();
        var selectedItemText = selectedItem != null ? selectedItem.get_text() : sender.get_text();
        var selectedItemVal = selectedItem != null ? selectedItem.get_value() : sender.get_value();
        if (sender.get_attributes().getAttribute("prevValue") == selectedItemVal) {
            return;
        }


        var divElement = $("div[id='" + sender.get_element().id + "']");
        // Call the service if one is attached to the check box list
        CallAttachedService(divElement);

        if (selectedItemVal == "0") {
            return;
        }
        //**-------------------**//

        //**REBUILD KEY AND VAL**//
        var splitVal = selectedItemVal.split("_");
        var key = splitVal[0];
        var value = splitVal[1];
        //**-------------------**//

        criteria.add(key, value + appendVal);

        //var adjustedID = stripString(key);
        //var id = "key_" + adjustedID;

        var id = key + "_updateMessage";
        var divID = "div[id='" + id + "']";

        if ($(divID)) {
            animateItem(divID);
        }
    }

    function onItemChecked(sender, eventArgs) {
        var selectedItem = eventArgs.get_item();
        var selectedItemVal = selectedItem != null ? selectedItem.get_value() : sender.get_value();

        var divElement = $("div[id='" + sender.get_element().id + "']");
        var childListID = divElement.attr("ChildCheckBoxList");
        if (childListID && childListID.length > 0) {
            var childDivElements = $("div[id*='" + childListID + "']");
            var childDivElement;
            for (var i = 0; i < childDivElements.length; ++i) {
                childDivElement = $find(childDivElements[i].id);
                if (childDivElement == null) continue;
            }

            if (childDivElement != null) {
                DisplayChildCheckboxItems(divElement, childDivElement);
            }
        }

        // Call the service if one is attached to the check box list
        CallAttachedService(divElement);

        //var checkBoxList = $find(sender.get_element().id);
        //checkBoxList.get_items().clear();

        if (!selectedItem.get_checked()) {
            if (criteria.actions.containsUniqueKey(selectedItemVal)) {
                criteria.removeAction(selectedItemVal);
            }
            removeCriterion("#" + selectedItemVal + "_RemoveCriterionButton", selectedItemVal);
        }
        else {
            if (criteria.actions.containsUniqueKey(selectedItemVal)) {
                var textDiv = "#" + selectedItemVal + "_Text";
                var imageDiv = "#" + selectedItemVal + "_Image";

                setTimeout(function () {
                    $(textDiv).css('text-decoration', 'none');
                    $(imageDiv).attr('src', '../../Images/close_x.gif');
                }, 0);


                var onClickJavascript = criteria.actions.list[selectedItemVal].onclick;
                if (onClickJavascript != null && onClickJavascript != "") {
                    $("#" + selectedItemVal + "_RemoveCriterionButton").attr('onclick', onClickJavascript);
                }

                criteria.removeAction(selectedItemVal);
            }
            else {
                var splitVal = selectedItemVal.split("_");
                var headerKey = splitVal[0];
                var value = splitVal[1];

                criteria.add(headerKey, value);

                //var id = "key_" + stripString(key);
                var id = headerKey + "_updateMessage";
                var divID = "div[id='" + id + "']";

                if ($(divID)) {
                    animateItem(divID);
                }
            }
        }
    }

    function CallAttachedService(divElement) {
        var serviceUrl = divElement.attr('serviceurl');
        var serviceSuccessfulCallback = divElement.attr('successcallback');
        if (serviceUrl != null && serviceSuccessfulCallback != null) {
            var data = {
                "container": {}
            };

            if (divElement.attr('dependencies') != null) {
                data = buildDependencyObject(divElement.attr('dependencies'));
            }

            ajaxWCFService({ url: serviceUrl, data: JSON.stringify(data), success: serviceSuccessfulCallback });
        }

    }

    function TooltipOnClientShow_DisplayChildCheckboxItems(sender, args) {
        var parentDivElements = $("div[id*='RadCombobBoxCriteriaCheckBoxList']");
        var childElements = $("div[id*='RadCombobBoxCriteriaCheckBoxList2']");
        DisplayChildCheckboxItems(parentDivElements[0], $find(childElements[0].id));
    }

    function DisplayChildCheckboxItems(divElement, list) {
        var parentList = $find(typeof (divElement[0]) == 'undefined' ? divElement.id : divElement[0].id);
        var showingAtLeastOneItem = false;
        var items = list.get_items();
        var checkedParentItems = parentList.get_checkedItems();

        //Start by hiding everything
        for (var i = 0; i < items.get_count() ; i++) {
            var item = items.getItem(i);
            item.get_element().style.display = "none";
        }

        //Loop through items and hide or display
        for (var k = 0; k < checkedParentItems.length; k++) {
            var selectedItem = checkedParentItems[k].get_value();
            for (var i = 0; i < items.get_count() ; i++) {
                var item = items.getItem(i);
                var attributes = item.get_attributes();
                var parentVal = attributes.getAttribute("parentValue");
                if (parentVal == selectedItem) {
                    showingAtLeastOneItem = true;
                    item.get_element().style.display = "";
                }
            }
        }

        (typeof (divElement[0]) == 'undefined' ? divElement : divElement[0]).style.display = (showingAtLeastOneItem) ? "" : "none";
    }

    function onInputBlur(sender) {
        var inputText = sender.get_value();

        var inputElement = $("input[id='" + sender.get_element().id + "']");

        var cmbBoxDivID = inputElement.attr("comboBoxDivID");

        var inputControl = $find(sender._clientID);

        var appendVal = "";
        if (cmbBoxDivID != null && cmbBoxDivID != "") {
            var cmbBoxLists = $("div[id*='RadComboBoxAssessmentTextSearch']");
            var cmbBoxList = null;
            for (var i = 0; i < cmbBoxLists.length; ++i) {

                cmbBoxList = $find(cmbBoxLists[i].id);

                if (cmbBoxList != null)
                    break;
            }

            if (cmbBoxList != null) {
                appendVal = cmbBoxList.get_text(); //adds TextSearch_ onto it, opposed to get_text()
                appendVal = appendVal + "||";
            }
        }

        if (inputText == null || inputText == "") {
            return;
        }

        var key = $(sender.get_element()).attr('updateMessageHeader');
        criteria.add(key, appendVal + inputText);

        var id = key + "_updateMessage";
        var divID = "div[id='" + id + "']";

        if ($(divID)) {
            animateItem(divID);
        }
    }

    function uncheckTooltipCheckBox(radListBoxItemID) {
        var checkBoxLists = $("div[id*='RadCombobBoxCriteriaCheckBoxList']");
        for (var i = 0; i < checkBoxLists.length; ++i) {
            var checkBoxList = $find(checkBoxLists[i].id);

            if (checkBoxList == null) {
                continue;
            }

            var foundItem = checkBoxList.findItemByAttribute("checkBoxID", radListBoxItemID);
            if (foundItem == null) {
                continue;
            }

            foundItem.uncheck();

            return;
        }
    }

    function stripString(str) {
        var result = str.replace("(", "");
        result = result.replace(")", "");
        result = result.replace(" ", "");
        result = result.replace("-", "");

        return result;
    }

    function DateSelected(sender, eventArgs) {

        var controlID = "#" + sender.get_element().id + "_wrapper";
        var dateRangePosition = $(controlID).attr('DateRangePosition');

        var val = sender.get_dateInput().get_value();

        criteria.add(dateRangePosition, val);

        var key2 = $(controlID).attr('updateMessageHeader');

        var divID = "div[id='" + key2 + "_updateMessage" + "']";
        if ($(divID)) {
            animateItem(divID);
        }
    }

    // When the tooltip is shown, automatically open the drop down control
    function onClientShowToolTipDropDownList(sender, eventArgs) {
        var partialID = $('#' + sender.get_id()).attr('dropDownListID');
        var genericDiv = $("div[id$='" + partialID + "']");
        var genericControl = $find(genericDiv.attr('id'));
        if (genericControl == null) {
            alert("onClientShowToolTipDropDownList: " + genericDiv.attr('id'));
            return;
        }

        genericControl.showDropDown();
    }

    // When the tooltip is show, automatically focus the textbox control
    function onClientShowToolTipTextBox(sender, eventArgs) {
        var partialID = $('#' + sender.get_id()).attr('textBoxID');
        var genericDiv = $("input[id$='" + partialID + "']");
        var genericControl = $find(genericDiv.attr('id'));
        if (genericControl == null) {
            alert("onClientShowToolTipTextBox: " + genericDiv.attr('id'));
            return;
        }

        genericControl.focus();
    }

    var demographicData = {
        items: null,
        add: function (item) {
            this.items.push(item);
        },
        contains: function (groupName) {
            for (var i = 0; i < this.items.length; ++i) {
                var item = this.items[i];

                if (item.groupName == groupName) {
                    return true;
                }
            }

            return false;
        }
    };

    function onDemographicCheckedChanged(sender, eventArgs) {
        if (!eventArgs.get_checked()) {
            return;
        }

        if (demographicData.items == null) {
            demographicData.items = [];
        }

        var groupName = sender.get_groupName();
        var text = sender.get_text();
        var value = sender.get_value();

        var splitVal = value.split("_");
        var demoField = splitVal[0];
        var abbreviation = splitVal[1];

        if (!demographicData.contains(groupName)) {
            if (text == "All") {
                return;
            }

            var demographicItem = {
                demoField: demoField,
                groupName: groupName,
                text: text,
                value: text,
                abbreviation: abbreviation
            };

            demographicData.add(demographicItem);
        }
        else {
            if (text == "All") {
                for (var i = 0; i < demographicData.items.length; ++i) {
                    if (demographicData.items[i].groupName == groupName) {
                        demographicData.items.splice(i, 1);
                        return;
                    }
                }
            }

            for (var j = 0; j < demographicData.items.length; ++j) {
                if (demographicData.items[j].groupName == groupName) {
                    demographicData.items[j].demoField = demoField;
                    demographicData.items[j].text = text;
                    demographicData.items[j].value = text;
                    demographicData.items[j].abbreviation = abbreviation;
                    break;
                }
            }
        }
    }

    function onDemographicSelectedIndexChanged(sender, eventArgs) {
        var text = sender.get_text();

        if (demographicData.items == null) {
            demographicData.items = [];
        }

        var splitVal = sender.get_value().split("_");
        var groupName = splitVal[0];
        var value = splitVal[1];
        var demoField = splitVal[2];

        if (!demographicData.contains(groupName)) {
            if (value == "-1") {
                return;
            }

            var demographicItem = {
                demoField: demoField,
                groupName: groupName,
                text: text,
                value: value,
                abbreviation: groupName
            };

            demographicData.add(demographicItem);
        }
        else {
            if (value == "-1") {
                for (var i = 0; i < demographicData.items.length; ++i) {
                    if (demographicData.items[i].groupName == groupName) {
                        demographicData.items.splice(i, 1);
                        return;
                    }
                }
            }

            for (var j = 0; j < demographicData.items.length; ++j) {
                if (demographicData.items[j].groupName == groupName) {
                    demographicData.items[j].demoField = demoField;
                    demographicData.items[j].text = text;
                    demographicData.items[j].value = value;
                    demographicData.items[j].abbreviation = groupName;
                    break;
                }
            }
        }
    }

    function buildDemographicsUpdateData() {
        if (demographicData.items == null) {
            return null;
        }

        if (demographicData.items.length < 1) {
            return "";
        }

        return JSON.stringify(demographicData);
    }

    function clearDemographicsData() {
        demographicData.items = null;
    }

    var rtiData = {
        items: null,
        add: function (item) {
            this.items.push(item);
        },
        contains: function (text) {
            for (var i = 0; i < this.items.length; ++i) {
                var item = this.items[i];

                if (item.text == text) {
                    return true;
                }
            }

            return false;
        }
    };

    function onRTICheckedChanged(sender, eventArgs) {
        var text = sender.get_text();
        var value = sender.get_value();

        if (rtiData.items == null) {
            rtiData.items = [];
        }

        if (eventArgs.get_checked()) {
            if (!rtiData.contains(text)) {
                var rtiItem = {
                    text: text,
                    value: value
                };

                rtiData.add(rtiItem);
            }
        } else {
            if (rtiData.contains(text)) {
                for (var i = 0; i < rtiData.items.length; ++i) {
                    if (rtiData.items[i].text == text) {
                        rtiData.items.splice(i, 1);
                        return;
                    }
                }
            }
        }
    }

    function buildRTIUpdateData() {
        if (rtiData.items == null) {
            return null;
        }

        if (rtiData.items.length < 1) {
            return "";
        }

        return JSON.stringify(rtiData);
    }
    function keepDialog() {
        keepOpen = true;
    }

    function closeCurrentDialogImmediately() {
        keepOpen = false;
        //try { window.clearTimeout(timer); } catch (ex) { }
        // $("div[id*=divToolTipTarget_]").dialog("close");
        $("div[id*=divToolTipTarget_]").each(function() {
            try {
                $("#" + this.id).dialog('close');
            } catch (ex2) {
            }

        });
    }

    function reloadCriteriaIntoPopup() {

        var tagFrame = top.findFrameWindowByUrl('AssessmentSearch_Expanded.aspx');
        var tagCriteria = $(tagFrame.document).find("#Tags_SelectedTagValues").val();

        // reloadSelectedCriteriaIntoPopup defined in Tags.ascx control
        if (reloadSelectedCriteriaIntoPopup) {
            reloadSelectedCriteriaIntoPopup(tagCriteria);
        }
    }

    function UpdateDuration(obj) {
        var criteriaName = $(obj).parents("div[keyName*=Criteria]").attr("key");
        var criteriaObj;

        for (var i = 0; i < window.searchControlSchemaLMRI.length; i++) {
            if (window.searchControlSchemaLMRI[i].Key == criteriaName) {
                criteriaObj = window.searchControlSchemaLMRI[i];
                break;
            }
        }

        var dialogTarget = $("#" + obj.parentElement.id);
        var dialogSelected = $("#" + obj.parentElement.id + '_Selected');
        var dialogSelectedText = $("#" + obj.parentElement.id + '_Selected_Text');

        var arr = [];
        var displayText = "";

        var textDays = dialogTarget.find("input[type=text]");

        if (textDays[0].value != null && textDays[0].value != "") {
            displayText += "Days: " + textDays[0].value;
            criteriaObj.Object = textDays[0].value;
        } else {
            displayText += "Days: 0";
            criteriaObj.Object = "0";
        }

        dialogTarget.find("select").each(function(i, e) {
            arr.push($(e).find("option:selected").text());
        });

        if (arr[0].toString().indexOf("Select") < 0) {
            displayText += " Hours: " + arr[0];
            criteriaObj.Object += ":" + arr[0];
        }
        if (arr[1].toString().indexOf("Select") < 0) {
            displayText += " Minutes: " + arr[1];
            criteriaObj.Object += ":" + arr[1];
        }
        criteria.add("Duration", criteriaObj.Object);
        $(dialogSelectedText).html(displayText);
        $(dialogSelected).show("fast");

        $("#searchControlSchemaLMRI").val(JSON.stringify(window.searchControlSchemaLMRI));

        var id = "Duration_updateMessage";
        var divID = "div[id='" + id + "']";

        if ($(divID)) {
            animateItem(divID);
        }
    }

    function getCriteriaByKey(key) {
        for (var i = 0; i < searchControlSchemaLMRI.length; i++) {
            if (searchControlSchemaLMRI[i].Key == key) {
                var criteria = searchControlSchemaLMRI[i];
                return criteria;
            }
        }
    }

    function getDependencyParameters(key) {
        for (var i = 0; i < searchControlSchemaLMRI.length; i++) {
            if (searchControlSchemaLMRI[i].Key == key) {
                var criteria = searchControlSchemaLMRI[i];
                if (criteria) {
                    return criteria.Dependencies;
                }
            }
        }
    }

    function getDependencyParameterValues(key, dependencies) {
        var values = [];
        if (dependencies && dependencies.length > 0) {
            for (var i = 0; i < dependencies.length; i++) {
                var criteria = getCriteriaByKey(dependencies[i].Key);
                if (criteria) {
                    var dep = { "Key": key, "DependencyKey": dependencies[i].Key, "Value": criteria.Value };
                    values.push(dep);
                }
            }
        }
        return values;
    }
    function PopulateGrade(standardElement) {
        var standardSet = $(standardElement).val();
        var temp = standardElement.id.substr(0, standardElement.id.lastIndexOf("_"));
        var temp2 = temp.substr(0, temp.lastIndexOf("_"));
        var ctl = $("select[id^=" + temp2 + "_ddlCriteriaGrades]")[0];
        var dllSubject = $("select[id^=" + temp2 + "_ddlCriteriaSubjects]")[0];
        var dllCourse = $("select[id^=" + temp2 + "_ddlCriteriaCourse]")[0];
        var dllStandards = $("select[id^=" + temp2 + "_ddlCriteriaStandards]")[0];
        $(dllSubject).empty();
        $(dllCourse).empty();
        $(dllStandards).empty();
        $(dllSubject).append($('<option></option>').val("0").html("Select Subject"));
        $(dllCourse).append($('<option></option>').val("0").html("Select Course"));
        $(dllStandards).append($('<option></option>').val("0").html("Select Standards"));

        $.ajax({
            type: "POST",
            url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardGrade',
                    data: "{'standardSet':'" + standardSet + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (result) {
                        if (ctl) {
                            var sValue = $("#" + ctl.id).val();
                            $("#" + ctl.id).html(result.d);
                            $("#" + ctl.id).val(sValue);
                        }
                    }
                });
            }


    function PopulateSubject(standardGrade) {
        var temp = standardGrade.id.substr(0, standardGrade.id.lastIndexOf("_"));
                var temp2 = temp.substr(0, temp.lastIndexOf("_"));
                var standardSetctrl = $("select[id^=" + temp2 + "_ddlCriteriaStandardSet]")[0];
                var standardSet = $(standardSetctrl).val();
                var grade = $(standardGrade).val();
                var temp = standardGrade.id.substr(0, standardGrade.id.lastIndexOf("_"));
                var temp2 = temp.substr(0, temp.lastIndexOf("_"));
                var ctl = $("select[id^=" + temp2 + "_ddlCriteriaSubjects]")[0];
                var dllCourse = $("select[id^=" + temp2 + "_ddlCriteriaCourse]")[0];
                var dllStandards = $("select[id^=" + temp2 + "_ddlCriteriaStandards]")[0];
                $(ctl).empty();
                $(dllCourse).empty();
                $(dllStandards).empty();
                $(ctl).append($('<option></option>').val("0").html("Select Subject"));
                $(dllCourse).append($('<option></option>').val("0").html("Select Course"));
                $(dllStandards).append($('<option></option>').val("0").html("Select Standards"));

                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardSubject',
                    data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (result) {
                        if (ctl) {
                            var sValue = $("#" + ctl.id).val();
                            $("#" + ctl.id).html(result.d);
                            $("#" + ctl.id).val(sValue);
                        }
                    }
                });
            }

            function PopulateCourse(standardSubject) {
                //var standardSet = $("#CriteriaStandardSet").val();
                var temp = standardSubject.id.substr(0, standardSubject.id.lastIndexOf("_"));
                var temp2 = temp.substr(0, temp.lastIndexOf("_"));
                var standardSet = $($("select[id^=" + temp2 + "_ddlCriteriaStandardSet]")[0]).val();
                var grade = $($("select[id^=" + temp2 + "_ddlCriteriaGrades]")[0]).val();
                var subject = $(standardSubject).val();
                var ctl = $("select[id^=" + temp2 + "_ddlCriteriaCourse]")[0];
                var dllStandards = $("select[id^=" + temp2 + "_ddlCriteriaStandards]")[0];

                $(ctl).empty();
                $(dllStandards).empty();
                $(ctl).append($('<option></option>').val("0").html("Select Course"));
                $(dllStandards).append($('<option></option>').val("0").html("Select Standards"));

                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardCourse',
                    data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "','subject':'" + subject + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (result) {
                        if (ctl) {
                            var sValue = $("#" + ctl.id).val();
                            $("#" + ctl.id).html(result.d);
                            $("#" + ctl.id).val(sValue);
                        }
                    }
                });
            }

            function PopulateStandards(standardCourse) {
                var temp = standardCourse.id.substr(0, standardCourse.id.lastIndexOf("_"));
                var temp2 = temp.substr(0, temp.lastIndexOf("_"));
                var standardSet = $($("select[id^=" + temp2 + "_ddlCriteriaStandardSet]")[0]).val();
                var grade = $($("select[id^=" + temp2 + "_ddlCriteriaGrades]")[0]).val();
                var subject = $($("select[id^=" + temp2 + "_ddlCriteriaSubjects]")[0]).val();
                var course = $(standardCourse).val();

                var ctl = $("select[id^=" + temp2 + "_ddlCriteriaStandards]")[0];
                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardsByStandardSetGradeSubjectCourse',
                    data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "','subject':'" + subject + "','course':'" + course + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (result) {
                        if (ctl) {
                            var sValue = $("#" + ctl.id).val();
                            $("#" + ctl.id).html(result.d);
                            $("#" + ctl.id).val(sValue);
                        }
                    }
                });
            }
            //
            // Return Standard selection items to tag text item
            //
            function selectStandardsCriteriaValue(standards) {

                var criteriaName = $(standards).parents("div[keyName*=Criteria]").attr("key");
                var criteriaObj;

                var option = $(standards).find("option:selected").val();
                var optionText = $(standards).find("option:selected").text();

                for (var i = 0; i < window.searchControlSchemaLMRI.length; i++) {
                    if (window.searchControlSchemaLMRI[i].Key == criteriaName) {
                        criteriaObj = window.searchControlSchemaLMRI[i];
                        break;
                    }
                }

                var dialogTarget = $("#" + standards.parentElement.parentElement.parentElement.parentElement.parentElement.id);
                var dialogSelected = $("#" + standards.parentElement.parentElement.parentElement.parentElement.parentElement.id + '_Selected');
                var dialogSelectedText = $("#" + standards.parentElement.parentElement.parentElement.parentElement.parentElement.id + '_Selected_Text');

                var arr = [];
                var displayText = "";
                dialogTarget.find("select").each(function (i, e) {
                    arr.push($(e).find("option:selected").text());
                });

                if (arr[0].toString().indexOf("Select") < 0) {
                    displayText += "Standard Set:   " + arr[0] + "<br/>";
                    criteriaObj.StandardSelection.StandardSet = arr[0];
                    $("#selStandardSet").val(arr[0]);
                }
                if (arr[1].toString().indexOf("Select") < 0) {
                    displayText += "Grade: " + arr[1] + "<br/>";
                    criteriaObj.StandardSelection.Grade = arr[1];
                }
                if (arr[2].toString().indexOf("Select") < 0) {
                    displayText += "Subject:  " + arr[2] + "<br/>";
                    criteriaObj.StandardSelection.Subject = arr[2];
                }
                if (arr[3].toString().indexOf("Select") < 0) {
                    displayText += "Course:  " + arr[3] + "<br/>";
                    criteriaObj.StandardSelection.Course = arr[3];
                }
                if (arr[4].toString().indexOf("Select") < 0) {
                    displayText += "Standard:  " + arr[4];
                    criteriaObj.StandardSelection.Key = arr[4];
                    criteriaObj.Object = option;
                }
                if (criteriaObj.Object != null) {
                    criteria.add("Assessed", criteriaObj.Object);
                }
                $(dialogSelectedText).html(displayText);
                $(dialogSelected).show("fast");

                $("#searchControlSchemaLMRI").val(JSON.stringify(window.searchControlSchemaLMRI));

                var id = "Assessed_updateMessage";
                var divID = "div[id='" + id + "']";

                if ($(divID)) {
                    animateItem(divID);
                }
            }
</script>


<div style="width: 200px; vertical-align: middle; text-align: center;">
    <telerik:RadButton ID="btnUpdateCriteria" runat="server" Text="Update Results" ToolTip="Update report criteria"
        Skin="Web20" OnClientClicked="finalizeCriteriaDeletionStringLrmi" OnClick="BtnUpdateLrmiCriteriaClick" AutoPostBack="false"
        Style="font-weight: bold; margin-top: 5px; margin-bottom: 5px;">
    </telerik:RadButton>
    <telerik:RadButton runat="server" ID="RadButtonClear" Text="Clear" ToolTip="Clear report criteria"
        Skin="Web20" 
        Style="font-weight: bold; margin-top: 5px; margin-bottom: 5px;" OnClientClicked="clearDemographicsData" 
        onclick="RadButtonClear_Click">
    </telerik:RadButton>
</div>
<asp:Repeater ID="criteriaRepeater" runat="server" OnItemDataBound="LoadCriteriaItems">
    <ItemTemplate>
        <placeholder id="phCriterionHeader" runat="server"></placeholder>
        <placeholder id="phCriterionItems" runat="server"></placeholder>
    </ItemTemplate>
</asp:Repeater>
<!-- This id is referenced in js -->
<asp:HiddenField ID="hiddenLrmiTextField" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="changedLrmiSelections" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="requiredFields" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="requiredFieldsSelected" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="selStandardSet" ClientIDMode="Static" runat="server" />
<br />
<br />