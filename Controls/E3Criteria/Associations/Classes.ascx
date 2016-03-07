<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Classes.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.Associations.Classes" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" HideEvent="ManualClose" Skin="Black" Height="55" Width="245" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <table>
       <tr>
            <td width="100"><b>Grade:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbGrade" EmptyMessage="Select Grade" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140" ></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>Subject:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbSubject" EmptyMessage="Select Subject" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>Course:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbCourse" EmptyMessage="Select Course" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       
    </table>
</telerik:RadToolTip>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        PopulateControls: function (changedControl) {
            var gradePos = 0;
            var subjectPos = 1;
            var coursePos = 2;
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;
            if (!data) {
                alert("Error finding CurriculumDependencyData.");
                return;
            }
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var cmbSubject = $find("<%= cmbSubject.ClientID %>");
            var cmbCourse = $find("<%= cmbCourse.ClientID %>");
            var selected = _this.GetSelectedValues();
            
            // populate grades 
            _this.PopulateCombo(cmbGrade, CriteriaDataHelpers.GetFieldDistinct(data, gradePos).sort());
            
            
            // filter subjects based on grade and repopulate dropdown
            var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selected.Grade]);
            var filteredSubjects = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredSubjectPos, subjectPos);
            _this.PopulateCombo(cmbSubject, filteredSubjects.sort());
            
            // filter courses based on grade and subject and repopulate
            var filteredCoursePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selected.Grade, selected.Subject]);
            var filteredCourses = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredCoursePos, coursePos);
            _this.PopulateCombo(cmbCourse, filteredCourses.sort());
            
        },

        Clear: function () {
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var cmbSubject = $find("<%= cmbSubject.ClientID %>");
            var cmbCourse = $find("<%= cmbCourse.ClientID %>");

            cmbGrade.set_text(cmbGrade.get_emptyMessage());
            cmbSubject.set_text(cmbSubject.get_emptyMessage());
            cmbCourse.set_text(cmbCourse.get_emptyMessage());

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
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = sender.get_attributes().getAttribute('CriteriaName');
            if (!fromPopulate) _this.PopulateControls(sender);
            CriteriaController.RemoveAll(criteriaName);     // clear whatever was there before

            var valueObject = _this.GetSelectedValues();
            if (valueObject.Course != "" || valueObject.Grade != "" || valueObject.Subject != "") {
                AssociationsRemoveAll();
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        GetSelectedValues: function () {
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var cmbSubject = $find("<%= cmbSubject.ClientID %>");
            var cmbCourse = $find("<%= cmbCourse.ClientID %>");
            var grade = cmbGrade.get_text();
            if (grade == cmbGrade.get_emptyMessage()) grade = "";
            var subject = cmbSubject.get_text();
            if (subject == cmbSubject.get_emptyMessage()) subject = "";
            var course = cmbCourse.get_text();
            if (course == cmbCourse.get_emptyMessage()) course = "";
            return { Grade: grade, Subject: subject, Course: course };
        }
    }

</script>

<script id="ClassesCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">
            {{if Value.Grade != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Grade:</B> {{:Value.Grade}}</div>{{/if}}
            {{if Value.Subject != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Subject:</B> {{:Value.Subject}}</div>{{/if}}
            {{if Value.Course != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Course:</B> {{:Value.Course}}</div>{{/if}}
        </div>
    {{/for}}
</script>
