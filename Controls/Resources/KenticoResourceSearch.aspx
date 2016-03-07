<%@ Page Title="Resource Search" Language="C#" AutoEventWireup="True" CodeBehind="KenticoResourceSearch.aspx.cs" Inherits="Thinkgate.Controls.Resources.KenticoResourceSearch" MasterPageFile="~/Search.Master" %>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="Curriculum" Src="~/Controls/E3Criteria/Associations/Curriculum.ascx" %>
<%@ Register TagPrefix="e3" TagName="Classes" Src="~/Controls/E3Criteria/Associations/Classes.ascx" %>
<%@ Register TagPrefix="e3" TagName="Text" Src="~/Controls/E3Criteria/Text.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextWithDropdown" Src="~/Controls/E3Criteria/TextWithDropdown.ascx" %>
<%@ Register TagPrefix="e3" TagName="Documents" Src="~/Controls/E3Criteria/Associations/Documents.ascx" %>
<%@ Register TagPrefix="e3" TagName="Schools" Src="~/Controls/E3Criteria/Associations/Schools.ascx" %>
<%@ Register TagPrefix="e3" TagName="Standards" Src="~/Controls/E3Criteria/Associations/Standards.ascx" %>
<%@ Register TagPrefix="e3" TagName="Students" Src="~/Controls/E3Criteria/Associations/Students.ascx" %>
<%@ Register TagPrefix="e3" TagName="Teachers" Src="~/Controls/E3Criteria/Associations/Teachers.ascx" %>
<%@ Register TagPrefix="e3" TagName="ResourceCategoryTypeSubType" Src="~/Controls/E3Criteria/ResourceCategoryTypeSubType.ascx" %>
<%@ MasterType VirtualPath="~/Search.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
	<e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
	<e3:ResourceCategoryTypeSubType ID="ctrlCategory" runat="server" CriteriaName="ResourceCategoryTypeSubType" />
	<e3:Text ID="txtResourceName" CriteriaName="ResourceName" Text="Resource Name" runat="server" />
	<e3:TextWithDropdown ID="txtSearch" CriteriaName="TextSearch" Text="Text Search" runat="server" DataTextField="Name" Visible="True" />
	<table style="margin-left: auto; margin-right: auto">
		<tr>
			<td style="font-weight: bold; font-size: 12pt; text-decoration: underline; padding-right: 20px;">Associations</td>
			<td>
				<telerik:RadButton ID="btnClear" runat="server" Text="Clear" ToolTip="Clear report criteria"
					Skin="Web20" Style="font-weight: bold; margin-top: 5px; margin-bottom: 5px;" AutoPostBack="False" OnClientClicked="AssociationsClear">
				</telerik:RadButton>
			</td>
		</tr>
	</table>
	<e3:Curriculum ID="ctrlCurriculum" CriteriaName="Curriculum" Text="Curriculum" runat="server" />
	<e3:Standards ID="ctrlStandards" CriteriaName="Standards" Text="Standards" runat="server" />
	<e3:Classes ID="ctrlClasses" CriteriaName="Classes" Text="Classes" runat="server" Visible="false" />
	<e3:Documents ID="ctrlDocuments" CriteriaName="Documents" Text="Documents" runat="server" Visible="false" />
	<e3:Schools ID="ctrlSchools" CriteriaName="Schools" Text="Schools" runat="server" Visible="false" />
	<e3:Students ID="ctrlStudents" CriteriaName="Students" Text="Students" runat="server" Visible="false" />
	<e3:Teachers ID="ctrlTeachers" CriteriaName="Teachers" Text="Teachers" runat="server" Visible="false" />
	<script type="text/javascript">

		function AssociationsRemoveAll() {
			CriteriaController.RemoveAll("Standards");
			CriteriaController.RemoveAll("Curriculum");
			CriteriaController.RemoveAll("Classes");
			CriteriaController.RemoveAll("Documents");
			CriteriaController.RemoveAll("Schools");
			CriteriaController.RemoveAll("Students");
			CriteriaController.RemoveAll("Teachers");
		}

		function AssociationsClear() {
			AssociationsRemoveAll();
			StandardsController.Clear();
			CurriculumController.Clear();
			ClassesController.Clear();
			DocumentsController.Clear();
			SchoolsController.Clear();
			StudentsController.Clear();
			TeachersController.Clear();
		}
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
	<asp:Panel runat="server" ID="gridResultsPanel">
		<telerik:RadGrid runat="server" ID="radGridResults" AutoGenerateColumns="False" Width="100%"
			AllowFilteringByColumn="False" PageSize="20" AllowPaging="True" AllowSorting="True" Height="600"
			OnPageIndexChanged="RadGridResults_PageIndexChanged" AllowMultiRowSelection="true"
			OnItemDataBound="radGridResults_ItemDataBound" OnSortCommand="OnSortCommand" CssClass="assessmentSearchHeader" Skin="Web20" OnNeedDataSource="RadGrid_NeedDataSource">
			<PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
			<ClientSettings EnableRowHoverStyle="true">
				<Selecting AllowRowSelect="True" UseClientSelectColumnOnly="true" />
				<ClientEvents OnGridCreated="changeGridHeaderWidth" />
				<Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True" ScrollHeight="460px"></Scrolling>
			</ClientSettings>
			<MasterTableView TableLayout="Auto" PageSize="100">
				<Columns>
					<telerik:GridTemplateColumn HeaderText="Name" DataField="ResourceName" UniqueName="Name"
						ItemStyle-Font-Size="Small">
						<ItemTemplate>
							<label runat="server" id="lblName"></label>
							<a runat="server" id="hlName" href="" onclick=""></a>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="View" DataField="ViewLink" ItemStyle-Font-Size="Small" HeaderStyle-Width="50" ItemStyle-HorizontalAlign="Center">
						<ItemTemplate>
							<a runat="server" href="" id="hlView" target="_blank">
								<asp:Image runat="server" ImageUrl="../Images/ViewPage.png" Width="15" /></a>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridBoundColumn DataField="Type" HeaderText="Type" ShowSortIcon="true"
						ItemStyle-Font-Size="Small" />
					<telerik:GridBoundColumn DataField="Subtype" HeaderText="Subtype" ShowSortIcon="true"
						ItemStyle-Font-Size="Small" />
					<telerik:GridBoundColumn DataField="Description" HeaderText="Description" ShowSortIcon="true"
						ItemStyle-Font-Size="Small" />

				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
		<asp:PlaceHolder ID="initialDisplayText" runat="server"></asp:PlaceHolder>
	</asp:Panel>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
	<script type="text/javascript">
		function alterPageComponents(state) {

			$('.rgHeaderDiv').each(function () {
				this.style.width = '';
			});

		}

		window.onload = function () {
			alterPageComponents('moot');
		};


		function openResource(id) {
            //TODO: Add Logic here after determining what behavior should accompany the resource name in the grid.
		    return;
		    //window.open('<%=ResolveUrl("~/SessionBridge.aspx?ReturnURL=") %>' + escape("display.asp?key=7266&fo=basic display&rm=page&xID=" + id + "&??hideButtons=Save And Return&??appName=E3"));
		}

	</script>
</asp:Content>



<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
	<style type="text/css">
		#rightColumn
		{
			background-color: white;
		}
	</style>
</asp:Content>
