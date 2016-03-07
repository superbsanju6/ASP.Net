<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradeSubjectStandardsV2.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.GradeSubjectStandardsV2" %>
<style type="text/css">
    .no-close .ui-dialog-titlebar-close {
        display: none;
    }

    .ui-dialog-titlebar {
        display: none;
    }
</style>
<input type="hidden" id="hdvSelectedValue_<%=this.Key %>" />
<div type="CriteriaControl" uitype="<%=this.UIType %>" key="<%=this.Key %>">
   
    <div id="divToolTipTarget_<%=this.Key %>" key="<%=this.Key %>" keyname="<%=this.Key %>Criteria" style="padding: 0px; width: auto;" onmouseover="keepDialog();" onmouseout="closeDialog('divToolTipTarget_<%=Key %>');">
        <table style="width: 98%;">
            <tr>
                <td style="width: 80px;">
                    <span style="font-weight: bold;">Standard&nbsp;Set:</span>
                </td>
                <td>
                    <asp:DropDownList ID="ddlCriteriaStandardSet" runat="server" Style="width: 145px; float: left; margin-left: 3px; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange="PopulateGrade(this); selectStandardsCriteriaValue(this);" onmouseover="keepDialog();" ></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 80px;">
                    <span style="font-weight: bold;">Grade:</span>
                </td>
                <td>

                    <asp:DropDownList ID="ddlCriteriaGrades" runat="server" Style="width: 145px; float: left; margin-left: 3px; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange="PopulateSubject(this); selectStandardsCriteriaValue(this);" onmouseover="keepDialog();" ></asp:DropDownList>
                </td>
            </tr>

            <tr>
                <td style="width: 80px;">
                    <span style="font-weight: bold;">Subject:</span>
                </td>
                <td>

                    <asp:DropDownList ID="ddlCriteriaSubjects" runat="server" Style="width: 145px; float: left; margin-left: 3px; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange="PopulateCourse(this); selectStandardsCriteriaValue(this);" onmouseover="keepDialog();" ></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 80px;">
                    <span style="font-weight: bold;">Course:</span>
                </td>
                <td>
                    <asp:DropDownList ID="ddlCriteriaCourse" runat="server" Style="width: 145px; float: left; margin-left: 3px; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange="PopulateStandards(this); selectStandardsCriteriaValue(this);" onmouseover="keepDialog();" ></asp:DropDownList>

                </td>
            </tr>
            <tr>
                <td style="width: 80px;">
                    <span style="font-weight: bold;">Standards:</span>
                </td>
                <td>
                    <asp:DropDownList ID="ddlCriteriaStandards" runat="server" Style="width: 145px; float: left; margin-left: 3px; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange=" selectStandardsCriteriaValue(this); closeToolTip(this);" onmouseover="keepDialog();" ></asp:DropDownList>

                </td>
            </tr>
        </table>
    </div>
    <% if (this.DefaultValue == null)
       { %>
    <div id="divToolTipTarget_<%=this.Key %>_Selected" style="width: 90%; margin-left: 5%; line-height: 25px; height: auto; float: left; display: none; border: 1px solid black; border-radius: 10px; background-color: transparent; padding-left: 5px; margin-bottom: 2px; margin-top: 2px;">
        <img id="Image1" src="<%: ResolveUrl("~/Images/close_x.png") %>" key="<%=this.Key %>" style="float: left; margin-top: 4px; cursor: pointer;" title="Remove this criteria" onclick="return removeCriteria(this);" />
        <span id="divToolTipTarget_<%=this.Key %>_Selected_Text" style="line-height: 12px; font-size: 8pt; font-style: italic; float: left; margin-left: 3px; margin-top: 3px; width: 85%; overflow: hidden;"></span>
    </div>
    <% }
       else
       { %>
        <div id="divToolTipTarget_<%=this.Key %>_Selected" type="selection" style="width: 90%; margin-left: 5%; line-height: 92px; height: auto; float: left; border: 1px solid black; border-radius: 10px; background-color: transparent; padding-left: 5px; margin-bottom: 2px; margin-top: 2px;">
        <% if (this.Locked)
           { %>
        <img id="Img1" src="<%: ResolveUrl("~/Images/close_x.png") %>" type="tip" key="<%=this.Key %>" style="float: left; margin-top: 4px; cursor: default;" title="Locked" />
        <% }
           else
           { %>
        <img id="Img2" src="<%: ResolveUrl("~/Images/close_x.png") %>" type="tip" key="<%=this.Key %>" style="float: left; margin-top: 4px; cursor: pointer;" title="Remove this criteria" onclick="return removeCriteria(this);" />
        <% } %>
            <span id="divToolTipTarget_<%=this.Key %>_Selected_Text" style="line-height: 12px; font-size: 8pt; font-style: italic; font-weight: bold; float: left; margin-left: 3px; margin-top: 3px; width: 85%; overflow: hidden;"><%=this.DefaultValue %></span>
    </div>
    <%} %>
</div>