<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Comments.aspx.cs" Inherits="Thinkgate.Controls.CompetencyWorksheet.Comments" Title="Comments" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <style>

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

        .classCart
        {
            top: 4px !important;
            left: 5px !important;
        }

        .classAdd
        {
            top: 4px !important;
            right: 5px !important;
        }

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

    </style>

   
</head>
<body>
    <form id="form1" runat="server">
        <div id="tbldiv" class="modal"></div>
        <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-migrate-1.1.0.min.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
                <asp:ScriptReference Path="~/Scripts/EditSubmitResultPagesWithinCustomDialog.js" />
                <asp:ScriptReference Path="~/Scripts/jquery.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-ui.min.js" />

            </Scripts>
        </telerik:RadScriptManager>

        <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
            <StyleSheets>
                <telerik:StyleSheetReference Path="~/Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" />
            </StyleSheets>
        </telerik:RadStyleSheetManager>

        <%-- <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <link href="Scripts/jquery-ui.css" rel="stylesheet" />
            </telerik:RadCodeBlock>--%>


        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="False" Modal="True" InitialBehaviors="Close"
            Animation="None">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Web20">
        </telerik:RadSkinManager>
        <div>
            <div style="overflow: hidden;">
                <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="CommentPanel">
                    <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
                        <div class="tableDiv" id="HeaderPart">
                            <div class="row">
                               
                                                <table border="1" style="width: 835px"  cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td style="width:85%"><strong>Student Name: </strong>
                                                            <asp:Label ID="lblStudentName" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="width:15%; text-align:center" rowspan="3" >
                                                           <asp:Button ID="btnPrint"  runat="server" CssClass="navButton btnPrint" ToolTip="Print" OnClientClick="PrintComments();" />
                                                            <%-- <input type="button"  id="btnPrint" value="" class="navButton btnPrint" />--%>

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
                            <br />
                            <table style="width: 835px">
                                <tr>
                                    <td align="right">
                                        <telerik:RadButton ID="btnAddCommentNew" runat="server" Text="Add Comment" OnClientClicked="showAddNewCommentsWindow">
                                            <Icon PrimaryIconUrl="~/Images/add.gif" PrimaryIconCssClass="classCart" />
                                        </telerik:RadButton>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        
                         <div style="height: 25px;">
                        <asp:Label ID="lblCommentMsg" runat="server" Visible="false"  Font-Size="Small" ForeColor="Red" Text="There are no records to display."></asp:Label>
                         </div>

                        <div id="gridComment" style="width: 100%; height: auto; padding: 0;">

                          <telerik:RadGrid runat="server" ID="radGridComments" 
                                AutoGenerateColumns="False" 
                                Width="95%"
                                AllowFilteringByColumn="False"
                                PageSize="1" 
                                AllowPaging="True"
                                AllowSorting="False" 
                                OnPageIndexChanged="RadGridComments_PageIndexChanged"
                                AllowMultiRowSelection="False"
                                OnItemCommand="radGridComments_ItemCommand"
                                OnItemDataBound="radGridComments_ItemDataBound"
                                OnSortCommand="OnSortCommand" 
                                Height="300px" 
                               CssClass="assessmentSearchHeader"
                               SelectedItemStyle-BackColor="White"
                               Skin="Web20">
                                <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>

                                <ClientSettings EnableRowHoverStyle="false">
                                    <Selecting AllowRowSelect="False" UseClientSelectColumnOnly="true" />
                              
                                    <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True"></Scrolling>
                                </ClientSettings>
                                <MasterTableView TableLayout="Fixed" Width="650px" PageSize="10" Height="30px" NoMasterRecordsText="There are no records to display.">
                                    <Columns>
                                      
                                       <telerik:GridTemplateColumn HeaderText="Remove" DataField="CompetencyCommentID" ItemStyle-Font-Size="Small" HeaderStyle-Width="80px">
                                              <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:ImageButton ID="lnkDelete" runat="server" ToolTip="Click to Delete Comment"
                                                    OnClientClick="return confirm('Are you sure you wish to remove this entry? Once removed, it cannot be retrieved.');" CommandName="Remove"   
                                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CompetencyCommentID") %>' SkinID="Close" ImageUrl="~/Images/cross.gif" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>

                                        <telerik:GridTemplateColumn HeaderText="Edit" DataField="CompetencyCommentID" ShowSortIcon="true" SortExpression="Name"
                                            ItemStyle-Font-Size="Small" HeaderStyle-Width="50px">
                                               <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                               <asp:HiddenField ID="hdnID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "CompetencyCommentID") %>' />
                                                <asp:ImageButton ID="lnkEdit" runat="server" ToolTip="Click to Edit Comment" CommandName="Edit"  OnClientClick="showEditCommentsWindow(this); return false"
                                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CompetencyCommentID") %>' SkinID="Close" ImageUrl="~/Images/Edit.png" />

                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        
                                        <telerik:GridBoundColumn DataField="CommentDT" HeaderText="Date" ShowSortIcon="true"
                                            ItemStyle-Font-Size="Small" HeaderStyle-Width="100px" ItemStyle-Width="150px" />
                                        <telerik:GridBoundColumn DataField="TeacherName" HeaderText="Teacher" ShowSortIcon="true"
                                            ItemStyle-Font-Size="Small" HeaderStyle-Width="150px" ItemStyle-Width="150px" />
                                        <telerik:GridBoundColumn DataField="Comment" HeaderText="Comments" ShowSortIcon="true"
                                            ItemStyle-Font-Size="Small" HeaderStyle-Width="459px" ItemStyle-Width="459px" />
                                        <telerik:GridBoundColumn DataField="Userid" Display="false" ItemStyle-Width="0px"  HeaderStyle-Width="0px"/>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>

                        </div>
                    </asp:Panel>

                </telerik:RadAjaxPanel>
                <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static">
                    <asp:Label runat="server" ID="lblResultMessage" Text="" />
                    <br />
                    <telerik:RadButton runat="server" ID="RadButtonOpenStaff" Text="OK" AutoPostBack="False" OnClientClicked="closeWindow" />
                    &nbsp;
                    <%--    <telerik:RadButton runat="server" ID="RadButtonClose" Text="Close" AutoPostBack="False" OnClientClicked="closeWindow" />--%>
                </asp:Panel>
                <%-- <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="addStaffLoadingPanel" runat="server" />--%>
            </div>
        </div>


       <div id="divCommentAddDialog" title="Comments" style="display: none;">
            <iframe id="divCommentAddDialogframe" src="" frameborder="0" width="430" height="170">No frames</iframe>
        </div>


    </form>
</body>
</html>


<script type="text/javascript">

    function openwin() {
        window.radopen(null, "RadWindow1");
    }

    function showAddNewCommentsWindow() {

        var WorksheetID = '<%= this.WorksheetId %>';
        var StandardID = '<%= this.StandardId %>';
        var StudentID = '<%= this.StudentId %>';

        showCommentDialog(WorksheetID, StandardID, StudentID);
    }

    function close() {
        customDialog({ url: 'Comments.aspx', maximize: true, maxwidth: 600, maxheight: 520 });
    }


    $(document).ready(function () {
        $("#tbldiv").addClass("modal");
        $('div#divCommentAddDialog').dialog({ autoOpen: false, modal: true });
        $("#divCommentAddDialog").dialog({
            title: "Comments",
            modal: true
        });
        setTimeout(function () { $("#tbldiv").removeClass("modal"); }, 3000);

        //$("#btnPrint").click(function () {
        //    window.print();
        //});
    });

    function showCommentDialog(WorksheetID, StandardID, StudentID) {

        $('#divCommentAddDialogframe').attr('src', 'AddComments.aspx?WorksheetID=' + WorksheetID + '&StandardID=' + StandardID + '&StudentID=' + StudentID);
        //$('#divCommentAddDialog').dialog("option", "height", "230");
        //$('#divCommentAddDialog').dialog("option", "width", "480");
        //$('#divCommentAddDialog').dialog("option", "position", "center");
        //$('#divCommentAddDialog').dialog("open");
        //return false;


    }

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

    function PrintComments() {
        var windowUrl = 'CommentsPrint.aspx ';
        var num;
        var uniqueName = new Date();
        var windowName = 'Print' + uniqueName.getTime();
        var printWindow = window.open(windowUrl, windowName, 'left=50000,top=50000,width=0,height=0');
        printWindow.focus();
        setTimeout(function () { printWindow.close(); }, 10000);
        return false;
    }

</script>
