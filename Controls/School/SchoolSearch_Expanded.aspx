<%@ Page Title="School Search" Language="C#" MasterPageFile="~/SearchExpanded.Master"
    AutoEventWireup="true" CodeBehind="SchoolSearch_Expanded.aspx.cs" Inherits="Thinkgate.Controls.School.SchoolSearch_Expanded" %>

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

    <div style="width: 100%;">
        <div class="listWrapper" style="width: 50%; text-align: left; float: left;">
            <ul>
            </ul>
        </div>
        <div style="width: 50%; text-align: right; float: right;">
            <asp:ImageButton ID="exportGridImgBtn" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
                OnClick="ExportGridImgBtn_Click" Width="20px" />
        </div>
    </div>
    <div style="bottom: 0px; left: 0px; height: 14.72px; width: 300px;">
        <div style="font-weight: bold; float: left;" id="selectedRecordsDiv">
        </div>
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
    <table width="100%">
        <tr>      
            <td style="float: right;">
                <!-- INVISIBLE -- FUTURE USE -->
                <asp:ImageButton runat="server" ToolTip="Grid View" ImageUrl="~/Images/list_view.png"
                    ID="imgGrid" OnClick="ImageGrid_Click" Width="22px" Height="15px" Visible="false" />
                <!-- INVISIBLE -- FUTURE USE -->
                <asp:ImageButton runat="server" ToolTip="Tile View" ImageUrl="~/Images/graphical_view.png"
                    ID="imgIcon" OnClick="ImageIcon_Click" Width="22px" Height="15px" Visible="false" />
            </td>
        </tr>
    </table>

    <asp:Panel runat="server" ID="gridResultsPanel">
        <telerik:RadGrid runat="server" ID="radGridResults" AutoGenerateColumns="False" Width="100%"
            AllowFilteringByColumn="False" PageSize="20" AllowPaging="True" AllowSorting="True" Height="780px"
            OnPageIndexChanged="RadGridResults_PageIndexChanged" AllowMultiRowSelection="true"
            OnItemDataBound="radGridResults_ItemDataBound" OnSortCommand="OnSortCommand"  CssClass="assessmentSearchHeader" Skin="Web20">
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="false"></PagerStyle>
            <ClientSettings EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="True" UseClientSelectColumnOnly="true" />
                <ClientEvents  OnRowSelected="RowSelected" OnRowDeselected="RowDeselected" OnGridCreated="changeGridHeaderWidth" />
                <Scrolling AllowScroll="true" UseStaticHeaders="True"  SaveScrollPosition="True"></Scrolling>
            </ClientSettings>
            <MasterTableView TableLayout="Auto" Width="858px" PageSize="100" Height="32px">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Name" DataField="Name" ShowSortIcon="true" SortExpression="Name"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="160px">
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkListSchoolName" runat="server" NavigateUrl="" Target="_blank"
                                Visible="True"><%# Eval("Name")%></asp:HyperLink>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Abbreviation" HeaderText="Abbreviation" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="70px" />
                    <telerik:GridBoundColumn DataField="Type" HeaderText="Type" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="70px" />
                    <telerik:GridBoundColumn DataField="Cluster" HeaderText="Cluster" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="100px" />
                    <telerik:GridBoundColumn DataField="SchoolID" HeaderText="School ID" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="60px" />
                    <telerik:GridBoundColumn DataField="Phone" HeaderText="Phone" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="80px" />
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
    <asp:TextBox ID="hiddenTextBox" runat="server" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenGuidBox" runat="server" Style="visibility: hidden; display: none;" />
</asp:Content>
