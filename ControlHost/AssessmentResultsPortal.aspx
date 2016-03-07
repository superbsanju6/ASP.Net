<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
	CodeBehind="AssessmentResultsPortal.aspx.cs" Inherits="Thinkgate.ControlHost.AssessmentResultsPortal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
	<script type="text/javascript">
		function SelectCheckBox(gridClientID, rowindex, checkboxID) {
			var grid = $find(gridClientID);
			var MasterTable = grid.get_masterTableView();
			var Row = MasterTable.get_dataItems()[rowindex];

			var checkboxes = Row._element.getElementsByTagName("INPUT");
			var index;
			for (index = 0; index < checkboxes.length; index++) {
				if (checkboxes[index].id.indexOf(checkboxID) != -1) {
					checkboxes[index].checked = !checkboxes[index].checked;
					checkboxes[index].disabled = false;
				}
			}
		}

		function CheckBoxListSelect(cbControl, state) {
			var chkBoxList = document.getElementById(cbControl);
			var chkBoxCount = chkBoxList.getElementsByTagName("input");
			for (var i = 0; i < chkBoxCount.length; i++) {
				chkBoxCount[i].checked = state;
			}

			return false;
		}
	</script>
	<style type="text/css">
		.rbl input[type="radio"]
		{
			margin-left: 10px;
			margin-right: 1px;
		}
	</style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<center>
		<table class="header" style="width: 990px">
			<tr>
				<td width="225px" style="text-align: center">
					<asp:Image runat='server' ID="headerImage" ImageUrl="~/Images/datautil_sm.jpg" />
				</td>
				<td style="vertical-align: top;">
					<table width="100%">
						<tr>
							<td>
								<asp:Label runat="server" ID="lblPageTitle" class="pageTitle" Text="Assessment Results Portal" />
							</td>
							<td>
								<asp:Label runat="server" ID="lblChartTitle1" CssClass="largeLabel" />
								<asp:Label runat="server" ID="lblChartTitle3" CssClass="largeLabel" />
								<asp:Label runat="server" ID="lblChartTitle4" CssClass="largeLabel" />
							</td>
							<td style="text-align: right">
								<asp:CheckBox runat="server" ID="chkPerformanceLevels" Checked="true" AutoPostBack="true"
									OnCheckedChanged="chkPerformanceLevels_CheckedChanged" />
								To hide or show performance level colors, click here.
								<br />
								<asp:CheckBox runat="server" ID="chkUnlockedColumns" Checked="true" AutoPostBack="true"
									OnCheckedChanged="chkUnlockedColumns_CheckedChanged" />
								To hide or show unlocked columns, click here.
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
		<table width="990px" style="padding: 0px; margin: 0px;">
			<tbody style="padding: 0px">
				<tr>
					<td width="225px" class="leftnav">
						<telerik:RadPanelBar runat="server" ID="leftNavPanelBar" Width="225" ExpandMode="MultipleExpandedItems">
							<Items>
								<telerik:RadPanelItem Text="Assessment Category" Expanded="true">
									<ContentTemplate>
										<div style="padding-left: 2px; padding-right: 2px;">
											<telerik:RadComboBox runat="server" ID="ddlAssessmentCategory" Width="125" AutoPostBack="true"
												OnSelectedIndexChanged="ddlAssessmentCategory_SelectedIndexChanged" />
										</div>
									</ContentTemplate>
								</telerik:RadPanelItem>
								<telerik:RadPanelItem Text="Portal Criteria" Expanded="false">
									<ContentTemplate>
										<div style="padding-left: 2px; padding-right: 2px;">
											Term(s)<br />
											<telerik:RadGrid ID="rgTerms" runat="server" AutoGenerateColumns="False" CellSpacing="0" EnableNoRecordsTemplate="true"
												a GridLines="None" AllowMultiRowSelection="True" OnItemDataBound="rgTerms_ItemDataBound"
												OnNeedDataSource="rgTerms_NeedDataSource">
												<MasterTableView EditMode="InPlace">
													<NoRecordsTemplate>
														<div>No selection made. Click Edit to select terms.</div>
													</NoRecordsTemplate>
													<CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>
													<RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
														<HeaderStyle Width="20px"></HeaderStyle>
													</RowIndicatorColumn>
													<ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
														<HeaderStyle Width="20px"></HeaderStyle>
													</ExpandCollapseColumn>
													<Columns>
														<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" UniqueName="TemplateColumn">
															<ItemTemplate>
																<asp:CheckBox runat="server" ID="checked" Checked='<%# Bind("checked") %>'></asp:CheckBox>
															</ItemTemplate>
														</telerik:GridTemplateColumn>
														<telerik:GridBoundColumn FilterControlAltText="Filter column column" UniqueName="category"
															DataField="categoryName">
														</telerik:GridBoundColumn>
													</Columns>
													<EditFormSettings>
														<EditColumn FilterControlAltText="Filter EditCommandColumn column">
														</EditColumn>
													</EditFormSettings>
												</MasterTableView>
												<FilterMenu EnableImageSprites="False">
													<WebServiceSettings>
														<ODataSettings InitialContainerName="">
														</ODataSettings>
													</WebServiceSettings>
												</FilterMenu>
												<HeaderContextMenu CssClass="GridContextMenu GridContextMenu_Default">
													<WebServiceSettings>
														<ODataSettings InitialContainerName="">
														</ODataSettings>
													</WebServiceSettings>
												</HeaderContextMenu>
											</telerik:RadGrid>
											<br />
											<telerik:RadButton ID="rbClearTerms" runat="server" Text="Clear" OnClick="rbClearTerms_Click">
											</telerik:RadButton>
											&nbsp;|&nbsp;
											<telerik:RadButton ID="rbEditTerms" runat="server" Text="Edit" OnClick="rbEditTerms_Click">
											</telerik:RadButton>
											&nbsp;|&nbsp;
											<telerik:RadButton runat="server" ID="btnRefreshResults1" Text="Refresh Results"
												OnClick="btnRefreshResults_Click" />
											<br />
											Assessment Type(s)<br />
											<telerik:RadGrid ID="rgAssessments" runat="server" AutoGenerateColumns="False" CellSpacing="0"
												a GridLines="None" AllowMultiRowSelection="True" OnItemDataBound="rgAssessments_ItemDataBound"
												OnNeedDataSource="rgAssessments_NeedDataSource">
												<MasterTableView EditMode="InPlace">
													<NoRecordsTemplate>
														<div>No selection made. Click Edit to select assessments.</div>
													</NoRecordsTemplate>
													<CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>
													<RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
														<HeaderStyle Width="20px"></HeaderStyle>
													</RowIndicatorColumn>
													<ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
														<HeaderStyle Width="20px"></HeaderStyle>
													</ExpandCollapseColumn>
													<Columns>
														<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" UniqueName="TemplateColumn">
															<ItemTemplate>
																<asp:CheckBox runat="server" ID="checked" Checked='<%# Bind("checked") %>'></asp:CheckBox>
															</ItemTemplate>
														</telerik:GridTemplateColumn>
														<telerik:GridBoundColumn FilterControlAltText="Filter column column" UniqueName="category"
															DataField="categoryName">
														</telerik:GridBoundColumn>
													</Columns>
													<EditFormSettings>
														<EditColumn FilterControlAltText="Filter EditCommandColumn column">
														</EditColumn>
													</EditFormSettings>
												</MasterTableView>
												<FilterMenu EnableImageSprites="False">
													<WebServiceSettings>
														<ODataSettings InitialContainerName="">
														</ODataSettings>
													</WebServiceSettings>
												</FilterMenu>
												<HeaderContextMenu CssClass="GridContextMenu GridContextMenu_Default">
													<WebServiceSettings>
														<ODataSettings InitialContainerName="">
														</ODataSettings>
													</WebServiceSettings>
												</HeaderContextMenu>
											</telerik:RadGrid>
											<br />
											<telerik:RadButton ID="rbClearAssessments" runat="server" Text="Clear" OnClick="rbClearAssessments_Click">
											</telerik:RadButton>
											&nbsp;|&nbsp;
											<telerik:RadButton ID="rbEditAssessments" runat="server" Text="Edit" OnClick="rbEditAssessments_Click">
											</telerik:RadButton>
											&nbsp;|&nbsp;
											<telerik:RadButton runat="server" ID="btnRefreshResults2" Text="Refresh Results"
												OnClick="btnRefreshResults_Click" />
											<br />
										</div>
									</ContentTemplate>
								</telerik:RadPanelItem>
								<telerik:RadPanelItem Text="Demographics">
									<ContentTemplate>
										<table width="100%">
											<tr>
												<td align="left" style="padding-left: 2px">
													Gender:
												</td>
											</tr>
											<tr>
												<td align="right" style="padding-right: 4px;">
													<asp:RadioButtonList ID="rblGender" runat="server" CssClass="rbl" RepeatDirection="Horizontal">
														<asp:ListItem Selected="True">All</asp:ListItem>
														<asp:ListItem>Male</asp:ListItem>
														<asp:ListItem>Female</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
											<tr>
												<td align="left" style="padding-left: 2px">
													Race:
												</td>
											</tr>
											<tr>
												<td align="right" style="padding-right: 4px;">
													<asp:DropDownList ID="ddlRace" runat="server" AppendDataBoundItems="true">
														<asp:ListItem Selected="True">All</asp:ListItem>
													</asp:DropDownList>
												</td>
											</tr>
											<tr>
												<td align="left" style="padding-left: 2px">
													English Language Learner:
												</td>
											</tr>
											<tr>
												<td align="right" style="padding-right: 4px;">
													<asp:RadioButtonList ID="rblEnglishLanguageLearner" CssClass="rbl" runat="server"
														RepeatDirection="Horizontal">
														<asp:ListItem Selected="True">All</asp:ListItem>
														<asp:ListItem>Yes</asp:ListItem>
														<asp:ListItem>No</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
											<tr>
												<td align="left" style="padding-left: 2px">
													Economically Disadvantaged:
												</td>
											</tr>
											<tr>
												<td align="right" style="padding-right: 4px;">
													<asp:RadioButtonList ID="rblEconomicallyDisadvantaged" CssClass="rbl" runat="server"
														RepeatDirection="Horizontal">
														<asp:ListItem Selected="True">All</asp:ListItem>
														<asp:ListItem>Yes</asp:ListItem>
														<asp:ListItem>No</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
											<tr>
												<td align="left" style="padding-left: 2px">
													Gifted:
												</td>
											</tr>
											<tr>
												<td align="right" style="padding-right: 4px;">
													<asp:RadioButtonList ID="rblGifted" runat="server" CssClass="rbl" RepeatDirection="Horizontal">
														<asp:ListItem Selected="True">All</asp:ListItem>
														<asp:ListItem>Yes</asp:ListItem>
														<asp:ListItem>No</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
											<tr>
												<td align="left" style="padding-left: 2px">
													Students With Disabilities:
												</td>
											</tr>
											<tr>
												<td align="right" style="padding-right: 4px;">
													<asp:RadioButtonList ID="rblStudentsWithDisabilities" CssClass="rbl" runat="server"
														RepeatDirection="Horizontal">
														<asp:ListItem Selected="True">All</asp:ListItem>
														<asp:ListItem>Yes</asp:ListItem>
														<asp:ListItem>No</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
											<tr>
												<td align="left" style="padding-left: 2px">
													Limited English Proficiency:
												</td>
											</tr>
											<tr>
												<td align="right" style="padding-right: 4px;">
													<asp:RadioButtonList ID="rblLimitedEnglishProficiency" CssClass="rbl" runat="server"
														RepeatDirection="Horizontal">
														<asp:ListItem Selected="True">All</asp:ListItem>
														<asp:ListItem>Yes</asp:ListItem>
														<asp:ListItem>No</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
											<tr>
												<td align="left" style="padding-left: 2px">
													ESOL:
												</td>
											</tr>
											<tr>
												<td align="right" style="padding-right: 4px;">
													<asp:RadioButtonList ID="rblESOL" runat="server" CssClass="rbl" RepeatDirection="Horizontal">
														<asp:ListItem Selected="True">All</asp:ListItem>
														<asp:ListItem>Yes</asp:ListItem>
														<asp:ListItem>No</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
											<tr>
												<td align="left" style="padding-left: 2px">
													ASTEP:
												</td>
											</tr>
											<tr>
												<td align="right" style="padding-right: 4px;">
													<asp:RadioButtonList ID="rblASTEP" runat="server" CssClass="rbl" RepeatDirection="Horizontal">
														<asp:ListItem Selected="True">All</asp:ListItem>
														<asp:ListItem>Yes</asp:ListItem>
														<asp:ListItem>No</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
											<tr>
												<td align="left" style="padding-left: 2px">
													Early Intervention Program:
												</td>
											</tr>
											<tr>
												<td align="right" style="padding-right: 4px;">
													<asp:RadioButtonList ID="rblEarlyInterventionProgram" CssClass="rbl" runat="server"
														RepeatDirection="Horizontal">
														<asp:ListItem Selected="True">All</asp:ListItem>
														<asp:ListItem>Yes</asp:ListItem>
														<asp:ListItem>No</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
										</table>
										<center>
											<telerik:RadButton runat="server" ID="btnRefreshResults3" Text="Refresh Results"
												OnClick="btnRefreshResults_Click" />
										</center>
									</ContentTemplate>
								</telerik:RadPanelItem>
							</Items>
						</telerik:RadPanelBar>
					</td>
					<td width="5px">
						&nbsp;
					</td>
					<td style="vertical-align: top">
						<telerik:RadAjaxPanel ID="resultsPanel" runat="server" LoadingPanelID="resultsLoadingPanel"
							Width="100%" Height="100%">
							<center>
								<br />
								<asp:Label runat="server" ID="lblChartTitle2" CssClass="largeLabel" />
								<br />
							</center>
							<telerik:RadTreeList ID="radTreeResults" runat="server" OnNeedDataSource="radTreeResults_NeedDataSource"
								OnItemDataBound="radTreeResults_ItemDataBound" ParentDataKeyNames="ParentConcatKey"
								DataKeyNames="ConcatKey" AutoGenerateColumns="false">
							</telerik:RadTreeList>
						</telerik:RadAjaxPanel>
						<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="resultsLoadingPanel" runat="server" />
					</td>
				</tr>
			</tbody>
		</table>
	</center>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageFooter" runat="server">
</asp:Content>
