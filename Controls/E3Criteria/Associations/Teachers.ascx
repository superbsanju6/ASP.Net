<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Teachers.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.Associations.Teachers" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" ShowEvent="OnClick" Width="300" HideEvent="ManualClose" Skin="Black" Height="55" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <table>
       <tr>
           <td width="200"><b>Name:</b></td>
            <td><telerik:RadTextBox ID="txtName" AutoPostBack="False" Width="140" Skin="Vista" runat="server"></telerik:RadTextBox></td>
       </tr>
       <tr>
            <td><b>User&nbsp;Type</b></td>
            <td><telerik:RadComboBox runat="server" ID="cmbUserType" EmptyMessage="Type" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" Skin="Vista" Width="140"></telerik:RadComboBox></td>
       </tr>
       <tr>
            <td><b>User&nbsp;ID:</b></td>
            <td><telerik:RadTextBox ID="txtUserId" AutoPostBack="False" Width="140" Skin="Vista" runat="server"></telerik:RadTextBox></td>
       </tr>
       
       
    </table>
</telerik:RadToolTip>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
       
        Clear: function () {
            var cmbUserType = $find("<%= cmbUserType.ClientID %>");
            var txtUserId = $find("<%= txtUserId.ClientID %>");
            var txtName = $find("<%= txtName.ClientID %>");

            cmbUserType.set_text(cmbUserType.get_emptyMessage());
            txtUserId.set_value("");
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
            if (valueObject.Name != "" || valueObject.UserType != "" || valueObject.UserId != "") {
                AssociationsRemoveAll();
                CriteriaController.Add(criteriaName, valueObject);
            }
        },

        GetSelectedValues: function () {
            var cmbUserType = $find("<%= cmbUserType.ClientID %>");
            var txtName = $find("<%= txtName.ClientID %>");
            var txtUserId = $find("<%= txtUserId.ClientID %>");
            var userType = cmbUserType.get_text();
            if (userType == cmbUserType.get_emptyMessage()) userType = "";
           
            var userId = txtUserId.get_value();
            var name = txtName.get_value();
            return { Name: name, UserType: userType, UserId: userId };
        }
    }

</script>

<script id="TeachersCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">
            {{if Value.Name != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>Name:</B> {{:Value.Name}}</div>{{/if}}
            {{if Value.UserType != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>User Type:</B> {{:Value.UserType}}</div>{{/if}}
            {{if Value.UserId != ""}}<div class="criteriaText" style="clear: left; margin-bottom: 2px"><B>User ID:</B> {{:Value.UserId}}</div>{{/if}}
        </div>
    {{/for}}
</script>
