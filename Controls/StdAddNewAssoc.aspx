<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StdAddNewAssoc.aspx.cs" Inherits="Thinkgate.Controls.StdAddNewAssoc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title></title>

    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <script type="text/javascript" src="../CMSScripts/Custom/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../CMSScripts/Custom/jquery-migrate-1.1.0.min.js"></script>
    <script type="text/javascript" src="../CMSScripts/Custom/jquery-core.js"></script>
    <script type="text/javascript" src="../CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script>
    <script type="text/javascript" src="../CMSScripts/Custom/jquery-cookie.js"></script>
    <script type="text/javascript" src="../CMSScripts/Custom/jquery.dataTables.js"></script>
    <script type="text/javascript" src="../CMSScripts/Custom/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="../CMSScripts/Custom/addNewDocument.js"></script>
    <script type="text/javascript" src="../CMSScripts/Custom/tgDivTools.js"></script>

    <script>var $j = jQuery.noConflict();</script>

    <link href="../CMSScripts/Custom/reset-min.css" rel="stylesheet" />
    <link href="../CMSScripts/Custom/site_jui.ccss" rel="stylesheet" />
    <link href="../CMSScripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
    <link href="../CMSScripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <link href="../CMSScripts/Custom/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../CMSScripts/CSS/tgwebparts.css" rel="stylesheet" />
    <link href="../CMSScripts/CSS/associationToolbar.css" rel="stylesheet" />

    <style type="text/css">
        select {
            width: 195px;
        }

        .tgLabel {
            text-align: left;
        }
    </style>
</head>
<body>
    <div class="css_clear"></div>
    <form id="frmStandardsAddNewAssoc" runat="server">
        <asp:ScriptManager ID="Scriptmanager1" runat="server"></asp:ScriptManager>
        <asp:HiddenField ID="SelectedItems" runat="server" />
        <asp:HiddenField ID="DocID" runat="server" />

        <div id="tabs">



            <div style="padding: 0px;height: 430px;  width: 95%; position: relative; padding-left: 5px;">

                <div class="ui-widget" id="buttonBarDiv2" style="display: block">
                    <span style="float: left; padding-right: 1em;">
                        <label class="tgLabel">Standard: </label>
                        <div id="StandardSetDdldiv">
                            <select id="StandardSetDdl" onchange="getStandardSetList();"></select>
                        </div>
                    </span>
                    <span style="float: left; padding-right: 1em;">
                        <label class="tgLabel">Grade: </label>
                        <div id="GradeDdldiv">
                            <select id="GradeDdl" onchange="getSubjectList();"></select>
                        </div>
                        </span>
                        <span style="float: left; padding-right: 1em;">
                            <label class="tgLabel">Subject: </label>
                            <div id="SubjectDdldiv">
                                <select id="SubjectDdl" onchange="getCourseList();"></select>
                            </div>
                        </span>
                         <span style="float: left; padding-right: 1em;">
                            <label class="tgLabel">Course: </label>
                            <div id="CourseDdldiv">
                                <select id="CourseDdl"></select>
                            </div>
                        </span>
                       
                    </div>
                    <br />
                    <br />
                    &nbsp;
                        <div class="ui-widget" id="Div1" style="display: block">
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">Text Search: </label>
                                <div id="TextSearchTxtdiv">
                                    <input id="TextSearch" type="text" value="" style="width: 165px;" />
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">&nbsp;</label>
                                <div id="TextSearchOptionsDdldiv">
                                    <select id="TextSearchOptionsDdl" style="width: 140px;">
                                        <option value="any">Any Words</option>
                                        <option value="all">All Words</option>
                                        <option value="exact">Exact Phrase</option>
                                    </select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">&nbsp;&nbsp;</label>
                                <div id="SearchResourcesButton" style="padding-left: 8px; vertical-align: bottom;">
                                    <input id="btnSearchResources" type="image" src="../Images/search-1.png" onclick="getStandardsDataTable(); return false;" />
                                </div>
                            </span>
                        </div>



                <br />

                <div id="showStandardsModal2" title="" style="display: block">
                    <div id="showStandardsModal-dialog-content2"></div>
                    <br />
                    <table id="availableStandardsDataTable" border="0" class="display" style="width: 95%"></table>
                </div>

                <div id="LinkButtonsDiv2" class="pull-left" style="position: relative; top: 5px;">
                    <asp:LinkButton ID="btnAddNewStandards" runat="server" OnClientClick="return attachStandardsToTagEA();" CssClass="btn btn-success"><i class="icon-share icon-white"></i>&nbsp;Select and Return</asp:LinkButton>
                    <asp:LinkButton ID="btnCloseDialog" runat="server" CssClass="btn btn-danger" OnClientClick="return closeDialog();return false;"><i class="icon-remove icon-white"></i>&nbsp;Cancel</asp:LinkButton>
                </div>
            </div>
        </div>
    </form>

    <script>

        function closeDialog() {
            var frame = top.findFrameWindowByUrl("ResourceSearchKentico.aspx");
            if (frame && frame.CloseDialog) {
                frame.CloseDialog();
            }
            return false;
        }

        function attachStandardsToTagEA() {
            var selectedItems = $j('input[name=SelectedItems]').val();
            var ids = '', names = '';
            $j(document).find(".row_selected").each(function (i, e) {
                ids = ids + e.cells[0].innerText + '|';
                names = names + e.cells[1].innerText + '<BR/>';
            });
            parent.closeModalDialog(ids, names);
            window.parent.CloseDialog();
            return false;

        }
        function getStandardsDataTable() {
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

            $j.ajax({
                type: "POST",
                url: "../Services/KenticoServices/KenticoWebServices.aspx/getStandardsList",
                data: "{'standardSet':'" + stdset + "', 'standardSetVal':'" + stdsetVal + "', 'grade':'" + grade + "', 'gradeVal':'" + gradeVal + "', 'subject':'" + subject + "', 'subjectVal':'" + subjectVal + "', 'course':'" + course + "', 'courseVal':'" + courseVal + "', 'searchOption':'" + searchOption + "', 'searchText':'" + searchText + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
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

        function addStandards() {
            // var fromLRMI = '<%=this.FromLRMI %>';
	        var selectedItems = $j('input[name=SelectedItems]').val();
	        //if (fromLRMI == "false") {
	        var ids = '', names = '';
	        $j(document).find(".row_selected").each(function (i, e) {
	            ids = ids + e.cells[0].innerText + '|';
	            names = names + e.cells[1].innerText + '<BR/>';
	        });
	        parent.closeModalDialog(ids, names);
	        window.parent.CloseDialog();
	        return false;
	        //}
	    }


	    function getCourseList() {
	        var stdset = $j('#StandardSetDdl').find(':selected').text();
	        var grade = $j('#GradeDdl').find(':selected').text();
	        var subject = $j('#SubjectDdl').find(':selected').text();
	        $j.ajax({
	            type: "POST",
	            url: "../Services/KenticoServices/KenticoWebServices.aspx/getStandardSetGradeSubjectCourse",
	            data: "{'standardSet':'" + stdset + "', 'grade':'" + grade + "', 'subject':'" + subject + "'}",

	            contentType: "application/json; charset=utf-8",
	            dataType: "json",

	            error: function (XMLHttpRequest, textStatus, errorThrown) {
	                //alert(textStatus + "\n" + errorThrown);
	            },
	            success: function (result) {
	                $j('#CourseDdldiv')[0].innerHTML = "<select id='CourseDdl'>" + result.d + "</select>";
	            }
	        });
	    }

	    function getStandardsList() {
	        $j.ajax({
	            type: "POST",
	            url: "../Services/KenticoServices/KenticoWebServices.aspx/getStandardSetList",
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


	    function getSubjectList() {
	        var stdset = $j('#StandardSetDdl').find(':selected').text();
	        var grade = $j('#GradeDdl').find(':selected').text();
	        $j.ajax({
	            type: "POST",
	            url: "../Services/KenticoServices/KenticoWebServices.aspx/getStandardSetGradeSubject",
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


	    function getStandardSetList() {
	        var stdset = $j('#StandardSetDdl').find(':selected').text();
	        $j.ajax({
	            type: "POST",
	            url: "../Services/KenticoServices/KenticoWebServices.aspx/getStandardSetGrade",
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

	    function updateAvailableStandardsDataTable(jsondata) {
	        try {
	            if (typeof oTable2 == 'undefined') {
	                oTable2 = $j('#availableStandardsDataTable').dataTable({
	                    //"iDisplayLength": 100,
	                    "bJQueryUI": true,
	                    "bPaginate": false,
	                    "bLengthChange": false,
	                    "sScrollY": 210,
	                    "aaSorting": [[0, "asc"]],
	                    "aaData": jsondata,
	                    "aoColumns":
					[
						//ID, Standard_Set, Grade, Subject, Course, Level, StandardName, \"Desc\" as Description
						{ "sTitle": "ID", "mData": "ID" },
						{ "sTitle": "Standard", "mData": "StandardName" },
						{ "sTitle": "Description", "mData": "Description" }
					]
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

	        tgMakeTableSelectable('availableStandardsDataTable');

	    }

	    $j(document).ready(function () {
	        getStandardsList();
	        updateAvailableStandardsDataTable({});
	        $j(".dataTables_filter").css('display', 'none');
	    });

    </script>
</body>
</html>
