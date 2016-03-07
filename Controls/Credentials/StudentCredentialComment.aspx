<%@ Page Title="Comments" Language="C#" MasterPageFile="~/AddNew.Master" AutoEventWireup="true" CodeBehind="StudentCredentialComment.aspx.cs" Inherits="Thinkgate.Controls.Credentials.StudentCredentialComment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <telerik:RadWindow ID="modalPopup" Title="Comments" Width="600px" Height="300px" runat="server" Modal="true" KeepInScreenBounds="true">
        <ContentTemplate>
            <table style="margin-left: auto; margin-right: auto; margin-top: 12px">
                <tr>
                    <td colspan="2">
                        <telerik:RadTextBox runat="server" MaxLength="250" TextMode="MultiLine" Width="500px" Height="115px"></telerik:RadTextBox></td>
                </tr>
                <tr style="empty-cells: show">
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td align="right" style="width: 80%; padding-top: 40px;">
                        <telerik:RadButton ID="RadButtonsave" Width="80px" Height="25px" runat="server" Text="Save"></telerik:RadButton>
                    </td>
                    <td align="center" style="width: 20%; padding-top: 40px;">
                        <telerik:RadButton ID="RadButtonCancel" runat="server" Text="Cancel" Width="80px" Height="25px"></telerik:RadButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </telerik:RadWindow>
    <table style="margin-top: 14px; margin-left: 11px; width: 94%; height: 80%">
        <tr>
            <td style="border: thin solid #000000;">
               
                   <span style="font-size: 12px; font-family: Arial; font-weight: bold; padding: 2px">  Student Name :</span>
                  <span style="font-size: 12px; font-family: Arial;  padding: 2px">  <asp:Label ID="lblstudentname"   runat="server" ></asp:Label></span>
                
            </td>
            <td rowspan="2" style="border-style: solid solid solid solid; border-width: thin; border-color: #000000; width: 130px" align="center">
                <asp:ImageButton ID="btnPrintBtn" runat="server" OnClick="btnPrintBtn_Click" ImageUrl="~/Images/Toolbars/print.png" />
            </td>
        </tr>
        <tr>
            <td style="border-style: solid solid solid solid; border-width: thin; border-color: #000000;">
                
                   <span style="font-size: 12px; font-family: Arial; font-weight: bold; padding: 2px;">  Credential :</span>  
                    <span>  <asp:Label ID="lblCredential"  runat="server" ></asp:Label></span> 

               
            </td>
        </tr>
        <tr>
            <td style="text-align: right;" colspan="2">
                <div style="padding: 10px; padding-top: 30px; padding-right: 0px; height: 20px;">

                    <telerik:RadButton ID="RadButton1" runat="server" Text="Add Comment" Skin="Web20" AutoPostBack="false" OnClientClicked="showAddNewCommentWindow">
                        <Icon PrimaryIconUrl="~/Images/add.gif" PrimaryIconWidth="15px" PrimaryIconHeight="15px"
                            PrimaryIconTop="4px" PrimaryIconLeft="7px"></Icon>
                    </telerik:RadButton>
                </div>
            </td>

        </tr>
        <tr>
            <td colspan="2">
                <div>
                    <telerik:RadGrid ID="RadGridStudentCredentialsComment" runat="server" AutoGenerateColumns="False" MasterTableView-DataKeyNames="ID" CellSpacing="0" CellPadding="0" GridLines="None" Skin="Web20" OnItemCommand="RadGridStudentCredentialsComment_ItemCommand" OnItemDataBound="RadGridStudentCredentialsComment_ItemDataBound"
                        OnPreRender="RadGridStudentCredentialsComment_PreRender"
                        >
                        <MasterTableView>
                            <Columns>

                                <telerik:GridButtonColumn ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ButtonType="ImageButton" HeaderStyle-Width="80px" 
                                    ImageUrl="~/Images/deleteIcon.png" HeaderTooltip="Click to Delete Comment" 
                                    ConfirmText="Are you sure you wish to remove this entry? Once removed, it cannot be retrieved." ConfirmDialogType="RadWindow" ConfirmDialogWidth="450px"
                                    ConfirmTitle="Delete Comment" ConfirmDialogHeight="90px" CommandName="Delete" FilterControlAltText="Filter column column" HeaderText="Remove" UniqueName="RemoveBtn">
                                </telerik:GridButtonColumn>

                                <telerik:GridTemplateColumn HeaderStyle-Width="55px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" DataField="ID" FilterControlAltText="Filter TemplateColumn column" HeaderText="Edit" UniqueName="TemplateColumn">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "ID") %>' />
                                        <asp:ImageButton ID="lnkEdit" runat="server" ToolTip="Click to Edit Comment" CommandName="Edit" 
                                            OnClientClick="showEditCommentsWindow(this); return false" Width="12px" Height="12px"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' SkinID="Close" ImageUrl="~/Images/Edit.png" />
                                    </ItemTemplate>

                                    <HeaderStyle Width="55px"></HeaderStyle>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderStyle-Width="55px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top"
                                     HeaderText="Date" >
                                    <ItemTemplate>
                                        <span id="spnCommentDate" runat="server">'<%# DataBinder.Eval(Container.DataItem, "DateCommented") %>'</span>
                                    </ItemTemplate>
                                    <HeaderStyle Width="55px"></HeaderStyle>
                                </telerik:GridTemplateColumn>

                                <telerik:GridBoundColumn DataField="CommentedBy" HeaderStyle-Width="155px" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top"  FilterControlAltText="Filter column4 column" HeaderText="Teacher" UniqueName="column4">

                                    <ColumnValidationSettings>
                                        <ModelErrorMessage Text=""></ModelErrorMessage>
                                    </ColumnValidationSettings>

                                    <HeaderStyle Width="155px"></HeaderStyle>
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="CommentText" FilterControlAltText="Filter column5 column" HeaderText="Comments" UniqueName="column5">

                                    <ColumnValidationSettings>
                                        <ModelErrorMessage Text=""></ModelErrorMessage>
                                    </ColumnValidationSettings>

                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="CreatedBy" Display="false" UniqueName="userid" ItemStyle-Width="0px" HeaderStyle-Width="0px">
                                    <ColumnValidationSettings>
                                        <ModelErrorMessage Text=""></ModelErrorMessage>
                                    </ColumnValidationSettings>

                                    <HeaderStyle Width="0px"></HeaderStyle>

                                    <ItemStyle Width="0px"></ItemStyle>
                                </telerik:GridBoundColumn>


                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </td>
        </tr>
    </table>
    <telerik:RadCodeBlock runat="server" ID="rdbScripts">
        <script type="text/javascript">          
            function showAddNewCommentWindow() {
                var StudentCredentialID = '<%= this.studentCredentialID %>';
                var StudentID = '<%= this.studentID %>';
                var CredentialID = '<%= this.credentialID %>';
                customDialog({ url: '../Credentials/AddComment.aspx?CommentID=0' + '&studCrdId=' + StudentCredentialID + '&studId=' + StudentID + '&crdId=' + CredentialID, maximize: true, maxwidth: 461, maxheight: 220 });
            }

            function showEditCommentsWindow(obj) {
                var ID = $(obj).siblings("[id$=hdnID]").first().val();

                var StudentCredentialID = '<%= this.studentCredentialID %>';
                var StudentID = '<%= this.studentID %>';
                var CredentialID = '<%= this.credentialID %>';

                var StudentCredentialID = '<%= this.studentCredentialID %>';
                customDialog({ url: '../Credentials/AddComment.aspx?CommentID=' + ID +'&studCrdId=' + StudentCredentialID + '&studId=' + StudentID + '&crdId=' + CredentialID, maximize: true, maxwidth: 461, maxheight: 220 });
            }            
        </script>
    </telerik:RadCodeBlock>
    <script type="text/javascript">
        function refreshCommentLink(linkid, commentscount) {            
            if (commentscount > 0) {                
                $('#' + linkid,parent.document).closest("td").find('[id$="lnkComment"]').css({ 'display': 'block', 'text-align': 'center' });
                $('#' + linkid, parent.document).closest("td").find('[id$="imgComment"]').css({ 'display': 'none', 'text-align': 'center' });                
            }
            else {
                $('#' + linkid, parent.document).closest("td").find('[id$="lnkComment"]').css({ 'display': 'none', 'text-align': 'center' });
                $('#' + linkid, parent.document).closest("td").find('[id$="imgComment"]').css({ 'display': 'block', 'text-align': 'center' });
            }
            $('#' + linkid, parent.document).closest("td").find('[id$="hdnCommentCount"]').val(commentscount);
        }
    </script>
</asp:Content>
