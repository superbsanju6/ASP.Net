<%@ Page Title="Image Search" Language="C#" AutoEventWireup="true" CodeBehind="ImageSearch_ExpandedV2.aspx.cs" Inherits="Thinkgate.Controls.Images.ImageSearch_ExpandedV2" MasterPageFile="~/Search.Master"%>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx"  %>
<%@ Register TagPrefix="e3" TagName="GradeSubjectCourseStandardSet" Src="~/Controls/E3Criteria/GradeSubjectCourseStandardSet.ascx"  %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx"  %>
<%@ Register TagPrefix="e3" TagName="TextWithDropdown" Src="~/Controls/E3Criteria/TextWithDropdown.ascx" %>
<%@ Register TagPrefix="e3" TagName="SearchPager" Src="~/Controls/E3Criteria/SearchPager.ascx" %>
<%@ Register TagPrefix="e3" TagName="SortBar" Src="~/Controls/E3Criteria/SortBar.ascx" %>
<%@ Register Src="~/Controls/E3Criteria/ExpirationStatusAndDateRange.ascx" TagPrefix="e3" TagName="ExpirationStatusAndDateRange" %>
<%@ MasterType virtualpath="~/Search.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    <e3:SortBar ID="SortBar" runat="server" OnSort="SortHandler"/>      
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results"/>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <e3:GradeSubjectCourseStandardSet runat="server" ID="ctrlGradeSubjectCourseStandardSet" CriteriaName="GradeSubjectCourseStandardSet"/>
    <e3:CheckBoxList ID="cblItemBank" CriteriaName="ItemBank" runat="server" Text="Item Bank" DataTextField="Label" DataValueField="Label"/>
   <e3:ExpirationStatusAndDateRange CriteriaName="ExpirationStatusAndDateRange" runat="server" id="ExpirationStatusDateRange" ContentType="Images"/>
   <e3:TextWithDropdown ID="txtSearch" CriteriaName="TextSearch" Text="Text Search" runat="server" DataTextField="Name"/>    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <div id="data" style="overflow: auto;" onscroll="onDataScroll();" ontouchmove="onDataScroll();">
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

        var initialRecordsToDisplay = 5;
        var baseRecordHeight = 150;
        var recordsPerPage = 100;
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
            VerifyIBAccess: function (val) {
                if (CanUserEditItemBank(val)) {
                    return true;
                }

                return false;
            }
        });

        function CanUserEditItemBank(itemID) {
            var isReturn = false;
            $.each(ItemArray[currentPage - 1], function (i) {
                if ($(this).attr('ID') == itemID) {
                    if ($(this)[0].ItemBankList) {
                        $.each($(this)[0].ItemBankList, function (p) {
                            var targettype = $(this)[0].TargetType;
                            var label = $(this)[0].Label;
                            $.each(IBEditArray[0].ItemBanks, function (r) {
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
                for (var y = 0; y < (addendumList.length < initialRecordsToDisplay ? addendumList.length : initialRecordsToDisplay); y++) {
                    tempArray[y] = addendumList[y];
                    RenderListArray.push([y]);
                }
                $('#data').html($.render.Tmpl(tempArray));

                var fillerDiv = '';
                for (var z = initialRecordsToDisplay; z < (addendumList.length > initialRecordsToDisplay ? addendumList.length : initialRecordsToDisplay); z++) {
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
                for (var y = 0; y < (addendumList.length < c ? addendumList.length : c); y++) {
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

        function SendResultsToWindow(imagePath) {
            var DialogueToSendResultsTo;
            DialogueToSendResultsTo = parent.getCurrentCustomDialogByName(getURLParm('NewAndReturnType'));
            try {
                if (DialogueToSendResultsTo != null)
                {
                    DialogueToSendResultsTo.get_contentFrame().contentWindow.InsertImage(imagePath);
                }
                else
                {
                    if (typeof parent.ContentEditor != "undefined" && parent.ContentEditor.relatedTarget.URL.toLowerCase().indexOf('contenteditor_addendum.aspx') > -1)
                    {
                        parent.ContentEditor.InsertImage(imagePath);
                    }
                    else {
                        parent.InsertImage(imagePath);
                    }
                }

            }
            catch (e) {
                alert('Error inserting Image');
            }

            closeCurrentCustomDialog();
        }

        function makeAddendumOverflowAuto(div) {
            div.style.overflow = "auto";
            div.style.cursor = "default";
        }
        
        function OpenAdvancedEditor(enc) {
            //window.open('../Assessment/ContentEditor/ContentEditor_Addendum.aspx?xID=' + enc);
            parent.customDialog({ url: ('../../Controls/Assessment/ContentEditor/ContentEditor_Addendum.aspx?xID=' + enc + '&NewAndReturnType=&NewAndReturnID='), maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorADDENDUM', title: 'Content Editor - ADDENDUM', destroyOnClose: true });
        }

        function pageSpecificSearch(prefetch, pagenumber) {
            Service2.ItemImageSearchV2(prefetch, pagenumber, 100, CriteriaController.ToJSON(), SortField, function (results) {
                onSearchSuccess(prefetch, results, pagenumber);
            }, onSearchFail);
        }

        function copyAddendum(id) {
            var panel = $find('copyAddendumXmlHttpPanel');
            var panelValue = '{"AddendumID":' + id + '}';
            panel.set_value(panelValue);
        }

        function newAddendumMessage(sender, args) {
            var result = args.get_content();

            if (!isNaN(result) && parseInt(result) > 0) {
                parent.radalert('The selected addendum has been copied and added to your personal bank.');
            }
            else {
                parent.radalert('An error has occurred. The addendum was not copied.');
            }
        }
        
        function RenderFirstPage() {
            currentPage = 1;
            renderData(ItemArray[0]);
            resizeContent();
            preFetchNext(2);
        }
        
    </script>
    <script id="Template" type="text/x-jsrender">
        <div class="tblContainer">
            <div class="imageRow" id="{{:ID}}">
                {{* if (newAndReturnType){ }}
                <div class="selectDiv"><img src="../../Images/select.png" style="cursor:pointer;" onclick="SendResultsToWindow('{{:FilePath}}');" /></div>
                {{* } }}
                <div class="imageDiv"><img src="{{:FilePath}}" alt="Image" style="width:125px; height: 125px;" /></div>
                <div class="imageInfo">
                    <div class="infoCol">
                        <table border="0" cellpadding="2" width="100%">
                            <tr>
                                <td class="labelTD">Grade:</td>
                                <td class="textTD">{{:Grade}}</td>
                            </tr>
                            <tr>
                                <td class="labelTD">Subject:</td>
                                <td class="textTD">{{:Subject}}</td>
                            </tr>
                            <tr>
                                <td class="labelTD">Course:</td>
                                <td class="textTD">{{:Course}}</td>
                            </tr>
                            <tr>
                                <td class="labelTD">Name:</td>
                                <td class="textTD" wrap>{{:Name}}</td>
                            </tr>
                            <tr>
                                <td class="labelTD">Keyword:</td>
                                <td class="textTD">{{:Keywords}}</td>
                            </tr>
                        </table>
                    </div>
                    <div class="infoCol">
                        <table border="0" cellpadding="2" width="100%">
                            <tr>
                                <td class="labelTD">Item Bank:</td>
                                <td class="textTD">{{:ItemBanks}}</td>
                            </tr>
                            <tr>
                                <td class="labelTD">Author:</td>
                                <td class="textTD">{{:CreatedByName}}</td>
                            </tr>
                            <tr>
                                <td class="labelTD">Date Created:</td>
                                <td class="textTD">{{:DateCreated}}</td>
                            </tr>
                            <tr>
                                <td class="labelTD">Last Updated:</td>
                                <td class="textTD">{{:DateUpdated}}</td>
                            </tr>
                            <tr>
                                <td class="labelTD">Copyright:</td>
                                <td class="textTD">{{if Copyright == null || Copyright == ""}} No {{else}} {{:Copyright}}{{/if}}</td>
                            </tr>
                            <tr>
                                <td class="labelTD">Expiration Date:</td>
                                <td class="textTD">{{if CopyRightExpiryDate}} {{:~formatDate(CopyRightExpiryDate)}} {{else}} Not defined {{/if}}</td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="iconsDiv">
                    <div class="itemActions" style="width: 21px;" onclick="window.open('{{:FilePath}}');" title="Online Preview"></div>
                    {{if ~VerifyIBAccess(ID)}}
                        <div class="itemActions" style="background-position: -92px 0px; width: 19px;" onclick="window.open('../Assessment/ContentEditor/ContentEditor_Image.aspx?xID={{:ID_Encrypted}}');" title="Advanced Edit"></div>
                        <div class="itemActions" style="background-position: -62px 0px; width: <%=ExpandPerm_IconWidth %>" <%=ExpandPerm_onclick %> title="Expand"></div>
                    {{/if}}
                </div>
            </div>
        </div>
    </script>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
         .imageRow
        {
            display: table-row;
            background-color: #FFF;
            padding: 3px;
            width: 100%;
            height: 150px;
        }

        div.RadToolTip {
            position: fixed !important;
        }
        .selectDiv
        {
            padding: 3px;
            width: 30px;
            height: 150px;
            text-align: center;
            float: left;
        }
        
        .iconsDiv
        {
            padding: 3px;
            width: 75px;
            height: 150px;
            text-align: center;
            float: left;
        }
        
        .imageDiv
        {
            display: table-cell;
            width: 150px;
            height: 150px;
            padding: 5px;
            text-align: left;
            float: left;
        }
        
        .imageInfoDiv
        {
            display: table-cell;
            width: 400px;
            height: 150px;
            padding: 3px;
            float: left;
        }
        
        .infoCol
        {
            width: 200px;
            float: left;
            margin-top: 10px;
            margin-left: 20px;
        }
        
        .tblContainer
        {
            display: table;
            padding: 0;
            width: 100%;
            border-bottom: solid 1px #000;
        }
        
        .labelTD
        {
            font-weight: bold;
            text-align: left;
            vertical-align: top;
            width: 1%;
            padding: 3px;
            white-space: nowrap;
        }
        
        .textTD
        {
            text-align: left;
            padding: 3px;
            white-space: normal;
        }
        
        .icons
        {
            display: inline;
            padding: 2px;
            cursor: pointer;
        }
        
        .scroller
        {
            height: 562px;
            overflow: auto;
        }
        
        .divIcon
        {
            height: 16px;
            float: right;
            margin-top: 2px;
            cursor: pointer;
            background-image: url('../../Images/AssessmentItemIcons.png');
            background-repeat: no-repeat;
        }

        .itemActions
        {
            height: 16px;
            background-repeat: no-repeat;
            background-image: url('../../Images/AssessmentItemIcons.png');
            float: right;
            margin-right: 5px;
            cursor: pointer;
        }
    </style>
</asp:Content>