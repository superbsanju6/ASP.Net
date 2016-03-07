function bindUsageGrid(jsondata) {
    try {
        var usageStatisticTable = $('#tblUsageStatistics').DataTable({
            data: jsondata.lstUsageData,
            columns: jsondata.lstMetaData,
            paging: false,
            searching: false,
            ordering: false,
            info: true,
            "drawCallback": function (settings) {
                var api = this.api();
                for (var i = 0, iLen = settings.aiDisplay.length ; i < iLen ; i++) {                    
                    //$('td:eq(0)', settings.aoData[settings.aiDisplay[i]].nTr).html(i + 1);
                }
                // Output the data for the visible rows to the browser's console
                //console.log(api.rows({ page: 'current' }).data());
            }
        });
    } catch (e) {
        alert("ex: " + e.message)
    }
}

function addColumnGroupTitle(lstGridColumGroup) {
    var columnGroupHeader = "<tr>";
    var colspancounter = 0, oldstartpoint = 1, totalNumericColumn = 0;
    var strSelectorLogin = ""; strSelectorCommon = ""; strSelectorClassroomAssessment = ""; strSelectorInstruction = "", strSelectorDistrictAssessment = "";
    for (var x = 0 ; x < lstGridColumGroup.length; x++) {
        var sectioncolor = "";
        colspancounter = lstGridColumGroup[x].ColumnSpan;

        if (lstGridColumGroup[x].GroupTitle.indexOf('Login') > -1) {
            sectionStyle = "logingroupcolumns";
            strSelectorLogin = "#tblUsageStatistics thead  tr:nth-child(2) th:nth-child(n+" + oldstartpoint.toString() + "):nth-child(-n+" + (colspancounter + oldstartpoint - 1).toString() + ")";
            oldstartpoint = colspancounter + oldstartpoint;
            totalNumericColumn += lstGridColumGroup[x].ColumnSpan;
        }
        else if (lstGridColumGroup[x].GroupTitle.indexOf('Classroom') > -1) {
            sectionStyle = "classroomgroupcolumns";
            strSelectorClassroomAssessment = "#tblUsageStatistics thead  tr:nth-child(2) th:nth-child(n+" + oldstartpoint.toString() + "):nth-child(-n+" + (colspancounter + oldstartpoint - 1).toString() + ")";
            oldstartpoint = colspancounter + oldstartpoint;
            totalNumericColumn += lstGridColumGroup[x].ColumnSpan;
        }
        else if (lstGridColumGroup[x].GroupTitle.indexOf('District') > -1) {
            sectionStyle = "districtgroupcolumns";
            strSelectorDistrictAssessment = "#tblUsageStatistics thead  tr:nth-child(2) th:nth-child(n+" + oldstartpoint.toString() + "):nth-child(-n+" + (colspancounter + oldstartpoint - 1).toString() + ")";
            oldstartpoint = colspancounter + oldstartpoint;
            totalNumericColumn += lstGridColumGroup[x].ColumnSpan;
        }
        else if (lstGridColumGroup[x].GroupTitle.indexOf('Instruction') > -1) {
            sectionStyle = "instructiongroupcolumns";
            strSelectorInstruction = "#tblUsageStatistics thead  tr:nth-child(2) th:nth-child(n+" + oldstartpoint.toString() + "):nth-child(-n+" + (colspancounter + oldstartpoint - 1).toString() + ")";
            oldstartpoint = colspancounter + oldstartpoint;
            totalNumericColumn += lstGridColumGroup[x].ColumnSpan;
        }
        else {
            sectionStyle = "commongroupcolumns";
            strSelectorCommon = "#tblUsageStatistics thead  tr:nth-child(2) th:nth-child(n+" + oldstartpoint.toString() + "):nth-child(-n+" + (colspancounter + oldstartpoint - 1).toString() + ")";
            oldstartpoint = colspancounter + oldstartpoint;
        }
        columnGroupHeader += "<th colspan='" + lstGridColumGroup[x].ColumnSpan + "' class='" + sectionStyle + "'>" + lstGridColumGroup[x].GroupTitle + "</th>";
    }

    columnGroupHeader += "</tr>";
    $('#tblUsageStatistics thead tr:first-child').before(columnGroupHeader);
    $('#tblUsageStatistics thead th').css({ 'border': 'solid 2px #dddddd' });
    $('#tblUsageStatistics thead th').css({ 'border-top': 'solid 2px #ffffff', 'border-left': 'solid 2px #ffffff', 'border-right': 'solid 2px #ffffff'});
    $('#tblUsageStatistics tr td').css({ 'font-weight': 'normal', 'font-color': '#000000', 'background-color': '#ffffff' });
    $('#tblUsageStatistics thead  tr:nth-child(2) th').css({ 'text-align': 'center' });
    $('#tblUsageStatistics thead  tr:nth-child(2) th').css({ 'border-bottom': 'solid 2px #000000' });
    

    $(strSelectorLogin).addClass('logintitle');
    $(strSelectorClassroomAssessment).addClass('classroomtitle');
    $(strSelectorDistrictAssessment).addClass('districttitle');
    $(strSelectorInstruction).addClass('instructiontitle');
    $(strSelectorCommon).addClass('commontitle');

    var numericColumnExpr = "#tblUsageStatistics tbody  tr td:nth-child(n4):nth-child(-n" + (totalNumericColumn + 3).toString() + ")";
    //$(numericColumnExpr).css({ 'text-align': 'right' });
    $(numericColumnExpr).each(function (index) {
        var nStr = $(this).text();
        //$(this).text(addCommas(nStr));
    });
    $('#tblUsageStatistics tbody  tr td').each(function (index) {
        var nStr = $(this).text(); if (nStr == "NA")
            $(this).css({ 'text-align': 'center', 'background-color': 'gray', 'color': 'black' });;
    });

    $('#tblUsageStatistics tr:first-child td').css({ 'border': 'solid 2px #000000', 'border-top': 'solid 2px #000000' });
    $('#tblUsageStatistics tr:first-child td:first-child').css({ 'border-left': 'solid 1px #ffffff' });
    
}

function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

