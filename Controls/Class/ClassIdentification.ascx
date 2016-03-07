<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassIdentification.ascx.cs" Inherits="Thinkgate.Controls.Class.ClassIdentification" %>
<style>
    .fieldValueTable td
{
        padding-bottom:0px;
}
</style>

<table width="100%" class="fieldValueTable">
    
    <tr >
        <td runat="server" id="tdCloneNotification" colspan="2" style="display: none; color: red; font-weight: bold;">
            This is a copy of an existing class.
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            School:
        </td>
        <td>
            <asp:Label runat="server" ID="lblSchool" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Grade:
        </td>
        <td>
            <asp:Label runat="server" ID="lblGrade" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Subject:
        </td>
        <td>
            <asp:Label runat="server" ID="lblSubject" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Course:
        </td>
        <td>
            <asp:Label runat="server" ID="lblCourse" />
        </td>
    </tr>
  <tr>
        <td class="fieldLabel">
            Section:
        </td>
        <td>
            <asp:Label runat="server" ID="lblSection" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Year:
        </td>
        <td>
            <asp:Label runat="server" ID="lblYear" />
        </td>
   </tr>
    <tr>
        <td class="fieldLabel">
            Semester:
        </td>
        <td>
            <asp:Label runat="server" ID="lblSemester" />
        </td>
   </tr>
    <tr>
        <td class="fieldLabel">
            Period:
        </td>
        <td>
            <asp:Label runat="server" ID="lblPeriod" />
        </td>
   </tr>
    <tr>
        <td class="fieldLabel">
            Block:
        </td>
        <td>
            <asp:Label runat="server" ID="lblBlock" />
        </td>
   </tr>
       <tr>
        <td class="fieldLabel">
            Retain on Resync:
        </td>
        <td>
            <asp:Label runat="server" ID="lblRetainOnResync" />
        </td>
   </tr>
</table>