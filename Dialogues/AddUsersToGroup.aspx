<%@ Page Title="" Language="C#" MasterPageFile="~/Search.Master" AutoEventWireup="true" CodeBehind="AddUsersToGroup.aspx.cs" Inherits="Thinkgate.Dialogues.AddUsersToGroup" %>

<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextControl" Src="~/Controls/E3Criteria/Text.ascx" %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx" %>
<%@ Register TagPrefix="e3" TagName="Demographics" Src="~/Controls/E3Criteria/Demographics.ascx" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ MasterType virtualpath="~/Search.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function CloseDialog(arg) {
            var oWindow = GetRadWindow();
            setTimeout(function () {
                oWindow.Close(arg);
            }, 0);
        }

        function OnEditClosed(sender, args) {
            var ajaxManager = window.$find("<%=RadAjaxManager.GetCurrent(Page).ClientID %>");
            if (ajaxManager == null) return;
            ajaxManager.ajaxRequestWithTarget('<%=grdDetail.UniqueID%>', args);
        }

        function OnDeleteClosed(sender, args) {
            if (args.get_argument() == 'Deleted')
                CloseDialog('Deleted');
        }

        function OnCopyClosed(sender, args) {
            var arg = args.get_argument();
            if(arg)
               document.location("AddUsersToGroup.aspx?roleId=" + args.get_argument());
        }

        function openEditGroup(groupId)
        {
            var url = '../Dialogues/EditGroup.aspx?groupId=' + groupId;
            customDialog({ url: url, width: 600, height: 400, onClosed: OnEditClosed, title: 'Edit Group' });
        }

        function openCopyGroup(roleId) {
            window.radopen("CopyGroup.aspx?roleId=" + roleId, "copyWindow");
        }

        function openDeleteGroup(roleId) {
            window.radopen("DeleteGroup.aspx?roleId=" + roleId, "deleteWindow");
        }

        function confirmDelete() {
            if (confirm('Are you sure you want delete this group?')) {
                return true;
            } else {
                return false;
            }      
        }
    </script>

    <style type="text/css">
        #container {
            width: 700px;
            margin-left: auto;
            margin-right: auto;
            height: 645px;
        }

        .panels {
            margin-left: auto;
            margin-right: auto;
            width: 95%;
        }

        .leftPanel {
            float: left;
            width: 98%;
        }

        .rightPanel {
            float: right;
            width: 98%;
        }

        .searchPanel {
            margin-top: 57px;
             width: 200px; 
             float: left; 
             height: 100%;  
             position: static;
        }

        .panelLabel {
            text-align: center;
            width: 100%;
            display: inline-block;
            font-size: large;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .detailGrid {
            margin-top: 10px;
            margin-bottom: 10px;
            margin-left: auto;
            margin-right: 10px;
            width: 95%;
        }

        .center {
            margin-left: auto;
            margin-right: auto;
            width: 50%;
            height: 525px;
        }

        .RadGrid_Default th.rgHeader {
            background-image: none;
            background-color: white;
        }

        .searchPanel {
            margin-bottom: 5px;
        }
        
        .countLabels {
            margin-top: 2px;
            font-size: small;
        }

    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" RunSearchOnPageLoad="True" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:TextControl ID="ctrlStudentName" CriteriaName="StudentName" runat="server" Text="Name" DataTextField="Name" DataValueField="Value" />
    <e3:TextControl ID="ctrlStudentID" CriteriaName="StudentID" runat="server" Text="Student ID" DataTextField="StudentID" DataValueField="Value" />
    <e3:CheckBoxList ID="ctrlRTI" CriteriaName="RTI" runat="server" Text="RTI" DataTextField="Tier" DataValueField="Tier" />
    <e3:CheckBoxList ID="ctrlCluster" CriteriaName="Cluster" runat="server" Text="Cluster" DataTextField="Cluster" DataValueField="Cluster" />
    <e3:CheckBoxList ID="ctrlSchoolType" CriteriaName="SchoolType" runat="server" Text="School Type" DataTextField="SchoolType" DataValueField="SchoolType" />
    <e3:DropDownList ID="ctrlSchool" CriteriaName="School" runat="server" Text="School" EmptyMessage="Select School" DataTextField="Name" DataValueField="ID" />
    <e3:CheckBoxList ID="ctrlGrade" CriteriaName="Grade" runat="server" Text="Grade" DataTextField="DisplayText" DataValueField="DisplayText" />
    <e3:DropDownList ID="ctrlClass" CriteriaName="Class" runat="server" Text="Class" EmptyMessage="Select a Class" DataTextField="FriendlyName" DataValueField="ID" />
    <e3:Demographics ID="ctrlDemographics" CriteriaName="Demographics" runat="server" Text="Demographics" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <div id="container">
        <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
        <telerik:RadAjaxManagerProxy runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="grdAvailable">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdSelected" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="grdAvailable" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="grdDetail" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="lblAvailable" />
                        <telerik:AjaxUpdatedControl ControlID="lblSelected" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdSelected">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdSelected" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="grdAvailable" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="grdDetail" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="lblAvailable" />
                        <telerik:AjaxUpdatedControl ControlID="lblSelected" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdDetail">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdDetail" LoadingPanelID="updPanelLoadingPanel" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManagerProxy>

        <div class="detailGrid">
            <telerik:RadGrid runat="server" ID="grdDetail" BorderStyle="None" OnNeedDataSource="DisplayDetailForGroup" OnItemDataBound="grdDetails_ItemDataBound">
                <MasterTableView AutoGenerateColumns="False" AllowSorting="False" DataKeyNames="ID">
                    <HeaderStyle BorderStyle="None" />
                    <Columns>
                        <telerik:GridTemplateColumn HeaderText="" UniqueName="EditGroup">
                            <ItemTemplate>
                                <asp:LinkButton ID="EditButton" runat="server" Text="Edit" OnClientClick='<%# String.Format("openEditGroup(\"{0}\"); return false;", Eval("ID")) %>' />
                            </ItemTemplate>
                            <ItemStyle BorderStyle="None" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="" UniqueName="DeleteGroup">
                            <ItemTemplate>
                                <asp:LinkButton ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteGroup_Click" OnClientClick="if(!confirmDelete()) return false;"/>
                            </ItemTemplate>
                            <ItemStyle BorderStyle="None" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="Name" HeaderText="Name" UniqueName="Name">
                            <ItemStyle BorderStyle="None" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Description" HeaderText="Description" UniqueName="Description">
                            <ItemStyle BorderStyle="None" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="CreatorName" HeaderText="Created By">
                            <ItemStyle BorderStyle="None" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:M/d/yy}">
                            <ItemStyle BorderStyle="None" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="StudentCount" HeaderText="Members">
                            <ItemStyle BorderStyle="None" />
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings>
                    <Scrolling AllowScroll="False"></Scrolling>
                </ClientSettings>
            </telerik:RadGrid>
        </div>

        <div class="panels" runat="server" id="divPanels">
            <table width="100%">
                <tr>
                    <td>
                        <div class="leftPanel">
                            <label class="panelLabel">Available</label>
                            <telerik:RadGrid runat="server" ID="grdAvailable" Height="500px" Width="300px" OnNeedDataSource="DisplayStudentsAvailableForGroup">
                                <MasterTableView AutoGenerateColumns="False" AllowSorting="True" DataKeyNames="ID" NoMasterRecordsText="There are no students assigned to this group">
                                    <Columns>
                                        <telerik:GridTemplateColumn>
                                            <HeaderTemplate>
                                                <asp:LinkButton Text="Add All" Font-Underline="True" OnClick="btnSelectAll_Click" runat="server"></asp:LinkButton>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton Text="Add" OnClick="btnAdd_Click" runat="server"></asp:LinkButton>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="Name" HeaderText="Name" />
                                        <telerik:GridBoundColumn DataField="SchoolName" HeaderText="School" />
                                    </Columns>
                                </MasterTableView>
                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="True" AllowScroll="True" />
                                </ClientSettings>
                            </telerik:RadGrid>
                            <asp:Label runat="server" ID="lblAvailable" Text="# Students Available" CssClass="countLabels" />
                        </div>
                    </td>
                    <td>
                        <div class="rightPanel">
                            <label class="panelLabel">Selected</label>
                            <telerik:RadGrid runat="server" ID="grdSelected" Height="500px" Width="300px" OnNeedDataSource="DisplayStudentsInGroup">
                                <MasterTableView AutoGenerateColumns="False" AllowSorting="True" DataKeyNames="ID" NoMasterRecordsText="There are no students assigned to this group">
                                    <Columns>
                                        <telerik:GridTemplateColumn>
                                            <HeaderTemplate>
                                                <asp:LinkButton Text="Remove All" Font-Underline="True" OnClick="btnRemoveAll_Click" runat="server"></asp:LinkButton>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton Text="Remove" OnClick="btnRemove_Click" runat="server"></asp:LinkButton>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="Name" HeaderText="Name" />
                                        <telerik:GridBoundColumn DataField="SchoolName" HeaderText="School" />
                                    </Columns>
                                </MasterTableView>
                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="True" AllowScroll="True" />
                                </ClientSettings>
                            </telerik:RadGrid>
                            <asp:Label runat="server" ID="lblSelected" Text="# Students Selected" CssClass="countLabels" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
