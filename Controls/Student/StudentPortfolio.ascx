<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentPortfolio.ascx.cs"
    Inherits="Thinkgate.Controls.Student.StudentPortfolio" %>
<telerik:RadComboBox ID="cmbYear" runat="server" ToolTip="Select a year" Skin="Web20"
    Width="95" OnSelectedIndexChanged="cmbYear_SelectedIndexChanged" AutoPostBack="true"
    CausesValidation="False" HighlightTemplatedItems="true">
</telerik:RadComboBox>
<telerik:RadGrid ID="gridPortfolio" runat="server" Width="98%" AutoGenerateColumns="false"
    OnItemDataBound="gridPortfolio_ItemDataBound">
    <MasterTableView>
        <Columns>
            <telerik:GridBoundColumn DataField="Name" HeaderText="Name" />
            <telerik:GridBoundColumn DataField="DateUploaded" HeaderText="Date Uploaded" />            
        </Columns>
    </MasterTableView>
</telerik:RadGrid>