<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompetencyTrackingDetailsStandardPDF.aspx.cs" Inherits="Thinkgate.Controls.Reports.CompetencyTrackingDetailsStandardPDF" %>

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
            text-align: left;
            font-size: 12px;
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
                <td><strong>Student Name: </strong>
                    <asp:Label ID="lblStudentName" runat="server"></asp:Label>
                </td>

            </tr>
            <tr>
                <td><strong>Rubric Value: </strong>
                    <asp:Label ID="lblrubricvalue" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td><strong>Standard: </strong>
                    <asp:Label ID="lnkStandard" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style1">
                    <asp:Label ID="lblStandardDesc" runat="server"></asp:Label>
                </td>

            </tr>
            <tr>

                <td style="border: none">
                    <div style="margin-top: 10px; margin-bottom: 10px">
                        <strong>Competencies:</strong>
                        <asp:Label runat="server" ID="lblcount" ForeColor="Black"  Font-Bold="true" Text="0"></asp:Label>
                    </div>

                </td>



            </tr>

            <tr>
                <td style="width: 800px; border: none;">

                    <asp:Repeater ID="RepDetails" runat="server">
                        <HeaderTemplate>
                            <table style="width: 800px;">
                                <tr>

                                    <td style="width: 100px">
                                        <b>Competency</b>
                                    </td>
                                    <td style="width: 230px">
                                        <b>Descripion</b>
                                    </td>


                                </tr>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>

                                <td>

                                    <a id="lnkChildStandard" runat="server"><%#Eval("ChildStandard") %></a>

                                </td>

                                <td>
                                    <asp:Label ID="lblChildStandardDesc" runat="server" Text='<%#Eval("ChildStandardDesc") %>' />

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
