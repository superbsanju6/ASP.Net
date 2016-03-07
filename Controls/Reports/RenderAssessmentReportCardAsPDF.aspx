<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RenderAssessmentReportCardAsPDF.aspx.cs" Inherits="Thinkgate.Controls.Reports.RenderAssessmentReportCardAsPDF" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Thinkgate Elements</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <input id="reportServerUrl" type="hidden" runat="server" />
            <input id="reportPath" type="hidden" runat="server" />
            <input id="reportUsername" type="hidden" runat="server" />
            <input id="reportPassword" type="hidden" runat="server" />
            <input id="reportDomain" type="hidden" runat="server" />
            <label id="lblMessage" visible="false" runat="server"></label>
       </div>
    </form>
</body>
</html>