<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CredentialListPdf.aspx.cs" Inherits="Thinkgate.Controls.Credentials.PDF.CredentialListPdf" %>

<meta http-equiv="content-type" content="text/xhtml; charset=utf-8" />
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Credential List</title>

    <style type="text/css">
        table {
            border-collapse: collapse;
            border: none;
            font-family: Calibri;
        }

        td {
            border: 1px solid black;
            text-align: center;
            font-family: Calibri;
            font-size: 13px;
            padding-left: 3px;
        }

        table td span {
            font-family: Calibri;
            font-size: 11px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <table style="width: 780px; margin: 0 auto;">
            <thead>
                <tr>
                    <td style="border: none; padding: 12px;">&nbsp;</td>

                </tr>
                <tr>
                    <td style="border: none;">
                        <asp:Repeater ID="RepDetails" runat="server" OnItemDataBound="RepDetails_ItemDataBound">
                            <HeaderTemplate>
                                <table>
                                    <tr>
                                        <td style="text-align: left; background-color: #C0C0C0; ">
                                            <b>Credential Name</b>
                                        </td>
                                        <td style="text-align: left; background-color: #C0C0C0; ">
                                            <b>Alignment </b>
                                        </td>
                                        <td style="width: 70px; text-align: left; background-color: #C0C0C0; ">
                                            <b>Status</b>
                                        </td>

                                    </tr>
                            </HeaderTemplate>

                            <ItemTemplate>
                                <tr>
                                    <td style="text-align: left; ">
                                        <asp:Label ID="lblCredentialName" runat="server" Text='<%#Eval("NAME") %>' />
                                        <asp:HiddenField ID="hdCredentialID" Value='<%#Eval("ID") %>' runat="server" />

                                    </td>

                                    <td style="text-align: left; ">
                                        <asp:Repeater ID="rptrAlignment" runat="server">

                                            <ItemTemplate>

                                                <asp:Label ID="lblAlignment" runat="server" Text='<%#Eval("CredentialAlignment") %>' />

                                                <br />
                                            </ItemTemplate>

                                        </asp:Repeater>

                                    </td>

                                    <td style="text-align: left; ">
                                        <asp:Label ID="lblstatus" runat="server" Text='<%#Eval("Status_Desc") %>' />

                                    </td>

                                </tr>

                            </ItemTemplate>
                            <AlternatingItemTemplate>
                                <tr>
                                    <td style="text-align: left; background-color: #dbe5f1; ">
                                        <asp:Label ID="lblCredentialName" runat="server" Text='<%#Eval("NAME") %>' />
                                        <asp:HiddenField ID="hdCredentialID" Value='<%#Eval("ID") %>' runat="server" />

                                    </td>

                                    <td style="text-align: left; background-color: #dbe5f1; ">
                                        <asp:Repeater ID="rptrAlignment" runat="server">

                                            <ItemTemplate>

                                                <asp:Label ID="lblAlignment" runat="server" Text='<%#Eval("CredentialAlignment") %>' />

                                                <br />
                                            </ItemTemplate>

                                        </asp:Repeater>

                                    </td>

                                    <td style="text-align: left; background-color: #dbe5f1; ">
                                        <asp:Label ID="lblstatus" runat="server" Text='<%#Eval("Status_Desc") %>' />

                                    </td>



                                </tr>


                            </AlternatingItemTemplate>

                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>

                </tr>

            </thead>

        </table>

    </form>
</body>
</html>
