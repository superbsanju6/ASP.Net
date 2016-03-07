<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecialPopulationUC.ascx.cs" Inherits="Thinkgate.Controls.Rating.SpecialPopulationUc" %>

<link rel="stylesheet" type="text/css" href="~/Styles/Ratings/SpecialPopulations.css" runat="server" />

<style type="text/css">
    #dvSP table {
        width: 100%;
    }
   
    .marginclass {margin-right:196px!important;}
                 
</style>

<div id="dvSP">
    <table>
        <tr>
            <td>
                <asp:Label ID="lblAge" CssClass="float-left" runat="server">Age</asp:Label>
            </td>
            <td>
                <asp:Label ID="lblGrade" CssClass="float-right marginclass" runat="server">Grade</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:DropDownList ClientIDMode="Static" Width="235" CssClass="dropdown float-left" ID="ddlAge" runat="server"></asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ClientIDMode="Static" Width="235" CssClass="dropdown float-right" ID="ddlGrade" runat="server"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <table>
                    <tr>
                        <asp:Repeater ID="rptSpecialPopulation" runat="server" OnItemDataBound="specialPopulations_ItemDataBound">
                            <ItemTemplate>
                                <td>
                                    <telerik:RadButton ID="chkSpecialPopulation" runat="server" Skin="Outlook" ButtonType="ToggleButton" Checked="false" 
                                        ToggleType="CheckBox" Enabled="true" AutoPostBack="false" Height="14"  Font-Size="12pt" ItemID='<%# Eval("ID") %>' Text='<%# Eval("Name") %>'></telerik:RadButton>
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </table>

            </td>
        </tr>
    </table>
</div>
