<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AnalysisPDFView.aspx.cs" Inherits="Thinkgate.ControlHost.AnalysisPDFView" %>
<html>
    <head>
        <title>Analysis Report</title>
    </head>
    <body>
        <div id="contentDiv" runat="server">
            <div style="width: 100%; text-align:center; vertical-align: top;" id="reportHeaderDiv" runat="server"></div>
            <div style="width: 100%; text-align:center; vertical-align: top;" id="barGraphLevelsContainerDiv" runat="server"></div>
            <div style="width: 100%; text-align:center; vertical-align: top;" id="barGraphPDFContainerDiv" runat="server"></div>
        </div>
    </body>
</html>