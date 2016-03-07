<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LrmiTagsSearch.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.LrmiTagsSearch" %>
<style>
    .hoverx:hover {
        cursor: pointer
    }
    .chkboxList {
    margin-top: 1px;height: 25px; width: 180px; line-height: 25px; display: block; clear: both; border: 1px solid black; border-top-left-radius: 10px; border-top-right-radius: 30px; border-bottom-left-radius: 30px; border-bottom-right-radius: 10px;
    }
</style>
<script type="text/javascript">
    $(document).ready(function () { reloadSelections(); });
    var isOpen = false;
    var openToolTip = "";

    var selectionAction = {
        none: 0,
        add: 1,
        remove: 2
    };

    var gradesList = {
        selectedGrades: {
            list: {},
            containsUniqueKey: function(uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function(key) {
                for (var grade in this.list) {
                    if (this.list[grade].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(selectionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.selectedGrades.containsUniqueKey(uniqueKey)) {
                this.selectedGrades.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.selectedGrades.list[uniqueKey] = criterion;
            }
        },
        remove: function (key) {
            //this.addAction(selectionAction.remove, key, key, 0);
            this.removeAction(key + "_" + key);
        },
        removeAction: function (uniqueKey) {
            if (this.selectedGrades.containsUniqueKey(uniqueKey)) {
                delete this.selectedGrades.list[uniqueKey];
            }
        },
        ignore: function (key) {
            if (!this.selectedGrades.containsKey(key)) {
                return;
            }

            this.selectedGrades.list[key].action = selectionAction.none;
        },
        clearActions: function () {
            this.selectedGrades.list = {};
        },
        buildActionStrings: function () {
            var deletionString = "";
            var updateString = "";

            for (var key in this.selectedGrades.list) {
                switch (this.selectedGrades.list[key].action) {
                case selectionAction.add:
                    updateString += this.selectedGrades.list[key].key + "~" + this.selectedGrades.list[key].value + ";";
                    break;

                case selectionAction.remove:
                    deletionString += this.selectedGrades.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    var subjectsList = {
        selectedSubjects: {
            list: {},
            containsUniqueKey: function (uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function (key) {
                for (var subject in this.list) {
                    if (this.list[subject].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(selectionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.selectedSubjects.containsUniqueKey(uniqueKey)) {
                this.selectedSubjects.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.selectedSubjects.list[uniqueKey] = criterion;
            }
        },
        remove: function (key) {
            this.removeAction(key + "_" + key);
        },
        removeAction: function (uniqueKey) {
            if (this.selectedSubjects.containsUniqueKey(uniqueKey)) {
                delete this.selectedSubjects.list[uniqueKey];
            }
        },
        ignore: function (key) {
            if (!this.selectedSubjects.containsKey(key)) {
                return;
            }

            this.selectedSubjects.list[key].action = selectionAction.none;
        },
        clearActions: function () {
            this.selectedSubjects.list = {};
        },
        buildActionStrings: function () {
            var deletionString = "";
            var updateString = "";

            for (var key in this.selectedSubjects.list) {
                switch (this.selectedSubjects.list[key].action) {
                case selectionAction.add:
                    updateString += this.selectedSubjects.list[key].key + "~" + this.selectedSubjects.list[key].value + ";";
                    break;

                case selectionAction.remove:
                    deletionString += this.selectedSubjects.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    var learningResourcesList = {
        selectedlearningResources: {
            list: {},
            containsUniqueKey: function (uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function (key) {
                for (var learningResource in this.list) {
                    if (this.list[learningResource].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(selectionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.selectedlearningResources.containsUniqueKey(uniqueKey)) {
                this.selectedlearningResources.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.selectedlearningResources.list[uniqueKey] = criterion;
            }
        },
        remove: function (key) {
            this.removeAction(key + "_" + key, key);
        },
        removeAction: function (uniqueKey, key) {
            removeCriteriaOptionByKey(key, this.selectedlearningResources.list);
        },
        ignore: function (key) {
            if (!this.selectedlearningResources.containsKey(key)) {
                return;
            }

            this.selectedlearningResources.list[key].action = selectionAction.none;
        },
        clearActions: function () {
            this.selectedlearningResources.list = {};
        },
        buildActionStrings: function () {
            var deletionString = "";
            var updateString = "";

            for (var key in this.selectedlearningResources.list) {
                switch (this.selectedlearningResources.list[key].action) {
                case selectionAction.add:
                    updateString += this.selectedlearningResources.list[key].key + "~" + this.selectedlearningResources.list[key].value + ";";
                    break;

                case selectionAction.remove:
                    deletionString += this.selectedlearningResources.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    var educationalUsesList = {
        selectedEducationalUses: {
            list: {},
            containsUniqueKey: function (uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function (key) {
                for (var educationalUse in this.list) {
                    if (this.list[educationalUse].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(selectionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.selectedEducationalUses.containsUniqueKey(uniqueKey)) {
                this.selectedEducationalUses.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.selectedEducationalUses.list[uniqueKey] = criterion;
            }
        },
        remove: function (key) {
            this.removeAction(key + "_" + key, key);
        },
        removeAction: function (uniqueKey, key) {
            removeCriteriaOptionByKey(key, this.selectedEducationalUses.list);
        },
        ignore: function (key) {
            if (!this.selectedEducationalUses.containsKey(key)) {
                return;
            }

            this.selectedEducationalUses.list[key].action = selectionAction.none;
        },
        clearActions: function () {
            this.selectedEducationalUses.list = {};
        },
        buildActionStrings: function () {
            var deletionString = "";
            var updateString = "";

            for (var key in this.selectedEducationalUses.list) {
                switch (this.selectedEducationalUses.list[key].action) {
                case selectionAction.add:
                    updateString += this.selectedEducationalUses.list[key].key + "~" + this.selectedEducationalUses.list[key].value + ";";
                    break;

                case selectionAction.remove:
                    deletionString += this.selectedEducationalUses.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    var endUsersList = {
        selectedEndUsers: {
            list: {},
            containsUniqueKey: function (uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function (key) {
                for (var endUser in this.list) {
                    if (this.list[endUser].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(selectionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.selectedEndUsers.containsUniqueKey(uniqueKey)) {
                this.selectedEndUsers.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.selectedEndUsers.list[uniqueKey] = criterion;
            }
        },
        remove: function (key) {
            this.removeAction(key + "_" + key, key);
        },
        removeAction: function (uniqueKey, key) {
            removeCriteriaOptionByKey(key, this.selectedEndUsers.list);
        },
        ignore: function (key) {
            if (!this.selectedEndUsers.containsKey(key)) {
                return;
            }

            this.selectedEndUsers.list[key].action = selectionAction.none;
        },
        clearActions: function () {
            this.selectedEndUsers.list = {};
        },
        buildActionStrings: function () {
            var deletionString = "";
            var updateString = "";

            for (var key in this.selectedEndUsers.list) {
                switch (this.selectedEndUsers.list[key].action) {
                case selectionAction.add:
                    updateString += this.selectedEndUsers.list[key].key + "~" + this.selectedEndUsers.list[key].value + ";";
                    break;

                case selectionAction.remove:
                    deletionString += this.selectedEndUsers.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    var usageRightsList = {
        selectedUsageRights: {
            list: {},
            containsUniqueKey: function (uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function (key) {
                for (var usageRight in this.list) {
                    if (this.list[usageRight].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(selectionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.selectedUsageRights.containsUniqueKey(uniqueKey)) {
                this.selectedUsageRights.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.selectedUsageRights.list[uniqueKey] = criterion;
            }
        },
        remove: function (key) {
            this.removeAction(key + "_" + key, key);
        },
        removeAction: function (uniqueKey, key) {
            removeCriteriaOptionByKey(key, this.selectedUsageRights.list);
        },
        ignore: function (key) {
            if (!this.selectedUsageRights.containsKey(key)) {
                return;
            }

            this.selectedUsageRights.list[key].action = selectionAction.none;
        },
        clearActions: function () {
            this.selectedUsageRights.list = {};
        },
        buildActionStrings: function () {
            var deletionString = "";
            var updateString = "";

            for (var key in this.selectedUsageRights.list) {
                switch (this.selectedUsageRights.list[key].action) {
                case selectionAction.add:
                    updateString += this.selectedUsageRights.list[key].key + "~" + this.selectedUsageRights.list[key].value + ";";
                    break;

                case selectionAction.remove:
                    deletionString += this.selectedUsageRights.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    var mediaTypesList = {
        selectedMediaTypes: {
            list: {},
            containsUniqueKey: function (uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function (key) {
                for (var mediaType in this.list) {
                    if (this.list[mediaType].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(selectionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.selectedMediaTypes.containsUniqueKey(uniqueKey)) {
                this.selectedMediaTypes.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.selectedMediaTypes.list[uniqueKey] = criterion;
            }
        },
        remove: function (key) {
            this.removeAction(key + "_" + key, key);
        },
        removeAction: function (uniqueKey, key) {
            removeCriteriaOptionByKey(key, this.selectedMediaTypes.list);
        },
        ignore: function (key) {
            if (!this.selectedMediaTypes.containsKey(key)) {
                return;
            }

            this.selectedMediaTypes.list[key].action = selectionAction.none;
        },
        clearActions: function () {
            this.selectedMediaTypes.list = {};
        },
        buildActionStrings: function () {
            var deletionString = "";
            var updateString = "";

            for (var key in this.selectedMediaTypes.list) {
                switch (this.selectedMediaTypes.list[key].action) {
                case selectionAction.add:
                    updateString += this.selectedMediaTypes.list[key].key + "~" + this.selectedMediaTypes.list[key].value + ";";
                    break;

                case selectionAction.remove:
                    deletionString += this.selectedMediaTypes.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    var languagesList = {
        selectedLanguages: {
            list: {},
            containsUniqueKey: function (uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function (key) {
                for (var language in this.list) {
                    if (this.list[language].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(selectionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.selectedLanguages.containsUniqueKey(uniqueKey)) {
                this.selectedLanguages.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.selectedLanguages.list[uniqueKey] = criterion;
            }
        },
        remove: function (key) {
            this.removeAction(key + "_" + key, key);
        },
        removeAction: function (uniqueKey, key) {
            removeCriteriaOptionByKey(key, this.selectedLanguages.list);
        },
        ignore: function (key) {
            if (!this.selectedLanguages.containsKey(key)) {
                return;
            }

            this.selectedLanguages.list[key].action = selectionAction.none;
        },
        clearActions: function () {
            this.selectedLanguages.list = {};
        },
        buildActionStrings: function () {
            var deletionString = "";
            var updateString = "";

            for (var key in this.selectedLanguages.list) {
                switch (this.selectedLanguages.list[key].action) {
                case selectionAction.add:
                    updateString += this.selectedLanguages.list[key].key + "~" + this.selectedLanguages.list[key].value + ";";
                    break;

                case selectionAction.remove:
                    deletionString += this.selectedLanguages.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    var ageAppropriateList = {
        selectedAgeAppropriate: {
            list: {},
            containsUniqueKey: function (uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function (key) {
                for (var age in this.list) {
                    if (this.list[age].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(selectionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.selectedAgeAppropriate.containsUniqueKey(uniqueKey)) {
                this.selectedAgeAppropriate.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.selectedAgeAppropriate.list[uniqueKey] = criterion;
            }
        },
        remove: function (key) {
            this.removeAction(key + "_" + key, key);
        },
        removeAction: function (uniqueKey, key) {
            removeCriteriaOptionByKey(key, this.selectedAgeAppropriate.list);
        },
        ignore: function (key) {
            if (!this.selectedAgeAppropriate.containsKey(key)) {
                return;
            }

            this.selectedAgeAppropriate.list[key].action = selectionAction.none;
        },
        clearActions: function () {
            this.selectedAgeAppropriate.list = {};
        },
        buildActionStrings: function () {
            var deletionString = "";
            var updateString = "";

            for (var key in this.selectedAgeAppropriate.list) {
                switch (this.selectedAgeAppropriate.list[key].action) {
                case selectedAgeAppropriate.add:
                    updateString += this.selectedAgeAppropriate.list[key].key + "~" + this.selectedAgeAppropriate.list[key].value + ";";
                    break;

                case selectionAction.remove:
                    deletionString += this.selectedAgeAppropriate.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    var interactivityTypeList = {
        selectedInteractivityType: {
            list: {},
            containsUniqueKey: function (uniqueKey) {
                if (this.list[uniqueKey] == null) {
                    return false;
                }

                return true;
            },
            containsKey: function (key) {
                for (var interactivityType in this.list) {
                    if (this.list[interactivityType].key == key) {
                        return true;
                    }
                }

                return false;
            }
        },
        add: function (key, value) {
            this.addAction(selectionAction.add, key + "_" + value, key, value);
        },
        addAction: function (action, uniqueKey, key, value) {
            if (this.selectedInteractivityType.containsUniqueKey(uniqueKey)) {
                this.selectedInteractivityType.list[uniqueKey].action = action;
            } else {
                var criterion = {
                    key: key,
                    action: action,
                    value: value
                };

                this.selectedInteractivityType.list[uniqueKey] = criterion;
            }
        },
        remove: function (key) {
            this.removeAction(key + "_" + key, key);
        },
        removeAction: function (uniqueKey, key) {
            removeCriteriaOptionByKey(key, this.selectedInteractivityType.list);
        },
        ignore: function (key) {
            if (!this.selectedInteractivityType.containsKey(key)) {
                return;
            }

            this.selectedInteractivityType.list[key].action = selectionAction.none;
        },
        clearActions: function () {
            this.selectedInteractivityType.list = {};
        },
        buildActionStrings: function () {
            var deletionString = "";
            var updateString = "";

            for (var key in this.selectedInteractivityType.list) {
                switch (this.selectedInteractivityType.list[key].action) {
                case selectedInteractivityType.add:
                    updateString += this.selectedInteractivityType.list[key].key + "~" + this.selectedInteractivityType.list[key].value + ";";
                    break;

                case selectionAction.remove:
                    deletionString += this.selectedInteractivityType.list[key].key + ";";
                    break;
                }
            }

            return { deletionString: deletionString, updateString: updateString };
        }
    };
    var creatorString = "";
    var publisherString = "";
    var textComplexityString = "";
    var readingLevelString = "";
    var timeRequiredString = "00:00:00";
    var timeRequiredDaysString = "0";
    var timeRequiredHoursString = "0";
    var timeRequiredMinutesString = "0";
    var assessedStandardSet = "";
    var assessedGrade = "";
    var assessedSubject = "";
    var assessedCourse = "";
    var assessedStandard = "";
    var selectedStandardId = "";

    $(document).bind('click', function (e) {
        if (!$(e.target).is('#special')) {
            var target = jQuery(e.target);
            if (target.is('img')) {
                //do not handle here, let actual handler fire.
            }
            else if (target.is('input') || target.is('select') || target.is('option')) {
                target.attr('data-selected', 'true');
                e.stopPropagation();
            } else {
                closeCurrentDialogImmediatelyLRMI();
            }
        }
    });

    function clearSelected() {
        gradesList.clearActions();
        subjectsList.clearActions();
        learningResourcesList.clearActions();
        educationalUsesList.clearActions();
        endUsersList.clearActions();
        usageRightsList.clearActions();
        mediaTypesList.clearActions();
        languagesList.clearActions();
        ageAppropriateList.clearActions();
        interactivityTypeList.clearActions();
        clearTextSelected();
        return true;
    }

    function clearTextSelected() {
        creatorString = "";
        publisherString = "";
        textComplexityString = "";
        readingLevelString = "";
        timeRequiredString = "00:00:00";
        timeRequiredDaysString = "0";
        timeRequiredHoursString = "0";
        timeRequiredMinutesString = "0";
        assessedStandardSet = "";
        assessedGrade = "";
        assessedSubject = "";
        assessedCourse = "";
        assessedStandard = "";
        selectedStandardId = "";
    }

    function closeCurrentDialogImmediatelyLRMI() {
        if ($(openToolTip)) {
            $(openToolTip).dialog("close");
        }
        switch (openToolTip.selector) {
            case "#HeaderDivExpandGrades":
                LoadGradeSelections();
                break;
            case "#HeaderDivExpandSubject":
                LoadSubjectSelections();
                break;
            case "#HeaderDivExpandLearningResources":
                LoadLearningResourceSelections();
                break;
            case "#HeaderDivExpandEducationalUse":
                LoadEducationalUseSelections();
                break;
            case "#HeaderDivExpandEndUser":
                LoadEndUsersSelections();
                break;
            case "#HeaderDivExpandCreator":
                LoadCreatorSelection();
                break;
            case "#HeaderDivExpandPublisher":
                LoadPublisherSelection();
                break;
            case "#HeaderDivExpandUsageRights":
                LoadUsageRightsSelections();
                break;
            case "#HeaderDivExpandMediaType":
                LoadMediaTypeSelections();
                break;
            case "#HeaderDivExpandLanguage":
                LoadLanguageSelections();
                break;
            case "#HeaderDivExpandAgeAppropriate":
                LoadAgeAppropriateSelections();
                break;
            case "#HeaderDivExpandTimeRequired":
                LoadTimeRequiredSelections();
                break;
            case "#HeaderDivExpandInteractivityType":
                LoadInteractivityTypeSelections();
                break;
            case "#HeaderDivExpandAssessed":
                LoadAssessedSelections();
                break;
            case "#HeaderDivExpandTextComplexity":
                LoadTextComplexitySelection();
                break;
            case "#HeaderDivExpandReadingLevel":
                LoadReadingLevelSelection();
                break;
        }
    }


    function showTooltipLRMI(whichButton, dialogTarget) {
        var tooltipHeight = '';
        var tooltipWidth = '';
        //debugger;
        tooltipHeight = '600';
        tooltipWidth = '250';
        
        closeCurrentDialogImmediatelyLRMI();
        dialogTarget = $("#" + dialogTarget);
        openToolTip = dialogTarget;
        $(dialogTarget).dialog({
            open: function () {
                $(".ui-dialog-titlebar").hide();
                isOpen = true;

            },
            close: function() {
                isOpen = false;
            },
            resizable: false,
            height: tooltipHeight,
            width: tooltipWidth,
            position: {
                my: "left top", at: "right top", of: $(whichButton), collision: "none"             
            }
        });
        $(dialogTarget).dialog("open");
    }

    function reloadSelections() {
        LoadGradeSelections();
        LoadSubjectSelections();
        LoadLearningResourceSelections();
        LoadEducationalUseSelections();
        LoadEndUsersSelections();
        LoadCreatorSelection();
        LoadPublisherSelection();
        LoadUsageRightsSelections();
        LoadMediaTypeSelections();
        LoadLanguageSelections();
        LoadAgeAppropriateSelections();
        LoadTimeRequiredSelections();
        LoadInteractivityTypeSelections();
        LoadAssessedSelections();
        LoadTextComplexitySelection();
        LoadReadingLevelSelection();       

    }

    function LoadGradeSelections() {
        var id = "GradesUpdateMessage";
        var currText = "";
        var currGrades = "";
        for (var key in gradesList.selectedGrades.list) {
            var theKey = gradesList.selectedGrades.list[key].key;
            var theText = gradesList.selectedGrades.list[key].value;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + theKey + "','textDiv" + theKey + "','" + id + "','" + theKey + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + theKey + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + theKey + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left; font-family: arial; font-size: 10pt\" id=\"textDiv" + theKey + "\" >" + theText + "</span><br><br>";
            currText = currText + imgDiv + textDiv;
            if (currGrades.length != 0) {
                currGrades = currGrades + ",";
            }
            currGrades = currGrades + theKey;
        }
        $("#hdnGrades").val(currGrades);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadSubjectSelections() {
        var id = "SubjectsUpdateMessage";
        var currText = "";
        var currSubjects = "";
        for (var key in subjectsList.selectedSubjects.list) {
            var theKey = subjectsList.selectedSubjects.list[key].key;
            var theText = subjectsList.selectedSubjects.list[key].value;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + theKey + "','textDiv" + theKey + "','" + id + "','" + theKey + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + theKey + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + theKey + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + theKey + "\" >" + theText + "</span><br><br>";
            currText = currText + imgDiv + textDiv;
            
            if (currSubjects.length != 0) {
                currSubjects = currSubjects + ",";
            }
            currSubjects = currSubjects + theKey;
        }
        $("#hdnSubjects").val(currSubjects);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadLearningResourceSelections() {
        var id = "LearningResourceUpdateMessage";
        var currText = "";
        var currResources = "";
        for (var key in learningResourcesList.selectedlearningResources.list) {
            var theKey = learningResourcesList.selectedlearningResources.list[key].key;
            var theText = learningResourcesList.selectedlearningResources.list[key].value;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + theKey + "','textDiv" + theKey + "','" + id + "','" + theKey + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + theKey + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + theKey + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + theKey + "\" >" + theText + "</span><br><br>";
            currText = currText + imgDiv + textDiv;
            if (currResources.length != 0) {
                currResources = currResources + ",";
            }
            currResources = currResources + theKey;
        }
        $("#hdnLearningResources").val(currResources);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadEducationalUseSelections() {
        var id = "EducationalUseUpdateMessage";
        var currText = "";
        var currEducationalUses = "";
        for (var key in educationalUsesList.selectedEducationalUses.list) {
            var theKey = educationalUsesList.selectedEducationalUses.list[key].key;
            var theText = educationalUsesList.selectedEducationalUses.list[key].value;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + theKey + "','textDiv" + theKey + "','" + id + "','" + theKey + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + theKey + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + theKey + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + theKey + "\" >" + theText + "</span><br><br>";
            currText = currText + imgDiv + textDiv;
            if (currEducationalUses.length != 0) {
                currEducationalUses = currEducationalUses + ",";
            }
            currEducationalUses = currEducationalUses + theKey;
        }
        $("#hdnEducationalUses").val(currEducationalUses);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadEndUsersSelections() {
        var id = "EndUserUpdateMessage";
        var currText = "";
        var currEndUsers = "";
        for (var key in endUsersList.selectedEndUsers.list) {
            var theKey = endUsersList.selectedEndUsers.list[key].key;
            var theText = endUsersList.selectedEndUsers.list[key].value;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + theKey + "','textDiv" + theKey + "','" + id + "','" + theKey + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + theKey + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + theKey + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + theKey + "\" >" + theText + "</span><br><br>";
            currText = currText + imgDiv + textDiv;
            if (currEndUsers.length != 0) {
                currEndUsers = currEndUsers + ",";
            }
            currEndUsers = currEndUsers + theKey;
        }
        $("#hdnEndUsers").val(currEndUsers);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadUsageRightsSelections() {
        var id = "UsageUpdateMessage";
        var currText = "";
        var currUsageRights = "";
        for (var key in usageRightsList.selectedUsageRights.list) {
            var theKey = usageRightsList.selectedUsageRights.list[key].key;
            var theText = usageRightsList.selectedUsageRights.list[key].value;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + theKey + "','textDiv" + theKey + "','" + id + "','" + theKey + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + theKey + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + theKey + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + theKey + "\" >" + theText + "</span><br><br>";
            currText = currText + imgDiv + textDiv;
            if (currUsageRights.length != 0) {
                currUsageRights = currUsageRights + ",";
            }
            currUsageRights = currUsageRights + theKey;
        }
        $("#hdnUsageRights").val(currUsageRights);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadMediaTypeSelections() {
        var id = "MediaTypeUpdateMessage";
        var currText = "";
        var currMediaTypes = "";
        for (var key in mediaTypesList.selectedMediaTypes.list) {
            var theKey = mediaTypesList.selectedMediaTypes.list[key].key;
            var theText = mediaTypesList.selectedMediaTypes.list[key].value;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + theKey + "','textDiv" + theKey + "','" + id + "','" + theKey + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + theKey + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + theKey + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + theKey + "\" >" + theText + "</span><br><br>";
            currText = currText + imgDiv + textDiv;
            if (currMediaTypes.length != 0) {
                currMediaTypes = currMediaTypes + ",";
            }
            currMediaTypes = currMediaTypes + theKey;
        }
        $("#hdnMediaTypes").val(currMediaTypes);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadLanguageSelections() {
        var id = "LanguageUpdateMessage";
        var currText = "";
        var currLanguages = "";
        for (var key in languagesList.selectedLanguages.list) {
            var theKey = languagesList.selectedLanguages.list[key].key;
            var theText = languagesList.selectedLanguages.list[key].value;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + theKey + "','textDiv" + theKey + "','" + id + "','" + theKey + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + theKey + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + theKey + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + theKey + "\" >" + theText + "</span><br><br>";
            currText = currText + imgDiv + textDiv;
            if (currLanguages.length != 0) {
                currLanguages = currLanguages + ",";
            }
            currLanguages = currLanguages + theKey;
        }
        $("#hdnLanguages").val(currLanguages);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadAgeAppropriateSelections() {
        var id = "AgeAppropriateUpdateMessage";
        var currText = "";
        var currAgeAppropriate = "";
        for (var key in ageAppropriateList.selectedAgeAppropriate.list) {
            var theKey = ageAppropriateList.selectedAgeAppropriate.list[key].key;
            var theText = ageAppropriateList.selectedAgeAppropriate.list[key].value;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + theKey + "','textDiv" + theKey + "','" + id + "','" + theKey + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + theKey + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + theKey + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + theKey + "\" >" + theText + "</span><br><br>";
            currText = currText + imgDiv + textDiv;
            if (currAgeAppropriate.length != 0) {
                currAgeAppropriate = currAgeAppropriate + ",";
            }
            currAgeAppropriate = currAgeAppropriate + theKey;
        }
        $("#hdnAgeAppropriate").val(currAgeAppropriate);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadInteractivityTypeSelections() {
        var id = "InteractivityTypeUpdateMessage";
        var currText = "";
        var currInteractivity = "";
        for (var key in interactivityTypeList.selectedInteractivityType.list) {
            var theKey = interactivityTypeList.selectedInteractivityType.list[key].key;
            var theText = interactivityTypeList.selectedInteractivityType.list[key].value;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + theKey + "','textDiv" + theKey + "','" + id + "','" + theKey + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + theKey + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + theKey + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + theKey + "\" >" + theText + "</span><br><br>";
            currText = currText + imgDiv + textDiv;
            if (currInteractivity.length != 0) {
                currInteractivity = currInteractivity + ",";
            }
            currInteractivity = currInteractivity + theKey;
        }
        $("#hdnInteractivity").val(currInteractivity);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadCreatorSelection() {
        var id = "CreatorUpdateMessage";
        var currText = "";
        var key = 1;
        if (creatorString != '') {
            currText = $("#" + id).val();
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + key + "','textDiv" + key + "','" + id + "','" + key + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + key + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + key + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + key + "\" >" + creatorString + "</span>";
            currText = currText + imgDiv + textDiv;
        }
        $("#hdnCreator").val(creatorString);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadPublisherSelection() {
        var id = "PublisherUpdateMessage";
        var currText = "";
        var key = 1;
        if (publisherString != '') {
            currText = $("#" + id).val();
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + key + "','textDiv" + key + "','" + id + "','" + key + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + key + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + key + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + key + "\" >" + publisherString + "</span>";
            currText = currText + imgDiv + textDiv;
        }
        $("#hdnPublisher").val(publisherString);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadTextComplexitySelection() {
        var id = "TextComplexityUpdateMessage";
        var currText = "";
        var key = 1;
        if (textComplexityString != '') {
            currText = $("#" + id).val();
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + key + "','textDiv" + key + "','" + id + "','" + key + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + key + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + key + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + key + "\" >" + textComplexityString + "</span>";
            currText = currText + imgDiv + textDiv;
        }
        $("#hdnTextComplexity").val(textComplexityString);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadReadingLevelSelection() {
        var id = "ReadingLevelUpdateMessage";
        var currText = "";
        var key = 1;
        if (readingLevelString != '') {
            currText = $("#" + id).val();
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + key + "','textDiv" + key + "','" + id + "','" + key + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + key + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + key + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + key + "\" >" + readingLevelString + "</span>";
            currText = currText + imgDiv + textDiv;
        }
        $("#hdnReadingLevel").val(readingLevelString);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadTimeRequiredSelections() {
        var id = "TimeRequiredUpdateMessage";
        var currText = "";
        if (timeRequiredDaysString != '0' || timeRequiredHoursString != "0" || timeRequiredMinutesString != "0") {
            if (timeRequiredDaysString.length == 1) {
                timeRequiredDaysString = "0" + timeRequiredDaysString;
            }
            if (timeRequiredHoursString.length == 1) {
                timeRequiredHoursString = "0" + timeRequiredHoursString;
            }
            if (timeRequiredMinutesString.length == 1) {
                timeRequiredMinutesString = "0" + timeRequiredMinutesString;
            }
            timeRequiredString = timeRequiredDaysString + ":" + timeRequiredHoursString + ":" + timeRequiredMinutesString;
            currText = $("#" + id).val();
            var key = 1;
            var jsOnClick = "removeCriterionLRMI(this,'imgDiv" + key + "','textDiv" + key + "','" + id + "','" + key + "');";
            var imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + key + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + key + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            var textDiv = "<span style=\"float:left;\" id=\"textDiv" + key + "\" >" + timeRequiredString + "</span>";
            currText = currText + imgDiv + textDiv;
        }
        $("#hdnTimeRequired").val(timeRequiredString);
        $("#" + id).text(currText);
        $("#" + id).html(currText);
        $("#" + id).show("fast");
    }

    function LoadAssessedSelections() {
        var id = "AssessedUpdateMessage";
        var currText = "";
        var key = 1;
        var jsOnClick = '';
        var imgDiv = '';
        var textDiv = '';
        var selectionString = "";
        if (assessedStandardSet != '') {
            selectionString = assessedStandardSet;
            $("#hdnAssessedStandardSet").val(assessedStandardSet);
        }
        if (assessedGrade != '') {
            selectionString += "<br/>" + assessedGrade;
            $("#hdnAssessedGrade").val(assessedGrade);
        }
        if (assessedSubject != '') {
            selectionString += "<br/>" + assessedSubject;
            $("#hdnAssessedSubject").val(assessedSubject);
        }
        if (assessedCourse != '') {
            selectionString += "<br/>" + assessedCourse;
            $("#hdnAssessedCourse").val(assessedCourse);
        }
        if (assessedStandard != '') {
            selectionString += "<br/>" + assessedStandard;
            $("#hdnAssessedStandard").val(assessedStandard);
        }

        if (selectionString != "") {
            jsOnClick = "removeCriterionLRMI(this,'imgDiv" + key + "','textDiv" + key + "','" + id + "','" + key + "');";
            imgDiv = "<div style=\"float:left;\" id=\"imgDiv" + key + "\">";
            imgDiv += "<a href=\"#\"><img id=\"img" + key + "\" src=\"../../Images/close_x.gif\" onclick=\"" + jsOnClick + "\"/></a>";
            imgDiv += "</div>";
            textDiv = "<span style=\"float:left;\" id=\"textDiv" + key + "\" >" + selectionString + "</span>";
            currText = currText + imgDiv + textDiv + "<br/>";
        }
        $("#" + id).text(currText);
            $("#" + id).html(currText);
            $("#" + id).show("fast");
    }
    
    function onTimeRequiredChanged(sender) {
        //handles all TimeRequired change events - text box and dropdownlist
        var senderName = sender.id;
        //var index = eventArgs.selectedIndex;
        //var items = sender.get_items();
        //var item = items.getItem(index);
        if (senderName == "rdlTimeRequiredHours") {
            timeRequiredHoursString = sender.value;
        } else {
            timeRequiredMinutesString = sender.value;
        }
    }

    function onTextChanged(sender, eventArgs) {
        var itemText = eventArgs.get_newValue();
        var key = "Creator";
        var senderName = sender.get_id();
        if (senderName == "tbCreator") {
            creatorString = itemText;
        } else if (senderName == "tbPublisher") {
            publisherString = itemText;
        } else if (senderName == "tbTextComplexity") {
            textComplexityString = itemText;
        } else if (senderName == "tbReadingLevel") {
            readingLevelString = itemText;
        } else if (senderName == "tbTimeRequiredDays") {
            timeRequiredDaysString = itemText;
        }
    }

    function onItemCheckedLRMI(sender, eventArgs) {
        var item = eventArgs.get_item();
        var itemText = item.get_text();
        var key = item.get_value();
        var senderName = sender.get_id();
        var id = "";
        var currText = "";

        if (senderName == "rlbGrades") {
            if (item.get_checked() == true) {
                gradesList.add(key, itemText);
            } else {
                gradesList.remove(key, itemText);
            }
        } else if (senderName == "rlbSubjects") {
            if (item.get_checked() == true) {
                subjectsList.add(key, itemText);
            } else {
                subjectsList.remove(key, itemText);
            }
        } else if (senderName == "rlbLearningResources") {
            if (item.get_checked() == true) {
                learningResourcesList.add(key, itemText);
            } else {
                learningResourcesList.remove(key, itemText);
            }
        } else if (senderName == "rlbEducationalUse") {
            if (item.get_checked() == true) {
                educationalUsesList.add(key, itemText);
            } else {
                educationalUsesList.remove(key, itemText);
            }
        } else if (senderName == "rlbEndUser") {
            if (item.get_checked() == true) {
                endUsersList.add(key, itemText);
            } else {
                endUsersList.remove(key, itemText);
            }
        } else if (senderName == "rlbUsageRights") {
            if (item.get_checked() == true) {
                usageRightsList.add(key, itemText);
            } else {
                usageRightsList.remove(key, itemText);
            }
        } else if (senderName == "rlbMediaType") {
            if (item.get_checked() == true) {
                mediaTypesList.add(key, itemText);
            } else {
                mediaTypesList.remove(key, itemText);
            }
        } else if (senderName == "rlbLanguage") {
            if (item.get_checked() == true) {
                languagesList.add(key, itemText);
            } else {
                languagesList.remove(key, itemText);
            }
        } else if (senderName == "rlbAgeAppropriate") {
            if (item.get_checked() == true) {
                ageAppropriateList.add(key, itemText);
            } else {
                ageAppropriateList.remove(key, itemText);
            }
        } else if (senderName == "rlbInteractivity") {
            if (item.get_checked() == true) {
                interactivityTypeList.add(key, itemText);
            } else {
                interactivityTypeList.remove(key, itemText);
            }
        }
    }

    function uncheckAll(dropdownlist) {
        var combo = $find(dropdownlist);
        for (var i = 0; i < combo.get_items().get_Count(); i++) {
            combo.get_items().getItem(1).uncheck();
        }
    }

    function uncheckSelected(dropdownlist, key) {
        var combo = $find(dropdownlist);
        var item = combo.findItemByValue(key);

        item.uncheck();
    }

    function removeCriterionLRMI(div, theimgdiv, thetextdiv, messagediv, key) {
        $("#" + thetextdiv).css('text-decoration', 'line-through');
        $("#" + theimgdiv).attr('src', '../../Images/commands/exclamation_red.png');
        $("#" + theimgdiv).attr('onclick', 'javascript:void(0);');
        switch (messagediv) {
            case "GradesUpdateMessage":
                uncheckSelected("rlbGrades", key);
                gradesList.remove(key);
                LoadGradeSelections();
                break;
            case "SubjectsUpdateMessage":
                uncheckSelected("rlbSubjects", key);
                subjectsList.remove(key);
                LoadSubjectSelections();
                break;
            case "LearningResourceUpdateMessage":
                uncheckSelected("rlbLearningResources", key);
                learningResourcesList.remove(key);
                LoadLearningResourceSelections();
                break;
            case "EducationalUseUpdateMessage":
                uncheckSelected("rlbEducationalUse", key);
                educationalUsesList.remove(key);
                LoadEducationalUseSelections();
                break;
            case "EndUserUpdateMessage":
                uncheckSelected("rlbEndUser", key);
                endUsersList.remove(key);
                LoadEndUsersSelections();
                break;
            case "CreatorUpdateMessage":
                creatorString = "";
                LoadCreatorSelection();
                break;
            case "PublisherUpdateMessage":
                publisherString = "";
                LoadPublisherSelection();
                break;
            case "UsageUpdateMessage":
                uncheckSelected("rlbUsageRights", key);
                usageRightsList.remove(key);
                LoadUsageRightsSelections();
                break;
            case "MediaTypeUpdateMessage":
                uncheckSelected("rlbMediaType", key);
                mediaTypesList.remove(key);
                LoadMediaTypeSelections();
                break;
            case "LanguageUpdateMessage":
                uncheckSelected("rlbLanguage", key);
                languagesList.remove(key);
                LoadLanguageSelections();
                break;
            case "AgeAppropriateUpdateMessage":
                uncheckSelected("rlbAgeAppropriate", key);
                ageAppropriateList.remove(key);
                LoadAgeAppropriateSelections();
                break;
            case "TimeRequiredUpdateMessage":
                timeRequiredString = "00:00:00";
                timeRequiredDaysString = "0";
                timeRequiredHoursString = "0";
                timeRequiredMinutesString = "0";
                LoadTimeRequiredSelections();
                break;
            case "AssessedUpdateMessage":
                assessedStandardSet = "";
                assessedGrade = "";
                assessedSubject = "";
                assessedCourse = "";
                assessedStandard = "";
                LoadAssessedSelections();
                break;
            case "TextComplexityUpdateMessage":
                textComplexityString = "";
                LoadTextComplexitySelection();
                break;
            case "ReadingLevelUpdateMessage":
                readingLevelString = "";
                LoadReadingLevelSelection();
                break;
            case "InteractivityTypeUpdateMessage":
                uncheckSelected("rlbInteractivity", key);
                interactivityTypeList.remove(key);
                LoadInteractivityTypeSelections();
                break;
        }
    }

    function uncheckItem() {

    }

    function KeyPress(sender, args) {
        if (args.get_keyCharacter() == sender.get_numberFormat().DecimalSeparator) {
            args.set_cancel(true);
        }
    }

    function PopulateGrade(standardElement, eventArgs) {
        var standardSet = $(standardElement).val();
        var ctl = $("#rlbAssessedGrade");
        var dllSubject = $("#rlbAssessedSubject");
        var dllCourse = $("#rlbAssessedCourse");
        var dllStandards = $("#rlbAssessedStandard");
        assessedStandardSet = standardSet;
        assessedGrade = '';
        $(dllSubject).empty();
        assessedSubject = '';
        $(dllCourse).empty();
        assessedCourse = '';
        $(dllStandards).empty();
        assessedStandard = '';
        $("#hdnAssessed").val('');
        $(dllSubject).append($('<option></option>').val("0").html("Select Subject"));
        $(dllCourse).append($('<option></option>').val("0").html("Select Course"));
        $(dllStandards).append($('<option></option>').val("0").html("Select Standards"));
        assessedStandardSet = standardSet;

        $.ajax({
            type: "POST",
            url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardGrade',
            data: "{'standardSet':'" + standardSet + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            error: function (xmlHttpRequest, textStatus, errorThrown) {
                alert("LRMI " + errorThrown);
            },
            success: function (result) {
                if (ctl) {
                    var str = result.d;
                    var correctedString = str.replace(/\"/g, '\'');
                    var sValue = $("#rlbAssessedGrade").val();
                    $("#rlbAssessedGrade").html(correctedString);
                    $("#rlbAssessedGrade").val(sValue);
                }
            }
        });
    }


    function PopulateSubject(standardGrade) {
        var standardSetctrl = $("#rlbAssessedStandardSet");
        var standardSet = $(standardSetctrl).val();
        var grade = $(standardGrade).val();
        var ctl = $("#rlbAssessedSubject");
        var dllCourse =  $("#rlbAssessedCourse");
        var dllStandards = $("#rlbAssessedStandard");
        $(ctl).empty();
        $(dllCourse).empty();
        $(dllStandards).empty();
        $(ctl).append($('<option></option>').val("0").html("Select Subject"));
        $(dllCourse).append($('<option></option>').val("0").html("Select Course"));
        $(dllStandards).append($('<option></option>').val("0").html("Select Standards"));
        assessedGrade = grade;
        assessedSubject = '';
        assessedCourse = '';
        assessedStandard = '';
        $("#hdnAssessed").val('');
        $.ajax({
            type: "POST",
            url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardSubject',
            data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("LRMI " + errorThrown);
            },
            success: function (result) {
                if (ctl) {
                    var str = result.d;
                    var correctedString = str.replace(/\"/g, '\'');
                    var sValue = $("#rlbAssessedSubject").val();
                    $("#rlbAssessedSubject").html(correctedString);
                    $("#rlbAssessedSubject").val(sValue);
                }
            }
        });
    }

    function PopulateCourse(standardSubject) {
        var standardSetctrl = $("#rlbAssessedStandardSet");
        var standardSet = $(standardSetctrl).val();
        var gradeControl = $("#rlbAssessedGrade");
        var grade = $(gradeControl).val();
        var subject = $(standardSubject).val();
        var ctl = $("#rlbAssessedCourse");
        var dllStandards = $("#rlbAssessedStandard");

        $(ctl).empty();
        $(dllStandards).empty();
        $(ctl).append($('<option></option>').val("0").html("Select Course"));
        $(dllStandards).append($('<option></option>').val("0").html("Select Standards"));
        assessedSubject = subject;
        assessedCourse = '';
        assessedStandard = '';
        $("#hdnAssessed").val('');

        $.ajax({
            type: "POST",
            url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardCourse',
            data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "','subject':'" + subject + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("LRMI " + errorThrown);
            },
            success: function (result) {
                if (ctl) {
                    var str = result.d;
                    var correctedString = str.replace(/\"/g, '\'');
                    var sValue = $("#rlbAssessedCourse").val();
                    $("#rlbAssessedCourse").html(correctedString);
                    $("#rlbAssessedCourse").val(sValue);
                }
            }
        });
    }

    function PopulateStandard(standardCourse) {
        var standardSetctrl = $("#rlbAssessedStandardSet");
        var standardSet = $(standardSetctrl).val();
        var gradeControl = $("#rlbAssessedGrade");
        var grade = $(gradeControl).val();
        var subjectControl = $("#rlbAssessedSubject");
        var subject = $(subjectControl).val();
        var course = $(standardCourse).val();
        assessedCourse = course;
        assessedStandard = '';
        $("#hdnAssessed").val('');
        var ctl = $("#rlbAssessedStandard");
        $.ajax({
            type: "POST",
            url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardsByStandardSetGradeSubjectCourse',
            data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "','subject':'" + subject + "','course':'" + course + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("LRMI " + errorThrown);
            },
            success: function (result) {
                if (ctl) {
                    var str = result.d;
                    var correctedString = str.replace(/\"/g, '\'');
                    var sValue = $("#rlbAssessedStandard").val();
                    $("#rlbAssessedStandard").html(correctedString);
                    $("#rlbAssessedStandard").val(sValue);
                }
            }
        });
    }

    function selectStandardsCriteriaValue(standards) {
        var option = $(standards).find("option:selected").val();
        assessedStandard = $(standards).find("option:selected").text();
        $("#hdnAssessed").val(option);
    }

    /*
        This function searches the specified criteria values list,
        and removes that criteria from list of selected criteria options.
    */
    function removeCriteriaOptionByKey(key, resourceList) {
        for (var resource in resourceList) {
            if (resourceList[resource].key == key) {
                delete resourceList[resource];
            }
        }
    }
</script>
<div style="width: 200px; vertical-align: middle; text-align: center;">
    <telerik:RadButton ID="btnUpdateCriteria" runat="server" Text="Search" ToolTip="Update report criteria"
        Skin="Web20" OnClick="RadButtonSearch_Click" AutoPostBack="true" OnClientLoad="reloadSelections"
        Style="font-weight: bold; margin-top: 5px; margin-bottom: 5px;">
    </telerik:RadButton>
    <telerik:RadButton runat="server" ID="RadButtonClear" Text="Clear" ToolTip="Clear report criteria"
        Skin="Web20" 
        Style="font-weight: bold; margin-top: 5px; margin-bottom: 5px;" 
        onclick="RadButtonClear_Click"
        OnClientClicked="clearSelected"
        AutoPostBack="True">
    </telerik:RadButton>
</div>
<div style="width: 200px; height: 560px; vertical-align: middle; text-align: center; overflow: auto; overflow-x: hidden">
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivGrades">
        <div class="left" style="text-align: left">Grades:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivGrades', 'HeaderDivExpandGrades');">
        <asp:Image ID="imgGrades" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>
    </div>
    <div class="criteriaUpdateMessageDiv" id="GradesUpdateMessage"></div>
    <div id="HeaderDivExpandGrades" runat="server" ClientIDMode="Static" style="overflow:hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadListBox runat="server" ID="rlbGrades" ClientIDMode="Static" Width="195px"
                AutoPostBack="False" CheckBoxes="True" Skin="Vista"></telerik:RadListBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivSubjects">
        <div class="left" style="text-align: left">Educational Subject:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivSubjects', 'HeaderDivExpandSubject');">
        <asp:Image ID="imgSubjects" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="SubjectsUpdateMessage"></div>
    <div id="HeaderDivExpandSubject" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadListBox runat="server" ID="rlbSubjects" ClientIDMode="Static" Width="225px"
                AutoPostBack="False" CheckBoxes="True" Skin="Vista" CssClass="HeightLimited"></telerik:RadListBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivLearningResources">
        <div class="left" style="text-align: left">Learning Resource:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivLearningResources', 'HeaderDivExpandLearningResources');">
        <asp:Image ID="imgLearningResources" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>
    </div>
    <div class="criteriaUpdateMessageDiv" id="LearningResourceUpdateMessage"></div>
    <div id="HeaderDivExpandLearningResources" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadListBox runat="server" ID="rlbLearningResources" ClientIDMode="Static" Width="195px"
                AutoPostBack="False" CheckBoxes="True" Skin="Vista"></telerik:RadListBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivEducationalUse">
        <div class="left" style="text-align: left">Educational Use:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivEducationalUse', 'HeaderDivExpandEducationalUse');">
        <asp:Image ID="imgEducationalUse" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="EducationalUseUpdateMessage"></div>
    <div id="HeaderDivExpandEducationalUse" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadListBox runat="server" ID="rlbEducationalUse" ClientIDMode="Static" Width="195px"
                AutoPostBack="False" CheckBoxes="True" Skin="Vista"></telerik:RadListBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivEndUser">
        <div class="left" style="text-align: left">End User:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivEndUser', 'HeaderDivExpandEndUser');">
        <asp:Image ID="imgEndUser" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="EndUserUpdateMessage"></div>
    <div id="HeaderDivExpandEndUser" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadListBox runat="server" ID="rlbEndUser" ClientIDMode="Static" Width="195px"
                AutoPostBack="False" CheckBoxes="True" Skin="Vista"></telerik:RadListBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivCreator">
        <div class="left" style="text-align: left">Creator:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivCreator', 'HeaderDivExpandCreator');">
        <asp:Image ID="imgCreator" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="CreatorUpdateMessage"></div>
    <div id="HeaderDivExpandCreator" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadTextBox runat="server" ID="tbCreator" ClientIDMode="Static" Skin="Vista" AutoPostBack="False" Width="100%">
                <ClientEvents OnValueChanged="onTextChanged" />
            </telerik:RadTextBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivPublisher">
        <div class="left" style="text-align: left">Publisher:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivPublisher', 'HeaderDivExpandPublisher');">
        <asp:Image ID="imgPublisher" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="PublisherUpdateMessage"></div>
    <div id="HeaderDivExpandPublisher" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadTextBox runat="server" ID="tbPublisher" ClientIDMode="Static" Skin="Vista" AutoPostBack="False" Width="195px">
                <ClientEvents OnValueChanged="onTextChanged" />
            </telerik:RadTextBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivUsageRights">
        <div class="left" style="text-align: left">Usage Rights:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivUsageRights', 'HeaderDivExpandUsageRights');">
        <asp:Image ID="imgUsageRights" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="UsageUpdateMessage"></div>
    <div id="HeaderDivExpandUsageRights" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadListBox runat="server" ID="rlbUsageRights" ClientIDMode="Static" Width="195px"
                AutoPostBack="False" CheckBoxes="True" Skin="Vista"></telerik:RadListBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivMediaType">
        <div class="left" style="text-align: left">Media Type:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivMediaType', 'HeaderDivExpandMediaType');">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="MediaTypeUpdateMessage"></div>
    <div id="HeaderDivExpandMediaType" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadListBox runat="server" ID="rlbMediaType" ClientIDMode="Static" Width="195px"
                AutoPostBack="False" CheckBoxes="True" Skin="Vista"></telerik:RadListBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivLanguage">
        <div class="left" style="text-align: left">Language:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivLanguage', 'HeaderDivExpandLanguage');">
        <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="LanguageUpdateMessage"></div>
    <div id="HeaderDivExpandLanguage" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadListBox runat="server" CheckBoxes="True" Width="195px" ClientIDMode="Static" ID="rlbLanguage" Skin="Vista" AutoPostBack="False"></telerik:RadListBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivAgeAppropriate">
        <div class="left" style="text-align: left">Age Appropriate:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivAgeAppropriate', 'HeaderDivExpandAgeAppropriate');">
        <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="AgeAppropriateUpdateMessage"></div>
    <div id="HeaderDivExpandAgeAppropriate" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadListBox runat="server" CheckBoxes="True" Width="195px" ClientIDMode="Static" ID="rlbAgeAppropriate" Skin="Vista" AutoPostBack="False"  CssClass="HeightLimited"></telerik:RadListBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivTimeRequired">
        <div class="left" style="text-align: left">Time Required:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivTimeRequired', 'HeaderDivExpandTimeRequired');">
        <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="TimeRequiredUpdateMessage"></div>
    <div id="HeaderDivExpandTimeRequired" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none; width: 270px">
        <div style="background-color:#CCCCCC; padding: 5px; width: 270px">
            <telerik:RadNumericTextBox runat="server" MinValue="0" MaxValue="999" MaxLength="3"  ID="tbTimeRequiredDays" Width="35px" onchange="onTextChanged" ClientIDMode="Static" Skin="Vista" AutoPostBack="False" ZIndex="1000000">
                <ClientEvents OnValueChanged="onTextChanged" />
                <NumberFormat GroupSeparator="" DecimalDigits="0" AllowRounding="true"   KeepNotRoundedValue="false"  />   
                <ClientEvents OnKeyPress="KeyPress" /> 
            </telerik:RadNumericTextBox>&nbsp;Days&nbsp;
            <asp:DropDownList runat="server" ID="rdlTimeRequiredHours" style="width: 40px"   onchange="onTimeRequiredChanged(this);" ClientIDMode="Static" Skin="Vista" AutoPostBack="False" ZIndex="1000000"></asp:DropDownList>&nbsp;Hours
            <asp:DropDownList runat="server" ID="rdlTimeRequiredMinutes" style="width: 40px" onchange="onTimeRequiredChanged(this);" ClientIDMode="Static" Skin="Vista" AutoPostBack="False" ZIndex="1000000"></asp:DropDownList>&nbsp;Minutes&nbsp;
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivAssessed">
        <div class="left" style="text-align: left">Assesses:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivAssessed', 'HeaderDivExpandAssessed');">
        <asp:Image ID="Image5" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="AssessedUpdateMessage"></div>
    <div id="HeaderDivExpandAssessed" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <asp:DropDownList runat="server" Width="150px" ID="rlbAssessedStandardSet" ClientIDMode="Static" Skin="Vista" AutoPostBack="False" ZIndex="1000000"></asp:DropDownList>
            <asp:DropDownList runat="server" Width="150px" ID="rlbAssessedGrade" ClientIDMode="Static" Skin="Vista" AutoPostBack="False" ZIndex="1000000"></asp:DropDownList>
            <asp:DropDownList runat="server" Width="150px" ID="rlbAssessedSubject" ClientIDMode="Static" Skin="Vista" AutoPostBack="False" ZIndex="1000000"></asp:DropDownList>
            <asp:DropDownList runat="server" Width="150px" ID="rlbAssessedCourse" ClientIDMode="Static" Skin="Vista" AutoPostBack="False" ZIndex="1000000"></asp:DropDownList>
            <asp:DropDownList runat="server" Width="150px" ID="rlbAssessedStandard" ClientIDMode="Static" Skin="Vista" AutoPostBack="False" ZIndex="1000000"></asp:DropDownList>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivTextComplexity">
        <div class="left" style="text-align: left">Text Complexity:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivTextComplexity', 'HeaderDivExpandTextComplexity');">
        <asp:Image ID="Image6" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="TextComplexityUpdateMessage"></div>
    <div id="HeaderDivExpandTextComplexity" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadTextBox runat="server"  ID="tbTextComplexity" ClientIDMode="Static" Skin="Vista" AutoPostBack="False">
                <ClientEvents OnValueChanged="onTextChanged" />
            </telerik:RadTextBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivReadingLevel">
        <div class="left" style="text-align: left">Reading Level:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivReadingLevel', 'HeaderDivExpandReadingLevel');">
        <asp:Image ID="Image7" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="ReadingLevelUpdateMessage"></div>
    <div id="HeaderDivExpandReadingLevel" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadTextBox runat="server"  ID="tbReadingLevel" ClientIDMode="Static" Skin="Vista" AutoPostBack="False">
                <ClientEvents OnValueChanged="onTextChanged" />
            </telerik:RadTextBox>
        </div>
    </div>
    <div class="criteriaHeaderDiv" ClientIDMode="Static" runat="server" id="HeaderDivInteractivityType">
        <div class="left" style="text-align: left">Interactivity Type:</div>
        <span style="padding-right: 5px" onclick="showTooltipLRMI('#HeaderDivInteractivityType', 'HeaderDivExpandInteractivityType');">
        <asp:Image ID="Image8" runat="server" ImageUrl="~/Images/commands/expand_bubble.png" Height="16" Width="16"/>
        </span>        
    </div>
    <div class="criteriaUpdateMessageDiv" id="InteractivityTypeUpdateMessage"></div>
    <div id="HeaderDivExpandInteractivityType" runat="server" class="right" ClientIDMode="Static" style="overflow: hidden;display:none">
        <div style="position: relative; background-color:#CCCCCC; padding: 5px">
            <telerik:RadListBox runat="server" CheckBoxes="True" ID="rlbInteractivity" Width="195px" ClientIDMode="Static" Skin="Vista" AutoPostBack="False"></telerik:RadListBox>
        </div>
    </div>
</div>
<asp:HiddenField ID="hdnGrades" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnSubjects" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnLearningResources" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnEducationalUses" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnEndUsers" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnCreator" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnPublisher" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnUsageRights" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnMediaTypes" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnLanguages" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnAgeAppropriate" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnTimeRequired" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnAssessedStandardSet" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnAssessedGrade" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnAssessedSubject" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnAssessedCourse" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnAssessed" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnTextComplexity" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnReadingLevel" ClientIDMode="Static" runat="server" />
<asp:HiddenField ID="hdnInteractivity" ClientIDMode="Static" runat="server" />