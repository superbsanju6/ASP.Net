<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardAnalysisDetail.ascx.cs"
    Inherits="Thinkgate.Controls.Reports.StandardAnalysisDetail" %>
<telerik:RadAjaxPanel runat="server" ID="panelItemAnalysisDetail" LoadingPanelID="loadingpanelItemAnalysisDetail">
    <telerik:RadGrid runat="server" ID="reportGrid" AutoGenerateColumns="false" OnItemDataBound="reportGrid_ItemDataBound"
        AllowFilteringByColumn="false">
        <MasterTableView>
            <Columns>                
                <telerik:GridTemplateColumn HeaderText="Standard" DataField="StandardNbr">
                    <ItemTemplate>
                        <a href="javascript:void(0);" onclick="openItemPreview('../ControlHost/PreviewStandard.aspx?xID=<%# Eval("xID") %>'); return false;"><%# Eval("StandardNbr")%></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="# Items" DataField="QuestionCount#" />
                <telerik:GridTemplateColumn HeaderText="Item #s" DataField="QuestionList">
                    <ItemTemplate>
                        <asp:Label ID="itemLinks" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="% Correct" DataField="PctCorrectDisp" />                
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
    </Windows>
</telerik:RadWindowManager>