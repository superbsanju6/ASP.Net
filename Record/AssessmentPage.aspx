<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="AssessmentPage.aspx.cs"
	Inherits="Thinkgate.Record.AssessmentPage"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<head id="Head1" runat="server">
    <link href="~/Styles/AssessmentCSS.css" rel="stylesheet" type="text/css" />
    <script src="~/Scripts/GoogleAnalytics.js" type="text/javascript"></script>
	<style type="text/css">
		<asp:Literal id="cssProofed" runat="server" />
		<asp:Literal id="cssDocuments" runat="server" />
	</style>


 </head>
            <script type="text/javascript">
                function fadeMessage() {
                    $('#DeleteFormMessageLabel').fadeIn();
                    $('#DeleteFormMessageLabel').fadeOut(4000);
                };

                function setTitle(assessmentTitle) {
                    var winCustomDialog = getCurrentCustomDialog();
                    var hasSecure = $("#hiddenAccessSecureTesting").val().toLowerCase() == "true" ? true : false;
                    var isSecureflag = $("#hiddenIsSecuredFlag").val().toLowerCase() == "true" ? true : false;
                    var secureAssessment = $("#hiddenSecureType").val().toLowerCase() == "true" ? true : false;
                    if (hasSecure == true && isSecureflag == true) {
                        if (secureAssessment == true) {
                            assessmentTitle = SetSecureAssessmentTitle(assessmentTitle);
                        }
                    }
                    if (winCustomDialog) {
                        //if (winCustomDialog._titleElement.innerText == " ") {
                            //winCustomDialog._titleElement.innerText = assessmentTitle;
                        //}
                        winCustomDialog._titleElement.innerHTML = assessmentTitle;
                    }
                };
                $(document).ready(function () {
                    
                    $("#AssessmentDirectionsContent").on('change', function () {
                        //Do calculation and change value of other span2,span3 here
                        $('.noError').css({ 'style': 'display: none' })
                    });

                    $("#AdminAssessmentDirectionsContent").on('change', function () {
                        //Do calculation and change value of other span2,span3 here
                        $('.noError').css({ 'style': 'display: none' })
                    });

                    $('.noError').css({ 'style': 'display: none' })
                });               

            </script>
	
<body class="BODY" style="margin: 0px; overflow: hidden; background-color: white" >
  	<asp:PlaceHolder runat="server">
        <script type="text/javascript">
            function GetEquationEditorURL() {
                var _EquationEditorURL = '<%=ConfigurationManager.AppSettings["EquationEditorURL"].ToString() %>';
                return _EquationEditorURL;
            }
        </script>
   </asp:PlaceHolder>
  
   <!-- <div class="itemActions" style="width: 14px; height: 14px; position: relative; top: 2px; cursor: pointer; background-position: -46px 0px; width: 16px;" onclick="removeItem(this);" title="Remove Item"></div> -->
	<script id="bankItemTemplate" type="text/x-jsrender">
		<div class="thumbInstance" id="B{{:ID}}" StandardID="{{:StandardID}}">
			<span>
				<input style="float: left" type="checkbox"/>
				<img class="zoom_in_img" src="<%=ImageWebFolder%>commands/view_assessment_small.png" title="Click to zoom item" onclick="previewBankQuestion('{{:EncryptedID}}')"></img>
				{{if OnAnyTest}}
				<img class="flag_img" src="<%=ImageWebFolder%>commands/flag_blue.png" style="display: <%=selectedAssessment.Category == Thinkgate.Base.Enums.AssessmentCategories.Classroom ? "none;" : "block;" %>" title="Item already exists on proofed assessment"></img>
				{{/if}}
			</span>
			<div style="position: absolute; top: 0px; left: 20px; width: 100%; height: 165px; background-color: red; opacity: 0.01;  filter: alpha(opacity = 1); z-index: 10"></div>

			<img class="thumbImage" src='<%=ItemThumbnailWebPath_Content%>{{:ThumbnailName}}'
					alt="Item Thumbnail" onerror="onImgError(this);" onLoad="setDefaultImage(this);" style="z-index: 5"/>
			<span class="ItemStandard">
				<div><span>{{:StandardName}}</span>{{if Standards.length > 1}}<img class="standardPlus" src="<%=ImageWebFolder%>BtnExpandPlus.png" onclick="thumbnailStandardSelect(this)"></img>{{/if}}</div>
			</span>
		</div>
	</script>
	<script id="documentFormTemplate" type="text/x-jsrender">
		<tr>
            {{if FormName != "Field Test"}}
			<td>Form {{:FormName}}</td>
			    {{if AssessmentFile}}
				    <td class="assessmentRows AssessmentCell" align="center">
					    <img style="cursor: pointer" src="../Images/commands/view_assessment_small.png" onclick="show_uploaded_doc('../Upload/{{:AssessmentFile}}');"/>
				    </td>
				    <td class="assessmentRows AssessmentCell">&nbsp;</td>
				    <td class="assessmentRows AssessmentCell" align="center">
					    <img style="cursor: pointer" src="../Images/CrossRedNew.png" onclick="delete_uploaded_doc({{:FormId}}, 'AssessmentFile');"/>
				    </td>
			    {{else}}
				    <td class="assessmentRows AssessmentCell">&nbsp;</td>
				    <td class="assessmentRows AssessmentCell" align="center">
					    <img style="cursor: pointer" src="../Images/Upload.png" onclick="show_upload_frame({{:FormId}}, 'AssessmentFile');"/>
				    </td>
				    <td class="assessmentRows AssessmentCell">&nbsp;</td>
			    {{/if}}
			    {{if AnswerKeyFile}}
				    <td class="assessmentRows AssessmentCell" align="center">
					    <img style="cursor: pointer" src="../Images/commands/view_assessment_small.png" onclick="show_uploaded_doc('../Upload/{{:AnswerKeyFile}}');"/>
				    </td>
				    <td class="assessmentRows AssessmentCell">&nbsp;</td>
				    <td class="assessmentRows AssessmentCell" align="center">
					    <img style="cursor: pointer" src="../Images/CrossRedNew.png" onclick="delete_uploaded_doc({{:FormId}}, 'AnswerKeyFile');"/>
				    </td>
			    {{else}}
				    <td class="assessmentRows AssessmentCell">&nbsp;</td>
				    <td class="assessmentRows AssessmentCell" align="center">
					    <img style="cursor: pointer" src="../Images/Upload.png" onclick="show_upload_frame({{:FormId}}, 'AnswerKeyFile');"/>
				    </td>
				    <td class="assessmentRows AssessmentCell">&nbsp;</td>
			    {{/if}}
			    {{if ReviewerFile}}
				    <td class="assessmentRows ReviewerCell" align="center">
					    <img style="cursor: pointer" src="../Images/commands/view_assessment_small.png" onclick="show_uploaded_doc('../Upload/{{:ReviewerFile}}');"/>
				    </td>
				    <td class="assessmentRows ReviewerCell">&nbsp;</td>
				    <td class="assessmentRows ReviewerCell" align="center">
					    <img class="ReviewEdit" style="cursor: pointer" src="../Images/CrossRedNew.png" onclick="delete_uploaded_doc({{:FormId}}, 'ReviewerFile');"/>&nbsp;
				    </td>
			    {{else}}
				    <td class="assessmentRows ReviewerCell">&nbsp;</td>
				    <td class="assessmentRows ReviewerCell" align="center">
					    <img class="ReviewEdit" style="cursor: pointer" src="../Images/Upload.png" onclick="show_upload_frame({{:FormId}}, 'ReviewerFile');"/>&nbsp;
				    </td>
				    <td class="assessmentRows ReviewerCell">&nbsp;</td>
			    {{/if}}
            {{/if}}
		</tr>
	</script>
	<script id="questionTemplate" type="text/x-jsrender">
		<div class="questionInstance fieldTestItem_{{:FieldTest}} " id="{{:ID}}" style="padding: 2px" NewlyAdded={{:NewlyAdded}} SharedContentID={{:SharedContentID}} IsCopyAccess={{:~VerifyIBCopyAccess(ID, SharedContentID)}} IsEditAccess={{:~VerifyIBEditAccess(ID, SharedContentID)}}>
			<div class="tblContainer" style="border: 1px solid #B7DEFF; width: 100%; table-layout: fixed">
				<div class="tblRow">
					<div class="tblCell_InternalQuestionLayout" style="width: 140px">
					Item <span class="sort_number"></span> Instructions:</div>
										
					<div update="D" class="SKEditableBody tblCell_InternalQuestionLayout" style="overflow: hidden;">
                        {{* if (!AssessmentIsProofed ) { }}  
						<div class="SKEditableBodyText" id="D_{{:ID}}" contenteditable="true" style="">{{:Directions||'<%=ItemDefaultDirections %>'}}</div>
                        {{* } else { }}
                        <div class="SKEditableBodyText" id="D_{{:ID}}" style="">{{:Directions||'<%=ItemDefaultDirections %>'}}</div>
                        {{* } }}
             		</div>                

					<div class="tblCell_InternalQuestionLayout" style="padding-left: 10px; width: 135px;">
						{{* if (ContentType != 'External' && !AssessmentIsProofed ) { }}
						<div class="itemActions" style="background-position: -33px 0px; width: 13px;" onclick="EraseItem(this)" title="Erase Item"></div>
						<div class="itemActions displayNoneFT{{:FieldTest}}" style="background-position: -22px 0px; width: 10px;" onclick="AutoReplace(this);" title="Auto Replace"></div>
						<div class="itemActions displayNoneFT{{:FieldTest}}" style="background-position: -80px 0px; width: 12px;" onclick="ManualReplace(this);" title="Manual Replace"></div>
						<div class="itemActions" style="width: 21px;" onclick="itemOnlinePreview(this);" title="Online Preview Item"></div>
						{{* } else if (ContentType != 'External') { }}
						<div class="itemActions" style="background-position: -62px 0px; width: 17px;" onclick="expandItem(this);" title="Expand Item"></div>
						<div class="itemActions" style="width: 21px;" onclick="itemOnlinePreview(this);" title="Online Preview Item"></div>
						{{* } }}
						{{* if (ContentType != 'External' && !AssessmentIsProofed) { }}
							{{if ~VerifyIBCopyAccess(ID, SharedContentID)}} 
								<div class="itemActions" style="position: relative; left: 1px; background-position: -92px 0px; width: 19px;" onclick="advancedItemEdit(this);" title="Advanced Item Edit"></div>
							{{/if}}
						{{* } }}
                		 {{* if (ContentType == 'External' && !AssessmentIsProofed) { }}
							{{if ~VerifyIBCopyAccess(ID, SharedContentID)}} 
								<div class="itemActions" style="position: relative; left: 1px; background-position: -92px 0px; width: 19px;" onclick="advancedItemEditExternal(this);" title="Advanced External Item Edit"></div>
							{{/if}}
						{{* } }}
					</div>                     
				</div>
			</div>
			{{if Addendum}}
			<div class="tblContainer AddendumRow">                
				<div class="tblRow">                                                           
					<div class="tblLeft">
						Addendum: <a href="" onclick="addendum_display({{:ID}}); return false;" >{{:Addendum.Addendum_Name}}</a>
            {{if ShowAddendumIcon}} 
              <img class="SortImg" id="sortorder_{{:ID}}"  title="Use default sort order for this addendum" src="../Images/Items_Sort.png" name="SortIcon" style="width: 15px; height: 16px; cursor: pointer;" onclick="CheckDefaultSortOrder({{:ID}}); return false;" />
            {{/if}}
					</div>
				</div>
			</div>
			{{/if}}
			{{if ScoreType == "R" && Rubric && ((RubricType == "B" && (RubricPoints > 0 || RubricID > 0)) || (RubricType == "A" && RubricID > 0 )) }}
			<div class="tblContainer RubricRow">                
				<div class="tblRow">                                                           
					<div class="tblLeft">
						Rubric: <a href="" onclick="rubric_display('{{:EncryptedID}}',{{:ID}}); return false;" >{{:Rubric.Name}}</a>
                        {{* if (ContentType != 'External') { }} 
                        
                             {{* if (AssessmentIsProofed) { }}  
                                {{if (DisplayRubric) }}
								    <input type="checkbox" name="chkRubric" checked="checked" disabled="disabled"                            
                                        onchange="assessmentitem_changeField(this, 'DisplayRubric', this.value);"/> Display Rubric on assessment
							    {{else}}
								    <input type="checkbox" name="chkRubric" disabled="disabled"  
                                        onchange="assessmentitem_changeField(this, 'DisplayRubric', this.value);"/> Display Rubric on assessment
							    {{/if}}                                                                          
                               
                            {{* } else { }}
                                {{if (DisplayRubric) }}
								    <input type="checkbox" name="chkRubric" checked="checked"                             
                                        onchange="assessmentitem_changeField(this, 'DisplayRubric', this.value);"/> Display Rubric on assessment
							    {{else}}
								    <input type="checkbox" name="chkRubric"  
                                        onchange="assessmentitem_changeField(this, 'DisplayRubric', this.value);"/> Display Rubric on assessment
							    {{/if}}
                            {{* } }}

                        {{* } }}
					</div>
				</div>
			</div>
			{{/if}}  
			<div class="tblContainer" style="clear: left; table-layout: fixed; position: relative; ">                
				<div class="tblRow">                                                           
					<div class="tblLeft" style="position: relative;">
						{{*
						if (!AssessmentIsProofed ){
						}}
                            
							<img class="visibilityHideFT{{:FieldTest}}" style="position: relative; top: 2px; cursor: pointer;" onclick="removeItem(this);" title="Remove Item" src="../Images/CrossRedNew.png"/>
                        {{if IsAnchorItem}}
                        <img alt="Anchor Item" class="visibilityHideFT{{:FieldTest}}" style="position: relative; top: 2px; width: 15px;" title="Anchor Item" src="../Images/anchor_icon.png"/>
                        {{/if}}
						{{*
						} else {
						}}
							<div style="width: 30px"></div>
							<div style="text-align: center; width: 30px; position: absolute; top: 0px; left: 0px">
								<a class="visibilityHideFT{{:FieldTest}}" href="" onclick="toggleScoreOnTest(this); return false">
									{{if ScoreOnTest == 'Yes'}}
										Score
									{{else}}
										Do Not Score
									{{/if}}
								</a>
							
						{{if IsAnchorItem}}
                        <img alt="Anchor Item" class="visibilityHideFT{{:FieldTest}}" style="position: relative; top: 2px; width: 15px;" title="Anchor Item" src="../Images/anchor_icon.png"/>
                        {{/if}}

			                 </div>
                        
                        {{* } }}
					</div>
														
					<div class="sort_number item_number_div tblCell_InternalQuestionLayout" style="padding-left: 10px;">{{:Sort}}.</div>
										
					{{* if (ContentType != 'External') { }}
					<div update="Q" class="SKEditableBody tblCell_InternalQuestionLayout">
                        {{* if (!AssessmentIsProofed ) { }}  
						<div class="SKEditableBodyText" id="Q_{{:ID}}" contenteditable="true">{{:Question_Text}}</div>
                        {{* } else { }}
                        <div class="SKEditableBodyText" id="Q_{{:ID}}">{{:Question_Text}}</div>
                        {{* } }}
					</div>
					{{* } }}
				</div>
			</div>
			<div class="tblContainer" style="padding-left: 40px; border-spacing: 2px; table-layout: fixed; border-bottom: 1px solid #CCCCCC"> 
				<!-- start child repeater -->
				<div class="tblRow showIfProofed">
					<div class="tblCell" style="border-bottom: 1px dashed black"></div>
					<div class="tblCell"></div>
				</div>
				{{for Responses}}
					<div class="tblRow">
						<div class="response_letter tblCell_InternalQuestionLayout">
							<input type="radio" name="A_{{:#parent.parent.data.ID}}" value="{{:ID}}" {{if (Correct)}} checked {{/if}} onchange="assessmentitem_changeField(this, 'Correct', this.value);"/>
							{{:Letter}}.
						</div>
						{{* if (ContentType != 'External') { }}
						<div update="A_{{:ID}}" class="SKEditableBody tblCell_InternalQuestionLayout answer_text">
                            {{* if (!AssessmentIsProofed ) { }}  
							<div class="SKEditableBodyText" id="A_{{:#parent.parent.data.ID}}_{{:ID}}" contenteditable="true" >{{:DistractorText}}</div>
                            {{* } else { }}
                            <div class="SKEditableBodyText" id="A_{{:#parent.parent.data.ID}}_{{:ID}}">{{:DistractorText}}</div>
                            {{* } }}
						</div> 
						{{* } }}
					</div>
				{{/for}}
				<div class="tblRow showIfProofed">
					<div class="tblCell" style="border-top: 1px dashed black"></div>
					<div class="tblCell"></div>
				</div>
				<!-- end child repeater -->
			</div>
			<div class="tblContainer" style="width: 100%">
				<div class="tblRow">
					<div class="tblCell" style="white-space: nowrap">
						<span class="itemFieldName borderIfProofed">Standard:
						{{if ~VerifyIBCopyAccess(ID, SharedContentID)}}
						 <div style="width: 240px;" onclick="comboPlaceholder_OnClick(this);" attachToCombo="comboStandard" class="RadComboBox RadComboBox_Web20" value="{{:StandardID}}" text="{{:StandardName}}">
							<table onmouseleave="$(this).removeClass('rcbHovered')" onmouseenter="$(this).addClass('rcbHovered')" style="border-width: 0px; width: 100%; border-collapse: collapse; table-layout: fixed;">
								<tbody>
									<tr class="rcbReadOnly">
										<td style="width: 100%; margin-top: -1px; margin-bottom: -1px;" class="rcbInputCell rcbInputCellLeft">
											<input style="text-align: left !important; display: block;" class="rcbInput" readOnly="readonly" value="{{:StandardName}}" title="{{:StandardName}}" type="text" autocomplete="off">
										</td>
										<td style="margin-top: -1px; margin-bottom: -1px;" class="rcbArrowCell rcbArrowCellRight">
											<a style="overflow: hidden; display: block; position: relative;">select</a>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
						{{else}}
							{{:StandardName}}
						{{/if}}
						</span>
                    <div style="margin-top: 7px; text-align: left; vertical-align: middle" value="{{:StandardID}}" text="{{:StandardName}}">
                        <asp:HyperLink ID="hlkStandardSearch" style="cursor: pointer; margin-left: 3px;" runat="server" Text="Search" onclick="ShowStandardSearch({{:ID}});" ForeColor="Blue"></asp:HyperLink>
                    </div>                           
					</div>
					<div class="tblCell" style="white-space: nowrap">
						<span class="itemFieldName">{{_dok/}}:</span> 
						{{if ~VerifyIBCopyAccess(ID, SharedContentID)}}
						<div class="RadComboBox RadComboBox_Web20" style="width:110px;" field="Rigor" value="{{:Rigor.Text}}" text="{{:Rigor.Text}}" onclick="if (AssessmentIsProofed) {return false;} comboPlaceholder_OnClick(this);" attachToCombo="comboRigor">
							<table onmouseleave="$(this).removeClass('rcbHovered')" onmouseenter="if(!AssessmentIsProofed) $(this).addClass('rcbHovered')" style="border-width:0;border-collapse:collapse;table-layout:fixed;width:100%">
								<tr class="rcbReadOnly">
									<td style="margin-top:-1px;margin-bottom:-1px; width:100%;" class="disabled_dropdown_placeholder rcbInputCell rcbInputCellLeft"><input type="text" class="rcbInput" value="{{:Rigor.Text}}" style="display: block;" readonly="readonly" title="Rigor selection" /></td>
									<td style="margin-top:-1px;margin-bottom:-1px;" class="disabled_dropdown_placeholder rcbArrowCell rcbArrowCellRight"><a style="overflow: hidden;display: block;position: relative;outline: none;">select</a></td>
								</tr>
							</table>
						</div>
						{{else}}
							{{:Rigor.Text}}
						{{/if}}
					</div>
					<div class="tblCell" style="white-space: nowrap">
						<span class="itemFieldName">Weight:</span> 
						<div class="RadComboBox RadComboBox_Web20 " style="width:70px;" field="ItemWeight" value="{{:ItemWeight.Text}}" text="{{:ItemWeight.Text}}" onclick="if (AssessmentIsProofed || {{:FieldTest}}) {return false;} comboPlaceholder_OnClick(this);" attachToCombo="comboItemWeight">
							<table onmouseleave="$(this).removeClass('rcbHovered')" onmouseenter="if(!AssessmentIsProofed && !({{:FieldTest}})) $(this).addClass('rcbHovered')" style="border-width:0;border-collapse:collapse;table-layout:fixed;width:100%">
								<tr class="rcbReadOnly">
									<td style="margin-top:-1px;margin-bottom:-1px;width:100%;" class="disabled_dropdown_placeholderFT{{:FieldTest}} disabled_dropdown_placeholder rcbInputCell rcbInputCellLeft"><input type="text" class="rcbInput RadComboPlaceHolder" value="{{:ItemWeight.Text}}" style="display: block;" readonly="readonly" title="Item weight selection" /></td>
									<td style="margin-top:-1px;margin-bottom:-1px;" class="disabled_dropdown_placeholderFT{{:FieldTest}} disabled_dropdown_placeholder rcbArrowCell rcbArrowCellRight"><a style="overflow: hidden;display: block;position: relative;outline: none;">select</a></td>
								</tr>
							</table>
						</div>
												  
						<span class="WeightPCT">( {{:~format_dec(Weight*100, 2)}}% )</span>
					</div>
					<div class="tblCell" style="white-space: nowrap">
						<span class="itemFieldName">Difficulty Index:</span>
						{{if ~VerifyIBCopyAccess(ID, SharedContentID)}}
						<div class="RadComboBox RadComboBox_Web20" style="width:80px;" field="DifficultyIndex" value="{{:DifficultyIndex.Text}}" text="{{:DifficultyIndex.Text}}" onclick="if (AssessmentIsProofed) {return false;} comboPlaceholder_OnClick(this);" attachToCombo="comboDifficultyIndex">
							<table onmouseleave="$(this).removeClass('rcbHovered')" onmouseenter="if(!AssessmentIsProofed) $(this).addClass('rcbHovered')" style="border-width:0;border-collapse:collapse;table-layout:fixed;width:100%">
								<tr class="rcbReadOnly">
									<td style="margin-top:-1px;margin-bottom:-1px;width:100%;" class="disabled_dropdown_placeholder rcbInputCell rcbInputCellLeft"><input type="text" class="rcbInput" value="{{:DifficultyIndex.Text}}" style="display: block;" readonly="readonly" title="Change difficulty for item" /></td>
									<td style="margin-top:-1px;margin-bottom:-1px;" class="disabled_dropdown_placeholder rcbArrowCell rcbArrowCellRight"><a style="overflow: hidden;display: block;position: relative;outline: none;">select</a></td>
								</tr>
							</table>
						</div>   
						{{else}}
							{{:DifficultyIndex.Text}}
						{{/if}}                                        
					</div>
				</div>
			</div>                          
		</div>                            
    </script>
	<div id="proof_dialog" style="display: none">
		<div class="rwDialogPopup radalert" style="margin: 0px; padding-bottom: 0px">
			<div style="color: red; font-weight: bold; font-size: 14pt; text-align: center; position: relative; left: -20px">Attention!</div><br/>
			<div>Selecting the Proof Assessment option signifies that the assessment is complete and it prevents other changes.<br/><br/>Select 'Proof Assessment' to enable printing and to approve this assessment</div>
			<hr/>
		</div>
	</div>
    <div id="itemChange_dialog" runat="server" style="display: none;"></div>
    <form id="form2" runat="server">
	<input id="lbl_TestID" type="hidden" runat="server" />
	<input id="lbl_OTCUrl" type="hidden" runat="server" />
    <input id="lbl_TestCategory" type="hidden" runat="server"/>
     <input id="lbl_TestType" type="hidden" runat="server"/>
    <input id="lbl_ImagesUrl" type="hidden" runat="server"/>   

    <input runat="server" type="hidden" id="hiddenAccessSecureTesting" clientidmode="Static" name="hiddenAccessSecureTesting" value="" />
    <input runat="server" type="hidden" id="hiddenIsSecuredFlag" clientidmode="Static" name="hiddenIsSecuredFlag" value="" />
    <input runat="server" type="hidden" id="hiddenSecureType" clientidmode="Static" name="hiddenSecureType" value="" />
        
	<%--Start: Bug#:252 and 247 , 18 Dec 2012: Jeetendra Kumar, When a keyword search is executed from an assessment you cannot end the search.--%>
	<telerik:RadScriptManager ID="RadScriptManager2" runat="server" AsyncPostBackTimeout="36000">
	<%--End: Bug#:252 and 247 , 18 Dec 2012: Jeetendra Kumar, When a keyword search is executed from an assessment you cannot end the search.--%>
		<Scripts>
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
			<asp:ScriptReference Path="~/scripts/master.js" />
			<asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
			<asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
			<asp:ScriptReference Path="~/scripts/jquery.ui.touch-punch.js" />            
            <asp:ScriptReference Path="~/Scripts/jsrender.js"/>
			<asp:ScriptReference Path="~/Scripts/AssessmentEdit.js?j=2" />
            <asp:ScriptReference Path="~/Scripts/ckeditor/ckeditor.js"/>
            <asp:ScriptReference Path="~/Scripts/ckeditor/adapters/jquery.js"/>
            <asp:ScriptReference Path="~/Scripts/InlineEditor.js"/>
            <asp:ScriptReference Path="~/Scripts/thinkgate.util.js"/>
		</Scripts>
		<Services>
		    <asp:ServiceReference Path="~/Services/Service2.svc" />
		</Services>
	</telerik:RadScriptManager>
        <telerik:RadScriptBlock runat="server" ID="disableAutoInline">
            <script type="text/javascript">
                if (CKEDITOR != 'undefined') {
                    CKEDITOR.disableAutoInline = true;
                }
            </script>
        </telerik:RadScriptBlock>
        <telerik:RadScriptBlock runat="server" ID="mathjaxLoad" Visible="False">
            
            <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$appSettings:MathJaxUrl%>' />/MathJax.js?config=TeX-AMS-MML_HTMLorMML,<asp:Literal runat='server' Text='<%$appSettings:MathJaxUrl%>' />/config/local/thinkgate-e3.js"></script>

        </telerik:RadScriptBlock>
	<telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false" Skin="Thinkgate_Window" EnableEmbeddedSkins="false">
			
		</telerik:RadWindowManager>
	<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
	</telerik:RadAjaxManager>
	<telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
	</telerik:RadSkinManager>
	<telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" ClientIDMode="Static"/>
	
	<div style="display: none">
		<div id="editorToolHolder"></div>
	</div>
	<div class="SKEditorWrapper" style="">
	    
	</div>

	<asp:Image runat="server" ID="undo_inactive" ImageUrl="~/Images/Toolbars/undo_inactive.png" style="display: none"/>
	<asp:Image runat="server" ID="undo_active" ImageUrl="~/Images/Toolbars/undo.png" style="display: none"/>        
        
	<div id="topPortion">
		<div id="toolbar_div" style="background-color: #DFE9F5">
			<telerik:RadToolBar
				ID="mainToolBar" runat="server"
				style="z-index:90001;" Width="722px" OnClientLoad="mainToolBar_OnClientLoad"
				Skin="Sitefinity" EnableRoundedCorners="true" EnableShadows="true" OnClientButtonClicked="mainToolBar_OnClientButtonClicked">
				<Items>
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/undo_inactive.png" ToolTip="Undo" Value="Undo" />
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/delete.png" ToolTip="Delete Assessment" Value="DeleteAssessment" />
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/copy_assessment.png" ToolTip="Copy Assessment" Value="Copy"/>
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/preview.png" ToolTip="Online Preview Assessment" Value="OnlinePreview" />
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/print.png" ToolTip="Print" Value="Print"/>
                    <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/H_button.png" ToolTip="Print HTML" Value="HTML" runat="server" ClientIDMode="Static" id="rtbBtnHtml"  />
                    <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/word_button.png" ToolTip="Print Word" Value="Word" runat="server" ClientIDMode="Static" id="rtbBtnWord" />
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/LrmiTags.png" ToolTip="Tags" Value="Tags" runat="server" ClientIDMode="Static" id="rtbBtnLRMI" />
                    <telerik:RadToolBarButton runat="server" IsSeparator="True" Value="sep"/> 
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/configure.png" ToolTip="Configure Assessment" Value="Configure"/>
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/reorder.png" ToolTip="Reorder Items" Value="Reorder"/>
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/proof.png" ProofedImageUrl="~/Images/Toolbars/proof_green.png" ToolTip="Proof Assessment" Value="Proof"/>
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/Scheduler.png" ToolTip="Schedule Assessment" Value="Scheduler" runat="server" ClientIDMode="Static" id="rtbBtnScheduler" />
				</Items>
			</telerik:RadToolBar>
			<!--<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/generate_assessment.png" ToolTip="Assessment Generator" Value="Generate"/>-->
					<!--<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/format.png" ToolTip="Format" Value="Format" style="display: none"/>
					<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/header_footer.png" ToolTip="Header/Footer" Value="Header" style="display: none"/>-->
			 <telerik:RadButton runat="server" ID="btnSummary" ClientIDMode="Static" Skin="Web20" style="float: right; margin-top: 10px; margin-right: 10px"
							Text="  Summary  " Width="100px" AutoPostBack="false" CssClass="radDropdownBtn"
							OnClientClicked="showSummaryScreen" />

			
			
		</div>
		<div id="standardDropdownHolder" style="display: none">
	
			<asp:Image CssClass="flag_img flag_yellow" ID="flag_yellow_template" runat="server" ImageUrl="~/Images/commands/flag_yellow.png" ToolTip="Item already added to this assessment" ClientIDMode="Static"/>
														
			<telerik:RadComboBox ID="comboStandard" runat="server" Width="240" Skin="Web20" CssClass="radDropdownBtn"
					ShowToggleImage="True" Style="vertical-align: middle;" OnClientDropDownOpened="comboStandardCommon_OnOpened"
					DropDownWidth="245" OnClientDropDownClosed="comboShared_OnClosed" AutoPostBack="False" ChildTreeName="CommonStandardTree">
				<ItemTemplate>
					<div id="div1">
						<telerik:RadTreeView ID="CommonStandardTree" runat="server" DataTextField="StandardName" DataValueField="StandardID" DataFieldID="StandardID" DataFieldParentID="ParentID"
								OnClientNodeClicking="treeItemStandardCommonClick_PlaceholderVersion" OnNodeDataBound="CommonStandardTree_OnNodeDataBound" ClientIDMode="Static" Height="300" EnableViewState="False">
							<DataBindings>
								<telerik:RadTreeNodeBinding Expanded="True" />
							</DataBindings>
						</telerik:RadTreeView>
					</div>
				</ItemTemplate>
				<Items>
					<telerik:RadComboBoxItem Text="Search Standards" />
				</Items>
			</telerik:RadComboBox>
			<telerik:RadComboBox ID="comboRigor" runat="server" Skin="Web20" CssClass="radDropdownBtn" Width="110"
					ShowToggleImage="True" Style="vertical-align: middle;" AutoPostBack="False" OnClientDropDownClosed="comboShared_OnClosed" OnClientSelectedIndexChanging="comboShared_OnIndexChanged" >
			</telerik:RadComboBox>
			<telerik:RadComboBox ID="comboItemWeight" runat="server" Skin="Web20" CssClass="radDropdownBtn" Width="70"
					ShowToggleImage="True" Style="vertical-align: middle;" AutoPostBack="False" OnClientDropDownClosed="comboShared_OnClosed" OnClientSelectedIndexChanging="comboShared_OnIndexChanged" >
			</telerik:RadComboBox>
			<telerik:RadComboBox ID="comboDifficultyIndex" runat="server" Skin="Web20" CssClass="radDropdownBtn" Width="80"
					ShowToggleImage="True" Style="vertical-align: middle;" AutoPostBack="False" OnClientDropDownClosed="comboShared_OnClosed" OnClientSelectedIndexChanging="comboShared_OnIndexChanged" >
			</telerik:RadComboBox>		
		</div>
	<div ID="DeleteFormMessageDiv" style="position: absolute;top: 15px;right: 120px;">
        <asp:Label ID="DeleteFormMessageLabel" runat="server" Font-Italic="True" style="display:none" ForeColor="#CC0000" Text="Form is successfully removed and remaining tabs are re-numbered."></asp:Label><br />
    </div>

		<telerik:RadTabStrip runat="server" ID="mainTabStrip"  Orientation="HorizontalTop" OnClientLoad="mainTabStrip_OnClientLoad"
				SelectedIndex="0" MultiPageID="RadMultiPageAssessmentEdit" Skin="Thinkgate_Blue" OnClientTabSelecting="mainTabStrip_OnClientTabSelecting" style="" EnableEmbeddedSkins="False" ClientIDMode="Static">
				<Tabs>
					<telerik:RadTab Text="Edit" Value="Edit">
					</telerik:RadTab>
					<telerik:RadTab Text="Preview" Value="Preview">
					</telerik:RadTab>
					<telerik:RadTab Text="Documents" Value="Documents">
					</telerik:RadTab>
				</Tabs>
			</telerik:RadTabStrip>
			<div style=" position: absolute;top: 43px;right: 0px;">
			
				<telerik:RadTabStrip runat="server" ID="radtab_FieldTest" Orientation="HorizontalTop" OnClientLoad="radtab_FieldTest_OnClientLoad"
					 Skin="Thinkgate_Blue" OnClientTabSelecting="formTabStrip_OnClientTabSelecting" EnableEmbeddedSkins="False"
					DataTextField="FormName" DataValueField="FormId" Height="25" Style="float: right; " ClientIDMode="Static" Visible="False">
					<Tabs>
						<telerik:RadTab Text="Field Test" Value="1000">
						</telerik:RadTab>
					</Tabs>
				</telerik:RadTabStrip>
				<asp:Image ID="imgAddNewForm" runat="server" ImageUrl="~/Images/commands/add_new.png" ToolTip="Add New Form"/>
				<telerik:RadAjaxPanel ID="ajaxPanel_FormTab" runat="server">
					<telerik:RadTabStrip runat="server" ID="radtab_Forms" Orientation="HorizontalTop" 
						SelectedIndex="0" Skin="Thinkgate_Blue" OnClientTabSelecting="formTabStrip_OnClientTabSelecting"  EnableEmbeddedSkins="False"
						DataTextField="FormName" DataValueField="FormId" ScrollButtonsPosition="Right" Height="25" EnableViewState="False"
						ClientIDMode="Static" Style="float: right" PerTabScrolling="True" OnClientLoad="RefreshFormTabs_callback">
					</telerik:RadTabStrip>
					<asp:Button ID="btn_AddNewForm" runat="server" OnClick="AddNewForm" ClientIDMode="Static" Style="visibility: hidden"/>
					<asp:Button ID="btn_DeleteForm" runat="server" OnClick="DeleteForm" ClientIDMode="Static" Style="visibility: hidden"/>
					<asp:TextBox ID="txt_TargetForm" runat="server" ClientIDMode="Static" Style="visibility: hidden;" />
                    <input id="formCount" type="hidden" runat="server"/>
				</telerik:RadAjaxPanel>
			</div>
		</div>
		
		<telerik:RadMultiPage runat="server" ID="RadMultiPageAssessmentEdit" SelectedIndex="0" CssClass="multiPage" ClientIDMode="Static">
		<telerik:RadPageView runat="server" ID="mpAssessmentEdit">
	
		  <div id="editContentContainer" class="tblContainer" style="vertical-align: top; height: 450px; overflow: hidden;"> 
			<div class="tblRow" style="padding: 3px; vertical-align: top; height: 100%;">
				<div id="leftHandSide" class="tblLeft" style="width: 100%; text-align: left; vertical-align: top; height: 100%;">
					
					<div class="headingArea" style="overflow-y: scroll;">
						<div class="headingToggle" style="position: relative">
							<div class="headingToggleImg headingToggleImgOff"></div>
							Heading and Instructions
							<div id="divItemCount" style="text-align: center;">
                                <div id="divAddNewItem" style="float: left;">
                                  <asp:Label runat="server" Text="Item Count:"></asp:Label> 
                                    <asp:Label runat="server" ID="lblItemCount"></asp:Label>
                                    <asp:TextBox ID="txtAddNumItems" ClientIDMode="Static" style="margin-left: 10px;" MaxLength="3" runat="server" Width="25" onkeypress="return isNumberKey(event)" ></asp:TextBox>
                                    <asp:Image ID="imgAddNewItem" style="margin-top: 2px; float: none" runat="server" ImageUrl="~/Images/commands/add_new.png" OnClientClick="AddBlankItems" ToolTip="Add Item"/>
                                </div>&nbsp;&nbsp;&nbsp;&nbsp;
							</div>		
						</div>
						<div class="tblContainer" style="table-layout: fixed; margin: 4px; margin-left: 10px;margin-right: 20px; width: 100%;">
							<div class="tblRow" style="">
								<div class="tblLeft" style="width: 100px; vertical-align: top;">
									Student&nbsp;Name:
								</div>
								<div class="tblRight" style="">
									<div style="width: 250px; border-bottom: 1px solid black;">&nbsp;</div>
								</div>
							</div>
							<div class="tblRow">
								<div class="tblLeft" style="width: 50px;">
									Teacher:
								</div>
								<div class="tblRight" style="">
									<div style="width: 250px; border-bottom: 1px solid black;">&nbsp;</div>
								</div>
							</div>
							<div class="tblRow">
								<div class="tblLeft" style="width: 50px;">
									Test:
								</div>
								<div class="tblRight" style="width: 100px;">
									<asp:Label runat="server" ID="lblTestName" />
								</div>
							</div>
							<div class="tblRow">
								<div class="tblLeft" style="width: 50px;">
									Description:
								</div>
								<div class="tblRight" style="width: 100px;">
									<asp:Label runat="server" ID="lblDescription" />
								</div>
							</div>
							<div class="tblRow">
								<div class="tblLeft" style="width: 50px;">
									Form:
								</div>
								<div class="tblRight" style="width: 100px;">
									<div id="lblForm"></div><br/>
								</div>
							</div>
                           <br/>						
                            <div class="tblRow" style="padding-right:10px;">
								<div class="tblLeft" style="width: 50px;">
									Administration Instructions:
								</div>
								<div ID="AdminAssessmentDirectionCellReadOnly" runat="server" Visible="False">
									<asp:Label runat="server" ID="AdminAssessmentDirectionCellReadOnlyLabel" />
								</div>
								<div class="tblRight" ID="AdminAssessmentDirectionsCell" style="" runat="server">
									<div id="AdminAssessmentDirectionsEditor" contenteditable="true">
									    <asp:Label runat="server" ID="AdminAssessmentDirectionsContent" ></asp:Label>
									</div>
								</div>                              
							</div>
                            <div class="tblRow">
								<div class="tblLeft" style="width: 50px; height: 15px">
								</div>
								<div class="tblRight" style="width: 100px;">
									<div id="lblMarginSpace"></div>
								</div>
							</div>
                            <div class="tblRow" style="padding-right:10px;">
								<div class="tblLeft" style="width: 200px;">
									Student Instructions:
								</div>
								<div ID="AssessmentDirectionCellReadOnly" runat="server" Visible="False">
									<asp:Label runat="server" ID="AssessmentDirectionCellReadOnlyLabel" />
								</div>
                              
								<div class="tblRight" ID="AssessmentDirectionsCell" style="" runat="server">
									<div id="AssessmentDirectionsEditor" contenteditable="true">
									    <asp:Label runat="server" ID="AssessmentDirectionsContent" ></asp:Label>
									</div>
								</div>
							</div>
						</div>
					</div>
					<div id="scrollUpHelper">Drag Here to Scroll Up</div>
					<div id="QuestionHolder" onscroll="createCkeditorOnScroll();" style="width: 500px; padding-left: 2px; overflow-y: scroll; overflow-x: auto; height: 100%; position: relative; border-bottom: 1px solid black">
						<telerik:RadAjaxPanel ID="ajaxPanel_Items" runat="server" >
							<div id="testQuestions"></div>
							<asp:Button ID="btn_bankToTestDrag" runat="server" OnClick="AddItemsFromBank" ClientIDMode="Static" Style="visibility: hidden;" />
							<asp:Button ID="btn_RefreshQuestionsPanel" runat="server" OnClick="RefreshQuestionsPanel" ClientIDMode="Static" Style="visibility: hidden;" />
                            <asp:Button ID="btn_AddBlankItem" runat="server" OnClick="AddBlankItems" ClientIDMode="Static" Style="visibility: hidden"/>
							<asp:TextBox ID="txt_QuestionIDsToAdd" runat="server" ClientIDMode="Static" Style="visibility: hidden;" />
							<asp:TextBox ID="txt_QuestionSort" runat="server" ClientIDMode="Static" Style="visibility: hidden;" />
							<asp:TextBox ID="txt_ActiveForm" runat="server" ClientIDMode="Static" Style="visibility: hidden;" />
							<asp:TextBox ID="txt_AssessmentForms" runat="server" ClientIDMode="Static" Style="visibility: hidden;" />
							
							<div runat="server" id="AddendumDivs" style="display: none" EnableViewState="False">
							</div>
							<div runat="server" id="RubricDivs" style="display: none" EnableViewState="False">
							</div>                        
						</telerik:RadAjaxPanel>                                                
						
					</div>
					<div id="scrollDownHelper">Drag Here to Scroll Down</div>
					
					
				</div>
				<div ID="bankSlider" class="tblMiddle" style="padding-left: 0px; background-color: #CCCCCC; vertical-align: middle; height: 100%" runat="server">
					<div id="bankSliderHandle"><asp:Image runat="server" id="bankSliderImg" style="position: relative; left: 1px; top: 40px ; width: 8px" ImageUrl="~/Images/arrow_gray_right.gif" opened_src="arrow_gray_right.gif" closed_src="arrow_gray_left.gif"/></div>
				</div>
				<div ID="bankColumn" class="tblRight" style="padding-left: 0px; border-left: 1px solid #CCCCCC; border-top: 1px solid #CCCCCC; height: 100%" runat="server">
					<div id="bankHolder" style="width: 245px; height: 100%; background-color: #848484; position: relative;">
						<div style="z-index: 4; position: absolute; top: 0px; left: 0px; width: 100%; height: 24px; background-color: #3D3D3D">
							<telerik:RadTabStrip runat="server" ID="radtab_SearchOptions" Orientation="HorizontalTop" OnClientLoad="radtab_SearchOptions_OnClientLoad"
								SelectedIndex="0" MultiPageID="rmpBankTabs" Skin="Thinkgate_Blue" EnableEmbeddedSkins="False" Width="218">
								<Tabs>
									<telerik:RadTab Text="Outline">
									</telerik:RadTab>
									<telerik:RadTab Text="Filter">
									</telerik:RadTab>
									<telerik:RadTab Text="Keyword">
									</telerik:RadTab>
								</Tabs>
							</telerik:RadTabStrip>
							<img src="../Images/new/expand.png" style="z-index: 6; float: left; margin-left: 5px; margin-top: 4px; cursor: pointer" onclick="itemSearch();"/>
						</div>
						
						<telerik:RadMultiPage runat="server" ID="rmpBankTabs" SelectedIndex="0" CssClass="multiPage" Height="100%">
							<telerik:RadPageView runat="server" ID="OutlineTab" style="position: absolute; height: 100%; top: 25px">
								<telerik:RadAjaxPanel ID="ajaxPanel_Bank" runat="server">
									 <div id="bankHeadingArea" style="height: 38px">
										<telerik:RadComboBox ID="OutlineStandardSet" runat="server" ToolTip="Standard Set Selection" 
											Skin="Web20" Width="90"         
											OnSelectedIndexChanged="OutlineStandardSet_OnChange" AutoPostBack="true" CssClass="radDropdownBtn"
											CausesValidation="False"  OnClientSelectedIndexChanging="bankReloadStart" HighlightTemplatedItems="true" DataTextField="Standard_Set" DataValueField="Standard_Set">
											<ItemTemplate>
												<span style="text-align: center"><%# Eval("Standard_Set")%></span>
											</ItemTemplate>
										</telerik:RadComboBox>								
										
										<telerik:RadComboBox ID="comboStandardOutline" runat="server" Width="150" Skin="Web20" CssClass="radDropdownBtn"
												ShowToggleImage="True" Style="vertical-align: middle;" OnClientDropDownOpened="comboWithNestedTree_OnOpened" 
												 ClientIDMode="Static" DropDownWidth="230" ChildTreeName="OutlineStandardTree">
											<ItemTemplate>
												<div id="div1">
													<telerik:RadTreeView ID="OutlineStandardTree" runat="server" ParentComboBox="comboStandardOutline" DataTextField="StandardNameWithCount" DataValueField="StandardID" DataFieldID="StandardID" DataFieldParentID="ParentID"
															 Height="400" OnClientNodeClicking="bankReloadStart" OnNodeClick="OutlineStandardTree_OnClick" OnNodeDataBound="OutlineStandardTree_OnNodeDataBound" ClientIDMode="Static" EnableViewState="True">
														<DataBindings>
															<telerik:RadTreeNodeBinding Expanded="True" />
														</DataBindings>
													</telerik:RadTreeView>
												</div>
											</ItemTemplate>
											<Items>
												<telerik:RadComboBoxItem Text="Search Standards" />
											</Items>
										</telerik:RadComboBox>                                       
										
										<asp:Label runat="server" ID="lblBankCount"></asp:Label>
									</div>
									<div id="bankScroller" class="bankScroller">
										<div id="bankQuestions">
										</div>
									</div>
								</telerik:RadAjaxPanel>
							</telerik:RadPageView>
							<telerik:RadPageView runat="server" ID="FilterTab" style="position: absolute; height: 100%; top: 25px;">
								<telerik:RadAjaxPanel ID="ajaxPanel_Bank_filter" runat="server">
									 <div id="bankHeadingArea_filter" style="height: 38px">
										<telerik:RadComboBox ID="comboStandardFilter" runat="server" Width="244" Skin="Web20" CssClass="radDropdownBtn" AutoPostBack="True"
												ShowToggleImage="True" Style="vertical-align: middle;" OnSelectedIndexChanged="comboStandardFilter_OnSelectedIndexChanged"
													 ClientIDMode="Static" OnClientSelectedIndexChanging="bankReloadStart" DropDownWidth="230" DataTextField="FullStandardName" DataValueField="StandardID" EmptyMessage="Search Standards" >
										</telerik:RadComboBox>      
										<asp:Label runat="server" ID="lblBankCount_filter"></asp:Label>                         
									 </div>
									 <div id="bankScroller_filter" class="bankScroller">
										<div id="bankQuestions_filter">
										</div>
									 </div>
								</telerik:RadAjaxPanel>
							</telerik:RadPageView>
							<telerik:RadPageView runat="server" ID="KeywordTab" style="position: absolute; height: 100%; top: 25px">
								<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
									 <div id="bankHeadingArea_keyword" style="height: 22px; background-color: white">
								
										<telerik:RadTextBox Width="220" ID="txtKeyword" runat="server" BorderColor="white" EmptyMessage="&#60; Enter text to search for items &#62;"
											 AutoPostBack="False">
										</telerik:RadTextBox>
										<asp:Image runat="server" onclick="KeywordSearch()" ImageUrl="~/Images/go.png" CssClass="keywordSearchButton"/>
										<asp:Button ID="btn_KeywordSearch" runat="server" OnClick="KeywordSearch" ClientIDMode="Static" Style="height: 0px; display: none;" />
									</div>
									<asp:Label runat="server" ID="lblBankCount_keyword"></asp:Label>
									<div id="bankScroller_keyword" class="bankScroller">
										<div id="bankQuestions_keyword">
										</div>
									</div>
								</telerik:RadAjaxPanel>
							</telerik:RadPageView>
						</telerik:RadMultiPage>
					</div>
				</div>
				<div class="tblRight rightHandToolbar" style="">
					<div id="rightHandToolbar" style="width: 1px; overflow: hidden">
						<img src="../Images/new/expand.png" style="float: left; margin-bottom: 4px;  margin-left: 4px; margin-top: 4px; cursor: pointer" onclick="itemSearch();"/>
						<asp:Image ID="Image3" CssClass="rightHandTabButton" ImageUrl="~/Images/tab_outline_vertical.png" runat="server" tab_name="Outline" style="width: 25px;"/>
						<asp:Image ID="Image4" CssClass="rightHandTabButton" ImageUrl="~/Images/tab_filter_vertical.png" runat="server" tab_name="Filter" style="width: 25px; position: relative; top:-4px"/>
						<asp:Image ID="Image5" CssClass="rightHandTabButton" ImageUrl="~/Images/tab_keyword_vertical.png" runat="server" tab_name="Keyword" style="width: 25px; position: relative; top:-9px"/>
					</div>
				</div>
			</div>
		</div>
			
		</telerik:RadPageView>
		<telerik:RadPageView runat="server" ID="mpPreview" style="height: 600px">            
			<iframe id="iframe_preview" runat="server" style="width: 100%; height: 100%" ></iframe>                
		</telerik:RadPageView>
		<telerik:RadPageView runat="server" ID="mpDocuments" style="height: 600px">
			<div id="documentTabText">
				<div class="tblContainer">
					<div class="tblRow">
						<div class="tblLeft" style="vertical-align: middle; width: 60px; text-align: left">
							<img src="../Images/alert.png"/>
						</div>
						<div class="tblLeft">
							The uploaded assessment form is for sharing the External assessment and may be viewed by students who take the test online in Elements<sup>TM</sup>.Take care that uploaded documents match the Answer Key content in Elements<sup>TM</sup> to ensure integrity of the results.
						</div>
					</div>
				</div>
			</div>
			<telerik:RadAjaxPanel ID="rapDocumentsTable" runat="server" LoadingPanelID="updPanelLoadingPanel">            
				<div id="documentTableHolder" runat="server">
					<table id="documentTable" cellspacing="0">
						<tr>
							<td>Documents:</td>
							<td colspan="3" class="assessmentHeaders AssessmentCell">Assessment</td>
							<td colspan="3" class="answerKeyHeaders AssessmentCell">AnswerKey</td>
							<td colspan="3" class="answerKeyHeaders ReviewerCell">Review Version</td>
						</tr>
						<tr>
							<td>
								Assessment Forms
							</td>
							<td class="assessmentHeaders AssessmentCell">
								Preview
							</td>
							<td class="assessmentHeaders AssessmentCell">
								Upload
							</td>
							<td class="assessmentHeaders AssessmentCell">
								Delete
							</td>
							<td class="answerKeyHeaders AssessmentCell">
								Preview
							</td>
							<td class="answerKeyHeaders AssessmentCell">
								Upload
							</td>
							<td class="answerKeyHeaders AssessmentCell">
								Delete
							</td>
							<td class="answerKeyHeaders ReviewerCell">
								Preview
							</td>
							<td class="answerKeyHeaders ReviewerCell">
								Upload
							</td>
							<td class="answerKeyHeaders ReviewerCell">
								Delete
							</td>
						</tr>
						<tbody id="DocumentsTableHolder">
				
						</tbody>    
					</table>
					<div class="chkReview"><asp:CheckBox ID="chkDisplayReview" runat="server" onclick="toggle_review_display(this);"/>Display Review Version Documents</div>
				</div>
				<asp:TextBox ID="FormToDelete" runat="server" ClientIDMode="Static" Style="visibility: hidden;" />
				<asp:TextBox ID="TypeToDelete" runat="server" ClientIDMode="Static" Style="visibility: hidden;" />
				<asp:Button ID="btn_RefreshDocumentsTable" runat="server" OnClick="RefreshDocumentsTable" ClientIDMode="Static" Style="visibility: hidden;" />
				<asp:Button ID="btn_DeleteDocument" runat="server" OnClick="DeleteDocument" ClientIDMode="Static" Style="visibility: hidden;" />
			</telerik:RadAjaxPanel>
			<div id="uploadForm" style="display: none">
				<hr/>
				<div class="center" style="width: 200px; font-weight: bold; font-size: 12pt">Upload Document</div>
				<div class="center" style="width: 90%; margin-top: 15px">You must honor all copyright protection notifications on all material used with Elements<sup>TM</sup>.
				 Only use content (Items, Distractors, Images, Addendums, and Assessments) that your school system has purchased the rights to reproduce or that has been marked as public domain.
				 You are responsible for any copyright infringements. If in doubt about the content you wish to use, contact your central office for permission or clarification.</div>
				 <div class="center" style="width: 600px">
					 <iframe id="uploadFrame" clientidmode="Static" src="" frameborder="0" scrolling="no" width="100%" style="margin-top: 15px; ">
					 </iframe>
				 </div>
			</div>

			
		</telerik:RadPageView>
		
		</telerik:RadMultiPage>
		 
		
	</form>
</body>
</html>
