<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewAssessmentItemComments.aspx.cs" Inherits="Thinkgate.Dialogues.Assessment.ViewAssessmentItemComments" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/Styles/Assessment/ViewAssessmentItemComments.css" rel="stylesheet" type="text/css" />
<title></title>
</head>
<body id="body">
    <form id="mainForm" runat="server">
        <div>
            <p id="headerText " align="center">
                <b>
                    <asp:Label runat="server" ID="lblHeader"></asp:Label></b>
            </p>
            <asp:Repeater ID="rptComments" runat="server" OnItemDataBound="OnItemDataBound">
                <ItemTemplate>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblItem" runat="server">  
                                </asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <br/>
                                <asp:Literal ID="ltrComments" runat="server"></asp:Literal>
                            </td>
                        </tr>
                    </table>
                    <hr />
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </form>
</body>
</html>
