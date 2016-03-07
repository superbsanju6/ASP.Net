<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DoubleCarousel.ascx.cs"
    Inherits="Thinkgate.Controls.DoubleCarousel" %>
<table width="100%">
    <tr>
        <td colspan="3" style="text-align: center">
            <asp:Panel ID="buttonsContainer1" runat="server" ClientIDMode="Static" CssClass="buttonsContainer">
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            <img src="../Images/back_icon.gif" class="navButton" id="prevButton1" alt="Back" />
        </td>
        <td id="tdTilesRotator1" runat="server" clientidmode="Static" 
            ontouchstart="touchStart(event,'tdTilesRotator1','tilesRotator1');" ontouchend="touchEnd(event);" ontouchmove="touchMove(event);" ontouchcancel="touchCancel(event);">
            <telerik:RadRotator runat="server" ID="tilesRotator1" Width="1015px" Height="280px"
                ItemWidth="1015px" RotatorType="Buttons" BorderStyle="None" ScrollDuration="500"
                WrapFrames="false">
                <ControlButtons LeftButtonID="prevButton1" RightButtonID="nextButton1" />
            </telerik:RadRotator>
        </td>
        <td>
            <img src="../Images/forward_icon.gif" class="navButton" id="nextButton1" alt="Next" />
        </td>
    </tr>
    <tr id="separatorRow" runat="server">
        <td colspan="3">
            <hr />
        </td>
    </tr>
    <tr>
        <td colspan="3" style="text-align: center">
            <asp:Panel ID="buttonsContainer2" runat="server" ClientIDMode="Static" CssClass="buttonsContainer">
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            <img src="../Images/back_icon.gif" class="navButton" id="prevButton2" alt="Back" runat="server" />
        </td>
        <td id="tdTilesRotator2" runat="server" clientidmode="Static" 
            ontouchstart="touchStart(event,'tdTilesRotator2','tilesRotator2');" ontouchend="touchEnd(event);" ontouchmove="touchMove(event);" ontouchcancel="touchCancel(event);">
            <telerik:RadRotator runat="server" ID="tilesRotator2" Width="1015px" Height="280px"
                ItemWidth="1015px" RotatorType="Buttons" BorderStyle="None" ScrollDuration="500"
                WrapFrames="false">
                <ControlButtons LeftButtonID="prevButton2" RightButtonID="nextButton2" />
            </telerik:RadRotator>
        </td>
        <td>
            <img src="../Images/forward_icon.gif" class="navButton" id="nextButton2" alt="Next" runat="server"/>
        </td>
    </tr>
</table>
