<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Dialogues/Assessment/AssessmentDialog.Master" CodeBehind="AssessmentSummary.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.AssessmentSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        body 
        {
            font-family: Sans-Serif, Arial;
            font-weight: bold;
            position: relative;
            font-size: .8em;
        }
        
        .containerDiv 
        {
            width: 99%;
        }
        
        .headerDiv 
        {
            margin-bottom: 50px;
            margin-left: 37px;
        }
        
        p 
        {
            font-weight: normal;
        }
        
        .formContainer 
        {
            width: 95%;
            text-align: left;
            margin-top: 10px;
        }
        
        .headerImg 
        {
            position: absolute;
            left: 10;
            top: 10;
            width: 30px;
        }
        
        .roundButtons 
        {
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            border: solid 1px #000;
            border-radius: 50px;
            float: right;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
        }

        .roundButtons_blue {
            color: #FFF;
            background-color: #36C;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            position: relative;
            border: solid 1px #000;
            border-radius: 50px;
            float: right;
            margin-left: 10px;
            cursor: pointer;
        }
        
        .imgButtons 
        {
            padding: 2px;
            display: inline;
            float: left;
            margin-left: 10px;
            cursor: pointer;
        }
        
        .headerTD 
        {
            font-weight: bold;
            padding: 3px;
            background-color: #9CF;
        }
        
        .headerTD2
        {
            font-weight: bold;
            padding: 3px;
        }
        
        #standardsTableHeader th {
            font-weight: bold;
            border-bottom: solid 1px #000;
        }

        #addendumTableHeader th {
            font-weight: bold;
            border-bottom: solid 1px #000;
        }
        
        #itemBankTableHeader th {
            font-weight: bold;
            border-bottom: solid 1px #000;
        }
        
        #rigorTable 
        {
            border: solid 2px #000;
            border-collapse: collapse;
            width: 90%;
        }
        
        #rigorTable td 
        {
            border: solid 1px #000;
        }
        
        .tableScroll {
            max-height: 85px;
            height:100%;
            overflow: auto;
        }

        .tableScrollAddendum {
            max-height: 150px;
            height:100%;
            overflow: auto;
        }
        
        .subHeader 
        {
            font-weight: bold;
            color: #36F;
            font-style: italic;
            width: 20%;
            padding: 3px;
            text-align: center;
            vertical-align: bottom;
        }
        
        .contentLabel 
        {
            font-weight: bold;
            padding: 3px;
            text-align: left;          
        }
        
        .contentLabel a
        {
            white-space: nowrap;                   
            overflow: hidden;  /* "overflow" value must be different from "visible" */
            text-overflow:    ellipsis;
            display:block;
            width:110px;
        }
        .contentDistribution 
        {
            /*text-align: left;*/
            font-weight: normal;
            /*padding-left: 8%;*/
            text-align: center;
        }
        
        .contentElement 
        {
            font-weight: bold;
            width: 10%;
            text-align: center;
            padding: 3px;
        }
        
        .contentElementInactive 
        {
            background-color: #C0C0C0;
            text-align: center;
            font-weight: normal;
        }
        
        .standardContentElement 
        {
            font-weight: bold;
            text-align: center;
            padding: 3px;
        }
        
        .tableContainer 
        {
            padding-bottom: 15px;
        }
        
        #distributionTotal
        {
            font-weight: bold;
            font-style: normal;
            color: #000;
            vertical-align: middle;
            text-align: center;
        }
        
        .summaryCriteriaLabels 
        {
            float: left;
        }
        
        .summaryCriteriaFields
        {
            float: left;
            margin-left: 20px;
            margin-right: 50px;
            color: #888;
        }
        
        .tableLabel 
        {
            margin-bottom: 5px;
            padding-left: 35px;
        }
        
        .summaryLabel a
        {
            margin-left: -71px;
            margin-right: 27px;
            font-weight: bold;
        }
        .headerTable {
        border-bottom:1px solid;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <span runat="server" id="summaryContent" style="padding-bottom:35px;">
        <img runat="server" id="headerImg" src="../../Images/repairtool.png" alt="Create assessment icon" ClientIDMode="Static" style="position: absolute;left: 10;top: 10;width: 30px;" />
        <div class="containerDiv" align="right" style="width: 99%;">
            <div class="formContainer" runat="server" id="formContainerDiv">
                <div valign="top" style="margin-bottom: 50px;margin-left: 37px;">
                    <div runat="server" id="assessmentTitle" style="padding-top: 10px;padding-bottom: 50px;text-align: center;font-size: 11pt;" ClientIDMode="Static"></div>
                    <div style="float: left;">
                        <p style="font-weight: normal;">Score Type:</p>
                        <p style="font-weight: normal;">Content Type:</p>
                    </div>
                    <div style="float: left;margin-left: 20px;margin-right: 50px;color: #888;">
                        <p style="font-weight: normal;">Percent</p>
                        <p style="font-weight: normal;" runat="server" id="contentType" ClientIDMode="Static">ItemBank</p>
                    </div>
                    <div style="float: left;">
                        <p style="font-weight: normal;">Number of Forms:</p>
                        <p style="font-weight: normal;" id="includeFieldTestLabel" ClientIDMode="Static" runat="server">Include Field Test:</p>
                        <p style="font-weight: normal;" id="numberOfAddendumsLabel_dup" ClientIDMode="Static" runat="server" Visible="false">Number of Addendums:</p>
                    </div>
                    <div style="float: left;margin-left: 20px;margin-right: 50px;color: #888;">
                        <p style="font-weight: normal;">1</p>
                        <p style="font-weight: normal;" id="includeFieldTestValue" ClientIDMode="Static" runat="server"></p>
                        <p style="font-weight: normal;" runat="server" id="addendumCount_dup" ClientIDMode="Static" Visible="false"></p>
                    </div>
                    <div runat="server" id="rubricAndAddendumLabels" objectScreen="assessment" style="float: left;display:none;">
                        <p style="font-weight: normal;">Number of Rubrics:</p>
                        <p style="font-weight: normal;" id="numberOfAddendumsLabel_orig" ClientIDMode="Static" runat="server">Number of Addendums:</p>
                    </div>
                    <div runat="server" id="rubricAndAddendumCountValues" style="float: left;margin-left: 20px;margin-right: 50px;color: #888; display:none;" objectScreen="assessment">
                        <p style="font-weight: normal;" runat="server" id="rubricCount" ClientIDMode="Static"></p>
                        <p style="font-weight: normal;" runat="server" id="addendumCount" ClientIDMode="Static"></p>
                    </div>
                </div>

                <div class="tableContainer" align="left" style="padding-bottom: 15px;">
                    <div runat="server" id="distributionContainerDiv" style="width:95%; height:110px;">
                        <span runat="server" id="spanBreak" Visible="false"><br/><br/></span>
                            <table border="0">
                               <tr>
                                <td valign="top">
                                    <p runat="server" style="font-weight: normal;" class="tableLabel" id="standardDistLabel">Item/Standard Distribution:</p>
                                    <div runat="server" id="standardsTableContainerDiv" style="width:300px; border:solid 2px #000; margin-left:37px;">
                                        <asp:Table runat="server" ID="standardsTableHeader" ClientIDMode="Static" BorderWidth="0" CssClass="headerTable" Width="100%" CellPadding="2" CellSpacing="0" GridLines="Both">
                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell Width="110" CssClass="contentLabel" HorizontalAlign="Left">Standard</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="128" CssClass="contentLabel" HorizontalAlign="Left">%&nbsp;of&nbsp;Assessment</asp:TableHeaderCell>
                                                <asp:TableHeaderCell CssClass="contentLabel">Items</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>                                            
                                        </asp:Table>
                                        <div class="tableScroll" runat="server" id="standardDistScrollContainerDiv">
                                            <asp:Table runat="server" ID="standardTable" ClientIDMode="Static" CellPadding="2" CellSpacing="0" GridLines="Both" BorderWidth="0" Width="100%">
                                            </asp:Table>
                                        </div>
                                    </div>
                                </td>
                                <td valign="top">
                                    <p class="tableLabel" runat="server" id="itemBankLabel" style="font-weight: normal; display:none;" objectScreen="assessment">Item Bank Distribution:</p>
                                    <div runat="server" id="itemBankTableContainerDiv" style="width:300px; border:solid 2px #000; margin-left:37px; display:none;" objectScreen="assessment">
                                        <asp:Table runat="server" ID="itemBankTableHeader" ClientIDMode="Static" BorderWidth="0" Width="100%" CssClass="headerTable" CellPadding="2" CellSpacing="0" GridLines="Both">
                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell Width="110" CssClass="contentLabel">Item Bank</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="128" CssClass="contentLabel">%&nbsp;of&nbsp;Assessment</asp:TableHeaderCell>
                                                <asp:TableHeaderCell CssClass="contentLabel">Items</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>
                                        </asp:Table>
                                        <div class="tableScroll" runat="server" id="itemBankDistScrollContainerDiv">
                                            <asp:Table runat="server" ID="itemBankTable" ClientIDMode="Static" CellPadding="2" CellSpacing="0" GridLines="Both" BorderWidth="0" Width="100%">
                                            </asp:Table>
                                        </div>
                                    </div>
                                </td>
                               </tr>
                            </table>
                    </div>
                </div>
                <br/>
                <p style="font-weight: normal;" class="tableLabel" id="lblRigor" runat="server">Item/Rigor Distribution:</p>
                <div class="tableContainer" align="center">
                    <table runat="server" id="rigorTable" ClientIDMode="Static" border="1" cellpadding="2" cellspacing="0">
                        <tr>
                            <td align="left" class="headerTD" style="font-weight: bold;padding: 3px;background-color: #9CF;">Distribution</td>
                            <td align="left" class="headerTD" style="font-weight: bold;padding: 3px;background-color: #9CF;" id="rigorLevelLabel" runat="server" ClientIDMode="Static">Rigor Level</td>
                            <td runat="server" id="AssmtItemSum" align="center" colspan="7" class="headerTD" style="font-weight: bold;padding: 3px;background-color: #9CF;">Assessment Item Summary</td>
                        </tr>
                        <tr>
                            <td class="subHeader" style="font-weight: bold;color: #36F;font-style: normal;width: 20%;padding: 3px;text-align: center;vertical-align: middle;color: #000;" id="distributionTotal" ClientIDMode="Static" runat="server"></td>
                            <td class="subHeader" style="font-weight: bold;color: #36F;font-style: italic;width: 20%;padding: 3px;text-align: center;vertical-align: bottom;">&nbsp;</td>
                            <td class="subHeader" style="font-weight: bold;color: #36F;font-style: italic;width: 20%;padding: 3px;text-align: center;vertical-align: bottom;">Multiple Choice(3)</td>
                            <td class="subHeader" style="font-weight: bold;color: #36F;font-style: italic;width: 20%;padding: 3px;text-align: center;vertical-align: bottom;">Multiple Choice(4)</td>
                            <td class="subHeader" style="font-weight: bold;color: #36F;font-style: italic;width: 20%;padding: 3px;text-align: center;vertical-align: bottom;">Multiple Choice(5)</td>
                            <td class="subHeader" style="font-weight: bold;color: #36F;font-style: italic;width: 20%;padding: 3px;text-align: center;vertical-align: bottom;">Short Answer</td>
                            <td class="subHeader" style="font-weight: bold;color: #36F;font-style: italic;width: 20%;padding: 3px;text-align: center;vertical-align: bottom;">Essay</td>
                            <td class="subHeader" style="font-weight: bold;color: #36F;font-style: italic;width: 20%;padding: 3px;text-align: center;vertical-align: bottom;">True/False</td>
                            <td runat="server" id="notSpecifiedCell" class="subHeader" style="font-weight: bold;color: #36F;font-style: italic;width: 20%;padding: 3px;text-align: center;vertical-align: bottom;">Not Specified</td>
                        </tr>
                    </table>
                    
                </div>

                <p runat="server" style="font-weight: normal;" class="tableLabel" id="lblAddendum">Addendum Distribution:</p>
                <div runat="server" id="addendumDistributionContainerDiv" style="width:450px; border:solid 2px #000; margin-left:37px;">
                    <asp:Table runat="server" ID="addendumTableHeader" ClientIDMode="Static" BorderWidth="0" CssClass="headerTable" Width="100%" CellPadding="2" CellSpacing="0" GridLines="Both">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell Width="336" CssClass="contentLabel" HorizontalAlign="Left">Addendums</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="contentLabel" id="addendumItemCountCell" ClientIDMode="Static">Item Count (0)</asp:TableHeaderCell>
                        </asp:TableHeaderRow>                                            
                    </asp:Table>
                    <div class="tableScrollAddendum" runat="server" id="addendumDistScrollContainerDiv">
                        <asp:Table runat="server" ID="addendumTable" ClientIDMode="Static" CellPadding="2" CellSpacing="0" GridLines="Both" BorderWidth="0" Width="100%">
                        </asp:Table>
                    </div>
                </div>
                <div style="margin-top: 15px;">
                    <asp:Button runat="server" ID="generateButton" ClientIDMode="Static" CssClass="roundButtons_blue" Text="&nbsp;&nbsp;Generate Assessment&nbsp;&nbsp;" 
                    OnClientClick="this.disabled = true; this.value = 'In Process...';" OnClick="generateButton_Click" UseSubmitBehavior="false" AutoPostBack="true"/>
                    <asp:Button runat="server" ID="backButton" ClientIDMode="Static" CssClass="roundButtons" Text="&nbsp;&nbsp;Back&nbsp;&nbsp;"
                    OnClientClick="backButtonClick(); return false;" />
                    <asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons" Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" 
                    OnClientClick="setTimeout(function() { getCurrentCustomDialog().close(); }, 0); return false;" />
                    <asp:ImageButton runat="server" ID="printButton" ClientIDMode="Static" CssClass="imgButtons" ImageUrl="~/Images/Printer.png"
                    OnClientClick="printPDFView();/*setTimeout(function() {window.radalert('Functionality is under construction.');}, 0);*/ return false;" />
                </div>
            </div>
        </div>

    </span>
    <input runat="server" type="hidden" id="hdnCallBackPage" clientidmode="Static" name="rigorSelection"/>
    <input runat="server" type="hidden" id="newAssessmentTitle" clientidmode="Static" name="newAssessmentTitle" value="" />
    
    <input runat="server" type="hidden" id="hiddenAccessSecureTesting" clientidmode="Static" name="hiddenAccessSecureTesting" value="" />
    <input runat="server" type="hidden" id="hiddenIsSecuredFlag" clientidmode="Static" name="hiddenIsSecuredFlag" value="" />
    <input runat="server" type="hidden" id="hiddenSecureType" clientidmode="Static" name="hiddenSecureType" value="" />
    <script type="text/javascript">
        var modalWin = parent.$find('RadWindow1Url');
        
        prepScreen();
        
        function prepScreen() {
            if(window.location.href.indexOf('xID=') > -1) {
                $('p[objectScreen="assessment"], div[objectScreen="assessment"]', '.containerDiv').css('display', '');
            }
        }

        function backButtonClick() {
            var page = $('#hdnCallBackPage').attr('value');
            var headerImg = $('#headerImg').attr('headerImgName');
            var allMax = true;
            if (navigator.appVersion.indexOf("Win") == -1) {
                allMax = false;
            }

            var hasSecure = $("#hiddenAccessSecureTesting").val().toLowerCase() == "true" ? true : false;
            var isSecureflag = $("#hiddenIsSecuredFlag").val().toLowerCase() == "true" ? true : false;
            var secureAssessment = $("#hiddenSecureType").val().toLowerCase() == "true" ? true : false;
            var isSecure = false;
            if (page == "rigor") {
                var dialogTitle = 'Assessment Item Rigor Selections';
                if (hasSecure == true && isSecureflag == true) {
                    if (secureAssessment == true) {
                        dialogTitle = SetSecureAssessmentTitle(dialogTitle);
                    }
                }
                var url = '../Dialogues/Assessment/AssessmentStandardsDetail.aspx?headerImg=' + headerImg + '&page=summary';
                var win = parent.customDialog({
                    name: 'RadWindowAddAssessment',
                    url: url,
                    title: dialogTitle,
                    maximize: true,
                    maxwidth: 1200,
                    maxheight: 550,
                    resizable: false,
                    movable: true,
                    center: true,
                    maximizable: allMax,
                });
                win.remove_beforeClose(win.confirmBeforeClose);
                win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
            }
            else if(page == "addendum")
            {
                var dialogTitle = 'Addendum Selection';
                if (hasSecure == true && isSecureflag == true) {
                    if (secureAssessment == true) {
                        dialogTitle = SetSecureAssessmentTitle(dialogTitle);
                    }
                }
                var url = '../Dialogues/Assessment/AssessmentAddendumsDetail.aspx?headerImg=' + headerImg + '&page=summary';
                var win = parent.customDialog({
                    name: 'RadWindowAddAssessment',
                    url: url,
                    title: dialogTitle,
                    maximize: true,
                    maxwidth: 1200,
                    maxheight: 550,
                    resizable: false,
                    movable: true,
                    center: true,
                    maximizable: allMax,
                });
                win.remove_beforeClose(win.confirmBeforeClose);
                win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
            }
            else {
                var dialogTitle = 'Assessment Standards Selection';
                if (hasSecure == true && isSecureflag == true) {
                    if (secureAssessment == true) {
                        dialogTitle = SetSecureAssessmentTitle(dialogTitle);
                    }
                }
                var win = parent.customDialog({
                    name: 'RadWindowAddAssessment',
                    url: '../Dialogues/Assessment/AssessmentStandards.aspx?headerImg=' + headerImg,
                    title: dialogTitle,
                    width: 1200,
                    height: 550,
                    maximizable: allMax,
                    resizable: false,
                    movable: true,
                    maximize_height: !allMax,
                    maximize_width: !allMax,
                    center: true
                });
                win.remove_beforeClose(win.confirmBeforeClose);
                win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
            }
        }

//        function closeCallback() {
//            if (modalWin) {
//                modalWin.set_width('950');
//                modalWin.set_height('675');
//                modalWin.remove_close(closeCallback);
//            }
//        }
        
        function closeSummary() {
            //closeCallback();
            if (modalWin) setTimeout(function() { modalWin.close(); }, 0);
        }

        function goToNewAssessment(id) {
            var modalTitle = document.getElementById('newAssessmentTitle').value;

            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: '../Record/AssessmentPage.aspx?xID=' + id,
                title: modalTitle,
                maximize: true,
                isSecure: true
            });
            win.remove_beforeClose(win.confirmBeforeClose);
        }

        function displayFullDescription(obj) {
            var fullTextSpan = $('.fullText', obj.parentNode);
            fullTextSpan.css('display', 'inline');
        }

        function printPDFView() {
            var url = window.location.href + '&printPDFView=yes';
            window.open(url, 'Assessment_Summary_PDFView');
        }

        function OpenAddendumText(id) {
            parent.customDialog({ url: ('../Controls/Assessment/ContentEditor/ContentEditor_Item_AddendumText.aspx?xID=' + id + '&by=addendum'), autoSize: true, name: 'ContentEditorItemAddendumText' });
        }

    </script>
</asp:Content>
