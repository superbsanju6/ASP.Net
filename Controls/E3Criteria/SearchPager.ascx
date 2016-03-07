<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchPager.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.SearchPager" %>

<div id ="SearchPager" style="height: 26px; width: 100%;">
    <div style="width: 50%; height: 100%; float: left; background-color: rgb(240, 240, 240);">
        <div id="PagerDiv" runat="server" style="height: 23px;">
            <div style="padding-top: 2px;">
                <div style="width: 200px;" id="templateSearchPage" class="rgWrap" lastpage="15">
                    <div class="rgWrap rgArrPart1">
                        <input style="border: 0px currentColor;" class="rgPageFirst" title="First Page" onclick="goToPage(1);" value=" " type="button">
                        <input style="border: 0px currentColor;" class="rgPagePrev" title="Previous Page" onclick="goToPrevPage();" value=" " type="button">
                    </div>
                    <div id="pagingScrollWrapper" class="rgWrap rgNumPart">
                        <div id="numberWrapper">
                            <asp:Repeater ID="PageList" runat="server">
                                <ItemTemplate>
                                    <a class="<%# GetItemClass(Container.DataItem.ToString())%>" id="pageTag_<%# Container.DataItem.ToString() %>" onclick="goToPage(<%# Container.DataItem.ToString() %>);"><span><%# Container.DataItem.ToString() %></span></a>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                    <div class="rgWrap rgArrPart2">
                        <input style="border: 0px currentColor;" class="rgPageNext" title="Next Page" onclick="goToNextPage(1);" value=" " type="button">
                        <input style="border: 0px currentColor;" id="rgPageLast" runat="server" class="rgPageLast" title="Last Page" onclick="goToLastPage(this);" value=" " type="button">
                    </div>
                </div>
             </div>       
        </div>
    </div>
    <div style="width: 50%; height: 26px; text-align: right; float: right; background-color: rgb(240, 240, 240);">
        <span style="font-weight: bold;" id="resultsFoundText" runat="server"></span>
    </div>
</div>