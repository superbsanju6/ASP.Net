var AssessmentAdminstration = function () {
    var $timeWarningIntervalImg, $timeIntervalMints, $timeAlloted, $timeIntervalMints, $timeExtn, $timedAssessment, $enForceChekTimeAssessment,
        $enForceChekTimeAllotted, $enForceChekTimeAllotted, $enForceChekTimeExtension, $enForceChekTimeWarning, $enForceChekAutoSubmit, $tblTimedAssessment,
        $chkboxTimeAssessment, $chkboxAutoSubmit, $rtbTimeAllotted, $rtbTimeExtn, $addIconNew, $dvTimeIntervalModal, $btnSave, $txtchange, $assessmentId,
        $classId, $TestId, $timeWarningInterval, $timeAllottedVal, $totalInterval, $studentId, $valResult, $hiddenIsTimeExtension, $objChkBox, $isChecked, 
        $timedOnCheckedStudentId, $hdnIsAdministrationTimeAssessment, $hdnIsAdministrationAutoSuubmit, $isTimeAllottedChnaged, $isTimeExtnChanged,
        $isTimeAllottedNTimeExtnChanged, $isTimeAllottedContinue, $isClickedTimeAssessment, $isSucessfull, $allStudentIdsList, $hiddenAccessSecurePermission, $hiddenisSecuredFlag, $hiddenisSecureAssessment, $tdRightButtons, $btnContainer, $hiddenShowSecureOTCRefreshButton,
    init = function () {
        $timeWarningIntervalImg = $(".addIconNew");
        $timeIntervalMints = $("#rdbMinutes");
        $timeAlloted = $("#rtbTimeAllotted");
        $timeIntervalMints = $("#rdbMinutes");
        $timeExtn = $("#rtbTimeExtn");
        $timedAssessment = $("#hiddenTimeAssessment").val().toLowerCase() == "true" ? true : false;
        $enForceChekTimeAssessment = $("#hiddenEnForceChkTimeAssessment").val().toLowerCase() == "true" ? true : false;
        $enForceChekTimeAllotted = $("#hiddenEnForceChkTimeAllotted").val().toLowerCase() == "true" ? true : false;
        $enForceChekTimeExtension = $("#hiddenEnForceChkTimeExtension").val().toLowerCase() == "true" ? true : false;
        $enForceChekTimeWarning = $("#hiddenEnForceChkTimeWarning").val().toLowerCase() == "true" ? true : false;
        $enForceChekAutoSubmit = $("#hiddenEnForceChkAutoSubmit").val().toLowerCase() == "true" ? true : false;
        $hiddenIsTimeExtension = $("#hiddenIsTimeExtension").val().toLowerCase() == "true" ? true : false;
        $tblTimedAssessment = $("#tblTimeAssessment");
        $chkboxTimeAssessment = $("#ChkboxTimeAssessment");
        $chkboxAutoSubmit = $("#ChkboxAutoSubmit");
        $rtbTimeAllotted = $("#rtbTimeAllotted");
        $rtbTimeExtn = $("#rtbTimeExtn");
        $addIconNew = $(".addIconNew");
        $dvTimeIntervalModal = $("#dvTimeIntervalModal");
        $btnSave = $("#btnSave");
        $assessmentId = $("#hdnAssessmentId").val();
        $classId = $("#hdnClassId").val();
        $testId = $("#hdnTestId").val();
        $timeWarningInterval = $("#hiddenTimeWarningInterval").val();
        $timeAllottedVal = $("#hiddenTimeAllottedVal").val();
        $totalInterval = $("#hiddenTimeWarningInterval").val();
        keyuptxtchange();
        $rtbTimeExtn.val('');
        $hdnIsAdministrationTimeAssessment = $("#hdnIsAdministrationTimeAssessment").val().toLowerCase() == "true" ? true : false;
        $hdnIsAdministrationAutoSuubmit = $("#hdnIsAdministrationAutoSuubmit").val().toLowerCase() == "true" ? true : false;
        $chkboxAutoSubmit.prop('checked', $hdnIsAdministrationAutoSuubmit);
        $chkboxTimeAssessment.prop('checked', $hdnIsAdministrationTimeAssessment);
        $valResult = false;
        $isTimeAllottedChnaged = false;
        $isTimeExtnChanged = false;
        $isTimeAllottedNTimeExtnChanged = false;
        $isTimeAllottedContinue = false;
        $isClickedTimeAssessment = false;
       // $timeAlloted.attr('disabled', 'disabled');
        $isSucessfull = false;
        $allStudentIdsList = $("#hdnAllStudentIdsList").val();
        $hiddenAccessSecurePermission = $("#hiddenAccessSecurePermission").val().toLowerCase() == "true" ? true : false;
        $hiddenisSecuredFlag = $("#hiddenisSecuredFlag").val().toLowerCase() == "true" ? true : false;
        $hiddenisSecureAssessment = $("#hiddenisSecureAssessment").val().toLowerCase() == "true" ? true : false;
        $tdRightButtons = $("#tdRightButtons");
        $btnContainer = $("#btnContainer");
        $hiddenShowSecureOTCRefreshButton = $("#hiddenShowSecureOTCRefreshButton").val().toLowerCase() == "true" ? true : false;
        
        // Add java script for numeric only.
        $timeAlloted.numeric({ decimal: false, negative: false }, function () {
            this.value = "";
            this.focus();
        });
        $timeExtn.numeric({ decimal: false, negative: false }, function () {
            this.value = "";
            this.focus();
        });
        $timeIntervalMints.numeric({ decimal: false, negative: false }, function () {
            this.value = "";
            this.focus();
        });

        $timeAlloted.on("keydown", function () {
            if ($studentId == undefined || $studentId == null || $studentId.length == 0) {
                customDialog({ title: 'Warning', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: 'A student selection is required before any changes can be attempted.' }, [{ title: 'OK' }]);
            }
        });

        $timeExtn.on("keydown", function () {
            if ($studentId == undefined || $studentId == null || $studentId.length == 0) {
                customDialog({ title: 'Warning', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: 'A student selection is required before any changes can be attempted.' }, [{ title: 'OK' }]);
            }
        });

        showHide();
    },
   

  
    showHide = function () {
        if ($hiddenAccessSecurePermission == true) {
            if ($hiddenisSecuredFlag == true && $hiddenisSecureAssessment == true) {
                $tblTimedAssessment.css("visibility", "hidden");
                $tdRightButtons.css("visibility", "hidden");
                $btnContainer.css("visibility", "hidden");
                if ($hiddenShowSecureOTCRefreshButton == true) {
                    $tblTimedAssessment.css("visibility", "visible");
                    $tdRightButtons.css("visibility", "visible");
                    $btnContainer.css("visibility", "visible");                    
                }
            }
        }
       if ($timedAssessment == false)
           $tblTimedAssessment.css("visibility", "hidden");
       else if ($timedAssessment == true) {
           $tblTimedAssessment.show();
           enableDisableControls();
           if ($enForceChekTimeAssessment == false)
               $chkboxTimeAssessment.removeAttr('disabled');
           else
               $chkboxTimeAssessment.attr('disabled', 'disabled');

           if ($enForceChekAutoSubmit == false)
               $chkboxAutoSubmit.removeAttr('disabled');
           else
               $chkboxAutoSubmit.attr('disabled', 'disabled');

           if ($enForceChekTimeAllotted == false)
               $rtbTimeAllotted.removeAttr('disabled');
           else
               $rtbTimeAllotted.attr('disabled', 'disabled');

           if ($enForceChekTimeExtension == false)
               $rtbTimeExtn.removeAttr('disabled');
           else {
               $rtbTimeExtn.attr('disabled', 'disabled');
           }

           if ($hiddenIsTimeExtension == false) {
               $rtbTimeExtn.attr('disabled', 'disabled');
               $chkboxAutoSubmit.attr('disabled', 'disabled');
           }
           if ($enForceChekTimeWarning == false)
           {
               showTimeInterval($timeWarningInterval);
               $addIconNew.bind();
               $timeWarningIntervalImg.css({ 'cursor': 'pointer' });
               addEvent();
           }
       else
           {
               showTimeInterval($timeWarningInterval);
               $addIconNew.unbind();
               $timeWarningIntervalImg.css({ 'cursor': '' });
               removeEvent();
           }

           if ($hdnIsAdministrationTimeAssessment == false)
           {
               $chkboxAutoSubmit.attr('disabled', 'disabled');
               $rtbTimeAllotted.attr('disabled', 'disabled');
               $rtbTimeExtn.attr('disabled', 'disabled');
               $addIconNew.unbind();
               $timeWarningIntervalImg.css({ 'cursor': '' });
               removeEvent();

           }
           if ($hdnIsAdministrationTimeAssessment == true)
           {
              
               if ($enForceChekAutoSubmit == false)
                   $chkboxAutoSubmit.removeAttr('disabled');
               else
                   $chkboxAutoSubmit.attr('disabled', 'disabled');

               if ($enForceChekTimeAllotted == false)
                   $rtbTimeAllotted.removeAttr('disabled');
               else
                   $rtbTimeAllotted.attr('disabled', 'disabled');

               if ($enForceChekTimeExtension == false)
                   $rtbTimeExtn.removeAttr('disabled');
               else {
                   $rtbTimeExtn.attr('disabled', 'disabled');
               }

               if ($hiddenIsTimeExtension == false) {
                   $rtbTimeExtn.attr('disabled', 'disabled');
                   $chkboxAutoSubmit.attr('disabled', 'disabled');
               }
               if ($enForceChekTimeWarning == false) {
                   $addIconNew.bind();
                   $timeWarningIntervalImg.css({ 'cursor': 'pointer' });
                   addEvent();
               }
               else {
                   $addIconNew.unbind();
                   $timeWarningIntervalImg.css({ 'cursor': '' });
                   removeEvent();
               }

           }
       }       
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
    openModalForTimeInnerval = function () {
        $timeIntervalMints.val('');
        showDialog();
    },
       showDialog = function () {
           $dvTimeIntervalModal.dialog({
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
             var noOfMin = $.trim($timeIntervalMints.val());
             if (noOfMin == "") {
                 $timeIntervalMints.focus();
             }
             else {
                 noOfMin = parseInt(noOfMin);
                 var flag = checkValExitInTimeWarring(noOfMin)
                 if (flag) {
                     addNewRowInTimeWarIntv(noOfMin);
                     addTimeWarningInterval(parseInt($testId), noOfMin);
                 }
             }
         },

  addTimeWarningInterval = function (testEventId, timeInterval) {
      $.ajax({
          type: "POST",
          url: "AssessmentAdministration.aspx/AddTimeWarningInterval",
          data: " {'testEventId':" + testEventId + ",'timeIntervalVal':'" + timeInterval + "'}",
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          error: function (XMLHttpRequest, textStatus, errorThrown) {

          },
          success: function (result) {
              var data = [];

          }
      });
  },

  deleteTimeWarningInterval = function (testEventId, timeInterval) {
      $.ajax({
          type: "POST",
          url: "AssessmentAdministration.aspx/DeleteTimeWarningInterval",
          data: " {'testEventId':" + testEventId + ",'timeIntervalVal':'" + timeInterval + "'}",
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          error: function (XMLHttpRequest, textStatus, errorThrown) {

          },
          success: function (result) {
              var data = [];

          }
      });
  },

           validateNumberOfMinutes = function () {
             var noOfMin = $.trim($timeIntervalMints.val());
             var timeAlloted = $timeAlloted.val();
             var flag = false;
             if (timeAlloted != "") {
                 var parsedTimeAlloted = parseInt(timeAlloted);
                 if (parsedTimeAlloted >= 1 && parsedTimeAlloted <= 420) {
                     var iNoOfMin = parseInt(noOfMin);
                     if (iNoOfMin >= 1 && iNoOfMin <= parseInt(timeAlloted)) {
                         flag = checkValExitInTimeWarring(iNoOfMin)
                         if (flag)
                             addNewRowInTimeWarIntv(iNoOfMin);

                     }

                     else

                         customDialog({ title: 'Warning', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: 'Time Warning Interval Minutes should be less then or equal to Time Alloted.' }, [{ title: 'OK' }]);


                 }
                 else

                     customDialog({ title: 'Warning', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: 'Alloted Time value should greater then 1 and less then or equal to 420.' }, [{ title: 'OK' }]);

             }
             else

                 customDialog({ title: 'Warning', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: 'Time alloted required.' }, [{ title: 'OK' }]);
             return flag;
         },
         checkValExitInTimeWarring = function (minutes) {
             var flag = true;
             $("#tblTimeWarningInterval tr").each(function () {
                 if (parseInt($(this).find(".minIdCell").html()) == minutes) {
                     flag = false;
                     customDialog({ title: 'Warning', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: 'This interval has already been defined, please enter a different value.' }, [{ title: 'OK',callback:emptyValue }]);
                     return flag;
                 }
                 if (parseInt(minutes) == 0) {
                     flag = false;
                     customDialog({ title: 'Warning', height: 100, width: 400, animation: 'None', dialog_style: 'alert', content: 'Only whole numbers greater than zero can be entered.' }, [{ title: 'OK', callback: emptyValue }]);
                     return flag;
                 }
             });
             return flag;
         },

         emptyValue = function ()
         {
             $timeIntervalMints.val('');
         },
       enableDisableControls = function () {
           $timeWarningIntervalImg.css({ 'cursor': 'pointer' });
           $timeWarningIntervalImg.off('click');
           $timeWarningIntervalImg.on('click', function () {
               openModalForTimeInnerval();
           });

       },

         showTimeInterval = function (val) {
             if (val !== "") {
                 var values = $timeWarningInterval.split(",");
                 $.each(values, function (index, value) {
                     $("#tblTimeWarningInterval").append("<tr><td class='minIdCell'>" + value + "</td> <td class='addIconNewDiv'><img src='../../Images/cross.png'  class='deleteImg' style='cursor:pointer'/></td></tr>");
                 });
                 $(".addIconNewDiv").unbind('click').on("click", deleteRow);
             }
         },

   addNewRowInTimeWarIntv = function (value) {
           $("#tblTimeWarningInterval").append("<tr><td class='minIdCell'>" + value + "</td> <td class='addIconNewDiv'><img src='../../Images/cross.png' style='cursor:pointer'/></td></tr>");
           $(".addIconNewDiv").unbind('click').on("click", deleteRow);
           $("#dvTimeIntervalModal").dialog('close');
      
   },
   deleteRow = function () {
       $(this).parent().remove();
       var deletedItem = $(this).parent().find('.minIdCell').html();
       deleteTimeWarningInterval(parseInt($testId), parseInt(deletedItem));
   },
    getMaxTimeInterval = function () {
        var timeInterValArray = [];
        $("#tblTimeWarningInterval tr").each(function () {
            var timeInterval = parseInt($(this).find(".minIdCell").html());
            timeInterValArray.push(timeInterval);
        });
        var abc = timeInterValArray.join(',');
        $("#hiddenTimeWarning").val(timeInterValArray.join(','));
        return Math.max.apply(Math, timeInterValArray);
    },
     hideControls = function () {

         $chkboxAutoSubmit.attr('disabled', 'disabled');
         $rtbTimeAllotted.attr('disabled', 'disabled');
         $rtbTimeExtn.attr('disabled', 'disabled');
         $addIconNew.unbind()


     },
       unhideControls = function () {

           $chkboxAutoSubmit.removeAttr('disabled');
           $rtbTimeAllotted.removeAttr('disabled');
           $rtbTimeExtn.removeAttr('disabled');
           $addIconNew.bind();

       },

       cancelContinueDetails = function () {
           unhideControls();
           $chkboxTimeAssessment.prop('checked', true);
       },

         continueDetails = function () {
             hideControls();
             $('#dvTimeAssessmentModal').dialog("close");
             SaveData();
             var flag = false;
             return flag;
         },

         keyuptxtchange = function () {
             $rtbTimeAllotted.keyup(function () {
                 $txtchange = $.trim($rtbTimeAllotted.val());
                 $txtchange = parseInt($txtchange);

             });

         },
          textchange = function (val) {
              customDialog({ title: "Confirm?", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "Are you certain you would like to change the existing Time Allotted on this assessment.  Select Continue to proceed or Cancel to decline the selection.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel", callback: cancelAlertMsgForAllotted }, { title: "Continue", callback: alertmsg }]);
          },

    cancelAlertMsgForAllotted = function () {
        $timeAlloted.val($timeAllottedVal);
        $txtchange = $timeAllottedVal;
        $isTimeAllottedChnaged = false;
      
    },
          textChangeForTimeExtension = function (val) {
              customDialog({ title: "Confirm?", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "Are you certain you would like to change the existing Time Extension for the selected students on this assessment.  Select Continue to proceed or Cancel to decline the selection.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel", callback: cancelArtmagForTimeExtension }, { title: "Continue", callback: artmagForTimeExtension }]);
              
          },
          artmagForTimeExtension = function () {
              $isTimeAllottedNTimeExtnChanged = true;
              if ($isTimeAllottedChnaged == true && $isTimeExtnChanged == true && $isTimeAllottedNTimeExtnChanged==true)
                  validate();
              if ($isTimeAllottedChnaged == false && $isTimeExtnChanged == true)
                  validate();
          },
          cancelArtmagForTimeExtension = function () {
              if ($('#ChkboxTimeAssessment').is(':checked') == false && $hdnIsAdministrationTimeAssessment == true) {
              }
              else
              $rtbTimeExtn.val('');
              $isTimeExtnChanged = false;
              if ($isTimeAllottedContinue == true)
                  SaveData();
              return false;
          },
            studentIdOnRowSelect = function (val) {
                $studentId = val;
                if ($studentId == undefined || $studentId == null || $studentId.length == 0 || $hdnIsAdministrationTimeAssessment == false || $enForceChekTimeWarning == true) {
                    removeEvent();
                }
                else
                    addEvent();
                return;
            },

            alertmsg = function () {
                $isTimeAllottedContinue = true;
                if ($isTimeAllottedChnaged == true && $isTimeExtnChanged == true && $isTimeAllottedNTimeExtnChanged==true)
                    validate();
                if ($isTimeAllottedChnaged == true && $isTimeExtnChanged == false)
                    validate();
            },

             isAutoSubmitChecked = function (objCheckBox, isChecked) {

                     if (isChecked == false) {
                         objCheckBox.checked = false;
                         return false;
                     }
                     if (isChecked == true)
                         customDialog({ title: "Confirm?", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "Are you certain you would like to automatically submit all in progress assessments once time has expired?  Select Continue to proceed or Cancel to decline the selection.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel", callback: setValue }, { title: "Continue" }]);
             },
              isTimeAssessmentChecked = function (objCheckBox, isChecked) {
                  $isClickedTimeAssessment = true;
                      if (isChecked == false) {
                          objCheckBox.checked = false;
                          return false;
                      }
                      if (isChecked == true) {
                          objCheckBox.checked = true;
                          return false;
                      }

              },

             setValue = function () {
                 $chkboxAutoSubmit.prop('checked', false);
             },

           isChkdTimedAssessment = function () {
               if ($('#ChkboxTimeAssessment').is(':checked')) {
                   unhideControls();
                   SaveData();
                   return flase;
               }
           },
           SaveData = function () {
               if ($timeExtn.val() != "") {
                   $.ajax({
                       type: "POST",
                       url: "AssessmentAdministration.aspx/SaveTimedAssessment",
                       data: " {'testId':" + $assessmentId + ",'isTimeAessessment':'" + $chkboxTimeAssessment.is(':checked') + "','isAutoSubmit':'" + $chkboxAutoSubmit.is(':checked') + "','timeExtn':'" + $timeExtn.val() + "','timeAllotedVal':'" + $timeAlloted.val() + "','studentIds':'" + $studentId + "','TestEventId':'" + $testId + "'}",
                       contentType: "application/json; charset=utf-8",
                       dataType: "json",

                       error: function (XMLHttpRequest, textStatus, errorThrown) {
                           // console.log("responseText: " + XMLHttpRequest.responseText);
                       },
                       success: function (result) {
                           var data = [];
                           if (result && result.d) {
                               $isSucessfull = true;
                               $('#Btnrefresh1').click();
                           }
                       }
                   });
               }
               else {
                   $.ajax({
                       type: "POST",
                       url: "AssessmentAdministration.aspx/SaveTimedAssessment",
                       data: " {'testId':" + $assessmentId + ",'isTimeAessessment':'" + $chkboxTimeAssessment.is(':checked') + "','isAutoSubmit':'" + $chkboxAutoSubmit.is(':checked') + "','timeExtn':'" + $timeExtn.val() + "','timeAllotedVal':'" + $timeAlloted.val() + "','studentIds':'" + $allStudentIdsList + "','TestEventId':'" + $testId + "'}",
                       contentType: "application/json; charset=utf-8",
                       dataType: "json",

                       error: function (XMLHttpRequest, textStatus, errorThrown) {
                           // console.log("responseText: " + XMLHttpRequest.responseText);
                       },
                       success: function (result) {
                           var data = [];
                           if (result && result.d) {
                               $isSucessfull = true;
                               $('#Btnrefresh1').click();
                           }
                       }
                   });
               }
               //}
           },
   timeallottedvalidation = function (val) {
           var timeAllotedVal = $.trim($timeAlloted.val());
           var parsedTimeAlloed = parseInt(timeAllotedVal);
           var timeExtnVal = $("#rtbTimeExtn").val();
           var parsedtimeExtn = parseInt(timeExtnVal);
           var flag = false;
           if (timeAllotedVal != "") {
               if (parsedTimeAlloed >= $timeAllottedVal && parsedTimeAlloed <= 420) {
                   if ($('#ChkboxTimeAssessment').is(':checked') == false) {
                       cancelArtmagForTimeExtension();
                       cancelAlertMsgForAllotted();
                       validate();
                       return false;
                   }

                   if (timeExtnVal != "") {
                       {
                           $isTimeExtnChanged = true;
                           textChangeForTimeExtension();
                           flag = true;
                       }
                   }
                   if ($txtchange != undefined) {
                       if ($timeAllottedVal != $txtchange) {
                           $isTimeAllottedChnaged = true;
                           textchange(val);
                           flag = true;
                       }
                   }
                
                   if (flag == false) {
                           validate();
                   }
               }
               else {
                   flag = false;
                   customDialog({ title: 'Warning', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: 'The value entered cannot be less than the original time allotted or greater than 420 minutes.' }, [{ title: 'OK' }]);
                   return flag;
               }
           }
           else {
               flag = false;
               customDialog({ title: 'Warning', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: 'Time alloted is required field.' }, [{ title: 'OK' }]);
               return flag;
           }
   },

  timedColumnCheckedUnchecked = function (studentId, controlId, keyValue) {
      $objChkBox = controlId;
      $isTimeChecked = keyValue;
      $timedOnCheckedStudentId = studentId;
      customDialog({ title: "Confirm?", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "Are you certain you would like to change the selected students timer status on this assessment.  Select Continue to proceed or Cancel to decline the selection.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel", callback: cancelAlertMsg }, { title: "Continue", callback: continueAlertMsg }]);

  },

  cancelAlertMsg = function () {
      if ($isTimeChecked == true)
          $('#' + $objChkBox).prop('checked', false);
      else
          $('#' + $objChkBox).prop("checked", true);
  },
  continueAlertMsg = function () {
      timedOnCheckChanged($isTimeChecked, $timedOnCheckedStudentId);

  },

  timedOnCheckChanged = function (isTimeChecked, timedOnCheckedStudentId) {
      $.ajax({
          type: "POST",
          url: "AssessmentAdministration.aspx/timedOnCheckChanged",
          data: " {'testEventId':" + $testId + ",'isTimeChecked':" + isTimeChecked + ",'studentId':" + timedOnCheckedStudentId + ",'testId':" + $assessmentId + "}",
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          error: function (XMLHttpRequest, textStatus, errorThrown) {
              // console.log("responseText: " + XMLHttpRequest.responseText);
          },
          success: function (result) {
              var data = [];
              if (result && result.d) {
                  $('#Btnrefresh1').click();
              }
          }
      });
  },

   validate = function () {
       var value = $('#ChkboxTimeAssessment').is(':checked');
       var flag = true;
       if (value == false && $hdnIsAdministrationTimeAssessment==true) {
           customDialog({ title: "Confirm?", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "Completing this selection will disable all other assessment timer related selections.  Select Continue to proceed or Cancel to decline the selection.<br/><br/>", dialog_style: 'confirm' }, [{ title: "Cancel", callback: cancelContinueDetails }, { title: "Continue", callback: continueDetails }]);
           flag = true;
           return flag;
       }
       if (value == false && $hdnIsAdministrationTimeAssessment == false) {
           SaveData();
       }
       else if ((value == true)) {
           SaveData();
           if ($isSucessfull == true)
               unhideControls();
           flag = true;
           return flag;
       }

   };
    return {
        "Init": init,
        "Validate": validate,
        "IsChkdTimedAssessment": isChkdTimedAssessment,
        "Timeallottedvalidation": timeallottedvalidation,
        "StudentIdOnRowSelect": studentIdOnRowSelect,
        "IsAutoSubmitChecked": isAutoSubmitChecked,
        "IsTimeAssessmentChecked": isTimeAssessmentChecked,
        "TimedColumnCheckedUnchecked": timedColumnCheckedUnchecked
    };
}();