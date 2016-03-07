<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ItemReviewMainScreen.aspx.cs" Inherits="Thinkgate.Controls.Rating.ItemReviewMainScreen" %>

<%@ Register TagPrefix="e3" TagName="SpecialPopulation" Src="~/Controls/Rating/SpecialPopulationUC.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <title></title>
    <link id="linkWindowStyle" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet" type="text/css" runat="server" />
    <link href="~/Styles/Ratings/AddEditQuestionReview.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/Ratings/SpecialPopulations.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
    <script type="text/javascript">

        var radRating;

        //SubmitReview validates the minimum criteria for successful save.
        //User should have entered comments or selected rating
        function submitReview() {
            var selectedItems = $('.rrtSelected');
            var reviews = $('#txtReivew').val();

            if (selectedItems.length == 0 && reviews.trim().length == 0) {
                $('#lblErrorMsg').css('visibility', 'visible');
            }
            else {
                $('#lblErrorMsg').css('visibility', 'hidden');
                __doPostBack('radSubmit', '');
            }
        }

        //Business Rule for No Rating Check box
        //When user selected No Rating checkbox the rating control will get disable with zero star selected.
        //When unchecked one star will get selected and rating control available for user selection.
        function noRatingCheckBox(sender) {
            var items = radRating._items;
            if (sender.checked) {

                for (var i = 0; i < items.length; i++) {
                    items[i].className = "";
                }
                radRating._value = 0;
                document.getElementById("rating_ClientState").value = "{\"value\":\"0\",\"readOnly\":false}";
                radRating._enableRatingControl();
            }
            else {
                if (items != null && items.length > 0) {
                    items[0].className = "rrtSelected";
                }
                radRating._value = 1;
                document.getElementById("rating_ClientState").value = "{\"value\":\"1\",\"readOnly\":false}";
                radRating._enableRatingControl(radRating);
            }
        }

        //Fires on rating control load. 
        //Get rating control instance and enable/disable the control based on No Rating Check box value.
        function setRatingControl(control) {
            radRating = control;

            var checkBox = document.getElementById('chkNoRating');

            if (checkBox.checked)
                radRating._enableRatingControl();
            else
                radRating._enableRatingControl(radRating);

            $('#lblErrorMsg').css('visibility', 'hidden');
        }

        //Close current dialog Refresh the information on parent dialog.
        function closeAndRefresh() {
            redirectToParent();
        }

        //Handles the refresh from both "Review Main Screen" And "Review Summary Screen"
        //Set refresh value so that parent dialog will understand to perform a refresh operation.
        //Close the current dialog
        function redirectToParent() {
            var source = document.getElementById("hidSource").value;
            switch (source) {
                case "ReviewMainScreen":
                    var hidReviewMainScreenRefresh = $("#hidRefresh", parent.document.body);
                    hidReviewMainScreenRefresh.val("true");
                    break;

                case "ReviewSummary":
                    var hidReviewSummaryRefresh = $("#hidRefresh", parent.parent.document.body);
                    hidReviewSummaryRefresh.val("true");
                    var actionId = document.getElementById("hidActionId").value;
                    var action = "#" + actionId + "";
                    var btnAction = $(action, parent.document.body);
                    if (btnAction != undefined && btnAction.length > 0) {
                        btnAction[0].click();
                    }
                    break;

                default:
                    break;
            }

            closeCurrentCustomDialog();
        }

        //Check for max comments entered by user
        //FireFox :- To avoid BackSpace and Delete button
        function isUnderCharLimit(val, event) {
            var len = val.value.length;
            if (len < 500) {
                if (checkForSelectionText(val) || event.keyCode == 8 || event.keyCode == 46) {
                    return true;
                }
            } else {
                return false;
            }
        }

        //Check for selection Text
        function checkForSelectionText(val) {
            if ((val.selectionEnd - val.selectionStart) > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        //Check for max comments entered by user
        //Set the remaining count
        function countChar(val) {
            var len = val.value.length;
            var charsRemaining = 500 - len;
            $('#remainingCharacters').html('<span class="black">Remaining characters: </span>' + charsRemaining);
        }

        //Handles the copy paste functionality of comment
        function pasteComment(val, e) {

            var paste = e.clipboardData && e.clipboardData.getData ? e.clipboardData.getData('text/plain') :
                        window.clipboardData && window.clipboardData.getData ? window.clipboardData.getData('Text') : false;
            var newText = '';
            var actualText = '';

            if ((val.value.length + paste.length) > 500) {
                newText = val.value.replaceAt(val.selectionStart, val.selectionEnd, paste);
                actualText = newText.substring(0, 500);
                document.getElementById('txtReivew').value = actualText;
                countChar(val);
                return false;
            }

            countChar(val);
        }

        //Replace the string based on start and end Index
        String.prototype.replaceAt = function (startIndex, endIndex, newString) {
            return this.substr(0, startIndex) + newString + this.substr(endIndex, this.length);
        }

        //Set the inital comment count when dialog loads.
        function init() {

            countChar(document.getElementById('txtReivew'));
        }
        window.onload = init;

    </script>

    <style type="text/css">
        hr {
            color: white;
            border: 1px solid white;
        }

        .font-smaller {
            font-size: smaller;
        }

        .margin-rigth-10 {
            margin-right: 15px;
        }

        .center {
            margin: auto;
            width: 70%;
        }

        .black {
            color: black;
        }

        #remainingCharacters {
            font-size: smaller;
            float: right;
            color: red;
        }

        .dropdown_role {
            width: 58%;
        }

        .errorMessage {
            color: red;
            font-size: small;
            font-weight: bold;
            float: left;
        }

        .disclaimer {
            font-weight: bold;
            font-size: small;
        }

        .centeredText {
            text-align: center;
        }

        .rating {
            margin-left: 0px;
            float: right;
        }

        .rating-text {
            margin-top: 4px;
            margin-left: 4px;
            float: right;
        }
    </style>
</head>
<body>
    <form id="frmMain" runat="server">
        <telerik:RadScriptManager ID="radScriptManager" runat="server" EnablePageMethods="True">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
                <asp:ScriptReference Path="~/scripts/master.js" />
            </Scripts>
        </telerik:RadScriptManager>

        <telerik:RadWindowManager ID="radWindowManager" runat="server" EnableShadow="false" Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />

        <telerik:RadAjaxPanel ID="ajaxReviewPanel" runat="server" LoadingPanelID="reviewProgress" Width="100%">
            <asp:HiddenField runat="server" ID="hidSource" />
            <asp:HiddenField runat="server" ID="hidActionId" />
            <table style="width: 100%;">

                <tr id="topTableRow">
                    <td colspan="2">
                        <asp:TextBox ID="txtReivew" runat="server" TextMode="MultiLine" Width="100%" Height="150px" onkeypress="return isUnderCharLimit(this, event)" onkeyup="countChar(this)" onpaste=" return pasteComment(this, event)"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span id="remainingCharacters">
                            <span class="black">Remaining characters: </span>500
                        </span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblRole" CssClass="float-left" runat="server">Role of Submitter</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ClientIDMode="Static" Width="237px" CssClass="dropdown_role float-left" ID="ddlRoles" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <e3:SpecialPopulation ClientIDMode="Static" ID="specialPopulationUC" runat="server" />
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="margin: auto; display: inline-block;">
                            <div class="rating-text">Click to Rate</div>
                            <telerik:RadRating ID="rating" runat="server" Skin="Default" ItemCount="5"
                                SelectionMode="Continuous" Precision="Item" Orientation="Horizontal"
                                OnClientLoad="setRatingControl" CssClass="rating">
                            </telerik:RadRating>

                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="centeredText" colspan="2">
                        <asp:CheckBox ID="chkNoRating" CssClass="checkbox" runat="server"
                            Text="No Rating Given" AutoPostBack="false" onclick="noRatingCheckBox(this)"
                            Font-Size="12pt" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="center">
                            <telerik:RadButton ClientIDMode="Static" ID="radCancel" Width="15%" runat="server" Text="Cancel" AutoPostBack="false" OnClientClicked="closeCurrentCustomDialog"></telerik:RadButton>
                            <telerik:RadButton ClientIDMode="Static" ID="radSubmit" Width="15%" runat="server" Text="Submit" AutoPostBack="false" OnClientClicked="submitReview" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span class="centeredText">
                            <asp:Label ID="lblErrorMsg" CssClass="errorMessage" runat="server" Width="100%">
                            Enter comment or select a rating
                            </asp:Label>
                        </span>
                    </td>
                </tr>
                <%--               <tr>
                   <td>
                       &nbsp;
                   </td>
               </tr>--%>
                <tr>
                    <td colspan="2" class="disclaimer">Any views or opinions expressed in this review are solely those of the author and do not necessarily represent those of the School, District, or State. 
                    </td>
                </tr>
            </table>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="reviewProgress" runat="server" Width="100%" />
    </form>
</body>
</html>
