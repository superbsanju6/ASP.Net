<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchControl.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.SearchControl" %>
<asp:HiddenField ID="hdnSearchControlSchema" runat="server" ClientIDMode="Static" />

<style type="text/css">
    .ui-widget-content {
        background: #ccc !important;
        padding: 1px 0;
        border-radius: 4px;
    }

    .newScrollBar {
        scrollbar-3dlight-color: #000;
        scrollbar-arrow-color: #fff;
        scrollbar-darkshadow-color: #324d92;
        scrollbar-face-color: #808080;
        scrollbar-highlight-color: #fff;
        scrollbar-shadow-color: #313132;
        scrollbar-track-color: #ffffff;
    }
</style>

<div id="divSearchControl_Container" style="border: 1px solid Black; border-radius: 10px; background-color: white; height: 512px; width: 200px;">
    <div id="divCriteriaPlaceHolder" runat="server" class="newScrollBar" style="margin: 5px; height: 512px; overflow-x: hidden; overflow-y: auto;">
    </div>
</div>
<input type="hidden" id="hdnCriteriaToolTipDelayHide" runat="server" value="0" />
<input type="hidden" id="hdnIsTeacherClick"  value="NO" />

<script type="text/javascript">
    var keepOpen = false;
    var delayHide = 5000; //default
    var isOpen = false;
    var currentCriteria = '';
    var standardObjectData = null;

    $(document).ready(function () {
        delayHide = $("input[id*=hdnCriteriaToolTipDelayHide]").val();
        if (delayHide == undefined) {
            delayHide = 5000;
        }
        if (parseInt(delayHide) == 0) {
            delayHide = 5000;
        }
        initPreSelect();


        $(document).mouseup(function (e) {
            /*Close any open dialog having divToolTipTarget_, when clicked outside of the box*/
            keepOpen = false;
            $("div[id*=divToolTipTarget_]").each(function () {
                var container = $("#" + this.id);
                if (!container.is(e.target) // if the target of the click isn't the container...
                    && container.has(e.target).length === 0) // ... nor a descendant of the container
                {
                    try {
                        /*close only if it is open*/
                        if (container.dialog("isOpen") && !isDatePickersOpen())
                            container.dialog('close');
                    } catch (ex2) { }
                }
            });
        });
    });

    /* function to initialize, pre-select the values and text of composite criteria control like Standards */
    function initPreSelect() {
        if (searchControlSchema)
        {
            for (var i = 0; i < searchControlSchema.length; i++) {
                var criteria = getCriteriaByKey(searchControlSchema[i].Key);
                if (criteria.Key === "Standards")
                {
                    if (criteria.Value && criteria.Value.Value && criteria.Value.Value != "") {
                        initPreSelectStandardCriteria(criteria);
                    }
                }
            }
        }
    }

    function initPreSelectStandardCriteria(criteria) {
        var target = $("#divToolTipTarget_" + criteria.Key);
        if (target && target.length > 0) {
            var ctls = target.find("select");
            if (ctls && ctls.length > 0) {
                var sset = ctls[0];
                var sgrade = ctls[1];
                var ssubject = ctls[2];
                var scourseName = ctls[3];
                var sname = ctls[4];

                if (standardObjectData == null) {
                    var sdata = $.parseJSON(criteria.Value.Value);
                    standardObjectData = sdata;
                    criteria.DataObject = sdata;
                }
                else {
                    var sdata = criteria.DataObject;
                }

                addItem(sset, sdata.StandardSet, sdata.StandardSet);
                addItem(sgrade, sdata.Grade, sdata.Grade);
                addItem(ssubject, sdata.Subject, sdata.Subject);
                addItem(scourseName, sdata.CourseName, sdata.CourseName);
                addItem(sname, sdata.StandardName, sdata.StandardName);

                sset.selectedIndex = findItemIndexByValue(sset, sdata.StandardSet);
                sgrade.selectedIndex = findItemIndexByValue(sgrade, sdata.Grade);
                ssubject.selectedIndex = findItemIndexByValue(ssubject, sdata.Subject);
                scourseName.selectedIndex = findItemIndexByValue(scourseName, sdata.CourseName);
                sname.selectedIndex = findItemIndexByValue(sname, sdata.StandardName);

                selectStandardsCriteriaValue(sname);
            }
        }
    }

    function isDatePickersOpen() {
        var startDatePickerActive = $("#startDate").datepicker("widget").is(":visible");
        var endDatePickerActive = $("#endDate").datepicker("widget").is(":visible");
        return (startDatePickerActive || endDatePickerActive);
    }

    function preSelectDefaultValues(criteria, controlId) {
        if (!criteria) {
            return;
        }
        if (!criteria.DefaultValue) {
            return;
        }
        if ($("#" + controlId).length == 0) {
            return;
        }
        var parts = criteria.DefaultValue.Value.split(",");
        if (criteria.UIType == '1') {
            var rows = $("#" + controlId)[0].rows;
            for (var i = 0; i < rows.length; i++) {
                for (var j = 0; j < parts.length; j++) {
                    var input = $(rows[i]).find("input[type=checkbox]");
                    if ($.trim(input.val()) == $.trim(parts[j])) {
                        input[0].checked = true;
                    }
                }
            }
        }
        else if (criteria.UIType == '2') {
            var ddl = $("#" + controlId)[0];
            for (var i = 0; i < ddl.options.length; i++) {
                var option = $(ddl.options[i]);
                if ($.trim(option.text()) == $.trim(criteria.DefaultValue.Value)) {
                    ddl.selectedIndex = i;
                    break;
                }
            }
        }
    }

    function showTooltip(whichButton, dialogTarget) {

        if ($("#divSchoolGradeName").html != "" && $("#divSchoolGradeName").css('display') != 'none' && $("#divToolTipTarget_SchoolGradeName_Selected").css('display') == 'none') {
            if (dialogTarget == 'divToolTipTarget_SchoolGradeName') {
                if ($("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool option").length == 1) {

                    $("#hdnIsTeacherClick").val("YES");

                    var selectID = $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool option").val();

                    var ctl = $("select[id=" + "MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames" + "_ddlCriteriaGrades]")[0];
                    var dllName = $("select[id=" + "MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames" + "_ddlCriteriaNames]")[0];                

                    $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool option:first").attr("selected", "selected");
                    $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool option").attr("selected", "selected");

                    $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames option[value='" + selectID + "']").attr("selected", "selected");

                    $(ctl).empty();
                    $(dllName).empty();

                    var sValue = $("#" + ctl.id).val();
                    var gValue = $("#" + dllName.id).val();
                    $.ajax({
                        type: "POST",
                        url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetSchoolGrade',
                        data: "{'schoolSet':'" + $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool option").val() + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",

                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert(errorThrown);
                        },
                        success: function (result) {
                            var gradeResult = result.d;
                            if (result.d.indexOf('$') >= 0) {
                                //split the result 
                                var gradeResult = result.d.split('$')[0];
                                var schoolResult = result.d.split('$')[1];
                            }

                            if (ctl) {
                                if (gradeResult.split('</option>').length > 2) {
                                    $("#" + ctl.id).html(gradeResult);
                                    $("#" + ctl.id).val(sValue);

                                    $(dllName).append($('<option></option>').val("0").html("Select Name"));

                                    selectTeacherCriteriaValue(ctl);
                                } else if (dllName) {
                                    $("#" + ctl.id).html(gradeResult);
                                    $("#" + ctl.id).val(sValue);

                                    $("#" + dllName.id).html(schoolResult);
                                    $("#" + dllName.id).val(gValue);
                              
                                    selectTeacherCriteriaValue(dllName);

                                }
                            }
                        }
                    });
                }
            }
        }

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
            tooltipWidth = '340';
        }
        else if (criteria.UIType == '14') {
            tooltipHeight = '47';
            tooltipWidth = '340';
        }
        else if (criteria.UIType == '15') {
            var controlId = $("#" + dialogTarget).find("select")[2].id;
            tooltipHeight = '100';
            tooltipWidth = '260';
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

    function reloadCriteriaIntoPopup() {
        
        var tagFrame = top.findFrameWindowByUrl('ResourceSearchKentico.aspx');
        var tagCriteria = $(tagFrame.document).find("#Tags_SelectedTagValues").val();

        // reloadSelectedCriteriaIntoPopup defined in Tags.ascx control
        if (reloadSelectedCriteriaIntoPopup) {
            reloadSelectedCriteriaIntoPopup(tagCriteria);
        }
    }

    function getDependencyParameters(key) {
        for (var i = 0; i < searchControlSchema.length; i++) {
            if (searchControlSchema[i].Key == key) {
                var criteria = searchControlSchema[i];
                if (criteria) {
                    return criteria.Dependencies;
                }
            }
        }
    }

    function getDependentCriterion(key) {
        var dependentCriterion = [];
        for (var i = 0; i < searchControlSchema.length; i++) {
            if (searchControlSchema[i].Dependencies && searchControlSchema[i].Dependencies.length > 0) {
                for (var j = 0; j < searchControlSchema[i].Dependencies.length; j++) {
                    if (searchControlSchema[i].Dependencies[j].Key == key) {
                        var criteria = searchControlSchema[i];
                        if (criteria) {
                            dependentCriterion.push(criteria);
                        }
                    }
                }
            }
        }
        return dependentCriterion;
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
    function validateRange(obj) {
        var criteriaName = $(obj).parents("div[keyName*=Criteria]").attr("key");
        var dialogTarget = $("#divToolTipTarget_" + criteriaName);
        var text = $(dialogTarget).find("input[type=text]");
        var text1;
        var text2;
        
        if (!isNaN(text[0].value)) {
            text1 = parseFloat(text[0].value);
            text2 = parseFloat(text[1].value);
        } else {
            text1 = text[0].value;
            text2 = text[1].value;
        }
        if (text1 > text2) {
            window.radalert("Invalid range. Please re-enter and try again.", 250, 47);
            return false;
        } else closeToolTip(obj);
    }
    function closeToolTip(obj) {
        if (parseInt($(obj).val()) === 0) {
            return;
        }
        var criteriaName = $(obj).parents("div[keyName*=Criteria]").attr("key");
        dialogTarget = $("#divToolTipTarget_" + criteriaName);
        var dialogSelected = $("#divToolTipTarget_" + criteriaName + '_Selected');
        var dialogSelectedText = $("#divToolTipTarget_" + criteriaName + '_Selected_Text');
        $(dialogTarget).dialog('close');
        
        for (var i = 0; i < searchControlSchema.length; i++) {
            if (searchControlSchema[i].Key == criteriaName) {
                var criteria = searchControlSchema[i];
                if (criteria) {

                    if (criteria.UIType == '1') {
                        $(dialogSelectedText).text($(obj).find("option:selected").text());
                    criteria.Value.Key = $(obj).val();
                    criteria.Value.Value = $(obj).find('option:selected').text();
                    }
                    else if (criteria.UIType == '2') {
                        $(dialogSelectedText).text($(obj).find("option:selected").text());
                        criteria.Value.Key = $(obj).val();
                        criteria.Value.Value = $(obj).find('option:selected').text();
                    }
                    else if (criteria.UIType == '5') {
                        var tagFrame = top.findFrameWindowByUrl('ResourceSearchKentico.aspx');
                        var tagCriteria = $(tagFrame.document).find("#Tags_SelectedTagValues").val();

                        criteria.Value.Key = 'Click to View Selected Tags';
                        criteria.Value.Value = tagCriteria;

                        $(dialogSelectedText).text("Click to View Selected Tags");
                        $(dialogSelectedText).show("fast");
                    }
                    else if (criteria.UIType == '3') {
                        var option = $(obj).find("option:selected").val();
                        var optionText = $(obj).find("option:selected").text();
                        var text = $(dialogTarget).find("input[type=text]").val();

                        criteria.Value.Key = option;
                        criteria.Value.Value = text;

                        $(dialogSelectedText).text(optionText + ": " + text);
                    }
                    else if (criteria.UIType == '11') {
                        var text = $(dialogTarget).find("input[type=text]").val();
                        if (text != null && text != "" && text.indexOf("_") < 0) {
                            criteria.Value.Value = text;
                            $(dialogSelectedText).text(text);
                        } 
                    } else if (criteria.UIType == '12') {
                        var text = $(dialogTarget).find("input[type=text]");
                        
                        if (text[0].value != null && text[0].value != "" && text[0].value.indexOf("_") < 0) {
                            criteria.Value.Value = text[0].value;
                            criteria.ToValue.Value = text[1].value;
                            $(dialogSelectedText).text(text[0].value + " To " + text[1].value);
                        }
                    }

                    if (criteria.Value.Value != "") (dialogSelected).show("fast");
                    criteria.IsUpdatedByUser = true;
                    break;
                }
            }
        }
        $("#hdnSearchControlSchema").val(JSON.stringify(searchControlSchema));
        //try { window.clearTimeout(timer); } catch (ex) { }
    }
    //
    // Return Standard selection items to tag text item
    //
    function selectStandardsCriteriaValue(standards) {

        var criteriaName = $(standards).parents("div[keyName*=Criteria]").attr("key");
        var criteria;

        var option = $(standards).find("option:selected").val();
        var optionText = $(standards).find("option:selected").text();

        for (var i = 0; i < window.searchControlSchema.length; i++) {
            if (window.searchControlSchema[i].Key == criteriaName) {
                criteria = window.searchControlSchema[i];
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

        if (arr[0].toString().indexOf("Select") < 0 ) {
            displayText += "Standard Set:   " + arr[0] + "<br/>";
            criteria.StandardSelection.StandardSet = arr[0];
        }
        else criteria.StandardSelection.StandardSet = null;
        
        if (arr[1].toString().indexOf("Select") < 0 ) {
            displayText += "Grade: " + arr[1] + "<br/>";
            criteria.StandardSelection.Grade = arr[1];
        }
        else criteria.StandardSelection.Grade = null;
        
        if (arr[2].toString().indexOf("Select") < 0 ) {
            displayText += "Subject:  " + arr[2] + "<br/>";
            criteria.StandardSelection.Subject = arr[2];
        }
        else criteria.StandardSelection.Subject = null;
        
        if (arr[3].toString().indexOf("Select") < 0 ) {
            displayText += "Course:  " + arr[3] + "<br/>";
            criteria.StandardSelection.Course = arr[3];
        }
        else criteria.StandardSelection.Course = null;

        if (arr[4].toString().indexOf("Select") < 0) {
            displayText += "Standard:  " + arr[4];
            criteria.Value.Value = optionText;
            criteria.Value.Key = option;
        } else {
            criteria.Value.Value = null;
            criteria.Value.Key = null;
        }

        $(dialogSelectedText).html(displayText);
            $(dialogSelected).show("fast");

            $("#hdnSearchControlSchema").val(JSON.stringify(window.searchControlSchema));
    }
    //
    // Return Curriculum selection items to tag text item
    //
    function selectCurriculumsCriteriaValue(curriculum) {

        var criteriaName = $(curriculum).parents("div[keyName*=Criteria]").attr("key");
        var criteria;

        var option = $(curriculum).find("option:selected").val();
        var optionText = $(curriculum).find("option:selected").text();

        for (var i = 0; i < window.searchControlSchema.length; i++) {
            if (window.searchControlSchema[i].Key == criteriaName) {
                criteria = window.searchControlSchema[i];
                break;
            }
        }

        var dialogTarget = $("#" + curriculum.parentElement.parentElement.parentElement.parentElement.parentElement.id);
        var dialogSelected = $("#" + curriculum.parentElement.parentElement.parentElement.parentElement.parentElement.id + '_Selected');
        var dialogSelectedText = $("#" + curriculum.parentElement.parentElement.parentElement.parentElement.parentElement.id + '_Selected_Text');

        var arr = [];
        var displayText = "";
        dialogTarget.find("select").each(function (i, e) {
            arr.push($(e).find("option:selected").text());
        });

        if (arr[0].toString().indexOf("Select") < 0) {
            displayText += "Grade:   " + arr[0] + "<br/>";
            criteria.CurriculumSelection.Grade = arr[0];
        }
        else criteria.CurriculumSelection.Grade = null;
        
        if (arr[1].toString().indexOf("Select") < 0) {
            displayText += "Subject: " + arr[1] + "<br/>";
            criteria.CurriculumSelection.Subject = arr[1];
        }
        else criteria.CurriculumSelection.Subject = null;
        

        if (arr[2].toString().indexOf("Select") < 0) {
            displayText += "Curriculum:  " + arr[2] + "<br/>";
            criteria.CurriculumSelection.Course = arr[2];
            criteria.Value.Value = optionText;
            criteria.Value.Key = option;
        }
        else criteria.CurriculumSelection.Course = null;

        $(dialogSelectedText).html(displayText);
        $(dialogSelected).show("fast");

        $("#hdnSearchControlSchema").val(JSON.stringify(window.searchControlSchema));
    }
    //
    function selectCriteriaValue(checkboxList) {
        var selectedItems = getSelectedItems(checkboxList);

        var dialogSelected = $("#" + checkboxList.parentElement.id + '_Selected');
        var dialogSelectedText = $("#" + checkboxList.parentElement.id + '_Selected_Text');

        if (selectedItems.length == 0) {
            $(dialogSelected).hide("fast");
            return;
        }

        var selectedTexts = '';
        var selectedValues = '';
        for (var i = 0; i < selectedItems.length; i++) {
            if (i < (selectedItems.length - 1)) {
                selectedTexts += selectedItems[i].Text + ", ";
                selectedValues += selectedItems[i].Value + ", ";
            }
            else {
                selectedTexts += selectedItems[i].Text;
                selectedValues += selectedItems[i].Value;
            }
        }

        $(dialogSelected).show("fast");
        $(dialogSelectedText).text(selectedTexts);

        var criteriaName = $(checkboxList).parents("div[keyName*=Criteria]").attr("key");

        for (var i = 0; i < searchControlSchema.length; i++) {
            if (searchControlSchema[i].Key == criteriaName) {
                var criteria = searchControlSchema[i];
                if (criteria) {
                    criteria.Value.Key = criteriaName;
                    criteria.Value.Value = selectedValues;
                    break;
                }
            }
        }
        $("#hdnSearchControlSchema").val(JSON.stringify(searchControlSchema));
        //try { window.clearTimeout(timer); } catch (ex) { }
    }
    function UpdateDuration(obj) {
        var criteriaName = $(obj).parents("div[keyName*=Criteria]").attr("key");
        var criteria;

       for (var i = 0; i < window.searchControlSchema.length; i++) {
            if (window.searchControlSchema[i].Key == criteriaName) {
                criteria = window.searchControlSchema[i];
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
                criteria.Value.Value = textDays[0].value;
            } else {
                displayText += "Days: 0";
                criteria.Value.Value = "0";
            }

        dialogTarget.find("select").each(function (i, e) {
            arr.push($(e).find("option:selected").text());
        });

        if (arr[0].toString().indexOf("Select") < 0) {
            displayText += " Hours: " + arr[0];
            criteria.Value.Value += ":" + arr[0];
        }
        if (arr[1].toString().indexOf("Select") < 0) {
            displayText += " Minutes: " + arr[1];
            criteria.Value.Value += ":" + arr[1];
    }

        $(dialogSelectedText).html(displayText);
        $(dialogSelected).show("fast");

        $("#hdnSearchControlSchema").val(JSON.stringify(window.searchControlSchema));
    }
    

    function UpdateDateRange(obj) {
        var criteriaName = $(obj).parents("div[keyName*=Criteria]").attr("key");
        var criteria;

        for (var i = 0; i < window.searchControlSchema.length; i++) {
            if (window.searchControlSchema[i].Key == criteriaName) {
                criteria = window.searchControlSchema[i];
                break;
            }
        }

        var dialogTarget = $("#" + obj.parentElement.id);
        var dialogSelected = $("#" + obj.parentElement.id + '_Selected');
        var dialogSelectedText = $("#" + obj.parentElement.id + '_Selected_Text');

        var arr = [];
        var displayText = "";
        criteria.Value.Value = "";

        dialogTarget.find("input[type=text]").each(function (i, e) {
            arr.push($(e).find("input[type=text]").context.value);
        });

        if (arr[0].toString() != null && arr[0].toString() != "") {
            displayText += " Start Date: " + arr[0];
            criteria.Value.Value += arr[0];
        }
        criteria.Value.Value += ":";
        if (arr[1].toString() != null && arr[1].toString() != "") {
            displayText += "<br/> End Date: " + arr[1];
            criteria.Value.Value += arr[1];
        }
        if (displayText == "")
            $(dialogSelected).hide("fast");
        else {
            $(dialogSelectedText).html(displayText);
            $(dialogSelected).show("fast");
        }

        $("#hdnSearchControlSchema").val(JSON.stringify(window.searchControlSchema));
    }
    function getSelectedItems(checkboxList) {
        var selected = [];
        $(checkboxList.rows).each(function (i, e) {
            var checkbox = $(e.cells[0]).find("input[type=checkbox]");
            if (checkbox.is(':checked')) {
                var selValue = checkbox.val();
                var selText = $(e.cells[0]).find("label").text();
                selected.push({ 'Text': selText, 'Value': selValue });
            }
        });
        return selected;
    }

    function hideToolTips() {
        closeCurrentDialogImmediately();
    }

    function cancelPropagation() {
        if (window.event && window.event.stopPropagation) {
            window.event.stopPropagation();
        }
        if (window.event && window.event.cancelBubble) {
            window.event.cancelBubble();
        }
        if (window.event && window.event.preventDefault) {
            window.event.preventDefault();
        }
    }

    function keepDialog() {
        keepOpen = true;
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

    function closeCurrentDialogImmediately() {
        keepOpen = false;
        //try { window.clearTimeout(timer); } catch (ex) { }
        // $("div[id*=divToolTipTarget_]").dialog("close");
        $("div[id*=divToolTipTarget_]").each(function () {
            try { $("#" + this.id).dialog('close'); } catch (ex2) { }
            
        });
    }

    function removeCriteria(obj) {

        var dialogSelected = $("#" + obj.parentElement.id);
        var dialogSelectedText = $("#" + obj.parentElement.id + '_Text');

        var key = $(obj).attr("key");

        if ($("#" + obj.parentElement.id + '_Text').text() == 'Teacher (My Docs folder)') {

            if ($("#hdnIsTeacherClick").val() == "YES") {
                $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool option:first").attr("selected", "selected");
            }
            
            $("#divSchoolGradeName").hide();
            $("#divToolTipTarget_SchoolGradeName_Selected").hide();
            $("#divSchoolGradeName").css("display", "none");
            $("#divToolTipTarget_SchoolGradeName_Selected").css("display", "none");
            //code written to make teacher options selected value 0 e.g make it default on changes 
            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool option[value='0']").attr("selected", "selected");
            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades option[value='0']").attr("selected", "selected");
            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames option[value='0']").attr("selected", "selected");

            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades option").empty();
            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames option").empty();

            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades option[value='0']").append($('<option></option>').val("0").html("Select Grade"));
            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames option[value='0']").append($('<option></option>').val("0").html("Select Name"));
        }

        var dependents = getDependentCriterion(key);
        if (dependents && dependents.length > 0) {
            for (var i = 0; i < dependents.length; i++) {
                removeDependentCriteriaByKey(dependents[i].Key);
            }
        }

       // $(dialogTarget).dialog('close');
        $(dialogSelectedText).text("");
        $(dialogSelected).hide("fast");

        var criteriaName = $(obj).attr("key");
        for (var i = 0; i < searchControlSchema.length; i++) {
            if (searchControlSchema[i].Key == criteriaName) {
                var criteria = searchControlSchema[i];
                if (criteria) {
                    criteria.Value.Key = $(obj).val();
                    criteria.Value.Value = $(obj).find('option:selected').text();
                    criteria.StandardSelection.StandardSet = null;
                    criteria.StandardSelection.Grade = null;
                    criteria.StandardSelection.Subject = null;
                    criteria.StandardSelection.Course = null;
                    criteria.CurriculumSelection.Grade = null;
                    criteria.CurriculumSelection.Subject = null;
                    criteria.CurriculumSelection.Course = null;
                    criteria.SchoolGradeNameSelection.School = null;
                    criteria.SchoolGradeNameSelection.Grade = null;
                    criteria.SchoolGradeNameSelection.Name = null;
                    // Values have been reset, now clear tooltip control selections.
                    if (criteria.UIType == '1') {
                        $("div[id=divToolTipTarget_" + criteriaName + "]").find("input[type=checkbox]").each(function (i, e) {
                            e.checked = false;
                        });
                    }
                    else if (criteria.UIType == '2') {
                        $("div[id=divToolTipTarget_" + criteriaName + "]").find("select")[0].selectedIndex = 0;
                    }
                    else if (criteria.UIType == '3') {
                        $("div[id=divToolTipTarget_" + criteriaName + "]").find("input[type=text]").each(function () {
                            $(this).val('');
                        });
                        $("div[id=divToolTipTarget_" + criteriaName + "]").find("select")[0].selectedIndex = 0;
                    }
                    else if (criteria.UIType == '5') {
                        // defined in Tag control
                        clearTagSelections();
                    }
                    else if (criteria.UIType == '7' || criteria.UIType == '8' || criteria.UIType == '15') {
                        $("div[id=divToolTipTarget_" + criteriaName + "]").find("select").each(function (i, e) {
                            if (e.options.length > 0) {
                                e.selectedIndex = 0;
                            }
                            if (criteria.UIType == '8') {
                                if (i > 0) {
                                    if (i == 1) {
                                        //clearDDL(e, "Select Subject");
                                        e.selectedIndex = 0;
                                    }
                                    else if (i == 2) {
                                        //clearDDL(e, "Select Curriculum");
                                        e.selectedIndex = 0;
                                    }
                                }
                            }
                            else if (criteria.UIType == '9') {
                                if (i > 0) {
                                    if (i == 1) {
                                        clearDDL(e, "Select Grade");
                                    }
                                    else if (i == 2) {
                                        clearDDL(e, "Select Subject");
                                    }
                                    else if (i == 3) {
                                        clearDDL(e, "Select Course");
                                    }
                                    else if (i == 4) {
                                        clearDDL(e, "Select Standards");
                                    }
                                }
                            }
                            else if (criteria.UIType == '15') {
                                $("#hdnIsTeacherClick").val('NO');
                                $("#divToolTipTarget_SchoolGradeName_Selected").hide();
                                if (i > 0) {
                                    if (i == 1) {
                                        clearDDL(e, "Select Grade");
                                    }
                                    else if (i == 2) {
                                        clearDDL(e, "Select Name");                                        
                                    }                                  
                                }
                            }
                        });
                    } else if (criteria.UIType == '10' || criteria.UIType == '11' || criteria.UIType == '14') {
                        $("div[id=divToolTipTarget_" + criteriaName + "]").find("input[type=text]").each(function () {
                            $(this).val('');
                        });
                    }
                    try {
                        $("div[id=divToolTipTarget_" + criteriaName + "]").dialog("close");
                    } catch (ex) { }
                    break;
                }
            }
        }
        $("#hdnSearchControlSchema").val(JSON.stringify(searchControlSchema));
        //try { window.clearTimeout(timer);} catch(ex) {}
        return false;
    }

    function removeDependentCriteriaByKey(key) {
        var criteriaContainer = $("div[type=CriteriaControl][key=" + key + "]");
        var obj = criteriaContainer.find("img[key=" + key + "]");
        if (obj && obj.length > 0) {
            removeCriteria(obj[0]);
        }
        return false;
    }

    function getCriteriaByKey(key) {
        for (var i = 0; i < searchControlSchema.length; i++) {
            if (searchControlSchema[i].Key == key) {
                var criteria = searchControlSchema[i];
                return criteria;
            }
        }
    }

    function resetSearch() {
        for (var i = 0; i < searchControlSchema.length; i++) {
            var criteria = searchControlSchema[i];
            if (criteria) {
                if (!criteria.Locked) {

                    criteria.Value.Key = "";
                    criteria.Value.Value = "";
                    criteria.StandardSelection.StandardSet = null;
                    criteria.StandardSelection.Grade = null;
                    criteria.StandardSelection.Subject = null;
                    criteria.StandardSelection.Course = null;
                    criteria.CurriculumSelection.Grade = null;
                    criteria.CurriculumSelection.Subject = null;
                    criteria.CurriculumSelection.Course = null;


                    var selected = $("div[id*=divToolTipTarget_" + criteria.Key + "]");
                    selected.hide("fast");

                    if (criteria.UIType == '1') {
                        selected.find("input[type=checkbox]").each(function (i, e) {
                            e.checked = false;
                        });
                    }
                    else if (criteria.UIType == '2') {
                        selected.find("select").each(function (i, e) {
                            if (e.options.length > 0) {
                                e.selectedIndex = 0;
                            }
                        });
                    }
                    else if (criteria.UIType == '3') {
                        selected.find("input[type=text]").each(function (i, e) {
                            e.value = '';
                        });
                    }
                    else if (criteria.UIType == '8' || criteria.UIType == '9' || criteria.UIType == '15') {
                        var criteriaName = criteria.Key;
                        $("div[id=divToolTipTarget_" + criteriaName + "]").find("select").each(function (i, e) {
                            if (e.options.length > 0) {
                                e.selectedIndex = 0;
                            }
                            if (criteria.UIType == '8') {
                                if (i > 0) {
                                    if (i == 1) {
                                        //clearDDL(e, "Select Subject");
                                        e.selectedIndex = 0;
                                    }
                                    else if (i == 2) {
                                        //clearDDL(e, "Select Curriculum");
                                        e.selectedIndex = 0;
                                    }
                                }
                            }
                            else if (criteria.UIType == '9') {
                                if (i > 0) {
                                    if (i == 1) {
                                        clearDDL(e, "Select Grade");
                                    }
                                    else if (i == 2) {
                                        clearDDL(e, "Select Subject");
                                    }
                                    else if (i == 3) {
                                        clearDDL(e, "Select Course");
                                    }
                                    else if (i == 4) {
                                        clearDDL(e, "Select Standards");
                                    }
                                }
                            }
                        });
                    }
                    else if (criteria.UIType == '11' || criteria.UIType == '12' || criteria.UIType == '14') {
                        selected.find("input[type=text]").each(function (i, e) {
                            e.value = '';
                        });
                    }

                    criteria.IsUpdatedByUser = true;
                    
                }
            }
        }

                    //$("#divToolTipTarget_" + criteria.Key).dialog("close");
                    $("div[id*=divToolTipTarget_]").each(function () {
                        try { $("#" + this.id).dialog('close'); } catch (ex) { }

                    });
        $("#hdnSearchControlSchema").val(JSON.stringify(searchControlSchema));
    }

    function clearDDL(ddl, text) {
        if (!ddl) {
            return;
        }
        $(ddl).empty();
        $(ddl).append($('<option></option>').val("0").html(typeof text == "undefined" ? "-- Select --" : text));
    }

    function addToCheckBoxListControl(controlId, jsonObject) {

        for (var toolTip in jsonObject) {
            var tableRow = controlId.insertRow();
            var tableCell = tableRow.insertCell();

            var checkBoxRef = document.createElement('input');
            var labelRef = document.createElement('label');

            checkBoxRef.type = 'checkbox';
            labelRef.innerHTML = jsonObject[toolTip].Value;
            checkBoxRef.value = jsonObject[toolTip].Key;

            tableCell.appendChild(checkBoxRef);
            tableCell.appendChild(labelRef);
        };
    }


    function addItem(ddl, val, text) {
        if (!ddl) {
            return;
        }
        var exists = false;
        $(ddl).find("option").each(function () {
            if (this.value == val) {
                exists = true;
            }
        });
        if (!exists) {
            $(ddl).append($('<option></option>').val(val).html(text));
        }
    }

    function findItemIndexByValue(ddl, val) {
        var index = -1;
        if (!ddl) {
            return index;
        }
        $(ddl).find("option").each(function (i, e) {
            if (e.value == val) {
                index = i;
                return;
            }
        });
        return index;
    }
    //
    // Return Teacher selection items to tag text item
     function selectTeacherCriteriaValue(schoolgradename) {

        var criteriaName = $(schoolgradename).parents("div[keyName*=Criteria]").attr("key");
        var criteria;

        var option = $(schoolgradename).find("option:selected").val();
        var optionText = $(schoolgradename).find("option:selected").text();

        for (var i = 0; i < window.searchControlSchema.length; i++) {
            if (window.searchControlSchema[i].Key == criteriaName) {
                criteria = window.searchControlSchema[i];
                break;
            }
        }

        var dialogTarget = $("#" + schoolgradename.parentElement.parentElement.parentElement.parentElement.parentElement.id);
        var dialogSelected = $("#" + schoolgradename.parentElement.parentElement.parentElement.parentElement.parentElement.id + '_Selected');
        var dialogSelectedText = $("#" + schoolgradename.parentElement.parentElement.parentElement.parentElement.parentElement.id + '_Selected_Text');

        var arr = [];
        var displayText = "";
        var NameValue = "";
        dialogTarget.find("select").each(function (i, e) {

            arr.push($(e).find("option:selected").text());

            if(schoolgradename.id== "MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames")
            {
                NameValue = $(schoolgradename).find("option:selected").val();
            }
        });

        if (arr[0].toString().indexOf("Select") < 0) {
            displayText += "School:   " + arr[0] + "<br/>";
            criteria.SchoolGradeNameSelection.School = arr[0];
        }
        if (arr[1].toString().indexOf("Select") < 0) {
            displayText += "Grade: " + arr[1] + "<br/>";
            criteria.SchoolGradeNameSelection.Grade = arr[1];
        }
        if (arr[2].toString().indexOf("Select") < 0) {
            displayText += "Name:  " + arr[2] + "<br/>";
            criteria.SchoolGradeNameSelection.Name = NameValue;//arr[2];
        }

        $(dialogSelectedText).html(displayText);
        $(dialogSelected).show("fast");

        $("#hdnSearchControlSchema").val(JSON.stringify(window.searchControlSchema));
    }

</script>

