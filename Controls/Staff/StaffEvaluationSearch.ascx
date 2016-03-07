<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StaffEvaluationSearch.ascx.cs"
	Inherits="Thinkgate.Controls.Teacher.StaffEvaluationSearch" %>
<style type="text/css">
	.lnk
	{
		margin-left: 4px;
	}
	.lgTxt
	{
		font-size: 11pt;
	}
	.normTxt
	{
		font-size: 10pt;
	}
	.RadComboBox .rcbInputCell .rcbEmptyMessage
	{
	    font-style: normal;   
	}
</style>
<script type="text/javascript">
	function loadStaffPage(UID_Encrypted, evalType) {
		if (evalType == 'TeacherClassroom')
		    window.open('../Record/Teacher.aspx?childPage=yes&xID=' + UID_Encrypted);
		else
			window.open('../Record/Staff.aspx?xID=' + UID_Encrypted);
		return false;
	}

	function loadEvalReport(ID_Encrypted) {
		parent.customDialog({ url: ('../Dialogues/TeacherEvalReport.aspx?xID=' + ID_Encrypted), autoSize: true, name: 'TeacherEvalReport' });
		return false;
	}

	function loadEvalForm(ID_Encrypted) {
		parent.customDialog({ url: ('../Dialogues/TeacherEvalForm.aspx?xID=' + ID_Encrypted), autoSize: true, name: 'TeacherEvalForm' });
		return false;
	}

	function loadStudentGrowth(ID_Encrypted) {
	    parent.customDialog({ url: ('../Dialogues/StudentGrowthData.aspx?xID=' + ID_Encrypted), title: 'Student Growth Data', 
	        maximize: true, maxwidth: 750, maxheight: 520, name: 'StudentGrowthData'
	    });
	    return false;
	}

	function loadAppraisal(ID_Encrypted) {
	    parent.customDialog({ url: ('../Dialogues/TeacherEvalForm.aspx?xID=' + ID_Encrypted), autoSize: true, name: 'TeacherEvalForm' });
		return false;
    }
</script>
<telerik:RadAjaxPanel ID="staffEvaluationSearchAjaxPanel" runat="server" LoadingPanelID="staffEvaluationSearchLoadingPanel" Height="124">
<div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
	<telerik:RadComboBox ID="cmbYear" runat="server" ToolTip="Select a year" Skin="Web20"
		Width="69" OnSelectedIndexChanged="cmbYear_SelectedIndexChanged" AutoPostBack="true"
		CausesValidation="False" HighlightTemplatedItems="true" EmptyMessage="Year">
		<ItemTemplate>
			<span>
				<%# Eval("DropdownText") %></span>
		</ItemTemplate>
	</telerik:RadComboBox>
	<telerik:RadComboBox ID="cmbSchool" runat="server" ToolTip="Select a school" Skin="Web20"
		Width="69" OnSelectedIndexChanged="cmbSchool_SelectedIndexChanged" AutoPostBack="true"
		CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="300" EmptyMessage="School">
		<ItemTemplate>
			<span>
				<%# Eval("SchoolName") %></span>
		</ItemTemplate>
	</telerik:RadComboBox>
	<telerik:RadComboBox ID="cmbGrade" runat="server" ToolTip="Select a grade" Skin="Web20"
		Width="69" OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged" AutoPostBack="true"
		CausesValidation="False" HighlightTemplatedItems="true" EmptyMessage="Grade">
		<ItemTemplate>
			<span>
				<%# Eval("Grade") %></span>
		</ItemTemplate>
	</telerik:RadComboBox>
	<telerik:RadComboBox ID="cmbName" runat="server" ToolTip="Select a staff member"
		Skin="Web20" Width="63" OnSelectedIndexChanged="cmbName_SelectedIndexChanged" AutoPostBack="true"
		CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="300" EmptyMessage="Name">
		<ItemTemplate>
			<span>
				<%# Eval("DropdownText")%></span>
		</ItemTemplate>
	</telerik:RadComboBox>
</div>
<div style="margin-top: 6px;">
	<asp:Label ID="lblNoResults" runat="server" Text="No Evaluations Present" CssClass="lnk normTxt" style="margin-left: 70px;" Visible="False"></asp:Label>
    <div id="lblInitialText" runat="server" style="font-size: 11pt; text-align:center; margin-top:25%;">Please select a school</div>
	<telerik:RadGrid runat="server" ID="grdEval" AutoGenerateColumns="false" Height="200px"
		Width="308px" AllowFilteringByColumn="false" Style="border-width: 0px;" Visible="False">
		<ClientSettings>
			<Scrolling AllowScroll="true" />
		</ClientSettings>
		<MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
			ShowHeadersWhenNoRecords="false">
			<Columns>
				<telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile"
					ItemStyle-BorderWidth="0">
					<ItemTemplate>
						<asp:ImageButton ID="imgEval" runat="server" ClientIDMode="Static" Style="float: left;"
							ImageUrl="~/Images/Clipboard2.png" OnClientClick='<%#Eval("ID_Encrypted", "return loadEvalReport(\"{0}\")")%>' />
						<asp:HyperLink ID="lnkStaffName" runat="server" CssClass="lnk lgTxt" NavigateUrl='<%# (_permLinkActive)?"#":""%>'
							onclick='<%# (_permLinkActive) ? String.Format("javascript:return loadStaffPage(\"{0}\", \"{1}\")", Eval("UID_Encrypted").ToString(), Eval("Type").ToString()):"javascript:return false;" %>'>
							<%# Eval("User_Full_Name")%></asp:HyperLink>
						<div style="margin-left: 10px; margin-top: -10px;">
							<ul>
								<li>
									<asp:HyperLink ID="lnkEvalName" runat="server" CssClass="lnk normTxt" NavigateUrl="#"
										onclick='<%#Eval("ID_Encrypted", "return loadEvalReport(\"{0}\")")%>'><%# Eval("EvalName")%></asp:HyperLink>
								</li>
								<li>
									<asp:HyperLink ID="lnkPride" runat="server" CssClass="lnk normTxt" NavigateUrl="#"
										onclick='<%#Eval("ID_Encrypted", "return loadEvalForm(\"{0}\")")%>' Visible='<%# !(Boolean)Eval("IsSA")%>'>PRIDE</asp:HyperLink>
									<asp:HyperLink ID="lnkAppraisal" runat="server" CssClass="lnk normTxt" NavigateUrl="#"
										onclick='<%#Eval("ID_Encrypted", "return loadAppraisal(\"{0}\")")%>' Visible='<%# (Boolean)Eval("IsSA")%>'>Appraisal</asp:HyperLink>
								</li>
								<li>
									<asp:HyperLink ID="lnkItem2" runat="server" CssClass="lnk normTxt" NavigateUrl="#"
										onclick='<%#Eval("ID_Encrypted", "return loadStudentGrowth(\"{0}\")")%>'>Student Growth</asp:HyperLink>
								</li>
							</ul>
						</div>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
			</Columns>
		</MasterTableView>
	</telerik:RadGrid>
</div>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="staffEvaluationSearchLoadingPanel"
	runat="server" />
