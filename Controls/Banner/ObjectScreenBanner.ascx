<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObjectScreenBanner.ascx.cs"
    Inherits="Thinkgate.Controls.Banner.ObjectScreenBanner" %>
    
<script type="text/javascript">
    function closeWindow() {
        window.open('', '_parent', '');
        window.close();
    }

    function showMenuAt(sender, e) {
        var contextMenu = $find("RadContextMenuActions");
        var x = $(sender).offset().left;
        var y = $("#headerDiv").height();

        if (isNaN(x) || isNaN(y)) {
            alert("Please provide valid integer coordinates");
            return;
        }

        contextMenu.showAt(x, y);

        $telerik.cancelRawEvent(e);
    }

</script>
<asp:LinkButton runat="server" ID="LinkButtonActions" Text="Actions" AlternateText="Actions" OnClientClick="showMenuAt(this, event)" />
| <a href="javascript:closeWindow()">Close</a>
<asp:PlaceHolder runat="server" ID="PlaceHolderActions"></asp:PlaceHolder>
