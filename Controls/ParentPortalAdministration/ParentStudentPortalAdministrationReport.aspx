<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Search.Master" Title="Parent Student Portal Administration" CodeBehind="ParentStudentPortalAdministrationReport.aspx.cs" Inherits="Thinkgate.Controls.ParentPortalAdministration.ParentStudentPortalAdministrationReport" %>

<%@ Register TagPrefix="telerik" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="telerik" TagName="RadDropDownList" Src="~/Controls/E3Criteria/RadDropDownList.ascx" %>

<%@ MasterType VirtualPath="~/Search.Master" %>

<asp:Content ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <telerik:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" />
</asp:Content>

<asp:Content ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <telerik:RadDropDownList ID="ddlSchoolType" EnableLoadOnDemand="True" OnChange="ddlSchoolType_OnChange()" IsNotAllowToEnterText="True" CriteriaName="SchoolType" runat="server" EmptyMessage="Select a School Type" Text="School Type" DataTextField="Text" DataValueField="Value" Required="True" OnItemsRequested="ddlSchoolType_ItemsRequested" />
    <telerik:RadDropDownList ID="ddlSchool" EnableLoadOnDemand="True" OnChange="ddlSchool_OnChange()" IsNotAllowToEnterText="True" OnClientItemsRequesting="BindDataOnDemand" CriteriaName="School" runat="server" EmptyMessage="Select a School" Text="School" DataTextField="SchoolName" DataValueField="ID" OnItemsRequested="ddlSchool_ItemsRequested" />
    <telerik:RadDropDownList ID="ddlStudentGrade" EnableLoadOnDemand="True" IsNotAllowToEnterText="True" OnClientItemsRequesting="BindDataOnDemand" CriteriaName="Grade" runat="server" EmptyMessage="Select a Grade" Text="Grade" DataTextField="Grade" DataValueField="Grade" OnItemsRequested="ddlStudentGrade_ItemsRequested" />
    <telerik:RadDropDownList ID="ddlStudentId" EnableVirtualScrolling="true" EnableLoadOnDemand="True" IsNotAllowToEnterText="False" CriteriaName="StudentId" runat="server" OnClientItemsRequesting="BindDataOnDemand" EmptyMessage="Select a Student ID" Text="Student ID" DataTextField="Student_Id" DataValueField="Student_Id" OnClientDropDownOpening="SetValue" OnItemsRequested="ddlStudentId_ItemsRequested" />
    <telerik:RadDropDownList ID="ddlStudentName" EnableVirtualScrolling="true" EnableLoadOnDemand="True" IsNotAllowToEnterText="False" CriteriaName="StudentName" runat="server" OnClientItemsRequesting="BindDataOnDemand" EmptyMessage="Select a Student" Text="Student Name" DataTextField="Student_Name" DataValueField="Student_Id" OnClientDropDownOpening="SetValue" OnItemsRequested="ddlStundentName_ItemsRequested" />
</asp:Content>

<asp:Content ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">

    <style type="text/css">
        .RadGrid_Web20 .rgCommandTable td {
            background-color: #7fa5d7;
            border: 0 none;
            padding: 0;
        }

        .RadGrid_Web20 thead .rgCommandCell {
            border-bottom: 0 none #7fa5d7;
            background-color: #7fa5d7;
        }

        .RadGrid .rgWrap {
            display: block;
            float: left;
            line-height: 22px;
            padding: 0 10px;
            white-space: nowrap;
        }

        .RadGrid_Web20 .rgHeader {
            color: #000;
            font-family: Calibri;
            font-size: 11pt;
        }

        div.RadGrid .rgAltRow {
            background: #E4EDFC;
        }
    </style>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
    </telerik:RadAjaxLoadingPanel>
    <div style="overflow-y: auto; height: 100%;">
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
            <div>
                <asp:Label runat="server" Text="Parent Student Portal Administration" Font-Size="18" Font-Names="Arial"></asp:Label>
                <asp:ImageButton ID="imgExport" Visible="false" runat="server" ClientIDMode="Static"
                    ImageUrl="~/Images/Toolbars/excel_button.png" text="Export to Excel" Enabled="True"
                    ToolTip="Export Searched Parent Student Portal information to Excel" OnClientClick="ConfirmMEssageonExcelImport(this);return false;"></asp:ImageButton>
            </div>

            <div>
                <asp:Label ID="lblNoResult" runat="server" Text="No results found." Font-Size="10" Font-Names="Arial" Visible="false"></asp:Label>
            </div>
            <div>
                <telerik:RadGrid runat="server" ID="radParentGrid" AutoGenerateColumns="False" Width="95%"
                    AllowFilteringByColumn="False" AllowMultiRowSelection="true" OnItemCommand="radParentGrid_ItemCommand" OnItemCreated="radParentGrid_ItemCreated"
                    CssClass="assessmentSearchHeader" Skin="Web20" Visible="false" AllowPaging="true" PageSize="10" PagerDropDownControlType="RadComboBox" OnPageIndexChanged="radParentGrid_PageIndexChanged"
                    OnItemDataBound="radParentGrid_ItemDataBound" OnPageSizeChanged="radParentGrid_OnPageSizeChanged" OnPreRender="radParentGrid_OnPreRender">
                    <MasterTableView>
                        <Columns>
                            <telerik:GridTemplateColumn HeaderText="Edit" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgEditParentAccessInformation" runat="server" Text="Edit" ImageUrl="~/Images/Toolbars/pencilicon.png" Height="25" Width="25"
                                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "StudentID") %>' CommandName="EditParentAccessInformation"
                                        OnClientClick="return showEditParentAccessInformation(this); " />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="StudentID" HeaderText="Student ID" HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblStudentID" runat="server" Text='<%# Bind("StudentID") %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="StudentName" HeaderText="Student Name" HeaderStyle-Width="18%" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-Font-Size="Small" />
                            <telerik:GridBoundColumn DataField="SchoolName" HeaderText="School" HeaderStyle-Width="18%" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-Font-Size="Small" />
                            <telerik:GridBoundColumn DataField="Grade" HeaderText="Grade" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Font-Size="Small" />
                            <telerik:GridBoundColumn DataField="StudentEmail" HeaderText="Student Email" HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left"
                                ItemStyle-Font-Size="Small" />
                            <telerik:GridBoundColumn DataField="ParentGuardianName" HeaderText="Parent/Guardian Name" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Font-Size="Small" />
                            <telerik:GridBoundColumn DataField="ParentGuardianIndicator" HeaderText="Parent/Guardian Indicator" HeaderStyle-Width="5%"
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Small" />
                            <telerik:GridTemplateColumn UniqueName="Select" DataField="ParentGuardianAccessEnabledValue" HeaderStyle-Width="12%" ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <asp:CheckBox ID="checkAll" runat="server" AutoPostBack="false" onclick="CheckAll(this)" Text="Enable Access for Parent/Guardian" TextAlign="Left" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="cboxSelect" runat="server" AutoPostBack="false" Checked='<%# Eval("ParentGuardianAccessEnabledValue") %>' onclick="unCheckHeader(this)" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="UserId" />
                        </Columns>
                        <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" AlwaysVisible="true" PageSizeControlType="RadComboBox" PagerTextFormat="{4} {5} items on {1} page(s) "></PagerStyle>
                    </MasterTableView>
                </telerik:RadGrid>

                <asp:HiddenField ID="hdnField" runat="server" />
                <asp:HiddenField ID="hdnchk" runat="server" />              
                
                <div id="divsave" runat="server" style="width: 95.2%; background-color: white" visible="false">
                    <table>
                        <tr>
                            <td style="width: 95%;"></td>
                            <td style="width: 5%;" align="right">
                                <telerik:RadButton ID="radSaveParentGrid" class="roundButtons" Font-Bold="true" Enabled="true" ClientIDMode="Static"
                                    Width="80px" runat="server" Text="Save"
                                    OnClientClicked="saveClientClick">
                                </telerik:RadButton>
                            </td>
                        </tr>
                    </table>
                </div>

                <div style="height: 40px"></div>
            </div>

            <div id="divChildGrid" runat="server" visible="false" style="padding-bottom: 20px">

                <div style="width: 95.2%; background-color: #7fa5d7">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width: 30%;"></td>
                            <td align="left" style="width: 65%;">
                                <asp:Label runat="server" ID="lblStudentID" Font-Bold="true" ForeColor="White"></asp:Label></td>
                            <td align="right" style="padding-right: 5px">
                                <asp:ImageButton ID="imgCloseChildGrid" runat="server"
                                    OnClick="imgCloseChildGrid_Click" OnClientClick="return confirm('Would you like to discard your changes and return to the interface ?\n Select OK to proceed or Cancel to resume the in progress update.');"
                                    Style="background-image: url('../../Images/radWindowImages.png'); display: table; width: 14px; height: 15px; background-position: -93px -3px;"
                                    src="" value="" /></td>
                        </tr>
                    </table>
                </div>

                <telerik:RadGrid runat="server"
                    ID="radParentStudentAccess"
                    AutoGenerateColumns="False"
                    Width="95%"
                    AllowFilteringByColumn="False"
                    AllowAutomaticUpdates="false"
                    AllowAutomaticInserts="false"
                    AllowMultiRowEdit="false"
                    Selecting-AllowRowSelect="true"
                    OnItemCreated="radParentStudentAccess_ItemCreated"
                    OnItemDataBound="radParentStudentAccess_ItemDataBound"
                    OnItemCommand="radParentStudentAccess_ItemCommand"
                    OnPreRender="radParentStudentAccess_PreRender"
                    OnUpdateCommand="radParentStudentAccess_UpdateCommand"
                    OnNeedDataSource="radParentStudentAccess_NeedDataSource"
                    CssClass="assessmentSearchHeader" Skin="Web20" Visible="false">
                    <MasterTableView DataKeyNames="StudentID,UserId,ParentFirstName,ParentLastName,Email,ParentGuardianIndicator" DataMember="UserId" TableLayout="Auto" InsertItemDisplay="Bottom" EditMode="InPlace"
                        CommandItemDisplay="Top">
                        <CommandItemTemplate>
                            <asp:ImageButton ID="imgAddNewRecord" CommandName="InitInsert" AlternateText="Add New" runat="server" ImageUrl="~/Images/Add.gif" Height="14px" Width="14px" />
                            <asp:Label ID="lblAddNew" runat="server" Text="Add New"></asp:Label>
                            <asp:LinkButton ID="lnkSave" runat="server" CommandName="PerformInsert" Visible="false"></asp:LinkButton>
                        </CommandItemTemplate>
                        <Columns>
                            <telerik:GridEditCommandColumn UniqueName="EditCommandColumn" ButtonType="ImageButton" HeaderText="Edit" HeaderStyle-Width="7%" ItemStyle-VerticalAlign="Top"></telerik:GridEditCommandColumn>
                            <telerik:GridBoundColumn UniqueName="StudentID" ItemStyle-VerticalAlign="Top" DataField="StudentID" />
                            <telerik:GridBoundColumn UniqueName="UserId" ItemStyle-VerticalAlign="Top" DataField="UserId" />
                            <telerik:GridBoundColumn UniqueName="ParentFirstName" ItemStyle-VerticalAlign="Top" DataField="ParentFirstName" HeaderText="Parent/Guardian First Name" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-Font-Size="Small" HeaderStyle-Width="23%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn UniqueName="ParentLastName" ItemStyle-VerticalAlign="Top" DataField="ParentLastName" HeaderText="Parent/Guardian Last Name" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-Font-Size="Small" HeaderStyle-Width="22.5%">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn UniqueName="Email" ItemStyle-VerticalAlign="Top" DataField="Email" HeaderText="Parent/Guardian Email" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-Font-Size="Small" HeaderStyle-Width="22.5%">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Parent/Guardian Indicator" ItemStyle-VerticalAlign="Top" UniqueName="ParentGuardianIndicator" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblParentGuardianIndicator"><%# DataBinder.Eval(Container.DataItem, "ParentGuardianIndicator")%></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadComboBox ID="ParentGuardianIndicator" runat="server" Width="40px">
                                        <Items>
                                            <telerik:RadComboBoxItem runat="server" Text="P" Value="P" />
                                            <telerik:RadComboBoxItem runat="server" Text="G" Value="G" />
                                        </Items>
                                    </telerik:RadComboBox>
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridCheckBoxColumn UniqueName="GridCheckBoxAccessEnabled" ItemStyle-VerticalAlign="Top" DataField="ParentGuardianAccessEnabledValue" HeaderText="Access Enabled" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                            </telerik:GridCheckBoxColumn>
                            <telerik:GridTemplateColumn HeaderText="Delete" UniqueName="DeleteButton" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgDelete" runat="server" title="Delete" ImageUrl="~/Images/remove.png" CommandName="MessageDelete" CommandArgument='<%#Eval("StudentID") %>' OnClientClick="return confirm('Are you sure you want to delete this record?');" Height="18px" Width="18px" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>

            </div>


            <div id="divStudentGrid" runat="server" visible="false" style="padding-bottom: 20px">
                <div style="width: 95.2%; background-color: #7fa5d7">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width: 30%;"></td>
                            <td align="left" style="width: 65%;">
                                <asp:Label runat="server" ID="lblStudentInformation" Font-Bold="true" ForeColor="White"></asp:Label></td>
                            <td align="right" style="padding-right: 5px">
                                <asp:ImageButton ID="imgCloseStudentGrid" runat="server"
                                    OnClick="imgCloseStudentGrid_Click"
                                    Style="background-image: url('../../Images/radWindowImages.png'); display: table; width: 14px; height: 15px; background-position: -93px -3px;"
                                    src="" value="" /></td>
                        </tr>
                    </table>
                </div>

                <telerik:RadGrid runat="server"
                    ID="radStudentInformation"
                    AutoGenerateColumns="False"
                    Width="95%"
                    AllowFilteringByColumn="False"
                    AllowAutomaticUpdates="false"
                    AllowAutomaticInserts="false"
                    AllowMultiRowEdit="false"
                    Selecting-AllowRowSelect="true"
                    CssClass="assessmentSearchHeader" Skin="Web20" Visible="false"
                    OnItemCreated="radStudentInformation_ItemCreated"
                    OnUpdateCommand="radStudentInformation_UpdateCommand"
                    OnNeedDataSource="radStudentInformation_NeedDataSource"
                    OnPreRender="radStudentInformation_OnPreRender">
                    <MasterTableView DataKeyNames="StudentID, UserId" DataMember="StudentID" TableLayout="Auto" InsertItemDisplay="Bottom" EditMode="InPlace">
                        <Columns>
                            <telerik:GridEditCommandColumn UniqueName="EditCommandColumn" ButtonType="ImageButton" HeaderText="Edit" HeaderStyle-Width="7%" ItemStyle-VerticalAlign="Top"></telerik:GridEditCommandColumn>
                            <telerik:GridBoundColumn UniqueName="StudentID" DataField="StudentID" />
                            <telerik:GridTemplateColumn UniqueName="StudentName" HeaderText="Student Name" HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblStudentName" runat="server" Text='<%# Bind("StudentName") %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn UniqueName="StudentEmail" ItemStyle-VerticalAlign="Top" DataField="StudentEmail" HeaderText="Email" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-Font-Size="Small" HeaderStyle-Width="22.5%">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" EnableModelErrorMessageValidation="true">
                                    <RequiredFieldValidator ForeColor="Red" ErrorMessage="*"></RequiredFieldValidator>
                                    <ModelErrorMessage BackColor="Red" />
                                </ColumnValidationSettings>
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn UniqueName="SchoolName" HeaderText="School Name" HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblSchoolName" runat="server" Text='<%# Bind("SchoolName") %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="Grade" HeaderText="Grade" HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblGrade" runat="server" Text='<%# Bind("Grade") %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>

            </div>
        </telerik:RadAjaxPanel>

        <div id="dvEmpty" runat="server" style="height: 100%; text-align: center; clear: both;">
            <div style="height: 40%"></div>
            <div style="height: 20%">
                <strong>Please select criteria for all required fields (indicated by <span style="color: red; font-weight: bold">*</span>)</strong>
                <br />
                <strong>then select Search.</strong>
            </div>
            <div style="height: 40%"></div>
        </div>
        <input type="hidden" id="hdnCheckValueflag" value="0" runat="server" />
    </div>
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

        function saveClientClick(sender, args) {
            isCheckboxActive = false;
            $('#<%= hdnCheckValueflag.ClientID %>').val(0);
            customDialog({ title: "Save data", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "Completing this selection will override any previously completed <br/>access related changes for the displayed students. Select OK to <br/>accept the changes or Cancel to return to the search results.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel" }, { title: "Ok", callback: saveClientClickcallback }]);
        }

        function saveClientClickcallback() {
            __doPostBack('radSaveParentGrid', '');
        }

        function ConfirmMEssageonExcelImport(obj) {
            if ($('#<%= hdnCheckValueflag.ClientID %>').val() == "1") {
                customDialog({ title: 'Alert', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: "Save must be selected to retain any Active Access for Parent/Guardian changes.<br/><br/>" }, [{ title: 'OK' }]);;
            }
            else {
                customDialog({ title: "Confirm Excel Import", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "Proceeding with this action will result in all Parent Student data <br/> being exported.The output will not be limited to the search results.<br/> Select OK to proceed or Cancel to return to the search results display.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel" }, { title: "Ok", callback: imgExportCallBack }]);
            }
        }

        function imgExportCallBack() {
            __doPostBack('imgExport', '');
        }

        function DeleteParentInfoAlert() {
               customDialog({ title: 'Alert', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: "Are you sure you want to delete this record?<br/><br/>" }, [{ title: 'OK' }]);;
        }

        function CheckDuplicateEmailAlert(firstName, lastName, email) {            
            customDialog({ title: "Confirm Save Data", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "The email entered matches an existing entry. Is this the Parent / <br/> Guardian you are attempting to add: <br/><br/> (" + firstName + " and " + lastName + " and " + email + ") <br/><br/>Select OK to add the displayed record or Cancel to the Add new <br/>view.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel" }, { title: "Ok", callback: checkDuplicateEmailClickcallback }]);
        }

        function checkDuplicateEmailClickcallback() {
            __doPostBack('btnCheckDuplicateEmail', '');
        }

        function showEditParentAccessInformation(obj) {

            if (MandatorytoSaveAfterChangeInCheckEvent() == true)
                return true;
            else
                return false;           
        }

        function ValidateDuplicateEmail() {
            if (!confirm("The email entered matches an existing entry. Is this the Parent Guardian you are attempting to?")) {
                return true;
            }
            else
                return false;
        }

        function ValidateDeleteParentAlert() {
            customDialog({ title: 'Alert', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: "The email entered matches an existing entry.<br/><br/>" }, [{ title: 'OK' }]);;
        }

        function EmailAlreadyExistAlert() {
            customDialog({ title: 'Alert', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: "The email entered matches an existing entry.<br/><br/>" }, [{ title: 'OK' }]);;
        }

        function ValidationMessageforTextControls() {
            customDialog({ title: 'Alert', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: "Please confirm that the first Name, Last Name, Email and Parent/<br/>Guardian indicator fields are all completed for the defined users.<br/><br/>" }, [{ title: 'OK' }]);;
        }

        function MandatorytoSaveAfterChangeInCheckEvent() {
          
            if (isCheckboxActive) {
                customDialog({ title: 'Alert', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: "Save must be selected to retain any Active Access for Parent/Guardian changes.<br/><br/>" }, [{ title: 'OK' }]);;

                isCheckboxActive = false;
                $('#<%= hdnCheckValueflag.ClientID %>').val(0);
                return false;
            }
            else
                return true;
        }

        var ddlSchoolType, ddlSchool, ddlStudentGrade, ddlStudentId, ddlStudentName;

        function Page_Initialize() {
            
            ddlSchoolType = $find("<%= ddlSchoolType.RadClientId %>");
            ddlSchool = $find("<%= ddlSchool.RadClientId %>");
            ddlStudentId = $find("<%= ddlStudentId.RadClientId %>");
            ddlStudentName = $find("<%= ddlStudentName.RadClientId %>");
            ddlStudentGrade = $find("<%= ddlStudentGrade.RadClientId %>");

        }

        function BindDataOnDemand(sender, eventArgs) {
            var context = eventArgs.get_context();

            context["SchoolType"] = ddlSchoolType.get_value();
            context["School"] = ddlSchool.get_value();
            context["StudentId"] = ddlStudentId.get_value();
            context["StudentName"] = ddlStudentName.get_value();
            context["Grade"] = ddlStudentGrade.get_value();

        }

        function OnClientDropDownOpened(sender, args) {

            $(".rtWrapperContent").each(function () {
                $(this).off('mouseleave').on('mouseleave', function () {
                    $('#' + sender._animatedElement.id).closest('.rcbSlide').hide();

                });
                $('.rcbList,.rcbItem,.rcbHovered,.rcbSlide').off('mouseenter').on('mouseenter', function () {
                    $('#' + sender._animatedElement.id).closest('.rcbSlide').show();
                });
            });
        }

        function CheckAll(id) {

            var masterTable = $find("<%= radParentGrid.ClientID %>").get_masterTableView();
            var row = masterTable.get_dataItems();
            isCheckboxActive = true;

            $('#<%= hdnCheckValueflag.ClientID %>').val(1);


            $('#<%= hdnField.ClientID %>').val(id.checked); // Select All

            $('#<%= hdnchk.ClientID %>').val(id.checked); // Select Check Box

            if (id.checked == true) {

                var msg1 = "Are you certain you would like to activate parent portal access for all users on all pages of the search results?";
                var msg2 = "Select OK to proceed or Cancel to decline this selection.";

                //customDialog({ title: "Confirm Enable disable portal access ", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "Are you certain you would like to activate parent portal access for all <br/> users on all pages of the search results? Select OK to proceed or Cancel to decline this selection.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel" }, { title: "Ok", callback: checkAllCallBack }]);
                var response = confirm(msg1 + "\n" + msg2);
                if (response) {
                    for (var i = 0; i < row.length; i++) {
                        if (masterTable.get_dataItems()) {
                            var checkSelect = masterTable.get_dataItems()[i].findElement("cboxSelect");
                            if (checkSelect != null) {
                                masterTable.get_dataItems()[i].findElement("cboxSelect").checked = true;
                            }
                        }
                    }
                }
                else {
                    var chkBox = $('input[id$="checkAll"]');
                    chkBox[0].checked = false;
                }
            }
            else {

                var msgd1 = "Are you certain you would like to deactivate parent portal access for all users on all pages of the search results?";
                var msgd2 = "Select OK to proceed or Cancel to decline this selection.";

                var response = confirm(msgd1 + "\n" + msgd2);
                //customDialog({ title: "Confirm Enable disable portal access ", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "Are you certain you would like to activate parent portal access for all <br/> users on all pages of the search results? Select OK to proceed or Cancel to decline this selection.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel" }, { title: "Ok", callback: checkAllCallBack }]);

                if (response) {
                    for (var i = 0; i < row.length; i++) {

                        if (masterTable.get_dataItems()) {
                            var uncheckSelect = masterTable.get_dataItems()[i].findElement("cboxSelect");
                            if (uncheckSelect != null) {
                                masterTable.get_dataItems()[i].findElement("cboxSelect").checked = false;
                            }
                        }
                    }
                }
                else {
                    var chkBox = $('input[id$="checkAll"]');
                    chkBox[0].checked = true;
                }
            }
        }

       <%-- function previousStateCallBack() {
            var chkBox = $('input[id$="checkAll"]');
            chkBox[0].checked = false;
        }

        function checkAllCallBack() {
            var masterTable = $find("<%= radParentGrid.ClientID %>").get_masterTableView();
            var row = masterTable.get_dataItems();

            for (var i = 0; i < row.length; i++) {
                if (masterTable.get_dataItems()) {
                    var checkSelect = masterTable.get_dataItems()[i].findElement("cboxSelect");
                    if (checkSelect != null) {
                        masterTable.get_dataItems()[i].findElement("cboxSelect").checked = true;
                    }
                }
            }
        }--%>

        var isCheckboxActive = false;

        function unCheckHeader(id) {
            isCheckboxActive = true;
            $('#<%= hdnCheckValueflag.ClientID %>').val(1);
            $('#<%= hdnchk.ClientID %>').val("");

            var masterTable = $find("<%= radParentGrid.ClientID %>").get_masterTableView();

            var chkBox = $('input[id$="checkAll"]');
            if (id.checked == false) {
                chkBox[0].checked = false;
            }
            else {
                var masterTable = $find("<%= radParentGrid.ClientID %>").get_masterTableView();
                var row = masterTable.get_dataItems();
                var Checkflag = true;
                for (var i = 0; i < row.length; i++) {

                    if (masterTable.get_dataItems()) {
                        var uncheckSelect = masterTable.get_dataItems()[i].findElement("cboxSelect");
                        if (uncheckSelect != null) {
                            if (uncheckSelect.checked == false)
                                Checkflag = false;
                        }
                    }
                }
                if (Checkflag) {
                    chkBox[0].checked = true;
                }
            }

        }

        function ddlSchoolType_OnChange() {
            ddlSchool_OnChange();
            ddlSchool.clearSelection();
            ddlSchool.clearItems();
            MandatorytoSaveAfterChangeInCheckEvent();
        }

        function ddlSchool_OnChange() {
            ddlStudentId.clearSelection();
            ddlStudentId.clearItems();
            ddlStudentName.clearSelection();
            ddlStudentName.clearItems();
            ddlStudentGrade.clearSelection();
            ddlStudentGrade.clearItems();
            MandatorytoSaveAfterChangeInCheckEvent();
        }


        function onChildGrid(sender, args) {

            var button = sender;
            if (button) {

                button.set_enabled(false);
            }
            return true;
        }

        $(function () {
            //This is temporary
            // Non-IE fire here
            if (BrowserDetect.browser != "Explorer") {
                Page_Initialize();
            }
            //

        })

        function SetValue(sender, args) {
            sender.clearItems();
        }

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
