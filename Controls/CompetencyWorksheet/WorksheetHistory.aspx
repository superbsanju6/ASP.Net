<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorksheetHistory.aspx.cs" Inherits="Thinkgate.Controls.CompetencyWorksheet.WorksheetHistory" Title="History" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style>
     .modal {
            display: block;
            position: fixed;
            z-index: 1000;
            top: 0;
            left: 0;
            right: 0px;
            height: 100%;
            width: auto;
            background: rgba( 255, 255, 255, .8 ) url('../../Styles/Thinkgate_Window/Common/loading.gif') 60% 50% no-repeat;
        }
       .navButton 
        {
        border: 0px solid #e3e3e3;
        color: #000;
        margin: 5px 30px 5px 20px;
        width:40px;
        height:40px;
        }

       .btnPrint 
       {
           background: #f2f2f2 url(../../images/toolbars/print.png) no-repeat;
           cursor:pointer;
           width:30px;
           height:30px;
       }
        .tableDiv
        {
            display: table;
        }

        .row
        {
            display: table-row;
        }

        .cellLabel
        {
            display: table-cell;
            width: 40%;
            height: 40px;
            text-align: left;
            font-size: 13pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
        }

        .cellValue
        {
            display: table-cell;
            height: 40px;
            text-align: left;
            font-size: 12pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
        }

        .textBox
        {
            border: solid 1px #000;
            padding: 2px;
            width: 250px;
        }

        .smallText
        {
            font-size: 9pt;
        }

        .radDropdownBtn
        {
            font-weight: bold;
            font-size: 11pt;
        }

        .labelForValidation
        {
            color: Red;
            display: block;
            font-size: 10pt;
        }

        .fieldLabel
        {
            font-weight: bold;
            font-size: 10pt;
            white-space: nowrap;
            text-align: left;
            width: 110px;
            height: 30px;
            vertical-align: top;
        }

        .fieldEntry
        {
            vertical-align: top;
            text-align: left;
        }

        .roundButtons
        {
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            position: relative;
            border: solid 1px #000;
            border-radius: 50px;
            float: right;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
        }

        .auto-style1
        {
            height: 76px;
        }
        
    </style>

   
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-migrate-1.1.0.min.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
                <asp:ScriptReference Path="~/Scripts/EditSubmitResultPagesWithinCustomDialog.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-ui.min.js" />
            </Scripts>
        </telerik:RadScriptManager>
          <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
            <StyleSheets>
                <telerik:StyleSheetReference Path="~/Scripts/jquery-ui.css" />
            </StyleSheets>
        </telerik:RadStyleSheetManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="False" Modal="True" InitialBehaviors="Close"
            Animation="None">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Web20">
        </telerik:RadSkinManager>

        <div style="overflow: hidden;">
            <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="WorksheetHistoryPanel">
                <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
                    <div class="tableDiv" id="HeaderPart">
                        <div class="row">
                            <table border="1" style="width: 835px" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td style="width: 85%"><strong>Student Name: </strong>
                                                            <asp:Label ID="lblStudentName" runat="server"></asp:Label>
                                    </td>
                                    <td rowspan="3" style="width: 15%; text-align: center">
                                      
                                         <%-- <input type="button"  id="btnPrint" value="Print" class="navButton btnPrint"  />--%>
                                        <asp:Button ID="btnPrint"  runat="server" CssClass="navButton btnPrint" ToolTip="Print" OnClientClick="PrintHistory();" />

                                    </td>
                                </tr>
                                <tr>
                                    <td><strong>Standard: </strong> 
                                           <asp:Label ID="lnkStandard" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="auto-style1">
                                        <asp:Label ID="lblStandardDesc" runat="server"></asp:Label>
                                    </td>

                                </tr>
                            </table>

                        </div>
                    </div>
                </asp:Panel>

            </telerik:RadAjaxPanel>
            <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="WorksheetHistoryPanel" runat="server" />
        </div>

        <br />
        <div id="Performance" class="performance" style="height: 25px;">
            <span style="font-size: medium; text-decoration: underline; text-align: center; font-weight: bold">Performance:</span>
            <br />
            <asp:Label ID="lblPerformanceMsg" runat="server" Font-Size="Small" ForeColor="Red" Visible="false" Text="There are no records to display." ></asp:Label>
        </div>


        <div style="width: 96%; height: auto; padding: 0;">

            <telerik:RadGrid runat="server" ID="radGridHistory" 
                AutoGenerateColumns="False" 
                Width="100%"
                AllowFilteringByColumn="False"
                PageSize="1"
                AllowPaging="False"
                AllowSorting="False"  
                OnPageIndexChanged="radGridHistory_PageIndexChanged"
               AllowMultiRowSelection="False"
                OnItemCommand="radGridHistory_ItemCommand"
                OnItemDataBound="radGridHistory_ItemDataBound"
                OnSortCommand="OnSortCommand" 
                Height="300px" 
                CssClass="assessmentSearchHeader" 
                SelectedItemStyle-BackColor="White"                
                Skin="Web20">
                <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom"></PagerStyle>
                <ClientSettings EnableRowHoverStyle="false">
                    <Selecting AllowRowSelect="False" UseClientSelectColumnOnly="true" />
                    <%-- <ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeselected" OnGridCreated="changeGridHeaderWidth" />--%>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True"></Scrolling>
                </ClientSettings>
                <MasterTableView TableLayout="Fixed" Width="650px"   NoMasterRecordsText="There are no records to display.">
                    <Columns>
                        <telerik:GridTemplateColumn HeaderText="Remove" DataField="WorksheetRubricXrefID" 
                            ItemStyle-Font-Size="Small" HeaderStyle-Width="80px">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:ImageButton ID="lnkDelete" runat="server" ToolTip="Click to Delete History"
                                    OnClientClick="parent.ChangeUpdatedValue(true);return confirm('Are you sure you wish to remove this entry? Once removed, it cannot be retrieved.');" CommandName="Remove"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "WorksheetRubricXrefID") %>' SkinID="Close" ImageUrl="~/Images/cross.gif" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="ScoreDate" HeaderText="Date" 
                            ItemStyle-Font-Size="Small" HeaderStyle-Width="100px" ItemStyle-Width="100px" />
                       <%-- <telerik:GridBoundColumn DataField="Name" HeaderText="Level" ShowSortIcon="true"
                            ItemStyle-Font-Size="Small" HeaderStyle-Width="250px" ItemStyle-Width="250px" />--%>
                        <telerik:GridTemplateColumn HeaderText="Level"  ItemStyle-Font-Size="Small" HeaderStyle-Width="380px" ItemStyle-Width="380px">
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# StripHtml(Eval("Name").ToString())%>'  ></asp:Label>
                                 
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="TeacherName" HeaderText="Teacher" 
                            ItemStyle-Font-Size="Small" HeaderStyle-Width="270px" ItemStyle-Width="270px" />
                        <telerik:GridBoundColumn DataField="Teacher" HeaderText="" Display="false" ItemStyle-Width="0px"  HeaderStyle-Width="0px" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>

        </div>

        
        <br />
        <div id="Comments"  style="height: 25px;">
            <span style="font-size: medium; text-decoration: underline; text-align: center; font-weight: bold">Comments:</span>
            <br />
             <asp:Label ID="lblCommentMsg" runat="server" Visible="false"  Font-Size="Small" ForeColor="Red" Text="There are no records to display."></asp:Label>
        </div>


        <div style="width: 96%; height: auto; padding: 0;">

            <telerik:RadGrid runat="server" ID="radGridComments" AutoGenerateColumns="False" Width="100%"
                AllowFilteringByColumn="False" PageSize="1" AllowPaging="False" AllowSorting="False"  SelectedItemStyle-BackColor="White" 
                OnPageIndexChanged="RadGridComments_PageIndexChanged" AllowMultiRowSelection="False" OnItemCommand="radGridComments_ItemCommand"
                OnItemDataBound="radGridComments_ItemDataBound" OnSortCommand="OnCommentSortCommand" Height="300px" CssClass="assessmentSearchHeader" Skin="Web20">
                <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom"></PagerStyle>
                <ClientSettings EnableRowHoverStyle="false">
                    <Selecting AllowRowSelect="False" UseClientSelectColumnOnly="true" />
                    <%--  <ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeselected" OnGridCreated="changeGridHeaderWidth" />--%>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True"></Scrolling>
                </ClientSettings>
                <MasterTableView TableLayout="Fixed" Width="650"  NoMasterRecordsText="There are no records to display.">
                    <Columns>
                        <%--<telerik:GridClientSelectColumn HeaderText="Select" Visible="false" UniqueName="Select" HeaderStyle-Width="15px"></telerik:GridClientSelectColumn>--%>
                        <telerik:GridTemplateColumn HeaderText="Remove" DataField="CompetencyCommentID" 
                            ItemStyle-Font-Size="Small" HeaderStyle-Width="80px">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:ImageButton ID="lnkDelete" runat="server" ToolTip="Click to Delete Comment"
                                    OnClientClick="return confirm('Are you sure you wish to remove this entry? Once removed, it cannot be retrieved.');" CommandName="Remove"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CompetencyCommentID") %>' SkinID="Close" ImageUrl="~/Images/cross.gif" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Edit" DataField="CompetencyCommentID" 
                            ItemStyle-Font-Size="Small" HeaderStyle-Width="50px">
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:HiddenField ID="hdnID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "CompetencyCommentID") %>' />
                                <asp:ImageButton ID="lnkEdit" runat="server" ToolTip="Click to Edit Comment" CommandName="Edit" OnClientClick="showEditCommentsWindow(this); return false"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CompetencyCommentID") %>' SkinID="Close" ImageUrl="~/Images/Edit.png" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="CommentDT" HeaderText="Date" 
                            ItemStyle-Font-Size="Small" HeaderStyle-Width="100px" ItemStyle-Width="150px" />
                        <telerik:GridBoundColumn DataField="TeacherName" HeaderText="Teacher" 
                            ItemStyle-Font-Size="Small"  HeaderStyle-Width="150px" ItemStyle-Width="150px" />
                        <telerik:GridBoundColumn DataField="Comment" HeaderText="Comments" 
                            ItemStyle-Font-Size="Small" HeaderStyle-Width="450px" ItemStyle-Width="450px" />
                        <telerik:GridBoundColumn DataField="Userid" HeaderText="" Display="false" ItemStyle-Width="0px"  HeaderStyle-Width="0px" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>

        </div>

        <div id="divCommentAddDialog" title="Competency Worksheet Comments" style="display: none;">
            <iframe id="divCommentAddDialogframe" src="" frameborder="0" width="430" height="170">No frames</iframe>
        </div>
        <div id="tbldiv"></div>
    </form>
</body>
</html>

 <script type="text/javascript">
     function openwin() {
         window.radopen(null, "RadWindow1");
     }

     $(document).ready(function () {
         $("#tbldiv").addClass("modal");
         $('div#divCommentAddDialog').dialog({ autoOpen: false, modal: true });
         $("#divCommentAddDialog").dialog({
             title: "Comments",
             modal: true
         });
         var isLoaded = false;

         var delay = 5//1 seconds

         setTimeout(function () { $("#tbldiv").removeClass("modal"); }, 3000);



         //$("#btnPrint").click(function () {
         //    window.print();
         //});
     });

     function showEditCommentsWindow(obj) {
         var ID = $(obj).siblings("[id$=hdnID]").first().val();
         showCommentDialogBox(ID);
     }

     function showCommentDialogBox(CompetencyCommentID) {

         $('#divCommentAddDialogframe').attr('src', 'AddComments.aspx?CommentID=' + CompetencyCommentID);
         //$('#divCommentAddDialog').dialog("option", "height", "230");
         //$('#divCommentAddDialog').dialog("option", "width", "480");
         //$('#divCommentAddDialog').dialog("option", "position", "center");
         //$('#divCommentAddDialog').dialog("open");
         //return false;
     }
     function PrintHistory() {

         var windowUrl = 'WorksheetHistoryPrint.aspx ';
         var num;
         var uniqueName = new Date();
         var windowName = 'Print' + uniqueName.getTime();
         var printWindow = window.open(windowUrl, windowName, 'left=50000,top=50000,width=0,height=0');
         printWindow.focus();
         setTimeout(function () { printWindow.close(); }, 10000);
         return false;
     }

    </script>