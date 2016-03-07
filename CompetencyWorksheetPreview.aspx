<%@ Page Title="" Language="C#" MasterPageFile="~/CompetencyWorksheet.Master"
    AutoEventWireup="true" CodeBehind="CompetencyWorksheetPreview.aspx.cs"
    Inherits="Thinkgate.CompetencyWorksheetPreview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">

    <link href="Scripts/reset-min.css" rel="stylesheet" />
    <link href="Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/DataTables/css/site_jui.ccss" rel="stylesheet" />
    <link href="Scripts/DataTables/css/demo_table_jui_cw.css" rel="stylesheet" />
    <link href="Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <link href="Styles/CompetencyWorksheets/CompetencyWorksheets.css" rel="stylesheet"/>

    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.cookie.js"></script>
    <script type="text/javascript" src="Scripts/DataTables/js/jquery.dataTables.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/Custom/addNewDocument.js"></script>
    <script type="text/javascript" src="Scripts/Custom/tgDivTools.js"></script>
    <script type="text/javascript" src="Scripts/master.js"></script>
    <script type="text/javascript" src="Scripts/jquery-migrate-1.1.0.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.js"></script>
    <script type="text/javascript" src="Scripts/DataTables/js/TableTools.js"></script>
    <script src="Scripts/DataTables/js/ZeroClipboard.js"></script>
    
    <script type="text/javascript">

        var WorksheetID;
        var ClassID;

        $(document).ready(function() {
            WorksheetID = '<%= WorksheetID %>';
            ClassID = '<%= ClassID %>';

            $(document).on("contextmenu", function (e) {                                
                    e.preventDefault();
            });
        });

    </script>

    <script type="text/javascript" src="Scripts/CompetencyWorksheets/CompetencyWorksheetsViewModel.js"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <!--[if gte IE 8]>

<style type="text/css">

.dataTable, .display {
    width: 1050px !important;
}
       
</style>

<![endif]-->

    <!--[if !IE]><!-->
    <style type="text/css">
    .dataTable, .display {
        width: 1150px !important;
        }       
</style>
 <!--<![endif]-->

    <%-- For IE9,10 specific CSS--%>
    <!--[if IE ]><!-->
    <style type="text/css">
            #tblViewByStandard table td {
                padding: 0px !important;
                margin: 0px !important;
            }

            #tblViewByStandard table td:nth-child(1) {
                width: 30.2% !important;
            }

            #tblViewByStandard table td:nth-child(2) {
                width: 9.95% !important;
            }

            #tblViewByStandard table td:nth-child(3) {
                width: 10% !important;
            }

            #tblViewByStandard table td:nth-child(4) {
                width: 9.95% !important;
            }

            #tblViewByStandard table td:nth-child(5) {
                width: 9.95% !important;
            }

            #tblViewByStandard table td:nth-child(6) {
                width: 10% !important;
            }

            #tblViewByStandard table td:nth-child(7) {
                width: 10% !important;
            }

            #tblViewByStandard table td:nth-child(8) {
                width: 10% !important;
            }
              #tblViewByStudent table{
            width: 100% !important;
            }
            #tblViewByStudent table td {
                padding: 0px !important;
                margin: 0px !important;
            }

           #tblViewByStudent table td:nth-child(1) {
                width: 14.75% !important;
            }

            #tblViewByStudent table td:nth-child(2) {
                width: 15.15% !important;
            }

            #tblViewByStudent table td:nth-child(3) {
                width: 9.90% !important;
            }

            #tblViewByStudent table td:nth-child(4) {
                width: 9.90% !important;
            }

            #tblViewByStudent table td:nth-child(5) {
                width: 9.95% !important;
            }

            #tblViewByStudent table td:nth-child(6) {
                width: 9.80% !important;
            }

            #tblViewByStudent table td:nth-child(7) {
                width: 9.95% !important;
            }

            #tblViewByStudent table td:nth-child(8) {
                width: 10% !important;
            }
            
    </style>
    <!--<![endif]-->

    <%-- For firefox specific CSS--%>
    <style type="text/css">
        @-moz-document url-prefix() {
            #tblViewByStandard table{
            width: 100% !important;
            }
            #tblViewByStandard table td {
                padding: 0px !important;
                margin: 0px !important;
            }

            #tblViewByStandard table td:nth-child(1) {
                width: 30.05% !important;
            }

            #tblViewByStandard table td:nth-child(2) {
                width: 9.95% !important;
            }

            #tblViewByStandard table td:nth-child(3) {
                width: 10.05% !important;
            }

            #tblViewByStandard table td:nth-child(4) {
                width: 9.95% !important;
            }

            #tblViewByStandard table td:nth-child(5) {
                width: 10.10% !important;
            }

            #tblViewByStandard table td:nth-child(6) {
                width: 9.95% !important;
            }

            #tblViewByStandard table td:nth-child(7) {
                width: 10% !important;
            }

            #tblViewByStandard table td:nth-child(8) {
                width: 10% !important;
            }

            #tblViewByStudent table {
                width: 100% !important;
            }  
             #tblViewByStudent table table {
               border:1px solid red !important;
            }

            #tblViewByStudent table td {
                padding: 0px !important;
                margin: 0px !important; 
                 width: 100% !important;
            }

            #tblViewByStudent table td:nth-child(1) {
                width: 14.85% !important;
            }

            #tblViewByStudent table td:nth-child(2) {
                width: 15.15% !important;
            }

            #tblViewByStudent table td:nth-child(3) {
                width: 9.95% !important;
            }

            #tblViewByStudent table td:nth-child(4) {
                width: 9.95% !important;
            }

            #tblViewByStudent table td:nth-child(5) {
                width: 9.95% !important;
            }

            #tblViewByStudent table td:nth-child(6) {
                width: 10.1% !important;
            }

            #tblViewByStudent table td:nth-child(7) {
                width: 9.95% !important;
            }

            #tblViewByStudent table td:nth-child(8) {
                width: 10% !important;
            }

        }
    </style>

    <%-- For Google crome specific CSS--%>
    <style type="text/css">
        @media screen and (-webkit-min-device-pixel-ratio:0) {
                    #tblViewByStandard table {
                        width: 100% !important;
                    }

                    #tblViewByStandard table td {
                        padding: 0px !important;
                        margin: 0px !important;
                    }

                    #tblViewByStandard table td:nth-child(1) {
                        width: 29.90% !important;
                    }

                    #tblViewByStandard table td:nth-child(2) {
                        width: 10% !important;
                    }

                    #tblViewByStandard table td:nth-child(3) {
                        width: 10.05% !important;
                    }

                    #tblViewByStandard table td:nth-child(4) {
                        width: 10% !important;
                    }

                    #tblViewByStandard table td:nth-child(5) {
                        width: 10% !important;
                    }

                    #tblViewByStandard table td:nth-child(6) {
                        width: 10% !important;
                    }

                    #tblViewByStandard table td:nth-child(7) {
                        width: 10% !important;
                    }

                    #tblViewByStandard table td:nth-child(8) {
                        width: 10% !important;
                    }

                    #tblViewByStudent table {
                        width: 100% !important;
                    }

                    #tblViewByStudent table td {
                        padding: 0px !important;
                        margin: 0px !important;
                    }
                    #tblViewByStudent table td:nth-child(1) {
                        width: 14.92% !important;
                    }

                    #tblViewByStudent table td:nth-child(2) {
                        width: 15.15% !important;
                    }

                    #tblViewByStudent table td:nth-child(3) {
                        width: 9.95% !important;
                    }

                    #tblViewByStudent table td:nth-child(4) {
                        width: 9.95% !important;
                    }

                    #tblViewByStudent table td:nth-child(5) {
                        width: 9.95% !important;
                    }

                    #tblViewByStudent table td:nth-child(6) {
                        width: 10.1% !important;
                    }

                    #tblViewByStudent table td:nth-child(7) {
                        width: 9.95% !important;
                    }

                    #tblViewByStudent table td:nth-child(8) {
                        width: 10% !important;
                    }
                }
    </style>

    <div id="divCommentDialog" title="Comments" style="display: none;">
        <iframe id="divCommentDialogframe" src="" frameborder="0" width="900" height="550">No frames</iframe>
    </div>

    <div id="divHistoryDialog" title="History" style="display: none;">
        <iframe id="divHistoryDialogframe" src="" frameborder="0" width="900" height="550">No frames</iframe>
    </div>

    <div id="divCWEditDialog" title="Edit Identification" style="display: none;">
        <iframe id="divCWEditDialogframe" src="" frameborder="0" width="570" height="400">No frames</iframe>
    </div>

    <div id="divCWCopyDialog" title="Copy Identification" style="display: none;">
        <iframe id="divCWCopyDialogframe" src="" frameborder="0" width="570" height="400">No frames</iframe>
    </div>

    <div id="divCWAddRemoveCompetenciesDialog" title="Add/Remove Competencies" style="display: none;">
        <iframe id="divCWAddRemoveCompetenciesframe" src="" frameborder="0" width="1020" height="640">No frames</iframe>
    </div>

    <table class="layouotDiamension layouotDiamension1 ">
        <tr>
            <td>
                <input type="button" value="Save" id="btnSave" class="navButton btnSave" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" PostBack="false" OnClientClick="CloseConfirmation();return false;" CssClass="navButton btnCancel" />
                <asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="navButton btnPrint" OnClientClick="return PrintWorksheet();" />

                <input type="button" value="Edit" id="btnEdit" class="navButton btnEdit" />
                <input type="button" value="Copy" id="btnCopy" class="navButton btnCopy" />
                <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClientClick="DeleteWorksheet(); return false;" CssClass="navButton btnDelete" />
            </td>
           <td class="floatRightMenu">
                <div>
                   <div class="floatLeftMenuText">View By:</div>
                   <select class="floatRight "id="ddlView">
                       <option value="Student" selected="selected">Student</option>
                       <option value="Standard">Standard</option>
                   </select>
                    </div>
            </td>
        </tr>
        <tr>
            <td><strong>Description:  </strong><span id="wDesc" style="width: 200px; word-wrap: break-word;"></span></td>
            <td class="floatRightMenu">
                <div>
                <div class="floatLeftMenuText" id="SearchCompetencyTitle">Student:</div>
                <select class="floatRight" id="ddlCompetency">
                    <option value="">All Students</option>
                </select>
                </div>
            </td>
        </tr>
    </table>
    <div>
        <div id="divViewByStandard" class="layouotDiamension marTop40px">
            <div id="showWorksheetModel" title="">
                <div id="showWorksheetModel-dialog-content2"></div>
                <br />
                <div class="test">
                    <div class="floatLeft">
                        <input type="button" value="Add/Remove Competencies" id="btnAddRemoveCompetencies" class="navButton btnAdd" />
                    </div>

                    <div class="floatRight">
                        <table>
                            <tr>
                                <td>
                                    <div class="greenBox"></div>
                                    This Worksheet</td>
                                <td>
                                    <div class="voiletBox"></div>
                                    Previous Worksheet(s)</td>
                            </tr>
                        </table>
                    </div>
                </div>

                <table id="tblViewByStandard" border="0" class="display" style="width: 100%; border-collapse: collapse;" cellpadding="0" cellspancing="0">
                </table>

            </div>
        </div>

        <div id="divViewByStudent" class="layouotDiamension marTop40px" style="display: none">
            <div id="showWorksheetModelByStudent" title="">
                <br />
                <div class="test">
                    <div class="floatLeft">
                        <input type="button" value="Add/Remove Competencies" id="btnAddRemoveCompetenciesByStudent" class="navButton btnAdd" />
                    </div>
                    <div class="floatRight">
                        <table>
                            <tr>
                                <td>
                                    <div class="greenBox"></div>
                                    This Worksheet</td>
                                <td>
                                    <div class="voiletBox"></div>
                                    Previous Worksheet(s)</td>
                            </tr>
                        </table>
                    </div>
                </div>
                <table id="tblViewByStudent" border="0" class="display" style="width: 100%; border-collapse: collapse;" cellpadding="0" cellspancing="0">
                </table>
            </div>

        </div>
    </div>
    <div id="tbldiv" class="modal"></div>
    <div id="dialog-message" title="Worksheet Copy Successful" style="display: none">
        <p>
            This is a copy of the previously viewed worksheet.
        </p>
    </div>
</asp:Content>
