<%@ Page Title="Staff Search" Language="C#" MasterPageFile="~/SearchExpanded.Master"
		AutoEventWireup="true" CodeBehind="TeacherSearch_Expanded.aspx.cs" Inherits="Thinkgate.Controls.Teacher.TeacherSearch_Expanded" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
 <script type="text/javascript">
     function alterPageComponents(state) {
         if (state == "open") {
             $('.rgHeaderDiv').each(function () {
                 this.style.width = '688px';
             });
         }
         else {
             $('.rgHeaderDiv').each(function () {
                 this.style.width = '888px';
             });
         }
     }

     function btnSelectReturn_Click() {
         var results = [];
         if ($find('<%=radGridResults.ClientID %>') != null) {
	        var MasterTable = $find('<%=radGridResults.ClientID %>').get_masterTableView();
	        var items = MasterTable.get_selectedItems();
	        for (var i = 0; i < items.length; i++) {
	            results.push(MasterTable.getCellByColumnUniqueName(items[i], "UserID").innerHTML);
	            if (i < items.length - 1) {
	                results.push(",");
	            }
	        }
	        if (results.length == 0) {
	            alert("No staff selected.");
	            //return;
	        }
	        var DialogueToSendResultsTo;
	        DialogueToSendResultsTo = parent.getCurrentCustomDialogByName('Add Teacher');
	        try {
	            DialogueToSendResultsTo.get_contentFrame().contentWindow.AddTeachers(results.join(''));
	        }
	        catch (e) {
	            try {
	                parent.AddTeachers(results.join(''));
	            }
	            catch (e) {
	            }
	        }
	        closeCurrentCustomDialog();
	    }
    }

 </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <script type="text/javascript">
        function getAllSchoolsFromSchoolTypes(result) {
            // School
            updateCriteriaControl('School', result.Schools, 'DropDownList', 'School');
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
		<asp:PlaceHolder runat="server" ID="criteraDisplayPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder"
		runat="server">
    <asp:Panel runat="server" DefaultButton="btnFake">
        <telerik:RadButton ID="btnFake" runat="server" AutoPostBack="false" ClientIDMode="Static" Width="0" Height="0"></telerik:RadButton>
		<div style="width: 100%;">
				<div class="listWrapper" style="width: 49%; text-align: left; float: left;">
                    <telerik:RadButton ID="btnSelectReturn" runat="server" Text="Select and Return" ToolTip="Add selected items to assessment"
                        Skin="Web20" AutoPostBack="false" OnClientClicked="btnSelectReturn_Click"
                        Style="font-weight: bold; margin: 5px; float: left;" Visible="False">
                    </telerik:RadButton>
				</div>
				<div style="width: 49%; text-align: right; float: right;height:30px;">
						<asp:ImageButton ID="exportGridImgBtn" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
								OnClick="ExportGridImgBtn_Click" Width="20px" />
				</div>
		</div>
		<div style="bottom: 0px; left: 0px; height: 14.72px; width: 300px;">
				<div style="font-weight: bold; float: left; " id="selectedRecordsDiv">
						</div>
		</div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
      <%--Changes made for BugId# 21093 to remove the bottom space --%>
    <style type="text/css">
		.rgDataDiv
		{			
			 height: 530px !important;
		}
    </style>
		<table width="100%">
				<tr>
						 <td style="float: right;">
								<!-- INVISIBLE -- FUTURE USE -->
								<asp:ImageButton runat="server" ToolTip="Grid View" ImageUrl="~/Images/list_view.png"
										ID="imgGrid" Width="22px" Height="15px" Visible="false" />
								<!-- INVISIBLE -- FUTURE USE -->
								<asp:ImageButton runat="server" ToolTip="Tile View" ImageUrl="~/Images/graphical_view.png"
										ID="imgIcon" Width="22px" Height="15px" Visible="false" />
						</td>
				</tr>
		</table>
		
		<asp:Panel runat="server" ID="gridResultsPanel">
				<telerik:RadGrid runat="server" ID="radGridResults" AutoGenerateColumns="False" Width="100%"
						AllowFilteringByColumn="False" PageSize="20" AllowPaging="True" AllowSorting="True"
						OnPageIndexChanged="RadGridResults_PageIndexChanged" AllowMultiRowSelection="true"
						OnItemDataBound="radGridResults_ItemDataBound" OnSortCommand="OnSortCommand" Height="601px" CssClass="assessmentSearchHeader" Skin="Web20">
						<PagerStyle Mode="NextPrevAndNumeric"  AlwaysVisible="false"></PagerStyle>
						<ClientSettings EnableRowHoverStyle="true">
								<Selecting AllowRowSelect="True" UseClientSelectColumnOnly="true" />
								<ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeselected" OnGridCreated="changeGridHeaderWidth" />
								<Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True">
								</Scrolling>
						</ClientSettings>
						<MasterTableView TableLayout="Auto" Width="100%" PageSize="100" Height="32px">
								<Columns>
                                        <telerik:GridClientSelectColumn HeaderText="Select" Visible="false" UniqueName="Select" HeaderStyle-Width="15px"></telerik:GridClientSelectColumn>
										<telerik:GridTemplateColumn HeaderText="Name" DataField="Name" ShowSortIcon="true" SortExpression="Name"
												ItemStyle-Font-Size="Small" HeaderStyle-Width="100px">
												<ItemTemplate>
														<asp:HyperLink ID="lnkListStaffName" runat="server" NavigateUrl="" Target="_blank"
																Visible="True"><%# Eval("Name")%></asp:HyperLink>
												</ItemTemplate>
										</telerik:GridTemplateColumn>
										<telerik:GridBoundColumn DataField="UserID" HeaderText="User ID" ShowSortIcon="true"
												ItemStyle-Font-Size="Small" HeaderStyle-Width="70px" />
										<telerik:GridBoundColumn DataField="UserType" HeaderText="User Type" ShowSortIcon="true"
												ItemStyle-Font-Size="Small" HeaderStyle-Width="50px" />
										<telerik:GridBoundColumn DataField="School" HeaderText="School" ShowSortIcon="true"
												ItemStyle-Font-Size="Small" HeaderStyle-Width="100px" />
								</Columns>
						</MasterTableView>
				</telerik:RadGrid>
				<asp:PlaceHolder ID="initialDisplayText" runat="server"></asp:PlaceHolder>
		</asp:Panel>
		<asp:Panel runat="server" ID="tileResultsPanel" Visible="False">
				<asp:Panel ID="buttonsContainer1" runat="server" ClientIDMode="Static" CssClass="pagingDivExpandedWindow">
				</asp:Panel>
				<div id="tileDivWrapper1" runat="server" class="searchWrapper">
						<div id="tileScrollDiv1" class="searchScroll">
								<div id="tileDiv1" runat="server" class="searchDiv">
								</div>
						</div>
				</div>
		</asp:Panel>
		<asp:TextBox ID="selectedSchoolInput" runat="server" ClientIDMode="Static" Style="display: none;" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>
