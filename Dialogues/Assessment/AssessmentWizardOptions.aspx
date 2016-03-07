<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Dialogues/Assessment/AssessmentDialog.Master" CodeBehind="AssessmentWizardOptions.aspx.cs"
    Inherits="Thinkgate.Dialogues.Assessment.AssessmentWizardOptions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        body {
            font-family: Sans-Serif, Arial;
        }

        p {
            font-family: Verdana;
            font-style: italic;
            font-size: 11px;
            font-weight: bold;
            margin-top: 10px !important;
            margin-bottom: 75px !important;
            color: #333;
        }

        .headerImg {
            position: absolute;
            left: 0;
            top: 50px;
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
            margin-left: 5px;
            margin-top: 40px;
            cursor: pointer;
            background-color: #FFF;
            width: 80px;
        }

        .rectangleButtons {
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 25px;
            display: inline;
            position: relative;
            border: solid 1px #000;
            border-radius: 5px;
            margin-left: 5px;
            margin-top: 15px;
            cursor: pointer;
            background-color: #FFF;
            width: 350px;
            height: 65px;
            text-align: center;
            margin-bottom: 15px;
            font-style: italic;
        }

        .useTheWizard{
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 25px;
            position: relative;
            border: solid 1px #000;
            border-radius: 5px;
            margin-left: 5px;
            margin-top: 15px;
            cursor: pointer;
            background-color: #FFF;
            width: 300px;
            height: 40px;
            text-align: center;
            margin-bottom: 15px;
            font-style: italic;
        }

        .roundButtons_blue {
            color: #FFF;
            background-color: #36C;
            font-size: 12pt;
            padding: 2px 0;
            position: relative;
            border: solid 1px #000;
            border-radius: 50px;
            cursor: pointer;
            font-weight:normal;
            font-style:normal;
            width: 180px;
            margin: 0 auto;
            margin-top:7px;
        }

        .roundButtons_blue_disabled {
            color: #BDBDBD;
            background-color: #7094DB;
            font-size: 12pt;
            padding: 2px 0;
            position: relative;
            border: 1px solid #4D4D4D;
            border-radius: 50px;
            cursor: default;
            font-weight:normal;
            font-style:normal;
            width: 98px;
            margin: 0 auto;
            margin-top:7px;
            text-shadow: 1px 1px #FFF;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <img runat="server" id="headerImg" src="../../Images/magicwand.png" alt="Create assessment icon" class="headerImg" clientidmode="Static" />

    <p>Choose one of the options below to select items by item type and rigor or by available addendums.</p>

    <div class="containerDiv" align="center">

        <asp:Button ID="btnRigorType" runat="server" Text="Select items by Item Type and Rigor" CssClass="rectangleButtons" OnClientClick="rigorTypeButtonClick(); return false;" />

        <asp:Button ID="btnAddendumType" runat="server" CountAddendums="0" Text="Select items by Available Addendums" CssClass="rectangleButtons" OnClientClick="addendumTypeButtonClick(this); return false;" />

        <div class="useTheWizard" id="useTheWizard" onclick="generateAssessmentClick(); return false;">Use Wizard to Select Items
            <div class="roundButtons_blue" id="generateAssessment">Generate Assessment
            </div>
        </div>

    </div>
    <div align="right" style="margin-right: 20px;">
        <asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons"
            Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="setTimeout(function() { modalWin.close(); }, 0); return false;" />
        <asp:Button runat="server" ID="backButton" ClientIDMode="Static" CssClass="roundButtons"
            Text="&nbsp;&nbsp;Back&nbsp;&nbsp;" OnClientClick="backButtonClick(); return false;" />
    </div>
    <input runat="server" type="hidden" id="newAssessmentTitle" clientidmode="Static" name="newAssessmentTitle" value="" />
    <input runat="server" type="hidden" id="blankItemCounts" clientidmode="Static" name="blankItemCounts" value="" />
    <span style="display: none;">
        <telerik:RadXmlHttpPanel runat="server" ID="submitXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/AssessmentWCF.svc"
            WcfServiceMethod="RequestNewAssessmentID" RenderMode="Block" OnClientResponseEnding="goToNewAssessment"
            objectToLoadID="assessmentID">
        </telerik:RadXmlHttpPanel>
    </span>
    <script type="text/javascript">
        var dropdownObjectBlurred = false;
        var modalWin = getCurrentCustomDialog();
        var prepWindow = true;
        function backButtonClick() {

            var allMax = true;
            if (navigator.appVersion.indexOf("Win") == -1) {
                allMax = false;
            }

            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: '../Dialogues/Assessment/AssessmentStandards.aspx?headerImg=' + $('#headerImg').attr('headerImgName'),
                title: 'Assessment Standards Selection',
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

        function addendumTypeButtonClick(addendumTypeButton) {
            var addendumCount = $.trim($(addendumTypeButton).attr("CountAddendums"));
            if (parseInt(addendumCount) <= 0)
            {
                setTimeout(function () {
                    parent.customDialog({ title: 'Alert', content: "There are no addendums available for the selected standards.", dialog_style: 'alert', closeMode: false }, [{ title: 'OK' }]);
                });
                return false;
            }
            var headerImg = $('#headerImg').attr('headerImgName');
            var url = '../Dialogues/Assessment/AssessmentAddendumsDetail.aspx?headerImg=' + headerImg;

            var allMax = true;
            if (navigator.appVersion.indexOf("Win") == -1) {
                allMax = false;
            }

            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: url,
                title: 'Addendum Selection',
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

        function rigorTypeButtonClick() {
            var headerImg = $('#headerImg').attr('headerImgName');
            var url = '../Dialogues/Assessment/AssessmentStandardsDetail.aspx?headerImg=' + headerImg;

            var allMax = true;
            if (navigator.appVersion.indexOf("Win") == -1) {
                allMax = false;
            }


            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: url,
                title: 'Assessment Item Rigor Selections',
                maximize: true,
                maxwidth: 1000,
                maxheight: 550,
                resizable: false,
                movable: true,
                center: true,
                maximizable: allMax,
            });
            win.remove_beforeClose(win.confirmBeforeClose);
            win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
        }

        function ResetGenerateButton() {
            $('#generateAssessment').attr("class", "roundButtons_blue");
            document.getElementById('generateAssessment').innerHTML = "Generate Assessment";
            $('#useTheWizard').css('cursor', 'pointer');
            $('#useTheWizard').attr("onclick", "generateAssessmentClick(); return false;");
        }

        function generateAssessmentClick() {
            $('#generateAssessment').attr("class", "roundButtons_blue_disabled");
            document.getElementById('generateAssessment').innerHTML = "In Process...";
            $('#useTheWizard').css('cursor', 'default');
            $('#useTheWizard').attr("onclick", "");
            var blankItemsCount = document.getElementById('blankItemCounts').value;
            if (parseInt(blankItemsCount) > 0) {

                function callbackFunction(arg) {
                    if (arg) {
                        createAssessment();
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
                createAssessment();
            }
        }

        function createAssessment() {
            var panel = $find('submitXmlHttpPanel');
            panel.set_value('{"StandardsList":""}');
        }

        function goToNewAssessment(sender, args) {
            var result = args.get_content();
            var modalTitle = document.getElementById('newAssessmentTitle').value;

            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: '../Record/AssessmentPage.aspx?xID=' + result,
                title: modalTitle,
                maximize: true
            });
            win.remove_beforeClose(win.confirmBeforeClose);
        }
    </script>
</asp:Content>
