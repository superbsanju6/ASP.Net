<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportCardStandard.ascx.cs"
    Inherits="Thinkgate.Controls.Reports.ReportCardStandard" %>


<table class="reportTable">
    <tr>
        <td>
            # Schools:
            <asp:Label runat="server" ID="lblSchoolCount" />
        </td>
        <td>
            # Teachers:
            <asp:Label runat="server" ID="lblTeacherCount" />
        </td>
        <td>
            # Classes:
            <asp:Label runat="server" ID="lblClassCount" />
        </td>
        <td>
            # Students:
            <asp:Label runat="server" ID="lblStudentCount" />
        </td>
    </tr>
</table>
<br />
<telerik:RadAjaxPanel ID="rgLoadingReportGridPanel" runat="server">
<div style="width: 100%; height: 25px;">
    <div style="float: left;"> <input type="checkbox" id ="htmlChkToggleGridColors" onchange=" if ($('#htmlChkToggleGridColors').is(':checked')) { if ($('#pPlevels')){$('#pPlevels').removeAttr('style'); 	}  $('.reportCardStandardItem').each(function() { $(this).attr('style','background-color: ' + $(this).attr('plevel') +';'); }); } else  { 	if ($('#pPlevels'))	{ $('#pPlevels').attr('style','display: none;');}  $('.reportCardStandardItem').each(function() { $(this).attr('style','background-color: #ffffff;'); }); } " />
    Show performance levels. </div>
    <div style="float: right; width:700px; text-align: center;"><asp:Panel ID="pPlevels" ClientIDMode="Static" runat="server" style="display: none;"></asp:Panel> </div>
</div>
   
<br />

<div class="tblContainer" style="width: 100%;">
    <div class="tblRow" style="">
        <div id="criteriaHolder" class="tblLeft" style="background-color: #d1c9ca; height: 400px; padding-top: 3px;">
            <asp:PlaceHolder ID="criteriaPlaceholder" runat="server"></asp:PlaceHolder>
        </div>
        <div id="criteriaScroller" class="tblMiddle" style="background-color: #d1c9ca;  text-align: center;">
        </div>
        <div class="tblRight" style="width: 100%;  vertical-align: top;">
        
            <div style="width: 100%; height: 100%; overflow: hidden;">
              <telerik:RadGrid runat="server" ID="reportGrid" AutoGenerateColumns="false" OnItemDataBound="reportGrid_ItemDataBound"
             Height="480" Width="780"  AllowFilteringByColumn="false">
            <ClientSettings>
                <Scrolling AllowScroll="true" FrozenColumnsCount="1" UseStaticHeaders="True" SaveScrollPosition="true"  />
            </ClientSettings>
        </telerik:RadGrid>
            </div>
        </div>
    </div>
</div>
</telerik:RadAjaxPanel>
