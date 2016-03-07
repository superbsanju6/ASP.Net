<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Documents.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.Associations.Documents" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" Width="300" HideEvent="ManualClose" Skin="Black" Height="55" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <table>
       <tr>
           <td width="200"><b>Template&nbsp;Type:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbTemplateType" EmptyMessage="Select Type" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140" ></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>Template&nbsp;Name:</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbTemplateName" EmptyMessage="Select Name" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>Document&nbsp;Name:</b></td>
            <td><telerik:RadTextBox ID="txtDocName" AutoPostBack="False" Width="140" Skin="Vista" runat="server"></telerik:RadTextBox> </td>
       </tr>
       
    </table>
</telerik:RadToolTip>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        PopulateControls: function (changedControl) {
            var templateTypePos = 1;
            var templateNamePos = 2;
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;
            if (!data) {
                alert("Error finding DocumentsDependencyData.");
                return;
            }
            var cmbTemplateType = $find("<%= cmbTemplateType.ClientID %>");
            var cmbTemplateName = $find("<%= cmbTemplateName.ClientID %>");
            var selected = _this.GetSelectedValues();
            
            _this.PopulateCombo(cmbTemplateType, CriteriaDataHelpers.GetFieldDistinct(data, templateTypePos).sort());
            
            var filteredTemplateNamePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [null, selected.TemplateType]);
            var filteredTemplateNames = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredTemplateNamePos, templateNamePos);
            _this.PopulateCombo(cmbTemplateName, filteredTemplateNames.sort());
            
        },

        Clear: function () {
            var cmbTemplateType = $find("<%= cmbTemplateType.ClientID %>");
            var cmbTemplateName = $find("<%= cmbTemplateName.ClientID %>");
            var txtDocName = $find("<%= txtDocName.ClientID %>");

            cmbTemplateType.set_text(cmbTemplateType.get_emptyMessage());
            cmbTemplateName.set_text(cmbTemplateName.get_emptyMessage());
            txtDocName.set_value("");
            
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
            if (valueObject.TemplateType != "" || valueObject.TemplateName != "" || valueObject.DocumentName != "") {
                AssociationsRemoveAll();
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        GetSelectedValues: function () {
            var cmbTemplateType = $find("<%= cmbTemplateType.ClientID %>");
            var cmbTemplateName = $find("<%= cmbTemplateName.ClientID %>");
            var txtDocName = $find("<%= txtDocName.ClientID %>");
            var templateType = cmbTemplateType.get_text();
            if (templateType == cmbTemplateType.get_emptyMessage()) templateType = "";
            var templateName = cmbTemplateName.get_text();
            if (templateName == cmbTemplateName.get_emptyMessage()) templateName = "";
            var documentName = txtDocName.get_value();
            return { TemplateType: templateType, TemplateName: templateName, DocumentName: documentName };
        }
    }

</script>

<script id="DocumentsCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">
            {{if Value.TemplateType != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Template Type:</B> {{:Value.TemplateType}}</div>{{/if}}
            {{if Value.TemplateName != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Template Name:</B> {{:Value.TemplateName}}</div>{{/if}}
            {{if Value.DocumentName != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Document Name:</B> {{:Value.DocumentName}}</div>{{/if}}
        </div>
    {{/for}}
</script>
