<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs"
    Inherits="Thinkgate.Account.ResetPassword" %>

<html>
<body>
    <form runat="server">
    <table border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse;">
        <tr>
            <td>
                <table border="0" cellpadding="0">
                    <tr>
                        <td align="right">
                            <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">Enter your email address:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="Email" runat="server" value=""></asp:TextBox>
                            <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                ErrorMessage="Email is required." ToolTip="Email is required." ValidationGroup="reset">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2" style="color: Red;">
                            <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" colspan="2">
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" ValidationGroup="reset" OnClick="btnSubmit_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
