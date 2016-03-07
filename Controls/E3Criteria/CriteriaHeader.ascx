<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CriteriaHeader.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaHeader" %>

<div style="float: left; margin-left: 3px">
    <div id="criteriaHeaderDiv_<%=CriteriaName%>" class="criteriaHeaderDiv">
        <div class="left"><%=Text%>:<span runat="server" ID="RequiredSpan" style="color: rgb(255, 0, 0); font-weight: bold;">*</span></div>
        
        <div style="overflow: hidden;" class="right" id="expand_bubble" runat="server"><img id ="imgageCtrl" runat ="server" style="width: 16px; height: 16px;" src="~/Images/commands/expand_bubble.png"></div>
    </div>
    <div id="selectedCritieriaDisplayArea_<%=CriteriaName%>" class="selectedCritieriaDisplayArea"></div>
</div>

<script id="DefaultCriteriaValueDisplayTemplate" type="text/x-jsrender">
    {{for Values}}
        <div class="{{:~getCSS(Applied, CurrentlySelected)}}">
            <div style="height:16px;width:16px; position: static !important;" class="imgBeforeCriteria" onclick="CriteriaController.RemoveByKey('{{:#parent.parent.data.CriteriaName}}', {{:Key}})"/>
            <div class="criteriaText" style="position: static !important;">{{:Value.Text}}</div>
        </div>
    {{/for}}
</script>

