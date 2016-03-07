<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemAnalysisDetail.ascx.cs"
    Inherits="Thinkgate.Controls.Reports.ItemAnalysisDetail" %>
<telerik:RadAjaxPanel runat="server" ID="panelItemAnalysisDetail" LoadingPanelID="loadingpanelItemAnalysisDetail">
    <telerik:RadGrid runat="server" ID="reportGrid" AutoGenerateColumns="false" OnItemDataBound="reportGrid_ItemDataBound"
        AllowFilteringByColumn="false">
        <MasterTableView>
            <Columns>                
                <telerik:GridTemplateColumn HeaderText="Item #" DataField="ChartGroup">
                    <ItemTemplate>
                        <a href="#" onclick="openItemPreview('../ControlHost/PreviewTestQuestion.aspx?xID=<%# Eval("xID")%>'); "><%# Eval("ChartGroup")%></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="RBT" DataField="qrigor" />
                <telerik:GridBoundColumn HeaderText="% Correct" DataField="PctCorrectDisp" />
                <telerik:GridBoundColumn HeaderText="A Answers" DataField="AFreq#" />
                <telerik:GridBoundColumn HeaderText="B Answers" DataField="BFreq#" />
                <telerik:GridBoundColumn HeaderText="C Answers" DataField="CFreq#" />
                <telerik:GridBoundColumn HeaderText="D Answers" DataField="DFreq#" />
                <telerik:GridBoundColumn HeaderText="Unanswered" DataField="NoFreq#" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel runat="server" ID="loadingpanelItemAnalysisDetail">
</telerik:RadAjaxLoadingPanel>
<telerik:RadWindowManager runat="server" ID="radWindowDetailMgr">
    <Windows>
        <telerik:RadWindow runat="server" ID="radWindowPreview" VisibleOnPageLoad="false"
            Width="500" Height="400" Modal="true" CssClass="expandedTab" Title="Assessment Item Preview"
            Skin="Office2010Silver" VisibleStatusbar="false" Behaviors="Close">
        </telerik:RadWindow>
        <telerik:RadWindow runat="server" ID="radWindowPieChart" VisibleOnPageLoad="false"
            Width="500" Height="400" Modal="true" CssClass="expandedTab" Title=""
            Skin="Office2010Silver" VisibleStatusbar="false" Behaviors="Close">
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>