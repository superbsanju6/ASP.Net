<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TeacherEvalForm.aspx.cs"
    Inherits="Thinkgate.Dialogues.TeacherEvalForm" %>

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
    <title></title>
    <base target="_self" />
    <style type="text/css">
        .infoTable
        {
            width: 300px;
            font-weight: bold;
            margin: 5px;
            font-size: 15px;
        }
        
        .overallTable
        {
            width: 300px;
        }
        
        .overallTable td
        {
            border: 1px solid black;
            font-size: 18px;
            font-weight: bold;
            text-align: left;
            padding: 5px;
        }
                
        .skillsTable th
        {
            text-align: center;
            font-weight: bold;
            border: 1px solid #89AEE5;                                               
        }
        
        .skillsTable td
        {
            text-align: center;
            border: 1px solid #89AEE5;            
        }
        
        .compDiv
        {
            cursor: pointer;
            background-color: #8FB0DB;
            color: white;
            font: normal 14px/24px "Segoe UI" ,Arial,sans-serif;
            padding: 3px;
            border: 1px solid #89AEE5;
            width: 942px;
        }
        
        .rubricSkillTable td
        {
            font-size: 11px;
            vertical-align: top;
            border: 1px solid #89AEE5;
            padding: 3px 3px 3px 3px;
        }
        
        .rubricSkillTable th
        {
            padding: 3px 3px 3px 3px;
            text-align: center;
            vertical-align: top;
            font-weight: bold;
            border: 1px solid #89AEE5;
        }
        
        .brownText 
        {
            color: #999900;
        }
        
        .scoreDescTable
        {
            font-size: 14px;
        }
        
        .scoreDescTable th
        {
            padding: 5px;
        }
    </style>
    <script type="text/javascript">
        function toggleAllSections() {
            //var sections = document.getElementsByClassName('compDiv');
            var sections = document.getElementsByTagName("DIV");
            for (var i = 0; i < sections.length; i++)
                if(sections[i].className == "compDiv")
                    showHideContents(sections[i]);
        }

        function showHideContents(containerDiv) {
            var isExpanded = containerDiv.getAttribute("expanded");
            var img = containerDiv.children[0];
            var contentsDiv = document.getElementById(containerDiv.id.replace('Panel', 'PanelContents'));
            if (isExpanded == "true") {
                containerDiv.setAttribute("expanded", "false");
                img.src = "../Images/down_arrow.png";
                showHideRows(contentsDiv, "none");
            }
            else {
                containerDiv.setAttribute("expanded", "true");
                img.src = "../Images/up_arrow.png";
                showHideRows(contentsDiv, "");
            }
        }

        function showHideRows(div, display) {
            var rows = div.getElementsByTagName('TR');
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].className == "skillsRow") {
                    rows[i].style.display = display;
                }
            }
        }

        function showRubricDialog(aTag) {
            var hidden = aTag.parentNode.children[1];
            var rubricText = hidden.value;
            var contentHTML = document.getElementById("rubricTemplate").innerHTML;

            var title = aTag.innerText;
            if (title.indexOf(' ') > -1) title = title.substring(0, title.indexOf(' '));

            contentHTML = contentHTML.replace("@@SKILLTEXT@@", aTag.innerHTML).replace("@@INDICATOR1@@", document.title.indexOf('School-Based Administrator') > -1 ? 'Needs Improvement' : 'Needs Improvement/<br />Developing');

            var scoresText = hidden.value.split("|");

            for (var i = 0; i <= 3; i++) {
                contentHTML = contentHTML.replace("@@SCORE" + i + "TEXT@@", (scoresText[i]) ? scoresText[i] : "");
            }

            customDialog(
                        { name: 'skillRubricDialog',
                            title: (document.title.indexOf('School-Based Administrator') > -1 ? 'School-Based Administrator Rubric ' : 'Teacher Evaluation Rubric ') + title,
                            content: contentHTML,
                            maxwidth: 700,
                            maximize_width: true
                        }
                    );
        }

        function openResources(aTag, type) {
            var rubricItemID = aTag.getAttribute("rubricItemID");
            
            customDialog(
                        { name: 'resourcesDialog',
                            title: type,
                            url: 'RubricItemResources.aspx?xID=' + rubricItemID + "&category=" + type,
                            maximize: true, maxwidth: 550, maxheight: 400
                        }
                    );
        }

        function showPointDescriptions() {
            var contentHTML = "<table class='scoreDescTable' width='100%'><tr><th style='background-color: #dadada'>0 - Unsatisfactory</th></tr>"
                            + "<tr><th>1 - Needs Improvement/Developing</th></tr>"
                            + "<tr><th style='background-color: #dadada'>2 - Effective</th></tr>"
                            + "<tr><th>3 - Highly Effective</th></tr></table>";

            customDialog(
                        { name: 'pointsDescDialog',
                            title: "Points Description",
                            content: contentHTML,
                            maximize: true, maxwidth: 250, maxheight: 150
                        }
                    );
        }
    </script>
</head>
<body style="font-family: Arial, Sans-Serif; font-size: 10pt; width: 870px; height: 600px;">
    <form runat="server" id="mainForm" method="post" style="width: 870px; height: 600px;">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
            <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
            <asp:ScriptReference Path="~/scripts/master.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
        Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
        <table width="100%">
            <tr>
                <td style="vertical-align: top">
                    <table cellpadding="5" class="infoTable">
                        <tr>
                            <td>
                                Name:
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblTeacherName" />
                            </td>
                        </tr>
                        <tr runat="server" id="trPosition">
                            <td>
                                Position:
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblPosition" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                School:
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblSchool" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                </td>
                <td style="text-align: right; vertical-align: top">
                    <img src="../Images/SarasotaLogo.jpg" alt="Sarasota" width="300" />
                </td>
            </tr>
            <tr>
                <td colspan="3" style="text-align: center">
                    <center>
                        <table cellpadding="5" class="overallTable" width="100%" runat="server" id="tableOverallScore">
                            <tr>
                                <td>
                                    PRIDE Total Score
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblTotalPoints" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    PRIDE Concordant
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblConcordantScore" />
                                </td>
                            </tr>
                        </table>
                    </center>
                    <img src="../Images/Toolbars/print.png" alt="Print" onclick="window.print()" style="cursor: pointer;
                        float: right;" />
                </td>
            </tr>
        </table>
        <br />
        <table width="950px" class="skillsTable">
            <thead>
                <tr class="skillsRow">
                    <th width="400px">
                    </th>
                    <th width="25px">
                        <a href='javascript:'  onclick="showPointDescriptions()">0</a>
                    </th>
                    <th width="25px">
                        <a href='javascript:'  onclick="showPointDescriptions()">1</a>
                    </th>
                    <th width="25px">
                        <a href='javascript:'  onclick="showPointDescriptions()">2</a>
                    </th>
                    <th width="25px">
                        <a href='javascript:'  onclick="showPointDescriptions()">3</a>
                    </th>
                    <th width="60px">
                        Weight
                    </th>
                    <th width="90px">
                        Your<br />
                        Total Points
                    </th>
                    <th width="100px" runat="server" id="thDistrictAvg">
                        District<br />
                        Average
                    </th>
                    <th width="200px" runat="server" id="thProfDev">
                        Professional Dev. Per<br />
                        Competency/Domain
                    </th>
                </tr>
            </thead>
        </table>
        <asp:Repeater runat="server" ID="compentencyRepeater" OnItemDataBound="compentencyRepeater_ItemDataBound">
            <ItemTemplate>
                <div id="compPanel" class="compDiv" runat="server" onclick="showHideContents(this)"
                    expanded="true">
                    <img src="../Images/up_arrow.png" alt="Expand Show Icon" />
                    <asp:Label runat="server" ID="lblCompText" />
                </div>
                <div runat="server" id="compPanelContents">
                    <asp:Repeater runat="server" ID="skillsRepeater" OnItemDataBound="skillsRepeater_ItemDataBound">
                        <HeaderTemplate>
                            <table width="950px" class="skillsTable">
                                <thead>
                                    <tr class="skillsRow">
                                        <th width="400px">
                                        </th>
                                        <th width="25px">
                                        </th>
                                        <th width="25px">
                                        </th>
                                        <th width="25px">
                                        </th>
                                        <th width="25px">
                                        </th>
                                        <th width="60px">
                                        </th>
                                        <th width="90px">
                                        </th>
                                        <th width="100px" runat="server" id="thDistrictAvg">
                                        </th>
                                        <th width="200px" runat="server" id="thProfDev">
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr class="skillsRow">
                                <td style="text-align: left; padding-left: 2px;" width="398px"> 
                                    <a href="javascript:" onclick="showRubricDialog(this);">
                                        <asp:Label runat="server" ID="lblSkillText" />
                                    </a>
                                    <asp:HiddenField runat="server" ID="hiddenRubricText" />
                                </td>
                                <td width="25px">
                                    <asp:Label runat="server" ID="lblScore0" Visible="false" Text="0" />
                                </td>
                                <td width="25px">
                                    <asp:Label runat="server" ID="lblScore1" Visible="false" Text="1" />
                                </td>
                                <td width="25px">
                                    <asp:Label runat="server" ID="lblScore2" Visible="false" Text="2" />
                                </td>
                                <td width="25px">
                                    <asp:Label runat="server" ID="lblScore3" Visible="false" Text="3" />
                                </td>
                                <td width="60px">
                                    <asp:Label runat="server" ID="lblWeight" />
                                </td>
                                <td width="90px">
                                    <asp:Label runat="server" ID="lblTotalPoints" />
                                </td>
                                <td runat="server" id="tdDistrictAvg" width="100px">
                                    <asp:Label runat="server" ID="lblDistrictAverage" />
                                </td>
                                <td style="text-align: left; padding-left: 2px;" runat="server" id="tdProfDev"  width="198px">
                                    <a href="javascript:" onclick="openResources(this.parentNode, 'PD Activities for Credit')">PD Activities for Credit</a><br />
                                    <a href="javascript:" onclick="openResources(this.parentNode, 'Free Online Videos')">Free Online Videos</a><br />
                                    <a href="javascript:" onclick="openResources(this.parentNode, 'Other Resources')">Other Resources</a>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                        </FooterTemplate>
                    </asp:Repeater>
                    <tr>
                        <td colspan="6" style="font-weight: bold" width="560px">
                            TOTAL FOR DOMAIN
                        </td>
                        <td style="font-weight: bold" width="90px">
                            <asp:Label runat="server" ID="lblCompTotalPoints" />
                        </td>
                        <td width="100px" runat="server" id="tdDistrictAvg">
                            <asp:Label runat="server" ID="lblCompDistrictAvg" />
                        </td>
                        <td width="200px" runat="server" id="tdProfDev">
                        </td>
                    </tr>
                    </table>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <table width="950px" class="skillsTable">
            <tr class="skillsRow">
                <td colspan="6" style="font-weight: bold; text-align: right;" width="560px">
                    TOTAL
                </td>
                <td style="font-weight: bold" width="90px">
                    <asp:Label runat="server" ID="lblOverallTotalPoints" />
                </td>
                <td width="100px" runat="server" id="tdDistrictAvg">
                    <asp:Label runat="server" ID="lblOverallDistrictAverage" />
                </td>
                <td width="200px" runat="server" id="tdProfDev">
                </td>
            </tr>
        </table>
        <div id="rubricTemplate" style="display: none">
            <table width="100%" class="rubricSkillTable">
                <tr>
                    <td>
                    </td>
                    <td colspan="4" style="text-align: center; font-style: italic; font-weight: bold;">
                        INDICATORS
                    </td>
                </tr>
                <tr>
                    <th style="text-align: center; vertical-align: middle;">
                        <span class="brownText">COMPETENCY</span>
                    </th>
                    <th>
                        0<br />
                        <span class="brownText">Unsatisfactory</span>
                    </th>
                    <th>
                        1<br />
                        <span class="brownText">@@INDICATOR1@@</span>
                    </th>
                    <th>
                        2<br />
                        <span class="brownText">Effective</span>
                    </th>
                    <th>
                        3<br />
                        <span class="brownText">Highly Effective</span>
                    </th>
                </tr>
                <tr>
                    <td>
                       <span class="brownText">@@SKILLTEXT@@</span>
                    </td>
                    <td>
                        @@SCORE0TEXT@@
                    </td>
                    <td>
                        @@SCORE1TEXT@@
                    </td>
                    <td>
                        @@SCORE2TEXT@@
                    </td>
                    <td>
                        @@SCORE3TEXT@@
                    </td>
                </tr>
            </table>
        </div>
        <script type="text/javascript">
            toggleAllSections();
        </script>
    </telerik:RadAjaxPanel>
    </form>
</body>
</html>
