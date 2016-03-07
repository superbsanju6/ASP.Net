<%@ Page Title="Standards Search" Language="C#" AutoEventWireup="true" CodeBehind="StandardsSearch_ExpandedV2.aspx.cs" Inherits="Thinkgate.Controls.Standards.StandardsSearch_ExpandedV2" MasterPageFile="~/Search.Master"%>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx"  %>
<%@ Register TagPrefix="e3" TagName="GradeSubjectCourseStandardSetForStandardSearch" Src="~/Controls/E3Criteria/GradeSubjectCourseStandardSetForStandardSearch.ascx"  %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextWithDropdown" Src="~/Controls/E3Criteria/TextWithDropdown.ascx"  %>
<%@ Register TagPrefix="e3" TagName="SortBar" Src="~/Controls/E3Criteria/SortBar.ascx" %>
<%@ Register TagPrefix="e3" Namespace="Thinkgate.Controls.E3Criteria" Assembly="Thinkgate" %>
<%@ MasterType virtualpath="~/Search.Master" %>

<asp:Content ID="Content8" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
      
        
        
          .RadTreeList .rtlVBorders .rtlR td, .RadTreeList .rtlVBorders .rtlA td, .RadTreeList .rtlVBorders .rtlREdit td, .RadTreeList .rtlVBorders .rtlRFooter td
        {
            overflow: hidden;
            white-space: nowrap;
            text-overflow: ellipsis;
        }

        
        .RadTreeList .rtlExpand, .RadTreeList .rtlCollapse
        {
            display: none !important;
        }

    
        #rightColumn
        {
            background-color: white;
        }
    
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <e3:SortBar ID="SortBar" runat="server" OnExcelClick="ExportGridImgBtn_Click" ShowSortDropdown="False" ShowExcelButton="True"/>      
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results"/>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:GradeSubjectCourseStandardSetForStandardSearch runat="server" ID="ctrlGradeSubjectCourseStandardSet" CriteriaName="GradeSubjectCourseStandardSet" ShowStandardLevels="true" />
    <e3:TextWithDropdown ID="txtSearch" CriteriaName="TextSearch" Text="Text Search" runat="server" DataTextField="Name"/>    
    <e3:DropDownList ID="cmbStandardFilter" CriteriaName="StandardFilter" runat="server" Text="Standard Filter" EmptyMessage="None" DataTextField="Key" DataValueField="Value"/>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <div id="gridViewContainer" style="height: 100%; width: 100%">
        <telerik:RadTreeList ID="radTreeResults" runat="server" ParentDataKeyNames="ParentID" Visible="False"
            DataKeyNames="StandardID" Skin="Office2010Silver"  Width="100%" Height="100%"
            AutoGenerateColumns="False" OnNeedDataSource="TreeListDataSourceNeeded" 
            OnItemCommand="radTreeResults_ItemCommand" OnItemDataBound="radTreeResults_ItemDataBound" ShowTreeLines="false">
            <ClientSettings>
                <Resizing ResizeMode="AllowScroll" AllowColumnResize="true" EnableRealTimeResize="true" />
                <Scrolling AllowScroll="true" SaveScrollPosition="true" ScrollHeight="560px"/>
                <ClientEvents OnTreeListCreated="TreeListCreated" />
            </ClientSettings>
            <Columns>
                <telerik:TreeListSelectColumn HeaderStyle-Width="40px" UniqueName="SelectColumn">
                </telerik:TreeListSelectColumn>
                <telerik:TreeListTemplateColumn HeaderStyle-Width="140px" DataField="NameDisplayText"
                    UniqueName="NameDisplayText" HeaderText="Name">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkExpandAll" runat="server" CommandName="ExpandCollapse"
                            CommandArgument="ExpandAll" Text="+" Visible="true"  StandardID='<%# Eval("EncryptedID") %>' StandardName='<%# Eval("StandardName") %>' ></asp:LinkButton>
                        <asp:LinkButton ID="lnkCollapseAll" runat="server" CommandName="ExpandCollapse"
                            CommandArgument="CollapseAll" Text="-" Visible="false"  StandardID='<%# Eval("EncryptedID") %>' StandardName='<%# Eval("StandardName") %>' ></asp:LinkButton>
                        <asp:HyperLink ID="lnkStandardName" runat="server" Target="_blank" NavigateUrl='<%#"~/Record/StandardsPage.aspx?xID=" + Eval("LinkID") %>'
                            Visible="True" Style="color: Blue;" StandardID='<%# Eval("EncryptedID") %>' StandardName='<%# Eval("StandardName") %>' ><%# Eval("NameDisplayText")%></asp:HyperLink>                        
                    </ItemTemplate>
                </telerik:TreeListTemplateColumn>
                <telerik:TreeListBoundColumn HeaderStyle-Width="300px" DataField="StandardText" UniqueName="StandardText"
                    HeaderText="Text" />
                <telerik:TreeListBoundColumn HeaderStyle-Width="45px" DataField="Grade" UniqueName="Grade"
                    HeaderText="Grade" />
                <telerik:TreeListBoundColumn HeaderStyle-Width="100px" DataField="Subject" UniqueName="Subject"
                    HeaderText="Subject" />
                <telerik:TreeListBoundColumn HeaderStyle-Width="100px" DataField="Course" UniqueName="Course"
                    HeaderText="Course" />
                <telerik:TreeListBoundColumn HeaderStyle-Width="100px" DataField="Level" UniqueName="Level"
                    HeaderText="Level" />
                <telerik:TreeListBoundColumn HeaderStyle-Width="50px" DataField="ItemCount" UniqueName="ItemCount"
                    HeaderText="Items" />
                <telerik:TreeListBoundColumn HeaderStyle-Width="75px" DataField="ResourceCount" UniqueName="ResourceCount"
                    HeaderText="Resources" />
            </Columns>
        </telerik:RadTreeList>
        <div id="divDefaultMessage" style="height: 100%; text-align: center;">
            <div style="height: 40%"></div>
            <div style="height: 20%">
            <strong>Please choose criteria and select Search.</strong>
            <br/>(<strong>Standard Set</strong> and <strong>Grade</strong> are required.)
            </div>
            <div style="height: 40%"></div>
        </div>
        <asp:HiddenField ID="hfQuestionID" ClientIDMode="Static" runat="server" />
    </div>
   
    <script type="text/javascript">

        $(document).ready(function () {
            alterPageComponents('moot');
            resizeContent();
        });

        function TreeListCreated() {
            resizeContent();
        }

        function expandedStandardsSearchAjaxResponseHandler(sender, eventArgs) {
            try {
                var treeList = $find('<%=this.radTreeResults.ClientID %>');
                if (treeList && treeList._data._itemData && treeList._data._itemData.length > 0) {
                    $("input[id*=SortBar_imgExcel]").show();
                }
                else {
                    $("input[id*=SortBar_imgExcel]").hide();
                }
            }
            catch (e) { }
        }

        function resizeContent() {
            //var pagerHeight = $("#SearchPager").height() + 4 || 0;         
            $(".rtlDataDiv").height($(window).height() - $("#topBar").height() - $(".rtlHeader").height() - $(".rtlHeader").height()*2.5);
           
        }

        function btnSelectReturn_Click() {
            var results = [];
            var questionID = document.getElementById("hfQuestionID").value;
            if ($find('<%=radTreeResults.ClientID %>') != null) {
                var items = $find('<%=radTreeResults.ClientID %>').get_selectedItems();
                for (var i = 0; i < items.length; i++) {
                    if (i > 0) results.push("|");
                    results.push($(items[i]._element).find('A')[0].getAttribute('StandardName'));
                    results.push(",");
                    results.push($(items[i]._element).find('A')[0].getAttribute('StandardID'));
                }
                if (results.length == 0) {
                    alert("No standards selected.");
                    return;
                }
                var newAndReturnType = getURLParm('NewAndReturnType');

                var DialogueToSendResultsTo;
                DialogueToSendResultsTo = parent.getCurrentCustomDialogByName(newAndReturnType);
                try {
                    DialogueToSendResultsTo.get_contentFrame().contentWindow.InsertStandard(results.join(''));
                }
                catch (e) {
                    try {
                        if (newAndReturnType == "AssessmentPage") {
                            parent.InsertStandardForItem(results[2], questionID);
                        }
                        else {
                            parent.InsertStandard(results.join(''));
                        }
                    }
                    catch (e) {
                        try {
                            parent.ContentEditorEXTERNALITEM.InsertStandard(results.join(''));
                        }
                        catch (e) {
                            
                        }
                    }
                }
                closeCurrentCustomDialog();
            }
        }

        function alterPageComponents(state) {
            $('.rgHeaderDiv').each(function () {
                this.style.width = '';
            });

        }
        
    </script>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
</asp:Content>
