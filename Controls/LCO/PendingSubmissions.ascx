<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PendingSubmissions.ascx.cs" Inherits="Thinkgate.Controls.LCO.PendingSubmissions" %>
<script type="text/javascript">
    function showAddNewCourseWindow() {
        customDialog({ url: '../Controls/LCO/AddCourse.aspx', maximize: true, maxwidth: 550, maxheight: 500 });
    }
</script>




<telerik:RadAjaxPanel ID="pendingSubmissionsAjaxPanel" runat="server" LoadingPanelID="pendingSubmissionsLoadingPanel">
    <!-- Filter combo boxes -->
    <div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
        <telerik:RadComboBox ID="cmbSubject" runat="server" ToolTip="Select a subject"
            Skin="Web20" Width="69"
            OnSelectedIndexChanged="cmb_SelectedIndexChanged" AutoPostBack="true"
            CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
            <ItemTemplate>
                <span><%# Eval("DropdownText") %></span>
            </ItemTemplate>
        </telerik:RadComboBox>
        <telerik:RadComboBox ID="cmbLEA" runat="server" ToolTip="Select a LEA"
            Skin="Web20" Width="69"
            OnSelectedIndexChanged="cmb_SelectedIndexChanged" AutoPostBack="true"
            CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
            <ItemTemplate>
                <span><%# Eval("DropdownText") %></span>
            </ItemTemplate>
        </telerik:RadComboBox>
    </div>

    <!-- Pages -->
    <asp:HiddenField ID="currView" runat="server" Value="listView" ClientIDMode="Static" />

    <!-- List View -->
    <div class="listView" id="divListView" runat="server">
        <div id="divListNoResults" runat="server" visible="false" style="width: 100%; text-align: center;">No pending submissions found for selected criteria.</div>
        <telerik:RadListBox runat="server" ID="lbxList" Width="100%" Height="185px">
            <ItemTemplate>
                <!-- First line -->
                <div>
                    <asp:HyperLink ID="lnkListCourseName" runat="server"  Target="_blank" Visible="True"><%# Eval("TestName")%></asp:HyperLink>
                    <asp:Label ID="lblDesc" runat="server" ToolTip='<%# Eval("Description")%>'><%# Eval("ListDescription")%></asp:Label>
                    <asp:Image ID="testImg" Style="float: left; padding: 2px;" Width="47" Height="56" runat="server" />
                </div>
                <div>
                    <asp:Label ID="lblLEA" runat="server"></asp:Label>
                </div>
                <div>
                    <asp:Label ID="lblDateEdited" runat="server"></asp:Label>
                </div>
            </ItemTemplate>
        </telerik:RadListBox>
    </div>
</telerik:RadAjaxPanel>

<div class="tabsAndButtonsWrapper_smallTile">
    <div id="addNewCourse" runat="server" class="searchTileBtnDiv_smallTile" visible="false" title="Add New Course" onclick="showAddNewCourseWindow()">
        <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
        <div style="padding: 0;">
            Add New
        </div>
    </div>
</div>
<!-- Bottom action button(s) -->
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="pendingSubmissionsLoadingPanel"
    runat="server" />
