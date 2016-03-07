<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Dialogues/Assessment/AssessmentDialog.Master"
		CodeBehind="AssessmentDocUpload.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.AssessmentDocUpload" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
		<style type="text/css">
				body
				{
						font-family: Sans-Serif, Arial;
						position: relative;
						font-size: .8em;
				}
				
				.containerDiv
				{
						width: 99%;
				}
				
				.headerDiv
				{
						margin-bottom: 10px;
						/*margin-left: 37px;*/
						font-weight: bold;
				}
				
				.formContainer
				{
						width: 95%;
						text-align: left;
				}
				
				.headerImg
				{
						position: absolute;
						left: 10;
						top: 10;
						width: 30px;
				}
				
				.roundButtons
				{
						color: #00F;
						font-weight: bold;
						font-size: 12pt;
						padding: 2px;
						display: inline;
						border: solid 1px #000;
						border-radius: 50px;
						margin-left: 10px;
						cursor: pointer;
						background-color: #FFF;
				}
				
		</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:HiddenField ID="hdnAssessmentFileUploadMsg" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnAssessmentFileUploadFlag" runat="server" ClientIDMode="Static" />
	<div style="width: 500px;">
		<br />
		<div>
			<img style="float: left;" alt="Alert" src="../../Images/alert.png" />
			<p style="font-style: italic; font-size: 10pt; text-align:justify; margin: 4px;">The uploaded forms are primarily for sharing the item bank assessment and are <u>not used</u> when administering
			the assessment online.  Take care that uploaded documents match the assessment in Elements<sup>TM</sup> to ensure integrity of the results.</p>
		</div>
		<br />
		<div>
			<table style="margin-left: 30px;">
				<tr>
					<td>
						<span style="font-weight: bold; vertical-align: middle">Filename:&nbsp;</span>
					</td>
					<td>
						<telerik:RadUpload ID="RadUpload1" runat="server" InitialFileInputsCount="1" AllowedFileExtensions=".pdf"
							MaxFileInputsCount="1" OverwriteExistingFiles="False"
							ControlObjectsVisibility="None" InputSize="45" Width="300px" EnableFileInputSkinning="true" Skin="Web20"
							ClientIDMode="Static" OnClientFileSelected="fileSelected">
							<Localization Select="Browse" /> 
						</telerik:RadUpload>
					</td>
				</tr>
			</table>			
			<asp:CustomValidator runat="server" ID="CustomValidator1" Display="Dynamic" ClientValidationFunction="validateRadUpload1">        
							Invalid extension. The only allowable file type is PDF.
			</asp:CustomValidator>
		</div>
		<br />
		<div>
			<asp:Button runat="server" ID="btnCancel" ClientIDMode="Static" CssClass="roundButtons" style="margin-left: 140px;"
							 Width="100" Text="Cancel" OnClientClick="closeCurrentCustomDialog(); return false;" />
			<asp:Button runat="server" ID="btnUpload" ClientIDMode="Static" style="margin-left: 30px;"
					CssClass="roundButtons" Enabled="false"
							 Width="100" Text="Upload" onclick="btnUpload_Click"/>
		</div>
		<br />
		<div>
			<p style="font-size: 10pt; text-align:justify; margin: 4px;">You must honor all copyright protection notifications on all material used within Elements<sup>TM</sup>.  Only use content (Questions, 
			Answers, Images, Addendums, Tests) that your school system has purchased the rights to reproduce or that has been marked as public domain.
			You are responsible for any copyright infringements. If in doubt about the content you wish to use, contact your central office for
			permission or clarification.</p>
		</div>
	</div>
	<script type="text/javascript">
		function fileSelected(radUpload, eventArgs) {
			var btn = document.getElementById("btnUpload");
			btn.disabled = false;
		}

		function validateRadUpload1(source, arguments) {
				arguments.IsValid = $find("RadUpload1").validateExtensions();
		}

		function InDev(sender, args) {
			alert('Function is in development');
		}

		function hideUploadOverlay() {
		    closeCurrentCustomDialog();
		}

		$(document).ready(function () {
		    if ($("#hdnAssessmentFileUploadMsg").val() != "") {
		        var title = $("#hdnAssessmentFileUploadFlag").val() == "E" ? "Error Uploading Files" : "Success Uploading Files";
		        var content = $("#hdnAssessmentFileUploadFlag").val() == "E" ? $("#hdnAssessmentFileUploadMsg").val() : '<div>' + $("#hdnAssessmentFileUploadMsg").val() + '</div>';

		        var wnd = parent.window.radalert(content, 400, 100, title, hideUploadOverlay, '');
		        wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
		        //$('a.rwPopupButton').css('float', 'right');
		    }
		});

	</script>
</asp:Content>
