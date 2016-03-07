<%@ Page Title="" Language="C#" MasterPageFile="~/AddNew.Master" AutoEventWireup="true" CodeBehind="CompetencyTrackingReportStudentList.aspx.cs" Inherits="Thinkgate.Controls.Reports.CompetencyTrackingReportStudentList" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
     <style type="text/css">

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table width="98%;"  style="margin: 5px 5px 5px 2px;" border="1">
        <tr>
            <td width="80%">
                <asp:Label runat="server" ID="lblViewBy" Font-Bold="true"></asp:Label>

                <span id="tdViewByText" runat="server"></span>
            </td> 
            <td rowspan="4">
                <asp:Button ID="btnPrint" runat="server"  CssClass="navButton btnPrint" ToolTip="Print" OnClick="btnPrint_Click" />
            </td>           
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="lblRubric" Font-Bold="true"></asp:Label>
                <span id="spnRubricValue" runat="server"></span>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="lblCompetency" Font-Bold="true"></asp:Label>
               <%-- <a href='#' id="aCompetencyValue" runat="server" target="_blank"></a>--%>
                 <asp:Label ID="aCompetencyValue" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td runat="server" id="spnCompetencyDetail"> <asp:Label ID="lblCompetencyDetail" runat="server"></asp:Label></td>
        </tr>
    </table>

    <div style="margin-top:10px; margin-bottom:10px">
    <asp:Label runat="server" ID="lblStudentCount" ForeColor="Black" Font-Bold="true"  ></asp:Label>
   </div>

    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="expandStudentCountLoadingPanel" Width="98%">
        <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
            <telerik:RadGrid ID="radGridStudentCount" runat="server" AutoGenerateColumns="false"  AllowAutomaticUpdates="True" OnItemDataBound="radGridStudentCount_ItemDataBound">
                <ClientSettings EnableRowHoverStyle="true">
                    <Selecting AllowRowSelect="false" />
                    <Scrolling AllowScroll="True" SaveScrollPosition="True"
                        UseStaticHeaders="True" />
                    <Resizing AllowColumnResize="True" />
                </ClientSettings>
                <HeaderStyle Font-Bold="true" CssClass="gridcolumnheaderstyle" />
                <MasterTableView>
                    <Columns>
                        
                        <telerik:GridBoundColumn   DataField="StudentID" Display="false" ReadOnly="True" UniqueName="LevelID" />
                        <telerik:GridTemplateColumn InitializeTemplatesFirst="false" HeaderText="Name">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="studentNameLink"></asp:Label>
                                </ItemTemplate>
                            <ItemStyle Width="225px" />
                            <HeaderStyle Width="225px" />
                            </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn HeaderText="Grade" DataField="Grade">
                               <ItemStyle Width="225px" />
                            <HeaderStyle Width="225px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="School" DataField="School" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </asp:Panel>
        <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static">
            <asp:Label runat="server" ID="lblResultMessage" Text="" CssClass="resultPanel" />
        </asp:Panel>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="expandStudentCountLoadingPanel" runat="server" />
</asp:Content>
