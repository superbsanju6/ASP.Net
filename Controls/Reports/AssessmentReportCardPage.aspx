<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentReportCardPage.aspx.cs" Inherits="Thinkgate.Controls.Reports.AssessmentReportCardPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assessment Report Card</title>
    <style type="text/css">
        .reportLink > div {
            padding: 3px;
        }

        .reportLink > div > a {
            font-size: 14px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center; padding-top: 15px; height: 85%;" class="reportLink">
            <div id="CurriculaDiv" runat="server">
                <asp:Repeater ID="rptCurricula" runat="server">
                    <ItemTemplate>
                        <div style="text-align: center; padding-top: 15px; height: 85%;" class="reportLink">
                            <a target="_blank" href='<%= ResolveUrl("~/Controls/Reports/RenderAssessmentReportCardAsPDF.aspx") %>?Grade=<%# Eval("EncryptedGrade") %>&Subject=<%# Eval("EncryptedSubject") %>&Course=<%# Eval("EncryptedCourse") %>&StudentId=<%= EncryptedStudentID.Value %>'><%# Eval("Grade") + "_" + Eval("Course") %> </a>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label runat="server" ID="lblEmptyMessage" Style="text-align: center; padding-top: 15px; font-size: 14px;"></asp:Label>
            </div>
            <asp:HiddenField ID="EncryptedStudentID" ClientIDMode="Static" runat="server" />
        </div>
    </form>
</body>
</html>
