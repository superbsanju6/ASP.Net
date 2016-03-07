<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CheckBoxList.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CheckBoxList" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" HideEvent="LeaveTargetAndToolTip" Skin="Black" Height="55" Width="205" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
        <telerik:RadListBox ID="RadListBox1" runat="server" CheckBoxes="true" AutoPostBack="False" Width="100%" CssClass="HeightLimited">
        </telerik:RadListBox>
</telerik:RadToolTip>



<script type="text/javascript">
    
    var <%=CriteriaName%>Controller = {
        DefaultTexts: <%= DefaultTextAsJs() %>,
        IsReadOnly: '<%= ReadOnly %>',
        CheckDefaultTexts: function () {
            var _this = <%=CriteriaName%>Controller;
            if (_this.DefaultTexts == null) return;
            var criteriaName = "<%=CriteriaName%>";
            var listBox = $find("<%= RadListBox1.ClientID %>");
            for (var j=0; j < _this.DefaultTexts.length; j++) {
                var listItem = listBox.findItemByText(_this.DefaultTexts[j]);
                if (listItem) {
                    CriteriaController.Add(criteriaName, _this.ValueObjectForItem(listItem));
                    listItem.check();
                }
            }
            if (_this.IsReadOnly == 'True') _this.SetReadOnly();
        },
        
        RemoveByKeyHandler: function(criteriaName, value) {
            var listBox = $find("<%= RadListBox1.ClientID %>");
            var chkbox = listBox.findItemByValue(value.Value);
            if (chkbox) chkbox.uncheck();
            <%=OnChange%>;
        },
        
        ValueObjectForItem: function(item, alternateText, alternateValue) {
            var valueObject = { };
            valueObject.Text = alternateText ? alternateText : item.get_text();
            valueObject.Value = alternateValue ? alternateValue : item.get_value();
            return valueObject;
        },

        OnCheck: function(sender, args) {
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";
            var item = args.get_item();
            var valueObject = _this.ValueObjectForItem(item);
            if (item.get_checked()) {
                CriteriaController.Add(criteriaName, valueObject);
            } else {
                CriteriaController.Remove(criteriaName, valueObject);
            }
            <%=OnChange%>;
        },
        
        SetReadOnly: function () {
            var listBox = $find("<%= RadListBox1.ClientID %>");
            $(listBox._element).find(':checkbox').attr('disabled', true);
            $(listBox._element).find('.rlbText').css('color', '#848484');
        },
        
        ClearHandler: function () {
            var _this = <%=CriteriaName%>Controller;
            return (_this.IsReadOnly != 'True');
        },
        
        PopulateList: function (arry) {
            var listBox = $find("<%= RadListBox1.ClientID %>");
            var selectedItems = listBox.get_checkedItems();
            var texts = [];
            for (var j = 0; j < selectedItems.length; j++)
                texts.push(selectedItems[j].get_text());
            listBox.get_items().clear();
            for (var j = 0; j < arry.length; j++) {
                var item = new Telerik.Web.UI.RadListBoxItem();
                item.set_text(arry[j]);
                var indexChecked = texts.indexOf(arry[j]);
                if (indexChecked != -1) {
                    texts.splice(indexChecked, 1);
                    item.check();
                }
                listBox.get_items().add(item);
            }
            for (var j = 0; j < texts.length; j++) {
                var _this = <%=CriteriaName%>Controller;
                var valueObject = _this.ValueObjectForItem(null, texts[j], texts[j]);
                CriteriaController.Remove("<%=CriteriaName%>", valueObject);
            }
        },

        populateTypesOnCall:function (arry, catName) {
            PopulateList(arry);              
        }
    };
    
</script>
