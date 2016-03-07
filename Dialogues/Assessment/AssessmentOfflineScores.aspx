<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentOfflineScores.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.AssessmentOfflineScores" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="mainHead" runat="server">
    <title></title>
    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet" type="text/css" runat="server" />
	<script type='text/javascript' src='../../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../../Scripts/jquery-migrate-1.1.0.min.js'></script>

    <script type="text/javascript">
        function saveConfirmation(sender, args) {          
            customDialog({ title: "Confirmation", maximize: true, maxwidth: 300, maxheight: 75, autoSize: false, content: "Are you sure you want to save these changes?<br/><br/>", dialog_style: 'confirm' }, [{ title: "No" }, { title: "Yes", callback: inputScoresCallback }]);
        }

        function inputScoresCallback() {          
            __doPostBack('radSave');            
        }
    </script>
    <style type="text/css">
        /*Common Style Starts Here*/
        .frmOfflineCss {
            width: 400px;
            height: 650px;
            font: normal 14px/24px "Segoe UI",Arial,sans-serif;
            
        }

        /*Common Style Ends Here*/

        /*Header Style Starts Here*/

        .dvHeader {
            width: 350px;
        }

        /*Header Style Ends Here*/     
        
         /*Body Style Start Here*/
        .txtScoreCss {
            border:none
        }
         /*Body Style Ends Here*/    
         

        /*Footer Style Starts Here*/
        .saveCss {
            margin-left:260px;
        }
        /*Footer Style Ends Here*/
    </style>
</head>
<body>
     <form id="frmOfflineTest" runat="server" class="frmOfflineCss">
          <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />  
            </Scripts>
        </telerik:RadScriptManager>
         <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
		    Skin="Thinkgate_Window" EnableEmbeddedSkins="False" Modal="True" InitialBehaviors="Close"
		    Animation="None">
	    </telerik:RadWindowManager>
        <%-- Header Student & Score --%>
        <div id="dvHeader" runat="server" class="dvHeader">
            <asp:Table ID="tbHeader" runat="server" Width="400px">
                <asp:TableHeaderRow BackColor="#8FB0DB">
                    <asp:TableHeaderCell Width="300px" BorderWidth="1px" BorderColor="Black">
                        <asp:Label ID="lblStudent" runat="server" Text="Student"></asp:Label>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="100px" BorderWidth="1px" BorderColor="Black">
                        <asp:Label ID="lblScore" runat="server" Text="Score"></asp:Label>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
        </div>

        <%-- Student Name and Scores --%>
        <div id="Body" runat="server">
            <asp:Repeater ID="rptScores" runat="server" OnItemDataBound="rptScores_ItemDataBound">
                <ItemTemplate>
                    <asp:Table ID="tbScoreDetails" runat="server" Width="400">
                        <asp:TableRow>
                            <asp:TableCell Width="300px" BorderWidth="1px" BorderColor="Black">                              
                                <asp:Label ID="lblStudents" Width="100%" runat="server"></asp:Label>
                                <asp:HiddenField ID="hidStudentID" runat="server"></asp:HiddenField>
                            </asp:TableCell>
                            <asp:TableCell ID="tbDynamicControl" runat="server" Width="100px" BorderWidth="1px" BorderColor="Black">                              
                                <telerik:RadNumericTextBox  Width="100%" 
                                     ID="radTxtScores" runat="server" Visible="true"></telerik:RadNumericTextBox> 
                                <asp:RadioButtonList ID="radScore" runat="server" Width="100%" RepeatDirection="Horizontal" Visible="false">
                                    <asp:ListItem  Text="Pass" Value="1"> </asp:ListItem>
                                    <asp:ListItem Text="Fail" Value="0"></asp:ListItem>
                                </asp:RadioButtonList>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <%-- Footer Save & Cancel Button --%>
        <div style="height:10px"></div>    
        <div id="dvFooter" runat="server"> 
            <asp:UpdatePanel ID="udpActions" runat="server">
                <ContentTemplate>
                    <telerik:RadButton ID="radSave" ClientIDMode="Static" Width="60px" runat="server" Text="Save" Skin="Web20" CssClass="saveCss" OnClick="radSave_Click" AutoPostBack="false" OnClientClicked="saveConfirmation"></telerik:RadButton>
                    &nbsp;
                    <telerik:RadButton ID="radCancel" Width="60px" runat="server" Skin="Web20" OnClientClicked="closeCurrentCustomDialog" Text="Cancel"></telerik:RadButton>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
      <div style="height:20px"></div>
    </form>
</body>
</html>