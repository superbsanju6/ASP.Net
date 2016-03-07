<%@ Page Title="Assessment Search" Language="C#" MasterPageFile="~/SearchExpanded.Master"
    AutoEventWireup="true" CodeBehind="AssessmentSearchExpanded.aspx.cs" Inherits="Thinkgate.Controls.Assessment.AssessmentSearchExpanded" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function alterPageComponents(state) {

            $('.rgHeaderDiv').each(function () {
                this.style.width = '';
            });

        }

        window.onload = function () {
            alterPageComponents('moot');
        }

    </script>
    <style type="text/css">
        .sort-btn{
            background: none;
            border: 0;
            color: white;
            font: 12px / 16px "segoe ui",arial,sans-serif;
            cursor:pointer;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder"
    runat="server">
    <script type="text/javascript">



        function getCurrGradesSubjectsAndCourses(result) {
            // Subject
            updateCriteriaControl('Subject', result.Subjects, 'CheckBoxList');

            // Course
            getCurrCoursesFromSubjects(result);
        }

        function getCurrCoursesFromSubjects(result) {
            // Course
            updateCriteriaControl('Curriculum', result.Courses, 'DropDownList', 'Course');
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <telerik:RadTabStrip ID="RadTabStripSearch" runat="server" MultiPageID="RadMultiPageSearch" SelectedIndex="0" Skin="Office2010Blue">
        <Tabs>
            <telerik:RadTab runat="server" TabIndex="0" Text="Basic" PageViewID="rpBasic"></telerik:RadTab>
            <telerik:RadTab runat="server" TabIndex="1" Text="Tags" PageViewID="rpLRMI" ></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage runat="server" ID="RadMultiPageSearch">
        <telerik:RadPageView runat="server" ID="rpBasic" Selected="True">
            <asp:PlaceHolder runat="server" ID="criteraDisplayPlaceHolder">                
            </asp:PlaceHolder>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="rpLRMI">
            <asp:PlaceHolder runat="server" ID="LRMISearchPlaceholder"></asp:PlaceHolder>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder"
    runat="server">
    <div style="width: 100%; height: 72px;">
        <div class="listWrapper" style="width: 50%; height: 62px; text-align: left; float: left;">
            <ul>
                <li class="searchAction">
                    <div runat="server" id="divBtnUnlock" clientidmode="Static" visible="True">
                        <telerik:RadButton ID="radBtnUnlock" ClientIDMode="Static" runat="server" Width="34px"
                            Height="34px" Enabled="false" OnClick="UnlockAssessments_Click">
                            <Image ImageUrl="~/Images/assessment/Unlock_Enabled.png" DisabledImageUrl="~/Images/assessment/Unlock_Disabled.png" />
                        </telerik:RadButton>
                        <br />
                        <a style="color: black;">Enable
                            <br />
                            Content</a>
                    </div>
                </li>
                <li class="searchAction">
                    <div runat="server" id="divBtnLock" clientidmode="Static" visible="True">
                        <telerik:RadButton ID="radBtnLock" ClientIDMode="Static" runat="server" Width="34px"
                            Height="34px" Enabled="false" OnClick="LockAssessments_Click">
                            <Image ImageUrl="~/Images/assessment/Lock_Enabled.png" DisabledImageUrl="~/Images/assessment/Lock_Disabled.png" />
                        </telerik:RadButton>
                        <br />
                        <a style="color: black;">Disable<br />
                            Content </a>
                    </div>
                </li>
                <li class="searchAction">
                    <div runat="server" id="divBtnActivate" clientidmode="Static" visible="True">
                        <telerik:RadButton ID="radBtnActivate" ClientIDMode="Static" runat="server" Width="34px"
                            Height="34px" Enabled="false" OnClick="ActivateAssessments_Click">
                            <Image ImageUrl="~/Images/assessment/Activate_Enabled.png" DisabledImageUrl="~/Images/assessment/Activate_Disabled.png" />
                        </telerik:RadButton>
                        <br />
                        <a style="color: black;">Enable Admin</a>
                    </div>
                </li>
                <li class="searchAction">
                    <div runat="server" id="divBtnDeactivate" clientidmode="Static" visible="True">
                        <telerik:RadButton ID="radBtnDeactivate" ClientIDMode="Static" runat="server" Width="34px"
                            Height="34px" Enabled="false" OnClick="DeactivateAssessments_Click">
                            <Image ImageUrl="~/Images/assessment/Deactivate_Enabled.png" DisabledImageUrl="~/Images/assessment/Deactivate_Disabled.png" />
                        </telerik:RadButton>
                        <br />
                        <a style="color: black;">Disable Admin</a>
                    </div>
                </li>
                <li class="searchAction">
                    <div runat="server" id="divBtnViewTestEvents" clientidmode="Static" visible="True">
                        <telerik:RadButton ID="radBtnViewTestEvents" ClientIDMode="Static" runat="server" Width="34px"
                            Height="34px" Enabled="false"  OnClick="ViewTestEvents_Click">
                            <Image ImageUrl="~/Images/assessment/Dashboard_Enabled.png" DisabledImageUrl="~/Images/assessment/Dashboard_Disabled.png" />
                        </telerik:RadButton>
                        <br />
                        <a style="color: black;">View Test Events</a>
                    </div>
                </li>
            </ul>
        </div>
        <div style="width: 50%; text-align: right; float: right;">
            <asp:ImageButton ID="exportGridImgBtn" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
                OnClick="ExportGridImgBtn_Click" Width="20px" />
        </div>
    </div>
    <div style="bottom: 0px; left: 0px; height: 14.72px; width: 300px;">
        <div style="font-weight: bold; float: left;" id="selectedRecordsDiv">
            <span id="lblSelectedCount" runat="server" clientidmode="Static"></span>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <table width="100%">
        <tr>
            <td>
                <asp:Label ID="lblSearchResultCount" CssClass="searchCarouselLabel" runat="server"
                    Text="" />
            </td>
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
            AllowFilteringByColumn="False" PageSize="20" AllowPaging="True" AllowSorting="True"
            OnPageIndexChanged="RadGridResults_PageIndexChanged" AllowMultiRowSelection="true"
            OnItemDataBound="radGridResults_ItemDataBound" OnSortCommand="OnSortCommand"  CssClass="assessmentSearchHeader" Skin="Web20" OnNeedDataSource="RadGrid_NeedDataSource">
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="false"  ></PagerStyle>
            <ClientSettings EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="True" UseClientSelectColumnOnly="true" />
                <ClientEvents OnRowSelecting="RowClicked" OnRowSelected="RowSelected" OnRowDeselecting="RowClicked" OnRowDeselected="RowDeselected" OnGridCreated="changeGridHeaderWidth" />
                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True" ScrollHeight="435px">
                </Scrolling>
            </ClientSettings>
            <MasterTableView TableLayout="Auto"  PageSize="100" >
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="" UniqueName="ButtonText" HeaderStyle-Width="30px">
                        <HeaderTemplate>
                            <input id="chkAll" runat="server" type="checkbox" clientidmode="Static" name="headerSelect" value="all" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <input id="chkRowInput" class="chkRow" runat="server" type="checkbox" name="rowSelect"
                                value='<%# Eval("EncryptedID")%>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="" UniqueName="ButtonText" HeaderStyle-Width="85px">
                        <ItemTemplate>
                            <asp:PlaceHolder ID="phButtons" runat="server">
                                <img id="imgGraphicPrint" runat="server" src="~/Images/Printer.png" alt="Print" title="Print"
                                    style="cursor: pointer; padding-right: 6px; padding-left: 2px;" onclick="alert('Missing event target')" />
                                <img alt="Administration" id="imgGraphicAdmin" runat="server" src="~/Images/dashboard_small.png"
                                    title="Administration" style="cursor: pointer; padding-right: 6px;" onclick="alert('Missing event target')" />
                                <img id="btnGraphicEdit1" runat="server" src="~/Images/Edit.png" alt="Edit" title="Edit"
                                    style="cursor: pointer;" />
                            </asp:PlaceHolder>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="TestName" UniqueName="Name"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="130px" ShowSortIcon="true">
                        <HeaderTemplate>
                             <asp:Button id="namesortbtn" runat="server"
                                  Text="Name"
                                 CommandName="Sort"
                                 CommandArgument="TestName" CssClass="sort-btn" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkListTestName" runat="server" NavigateUrl="" Target="_blank"
                                Visible="False"><%# Eval("TestName")%></asp:HyperLink>
                            <asp:Label runat="server" ID="lblListTestName" runat="server" Visible='False' Text='<%# Eval("TestName")%>'></asp:Label>
                            <div style="display: none;">
                                <telerik:RadTextBox ID="hiddenEncryptedID" runat="server" Value='<%# Eval("EncryptedID")%>'
                                    Style="display: none; width: 0px !important;" />
                            </div>
                            <asp:Image runat="server" ToolTip="Secure" ImageUrl="~/Images/IconSecure-lock.png" ID="imgIconSecure" Width="22px" Height="15px" Visible="false" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Description" HeaderText="Description" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="245px" />
                    <telerik:GridBoundColumn DataField="Grade" HeaderText="Grade" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="55px" />
                    <telerik:GridBoundColumn DataField="Subject" HeaderText="Subject" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="90px" />
                    <telerik:GridBoundColumn DataField="Course" HeaderText="Course" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="90px" />
                    <telerik:GridBoundColumn DataField="Term" HeaderText="Term" ShowSortIcon="true" ItemStyle-Font-Size="Small"
                        HeaderStyle-Width="45px" />
                    <telerik:GridBoundColumn DataField="TestType" HeaderText="Type" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="80px" />
                    <telerik:GridBoundColumn DataField="Status" HeaderText="Status" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="80px" />
                    <telerik:GridBoundColumn DataField="Scheduling" HeaderText="Scheduling" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="200px" />
                    <telerik:GridBoundColumn DataField="Author" HeaderText="Author" ShowSortIcon="true" UniqueName="Author"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="85px" />
                    <telerik:GridBoundColumn DataField="DateCreated" HeaderText="Created" ShowSortIcon="true" UniqueName="DateCreated"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="80px" />
                    <telerik:GridBoundColumn DataField="DateUpdated" HeaderText="Last Updated" ShowSortIcon="true" UniqueName="DateUpdated"
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
    <asp:TextBox ID="selectedAssessmentInput" runat="server" ClientIDMode="Static" Style="display: none;" />
    <asp:Button ID="gotoStaffButton" runat="server" OnClick="GotoAssessmentButtonClick"
        ClientIDMode="Static" Style="display: none;" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
    <asp:TextBox ID="hiddenTextBox" runat="server" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenGuidBox" runat="server" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenChkAll" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenSelected" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenDeSelected" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenSelectedCount" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenTotalCount" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenLrmiTextBox" runat="server" Style="visibility: hidden; display: none;" />
</asp:Content>
