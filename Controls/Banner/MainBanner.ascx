<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainBanner.ascx.cs"
    Inherits="Thinkgate.Controls.Banner.MainBanner" %>
<script src="<%: ResolveUrl("~/Scripts/MinimumPasswordRequirement/MinimumPasswordRequirements.js") %>"></script>
<script type="text/javascript">
    function helpItemClicked(sender, args) {
        var action = args._item._attributes.getAttribute('action');
        var actionUrl = args._item._attributes.getAttribute('actionUrl');
        
        switch (action) {
            case 'window':
                window.open(actionUrl);
                break;
            case 'customDialog':
                var actionName = args._item._attributes.getAttribute('actioName');
                var actionTitle = args._item._attributes.getAttribute('actionTitle');                
                customDialog({ name: actionName, maximize: true, title: actionTitle, url: actionUrl });
                break;
            case 'href':
                window.location.href(actionUrl);
                break;
            default:
                break;
        }
                
    }

    function showMenuAt(id, sender, e) {
        var contextMenu = $find(id);
        var x = $(sender).offset().left;
        var y = $("#headerDiv").height();

        if (isNaN(x) || isNaN(y)) {
            alert("Please provide valid integer coordinates");
            return;
        }

        contextMenu.showAt(x, y);

        $telerik.cancelRawEvent(e);
    }

    function showMenuHelpAt(sender, e) {
       showMenuAt("<%= ctxHelpMenu.ClientID %>", sender, e);
    }

    function showMenuAccountAt(sender, e) {
        showMenuAt("<%= ctxAccountMenu.ClientID %>", sender, e);
    }

    function SignOffButtonNonSso_ClientClick(sender, e) {
        Service2.Logout(signOff_CallBack_Success, signOff_CallBack_Success);
        return false; // do NOT invoke the OnClick function
    }

    function SignOffButtonSso_ClientClick(sender, e) {
        return true; // DO invoke the OnClick function
    }

    //function SignOffButton_Click(sender, e) {
    //    Service2.Logout(signOff_CallBack_Success, signOff_CallBack_Success);
    //    return false;
    //}

    function signOff_CallBack_Success() {
        window.close();
    }

    function RefreshWindow() {
        window.location.reload();
    }
</script>
<asp:ImageButton ID="btnRefresh" ToolTip="Refresh" Visible="false" ImageUrl="~/Images/refresh.gif" style="margin-right: 20px; vertical-align: bottom;" runat="server" OnClientClick="RefreshWindow" />
<asp:LinkButton runat="server" ID="HomeButton" Text="Home" AlternateText="Home" OnClick="HomeButton_Click" />
| 
<asp:LinkButton runat="server" ID="AccountLink" Text="Account" AlternateText="Account" OnClientClick="showMenuAccountAt(this, event)" />
|
<asp:LinkButton runat="server" ID="HelpLink" Text="Help" AlternateText="Help" OnClientClick="showMenuHelpAt(this, event)" />
|
<asp:LinkButton runat="server" ID="SignOffLink" Text="Log Out" AlternateText="Sign Off" OnClientClick="SignOffButtonNonSso_ClientClick();" OnClick="SignOffButton_Click" />
<telerik:RadContextMenu ID="ctxHelpMenu" runat="server" EnableRoundedCorners="true"
    EnableShadows="True" Skin="Office2010Silver" OnClientItemClicked="helpItemClicked" ClientIDMode="Static">
    <Items>
    </Items>
</telerik:RadContextMenu>
<telerik:RadContextMenu ID="ctxAccountMenu" runat="server" EnableRoundedCorners="true"
    EnableShadows="True" Skin="Office2010Silver" OnClientItemClicked="helpItemClicked" ClientIDMode="Static">
    <Items>
    </Items>
</telerik:RadContextMenu>
