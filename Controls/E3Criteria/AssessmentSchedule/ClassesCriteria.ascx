<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="ClassesCriteria.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.AssessmentSchedule.ClassesCriteria" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx" %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server" />
<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" HideEvent="ManualClose" Skin="Black" Height="55" Width="245" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <table>
        <tr>
            <td width="100"><b>Teacher:</b></td>
            <td>                
                <telerik:RadTextBox runat="server" ID="txtTeacher" EmptyMessage="Input Teacher" Width="140" Skin="Vista"></telerik:RadTextBox>
            </td>
        </tr>        
        <tr>
            <td><b>Semester:</b></td>
            <td>
                <telerik:RadComboBox runat="server" ID="cmbSemester" EmptyMessage="Select Semester" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="True" ZIndex="8005" Skin="Vista" Width="140" DataTextField="Name" DataValueField="Value"></telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td><b>Period:</b></td>
            <td>
                <telerik:RadComboBox runat="server" ID="cmbPeriod" EmptyMessage="Select Period" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="True" ZIndex="8005" Skin="Vista" Width="140" DataTextField="Name" DataValueField="Value"></telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td><b>Section:</b></td>
            <td>
                <telerik:RadTextBox runat="server" ID="txtSection" EmptyMessage="Input Section" Width="140" Skin="Vista"></telerik:RadTextBox>
            </td>
        </tr>
        <tr>
            <td><b>Block:</b></td>
            <td>
                <telerik:RadTextBox runat="server" ID="txtBlock" EmptyMessage="Input Block" Width="140" Skin="Vista" ClientEvents-OnLoad="ClassesController.PopulateControls" ></telerik:RadTextBox>
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
            var txtTeacher = $find("<%= txtTeacher.ClientID%>");
            var cmbSemester = $find("<%= cmbSemester.ClientID%>");
            var cmbPeriod = $find("<%= cmbPeriod.ClientID%>"); 
            var txtSection = $find("<%= txtSection.ClientID%>");
            var txtBlock = $find("<%= txtBlock.ClientID%>");
                   
            //
            // Populate cmbSemester dropdownlist control
            //
            if (!ClassesCrit_SemesterDependencyData) {
                alert("Error finding the classes criteria's semester data.");
                return;
            }
            _this.PopulateCombo(cmbSemester, CriteriaDataHelpers.GetFieldDistinct(ClassesCrit_SemesterDependencyData, 0).sort());

            //
            // Populate cmbPeriod dropdownlist control
            //
            if (!ClassesCrit_PeriodDependencyData) {
                alert("Error finding the classes criteria's period data.");
                return;
            }
            _this.PopulateCombo(cmbPeriod, CriteriaDataHelpers.GetFieldDistinct(ClassesCrit_PeriodDependencyData, 0).sort());

         },

        Clear: function () {
            //
            // Get references to the controls we will use.
            //    
            var txtTeacher = $find("<%= txtTeacher.ClientID%>");
            var cmbSemester = $find("<%= cmbSemester.ClientID%>");
            var cmbPeriod = $find("<%= cmbPeriod.ClientID%>");
            var txtSection = $find("<%= txtSection.ClientID%>");
            var txtBlock = $find("<%= txtBlock.ClientID%>");

            // set empty message(s)
            txtTeacher.set_value(txtTeacher.get_emptyMessage());
            cmbSemester.set_text(cmbSemester.get_emptyMessage());
            cmbPeriod.set_text(cmbPeriod.get_emptyMessage());
            txtSection.set_value(txtSection.get_emptyMessage());
            txtBlock.set_value(txtBlock.get_emptyMessage());

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
            //
            // Clear whatever was there before.
            //
            CriteriaController.RemoveAll(criteriaName);

            //
            // If values entered, add them.
            //
            var valueObject = _this.GetSelectedValues();
            if (valueObject.Teacher != "" ||
                valueObject.Semester != "" ||
                valueObject.Period != "" ||
                valueObject.Block != "" ||
                valueObject.Section != "")
            {
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        OnTextChanged: function (sender, args, fromPopulate) {
            var _this = <%=CriteriaName%>Controller;
            // TODO: Hack.
            var criteriaName = "Classes"; /*sender.get_attributes().getAttribute('CriteriaName');*/
            CriteriaController.RemoveAll(criteriaName);

            var valueObject = _this.GetSelectedValues();
            if (valueObject.Teacher != "" ||
                valueObject.Semester != "" ||
                valueObject.Period != "" ||
                valueObject.Block != "" ||
                valueObject.Section != "") {
                CriteriaController.Add(criteriaName, valueObject);
            }
        },
        
        GetSelectedValues: function () {
            //
            // Get references to controls we will use.
            //
            var txtTeacher = $find("<%= txtTeacher.ClientID%>");
            var cmbSemester = $find("<%= cmbSemester.ClientID%>");
            var cmbPeriod = $find("<%= cmbPeriod.ClientID%>");
            var txtSection = $find("<%= txtSection.ClientID%>");
            var txtBlock = $find("<%= txtBlock.ClientID%>");
            
            //
            // Fetch the text values from these controls.
            //
            var teacher = txtTeacher.get_value();
            if (teacher == txtTeacher.get_emptyMessage()) teacher = "";

            var semester = cmbSemester.get_text();
            if (semester == cmbSemester.get_emptyMessage()) semester = "";

            var period = cmbPeriod.get_text();
            if (period == cmbPeriod.get_emptyMessage()) period = "";

            var section = txtSection.get_value();
            if (section == txtSection.get_emptyMessage()) section = "";

            var block = txtBlock.get_value();
            if (block == txtBlock.get_emptyMessage()) block = "";

            return {
                Teacher: teacher,
                Semester: semester,
                Period: period,
                Section: section,
                Block: block
            };
        }

    }

</script>




<script id="ClassesCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">
            {{if Value.Teacher != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Teacher:</B> {{:Value.Teacher}}</div>{{/if}}
            {{if Value.Semester != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Schedule:</B> {{:Value.Semester}}</div>{{/if}}
            {{if Value.Period != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Period:</B> {{:Value.Period}}</div>{{/if}}
            {{if Value.Section != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Section:</B> {{:Value.Section}}</div>{{/if}}
            {{if Value.Block != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Block:</B> {{:Value.Block}}</div>{{/if}}
        </div>
    {{/for}}
</script>
