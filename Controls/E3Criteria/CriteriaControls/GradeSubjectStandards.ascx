<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradeSubjectStandards.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.GradeSubjectStandards" %>
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
    <div style="margin-top: 1px; height: 25px; background-color: #fff; width: 180px; line-height: 25px; display: block; clear: both; border: 1px solid black; border-top-left-radius: 10px; border-top-right-radius: 30px; border-bottom-left-radius: 30px; border-bottom-right-radius: 10px;">
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
            <span id="divToolTipTarget_<%=this.Key %>_Selected_Text" style="line-height: 12px; font-size: 8pt; font-style: italic; font-weight: bold; float: left; margin-left: 3px; margin-top: 3px; width: 85%; overflow: hidden;"><%=this.DefaultValue.Value %></span>
    </div>
    <%} %>
</div>




        <script type="text/javascript">

            function PopulateGrade(standardElement) {
                //alert(standardElement);
                var standardSet = $(standardElement).val();
               // alert(standardSet);
                var ctl = $("select[id=" + standardElement.id.substr(0, standardElement.id.lastIndexOf("_")) + "_ddlCriteriaGrades]")[0];
                var dllSubject = $("select[id=" + standardElement.id.substr(0, standardElement.id.lastIndexOf("_")) + "_ddlCriteriaSubjects]")[0];
                var dllCourse = $("select[id=" + standardElement.id.substr(0, standardElement.id.lastIndexOf("_")) + "_ddlCriteriaCourse]")[0];
                var dllStandards = $("select[id=" + standardElement.id.substr(0, standardElement.id.lastIndexOf("_")) + "_ddlCriteriaStandards]")[0];
                $(ctl).empty();
                $(dllSubject).empty();
                $(dllCourse).empty();
                $(dllStandards).empty();
                $(ctl).append($('<option></option>').val("0").html("Select Grade"));
                $(dllSubject).append($('<option></option>').val("0").html("Select Subject"));
                $(dllCourse).append($('<option></option>').val("0").html("Select Course"));
                $(dllStandards).append($('<option></option>').val("0").html("Select Standards"));
                
               $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardGrade',
                    data: "{'standardSet':'" + standardSet + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (result) {
                        if (ctl) {
                            var sValue = $("#" + ctl.id).val();
                            $("#" + ctl.id).html(result.d);
                            $("#" + ctl.id).val(sValue);
                        }
                    }
                });
            }


            function PopulateSubject(standardGrade) {
                var standardSetctrl = $("select[id=" + standardGrade.id.substr(0, standardGrade.id.lastIndexOf("_")) + "_ddlCriteriaStandardSet]")[0];
                var standardSet = $(standardSetctrl).val();
               
                var grade = $(standardGrade).val();
                var ctl = $("select[id=" + standardGrade.id.substr(0, standardGrade.id.lastIndexOf("_")) + "_ddlCriteriaSubjects]")[0];
                var dllCourse = $("select[id=" + standardGrade.id.substr(0, standardGrade.id.lastIndexOf("_")) + "_ddlCriteriaCourse]")[0];
                var dllStandards = $("select[id=" + standardGrade.id.substr(0, standardGrade.id.lastIndexOf("_")) + "_ddlCriteriaStandards]")[0];
                $(ctl).empty();
                $(dllCourse).empty();
                $(dllStandards).empty();
                $(ctl).append($('<option></option>').val("0").html("Select Subject"));
                $(dllCourse).append($('<option></option>').val("0").html("Select Course"));
                $(dllStandards).append($('<option></option>').val("0").html("Select Standards"));
                
                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardSubject',
                    data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (result) {
                        if (ctl) {
                            var sValue = $("#" + ctl.id).val();
                            $("#" + ctl.id).html(result.d);
                            $("#" + ctl.id).val(sValue);
                        }
                    }
                });
            }

            function PopulateCourse(standardSubject) {
                //var standardSet = $("#CriteriaStandardSet").val();
                var standardSet = $($("select[id=" + standardSubject.id.substr(0, standardSubject.id.lastIndexOf("_")) + "_ddlCriteriaStandardSet]")[0]).val();
                var grade = $($("select[id=" + standardSubject.id.substr(0, standardSubject.id.lastIndexOf("_")) + "_ddlCriteriaGrades]")[0]).val();
                var subject = $(standardSubject).val();                
                var ctl = $("select[id=" + standardSubject.id.substr(0, standardSubject.id.lastIndexOf("_")) + "_ddlCriteriaCourse]")[0];
                var dllStandards = $("select[id=" + standardSubject.id.substr(0, standardSubject.id.lastIndexOf("_")) + "_ddlCriteriaStandards]")[0];
                $(ctl).empty();
                $(dllStandards).empty();
                $(ctl).append($('<option></option>').val("0").html("Select Course"));
                $(dllStandards).append($('<option></option>').val("0").html("Select Standards"));
                
                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardCourse',
                    data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "','subject':'" + subject + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            },
            success: function (result) {
                if (ctl) {
                    var sValue = $("#" + ctl.id).val();
                    $("#" + ctl.id).html(result.d);
                    $("#" + ctl.id).val(sValue);
                }
            }
        });
    }

            function PopulateStandards(standardCourse) {
                var standardSet = $($("select[id=" + standardCourse.id.substr(0, standardCourse.id.lastIndexOf("_")) + "_ddlCriteriaStandardSet]")[0]).val();
                var grade = $($("select[id=" + standardCourse.id.substr(0, standardCourse.id.lastIndexOf("_")) + "_ddlCriteriaGrades]")[0]).val();
                var subject = $($("select[id=" + standardCourse.id.substr(0, standardCourse.id.lastIndexOf("_")) + "_ddlCriteriaSubjects]")[0]).val();
                var course = $(standardCourse).val();

                var ctl = $("select[id=" + standardCourse.id.substr(0, standardCourse.id.lastIndexOf("_")) + "_ddlCriteriaStandards]")[0];
                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardsByStandardSetGradeSubjectCourse',
                    data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "','subject':'" + subject + "','course':'" + course + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(errorThrown);
                    },
                    success: function (result) {
                        if (ctl) {
                            var sValue = $("#" + ctl.id).val();
                            $("#" + ctl.id).html(result.d);
                            $("#" + ctl.id).val(sValue);
                        }
                    }
                });
    }

        </script>