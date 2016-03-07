<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradeSubjectCurriculum.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.GradeSubjectCurriculum" %>
<style type="text/css">
    .no-close .ui-dialog-titlebar-close {
        display: none;
    }
    .ui-dialog-titlebar {
        display: none;
    }
</style>
<input type="hidden" id="hdvSelectedValue_<%=this.Key %>" />
<div type="CriteriaControl" UIType="<%=this.UIType %>" key="<%=this.Key %>">
    <div style="margin-top: 1px; height: 25px; background-color: #fff; width: 180px; line-height: 25px; display: block; clear: both; border: 1px solid black; border-top-left-radius: 10px; border-top-right-radius: 30px; border-bottom-left-radius: 30px; border-bottom-right-radius: 10px;">
        <div style="margin-left: 10px; line-height: 20px; height: 20px;">
            <div style="float: left; width: 80px;">
                <asp:Label ID="CriteriaHeaderText" runat="server" Text=""></asp:Label>
                <asp:Label ID="RequiredCriteriaIndicator" runat="server" Text="" style="width: 2px; line-height: 20px;"></asp:Label>
            </div>
            <% if (this.Locked) { %>
                    <div id="divToolTipSource_<%=this.Key %>" style="padding: 0px; margin-left: 130px; margin-top: 2px; line-height: 30px; border: 0px solid currentColor; cursor: default;" title="Locked">
                        <img type="imgtooltipImage" src='<%: ResolveUrl("~/Images/expand_bubble.png") %>' alt="" style="cursor: default;" title="Locked" />
                    </div>
            <% } else { %>
                    <div id="divToolTipSource_<%=this.Key %>" style="padding: 0px; margin-left: 130px; margin-top: 2px; line-height: 30px; border: 0px solid currentColor; cursor: pointer;" onclick="showTooltip(this, 'divToolTipTarget_<%=this.Key%>');">
                        <img type="imgtooltipImage" src='<%: ResolveUrl("~/Images/expand_bubble.png") %>' alt="" style="cursor: pointer;" />
                    </div>
            <% } %>
        </div>
    </div>
    <div id="divToolTipTarget_<%=this.Key %>" key="<%=this.Key %>" keyname="<%=this.Key %>Criteria" style="padding: 0px; display: none; height: auto; width: auto;" onmouseover="keepDialog();" onmouseout="closeDialog('divToolTipTarget_<%=this.Key %>');">
        <table style="width: 95%;">
            <tr>
                <td style="width: 60px;">
                    <span style="font-weight: bold;">Grade:</span>
                </td>
                <td>
                    <asp:DropDownList ID="RES_GSC_CriteriaGrades" runat="server" Style="width: 150px; float: left; margin-left: 3px; font-size: 12px; font-family: 'Segoe UI', Arial, sans-serif;" onchange="populateSubjects(this); selectCurriculumsCriteriaValue(this);"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 60px;">
                    <span style="font-weight: bold;">Subject:</span>
                </td>
                <td>
                    <asp:DropDownList ID="RES_GSC_CriteriaSubjects" runat="server" Style="width: 150px; float: left; margin-left: 3px; font-size: 12px; font-family: 'Segoe UI', Arial, sans-serif;" onchange="populateCurrCourses(this); selectCurriculumsCriteriaValue(this);" ></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 60px;">
                    <span style="font-weight: bold;">Curriculum:</span>
                </td>
                <td>
                    <asp:DropDownList ID="RES_GSC_CriteriaCurricula" runat="server" Height="20" Style="width: 150px; float: left; margin-left: 3px; font-size: 12px; font-family: 'Segoe UI', Arial, sans-serif;" onchange="selectCurriculumsCriteriaValue(this); closeToolTip(this);" ></asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
    <% if (this.DefaultValue == null) { %>
            <div id="divToolTipTarget_<%=this.Key %>_Selected" style="width: 90%; margin-left: 5%; line-height: 25px; height: auto; float: left; display: none; border: 1px solid black; border-radius: 10px; background-color: transparent; padding-left: 5px; margin-bottom: 2px; margin-top: 2px;">
                <img id="Image1" src="<%: ResolveUrl("~/Images/close_x.png") %>" key="<%=this.Key %>" style="float: left; margin-top: 4px; cursor: pointer;" title="Remove this criteria" onclick="return removeCriteria(this);" />
                <span id="divToolTipTarget_<%=this.Key %>_Selected_Text" style="line-height: 12px; font-size: 8pt; font-style: italic; float: left; margin-left: 3px; margin-top: 3px; width: 85%; overflow: hidden;"></span>
            </div>
    <% } else { %>
            <div id="divToolTipTarget_<%=this.Key %>_Selected" type="selection" style="width: 90%; margin-left: 5%; line-height: 25px; height: auto; float: left; border: 1px solid black; border-radius: 10px; background-color: transparent; padding-left: 5px; margin-bottom: 2px; margin-top: 2px;">
                <% if (this.Locked) { %>
                    <img id="Img1" src="<%: ResolveUrl("~/Images/close_x.png") %>" type="tip" key="<%=this.Key %>" style="float: left; margin-top: 4px; cursor: default;" title="Locked" />
                <% } else { %>
                    <img id="Img2" src="<%: ResolveUrl("~/Images/close_x.png") %>" type="tip" key="<%=this.Key %>" style="float: left; margin-top: 4px; cursor: pointer;" title="Remove this criteria" onclick="return removeCriteria(this);" />
                <% } %>
                <span id="divToolTipTarget_<%=this.Key %>_Selected_Text" style="line-height: 12px; font-size: 8pt; font-style: italic; font-weight: bold; float: left; margin-left: 3px; margin-top: 3px; width: 85%; overflow: hidden;"><%=this.DefaultValue.Value %></span>
            </div>
    <%} %>
</div>
<script type="text/javascript">

    function selectGSCCriteria(ddl, type) {
        var criteriaName = $(ddl).parents("div[keyName*=Criteria]").attr("key");
        dialogTarget = $("#divToolTipTarget_" + criteriaName);
        var dialogSelected = $("#divToolTipTarget_" + criteriaName + '_Selected');
        var dialogSelectedText = $("#divToolTipTarget_" + criteriaName + '_Selected_Text');

        var criteria = getCriteriaByKey(criteriaName);
        if (criteria) {
            if (criteria.UIType == '7') {

                criteria.Value.Key = '';
                criteria.Value.Value = '';

                var arr = [];
                var displayText = "";
                var valueText = "";
                dialogTarget.find("select").each(function (i, e) {
                    var selValue = $(e).find("option:selected").val();
                    var selText = $(e).find("option:selected").text();
                    arr.push({ "Key": selValue, "Value": selText });
                });
                
                var heightLevel = 0;
                var dynamicHeight = 0;

                if (arr[0].Key != '0') {
                    valueText = arr[0].Key + ",";
                    displayText = "Grade:   " + arr[0].Value + "<br/>";
                    heightLevel += 1;
                }
                else {
                    valueText = ",";
                    displayText = "";
                }
                
                if (arr[1].Key != '0') {
                    valueText += arr[1].Key + ",";
                    displayText += "Subject: " + arr[1].Value + "<br/>";
                    heightLevel += 1;
                }
                else {
                    valueText += ",";
                    displayText += "";
                }
                
                if (arr[2].Key != '0') {
                    valueText += arr[2].Key + ",";
                    displayText += "Curriculum:  " + arr[2].Value;
                    heightLevel += 1;
                }
                else {
                    valueText += ",";
                    displayText += "";
                }
                
                if (heightLevel == 1)
                    dynamicHeight = 25;
                else if (heightLevel == 2)
                    dynamicHeight = 40;
                else if (heightLevel == 3)
                    dynamicHeight = 58;
                else if (heightLevel == 0)
                    dynamicHeight = 58;
                
                if (heightLevel > 0) {
                    criteria.Value.Key = valueText;
                    criteria.Value.Value = 'Multiple';

                    $(dialogSelectedText).html(displayText);
                    $(dialogSelected).css({ "height": dynamicHeight }).show("fast");
                    $(dialogSelectedText).show("fast");
                }
                else {
                    criteria.Value.Key = "";
                    criteria.Value.Value = "";
                    dialogSelected.hide("fast");
                }
                $("#hdnSearchControlSchema").val(JSON.stringify(searchControlSchema));
            }
        }
    }

    function populateSubjects(gradeElement) {
        var grade = $(gradeElement).val();
        var ctl = $("select[id*=RES_GSC_CriteriaSubjects]")[0];
        var curr = $("select[id*=RES_GSC_CriteriaCurricula]")[0];
        $(ctl).empty();
        $(curr).empty();
        $(ctl).append($('<option></option>').val("0").html("Select Subject"));
        $(curr).append($('<option></option>').val("0").html("Select Curriculum"));
         
        $.ajax({
            type: "POST",
            url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/getSubjects',
            data: "{'grade':'" + grade + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(errorThrown);
                },
                success: function (result) {
                    if (ctl) {
                        $("#" + ctl.id).html(result.d);
                        //selectGSCCriteria($("select[id*=RES_GSC_CriteriaGrades]")[0], 'G');
                        populateCurrCourses(ctl);
                    }
                }
            });
    }

    function populateCurrCourses(subjectElement) {
        var grade = $("select[id*=RES_GSC_CriteriaGrades]").val();
        var subject = $(subjectElement).val();
        var ctl = $("select[id*=RES_GSC_CriteriaCurricula]")[0];
        $(ctl).empty();
        $(ctl).append($('<option></option>').val("0").html("Select Curriculum"));
        
        $.ajax({
            type: "POST",
            url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/getCurrCourses',
            data: "{'grade':'" + grade + "', 'subject':'" + subject + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //alert(errorThrown);
            },
            success: function (result) {
                if (ctl) {
                    $("#" + ctl.id).html(result.d);
                    //selectGSCCriteria($("select[id*=RES_GSC_CriteriaSubjects]")[0], 'S');
                }
            }
        });
    }

</script>