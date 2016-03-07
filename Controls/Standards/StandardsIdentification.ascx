<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardsIdentification.ascx.cs" Inherits="Thinkgate.Controls.Standards.StandardsIdentification" %>
    
<link href="~/Styles/Site.css" rel="stylesheet" type="text/css"/>
<div class="identificationTile-DivAroundTable">
    <table width="100%" class="fieldValueTable" >
        <tr>
            <td class="fieldLabel" style="width: 95px;">
                Standard Set:
            </td>
            <td>
                <asp:Label runat="server" ID="lblStandard_Set" />
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
                Level:
            </td>
            <td>
                <asp:Label runat="server" ID="lblLevel" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                State Number:
            </td>
            <td>
                <asp:Label runat="server" ID="lblStateNbr" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Keywords:
            </td>
            <td>
                <asp:Label runat="server" ID="lblKeywords" />
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
    </table>
</div>

