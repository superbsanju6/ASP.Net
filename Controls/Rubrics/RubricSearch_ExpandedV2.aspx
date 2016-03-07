<%@ Page Title="Rubric Search" Language="C#" AutoEventWireup="true" CodeBehind="RubricSearch_ExpandedV2.aspx.cs" Inherits="Thinkgate.Controls.Rubrics.RubricSearch_ExpandedV2" MasterPageFile="~/Search.Master" %>

<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="GradeSubjectCourseStandardSet" Src="~/Controls/E3Criteria/GradeSubjectCourseStandardSet.ascx" %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextWithDropdown" Src="~/Controls/E3Criteria/TextWithDropdown.ascx" %>
<%@ Register TagPrefix="e3" TagName="SearchPager" Src="~/Controls/E3Criteria/SearchPager.ascx" %>
<%@ Register TagPrefix="e3" TagName="SortBar" Src="~/Controls/E3Criteria/SortBar.ascx" %>
<%@ Register TagPrefix="e3" TagName="DateRange" Src="~/Controls/E3Criteria/DateRange.ascx" %>
<%@ Register Src="~/Controls/E3Criteria/ExpirationStatusAndDateRange.ascx" TagPrefix="e3" TagName="ExpirationStatusAndDateRange" %>
<%@ MasterType VirtualPath="~/Search.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <e3:SortBar ID="SortBar" runat="server" OnSort="SortHandler" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:GradeSubjectCourseStandardSet runat="server" ID="ctrlGradeSubjectCourseStandardSet" CriteriaName="GradeSubjectCourseStandardSet" />
    <e3:CheckBoxList ID="ctrlRubricType" CriteriaName="RubricType" Text="Rubric Type" runat="server" />
    <e3:CheckBoxList ID="cblItemBank" CriteriaName="ItemBank" runat="server" Text="Item Bank" DataTextField="Label" DataValueField="Label" />
    <e3:DateRange ID="drCreatedDate" CriteriaName="CreatedDate" Text="Created Date" runat="server" />
    <e3:ExpirationStatusAndDateRange CriteriaName="ExpirationStatusAndDateRange" runat="server" ID="ExpirationStatusDateRange" ContentType="Rubrics" />
    <e3:TextWithDropdown ID="txtSearch" CriteriaName="TextSearch" Text="Text Search" runat="server" DataTextField="Name" />
    <telerik:RadXmlHttpPanel runat="server" ID="copyRubricXmlHttpPanel" ClientIDMode="Static" EnableAjaxSkinRendering="true" OnServiceRequest="copyRubricXmlHttpPanel_ServiceRequest" RenderMode="Block" OnClientResponseEnding="newRubricMessage"></telerik:RadXmlHttpPanel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <div id="data" style="overflow: auto;" onscroll="onDataScroll();" ontouchmove="onDataScroll();">
    </div>
    <e3:SearchPager ID="SearchPager" runat="server" Visible="False" />
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        var ItemArray = [];
        var SortField = "";
        var specificItem_refresh_needed = null;

        var initialRecordsToDisplay = 8;
        var baseRecordHeight = 100;
        var RenderListArray = [];
        var bScrollLoading = true;  //Set to true if you want results to render as user scrolls
        var newAndReturnType;        

        $(document).ready(function () {
            newAndReturnType = getURLParm("NewAndReturnType") != "" ? true : false;
            resizeContent();
            $("#data").html("");
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
            VerifyIBCopyAccess: function (val) {
                if (CanUserUseItemBank(val, IBCopyArray)) {
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
            escapeField: function (val) {
                return escape(val);
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
            alert('error');
            callInProgress = false;
            HideSpinner();
        }

        function SendResultsToWindow(RubricIDEncrypted, RubricName, RubricType, RubricPoints, CriteriaCount, ID) {
            ShowSpinner("AjaxPanelResults");
            var DialogueToSendResultsTo;
            DialogueToSendResultsTo = parent.getCurrentCustomDialogByName(getURLParm('NewAndReturnType'));
            try {
                DialogueToSendResultsTo.get_contentFrame().contentWindow.InsertRubric(RubricIDEncrypted, RubricName, RubricType, RubricPoints, CriteriaCount, ID);
            }
            catch (e) {
                try {
                    parent.InsertRubric(RubricIDEncrypted, RubricName, RubricType, RubricPoints, CriteriaCount, ID);
                }
                catch (e) {

                }
            }
          
            isServiceComplete();

        }

        function isServiceComplete()
        {
            setInterval(function () {
                if (callInProgress == false) {
                    closeCurrentCustomDialog();
                    setTimeout(isServiceComplete, 0)
                }
                else {
                    isServiceComplete();
                }
            }, 1000);
        }


        function makeAddendumOverflowAuto(div) {
            div.style.overflow = "auto";
            div.style.cursor = "default";
        }

        function OpenAdvancedEditor(enc) {
            //window.open('../Assessment/ContentEditor/ContentEditor_Addendum.aspx?xID=' + enc);
            parent.customDialog({ url: ('<% =ResolveUrl("~/Controls/Assessment/ContentEditor/ContentEditor_Rubric.aspx") %>?xID=' + enc + '&NewAndReturnType=&NewAndReturnID='), maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorRUBRIC', title: 'Content Editor - RUBRIC', destroyOnClose: true });
        }

        function pageSpecificSearch(prefetch, pagenumber) {
            Service2.RubricSearchV2(prefetch, pagenumber, 100, CriteriaController.ToJSON(), SortField, function (results) {
                isResponseCompleted = true; onSearchSuccess(prefetch, results, pagenumber, closeCurrentCustomDialog);
            }, onSearchFail);
        }

        function copyRubric(id) {
            var panel = $find('copyRubricXmlHttpPanel');
            var panelValue = id;
            panel.set_value(panelValue);
        }

        function newRubricMessage(sender, args) {
            parent.radalert('The selected rubric has been copied and added to your personal bank.');
        }

        function RenderFirstPage() {
            currentPage = 1;
            renderData(ItemArray[0]);
            resizeContent();
            preFetchNext(2);
        }

        function previewRubric(rubricID) {
            Service2.RubricGetDirectionsAndContentFormatted(rubricID, previewRubricOnSuccess)
        }

        function previewRubricOnSuccess(result_raw) {
            var previewRubricDirections;
            var previewRubricContent;

            var parsedResults = jQuery.parseJSON(result_raw);
            previewRubricDirections = parsedResults.PayLoad[0];
            previewRubricContent = parsedResults.PayLoad[1];

            var rubricContent = '<div class="previewRubricDirections" style="width: 95%; border: 1px solid black; margin-bottom: 10px;"><div class="fieldLabel" style="text-align: center; margin-bottom: 10px">Rubric Directions</div><div runat="server" id="ContentEditor_Rubric_Preview_Directions" clientidmode="Static">' + previewRubricDirections + '</div></div><div runat="server" id="ContentEditor_Rubric_Preview_Contents" class="previewRubricContent" style="width: 95%; height: auto" clientidmode="Static">' + previewRubricContent + '</div>';
            parent.customDialog({ title: 'Rubric Preview', maximize: true, maxwidth: 800, maxheight: 400, content: "<div style='overflow: auto; height: 400px'>" + rubricContent + "</div>" });
        }        
    </script>
    <script id="Template" type="text/x-jsrender">
        <div class="tblContainer">
            <div class="rubricRow" id="{{:ID}}" valign="top">
                {{* if (newAndReturnType){ }}
						<div class="selectDiv">
                            <img src="../../Images/select.png" style="cursor: pointer;" onclick="SendResultsToWindow('{{:ID_Encrypted}}', $('#rubricName_{{:ID}}').html(), '{{:Type}}', '{{:MaxPoints}}', '{{:CriteriaCount}}', {{:ID}});" /></div>
                {{* } }}
						<div class="rubricDiv" id="rubric_{{:ID}}">{{:Content}}</div>
                <div class="iconsDiv">
                    {{if ((Copyright != "Yes") || (Copyright == "Yes" && ~VerifyExpiryDate(CopyRightExpiryDate)))}}
								{{if ~VerifyIBCopyAccess(ID)}}       
                                   <div class="icons" onclick="copyRubric({{:ID}});" title="Copy"></div>
                    {{/if}}
                                {{if ~VerifyIBEditAccess(ID) }}
									<div class="itemActions" style="background-position: -62px 0px; width: <%=ExpandPerm_IconWidth %>;" <%=ExpandPerm_onclick %> title="Expand"></div>
                    <div class="itemActions" style="background-position: -92px 0px; width: 19px;" onclick="OpenAdvancedEditor('{{:ID_Encrypted}}');" title="Advanced Edit"></div>
                    {{/if}}
                            {{/if}}
								<div class="itemActions" style="width: 21px;" onclick="previewRubric('{{:ID_Encrypted}}')" title="Online Preview"></div>
                </div>
                <br />
                <div class="tblContainer" style="width: 100%; padding-top: 15px;">
                    <div class="tblRow">
                        <div class="tblRight labelTD" style="font-weight: bold;">Name:</div>
                        <div class="tblRight textTD" id="rubricName_{{:ID}}">{{:Name}}</div>
                        <div class="tblRight labelTD" style="font-weight: bold;">Author:</div>
                        <div class="tblRight textTD">{{:CreatedByName}}</div>
                        <div class="tblRight labelTD" style="font-weight: bold;">Date Created:</div>
                        <div class="tblRight textTD">{{:DateCreated}}</div>
                        <div class="tblRight labelTD" style="font-weight: bold;">Last Updated:</div>
                        <div class="tblRight textTD">{{:DateUpdated}}</div>
                    </div>
                    <div class="tblRow">
                        <div class="tblRight labelTD" style="font-weight: bold;">Grade:</div>
                        <div class="tblRight textTD">{{:Grade}}</div>
                        <div class="tblRight labelTD" style="font-weight: bold;">Subject:</div>
                        <div class="tblRight textTD">{{:Subject}}</div>
                        <div class="tblRight labelTD" style="font-weight: bold;">Standard:</div>
                        <div class="tblRight textTD">{{:Course}}</div>
                        <div class="tblRight labelTD" style="font-weight: bold;">Item Bank:</div>
                        <div class="tblRight textTD">{{:ItemBanks}}</div>
                    </div>
                    <div class="tblRow">
                        <div class="tblRight labelTD" style="font-weight: bold;">Copyright:</div>
                        <div class="tblRight textTD">{{if Copyright == null || Copyright == ""}} No {{else}} {{:Copyright}}{{/if}}</div>
                        <div class="tblRight labelTD" style="font-weight: bold;">Expiration Date:</div>
                        <div class="tblRight textTD">{{if CopyRightExpiryDate}} {{:~formatDate(CopyRightExpiryDate)}} {{else}} Not defined {{/if}}</div>

                    </div>
                </div>
            </div>
        </div>
    </script>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .rubricRow {
            display: table-row;
            background-color: #FFF;
            padding: 3px;
            width: 100%;
            height: 150px;
        }

        div.RadToolTip {
            position: fixed !important;
        }

        .selectDiv {
            padding: 3px;
            width: 30px;
            height: 150px;
            text-align: center;
            float: left;
        }

        .iconsDiv {
            padding: 3px;
            width: 150px;
            text-align: right;
            float: right;
            z-index: 20;
        }

        .rubricDiv {
            box-shadow: 10px 10px 5px #888;
            display: table-cell;
            width: 80%;
            padding: 5px;
            text-align: left;
            float: left;
            overflow: auto;
        }

        .rubricInfo {
            margin-top: 5px;
            margin-bottom: 30px;
        }

        .tblContainer {
            display: table;
            padding: 0;
            width: 100%;
            border-bottom: solid 1px #000;
        }

        .labelTD {
            text-align: left;
            vertical-align: top;
            width: 50px;
            padding: 3px;
            white-space: nowrap;
            font-size: 9pt;
        }

        .textTD {
            text-align: left;
            vertical-align: top;
            padding: 3px;
            white-space: normal;
            width: 100px;
            font-size: 9pt;
        }

        .icons {
            display: inline;
            padding-right: 10px;
            padding-bottom: 6px;
            padding-left: 10px;
            padding-top: 0px;
            background-repeat: no-repeat;
            background-image: url('../../Images/copy.gif');
            cursor: pointer;
            top: 10px;
        }

        .scroller {
            height: 555px;
            overflow: hidden;
        }

        .itemActions {
            height: 16px;
            background-repeat: no-repeat;
            background-image: url('../../Images/AssessmentItemIcons.png');
            float: right;
            margin-right: 5px;
            cursor: pointer;
        }
    </style>
</asp:Content>
