<%@ Page Title="Class Search" Language="C#" MasterPageFile="~/SearchExpanded.Master"
    AutoEventWireup="true" CodeBehind="ClassSearch_Expanded.aspx.cs" Inherits="Thinkgate.Controls.Class.ClassSearch_Expanded" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function alterPageComponents(state) {
            if (state == "open") {
                $('.rgHeaderDiv').each(function () {
                    this.style.width = '700px';
                    this.style.marginRight = '17px';
                });
                $('.RadGrid').each(function () {
                    this.style.width = '700px';
                });
            }
            else {
                $('.rgHeaderDiv').each(function () {
                    this.style.width = '900px';
                    this.style.marginRight = '17px';
                });
                $('.RadGrid').each(function () {
                    this.style.width = '900px';
                });

            }
        }
    </script>
    <style>
        .RadListBox .rlbGroup {
            max-height: 550px;
            overflow-y: auto;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <script type="text/javascript">
        function getClassGradesSubjectsAndCourses(result) {
            // Subject
            updateCriteriaControl('Subject', result.Subjects, 'CheckBoxList');

            // Course
            updateCriteriaControl('ClassCourses', result.Courses, 'DropDownList', 'Course');
        }

        function getClassCoursesFromSubjects(result) {
            // Course
            updateCriteriaControl('ClassCourses', result.Courses, 'DropDownList', 'Course');
        }

        function getAllSchoolsFromSchoolTypes(result) {
            // School
            updateCriteriaControl('School', result.Schools, 'CheckBoxList', 'School');
        }

         function OpenUrlWithEncryptedIdForClassID(classId) {
            $.ajax({
                type: "POST",
                url: "./ClassSearch_Expanded.aspx/GetUrlWithEncryptedIdForClassID",
                data: "{'classId':" + classId + "}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {

                    window.open(result.d);
                }
            });
        }

    </script>
    <style type="text/css">
        .rlbGroupRight {
            overflow: auto;
            height: auto;
            max-height:550px;
            width: 180px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <asp:PlaceHolder runat="server" ID="criteraDisplayPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder"
    runat="server">
    <div style="width: 100%; height: 30px;">
        <div class="listWrapper" style="width: 50%; height: 30px; text-align: left; float: left;">
            <div id="resultsFoundDiv" runat="server" style="width: 250px;">
            </div>
        </div>
        <div style="width: 50%; text-align: right; float: right;">
            <asp:ImageButton ID="exportGridImgBtn" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
                OnClick="ExportGridImgBtn_Click" Width="20px" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">

    <%--Changes made for BugId# 21093 to remove the bottom space --%>
    <style type="text/css">
        .rgDataDiv {
            height: 530px !important;
        }
    </style>    

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
            OnSortCommand="OnSortCommand"
            Height="590px" CssClass="assessmentSearchHeader" Skin="Web20">
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="false"></PagerStyle>
            <ClientSettings EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="True" UseClientSelectColumnOnly="true" />                
                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True"></Scrolling>
            </ClientSettings>
            <MasterTableView TableLayout="Auto" PageSize="100" Height="32px">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Class" DataField="SchoolName" UniqueName="Name"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="200px" SortExpression="SchoolName">
                        <ItemTemplate>
                            <a style="cursor: pointer; text-decoration: underline"onclick="OpenUrlWithEncryptedIdForClassID('<%# Eval("ClassID")%>')" >
                                <%# Eval("ClassName")%>
                            </a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="SchoolName" HeaderText="School" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="170px" />
                    <telerik:GridBoundColumn DataField="Grade" HeaderText="Grade" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="55px" />
                    <telerik:GridBoundColumn DataField="Subject" HeaderText="Subject" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="90px" />
                    <telerik:GridBoundColumn DataField="Course" HeaderText="Course" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="90px" />
                    <telerik:GridBoundColumn DataField="TeacherName" HeaderText="Teacher" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="170px" />
                    <telerik:GridBoundColumn DataField="Semester" HeaderText="Semester" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="80px" />
                    <telerik:GridBoundColumn DataField="Period" HeaderText="Period" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="80px" />
                    <telerik:GridBoundColumn DataField="Block" HeaderText="Block" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="80px" />
                    <telerik:GridBoundColumn DataField="Section" HeaderText="Section" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="80px" />
                    <telerik:GridBoundColumn DataField="Year" HeaderText="Year" ShowSortIcon="true" ItemStyle-Font-Size="Small"
                        HeaderStyle-Width="100px" />
                    <telerik:GridBoundColumn DataField="CourseID" HeaderText="Course ID" ShowSortIcon="true"
                        ItemStyle-Font-Size="Small" HeaderStyle-Width="150px" />
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
</asp:Content>
