<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemAdvanced.ascx.cs" Inherits="Thinkgate.Controls.Items.ItemAdvanced" %>
<link href="~/Styles/Site.css" rel="stylesheet" type="text/css"/>
<div style="height: 231px;overflow-y:auto">
    <table runat="server" id="tblItemAdvanced" width="100%" class="fieldValueTable">
        <tr>
            <td class="fieldLabel" style="width:120px ">
                <asp:Label runat="server" ID="lblRigor" />
            </td>
            <td>
                <asp:Label runat="server" ID="lblRigorLevel" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Item Difficulty:
            </td>
            <td>
                <asp:Label runat="server" ID="lblItemDifficulty" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Difficulty Index:
            </td>
            <td>
                <asp:Label runat="server" ID="lblDifficultyIndex" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel" colspan="2">
                Distractor Rationale
            </td>
        </tr>
    </table>
</div>
