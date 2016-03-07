<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ChgTeacherPasswordAdmin.aspx.cs" Inherits="Thinkgate.Controls.ChgTeacherPasswordAdmin" %>
<%@ Register Src="~/Account/PwdChgCtrl.ascx" TagName="PwdChgCtrl" TagPrefix="e3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta http-equiv="X-UA-Compatible" content="IE=8" />
        <meta http-equiv="PRAGMA" content="NO-CACHE" />
        <meta http-equiv="Expires" content="-1" />
        <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
         <title></title>
        <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
            <StyleSheets>
                <telerik:StyleSheetReference Path="~/Styles/reset.css" />
            </StyleSheets>
        </telerik:RadStyleSheetManager>
        <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
        <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
        <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css" rel="stylesheet" type="text/css" runat="server" />    
        <style type="text/css">
            body
            {
                font-family: Sans-Serif, Arial;
                background-color: white;
            }

            .fieldAddModalTable td
            {
                vertical-align: text-top;
            }
        
        .RadComboBox_Web20 .rcbInputCellLeft, .RadComboBox_Web20 .rcbInputCellRight, .RadComboBox_Web20 .rcbArrowCellLeft, .RadComboBox_Web20 .rcbArrowCellRight
        {
            /*background-image: url('../../../Images/rcbSprite.png') !important;*/
        }
        
        .RadComboBox_Web20 .rcbHovered .rcbInputCell .rcbInput, .RadComboBox_Web20 .rcbFocused .rcbInputCell .rcbInput
        {
            color: #fff !important;
        }
        
        .RadComboBox_Web20 .rcbMoreResults a
        {
            /*background-image: url('../../../Images/rcbSprite.png') !important;*/
        }
        
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
            
            <!-- The following tag is needed because it adds to functions to our rad controls whiche we use to confirm when 
                 user closes the customDialog windows                                                                       -->
            <telerik:RadScriptManager ID="rsmPasswordChange" runat="server">
                <Scripts>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                    <asp:ScriptReference Path="~/Scripts/jquery-1.9.1.min.js" />
                    <asp:ScriptReference Path="~/Scripts/master.js" />
                    <asp:ScriptReference Path="~/Scripts/EditSubmitResultPagesWithinCustomDialog.js"></asp:ScriptReference>
                    <asp:ScriptReference Path="~/Scripts/master.js"></asp:ScriptReference>
                </Scripts>
            </telerik:RadScriptManager>
            
            <!-- The following tag is needed because it adds to the window object the "radConfirm()" function whiche we use
                 to confirm when user closes the customDialog windows                                                       -->
            <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
                Skin="Thinkgate_Window" EnableEmbeddedSkins="False" Modal="True" InitialBehaviors="Close"
                Animation="None">
            </telerik:RadWindowManager>

            <e3:PwdChgCtrl ID="ctrlPW" runat="server"></e3:PwdChgCtrl>

        </form>
    </body>
    <telerik:RadCodeBlock runat="server" ID="pwdChgCtrl_RadCodeBlock" ClientIDMode="Static">
        <script type="text/javascript">
            /*************************************************************************************
            Initialization Script
            *************************************************************************************/
            $(function () {

                /**************************************************************************************
                These buttons are in the pwdChgCtrl ascx control.  These aren't mapped in the ascx 
                control themselves because the control doesn't know in what page or context it 
                will be called, so we need to map them from with the page that contains the ascx 
                control.
                **************************************************************************************/
                plsWireUp_CancelBtnClickedEvent = closeWindow;
                plsWireUp_CloseBtnClickedEvent = closeWindow;
                //$('#pwdChgCtrl_CancelBtn')[0].OnClientClicked = closeWindow;  //function closeWindow() is in EditSubmitResultPagesWithinCustomDialog.js
                //$('#pwdChgCtrl_CloseBtn')[0].OnClientClicked = closeWindow;

                addWindowBeforeCloseEvent(); //This and other common functions are in AddNew.Master
            });
        </script>
    </telerik:RadCodeBlock>
</html>
