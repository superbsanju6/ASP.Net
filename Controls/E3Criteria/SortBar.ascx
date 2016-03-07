<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SortBar.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.SortBar" %>

<div id="topBar" style="padding: 2px; background-color: #F0F0F0; border: solid 1px #000;">
        <table style="width: 100%;">
            <tr>
                <td style="text-align: left; width: 1%; white-space: nowrap; font-weight: bold;">
                    <span runat="server" id="sortSpan">
                        Sort by:
                        <telerik:RadComboBox ID="sortByDropdown" runat="server" EmptyMessage="Sort By" OnClientSelectedIndexChanged="btnSortClick" DataTextField="Name" DataValueField="Name"
                            Skin="Web20" Width="100" AutoPostBack="false">
                       
                        </telerik:RadComboBox>
                    </span>
                </td>
                <td style="text-align: center;">
                </td>
                <td style="text-align: right; width: 25%;">
                    <asp:ImageButton runat="server" id="imgExcel" Width="20" style="cursor: pointer" ImageUrl="~/Images/commands/excel_button_edited.png" OnClick="ExcelButtonClick" AlternateText="Excel"/>          
                </td>
            </tr>
            <tr>
                <td style="text-align: left; width: 1%; white-space: nowrap; font-weight: bold;">
                </td>
                <td style="text-align: center;">
                </td>
                <td style="text-align: right; width: 25%;">
                </td>
            </tr>
        </table>
        <telerik:RadButton ID="btnSelectReturn" runat="server" Text="Select and Return" ToolTip="Add selected items to assessment"
            Skin="Web20" AutoPostBack="false" OnClientClicked="btnSelectReturn_Click"
            Style="font-weight: bold; margin-top: 2px;" Visible="False">
        </telerik:RadButton>
    </div>
    
<script type="text/javascript">
    function btnSortClick() {
        if (!SearchHasBeenRun) return;
        $find("AjaxPanelResults").ajaxRequest(CriteriaController.ToJSON());
    }
</script>