<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BalancedScorecardPage.aspx.cs" Inherits="Thinkgate.Controls.Reports.BalancedScorecardPage" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js"></script>
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>
<script type="text/javascript" src="http://code.jquery.com/jquery-1.8.2.js"></script>
<script src="http://code.jquery.com/jquery-1.9.0.js"></script>
<script src="http://code.jquery.com/jquery-migrate-1.0.0.js"></script>
<script type="text/javascript" src="jquery.print.js"></script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Balanced Scorecard</title>
    <link href="~/Styles/Site.css?v=2" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #rptViewer_ctl09 {
            background-color: #e9e9ff !important;
            height: 650px !important;
        }

        #rptViewer_fixedTable {
            width: 1350px !important;
            margin-left: 35px !important;
        }

        #reportTitle {
            font-family: Tahoma;
            font-size: 12pt;
            font-style: normal;
            font-weight: 700;
            white-space: pre-wrap;
            direction: ltr;
            text-align: center;
            width: 1350px !important;
            margin-left: 35px !important;
            margin-top: -50px !important;
        }
    </style>

    <script lang="javascript">
        $.noConflict();
        var browserIE;
        $(document).ready(function () {
            if ($.browser.msie && $.browser.version > 6) {
                browserIE = true;
            }
            else {
                browserIE = false;
            }

        });
        function PrintMasterReport() {
            if (browserIE == true) {
                var rptViewerReference = $find("rptViewerMasterReport");
                var isLoading = rptViewerReference.get_isLoading();

                if (!isLoading) {
                    var reportAreaContent = rptViewerReference.get_reportAreaContentType();
                    if (reportAreaContent == Microsoft.Reporting.WebFormsClient.ReportAreaContent.ReportPage) {
                        $find("rptViewerMasterReport").invokePrintDialog();
                    }
                }
            }
            else {

                var newstr = document.getElementById("VisibleReportContentrptViewer_ctl09").innerHTML;
                var oldstr = document.getElementById("body1").innerHTML;
                $('#GoalBar').hide();
                document.getElementById("body1").innerHTML = newstr
                window.print();
                document.getElementById("body1").innerHTML = oldstr;
                $('#GoalBar').show();
                return false;
            }
        }
    </script>
</head>
<body class="default" >
    <form id="form1" runat="server">
        <table>
            <tr>
                <td>
                    <img id="GoalBar" src="../../Images/tiles/tile_header_glass.png" style="width: 1350px; height: 50px; margin-left: 35px; margin-top: 10px;" />
                    <div id="reportTitle">Balanced Scorecard Report</div>
                    <div style="width: 1350px; margin-left: 35px;" >
                        Select Goal:
                        <telerik:RadComboBox ID="cmbGoal" runat="server" ToolTip="Change Goal"
                            Skin="Web20" AutoPostBack="true" OnSelectedIndexChanged="cmbGridDisplay_SelectedIndexChanged" Style="width: 160px; margin-top: 5px;">
                        </telerik:RadComboBox>
                        <div style="width: 65px; margin-top: -30px; margin-left: 1285px; height: 30px;">
                            <asp:ImageButton ID="btnExportExcel" runat="server" ToolTip="Export All Goals" OnClick="btnExportExcel_Click" ImageUrl="~/Images/Toolbars/excel_button.png" />
                            <img id="btnPrintMasterReport" title="Print All Goals" style="cursor: pointer;" onclick="PrintMasterReport();" src="../../Images/Toolbars/print.png" />
                        </div>
                    </div>
                    <div id="body1">
                    <div>
                        <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
                        <rsweb:ReportViewer ID="rptViewer" ClientIDMode="Static" runat="server" maxwidth="800px" maxheight="800px"></rsweb:ReportViewer>
                        <input id="reportServerUrl" type="hidden" runat="server" />
                        <input id="reportPath" type="hidden" runat="server" />
                        <input id="reportUsername" type="hidden" runat="server" />
                        <input id="reportPassword" type="hidden" runat="server" />
                        <input id="reportDomain" type="hidden" runat="server" />
                    </div>
                    <div style="display: none">
                        <rsweb:ReportViewer ID="rptViewerMasterReport" ClientIDMode="Static" runat="server" maxwidth="800px" maxheight="800px"></rsweb:ReportViewer>
                    </div>
                        </div>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
