<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="DifficultyRange.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.DifficultyRange" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" Width="320" Skin="Black" EnableShadow="True" ShowEvent="OnClick" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <div style="position: relative;">
        <div style="width: 300px;font-weight:bold">
        Difficulty (.20 - .90) 
        </div>
        <div style="width: 300px;">
            <div style="width: 85px; float: left;">
                <telerik:RadNumericTextBox ID="rntbStart" Label="Start" LabelWidth="30px" AutoPostBack="False" Width="70" Skin="Vista" runat="server"></telerik:RadNumericTextBox> 
            </div> <div style="width: 18px; float: left;">  </div> 
            <div style="width: 80px; float: left;">
                <telerik:RadNumericTextBox ID="rntbEnd" Label="End" LabelWidth="25px" AutoPostBack="False" Width="65" Skin="Vista" runat="server"></telerik:RadNumericTextBox> 
            </div>
        </div>
    </div>
</telerik:RadToolTip>

<script type="text/javascript">

    var DifficultyController = {
        OnChange: function(sender, args) {
            if (sender._element) {
                if (sender._element.getAttribute("SuppressNextChangeEvent") == "true") {
                    // we are here just because we've cleared the dropdown and it fired the updated event. Suppress it
                    sender._element.setAttribute("SuppressNextChangeEvent", "false");
                    return;
                }
            }
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";

            var type = sender._element.id.indexOf("Start", sender._element.id.length - 5) > 0 ? "Start" : "End";
            var values = CriteriaController.GetValues(criteriaName);
            if (values)
                for (var j=0; j < values.length; j++) {
                    if (values[j].Value.Type == type)
                        CriteriaController.Remove(criteriaName, values[j].Value);
                }

            var newval = args.get_newValue() + 0.0;
            if (newval < 0.20) {
                newval = 0.20;
            } else {
                if (newval > 0.90) {
                    newval = 0.90;
                } 
            }
            var valueObject = _this.ValueObjectForItem(newval, type);
            CriteriaController.Add(criteriaName, valueObject);
            <%=OnChange%>;
        },
    
        ValueObjectForItem: function(difficulty, type) {
            var valueObject = { };
            valueObject.Difficulty = difficulty;
            valueObject.Type = type;
            return valueObject;
        },
        
        RemoveByKeyHandler: function(criteriaName, value, calledFromAdd) {
            if (calledFromAdd) return;      // if we're calling this from an add, that means essentially we've changed the text or option. If I were to continue, it would clear the text box and dropdown which is not what I want because we obviously have a valid value in addition to the old removed one
            var values = CriteriaController.GetValues(criteriaName);
            if (values)
                for (var j=0; j < values.length; j++) {
                    if (values[j].Value.Type == value.Type && values[j].CurrentlySelected) return;
                }
            
            var rntbStart = $find("<%= rntbStart.ClientID %>");
            var rntbEnd = $find("<%= rntbEnd.ClientID %>");
            if (value.Type == 'Start') {
                rntbStart._element.setAttribute("SuppressNextChangeEvent", "true");
                rntbStart.clear();
            } else {
                rntbEnd._element.setAttribute("SuppressNextChangeEvent", "true");
                rntbEnd.clear();
            }
        }    
    };

</script>
<script id="DifficultyCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        {{if Value.Type == 'Start'}}
        <div class="{{:~getCSS(Applied, CurrentlySelected)}}">
            <div class="imgBeforeCriteria" onclick="CriteriaController.RemoveByKey('<%=CriteriaName%>', {{:Key}})"/>
            <div class="criteriaText">{{:Value.Type}}: {{:Value.Difficulty}}</div>
        </div>
        {{/if}}
    {{/for}}
    {{for Values}}
        {{if Value.Type == 'End'}}
        <div class="{{:~getCSS(Applied, CurrentlySelected)}}">
            <div class="imgBeforeCriteria" onclick="CriteriaController.RemoveByKey('<%=CriteriaName%>', {{:Key}})"/>
            <div class="criteriaText">{{:Value.Type}}: {{:Value.Difficulty}}</div>
        </div>
        {{/if}}
    {{/for}}
</script>