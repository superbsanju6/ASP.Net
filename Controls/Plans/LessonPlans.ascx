<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LessonPlans.ascx.cs" Inherits="Thinkgate.Controls.Plans.LessonPlans" %>
<telerik:RadAjaxPanel runat="server" ID="lessonPlansPanel" LoadingPanelID="lessonPlansLoadingPanel">
    <style type="text/css">
        .searchResuls_smallTile_Items
        {
            text-align: left;
            vertical-align: top;
        }
        .lessonPlanView_icon img
        {
            width: 15px;
        }
    </style>
    <div style="width: 300px; height: 238px; overflow: hidden;">
        <div id="searchHolder" runat="server">
            <div class="buttonAreaFloatLeft">
                <telerik:RadComboBox runat="server" ID="resourceStandardsDropdown" ClientIDMode="AutoID" Skin="Web20" Text="All Standards"
                    CssClass="radDropdownBtn" Width="150" dropdownListID="resourceStandardsList" AutoPostBack="false" MaxHeight="150"
                    EmptyMessage="Standards" HighlightTemplatedItems="true" OnSelectedIndexChanged="ResourceStandards_OnSelectedIndexChanged" ToolTip="Select standards">
                    <ItemTemplate>
                        <div onclick="RadCombobox_CustomMultiSelectDropdown_StopPropagation(event)">
                            <asp:CheckBox runat="server" ID="resourceStandardsCheckbox" ClientIDMode="AutoID" />
                            <asp:Label runat="server" ID="resourceStandardsLabel" ClientIDMode="AutoID" AssociatedControlID="resourceStandardsCheckbox">
                                <img runat="server" id="okImgBtn" src="../../Images/ok.png" onclick="if(RadCombobox_CustomMultiSelectDropdown_reloadingTable) RadCombobox_CustomMultiSelectDropdown_reloadingTable=false; $('#' + this.parentNode.id.replace('resourceStandardsLabel', 'resourceStandardsCheckbox')).click();" style="cursor:pointer;">
                            </asp:Label>
                        </div>
                    </ItemTemplate>
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="resourceTypesDropdown" Skin="Web20" ClientIDMode="Static"
                    runat="server" EmptyMessage="Type" AutoPostBack="false" Width="125" HighlightTemplatedItems="true" 
                    OnSelectedIndexChanged="cmbType_SelectedIndexChanged" OnClientSelectedIndexChanged="lessonPlans_OnclientSelectedIndexChanged">
                </telerik:RadComboBox>
            </div>
        </div>
        <div class="graphicalView">
            <asp:Panel ID="pnlGraphicNoResults" runat="server" Visible="false" Height="190px">
                <div style="width: 100%; text-align: center;">
                    No lesson plans found for selected criteria.</div>
            </asp:Panel>
            <telerik:RadListBox runat="server" ID="lbxPlans" Width="100%" Height="190px"
                OnItemDataBound="lbxPlans_ItemDataBound">
                <ItemTemplate>
                    <asp:Image ID="testImg" Style="float: left; padding: 2px;" Width="47" Height="56"
                        ImageUrl='~/Images/lesson_plan.png' runat="server" />
                    <!-- Test name, Line 1 -->
                    <div>                        
                         <asp:Label ID="lblPlanName" runat="server"><%# Eval("PlanName")%></asp:Label>
                        <asp:HyperLink ID="lnkPlanName" runat="server" Visible="false" NavigateUrl=""
                            Target="_blank"><%# Eval("PlanName")%></asp:HyperLink>
                    </div>
                    <!-- Line 3 -->
                    <asp:Panel ID="graphicLine3Summary" runat="server" Visible="True">
                        <asp:ImageButton ID="btnGraphicUpdate" runat="server" ImageUrl="~/Images/Edit.png"
                            ToolTip="Update" Style="cursor: pointer;" />
                        <asp:HyperLink ID="lnkGraphicView" runat="server" ImageUrl="~/Images/ViewPage.png" CssClass="lessonPlanView_icon"
                            ToolTip="View" Style="cursor: pointer;" />
                        <!--<img id="imgGraphicPrint2" runat="server" src="~/Images/Printer.png" alt="Print"
                            title="Print" style="cursor: pointer;" onclick="alert('Missing event target')" />-->
                        <img id="imgAddToCalendar" runat="server" src="~/Images/commands/calendar_add_small.png"
                            alt="Add to Calendar" lessonplanid='<%# Eval("ID")%>' title="Add to Calendar" style="cursor: pointer;"
                            onclick="if(ShowInstructionalPlanCalendar) ShowInstructionalPlanCalendar('lessonplanid', this.getAttribute('lessonplanid'));"
                            visible="false" />
                    </asp:Panel>
                </ItemTemplate>
            </telerik:RadListBox>
        </div>
        <div runat="server" id="BtnAdd" class="searchTileBtnDiv_smallTile" title="Add Lesson Plan" style="margin-top: 1px;">
            <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
            <div style="padding: 0;">Add New</div>
        </div>
    </div>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="lessonPlansLoadingPanel" runat="server" />
