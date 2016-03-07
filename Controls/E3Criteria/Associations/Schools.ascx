<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Schools.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.Associations.Schools" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" Width="300" HideEvent="ManualClose" Skin="Black" Height="55" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <table>
       <tr>
           <td width="200"><b>School&nbsp;Type:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbSchoolType" EmptyMessage="Select Type" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140" ></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>School:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbSchool" EmptyMessage="Select School" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       <tr style='display:<%= (IncludeSchoolIdControl == true) ? "block" : "none"%>;'>
            <td><b>School&nbsp;ID:</b></td>
            <td><telerik:RadTextBox ID="txtSchoolId" AutoPostBack="False" Width="140" Skin="Vista" runat="server" ClientEvents-OnLoad="SchoolsController.PopulateControls"></telerik:RadTextBox> </td>
       </tr>
       
    </table>
</telerik:RadToolTip>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        PopulateControls: function (changedControl) {
            var schoolTypePos = 0;
            var schoolPos = 1;
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;
            if (!data) {
                alert("Error finding SchoolsDependencyData.");
                return;
            }
            var cmbSchoolType = $find("<%= cmbSchoolType.ClientID %>");
            var cmbSchool = $find("<%= cmbSchool.ClientID %>");
            var selected = _this.GetSelectedValues();
            _this.PopulateCombo(cmbSchoolType, CriteriaDataHelpers.GetFieldDistinct(data, schoolTypePos).sort());
            
            var filteredSchoolPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selected.SchoolType]);
            var filteredSchools = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredSchoolPos, schoolPos);
            _this.PopulateCombo(cmbSchool, filteredSchools.sort());
            
        },

        Clear: function () {
            var cmbSchoolType = $find("<%= cmbSchoolType.ClientID %>");
            var cmbSchool = $find("<%= cmbSchool.ClientID %>");
            
            cmbSchoolType.set_text(cmbSchoolType.get_emptyMessage());
            cmbSchool.set_text(cmbSchool.get_emptyMessage());

            if ("<%= IncludeSchoolIdControl %>" == "True") {
                var txtSchoolId = $find("<%= txtSchoolId.ClientID %>");
                txtSchoolId.set_value("");
            }
            
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
                this.OnChange(combo, null, true);
            }
        },

        OnChange: function (sender, args, fromPopulate) {
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";
            if (!fromPopulate) _this.PopulateControls(sender);
            CriteriaController.RemoveAll(criteriaName);     // clear whatever was there before

            var valueObject = _this.GetSelectedValues();
            if (valueObject.SchoolType != "" || valueObject.School != "" || valueObject.SchoolId != "") {
                AssociationsRemoveAll();
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        OnTextChanged: function (sender, args, fromPopulate) {
            var _this = <%=CriteriaName%>Controller;
            // TODO: Hack.
            var criteriaName = "Schools"; /*sender.get_attributes().getAttribute('CriteriaName');*/

            var valueObject = _this.GetSelectedValues();
            if (valueObject.SchoolType != "" ||
                valueObject.School != "" ||
                valueObject.SchoolId != "" ||
                valueObject.Section != "") {
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        GetSelectedValues: function () {
            var cmbSchoolType = $find("<%= cmbSchoolType.ClientID %>");
            var cmbSchool = $find("<%= cmbSchool.ClientID %>");
            var schoolType = cmbSchoolType.get_text();
            if (schoolType == cmbSchoolType.get_emptyMessage()) schoolType = "";
            var school = cmbSchool.get_text();
            if (school == cmbSchool.get_emptyMessage()) school = "";

            if ("<%= IncludeSchoolIdControl %>" != "True") {
                return { SchoolType: schoolType, School: school, SchoolId: "" };
            }
            else
            {
                var txtSchoolId = $find("<%= txtSchoolId.ClientID %>");
                var schoolId = txtSchoolId.get_value(); // todo: busting here... 
                return { SchoolType: schoolType, School: school, SchoolId: schoolId };
            }

            
        }
    }

</script>

<script id="SchoolsCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">
            {{if Value.SchoolType != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>School Type:</B> {{:Value.SchoolType}}</div>{{/if}}
            {{if Value.School != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>School:</B> {{:Value.School}}</div>{{/if}}
            {{if Value.SchoolId != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>School ID:</B> {{:Value.SchoolId}}</div>{{/if}}
        </div>
    {{/for}}
</script>
