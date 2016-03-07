<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentObjectsDocuments.ascx.cs"
	Inherits="Thinkgate.Controls.Assessment.AssessmentObjectsDocuments" %>
<style type="text/css">
	.docTable
	{
		width: auto;
		font-size: 9pt;
		color: Black;
		margin-left: auto;
		margin-right: auto;
		margin-top: 8px;
	}
	.rptTable
	{
	}
	.cellCommon
	{
		padding-top: 2px;
		padding-bottom: 2px;
		padding-left: 4px;
		padding-right: 4px;
		font-size: 9pt;
		border-width: 1px;
		border-style: solid;
	}
	.cellContent
	{
		background-color: White;
		border-color: Black;
		text-align: center;
		vertical-align: middle;
	}
	
	.cellHeader
	{
/*		background-image: url(../Images/HeaderBkgnd1.png); */
		background-color: rgb(219, 219, 218);
		color: Black;
		border-color:rgb(164, 171, 178);
		vertical-align: middle;
		text-align: left;
	}
	.headerBtn
	{
		width: 12;
		height: 12;
		vertical-align: middle;
	}
	.contentBtn
	{
		vertical-align: middle;
	}
</style>

<script type="text/javascript">
	function ShowHideReview(sender, args) {
		var rpt = $('.rptTable');
		if (args.get_checked())
			rpt.css('display', '');	
		else
			rpt.css('display', 'none');
	}

	function btnViewFile_Clicked(sender, args) {
		var filename = sender.get_value();
		window.open('../Upload/' + filename);
	}

	function btnUploadAssessment_Clicked(sender, args) {
		var v = sender.valueOf();
		var formID = v.get_value().toString();
		var assessmentID = document.getElementById("hiddenAssessmentID").value;
		parent.customDialog({ url: ('../Dialogues/Assessment/AssessmentDocUpload.aspx?xID=' + assessmentID + '&yID=' + formID + '&doctype=AssessmentFile'), autoSize: true, name: 'DocumentUploadAssessmentFile', title: 'Assessment Document Upload', onClosed: uploadComplete, destroyOnClose: true });
	}

	function btnUploadAnswerKey_Clicked(sender, args) {
		var v = sender.valueOf();
		var formID = v.get_value().toString();
		var assessmentID = document.getElementById("hiddenAssessmentID").value;
		parent.customDialog({ url: ('../Dialogues/Assessment/AssessmentDocUpload.aspx?xID=' + assessmentID + '&yID=' + formID + '&doctype=AnswerKeyFile'), autoSize: true, name: 'DocumentUploadAnswerKeyFile', title: 'Answer Key Document Upload', onClosed: uploadComplete, destroyOnClose: true });
	}

	function btnUploadReview_Clicked(sender, args) {
		var v = sender.valueOf();
		var formID = v.get_value().toString();
		var assessmentID = document.getElementById("hiddenAssessmentID").value;
		parent.customDialog({ url: ('../Dialogues/Assessment/AssessmentDocUpload.aspx?xID=' + assessmentID + '&yID=' + formID + '&doctype=ReviewerFile'), autoSize: true, name: 'DocumentUploadReviewerFile', title: 'Reviewer Document Upload', onClosed: uploadComplete, destroyOnClose: true });
	}

	function uploadComplete() {	   
	    document.getElementById('btnRefresh').click();	    
	}

	function deleteAllAssessment() {	   
		document.getElementById('hiddenBtnDeleteAllAssessment').click();
	}

	function deleteAllAnswerKey() {
		document.getElementById('hiddenBtnDeleteAllAnswerKey').click();
	}

	function deleteAllReview() {
		document.getElementById('hiddenBtnDeleteAllReview').click();
	}

	function InDev() {
		alert('Function is in development');
	} 
	</script>
<asp:HiddenField ID="hiddenAssessmentID" runat="server" ClientIDMode="Static"/>
<asp:HiddenField ID="hiddenUploadPath" runat="server" ClientIDMode="Static"/>
<telerik:RadAjaxPanel ID="rapAssessmentObjectsDocuments" runat="server" LoadingPanelID="assessmentObjectsDocumentsLoadingPanel">
	<asp:Panel ID="pnlTables" runat="server" ScrollBars="Auto" Height="210px" Width="100%">
		<asp:Repeater ID="rptAssessment" runat="server">
			<HeaderTemplate>
				<table class="docTable" cellpadding="5" cellspacing="0">
					<tr>
						<td class="docCell cellCommon cellHeader">
							<span>Documents</span>
						</td>
						<td class="cellCommon cellHeader" colspan="3">
							<span>Assessment</span>
						</td>
					</tr>
					<tr>
						<td class="cellCommon cellHeader">
							<span>Assessment Forms</span>
						</td>
						<td class="cellCommon cellHeader">
							<span>Preview</span>
						</td>
						<td class="cellCommon cellHeader">
							<span>Upload</span>
						</td>
						<td class="cellCommon cellHeader">
							<span>Delete</span>
							<asp:ImageButton ID="btnDeleteAllAssessment" runat="server" CssClass="headerBtn"
							 ImageUrl="~/Images/X.png" OnClientClick="deleteAllAssessment(); return false;"/>
						</td>
					</tr>
			</HeaderTemplate>
			<ItemTemplate>
					<tr>
						<td class="cellCommon cellContent" style="text-align: left;">
							<span><%# Eval("FormIDDisplay") %></span>
						</td>
						<td class="cellCommon cellContent">
							<telerik:RadButton ID="btnViewAssessment" runat="server" Text="Preview Assessment Document" Image-EnableImageButton="true" Image-ImageUrl="~/Images/ViewPageSmall.png" Width="12" Height="16"
								Visible='<%# Eval("HasAssessmentDoc") %>' Value='<%# Eval("AssessmentFile") %>' AutoPostBack="false" OnClientClicked="btnViewFile_Clicked">
							</telerik:RadButton>
						</td>
						<td class="cellCommon cellContent">
							<telerik:RadButton ID="btnUploadAssessment" runat="server" Text="Upload Assessment Document" Image-EnableImageButton="true" Image-ImageUrl="~/Images/Upload.png" Width="16" Height="16"
								Visible='<%# !(Boolean)Eval("HasAssessmentDoc") %>' Value='<%# Eval("FormID") %>' AutoPostBack="false" OnClientClicked="btnUploadAssessment_Clicked">
							</telerik:RadButton>
						</td>
						<td class="cellCommon cellContent">
							<telerik:RadButton ID="btnDeleteAssessment" runat="server" Text="Delete Assessment Document" Image-EnableImageButton="true" Image-ImageUrl="~/Images/X.png" Width="16" Height="16"
								Visible='<%# Eval("HasAssessmentDoc") %>' Value='<%# Eval("FormID") %>' AutoPostBack="true">
							</telerik:RadButton>
						</td>
					</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
		</asp:Repeater>

		<asp:Repeater ID="rptAnswerKey" runat="server">
			<HeaderTemplate>
				<table class="docTable" cellpadding="5" cellspacing="0">
					<tr>
						<td class="docCell cellCommon cellHeader">
							<span>Documents</span>
						</td>
						<td class="cellCommon cellHeader" colspan="3">
							<span>Answer Key</span>
						</td>
					</tr>
					<tr>
						<td class="cellCommon cellHeader">
							<span>Assessment Forms</span>
						</td>
						<td class="cellCommon cellHeader">
							<span>Preview</span>
						</td>
						<td class="cellCommon cellHeader">
							<span>Upload</span>
						</td>
						<td class="cellCommon cellHeader">
							<span>Delete</span>
							<asp:ImageButton ID="btnDeleteAllAnswerKey" runat="server" CssClass="headerBtn"
							 ImageUrl="~/Images/X.png" OnClientClick="deleteAllAnswerKey(); return false;"/>
						</td>
					</tr>
			</HeaderTemplate>
			<ItemTemplate>
					<tr>
						<td class="cellCommon cellContent" style="text-align: left;">
							<span><%# Eval("FormIDDisplay") %></span>
						</td>
						<td class="cellCommon cellContent">
							<telerik:RadButton ID="btnViewAnswerKey" runat="server" Text="Preview Answer Key Document" Image-EnableImageButton="true" Image-ImageUrl="~/Images/ViewPageSmall.png" Width="12" Height="16"
								Visible='<%# Eval("HasAnswerKeyDoc") %>' Value='<%# Eval("AnswerKeyFile") %>' AutoPostBack="false" OnClientClicked="btnViewFile_Clicked">
							</telerik:RadButton>
						</td>
						<td class="cellCommon cellContent">
							<telerik:RadButton ID="btnUploadAnswerKey" runat="server" Text="Upload Answer Key Document" Image-EnableImageButton="true" Image-ImageUrl="~/Images/Upload.png" Width="16" Height="16"
								Visible='<%# !(Boolean)Eval("HasAnswerKeyDoc") %>' Value='<%# Eval("FormID") %>' AutoPostBack="false" OnClientClicked="btnUploadAnswerKey_Clicked">
							</telerik:RadButton>
						</td>
						<td class="cellCommon cellContent">
							<telerik:RadButton ID="btnDeleteAnswerKey" runat="server" Text="Delete Answer Key Document" Image-EnableImageButton="true" Image-ImageUrl="~/Images/X.png" Width="16" Height="16"
								Visible='<%# Eval("HasAnswerKeyDoc") %>' Value='<%# Eval("FormID") %>' AutoPostBack="true">
							</telerik:RadButton>
						</td>
					</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
		</asp:Repeater>

		<asp:Repeater ID="rptReview" runat="server" ClientIDMode="Static">
			<HeaderTemplate>
			 <%--Bug Doc Id - 323 and TFS ID-168 Changes= style="display:none;" in just bellow line,  By Jeetendra Begin--%>
				<table class="docTable rptTable" cellpadding="5" cellspacing="0">
				<%--Changes end --- bug doc Id 323 TFS ID - 168--%>
					<tr>
						<td class="docCell cellCommon cellHeader">
							<span>Documents</span>
						</td>
						<td class="cellCommon cellHeader" colspan="3">
							<span>Review Version</span>
						</td>
					</tr>
					<tr>
						<td class="cellCommon cellHeader">
							<span>Assessment Forms</span>
						</td>
						<td class="cellCommon cellHeader">
							<span>Preview</span>
						</td>
						<td class="cellCommon cellHeader">
							<span>Upload</span>
						</td>
						<td class="cellCommon cellHeader">
							<span>Delete</span>
							<asp:ImageButton ID="btnDeleteAllReview" runat="server" CssClass="headerBtn"
							 ImageUrl="~/Images/X.png" OnClientClick="deleteAllReview(); return false;"/>
						</td>
					</tr>
			</HeaderTemplate>
			<ItemTemplate>
					<tr>
						<td class="cellCommon cellContent" style="text-align: left;">
							<span><%# Eval("FormIDDisplay") %></span>
						</td>
						<td class="cellCommon cellContent">
							<telerik:RadButton ID="btnViewReview" runat="server" Text="Preview Reviewer Document" Image-EnableImageButton="true" Image-ImageUrl="~/Images/ViewPageSmall.png" Width="12" Height="16"
								Visible='<%# Eval("HasReviewDoc") %>' Value='<%# Eval("ReviewerFile") %>' AutoPostBack="false" OnClientClicked="btnViewFile_Clicked">
							</telerik:RadButton>
						</td>
						<td class="cellCommon cellContent">
							<telerik:RadButton ID="btnUploadReview" runat="server" Text="Upload Review Document" Image-EnableImageButton="true" Image-ImageUrl="~/Images/Upload.png" Width="16" Height="16"
								Visible='<%# !(Boolean)Eval("HasReviewDoc") && (Boolean)Eval("CanUploadReview")%>' Value='<%# Eval("FormID") %>' AutoPostBack="false" OnClientClicked="btnUploadReview_Clicked">
							</telerik:RadButton>
						</td>
						<td class="cellCommon cellContent">
							<telerik:RadButton ID="btnDeleteReview" runat="server" Text="Delete Reviewer Document" Image-EnableImageButton="true" Image-ImageUrl="~/Images/X.png" Width="16" Height="16"
								Visible='<%# (Boolean)Eval("HasReviewDoc") && (Boolean)Eval("CanUploadReview") %>' Value='<%# Eval("FormID") %>' AutoPostBack="true">
							</telerik:RadButton>
						</td>
					</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
		</asp:Repeater>

	</asp:Panel>

	<%--Default the option was checked, Bug Doc Id - 323 and TFS ID-168,  By Jeetendra Begin--%>
	<telerik:RadButton ID="cbxReview" runat="server" ToggleType="CheckBox" 
		ButtonType="ToggleButton" Skin="Web20"
			Text="Display Review Version Documents" Checked="false"
			OnClientCheckedChanged="ShowHideReview" AutoPostBack="false"/>
	 <%--Bug doc id 323 and TFS ID-168 End--%>

	<!-- Hidden, tiny button to cause refresh of tile -->
	<asp:Button ID="btnRefresh" runat="server" ClientIDMode="Static" Width="0" Height="0" Font-Size="1pt"
		Style="visibility: hidden;" onclick="btnRefresh_Click" />
	<asp:Button ID="hiddenBtnDeleteAllAssessment" runat="server" ClientIDMode="Static" Width="0" Height="0" Font-Size="1pt"
		Style="visibility: hidden;" onclick="btnDeleteAllAssessment_Click" />
	<asp:Button ID="hiddenBtnDeleteAllAnswerKey" runat="server" 
		ClientIDMode="Static" Width="0" Height="0" Font-Size="1pt"
		Style="visibility: hidden;" onclick="btnDeleteAllAnswerKey_Click"/>
	<asp:Button ID="hiddenBtnDeleteAllReview" runat="server" ClientIDMode="Static" 
		Width="0" Height="0" Font-Size="1pt"
		Style="visibility: hidden;" onclick="btnDeleteAllReview_Click"/>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="assessmentObjectsDocumentsLoadingPanel"
	runat="server" />

