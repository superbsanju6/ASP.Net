<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Competencyworksheetsearch.ascx.cs" Inherits="Thinkgate.Controls.CompetencyWorksheet.Competencyworksheetsearch" %>
<style>
    .rbPrimaryIcon {
        top: 5px !important;
    }

    .labels {
        width: 50px;
        position: relative;
        float: left;
        left: 15px;
    }
</style>
<script type="text/javascript" src="../../Scripts/master.js"></script>



<asp:HiddenField runat="server" ID="hdnCmsDocumentLocation" Value="2" />
<telerik:RadAjaxPanel ID="worksheetSearchPanel" runat="server" LoadingPanelID="worksheetLoadingPanel">
    <telerik:RadScriptBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
        function clientClose() {
            var wnd = $find("<%=wndAddDocument.ClientID %>");
            wnd.close();
        }

        function openIdentificationWnd(sender, args) {
            var radbutton = $find("<%= rdoAddNew.ClientID%>");
            if (radbutton.get_checked()) {
                var urlstring = "../Controls/CompetencyWorksheet/CompetencyWorksheetIdentification.aspx?userID=" + '<%= this.session.LoggedInUser.UserId%>' + "&IsNew=1";
                parent.customDialog({ url: urlstring, name: "CompetencyWorksheetIdentification", maximize: true, maxwidth: 500, maxheight: 450, title: 'Identification' });
            }
            else {
                var urlstring = "../Controls/UnitPlans/AddExistingCompetencyList.aspx?reqtype=2&IsNew=1";
                parent.customDialog({ url: urlstring, name: "AddExistingCompetencyList", maximize: true, maxwidth: 1100, maxheight: 700, title: 'Existing Item Search' });
            }

            var wnd = $find("<%=wndAddDocument.ClientID %>");
            wnd.close();
        }
    </script>
</telerik:RadScriptBlock>
    <telerik:RadWindowManager ID="wndWindowManager" runat="server">
    <Windows>
        <telerik:RadWindow runat="server" ID="wndAddDocument"
            Title="Add New Competency Worksheet"
            ShowContentDuringLoad="False"
            Behavior="None"
            ReloadOnShow="True"
            Modal="True"
            Skin="Web20"
            VisibleStatusbar="False"
            AutoSize="True"
            AutoSizeBehaviors="Default"
            DestroyOnClose="true">
            <ContentTemplate>
                <%-- TYPE --%>
                <div runat="server" id="divTypeSubtype" visible="True" style="margin-left: auto; margin-right: auto; height: 155px; width: 375px; font-size: 12pt">
                    <div style="float: none; margin-right: 20px;">
                        <%--Type Buttons--%>
                        <br />
                        <telerik:RadButton runat="server" ID="rdoAddNew"
                            Skin="Web20" Font-Size="12pt"
                            Text="Add New Worksheet"
                            ToggleType="Radio"
                            ButtonType="ToggleButton"
                            Checked="True"
                            GroupName="CreateNewExisting"
                            AutoPostBack="False" />
                        <br />
                        <telerik:RadButton runat="server" ID="rdoAddExisting"
                            Skin="Web20" Font-Size="12pt"
                            Text="Use an Existing List"
                            ToggleType="Radio"
                            ButtonType="ToggleButton"
                            Checked="False"
                            GroupName="CreateNewExisting"
                            AutoPostBack="False" />
                        <br />
                         <div style="float: right; margin-right: 20px;">
                            <%--New or Existing buttons--%>
                            <telerik:RadButton runat="server" ID="btnOkNew"
                                Text="OK"
                                Skin="Web20"
                                OnClientClicked="openIdentificationWnd"
                                AutoPostBack="false" />
                            <telerik:RadButton runat="server" ID="btnCancelNew"
                                Text="Cancel"
                                Skin="Web20"
                                OnClientClicked="clientClose"
                                AutoPostBack="false" />

                        </div>
                    </div>
                </div>
                <%-- WHERE --%>
                <div runat="server" id="divAddWhere"
                    visible="True"
                    style="margin-left: auto; margin-right: auto; width: 375px; font-size: 12pt; display: none;">
                </div>

                <%-- NEW OR EXISTING --%>
                <div runat="server" id="divAddNewOrExisting" style="margin-left: auto; margin-right: auto; width: 375px; font-size: 12pt; display: none">
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
        <%-- Not being used? --%>
        <telerik:RadWindow runat="server" ID="wndCmsNewDocumentShell"
            AutoSize="True"
            ShowContentDuringLoad="False"
            Behaviors="Close,Move,Resize"
            ReloadOnShow="True"
            Modal="True"
            Skin="Web20"
            VisibleStatusbar="False"
            OnClientClose="OnCmsDocumentDialogClosed" />
    </Windows>
</telerik:RadWindowManager>
    <div id="searchHolder">
        <div class="searchTextDiv_smallTile">
            <input type="text" name="text" id="worksheetSearchText_smallTile" clientidmode="Static"
                class="searchStyle" runat="server" defaulttext="Search ..." onkeypress="return searchSmallTile_SubmitOnEnter(this, event);"
                onclick="if ($(this).val() == 'Search by name,description,...') { $(this).val(''); }" />
        </div>
        <div class="searchTileButtonDiv_smallTile">
            <asp:ImageButton ID="groupSearchButton_smallTile" ClientIDMode="Static" runat="server"
                ImageUrl="~/Images/go.png" OnClick="groupSearchButton_smallTile_Click"></asp:ImageButton>
        </div>
    </div>
    <div class="" style="height: auto; padding: 0px;" id="">
        <asp:Panel ID="pnlNoResults" runat="server"
            Visible="false"
            Height="100%">
            <div style="width: 50%; height: 180px; text-align: center;">
                <b>No results found.</b>
            </div>
        </asp:Panel>
        <telerik:RadListBox runat="server" ID="lbxList" Width="100%" Height="180px">
            <ItemTemplate>
                <asp:Image ID="wsimg" Style="float: left; padding: 2px;" Width="47" Height="56"
                    ImageUrl='~/Images/worksheet_icon.png' runat="server" />
                <div>
                    
                    <asp:Label runat="server" ID="lnkWSName">
                                    <a href="../CompetencyWorksheetPreview.aspx?xID=<%# Eval("ID_Encrypted") %>&classid=<%# classid %>"  target="_blank"><%# Eval("Name") %></a>
                    </asp:Label>
                    <asp:Label ID="lblDesc" runat="server"> <%=" - " %><%# Eval("description")%></asp:Label>
                    <br />
                    

                </div>
            </ItemTemplate>
        </telerik:RadListBox>
    </div>


<div runat="server" id="btnAdd" class="searchTileBtnDiv_smallTile" title="Add New"
    style="margin-top: 1px;">
    <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
    <div style="padding: 0;">
        Add New
    </div>
</div>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="worksheetLoadingPanel" runat="server" />