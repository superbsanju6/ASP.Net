<%@ Page Title="Edit Credentials List" Language="C#" MasterPageFile="~/AddNew.Master" AutoEventWireup="true" CodeBehind="EditCredentials.aspx.cs" Inherits="Thinkgate.Controls.Credentials.EditCredentials" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <telerik:RadCodeBlock ID="JsCodeEditCredential" runat="server">
        <script type="text/javascript">
            function openEditCredentials(credentialId) {
                customDialog({ url: '<%= this.ResolveUrl("~/Controls/Credentials/AddCredential.aspx") %>' + '?crdId=' + credentialId, title: "Edit Credential", maximize: true, maxwidth: 500, maxheight: 500, onClosing: function () { $('[id$="btnHiddenBind"]').click(); } });
            }

            function EntryAdded(sender, args) {
                var dropDown = sender;
                if (dropDown) {
                    setTimeout(function () {
                        dropDown.closeDropDown();
                    <%= this.ClientScript.GetPostBackEventReference(this.ddlAlignments, "OnClientEntryAdded") %>
                    }, 100);
                }
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div>
        <table width="100%;">
            <tr>
                <td class="fieldLabel1" style="padding-top: 10px;"></td>
                <td colspan="2" style="float: right; padding-right: 10px;">
                    <asp:ImageButton ID="ImageButton1" runat="server" OnClick="btnExportExcel_Click" ImageUrl="~/Images/excel_icon.png" Style="width: 25px; height: 25px;" />
                    <asp:ImageButton ID="btnPrintBtn" runat="server" OnClick="btnPrintBtn_Click" ImageUrl="~/Images/Toolbars/print.png" Style="width: 25px; height: 25px;" />
                </td>
            </tr>
        </table>
        <div style="padding-bottom: 30px;"></div>
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="editCredentialLoadingPanel">
            <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
                <table class="fieldValueTable fieldAddModalTable" style="width: 98%; margin-left: auto; margin-right: auto;">
                    <tr>
                        <td style="padding-left: 10px; padding-bottom: 5px;">
                            <span style="font-weight: bold; margin-left: 7px; margin-right: 5px;">List:</span>
                            <telerik:RadComboBox runat="server" ID="ddlEarned"
                                OnSelectedIndexChanged="ddlEarned_SelectedIndexChanged" ClientIDMode="AutoID" Skin="Web20"
                                Text="" CssClass="radDropdownBtn" Width="100" AutoPostBack="true">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Active" Value="Active" />
                                    <telerik:RadComboBoxItem Text="Deactivated" Value="Deactivated" />
                                </Items>
                            </telerik:RadComboBox>
                            <span runat="server" id="spnAlignment">
                                <span style="font-weight: bold; margin-left: 35px; margin-right: 7px;">Alignment:</span>
                                <telerik:RadDropDownTree runat="server" ID="ddlAlignments" DataTextField="CredentialAlignment" DataFieldParentID="ParentID"
                                    DataFieldID="ID" OnDataBound="ddlAlignments_DataBound" OnNodeDataBound="ddlAlignments_NodeDataBound"
                                    DataValueField="ID" Width="300" OnClientEntryAdded="EntryAdded" OnEntryAdded="ddlAlignments_EntryAdded">
                                    <DropDownSettings OpenDropDownOnLoad="false" Height="280" />
                                </telerik:RadDropDownTree>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel1" style="text-align: center;">
                            <telerik:RadGrid ID="gridCredentials" runat="server" AutoGenerateColumns="false" Width="98%" Height="465px" AllowAutomaticUpdates="True"
                                OnItemDataBound="gridCredentials_ItemDataBound" MasterTableView-DataKeyNames="ID" OnPreRender="gridCredentials_PreRender">
                                <ClientSettings EnableRowHoverStyle="true">
                                    <Selecting AllowRowSelect="false" />
                                    <Scrolling AllowScroll="True" SaveScrollPosition="True"
                                        UseStaticHeaders="True" />
                                    <Resizing AllowColumnResize="True" />
                                </ClientSettings>
                                <HeaderStyle Font-Bold="true" CssClass="gridcolumnheaderstyle" />
                                <MasterTableView>
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="ID" Display="false" ReadOnly="True" />
                                        <telerik:GridTemplateColumn HeaderText="Edit" DataField="Value" ItemStyle-Width="50px" HeaderStyle-Width="50px"
                                            ItemStyle-CssClass="gridcolumnstyleleft" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <img src="../../Images/Edit.png" style="cursor: pointer;vertical-align:middle;" onclick='<%# string.Format("openEditCredentials({0})", HttpUtility.UrlEncode(Eval("ID").ToString())) %>'
                                                    alt="Edit credential details" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Credential Name" ItemStyle-HorizontalAlign="Left" 
                                            ItemStyle-Width="350px" HeaderStyle-Width="400px" ItemStyle-CssClass="gridtopcolumnstyle">
                                            <ItemTemplate>
                                                <span runat="server" id="spanCredentialName">
                                                    <%# Eval("Name") %>
                                                </span>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Alignment" ItemStyle-HorizontalAlign="Left" UniqueName="Alignment"
                                            ItemStyle-CssClass="gridtopcolumnstyle"
                                            ItemStyle-Width="350px" HeaderStyle-Width="200px">
                                            <ItemTemplate>
                                                <span runat="server" id="spanAlignment"></span>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; padding-top: 30px;">
                            <telerik:RadButton runat="server" ID="RadButtonCancel" Text="Close" AutoPostBack="False" OnClientClicked="closeWindow" ClientIDMode="Static" />
                        </td>
                    </tr>
                </table>
                <button id="btnHiddenBind" style="display: none" runat="server" onserverclick="btnHiddenBind_ServerClick"></button>
            </asp:Panel>
            <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static">
                <asp:Label runat="server" ID="lblResultMessage" Text="" CssClass="resultPanel" />
                <br />
                <telerik:RadButton runat="server" ID="RadButton1" Text="Close" AutoPostBack="False" CssClass="resultPanel" OnClientClicked="closeWindow" />
            </asp:Panel>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="editCredentialLoadingPanel" runat="server" />

    </div>
    <script type="text/javascript">
        $(function () {
            setControlCSS();
        });

        function setControlCSS()
        {
            $('[id$="gridCredentials_GridData"]').css({ 'height': '440px' });
            $('.gridcolumnstyle').css({ 'vertical-align': 'middle' });
            $('.gridcolumnstyleleft').css({ 'vertical-align': 'middle', 'border-left': 'none' });
            $('.gridtopcolumnstyle').css({ 'vertical-align': 'top' });
        }

        function showNoResultPopUp() {
            alert("No records found to export to excel");
        }

        function closeWindow() {
            var oWnd = getCurrentCustomDialog();
            setTimeout(function () {
                oWnd.close();
            }, 100)
            if (window.top.reloadPlanningCredentialsTile) {
                window.top.reloadPlanningCredentialsTile();
            }
        }
    </script>
</asp:Content>
