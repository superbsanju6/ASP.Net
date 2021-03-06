﻿<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="Schedules_Edit.ascx.cs" Inherits="Thinkgate.Controls.Schedules_Edit" %>
<style type="text/css">
    .outerDiv
    {
    }
</style>

<telerik:RadCodeBlock ID="CodeBlock" runat="server">
    <script type="text/javascript">
        var Schedules_Edit_Control = {
            ControlsArray: [],

            LoadControlsArray: function () {

                this.ControlsArray.length = 0;

                // Since these telerik controls were generated by the codebehind and not within the .ascx, we cannot use the "ClientID" 
                // in order to reference each telerik control.  Therefore, we have to use other means for getting the information out 
                // of the rendered .ascx.

                // First, let's get an array of all our controls because when we generated, we put "Schedules_Edit_Control" in the class attribute of each.
                var jqArrayOfControls = $('.Schedules_Edit_Control');

                // Next, let's get an array of those controls that are labels so we can iterate through each schedule type.
                var jqArrayOfLabels = jqArrayOfControls.filter('.label');

                var jqArrayOfCtrlsByType;

                // Iterate through each schedule type to get beginning, ending date ranges, and locked status.
                for (var i = 0; i < jqArrayOfLabels.length; i++) {

                    // Make an array of telerik controls that belong to this schedule type.
                    jqArrayOfCtrlsByType = jqArrayOfControls.filter('.' + jqArrayOfLabels[i].innerHTML);

                    // Build an object for this schedule type that we can pass to the hosting .aspx.
                    var oSchedCtrl = new Object()

                    // Get numeric representation of assessment schedule type. This will need to be rewritten if we want the Schedules_Edit.ascx control to be independent of what uses it.
                    var label = jqArrayOfLabels[i];
                    oSchedCtrl.ScheduleTypeName = label.innerText;
                    oSchedCtrl.DefaultValuesArray = label.attributes["DefaultValues"].value.split("|")
                    oSchedCtrl.ScheduleTypeID = parseInt(label.attributes["ScheduleTypeID"].value);
                    oSchedCtrl.DocumentTypeID = parseInt(label.attributes["DocTypeID"].value);

                    var ctrlID = jqArrayOfCtrlsByType.filter('.BeginDate')[0].id;
                    oSchedCtrl.BeginDateCtrl = $find(ctrlID.slice(0, ctrlID.length - 8));

                    ctrlID = jqArrayOfCtrlsByType.filter('.EndDate')[0].id;
                    oSchedCtrl.EndDateCtrl = $find(ctrlID.slice(0, ctrlID.length - 8));

                    oSchedCtrl.ToggleCtrl = $find(jqArrayOfCtrlsByType.filter('.Toggle')[0].id)

                    this.ControlsArray.push(oSchedCtrl);
                }

            },

            ControlValuesArray: [],

            LoadControlValuesArray: function () {
                this.ControlValuesArray.length = 0;

                if (this.ControlsArray.length == 0) this.LoadControlsArray();

                for (var i = 0; i < this.ControlsArray.length; i++) {

                    var oSchedValues = new Object()

                    // Get numeric representation of assessment schedule type. This will need to be rewritten if we want the Schedules_Edit.ascx control to be independent of what uses it.
                    oSchedValues.ScheduleTypeName = this.ControlsArray[i].ScheduleTypeName;
                    oSchedValues.ScheduleTypeID = this.ControlsArray[i].ScheduleTypeID;
                    oSchedValues.DocumentTypeID = this.ControlsArray[i].DocumentTypeID;
                    oSchedValues.Locked = this.ControlsArray[i].ToggleCtrl.get_selectedToggleStateIndex();
                    oSchedValues.Lock_Begin = this.ControlsArray[i].BeginDateCtrl.get_selectedDate();
                    if (oSchedValues.Lock_Begin != null) oSchedValues.Lock_Begin = oSchedValues.Lock_Begin.format('yyyy/MM/dd');
                    oSchedValues.Lock_End = this.ControlsArray[i].EndDateCtrl.get_selectedDate();
                    if (oSchedValues.Lock_End != null) oSchedValues.Lock_End = oSchedValues.Lock_End.format('yyyy/MM/dd');
                    this.ControlValuesArray.push(oSchedValues);
                }
            },

            SetDateControlsToDefaults: function () {
                if (this.ControlsArray.length == 0) this.LoadControlsArray();

                for (var i = 0; i < this.ControlsArray.length; i++) {

                    var defaultBeginDate = this.ControlsArray[i].DefaultValuesArray[0];
                    if (defaultBeginDate && !isNaN(Date.parse(defaultBeginDate)))
                        this.ControlsArray[i].BeginDateCtrl.set_selectedDate(new Date(defaultBeginDate));

                    var defaultEndDate = this.ControlsArray[i].DefaultValuesArray[1];
                    if (defaultEndDate && !isNaN(Date.parse(defaultEndDate)))
                        this.ControlsArray[i].EndDateCtrl.set_selectedDate(new Date(defaultEndDate));

                    var defaultLockedValue = this.ControlsArray[i].DefaultValuesArray[2];
                    if (defaultLockedValue)
                        this.ControlsArray[i].ToggleCtrl.set_selectedToggleStateIndex((new Boolean(defaultLockedValue)) ? <%=cbxChecked%> : <%=cbxUnchecked%>);
                }
            },

            ClearDatesByType: function(sType) {
                if (this.ControlsArray.length == 0) this.LoadControlsArray();

                for (var i = 0; i < this.ControlsArray.length; i++) {
                    if (this.ControlsArray[i].ScheduleTypeName == sType) {
                        this.ControlsArray[i].BeginDateCtrl.clear();
                        this.ControlsArray[i].EndDateCtrl.clear();
                        break;
                    }
                }
            },

            ValidateDateRanges: function() {
                if (this.ControlValuesArray.length == 0) this.LoadControlValuesArray();
                var errorMsg = "";
                for (var i = 0; i < this.ControlValuesArray.length; i++) {

                    // Test to see if both dates are legit
                    if (!isNaN(Date.parse(this.ControlValuesArray[i].Lock_Begin)) &&
                        !isNaN(Date.parse(this.ControlValuesArray[i].Lock_End))) {
                        
                        //test to see if begginning date > ending date
                        if (Date.parse(this.ControlValuesArray[i].Lock_Begin) > Date.parse(this.ControlValuesArray[i].Lock_End)) {
                            errorMsg += "<li>   The " + this.ControlValuesArray[i].ScheduleTypeName + " date window\'s begin date must not precede it\'s end date.</li>";
                        }
                    }
                }

                return errorMsg;
            }
        }

        //Called when user clicks "Cancel" RadButton.
        function hideEditControl() {
            <%=CancelHandler + "();"%>
        }

        function sendEditControlDatatoParent() {
            //if (Schedules_Edit_Control.ControlValuesArray.length == 0)
            Schedules_Edit_Control.LoadControlValuesArray();
            var errorMsg = Schedules_Edit_Control.ValidateDateRanges();
            if (errorMsg.length > 0) {
                errorMsg = "<b>Cannot save for the following reasons:</b><br><ul>" + errorMsg + "</ul><br><b>Please correct and try saving again.</b>";
                customDialog({maximize: true, maxwidth: 500, maxheight: 100, animation: "None", dialog_style: "alert", content: errorMsg   }, [{ title: "Ok"}]);
            }
            else {
                <%=SaveHandler + "(Schedules_Edit_Control.ControlValuesArray);" %>
            }
        }

    </script>
</telerik:RadCodeBlock>

<div style="overflow: hidden;">
    <asp:Panel runat="server" ID="Schedules_Edit_EditPanel" ClientIDMode="Static" >
        <table class="fieldValueTable" style="width: 400px; margin-left: auto; margin-right: auto;">
            <tr>
                <td runat="server" id="cellSchedControlsTable" ClientIDMode="Static" class="fieldLabel">
                    <table runat="server" id="SchedControlsTable" ClientIDMode="Static">
                        <tr>
                            <th></th>
                            <th>Begin</th>
                            <th>End</th>
                            <th></th>
                            <th>Disabled</th>
                        </tr>
                    </table>
                    <input runat="server" id="testinput" ClientIDMode="Static" type="hidden" value ="Banana"/>
                </td>
                <td class="fieldLabel" style="text-align: right;">
                    <div style="display:block; height:50px; ">
                        <telerik:RadButton runat="server" ID="rbSave" ClientIDMode="Static" Text="Save" AutoPostBack="false" Width="60px" OnClientClicked="sendEditControlDatatoParent" />
                    </div>
                    <div style="display:block;">
                        <telerik:RadButton runat="server" ID="rbCancel" ClientIDMode="Static" Text="Cancel" AutoPostBack="false" OnClientClicked="hideEditControl" Width="60px" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="Schedules_Edit_ResultPanel" ClientIDMode="Static">
        <asp:Label runat="server" ID="lblResultMessage" ClientIDMode="Static" Text="" />
        <br />
        <telerik:RadButton runat="server" ID="RadButtonOpenSchool" ClientIDMode="Static" Text="Ok" AutoPostBack="False"
            OnClientClicked="hideEditControl" />
    </asp:Panel>
</div>
