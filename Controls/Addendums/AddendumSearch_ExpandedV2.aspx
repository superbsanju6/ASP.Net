<%@ Page Title="Addendum Search" Language="C#" AutoEventWireup="true" CodeBehind="AddendumSearch_ExpandedV2.aspx.cs" Inherits="Thinkgate.Controls.Addendums.AddendumSearch_ExpandedV2" MasterPageFile="~/Search.Master" %>

<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx" %>
<%@ Register TagPrefix="e3" TagName="GradeSubjectCourseStandardSet" Src="~/Controls/E3Criteria/GradeSubjectCourseStandardSet.ascx" %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx" %>
<%@ Register TagPrefix="e3" TagName="AddendumType" Src="~/Controls/E3Criteria/AddendumType.ascx" %>
<%@ Register TagPrefix="e3" TagName="TextWithDropdown" Src="~/Controls/E3Criteria/TextWithDropdown.ascx" %>
<%@ Register TagPrefix="e3" TagName="SearchPager" Src="~/Controls/E3Criteria/SearchPager.ascx" %>
<%@ Register TagPrefix="e3" TagName="SortBar" Src="~/Controls/E3Criteria/SortBar.ascx" %>
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
    <e3:AddendumType ID="ctrlAddendumType" CriteriaName="AddendumType" Text="Type" runat="server" ChildDataTextField="Genre" ChildDataValueField="Genre" />
    <e3:CheckBoxList ID="cblItemBank" CriteriaName="ItemBank" runat="server" Text="Item Bank" DataTextField="Label" DataValueField="Label" />
    <e3:ExpirationStatusAndDateRange CriteriaName="ExpirationStatusAndDateRange" runat="server" ID="ExpirationStatusDateRange" ContentType="Addendums" />
    <e3:TextWithDropdown ID="txtSearch" CriteriaName="TextSearch" Text="Text Search" runat="server" DataTextField="Name" />
    <telerik:RadXmlHttpPanel runat="server" ID="copyAddendumXmlHttpPanel" EnableClientScriptEvaluation="true"
        OnServiceRequest="copyAddendumXmlHttpPanel_ServiceRequest" OnClientResponseEnding="newAddendumMessage">
    </telerik:RadXmlHttpPanel>
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

        var initialRecordsToDisplay = 5;
        var baseRecordHeight = 300;
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
                if (CanUserUseItemBank(val, IBCopyArray)) {
                    return true;
                }
                return false;
            }
        });

        function CanUserUseItemBank(itemID, IBarray) {
            var isReturn = false;
            $.each(ItemArray[currentPage - 1], function (i) {
                if ($(this).attr('ID') == itemID) {
                    if ($(this)[0].Addendum_ItemBankList) {
                        $.each($(this)[0].Addendum_ItemBankList, function (p) {
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

                        MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
                    }
                }
            }
        }

        function onSearchFail(error) {
            alert('error');
            callInProgress = false;
            HideSpinner();
        }

        function SendResultsToWindow(AddendumID, AddendumName, AddendumType, AddendumGenre) {
            var DialogueToSendResultsTo;
            DialogueToSendResultsTo = parent.getCurrentCustomDialogByName(getURLParm('NewAndReturnType'));
            try {
                DialogueToSendResultsTo.get_contentFrame().contentWindow.InsertAddendum(AddendumID, AddendumName, AddendumType, AddendumGenre);
            }
            catch (e) {
                try {
                    parent.InsertAddendum(AddendumID, AddendumName, AddendumType, AddendumGenre);
                }
                catch (e) {
                    //parent.ItemSearch.InsertAddendum(AddendumID, AddendumName, AddendumType, AddendumGenre);
                }
            }

            closeCurrentCustomDialog();
        }

        function makeAddendumOverflowAuto(div) {
            div.style.overflow = "auto";
            div.style.cursor = "default";
        }

        function OpenAdvancedEditor(enc, clientFolder) {
            //window.open('../Assessment/ContentEditor/ContentEditor_Addendum.aspx?xID=' + enc);            
            parent.customDialog({ url: (clientFolder + '/Controls/Assessment/ContentEditor/ContentEditor_Addendum.aspx?xID=' + enc + '&NewAndReturnType=&NewAndReturnID='), maximize: true, maxheight: 700, maxwidth: 1100, name: 'ContentEditorADDENDUM', title: 'Content Editor - ADDENDUM', destroyOnClose: true });
        }

        function pageSpecificSearch(prefetch, pagenumber) {
            Service2.AddendumSearchV2(prefetch, pagenumber, 100, CriteriaController.ToJSON(), SortField, function (results) {
                onSearchSuccess(prefetch, results, pagenumber);
            }, onSearchFail);
        }

        function copyAddendum(id) {
            var panel = $find('<%= copyAddendumXmlHttpPanel.ClientID %>');
            var panelValue = id;
            panel.set_value(panelValue);
        }

        function newAddendumMessage(sender, args) {
            parent.radalert('The selected addendum has been copied and added to your personal bank.');
        }

        function RenderFirstPage() {
            currentPage = 1;
            renderData(ItemArray[0]);
            resizeContent();
            preFetchNext(2);
            //MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        }

        function AddendumSearchOnClick(id) {
            var addendumNameID = '#addendumName_' + id;
            var addendumTypeID = '#addendumType_' + id;
            var addendumGenreID = '#addendumGenre_' + id;

            SendResultsToWindow(id, $(addendumNameID).html(), $(addendumTypeID).html(), $(addendumGenreID).html());
        }

        function showAddendumPreview(id) {
            var addendumContent = $('#addendum_' + id).html();
            window.customDialog(
                           {
                               title: 'Addendum Preview',
                               maxwidth: 600,
                               maxheight: 400,
                               maximize: true,
                               content: "<div style='overflow: auto; height: 400px; max-width: 800px;'>" + addendumContent + "</div>"
                           }
                        );


        }

    </script>
    <script id="Template" type="text/x-jsrender">
        <div id="{{:ID}}" style="border-bottom: 1px solid black; padding-left: 10px; margin-top: 10px; height: 400px;">
            {{* if (newAndReturnType){ }}
            <div class="selectDiv" onclick="AddendumSearchOnClick({{:ID}});"></div>
            {{* } }}
            <div class="addendumText" id="addendum_{{:ID}}" onclick="makeAddendumOverflowAuto(this)">
                <span style="display: inline-block;">{{:Addendum_Text}}</span>
            </div>            
            <div class="iconsDiv">
                {{if ((Addendum_Copyright != "Yes") || (Addendum_Copyright == "Yes" && ~VerifyExpiryDate(Addendum_CopyRightExpiryDate)))}}
                    <div style="float: left">
                        {{if ~VerifyIBCopyAccess(ID) }}
                                <div class="icons" onclick="copyAddendum({{:ID}});" title="Copy"></div>
                        {{/if}}
                     </div>
                        {{if ~VerifyIBEditAccess(ID) }}
                                <div class="itemActions" style="background-position: -62px 0px; width: <%=ExpandPerm_IconWidth %>" <%=ExpandPerm_onclick %> title="Expand"></div>
                                <div class="itemActions" style="background-position: -92px 0px; width: 19px;" onclick="OpenAdvancedEditor('{{:ID_Encrypted}}', '<%=clientFolder %>');" title="Advanced Edit"></div>
                        {{/if}}                   
                {{/if}}
                <div class="itemActions" style="width: 21px; float: right !important;" title="Online Preview" onclick ="showAddendumPreview({{:ID}});"></div>
            </div>
            <br />
            <div style="margin-top: 10px; margin-bottom: 10px; float: left;">
                <table class="questionInformation" style="width: 100%;">
                    <tr>
                        <td class="labelTD">Name:</td>
                        <td class="textTD" id="addendumName_{{:ID}}">{{:Addendum_Name}}</td>
                        <td class="labelTD">Author:</td>
                        <td class="textTD">{{:Addendum_CreatedByName}}</td>
                        <td class="labelTD">Date Created:</td>
                        <td class="textTD">{{:Addendum_DateCreated}}</td>
                        <td class="labelTD">Last Updated:</td>
                        <td class="textTD">{{:Addendum_DateUpdated}}</td>
                    </tr>
                    <tr>
                        <td class="labelTD">Grade:</td>
                        <td class="textTD">{{:Addendum_Grade}}</td>
                        <td class="labelTD">Subject:</td>
                        <td class="textTD">{{:Addendum_Subject}}</td>
                        <td class="labelTD">Standard:</td>
                        <td class="textTD">{{:Addendum_Course}}</td>
                        <td class="labelTD">Items:</td>
                        <td class="textTD">{{:Addendum_ItemCount}}</td>
                    </tr>
                    <tr>
                        <td class="labelTD">Type:</td>
                        <td class="textTD" id="addendumType_{{:ID}}">{{:Addendum_Type}}</td>
                        <td class="labelTD">Genre:</td>
                        <td class="textTD" id="addendumGenre_{{:ID}}">{{:Addendum_Genre}}</td>
                        <%= (_perm_Field_Lexile) ? "<td class='labelTD'>Lexile:</td>" :""%>
                        <%= (_perm_Field_Lexile) ? "<td class='textTD'>{{:Addendum_Lexile}}</td>" :""%>
                        <td class="labelTD">Item Bank:</td>
                        <td class="textTD">{{:Addendum_ItemBanks}}</td>
                    </tr>
                        <tr>
                        <td class="labelTD">Copyright:</td>
                        <td class="textTD" id="Td1">{{if Addendum_Copyright == null || Copyright == ""}} No {{else}} {{:Addendum_Copyright}}{{/if}}</td>
                        <td class="labelTD">Expiration Date:</td>
                        <td class="textTD" id="Td2">{{if Addendum_CopyRightExpiryDate}} {{:~formatDate(Addendum_CopyRightExpiryDate)}}{{else}} Not defined {{/if}}</td>
                       
                    </tr>
                </table>
            </div>
        </div>
    </script>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">

  <style type="text/css">
        .icons {
            padding-right: 3px;
            height: 20px;
            width: 21px;
            top: 1px !important;
            background-repeat: no-repeat;
            background-image: url('../../Images/copy.gif');
            cursor: pointer;
         
        }

        #rightColumn {
            background-color: white;
        }

        div.RadToolTip {
            position: fixed !important;
        }

        .questionInformation td span {
            font-weight: normal;
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

        .addendumText {
            box-shadow: 10px 10px 5px #888;
            cursor: pointer;
            height: 200px;
            overflow: hidden;
            border: 2px solid #000;
            width: 80%;
            margin-bottom: 10px;
        }

        .labelTD {
            font-weight: bold;
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

        .selectDiv {
            padding: 3px;
            width: 30px;
            height: 9px;
            text-align: center;
            float: left;
            background-repeat: no-repeat;
            background-image: url('../../Images/select.png');
            z-index: 21;
            cursor: pointer;
        }

        .itemActions {
            height: 16px;
            top: 1px !important;
            background-repeat: no-repeat;
            background-image: url('../../Images/AssessmentItemIcons.png');
            float: right;
            margin-right: 5px;
            cursor: pointer;
        }

        .iconsDiv {
            padding: 3px;         
            text-align: right;
            float: right;          
            z-index: 20;
        }
    </style>


</asp:Content>
