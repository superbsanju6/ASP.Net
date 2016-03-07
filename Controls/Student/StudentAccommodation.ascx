<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentAccommodation.ascx.cs"
    Inherits="Thinkgate.Controls.Student.StudentAccommodation" %>
<telerik:RadAjaxPanel runat="server" ID="studentAccommodationsPanel" LoadingPanelID="studentAccommodationsLoadingPanel">
    <telerik:RadMultiPage runat="server" ID="RadMultiPageStudentAccommodations" SelectedIndex="0"
        Height="210px" Width="300px" CssClass="multiPage">
        <telerik:RadPageView runat="server" ID="radPageSubjectLevel">
            <telerik:RadGrid ID="gridSubjectLevel" runat="server" Width="290px" AutoGenerateColumns="false" Height="200px" ClientSettings-Scrolling-AllowScroll="true">
                <MasterTableView>
                    <Columns>
                        <telerik:GridBoundColumn DataField="Description" HeaderText="Description" />
                        <telerik:GridCheckBoxColumn DataField="Value" HeaderText="Value" DataType="System.Boolean" ColumnEditorID="1" ReadOnly="False" ForceExtractValue="InEditMode" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="studentAccommodationsLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>
