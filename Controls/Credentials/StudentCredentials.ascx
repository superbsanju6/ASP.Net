<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentCredentials.ascx.cs" Inherits="Thinkgate.Controls.Credentials.StudentCredentials" %>

<telerik:RadCodeBlock ID="jsCredentialList" runat="server">
    <script type="text/javascript">
        function reloadStudentCredentialsTile() {
            <%= this.Page.ClientScript.GetPostBackEventReference(btnPostbackTargetSTC, "StudentTileRefresh") %>
        }
    </script>
</telerik:RadCodeBlock>
<telerik:RadAjaxPanel runat="server" ID="studentCredentialPanel" LoadingPanelID="stdentCredentailLoadingPanel">
    <asp:Button ID="btnPostbackTargetSTC" runat="server" OnClick="btnPostbackTargetSTC_Click" style="display: none;" />
    <telerik:RadMultiPage runat="server" ID="studentCredentialsRadMultiPage" SelectedIndex="0"
        Height="210px" Width="300px" CssClass="multiPage">
        <telerik:RadPageView ID="studentCredentialPageView" runat="server">
            <telerik:RadGrid ID="studentCredentialGrid" runat="server" Width="308px" AutoGenerateColumns="false" Height="231px" ClientSettings-Scrolling-AllowScroll="true">
                <MasterTableView>
                    <ItemStyle VerticalAlign="Top" />                    
                    <Columns>                                                
                        <telerik:GridBoundColumn DataField="CredentialName" HeaderText="Credential Name" ItemStyle-VerticalAlign="Top"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EarnedDate" HeaderText="Earned"  DataFormatString="{0:MM/dd/yy}"  ItemStyle-VerticalAlign="Top"></telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="stdentCredentailLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>
