<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentCountListPdf.aspx.cs" Inherits="Thinkgate.Controls.Credentials.PDF.StudentCountListPdf" %>
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
            font-family: Calibri;
            padding-left: 3px;
        }

        table td span {
            font-family: Calibri;
            font-size: 14px;
        }
        ul, li {
            margin: 0px;
            padding: 0px;
            list-style-type: none;
        }
    </style>
    <style type="text/css">
    .WordWrap { width:100%;word-break : break-all }
    .WordBreak { width:100px; OVERFLOW:hidden; TEXT-OVERFLOW:ellipsis}
</style>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
        <table style="width: 800px;">
            <thead>

                <tr>

                    <td style="border: none; width:500px">
                        <table class="WordWrap" >
                            <tr>
                                <td style="text-align: left; vertical-align: top;">

                                    <span><b>Credential Name:</b></span>


                                </td>
                                <td style="text-align: center;">
                                    <span>
                                        <asp:Label ID="lblcredentialName" runat="server" Width="200px"></asp:Label></span>

                                </td>

                            </tr>
                            <tr>
                                <td style="text-align: left;  vertical-align: top;">

                                    <span><b>Student Count:</b></span>



                                </td>
                                <td>
                                    <span>
                                        <asp:Label ID="lblstudentcount" runat="server"></asp:Label></span>

                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: left !important; vertical-align: top;" >

                                    <span><b>URL Links:</b></span>



                                </td>
                                <td style="text-align: left !important;" >
                                    <asp:GridView ID="rptrurl" runat="server" AutoGenerateColumns="False" BorderStyle="None" BorderWidth="0px"  ShowHeader="False" GridLines="None">
                                        
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>

                                                    
                                                    <asp:LinkButton ID="lblURL" runat="server"  Text='<%#Eval("URL") %>'  />



                                                </ItemTemplate>
                                                <ItemStyle BorderWidth="0px" BorderStyle="None" Wrap="true" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>

                    </td>

                </tr>

                <tr>
                    <td style="border: none; padding: 6px;">&nbsp;</td>

                </tr>
                <tr>
                    <td style="width: 800px; border: none;">
                        <asp:Repeater ID="RepDetails" runat="server" OnItemDataBound="RepDetails_ItemDataBound">
                            <HeaderTemplate>
                                <table style="width: 800px;">
                                    <tr>
                                        <td style="text-align: left;">
                                            <b>Student Name</b>
                                        </td>
                                        <td>
                                            <b>Date Earned </b>
                                        </td>
                                        <td>
                                            <b>Expiration Date</b>
                                        </td>
                                        <td>
                                            <b>Recorded By</b>
                                        </td>

                                    </tr>
                            </HeaderTemplate>

                            <ItemTemplate>
                                <tr>
                                    <td style="background-color: #dbe5f1; text-align: left; width: 230px">
                                        <asp:Label ID="lblSubject" runat="server" Text='<%#Eval("StudentName") %>' Font-Bold="true" />

                                        <asp:HiddenField ID="hdCredentialID" Value='<%#Eval("CredentialID") %>' runat="server" />

                                    </td>

                                    <td style="background-color: #dbe5f1; width: 190px">
                                        <asp:Label ID="Label1" runat="server" Text='<%# string.Format("{0:MM/dd/yyyy}", Eval("DateEarned"))%>'   />

                                    </td>

                                    <td style="background-color: #dbe5f1;width: 190px">
                                        <asp:Label ID="Label2" runat="server" Text='<%# string.Format("{0:MM/dd/yyyy}", Eval("ExpirationDate"))%>'  />

                                    </td>
                                    <td style="background-color: #dbe5f1;width: 190px">
                                        <asp:Label ID="Label3" runat="server" Text='<%#Eval("RecordedBy") %>' />

                                    </td>


                                </tr>
                                <tr>
                                    <td style="text-align: right; padding-right: 3px; font-family: Calibri">Comments:

                                    </td>
                                    <td style="text-align: left;" colspan="3">
                                        <asp:Repeater ID="rptrcomments" runat="server">
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
