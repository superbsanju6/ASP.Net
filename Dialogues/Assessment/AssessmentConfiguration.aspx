<%@ page language="C#" autoeventwireup="true" masterpagefile="~/Dialogues/Assessment/AssessmentDialog.Master"
    codebehind="AssessmentConfiguration.aspx.cs" inherits="Thinkgate.Dialogues.Assessment.AssessmentConfiguration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        body {
            font-family: Sans-Serif, Arial;
            position: relative;
            font-size: .8em;
        }

        .containerDiv {
            width: 99%;
        }

        .headerDiv {
            margin-bottom: 10px;
            /*margin-left: 37px;*/
            font-weight: bold;
        }
        /*#RadWindowWrapper_RadWindow1Url {
            width: 1250px !important;
        }*/

        p {
            font-style: italic;
        }

        .formContainer {
            width: 95%;
            text-align: left;
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
            border: solid 1px #000;
            border-radius: 50px;
            float: right;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
        }

        .RadComboBox_Web20 .rcbInputCellLeft, .RadComboBox_Web20 .rcbInputCellRight, .RadComboBox_Web20 .rcbArrowCellLeft, .RadComboBox_Web20 .rcbArrowCellRight {
            /*background-image: url('../../Images/rcbSprite.png') !important;*/
        }

        .RadComboBox_Web20 .rcbHovered .rcbInputCell .rcbInput, .RadComboBox_Web20 .rcbFocused .rcbInputCell .rcbInput {
            color: #fff !important;
        }

        .RadComboBox_Web20 .rcbMoreResults a {
            /*background-image: url('../../Images/rcbSprite.png') !important;*/
        }

        table thead {
            font-weight: bold;
            text-align: center;
        }

        .configTable {
            margin: 5px;
            border: solid 1px rgb(153, 102, 0);
        }

            .configTable td {
                border: solid 1px rgb(153, 102, 0);
                padding-top: 2px;
                padding-bottom: 2px;
                padding-left: 15px;
                padding-right: 15px;
                white-space: nowrap;
                vertical-align: middle;
            }

        .additionalDataTable {
            margin: 5px;
        }

            .additionalDataTable td {
                padding-top: 2px;
                padding-bottom: 2px;
                padding-left: 15px;
                padding-right: 15px;
                white-space: nowrap;
                vertical-align: middle;
            }

        .inputLabels {
            margin-left: 5px;
            margin-right: 15px;
        }

        .inputCheckboxes {
            margin-right: 5px;
        }

        .freeFormInputs {
            border: solid 1px #888;
            padding: 2px;
            filter: progid:DXImageTransform.Microsoft.Shadow(color='#969696', Direction=135, Strength=3);
            line-height: 15px;
        }

        .inputText_default {
            color: #888;
            font-style: italic;
            border: solid 1px #000;
            padding: 2px;
        }

        .inputText_entered {
            color: #000;
            border: solid 1px #000;
            padding: 2px;
        }

        .dashedLine {
            border: dashed 2px #000;
        }

        .largeLabel {
            color: rgb(153, 102, 0);
            font-size: 12pt;
            font-style: italic;
        }

        .hiddenUpdateButton {
            display: none;
        }

        #assessmentTitle {
            text-align: center;
        }

        .RadTabStripTop_Thinkgate_Blue .rtsLevel {
            background-color: #DFE9F5;
        }

        .configTable td table td {
            border: 0px;
        }

        #fieldsetTable {
            margin-bottom: 20px;
            width: 575px;
            border: 0px;
        }

        #fieldsetTableFirst {
            font-family: Tahoma, Arial, Helvetica, sans-serif;
            color: #3b3b3b !important;
            font-size: 12px !important;
            border-collapse: collapse;
            border: 2px solid #d0d7e5;
            border-bottom: 0px;
            width: 90%;
        }

        #fieldsetTableSecond {
            font-family: Tahoma, Arial, Helvetica, sans-serif;
            color: #3b3b3b !important;
            font-size: 12px !important;
            border-collapse: collapse;
            border: 2px solid #d0d7e5;
            border-bottom: 1px solid #d0d7e5;
            border-top: 0px;
            width: 90%;
        }
        .fieldsetAllowedOTCNavigationTable {
            font-family: Tahoma, Arial, Helvetica, sans-serif;
            color: #3b3b3b !important;
            font-size: 12px !important;
            border-collapse: collapse;
            border: 2px solid #d0d7e5;
            border-top: 0px;
            width: 63%;
        }

            #fieldsetTableSecond tr td:first-child {
                width: 57%;
                padding-left: 5px;
            }

            #fieldsetTableSecond tr td:nth-child(2) {
                
            }

            #fieldsetTableSecond tr td:nth-child(3) {
                width: 30%;
                border-left: 1px solid #d0d7e5;
                padding-left: 63px;
            }

        #fieldsetTableFirst tr {
            border: 1px solid #d0d7e5;
        }
        #tblTimeWarningInterval tr {
            border: 0px solid #d0d7e5 !important;
        }

        .spacingTd td {
            padding: 2px;
        }

        #tblTimeWarningInterval td {
            padding: 0px !important;
        }

        #fieldsetTableFirst tr td:first-child {
            width: 26%;
            padding-left: 5px;
        }
        .minIdCell {
            width: 0px !important;
        }
        #fieldsetTableFirst tr td:nth-child(2) {
           
        }

        #fieldsetTableFirst tr td:nth-child(3) {
            width: 30%;
            border-left: 1px solid #d0d7e5;
            padding-left: 63px;
        }

        #fieldsetTable ul {
            list-style-type: none;
            padding: 0px;
            margin: 0px;
        }

            #fieldsetTable ul li:nth-child(2) {
                width: 100px;
                height: 50px;
                float: left;
            }

            #fieldsetTable ul li:nth-child(3) {
                width: 100px;
                height: 70px;
                float: left;
                background-color: green;
                margin-left: 15px;
            }

        .fieldsetTableHeader {
            padding-left: 366px;
            padding-top: 10px;
        }

        .addIconNew {
            background-image: url("../../Images/add.gif");
            width: 20px;
            height: 16px;
            float: left;
            background-repeat: no-repeat;
            padding-right: 3px;
        }

        .addIconNewDiv {
            width: 16px;
            height: 16px;
            float: right;
            background-repeat: no-repeat;
        }

        .riTextBox, .riEnabled {
            background-color: #fff !important;
            border: 2px solid #558dd2 !important;
            margin-bottom: 2px !important;
        }

        #fieldsetTableFirst input[type='checkbox'], #fieldsetTableSecond input[type='checkbox'] {
            border: 0px !important;
            outline: 2px solid #83ade0 !important;
        }
        /*.ui-dialog ui-widget ui-widget-content ui-corner-all ui-front ui-dialog-buttons ui-draggable ui-resizable*/
        .ui-dialog-titlebar, .ui-widget-header {
            display: none !important;
        }

        div.ui-dialog {
            height: 50px !important;
            width: 114px !important;
            left: 160px !important;
            top: 140px !important; 
            border: 2px solid #558dd2 !important;
            background-color: #c6d9f0 !important;
            padding: 3px !important;
            overflow: hidden !important;
        }

        .ui-dialog-content {
            height: 22px !important;
            padding: 0px;
            margin: 0px;
            overflow: hidden !important;
        }

        .ui-widget-content {
            background-image: none !important;
            padding: 0px !important;
            margin: 2px !important;
        }

        .ui-button, .ui-widget, .ui-state-default, .ui-corner-all, .ui-button-text-only {
            padding: 0px !important;
            margin: 0px 7px 0px 0px !important;
        }

        .ui-dialog-buttonpane {
            border: 0px !important;
            margin: 0px !important;
            padding: 0px !important;
            float: left;
            background-color: #c6d9f0 !important;
            padding-left: 8px !important;
            margin-top: 3px !important;
        }

        .ui-button {
            border: 2px solid #000000 !important;
            color: #000000;
            background: none !important;
            background-color: #548dd4 !important;
        }

        .ui-button-text {
            padding: 0px !important;
            color: black !important;
        }

        .ui-corner-all, .ui-corner-top, .ui-corner-left, .ui-corner-tl {
            border-top-left-radius: 0px !important;
        }

        .ui-corner-all, .ui-corner-top, .ui-corner-right, .ui-corner-tr {
            border-top-right-radius: 0px !important;
        }

        .ui-corner-all, .ui-corner-bottom, .ui-corner-left, .ui-corner-bl {
            border-bottom-left-radius: 0px !important;
        }

        .ui-corner-all, .ui-corner-bottom, .ui-corner-right, .ui-corner-br {
            border-bottom-right-radius: 0px !important;
        }

        .divScroll {
            height: 51px;
            width: 95px;
            overflow-x: hidden;
            overflow-y: auto;
            border: 2px solid #83ade0;
        }
    </style>
    <link href="../../Thinkgate_Blue/TabStrip.Thinkgate_Blue.css?v=2" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:content id="Content2" contentplaceholderid="MainContentPlaceHolder" runat="server">
    <input type="hidden" runat="server" id="hdnAssessmentType" clientidmode="Static" />
    <input type="hidden" runat="server" id="hdnTimeIntervals" clientidmode="Static" />
    <input type="hidden" runat="server" id="hdnIsAssessmentProofed" clientidmode="Static" />
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" OnClientTabSelecting="OnClientTabSelecting"
        SelectedIndex="0" Align="Justify" Width="347px" Skin="Thinkgate_Blue" EnableEmbeddedSkins="False" ClientIDMode="Static">
        <Tabs>
            <telerik:RadTab Text="General" Selected="True">
            </telerik:RadTab>
            <telerik:RadTab Text="Online Tools">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>


    <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" Width="100%"
        CssClass="Web20">
        <telerik:RadPageView runat="server" ID="RadPageView1" CssClass="Web20">

            <div>


                <img runat="server" id="headerImg" src="../../Images/repairtool.png" alt="Create assessment icon"
                    class="headerImg" clientidmode="Static" />
                <br />
                <div class="containerDiv" align="right">
                    <div class="formContainer">
                        <div class="headerDiv">
                            <div runat="server" id="assessmentTitle" clientidmode="Static">
                            </div>
                        </div>
                        <p>
                            Configure the following assessment features:
                        </p>
                        <table width="95%" class="configTable">
                            <thead>
                                <tr>
                                    <td width="25%">Component
                                    </td>
                                    <td>Configuration Settings
                                    </td>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>Content Type
                                    </td>
                                    <td>
                                        <input runat="server" type="radio" id="itemBankRadio" name="contentType" value="Item Bank"
                                            onclick="adjustContent(this);" clientidmode="Static" /><label for="itemBankRadio" class="inputLabels">Item
                                    Bank</label>
                                        <input runat="server" type="radio" id="externalRadio" name="contentType" value="External"
                                            onclick="adjustContent(this);" clientidmode="Static" /><label for="externalRadio" class="inputLabels">External</label>
                                    </td>
                                </tr>
                                <tr runat="server" clientidmode="Static" id="includeFieldTestCell">
                                    <td>Include Field Test
                                    </td>
                                    <td>
                                        <input runat="server" type="radio" id="includeFieldTestYes" name="includeFieldTest"
                                            value="Yes" onclick="adjustContent(this);" clientidmode="Static" /><label for="includeFieldTestYes" class="inputLabels">Yes</label>
                                        <input runat="server" type="radio" id="includeFieldTestNo" name="includeFieldTest"
                                            value="No" onclick="adjustContent(this);" clientidmode="Static" /><label for="includeFieldTestNo" class="inputLabels">No</label>
                                    </td>
                                </tr>
                                <tr runat="server" clientidmode="Static" id="onlineContentFormatCell">
                                    <td>Online Content Format
                                    </td>
                                    <td>
                                        <input runat="server" type="radio" id="onlineContentFormatBoth" name="onlineContentFormat"
                                            value="Test and Response Card" clientidmode="Static" /><label for="onlineContentFormatBoth" class="inputLabels">Assessment
                                    & Response Card</label>
                                        <input runat="server" type="radio" id="onlineContentFormatResponseOnly" name="onlineContentFormat"
                                            value="Response Card Only" clientidmode="Static" /><label for="onlineContentFormatResponseOnly" class="inputLabels">Response
                                    Card Only</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Number of Forms
                                    </td>
                                    <td>
                                        <telerik:RadComboBox runat="server" ClientIDMode="Static" ID="numberOfForms" Width="50"
                                            Skin="Web20">
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Distractor Labels
                                    </td>
                                    <td>
                                        <telerik:RadComboBox runat="server" ClientIDMode="Static" ID="distractorLabels" Width="100"
                                            Skin="Web20">
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Keywords
                                    </td>
                                    <td>
                                        <telerik:RadTextBox runat="server" EmptyMessage="Enter keywords here" Width="200"
                                            ID="keywords" ClientIDMode="Static">
                                        </telerik:RadTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Score Type
                                    </td>
                                    <td>
                                        <telerik:RadComboBox runat="server" ClientIDMode="Static" ID="scoreType" Width="100"
                                            Skin="Web20" Enabled="false">
                                            <Items>
                                                <telerik:RadComboBoxItem runat="server" Text="Percent" Value="Percent" Selected="true" />
                                            </Items>
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Performance Level Set
                                    </td>
                                    <td>
                                        <telerik:RadComboBox runat="server" ClientIDMode="Static" ID="performanceLevelSet"
                                            Width="100" Skin="Web20">
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr runat="server" clientidmode="Static" id="itemBankCell">
                                    <td>Item Banks
                                    </td>
                                    <td>
                                        <asp:Table BorderWidth="0" ID="itemBankTable" runat="server" ClientIDMode="Static">
                                        </asp:Table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Identification
                                    </td>
                                    <td>
                                        <a href="javascript:void(0);" onclick="setTimeout(function() { editIdentification(); }, 0);" id="assessmentIdentificationButton" clientidmode="Static" runat="server">Edit</a>
                                        <span>Click to edit Grade, Subject, Course, Type, Term, Description</span>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <br />
                        <table class="additionalDataTable">
                            <tr>
                                <td style="text-align: right;">
                                    <label for="sourceInput" style="font-weight: bold;">
                                        Source:
                                    </label>
                                </td>
                                <td style="text-align: left;" colspan="3">
                                    <textarea type="text" id="sourceInput" runat="server" clientidmode="Static" rows="2"
                                        cols="40" class="freeFormInputs" title="This field is limited to 50 characters." characterlimit="50" onkeyup="limitPastedText(this, 50);" onkeypress="return checkCharacterLimit(this, 50);"></textarea>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right;">
                                    <label for="creditInput" style="font-weight: bold;">
                                        Credit:
                                    </label>
                                </td>
                                <td style="text-align: left;">
                                    <textarea type="text" id="creditInput" runat="server" clientidmode="Static" rows="2" onclick="openInNewWindow(this)"
                                        cols="40" class="freeFormInputs" title="This field is limited to 500 characters." characterlimit="500" onkeyup="limitPastedText(this, 500);" onkeypress="return checkCharacterLimit(this, 500);"></textarea>
                                </td>
                                <td style="text-align: left;">
                                    <div style="margin-bottom: 5px;">
                                        <span style="margin-right: 10px; font-weight: bold;">Client:</span><span id="schoolDistrictName"
                                            runat="server" clientidmode="Static"></span>
                                    </div>
                                    <div>
                                        <span style="margin-right: 10px; font-weight: bold;">Author:</span><span id="authorName"
                                            runat="server" clientidmode="Static"></span>
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <span style="display: none;">
                            <hr class="dashedLine" />
                            <span class="largeLabel">ONLINE TESTING OPTIONS:</span>
                            <div style="margin: 10px;">
                                <span style="margin-right: 30px;">Online Tools: </span>
                                <input type="checkbox" id="calculatorCheckbox" /><label for="calculatorCheckbox"
                                    class="inputLabels">Calculator</label>
                                <input type="checkbox" id="rulerCheckbox" /><label for="rulerCheckbox" class="inputLabels">Ruler</label>
                                <input type="checkbox" id="protractorCheckbox" /><label for="protractorCheckbox"
                                    class="inputLabels">Protractor</label>
                                <label for="timeAllocatedInput" class="inputLabels" style="margin-right: 5px;">
                                    Time Allocated:</label><input type="text" id="timeAllocatedInput" class="freeFormInputs"
                                        size="15" />
                            </div>
                        </span>
                        <hr class="dashedLine" />
                        <span class="largeLabel">PRINT OPTIONS:</span>
                        <div style="margin: 10px;">
                            <span style="margin-right: 20px;">Number of Columns: </span>
                            <input type="radio" runat="server" clientidmode="Static" id="numberOfColumns1" name="numberOfColumns"
                                value="1" /><label for="numberOfColumns1" class="inputLabels">1</label>
                            <input type="radio" runat="server" clientidmode="Static" id="numberOfColumns2" name="numberOfColumns"
                                value="2" /><label for="numberOfColumns2" class="inputLabels">2</label>
                            <span style="margin-left: 30px; margin-right: 20px;">Short Answer Section on Bubble
                    Sheet: </span>
                            <input type="radio" runat="server" clientidmode="Static" runat="server" id="shortAnswerBubbleSheetYes"
                                name="shortAnswerBubbleSheet" value="Yes" /><label for="shortAnswerBubbleSheetYes"
                                    class="inputLabels">Yes</label>
                            <input type="radio" runat="server" clientidmode="Static" runat="server" id="shortAnswerBubbleSheetNo"
                                name="shortAnswerBubbleSheet" value="No" /><label for="shortAnswerBubbleSheetNo"
                                    class="inputLabels">No</label>
                            <br />
                            <br />
                            <span runat="server" id="coverPageGroup">
                                <span style="margin-right: 20px;">Include Cover Page: </span>
                                <input type="radio" runat="server" clientidmode="Static" id="includeCoverPageYes"
                                    name="includeCoverPage" value="Yes" /><label for="includeCoverPageYes" class="inputLabels">Yes</label>
                                <input type="radio" runat="server" clientidmode="Static" id="includeCoverPageNo"
                                    name="includeCoverPage" value="No" /><label for="includeCoverPageNo" class="inputLabels">No</label>
                                <a href="javascript:void(0);" onclick="updateCoverPage(); return false;">Edit</a>
                            </span>
                        </div>
                        <div style="height: 100%;">
                            <asp:Button runat="server" ID="generateButton" ClientIDMode="Static" CssClass="roundButtons"
                                Text="&nbsp;&nbsp;Generate Assessment&nbsp;&nbsp;" OnClientClick="updateConfiguration(); return false;" />
                            <asp:Button runat="server" ID="backButton" ClientIDMode="Static" CssClass="roundButtons"
                                Text="&nbsp;&nbsp;Back&nbsp;&nbsp;" OnClientClick="backButtonClick(); return false;" />
                            <asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons"
                                Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="cancelButtonClick(); return false;" />
                            <div class="hiddenUpdateButton">
                                <telerik:RadButton runat="server" ID="updateButtonTrigger" ClientIDMode="Static" Text="" OnClick="UpdateConfiguration_Click" />
                            </div>
                            <div class="hiddenUpdateButton">
                                <telerik:RadButton runat="server" ID="updateContentTypeChangedButtonTrigger" Text=""
                                    OnClick="UpdateConfigurationContentTypeChange_Click" ClientIDMode="Static" />
                            </div>
                            <div class="hiddenUpdateButton">
                                <telerik:RadButton runat="server" ID="updateFieldTestChangedButtonTrigger" Text=""
                                    OnClick="UpdateConfigurationFieldTestChange_Click" ClientIDMode="Static" />
                            </div>
                        </div>
                        <input type="hidden" id="assessmentItemCount" clientidmode="Static" runat="server"
                            value="0" />
                        <input type="hidden" id="hiddenUserID" clientidmode="Static" runat="server" value="0" />
                        <input type="hidden" id="assessmentID" clientidmode="Static" runat="server" value="0" />
                        <input type="hidden" id="encryptedAssessmentID" clientidmode="Static" runat="server"
                            value="0" />
                        <input type="hidden" id="encryptedTeacherID" clientidmode="Static" runat="server"
                            value="0" />
                        
                        <input runat="server" type="hidden" id="hiddenAccessSecureTesting" clientidmode="Static" name="hiddenAccessSecureTesting" value="" />
                        <input runat="server" type="hidden" id="hiddenIsSecuredFlag" clientidmode="Static" name="hiddenIsSecuredFlag" value="" />
                        <input runat="server" type="hidden" id="hiddenSecureType" clientidmode="Static" name="hiddenSecureType" value="" />
                    </div>
                </div>
                <span style="display: none;">
                    <telerik:RadXmlHttpPanel runat="server" ID="updateDistractorLabelsXmlHttpPanel" ClientIDMode="Static"
                        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
                        WcfServiceMethod="RequestDistractorLabels" RenderMode="Block" OnClientResponseEnding="updateDistractorLabels">
                    </telerik:RadXmlHttpPanel>
                    <telerik:RadXmlHttpPanel runat="server" ID="storeAssessmentItemBanksXmlHttpPanel"
                        ClientIDMode="Static" Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
                        WcfServiceMethod="StoreAssessmentItemBanksSelected" RenderMode="Block" OnClientResponseEnding="saveConfiguration">
                    </telerik:RadXmlHttpPanel>
                </span>
            </div>
        </telerik:RadPageView>

        <telerik:RadPageView runat="server" ID="RadPageView2" CssClass="Web20">
            <div id="dvTimedControls">
                <div class="fieldsetTableHeader">
                    <div>Enforce on Administration</div>
                   
                </div>
                <fieldset id="fieldsetTable">
                    <div style="float: right; width: 44px;">
                          <asp:ImageButton ID="btnHelp" runat="server" ImageUrl="~/Images/helpIcon.png" Width="39" Height="" AutoPostBack="true" OnClick="btnHelp_Click"  />
                    </div>
                    <table id="fieldsetTableFirst" class="spacingTd">
                        <tr>
                            <td>Timed Assessment:
                            </td>
                            <td>
                                <telerik:RadComboBox runat="server" ClientIDMode="Static" ID="rcbTimedAssessment" Width="50" Skin="Web20" OnClientSelectedIndexChanged="timedAssessmentChangeEvent" >
                                    <Items>
                                        <telerik:RadComboBoxItem Text="No" Value="0" />
                                        <telerik:RadComboBoxItem Text="Yes" Value="1" />
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                            <td>
                                <asp:CheckBox ID="chkTimedAssessment" runat="server" ClientIDMode="Static" />
                            </td>
                        </tr>
                        <tr>
                            <td>Time Allotted:</td>
                            <td>
                                <telerik:RadTextBox runat="server" Width="50" MaxLength="3"
                                    ID="rtbTimeAllotted" ClientIDMode="Static">
                                </telerik:RadTextBox>
                                <span>Minutes (maximum 420).</span>
                            </td>
                            <td>
                                <asp:CheckBox ID="chkTimeAllotted" runat="server" ClientIDMode="Static" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>Allow Time Extension:
                            </td>
                            <td>
                                <telerik:RadComboBox runat="server" ClientIDMode="Static" ID="rcbTimeExtension" Width="50" Skin="Web20" OnClientSelectedIndexChanged="timeExtensionChangeEvent">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="No" Value="0" />
                                        <telerik:RadComboBoxItem Text="Yes" Value="1" />
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                            <td>
                                <asp:CheckBox ID="chkTimeExtension" runat="server" ClientIDMode="Static" />
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">Time Warning Interval:</td>
                            <td>
                                <ul>
                                    <li class="addIconNew" title="Add Time Warning Interval" ></li>
                                    <li style="height: 56px;">
                                        <div class="divScroll">
                                            <table id="tblTimeWarningInterval" align="center">
                                            </table>
                                        </div>
                                    </li>
                                </ul>
                            </td>
                            <td>
                                <asp:CheckBox ID="chkTimeWarningInterval" runat="server" ClientIDMode="Static" />
                            </td>
                        </tr>
                    </table>
                    <div style="display: none;" id="dvTimeIntervalModal">
                        <telerik:RadTextBox runat="server" Width="50" MaxLength="3" ID="rdbMinutes" ClientIDMode="Static"></telerik:RadTextBox>
                        Minutes
                    </div>
                    <table id="fieldsetTableSecond" class="spacingTd">
                        <tr>
                            <td>Auto Submit Results When Time Has Expired:</td>
                            <td>
                                <telerik:RadComboBox runat="server" ClientIDMode="Static" ID="rcbAutoSubmit" Width="50" Skin="Web20" OnClientSelectedIndexChanged="autoSubmitChangeEvent">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="No" Value="0" />
                                        <telerik:RadComboBoxItem Text="Yes" Value="1" Selected="true" />
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                            <td style="border-bottom: 2px solid #d0d7e5;">
                                <asp:CheckBox ID="chkHasAutoSubmit" runat="server" ClientIDMode="Static" />
                            </td>
                        </tr>
                    </table>
                    <table id="fieldsetAllowedOTCNavigation" runat="server" class="fieldsetAllowedOTCNavigationTable">
                        <tr>
                            <td style="padding-left: 5px;">Lock test if student navigates away from online test:</td>
                            <td style="padding-right: 10px;">
                                <telerik:RadComboBox runat="server" ClientIDMode="Static" ID="rcbAllowedOTCNavigation" Width="50" Skin="Web20">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="No" Value="0" />
                                        <telerik:RadComboBoxItem Text="Yes" Value="1" Selected="true" />
                                    </Items>
                                </telerik:RadComboBox>
                             
                            </td>
                            
                          
                        </tr>
                    </table>

                </fieldset>

            </div>

            <div>
                <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
                    <telerik:RadGrid ID="gridTestTool" runat="server" AutoGenerateColumns="false" Width="98%" AllowAutomaticUpdates="True" OnNeedDataSource="gridTestTool_NeedDataSource" OnItemDataBound="gridTestTool_ItemDataBound">
                        <MasterTableView EditMode="InPlace" AllowAutomaticUpdates="True">

                            <Columns>
                                <telerik:GridBoundColumn DataField="ID" Display="false" ReadOnly="True" />

                                <telerik:GridBoundColumn DataField="Name" ReadOnly="True" />
                                <telerik:GridTemplateColumn>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="false" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </asp:Panel>

                <div style="margin-top: 20px; margin-right: 25px;">
                    <asp:Button runat="server" ID="RadButtonOk" ClientIDMode="Static" CssClass="roundButtons"
                        Text="&nbsp;&nbsp;Update&nbsp;&nbsp;" OnClientClick="saveAccommodations(); return false;" AutoPostBack="False" />
                    <asp:Button runat="server" ID="RadButtonClose" ClientIDMode="Static" CssClass="roundButtons"
                        Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="cancelButtonClick(); return false;" AutoPostBack="False" />
                </div>


            </div>
        </telerik:RadPageView>

    </telerik:RadMultiPage>

    <link href="../../Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-migrate-1.1.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" type="text/javascript"></script>
    <script src="../../Scripts/JQueryBlock/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../../Scripts/numeric/numeric.js" type="text/javascript"></script>
    <script src="../../Scripts/Shared/helper.js" type="text/javascript"></script>
    <script src="../../Scripts/TimedAssessment/AssessmentConfiguration.js" type="text/javascript"></script>

    <script type="text/javascript">
        function setConfigTitle() {
            var title = " Configure Assessment";
            var winCustomDialog = getCurrentCustomDialog();
            var hasSecure = $("#hiddenAccessSecureTesting").val().toLowerCase() == "true" ? true : false;
            var isSecureflag = $("#hiddenIsSecuredFlag").val().toLowerCase() == "true" ? true : false;
            var secureAssessment = $("#hiddenSecureType").val().toLowerCase() == "true" ? true : false;
            if (hasSecure == true && isSecureflag == true) {
                if (secureAssessment == true) {
                    title = SetSecureAssessmentTitle(title);
                }
            }
            if (winCustomDialog) {
                //if (winCustomDialog._titleElement.innerText == " ") {
                //winCustomDialog._titleElement.innerText = assessmentTitle;
                //}
                winCustomDialog._titleElement.innerHTML = title;
            }

        }

        function OnClientTabSelecting(sender, eventArgs) {
            var tab = eventArgs.get_tab();
            if (tab.get_text() == "Online Tools")
                AssessmentConfiguration.Init();
        }
        var modalWin = parent.$find('RadWindow1Url');
        prepModalWindow();
        
        function timeExtensionChangeEvent(sender, eventArgs) {
            AssessmentConfiguration.ChangeSelectedIndexOfAutoSubmit(eventArgs.get_item().get_value());
        }

        function autoSubmitChangeEvent(sender, eventArgs) {
            AssessmentConfiguration.ChangeSelectedIndexOfTimeExtension(eventArgs.get_item().get_value());
        }

        function timedAssessmentChangeEvent(sender, eventArgs) {
            if (eventArgs.get_item().get_value() == "0")
                AssessmentConfiguration.EnableDisableControls(true);
            else {
                AssessmentConfiguration.EnableDisableControls(false);
                //if ($("#rtbTimeAllotted").val() == '')
                //    AssessmentConfiguration.MessageAlert("An Update cannot be completed without a Time Allotted entry.", "rcbTimedAssessment");
            }

        }
        function backButtonClick() {
            var win = parent.customDialog({
                url: '../Dialogues/Assessment/AssessmentStandards.aspx?headerImg=' + $('#headerImg').attr('headerImgName'),
                title: 'Assessment Standards Selection',
                maximize: true, maxwidth: '1200', maxheight: '500'
            });

            win.addConfirmDialog({ title: 'Cancel Assessment' }, window);
        }

        function cancelButtonClick() {
            var win = parent.$find('RadWindow1Url');
            setTimeout(function () {
                win.close();
            }, 0);
        }

        function editIdentification() {

            modalWin.SetUrl('../Dialogues/Assessment/CreateAssessmentIdentification.aspx?xID=' + $('#encryptedAssessmentID').attr('value') + '&yID=' + $('#encryptedTeacherID').attr('value') + '&senderPage=config');
            modalWin.SetSize(650, 650);
            modalWin.Center();
            modalWin.SetTitle('Assessment Identification');
        }

        function prepScreen() {
            if (window.location.href.indexOf('xID=') > -1) {
                $('p[objectScreen="assessment"], div[objectScreen="assessment"]', '.containerDiv').css('display', '');
            }
        }

        function prepModalWindow() {
            if (modalWin && modalWin.isVisible()) {
                modalWin.remove_beforeClose(modalWin.confirmBeforeClose);
                modalWin.addConfirmDialog({ title: 'Cancel Changes', width: '510', text: 'All changes made will not be saved. Are you sure you want to continue?' }, window);
            }
        }

        function updateConfiguration() {
            var panel = $find('storeAssessmentItemBanksXmlHttpPanel');
            var itemBankList = "";
            var sourceInput = document.getElementById('sourceInput');
            var creditInput = document.getElementById('creditInput');

            if (sourceInput.innerText && typeof (sourceInput.innerText) != 'undefined' && sourceInput.innerText.length > sourceInput.getAttribute('characterLimit')) {
                radalert('The source text exceeds the maximum allowed ' + sourceInput.getAttribute('characterLimit') + ' characters. Please reduce the amount of text to meet these requirements.');
                return false;
            }
            else if (sourceInput.innerHTML && typeof (sourceInput.innerHTML) != 'undefined' && sourceInput.innerHTML.length > sourceInput.getAttribute('characterLimit')) {
                radalert('The source text exceeds the maximum allowed ' + sourceInput.getAttribute('characterLimit') + ' characters. Please reduce the amount of text to meet these requirements.');
                return false;
            }

            if (creditInput.innerText && typeof (creditInput.innerText) != 'undefined' && creditInput.innerText.length > creditInput.getAttribute('characterLimit')) {
                radalert('The credit text exceeds the maximum allowed ' + creditInput.getAttribute('characterLimit') + ' characters. Please reduce the amount of text to meet these requirements.');
                return false;
            }
            else if (creditInput.innerHTML && typeof (creditInput.innerHTML) != 'undefined' && creditInput.innerHTML.length > creditInput.getAttribute('characterLimit')) {
                radalert('The credit text exceeds the maximum allowed ' + creditInput.getAttribute('characterLimit') + ' characters. Please reduce the amount of text to meet these requirements.');
                return false;
            }

            $('#itemBankTable input[id*="itemBank"]').each(function () {
                //run code
                if ($(this).attr('checked')) itemBankList += '"' + $(this).attr('value') + '",';
            });

            itemBankList = itemBankList.substr(0, itemBankList.lastIndexOf(','));

            panel.set_value('{"AssessmentID":' + $('#assessmentID').attr('value') + ', "ItemBankList":[' + itemBankList + ']}');
        }

        function saveConfiguration(sender, args) {
            $find('updateButtonTrigger').click();
        }

        function goToNewAssessment(sender, args) {
            var result = args.get_content();
            var modalTitle = document.getElementById('assessmentTitle').innerHTML;

            parent.customDialog({
                url: '../Record/AssessmentPage.aspx?xID=' + result,
                title: modalTitle,
                maximize: true
            });
        }

        function adjustContent(obj) {
            function contentTypeCallbackFunction(arg) {
                if (arg) {
                    $find('updateContentTypeChangedButtonTrigger').click();
                }
                else {
                    modalWin.setActive(true);
                    switch (obj.id) {
                        case 'itemBankRadio':
                            document.getElementById('itemBankRadio').checked = false;
                            document.getElementById('externalRadio').checked = true;
                            break;
                        default:
                            document.getElementById('itemBankRadio').checked = true;
                            document.getElementById('externalRadio').checked = false;
                            break;
                    }
                }
            }

            function fieldTestCallbackFunction(arg) {
                if (arg) {
                    $find('updateFieldTestChangedButtonTrigger').click();
                }
                else {
                    modalWin.setActive(true);
                    switch (obj.id) {
                        case 'includeFieldTestNo':
                            document.getElementById('includeFieldTestYes').checked = true;
                            document.getElementById('includeFieldTestNo').checked = false;
                            break;
                        default:
                            document.getElementById('includeFieldTestYes').checked = false;
                            document.getElementById('includeFieldTestNo').checked = true;
                            break;
                    }
                }
            }

            if (parseInt(document.getElementById('assessmentItemCount').value) > 0) {
                if (obj.getAttribute('name').indexOf('contentType') > -1) {
                    var confirmDialogText = '<div style="font-style:italic;font-weight:bold;text-align:center;">Attention!</div><br/><br/>Changing the Content Type selection removes all items that are currently on the assessment.<br/><br/>Do you want to continue?';
                    var wnd = window.radconfirm(confirmDialogText, contentTypeCallbackFunction, 400, 100, null, 'Remove Items');
                    wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                }
                else if (obj.id.indexOf('includeFieldTestNo') > -1) {
                    var confirmDialogText = '<div style="font-style:italic;font-weight:bold;text-align:center;">Attention!</div><br/><br/>Changing the Include Field Test selection to No removes all Field Test items that are currently on the assessment.<br/><br/>Do you want to continue?';
                    var wnd = window.radconfirm(confirmDialogText, fieldTestCallbackFunction, 400, 100, null, 'Remove Field Test Items');
                    wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                }
            }
            else {
                if (obj.getAttribute('name').indexOf('contentType') > -1) {
                    $find('updateContentTypeChangedButtonTrigger').click();
                }
                else if (obj.id.indexOf('includeFieldTestNo') > -1) {
                    $find('updateFieldTestChangedButtonTrigger').click();
                }
            }
        }

        function getDistractorLabels(sender, args) {
            var itemValue = args.get_item().get_value();
            var userID = document.getElementById('hiddenUserID').value;

            var panel = $find('updateDistractorLabelsXmlHttpPanel');

            panel.set_value('{"NumberOfDistractors":"' + itemValue + '", "UserID":"' + userID + '"}');
        }

        function updateDistractorLabels(sender, args) {
            var senderElement = sender.get_element();
            var results = args.get_content();
            var dropdownObject = $find('distractorLabels');

            //Clear all context menu items
            clearAllDropdownItems(dropdownObject);

            /*Add each new context menu item
            results[i].Key = dropdown value
            results[i].Value = text display
            */
            for (var i = 0; i < results.length; i++) {
                addDropdownItem(dropdownObject, results[i].Key, results[i].Value);
            }

            if (results.length >= 1) {
                dropdownObject.get_items().getItem(0).select();
            }
        }

        function addDropdownItem(dropdownObject, itemValue, itemText) {
            if (!dropdownObject || !itemValue || !itemText) {
                return false;
            }

            /*indicates that client-side changes are going to be made and 
            these changes are supposed to be persisted after postback.*/
            dropdownObject.trackChanges();

            //Instantiate a new client item
            var item = new Telerik.Web.UI.RadComboBoxItem();

            //Set its text and add the item
            item.set_text(itemText);
            item.set_value(itemValue);
            dropdownObject.get_items().add(item);

            //submit the changes to the server.
            dropdownObject.commitChanges();
        }

        function clearAllDropdownItems(dropdownObject) {
            var allItems = dropdownObject.get_items().get_count();
            if (allItems < 1) {
                return false;
            }

            /*indicates that client-side changes are going to be made and 
            these changes are supposed to be persisted after postback.*/
            dropdownObject.trackChanges();

            //clear all items
            dropdownObject.get_items().clear();

            //submit the changes to the server.
            dropdownObject.commitChanges();

            return false;
        }

        function updateCoverPage() {
            parent.customDialog({
                url: '../ControlHost/AssessmentCoverPageEditor.aspx?xID=' + $('#encryptedAssessmentID').attr('value'),
                title: 'Assessment Cover Page',
                maximize: true, maxwidth: 1000, maxheight: 600
            });
        }

        function checkCharacterLimit(textField, characterLimit) {
            if (textField.innerText && typeof (textField.innerText) != 'undefined') {
                if (textField.innerText.length >= characterLimit) return false;
            }
            else if (textField.innerHTML && typeof (textField.innerHTML) != 'undefined') {
                if (textField.innerHTML.length >= characterLimit) return false;
            }

            return true;
        }

        function limitPastedText(textField, characterLimit) {
            if (textField.innerText && typeof (textField.innerText) != 'undefined') {
                if (textField.innerText.length > characterLimit) textField.innerText = textField.innerText.substring(0, characterLimit);
            }
            else if (textField.innerHTML && typeof (textField.innerHTML) != 'undefined') {
                if (textField.innerHTML.length > characterLimit) textField.innerHTML = textField.innerHTML.substring(0, characterLimit);
            }
        }

        function openInNewWindow(obj) {
            var objLineHeight = 15; // This should match the line-height in the CSS
            var objHeight = obj.scrollHeight; // Get the scroll height of the textarea
            var numberOfLines = Math.floor(objHeight / objLineHeight);

            if (numberOfLines > 2) {
                var winContent = '<textarea type="text" id="expandedCreditInput" rows="16" cols="50" class="freeFormInputs"'
                    + ' title="This field is limited to 500 characters." characterLimit="500" onkeyup="limitPastedText(this, 500);"'
                    + ' onkeypress="return checkCharacterLimit(this, 500);">' + $('#' + obj.id).val() + '</textarea>';
                customDialog({ title: 'Credit', maximize: true, maxwidth: 400, maxheight: 300, content: winContent, onClosing: function () { $('#' + obj.id).val($('#expandedCreditInput').val()); } });
            }
        }

        function saveAccommodations() {
            var tabControl = $find("RadTabStrip1");
            if (tabControl.get_selectedTab().get_text() == "Online Tools")
                if (AssessmentConfiguration.Validate()) {
                    //Helper.Block("#Content2", "Saving configuration, please wait...");
                    __doPostBack('RadButtonOk', '');
                }
        }
    </script>
</asp:content>
