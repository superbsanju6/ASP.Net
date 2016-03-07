<%@ Page Title="Item Search" Language="C#" AutoEventWireup="True" CodeBehind="ItemSearch.aspx.cs" Inherits="Thinkgate.Controls.Items.ItemSearch" MasterPageFile="~/Search.Master"%>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx"  %>
<%@ Register TagPrefix="e3" TagName="GradeSubjectCourseStandardSet" Src="~/Controls/E3Criteria/GradeSubjectCourseStandardSet.ascx"  %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx"  %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextWithDropdown" Src="~/Controls/E3Criteria/TextWithDropdown.ascx" %>
<%@ Register TagPrefix="e3" TagName="SearchPager" Src="~/Controls/E3Criteria/SearchPager.ascx" %>
<%@ Register TagPrefix="e3" TagName="SortBar" Src="~/Controls/E3Criteria/SortBar.ascx" %>
<%@ Register TagPrefix="e3" TagName="AddendumType" Src="~/Controls/E3Criteria/AddendumType.ascx" %>
<%@ Register TagPrefix="e3" TagName="AddendumSearch" Src="~/Controls/E3Criteria/AddendumSearch.ascx" %>
<%@ Register TagPrefix="e3" TagName="DateRange" Src="~/Controls/E3Criteria/DateRange.ascx" %>
<%@ Register TagPrefix="e3" TagName="DifficultyRange" Src="~/Controls/E3Criteria/DifficultyRange.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextControl" Src="~/Controls/E3Criteria/Text.ascx" %>
<%@ Register TagPrefix="e3" TagName="Duration" Src="~/Controls/E3Criteria/Duration.ascx" %>
<%@ Register TagPrefix="e3" TagName="StandardGradeSubjectCourseSet" Src="~/Controls/E3Criteria/StandardGradeSubjectId.ascx" %>
<%@ Register Src="~/Controls/E3Criteria/ExpirationStatusAndDateRange.ascx" TagPrefix="e3" TagName="ExpirationStatusAndDateRange" %>
<%@ MasterType virtualpath="~/Search.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <div id="hidden_content" style="display: none">
        <asp:Image CssClass="flag_img flag_yellow" ID="flag_yellow_template" runat="server" ImageUrl="~/Images/commands/flag_yellow.png" ToolTip="Item already added to this assessment" ClientIDMode="Static"/>
    </div>
    <e3:SortBar ID="SortBar" runat="server" OnSort="SortHandler"/> 
       
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results"/>
    
    <asp:HiddenField ID="hidRefresh" ClientIDMode="Static" runat="server" Value="false" />
    <asp:HiddenField ID="hidItemID" ClientIDMode="Static" runat="server" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
   <telerik:RadTabStrip ID="rtsSearch" runat="server" MultiPageID="rmpSearch" SelectedIndex="0" Skin="Office2010Blue">
        <Tabs>
            <telerik:RadTab runat="server" TabIndex="0" Text="Basic" PageViewID="rpBasic"></telerik:RadTab>
            <telerik:RadTab runat="server" TabIndex="1" Text="Advanced" PageViewID="rpAdvanced"></telerik:RadTab>
            <telerik:RadTab runat="server" TabIndex="2" Text="Tags" PageViewID="rpLRMI" ></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage runat="server" ID="rmpSearch">
        <telerik:RadPageView runat="server" ID="rpBasic" Selected="True">
            <e3:GradeSubjectCourseStandardSet runat="server" ID="ctrlGradeSubjectCourseStandardSet" CriteriaName="GradeSubjectCourseStandardSet"/>
            <e3:CheckBoxList ID="cblItemBank" CriteriaName="ItemBank" runat="server" Text="Item Bank" DataTextField="Label" DataValueField="Label"/>
            <e3:CheckBoxList ID="cblItemReservation" CriteriaName="ItemReservation" runat="server" Text="Item Reservation" DataTextField="Name" DataValueField="Value"/>
            <e3:DropDownList ID="cmbStandardFilter" CriteriaName="StandardFilter" runat="server" Text="Standard Filter" EmptyMessage="None" DataTextField="Key" DataValueField="Value"/>
            <e3:TextWithDropdown ID="txtSearch" CriteriaName="TextSearch" Text="Text Search" runat="server" DataTextField="Name"/>    
            <e3:CheckBoxList ID="cmbRatingFilter" CriteriaName="RatingFilter" runat="server" Text=" Average Rating" EmptyMessage="None" DataTextField="DisplayValue" DataValueField="Id" />
            <input id="lbl_OTCUrl" type="hidden" runat="server" clientidmode="Static" />            
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="rpAdvanced">
            <e3:CheckBoxList ID="cblItemType" CriteriaName="ItemType" runat="server" Text="Item Type" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList> 
            <e3:CheckBoxList ID="cblScoreType" CriteriaName="ScoreType" runat="server" Text="Score Type" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList>  
            <e3:AddendumSearch ID="ctrlAddendumSearch" CriteriaName="Addendum" Text="Addendum" runat="server"/>
            <e3:AddendumType ID="ctrlAddendumType" CriteriaName="AddendumType" Text="Addendum Type" runat="server" ChildDataTextField="Genre" ChildDataValueField="Genre"/>
            <e3:DateRange ID="drCreatedDate" CriteriaName="CreatedDate" Text="Created Date" runat="server"/>
            <e3:ExpirationStatusAndDateRange CriteriaName="ExpirationStatusAndDateRange" runat="server" id="ExpirationStatusDateRange" ContentType="Items"/>
            <e3:CheckBoxList ID="cblItemReviewStatus" CriteriaName="ItemReviewStatus" runat="server" Text="Item Status" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList>  
            <e3:CheckBoxList ID="cblDifficultyIndex" CriteriaName="DifficultyIndex" runat="server" Text="Difficulty Index" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList> 
            <e3:DifficultyRange ID="drDifficulty" CriteriaName="Difficulty" Text="Difficulty" runat="server"/>
            <e3:CheckBoxList ID="cblRigor" CriteriaName="Rigor" runat="server" Text="Rigor" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList>   
            <e3:DropDownList ID="ddlAnchorItem" CriteriaName="AnchorItem" runat="server" Text="Anchor Item" EmptyMessage="None" DataTextField="Name" DataValueField="Value" />
            &nbsp;
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="rpLRMI">
            <e3:CheckBoxList ID="cbGradeLevel" CriteriaName="GradeLevel" runat="server" Text="Grades" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList>
            <e3:CheckBoxList ID="cbEducationalSubject" CriteriaName="EducationalSubject" runat="server" Text="Educational Subject" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList>
            <e3:CheckBoxList ID="cbLearningResource" CriteriaName="LearningResource" runat="server" Text="Learning Resource" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList>
            <e3:CheckBoxList ID="cbEducationalUse" CriteriaName="EducationalUse" runat="server" Text="Educational Use" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList>
            <e3:CheckBoxList ID="cbEndUser" CriteriaName="EndUser" runat="server" Text="End User" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList>
            <e3:TextControl ID="txtCreator" CriteriaName="Creator" runat="server" Text="Creator" DataTextField="Name" DataValueField="Value"></e3:TextControl>
            <e3:TextControl ID="txtPublisher" CriteriaName="Publisher" runat="server" Text="Publisher" DataTextField="Name" DataValueField="Value"></e3:TextControl>
            <e3:DropDownList ID="cmbUsageRight" CriteriaName="UsageRight" runat="server" Text="Usage Rights" EmptyMessage="None" DataTextField="Name" DataValueField="Value"/>
            <e3:CheckBoxList ID="cbMediaType" CriteriaName="MediaType" runat="server" Text="Media Type" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList>
            <e3:DropDownList ID="cmbLanguage" CriteriaName="Language" runat="server" Text="Language" EmptyMessage="None" DataTextField="Name" DataValueField="Value"/>
            <e3:DropDownList ID="cmbAgeAppropriate" CriteriaName="AgeAppropriate" runat="server" Text="Age Appropriate" EmptyMessage="None" DataTextField="Name" DataValueField="Value"/>
            <e3:Duration ID="txtDuration" CriteriaName="Duration" runat="server" Text="Time Required" DataTextField="Name" DataValueField="Value"></e3:Duration>
            <e3:StandardGradeSubjectCourseSet ID="cmbStandardSet" CriteriaName="StandardSetId" runat="server" Text="Assessed"></e3:StandardGradeSubjectCourseSet>
            <e3:TextControl ID="txtTextComplexity" CriteriaName="TextComplexity" runat="server" Text="Text Complexity" DataTextField="Name" DataValueField="Value"></e3:TextControl>
            <e3:TextControl ID="txtReadingLevel" CriteriaName="ReadingLevel" runat="server" Text="Reading Level" DataTextField="Name" DataValueField="Value"></e3:TextControl>
            <e3:CheckBoxList ID="cbInteractivity" CriteriaName="Interactivity" runat="server" Text="Interactivity Type" DataTextField="Name" DataValueField="Value"></e3:CheckBoxList>
             &nbsp;
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    
      <script type="text/javascript">

          function InsertAddendum(AddendumID, AddendumName, AddendumType, AddendumGenre) {
              var criteriaName = "Addendum";

              var valueObject = {};
              valueObject.Text = AddendumName;
              valueObject.Value = AddendumID;
              valueObject.Name = AddendumName;
              valueObject.ID = AddendumID;
              valueObject.Type = AddendumType;
              valueObject.Genre = AddendumGenre;

              if ((valueObject.ID).length < 1) {
                  // we've cleared the text box, clear them
                  CriteriaController.RemoveAll(criteriaName);
              } else {
                  CriteriaController.Add(criteriaName, valueObject);
              }
          }
      </script>
   
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <div id="data" style="overflow: auto;" onscroll="onDataScroll();" ontouchmove="onDataScroll();">
    </div>
     <div id="divDefaultMessage" style="height: 100%; text-align: center;">
            <div style="height: 40%"></div>
            <div style="height: 20%">
            <strong>Please select at least 2 criteria.</strong>
            </div>
            <div style="height: 40%"></div>
        </div>
    <e3:SearchPager ID="SearchPager" runat="server" Visible="False"/>    
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
   
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        var ItemArray = [];
        var SortField = "";

        var specificItem_refresh_needed = null;

        var initialRecordsToDisplay = 100;
        var baseRecordHeight = 210;
        var recordsPerPage = 100;
        var RenderListArray = [];
        var bScrollLoading = true;  //Set to true if you want results to render as user scrolls

        $(function () {
            $("#criteriaListTabs").tabs();
        });

        $(document).ready(function () {
            resizeContent();
            $("#data").html("");
            $("#data").hide();
            $.templates("Tmpl", {
                markup: "#Template",
                allowCode: true
            });
        });

        $.views.helpers({
            formatDate: function (val) {
                if (val) {
                    var date = new Date(parseInt(val.replace("/Date(", "").replace(")/", ""), 10));
                    return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
                }

                return null;
            },
            VerifyIBEditAccess: function (val) {
                if (CanUserUseItemBank(val, IBEditArray)) {
                    return true;
                }
                return false;
            },
            VerifyExpiryDate: function (val) {
                if (val) {
                    var CopyRightDate = new Date(parseInt(val.replace("/Date(", "").replace(")/", ""), 10));
                    var today = new Date();
                    if (CopyRightDate < today) {
                        return false;
                    }
                }
                return true;
            },
            VerifyIBCopyAccess: function (val) {
                if (getURLParm("ItemSearchMode") != '')
                    return false;
                if (CanUserUseItemBank(val, IBCopyArray)) {
                    return true;
                }
                return false;
            },
            formatAvgRating: function (val) {
                if (val) {
                    return parseFloat(Math.round(val * 100) / 100).toFixed(2);
                }
            }
        });

        function CanUserUseItemBank(itemID, IBarray) {
            var isReturn = false;
            $.each(ItemArray[currentPage - 1], function (i) {
                if ($(this).attr('ID') == itemID) {
                    if ($(this)[0].ItemBankList) {
                        $.each($(this)[0].ItemBankList, function (p) {
                            var targettype = $(this)[0].TargetType;
                            var label = $(this)[0].Label;
                            $.each(IBarray[0].ItemBanks, function (r) {
                                if ($(this)[0].TargetType == targettype && $(this)[0].Label == label) {
                                    isReturn = true;
                                    return true;
                                }
                            });
                            if (isReturn == true)
                                return true;
                        });
                    }
                }
                if (isReturn == true)
                    return true;
            });
            return isReturn;
        }

        $(window).resize(function () {
            resizeContent();
        });

        function resizeContent() {
            var pagerHeight = $("#SearchPager").height() + 4 || 0;
            $("#data").height($(window).height() - $("#topBar").height() - pagerHeight);
        }

        function expandItem(questionID) {
            //functionIsInDevelopment();
            window.open('<% =ResolveUrl("~/Record/BankQuestionPage.aspx?xID=") %>' + questionID);
        }


        function showReviewSummary(itemId)
        {
            var height = window.$telerik.$(window).height();
            var width = window.$telerik.$(window).width();

            document.getElementById("hidItemID").value = itemId;

            customDialog({
                url: ('../Rating/ItemReviewSummaryScreen.aspx?itemId=' + itemId),
                width: width,
                height: height,
                movable: false,
                center: true,
                title: 'Review Summary',
                onClosed: refreshData
            });
        }

        function refreshData() {
            var hidRefreshValue = document.getElementById('hidRefresh');
            var hidItemId = document.getElementById("hidItemID");

            if (hidRefreshValue.value == "true") {
                GetReviewAverageAndCount(hidItemId.value);
            }
        }

        function GetCallbackResult(args, context)
        {
            var hidItemId = document.getElementById("hidItemID");
            var hidRefreshValue = document.getElementById('hidRefresh');

            var reviewContainerId = "reviewContainer_" + hidItemId.value;
            var dvReview = document.getElementById(reviewContainerId);

            var xml = args,
                xmlDoc = $.parseXML(xml),
                $xml = $(xmlDoc),
                $average = $xml.find("AverageRating"),
                $count = $xml.find("ReviewCount");

            if ($count.text() == '0') {
                dvReview.innerHTML = " <div class=\"no-reviews\"> No Reviews/Not Rated </div>";
            }
            else {
                dvReview.innerHTML = "<a href=\"javascript:showReviewSummary(" + hidItemId.value + ")\">" + $count.text() + " Reviews - Rating: " + $average.text() + "/5</a>";
            }

            hidRefreshValue.value = "false";
            hidItemId.value = "";
        }

        //Show the rating dialog box.
        function showRatingControl(itemId) {
            var height = window.$telerik.$(window).height();
            var width = window.$telerik.$(window).width();

            document.getElementById("hidItemID").value = itemId;

            var heightCal = height;
            var widthCal = width;  //0.65

            customDialog({
                url: ('../Rating/ItemReviewMainScreen.aspx?itemId=' + itemId + '&source=ReviewMainScreen'),
                width: widthCal,
                height: heightCal,
                movable: false,
                center: true,
                title: 'Rating',
                onClosed: refreshData
            });
        }

        function itemSearchOnlinePreview(itemID) {
            customDialog({
                name: 'OnlinePreview',
                maximize: true,
                resizable: true,
                movable: true,
                title: 'Online Preview',
                url: document.getElementById('lbl_OTCUrl').value + "?assessmentID=0&studentID=0&itemID=" + itemID
            });

        }


        function itemSearchAdvancedItemEdit(itemID) {
            parent.customDialog({ url: '../Controls/Assessment/ContentEditor/ContentEditor_Item.aspx?evokingWindowName=ItemSearch_Expanded' + '&xID=' + itemID, maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorITEM', title: 'Content Editor - ITEM', onClosed: ItemEditor_OnClientClose, destroyOnClose: true });
        }

        function UpdateSingleItem_FromServer(itemID) {
            Service2.GetBankItemJSONById(itemID, function (result_raw) {
                var parsedResults = jQuery.parseJSON(result_raw);
                UpdateSingleItem(parsedResults, itemID);
            }, onSearchFail);
        }

        function ItemEditor_OnClientClose() {
            if (specificItem_refresh_needed) {
                UpdateSingleItem_FromServer(specificItem_refresh_needed);
                specificItem_refresh_needed = null;
            }
        }

        function UpdateSingleItem(itemJSON, itemID) {
            $.each(ItemArray[currentPage - 1], function (i) {
                if ($(this).attr('ID') == itemID) {
                    ItemArray[currentPage - 1][i] = itemJSON;
                    return false;
                }
            });

            $('#container_' + itemID).html($.render.Tmpl(itemJSON));
        }

        function CopyItem(id) {
            Service2.CopyItemToUserPersonalBank(id,
                    function (result_raw) {
                        parent.radalert('The selected item has been copied and added to your personal bank.');
                    },
                    copyToItemBank_CallBack_Failure);

            return false;
        }

        function copyToItemBank_CallBack_Failure() {
        }

        function renderData(addendumList) {
            if (bScrollLoading) {
                RenderListArray = [];
                var tempArray = [];
                for (var y = 0; y < (addendumList.length < initialRecordsToDisplay ? addendumList.length : initialRecordsToDisplay) ; y++) {
                    tempArray[y] = addendumList[y];
                    RenderListArray.push([y]);
                }
                $('#data').html($.render.Tmpl(tempArray));

                var fillerDiv = '';
                for (var z = initialRecordsToDisplay; z < (addendumList.length > initialRecordsToDisplay ? addendumList.length : initialRecordsToDisplay) ; z++) {
                    fillerDiv += '<div id=\'fillerDiv_' + z + '\' style=\'height:' + baseRecordHeight + 'px;border-bottom: 1px solid black;\'>' + z + '. Loading...</div>';
                }

                document.getElementById('data').innerHTML = document.getElementById('data').innerHTML + fillerDiv;
                document.getElementById('data').scrollTop = 0;
            }
            else {
                $('#data').html($.render.Tmpl(addendumList));
            }
            updateBankFlags();
            reCheckItemSelection();
        }

        function reCheckItemSelection() {
            for (var i = 0; i < itemSelection.length; i++) {
                var itemID = itemSelection[i].substring(0, itemSelection[i].indexOf("_"));
                if ($('#chk_' + itemID).length != 0) {
                    $('#chk_' + itemID).attr("checked", "checked");
                }
            }
        }

        function onDataScroll() {
            if (bScrollLoading) {
                var addendumList = ItemArray[currentPage - 1];
                var c = (document.getElementById('data').scrollTop + (baseRecordHeight * initialRecordsToDisplay)) / baseRecordHeight;
                for (var y = 0; y < (addendumList.length < c ? addendumList.length : c) ; y++) {
                    if (!RenderListArray[y]) {
                        $('#fillerDiv_' + y).css('border-bottom', '0px');
                        $('#fillerDiv_' + y).css('height', 'auto');
                        $('#fillerDiv_' + y).html($.render.Tmpl(addendumList[y]));
                        RenderListArray.push([y]);
                    }
                }
            }
        }

        function onSearchFail(error) {
            //BUG 21109: show error message only if _statusCodde is not Zero(0).
            if (error._statusCode != 0) {
                alert(error._message);
            }
            callInProgress = false;
            HideSpinner();
        }

        function btnSelectReturn_Click(sender, args) {
            parent.itemSearchAddList = [];
            for (var i = 0; i < itemSelection.length; i++) {
                var itemID = itemSelection[i].substring(0, itemSelection[i].indexOf("_"));
                var standardID = itemSelection[i].substring(itemSelection[i].indexOf("_") + 1, itemSelection[i].length);
                parent.itemSearchAddList.push([itemID, standardID]);
            }
            closeCurrentCustomDialog();
        }

        function btnSingleSelect_Click(selected_icon) {
            var $item = $(selected_icon).closest('.innerContainer');
            var itemId = $item.attr('id');
            parent.itemSearchAddList = [itemId];
            closeCurrentCustomDialog();
        }

        function updateBankFlags() {
            if (parent && parent.AssessmentItems && parent.AssessmentItems.length > 0) {
                var assessmentItems = parent.AssessmentItems;
                var $flag_yellow_template = $($("#flag_yellow_template").get(0));
                $(".innerContainer").each(function (index, elm) {
                    var $elm = $(elm);
                    for (var j = 0; j < assessmentItems.length; j++) {
                        if ($elm.attr('id') == assessmentItems[j].SharedContentID) {
                            if ($elm.find(".flag_yellow").length > 0) continue;
                            $flag_yellow_template.clone().insertAfter($elm.find(".chkDiv"));
                            break;
                        }
                    }

                });
            }
        }

        function pageSpecificSearch(prefetch, pagenumber) {
            Service2.QuestionSearch(prefetch, pagenumber, 100, CriteriaController.ToJSON(), SortField, getURLParm("ItemSearchMode"), TestYear, TestCurrCourseID, function (results) {
                onSearchSuccess(prefetch, results, pagenumber);
            }, onSearchFail);
        }

        function OpenAddendumText(id) {
            customDialog({ url: ('../Assessment/ContentEditor/ContentEditor_Item_AddendumText.aspx?xID=' + id), autoSize: true, name: 'ContentEditorItemAddendumText' });
        }

        function OpenRubricText(id) {
            customDialog({ url: ('../Rubrics/RubricText.aspx?xID=' + id), autoSize: true, name: 'RubricText' });
        }

        function RenderFirstPage() {
            currentPage = 1;
            renderData(ItemArray[0]);
            resizeContent();
            preFetchNext(2);
            $('#data').show();
            $('#divDefaultMessage').hide();
        }

        var itemSelection = [];
        function addItemToSelection(itemID, standardID) {
            if (itemSelection.indexOf(itemID + "_" + standardID) > -1) {
                itemSelection.splice(itemSelection.indexOf(itemID + "_" + standardID), 1);
            }
            else {
                itemSelection.push(itemID + "_" + standardID);
            }
        }


    </script>
    <style type="text/css">
        .rubricSpan {
            white-space: pre-wrap; /* css-3 */    
            white-space: -moz-pre-wrap; /* Mozilla, since 1999 */
            white-space: -pre-wrap; /* Opera 4-6 */    
            white-space: -o-pre-wrap; /* Opera 7 */    
            word-wrap: break-word; /* Internet Explorer 5.5+ */
        }
    </style>
    <script id="Template" type="text/x-jsrender">
       <div id="container_{{:ID}}" class="tblContainer" style="width:100%; padding-right: 17px;">
            <div class="innerContainer" id="{{:ID}}" StandardID="{{if Standards && Standards.length > 0}}{{:Standards[0].ID}}{{/if}}" style="border-bottom: 1px solid black; padding-left: 10px; margin-top: 10px;">
                <div style="display: table">
                    <div style="display: table-row">
                        <div style="display: table-cell">
                            <img src="<%=ImageWebFolder%>select_arrow.png" style="cursor: pointer; <%=HideIfNotSingleSelect %>;  position: relative; left: -5px;" onclick="btnSingleSelect_Click(this);"/>
                            <span class="chkDiv" style="display: inline-block; position: relative; left: -5px;" onclick="addItemToSelection('{{:ID}}', '{{if Standards && Standards.length > 0}}{{:Standards[0].ID}}{{else}}0{{/if}}')">
                                <input id="chk_{{:ID}}" class="chkSelect" type="checkbox" style="<%=HideIfNotMultiSelect %>;" />
                            </span> 
                            {{if OnAnyTest}}
                                <img class="flag_img" src="<%=ImageWebFolder%>commands/flag_blue.png" ToolTip="Item already exists on proofed assessment" />
                            {{/if}}   
                        </div>
                        <div style="display: table-cell; width: 100%;">
                            <div style=" border: 1px solid black; box-shadow: 10px 10px 5px   #888; width: 100%;">
                                <div style="width: 100%; vertical-align: top; margin-bottom:  10px;">
                                    <div style="width: 80%;">{{:Question_Text}}</div>
                                    <div style="float: right; width: 20%; position: absolute; top: 0; right: 0; ">
                                        <div class="itemActions" style="background-position: -62px 0px; width: <%=ExpandPermIconWidth %>;" <%=ExpandPermOnclick %> title="Expand"></div>
                                            
                                       
                                             
                                               {{if ~VerifyIBEditAccess(ID) && ((Copyright != "Yes") || (Copyright == "Yes" && ~VerifyExpiryDate(CopyRightExpiryDate)))}}
                                         <div class="itemActions" style="background-position: -92px 0px; width: 19px;" onclick="itemSearchAdvancedItemEdit('{{:IDEncrypted}}');" title="Advanced Edit"></div>
                                             {{/if}}
                                         {{if ~VerifyIBCopyAccess(ID) && ((Copyright != "Yes") || (Copyright == "Yes" && ~VerifyExpiryDate(CopyRightExpiryDate)))}}
                                            <div class="itemCopyActions" style="width:19px;" onclick="CopyItem('{{:IDEncrypted}}');" title="Copy"></div>
                                        {{/if}}
                                       
                                        <img class="itemRating" width="17px" height="16px" src="../../Images/icons/RatingIcon.png" onclick="showRatingControl({{:ID}})" alt="Rating" title="Rating" />                                        

                                        <div class="itemActions" style="width: 17px;"  onclick="itemSearchOnlinePreview({{:ID}});" title="Online Preview"></div>
                                    </div>
                                </div>
                                <div style="margin-left: 40px;  width: 90%;">
                                        {{for Responses}}
                                            <table>
                                                <tr>
                                                    <td style="vertical-align:top" ><input type="radio" value="{{:ID}}" {{if (Correct)}} checked="checked" {{/if}} disabled="disabled" /></td>
                                                    <td style="vertical-align:top"><span style="font-weight: bold;">{{:Letter}}.</span></td>
                                                    <td style="vertical-align:top"><span style="display: inline-block;width:580px;">{{:DistractorText}}</span></td>
                                                </tr>
                                            </table>
                                            <br/>
                                        {{/for}}
                                 </div>
                            </div>
                            <div style="margin-top: 10px; margin-bottom: 10px;">
                                <table class="questionInformation" style="width: 100%; table-layout: fixed;">
                                    <colgroup>
                                        <col style="width: 140px;" />
                                        <col style="background-color:#efefef; width:140px;"/>
                                        <col style="width:160px;"/>
                                        <col style="background-color:#efefef; width:110px;"/>
                                        <col style="width:150px;"/>
                                    </colgroup>
                                    <tr>
                                        <td>
                                            Author: <span>{{:CreatedByName}}</span>
                                        </td>
                                        <td>
                                            Date Created: <span>{{:~formatDate(DateCreated)}}</span>
                                        </td>
                                        <td>
                                            Last Updated: <span>{{:~formatDate(DateUpdated)}}</span>
                                        </td>
                                        <td>
                                            Item ID: <span>{{:ID}}</span>
                                        </td>
                                        <td>
                                            3rd Party ID: <span>{{:VendorId}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Item Status: <span>{{:Status}}</span>
                                        </td>
                                        <td>
                                            Item Reservation: <span>{{:TestType}}</span>
                                        </td>
                                        <td>
                                            Difficulty Index: <span>{{:DifficultyIndex.Text}}</span>
                                        </td>
                                        <td>
                                            Difficulty: {{if ItemDifficulty > 0}}<span>{{:ItemDifficulty}}</span>{{/if}}
                                        </td>
                                        <td>
                                            Rigor: <span>{{:Rigor.Text}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Addendum: <span>{{if Addendum}}<a href="" onclick="OpenAddendumText('{{:EncryptedID}}'); return false;">{{:Addendum.Addendum_Name}}</a>{{/if}}</span>
                                        </td>
                                        <td>
                                            Rubric: <span class="rubricSpan">{{if Rubric}}<a href="" onclick="OpenRubricText('{{:Rubric.ID}}'); return false;">{{:Rubric.Name}}</a>{{/if}}</span>
                                        </td>
                                        <td>
                                            Standards:
                                            {{if Standards}}
                                                <ul class="unorderedList">
                                                    {{for Standards}}
                                                        <li>
                                                            {{:StandardName}}
                                                        </li>
                                                    {{/for}}
                                                </ul>
                                            {{/if}}
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            Item Bank:
                                            {{if ItemBankList}}
                                                <ul class="unorderedList">
                                                    {{for ItemBankList}}
                                                        <li>
                                                            {{:Label}}
                                                        </li>
                                                    {{/for}}
                                                </ul>
                                            {{/if}}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Copyright: <span>{{if Copyright == null || Copyright == ""}} No {{else}} {{:Copyright}}{{/if}}</span>
                                        </td>
                                        <td>
                                            Expiration Date: <span>{{if CopyRightExpiryDate}} {{:~formatDate(CopyRightExpiryDate)}} {{else}} Not defined {{/if}}</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td align="left" colspan="2" bgcolor="White">
                                            <div id="reviewContainer_{{:ID}}">
                                                {{if NumRatings != 0}}
                                                    <a href="javascript:showReviewSummary({{:ID}});">
                                                        {{:NumRatings}} Reviews - Rating: {{:~formatAvgRating(AverageRating)}}/5
                                                   </a>
                                                {{else}} 
                                                    <div class="no-reviews" style="">
                                                        No Reviews/Not Rated
                                                    </div>
                                                {{/if}}
                                             </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div> <!-- cell -->
                    </div> <!-- row -->
                </div> <!-- table -->
            </div>
        </div>
    </script>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Styles/ItemBanks/Inspect.css" rel="stylesheet" type="text/css"/>
    <link href="../../Styles/ItemBanks/MCAS.css" rel="stylesheet" type="text/css"/>
    <link href="../../Styles/ItemBanks/NWEA.css" rel="stylesheet" type="text/css"/>
    <style type="text/css">

        div.RadToolTip {
            position: fixed !important;
        }

        .no-reviews {
            font-weight: bold;
            color: black;
        }

        .unorderedList {
            font-weight: normal;
            list-style-type: square;
            margin: 0px 0px 0px 15px;
            padding: 0px 0px 0px 0px;
        }

        #rightColumn {
            background-color: white;
        }

        .questionInformation td {
            font-weight: bold;
            vertical-align: top;
        }

        .questionInformation span {
            display: block;
            margin: 0px 0px 10px 10px;
            font-weight: normal;
            font-size: small;
        }

        .itemFieldName {
            font-family: arial;
            font-weight: bold;
            font-size: 8pt;
            padding-left: 10px;
            padding-right: 3px;
            padding-top: 2px;
            float: left;
        }

        .itemActions {
            height: 16px;
            background-repeat: no-repeat;
            background-image: url('../../Images/AssessmentItemIcons.png');
            float: right;
            margin-right: 5px;
            cursor: pointer;
        }

        .itemRating {
            background-repeat: no-repeat;
            float: right;
            margin-right: 5px;
            cursor: pointer;
        }

        .itemCopyActions {
            height: 16px;
            background-repeat: no-repeat;
            background-image: url('../../Images/copy.gif');
            float: right;
            margin-right: 5px;
            cursor: pointer;
        }

        .flag_img {
            position: relative;
            left: -5px;
            margin-top: 5px;
        }

        .innerContainer .class26 td.class9{
            height:0px;
        }
    </style>
</asp:Content>