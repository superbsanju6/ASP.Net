<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddNewItem.aspx.cs" Inherits="Thinkgate.Controls.Items.AddNewItem"
    ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4"
        rel="stylesheet" type="text/css" runat="server" />
    <title>Identification</title>
    <base target="_self" />
    <style type="text/css">
        .editorTable
        {
            text-align: left;
            font-size: 12pt;
        }
        
        .editorLabel
        {
            font-weight: bold;
            padding-top: 25px;
        }
        
        .editorControl
        {
            padding-left: 40px;
            padding-top: 25px;
        }
        .roundButtons
        {
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            position: relative;
            border: solid 1px #000;
            border-radius: 50px;
            float: right;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
        }
        .RadComboBox_Web20 .rcbInputCellLeft, .RadComboBox_Web20 .rcbInputCellRight, .RadComboBox_Web20 .rcbArrowCellLeft, .RadComboBox_Web20 .rcbArrowCellRight
        {
            /*background-image: url('../Images/rcbSprite.png') !important;*/
        }
        
        .RadComboBox_Web20 .rcbHovered .rcbInputCell .rcbInput, .RadComboBox_Web20 .rcbFocused .rcbInputCell .rcbInput
        {
            color: #fff !important;
        }
        
        .RadComboBox_Web20 .rcbMoreResults a
        {
            /*background-image: url('../Images/rcbSprite.png') !important;*/
        }
        
        .hiddenUpdateButton
        {
            display: none;
        }

        .rbPrimaryIcon 
        {
            top: 5px !important; /*added for tfs bug id 9040*/ 
        }

        .rcbCheckBox
        {
            margin-right: 5px;
        }


    </style>
</head>

<body style="background-color: white; font-family: Arial, Sans-Serif; font-size: 10pt;">
    <form runat="server" id="mainForm" method="post">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
            <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
            <asp:ScriptReference Path="~/scripts/master.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
        Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    </telerik:RadAjaxManager>
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
    </telerik:RadSkinManager>
    <input id="inpItemType" type="hidden" value="0" runat="server" />
    <input id="inpxID" type="hidden" value="0" runat="server" />
    <input id="inpxIDEnc" type="hidden" value="0" runat="server" />
    <input id="inpxNewAndReturnType" type="hidden" value="0" runat="server" />
    <input id="inpxNewAndReturnID" type="hidden" value="0" runat="server" />
    <asp:Panel runat="server" Width="450">
        <table class="editorTable" style="margin-left: 40px;">
            <tr id="trGrade" runat="server">
                <td class="editorLabel">
                    <span>Grade:</span>
                </td>
                <td class="editorControl">
                    <telerik:RadComboBox ID="cmbGrade" runat="server" ToolTip="Select a grade" Skin="Web20"
                        Width="200" OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged" AutoPostBack="true"
                        CausesValidation="False" EmptyMessage="&lt;Select One&gt;" OnClientLoad="RadComboBox_FixEmptyMessage">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr id="trSubject" runat="server">
                <td class="editorLabel">
                    <span>Subject:</span>
                </td>
                <td class="editorControl">
                    <telerik:RadComboBox ID="cmbSubject" runat="server" ToolTip="Select a subject" Skin="Web20"
                        Width="200" OnSelectedIndexChanged="cmbSubject_SelectedIndexChanged" AutoPostBack="true"
                        CausesValidation="False" EmptyMessage="&lt;Select One&gt;" OnClientLoad="RadComboBox_FixEmptyMessage">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr id="trCourse" runat="server">
                <td class="editorLabel">
                    <span>Course:</span>
                </td>
                <td class="editorControl">
                    <telerik:RadComboBox ID="cmbCourse" runat="server" ToolTip="Select a standard course"
                        Skin="Web20" Width="200" OnSelectedIndexChanged="cmbCourse_SelectedIndexChanged"
                        AutoPostBack="true" CausesValidation="False" EmptyMessage="&lt;Select One&gt;"
                        OnClientLoad="RadComboBox_FixEmptyMessage">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr id="trAddendumType" runat="server">
                <td class="editorLabel">
                    <span>Type:</span>
                </td>
                <td class="editorControl">
                    <telerik:RadComboBox ID="cmbAddendumType" runat="server" ToolTip="Select an addendum type"
                        Skin="Web20" Width="200" OnSelectedIndexChanged="cmbAddendumType_SelectedIndexChanged"
                        AutoPostBack="true" CausesValidation="False" EmptyMessage="&lt;Select One&gt;"
                        OnClientLoad="RadComboBox_FixEmptyMessage">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr id="trAddendumGenre" runat="server">
                <td class="editorLabel">
                    <span>Genre:</span>
                </td>
                <td class="editorControl">
                    <telerik:RadComboBox ID="cmbAddendumGenre" runat="server" ToolTip="Select an assessment genre"
                        Skin="Web20" Width="200" OnSelectedIndexChanged="cmbAddendumGenre_SelectedIndexChanged"
                        AutoPostBack="true" CausesValidation="False" EmptyMessage="&lt;Select One&gt;"
                        OnClientLoad="RadComboBox_FixEmptyMessage">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr id="trName" runat="server">
                <td class="editorLabel">
                    <span>Name:</span>
                </td>
                <td class="editorControl">
                    <telerik:RadTextBox ID="tbxName" runat="server" Skin="Web20" Width="200" OnTextChanged="tbxName_TextChanged"
                        AutoPostBack="true">
                    </telerik:RadTextBox>
                </td>
            </tr>
            <tr id="trItemBanks" runat="server">
                <td class="editorLabel">
                    <span>Item Banks:</span>
                </td>
                <td class="editorControl">
                    <telerik:RadComboBox ID="cmbItemBanks" runat="server" ToolTip="Select an item bank"
                        Skin="Web20" Width="200" OnItemChecked="cmbItemBanks_ItemChecked"
                        AutoPostBack="true" CausesValidation="False" CheckBoxes="true" EmptyMessage="&lt;Select&gt;"
                     >  <%--OnClientLoad="RadComboBox_FixEmptyMessage"--%>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr id="trQuestionType" runat="server">
                <td class="editorLabel">
                    <span>ItemType:</span>
                </td>
                <td class="editorControl">
                    <telerik:RadComboBox ID="cmbQuestionType" runat="server" ToolTip="Select an item type"
                        Skin="Web20" Width="200" OnSelectedIndexChanged="cmbQuestionType_SelectedIndexChanged"
                        AutoPostBack="true" CausesValidation="False" EmptyMessage="&lt;Select One&gt;"
                        OnClientLoad="RadComboBox_FixEmptyMessage">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr id="trScoreType" runat="server">
                <td class="editorLabel">
                    <span>Score Type:</span>
                </td>
                <td class="editorControl">
                    <telerik:RadComboBox ID="cmbScoreType" runat="server" ToolTip="Select a scoring type"
                        Skin="Web20" Width="200" OnSelectedIndexChanged="cmbScoreType_SelectedIndexChanged"
                        AutoPostBack="true" CausesValidation="False" EmptyMessage="&lt;Select One&gt;"
                        OnClientLoad="RadComboBox_FixEmptyMessage">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr id="trCopyright" runat="server">
                <td class="editorLabel">
                    <a href="#" onclick="showCopyright()">Copyright:</a>
                </td>
                <td class="editorControl">
                    <telerik:RadButton ID="cbxYes" runat="server" Text="Yes" ButtonType="ToggleButton"
                        ToggleType="Radio" GroupName="Copyright" Font-Size="12pt" Skin="Web20">
                    </telerik:RadButton>
                    <telerik:RadButton ID="cbxNo" runat="server" Text="No" ButtonType="ToggleButton" Checked="True"
                        ToggleType="Radio" GroupName="Copyright" Font-Size="12pt" Skin="Web20" >
                    </telerik:RadButton>

                </td>
                </tr>
                
        </table>
        <br />
        <asp:Panel ID="Panel2" runat="server" Style="float: right; margin-right: 4px; margin-top: 10px;">
            <asp:Button runat="server" ID="btnContinue" ClientIDMode="Static" CssClass="roundButtons"
                Enabled="false" Width="100" Text="Continue" OnClick="btnContinue_Click" />
            <asp:Button runat="server" ID="btnCancel" ClientIDMode="Static" CssClass="roundButtons"
                Width="100" Text="Cancel" OnClientClick="cancelConfirm(); return false;" />
        </asp:Panel>
    </asp:Panel>
    <script type="text/javascript">
        // If we have an xid when the window loads, a new item has been created.
        // In this case we need to show the proper item content editor.
        window.onload = function () {
            $('.editorLabel').bind("contextmenu", function (e) {
                e.preventDefault();
            });
            var xIDele = document.getElementById("inpxID");
            var xID = xIDele.value;
            if (xID == '0')
                return;

            var xIDeleEnc = document.getElementById("inpxIDEnc");
            var xIDEnc = xIDeleEnc.value;

            var inpxNewAndReturnType = document.getElementById("inpxNewAndReturnType").value;
            var inpxNewAndReturnID = document.getElementById("inpxNewAndReturnID").value;

            // We just call customDialog with this pages' name to replace it with a new dialog.
            var itemType = getItemType();
            switch (itemType) {

                case 'Image':
                    parent.customDialog({ url: (appClient + 'Controls/Assessment/ContentEditor/ContentEditor_Image.aspx?isNew=Yes&xID=' + xIDEnc + '&NewAndReturnType=' + inpxNewAndReturnType + '&NewAndReturnID=' + inpxNewAndReturnID), maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorIMAGE', title: 'Content Editor - IMAGE', destroyOnClose: true });
                    break;

                case 'Item':
                    parent.customDialog({ url: (appClient + 'Controls/Assessment/ContentEditor/ContentEditor_Item.aspx?isNew=Yes&xID=' + xIDEnc + '&NewAndReturnType=' + inpxNewAndReturnType + '&NewAndReturnID=' + inpxNewAndReturnID), maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorITEM', title: 'Content Editor - ITEM', destroyOnClose: true });
                    break;

                case 'Addendum':
                    parent.customDialog({ url: (appClient + 'Controls/Assessment/ContentEditor/ContentEditor_Addendum.aspx?isNew=Yes&xID=' + xIDEnc + '&NewAndReturnType=' + inpxNewAndReturnType + '&NewAndReturnID=' + inpxNewAndReturnID), maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorADDENDUM', title: 'Content Editor - ADDENDUM', destroyOnClose: true });
                    break;

                case 'Rubric':
                    parent.customDialog({ url: (appClient + 'Controls/Assessment/ContentEditor/ContentEditor_Rubric.aspx?isNew=Yes&xID=' + xIDEnc + '&NewAndReturnType=' + inpxNewAndReturnType + '&NewAndReturnID=' + inpxNewAndReturnID), maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorRUBRIC', title: 'Content Editor - RUBRIC', destroyOnClose: true });
                    break;


                case 'RubricHolistic':
                    parent.customDialog({ url: (appClient + 'Controls/Assessment/ContentEditor/ContentEditor_Rubric.aspx?Type=Holistic&isNew=Yes&xID=' + xIDEnc + '&NewAndReturnType=' + inpxNewAndReturnType + '&NewAndReturnID=' + inpxNewAndReturnID), maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorRUBRIC', title: 'Content Editor - RUBRIC', destroyOnClose: true });
                    break;

                case 'RubricAnalytical':
                    parent.customDialog({ url: (appClient + 'Controls/Assessment/ContentEditor/ContentEditor_Rubric.aspx?Type=Analytical&isNew=Yes&xID=' + xIDEnc + '&NewAndReturnType=' + inpxNewAndReturnType + '&NewAndReturnID=' + inpxNewAndReturnID), maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorRUBRIC', title: 'Content Editor - RUBRIC', destroyOnClose: true });
                    break;

            }

            closeCurrentCustomDialog();
        }

        function getItemType() {
            var ele = document.getElementById("inpItemType");
            return ele.value;
        }

        function cancelConfirm() {
            var wnd = radconfirm('Are you sure you want to cancel?', confirmCallback, 330, 100, null, 'Cancel Add ' + getItemType());
        }

        function confirmCallback(arg) {
            if (arg == true)
                closeCurrentCustomDialog();
        }

        function showCopyright() {
            //var win = customDialog({ maximize: true, maxwidth: 400, maxheight: 200, autoSize: true, content: 'You must honor all copyright protection notifications on all material used within Elements. Only use content (Items, Distractors, Images, Addendums, Assignments) that your school system has purchased the rights to reproduce or that has been marked as public domain. Your are responsible for any copyright infringements. If in doubt about the content you wish to use, contact your central office for permission or clarification.', dialog_style: 'alert' },
			//													 [{ title: 'Ok'}]);
            //if (win) {

            //    /* Set height and width explicitly as these were modified during dialog load and show. */
            //    win.set_width(400);
            //    win.set_height(200);

            //    /* Set modal background height and width, spanning to container. */
            //    $('.rwTable').css("table-layout", "fixed");
            //    $('.TelerikModalOverlay').css({ width: '100%', height: '100%' });

            //    win.center();
            //}
            parent.customDialog({ maximize: true, maxwidth: 400, maxheight: 200, autoSize: true, content: 'You must honor all copyright protection notifications on all material used within Elements. Only use content (Items, Distractors, Images, Addendums, Assignments) that your school system has purchased the rights to reproduce or that has been marked as public domain. Your are responsible for any copyright infringements. If in doubt about the content you wish to use, contact your central office for permission or clarification.', dialog_style: 'alert' },
																 [{ title: 'Ok' }]);
        }

        // Fix a problem with the RadComboBox where selecting an index of < 0 does not
        // cause the empty message to be displayed. It just leaves the current message selected.
        function RadComboBox_FixEmptyMessage(combo) {
            var selectedIdx = combo.get_selectedIndex();
            if (selectedIdx == null)
                combo.set_text(combo.get_emptyMessage());
        }


    </script>
    </form>
</body>
</html>
