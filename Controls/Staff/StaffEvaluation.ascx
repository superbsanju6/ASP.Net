<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StaffEvaluation.ascx.cs"
	Inherits="Thinkgate.Controls.Teacher.StaffEvaluation" %>
<style type="text/css">
	.lnk
	{
		margin-left: 4px;
	}
	.lgTxt
	{
		font-size: 10pt;
	}
	.normTxt
	{
		font-size: 10pt;
	}
</style>
<script type="text/javascript">
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
<div style="margin-top: 6px;">
	<telerik:RadGrid runat="server" ID="grdEval" AutoGenerateColumns="false" Height="220px"
		Width="308px" AllowFilteringByColumn="false"
		Style="border-width: 0px;">
		<MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false"
			ShowHeadersWhenNoRecords="false">
			<Columns>
				<telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile" ItemStyle-BorderWidth="0">
					<ItemTemplate>
						<asp:ImageButton ID="imgEval" runat="server" ClientIDMode="Static" Style="float: left;"
							ImageUrl="~/Images/Clipboard2.png" OnClientClick='<%#Eval("ID_Encrypted", "return loadEvalReport(\"{0}\")")%>'/>
						<asp:HyperLink ID="lnkEvalName" runat="server" CssClass="lnk lgTxt" NavigateUrl="#"
							onclick='<%#Eval("ID_Encrypted", "return loadEvalReport(\"{0}\")")%>'><%# Eval("EvalName")%></asp:HyperLink>
						<div style="margin-left: 10px; margin-top: -10px;">
							<ul>
								<li>
									<asp:HyperLink ID="lnkPride" runat="server" CssClass="lnk lgTxt" NavigateUrl="#"
										onclick='<%#Eval("ID_Encrypted", "return loadEvalForm(\"{0}\")")%>' Visible='<%# !(Boolean)Eval("IsSA")%>'>PRIDE</asp:HyperLink>
									<asp:HyperLink ID="lnkAppraisal" runat="server" CssClass="lnk lgTxt" NavigateUrl="#"
										onclick='<%#Eval("ID_Encrypted", "return loadAppraisal(\"{0}\")")%>' Visible='<%# (Boolean)Eval("IsSA")%>'>Appraisal</asp:HyperLink>
								</li>
								<li>
									<asp:HyperLink ID="lnkItem2" runat="server" CssClass="lnk lgTxt" NavigateUrl="#"
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
