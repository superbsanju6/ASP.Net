<%@ Control Language="C#" CodeBehind="InstructionAssignment.ascx.cs" Inherits="Thinkgate.Controls.InstructionMaterial.InstructionAssignment" %>
<%@ Import Namespace="Thinkgate.Classes" %>


<telerik:RadScriptBlock ID="rdscblock" runat="server">
    <script type="text/javascript" lang="Javascript">
        function openInstructionSearch() {
            var urlstring = "../Controls/Resources/ResourceSearchKentico.aspx?isInstructionMaterial=1";
            parent.customDialog({ url: urlstring, name: "InstructionSearch", maximize: true, width: 900, center: true, maxwidth: 935, height: 500, maxheight: 700, title: 'Instruction Search', onClosed: CloseDialog });
        }

        function EditInstructionSearch(NodeID) {
            var ClassID = '<%= SessionObject.clickedClass == null? 0 : SessionObject.clickedClass.ID %>';
            var GroupID = '<%= SessionObject.clickedGroup == null? 0 : SessionObject.clickedGroup.ID %>';
            var urlstring = "../Controls/InstructionMaterial/ManageAssignments.aspx?DocumentNodeID=" + NodeID + "&GroupID=" + GroupID + "&ClassID=" + ClassID;
            parent.customDialog({ url: urlstring, name: "Manage Assignment", maximize: true, maxwidth: 1100, maxheight: 700, title: 'Manage Assignments', onClosed: CloseDialog });
        }
        function submitform() {
            document.getElementById('btnSearch').click()
        }

        function CloseDialog() {
            window.top.location.href = window.top.location.href;
        }


    </script>
</telerik:RadScriptBlock>

<telerik:RadCodeBlock ID="codesection" runat="server">
    <telerik:RadAjaxPanel runat="server" ID="resourcesPanel" LoadingPanelID="resourcesLoadingPanel">

        <style type="text/css">
            .searchResuls_smallTile_Items {
                text-align: left;
                vertical-align: top;
            }

            .resourceView_icon img {
                width: 15px;
            }

           .buttonAreaFloatRight {
            float: right;
            top:2px;
            left:3px;
        }

            .RadComboBox .rcbInputCell .rcbEmptyMessage {
                font-style: normal;
            }
        </style>
        <div style="width: 300px; height: 238px; overflow: hidden;">
            <div id="searchHolder">
                <div class="buttonAreaFloatLeft" style="z-index: 500">
                    <telerik:RadComboBox ID="cmbType" runat="server" AutoPostBack="false"  Text="Type" ToolTip="Select a Type" Skin="Web20" EmptyMessage="Select a type"
                        Width="70"
                        CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
                    </telerik:RadComboBox>
                    <telerik:RadComboBox ID="cmbGroups" AutoPostBack="false"  runat="server" Text="Groups" ToolTip="Select a group" Skin="Web20" EmptyMessage="Select a group"
                        Width="70"
                        CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
                    </telerik:RadComboBox>
                    <telerik:RadComboBox ID="cmbStudents" runat="server" Text="Students" AutoPostBack="false"  ToolTip="Select a student" Skin="Web20" EmptyMessage="Select a student"
                        Width="75"
                        CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
                    </telerik:RadComboBox>
                </div>
                <div class="buttonAreaFloatRight">
                    <asp:ImageButton ID="Search" runat="server"
                        ImageUrl="~/Images/go.png" OnClick="Search_Click"></asp:ImageButton>
                </div>
            </div>
            <div class="graphicalView">
                <asp:Panel ID="pnlNoResults" runat="server" Visible="false" Height="180px">
                    <div style="width: 100%; text-align: center;">
                        <br />
                        <br />
                        Currently no assignments have been established to any students in a selected Group.
                    </div>
                </asp:Panel>
                <telerik:RadListBox runat="server" ID="lbxResources" Width="100%" Height="180px">
                    <ItemTemplate>
                        <div style="top: 1%">
                            <div style="float: left; width: 20%;">
                                <img src="<%#  "../Images/resources.png" %>" style="height: 56px; width: 47px; float: left; padding: 2px;">
                            </div>
                            <div style="float: right; width: 80%;">
                                <%#  (Convert.ToDateTime( Eval("ExpirationDate")).Date< DateTime.Now.Date && expiredContentView==false)?"<span style='color:#595959;'>"+ Eval("NodeName") +"</span>"+ "<span style='color:#a3171e'> - Expired </span>" : "<a href="+KenticoHelper.KenticoVirtualFolderRelative+ Eval("NodeAliasPath")+ ".aspx?viewmode=3&isThinkgateSession=true&portalName=" + PortalName + " target='_blank' title="+ KenticoHelper.KenticoVirtualFolderRelative+ Eval("NodeAliasPath")+">" + Eval("NodeName")+ "</a>"+ ((Convert.ToDateTime( Eval("ExpirationDate")).Date< DateTime.Now.Date)? "<span style='color:#802A2A'> - Expired </span>" :"") %>
                                <%=" - " %><%# Eval("Description")%>
                                <br />
                                <span style="font-size: 85%;"><%# Eval("ExpirationDate","{0:MM/dd/yy}")=="12/31/99" ? "Expiration Date: NA" : "Expiration Date:"+ Eval("ExpirationDate","{0:MM/dd/yy}")%></span>
                                <br />

                                <%# (Convert.ToDateTime(Eval("ExpirationDate")).Date>= DateTime.Now.Date  && SessionObject.LoggedInUser.Roles.Where(x => x.RoleName.ToLower() == "teacher").Count()> 0)
                    ? "<div onclick='EditInstructionSearch(" + Eval("NodeId") + ")'> <img style='height:16px;' title='Manage Assignment' src='../Images/IMStudent_Small.png' /> </div>" : ""  %>



                            </div>
                        </div>
                    </ItemTemplate>
                </telerik:RadListBox>
            </div>

            <div runat="server" id="btnAddIM" class="searchTileBtnDiv_smallTile" title="Assign Instruction Material"
                style="margin-top: 1px;" onclick="openInstructionSearch()">
                <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
                <div style="padding: 0;">
                    Assign
                </div>
            </div>
        </div>
    </telerik:RadAjaxPanel>
</telerik:RadCodeBlock>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="resourcesLoadingPanel" runat="server" />


