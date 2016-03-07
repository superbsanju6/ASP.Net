<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TagUserControl.aspx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.TagUserControl" %>

<%@ Register  Src="~/Controls/E3Criteria/CriteriaControls/Tags.ascx" TagPrefix="e3" TagName="SearchControl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
		<telerik:RadScriptManager ID="RadScriptManager2" runat="server">
		<Scripts>
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
		</Scripts>
	</telerik:RadScriptManager>
	    <div>
	        <e3:SearchControl ID="LRMITags" ClientIDMode="Static" runat="server"></e3:SearchControl>
	    </div>
        
	</form>

</body>
</html>
