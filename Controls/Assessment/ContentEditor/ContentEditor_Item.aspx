<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ContentEditor_Item.aspx.cs"
    Inherits="Thinkgate.Controls.Assessment.ContentEditor.ContentEditor_Item" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<%--This height of 100% helps elements fill the page--%>
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
	<link href="~/Styles/ContentEditorCSS.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/ItemBanks/MCAS.css" rel="stylesheet" type="text/css"/>
    <link href="~/Styles/ItemBanks/Inspect.css" rel="stylesheet" type="text/css"/>
    <link href="~/Styles/ItemBanks/NWEA.css" rel="stylesheet" type="text/css"/>
    <title></title>
   
    <base target="_self" />
    <script type='text/javascript' src='../../../Scripts/jquery-1.9.1.min.js'></script>
    <script type='text/javascript' src='../../../Scripts/jquery-migrate-1.1.0.min.js'></script>
    <script type='text/javascript' src='../../../Scripts/jquery.scrollTo.js'></script>
	<script type="text/javascript">        var $j = jQuery.noConflict();</script>
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$appSettings:MathJaxUrl%>' />/MathJax.js?config=TeX-AMS-MML_HTMLorMML,<asp:Literal runat='server' Text='<%$appSettings:MathJaxUrl%>' />/config/local/thinkgate-e3.js"></script>
    </telerik:RadScriptBlock> 
</head>
  

<!--Bug 14195: JDW 03_05_2014 - Scroll bars not working/not appearing correctly and consistantly after images are added to distractors-->
<!--    14195: Override whatever the custom dialog's JS method 'DestroyOnClose' is doing to this page's scroll bars when it's is called-->
<body id="ovrRideForScrollBarDisplay" style="background-color: rgb(222, 226, 231);">
    <asp:PlaceHolder runat="server">
        <script type="text/javascript">
            function GetEquationEditorURL() {
                var _EquationEditorURL = '<%=ConfigurationManager.AppSettings["EquationEditorURL"].ToString() %>';
                return _EquationEditorURL;
            }
        </script>
   </asp:PlaceHolder>
    <form id="form_image" runat="server" style="min-width: 1000px;">
        <div runat="server" id="divItemID" style="display: none;">
        </div>
        <div runat="server" id="divEncItemID" style="display: none;">
        </div>
        <div runat="server" id="divAssessmentID" style="display: none;">
        </div>
        <div runat="server" id="divTestQ" style="display: none;">
        </div>
        <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/ckeditor/ckeditor.js"/>
                <asp:ScriptReference Path="~/Scripts/ckeditor/adapters/jquery.js"/>
                <asp:ScriptReference Path="~/Scripts/ContentEditor_Item.js" />
                <asp:ScriptReference Path="~/Scripts/master.js?d=2" />
                <asp:ScriptReference Path="~/Scripts/json2.js" />
                <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery.ui.touch-punch.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
		        <asp:ScriptReference Path="~/Scripts/ContentCkEditor.js"/>
            </Scripts>
            <Services>
                <asp:ServiceReference Path="~/Services/Service2.svc" />
            </Services>
        </telerik:RadScriptManager>
        <script type="text/javascript">
            var rubricTextSrc = '<%= ResolveUrl("~/Controls/Rubrics/RubricText.aspx")%>';
            var useTQ = ('<%=UseTq %>' == 'True');
            var cursorPosition;
            var currentEditor;
            var isCalled = false;
            /*****************************************************************************************************************************************
            * Confirm close as items without "standard" set are orphaned in the database and there is no way for customer to view orphaned items
            ********************************************************************************************************************************************/
            //Track window across postbacks of client
            if (parent) {
               
                    var win = getCurrentCustomDialog();
                    if (parent.onClientBeforeClose) {
                        win.remove_beforeClose(parent.onClientBeforeClose);
                    }
                    parent.onClientBeforeClose = localClientBeforeClose;
                    win.add_beforeClose(parent.onClientBeforeClose);
            
            }
           
            function localClientBeforeClose(sender, arg) {
                if (isCalled == false)
                {
                    function confirmCallback(arg) {
                        if (arg == true) {
                            sender.remove_beforeClose(localClientBeforeClose);

                            //Have to do this for IE since it doesn't dispose IFrames properly.
                            setTimeout(function () {
                                sender.close();
                            }, 0);
                        }
                    }
                    function saveContentBeforeClose(editor) {
                        if (typeof currentEditableBody !== "undefined" && editor !== null) {
                            assessmentitem_changeField(currentEditableBody, 'Item_Text', editor.getData());
                        }
                    }

                    saveContentBeforeClose(currentEditor);
                    //Verify if standard is selected in one of three locations (multiple select or single select)       
                    if (sender._name == "ContentEditorITEM" && ($("#divStandardsLabel a").length + $("#divStandards ul li a").length + $("#lblStandardNamePanel").text().length) <= 1) {
                        var tabstrip = $find("<%= RadTabStrip1.ClientID %>");
                        if (tabstrip.get_selectedTab().get_value() == "Preview") {
                            tabstrip.findTabByValue("Edit").select();
                        }
                        arg.set_cancel(true);                   
                        if (isItemDeleted == false) {
                        var wnd = radconfirm('If you exit without attaching a standard, the item will be marked as unmapped. Select the unmapped standard set in the item search to find items not attached to standards.', confirmCallback, 330, 100, null, 'A standard is recommended for this item');
                        wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                        }
                        else {                       
                            isCalled = true;
                            isItemDeleted = false;
                            var wnd = getCurrentCustomDialog();
                        wnd.close();

                            //wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);                                               
                        }
                        }
                    }
            }

            function setCursorPos(editor) {
                try {
                    currentEditor = editor;
                    currentEditableBody = $(editor.element.$).parent();
                }
                catch (e) {

                }
            }

            function InsertImage(img) {
                if (currentEditor == null) setCursorPos(CKEDITOR.instances.Question_Text);
                if (currentEditor !== null) {
                    currentEditor.insertHtml(String.format("<img src='{0}' border='0' align='' alt='' /> ", img));
                    var itemText = currentEditor.getData();
                    var currentEditableBody = $(currentEditor.element.$).parent();
                    assessmentitem_changeField(currentEditableBody, 'Item_Text', itemText);
                }
            }

            function onLinePreviewClick() {
                $("#iframe_preview").hide();

                var otc_url = document.getElementById('lbl_OTCUrl').value + ($('#ItemContentTile_hdnThumbnailURL')[0].value);
                customDialog(
				    { name: 'OnlinePreview',
				        maximize: true,
				        title: 'Online Preview',
				        url: otc_url,
				        dialog_style: 'alert',
				        onClosed: showIFrame
				    }
			    );
            }

            function showIFrame() {
                $("#iframe_preview").show();
            }

            function MessageAlert(message) {
                parent.customDialog({ title: 'Update Error', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: message }, [{ title: 'OK' }]);;
            }

            if (CKEDITOR) {
                CKEDITOR.disableAutoInline = true;
            }

        </script>
	<input runat="server" type="hidden" id="lbl_OTCUrl"  />
        <input runat="server" type="hidden" id="ItemContentTile_hdnThumbnailURL" clientidmode="Static" />
        <input runat="server" type="hidden" id="ContentEditor_Item_hdnRubricID" clientidmode="Static" />
    <input id="lbl_TestCategory" type="hidden" runat="server"/>
         <input id="lbl_TestType" type="hidden" runat="server"/>
	<div runat="server" id="ContentEditor_Item_hdnRubricContent" clientidmode="Static" style="display:none;"></div>

        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
        </telerik:RadSkinManager>
        <asp:Image runat="server" ID="undo_inactive" ImageUrl="~/Images/Toolbars/undo_inactive.png"
            Style="display: none" />
        <asp:Image runat="server" ID="undo_active" ImageUrl="~/Images/Toolbars/undo.png"
            Style="display: none" />
        <div class="IconHeader">
            <telerik:RadToolBar ID="mainToolBar" runat="server" Style="" Width="722px" Skin="Sitefinity" OnClientLoad="mainToolBar_OnClientLoad"
                EnableRoundedCorners="true" EnableShadows="true" OnClientButtonClicked="mainToolBar_OnClientButtonClicked">
                <Items>
                    <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/undo_inactive.png" ToolTip="Undo"
                        Value="Undo" Style="display: none" />
                    <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/delete.png" ToolTip="Delete Item"
                        Value="DeleteItem" />
                    <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/copy_assessment.png" ToolTip="Copy Item"
					Value="CopyItem" ID="btnCopyItem"/>
                    <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/print.png" ToolTip="Print"
                        Value="PrintItem" />
                    <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/preview.png" ToolTip="Item Online Preview"
                        Value="OnlinePreview" />
                    <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/LrmiTags.png" ToolTip="Tags"
                        Value="Tags" runat="server" ClientIDMode="Static" id="rtbBtnLRMI" />                  
                </Items>
            </telerik:RadToolBar>
		<asp:Image ID="FinishAndReturn" runat="server" Style="float: right; margin-top: 10px;
			margin-right: 10px; display: none;" ImageUrl="~/Images/finish_return.png" onclick="InsertImage('abc123.png');" />
        </div>
        <br />
        <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1"
            SelectedIndex="0" Align="Justify" ReorderTabsOnSelect="true" Width="347px" Skin="Thinkgate_Blue"
            EnableEmbeddedSkins="false" OnClientTabSelecting="mainTabStrip_OnClientTabSelecting">
            <Tabs>
                <telerik:RadTab Text="Edit" Value="Edit">
                </telerik:RadTab>
                <telerik:RadTab Text="Preview" Value="Preview">
                </telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
        <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" Width="100%"
            CssClass="Web20">
            <telerik:RadPageView runat="server" ID="RadPageView1" CssClass="Web20">
                <telerik:RadPanelBar runat="server" ID="RadPanelBar1" Width="100%" Skin="Web20" ExpandMode="MultipleExpandedItems"
                    OnClientLoad="onLoad" ClientIDMode="Static">
                    <Items>
                        <telerik:RadPanelItem Text="Identification" Expanded="true" runat="server" ID="Item_Identification">
                            <HeaderTemplate>
                                <div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                                <span class="rpExpandHandle"></span>
                                            </td>
                                            <td style="width: 25%;">
                                                <div class="RadPanelItemHeader">
												Identification:</div>
                                            </td>
										<td style="width: 25%;">
											Grade:&nbsp;&nbsp;<asp:Label runat="server" ID="lblGradeEdit"></asp:Label>
                                            </td>
										<td style="width: 25%;">
											Subject:&nbsp;&nbsp;<asp:Label runat="server" ID="lblSubjectEdit"></asp:Label>
                                            </td>
										<td style="width: 15%;" unselectable="on">
											Status:&nbsp;&nbsp;<asp:Label runat="server" ID="lblStatusEdit"></asp:Label>
                                            </td>
                                            <td style="width: 5%;" unselectable="on">
											<div onclick="stopPropagation(event)" style="" unselectable="on"><font style="text-align:right;display:inline-block;width:50%;" unselectable="on"><a href="javascript:" onclick="stopPropagation(this); OpenAdvancedItemInfo();" unselectable="on">Advanced</a></font></div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 40%; border-right: 1px solid grey;">
                                            <!-- Column 1  -->
                                            <table width="100%" class="fieldValueTable">
                                                <tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Grade:
                                                    </td>
                                                    <td>
                                                        <telerik:RadComboBox ID="gradeDropdown" ClientIDMode="Static" runat="server" Skin="Web20"
                                                            OnClientSelectedIndexChanged="requestFilter" EmptyMessage="Select" xmlHttpPanelID="standardsFilterSubjectXmlHttpPanel"
                                                            field="Grade">
                                                        </telerik:RadComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Subject:
                                                    </td>
                                                    <td>
                                                        <telerik:RadComboBox ID="subjectDropdown" ClientIDMode="Static" runat="server" Skin="Web20"
                                                            OnClientSelectedIndexChanged="requestFilter" xmlHttpPanelID="standardsFilterCourseXmlHttpPanel"
                                                            EmptyMessage="Select" field="Subject">
                                                        </telerik:RadComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Keywords:
                                                    </td>
                                                    <td>
                                                        <telerik:RadTextBox runat="server" ID="txtKeywords" Skin="Web20" field="Keywords"
                                                            ClientEvents-OnValueChanged="Assessment_ChangeField" />
                                                    </td>
                                                </tr>
                                                <tr>
												<td class="fieldLabel" style="padding-left: 10px;">
													Item Status:
                                                    </td>
                                                    <td class="fieldLabel" style="padding-left: 10px;">
                                                        <telerik:RadComboBox ID="itemstatusDropDown" ClientIDMode="Static" runat="server"
                                                            Skin="Web20" OnClientSelectedIndexChanged="Assessment_ChangeField" EmptyMessage="Select"
                                                            field="ReviewStatus" Width="95">
                                                        </telerik:RadComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
												<td class="fieldLabel" style="padding-left: 10px;">
													Reservation:
                                                    </td>
                                                    <td class="fieldLabel" style="padding-left: 10px;">
                                                        <telerik:RadComboBox ID="reservationDropDown" ClientIDMode="Static" runat="server"
                                                            Skin="Web20" OnClientSelectedIndexChanged="Assessment_ChangeField" EmptyMessage="Select"
                                                            field="Test_Type" Width="95">
                                                        </telerik:RadComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Comments:
                                                    </td>
                                                    <td>
                                                        <telerik:RadTextBox runat="server" ID="txtComments" Skin="Web20" field="Comments"
                                                            ClientEvents-OnValueChanged="Assessment_ChangeField" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="width: 20%; border-right: 1px solid grey; vertical-align: top; padding: 0px 5px 0px 5px;">
                                            <!-- Column 2  -->
                                            <table style="width: 100%;" id="tblItemBank" runat="server">
                                                <tr>
                                                    <td style="width: 40%; padding: 0px 5px 0px 5px;">
                                                        <hr />
                                                    </td>
												<td style="font-weight: bold; white-space: nowrap;">
													Item Banks
                                                    </td>
                                                    <td style="width: 40%; padding: 0px 5px 0px 5px;">
                                                        <hr />
                                                    </td>
                                                </tr>
                                            </table>
                                            <div runat="server" id="ItemBankCheckBoxes" style="width: 100%;">
                                            </div>
                                        </td>
                                        <td style="width: 40%; vertical-align: top; padding: 0px 5px 0px 5px;" class="fieldValueTable">
                                            <!-- Column 3  -->
                                            <table style="width: 100%;">
                                                <tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Author:
                                                    </td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblAuthor"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													District:
                                                    </td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblDistrict"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Copyright:
                                                    </td>
                                                    <td>
                                                        <telerik:RadButton ID="rbCopyRightYes" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
                                                            OnClientClicking="Assessment_ChangeField_copyright" field="Copyright" cbvalue="Yes"
                                                            GroupName="CopyRight" Text="Yes" AutoPostBack="false">
                                                        </telerik:RadButton>
                                                        &nbsp;&nbsp;&nbsp;
													<telerik:RadButton ID="rbCopyRightNo" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
                                                        OnClientClicking="Assessment_ChangeField_copyright" field="Copyright" cbvalue="No"
                                                        GroupName="CopyRight" Text="No" AutoPostBack="false">
                                                    </telerik:RadButton>
                                                    </td>
                                                </tr>
                                                                                           
                                                <tr id="trExpiryDate" runat="server" clientidmode="Static">
                                                    <td class="fieldLabel" style="width: 95px; padding-left: 10px;">Expiration Date:
                                                    </td>
                                                    <td>

                                                        <asp:Label runat="server" ID="lblExpirationDate"></asp:Label>
                                                      

                                                    </td>
                                                </tr>

                                                <tr id="trSource" runat="server" clientidmode="Static">
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Source:
                                                    </td>
                                                    <td>
                                                        <telerik:RadTextBox runat="server" ID="txtSource" Skin="Web20" field="Source" ClientEvents-OnValueChanged="Assessment_ChangeField" />
                                                    </td>
                                                </tr>
                                                <tr id="trCredit" runat="server" clientidmode="Static">
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Credit:
                                                    </td>
                                                    <td>
                                                        <telerik:RadTextBox runat="server" ID="txtCredit" Skin="Web20" field="Credit" ClientEvents-OnValueChanged="Assessment_ChangeField" />
                                                    </td>
                                                </tr>
                                                <tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Date Created:
                                                    </td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblDateCreated"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Last Updated:
                                                    </td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblDateUpdated"></asp:Label>
                                                    </td>
                                                </tr>
                                            <tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Anchor Item:
												</td>
												<td>
													<telerik:RadButton ID="rbAnchorItemYes" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
														OnClientClicking="Assessment_ChangeField" field="AnchorItem" cbvalue= "true"
														GroupName="AnchorItem" Text="Yes" AutoPostBack="false">
													</telerik:RadButton>
													&nbsp;&nbsp;&nbsp;
													<telerik:RadButton ID="rbAnchorItemNo" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
														OnClientClicking="Assessment_ChangeField" field="AnchorItem" cbvalue="false"
														GroupName="AnchorItem" Text="No" AutoPostBack="false">
													</telerik:RadButton>
												</td>
											</tr>
                                                <tr id="trDocumentUpload" runat="server">
                                                    <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                        Document Upload:
                                                    </td>
                                                    <td>
                                                        <telerik:RadButton ID="rbDocUploadYes" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
														OnClientClicking="Assessment_ChangeField" field="DocumentUpload" cbvalue= "true"  
														GroupName="DocUpload" Text="Yes" AutoPostBack="false">
													</telerik:RadButton>
													&nbsp;&nbsp;&nbsp;
													<telerik:RadButton ID="rbDocUploadNo" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
														OnClientClicking="Assessment_ChangeField" field="DocumentUpload" cbvalue="false"
														GroupName="DocUpload" Text="No" AutoPostBack="false">
													</telerik:RadButton>
                                                    </td>
                                                </tr>
                                                 <tr id="trAllowComments" runat="server">
                                                    <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                        Allow Comments:
                                                    </td>
                                                    <td>
                                                        <telerik:RadButton ID="rbAllowCommentYes" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
														OnClientClicking="Assessment_ChangeField" field="AllowComments" cbvalue= "true"
														GroupName="AllowComment" Text="Yes" AutoPostBack="false">
													</telerik:RadButton>
													&nbsp;&nbsp;&nbsp;
													<telerik:RadButton ID="rbAllowCommentNo" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
														OnClientClicking="Assessment_ChangeField" field="AllowComments" cbvalue="false"
														GroupName="AllowComment" Text="No" AutoPostBack="false">
													</telerik:RadButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </telerik:RadPanelItem>
                        <telerik:RadPanelItem Expanded="true" runat="server" ID="pnlItem_Standard">
                            <HeaderTemplate>
                                <div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                                <span class="rpExpandHandle"></span>
                                            </td>
                                            <td style="width: 40%; white-space: nowrap;">
                                                <div class="RadPanelItemHeader" id="divStandardsLabel" runat="server">
                                                    Standard(s): &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label runat="server" ID="lblStandardNamePanel"
                                                        ClientIDMode="Static">
                                                        <a href="javascript:" id="lblStandardNamePanelLink" runat="server" clientidmode="Static" style="display:none;"></a>
                                                        </asp:Label>
                                                    &nbsp;<img src="../../../Images/cross.gif" title="Remove Standard" alt="" id="imgRemoveStandard"
                                                        runat="server" clientidmode="Static" />
                                                </div>
                                            </td>
                                            <td style="width: 15%; white-space: nowrap;">
                                                <div onclick="stopPropagation(event)" style="" unselectable="on">
                                                    <div style='background-image: url("../../../Images/find_btn.png");' id="Div2" class="bottomTextButton"
                                                        title="Search Standards" onclick="SearchItem('Standards');">
													Search</div>
                                                </div>
                                            </td>
										<td style="width: 15%; white-space: nowrap;">
										</td>
										<td style="width: 29%; white-space: nowrap;">
											&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="divUploadTemplate" id="divStandards">
                                    <asp:Repeater ID="rptrStandards" runat="server">
                                        <HeaderTemplate>
                                            <ul>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <li id="StandardListElement">
                                                <a id="AddedStandardItem" href="javascript:" onclick="OpenStandardText('<%# DataBinder.Eval(Container.DataItem, "EncID")%>');">
                                                    <%# DataBinder.Eval(Container.DataItem, "StandardName")%>
                                            </a> &nbsp;<img src="../../../Images/cross.gif" title="Remove Standard" onclick="RemoveStandardfromItem('<%# DataBinder.Eval(Container.DataItem, "EncID")%>'); return false;" />
                                            </li>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </ul>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </ContentTemplate>
                        </telerik:RadPanelItem>
                        <telerik:RadPanelItem Expanded="true" runat="server" ID="pnlItem_Addendum">
                            <HeaderTemplate>
                                <div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
                                    <table style="width: 100%;">
                                        <tr runat="server" id="trAddendum_No" style="" clientidmode="Static">
                                            <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                                <span class="rpExpandHandle"></span>
                                            </td>
                                            <td style="width: 40%;">
                                                <div class="RadPanelItemHeader">
												Addendum:</div>
                                            </td>
                                            <td style="width: 15%;">
                                                <div onclick="stopPropagation(event)" style="" unselectable="on">
                                                    <div style='background-image: url("../../../Images/find_btn.png");' id="Div3" class="bottomTextButton"
                                                        title="Search Addendums" onclick="SearchItem('Addendum');">
													Search</div>
                                                </div>
                                            </td>
                                            <td style="width: 15%;">
                                                <div onclick="stopPropagation(event)" style="" unselectable="on">
                                                    <div style='background-image: url("../../../Images/add.gif");' id="Div4" class="bottomTextButton"
                                                        title="Add New Addendum" onclick="stopPropagation(this); addNewItem('Addendum');"
                                                        unselectable="on">
													Add New</div>
                                                </div>
                                            </td>
										<td style="width: 29%;">
											&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                        <tr runat="server" id="trAddendum_Yes" style="display: none;" clientidmode="Static">
                                            <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                                <span class="rpExpandHandle"></span>
                                            </td>
                                            <td style="width: 40%; white-space: nowrap;">
                                                <div class="RadPanelItemHeader">
                                                    Addendum: &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="javascript:" onclick="OpenAddendumText();"><asp:Label
                                                        runat="server" ID="lblNameAddendum" ClientIDMode="Static"></asp:Label></a> &nbsp;<img
                                                            src="../../../Images/cross.gif" alt="Remove Addendum" onclick="RemoveAddendum();" />
                                                </div>
                                            </td>
										<td style="width: 15%; white-space: nowrap; text-align: right;">
											Type:&nbsp;&nbsp;<asp:Label runat="server" ID="lblTypeAddendum" ClientIDMode="Static"></asp:Label>
                                            </td>
										<td style="width: 15%; white-space: nowrap; text-align: right;">
											Genre:&nbsp;&nbsp;<asp:Label runat="server" ID="lblGenreAddendum" ClientIDMode="Static"></asp:Label>
                                            </td>
										<td style="width: 29%;">
											&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="divUploadTemplate">
                                </div>
                            </ContentTemplate>
                        </telerik:RadPanelItem>
                        <telerik:RadPanelItem Expanded="false" runat="server" ID="Addendum_Image">
                            <HeaderTemplate>
                                <div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                                <span class="rpExpandHandle"></span>
                                            </td>
                                            <td style="width: 40%;">
                                                <div class="RadPanelItemHeader">
												Images:</div>
                                            </td>
                                            <td style="width: 15%;">
                                                <div onclick="stopPropagation(event)" style="" unselectable="on">
                                                    <div style='background-image: url("../../../Images/find_btn.png");' id="Div1" class="bottomTextButton"
                                                        title="Search Images" onclick="stopPropagation(this); SearchItem('Image');">
													Search</div>
                                                </div>
                                            </td>
                                            <td style="width: 15%;">
                                                <div onclick="stopPropagation(event)" style="" unselectable="on">
                                                    <div style='background-image: url("../../../Images/add.gif");' id="btnAdd" class="bottomTextButton"
                                                        title="Add New Image" onclick="stopPropagation(this); addNewItem('Image');" unselectable="on">
													Add New</div>
                                                </div>
                                            </td>
										<td style="width: 29%;">
											&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="divUploadTemplate">
                                    <div class="LegalMessage">
                                        <br />
                                        You must honor all copyright protection notifications on all material used within
									Elements&trade;. Only use content (Items, Distractors,<br />
                                        Images, Addendums, and Assessments) that your school system has purchased the rights
									to reproduce or that has been marked as public<br />
                                        domain. You are responsible for any copyright infringements. If in doubt about the
									content you wish to use, contact your central
									<br />
                                        office for permission clarification.
									<br />
                                        &nbsp;
                                    </div>
                                </div>
                            </ContentTemplate>
                        </telerik:RadPanelItem>
                        <telerik:RadPanelItem Expanded="true" runat="server" ID="pnlItem_Rubric" Style="display: none;"
                            ClientIDMode="Static">
                            <HeaderTemplate>
                                <div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
                                    <table style="width: 100%;">
                                        <tr runat="server" id="trRubric_No" style="display: none;" clientidmode="Static">
                                            <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                                <span class="rpExpandHandle"></span>
                                            </td>
                                            <td style="width: 40%;">
                                                <div class="RadPanelItemHeader">
												Rubric:</div>
                                            </td>
                                            <td style="width: 15%;">
                                                <div onclick="stopPropagation(event)" style="" unselectable="on">
                                                    <div style='background-image: url("../../../Images/find_btn.png");' id="Div5" class="bottomTextButton"
                                                        title="Search Rubrics" onclick="stopPropagation(this); SearchItem('Rubric'); ">
													Search</div>
                                                </div>
                                            </td>
                                            <td style="width: 15%;">
                                                <div onclick="stopPropagation(event)" style="" unselectable="on">
                                                    <div style='background-image: url("../../../Images/add.gif");' id="Div6" class="bottomTextButton"
                                                        title="Add New" onclick="stopPropagation(this); addNewItem('Rubric');" unselectable="on">
													Add New</div>
                                                </div>
                                            </td>
										<td style="width: 29%;">
											&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                        <tr runat="server" id="trRubric_Yes" style="display: none;" clientidmode="Static">
                                            <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                                <span class="rpExpandHandle"></span>
                                            </td>
                                            <td style="width: 40%;">
                                                <div class="RadPanelItemHeader">
                                                    Rubric:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
												<a runat="server" id="ContentEditor_Item_hlRubricContent" clientidmode="Static" href="javascript:void(0);" onclick="OpenRubricText();">
                                                    <asp:Label runat="server" ID="lblRubricName" ClientIDMode="Static"></asp:Label>
												</a> &nbsp;
												<img runat="server" id="ContentEditor_Item_imgRemoveRubric" clientidmode="Static" style="display:inline;" src="../../../Images/cross.gif" alt="Remove Addendum" onclick="RemoveRubric();" />
                                                </div>
                                            </td>
										<td style="width: 15%; white-space: nowrap; text-align: right;">
											Type:&nbsp;&nbsp;<asp:Label runat="server" ID="lblRubricType" ClientIDMode="Static"></asp:Label>
                                            </td>
                                            <td style="width: 15%; white-space: nowrap; text-align: right;">
                                                <div runat="server" id="ContentEditor_Item_divRubricPanelRubricScoring" clientidmode="Static">
                                                    Scoring:&nbsp;&nbsp;<asp:Label runat="server" ID="lblRubricScoring" ClientIDMode="Static"></asp:Label>
                                                </div>
                                            </td>
										<td style="width: 29%;">
											&nbsp;&nbsp;
                                            <label id="lblInlineEditorStatus"></label>
                                            </td>

                                        </tr>
                                    </table>
                                </div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="divUploadTemplate">
                                </div>
                            </ContentTemplate>
                        </telerik:RadPanelItem>
                    </Items>
                </telerik:RadPanelBar>
			<div id="TopItemDropdowns" style="width: 100%; background-color: White; padding:2px;" class="fieldLabel">
                    Item Type:
				<telerik:RadComboBox ID="cmbItemType" runat="server" Skin="Web20" OnClientSelectedIndexChanged="Assessment_ChangeField_ItemType" field="QuestionType" Width="200px"></telerik:RadComboBox>
                    Score Type:
				<telerik:RadComboBox ID="cmbScoreType" runat="server" Width="130px" Skin="Web20" OnClientSelectedIndexChanged="Assessment_ChangeField_ScoreType" field="ScoreType"></telerik:RadComboBox>
                    <div runat="server" id="divRubricTypeLabel" clientidmode="Static" style="display: none;">
					<a href="javascript:" onclick="RubricTypeMessage();" >Rubric Type</a>
                        <telerik:RadComboBox ID="cmbRubricType" runat="server" Width="90px" Skin="Web20" OnClientSelectedIndexChanged="Assessment_ChangeField_RubricType"
                            field="RubricType">
                        </telerik:RadComboBox>
                        <div runat="server" id="divRubricScoringLabel" clientidmode="Static" style="display: none;">
                            Rubric Scoring:
						<telerik:RadComboBox ID="cmbRubricScoring" runat="server" Width="100px" Skin="Web20" OnClientSelectedIndexChanged="Assessment_ChangeField_RubricScoring"
                            field="RubricPoints">
                        </telerik:RadComboBox>
                        </div>
                    </div>
            </div>
            <div id="ckeditorToolbarHolder" style="width: 90%; height: 15px"></div>
			<div id="QuestionHolder" Width="100%" style="height: 100%; padding-left: 2px; border-bottom-color: black;
				border-bottom-width: 1px; border-bottom-style: solid; position: relative; overflow-x: hidden;">
                    <div id="ajaxPanel_ItemsPanel" style="display: block;">
                        <div id="ajaxPanel_Items">
                            <div class="ui-sortable ui-sortable-disabled" id="testQuestions">
                                <div style="padding: 2px; width: 100%;" id="<%=ItemID%>" class="questionInstance fieldTestItem_false questionInstance_highlight">
								<div style="clear: left; position: relative; table-layout: fixed;" class="tblContainer">
                                        <div class="tblRow" style="width: 100%;">
                                            <div style="padding-left: 10px; width: 20px;" class="sort_number item_number_div tblCell_InternalQuestionLayout">
                                            </div>
										<div class="SKEditableBody tblCell_InternalQuestionLayout" update="Q" style="width: 800px;
											min-height: 25px;">
											<div class="SKEditableBodyText" runat="server" contenteditable="true" id="Question_Text" style="min-height: 25px;">
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <table runat="server" id="contentEditor_Item_tblAdvanced" clientidmode="Static" style="width: 100%; display: block;">
                                <tr>
										<td style="width: 50%; text-align: left; font-size: 10px;">
                                        <div runat="server" id="divCorrectAnswer">
                                            Correct Answer:
                                        </div>
                                    </td>
                                    <td style="width: 50%; text-align: right;" id="tdRationalShowHide" runat="server"
                                        clientidmode="Static">
                                        <a href="javascript:" onclick="RationalShowHide(this);">Advanced</a>
                                    </td>
                                </tr>
                            </table>
								<div id="DistractorHolder" style="padding-left: 40px; display: block; border-bottom-color: rgb(204, 204, 204);
									border-bottom-width: 1px; border-bottom-style: solid; table-layout: fixed; border-spacing: 2px;"
                                class="tblContainer">
                                <asp:Repeater runat="server" ID="rptrItemQuestions">
                                    <HeaderTemplate>
                                            <div id="sortable"> <%--Original div was unclosed which broke PDF preview. Now closed in Footer to enable drag/drop--%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                              <div class="tblRow" >
                                            <!--class="response_letter tblCell_InternalQuestionLayout"-->
												<div class="handler" style="width: 40px; height: 40px; float: left;">
                                                <input id="CorrectAnswer" onchange="assessmentitem_changeField(this, 'Correct', this.value);" name="<%# "A_" + ItemID %>"
                                                    value="<%# DataBinder.Eval(Container.DataItem, "ID")%>" type="radio" <%# (bool)(DataBinder.Eval(Container.DataItem, "Correct")) ? "checked='true'" : ""  %> /><span class="sort_Index"><%# DataBinder.Eval(Container.DataItem, "Letter")%>.</span>
                                            </div>
                                            <div style="float: right;">
                                                <div class="SKEditableBody tblCell_InternalQuestionLayout answer_text" update="A_<%# DataBinder.Eval(Container.DataItem, "ID")%>"
                                                    style="width: 800px; display: inline-block;">
														<div  id="Distractor_Text_<%# DataBinder.Eval(Container.DataItem, "ID")%>" class="SKEditableBodyText" contenteditable="true">
                                                        <%# DataBinder.Eval(Container.DataItem, "DistractorText")%>
                                                    </div>
                                                </div>
                                                <div>
														<div id="DRHeader_<%# DataBinder.Eval(Container.DataItem, "ID")%>" style="display: none;
															width: 180px; text-align: right; height: 60px; float: left; padding-right: 5px;">
                                                        Rationale:
                                                    </div>
                                                    <div id="DRbody_<%# DataBinder.Eval(Container.DataItem, "ID")%>" class="SKEditableBody tblCell_InternalQuestionLayout answer_text"
															update="DR_<%# DataBinder.Eval(Container.DataItem, "ID")%>" style="display: none;
															xposition: relative; xdisplay: none; left: 195px; width: 615px;">
															<div id="DistractorRationale_Text_<%# DataBinder.Eval(Container.DataItem, "ID")%>" class="SKEditableBodyText" style="text-align: left; width: 615px;">
                                                            <%# DataBinder.Eval(Container.DataItem, "DistractorRationale")%>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </div> <%-- This is needed for the matching div tag in the header that enables drag and drop--%>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </div>
				</div>
			</div>
            </telerik:RadPageView>
		<telerik:RadPageView runat="server" ID="RadPageView5" CssClass="Web20" >
                <telerik:RadPanelBar runat="server" ID="RadPanelBar2s" Width="100%"
                    Skin="Web20" ExpandMode="MultipleExpandedItems" OnClientLoad="onLoad">
                    <Items>
                        <telerik:RadPanelItem Expanded="true">
                            <HeaderTemplate>
                                <div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                                <span class="rpExpandHandle"></span>
                                            </td>
                                            <td style="width: 25%;">
                                                <div class="RadPanelItemHeader">
												Identification:</div>
                                            </td>
										<td style="width: 25%;">
											Grade:&nbsp;&nbsp;<asp:Label runat="server" ID="lblGradePreview"></asp:Label>
                                            </td>
										<td style="width: 25%;">
											Subject:&nbsp;&nbsp;<asp:Label runat="server" ID="lblSubjectPreview"></asp:Label>
                                            </td>
										<td style="width: 24%;">
											Status:&nbsp;&nbsp;<asp:Label runat="server" ID="lblStatusPreview"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <iframe id="iframe_preview" style="width: 100%;"></iframe>
                            </ContentTemplate>
                        </telerik:RadPanelItem>
                    </Items>
                </telerik:RadPanelBar>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="false" ReloadOnShow="true">
        </telerik:RadWindowManager>
        <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
        <telerik:RadXmlHttpPanel runat="server" ID="standardsFilterSubjectXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/StandardsWCF.svc"
            WcfServiceMethod="LoadStandardsSubjectList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
            objectToLoadID="subjectDropdown">
        </telerik:RadXmlHttpPanel>
        <telerik:RadXmlHttpPanel runat="server" ID="standardsFilterCourseXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/StandardsWCF.svc"
            WcfServiceMethod="LoadStandardsCourseList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
            objectToLoadID="courseDropdown">
        </telerik:RadXmlHttpPanel>
    </form>
</body>
</html>
