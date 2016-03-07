<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentGrades.ascx.cs"
    Inherits="Thinkgate.Controls.Student.StudentGrades" %>
<telerik:RadAjaxPanel ID="studentGradesAjaxPanel" runat="server" LoadingPanelID="studentGradesLoadingPanel">
    <!-- Filter combo boxes -->
    <div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
        <telerik:RadComboBox ID="cmbYear" runat="server" ToolTip="Select a year" Skin="Web20"
            Width="95" OnSelectedIndexChanged="cmbYear_SelectedIndexChanged" AutoPostBack="true"
            CausesValidation="False" HighlightTemplatedItems="true">
        </telerik:RadComboBox>
        <telerik:RadComboBox ID="cmbTerm" runat="server" ToolTip="Select a term" Skin="Web20"
            Width="69" OnSelectedIndexChanged="cmbTerm_SelectedIndexChanged" AutoPostBack="true"
            CausesValidation="False" HighlightTemplatedItems="true">
        </telerik:RadComboBox>
    </div>
    <telerik:RadGrid ID="gridStudentGrades" runat="server" Width="98%" AutoGenerateColumns="false"
        HeaderStyle-Height="0">
        <MasterTableView>
            <Columns>
                <telerik:GridBoundColumn DataField="FriendlyName" />
                <telerik:GridBoundColumn DataField="StudentGrade" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="studentGradesLoadingPanel"
    runat="server" />
