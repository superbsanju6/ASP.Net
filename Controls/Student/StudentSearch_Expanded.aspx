<%@ Page Title="Student Search" Language="C#" MasterPageFile="~/SearchExpanded.Master"
    AutoEventWireup="True" CodeBehind="StudentSearch_Expanded.aspx.cs" Inherits="Thinkgate.Controls.Student.StudentSearch_Expanded" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <div style="width: 100%; text-align: right;">
        <asp:ImageButton ID="btnStudentRosterExport" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png" OnClick="btnExport_Click" Width="20px" />
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <%--Changes made for BugId# 21093 to remove the bottom space --%>
     <style type="text/css">
		.rgDataDiv
		{			
			 height: 530px !important;
		}

    </style>
    <table width="99.5%">
        <tr>
            <td>
                <asp:Label ID="lblSearchResultCount" CssClass="searchCarouselLabel" runat="server"
                    Text="" />
            </td>
            <td style="float: right;">
                <asp:ImageButton runat="server" ToolTip="Grid View" ImageUrl="~/Images/list_view.png"
                    ID="imgGrid" Width="22px" Height="15px" Visible="false" />
                <asp:ImageButton runat="server" ToolTip="Tile View" ImageUrl="~/Images/graphical_view.png"
                    ID="imgIcon" Width="22px" Height="15px" Visible="false" />
            </td>
        </tr>
    </table>
    <asp:Panel runat="server" ID="gridResultsPanel">
        <telerik:RadGrid runat="server" ID="radGridResults" AutoGenerateColumns="False" Width="99.5%"
            AllowFilteringByColumn="False" PageSize="20" AllowPaging="True" AllowSorting="True"
            OnPageIndexChanged="RadGridResults_PageIndexChanged" Skin="Web20" CssClass="assessmentSearchHeader"
            onitemdatabound="radGridResults_ItemDataBound" Height="600px" OnSortCommand="OnSortCommand">
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="false"></PagerStyle>
            <MasterTableView TableLayout="Auto" Height="32px" AllowSorting="true">
                <Columns>
                    <telerik:GridHyperLinkColumn DataTextField="StudentName" UniqueName="ViewHyperLinkColumn" HeaderText="Name" Target="_blank" SortExpression="StudentName" />
                    <telerik:GridBoundColumn DataField="StudentID" HeaderText="Student ID" DataType="System.Int32" />
                    <telerik:GridBoundColumn DataField="FormattedGrade" HeaderText="Grade" />
                    <telerik:GridBoundColumn DataField="SchoolName" HeaderText="School" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
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
    <asp:TextBox ID="selectedStudentInput" runat="server" ClientIDMode="Static" Style="display: none;" />
    <asp:Button ID="gotoStudentBtn" runat="server" OnClick="GotoStudentBtnClick" ClientIDMode="Static"
        Style="display: none;" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
    <asp:TextBox ID="hiddenTextBox" runat="server" Style="visibility: hidden; display: none;" />
</asp:Content>
