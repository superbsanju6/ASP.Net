<%@ Page Language="C#" MasterPageFile="~/SearchExpanded.Master" AutoEventWireup="true"
    CodeBehind="AssessmentAssignmentSearch_Expanded.aspx.cs" Inherits="Thinkgate.Controls.Assessment.AssessmentAssignmentSearch_Expanded" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <telerik:RadScriptBlock ID="serverScriptBlock" runat="server">
        <script type="text/javascript">
            function alterPageComponents(state) {
                $('.rgHeaderDiv').each(function () {
                    this.style.width = '';
                });
            }


            window.onload = function () {
                alterPageComponents('moot');
            }

            function findSelectedClassTestEvents(sender, args) {
                var assessmentList = "";
                var assessmentCategory = document.getElementById('<%= hiddenAssessmentCategory.ClientID %>').value;

                if ($('#chkAll').attr('checked')) {

                    assessmentList = document.getElementById('<%= imgPrintBubble.ClientID %>').attributes['TEIDS'].value;
                }
                else {
                    assessmentList = document.getElementById('<%= hidSelectedTestEventIDs.ClientID %>').value;
                }

                searchAsssessmentAssignment_printClick(assessmentList, assessmentCategory);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder"
    runat="server">
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
    <div style="width: 100%; height: 72px;">
        <div class="listWrapper" style="width: 50%; height: 62px; text-align: left; float: left;" runat="server" id="headerButtonsWrapper">
            <ul>
                <li class="searchAction" id="printBubble" runat="server">
                    <telerik:RadButton ID="imgPrintBubble" ClientIDMode="Static" runat="server" Width="34px"
                        AutoPostBack="false" Height="34px" Enabled="false" OnClientClicked="findSelectedClassTestEvents">
                        <Image ImageUrl="~/Images/assessment/bubble_active.png" DisabledImageUrl="~/Images/assessment/bubble_inactive.png" />
                    </telerik:RadButton>
                    <br />
                    <a style="color: black;">Bubble
                        <br />
                        Sheets</a> </li>
                <li class="searchAction" id="btnEnable" runat="server">
                    <telerik:RadButton ID="radBtnEnable" ClientIDMode="Static" runat="server" Width="34"
                        Height="34" Enabled="false" OnClick="EnableAssessments_Click">
                        <Image ImageUrl="~/Images/assessment/enable_active.png" DisabledImageUrl="~/Images/assessment/enable_inactive.png" />
                    </telerik:RadButton>
                    <br />
                    <a style="color: black;">Enable
                        <br />
                        Online</a> </li>
                <li class="searchAction" id="btnDisable" runat="server">
                    <telerik:RadButton ID="radBtnDisable" ClientIDMode="Static" runat="server" Width="34px"
                        Height="34px" Enabled="false" OnClick="DisableAssessments_Click">
                        <Image ImageUrl="~/Images/assessment/disable_active.png" DisabledImageUrl="~/Images/assessment/disable_inactive.png" />
                    </telerik:RadButton>
                    <br />
                    <a style="color: black;">Disable
                        <br />
                        Online</a> </li>
            </ul>
        </div>       
        <%--Start: BugID#:150 , 12/12/2012 : Jeetendra Kumar, To Display No of Results Found after searching.--%>       
        <div style="width: 50%; text-align: right; float: right;">       
        <table width="100%">
        <tr>
        <td>
            <asp:ImageButton ID="exportGridImgBtn" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
                OnClick="ExportGridImgBtn_Click" Width="20px" />
        </td>
        </tr>
         <tr>
            <td>
                &nbsp;
            </td>
        </tr>
         <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>        
               <td style="float: right; padding-right:5px; ">
                <asp:Label ID="lblSearchTotal" style="color: black;"  runat="server"
                    Text="" />
        </td>
        </tr>
        </table>
        </div> 
        <%--End: BugID#:150 , 12/12/2012: Jeetendra Kumar, To Display No of Results Found after searching.--%>       
    </div>
    <div style="bottom: 0px; left: 0px;">
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
            Skin="Web20" AllowFilteringByColumn="False" PageSize="20" AllowPaging="True" 
            AllowSorting="True" OnPageIndexChanged="RadGridPageIndexChanged" AllowMultiRowSelection="true"
            OnItemDataBound="radGridResults_ItemDataBound" OnSortCommand="OnSortCommand">            
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="false"></PagerStyle>
            <ClientSettings EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="True" UseClientSelectColumnOnly="true" />
                <ClientEvents OnRowSelecting="RowClicked" OnRowSelected="RowSelected" OnRowDeselecting="RowClicked" OnRowDeselected="RowDeselected" />
                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True" ScrollHeight="480px">
                </Scrolling>
            </ClientSettings>
            <ExportSettings IgnorePaging="true" OpenInNewWindow="true">
            </ExportSettings>
            <MasterTableView TableLayout="Auto">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="" UniqueName="ButtonText" HeaderStyle-Width="30px">
                        <HeaderTemplate>
                            <input id="chkAll" runat="server" type="checkbox" clientidmode="Static" name="headerSelect"
                                value="all" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <input id="chkRowInput" class="chkRow" runat="server" type="checkbox" name="rowSelect"
                                value='<%# Eval("EncryptedID")%>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="" UniqueName="ButtonText" HeaderStyle-Width="35px">
                        <ItemTemplate>
                            <asp:PlaceHolder ID="phButtons" runat="server">
                                <img alt="Administration" id="imgGraphicAdmin" runat="server" src="~/Images/dashboard_small.png"
                                    title="Administration" style="cursor: pointer;" onclick="alert('Missing event target')" />
                            </asp:PlaceHolder>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Assessment" DataField="TestName" UniqueName="Name"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="200px">
                        <ItemTemplate>            
                           <asp:HyperLink ID="lnkListTestName" runat="server" NavigateUrl="" Target="_blank"
                                Visible="False" Style="color: Blue;"><%# String.Format("{0} - {1}", Eval("TestName"), Eval("Description")) %></asp:HyperLink>                            
                            <asp:Label runat="server" ID="lblListTestName" runat="server" Visible='False' Text='<%# String.Format("{0} - {1}", Eval("TestName"), Eval("Description")) %>'></asp:Label>
                            <asp:Image runat="server" ToolTip="Secure" ImageUrl="~/Images/IconSecure-lock.png" ID="imgIconSecure" Width="22px" Height="15px" Visible="false" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Class\Group" DataField="ClassName" UniqueName="Name"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="165px" ShowSortIcon="true" SortExpression="ClassName">
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkListClassName" runat="server" NavigateUrl="" Target="_blank"
                                Visible="True" Style="color: Blue;"><%# Eval("ClassName")%></asp:HyperLink>
                            <div style="display: none;">
                                <telerik:RadTextBox ID="hiddenEncryptedID" runat="server" Value='<%# Eval("EncryptedID")%>'
                                    Style="display: none; width: 0px !important;" />
                                <telerik:RadTextBox ID="hiddenTestID" runat="server" Value='<%# Eval("TestID")%>'
                                    Style="display: none; width: 0px !important;" />
                                <telerik:RadTextBox ID="hiddenClassID" runat="server" Value='<%# Eval("ClassID")%>'
                                    Style="display: none; width: 0px !important;" />
                                <telerik:RadTextBox ID="hiddenClassTestEventID" runat="server" Value='<%# Eval("TestEventID")%>'
                                    Style="display: none; width: 0px !important;" />
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="TeacherName" HeaderText="Teacher" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="165px" />
                    <telerik:GridBoundColumn DataField="TestEventID" HeaderText="Test ID" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="150px"  ItemStyle-Width="150px"/>
                    <telerik:GridBoundColumn DataField="AvgScore" HeaderText="Avg Score" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="90px" />
                    <telerik:GridBoundColumn DataField="TotalScored" HeaderText="Total Scored" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="90px" />
                    <telerik:GridBoundColumn DataField="Scheduling" HeaderText="Scheduling" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="200px" />
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
    <asp:TextBox ID="hiddenListOfTestEventIDs" ClientIDMode="Static" runat="server" Style="visibility: hidden;
        display: none;" />
    <asp:HiddenField ID="hiddenAssessmentCategory" runat="server" />
    <asp:TextBox ID="hiddenChkAll" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenSelected" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
    <asp:HiddenField ID="hidSelectedTestEventIDs" ClientIDMode="Static" runat="server" />
    <asp:TextBox ID="hiddenDeSelected" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenSelectedCount" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenTotalCount" runat="server" ClientIDMode="Static" Style="visibility: hidden; display: none;" />
</asp:Content>
