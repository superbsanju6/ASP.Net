<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GenericCMSDocumentTile.ascx.cs"
    Inherits="Thinkgate.Controls.Documents.GenericCMSDocumentTile" %>

<%@ Import Namespace="Thinkgate.Classes" %>

<style>
    .rbPrimaryIcon {
        top: 5px !important;
    }

    .dropdown {
        position: relative;
        float: left;
        left: 50px;
    }

    .labels {
        width: 50px;
        position: relative;
        float: left;
        left: 15px;
    }
</style>

<telerik:radajaxloadingpanel clientidmode="Static" id="documentTypeListLoadingPanel" runat="server" />

<telerik:radwindowmanager id="wndWindowManager" runat="server">
    <Windows>
        <telerik:RadWindow runat="server" ID="wndAddDocument"
            Title=""
            ShowContentDuringLoad="False"
            Behavior="None"
            ReloadOnShow="True"
            Modal="True"
            Skin="Web20"
            VisibleStatusbar="False"
            AutoSize="True"
            AutoSizeBehaviors="Default"
            DestroyOnClose="true">
            <ContentTemplate>
                <%-- TYPE --%>
                <div runat="server" id="divTypeSubtype" visible="True" style="margin-left: auto; margin-right: auto; width: 500px; height: 225px; font-size: 12pt; display: none">
                    <asp:HiddenField runat="server" ID="hdnSubTypes" ClientIDMode="Static" />
                    <asp:HiddenField runat="server" ID="hdnFormTypes" ClientIDMode="Static" />

                    <label>Please select the resource type you would like to add</label>
                    <br />
                    <br />
                    <span class="labels">Type:     
                    </span>
                    <asp:DropDownList ID="ddlType" runat="server" Width="200" CssClass="dropdown" ToolTip="Select a Type" onchange="populateSubType(this);"></asp:DropDownList>
                    <br />
                    <br />
                    <span class="labels">Subtype:
                    </span>
                    <asp:DropDownList ID="ddlSubType" runat="server" Width="200" CssClass="dropdown" ToolTip="Select a SubType" onchange="populateFormType(this);"></asp:DropDownList>
                    <br />
                    <br />
                    <div id="divFormType" style="display: none">
                        <span class="labels">FormType:
                        </span>
                        <asp:DropDownList ID="ddlFormType" runat="server" Width="200" CssClass="dropdown" ToolTip="Select a FormType"></asp:DropDownList>
                    </div>
                    <br />
                    <br />
                    <div style="float: right; margin-right: 20px;">
                        <%--Type Buttons--%>
                        <telerik:RadButton runat="server" ID="btnOkType"
                            Text="OK"
                            Skin="Web20"
                            AutoPostBack="false" />
                        <telerik:RadButton runat="server" ID="btnCancelType"
                            Text="Cancel"
                            Skin="Web20"
                            AutoPostBack="false" />

                        <div id="div" style="display: none">
                            <telerik:RadButton runat="server" ID="hdnBtnOkType"
                                Text="Random"
                                Skin="Web20"
                                AutoPostBack="true" OnClick="BtnOkTypeOnClick" />
                        </div>

                    </div>

                </div>
                <%-- WHERE --%>
                <div runat="server" id="divAddWhere"
                    visible="True"
                    style="margin-left: auto; margin-right: auto; width: 500px; height: 225px; font-size: 12pt;">
                    <label>Where do you want to Create the New Document?</label>                                       
                    <br />
                    <asp:Literal ID="litState" Text="<br/>" runat="server"></asp:Literal>
                    <telerik:RadButton runat="server" ID="rdoState"
                        name="where"
                        Skin="Web20"
                        CommandArgument="0"
                        Font-Size="12pt"
                        Text="State"                      
                        ToggleType="Radio"
                        ButtonType="ToggleButton"
                        Checked="False"
                        GroupName="CreateWhere"
                        AutoPostBack="False" />
                    <br />
                    <telerik:RadButton runat="server" ID="rdoDistrict"
                        name="where"
                        Skin="Web20"
                        CommandArgument="1"                          
                        Font-Size="12pt"
                        Text="District"
                        ToggleType="Radio"
                        ButtonType="ToggleButton"
                        Checked="False"
                        GroupName="CreateWhere"
                        AutoPostBack="False" />
                    <%--<br />--%>
                    <asp:Literal ID="litDistrict" Text="<br/>" runat="server"></asp:Literal>
                    <telerik:RadButton runat="server" ID="rdoMyDocuments"
                        name="where"
                        Skin="Web20"
                        CommandArgument="2"                        
                        Font-Size="12pt"
                        Text="My Documents"
                        ToggleType="Radio"
                        ButtonType="ToggleButton"
                        Checked="True"
                        GroupName="CreateWhere"
                        AutoPostBack="False" />
                    <br />
                    <telerik:RadButton runat="server" ID="rdoShared"
                        name="where"
                        Skin="Web20"
                        CommandArgument="2"                          
                        Font-Size="12pt"
                        Text="Shared"
                        ToggleType="Radio"
                        ButtonType="ToggleButton"
                        Checked="False"
                        GroupName="CreateWhere"
                        AutoPostBack="False" />
                    <br />
                    <br />
                    <div style="float: right; margin-right: 20px;">
                        <%--Where Buttons--%>
                        <telerik:RadButton runat="server" ID="btnOkWhere"
                            Text="OK"
                            Skin="Web20"
                            AutoPostBack="False" />
                        <telerik:RadButton runat="server" ID="btnCancelWhere"
                            Text="Cancel"
                            Skin="Web20"
                            AutoPostBack="false" />
                    </div>
                </div>
                <%-- NEW OR EXISTING --%>
                <div runat="server" id="divAddNewOrExisting" style="margin-left: auto; margin-right: auto; width: 500px; height: 225px; font-size: 12pt; display: none;">
                    <br />
                    <telerik:RadButton runat="server" ID="rdoAddNew"
                        Skin="Web20" Font-Size="12pt"
                        Text="Add New"
                        ToggleType="Radio"
                        ButtonType="ToggleButton"
                        Checked="false"
                        GroupName="CreateNewExisting"
                        AutoPostBack="False" />
                    <br />
                    <telerik:RadButton runat="server" ID="rdoAddExisting"
                        Skin="Web20" Font-Size="12pt"
                        Text="Copy Existing"
                        ToggleType="Radio"
                        ButtonType="ToggleButton"
                        Checked="true"
                        GroupName="CreateNewExisting"
                        AutoPostBack="False" />
                    <br />
                    <br />
                    <div style="float: right; margin-right: 20px;">
                        <%--New or Existing buttons--%>
                        <telerik:RadButton runat="server" ID="btnOkNew"
                            Text="OK"
                            Skin="Web20"
                            AutoPostBack="false" />
                        <telerik:RadButton runat="server" ID="btnCancelNew"
                            Text="Cancel"
                            Skin="Web20"
                            AutoPostBack="false" />

                    </div>

                </div>


            </ContentTemplate>


        </telerik:RadWindow>
        <%-- Not being used? --%>
        <telerik:RadWindow runat="server" ID="wndCmsNewDocumentShell"
            AutoSize="True"
            ShowContentDuringLoad="False"
            Behaviors="Close,Move,Resize"
            ReloadOnShow="True"
            Modal="True"
            Skin="Web20"
            VisibleStatusbar="False"
            OnClientClose="OnCmsDocumentDialogClosed" />
    </Windows>
</telerik:radwindowmanager>

<asp:HiddenField runat="server" ID="hdnCmsDocumentLocation" Value="2" />

<telerik:radajaxpanel id="documentTypeListAjaxPanel" runat="server" loadingpanelid="documentTypeListLoadingPanel">

    <div style="height: 25px">

        <telerik:RadComboBox ID="cmbUserType" runat="server"
            ToolTip="Document Scope"
            Skin="Web20"
            Width="70"
            DropDownWidth="200px"
            CausesValidation="False"
            HighlightTemplatedItems="true"
            AutoPostBack="True"
            OnSelectedIndexChanged="cmbDocumentList_SelectedIndexChanged">
            <ItemTemplate>
                <span><%# Eval("Name")%></span>
            </ItemTemplate>
        </telerik:RadComboBox>

        <telerik:RadComboBox ID="cmbGrade" runat="server"
            ToolTip="Select a grade"
            Skin="Web20"
            Width="70"
            AutoPostBack="true"
            CausesValidation="False"
            OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged"
            HighlightTemplatedItems="true">
            <ItemTemplate>
                <span>
                    <%# Eval("Grade") %>
                </span>
            </ItemTemplate>
        </telerik:RadComboBox>

        <telerik:RadComboBox ID="cmbSubject" runat="server"
            ToolTip="Select a subject"
            Skin="Web20"
            Width="70"
            MaxHeight="300"
            AutoPostBack="true"
            CausesValidation="False"
            OnSelectedIndexChanged="cmbSubject_SelectedIndexChanged"
            HighlightTemplatedItems="true"
            DropDownWidth="200px">
            <ItemTemplate>
                <span>
                    <%# Eval("Subject") %>
                </span>
            </ItemTemplate>
        </telerik:RadComboBox>

        <telerik:RadComboBox ID="cmbCourse" runat="server"
            ToolTip="Select a course"
            Skin="Web20"
            Width="70"
            MaxHeight="300"
            AutoPostBack="true"
            CausesValidation="False"
            OnSelectedIndexChanged="cmbCourse_SelectedIndexChanged"
            HighlightTemplatedItems="true"
            DropDownWidth="200px">
            <ItemTemplate>
                <span>
                    <%# Eval("Course") %>
                </span>
            </ItemTemplate>
        </telerik:RadComboBox>

        <telerik:RadComboBox ID="cmbResourceType" runat="server"
            ToolTip="Select a Type"
            Skin="Web20"
            Width="70"
            AutoPostBack="true"
            CausesValidation="False"
            HighlightTemplatedItems="true"
            OnSelectedIndexChanged="cmbResourceType_SelectedIndexChanged"
            Visible="false"
            DropDownWidth="200px">
            <ItemTemplate>
                <span>
                    <%# Eval("DropdownText") %>
                </span>
            </ItemTemplate>
        </telerik:RadComboBox>

        <telerik:RadComboBox ID="cmbResourceSubType" runat="server"
            ToolTip="Select a SubType"
            Skin="Web20"
            Width="70"
            AutoPostBack="true"
            CausesValidation="False"
            HighlightTemplatedItems="true"
            Visible="false"
            OnSelectedIndexChanged="cmbResourceSubType_SelectedIndexChanged"
            DropDownWidth="200px">
            <ItemTemplate>
                <span>
                    <%# Eval("DropdownText") %>
                </span>
            </ItemTemplate>
        </telerik:RadComboBox>

    </div>

    <div class="" style="height: auto; padding: 0;">
        <asp:Panel ID="pnlNoResults" runat="server"
            Visible="false"
            Height="100%">
            <div style="width: 100%; text-align: center;">
                No results found.
            </div>
        </asp:Panel>
        <telerik:RadListBox runat="server" ID="lbxdocumentTypeList"
            Width="100%"
            Height="190px">
            <ItemTemplate>
                <div>
                    <img src="<%# ResourceToShow != "Thinkgate.CompetencyList" ? "../Images/resources.png" : "../Images/competency.png"  %>" style="height: 56px; width: 47px; float: left; padding: 2px;">
                    <a href="<%:KenticoHelper.KenticoVirtualFolderRelative%><%# Eval("NodeAliasPath")%>.aspx?viewmode=3&isThinkgateSession=true&portalName=<%= PortalName %>" target="_blank" title="<%:KenticoHelper.KenticoVirtualFolderRelative%><%# Eval("NodeAliasPath")%>"><%# Eval("NodeName")%></a>
                    <%=" - " %><%# Eval("Description")%>
                    <br />
                    <asp:PlaceHolder ID="phEdit" runat="server" Visible="<%# ((UserNodeList)Container.DataItem).IsEditable %>">
                        <%# "<a href=" + KenticoHelper.KenticoVirtualFolderRelative + "/CMSModules/Content/CMSDesk/Edit/edit.aspx?nodeid=" + Eval("NodeId") + "&culture=en-US&isThinkgateSession='true' target='_blank'><img style='height:16px' title='Edit " + Eval("NodeName") + "' src='../Images/Edit.png' alt='Edit " + Eval("NodeName") + "'></a>" %>	
                    </asp:PlaceHolder>
                </div>
            </ItemTemplate>
        </telerik:RadListBox>
    </div>
</telerik:RadAjaxPanel>
<div style="float:left;margin-left:5px;" id="divOhioLink" runat="server" visible="false"><asp:Label ID="Label1" runat="server" Text="Label">Click <asp:HyperLink ID="hLinkOhio" Target="_blank" runat="server">here</asp:HyperLink> for INFOhio resources</asp:Label>
        </div>
<div runat="server" id="btnAdd" class="searchTileBtnDiv_smallTile" title="Add New"
    style="margin-top: 1px;">
    <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
    <div style="padding: 0;">
        Add New
    </div>
</div>

<%-- ReSharper disable UseOfImplicitGlobalInFunctionScope --%>

<script type="text/javascript">

    function populateSubType(typeElement) {
        var type = $(typeElement).val();
        var ctrlSubType = typeElement.id.replace("ddlType", "ddlSubType");
        var control = $("#" + ctrlSubType);
        $.ajax({
            type: "POST",
            url: "../Services/KenticoServices/KenticoWebServices.aspx/getsubtype",
            data: "{'type':'" + type + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            error: function (xmlHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            },
            success: function (result) {
                if (control) {
                    control.html(result.d);
                }
            }
        });

    }

    function populateFormType(subtypeElement) {
        var subtype = $(subtypeElement).val();
        var ctrlFormType = subtypeElement.id.replace("ddlSubType", "ddlFormType");
        var control = $("#" + ctrlFormType);

        var addNewbutton = subtypeElement.id.replace("ddlSubType", "rdoAddNew");

        if (addNewbutton && $find(addNewbutton).get_checked()) {
            $.ajax({
                type: "POST",
                url: "../Services/KenticoServices/KenticoWebServices.aspx/GetFormType",
                data: "{'subtype':'" + subtype + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (xmlHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                },
                success: function (result) {
                    if (control) {
                        if (result.d == null) {
                            $('#divFormType').css('display', 'none');
                        } else {
                            $('#divFormType').css('display', 'block');
                            control.html(result.d);
                        }
                    }
                }
            });
        } else {
            $('#divFormType').css('display', 'none');
        }

    }

</script>

<%-- ReSharper restore UseOfImplicitGlobalInFunctionScope --%>

