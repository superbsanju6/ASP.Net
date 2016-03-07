<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pacing.aspx.cs" Inherits="Thinkgate.Dialogues.Pacing" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="width: 950px;">
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
        .pacingTable
        {
            width: 100%;
            background-color: White;
        }
        
        .pacingTable th
        {
            background-color: #dadada;
            font-weight: bold;
            border: 1px solid black;
            padding: 3px;
        }
        
        .pacingTable td
        {
            border: 1px solid black;
            vertical-align: top;
            padding: 3px;
        }
        
        .bluePacingTD
        {
            border: 1px solid black;
            text-align: center;
            background-color: Aqua;
            vertical-align: top;
        }
        
        .pacingCenterTD
        {
            border: 1px solid black;
            text-align: center;
            vertical-align: top;
        }
        
        .pacingHeaderDiv
        {
            width: 100%;
            background-color: #dadada;
            text-align: center;
            font-weight: bold;
            padding; 3px;
        }
        
        .pacingBodyDiv 
        {
            width: 100%;
            padding: 3px;    
            height: 225px;
            overflow: auto;       
        }
        
        span.heavy 
        {
            font-weight: bold;
            font-size: 16px;
            padding: 8px;
        }
    </style>
    <script type="text/javascript">
        function changeSelectedStandard(standardID) {
            document.getElementById("hiddenSelectedStandard").value = standardID;
        }
    </script>
</head>
<body style="font-family: Arial, Sans-Serif; font-size: 10pt;
    width: 950px;">
    <form runat="server" id="mainForm" method="post" style="width: 950px;">
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
    <asp:HiddenField runat="server" ID="hiddenSelectedStandard" ClientIDMode="Static" />
    <asp:Repeater runat="server" ID="repeaterStandards" OnItemDataBound="repeaterStandards_ItemDataBound">
        <HeaderTemplate>
            <table cellpadding="3" class="pacingTable">
                <tr>
                    <th>
                        &nbsp;
                    </th>
                    <th>
                        Standards
                    </th>
                    <th colspan="4">
                        Schedule: <span class="subtle">Instructional Days:
                            <asp:Label runat="server" ID="lblInstructionalDays" /></span> <span class="subtle">Unplanned
                                Days:
                                <asp:Label runat="server" ID="lblUnplannedDays" /></span>
                    </th>
                    <th colspan="2" style='text-align: center'>
                        Score
                    </th>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td class="bluePacingTD">
                            Suggested Days
                        </td>
                        <td class="bluePacingTD">
                            Completed
                        </td>
                        <td class="bluePacingTD">
                            Actual Days
                        </td>
                        <td class="bluePacingTD">
                            +/- Days
                        </td>
                        <td style='text-align: center'>
                            District
                        </td>
                        <td style='text-align: center'>
                            Class
                        </td>
                    </tr>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr runat="server" id="trStandard">
                <td class='pacingCenterTD'>
                    <asp:RadioButton runat="server" ID="radioSelectedItem" GroupName="standardRadioButtons" AutoPostBack="true" 
                            onclick='changeSelectedStandard(this.parentNode.title);'  ToolTip='<%# Eval("ID") %>' />
                </td>
                <td>
                    <%# Eval("StandardName") %>
                </td>
                <td class='pacingCenterTD'>
                    <%# Eval("SuggestedDays")%>
                </td>
                <td class='pacingCenterTD'>
                    <asp:CheckBox runat="server" ID="chkIsCompleted" />
                </td>
                <td class='pacingCenterTD'>
                    <%# Eval("ActualDays") %>
                </td>
                <td class='pacingCenterTD'>
                    <%# Eval("PlusMinusDays")%>
                </td>
                <td class='pacingCenterTD' runat="server" id="tdDistrictScore">
                    <%# Eval("DistrictScore")%>
                </td>
                <td class='pacingCenterTD' runat="server" id="tdClassroomScore">
                    <%# Eval("ClassroomScore") %>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <table class="pacingTable">
        <tr>
            <td width="33%">
                <div class="pacingHeaderDiv">
                    Essential Learning</div>
                <div class="pacingBodyDiv" runat="server" id="essentialLearningDiv">
                </div>
            </td>
            <td width="33%">
                <div class="pacingHeaderDiv">
                    Essential Terminology</div>
                <div class="pacingBodyDiv" runat="server" id="essentialTerminologyDiv">
                </div>
            </td>
            <td width="33%">
                <div class="pacingHeaderDiv">
                    Assessments</div>
                <div class="pacingBodyDiv">
                    <table width="95%">
                        <tr>
                            <td width="50%" class="pacingCenterTD">
                                District
                            </td>
                            <td width="50%" class="pacingCenterTD">
                                Classroom
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Repeater runat="server" ID="repeaterDistrictAssessments">
                                    <ItemTemplate>
                                        <a href="#" onclick="alert('Function in Development');">
                                            <%# Eval("TestName")%></a><br />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                            <td>
                                <asp:Repeater runat="server" ID="repeaterClassroomAssessments">
                                    <ItemTemplate>
                                        <a href="#" onclick="alert('Function in Development');">
                                            <%# Eval("TestName")%></a><br />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td width="33%">
                <div class="pacingHeaderDiv">
                    Lesson Plans</div>
                <div class="pacingBodyDiv">
                    <asp:Repeater runat="server" ID="repeaterLessonPlans">
                        <ItemTemplate>
                            <a href="#" onclick="alert('Function in Development');">
                                <%# Eval("PlanName")%></a><br />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </td>
            <td width="33%">
                <div class="pacingHeaderDiv">
                    Resources</div>
                <div class="pacingBodyDiv">
                     <asp:Repeater runat="server" ID="repeaterResources">
                        <ItemTemplate>
                            <a href="#" onclick="alert('Function in Development');">
                                <%# Eval("ResourceName")%></a><br />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </td>
            <td width="33%">
                <div class="pacingHeaderDiv">
                    Performance</div>
                <div class="pacingBodyDiv">
                    <span class="heavy">My Classes</span><br />
                    <br />
                    <span class="heavy">My School</span><br />
                    <br />
                    <span class="heavy">My District</span><br />
                </div>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
				

    </script>
    </form>
</body>
</html>
