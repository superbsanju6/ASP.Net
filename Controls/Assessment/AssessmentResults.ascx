<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentResults.ascx.cs"
    Inherits="Thinkgate.Controls.Assessment.AssessmentResults" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        function showReportWindow() {
            var oWnd = $find("<%= UserListDialog.ClientID %>");
            oWnd.show();
            oWnd.autoSize();
        }
    </script>
</telerik:RadScriptBlock>
<telerik:RadAjaxPanel ID="assessmentsResultsAjaxPanel" runat="server" LoadingPanelID="assessmentResultsLoadingPanel">
    <asp:HiddenField runat="server" ID="AssessmentResults_level" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="AssessmentResults_levelID" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="AssessmentResults_term" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="AssessmentResults_type" ClientIDMode="Static" />

    <!-- Filters -->
    <div style="z-index: 1; height: 24px; margin-left: 4px;">
        <telerik:RadComboBox ID="cmbGrade" runat="server" ToolTip="Select a grade"
            Skin="Web20" Width="69"
            OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged" AutoPostBack="true"
            CausesValidation="False" HighlightTemplatedItems="true">
            <ItemTemplate>
                <span><%# Eval("Grade") %></span>
            </ItemTemplate>
        </telerik:RadComboBox>
        <telerik:RadComboBox ID="cmbSubject" runat="server" ToolTip="Select a subject"
            Skin="Web20" Width="69"
            OnSelectedIndexChanged="cmbSubject_SelectedIndexChanged" AutoPostBack="true"
            CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
            <ItemTemplate>
                <span><%# Eval("DropdownText") %></span>
            </ItemTemplate>
        </telerik:RadComboBox>
        <telerik:RadComboBox ID="cmbTerm" runat="server" ToolTip="Select a term"
            Skin="Web20" Width="69"
            OnSelectedIndexChanged="cmbTerm_SelectedIndexChanged" AutoPostBack="true"
            CausesValidation="False" HighlightTemplatedItems="true">
            <ItemTemplate>
                <span><%# Eval("Term") %></span>
            </ItemTemplate>
        </telerik:RadComboBox>
        <telerik:RadComboBox ID="cmbTestType" runat="server" ToolTip="Select a test type"
            Skin="Web20" Width="69"
            OnSelectedIndexChanged="cmbTestType_SelectedIndexChanged" AutoPostBack="true"
            CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
            <ItemTemplate>
                <span><%# Eval("DisplayText") %></span>
            </ItemTemplate>
        </telerik:RadComboBox>
    </div>
    <!-- Pages -->
    <telerik:RadMultiPage runat="server" ID="radMultiPageAssessments" SelectedIndex="0"
        Height="186px" Width="100%" CssClass="multiPage" Style="overflow-y: auto ! important; overflow-x: hidden ! important;">
        <!-- List View -->
        <telerik:RadPageView runat="server" ID="rpvList">
            <telerik:RadGrid ID="rgAssessmentResults" runat="server" AllowPaging="False" AllowSorting="False"
                ClientIDMode="Static" Width="100%" AutoGenerateColumns="False" CellSpacing="0"
                GridLines="None" ShowFooter="false" AutoGenerateEditColumn="False"
                OnItemDataBound="rgAssessmentResults_ItemDataBound"
                OnNeedDataSource="rgAssessmentResults_NeedDataSource">
                <SortingSettings EnableSkinSortStyles="False" />
                <MasterTableView DataKeyNames="TestName">
                    <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                    </ExpandCollapseColumn>
                    <Columns>
                        <telerik:GridTemplateColumn DataField="TestName" HeaderText="Assessment Name" FilterControlAltText="Filter TemplateColumn column"
                            UniqueName="Description">
                            <ItemTemplate>
                                <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("TestName") + " " + Eval("Description") %>' ToolTip='<%# Eval("TestName") + " " + Eval("Description") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="70%" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="Proficient" HeaderText="" FilterControlAltText="Filter TemplateColumn column"
                            Visible="true" UniqueName="Proficient">
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkProficient" runat="server" Text='<%# Eval("Score") %>' ToolTip='<%# Eval("TestName") + " " + Eval("Description") %>'></asp:HyperLink>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="30%" />
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
    </telerik:RadMultiPage><!--
						no spaces between the tabstrip and multipage, in order to remove unnecessary whitespace
 -->
    <div>
        <telerik:RadTabStrip runat="server" ID="AssessmentResults_RadTabStrip" Orientation="HorizontalBottom" ClientIDMode="Static"
            SelectedIndex="1" MultiPageID="RadMultiPageClassSummary" Skin="Thinkgate_Blue"
            EnableEmbeddedSkins="False" OnTabClick="RadTabStrip2_TabClick">
            <Tabs>
                <telerik:RadTab runat="server" ID="stateRadTab" Text="State">
                </telerik:RadTab>
                <telerik:RadTab runat="server" ID="districtRadTab" Text="District">
                </telerik:RadTab>
                <telerik:RadTab runat="server" ID="classroomRadTab" Text="Classroom" Selected="True">
                </telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
    </div>
    <telerik:RadWindowManager ID="RadWindowManager2" runat="server" EnableShadow="true"
        AutoSize="true" Skin="Thinkgate_Window" EnableEmbeddedSkins="False" VisibleStatusbar="False">
        <Windows>
            <telerik:RadWindow ID="UserListDialog" runat="server" Title="Report Selection"
                ReloadOnShow="true" ShowContentDuringLoad="false" Modal="true" Behaviors="Close">
                <ContentTemplate>
                    <div id="contentDiv" clientidmode="Static" style="width: 350px; background-color: #f0f0f0; padding-top: 20px; padding-left: 20px; padding-bottom: 20px;">
                        <table style="width: 100%">
                            <tr>
                                <div id="Div1"></div>
                            </tr>
                            <tr>
                                <td id="list1">
                                    <div id="Div2"></div>
                                </td>
                                <td id="list2" valign="top">
                                    <div id="Div3"></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadWindow ID="criteriaWindow" runat="server" EnableShadow="true" Skin="Office2010Silver"
        Behaviors="Close" VisibleOnPageLoad="false" Animation="Slide"
        Modal="true" AutoSize="true" ClientIDMode="Static" VisibleStatusbar="false">
        <ContentTemplate>
            <div id="chooseReportDiv" clientidmode="Static" style="width: 450px; height: 350px; background-color: #f0f0f0; display: none;">
                <h1>Groups</h1>
                <table width="100%">
                    <tr>
                        <td width="33%">At Risk
                        </td>
                        <td width="33%">
                            <a href="#" onclick="openReport();">Analysis</a>
                        </td>
                        <td width="33%">My Group 2
                        </td>
                    </tr>
                    <tr>
                        <td width="33%">Mastery
                        </td>
                        <td width="33%">Report Cards
                        </td>
                        <td width="33%">My Group 3
                        </td>
                    </tr>
                    <tr>
                        <td width="33%">Results
                        </td>
                        <td width="33%">Progress Reports
                        </td>
                        <td width="33%"></td>
                    </tr>
                    <tr>
                        <td width="33%">Proficiency
                        </td>
                        <td width="33%">My Group 1
                        </td>
                        <td width="33%"></td>
                    </tr>
                </table>
                <br />
                <h1>Reports</h1>
                <table width="100%">
                    <tr>
                        <td width="50%">At Risk Students by Standard
                        </td>
                        <td width="50%">Distractor Analysis
                        </td>
                    </tr>
                    <tr>
                        <td width="50%">At Risk Students by Test
                        </td>
                        <td width="50%">Item Analysis
                        </td>
                    </tr>
                    <tr>
                        <td width="50%">At Risk Students by Domain
                        </td>
                        <td width="50%">Score Analysis
                        </td>
                    </tr>
                    <tr>
                        <td width="50%">At Risk Standards by Student
                        </td>
                        <td width="50%">Standard Analysis
                        </td>
                    </tr>
                    <tr>
                        <td width="50%">At Risk Subgroups by Domain
                        </td>
                        <td width="50%">Test Summary
                        </td>
                    </tr>
                    <tr>
                        <td width="50%">Mastery Students by Standard
                        </td>
                        <td width="50%">Progress Report by Student
                        </td>
                    </tr>
                    <tr>
                        <td width="50%">Mastery Students by Test
                        </td>
                        <td width="50%">Progress Report by Demographic
                        </td>
                    </tr>
                    <tr>
                        <td width="50%">Mastery Students by Domain
                        </td>
                        <td width="50%">Report Card by Standard
                        </td>
                    </tr>
                    <tr>
                        <td width="50%">Mastery Standards by Student
                        </td>
                        <td width="50%">Mastery Subgroups by Domain
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
        <telerik:RadXmlHttpPanel runat="server" ID="reportListXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/ReportsWCF.svc"
        WcfServiceMethod="GetReportListByLevel" RenderMode="Block" OnClientResponseEnding="loadReportList">
    </telerik:RadXmlHttpPanel>
</telerik:RadAjaxPanel>
<input type="hidden" runat="server" id="IsSuggestedResourcesVisible" ClientIDMode="Static" value=""/>
<input type="hidden" runat="server" id="IsStudentResponseVisible" ClientIDMode="Static" value="true"/> 
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="assessmentResultsLoadingPanel"
    runat="server" />