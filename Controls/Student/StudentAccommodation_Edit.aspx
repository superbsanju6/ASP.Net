<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/AddNew.Master" CodeBehind="StudentAccommodation_Edit.aspx.cs" Inherits="Thinkgate.Controls.Student.StudentAccommodation_Edit" %>
   
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 400px;
            height: 24px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <style>
            td {
                /* border: solid blue 1px; */
            }
            td.fieldLabel {
                width: 120px;
                margin-top: 0px;
                padding-bottom: 20px;
                vertical-align: top;
            }
                
            td.fieldEdit {
                vertical-align: top;
            }

            .resultPanel 
            {
                text-align: center;
            }
            tr {
                /* border: solid greenyellow 1px; */
            }
        </style>
        <script type="text/javascript">

            function saveAccommodations() {
                
                autoSizeWindow();
                /***********************************************************************
                    Confirmation before close was only needed if the user cancelled or 
                    closed (x in upper right).  We are now posting back so we can add. 
                    Remove the confirm action from the customDialog's close process.
                    ***********************************************************************/
                getCurrentCustomDialog().remove_beforeClose(onClientBeforeClose);
                var parentCustomDialog = getCurrentCustomDialog();
                parentCustomDialog.isAccommodationSaved = true;
                __doPostBack('RadButtonOk', '');
        }
            $(function () {
                    var parentCustomDialog = getCurrentCustomDialog();
                    parentCustomDialog.set_width(450);
                    if (parentCustomDialog.isAccommodationSaved) {
                        //setting the height of iframe in the timeout function because in IE it takes some time to render the iframe.
                        setTimeout(function () {
                            $(parentCustomDialog.GetContentFrame()).css("height", "50px");
                        }, 100);
                        
                    }
                    autoSizeWindow();
                }
            );
        </script>
    </telerik:RadCodeBlock>
    <div style="overflow: hidden;">
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="saveStudentLoadingPanel">
            <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static" >
                <table class="fieldValueTable fieldAddModalTable" style="width: 400px; margin-left: auto; margin-right: auto;">
                    <tr>
                        <td class="fieldLabel1" style="width: 400px;">
                            <asp:Label runat="server" ID="LabelStudentName" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldLabel1" style="width: 400px;">
                            <telerik:RadGrid ID="gridSubjectLevel" runat="server" AutoGenerateColumns="false" Width="98%" AllowAutomaticUpdates="True">
                                <MasterTableView EditMode="InPlace" AllowAutomaticUpdates="True">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="ID" Display="false" ReadOnly ="True" />
                                        <telerik:GridBoundColumn DataField="StudentID" Display="false" ReadOnly ="True" />
                                        <telerik:GridBoundColumn DataField="Description" HeaderText="Description" ReadOnly="True" />
                                        <telerik:GridTemplateColumn HeaderText="Value" DataField="Value"> 
                                            <ItemTemplate> 
                                                <asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="true" Checked='<%# Convert.ToBoolean(Eval("Value")) %>' /> 
                                            </ItemTemplate> 
                                        </telerik:GridTemplateColumn> 
                                        <telerik:GridBoundColumn DataField="Value" Display="false" ReadOnly ="True" />
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                         </td>
                    </tr>
                   <tr>
                        <td style="text-align: right;">
                            <telerik:RadButton runat="server" ID="RadButtonClose" Text="Close" AutoPostBack="False"
                                OnClientClicked="closeWindow" />
                            &nbsp;
                            <telerik:RadButton runat="server" ID="RadButtonOk" Text="OK" AutoPostBack="False"
                                OnClientClicked="saveAccommodations" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
             <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static" >
                <asp:Label runat="server" ID="lblResultMessage" Text="" CssClass="resultPanel"/>
                <br />
                <telerik:RadButton runat="server" ID="RadButton1" Text="Close" AutoPostBack="False" CssClass="resultPanel" OnClientClicked="refreshParentWindow" />
            </asp:Panel>
       </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="saveStudentLoadingPanel" runat="server" />
    </div>
</asp:Content>
