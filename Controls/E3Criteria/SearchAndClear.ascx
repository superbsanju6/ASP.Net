<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchAndClear.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.SearchAndClear" %>

<div style="width: 100%; vertical-align: middle; height:33px; text-align: center; overflow: hidden">
    <telerik:RadButton ID="btnSearch" runat="server" CssClass="searchButton" Text="Update Results" ToolTip="Update report criteria"
        Skin="Web20" Style="font-weight: bold; margin-top: 5px; margin-bottom: 5px;" AutoPostBack="False">
    </telerik:RadButton>
    <telerik:RadButton ID="btnClear" runat="server" Text="Clear" ToolTip="Clear report criteria"
        Skin="Web20" Style="font-weight: bold; margin-top: 5px; margin-bottom: 5px;" AutoPostBack="False">
    </telerik:RadButton>
</div>

<script type="text/javascript">


    function searchOnLoad(cnt) {
        if ($find("AjaxPanelResults"))
            btnSearchClick();
        else {
            if (!cnt) cnt = 0;
            if (cnt >= 20) return;
            setTimeout("searchOnLoad(" + (++cnt) + ")", 250);
        }
    }

    function btnSearchClick() {
        CriteriaController.UpdateCriteriaForSearch();
        $find("AjaxPanelResults").ajaxRequest(CriteriaController.ToJSON());
        SearchHasBeenRun = true;
    }

    function btnClearClick() {
        CriteriaController.Clear();
    }
</script>