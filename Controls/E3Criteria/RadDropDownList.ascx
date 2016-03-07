<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RadDropDownList.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.RadDropDownList" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx" %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server" />
<%--CssClass="jqRadToolTip" is using for Jquery function  to disable and enable--%>
<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" Skin="Black" Height="55" Width="205" HideEvent="Default" EnableShadow="True" AutoCloseDelay="200000" Position="MiddleRight" RelativeTo="Element" CssClass="jqRadToolTip">
    <telerik:RadAjaxPanel ID="InnerPanel" runat="server">
        <telerik:RadComboBox runat="server" ID="RadComboBox1" AllowCustomText="True" MarkFirstMatch="True"
            EnableTextSelection="True" ZIndex="8005"
            Skin="Vista" Width="180" MaxHeight="200px">
        </telerik:RadComboBox>
    </telerik:RadAjaxPanel>
</telerik:RadToolTip>


<script type="text/javascript">
    
    var <%=CriteriaName%>Controller = {
        DefaultTexts: <%= DefaultTextAsJs() %>,
        CheckDefaultTexts: function () {
            var _this = <%=CriteriaName%>Controller;
            if (_this.DefaultTexts == null) return;
            var criteriaName = "<%=CriteriaName%>";
            var listBox = $find("<%= RadComboBox1.ClientID %>");
            for (var j=0; j < _this.DefaultTexts.length; j++) {
                var listItem = listBox.findItemByText(_this.DefaultTexts[j]);
                if (listItem) {
                    listItem.select();
                    CriteriaController.Add(criteriaName, _this.ValueObjectForItem(listItem));
                }
            }
        },
        
        OnChange: function (sender, args) {            
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";         
            var item = args.get_item();
            if(item !=null && item.get_value().length > 0)
            {
                var valueObject = _this.ValueObjectForItem(item);
                CriteriaController.Add(criteriaName, valueObject);          
            }
            else
            {                   
                CriteriaController.RemoveAll(criteriaName);            
            }
            //var comboBox = sender;
            <%=OnChange%>;            
        },
        
        ValueObjectForItem: function(item, alternateText, alternateValue) {
            var valueObject = { };
            valueObject.Text = alternateText ? alternateText :  ( item !=null ? item.get_text() : '');
            valueObject.Value = alternateValue ? alternateValue : ( item !=null ? item.get_value() : '');
            return valueObject;
        },
        
        RemoveByKeyHandler: function(criteriaName, value, calledFromAdd) {
            if (calledFromAdd) return;
            var comboBox = $find("<%= RadComboBox1.ClientID %>");
            if (comboBox.get_text() == value.Text)
            {
                comboBox.clearSelection();
                //comboBox.set_text(comboBox.get_emptyMessage() || "");
            }
           <%=OnRemoveByKeyHandler%>
        },
        
        PopulateList: function (arry, text_pos, value_pos) {
            //alert('populate <%=CriteriaName%>');
            // text_pos and value_pos are optional, and is specific to situations where arry is an array of arrays
            var combo = $find("<%= RadComboBox1.ClientID %>");
            combo.clearItems();
            for (var j = 0; j < arry.length; j++) {
                var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                if (text_pos != null && value_pos != null) {
                    comboItem.set_text(arry[j][text_pos]);
                    comboItem.set_value(arry[j][value_pos]);
                } else {
                    comboItem.set_text(arry[j]);
                    comboItem.set_value(arry[j]);
                }
                combo.get_items().add(comboItem);
            }
            if (!combo.findItemByText(combo.get_text()) && combo.get_text() != combo.get_emptyMessage()) {
                CriteriaController.RemoveAll('<%=CriteriaName%>');
                combo.set_text(combo.get_emptyMessage() || "");
            }
        },
        
        CheckComboForSelectedValueAfterDependencyChange: function () {
            var combo = $find("<%= RadComboBox1.ClientID %>");
            if (!combo.findItemByText(combo.get_text()) && combo.get_text() != combo.get_emptyMessage()) {
                CriteriaController.RemoveAll('<%=CriteriaName%>');
                combo.set_text(combo.get_emptyMessage() || "");
            }
        },
        
        InitialLoad: function() {
            var dataPos = 0;
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;
            var targetData = CriteriaDataHelpers.GetFieldDistinct(data, dataPos);
            _this.PopulateList(targetData);
        },
        
        requestItems: function(text, append) {
            var comboBox = $find("<%= RadComboBox1.ClientID %>");
            comboBox.requestItems(text, append);
        },
        
        CloseTooltip: function() {
            var tooltip = $find("<%= RadToolTip1.ClientID %>");
            tooltip.hide();
        },
        
        Clear: function() {
            var comboBox = $find("<%= RadComboBox1.ClientID %>");            
            comboBox.set_text(comboBox.get_emptyMessage());            
        }
    }   
    
</script>
