<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentAddendums.ascx.cs"
    Inherits="Thinkgate.Controls.Assessment.AssessmentAddendums" %>
<%@ Register Src="~/Charts/BarChart.ascx" TagName="BarChart" TagPrefix="bc" %>

<bc:BarChart ID="barchart1" runat="server" ClientIDMode="static" Width="311" GridLines="7" Visible="false" />
<bc:BarChart ID="barchart2" runat="server" ClientIDMode="static" Width="311" GridLines="7" Visible="false" />

<div>
<div class="bottomTextButton" style="background-image: url(../Images/add.gif);" title="Add New" onclick='customDialog({
            title: "Add",
            height: 75,
            width: 400,
            autoSize: false,
            content: "Sorry. We are in the process of implementing this functionality.",
            dialog_style: "alert"
        }, 
        [{ title: "Ok" }]);'>Add New</div>
	<div class="bottomTextButton" style="background-image: url(../Images/mag_glass.png);" title="Search" onclick='customDialog({
            title: "Search",
            height: 75,
            width: 400,
            autoSize: false,
            content: "Sorry. We are in the process of implementing this functionality.",
            dialog_style: "alert"
        }, 
        [{ title: "Ok" }]);'>Search</div>
	
</div>