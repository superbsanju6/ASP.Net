<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeacherPortalPreview.ascx.cs"
    Inherits="Thinkgate.Controls.Teacher.TeacherPortalPreview" %>
<telerik:RadAjaxPanel ID="contentPanel" runat="server" LoadingPanelID="contentPanelLoadingPanel"
    Width="100%">

    Class:&nbsp;
    <telerik:RadComboBox runat="server" ID="ddlClass" Width="125" AutoPostBack="true"
        OnSelectedIndexChanged="ddlClass_SelectedIndexChanged" />
        <a id="A2" runat="server" href="javascript:OpenAssessmentResults()">Open Portal</a>
        <a id="A1" runat="server" href="~/ControlHost/AssessmentResultsPortal.aspx" target="_blank">(New)</a>
    <br />
    Curriculum:&nbsp;
    <telerik:RadComboBox runat="server" ID="ddlCurriculum" Width="125" AutoPostBack="true"
        OnSelectedIndexChanged="ddlCurriculum_SelectedIndexChanged" />
    <br />    
    <telerik:RadTreeList ID="radTreeResults" runat="server" OnNeedDataSource="radTreeResults_NeedDataSource"
        OnItemDataBound="radTreeResults_ItemDataBound" ParentDataKeyNames="ParentConcatKey"
        Width="290" DataKeyNames="ConcatKey" AutoGenerateColumns="false">
    </telerik:RadTreeList>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="contentPanelLoadingPanel"
    runat="server" />