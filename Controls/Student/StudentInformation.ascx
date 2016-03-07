<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentInformation.ascx.cs"
    Inherits="Thinkgate.Controls.Student.StudentInformation" %>
<%--<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />--%>
<div class="identificationTile-DivAroundTable">
    <telerik:RadAjaxPanel ID="studentProfilePanel" runat="server" LoadingPanelID="studentProfileLoadingPanel"
        Width="100%" Height="100%">

        <table width="100%" class="fieldValueTable">
            <tr>
                <td class="fieldLabel">Name:
                </td>
                <td>
                    <asp:Label runat="server" ID="lblStudentName" />
                </td>
            </tr>

            <!--JDW: New labels for Additional Demographic fields. I will leave this in case we go back to literals put on page-->
            <%--<tr>
            <td class="fieldLabel">
                Address:
            </td>
            <td>
                <asp:Label runat="server" ID="lblStudentAddress" />
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Home Phone: 
            </td>
            <td>
                <asp:Label runat="server" ID="lblPhone" /> 
            </td>
        </tr>
        <tr>
            <td class="fieldLabel">
                Date Of Birth:
            </td>
            <td>
                <asp:Label runat="server" ID="lblBirthDate"/>
            </td>
        </tr>--%>

            <tr>
                <td class="fieldLabel">Student ID:
                </td>
                <td>
                    <asp:Label runat="server" ID="lblStudentID" />
                </td>
            </tr>
            <tr>
                <td class="fieldLabel">School:
                </td>
                <td>
                    <asp:Label runat="server" ID="lblSchool" />
                </td>
            </tr>
            <tr>
                <td class="fieldLabel">Grade:
                </td>
                <td>
                    <asp:Label runat="server" ID="lblGrade" />
                </td>
            </tr>
            <tr>
                <td class="fieldLabel">Email:
                </td>
                <td>
                    <a runat="server" id="hlStudentEmail" href="#">
                        <asp:Label runat="server" ID="lblEmail" />
                    </a>
                </td>
            </tr>
        </table>

        <table runat="server" id="tblStudentDemographics" class="fieldValueTable" width="125%">
        </table>

    </telerik:RadAjaxPanel>
</div>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="studentProfileLoadingPanel"
    runat="server" />
