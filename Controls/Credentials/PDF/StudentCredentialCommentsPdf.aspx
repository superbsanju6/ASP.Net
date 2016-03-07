<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentCredentialCommentsPdf.aspx.cs" Inherits="Thinkgate.Controls.Credentials.PDF.StudentCredentialCommentsPdf" %>
<meta http-equiv="content-type" content="text/xhtml; charset=utf-8" />
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <style type="text/css">
        table {
            border-collapse: collapse;
            border: none;
            font-family: Calibri;
        }

        td {
            border: 1px solid black;
            text-align: center;
            font-size: 14px;
            font-family: Calibri;
            padding-left: 3px;
        }

        table td span {
            font-family: Calibri;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <table style="width: 800px; margin: 0 auto;">
            <tr>
                <td style="text-align: left">

                    <span style="font-size: 12px; font-family: Arial; font-weight: bold; padding: 2px">Student Name :</span>
                    <span style="font-size: 12px; font-family: Arial; padding: 2px">
                        <asp:Label ID="lblstudentname" runat="server"></asp:Label></span>

                </td>

            </tr>
            <tr>
                <td style="text-align: left">

                    <span style="font-size: 12px; font-family: Arial; font-weight: bold; padding: 2px;">Credential :</span>
                    <span>
                        <asp:Label ID="lblCredential" runat="server"></asp:Label></span>


                </td>
            </tr>
            
                    <tr>
                        <td style="border:none; padding:10px;">
                            &nbsp;</td>

                    </tr>

            <tr>
                <td style="width: 800px; border: none;">

                    <asp:Repeater ID="RepDetails" runat="server" >
                        <HeaderTemplate>
                            <table style="width: 800px;">
                                <tr>

                                    <td style="width:100px">
                                        <b>Date</b>
                                    </td>
                                    <td style="width:230px">
                                        <b>Teacher</b>
                                    </td>
                                    <td style="text-align:left">
                                        <b>Comments</b>
                                    </td>

                                </tr>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>

                                <td style="background-color: #EBEFF0;">
                                    <asp:Label ID="Label1" runat="server" Text='<%#Eval("DateCommented") %>' />

                                </td>

                                <td style="background-color: #EBEFF0;">
                                    <asp:Label ID="Label2" runat="server" Text='<%#Eval("CommentedBy") %>' />

                                </td>
                                <td style="background-color: #EBEFF0;text-align:left">
                                    <asp:Label ID="Label3" runat="server" Text='<%#Eval("CommentText") %>' />

                                </td>


                            </tr>

                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>


                </td>
            </tr>
        </table>

    </form>
</body>
</html>
