<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Message.ascx.cs" Inherits="Thinkgate.Controls.Student.Message" %>
<div style="text-align: left;"><b>Subject:</b>
<div runat="server" id="subjectPanel" >
</div>
<b>Body:</b>
<div runat="server" id="bodyPanel" >
</div>
<br />
</div>
<center>
    <telerik:RadButton runat="server" ID="btnSaveProfile" Text="Reply" />
</center>
