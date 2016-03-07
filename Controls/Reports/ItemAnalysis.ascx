<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemAnalysis.ascx.cs"
		Inherits="Thinkgate.Controls.Reports.ItemAnalysis" %>
<script type="text/javascript">
    function openItemAnalysisDetail(chartItem) {
        document.getElementById("itemAnalysis_selectedChartItem").value = chartItem;
        document.getElementById("itemAnalysis_btnSelectChartItem").click();
    }

    function openItemPreview(url) {
        var radWin = radopen(url, 'radWindowPreview');
        radWin.setSize(600, 450);
        radWin.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
        radWin.set_visibleStatusbar(false);
        radWin.center();
    }

    function openPDF() {
        var formDropdown = $find('rcbItemAnalysisForms');
        var formValue = formDropdown ? formDropdown.get_selectedItem().get_value() : '';
        var parms = 'analysisType=' + $('#analysisReportType').attr('value') + '&guid=' + $('#guidValue').attr('value') + '&formID=' + formValue
            + '&level=' + $('#levelValue').attr('value') + '&levelID=' + $('#levelIDValue').attr('value');
        printPDFView(true, parms);
    }

    function printExcel() {
        var excelButton = document.getElementById('exportGridImgBtn');
        if (excelButton) {
            excelButton.click();
        }
    }
</script>
<style type="text/css">
		.hiddenButton
		{
				display: none;
		}
		
		td.chartGroupLabel
		{
				width: 20px;
				vertical-align: top;
				font-weight: bold;
				border: 1px solid #dadada;
				border-bottom: 0px;
		}
		
		td.chartAmountLabel
		{
				font-weight: bold;
				border: 1px solid #dadada;
		}
		
		.barDiv
		{
				border: 1px solid black;
				text-align: right;
				font-weight: bold;
		}
		.imgButtons 
        {
            padding: 2px;
            margin-right: 10px;
            cursor: pointer;
		    float: right;
        }
</style>
<table class="reportTable" id="reportTable" runat="server">
		<tr>
				<td>
						# Schools:
						<asp:Label runat="server" ID="lblSchoolCount" />
				</td>
				<td>
						# Teachers:
						<asp:Label runat="server" ID="lblTeacherCount" />
				</td>
				<td>
						# Classes/Groups:
						<asp:Label runat="server" ID="lblClassCount" />
				</td>
				<td>
						# Students:
						<asp:Label runat="server" ID="lblStudentCount" />
				</td>
                <td>
						<span id="classStudentCountTitle" runat="server"># Class Students:</span>
						<asp:Label runat="server" ID="lblClassStudentCount" />
				</td>
		</tr>
		<tr>
				<td>
						High:
						<asp:Label runat="server" ID="lblHigh" />
				</td>
				<td>
						Low:
						<asp:Label runat="server" ID="lblLow" />
				</td>
				<td>
						Median:
						<asp:Label runat="server" ID="lblMedian" />
				</td>
				<td>
						Mean:
						<asp:Label runat="server" ID="lblMean" />
				</td>
                <td></td>
		</tr>
</table>
<div style="width: 100%; overflow: hidden;">
		<div style="width: 200px; float: left;">
			<telerik:RadComboBox ID="rcbItemAnalysisForms" runat="server" Skin="Web20" DataTextField="FormName" DataValueField="FormID" OnSelectedIndexChanged="ctxForm_ItemClick" AutoPostBack="true"></telerik:RadComboBox>
        </div> 
      

        <asp:ImageButton runat="server" ID="printButton" ClientIDMode="Static" CssClass="imgButtons" ImageUrl="~/Images/Printer.png" OnClientClick="openPDF(); return false;" />
        <asp:ImageButton ID="excelImgBtn" ClientIDMode="Static" runat="server" ImageUrl="~/Images/commands/excel_button_edited.png"
                 Width="15px" CssClass="imgButtons" OnClientClick="printExcel(); return false;" />
		
       
    <div style="width: 700px; float: right;">
				<asp:Repeater runat="server" ID="chartItemRepeater">
						<HeaderTemplate>
								<center>
										<table>
												<tr>
						</HeaderTemplate>
						<ItemTemplate>
								<td>
										<div class="chartItemDiv" style="background-color: <%# Eval("color") %>; cursor: pointer; height:70px;"
												onclick="openItemAnalysisDetail('<%# Eval("chartItem") %>')">
												<b>
														<%# Eval("chartItem") %></b>
												<br />
												<font style="font-size: 8pt">
														<%# Eval("asOfDate")%></font><br/>(Click for details)
										</div>
								</td>
						</ItemTemplate>
						<FooterTemplate>
								</tr></table></center>
						</FooterTemplate>
				</asp:Repeater>
		</div>
</div>
<div class="tblContainer" style="width: 100%; height: 430px;">
		<div class="tblRow" style="height: 100%;">
				<div id="criteriaHolder" class="tblLeft" style="background-color: #d1c9ca; height: 100%; padding-top: 3px;">
						<asp:PlaceHolder ID="criteriaPlaceholder" runat="server"></asp:PlaceHolder>
				</div>


				<div id="criteriaScroller" runat="server" class="tblMiddle" style="width: 10px; height: 100%; vertical-align: top; background-color: #CCCCCC;">
					<div id="columnExpanderHandle" onclick="criteriaSliderGo();" style="cursor: pointer;
						 height: 100px; background-color: #0F3789; position: relative; top: 42%;">
						<asp:Image runat="server" ID="columnExpanderHandleImage" ClientIDMode="Static" Style="position: relative;
							 left: 1px; top: 40px; width: 8px" ImageUrl="~/Images/arrow_gray_left.gif" />
					</div>
				</div>

				<div class="tblRight" style="width: 100%; vertical-align: top;">
						<div style="width: 100%; height: 430px; overflow: scroll;">
								<asp:Repeater runat="server" ID="chartSeriesRepeater" OnItemDataBound="chartSeriesRepeater_ItemDataBound">
										<HeaderTemplate>
												<table width="100%">
										</HeaderTemplate>
										<ItemTemplate>
												<tr style="background-color: White">
														<td class="chartGroupLabel" id="phItemLabelCell" runat="server" width="1%">
																<span id="phItemLabel" runat="server"><%# Eval("ChartGroup")%></span>
														</td>
														<span runat="server" id="chartGroup" style="display:none;"><%# Eval("ChartGroup") %></span>
														<span runat="server" id="itemIdentifier" style="display:none;"><%# Eval("xID")%></span>
														<asp:Repeater runat="server" ID="barLineRepeater">
																<HeaderTemplate>
																		<td style="border: 1px solid #dadada; padding-bottom: 2px;">
																</HeaderTemplate>
																<ItemTemplate>
																		<table width="100%">
																				<tr>
																						<td style="border: 1px solid #dadada;">
																								<div class="barDiv" style='background-color: <%# Eval("color") %>; width: <%# Eval("chartAmount") %>%'>
																										&nbsp;
																								</div>
																						</td>
																						<td width="70px" class="chartAmountLabel">
																								<%# Eval("formattedChartAmount")%>&nbsp;
																						</td>
																				</tr>
																		</table>
																</ItemTemplate>
																<FooterTemplate>
																		<br />
																		</td>
																</FooterTemplate>
														</asp:Repeater>
												</tr>
										</ItemTemplate>
										<FooterTemplate>
												</table>
										</FooterTemplate>
								</asp:Repeater>
						</div>
				</div>
		</div>
</div>
<input type="hidden" id="analysisReportType" runat="server" ClientIDMode="Static"/>
<input type="hidden" id="guidValue" runat="server" ClientIDMode="Static"/>
<input type="hidden" id="levelValue" runat="server" ClientIDMode="Static"/>
<input type="hidden" id="levelIDValue" runat="server" ClientIDMode="Static"/>
<asp:HiddenField runat="server" ID="itemAnalysis_selectedChartItem" Value="" ClientIDMode="Static" />
<asp:Button runat="server" ID="itemAnalysis_btnSelectChartItem" OnClick="btnSelectChartItem_Click"
		ClientIDMode="Static" CssClass="hiddenButton" />
<telerik:RadWindowManager runat="server" ID="radWindowMgr">
		<Windows>
				<telerik:RadWindow runat="server" ID="radWindowDetail" VisibleOnPageLoad="false"
						MinWidth="500" AutoSize="true" Modal="true" CssClass="expandedTab" Animation="Slide"
						Skin="Office2010Silver" VisibleStatusbar="false" Behaviors="Close">
				</telerik:RadWindow>
				<telerik:RadWindow runat="server" ID="radWindowPreview" VisibleOnPageLoad="false" 
						Width="500" Height="400" Modal="true" CssClass="expandedTab" Title="Assessment Item Preview"
						Skin="Office2010Silver" VisibleStatusbar="false" Behaviors="Close">
				</telerik:RadWindow>
		</Windows>
</telerik:RadWindowManager>
