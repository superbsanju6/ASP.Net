<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserPreferences.aspx.cs" Inherits="Thinkgate.Dialogues.WebForm1" %>
<%@ Import Namespace="System.Drawing" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />    
    <title>User Preferences</title>
    <base target="_self" />
    <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server"></telerik:RadStyleSheetManager>
    <script type="text/javascript">
      

        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function Save() {
            GetRadWindow().close();
        }

        function UpdateColorField(sender, eventArgs) {

            $get("colorHexVal").value = sender.get_selectedColor();
            document.getElementById('radialImageDiv').style.backgroundColor = sender.get_selectedColor();
            document.getElementById('stripesImageDiv').style.backgroundColor = sender.get_selectedColor();

        }
    </script>
</head>
<body>
    <form id="form2" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
        </telerik:RadScriptManager>
        <b>Background Preferences</b>
         <table style="text-align: left; width: 250px; vertical-align:top;">
         <tr>
            <td>
            Image:
            </td>
            <td></td>
         </tr>
         </table>
         <table style="text-align: left; width: 250px; vertical-align:top;">
         <tr>
            <td>
                <asp:RadioButton id="rbRadial" GroupName="images" runat="server" />
                <div id="radialImageDiv" runat="server" clientidmode="Static" >
                    <img src="../Images/bgs/radial_preview.png" />
                </div>
            </td>
            <td>
                <asp:RadioButton id="rbStripes" GroupName="images" runat="server" />
                <div id="stripesImageDiv" runat="server" clientidmode="Static">
                    <img src="../Images/bgs/stripes_preview.png" />
                </div>
            </td>
            <td>
                <asp:RadioButton id="rbWaterfall" GroupName="images" runat="server" />
                <div id="Div1" runat="server" clientidmode="Static" >
                    <img src="../Images/bgs/waterfalls_preview.jpg" />
                </div>
                
            </td>
         </tr>
         </table>

         <table style="text-align: left; width: 250px; vertical-align:top;">
         <tr>
            <td>
                Color:
            </td>
            <td>
                <telerik:RadColorPicker  AutoPostBack="false"  runat="server" PaletteModes="HSV" OnClientColorChange="UpdateColorField"
                ID="RadColorPicker" ShowIcon="true" Preset="Standard"  />
            </td>
         </tr>
        <tr>
            <td>
             
            </td>
            <td>
                <br />
                <asp:Button ID="saveBtn" runat="server" Text="Save" OnClientClick="Save();" OnClick="SaveBtnClick" /><br />
            </td>
        </tr>
    </table>
    </form>
    <input type="hidden" id="colorHexVal" runat="server" clientidmode="Static" />

    <script type="text/javascript">
        function LoadColors() {
            document.getElementById('radialImageDiv').style.backgroundColor = $get("colorHexVal").value;
            document.getElementById('stripesImageDiv').style.backgroundColor = $get("colorHexVal").value;
        }


        LoadColors();
        
        </script>
</body>
</html>