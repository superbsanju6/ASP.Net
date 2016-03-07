<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentTags.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.AssessmentTags" %>

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
		</Scripts>
	</telerik:RadScriptManager>
    <div>
        <uc1:Tags runat="server" ClientIDMode="Static" ID="ctlTags" />
    </div>
    </form>
</body>
</html>
