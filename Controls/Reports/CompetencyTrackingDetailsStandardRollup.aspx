<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompetencyTrackingDetailsStandardRollup.aspx.cs" Inherits="Thinkgate.Controls.Reports.CompetencyTrackingDetailsStandardRollup" %>

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

        .navButton {
            border: 0px solid #e3e3e3;
            color: #000;
            margin: 5px 30px 5px 20px;
            width: 40px;
            height: 40px;
        }

        .btnPrint {
            background: #f2f2f2 url(../../images/toolbars/print.png) no-repeat;
            cursor: pointer;
            width: 30px;
            height: 30px;
        }

        .tableDiv {
            display: table;
        }

        .row {
            display: table-row;
        }

        .cellLabel {
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

        .cellValue {
            display: table-cell;
            height: 40px;
            text-align: left;
            font-size: 12pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
        }

        .textBox {
            border: solid 1px #000;
            padding: 2px;
            width: 250px;http://designscrazed.org/free-responsive-html5-css3-templates/
        }

        .smallText {
            font-size: 9pt;
        }

        .radDropdownBtn {
            font-weight: bold;
            font-size: 11pt;
        }

        .labelForValidation {
            color: Red;
            display: block;
            font-size: 10pt;
        }

        .fieldLabel {
            font-weight: bold;
            font-size: 10pt;
            white-space: nowrap;
            text-align: left;
            width: 110px;
            height: 30px;
            vertical-align: top;
        }

        .fieldEntry {
            vertical-align: top;
            text-align: left;
        }

        .roundButtons {
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

        .auto-style1 {
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


            <div class="tableDiv" id="HeaderPart">
                <div class="row">
                    <table border="1" style="width: 835px" cellspacing="0" cellpadding="0">
                        <tr>
                            <td style="width: 85%"><strong>Student Name: </strong>
                                <asp:Label ID="lblStudentName" runat="server"></asp:Label>
                            </td>
                            <td rowspan="4" style="width: 15%; text-align: center">


                                <asp:Button ID="btnPrint" runat="server" CssClass="navButton btnPrint" ToolTip="Print" OnClick="btnPrint_Click" />

                            </td>
                        </tr>
                        <tr>
                            <td><strong>Rubric Value: </strong>
                                <asp:Label ID="lblrubricvalue" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td><strong>Standard: </strong>
                                <asp:Label ID="lnkStandard" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style1" >
                                <asp:Label ID="lblStandardDesc" runat="server"></asp:Label>
                            </td>

                        </tr>
                    </table>

                </div>
            </div>



        </div>

        <br />
        <div id="Performance" class="performance" style="height: 25px;">
            <span style="font-size: medium; text-align: center; font-weight: bold">Competencies:
                <asp:Label runat="server" ID="lblcount" Text="0"></asp:Label></span>
            <br />
            <asp:Label ID="lblPerformanceMsg" runat="server" Font-Size="Small" ForeColor="Red" Visible="false" Text="There are no records to display."></asp:Label>
        </div>


        <div style="width: 96%; padding: 0;">

            <telerik:RadGrid runat="server" ID="radGridCompetency"
                AutoGenerateColumns="False"
                Width="100%"
                AllowFilteringByColumn="False"
                PageSize="1"
                AllowPaging="False"
                AllowSorting="False"
                OnPageIndexChanged="radGridCompetency_PageIndexChanged"
                AllowMultiRowSelection="False"
                OnItemCommand="radGridCompetency_ItemCommand"
                OnItemDataBound="radGridHistory_ItemDataBound"
                Height="230px"
                CssClass="assessmentSearchHeader"
                SelectedItemStyle-BackColor="White"
                Skin="Web20">
                <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom"></PagerStyle>
                <ClientSettings EnableRowHoverStyle="false">
                    <Selecting AllowRowSelect="False" UseClientSelectColumnOnly="true" />
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True"></Scrolling>
                </ClientSettings>
                <MasterTableView TableLayout="Fixed" Width="100%" NoMasterRecordsText="There are no records to display.">
                    <Columns>
                        <telerik:GridTemplateColumn HeaderText="Competency" ItemStyle-CssClass="gridcolumnstyle" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="250px" HeaderStyle-Width="250px">
                            <ItemTemplate>

                                <asp:Label ID="lblchildStd" Width="250px" runat="server" Text='<%# Eval("ChildStandard") %>'></asp:Label>
                                <asp:HiddenField ID="hd_std_id" runat="server" Value='<%# Eval("Id") %>' />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="ChildStandardDesc" HeaderText="Descripion" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="564px" HeaderStyle-Width="564px"  />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>

        </div>


        <br />
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


</script>
