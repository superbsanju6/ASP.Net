<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardsProficiency.ascx.cs" Inherits="Thinkgate.Controls.Reports.StandardsProficiency" %>

<style type="text/css">
    .reportLink > div {
        padding: 3px;
    }

    .reportLink > div > a {
        font-size: 14px;
    }
</style>

<div style="text-align: center; padding-top: 15px; height: 85%; overflow-y: auto;" class="reportLink">
    <div id="CurriculaDiv" runat="server">
        <asp:Repeater ID="rptCurricula" runat="server">
            <ItemTemplate>
                <div style="text-align: center; padding-top: 15px; height: 85%;" class="reportLink">
                     <a target="_blank" href='<%= ResolveUrl("~/Controls/Reports/RenderStandardProficiencyReportAsPDF.aspx") %>?Grade=<%#Eval("EncryptedGrade")%>&Subject=<%# Eval("EncryptedSubject") %>&Course=<%# Eval("EncryptedCourse") %>&StudentId=<%= SelectedStudentID.Value %>'> <%# Eval("Grade") + "_" + Eval("Course") %> </a>
                </div>
            </ItemTemplate>
       </asp:Repeater>
        <asp:Label runat="server" ID="lblEmptyMessage" style="text-align: center; padding-top: 15px; font-size:14px;"></asp:Label>
    </div>
 <asp:HiddenField ID="SelectedStudentID" ClientIDMode="Static" runat="server" />
</div>