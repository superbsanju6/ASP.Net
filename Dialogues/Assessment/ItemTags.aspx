<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ItemTags.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.ItemTags" %>

<%@ Register Src="~/Controls/E3Criteria/CriteriaControls/Tags.ascx" TagPrefix="uc1" TagName="Tags" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="ScriptManager" runat="server">
		<Scripts>
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
               
		    <asp:ScriptReference Path="~/Scripts/jquery-1.9.1.js" />
		    <asp:ScriptReference Path="~/Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
            <asp:ScriptReference Path="~/Scripts/jquery.inputmask.js"/>
		</Scripts>
	</telerik:RadScriptManager>
    <div>
        <uc1:Tags runat="server" ClientIDMode="Static" ID="ctlTags" />
    </div>
    </form>
</body>
</html>
