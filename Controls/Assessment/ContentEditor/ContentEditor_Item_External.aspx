<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ContentEditor_Item_External.aspx.cs"
	Inherits="Thinkgate.Controls.Assessment.ContentEditor.External.ContentEditor_Item" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;" xmlns:m="http://www.w3.org/1998/Math/MathML">
<%--This height of 100% helps elements fill the page--%>
<head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=8" />
	<meta http-equiv="PRAGMA" content="NO-CACHE" />
	<meta http-equiv="Expires" content="-1" />
	<meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
	<link href="~/Styles/ContentEditorCSS.css" rel="stylesheet" type="text/css" />
	<title></title>
	<base target="_self" />
	<script type='text/javascript' src='../../../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../../../Scripts/jquery-migrate-1.1.0.min.js'></script>
	<script type='text/javascript' src='../../../Scripts/jquery.scrollTo.js'></script>
	<script type="text/javascript">        var $j = jQuery.noConflict();</script>
</head>
<body style="background-color: rgb(222, 226, 231);">
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
			<asp:ScriptReference Path="~/Scripts/master.js" />
			<asp:ScriptReference Path="~/Scripts/json2.js" />
            <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
            <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
            <asp:ScriptReference Path="~/Scripts/jquery.ui.touch-punch.js" />
            <asp:ScriptReference Path="~/Scripts/master.js" />
            <asp:ScriptReference Path="~/Scripts/ContentCkEditor.js"/>
            <asp:ScriptReference Path="~/Scripts/thinkgate.util.js"/>
		</Scripts>
		<Services>
			<asp:ServiceReference Path="~/Services/Service2.svc" />
		</Services>
	</telerik:RadScriptManager>
        	<script type="text/javascript">
        	    var rubricTextSrc = '<%= ResolveUrl("~/Controls/Rubrics/RubricText.aspx")%>';
        	    var useTQ = ('<%=useTestQuestion %>' == 'True');

        	    if (typeof CKEDITOR !== 'undefined') {
        	        CKEDITOR.disableAutoInline = true;
        	    }
             </script>
	<input runat="server" type="hidden" id="ItemContentTile_hdnThumbnailURL" clientidmode="Static" />
	<input runat="server" type="hidden" id="ContentEditor_Item_hdnRubricID" clientidmode="Static" />
    <input id="lbl_TestCategory" type="hidden" runat="server"/>
	<div runat="server" id="ContentEditor_Item_hdnRubricContent" clientidmode="Static" style="display:none;"></div>

	<telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
	</telerik:RadSkinManager>
	<div class="IconHeader">
		<%--<telerik:RadToolBar ID="mainToolBar" runat="server" Style="" Width="722px" Skin="Sitefinity" OnClientLoad="mainToolBar_OnClientLoad"
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
			</Items>
		</telerik:RadToolBar>--%>
	</div>
	<br />
<%--	<telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1"
		SelectedIndex="0" Align="Justify" ReorderTabsOnSelect="true" Width="347px" Skin="Thinkgate_Blue"
		EnableEmbeddedSkins="false" OnClientTabSelecting="mainTabStrip_OnClientTabSelecting">
		<Tabs>
			<telerik:RadTab Text="Edit" Value="Edit">
			</telerik:RadTab>
			<telerik:RadTab Text="Preview" Value="Preview">
			</telerik:RadTab>
		</Tabs>
	</telerik:RadTabStrip>--%>
	<telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" Width="100%"
		CssClass="Web20">
		<telerik:RadPageView runat="server" ID="RadPageView1" CssClass="Web20">
			<telerik:RadPanelBar runat="server" ID="RadPanelBar1" Width="100%" Skin="Web20" ExpandMode="MultipleExpandedItems"
				OnClientLoad="onLoad" ClientIDMode="Static">
				<Items>
					<telerik:RadPanelItem Text="Identification" Expanded="true" runat="server" ID="Item_Identification">
						<HeaderTemplate>
<%--							<div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
							</div>--%>
						</HeaderTemplate>
						<ContentTemplate>
						<table class="tblMain">
                    <tr>
                        <td colspan="4"></td>
                    </tr>
                    <tr>
                        <td class="tdLabel"> 
                              Marzano:
                        </td>
                        <td class="tdCombo">
                            <telerik:RadComboBox ID="comboMarzano" ClientIDMode="Static" runat="server" Skin="Web20"
                                EmptyMessage="Select" field="Marzano" OnClientSelectedIndexChanged="updateRigor">
                            </telerik:RadComboBox>
                        </td>
                        <td class="tdLabel">
                            <asp:Literal runat="server" ID="Literal1"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdLabel">
                             RBT:
                        </td>
                        <td class="tdCombo">
                            <telerik:RadComboBox ID="comboRBT" ClientIDMode="Static" runat="server" Skin="Web20"
                                EmptyMessage="Select" field="RBT" OnClientSelectedIndexChanged="updateRigor">
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                   <tr>
                        <td class="tdLabel">
                            Difficulty Index:
                        </td>
                        <td class="tdCombo">
                            <telerik:RadComboBox ID="comboDifficultyIndex" ClientIDMode="Static" runat="server"
                                Skin="Web20" EmptyMessage="Select" field="DifficultyIndex" OnClientSelectedIndexChanged="updateDifficultyIndex">
                            </telerik:RadComboBox>
                        </td>
                   </tr>
                   <tr>
                        <td class="tdLabel">
                            Webb:
                        </td>
                        <td class="tdCombo">
                            <telerik:RadComboBox ID="comboWebb" ClientIDMode="Static" runat="server" Skin="Web20"
                                EmptyMessage="Select" field="Webb" OnClientSelectedIndexChanged="updateRigor">
                            </telerik:RadComboBox>
                        </td>
                        <td>
                        </td>
                        <td class="tdCombo">
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
													ClientIDMode="Static"></asp:Label>
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
			<div style="display: none; height: 24px;">
				<div id="editorToolHolder" style="height: 24px;">
				</div>
			</div>
			
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
			<div id="QuestionHolder" style="height: 480px; padding-left: 2px; border-bottom-color: black;
				border-bottom-width: 1px; border-bottom-style: solid; position: relative; overflow-x: auto;
				overflow-y: auto;">
				<div id="ajaxPanel_ItemsPanel" style="display: block;">
					<div id="ajaxPanel_Items">
						<div class="ui-sortable ui-sortable-disabled" id="testQuestions">
							<div style="padding: 2px; width: 100%;" id="<%=_iItemID%>" class="questionInstance fieldTestItem_false questionInstance_highlight">
<%--								<div style="clear: left; position: relative; table-layout: fixed;" class="tblContainer">
									<div class="tblRow" style="width: 100%;">
										<div style="padding-left: 10px; width: 20px;" class="sort_number item_number_div tblCell_InternalQuestionLayout">
										</div>
										<div class="SKEditableBody tblCell_InternalQuestionLayout" update="Q" style="width: 800px;
											min-height: 25px;">
											<div class="SKEditableBodyText" runat="server" id="Question_Text" style="min-height: 25px;">
											</div>
										</div>
									</div>
								</div>--%>
								<table runat="server" id="contentEditor_Item_tblAdvanced" clientidmode="Static" style="width: 100%; display: block;">
									<tr>
										<td style="width: 50%; text-align: left; font-size: 10px;">
											<div runat="server" id="divCorrectAnswer">
												Rationale:
											</div>
										</td>
<%--										<td style="width: 50%; text-align: right;" id="tdRationalShowHide" runat="server"
											clientidmode="Static">
											<a href="javascript:" onclick="RationalShowHide(this);">Advanced</a>
										</td>--%>
									</tr>
								</table>
								<div id="DistractorHolder" style="padding-left: 40px; display: block; border-bottom-color: rgb(204, 204, 204);
									border-bottom-width: 1px; border-bottom-style: solid; table-layout: fixed; border-spacing: 2px;"
									class="tblContainer">
									<asp:Repeater runat="server" ID="rptrItemQuestions">
										<HeaderTemplate>
                                            
                      					</HeaderTemplate>
										<ItemTemplate>
                                              <div class="tblRow" >
												<!--class="response_letter tblCell_InternalQuestionLayout"-->
												<div style="width: 40px; float: left;">
													<input id="CorrectAnswer" onchange="assessmentitem_changeField(this, 'Correct', this.value);" name="<%# "A_" + _iItemID %>"
														value="<%# DataBinder.Eval(Container.DataItem, "ID")%>" type="radio" <%# (bool)(DataBinder.Eval(Container.DataItem, "Correct")) ? "checked='true'" : ""  %> /><span class="sort_Index"><%# DataBinder.Eval(Container.DataItem, "Letter")%>.</span>
												</div>
<%--												<div style="float: right;">
													<div class="SKEditableBody tblCell_InternalQuestionLayout answer_text" update="A_<%# DataBinder.Eval(Container.DataItem, "ID")%>"
														style="width: 800px; display: inline-block;">
														<div  id="Distractor_Text" class="SKEditableBodyText">
															<%# DataBinder.Eval(Container.DataItem, "DistractorText")%>
                                                    	</div>
													</div>
													<div>--%>
														<div id='DRHeader_<%# DataBinder.Eval(Container.DataItem, "ID")%>' style="display: none;
															width: 180px; text-align: right; height: 60px; float: left; padding-right: 5px;">
															Rationale:
                                                        </div>
														<div id='DRbody_<%# DataBinder.Eval(Container.DataItem, "ID")%>' class="SKEditableBody tblCell_InternalQuestionLayout answer_text"
															update="DR_<%# DataBinder.Eval(Container.DataItem, "ID")%>" style="display: inline-block;
															xposition: relative; xdisplay: none; left: 195px; width: 615px;">
															<div id="DistractorRationale_Text_<%# DataBinder.Eval(Container.DataItem, "ID")%>" class="SKEditableBodyText" contenteditable="true" style="text-align: left; width: 615px;">
																<%# DataBinder.Eval(Container.DataItem, "DistractorRationale")%>
                                                            </div>
														</div>
											</div>
<%--												</div>
											</div>--%>
										</ItemTemplate>
										<FooterTemplate>
                                          
										</FooterTemplate>
									</asp:Repeater>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</telerik:RadPageView>
	</telerik:RadMultiPage>
	<telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
		Skin="Thinkgate_Window" EnableEmbeddedSkins="false">
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
           <script type="text/javascript">
               var rigorCtrlName = 'combo<%= _Dok %>';

               function updateRigor(sender, args) {
                   var comboWebb = $find('comboWebb').get_value();
                   var comboRBT = $find('comboRBT').get_value();
                   var comboMarzano = $find('comboMarzano').get_value();

                   var context = getURLParm('qType');
                   try {
                       if (context == 'TestQuestion') {
                           assessment_changeField('Rigor', $find(rigorCtrlName).get_text());

                       } else {
                           var ustring = 'Webb=' + comboWebb + '|RBT=' + comboRBT + '|Marzano=' + comboMarzano + '|';
                           assessment_changeField('RigorString', ustring);
                       }
                   }
                   catch (e) {
                       errormsg();
                   }
               }
               function updateDifficultyIndex(sender, args) {
                   // 2012-08-16 DHB - Back end piece to update testQuestion record with new 
                   // difficulty index is expecting the text instead of the value.
                   //var comboDifficultyIndex = $find('comboDifficultyIndex').get_value();
                   var comboDifficultyIndex = $find('comboDifficultyIndex').get_text();

                   var context = getURLParm('qType');
                   try {
                       assessment_changeField('DifficultyIndex', comboDifficultyIndex);
                   }
                   catch (e) {
                       errormsg();
                   }
               }
               function errormsg() {
                   alert('There was an error saving your selection.');
               }
    </script>
</body>
</html>
