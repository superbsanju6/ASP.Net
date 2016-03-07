<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentUpload.aspx.cs" Inherits="Thinkgate.Record.AssessmentUpload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function validateRadUpload1(source, arguments) {
            arguments.IsValid = $find("RadUpload1").validateExtensions();
        }

        function ClearUpload() {
            debugger;
            var upload = $find("RadUpload1");
            var fileInputs = upload.getFileInputs();
            for (var i = fileInputs.length - 1; i >= 0; i--) {
                upload.clearFileInputAt(i);
            }
            parent.hide_upload_frame();
        }
    </script>
    <title></title>
</head>
<body style="background-color: transparent">
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="ScriptManager1" runat="server" />
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
        </telerik:RadSkinManager>
        
        <table>
            <tr>
                <td style="font-weight: bold">Local&nbsp;filename:&nbsp;</td>
                <td>
                    <telerik:RadUpload ID="RadUpload1" runat="server" InitialFileInputsCount="1" AllowedFileExtensions=".pdf"
                            MaxFileInputsCount="1" OverwriteExistingFiles="False"
                            ControlObjectsVisibility="None" InputSize="45" Width="500px" EnableFileInputSkinning="true" Skin="Web20"
                            ClientIDMode="Static">
                        <Localization Select="Browse" /> 
                    </telerik:RadUpload>
                    <asp:CustomValidator runat="server" ID="CustomValidator1" Display="Dynamic" ClientValidationFunction="validateRadUpload1">        
                        Invalid extension. The only allowable file type is PDF.
                    </asp:CustomValidator>
                </td>
            </tr>
        </table>
        
        <asp:Button runat="server" ID="buttonSubmit" ClientIDMode="Static" CssClass="roundButtons" Text="&nbsp;&nbsp;Upload&nbsp;&nbsp;" 
                    OnClick="buttonSubmit_Click" style="position: absolute; left: 100px" />
        <asp:Button runat="server" ID="buttonCancel" ClientIDMode="Static" CssClass="roundButtons" Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" 
                    style="position: absolute; left: 300px;" OnClientClick="ClearUpload();" />    
        
    </form>
</body>
</html>
