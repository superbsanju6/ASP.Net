<%@ Page Title="" Language="C#" MasterPageFile="~/Search.Master" AutoEventWireup="true" CodeBehind="ManageAssignments.aspx.cs" Inherits="Thinkgate.Controls.InstructionMaterial.ManageAssignments" %>

<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextControl" Src="~/Controls/E3Criteria/Text.ascx" %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx" %>
<%@ Register TagPrefix="e3" TagName="Demographics" Src="~/Controls/E3Criteria/Demographics.ascx" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ MasterType VirtualPath="~/Search.Master" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        Telerik.Web.UI.RadWindowUtils.Localization =
            {
                "OK": "Yes",
                "Cancel": "No",
            };
        var confirmed = false;
        //global DOM ID registry.  filled up by scripts rendered from templates. 
        var assignmentDateElements = [];
        var dueDateElements = [];        

        
        function confirmCallbackCloseClient() {          
            var modalWin = getCurrentCustomDialog();
            modalWin.remove_beforeClose(callmessage);
            setTimeout(function () {
                modalWin.Close();
            }, 0);
        }


        function OnEditClosed(sender, args) {
            var ajaxManager = window.$find("<%=RadAjaxManager.GetCurrent(Page).ClientID %>");
            if (ajaxManager == null) return;
            ajaxManager.ajaxRequestWithTarget('<%=grdDetail.UniqueID%>', args);
        }

        //function OnDeleteClosed(sender, args) {
        //    if (args.get_argument() == 'Deleted')
        //        CloseDialog('Deleted');
        //}

        function OnCopyClosed(sender, args) {
            var arg = args.get_argument();
            if (arg)
                document.location("AddUsersToGroup.aspx?roleId=" + args.get_argument());
        }

        function openEditGroup(groupId) {
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

        function confirmCallbackFunction() {
            ShowSpinner("form1");
            var button = $telerik.findButton("<%= btnSave.ClientID %>");
            button.click();
            $("#isCompleted").val("1");
           // btnSearchClick();
        }

        function confirmCallbackClose() {
            $("#isCompleted").val("1");
            btnSearchClick();
        }
        function confirmCallbackFunctionClient() {
            $("#isCompleted").val("1");
            ShowSpinner("form1");
            var button = $telerik.findButton("<%= btnSaveClient.ClientID %>");
            button.click();            
        }

        function onDateChanged(sender, eventArgs) {
            var date = sender.get_selectedDate();
            var formattedDate = sender.get_selectedDate().localeFormat();
            var SenderId = sender.get_id();
            var RadDatePicker2 = $telerik.findDatePicker(SenderId.replace("calAssignmentDate", "calDueDate"));
            RadDatePicker2.set_minDate(date);
            updated();
        }
        function onDueDateChanged(sender, eventArgs) {
            var date = sender.get_selectedDate();
            var formattedDate = sender.get_selectedDate().localeFormat();
            var SenderId = sender.get_id();
            var RadDatePicker2 = $telerik.findDatePicker(SenderId.replace("calDueDate", "calAssignmentDate"));
            RadDatePicker2.set_maxDate(date);
            updated();
        }

        function onMasterAssignmentDateChanged(sender, eventArgs) {            
            var useDate = sender.get_selectedDate();
            var SenderId = sender.get_id();
            var RadDatePicker2 = $telerik.findDatePicker(SenderId.replace("calMasterAssignmentDate", "calMasterDueDate"));
            RadDatePicker2.set_minDate(useDate);
            customDialog({ title: 'Confirm?', maximize: true, maxwidth: 500, maxheight: 120, content: 'Do you want to reset the dates for all students?<br/>', dialog_style: 'confirm' }, [{ title: 'No' }, { title: 'Yes', callback: updateAssignmentDate, argArray: [useDate] }]);
            updated();
        }

        function onMasterDueDateChanged(sender, eventArgs) {
            
            var useDate = sender.get_selectedDate();
            var SenderId = sender.get_id();
            var RadDatePicker2 = $telerik.findDatePicker(SenderId.replace("calMasterDueDate", "calMasterAssignmentDate"));
            RadDatePicker2.set_maxDate(useDate);
            customDialog({ title: 'Confirm?', maximize: true, maxwidth: 500, maxheight: 120, content: 'Do you want to reset the dates for all students?<br/>', dialog_style: 'confirm' }, [{ title: 'No' }, { title: 'Yes', callback: updateDueDate, argArray: [useDate] }]);
            updated();
        }

        function updateAssignmentDate(date) {
            var selectedGrid = $find('<%=grdSelected.ClientID %>').get_masterTableView();
            var dueDate = $telerik.findDatePicker('<%=calMasterAssignmentDate.ClientID%>');
            var maxDate = dueDate.get_maxDate();
            $.each(selectedGrid.get_dataItems(), function (index, value) {                
                var mindate = new Date(0001, 0, 1);
                //var maxdate = new Date(9999, 11, 31);
                value.findControl('calAssignmentDate').set_minDate(mindate);
                value.findControl('calAssignmentDate').set_maxDate(maxDate);
                value.findControl('calAssignmentDate').set_selectedDate(date);
            });
        }

        function updateDueDate(date) {
            var selectedGrid = $find('<%=grdSelected.ClientID %>').get_masterTableView();
            var isvalid = true;

            $.each(selectedGrid.get_dataItems(), function (index, value) {
                var selectedssigndate = value.findControl('calAssignmentDate').get_selectedDate();
                if (selectedssigndate > date)
                    isvalid = false;
            });

            if (isvalid == false) {
                alert("Due date should be equal or greater than Assignment date or Current date.");
                return false;
            }
            else {
                $.each(selectedGrid.get_dataItems(), function (index, value) {
                    var selectedssigndate = value.findControl('calAssignmentDate').get_selectedDate();
                    if (selectedssigndate <= date)
                        value.findControl('calDueDate').set_selectedDate(date);
                });
            }
        }
        //looks for an element that has been registered with the global array 
        //requires that we emit a registration script block for each server control 
        function GetRegisteredServerElement(serverID, useDate, calender) {
            var clientID = "";
            if (calender == 'assignment') {
                for (var i = 0; i < assignmentDateElements.length; i++) {
                    clientID = assignmentDateElements[i];
                    if (clientID.indexOf(serverID) >= 0)
                        var rdpicker = $find(clientID);
                    rdpicker.set_selectedDate(useDate);
                }
            } else {

                for (var i = 0; i < dueDateElements.length; i++) {
                    clientID = dueDateElements[i];
                    if (clientID.indexOf(serverID) >= 0)
                        var rdpicker = $find(clientID);
                    rdpicker.set_selectedDate(useDate);
                }
            }
        }

        function updated() {           
            var modalWin = getCurrentCustomDialog();
            if (modalWin != null) {
                modalWin.remove_beforeClose(modalWin.confirmBeforeClose);
                modalWin.remove_beforeClose(callmessage);
                modalWin.add_beforeClose(callmessage);
            }
        }
        function callmessage(sender, arg) {           
            arg.set_cancel(true);
            customDialog({
                title: "Confirm changes",
                maxheight: 100,
                maxwidth: 500,
                maximize: true,
                autoSize: false,
                content: "Do you want to save the changes to the current assignments?", dialog_style: 'confirm'
            },
                    [{ title: 'No', callback: confirmCallbackCloseClient }, { title: 'Yes', callback: confirmCallbackFunctionClient }]);
        }
       
        function Saved()
        {
            var modalWin = getCurrentCustomDialog();
            modalWin.remove_beforeClose(callmessage);
        }
       
    </script>
    <style type="text/css">
        .floatLeft {
            float: left;
        }
        #container {
            width: 870px;
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
            margin-left: 5px;
            float: left;
            width: 85%;
        }

        .rightPanel {
            margin-left: 5px;
            float: left;
            width: 95%;
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
            width: 95%;
            display: inline-block;
            font-size: large;
            margin-top: 10px;
            margin-bottom: 10px;
        }

        .detailGrid {
            margin-top: 5px;
            margin-bottom: 5px;
            margin-left: auto;
            margin-right: 5px;
            padding-bottom: 5px;
            width: 96%;
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
            margin-top: 5px;
            font-size: small;
        }

        .RadGrid_Transparent .rgRow td, .RadGrid_Transparent .rgAltRow td {
            border-color: black;
        }

        .RadGrid_Transparent .rgHeader, .RadGrid_Transparent th.rgResizeCol, .RadGrid_Transparent .rgHeaderWrapper {
            border-color: black;
        }
    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" RunSearchOnPageLoad="False"/>
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
    <e3:DropDownList ID="ctrlGroup" CriteriaName="Group" runat="server" Text="Groups" EmptyMessage="Select a Group" DataTextField="Name" DataValueField="ID" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <div id="container" style="overflow-y: auto; overflow-x: hidden; height: inherit">
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
                        <telerik:AjaxUpdatedControl ControlID="calenders" />
                        <telerik:AjaxUpdatedControl ControlID="savedState" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdSelected">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdSelected" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="grdAvailable" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="grdDetail" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="lblAvailable" />
                        <telerik:AjaxUpdatedControl ControlID="lblSelected" />
                        <telerik:AjaxUpdatedControl ControlID="calAssignmentDate" />
                        <telerik:AjaxUpdatedControl ControlID="calDueDate" />
                        <telerik:AjaxUpdatedControl ControlID="calenders" />
                        <telerik:AjaxUpdatedControl ControlID="savedState" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdDetail">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdDetail" LoadingPanelID="updPanelLoadingPanel" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="btnSave">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="btnSave" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="grdSelected" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="grdAvailable" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="grdDetail" LoadingPanelID="updPanelLoadingPanel" />
                        <telerik:AjaxUpdatedControl ControlID="lblAvailable" />
                        <telerik:AjaxUpdatedControl ControlID="lblSelected" />
                        <telerik:AjaxUpdatedControl ControlID="calAssignmentDate" />
                        <telerik:AjaxUpdatedControl ControlID="calDueDate" />
                        <telerik:AjaxUpdatedControl ControlID="isCompleted" />
                        
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>

        </telerik:RadAjaxManagerProxy>

        <div class="detailGrid">
            <div>
                <span style="font-weight: bold; bottom: 50px; clear: both; width: 100%;">Assign Instructions to Students:</span><br />
                <div style="padding-top: 10px;">Select Students who needs to have below instruction Assigned. Set Assigned and Due date as applicable to all students using available selection option under Selected. If you need to change the dates for a selected student dates can be changed using dates next to the student name.</div>
            </div>
            <br />
            <telerik:RadGrid runat="server" ID="grdDetail" BorderStyle="Solid" BorderColor="black" BorderWidth="1px" Width="815px" AllowSorting="false" OnNeedDataSource="DisplayDetailForInstructionMaterial" OnItemDataBound="grdDetails_ItemDataBound">
                <MasterTableView AutoGenerateColumns="False" AllowSorting="false" DataKeyNames="DocumentId">
                    <Columns>
                        <telerik:GridBoundColumn DataField="NodeName" HeaderText="Name" UniqueName="Name">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Description" HeaderText="Description" UniqueName="Description">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="FriendlyName" HeaderText="Type">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn DataField="ExpirationDate" HeaderText="Expiration Date">
                            <ItemTemplate>
                                <span><%# Eval("ExpirationDate","{0:MM/dd/yy}")=="12/31/99" ? "NA" : Eval("ExpirationDate","{0:MM/dd/yy}")%></span>
                            </ItemTemplate>

                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="StudentCount" HeaderText="Student Count">
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
                        <label class="panelLabel">Available</label></td>
                    <td>
                        <label class="panelLabel">Selected</label></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <div id="calenders" runat="server" style="height: 30px; background-color: white; margin-bottom: 5px; width: 70%; float: right; padding-top: 2%; display: -ms-flexbox inline-block;">
                            <span style="margin-left: 1%; padding-top: 5px; float: left; width: 106px;">Assignment Date </span>
                            <telerik:RadDatePicker ID="calMasterAssignmentDate" MinDate="1/1/0001" runat="server" Width="100px" CssClass="floatLeft">
                                <ClientEvents OnDateSelected="onMasterAssignmentDateChanged" />
                            </telerik:RadDatePicker>
                            <span style="margin-left: 1%;  padding-top: 5px; float: left; width: 57px;">Due Date </span>
                            <telerik:RadDatePicker ID="calMasterDueDate" MinDate="1/1/0001" runat="server" Width="100px" CssClass="floatLeft">
                                <ClientEvents OnDateSelected="onMasterDueDateChanged" />
                            </telerik:RadDatePicker>
                        </div>

                    </td>
                </tr>

                <tr>
                    <td>
                        <div class="leftPanel">

                            <telerik:RadGrid runat="server" ID="grdAvailable" BorderStyle="Solid" BorderColor="black" BorderWidth="1px" Height="330px" Width="270px" OnNeedDataSource="DisplayStudentsAvailable" AllowSorting="false">
                                <MasterTableView AutoGenerateColumns="False" AllowSorting="false" DataKeyNames="ID">
                                    <Columns>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="60px">
                                            <HeaderTemplate>
                                                <asp:LinkButton Text="Add All" Font-Underline="True" OnClick="btnSelectAll_Click" runat="server" OnClientClick="updated();"></asp:LinkButton>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton Text="Add" OnClick="btnAdd_Click" runat="server" OnClientClick="updated();"></asp:LinkButton>
                                            </ItemTemplate>

                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="Name" HeaderText="Student Name" />
                                        <telerik:GridBoundColumn DataField="SchoolName" HeaderText="School Name" />
                                    </Columns>
                                </MasterTableView>
                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="True" AllowScroll="True" />
                                </ClientSettings>
                            </telerik:RadGrid>
                            <div class="countLabels">
                                <asp:Label runat="server" ID="lblAvailable" Text="# Students Available" />
                            </div>
                        </div>
                    </td>
                    <td>
                        <div class="rightPanel">


                            <telerik:RadGrid runat="server" ID="grdSelected" Height="330px" BorderStyle="Solid" BorderColor="black" BorderWidth="1px" Width="550px" OnNeedDataSource="DisplaySelectedStudentsForIM" OnItemDataBound="grdSelected_ItemDataBound" AllowSorting="false">
                                <MasterTableView AutoGenerateColumns="False" AllowSorting="false" DataKeyNames="ID" NoMasterRecordsText="">
                                    <Columns>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="80px">
                                            <HeaderTemplate>
                                                <asp:LinkButton Text="Remove All" Font-Underline="True" OnClick="btnRemoveAll_Click" runat="server" OnClientClick="updated();"></asp:LinkButton>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton Text="Remove" OnClick="btnRemove_Click" runat="server" OnClientClick="updated();"></asp:LinkButton>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="Name" HeaderText="Student Name" HeaderStyle-Width="80px" />
                                        <telerik:GridBoundColumn DataField="TeacherName" HeaderText="Teacher Name" HeaderStyle-Width="80px" />
                                        <telerik:GridBoundColumn DataField="SchoolName" HeaderText="School Name" HeaderStyle-Width="80px" />
                                        <telerik:GridBoundColumn DataField="IsNew" HeaderText="" Display="false" />
                                        <telerik:GridTemplateColumn HeaderText="Assignment Date" HeaderStyle-Width="100px">
                                            <ItemTemplate>
                                                <telerik:RadDatePicker ID="calAssignmentDate" MinDate="1/1/0001" runat="server" Width="100px" DbSelectedDate='<%# Bind("AssignmentDate") %>' OnSelectedDateChanged="calAssignmentDate_SelectedDateChanged">
                                                    <ClientEvents OnDateSelected="onDateChanged" />
                                                </telerik:RadDatePicker>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Due Date" HeaderStyle-Width="100px">
                                            <ItemTemplate>
                                                <telerik:RadDatePicker ID="calDueDate" MinDate="1/1/0001" runat="server" Width="100px" DbSelectedDate='<%# Bind("DueDate") %>' OnSelectedDateChanged="calDueDate_SelectedDateChanged">
                                                    <ClientEvents OnDateSelected="onDueDateChanged" />
                                                </telerik:RadDatePicker>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>

                                    </Columns>
                                </MasterTableView>
                                <ClientSettings>
                                    <Scrolling UseStaticHeaders="True" AllowScroll="True" />
                                </ClientSettings>
                            </telerik:RadGrid>
                            <div class="countLabels">
                                <asp:Label runat="server" ID="lblSelected" Text="# Students Selected" />
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
            <div style="float: right; bottom: 10px; clear: both; margin-right: 0px;">
                <telerik:RadButton ButtonType="StandardButton" ID="btnSave" OnClick="btnSave_Click" runat="server" Text="Save" Width="70"></telerik:RadButton>
                <div style="display:none">
                <telerik:RadButton ButtonType="StandardButton" ID="btnSaveClient"  OnClick="btnSaveClient_Click" runat="server" Text="Save" Width="70"></telerik:RadButton>
                    </div>
            </div>
        </div>
    </div>  
    <asp:HiddenField ID="isCompleted" runat="server" ClientIDMode="Static" />      
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.0/themes/smoothness/jquery-ui.css" />
    <script src="//code.jquery.com/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="//code.jquery.com/ui/1.11.0/jquery-ui.js" type="text/javascript"></script>
    <link rel="stylesheet" href="/resources/demos/style.css" />
</asp:Content>
