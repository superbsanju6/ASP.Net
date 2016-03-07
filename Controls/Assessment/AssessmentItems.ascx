<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentItems.ascx.cs"
		Inherits="Thinkgate.Controls.Assessment.AssessmentItems" %>
<%@ Register Src="~/Charts/BarChart.ascx" TagName="BarChart" TagPrefix="bc" %>
<style>
    .rdContent{
    overflow: hidden !important;
    }
</style>

<asp:PlaceHolder ID="phNoItems" runat="server"></asp:PlaceHolder>
<bc:BarChart ID="barchart1" runat="server" ClientIDMode="static" Width="311" Height="105"
		GridLines="7" BarHexColor="#007fff"/>
<bc:BarChart ID="barchart2" runat="server" ClientIDMode="static" Width="311" Height="105"
		GridLines="7" BarHexColor="#007fff" />
<div id="searchAddDiv">
		<div id="btnAdd" runat="server" class="bottomTextButton" style="background-image: url(../Images/add.gif);" title="Add New"
				onclick='alert("Function is in development");'>
				Add </div>
		<div id="btnSearch" runat="server" class="bottomTextButton"  onclick="alert('Function is in development')"
				style="background-image: url(../Images/mag_glass.png);" title="Search"> Search
		</div>
</div>
<script type="text/javascript">
	function addNewItem(itemType){
	    var dTitle = "Identification";
	    var dHeight = 0;
	    var dWidth = 0;
	    switch (itemType) {
	        case "Rubric":
	            dHeight = 405;
	            dWidth = 480;
	            break;
	        case "Item":
	            dHeight = 405;
	            dWidth = 480;
	            break;
	        case "Image":
	            dHeight = 265;
	            dWidth = 480;
	            break;
	        case "Addendum":
	            dHeight = 500;
	            dWidth = 480;
	            break;
	    }
		customDialog({ url: ('../Dialogues/AddNewItem.aspx?xID=' + itemType), title: dTitle , height: dHeight, width: dWidth, autoSize: true, name: 'NewItem' });
	}

</script>
