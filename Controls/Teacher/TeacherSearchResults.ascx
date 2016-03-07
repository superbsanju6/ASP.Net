<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeacherSearchResults.ascx.cs"
    Inherits="Thinkgate.Controls.Teacher.TeacherSearchResults" %>
<script src="../Scripts/rotatorPaging.js" type="text/javascript"></script>
<script type="text/javascript">
    function openTeacher(ID) {
        window.location.href = "../Record/Teacher.aspx?childPage=yes&xid=" + ID;
    }
</script>
<table width="100%" class="studenttiles" id="tblstudenttiles" ontouchstart="touchStart(event,'tblstudenttiles');"
    ontouchend="touchEnd(event);" ontouchmove="touchMove(event);" ontouchcancel="touchCancel(event);">
    <tr>
        <td width="100">
            <img src="../Images/left2.png" class="tilesPrevButton" id="prevButton" width="25" />
        </td>
        <td width="100%">
            <telerik:RadRotator runat="server" ID="tilesRotator" Width="100%" Height="700px"
                ItemWidth="1011px" RotatorType="Buttons" BorderStyle="None" ScrollDuration="500"                
                WrapFrames="false">
                <ControlButtons LeftButtonID="prevButton" RightButtonID="nextButton" />
            </telerik:RadRotator>
            <center>
                <asp:Panel ID="ButtonsContainer" runat="server" ClientIDMode="Static">
                </asp:Panel>
            </center>
        </td>
        <td width="100">
            <img src="../Images/right2.png" class="tilesNextButton" id="nextButton" width="25" />
        </td>
    </tr>
</table>
