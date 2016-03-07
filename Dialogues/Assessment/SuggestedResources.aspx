<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SuggestedResources.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.SuggestedResources" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Suggested Resources</title>
    <script src='../../Scripts/jquery-1.9.1.min.js'></script>
    <script src="../../Scripts/master.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            CurrentStatus = '<%= CurrentStatus %>';
            LevelColor = '<%= LevelColor %>';
            if (CurrentStatus == 'NOSTANDARD' || CurrentStatus == 'PROFICIENCYMET') {
                var oWnd = getCurrentCustomDialog();
                setTimeout(function () {
                    oWnd.set_height(260);
                    oWnd.center();
                }, 0);
            }
            if (LevelColor != null && LevelColor != '')
                $('.profLabelTd').css({'background-color':LevelColor});

         });
	</script>
    <style type="text/css">
        body {
            font-family: Calibri, Arial;
            background-color: #EDEDED;
        }
        a:link {
            color: #007FFF;
        }
        a:visited {
            color: #007FFF;
        }
        a:hover {
            color: #007FFF;
        }
        a:active {
            color: #007FFF;
        }
        .aname
        {
            font-size: 18px;
            font-style: normal;
            font-weight: lighter;
        }
        .message
        {
            font-size: 20px;
            text-decoration: underline;
            word-wrap: break-word;
            height: auto;
        }
        .messageContainer
        {
            width: 85%;
            padding-left: 10px;
            margin-top: 20px;
        }
        .headerLabel
        {
            font-size: 18px;
            font-weight: bold;
        }
        .backColorGray {
            background-color: #EDEDED;
            margin-top:10px;
        }
        .studNameLabel {
            margin-bottom: 5px;
            margin-left: 10px;
        }
        .repeterHeader {
            table-layout: fixed; 
            width: 95%; 
            margin-left: 10px;
        }
        .rptStudentProficiencyTableTr {
            background-color: #94B2D6;
            height: 15px;
            line-height: 15px;
        }
        .standTd {
            border: 1px solid black;
            width: 70%;
        }
        .proficiencyTd {
            width: 30%;
            border: 1px solid black;
            border-left: 0px solid black;
        }
        .height45px {
            height: 45px;
        }
        .hydStndTd {
            text-align: left; border: 1px solid black; border-top: 0px solid black; vertical-align: bottom; background-color: white;
        }
        .profLabelTd {
            text-align: left; border-bottom: 1px solid black; border-right: 1px solid black; vertical-align: bottom; background-color: red;
        }
        .width100p {
            width:100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divHeader" runat="server">
            <table class="width100p">
                <tr>
                    <td style="width:25%;"><span class="headerLabel">Assessment:</span></td>
                    <td style="width:75%;"><span class="aname"><%=this.AssessmentName %></span><br /></td>
                </tr>
                <tr>
                    <td></td>
                    <td><span style="font-size:14px;"><%= this.AssessmentHoverName%></span><br /></td>
                </tr>
            </table>
            <table class="width100p">
                <tr>
                    <td style="width:13%;"><span class="headerLabel">Level:</span><br /></td>
                    <td style="width:87%;"><asp:Label ID="LevelNameLabel" runat="server" CssClass="aname" Text=""></asp:Label></td>
                </tr>
            </table>
        </div>
        <div id="divProficiencyMet" class="messageContainer" runat="server" visible="false">
            <span class="message">The Proficiency Rating for all Standards on this assessment were met.</span>
        </div>
        <div id="divNoStandards" class="messageContainer" runat="server" visible="false">
            <span class="message">There are no Standards associated on this assessment.</span>
        </div>
        <div class="backColorGray">
            <asp:Repeater ID="rptStudents" runat="server" OnItemDataBound="rptStudents_ItemDataBound">
                <HeaderTemplate></HeaderTemplate>
                <ItemTemplate>
                    <div class="studNameLabel">
                        <asp:Label ID="StudentNameLabel" runat="server" Text='<%#Eval("StudentName") %>' Visible='<%# this.Level.Equals("Class", StringComparison.InvariantCultureIgnoreCase) %>'></asp:Label><br />
                    </div>
                    <asp:Repeater ID="rptStudentProficiency" runat="server" OnItemDataBound="rptStudentProficiency_ItemDataBound">
                        <HeaderTemplate>
                            <table cellpadding="5" cellspacing="0" class="repeterHeader">
                                <tr class="rptStudentProficiencyTableTr">
                                    <td class="standTd">Standard</td>
                                    <td class="proficiencyTd">Proficiency</td>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr class="height45px">
                                <td class="hydStndTd" style="word-wrap:break-word">
                                    <asp:HyperLink ID="hypStandard" Font-Bold="true" runat="server" Target="_blank"></asp:HyperLink>
                                </td>
                                <td class="profLabelTd">
                                    <asp:Label ID="ProficiencyLabel" runat="server" Text='<%#string.Format("{0:0.##}%", Eval("Proficiency")) %>'></asp:Label>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </ItemTemplate>
                <SeparatorTemplate>
                    <br />
                </SeparatorTemplate>
            </asp:Repeater>
        </div>
    </form>
</body>
</html>
