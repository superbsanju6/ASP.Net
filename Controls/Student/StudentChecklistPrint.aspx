<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentChecklistPrint.aspx.cs" Inherits="Thinkgate.Controls.Student.StudentChecklistPrint" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.9.1.js"></script>
    <script src="../../Scripts/jquery-migrate-1.1.0.min.js"></script>
    <script src="../../Scripts/jquery.min.js"></script>
    <script src="../../Scripts/jquery-ui.min.js"></script>
    <script src="../../Scripts/master.js"></script>
    <script src="../../Scripts/EditSubmitResultPagesWithinCustomDialog.js"></script>
       <%--<link href="~/Thinkgate_Blue/TabStrip.Thinkgate_Blue.css?v=2" rel="stylesheet" type="text/css" /> --%>
    <link href="../../Skins/Office2007/PanelBar.Office2007.css" rel="stylesheet" type="text/css" />
    <style>
        
body, div, dl, dt, dd, h1, h2, h3, h4, h5, h6, pre, form, fieldset, input, textarea, p, blockquote, th, td {
    margin: 0;
    padding: 0;
}
table {
    border-collapse: collapse;
    border-spacing: 0;
}
img {
    border: 0 none;
}
address, caption, cite, code, dfn, th, var {
    font-style: normal;
    font-weight: normal;
}
caption, th {
    text-align: left;
}
h1, h2, h3, h4, h5, h6 {
    font-size: 100%;
    font-weight: normal;
}
q:before, q:after {
    content: "";
}
abbr, acronym {
    border: 0 none;
}
div {
    position: relative;
}
input[type="submit"]:disabled {
    color: gray;
    opacity: 0.7;
}
* {
    outline: medium none;
}
.RadWindow {
    backface-visibility: hidden;
    transform: rotate3d(0, 0, 1, 0deg);
}
.RadWindow table.rwTable, .RadWindow table.rwShadow, .RadWindow .rwTitlebarControls {
    border: 0 none;
    padding: 0;
}
.RadWindow .rwCorner, .RadWindow .rwTitlebar, .RadWindow .rwStatusbar, .RadWindow .rwFooterCenter, .RadWindow .rwTitlebarControls td {
    border: 0 none;
    border-collapse: collapse;
    margin: 0;
    padding: 0;
    vertical-align: top;
}
.RadWindow .rwTopResize {
    background-position: 0 -31px;
    background-repeat: repeat-x;
    font-size: 1px;
    height: 4px;
    line-height: 4px;
    width: 100%;
}
.RadWindow .rwStatusbarRow .rwCorner {
    background-repeat: no-repeat;
}
.RadWindow .rwStatusbarRow .rwBodyLeft {
    background-position: -16px 0;
}
.RadWindow .rwStatusbarRow .rwBodyRight {
    background-position: -24px 0;
}
.RadWindow .rwStatusbar {
    background-position: 0 -113px;
    background-repeat: repeat-x;
    height: 22px;
}
.RadWindow .rwStatusbar div {
    background-position: 0 -94px;
    background-repeat: no-repeat;
    height: 18px;
    padding: 0 3px 0 0;
    width: 18px;
}
.RadWindow .rwTable {
    height: 100%;
    table-layout: auto;
    width: 100%;
}
.RadWindow .rwCorner {
    width: 8px;
}
.RadWindow .rwTopLeft, .RadWindow .rwTopRight, .RadWindow .rwTitlebar, .RadWindow .rwFooterLeft, .RadWindow .rwFooterRight, .RadWindow .rwFooterCenter {
    background-repeat: no-repeat;
    font-size: 1px;
    height: 8px;
    line-height: 1px;
}
.RadWindow .rwBodyLeft, .RadWindow .rwBodyRight {
    background-repeat: repeat-y;
}
.RadWindow .rwBodyRight {
    background-position: -8px 0;
}
.RadWindow .rwTopLeft {
    background-position: 0 0;
}
.RadWindow .rwTopRight {
    background-position: -8px 0;
}
.RadWindow table .rwTitlebar {
    -moz-user-select: none;
    background-position: 0 -31px;
    background-repeat: repeat-x;
}
.RadWindow .rwFooterLeft {
    background-position: 0 -62px;
}
.RadWindow .rwFooterRight {
    background-position: -8px -62px;
}
.RadWindow .rwFooterCenter {
    background-position: 0 -70px;
    background-repeat: repeat-x;
}
.RadWindow .rwTitlebarControls {
    height: 27px;
    width: 100%;
}
.RadWindow .rwIframeWrapperIOS {
    height: 100%;
    overflow: scroll;
}
.RadWindow .rwWindowContent {
    background: none repeat scroll 0 0 white;
    height: 100% !important;
}
.RadWindow td.rwLoading {
    background-position: center center;
    background-repeat: no-repeat;
}
.RadWindow .rwStatusbar .rwLoading {
    background-repeat: no-repeat;
}
.RadWindow .rwStatusbar .rwLoading {
    padding-left: 30px;
}
.RadWindow td.rwStatusbar input {
    -moz-user-select: none;
    background-color: transparent !important;
    background-position: left center !important;
    background-repeat: no-repeat !important;
    border: 0 none !important;
    cursor: default;
    display: block;
    float: left;
    font: 12px/18px "Segoe UI",Arial,Verdana,sans-serif;
    height: 18px;
    margin: 0;
    overflow: hidden;
    padding: 4px 0 0 3px;
    text-overflow: ellipsis;
    vertical-align: middle;
    width: 100%;
}
.RadWindow .rwControlButtons {
    float: right;
    list-style: none outside none;
    margin: 2px 0 0;
    padding: 0;
    white-space: nowrap;
}
.RadWindow .rwControlButtons li {
    float: left;
    padding: 0 1px 0 0;
}
.RadWindow .rwControlButtons a {
    background-repeat: no-repeat;
    cursor: default;
    display: block;
    font-size: 1px;
    height: 21px;
    line-height: 1px;
    outline: 0 none;
    text-decoration: none;
    width: 30px;
}
.RadWindow .rwControlButtons span {
    display: block;
}
.RadWindow .rwReloadButton {
    background-position: -120px 0;
}
.RadWindow .rwReloadButton:hover {
    background-position: -120px -21px;
}
.RadWindow .rwPinButton {
    background-position: -180px 0;
}
.RadWindow .rwPinButton:hover {
    background-position: -180px -21px;
}
.RadWindow .rwPinButton.on {
    background-position: -150px 0;
}
.RadWindow .rwPinButton.on:hover {
    background-position: -150px -21px;
}
.RadWindow .rwMinimizeButton {
    background-position: 0 0;
}
.RadWindow .rwMinimizeButton:hover {
    background-position: 0 -21px;
}
.RadWindow .rwMaximizeButton {
    background-position: -60px 0;
}
.RadWindow .rwMaximizeButton:hover {
    background-position: -60px -21px;
}
.RadWindow .rwCloseButton {
    background-position: -90px 0;
}
.RadWindow .rwCloseButton:hover {
    background-position: -90px -21px;
}
.RadWindow.rwMaximizedWindow .rwMaximizeButton, .RadWindow.rwMinimizedWindow .rwMinimizeButton {
    background-position: -30px 0;
}
.RadWindow.rwMaximizedWindow .rwMaximizeButton:hover, .RadWindow.rwMinimizedWindow .rwMinimizeButton:hover {
    background-position: -30px -21px;
}
.RadWindow.rwMaximizedWindow .rwTopResize, .RadWindow.rwMaximizedWindow .rwCorner, .RadWindow.rwMaximizedWindow .rwFooterCenter, .RadWindow.rwMaximizedWindow .rwTitlebar {
    cursor: default !important;
}
.RadWindow .rwIcon {
    background-position: 0 -78px;
    background-repeat: no-repeat;
    cursor: default;
    display: block;
    height: 16px;
    margin: 3px 5px 0 0;
    width: 16px;
}
.RadWindow .rwTitleRow em {
    color: black;
    float: left;
    font: bold 12px "Segoe UI",Arial;
    overflow: hidden;
    padding: 3px 0 0 1px;
    text-overflow: ellipsis;
    white-space: nowrap;
}
.RadWindow.rwInactiveWindow .rwTitlebarControls {
    position: static;
}
.RadWindow .rwDialogPopup {
    color: black;
    cursor: default;
    font: 12px "Segoe UI",Arial,Verdana;
    margin: 16px;
    padding: 1px 0 16px 50px;
}
.RadWindow .rwDialogPopup .rwPopupButton, .RadWindow .rwDialogPopup .rwPopupButton span {
    display: block;
    float: left;
}
.RadWindow .rwControlButtons a {
    overflow: hidden;
    text-align: center;
    text-indent: -3333px;
}
.RadWindow .rwDialogText {
    text-align: left;
}
.RadWindow.rwMinimizedWindow .rwPinButton, .RadWindow.rwMinimizedWindow .rwReloadButton, .RadWindow.rwMinimizedWindow .rwMaximizeButton, .RadWindow.rwMinimizedWindow .rwTopResize {
    display: none !important;
}
.RadWindow .rwDialogInput {
    color: black;
    display: block;
    font: 12px "Segoe UI",Arial,Verdana;
    margin: 8px 0;
    width: 100%;
}
.RadWindow .rwWindowContent .radconfirm, .RadWindow .rwWindowContent .radalert {
    background-color: transparent;
    background-position: left center;
    background-repeat: no-repeat;
}
.RadWindow .rwWindowContent .radconfirm {
    background-image: url("/WebResource.axd?d=JyMEq88cLnSmH67eOt5IiKOP2nYMaKO69BFsk0L_boX355ddPr2ibV0tik2sHRD9Up4smpCRrPgprt-kGJyReoF6WgxBPCNfQ5sFW6G4E8V-IWA-kWCSgQDYsAfL35cxyZVxu9xerPO1sGzd5-pYKXkScVSxVHH6eSubB9RcsvaKxrla0&t=635418873281204010");
}
.RadWindow .rwWindowContent .radalert {
    background-image: url("/WebResource.axd?d=d2iPIL1wm11SFigHuV3k4NUFryrkbE6Pw9Sb7YBPszwy6mpll26rzY8ufyykDrs_BUPIsyPqOcSnyMuAMaMTwJhFEr4gcTKR60w1BxR3zz46RCP95d3HahRJvrIhkIpumRfQha4SRIrtRHiy3ZS_awYNwv6eK42lauJir12JHhA8A3Q50&t=635418873281204010");
}
.RadWindow .rwWindowContent .radprompt {
    padding: 0;
}
.RadWindow .rwPopupButton, .RadWindow .rwPopupButton span {
    color: black;
    cursor: default;
    height: 21px;
    line-height: 21px;
    text-decoration: none;
}
.RadWindow .rwPopupButton {
    background-position: 0 -136px;
    background-repeat: no-repeat;
    margin: 8px 8px 8px 0;
    padding: 0 0 0 3px;
}
.RadWindow .rwWindowContent .rwPopupButton .rwOuterSpan {
    background-position: right -136px;
    background-repeat: no-repeat;
    padding: 0 3px 0 0;
}
.RadWindow .rwWindowContent .rwPopupButton .rwInnerSpan {
    background-position: 0 -157px;
    background-repeat: repeat-x;
    padding: 0 12px;
}
.RadWindow .rwWindowContent .rwPopupButton:hover {
    background-position: 0 -178px;
    margin: 8px 8px 8px 0;
    padding: 0 0 0 3px;
}
.RadWindow .rwWindowContent .rwPopupButton:hover .rwOuterSpan {
    background-position: right -178px;
    padding: 0 3px 0 0;
}
.RadWindow .rwWindowContent .rwPopupButton:hover .rwInnerSpan {
    background-position: 0 -199px;
    padding: 0 12px;
}
.RadWindow .rwStatusbarRow .rwBodyLeft {
    background-position: -16px 0;
}
.RadWindow .rwStatusbarRow .rwBodyRight {
    background-position: -24px 0;
}
.RadWindow.rwMinimizedWindow .rwContentRow, .RadWindow.rwMinimizedWindow .rwStatusbarRow {
    display: none;
}
.RadWindow.rwMinimizedWindow table.rwTitlebarControls {
    margin-top: 4px;
}
.RadWindow.rwMinimizedWindow .rwControlButtons {
    width: 66px !important;
}
.RadWindow.rwMinimizedWindow em {
    width: 90px;
}
.RadWindow.rwMinimizedWindow, .RadWindow .rwMinimizedWindowOverlay {
    float: left !important;
    height: 30px !important;
    overflow: hidden !important;
    width: 200px !important;
}
.RadWindow.rwMinimizedWindow .rwCorner.rwTopLeft {
    background-position: 0 -220px;
    background-repeat: no-repeat;
}
.RadWindow.rwMinimizedWindow .rwCorner.rwTopRight {
    background-position: -8px -220px;
    background-repeat: no-repeat;
}
.RadWindow.rwMinimizedWindow .rwTitlebar {
    background-position: 0 -250px !important;
    background-repeat: repeat-x;
}
.RadWindow.rwInactiveWindow .rwCorner, .RadWindow.rwInactiveWindow .rwTitlebar, .RadWindow.rwInactiveWindow .rwFooterCenter {
    opacity: 0.65 !important;
}
.RadWindow ul.rwControlButtons span {
}
.RadWindow.rwNoTitleBar table tr.rwTitleRow td.rwTopLeft {
    background-position: 0 -280px;
}
.RadWindow.rwNoTitleBar table tr.rwTitleRow td.rwTitlebar {
    background-position: 0 -288px;
    background-repeat: repeat-x;
}
.RadWindow.rwNoTitleBar table tr.rwTitleRow td.rwTopRight {
    background-position: -8px -280px;
}
.RadWindow.rwNoTitleBar table div.rwTopResize {
    background: none repeat scroll 0 center rgba(0, 0, 0, 0);
}
.RadWindow .rwShadow .rwTopLeft, .RadWindow .rwShadow .rwTopRight, .RadWindow.rwMinimizedWindow .rwShadow .rwCorner.rwTopLeft, .RadWindow.rwMinimizedWindow .rwShadow .rwCorner.rwTopRight {
    width: 15px !important;
}
.RadWindow .rwShadow .rwTopLeft, .RadWindow .rwShadow .rwTopRight {
    height: 38px;
}
.RadWindow .rwShadow .rwTopLeft, .RadWindow.rwMinimizedWindow .rwShadow .rwCorner.rwTopLeft {
    background-position: 0 -297px !important;
}
.RadWindow .rwShadow .rwTopRight, .RadWindow.rwMinimizedWindow .rwShadow .rwCorner.rwTopRight {
    background-position: 0 -335px !important;
}
.RadWindow .rwShadow .rwTopResize {
    background-position: 0 -376px !important;
    height: 8px;
}
.RadWindow .rwShadow .rwTitlebar, .RadWindow.rwMinimizedWindow .rwShadow .rwTitlebar {
    background-position: 0 -391px !important;
    background-repeat: repeat-x !important;
    height: 30px !important;
}
.RadWindow .rwInactiveWindow.rwMinimizedWindow {
}
.RadWindow .rwShadow .rwFooterLeft, .RadWindow .rwShadow .rwFooterRight, .RadWindow .rwShadow .rwFooterCenter {
    height: 14px;
}
.RadWindow .rwShadow .rwFooterLeft {
    background-position: 0 -431px;
    width: 15px;
}
.RadWindow .rwShadow .rwFooterCenter {
    background-position: 0 -461px;
    background-repeat: repeat-x;
}
.RadWindow .rwShadow .rwFooterRight {
    background-position: 0 -446px;
    width: 15px;
}
.RadWindow .rwShadow .rwBodyLeft, .RadWindow .rwShadow .rwBodyRight {
    background-repeat: repeat-y;
    width: 15px;
}
.RadWindow .rwShadow .rwBodyLeft {
    background-position: -33px 0;
}
.RadWindow .rwShadow .rwBodyRight {
    background-position: -52px 0;
}
.RadWindow .rwShadow .rwIcon {
    margin: 7px 5px 0 1px;
}
.RadWindow .rwShadow em {
    padding: 7px 0 0 1px;
}
.RadWindow.rwMinimizedWindow .rwShadow .rwCorner.rwTopLeft, .RadWindow.rwMinimizedWindow .rwShadow .rwCorner.rwTopRight {
    height: 1px !important;
}
.RadWindow.rwMinimizedWindowShadow {
    overflow: visible !important;
}
.RadWindow.rwMinimizedWindowShadow .rwTable {
    height: auto !important;
    width: 210px !important;
}
.RadWindow.rwMinimizedWindow .rwShadow .rwFooterLeft {
    background-position: 0 -432px;
}
.RadWindow.rwMinimizedWindow .rwShadow .rwFooterCenter {
    background-position: 0 -462px;
}
.RadWindow.rwMinimizedWindow .rwShadow .rwFooterRight {
    background-position: 0 -447px;
}
.RadWindow.rwMinimizedWindowShadow .rwShadow .rwTitlebarControls {
    display: block;
}
.RadWindow.rwMinimizedWindowShadow .rwShadow .rwTitlebarControls .rwControlButtons .rwPinButton, .RadWindow.rwMinimizedWindowShadow .rwShadow .rwTitlebarControls .rwControlButtons .rwReloadButton, .RadWindow.rwMinimizedWindowShadow .rwShadow .rwTitlebarControls .rwControlButtons .rwMaximizeButton, .RadWindow.rwMinimizedWindowShadow .rwShadow .rwContentRow, .RadWindow.rwMinimizedWindowShadow .rwShadow .rwStatusbarRow {
    display: none !important;
}
.RadWindow .rwMinimizedWindowShadow .rwShadow .rwTopLeft, .RadWindow .rwMinimizedWindowShadow .rwShadow .rwTopRight, .RadWindow .rwMinimizedWindowShadow .rwShadow .rwFooterLeft, .RadWindow .rwMinimizedWindowShadow .rwShadow .rwFooterRight, .RadWindow .rwMinimizedWindowShadow .rwShadow .rwFooterCenter, .RadWindow .rwMinimizedWindowShadow .rwShadow .rwTopResize {
    cursor: default !important;
}
.RadWindow.rwNoTitleBar table.rwShadow tr td.rwTopLeft {
    background-position: 0 -480px !important;
}
.RadWindow.rwNoTitleBar table.rwShadow tr td.rwTitlebar {
    background-position: 0 -525px !important;
}
.RadWindow.rwNoTitleBar table.rwShadow tr td.rwTopRight {
    background-position: 0 -500px !important;
}
.RadWindow.rwNoTitleBar .rwShadow .rwTitlebar, .RadWindow.rwNoTitleBar .rwShadow .rwTopLeft, .RadWindow.rwNoTitleBar .rwShadow .rwTopRight {
    height: 13px !important;
}
.RadWindow.rwNoTitleBar.rwInactiveWindow table.rwShadow tr td.rwTopLeft {
    background-position: 8px -280px !important;
}
.RadWindow.rwNoTitleBar.rwInactiveWindow table.rwShadow tr td.rwTitlebar {
    background-position: 0 -288px !important;
}
.RadWindow.rwNoTitleBar.rwInactiveWindow table.rwShadow tr td.rwTopRight {
    background-position: -9px -280px !important;
}
.RadWindow.rwNoTitleBar.rwInactiveWindow table.rwShadow .rwTitlebar, .RadWindow.rwNoTitleBar.rwInactiveWindow table.rwShadow .rwTopLeft, .RadWindow.rwNoTitleBar.rwInactiveWindow table.rwShadow .rwTopRight {
    height: 8px !important;
}
html:first-child .RadWindow ul {
    border: 1px solid transparent;
    float: right;
}
.RadWindow_rtl .rwControlButtons {
    float: left;
}
div.RadWindow_rtl .rwControlButtons li {
    float: right;
}
div.RadWindow_rtl table.rwShadow .rwControlButtons li {
    float: right;
}
.RadWindow.RadWindow_rtl div.rwDialogText, .RadWindow.RadWindow_rtl div.rwDialogText {
    text-align: right;
}
.RadWindow.RadWindow_rtl div.rwDialogPopup div a, .RadWindow.RadWindow_rtl div.rwDialogPopup div a {
    float: right;
}
.RadWindow.RadWindow_rtl div.rwDialogPopup, .RadWindow.RadWindow_rtl div.rwDialogPopup {
    background-position: right center;
    padding: 1px 50px 16px 0;
}
.RadWindow.RadWindow_rtl div.rwDialogPopup.radprompt, .RadWindow.RadWindow_rtl div.rwDialogPopup.radprompt {
    padding: 1px 0 16px;
}
.RadWindow.RadWindow_rtl .rwPopupButton, .RadWindow.RadWindow_rtl .rwPopupButton:hover {
    margin: 8px 0 8px 8px;
}
.RadComboBox {
    display: inline-block;
    margin: 0;
    padding: 0;
    text-align: left;
    vertical-align: middle;
    white-space: nowrap;
}
.RadComboBox:after {
    clear: both;
    content: "";
    display: block;
    height: 0;
}
.RadComboBox table {
    background: none repeat scroll 0 center rgba(0, 0, 0, 0);
    border: 0 none;
    border-collapse: collapse;
    display: inline-block;
    margin: 0;
    padding: 0;
    width: 100%;
}
.RadComboBox .rcbInputCell, .RadComboBox .rcbArrowCell {
    background-color: transparent;
    background-repeat: no-repeat;
    margin: 0;
    padding: 0;
}
.RadComboBox .rcbInputCell {
    height: 20px;
    line-height: 20px;
    text-align: left;
    vertical-align: middle;
    width: 100%;
}
.RadComboBox table td.rcbInputCell {
    padding: 0 4px 0 5px;
}
.RadComboBox .rcbInput {
    background: none repeat scroll 0 center rgba(0, 0, 0, 0);
    border: 0 none;
    margin: 0;
    outline: 0 none;
    padding: 2px 0 1px;
    vertical-align: middle;
    width: 100%;
}
.RadComboBox .rcbDisabled .rcbInput {
    cursor: default;
}
.RadComboBox .rcbEmptyMessage {
    font-style: italic;
}
.RadComboBox .rcbArrowCell {
    width: 18px;
}
.RadComboBox .rcbArrowCell a {
    cursor: default;
    display: block;
    font-size: 0;
    height: 22px;
    line-height: 1px;
    outline: 0 none;
    overflow: hidden;
    position: relative;
    text-decoration: none;
    text-indent: 9999px;
    width: 18px;
}
.RadComboBox table td.rcbArrowCell {
    padding: 0;
}
.RadComboBox .rcbArrowCellHidden, .RadComboBox .rcbArrowCellHidden a {
    width: 3px;
}
.RadComboBox .rcbReadOnly .rcbInput {
    cursor: default;
}
.RadComboBox .rcbLabel {
    line-height: 22px;
    padding-right: 10px;
    vertical-align: top;
}
.RadComboBox_rtl {
    text-align: right;
}
.RadComboBox_rtl .rcbInputCell {
    padding-left: 4px;
    padding-right: 5px;
}
.RadComboBox_rtl .rcbInput {
    text-align: right;
}
.RadComboBox_rtl .rcbLabel {
    padding: 0 0 0 10px;
    text-align: right;
}
.rcbSlide {
    display: none;
    float: left;
    overflow: hidden;
    position: absolute;
}
.RadComboBoxDropDown {
    border: 1px solid;
    cursor: default;
    position: absolute;
    text-align: left;
}
.RadComboBoxDropDown:after {
    clear: both;
    content: "";
    display: block;
    height: 0;
}
.RadComboBoxDropDown.rcbAutoWidth {
    min-width: 158px;
}
.RadComboBoxDropDown.rcbAutoWidth .rcbList {
    white-space: nowrap;
}
@media �screen {
.RadComboBoxDropDown.rcbAutoWidth .rcbList {
    min-width: 148px;
}
}
.RadComboBoxDropDown.rcbAutoWidthResizer .rcbScroll {
    overflow-x: visible !important;
    overflow-y: scroll !important;
}
.RadComboBoxDropDown .rcbScroll {
    overflow: auto;
    position: relative;
}
.RadComboBoxDropDown .rcbScroll:after {
    clear: both;
    content: "";
    display: block;
    height: 0;
}
.RadComboBoxDropDown .rcbHeader, .RadComboBoxDropDown .rcbFooter {
    background-repeat: repeat-x;
    padding: 5px 7px 4px;
}
.RadComboBoxDropDown .rcbHeader:after, .RadComboBoxDropDown .rcbFooter:after {
    clear: both;
    content: "";
    display: block;
    height: 0;
}
.RadComboBoxDropDown .rcbHeader {
    border-bottom: 1px solid;
    margin-bottom: 1px;
}
.RadComboBoxDropDown .rcbFooter {
    border-top: 1px solid;
    margin-top: 1px;
}
.RadComboBoxDropDown .rcbList {
    list-style: none outside none;
    margin: 0;
    padding: 0;
    position: relative;
}
.RadComboBoxDropDown .rcbList:after {
    clear: both;
    content: "";
    display: block;
    height: 0;
}
.RadComboBoxDropDown .rcbItem, .RadComboBoxDropDown .rcbHovered, .RadComboBoxDropDown .rcbDisabled, .RadComboBoxDropDown .rcbLoading, .RadComboBoxDropDown .rcbCheckAllItems, .RadComboBoxDropDown .rcbCheckAllItemsHovered {
    background-repeat: repeat-x;
    height: auto;
    margin: 0 1px;
    min-height: 13px;
    padding: 2px 6px;
}
.RadComboBoxDropDown .rcbItem:after, .RadComboBoxDropDown .rcbHovered:after, .RadComboBoxDropDown .rcbDisabled:after, .RadComboBoxDropDown .rcbLoading:after, .RadComboBoxDropDown .rcbCheckAllItems:after, .RadComboBoxDropDown .rcbCheckAllItemsHovered:after {
    clear: both;
    content: "";
    display: block;
    height: 0;
}
.RadComboBoxDropDown .rcbItem > label, .RadComboBoxDropDown .rcbHovered > label, .RadComboBoxDropDown .rcbDisabled > label, .RadComboBoxDropDown .rcbLoading > label, .RadComboBoxDropDown .rcbCheckAllItems > label, .RadComboBoxDropDown .rcbCheckAllItemsHovered > label {
    display: block;
    margin: -2px -6px;
    padding: 2px 6px;
}
.RadComboBoxDropDown .rcbNoWrap .rcbItem, .RadComboBoxDropDown .rcbNoWrap .rcbHovered, .RadComboBoxDropDown .rcbNoWrap .rcbDisabled, .RadComboBoxDropDown .rcbNoWrap .rcbLoading {
    white-space: nowrap;
}
.RadComboBoxDropDown .rcbDisabled {
    cursor: default;
}
.RadComboBoxDropDown .rcbLoading {
    text-align: center;
}
.RadComboBoxDropDown em {
    font-style: normal;
    font-weight: bold;
}
.RadComboBoxDropDown .rcbCheckBox, .RadComboBoxDropDown .rcbCheckAllItemsCheckBox {
    vertical-align: middle;
}
.RadComboBoxDropDown .RadComboBoxDropDown .rcbCheckAllItems {
    background-image: url("/WebResource.axd?d=zKfkV4OfIxuUNsY8to7ehVr97J66s111d1TDJ-cokG5YCDhA9QQHHrGVmG2knQbhamU-4cb-5ebUauXt7ZeAlGkB36MBMtGOJn1BElFspupmEgQkUcFod6DEDyQXbz_eJh66ExRbiEj_cUb3CXXHFpDFLnTo48hVBlMsK66uVxCUh4TS0&t=635418873281204010");
    background-position: 0 0;
    background-repeat: repeat-x;
}
.RadComboBoxDropDown .RadComboBoxDropDown .rcbCheckAllItemsHovered {
    background-position: 0 -20px;
}
.RadComboBoxDropDown .rcbImage {
    margin: 0 6px 2px 0;
    vertical-align: middle;
}
.RadComboBoxDropDown .rcbMoreResults {
    background-repeat: repeat-x;
    border-top-style: solid;
    border-top-width: 1px;
    clear: both;
    margin-top: 1px;
    padding: 0 6px;
    position: relative;
    text-align: center;
}
.RadComboBoxDropDown .rcbMoreResults a {
    background-repeat: no-repeat;
    cursor: pointer;
    display: inline-block;
    height: 9px;
    overflow: hidden;
    text-decoration: none;
    text-indent: -9999px;
    vertical-align: middle;
    width: 15px;
}
.RadComboBoxDropDown .rcbMoreResults span {
    display: inline-block;
    height: 19px;
    line-height: 19px;
    vertical-align: middle;
}
.RadComboBoxDropDown .rcbSeparatedList .rcbSeparator {
    padding-left: 6px;
}
.RadComboBoxDropDown .rcbSeparatedList .rcbItem, .RadComboBoxDropDown .rcbSeparatedList .rcbHovered, .RadComboBoxDropDown .rcbSeparatedList .rcbDisabled, .RadComboBoxDropDown .rcbSeparatedList .rcbLoading {
    padding-left: 12px;
}
.RadComboBoxDropDown_rtl {
    direction: rtl;
    text-align: right;
}
.RadComboBoxDropDown_rtl .rcbImage {
    margin: 0 0 2px 6px;
}
.RadComboBoxDropDown_rtl .rcbSeparatedList .rcbSeparator {
    padding-right: 6px;
}
.RadComboBoxDropDown_rtl .rcbSeparatedList .rcbItem, .RadComboBoxDropDown_rtl .rcbSeparatedList .rcbHovered, .RadComboBoxDropDown_rtl .rcbSeparatedList .rcbDisabled, .RadComboBoxDropDown_rtl .rcbSeparatedList .rcbLoading {
    padding-right: 12px;
}
.RadComboBoxWithLabel {
}
.RadComboBoxWithLabel table {
    display: inline-block;
    vertical-align: top;
}
@media screen and (min-width: 550px) {
.RadComboBoxDropDown_rtl .rcbItem, .RadComboBoxDropDown_rtl .rcbHovered, .RadComboBoxDropDown_rtl .rcbDisabled, .RadComboBoxDropDown_rtl .rcbLoading {
    padding: 2px 6px 2px 19px;
}
}
.RadPanelBar {
    text-align: left;
    width: 250px;
}
.RadPanelBar_rtl {
    text-align: right;
}
.RadPanelBar .rpRootGroup {
    border-style: solid;
    border-width: 1px;
}
.RadPanelBar .rpRootGroup, .RadPanelBar .rpGroup, .RadPanelBar .rpItem {
    list-style: none outside none;
    margin: 0;
    padding: 0;
}
.RadPanelBar .rpSlide {
    display: none;
    float: none;
    height: auto;
    overflow: hidden;
    position: relative;
}
.RadPanelBar .rpItem {
    display: block;
    float: none;
    overflow: hidden;
    position: static;
}
* html .RadPanelBar .rpItem {
    display: inline;
}
* html .RadPanelBar .rpSeparator {
    display: block;
}
* html .RadPanelBar .rpGroup .rpItem {
    display: block;
}
.RadPanelBar .rpItem:after, .RadPanelBar .rpText:after, .RadPanelBar .rpTemplate:after {
    clear: both;
    content: "";
    display: block;
    font-size: 0;
    height: 0;
    line-height: 0;
    visibility: hidden;
}
.RadPanelBar .rpLink {
    background-repeat: repeat-x;
    border-bottom-style: solid;
    border-bottom-width: 1px;
    cursor: pointer;
    overflow: hidden;
    text-decoration: none;
}
.RadPanelBar .rpLink:focus {
    outline: 0 none;
}
.RadPanelBar .rpLink, .RadPanelBar .rpOut, .RadPanelBar .rpText {
    display: block;
}
* + html .RadPanelBar .rpItem {
    display: inline;
}
* + html .RadPanelBar .rpGroup .rpItem {
    display: block;
}
* + html .RadPanelBar .rpSeparator {
    display: block;
}
.RadPanelBar .rpHeaderTemplate, .RadPanelBar .rpOut {
    border-bottom-style: solid;
    border-bottom-width: 1px;
}
.RadPanelBar .rpHeaderTemplate {
    line-height: 25px;
}
* html .RadPanelBar .rpHeaderTemplate {
    height: 25px;
}
.RadPanelBar .rpFocused .rpOut, .RadPanelBar a.rpLink:hover .rpOut, .RadPanelBar .rpSelected .rpOut, .RadPanelBar a.rpSelected:hover .rpOut {
    border-bottom-width: 0;
    padding-bottom: 1px;
}
.RadPanelBar a.rpDisabled:hover .rpOut {
    border-bottom-width: 1px;
    padding-bottom: 0;
}
.RadPanelBar .rpImage {
    border: 0 none;
    float: left;
    padding: 4px 3px 3px;
    vertical-align: middle;
}
.RadPanelBar_rtl .rpImage {
    float: right;
}
* html .RadPanelBar .rpImage {
    padding-bottom: 4px;
}
.RadPanelBar .rpText {
    padding: 0 10px;
}
.RadPanelBar .rpGroup {
    overflow-x: hidden;
    overflow-y: auto;
    position: relative;
}
.RadPanelBar .rpGroup .rpLink, .RadPanelBar .rpGroup .rpTemplate {
    background-color: transparent;
    border-bottom: 0 none;
    font-size: 12px;
    line-height: 22px;
}
.RadPanelBar .rpGroup a.rpLink:hover {
    border-bottom: 0 none;
}
.RadPanelBar .rpGroup .rpLink .rpOut, .RadPanelBar .rpGroup .rpExpanded .rpOut, .RadPanelBar .rpGroup .rpSelected .rpOut, .RadPanelBar .rpGroup a.rpLink:hover .rpOut, .RadPanelBar .rpGroup a.rpExpanded:hover .rpOut, .RadPanelBar .rpGroup a.rpSelected:hover .rpOut {
    padding-bottom: 0;
}
.RadPanelBar .rpGroup a.rpDisabled:hover .rpOut {
    border-bottom: 0 none;
}
.RadPanelBar .rpGroup .rpOut {
    border-bottom: 0 none;
    margin-right: 3px;
}
.RadPanelBar .rpGroup .rpImage {
    padding: 3px 3px 3px 9px;
}
.RadPanelBar .rpLevel2 .rpTemplate, .RadPanelBar .rpLevel2 .rpOut {
    padding-left: 15px;
}
.RadPanelBar .rpLevel3 .rpTemplate, .RadPanelBar .rpLevel3 .rpOut {
    padding-left: 30px;
}
.RadPanelBar_rtl .rpLevel2 .rpTemplate, .RadPanelBar_rtl .rpLevel2 .rpOut {
    padding-left: 0;
    padding-right: 15px;
}
.RadPanelBar_rtl .rpLevel3 .rpTemplate, .RadPanelBar_rtl .rpLevel3 .rpOut {
    padding-left: 0;
    padding-right: 30px;
}
.RadPanelBar .rpLevel1 .rpFirst {
    padding-top: 1px;
}
.RadPanelBar .rpLevel2 .rpItem {
    padding-top: 0;
}
.RadPanelBar .rpLevel1 .rpLast {
    padding-bottom: 1px;
}
.RadPanelBar .rpLevel2 .rpItem {
    padding-bottom: 0;
}
.RadPanelBar .rpExpandable .rpExpandHandle, .RadPanelBar .rpExpanded .rpExpandHandle {
    display: block;
    float: right;
    height: 15px;
    margin: 5px 5px 0 0;
    width: 15px;
}
.RadPanelBar .rpGroup .rpExpanded .rpExpandHandle, .RadPanelBar .rpGroup .rpExpandable .rpExpandHandle {
    margin: 4px 2px 0 0;
}
.RadPanelBar_rtl .rpExpandable .rpExpandHandle, .RadPanelBar_rtl .rpExpanded .rpExpandHandle {
    float: left;
    margin: 5px 0 0 5px;
}
.RadPanelBar_rtl .rpGroup .rpExpanded .rpExpandHandle, .RadPanelBar_rtl .rpGroup .rpExpandable .rpExpandHandle {
    margin: 4px 0 0 5px;
}
@media print {
.RadPanelBar div.rpSlide, .RadPanelBar li.rpItem, .RadPanelBar a.rpLink {
    overflow: visible;
}
.RadPanelBar ul.rpGroup {
    overflow: visible !important;
}
}
.RadGrid .rgMasterTable, .RadGrid .rgDetailTable, .RadGrid .rgEditForm table {
    border-collapse: separate;
    border-spacing: 0;
}
.RadGrid .rgRow, .RadGrid .rgAltRow, .RadGrid .rgHeader, .RadGrid .rgResizeCol, .RadGrid .rgPager, .RadGrid .rgGroupPanel, .RadGrid .rgGroupHeader {
    cursor: default;
}
.RadGrid input[type="image"] {
    cursor: pointer;
}
.RadGrid .rgRow td, .RadGrid .rgAltRow td, .RadGrid .rgEditRow td, .RadGrid .rgFooter td, .RadGrid .rgFilterRow td, .RadGrid .rgHeader, .RadGrid .rgResizeCol, .RadGrid .rgGroupHeader td {
    padding-left: 7px;
    padding-right: 7px;
}
.RadGrid .rgClipCells .rgHeader, .RadGrid .rgClipCells .rgFilterRow > td, .RadGrid .rgClipCells .rgRow > td, .RadGrid .rgClipCells .rgAltRow > td, .RadGrid .rgClipCells .rgEditRow > td, .RadGrid .rgClipCells .rgFooter > td {
    overflow: hidden;
}
.RadGrid .rgSave, .RadGrid .rgAdd, .RadGrid .rgRefresh, .RadGrid .rgEdit, .RadGrid .rgDel, .RadGrid .rgDrag, .RadGrid .rgFilter, .RadGrid .rgPagePrev, .RadGrid .rgPageNext, .RadGrid .rgPageFirst, .RadGrid .rgPageLast, .RadGrid .rgExpand, .RadGrid .rgCollapse, .RadGrid .rgSortAsc, .RadGrid .rgSortDesc, .RadGrid .rgUpdate, .RadGrid .rgCancel, .RadGrid .rgUngroup, .RadGrid .rgExpXLS, .RadGrid .rgExpDOC, .RadGrid .rgExpPDF, .RadGrid .rgExpCSV {
    background-color: transparent;
    background-repeat: no-repeat;
    border: 0 none;
    cursor: pointer;
    font-size: 1px;
    height: 16px;
    margin: 0;
    padding: 0;
    vertical-align: middle;
    width: 16px;
}
.RadGrid .rgBatchChanged {
    background-image: url("/WebResource.axd?d=2Px6G84_SJyaafDqld_NdGBGtvhLPD9E8ee7F6pIDYDpD69lp1dIUvVSjr7VZzXwetBMN7jssEYa_xsZaQD9igk73bPKz1NtVvZ5gKS4FDabSx6QBxgsQzdhu4YKMUFXbMCh_XjItZaJGEcXjyPsFpCWyX41&t=635418873281204010");
    background-position: 0 0;
    background-repeat: no-repeat;
}
.RadGrid .rgSave {
    background-position: 0 -1825px;
    height: 18px;
    width: 18px;
}
.RadGrid .rgSave, .RadGrid .rgCancel {
    margin: 0 3px 0 10px;
}
.RadGrid .rgBatchContainer {
    max-width: 90%;
}
.RadGrid .rgGroupItem input, .RadGrid .rgCommandRow img, .RadGrid .rgCommandRow a, .RadGrid .rgHeader input {
    vertical-align: middle;
}
.RadGrid .rgFilterRow img, .RadGrid .rgFilterRow input {
    vertical-align: middle;
}
.RadGrid .rgFilterRow .RadAutoCompleteBox {
    display: inline-block;
    vertical-align: middle;
}
* + html .RadGrid .rgFilterRow .RadAutoCompleteBox {
    display: inline;
}
.RadGrid .rgPager img {
    vertical-align: middle;
}
.RadGrid .rgRow td, .RadGrid .rgAltRow td, .RadGrid .rgEditRow td, .RadGrid .rgFooter td {
    padding-bottom: 3px;
    padding-top: 4px;
}
.RadGrid table.rgMasterTable tr .rgDragCol {
    padding-left: 0;
    padding-right: 0;
    text-align: center;
}
.RadGrid .rgDrag {
    cursor: url("/WebResource.axd?d=E7c9flGUldxSWZYrNhqdwAW0PgJLH2RyEe0VrlkFhwpj8iFCkClCmOyBG5VkXoTA620T1ZGLKqNNAXJ6crVJol5YPTllJ4EXwsga0tGQf3WwuRPFpmbHOIqM1PVut85tviq-uQyRc---7PeZmTlWoOz1H8Y1&t=635418873281204010"), move;
    height: 15px;
    width: 15px;
}
.RadGrid .rgPager .rgStatus {
    padding: 3px 0 2px;
    width: 35px;
}
.RadGrid .rgStatus div {
    background-color: transparent;
    background-position: center center;
    background-repeat: no-repeat;
    border: 0 none;
    height: 24px;
    margin: 0 auto;
    overflow: hidden;
    padding: 0;
    text-indent: -2222px;
    width: 24px;
}
.RadGrid .rgPager td {
    padding: 0;
}
.RadGrid td.rgPagerCell {
    border: 0 none;
    padding: 5px 0 4px;
}
.RadGrid .rgWrap {
    float: left;
    line-height: 22px;
    padding: 0 10px;
    white-space: nowrap;
}
.RadGrid .rgArrPart1 {
    padding-right: 0;
}
.RadGrid .rgArrPart2 {
    padding-left: 0;
}
.RadGrid .rgInfoPart {
    float: right;
}
.RadGrid .rgInfoPart strong {
    font-weight: normal;
}
.RadGrid .rgArrPart1 img, .RadGrid .rgArrPart2 img {
    border: 0 none;
    margin: -3px 1px 0;
}
.RadGrid .rgPageFirst, .RadGrid .rgPagePrev, .RadGrid .rgPageNext, .RadGrid .rgPageLast {
    height: 22px;
    vertical-align: top;
    width: 22px;
}
.RadGrid .NextPrev .rgPageFirst, .RadGrid .NextPrev .rgPagePrev, .RadGrid .NextPrev .rgPageNext, .RadGrid .NextPrev .rgPageLast {
    vertical-align: middle;
}
.RadGrid .rgPageFirst, .RadGrid .rgPagePrev {
    margin-right: 1px;
}
.RadGrid .rgPageNext, .RadGrid .rgPageLast {
    margin-left: 1px;
}
.RadGrid .rgPager .rgPagerButton {
    border-style: solid;
    border-width: 1px;
    cursor: pointer;
    font-size: 12px;
    height: 22px;
    line-height: 12px;
    margin: 0 14px 0 0;
    padding: 0 4px 2px;
    vertical-align: top;
}
.RadGrid .rgNumPart {
    padding: 0;
}
.RadGrid .NumericPages .rgNumPart {
    padding: 0 10px;
}
.RadGrid .rgNumPart a {
    float: left;
    line-height: 22px;
    margin: 0;
    padding: 0 5px 0 0;
    text-decoration: none;
}
.RadGrid .rgNumPart span {
    float: left;
    padding: 0 0 0 5px;
}
.RadGrid .rgNumPart a:hover span {
    cursor: pointer;
}
.RadGrid .rgNumPart a.rgCurrentPage {
    cursor: default;
}
.RadGrid .rgNumPart a.rgCurrentPage:hover, .RadGrid .rgNumPart a.rgCurrentPage span, .RadGrid .rgNumPart a.rgCurrentPage:hover span {
    cursor: default;
}
.RadGrid .NextPrevNumericAndAdvanced .rgAdvPart {
    float: none;
    text-align: center;
}
.RadGrid .rgPager .RadSlider {
    float: left;
    margin: 0 10px 0 0;
}
.RadGrid .rgPagerLabel {
    margin: 0 4px 0 0;
    vertical-align: top;
}
.RadGrid .rgPager .RadComboBox {
    margin: 0 4px 0 0;
    vertical-align: top;
}
.RadGrid .rgPager .RadInput {
    display: inline-block;
    margin: 0 4px 0 0;
    vertical-align: top;
}
.RadGrid .rgPager .riTextBox {
    height: 15px;
}
.RadGrid div.rgHeaderWrapper {
    border-left: 0 none;
    border-right: 0 none;
    overflow: hidden;
    padding: 0;
}
.RadGrid .rgFooterWrapper {
    border-top-style: solid;
    overflow: hidden;
}
.rgCellSelectorArea {
    opacity: 0.1;
    position: absolute;
    z-index: 1000100;
}
.rgNoScrollImage div.rgHeaderDiv {
    background-image: none;
}
.rgMultiHeader {
    overflow: hidden;
}
.rgMultiHeader .rgHeaderDiv {
    margin-left: -1px;
}
.rgHeaderWrapper .rgHeaderDiv {
    border-right: 1px solid transparent;
    margin-bottom: -1px;
    margin-right: -1px;
}
.rgFooterWrapper .rgFooterDiv {
    margin-top: -1px;
}
.RadGrid .rgHeader, .RadGrid th.rgResizeCol {
    font-weight: normal;
    padding-bottom: 4px;
    padding-top: 5px;
    text-align: left;
}
.RadGrid .rgHeader a {
    text-decoration: none;
}
.RadGrid .rgCheck input {
    cursor: default;
    height: 15px;
    margin-bottom: 0;
    margin-top: 0;
    padding-bottom: 0;
    padding-top: 0;
}
.rfdCheckbox .RadGrid .rgCheck input {
    height: 20px;
}
.rgPager .riSingle .riTextBox {
    height: 22px;
}
* + html .RadGrid .rgPager .RadComboBox {
    margin-top: -1px;
}
* html .RadGrid .rgPager .RadComboBox {
    margin-top: -1px;
    padding: 1px 0;
}
.RadGrid .rgPagerTextBox {
    text-align: center;
}
.GridReorderTop, .GridReorderBottom {
    height: 9px;
    margin: 0 0 0 -5px;
    padding: 0;
    width: 9px;
}
.RadGrid .rgFilterRow td {
    padding-bottom: 7px;
    padding-top: 4px;
}
.RadGrid .rgFilter {
    height: 22px;
    margin: 0 0 0 2px;
    width: 22px;
}
.RadGrid .rgFilterBox {
    border-style: solid;
    border-width: 1px;
    font-size: 12px;
    height: 15px;
    margin: 0;
    padding: 2px 1px 3px;
    vertical-align: middle;
}
.rgFilterRow .riSingle .riTextBox {
    height: 22px;
}
.RadGrid .rgFilterRow .RadInput, .RadGrid .rgFilterRow .RadRating {
    display: inline-block;
    vertical-align: middle;
}
* + html .RadGrid .rgFilterRow .RadRating, * html .RadGrid .rgFilterRow .RadRating {
    display: inline;
}
.GridContextMenu .rmLeftImage {
    background-color: transparent;
    background-repeat: no-repeat;
}
.RadMenu .rmGroup .rgHCMItem .rmText {
    padding: 6px 5px 5px 30px;
    width: 161px;
}
.rgHCMItem .rgHCMClear, .rgHCMItem .rgHCMShow, .rgHCMItem .rgHCMAnd, .rgHCMItem .rgHCMFilter {
    display: block;
}
.rgHCMItem .rgHCMShow, .rgHCMItem .rgHCMAnd {
    line-height: 12px;
    padding-top: 5px;
}
.rgHCMItem .rgHCMClear, .rgHCMItem .rgHCMShow, .rgHCMItem .RadComboBox {
    margin: 0 0 5px;
}
.rgHCMItem .rgHCMAnd {
    margin: 5px 0;
}
.rgHCMItem .rgHCMFilter {
    margin-top: 11px;
}
.rgHCMItem .rgHCMClear, .rgHCMItem .rgHCMFilter {
    border-radius: 3px;
    border-style: solid;
    border-width: 1px;
    cursor: pointer;
    font-size: 12px;
    padding: 1px 0;
    width: 160px;
}
.RadGrid .rgGroupPanel {
    height: 24px;
}
.RadGrid .rgGroupItem {
    font-weight: normal;
    line-height: 20px;
    padding: 0 2px 1px 3px;
    vertical-align: middle;
}
.RadGrid .rgGroupHeader td {
    padding-bottom: 0;
    padding-top: 0;
}
.RadGrid .rgGroupHeader td p {
    display: inline;
    margin: 0;
    padding: 0 10px;
}
.RadGrid .rgGroupHeader td div div {
    padding: 0 10px;
    top: -0.8em;
}
.RadGrid table.rgMasterTable tr .rgGroupCol, .RadGrid table.rgMasterTable tr .rgExpandCol {
    padding-left: 0;
    padding-right: 0;
    text-align: center;
}
* html .RadGrid .rgGroupHeader td div div {
    top: 0;
}
.RadGrid .rgGroupHeader td div div div {
    border: 0 none;
    padding: 0;
    top: 0;
}
.RadGrid .rgUpdate, .RadGrid .rgCancel {
    height: 18px;
    width: 18px;
}
.RadGrid .rgDetailTable {
    border-style: solid;
    border-width: 1px 0 1px 1px;
}
.RadGrid .rgAdd, .RadGrid .rgRefresh {
    height: 18px;
    vertical-align: bottom;
    width: 18px;
}
* + html .RadGrid .rgPager .rgPagerButton, * + html .RadGrid .rgPagerLabel, * + html .RadGrid .rgPager .RadComboBox, * + html .RadGrid .rgAdd, * + html .RadGrid .rgRefresh {
    vertical-align: middle;
}
* html .RadGrid .rgPager .rgPagerButton, * html .RadGrid .rgPagerLabel {
    vertical-align: middle;
}
* html .RadGrid .rgPager .RadComboBox, * html .RadGrid .rgPager .RadInput {
    vertical-align: middle;
}
* html .RadGrid .rgAdd, * html .RadGrid .rgRefresh {
    vertical-align: middle;
}
.RadGrid .rgEdit, .RadGrid .rgDel {
    display: inline-block;
    height: 15px;
    text-indent: -9999px;
    width: 15px;
}
.rgPager thead, .rgPager caption, .rgCommandTable thead {
    display: none;
}
.RadGridRTL .rgHeader, .RadGridRTL .rgResizeCol {
    text-align: right;
}
.RadGridRTL .rgPager .rgStatus {
    border-left-width: 1px;
    border-right: 0 none;
}
.RadGridRTL .rgWrap {
    float: right;
}
.RadGridRTL .rgArrPart1 {
    padding-left: 0;
    padding-right: 10px;
}
.RadGridRTL .rgPageFirst, .RadGridRTL .rgPagePrev {
    margin-left: 1px;
    margin-right: 0;
}
.RadGridRTL .rgPageNext, .RadGridRTL .rgPageLast {
    margin-left: 0;
    margin-right: 1px;
}
.RadGridRTL .rgInfoPart {
    float: left;
}
.RadGridRTL .rgNumPart {
    width: 220px;
}
.RadGridRTL .rgNumPart a {
    float: right;
}
.RadGridRTL .rgDetailTable {
    border-left-width: 0;
    border-right-width: 1px;
}
.RadGridRTL input.rgRefresh, .RadGridRTL input.rgAdd {
    margin: 0 0 0 4px;
}
.RadGridRTL .rgInfoPart {
    text-align: left;
}
.RadGridRTL .rgSliderLabel {
    float: right;
}
.RadGridRTL .rgPager div.RadSlider {
    float: right;
    margin: 0 0 0 10px;
}
.RadComboBox_Web20 {
    color: black;
    font: 12px/16px "Segoe UI",Arial,Helvetica,sans-serif;
}
.RadComboBox_Web20 .rcbInputCell, .RadComboBox_Web20 .rcbArrowCell {
    background-image: url("/WebResource.axd?d=_NEytZs8vU2sBJ0rhxxm6eQ5GUtnVkfFNSdfXywBstnjhPb4FYjIkVXzLgn0Xh-pHPiunl7v7U3D66ojooWd67cYIqVoZK1iPx4zhzS7VAzGSE7iPxzpMMa6M4HJhJymSHq6oSoAZ5FBbGOr7jeR6KRlOWG_TUZW8doHIRQH2iqaJuUL0&t=635418873532749162");
}
.RadComboBox_Web20 .rcbInputCellLeft {
    background-position: 0 0;
}
.RadComboBox_Web20 .rcbInputCellRight {
    background-position: 100% 0;
}
.RadComboBox_Web20 .rcbInput {
    color: black;
    font: 12px/16px "Segoe UI",Arial,Helvetica,sans-serif;
}
.RadComboBox_Web20 .rcbEmptyMessage {
    color: #737373;
    font-style: italic;
}
.RadComboBox_Web20 .rcbArrowCellLeft {
    background-position: 0 -176px;
}
.RadComboBox_Web20 .rcbArrowCellRight {
    background-position: -18px -176px;
}
.RadComboBox_Web20 .rcbArrowCellLeft.rcbArrowCellHidden {
    background-position: 0 0;
}
.RadComboBox_Web20 .rcbArrowCellRight.rcbArrowCellHidden {
    background-position: 100% 0;
}
.RadComboBox_Web20 .rcbHovered .rcbInputCellLeft {
    background-position: 0 -22px;
}
.RadComboBox_Web20 .rcbHovered .rcbInputCellRight {
    background-position: 100% -22px;
}
.RadComboBox_Web20 .rcbHovered .rcbInput {
    color: black;
}
.RadComboBox_Web20 .rcbHovered .rcbArrowCellLeft {
    background-position: -36px -176px;
}
.RadComboBox_Web20 .rcbHovered .rcbArrowCellRight {
    background-position: -54px -176px;
}
.RadComboBox_Web20 .rcbHovered .rcbArrowCellLeft.rcbArrowCellHidden {
    background-position: 0 -22px;
}
.RadComboBox_Web20 .rcbHovered .rcbArrowCellRight.rcbArrowCellHidden {
    background-position: 100% -22px;
}
.RadComboBox_Web20 .rcbFocused .rcbInputCellLeft {
    background-position: 0 -44px;
}
.RadComboBox_Web20 .rcbFocused .rcbInputCellRight {
    background-position: 100% -44px;
}
.RadComboBox_Web20 .rcbFocused .rcbInput {
    color: black;
}
.RadComboBox_Web20 .rcbFocused .rcbArrowCellLeft {
    background-position: -72px -176px;
}
.RadComboBox_Web20 .rcbFocused .rcbArrowCellRight {
    background-position: -90px -176px;
}
.RadComboBox_Web20 .rcbFocused .rcbArrowCellLeft.rcbArrowCellHidden {
    background-position: 0 -44px;
}
.RadComboBox_Web20 .rcbFocused .rcbArrowCellRight.rcbArrowCellHidden {
    background-position: 100% -44px;
}
.RadComboBox_Web20 .rcbDisabled .rcbInputCellLeft {
    background-position: 0 -66px;
}
.RadComboBox_Web20 .rcbDisabled .rcbInputCellRight {
    background-position: 100% -66px;
}
.RadComboBox_Web20 .rcbDisabled .rcbInput {
    color: #777;
}
.RadComboBox_Web20 .rcbDisabled .rcbArrowCellLeft {
    background-position: -108px -176px;
}
.RadComboBox_Web20 .rcbDisabled .rcbArrowCellRight {
    background-position: -126px -176px;
}
.RadComboBox_Web20 .rcbDisabled .rcbArrowCellLeft.rcbArrowCellHidden {
    background-position: 0 -66px;
}
.RadComboBox_Web20 .rcbDisabled .rcbArrowCellRight.rcbArrowCellHidden {
    background-position: 100% -66px;
}
.RadComboBox_Web20 .rcbReadOnly .rcbInputCellLeft {
    background-position: 0 -88px;
}
.RadComboBox_Web20 .rcbReadOnly .rcbInputCellRight {
    background-position: 100% -88px;
}
.RadComboBox_Web20 .rcbReadOnly .rcbInput {
    color: white;
}
.RadComboBox_Web20 .rcbReadOnly .rcbArrowCellLeft {
    background-position: -144px -176px;
}
.RadComboBox_Web20 .rcbReadOnly .rcbArrowCellRight {
    background-position: -162px -176px;
}
.RadComboBox_Web20 .rcbReadOnly .rcbArrowCellLeft.rcbArrowCellHidden {
    background-position: 0 -88px;
}
.RadComboBox_Web20 .rcbReadOnly .rcbArrowCellRight.rcbArrowCellHidden {
    background-position: 100% -88px;
}
.RadComboBox_Web20 .rcbHovered .rcbReadOnly .rcbInputCellLeft {
    background-position: 0 -110px;
}
.RadComboBox_Web20 .rcbHovered .rcbReadOnly .rcbInputCellRight {
    background-position: 100% -110px;
}
.RadComboBox_Web20 .rcbHovered .rcbReadOnly .rcbInput {
    color: #0f3789;
}
.RadComboBox_Web20 .rcbHovered .rcbReadOnly .rcbArrowCellLeft {
    background-position: -180px -176px;
}
.RadComboBox_Web20 .rcbHovered .rcbReadOnly .rcbArrowCellRight {
    background-position: -198px -176px;
}
.RadComboBox_Web20 .rcbHovered .rcbReadOnly .rcbArrowCellLeft.rcbArrowCellHidden {
    background-position: 0 -110px;
}
.RadComboBox_Web20 .rcbHovered .rcbReadOnly .rcbArrowCellRight.rcbArrowCellHidden {
    background-position: 100% -110px;
}
.RadComboBox_Web20 .rcbFocused .rcbReadOnly .rcbInputCellLeft {
    background-position: 0 -132px;
}
.RadComboBox_Web20 .rcbFocused .rcbReadOnly .rcbInputCellRight {
    background-position: 100% -132px;
}
.RadComboBox_Web20 .rcbFocused .rcbReadOnly .rcbInput {
    color: white;
}
.RadComboBox_Web20 .rcbFocused .rcbReadOnly .rcbArrowCellLeft {
    background-position: -216px -176px;
}
.RadComboBox_Web20 .rcbFocused .rcbReadOnly .rcbArrowCellRight {
    background-position: -234px -176px;
}
.RadComboBox_Web20 .rcbFocused .rcbReadOnly .rcbArrowCellLeft.rcbArrowCellHidden {
    background-position: 0 -132px;
}
.RadComboBox_Web20 .rcbFocused .rcbReadOnly .rcbArrowCellRight.rcbArrowCellHidden {
    background-position: 100% -132px;
}
.RadComboBox_Web20 .rcbDisabled .rcbReadOnly .rcbInputCellLeft {
    background-position: 0 -154px;
}
.RadComboBox_Web20 .rcbDisabled .rcbReadOnly .rcbInputCellRight {
    background-position: 100% -154px;
}
.RadComboBox_Web20 .rcbDisabled .rcbReadOnly .rcbInput {
    color: #777;
}
.RadComboBox_Web20 .rcbDisabled .rcbReadOnly .rcbArrowCellLeft {
    background-position: -252px -176px;
}
.RadComboBox_Web20 .rcbDisabled .rcbReadOnly .rcbArrowCellRight {
    background-position: -270px -176px;
}
.RadComboBox_Web20 .rcbDisabled .rcbReadOnly .rcbArrowCellLeft.rcbArrowCellHidden {
    background-position: 0 -154px;
}
.RadComboBox_Web20 .rcbDisabled .rcbReadOnly .rcbArrowCellRight.rcbArrowCellHidden {
    background-position: 100% -154px;
}
.RadComboBoxDropDown_Web20 {
    background: none repeat scroll 0 0 white;
    border-color: #6788be;
    color: black;
    font: 12px/16px "Segoe UI",Arial,Helvetica,sans-serif;
}
.RadComboBoxDropDown_Web20 .rcbHeader, .RadComboBoxDropDown_Web20 .rcbFooter {
    background-color: #93b4df;
    color: black;
}
.RadComboBoxDropDown_Web20 .rcbHeader {
    border-bottom-color: #6788be;
}
.RadComboBoxDropDown_Web20 .rcbFooter {
    border-top-color: #6788be;
}
.RadComboBoxDropDown_Web20 .rcbHovered {
    background: none repeat scroll 0 0 #e7f1ff;
    color: black;
}
.RadComboBoxDropDown_Web20 .rcbDisabled {
    background-color: transparent;
    color: #777;
}
.RadComboBoxDropDown_Web20 .rcbLoading {
    background: none repeat scroll 0 0 #e7f1ff;
    color: black;
}
.RadComboBoxDropDown_Web20 .rcbItem em, .RadComboBoxDropDown_Web20 .rcbHovered em {
    background: none repeat scroll 0 0 #e7f1ff;
    color: black;
}
.RadComboBoxDropDown_Web20 .rcbCheckAllItems {
    background-color: #93b4df;
    color: black;
}
.RadComboBoxDropDown_Web20 .rcbCheckAllItemsHovered {
    background-color: #93b4df;
    color: black;
}
.RadComboBoxDropDown_Web20 .rcbMoreResults {
    background-color: #93b4df;
    border-top-color: #6788be;
    color: black;
}
.RadComboBoxDropDown_Web20 .rcbMoreResults a {
    background-image: url("/WebResource.axd?d=_NEytZs8vU2sBJ0rhxxm6eQ5GUtnVkfFNSdfXywBstnjhPb4FYjIkVXzLgn0Xh-pHPiunl7v7U3D66ojooWd67cYIqVoZK1iPx4zhzS7VAzGSE7iPxzpMMa6M4HJhJymSHq6oSoAZ5FBbGOr7jeR6KRlOWG_TUZW8doHIRQH2iqaJuUL0&t=635418873532749162");
    background-position: -308px -181px;
}
.RadComboBoxDropDown_Web20 .rcbSeparator {
    background: none repeat scroll 0 0 #8a8a8a;
    color: #fff;
}
.RadPanelBar_Web20 {
    background: none repeat scroll 0 0 #fff;
}
.RadPanelBar_Web20 .rpRootGroup {
    border-color: #7496ce;
}
.RadPanelBar_Web20 a.rpLink, .RadPanelBar_Web20 .rpTemplate, .RadPanelBar_Web20 div.rpHeaderTemplate {
    font: 12px/24px "Segoe UI",Arial,sans-serif;
}
.RadPanelBar_Web20 a.rpLink, .RadPanelBar_Web20 div.rpHeaderTemplate {
    background-color: #8fb0db;
    background-image: url("/WebResource.axd?d=ht4lSXi6jJOL5h4Z0WHb_bZMatB4jnuGA7nW_pVz4UB16DQ0agOcwikCNpCujKjtT_dWY8TbKnzLnerLWkmbJ6rgk7xPvUZLcP_BhG6ppKtwMQX1xje8Jim-DVOLB8sVHSq5S-nR50FugIUcJ9D5444LWsS3ngfvEdLG3CdGybHb3YIT0&t=635418873532749162");
    border-color: #89aee5;
    color: #fff;
}
.RadPanelBar_Web20 .rpOut {
    border-color: #89aee5;
}
* html .RadPanelBar_Web20 .rpRootGroup .rpHeaderTemplate {
    height: 25px;
}
.RadPanelBar_Web20 a.rpFocused, .RadPanelBar_Web20 div.rpFocused, .RadPanelBar_Web20 a.rpLink:hover {
    background-color: #ffca5e;
    color: #0f3789;
}
.RadPanelBar_Web20 a.rpExpanded, .RadPanelBar_Web20 div.rpExpanded {
    border-color: #002d96;
}
.RadPanelBar_Web20 a.rpSelected, .RadPanelBar_Web20 div.rpSelected, .RadPanelBar_Web20 a.rpSelected:hover {
    background-color: #c3d8f1;
    border-color: #9eb6ce;
    color: #0f3789;
}
.RadPanelBar_Web20 a.rpDisabled, .RadPanelBar_Web20 div.rpDisabled, .RadPanelBar_Web20 a.rpDisabled:hover {
    color: #ccc;
}
.RadPanelBar_Web20 a.rpDisabled:hover {
    border-color: #002d96;
}
.RadPanelBar_Web20 a.rpDisabled .rpOut, .RadPanelBar_Web20 a.rpDisabled:hover .rpOut {
    border-color: #89aee5;
}
.RadPanelBar_Web20 .rpGroup {
    background-color: #fff;
}
.RadPanelBar_Web20 .rpGroup a.rpLink, .RadPanelBar_Web20 .rpGroup div.rpHeaderTemplate, .RadPanelBar_Web20 .rpGroup .rpTemplate {
    background-color: transparent;
    background-image: none;
    color: #000;
    line-height: 20px;
    min-height: 20px;
}
* html .RadPanelBar_Web20 .rpGroup a.rpLink, * html .RadPanelBar_Web20 .rpGroup div.rpHeaderTemplate {
    height: 20px;
}
.RadPanelBar_Web20 .rpGroup div.rpHeaderTemplate, .RadPanelBar_Web20 .rpGroup a.rpLink {
    background-color: transparent;
    background-image: none;
    margin: 0 1px;
    padding: 1px;
}
.RadPanelBar_Web20 .rpGroup .rpOut {
    border-bottom: 0 none;
    margin-right: 3px;
}
.RadPanelBar_Web20 .rpGroup .rpItem a.rpLink:hover {
    background-color: #e7f1ff;
    border: 1px solid #bcd2f1;
    color: #0f3789;
    padding: 0;
}
.RadPanelBar_Web20 .rpGroup li.rpItem .rpSelected, .RadPanelBar_Web20 .rpGroup .rpItem a.rpSelected:hover {
    background-color: #d4ffbc;
    border: 1px solid #85c843;
    color: #0e4302;
    padding: 0;
}
.RadPanelBar_Web20 .rpGroup li.rpItem .rpDisabled, .RadPanelBar_Web20 .rpGroup .rpItem a.rpDisabled:hover, .RadPanelBar_Web20 .rpGroup .rpItem a.rpDisabled:hover .rpOut {
    background-image: none;
}
.RadPanelBar_Web20 .rpGroup li.rpItem .rpDisabled, .RadPanelBar_Web20 .rpGroup .rpItem a.rpDisabled:hover {
    background-color: #fff;
    border: 0 none;
    color: #999;
    padding: 1px;
    text-decoration: none;
}
.RadPanelBar_Web20 .rpGroup .rpItem a.rpDisabled:hover .rpOut {
    border-bottom: 0 none;
}
.RadPanelBar_Web20 a.rpFocused, .RadPanelBar_Web20 div.rpFocused, .RadPanelBar_Web20 a.rpLink:hover, .RadPanelBar_Web20 a.rpExpanded:hover {
    background-position: 0 -200px;
}
.RadPanelBar_Web20 a.rpLink, .RadPanelBar_Web20 div.rpHeaderTemplate, .RadPanelBar_Web20 a.rpExpanded, .RadPanelBar_Web20 div.rpExpanded, .RadPanelBar_Web20 .rpItem a.rpLinkExpandHovered {
    background-position: 0 0;
}
.RadPanelBar_Web20 a.rpSelected, .RadPanelBar_Web20 div.rpSelected, .RadPanelBar_Web20 a.rpSelected:hover {
    background-position: 0 -400px;
}
.RadPanelBar_Web20 .rpFirst a.rpLink, .RadPanelBar_Web20 .rpFirst div.rpHeaderTemplate, .RadPanelBar_Web20 li.rpFirst .rpExpanded {
    background-position: 0 -1px;
}
.RadPanelBar_Web20 li.rpFirst .rpFocused, .RadPanelBar_Web20 .rpFirst a.rpLink:hover {
    background-position: 0 -201px;
}
.RadPanelBar_Web20 li.rpFirst .rpSelected, .RadPanelBar_Web20 .rpFirst a.rpSelected:hover {
    background-position: 0 -401px;
}
.RadPanelBar_Web20 a.rpDisabled:hover {
    cursor: default;
}
.RadPanelBar_Web20 a.rpDisabled, .RadPanelBar_Web20 div.rpDisabled, .RadPanelBar_Web20 a.rpDisabled:hover, .RadPanelBar_Web20 a.rpDisabled:hover .rpOut {
    background-position: 0 0;
}
.RadPanelBar_Web20 .rpExpandable span.rpExpandHandle, .RadPanelBar_Web20 .rpExpanded span.rpExpandHandle {
    background-color: transparent;
    background-image: url("/WebResource.axd?d=7p9lD7JufGpdxv1yOoN_9AziLMm3Di7dMWaWaFnxDB1aX7hPVmMCANpXyjWP9DbLNvvAdHuXA-bjYzqwh0MExHoU-qbKKZRgGSgaN2TmwP1gQCQz4kSylDCceE3o02ik_QWsedECYUyvUTUjnBnKrvXdwYT5Ztlj7cAoajE5FRrippZ20&t=635418873532749162");
    background-repeat: no-repeat;
}
.RadPanelBar_Web20_rtl .rpGroup .rpText {
    padding: 0 10px;
}
.RadPanelBar_Web20 .rpExpandable span.rpExpandHandle, .RadPanelBar_Web20 a.rpExpandable:hover .rpNavigation .rpExpandHandle {
    background-position: 0 -5px;
}
.RadPanelBar_Web20 a.rpExpandable:hover .rpExpandHandle, .RadPanelBar_Web20 a.rpExpandable:hover .rpNavigation .rpExpandHandleHovered {
    background-position: 100% -5px;
}
.RadPanelBar_Web20 .rpExpanded span.rpExpandHandle, .RadPanelBar_Web20 a.rpExpanded:hover .rpNavigation .rpExpandHandle {
    background-position: 0 -181px;
}
.RadPanelBar_Web20 a.rpExpanded:hover .rpExpandHandle, .RadPanelBar_Web20 a.rpExpanded:hover .rpNavigation .rpExpandHandleHovered {
    background-position: 100% -181px;
}
.RadPanelBar_Web20 div.rpExpandable .rpExpandHandleHovered {
    background-position: 0 -5px;
}
.RadPanelBar_Web20 div.rpExpanded .rpExpandHandleHovered {
    background-position: 0 -181px;
}
.RadPanelBar_Web20 .rpGroup a.rpLink, .RadPanelBar_Web20 .rpGroup div.rpHeaderTemplate, .RadPanelBar_Web20 .rpGroup a.rpLink .rpOut {
    background-position: 0 200px;
}
.RadPanelBar_Web20 .rpGroup a.rpLink:hover, .RadPanelBar_Web20 ul.rpGroup .rpFocused {
    background-position: 100% -22px;
}
.RadPanelBar_Web20 .rpGroup a.rpLink:hover .rpOut, .RadPanelBar_Web20 .rpGroup a.rpFocused .rpOut {
    background-position: 0 0;
}
.RadPanelBar_Web20 ul.rpGroup .rpSelected, .RadPanelBar_Web20 .rpGroup a.rpSelected:hover {
    background-color: transparent;
    background-position: 100% -222px;
}
.RadPanelBar_Web20 .rpGroup a.rpSelected .rpOut, .RadPanelBar_Web20 .rpGroup a.rpSelected:hover .rpOut {
    background-position: 0 -200px;
}
.RadPanelBar_Web20 ul.rpGroup .rpExpandable .rpExpandHandle, .RadPanelBar_Web20 .rpGroup a.rpExpandable:hover .rpNavigation .rpExpandHandle {
    background-position: 0 -343px;
}
.RadPanelBar_Web20 .rpGroup a.rpExpandable:hover .rpExpandHandle, .RadPanelBar_Web20 .rpGroup a.rpExpandable:hover .rpNavigation .rpExpandHandleHovered {
    background-position: 100% -343px;
}
.RadPanelBar_Web20 ul.rpGroup .rpExpanded .rpExpandHandle, .RadPanelBar_Web20 .rpGroup a.rpExpanded:hover .rpNavigation .rpExpandHandle {
    background-position: 0 -482px;
}
.RadPanelBar_Web20 .rpGroup a.rpExpanded:hover .rpExpandHandle, .RadPanelBar_Web20 .rpGroup a.rpExpanded:hover .rpNavigation .rpExpandHandleHovered {
    background-position: 100% -482px;
}
.RadPanelBar_Web20 .rpGroup div.rpExpandable .rpExpandHandleHovered {
    background-position: 0 -343px;
}
.RadPanelBar_Web20 .rpGroup div.rpExpanded .rpExpandHandleHovered {
    background-position: 0 -482px;
}
.RadPanelBar .rpGroup a.rpExpanded span.rpExpandHandle, .RadPanelBar .rpGroup a.rpExpandable span.rpExpandHandle {
    margin: 4px 0 0;
}
.RadPanelBar_rtl .rpGroup a.rpExpanded span.rpExpandHandle, .RadPanelBar_rtl .rpGroup a.rpExpandable span.rpExpandHandle {
    margin: 4px 0 0 3px;
}
.RadGrid_Web20 {
    background: none repeat scroll 0 0 #fff;
    border: 1px solid #4e75b3;
    color: #000;
    font: 12px/16px "segoe ui",arial,sans-serif;
}
.RadGrid_Web20 .rgMasterTable, .RadGrid_Web20 .rgDetailTable, .RadGrid_Web20 .rgGroupPanel table, .RadGrid_Web20 .rgCommandRow table, .RadGrid_Web20 .rgEditForm table, .RadGrid_Web20 .rgPager table {
    font: 12px/16px "segoe ui",arial,sans-serif;
}
.GridToolTip_Web20 {
    font: 12px/16px "segoe ui",arial,sans-serif;
}
.RadGrid_Web20 .rgHeader:first-child, .RadGrid_Web20 th.rgResizeCol:first-child, .RadGrid_Web20 .rgFilterRow > td:first-child, .RadGrid_Web20 .rgRow > td:first-child, .RadGrid_Web20 .rgAltRow > td:first-child {
    border-left-width: 0;
    padding-left: 8px;
}
.RadGrid_Web20 .rgSave, .RadGrid_Web20 .rgAdd, .RadGrid_Web20 .rgRefresh, .RadGrid_Web20 .rgEdit, .RadGrid_Web20 .rgDel, .RadGrid_Web20 .rgFilter, .RadGrid_Web20 .rgPagePrev, .RadGrid_Web20 .rgPageNext, .RadGrid_Web20 .rgPageFirst, .RadGrid_Web20 .rgPageLast, .RadGrid_Web20 .rgExpand, .RadGrid_Web20 .rgCollapse, .RadGrid_Web20 .rgSortAsc, .RadGrid_Web20 .rgSortDesc, .RadGrid_Web20 .rgUpdate, .RadGrid_Web20 .rgCancel, .RadGrid_Web20 .rgUngroup, .RadGrid_Web20 .rgExpXLS, .RadGrid_Web20 .rgExpDOC, .RadGrid_Web20 .rgExpPDF, .RadGrid_Web20 .rgExpCSV {
    background-image: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418873532749162");
}
.RadGrid_Web20 .rgHeaderDiv {
    background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418873532749162") repeat-x scroll 0 -7050px #537ab8;
}
.rgTwoLines .rgHeaderDiv {
    background-position: 0 -6550px;
}
.RadGrid_Web20 .rgHeader, .RadGrid_Web20 th.rgResizeCol, .RadGrid_Web20 .rgHeaderWrapper {
    -moz-border-bottom-colors: none;
    -moz-border-left-colors: none;
    -moz-border-right-colors: none;
    -moz-border-top-colors: none;
    background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418873532749162") repeat-x scroll 0 -2300px #7fa5d7;
    border-color: #fff #6e8ec2 #204582 #355a99;
    border-image: none;
    border-right: 1px solid #6e8ec2;
    border-style: solid;
    border-width: 0 1px 1px;
}
.RadGrid_Web20 .rgMultiHeaderRow th.rgHeader, .RadGrid_Web20 .rgMultiHeaderRow th.rgResizeCol {
    -moz-border-bottom-colors: none;
    -moz-border-left-colors: none;
    -moz-border-right-colors: none;
    -moz-border-top-colors: none;
    background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418873532749162") repeat-x scroll 0 -2300px #7fa5d7;
    border-color: #fff #6e8ec2 #204582 #355a99;
    border-image: none;
    border-right: 1px solid #6e8ec2;
    border-style: solid;
    border-width: 0 1px 1px;
}
.RadGrid_Web20 .rgHeaderDiv {
    border-right-color: #6788be;
}
.RadGrid_Web20 th.rgSorted {
    background-color: #c0d5ee;
    background-position: 0 -2600px;
    border-color: #fff #8eaad6 #204582 #204582;
}
.RadGrid_Web20 .rgHeader {
    color: #fff;
}
.RadGrid_Web20 .rgHeader a {
    color: #fff;
}
.RadGrid_Web20 th.rgSorted {
    color: #0f3789;
}
.RadGrid_Web20 th.rgSorted a {
    color: #0f3789;
}
.RadGrid_Web20 .rgRow td, .RadGrid_Web20 .rgAltRow td, .RadGrid_Web20 .rgEditRow td, .RadGrid_Web20 .rgFooter td {
    border-style: solid;
    border-width: 0 1px 1px;
}
.RadGrid_Web20 .rgRow td, .RadGrid_Web20 .rgAltRow td {
    border-color: #fff #fff #cfd9e7 #829cbf;
}
.RadGrid_Web20 .rgRow .rgSorted, .RadGrid_Web20 .rgAltRow .rgSorted {
    background-color: #e3eeff;
    border-bottom-color: #cfd9e7;
}
.RadGrid_Web20 .rgSelectedRow .rgSorted, .RadGrid_Web20 .rgActiveRow .rgSorted, .RadGrid_Web20 .rgHoveredRow .rgSorted, .RadGrid_Web20 .rgEditRow .rgSorted {
    background-color: transparent;
}
.RadGrid_Web20 .rgRow a, .RadGrid_Web20 .rgAltRow a, .RadGrid_Web20 .rgEditRow a, .RadGrid_Web20 .rgFooter a, .RadGrid_Web20 .rgEditForm a {
    color: #0f3789;
}
.RadGrid_Web20 .rgMasterTable .rgSelectedCell, .RadGrid_Web20 .rgSelectedRow {
    background: none repeat scroll 0 0 #cdffb1;
}
* + html .RadGrid_Web20 .rgSelectedRow .rgSorted, * html .RadGrid_Web20 .rgSelectedRow .rgSorted {
    background-color: #cdffb1;
}
.RadGrid_Web20 .rgMasterTable .rgActiveCell, .RadGrid_Web20 .rgActiveRow, .RadGrid_Web20 .rgHoveredRow {
    background: none repeat scroll 0 0 #e3eeff;
}
* + html .RadGrid_Web20 .rgActiveRow .rgSorted, * + html .RadGrid_Web20 .rgHoveredRow .rgSorted {
    background-color: #e3eeff;
}
* html .RadGrid_Web20 .rgActiveRow .rgSorted, * html .RadGrid_Web20 .rgHoveredRow .rgSorted {
    background-color: #e3eeff;
}
.RadGrid_Web20 .rgEditRow {
    background: none repeat scroll 0 0 #fff;
}
* + html .RadGrid_Web20 .rgEditRow .rgSorted, * html .RadGrid_Web20 .rgEditRow .rgSorted {
    background-color: #fff;
}
.RadGrid_Web20 .rgSelectedRow td, .RadGrid_Web20 .rgActiveRow td, .RadGrid_Web20 .rgHoveredRow td, .RadGrid_Web20 .rgEditRow td {
    border-left-width: 0;
    border-right-width: 0;
    padding-left: 8px;
    padding-right: 8px;
}
.RadGrid_Web20 .rgSelectedRow td {
    border-bottom-color: #71bf25;
}
.RadGrid_Web20 .rgSelectedRow td.rgSorted {
    border-bottom-color: #71bf25;
}
.RadGrid_Web20 .rgActiveRow td, .RadGrid_Web20 .rgHoveredRow td, .RadGrid_Web20 .rgActiveRow td.rgSorted, .RadGrid_Web20 .rgHoveredRow td.rgSorted {
    border-bottom-color: #b1caee;
}
.RadGrid_Web20 .rgEditRow td {
    border-color: #fff #fff #4e75b3;
}
.RadGrid_Web20 .rgEditRow td.rgSorted {
    border-color: #fff #fff #4e75b3;
}
.RadGrid_Web20 .rgDrag {
    background-image: url("/WebResource.axd?d=Qz_nTZWj4fA2Tc_Fp7SbqTtsHE0KBEeGv91kRVGfunHdllk6fVTj3elnccs7Cn_RFmMWmqwaE5l3Tu4tR7WA01yWZ4Q6GDBbhBWiwvJcD2DuTatQXOLcABamI927bh8nB9ElS9wg2SkpwfbEcc7MRRcjNuNC8ne9nydbuUpBx5xD7OEn0&t=635418873532749162");
}
.RadGrid_Web20 .rgFooterDiv, .RadGrid_Web20 .rgFooter, .RadGrid_Web20 .rgFooterWrapper {
    background: none repeat scroll 0 0 #e3eeff;
}
.RadGrid_Web20 .rgFooter td, .RadGrid_Web20 .rgFooterWrapper {
    border-color: #829cbf #e3eeff #fff;
    border-top-width: 1px;
}
.RadGrid_Web20 .rgPager .rgStatus {
    -moz-border-bottom-colors: none;
    -moz-border-left-colors: none;
    -moz-border-right-colors: none;
    -moz-border-top-colors: none;
    border-color: #829cbf #8caabb #fff;
    border-image: none;
    border-style: solid;
    border-width: 1px 1px 1px 0;
}
.RadGrid_Web20 .rgStatus div {
    background-image: url("/WebResource.axd?d=ASQBM4CNRfJXUqiVnHD5pOKMUx18tY1563HVr6TN9Xn_MkilzIY0y6HgCXQBHr3xSa48lTJdMvx0QGtqC9KvzDwsoZYSFhZCb9p7otdvpBeI8BEMfsIUqjYmXNmDq4W1mH5Zwd2txZt-k_33QPvCJ7znqNqAhtPuq0xIZPmJZBbx_uIA0&t=635418873532749162");
}
.RadGrid_Web20 .rgPager {
    background: none repeat scroll 0 0 #e3eeff;
}
.RadGrid_Web20 td.rgPagerCell {
    -moz-border-bottom-colors: none;
    -moz-border-left-colors: none;
    -moz-border-right-colors: none;
    -moz-border-top-colors: none;
    border-color: #829cbf #fff #fff;
    border-image: none;
    border-style: solid;
    border-width: 1px 0 1px 1px;
}
.RadGrid_Web20 .rgInfoPart {
    color: #666;
}
.RadGrid_Web20 .rgInfoPart strong {
    color: #000;
}
.RadGrid_Web20 .rgPageFirst {
    background-position: 0 -550px;
}
.RadGrid_Web20 .rgPageFirst:hover {
    background-position: 0 -600px;
}
.RadGrid_Web20 .rgPagePrev {
    background-position: 0 -700px;
}
.RadGrid_Web20 .rgPagePrev:hover {
    background-position: 0 -750px;
}
.RadGrid_Web20 .rgPageNext {
    background-position: 0 -850px;
}
.RadGrid_Web20 .rgPageNext:hover {
    background-position: 0 -900px;
}
.RadGrid_Web20 .rgPageLast {
    background-position: 0 -1000px;
}
.RadGrid_Web20 .rgPageLast:hover {
    background-position: 0 -1050px;
}
.RadGrid_Web20 .rgPager .rgPagerButton {
    background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418873532749162") repeat-x scroll 0 -1550px #466abb;
    border-color: #00002a #00002a #000;
    color: #fff;
    font: 12px/12px "segoe ui",arial,sans-serif;
}
.RadGrid_Web20 .rgNumPart a {
    color: #000;
}
.RadGrid_Web20 .rgNumPart a:hover, .RadGrid_Web20 .rgNumPart a.rgCurrentPage {
    background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418873532749162") no-repeat scroll 0 0 rgba(0, 0, 0, 0);
}
.RadGrid_Web20 .rgNumPart a:hover span, .RadGrid_Web20 .rgNumPart a.rgCurrentPage span {
    background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418873532749162") no-repeat scroll 0 0 rgba(0, 0, 0, 0);
}
.RadGrid_Web20 .rgNumPart a:hover {
    background-position: 100% -1250px;
    color: #0f3789;
}
.RadGrid_Web20 .rgNumPart a:hover span {
    background-position: 0 -1150px;
}
.RadGrid_Web20 .rgNumPart a.rgCurrentPage {
    background-position: 100% -1450px;
    color: #0d202b;
}
.RadGrid_Web20 .rgNumPart a.rgCurrentPage:hover {
    background-position: 100% -1450px;
    color: #0d202b;
}
.RadGrid_Web20 .rgNumPart a.rgCurrentPage span, .RadGrid_Web20 .rgNumPart a.rgCurrentPage:hover span {
    background-position: 0 -1350px;
}
.RadGrid_Web20 .rgHeader .rgSortAsc {
    background-position: 3px -247px;
    height: 10px;
}
.RadGrid_Web20 .rgHeader .rgSortDesc {
    background-position: 3px -197px;
    height: 10px;
}
.RadGrid_Web20 .rgSorted .rgSortAsc {
    background-position: 3px -147px;
}
.RadGrid_Web20 .rgSorted .rgSortDesc {
    background-position: 3px -97px;
}
.GridReorderTop_Web20 {
    background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418873532749162") no-repeat scroll 0 0 rgba(0, 0, 0, 0);
}
.GridReorderBottom_Web20 {
    background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418873532749162") no-repeat scroll 0 -50px rgba(0, 0, 0, 0);
}
.RadGrid_Web20 .rgFilterRow {
    background: none repeat scroll 0 0 #537ab8;
}
.RadGrid_Web20 .rgFilterRow td {
    -moz-border-bottom-colors: none;
    -moz-border-left-colors: none;
    -moz-border-right-colors: none;
    -moz-border-top-colors: none;
    border-color: #537ab8 #537ab8 #3c5c90;
    border-image: none;
    border-right: 1px solid #537ab8;
    border-style: solid;
    border-width: 0 1px 1px;
}
.RadGrid_Web20 .rgFilter {
    background-position: 0 -300px;
}
.RadGrid_Web20 .rgFilter:hover {
    background-position: 0 -350px;
}
.RadGrid_Web20 .rgFilterActive {
    background-position: 0 -400px;
}
.RadGrid_Web20 .rgFilterActive:hover {
    background-position: 0 -400px;
}
.RadGrid_Web20 .rgFilterBox {
    border: 1px solid #4e75b3;
    color: #000;
    font: 12px "segoe ui",arial,sans-serif;
}
.RadMenu_Web20 .rgHCMClear, .RadMenu_Web20 .rgHCMFilter {
    background: url("/WebResource.axd?d=fmvs-Vr4l6cr9VTKrhIIOsUP06E3aQwQMtztnLtX_kUl-lfDXdJr_e2ePvkAh6vGdC08n2gvVBX93sZygOao-p5Nmg8az2zkHJoIpVbQv_Ng3ny4gU_O4BoddcK6eAZoIB9onYiGJsvJ8H-Up_tAqOPpuA82jZxzP6dh-EzKJw8W483RcqB1vXWt_NuApzX40nasyw2&t=635418873532749162") repeat-x scroll center -23px #617fc5;
    border-color: #0f1d48 #07112d #010615;
    border-radius: 2px;
    color: #fff;
    font-family: "segoe ui",arial,sans-serif;
}
.RadMenu_Web20 .rgHCMClear:hover, .RadMenu_Web20 .rgHCMFilter:hover {
    background-color: #c1d5ef;
    background-position: center -67px;
    border-color: #3f5f94;
    color: #0f3789;
}
.GridContextMenu_Web20 .rmLeftImage {
    background-image: url("/WebResource.axd?d=wk8l0dpTtBYv1b0d-z5opEXRUYRDadNUhOqsQWwDmqonnv3-i5adsudcwQ7OdnejRPfOmFtlbBQ7vbeKCChHlp5v4HdSUt_sxGVbq9zPxAn-5Urx3yeSngao8RYac0pWjxKXlc_w8UobOEEUH0c-PTKGLpJXwgkz3WbyMes4etFRmyp60&t=635418873532749162");
}
.GridContextMenu_Web20 .rgHCMSortAsc .rmLeftImage {
    background-position: 0 0;
}
.GridContextMenu_Web20 .rgHCMSortDesc .rmLeftImage {
    background-position: 0 -40px;
}
.GridContextMenu_Web20 .rgHCMUnsort .rmLeftImage {
    background-position: 0 -80px;
}
.GridContextMenu_Web20 .rgHCMGroup .rmLeftImage {
    background-position: 0 -120px;
}
.GridContextMenu_Web20 .rgHCMUngroup .rmLeftImage {
    background-position: 0 -160px;
}
.GridContextMenu_Web20 .rgHCMCols .rmLeftImage {
    background-position: 0 -200px;
}
.GridContextMenu_Web20 .rgHCMFilter .rmLeftImage {
    background-position: 0 -240px;
}
.GridContextMenu_Web20 .rgHCMUnfilter .rmLeftImage {
    background-position: 0 -280px;
}
.RadGrid_Web20 .rgGroupPanel {
    -moz-border-bottom-colors: none;
    -moz-border-left-colors: none;
    -moz-border-right-colors: none;
    -moz-border-top-colors: none;
    background: none repeat scroll 0 0 #e3eeff;
    border-color: -moz-use-text-color -moz-use-text-color #4e75b3;
    border-image: none;
    border-style: none none solid;
    border-width: 0 0 1px;
}
.RadGrid_Web20 .rgGroupPanel td {
    border: 0 none;
    padding: 3px;
    vertical-align: middle;
}
.RadGrid_Web20 .rgGroupPanel td td {
    padding: 0;
}
.RadGrid_Web20 .rgGroupPanel .rgSortAsc {
    background-position: 4px -144px;
}
.RadGrid_Web20 .rgGroupPanel .rgSortDesc {
    background-position: 4px -94px;
}
.RadGrid_Web20 .rgUngroup {
    background-position: 0 -6500px;
}
.RadGrid_Web20 .rgGroupItem {
    background: none repeat scroll 0 0 #c6d8f2;
    border: 1px solid #4e75b3;
    color: #0f3789;
}
.RadGrid_Web20 .rgGroupHeader {
    background: none repeat scroll 0 0 #859bbc;
    color: #fff;
    font-size: 1.1em;
    line-height: 21px;
}
.RadGrid_Web20 .rgGroupHeader td {
    padding: 0 8px;
}
.RadGrid_Web20 td.rgGroupCol, .RadGrid_Web20 td.rgExpandCol {
    background: none repeat scroll 0 0 #859bbc;
    border-color: #859bbc;
}
.RadGrid_Web20 .rgExpand {
    background-position: 5px -496px;
}
.RadGrid_Web20 .rgCollapse {
    background-position: 3px -444px;
}
.RadGrid_Web20 .rgEditForm {
    border-bottom: 1px solid #829cbf;
}
.RadGrid_Web20 .rgUpdate {
    background-position: 0 -1800px;
}
.RadGrid_Web20 .rgCancel {
    background-position: 0 -1850px;
}
.RadGrid_Web20 .rgDetailTable {
    border-color: #4e75b3;
}
.RadGrid_Web20 .rgCommandRow {
    background: url("/WebResource.axd?d=M_gmP3-oJcQ6aWk6kcjmsez5Sn93QeW26lWDG7JcGvwAB4swmL8QidC3MhgYWrI072nto5bVDc833RtbE2pKHnh3kgguPr2wHC-uRfKeGGayCSUzDWCRskXpUgcDGGul9v76FvHFYU7t5_mMW6HWu7B7pakgqsvwrpL81j6Zin7L72Lz0&t=635418873532749162") repeat-x scroll 0 -2099px #3b548c;
    color: #fff;
}
.RadGrid_Web20 .rgCommandCell {
    border: 0 none;
    padding: 0;
}
.RadGrid_Web20 thead .rgCommandCell {
    border-bottom: 1px solid #6788be;
}
.RadGrid_Web20 tfoot .rgCommandCell {
    border-top: 1px solid #6788be;
}
.RadGrid_Web20 .rgCommandTable {
    border: 1px solid #243567;
}
.RadGrid_Web20 .rgCommandTable td {
    border: 0 none;
    padding: 2px 7px;
}
.RadGrid_Web20 .rgCommandTable td td {
    padding: 0;
}
.RadGrid_Web20 .rgCommandRow a {
    color: #fff;
    text-decoration: none;
}
.RadGrid_Web20 .rgAdd {
    background-position: 0 -1650px;
    margin-right: 3px;
}
.RadGrid_Web20 .rgRefresh {
    background-position: 0 -1600px;
    margin-right: 3px;
}
.RadGrid_Web20 .rgEdit {
    background-position: 0 -1700px;
}
.RadGrid_Web20 .rgDel {
    background-position: 0 -1750px;
}
.RadGrid_Web20 .rgExpXLS, .RadGrid_Web20 .rgExpDOC, .RadGrid_Web20 .rgExpPDF, .RadGrid_Web20 .rgExpCSV {
    background-image: url("/WebResource.axd?d=fAn2p7XVoFmv2MG0PTyQcOHocara3x3kLoL68aNfnG6JYwxe8fZCAWrTyb3y_x4ZdzRLPjJ5URkxEXdBMsfeS1y-xDU_E8gmN1Hx35ad_14xRjl8hSbkLW6A-so3gpOHfQDHTjxWeehWAGJYe7VkJSmjK-cltBRIfBPYqR3G3NSyo_h10&t=635418873532749162");
}
.RadGrid_Web20 .rgExpXLS {
    background-position: 0 0;
}
.RadGrid_Web20 .rgExpDOC {
    background-position: 0 -50px;
}
.RadGrid_Web20 .rgExpPDF {
    background-position: 0 -100px;
}
.RadGrid_Web20 .rgExpCSV {
    background-position: 0 -150px;
}
.rgCellSelectorArea_Web20, .GridRowSelector_Web20 {
    background: none repeat scroll 0 0 #1346ad;
}
.GridItemDropIndicator_Web20 {
    border-top: 1px dashed #1346ad;
}
.GridToolTip_Web20 {
    background: none repeat scroll 0 0 #e3eeff;
    border: 1px solid #4e75b3;
    color: #000;
    padding: 3px;
}
.RadGridRTL_Web20 .rgHeader:first-child, .RadGridRTL_Web20 th.rgResizeCol:first-child, .RadGridRTL_Web20 .rgFilterRow > td:first-child, .RadGridRTL_Web20 .rgRow > td:first-child, .RadGridRTL_Web20 .rgAltRow > td:first-child {
    border-left-width: 1px;
    padding-left: 7px;
}
.RadGridRTL_Web20 .rgPageFirst {
    background-position: 0 -1000px;
}
.RadGridRTL_Web20 .rgPageFirst:hover {
    background-position: 0 -1050px;
}
.RadGridRTL_Web20 .rgPagePrev {
    background-position: 0 -850px;
}
.RadGridRTL_Web20 .rgPagePrev:hover {
    background-position: 0 -900px;
}
.RadGridRTL_Web20 .rgPageNext {
    background-position: 0 -700px;
}
.RadGridRTL_Web20 .rgPageNext:hover {
    background-position: 0 -750px;
}
.RadGridRTL_Web20 .rgPageLast {
    background-position: 0 -550px;
}
.RadGridRTL_Web20 .rgPageLast:hover {
    background-position: 0 -600px;
}
.RadGridRTL_Web20 .rgExpand {
    background-position: -20px -496px;
}


    </style>
    <style>
        .MainTag {
            float: left;
            width: 980px;
            margin: 5px 5px 5px 5px;
            font-family: Calibri, sans-serif; color: black;
        }

        .HeaderTag {
            float: left;
            border-width: 1px;
            border-style: solid;
            border-color: #66CCFF;
            width: 100%;
        }

        div.RadPanelBar .rpRootGroup .rpText {
            text-align: left;
        }

        div.RadPanelBar .rpGroup .rpText {
            text-align: left;
        }

        .labelGrade {
            float:left;width:100%; margin:2px 2px 2px 2px;background-color: #99CCFF;font-family: Calibri,sans-serif;font-size: 14px;
        }

        .rpnlbarStudentChecklist {
            float:left; margin: 10px 10px 10px 0px; width: 980px; 
        }
        .labelIntroduction{
            float:left;margin-top:10px; background-color: #99CCFF;text-align:justify;font-family: Calibri,sans-serif;font-size: 12px;
        }
        .labelSchool{
            float:left; width:100px;font-family: Calibri,sans-serif;font-size: 12px;
        }
        .textSchool {
            float:left; width:145px; margin-left:5px;font-family: Calibri,sans-serif;font-size: 12px;
        }
        .labelStudent {
            float:left; width:100px;font-family: Calibri,sans-serif;font-size: 12px;
        }
        .RadPanelBar_Web20 .rpExpandable span.rpExpandHandle, .RadPanelBar_Web20 a.rpExpandable:hover .rpNavigation .rpExpandHandle 
        {
         background-position: 0 -5px;
         float: left !important;
         margin-left: 5px !important;
        }

        .RadPanelBar_Web20 .rpExpanded span.rpExpandHandle, .RadPanelBar_Web20 a.rpExpanded:hover .rpNavigation .rpExpandHandle {
    background-position: 0 -181px;
}

    </style>
    <script>
        $(document).ready(function () {

            var divMainTag = "<div class='MainTag'>";
            
            var tblIdGradeName = "<div class ='labelGrade'>" + window.opener.document.getElementById("MainContentPlaceHolder_lblGradeName").innerHTML + "</div>";

            var headerTag = "<div class='HeaderTag'>";
            var subHeaderTag = "<div style='float:left;width:250px;'>";
                var groupSchoolTag = "<div style='float:left;width:250px;'>";
                var tblIdSchoolName = "<div class='labelSchool'><b>" + window.opener.document.getElementById("MainContentPlaceHolder_lblSchoolName").innerHTML + "</b></div>";
                var tblIdSchoolNameDisplay = "<div class='textSchool'>" + window.opener.document.getElementById("MainContentPlaceHolder_lblSchoolNameDisplay").innerHTML + "</div>";
                var endgroupSchoolTag = "</div>"

                var groupStudentTag = "<div style='float:left;width:250px;'>";
                var tblIdStudentName = "<div class='labelStudent'><b>" + window.opener.document.getElementById("MainContentPlaceHolder_lblStudentName").innerHTML + "</b></div>";
                var tblIdStudentNameDisplay = "<div style='float:left; width:145px; margin-left:5px;font-family: Calibri,sans-serif;font-size: 12px;'>" + window.opener.document.getElementById("MainContentPlaceHolder_lblStudentNameDisplay").innerHTML + "</div>";
                var endgroupStudentTag = "</div>";
            var endSubHeaderTag = "</div>";

            var tblIdCounselorName = "<div style='float:left;vertical-align:top;width:270px;'> <div style='float:left; margin-left:20px;font-family: Calibri,sans-serif;font-size: 12px;'><b>" + window.opener.document.getElementById("MainContentPlaceHolder_lblCounselorName").innerHTML + "</b></div>";
            var tblIdCounselorNameDisplay = "<div style='float:left;margin-left:5px; font-family: Calibri,sans-serif;font-size: 12px;'>" + window.opener.document.getElementById("MainContentPlaceHolder_lblCounselorNameDisplay").innerHTML + "</div></div>";

            var tbllogo = "<div style='float:right; vertical-align:top;margin-right:5px;font-family: Calibri,sans-serif;'>" + "<img width='180' height='70' alt='Paulding County School District' src='../../Images/GAPauldinglogo.jpg'>" + "</div>";

            var endheaderTag = "</div>";


            var tblIdIntroduction = "<div class='labelIntroduction'>" + window.opener.document.getElementById("MainContentPlaceHolder_lblIntroduction").innerHTML + "</div>";

            var tblIdCmntcontent = "<div class='rpnlbarStudentChecklist RadPanelBar RadPanelBar_Web20'> " + window.opener.document.getElementById("ctl00_MainContentPlaceHolder_rpnlbarStudentChecklist").innerHTML + "</div>";

            var enddivMainTag = "</div> ";

            document.getElementById("divPrint").innerHTML = divMainTag
                                                                + tblIdGradeName
                                                                + headerTag
                                                                    + subHeaderTag
                                                                        + groupStudentTag
                                                                            + tblIdStudentName + tblIdStudentNameDisplay
                                                                        + endgroupStudentTag
                                                                        + groupSchoolTag
                                                                            + tblIdSchoolName + tblIdSchoolNameDisplay
                                                                        + endgroupSchoolTag
                                                                    + endSubHeaderTag
                                                                    + tblIdCounselorName + tblIdCounselorNameDisplay
                                                                    + tbllogo                        
                                                                + endheaderTag
                                                                + tblIdIntroduction
                                                                + tblIdCmntcontent
                                                          + enddivMainTag;
            self.print();

        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
     <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
        </Scripts>
    </telerik:RadScriptManager>


        <div id="divPrint">
        </div>

    </form>
</body>
</html>
