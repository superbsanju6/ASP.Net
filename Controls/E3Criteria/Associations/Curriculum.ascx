<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="Curriculum.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.Associations.Curriculum" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" HideEvent="ManualClose" Skin="Black" Height="55" Width="245" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <table>
       <tr>
            <td width="100"><b>Grade:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbGrade" CtrlName="cmbGrade" EmptyMessage="Select Grade" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140" ></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>Subject:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbSubject" CtrlName="cmbSubject" EmptyMessage="Select Subject" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>Curriculum:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbCurriculum" CtrlName="cmbCurriculum" EmptyMessage="Select Curriculum" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       <tr id="trType" runat="server">
            <td><b>Type:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbType" CtrlName="cmbType" EmptyMessage="Select Type" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       <tr id="trTerm" runat="server">
            <td><b>Term:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbTerm" CtrlName="cmbTerm" EmptyMessage="Select Term" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       
    </table>
</telerik:RadToolTip>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        PopulateControls: function (changedControl) {
            var gradePos = 0;
            var subjectPos = 1;
            var curriculumPos = 2;
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;
            if (!data) {
                alert("Error finding CurriculumDependencyData.");
                return;
            }
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var cmbSubject = $find("<%= cmbSubject.ClientID %>");
            var cmbCurriculum = $find("<%= cmbCurriculum.ClientID %>");
            var selected = _this.GetSelectedValues();
            
            if ( "<%= IncludeTypeAndTermControls %>" == "True" ) {
                var cmbType = $find("<%= cmbType.ClientID %>");
                var cmbTerm = $find("<%= cmbTerm.ClientID %>");
                _this.PopulateCombo(cmbType, CriteriaDataHelpers.GetFieldDistinct(data.TypeData, 0).sort());
                _this.PopulateCombo(cmbTerm, CriteriaDataHelpers.GetFieldDistinct(data.TermData, 0).sort());
                data = data.GradeSubjectCurriculumData;
            }

            // populate grades 
            _this.PopulateCombo(cmbGrade, CriteriaDataHelpers.GetFieldDistinct(data, gradePos).sort());
            
            // filter subjects based on grade and repopulate dropdown
            var filteredSubjectPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selected.Grade]);
            var filteredSubjects = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredSubjectPos, subjectPos);
            _this.PopulateCombo(cmbSubject, filteredSubjects.sort());
            
            // filter curriculums based on grade and subject and repopulate
            var filteredCurriculumPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selected.Grade, selected.Subject]);
            var filteredCurriculums = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredCurriculumPos, curriculumPos);
            _this.PopulateCombo(cmbCurriculum, filteredCurriculums.sort());

        },

        Clear: function () {
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var cmbSubject = $find("<%= cmbSubject.ClientID %>");
            var cmbCurriculum = $find("<%= cmbCurriculum.ClientID %>");

            cmbGrade.set_text(cmbGrade.get_emptyMessage());
            cmbSubject.set_text(cmbSubject.get_emptyMessage());
            cmbCurriculum.set_text(cmbCurriculum.get_emptyMessage());

            if ("<%= IncludeTypeAndTermControls %>" == "True") {
                var cmbType = $find("<%= cmbType.ClientID %>");
                var cmbTerm = $find("<%= cmbTerm.ClientID %>");
                cmbType.set_text(cmbType.get_emptyMessage());
                cmbTerm.set_text(cmbTerm.get_emptyMessage());
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
                this.OnComboChanged(combo, null, true);
            }
        },

        OnComboChanged: function (sender, args, fromPopulate) {
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = sender.get_attributes().getAttribute('CriteriaName');
            var controlName = sender._attributes.getAttribute("CtrlName");

            //
            // If Grade, Subject, or Curriculum control changed, then all three need to be reloaded 
            // according to what was picked since these controls are inter-related.
            if (!fromPopulate && "cmbGrade, cmbSubject, cmbCurriculum".indexOf(controlName) >= 0) _this.PopulateControls(sender);

            CriteriaController.RemoveAll(criteriaName);     // clear whatever was there before

            var valueObject = _this.GetSelectedValues();

            if (valueObject.Curriculum != "" ||
                valueObject.Grade != "" ||
                valueObject.Subject != "" ||
                valueObject.Type != "" ||
                valueObject.Term != "")
            {
                //AssociationsRemoveAll();
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        GetSelectedValues: function () {
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var cmbSubject = $find("<%= cmbSubject.ClientID %>");
            var cmbCurriculum = $find("<%= cmbCurriculum.ClientID %>");

            var grade = cmbGrade.get_text();
            if (grade == cmbGrade.get_emptyMessage()) grade = "";
            var subject = cmbSubject.get_text();
            if (subject == cmbSubject.get_emptyMessage()) subject = "";
            var curriculum = cmbCurriculum.get_text();
            if (curriculum == cmbCurriculum.get_emptyMessage()) curriculum = "";

            if ("<%= IncludeTypeAndTermControls %>" != "True") {
                //
                // If we're in this if block, then Type and Term won't be displayed within the criteria,
                // so by setting Type and Term to "", these shouldn't display.
                //
                return { Grade: grade, Subject: subject, Curriculum: curriculum, Type: "", Term: "" };
            }
            else
            {
                var cmbType = $find("<%= cmbType.ClientID %>");
                var type = cmbType.get_text();
                if (type == cmbType.get_emptyMessage()) type = "";

                var cmbTerm = $find("<%= cmbTerm.ClientID %>");
                var term = cmbTerm.get_text();
                if (term == cmbTerm.get_emptyMessage()) term = "";

                return { Grade: grade, Subject: subject, Curriculum: curriculum, Type: type, Term: term };
            }
        }
    }


</script>

<script id="CurriculumCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">
            {{if Value.Grade != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Grade:</B> {{:Value.Grade}}</div>{{/if}}
            {{if Value.Subject != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Subject:</B> {{:Value.Subject}}</div>{{/if}}
            {{if Value.Curriculum != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Curriculum:</B> {{:Value.Curriculum}}</div>{{/if}}
            {{if Value.Type != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Type:</B> {{:Value.Type}}</div>{{/if}}
            {{if Value.Term != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Term:</B> {{:Value.Term}}</div>{{/if}}
        </div>
    {{/for}}
</script>
