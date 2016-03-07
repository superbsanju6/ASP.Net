<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CheckboxList.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.CheckboxList" %>
<style type="text/css">
    .no-close .ui-dialog-titlebar-close {
        display: none;
    }
    .ui-dialog-titlebar {
        display: none;
    }
    .chkboxList {
    margin-top: 1px;height: 24px; width: 180px; line-height: 24px; display: block; clear: both; border: 1px solid black; border-top-left-radius: 10px; border-top-right-radius: 30px; border-bottom-left-radius: 30px; border-bottom-right-radius: 10px;
    }
    @-moz-document url-prefix() {
   .chkboxList { height: 23px;line-height:23px }
}
</style>
<input type="hidden" id="hdvSelectedValue_<%=this.Key %>" />
<div type="CriteriaControl" UIType="<%=this.UIType %>" key="<%=this.Key %>">
    <div class="chkboxList">
        <div style="margin-left: 10px; line-height: 20px; height: 20px;">
            <div style="float: left; width: 95px;">
                <asp:Label ID="CriteriaHeaderText" runat="server" Text=""></asp:Label>
                <asp:Label ID="RequiredCriteriaIndicator" runat="server" Text="" style="width: 2px; line-height: 20px;"></asp:Label>
            </div>
            <% if (this.Locked) { %>
                    <div id="divToolTipSource_<%=this.Key %>" style="margin-left: 130px; margin-top: 2px; line-height: 30px; border: 0px solid currentColor; cursor: default;" title="Locked">
                        <img type="imgtooltipImage" src='<%: ResolveUrl("~/Images/expand_bubble.png") %>' alt="" style="cursor: default;" title="Locked" />
                    </div>
            <% } else { %>
                    <div id="divToolTipSource_<%=this.Key %>" style="margin-left: 130px; margin-top: 2px; line-height: 30px; border: 0px solid currentColor; cursor: pointer;" onclick="showTooltip(this, 'divToolTipTarget_<%=this.Key%>');">
                        <img type="imgtooltipImage" src='<%: ResolveUrl("~/Images/expand_bubble.png") %>' alt="" style="cursor: pointer;" />
                    </div>
            <% } %>
        </div>
    </div>
    <div id="divToolTipTarget_<%=this.Key %>" key="<%=this.Key %>" keyname="<%=this.Key %>Criteria" style="padding: 0px; display: none; height: auto; width: auto;" onmouseover="keepDialog();" onmouseout="closeDialog('divToolTipTarget_<%=this.Key %>');">
        <asp:CheckBoxList ID="chk" runat="server" BorderWidth="0" Style="width: auto; height: auto; float: left; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange="selectCriteriaValue(this);"></asp:CheckBoxList>
    </div>
    <% if (this.DefaultValue == null) { %>
            <div id="divToolTipTarget_<%=this.Key %>_Selected" style="width: 90%; margin-left: 5%; line-height: 25px; height: auto; float: left; display: none; border: 1px solid black; border-radius: 10px; background-color: transparent; padding-left: 5px; margin-bottom: 2px; margin-top: 2px;">
                <img id="Image1" src="<%: ResolveUrl("~/Images/close_x.png") %>" key="<%=this.Key %>" style="float: left; margin-top: 4px; cursor: pointer;" title="Remove this criteria" onclick="return removeCriteria(this);" />
                <span id="divToolTipTarget_<%=this.Key %>_Selected_Text" style="line-height: 12px; font-style: italic; height: auto; float: left; margin-left: 2px; width: 85%; overflow: hidden;"></span>
            </div>
    <% } else { %>
            <div id="divToolTipTarget_<%=this.Key %>_Selected" type="selection" style="width: 90%; margin-left: 5%; line-height: 25px; height: auto; float: left; border: 1px solid black; border-radius: 10px; background-color: transparent; padding-left: 5px; margin-bottom: 2px; margin-top: 2px;">
                <% if (this.Locked) { %>
                    <img id="Img1" src="<%: ResolveUrl("~/Images/close_x.png") %>" type="tip" key="<%=this.Key %>" style="float: left; margin-top: 4px; cursor: default;" title="Locked" />
                <% } else { %>
                    <img id="Img2" src="<%: ResolveUrl("~/Images/close_x.png") %>" type="tip" key="<%=this.Key %>" style="float: left; margin-top: 4px; cursor: pointer;" title="Remove this criteria" onclick="return removeCriteria(this);" />
                <% } %>
                <span id="divToolTipTarget_<%=this.Key %>_Selected_Text" style="line-height: 12px; font-style: italic; font-weight: bold; height: auto; float: left; margin-left: 2px; width: 85%; overflow: hidden;"><%=this.DefaultValue.Value %></span>
            </div>
    <%} %>
</div>
