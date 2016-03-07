<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentChecklistTile.ascx.cs" Inherits="Thinkgate.Controls.Student.StudentChecklistTile" %>
<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
<telerik:RadComboBox ID="cmbGrade" runat="server" Skin="Web20"
    Style="width:160px;" OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged" AutoPostBack="true"
    CausesValidation="False" EmptyMessage="Student Grade" HighlightTemplatedItems="true">
</telerik:RadComboBox>
<div id="msgStudentChecklist" runat="server" class="divadvisementchecklist"
    title="Advisement CheckList" visible="false" onclick="showAddNewStudentWindow()">   
    <table>
        <tr>
            <td>
                <img runat="server" src="../../Images/ClientLogos/ChecklistIcon.png" height="26" width="18" />
                <a id="lnkStudentChecklist" runat="server" href="#"></a>
            </td>           
            <td style="vertical-align: top; padding-left:4px;">
                <asp:Label runat="server" ID="lblStudentChecklist" Style="text-decoration: underline; color: blue;"></asp:Label>
            </td>
        </tr>
    </table> 
</div>
<div id="msgStudentGrade" runat="server" >
          <div style="padding: 60px 0px 0px 75px;">
            Please Select a Student Grade</div>          
</div> 

<script type="text/javascript">
    function showAddNewStudentWindow() {
        customDialog({ url: '../Controls/Student/StudentChecklist.aspx', maximize: true, resizable: true, title: 'Advisement Checklist', maxwidth: 930, maxheight: 675 });
    }
</script>