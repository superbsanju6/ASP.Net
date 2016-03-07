<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ACSchoolTeacher.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.AutoCompleteCriteriaControls.ACSchoolTeacher" %>
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
</style>
<script type="text/javascript">

    function getteacherDisabled()
    {
        $find("<%=ddlSchool.ClientID%>").disable();
        $find("<%=ddlTeacher.ClientID%>").disable();
    }
    function getteacherEnabled()
    {
        $find("<%=ddlSchool.ClientID%>").enable();
        selectSingleSchool();
        $find("<%=ddlTeacher.ClientID%>").enable();
    }


    function selectSingleSchool() {

        var finalcontrolId = $find("<%= ddlSchool.ClientID %>");

        if (finalcontrolId.get_items().get_count() == 1) {
            var schoolCmbItemText = finalcontrolId.get_items(0)._array[0]._text;

            var schoolcomboitem = finalcontrolId.findItemByText(schoolCmbItemText);
            if (schoolcomboitem) {
                schoolcomboitem.select();

            }
        }

    }
</script>
<telerik:RadToolTip ID="RadToolTip1" runat="server" Width="260" Height="60px" Skin="Black" EnableShadow="True" ShowEvent="OnClick" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <div style="position: relative">
        <div style="width: 240px;">
            <div style="width: 220px; float: left;">
                <table style="width: 98%;">
                    <tr>
                        <td style="width: 80px;">
                            <span style="font-weight: bold;">School:</span>
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="ddlSchool" EmptyMessage="Select School" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" 
                                Skin="Vista" Width="180" MaxHeight="250px">
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="height: 10px"></td>
                    </tr>
                    <tr>
                        <td style="width: 80px;">
                            <span style="font-weight: bold;">Teacher:</span>
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="ddlTeacher" EmptyMessage="Select Teacher" AutoPostBack="False" MarkFirstMatch="True" AllowCustomText="False" ZIndex="8005" 
                                Skin="Vista" Width="180" MaxHeight="250px">
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
        DefaultTexts: <%= DefaultTextAsJs() %>,
        CheckDefaultTexts: function () {
            var _this = <%=CriteriaName%>Controller;
            if (_this.DefaultTexts == null) return;
            var criteriaName = "<%=CriteriaName%>";
            var listBox = $find("<%= ddlSchool.ClientID %>");
            for (var j=0; j < _this.DefaultTexts.length; j++) {
                var listItem = listBox.findItemByText(_this.DefaultTexts[j]);
                if (listItem) {
                    listItem.select();
                    CriteriaController.Add(criteriaName, _this.ValueObjectForItem(listItem));
                }
            }
        },
        
        OnChange: function (sender, args) {         
          
            var schoolID=sender._value;         
            <%=CriteriaName%>Controller.PopulateGrade(schoolID);
            var comboBox = $find("<%= ddlTeacher.ClientID %>");            
            comboBox.set_text(comboBox.get_emptyMessage());    
        },

        OnChangeTeacher: function (sender, args) {          
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";           
            var item = args.get_item();
            var valueObject = _this.ValueObjectForItem(item);
            CriteriaController.Add(criteriaName, valueObject);
            var comboBox = sender;
            <%=OnChange%>;
        },
        
        ValueObjectForItem: function(item, alternateText, alternateValue) {
            var valueObject = { };
            valueObject.Text = alternateText ? alternateText : item.get_text();
            valueObject.Value = alternateValue ? alternateValue : item.get_value();
            return valueObject;
        },
        
        RemoveByKeyHandler: function(criteriaName, value, calledFromAdd) {
            if (calledFromAdd) return;
            var comboBox = $find("<%= ddlSchool.ClientID %>");
            if (comboBox.get_text() == value.Text)
            {
                comboBox.clearSelection();
                comboBox.set_text(comboBox.get_emptyMessage() || "");
            }
            <%=OnChange%>;
        },

        PopulateGrade: function (standardElement) {

          

            var schoolId = standardElement;
            $.ajax({
                type: "POST",
                url: "./CompetencyTrackingReportPage.aspx/GetTeacherListForSchool",
                data: "{'schoolID':" + schoolId + "}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    var data = [];
                    var data1 = [];
                    data = JSON.parse(result.d);

                    for (var i = 0; i < data.length; i++) {
                        data1.push([data[i].TeacherName, data[i].TeacherPage]);
                    }
                    <%=CriteriaName%>Controller.PopulateListForTeacher(data1, 0, 1);
                }
            });
        },
        
        PopulateList: function (arry, text_pos, value_pos) {
            //alert('populate <%=CriteriaName%>');
            // text_pos and value_pos are optional, and is specific to situations where arry is an array of arrays
            var combo = $find("<%= ddlSchool.ClientID %>");
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
            if (!combo.findItemByText(combo.get_text()) && combo.get_text() != combo.get_emptyMessage()) {
                CriteriaController.RemoveAll('<%=CriteriaName%>');
                combo.set_text(combo.get_emptyMessage() || "");
            }
        },

        PopulateListForTeacher: function (arry, text_pos, value_pos) {
            //alert('populate <%=CriteriaName%>');
            // text_pos and value_pos are optional, and is specific to situations where arry is an array of arrays
            var combo = $find("<%= ddlTeacher.ClientID %>");
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
        
        CheckComboForSelectedValueAfterDependencyChange: function () {
            var combo = $find("<%= ddlSchool.ClientID %>");
            if (!combo.findItemByText(combo.get_text()) && combo.get_text() != combo.get_emptyMessage()) {
                CriteriaController.RemoveAll('<%=CriteriaName%>');
                combo.set_text(combo.get_emptyMessage() || "");
            }
        },
        
        InitialLoad: function() {
            var dataPos = 0;
            var _this = <%=CriteriaName%>Controller;
            var data = <%=CriteriaName%>DependencyData;
            var targetData = CriteriaDataHelpers.GetFieldDistinct(data, dataPos);
            _this.PopulateList(targetData);          
        },
        
        requestItems: function(text, append) {
            var comboBox = $find("<%= ddlSchool.ClientID %>");
            comboBox.requestItems(text, append);
        },
        
        CloseTooltip: function() {
            var tooltip = $find("<%= RadToolTip1.ClientID %>");
            tooltip.hide();
        },
        
        Clear: function() {
            var comboBox1 = $find("<%= ddlSchool.ClientID %>");            
            comboBox1.set_text(comboBox1.get_emptyMessage());            
            var comboBox2 = $find("<%= ddlTeacher.ClientID %>");            
            comboBox2.set_text(comboBox2.get_emptyMessage()); 
        }
    }
   
</script>
