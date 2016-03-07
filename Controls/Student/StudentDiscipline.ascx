<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentDiscipline.ascx.cs"
    Inherits="Thinkgate.Controls.Student.StudentDiscipline" %>
<telerik:RadAjaxPanel runat="server" ID="studentDisciplinePanel" LoadingPanelID="studentDisciplineLoadingPanel">
    <telerik:RadComboBox ID="cmbYear" runat="server" ToolTip="Select a year" Skin="Web20"
        Width="95" OnSelectedIndexChanged="cmbYear_SelectedIndexChanged" AutoPostBack="true"
        CausesValidation="False" HighlightTemplatedItems="true">
    </telerik:RadComboBox>
    <telerik:RadComboBox ID="cmbTerm" runat="server" ToolTip="Select a term" Skin="Web20"
        Width="69" OnSelectedIndexChanged="cmbTerm_SelectedIndexChanged" AutoPostBack="true"
        CausesValidation="False" HighlightTemplatedItems="true">
    </telerik:RadComboBox>
    <telerik:RadMultiPage runat="server" ID="RadMultiPageStudentDiscipline" SelectedIndex="0"
        Height="185px" Width="300px" CssClass="multiPage">
        <telerik:RadPageView runat="server" ID="radPageGradeLevel">
            <telerik:RadGrid ID="gridGradeLevel" runat="server" Width="98%" AutoGenerateColumns="false" OnItemDataBound="grid_ItemDataBound">
                <MasterTableView>
                    <Columns>
                        <telerik:GridBoundColumn DataField="Grade" HeaderText="" />
                        <telerik:GridBoundColumn DataField="WTD" HeaderText="WTD" />
                        <telerik:GridBoundColumn DataField="MTD" HeaderText="MTD" />
                        <telerik:GridBoundColumn DataField="YTD" HeaderText="YTD" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="radPageViewReferral">
            <telerik:RadGrid ID="gridReferralType" runat="server" Width="98%" AutoGenerateColumns="false" OnItemDataBound="grid_ItemDataBound">
                <MasterTableView>
                    <Columns>
                        <telerik:GridBoundColumn DataField="ReferralType" HeaderText="" />
                        <telerik:GridBoundColumn DataField="WTD" HeaderText="WTD" />
                        <telerik:GridBoundColumn DataField="MTD" HeaderText="MTD" />
                        <telerik:GridBoundColumn DataField="YTD" HeaderText="YTD" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="radPageViewConsequences">
            <telerik:RadGrid ID="gridConsequences" runat="server" Width="98%" AutoGenerateColumns="false" OnItemDataBound="grid_ItemDataBound">
                <MasterTableView>
                    <Columns>
                        <telerik:GridBoundColumn DataField="Consequence" HeaderText="" />
                        <telerik:GridBoundColumn DataField="WTD" HeaderText="WTD" />
                        <telerik:GridBoundColumn DataField="MTD" HeaderText="MTD" />
                        <telerik:GridBoundColumn DataField="YTD" HeaderText="YTD" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    <telerik:RadTabStrip runat="server" ID="studentAttendanceRadTabStrip" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageStudentDiscipline" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left" OnClientTabSelected="toggleView_RadTab_SmallTile">
        <Tabs>
            <telerik:RadTab Text="Grade Level" Width="90px">
            </telerik:RadTab>
            <telerik:RadTab Text="Referral Type" Width="90px">
            </telerik:RadTab>
            <telerik:RadTab Text="Consequences" Width="90px">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="studentDisciplineLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>
