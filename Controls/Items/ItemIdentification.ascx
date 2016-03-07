<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="ItemIdentification.ascx.cs"
    Inherits="Thinkgate.Controls.Items.ItemIdentification" %>
<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
<div class="identificationTile-DivAroundTable">
    <script type="text/javascript">
        function displayRubricCriteria() {
            var hdnRubricName = $('#ItemIDTile_hdnRubricName')[0].value;
            var hdnRubricContent = $('#ItemIDTile_hdnRubricContent')[0].value;
            var hdnRubricPageUrl = $('#ItemIDTile_hdnRubricPageUrl')[0].value;
            var outerHTMLStuff = "";
            
            if (hdnRubricPageUrl == null || hdnRubricPageUrl == "") {
                outerHTMLStuff = '<table><tr><td><div class="contentTile-PreviewLabel">###hdnRubricName###</div></td></tr><tr><td>###hdnRubricContent###</td></tr></table>';
            } else {
                outerHTMLStuff = '<table><tr><td><a href="###hdnRubricPageUrl###" target="_blank">###hdnRubricName###</a></td></tr><tr><td>###hdnRubricContent###</td></tr></table>';
                outerHTMLStuff = outerHTMLStuff.replace("###hdnRubricPageUrl###", hdnRubricPageUrl);
            }
            outerHTMLStuff = outerHTMLStuff.replace("###hdnRubricName###", hdnRubricName);
            outerHTMLStuff = outerHTMLStuff.replace("###hdnRubricContent###", hdnRubricContent);
        
            customDialog(
                            {   maxwidth: 600,
                                maxheight: 100,
                                content: outerHTMLStuff,
                                model: false,
                                maximize: true
                            }
                        );
        }
        
        function launchRubricPage() {
            var hdnRubricPageUrl = $('#ItemIDTile_hdnRubricPageUrl')[0].value;
            window.Open(hdnRubricPageUrl);
        }

</script>
    <table width="100%" class="fieldValueTable">
        <tr>
            <td class="fieldLabel" style="width: 120px">
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
                Item Banks:
            </td>
            <td>
                <asp:Label runat="server" ID="lblItemBanks" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Item Status:
            </td>
            <td>
                <asp:Label runat="server" ID="lblStatus" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Item Reservation:
            </td>
            <td>
                <asp:Label runat="server" ID="lblReservation" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Item Type:
            </td>
            <td>
                <asp:Label runat="server" ID="lblType" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Score Type:
            </td>
            <td>
                <asp:Label runat="server" ID="lblScoreType" />
            </td>
        </tr>
         <tr>
            <td class="fieldLabel">
                Anchor Item:
            </td>
            <td>
                <asp:Label runat="server" ID="lblAnchorItem" />
            </td>
        </tr>
            <tr runat="server" id="rowRubricType" style="display: none;">
                <td class="fieldLabel">
                    Rubric Type:
                </td>
                <td>
                    <asp:Label runat="server" ID="lblRubricType" />
                </td>
            </tr>
            <tr runat="server" id="rowRubricScoring" style="display: none;" >
                <td class="fieldLabel">
                    Rubric Scoring:
                </td>
                <td>
                    <a id="hlRubricScoring" href="javascript:void(0);" runat="server">
                        <asp:Label runat="server" ID="lblRubricScoring" />
                    </a>
                </td>
            </tr>
            <tr runat="server" id="rowRubricName" style="display: none;">
                <td class="fieldLabel">
                    Rubric:
                </td>
                <td>
                    <a id="hlRubricName" href="javascript:void(0);" runat="server">
                        <asp:Label runat="server" ID="lblRubricName" />
                    </a>
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
                Expiration Date:
            </td>
            <td>
                <asp:Label runat="server" ID="lblCopyRightExpiryDate" />
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
<input type="hidden" runat="server" id="ItemIDTile_hdnRubricPageUrl" ClientIDMode="Static"/>
<input type="hidden" runat="server" id="ItemIDTile_hdnRubricContent" ClientIDMode="Static"/>
<input type="hidden" runat="server" id="ItemIDTile_hdnRubricName" ClientIDMode="Static"/>