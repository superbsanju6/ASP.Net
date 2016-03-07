<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Assignment.aspx.cs" Inherits="Thinkgate.Controls.AssignmentShare.Assignment" %>
<%@ Register TagPrefix="e3" TagName="AssignStudentsToAssessment" Src="~/Controls/AssignmentShare/AssignStudentsToAssessment.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
        <StyleSheets>
            <telerik:StyleSheetReference Path="~/Styles/reset.css" />
            <telerik:StyleSheetReference Path="~/Styles/Site.css" />
            <telerik:StyleSheetReference Path="~/Styles/SearchExpanded.css" />
        </StyleSheets>
    </telerik:RadStyleSheetManager>
    <link href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet" type="text/css" runat="server" />
    <link rel="stylesheet" type="text/css" href="../../Styles/AssignmentsSharing.css" />
    <script type="text/javascript">
        function ConfirmRemoveAll(button, args) {
            if (window.confirm("You have selected all records in the list.  Are you sure you want to continue?")) {
                button.set_autoPostBack(true);
            }
            else {
                button.set_autoPostBack(false);
            }
        }

        // For closing Window.
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        // For closing Window.
        function CloseDialog(arg) {
            var oWindow = GetRadWindow();
            setTimeout(function () {
                oWindow.Close(arg);
            }, 0);
        }

        /***********************************************************************************************************************
        *   Criteria related functions (taken from SearchExpanded.Master)
        ***********************************************************************************************************************/

        function toggleSidebar() {
            $('#leftColumn').toggle('slide', { direction: 'left' }, 500);

            var leftColumnWidth = convertToInt($('#leftColumn').css('width'));
            var rightColumnMarginLeft = convertToInt($('#rightColumn').css('margin-left'));
            var columnExpanderWidth = convertToInt($('#columnExpander').css('width'));
            var columnExpanderMarginLeft = convertToInt($('#columnExpander').css('margin-left'));

            if (rightColumnMarginLeft == columnExpanderWidth) {
                rightColumnMarginLeft = -rightColumnMarginLeft;
            }

            var newRightColumnMarginLeft = Math.abs(rightColumnMarginLeft - leftColumnWidth);
            var newColumnExpanderMarginLeft = Math.abs(columnExpanderMarginLeft - leftColumnWidth);

            $('#rightColumn').animate({ 'margin-left': newRightColumnMarginLeft }, 500);
            $('#columnExpander').animate({ 'margin-left': newColumnExpanderMarginLeft }, 500, function () {
                if (newColumnExpanderMarginLeft == 0) {
                    //closed
                    $('#columnExpanderHandleImage').attr('src', '../../Images/arrow_gray_right.gif');
                    if (typeof alterPageComponents == 'function')
                        alterPageComponents('closed');
                }
                else {
                    //open
                    $('#columnExpanderHandleImage').attr('src', '../../Images/arrow_gray_left.gif');

                    if (typeof alterPageComponents == 'function')
                        alterPageComponents('open');
                }
            });
        }

        function convertToInt(value) {
            return value.replace(/[^-\d\.]/g, '');
        }

        function buildDependencyObject(dependencies) {
            var data = {
                "container": {}
            };

            var dependencyObject = jQuery.parseJSON(dependencies);

            for (var dependency in dependencyObject) {
                var key = dependencyObject[dependency].key;
                var currentKey = dependencyObject[dependency].value;

                data.container[currentKey + "Key"] = key;
                data.container[currentKey] = [];

                var genericDiv = $("div[id*='" + getControlType(dependencyObject[dependency].type) + key + "']:not(div[id$='_DropDown'])");

                if (genericDiv == null) {
                    alert("Couldn't find: " + dependencyObject[dependency].type);
                    continue;
                }

                var genericControl = $find(genericDiv.attr('id'));
                if (genericControl == null) {
                    continue;
                }

                var controlItems = genericControl.get_items()._array;
                var jsonItem = {};
                for (var i = 0; i < controlItems.length; ++i) {
                    var selected = false;

                    switch (dependencyObject[dependency].type) {
                        case "CheckBoxList":
                            selected = controlItems[i].get_checked();
                            jsonItem = {
                                "DisplayText": controlItems[i].get_text(),
                                "Value": controlItems[i].get_value(),
                                "Selected": selected
                            };
                            break;

                        case "DropDownList":
                            selected = controlItems[i].get_selected();
                            jsonItem = {
                                "ID": controlItems[i].get_value().replace(key + "_", ''),
                                "DisplayText": controlItems[i].get_text(),
                                "Value": controlItems[i].get_value(),
                                "Selected": selected
                            };
                            break;
                    }

                    if (!selected) {
                        continue;
                    }

                    data.container[currentKey].push(jsonItem);
                }
            }
            return data;
        }

        function getControlType(type) {
            switch (type) {
                case "CheckBoxList":
                    return "RadCombobBoxCriteriaCheckBoxList";

                case "DropDownList":
                    return "RadCombobBoxCriteriaDropDownList";

                default:
                    return "";
            }
        }

        var serviceControlsList = {};
        var loadingInterval = "";

        function checkServiceControlsFullyLoaded() {
            for (var control in serviceControlsList) {
                if (!serviceControlsList[control].loaded) {
                    return;
                }
            }

            for (var control in serviceControlsList) {
                serviceControlsList[control].callback();
            }

            if (loadingInterval != "") {
                window.clearInterval(loadingInterval);
            }
        }

        function addServiceControl(key) {
            var serviceControl = {
                loaded: false
            };

            serviceControlsList[key] = serviceControl;
        }

        function loadServiceData(key, type) {
            var partialID = getControlType(type) + key;
            var genericDiv = $("div[id$='" + partialID + "']:not(div[id$='_DropDown'])");

            var genericControl = $find(genericDiv.attr('id'));
            if (genericControl == null) {

                //alert("loadServiceData: " + genericDiv.attr('id'));
                return;
            }

            var serviceUrl = genericDiv.attr('serviceurl');
            var serviceSuccessfulCallback = genericDiv.attr('successcallback');
            var dependencies = genericDiv.attr('dependencies');

            if (serviceUrl != null) {
                ajaxWCFService({ url: serviceUrl, data: JSON.stringify(buildDependencyObject(dependencies)), success: serviceSuccessfulCallback });
            }
        }

        function updateCriteriaControl(key, data, type, headerText) {
            var partialID = getControlType(type) + key;
            var genericDiv = $("div[id$='" + partialID + "']");
            var genericControl = $find(genericDiv.attr('id'));
            if (genericControl == null) {
                alert(genericDiv.attr('id'));
                return;
            }

            var genericControlItems = genericControl.get_items();
            genericControlItems.clear();

            switch (type) {
                case "DropDownList":
                    var blankItem = new Telerik.Web.UI.RadComboBoxItem();
                    blankItem.set_text("Select a " + headerText);
                    blankItem.set_value('0');
                    genericControlItems.add(blankItem);
                    break;
            }

            for (var contract in data) {
                var item = null;

                switch (type) {
                    case "CheckBoxList":
                        item = new Telerik.Web.UI.RadListBoxItem();
                        item.set_checked(data[contract].Selected);
                        break;

                    case "DropDownList":
                        item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_selected(data[contract].Selected);
                        break;
                }

                if (data[contract].Value) {
                    item.set_text(data[contract].DisplayText);
                    item.set_value(data[contract].Value);

                    genericControlItems.add(item);
                }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-migrate-1.1.0.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery.scrollTo-min.js" />
                <asp:ScriptReference Path="~/Scripts/expandedSearchPage.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
                <asp:ScriptReference Path="~/Scripts/jsrender.js" />
                <asp:ScriptReference Path="~/Scripts/jquery.jfade.1.0.min.js" />
                <asp:ScriptReference Path="~/Scripts/searchExpandedCustomPager.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="False" Modal="True" InitialBehaviors="Close"
            Animation="None">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
        </telerik:RadSkinManager>


        <%-- Main Content --%>

        <div id="topDiv">
            <span class="bold">
                <asp:Label ID="lblContentType" runat="server" />:&nbsp;</span>
            <asp:Label ID="lblAssignmentName" runat="server" />
        </div>
        <div id="topLeftDiv" class="LeftDiv">
            Available
        </div>
        <div id="topRightDiv" class="RightDiv">
            Selected
        </div>
        <telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel"
        Style="height: 100%;">


        <div id="RightOuterDiv" class="RightDiv">
            <div id="RightTabDiv">
            </div>
            <telerik:RadGrid ID="gridSelected" runat="server"
                AutoGenerateColumns="false"
                OnItemCreated="gridSelected_ItemCreated"
                OnNeedDataSource="gridSelected_NeedDataSource"
                OnUpdateCommand="gridSelected_UpdateCommand"
                AllowPaging="true"
                        PageSize="10" OnItemDataBound="gridSelected_ItemDataBound">
                <PagerStyle ShowPagerText="false" />
                <MasterTableView DataKeyNames="ID">
                    <Columns>
                        <telerik:GridTemplateColumn>
                            <HeaderTemplate>
                                <telerik:RadButton ID="btnRemoveAll" runat="server" Text="Remove All" OnClientClicked="ConfirmRemoveAll" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <telerik:RadButton ID="btnRemove" runat="server" Text="Remove" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn Visible="false" HeaderText="ID" DataField="ID" ReadOnly="true" />
                            <telerik:GridBoundColumn Visible="true" HeaderText="TypeID" DataField="TypeID" ReadOnly="true" />
                            <telerik:GridBoundColumn Visible="true" HeaderText="InfoID" DataField="InfoID" ReadOnly="true" />
                        <telerik:GridBoundColumn HeaderText="Type" DataField="Type" ReadOnly="true" />
                        <telerik:GridHyperLinkColumn HeaderText="Name" DataTextField="Name" />
                        <telerik:GridBoundColumn HeaderText="Date Assigned" DataField="DateAssigned" DataFormatString="{0:d}" ReadOnly="true" />
                        <telerik:GridDateTimeColumn HeaderText="Due Date" DataField="DueDate" DataFormatString="{0:d}" />
                        <telerik:GridEditCommandColumn ButtonType="ImageButton" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>

        <div id="LeftOuterDiv" class="LeftDiv">
            <%--Tab Strip--%>
            <div id="LeftTabDiv">
                <telerik:RadTabStrip runat="server" ID="tabAvailable" SelectedIndex="1" MultiPageID="multpageAvailable">
                    <Tabs>
                        <telerik:RadTab Text="Group" />
                        <telerik:RadTab Text="Student" />
                    </Tabs>
                </telerik:RadTabStrip>
            </div>
            <%--Multi Page--%>
            <telerik:RadMultiPage ID="multpageAvailable" runat="server">

                <%--Group page--%>
                <telerik:RadPageView ID="pageGroup" runat="server">
                </telerik:RadPageView>

                <%--Student Page--%>
                <telerik:RadPageView ID="pageStudent" runat="server" Selected="true">

                        <e3:AssignStudentsToAssessment runat="server" id="ucAssignStudentsToAssessment" ></e3:AssignStudentsToAssessment>

                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </div>

        <div>
            <table width="99.5%">
                <tr>
                    <td>
                        <asp:Label ID="lblSearchResultCount" CssClass="searchCarouselLabel" runat="server"
                            Text="" />
                    </td>
                </tr>
            </table>
            <asp:Panel runat="server" ID="gridResultsPanel">
                <telerik:RadGrid    runat="server" 
                                    ID="radGridResults" 
                                    AutoGenerateColumns="False" 
                                    Width="99.5%"
                                    AllowFilteringByColumn="False" 
                                    PageSize="20" AllowPaging="True" 
                                    AllowSorting="True"
                                    OnPageIndexChanged="RadGridResults_PageIndexChanged" 
                                    Skin="Web20" CssClass="assessmentSearchHeader"
                                    OnItemDataBound="radGridResults_ItemDataBound" 
                                    Height="600px" OnSortCommand="OnSortCommand">
                    <PagerStyle Mode="NumericPages" />
                    <MasterTableView TableLayout="Auto" Height="32px" AllowSorting="true">
                        <Columns>
                            <telerik:GridTemplateColumn>
                                <HeaderTemplate>
                                    <telerik:RadButton ID="btnAssignAll" runat="server" Text="Assign" OnClientClicked="ConfirmAssignAll" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <telerik:RadButton ID="btnAssign" runat="server" Text="Assign" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>                            
                            <telerik:GridHyperLinkColumn DataTextField="Name" UniqueName="ViewHyperLinkColumn" HeaderText="Name" Target="_blank" SortExpression="Name" />
                            <telerik:GridBoundColumn DataField="Type" HeaderText="Type" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </asp:Panel>
        </div>

        <div id="Bottom">
            <telerik:RadButton runat="server" ID="btnClose"
                AutoPostBack="False"
                OnClientClicked="CloseDialog"
                Text="Close"
                UseSubmitBehavior="False" />
        </div>
        <%-- End Main Content --%>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    <input clientidmode="Static" type="hidden" runat="server" id="ReturnToPostBackPage" />
    <input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv1_currentPage"
        value="1" />
    <input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv2_currentPage"
        value="1" />
    <input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv1_postBackPage" />
    <input clientidmode="Static" type="hidden" runat="server" id="tileScrollDiv2_postBackPage" />
    </form>
</body>
</html>
