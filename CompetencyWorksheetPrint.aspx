<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompetencyWorksheetPrint.aspx.cs" Inherits="Thinkgate.CompetencyWorksheetPrint" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <link href="Scripts/reset-min.css" rel="stylesheet" />
    <link href="Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/DataTables/css/site_jui.ccss" rel="stylesheet" />
    <link href="Scripts/DataTables/css/demo_table_jui_cw.css" rel="stylesheet" />
    <link href="Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <link href="Scripts/printWorksheet.css" rel="stylesheet" />
     <link href="Scripts/printWorksheet.css" rel="stylesheet" media="print, handheld">
   <!--[if gte IE 8]>

<style type="text/css">

.dataTable, .display {
    width: 1050px !important;
}
       
</style>

<![endif]-->

<style type="text/css">

@media print {
    thead { background:#cccccc !important }
      .backColorLightGreen {
            background-color: #76923c !important;
            color :#ffffff !important;
          
        }
       .backColorGrey {
            background-color: #bfbfbf !important;
        }
  }

</style>

    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.cookie.js"></script>
    <script type="text/javascript" src="Scripts/DataTables/js/jquery.dataTables.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/Custom/addNewDocument.js"></script>
    <script type="text/javascript" src="Scripts/Custom/tgDivTools.js"></script>
    <script type="text/javascript" src="Scripts/master.js"></script>
    <script type="text/javascript" src="Scripts/jquery-migrate-1.1.0.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.js"></script>

    <script>

        $(document).ready(function () {
            var printContent;
            var printContentHeader;

            if (window.opener.$("#SearchCompetencyTitle").text() == "Standard:") {
                printContentHeader = window.opener.$(".dataTables_scrollHeadInner")[0];
                printContent = window.opener.document.getElementById("tblViewByStandard");
            }
            else {
                printContentHeader = window.opener.$(".dataTables_scrollHeadInner")[0];
                printContent = window.opener.document.getElementById("tblViewByStudent");
            }

            document.getElementById("divPrint").innerHTML = printContentHeader.outerHTML + printContent.outerHTML;
            self.print();
        });

    </script>

    <style>
        @media Print {
            .noprint {
                display: none !important;
            }

            a:link:after, a:visited:after {
                display: none;
                content: "";
            }
        }

        #divViewByStandard tr.even td:nth-child(2) {
            width: 15%;
        }

        #divViewByStandard tr.even td:nth-child(3) {
            width: 80%;
        }

        #divViewByStandard tr.odd td:nth-child(2) {
            width: 15%;
        }

        #divViewByStandard tr.odd td:nth-child(3) {
            width: 80%;
        }

        .dataTable tr.odd td:nth-child(2) {
            width: 20%;
        }

        .dataTable tr.odd td:nth-child(3) {
            width: 80%;
        }
        
        .dataTables_scrollHeadInner .dataTable th + th {
            width: 12.5% !important;
        }

            .dataTables_scrollHeadInner .dataTable th + th + th {
                width: 12.3% !important;
            }

                .dataTables_scrollHeadInner .dataTable th + th + th + th {
                    width: 8.2% !important;
                }

                    .dataTables_scrollHeadInner .dataTable th + th + th + th + th {
                        width: 8.2% !important;
                    }

                        .dataTables_scrollHeadInner .dataTable th + th + th + th + th + th {
                            width: 8.2% !important;
                        }

                            .dataTables_scrollHeadInner .dataTable th + th + th + th + th + th + th {
                                width: 8.2% !important;
                            }

                                .dataTables_scrollHeadInner .dataTable th + th + th + th + th + th + th + th {
                                    width: 8.2% !important;
                                }

                                    .dataTables_scrollHeadInner .dataTable th + th + th + th + th + th + th + th + th {
                                        width: 8.2% !important;
                                    }

                                        .dataTables_scrollHeadInner .dataTable th + th + th + th + th + th + th + th + th + th {
                                            width: 8.2% !important;
                                        }

        .dataTables_scrollHeadInner .dataTable th + th {
            width: 12.3% !important;
        }

            .dataTables_scrollHeadInner .dataTable th + th + th {
                width: 12.3% !important;
            }

                .dataTables_scrollHeadInner .dataTable th + th + th + th {
                    width: 8.2% !important;
                }

                    .dataTables_scrollHeadInner .dataTable th + th + th + th + th {
                        width: 8.2% !important;
                    }

                        .dataTables_scrollHeadInner .dataTable th + th + th + th + th + th {
                            width: 8.2% !important;
                        }

                            .dataTables_scrollHeadInner .dataTable th + th + th + th + th + th + th {
                                width: 8.2% !important;
                            }

                                .dataTables_scrollHeadInner .dataTable th + th + th + th + th + th + th + th {
                                    width: 8.2% !important;
                                }

                                    .dataTables_scrollHeadInner .dataTable th + th + th + th + th + th + th + th + th {
                                        width: 8.2% !important;
                                    }

                                        .dataTables_scrollHeadInner .dataTable th + th + th + th + th + th + th + th + th + th {
                                            width: 9.2% !important;
                                        }

        .dataTables_scrollHead {
            width: 1289px !important;
            border-right: 1px solid #000000 !important;
        }

    </style>    
<!--[if !IE]><!-->
    <style type="text/css">
    .dataTable, .display {
        width: 1150px !important;
        }       
</style>
 <!--<![endif]-->
<style type="text/css">
    /* Firefox */
        @-moz-document url-prefix() {
            .dataTable, .display {
                width: 1300px !important;
            }
        }
     /* Safari */
    @media screen and (-webkit-min-device-pixel-ratio:0) {
        .dataTable, .display {
            width: 1150px !important;
        }
    }  
</style>

</head>
<body>
    <div id="divPrint">
    </div>
</body>
</html>
