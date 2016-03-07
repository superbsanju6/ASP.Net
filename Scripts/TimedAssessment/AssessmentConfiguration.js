var AssessmentConfiguration = function () {
    var $timedAssessment,
        $timedAssessmentChk,
        $timeAlloted,
        $timeAllotedChk,
        $allowTimeExtension,
        $allowTimeExtensionChk,
        $timeWarningIntervalImg,
        $timeWarningIntervalChk,
        $autoSubmit,
        $autoSubmitChk,
        $assessmentType,
        $container,
        $msgChkObj,
        $timeIntervalMints,
        $timeIntervals,
        $isAssessmentProofed,
        $cntlId,
        $testId,
        $btnHelp,

        init = function () {
            //controls on page.
            $container = $("#dvTimedControls");
            $assessmentType = $("#hdnAssessmentType");
            $timedAssessment = $find("rcbTimedAssessment");
            $timedAssessmentChk = $("#chkTimedAssessment");
            $timeAlloted = $("#rtbTimeAllotted");
            $timeAllotedChk = $("#chkTimeAllotted");
            $allowTimeExtension = $find("rcbTimeExtension");
            $allowTimeExtensionChk = $("#chkTimeExtension");
            $timeWarningIntervalImg = $(".addIconNew");
            $timeWarningIntervalChk = $("#chkTimeWarningInterval");
            $autoSubmit = $find("rcbAutoSubmit");
            $autoSubmitChk = $("#chkHasAutoSubmit");
            $timeIntervalMints = $("#rdbMinutes");
            $timeIntervals = $("#hdnTimeIntervals");
            $isAssessmentProofed = $("#hdnIsAssessmentProofed");
            $testId = $("#assessmentID").val();
            $btnHelp=$("#btnHelp");
            // disable every thing if assessment is Proofed.


            if ($timeIntervals.val() != "") {
                createRows($timeIntervals.val());
                $timeIntervals.val('');
            }


            // Add java script for numeric only.
            $timeAlloted.numeric({ decimal: false, negative: false }, function () {
                this.value = "";
                this.focus();
            });

            $timeIntervalMints.numeric({ decimal: false, negative: false }, function () {
                this.value = "";
                this.focus();
            });

            $timeAlloted.on("keyup", function () {
                handleTimeAllotedAlert();
            });

            $timeIntervalMints.on("keyup", function () {
                handleTimeIntervalAlert();
            });


            // check uncheck check box on the bases of assessment type.
            if ($assessmentType.val() != "CLASSROOM" && $timedAssessment.get_value() == "0")
                checkedEnforceOnAdministration(true);


            // check value of time assessment
            if ($timedAssessment.get_value() == "0")
                enableDisableControls(true);
            else
                enableDisableControls(false);


            // add event to checkboxes 
            $('#dvTimedControls input[type=checkbox]').each(function () {
                $(this).on('click', function () {
                    if (!$(this).is(':checked'))
                        alertMessage($(this));
                });
            });

            // disable every thing if assessment is Proofed
            if ($isAssessmentProofed.val() == "True") {
                disbaleAll(true);
            }

        },
        handleTimeIntervalAlert = function () {
            var timeInterval = $timeIntervalMints.val();
            var timeAlloted = $timeAlloted.val();
            $cntlId = '';
            $cntlId = $timeIntervalMints.prop("id");
            if (timeAlloted == "") {
                messageAlert("Save cannot be completed without a Time Allotted entry.");
              }
            //else if (timeInterval == "")
            //    messageAlert("Save cannot be completed without a Time Interval entry.");
            else if (parseInt(timeInterval) > parseInt(timeAlloted))
                messageAlert("The Time Warning Interval cannot exceed the Time Allotted.");
            else if (parseInt(timeInterval) == 0)
                messageAlert("Only whole numbers greater than zero can be entered.");
        },
    handleTimeAllotedAlert = function () {
        var timeAlloted = $timeAlloted.val();
        $cntlId = '';
        $cntlId = $timeAlloted.prop("id");
        //if (timeAlloted == "")
        //    messageAlert("An Update cannot be completed without a valid Time Allotted entry.");
    //else if (parseInt(timeAlloted) == 0)
        if (parseInt(timeAlloted) == 0)
        messageAlert("An Update cannot be completed without a valid Time Allotted entry.");
        else if (parseInt(timeAlloted) > 420)
            messageAlert("The Time Allotted cannot exceed 420 minutes.");


    },
    disbaleAll = function (flag) {
        $timedAssessment.disable();
        $timedAssessmentChk.attr('disabled', flag);
        $timeAlloted.attr('disabled', flag);
        $timeAllotedChk.attr('disabled', flag);
        $allowTimeExtension.disable();
        $allowTimeExtensionChk.attr('disabled', flag);
        $timeWarningIntervalImg.attr('disabled', flag);
        $timeWarningIntervalImg.css({ 'cursor': '' });
        $allowTimeExtensionChk.attr('disabled', flag);
        $timeWarningIntervalChk.attr('disabled', flag);
        $autoSubmit.disable();
        $autoSubmitChk.attr('disabled', flag);
        removeEvent();
       
    },
        removeEvent = function () {
            $("#tblTimeWarningInterval tr").each(function () {
                $(".addIconNewDiv").off("click");
                $(this).find('.deleteImg').css({ 'cursor': '' });
            });
        },
        addEvent = function () {
            $("#tblTimeWarningInterval tr").each(function () {
                $(".addIconNewDiv").on("click", deleteRow);
                $(this).find('.deleteImg').css({ 'cursor': 'pointer' });
            });
        },
        createRows = function (val) {
            var timeIntervalArray = val.split(',');
            for (var i = 0; i < timeIntervalArray.length; i++) {
                addNewRowInTimeWarIntv(parseInt(timeIntervalArray[i]), "load");
            }
        },


        changeSelectedIndexOfAutoSubmit = function (value) {
            if (value == "0") {
                $autoSubmit.set_text('Yes');
            } else {
                $autoSubmit.set_text('No');
            }
        },
        changeSelectedIndexOfTimeExtension = function (value) {
            if (value == "0") {
                $allowTimeExtension.set_text('Yes');

            } else {
                $allowTimeExtension.set_text('No');
            }
        },
        alertMessage = function (obj) {
            $msgChkObj = obj;
            customDialog({
                title: "Timed Assessment",
                maxheight: 400,
                maxwidth: 110,
                minimize: true,
                autoSize: false,
                onClosing: function (content) {
                    contentTypeCallbackFunction(true);
                },
                content: "Removing this selection will allow this option to be changed on the Assessment Administration screen.  Select Continue to proceed with the action or Cancel to undo the action.",
                dialog_style: 'confirm'
            }, [{ title: "Cancel", callback: contentTypeCallbackFunction, argArray: [true] }, { title: "Continue", callback: contentTypeCallbackFunction, argArray: [false] }]);
        },
        contentTypeCallbackFunction = function (arg) {
            $msgChkObj.attr('checked', arg);
        },
        checkedEnforceOnAdministration = function (flag) {
            $timedAssessmentChk.attr('checked', flag);
            $timeAllotedChk.attr('checked', flag);
            $allowTimeExtensionChk.attr('checked', flag);
            $timeWarningIntervalChk.attr('checked', flag);
            $autoSubmitChk.attr('checked', flag);
        },
        openModalForTimeInnerval = function () {
            $timeIntervalMints.val('');
            showDialog();
        },
        showDialog = function () {
            $("#dvTimeIntervalModal").dialog({
                height: 30,
                width: 50,
                modal: true,
                position: [300, 140],
                resizable: false,
                buttons: {
                    "Save": saveTime,
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                },
            });
        },
        saveTime = function () {
            valdateNumberOfMinutes();
        },
        valdateNumberOfMinutes = function () {
            $cntlId = '';
            $cntlId = $timeIntervalMints.prop("id");
            var noOfMin = $.trim($timeIntervalMints.val());
            if (noOfMin != "") {
                var iNoOfMin = parseInt(noOfMin);
                var flag = checkValExitInTimeWarring(iNoOfMin);
                if (flag) {
                    addNewRowInTimeWarIntv(iNoOfMin, "Add");
                    addTimeWarningInterval(parseInt($testId), iNoOfMin);
                }
            } else
                messageAlert("Save cannot be completed without a Time Interval entry.");
        },

        addNewRowInTimeWarIntv = function (min, from) {
            $("#tblTimeWarningInterval").append("<tr><td class='minIdCell'>" + min + "</td> <td class='addIconNewDiv'><img src='../../Images/cross.png' class='deleteImg' style='cursor:pointer'/></td></tr>");
            $(".addIconNewDiv").on("click", deleteRow);
            if (from == "Add")
                $("#dvTimeIntervalModal").dialog('close');
        },

        deleteRow = function () {
            $(this).parent().remove();
            var deletedItem = $(this).parent().find('.minIdCell').html();
            deleteTimeWarningInterval(parseInt($testId), parseInt(deletedItem));
        },

        checkValExitInTimeWarring = function (minutes) {
            var flag = true;
            $("#tblTimeWarningInterval tr").each(function () {
                if (parseInt($(this).find(".minIdCell").html()) == minutes) {
                    $cntlId = '';
                    $cntlId = $timeIntervalMints.prop("id");
                    flag = false;
                    messageAlert("This interval has already been defined, please enter a different value.");
                    return flag;
                }
            });
            return flag;
        },
        resetTimedControls = function () {
            $timeAlloted.val('');
            $allowTimeExtension.set_text('No');
            //$("#tblTimeWarningInterval").html('');
            $autoSubmit.set_text('Yes');
            if ($assessmentType.val() != "CLASSROOM") {
                checkedEnforceOnAdministration(true);
            }
        },
        enableDisableControls = function (flag) {
            if (!flag) {
                $timeWarningIntervalImg.css({ 'cursor': 'pointer' });
                $timeWarningIntervalImg.off('click');
                $timeWarningIntervalImg.on('click', function () {
                    openModalForTimeInnerval();
                });
                addEvent();
                $allowTimeExtension.enable();
                $autoSubmit.enable();
                if ($isAssessmentProofed.val() == "True") {
                    $(".addIconNew").unbind();
                    $timeWarningIntervalImg.css({ 'cursor': '' });
                    removeEvent();
                }
            } else {
                $timeWarningIntervalImg.css({ 'cursor': '' });
                removeEvent();
                resetTimedControls();
                $allowTimeExtension.disable();
                $autoSubmit.disable();
                $(".addIconNew").unbind();
            }
            $allowTimeExtensionChk.attr('disabled', flag);
            $timedAssessmentChk.attr('disabled', flag);
            $timeAlloted.attr('disabled', flag);
            $timeAllotedChk.attr('disabled', flag);
            $allowTimeExtensionChk.attr('disabled', flag);
            $timeWarningIntervalImg.attr('disabled', flag);
            $timeWarningIntervalChk.attr('disabled', flag);
            $autoSubmitChk.attr('disabled', flag);
        },
        getMaxTimeInterval = function () {
            var timeIntervalArray = [];
            $("#tblTimeWarningInterval tr").each(function () {
                var timeInterval = parseInt($(this).find(".minIdCell").html());
                timeIntervalArray.push(timeInterval);
            });
            var abc = timeIntervalArray.join(',');
            $timeIntervals.val(timeIntervalArray.join(','));
            return Math.max.apply(Math, timeIntervalArray);
        },
        validate = function () {
            var flag = true;
            if ($timedAssessment.get_value() == "1") {
                var timeAllotedVal = $.trim($timeAlloted.val());
                if (timeAllotedVal != "") {
                    var maxTimeInterVal = getMaxTimeInterval();
                    if (timeAllotedVal < maxTimeInterVal) {
                        $cntlId = '';
                        messageAlert("The Time Warning Interval cannot exceed the Time Allotted.");
                        flag = false;
                    }
                }
                else {
                    flag = false;
                    $cntlId = '';
                    messageAlert("An Update cannot be completed without a Time Allotted entry.");
                }
            } else
                flag = true;
            return flag;
        },
        messageAlert = function (message) {
            //var wnd = window.radalert(message, 400, 100, 'Alert');
            //wnd.add_close(clearValuesCallback);
            //wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            customDialog({ title: 'Alert', height: 100, width: 400, animation: 'None', dialog_style: 'alert', content: message }, [{ title: 'OK', callback: clearValuesCallback }]);
        },
    clearValuesCallback = function () {
        switch ($cntlId) {
            case "rtbTimeAllotted":
                $timeAlloted.val('');
                break;
            case "rdbMinutes":
                $timeIntervalMints.val('');
                break;
        }
    },
      addTimeWarningInterval = function (testId, timeInterval) {
          $.ajax({
              type: "POST",
              url: "AssessmentConfiguration.aspx/AddTimeWarningInterval",
              data: " {'testId':" + testId + ",'timeIntervalVal':'" + timeInterval + "'}",
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              error: function (XMLHttpRequest, textStatus, errorThrown) {

              },
              success: function (result) {
                  var data = [];

              }
          });
      },
      deleteTimeWarningInterval = function (testId, timeInterval) {
          $.ajax({
              type: "POST",
              url: "AssessmentConfiguration.aspx/DeleteTimeWarningInterval",
              data: " {'testId':" + testId + ",'timeIntervalVal':'" + timeInterval + "'}",
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              error: function (XMLHttpRequest, textStatus, errorThrown) {

              },
              success: function (result) {
                  var data = [];

              }
          });
      };
    return {
        "Init": init,
        "EnableDisableControls": enableDisableControls,
        "ChangeSelectedIndexOfAutoSubmit": changeSelectedIndexOfAutoSubmit,
        "ChangeSelectedIndexOfTimeExtension": changeSelectedIndexOfTimeExtension,
        "Validate": validate,
        "MessageAlert": messageAlert
    };
}();