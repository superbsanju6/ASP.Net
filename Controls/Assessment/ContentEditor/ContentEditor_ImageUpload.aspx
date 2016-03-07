<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentEditor_ImageUpload.aspx.cs"
    Inherits="Thinkgate.Controls.Assessment.ContentEditor.ContentEditor_ImageUpload" %>

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
               
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
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
