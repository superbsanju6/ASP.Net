<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs"
    Inherits="Thinkgate.Account.ChangePassword" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head runat="server">
    <title>Change Password</title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-migrate-1.1.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" type="text/javascript"></script>
    <script src="../Scripts/MinimumPasswordRequirement/MinimumPasswordRequirements.js" type="text/javascript"></script>
    <style>
        .ui-dialog-content li {
            font-size: 14px !important;
        }
    </style>
</head>
<body runat="server" id="bodyTag">

    <div id="realBody" style="width: 400px; height: 100%; margin-left: auto; margin-right: auto">
        <form id="form1" runat="server" style="height: 100%;">
            <div id="displayMsgChild" style="display: none;" runat="server" clientidmode="Static">
            </div>
            <telerik:RadScriptManager ID="RadScriptManager1" runat="server">

                <Scripts>
                    <%--Needed for JavaScript IntelliSense in VS2010--%>
                    <%--For VS2008 replace RadScriptManager with ScriptManager--%>
                    <%--<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                    <asp:ScriptReference Path="~/Scripts/fullscreenBackground.js" />--%>

                </Scripts>


            </telerik:RadScriptManager>
            <p style="">
                <br />
                <br />
                <center>
                    <span style="font-size: 10pt; font-weight: bold">Signin successful. You must now change your password:</span>

                    <table style="border: 1px solid #000;">

                        <tr>
                            <td align="right">Old Password &nbsp;&nbsp;
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtOldPassword" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">New Password &nbsp;&nbsp;
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">Confirm New Password &nbsp;&nbsp;
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtConfirmNewPassword" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td align="left">
                                <a runat="server" id="h1MinimumPassword" clientidmode="Static" style="color:blue;text-decoration:underline;cursor:pointer">Minimum Password Requirements</a>
                            </td>
                        </tr>
                        <tr>
                            <td align="right"></td>
                            <td align="left">
                                <asp:Button ID="btnSubmitPasswordChanges" runat="server" Text="Submit Change" CausesValidation="true"
                                    OnClick="btnSubmitPasswordChanges_Click" />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:Label ID="txtMessages" runat="server" Height="100" Width="400" Visible="false" Style="color: #8A0024;" ClientIDMode="Static"></asp:Label>
                    <div id="dvSuccess" runat="server" visible="false">
                        Your password has been changed. Click
            <asp:LinkButton ID="hlSuccess" runat="server" OnClick="hlSuccess_Click">Here</asp:LinkButton>
                        to continue.
                    </div>
                </center>
            </p>
        </form>
    </div>
    <script type="text/javascript">
        $("#h1MinimumPassword").on("click", function () {
           PasswordRequirement.ShowDialog($("#displayMsgChild").html());
        });
    </script>
</body>
</html>
