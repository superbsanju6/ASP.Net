<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourseSelector.ascx.cs"
    Inherits="Thinkgate.Controls.Course.CourseSelector" %>

<div id="iPhoneFrame" class="OrientationVertical">
    <div id="RadMenu_iPhone_wrapper">
        <a class="backButton" title="Back"><span>Back</span></a>
        <div id="currentSelection" style="padding-left: 150px"></div>
        <div id="RadMenu_iPhone_Content">
            <telerik:RadMenu runat="server" ID="coursesMenu" EnableSelection="false" EnableEmbeddedSkins="false"
                Skin="iPhone" ClickToOpen="true" Flow="vertical">                
            </telerik:RadMenu>
        </div>
    </div>
</div>

<script type="text/javascript" src="../scripts/iPodRadMenu.js"></script>

<script type="text/javascript">
    var iPodMenu1 = null;

    function pageLoad() {
        var RadMenu1 = $find('<%= coursesMenu.ClientID %>');
        iPodMenu1 = new iPodMenu(RadMenu1);
        if ($telerik.$('.OrientationVertical').length)
            iPodMenu1.changeOrientation('Vertical');
        else
            iPodMenu1.changeOrientation('Horizontal');
    }
</script>
