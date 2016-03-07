<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentAdministration.aspx.cs"
    Inherits="Thinkgate.Dialogues.Assessment.AssessmentAdministration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet"
        type="text/css" runat="server" />
    <link href="~/Styles/Assessment/AssessmentAdministration.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />


    <base target="_self" />
    <style>
        ul {
            list-style-type: none;
            padding: 0px;
            margin: 0px;
        }

        .headerCol2 {
            width: 30% !important;
        }

        #headerTable {
            width: 64% !important;
            float: left !important;
        }

        .divScroll {
            height: 51px;
            width: 95px;
            overflow-x: hidden;
            background-color: #fff;
            overflow-y: auto;
            border: 2px solid #558dd2;
        }

        .addIconNewDiv {
            width: 16px;
            height: 16px;
            float: right;
            margin-left: 3px;
            background-repeat: no-repeat;
        }

        .addIconNew {
            background-image: url("../../Images/addTimeInterval.png");
            width: 16px;
            height: 20px;
            float: left;
            margin-left: 4px;
            margin-right: 4px;
            background-repeat: no-repeat;
            padding-right: 5px;
        }

        .width108px {
            width: 108px;
        }

        .riTextBox, .riEnabled {
            background-color: #fff;
            border: 2px solid #558dd2 !important;
            margin-bottom: 2px !important;
        }

        #TimeAssessment tr {
            padding-bottom: 5px;
        }

        .ui-dialog-titlebar, .ui-widget-header {
            display: none !important;
        }

        body:nth-of-type(1) .ui-dialog {
            height: 50px !important;
            width: 117px !important;
            border: 2px solid #558dd2 !important;
            background-color: #c6d9f0 !important;
            padding: 3px !important;
            overflow: hidden !important;
            left: 783px !important;
            top: 107px !important;
        }

        .ui-widget-content {
            background-image: none !important;
            padding: 0px !important;
            margin: 2px !important;
        }

        .ui-dialog-content {
            height: 22px !important;
            padding: 0px;
            margin: 0px;
            overflow: hidden !important;
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

            .ui-button:hover {
                color: #000000;
                background: none !important;
                background-color: #548dd4 !important;
            }

        .ui-button-text {
            padding: 0px !important;
            color: black !important;
        }

        .RadWindow .rwTopLeft, .RadWindow .rwTopRight, .RadWindow .rwTitlebar, .RadWindow .rwFooterLeft, .RadWindow .rwFooterRight, .RadWindow .rwFooterCenter {
            height: 5px !important;
        }

        .dvTimeIntervalModal {
            min-height: 19px !important;
        }

        .CentreAligned {
            text-align: center;
        }

        .RadForm a.rfdSkinnedButton, .RadForm a.rfdSkinnedButton * {
            display: none !important;
        }

        .imageMargin {
            float: right;
            margin-right: -7px;
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

        .opaque {
            opacity: 0.5 !important;
        }
    </style>

</head>
<body style="width: 100% !important;">
    <form runat="server" id="mainForm" method="post">
        <asp:HiddenField ID="hiddenAccessSecurePermission" runat="server" />
        <asp:HiddenField ID="hiddenisSecuredFlag" runat="server" />
        <asp:HiddenField ID="hiddenisSecureAssessment" runat="server" />
        <asp:HiddenField ID="hiddenShowSecureOTCRefreshButton" runat="server" />
                
        <asp:HiddenField ID="hiddenAssessmentCategory" runat="server" />
        <asp:HiddenField ID="hiddenTimeAssessment" runat="server" />
        <asp:HiddenField ID="hiddenEnForceChkTimeAssessment" runat="server" />
        <asp:HiddenField ID="hiddenEnForceChkTimeAllotted" runat="server" />
        <asp:HiddenField ID="hiddenEnForceChkTimeExtension" runat="server" />
        <asp:HiddenField ID="hiddenEnForceChkTimeWarning" runat="server" />
        <asp:HiddenField ID="hiddenEnForceChkAutoSubmit" runat="server" />
        <asp:HiddenField ID="hiddenTimeAllottedVal" runat="server" />
        <asp:HiddenField ID="hiddenIsAutoSubmit" runat="server" />
        <asp:HiddenField ID="hiddenIsTimeExtension" runat="server" />
        <asp:HiddenField ID="hiddenTimeWarningInterval" runat="server" />
        <asp:HiddenField ID="hiddenIsValid" runat="server" />
        <asp:HiddenField ID="hdnIsTimeAssessmentClick" Value="false" runat="server" />
        <asp:HiddenField ID="hdnAssessmentId" runat="server" />
        <asp:HiddenField ID="hdnClassId" runat="server" />
        <asp:HiddenField ID="hdnTestId" runat="server" />
        <asp:HiddenField ID="hdnIsAdministrationTimeAssessment" runat="server" />
        <asp:HiddenField ID="hdnIsAdministrationAutoSuubmit" runat="server" />
        <asp:HiddenField ID="hdnAllStudentIdsList" runat="server" />

        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-migrate-1.1.0.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
                <asp:ScriptReference Path="~/Scripts/numeric/numeric.js" />
                <asp:ScriptReference Path="~/scripts/master.js" />
                <asp:ScriptReference Path="~/Scripts/TimedAssessment/AssessmentAdminstration.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />
        <input id="inpOnlineEnabled" type="hidden" value="false" runat="server" />
        <input id="inpAssessmentID" type="hidden" value="0" runat="server" />
        <input id="inpClassID" type="hidden" value="0" runat="server" />
        <input id="inpIsGroup" type="hidden" value="0" runat="server" />
        <input id="inpCteID" type="hidden" value="0" runat="server" />
        <asp:Button ID="btnDoReset" runat="server" OnClick="btnDoReset_Click" ClientIDMode="Static"
            Height="0px" Width="0px" Style="visibility: hidden;" />
        <asp:Panel ID="headerContent" runat="server" Width="955px" Font-Names="Arial, Verdana">


            <table id="headerTable">
                <tr>
                    <td class="headerCol1" style="width: 30%">Assessment Name:
                    </td>
                    <td class="headerCol2">
                        <asp:Label ID="lblTestName" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="headerCol1" id="lblScheduling">Scheduling
                    </td>
                    <td class="headerCol2"></td>
                </tr>
                <tr>
                    <td class="headerCol1">Description:
                    </td>
                    <td class="headerCol2">
                        <asp:Label ID="lblDescription" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="headerCol1">Security Status:
                    </td>
                    <td class="headerCol2">
                        <asp:Label ID="lblSecurityStatus" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="headerCol1">Teacher:
                    </td>
                    <td class="headerCol2">
                        <asp:Label ID="lblTeacherName" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="headerCol1">Content Window:
                    </td>
                    <td class="headerCol2">
                        <asp:Label ID="lblContentWindow" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="headerCol1">Test ID:
                    </td>
                    <td class="headerCol2">
                        <asp:Label ID="lblClassTestEventID" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="headerCol1">Print Window:
                    </td>
                    <td class="headerCol2">
                        <asp:Label ID="lblPrintWindow" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="headerCol1">
                        <asp:Label ID="lblNameIdentifier" runat="server"></asp:Label>
                    </td>
                    <td class="headerCol2">
                        <asp:Label ID="lblClassName" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="headerCol1">Administration Instructions: 
                    </td>
                    <td class="headerCol2">
                        <% if (!string.IsNullOrEmpty(administrationDirections))
                           { %>
                        <input type="hidden" value="1" id="hdnAdminIns" />
                        <asp:LinkButton runat="server" ID="lnkViewInst" alternatetext="View" OnClick="lnkViewInst_Click"><b>View</b></asp:LinkButton>

                        <%}
                           else
                           { %>
                        <input type="hidden" value="0" id="hdnAdminIns" />
                        None   
                        <%}%>                     
                          
                    </td>
                </tr>
            </table>

            <table id="tblTimeAssessment" style="float: left; margin-bottom: 30px; font-weight: bold; margin-top: 2px; width: 318px" runat="server">
                <tr>
                    <td>

                        <div style="width: 268px">
                            <asp:ImageButton ID="btnSave" runat="server" Width="70px" Height="21px" ImageUrl="~/Images/save.png" AutoPostBack="true" OnClientClick="if(! Save());return false; " />
                            <%--<asp:ImageButton ID="ImageButton1" runat="server" Width="60px" Height="18px" ImageUrl="~/Images/save.png"   AutoPostBack="true" OnClientClick=" Save();return false; "  />--%>
                        </div>


                    </td>
                    <td>

                        <div>
                            <%-- <img src="../../Images/Help.png" alt="" width="42" height="25" />--%>
                            <asp:ImageButton ID="btnHelp" runat="server" ImageUrl="~/Images/helpIcon.png" Width="39" Height="" AutoPostBack="true" OnClick="btnHelp_Click" />
                        </div>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <table>
                            <tr>
                                <td style="padding-right: 5px;">Timed Assessment: 
                                </td>
                                <td>
                                    <asp:CheckBox ID="ChkboxTimeAssessment" runat="server" ClientIDMode="Static" Checked="false" AutoPostBack="true" OnClick=" return IsTimeAssessmentChecked(this,this.checked);" />
                                </td>

                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <table>
                            <tr>
                                <td style="padding-right: 5px;">Auto Submit Results When Time Expires: </td>
                                <td>
                                    <asp:CheckBox ID="ChkboxAutoSubmit" runat="server" ClientIDMode="Static" AutoPostBack="true" OnClick=" return IsAutoSubmitChecked(this,this.checked);  " />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td colspan="3">
                        <table>
                            <tr>
                                <td class="width108px">Time Allotted:</td>
                                <td>
                                    <telerik:RadTextBox runat="server" Width="50" MaxLength="3"
                                        ID="rtbTimeAllotted" ClientIDMode="Static">
                                        <EnabledStyle HorizontalAlign="Center" CssClass="CentreAligned" />
                                    </telerik:RadTextBox>
                                    <span style="font-weight: normal !important">minutes (max 420)</span>
                                </td>
                            </tr>
                            <tr>
                                <td>Time Extension:</td>
                                <td>
                                    <telerik:RadTextBox runat="server" Width="50" MaxLength="3"
                                        ID="rtbTimeExtn" ClientIDMode="Static">
                                        <EnabledStyle HorizontalAlign="Center" CssClass="CentreAligned" />
                                    </telerik:RadTextBox>
                                    <span style="font-weight: normal !important">minutes</span>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>


                <tr>
                    <td colspan="3">
                        <table>
                            <tr>
                                <td valign="top">Time Warning Interval:<br /><span style="float: right;font-weight: normal !important">(minutes)</span></td>
                                <td>
                                    <ul>
                                        <li class="addIconNew"></li>
                                        <li style="float: left">
                                            <div class="divScroll">
                                                <table id="tblTimeWarningInterval" align="center">
                                                </table>
                                            </div>
                                        </li>                                       
                                    </ul>
                                </td>                                
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div style="display: none;" id="dvTimeIntervalModal">
                <telerik:RadTextBox runat="server" Width="50" MaxLength="3" ID="rdbMinutes" ClientIDMode="Static"></telerik:RadTextBox>
                Minutes
            </div>
            <%-- <div style="display: none;" id="dvTimeAssessmentModal">
                <p>Completing this selection will disable all other assessment timer related selections.  Select Continue to proceed or Cancel to decline the selection.</p>
            </div>--%>
            <!-- The icon buttons -->

            <asp:ImageButton ID="btnRefresh" runat="server" Height="24px" Width="24px" ImageUrl="~/Images/refresh.png"
                OnClick="btnRefresh_Click" Style="" ToolTip="Refresh Table" />
            <div style="display: none">
                <asp:ImageButton ID="Btnrefresh1" runat="server" Height="10px" Width="10px" ImageUrl="~/Images/refresh.png"
                    OnClick="Btnrefresh1_Click" Style="" ToolTip="Refresh Table" />
            </div>


            <table id="buttonTable">
                <tr>
                    <td>
                        <table id="printIconTable" runat="server">
                            <tr>
                                <td>
                                    <div class="btnContainer" id="cntPrintAssessment" runat="server">
                                        <img id="btnPrintAssessment" alt="Print the assessment" title="Print the assessment"
                                            src="../../Images/BtnPrintAssessment.png" class="btn"
                                            onclick="printAssessment()" />
                                        <p class="imageBtnText">
                                            Assessment
                                        </p>
                                    </div>
                                    <div class="btnContainer" id="cntdisPrintAssessment" runat="server">
                                        <img id="btndisPrintAssessment" alt="Print the assessment" title="Print the assessment"
                                            src="../../Images/BtnPrintAssessment.png" class="btn" />
                                        <p class="imageBtnText">
                                            Assessment
                                        </p>
                                    </div>
                                </td>
                                <td>
                                    <div class="btnContainer" id="bubbleSheetContainer" runat="server">
                                        <img id="btnPrintBubbleSheets" alt="Print the bubble sheets" title="Print the bubble sheets"
                                            src="../../Images/BtnPrintBubble.png" class="btn" onclick="printBubbleSheets()" />
                                        <p class="imageBtnText" id="printBubbleSheetText">
                                            Bubble Sheets
                                        </p>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td id="tdMiddleButtons">
                        <asp:Panel ID="Panel1" runat="server" Style="margin-left: 100px;">
                            <table>
                                <tr>
                                    <td>
                                        <div class="btnContainer" id="divButtonReset" runat="server">
                                            <img id="btnReset" runat="server" clientidmode="Static" alt="Reset Scores"
                                                title="Reset Scores" src="" class="btn" onclick="confirmReset()" />
                                            <p class="imageBtnText">
                                                Reset
                                            </p>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="btnContainer" id="divButtonManual" runat="server">
                                            <img id="btnManualInput" runat="server" clientidmode="Static" alt="Manual input" title="Manual input" src="" class="btn"
                                                onclick="manualInputClick()" />
                                            <p class="imageBtnText">
                                                Manual
                                            </p>
                                        </div>
                                    </td>
                                    <td id="scansTD" runat="server">
                                        <div class="btnContainer" id="btnScanContainer" runat="server">
                                            <img id="btnScans" alt="Scans" runat="server" title="Scans" src="" class="btn"
                                                onclick="" />
                                            <p class="imageBtnText">
                                                Scans
                                            </p>
                                        </div>
                                    </td>
                                    <td id="commentsTD" runat="server">
                                        <div class="btnContainer" id="btnContainer">
                                            <img id="btnComments" alt="Comments" runat="server" title="Comments" src="~/Images/BtnCommentDisabled.png"
                                                class="btn" onclick="showComments()" />
                                            <p class="imageBtnText">
                                                Comments
                                            </p>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                    <td id="tdRightButtons">
                        <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Right" Style="float: right; margin-right: 10px;">
                            <table>
                                <tr>
                                    <td>
                                        <div class="btnContainer">
                                            <asp:ImageButton ID="btnEnableDisable" runat="server" CssClass="btn" ToolTip="Click to enable-disable online assessment"
                                                ImageUrl="~/Images/disable.png" OnClick="btnEnableDisable_Click" />
                                            <p id="txtEnableDisableLive" runat="server" class="imageBtnText">
                                                Disabled
                                            </p>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="btnContainer">
                                            <asp:ImageButton ID="btnContinue" runat="server" CssClass="btn" ToolTip="Continue the assessment"
                                                ImageUrl="~/Images/BtnStartDisabled.png" Enabled="False" OnClick="btnContinue_Click" />
                                            <p class="imageBtnText">
                                                Continue
                                            </p>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="btnContainer">
                                            <asp:ImageButton ID="btnSuspend" runat="server" CssClass="btn" ToolTip="Suspend the assessment"
                                                ImageUrl="~/Images/BtnStopDisabled.png" Enabled="False" OnClick="btnSuspend_Click" />
                                            <p class="imageBtnText">
                                                Suspend
                                            </p>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <telerik:RadSkinManager ID="QsfSkinManager" runat="server" Skin="Default" ShowChooser="false" />
        <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="Timer1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdAssessmentAdmin" LoadingPanelID="LoadingPanel1"></telerik:AjaxUpdatedControl>
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
        </telerik:RadAjaxLoadingPanel>
        <asp:Panel ID="Panel3" runat="server">
            <asp:Timer ID="Timer1" runat="server" Interval="7000000" OnTick="Timer1_Tick">
            </asp:Timer>
        </asp:Panel>
        <div id="assessmentAdminView" runat="server">

            <telerik:RadGrid ID="grdAssessmentAdmin" runat="server" AllowSorting="true" Style="margin-left: 4px; margin-right: 4px; width: auto;"
                OnNeedDataSource="grdAssessmentAdmin_NeedDataSource" OnItemDataBound="grdAssessmentAdmin_ItemDataBound" OnItemCreated="grdAssessmentAdmin_ItemCreated"
                Skin="Web20" Font-Size="9pt" AllowMultiRowSelection="true" AutoGenerateColumns="false">
                <ClientSettings Selecting-AllowRowSelect="true" EnableRowHoverStyle="True">
                    <Selecting AllowRowSelect="True" />
                    <Scrolling AllowScroll="True" ScrollHeight="500px" UseStaticHeaders="false" />
                    <ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeselected" />
                </ClientSettings>
                <SortingSettings SortedBackColor="Bisque" EnableSkinSortStyles="false" />
                <MasterTableView Width="100%" DataKeyNames="StudentID" ClientDataKeyNames="StudentID">
                    <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                    </RowIndicatorColumn>
                    <Columns>
                        <telerik:GridClientSelectColumn UniqueName="AdminClientSelectColumn" />
                        <telerik:GridTemplateColumn HeaderText="Student Name" ItemStyle-Wrap="false" ShowSortIcon="true"
                            SortExpression="StudentName" SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <ItemTemplate>
                                <div style="float: left">
                                    <a href="../../Record/Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('../../Record/Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;">
                                        <%# Eval("StudentName") %> </a>
                                </div>
                                <div>
                                    <asp:Image ID="imgeTringle" runat="server" alt="This student’s Online Testing Portal session may be offline.  Please confirm."
                                        title="This student’s Online Testing Portal session may be offline.  Please confirm." ImageUrl="~/Images/Tringle.png" Visible="false" CssClass="imageMargin" />
                                </div>
                            </ItemTemplate>
                            <ItemStyle Wrap="False"></ItemStyle>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Student ID" ShowSortIcon="true" SortExpression="DistrictStudentID"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <ItemTemplate>
                                <span>
                                    <%# Eval("DistrictStudentID")%></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Login ID" ShowSortIcon="true" SortExpression="LoginID"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <ItemTemplate>
                                <span>
                                    <%# Eval("LoginID") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Form #" ShowSortIcon="true" SortExpression="FormID"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <ItemTemplate>
                                <span>
                                    <%# Eval("FormID") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Status" ShowSortIcon="true" SortExpression="Status"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <ItemTemplate>
                                <span>
                                    <%# Eval("Status") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="TTimed" HeaderText="Timed" SortExpression="Timed"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <span>
                                    <asp:CheckBox ID="cbChecked" runat="server" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Timed")) %>'></asp:CheckBox></span>
                                </span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Start" ShowSortIcon="true" SortExpression="TestStartDate"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <ItemTemplate>
                                <span>                                  
                                    <%# Eval("TestStartDate")!=DBNull.Value? Convert.ToDateTime(Eval("TestStartDate")).ToString("g"):"" %>
                                </span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="End" ShowSortIcon="true" SortExpression="TestCompleteDate"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <ItemTemplate>
                                <span>
                                     <%# Eval("TestCompleteDate")!=DBNull.Value? Convert.ToDateTime(Eval("TestCompleteDate")).ToString("g"):"" %>                              
                                </span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn> 
                        <telerik:GridTemplateColumn HeaderText="Total Time (minutes)"  ShowSortIcon="true" SortExpression="TotalTime"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <HeaderStyle Width="90px" /> 
                            <ItemTemplate>
                                <span>
                                    <%# Eval("TotalTime") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="TimeRemaining" HeaderText="Time Remaining (minutes)" ShowSortIcon="true" SortExpression="TimeRemaining"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <HeaderStyle Width="110px" />
                            <ItemTemplate>
                                <span>
                                    <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Timed")) == false ? string.Empty : Eval("TimeRemaining") %></span>
                                <asp:HiddenField ID="hdnMinuteDiff" Value='<%# DataBinder.Eval(Container.DataItem, "MinuteDiff") %>' runat="server" />
                                </span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Score" ShowSortIcon="true" SortExpression="Score" UniqueName="Score"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <ItemTemplate>
                                <span>
                                    <%# Eval("Score") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>

                         <telerik:GridTemplateColumn HeaderText="Resume" ShowSortIcon="true" SortExpression="Score" UniqueName="Resume"
                            SortAscImageUrl="~/Images/uparrow.gif" SortDescImageUrl="~/Images/downarrow.gif">
                            <ItemTemplate>
                                <span>

                                     <%# Eval("Score")!=DBNull.Value ? "<img id='btnRestart' src='../../Images/restart.png' onclick='confirmRestart()' alt='Once Student score will be completed, Restart Assessment will be Enabled else Disabled'   title='Once Student score will be completed, Restart Assessment will be Enabled else Disabled' />" : "<img id='btnRestartinactive' src='../../Images/restart_inactive.png'  alt='Once Student score will be completed, Restart Assessment will be Enabled else Disabled'   title='Once Student score will be completed, Restart Assessment will be Enabled else Disabled' />" %>

                            </ItemTemplate>
                        </telerik:GridTemplateColumn>

                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
        <div id="onlineLiveView" runat="server">
            <telerik:RadGrid ID="grdLive" runat="server" CellSpacing="0" Skin="WebBlue" Font-Size="9pt"
                AllowMultiRowSelection="true">
                <ClientSettings EnableRowHoverStyle="True">
                    <Selecting AllowRowSelect="True" />
                    <Scrolling AllowScroll="True" ScrollHeight="500px" UseStaticHeaders="True" />
                    <Resizing AllowResizeToFit="True" ClipCellContentOnResize="False" EnableRealTimeResize="True"
                        AllowColumnResize="True" />
                </ClientSettings>
                <MasterTableView AutoGenerateColumns="false">
                    <Columns>
                        <telerik:GridClientSelectColumn UniqueName="AdminClientSelectColumn" />
                        <telerik:GridTemplateColumn HeaderText="Student Name">
                            <ItemTemplate>
                                <span>
                                    <%# Eval("StudentName") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Student ID">
                            <ItemTemplate>
                                <span>
                                    <%# Eval("StudentID") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Login ID">
                            <ItemTemplate>
                                <span>
                                    <%# Eval("LoginID") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Form #">
                            <ItemTemplate>
                                <span>
                                    <%# Eval("FormID") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Status">
                            <ItemTemplate>
                                <span>
                                    <%# Eval("Status") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Time Remaining">
                            <ItemTemplate>
                                <span id="timeRemainingColumn">
                                    <%# ((TimeSpan)Eval("TimeRemaining")).ToString("g") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
        <script type="text/javascript">
            $(document).ready(function () {
                var studentId;
                AssessmentAdminstration.Init();
                var cbChkBox = '';
                var isCbChecke = true;
            });
            function AlertMessage(studentId, controlId) {
                var keyValue = document.getElementById(controlId).checked;
                AssessmentAdminstration.TimedColumnCheckedUnchecked(studentId, controlId, keyValue);

            }

            function Save() {

                var val = $('#ChkboxTimeAssessment').is(':checked');
                var studentIDs = getSelectedStudentIds();
                AssessmentAdminstration.StudentIdOnRowSelect(studentIDs);
                var flag = AssessmentAdminstration.Timeallottedvalidation(val);
                return flag;

            }
            function IsAutoSubmitChecked(objCheckBox, isChecked) {
                AssessmentAdminstration.IsAutoSubmitChecked(objCheckBox, isChecked);
            }
            function IsTimeAssessmentChecked(objCheckBox, isChecked) {
                AssessmentAdminstration.IsTimeAssessmentChecked(objCheckBox, isChecked);
            }

            function CheckedChange(value) {
                $("#hdnIsTimeAssessmentClick").val('true');
                var flag = AssessmentAdminstration.IsChkdTimedAssessment;
                return false;

            }

            var adminHasRowsSelected = false;

            function pageLoad(sender, args) {
                UpdateButtonState();
            }

            function RowSelected(sender, eventArgs) {
                UpdateButtonState();
                if ($("#hiddenTimeAssessment").val().toLowerCase() == "true") {
                    //var masterTable = sender.get_masterTableView();
                    //var selectedrow = masterTable.get_selectedItems();
                    //for (var i = 0; i < selectedrow.length; i++) {
                    //    var selectedRowCheckBox = selectedrow[i].findElement('cbChecked');
                    //    $("#" + selectedRowCheckBox._rfddecoratedID).removeClass('rfdInputDisabled');
                    //    var element = document.getElementById(selectedRowCheckBox.id);
                    //    element.removeAttribute('disabled');
                    //    selectedRowCheckBox.disabled = false;
                    //}
                    var studentIDs = getSelectedStudentIds();
                    AssessmentAdminstration.StudentIdOnRowSelect(studentIDs);
                }
            }

            function RowDeselected(sender, eventArgs) {
                UpdateButtonState();
                if ($("#hiddenTimeAssessment").val().toLowerCase() == "true") {
                    //var master = sender.get_masterTableView();
                    //var index = eventArgs.get_itemIndexHierarchical();
                    //var row = sender.get_masterTableView().get_dataItems()[eventArgs.get_itemIndexHierarchical()];
                    //var selectedRowCheckBox = row.findElement('cbChecked');
                    //if (document.getElementById(selectedRowCheckBox.id).disabled == false && document.getElementById(selectedRowCheckBox.id).checked == false) {
                    //    var element = document.getElementById(selectedRowCheckBox.id);
                    //    element.setAttribute('disabled', 'disabled');
                    //    $("#" + selectedRowCheckBox._rfddecoratedID).addClass('rfdInputDisabled');
                    //}
                    var studentIDs = getSelectedStudentIds();
                    AssessmentAdminstration.StudentIdOnRowSelect(studentIDs);
                }
            }

            // Set button images and cursors based on grid row selection.
            function UpdateButtonState() {
                var grid = $find('<%= grdAssessmentAdmin.ClientID %>');
                var gridSelectedItems = grid.get_selectedItems();
                adminHasRowsSelected = gridSelectedItems.length > 0;

                var btn = document.getElementById('btnReset');
                if (btn) {
                    btn.disabled = btn.getAttribute('buttonSecurity') == 'Inactive' || !adminHasRowsSelected;
                    btn.onclick = btn.disabled ? "" : function () { confirmReset(); };
                    btn.src = btn.disabled ? '../../Images/BtnResetDisabled.png' : '../../Images/BtnReset.png';
                    var cursor = btn.disabled ? 'default' : 'pointer';
                    var v = $('#btnReset');
                    $('#btnReset').css('cursor', cursor);
                }

                btn = document.getElementById('btnManualInput');
                if (btn) {
                    btn.disabled = btn.getAttribute('buttonSecurity') == 'Inactive' || (!adminHasRowsSelected && btn.getAttribute('contentType') != 'No Items/Content');
                    btn.onclick = btn.disabled ? "" : function () { manualInputClick(btn.getAttribute('AssessmentID'), btn.getAttribute('ClassID'), btn.getAttribute('ScoreType')); };
                    btn.src = btn.disabled ? '../../Images/BtnManualInputDisabled.png' : '../../Images/BtnManualInput.png';
                    $('#btnManualInput').css('cursor', btn.disabled ? 'default' : 'pointer');
                }

                btn = document.getElementById('btnComments');
                if (btn) {
                    btn.disabled = gridSelectedItems.length != 1;
                    btn.src = btn.disabled ? '../../Images/BtnCommentDisabled.png' : '../../Images/BtnCommentEnabled.png';
                    $('#btnComments').css('cursor', btn.disabled ? 'default' : 'pointer');
                    if (btn.disabled)
                        $('#btnComments').attr('onclick', '');
                    else
                        $('#btnComments').attr('onclick', 'showComments()');
                }

                btn = document.getElementById('btnContinue');
                btn.disabled = !adminHasRowsSelected;
                btn.src = btn.disabled ? '../../Images/BtnStartDisabled.png' : '../../Images/BtnStart.png';

                btn = document.getElementById('btnSuspend');
                btn.disabled = !adminHasRowsSelected;
                btn.src = btn.disabled ? '../../Images/BtnStopDisabled.png' : '../../Images/BtnStop.png';
            }

            // Confirm that the user wants to reset the selected students scores.
            function confirmReset() {
                var confirmDialogText = 'Are you sure you want to reset scores for the selected student(s)?';
                customDialog({ maximize: true, maxwidth: 300, maxheight: 100, title: 'Reset Scores', content: confirmDialogText, dialog_style: 'confirm' },
                            [{ title: 'OK', callback: resetCallbackFunction }, { title: 'Cancel' }]);
            }

            // Confirm that the user wants to reset the selected students scores.
            function confirmRestart() {
              
                RowSelected(this, window.e);
             
                var confirmDialogText = 'Are you sure you want to restart assessment for selected student?';
                customDialog({ maximize: true, maxwidth: 300, maxheight: 100, title: 'Restart Assessment', content: confirmDialogText, dialog_style: 'confirm' },
                            [{ title: 'OK', callback: resetCallbackFunction }, { title: 'Cancel' }]);
            }

            //method for displaying comments
            function showComments() {
                var assessmentID = document.getElementById('btnManualInput').getAttribute('AssessmentID');

                //since the comments button will only be displayed when one student is selected
                //we can grab all selected studentIDs and select the first item in the list
                var studentIDs = getSelectedStudentIds();
                var selectedStudentID = studentIDs[0];

                customDialog({
                    url: ('ViewAssessmentItemComments.aspx?assessmentID=' + assessmentID + '&studentID=' + selectedStudentID),
                    autoSize: false,
                    width: 350,
                    height: 350
                });
            }

            // Yes, reset the scores. Generate a server side button click event from a hidden button 'btnDoReset'.				
            function resetCallbackFunction() {
                var btn = document.getElementById('btnDoReset');
                btn.click();
            }

            function manualInputClick(assessmentID, classID, scoreType) {
                var assessmentID = document.getElementById('btnManualInput').getAttribute('AssessmentID');
                var classID = document.getElementById('btnManualInput').getAttribute('ClassID');
                var scoreType = document.getElementById('btnManualInput').getAttribute('ScoreType');
                var contentType = document.getElementById('btnManualInput').getAttribute('ContentType');
                if (contentType == 'No Items/Content') {
                    var modalWin = getCurrentCustomDialog();
                    var win = customDialog({
                        name: 'RadWindowAssessmentInputScores',
                        url: '../Assessment/AssessmentOfflineScores.aspx?assessmentID=' + assessmentID + '&classID=' + classID + '&scoreType=' + scoreType,
                        title: 'Offline Test - 3rd Party Assessment on Fractions',
                        maximize: true, maxwidth: 600, maxheight: 520
                    });
                }
                else {
                    var cteIdEle = document.getElementById("inpCteID");
                    var cteId = cteIdEle.value;

                    var url = "../../SessionBridge.aspx?ReturnURL=" +
                        escape("fastsql_v2_direct.asp?id=ManualInput_v2_bridge|manual_input_prompt&??CteID=") +
                        cteId.toString() + escape("&ste_list=");

                    var studentIdArray = getSelectedStudentIds();
                    for (var i = 0; i < studentIdArray.length; i++) {
                        var argStr = (i > 0) ? "," : "";
                        argStr += studentIdArray[i] + "|0";
                        url += escape(argStr);
                    }

                    window.open(url);
                }
            }

            function printBubbleSheets() {
                var cteIdEle = document.getElementById("inpCteID");
                var cteId = cteIdEle.value;
                var assessmentCategory = document.getElementById('<%= hiddenAssessmentCategory.ClientID %>').value;
                var studentIdArray = getSelectedStudentIds();
                var studentIds = "";
                for (var i = 0; i < studentIdArray.length; i++) {
                    studentIds += (i > 0) ? "," : "";
                    studentIds += studentIdArray[i];
                }
                customDialog({ url: ('AssessmentPrintBubbleSheets.aspx?xID=' + cteId + '&yID=' + escape(studentIds) + '&assessmentCategory=' + assessmentCategory), autoSize: true });
            }

            // Show the print dialog.
            function printAssessment() {
                //AssessmentAdminstration.ShowHide("True");
                var assessmentIDEle = document.getElementById("inpAssessmentID");
                var assessmentID = assessmentIDEle.value;
                var testNameEle = document.getElementById("lblTestName");

                customDialog({ url: ('AssessmentPrint.aspx?xID=' + assessmentID.toString() + '&yID=' + escape(testNameEle.textContent)), autoSize: true, name: 'AssessmentPrint' });
            }


            // Show the scans popup.
            function showAssessmentScans(assessmentID, classID, isGroup) {
                customDialog({ url: ('AssessmentScans.aspx?xID=' + assessmentID + '&yID=' + classID + '&isGroup=' + isGroup), autoSize: true });
            }

            // Return an array of selected student ids as strings.
            function getSelectedStudentIds() {
                var studentIds = new Array();
                var grid = $find("<%=grdAssessmentAdmin.ClientID %>");
                var masterTableView = grid.get_masterTableView();
                var selectedItems = masterTableView.get_selectedItems();
                for (var i = 0; i < selectedItems.length; i++) {
                    var item = selectedItems[i];
                    var keyItem = item.getDataKeyValue("StudentID");
                    studentIds[i] = keyItem.toString();
                }
                return studentIds;
            }

            // Show a generic under construction dialog.
            function ShowUnderConstructionDialog() {
                customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'Under Construction', content: 'Functionality is under construction', dialog_style: 'alert' },
                                    [{ title: 'Cancel' }]);
            }          


            function closeWindow() {

                var window = document.getElementById("<%=RadWindow1.ClientID %>");
                if (window != null || window != undefined) {
                    window.control.Close();
                }
            }



            function setCustomPosition(sender, args) {

                sender.moveTo(sender.get_left(), sender.get_top());
            }        

        </script>


        <telerik:RadWindow ID="RadWindow1" ReloadOnShow="true" OnClientShow="setCustomPosition" Top="200" CenterIfModal="true" Modal="true" Left="300" runat="server" SaveScrollPosition="false" PersistScrollPosition="false" DestroyOnClose="true" AutoSize="false" AutoSizeBehaviors="Default" Height="200" Width="300" Behaviors="Move, Close" Skin="Web20" Scrolling="None" Title="Administration Instructions" VisibleStatusbar="false" EnableViewState="false">
            <ContentTemplate>
                <div style="padding: 6px; min-height: 100px; position: relative;" id="divContent" runat="server">
                    <%=administrationDirections.Trim() %>
                </div>
                <span style="float: right; padding: 10px">
                    <telerik:RadButton runat="server" ID="btnClose" Text="OK" Skin="Web20"
                        AutoPostBack="false" OnClientClicked="closeWindow" />
                </span>
            </ContentTemplate>
        </telerik:RadWindow>

    </form>
</body>
</html>
