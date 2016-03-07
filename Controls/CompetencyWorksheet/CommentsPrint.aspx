<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommentsPrint.aspx.cs" Inherits="Thinkgate.Controls.CompetencyWorksheet.CommentsPrint" %>

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
        #radGridComments_ctl00_Header {
               table-layout: auto!important;
               border-collapse: separate;
                border-spacing: 0;
                margin-left:2px;
                width: 842px!important;
                height:24px!important;
                color: #fff!important;
                background-color: #537AB8!important;
                background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635278870841125132") repeat-x scroll 0 -7050px #537AB8;
        }
        #radGridComments_ctl00_Header th {
                    border: 1px solid #CFD9E7;
                    padding: 4px 0;
                     
                border-bottom:0px none;
                }

         #radGridComments_ctl00 {
                 border: 1px solid #537AB8;
                margin: 0;
                padding: 0;
               border-collapse: separate;
                border-spacing: 0;
                margin-left:2px;
                margin-top:-23px;
            }
              

                #radGridComments_ctl00 td {
                    border: 1px solid #CFD9E7;
                    padding: 4px;
                }
        #HeaderPart {padding-bottom:20px;
        }
       
        }

    </style>

   

    <script>


        $(document).ready(function () {
          
            var printContent;
            var printContentHeader;
            var tbl = $('#gridComment').find('table').attr("id");
            var len = $('#gridComment').find('table').length;
            var tblIdheader = window.opener.document.getElementById("gridComment").getElementsByTagName("table")[0].id;
            var tblIdcontent = window.opener.document.getElementById("gridComment").getElementsByTagName("table")[1].id;
            var tblIdpager = window.opener.document.getElementById("gridComment").getElementsByTagName("table")[2].id;
            printContentHeader = window.opener.document.getElementById("HeaderPart").outerHTML;
            var printHeader = window.opener.document.getElementById(tblIdheader).outerHTML;
            var printContent = window.opener.document.getElementById(tblIdcontent).outerHTML;
            var printPager = window.opener.document.getElementById(tblIdpager).outerHTML;
            document.getElementById("divPrint").innerHTML = printContentHeader + printHeader + printContent + printPager;
            self.print();
           
        });

    </script>

    

</head>
<body>
   

    <div id="divPrint">
        
    </div>
   
</body>
</html>
