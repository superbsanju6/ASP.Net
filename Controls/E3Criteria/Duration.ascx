<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Duration.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.Duration" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>
<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>
<style>
    .durationBox {
        border: solid 1px #6788be; 
        color: #333; 
        font-size: 11.5px; 
        font-family: 'segoe ui', arial, sans-serif; 
        padding-bottom: 3px; 
        padding-top: 2px; 
        padding-left: 5px; 
        padding-right: 5px; 
    }
</style>
<telerik:RadToolTip ID="RadToolTip1" runat="server" Width="320" Skin="Black" EnableShadow="True" ShowEvent="OnClick" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <div style="position: relative">
        <div style="width: 300px;">
            <div style="width: 280px; float: left;">
                <asp:TextBox runat="server" ID="rtbDuration" MaxLength="3" ToolTip="Enter Time in Days" ClientIDMode="Static" CssClass="durationBox" Width="23px"  />&nbsp;Days&nbsp;
                <asp:DropDownList ID="DurationHours" runat="server" ToolTip="Enter Time in Hours" ClientIDMode="Static" CssClass="durationBox" Width="55px"  />&nbsp;Hours&nbsp;
                <asp:DropDownList ID="DurationMinutes" runat="server" ToolTip="Enter Time in Minutes" ClientIDMode="Static" CssClass="durationBox" Width="55px"  />&nbsp;Minutes						
            </div>
        </div>
    </div>
</telerik:RadToolTip>
<script type="text/javascript">

    var <%=CriteriaName%>Controller = {
        OnChange: function (sender, args) {
            if(sender != null)
                if (sender.get_attributes) {
                    if (sender.get_attributes().getAttribute("SuppressNextChangeEvent") == "true") {
                        // we are here just because we've cleared the dropdown and it fired the updated event. Suppress it
                        sender.get_attributes().setAttribute("SuppressNextChangeEvent", "false");
                        return;
                    }
                }
            var criteriaName = "<%=CriteriaName%>";
            var durationDays = $('#<%= rtbDuration.ClientID %>');
            var durationHours = $('#<%= DurationHours.ClientID %>');
            var durationMinutes = $('#<%= DurationMinutes.ClientID %>');
            var valueObject = {};
            valueObject.Text = durationDays.val() + ":" + durationHours.val() + ":" + durationMinutes.val();
            if ((durationDays.val() + ":" + durationHours.val() + ":" + durationMinutes.val()) == "0:0:0") {
                // we've cleared the text box, clear them
                CriteriaController.RemoveAll(criteriaName);
            } else {
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        RemoveByKeyHandler: function (criteriaName, value, calledFromAdd) {
            if (calledFromAdd) return;      // if we're calling this from an add, that means essentially we've changed the text or option. If I were to continue, it would clear the text box and dropdown which is not what I want because we obviously have a valid value in addition to the old removed one
            if (CriteriaController.CountValues(criteriaName) > 0) return;       // if we definitely have multiple values, again, we don't want to clear the text box and dropdown
            var durationDays = $('#<%= rtbDuration.ClientID %>');
            var durationHours = $('#<%= DurationHours.ClientID %>');
            var durationMinutes = $('#<%= DurationMinutes.ClientID %>');
            durationDays.value = "0";
            durationHours.value = "0";
            durationMinutes.value = "0";
        }
    };

</script>