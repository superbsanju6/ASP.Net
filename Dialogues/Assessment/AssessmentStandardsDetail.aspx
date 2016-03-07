<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Dialogues/Assessment/AssessmentDialog.Master" CodeBehind="AssessmentStandardsDetail.aspx.cs" 
Inherits="Thinkgate.Dialogues.Assessment.AssessmentStandardsDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        body 
        {
            font-family: Sans-Serif, Arial;
            font-weight: bold;
        }
        
        .containerDiv 
        {
            width: 99%;
        }
        
        p 
        {
            font-family: Verdana;
            font-style: italic;
            font-size: 11px;
            margin-bottom: 5px !important;
            margin-top: 5px;
            color: #333;
            font-weight: normal;
        }
        
        .labels 
        {
            font-size: 12pt;
            width: 100px;
            text-align: left;
            margin-right: 10px;
            position: relative;
            float: left;
        }
        
        input 
        {
            text-align: center;
        }
        
        input.disabled
        {
            background-color: #E0E0E0;
            border: solid 1px #D0D0D0;
            color: #000;
        }
        
        .countsTextBox
        {
            padding-top: 5px;
            padding-bottom: 5px;
            border: solid 1px #000 !important;
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
            position: relative;
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

        .headerTD 
        {
            font-weight: bold;
            font-size: 14pt;
            padding: 3px;
        }
        
        #standardsTable 
        {
            border: solid 2px #000;
            border-collapse: collapse;
            width: 100%;
        }
        
        #standardsTable td 
        {
            border: solid 1px #000;
        }
        
        #standardName 
        {
            color: #A80000;
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
        }
        
        .contentElement 
        {
            font-weight: bold;
            width: 10%;
            text-align: center;
            padding: 3px;
        }
        
        .contentElement input
        {
            border: solid 1px #69F;
        }
        
        .empty 
        {
            background-color: #E3E4FA;  
        }
        
        .contentElement a 
        {
            color: #69F;
        }
        
        .contentElementInactive 
        {
            background-color: #C0C0C0;
            text-align: center;
            font-weight: normal;
        }
        
        #assessmentTitle 
        {
            padding-top: 10px;
            padding-bottom: 50px;
            text-align: center;
            font-size: 11pt;
        }
        
        .itemTotalDiv 
        {
            padding-bottom: 10px;
            font-size: 11pt;
        }
        
        .tableContainer 
        {
            padding-bottom: 15px;
        }

        .standardList
        {
            border: solid 1px #000;
            border-collapse: collapse;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" ClientIDMode="Static" runat="server" />
    <telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel" Width="100%" Height="100%">
        <img runat="server" id="headerImg" src="../../Images/repairtool.png" alt="Create assessment icon" class="headerImg" ClientIDMode="Static" />
        <div class="containerDiv" align="right">
            <div class="formContainer">
                <div class="headerDiv" style="overflow:hidden;">
                    <div runat="server" id="assessmentTitle" ClientIDMode="Static"></div>
                    <table style="padding-bottom: 10px; font-size: 11pt;">
                        <tr>
                            <td><input runat="server" id="assessmentItemCount" ClientIDMode="Static" class="disabled countsTextBox" onfocus="this.blur();" type="text" size="5" /></td>
                            <td style="padding-left:10px;">Assessment Item Count</td>
                            <td style="padding-left:10px;"><input runat="server" id="itemsSpecified" ClientIDMode="Static" class="disabled countsTextBox" onfocus="this.blur();" type="text" size="5" /></td>
                            <td style="padding-left:10px;">Standard Item Count&nbsp;</td>
                            <td><div id="lblStandardStateNbr" runat="server" clientidmode="Static"></div></td>
                        </tr>
                    </table>
                    <p style="float:left;padding-top:10px;">Specify in the space provided by each item type the number of available items desired for the assessment.</p>
                    <div style="float:right;margin-right:10px;">
                        <telerik:RadButton runat="server" ID="summaryButton" ClientIDMode="Static" Skin="Web20" OnClick="summaryButton_Click"
                            Text="  Summary  " Width="100px" CssClass="radDropdownBtn" UseSubmitBehavior="false" AutoPostBack="true"
                            OnClientClicked="updateAssessmentStandards" />
                    </div>
                </div>

                <div class="tableContainer" style="overflow:hidden;">
                    <div style="width:17%; float:left; display:inline;">
                        <div style="text-align:center !important;background-color: #9CF;font-size:12pt;padding:5px 0 5px 0;border-top: solid 1px #000;border-left: solid 1px #000;">Selected Standards</div>
                        <telerik:RadListBox ID="standardsList" runat="server" DataValueField="StandardID" DataTextField="StateNbr" CssClass="standardList" Font-Bold="false"
                            Width="100%" AutoPostBack="true" Height="251px" OnSelectedIndexChanged="standardsList_SelectedIndexChanged" OnClientSelectedIndexChanged="updateAssessmentStandards">
                            <ItemTemplate> 
                                <asp:Label ID="lblStandardStateNbr" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "StateNbr") %>' width="100%" ToolTip='<%# DataBinder.Eval(Container.DataItem, "Description") %>'></asp:Label> 
                            </ItemTemplate>   
                        </telerik:RadListBox>
                    </div>
                    <div style="float:left; display:inline; width:82%;">
                        <table runat="server" id="standardsTable" ClientIDMode="Static" border="1" cellpadding="2" cellspacing="0">
                            <tr>
                                <td align="left" id="standardName" ClientIDMode="Static" class="headerTD"></td>
                                <td align="center" colspan="14" class="headerTD" style="background-color: #9CF;">Available Items by Item Type</td>
                            </tr>
                            <tr>
                                <td class="subHeader" id="dokType" runat="server" ClientIDMode="Static"></td>
                                <td class="subHeader" colspan="2">Multiple Choice<br />(3 Distractors)</td>
                                <td class="subHeader" colspan="2">Multiple Choice<br />(4 Distractors)</td>
                                <td class="subHeader" colspan="2">Multiple Choice<br />(5 Distractors)</td>
                                <td class="subHeader" colspan="2">Short Answer</td>
                                <td class="subHeader" colspan="2">Essay</td>
                                <td class="subHeader" colspan="2">True/False</td>
                                <td class="subHeader" id="BlueprintHeader" colspan="1" visible ="false">Blueprint</td>
                            </tr>
                        </table>
                    </div>
                </div>

                <asp:Button runat="server" ID="updateButton" ClientIDMode="Static" CssClass="roundButtons_blue" Text="&nbsp;&nbsp;Generate Assessment&nbsp;&nbsp;" UseSubmitBehavior="false" OnClick="updateButton_Click"
                    OnClientClick="this.disabled = true; this.value = 'In Process...'; updateAssessmentStandards(this);" />
                  
                <asp:Button runat="server" ID="backButton" ClientIDMode="Static" CssClass="roundButtons"
                    Text="&nbsp;&nbsp;Back&nbsp;&nbsp;" OnClientClick="backButtonClick(); return false;" />

                <asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons"
                    Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="setTimeout(function() { getCurrentCustomDialog().close(); }, 0); return false;" />
            </div>
        </div>

        <input runat="server" type="hidden" id="assessmentID" clientidmode="Static" name="assessmentID" value="" />
        <input runat="server" type="hidden" id="standardID" clientidmode="Static" name="standardID" />
        <input runat="server" type="hidden" id="hiddenstandardSet" clientidmode="Static" name="hiddenstandardSet"/>
        <input runat="server" type="hidden" id="hdnRigorSelection" clientidmode="Static" name="rigorSelection"/>
        <input runat="server" type="hidden" id="newAssessmentTitle" clientidmode="Static" name="newAssessmentTitle" value="" />
        
        <input runat="server" type="hidden" id="hiddenAccessSecureTesting" clientidmode="Static" name="hiddenAccessSecureTesting" value="" />
        <input runat="server" type="hidden" id="hiddenIsSecuredFlag" clientidmode="Static" name="hiddenIsSecuredFlag" value="" />
        <input runat="server" type="hidden" id="hiddenSecureType" clientidmode="Static" name="hiddenSecureType" value="" />

        </telerik:RadAjaxPanel>
   
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
     <script type="text/javascript">

         $(document).ready(function () {
             setInterval(function () { changeListHeight(); }, 0);
         })
         function changeListHeight() {
             var ht = parseInt($('#standardsTable').css('height').replace("px", '')) - 32;
             $("#<%= standardsList.ClientID %>").css('height', ht + "px");
         }

        function backButtonClick() {
            var confirmDialogText = 'By selecting the <b> Back </b> button, you will lose any data entered on this screen. Select <b> Ok </b> to proceed with this action. Select <b> Cancel </b> to remain on this screen.';
            customDialog({ maximize: true, maxwidth: 432, maxheight: 150, resizable: false, title: 'Alert', content: confirmDialogText, dialog_style: 'alert', closeMode: false },
                [{ title: 'Cancel' }, { title: 'OK', callback: backCallbackFunction }]);
        }

        function backCallbackFunction() {
            var allMax = true;
            if (navigator.appVersion.indexOf("Win") == -1) {
                allMax = false;
            }

            var hasSecure = $("#hiddenAccessSecureTesting").val().toLowerCase() == "true" ? true : false;
            var isSecureflag = $("#hiddenIsSecuredFlag").val().toLowerCase() == "true" ? true : false;
            var secureAssessment = $("#hiddenSecureType").val().toLowerCase() == "true" ? true : false;
            var dialogTitle = 'Assessment Standards Selection';
            if (hasSecure == true && isSecureflag == true) {
                if (secureAssessment == true) {
                    dialogTitle = SetSecureAssessmentTitle(dialogTitle);
                }
            }

            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: '../Dialogues/Assessment/AssessmentStandards.aspx?headerImg=' + $('#headerImg').attr('headerImgName'),
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

        function getTotalItemCount(val) {
            var totalItemCount = 0;
            var standardsTable = $('#standardsTable');

            $('input[id$="_input"]', standardsTable).each(function (index) {
                totalItemCount += ($(this).attr('value') == '' ? 0 : parseInt($(this).attr('value')));
            });
            if (parseInt(val) > parseInt(totalItemCount))
                totalItemCount = val;

            var objassessmentItemCount = $('#assessmentItemCount').attr('value');
            var olditemsSpecified = $('#itemsSpecified').attr('value');

            var diff = olditemsSpecified - totalItemCount;
            var newvalue = objassessmentItemCount - diff;

            $('#assessmentItemCount').attr('value', newvalue);
            $('#itemsSpecified').attr('value', totalItemCount);
        }

        function onlyNumsLessOrEqualToCount(id, obj, e) {
            var itemsAvailable = typeof ($('#' + id.replace('input', 'link')).html()) == 'undefined' ? 0 : $('#' + id.replace('input', 'link')).html();
            var existingValue = obj.value.length == 0 || isNaN(obj.value) ? '' : obj.value;
            var caretStart = obj.selectionStart;
            var caretEnd = obj.selectionEnd;
            var keynum = '';
            var keychar = '';
            var notRange = true;
            
            if (window.event) {// IE
                keynum = e.keyCode;
            }
            else if (e.which) {// Netscape/Firefox/Opera
                keynum = e.which;
            }
            
            if(typeof(caretStart) == 'undefined') {
                caretStart = doGetCaretPosition(obj);
            }
            if (typeof (caretEnd) == 'undefined') {
                if (document.selection.type == 'None') {
                    caretEnd = caretStart;
                }
                else {
                    caretEnd = document.selection.createRange().text.length;
                    notRange = false;
                }
            }

            if (keynum == '') return true;

            if (existingValue != '') {
                if (caretStart == caretEnd && notRange) {
                    keychar = existingValue.substr(0, caretStart) + String.fromCharCode(keynum) + existingValue.substr(caretStart, existingValue.length);
                }
                else if (notRange) {
                    keychar = existingValue.substr(0, caretStart) + String.fromCharCode(keynum) + existingValue.substr(caretEnd, existingValue.length);
                }
                else {
                    keychar = existingValue.substr(0, caretStart) + String.fromCharCode(keynum) + existingValue.substr(caretStart + caretEnd, existingValue.length);
                }
            }
            else {
                keychar = existingValue + String.fromCharCode(keynum);
            }

            if (!isNaN(keychar) && parseInt(keychar) <= itemsAvailable) {
                if (parseInt(keychar) == 0) {
                    obj.value = '';
                    obj.className = 'empty';
                }
                else {
                    obj.value = Math.round(keychar);
                    if (typeof (obj.selectionStart) != 'undefined') obj.selectionStart = obj.selectionEnd;
                    else {
                        obj.onfocus = function () { this.value = this.value; this.onfocus = null; };
                        obj.blur();
                        obj.focus();
                    }
                    obj.className = '';
                }
            }

            return false;
        }

        function doGetCaretPosition(ctrl) {
            var CaretPos = 0;
            // IE Support
            if (document.selection) {

                ctrl.focus();
                var Sel = document.selection.createRange();
                var SelLength = document.selection.createRange().text.length;
                Sel.moveStart('character', -ctrl.value.length);
                CaretPos = Sel.text.length - SelLength;
            }
            // Firefox support
            else if (ctrl.selectionStart || ctrl.selectionStart == '0')
                CaretPos = ctrl.selectionStart;

            return (CaretPos);

        }

        function updateAssessmentStandards(e) {
            var standardsTable = $('#standardsTable');
            var standardID = $('#standardID').attr('value');
            var panelValue = '{"StandardID":' + standardID + ', "RigorLevels":';
            var itemsSpecified = $('#itemsSpecified').attr('value');
            var standardName = $('#standardName').text();
            var rigorLevels = '[';
            var multipleChoice3Counts = '[';
            var multipleChoice4Counts = '[';
            var multipleChoice5Counts = '[';
            var shortAnswerCounts = '[';
            var essayCounts = '[';
            var trueFalseCounts = '[';
            var blueprintCounts = '[';
            var standardset = $('#hiddenstandardSet').attr('value');

            $('.contentLabel', standardsTable).each(function (index) {
                rigorLevels += '"' + $(this).html() + '",';

                var multipleChoice3Input = $('#' + ($(this).html().toLowerCase() == 'not specified' ? 'na' : $(this).html().toLowerCase()) + '_multiplechoice3_input');
                multipleChoice3Counts += (typeof (multipleChoice3Input.attr('value')) == 'undefined' || multipleChoice3Input.attr('value') == '' ? 0 : multipleChoice3Input.attr('value')) + ',';

                var multipleChoice4Input = $('#' + ($(this).html().toLowerCase() == 'not specified' ? 'na' : $(this).html().toLowerCase()) + '_multiplechoice4_input');
                multipleChoice4Counts += (typeof (multipleChoice4Input.attr('value')) == 'undefined' || multipleChoice4Input.attr('value') == '' ? 0 : multipleChoice4Input.attr('value')) + ',';

                var multipleChoice5Input = $('#' + ($(this).html().toLowerCase() == 'not specified' ? 'na' : $(this).html().toLowerCase()) + '_multiplechoice5_input');
                multipleChoice5Counts += (typeof (multipleChoice5Input.attr('value')) == 'undefined' || multipleChoice5Input.attr('value') == '' ? 0 : multipleChoice5Input.attr('value')) + ',';

                var shortAnswerInput = $('#' + ($(this).html().toLowerCase() == 'not specified' ? 'na' : $(this).html().toLowerCase()) + '_shortanswer_input');
                shortAnswerCounts += (typeof (shortAnswerInput.attr('value')) == 'undefined' || shortAnswerInput.attr('value') == '' ? 0 : shortAnswerInput.attr('value')) + ',';

                var essayInput = $('#' + ($(this).html().toLowerCase() == 'not specified' ? 'na' : $(this).html().toLowerCase()) + '_essay_input');
                essayCounts += (typeof (essayInput.attr('value')) == 'undefined' || essayInput.attr('value') == '' ? 0 : essayInput.attr('value')) + ',';

                var trueFalseInput = $('#' + ($(this).html().toLowerCase() == 'not specified' ? 'na' : $(this).html().toLowerCase()) + '_truefalse_input');
                trueFalseCounts += (typeof (trueFalseInput.attr('value')) == 'undefined' || trueFalseInput.attr('value') == '' ? 0 : trueFalseInput.attr('value')) + ',';

                var blueprintInput = $('#' + ($(this).html().toLowerCase() == 'not specified' ? 'na' : $(this).html().toLowerCase()) + '_blueprint_input');
                blueprintCounts += (typeof (blueprintInput.attr('value')) == 'undefined' || blueprintInput.attr('value') == '' ? 0 : blueprintInput.attr('value')) + ',';
            });

            rigorLevels = rigorLevels.substr(0, rigorLevels.lastIndexOf(',')) + ']';
            multipleChoice3Counts = multipleChoice3Counts.substr(0, multipleChoice3Counts.lastIndexOf(',')) + ']';
            multipleChoice4Counts = multipleChoice4Counts.substr(0, multipleChoice4Counts.lastIndexOf(',')) + ']';
            multipleChoice5Counts = multipleChoice5Counts.substr(0, multipleChoice5Counts.lastIndexOf(',')) + ']';
            shortAnswerCounts = shortAnswerCounts.substr(0, shortAnswerCounts.lastIndexOf(',')) + ']';
            essayCounts = essayCounts.substr(0, essayCounts.lastIndexOf(',')) + ']';
            trueFalseCounts = trueFalseCounts.substr(0, trueFalseCounts.lastIndexOf(',')) + ']';
            blueprintCounts = blueprintCounts.substr(0, blueprintCounts.lastIndexOf(',')) + ']';
            
            /*******************************************************************************************
            * 2012-07-22 - Per Ashley Reeves, remove this rule as any adjustment of # of questions per
            * standard with Item Rigors window should be reflected in Assessment Standards window. - DHB
            *******************************************************************************************/
            panelValue += rigorLevels + ', "TotalItemCount":' + itemsSpecified +
                                        ', "MultipleChoice3Counts":' + multipleChoice3Counts +
                                        ', "MultipleChoice4Counts":' + multipleChoice4Counts +
                                        ', "MultipleChoice5Counts":' + multipleChoice5Counts +
                                        ', "ShortAnswerCounts":' + shortAnswerCounts +
                                        ', "EssayCounts":' + essayCounts + 
                                        ', "TrueFalseCounts":' + trueFalseCounts +
                                        ', "BlueprintCounts":' + blueprintCounts +
                                        ', "StandardSet":"' + standardset +
                                        '", "StandardName":"' + standardName +'"}';
            $('#hdnRigorSelection').attr('value', panelValue);

            if (e.id == "updateButton") {
                ShowSpinner('form1');
            }
        }

        function goBackToStandardsList(sender, args) {
            //var result = args.get_content();
        
            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: '../Dialogues/Assessment/AssessmentStandards.aspx?headerImg=lightningbolt&prevstandardSet=' + $('#hiddenstandardSet').attr('value'),
                title: 'Assessment Standards Selection',
                maximize: true, maxwidth: 1200, maxheight: 500
            });
            win.remove_beforeClose(win.confirmBeforeClose);
            win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
        }

        /*GENERATE ASSESMENT SAVE*/

        function ShowSpinner(target) {
            currentLoadingPanel = $find("updPanelLoadingPanel");

            currentUpdatedControl = target ? target : "form2";
            //show the loading panel over the updated control 
            currentLoadingPanel.show(currentUpdatedControl);
        }
        function goToNewAssessment(id) {
            // var result = args.get_content();
            var modalTitle = document.getElementById('newAssessmentTitle').value;

            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: '../Record/AssessmentPage.aspx?xID=' + id,
                title: modalTitle,
                maximize: true,
                destroyOnClose: true,
            });
            win.remove_beforeClose(win.confirmBeforeClose);
        }

        function goToAssessmentSummary() {
            var headerImg = $('#headerImg').attr('headerImgName');

            var hasSecure = $("#hiddenAccessSecureTesting").val().toLowerCase() == "true" ? true : false;
            var isSecureflag = $("#hiddenIsSecuredFlag").val().toLowerCase() == "true" ? true : false;
            var secureAssessment = $("#hiddenSecureType").val().toLowerCase() == "true" ? true : false;
            var dialogTitle = 'Assessment Criteria Summary';
            if (hasSecure == true && isSecureflag == true) {
                if (secureAssessment == true) {
                    dialogTitle = SetSecureAssessmentTitle(dialogTitle);
                }
            }

            var url = '../Dialogues/Assessment/AssessmentSummary.aspx?headerImg=' + headerImg + '&page=rigor';
            var adjustedHeight = Math.ceil($telerik.$(parent.window).height() * 96 / 100);
            var winHeight = adjustedHeight < 625 ? adjustedHeight : 625; //to compensate for clients with very low resolution
            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: url,
                title: dialogTitle,
                maximize: true, maxwidth: 800
            });
            win.remove_beforeClose(win.confirmBeforeClose);
            win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
        }
        /* END CODE BLOCK*/
    </script>
    </telerik:RadScriptBlock>
</asp:Content>