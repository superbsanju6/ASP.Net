<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DistrictCriteria.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.AssessmentSchedule.DistrictCriteria" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx" %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server" />
<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" HideEvent="ManualClose" Skin="Black" Height="55" Width="245" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <table>            
        <tr>
            <td><b>District:</b></td>
            <td>
                <telerik:RadComboBox runat="server" ID="cmbDistrict" EmptyMessage="Select District" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140" DataTextField="Name" DataValueField="Value"></telerik:RadComboBox>
            </td>
        </tr>
    </table>
</telerik:RadToolTip>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {

        PopulateControls: function (changedControl) {
            //
            // Get references to the controls we will use.
            //
            var _this = <%=CriteriaName%>Controller;
            var cmbDistrict = $find("<%= cmbDistrict.ClientID%>");

            //
            // Populate cmbDistrict dropdownlist control
            //
            if (!DistrictCrit_DistrictDependencyData) {
                alert("Error finding the schedule criteria's district data.");
                return;
            }
            _this.PopulateCombo(cmbDistrict, CriteriaDataHelpers.GetFieldDistinct(DistrictCrit_DistrictDependencyData, 0).sort());

        },

        Clear: function () {
            //
            // Get references to the controls we will use.
            //    
            var cmbDistrict = $find("<%= cmbDistrict.ClientID%>");

            // set empty message(s)
            cmbDistrict.set_text(cmbDistrict.get_emptyMessage());

            this.PopulateControls();
        },

        ClearHandler: function () {
            return false;
        },

        PopulateCombo: function (combo, arry) {
            combo.clearItems();
            for (var j = 0; j < arry.length; j++) {
                var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                comboItem.set_text(arry[j]);
                combo.get_items().add(comboItem);
            }

            if (!combo.findItemByText(combo.get_text())) {
                combo.set_text(combo.get_emptyMessage());
                this.OnComboChanged(combo, null, true);
            }
        },

        OnComboChanged: function (sender, args, fromPopulate) {
            //
            // Get References to controls we will use.
            //
            var _this = <%=CriteriaName%>Controller;

            var criteriaName = sender.get_attributes().getAttribute('CriteriaName');
            //if (!fromPopulate) _this.PopulateControls(sender);

            //
            // Clear whatever was there before.
            //
            CriteriaController.RemoveAll(criteriaName);

            //
            // If values entered, add them.
            //
            var valueObject = _this.GetSelectedValues();
            if (valueObject.District != "") {
                //CriteriaController.RemoveAll(criteriaName);
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        GetSelectedValues: function () {
            //
            // Get references to controls we will use.
            //
            var cmbDistrict = $find("<%= cmbDistrict.ClientID%>");

            //
            // Fetch the text values from these controls.
            //
            var district = cmbDistrict.get_text();
            if (district == cmbDistrict.get_emptyMessage()) district = "";

            return {District: district};
        }

    }

</script>




<script id="DistrictCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">
            {{if Value.District != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>District:</B> {{:Value.District}}</div>{{/if}}
        </div>
    {{/for}}
</script>
