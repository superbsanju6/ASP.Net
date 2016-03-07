<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RadCheckBoxList.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.RadCheckBoxList" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx" %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server" />
<%--CssClass="jqRadToolTip" is using for Jquery function  to disable and enable--%>
<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" Skin="Black" Height="55" Width="205" HideEvent="Default" EnableShadow="True" AutoCloseDelay="200000" Position="MiddleRight" RelativeTo="Element" CssClass="jqRadToolTip">
    <telerik:RadAjaxPanel ID="InnerPanel" runat="server">
        <telerik:RadComboBox ID="RadListBox1" AllowCustomText="True" MarkFirstMatch="True"
            EnableTextSelection="True" runat="server" ZIndex="8005" Skin="Vista" Width="180">
        </telerik:RadComboBox>
    </telerik:RadAjaxPanel>
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
        
        RemoveByKeyHandler: function(criteriaName, value, calledFromAdd) {
            if (calledFromAdd) return;
            var listBox = $find("<%= RadListBox1.ClientID %>");
            var chkbox = listBox.findItemByValue(value.Value);
            if (chkbox) chkbox.uncheck();
            if (listBox.get_text() == value.Text)
            {
                listBox.clearSelection();
                //comboBox.set_text(comboBox.get_emptyMessage() || "");
            }
          <%=OnRemoveByKeyHandler%>
        },

        AddByCheckAll : function(sender, args)
        {
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";
            var items = $find("<%= RadListBox1.ClientID %>").get_items();                        
            for (var j = 0; j < items._array.length; j++)
            {
                var valueObject = _this.ValueObjectForItem(items._array[j]);
                if (items._array[j].get_checked()) {
                    CriteriaController.Add(criteriaName, valueObject);
                } else {
                    CriteriaController.Remove(criteriaName, valueObject);
                }
                
            }
            CriteriaController.UpdateCriteriaForSearch();
        },
        
        ValueObjectForItem: function(item, alternateText, alternateValue) {
            var valueObject = { };
            valueObject.Text = alternateText ? alternateText :  ( item !=null ? item.get_text() : '');
            valueObject.Value = alternateValue ? alternateValue : ( item !=null ? item.get_value() : '');
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

        OnChange: function (sender, args) {            
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";
            var item = args.get_item();
            var valueObject = _this.ValueObjectForItem(item);
            if(item !=null && item.get_value().length > 0)
            {
                CriteriaController.Add(criteriaName, valueObject);          
            }
            else
            {
                CriteriaController.RemoveAll(criteriaName);
            }
            //var comboBox = sender;
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
