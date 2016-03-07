<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompetencyStdAddNewAssoc.aspx.cs" Inherits="Thinkgate.Controls.CompetencyWorksheet.CompetencyStdAddNewAssoc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <script type="text/javascript" src="../../CMSScripts/Custom/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/jquery-migrate-1.1.0.min.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/jquery-core.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script>
    <script type="text/javascript" src="../../CMSScripts/jquery-cookie.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/jquery.dataTables.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/addNewDocument.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/tgDivTools.js"></script>
    <script type="text/javascript" src="../../Scripts/master.js"></script>


    <script>var $j = jQuery.noConflict();</script>

    <link href="../../CMSScripts/Custom/reset-min.css" rel="stylesheet" />
    <link href="../../CMSScripts/Custom/site_jui.ccss" rel="stylesheet" />
    <link href="../../CMSScripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
    <link href="../../CMSScripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <link href="../../CMSScripts/Custom/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../CMSWebParts/ThinkgateWebparts/css/tgwebparts.css" rel="stylesheet" />
    <link href="../../CMSWebParts/ThinkgateWebparts/css/associationToolbar.css" rel="stylesheet" />


    <style type="text/css">
        select {
            width: 180px !important;
            font-size: 11px !important;
        }

            select:hover {
                cursor: default;
            }

        .modal {
            display: block;
            position: fixed;
            z-index: 1000;
            top: 0;
            left: 0;
            right: 0px;
            height: 100%;
            width: auto;
            background: rgba( 255, 255, 255, .8 ) url('../../Styles/Thinkgate_Window/Common/loading.gif') 60% 50% no-repeat;
        }

        .row {
            display: table-row;
            float: right;
        }

        .roundButtons {
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            position: relative;
            border: solid 1px #000;
            border-bottom: solid 1px #000;
            border-radius: 50px;
            float: right;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
            text-align: center;
        }

        .cellValue {
            display: table-cell;
            height: 30px;
            text-align: left;
            font-size: 11pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
        }
        .headerwidth {
width: 0px;
display: none;
}


        input[type="button"]:disabled {
            color: #bfbdbd;
            background-color: white;
        }
    </style>
</head>
<body>
    <div class="css_clear"></div>

    <form id="frmStandardsAddNewAssoc" runat="server">
     <telerik:RadScriptManager ID="radScriptManager" runat="server" EnablePageMethods="True">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
                <asp:ScriptReference Path="~/scripts/master.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <asp:HiddenField ID="SelectedItems" runat="server" />
        <asp:HiddenField ID="DocID" runat="server" />

        <%
            bool createAllowed = true;
            bool modifyAllowed = true;
            int nodeID = Convert.ToInt32(this.DocumentID);
        %>
        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">Current</a></li>
                <%
                    if (createAllowed || modifyAllowed)
                    {
                %>
                <li><a href="#tabs-2">Available</a></li>
                <%
                    }
                %>
            </ul>

            <div id="tabs-1" style="padding-top: -5px;">
                <div style="padding: 0px; height: 410px; width: 100%; position: relative;">

                    <div id="showStandardsModal" title="" style="display: block">

                        <div id="showStandardsModal-dialog-content"></div>
                        <br />

                        <div class="dataTables_wrapper">
                            <table id="currentStandardsDataTable" border="0" class="display" style="width: 95%"></table>
                        </div>
                    </div>

                    <div id="LinkButtonsDiv1" class="pull-left" style="position: relative; top: 5px;">
                        <%
                            if (createAllowed || modifyAllowed)
                            {
                        %>
                        <a href="#" onclick="validationFromHistory()" id="btnDelNewStandards2" class="btn btn-success">
                            <i class="icon-trash icon-white"></i>&nbsp;Delete Selected Standard</a>
                        <asp:LinkButton ID="btnDelNewStandards" ClientIDMode="Static" runat="server" OnClick="DelSelectedItems_Click" Style="display: none"></asp:LinkButton>
                        <%
                            }
                        %>
                    </div>
                </div>
            </div>

            <%
                if (createAllowed || modifyAllowed)
                {
            %>
            <div id="tabs-2" style="padding-top: -5px;">
                <%
                }
                else
                {
                %>
                <div id="Div1" style="padding-top: -5px; display: none;">
                    <%
                }
                    %>
                    <div style="padding: 0px; height: 425px; width: 100%; position: relative; top: -5px;">

                        <div class="ui-widget" id="buttonBarDiv2" style="display: block">
                            <div class="ui-widget" id="Div2" style="display: block">
                                <span style="float: left; padding-right: 1em;">
                                    <span class="tgLabel1">Standard: </span>
                                    <div id="StandardSetDdldiv">
                                        <select id="StandardSetDdl" onchange="getStandardSetList();"></select>
                                    </div>
                                </span>
                                <span style="float: left; padding-right: 1em;">
                                    <span class="tgLabel1">Grade: </span>
                                    <div id="GradeDdldiv">
                                        <select id="GradeDdl" onchange="getSubjectList();"></select>
                                    </div>
                                </span>
                                <span style="float: left; padding-right: 1em;">
                                    <span class="tgLabel1">Subject: </span>
                                    <div id="SubjectDdldiv">
                                        <select id="SubjectDdl" onchange="getCourseList();"></select>
                                    </div>
                                </span>
                                <span style="float: left; padding-right: 1em;">
                                    <span class="tgLabel1">Course: </span>
                                    <div id="CourseDdldiv">
                                        <select id="CourseDdl"></select>
                                    </div>
                                </span>

                            </div>
                            <br />
                            <br />
                            &nbsp;
                        <div class="ui-widget" id="Div3" style="display: block; clear: both;">
                            <span style="float: left; padding-right: 1em;">
                                <span class="tgLabel1">Text Search: </span>
                                <div id="TextSearchTxtdiv">
                                    <input id="TextSearch" type="text" value="" style="width: 165px;" />
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <span class="tgLabel1">&nbsp;</span>
                                <div id="TextSearchOptionsDdldiv">
                                    <select id="TextSearchOptionsDdl" style="width: 140px;">
                                        <option value="any">Any Words</option>
                                        <option value="all">All Words</option>
                                        <option value="exact">Exact Phrase</option>
                                    </select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <span class="tgLabel1">&nbsp;&nbsp;</span>
                                <div id="SearchResourcesButton" style="padding-left: 8px; vertical-align: bottom;">
                                    <input id="btnSearchResources" type="image" src="../../Images/search-1.png" onclick="getStandardsDataTable(); return false;" />
                                </div>
                            </span>
                        </div>



                            <br />

                            <div id="showStandardsModal2" title="" style="display: block">
                                <div id="showStandardsModal-dialog-content2"></div>

                                <div class="dataTables_wrapper">
                                    <span style="font-size: 87%; vertical-align: text-bottom;">
                                        <input id="chkSelectAll" type="checkbox" onclick="toggleClick(this)" class="checkbox" />&nbsp;Select All Standards</span>
                                    <table class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix" width="100%">
                                        <tr>
                                            <td>
                                                <table id="availableStandardsDataTable" border="0" class="display" style="width: 95%"></table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <div id="LinkButtonsDiv2" class="pull-left" style="position: relative; top: 5px;">
                                <a href="#" onclick="validationMessage()" id="btnAddNewStandards2" class="btn btn-success">
                                    <i class="icon-share icon-white"></i>&nbsp;Add Selected Standards</a>
                                <asp:Button ID="btnAddNewStandards"  Style="display: none" runat="server" OnClick="AddSelectedItems_Click"></asp:Button>
                            </div>
                        </div>

                    </div>
                </div>
                <%
                
                    if (PreviousPage != "")
                    {
                %>
                <div class="row">
                    <div class="cellValue">
                        <input id="btnContinue" class="roundButtons" value="  Save  " style="width: 100px;" type="button" onclick="closeCmsDialogOnCancel(this);" />
                        <input id="btnClose" class="roundButtons" value="  Close  " onclick="closeCmsDialogOnCancel(this);" style="width: 100px;" type="button" />
                    </div>
                </div>
                <%
                    } 
                %>
            </div>
            <br />
            <br /></div>
    </form>

    <script type="text/javascript">
        // Initialize variables
        var newEntry, temptable = [], includedIds = [], savedStandards = [], temptable_ava = [], includedIds_ava = [];
        var isMerged = false;
        var page = '<%=this.PreviousPage %>';
        /* Select stanadrd to tag's educational alignments in tag's UI. This needs to be saved into EA as pipe separated values.
           It is different from association. Also, selected standards must be associated only when tag is saved, not before that.
        */
        function attachStandardsToTagEA() {
            var selectedItems = $j('input[name=SelectedItems]').val();

            $j.ajax({
                type: "POST",
                url: "CompetencyStdAddNewAssoc.aspx/AddSelectedItems",
                data: "{'docid':'" + getDocIDValue() + "', 'SelectedItems':'" + selectedItems + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    getStandardCount(getDocIDValue());
                }
            });
            return true;

        }

        function getStandardsList() {
            $j.ajax({
                type: "POST",
                url: "CompetencyStdAddNewAssoc.aspx/getStandardSetList",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#StandardSetDdldiv')[0].innerHTML = "<select id='StandardSetDdl' onchange='getStandardSetList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("GradeDdl");
                    tgClearSelectOptionsFast("SubjectDdl");
                    tgClearSelectOptionsFast("CourseDdl");

                }
            });
        }

        function getStandardCount(docid) {
            var count = 0;

            $j.ajax({
                type: "POST",
                url: "CompetencyStdAddNewAssoc.aspx/getStandardCount",
                data: "{'docid':'" + docid + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    count = result.d;
                    if (count > 0) {
                        getCurrentStandards(docid);
                        $j('#tabs').tabs("option", "active", 0);

                    } else {
                        updateCurrentStandardsDataTable(null);
                        $j('#tabs').tabs("option", "active", 1);
                    }
                    updateAvailableStandardsDataTable("");
                    getStandardsList();
                }
            });
            return count;
        }

        function getCurrentStandards(docid) {

            $j.ajax({
                type: "POST",
                url: "CompetencyStdAddNewAssoc.aspx/getCurrentStandards",
                data: "{'docid':'" + docid + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    var data = JSON.parse(result.d);
                    updateCurrentStandardsDataTable(data);
                    savedStandards = [];
                    for (var i = 0; i < data.length; i++) {
                        //alert(data[i].ID);
                        includedIds.push(parseInt(data[i].ID));
                        var newEntry1 = {
                            "ID": parseInt(data[i].ID),
                            "StandardName": data[i].StandardName,
                            "Description": data[i].Description
                        };
                        savedStandards.push(newEntry1);
                    }
                    if(page!="")
                        $j('input[name=SelectedItems]').val(includedIds + "");                    
                }
            });
        }


        function getClientID() {
            $j.ajax({
                type: "POST",
                url: "CompetencyStdAddNewAssoc.aspx/getDistrictFromParmsTable",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                    return null;
                },
                success: function (result) {
                    return result.d;
                }
            });

        }

        function getStandardSetList() {
            var stdset = $j('#StandardSetDdl').find(':selected').text();
            $j.ajax({
                type: "POST",
                url: "CompetencyStdAddNewAssoc.aspx/getStandardSetGrade",
                data: "{'standardSet':'" + stdset + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#GradeDdldiv')[0].innerHTML = "<select id='GradeDdl' onchange='getSubjectList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("SubjectDdl");
                    tgClearSelectOptionsFast("CourseDdl");
                }
            });
        }

        function getSubjectList() {
            var stdset = $j('#StandardSetDdl').find(':selected').text();
            var grade = $j('#GradeDdl').find(':selected').text();
            $j.ajax({
                type: "POST",
                url: "CompetencyStdAddNewAssoc.aspx/getStandardSetGradeSubject",
                data: "{'standardSet':'" + stdset + "', 'grade':'" + grade + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#SubjectDdldiv')[0].innerHTML = "<select id='SubjectDdl' onchange='getCourseList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("CourseDdl");
                }
            });
        }

        function getCourseList() {
            var stdset = $j('#StandardSetDdl').find(':selected').text();
            var grade = $j('#GradeDdl').find(':selected').text();
            var subject = $j('#SubjectDdl').find(':selected').text();
            $j.ajax({
                type: "POST",
                url: "CompetencyStdAddNewAssoc.aspx/getStandardSetGradeSubjectCourse",
                data: "{'standardSet':'" + stdset + "', 'grade':'" + grade + "', 'subject':'" + subject + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#CourseDdldiv')[0].innerHTML = "<select id='CourseDdl' >" + result.d + "</select>";
                }
            });
        }

        function getStandardsDataTable() {
            /* reset all selected Items*/
            $j('input[name=SelectedItems]').val('');
            /* reset Select all checkbox*/
            $j('#chkSelectAll').removeAttr('checked');
            temptable_ava = [];
            includedIds_ava = [];
            var stdset = $j('#StandardSetDdl').find(':selected').text();
            var grade = $j('#GradeDdl').find(':selected').text();
            var subject = $j('#SubjectDdl').find(':selected').text();
            var course = $j('#CourseDdl').find(':selected').text();
            var stdsetVal = $j('#StandardSetDdl').find(':selected').val();
            var gradeVal = $j('#GradeDdl').find(':selected').val();
            var subjectVal = $j('#SubjectDdl').find(':selected').val();
            var courseVal = $j('#CourseDdl').find(':selected').val();
            var searchOption = $j("#TextSearchOptionsDdl").val();
            var searchText = escape($j("#TextSearch").val());

            if (stdsetVal == 0) return false;

            $j.ajax({
                type: "POST",
                url: "CompetencyStdAddNewAssoc.aspx/GetStandardsList",
                data: "{'standardSet':'" + stdset + "', 'standardSetVal':'" + stdsetVal + "', 'grade':'" + grade + "', 'gradeVal':'" + gradeVal + "', 'subject':'" + subject + "', 'subjectVal':'" + subjectVal + "', 'course':'" + course + "', 'courseVal':'" + courseVal + "', 'searchOption':'" + searchOption + "', 'searchText':'" + searchText + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (xhr, textStatus, err) {
                    console.log("responseText: " + xhr.responseText);
                },
                success: function (result) {
                    var data = [];
                    if (result && result.d) {
                        data = JSON.parse(result.d);
                    }
                    updateAvailableStandardsDataTable(data);
                }
            });
        }

        $j(document).ready(function () {
            $j("#tabs").tabs();
            updateCurrentStandardsDataTable({});
            updateAvailableStandardsDataTable({});
            $j("#tabs-1").click(getStandardCount(getDocIDValue()));
            $j(".dataTables_filter").css('display', 'none');
        });


        function getDocIDValue() {
            return $j("#DocID").val();
        }


        function updateCurrentStandardsDataTable(jsondata) {
            try {
                if (typeof oTable1 == 'undefined') {
                    oTable1 = $j('#currentStandardsDataTable').dataTable({
                        //"iDisplayLength": 25,
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "sScrollY": 200,
                        "aaSorting": [],
                        "aaData": jsondata,
                        "bAutoWidth": false,
                        "sScrollXInner": "100%",
                        "aoColumns":
					[
						{ "sTitle": "ID", "mData": "ID", "sClass": "headerwidth" },
						{ "sTitle": "Standard", "mData": "StandardName", "sWidth": "30%" },
						{ "sTitle": "Description", "mData": "Description", "sWidth": "70%" }
					]
                    });

                }
                else {
                    oTable1.fnClearTable(0);

                    if (jsondata !== null) {
                        oTable1.fnAddData(jsondata);
                        $j("#btnContinue").prop('disabled', false).prop('refresh');
                    } else {
                        $j("#btnContinue").prop('disabled', true).prop('refresh');
                    }


                    setTimeout(function () { oTable1.fnAdjustColumnSizing(); }, 10);
                    oTable1.fnFilter("x", 0); oTable1.fnFilter("", 0);


                    oTable1.fnDraw();
                }
            } catch (e) {
                alert("ex: " + e.message)
            }
            if (page != "")
                tgMakeTableSelectableRow('currentStandardsDataTable');
            else
                tgMakeTableSelectable('currentStandardsDataTable');
            return oTable1;
        }


        function updateAvailableStandardsDataTable(jsondata) {
            try {
                if (typeof oTable2 == 'undefined') {
                    oTable2 = $j('#availableStandardsDataTable').dataTable({
                        //"iDisplayLength": 100,
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "sScrollY": 170,
                        "aaSorting": [],
                        "aaData": jsondata,
                        "aoColumns":
					[
						//ID, Standard_Set, Grade, Subject, Course, Level, StandardName, \"Desc\" as Description                        
						{ "sTitle": "ID", "mData": "ID", "sClass": "headerwidth" },
						{ "sTitle": "Standard", "mData": "StandardName", "sWidth": "30%" },
						{ "sTitle": "Description", "mData": "Description", "sWidth": "70%" }
					],
                    });

                }
                else {
                    oTable2.fnClearTable(0);

                    oTable2.fnAddData(jsondata);

                    oTable2.fnDraw();
                }
            } catch (e) {
                alert("ex: " + e.message)
            }
            if (page != "")
                tgMakeTableSelectableRow('availableStandardsDataTable');
            else
                tgMakeTableSelectable('availableStandardsDataTable');

        }

        function toggleClick(ctrl) {
            if (page == "")
                tgSelectAll('availableStandardsDataTable');
            else
                tgSelectAllClient('availableStandardsDataTable');
        }

        function tgSelectAllClient(tableID) {
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
                fnGetSelectedRow(tableID);


            }
        }
        function validationMessage() {
            var isvalid = true;
            if (page != "") {
                // $j("#tabs").tabs("refresh");
                if (savedStandards.length > 0 && isMerged == false) { temptable = $j.merge($j.merge([], savedStandards), temptable); isMerged = true; }
                var countExisting = parseInt(temptable.length) || 0;
                var countAvailable = parseInt(temptable_ava.length) || 0;
                if ((countExisting + countAvailable) > 500) {
                    confirm("A worksheet may not contain more than 500 standards. The selected worksheet has reached this limit. You may either remove standards currently on the worksheet or create a new worksheet.");
                    isvalid = false;
                }
                else {
                    temptable = $j.merge($j.merge([], temptable_ava), temptable)
                    includedIds = $j.merge($j.merge([], includedIds_ava), includedIds)
                    $j('input[name=SelectedItems]').val(includedIds + "");
                    temptable_ava = [];
                    includedIds_ava = [];
                    if (temptable.length == 0) return false;
                    updateCurrentStandardsDataTable(temptable);
                    $j("#tabs").tabs("option", "active", 0);
                    $j("#StandardSetDdldiv")[0].innerHTML = "<select id='StandardSetDdl' onchange='getStandardSetList();'></select>";
                    $j('#GradeDdldiv')[0].innerHTML = "<select id='GradeDdl' onchange='getSubjectList();'></select>";
                    $j('#SubjectDdldiv')[0].innerHTML = "<select id='SubjectDdl' onchange='getCourseList();'></select>";
                    $j('#CourseDdldiv')[0].innerHTML = "<select id='CourseDdl'></select>"; $j('#chkSelectAll').removeAttr('checked');
                    //temptable = [];
                    // includedIds = [];
                    getStandardsList();
                    updateAvailableStandardsDataTable("");
                    isvalid = true;
                }
            }
            else {
                var selectedItems = $j('input[name=SelectedItems]').val();
                if (selectedItems.length == 0) return false;
                $j.ajax({
                    type: "POST",
                    url: "CompetencyStdAddNewAssoc.aspx/ValidateUI",
                    data: "{'SelectedItems':'" + selectedItems + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //alert(textStatus + "\n" + XMLHttpRequest.responseText);
                    },
                    success: function (result) {
                        if (result && result.d) {
                            var counter = result.d;
                            // alert(counter);
                            if (counter > 500) {
                                confirm("A worksheet may not contain more than 500 standards. The selected worksheet has reached this limit. You may either remove standards currently on the worksheet or create a new worksheet.");
                                isvalid = false;
                            }
                            else {
                                isvalid = true;
                                $j('#btnAddNewStandards').click();
                            }
                        }
                    }
                });
            }
            return isvalid;
        }
        function validationFromHistory() {
            if (page == "") {
                var selectedItems = $j('input[name=SelectedItems]').val();
                $j.ajax({
                    type: "POST",
                    url: "CompetencyStdAddNewAssoc.aspx/ValidateWorksheetHistory",
                    data: "{'SelectedItems':'" + selectedItems + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus + "\n" + XMLHttpRequest.responseText);
                    },
                    success: function (result) {
                        if (result && result.d) {
                            var counter = result.d;
                            if (counter > 0) {
                                if (window.confirm("Are you sure you wish to remove the selected competencies? There are competencies that have been scored on this worksheet and if removed all data for the competencies will be lost. Do you wish to continue?")) {
                                    document.getElementById('btnDelNewStandards').click();
                                }
                            }
                            else {
                                document.getElementById('btnDelNewStandards').click();
                            }
                        }
                    }
                });
            }
            else {
                if (oTable1 != 'undefined') {
                    var anSelected = fnGetSelectedRows(oTable1);
                    for (var i = 0 ; i < anSelected.length; i++) {
                        var iRow = oTable1.fnGetPosition(anSelected[i]);
                        var dat = oTable1.fnGetData(iRow[0]);
                        var unSelectedNodeIdIndex = jQuery.inArray(dat[iRow].ID, includedIds);
                        //alert(dat[iRow].ID + "  Index: " + unSelectedNodeIdIndex);
                        if (unSelectedNodeIdIndex !== -1) {
                            includedIds.splice(unSelectedNodeIdIndex, 1);
                            if (isMerged == true)
                                temptable = findAndRemove(temptable, 'ID', dat[iRow].ID);
                            else
                                savedStandards = findAndRemove(savedStandards, 'ID', dat[iRow].ID);
                             
                        }
                        oTable1.fnDeleteRow(iRow);
                    }
                    $j('input[name=SelectedItems]').val(includedIds + "");                    
                    if (includedIds !== null) {
                        if (includedIds.length == 0)
                            $j("#btnContinue").prop('disabled', true).prop('refresh');
                        else
                            $j("#btnContinue").prop('disabled', false).prop('refresh');
                    }
                }
            }
        }

        function findAndRemove(array, property, value) {
            var tmpArray = array.slice(0);
            $j.each(array, function (index, result) {
                if (result[property] == value) {
                    //Remove from array
                    tmpArray.splice(index, 1);
                }
            });
            return tmpArray;
        }

        /* Get the rows which are currently selected */
        function fnGetSelectedRows(oTableLocal) {
            var aReturn = new Array();
            var aTrs = oTableLocal.fnGetNodes();

            for (var i = 0 ; i < aTrs.length ; i++) {
                if ($j(aTrs[i]).hasClass('row_selected')) {
                    aReturn.push(aTrs[i]);


                }
            }
            return aReturn;
        }

        $j(document).on({
            ajaxStart: function () {
                $j("#tbldiv").addClass("modal");
            },
            ajaxStop: function () {
                $j("#tbldiv").removeClass("modal");
            }
        });

        function closeCmsDialogOnCancel(btn) {
            var page = '<%=this.PreviousPage %>';
            var id = $j(btn).attr("id");
            if (id == "btnClose") {
                if (page == "") {
                    window.parent.jQuery('#divCWAddRemoveCompetenciesDialog').dialog('close');
                    return false;
                }
                else {
                    window.top.location.reload(true);
                    closeCurrentCustomDialog();
                }

            } else {
                if (page == "") {

                    window.parent.jQuery('#divCWAddRemoveCompetenciesDialog').dialog('close');
                }
                else {
                    $j('#btnAddNewStandards').click();
                    setTimeout(function () {
                        closedialogNRefreshPage();
                    },200);
                    
                }
            }
            return false;
        }

        function closedialogNRefreshPage() {
        
            closeCurrentCustomDialog();
            window.top.location.reload(true);
        }

        function refreshWindow() {
            window.location.href = window.location.href;
        }
        function tgMakeTableSelectableRow(tableID) {
            if (objExists(tableID)) {
                /* Add a click handler to the table rows - this could be used as a callback */
                $j('#' + tableID + ' tr').click(function () {
                    if ($j(this).hasClass('row_selected')) {
                        $j(this).removeClass('row_selected');
                    } else {
                        $j(this).addClass('row_selected');
                    }
                    fnGetSelectedRow(tableID);
                });
            } else {
                alert("Not Exists: " + tableID);
            }

        }
        function fnGetSelectedRow(tableID) {
            var aReturn = new Array();
            var aTrs = $j(".row_selected").children();
            if (objExists(aTrs) && aTrs.length > 0) {
                aReturn = getSelectedRowColumnValues(aTrs);
            }
            else {
                temptable_ava = [];
                includedIds_ava = [];
            }

            $j('input[name=SelectedItems]').val(aReturn + "");
            return aReturn;
        }

        function getSelectedRowColumnValues(aTrs) {

            var aReturn = new Array();
            var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
            var countExisting = parseInt(temptable.length) || 0;
            temptable_ava = [];
            includedIds_ava = [];
            for (var i = 0 ; i < aTrs.length ;) {
                var parid = aTrs[i].textContent;
                var counter = parseInt(includedIds_ava.length) || 0;
                if ((jQuery.inArray(parseInt(parid), includedIds) == -1 && jQuery.inArray(parseInt(parid), includedIds_ava) == -1 && !isNaN(parseInt(parid))) && (countExisting + counter) <= 500) {

                    var tblid = aTrs[i + 1].textContent;
                    var newEntry1 = {
                        "ID": parseInt(aTrs[i].textContent),
                        "StandardName": aTrs[i + 1].textContent,
                        "Description": aTrs[i + 2].textContent
                    };

                    temptable_ava.push(newEntry1);
                    includedIds_ava.push(parseInt(parid));
                }
                aReturn.push(parid);
                i = i + nbrcols;
            }

            return includedIds;
        }
    </script>
    <p>
        &nbsp;
    </p>
    <div id="tbldiv" class="modal"></div>
</body>
</html>
