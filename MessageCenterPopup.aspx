<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MessageCenterPopup.aspx.cs" Inherits="Thinkgate.MessageCenterPopup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body style="background-color: white; font-family: Arial; font-size: 10pt;">
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="ScriptManager2" runat="server" />
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent" />
        <table width="100%">
            <tr>
                <td align="center">
                    <div id="messageContainer" runat="server" clientidmode="Static" style="border: 1px solid #3F76A6; text-align: left;">
                        <asp:Panel ID="MainContent" runat="server">
                            <table width="100%" style="border-collapse: collapse">
                                <tr>
                                    <td style="background-color: #3F76A6;">
                                        <asp:Label ID="Title" runat="server" ForeColor="White"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <table width="100%" style="border: 3px solid white">
                                <tr style="width: 750px; height: 350px; overflow: auto;">
                                    <td style="border: 1px solid; height: 250px; text-align: left; vertical-align: text-top">
                                        <asp:Label ID="Description" BorderColor="White" BorderWidth="2px" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: left; height: 20px">
                                        <asp:Label ID="Attachment" runat="server" Font-Bold="true" Text="Attachments"></asp:Label>
                                        <br />
                                        <asp:Repeater ID="rptAttachments" runat="server">
                                            <ItemTemplate>
                                                <div style="margin-top: 5px; margin-left: 5px;">
                                                    <a href='<%#Eval("AttachmentURL") %>' target="_blank" title='<%#Eval("AttachmentDisplayName") %>'><%#Eval("AttachmentDisplayName") %> </a>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:Panel ID="UserAgreementFooter" runat="server" Font-Names="Arial, Verdana">
                            <table width="100%">
                                <tr style="width: 750px; height: 180px; text-align: center">
                                    <td style="text-align: right">
                                        <telerik:RadButton runat="server" ID="IAgree" OnClick="IAgree_Click" ClientIDMode="Static" Skin="Web20"
                                            Text="I Agree" Width="80px" CssClass="radDropdownBtn" />
                                        &nbsp
                                    </td>
                                    <td style="text-align: left">
                                        <telerik:RadButton runat="server" ID="IDisagree" OnClick="IDisagree_Click" ClientIDMode="Static" Skin="Web20"
                                            Text="I Disagree" Width="80px" CssClass="radDropdownBtn" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:Panel ID="MessageCenterFooter" runat="server" Font-Names="Arial, Verdana">
                            <table width="100%">
                                <tr>
                                    <td style="text-align: left;">
                                        <asp:CheckBox ID="DoNotShow" runat="server" Text="Do Not Show Message Again" OnCheckedChanged="DoNotShow_CheckedChanged" />
                                    </td>
                                    <td style="text-align: right;">
                                        <telerik:RadButton runat="server" ID="OK" OnClick="OK_Click" ClientIDMode="Static" Skin="Web20"
                                            Text="OK" Width="50px" CssClass="radDropdownBtn" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </div>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
