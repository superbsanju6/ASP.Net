<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssociationsToolbar.ascx.cs" Inherits="AssociationsToolbar" %>

<style type="text/css">
	.windowclass
	{
		z-index:10000;
	}
</style>
<telerik:RadMenu ID="AssociationsRadMenu" runat="server" Width="100%" ClickToOpen="True" 
				EnableAutoScroll="True" EnableRootItemScroll="True" Skin="WebBlue" OnClientItemClicked="onClicked" 
	>
</telerik:RadMenu>

<script type="text/javascript">
	function onClicked(sender, eventArgs) {
        // Item is the RadMenuItem that was clicked.
	    var item = eventArgs.get_item();
        // Value is the complete relative url to open in the RadWindow (includes arguments).
	    var value = item.get_value();

/* We should use our normal customDialog(...) in master.js
		var oWnd = window.radopen(value, "RadWindow1");
		oWnd.set_cssClass("windowclass");
		oWnd.setActive();
		oWnd.center();
		//set a function to be called when RadWindow is closed
		oWnd.add_close(onRadWindowClose);
*/
	}

	function onRadWindowClose(sender, eventArgs) {
	//	alert("closed");
	}
</script>
