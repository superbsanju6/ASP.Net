var CriteriaController = {
    CurrentUniqueKey: 1,

    CriteriaNodes: [],      // will hold the nodes. Each node represents one Criteria Item or field

    CriteriaNode: function (criteriaName, value) {
        var obj = {
            CriteriaName: criteriaName,
            Values: [],
            FindValuePosition: function (valueObject) {
                for (var j = 0; j < this.Values.length; j++) {
                    if (this.isValueEqual(this.Values[j].Value, valueObject)) return j;
                }
                return -1;
            },
            FindValue: function (valueObject) {
                var pos = this.FindValuePosition(valueObject);
                if (pos == -1) return null;
                return this.Values[pos];
            },
            FindValuePositionByKey: function (key) {
                for (var j = 0; j < this.Values.length; j++) {
                    if (this.Values[j].Key == key) return j;
                }
            },
            FindValueByKey: function (key) {
                var pos = this.FindValuePositionByKey(key);
                if (pos == -1) return null;
                return this.Values[pos];
            },
            AddValue: function (value) {
                obj.Values.push(CriteriaController.SelectedValue(value));
            },
            RemoveValueByPos: function (pos) {
                if (pos < 0) return;
                this.Values.splice(pos, 1);
            },
            RemoveValue: function (valueObject) {
                var pos = this.FindValuePosition(valueObject);
                if (pos == -1) return;
                this.RemoveValueByPos(pos);
            },
            isValueEqual: function (value1, value2) {
                var p;
                for (p in value1) {
                    if (typeof (value2[p]) == 'undefined') { return false; }
                }

                for (p in value1) {
                    if (value1[p]) {
                        switch (typeof (value1[p])) {
                            case 'object':
                                if (value1[p].length && value2[p].length) {
                                    if (value1[p].length != value2[p].length) { return false; }
                                    for (var j = 0; j < value1[p].length; j++) {
                                        if (value1[p][j] != value2[p][j]) { return false; }
                                    }
                                    break;
                                }
                                if (!value1[p].equals(value2[p])) { return false; } break;
                            case 'function':
                                if (typeof (value2[p]) == 'undefined' ||
                              (p != 'equals' && value1[p].toString() != value2[p].toString()))
                                    return false;
                                break;
                            default:
                                if (value1[p] != value2[p]) { return false; }
                        }
                    } else {
                        if (value2[p])
                            return false;
                    }
                }

                for (p in value2) {
                    if (typeof (value1[p]) == 'undefined') { return false; }
                }

                return true;
            }
        };
        obj.AddValue(value);
        return obj;
    },

    SelectedValue: function (value) {
        return {
            Key: this.CurrentUniqueKey++,
            Value: value,
            Applied: false,
            CurrentlySelected: true
        };
    },

    // find the criteria node for a selected criteria item
    FindNode: function (criteriaName) {
        for (var j = 0; j < this.CriteriaNodes.length; j++) {
            if (this.CriteriaNodes[j].CriteriaName == criteriaName) return this.CriteriaNodes[j];
        }
        return null;
    },

    GetValuesAsSinglePropertyArray: function (criteriaName, propertyName, restrictToCurrentlySelected) {
        var criteriaNode = this.FindNode(criteriaName);
        if (!criteriaNode) return null;
        var retArray = [];
        for (var j = 0; j < criteriaNode.Values.length; j++) {
            if (!restrictToCurrentlySelected || criteriaNode.Values[j].CurrentlySelected) {
                if (propertyName)
                    retArray.push(criteriaNode.Values[j].Value[propertyName]);
                else
                    retArray.push(criteriaNode.Values[j].Value);
            }
        }
        return retArray;
    },

    GetValues: function (criteriaName) {
        var criteriaNode = this.FindNode(criteriaName);
        if (!criteriaNode) return null;
        return criteriaNode.Values;
    },

    Add: function (criteriaName, value) {
        var criteriaNode = this.FindNode(criteriaName);
        if (!criteriaNode) {
            this.CriteriaNodes.push(criteriaNode = this.CriteriaNode(criteriaName, value));    // criteria not previous set to anything, add new item
        } else {
            if (criteriaNode.Values.length > 0 && this.GetRestrictValueCount(criteriaName) != this.RestrictValueOptions.Unlimited) {
                this.RemoveByKey(criteriaName, criteriaNode.Values[criteriaNode.Values.length - 1].Key, true);
            }
            var valueItem = criteriaNode.FindValue(value);
            if (!valueItem) {
                criteriaNode.AddValue(value); // criteria node is there, but this is a new value. add it to value array
            } else {
                valueItem.CurrentlySelected = true;   // value is already there, ensure is marked as selected
            }
        }
        this.UpdateValueDisplayArea(criteriaNode);
    },

    CountValues: function (criteriaName) {
        var criteriaNode = this.FindNode(criteriaName);
        if (!criteriaNode) return 0;
        return criteriaNode.Values.length;
    },

    RemoveAllDependency: function (criteriaName) {
        this.RemoveAllInNodeDependency(this.FindNode(criteriaName));
    },
    RemoveAllInNodeDependency: function (criteriaNode) {
        if (!criteriaNode) return;      // found none of this criteria
        for (var i = criteriaNode.Values.length - 1; i >= 0; i--) {
            this.RemoveByKeyDependency(criteriaNode.CriteriaName, criteriaNode.Values[i].Key);
        }
    },
    RemoveByKeyDependency: function (criteriaName, key, calledFromAdd) {
        var criteriaNode = this.FindNode(criteriaName);
        if (!criteriaNode) return;      // found none of this criteria
        /*var pos = criteriaNode.FindValueByKey(key);
        criteriaNode.RemoveValueByPos(pos);*/
        var valueObject = criteriaNode.FindValueByKey(key);
        this.RemoveCommonLogic(criteriaNode, valueObject);
        // begin custom remove handler: if there's an optional remove by key handler defined for the control, call it. This could uncheck a value or clear a text box       
        /* end custom remove handler */
        this.UpdateValueDisplayArea(criteriaNode);
    },

    RemoveCommonLogic: function (criteriaNode, valueObject) {
        var removeAppliedAsWell = this.GetRestrictValueCount(criteriaNode.CriteriaName) == this.RestrictValueOptions.OnlyOne_Period;
        if (!valueObject) return;
        if (!valueObject.Applied || removeAppliedAsWell) {
            criteriaNode.RemoveValue(valueObject.Value);      // value is not marked as applied, just remove it
        } else {
            valueObject.CurrentlySelected = false; // value is already there and marked as applied, ensure is marked as selected
        }
    },

    RemoveAllInNode: function (criteriaNode) {
        if (!criteriaNode) return;      // found none of this criteria
        for (var i = criteriaNode.Values.length - 1; i >= 0; i--) {
            this.RemoveByKey(criteriaNode.CriteriaName, criteriaNode.Values[i].Key);
        }
    },

    RemoveAll: function (criteriaName) {
        this.RemoveAllInNode(this.FindNode(criteriaName));
    },

    Remove: function (criteriaName, value) {
        var criteriaNode = this.FindNode(criteriaName);
        if (!criteriaNode) return;      // found none of this criteria
        var valueObject = criteriaNode.FindValue(value);
        if (!valueObject) return;         // value is not present
        this.RemoveCommonLogic(criteriaNode, valueObject);
        this.UpdateValueDisplayArea(criteriaNode);
    },

    RemoveByKey: function (criteriaName, key, calledFromAdd) {
        
        var criteriaNode = this.FindNode(criteriaName);
        if (!criteriaNode) return;      // found none of this criteria
        /*var pos = criteriaNode.FindValueByKey(key);
        criteriaNode.RemoveValueByPos(pos);*/
            
        var valueObject = criteriaNode.FindValueByKey(key);
        this.RemoveCommonLogic(criteriaNode, valueObject);
        // begin custom remove handler: if there's an optional remove by key handler defined for the control, call it. This could uncheck a value or clear a text box
        var specialHandlerFunction;
        try {
            specialHandlerFunction = eval(criteriaName + "Controller.RemoveByKeyHandler");
        } catch (e) {
        }
        if (specialHandlerFunction && typeof specialHandlerFunction != "undefined") specialHandlerFunction.apply(eval(criteriaName + "Controller"), [criteriaName, valueObject.Value, calledFromAdd, key]);
        /* end custom remove handler */       
        if ($("#hdnSchoolClickedFromAssReprot").val() == "YES") {
            PopulateDefaultAssesementGraded();
            $("#hdnSchoolClickedFromAssReprot").val("NO");
        }
        this.UpdateValueDisplayArea(criteriaNode);

      
    },

    Clear: function () {
        for (var j = 0; j < this.CriteriaNodes.length; j++) {
            var removeValuesInNode = true;
            // begin custom remove handler: if there's an optional remove by key handler defined for the control, call it. This could uncheck a value or clear a text box
            var specialHandlerFunction;
            try {
                specialHandlerFunction = eval(this.CriteriaNodes[j].CriteriaName + "Controller.ClearHandler");
            } catch (e) {
            }
            if (specialHandlerFunction && typeof specialHandlerFunction != "undefined") removeValuesInNode = specialHandlerFunction.apply(eval(this.CriteriaNodes[j].CriteriaName + "Controller"));
            /* end custom remove handler */
            if (removeValuesInNode)
                this.RemoveAllInNode(this.CriteriaNodes[j]);
        }
    },

    UpdateCriteriaForSearch: function () {
        // right now I'm calling this before the search even though it may make more sense to send the criteria as it exists, have the server do the updates, then return
        //  the updated object. Thus if the search fails, it won't update the criteria. However this is easier and it would be easy to change later.
        for (var j = 0; j < this.CriteriaNodes.length; j++) {
            for (var i = this.CriteriaNodes[j].Values.length - 1; i >= 0; i--) {
                // if the item is not selected, remove it at this point
                if (!this.CriteriaNodes[j].Values[i].CurrentlySelected) {
                    this.CriteriaNodes[j].Values[i].Applied = false;
                    this.RemoveByKey(this.CriteriaNodes[j].CriteriaName, this.CriteriaNodes[j].Values[i].Key);
                } else {
                    // otherwise, flag the item as applied and update the HTML
                    this.CriteriaNodes[j].Values[i].Applied = true;
                    this.UpdateValueDisplayArea(this.CriteriaNodes[j]);
                }
            }
        }
    },

    // this updates the display area under a criteriaNode with the current values based on the defined jsRender template for the specific control
    UpdateValueDisplayArea: function (criteriaNode) {
        var criteriaName = criteriaNode.CriteriaName;        
        var $area = $("#selectedCritieriaDisplayArea_" + criteriaName);
        if (!$area || $area.count == 0) {
            alert("error finding #selectedCritieriaDisplayArea_" + criteriaName);
            return;
        }
        var readOnly = this.GetReadOnly(criteriaName);
        $.views.helpers({
            getCSS: function (applied, currentlySelected) {
                if (!applied && currentlySelected)
                    return "criteria_SelectedButUnapplied" + (readOnly ? " CritReadOnly" : "");
                else if (applied && currentlySelected)
                    return "criteria_Applied" + (readOnly ? " CritReadOnly" : "");
                else if (!applied && !currentlySelected)
                    return "criteria_Removed" + (readOnly ? " CritReadOnly" : "");
                else if (applied && !currentlySelected)
                    return "criteria_AppliedButPendingRemove" + (readOnly ? " CritReadOnly" : "");
                return (readOnly ? " CritReadOnly" : "");
            }
        });
        var templateName = this.GetTemplateName(criteriaName);
        $.templates("", {
            markup: "#" + templateName,
            allowCode: true
        });
        $area.html($("#" + templateName).render(criteriaNode));
    },

    ToJSON: function () {
        return JSON.stringify(this);
    },

    /*GetNewValueObject: function (criteriaName) {
    return jQuery.extend(true, {}, eval(criteriaName + "_Config").ValueDisplayTemplateName);
    },*/

    GetTemplateName: function (criteriaName) {
        return eval(criteriaName + "_Config").ValueDisplayTemplateName;
    },

    GetRestrictValueCount: function (criteriaName) {
        return eval(criteriaName + "_Config").RestrictValueCount;
    },
    
    GetReadOnly: function (criteriaName) {
        return eval(criteriaName + "_Config").ReadOnly;
    }
};

var CriteriaDataHelpers = {
    /*
    This library can be done to do a multi-field filter: Example:
    var filteredPositions = CriteriaDataHelpers.GetFilteredDataPositions(data, [selected.Grade, selected.Subject, selected.Curriculum]);
    var filteredGrades = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredPositions, 0);
    var filteredSubjects = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredPositions, 1);
    var filteredCurriculums = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredPositions, 2);
    */
    GetFilteredDataPositions: function (dataArray, filterArray) {
        var filteredPositions = [];
        for (var j = 0; j < dataArray.length; j++) {
            var exclude = false;
            var currNode = dataArray[j];
            for (var f = 0; f < filterArray.length; f++) {
                switch (typeof (currNode[f])) {
                    case 'object':
                        if (currNode[f].length || filterArray[f] != "") {
                            if (filterArray[f] != null && filterArray[f] != "") {
                                var foundValueInSelected = false;
                                for (var x = 0; x < currNode[f].length; x++) {
                                    for (var y = 0; y < filterArray[f].length; y++) {
                                        if (filterArray[f][y] == currNode[f][x]) {
                                            foundValueInSelected = true;
                                            break;
                                        }
                                    }
                                }
                                if (!foundValueInSelected) exclude = true;
                            }
                        }
                        else
                        {
                            exclude = true;
                        }
                        break;
                    default:
                        if (filterArray[f] != null && filterArray[f] != "" && typeof (filterArray[f]) == 'object' && filterArray[f].length && filterArray[f].length > 0) {
                            var foundValueInSelected = false;
                            for (var x = 0; x < filterArray[f].length; x++) {
                                if (filterArray[f][x] == currNode[f]) {
                                    foundValueInSelected = true;
                                    break;
                                }
                            }
                            if (!foundValueInSelected) exclude = true;
                        } else if (filterArray[f] != null && filterArray[f] != "" && filterArray[f] != currNode[f])
                            exclude = true;
                }

                /*if (filterArray[f] != null && filterArray[f] != "" && filterArray[f] != currNode[f])
                exclude = true;*/
            }
            if (!exclude) filteredPositions.push(j);
        }
        return filteredPositions;
    },

    GetFieldDistinct: function (dataArray, fieldPos) {
        var distinctValues = {};
        for (var j = 0; j < dataArray.length; j++) {
            if (typeof (dataArray[j][fieldPos]) == "object" && dataArray[j][fieldPos].length) {
                for (var x = 0; x < dataArray[j][fieldPos].length; x++) {
                    distinctValues[dataArray[j][fieldPos][x]] = true;
                }
            } else {
                distinctValues[dataArray[j][fieldPos]] = true;
            }
        }
        var retArray = [];
        for (var value in distinctValues) {
            if (value != "")
                retArray.push(value);
        }
        return retArray;
    },

    GetFilteredFieldDistinct: function (dataArray, filteredPositions, fieldPos) {
        var distinctValues = {};
        for (var pos = 0; pos < filteredPositions.length; pos++) {
            if (typeof (dataArray[filteredPositions[pos]][fieldPos]) == "object" && dataArray[filteredPositions[pos]][fieldPos].length) {
                for (var j = 0; j < dataArray[filteredPositions[pos]][fieldPos].length; j++) {
                    distinctValues[dataArray[filteredPositions[pos]][fieldPos][j]] = true;
                }
            } else {
                distinctValues[dataArray[filteredPositions[pos]][fieldPos]] = true;
            }
        }
        var retArray = [];
        for (var value in distinctValues) {
            if (value != "")
                retArray.push(value);
        }
        return retArray;
    },

    GetFilteredFieldDistinctTextValue: function (dataArray, filteredPositions, fieldPos, valuePol) {
        var distinctValues = [];
        for (var pos = 0; pos < filteredPositions.length; pos++) {
            if (typeof (dataArray[filteredPositions[pos]][fieldPos]) == "object" && dataArray[filteredPositions[pos]][fieldPos]!=null) {
                for (var j = 0; j < dataArray[filteredPositions[pos]][fieldPos].length; j++) {
                    distinctValues[dataArray[filteredPositions[pos]][fieldPos][j]] = true;
                    distinctValues[dataArray[filteredPositions[pos]][valuePol][j]] = true;
                }
            } else {

                if (dataArray[filteredPositions[pos]][fieldPos] != "") {
                    if (dataArray[filteredPositions[pos]][fieldPos] != null) {
                        var arr = new Array();
                        arr.push(dataArray[filteredPositions[pos]][fieldPos]);
                        arr.push(dataArray[filteredPositions[pos]][valuePol]);

                        distinctValues.push(arr);
                    }
                }

            }
        }
        // This distinct will work if array has text and value both. Filtering Distinct value based on Value.
        var arrDistinct = [];        
        $(distinctValues).each(function (index, item) {
            var found = 0;
            if(index==0)
                arrDistinct.push(item);
            $(arrDistinct).each(function (i, value) {
                if (item[1] == value[1]) {
                    found = 1;
                }                
            });
            if (found == 0) {
                arrDistinct.push(item);
            }            
        });

        return arrDistinct;
    }
}

function PopulateDefaultAssesementGraded() {
    $.ajax({
        type: "POST",
        url: "./AssessmentItemUsageReport.aspx/PopulateDefaultGrade",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(textStatus + "\n" + errorThrown);
        },
        success: function (result) {
            var data = [];
            var data1 = [];
            data = JSON.parse(result.d);
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    data1.push([data[i].Grade]);
                }
            } else {
                //No classes found, do something?
            }
            GradeController.PopulateList(data1, 0, 1);
        }
    });
}


