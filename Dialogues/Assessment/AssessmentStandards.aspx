<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Dialogues/Assessment/AssessmentDialog.Master"
    CodeBehind="AssessmentStandards.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.AssessmentStandards" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        body {
            font-family: Sans-Serif, Arial;
            font-weight: bold;
        }

        .containerDiv {
            width: 99%;
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

        .labels {
            font-size: 12pt;
            width: 100px;
            text-align: left;
            margin-right: 10px;
            position: relative;
            float: left;
        }

        input {
            text-align: center;
        }

            input.disabled {
                background-color: #E0E0E0;
                border: solid 1px #D0D0D0;
                color: #000;
            }

        .formContainer {
            width: 95%;
            text-align: left;
            margin-top: 10px;
        }

        .headerImg {
            position: absolute;
            left: 10;
            top: 10;
            width: 30px;
        }

        .roundButtons {
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

        .radDropdownBtn {
            font-weight: bold;
            font-size: 11pt;
        }

        .filterButtonsDiv {
            margin-left: 60px;
        }

        .filterButtonDiv {
            position: relative;
            float: left;
            margin-right: 20px;
        }

        #standardsTable {
            width: 100%;
            font-size: 10pt;
        }

        #standardsTable td {
            border: solid 1px #000;
        }
        
        #standardsTable tr td:first-child {
            border-left: solid 0px #000;
        }

        #standardsTable tr td:last-child {
            border-right: solid 0px #000;
        }

        #standardsTable tr:last-child td {
            border-bottom: solid 0px #000;
        }

        #standardsTableHeader th {
            font-weight: bold;
            font-size: 10pt;
        }

            #standardsTableHeader th.alignCellCenter {
                border-top: 0px;
                border-bottom: 0px;
                border-left: solid 1px #000;
                border-right: solid 1px #000;
            }

            #standardsTableHeader th.noBorderRight {
                border-top: 0px;
                border-bottom: 0px;
                border-left: solid 1px #000;
                border-right: 0px;
            }

            #standardsTableHeader th.noBorderLeft {
                border-top: 0px;
                border-bottom: 0px;
                border-left: 0px;
                border-right: solid 1px #000;
            }

        #totalTable {
            margin-bottom: 10px;
            margin-top: 5px;
            width: 100%;
            font-size: 12pt;
            font-weight: bold;
        }

        .alignCellCenter {
            text-align: center;
        }

        .alignCellLeft {
            text-align: left;
        }

        .noBorderRight {
            border-right: 0px !important;
            padding-left: 3px !important;
        }

        .noBorderLeft {
            border-left: 0px !important;
        }

        .tableContainer {
            border: solid 1px #000;
        }

        .tableScroll {
            height: 290px;
            overflow: auto;
        }

        .fullText {
            display: none;
            position: absolute;
            z-index: 9999;
            background-color: #FFF;
            border-top: solid 1px #D0D0D0;
            border-left: solid 1px #D0D0D0;
            border-right: solid 2px #A0A0A0;
            border-bottom: solid 2px #A0A0A0;
            filter: progid:DXImageTransform.Microsoft.Shadow(color='#969696', Direction=135, Strength=3);
            width: 265px;
            top: 0px;
            left: 0px;
            padding: 2px;
        }

        a.standardTextLink {
            color: #000;
            text-decoration: none;
        }

            a.standardTextLink:active {
                color: #000;
                text-decoration: none;
            }

            a.standardTextLink:hover {
                color: #000;
                text-decoration: underline;
            }

            a.standardTextLink:visited {
                color: #000;
                text-decoration: none;
            }

        .itemBankListBoxDiv_Hidden {
            display: none;
        }

        .itemBankListBoxDiv_Visible {
            position: absolute;
            z-index: 2;
            left: 86px;
        }

        .RadComboBox_Web20 .rcbInputCellLeft, .RadComboBox_Web20 .rcbInputCellRight, .RadComboBox_Web20 .rcbArrowCellLeft, .RadComboBox_Web20 .rcbArrowCellRight {
            /*background-image: url('../../Images/rcbSprite.png') !important;*/
        }

        .RadComboBox_Web20 .rcbHovered .rcbInputCell .rcbInput, .RadComboBox_Web20 .rcbFocused .rcbInputCell .rcbInput {
            color: white !important;
        }

        td span input[type="submit"]:disabled {
            color: white !important;
            opacity: 1 !important;
        }

        .RadComboBox_Web20 .rcbMoreResults a {
            /*background-image: url('../../Images/rcbSprite.png') !important;*/
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <img runat="server" id="headerImg" src="../../Images/repairtool.png" alt="Create assessment icon"
        class="headerImg" clientidmode="Static" />
    <div class="containerDiv" align="right">
        <div class="formContainer">
            <div class="filterButtonsDiv">
                <div class="filterButtonDiv">
                    Standards:
                    <telerik:RadComboBox runat="server" ID="standardsSetDropdown" ClientIDMode="Static"
                        Skin="Web20" Text="" AutoPostBack="true" OnSelectedIndexChanged="LoadStandardsTable_Click"
                        Width="100" AccessibilityMode="false" OnClientDropDownOpened="verifyItemBanks" />
                </div>
                <div class="filterButtonDiv" id="itemBankContainer" runat="server" clientidmode="Static">
                    Operational Item Bank(s):
                    <telerik:RadComboBox runat="server" ID="itemBankDropdown" ClientIDMode="AutoID" Skin="Web20" Text="All"
                        CssClass="radDropdownBtn" Width="200" dropdownListID="itemBankList" AutoPostBack="false"
                        EmptyMessage="All" HighlightTemplatedItems="true" OnSelectedIndexChanged="LoadStandardsTable_Selected">
                        <ItemTemplate>
                            <div onclick="StopPropagation(event)">
                                <asp:CheckBox runat="server" ID="itemBankCheckbox" ClientIDMode="AutoID" onclick="evaluateCheckedItem(this)" />
                                <asp:Label runat="server" ID="itemBankLabel" ClientIDMode="AutoID" AssociatedControlID="itemBankCheckbox">
                                    <img src="../../Images/ok.png" onclick="$('#' + this.parentNode.id.replace('itemBankLabel', 'itemBankCheckbox')).click();" style="cursor:pointer;">
                                </asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadComboBox>
                </div>

                <div class="filterButtonDiv" id="ftItemBankContaincer" runat="server" clientidmode="Static">
                    Field Test Item Bank(s):
                    <telerik:RadComboBox runat="server" ID="ftitemBankDropdown" ClientIDMode="AutoID" Skin="Web20" Text="All"
                        CssClass="radDropdownBtn" Width="200" dropdownListID="itemBankList" AutoPostBack="false"
                        EmptyMessage="All" HighlightTemplatedItems="true" OnSelectedIndexChanged="LoadStandardsTable_Selected">
                        <ItemTemplate>
                            <div onclick="StopPropagation(event)">
                                <asp:CheckBox runat="server" ID="ftitemBankCheckbox" ClientIDMode="AutoID" onclick="evaluateftCheckedItem(this)" />
                                <asp:Label runat="server" ID="ftitemBankLabel" ClientIDMode="AutoID" AssociatedControlID="ftitemBankCheckbox">
                                    <img src="../../Images/ok.png" onclick="$('#' + this.parentNode.id.replace('ftitemBankLabel', 'ftitemBankCheckbox')).click();" style="cursor:pointer;">
                                </asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadComboBox>
                </div>
                <br />
                <p style="margin-top: 10px;">
                    Select the standards for the assessment and indicate the number of items described
                    for each.
                </p>
            </div>
            <div class="tableContainer">
                <asp:Table ID="standardsTableHeader" ClientIDMode="Static" Width="100%" BorderWidth="0"
                    CellPadding="2" CellSpacing="0" runat="server">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell CssClass="alignCellCenter noBorderLeft" Width="100">Select<br />Standards</asp:TableHeaderCell>
                        <asp:TableHeaderCell CssClass="alignCellCenter" Width="100">Operational Items </asp:TableHeaderCell>
                        <asp:TableHeaderCell CssClass="alignCellCenter" Width="100">Available<br />Items</asp:TableHeaderCell>
                        <asp:TableHeaderCell CssClass="alignCellCenter" Width="100" ID="thFieldTestItem">Field Test Items </asp:TableHeaderCell>
                        <asp:TableHeaderCell CssClass="alignCellCenter" Width="100" ID="thFieldTestAvail">Available<br />Items</asp:TableHeaderCell>
                        <asp:TableHeaderCell CssClass="alignCellLeft noBorderRight" Width="220">Standard Name</asp:TableHeaderCell>
                        <asp:TableHeaderCell CssClass="alignCellLeft">Standard<br />Text</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
                <div class="tableScroll">
                    <asp:Table ID="standardsTable" ClientIDMode="Static" CellPadding="2"
                        CellSpacing="0" runat="server">
                    </asp:Table>
                </div>
            </div>
            <table id="totalTable" border="0" cellpadding="2" cellspacing="0">
                <tr>
                    <td style="width: 100px; text-align: center;">Total:
                    </td>
                    <td style="width: 100px; text-align: center;">
                        <input runat="server" id="totalSelected" clientidmode="Static" type="text" size="5" value="0" onfocus="this.blur();"
                            class="disabled" />
                    </td>
                    <td style="width: 100px; text-align: center;">
                        <input runat="server" id="totalCount" clientidmode="Static" type="text" size="5" value="0" onfocus="this.blur();"
                            class="disabled" />
                    </td>

                    <td style="width: 100px; text-align: center;">
                        <input runat="server" id="ftTotalSelected" clientidmode="Static" type="text" size="5" value="0" onfocus="this.blur();"
                            class="disabled" />
                    </td>
                    <td style="width: 100px; text-align: center;">
                        <input runat="server" id="ftTotalCount" clientidmode="Static" type="text" size="5" value="0" onfocus="this.blur();"
                            class="disabled" />
                    </td>
                    <td></td>

                    <%--Assessment Summary --%>
                    <td style="text-align: right;">
                        <telerik:RadButton runat="server" ID="summaryButton" ClientIDMode="Static" Skin="Web20"
                            Text="  Summary  " Width="100px" AutoPostBack="false" CssClass="radDropdownBtn"
                            OnClientClicked="loadStandardCounts" xmlHttpPanelID="submitStandardCountsSummaryXmlHttpPanel" />
                    </td>
                </tr>
            </table>

            <asp:Button runat="server" ID="generateButton" ClientIDMode="Static" CssClass="roundButtons_blue"
                Text="&nbsp;&nbsp;Generate Assessment&nbsp;&nbsp;" OnClientClick="this.disabled = true; this.value = 'In Process...'; createAssessment(); return false;" />

            <asp:Button runat="server" ID="nextButton" ClientIDMode="Static" CssClass="roundButtons" Text="&nbsp;&nbsp;Next&nbsp;&nbsp;" 
                  OnClientClick="this.disabled = true; this.value = 'In Process...'; loadStandardCounts(null, null, {obj: this}); return false;" disabled="disabled" xmlHttpPanelID="submitStandardCountsNextButtonXmlHttpPanel"/>

            <asp:Button runat="server" ID="backButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Back&nbsp;&nbsp;" OnClientClick="loadStandardCounts(null, null, {obj: this}); return false;"
                xmlHttpPanelID="submitStandardCountsBackButtonXmlHttpPanel" />

            <asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="setTimeout(function() { getCurrentCustomDialog().close(); }, 0); return false;" />

            <input runat="server" type="hidden" id="assessmentID" clientidmode="Static" name="assessmentID" value="" />
            <input runat="server" type="hidden" id="standardsCountList" clientidmode="Static" name="standardsCountList" value="" />
            <input runat="server" type="hidden" id="newAssessmentTitle" clientidmode="Static" name="newAssessmentTitle" value="" />
            <input runat="server" type="hidden" id="assessmentContentType" clientidmode="Static" name="assessmentContentType" value="" />
            <input runat="server" type="hidden" id="hiddenStandard" clientidmode="Static" name="hiddenStandard" value="" />
            <input runat="server" type="hidden" id="assessmentSelection" clientidmode="Static" name="assessmentSelection" value="" />
            <input runat="server" type="hidden" id="hiddenAccessSecureTesting" clientidmode="Static" name="hiddenAccessSecureTesting" value="" />
            <input runat="server" type="hidden" id="hiddenIsSecuredFlag" clientidmode="Static" name="hiddenIsSecuredFlag" value="" />
            <input runat="server" type="hidden" id="hiddenSecureType" clientidmode="Static" name="hiddenSecureType" value="" />
        </div>
    </div>
    <span style="display: none;">

        <telerik:RadXmlHttpPanel runat="server" ID="submitXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="RequestNewAssessmentID" RenderMode="Block" OnClientResponseEnding="goToNewAssessment"
            objectToLoadID="assessmentID">
        </telerik:RadXmlHttpPanel>

        <telerik:RadXmlHttpPanel runat="server" ID="submitStandardCountsSummaryXmlHttpPanel"
            ClientIDMode="Static" Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="StoreStandardCountsAndNames" RenderMode="Block" OnClientResponseEnding="goToAssessmentSummary"
            objectToLoadID="assessmentID">
        </telerik:RadXmlHttpPanel>

        <telerik:RadXmlHttpPanel runat="server" ID="submitStandardCountsBackButtonXmlHttpPanel"
            ClientIDMode="Static" Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="StoreStandardCountsAndNames" RenderMode="Block" OnClientResponseEnding="goBackToAssessmentIdentification"
            objectToLoadID="assessmentID">
        </telerik:RadXmlHttpPanel>

        <telerik:RadXmlHttpPanel runat="server" ID="updateItemTotalsXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="StoreStandardCount" RenderMode="Block" objectToLoadID="assessmentID">
        </telerik:RadXmlHttpPanel>

        <telerik:RadXmlHttpPanel runat="server" ID="submitStandardCountsNextButtonXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="StoreStandardCountsAndNames" RenderMode="Block" OnClientResponseEnding="goToAssessmentWizardOptions"
            objectToLoadID="assessmentID">
        </telerik:RadXmlHttpPanel>

    </span>
    <script type="text/javascript">
        $( document ).ready(function() {
            calculateTotalItems();
        });

        $('body').click(function (e) {
            hideItemBankList(e);
        });


        function StopPropagation(e) {
            //cancel bubbling
            e.cancelBubble = true;
            if (e.stopPropagation) {
                e.stopPropagation();
            }
        }

        function assessmentStandardsAlert(message, width, height) {
            var wnd = window.radalert(message, width, height);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
        }

        function verifyItemBanks(sender, args) {
            if ($('#itemBankContainer').css('display') == 'none') return true;
            var itemBankDropdown = $find("<%= itemBankDropdown.ClientID %>");
            var anyItemsChecked = false;
            var checkbox;
            var labelValue;
            var items = itemBankDropdown.get_items();

            for (var i = 1; i < items.get_count() ; i++) {
                checkbox = $get(itemBankDropdown.get_id() + '_i' + i + '_itemBankCheckbox');
                labelValue = $get(itemBankDropdown.get_id() + '_i' + i + '_itemBankLabel').innerHTML;
                if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                if (checkbox.checked) anyItemsChecked = true;
            }

            if (!anyItemsChecked) {
                assessmentStandardsAlert('Please select an item bank.', 300, 100);
                $find('standardsSetDropdown').hideDropDown();
            }
        }

        function evaluateSelectedItem(sender, args) {
            var item = args.get_item();
            var itemIsCheckable = item.get_checkable();
            var itemChecked = item.get_checked();
            var itemValue = item.get_value();
            var itemsList = sender._children._array;
            var allItemsChecked = true;
            var itemsCheckedCount = 0;
            var itemBankButton = $find('itemBankButton');

            if (itemIsCheckable) {
                if (itemChecked) {
                    item.uncheck();
                    itemChecked = false;
                }
                else {
                    item.check();
                    itemChecked = true;
                }
            }

            if (!itemIsCheckable) {
                var anyItemsChecked = false;
                for (var i = 1; i < itemsList.length; i++) {
                    if (!itemsList[i].get_checkable()) continue;

                    if (itemsList[i].get_checked()) {
                        anyItemsChecked = true;
                        break;
                    }
                    else {
                        itemsCheckedCount++;
                    }
                }

                if (anyItemsChecked) {
                    __doPostBack('itemBankList');
                }
                else {
                    assessmentStandardsAlert('Please select an item bank.', 300, 100);
                }

                return false;
            }

            if (itemChecked && itemValue == 'All') {
                //sender.checkItems(itemsList);
                for (var i = 1; i < itemsList.length; i++) {
                    if (!itemsList[i].get_checkable()) continue;
                    itemsList[i].check();
                }

                itemBankButton.set_text('All');
            }
            else if ((itemValue == 'All' || itemValue == '<Select One>') && !itemChecked) {
                //sender.uncheckItems(itemsList);
                for (var i = 1; i < itemsList.length; i++) {
                    if (!itemsList[i].get_checkable()) continue;
                    itemsList[i].uncheck();
                }

                itemBankButton.set_text('<Select One>');
            }
            else {
                for (var i = 1; i < itemsList.length; i++) {
                    if (!itemsList[i].get_checkable()) continue;

                    if (!itemsList[i].get_checked()) {
                        allItemsChecked = false;
                    }
                    else {
                        itemsCheckedCount++;
                    }
                }

                if (allItemsChecked) {
                    itemsList[0].check();
                    itemBankButton.set_text('All');
                }
                else {
                    itemsList[0].uncheck();

                    switch (itemsCheckedCount) {
                        case 0:
                            itemBankButton.set_text('<Select One>');
                            break;
                        case 1:
                            for (var i = 1; i < itemsList.length; i++) {
                                if (!itemsList[i].get_checkable()) continue;

                                if (itemsList[i].get_checked()) {
                                    itemBankButton.set_text(itemsList[i].get_value());
                                }
                            }
                            break;
                        default:
                            itemBankButton.set_text('Multiple');
                            break;
                    }
                }
            }
        }

        var reloadingTable = false;
        function evaluateCheckedItem(item) {
            var itemBankDropdown = $find("<%= itemBankDropdown.ClientID %>");
            var itemChecked = item.checked;
            var itemValue = $get(item.id.replace('itemBankCheckbox', 'itemBankLabel')).innerHTML;
            var allItemsChecked = true;
            var itemsCheckedCount = 0;
            var checkbox;
            var labelValue;
            var items = itemBankDropdown.get_items();

            if (itemChecked && itemValue == 'All') {
                for (var i = 1; i < items.get_count() ; i++) {
                    checkbox = $get(itemBankDropdown.get_id() + '_i' + i + '_itemBankCheckbox');
                    labelValue = $get(itemBankDropdown.get_id() + '_i' + i + '_itemBankLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;
                    checkbox.checked = true;
                }

                itemBankDropdown.set_text('All');
            }
            else if ((itemValue == 'All' || itemValue == '<Select One>') && !itemChecked) {
                for (var i = 1; i < items.get_count() ; i++) {
                    checkbox = $get(itemBankDropdown.get_id() + '_i' + i + '_itemBankCheckbox');
                    labelValue = $get(itemBankDropdown.get_id() + '_i' + i + '_itemBankLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;
                    checkbox.checked = false;
                }

                itemBankDropdown.set_text('<Select One>');
            }
            else {
                for (var i = 1; i < items.get_count() ; i++) {
                    checkbox = $get(itemBankDropdown.get_id() + '_i' + i + '_itemBankCheckbox');
                    labelValue = $get(itemBankDropdown.get_id() + '_i' + i + '_itemBankLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                    if (!checkbox.checked) {
                        allItemsChecked = false;
                    }
                    else {
                        itemsCheckedCount++;
                    }
                }

                if (allItemsChecked) {
                    checkbox = $get(itemBankDropdown.get_id() + '_i0_itemBankCheckbox');
                    checkbox.checked = true;
                    itemBankDropdown.set_text('All');
                }
                else {
                    checkbox = $get(itemBankDropdown.get_id() + '_i0_itemBankCheckbox');
                    checkbox.checked = false;

                    switch (itemsCheckedCount) {
                        case 0:
                            itemBankDropdown.set_text('<Select One>');
                            break;
                        case 1:
                            for (var i = 1; i < items.get_count() ; i++) {
                                checkbox = $get(itemBankDropdown.get_id() + '_i' + i + '_itemBankCheckbox');
                                labelValue = $get(itemBankDropdown.get_id() + '_i' + i + '_itemBankLabel').innerHTML;
                                if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                                if (checkbox.checked) {
                                    var labelValue = $get(checkbox.id.replace('itemBankCheckbox', 'itemBankLabel')).innerHTML;
                                    itemBankDropdown.set_text(labelValue);
                                }
                            }
                            break;
                        default:
                            itemBankDropdown.set_text('Multiple');
                            break;
                    }
                }
            }

            if (itemValue.toLowerCase().indexOf('<img') > -1) {
                if (allItemsChecked || itemsCheckedCount > 0) {
                    if (reloadingTable) return false;
                    reloadingTable = true;
                    __doPostBack("<%= itemBankDropdown.ClientID %>");
                }
                else {
                    assessmentStandardsAlert('Please select an item bank.', 300, 100);
                }
            }
        }

        function evaluateftCheckedItem(item) {
            var ftitemBankDropdown = $find("<%= ftitemBankDropdown.ClientID %>");
            var itemChecked = item.checked;
            var itemValue = $get(item.id.replace('ftitemBankCheckbox', 'ftitemBankLabel')).innerHTML;
            var allItemsChecked = true;
            var itemsCheckedCount = 0;
            var checkbox;
            var labelValue;
            var items = ftitemBankDropdown.get_items();

            if (itemChecked && itemValue == 'All') {
                for (var i = 1; i < items.get_count() ; i++) {
                    checkbox = $get(ftitemBankDropdown.get_id() + '_i' + i + '_ftitemBankCheckbox');
                    labelValue = $get(ftitemBankDropdown.get_id() + '_i' + i + '_ftitemBankLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;
                    checkbox.checked = true;
                }

                ftitemBankDropdown.set_text('All');
            }
            else if ((itemValue == 'All' || itemValue == '<Select One>') && !itemChecked) {
                for (var i = 1; i < items.get_count() ; i++) {
                    checkbox = $get(ftitemBankDropdown.get_id() + '_i' + i + '_ftitemBankCheckbox');
                    labelValue = $get(ftitemBankDropdown.get_id() + '_i' + i + '_ftitemBankLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;
                    checkbox.checked = false;
                }

                ftitemBankDropdown.set_text('<Select One>');
            }
            else {
                for (var i = 1; i < items.get_count() ; i++) {
                    checkbox = $get(ftitemBankDropdown.get_id() + '_i' + i + '_ftitemBankCheckbox');
                    labelValue = $get(ftitemBankDropdown.get_id() + '_i' + i + '_ftitemBankLabel').innerHTML;
                    if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                    if (!checkbox.checked) {
                        allItemsChecked = false;
                    }
                    else {
                        itemsCheckedCount++;
                    }
                }

                if (allItemsChecked) {
                    checkbox = $get(ftitemBankDropdown.get_id() + '_i0_ftitemBankCheckbox');
                    checkbox.checked = true;
                    ftitemBankDropdown.set_text('All');
                }
                else {
                    checkbox = $get(ftitemBankDropdown.get_id() + '_i0_ftitemBankCheckbox');
                    checkbox.checked = false;

                    switch (itemsCheckedCount) {
                        case 0:
                            ftitemBankDropdown.set_text('<Select One>');
                            break;
                        case 1:
                            for (var i = 1; i < items.get_count() ; i++) {
                                checkbox = $get(ftitemBankDropdown.get_id() + '_i' + i + '_ftitemBankCheckbox');
                                labelValue = $get(ftitemBankDropdown.get_id() + '_i' + i + '_ftitemBankLabel').innerHTML;
                                if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                                if (checkbox.checked) {
                                    var labelValue = $get(checkbox.id.replace('ftitemBankCheckbox', 'ftitemBankLabel')).innerHTML;
                                    ftitemBankDropdown.set_text(labelValue);
                                }
                            }
                            break;
                        default:
                            ftitemBankDropdown.set_text('Multiple');
                            break;
                    }
                }
            }

            if (itemValue.toLowerCase().indexOf('<img') > -1) {
                if (allItemsChecked || itemsCheckedCount > 0) {
                    if (reloadingTable) return false;
                    reloadingTable = true;
                    __doPostBack("<%= ftitemBankDropdown.ClientID %>");
                }
                else {
                    assessmentStandardsAlert('Please select an item bank.', 300, 100);
                }
            }
        }


        function selectStandard(id, add, input, selected) {
            var itemsEnteredInput = input ? input : document.getElementById('itemsEnteredInput_' + id);
            var standardsCheckbox = input ? document.getElementById('standardsCheckbox_' + id) : null;

            if (input) {
                if (input.value == '' || input.value == '0') {
                    if (standardsCheckbox) standardsCheckbox.checked = false;
                }
                else if (!isNaN(input.value)) {
                    if (itemsEnteredInput.value == '') {
                        itemsEnteredInput.value = '1';
                    }

                    if (standardsCheckbox) standardsCheckbox.checked = true;
                }
            }
            else {
                if (add) {
                    if (itemsEnteredInput.value == '') {
                        itemsEnteredInput.value = '1';
                    }
                }
                else {
                    itemsEnteredInput.value = '';
                }
            }

            calculateTotalItems();
            updateItemTotal(id, input);
        }

        function selectFTStandard(id, add, input, selected) {
            var itemsEnteredInput = input ? input : document.getElementById('ftitemsEnteredInput_' + id);
            var standardsCheckbox = input ? document.getElementById('standardsCheckbox_' + id) : null;

            if (input) {
                if (input.value == '' || input.value == '0') {
                    if (standardsCheckbox) standardsCheckbox.checked = false;
                }
                else if (!isNaN(input.value)) {
                    if (itemsEnteredInput.value == '') {
                        itemsEnteredInput.value = '1';
                    }

                    if (standardsCheckbox) standardsCheckbox.checked = true;
                }
            }
            else {
                if (add) {
                    if (itemsEnteredInput.value == '') {
                        itemsEnteredInput.value = '1';
                    }
                }
                else {
                    itemsEnteredInput.value = '';
                }
            }

            calculateTotalFTItems();
            updateFTItemTotal(id, input);
        }

        function updateItemTotal(id, input) {
            if (!input) input = document.getElementById('itemsEnteredInput_' + id);
            var panel = $find('updateItemTotalsXmlHttpPanel');
            var standardsSetDropdown = $find('standardsSetDropdown').get_element();
            var standardsSetValue = standardsSetDropdown.value;
            var standardsTable = $('#standardsTable');
            var itemCount = (input.value == '' ? 0 : input.value);
            var totalCountAvail = parseInt($('#standardsCountLabel_' + id).text());
            var blankItemCount = (itemCount > totalCountAvail ? itemCount - totalCountAvail : 0);
            var rigor = $('[id="rigor_' + id + '"]', standardsTable).val();

            var panelValue = '{"StandardID":' + id + ', "TotalItemCount":' + itemCount + ', "StandardSet":"' + standardsSetValue +
                '", "StandardName":"' + $('a[id^="itemName_' + id + '"]', standardsTable).html() + '", "BlankItemCount":"' + blankItemCount + '", "RigorLevelCounts":[' + rigor + '] }';

            panel.set_value(panelValue);
        }

        function updateFTItemTotal(id, input) {
            if (!input) input = document.getElementById('ftitemsEnteredInput_' + id);
            var panel = $find('updateItemTotalsXmlHttpPanel');
            var standardsSetDropdown = $find('standardsSetDropdown').get_element();
            var standardsSetValue = standardsSetDropdown.value;
            var standardsTable = $('#standardsTable');
            var itemCount = (input.value == '' ? 0 : input.value);
            var totalCountAvail = parseInt($find('ftstandardsCountButton_' + id).get_text());
            var blankItemCount = (itemCount > totalCountAvail ? itemCount - totalCountAvail : 0);

            var panelValue = '{"StandardID":' + id + ', "TotalItemCount":' + itemCount + ', "StandardSet":"' + standardsSetValue +
                '", "StandardName":"' + $('a[id^="itemName_' + id + '"]', standardsTable).html() + '", "BlankItemCount":"' + blankItemCount + '"}';

            panel.set_value(panelValue);
        }

        function calculateTotalItems() {
            var totalItemCount = 0;
            var standardsTable = $('#standardsTable');

            $('input[id^="itemsEnteredInput"]', standardsTable).each(function (index) {
                if ($(this).attr('value') != '') totalItemCount += parseInt($(this).attr('value'));
            });

            $('#totalSelected').attr('value', totalItemCount);

            if (totalItemCount > 0) {
                $('#nextButton').removeAttr("disabled");
            }
            else {
                $('#nextButton').attr('disabled', 'disabled');
            }
        }


        function calculateTotalFTItems() {
            var totalItemCount = 0;
            var standardsTable = $('#standardsTable');

            $('input[id^="ftitemsEnteredInput"]', standardsTable).each(function (index) {
                if ($(this).attr('value') != '') totalItemCount += parseInt($(this).attr('value'));
            });

            $('#ftTotalSelected').attr('value', totalItemCount);
        }

        function onlyNumsLessOrEqualToCount(id, obj, e) {
            var standardsCountButtonText = $('#standardsCountLabel_' + id).text();
            var existingValue = obj.value.length == 0 || isNaN(obj.value) ? '' : obj.value;
            var caretStart = obj.selectionStart;
            var caretEnd = obj.selectionEnd;
            var keynum = '';
            var keychar = '';
            var numcheck = '';
            var notRange = true;

            standardsCountButtonText = standardsCountButtonText.length == 0 || isNaN(standardsCountButtonText) ? 0 : parseInt(standardsCountButtonText);

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

            if (keynum == '' || keynum == 46 || keynum == 8) return true;

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

            //if (!isNaN(keychar) && parseInt(keychar) <= standardsCountButtonText) {
            if (!isNaN(keychar)) {
                obj.value = parseInt(keychar) == 0 ? '' : (Math.round(keychar) == 0 ? '' : Math.round(keychar));
                if (typeof (obj.selectionStart) != 'undefined') obj.selectionStart = obj.selectionEnd;
                else {
                    obj.onfocus = function () { this.value = this.value; this.onfocus = null; };
                    obj.blur();
                    obj.focus();
                }
                selectStandard(id, null, obj);
                calculateTotalItems();
            }

            return false;
        }

        function FTonlyNumsLessOrEqualToCount(id, obj, e) {

            var standardsCountButtonText = $find('ftstandardsCountButton_' + id).get_text();
            var existingValue = obj.value.length == 0 || isNaN(obj.value) ? '' : obj.value;
            var caretStart = obj.selectionStart;
            var caretEnd = obj.selectionEnd;
            var keynum = '';
            var keychar = '';
            var numcheck = '';
            var notRange = true;

            standardsCountButtonText = standardsCountButtonText.length == 0 || isNaN(standardsCountButtonText) ? 0 : parseInt(standardsCountButtonText);

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

            //if (!isNaN(keychar) && parseInt(keychar) <= standardsCountButtonText) {
            if (!isNaN(keychar)) {
                obj.value = parseInt(keychar) == 0 ? '' : (Math.round(keychar) == 0 ? '' : Math.round(keychar));
                if (typeof (obj.selectionStart) != 'undefined') obj.selectionStart = obj.selectionEnd;
                else {
                    obj.onfocus = function () { this.value = this.value; this.onfocus = null; };
                    obj.blur();
                    obj.focus();
                }
                selectFTStandard(id, null, obj);
                calculateTotalFTItems();
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

        function getCaret(el) {
            if (el.selectionStart) {
                return el.selectionStart;
            } else if (document.selection) {
                el.focus();

                var r = document.selection.createRange();
                if (r == null) {
                    return 0;
                }

                var re = el.createTextRange(),
                rc = re.duplicate();
                re.moveToBookmark(r.getBookmark());
                rc.setEndPoint('EndToStart', re);

                return rc.text.length;
            }
            return 0;
        }

        function deleteAndBackspace_onKeyup(id, obj, e) {
            if (window.event) {// IE
                keynum = e.keyCode;
            }
            else if (e.which) {// Netscape/Firefox/Opera
                keynum = e.which;
            }

            if (keynum == 46 || keynum == 8) {
                selectStandard(id, null, obj);
            }
        }

        function ResetGenerateButton() {
            var btnGenerate = document.getElementById('generateButton');
            btnGenerate.disabled = false;
            btnGenerate.value = "  Generate Assessment  ";
        }

        function ResetNextButton() {
            var btnNext = document.getElementById('nextButton');
            btnNext.disabled = false;
            btnNext.value = "  Next  ";
        }

        function createAssessment() {
            var standardsCountList = '';
            var standardsTable = $('#standardsTable');
            var panel = $find('submitXmlHttpPanel');
            var panelValue;
            var panelValue_standardIDs = '';
            var panelValue_standardCounts = '';
            var isBlankItems = false;

            $('input[id^="itemsEnteredInput"]', standardsTable).each(function (index) {
                var standardID = $(this).attr('id').replace('itemsEnteredInput_', '');
                var totalCountAvail = $('#standardsCountLabel_' + standardID).text();
                var count = $(this).attr('value') != '' ? $(this).attr('value') : 0;

                if (count > 0) {
                    if (parseInt(count) > parseInt(totalCountAvail)) isBlankItems = true;

                    standardsCountList += '@' + standardID + '=' + count + ';@';
                    panelValue_standardIDs += standardID + ',';
                    panelValue_standardCounts += count + ',';
                }
            });

            panelValue_standardIDs = panelValue_standardIDs.substr(0, panelValue_standardIDs.lastIndexOf(','));
            panelValue_standardCounts = panelValue_standardCounts.substr(0, panelValue_standardCounts.lastIndexOf(','));

            if (standardsCountList != '') {
                if (isBlankItems && $('#assessmentContentType').attr('value') != 'External') {
                    function callbackFunction(arg) {
                        if (arg) {
                            panelValue = '{"StandardIDs":[' + panelValue_standardIDs + '], "StandardCounts":[' + panelValue_standardCounts + '], "StandardsList":"' + standardsCountList + '"}';
                            panel.set_value(panelValue);
                        }
                        else {
                            ResetGenerateButton();
                        }
                    }

                    var confirmDialogText = 'You have selected more items for a standard that is available in the item bank. This will create a blank item(s) on the assessment. Do you want to continue?';
                    var wnd = window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Confirm Standard Selections');
                    wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                }
                else {
                    panelValue = '{"StandardIDs":[' + panelValue_standardIDs + '], "StandardCounts":[' + panelValue_standardCounts + ']}';
                    panel.set_value(panelValue);
                }
            }
            else {
                assessmentStandardsAlert('Please select the standards and number of items you wish to see on your assessment before proceeding.', 300, 100);
                ResetGenerateButton();
            }
        }

        function goToAssessmentWizardOptions(sender, args) {
            var assessmentSelection = document.getElementById('assessmentSelection').value;
            var headerImg = $('#headerImg').attr('headerImgName');
            var allMax = true;
            if (navigator.appVersion.indexOf("Win") == -1) {
                allMax = false;
            }
            //var isSecure = '<%= Request.QueryString["isSecure"] %>';
            
            var hiddenAccessSecurePermission = $("#hiddenAccessSecureTesting").val().toLowerCase() == "true" ? true : false;
            var hiddenIsSecuredFlag = $("#hiddenIsSecuredFlag").val().toLowerCase() == "true" ? true : false;
            var hiddenSecureType = $("#hiddenSecureType").val().toLowerCase() == "true" ? true : false;

            if (assessmentSelection == 'Rigor') {
                var title = "Assessment Item Rigor Selections";
                //if (isSecure == 'true') {
                //    title = "[SECURE] Assessment Item Rigor Selections";
                //}
                var isSecure = false;
                if (hiddenAccessSecurePermission == true && hiddenIsSecuredFlag == true) {
                    if (hiddenSecureType == true) {
                        isSecure = true;
                        title = SetSecureAssessmentTitle(title);
                    }
                }
                setTimeout(function () {
                    var url = '../Dialogues/Assessment/AssessmentStandardsDetail.aspx?headerImg=' + headerImg + '&isSecure=' + isSecure;
                    var win = parent.customDialog({
                        name: 'RadWindowAddAssessment',
                        url: url,
                        title: title,
                        maximize: true,
                        maxwidth: 1200,
                        maxheight: 550,
                        resizable: false,
                        movable: true,
                        center: true,
                        maximizable: allMax,
                    });
                    win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
                }, 1000);
            }
            else if (assessmentSelection == 'Addendum') {
                var title = "Addendum Selection";
                //if (isSecure == 'true') {
                //    title = "[SECURE] Addendum Selection";
                //}
                var isSecure = false;
                if (hiddenAccessSecurePermission == true && hiddenIsSecuredFlag == true) {
                    if (hiddenSecureType == true) {
                        isSecure = true;
                        title = SetSecureAssessmentTitle(title);
                    }
                }
                setTimeout(function () {
                    var url = '../Dialogues/Assessment/AssessmentAddendumsDetail.aspx?headerImg=' + headerImg + '&isSecure=' + isSecure;
                    var win = parent.customDialog({
                        name: 'RadWindowAddAssessment',
                        url: url,
                        title: title,
                        maximize: true,
                        maxwidth: 1200,
                        maxheight: 550,
                        resizable: false,
                        movable: true,
                        center: true,
                        maximizable: allMax,
                    });

                    win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
                }, 1000);
            }
        }

          function goToNewAssessment(sender, args) {
              var result = args.get_content();
              var modalTitle = document.getElementById('newAssessmentTitle').value;

              parent.customDialog({
                  name: 'RadWindowAddAssessment',
                  url: '../Record/AssessmentPage.aspx?xID=' + result,
                  title: modalTitle,
                  maximize: true
              });
          }

          function displayFullDescription(obj) {
              var spanContainer = obj.parentNode;
              var fullTextSpan = $('.fullText', obj.parentNode);
              fullTextSpan.css('display', 'inline');
          }

          function loadStandardCounts(sender, args, data) {
              var senderElement = (sender ? sender.get_element() : data.obj);
              var xmlHttpPanelID = senderElement.getAttribute('xmlHttpPanelID');
              var panel = $find(xmlHttpPanelID);
              var panelObj = panel.get_element();
              var panelValue = '{"StandardIDs":[';
              var panelValue_standardIDs = '';
              var panelValue_standardCounts = '';
              var panelValue_standardNames = '';
              var panelValue_blankItemCounts = '';
              var standardsTable = $('#standardsTable');
              var standardsSetDropdown = $find('standardsSetDropdown').get_element();
              var standardsSetValue = standardsSetDropdown.value;

              if (data) {
                  panelObj.setAttribute('standardID', data.standardID);
                  panelObj.setAttribute('standardName', data.standardName);
                  panelObj.setAttribute('itemCountInput', data.itemCountInput);
                  panelObj.setAttribute('headerImg', data.headerImg);
                  panelObj.setAttribute('standardSet', data.standardSet);
                  panelObj.setAttribute('blueprintID', data.blueprintID);
              }

              $('input[id^="itemsEnteredInput"]', standardsTable).each(function (index) {
                  var standardID = $(this).attr('id').replace('itemsEnteredInput_', '');
                  var totalCountAvail = $('#standardsCountLabel_' + standardID).text();
                  var count = $(this).attr('value') != '' ? $(this).attr('value') : 0;

                  if (count > 0) {
                      if (parseInt(count) > parseInt(totalCountAvail)) {
                          panelValue_blankItemCounts += '"' + standardID + '|' + (parseInt(count) - parseInt(totalCountAvail)) + '",';
                      }
                      panelValue_standardIDs += standardID + ',';
                      panelValue_standardCounts += count + ',';
                      panelValue_standardNames += '"' + $('a[id^="itemName_' + standardID + '"]', standardsTable).html() + '",';
                  }
              });

              panelValue_standardIDs = panelValue_standardIDs.substr(0, panelValue_standardIDs.lastIndexOf(','));
              panelValue_standardCounts = panelValue_standardCounts.substr(0, panelValue_standardCounts.lastIndexOf(','));
              panelValue_standardNames = panelValue_standardNames.substr(0, panelValue_standardNames.lastIndexOf(','));
              panelValue_blankItemCounts = panelValue_blankItemCounts.substr(0, panelValue_blankItemCounts.lastIndexOf(','));

              if (panelValue_blankItemCounts.length > 0 && xmlHttpPanelID == 'submitStandardCountsNextButtonXmlHttpPanel')
              {
                  function callbackFunction(arg) {
                      if (arg) {
                          panelValue += panelValue_standardIDs + '], "StandardCounts":[' + panelValue_standardCounts + '], "StandardNames":[' + panelValue_standardNames + '], "StandardSet":"' + standardsSetValue + '", '
                            + '"BlankItemCounts":[' + panelValue_blankItemCounts + ']}';
                          panel.set_value(panelValue);
                      }
                      else
                      {
                          ResetNextButton();
                  }
                  }

                  var confirmDialogText = 'You have selected more items for a standard that is available in the item bank. This will create a blank item(s) on the assessment. Do you want to continue?';
                  var wnd = window.radconfirm(confirmDialogText, callbackFunction, 300, 200, null, 'Confirm Standard Selections');
                  wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
              }
              else {
                  panelValue += panelValue_standardIDs + '], "StandardCounts":[' + panelValue_standardCounts + '], "StandardNames":[' + panelValue_standardNames + '], "StandardSet":"' + standardsSetValue + '", '
                           + '"BlankItemCounts":[' + panelValue_blankItemCounts + ']}';
                  panel.set_value(panelValue);
              }
          }

          function goToAssessmentSummary(sender, args) {
              var standardsCountList = '';
              var isBlankItems = false;
              $('input[id^="itemsEnteredInput"]', standardsTable).each(function (index) {
                  var standardID = $(this).attr('id').replace('itemsEnteredInput_', '');
                  var totalCountAvail = $('#standardsCountLabel_' + standardID).text();
                  var count = $(this).attr('value') != '' ? $(this).attr('value') : 0;

                  if (count > 0) {
                      if (parseInt(count) > parseInt(totalCountAvail)) isBlankItems = true;

                      standardsCountList += '@' + standardID + '=' + count + ';@';
                  }
              });
              if (standardsCountList == '') {
                  assessmentStandardsAlert('Please select the standards and number of items you wish to see on your assessment before proceeding.', 300, 100);
                  ResetGenerateButton();
                  return false;
              }


              var headerImg = $('#headerImg').attr('headerImgName');
              var hiddenAccessSecurePermission = $("#hiddenAccessSecureTesting").val().toLowerCase() == "true" ? true : false;
              var hiddenIsSecuredFlag = $("#hiddenIsSecuredFlag").val().toLowerCase() == "true" ? true : false;
              var hiddenSecureType = $("#hiddenSecureType").val().toLowerCase() == "true" ? true : false;
              var dialogTitle = "Assessment Criteria Summary";
              if (hiddenAccessSecurePermission == true && hiddenIsSecuredFlag==true)
              {
                  if(hiddenSecureType==true)
                  {
                      //dialogTitle = "[SECURE] Assessment Criteria Summary";
                      dialogTitle = SetSecureAssessmentTitle(dialogTitle);
                  }
              }


              var url = '../Dialogues/Assessment/AssessmentSummary.aspx?headerImg=' + headerImg;
              var adjustedHeight = Math.ceil($telerik.$(parent.window).height() * 96 / 100);
              var winHeight = adjustedHeight < 625 ? adjustedHeight : 625; //to compensate for clients with very low resolution

              var win = parent.customDialog({
                  name: 'RadWindowAddAssessment',
                  url: url,
                  title: dialogTitle,
                  maximize: true, maxwidth: 800
              });

              win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
          }

          function goBackToAssessmentIdentification(sender, args) {
              var headerImg = $('#headerImg').attr('headerImgName');

              var win = parent.customDialog({
                  name: 'RadWindowAddAssessment',
                  url: '../Dialogues/Assessment/CreateAssessmentIdentification.aspx?headerImg=' + headerImg,
                  title: 'Assessment Identification',
                  maximize: true, maxwidth: 650, maxheight: 650
              });

              win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
          }

          function hideItemBankList(e) {
              var target = e ? (e.target ? e.target : e.srcElement) : window.event ? (window.event.target ? window.event.target : window.event.srcElement) : null;

              if (target._itemTypeName && target._itemTypeName == 'Telerik.Web.UI.RadListBoxItem') return;
              if (target.className && (target.className == 'rlbCheck' || target.className == 'rlbText' || target.className == 'rlbImage')) return;

              $('#itemBankListBoxDiv').attr('class', 'itemBankListBoxDiv_Hidden');
          }

    </script>
</asp:Content>
