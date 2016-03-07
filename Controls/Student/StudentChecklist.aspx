<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/AddNew.Master" CodeBehind="StudentChecklist.aspx.cs" Inherits="Thinkgate.Controls.Student.StudentChecklist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        div.RadPanelBar .rpRootGroup .rpText {
            text-align: left;
        }

        div.RadPanelBar .rpGroup .rpText {
            text-align: left;
        }
        .RadPanelBar_Web20 .rpExpandable span.rpExpandHandle, .RadPanelBar_Web20 a.rpExpandable:hover .rpNavigation .rpExpandHandle 
        {
         background-position: 0 -5px;
         float: left !important;
         margin-left: 5px !important;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel ID="pnlStudentCheckList" runat="server">
        <div style="width: 870px;">
            <div style="padding-left: 10px">
                <telerik:RadComboBox ID="cmbGrade" runat="server" Skin="Web20" ClientIDMode="Static" AllowCustomText="False"
                    Style="width: 120px;" AutoPostBack="true" OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged">
                </telerik:RadComboBox>
            </div>
            <div style="width: 870px; padding-left: 10px">
                <div style="width: 870px; padding-top: 10px; padding-bottom: 10px;">
                    <table style="background-color: #99CCFF;">
                        <tr>
                            <td style="width: 870px;">
                                <asp:Label ID="lblGradeName" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="border-width: 1px; border-style: solid; border-color: #66CCFF;">
                    <table>
                        <tr style="vertical-align: top;">
                            <td>
                                <table style="width: 620px; padding-bottom: 20px;">
                                    <tr style="vertical-align: top;">
                                        <td>
                                            <b>
                                                <asp:Label ID="lblStudentName" runat="server" Text="Student Name: "></asp:Label></b>
                                            <asp:Label ID="lblStudentNameDisplay" runat="server"></asp:Label>
                                        </td>
                                        <td>
                                            <b>
                                                <asp:Label ID="lblCounselorName" runat="server" Text="Counselor Name: "></asp:Label></b>
                                            <asp:Label ID="lblCounselorNameDisplay" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style="padding-top: 15px;">
                                        <td>
                                            <b>
                                                <asp:Label ID="lblSchoolName" runat="server" Text="School: "></asp:Label>
                                            </b>
                                            <asp:Label ID="lblSchoolNameDisplay" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style="height: 60px">
                                        <td>
                                            <a id="hlnkExpandAll" clientidmode="Static" style="color: blue; text-decoration: underline; cursor: pointer" onclick="return expandAll()">Expand</a>&nbsp;/&nbsp;
                                            <a id="hlnkCollapseAll" clientidmode="Static" style="color: blue; text-decoration: underline; cursor: pointer" onclick="return collapseAll()">Collapse All</a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table style="width: 245px;">
                                    <tr>
                                        <td align="right">
                                            <img runat="server" alt="Paulding County School District" src="../../Images/GAPauldinglogo.jpg" height="70" width="180" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="padding-top: 5px;">
                                            <img id="btnPrintstudentChecklist" alt="Print the student checklist" title="Print the student checklist"
                                                src="../../Images/printIcon.png" class="btn" style ="cursor:pointer;"
                                                onclick="PrintStudentChecklist()" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="padding-top: 10px; width: 870px;">
                    <table style="background-color: #99CCFF; text-align:justify;">
                        <tr>
                            <td>
                                <asp:Label ID="lblIntroduction" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="float: left; margin: 10px 10px 10px 0px; width: 870px;">
                    <telerik:RadPanelBar runat="server" ID="rpnlbarStudentChecklist" ExpandMode="MultipleExpandedItems" Width="870px" Height="100%">
                        <Items>
                            <telerik:RadPanelItem ID="rpnlitemJuly" Text="July" runat="server">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistJuly" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem ID="rpnlitemAugust" Text="August" runat="server">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistAugust" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem ID="rpnlitemSeptember" Text="September" runat="server">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistSeptember" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="October" runat="server" ID="rpnlitemOctober">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistOctober" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>' ReadOnly="True"></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="November" runat="server" ID="rpnlitemNovember">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistNovember" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="December" runat="server" ID="rpnlitemDecember">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistDecember" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="true">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="true">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="January" runat="server" ID="rpnlitemJanuary">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistJanuary" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="February" runat="server" ID="rpnlitemFebruary">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistFebruary" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="March" runat="server" ID="rpnlitemMarch">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistMarch" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="April" runat="server" ID="rpnlitemApril">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistApril" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="May" runat="server" ID="rpnlitemMay">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistMay" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="June" runat="server" ID="rpnlitemJune">
                                <ContentTemplate>
                                    <div style="padding-left: 2px; padding-right: 2px;">
                                        <telerik:RadGrid ID="rgStudentChecklistJune" runat="server" AutoGenerateColumns="False" ShowHeader="false" CellSpacing="0" EnableNoRecordsTemplate="true"
                                            GridLines="None" AllowMultiRowSelection="True">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridTemplateColumn DataField="checkboxStatus" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("checkboxStatus") %>'></asp:CheckBox>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="20px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="Course" ReadOnly="True">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCourse" runat="server" Text='<%# Eval("Course") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle VerticalAlign="Top" Width="40px" />
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="HTMLText" ReadOnly="True"></telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </div>
                                </ContentTemplate>
                            </telerik:RadPanelItem>
                        </Items>
                    </telerik:RadPanelBar>
                </div>
            </div>
        </div>
    </asp:Panel>

    <script type="text/javascript">

        function collapseAll() {
            var panelBar = $find("<%= rpnlbarStudentChecklist.ClientID %>");

            for (var i = 0; i < panelBar.get_allItems().length; i++) {
                panelBar.get_allItems()[i].collapse();
            }
        }

        function expandAll() {
            var panelBar = $find("<%= rpnlbarStudentChecklist.ClientID %>");

            for (var i = 0; i < panelBar.get_allItems().length; i++) {
                panelBar.get_allItems()[i].expand();
            }
        }

        function PrintStudentChecklist() {
            var windowUrl = 'StudentChecklistPrint.aspx ';
            var num;
            var uniqueName = new Date();
            var windowName = 'Print' + uniqueName.getTime();
            var printWindow = window.open(windowUrl, windowName, 'left=50000,top=50000,width=auto,height=auto,scrollbars=1,resizable=1');
            printWindow.focus();
             setTimeout(function () { printWindow.close(); }, 10000);
            return false;
        }

    </script>
</asp:Content>

