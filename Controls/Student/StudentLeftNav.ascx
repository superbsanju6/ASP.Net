<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentLeftNav.ascx.cs"
    Inherits="Thinkgate.Controls.Student.StudentLeftNav" %>
<script type="text/javascript">
    function leftNavItemClick(sender, eventArgs) {
        var item = eventArgs.get_item().get_text();
        if (item == "My Elements") {
            window.location.href = "../MyElements.aspx";
            return false;
        }
        return true;
    }
</script>
<div class="rectangle">
    <h2>
        Student</h2>
</div>
<div class="triangle-r">
</div>
<br />
<br />
<telerik:RadPanelBar runat="server" ID="leftNavPanelBar" OnItemClick="leftNavPanelBar_Click" OnClientItemClicked="leftNavItemClick"
    Width="200" ExpandMode="SingleExpandedItem">
    <Items>
        <telerik:RadPanelItem Text="My Elements" CssClass="myElementPanelItem" SelectedCssClass="myElementPanelItem_Selected">
        </telerik:RadPanelItem>   
        <telerik:RadPanelItem Text="Dashboard" Selected="true" Expanded="false">
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Profile">
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Assessment Results">
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Classes">
            <Items>
            </Items>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Grades">
            <Items>
            </Items>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Attendance">
            <Items>
            </Items>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Discipline">
            <Items>
            </Items>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Interventions">
            <Items>
            </Items>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Graduation Readiness">
            <Items>
            </Items>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Career Readiness">
            <Items>
            </Items>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Documentation">
            <Items>
            </Items>
        </telerik:RadPanelItem>
    </Items>
</telerik:RadPanelBar>
