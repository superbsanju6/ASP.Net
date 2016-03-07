<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AtRiskStandard.ascx.cs" Inherits="Thinkgate.Controls.Reports.AtRiskStandard" %>

<telerik:RadAjaxPanel runat="server" ID="atRiskStudentPanel" LoadingPanelID="atRiskStudentLoadingPanel">

<div class="tblContainer" style="width: 100%; ">
		<div class="tblRow" style="">
				<div id="criteriaHolder" class="tblLeft" style="background-color: #d1c9ca; height: 100%; padding-top: 3px;">
						<asp:PlaceHolder ID="criteriaPlaceholder" runat="server"></asp:PlaceHolder>
				</div>

				<div id="criteriaScroller" class="tblMiddle" style="width: 10px; height: 100%; vertical-align: top; background-color: #CCCCCC;">
					<div id="columnExpanderHandle" onclick="criteriaSliderGo();" style="cursor: pointer;
						 height: 100px; background-color: #0F3789; position: relative; top: 200px;">
						<asp:Image runat="server" ID="columnExpanderHandleImage" ClientIDMode="Static" Style="position: relative;
							 left: 1px; top: 40px; width: 8px" ImageUrl="~/Images/arrow_gray_left.gif" />
					</div>
				</div>

				<div class="tblRight" style="width: 100%;  height: 550px; vertical-align: top;">
						<div style="width: 100%; height: 100%; overflow: hidden;">
								<telerik:RadTreeList ID="atRiskTree" AllowLoadOnDemand="false" AllowSorting="true"
				AutoGenerateColumns="false"  OnItemDataBound="atRiskTree_ItemDataBound"
				 DataKeyNames="Key" ParentDataKeyNames="parentconcatkey"
				 AllowMultiItemSelection="false" runat="server" Height="100%" Width="100%">
				<Columns>
						<telerik:TreeListTemplateColumn DataField="KeyDisp" UniqueName="KeyDisp">
								<ItemTemplate>
										<asp:Label ID="lblKeyDisp" runat="server" Text='<%# Eval("KeyDisp")%>' Font-Bold="true" />: 
										<asp:Label ID="lblScore" runat="server" Text='<%# Eval("Score")%>' ForeColor="Blue" /><br />
										<asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description")%>' />
								</ItemTemplate>
						</telerik:TreeListTemplateColumn>
				</Columns>
				<ClientSettings AllowPostBackOnItemClick="false">
						<Scrolling AllowScroll="true" />
				</ClientSettings>
		</telerik:RadTreeList>
						</div>
				</div>
		</div>
</div>
	 
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel runat="server" ID="atRiskStudentLoadingPanel">
</telerik:RadAjaxLoadingPanel>
