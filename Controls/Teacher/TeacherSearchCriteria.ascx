<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeacherSearchCriteria.ascx.cs"
    Inherits="Thinkgate.Controls.Teacher.TeacherSearchCriteria" %>
<script type="text/javascript">
    function leftNavItemClick(sender, eventArgs) {
        var item = eventArgs.get_item().get_text();
        if (item == "My Elements") {
            window.location.href = "../MyElements.aspx";
            return false;
        }
    }
</script>
<telerik:RadPanelBar runat="server" ID="leftNavPanelBar" Width="200" ExpandMode="MultipleExpandedItems" OnClientItemClicked="leftNavItemClick">
    <Items>
        <telerik:RadPanelItem Text="My Elements" CssClass="myElementPanelItem" SelectedCssClass="myElementPanelItem_Selected">
        </telerik:RadPanelItem>                
        <telerik:RadPanelItem Text="Profile" Selected="true" Expanded="true">
            <ContentTemplate>
                Name: <asp:TextBox runat="server" ID="txtName" Width="125"/><br />
                Student ID: <asp:TextBox runat="server" ID="TextBox1" Width="125"/><br />
                Login ID: <asp:TextBox runat="server" ID="TextBox2" Width="125"/>
            </ContentTemplate>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="School/Grade">
            <ContentTemplate>
                School Type: <asp:DropDownList runat="server" ID="ddlSchool" Width="125"/><br />
                School: <asp:DropDownList runat="server" ID="DropDownList1" Width="125"/><br />
                Grade: <asp:DropDownList runat="server" ID="DropDownList5" Width="125"/><br />
            </ContentTemplate>
        </telerik:RadPanelItem>
       
    </Items>
</telerik:RadPanelBar>
