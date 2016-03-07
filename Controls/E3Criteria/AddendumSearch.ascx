<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="AddendumSearch.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.AddendumSearch" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>

<telerik:RadToolTip ID="RadToolTip1" runat="server" Width="0" Skin="Black" EnableShadow="True" OnClientShow="SearchItem" ShowEvent="OnClick" AutoCloseDelay="2" Position="MiddleRight" RelativeTo="Element">
    <div style="position: relative;"><div style="width: 0; height: 0"></div></div>
</telerik:RadToolTip>

<script type="text/javascript">

    function SearchItem() {
        var itemType = "Addendum";
        var TestCategory = '<%=TestCategory%>';
        var AssessmentId = '<%=AssessmentID%>';
       
        customDialog({ url: ('<% =ResolveUrl("~/Controls/Addendums/AddendumSearch_ExpandedV2.aspx") %>?NewAndReturnType=' + 'Search' + itemType + '&TestCategory=' + TestCategory + '&ShowExpiredItems=No&AssessmentID=' + AssessmentId), maximize: true, maxwidth: 900, maxheight: 550, name: ('Search' + itemType), title: ('Search ' + itemType), destroyOnClose: true });    
    }
   
</script>

<style type="text/css">
    .GrayDisabled .rlbDisabled .rlbText {
        color: #848484;
    }
</style>