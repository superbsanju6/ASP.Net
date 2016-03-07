<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportTestResponseCard.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.ImportTestResponseCard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="radScriptManager" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
            </Scripts>
        </telerik:RadScriptManager>

        <div align="center" style="border: 1px solid black; font-family: Arial; font-size: 10pt">
            <br />
            <br />
            <table cellspacing="0" cellpadding="4" border="1" style="border-color: black; text-align: left; width: 420px">
                <tr>
                    <td style="width: 100px"><b>Grade: </b></td>
                    <td>
                        <asp:Label ID="lblGrade" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td><b>Subject: </b></td>
                    <td>
                        <asp:Label ID="lblSubject" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td><b>Course: </b></td>
                    <td>
                        <asp:Label ID="lblCourse" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td><b>Assessment: </b></td>
                    <td>
                        <asp:Label ID="lblAssessment" runat="server"></asp:Label></td>
                </tr>
            </table>
            <br />
            <br />
            <p>
                Click browse to select a Test Response Card file to import.
            </p>
            <telerik:RadUpload ID="radFileUpload"
                runat="server" InitialFileInputsCount="1" AllowedFileExtensions=".csv"
                MaxFileInputsCount="1" OverwriteExistingFiles="False" ControlObjectsVisibility="None"
                InputSize="70" Width="500px" EnableFileInputSkinning="true" Skin="Web20"
                ClientIDMode="Static" CssClass="formElement">
                <Localization Select="Browse" />
            </telerik:RadUpload>
            <br />
            <telerik:RadButton ID="radImport" runat="server" AutoPostBack="true" Text="Import" Skin="Web20" OnClick="radImport_Click"></telerik:RadButton>
            &nbsp;
            <telerik:RadButton ID="radCancel" runat="server" AutoPostBack="false" Text="Cancel" Skin="Web20" OnClientClicked="CloseWin"></telerik:RadButton>
            <br />
            <br />
            <br />
        </div>

        <script type="text/javascript">
            function CloseWin() {
                closeCurrentCustomDialog();
            }

            function MessageAlert(message) {
                parent.customDialog({ title: 'Test Response Card Import', maximize: true, maxwidth: 500, maxheight: 100, animation: 'None', dialog_style: 'alert', content: message }, [{ title: 'OK' }]);;
            }
        </script>
    </form>
</body>
</html>
