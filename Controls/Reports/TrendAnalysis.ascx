<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TrendAnalysis.ascx.cs"
    Inherits="Thinkgate.Controls.Reports.TrendAnalysis" %>
<script type="text/javascript">

</script>
<div class="tblContainer" style="width: 100%; height: 580px;">
    <div class="tblRow" style="height: 100%;">
        <div id="criteriaHolder" class="tblLeft" style="background-color: #d1c9ca; height: 100%;
            padding-top: 3px;">
            <asp:PlaceHolder ID="criteriaPlaceholder" runat="server"></asp:PlaceHolder>
        </div>
        <div id="criteriaScroller" class="tblMiddle" style="width: 10px; height: 100%; vertical-align: top;
            background-color: #CCCCCC;">
            <div id="columnExpanderHandle" onclick="criteriaSliderGo();" style="cursor: pointer;
                height: 100px; background-color: #0F3789; position: relative; top: 42%;">
                <asp:Image runat="server" ID="columnExpanderHandleImage" ClientIDMode="Static" Style="position: relative;
                    left: 1px; top: 40px; width: 8px" ImageUrl="~/Images/arrow_gray_left.gif" />
            </div>
        </div>
        <div class="tblRight" style="width: 100%; vertical-align: top;">
            <div style="width: 100%; height: 100%; overflow: scroll;">
                
            </div>
        </div>
    </div>
</div>
