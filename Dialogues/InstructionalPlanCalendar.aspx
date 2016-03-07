<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstructionalPlanCalendar.aspx.cs"
    Inherits="Thinkgate.Dialogues.InstructionalPlanCalendar" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4"
        rel="stylesheet" type="text/css" runat="server" />
    <title>Instructional Plan Calendar</title>
    <base target="_self" />
    <script type="text/javascript">
        var insertedAppointment = false;
        function insertAppointment(sender, e) {
            if (insertedAppointment) return;
            if (newCalendarEventSubject.length == 0) return;

            var slot = sender.get_activeModel().getTimeSlotFromDomElement(e.get_domEvent().target);
            var newAppointment = new Telerik.Web.UI.SchedulerAppointment();
            newAppointment.set_start(slot.get_startTime());
            newAppointment.set_end(slot.get_endTime());
            newAppointment.set_subject(newCalendarEventSubject);

            var scheduler = $find('InstructionalPlanScheduler');
            scheduler.insertAppointment(newAppointment);
            insertedAppointment = true;
        }

        function Export(sender, e) {
            $find("RadAjaxManager1").__doPostBack(sender.name, "");
        }
    </script>
    <style type="text/css">
        .RadScheduler .rsExportButton
        {
            position: absolute;
            bottom: 0;
            right: 0;
            border: 0;
            height: 16px;
            width: 16px;
            background: url('../Images/smallOutlook.gif') no-repeat center center;
        }
        
        .RadScheduler .rsAllDayRow .rsExportButton
        {
            right: 20px;
            height: 16px;
            width: 16px;
            background: url('../Images/smallOutlook.gif');
        }
    </style>
</head>
<body style="background-color: white; font-family: Arial, Sans-Serif; font-size: 10pt;">
    <form runat="server" id="mainForm" method="post">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
            <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
            <asp:ScriptReference Path="~/scripts/master.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
        Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" ClientIDMode="Static" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="Panel1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="InstructionalPlanScheduler" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
    </telerik:RadSkinManager>
    <telerik:RadAjaxLoadingPanel runat="Server" ID="RadAjaxLoadingPanel1" />
    <asp:UpdatePanel ID="updatePanel" runat="server">
        <ContentTemplate>
            <script type="text/javascript">                var newCalendarEventSubject = '<%= newEventTitle %>';</script>
            <div style="text-align: center;">
                <asp:Panel runat="server" ID="pnlHeader" Visible="false">
                    <span style="float: left">Class:
                        <telerik:RadComboBox runat="server" ID="ddlClassList">
                        </telerik:RadComboBox>
                    </span><span style="float: right">
                        <asp:ImageButton runat="server" ID="exportButton" ImageUrl="~/Images/exportButton.gif"
                            AlternateText="Export All to iCalendar" OnClientClick="Export(this, event); return false;"
                            OnClick="exportButton_Click" />
                    </span>
                </asp:Panel>
                <telerik:RadScheduler ID="InstructionalPlanScheduler" ClientIDMode="Static" runat="server"
                    SelectedView="MonthView" ShowViewTabs="true" DataKeyField="ID" DataStartField="StartDate"
                    TimelineView-UserSelectable="false" DataEndField="EndDate" DataSubjectField="Subject" DataDescriptionField="Description"
                    ShowHoursColumn="false" StartEditingInAdvancedForm="true" StartInsertingInAdvancedForm="true"
                    Height="580" Width="775" OnAppointmentInsert="InstructionalPlanScheduler_AppointmentInsert"
                    OnAppointmentCommand="InstructionalPlanScheduler_AppointmentCommand" OnAppointmentUpdate="InstructionalPlanScheduler_AppointmentUpdate"
                    OnClientTimeSlotClick="insertAppointment" OnAppointmentCreated="InstructionalPlanScheduler_AppointmentCreated">
                    <AdvancedForm Modal="true" Enabled="false" />
                    <AppointmentTemplate>
                        <asp:HyperLink ID="previewLink" runat="server" NavigateUrl="" Target="_blank">
                            <asp:Image ID="previewImage" runat="server" ImageUrl="../Images/ViewPage.png" Width="15" />
                        </asp:HyperLink>                       
                        <%# Eval("Subject") %>
                        <div style="text-align: right;">
                            <asp:Button runat="server" ID="Button1" CssClass="rsExportButton" ToolTip="Export to iCalendar"
                                CommandName="Export" OnClientClick="Export(this, event); return false;" Style="cursor: pointer;
                                cursor: hand;" />
                        </div>
                    </AppointmentTemplate>
                </telerik:RadScheduler>
                <span>Select the white space within a single day to add an event</span>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
