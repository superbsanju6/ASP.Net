<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentContent.ascx.cs"
	Inherits="Thinkgate.Controls.Assessment.AssessmentContent" %>

<style type="text/css">
	.tblTextWd
	{
		width: 120px;
	}

	.cellCommon
	{
		padding-top: 2px;
		padding-bottom: 2px;
		padding-left: 4px;
		padding-right: 4px;
		text-align: center;
	}
	
	.cellContent
	{
		background-color: White;
	}
	
	.cellHeader
	{
/*		background-image: url(../Images/HeaderBkgnd1.png); */
		background-color: Gray;
		color: White;
	}
</style>

<telerik:RadAjaxPanel ID="assessmentContentAjaxPanel" runat="server" LoadingPanelID="assessmentContentLoadingPanel">
	<telerik:RadMultiPage runat="server" ID="assessmentContentRadMultiPage" SelectedIndex="0" 
	Height="210px" Width="100%" CssClass="multiPage" Style="overflow-y: auto ! important; overflow-x: hidden ! important;">
		<telerik:RadPageView runat="server" ID="radPageViewAttributes" Height="209px" Width="100%" style="border-bottom: 1px solid Black;">
			<table width="100%" class="fieldValueTable">
				<!-- This is a workaround for a Telerik bug that causes the whole PageView to move down if a margin-top is put
						 on the table. So we just make an empty row that is the height of the top margin desired.
						
						 Another kludge: The table labels have to be made a fixed width because by default they are taking up most of the width.
						 Dunno why. It just happens when on this RadPageView.
				-->
				<tr style="height: 15px;"><td class="fieldLabel">&nbsp</td></tr>
				<tr>
					<td class="fieldLabel tblTextWd">Status:</td>
					<td>
						<asp:Label runat="server" ID="lblStatus" />
					</td>
				</tr>
				<tr>
					<td class="fieldLabel tblTextWd">Standards:</td>
					<td>
						<asp:Label runat="server" ID="lblStandards" />
					</td>
				</tr>
				<tr>
					<td class="fieldLabel tblTextWd">Rigor Levels:</td>
					<td>
						<asp:Label runat="server" ID="lblRigorLevels" />
					</td>
				</tr>
				<tr>
					<td class="fieldLabel tblTextWd">Items:</td>
					<td>
						<asp:Label runat="server" ID="lblitems" />
					</td>
				</tr>
				<tr>
					<td class="fieldLabel tblTextWd">Addendums:</td>
					<td>
						<asp:Label runat="server" ID="lblAddendums" />
					</td>
				</tr>
				<tr>
					<td class="fieldLabel tblTextWd">Rubrics:</td>
					<td>
						<asp:Label runat="server" ID="lblRubrics" />
					</td>
				</tr>
				<tr>
					<td class="fieldLabel tblTextWd">Last Edited:</td>
					<td>
						<asp:Label runat="server" ID="lblLastEdited" />
					</td>
				</tr>
			</table>
		</telerik:RadPageView>

		<telerik:RadPageView runat="server" ID="radPageViewStdDist" Height="209px" Width="100%" style="border-bottom: 1px solid Black;">
			<telerik:RadGrid ID="grdStdDist" runat="server" AllowPaging="False" AllowSorting="False"
				Width="100%" AutoGenerateColumns="False" CellSpacing="0"
				GridLines="None" ShowFooter="false" AutoGenerateEditColumn="False">
				<SortingSettings EnableSkinSortStyles="False" />
				<MasterTableView>
					<Columns>
						<telerik:GridTemplateColumn DataField="StandardName" HeaderText="Standard">
							<ItemTemplate>
								<asp:HyperLink ID="lnkStandard" runat="server"><%# Eval("StandardName") %></asp:HyperLink>
							</ItemTemplate>
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="30%"/>
						</telerik:GridTemplateColumn>
						<telerik:GridTemplateColumn DataField="PercentAssessment" HeaderText="% of Assessment">
							<ItemTemplate>
								<asp:Label ID="lblPercent" runat="server" Text='<%# Eval("PercentAssessment") %>'></asp:Label>
							</ItemTemplate>
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="30%" />
						</telerik:GridTemplateColumn>
						<telerik:GridTemplateColumn DataField="ItemCount" HeaderText="Items">
							<ItemTemplate>
								<asp:Label ID="lblItems" runat="server" Text='<%# Eval("StandardQuestionCount") %>'></asp:Label>
							</ItemTemplate>
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="30%" />
						</telerik:GridTemplateColumn>
					</Columns>
				</MasterTableView>
			</telerik:RadGrid>
		</telerik:RadPageView>

		<telerik:RadPageView runat="server" ID="radPageViewRigorDist" Height="209px" Width="100%" style="border-bottom: 1px solid Black;">
			<asp:Panel ID="pnlRigorDist" runat="server" ScrollBars="Auto" Height="100%" Width="100%">
				<asp:Literal ID="litRigorTbl" runat="server"></asp:Literal>
<!-- Initial test table
				<table border="1">
					<tr>
						<th class="cellCommon cellHeader">Distribution</th>
						<th class="cellCommon cellHeader">RBT</th>
						<th class="cellCommon cellHeader" colspan="4">Item Criteria Summary</th>
					</tr>
					<tr>
						<td class="cellCommon cellContent">15</td>
						<td class="cellCommon cellContent"></td>
						<th class="cellCommon cellHeader">Multiple Choice</th>
						<th class="cellCommon cellHeader">Short Answer</th>
						<th class="cellCommon cellHeader">Essay</th>
						<th class="cellCommon cellHeader">True False</th>
					</tr>

					<tr>
						<td class="cellCommon cellContent">6</td>
						<td class="cellCommon cellContent">Remembering</td>
						<td class="cellCommon cellContent">5</td>
						<td class="cellCommon cellContent">1</td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
					</tr>

					<tr>
						<td class="cellCommon cellContent">4</td>
						<td class="cellCommon cellContent">Understanding</td>
						<td class="cellCommon cellContent">4</td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
					</tr>

					<tr>
						<td class="cellCommon cellContent">3</td>
						<td class="cellCommon cellContent">Applying</td>
						<td class="cellCommon cellContent">2</td>
						<td class="cellCommon"></td>
						<td class="cellCommon cellContent">1</td>
						<td class="cellCommon"></td>
					</tr>

					<tr>
						<td class="cellCommon cellContent"></td>
						<td class="cellCommon cellContent">Analyzing</td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
					</tr>

					<tr>
						<td class="cellCommon cellContent">2</td>
						<td class="cellCommon cellContent">Evaluating</td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon cellContent">2</td>
					</tr>

					<tr>
						<td class="cellCommon cellContent"></td>
						<td class="cellCommon cellContent">Creating</td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
					</tr>

					<tr>
						<td class="cellCommon cellContent"></td>
						<td class="cellCommon cellContent">Not Specified</td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
					</tr>

					<tr>
						<td class="cellCommon cellContent"></td>
						<td class="cellCommon cellContent">Blank Items</td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
						<td class="cellCommon"></td>
					</tr>
				</table>
-->
			</asp:Panel>
		</telerik:RadPageView>
	</telerik:RadMultiPage>		

	<telerik:RadTabStrip runat="server" ID="assessmentContentRadTabStrip" Orientation="HorizontalBottom"
				SelectedIndex="0" MultiPageID="assessmentContentRadMultiPage" Skin="Thinkgate_Blue" EnableEmbeddedSkins="false" Align="Left"
				OnClientTabSelected="toggleView_RadTab_SmallTile" style="margin-top: 0px;">
		<Tabs>
			<telerik:RadTab Text="Summary">
			</telerik:RadTab>
			<telerik:RadTab Text="Item/Standard Dist.">
			</telerik:RadTab>
			<telerik:RadTab Text="Rigor Dist.">
			</telerik:RadTab>
		</Tabs>
	</telerik:RadTabStrip>

</telerik:RadAjaxPanel>

<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="assessmentContentLoadingPanel"	runat="server" />
