<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentCredentialPdf.aspx.cs" Inherits="Thinkgate.Controls.Credentials.PDF.StudentCredentialPdf" %>

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
            font-size: 13px;
            padding-left: 3px;
            font-family: Calibri;
        }

        table td span {
            font-family: Calibri;
            font-size: 11px;
        }
        ul, li {
            margin: 0px;
            padding: 0px;
            list-style-type: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <table style="width: 800px;">
            <thead>
                <tr>
                    <td style="border: none; text-align: left;">
                        <table >
                            <tr>
                                <td style="text-align: left; padding-right: 30px; padding-left: 3px">

                                    <b>Student Name:</b>


                                </td>
                                <td>
                                    <span style="padding-left: 30px; padding-right: 30px">
                                        <asp:Label ID="lblstuaname" runat="server"></asp:Label></span>

                                </td>

                            </tr>
                            <tr>
                                <td style="text-align: left; padding-right: 30px; padding-left: 3px">

                                    <b>Credentials Earned :</b>



                                </td>
                                <td>
                                    <span style="padding-left: 30px; padding-right: 30px">
                                        <asp:Label ID="lblcredentialcount" runat="server"></asp:Label></span>

                                </td>
                            </tr>

                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="border: none; padding: 10px;">&nbsp;</td>

                </tr>
                <tr>
                    <td style="width: 800px; border: none;">
                        <asp:Repeater ID="RepDetails" runat="server" OnItemDataBound="RepDetails_ItemDataBound">
                            <HeaderTemplate>
                                <table>
                                    <tr>
                                        <td style="text-align: left; width: 200px;">
                                            <b>Credential Name</b>
                                        </td>
                                        <td style=" width: 200px;">
                                            <b>Date Earned </b>
                                        </td>
                                        <td style="width: 200px;">
                                            <b>Expiration Date</b>
                                        </td>
                                        <td style="width: 200px;">
                                            <b>Recorded By</b>
                                        </td>

                                    </tr>
                            </HeaderTemplate>

                            <ItemTemplate>
                                <tr>
                                    <td style="background-color: #dbe5f1; text-align: left;">
                                        <asp:Label ID="lblSubject" runat="server" Text='<%#Eval("CREDENTIALNAME") %>' Font-Bold="true" />
                                        <asp:HiddenField ID="hdCredentialID" Value='<%#Eval("CredentialID") %>' runat="server" />

                                    </td>

                                    <td style="background-color: #dbe5f1;">
                                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("EarnedDate") %>' />

                                    </td>

                                    <td style="background-color: #dbe5f1;">
                                        <asp:Label ID="Label2" runat="server" Text='<%#Eval("ExpirationDate") %>' />

                                    </td>
                                    <td style="background-color: #dbe5f1;">
                                        <asp:Label ID="Label3" runat="server" Text='<%#Eval("recordedby") %>' />

                                    </td>


                                </tr>
                                <tr>
                                    <td style="text-align: right; padding-right: 3px">Comments:

                                    </td>
                                    <td style="text-align: left;" colspan="3">
                                        <asp:Repeater ID="rptrcomments" runat="server" >
                                            <HeaderTemplate>
                                               <ul>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                
                                                   <li>
                                                    <asp:Label ID="lblComment" runat="server" Text='<%#Eval("CommentText") %>'  />
                                                    </li>
                                               

                                            </ItemTemplate>

                                            <FooterTemplate>
                                              </ul>
                                            </FooterTemplate>

                                        </asp:Repeater>

                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right; padding-right: 3px">URL:

                                    </td>
                                    <td style="text-align: left;" colspan="3">
                                        <asp:Repeater ID="rptrurl" runat="server" OnItemDataBound="rptrurl_ItemDataBound">
                                            <HeaderTemplate>
                                               <ul>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                               
                                                   <li>
                                                    <asp:LinkButton ID="lblURL" runat="server" Text='<%#Eval("URL") %>'  />
                                               
                                                </li>
                                              

                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </ul>
                                            </FooterTemplate>
                                        </asp:Repeater>

                                    </td>
                                </tr>

                            </ItemTemplate>
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
