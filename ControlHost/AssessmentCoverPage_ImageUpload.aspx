<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentCoverPage_ImageUpload.aspx.cs" Inherits="Thinkgate.ControlHost.AssessmentCoverPage_ImageUpload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function validateRadUpload1(source, arguments) {
            arguments.IsValid = $find("RadUpload1").validateExtensions();
        }
    </script>
    <title></title>
</head>
<body style="background-color: transparent">
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="ScriptManager1" runat="server" />
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
    </telerik:RadSkinManager>
    <script type="text/javascript">
        function getRadWindow() //mandatory for the RadWindow dialogs functionality
        {
            if (window.radWindow) {
                return window.radWindow;
            }
            if (window.frameElement && window.frameElement.radWindow) {
                return window.frameElement.radWindow;
            }
            return null;
        }

        function initDialog() //called when the dialog is initialized
        {
            var clientParameters = getRadWindow().ClientParameters;
        }
        if (window.attachEvent) {
            window.attachEvent("onload", initDialog);
        }
        else if (window.addEventListener) {
            window.addEventListener("load", initDialog, false);
        }

        function insertImage(url) //fires when the Insert Link button is clicked
        {
            var closeArgument = {};
            closeArgument.image = url;
            getRadWindow().close(closeArgument);
        }
    </script>
    <table style="width: 100%;">
        <tr>
            <td style="font-weight: bold; width: 45%; text-align: right;">
                Local&nbsp;filename:&nbsp;
            </td>
            <td style="width: 55%;">
                <telerik:RadUpload ID="RadUpload1" runat="server" InitialFileInputsCount="1" AllowedFileExtensions=".jpg,.png,.gif,.bmp"
                    MaxFileInputsCount="1" OverwriteExistingFiles="False" ControlObjectsVisibility="None"
                    InputSize="45" Width="400px" EnableFileInputSkinning="true" Skin="Web20" ClientIDMode="Static">
                    <Localization Select="Browse" />
                </telerik:RadUpload>
                <asp:CustomValidator runat="server" ID="CustomValidator1" Display="Dynamic" ClientValidationFunction="validateRadUpload1">        
                        Invalid extension. The only allowable file types are JPG/PNG/GIF/BMP.
                </asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:center; width:100%; margin:auto;">
                <asp:Button runat="server" ID="buttonSubmit" ClientIDMode="Static" CssClass="roundButtons"
                    Text="&nbsp;&nbsp;Upload&nbsp;&nbsp;" OnClick="buttonSubmit_Click"  />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
