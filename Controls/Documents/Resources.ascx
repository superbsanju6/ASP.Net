<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Resources.ascx.cs" Inherits="Thinkgate.Controls.Documents.Resources" %>
<telerik:RadAjaxPanel runat="server" ID="resourcesPanel" LoadingPanelID="resourcesLoadingPanel">
    <style type="text/css">
        .searchResuls_smallTile_Items
        {
            text-align: left;
            vertical-align: top;
        }
        .resourceView_icon img
        {
            width: 15px;
        }
    </style>
    <div style="width: 300px; height: 238px; overflow: hidden;">
        <div id="searchHolder" runat="server">
            <div class="buttonAreaFloatLeft" style="z-index: 500">
                <telerik:RadComboBox ID="cmbStandard" runat="server" ToolTip="Select a standard"
                    Skin="Web20" Width="69" OnItemChecked="cmbStandard_ItemChecked" CheckBoxes="true"
                    AutoPostBack="true" CausesValidation="False" EmptyMessage="Standards" MaxHeight="150">
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="cmbGrade" runat="server" ToolTip="Select a grade" Skin="Web20"
                    Width="70" OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged" AutoPostBack="true"
                    CausesValidation="False" HighlightTemplatedItems="true">
                    <ItemTemplate>
                        <span>
                            <%# Eval("Grade") %></span>
                    </ItemTemplate>
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="cmbSubject" runat="server" ToolTip="Select a subject" Skin="Web20"
                    Width="70" OnSelectedIndexChanged="cmbSubject_SelectedIndexChanged" AutoPostBack="true"
                    CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
                    <ItemTemplate>
                        <span>
                            <%# Eval("Subject") %></span>
                    </ItemTemplate>
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="cmbCourse" runat="server" ToolTip="Select a course" Skin="Web20"
                    Width="70" OnSelectedIndexChanged="cmbCourse_SelectedIndexChanged" AutoPostBack="true"
                    CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
                    <ItemTemplate>
                        <span>
                            <%# Eval("Course") %></span>
                    </ItemTemplate>
                </telerik:RadComboBox>
                 <telerik:RadComboBox ID="cmbCategory" runat="server" ToolTip="Select a course" Skin="Web20"
                    Width="70" OnSelectedIndexChanged="cmbCategory_SelectedIndexChanged" AutoPostBack="true"
                    CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="cmbType" runat="server" ToolTip="Select a course" Skin="Web20"
                    Width="70" OnSelectedIndexChanged="cmbType_SelectedIndexChanged" AutoPostBack="true"
                    CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="cmbSubtype" runat="server" ToolTip="Select a course" Skin="Web20"
                    Width="70" OnSelectedIndexChanged="cmbSubtype_SelectedIndexChanged" AutoPostBack="true"
                    CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
                </telerik:RadComboBox>
            </div>
        </div>
        <div class="graphicalView">
            <asp:Panel ID="pnlGraphicNoResults" runat="server" Visible="false" Height="210px">
                <div style="width: 100%; text-align: center;">
                    <br />
                    <br />
                    No resources found for selected criteria.
                </div>
            </asp:Panel>
            <telerik:RadListBox runat="server" ID="lbxResources" Width="100%" Height="190px"
                OnItemDataBound="lbxResources_ItemDataBound">
                <ItemTemplate>
                    <asp:Image ID="testImg" Style="float: left; padding: 2px;" Width="47" Height="56"
                        ImageUrl='~/Images/resources.png' runat="server" />
                    <!-- Test name, Line 1 -->
                    <div>
                        <asp:Label ID="lblResourceName" runat="server"><%# Eval("ResourceName")%></asp:Label>
                        <asp:HyperLink ID="lnkResourceName" runat="server" Visible="false" NavigateUrl=""
                            Target="_blank"><%# Eval("ResourceName")%></asp:HyperLink>
                            <asp:Label ID="lblResourceDesc" runat="server" ToolTip='<%# Eval("Description")%>'><%# Eval("Description")%></asp:Label>
                    </div>
                    <!-- Line 3 -->
                    <asp:Panel ID="graphicLine3Summary" runat="server" Visible="True">
                        <asp:HyperLink ID="btnGraphicUpdate" runat="server" ImageUrl="~/Images/Edit.png" ToolTip="Update" Target="_blank" />
                        <asp:HyperLink ID="lnkGraphicView" runat="server" ImageUrl="~/Images/ViewPage.png" CssClass="resourceView_icon" ToolTip="View" />
                        <!--
                        <img id="imgGraphicPrint2" runat="server" src="~/Images/Printer.png" alt="Print"
                            title="Print" style="cursor: pointer;" onclick="alert('Missing event target')" />-->
                        <img id="imgAddToCalendar" runat="server" src="~/Images/commands/calendar_add_small.png"
                            alt="Add to Calendar" resourceid='<%# Eval("ID")%>' title="Add to Calendar" style="cursor: pointer;"
                            onclick="if(ShowInstructionalPlanCalendar) ShowInstructionalPlanCalendar('resourceid', this.getAttribute('resourceid'));"
                            visible="false" />
                    </asp:Panel>
                </ItemTemplate>
            </telerik:RadListBox>
        </div>
        <div runat="server" id="btnAdd" class="searchTileBtnDiv_smallTile" title="Add Resource"
            style="margin-top: 1px;">
            <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon">
            </span>
            <div style="padding: 0;">
                Add New</div>
        </div>
    </div>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="resourcesLoadingPanel" runat="server" />
