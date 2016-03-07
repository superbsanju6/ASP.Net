<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TeacherEvalReport.aspx.cs"
    Inherits="Thinkgate.Dialogues.TeacherEvalReport" %>

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
        .evalHeaderTable th
        {
            font-weight: bold;
            text-decoration: underline;
            padding: 5px;
            text-align: center;
        }
        
        .evalHeaderTable td
        {
            padding: 5px;
            text-align: center;
        }
        
        .concordantRangesTable td
        {
            text-align: center;
            padding: 3px;
            border: 1px solid black;
            font-size: 12px;
        }
        
        .concordantRangesTable th
        {
            text-align: center;
            border: 1px solid black;
            font-weight: bold;
            padding: 3px;
        }
        
        .signLabel
        {
            font-size: 12px;
            font-weight: bold;
        }
        
        .signText
        {
            font-size: 12px;
            font-style: italic;
            font-weight: bold;
            color: #3333CC;
        }
        
        .signCheckbox > input
        {
            display:inline;
        }
        
        .finalRatingDiv
        {
            text-align: center;
            font-weight: bold;
            padding: 35px 5px 35px 5px !important;
            background: url('../Images/new_tg_bg.png');
            background-repeat: repeat;
            font-size: 24px;
            padding: 10px;
            color: #3333CC;            
        }
        
        .teacherTD
        {
            text-align: center;
            vertical-align: top;
            font-weight: bold;
            font-size: 18px;
        }
        
        .progressBarDiv
        {
            width: 125px;
            border: 1px solid black;
            text-align: center;
        }
        
        .progressBarDiv div
        {
            text-align: center;
            color: White;
            font-weight: bold;
            border-right: 1px solid black;
        }
        
        .resourcesTable
        {
            font-size: 14px;
        }
        
        .resourcesTable th
        {
            padding: 5px;
        }
    </style>
    <script type="text/javascript">

        function openEvalForm() {
            var url = window.location.href.replace("TeacherEvalReport", "TeacherEvalForm");
            customDialog(
                        { name: 'teacherEvalForm',
                            url: url,
                            maximize: true
                        }
                    );
        }

        function launchStudentGrowthData() {
            customDialog(
                        { name: 'studentGrowthData',
                            title: 'Student Growth Data',
                            url: 'StudentGrowthData.aspx?xID=' + getURLParm("xID"),
                            maximize: true, maxwidth: 750, maxheight: 520
                        }
                    );
        }

        function openResources() {
            var type = document.getElementById("hiddenEvalType").value;
            var contentHTML = "<table class='resourcesTable' width='100%'>"
                            + "<tr><th style='background-color: #dadada'><a href='javascript:' onclick='openFAQ();'>Evaluation FAQ</a></th></tr>";

            if (type == "Administrator")
                contentHTML += "<tr><th><a href='javascript:' onclick='openConcordantTable(\"Admin\")'>Administrator Concordant Table</a></th></tr>";


            contentHTML += "<tr><th" + (type == "Administrator" ? " style='background-color:#dadada;'" : "") + "><a href='javascript:' onclick='openConcordantTable(\"PRIDE\")'>PRIDE Concordant Table</a></th></tr>";

            if (type == "Administrator" || type == "TeacherClassroom")
                contentHTML += "<tr><th" + (type == "TeacherClassroom" ? " style='background-color: #dadada'" : "") + "><a href='javascript:' onclick='openPRIDERubric(\"../faq/Pride Rubric Classroom Instructional.pdf\");'>Classroom PRIDE Rubric</a></th></tr>";

            if (type == "Administrator" || type == "TeacherNonClassroom")
                contentHTML += "<tr><th style='background-color: #dadada'><a href='javascript:' onclick='openPRIDERubric(\"../faq/Pride Rubric Non-Classroom Instructional.pdf\");'>Non-Classroom PRIDE Rubric</a></th></tr>";

            if (type == "Administrator")
                contentHTML += "<tr><th><a href='javascript:' onclick='openPRIDERubric(\"../faq/School_Adm_Rubric.pdf\");'>School-Based Administrator Rubric</a></th></tr>";

            contentHTML += "<tr><th" + (type == "Administrator" ? " style='background-color:#dadada;'" : "") + "><a href='javascript:' onclick='openStudentGrowthConList();'>Student Growth Concordant Tables</a></th></tr>";

            if (type != "Administrator")
                contentHTML += "<tr><th style='background-color: #dadada'><a href='javascript:' onclick='openPRIDERubric(\"../faq/Teacher Evaluation Guide v3.pdf\");'>Teacher Evaluation Guide</a></th></tr>";


            contentHTML += '</table>';

            customDialog(
                        { name: 'resourcesDialog2',
                            title: 'Resources',
                            content: contentHTML,
                            maximize: true, maxwidth: 300, maxheight: 250
                        }
                    );
        }

        function openFAQ() {
            window.open("../faq/FAQTeacherEvaluation.pdf");
        }

        function openStudentGrowthConList() {
            var contentHTML = document.getElementById("concordantGroups").innerHTML;

            customDialog(
                        { name: 'resourcesDialog3',
                            title: 'Student Growth Concordant Tables',
                            content: contentHTML,
                            maximize: true, maxwidth: 300, maxheight: 250
                        }
                    );
        }

        function openSGCT(aTag) {
            //var group = aTag.children[0].innerText.replace("Group ","");
            var divs = aTag.parentNode.getElementsByTagName("DIV");
            if (divs.length == 0) return;

            if (divs[0].style.display == "none")
                divs[0].style.display = "";
            else
                divs[0].style.display = "none";
        }

        function openConcordantTable(type) {
            if (type == 'Admin' || type == 'PRIDE') {
                var title = (type == 'Admin') ? 'Administrator Concordant Table' : 'PRIDE Concordant Table';
                var contentHTML = (type == 'Admin') ? document.getElementById("hiddenAdminConcordantTable").innerHTML : document.getElementById("hiddenPRIDEConcordantTable").innerHTML;
                customDialog(
                        { name: 'concordantTableStatic',
                            title: title,
                            content: contentHTML,
                            maximize: true, maxwidth: 300, maxheight: 500
                        }
                    );
            }
            else {
                parent.customDialog(
                        { name: 'concordantTable',
                            url: '../Dialogues/RubricConcordantTables.aspx?xID=' + type,                            
                            maximize: true, maxwidth: 750, maxheight: 520
                        }
                    );

            }
        }

        function openPRIDERubric(pdf) {
            window.open(pdf);
        }

    </script>
</head>
<body style="font-family: Arial, Sans-Serif; font-size: 10pt; width: 950px; height: 600px;">
    <form runat="server" id="mainForm" method="post" style="width: 950px; height: 600px;">
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
        <asp:HiddenField runat="server" ID="hiddenEvalType" ClientIDMode="Static" />
        <div id="hiddenPRIDEConcordantTable" style="display: none;">
            <div style="height: 500px; overflow: auto;">
                <asp:Repeater runat="server" ID="prideConcordantRangesTable">
                    <HeaderTemplate>
                        <table width="100%" class="concordantRangesTable">
                            <tr>
                                <th>
                                    Min
                                </th>
                                <th>
                                    Max
                                </th>
                                <th>
                                    Concordant
                                </th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr style='background-color: <%# Eval("Color") %>'>
                            <td>
                                <%# Eval("minValue") %>
                            </td>
                            <td>
                                <%# Eval("maxValue") %>
                            </td>
                            <td>
                                <%# Eval("concordantValue")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table></FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
        <div id="hiddenAdminConcordantTable" style="display: none;">
            <div style="height: 500px; overflow: auto;">
                <asp:Repeater runat="server" ID="adminConcordantRangesTable">
                    <HeaderTemplate>
                        <table width="100%" class="concordantRangesTable">
                            <tr>
                                <th>
                                    Min
                                </th>
                                <th>
                                    Max
                                </th>
                                <th>
                                    Concordant
                                </th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr style='background-color: <%# Eval("Color") %>'>
                            <td>
                                <%# Eval("minValue")%>
                            </td>
                            <td>
                                <%# Eval("maxValue")%>
                            </td>
                            <td>
                                <%# Eval("concordantValue")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table></FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
        <table width="100%">
            <tr>
                <td style="vertical-align: top;" width="35px">
                    <img src="../Images/Toolbars/print.png" alt="Print" onclick="window.print()" style="cursor: pointer" />
                </td>
                <td class="teacherTD">
                    <asp:Label runat="server" ID="lblTeacherName" /><br />
                    <asp:Label runat="server" ID="lblTeacherUserName" /><br/>
                    <asp:Label runat="server" ID="lblTeacherPosition" />
                </td>
                <td style="text-align: right; vertical-align: top;" width="300px">
                    <img src="../Images/SarasotaLogo.jpg" alt="Sarasota" width="300" />
                </td>
            </tr>
        </table>
        <br />
        <table width="100%" class="evalHeaderTable">
            <tr>
                <th style="text-align: left">
                    Evaluation Components
                </th>
                <th>
                    Points Earned
                </th>
                <th>
                    Concordant Points<br />
                    (0-4) Possible
                </th>
                <th>
                    Weight in Total<br />
                    Appraisal
                </th>
                <th>
                    Number of Years of<br />
                    Data Included
                </th>
            </tr>
            <tr>
                <td style="text-align: left">
                    <asp:Label runat="server" ID="lblPrideLabel" />
                </td>
                <td>
                    <a href="#" onclick="openEvalForm()">
                        <asp:Label runat="server" ID="lblTotalPoints" /></a> out of 100
                </td>
                <td width="150px">
                    <div class="progressBarDiv" runat="server" id="divPrideScoreContainer">
                        <div runat="server" id="divPrideScore">
                            &nbsp;
                        </div>
                    </div>
                </td>
                <td>
                    <asp:Label runat="server" ID="lblPrideWeight" /> 
                </td>
                <td>
                    <asp:Label runat="server" ID="lblPrideYears" />
                </td>
            </tr>
            <tr>
                <td style="text-align: left">
                    Student Growth Component 
                </td>
                <td width="150px">
                    <a href="#" runat="server" id="studentGrowthDetailsLnk" clientidmode="Static" onclick="launchStudentGrowthData()">Details </a>
                    <!--
                    <table width="100%" cellpadding="0" style="font-size: 12px;">
                        <tr>
                            <td style="padding: 0px !important">
                                <b>-3.00</b>
                            </td>                            
                            <td width="40%" style="text-align: right; border: 1px solid black; padding: 0px !important;
                                text-align: center;">
                                <div runat="server" id="divGrowthPointsNegative">
                                </div>
                            </td>
                            <td width="40%" style="text-align: left; border: 1px solid black; padding: 0px !important;
                                text-align: center;">
                                <div runat="server" id="divGrowthPointsPositive">
                                </div>
                            </td>                            
                            <td style="padding: 0px !important">
                                <b>3.00</b>
                            </td>
                        </tr>
                    </table>
                    -->
                </td>
                <td width="150px">
                    <div class="progressBarDiv" runat="server" id="divStudentGrowthScoreContainer">
                        <div runat="server" id="divStudentGrowthScore">
                            &nbsp;
                        </div>
                    </div>
                </td>
                <td>
                    <asp:Label runat="server" ID="lblStudentGrowthWeight" />
                </td>
                <td>
                    <asp:Label runat="server" ID="lblStudentGrowthYears" />
                </td>
            </tr>
            <tr>
                <td style="text-align: left">
                    Final Score
                </td>
                <td>
                </td>
                <td width="150px">
                    <div class="progressBarDiv" runat="server" id="divFinalScoreContainer">
                        <div runat="server" id="divFinalScore">
                            &nbsp;
                        </div>
                    </div>
                </td>
                <td>
                    
                </td>
                <td>
                    <asp:Label runat="server" ID="Label2" />
                </td>
            </tr>
        </table>
        <br />
        <table width="100%">
            <tr>
                <td width="50%" style="vertical-align: top">
                    <telerik:RadChart ID="evaluationChart" runat="server" Width="425px" Height="425px"
                        DefaultType="Bar" Skin="Marble" AutoLayout="true" AutoTextWrap="False" CreateImageMap="false">
                        <ChartTitle>
                            <TextBlock Text="Evaluation Components" />
                        </ChartTitle>                        
                    </telerik:RadChart>
                </td>
                <td width="50%" style="vertical-align: top">
                    <div class="finalRatingDiv">
                        Final Rating = <asp:Label runat="server" ID="lblFinalRating" />
                    </div>
                    <br /><br />
                    <table width="100%" cellpadding="3" class="concordantRangesTable">
                        <tr>
                            <th colspan="3">
                                Concordant Ranges
                            </th>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                Minimum
                            </td>
                            <td>
                                Maximum
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left;">
                                Highly Effective
                            </td>
                            <td>
                                3.00
                            </td>
                            <td>
                                4.00
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left;">
                                Effective
                            </td>
                            <td>
                                2.00
                            </td>
                            <td>
                                2.99
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left;">
                                Needs Improvement
                            </td>
                            <td>
                                1.00
                            </td>
                            <td>
                                1.99
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left;">
                                Unsatisfactory
                            </td>
                            <td>
                                0.00
                            </td>
                            <td>
                                0.99
                            </td>
                        </tr>
                    </table>
                    <br />
                    <div style="width: 100%; text-align: right">
                        <a href="javascript:" onclick="openResources()">Resources </a>
                    </div>
                    <br /><br /><br />

                    <!-- Per Tracy Godfrey, Unions in Sarasota would not accept the electronic signature section so it is to remain hidden until Sarasota approves. DHB 8/15/2012 -->

                    <div>
                        <div style="font-style: italic; font-size: 12px; padding-bottom:10px;">I acknowledge that I have reviewed this report.</div>
                        <asp:CheckBox runat="server" ID="chkEvaluateeSigned" OnCheckedChanged="chkSigned_Checked" CssClass="signCheckbox" 
                        AutoPostBack="true" onclick="if(!confirm('This action records your electronic signature including the date and time. Select OK to continue; otherwise select Cancel.')) return false;" />
                        <span class="signText" runat="server" id="divEvaluateeSignLine">
                            Electronically acknowledged by
                            <asp:Label runat="server" ID="lblEvaluateeSignature" ForeColor="Black" />
                            on
                            <asp:Label runat="server" ID="lblEvaluateeDate" ForeColor="Black" />
                        </span>
                        <hr />
                        <span class="signLabel">Evaluatee Signature:</span> <span class="signLabel" style="padding-left: 300px"> Date:</span>
                    </div>
                    <!--
                    <br />
                    <asp:CheckBox runat="server" ID="chkEvaluatorSigned" OnCheckedChanged="chkSigned_Checked"
                        AutoPostBack="true" onclick="return confirm('Are you sure you wish to electronically sign this evaluation?');" />
                    <div class="signText" runat="server" id="divEvaluatorSignLine">
                        Electronically signed by
                        <asp:Label runat="server" ID="lblEvaluatorSignature" ForeColor="Black" />
                        on
                        <asp:Label runat="server" ID="lblEvaluatorDate" ForeColor="Black" /></div>
                    <hr />
                    <span class="signLabel">Evaluator Signature:</span> <span class="signLabel" style="padding-left: 300px">
                        Date:</span>
                    -->
                </td>
            </tr>
        </table>
        <div id="concordantGroups" style="display: none;">
            <asp:Repeater runat="server" ID="concordantGroupsRepeater" OnItemDataBound="concordantGroupsRepeater_ItemDataBound">
                <HeaderTemplate>
                    <table width="100%" class="resourcesTable">
                        <tr>
                            <th style='background-color: #dadada'>
                                <a href='javascript:' onclick='openPRIDERubric("../faq/Concordant Table Group Descriptions.pdf");'>What Group Am I?</a>
                            </th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <th runat="server" id="groupNumberCell">
                            <a href='javascript:' onclick='openSGCT(this)'>
                                <asp:Label runat="server" ID="lblGroupNumber" /></a>
                            <asp:Repeater runat="server" ID="concordantYearsRepeater" OnItemDataBound="concordantYearsRepeater_ItemDataBound">
                                <HeaderTemplate>
                                    <div style="display: none" class="concordantList">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div>
                                    <a href="javascript:" onclick="openSGCT(this)" style="padding-left: 20px"><asp:Label runat="server" ID="lblGroupYear" /></a><br/>
                                    <asp:Repeater runat="server" ID="concordantForYearRepeater" OnItemDataBound="concordantRepeater_ItemDataBound">
                                        <HeaderTemplate>
                                            <div style="display: none" class="concordantList">
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <a href='javascript:' runat="server" id="aTagOpenConcordant" style="padding-left: 40px">
                                                <asp:Label runat="server" ID="lblConcordantName" />
                                            </a>
                                            <br />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </div>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>
                            <asp:Repeater runat="server" ID="concordantRepeater" OnItemDataBound="concordantRepeater_ItemDataBound">
                                <HeaderTemplate>
                                    <div style="display: none" class="concordantList">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <a href='javascript:' runat="server" id="aTagOpenConcordant" style="padding-left: 20px">
                                        <asp:Label runat="server" ID="lblConcordantName" />
                                    </a>
                                    <br />
                                </ItemTemplate>
                                <FooterTemplate>
                                    </div></FooterTemplate>
                            </asp:Repeater>
                        </th>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </telerik:RadAjaxPanel>
    </form>
</body>
</html>
