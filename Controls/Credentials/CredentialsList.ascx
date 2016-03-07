<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CredentialsList.ascx.cs" Inherits="Thinkgate.Controls.Credentials.CredentialsList" %>

<telerik:RadCodeBlock ID="jsCredentialList" runat="server">
    <script type="text/javascript">
        function showAddNewCredentialWindow() {
            customDialog({ name: "RadWindowManager1", url: '../Controls/Credentials/AddCredential.aspx', title: "New Credential", maximize: true, maxwidth: 500, maxheight: 500 });
        }

        function openTeacherSearchExpanded(objLevel, objLevelID) {
            var level = objLevel.value;
            var levelID = objLevelID.value;
            var url = '../Controls/Teacher/TeacherSearch_Expanded.aspx?level=' + level + '&levelID=' + levelID;

            customDialog({ url: url, maximize: true, maxwidth: 950, maxheight: 675 });
        }

        function reloadPlanningCredentialsTile() {
            <%= this.Page.ClientScript.GetPostBackEventReference(btnPostbackTarget, "TileRefresh") %>
        }
    </script>
</telerik:RadCodeBlock>
<telerik:RadAjaxPanel ID="RadAjaxPanelCredentials" runat="server" LoadingPanelID="RadAjaxLoadingPanelCredential">
    <asp:Button ID="btnPostbackTarget" runat="server" OnClick="btnPostbackTarget_Click" style="display: none;" />
    <telerik:RadMultiPage ID="RadMultiPageCredentials" runat="server" SelectedIndex="0"
        Height="209px" Width="309px" CssClass="multiPage">
        <telerik:RadPageView ID="RadPageViewCredentials" runat="server">
            <telerik:RadGrid ID="RadGridActiveCredential"  ClientSettings-Scrolling-UseStaticHeaders="true" runat="server" Width="309px" AutoGenerateColumns="false" Height="207px" ClientSettings-Scrolling-AllowScroll="true">
                <MasterTableView>                    
                    <Columns>
                        <telerik:GridBoundColumn AllowSorting="true" HeaderStyle-Font-Bold="true"  DataField="NAME" HeaderText="Credential Name"></telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageViewDeactivatedCredentials" runat="server">
            <telerik:RadGrid ID="RadGridDeactivatedCredential" runat="server" ClientSettings-Scrolling-UseStaticHeaders="true" Width="309px" AutoGenerateColumns="false" Height="207px" ClientSettings-Scrolling-AllowScroll="true">
                <MasterTableView>                    
                    <Columns>
                        <telerik:GridBoundColumn AllowSorting="true" HeaderStyle-Font-Bold="true"  DataField="NAME" HeaderText="Credential Name"></telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    <div style="float: left;">
        <telerik:RadTabStrip runat="server" ID="RadTabStrip2" Orientation="HorizontalBottom"
            SelectedIndex="0" MultiPageID="RadMultiPageCredentials" Skin="Thinkgate_Blue"
            EnableEmbeddedSkins="False">
            <Tabs>
                <telerik:RadTab Text="Active"  Style="padding-left: 0px !important; padding-right: 0px !important">
                </telerik:RadTab>
                <telerik:RadTab Text="Deactivated">
                </telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
    </div>
    <div id="addNewCredential" runat="server" class="searchTileBtnDiv_smallTile" title="Add New Credential" onclick="showAddNewCredentialWindow()">
        <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
        <div style="padding: 0;">
            Add New
        </div>
    </div>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="RadAjaxLoadingPanelCredential"
    runat="server" />
