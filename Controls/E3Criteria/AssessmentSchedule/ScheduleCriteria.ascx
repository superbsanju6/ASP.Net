<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="ScheduleCriteria.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.AssessmentSchedule.ScheduleCriteria" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx" %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server" />
<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" HideEvent="ManualClose" Skin="Black" Height="55" Width="245" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <table>
        <tr>
            <td width="100"><b>Assessment Category</b></td>
            <td>
                <telerik:RadComboBox runat="server" ID="cmbCategory" EmptyMessage="Select Category" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="True" ZIndex="8005" Skin="Vista" Width="140" DataTextField="Name" DataValueField="Value" DefaultTexts="District" LastSelected="" ></telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td><b>Schedule:</b></td>
            <td>
                <telerik:RadComboBox runat="server" ID="cmbSchedule" EmptyMessage="Select Schedule" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="True" ZIndex="8005" Skin="Vista" Width="140" DataTextField="Name" DataValueField="Value"  LastSelected="" ></telerik:RadComboBox>
            </td>
        </tr>
    </table>
</telerik:RadToolTip>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {

        DefaultSchedule: '<%= DefaultSchedule %>',
        DefaultCategory: '<%= DefaultCategory %>',
        ScheduleIsReadOnly: '<%= ScheduleReadOnly %>',
        CategoryIsReadOnly: '<%= CategoryReadOnly %>',

        SelectDefaultTexts: function () {
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";

            if (_this.DefaultCategory != "" || _this.DefaultSchedule != "") {

                if (_this.DefaultSchedule != "") {
                    var radComboBoxCtrl = $find("<%= cmbSchedule.ClientID %>");
                    radComboBoxCtrl.set_text(_this.DefaultSchedule);
                }

                if (_this.DefaultCategory != "") {
                    var radComboBoxCtrl = $find("<%= cmbCategory.ClientID %>");
                    var rcbItem = radComboBoxCtrl.findItemByText(_this.DefaultCategory);
                    if (rcbItem) {
                        rcbItem.set_selected(true);
                        radComboBoxCtrl.set_selectedItem(rcbItem);
                        radComboBoxCtrl.set_text(rcbItem.get_text());
                    }
                }

                var valueObject = _this.GetSelectedValues();
                if (valueObject.Category != "" || valueObject.ScheduleLevel != "")
                    CriteriaController.Add(criteriaName, valueObject);

            }

            if (_this.ScheduleIsReadOnly == 'True') _this.SetScheduleReadOnly();
            if (_this.CategoryIsReadOnly == 'True') _this.SetCategoryReadOnly();
        },

        SetScheduleReadOnly: function () {
            var radComboBoxCtrl = $find("<%= cmbSchedule.ClientID %>");
            $(radComboBoxCtrl).attr('disabled', true);

        },

        SetCategoryReadOnly: function () {
            var radComboBoxCtrl = $find("<%= cmbCategory.ClientID %>");
            radComboBoxCtrl.disable();
            //$(radComboBoxCtrl).attr('disabled', true);

        },


        PopulateControls: function(changedControl){

            //
            // Get references to the controls we will use.
            //
            var _this = <%=CriteriaName%>Controller;
            var cmbSchedule = $find("<%= cmbSchedule.ClientID%>");
            var cmbCategory = $find("<%= cmbCategory.ClientID%>");
            var data = <%=CriteriaName%>DependencyData;

            if (!data) {
                alert("Error finding the schedule criteria data.");
                return;
            }

            //
            // Populate cmbCategory dropdownlist control
            //
            _this.PopulateCombo(cmbCategory, CriteriaDataHelpers.GetFieldDistinct(data.CategoryData, 0).sort());

            //
            // Populate cmbSchedule dropdownlist control
            //
            _this.PopulateCombo(cmbSchedule, CriteriaDataHelpers.GetFieldDistinct(data.ScheduleLevelData, 0));

            if (_this.DefaultCategory != '' || _this.DefaultSchedule != '') _this.SelectDefaultTexts();
        },

        Clear: function () {
            //
            // Get references to the controls we will use.
            //
            var cmbCategory = $find("<%= cmbCategory.ClientID%>");
            var cmbSchedule = $find("<%= cmbSchedule.ClientID%>");

            cmbCategory.set_text(cmbCategory.get_emptyMessage());
            cmbSchedule.set_text(cmbSchedule.get_emptyMessage());

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
            // 
            //
            var valueObject = _this.GetSelectedValues();
            if (valueObject.Category != "" || valueObject.ScheduleLevel != "") {
                //CriteriaController.RemoveAll(criteriaName);
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        GetSelectedValues: function () {
            //
            // Get references to controls we will use.
            //
            var cmbCategory = $find("<%= cmbCategory.ClientID%>");
            var cmbSchedule = $find("<%= cmbSchedule.ClientID%>");

            //
            // Fetch the text values from these controls.
            //
            var category = cmbCategory.get_text();
            if (category == cmbCategory.get_emptyMessage()) category = "";

            var scheduleLevel = cmbSchedule.get_text();
            if (scheduleLevel == cmbSchedule.get_emptyMessage()) scheduleLevel = "";

            return { Category: category, ScheduleLevel: scheduleLevel };
        }
        
    }

</script>




<script id="ScheduleCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">
            {{if Value.Category != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Category:</B> {{:Value.Category}}</div>{{/if}}
            {{if Value.ScheduleLevel != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Schedule:</B> {{:Value.ScheduleLevel}}</div>{{/if}}
        </div>
    {{/for}}
</script>
