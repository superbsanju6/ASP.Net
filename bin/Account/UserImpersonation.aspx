<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserImpersonation.aspx.cs" Inherits="Thinkgate.Account.UserImpersonation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/impersonationForm.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div runat="server" id="frmImpersonate" style="position: fixed; top: 50%; left: 50%;" visible="false">
        <div id="impersonationFormBackground">
            <p class="impersonationHeader">
                IMPERSONATE A USER
            </p>
            <p>
                <asp:Label runat="server" ID="lblError" Visible="false" ForeColor="Red"></asp:Label>
            </p>
            <p class="text">
                <span class="impersonationLabel">User To Impersonate</span>
                <asp:TextBox runat="server" ID="UserToImpersonate" Text="" CssClass="impersonationInputBox"></asp:TextBox>
            </p>
            <p class="text">
                <span class="impersonationLabel">Password</span>
                <asp:TextBox runat="server" ID="ImpersonatePW" Text="" TextMode="Password" CssClass="impersonationInputBox"></asp:TextBox>
            </p>
            <p>
                <asp:Button runat="server" ID="btnImpersonate" Text="Impersonate" OnClick="BtnImpersonateClick" CssClass="impersonateButton"></asp:Button>
            </p>
        </div>
    </div>
</asp:Content>