<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextWithDropdown.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.TextWithDropdown" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" Width="400" Skin="Black" EnableShadow="True" ShowEvent="OnClick" HideEvent="LeaveTargetAndToolTip" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <div style="position: relative;">
        <div>Text Search</div>
        <div style="width: 350px;">
            <div style="width: 149px; float: left;">
                <telerik:RadTextBox ID="RadTextBox1" AutoPostBack="False" Skin="Vista" runat="server"></telerik:RadTextBox> 
            </div>
        </div>
        <div style="width: 149px; float: right; margin-right: 10px">
            <telerik:RadComboBox ID="RadComboBox1" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" runat="server"></telerik:RadComboBox>
        </div>
    </div>
</telerik:RadToolTip>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        OnChange: function(sender, args) {
            if (sender.get_attributes) {
                if (sender.get_attributes().getAttribute("SuppressNextChangeEvent") == "true") {
                    // we are here just because we've cleared the dropdown and it fired the updated event. Suppress it
                    sender.get_attributes().setAttribute("SuppressNextChangeEvent", "false");
                    return;
                }
            }
            var criteriaName = "<%=CriteriaName%>";
            var txtBox = $find("<%= RadTextBox1.ClientID %>");
            var comboBox = $find("<%= RadComboBox1.ClientID %>");
            var valueObject = { };
            valueObject.Text = txtBox.get_value();
            valueObject.Option = comboBox.get_text();
            if (txtBox.get_value() == "") {
                // we've cleared the text box, clear them
                CriteriaController.RemoveAll(criteriaName);
            } else {
                CriteriaController.Add(criteriaName, valueObject);
            }
        },
   
        RemoveByKeyHandler: function(criteriaName, value, calledFromAdd) {
            if (calledFromAdd) return;      // if we're calling this from an add, that means essentially we've changed the text or option. If I were to continue, it would clear the text box and dropdown which is not what I want because we obviously have a valid value in addition to the old removed one
            if (CriteriaController.CountValues(criteriaName) > 0) return;       // if we definitely have multiple values, again, we don't want to clear the text box and dropdown
            var txtBox = $find("<%= RadTextBox1.ClientID %>");
            var comboBox = $find("<%= RadComboBox1.ClientID %>");
            txtBox.set_value("");
            comboBox.get_attributes().setAttribute("SuppressNextChangeEvent", "true");
            comboBox.get_items().getItem(0).select();
        },
        
        OnKeyPress: function (sender, eventArgs) {
            var c = eventArgs.get_keyCode();
            if (c == 13) <%=CriteriaName%>Controller.OnChange(sender, eventArgs);
               
        }
    }

</script>
<script id="TextWithDropdownCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="{{:~getCSS(Applied, CurrentlySelected)}}">
            <div class="imgBeforeCriteria" onclick="CriteriaController.RemoveByKey('<%=CriteriaName%>', {{:Key}})"/>
            <div class="criteriaText">{{:Value.Option}}: {{:Value.Text}}</div>
        </div>
    {{/for}}
</script>