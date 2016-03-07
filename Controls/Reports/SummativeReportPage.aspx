<%@ Page Title="Standard Search" Language="C#" MasterPageFile="~/SearchExpanded.Master"
    AutoEventWireup="true" CodeBehind="SummativeReportPage.aspx.cs" Inherits="Thinkgate.Controls.Reports.SummativeReportPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function getSchoolsAndTeachers(result) {
            // Schools
            updateCriteriaControl('School', result.Schools, 'DropDownList', 'School');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder"
    runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <asp:PlaceHolder runat="server" ID="criteraDisplayPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder"
    runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <div id="divExportOptions" style="text-align: left">
        <asp:ImageButton runat="server" ID="btnExportExcel" OnClick="btnExportExcel_Click"
            ImageUrl="~/Images/Toolbars/excel_button.png" />
        <asp:ImageButton runat="server" ID="btnFLDOE" OnClientClick="window.open('http://www.fldoe.org/asp/k12memo/pdf/tngcbtf.pdf'); return false;"
            ImageUrl="~/Images/Toolbars/info.png" Visible="False"/>
    </div>
    <div id="lblInitialText" runat="server" style="font-size: 11pt; text-align:center; margin-top:25%;">Please select criteria for all required fields (Indicated by <span style="color: rgb(255, 0, 0); font-weight: bold;">*</span>)<br/> then Update Results.</div>
    <telerik:RadTreeList ID="radTreeResults" runat="server" ParentDataKeyNames="ParentConcatKey"
        OnChildItemsDataBind="TreeListChild_DataSourceNeeded" OnNeedDataSource="TreeListDataSourceNeeded"
        OnItemDataBound="radTreeResults_ItemDataBound" AllowLoadOnDemand="true" Skin="Office2010Silver"
        DataKeyNames="ConcatKey" AutoGenerateColumns="false" Height="600px" Width="100%" 
        HeaderStyle-Width="100px" CssClass="rtlSummativeResults" Visible="False">
        <ClientSettings Scrolling-AllowScroll="true" Scrolling-UseStaticHeaders="true">
            <Resizing ResizeMode="AllowScroll" AllowColumnResize="true" EnableRealTimeResize="true" />
        </ClientSettings>
    </telerik:RadTreeList>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
    <asp:TextBox ID="hiddenTextBox" runat="server" Style="visibility: hidden; display: none;" />
    <asp:TextBox ID="hiddenGuidBox" runat="server" Style="visibility: hidden; display: none;" />
    <script type="text/javascript">

    </script>
</asp:Content>
