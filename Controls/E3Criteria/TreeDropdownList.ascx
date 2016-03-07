<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TreeDropdownList.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.TreeDropdownList" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx" %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server" />

<telerik:RadToolTip ID="RadToolTip1" runat="server" ClientIDMode="Static" ShowEvent="OnClick" Skin="Black" Height="55" Width="318" HideEvent="LeaveTargetAndToolTip" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <telerik:RadAjaxPanel ID="InnerPanel" runat="server">
        <telerik:RadDropDownTree runat="server" ID="ddlAlignments" DataTextField="CredentialAlignment" DataFieldParentID="ParentID" AutoPostBack="false"
            DataFieldID="ID" OnDataBound="ddlAlignments_DataBound" OnNodeDataBound="ddlAlignments_NodeDataBound" BackColor="White"
            DataValueField="ID" Width="300" OnClientEntryAdded="CriteriaTreeDropDownEntryAdded" OnEntryAdded="ddlAlignments_EntryAdded" DefaultMessage="Select Alignment" DefaultValue="0" ZIndex="8005" OnClientDropDownOpened="dropDownOpened" OnClientDropDownClosed="dropDownClosed" Skin="Vista">
            <DropDownSettings OpenDropDownOnLoad="false" Height="280" />
        </telerik:RadDropDownTree>

    </telerik:RadAjaxPanel>
</telerik:RadToolTip>


<script type="text/javascript">

    function dropDownOpened(sender, args)
    {
        var tooltip = $find("RadToolTip1");
        tooltip.set_hideEvent(Telerik.Web.UI.ToolTipHideEvent.FromCode);
    }

    function dropDownClosed(sender, args)
    {
        var tooltip = $find("RadToolTip1");
        tooltip.hide();
        tooltip.set_hideEvent(Telerik.Web.UI.ToolTipHideEvent.LeaveTargetAndToolTip);
    }

    function CriteriaTreeDropDownEntryAdded(sender, args) {
        var dropDown = sender;
        if (dropDown) {
            setTimeout(function () {
                dropDown.closeDropDown();
                var _this = <%=CriteriaName%>Controller;
                if (_this)
                {
                    _this.OnChange(sender, args);
                }
            }, 100);
        }
    }
    
    var <%=CriteriaName%>Controller = {
        DefaultTexts: <%= DefaultTextAsJs() %>,
        CheckDefaultTexts: function () {
            var _this = <%=CriteriaName%>Controller;
            if (_this.DefaultTexts == null) return;
            var criteriaName = "<%=CriteriaName%>";
            var listBox = $find("<%= ddlAlignments.ClientID %>");
            for (var j=0; j < _this.DefaultTexts.length; j++) {
                var listItem = listBox.findItemByText(_this.DefaultTexts[j]);
                if (listItem) {
                    listItem.select();
                    CriteriaController.Add(criteriaName, _this.ValueObjectForItem(listItem));
                }
            }
        },
        
        OnChange: function (sender, args) {
            var criteriaName = "<%=CriteriaName%>";
            var dropdowntree1 = $find("<%= ddlAlignments.ClientID %>");
            var _this = <%=CriteriaName%>Controller;
            var tree = dropdowntree1.get_embeddedTree();
            var selectedNodeValues = "";
            
            var selectedValue = sender.get_selectedValue();
            var selectedNode = tree.findNodeByValue(selectedValue);

            selectedNodeValues += selectedNode.get_value() + ",";
            if(selectedNode._hasChildren())
            {
                var children = selectedNode._getChildren()._array;
                
                $.each(children, function(index, value) {
                    selectedNodeValues += value.get_value() + ','
                });
            }
            
            var valueObject = { Text: sender.get_selectedText(), Value: selectedNodeValues.slice(0,-1) };
            CriteriaController.Add(criteriaName, valueObject);

            <%=OnChange%>;
        },
        
        ValueObjectForItem: function(item, alternateText, alternateValue) {
            var valueObject = { };
            valueObject.Text = alternateText ? alternateText : item.get_text();
            valueObject.Value = alternateValue ? alternateValue : item.get_value();
            return valueObject;
        },
        
        RemoveByKeyHandler: function(criteriaName, value, calledFromAdd) {
            if (calledFromAdd) return;
            var comboBox = $find("<%= ddlAlignments.ClientID %>");
            if (comboBox.get_selectedText() == value.Text)
            {
                comboBox._entries.clear();
                comboBox.get_embeddedTree().findNodeByValue("0").select();
            }
            <%=OnChange%>;
        },
        
        PopulateList: function (arry, text_pos, value_pos) {
            //alert('populate <%=CriteriaName%>');
            // text_pos and value_pos are optional, and is specific to situations where arry is an array of arrays
            var combo = $find("<%= ddlAlignments.ClientID %>");
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
            var combo = $find("<%= ddlAlignments.ClientID %>");
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
            var comboBox = $find("<%= ddlAlignments.ClientID %>");
            comboBox.requestItems(text, append);
        },
        
        CloseTooltip: function() {
            var tooltip = $find("<%= RadToolTip1.ClientID %>");
            tooltip.hide();
        },
        
        Clear: function() {
            var comboBox = $find("<%= ddlAlignments.ClientID %>");            
            comboBox.set_text(comboBox.get_emptyMessage());            
        }
    }
   
</script>
