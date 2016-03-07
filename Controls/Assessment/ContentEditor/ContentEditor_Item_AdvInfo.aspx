<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentEditor_Item_AdvInfo.aspx.cs"
    Inherits="Thinkgate.Controls.Assessment.ContentEditor.ContentEditor_Item_AdvInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Advanced Item Information</title>
    <style>
        .tblMain
        {
            width: 100%;
            padding: 10px;
        }
        .tdLabel
        {
            font-weight: bold;
            width: 25%;
            padding: 10px;
        }
        .tdCombo
        {
            width: 25%;
            padding: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            <asp:ScriptReference Path="~/Scripts/master.js?d=1" />
        </Scripts>
    </telerik:RadScriptManager>
    <div>
        <table class="tblMain">
            <tr><td colspan="4"></td></tr>
            <tr>
                <td class="tdLabel">
                    Marzano:
                </td>
                <td class="tdCombo">
                    <telerik:RadComboBox ID="comboMarzano" ClientIDMode="Static" runat="server" Skin="Web20"
                        EmptyMessage="Select" field="Marzano" OnClientSelectedIndexChanged="updateRigor">
                    </telerik:RadComboBox>
                </td>
                <td class="tdLabel">
                    Item Difficulty:
                </td>
                <td class="tdLabel">
                    <asp:Literal runat="server" ID="Literal1"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="tdLabel">
                    RBT:
                </td>
                <td class="tdCombo">
                    <telerik:RadComboBox ID="comboRBT" ClientIDMode="Static" runat="server" Skin="Web20"
                        EmptyMessage="Select" field="RBT" OnClientSelectedIndexChanged="updateRigor">
                    </telerik:RadComboBox>
                </td>
                <td class="tdLabel">
                    Difficulty Index:
                </td>
                <td class="tdCombo">
                    <telerik:RadComboBox ID="comboDifficultyIndex" ClientIDMode="Static" runat="server"
                        Skin="Web20" EmptyMessage="Select" field="DifficultyIndex" OnClientSelectedIndexChanged="updateDifficultyIndex">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="tdLabel">
                    Webb:
                </td>
                <td class="tdCombo">
                    <telerik:RadComboBox ID="comboWebb" ClientIDMode="Static" runat="server" Skin="Web20"
                        EmptyMessage="Select" field="Webb" OnClientSelectedIndexChanged="updateRigor">
                    </telerik:RadComboBox>
                </td>
                <td>
                </td>
                <td class="tdCombo">
                </td>
            </tr>
        </table>
    </div>
    </form>
    <script type="text/javascript">
        var rigorCtrlName = 'combo<%= _Dok %>';

        function updateRigor(sender, args) {
            var comboWebb = $find('comboWebb').get_value();
            var comboRBT = $find('comboRBT').get_value();
            var comboMarzano = $find('comboMarzano').get_value();

            var context = getURLParm('qType');
            try {
                if (context == 'TestQuestion') {
                    parent.assessment_changeField('Rigor', $find(rigorCtrlName).get_text());

                } else {
                    var ustring = 'Webb=' + comboWebb + '|RBT=' + comboRBT + '|Marzano=' + comboMarzano + '|';
                    parent.assessment_changeField('RigorString', ustring);
                }
            }
            catch (e) {
                errormsg();
            }
        }
        function updateDifficultyIndex(sender, args) {
            // 2012-08-16 DHB - Back end piece to update testQuestion record with new 
            // difficulty index is expecting the text instead of the value.
            //var comboDifficultyIndex = $find('comboDifficultyIndex').get_value();
            var comboDifficultyIndex = $find('comboDifficultyIndex').get_text();

            var context = getURLParm('qType');
            try {
                    parent.assessment_changeField('DifficultyIndex', comboDifficultyIndex);
            }
            catch (e) {
                errormsg();
            }
        }
        function errormsg() {
            alert('There was an error saving your selection.');
        }
    </script>
</body>
</html>
