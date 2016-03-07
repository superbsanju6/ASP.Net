<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardGradeSubjectId.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.StandardGradeSubjectId" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx"  %>
<e3:CriteriaHeader ID="CriteriaHeader" runat="server"/>
<style>
    .ControlClass {
        width: 145px; 
        float: left; 
        margin-left: 3px; 
        font-size: 12px; 
        font-family: Arial, 'Trebuchet MS';
    }
</style>
<telerik:RadToolTip ID="RadToolTip1" runat="server" Width="320" Skin="Black" EnableShadow="True" ShowEvent="OnClick" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <div style="position: relative">
        <div style="width: 300px;">
            <div style="width: 280px; float: left;"> 
                <table style="width: 98%;">
                            <tr>
                                <td style="width: 80px;">
                                    <span style="font-weight: bold;">Standard&nbsp;Set:</span>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlCriteriaStandardSet" runat="server" CssClass="ControlClass"  ></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 80px;">
                                    <span style="font-weight: bold;">Grade:</span>
                                </td>
                                <td>

                                    <select ID="ddlCriteriaGrades" class="ControlClass" onchange="<%=CriteriaName + "Controller.OnChange();" %>" >
                                        <option Value="0">Select Grade</option>
                                    </select>
                                </td>
                            </tr>

                            <tr>
                                <td style="width: 80px;">
                                    <span style="font-weight: bold;">Subject:</span>
                                </td>
                                <td>

                                    <select ID="ddlCriteriaSubjects" class="ControlClass" onchange="<%=CriteriaName + "Controller.OnChange();" %>">
                                        <option Value="0">Select Subject</option>
                                    </select>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 80px;">
                                    <span style="font-weight: bold;">Course:</span>
                                </td>
                                <td>
                                    <select ID="ddlCriteriaCourse" class="ControlClass" onchange="<%=CriteriaName + "Controller.OnChange();" %>">
                                        <option Value="0">Select Course</option>
                                    </select>

                                </td>
                            </tr>
                            <tr>
                                <td style="width: 80px;">
                                    <span style="font-weight: bold;">Standards:</span>
                                </td>
                                <td>
                                    <select ID="ddlCriteriaStandards" class="ControlClass" onchange="<%=CriteriaName + "Controller.OnChange();" %>">
                                        <option Value="0">Select Standard</option>
                                    </select>

                                </td>
                            </tr>
                        </table>
                </div>
            </div>
        </div>
    </telerik:RadToolTip>
<script type="text/javascript">

    var <%=CriteriaName%>Controller = {
        OnChange: function (sender, args) {
            if (sender != null)
                if (sender.get_attributes) {
                    if (sender.get_attributes().getAttribute("SuppressNextChangeEvent") == "true") {
                        // we are here just because we've cleared the dropdown and it fired the updated event. Suppress it
                        sender.get_attributes().setAttribute("SuppressNextChangeEvent", "false");
                        return;
                    }
                }
            var criteriaName = "<%=CriteriaName%>";
            var standardSet = $('#<%= ddlCriteriaStandardSet.ClientID %>');
            var grades = $('#ddlCriteriaGrades');
            var subjects = $('#ddlCriteriaSubjects');
            var course = $('#ddlCriteriaCourse');
            var standards = $('#ddlCriteriaStandards');
            var valueObject = {};
            valueObject.StandardSet = standardSet.val();
            valueObject.Grades = grades.val();
            valueObject.Subjects = subjects.val();
            valueObject.Courses = course.val();
            valueObject.Standard = standards.find('option:selected').text();
            valueObject.StandardId = standards.val();

            if (standardSet.val() == "0" && grades.val() == "0" && subjects.val() == "0" && course.val() == "0" && standards.val() == "0") {
                // we've cleared the text box, clear them
                CriteriaController.RemoveAll(criteriaName);
            } else {
                CriteriaController.Add(criteriaName, valueObject);
                var target = event.target || event.srcElement;
                switch(target.id) {
                    case standardSet.attr('id'):
                        <%=CriteriaName%>Controller.PopulateGrade(standardSet);
                        break;
                    case grades.attr('id'):
                        <%=CriteriaName%>Controller.PopulateSubject(grades);
                        break;
                    case subjects.attr('id'):
                        <%=CriteriaName%>Controller.PopulateCourse(subjects);
                        break;
                    case course.attr('id'):
                        <%=CriteriaName%>Controller.PopulateStandards(course);
                        break;
                }               
            }
        },

        RemoveByKeyHandler: function (criteriaName, value, calledFromAdd) {
            if (calledFromAdd) return;      // if we're calling this from an add, that means essentially we've changed the text or option. If I were to continue, it would clear the text box and dropdown which is not what I want because we obviously have a valid value in addition to the old removed one
            if (CriteriaController.CountValues(criteriaName) > 0) return;       // if we definitely have multiple values, again, we don't want to clear the text box and dropdown
            var standardSet = $('#<%= ddlCriteriaStandardSet.ClientID %>');
            var grades = $('#ddlCriteriaGrades');
            var subjects = $('#ddlCriteriaSubjects');
            var course = $('#ddlCriteriaCourse');
            var standards = $('#ddlCriteriaStandards');
            standardSet.val = "0";
            grades.val = "0";
            subjects.val = "0";
            course.val = "0";
            standards.val = "0";
        },
       PopulateGrade: function (standardElement) {
    
           var standardSet = $(standardElement).val();
           var ctl = $('#ddlCriteriaGrades');
           var dllSubject = $('#ddlCriteriaSubjects');
           var dllCourse = $('#ddlCriteriaCourse');
           var dllStandards = $('#ddlCriteriaStandards');
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
                                    var sValue = ctl.val();
                                    ctl.html(result.d);
                                    ctl.val(sValue);
                                }
                            }
                       });
        },


       PopulateSubject: function (standardGrade) {
                    var standardSetctrl = $('#<%= ddlCriteriaStandardSet.ClientID %>');
                    var standardSet = $(standardSetctrl).val();
               
                    var grade = $(standardGrade).val();
                    var ctl = $('#ddlCriteriaSubjects');
                    var dllCourse = $('#ddlCriteriaCourse');
                    var dllStandards = $('#ddlCriteriaStandards');
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
                            var sValue = ctl.val();
                            ctl.html(result.d);
                            ctl.val(sValue);
                        }
                    }
                });
        },

        PopulateCourse:   function (standardSubject) {
            var standardSet = $('#<%= ddlCriteriaStandardSet.ClientID %>').val();
            var grade = $('#ddlCriteriaGrades').val();
                var subject = $(standardSubject).val();                
                var ctl = $('#ddlCriteriaCourse');
                var dllStandards = $('#ddlCriteriaStandards');
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
                            var sValue = ctl.val();
                            ctl.html(result.d);
                            ctl.val(sValue);
                        }
                    }
                });
            },

         PopulateStandards:   function (standardCourse) {
             var standardSet = $('#<%= ddlCriteriaStandardSet.ClientID %>').val();
             var grade = $('#ddlCriteriaGrades').val();
             var subject = $('#ddlCriteriaSubjects').val();
             var course = $(standardCourse).val();

             var ctl = $('#ddlCriteriaStandards');
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
                            var sValue = ctl.val();
                            ctl.html(result.d);
                            ctl.val(sValue);
                        }
                    }
                });
            }
    };

</script>
<script id="<%=CriteriaName%>" type="text/x-jsrender">   
   {{for Values}}
     <div class="{{:~getCSS(Applied, CurrentlySelected)}}">      
            <div class="imgBeforeCriteria" onclick="CriteriaController.RemoveByKey('<%=CriteriaName%>', {{:Key}})"/>
                    <div class="criteriaText" style="font-size: 8pt">Standard Set: {{:Value.StandardSet}}</div>
                    {{if Value.Grades != 0 }}<br /><div class="criteriaText" style="font-size: 8pt">Grade: {{:Value.Grades}}</div>{{/if}}
                    {{if Value.Subjects != 0 }}<br /><div class="criteriaText" style="font-size: 8pt">Subject: {{:Value.Subjects}}</div>{{/if}}
                    {{if Value.Courses != 0 }}<br /><div class="criteriaText" style="font-size: 8pt">Course: {{:Value.Courses}}</div>{{/if}}
                    {{if Value.StandardId != 0 }}<br /><div class="criteriaText" style="font-size: 8pt">Standards: {{:Value.Standard}}</div>{{/if}}
     </div>
   {{/for}}    
</script>