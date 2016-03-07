<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RenderAssessmentItemUsageReportView.aspx.cs" Inherits="Thinkgate.Record.RenderAssessmentItemUsageReportView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1 style="text-align:center">Assessment Item Usage Report</h1>
        <table border="1" style="width:100%;">
            <tr>
                <td style="width:30%;word-wrap:normal">Assessment Item Usage Report as of: <asp:Label ID="lblDate" runat="server"></asp:Label></td>
                <td style="width:70%;word-wrap:normal">
                    <asp:GridView ID="GridView3" runat="server" Visible="true" Width="100%" ShowHeader="false">
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <br/>
        <asp:GridView ID="GridView1" runat="server" Visible="true" Width="100%">
        </asp:GridView>
        <br/>
        <asp:GridView ID="GridView2" runat="server" Visible="true" Width="100%">
        </asp:GridView>
    </div>
    </form>
</body>
</html>
