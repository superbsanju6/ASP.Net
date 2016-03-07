<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Dialogues/Assessment/AssessmentDialog.Master" CodeBehind="AssessmentAddendumsDetail.aspx.cs"
    Inherits="Thinkgate.Dialogues.Assessment.AssessmentAddendumsDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        body {
            font-family: Sans-Serif, Arial;
            font-weight: bold;
        }

        input {
            text-align: center;
        }

        input.disabled {
            background-color: #E0E0E0;
            border: solid 1px #D0D0D0;
            color: #000;
        }

        .headerImg {
            position: absolute;
            left: 2px;
            top: 10px;
            width: 40px;
        }

        .containerDiv {
            width: 99%;
        }

        .formContainer {
            width: 95%;
            text-align: left;
            margin-top: 10px;
        }

        .roundButtons {
            color: #00F;
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
            font-weight: bold;
        }

        .standardsFixedCols {
            float: left; 
            margin: 0px; 
            padding: 0px; 
            width: 364px; 
            height: 111%; 
            overflow-x: hidden; 
            overflow-y: hidden;
        }

        @media screen and (max-width: 1366px) {
            .standardsFixedCols {
                float: left; 
                margin: 0px; 
                padding: 0px; 
                width: 364px; 
                height: 108%; 
                overflow-x: hidden; 
                overflow-y: hidden;
            }
        }

        .roundButtons_blue {
            color: #FFF;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            position: relative;
            border: solid 1px #000;
            border-radius: 50px;
            float: right;
            margin-left: 10px;
            cursor: pointer;
            background-color: #36C;
        }

        #assessmentTitle {
            padding-top: 10px;
            padding-bottom: 30px;
            text-align: center;
            font-size: 11pt;
        }

        #itemsSpecified {
            padding-top: 5px;
            padding-bottom: 5px;
            border: solid 1px #000;
        }

        .itemTotalDiv {
            padding-bottom: 2px;
            padding-left: 2px;
            padding-right: 2px;
            font-size: 11pt;
        }

        .summaryButton {
            float: right;
        }

        .tableContainer
         {
            padding-bottom: 15px;
            margin-bottom: 5px;
            height: 100% !important;
            position:absolute !important;
            width: 100%;
            /*height: 320px;*/
            overflow: hidden;
         }

        .contentHeaderSelectAllItems {
            font-size: 12px;
            font-weight: bold;
            /*padding: 3px;*/
            width: 50px;
            text-align: center;
            border: 1px solid black;
        }

        .contentSelectAllItems {
            /*padding: 3px;*/
            width: 50px;
            min-width: 50px;
            text-align: center;
        }

        .contentHeaderName {
            font-size: 12px;
            font-weight: bold;
            width: 250px;
            min-width: 250px;
            height: 35px;
            line-height: 35px;
            border: 1px solid black;
        }

        .contentHeaderName span {
            padding-left: 5px;
        }

        .contentAddendumName {
            white-space: nowrap;
            font-size: 12px;
            font-weight: normal;
            text-align: left;
            width: 250px;
            min-width: 250px;
            max-width: 250px;
            height: 20px;
            overflow: hidden;
            width: 250px;
            text-overflow: ellipsis;
        }

        .contentAddendumName a {
            padding-left: 5px;
        }

        .nameLinkPartialDisplay
        {
            white-space: nowrap;                   
            overflow: hidden;
            text-overflow: ellipsis;
            display: inline-block;
            max-width: 152px;

            font-size: 12px;
            font-weight: normal;
            text-align: left;
            /*padding: 0px;
            padding-left: 2px;*/
        }

        .contentHeaderSelItems {
            font-size: 12px;
            font-weight: bold;
            width: 60px;
            min-width: 60px;
            text-align: center;
            border: 1px solid black;
        }

        .contentHeaderSelItemsNormal {
            font-size: 12px;
            font-weight: normal;
            /*padding: 3px;*/
            width: 60px;
            min-width: 60px;
            text-align: center;
        }

        .contentLabel {
            font-weight: bold;
        }

        .contentLabelNormal {
            font-size: 11px;
            font-weight: normal;
            text-align: center;
        }

        .contentElement {
            font-size: 12px;
            font-weight: bold;
            min-width: 81px;
            width: 81px;
            height: 35px;
            line-height: 34px;
            text-align: center;
            vertical-align: initial;
        }

        .contentElementData {
            font-size: 12px;
            font-weight: normal;
            min-width: 40px;
            text-align: center;
            height: 20px;

        }

        .contentElementData input {
            border: solid 1px #69F;
            width: 25px;
        }

        .contentElementInactive {
            background-color: #C0C0C0;
            text-align: center;
            font-weight: normal;
            width: 40px;
            min-width: 40px;
            max-width: 40px;
            height: 20px;
            /*padding: 3px;*/
        }

        .containerDiv
        {
            height:69%;
            position:absolute;
        }

        .formContainer
        {
            height:100%;
        }
        
        p {
            font-family: Verdana;
            font-style: italic;
            font-size: 11px;
            margin-bottom: 5px !important;
            margin-top: 5px;
            color: #333;
            font-weight: normal;
        }
         #overlay {
           position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: #000;
            filter:alpha(opacity=30);
            -moz-opacity:0.5;
            -khtml-opacity: 0.5;
            opacity: 0.3;
            z-index: 10000;
        }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  
   <div id="divSpinner" style="display:none;text-align:center">
                        <asp:Image ID="spinnerImage" ImageUrl="~/Skins/Common/Loading.gif" runat="server" ImageAlign="Middle" />
                    </div>
      <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    <img runat="server" id="headerImg" src="../../Images/magicwand.png" alt="Create assessment icon" class="headerImg" clientidmode="Static" />

    <div class="containerDiv" align="right">
        <div class="formContainer">
            <div class="headerDiv">
                <div runat="server" id="assessmentTitle" clientidmode="Static"></div>
                <div class="itemTotalDiv">
                    <p>When there are not enough items for the available addendums, the system will randomly select items for the selected standards.</p>
                    <input runat="server" id="itemsSpecified" clientidmode="Static" class="disabled" onfocus="this.blur();" type="text" size="5" />&nbsp;&nbsp;&nbsp;Assessment Items

                    <span class="summaryButton">
                        <telerik:RadButton runat="server" ID="summaryButton" ClientIDMode="Static" Skin="Web20"
                            Text="  Summary  " AutoPostBack="true" CssClass="radDropdownBtn" UseSubmitBehavior="false"
                            OnClientClicked="updateAddendumStandards" OnClick="summaryButton_Click" />
                    </span>
                </div>
            </div>

            <div class="tableContainer" id="addendumTable" runat="server">
                <div style="margin: 0px; padding: 0px; width: 100%; height: 37px;">
                    <table runat="server" id="standardsTableNameAndSel" clientidmode="Static" border="0" style="height: 35px; float: left;">
                        <tr>
                            <td class="contentHeaderSelectAllItems">
                                <span>Select All Items</span>
                            </td>
                            <td class="contentHeaderName">
                                <span>Addendum Name</span>
                            </td>
                            <td class="contentHeaderSelItems">
                                <span>Selected Items</span>
                            </td>
                        </tr>
                    </table>
                    <div id="divstandardsTableFixedRow" style="width: 65.4%; overflow: hidden; height: 37px; line-height: 37px; margin-left: 312px;">
                        <table runat="server" id="standardsTableFixedRow" clientidmode="Static" border="1" style="">
                        </table>
                    </div>
                </div>
                <div style="margin: 0px; padding: 0px; width: 100%; height: 78%;">
                    <div id="divStandardsTableFixedCols" class="standardsFixedCols">
                        <table runat="server" id="standardsTableFixedCols" clientidmode="Static" border="1" style="margin: 0px; padding: 0px;">
                        </table>
                    </div>
                    <div id="divStandardsTable" style="float: left; margin: 0px; padding: 0px; width: 66.9%; height: 114%; overflow: scroll;">
                        <table runat="server" id="standardsTable" clientidmode="Static" border="1" style="margin: 0px; padding: 0px;">
                        </table>
                    </div>
                </div>
            </div>
            <div ID="lblNoAddendum" runat="server" visible="false" style="margin-top:30px;">
                <asp:Label runat="server" Font-Bold="false" Style="font-family: Verdana; font-size: 12px;">There are no addendums available for the selected standards.</asp:Label>
            </div>
            <input runat="server" type="hidden" id="hdnAddendumSelection" clientidmode="Static" name="AddendumSelection"/>
            <input runat="server" type="hidden" id="newAssessmentTitle" clientidmode="Static" name="newAssessmentTitle" value="" />
            <input runat="server" type="hidden" id="initialTotalItemCount" clientidmode="Static" name="initialTotalItemCount" value="" />
            <input runat="server" type="hidden" id="noAddendum" clientidmode="Static" name="noAddendum" value="false" />
        </div>

        <div style="float:right;margin-top:10%;">
             <asp:Button runat="server" ID="generateButton" ClientIDMode="Static" CssClass="roundButtons_blue" Text="&nbsp;&nbsp;Generate Assessment&nbsp;&nbsp;" UseSubmitBehavior="false" OnClick="generateButton_Click"
                    OnClientClick="this.disabled = true; this.value = 'In Process...'; updateAddendumStandards(this);" />
 
            <asp:Button runat="server" ID="backButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Back&nbsp;&nbsp;" OnClientClick="backButtonClick(); return false;" />
 
            <asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="setTimeout(function() { getCurrentCustomDialog().close(); }, 0); return false;" />
        </div>
        <input runat="server" type="hidden" id="hiddenAccessSecureTesting" clientidmode="Static" name="hiddenAccessSecureTesting" value="" />
            <input runat="server" type="hidden" id="hiddenIsSecuredFlag" clientidmode="Static" name="hiddenIsSecuredFlag" value="" />
            <input runat="server" type="hidden" id="hiddenSecureType" clientidmode="Static" name="hiddenSecureType" value="" />
    </div>
    <script type="text/javascript">

        $(document).ready(function () {
            $("#standardsTable").find("input[id*=_input]").bind('paste', function (e) {
                e.preventDefault();
            });

            var divToScroll = $get("divStandardsTableFixedCols");
            var divSource = $get("divStandardsTable");
            var divHorizontal = $get("divstandardsTableFixedRow");

            $('#divStandardsTable').scroll(function () {
                $(divToScroll).scrollTop(divSource.scrollTop);
                $(divHorizontal).scrollLeft(divSource.scrollLeft);
            });
        });

        function backButtonClick() {
            var noAddendum = document.getElementById('noAddendum').value;
            if (noAddendum == "false") {
                var confirmDialogText = 'By selecting the <b> Back </b> button, you will lose any data entered on this screen. Select <b> Ok </b> to proceed with this action. Select <b> Cancel </b> to remain on this screen.';

                customDialog({ maximize: true, maxwidth: 432, maxheight: 150, resizable: false, title: 'Alert', content: confirmDialogText, dialog_style: 'alert', closeMode: false },
                    [{ title: 'Cancel' }, { title: 'OK', callback: backCallbackFunction }]);
            }
            else
            {
                backCallbackFunction();
            }
        }

        function backCallbackFunction() {
            var allMax = true;
            if (navigator.appVersion.indexOf("Win") == -1) {
                allMax = false;
            }

            var hiddenAccessSecurePermission = $("#hiddenAccessSecureTesting").val().toLowerCase() == "true" ? true : false;
            var hiddenIsSecuredFlag = $("#hiddenIsSecuredFlag").val().toLowerCase() == "true" ? true : false;
            var hiddenSecureType = $("#hiddenSecureType").val().toLowerCase() == "true" ? true : false;
            var title = "Assessment Standards Selection";
            if (hiddenAccessSecurePermission == true && hiddenIsSecuredFlag == true) {
                if (hiddenSecureType == true) {
                    title = SetSecureAssessmentTitle(title);
                }
            }

            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: '../Dialogues/Assessment/AssessmentStandards.aspx?headerImg=' + $('#headerImg').attr('headerImgName'),
                title: title,
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

        function OpenAddendumText(id) {
            customDialog({ url: ('../../Controls/Assessment/ContentEditor/ContentEditor_Item_AddendumText.aspx?xID=' + id + '&by=addendum'), autoSize: true, name: 'ContentEditorItemAddendumText' });
        }

        function getTotalItemCount(val, inputTextbox, flag) {
            var totalItemCount = 0;
            var standardsTable = $('#standardsTable');
            var standardID = $(inputTextbox).attr("standardID");
            var totalStandardItemCount = 0;

            /* Find SUM of inputs for the current row to show into "Selected Items" */
            var rowTotal = 0;
            $(inputTextbox).parent("td").parent("tr").find("input[id*=_input]").each(function (index, countTextbox) {
                var selCount = $.trim($(countTextbox).val());
                if (selCount != "" && selCount.length > 0) {
                    rowTotal += parseInt(selCount);
                }
            });

            var addendumId = $(inputTextbox).parent("td").parent("tr").attr("AddendumID");

            if (flag == true) {
                var totalAvailCount = 0;
                $(inputTextbox).parent("td").parent("tr").find(".contentElementData:nth-child(even)").each(function (index) {
                    var selCount = $.trim($(this).html());
                    totalAvailCount += parseInt(selCount);
                });

                var checkbox = $("#standardsTableFixedCols").find("tr[AddendumID=" + addendumId + "]").find(".contentSelectAllItems").find("input");

                if(rowTotal == totalAvailCount)
                {
                    $(checkbox).attr('checked', 'checked');
                }
                else
                {
                    $(checkbox).removeAttr('checked');
                }
            }

            $("#standardsTableFixedCols").find("tr[AddendumID=" + addendumId + "]").find(".contentHeaderSelItemsNormal").text(rowTotal);

            /* Find SUM of inputs for the current standard, to show into Standard Columns header */
            var enteredItemCount = 0;
            $(inputTextbox).parents("table").find("input[standardID=" + standardID + "]").each(function (index, textBox) {
                var selCount = $.trim($(textBox).val());
                if (selCount != "" && selCount.length > 0) {
                    enteredItemCount += parseInt(selCount);
                }
            });
            var targetCell = $("#divstandardsTableFixedRow").find("tr").first().find("td[standardID=" + standardID + "]");
            var originalItemCount = targetCell.attr("originalItemCount");
            if (parseInt(enteredItemCount) > parseInt(originalItemCount)) {
                enteredItemCount = enteredItemCount;
            }
            else {
                enteredItemCount = originalItemCount;
            }
            $(targetCell.find("span")[1]).text(" (" + enteredItemCount + ")");
            targetCell.attr("newItemCount", enteredItemCount);

            /* Find SUM of Standard Headers' item count, to show into total "Assessment Items" count */
            var totalEffectiveItemsCount = 0;
            $("#divstandardsTableFixedRow").find("tr").first().find(".contentElement").each(function (index, td) {
                var newItemCounts = $(td).attr("newItemCount");
                totalEffectiveItemsCount += parseInt(newItemCounts);
            });
            $('#itemsSpecified').attr('value', totalEffectiveItemsCount);
        }

        function onlyNumsLessOrEqualToCount(id, obj, e) {
            var itemsAvailable = $(obj).attr("itemCount");
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

            if (typeof (caretStart) == 'undefined') {
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
            if (parseInt(keynum) == 8) return true;

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

        function updateAddendumStandards(e) {           
            ShowSpinner();
            var standardsTable = $('#standardsTable');
            var panelValue = '{"AddendumLevels":[';
            var itemsSpecified = $('#itemsSpecified').attr('value');
            var addendumLevels = '],';
            var standardinputCounts = '[';
            var standardtotals = '[';
            var standardset = $('#hiddenstandardSet').attr('value');
            var colCount = 0;
            var itemcol = '';

            /* Get standards, and item counts for each standard */
            $('#standardsTableFixedRow tr:first').each(function (index) {
                var totalcols = $(this).find('td').length;
                $(this).find('td').each(function (i) {
                    if ($(this).attr('colspan')) {
                        var standardID = '"StandardID":' + $(this).find('span:eq(0)').attr('StdID') + ','
                        var name = '"StandardName":"' + $(this).find('span:eq(0)').attr('StdName') + '",'
                        var count = '"Count":' + $(this).find('span:eq(1)').html().replace('(', '').replace(')', '').trim()
                        itemcol += '{' + standardID + name + count + '},';
                    }
                });
                standardtotals = standardtotals + itemcol;
            });
            standardtotals = standardtotals.substr(0, standardtotals.lastIndexOf(',')) + ']';

            var items = '';
            $('#standardsTableFixedCols tr').each(function (index) {
                var ID = '';
                var addendumName = '';
                var count = '';
                var include = true;
                
                $(this).find('td').each(function (i) {
                    include = true;
                    if ($(this).hasClass('contentAddendumName')) {
                        ID = '"AddendumID":"' + $(this).find('a').attr('AddendumID') + '",';
                        addendumName = $(this).context.innerText;
                        if (addendumName == undefined) {
                            addendumName = $(this).context.textContent;
                        }
                        addendumName = escape(addendumName);
                        addendumName = '"AddendumName":"' + addendumName + '",';
                    }
                    if ($(this).hasClass('contentHeaderSelItemsNormal')) {
                        count = '"Count":' + $(this).html();
                        var itemCountValue = $.trim($(this).html());
                        if (itemCountValue == "" || itemCountValue == "0") {
                            include = false;
                            return;
                        }
                    }
                });
                if (include) {
                    items += '{' + ID + addendumName + count + '},';
                }
            });
            if (items.length > 0) {
                items = items.substr(0, items.lastIndexOf(','));
            }
            items = '[' + items + ']';

            $('#standardsTable tr').each(function (index) {
                var item = '';
                var addendumvalue = '"AddendumID":' + $(this).attr('AddendumID') + ',';
                var totalColumns = $(this).find('td').length;
                var counter = 0;
                $(this).find('td').each(function (i) {                               
                    
                    if ($(this).find('input').length > 0) {
                        var standardvalue = '"StandardID":' + $(this).attr('standardid') + ','
                        var textinput = $(this).find('input').attr('value').trim();
                        if (textinput != "") {
                            if ($(this).attr('colspan')) {
                                colCount += +$(this).attr('colspan');                                
                                standardinputCounts = '"ItemCount":' + textinput + ',';
                            } else {
                                standardinputCounts = '"ItemCount":' + textinput + ',';
                                colCount++;
                            }
                            item += '{' + addendumvalue + standardvalue + standardinputCounts;
                            item = item.substr(0, item.lastIndexOf(',')) + '},';
                        }
                        
                    }                    
                });
                panelValue += item;               

            });
            if (panelValue.indexOf(',') != -1)
                panelValue = panelValue.substr(0, panelValue.lastIndexOf(',')) + ']';
            else
                panelValue += ']';

            var selectAllState = '';
            var selectAllId = '';
            var state = '';

            $('#standardsTableFixedCols tr').each(function (index) {
                var chkSelectAll = $(this).find('td:eq(0)').find('input:eq(0)');
                selectAllId = '"SelectAllId":"' + chkSelectAll.attr('AddendumIDItemBank') + '",';
                state = '"State":' + chkSelectAll.is(':checked');
                selectAllState += '{' + selectAllId + state + '},';
            });

            if (selectAllState.length > 0) {
                selectAllState = selectAllState.substr(0, selectAllState.lastIndexOf(','));
            }
            selectAllState = '[' + selectAllState + ']';

            panelValue +=  ',"TotalItemCount":' + itemsSpecified +
                                        ', "StandardCounts":' + standardtotals +', "AddendumCounts":'+ items + ', "SelectAllStates":' + selectAllState + '}';

            $('#hdnAddendumSelection').attr('value', panelValue);

            HideSpinner();
           
        }

        function goToNewAssessment(id) {
            // var result = args.get_content();
            var modalTitle = document.getElementById('newAssessmentTitle').value;

            var hiddenAccessSecurePermission = $("#hiddenAccessSecureTesting").val().toLowerCase() == "true" ? true : false;
            var hiddenIsSecuredFlag = $("#hiddenIsSecuredFlag").val().toLowerCase() == "true" ? true : false;
            var hiddenSecureType = $("#hiddenSecureType").val().toLowerCase() == "true" ? true : false;
            if (hiddenAccessSecurePermission == true && hiddenIsSecuredFlag == true) {
                if (hiddenSecureType == true) {
                    modalTitle = SetSecureAssessmentTitle(modalTitle);
                }
            }

            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: '../Record/AssessmentPage.aspx?xID=' + id,
                title: modalTitle,
                maximize: true
            });
            win.remove_beforeClose(win.confirmBeforeClose);
        }

        function goToAssessmentSummary() {
            var headerImg = $('#headerImg').attr('headerImgName');
            var url = '../Dialogues/Assessment/AssessmentSummary.aspx?headerImg=' + headerImg + '&page=addendum';
            var adjustedHeight = Math.ceil($telerik.$(parent.window).height() * 96 / 100);
            var winHeight = adjustedHeight < 625 ? adjustedHeight : 625; //to compensate for clients with very low resolution

            var hiddenAccessSecurePermission = $("#hiddenAccessSecureTesting").val().toLowerCase() == "true" ? true : false;
            var hiddenIsSecuredFlag = $("#hiddenIsSecuredFlag").val().toLowerCase() == "true" ? true : false;
            var hiddenSecureType = $("#hiddenSecureType").val().toLowerCase() == "true" ? true : false;
            var title = "Assessment Criteria Summary";
            if (hiddenAccessSecurePermission == true && hiddenIsSecuredFlag == true) {
                if (hiddenSecureType == true) {
                    title = SetSecureAssessmentTitle(title);
                }
            }

            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: url,
                title: title,
                height: winHeight,
                width: 630
            });
            win.remove_beforeClose(win.confirmBeforeClose);
            win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
        }

        function SelectItems(obj, addendumID) {
            var totalCount = document.getElementById('initialTotalItemCount').value;
            var textBoxId = '';
            $("#standardsTable").find("tr[AddendumID=" + addendumID + "]").find(".contentElementData").each(function (index) {
                if ($(this).find('input').length > 0) {
                    textBoxId = $(this).find('input:eq(0)');
                    if (!($(obj).is(':checked')))
                    {
                        $(textBoxId).attr('value', '');
                        getTotalItemCount(totalCount, textBoxId, false);
                    }
                }
                else {
                    if ($(obj).is(':checked')) {
                        $(textBoxId).attr('value', $.trim($(this).html()));
                        getTotalItemCount(totalCount, textBoxId, false);
                    }
                }
            });
        }
        function ShowSpinner() {
            var overlay = jQuery('<div id="overlay"> </div>');
            overlay.appendTo(document.body);
            document.getElementById("divSpinner").style.display = "block";
        }
        function HideSpinner() {
            $('#overlay').addClass('overlay-disabled');
            document.getElementById("divSpinner").style.display = "none";
        }
    </script> 
</asp:Content>
