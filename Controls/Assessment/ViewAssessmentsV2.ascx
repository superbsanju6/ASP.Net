<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewAssessmentsV2.ascx.cs" Inherits="Thinkgate.Controls.Assessment.ViewAssessmentsV2" %>

<div style="display: none;">
    <asp:Button ID="performanceLevelSetIcon" runat="server" OnClick="performanceLevelSetIcon_Click" ClientIDMode="Static" />
</div>

<telerik:RadAjaxPanel ID="viewAssessmentsAjaxPanel" runat="server" LoadingPanelID="viewAssessmentsLoadingPanel">
    <div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
		<telerik:RadComboBox ID="cmbGrade" runat="server" ToolTip="Select a grade" Skin="Web20" Width="69" AutoPostBack="true" CausesValidation="False" HighlightTemplatedItems="true">
            <ItemTemplate>
				<span><%# Eval("Grade") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbSubject" runat="server" ToolTip="Select a subject" Skin="Web20" Width="69" AutoPostBack="true" CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
            <ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbTerm" runat="server" ToolTip="Select a term" Skin="Web20" Width="69" AutoPostBack="true" CausesValidation="False" HighlightTemplatedItems="true">
            <ItemTemplate>
				<span><%# Eval("Term") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbTestType" runat="server" ToolTip="Select a test type" Skin="Web20" Width="69" AutoPostBack="true" CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
		     <ItemTemplate>
				<span><%# Eval("DisplayText") %></span>
			</ItemTemplate>
        </telerik:RadComboBox>
		<telerik:RadComboBox ID="cmbStatus" runat="server" ToolTip="Select a test status" Skin="Web20" Width="69" AutoPostBack="true" CausesValidation="False" HighlightTemplatedItems="true">
		    <ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
        </telerik:RadComboBox>
	</div>

    <telerik:RadMultiPage runat="server" ID="ItemsRadMultiPage" SelectedIndex="0"
        Height="190px" Width="100%" CssClass="multiPage OverflowY">
        <telerik:RadPageView runat="server" ID="radPageViewFormatives" Height="184px" Width="100%" CssClass="pageView">
            <div class="graphicalView" id="divGraphicalView" runat="server">
                <asp:Panel ID="pnlNoResults" runat="server" Visible="false" Height="190px">
                    <div style="width: 100%; text-align: center;">No assessments found for selected criteria.</div>
                </asp:Panel>
                <telerik:RadListBox runat="server" ID="lbx" Width="100%" Height="185px" OnItemDataBound="lbxList_ItemDataBound">
                    <ItemTemplate>
                        <asp:Image ID="testImg" Style="float: left; padding: 2px;" Width="47" Height="56" ImageUrl='~/Images/editable.png' runat="server" />
                        <div>
                            <asp:HyperLink ID="lnkTestName" runat="server" Target="_blank" Visible="True"></asp:HyperLink>
                            <asp:Label ID="lblDesc" runat="server"></asp:Label>
                        </div>
                        <asp:Panel ID="graphicLine2Edit" runat="server">
                            <span id="spnNumItems" runat="server"></span>
                            <asp:HyperLink ID="hlkEdit" runat="server" ImageUrl="~/Images/Edit.png"
                                ToolTip="Edit" Style="margin-left: 30px;" />
                            <asp:HyperLink ID="hlkPrint" runat="server" ImageUrl="~/Images/Printer.png"
                                ToolTip="Print" NavigateUrl="javascript:alert('Missing event target');" />
                            <%-- TFS: 6703--%>
                            <%--   <asp:HyperLink ID="hlkAdmin" runat="server" ImageUrl="~/Images/dashboard_small.png"
                            ToolTip="Administration" NavigateUrl="javascript:alert('Missing event target');" />--%>

                            <img alt="Administration" id="hlkAdmin" runat="server" src="~/Images/dashboard_small.png"
                                title="Administration" style="cursor: pointer; padding-right: 6px;" onclick="alert('Missing event target')" />
                        </asp:Panel>
                        <asp:Panel ID="graphicLine3Edit" runat="server" Visible="False">
                            <span id="spnLastEdit" runat="server"></span>
                        </asp:Panel>
                    </ItemTemplate>
                </telerik:RadListBox>
            </div>
        </telerik:RadPageView>

        <telerik:RadPageView runat="server" ID="radPageViewSecure" Height="184px" Width="100%" CssClass="pageView">
            <div class="secureView" id="divSecureView" runat="server">
                <asp:Panel ID="pnlNoResultsSecure" runat="server" Visible="false" Height="190px">
                    <div style="width: 100%; text-align: center;">No secure assessments found for selected criteria.</div>
                </asp:Panel>
                <telerik:RadListBox runat="server" ID="rlbSecure" Width="100%" Height="184px" OnItemDataBound="rlbSecureList_ItemDataBound">
                    <ItemTemplate>
                        <asp:Image ID="testImg" Style="float: left; padding: 2px;" Width="47" Height="56" ImageUrl='~/Images/editable.png' runat="server" />
                        <div>
                            <asp:HyperLink ID="lnkTestName" runat="server" Target="_blank" Visible="True"></asp:HyperLink>
                            <asp:Label ID="lblDesc" runat="server"></asp:Label>
                        </div>
                        <asp:Panel ID="graphicSecure" runat="server">
                            <span id="spnNumItemsSecure" runat="server"></span>
                            <asp:HyperLink ID="hlkEditSecure" runat="server" ImageUrl="~/Images/Edit.png"
                                ToolTip="Edit" Style="margin-left: 30px;" />
                            <asp:HyperLink ID="hlkPrintSecure" runat="server" ImageUrl="~/Images/Printer.png"
                                ToolTip="Print" NavigateUrl="javascript:alert('Missing event target');" />                           

                            <img alt="Administration" id="hlkAdminSecure" runat="server" src="~/Images/dashboard_small.png"
                                title="Administration" style="cursor: pointer; padding-right: 6px;" onclick="alert('Missing event target')" />
                        </asp:Panel>
                        <asp:Panel ID="graphicLine3EditSecure" runat="server" Visible="False">
                            <span id="spnLastEditSecure" runat="server"></span>
                        </asp:Panel>
                    </ItemTemplate>
                </telerik:RadListBox>
            </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>

    <div style="width: 41%; float: left; margin-top: -5px;">
        <telerik:RadTabStrip runat="server" ID="SecureFormativetabStrip" SelectedIndex="0" Orientation="HorizontalBottom" MultiPageID="ItemsRadMultiPage"
            Skin="Thinkgate_Blue" EnableEmbeddedSkins="false" Align="Left" OnTabClick="RadTabStrip1_TabClick"
            CssClass="radtabmarginclass">
            <Tabs>
                <telerik:RadTab Text="Formative" CssClass="class1_text" Width="74">
                </telerik:RadTab>
                <telerik:RadTab Text="Secure" CssClass="class1_text" Width="53">
                </telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
    </div>
      <div style="font-size:10.4px">
        <div runat="server" id="BtnAdd" class="searchTileBtnDiv_smallTile" title="Add Assessment" style="margin: 0px; margin-top: 1px; margin-right: 5px;"
            onclick="customDialog({ name: 'RadWindowAddAssessment', url: '../Dialogues/Assessment/CreateAssessmentOptions.aspx?level=' + this.getAttribute('level') + '&amp;testCategory=' + this.getAttribute('testCategory'), title: 'Options to Create Assessment', maximize: true, maxwidth: 550, maxheight: 450});">
            <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
            
                Add
            
        </div>
        <div runat="server" id="btnScheduler" class="searchTileBtnDiv_smallTile" title="Scheduler" style="margin: 0px; margin-top: 1px;"
            onclick="window.open('../Controls/Assessment/AssessmentScheduling.aspx?xID=' + this.getAttribute('xID') + '&amp;yID=' + this.getAttribute('yID'));">
            <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_SchedulerIcon"></span>
            
                Schedule
            
        </div>

        <div runat="server" id="BtnTestEvents" class="searchTileBtnDiv_smallTile" title="Test Events" style="margin: 0px; margin-top: 1px;"
            onclick="customDialog({name: 'RadWindowTestEvents', url: '../Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?navigateFrom=' + 'TestEvents'  + '&amp;level= ' + this.getAttribute('level') + '&amp;testCategory=' + this.getAttribute('testCategory'), title: 'Assessment Search', maximize: true, maxwidth: 950, maxheight: 675});">
            <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_hourglassIcon" style="left: 7px;"></span>

            Test Events

        </div>
    </div>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="viewAssessmentsLoadingPanel" runat="server" />
<script>
    function OnClientTabSelected(sender, eventArgs) {
        var tab = eventArgs.get_tab();
        if (tab.get_text() == "Formatives") {
            document.getElementById('ComboOuterDiv').style.display = '';
        }
        else { document.getElementById('ComboOuterDiv').style.display = 'none'; }
    }

</script>

<style type="text/css">
     .OverflowY
    {
         overflow-y: hidden !important;
         overflow-x: hidden !important;
         border: 1px solid #808080;
         clear: both !important;
    }
</style>