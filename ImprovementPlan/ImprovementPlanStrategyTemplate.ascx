<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImprovementPlanStrategyTemplate.ascx.cs" Inherits="Thinkgate.ImprovementPlan.ImprovementPlanStrategyTemplate" ValidateRequestMode="Disabled" %>

<style type="text/css">
    .rowHeader {
        border-top: 2px solid black;
        border-bottom: 2px solid black;
        background-color: #c3c3c3;
    }

    input[disabled="disabled"] {
        color: black;
    }

    .border-2px {
        border: 2px solid black;
    }

    .viewModeCss {
        border: none;
        background-color: transparent;
        overflow: hidden;
        vertical-align: middle;
    }

    .white-forecolor {
        color: white;
    }

    .width-80per {
        width: 10px;
    }


    .centered {
        margin: 0 auto;
        text-align: left;
        width: 98%;
    }


    .text-align-center {
        text-align: center;
    }

    .full-width {
        width: 100%;
    }

    .width-half {
        width: 75%;
    }

    .width-90per {
        width: 90%;
    }

    .width-40per {
        width: 40%;
    }

    .width-99per {
        width: 99%;
    }

    .pad-left-1 {
        padding-left: 1%;
    }

    .text-align-left {
        text-align: left;
    }

    .text-align-right {
        text-align: right;
    }

    #tbHeader, #tbStrategy {
        width: 100%;
    }


        #tbHeader td:first-child + td + td {
            text-align: right;
        }

        #tbHeader td:first-child + td {
            width: 63%;
        }

        #tbStrategy td:first-child + td {
            width: 45%;
            height: 40px;
        }



    .tbStrategy {
        background-color: brown;
        color: white;
        border-top: 2px solid black;
        border-bottom: 2px solid black;
    }

    .text-area-height {
        height: 50px;
    }

    .tbSmartGoal {
        width: 100%;
    }


    #tbSmartGoal td:first-child + td {
        width: 90%;
    }

    table#tbSmartGoal > tbody > tr:first-child > td:first-child {
        background-color: brown;
        border-top: 2px solid black;
        border-bottom: 2px solid black;
        border-right: 2px solid black;
        color: white;
    }

    .txtSmartGoalCss {
    }

    .tbStrategyHeader {
        text-align: center;
        font-size: smaller;
        font-weight: bold;
        border-top: 2px solid black;
        border-bottom: 2px solid black;
        background-color: #c3c3c3;
    }

        .tbStrategyHeader td {
            border-right: 2px solid black;
        }

    .tbStrategyItem {
        text-align: center;
        font-size: smaller;
        font-weight: bold;
        border-bottom: 2px solid black;
    }

        .tbStrategyItem td {
            border-right: 2px solid black;
        }

    .footer-heading {
        font-size: smaller;
        text-align: center;
        font-style: italic;
    }

    .button-css {
        background-color: #7cb8eb;
        border: 0px;
        font-weight: bold;
        color: white;
        width: 100px;
        margin-right: 50px;
        height: 30px;
        cursor: pointer;
    }

    ul li {
        float: left;
        list-style: none;
    }

    .font-family {
        font-family: Segoe UI;
    }

    .font-bold {
        font-weight: bold;
        font-size: 14px;
    }

    .padd-bott-1 {
        padding-bottom: 1px;
    }
</style>

<script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);

    function EndRequest(sender, args) {

        if (args.get_error() == undefined) {
            setDateWidth();
        }
    }



    $(document).ready(function () {
        setDateWidth();
    });

    function setDateWidth() {
        $('.rcTable').css('width', '100%');
    }


    function showCoverValidation() {
        var height = $telerik.$(window).height();
        var width = $telerik.$(window).width();

        var height_Cal = (height * 0.1);
        var width_Cal = (width * 0.35);


        customDialog({
            title: "Alert",
            height: height_Cal,
            width: width_Cal,
            autoSize: false,
            content: "Please confirm your content is saved before returning to the cover page. <br/> Select OK to proceed or CANCEL to return to the current form.",
            dialog_style: "alert",
        }, [{ title: "Cancel" }, { title: "Ok", callback: redirectToCover }]);
    }

    function redirectToCover() {
        __doPostBack('btnCover', '');
    }

    function deleteValidation() {

        var height = $telerik.$(window).height();
        var width = $telerik.$(window).width();

        var height_Cal = (height * 0.1);
        var width_Cal = (width * 0.35);

        var strategyDllCtrl = document.getElementById('ddlStrategy');

        if (strategyDllCtrl.selectedIndex == -1) {

            customDialog({
                title: 'Alert',
                height: height_Cal,
                width: width_Cal,
                autoSize: false,
                content: 'No Strategy available to delete',
                dialog_style: "alert",
            }, [{ title: "Ok" }]);
        }
        else {

            customDialog({
                title: 'Alert',
                height: height_Cal,
                width: width_Cal,
                autoSize: false,
                content: 'Are you certain you would like to delete this Strategy entry? <br/> Select OK to proceed or CANCEL to return to the current form.',
                dialog_style: "alert",

            }, [{ title: "Cancel" }, { title: "Ok", callback: removeStrategyPlan }]);
        }
    }


    function removeStrategyPlan() {
        __doPostBack('btnDelete', '');
    }

    function showSaveValidation(sender) {

        var strategyTextCtrl = document.getElementById('txtStrategy');
        var strategyDllCtrl = document.getElementById('ddlStrategy');


        //if (strategyDllCtrl.selectedIndex == -1)
        //{
        //    var height = $telerik.$(window).height();
        //    var width = $telerik.$(window).width();

        //    var height_Cal = (height * 0.1);
        //    var width_Cal = (width * 0.35);


        //    customDialog({
        //        title: 'Alert',
        //        height: height_Cal,
        //        width: width_Cal,
        //        autoSize: false,
        //        content: 'A Strategy must be added before saving',
        //        dialog_style: "alert",
        //    }, [{ title: "Ok" }]);
        //}
        //else 

        if (strategyTextCtrl.value.trim() == '') {
            var height = $telerik.$(window).height();
            var width = $telerik.$(window).width();

            var height_Cal = (height * 0.1);
            var width_Cal = (width * 0.35);


            customDialog({
                title: 'Alert',
                height: height_Cal,
                width: width_Cal,
                autoSize: false,
                content: 'A Strategy must be entered before the template can be saved.',
                dialog_style: "alert",
            }, [{ title: "Ok" }]);

        }
        else {
            __doPostBack('btnSave', '');
        }

        return false;
    }

    function showSaveValidationForAdd(sender) {

        var strategyTextCtrl = document.getElementById('txtStrategy');

        if (strategyTextCtrl.value.trim() == '') {
            var height = $telerik.$(window).height();
            var width = $telerik.$(window).width();

            var height_Cal = (height * 0.1);
            var width_Cal = (width * 0.35);


            customDialog({
                title: 'Alert',
                height: height_Cal,
                width: width_Cal,
                autoSize: false,
                content: 'A Strategy must be entered before adding',
                dialog_style: "alert",
            }, [{ title: "Ok" }]);

        }
        else {
            __doPostBack('btnSaveAndAdd', '');
        }

        return false;
    }



    function onSelectedIndexChange() {
        __doPostBack('ddlStrategy', '');
    }

</script>

<div id="dvContainer" class="centered border-2px font-family">
    <telerik:RadAjaxPanel ID="improvementAjaxPanel" runat="server" LoadingPanelID="improvementPlanProgress" Width="100%">
        <div id="dvHeader">

            <h3 class="text-align-center">
                <b>
                    <asp:Label ID="lblPageTitle" runat="server"></asp:Label>
                </b>
            </h3>

            <table id="tbHeader" class="full-width">
                <tr>
                    <td>District:
                    </td>
                    <td>
                        <asp:Label ID="lblDistrict" runat="server"></asp:Label>
                    </td>
                    <td>School Year: &nbsp;
                    </td>
                    <td>
                        <asp:Label ID="lblSchoolYear" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSchool" runat="server">School:</asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblSchoolVal" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>



        </div>

        <div id="dvBody">
            <%--Strategy And Person Responsbile Info--%>
            <table id="tbStrategy" class="full-width tbStrategy">
                <tr>
                    <td>Strategy:
                        <label style="color: red">*</label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlStrategy" runat="server" CssClass="width-40per" ClientIDMode="Static" onchange="onSelectedIndexChange(); return false;">
                        </asp:DropDownList>
                    </td>
                    <td>Person(s) Responsible:
                    </td>
                    <td></td>
                </tr>

                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="txtStrategy" runat="server" CssClass="text-area-height width-half" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                    </td>
                    <td colspan="2">
                        <asp:TextBox ID="txtPersonResponsibles" runat="server" ClientIDMode="Static" CssClass="full-width text-area-height width-half" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
            </table>

            <br />
            <table id="tbSmartGoal" class="full-width">
                <tr>
                    <td>SMART Goals:
                    </td>
                    <td class="pad-left-1">
                        <asp:Button ID="btnAddSmartGoal" runat="server" Text="Add New" Width="150px" CssClass="button-css" OnClick="btnAddSmartGoal_Click" /></td>
                </tr>
                <tr>
                    <td class="full-width">

                        <asp:Repeater ID="rptSmartGoal" runat="server" OnItemDataBound="rptSmartGoal_ItemDataBound">
                            <ItemTemplate>
                                <br />
                                <table class="full-width">
                                    <tr>
                                        <td style="width: 5%" />
                                        <td>
                                            <asp:Label ID="lblSmartGoal" runat="server" CssClass="font-bold">SMART Goal <%# string.Format("{0} -", Container.ItemIndex + 1)  %></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 5%" />
                                        <td>
                                            <asp:TextBox ID="txtSmartGoal" runat="server" CssClass="full-width text-area-height txtSmartGoalCss"
                                                TextMode="MultiLine"
                                                SmartGoalID='<%# DataBinder.Eval(Container.DataItem, "ID") %>'
                                                StrategyID='<%# DataBinder.Eval(Container.DataItem, "StrategyID") %>'
                                                Text='<%# DataBinder.Eval(Container.DataItem, "SmartGoal") %>'
                                                InitialValue='<%# DataBinder.Eval(Container.DataItem, "InitialSmartGoal") %>'></asp:TextBox>

                                            <asp:Label ID="lblSmartGoalDetail" Font-Size="Small" runat="server" CssClass="full-width txtSmartGoalCss" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "SmartGoal") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />

                            </ItemTemplate>
                        </asp:Repeater>

                    </td>
                </tr>
            </table>
            <br />
            <div class="padd-bott-1">
                <asp:Button ID="btnAddAction" runat="server" Text="Add New" Width="150px" CssClass="button-css" OnClick="btnAddAction_Click" />
            </div>

            <asp:DataGrid ID="dgActions" runat="server" AllowSorting="false" AutoGenerateColumns="false" Width="100%" OnItemDataBound="dgActions_ItemDataBound">
                <Columns>
                    <asp:TemplateColumn HeaderText="District Strategic Goal" HeaderStyle-Width="13%">
                        <%--  <ItemTemplate>
                            <%#Container.DataItem("ShipVia")%>
                        </ItemTemplate>--%>
                        <ItemTemplate>
                            <asp:HiddenField ID="hidActionID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "ID") %>' />
                            <asp:DropDownList ID="ddlStrategyGoal" runat="server" Width="100%"
                                InitialValue='<%# DataBinder.Eval(Container.DataItem, "InitialStrategicGoalID") %>'>
                            </asp:DropDownList>
                            <div style="text-align: left">
                                <asp:Label ID="lblStrategyGoal" runat="server" Font-Size="Small" CssClass="width-90per" Visible="false" Font-Bold="false"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Action Steps" HeaderStyle-Width="27%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtActionSteps" runat="server" TextMode="MultiLine" Height="40px" CssClass="width-99per"
                                InitialValue='<%# DataBinder.Eval(Container.DataItem, "InitialActionStep") %>'></asp:TextBox>
                            <div style="text-align: left">
                                <asp:Label ID="lblActionSteps" runat="server" Font-Size="Small" CssClass="width-99per" Visible="false" Font-Bold="false"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Person(s) Responsible" HeaderStyle-Width="10%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtPersonResponsible" runat="server" TextMode="MultiLine" Height="40px" CssClass="width-99per"
                                InitialValue='<%# DataBinder.Eval(Container.DataItem, "InitialPersonResponsible") %>'></asp:TextBox>
                            <asp:Label ID="lblPersonResponsible" runat="server" Font-Size="Small" CssClass="width-99per" Visible="false" Font-Bold="false"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Start Date" HeaderStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <telerik:RadDatePicker ID="rdpStartDate" runat="server" Style="width: 90%"
                                InitialValue='<%# DataBinder.Eval(Container.DataItem, "InitialStartDate") %>'>
                            </telerik:RadDatePicker>
                            <asp:Label ID="lblStartDate" runat="server" Font-Size="Small" CssClass="width-90per" Visible="false" Font-Bold="false"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Finish Date" HeaderStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <telerik:RadDatePicker ID="rdpFinishDate" runat="server" Style="width: 90%"
                                InitialValue='<%# DataBinder.Eval(Container.DataItem, "InitialFinishDate") %>'>
                            </telerik:RadDatePicker>
                            <asp:Label ID="lblFinishDate" runat="server" Font-Size="Small" CssClass="width-90per" Visible="false" Font-Bold="false"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Status <br /> (See Key Below)" HeaderStyle-Width="8%">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="full-width"
                                InitialValue='<%# DataBinder.Eval(Container.DataItem, "InitialStatusID") %>'>
                            </asp:DropDownList>
                            <asp:Label ID="lblStatus" runat="server" Font-Size="Small" CssClass="width-90per" Visible="false" Font-Bold="false"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Resource/Costs" HeaderStyle-Width="8%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtResrouceCosts" runat="server" TextMode="MultiLine" Height="40px" CssClass="width-99per"
                                InitialValue='<%# DataBinder.Eval(Container.DataItem, "InitialResourceCosts") %>'></asp:TextBox>
                            <asp:Label ID="lblResourceCost" runat="server" Font-Size="Small" CssClass="width-99per" Visible="false" Font-Bold="false"></asp:Label>
                        </ItemTemplate>                        
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderStyle-Width="20%">
                        <HeaderTemplate>
                            <i>Expected Results</i>
                            <br />
                            <div style="font-size: x-small">
                                Means of Evaluation for Implementation of Strategy and Impact on Student Learning (Artifacts & Evidence)
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="txtExpectedResults" runat="server" TextMode="MultiLine" Height="40px" CssClass="width-99per"
                                InitialValue='<%# DataBinder.Eval(Container.DataItem, "InitialExpectedResults") %>'></asp:TextBox>
                            <asp:Label ID="lblExpectedResults" runat="server" Font-Size="Small" CssClass="width-99per" Visible="false" Font-Bold="false"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
                <HeaderStyle HorizontalAlign="Center" Font-Bold="true" Font-Size="Small" BackColor="LightGray" />

            </asp:DataGrid>
        </div>

        <div id="dvFooter" class="full-width">

            <div id="dvFooterHeading" class="footer-heading full-width">
                The vision of the Paulding County School District is to provide a safe, healthy, supportive environment focused on learning and committed to 
                    high academic achievement. Through the shared responsibility of all stakeholders, students will be prepared as lifelong learners and as participating, contributing
                    members of our dynamic and diverse community.                    
            </div>



            <br />
            <div id="dvActions" runat="server">
                <ul>
                    <li>
                        <asp:Button ID="btnCover" runat="server" Text="Cover" ClientIDMode="Static" CssClass="button-css" OnClientClick="showCoverValidation(this); return false;" AutoPostBack="false" /></li>
                    <li>
                        <asp:Button ID="btnSave" runat="server" Text="Save" ClientIDMode="Static" CssClass="button-css" OnClientClick="javascript: showSaveValidation(this); return false;" AutoPostBack="false" />
                    </li>
                    <li>
                        <asp:Button ID="btnSaveAndAdd" runat="server" Text="Save & Add" ClientIDMode="Static" CssClass="button-css" OnClientClick="javascript: showSaveValidationForAdd(this); return false;" AutoPostBack="false" />
                    </li>
                    <li>
                        <asp:Button ID="btnDelete" runat="server" ClientIDMode="Static" Text="Delete" CssClass="button-css" OnClientClick="deleteValidation(); return false;" AutoPostBack="false" /></li>

                </ul>
            </div>

            <br />
            <br />
            <%-- <div id="dvFinalize">
                Finalize: &nbsp<asp:CheckBox ID="chkFinalize" runat="server" Text="" />
            </div>--%>

            <div id="dvStatusKey">
                Status Key: D=Draft IP=In-Progress C=Cancelled P=Postponed CP=Complete
            </div>
        </div>

    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="improvementPlanProgress" runat="server" Width="100%" />
</div>
