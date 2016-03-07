<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UITable.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.Table" %>
<table style="margin-left: auto; margin-right: auto" cellspacing="0" cellpadding="0">
    <tr>
        <td style="font-weight: bold; font-size: 11pt; text-decoration: underline; padding-right: 20px;">Associations</td>
        <td>
            <asp:ImageButton ID="btnClearCriteria" runat="server" OnClientClick="return resetStandards();" ImageUrl="~/Images/clear-1.png" Style="font-weight: bold; margin-top: 7px; margin-bottom: 5px; outline: none;" />
        </td>
    </tr>
</table>
<script type="text/javascript" lang="js">
    function resetStandards() {
        for (var i = 0; i < searchControlSchema.length; i++) {
            var criteria = searchControlSchema[i];
            if (criteria) {
                if (criteria.Key == "Standards" || criteria.Key == "Curriculum" || criteria.Key == "Documents" || criteria.Key == "Tags") {
                    if (!criteria.Locked) {

                        criteria.Value.Key = "";
                        criteria.Value.Value = "";

                        var selected = $("div[id*=divToolTipTarget_" + criteria.Key + "]")
                        selected.hide("fast");

                        if (criteria.UIType == '1') {
                            selected.find("input[type=checkbox]").each(function (i, e) {
                                e.checked = false;
                            });
                        }
                        else if (criteria.UIType == '2') {
                            selected.find("select").each(function (i, e) {
                                if (e.options.length > 0) {
                                    e.selectedIndex = 0;
                                }
                            });
                        }
                        else if (criteria.UIType == '3') {
                            selected.find("input[type=text]").each(function (i, e) {
                                e.value = '';
                            });
                            selected.find("select").each(function (i, e) {
                                if (e.options.length > 0) {
                                    e.selectedIndex = 0;
                                }
                            });
                        }
                        else if (criteria.UIType == '5') {
                            var tagFrame = top.findFrameWindowByUrl('ResourceSearchKentico.aspx');
                            var tagCriteria = $(tagFrame.document).find("#Tags_SelectedTagValues").val("");
                            // defined in Tag control
                            clearTagSelections();
                        }
                        else if (criteria.UIType == '7' || criteria.UIType == '8') {
                            selected.find("select").each(function (i, e) {
                                if (e.options.length > 0) {
                                    e.selectedIndex = 0;
                                }
                                if (criteria.UIType == '7') {
                                    if (i > 0) {
                                        if (i == 1) {
                                            //clearDDL(e, "Select Subject");
                                            e.selectedIndex = 0;
                                        }
                                        else if (i == 2) {
                                            //clearDDL(e, "Select Curriculum");
                                            e.selectedIndex = 0;
                                        }
                                    }
                                }
                                else if (criteria.UIType == '8') {
                                    if (i > 0) {
                                        if (i == 1) {
                                            clearDDL(e, "Select Grade");
                                        }
                                        else if (i == 2) {
                                            clearDDL(e, "Select Subject");
                                        }
                                        else if (i == 3) {
                                            clearDDL(e, "Select Course");
                                        }
                                        else if (i == 4) {
                                            clearDDL(e, "Select Standards");
                                        }
                                    }
                                }
                            });
                        }

                        criteria.IsUpdatedByUser = true;
                        $("#divToolTipTarget_" + criteria.Key).dialog("close");
                    }
                }
            }
        }
        $("#hdnSearchControlSchema").val(JSON.stringify(searchControlSchema));
        return false;
    }
</script>
