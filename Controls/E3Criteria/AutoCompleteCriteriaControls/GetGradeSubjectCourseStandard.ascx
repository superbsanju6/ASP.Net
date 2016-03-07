<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GetGradeSubjectCourseStandard.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.AutoCompleteCriteriaControls.GetGradeSubjectCourseStandard" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx" %>
<e3:CriteriaHeader ID="CriteriaHeader" runat="server" />
<style>
    .ControlClass {
        width: 145px;
        float: left;
        margin-left: 3px;
        font-size: 12px;
        font-family: Arial, 'Trebuchet MS';
    }
    .marginLeft
    {
        margin-left:18px !important;
    }

   .rcbWidth
{
    max-height:150px !important;
}
   

     #RadToolTipWrapper_ctl00_LeftColumnContentPlaceHolder_cmbStandardSet_RadToolTip1
        {
        height:300px !important;
    }
</style>
<telerik:RadToolTip ID="RadToolTip1" runat="server" Width="320" Skin="Black" EnableShadow="True" ShowEvent="OnClick"  AutoCloseDelay="200000" Position="MiddleRight" RelativeTo="Element">
    <div style="position: relative">
        <div style="width: 300px;">
            <div style="width: 280px;  float: left;">
                <table style="width: 98%;">
                    <tr>
                        <td style="width: 80px;">
                            <span style="font-weight: bold;">Standard&nbsp;Set:</span>
                        </td>
                        <td>

                            <telerik:RadComboBox runat="server" ID="ddlCriteriaStandardSet" ClientIDMode="Static" EmptyMessage="Standard Set" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005"
                                Skin="Vista" Width="180" MaxHeight="280px">
                            </telerik:RadComboBox>
                         </td>
                    </tr>
                    <tr>
                        <td style="width: 80px;">
                            <span style="font-weight: bold;">Grade:</span>
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="ddlCriteriaGrades"  EmptyMessage="Grade"  AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005"
                                Skin="Vista" Width="180" MaxHeight="280px">
                            </telerik:RadComboBox>

                        </td>
                    </tr>

                    <tr>
                        <td style="width: 80px;">
                            <span style="font-weight: bold;">Subject:</span>
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="ddlCriteriaSubjects"  EmptyMessage="Subject" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005"
                                Skin="Vista" Width="180" MaxHeight="280px">
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 80px;">
                            <span style="font-weight: bold;">Course:</span>
                        </td>
                        <td>


                            <telerik:RadComboBox runat="server" ID="ddlCriteriaCourse"  EmptyMessage="Course"  AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005"
                                Skin="Vista" Width="180" MaxHeight="280px">
                            </telerik:RadComboBox>

                        </td>
                    </tr>
                    <tr>
                        <td style="width: 80px;">
                            <span style="font-weight: bold;">Standards:</span>
                        </td>
                        <td>

                            <telerik:RadComboBox runat="server" ID="ddlCriteriaStandards"  DropDownWidth="240"  EmptyMessage="Standards" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005"
                                Skin="Vista" Width="180" CheckBoxes="true" MaxHeight="280px">
                            </telerik:RadComboBox>

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
            var grades = $('#<%= ddlCriteriaGrades.ClientID %>');
            var subjects = $('#<%= ddlCriteriaSubjects.ClientID %>');
            var course = $('#<%= ddlCriteriaCourse.ClientID %>');
            var standards = $('#<%= ddlCriteriaStandards.ClientID %>');
            var valueObject = {};
            valueObject.StandardSet = standardSet.val();
            valueObject.Grades = grades.val();
            valueObject.Subjects = subjects.val();
            valueObject.Courses = course.val();
            var standardsVal = "";

            var item = args.get_item();
            var items = sender.get_items();
            var selectedStandards = [], standardIds = [];
            for (var j = 0; j < items.get_count() ; j++) {
                var itemCheckedCount = 0;
                if (items.getItem(j).get_checked()) {
                    itemCheckedCount++;
                    selectedStandards.push(items.getItem(j).get_text());
                    standardIds.push(items.getItem(j).get_value());
                }
            }

            // CriteriaController.RemoveAll(criteriaName);     // clear whatever was there before
            if (selectedStandards.length > 0) {
                if (selectedStandards.length > 1) {
                    standardsVal = "Multiple";
                } else {
                    if (item) valueObject.StandardName = item.get_text();
                    standardsVal = selectedStandards;
                }


            }

            valueObject.Standard = standardsVal;
            valueObject.StandardId = standardIds.join("_");

           
            if (standardSet.val() == "0" && grades.val() == "0" && subjects.val() == "0" && course.val() == "0" && standards.val() == "0") {
                // we've cleared the text box, clear them
                CriteriaController.RemoveAll(criteriaName);
            }


            else {
                
                switch (sender._element.id) {
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
                //This is used in Special condition in Competency Tracking Report -US 23008
                var pageName = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);
                if (pageName == "CompetencyTrackingReportPage.aspx") {
                    if ((standardSet.val() != "" && standardSet.val() != "Standard Set") && (grades.val() != "" && grades.val() != "Grade") && (subjects.val() != "" && subjects.val() != "Subject") && (course.val() != "") && (course.val() != "Course")) {
                        CriteriaController.Add(criteriaName, valueObject);
                    }
                }
            }
            <%=OnChange%>;

        },

        Clear: function () {
           // debugger;
            var CriteriaStandardSet = $find("<%= ddlCriteriaStandardSet.ClientID %>");
            CriteriaStandardSet.clearSelection();
            CriteriaStandardSet.set_text(CriteriaStandardSet.get_emptyMessage());
          
            var CriteriaGrades = $find("<%= ddlCriteriaGrades.ClientID %>");
            CriteriaGrades.clearItems();
            CriteriaGrades.set_text(CriteriaGrades.get_emptyMessage());
         
            var CriteriaSubjects = $find("<%= ddlCriteriaSubjects.ClientID %>");
            CriteriaSubjects.clearItems();
            CriteriaSubjects.set_text(CriteriaSubjects.get_emptyMessage());
            

            var CriteriaCourse = $find("<%= ddlCriteriaCourse.ClientID %>");
            CriteriaCourse.clearItems();
            CriteriaCourse.set_text(CriteriaCourse.get_emptyMessage());
            
           
            var CriteriaStandards = $find("<%= ddlCriteriaStandards.ClientID %>");
            CriteriaStandards.clearItems();
            CriteriaStandards.set_text(CriteriaStandards.get_emptyMessage());
          
           
        },

        RemoveByKeyHandler: function (criteriaName, value, calledFromAdd) {
            if (calledFromAdd) return;
            this.Clear();
         },



        PopulateGrade: function (standardSetctrl) {

            var standardSet = $(standardSetctrl).val();

            var CriteriaGrades = $find("<%= ddlCriteriaGrades.ClientID %>");
            CriteriaGrades.clearSelection();
            CriteriaGrades.set_text(CriteriaGrades.get_emptyMessage());

            var CriteriaSubjects = $find("<%= ddlCriteriaSubjects.ClientID %>");
            CriteriaSubjects.clearItems();
            CriteriaSubjects.set_text(CriteriaSubjects.get_emptyMessage());

            var CriteriaCourse = $find("<%= ddlCriteriaCourse.ClientID %>");
            CriteriaCourse.clearItems();
            CriteriaCourse.set_text(CriteriaCourse.get_emptyMessage());
          

            var CriteriaStandards = $find("<%= ddlCriteriaStandards.ClientID %>");
            CriteriaStandards.clearItems();
            CriteriaStandards.set_text(CriteriaStandards.get_emptyMessage());
           

            $.ajax({
                type: "POST",
                url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardGradeList',
                data: "{'standardSet':'" + standardSet + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    var data = [];
                    var data1 = [];
                    data = JSON.parse(result.d);

                    for (var i = 0; i < data.length; i++) {
                        data1.push([data[i].Grade, data[i].Grade]);
                    }
                    var ddlControl = $find("<%= ddlCriteriaGrades.ClientID %>");
                    <%=CriteriaName%>Controller.PopulateList(ddlControl, data1, 0, 1);
                }
            });


        },


        PopulateSubject: function (standardGradectrl) {

         

            var CriteriaCourse = $find("<%= ddlCriteriaCourse.ClientID %>");
            CriteriaCourse.clearItems();
            CriteriaCourse.set_text(CriteriaCourse.get_emptyMessage());


            var CriteriaStandards = $find("<%= ddlCriteriaStandards.ClientID %>");
            CriteriaStandards.clearItems();
            CriteriaStandards.set_text(CriteriaStandards.get_emptyMessage());



            var standardSetctrl = $('#<%= ddlCriteriaStandardSet.ClientID %>');
            var standardSet = $(standardSetctrl).val();
            var standardGrade = $(standardGradectrl).val();


            $.ajax({
                type: "POST",
                url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardSubjectList',
                data: "{'standardSet':'" + standardSet + "','grade':'" + standardGrade + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    var data = [];
                    var data1 = [];
                    data = JSON.parse(result.d);

                    for (var i = 0; i < data.length; i++) {
                        data1.push([data[i].Subject, data[i].Subject]);
                    }
                    var ddlControl = $find("<%= ddlCriteriaSubjects.ClientID %>");
                   <%=CriteriaName%>Controller.PopulateList(ddlControl, data1, 0, 1);
               }
            });
        },

        PopulateCourse: function (standardSubjectCtrl) {

            var CriteriaStandards = $find("<%= ddlCriteriaStandards.ClientID %>");
            CriteriaStandards.clearItems();
            CriteriaStandards.set_text(CriteriaStandards.get_emptyMessage());

            var standardSet = $('#<%= ddlCriteriaStandardSet.ClientID %>').val();
            var grade = $('#<%= ddlCriteriaGrades.ClientID %>').val();
            var subject = $(standardSubjectCtrl).val();

            $.ajax({
                type: "POST",
                url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardCourseList',
                data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "','subject':'" + subject + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    var data = [];
                    var data1 = [];
                    data = JSON.parse(result.d);

                    for (var i = 0; i < data.length; i++) {
                        data1.push([data[i].Course, data[i].Course]);
                    }
                    var ddlControl = $find("<%= ddlCriteriaCourse.ClientID %>");
                    <%=CriteriaName%>Controller.PopulateList(ddlControl, data1, 0, 1);
                }
            });
        },



        PopulateList: function (ddl, arry, text_pos, value_pos) {
            var combo = ddl;
            combo.clearItems();
            for (var j = 0; j < arry.length; j++) {
                var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                if (text_pos != null && value_pos != null) {
                    comboItem.set_text(arry[j][text_pos]);
                    comboItem.set_value(arry[j][value_pos]);
                } else {
                    comboItem.set_text(arry[j]);
                    comboItem.set_value(arry[j]);
                }
                combo.get_items().add(comboItem);
            }


        },

       
        PopulateStandards: function (standardCourseCtrl) {

           

            var standardSet = $('#<%= ddlCriteriaStandardSet.ClientID %>').val();
            var grade = $('#<%= ddlCriteriaGrades.ClientID %>').val();
            var subject = $('#<%= ddlCriteriaSubjects.ClientID %>').val();
            var course = $(standardCourseCtrl).val();
            $.ajax({
                type: "POST",
                url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetStandardsByStandardSetGradeSubjectCourseList',
                data: "{'standardSet':'" + standardSet + "','grade':'" + grade + "','subject':'" + subject + "','course':'" + course + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    var data = [];
                    var data1 = [];
                    data = JSON.parse(result.d);
                  
                    for (var i = 0; i < data.length; i++) {
                        data1.push([data[i].StandardName, data[i].ID]);
                    }
                    var ddlControl = $find("<%= ddlCriteriaStandards.ClientID %>");
                     <%=CriteriaName%>Controller.PopulateList(ddlControl, data1, 0, 1);
                 }

            });
           
        }
   

    };

</script>
<script id="<%=CriteriaName%>" type="text/x-jsrender">
   {{for Values}}
     <div class="{{:~getCSS(Applied, CurrentlySelected)}}">
         <div class="imgBeforeCriteria" onclick="CriteriaController.RemoveByKey('<%=CriteriaName%>', {{:Key}})" />
    <%--<div class="criteria_Simple{{if !Applied}}Unapplied{{/if}}">--%>
         <div class="criteriaText" style="font-size: 8pt;margin-left:5px;height:16px">Standard Set: {{:Value.StandardSet}}</div>
         {{if Value.Grades != 0 }}<div class="criteriaText marginLeft criteriaPaddingBottom" style="font-size: 8pt">Grade: {{:Value.Grades}}</div>{{/if}}
         {{if Value.Subjects != 0 }}<div class="criteriaText marginLeft criteriaPaddingBottom" style="font-size: 8pt">Subject: {{:Value.Subjects}}</div>{{/if}}
         {{if Value.Courses != 0 }}<div class="criteriaText marginLeft criteriaPaddingBottom" style="font-size: 8pt">Course: {{:Value.Courses}}</div>{{/if}}
         {{if Value.StandardId != 0 }}<div class="criteriaText marginLeft criteriaPaddingBottom" style="font-size: 8pt">Standards: {{:Value.Standard}}</div>{{/if}}
     </div>
    {{/for}}    
</script>
<style type="text/css">
    .rcbCheckBox {
        margin-right: 2px;
    }
    .criteriaPaddingBottom{
        padding-bottom:5px;
    }

</style>
