<%@ Page Title="Edit Student Demographics" Language="C#" MasterPageFile="~/AddNew.Master" AutoEventWireup="true" CodeBehind="StudentDemographics_Edit.aspx.cs" Inherits="Thinkgate.Controls.Student.StudentDemographics_Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            var parentCustomDialog = getCurrentCustomDialog();
            parentCustomDialog.set_width(450);
            autoSizeWindow();
        });

        function updateStudentDemographics(sender, args) {
            var allDemos = document.getElementsByClassName("comboDemographics");
            var studentID = document.getElementById('hidStudentID').value;
            var stringDemoFields = '';
            for (var i = 0; i < allDemos.length; ++i) {
                stringDemoFields = stringDemoFields + allDemos[i].id.replace("cmbDemoField", "") + "," + $find(allDemos[i].id).get_value();
                if (i != allDemos.length - 1){
                    stringDemoFields = stringDemoFields + "|";
                }
            }
            Service2.UpdateStudentDemographics(studentID, stringDemoFields, updateDemographics_callback_success, updateDemographics_callback_fail);
        }

        function updateDemographics_callback_success() {
            var parentCustomDialog = getCurrentCustomDialog();
            parentCustomDialog.add_beforeClose(refreshParentWindow);
            closeWindow();
        }

        function updateDemographics_callback_fail() {
            alert('Unable to update demographics for the selected student');
        }

        function refreshParentWindow() {
            parent.window.location.reload();
        }
    </script>
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="saveDemographicsLoadingPanel">
            <br />  
            <asp:Table ID="tblDemographics" ClientIDMode="Static" runat="server" HorizontalAlign="Center" CellPadding="3"></asp:Table>
            <br />
            <asp:Table runat="server" HorizontalAlign="Center">
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell HorizontalAlign="Right">
                        <telerik:RadButton runat="server" ID="btnCancel" style="margin: 3px;" Text="Cancel" AutoPostBack="False" OnClientClicked="closeWindow" />
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Left">
                        <telerik:RadButton runat="server" ID="btnUpdate" style="margin: 3px;" Text="Update" AutoPostBack="False" OnClientClicked="updateStudentDemographics"/>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="saveDemographicsLoadingPanel" runat="server" />
    <asp:HiddenField ID="hidStudentID" runat="server" ClientIDMode="Static" />
</asp:Content>
