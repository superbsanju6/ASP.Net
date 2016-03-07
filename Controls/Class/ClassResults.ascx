<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassResults.ascx.cs"
    Inherits="Thinkgate.Controls.Class.ClassResults" %>
<telerik:RadAjaxPanel runat="server" ID="classResultsPanel" LoadingPanelID="classResultsLoadingPanel">
    <telerik:RadMultiPage runat="server" ID="RadMultiPageClassResults" SelectedIndex="0"
        Width="100%" Height="210px" CssClass="multiPage">
        <telerik:RadPageView runat="server" ID="viewState">
            <div style="width: 100%; background-color: Gray; border-bottom: 1px solid black;">
                <div class="tblContainer">
                    <div class="tblRow" style="width: 200px; padding: 3px;">
                        <div class="tblLeft" style="width: 80px;">
                            <div class="roundedCorner" style="float: left; background-color: Navy; padding-top: 7px;
                                text-align: center; width: 45px; height: 20px;">
                                <a style="color: White;">Term</a>
                            </div>
                            <div class="roundedCorner" style="float: right; background-color: White; padding-top: 7px;
                                text-align: center; width: 20px; height: 20px;">
                                <a style="color: Navy;">1</a>
                            </div>
                        </div>
                        <div class="tblMiddle" style="width: 187px; padding-left: 10px;">
                            <div class="roundedCorner" style="float: left; background-color: Navy; padding-top: 7px;
                                text-align: center; width: 100px; height: 20px;">
                                <a style="color: White;">Assessment Type</a>
                            </div>
                            <div class="roundedCorner" style="float: right; background-color: White; padding-top: 7px;
                                text-align: center; width: 75px; height: 20px;">
                                <a style="color: Navy;">Benchmark</a>
                            </div>
                        </div>
                        <div class="tblRight" style="padding-left: 10px;">
                            <div style="width: 20px; height: 20px; background-color: Navy; cursor: pointer;"
                                onclick="$('#divResultsCriteria').slideToggle('slow');">
                                <a style="color: white; font-size: large;"><b>^</b></a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divResultsCriteria" style="display: none;">
                <div class="tblContainer" style="background-color: Gray; position: relative; width: 310px;
                    height: 45px;">
                    <div class="tblRow">
                        <div class="tblLeft">
                            <div style="height: 40px; padding: 0px; float: left; width: 70px; margin-top: 10px;
                                margin-left: 25px;">
                                <a style="font-size: small; color: Navy;">1 2 3 4 5 6</a>
                            </div>
                        </div>
                        <div class="tblMiddle">
                            <div style="height: 40px; padding: 0px; float: right; margin-top: 6px;">
                                <div class="tblContainer" style="width: 170px; height: 35px; text-align: center;">
                                    <div class="tblRow" style="width: 50px; padding: 3px;">
                                        <div class="tblLeft" style="border: 1px solid black;">
                                            <a style="font-size: x-small; color: Navy;">Benchmark</a></div>
                                        <div class="tblMiddle" style="width: 1px;">
                                        </div>
                                        <div class="tblRight">
                                            <a style="font-size: x-small; color: Navy;">Pre-Test</a></div>
                                    </div>
                                    <div class="tblRow" style="width: 40px; padding: 3px;">
                                        <div class="tblLeft">
                                            <a style="font-size: x-small; color: Navy;">Exam</a></div>
                                        <div class="tblMiddle">
                                        </div>
                                        <div class="tblRight">
                                            <a style="font-size: x-small; color: Navy;">Post-Test</a></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tblRight">
                            <asp:ImageButton ID="btnGo" runat="server" Width="15" Height="15" ImageUrl="~/Images/GO-icon.gif" />
                        </div>
                    </div>
                </div>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="viewDistrict">
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="viewClassroom">
        </telerik:RadPageView>
    </telerik:RadMultiPage><!--
            no spaces between the tabstrip and multipage, in order to remove unnecessary whitespace
 --><telerik:RadTabStrip runat="server" ID="RadTabStrip2" Orientation="HorizontalBottom"
     SelectedIndex="0" MultiPageID="RadMultiPageClassResults">
     <Tabs>
         <telerik:RadTab Text="State">
         </telerik:RadTab>
         <telerik:RadTab Text="District">
         </telerik:RadTab>
         <telerik:RadTab Text="Classroom">
         </telerik:RadTab>
     </Tabs>
 </telerik:RadTabStrip>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="classResultsLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>
