/* Declaring Publish Variables */
var message = "";
var flag = "No";
var oTable1;
var oTable2;
var WorksheetID;
var ClassID;
var CWHArray = new Array();
var CWHRIArray = new Array();
var found = 0;
var isViewByStandard = false;
var isSaved = false;
var percentage;
var standardData;
var studentData;
var isFiltered;
var filteredID = '';
var relativePathPrefix = '';
var prevCompValue = 'All Students';
var isHistoryUpdated = false;

function PrintWorksheet() {
    var windowUrl = relativePathPrefix + 'CompetencyWorksheetPrint.aspx';
    var num;
    var uniqueName = new Date();
    var windowName = 'Print' + uniqueName.getTime();
    var printWindow = window.open(windowUrl, windowName, 'left=50000,top=50000,width=0,height=0');
    printWindow.focus();
    setTimeout(function () { printWindow.close(); }, 10000);

    return false;
}

function getViewByStandardData(ID, ClassID) {
    $("#ddlView").attr("disabled", "disabled");
    $("#ddlCompetency").attr("disabled", "disabled");
    $.ajax({
        type: "POST",
        url: relativePathPrefix + "CompetencyWorksheetPreview.aspx/GetViewByStandardGrid",
        data: "{'WorksheetId':'" + ID + "','ClassID':'" + ClassID + "'}",

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ddlView").removeAttr("disabled");
            $("#ddlCompetency").removeAttr("disabled");
            //alert(XMLHttpRequest.responseText);
        },
        success: function (result) {
            $("#ddlView").removeAttr("disabled");
            $("#ddlCompetency").removeAttr("disabled");
            var data = [];

            if (result && result.d) {
                data = JSON.parse(result.d);
            }

            standardData = data;

            if (data.length > 4) {
                if (data[4].Table4.length == 0) {
                    alert("Worksheet either deleted or moved.");
                    window.close();

                    return;
                }
                $("#wDesc").text(data[4].Table4[0]["WorksheetDescription"]);
                document.title = data[4].Table4[0]["WorksheetName"];
            }
       
            if (isFiltered == true) {
                filterByStandard(filteredID, standardData);
            }
            else {
                if (standardData[0].Table.length > 0) {
                    filteredID = standardData[0].Table[0].StandardID;
                    filterByStandard(filteredID, data);
                    populateStandards(data[0].Table);
                }
                else {
                    alert("Please associate Standards to Current Worksheet!");
                    $("#ddlCompetency").empty();
                }
            }

            if (sessionStorage.reloadAfterPageLoad == "true") {
                $("#dialog-message").dialog({
                    modal: true,
                    buttons: {
                        Ok: function () {
                            $(this).dialog("close");
                        }
                    }
                });
                sessionStorage.reloadAfterPageLoad = false;
            }

            $('#divViewByStandard').show();
            $('#divViewByStudent').hide();
        }
    });
}

function getViewByStudentData(ID, ClassID) {
    $("#ddlView").attr("disabled", "disabled");
    $("#ddlCompetency").attr("disabled", "disabled");
    $.ajax({
        type: "POST",
        url: relativePathPrefix + "CompetencyWorksheetPreview.aspx/GetViewByStudentGrid",
        data: "{'WorksheetId':'" + ID + "','ClassID':'" + ClassID + "'}",

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ddlView").removeAttr("disabled");
            $("#ddlCompetency").removeAttr("disabled");
            //alert(XMLHttpRequest.responseText);
        },
        success: function (result) {
            $("#ddlView").removeAttr("disabled");
            $("#ddlCompetency").removeAttr("disabled");
            var data = [];

            if (result && result.d) {
                data = JSON.parse(result.d);
            }

            if (data.length > 3) {
                $("#wDesc").text(data[3].Table3[0]["WorksheetDescription"]);
            }

            studentData = data;

            if (data[0].Table.length > 0) {
                if (isFiltered == true) {
                    filterByStudent(filteredID, studentData);
                }
                else {
                    filteredID = studentData[0].Table[0].ID;
                    filterByStudent(filteredID, studentData);
                    populateStudents(data[0].Table);
                }
            }
            else {
                if (isFiltered == true) {
                    filterByStudent(filteredID, studentData);
                }
                else {
                    filteredID = studentData[0].Table[0].ID;
                    filterByStudent(filteredID, studentData);
                    populateStudents(data[1].Table1);
                }
            }

            if (sessionStorage.reloadAfterPageLoad == "true") {
                alert("This is copy of previously viewed worksheet");
                sessionStorage.reloadAfterPageLoad = false;
            }

            $('#divViewByStandard').hide();
            $('#divViewByStudent').show();
        }
    });
}

function saveHistory() {
    if (CWHArray.length > 0 && CWHRIArray.length > 0) {

        $.ajax({
            type: "POST",
            url: relativePathPrefix + "CompetencyWorksheetPreview.aspx/SaveWorksheetHistory",
            data: "{'History':'" + JSON.stringify(CWHArray) + "','HistoryRubricItems':'" + JSON.stringify(CWHRIArray) + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            success: function (result) {
                isSaved = true;
                CWHArray = new Array();
                CWHRIArray = new Array();

                if (isViewByStandard == true) {
                    getViewByStandardData(WorksheetID, ClassID);
                }
                else {
                    getViewByStudentData(WorksheetID, ClassID);
                }
            }
        });
    }
    else {
        alert("Please select a Competency Worksheet Rubric Item!");
    }
}

function updateAvailableStandardsDataTable(jsondata, data) {
    try {
        var column_names = new Array();
        column_names.push({ "sTitle": "StandardID", "mData": "ID", "sClass": "hiddenColumn" });
        column_names.push({ "sTitle": "Competency Name", "mData": "StandardName", "sWidth": "15%", "bSortable": false });
        column_names.push({ "sTitle": "Competency Text", "mData": "StandardDesc", "sWidth": "15%", "bSortable": false });

        var rubricdata = data[3].Table3;
        var percent = 50 / rubricdata.length;
        var rounded = Math.round(percent * 10) / 10;
        percentage = rounded + "%";

        for (j = 0; j < rubricdata.length; ++j) {
            column_names.push({ "sTitle": rubricdata[j]["Name"], "mData": null, "sWidth": percentage, "sDefaultContent": "", "bSortable": false });
        }

        column_names.push({ "sTitle": "Comments", "mDataProp": null, "sWidth": "10%", "sDefaultContent": "", "bSortable": false, "sClass": "hideme" });
        column_names.push({ "sTitle": "History", "mDataProp": null, "sWidth": "10%", "sDefaultContent": "", "bSortable": false, "sClass": "hideme" });

        oTable2 = undefined;
        var tableID = "tblViewByStandard";
        if (typeof oTable2 == 'undefined') {
            oTable2 = $('#tblViewByStandard').dataTable({
                "iDisplayLength": 10,
                "bRetrieve": false,
                "bProcessing": false,
                "bDestroy": true,
                "bPaginate": false,
                "bFilter": false,
                "bInfo": false,
                "bLengthChange": false,
                "bScrollInfinite": true,
                "bScrollCollapse": true,
                "sScrollY": "520px",
                "aaSorting": [[0, "asc"]],
                "bSorting": false,
                "aaData": jsondata,
                "bAutoWidth": false,
                "sPaginationType": "full_numbers",
                "aoColumns": column_names,
                "fnDrawCallback": function (oSettings) {
                    createInnerTable(jsondata, data);
                    $('#tblViewByStandard').dataTable()._fnScrollDraw();
                    $('#tblViewByStandard').closest(".dataTables_scrollBody").height(520);
                },
                "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                }
            });
        }
        else {
            oTable2.fnClearTable(0);
            oTable2.fnAddData(jsondata);
            oTable2.fnDraw();
        }
    } catch (e) {
        alert("ex: " + e.message);
    }
}

function updateAvailableStudentsDataTable(jsondata, data) {
    try {
        var column_names = new Array();

        column_names.push({ "sTitle": "StudentID", "mData": "StudentID", "sClass": "hiddenColumn" });
        column_names.push({ "sTitle": "Competency Name", "mData": "EntityName", "sWidth": '15%', "bSortable": false });
        column_names.push({ "sTitle": "Competency Text", "mData": null, "sWidth": '21%', "sDefaultContent": "", "bSortable": false });

        var rubricdata = data[2].Table2;
        var percent = 50 / rubricdata.length;
        var rounded = Math.round(percent * 10) / 10;
        percentage = rounded + "%";

        for (j = 0; j < rubricdata.length; ++j) {
            column_names.push({ "sTitle": rubricdata[j]["Name"], "mData": null, "sWidth": percentage, "sDefaultContent": "", "bSortable": false });
        }

        column_names.push({ "sTitle": "Comments", "mDataProp": null, "sWidth": "7%", "sDefaultContent": "", "bSortable": false, "sClass": "hideme" });
        column_names.push({ "sTitle": "History", "mDataProp": null, "sWidth": "7%", "sDefaultContent": "", "bSortable": false, "sClass": "hideme" });

        oTable1 = undefined;
        var tableID = "tblViewByStudent";
        if (typeof oTable1 == 'undefined') {
            oTable1 = $('#tblViewByStudent').dataTable({
                "iDisplayLength": 10,
                "bRetrieve": false,
                "bProcessing": true,
                "bDestroy": true,
                "bPaginate": false,
                "bFilter": false,
                "bInfo": false,
                "bLengthChange": false,
                "bScrollInfinite": true,
                "bScrollCollapse": true,
                "sScrollY": "520px",
                "aaSorting": [],
                "bSorting": false,
                "aaData": jsondata,
                "bAutoWidth": false,
                "sPaginationType": "full_numbers",
                "aoColumns": column_names,
                "fnDrawCallback": function (oSettings) {
                    createInnerTableByStudent(data);
                    $('#tblViewByStudent').dataTable()._fnScrollDraw();
                    $('#tblViewByStudent').closest(".dataTables_scrollBody").height(520);

                },
                "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                }
            });
        }
        else {
            oTable1.fnClearTable(0);
            oTable1.fnAddData(jsondata);
            oTable1.fnDraw();
        }
    } catch (e) {
        alert("ex: " + e.message);
    }
}

function createInnerTable(jsondata, data) {

    if (data[0].Table.length == 0) {
        alert("Please associate Standards to Current Worksheet!");

        return;
    }

    if (data[1].Table1.length == 0) {
        alert("Please assign Class or Group to Current Worksheet!");

        return;
    }
    if (data[2].Table2.length == 0) {
        alert("Please associate Student with Class or Group to Current Worksheet!");

        return;
    }

    if (data[3].Table3.length == 0) {
        alert("Please assign Rubric Item to Current Worksheet!");

        return;
    }

    var nTrs = $('#tblViewByStandard tbody tr');
    var sLastGroup = "";

    var iColspan = nTrs[0].getElementsByTagName('td').length;
    var sLastGroup = "";

    for (var i = 0 ; i < nTrs.length ; i++) {
        nTrs[i].cells[1].innerHTML = "<a style='text-decoration: underline; color: #fff;' target='_blank' href='Record/StandardsPage.aspx?xID=" + jsondata[i]["EncryptedStandardID"] + "'>" + nTrs[i].cells[1].innerHTML + "</a>";
        nTrs[i].cells[1].ClassName = "borderRight1px";
        nTrs[i].cells[2].innerHTML = nTrs[i].cells[2].innerHTML;
        var iDisplayIndex = i + 1;
        var sGroup = "";
        var nGroup = document.createElement('tr');
        nGroup.id = "grp_" + i;
        var nCell = document.createElement('td');
        nCell.colSpan = iColspan;
        tableid = i;

        nCell.innerHTML = fnFormatDetails(tableid, data[1].Table1, data[2].Table2, data[3].Table3, data[0].Table[i].WorksheetID, data[0].Table[i].StandardID, data);
        
        nGroup.appendChild(nCell);

        if ((i + 1) > nTrs.length - 1) {
            $('#tblViewByStandard').append($("<tr>" + nGroup.innerHTML + "</tr>"));
        }
        else {
            nTrs[i].parentNode.insertBefore(nGroup, nTrs[i + 1]);
        }
    }
}

function createInnerTableByStudent(data) {

    if (data[0].Table.length == 0 && data[1].Table1.length == 0) {
        alert("Please assign Class or Group to Current Worksheet!");

        return;
    }
    else if ((data[0].Table.length == 1 && data[0].Table.EntityID == null) && data[1].Table1.length == 1 && data[1].Table1.EntityID == null) {
        alert("Please associate Student with Class or Group to Current Worksheet!");

        return;
    }
    if (data[1].Table1.length == 0) {
        alert("Please associate Standards to proceed!");

        return;
    }
    if (data[2].Table2.length == 0) {
        alert("Please assign Rubric Item to Current Worksheet!");

        return;
    }

    var nTrs = $('#tblViewByStudent tbody tr');
    var sLastGroup = "";

    var currentStudent = 'All Students';
    if ($("#ddlCompetency option:selected").text() != '' && filteredID != '') {
        currentStudent = $("#ddlCompetency option:selected").text();
    }

    var iColspan = nTrs[0].getElementsByTagName('td').length;
    nTrs[0].getElementsByTagName('td')[1].innerHTML = currentStudent;
    var sLastGroup = "";
    for (var i = 0 ; i < nTrs.length ; i++) {
        var iDisplayIndex = i + 1;
        var sGroup = "";
        var nGroup = document.createElement('tr');
        nGroup.id = "grp_" + i;
        var nCell = document.createElement('td');
        nCell.colSpan = iColspan;
        tableid = i;

        nCell.innerHTML = fnFormatDetailsByStudent(tableid, data[1].Table1, data[2].Table2, data[0].Table[i].WorksheetID, data[0].Table[i].EntityID, data[0].Table[i].CompetencyRubricID, data[0].Table[i].TeacherID, data);

        nGroup.appendChild(nCell);

        if ((i + 1) > nTrs.length - 1) {
            $('#tblViewByStudent').append($("<tr>" + nGroup.innerHTML + "</tr>"));
        }
        else {
            nTrs[i].parentNode.insertBefore(nGroup, nTrs[i + 1]);
        }
    }
}

function formatJSONDate(jsonDate) {
    var str, year, month, day, hour, minute, d, finalDate;
    str = jsonDate.replace(/\D/g, "");
    d = new Date(parseInt(str));
    year = pad(d.getFullYear());
    month = pad(d.getMonth() + 1);
    day = pad(d.getDate());
    hour = pad(d.getHours());
    minutes = pad(d.getMinutes());
    finalDate = month + "/" + day + "/" + year;

    return finalDate;
}

function formatNewJSONDate(newDateFormate) {
    var currentTime = new Date(parseInt(newDateFormate));
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    var finalDate = pad(month) + "/" + pad(day) + "/" + year;
    finalDate = '(' + finalDate + ')';

    return finalDate;
}

function pad(num) {
    num = "0" + num;

    return num.slice(-2);
}

/* Formating function for row details View By Standard */
function fnFormatDetails(tableid, classdata, studentdata, rubricdata, WorksheetID, StandardID, data) {
    var numOfClassRecs = classdata.length;
    var numOfStudentRecs = studentdata.length;
    var numOfrubricRecs = rubricdata.length;
    var WorksheetHistory = new Array();
    var WorksheetHistoryOther = new Array();

    if (filteredID != '') {
        StandardID = filteredID;
    }

    var sOut = '<table  id="tblchild+"' + tableid + 'cellspacing="0" cellpadding="0" border="1" style="background-color: #fff; width: 100%;">';

    for (i = 0; i < numOfClassRecs; ++i) {
        sOut += '<tr id=classgrp_' + tableid + '>';
        sOut += '<td style="width: 30%;" class="leftAlign" >' + classdata[i]["PreviewName"] + '</td>';
        for (j = 0; j < numOfrubricRecs; ++j) {
            sOut += '<td width="' + percentage + '" class="height40px"> <input type="checkbox" id="chkSelectAll_' + tableid + '_' + classdata[i]["PreviewID"] + '_' + rubricdata[j]["CompetencyRubricItemID"] + '_' + StandardID + '_' + j + '" onclick="toggleChecks(this);" ></input>';
        }
        sOut += '<td width="10%" class="hideme"></td>';
        sOut += '<td style="width:10%; " class="hideme"></td>';
        sOut += '</tr>';
    }

    for (i = 0; i < numOfStudentRecs; ++i) {
        var StudentID = studentdata[i]["StudentID"];
        var CompetencyRubricID = studentdata[i]["CompetencyRubricID"];
        var TeacherID = studentdata[i]["TeacherID"];
        sOut += '<tr id=studentgrp_' + tableid + '>';

        if (data.length > 5) {
            WorksheetHistory = $.grep(data[5].Table5, function (e) { return (e.WorksheetID == WorksheetID && e.StudentID == StudentID && e.StandardID == StandardID && e.CompetencyRubricID == CompetencyRubricID && e.Teacher == TeacherID) });
        }

        sOut += '<td style="width: 30%;" class="leftAlign" >' + studentdata[i]["StudentName"] + ' <input type="hidden" id="hdn_"' + StudentID + ' value=' + StudentID + '_' + CompetencyRubricID + '_' + TeacherID + '/> </td>';

        for (j = 0; j < numOfrubricRecs; ++j) {
            var colorClass = '';
            var scoreDate = '';
            /* To check whether student and standard exist in another worksheet also. */
            if (data.length > 7) {
                WorksheetHistoryOther = $.grep(data[7].Table7, function (e) { return (e.StudentID == StudentID && e.StandardID == StandardID && e.CompetencyRubricID == CompetencyRubricID && e.CompetencyRubricItemID == rubricdata[j]["CompetencyRubricItemID"]) });
                if (WorksheetHistoryOther.length > 0) {
                    scoreDate = formatNewJSONDate(WorksheetHistoryOther[0].ScoreDate.replace("/", "").replace("Date(", "").replace(")", "").replace("/", ""));
                    colorClass = "backColorGrey";
                }
            }

            if (WorksheetHistory.length > 0) {
                var WorksheetHistoryRI = $.grep(data[6].Table6, function (e) { return (e.WorkSheetHistoryID == WorksheetHistory[0].ID && e.CompetencyRubricItemID == rubricdata[j]["CompetencyRubricItemID"]); });
                if (WorksheetHistoryRI.length > 0) {
                    scoreDate = formatNewJSONDate(WorksheetHistoryRI[0].ScoreDate.replace("/", "").replace("Date(", "").replace(")", "").replace("/", ""));
                    colorClass = "backColorLightGreen";
                }
            }

            sOut += '<td ' + 'Class="' + colorClass + '"' + ' width="' + percentage + '" ><input type="checkbox" id="chkchild_' + tableid + '_' + classdata[0]["PreviewID"] + '_' + rubricdata[j]["CompetencyRubricItemID"] + '_' + StandardID + '_' + j + '" onclick="childtoggleChecks(this);" ></input> <br/> ' + scoreDate + ' </td>'; //<br />(10/12/2013) ';
        }

        var commentMethodCall = "showCommentDialog(" + WorksheetID + "," + StandardID + ",'" + StudentID + "')";
        var historyMethodCall = "showHistoryDialog(" + WorksheetID + "," + StandardID + ",'" + StudentID + "')";
        sOut += '<td width="10%" class="hideme"  ><a  onclick=' + commentMethodCall + '><img src="images/Comment.png" style="cursor: pointer;" alt="" height="23" width="25" /></a<</td>';
        sOut += '<td style="background-color: #BFBFBF; width:10%;"  class="hideme" ><a style="text-decoration: underline;" href="#" onclick=' + historyMethodCall + ' >History</a></td>';
        sOut += '</tr>';
    }

    sOut += '</table>';

    return sOut;
}

/* Formating function for row details View By Student */
function fnFormatDetailsByStudent(tableid, standarddata, rubricdata, WorksheetID, StudentID, CompetencyRubricID, TeacherID, data) {
    var numOfStandardRecs = standarddata.length;
    var numOfRubricRecs = rubricdata.length;
    var sOut = '<table  id="tblchild+"' + tableid + 'cellspacing="0" cellpadding="0" border="1" style="background-color: #fff; width: 100%">';

    if (filteredID != '') {
        StudentID = filteredID;
    }
    
    var WorksheetHistory = new Array();
    var WorksheetHistoryOther = new Array();

    for (i = 0; i < numOfStandardRecs; ++i) {
        var StandardID = standarddata[i]["StandardID"];
        if (data.length > 4) {
            WorksheetHistory = $.grep(data[4].Table4, function(e) {
                if (e.WorksheetID == WorksheetID && e.StudentID == StudentID && e.StandardID == StandardID && e.CompetencyRubricID == CompetencyRubricID && e.Teacher == TeacherID) {
                    return e;
                }
            });
        }

        sOut += '<tr id=studentgrp_' + tableid + '_' + standarddata[i]["StandardID"] + '>';
        sOut += "<td class='leftAlign' style='word-wrap:break-word; width: 15%;'><a style='text-decoration: underline;' target='_blank' href='Record/StandardsPage.aspx?xID=" + standarddata[i]["EncryptedStandardID"] + "' >" + standarddata[i]["StandardName"] + "</a></td>";
        sOut += '<td class="leftAlign" style="word-wrap:break-word; width: 15%;">' + standarddata[i]["StandardDesc"] + ' <input type="hidden" id="hdn_"' + StudentID + '_' + StandardID + ' value=' + StudentID + '_' + CompetencyRubricID + '_' + TeacherID + '/> </td>';

        for (j = 0; j < numOfRubricRecs; ++j) {

            var colorClass = '';
            var scoreDate = '';

            /* To check whether student and standard exist in another worksheet also. */
            if (data.length > 6) {
                WorksheetHistoryOther = $.grep(data[6].Table6, function(e) {
                    return (e.StudentID == StudentID && e.StandardID == StandardID && e.CompetencyRubricID == CompetencyRubricID && e.CompetencyRubricItemID == rubricdata[j]["CompetencyRubricItemID"]);
                });

                if (WorksheetHistoryOther.length > 0) {
                    scoreDate = formatNewJSONDate(WorksheetHistoryOther[0].ScoreDate.replace("/", "").replace("Date(", "").replace(")", "").replace("/", ""));
                    colorClass = "backColorGrey";
                }
            }

            if (WorksheetHistory.length > 0) {
                var WorksheetHistoryRI = $.grep(data[5].Table5, function(e) {
                    return (e.WorkSheetHistoryID == WorksheetHistory[0].ID && e.CompetencyRubricItemID == rubricdata[j]["CompetencyRubricItemID"]);
                });

                if (WorksheetHistoryRI.length > 0) {
                    scoreDate = formatNewJSONDate(WorksheetHistoryRI[0].ScoreDate.replace("/", "").replace("Date(", "").replace(")", "").replace("/", ""));
                    colorClass = "backColorLightGreen";
                }
            }

            sOut += '<td  width="' + percentage + '"' + 'Class="' + colorClass + '"><input onclick="childtoggleViewByStudentsChecks(this);" type="checkbox" id="chkchild_' + tableid + '_' + standarddata[i]["StandardID"] + '_' + rubricdata[j]["CompetencyRubricItemID"] + '_' + j + '" " ></input><br />' + scoreDate + '</td>';
        }

        if (filteredID != '') {
            var methodcallByStudent = "showCommentDialog(" + WorksheetID + "," + StandardID + ",'" + StudentID + "')";
            var historyMethodCallByStudent = "showHistoryDialog(" + WorksheetID + "," + StandardID + ",'" + StudentID + "')";
            sOut += '<td width="10%" class="hideme"><a onclick=' + methodcallByStudent + '><img src="images/Comment.png" style="cursor: pointer;" alt="" height="23" width="25" /></a<</td>';
            sOut += '<td style="background-color: #BFBFBF; width:10%; " class="hideme"><a style="text-decoration: underline;" href="#" onclick=' + historyMethodCallByStudent + ' >History</a></td>';
            sOut += '</tr>';
        }
        else {
            sOut += '<td width="10%" class="hideme">&nbsp;</td>';
            sOut += '<td style="width:10%;" class="hideme">&nbsp;</td>';
            sOut += '</tr>';
        }
    }

    sOut += '</table>';

    return sOut;
}

function competencyRubricExists(StandardID, StudentID, RubricID, WorksheetID, TeacherID) {
    var IndexValue = -1;
    $.each(CWHArray, function (index, value) {
        if (value.studentid == StudentID && value.standardid == StandardID) {
            IndexValue = index;

            return false;
        }
    });

    return IndexValue;
}

function competencyRubricItemExists(StandardID, StudentID, RubricItemID, WorksheetID, TeacherID) {
    var IndexValue = -1;
    $.each(CWHRIArray, function (index, value) {
        if (value.studentid == StudentID && value.standardid == StandardID && value.competencyrubricitemid == RubricItemID) {
            IndexValue = index;

            return false;
        }
    });

    return IndexValue;
}

function competencyRubricItemStudentStandardExists(StandardID, StudentID) {
    var IndexValue = -1;
    $.each(CWHRIArray, function (index, value) {
        if (value.studentid == StudentID && value.standardid == StandardID) {
            IndexValue = index;

            return false;
        }
    });

    return IndexValue;
}

function childtoggleChecks(c) {
    var lStudentID = c.parentNode.parentNode.cells[0].firstElementChild.value.replace('/', '').split('_')[0];
    var RubricID = c.parentNode.parentNode.cells[0].firstElementChild.value.replace('/', '').split('_')[1];
    var TeacherID = c.parentNode.parentNode.cells[0].firstElementChild.value.replace('/', '').split('_')[2];
    var RubricItemID = c.id.split('_')[3];
    var StandardID = c.id.split('_')[4];

    if (c.checked) {
        var result = $.grep(CWHArray, function (e) { return (e.worksheetid == WorksheetID && e.studentid == lStudentID && e.standardid == StandardID && e.rubricid == RubricID && e.teacherid == TeacherID); });

        if (result.length == 0) {
            CWHArray.push({ worksheetid: WorksheetID, studentid: lStudentID, standardid: StandardID, rubricid: RubricID, teacherid: TeacherID });
        }
                
        CWHRIArray.push({ studentid: lStudentID, standardid: StandardID, worksheethistoryid: 0, competencyrubricitemid: RubricItemID, worksheetid: WorksheetID, teacherid: TeacherID });
        if(isViewByStandard==false)
            UnCheckRubricItems(c, lStudentID);
        else
            UnCheckRubricItemsForStandard(c, StandardID)
        
    }
    else {
        /*  Finding Index no to delete from Competency Worksheet Rubtic Item array */
        var indexRI = competencyRubricItemExists(StandardID, lStudentID, RubricItemID, WorksheetID, TeacherID);

        if (indexRI > -1) {
            CWHRIArray.splice(indexRI, 1);
            /* Checking still any Competency Worksheet Rubtic Item exist. If No delete from Cometency Rubric History array. */
            var indexRiInner = competencyRubricItemStudentStandardExists(StandardID, lStudentID);

            if (indexRiInner == -1) {

                var indexHis = competencyRubricExists(StandardID, lStudentID, RubricID, WorksheetID, TeacherID);

                if (indexHis > -1) {
                    CWHArray.splice(indexHis, 1);

                }
            }
        }
    }
}

function toggleChecks(c) {
    var rIndex = c.id.split('_')[1];

    var nTrs = $('#tblViewByStandard tbody tr #studentgrp_' + rIndex);
    CWHArray = new Array();
    CWHRIArray = new Array();
    for (i = 0; i < nTrs.length; i++) {
        var Row = nTrs[i];
        var rowID = nTrs[i].id;
        if (c.checked) {
            UnCheckRubricItemsForParentStandard(c, StandardID);
        }
        for (j = 1; j < nTrs[i].cells.length - 2; j++) {
           
            var inputID = nTrs[i].cells[j].firstElementChild.id;

            if (inputID == c.id.replace("chkSelectAll", "chkchild")) {
                var chkChild = nTrs[i].cells[j].firstElementChild;
                var lStudentID = nTrs[i].cells[0].firstElementChild.value.replace('/', '').split('_')[0];
                var RubricID = nTrs[i].cells[0].firstElementChild.value.replace('/', '').split('_')[1];
                var TeacherID = nTrs[i].cells[0].firstElementChild.value.replace('/', '').split('_')[2];
                var RubricItemID = nTrs[i].cells[j].firstElementChild.id.split('_')[3];

                var StandardID = c.id.split('_')[4];

                if (c.checked) {
                    nTrs[i].cells[j].firstElementChild.checked = true;
                    var result = $.grep(CWHArray, function (e) { return (e.worksheetid == WorksheetID && e.studentid == lStudentID && e.standardid == StandardID && e.rubricid == RubricID && e.teacherid == TeacherID); });

                    if (result.length == 0) {
                        CWHArray.push({ worksheetid: WorksheetID, studentid: lStudentID, standardid: StandardID, rubricid: RubricID, teacherid: TeacherID });
                    }

                    UnCheckRubricItemsForStandard(chkChild, StandardID);
                    CWHRIArray.push({ studentid: lStudentID, standardid: StandardID, worksheethistoryid: 0, competencyrubricitemid: RubricItemID, worksheetid: WorksheetID, teacherid: TeacherID });
                }
                else {
                    nTrs[i].cells[j].firstElementChild.checked = false;

                    /*  Finding Index no to delete from Competency Worksheet Rubtic Item array */
                    var indexRI = competencyRubricItemExists(StandardID, lStudentID, RubricItemID, WorksheetID, TeacherID);

                    if (indexRI > -1) {
                        CWHRIArray.splice(indexRI, 1);
                        /* Checking still any Competency Worksheet Rubtic Item exist. If No delete from Cometency Rubric History array. */
                        var indexRiInner = competencyRubricItemStudentStandardExists(StandardID, lStudentID);
                        if (indexRiInner == -1) {
                            var indexHis = competencyRubricExists(StandardID, lStudentID, RubricID, WorksheetID, TeacherID);
                            if (indexHis > -1) {
                                CWHArray.splice(indexHis, 1);

                            }
                        }
                    }
                }
                break;
            }
        }
    }
}

function childtoggleViewByStudentsChecks(c) {
    var StudentID = c.parentNode.parentNode.cells[1].firstElementChild.value.replace('/', '').split('_')[0];
    var isAllStudents = false;

    if ((filteredID == 0 || filteredID == '') && isViewByStandard == false) {
        isAllStudents = true;
    }

    if (isAllStudents) {
         CWHArray = new Array();
            CWHRIArray = new Array();
        for (i = 1; i < studentData[0].Table.length; i++) {
           
            AddRemoveStudentsFromArray(c, studentData[0].Table[i].EntityID);
        }
    }
    else {
        AddRemoveStudentsFromArray(c, StudentID);
    }
}

function AddRemoveStudentsFromArray(c, StudentID) { 
    var RubricID = c.parentNode.parentNode.cells[1].firstElementChild.value.replace('/', '').split('_')[1];
    var TeacherID = c.parentNode.parentNode.cells[1].firstElementChild.value.replace('/', '').split('_')[2];
    var RubricItemID = c.id.split('_')[3];
    var StandardID = c.id.split('_')[2];

    if (c.checked) {

        var result = $.grep(CWHArray, function (e) { return (e.worksheetid == WorksheetID && e.studentid == StudentID && e.standardid == StandardID && e.rubricid == RubricID && e.teacherid == TeacherID); });

        if (result.length == 0) {
            CWHArray.push({ worksheetid: WorksheetID, studentid: StudentID, standardid: StandardID, rubricid: RubricID, teacherid: TeacherID });
        }
        UnCheckRubricItems(c, StudentID);
        CWHRIArray.push({ studentid: StudentID, standardid: StandardID, worksheethistoryid: 0, competencyrubricitemid: RubricItemID, worksheetid: WorksheetID, teacherid: TeacherID });
        
    }
    else {
        /*  Finding Index no to delete from Competency Worksheet Rubtic Item array */
        var indexRI = competencyRubricItemExists(StandardID, StudentID, RubricItemID, WorksheetID, TeacherID);

        if (indexRI > -1) {
            CWHRIArray.splice(indexRI, 1);
            /* Checking still any Competency Worksheet Rubtic Item exist. If No delete from Cometency Rubric History array. */
            var indexRiInner = competencyRubricItemStudentStandardExists(StandardID, StudentID);

            if (indexRiInner == -1) {
                var indexHis = competencyRubricExists(StandardID, StudentID, RubricID, WorksheetID, TeacherID);

                if (indexHis > -1) {
                    CWHArray.splice(indexHis, 1);
                }
            }
        }
    }
}

function UnCheckRubricItems(c,StudentID)
{
    var CurrentRubricIndex= c.id.split('_')[4];
    for (j = 0; j < 5; j++)
    {
        if (j != CurrentRubricIndex)
        {
            if (c.parentNode.parentNode.cells[j + 2].firstElementChild.checked)
            {
                c.parentNode.parentNode.cells[j + 2].firstElementChild.checked = false;                
                    RemoveUnselcted(c, StudentID, c.parentNode.parentNode.cells[j + 2].firstElementChild.id);                
            }            
         }
    }
}
function UnCheckRubricItemsForStandard(c, StandardID) {  
    var CurrentRubricIndex = c.id.split('_')[5];
    for (j = 0; j < 5; j++) {
        if (j != CurrentRubricIndex) {
            if (c.parentNode.parentNode.cells[j + 1].firstElementChild.checked) {
                c.parentNode.parentNode.cells[j + 1].firstElementChild.checked = false;
                RemoveUnselctedForStandard(c, StandardID, c.parentNode.parentNode.cells[j + 1].firstElementChild.id);
            }
        }
    }
}

function UnCheckRubricItemsForParentStandard(c, StandardID) {
    var CurrentRubricIndex = c.id.split('_')[5];
    for (j = 0; j < 5; j++) {
        if (j != CurrentRubricIndex) {
            if (c.parentNode.parentNode.cells[j + 1].firstElementChild.checked) {
                c.parentNode.parentNode.cells[j + 1].firstElementChild.checked = false;               
            }
        }
    }
}

function RemoveUnselcted(c, StudentID, pRubricItemID)
{

    var RubricID = c.parentNode.parentNode.cells[1].firstElementChild.value.replace('/', '').split('_')[1];
    var TeacherID = c.parentNode.parentNode.cells[1].firstElementChild.value.replace('/', '').split('_')[2];
    var RubricItemID = pRubricItemID.split('_')[3];
    var StandardID = c.id.split('_')[2];


    /*  Finding Index no to delete from Competency Worksheet Rubtic Item array */
    var indexRI = competencyRubricItemExists(StandardID, StudentID, RubricItemID, WorksheetID, TeacherID);

    if (indexRI > -1) {
        CWHRIArray.splice(indexRI, 1);
        /* Checking still any Competency Worksheet Rubtic Item exist. If No delete from Cometency Rubric History array. */
        var indexRiInner = competencyRubricItemStudentStandardExists(StandardID, StudentID);

        if (indexRiInner == -1) {
            var indexHis = competencyRubricExists(StandardID, StudentID, RubricID, WorksheetID, TeacherID);

            //if (indexHis > -1) {
            //    CWHArray.splice(indexHis, 1);
            //}
        }
    }
}

function RemoveUnselctedForStandard(c, StandardID, pRubricItemID) {

    var lStudentID = c.parentNode.parentNode.cells[0].firstElementChild.value.replace('/', '').split('_')[0];
    var RubricID = c.parentNode.parentNode.cells[0].firstElementChild.value.replace('/', '').split('_')[1];
    var TeacherID = c.parentNode.parentNode.cells[0].firstElementChild.value.replace('/', '').split('_')[2];
    var RubricItemID = pRubricItemID.split('_')[3];
    //var StandardID = StandardID;


    /*  Finding Index no to delete from Competency Worksheet Rubtic Item array */
    var indexRI = competencyRubricItemExists(StandardID, lStudentID, RubricItemID, WorksheetID, TeacherID);

    if (indexRI > -1) {
        CWHRIArray.splice(indexRI, 1);
        /* Checking still any Competency Worksheet Rubtic Item exist. If No delete from Cometency Rubric History array. */
        var indexRiInner = competencyRubricItemStudentStandardExists(StandardID, lStudentID);

        if (indexRiInner == -1) {

            var indexHis = competencyRubricExists(StandardID, lStudentID, RubricID, WorksheetID, TeacherID);

            if (indexHis > -1) {
                CWHArray.splice(indexHis, 1);

            }
        }
    }
}

function RemoveUnselcted(c, StudentID, pRubricItemID) {

    var RubricID = c.parentNode.parentNode.cells[1].firstElementChild.value.replace('/', '').split('_')[1];
    var TeacherID = c.parentNode.parentNode.cells[1].firstElementChild.value.replace('/', '').split('_')[2];
    var RubricItemID = pRubricItemID.split('_')[3];
    var StandardID = c.id.split('_')[2];


    /*  Finding Index no to delete from Competency Worksheet Rubtic Item array */
    var indexRI = competencyRubricItemExists(StandardID, StudentID, RubricItemID, WorksheetID, TeacherID);

    if (indexRI > -1) {
        CWHRIArray.splice(indexRI, 1);
        /* Checking still any Competency Worksheet Rubtic Item exist. If No delete from Cometency Rubric History array. */
        var indexRiInner = competencyRubricItemStudentStandardExists(StandardID, StudentID);

        if (indexRiInner == -1) {
            var indexHis = competencyRubricExists(StandardID, StudentID, RubricID, WorksheetID, TeacherID);

            //if (indexHis > -1) {
            //    CWHArray.splice(indexHis, 1);
            //}
        }
    }
}

function toggleViewByStudentsChecks(c) {
    var groupIndex = c.id.split('_')[1];
    var standardID = c.id.split('_')[2];
    var competencyRubricItemID = c.id.split('_')[3];

    for (studentGroupIndex = 1; studentGroupIndex < studentGroupIndex + groupIndex; studentGroupIndex++) {
        var nTrs = $('#tblViewByStudent tbody tr #studentgrp_' + studentGroupIndex + '_' + standardID);

        if (nTrs.length > 0) {
            for (i = 0; i < nTrs.length; i++) {
                for (j = 1; j < nTrs[i].cells.length - 2; j++) {
                    var inputID = nTrs[i].cells[j + 1].firstElementChild.id;
                    if (inputID == "chkchild_" + studentGroupIndex + "_" + standardID + "_" + competencyRubricItemID) {
                        var StudentID = nTrs[i].cells[1].firstElementChild.value.replace('/', '').split('_')[0];
                        var RubricID = nTrs[i].cells[1].firstElementChild.value.replace('/', '').split('_')[1];
                        var TeacherID = nTrs[i].cells[1].firstElementChild.value.replace('/', '').split('_')[2];
                        var RubricItemID = nTrs[i].cells[j + 1].firstElementChild.id.split('_')[3];

                        if (c.checked) {
                            nTrs[i].cells[j + 1].firstElementChild.checked = true;
                            var result = $.grep(CWHArray, function (e) { return (e.worksheetid == WorksheetID && e.studentid == StudentID && e.standardid == standardID && e.rubricid == RubricID && e.teacherid == TeacherID); });

                            if (result.length == 0) {
                                CWHArray.push({ worksheetid: WorksheetID, studentid: StudentID, standardid: standardID, rubricid: RubricID, teacherid: TeacherID });
                            }
                            CWHRIArray.push({ studentid: StudentID, standardid: standardID, worksheethistoryid: 0, competencyrubricitemid: RubricItemID, worksheetid: WorksheetID, teacherid: TeacherID });
                        }
                        else {
                            nTrs[i].cells[j + 1].firstElementChild.checked = false;
                            /*  Finding Index no to delete from Competency Worksheet Rubtic Item array */
                            var indexRI = competencyRubricItemExists(standardID, StudentID, RubricItemID, WorksheetID, TeacherID);

                            if (indexRI > -1) {
                                CWHRIArray.splice(indexRI, 1);
                                /* Checking still any Competency Worksheet Rubtic Item exist. If No delete from Cometency Rubric History array. */
                                var indexRiInner = competencyRubricItemStudentStandardExists(standardID, StudentID);

                                if (indexRiInner == -1) {
                                    var indexHis = competencyRubricExists(standardID, StudentID, RubricID, WorksheetID, TeacherID);
                                    if (indexHis > -1) {
                                        CWHArray.splice(indexHis, 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else {
            break;
        }
    }
}

function cloneObject(obj) {
    var newObj = (obj instanceof Array) ? [] : {};

    for (var i in obj) {
        if (obj[i] && typeof obj[i] == "object")
            newObj[i] = obj[i].clone();
        else
            newObj[i] = obj[i];
    }

    return newObj;
}

function filterByStandard(filteredID, pStandardData) {
    var lStandardData = JSON.parse(JSON.stringify(pStandardData[0].Table));
    lStandardData = $.grep(lStandardData, function (e) { return (e.StandardID == filteredID); });
    updateAvailableStandardsDataTable(lStandardData, standardData);
}

function filterByStudent(filteredID, pStudentdData) {
    if (pStudentdData[0].Table.length > 0) {
        var lStudentdData = JSON.parse(JSON.stringify(pStudentdData[0].Table));
        lStudentdData = $.grep(lStudentdData, function (e) { return ($.trim(e.EntityID) == filteredID); });
        updateAvailableStudentsDataTable(lStudentdData, studentData);
    } else {
        var lStudentdData = JSON.parse(JSON.stringify(pStudentdData[1].Table1));
        lStudentdData = $.grep(lStudentdData, function (e) { return ($.trim(e.EntityID) == filteredID); });
        updateAvailableStudentsDataTable(lStudentdData, studentData);
    }
}

function populateStandards(standardsData) {
    $("#ddlCompetency").empty();

    for (i = 0; i < standardsData.length; i++) {
        $("#ddlCompetency").append(
            $('<option></option>').val(standardsData[i].StandardID).text(standardsData[i].StandardName)
        );
    }
}

function populateStudents(studentsData) {
    $("#ddlCompetency").empty();
    $("#ddlCompetency").append($('<option></option>').val('').text('All Students'));

    for (i = 1; i < studentsData.length; i++) {
        $("#ddlCompetency").append(
            $('<option></option>').val(studentsData[i].EntityID).text(studentsData[i].EntityName)
        );
    }
}

function showCommentDialog(WorksheetID, StandardID, StudentID) {
    $('#divCommentDialogframe').attr('src', relativePathPrefix + 'Controls/CompetencyWorksheet/Comments.aspx?WorksheetID=' + WorksheetID + '&StandardID=' + StandardID + '&StudentID=' + StudentID);
    var opt2 = {
        autoOpen: false,
        modal: true,
        width: 'auto',
        height: 'auto',
        postition: 'center',
        close: function () {
            $('#divCommentDialogframe').attr('src', '');
        }
    };
    var theDialog2 = $("#divCommentDialog").dialog(opt2);
    theDialog2.dialog("open");

    return false;
}

function ChangeUpdatedValue(boolHistoryUpdated) {
    isHistoryUpdated = boolHistoryUpdated;
}

function showHistoryDialog(WorksheetID, StandardID, StudentID) {

    $('#divHistoryDialogframe').attr('src', relativePathPrefix + 'Controls/CompetencyWorksheet/WorksheetHistory.aspx?WorksheetID=' + WorksheetID + '&StandardID=' + StandardID + '&StudentID=' + StudentID);
    var opt = {
        autoOpen: false,
        modal: true,
        width: 'auto',
        height: 'auto',
        postition: 'fixed',
        close: function() {
            if (isHistoryUpdated) {
                $('#divHistoryDialogframe').attr('src', '');
                var redirectUrl = "";
                var loc = window.location.href;
                redirectUrl = loc;
                index = loc.indexOf('#');

                if (index > 0) {
                    redirectUrl = loc.substring(0, index);
                }

                if (isViewByStandard == true) {
                    getViewByStandardData(WorksheetID, ClassID);
                }
                else {
                    getViewByStudentData(WorksheetID, ClassID);
                }
                isHistoryUpdated = false;
            }
        }
    };

    var theDialog = $("#divHistoryDialog").dialog(opt);
    theDialog.dialog("open");

    return false;
}

function showEditDialog(WorksheetID) {
    $('#divCWEditDialogframe').attr('src', relativePathPrefix + 'Controls/CompetencyWorksheet/CompetencyWorksheetIdentification.aspx?WorksheetID=' + WorksheetID + '&type=Edit');
    var opt = {
        autoOpen: false,
        modal: true,
        width: 'auto',
        height: 'auto',
        postition: 'center',
        close: function () {
            parent.$('#divCWEditDialogframe').attr('src', '');
        }
    };

    var theDialog = $("#divCWEditDialog").dialog(opt);
    theDialog.dialog("open");

    return false;
}

function showCopyDialog(WorksheetID) {
    $('#divCWCopyDialogframe').attr('src', relativePathPrefix + 'Controls/CompetencyWorksheet/CompetencyWorksheetIdentification.aspx?WorksheetID=' + WorksheetID + '&type=Copy');

    var opt = {
        autoOpen: false,
        modal: true,
        width: 'auto',
        height: 'auto',
        postition: 'center',
        close: function () {
            $('#divCWCopyDialogframe').attr('src', '');
        }
    };

    var theDialog = $("#divCWCopyDialog").dialog(opt);
    theDialog.dialog("open");

    return false;
}

function addCompetencies(WorksheetID) {
    $('#divCWAddRemoveCompetenciesframe').attr('src', relativePathPrefix + 'Controls/CompetencyWorksheet/CompetencyStdAddNewAssoc.aspx?parentnodeid=' + WorksheetID);
    var opt = {
        autoOpen: false,
        modal: true,
        width: 'auto',
        height: 'auto',
        postition: 'center',
        close: function () {
            var redirectUrl = "";
            var loc = window.location.href;
            redirectUrl = loc;
            index = loc.indexOf('#');
            if (index > 0) {
                redirectUrl = loc.substring(0, index);
            }
            window.location.href = redirectUrl;
        }
    };

    var theDialog = $("#divCWAddRemoveCompetenciesDialog").dialog(opt);
    theDialog.dialog("open");

    return false;
}

function CloseConfirmation() {
    if (CWHRIArray.length > 0) {
        if (confirm("Are you sure you want to cancel. Any updates will be lost.")) {
            window.open('', '_self', '');
            window.close();
        }
    }
    else {
        window.open('', '_self', '');
        window.close();
    }
}

function DeleteWorksheet() {
    if (window.confirm("Are you sure you want to delete this worksheet? You will not be able to recover it once it has been deleted.")) {
        $.ajax({
            type: "POST",
            url: relativePathPrefix + "CompetencyWorksheetPreview.aspx/DeleteWorksheet",
            data: "{'WorksheetId':'" + WorksheetID + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(textStatus + "\n" + XMLHttpRequest.responseText);
            },
            success: function (result) {
                if (result && result.d) {
                    var msg = result.d;
                    alert(msg);
                }
                window.opener.location.href = window.opener.location.href;
                window.open('', '_self', '');
                window.close();
            }
        });
    }
    else {
        return false;
    }
}

$(document).on({
    ajaxStart: function () {
        $("#tbldiv").addClass("modal");
    },
    ajaxStop: function () {
        $("#tbldiv").removeClass("modal");
    }
});

function clickViewByStandard(sender, args) {
    if (isViewByStandard == true) {
        return false;
    }

    if (CWHArray.length > 0 || CWHRIArray.length > 0) {
        if (window.confirm('There are unsaved changes on the worksheet. Are you sure you want to proceed?')) {
            switchToStandardView();
        }
        else {
            args.set_cancel(true);
        }
    }
    else {
        switchToStandardView();
    }
}

function clickViewByStudent(sender, args) {
    if (isViewByStandard == false)
        return false;

    if (CWHArray.length > 0 || CWHRIArray.length > 0) {
        if (window.confirm('There are unsaved changes on the worksheet. Are you sure you want to proceed?')) {
            switchToStudentView();
        }
        else {
            args.set_cancel(true);
        }
    }
    else {
        switchToStudentView();
    }
}

function switchToStandardView() {
    $("#SearchCompetencyTitle").html("Standard:");
    isViewByStandard = true;
    getViewByStandardData(WorksheetID, ClassID);
    CWHArray = new Array();
    CWHRIArray = new Array();  
    filteredID = '';
    isFiltered = false;
}

function switchToStudentView() {
    $("#SearchCompetencyTitle").html("Student:");
    isViewByStandard = false;
    getViewByStudentData(WorksheetID, ClassID);
    CWHArray = new Array();
    CWHRIArray = new Array();
    filteredID = '';
    isFiltered = false;
}

$(document).ready(function () {

    var href = "#";

    $('#divViewByStudent').show();
    $('#divViewByStandard').hide();
    getViewByStudentData(WorksheetID, ClassID);
    
    $('div#divCommentDialog').dialog({ autoOpen: false, modal: true });

    $("#divCommentDialog").dialog({
        title: "Comments",
        modal: true
    });

    $('div#divHistoryDialog').dialog({ autoOpen: false, modal: true });

    $("#divHistoryDialog").dialog({
        title: "History",
        modal: true
    });

    $('div#divCWEditDialog').dialog({ autoOpen: false, modal: true });

    $("#divCWEditDialog").dialog({
        title: "Edit Identification",
        modal: true
    });

    $('div#divCWCopyDialog').dialog({ autoOpen: false, modal: true });

    $("#divCWCopyDialog").dialog({
        title: "Copy Identification",
        modal: true
    });

    $("#btnEdit").click(function () {
        showEditDialog(WorksheetID);
    });

    $("#btnCopy").click(function () {
        showCopyDialog(WorksheetID);
    });

    $("#btnSave").click(function () {
        saveHistory();
    });

    $("#btnAddRemoveCompetencies").click(function () {
        addCompetencies(WorksheetID);
    });

    $("#btnAddRemoveCompetenciesByStudent").click(function () {
        addCompetencies(WorksheetID);
    });

    $('#ddlView').change(function(c) {
        var optionSelected = $(this).val();

        if (CWHArray.length > 0 || CWHRIArray.length > 0) {
            if (!window.confirm('There are unsaved changes on the worksheet. Are you sure you want to proceed?')) {
                if (optionSelected == 'Student') {
                    $(this).val('Standard');
                }
                else {
                    $(this).val('Student');
                }
                return;
            }
        }

        if (optionSelected == 'Student') {
            switchToStudentView();
        }
        else {
            switchToStandardView();
        }
    });

    $("#ddlCompetency").change(function (c) {
        $("#tbldiv").addClass("modal");
        filteredID = $(this).val();
        isFiltered = true;

        if (CWHArray.length > 0 || CWHRIArray.length > 0) {
            if (!window.confirm('There are unsaved changes on the worksheet. Are you sure you want to proceed?')) {
                if ($("#ddlCompetency" + "  option[value*='" + prevCompValue + "']").length > 0) {
                    $(this).val(prevCompValue);
                }
                else {
                    $("#ddlCompetency").prop('selectedIndex', 0);
                }

                $("#tbldiv").removeClass("modal");
                return;
            }
        }

        prevCompValue = $(this).val();
        CWHArray = new Array();
        CWHRIArray = new Array();

        if (filteredID == '-1')
            return;

        if (($("#SearchCompetencyTitle").text() == 'Standard:')) {            
            var lStandardData = standardData;
            setTimeout(function() {
                filterByStandard(filteredID, lStandardData);
                $("#tbldiv").removeClass("modal");
            }, 0);
        }
        else {
            var lStudentData = studentData;           
            setTimeout(function () {
                filterByStudent(filteredID, lStudentData);
                $("#tbldiv").removeClass("modal");
            }, 0);
        }
    });
});