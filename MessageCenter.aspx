<%@ Page Title="" Language="C#" MasterPageFile="~/Search.Master" AutoEventWireup="true" CodeBehind="MessageCenter.aspx.cs" Inherits="Thinkgate.MessageCenter" %>
<%@ Import Namespace="Thinkgate.Classes" %>

<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="DateRange" Src="~/Controls/E3Criteria/DateRange.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextWithDropDown" Src="~/Controls/E3Criteria/TextWithDropDown.ascx" %>
<%@ MasterType VirtualPath="~/Search.Master" %>



<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <div style="width: 100%; height: 72px;">
        <div class="listWrapper" style="width: 50%; height: 62px; text-align: left; float: left;">
            <ul>
                <li class="searchAction">
                    <div id="btnAdd" runat="server" class="bottomTextButton" style="width: 88px; float: left; margin-left: 5px; margin-top: 11px; background-image: url('Images/add.gif');" title="Add Message"
                        onclick='addMessage();'>
                        <div style="padding-top: 1px; text-align: left;">Add Message</div>
                    </div>

                </li>
            </ul>
        </div>
        <div style="width: 50%; text-align: right; float: right;">
            <asp:ImageButton ID="exportGridImgBtn" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
                OnClick="ExportGridImgBtn_Click" Width="20px" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <!-- criteria -->
    <e3:DropDownList ID="cblUserGroup" CriteriaName="UserGroup" runat="server" Text="User Group" />
    <e3:DropDownList ID="cblType" CriteriaName="Type" runat="server" Text="Type" />
    <e3:TextWithDropDown ID="cblTextSearch" CriteriaName="TextSearch" runat="server" Text="Text Search" />
    <e3:DateRange ID="cblDateAdded" CriteriaName="DateAdded" runat="server" Text="Date Added" />
    <e3:DateRange ID="cblPostOn" CriteriaName="PostOn" runat="server" Text="Post On Date" />
    <e3:DateRange ID="cblRemoveOn" CriteriaName="RemoveOn" runat="server" Text="Remove On Date" />

    <telerik:RadXmlHttpPanel runat="server" ID="reportListXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/ReportsWCF.svc"
        WcfServiceMethod="GetReportListByLevel" RenderMode="Block" OnClientResponseEnding="loadReportList">
    </telerik:RadXmlHttpPanel>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <input type="hidden" runat="server" id="lockedTestIDs" clientidmode="Static" value="" />
    <input type="hidden" runat="server" id="portalType" clientidmode="Static" value="Teacher" />
    <input type="hidden" runat="server" id="yearList" clientidmode="Static" value="" />
    <input type="hidden" runat="server" id="districtYear" clientidmode="Static" value="" />
    <input type="hidden" runat="server" id="classId" clientidmode="Static" value="" />
    <input type="hidden" runat="server" id="parentNodeId" clientidmode="Static" value="" />
    <input type="hidden" runat="server" id="clientName" clientidmode="Static" value="" />
     <input type="hidden" runat="server" id="stateInitial" clientidmode="Static" value="" />
    <div runat="server" clientidmode="Static" id="initMessage" style="text-align: center; height: 300px; padding-top: 15%; font-size: 14pt;">
        <span style="font-weight: bold;">Please select criteria</span><br />
    </div>
    <asp:Panel runat="server" ID="gridResultsPanel" Visible="false">
        <telerik:RadGrid runat="server" ID="radGridResults" AutoGenerateColumns="False" Width="100%"
            AllowFilteringByColumn="False" PageSize="1" AllowPaging="True" OnPageIndexChanged="RadGridResults_PageIndexChanged"
            AllowSorting="True" AllowMultiRowSelection="true" MasterTableView-PageSize="1" Height="520px"
            CssClass="assessmentSearchHeader" OnItemDataBound="radGridResults_ItemDataBound" OnItemCommand="radGridResults_ItemCommand" OnSortCommand="radGridResults_SortCommand" Skin="Web20">                                            
            <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
            <ClientSettings EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="True" UseClientSelectColumnOnly="true" />
                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True" ScrollHeight="460px"></Scrolling>
            </ClientSettings>

            <MasterTableView TableLayout="Auto" PageSize="20">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Edit" UniqueName="EditButton" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                             <a   runat="server" id="EditDocumentImg"   >
                                <img src="Images/Edit.png" alt="" style="cursor:pointer;" title="Edit Document"></a>                           
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Delete" UniqueName="DeleteButton" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:ImageButton ID="DeleteDocument" runat="server" title="Delete Document" ImageUrl="~/Images/cross.gif" CommandName="MessageDelete" CommandArgument='<%#Eval("DocumentID") %>' OnClientClick="return confirm('Are you sure you want to delete this message?');" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Title" SortExpression="Title" ShowSortIcon="true" UniqueName="Title">
                        <ItemTemplate>
                            <a hreflang="#"  onclick="viewMessage('<%#Eval("DocumentNodeID")%>', '<%#Eval("Title")%>')">
                             <u>   <span style="cursor: pointer; color:#00F;" title='<%#Eval("Title")%>'><%#Eval("Title")%></span></u>

                            </a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="UserGroupEnum" HeaderText="User Group" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" />
                    <telerik:GridBoundColumn DataField="MessageCenterEnum" HeaderText="Type" ShowSortIcon="true" ItemStyle-Font-Size="Small" 
                        />
                    <telerik:GridBoundColumn DataField="DateAdded" HeaderText="Date Added" ShowSortIcon="true" ItemStyle-Font-Size="Small" />
                    <telerik:GridBoundColumn DataField="PostOn" HeaderText="Post On" ShowSortIcon="true" ItemStyle-Font-Size="Small"
                        />
                    <telerik:GridBoundColumn DataField="RemoveOn" HeaderText="Remove On" ShowSortIcon="true" ItemStyle-Font-Size="Small"
                         />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </asp:Panel>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .RadTreeList_Transparent .rtlR td[class*="noShade"]
        {
            background-image: none;
        }

        .RadTreeList_Transparent .rtlA td[class*="noShade"]
        {
            background-image: none;
        }

        .rbl input[type="radio"]
        {
            margin-left: 10px;
            margin-right: 1px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    
    <script type="text/javascript">
        function loadReportList(sender, args) {
        }

        function addMessage() {
            customDialog({ url: ('../<%:KenticoHelper.KenticoVirtualFolder%>/CMSModules/Thinkgate/MessageCenter/MessageCenterEdit.aspx?action=new&classid=' + $('#classId').val() + '&parentnodeid=' + $('#parentNodeId').val() + '&parentculture=en-US' + '&clientName=' + $('#clientName').val() + '&State=' + $('#stateInitial').val() + '&isThinkgateSession=true'), name: 'AddMessage', title: 'Add Message', width: 800, height: 550 });
        }

        function editMessage(docid) {            
            customDialog({ url: ('../<%:KenticoHelper.KenticoVirtualFolder%>/CMSModules/Thinkgate/MessageCenter/MessageCenterEdit.aspx?nodeid=' + docid + '&culture=en-US' + '&clientName=' + $('#clientName').val() + '&State=' + $('#stateInitial').val() + '&isThinkgateSession=true'), name: 'EditMessage', title: 'Edit Message', autoSize: true });
           
        }

        function viewMessage(nodeid, messageTitle) {
            parent.customDialog({ url: ('<%= Request.ApplicationPath%>/MessageCenterPopup.aspx?MessageCenterNodeID=' + nodeid), name: 'ViewMessage', title: messageTitle, maximize: true, maxwidth: 820, maxheight: 560 });
        }
	</script>
</asp:Content>
