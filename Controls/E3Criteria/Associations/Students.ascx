<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Students.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.Associations.Students" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" Width="300" HideEvent="ManualClose" Skin="Black" Height="55" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <table>
       <tr>
           <td width="200"><b>Name:</b></td>
            <td><telerik:RadTextBox ID="txtName" AutoPostBack="False" Width="140" Skin="Vista" runat="server"></telerik:RadTextBox></td>
       </tr>
       <tr>
            <td><b>Student&nbsp;ID:</b></td>
            <td><telerik:RadTextBox ID="txtId" AutoPostBack="False" Width="140" Skin="Vista" runat="server"></telerik:RadTextBox></td>
       </tr>
       <tr>
            <td><b>Student&nbsp;Grade</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbGrade" EmptyMessage="Grade" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       
    </table>
</telerik:RadToolTip>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
       
        Clear: function () {
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var txtId = $find("<%= txtId.ClientID %>");
            var txtName = $find("<%= txtName.ClientID %>");

            cmbGrade.set_text(cmbGrade.get_emptyMessage());
            txtId.set_value("");
            txtName.set_value("");
        },

        ClearHandler: function () {
            return false;
        },

        OnChange: function (sender, args, fromPopulate) {
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";
            CriteriaController.RemoveAll(criteriaName);     // clear whatever was there before

            var valueObject = _this.GetSelectedValues();
            if (valueObject.Name != "" || valueObject.Id != "" || valueObject.Grade != "") {
                AssociationsRemoveAll();
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        GetSelectedValues: function () {
            var cmbGrade = $find("<%= cmbGrade.ClientID %>");
            var txtId = $find("<%= txtId.ClientID %>");
            var txtName = $find("<%= txtName.ClientID %>");
            var grade = cmbGrade.get_text();
            if (grade == cmbGrade.get_emptyMessage()) grade = "";
           
            var name = txtName.get_value();
            var id = txtId.get_value();
            return { Name: name, Id: id, Grade: grade };
        }
    }

</script>

<script id="StudentsCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">
            {{if Value.Name != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Name:</B> {{:Value.Name}}</div>{{/if}}
            {{if Value.Id != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Student ID:</B> {{:Value.Id}}</div>{{/if}}
            {{if Value.Grade != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Grade:</B> {{:Value.Grade}}</div>{{/if}}
        </div>
    {{/for}}
</script>
