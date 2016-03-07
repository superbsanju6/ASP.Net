var reportSelectionObject = new Object();
reportSelectionObject.category = "";
reportSelectionObject.level = "";
reportSelectionObject.levelID = "";
reportSelectionObject.levelIDEncrypted = "";
reportSelectionObject.testID = "";
reportSelectionObject.term = "All";
reportSelectionObject.year = "";
reportSelectionObject.type = "All";
reportSelectionObject.classID = "";
reportSelectionObject.parent = "";
reportSelectionObject.parentID = "";
reportSelectionObject.assessmentTitle = "";
reportSelectionObject.multiSelectTestIDs = "";
reportSelectionObject.reportType = "";
reportSelectionObject.steID = '';
reportSelectionObject.groups = "";
reportSelectionObject.testEventID = null;
reportSelectionObject.isContentLocked = '';
reportSelectionObject.pathURLPrefix = '';



function openReport(name, type, criteria, id) {
    var selectedReport = '';
    var url;
    var legacyURL;
    var reroster = $('#portalType').attr('value') == 'Admin' || $('#yearList').attr('value') == $('#districtYear').attr('value') ? '@RR=none' : '';

    switch (name.toLowerCase()) {
        case 'item analysis':
        case 'distractor analysis':
        case 'standard analysis':
        case 'at risk standards by student':
        case 'at risk students by standard':
        case 'grade conversion':
        case 'student responses':
        case 'suggested resources':
            selectedReport = name;
            break;
    }

    var height = 0;
    if (reportSelectionObject.level == "Student") {
        height = 380;
    }
    else if (reportSelectionObject.level == "Class") {
        height = 660;
    }

    if (selectedReport.toLowerCase() == 'suggested resources') {
        var parameters = "level=" + reportSelectionObject.level + "&levelID=" + reportSelectionObject.levelIDEncrypted + "&testID=" +
        reportSelectionObject.testID + "&year=" + reportSelectionObject.year + "&term=" + reportSelectionObject.term + "&type=" + reportSelectionObject.type + "&cid=" +
        reportSelectionObject.classID + "&parent=" + reportSelectionObject.parent + "&parentID=" + reportSelectionObject.parentID + '&selectedReport=' + selectedReport +
        '&multiTestIDs=' + multiTestIDs + "&groups=" + reportSelectionObject.groups + "&testEventID=" + reportSelectionObject.testEventID + "&category=" + reportSelectionObject.category
        + "&assessmentTitleDate=" + reportSelectionObject.assessmentTitle.substr(reportSelectionObject.assessmentTitle.lastIndexOf("as of") + 5, 20);
        {
            url = reportSelectionObject.pathURLPrefix + 'Dialogues/Assessment/SuggestedResources.aspx?' + parameters;
            customDialog({ url: url, maximize: true, maxwidth: 380, maxheight: height, destroyOnClose: true });
        }
    }

    else if (selectedReport.toLowerCase() == 'student responses') {
        legacyURL = 'display.asp?fo=search results&rm=searchpage&key=7020&??STEID=-1&??STELIST=' + reportSelectionObject.steID;
        legacyURL += '&??Answer Sequence=Student Viewed Order&??HighlightCorrect=Yes&??btp=0&??ClassTestEventID=' + reportSelectionObject.testEventID + '&??QPAGE=0&??FormID=&??Proof=Yes';
        url = reportSelectionObject.pathURLPrefix + 'SessionBridge.aspx?ReturnURL=' + escape(legacyURL) + '&selectedReport=' + name;
        window.open(url);
    }
    else if (selectedReport.length > 0 || type == 'group') {
        var multiTestIDs = '';
        if (reportSelectionObject.reportType == 'tportalreportmtest') {
            multiTestIDs = reportSelectionObject.multiSelectTestIDs;
        }

        window.open(reportSelectionObject.pathURLPrefix + "Record/Reports.aspx?xID=UE1ncWxHRW5xaHg2eWRLa2x5UHhPdz09&level=" + reportSelectionObject.level + "&levelID=" + reportSelectionObject.levelIDEncrypted + "&testID=" +
                    reportSelectionObject.testID + "&year=" + reportSelectionObject.year + "&term=" + reportSelectionObject.term + "&type=" + reportSelectionObject.type + "&cid=" +
                    reportSelectionObject.classID + "&parent=" + reportSelectionObject.parent + "&parentID=" + reportSelectionObject.parentID + '&selectedReport=' + selectedReport +
                    '&multiTestIDs=' + multiTestIDs + "&groups=" + reportSelectionObject.groups);
    }
    else {
        var lvl = 'none';
        var parentCrit = '';
        var reportPage = 0;
        var reportGroup = '';

        switch (reportSelectionObject.level.toLowerCase()) {
            case 'class':
                lvl = 'CID';
                break;
            case 'student':
                lvl = 'StudentRecID';
                break;
            case 'school':
                lvl = 'SCH';
                break;
            case 'teacher':
                lvl = 'TEAID';
                break;
        }

        if (reportSelectionObject.parent.toLowerCase() == 'school') {
            parentCrit = 'SCH=' + reportSelectionObject.parentID + '@@@@';
        }

        switch (type.toLowerCase()) {
            case 'adreport':
                criteria = criteria.split('@@');
                for (var i = 0; i < criteria.length; i++) {
                    if (criteria[i].toLowerCase().indexOf('rept') > -1) {
                        var reportPageArr = criteria[i].split('=');
                        if (reportPageArr.length > 1) {
                            reportPage = reportPageArr[1];
                        }
                    }
                    else if (criteria[i].toLowerCase().indexOf('group') > -1) {
                        var reportGroupArr = criteria[i].split('=');
                        if (reportGroupArr.length > 1) {
                            reportGroup = reportGroupArr[1];
                        }
                    }
                }

                if (reportPage == 6752 || reportPage == 6753 || reportPage == 6724 || reportPage == 6723)
                    legacyURL = 'display.asp?module=display&key=' + reportPage + '&fo=search%20results&rm=searchpage&xID=target=report&&formatoption=search%20results&retrievemode=searchpage';
                else if (reportPage == 6757 || reportPage == 6761 || reportPage == 6775) {
                    legacyURL = 'display.asp?module=display&key=' + reportPage + '&fo=menu&rm=page&xID=target=report&&formatoption=menu&retrievemode=page&??TID=' + reportSelectionObject.testID;
                    legacyURL += '&??LEVEL=' + reportSelectionObject.level + '&??LID=' + reportSelectionObject.levelID;
                } else
                    legacyURL = 'display.asp?key=' + reportPage + '&fo=menu&rm=page&xID=target=report&&formatoption=menu&retrievemode=page';

                if (reportSelectionObject.reportType == 'tportalreport1test') {
                    legacyURL += '&??var=&??CO=@@Product=none@@' + reroster + '@@TID=' + reportSelectionObject.testID + '@@' + '@@GRP=' + reportSelectionObject.groups + '@@' + lvl + '=' + reportSelectionObject.levelID + '@@@@TYRS=' + reportSelectionObject.year;
                    legacyURL += '@@TTERMS=All@@TTYPES=All@@@@' + parentCrit + '1test=yes@@&??YR=' + reportSelectionObject.year + '&??YEAR=' + reportSelectionObject.year;
                    legacyURL += '&??SLID=0&cb=y&ci=h&di=y&do=n&bc=1&ssn=yes&??outformat=&??stdToggle=SS&??ClassID=&??Group=' + reportGroup;
                }
                else if (reportSelectionObject.reportType == 'tportalreportmtest') {
                    legacyURL += '&??var=&??CO=@@Product=none@@' + reroster + '@@ZZ=' + reportSelectionObject.multiSelectTestIDs + '@@' + '@@GRP=' + reportSelectionObject.groups + '@@' + lvl + '=' + reportSelectionObject.levelID;
                    legacyURL += '@@@@TYRS=' + reportSelectionObject.year + '@@TCAT=' + reportSelectionObject.category + '@@TTERMS=All@@TTYPE=ALL@@TTYPES=All@@@@';
                    legacyURL += 'TLISTMASTER=' + reportSelectionObject.multiSelectTestIDs + '@@' + parentCrit + 'PT=1@@&??YR=' + reportSelectionObject.year;
                    legacyURL += '&??YR=' + reportSelectionObject.year + '&??YEAR=' + reportSelectionObject.year;
                    legacyURL += '&??SLID=0&cb=y&ci=h&di=y&do=n&bc=1&ssn=yes&??outformat=&??stdToggle=SS&??ClassID=&??Group=' + reportGroup;
                }

                break;
            case 'outline':
            case 'outlinereport':
                if (reportSelectionObject.reportType == 'tportalreport1test') {
                    legacyURL = 'fastsql_v2_direct.asp?module=fastsql_v2_direct&ID=6738|OutlineReport&target=report&formatoption=menu&retrievemode=page&??var=&';
                    legacyURL += '??CRIT=@@Product=none@@' + reroster + '@@TID=' + reportSelectionObject.testID + '@@' + '@@GRP=' + reportSelectionObject.groups + '@@' + lvl + '=' + reportSelectionObject.levelID;
                    legacyURL += '@@@@TYRS=' + reportSelectionObject.year + '@@TTERMS=All@@TTYPES=All@@@@' + parentCrit + '1test=yes@@@@PT=1@@&??YR=' + reportSelectionObject.year;
                    legacyURL += '&??YEAR=' + reportSelectionObject.year + '&??SLID=0&??RID=' + id + '&cb=y&ci=h&di=y&do=n&bc=1&??outformat=&??stdToggle=SS&??ClassID=';
                }
                else if (reportSelectionObject.reportType == 'tportalreportmtest') {
                    legacyURL = 'fastsql_v2_direct.asp?module=fastsql_v2_direct&ID=6738|OutlineReport&target=report&formatoption=menu&retrievemode=page&??var=&';
                    legacyURL += '??CRIT=@@Product=none@@' + reroster + '@@ZZ=' + reportSelectionObject.multiSelectTestIDs + '@@' + '@@GRP=' + reportSelectionObject.groups + '@@' + lvl + '=' + reportSelectionObject.levelID;
                    legacyURL += '@@@@TYRS=' + reportSelectionObject.year + '@@TCAT=' + reportSelectionObject.category + '@@TTERMS=All@@TTYPE=ALL@@TTYPES=All@@@@';
                    legacyURL += 'TLISTMASTER=' + reportSelectionObject.multiSelectTestIDs + '@@' + parentCrit + 'PT=1@@&??YR=' + reportSelectionObject.year;
                    legacyURL += '&??YR=' + reportSelectionObject.year + '&??YEAR=' + reportSelectionObject.year + '&??SLID=0&??RID=' + id;
                    legacyURL += '&cb=y&ci=h&di=y&do=n&bc=1&??outformat=&??stdToggle=SS&??ClassID=';
                }
                break;
        }

        url = reportSelectionObject.pathURLPrefix + 'SessionBridge.aspx?ReturnURL=' + escape(legacyURL + '&RptOrig=E3') + '&selectedReport=' + name;
        window.open(url);
    }
}

function ShowEditForm(category, rowLevel, rowLevelID, rowLevelIDEncrytped, columnTestID, selectedTerm, selectedYear, selectedType, selectedClass, parentLevel,
            parentLevelID, assessmentTitle, reportType, multiSelectTestIDs, steID, groups, testEventID, isContentLocked, pathURLPrefix) {
    reportSelectionObject.category = category;
    reportSelectionObject.level = rowLevel;
    reportSelectionObject.levelID = rowLevelID;
    reportSelectionObject.levelIDEncrypted = rowLevelIDEncrytped;
    reportSelectionObject.testID = columnTestID;
    reportSelectionObject.term = selectedTerm;
    reportSelectionObject.year = selectedYear;
    reportSelectionObject.type = selectedType;
    reportSelectionObject.classID = rowLevel.toLowerCase() == 'class' ? rowLevelID : selectedClass;
    reportSelectionObject.parent = parentLevel;
    reportSelectionObject.parentID = parentLevelID;
    reportSelectionObject.assessmentTitle = assessmentTitle;
    reportSelectionObject.pathURLPrefix = pathURLPrefix;
    reportSelectionObject.reportType = reportType;
    if ($('#lockedTestIDs').attr('value') != "" && $.trim($('#lockedTestIDs').attr('value')) != "," && $('#lockedTestIDs').attr('value') != null) {
        var tlockedTestIDs = $('#lockedTestIDs').attr('value').replace(/^,|,$/g, '');
        reportSelectionObject.multiSelectTestIDs = tlockedTestIDs;
    } else {
        reportSelectionObject.multiSelectTestIDs = multiSelectTestIDs;
    }
    reportSelectionObject.steID = steID;
    reportSelectionObject.testEventID = testEventID;
    reportSelectionObject.groups = groups;
    reportSelectionObject.isContentLocked = isContentLocked;

    var panel = $find('reportListXmlHttpPanel');
    panel.set_value('{"Level":"' + rowLevel + '", "ReportType":"' + reportType + '", "testCategory":"' + category + '"}');
    return false;
}

function loadReportList(sender, args) {
    /****************************************
    results properties
    -------------------------------
    - results[0] = report name
    - results[1] = report type
    - results[2] = criteria
    - results[3] = report id
    - results[4] = report title
    ****************************************/
    var divCol1 = '';
    var divCol2 = '';
    var results = args.get_content();
    var divHeader = '<div style="text-align:center;">' + reportSelectionObject.assessmentTitle + '</div><hr/>';
    for (var i = 0; i < results.length; i++) {
        if (results[i][0].toLowerCase() == "suggested resources") {
            /* Skip generation through miscXREF report list */
            continue;
        }
        divCol1 += '<div style="padding:3px;"><a href="javascript:void(0);" onclick="openReport(\'' + results[i][0] + '\', \'' + results[i][1] + '\', \'' + results[i][2] + '\', ' + results[i][3] + ');">' + results[i][0] + '</a></div>';
    }
    if (reportSelectionObject.steID && reportSelectionObject.steID.length > 0 && reportSelectionObject.testEventID) {
        switch (reportSelectionObject.level.toLowerCase()) {
            case 'student':
                if (reportSelectionObject.isContentLocked == 'False') {
                    if (document.getElementById("IsStudentResponseVisible").value == 'true')
                        divCol1 += '<div style="padding:3px;"><a href="javascript:void(0);" onclick="openReport(\'Student Responses\', null, null, null);">Student Responses</a></div>';
                }
                break;
            case 'class':
                if (reportSelectionObject.isContentLocked == 'False') {
                    if (document.getElementById("IsStudentResponseVisible").value == 'true')
                        divCol1 += '<div style="padding:3px;"><a href="javascript:void(0);" onclick="openReport(\'Student Responses\', null, null, null);">Student Responses</a></div>';
                }
                break;
        }
    }

    /* Report links in second column - Suggested Resources */
    if (document.getElementById("IsSuggestedResourcesVisible").value == 'true') {
        if (reportSelectionObject.testID != "" && reportSelectionObject.testID.length <= 7) {
            if (reportSelectionObject.level == "Class") {
                divCol2 += '<div style="padding:3px;"><a href="javascript:void(0);" onclick="openReport(\'Suggested Resources\', null, null, null);">Suggested Resources</a></div>';
                if (divCol2 != '') {
                    $("#contentDiv").css({
                        "width": 450
                    });
                    $("#list1").css({
                        "width": 250
                    });
                    $("#list2").css({
                        "width": 150
                    });
                }
            }
            else if (reportSelectionObject.level == "Student") {
                divCol2 += '<div style="padding:3px;"><a href="javascript:void(0);" onclick="openReport(\'Suggested Resources\', null, null, null);">Suggested Resources</a></div>';
                if (divCol2 != '') {
                    $("#contentDiv").css({
                        "width": 350
                    });
                    $("#list1").css({
                        "width": 150
                    });
                    $("#list2").css({
                        "width": 150
                    });
                }
            }
            else {
                $("#contentDiv").css({
                    "width": 350
                });
            }
        }
        else {
            $("#contentDiv").css({
                "width": 350
            });
        }
    }
    /* Suggested Resources */

    $('#Div1').html(divHeader);
    $('#Div2').html(divCol1);
    $('#Div3').html(divCol2);

    showReportWindow();
}