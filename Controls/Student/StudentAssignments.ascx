<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentAssignments.ascx.cs" Inherits="Thinkgate.Controls.Student.StudentAssignments" %>
<telerik:RadAjaxPanel ID="AssignmentsAjaxPanel" runat="server" LoadingPanelID="AsssignmentsLoadingPanel">
    <telerik:RadComboBox ID="cmbType" runat="server" 
        ToolTip="Select a type" 
        Skin="Web20"
        Width="95" 
        AutoPostBack="true"
        CausesValidation="False" 
        HighlightTemplatedItems="true" OnSelectedIndexChanged="cmbType_SelectedIndexChanged">
    </telerik:RadComboBox>
    <telerik:RadGrid ID="grdAssignments" runat="server" 
            Skin="Web20" 
            AutoGenerateColumns="false" 
            OnItemDataBound="grdAssignments_ItemDataBound">
        <MasterTableView>
            <Columns>
                <telerik:GridBoundColumn Visible="false" HeaderText="ID" DataField="ID" />
                <telerik:GridBoundColumn Visible="false" HeaderText="TypeID" DataField="TypeID" />
                <telerik:GridBoundColumn Visible="false" HeaderText="InfoID" DataField="InfoID" />
                <telerik:GridBoundColumn HeaderText="Type" DataField="Type" />
                <telerik:GridHyperLinkColumn HeaderText="Name" DataTextField="Name" />
                <telerik:GridBoundColumn HeaderText="Due Date" DataField="DueDate" DataFormatString="{0:d}" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="AssignmentsLoadingPanel" runat="server" />