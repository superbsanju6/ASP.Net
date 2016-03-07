<%@ Page Title="" Language="C#" MasterPageFile="~/AddNew.Master" AutoEventWireup="true" CodeBehind="CredentialTrackingStudentCount.aspx.cs" Inherits="Thinkgate.Controls.Reports.CredentialTrackingStudentCount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table width="100%;">
        <tr>
            <td class="fieldLabel1" style="padding-top: 10px;padding-bottom:15px;">
                <span style="font-weight: bold; padding-right: 10px;">Student Count:</span>
                <asp:Label runat="server" ID="lblStudentCount"></asp:Label>
                
            </td>
            <td>
                <asp:ImageButton ID="btnPrintBtn" runat="server" OnClick="btnPrintBtn_Click" ImageUrl="~/Images/Toolbars/print.png" Style="padding-right: 20px; float: right;" />
            </td>
        </tr>
    </table>

    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="expandStudentCountLoadingPanel" >
        <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
            <telerik:RadGrid ID="radGridStudentCount" runat="server" AutoGenerateColumns="false" Width="100%" Height="445px" AllowAutomaticUpdates="True"
                OnItemCommand="radGridStudentCount_ItemCommand">
                <ClientSettings EnableRowHoverStyle="true">
                    <Selecting AllowRowSelect="false" />
                    <Scrolling AllowScroll="True" SaveScrollPosition="True"
                        UseStaticHeaders="True" />
                    <Resizing AllowColumnResize="True" />
                </ClientSettings>
                <HeaderStyle Font-Bold="true" CssClass="gridcolumnheaderstyle" />
                <MasterTableView>
                    <Columns>
                        <telerik:GridBoundColumn DataField="StudentID" Display="false" ReadOnly="True" UniqueName="LevelID" />
                        <telerik:GridBoundColumn DataField="CredentialID" Display="false" ReadOnly="True" UniqueName="LevelID" />
                        <telerik:GridBoundColumn HeaderText="Student Name" DataField="StudentName" />
                        <telerik:GridBoundColumn HeaderText="Date Earned" DataField="DateEarned" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                        <telerik:GridBoundColumn HeaderText="Expiration Date" DataField="ExpirationDate" DataFormatString="{0:MM/dd/yyyy}"  ItemStyle-HorizontalAlign="Center"  HeaderStyle-HorizontalAlign="Center" />
                        <telerik:GridBoundColumn HeaderText="Recorded By" DataField="RecordedBy"  ItemStyle-HorizontalAlign="Center"  HeaderStyle-HorizontalAlign="Center" />
                        <telerik:GridTemplateColumn HeaderText="Comments" ItemStyle-HorizontalAlign="center"  HeaderStyle-HorizontalAlign="Center"  ItemStyle-Wrap="false" HeaderStyle-Wrap="false"
                            ItemStyle-Width="70px" HeaderStyle-Width="100px" ItemStyle-CssClass="gridcolumnstyle">
                            <ItemTemplate>
                              <%--  <asp:LinkButton ID="lnkbStudentCount" runat="server" Text='<%# Convert.ToInt32(Eval("CommentsCount")) > 0?"View":"" %>'
                                    CommandName="ViewComments" CommandArgument='<%# Eval("StudentID") + ","+ Eval("CredentialID") %>'>
                                </asp:LinkButton>--%>

                                <a href="#" id="lnkComment" onclick='<%# string.Format("openComments(this, {0}, {1}, {2})", HttpUtility.UrlEncode("0"), 
                                                HttpUtility.UrlEncode(Eval("StudentID").ToString()),
                                                HttpUtility.UrlEncode(Eval("CredentialID").ToString())
                                                ) %>'                                                    
                                                    runat="server"
                                                    >
                                   <%# Convert.ToInt32(Eval("CommentsCount")) > 0?"View":"" %>

                                </a>

                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </asp:Panel>
        <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static">
            <asp:Label runat="server" ID="lblResultMessage" Text="" CssClass="resultPanel" />
        </asp:Panel>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="expandStudentCountLoadingPanel" runat="server" />
    <script type="text/javascript">

        function openComments(sender ,studCrdId, studId, crdId) {
            customDialog({
                url: '../Credentials/StudentCredentialComment.aspx?studCrdId=' + studCrdId + '&studId=' + studId + '&crdId=' + crdId+'&isReport=yes', maximize: true, maxwidth: 1300, maxheight: 1100, title: 'Comments'});
        }
    </script>
</asp:Content>
