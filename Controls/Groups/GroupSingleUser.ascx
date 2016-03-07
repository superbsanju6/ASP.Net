<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupSingleUser.ascx.cs" Inherits="Thinkgate.Controls.Groups.GroupSingleUser" %>

<script type="text/javascript">
    
    function openInfoWindow() {
        var url = '../Dialogues/AddNewGroup.aspx';
        customDialog({ url: url, width: 600, height: 400, maximize: false, onClosed: OnAddNewGroupClosed });
    }

    function OnAddNewGroupClosed(args) {
        //parent.__doPostBack();
        window.top.location.href = window.top.location.href;
    }

    function openAddNewGroup(ID, Name) {
        var url = '../Dialogues/AddUsersToGroup.aspx?groupId=' + ID;
        customDialog({ url: url, width: 950, height: 735, onClosed: OnAddNewGroupClosed, title: 'Group: ' + Name});
    }
</script>

<telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />

<telerik:RadAjaxManagerProxy runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="grdSingleUserMine" LoadingPanelID="updPanelLoadingPanel" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>

<telerik:RadGrid ID="grdSingleUserMine" runat="server" AllowSorting="True" Width="300px" Height="190px" 
    AutoGenerateColumns="false" OnNeedDataSource="LoadGroups" Style="background-image: url(../../Images/transparent_bkgd.png);">
    <MasterTableView>
        <Columns>
            <telerik:GridTemplateColumn HeaderText="Groups">
                <ItemTemplate>
                    <asp:LinkButton ID="GroupButton" ForeColor="Blue" runat="server" Text='<%#Eval("Name")%>' OnClientClick='<%#String.Format("openAddNewGroup({0}, \"{1}\"); return false;", Eval("ID"), Eval("Name"))%>' />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="StudentCount" HeaderText="Count" />
        </Columns>
    </MasterTableView>
    <ClientSettings>
        <Scrolling AllowScroll="True" />
    </ClientSettings>
</telerik:RadGrid>

<div class="searchTileBtnDiv_smallTile" title="Add Group" onclick="openInfoWindow()">
    <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
    Add New
</div>




