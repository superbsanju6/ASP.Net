<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetPerformanceLevelList.aspx.cs" Inherits="Thinkgate.Controls.Assessment.SetPerformanceLevelList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Performance Level List</title>

    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <script type="text/javascript" src="../../CMSScripts/Custom/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/jquery-migrate-1.1.0.min.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/jquery-core.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script>
    <script type="text/javascript" src="../../CMSScripts/jquery-cookie.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/jquery.dataTables.js"></script>
    <script type="text/javascript" src="../../CMSScripts/Custom/bootstrap/js/bootstrap.min.js"></script>


    <script type="text/javascript" src="../../Scripts/master.js"></script>


    <script>var $j = jQuery.noConflict();</script>

    <link href="../../CMSScripts/Custom/reset-min.css" rel="stylesheet" />
    <link href="../../CMSScripts/Custom/site_jui.ccss" rel="stylesheet" />
    <link href="../../CMSScripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
    <link href="../../CMSScripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <link href="../../CMSScripts/Custom/bootstrap/css/bootstrap.min.css" rel="stylesheet" />




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

        .dataTables_scrollBody {
            overflow: auto;
            height: 570px;
            width: 1000px;
        }


        .navButton {
            border: 1px solid #e3e3e3;
            color: #000;
            padding: 5px 5px 5px 25px;
            font-size: 14px;
        }

        .btnAdd {
            background: #f2f2f2 url(../../images/add.gif) 5px 50% no-repeat;
        }


        .test {
            position: fixed;
            width: 97% !important;
            /*z-index: 9999;*/
            margin-top: 30px;
            background: #ffffff;
            padding-top: 20px;
        }

        .floatLeft {
            float: left;
        }

        /*.dataTables_scrollBody {
            height: 425px;
        }*/
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div>



            <div id="divPerformanceLevelDialog" title="Add Performance Level" style="display: none;">
                <iframe id="divPerformanceLevelframe" src="" frameborder="0" width="950" height="500">No frames</iframe>
            </div>
            <div class="floatLeft">
           <input type="button" value="Add Performance Level" id="btnAddPerformanceLevelnew" class="navButton btnAdd" onclick="addPerformanceLevel();" />
                         
                </div>
            <div class="test">
                
           


                <div style="padding: 0px; height: 525px; width: 1000px; position: relative; top: -5px;">
                    <div id="showStandardsModal2" title="" style="display: block">
                        <div id="showPerformanceLevelsModal-dialog-content2"></div>

                        <div class="dataTables_wrapper">
                            <%-- <span style="font-size: 87%; vertical-align: text-bottom;">
                                        <input id="chkSelectAll" type="checkbox" onclick="toggleClick(this)" class="checkbox" />&nbsp;Select All Standards</span>--%>
                            <table class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix" width="100%">
                                <tr>
                                    <td>
                                        <table id="PerformanceLevelSetTable" border="0" class="display" style="width: 100%; height: 525px; border-collapse: collapse;" cellpadding="0" cellspancing="0"></table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <script>


            $j(document).ready(function () {



             
                $j.ajax({
                    type: "POST",
                    url: "SetPerformanceLevelList.aspx/GetPerofmranceLevel",
                    data: "{'Id':'0'}",

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
                        updatePerformanceLevelDataTable(data);
                    }
                });

              
            });

            function updatePerformanceLevelDataTable(jsondata) {
                try {
                    if (typeof oTable2 == 'undefined') {
                    oTable2 = $j('#PerformanceLevelSetTable').dataTable({
                        //"iDisplayLength": 100,
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "sScrollY": 400,
                        "aaSorting": [],
                        "aaData": jsondata,
                        "aoColumns":
                    [
                        //ID, Standard_Set, Grade, Subject, Course, Level, StandardName, \"Desc\" as Description                        
                        { "sTitle": "ID", "mData": "ID", "sClass": "headerwidth" },
                        { "sTitle": "Set Name", "mData": "SetName", "sWidth": "10%", "sClass": "linkme" },
                        { "sTitle": "Set Desc", "mData": "SetDescription", "sWidth": "10%" },
                        { "sTitle": "Set Min Score", "mData": "SetMinScore", "sWidth": "10%" },
                        { "sTitle": "Set Max Score", "mData": "SetMaxScore", "sWidth": "10%" },
                        { "sTitle": "Level Name", "mData": "LevelText", "sWidth": "10%" },
                        { "sTitle": "Level Desc", "mData": "LevelDescription", "sWidth": "10%" },
                        { "sTitle": "Level Min Score", "mData": "LevelMinScore", "sWidth": "10%" },
                        { "sTitle": "Level Max Score", "mData": "LevelMaxScore", "sWidth": "10%" },
                        { "sTitle": "Level Color", "mData": "LevelColor", "sWidth": "10%" },
                        { "sTitle": "Level Index", "mData": "LevelIndex", "sWidth": "10%" },
                        { "sTitle": "Level Abbr", "mData": "LevelAbbr", "sWidth": "10%" }
                    ],
                        "fnDrawCallback": function (oSettings) {
                           
                        },
                        "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                            // var td = $j('#PerformanceLevelSetTable tbody tr td.linkme');
                            decorateRow(nRow);
                            return nRow;

                        }
                       
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
                //if (page != "")
                //    tgMakeTableSelectableRow('availableStandardsDataTable');
                //else
                //    tgMakeTableSelectable('availableStandardsDataTable');

            }

            function decorateRow(row)
            {
                //$j('.linkme', row).html('<a style=color:blue; href="SetPerformanceLevelCut.aspx?ID=' + $j('.headerwidth', row).html() + '">' + $j('.linkme', row).html() + ' </a>');
                $j('.linkme', row).html('<a  style="text-decoration: underline;color:blue;" href="#" onclick="editPerformanceLevel(' + $j('.headerwidth', row).html() + ')">' + $j('.linkme', row).html() + ' </a>');
            }

          

            var relativePathPrefix = '';



            function editPerformanceLevel(id) {
                var k = 1;
                $j('#divPerformanceLevelframe').attr('src', 'SetPerformanceLevelCut.aspx?ID=' + id);
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

                var theDialog = $j("#divPerformanceLevelDialog").dialog(opt);
                theDialog.dialog("open");

                return false;
            }

            function addPerformanceLevel() {
                var k = 1;
                $j('#divPerformanceLevelframe').attr('src', 'SetPerformanceLevelCut.aspx');
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

                var theDialog = $j("#divPerformanceLevelDialog").dialog(opt);
                theDialog.dialog("open");

                return false;
            }

        </script>

    </form>
</body>
</html>

