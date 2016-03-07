<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportCriteria.ascx.cs"
    Inherits="Thinkgate.Controls.Reports.ReportCriteria" %>
<style type="text/css">
    div.noWrapRadListBox .rlbText{
        white-space: nowrap;
    }
    .rlbGroupRight {
        overflow-x: scroll !important;
    }
</style>
<script type="text/javascript">
    var criterionAction = {
        none: 0,
        add: 1,
        remove: 2
    };

    var criteria = {
        actions: {
            list: {},
            containsUniqueKey: function (uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function (key) {
                for (var criterion in this.list) {
                    if (this.list[criterion].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(criterionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.actions.containsUniqueKey(uniqueKey)) {
                this.actions.list[uniqueKey].action = action;
            }
            else {
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
            }
            else if (action == 2) { //Remove key from requiredFields hidden value.
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
        remove: function (key) {
            this.addAction(criterionAction.remove, key, key, 0);
        },
        removeAction: function (uniqueKey) {
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
        ignore: function (key) {
            if (!this.actions.containsKey(key)) {
                return;
            }

            this.actions.list[key].action = criterionAction.none;
        },
        clearActions: function () {
            this.actions.list = {};
        },
        buildActionStrings: function () {
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

    function removeCriterion(div, key) {
        updateHiddenSelectedValue(key, false);

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

    function finalizeCriteriaDeletionString(sender, args) {
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
            if (document.getElementById('hiddenTextField')) {
                document.getElementById('hiddenTextField').value = actionStrings.deletionString;
            }

            if (document.getElementById('changedSelections')) {
                document.getElementById('changedSelections').value = actionStrings.updateString;
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

    function updateSubjectSelectedFeild() {
        var hiddenSubjectListSelected = document.getElementById("hiddenSubjectListSelected");

        if (hiddenSubjectListSelected != null && hiddenSubjectListSelected.value != "") {
            var subjects = hiddenSubjectListSelected.value.split(",");
            var checkBoxLists = $("div[id*='RadCombobBoxCriteriaCheckBoxList']");
            for (var i = 0; i < checkBoxLists.length; ++i) {
                var checkBoxList = $find(checkBoxLists[i].id);

                if (checkBoxList == null) {
                    continue;
                }

                var allItems = checkBoxList._element.getElementsByTagName("input");
                for (var j = 0; j < allItems.length; j++) {
                    for (var k = 0; k < subjects.length; k++) {
                        var subjectNameToSelect = subjects[k].replace("Subject_", "");
                        if (subjectNameToSelect == allItems[j].nextSibling.innerText) {
                            var item = allItems.item(j);
                            setTimeout(function () {
                                $(item).click();
                            }, 500);
                        }
                    }
                }
            }
        }
    }

    function updateHiddenSelectedValue(selectedItemValue, isToAdd) {
        var hiddenGradeListSelected = document.getElementById("hiddenGradeListSelected");
        var hiddenSubjectListSelected = document.getElementById("hiddenSubjectListSelected");
        var hiddenSchoolTypeListSelected = document.getElementById("hiddenSchoolTypeListSelected");
        var hiddenFieldsSelected = document.getElementById("hiddenFieldsSelected");
        var itemToRemove = selectedItemValue + ",";
        var regExpression = new RegExp(itemToRemove, "g");
        var headerKey = selectedItemValue.split("_");

        if (isToAdd == true) {
            if (headerKey[0] == "Grade" && hiddenGradeListSelected.value.indexOf(selectedItemValue) == -1)
                hiddenGradeListSelected.value += selectedItemValue + ",";
            if (headerKey[0] == "Subject" && hiddenSubjectListSelected.value.indexOf(selectedItemValue) == -1)
                hiddenSubjectListSelected.value += selectedItemValue + ",";
            if (headerKey[0] == "SchoolType" && hiddenSchoolTypeListSelected.value.indexOf(selectedItemValue) == -1)
                hiddenSchoolTypeListSelected.value += selectedItemValue + ",";
        } else {
            hiddenFieldsSelected.value = hiddenFieldsSelected.value.replace(regExpression, "");
            if (headerKey[0] == "Grade") {
                hiddenGradeListSelected.value = hiddenGradeListSelected.value.replace(regExpression, "");
            }

            if (headerKey[0] == "Subject") {
                hiddenSubjectListSelected.value = hiddenSubjectListSelected.value.replace(regExpression, "");
            }
            if (headerKey[0] == "SchoolType") {
                hiddenSchoolTypeListSelected.value = hiddenSchoolTypeListSelected.value.replace(regExpression, "");
            }
        }

    }

    function onItemChecked(sender, eventArgs) {
        var hiddenGradeListSelected = document.getElementById("hiddenGradeListSelected");
        var hiddenSubjectListSelected = document.getElementById("hiddenSubjectListSelected");
        var hiddenSchoolTypeListSelected = document.getElementById("hiddenSchoolTypeListSelected");
        var selectedItem = eventArgs.get_item();
        var selectedItemVal = selectedItem != null ? selectedItem.get_value() : sender.get_value();
        var splitVal = selectedItemVal.split("_");
        var headerKey = splitVal[0];
        var value = splitVal[1];
        var isChecked = selectedItem.get_checked();

        var divElement = $("div[id='" + sender.get_element().id + "']");

        if ($("div[id*='RadCombobBoxCriteriaCheckBoxListCluster']").length > 0) {
            if (sender.get_element().id == $("div[id*='RadCombobBoxCriteriaCheckBoxListCluster']")[0].id) {

                var splitValNew = selectedItemVal.split("Cluster_");
                value = splitValNew[1];
            }
        }
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


        // return without setting up removeAction if item is already selected.
        if (isChecked && selectedItemVal.indexOf("Grade_") !== -1 && hiddenGradeListSelected.value.indexOf(selectedItemVal) !== -1) {          
            criteria.add(headerKey, value);
            return;
        }
        if (isChecked && selectedItemVal.indexOf("Subject_") !== -1 && hiddenSubjectListSelected.value.indexOf(selectedItemVal) !== -1) {
            criteria.add(headerKey, value);
            return;
        }
        if (isChecked && selectedItemVal.indexOf("SchoolType_") !== -1 && hiddenSchoolTypeListSelected.value.indexOf(selectedItemVal) !== -1) {         
            criteria.add(headerKey, value);
            return;
        }

        if (!isChecked) {
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
                criteria.add(headerKey, value);
                updateHiddenSelectedValue(selectedItemVal, isChecked);


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
</script>
<div style="width: 200px; vertical-align: middle; text-align: center;">
    <telerik:RadButton ID="btnUpdateCriteria" runat="server" Text="Update Results" ToolTip="Update report criteria"
        Skin="Web20" OnClientClicked="finalizeCriteriaDeletionString" OnClick="BtnUpdateCriteriaClick" AutoPostBack="false"
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
<asp:HiddenField ID="hiddenTextField" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="changedSelections" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="requiredFields" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="requiredFieldsSelected" ClientIDMode="Static" runat="server" />
<asp:TextBox ID="hiddenGradeListSelected" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
<asp:TextBox ID="hiddenSchoolTypeListSelected" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
<asp:TextBox ID="hiddenSubjectListSelected" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
<asp:TextBox ID="hiddenFieldsSelected" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
<br />
<br />
