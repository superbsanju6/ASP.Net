<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentEditor_Rubric.aspx.cs"
	Inherits="Thinkgate.Controls.Assessment.ContentEditor.ContentEditor_Rubric" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<%--This height of 100% helps elements fill the page--%>
<head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=8" />
	<meta http-equiv="PRAGMA" content="NO-CACHE" />
	<meta http-equiv="Expires" content="-1" />
	<meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
	<link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
	<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
	<link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4"
		rel="stylesheet" type="text/css" runat="server" />
	<link href="~/Thinkgate_Blue/TabStrip.Thinkgate_Blue.css?v=2" rel="stylesheet" type="text/css" />
	<title id="RubricTitle" runat="server"></title>
	<base target="_self" />
	<telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
	</telerik:RadStyleSheetManager>
	<script type='text/javascript' src='../../../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../../../Scripts/jquery-migrate-1.1.0.min.js'></script>
	<script type='text/javascript' src='../../../Scripts/jquery.scrollTo.js'></script>
	<script type="text/javascript">        var $j = jQuery.noConflict();</script>
	<style>
		.bottomTextButton
		{
			vertical-align: middle;
		}
		.previewRubricDirections
		{
			width: 100%;
			height: 100%;
			margin: auto;
			/*text-align: center;*/
			vertical-align: middle;
			overflow: auto;
		}
		.previewRubricContent
		{
			width: 100%;
			height: 100%;
			margin: auto;
			/*text-align: center;*/
			vertical-align: middle;
			overflow: auto;
		}
		table.ItemBankList td
		{
			width: 100%;
			padding: 10px;
		}
		.divUploadTemplate
		{
			background-color: White;
			width: 100%;
			height: 100%;
		}
		.center
		{
			text-align: center;
			width: 100%;
			margin: auto;
		}
		.IdentificationSubHeader
		{
			font-style: italic;
			color: Gray;
			line-height: 12px;
		}
		.ImageUploader
		{
			text-align: center;
			background-color: White;
			width: 100%;
			white-space: nowrap;
			font-weight: bold;
		}
		.LegalMessage
		{
			text-align: center;
			background-color: White;
			height: 100%;
			line-height: 12px;
		}
		.IconHeader
		{
			xheight: 35px;
			width: 100%;
			background-color: rgb(222, 226, 231);
		}
		
		.rpFirst .rpTemplate
		{
			position: relative;
			font-size: 0;
			line-height: 0;
		}
		
		div.RadPanelBar
		{
			margin: 0 auto 0;
			xmargin: 20px auto 0;
		}
		
		.linksPanel
		{
			xposition: absolute;
			right: 30px;
			bottom: 30px;
			width: 200px;
			height: 30px;
			text-align: right;
		}
		
		.RadPanelBar .rpFirst .rpTemplate
		{
			zoom: 1;
			background: url('Images/tstore.jpg') no-repeat 0 0;
			xheight: 94px;
			width: 100%;
		}
		
		.linksPanel a
		{
			color: White;
		}
		
		/* styling of template contents */
		.rpTemplate .productList
		{
			padding: 0;
			width: 662px;
			margin: 0 auto;
			display: block;
		}
		
		.rpTemplate .productList:after
		{
			content: ".";
			display: block;
			visibility: hidden;
			font-size: 0;
			line-height: 0;
			overflow: hidden;
			height: 0;
			clear: both;
		}
		
		.rpTemplate .productList li
		{
			list-style-type: none;
			float: left;
			text-align: center;
			width: 220px;
			margin: 15px 0 0;
			padding: 0;
			border-right: 1px solid #bec7cb;
		}
		
		.rpTemplate .productList li.last
		{
			border-right-width: 0;
		}
		
		.rpTemplate .productList li div
		{
			font: normal 12px Arial,sans-serif;
			text-decoration: underline;
			color: #333;
		}
		
		div.RadPanelBar .rpTemplate
		{
			background: #EDF9FE;
			overflow: hidden !important;
			height: 100%;
		}
		.rpHeaderTemplate
		{
			overflow: hidden;
			/*height: 26px;*/
		}
		.RadPanelItemHeader
		{
			padding-left: 5px;
		}
		.CriteriaPoints
		{
			width: 30px;
			height: 25px;
			border: 1px solid;
			cursor: pointer;
		}
		.CriteriaPointsSelected
		{
			width: 30px;
			height: 25px;
			border: 1px solid;
			background-color: #FFA500;
			cursor: pointer;
		}
		.tblCPS
		{
			width: 100%;
			border-spacing: 5px;
			border-collapse: separate;
		}
		.tblCPSHeaderX
		{
			text-align: center;
			font-weight: bold;
			padding-bottom: 0px;
			height: 10px;
			line-height: 10px;
			vertical-align: bottom;
		}
		.tblCPSHeaderXTitle
		{
			text-align: center;
			font-weight: bold;
			padding-bottom: 5px;
			height: 10px;
			line-height: 10px;
			vertical-align: bottom;
			letter-spacing: 5px;
		}
		.tblCPSHeaderY
		{
			width: 10px;
			font-weight: bold;
			padding-right: 0px;
			text-align: center;
		}
		.tblCPSHeaderYTitle
		{
			width: 10px;
			font-weight: bold;
			padding-right: 0px;
			text-align: center;
			vertical-align: middle;
		}
		.rubricItemB_div
		{
			width: 100%;
			xtext-align: center;
			xpadding: 20px;
		}
		.rubricItemB_table
		{
			width: 90%;
			height: 90%;
			overflow: hidden;
			margin: auto;
			margin-top: 10px;
			margin-bottom: 10px;
		}
		.rubricItemB_td
		{
			background-color: lightblue;
			border: solid 1px;
			font-weight: bold;
		}
		.riDescription
		{
			width: 850px;
		}
		.riRowNum
		{
			width: 20px;
		}
		.riPreview
		{
			width: 18px;
		}
		.riBTD
		{
			border: solid 1px;
			height: 100px;
		}
		.riPreviewIcon
		{
			display: block; 
			vertical-align: top;
			position: relative;
			top: -30%;
		}
		.riEditIcon
		{
			display: block; 
			vertical-align: bottom;
			position: relative;
			top: 30%
		}
		.riBoldText
		{
			font-weight: bold;
		}
		.rubricItemBPointDescriptionDiv
		{
			margin: 3px;
			border-style: none;
			height: 100%;
			cursor: pointer;
			min-height: 198px;
			min-width: 198px;
			padding: 3px;
			xmax-height:700px;
			max-width:700px;
			overflow:hidden;            
		}
		.riATD
		{
			border: solid 1px;
			height: 100px;
		}
		.rubricItemA_td
		{
			background-color: lightblue;
			border: solid 1px;
			font-weight: bold;
			/* min-width: 50px;*/
		}
		.rubricItemAPointDescriptionDiv
		{
			margin: 3px;
			border-style: none;
			height: 100%;
			cursor: pointer;
			min-height: 198px;
			min-width: 198px;
			padding: 3px;
			max-height:198px;
			max-width:300px;
			overflow:hidden;
		}
		.centerText
		{
			text-align: center;
		}
		.riCriteria
		{
			padding-right: 20px;
		}
		.riSpacer
		{
			min-width: 25px;
		}
		.wordwrap
		{
			white-space: pre-wrap; /* CSS3 */
			white-space: -moz-pre-wrap; /* Firefox */
			white-space: -pre-wrap; /* Opera <7 */
			white-space: -o-pre-wrap; /* Opera 7 */
			word-wrap: break-word; /* IE */
		}
	</style>
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
	<form id="form_rubric" runat="server" style="min-width: 1000px;">
	<div runat="server" id="RubricItem" style="display: none;">
	</div>
	<input runat="server" id="ContentEditor_Rubric_hdnRubricID" clientidmode="Static" type="hidden"/>
	<telerik:RadScriptManager ID="RadScriptManager1" runat="server">
		<Scripts>
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
			<asp:ScriptReference Path="~/Scripts/master.js?d=2" />
			<asp:ScriptReference Path="~/Scripts/master.js" />
            <asp:ScriptReference Path="~/Scripts/jsrender.js" />
            <asp:ScriptReference Path="~/Scripts/ckeditor/ckeditor.js"/>
            <asp:ScriptReference Path="~/Scripts/ckeditor/adapters/jquery.js"/>
		</Scripts>
		<Services>
			<asp:ServiceReference Path="~/Services/Service2.svc" />
		</Services>
	</telerik:RadScriptManager>
	<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
		<script type="text/javascript">
			var $ = $telerik.$;
			var uploadsInProgress = 0;
			var cursorPosition = null;
			var rubricContentNeedsRefreshing = false;
			var panelBar;
			var panelBarProductsTab;
			var multiPage;
			var rowCount = 0;
			String.prototype.trimString = function () {
				return this.replace(/^\s+|\s+$/g, "");
			};

			function onFileSelected(sender, args) {
				if (!uploadsInProgress)
					$("#SaveButton").attr("disabled", "disabled");

				uploadsInProgress++;
			}

			function onFileUploaded(sender, args) {
				decrementUploadsInProgress();
			}

			function onUploadFailed(sender, args) {
				decrementUploadsInProgress();
			}

			function decrementUploadsInProgress() {
				uploadsInProgress--;

				if (!uploadsInProgress)
					$("#SaveButton").removeAttr("disabled");
			}
			//<![CDATA[
			function validationFailed(sender, eventArgs) {
				$(".ErrorHolder").append("<p>Validation failed for '" + eventArgs.get_fileName() + "'.</p>").fadeIn("slow");
			}
			//]]>

			function mainToolBar_OnClientButtonClicked(sender, args) {
				var selected_button_value = args.get_item().get_value();
				switch (selected_button_value) {
					case "DeleteRubric":
						customDialog({ title: "Delete Rubric?", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "You are about to delete this Rubric. Do you wish to continue?<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel" }, { title: "Ok", callback: deleteRubric_confirmCallback}]);
						break;
					default:
						alert('Function is in development.');
				}
			}
			function deleteRubric_confirmCallback() {
				var ID = document.getElementById('RubricItem').innerHTML;
				Service2.DeleteRubricFromDatabase(ID, function (result_raw) {
					if (result_raw.length > 0) {
						alert('Cannot delete a rubric that is being used on another question.');
					}
					else {
						closeCurrentCustomDialog();

					}
				}, deleteRubric_onFailure);

			}
			function deleteRubric_onSuccess(result_raw) {
				alert(result_raw);
			}
			function deleteRubric_onFailure() {

			}

			//            function previewPDF() {
			//                var $iframe_preview = $('#iframe_preview');
			//                var divIID = document.getElementById('RubricItem').innerHTML.trimString();
			//                $('#iframe_preview').attr('height', '600px');
			//                $('#iframe_preview').attr('src', appclient + 'Record/RenderRubricAsPDF.aspx?xID=' + divIID, 'print');
			//            }

			function mainTabStrip_OnClientTabSelecting(sender, args) {
				var selected_tab_value = args.get_tab().get_value();
				if (selected_tab_value == 'Preview') {

					var previewRubricDirections = $('#ContentEditor_Rubric_Preview_Directions')[0];
					var previewRubricContent = $('#ContentEditor_Rubric_Preview_Contents')[0];
					if (rubricContentNeedsRefreshing) {
						var divIID = document.getElementById('RubricItem').innerHTML.trimString();
						Service2.RubricGetDirectionsAndContentFormatted(divIID, function (result_raw) {
							var parsedResults = jQuery.parseJSON(result_raw);
							previewRubricDirections.innerHTML = parsedResults.PayLoad[0];
							previewRubricContent.innerHTML = parsedResults.PayLoad[1];
						}, function () {
							alert("Error retrieving and displaying updated rubric content.  Please contact system administrator.");
						});
						rubricContentNeedsRefreshing = false;
					}
					//previewRubric.style.height = '600px';
					//previewPDF();
					//$('#iframe_preview').attr('height', '600px');
				}
			}

			function updateItemBanks(obj) {
				//debugger;
			    var checks = $j("[type='checkbox'][checked='true'][targetType='5'].itemBankUpdate");
			    var inBank = $j("[type='checkbox'][inBank='true'][targetType='5'].itemBankUpdate");
				var itemBanks = '';

				if (obj.getAttribute('targetType') == '5') {
					if (obj.checked) {
						if (obj.getAttribute('multiBanks') == 'false') {
							for (var i = 0; i < itemBankList.length; i++) {
								document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = (itemBankList[i][0] == '5' && obj.getAttribute('target') == itemBankList[i][1]) ? true : false;
								document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = (itemBankList[i][0] == '5') ? false : true;
							}
						} else {
							for (var i = 0; i < itemBankList.length; i++) {
								if (itemBankList[i][0] != '5') {
									document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = false;
									document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = true;
								} else {
									if (itemBankList[i][3] == 'false') {
										document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = false;
									} else {
										if (itemBankList[i][2] == 'true' || obj.getAttribute('target') == itemBankList[i][1]) {
											document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = true;
										}
									}
								}
							}
						}
					} else {
						for (var i = 0; i < itemBankList.length; i++) {
							if (checks.length != 0 || inBank.length != 0) {
								if (itemBankList[i][0] < '5') {
									document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = false;
									document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = true;
								}
								else {
									if (checks.length < 1) {
										document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = (itemBankList[i][2] == 'true') ? true : false;
									}
								}
							} else {
								document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = (itemBankList[i][2] == 'true') ? true : false;
								document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = (itemBankList[i][0] == 2) ? true : false;
							}
						}
					}
				}

				for (var i = 0; i < itemBankList.length; i++) {
					itemBanks += (document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked) ? ',' + itemBankList[i][0] + ':' + itemBankList[i][1] : '';
				}
				if (itemBanks != '') {
					//alert(itemBanks.substring(1));
					Rubric_changeItemBank();
				}
			}


			function Rubric_changeItemBank() {
				var updatestring = '';
				var iBank = $j("[type='checkbox'][cbtype='ItemBank']");
				for (var x = 0; x < iBank.length; x++) {
					if (iBank[x].checked) {
						updatestring += '' + iBank[x].getAttribute('TargetType') + '@';
						updatestring += '' + iBank[x].getAttribute('Target') + '|';
					}

				}
				updatestring += ''
				if (updatestring.length > 5) {
					//alert(updatestring);
					var ID = document.getElementById('RubricItem').innerHTML;
					Service2.UpdateItem_ItemBank(ID, 4, updatestring, UpdateRubric_ItemBank_onSuccess, UpdateRubric_ItemBank_onFailure);
				}
			}

			function UpdateRubric_ItemBank_onSuccess() {

			}
			function UpdateRubric_ItemBank_onFailure() {

			}

			function Rubric_changeField_copyright(sender, args) {
				var value = sender._element.attributes['cbvalue'].value;
				if (value == 'Yes') {
					//Show Source and credit
					document.getElementById('trSource').style.display = '';
					document.getElementById('trCredit').style.display = '';
					document.getElementById('trExpiryDate').style.display = '';
				}
				else {
					//Hide Source and credit
					document.getElementById('trSource').style.display = 'none';
					document.getElementById('trCredit').style.display = 'none';
					document.getElementById('trExpiryDate').style.display = 'none';
				}
				Rubric_changeField(sender, args);
			}

			function Rubric_changeField_Type(sender, args) {

				var value = args.get_item().get_value();
				if (value == 'B') {
					document.getElementById('trRubricScoring').style.display = 'table-row';
					document.getElementById('trAnalyticalCriteriaPointsSelection').style.display = 'none';
				}
				else {
					document.getElementById('trRubricScoring').style.display = 'none';
					document.getElementById('trAnalyticalCriteriaPointsSelection').style.display = 'table-row';
				}
				Rubric_changeField(sender, args);

			}
			
		
			function Rubric_changeField(sender, args) {
				var field = sender._element.attributes['field'].value;
				var value = '';
				var text = '';
				if (sender._element.nodeName == "INPUT")
					value = args.get_newValue();
				else if (field == 'Copyright')
					value = sender._element.attributes['cbvalue'].value;
				else {
					value = args.get_item().get_value();
					text = args.get_item().get_text();
				}

				var ID = document.getElementById('RubricItem').innerHTML;
				Service2.RubricUpdateField(ID, field, value,
					function (result_raw) {
						result = jQuery.parseJSON(result_raw);
						if (result.StatusCode > 0) {
							Rubric_changeField_event_onFailure(null, result.Message);
							return;
						}
						if (result.ExecOnReturn) {
							eval(result.ExecOnReturn);
						}
						if (field == 'Type' || field == 'MaxPoints') {
							refreshRubricContent();
						}
						if (field == 'Name') {
							document.getElementById('lblNameEdit').innerHTML = value;
							document.getElementById('lblNamePreview').innerHTML = value;
						}
						if (field == 'Type') {
							document.getElementById('lblTypeEdit').innerHTML = text;
							document.getElementById('lblTypePreview').innerHTML = text;

							if (text == "Analytical") {
								$("#IdentificationPanel_Scoring")[0].style.display = "none";
								$("#PreviewPanel_Scoring")[0].style.display = "none";
								$("#lblTypeEdit")[0].innerHTML += " " + $('#txtCriteria')[0].value + " x " + $('#cmbScoring')[0].control.get_value();
								$("#lblTypePreview")[0].innerHTML = $("#lblTypeEdit")[0].innerHTML;
							} else {
								document.getElementById("IdentificationPanel_Scoring").style.display = "inline";
								document.getElementById("PreviewPanel_Scoring").style.display = "inline";
							}
						}
						if (field == 'MaxPoints') {
							document.getElementById('lblScoringEdit').innerHTML = value + ' Point';
							document.getElementById('lblScoringPreview').innerHTML = value + ' Point';
							document.getElementById('txtPoints').value = value;
						}
					}, Rubric_changeField_event_onFailure);
			}
			function Rubric_changeField_event_onFailure() {

			}

			function Rubric_changeFieldValue(field, value) {
				var ID = document.getElementById('RubricItem').innerHTML;
				Service2.RubricUpdateField(ID, field, value,
					function (result_raw) {
						result = jQuery.parseJSON(result_raw);
						if (result.StatusCode > 0) {
							Rubric_changeField_event_onFailure(null, result.Message);
							return;
						}
						if (result.ExecOnReturn) {
							eval(result.ExecOnReturn);
						}
						if (field == 'Type' || field == 'MaxPoints') {
							refreshRubricContent();
						}
					}, Rubric_changeField_event_onFailure);
			}

			function Rubric_changeFieldValuePandC(p, c) {
				var ID = document.getElementById('RubricItem').innerHTML;

				$("#lblTypeEdit")[0].innerHTML = "Analytical " + c.toString() + " x " + p.toString();
				$("#lblTypePreview")[0].innerHTML = $("#lblTypeEdit")[0].innerHTML;

				Service2.RubricUpdatePointsAndCriteria(ID, p, c,
					function (result_raw) {
						result = jQuery.parseJSON(result_raw);
						if (result.StatusCode > 0) {
							Rubric_changeField_event_onFailure(null, result.Message);
							return;
						}
						refreshRubricContent();
					}, Rubric_changeField_event_onFailure);
			}


			function getURLParm(name) {
				name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
				var regexS = "[\\?&]" + name + "=([^&#]*)";
				var regex = new RegExp(regexS);
				var results = regex.exec(window.location.href);
				if (results == null)
					return "";
				else
					return decodeURIComponent(results[1].replace(/\+/g, " "));
			}

			function OpenRubricItemCount() {
				if (document.getElementById('lblItemCount').innerHTML == '0')
					return false;
				var ID = document.getElementById('RubricItem').innerHTML;
				customDialog({ url: (appClient + 'Controls/Assessment/ContentEditor/ContentEditor_Rubric_Questions.aspx?xID=' + ID), autoSize: true, name: 'ContentEditorRubricQuestions', title: 'Items Found'});
			}


			function SendResultsToWindow() {
              
				var RubricType = $find('cmbType').get_value();
				var RubricIDEncrypted = document.getElementById('RubricItem').innerHTML.trimString();
				var RubricPoints = document.getElementById('txtPoints').value;
				var CriteriaCount = $("#txtCriteria")[0].value;
				var RubricID = $("#ContentEditor_Rubric_hdnRubricID")[0].value;
				var RubricName = $.trim($("#txtName").val());
				if (RubricName == "") {
				    RubricValidationMessage();
				    return false;
				}
				var DialogueToSendResultsTo;
				DialogueToSendResultsTo = parent.getCurrentCustomDialogByName(getURLParm('NewAndReturnType'));
				try {
					DialogueToSendResultsTo.get_contentFrame().contentWindow.InsertRubric(RubricIDEncrypted, RubricName, RubricType, RubricPoints, CriteriaCount, RubricID);
				}
				catch (e) {
					try {
						parent.InsertRubric(RubricIDEncrypted, RubricName, RubricType, RubricPoints, CriteriaCount, RubricID);
					}
					catch (e) {

					}
				}

				closeCurrentCustomDialog();
			}


			function buildCriteriaPointsSelection() {
				var criteria = document.getElementById('txtCriteria').value;
				var points = document.getElementById('txtPoints').value;
				var sTable = '<table class="tblCPS">';
				var xMax = 10;
				var yMax = 10;
				sTable += '<tr>';
				sTable += '<td colspan="2"></td><td class="tblCPSHeaderXTitle" colspan="' + (xMax + 1) + '"><img src="../../../Images/arrow_left_10.png" border="0" style="padding-right:10px;">POINTS<img src="../../../Images/arrow_right_10.png" border="0" style="padding-left:10px;"></td></tr>';

				//------Start Header numbers
				sTable += '<tr>';
				sTable += '<td class="tblCPSHeaderX" colspan="2"></td>';
				for (var h = 0; h <= xMax; h++) {
					sTable += '<td class="tblCPSHeaderX">' + h + '</td>';
				}
				sTable += '</tr>';
				//------End Header

				for (var y = 1; y <= yMax; y++) {
					sTable += '<tr>';
					if (y == 1)
						sTable += '<td class="tblCPSHeaderYTitle" rowspan="' + (yMax) + '"><img src="../../../Images/arrow_up_10.png" border="0" style="padding-bottom:10px;">C<br/>R<br/>I<br/>T</br>E</br>R</br>I</br>A<img src="../../../Images/arrow_down_10.png" border="0" style="padding-top:10px;"></td>';
					sTable += '<td class="tblCPSHeaderY">' + y + '</td>';

					for (var x = 0; x <= xMax; x++) {
						sTable += '<td x="' + x + '" y="' + y + '"';
						if (x <= points && y <= criteria)
							sTable += ' class="CriteriaPointsSelected"';
						else
							sTable += ' class="CriteriaPoints"';


						if (x < 2 || y < 2) sTable += ' title="Rubric must have a minimum of 2 criteria and 2 points." '; 
						else  sTable += ' onclick="PointCriteriaSelection(this);"';

						sTable += ' >&nbsp;</td>';
					}
					sTable += '</tr>';
				}
				sTable += '</table>';
				document.getElementById('divCriteriaPointsSelector').innerHTML = sTable;
			}

			function PointCriteriaSelection(el) {
				var c = document.getElementById('txtCriteria').value;
				var p = document.getElementById('txtPoints').value;
				var x = el.getAttribute('x');
				var y = el.getAttribute('y');
				document.getElementById('txtCriteria').value = y;
				document.getElementById('txtPoints').value = x;

				buildCriteriaPointsSelection();
				if (c != y || p != x) {
					Rubric_changeFieldValuePandC(x, y);
				}
			}

			function CriteriaUpdate() {
				var y = document.getElementById('txtCriteria').value;
				if ((y < 2) || (y > 10) || (isNaN(y))) {
					document.getElementById('txtCriteria').value = 2;
				}
				buildCriteriaPointsSelection();
				Rubric_changeFieldValuePandC(document.getElementById('txtPoints').value, document.getElementById('txtCriteria').value);
			}

			function PointsUpdate() {

				var x = document.getElementById('txtPoints').value;
				if ((x < 2) || (x > 10) || (isNaN(x))) {
					document.getElementById('txtPoints').value = 2;
				}
				buildCriteriaPointsSelection();
				Rubric_changeFieldValuePandC(document.getElementById('txtPoints').value, document.getElementById('txtCriteria').value);
			}

			function mainToolBar_OnClientButtonClicked(sender, args) {
				var selected_button_value = args.get_item().get_value();
				switch (selected_button_value) {
					case "DeleteRubric":
						customDialog({ title: "Delete Rubric?", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "You are about to delete this Rubric. Do you wish to continue?<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel" }, { title: "Ok", callback: deleteRubric_confirmCallback}]);
						break;
					case "PrintItem":
						OpenPrintWindow();
						break;
					case "AddendumInsert":
						InsertAddendum(6, 'The Donkey and the Load of Salt', 'passage', 'Folktale');
						break;
					case "OnlinePreview":
						onLinePreviewClick();
						break;
					case "CopyItem":
						CopyItem();
						break;
					default:
						alert('Function is in development.');
				}
			}

			function RubricTypeMessage() {
			    customDialog({ title: "Rubric Type", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "An Analytical rubric identifies and assesses components of a finished product.<br/><br/> A Holistic rubric assesses student work as a whole.", dialog_style: 'confirm' }, [{ title: "Ok" }]);
			}
			function CopyrightMessage() {
			    customDialog({ title: "Copyright", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "You must honor all copyright protection notifications on all material used within Elements&trade;. Only use content (Items, Distractors, Images, Addendums, and Assessments) that your school system has purchased the rights to reproduce or that has been marked as public domain. You are responsible for any copyright infringements. If in doubt about the content you wish to use, contact your central office for permission clarification.", dialog_style: 'confirm' }, [{ title: "Ok" }]);
			}

			function RubricValidationMessage() {
			    customDialog({ title: "Rubric Validation", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "Please enter name.", dialog_style: 'confirm' }, [{ title: "Ok" }]);
			}

			function refreshRubricContent() {
				var ID = document.getElementById('RubricItem').innerHTML;
				rubricContentNeedsRefreshing = true;
				
				Service2.RubricGetRubricItems(ID,
					function (result_raw) {
						result = jQuery.parseJSON(result_raw);
						RubricItemArray[0] = result.PayLoad;
						var Template = $find('cmbType').get_value();
						renderData(RubricItemArray[0], Template);

					}, Rubric_changeField_event_onFailure);
			}


			function editRubricItem(el, sTitle) {
			    customDialog({ modal: true, url: (appClient + 'Controls/Assessment/ContentEditor/ContentEditor_RubricItem.aspx?xID=' + el), width: 600, height: 650, name: 'ContentEditorRubricItem_' + el, title: sTitle, destroyOnClose: true });
			}

			function riOnLinePreview(elemID, sTitle) {
				var element = $('#' + elemID);
				var content = element.html();
				customDialog({ maxheight: 400, maxwidth: 800, maximize: true, content: (content == 'Click to Edit') ? '' : content, title: sTitle });
			}

			function SaveRubricItem(id) {
				var RID = id.match('ri(.*)_')[1];
				var ID = document.getElementById('RubricItem').innerHTML.trimString();
				var ustring = '';
				var el = '';
				var points = document.getElementById('txtPoints').value;
				var ucolumn = '';
				if (id.indexOf('_name') > 0) {
					ucolumn = 'Name';
					ustring = document.getElementById(id).innerHTML.trimString();
				}
				else {
					ucolumn = 'ScoreDesc';
					for (var x = 0; x <= points; x++) {
						el = 'ri' + RID + '_' + x;
						ustring += (document.getElementById(el).innerHTML.trimString() == 'Click to Edit' ? '' : document.getElementById(el).innerHTML.trimString()) + '|';
					}
					//ustring = ustring.slice(0, -1);
				}
				Service2.RubricItemUpdateField(ID, RID, ucolumn, ustring,
					function (result_raw) {
						result = jQuery.parseJSON(result_raw);
						if (result.StatusCode > 0) {
							Rubric_changeField_event_onFailure(null, result.Message);
							return;
						}

					}, Rubric_changeField_event_onFailure);

				rubricContentNeedsRefreshing = true;
			}

			function onLoad(sender) {
				panelBar = sender;
				panelBarProductsTab = panelBar.get_items().getItem(1);
				//            multiPage = panelBar.get_items().getItem(1).findControl("RadMultiPage1");
			}

			function onItemClicked(sender, eventArgs) {
				if (!panelBarProductsTab.get_selected()) {
					panelBarProductsTab.expand();
					panelBarProductsTab.select();
				}

				var pageView = multiPage.get_pageViews().getPageView(
				eventArgs.get_item().get_index());

				pageView.set_selected(true);
			}


			function stopPropagation(e) {
				e.cancelBubble = true;

				if (e.stopPropagation)
					e.stopPropagation();
			}


			function addNewItem(itemType) {

				//parent.customDialog({ url: ('../../../Controls/Assessment/ContentEditor/ContentEditor_Image.aspx?xID=a0pFaDN2aHh0RVNINHRCVkxpY3J2QT09&NewAndReturnID=a3o5YVh2TGtiZnJtQkVSQkhQV2VsQT09&NewAndReturnType=Rubric'), autoSize: true, name: 'ContentEditorIMAGE' });

				parent.customDialog({ url: (appClient + 'Dialogues/AddNewItem.aspx?xID=' + itemType + '&NewAndReturnID=' + document.getElementById('RubricItem').innerHTML + '&NewAndReturnType=ContentEditorRubric'), autoSize: true, name: 'NewItem', title: ('Add New ' + itemType) });

			}

			function SearchItem(itemType) {

			    parent.customDialog({ url: (appClient + 'Controls/Images/ImageSearch_Expandedv2.aspx?NewAndReturnType=ContentEditorRubric&ShowExpiredItems=No'), maxwidth: 900, maximize_width: true, name: ('Search' + itemType), title: ('Search ' + itemType), destroyOnClose: true });
			}

			var modalWin = parent.$find('RadWindow1Url');

			function displayFullDescription(obj) {
				var spanContainer = obj.parentNode;
				var fullTextSpan = $('.fullText', obj.parentNode);
				fullTextSpan.css('display', 'inline');
			}

			function requestFilter(sender, args) {
				var senderElement = sender.get_element();
				var itemText = args.get_item().get_text();
				var panelID = senderElement.getAttribute('xmlHttpPanelID');
				var panel = $find(panelID);

				switch (panelID) {
					case 'standardsFilterSubjectXmlHttpPanel':
						var courseDropdown = $find('courseDropdown');
						clearAllDropdownItems(courseDropdown);
						panel.set_value('{"Grade":"' + itemText + '"}');
						Rubric_changeField(sender, args);
						break;
					case 'standardsFilterCourseXmlHttpPanel':
						var gradeDropdown = $find('gradeDropdown');
						var grade = gradeDropdown.get_selectedItem().get_value();
						panel.set_value('{"Grade":"' + grade + '","Subject":"' + itemText + '"}');
						Rubric_changeField(sender, args);
						break;
				}

			}

			function loadChildFilter(sender, args) {
				//load panel
				var senderElement = sender.get_element();
				var results = args.get_content();
				var dropdownObjectID = senderElement.getAttribute('objectToLoadID');
				var dropdownObject = $find(dropdownObjectID);

				//Clear all context menu items
				clearAllDropdownItems(dropdownObject);

				/*Add each new context menu item
				results[i].Key(value) = ID
				results[i].Value(text) = display text
				*/
				for (var i = 0; i < results.length; i++) {
					addDropdownItem(dropdownObject, results[i].Key ? results[i].Key : results[i], results[i].Value);
				}

				if (results.length == 1) {
					dropdownObject.get_items().getItem(0).select();
				}
				else if (dropdownObject._emptyMessage) {
					dropdownObject.clearSelection();
				}
			}

			function addDropdownItem(dropdownObject, itemTextandValue, value) {
				if (dropdownObject && itemTextandValue) {

					/*indicates that client-side changes are going to be made and 
					these changes are supposed to be persisted after postback.*/
					dropdownObject.trackChanges();

					//Instantiate a new client item
					var item = new Telerik.Web.UI.RadComboBoxItem();

					//Set its text and add the item
					if (typeof (value) == 'undefined') {
						item.set_text(itemTextandValue);
						item.set_value(itemTextandValue);
					} else {
						item.set_text(value);
						item.set_value(itemTextandValue);
					}
					dropdownObject.get_items().add(item);

					//submit the changes to the server.
					dropdownObject.commitChanges();
				}

				return false;
			}

			function clearAllDropdownItems(dropdownObject) {
				var allItems = dropdownObject.get_items().get_count();
				if (allItems < 1) {
					return false;
				}

				/*indicates that client-side changes are going to be made and 
				these changes are supposed to be persisted after postback.*/
				dropdownObject.trackChanges();

				//clear all items
				dropdownObject.get_items().clear();

				//submit the changes to the server.
				dropdownObject.commitChanges();

				return false;
			}

			function editFilterName(obj) {
				standardFilterName = document.getElementById('standardFilterName');
				switch (obj.innerText) {
					case 'Edit':
						standardFilterName.className = 'standardFilterNameHeader_edit';
						standardFilterName.disabled = false;
						obj.innerText = 'Done';
						break;
					default:
						standardFilterName.className = 'standardFilterNameHeader';
						standardFilterName.disabled = true;
						obj.innerText = 'Edit';
						break;
				}
			}

			function renderData(objectList, template) {

				var sContent = '';
				var sTable = '';
				if (template == 'B') {
					$.templates("rubricTmplB", {
						markup: '#rubricItemB',
						allowCode: true
					});

					// Now create the rest of the rows for our table of analytical rubric items.
					sContent = $.render.rubricTmplB(objectList);

					// Set up header row for our table of analytical rubric items.
					sTable += '<div class="rubricItemB_div"><table class="rubricItemB_table">';
					sTable += '<tr>';
					sTable += '<td class="rubricItemB_td riRowNum centerText">Pts</td>';
					sTable += '<td class="rubricItemB_td riDescription centerText">Description</td>';
					sTable += '<td class="rubricItemB_td riPreview"></td>';
					sTable += '</tr>';
					sTable += sContent + '</table></div>';

					// Stuff our JSRendered table into the desired div element
					$('#data').html(sTable);
				}
				if (template == 'A') {
					$.templates("rubricTmplA", {
						markup: '#rubricItemA',
						allowCode: true
					});

					// Set up header row for our table of analytical rubric items.
					sTable += '<div class="rubricItemA_div"><table class="rubricItemB_table">';
					sTable += '<tr>';
					sTable += '<td class="rubricItemA_td riRowNum centerText">&nbsp;&nbsp;&nbsp;&nbsp;</td>';
					sTable += '<td class="rubricItemA_td centerText riCriteria">Criteria</td>';
					sTable += '<td class="rubricItemA_td riPreview"></td>';
					sTable += '<td class="riSpacer"></td>';

					for (var x = 0; x <= 10; x++) {
						if (objectList[0].PointsDescription[x]) {
							sTable += '<td class="rubricItemA_td centerText">' + x + 'Pts</td>';
							sTable += '<td class="rubricItemA_td riPreview"></td>';
						}
					}
					sTable += '</tr>';

					// Now create the rest of the rows for our table of analytical rubric items.
					sContent = $.render.rubricTmplA(objectList);

					// Now put the header row, content rows together and complete the table
					sTable += sContent + '</table></div>';

					// Stuff our JSRendered table into the desired div element
					$('#data').html(sTable);
				}
			}

			if (CKEDITOR) {
			    window.onload = function () {
			       
			        CKEDITOR.replace("CkEditorRubricDirections", { toolbar: 'rubricEditor', height: "500px" });
			        CKEDITOR.on('instanceReady', function (event) {

			            event.editor.on('blur', function () {
			                Rubric_Edit_save(this);

			            });
			        });

			    };
			}

			$(document).ready(function () {
			    //prepare_inline_editor_areas();
			    try {
			        buildCriteriaPointsSelection();
			        var dlg = getCurrentCustomDialog();
			        dlg.add_beforeClose(function () { Rubric_Edit_save(CKEDITOR.instances.CkEditorRubricDirections); });
			    }
			    catch (e) {

			    }
			});

			function Rubric_Edit_save(editor) {
			    if (editor.checkDirty()) {
			        var editedContent = editor.getData();
			        Rubric_changeFieldValue("Directions", editedContent);
			        rubricContentNeedsRefreshing = true;
			        $('#contentEditor_Rubric_DirectionsHeaderContentsDiv')[0].innerHTML = editedContent;
			    }			    
			}
		</script>

		<!-- ******************************************************************
			So, why are some of the div tags in the jsrender template below so
			long??  We found that if you made them pretty and readable by adding
			spaces, those spaces would show up in the results.
		******************************************************************-->
		<script id="rubricItemB" type="text/x-jsrender">
			 {{for PointsDescription}}
				 <tr>
					<td class="riBTD riBoldText centerText">{{:#index}}</td>
					<td class="riBTD">
						<div class="rubricItemBPointDescriptionDiv" ID="ri{{:#parent.parent.data.ID}}_{{:#index}}" onclick="return editRubricItem('ri{{:#parent.parent.data.ID}}_{{:#index}}','Edit {{:~formattedTitle_Holistic(#index)}}');">{{if PointDescription == ''}}Click to Edit{{else}}{{:PointDescription}}{{/if}}</div>
					</td>
					<td class="riBTD" >
						<img src="../../../images/viewPageSmall.png" onclick="return riOnLinePreview('ri{{:#parent.parent.data.ID}}_{{:#index}}', 'Preview {{:~formattedTitle_Holistic(#index)}}');" class="riPreviewIcon" title="Preview Item"/>
						<img src="../../../images/edit.png" onclick="return editRubricItem('ri{{:#parent.parent.data.ID}}_{{:#index}}', 'Edit {{:~formattedTitle_Holistic(#index)}}');" class="riEditIcon" title="Edit Item"/>
					</td>
				 </tr>
			 {{/for}}
		</script>
		<script id="rubricItemA" type="text/x-jsrender">
			 <tr id="{{:RubricID}}_{{:ID}}">
				<td class="riATD riBoldText">{{:~updateRowCount(#index+1)}}{{:~getRowCount()}}</td>
				<td class="riATD">
					<div class="rubricItemAPointDescriptionDiv riCriteria wordwrap" ID="ri{{:ID}}_name" onclick="return editRubricItem('ri{{:ID}}_name', 'Edit {{:~formattedTitle_Analytical()}}');">{{if Name == ''}}Click to Edit{{else}}{{:Name}}{{/if}}</div>
				</td>
				<td class="riBTD" >
					<img src="../../../images/viewPageSmall.png" onclick="return riOnLinePreview('ri{{:ID}}_name', 'Preview {{:~formattedTitle_Analytical()}}');" class="riPreviewIcon" title="Preview Criteria"/>
					<img src="../../../images/edit.png" onclick="return editRubricItem('ri{{:ID}}_name', 'Edit {{:~formattedTitle_Analytical()}}');"  class="riEditIcon" title="Edit Criteria"/>
				</td>
				<td class="riSpacer"></td>
				{{for PointsDescription}}
				  <td class="riATD">
					<div class="rubricItemAPointDescriptionDiv wordwrap" ID="ri{{:#parent.parent.data.ID}}_{{:#index}}" onclick="return editRubricItem('ri{{:#parent.parent.data.ID}}_{{:#index}}', 'Edit {{:~formattedTitle_Analytical(#index)}}');">{{if PointDescription == ''}}Click to Edit{{else}}{{:PointDescription}}{{/if}}</div>
				  </td>
				  <td class="riATD"> 
					 <img src="../../../images/viewPageSmall.png" onclick="return riOnLinePreview('ri{{:#parent.parent.data.ID}}_{{:#index}}', 'Preview {{:~formattedTitle_Analytical(#index)}}');" class="riPreviewIcon" title="Preview Item" />
					 <img src="../../../images/edit.png"  onclick="return editRubricItem('ri{{:#parent.parent.data.ID}}_{{:#index}}', 'Edit {{:~formattedTitle_Analytical(#index)}}');" class="riEditIcon" title="Edit Item"/>
				  </td>
				{{/for}}
			 </tr>
		
		</script>
		<script type="text/javascript">
			$.views.helpers({
				updateRowCount: function (value) {
					rowCount = value;
					return '';
				},

				getRowCount: function () {
					return rowCount;
				},

				formattedTitle_Holistic: function (itemIndex) {
					if (itemIndex == 1)
						return '1 Point';
					else
						return itemIndex + ' Points';
				},
				formattedTitle_Analytical: function (itemIndex) {
					var title = 'Criteria-' + rowCount;
					if (itemIndex) title += itemIndex.toString();
					return title;
				}
			});

			$(function () {

			});
		</script>
	</telerik:RadScriptBlock>
	<div class="IconHeader">
		<telerik:RadToolBar ID="mainToolBar" runat="server" Style="z-index: 20;" Width="722px"
			Skin="Sitefinity" EnableRoundedCorners="true" EnableShadows="true" OnClientButtonClicked="mainToolBar_OnClientButtonClicked">
			<Items>
				<telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/delete.png" ToolTip="Delete Rubric"
					Value="DeleteRubric" />
			</Items>
		</telerik:RadToolBar>
		<asp:Image ID="FinishAndReturn" runat="server" Style="float: right; margin-top: 10px;
			margin-right: 10px; display: none;" ImageUrl="~/Images/finish_return.png" />
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
			<telerik:RadPanelBar runat="server" ID="RadPanelBar1" Width="100%"  Skin="Web20" ExpandMode="MultipleExpandedItems" OnClientLoad="onLoad">
				<Items>
					<telerik:RadPanelItem Text="Identification" Expanded="true" runat="server" ID="Rubric_Identification">
						<HeaderTemplate>
							<div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
								<table style="width: 100%;">
									<tr>
										<td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
											<span class="rpExpandHandle"></span>
										</td>
										<td style="width: 25%; vertical-align:top;">
											<div class="RadPanelItemHeader">
												Identification:</div>
										</td>
										<td style="width: 25%; vertical-align:top;">
											Name:&nbsp;&nbsp;<asp:Label runat="server" ID="lblNameEdit" ClientIDMode="Static"></asp:Label>
										</td>
										<td runat="server" id="IdentificationPanel_RubricType" clientidmode="Static" style="width: 25%; vertical-align:top;">
											Type:&nbsp;&nbsp;<asp:Label runat="server" ID="lblTypeEdit" ClientIDMode="Static"></asp:Label>
										</td>
										<td runat="server" id="IdentificationPanel_Scoring" clientidmode="Static" style="width: 24%; vertical-align:top;">
											Scoring:&nbsp;&nbsp;<asp:Label runat="server" ID="lblScoringEdit" ClientIDMode="Static"></asp:Label>
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
												<td colspan="2" class="IdentificationSubHeader">
													Complete the entries below with helpful information
													<br />
													to easily locate the rubric when searching.
												</td>
											</tr>
											<tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">Name:</td>
												<td>
													<telerik:RadTextBox runat="server" ID="txtName" field="Name" Skin="Web20" ClientEvents-OnValueChanged="Rubric_changeField"
														ClientIDMode="Static" />
												</td>
											</tr>
											<tr>
												<td class="fieldLabel" style="padding-left: 10px;">
													<a href="javascript:" onclick="return RubricTypeMessage();">Rubric Type:</a>
												</td>
												<td class="fieldLabel" style="">
													<telerik:RadComboBox ID="cmbType" ClientIDMode="Static" runat="server" Skin="Web20"
														OnClientSelectedIndexChanged="Rubric_changeField_Type" EmptyMessage="Select"
														field="Type" xWidth="95">
													</telerik:RadComboBox>
												</td>
											</tr>
											<tr id="trRubricScoring" runat="server" clientidmode="Static" style="">
												<td class="fieldLabel" style="padding-left: 10px;">Scoring:</td>
												<td class="fieldLabel" style="">
													<telerik:RadComboBox ID="cmbScoring" ClientIDMode="Static" runat="server" Skin="Web20"
														OnClientSelectedIndexChanged="Rubric_changeField" EmptyMessage="Select" field="MaxPoints"
														xWidth="95">
													</telerik:RadComboBox>
												</td>
											</tr>
											<tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px; overflow: hidden;">Keywords:</td>
												<td>
													<telerik:RadTextBox runat="server" ID="txtKeywords" Skin="Web20" field="Keywords" ClientEvents-OnValueChanged="Rubric_changeField" 
														ClientIDMode="Static"/>
												</td>
											</tr>
											<tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">Grade:</td>
												<td>
													<telerik:RadComboBox ID="gradeDropdown" ClientIDMode="Static" runat="server" Skin="Web20"
														OnClientSelectedIndexChanged="requestFilter" EmptyMessage="Select" xmlHttpPanelID="standardsFilterSubjectXmlHttpPanel"
														field="Grade" ClientEvents-OnValueChanged="Rubric_changeField">
													</telerik:RadComboBox>
												</td>
											</tr>
											<tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">Subject:</td>
												<td>
													<telerik:RadComboBox ID="subjectDropdown" ClientIDMode="Static" runat="server" Skin="Web20"
														OnClientSelectedIndexChanged="requestFilter" xmlHttpPanelID="standardsFilterCourseXmlHttpPanel"
														EmptyMessage="Select" field="Subject" ClientEvents-OnValueChanged="Rubric_changeField">
													</telerik:RadComboBox>
												</td>
											</tr>
											<tr>
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">Course:</td>
												<td>
													<telerik:RadComboBox ID="courseDropdown" ClientIDMode="Static" runat="server" Skin="Web20"
														EmptyMessage="Select" field="Course" ClientEvents-OnValueChanged="Rubric_changeField"
														OnClientSelectedIndexChanged="Rubric_changeField">
													</telerik:RadComboBox>
												</td>
											</tr>
										</table>
									</td>
									<td style="width: 20%; border-right: 1px solid grey; vertical-align: top; padding: 0px 5px 0px 5px;">
										<!-- Column 2  -->
										<table style="width: 100%;">
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
													<a href="javascript:" onclick="return CopyrightMessage();">Copyright:</a>
												</td>
												<td>
													<telerik:RadButton ID="rbCopyRightYes" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
														OnClientClicking="Rubric_changeField_copyright" field="Copyright" cbvalue="Yes"
														GroupName="CopyRight" Text="Yes" AutoPostBack="false">
													</telerik:RadButton>
													&nbsp;&nbsp;&nbsp;
													<telerik:RadButton ID="rbCopyRightNo" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
														OnClientClicking="Rubric_changeField_copyright" field="Copyright" cbvalue="No"
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
													<telerik:RadTextBox runat="server" ID="txtSource" Skin="Web20" field="Source" ClientEvents-OnValueChanged="Rubric_changeField" />
												</td>
											</tr>
											<tr id="trCredit" runat="server" clientidmode="Static">
												<td class="fieldLabel" style="width: 95px; padding-left: 10px;">
													Credit:
												</td>
												<td>
													<telerik:RadTextBox runat="server" ID="txtCredit" Skin="Web20" field="Credit" ClientEvents-OnValueChanged="Rubric_changeField" />
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
										</table>
									</td>
								</tr>
								<tr id="trAnalyticalCriteriaPointsSelection" runat="server" clientidmode="Static"
									style="display: none;">
									<td colspan="3" style="width: 100%; border-top: 1px solid;">
										<table>
											<tr>
												<td style="width: 35%; padding-left: 10px; text-align: left; vertical-align: top;
													padding-top: 10px;">
													<div class="IdentificationSubHeader">Enter the number of Criteria and Points below<br /></div>
													<div class="IdentificationSubHeader" style="width: 100%; text-align: center;">Or</div>
													<div class="IdentificationSubHeader">Select a box to the right with the corresponding combination.</div>
													<div style="padding-left: 30px;">
														<font style="font-weight: bold">Criteria: (limit 10)</font>
														<br />
														<asp:TextBox runat="server" ID="txtCriteria" Width="50" ClientIDMode="Static" onchange="return CriteriaUpdate();">
														</asp:TextBox>
														<br />
														<font style="font-weight: bold">Points: (limit 10)</font>
														<br />
														<asp:TextBox ID="txtPoints" runat="server" Width="50" ClientIDMode="Static" onchange="return PointsUpdate();">
														</asp:TextBox>
													</div>
													<br />
													<div class="IdentificationSubHeader" >Collapse the Identification section to edit the rubric content.</div>
												</td>
												<td style="width: 65%; padding-left: 50px;">
													<div id="divCriteriaPointsSelector">
													</div>
												</td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</ContentTemplate>
					</telerik:RadPanelItem>
					<telerik:RadPanelItem Expanded="False" runat="server" ID="Rubric_Directions" Visible="True" >
						<HeaderTemplate>
							<div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
								<table style="width: 100%;">
									<tr>
										<td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
											<span class="rpExpandHandle"></span>
										</td>
										<td style="width: 15%;">
											<div class="RadPanelItemHeader">Directions:</div>
										</td>
										<td style="width: 84%;" >
											<div runat="server" id="contentEditor_Rubric_DirectionsHeaderContentsDiv" clientidmode="Static"  style=" width: 870px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis;"></div>
										</td>
									</tr>
								</table>
							</div>
						</HeaderTemplate>
						<ContentTemplate>
							<div id="divDirections" style="height: 550px; width: 100%;">
							    <div id="CkEditorRubricDirections" runat="server" style="width: 100%; height: 550px;" ClientIDMode="Static">
							        <asp:Label runat="server" ID="lblCkEditorRubricDirections"></asp:Label>
							    </div>
								
							</div>
						</ContentTemplate>
					</telerik:RadPanelItem>
					<telerik:RadPanelItem Expanded="false" runat="server" ID="Rubric_Content">
						<HeaderTemplate>
							<div style="float: left; padding-top: 1px; z-index: 2800;">
								<table style="width: 100%;">
									<tr>
										<td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
											<span class="rpExpandHandle"></span>
										</td>
										<td style="width: 15%;">
											<div class="RadPanelItemHeader">
												Content:</div>
										</td>
										<td style="width: 15%;">
										</td>
										<td style="width: 15%;">
										</td>
										<td style="width: 54%;">
											&nbsp;&nbsp;
										</td>
									</tr>
								</table>
							</div>
						</HeaderTemplate>
						<ContentTemplate>
							<div class="divUploadTemplate">
								<div id="data" style="overflow: auto;">
								</div>
							</div>
						</ContentTemplate>
					</telerik:RadPanelItem>
				</Items>
			</telerik:RadPanelBar>
		</telerik:RadPageView>
		<telerik:RadPageView runat="server" ID="RadPageView5" CssClass="Web20">
			<telerik:RadPanelBar runat="server" ID="RadPanelBar2s" Width="100%"
				Skin="Web20" ExpandMode="MultipleExpandedItems" OnClientLoad="onLoad">
				<Items>
					<telerik:RadPanelItem Expanded="true">
						<HeaderTemplate>
							<div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
								<table style="width: 100%;">
									<tr>
										<td style="width: 25%;">
											<div class="RadPanelItemHeader">
												Identification:</div>
										</td>
										<td style="width: 25%;">
											Name:&nbsp;&nbsp;<asp:Label runat="server" ID="lblNamePreview" ClientIDMode="Static"></asp:Label>
										</td>
										<td style="width: 25%;">
											Type:&nbsp;&nbsp;<asp:Label runat="server" ID="lblTypePreview" ClientIDMode="Static"></asp:Label>
										</td>
										<td runat="server" ID="PreviewPanel_Scoring" clientidmode="Static" style="width: 25%;">
											Scoring:&nbsp;&nbsp;<asp:Label runat="server" ID="lblScoringPreview" ClientIDMode="Static"></asp:Label>
										</td>
									</tr>
								</table>
							</div>
						</HeaderTemplate>
						<ContentTemplate>
							<div class="previewRubricDirections" style="width: 95%; border: 1px solid black; margin-bottom: 10px;">
								<div class="fieldLabel" style="text-align: center; margin-bottom: 10px">Rubric Directions</div>
								<div runat="server" id="ContentEditor_Rubric_Preview_Directions" clientidmode="Static"></div>
							</div>
							<div runat="server" id="ContentEditor_Rubric_Preview_Contents" class="previewRubricContent" style="width: 95%; height: auto" clientidmode="Static">
							</div>
							<!-- iframe id="iframe_preview" style="width: 100%; height: 600px;"></iframe -->
						</ContentTemplate>
					</telerik:RadPanelItem>
				</Items>
			</telerik:RadPanelBar>
		</telerik:RadPageView>
	</telerik:RadMultiPage>
	<%--    <telerik:RadWindow ID="RadWindow1" runat="server" Skin="Forest" Behaviors="Close, Move"
		Modal="true" Width="440px" Height="385px">
		<ContentTemplate>
			<img id="Img1" src="Images/discount.jpg" runat="server" alt="promotions" />
		</ContentTemplate>
	</telerik:RadWindow>--%>
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
</body>
</html>
