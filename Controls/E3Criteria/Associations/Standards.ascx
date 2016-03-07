<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Standards.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.Associations.Standards" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" HideEvent="ManualClose" Skin="Black" Height="55" Width="245" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <table>
       <tr>
            <td width="100"><b>Standard Set:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbStandardSet" EmptyMessage="Select Set" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>Grade:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbGrade" EmptyMessage="Select Grade" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>Subject:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbSubject" EmptyMessage="Select Subject" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140" ></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>Course:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbCourse" EmptyMessage="Select Course" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>Standard:</b></td>
            <td>
                <telerik:RadComboBox runat="server" ID="cmbStandard" Enabled="False" EmptyMessage="Select Standards" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"
                    EnableLoadOnDemand="False" CheckBoxes="True" DropDownWidth="240">
                    <WebServiceSettings Method="StandardsByStandardSetGradeSubjectCourse" Path="~/Services/StandardsAJAX.svc" />       
                </telerik:RadComboBox>
           </td>
       </tr>
       
    </table>
</telerik:RadToolTip>

<script type="text/javascript">
  
    var <%=CriteriaName%>Controller = {
        PopulateControls: function () {
            var gradePos = 0;
            var subjectPos = 1;
            var coursePos = 2;
            var standardSetPos = 3;
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;
            if (!data) {
                alert("Error finding StandardsDependencyData.");
                return;
            }
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var cmbSubject = $find("<%= cmbSubject.ClientID %>");
            var cmbCourse = $find("<%= cmbCourse.ClientID %>");
            var cmbStandardSet = $find("<%= cmbStandardSet.ClientID %>");
            var selected = _this.GetSelectedValues();
            

             // populate standardsets 
            cmbStandardSet.clearItems();
            _this.PopulateCombo(cmbStandardSet, CriteriaDataHelpers.GetFieldDistinct(data, standardSetPos).sort());
            
            // filter grades based on standardset and repopulate dropdown
            var filteredGradePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [null, null, null, [selected.StandardSet]]);
            var filteredGrades = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredGradePos, gradePos);
            _this.PopulateCombo(cmbGrade, filteredGrades.sort());
            

            // filter subjects based on grade, standardset and repopulate dropdown
            var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selected.Grade, null, null, [selected.StandardSet]]);
            var filteredSubjects = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredSubjectPos, subjectPos);
            _this.PopulateCombo(cmbSubject, filteredSubjects.sort());
            
            // filter curriculums based on grade, subject, standardset and repopulate
            var filteredCoursePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selected.Grade, selected.Subject, null, [selected.StandardSet]]);
            var filteredCourses = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredCoursePos, coursePos);
            _this.PopulateCombo(cmbCourse, filteredCourses.sort());
            
            
            
        },

        ClearHandler: function () {
            return false;
        },

        Clear: function () {
            var cmbStandardSet = $find("<%= cmbStandardSet.ClientID %>");
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var cmbSubject = $find("<%= cmbSubject.ClientID %>");
            var cmbCourse = $find("<%= cmbCourse.ClientID %>");
            var cmbStandard = $find("<%= cmbStandard.ClientID %>");

            cmbStandardSet.set_text(cmbStandardSet.get_emptyMessage());
            cmbGrade.set_text(cmbGrade.get_emptyMessage());
            cmbSubject.set_text(cmbSubject.get_emptyMessage());
            cmbCourse.set_text(cmbCourse.get_emptyMessage());
            cmbStandard.set_text(cmbStandard.get_emptyMessage());

            cmbStandard.disable();

            this.PopulateControls();
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
            var selectedValues = _this.GetSelectedValues();

            if (selectedValues.StandardSet != "" && selectedValues.Grade != "" && selectedValues.Subject != "" && selectedValues.Course != "") {
                var cmbStandard = $find("<%= cmbStandard.ClientID %>");
                cmbStandard.enable();
                cmbStandard.requestItems("", false);
            }
            if (!fromPopulate) _this.PopulateControls();
        },

        OnChecked: function (sender, args) {
            var _this = <%=CriteriaName%>Controller;
            var item = args.get_item();
            var items = sender.get_items();
            if (item.get_text() == "All") {
                var isChecked = item.get_checked();
                for (var j = 0; j < items.get_count(); j++) {
                    items.getItem(j).set_checked(isChecked);
                }
            }
            var criteriaName = sender.get_attributes().getAttribute('CriteriaName');
            var valueObject = _this.GetSelectedValues();
            var selectedStandards = [];
            for (var j = 0; j < items.get_count(); j++) {
                if (items.getItem(j).get_checked()) {
                    selectedStandards.push(items.getItem(j).get_value());
                }
            }
            CriteriaController.RemoveAll(criteriaName);     // clear whatever was there before
            if (selectedStandards.length > 0) {
                if (selectedStandards.length > 1) {
                    valueObject.StandardName = "Multiple";
                } else {
                    var item = sender.findItemByValue(selectedStandards[0]);
                    if (item) valueObject.StandardName = item.get_text();
                }
                valueObject.Standards = selectedStandards;
                AssociationsRemoveAll();    // only allow one selected value in all associations
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        GetSelectedValues: function () {
            var cmbStandardSet = $find("<%= cmbStandardSet.ClientID %>");
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var cmbSubject = $find("<%= cmbSubject.ClientID %>");
            var cmbCourse = $find("<%= cmbCourse.ClientID %>");
            var standardSet = cmbStandardSet.get_text();
            if (standardSet == cmbStandardSet.get_emptyMessage()) standardSet = "";
            var grade = cmbGrade.get_text();
            if (grade == cmbGrade.get_emptyMessage()) grade = "";
            var subject = cmbSubject.get_text();
            if (subject == cmbSubject.get_emptyMessage()) subject = "";
            var course = cmbCourse.get_text();
            if (course == cmbCourse.get_emptyMessage()) course = "";
            return { StandardSet: standardSet, Grade: grade, Subject: subject, Course: course };
        },

        OnClientRequesting: function (sender, e) {
            var _this = <%=CriteriaName%>Controller;
            var context = e.get_context();
            var selectedValues = _this.GetSelectedValues();
            //using jQuery to get the checked item and pass it to the server
            context["StandardSet"] = selectedValues.StandardSet;
            context["Grade"] = selectedValues.Grade;
            context["Subject"] = selectedValues.Subject;
            context["Course"] = selectedValues.Course;
            context["IncludeAllOption"] = true;
        }
    }
</script>

<script id="StandardsCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">
            <div class="criteriaText" style="clear: left; margin-bottom: 2px; "><B>Standard Set:</B> {{:Value.StandardSet}}</div>
            <div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Grade:</B> {{:Value.Grade}}</div>
            <div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Subject:</B> {{:Value.Subject}}</div>
            <div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Course:</B> {{:Value.Course}}</div>
            <div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Standard:</B> {{:Value.StandardName}}</div>
        </div>
    {{/for}}
</script>

<style type="text/css">
    .rcbCheckBox {
        margin-right: 2px;
    }
</style>