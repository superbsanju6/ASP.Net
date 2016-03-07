<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Demographics.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.Demographics" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" Skin="Black" Height="55" Width="390" EnableShadow="True" HideEvent="LeaveTargetAndToolTip" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <div style="background-color: white; border: 1px solid #979797">
        <asp:Repeater ID="rptDropdown" runat="server" OnItemDataBound="BindComboValues">
            <ItemTemplate>
                <div style="margin-top: 5px; margin-left: 5px;">
                    <span style="width: 200px; float: left;"><%# Eval("Label")%>:</span>
                    <telerik:RadComboBox ID="demoCombo" runat="server" AutoPostBack="False" AllowCustomText="False" MarkFirstMatch="True"
                         Skin="Vista" ZIndex="8500" DataTextField="Abbreviation" DataValueField="Value" AppendDataBoundItems="True">
                    </telerik:RadComboBox>
		        </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rptRadio" runat="server" OnItemDataBound="BindRadios">
            <ItemTemplate>
                <div style="margin-top: 5px; margin-left: 5px;">
                    <span style="width: 200px; float: left;"><%# Eval("Label") %>:</span>
                    <div style="display: inline;">
                        <telerik:RadButton ID="bAll" runat="server" AutoPostBack="False" ToggleType="Radio" ButtonType="ToggleButton" Skin="Web20" Text="All" Value="-1" Checked="True"></telerik:RadButton>
                        <telerik:RadButton ID="bYes" runat="server" AutoPostBack="False" ToggleType="Radio" ButtonType="ToggleButton" Skin="Web20" Text="Yes" Value="Yes"></telerik:RadButton>
                        <telerik:RadButton ID="bNo" runat="server" AutoPostBack="False" ToggleType="Radio" ButtonType="ToggleButton" Skin="Web20" Text="No" Value="No"></telerik:RadButton>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        </div>
        <telerik:RadButton ID="bReset" runat="server" AutoPostBack="False" Skin="Web20" Text="Reset All" style="float: right"></telerik:RadButton>
    
</telerik:RadToolTip>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        OnChange: function (sender, args) {
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";
            if (args.get_item) {
                var item = args.get_item();
                var demoField = sender.get_attributes().getAttribute('DemoField');
                var demoLabel = sender.get_attributes().getAttribute('DemoLabel');
            } else if (sender.get_groupName) {
                var item = sender;
                var demoField = sender._element.getAttribute('DemoField');
                var demoLabel = sender._element.getAttribute('DemoLabel');
            }
            var valueObject = _this.ValueObjectForItem(item);
            valueObject.DemoField = demoField;
            valueObject.DemoLabel = demoLabel;
            var existingValues = CriteriaController.GetValues(criteriaName);
            if (existingValues) {
                for (var j = 0; j < existingValues.length; j++) {
                    if (existingValues[j].Value.DemoField == valueObject.DemoField) {
                        CriteriaController.RemoveByKey(criteriaName, existingValues[j].Key, true);
                    }
                }
            }
            if (valueObject.DemoValue != -1) CriteriaController.Add(criteriaName, valueObject);
            <%=OnChange%>;
        },
        
        ValueObjectForItem: function(item) {
            var valueObject = { };
            valueObject.DemoValueText = item.get_text();
            valueObject.DemoValue = item.get_value();
            return valueObject;
        },
        
        Clear: function() {
            $(".DemographicsFinder").each(function(index, elm) {
                var id = elm.getAttribute('id');
                var obj = $find(id);
                if (obj.get_items) {
                    (obj.get_items()).getItem(0).select();
                } else {
                    if (obj.get_value() == -1)
                        obj.set_checked(true);
                    else
                        obj.set_checked(false);
                }
            });
            CriteriaController.RemoveAll('Demographics');
        },
        
        
        ClearHandler: function () {
            this.Clear();
            return false;
        }
    }

</script>

<script id="DemographicsCriteriaValueDisplayTemplate" type="text/x-jsrender">
    
    {{for Values}}
        <div class="{{:~getCSS(Applied, CurrentlySelected)}}">
            <div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>{{:Value.DemoLabel}}:</B> {{:Value.DemoValueText}}</div>
        </div>
    {{/for}}
</script>
