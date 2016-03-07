<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ParentStudentPortalAdministration.ascx.cs" Inherits="Thinkgate.Controls.ParentPortalAdministration.ParentStudentPortalAdministration" %>
<style type="text/css">
    .reportLink > div {
        padding: 3px;
    }
    
        .reportLink > div > a {
        font-size: 14px;
    }
</style>
<div style="text-align: center; padding-top: 15px;height:85%; overflow-y:hidden;" class="reportLink">
    <div id="divParentStudentPortalAdministrationReport" runat="server">
        <a target="_blank" href='<%= ResolveUrl("~/Controls/ParentPortalAdministration/ParentStudentPortalAdministrationReport.aspx") %>'>Parent Student Portal Administration</a>
    </div> 
</div>