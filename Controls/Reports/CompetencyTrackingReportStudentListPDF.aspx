<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompetencyTrackingReportStudentListPDF.aspx.cs" Inherits="Thinkgate.Controls.Reports.CompetencyTrackingReportStudentListPDF" %>
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
            <td >
                <asp:Label runat="server" ID="lblViewBy" Font-Bold="true"></asp:Label>

                <span id="tdViewByText" runat="server"></span>
            </td> 
                    
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="lblRubric" Font-Bold="true"></asp:Label>
                <span id="spnRubricValue" runat="server"></span>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="lblCompetency" Font-Bold="true"></asp:Label>
                <a href="#" id="aCompetencyValue" runat="server"></a>
            </td>
        </tr>
        <tr>
            <td runat="server" id="spnCompetencyDetail"></td>
        </tr>
          <tr>
              <td style="border:none">
                   <div style="margin-top:10px; margin-bottom:10px">
    <asp:Label runat="server" ID="lblStudentCount" ForeColor="Black"  Font-Bold="true" ></asp:Label>
   </div>

              </td>
          </tr>

                      <tr>
                <td style="width: 800px; border: none;">

                    <asp:Repeater ID="radGridStudentCount" runat="server"  >
                        <HeaderTemplate>
                            <table style="width: 800px;">
                                <tr>

                                    <td style="width:100px">
                                        <b>Name</b>
                                    </td>
                                    <td style="width:100px">
                                        <b>Grade</b>
                                    </td>
                                      <td >
                                        <b>School</b>
                                    </td>

                                </tr>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>

                                <td>
                                   
                                    <asp:Label ID="lblname" runat="server" Text='<%#Eval("Name") %>' />

                                </td>

                                <td>
                                    <asp:Label ID="lblGrade" runat="server" Text='<%#Eval("Grade") %>' />

                                </td>
                                 <td>
                                    <asp:Label ID="lblSchool" runat="server" Text='<%#Eval("School") %>' />

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
