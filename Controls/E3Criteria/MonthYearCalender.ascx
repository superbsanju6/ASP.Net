<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="MonthYearCalender.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.MonthYearCalender" %>
<%@ Register TagPrefix="e3" TagName="CriteriaHeader" Src="~/Controls/E3Criteria/CriteriaHeader.ascx" %>

<e3:CriteriaHeader ID="CriteriaHeader" runat="server" />

<telerik:RadToolTip ID="RadToolTip1" runat="server" Width="140" Skin="Black" EnableShadow="True" ShowEvent="OnClick" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element">
    <div style="position: relative;">
        <div style="width: 140px; float: left;">
            <telerik:RadMonthYearPicker ID="rdPickerStart" runat="server" DateInput-DateFormat="MMM yyyy" DateInput-DisplayDateFormat="MMM yyyy" AutoPostBack="False" Skin="Vista" Width="140"  MinDate="01/01/1980" MaxDate="01/01/3000" ZIndex="8005">
            </telerik:RadMonthYearPicker>
        </div>
    </div>
</telerik:RadToolTip>

<%-- ReSharper disable once AssignedValueIsNeverUsed --%>
<%-- ReSharper disable once AssignedValueIsNeverUsed --%>
<script type="text/javascript">

    var <%=CriteriaName%>Controller = {
        OnChange: function (sender, args) {
            //if (sender._element) {
            //    if (sender._element.getAttribute("SuppressNextChangeEvent") == "true") {
            //        // we are here just because we've cleared the dropdown and it fired the updated event. Suppress it
            //        sender._element.setAttribute("SuppressNextChangeEvent", "false");
            //        return;
            //    }
            //}
            var _this = <%=CriteriaName%>Controller;
            var criteriaName = "<%=CriteriaName%>";

            var type = sender._element.id.indexOf("Start", sender._element.id.length - 5) > 0 ? "Start" : "End";
           
            var values = CriteriaController.GetValues(criteriaName);
            if (values)
                for (var j = 0; j < values.length; j++) {
                    if (values[j].Value.Type == type)
                        CriteriaController.Remove(criteriaName, values[j].Value);
                }

            var valueObject = _this.ValueObjectForItem(args.get_newValue(), type);
            CriteriaController.Add(criteriaName, valueObject);

            /* Check date validation - we need type, criteriaName and values, also, the only controllers
             * that will have this functionality at the moment are the 'CreatedDate' and 'ExpirationStatus'
             * controllers. */
            ValidationOfDates(type, criteriaName, values);

            <%=OnChange%>;
        },


        ValueObjectForItem: function (date, type) {
            var valueObject = {};
            valueObject.Date = date;
            valueObject.Type = type;
            return valueObject;
        },

        RemoveByKeyHandler: function (criteriaName, value, calledFromAdd) {
            if (calledFromAdd) return;      // if we're calling this from an add, that means essentially we've changed the text or option. If I were to continue, it would clear the text box and dropdown which is not what I want because we obviously have a valid value in addition to the old removed one
            var values = CriteriaController.GetValues(criteriaName);
            if (values)
                for (var j = 0; j < values.length; j++) {
                    if (values[j].Value.Type == value.Type && values[j].CurrentlySelected) return;
                }

            var rdPickerStart = $find("<%= rdPickerStart.ClientID %>");

            if (value.Type == 'Start') {
                rdPickerStart._element.setAttribute("SuppressNextChangeEvent", "true");
                rdPickerStart.clear();
            } else {
                rdPickerEnd._element.setAttribute("SuppressNextChangeEvent", "true");
                rdPickerEnd.clear();
            }
        }
    };


    /* Validation: checks type ('End' or 'Start'), grabs values from
     * RadCalenderDay control. If both have values, grabs a result from 
     * a date check on the two date objects. Displays alert if end date is not valid */
    function ValidationOfDates(type, criteriaName, values) {
        /* Check for CreatedDateController */
        //if (criteriaName.contains("StartMonthYear")) {
        //    if (values) {
        //        if (values[0].Value.Type == "End") {
        //            var rdCalEnd = convertDate(values[0].Value.Date);
        //            var rdCalStart = convertDate(values[1].Value.Date);
        //        } else if (values[1].Value.Type == "End") {
        //            var rdCalEnd = convertDate(values[1].Value.Date);
        //            var rdCalStart = convertDate(values[0].Value.Date);
        //        }
        //        /* make sure we have 2 valid date values to compare. On the first changeEvent, we will only have 1, this makes sure that
        //         * nothing is fired unless we have two, defined, formatted values to convert to date objects */
        //        if (rdCalStart != "" && rdCalEnd != "") {
        //            checkDateAttributes(rdCalStart, rdCalEnd);
        //        }
        //    }
        //}

        /* Check for ExpirationDateRangeController */
        //if (criteriaName.contains("ExpirationDateRange")) {
            /* get the RadCalenderDate picker for start and end dates */
         //   var rdPickerStart = $find("<%= rdPickerStart.ClientID %>")._element.control._element.value; // one of a couple properties 


            /* this doesn't fire on the first change event, make sure user has selected two values */
       //     if (rdPickerStart != "" && rdPickerEnd != "") {
         //       if (values[0].Value.Type == "End") {
            //        var rdCalEnd = convertDate(values[0].Value.Date);
            //        var rdCalStart = convertDate(values[1].Value.Date);
            //    } else if (values[1].Value.Type == "End") {
            //        var rdCalEnd = convertDate(values[1].Value.Date);
            //        var rdCalStart = convertDate(values[0].Value.Date);
            //    }
                /* make sure we have 2 valid date values to compare */
            //    if (rdCalStart != "" && rdCalEnd != "") {
             //       checkDateAttributes(rdCalStart, rdCalEnd);
            //    }
           // }
       // }
    }

    /* Converts the date sent from any constructor type that it is and returns a date-formatted
     * object. Most of the dates sent here are in a string format, due inconsistiencies with 
     * the dynamic controllers, this is a safety net to make sure dates are compared. */
    function convertDate(d) {
        return (
            d.constructor === Date ? d :
                d.constructor === Array ? new Date(d[0], d[1], d[2]) :
                d.constructor === Number ? new Date(d) :
                d.constructor === String ? new Date(d) :
                typeof d === "object" ? new Date(d.year, d.month, d.date) :
                NaN
        );
    }

    /* function assures all values are Dates before calling 'checkEndDate', then alerts user if 
     * the endDate was an invalid date based off the number returned (-1 invalid, 0 valid). */
    function checkDateAttributes(rdDtStart, rdDtEnd) {
        if (rdDtStart.constructor === Date && rdDtEnd.constructor === Date) {
            /* create variable to grab result from date check */
            var dateResult = checkEndDate(rdDtStart, rdDtEnd);
            /* display an alert box to let user know the end date is invalid */
            if (dateResult == -1) {
                alert('Please provide a valid end date. It cannot be less than the start date. Please re-enter...');
                return;
            }
        } else {   // If user gets this message, that would indicate that the RadCalenderDateInputs are not being converted to Date objects
            alert('The start and end date objects are not formatted date objects');
            return;
        }
    }

    /* Accepts 2 date-string objects and compares, checking 
     * that end date is not less than the start date */
    function checkEndDate(startDate, endDate) {
        //alert('inside check date');  debugging purposes
        if (endDate < startDate) {
            return -1;
        } else {
            return 0;
        }
    }


</script>
<script id="<%=CriteriaName%>" type="text/x-jsrender">
    {{for Values}}
        {{if Value.Type == 'Start'}}
        <div class="{{:~getCSS(Applied, CurrentlySelected)}}">
            <div class="imgBeforeCriteria" onclick="CriteriaController.RemoveByKey('<%=CriteriaName%>', {{:Key}})" />
            <div class="criteriaText">{{:Value.Date}}</div>
        </div>
    {{/if}}
    {{/for}}
    {{for Values}}
        {{if Value.Type == 'End'}}
        <div class="{{:~getCSS(Applied, CurrentlySelected)}}">
            <div class="imgBeforeCriteria" onclick="CriteriaController.RemoveByKey('<%=CriteriaName%>', {{:Key}})" />
            <div class="criteriaText">{{:Value.Date}}</div>
        </div>
    {{/if}}
    {{/for}}
</script>
