<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="RubricIdentification.ascx.cs"
    Inherits="Thinkgate.Controls.Rubrics.RubricIdentification" %>
    
<link href="~/Styles/Site.css" rel="stylesheet" type="text/css"/>
<div style="height: 100%">
    <table width="100%" class="fieldValueTable" >
        <tr>
            <td class="fieldLabel" style="width:95px ">
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
                Name:
            </td>
            <td>
                <asp:Label runat="server" ID="lblName" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Item Banks:
            </td>
            <td>
                <asp:Label runat="server" ID="lblItemBanks" />
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
                Scoring:
            </td>
            <td>
                <asp:Label runat="server" ID="lblScoring" />
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
                Copyright:
            </td>
            <td>
                <asp:Label runat="server" ID="lblCopyright" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Source:
            </td>
            <td>
                <asp:Label runat="server" ID="lblSource" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Credit:
            </td>
            <td>
                <asp:Label runat="server" ID="lblCredit" />
            </td>
        </tr>
    </table>
</div>
