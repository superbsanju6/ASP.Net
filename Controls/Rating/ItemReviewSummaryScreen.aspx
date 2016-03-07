<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ItemReviewSummaryScreen.aspx.cs" Inherits="Thinkgate.Controls.Rating.ItemReviewSummaryScreen" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link id="lnkWindowStyle" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet"
        type="text/css" runat="server" />
    <title></title>
    <base target="_self" />

    <style>
        

         .disclaimer
         {
             font-weight: bold;
             font-size: small;
         }

        #dvRatingSummary table {
            width: 100%;
            border: 1px solid black;
            padding:0;
            border-spacing: 0;
                               }

        #dvRatingSummary table tr:first-child {
            font-weight: bolder;
            background-color: #94baf2;
            height: 30px;
                                              }

        #dvRatingSummary table tr:first-child td {
            text-align: center;
            border: 1px solid #c5cedd;
                                                 }

        #dvRatingSummary table tr:not(:first-child) {
            background-color: whitesmoke;
            text-align: center;
                                                    }



        #dvRatingCount table tr td {
            border-bottom: 1px dotted grey;
                                   }


        body {
            background-color: #DEE2E7;
             }

        #dvRatingCount {
            width: 40%;
            border: 4px solid black;
            background-color: whitesmoke;
            margin-left: 5px;
            margin-top: 5px;
                       }

        #lblTotalRating, #lblAverageRating, #lblNaRating {
            font-weight: bold;
            font-size: 11pt;
                                                         }

        #lblTotalRating, #lblNaRating {
            margin-left: 6px;
                                      }

        a {
            font-weight: bold;
            font-size: 11pt;
          }

        .left-text {
            text-align: left;
                   }

        .dot-line {
            border-bottom: 1px dotted grey;
                  }

        .more-lnk {
            font-size: small;
                  }

        .commentFull {
                     }

        .display-none {
            display: none;
                      }

        .commentLimit
        {
            
        }

        .summaryRatingTableColumn, .dateTableColumn
        {
            width:10%;
        }

        .ratingCreatorNameTableColumn
        {
            width: 20%;
        }

        #commentTableColumn
        {
            width: 60%;
        }

        #headerTable
        {
            width: 100%;
        }

        .btn-style
        {
            height: 50px;
            vertical-align: central;
        }

        .word-wrap 
        {
           width: 30em;
           word-wrap: break-word;
        }
    </style>

    <script type="text/javascript">
       
        //Show the ItemReviewMainScreen 
        //User should be able to edit the item review.
        function showReviewMainScreen(questionId) {
            var height = window.$telerik.$(window).height();
            var width = window.$telerik.$(window).width();

            var heightCal = (height * 0.99);
            var widthCal = (width * 0.99);

            var actionId = document.getElementById("hidActionId").value;

            customDialog({
                url: ('ItemReviewMainScreen.aspx?itemId=' + questionId + '&source=ReviewSummary&actionId=' + actionId),
                width: widthCal,
                height: heightCal,
                movable: false,
                center: true,
                title: 'Rating'
            });
        }

        //Set the parent dialog refresh flag 
        //So the respective review value will be refreshed.
        function setParentRefreshFlag() {
            var hidRefresh = $("#hidRefresh", parent.document.body);
            hidRefresh.val("true");
        }

        //Shows the Special Population when user click on Special Population button.
        function showSpecialPopulationScreen(questionId, userId)
        {
            var height = window.$telerik.$(window).height();
            var width = window.$telerik.$(window).width();

            var heightCal = (height * 0.40);
            var widthCal = (width * 0.95);
            

            customDialog({
                url: ('SpecialPopulationScreen.aspx?questionId=' + questionId + '&userId=' + userId + '&isEnabled=false'),
                width: widthCal,
                height: heightCal,
                movable: false,
                center: true,
                title: 'Special Population'
            });
        }

        //ReadMore and ReadLess Functionality
        function showFullComment(obj, commentId) {

            var mode = $(obj).attr("CommentMode");

            switch (mode) {
                case "ReadMore":
                    $('.commentFull[CommentID="' + commentId + '"]').each(function () {
                        $(this).removeClass('display-none');
                        $(obj).attr("CommentMode", "ReadLess");
                        obj.innerHTML = 'Read Less';
                    });

                    $('.commentLimit[CommentID="' + commentId + '"]').each(function () {
                        $(this).addClass('display-none');
                    });

                    break;
                case "ReadLess":
                    $('.commentFull[CommentID="' + commentId + '"]').each(function () {
                        $(this).addClass('display-none');

                    });

                    $('.commentLimit[CommentID="' + commentId + '"]').each(function () {
                        $(this).removeClass('display-none');
                        $(obj).attr("CommentMode", "ReadMore");
                        obj.innerHTML = 'Read More';
                    });

                    break;

                default:
                    break;
            }

            return false;
        }

        //Set the rating control tool tip based on actual average value.
        function setRatingControl(control) {

             if (control != undefined) {
                var items = control._items;
                var average = document.getElementById("hidAverageRating");

                 if (average != null) 
                 {
                     for (var i = 0; i < items.length; i++) {
                         if (items[i].firstChild != undefined) {
                             items[i].firstChild.title = average.value;
                         }
                     }
                 }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="radScriptManager" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
                <asp:ScriptReference Path="~/scripts/master.js" />
            </Scripts>
        </telerik:RadScriptManager>

        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false" Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />

        <div id="dvHeader">
        </div>
        <telerik:RadAjaxPanel ID="ajaxReviewSummaryPanel" runat="server" LoadingPanelID="reviewSummaryProgress" Width="100%">
            <asp:UpdatePanel ID="updReviewCount" runat="server">
                <ContentTemplate>
                    <asp:HiddenField ClientIDMode="Static" ID="hidActionId" runat="server" />
                    <br />
                    <div id="dvRatingCount">
                        <table id="headerTable">
                            <tr>
                                <td>
                                    <telerik:RadRating ID="averageRating" runat="server" Skin="Default" ItemCount="5" SelectionMode="Continuous"
                                        Orientation="Horizontal" Enabled="False" ReadOnly="True" OnClientLoad="setRatingControl">
                                    </telerik:RadRating>
                                </td>
                                <td>
                                    <asp:Label ID="lblAverageRating" runat="server"></asp:Label>
                                    <asp:HiddenField runat="server" ID="hidAverageRating" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblTotalRating" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblShowAll">Show All</asp:Label>
                                    <asp:LinkButton runat="server" ID="btnShowAll" OnClick="showReviews_OnClick">Show All</asp:LinkButton>
                                </td>
                            </tr>
                            <asp:Repeater ID="rptRating" runat="server" OnItemDataBound="rptRating_OnItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <telerik:RadRating ID="rating" runat="server" Skin="Default" ItemCount="5" SelectionMode="Continuous" Precision="Item" Orientation="Horizontal" Enabled="False" ReadOnly="True"></telerik:RadRating>
                                        </td>
                                        <td>
                                            <asp:LinkButton runat="server" ID="btnReviewCount" OnClick="showReviews_OnClick"></asp:LinkButton>
                                            <asp:Label runat="server" ID="lblReviewCount">0</asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr>
                                <td>
                                    <asp:Label ID="lblNaRating" runat="server">No Rating Given</asp:Label>
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblNAReviewCount">0</asp:Label>
                                    <asp:LinkButton runat="server" ID="btnNAReviewCount" OnClick="showReviews_OnClick">0</asp:LinkButton>
                                </td>
                            </tr>
                        </table>

                    </div>

                    <br />

                    <div id="dvRatingSummary">
                        <table>
                            <tr>
                                <td>Review</td>
                                <td>Rank</td>
                                <td>Reviewer</td>
                                <td>Date</td>
                            </tr>
                            <asp:Repeater ID="rptReviewSummary" runat="server" OnItemDataBound="rptReviewSummary_OnItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td class="left-text" id="commentTableColumn">
                                            <p class="commentLimit word-wrap" commentid='<%# Eval("ReviewComment.CommentID") %>'>
                                                <asp:Literal ID="ltrReviewLimited" runat="server"></asp:Literal>
                                            </p>
                                            <p class="commentFull display-none word-wrap" commentid='<%# Eval("ReviewComment.CommentID") %>'>
                                                <asp:Literal ID="ltrReviewFull" runat="server"></asp:Literal>
                                            </p>
                                            <asp:LinkButton ID="btnComment" runat="server" CssClass="more-lnk" Visible="false" CommentMode="ReadMore"
                                                OnClick='<%#Eval("ReviewComment.CommentID", "return showFullComment(this, {0})") %>'>Read More</asp:LinkButton>
                                        </td>
                                        <td class="summaryRatingTableColumn">
                                            <telerik:RadRating ID="rating" runat="server" Skin="Default" ItemCount="5" SelectionMode="Continuous" Precision="Item" Orientation="Horizontal" Enabled="False" ReadOnly="True"></telerik:RadRating>
                                        </td>
                                        <td class="ratingCreatorNameTableColumn">
                                            <asp:Label ID="lblReviewer" runat="server"></asp:Label>
                                        </td>
                                        <td class="dateTableColumn">
                                            <asp:Label ID="lblDate" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="left-text dot-line btn-style">
                                            <telerik:RadButton ID="btnEdit" runat="server" Text="Edit" AutoPostBack="false" OnClick='<%#Eval("QuestionId", "showReviewMainScreen({0})") %>'></telerik:RadButton>
                                            &nbsp;
                                            <telerik:RadButton ID="btnDelete" runat="server" Text="Delete" ReviewId='<%# Eval("Review.Id") %>' OnClick="btnDelete_OnClick"></telerik:RadButton>
                                            &nbsp;
                                            <telerik:RadButton ID="btnSP" runat="server" Text="Special Populations" AutoPostBack="false" OnClick='<%# "return showSpecialPopulationScreen(\""+ Eval("QuestionId") +"\", \""+ Eval("CreatedById") +"\");" %>' />
                                        </td>
                                        <td colspan="3" class="dot-line"></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </div>

                    <div class="disclaimer">
                        Any views or opinions expressed in this review are solely those of the author and do not necessarily represent those of the School, District, or State.  
               
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="reviewSummaryProgress" runat="server" Width="100%" />
    </form>
</body>
</html>
