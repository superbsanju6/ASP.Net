<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchoolIdentification.ascx.cs" Inherits="Thinkgate.Controls.School.SchoolIdentification" %>

<table width="100%" class="fieldValueTable">
    <tr>
        <td class="fieldLabel">
            Name:
        </td>
        <td>
            <asp:Label runat="server" ID="lblName" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            School ID:
        </td>
        <td>
            <asp:Label runat="server" ID="lblID" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Abbreviation:
        </td>
        <td>
            <asp:Label runat="server" ID="lblAbbreviation" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Phone:
        </td>
        <td>
            <asp:Label runat="server" ID="lblPhone" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Type:
        </td>
        <td>
            <asp:Label runat="server" ID="lblType" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Cluster:
        </td>
        <td>
            <asp:Label runat="server" ID="lblCluster" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Students:
        </td>
        <td>
            <asp:Label runat="server" ID="lblStudentCt" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Teachers:
        </td>
        <td>
            <asp:Label runat="server" ID="lblTeacherCt" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Classes:
        </td>
        <td>
            <asp:Label runat="server" ID="lblClassCt" />
        </td>
    </tr>
    <tr>
        <td class="fieldLabel">
            Grades:
        </td>
        <td>
            <asp:Label runat="server" ID="lblGrades" />
        </td>
    </tr>
</table>