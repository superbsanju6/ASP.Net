<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentSearchCriteria.ascx.cs"
    Inherits="Thinkgate.Controls.Student.StudentSearchCriteria" %>
<script type="text/javascript">
    function leftNavItemClick(sender, eventArgs) {
        var item = eventArgs.get_item().get_text();
        if (item == "My Elements") {
            window.location.href = "../MyElements.aspx";
            return false;
        }
    }
</script>
<telerik:RadPanelBar runat="server" ID="leftNavPanelBar" Width="200" ExpandMode="MultipleExpandedItems"
    OnClientItemClicked="leftNavItemClick">
    <Items>
        <telerik:RadPanelItem Text="Name">
            <ContentTemplate>
                <div class="threeItem_wrapper" style="width: 200px;">
                    <div id="nameLabel" class="threeItem_left" style="width: 42px; height: 30px;">
                        Name:
                    </div>
                    <div id="nameTxtBox" class="threeItem_center" style="width: 128px; height: 30px;">
                        <div style="padding: 2px;">
                            <asp:TextBox runat="server" ID="txtName" Width="120" />
                        </div>
                    </div>
                    <div id="nameGoBtn" class="threeItem_right" style="width: 27px; height: 30px;">
                        <div style="padding: 4px; height: 30px;">
                            <asp:ImageButton runat="server" ID="imgBtnNameSearch" ImageUrl="~/Images/GO-icon.gif" onclick="SearchBtnClick"/>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Student ID">
            <ContentTemplate>
            </ContentTemplate>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Login ID">
            <ContentTemplate>
            </ContentTemplate>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="RTI">
            <ContentTemplate>
            </ContentTemplate>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="School Type">
            <ContentTemplate>
            </ContentTemplate>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="School">
            <ContentTemplate>
            </ContentTemplate>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Grade">
            <ContentTemplate>
            </ContentTemplate>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="Demographics">
            <ContentTemplate>
            </ContentTemplate>
        </telerik:RadPanelItem>
        <telerik:RadPanelItem Text="UDFs">
            <ContentTemplate>
            </ContentTemplate>
        </telerik:RadPanelItem>
    </Items>
</telerik:RadPanelBar>
