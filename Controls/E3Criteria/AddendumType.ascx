<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddendumType.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.AddendumType" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" HideEvent="LeaveTargetAndToolTip" Skin="Black" Height="55" Width="375" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element" CssClass="HeightLimited">
        <div style="float: left">Type:<br/>
            <telerik:RadListBox ID="RadListBox1" runat="server" CheckBoxes="true" AutoPostBack="False" Width="120" CssClass="HeightLimited">
            </telerik:RadListBox>
        </div>
        <div style="float: left; margin-left: 10px;">Genre (passages only):<br/>
            <telerik:RadListBox ID="RadListBox2" runat="server" CheckBoxes="true" AutoPostBack="False" Width="215" CssClass="HeightLimited GrayDisabled">
            </telerik:RadListBox>
        </div>
</telerik:RadToolTip>

<script type="text/javascript">
    
    var <%=CriteriaName%>Controller = {
        
        InitialGenreHide: function() {
            // by default, we come in with the checkboxes in the 2nd list disabled
            var childListBox = $find("<%= RadListBox2.ClientID %>");
            var items = childListBox.get_items();
            // first we need to add Not Specified
            var item = new Telerik.Web.UI.RadListBoxItem();
            item.set_text("Not Specified");
            item.set_value("Not Specified");
            items.insert(0, item); 
            for (j=0; j < items.get_count(); j++) {
                items.getItem(j).set_enabled(false);   
            }
            
        },
        
        EnsurePassageSelection: function(key) {
            // essentially this is a logic cleanup routine.
            var listBox = $find("<%= RadListBox1.ClientID %>");
            var passagebox = listBox.findItemByValue("passage");
            var criteriaName = "<%=CriteriaName%>";
            var currentItems = CriteriaController.GetValues(criteriaName);
            var foundBasePassageItem = false;
            for (var j=currentItems.length-1; j >= 0; j--) {
                // the passage box is not checked but we have selected genres. Remove them
                if (!passagebox.get_checked() && currentItems[j].Value.Genre != null) {
                    if (!key)       // threw this in to prevent an endless loop since we call EnsurePassageSelection from RemoveByKey
                        CriteriaController.RemoveByKey(criteriaName, currentItems[j].Key);
                }
                if (currentItems[j].Value.Value == 'passage') foundBasePassageItem = true;      // flag if we have a criteria item selected for passage. Will be used later
            }
            var childListBox = $find("<%= RadListBox2.ClientID %>");
            if (childListBox.get_checkedItems().length > 0) {
                // if there are checked genres, then we don't want to leave the 'passage' criteria out there, so remove it. This however will not uncheck the check box which is what we want. In fact, we will ensure the passage checkbox is checked
                CriteriaController.Remove(criteriaName, this.ValueObjectForItem(passagebox));
                passagebox.set_checked(true);       // had to add this in because if you search for passages, then select a genre, the on the next search it would remove the passages criteria (as it should), but also clears the checkbox
            } else {
                // by the oppoosite token, if the passage check box is checked, but we don't have a criteria item for it AND none of the genres are selected, we need to add back in a criteria for passage
                if (passagebox.get_checked() && !foundBasePassageItem) CriteriaController.Add(criteriaName, this.ValueObjectForItem(passagebox));
            }
            
            // now we just set enabled/disabled on the genres based on the passage box
            var items = childListBox.get_items();
            for (j=0; j < items.get_count(); j++) {
                items.getItem(j).set_enabled(passagebox.get_checked());   
            }
        },
        
        RemoveByKeyHandler: function(criteriaName, value, calledFromAdd, key) {
            // need to determine if we are removing a genre or type so we can clear the checkbox. Then we need to call the logic cleanup routine to verify things
            if (value.Genre != null) {
                var listBox = $find("<%= RadListBox2.ClientID %>");
                var lookup_val = value.Genre;
            } else {
                var listBox = $find("<%= RadListBox1.ClientID %>");
                var lookup_val = value.Value;
            }
            var chkbox = listBox.findItemByValue(lookup_val);
            if (chkbox) chkbox.uncheck();
            this.EnsurePassageSelection(key);
        },
        
        ClearHandler: function() {
            // need to make sure we uncheck passage box because otherwise passage will get added back in as criteria by EnsurePassageSelection
            var listBox = $find("<%= RadListBox1.ClientID %>");
            var passagebox = listBox.findItemByValue("passage");
            passagebox.uncheck();
            return true;
        },
        
        ValueObjectForItem: function(item, alternateText, alternateValue) {
            var childListBox = $find("<%= RadListBox2.ClientID %>");
            var valueObject = { };
            if (item.get_listBox() == childListBox) {
                valueObject.Text = 'passage: ' + item.get_text();
                valueObject.Value = null;
                valueObject.Genre = item.get_value();
            } else {
                valueObject.Text = alternateText ? alternateText : item.get_text();
                valueObject.Value = alternateValue ? alternateValue : item.get_value();
                valueObject.Genre = null;
            }
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
            _this.EnsurePassageSelection();
        }
        
    };

    
</script>

<style type="text/css">
    .GrayDisabled .rlbDisabled .rlbText {
        color: #848484;
    }
</style>