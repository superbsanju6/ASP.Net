<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpirationStatusAndDateRange.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.ExpirationStatusAndDateRange" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="DateRange" Src="~/Controls/E3Criteria/DateRange.ascx" %>


<e3:DropDownList ID="ddlExpirationStatus" CriteriaName="ExpirationStatus" runat="server" Text="Expiration Status" EmptyMessage="None" DataTextField="Name" DataValueField="Value"/>
<asp:Panel runat="server" ID="Panel1" ClientIDMode="Static" >
<e3:DateRange ID="drCopyRightExpiryDate" CriteriaName="ExpirationDateRange" Text="Expiration Date" runat="server"/>
</asp:Panel>
<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        OnChange: function() {
            var expiryDateElement = ("<%= drCopyRightExpiryDate.ClientID %>");
            if (typeof <%=ddlExpirationStatus.CriteriaName%>Controller != 'undefined') {
                var selectedExpiryStatus = CriteriaController.GetValuesAsSinglePropertyArray("ExpirationStatus", "Value");
                if ($.inArray('E', selectedExpiryStatus) > -1) {
                    CriteriaController.RemoveAll("ExpirationDateRange");
                    document.getElementById('Panel1').style.display = "none";
                } else {
                    document.getElementById('Panel1').style.display = "";
                }
            }
        }
    }
</script>
