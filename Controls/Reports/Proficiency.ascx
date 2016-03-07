<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Proficiency.ascx.cs"
    Inherits="Thinkgate.Controls.Reports.Proficiency" %>
<script type="text/javascript">
    function getTeachers(result) {

        // Teachers
        updateCriteriaControl('Teacher', result.Teachers, 'DropDownList', 'Teacher');
    }

    function getSchoolsAndTeachers(result) {

        // Schools
        updateCriteriaControl('School', result.Schools, 'DropDownList', 'School');

        // Teachers
        updateCriteriaControl('Teacher', result.Teachers, 'DropDownList', 'Teacher');
    }

    function buildDependencyObject(dependencies) {
  
        var data = {
            "container": {}
        };

        var dependencyObject = jQuery.parseJSON(dependencies);

        for (var dependency in dependencyObject) {
            var key = dependencyObject[dependency].key;
            var currentKey = dependencyObject[dependency].value;
            
            if(!key || typeof (key) == 'undefined') continue;

            data.container[currentKey + "Key"] = key;
            data.container[currentKey] = [];

            var genericDiv = $("div[id*='" + getControlType(dependencyObject[dependency].type) + key + "']:not(div[id$='_DropDown'])");

            if (genericDiv == null) {
                alert("Couldn't find: " + dependencyObject[dependency].type);
                continue;
            }

            var genericControl = $find(genericDiv.attr('id'));
            if (genericControl == null) {
                continue;
            }

            var controlItems = genericControl.get_items()._array;
            var jsonItem = {};
            for (var i = 0; i < controlItems.length; ++i) {
                var selected = false;

                switch (dependencyObject[dependency].type) {
                    case "CheckBoxList":
                        selected = controlItems[i].get_checked();
                        jsonItem = {
                            "DisplayText": controlItems[i].get_text(),
                            "Value": controlItems[i].get_value(),
                            "Selected": selected
                        };
                        break;

                    case "DropDownList":
                        selected = controlItems[i].get_selected();
                        jsonItem = {
                            "ID": controlItems[i].get_value().replace(key + "_", ''),
                            "DisplayText": controlItems[i].get_text(),
                            "Value": controlItems[i].get_value(),
                            "Selected": selected
                        };
                        break;
                }

                if (!selected) {
                    continue;
                }

                data.container[currentKey].push(jsonItem);
            }
        }
        return data;
    }

    function getControlType(type) {
        switch (type) {
            case "CheckBoxList":
                return "RadCombobBoxCriteriaCheckBoxList";

            case "DropDownList":
                return "RadCombobBoxCriteriaDropDownList";

            default:
                return "";
        }
    }

    function updateCriteriaControl(key, data, type, headerText) {
        var partialID = getControlType(type) + key;
        var genericDiv = $("div[id$='" + partialID + "']");
        var genericControl = $find(genericDiv.attr('id'));
        if (genericControl == null) {
            alert(genericDiv.attr('id'));
            return;
        }

        var genericControlItems = genericControl.get_items();
        genericControlItems.clear();

        switch (type) {
            case "DropDownList":
                var blankItem = new Telerik.Web.UI.RadComboBoxItem();
                blankItem.set_text("Select a " + headerText);
                blankItem.set_value('0');
                genericControlItems.add(blankItem);
                break;
        }

        for (var contract in data) {
            var item = null;

            switch (type) {
                case "CheckBoxList":
                    item = new Telerik.Web.UI.RadListBoxItem();
                    item.set_checked(data[contract].Selected);
                    break;

                case "DropDownList":
                    item = new Telerik.Web.UI.RadComboBoxItem();
                    item.set_selected(data[contract].Selected);
                    break;
            }

            if (data[contract].Value) {
                item.set_text(data[contract].DisplayText);
                item.set_value(data[contract].Value);

                genericControlItems.add(item);
            }
        }
    }

    var serviceControlsList = {};
    var loadingInterval = "";

    function checkServiceControlsFullyLoaded() {
        for (var control in serviceControlsList) {
            if (!serviceControlsList[control].loaded) {
                return;
            }
        }

        for (var control in serviceControlsList) {
            serviceControlsList[control].callback();
        }

        if (loadingInterval != "") {
            window.clearInterval(loadingInterval);
        }
    }

    function addServiceControl(key) {
        var serviceControl = {
            loaded: false
        };

        serviceControlsList[key] = serviceControl;
    }

    function loadServiceData(key, type) {
        var partialID = getControlType(type) + key;
        var genericDiv = $("div[id$='" + partialID + "']:not(div[id$='_DropDown'])");

        var genericControl = $find(genericDiv.attr('id'));
        if (genericControl == null) {

            //alert("loadServiceData: " + genericDiv.attr('id'));
            return;
        }

        var serviceUrl = genericDiv.attr('serviceurl');
        var serviceSuccessfulCallback = genericDiv.attr('successcallback');
        var dependencies = genericDiv.attr('dependencies');
   
        if (serviceUrl != null) {
            ajaxWCFService({ url: serviceUrl, data: JSON.stringify(buildDependencyObject(dependencies)), success: serviceSuccessfulCallback });
        }
    }
    </script>
<style type="text/css">
    .profLevel
    {
        width: 150px;
        border: 0px;
        margin: 2px;
        padding: 2px;
        float: left;
    }
    
    .reportSection
    {
        text-align: center;
        width: 100%;
    }
</style>
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
            <div style="width: 100%; height: 565px;">
                <div id="divExportOptions" style="text-align: left">
                    <asp:ImageButton runat="server" ID="exportGridImgBtn" OnClick="ExportGridImgBtn_Click"
                        ImageUrl="~/Images/Toolbars/excel_button.png" />
                    <asp:ImageButton runat="server" ID="btnFLDOE" OnClientClick="window.open('http://www.fldoe.org/asp/k12memo/pdf/tngcbtf.pdf'); return false;"
                        ImageUrl="~/Images/Toolbars/info.png" Visible="False"/>
                </div>
                <div id="divProficiencyLevels" class="reportSection">
                    <asp:Panel runat="server" ID="pnlProficiencyLevels" />
                </div>
                <div id="divDropdownMenus" class="reportSection">
                    <telerik:RadComboBox ID="cmbGridDisplay" runat="server" ToolTip="Select a display option"
                        Skin="Web20" OnSelectedIndexChanged="cmbGridDisplay_SelectedIndexChanged" AutoPostBack="true"
                        CausesValidation="False" HighlightTemplatedItems="true" Visible="False">
                        <Items>
                            <telerik:RadComboBoxItem Value="Level" Text="Show Level Distribution" />
                            <telerik:RadComboBoxItem Value="Score" Text="Show Average Scores" />
                            <telerik:RadComboBoxItem Value="All" Text="Show All Statistics" />
                        </Items>
                    </telerik:RadComboBox>
                    <telerik:RadComboBox ID="cmbDomain" runat="server" ToolTip="Select a domain" Skin="Web20" Visible="false"
                        OnSelectedIndexChanged="cmbDomain_SelectedIndexChanged" AutoPostBack="true" CausesValidation="False"
                        HighlightTemplatedItems="true">
                        <ItemTemplate>
                            <span>
                                <%# Eval("Domain") %></span>
                        </ItemTemplate>
                    </telerik:RadComboBox>
                </div>
                <div id="lblInitialText" runat="server" style="font-size: 11pt; text-align:center; margin-top:25%;">Please select criteria for all required fields (Indicated by <span style="color: rgb(255, 0, 0); font-weight: bold;">*</span>)<br/> then Update Results.</div>
                <div id="divResults" class="reportSection" style="background-color: white">
                    <telerik:RadGrid runat="server" ID="gridResults" AutoGenerateColumns="false" OnItemDataBound="gridResults_DataBound" Visible="False">
                    </telerik:RadGrid>
                </div>
            </div>
        </div>
    </div>
</div>
