<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UnauthorizedAccess.aspx.cs" Inherits="Thinkgate.UnauthorizedAccess" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <h2>Unauthorized Access</h2>
    <p style="color: white;">
        You have attempted to access a page that you are not authorized to view.
    </p>
    <p style="color: white;">
        If you have any questions, please contact the site administrator.
    </p>
    <p style="color: white;">
        Click &nbsp;
		<asp:HyperLink ID="hlLogin" runat="server" NavigateUrl="~/TGLogin.aspx">here</asp:HyperLink> &nbsp;to login;
    </p>
	
</asp:Content>
