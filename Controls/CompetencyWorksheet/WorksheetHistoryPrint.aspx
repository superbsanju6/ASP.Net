<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorksheetHistoryPrint.aspx.cs" Inherits="Thinkgate.Controls.CompetencyWorksheet.WorksheetHistoryPrint" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">



    <title></title>

    <script src="../../Scripts/jquery-1.9.1.js"></script>
    <script src="../../Scripts/jquery-migrate-1.1.0.min.js"></script>
    <script src="../../Scripts/jquery.min.js"></script>
    <script src="../../Scripts/jquery-ui.min.js"></script>
    <script src="../../Scripts/master.js"></script>
    <script src="../../Scripts/EditSubmitResultPagesWithinCustomDialog.js"></script>


    <style>
        @media Print {
            .performance, #Comments {
                padding: 20px 0 0;
            }

            .rgMasterTable .rgClipCells {
                background-color: #537AB8;
            }

            #radGridHistory_ctl00_Header, #radGridComments_ctl00_Header {
               
                table-layout: auto!important;
                margin-left:2px;
                 border-collapse: collapse;
                border-spacing: 0;
                width: 833px!important;
                color: #fff!important;
                background-color: #537AB8!important;
                background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635278870841125132") repeat-x scroll 0 -7050px #537AB8;
                 
            }

            #radGridHistory_ctl00, #radGridComments_ctl00 {
                border: 1px solid #537AB8;
                margin: 0;
                padding: 0;
               border-collapse: separate;
                border-spacing: 0;
                margin-left:2px;
            }
              #radGridHistory_ctl00_Header th, #radGridComments_ctl00_Header th {
                    border: 1px solid #CFD9E7;
                    padding: 4px 0;
                   
                border-bottom:0px none;
                }

                #radGridHistory_ctl00 td, #radGridComments_ctl00 td {
                    border: 1px solid #CFD9E7;
                    padding: 4px;
                }
                #radGridHistory_ctl00 td, #radGridComments_ctl00 td:first-child {
                    border-left: 1px solid #CFD9E7;
                    padding: 4px;
                }
                
        }
    </style>
   


    <script>


        $(document).ready(function () {
            var printContent;
            var printContentHeader;

            //history
            var tblIdheader = window.opener.document.getElementById("radGridHistory").getElementsByTagName("table")[0].id;
            var tblIdcontent = window.opener.document.getElementById("radGridHistory").getElementsByTagName("table")[1].id;
            ////comment
            var tblIdCmntheader = window.opener.document.getElementById("radGridComments").getElementsByTagName("table")[0].id;
            var tblIdCmntcontent = window.opener.document.getElementById("radGridComments").getElementsByTagName("table")[1].id;



            printContentHeader = window.opener.document.getElementById("HeaderPart").outerHTML;
            //history
            var printHistoryContent1 = window.opener.document.getElementById(tblIdheader).outerHTML;
            var printHistoryContent2 = window.opener.document.getElementById(tblIdcontent).outerHTML;
            ////comment
            var printcommentContent1 = window.opener.document.getElementById(tblIdCmntheader).outerHTML;
            var printcommentContent2 = window.opener.document.getElementById(tblIdCmntcontent).outerHTML;

            document.getElementById("divPrint").innerHTML = printContentHeader + window.opener.document.getElementById("Performance").outerHTML + printHistoryContent1 + printHistoryContent2 + window.opener.document.getElementById("Comments").outerHTML + printcommentContent1 + printcommentContent2;
            self.print();
            
        });

    </script>

  

</head>
<body>


    <div id="divPrint">
    </div>

</body>
</html>
