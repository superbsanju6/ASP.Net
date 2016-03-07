<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="AssignStudentsToAssessment.ascx.cs" Inherits="Thinkgate.Controls.AssignmentShare.AssignStudentsToAssessment" %>
<div id="ucAssignStudentsToAssessment">
    <telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel"
        Style="height: 100%;">
            <div class="LeftDiv">Criteria</div>
            <div style="overflow: hidden;" class="right" id="ToolTipAnchor" runat="server"><img id ="imgageCtrl2" runat ="server" style="width: 16px; height: 16px;" src="~/Images/commands/expand_bubble.png"></div>
            <div id="CriteriaDiv" style="width: 200px; float: left; height: 100%; background-color: #848484">
            <script type="text/javascript">
                function getAllSchoolsFromSchoolTypes(result) {
                    // School
                    updateCriteriaControl('School', result.Schools, 'DropDownList', 'School');
                }
            </script>
            <telerik:RadToolTip ID="RadToolTip" runat="server" ShowEvent="OnClick" Width="500" HideEvent="ManualClose"  Skin="Black" Height="55" EnableShadow="True" AutoCloseDelay="20000" Position="MiddleRight" RelativeTo="Element" TargetControlID="imgageCtrl2">
                <div> THis is the rad tool tip!</div>
                <asp:PlaceHolder runat="server" ID="criteraDisplayPlaceHolder"></asp:PlaceHolder>
            </telerik:RadToolTip>
        </div>
        <telerik:RadWindowManager ID="RadWindowManager" runat="server" EnableShadow="true" Skin="Telerik">
        </telerik:RadWindowManager>
    </telerik:RadAjaxPanel>

    <asp:TextBox ID="hiddenTextBox" runat="server" Style="visibility: hidden; display: none;" />

</div>
