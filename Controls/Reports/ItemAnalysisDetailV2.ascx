<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemAnalysisDetailV2.ascx.cs" Inherits="Thinkgate.Controls.Reports.ItemAnalysisDetailV2" %>
<style type="text/css">
    .bkColorForMCItems
    {
        background-color: #BDD3EB !important;
        
    }
    .bkColorForTFItems
    {
        background-color: #EAEFB9 !important;
    }
    .bkColorForRWItems
    {
        background-color: #E6CEAC !important;
    }
    .bkColorForRubricItems
    {
        background-color: #E2C9E6 !important;
    }
    .bkColorForUnAnsweredItems
    {
        background-color: #DFDFDF !important;
    }
    .toggleCtrl
    {
        display: inline;
    }
    .rgHeader {
        white-space: nowrap;
    }
</style>
<telerik:RadAjaxPanel runat="server" ID="panelItemAnalysisDetail" LoadingPanelID="loadingpanelItemAnalysisDetail">
    <div id="ItemAnalisysDetail_divColumnToggleControls" clientidmode="Static" style="text-align: center;">
        <div runat="server" id="ItemAnalysisDetail_MCToggle" clientidmode="Static" class="bkColorForMCItems toggleCtrl" style="display: inline">
            <input type="checkbox" onclick="toggleColumns(this, colAryMC);" checked="checked" />&nbsp; Choice&nbsp;&nbsp;</div>
        <div runat="server" id="ItemAnalysisDetail_TFToggle" clientidmode="Static" class="bkColorForTFItems toggleCtrl">
            <input type="checkbox" onclick="toggleColumns(this, colAryTF);" checked="checked" />&nbsp;True/False&nbsp;&nbsp;</div>
        <div runat="server" id="ItemAnalysisDetail_RWToggle" clientidmode="Static" class="bkColorForRWItems toggleCtrl">
            <input type="checkbox" onclick="toggleColumns(this, colAryRW);" checked="checked" />&nbsp;Right/Wrong&nbsp;&nbsp;</div>
        <div runat="server" id="ItemAnalysisDetail_RubricToggle" clientidmode="Static" class="bkColorForRubricItems toggleCtrl">
            <input type="checkbox" onclick="toggleColumns(this, colAryRubric);" checked="checked" />&nbsp;Rubric&nbsp;&nbsp;</div>
        <div runat="server" id="ItemAnalysisDetail_UnansweredToggle" clientidmode="Static" class="bkColorForUnAnsweredItems toggleCtrl">
            <input type="checkbox" onclick="toggleColumns(this, colAryUnanswered);" checked="checked" />&nbsp;Unanswered&nbsp;&nbsp;</div>
    </div>
    <telerik:RadGrid runat="server" ID="ItemAnalysisDetail_reportGrid" AutoGenerateColumns="false" OnItemDataBound="reportGrid_ItemDataBound" AllowFilteringByColumn="false" SkinsOverrideStyles="false">
        <MasterTableView>
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Item #" DataField="ChartGroup" >
                    <ItemTemplate>
                        <asp:Label ID="itemLinks" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="RBT" DataField="qrigor" />
                <telerik:GridBoundColumn HeaderText="% Correct" DataField="PctCorrectDisp" />
                <telerik:GridBoundColumn HeaderText="A Answers" DataField="AFreq#" HeaderStyle-CssClass="bkColorForMCItems" />
                <telerik:GridBoundColumn HeaderText="B Answers" DataField="BFreq#" HeaderStyle-CssClass="bkColorForMCItems" />
                <telerik:GridBoundColumn HeaderText="C Answers" DataField="CFreq#" HeaderStyle-CssClass="bkColorForMCItems" />
                <telerik:GridBoundColumn HeaderText="D Answers" DataField="DFreq#" HeaderStyle-CssClass="bkColorForMCItems" />
                <telerik:GridBoundColumn HeaderText="E Answers" DataField="EFreq#" HeaderStyle-CssClass="bkColorForMCItems" />

                <telerik:GridBoundColumn HeaderText="True" DataField="TFreq#" HeaderStyle-CssClass="bkColorForTFItems" />
                <telerik:GridBoundColumn HeaderText="False" DataField="FFreq#" HeaderStyle-CssClass="bkColorForTFItems" />
                <telerik:GridBoundColumn HeaderText="Right" DataField="RFreq#" HeaderStyle-CssClass="bkColorForRWItems" />
                <telerik:GridBoundColumn HeaderText="Wrong" DataField="WFreq#" HeaderStyle-CssClass="bkColorForRWItems" />
                <telerik:GridTemplateColumn HeaderText="RubricType" DataField="RubricType" HeaderStyle-CssClass="bkColorForRubricItems" >
                    <ItemTemplate>
	                    <%# Eval("RubricType").ToString() == "B" ? "H" : Eval("RubricType").ToString() %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="0 Points" DataField="0Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="1 Point" DataField="1Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="2 Points" DataField="2Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="3 Points" DataField="3Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="4 Points" DataField="4Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="5 Points" DataField="5Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="6 Points" DataField="6Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="7 Points" DataField="7Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="8 Points" DataField="8Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="9 Points" DataField="9Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="10 Points" DataField="10Freq#" HeaderStyle-CssClass="bkColorForRubricItems" />
                <telerik:GridBoundColumn HeaderText="Unanswered" DataField="NoFreq#" HeaderStyle-CssClass="bkColorForUnAnsweredItems" />
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <ClientEvents OnGridCreated="getGridObject"></ClientEvents>
        </ClientSettings>
    </telerik:RadGrid>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel runat="server" ID="loadingpanelItemAnalysisDetail">
</telerik:RadAjaxLoadingPanel>
<telerik:RadWindowManager runat="server" ID="radWindowDetailMgr">
    <Windows>
        <telerik:RadWindow runat="server" ID="radWindowPreview" VisibleOnPageLoad="false" Width="500" Height="400" Modal="true" CssClass="expandedTab" Title="Assessment Item Preview" Skin="Office2010Silver" VisibleStatusbar="false" Behaviors="Close">
        </telerik:RadWindow>
        <telerik:RadWindow runat="server" ID="radWindowPieChart" VisibleOnPageLoad="false" Width="500" Height="400" Modal="true" CssClass="expandedTab" Title="" Skin="Office2010Silver" VisibleStatusbar="false" Behaviors="Close">
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>
<telerik:RadScriptBlock runat="server">
<script type="text/javascript">

    var reportGrid;

    function getGridObject(sender, args) {
        reportGrid = sender;
    }

    /*********************************************************************
    Set up meaningful variables for grid columns.  See Code-Behind file
    for column index definitions - only wanted one location to deal with
    if changes are needed in the future.
    *********************************************************************/
    var colIdxItemNbr = 0;
    var colIdxRBT = 1;
    var colIdxPctCorrect = 2;
    var colIdxA = 3;
    var colIdxB = 4;
    var colIdxC = 5;
    var colIdxD = 6;
    var colIdxE = 7;
    var colIdxTrue = 8;
    var colIdxFalse = 9;
    var colIdxRight = 10;
    var colIdxWrong = 11;
    var colIdxRubricType = 12;
    var colIdxPt0 = 13;
    var colIdxPt1 = 14;
    var colIdxPt2 = 15;
    var colIdxPt3 = 16;
    var colIdxPt4 = 17;
    var colIdxPt5 = 18;
    var colIdxPt6 = 19;
    var colIdxPt7 = 20;
    var colIdxPt8 = 21;
    var colIdxPt9 = 22;
    var colIdxPt10 = 23;
    var colIdxUnanswered = 24;

    /*********************************************************************
    Set up meaningful groupings of grid columns
    *********************************************************************/
//    var colAryMC = [colIdxA, colIdxB, colIdxC, colIdxD];
//    var colAryTF = [colIdxTrue, colIdxFalse];
//    var colAryRW = [colIdxRight, colIdxWrong];
//    var colAryRubric = [colIdxRubricType, colIdxPt0, colIdxPt0, colIdxPt1, colIdxPt2, colIdxPt3, colIdxPt4, colIdxPt5, colIdxPt6, colIdxPt7, colIdxPt8, colIdxPt9, colIdxPt10];
    var colAryUnanswered = [colIdxUnanswered];

    /*********************************************************************
    toggleMCColumns - This function will toggle the columns in the grid that
    relate to Multiple Choice items
    *********************************************************************/
    function toggleColumns(ctrl, groupingArray) {
        var i;
        if (ctrl.checked == false) {
            //Hide the columns
            for (i = 0; i < groupingArray.length; i++) {
                reportGrid.get_masterTableView().hideColumn(groupingArray[i]);
            }
        } else {
            //Show the columns
            for (i = 0; i < groupingArray.length; i++) {
                reportGrid.get_masterTableView().showColumn(groupingArray[i]);
            }
        }
        return false;
    }

</script>
</telerik:RadScriptBlock>
