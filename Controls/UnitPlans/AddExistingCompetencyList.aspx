<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddExistingCompetencyList.aspx.cs" Inherits="Thinkgate.Controls.UnitPlans.AddExistingCompetencyList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Existing Item Search</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <link href="~/Scripts/reset-min.css" rel="stylesheet" />
    <link href="~/Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="~/Scripts/DataTables/css/site_jui.ccss" rel="stylesheet" />
    <link href="~/Scripts/DataTables/css/demo_table_jui.css" rel="stylesheet" />
    <link href="~/Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />

    <script type="text/javascript" src="../../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery-migrate-1.1.0.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.cookie.js"></script>
    <script type="text/javascript" src="../../Scripts/DataTables/js/jquery.dataTables.js"></script>
    <script type="text/javascript" src="../../Scripts/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="../../Scripts/Custom/addNewDocument.js"></script>
    <script type="text/javascript" src="../../Scripts/Custom/tgDivTools.js"></script>
    <script type="text/javascript" src="../../Scripts/master.js"></script>


    <style>
        .paging_full_numbers a.paginate_button {
            border: 0px none white !important;
            background-color: white !important;
        }

        .paging_full_numbers a.paginate_active {
            border: 0px none white !important;
        }

        .hiddenColumn {
            display: none;
        }

        table thead tr th {
            font-weight: bold;
            font-size: 105%;
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


        .textOverFlow {
            white-space: normal;
            text-overflow: ellipsis;
            overflow: hidden;
            display: block;
            width: 430px;
        }

        .noclose .ui-dialog-titlebar-close {
            display: none;
        }
    </style>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var message = "";
            var flag = "No";
            var isVerified = false;
            function getCourseList() {
                j$("#AddExistingMessage").text("");
                var grade = $('#GradeDdl').find(':selected').text().trim();
                var subject = $('#SubjectDdl').find(':selected').text().trim();

                j$.ajax({
                    type: "POST",
                    url: "../../Services/KenticoServices/KenticoWebServices.aspx/getCurrCourses",
                    data: "{'grade':'" + grade + "', 'subject':'" + subject + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //alert(textStatus + "\n" + errorThrown);
                    },
                    success: function (result) {
                        $("#SubjectText").val(subject);
                        $('#CourseDdldiv')[0].innerHTML = "<select id='CourseDdl' style='width: 180px;'>" + result.d + "</select>";
                    }
                });
            }


            function getSubjectList() {
                j$("#AddExistingMessage").text("");
                var grade = $('#GradeDdl').find(':selected').text();
                j$.ajax({
                    type: "POST",
                    url: "../../Services/KenticoServices/KenticoWebServices.aspx/getSubjects",
                    data: "{'grade':'" + grade + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //alert(textStatus + "\n" + errorThrown);
                    },
                    success: function (result) {
                        $('#SubjectDdldiv')[0].innerHTML = "<select id='SubjectDdl' onchange='getCourseList();' style='width: 180px;'>" + result.d + "</select>";
                        tgClearSelectOptionsFast("CourseDdl");
                        $("#CourseDdldiv")[0].innerHTML = "<select id='CourseDdl' style='width: 180px;'><option value='0'>---- Select Item ----- </option></select>";
                    }
                });
            }

            function getDocumentType() {
                j$.ajax({
                    type: "POST",
                    url: "AddExistingCompetencyList.aspx/BindResourceTypes",
                    data: null,

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus + "\n" + errorThrown);
                    },
                    success: function (result) {
                        if (result.d != "") {
                            $('#DocTypeDdldiv')[0].innerHTML = "<select id='DocTypeDdl' style='width: 180px;'>" + result.d + "</select>";
                        }
                    }
                });
            }


            function getGridData(flag) {
                if (flag != "No") {
                    if (flag != "All") {
                        var scopeVal = $('#drpdwnScope').find(':selected').val();
                        var classnameText = $('#DocTypeDdl').find(':selected').val();
                        var gradeVal = $('#GradeDdl').find(':selected').val();
                        var gradeText = $('#GradeDdl').find(':selected').text();
                        var subjectVal = $('#SubjectDdl').find(':selected').val();
                        var subjectText = $('#SubjectDdl').find(':selected').text();
                        var courseVal = $('#CourseDdl').find(':selected').val();
                        var courseText = $('#CourseDdl').find(':selected').text();
                        var selectedsearchoption = $("#TextSearchOptionsDdl").find(':selected').val();
                        var searchstring = $("#TextSearch").val();
                    } else if (flag == "All") {
                        j$("#AddExistingMessage").text("");
                    }

                    j$.ajax({
                        type: "POST",
                        url: "AddExistingCompetencyList.aspx/BindCompetencyGrid",
                        data: "{'scope':'" + scopeVal + "','classname':'" + classnameText + "','grade':'" + gradeText + "','gradeVal':'" + gradeVal + "', 'subject':'" + subjectText + "', 'subjectVal':'" + subjectVal + "', 'course':'" + courseText + "','courseVal':'" + courseVal + "','searchOption':'" + selectedsearchoption + "','searchText':'" + searchstring + "'}",

                        contentType: "application/json; charset=utf-8",
                        dataType: "json",

                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            //alert(textStatus + "\n" + XMLHttpRequest.responseText);
                            console.log("responseText: " + XMLHttpRequest.responseText);
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
            }


            function closeWindow() {
                var oWnd = getCurrentCustomDialog();
                var oWindow = GetRadWindow();
                oWindow.argument = null;
                oWindow.close();

                setTimeout(function () {
                    oWnd.close();
                    var oWindow = null;
                    if (window.radWindow) oWindow = window.radWindow;
                    else if (window.frameElement && window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                    oWindow.close();
                }, 0);
                return false;
            }
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function saveData() {
                j$("#AddExistingMessage").text("");
                var selectedNodeId = j$('#SelectedItems').val();
                var selecteddesc = j$("#selectedNodeDesc").val();
                var selectedname = j$("#selectedNodeName").val();
                selectedname = selectedname.substr(0, 100);
                selecteddesc = selecteddesc.substr(0, 200);
                var reqType = j$("#requestType").val();
                if (selectedNodeId != "" && selectedNodeId != ",") {
                    if (reqType == 2) {
                        parent.customDialog({ url: ('../Controls/CompetencyWorksheet/CompetencyWorksheetIdentification.aspx?id=' + selectedNodeId + '&name=' + selectedname + '&desc=' + selecteddesc + "&IsNew=1"), width: 500, height: 450, name: 'CompetencyWorksheetIdentification', title: 'Competency Worksheet Identification', onOpen: closeWindow, onClosed: closeCurrentCustomDialog, destroyOnClose: true });
                    }
                    else if (reqType == 1) {

                        j$.ajax({
                            type: "POST",
                            url: "AddExistingCompetencyList.aspx/CopySelectedItems",
                            data: "{'NodeId':'" + selectedNodeId + "','desc':'" + selecteddesc + "','currID':'-1'}",

                            contentType: "application/json; charset=utf-8",
                            dataType: "json",

                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                console.log("responseText: " + XMLHttpRequest.responseText);
                            },
                            success: function (result) {
                                var data = [];
                                if (result && result.d) {
                                    message = result.d;
                                    getGridData();
                                    $("#AddExistingMessage").text(message);
                                    $('#SelectedItems').val("");
                                }
                            }
                        });

                    }
                }
            }

            function saveRestrictCurriculaData() {
                var selectedNodeId = document.getElementById('SelectedItems').value;
                var selecteddesc = document.getElementById("selectedNodeDesc").value;
                var selectedname = document.getElementById("selectedNodeName").value;
                selectedname = selectedname.substr(0, 100);
                selecteddesc = selecteddesc.substr(0, 200);
                var reqType = document.getElementById("requestType").value;
                var currValue = $("input[type=radio]:checked").val();
                if (currValue == 'undefined' || currValue == "") currValue = "-1";
                if (selectedNodeId != "" && selectedNodeId != ",") {
                    if (reqType == 2) {
                        parent.customDialog({ url: ('../Controls/CompetencyWorksheet/CompetencyWorksheetIdentification.aspx?id=' + selectedNodeId + '&name=' + selectedname + '&desc=' + selecteddesc + "&IsNew=1"), width: 500, height: 450, name: 'CompetencyWorksheetIdentification', title: 'Competency Worksheet Identification', onOpen: closeWindow, onClosed: closeCurrentCustomDialog, destroyOnClose: true });
                    }
                    else if (reqType == 1) {

                        $.ajax({
                            type: "POST",
                            url: "AddExistingCompetencyList.aspx/CopySelectedItems",
                            data: "{'NodeId':'" + selectedNodeId + "','desc':'" + selecteddesc + "' ,'currID':'" + currValue + "'}",

                            contentType: "application/json; charset=utf-8",
                            dataType: "json",

                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                console.log("responseText: " + XMLHttpRequest.responseText);
                            },
                            success: function (result) {
                                closeCurrentCustomDialog();
                            }
                        });

                    }
                }

            }
            function validateStandards()
            {
                var selectedNodeId = j$('#SelectedItems').val();                
                j$.ajax({
                    type: "POST",
                    url: "AddExistingCompetencyList.aspx/ValidateDocStandards",
                    data: "{'NodeId':'" + selectedNodeId + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log("responseText: " + XMLHttpRequest.responseText);
                    },
                    success: function (result) {                        
                        if (result && result.d) {
                            var opt = {
                                autoOpen: false,
                                modal: true,
                                width: 500,
                                title: "500+ Standards",
                                height: 'auto',
                                postition: 'center',
                                dialogClass: "noclose",
                                buttons: {
                                    "OK": function () {
                                        saveData();
                                    },
                                    Cancel: function () {
                                        j$(this).dialog("close");
                                    }
                                }

                            };
                            var theDialog = j$("#divValidateStandards").dialog(opt);
                            theDialog.dialog("open");

                        }
                        else {
                            saveData();
                        }
                    }
                });

            }

            function validateCurricula() {

                var selectedNodeId = j$('#SelectedItems').val();

                if ((typeof (selectedNodeId) == 'undefined') || selectedNodeId == null || selectedNodeId.length === 0 || selectedNodeId=='') {
                    return alert("Please select an item from the existing item search list.");
                }

                var reqType = j$("#requestType").val();
                if (reqType == 2) {
                    validateStandards();
                    return
                }
                else {
                    isVerified = true;
                    var isValid = true;
                    j$.ajax({
                        type: "POST",
                        url: "AddExistingCompetencyList.aspx/ValidateDocCurricula",
                        data: "{'NodeId':'" + selectedNodeId + "'}",

                        contentType: "application/json; charset=utf-8",
                        dataType: "json",

                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            console.log("responseText: " + XMLHttpRequest.responseText);
                        },
                        success: function (result) {
                            var data = [];
                            if (result && result.d) {
                                if (result.d != "") {
                                    data = JSON.parse(result.d);
                                    j$("#divCuriculmlist").html('');
                                    j$.each(data, function (i, item) {
                                        var radioBtn = j$("<span style='margin-bottom:5px;'><input type='radio' style='margin-top:0px;' name='rbtnCount' id=" + item.ID + " value='" + item.ID + "' /><span class='rbText' style='font-size:9.5pt;font-family:Arial;padding-left:8px;'>" + item.Grade + " " + item.Subject + " " + item.Course + "</span> </span><br/>");
                                        radioBtn.appendTo("#divCuriculmlist");
                                        var opt = {
                                            autoOpen: false,
                                            modal: true,
                                            width: 600,
                                            title: "Select Curriculum",
                                            height: 'auto',
                                            postition: 'center',
                                            dialogClass: "noclose",
                                            buttons: {
                                                "OK": function () {
                                                    isVerified = true;
                                                    saveRestrictCurriculaData();
                                                },
                                                Cancel: function () {
                                                    j$(this).dialog("close");
                                                }
                                            }

                                        };

                                        var theDialog = j$("#divUpdateCurriculum").dialog(opt);
                                        theDialog.dialog("open");
                                        j$("input:radio[name=rbtnCount]:first").attr('checked', true);
                                        isValid = false;

                                    });
                                }
                            }
                            else {
                                saveData();
                            }
                        }
                    });
                    return isValid;
                }
            }

            var j$ = jQuery.noConflict();
            function updateAvailableStandardsDataTable(jsondata) {
                try {
                    var tableID = "tblAddExistingCompetencyList";
                    if (typeof oTable2 == 'undefined') {
                        oTable2 = j$('#tblAddExistingCompetencyList').dataTable({
                            "iDisplayLength": 10,
                            "bPaginate": true,
                            "bFilter": false,
                            "bSort": false,
                            "bLengthChange": false,
                            "sScrollY": 280,
                            "aaSorting": [[0, "asc"]],
                            "aaData": jsondata,
                            "bAutoWidth": false,
                            "sPaginationType": "full_numbers",
                            "aoColumns":
                        [
                            { "sTitle": "DocumentNodeID", "mData": "DocumentNodeID", "sClass": "hiddenColumn" },
                            { "sTitle": "List Type", "mData": "DocumentType", "sWidth": '15%' },
                            { "sTitle": "Document Name", "mData": "ResourceName", "sWidth": '25%' },
                            { "sTitle": "Description", "mData": "Description", "sWidth": '40%' },
                            { "sTitle": "Author", "mData": "AuthorName", "sWidth": '20%' }

                        ],
                            "fnDrawCallback": function (oSettings) {
                                j$('#tblAddExistingCompetencyList tr').removeClass("row_selected");
                                tgMakeTableSelectable('tblAddExistingCompetencyList');
                            }
                        });
                    }
                    else {
                        oTable2.fnClearTable(0);
                        oTable2.fnAddData(jsondata);
                        oTable2.fnPageChange(0);
                        oTable2.fnDraw();
                    }


                } catch (e) {
                    alert("ex: " + e.message)
                }

                tgMakeTableSelectable('tblAddExistingCompetencyList');

            }




            var radwindow;
            j$(document).ready(function () {
                getGridData('All');
                getDocumentType();
                var tgTableName = 'tblAddExistingCompetencyList';
                tgMakeTableSelectable(tgTableName);
            });


            j$(document).on({
                ajaxStart: function () {
                    j$("#tbldiv").addClass("modal");
                },
                ajaxStop: function () {
                    j$("#tbldiv").removeClass("modal");
                }
            });
        </script>
    </telerik:RadCodeBlock>
</head>
<body style="max-width: 99%; margin-left: 1em;">
    <form id="addExistingCompetencyListForm" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="wndWindowManager" runat="server" DestroyOnClose="true"></telerik:RadWindowManager>        
        <asp:HiddenField ID="SearchSelectedOption" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ParentNode" runat="server" Value='<%# Eval("ParentNodeID") %>' />
        <asp:HiddenField ID="SelectedItems" runat="server" />
        <asp:HiddenField ID="selectedNodeDesc" runat="server" />
        <asp:HiddenField ID="requestType" runat="server" />
        <asp:HiddenField ID="selectedNodeName" runat="server" />
        <br />
        <div style="margin-right: 2em;">
            <div id="LinkButtonsDiv1" class="pull-right">
                <a href="#" onclick="validateCurricula();" id="okbtn" class="btn btn-success" runat="server">
                    <i class="icon-share icon-white"></i>&nbsp;Select List
                </a>
                <a href="#" onclick="closeAddExistingDialog();" id="cancelbtn" class="btn btn-danger">
                    <i class="icon-remove icon-white"></i>&nbsp;Cancel
                </a>
            </div>

        </div>
        <div style="clear: both; width: 100px; float: left; margin-left: 10px;">
            <table style="width: 70%;">
                <tr>
                    <td style="width: 20%;">Scope</td>
                    <td style="width: 20%;">List Type</td>
                    <td style="width: 20%;">Grade</td>
                    <td style="width: 20%;">Subject</td>
                    <td style="width: 20%;">Course</td>
                </tr>
                <tr>
                    <td style="width: 20%; padding-right: 1em;">
                        <select id="drpdwnScope" class="dropdown-panel" style="width: 180px">
                            <option value="All">&nbsp;---- Select Item ---- </option>
                            <option value="MyDocuments">My Doc</option>
                            <option value="DistrictDocuments">District</option>
                            <option value="StateDocuments">State</option>
                        </select>
                    </td>
                    <td style="width: 20%; padding-right: 1em;">
                        <div id="DocTypeDdldiv">
                            <select id="DocTypeDdl" style="width: 180px;" onclick="getDocumentType();">
                                <option value="0">---- Select Item ----- </option>
                            </select>
                        </div>
                    </td>
                    <td style="width: 20%; padding-right: 1em;">
                        <div id="GradeDdldiv">
                            <asp:DropDownList ID="GradeDdl" runat="server" AutoPostBack="false" ClientIDMode="Static" onchange="getSubjectList();" Width="180"></asp:DropDownList>
                        </div>
                    </td>
                    <td style="width: 20%; padding-right: 1em;">
                        <div id="SubjectDdldiv">
                            <select id="SubjectDdl" onchange="getCourseList();" style="width: 180px;">
                                <option value="0">---- Select Item ----- </option>
                            </select>
                        </div>
                    </td>
                    <td style="width: 20%; padding-right: 1em;">
                        <div id="CourseDdldiv">
                            <select id="CourseDdl" style="width: 180px;">
                                <option value="0">---- Select Item ----- </option>
                            </select>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <br />
        <div class="ui-widget" id="Div1" style="display: block; clear: both; margin-left: 10px;">
            <span style="float: left; padding-right: 1em;">
                <label class="tgLabel1">Text Search: </label>
                <div id="TextSearchTxtdiv">
                    <asp:TextBox ID="TextSearch" ClientIDMode="Static" runat="server" Style="width: 165px;"></asp:TextBox>
                </div>
            </span>
            <span style="float: left; padding-right: 1em;">
                <label class="tgLabel1">&nbsp;</label>
                <div id="TextSearchOptionsDdldiv">
                    <select id="TextSearchOptionsDdl" style="width: 140px;">
                        <option value="any">Any Words</option>
                        <option value="all">All Words</option>
                        <option value="exact">Exact Phrase</option>
                        <option value="author">Author</option>
                    </select>
                </div>
            </span>
            <span style="float: right; padding-right: 7em;">
                <label class="tgLabel1">&nbsp;</label>
                <a href="#" onclick="getGridData();">
                    <image src="../../Images/search-1.png" />
                </a>
            </span>
        </div>
        <div>
            <div>
                <h4><span style="color: red; font-size: 12px;">
                    <label id="AddExistingMessage"></label>
                </span></h4>

            </div>
        </div>
        <br />
        <div style="width: 98%; margin-left: 1em; margin-top: 1em;">
            <div id="showStandardsModal2" title="">
                <div id="showStandardsModal-dialog-content2"></div>
                <br />
                <table id="tblAddExistingCompetencyList" border="0" class="display" style="width: 95%">
                </table>

            </div>
        </div>
        <div id="tbldiv" class="modal"></div>
        <div id="divUpdateCurriculum" style="margin-left: auto; margin-right: auto; width: 500px; height: 225px; font-size: 11pt; font-family: Arial; display: none;">
            <p>There is more than one curriculum associated with the existing document selected. Please choose one curriculum to associate to the newly created document and select OK.</p>
            <div id="divCuriculmlist" style="height: 100px;"></div>
        </div>
        <div id="divValidateStandards" style="margin-left: auto; margin-right: auto; width: 500px; height: 225px; font-size: 11pt; font-family: Arial; display: none;">
            <p>The existing document contains more than 500 standard associations. The first 500 will be selected and added to the new worksheet. Standard associations may be updated in the worksheet at any time.</p>            
        </div>
    </form>

</body>
</html>
