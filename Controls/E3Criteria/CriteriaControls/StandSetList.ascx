<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandSetList.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.StandSetList" %>

<style>
    .greenButton {
		background-color: rgb(91, 183, 91);
		border: 1px solid lightgray;
		border-width: 1px;
		color: black;
		cursor: pointer;
		display: inline-block;
		font-family: Verdana,Arial,sans-serif;
		font-size: 14px;
		line-height: 15px;
		text-decoration: none;
		padding: 12px;
		padding-top: 5px;
		padding-bottom: 5px;
		border-radius: 5px;
	}
    .redButton {
		background-color: rgb(218, 79, 73);
		border: 1px solid lightgray;
		border-width: 1px;
		color: white;
		cursor: pointer;
		display: inline-block;
		font-family: Verdana,Arial,sans-serif;
		font-size: 14px;
		line-height: 15px;
		text-decoration: none;
		padding: 12px;
		padding-top: 5px;
		padding-bottom: 5px;
		border-radius: 5px;
	}
    .okbuttonImage {
		cursor: pointer;
		display: inline-block;
		color: #fff;
		margin-top: 1px;
		width: 14px;
		height: 14px;
		line-height: 14px;
		text-align: center;
		vertical-align: text-top;
	}

	.cancelbuttonImage {
		cursor: pointer;
		display: inline-block;
		color: #fff;
		margin-top: 1px;
		width: 14px;
		height: 14px;
		line-height: 14px;
		text-align: center;
		vertical-align: text-top;
	}
    .removebutton {
        cursor: pointer
    }
</style>

<asp:Panel id="StandSetListPanel" runat="server" style="z-index: 100; position: absolute; top: 25px; left: 25px; background-color: whitesmoke; border: solid 1px gray; padding: 3px;">
    <div>
        <table width="525px" style="border-spacing: 0">
            <tr style="background-color: blue; font-family: arial; color:whitesmoke;">
                <td style="width: 500px">Standards</td>
                <td onclick="CloseStardardSetList('Cancel');" style="cursor: pointer">X</td>
            </tr>
        </table>
    </div>
    <div style="overflow-y: scroll; overflow-x: hidden; height: 400px; width: 525px">
        <asp:Table ID="StandardSetTable" runat="server" Width="500px">
            <asp:TableRow runat="server">
                <asp:TableCell ID="RemoveColumn" runat="server" ToolTip="Remove Standard Set" Width="50px" CssClass="removebutton"></asp:TableCell>
                <asp:TableCell ID="StandardDescription" runat="server"></asp:TableCell>
                <asp:TableCell ID="StandardId" runat="server" Width="1px"></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div style="width: 525px" align="right">
    <asp:LinkButton ID="StandsetSave" runat="server" CssClass="greenButton" OnClientClick="CloseStardardSetList('Save');" OnClick="Save_Button_Clicked" ><i class="okbuttonImage"></i>Save</asp:LinkButton>&nbsp;
    <asp:LinkButton ID="StandsetCancel" runat="server" CssClass="redButton" OnClientClick="CloseStardardSetList('Cancel');" OnClick="Cancel_Button_Clicked"><i class="cancelbuttonImage"></i>Cancel</asp:LinkButton>
        <asp:HiddenField ID="hidStandardAlignment" runat="server"/>
        <asp:HiddenField ID="hidStandardSetSelected" runat="server"/>
    </div>
</asp:Panel>
<script type="text/javascript">
    $("[id^=RemoveColumn_]").attr("onclick", "ToggleStandard(this);");

    function CloseStardardSetList(saveCancel) {
        if (saveCancel == 'Save') {
            var hldData = "";
            $('#StandardSetTable tr').each(function() {
                var thisRow = $(this);
                if(thisRow.find('td').eq(1).css('text-decoration') != 'line-through')
                    hldData += thisRow.find('td:eq(2)').html() + ",";
            });
            
            $('#hidStandardSetSelected').val(hldData.substr(0, hldData.length - 1));
        }

        $('#StandSetListPanel').toggle();
        $('#StandardSetTable tr').remove();
        
    }

    function ToggleStandard(elementObject) {
        var elementRecordNumber = elementObject.parentElement.parentElement.rowIndex;
        if ($('#StandardSetTable tr').eq(elementRecordNumber).find('td').eq(1).css('text-decoration') != 'line-through') {
            $('#StandardSetTable tr').eq(elementRecordNumber).find('td').eq(1).css('text-decoration', 'line-through');
        } else {
            $('#StandardSetTable tr').eq(elementRecordNumber).find('td').eq(1).css('text-decoration', '');
        }
    }
</script>
