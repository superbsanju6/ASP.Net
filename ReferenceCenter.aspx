<%@ Page Language="C#" MasterPageFile="~/Search.Master" AutoEventWireup="true" CodeBehind="ReferenceCenter.aspx.cs" Inherits="Thinkgate.ReferenceCenter" %>
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
                    <div id="btnAdd" runat="server" class="bottomTextButton" style="width: 88px; float: left; margin-left: 5px; margin-top: 11px; background-image: url('Images/add.gif');" title="Add Reference"
                        onclick='addReference();'>
                        <div style="padding-top: 1px; text-align: left;">Add Reference</div>
                    </div>

                </li>
            </ul>
        </div>
        <div style="width: 50%; text-align: right; float: right;">
            <asp:ImageButton ID="exportGridImgBtn" OnClick="exportGridImgBtn_Click" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
                Width="20px" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <!-- criteria -->
    <e3:DropDownList ID="cblUserGroup" CriteriaName="UserGroup" runat="server" Text="User Group" />
    <e3:DropDownList ID="cblCategory" CriteriaName="Category" runat="server" Text="Category" />
    <e3:DropDownList ID="cblComponent" CriteriaName="Component" runat="server" Text="Component" />
    <e3:DropDownList ID="cblType" CriteriaName="Type" runat="server" Text="Type" />
    <e3:DropDownList ID="cblFormat" CriteriaName="Format" runat="server" Text="Format" />
    <e3:TextWithDropDown ID="cblTextSearch" CriteriaName="TextSearch" runat="server" Text="Text Search" />
    <e3:DateRange ID="cblDateAdded" CriteriaName="DateAdded" runat="server" Text="Date Added" />

    <telerik:RadXmlHttpPanel runat="server" ID="reportListXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/ReportsWCF.svc"
        WcfServiceMethod="GetReportListByLevel" RenderMode="Block" OnClientResponseEnding="loadReportList">
    </telerik:RadXmlHttpPanel>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">

	<script type="text/javascript">
		function GridCreated(sender, args) {
			var scrollArea = sender.GridDataDiv;
			var parent = $get("ctl00_ctl00_AjaxPanelResultsPanel");
			var gridHeader = sender.GridHeaderDiv;

			var theHeight = (parent.clientHeight - gridHeader.clientHeight) - 100;
			$("#ctl00_RightColumnContentPlaceHolder_radGridResults").height(theHeight);
			$("#ctl00_RightColumnContentPlaceHolder_radGridResults_GridData").height(theHeight - 75);
		}
	</script>

     <input type="hidden" runat="server" id="classId" clientidmode="Static" value="" />
    <input type="hidden" runat="server" id="parentNodeId" clientidmode="Static" value="" />
    <input type="hidden" runat="server" id="clientName" clientidmode="Static" value="" />
     <input type="hidden" runat="server" id="stateInitial" clientidmode="Static" value="" />
    <div runat="server" clientidmode="Static" id="initMessage" style="text-align: center; height: 300px; padding-top: 15%; font-size: 14pt;">
        <span style="font-weight: bold;">Please select criteria</span><br />
    </div>
		<asp:Panel runat="server" ID="gridResultsPanel" Visible="false">
			<telerik:RadGrid 
					runat="server" 
					ID="radGridResults" 
					AutoGenerateColumns="False" 
					Width="100%"
					Height="520px"
					AllowFilteringByColumn="False" 
					PageSize="1" 
					AllowPaging="True" 
					AllowSorting="True" 
					AllowMultiRowSelection="true" 
					CssClass="assessmentSearchHeader" 
					OnItemDataBound="radGridResults_ItemDataBound" 
					OnItemCommand="radGridResults_ItemCommand" 
					OnSortCommand="radGridResults_SortCommand" 
					Skin="Web20"
					OnPageIndexChanged="RadGridResults_PageIndexChanged">
				<PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
				<ClientSettings EnableRowHoverStyle="true">
					<Selecting AllowRowSelect="True" UseClientSelectColumnOnly="true" />
					<ClientEvents OnGridCreated="GridCreated" />
					<Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True"></Scrolling>
				</ClientSettings>

				<MasterTableView TableLayout="Fixed" PageSize="20">
                   
					<Columns>                        
						<telerik:GridTemplateColumn HeaderText="Edit" UniqueName="EditButton" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
							<ItemTemplate>
								<a href='#' onclick="editReference(<%#Eval("DocumentNodeID")%>)">
									<img src="Images/Edit.png" alt="" title="Edit Document"></a>
							</ItemTemplate>
						</telerik:GridTemplateColumn>
						<telerik:GridTemplateColumn HeaderText="Delete" UniqueName="DeleteButton" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
							<ItemTemplate>
								<asp:ImageButton ID="DeleteDocument" runat="server" title="Delete Document" ImageUrl="~/Images/cross.gif" CommandName="ReferenceDelete" CommandArgument='<%#Eval("DocumentID") %>' OnClientClick="return confirm('Are you sure you want to delete this reference?');" />
							</ItemTemplate>
						</telerik:GridTemplateColumn>
						<telerik:GridTemplateColumn HeaderText="Title" SortExpression="Title" UniqueName="Title">
							<ItemTemplate>
								<a hreflang="#" onclick="viewReference('<%#Eval("NodeAlias")%>')">
									<u><span style="cursor: pointer; color: #00F;" title='<%#Eval("Title")%>'><%#Eval("Title")%></span></u>
								</a>
							</ItemTemplate>
						</telerik:GridTemplateColumn>
						<telerik:GridBoundColumn DataField="UserGroup" HeaderText="User Group" ShowSortIcon="true" 
							ItemStyle-Font-Size="Small" />
						<telerik:GridBoundColumn DataField="Component" HeaderText="Component" ShowSortIcon="true" 
							ItemStyle-Font-Size="Small" />
						<telerik:GridBoundColumn DataField="CategoryList" HeaderText="Category" ShowSortIcon="true" 
							ItemStyle-Font-Size="Small" />
						<telerik:GridBoundColumn DataField="Format" HeaderText="Format" ShowSortIcon="true" ItemStyle-Font-Size="Small"
							/>
						<telerik:GridBoundColumn DataField="FileTypes" HeaderText="Type" ShowSortIcon="true" ItemStyle-Font-Size="Small" 
							 />
						<telerik:GridBoundColumn DataField="DateAdded" HeaderText="Date Added" ShowSortIcon="true" ItemStyle-Font-Size="Small" 
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
        .RadTreeList_Transparent .rtlR td[class*="noShade"] {
            background-image: none;
        }

        .RadTreeList_Transparent .rtlA td[class*="noShade"] {
            background-image: none;
        }

        .rbl input[type="radio"] {
            margin-left: 10px;
            margin-right: 1px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
<script type="text/javascript">
    function loadReportList(sender, args) {
    }

    function addReference() {

        var kenticourl = '../<%:KenticoHelper.KenticoVirtualFolder%>/CMSModules/Thinkgate/MessageCenter/ReferenceCenterEdit.aspx?action=new&classid=' + $('#classId').val() + '&parentnodeid=' + $('#parentNodeId').val() + '&parentculture=en-US' + '&clientName=' + $('#clientName').val() + '&isThinkgateSession=true' + '&State=' + $('#stateInitial').val();
    	    customDialog({ url: (kenticourl), autoSize: true, name: 'AddReference', title: 'Add Reference' });
    	}

    	function editReference(docid) {

    	    var kenticourl = '../<%:KenticoHelper.KenticoVirtualFolder%>/CMSModules/Thinkgate/MessageCenter/ReferenceCenteredit.aspx?nodeid=' + docid + '&culture=en-US' + '&clientName=' + $('#clientName').val() + '&isThinkgateSession=true' + '&State=' + $('#stateInitial').val();
    	    customDialog({ url: (kenticourl), autoSize: true, name: 'AddReference', title: 'Add Reference' });
    	}

    	function viewReference(title) {
    	    var titlebreak = title.split(' ');
    	    for (var i = 0; i < titlebreak.length - 1; i++) {
    	        title = title.replace(' ', '-');
    	    }

    	    var baseUrl = window.location.protocol + '//' + window.location.hostname;
    	    var kenticourl = baseUrl + '/<%:KenticoHelper.KenticoVirtualFolder%>/ReferenceCenter/' + title + '.aspx?isThinkgateSession=true';
    		customDialog({ url: (kenticourl), name: 'AddReference', title: 'Add Reference', maximize: true, maxwidth: 450, maxheight: 370 });
        }
    </script>
</asp:Content>
