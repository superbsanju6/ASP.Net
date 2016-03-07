<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradeConversion.ascx.cs" Inherits="Thinkgate.Controls.Reports.GradeConversion" %>
<script type="text/javascript">
    function printExcel() {
        var excelButton = document.getElementById('exportGridImgBtn');
        if (excelButton) {
            excelButton.click();
        }
    }
</script>
<div style="width: 100%; margin-top: 10px; margin-bottom: 10px;">
    <asp:ImageButton ID="excelImgBtn" style="float:right; margin: 5px;" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png" Width="20px" CssClass="imgButtons" OnClientClick="printExcel(); return false;" />
</div>
<div style="width: 100%; margin-top: 10px; margin-bottom: 10px; align-content: center; text-align: center;">
    <asp:Table ID="tblPerformanceLevels" runat="server" HorizontalAlign="Center"></asp:Table>
</div>
<asp:Label ID="lblNoRecords" Font-Size="20" runat="server" Width="100%" Visible="false" style="text-align: center; align-content: center;" Text="No Grade Conversion Available for This Test"></asp:Label>
<telerik:RadGrid ID="rgdGradeConversion" runat="server" style="margin-top: 20px;" Height="505" AllowSorting="true" OnColumnCreated="rgdGradeConversion_ColumnCreated" OnItemDataBound="rgdGradeConversion_ItemDataBound">
    <HeaderStyle Wrap="false" />
    <MasterTableView TableLayout="Fixed">
        <Columns></Columns>
    </MasterTableView>
    <ClientSettings>
        <Scrolling AllowScroll="true" UseStaticHeaders="true" />
    </ClientSettings>
</telerik:RadGrid>