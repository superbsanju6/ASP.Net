<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DistrictInformation.ascx.cs" Inherits="Thinkgate.Controls.District.DistrictInformation" %>
<table width="100%" class="fieldValueTable">
    <tr>
        <td class="fieldLabel">
            District:
        </td>
        <td>
            <asp:Label runat="server" ID="lblDistrict" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Client ID:
        </td>
        <td>
            <asp:Label runat="server" ID="lblClientID" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Schools:
        </td>
        <td>
            <asp:Label runat="server" ID="lblSchools" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Teachers:
        </td>
        <td>
            <asp:Label runat="server" ID="lblTeachers" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Students:
        </td>
        <td>
            <asp:Label runat="server" ID="lblStudents" />
        </td>
    </tr>
</table>