<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SearchExpanded.Master" CodeBehind="StudentAssignments_Edit.aspx.cs" Inherits="Thinkgate.Controls.Student.StudentAssignments_Edit" %>
<%@ Register Src="~/Controls/Student/AssignmentsAndSharing.ascx" TagName="AssignShare" TagPrefix="E3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="../../Styles/AssignmentsSharing.css" /> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">    
    <E3:AssignShare id="asAssignShare" runat="server"></E3:AssignShare>
</asp:Content>