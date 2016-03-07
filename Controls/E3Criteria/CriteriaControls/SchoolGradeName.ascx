<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchoolGradeName.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.SchoolGradeName" %>
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
    <div  id="divSchoolGradeName" style="margin-top: 1px; height: 25px; background-color: #fff; width: 180px; line-height: 25px; display: block; clear: both; border: 1px solid black; border-top-left-radius: 10px; border-top-right-radius: 30px; border-bottom-left-radius: 30px; border-bottom-right-radius: 10px;">
        <div style="margin-left: 10px; line-height: 20px; height: 20px;">
            <div style="float: left; width: 80px;">
                <asp:Label ID="CriteriaHeaderText" runat="server" Text=""></asp:Label>
                <asp:Label ID="RequiredCriteriaIndicator" runat="server" Text="" Style="width: 2px; line-height: 20px;"></asp:Label>
            </div>
            <% if (this.Locked)
               { %>
            <div id="divToolTipSource_<%=this.Key %>" style="padding: 0px; margin-left: 130px; margin-top: 2px; line-height: 30px; border: 0px solid currentColor; cursor: default;" title="Locked">
                <img type="imgtooltipImage" src='<%: ResolveUrl("~/Images/expand_bubble.png") %>' alt="" style="cursor: default;" title="Locked" />
            </div>
            <% }
               else
               { %>
            <div id="divToolTipSource_<%=this.Key %>" style="padding: 0px; margin-left: 130px; margin-top: 2px; line-height: 30px; border: 0px solid currentColor; cursor: pointer;" onclick="showTooltip(this, 'divToolTipTarget_<%=this.Key%>');">
                <img type="imgtooltipImage" src='<%: ResolveUrl("~/Images/expand_bubble.png") %>' alt="" style="cursor: pointer;" />
            </div>
            <% } %>
        </div>
    </div>
    <div id="divToolTipTarget_<%=this.Key %>" key="<%=this.Key %>" keyname="<%=this.Key %>Criteria" style="padding: 0px; display: none; width: auto;" onmouseover="keepDialog();" onmouseout="closeDialog('divToolTipTarget_<%=this.Key %>');">
        <table style="width: 98%;">
            <tr>
                <td style="width: 80px;">
                    <span style="font-weight: bold;">School:</span>
                </td>
                <td>
                    <%--<asp:DropDownList ID="ddlCriteriaStandardSet" runat="server" Style="width: 145px; float: left; margin-left: 3px; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange="PopulateGrade(this); selectStandardsCriteriaValue(this);" onmouseover="keepDialog();" ></asp:DropDownList>--%>
                    <asp:DropDownList ID="ddlCriteriaSchool" runat="server" Style="width: 145px; float: left; margin-left: 3px; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange="PopulateSchoolGrade(this); selectTeacherCriteriaValue(this);" onmouseover="keepDialog();" ></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 80px;">
                    <span style="font-weight: bold;">Grade:</span>
                </td>
                <td>
                    <asp:DropDownList ID="ddlCriteriaGrades" runat="server" Style="width: 145px; float: left; margin-left: 3px; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange="PopulateGradName(this); selectTeacherCriteriaValue(this);" onmouseover="keepDialog();" ></asp:DropDownList>
                </td>
            </tr>

            <tr>
                <td style="width: 80px;">
                    <span style="font-weight: bold;">Name:</span>
                </td>
                <td>
                    <%-- <asp:DropDownList ID="ddlCriteriaSubjects" runat="server" Style="width: 145px; float: left; margin-left: 3px; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange="PopulateCourse(this); selectStandardsCriteriaValue(this);" onmouseover="keepDialog();" ></asp:DropDownList>--%>
                    <asp:DropDownList ID="ddlCriteriaNames" runat="server" Style="width: 145px; float: left; margin-left: 3px; font-size: 12px; font-family: Arial, 'Trebuchet MS';" onchange="selectTeacherCriteriaValue(this);" onmouseover="keepDialog();" ></asp:DropDownList>
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
            <span id="divToolTipTarget_<%=this.Key %>_Selected_Text" style="line-height: 12px; font-size: 8pt; font-style: italic; font-weight: bold; float: left; margin-left: 3px; margin-top: 3px; width: 85%; overflow: hidden;"><%=this.DefaultValue.Value %></span>
    </div>
    <%} %>
</div>

        <script type="text/javascript">            

            function PopulateSchoolGrade(standardSchool) {

                var schoolSet = $(standardSchool).val();
                $("select[id=" + standardSchool.id.substr(0, standardSchool.id.lastIndexOf("_")) + "_ddlCriteriaGrades]").attr("disabled", "disabled");

                var ctl = $("select[id=" + standardSchool.id.substr(0, standardSchool.id.lastIndexOf("_")) + "_ddlCriteriaGrades]")[0];
                var dllName = $("select[id=" + standardSchool.id.substr(0, standardSchool.id.lastIndexOf("_")) + "_ddlCriteriaNames]")[0];
                $(ctl).empty();
                $(dllName).empty();

                var sValue = $("#" + ctl.id).val();
                var gValue = $("#" + dllName.id).val();

                $(ctl).append($('<option></option>').val("0").html("Select Grade"));
                $(dllName).append($('<option></option>').val("0").html("Select Name"));

                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetSchoolGrade',
                    data: "{'schoolSet':'" + schoolSet + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (result) {
                        var gradeResult = result.d;
                        if (result.d.indexOf("$") >= 0) {
                            //split the result 
                            var gradeResult = result.d.split('$')[0];
                            var schoolResult = result.d.split('$')[1];
                        }

                        if (ctl) {
                            if (gradeResult.split('</option>').length > 2) {
                                $("#" + ctl.id).html(gradeResult);
                                $("#" + ctl.id).val(sValue);

                                selectTeacherCriteriaValue(ctl);
                            } else if (dllName) {
                                $("#" + ctl.id).html(gradeResult);
                                $("#" + ctl.id).val(sValue);

                                $("#" + dllName.id).html(schoolResult);
                                $("#" + dllName.id).val(gValue);
                              
                                selectTeacherCriteriaValue(dllName);

                            }
                        }
                        $("select[id=" + standardSchool.id.substr(0, standardSchool.id.lastIndexOf("_")) + "_ddlCriteriaGrades]").removeAttr("disabled");
                    }
                });
            }

            function PopulateGradName(standardGrade) {

                var gradeSet = $(standardGrade).val();
                $("select[id=" + standardGrade.id.substr(0, standardGrade.id.lastIndexOf("_")) + "_ddlCriteriaNames]").attr("disabled", "disabled");
                var dllName = $("select[id=" + standardGrade.id.substr(0, standardGrade.id.lastIndexOf("_")) + "_ddlCriteriaNames]")[0];
                var dllSchool = $("select[id=" + standardGrade.id.substr(0, standardGrade.id.lastIndexOf("_")) + "_ddlCriteriaSchool]")[0];

                $(dllName).empty();
                $(dllName).append($('<option></option>').val("0").html("Select Name"));

                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetSchoolGradeName',
                    data: "{'gradeSet':'" + gradeSet + "','schoolSet':'" + $("#" + dllSchool.id).val() + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (result) {                        
                            if (dllName) {
                                var sValue = $("#" + dllName.id).val();
                                $("#" + dllName.id).html(result.d);
                                $("#" + dllName.id).val(sValue);
                            }
                            if (result.d.split('</option>').length==2)
                            {
                                selectTeacherCriteriaValue(dllName);
                            }
                            $("select[id=" + standardGrade.id.substr(0, standardGrade.id.lastIndexOf("_")) + "_ddlCriteriaNames]").removeAttr("disabled");
                    }                    
                });
            }
        </script>