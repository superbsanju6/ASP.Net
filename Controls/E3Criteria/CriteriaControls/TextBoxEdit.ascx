﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextBoxEdit.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.TextBoxEdit" %>
<style type="text/css">
    .no-close .ui-dialog-titlebar-close {
        display: none;
    }
     ::-ms-clear {
        display: none;
    }

    .ui-dialog-titlebar {
        display: none;
    }

 .selectedNew {
    width: 90%; margin-left: 5%; line-height: 25px; min-height: 25px; height: auto; float: left; display: none; border: 1px solid #000000!important; border-radius: 10px; background-color: transparent; padding-left: 5px; margin-bottom: 2px; margin-top: 2px;
    }
    .selectNew2 {
    padding: 0; margin-left: 130px; margin-top: 2px; line-height: 30px; border: 0 solid currentColor; cursor: default;
    }
    .selectNew3 {
    padding: 0; margin-left: 130px; margin-top: 2px; line-height: 30px; border: 0 solid currentColor; cursor: pointer;
    }
    .selectNew4 {
    line-height: 25px; padding: 0; display: none;
    }

</style>
<input type="hidden" id="hdvSelectedValue_<%=Key %>" />
<div type="CriteriaControl" UIType="<%=UIType %>" key="<%=Key %>">
    <div class="chkboxList">
        <div style="margin-left: 10px; line-height: 20px; height: 20px;">
            <div style="float: left; width: 80px;">
                <asp:Label ID="CriteriaHeaderText" runat="server" Text=""></asp:Label>
                <asp:Label ID="RequiredCriteriaIndicator" runat="server" Text="" style="width: 2px; line-height: 20px;"></asp:Label>
            </div>
            <% if (Locked) { %>
                    <div id="divToolTipSource_<%=Key %>" class="selectNew2" title="Locked">
                        <img type="imgtooltipImage" src='<%: ResolveUrl("~/Images/expand_bubble.png") %>' alt="" style="cursor: default;" title="Locked" />
                    </div>
            <% } else { %>
                    <div id="divToolTipSource_<%=Key %>" class="selectNew3" onclick="showTooltip(this, 'divToolTipTarget_<%=Key%>');">
                        <img type="imgtooltipImage" src='<%: ResolveUrl("~/Images/expand_bubble.png") %>' alt="" style="cursor: pointer; " />
                    </div>
            <% } %>
        </div>
    </div>
    <div id="divToolTipTarget_<%=Key %>" key="<%=Key %>" keyname="<%=Key %>Criteria" style="padding-left: 5px; padding-top: 5px;" class="selectNew4" onmouseover="keepDialog();" onmouseout="closeDialog('divToolTipTarget_<%=Key %>');">
        <asp:TextBox ID="txt" runat="server"  Text="" style="font-size: 12px; font-family: Arial, 'Trebuchet MS';" onblur="closeToolTip(this);" onmouseover="keepDialog();"></asp:TextBox>
 
    </div>
    <% if (DefaultValue == null) { %>
            <div id="divToolTipTarget_<%=Key %>_Selected" class="selectedNew" style="display:none;" >
                <img id="Image1" src="<%: ResolveUrl("~/Images/close_x.png") %>" style="float:left; cursor:pointer; margin-top:4px;" key="<%=Key %>"  title="Remove this criteria" onclick="return removeCriteria(this);" />
                <span id="divToolTipTarget_<%=Key %>_Selected_Text" style="line-height: 12px; font-style:italic; height: auto; float: left;"></span>
            </div>
    <% } else { %>
            <div id="divToolTipTarget_<%=Key %>_Selected" type="selection" class="selectedNew"  >
                <% if (Locked) { %>
                    <img id="Img1" src="<%: ResolveUrl("~/Images/close_x.png") %>" type="tip" key="<%=Key %>" style="float: left; margin-top: 4px; cursor: default;" title="Locked" />
                <% } else { %>
                    <img id="Img2" src="<%: ResolveUrl("~/Images/close_x.png") %>" type="tip" key="<%=Key %>" style="float: left; margin-top: 4px; cursor: pointer;" title="Remove this criteria" onclick="return removeCriteria(this);" />
                <% } %>
                <span id="divToolTipTarget_<%=Key %>_Selected_Text" style="line-height: 12px; font-style: italic; font-weight: bold; height: auto; float: left; margin-left: 2px; width: 85%; overflow: hidden;"><%=DefaultValue.Value %></span>
            </div>
    <%} %>
</div>


