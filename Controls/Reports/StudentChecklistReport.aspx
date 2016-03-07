<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Search.Master" Title="Student Checklist Report" CodeBehind="StudentChecklistReport.aspx.cs" EnableViewState="true" Inherits="Thinkgate.Controls.Reports.StudentChecklistReport" %>

<%@ Register TagPrefix="telerik" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="telerik" TagName="RadDropDownList" Src="~/Controls/E3Criteria/RadDropDownList.ascx" %>
<%@ Register TagPrefix="telerik" TagName="RadCheckBoxList" Src="~/Controls/E3Criteria/RadCheckBoxList.ascx" %>


<%@ MasterType VirtualPath="~/Search.Master" %>


<asp:Content ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <telerik:SearchAndClear ID="SearchAndClear" OnClientClear="rtsTabs_OnClientTabSelecting" runat="server" StarterText="Search" AfterSearchText="Update Results" />
</asp:Content>

<asp:Content ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <telerik:RadAjaxLoadingPanel ID="ralPanel" runat="server">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxPanel runat="server" LoadingPanelID="ralPanel" ClientEvents-OnResponseEnd="Page_Initialize()" ClientEvents-OnRequestStart="OnRequestStartOfPanel">

        <telerik:RadTabStrip ID="rtsTabs" OnClientTabSelecting="rtsTabs_OnClientTabSelecting" runat="server" SelectedIndex="0" AutoPostBack="true" Skin="Office2010Black" OnInit="rtsTabs_Init" Align="Justify">
        </telerik:RadTabStrip>
        <br />
        <telerik:RadDropDownList ID="ddlStudentGrade" EnableLoadOnDemand="True" OnRemoveByKeyHandler="RemoveBySelectedItems()" OnChange="ddlStudentGrade_OnChange()" IsNotAllowToEnterText="True" OnClientDropDownOpened="OnClientDropDownOpened" OnClientItemsRequesting="BindDataOnDemand" CriteriaName="StudentGrade" runat="server" EmptyMessage="Select a Student Grade" Text="Student Grade" DataTextField="Text" DataValueField="Value" Required="True" />
        <telerik:RadCheckBoxList ID="cblMonth" EnableCheckAllItemsCheckBox="True" OnRemoveByKeyHandler="RemoveBySelectedItems()" EnableLoadOnDemand="True" CriteriaName="Month" OnChange="cblMonth_OnChange()" IsNotAllowToEnterText="True" OnClientDropDownOpened="OnClientDropDownOpened" OnClientItemsRequesting="BindDataOnDemand" runat="server" Text="Month" EmptyMessage="Select a Month" DataTextField="Month" DataValueField="SequenceMonth" Required="True" />
        <telerik:RadDropDownList ID="ddlSchool" EnableLoadOnDemand="True" OnChange="ddlSchool_OnChange()" OnRemoveByKeyHandler="RemoveBySelectedItems()" IsNotAllowToEnterText="True" OnClientDropDownOpened="OnClientDropDownOpened" OnClientItemsRequesting="BindDataOnDemand" CriteriaName="School" runat="server" Text="School" EmptyMessage="Select a School" DataTextField="SchoolName" DataValueField="SchoolId" />
        <telerik:RadDropDownList ID="ddlStundentName" Height="200px" EnableVirtualScrolling="true" OnClientDropDownOpened="OnClientDropDownOpened" EnableLoadOnDemand="True" CriteriaName="StudentName" OnClientItemsRequesting="BindDataOnDemand" runat="server" Text="Student Name" EmptyMessage="Select a Student" DataTextField="Student_Name" DataValueField="Student_Id" />
        <telerik:RadDropDownList ID="ddlCounselor" EnableLoadOnDemand="True" CriteriaName="Counselor" OnClientDropDownOpened="OnClientDropDownOpened" OnClientItemsRequesting="BindDataOnDemand" runat="server" Text="Counselor" DataTextField="Name" EmptyMessage="Select a Counselor" DataValueField="counselorId" />

        <telerik:RadCodeBlock runat="server">
            <span style="<%= this.rtsTabs.SelectedIndex != (int)SearchTab.Basic ? "": "display:none" %>">
                <telerik:RadCheckBoxList ID="ddlItemNumber" EnableLoadOnDemand="True" OnClientItemsRequesting="BindDataOnDemand" IsNotAllowToEnterText="True" OnClientDropDownOpened="OnClientDropDownOpened" CriteriaName="Item_Number" runat="server" Text="Item Number" EmptyMessage="Select a Item Number" DataTextField="Text" DataValueField="Value" />
            </span>
        </telerik:RadCodeBlock>
    </telerik:RadAjaxPanel>
</asp:Content>

<asp:Content ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">

    <telerik:RadAjaxPanel LoadingPanelID="ralPanel_Tree" runat="server" ClientEvents-OnRequestStart="OnRequestStartOfPanel">
        <asp:HiddenField ID="hdnGrade" runat="server" />
        <asp:HiddenField ID="hdnMonth" runat="server" />
        <asp:HiddenField ID="hdnItemNumber" runat="server" />
        <asp:ImageButton ID="imgExport" ClientIDMode="Static" Visible="false" runat="server" OnClick="imgExport_Click" ImageUrl="~/Images/Toolbars/excel_button.png" Text="Export to Excel" />
        <telerik:RadTreeList ID="radTreeResults" ClientSettings-Scrolling-AllowScroll="true" Visible="false" Height="600px" runat="server" AutoGenerateColumns="false" ParentDataKeyNames="ParentId" DataKeyNames="Id" OnNeedDataSource="radTreeResults_NeedDataSource" OnItemDataBound="radTreeResults_ItemDataBound"
            Skin="Office2010Silver">
        </telerik:RadTreeList>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="ralPanel_Tree" runat="server">
    </telerik:RadAjaxLoadingPanel>

    <div id="dvEmpty" runat="server" style="height: 100%; text-align: center; clear: both;">
        <div style="height: 40%"></div>
        <div style="height: 20%">
            <strong>Please select criteria for all required fields (indicated by <span style="color: red; font-weight: bold">*</span>)</strong>
            <br />
            <strong>then Update Results.</strong>
        </div>
        <div style="height: 40%"></div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>


<asp:Content ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/thinkgate.util.js") %>"></script>
    <script type="text/javascript">

        //This is temporary
        // IF IE load tempSetInitialComboBoxItems() here, else it will fire on DOMReady
        if (BrowserDetect.browser == "Explorer") {
            Sys.Application.add_load(Page_Initialize);
        }
        //

        var ddlStudentGrade, cblMonth, ddlSchool, ddlStundentName, ddlCounselor, ddlItemNumber;

        function Page_Initialize() {

            ddlStudentGrade = $find("<%= ddlStudentGrade.RadClientId %>");
            cblMonth = $find("<%= cblMonth.RadClientId %>");
            ddlSchool = $find("<%= ddlSchool.RadClientId %>");
            ddlStundentName = $find("<%= ddlStundentName.RadClientId %>");
            ddlCounselor = $find("<%= ddlCounselor.RadClientId %>");
            ddlItemNumber = $find("<%= ddlItemNumber.RadClientId %>");

            Validation();
        }

        function OnClientDropDownOpened(sender, args) {
            $(".rtWrapperContent").each(function () {
                $(this).off('mouseleave').on('mouseleave', function () {
                    $('#' + sender._animatedElement.id).closest('.rcbSlide').hide();
                })
                $('.rcbList,.rcbItem,.rcbHovered,.rcbSlide').off('mouseenter').on('mouseenter', function () {
                    $('#' + sender._animatedElement.id).closest('.rcbSlide').show();
                })               
            })
        }

        function Validation() {
            RestirctSelection(cblMonth, ddlStudentGrade.get_value() == '')
            RestirctSelection(ddlSchool, ddlStudentGrade.get_value() == '')
            RestirctSelection(ddlStundentName, ddlStudentGrade.get_value() == '' || ddlSchool.get_value() == '')
            RestirctSelection(ddlCounselor, ddlStudentGrade.get_value() == '' || ddlSchool.get_value() == '')
            RestirctSelection(ddlItemNumber, ddlStudentGrade.get_value() == '' || cblMonth.get_value() == '')
            try{
                CriteriaController.UpdateCriteriaForSearch();
            }catch(e){
                CriteriaController.UpdateValueDisplayArea(CriteriaController.CriteriaNodes[0]);
            }
                
                
        }

        var RestirctSelection = function (domElement, isRestrict) {
            var element = $('#' + domElement._element.id).closest('.jqRadToolTip').prevUntil('DIV').prev('DIV').find('.criteriaHeaderDiv');
            if (isRestrict)
                $(element).css("background", "#D2D2D2").find('IMG').hide();
            else
                $(element).css("background", "#F0F0F0").find('IMG').show();
        }

        function BindDataOnDemand(sender, eventArgs) {
            var context = eventArgs.get_context();
            context["Grade"] = ddlStudentGrade.get_value();
            context["School"] = ddlSchool.get_value();
            context["Months"] = cblMonth.get_value();
        }

        function rtsTabs_OnClientTabSelecting(sender, e) {
            CriteriaController.RemoveAllDependency('<%= ddlStudentGrade.CriteriaName %>');            
                ddlStudentGrade.clearSelection();
                ddlStudentGrade.clearItems();
                ddlStudentGrade_OnChange();
                cblMonth_OnChange()
            }

            function ddlStudentGrade_OnChange() {
                CriteriaController.RemoveAllDependency('<%= cblMonth.CriteriaName %>');
                        CriteriaController.RemoveAllDependency('<%= ddlSchool.CriteriaName %>');
                        cblMonth.clearSelection();
                        cblMonth.clearItems();
                        ddlSchool.clearSelection();
                        ddlSchool.clearItems();
                        cblMonth_OnChange();
                        ddlSchool_OnChange();
                    }

                    function RemoveBySelectedItems() {
                        if (ddlStudentGrade.get_value() == '')
                            ddlStudentGrade_OnChange();            
                        if (cblMonth.get_value() == '') 
                            cblMonth_OnChange();
                        if (ddlSchool.get_value() == '')
                            ddlSchool_OnChange();            
                    }

                    function cblMonth_OnChange() {
                        CriteriaController.RemoveAll('<%= ddlItemNumber.CriteriaName %>');
                    ddlItemNumber.clearSelection();
                    ddlItemNumber.clearItems();
                    Validation()
                }

                function ddlSchool_OnChange() {
                    CriteriaController.RemoveAllDependency('<%= ddlStundentName.CriteriaName %>');
            CriteriaController.RemoveAllDependency('<%= ddlCounselor.CriteriaName %>');
            ddlStundentName.clearSelection();
            ddlStundentName.clearItems();
            ddlCounselor.clearSelection();
            ddlCounselor.clearItems();
            Validation();
        }

        function OnRequestStartOfPanel(sender, args) {
            if (args.get_eventTarget().indexOf("imgExport") >= 0 || args.get_eventTarget().indexOf("rtsTabs") >= 0) {
                args.set_enableAjax(false);
            }
        }

        $(function () {
            //This is temporary
            // Non-IE fire here
            if (BrowserDetect.browser != "Explorer") {
                Page_Initialize();
            }
            //

        })
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=10" />
    <style type="text/css">
        #leftColumn, #ctl00_LeftColumnContentPlaceHolder_ctl00, .RadAjaxPanel {
            position: static !important;
        }
    </style>

</asp:Content>
