<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MTSSFormsDocumentTile.ascx.cs" Inherits="Thinkgate.Controls.Documents.MTSSFormsDocumentTile" %>

<telerik:RadAjaxLoadingPanel ID="MTSSFormsAjaxLoadingPanel" runat="server" />

<script type="text/javascript">
    function addForm(sender, args) {
        var fm = $find('cmbFormName');
        var std = $find('cmbStudentName');
        var rti = $find('cmbIntervention');
        var tr = $find('cmbTier');
        var kn = $get('KenticoVirtualFolder');
        var url = '/' + kn.value + fm.get_value() + '.aspx?Student_no=' + std.get_value() + '&Issue_ID=' + rti.get_value();
        var w = window.open(url, '_blank');

        closeWindow(sender);
    }

    function closeWindow(sender) {
        var ary = [$find('cmbFormName'), $find('cmbStudentName'), $find('cmbIntervention'), $find('cmbTier')];
        resetSelections(ary);
        var wnd = window.$find(sender.get_commandArgument());
        wnd.close();
    }

    function resetSelections(ary) {
        for (i = 0; i < ary.length; i++) {
            ary[i].trackChanges();
            ary[i].get_items().getItem(0).select();
            ary[i].commitChanges();
        }
    }
    function OnClientItemsRequesting(sender, eventArgs) {
        var context = eventArgs.get_context();
        context["Tier"] = $find('cmbTier').get_value();
        context["UserPage"] = $get('userpage').value;
    }

    function callWebService() {
        var combo = $find('cmbIntervention');
        combo.requestItems("Item1", false);

        combo.trackChanges();
        combo.get_items().getItem(0).select();
        combo.commitChanges();
    }

 </script>

<asp:HiddenField runat="server" ID="userpage" ClientIDMode="static" />
<asp:HiddenField runat="server" ID="KenticoVirtualFolder" ClientIDMode="static" />

<telerik:RadWindowManager ID="wndWindowManager" runat="server">
    <Windows>
        <telerik:RadWindow runat="server" 
            ID="wndAddDocument"
            Title="RTI Forms" 
            ShowContentDuringLoad="False"
            Behavior="Close" 
            ReloadOnShow="True" 
            Modal="True" 
            Skin="Web20" 
            VisibleStatusbar="False" 
            AutoSize="True" 
            AutoSizeBehaviors="Default"
            DestroyOnClose="true"
            ZIndex="1000">
            <ContentTemplate>
                <div runat="server" id="divAddWhere" Visible="True" style="margin-left: auto; margin-right: auto; width: 500px; font-size: 12pt">
                </br>
                    <table>
                        <tr style="padding-bottom:50px;padding-top:50px">
                            <td style="text-align:right;padding-right:30px;padding-left:10px">Form Name:</td>
                            <td>
                                <telerik:RadComboBox ID="cmbFormName" runat="server" Width="300px" Skin="Web20" ClientIDMode="static" ZIndex="10000" AppendDataBoundItems="true">
                                    <Items>
                                       <telerik:RadComboBoxItem Text="Select a Form" />
                                    </Items>
                                </telerik:RadComboBox></td>
                        </tr>
                        <tr>
                            <td style="height:30px"></td>
                        </tr>
                        <tr style="padding-bottom:50px;padding-top:50px">
                            <td style="text-align:right;padding-right:30px;padding-left:10px">Student Name:</td>
                            <td>
                                <telerik:RadComboBox runat="server" Width="300px" ID="cmbStudentName" Skin="Web20"  ClientIDMode="static" ZIndex="10000" AppendDataBoundItems="true">
                                    <Items>
                                       <telerik:RadComboBoxItem Text="Select a Student" />
                                    </Items>
                                </telerik:RadComboBox></td>
                        </tr>
                        <tr>
                            <td style="height:30px"></td>
                        </tr>
                        <tr style="padding-bottom:50px;padding-top:50px">
                            <td style="text-align:right;padding-right:30px;padding-left:10px">Tier:</td>
                            <td>
                                <telerik:RadComboBox runat="server" Width="300px" ID="cmbTier" Skin="Web20"  ClientIDMode="static" 
                                                     ZIndex="10000" AppendDataBoundItems="true" OnClientSelectedIndexChanged="callWebService">
                                    <Items>
                                       <telerik:RadComboBoxItem Text="Select a Tier" />
                                       <telerik:RadComboBoxItem Text="Analysis" Value="Analysis"/>
                                       <telerik:RadComboBoxItem Text="Tier 2" Value="Tier 2"/>
                                       <telerik:RadComboBoxItem Text="Tier 3" Value="Tier 3"/>
                                    </Items>
                                </telerik:RadComboBox></td>
                        </tr>
                        <tr>
                            <td style="height:30px"></td>
                        </tr>
                        <tr style="padding-bottom:50px;padding-top:50px"">
                            <td style="text-align:right;padding-right:30px;padding-left:10px">RTI Alignment:</td>
                            <td>
                                <telerik:RadComboBox runat="server" Width="300px" ID="cmbIntervention" ClientIDMode="static" ZIndex="10000" 
                                                     OnClientItemsRequesting="OnClientItemsRequesting" EnableLoadOnDemand="false" Skin="Web20"
                                                     WebServiceSettings-Path="~/Services/KenticoCMSRequests.asmx" AppendDataBoundItems="true"
                                                     WebServiceSettings-Method="GetMTSSInterventions" AllowCustomText="false" MarkFirstMatch="false" >
                                 <Items>
                                       <telerik:RadComboBoxItem Text="Select an Alignment"/>
                                </Items>
                                </telerik:RadComboBox>
                           </td>
                        </tr>
                    </table>
               </br>
                <div style="float: right; margin-right: 20px; height:50px">
                    <telerik:RadButton 
                        runat="server"
                        ID="btnOkWhere" 
                        Text="OK" 
                        Skin="Web20"
                        AutoPostBack="false"
                        OnClientClicked="addForm"
                    />
                    <telerik:RadButton 
                        runat="server" 
                        ID="btnCancelWhere"
                        Text="Cancel" 
                        Skin="Web20" 
                        AutoPostBack="false"
                        OnClientClicked="closeWindow"
                    />
                </div>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
      </Windows>
</telerik:RadWindowManager>
<telerik:RadAjaxPanel ID="MTSSFormsAjaxPanel" runat="server" LoadingPanelID="MTSSFormsAjaxLoadingPanel" Height="209px">
    <telerik:RadMultiPage runat="server" ID="RadMultiPageTemplates" SelectedIndex="0" CssClass="multiPage">  
         <telerik:RadPageView runat="server" ID="TemplatesView">
                    <telerik:RadListBox runat="server" ID="MTSSFormsList" Height="208px" width="100%">
                        <ItemTemplate>
                            <div>
                              <a href="/<%=KenticoVirtualFolder.Value%><%# Eval("NodeAliasPath")%>.aspx?viewmode=3" target="_blank"><%# Eval("NodeName")%></a>
                              <br />
                            </div>
                        </ItemTemplate>
                    </telerik:RadListBox>
         </telerik:RadPageView>
         <telerik:RadPageView runat="server" ID="UnAligned" BackColor="#aeaeac">
                <telerik:RadComboBox ID="MTSSFormscmbUnaligned" runat="server"
                    ToolTip="Sort By Type"
                    Skin="Web20"
                    Width="250"
                    DropDownWidth="250"
                    CausesValidation="False"
                    HighlightTemplatedItems="true"
                    AutoPostBack="True"
                    OnSelectedIndexChanged="cmbUnaligned_SelectedIndexChanged">
                    <Items>  
                        <telerik:RadComboBoxItem runat="server" Text="Forms Without Interventions" Value="RTI" />   
                        <telerik:RadComboBoxItem runat="server" Text="Students Without Forms" Value="KENTICO"/>   
                   </Items>
               </telerik:RadComboBox>
               <asp:Panel runat="server" ID="MTSSFormsUnalignedPanel" Height="185px" ScrollBars="Auto" Style="margin-left:10px;margin-right:10px">
                    <asp:PlaceHolder runat="server" ID="SortByFormsPlaceHolder"></asp:PlaceHolder>
               </asp:Panel>
     </telerik:RadPageView>
         <telerik:RadPageView runat="server" ID="MTSSForms504">
                    <telerik:RadListBox runat="server" ID="Forms504List" Height="208px" width="100%">
                        <ItemTemplate>
                            <div>
                              <a href="/<%=KenticoVirtualFolder.Value%><%# Eval("NodeAliasPath")%>.aspx?viewmode=3" target="_blank"><%# Eval("NodeName")%></a>
                              <br />
                            </div>
                        </ItemTemplate>
                    </telerik:RadListBox>
         </telerik:RadPageView>
     </telerik:RadMultiPage>
</telerik:RadAjaxPanel>
<div>
    <telerik:RadTabStrip runat="server" ID="RadTabStrip2" Orientation="HorizontalBottom" SelectedIndex="0" MultiPageID="RadMultiPageTemplates" 
                         Skin="Thinkgate_Blue" EnableEmbeddedSkins="False" Style="float: left">
        <Tabs>
            <telerik:RadTab Text="Templates" runat="server" ID="TabTemplatesView" Font-Size="X-Small" Visible="false"></telerik:RadTab>
            <telerik:RadTab Text="Unaligned" runat="server" ID="TabUnAlignedView" Font-Size="X-Small" Visible="false"></telerik:RadTab>
            <telerik:RadTab Text="504" runat="server" ID="Tab504Forms" Font-Size="X-Small" Visible="false"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <div runat="server" id="btnAdd" class="searchTileBtnDiv_smallTile" title="Add New" style="margin-top: 1px;float:right;" >
        <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
        <div style="padding: 0;">Add New</div>
    </div>
</div>