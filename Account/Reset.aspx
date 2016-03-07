<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reset.aspx.cs" Inherits="Thinkgate.reset" %>

<%--<link href="Styles/reset.css" rel="stylesheet" />
<link href="Styles/Site.css" rel="stylesheet" />
<link href="Styles/Login.css" rel="stylesheet" />--%>
<link href="../Styles/reset.css" rel="stylesheet" />
<link href="../Styles/Site.css" rel="stylesheet" />
<link href="../Styles/Login.css" rel="stylesheet" />
<script type="text/javascript">
    function closePopitupDialog() {
        parent.$('.ui-icon-closethick').click();
    }
</script>

<form id="frmPwdReset" name="frmPwdReset" runat="server">
    <div class="resetPasswordContent">
     <%--   <div class="resetPasswordPageHeader">User name and Client ID are required to reset the password</div>--%>
           <div class="resetPasswordPageHeader">User name is required to reset the password</div>
        <section class="login-modal" style="opacity: 1;">
            <div class="field email-field">
                <asp:Label CssClass="text" ID="txtUserNameLabel" runat="server" AssociatedControlID="txtUserName">Username:</asp:Label>
                <asp:TextBox ID="txtUserName" runat="server" CssClass="text"></asp:TextBox>
                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="txtUserName"
                    ErrorMessage="Username is required." ToolTip="Username is required." ValidationGroup="myLogin"
                    ForeColor="red">* Username is required</asp:RequiredFieldValidator>
            </div>
           <%-- <div class="field email-field" style="display:none">
                <asp:Label CssClass="text" ID="txtClientIDLabel" runat="server" AssociatedControlID="txtClientID">Client ID:</asp:Label>
                <asp:TextBox ID="txtClientID" runat="server" CssClass="text"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ClientIDRequired" runat="server" ControlToValidate="txtClientID"
                    ErrorMessage="Client ID is required." ToolTip="Client ID is required." ValidationGroup="myLogin"
                    ForeColor="red">* Client ID is required</asp:RequiredFieldValidator>
            </div>--%>

            <div class="footer"  style="top:50px;">
                <asp:Button class="button primary-button" Style="width: 45%; float: left;" ID="btnReset" OnClick="Reset_Click" runat="server" Text="Reset Password" ValidationGroup="myLogin" />
                <asp:Button class="button primary-button" Style="width: 45%; float: right;" ID="btnCancel" OnClientClick="closePopitupDialog();" runat="server" Text="Cancel" />
            </div>

            <div  class="footer" style="padding-bottom: 1em; float:left; color:red; width: 350px !important;top:50px;">
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
            </div>

        </section>
    </div>
</form>



